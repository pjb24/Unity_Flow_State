using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.CloudCode;

namespace FlowState.Runtime.Features
{
    public sealed class UgsOnlineRecordTransport : IOnlineRecordTransport, IOnlineAccountTransport
    {
        private const int ResponseTimeoutMilliseconds = 5000;
        public const string VerificationModuleName = "FlowStateVerification";
        private int _requestSequence;
        public string LastDiagnostic { get; private set; } = "ModulePhase=NotRequested";
        public Task<T> CallAsync<T>(string endpoint, string request)
        {
            return CallEndpointAsync<T>(endpoint, new Dictionary<string, object> { { "request", request } });
        }

        public Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            if (parameters != null)
                foreach (KeyValuePair<string, string> parameter in parameters) arguments.Add(parameter.Key, parameter.Value);
            return CallEndpointAsync<T>(endpoint, arguments);
        }

        private async Task<T> CallEndpointAsync<T>(string endpoint, Dictionary<string, object> arguments)
        {
            string function = GetModuleFunctionName(endpoint);
            int sequence = ++_requestSequence;
            bool clientTimedOut = false;
            Stopwatch elapsed = Stopwatch.StartNew();
            SetDiagnostic(sequence, function, "RequestStarted", 0, elapsed);
            try
            {
                Task<T> call = CloudCodeService.Instance.CallModuleEndpointAsync<T>(VerificationModuleName, function, arguments);
                if (await Task.WhenAny(call, Task.Delay(ResponseTimeoutMilliseconds)) != call)
                {
                    SetDiagnostic(sequence, function, "ClientTimeout", 0, elapsed);
                    clientTimedOut = true;
                    ObserveLateResultAsync(call, sequence, function, elapsed);
                    throw new TimeoutException("Cloud Code response timed out.");
                }
                T result = await call;
                SetDiagnostic(sequence, function, "ResponseReceived", 0, elapsed, result as OnlineAccountResponse);
                return result;
            }
            catch (TimeoutException)
            {
                if (!clientTimedOut) SetDiagnostic(sequence, function, "SdkTimeout", 0, elapsed);
                throw;
            }
            catch (Exception error)
            {
                SetDiagnostic(sequence, function, "RequestFailed", GetErrorCode(error), elapsed);
                throw;
            }
        }

        private void SetDiagnostic(int sequence, string function, string phase, int errorCode,
            Stopwatch elapsed, OnlineAccountResponse response = null)
        {
            if (sequence != _requestSequence) return;
            LastDiagnostic = "Module=" + VerificationModuleName + " / Function=" + function +
                " / ModulePhase=" + phase + " / SDKErrorCode=" + errorCode +
                " / TimeoutMs=" + ResponseTimeoutMilliseconds + " / RequestOrdinal=" + sequence +
                " / ClientElapsedMs=" + elapsed.ElapsedMilliseconds +
                " / ServerElapsedMs=" + (response == null || !response.serverTimingPresent ? -1 : response.serverElapsedMilliseconds) +
                " / ServerServiceCalls=" + (response == null || !response.serverTimingPresent ? -1 : response.serverServiceCalls);
        }

        private static int GetErrorCode(Exception error)
        {
            RequestFailedException requestError = error as RequestFailedException;
            return requestError == null ? 0 : requestError.ErrorCode;
        }

        public static string GetModuleFunctionName(string endpoint)
        {
            switch (endpoint)
            {
                case "get-public-player-number": return "GetPublicPlayerNumber";
                case "get-account-transfer-status": return "GetAccountTransferStatus";
                case "get-account-personal-bests": return "GetAccountPersonalBests";
                case "start-account-transfer": return "StartAccountTransfer";
                case "reissue-account-transfer": return "ReissueAccountTransfer";
                case "cancel-account-transfer": return "CancelAccountTransfer";
                case "complete-account-transfer": return "CompleteAccountTransfer";
                case "submit-record": return "SubmitRecord";
                case "query-records": return "QueryRecords";
                default: throw new ArgumentException("Unknown verification endpoint.", nameof(endpoint));
            }
        }

        private async void ObserveLateResultAsync<T>(Task<T> call, int sequence, string function, Stopwatch elapsed)
        {
            // Timeout does not cancel a remote commit. The ledger makes the next retry safe.
            try
            {
                T result = await call;
                // Timing is diagnostic only. Never apply late account/record state.
                SetDiagnostic(sequence, function, "LateResponseReceived", 0, elapsed, result as OnlineAccountResponse);
            }
            catch (Exception error)
            {
                SetDiagnostic(sequence, function, "LateRequestFailed", GetErrorCode(error), elapsed);
                UnityEngine.Debug.LogWarning("[UgsOnlineRecordTransport] Late request failed. SDKErrorCode=" + GetErrorCode(error));
            }
        }
    }
}
