using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Services.CloudCode.Core;

namespace FlowState.Server
{
    internal sealed class ServerServices : IDisposable
    {
        internal const string ProjectId = "c76d55cf-7846-494b-9dce-a0797b179b36";
        internal const string EnvironmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
        internal const int RequestBudgetMilliseconds = 5000;
        private const int MaximumResponseBytes = 8_000_000;
        // Reuse connections across invocations, but never share Player/service
        // credentials or per-invocation cancellation on DefaultRequestHeaders.
        private static readonly HttpClient SharedHttp = new HttpClient(new SocketsHttpHandler {
            AllowAutoRedirect = false, PooledConnectionLifetime = TimeSpan.FromMinutes(5) });
        private readonly string _serviceToken;
        private readonly Stopwatch _clock;
        private readonly CancellationTokenSource _deadline;

        internal ServerServices(IExecutionContext context)
        {
            _clock = Stopwatch.StartNew();
            _deadline = new CancellationTokenSource(RequestBudgetMilliseconds);
            _serviceToken = context.ServiceToken;
        }

        internal int RemainingMilliseconds
        {
            get { return Math.Max(0, RequestBudgetMilliseconds - (int)_clock.ElapsedMilliseconds); }
        }

        internal CancellationToken DeadlineToken { get { return _deadline.Token; } }
        internal int ElapsedMilliseconds { get { return (int)_clock.ElapsedMilliseconds; } }
        internal int ServiceCalls { get; private set; }

        internal void ThrowIfExpired()
        {
            if (RemainingMilliseconds <= 0 || DeadlineToken.IsCancellationRequested)
                throw new TimeoutException("Server response budget exhausted.");
        }

        internal string Call(string method, string argumentsJson)
        {
            try
            {
                ThrowIfExpired();
                JArray args = JArray.Parse(argumentsJson);
                if ((string)args[0] != ProjectId) return Failure(403);
                string target = Uri.EscapeDataString((string)args[1]);
                string path;
                string verb = "GET";
                JToken body = null;
                string saveRoot = "https://cloud-save.services.api.unity.com/v1/data/projects/" + ProjectId;
                string boardRoot = "https://leaderboards.services.api.unity.com/v1/projects/" + ProjectId + "/leaderboards/";
                switch (method)
                {
                    case "getPrivateCustomItems":
                        path = saveRoot + "/custom/" + target + "/private/items" + Keys((JArray)args[2]); break;
                    case "getProtectedItems":
                        path = saveRoot + "/players/" + target + "/protected/items" + Keys((JArray)args[2]); break;
                    case "setPrivateCustomItem":
                        path = saveRoot + "/custom/" + target + "/private/items"; verb = "POST"; body = args[2]; break;
                    case "setPrivateCustomItemBatch":
                        path = saveRoot + "/custom/" + target + "/private/item-batch"; verb = "POST"; body = args[2]; break;
                    case "setProtectedItem":
                        path = saveRoot + "/players/" + target + "/protected/items"; verb = "POST"; body = args[2]; break;
                    case "setProtectedItemBatch":
                        path = saveRoot + "/players/" + target + "/protected/item-batch"; verb = "POST"; body = args[2]; break;
                    case "getLeaderboardScores":
                        CheckBoard((string)args[1]);
                        path = boardRoot + target + "/scores?offset=" + (int)args[2] + "&limit=" + (int)args[3] + "&includeMetadata=true"; break;
                    case "getLeaderboardPlayerScore":
                        CheckBoard((string)args[1]);
                        path = boardRoot + target + "/scores/players/" + Uri.EscapeDataString((string)args[2]) + "?includeMetadata=true"; break;
                    case "addLeaderboardPlayerScore":
                        CheckBoard((string)args[1]);
                        path = boardRoot + target + "/scores/players/" + Uri.EscapeDataString((string)args[2]);
                        verb = "POST"; body = args[3]; break;
                    default: return Failure(403);
                }
                int remaining = RemainingMilliseconds;
                if (remaining <= 0) return Failure(503);
                using HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(verb), path);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceToken);
                if (body != null) request.Content = new StringContent(body.ToString(Formatting.None), Encoding.UTF8, "application/json");
                // No automatic retries: a timed-out CAS/write may already have committed.
                ServiceCalls++;
                using HttpResponseMessage response = SharedHttp.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    DeadlineToken).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return Failure((int)response.StatusCode);
                if (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value > MaximumResponseBytes)
                    return Failure(503);
                using System.IO.Stream input = response.Content.ReadAsStreamAsync(DeadlineToken).GetAwaiter().GetResult();
                using System.IO.MemoryStream output = new System.IO.MemoryStream();
                byte[] buffer = new byte[8192];
                int count;
                while ((count = input.ReadAsync(buffer.AsMemory(), DeadlineToken).GetAwaiter().GetResult()) > 0)
                {
                    if (output.Length + count > MaximumResponseBytes) return Failure(503);
                    output.Write(buffer, 0, count);
                }
                string json = Encoding.UTF8.GetString(output.ToArray());
                JToken data = string.IsNullOrWhiteSpace(json) ? new JObject() : JToken.Parse(json);
                ThrowIfExpired();
                return JsonConvert.SerializeObject(new { ok = true, value = new { data } });
            }
            catch (Exception) { return Failure(503); }
        }

        public void Dispose() { _deadline.Dispose(); }

        private static string Keys(JArray keys)
        {
            string result = "";
            foreach (JToken key in keys)
            {
                result += result.Length == 0 ? "?" : "&";
                result += "keys=" + Uri.EscapeDataString((string)key);
            }
            return result;
        }

        private static void CheckBoard(string board)
        {
            if (board != "fs-stage-stage-001-r1" && board != "fs-infinite-v2")
                throw new InvalidOperationException("Invalid board.");
        }

        private static string Failure(int status)
        { return JsonConvert.SerializeObject(new { ok = false, status }); }
    }
}
