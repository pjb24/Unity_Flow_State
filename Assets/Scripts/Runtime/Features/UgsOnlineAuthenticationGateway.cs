using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class UgsOnlineAuthenticationGateway : IOnlineAuthenticationGateway
    {
        private static Task _initialization;
        private static readonly System.Threading.SemaphoreSlim AuthenticationLock =
            new System.Threading.SemaphoreSlim(1, 1);

        public async Task<OnlineAuthenticationResult> TryAuthenticateAsync()
        {
            await AuthenticationLock.WaitAsync();
            try
            {
                if (Application.cloudProjectId != OnlineRecordConfiguration.ProjectId)
                {
                    throw new InvalidOperationException();
                }

                // Refuse externally initialized services: their environment is unverified.
                if (_initialization == null)
                {
                    if (UnityServices.State != ServicesInitializationState.Uninitialized)
                    {
                        throw new InvalidOperationException();
                    }
                    InitializationOptions options = new InitializationOptions();
                    options.SetEnvironmentName(
                        OnlineRecordConfiguration.VerificationEnvironmentName);
                    options.SetProfile("flow-state-verification");
                    _initialization = UnityServices.InitializeAsync(options);
                }
                await _initialization;

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                return new OnlineAuthenticationResult(
                    AuthenticationService.Instance.IsSignedIn,
                    AuthenticationService.Instance.PlayerId);
            }
            catch (Exception)
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    _initialization = null;
                }
                Debug.LogWarning(
                    "[UgsOnlineAuthenticationGateway] Anonymous authentication failed.");
                return new OnlineAuthenticationResult(false, string.Empty);
            }
            finally
            {
                AuthenticationLock.Release();
            }
        }
    }
}
