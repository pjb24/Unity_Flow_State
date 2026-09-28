using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;

namespace FlowState.Runtime.Features
{
    public sealed class UgsOnlineRecordTransport : IOnlineRecordTransport
    {
        private const int ResponseTimeoutMilliseconds = 15000;
        public async Task<T> CallAsync<T>(string endpoint, string request)
        {
            Task<T> call = CloudCodeService.Instance.CallEndpointAsync<T>(endpoint,
                new Dictionary<string, object> { { "request", request } });
            if (await Task.WhenAny(call, Task.Delay(ResponseTimeoutMilliseconds)) != call)
            {
                ObserveLateResultAsync(call);
                throw new TimeoutException("Cloud Code response timed out.");
            }
            return await call;
        }

        private static async void ObserveLateResultAsync<T>(Task<T> call)
        {
            // Timeout does not cancel a remote commit. The ledger makes the next retry safe.
            try { await call; }
            catch (Exception) { UnityEngine.Debug.LogWarning("[UgsOnlineRecordTransport] Late request failed."); }
        }
    }
}
