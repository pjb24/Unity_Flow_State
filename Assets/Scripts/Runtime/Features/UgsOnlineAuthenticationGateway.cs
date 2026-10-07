using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class UgsOnlineAuthenticationGateway : IOnlineAuthenticationSession
    {
        private static Task _initialization;
        private static string _initializedProfile;
        private readonly string _profile;
        public string LastDiagnostic { get; private set; } = "AuthPhase=NotAttempted / AuthFailure=None";

        public UgsOnlineAuthenticationGateway(string profile = "flow-state-verification")
        {
            _profile = profile;
        }
        private static readonly System.Threading.SemaphoreSlim AuthenticationLock =
            new System.Threading.SemaphoreSlim(1, 1);

        public async Task<bool> TryClearSessionAsync(string expectedPlayerId)
        {
            await AuthenticationLock.WaitAsync();
            try
            {
                if (_initialization == null || _initializedProfile != _profile || string.IsNullOrEmpty(expectedPlayerId)) return false;
                await _initialization;
                if (AuthenticationService.Instance.Profile != _profile) return false;
                if (!AuthenticationService.Instance.IsSignedIn && !AuthenticationService.Instance.SessionTokenExists)
                    return true;
                if (AuthenticationService.Instance.PlayerId != expectedPlayerId) return false;
                AuthenticationService.Instance.SignOut(true);
                AuthenticationService.Instance.ClearSessionToken();
                return true;
            }
            catch (Exception) { return false; }
            finally { AuthenticationLock.Release(); }
        }

        public async Task<OnlineAuthenticationResult> TryAuthenticateAsync()
        {
            System.Diagnostics.Stopwatch elapsed = System.Diagnostics.Stopwatch.StartNew();
            await AuthenticationLock.WaitAsync();
            string phase = "ScopeCheck";
            string failure = "SdkFailure";
            try
            {
                if (Application.cloudProjectId != OnlineRecordConfiguration.ProjectId)
                {
                    failure = "ProjectMismatch";
                    throw new InvalidOperationException();
                }
                if (_profile != "flow-state-verification" && _profile != "flow-state-phase2-a" &&
                    _profile != "flow-state-phase2-b" && _profile != "flow-state-phase3-new")
                {
                    failure = "UnsupportedProfile";
                    throw new InvalidOperationException();
                }
                if (_initialization != null && _initializedProfile != _profile)
                { failure = "InitializedProfileMismatch"; throw new InvalidOperationException(); }

                // Refuse externally initialized services: their environment is unverified.
                if (_initialization == null)
                {
                    if (UnityServices.State != ServicesInitializationState.Uninitialized)
                    {
                        failure = "ExternalServicesInitialization";
                        throw new InvalidOperationException();
                    }
                    phase = "ServicesInitialize";
                    InitializationOptions options = new InitializationOptions();
                    options.SetEnvironmentName(
                        OnlineRecordConfiguration.VerificationEnvironmentName);
                    if (_profile == "flow-state-verification") options.SetProfile("flow-state-verification");
                    else options.SetProfile(_profile);
                    _initializedProfile = _profile;
                    _initialization = UnityServices.InitializeAsync(options);
                }
                phase = "ServicesInitialize";
                await _initialization;

                phase = "ProfileCheck";
                if (AuthenticationService.Instance.Profile != _profile)
                { failure = "SdkProfileMismatch"; throw new InvalidOperationException(); }
                phase = "AnonymousSignIn";
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                LastDiagnostic = "AuthPhase=Complete / AuthFailure=None / SDKState=" + UnityServices.State +
                    " / AuthElapsedMs=" + elapsed.ElapsedMilliseconds;
                return new OnlineAuthenticationResult(
                    AuthenticationService.Instance.IsSignedIn,
                    AuthenticationService.Instance.PlayerId);
            }
            catch (Exception error)
            {
                RequestFailedException requestError = error as RequestFailedException;
                int errorCode = requestError == null ? 0 : requestError.ErrorCode;
                LastDiagnostic = "AuthPhase=" + phase + " / AuthFailure=" + failure +
                    " / SDKState=" + UnityServices.State + " / SDKErrorCode=" + errorCode +
                    " / AuthElapsedMs=" + elapsed.ElapsedMilliseconds;
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    _initialization = null;
                }
                Debug.LogWarning(
                    "[UgsOnlineAuthenticationGateway] Anonymous authentication failed. " + LastDiagnostic);
                return new OnlineAuthenticationResult(false, string.Empty);
            }
            finally
            {
                AuthenticationLock.Release();
            }
        }
    }
}
