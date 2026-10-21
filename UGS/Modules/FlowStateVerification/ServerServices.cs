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
        internal string ProjectId { get; }
        internal string EnvironmentId { get; }
        internal string EnvironmentName { get; private set; }
        internal const string EnvironmentNameSecretName = "FS_ENVIRONMENT_NAME_V1";
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
            if (context == null || string.IsNullOrWhiteSpace(context.ProjectId) ||
                string.IsNullOrWhiteSpace(context.EnvironmentId))
                throw new InvalidOperationException("ContextMismatch");
            ProjectId = context.ProjectId;
            EnvironmentId = context.EnvironmentId;
            EnvironmentName = NormalizeEnvironmentName(context.EnvironmentName);
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

        private static string NormalizeEnvironmentName(string name)
        {
            string normalized = name == null ? null : name.Trim().ToLowerInvariant();
            return normalized == "verification" || normalized == "production" ? normalized : null;
        }

        // Scope still comes exclusively from this invocation's Project/Environment ID.
        // If optional display-name metadata is unusable, read the label from a Secret
        // in that SAME scope. Cache only on this per-invocation instance.
        internal void EnsureEnvironmentName(Func<string, int, string> getSecret, int remainingMilliseconds)
        {
            ThrowIfExpired();
            if (EnvironmentName != null) return;
            int remaining = Math.Min(remainingMilliseconds, RemainingMilliseconds);
            if (remaining <= 0) throw new TimeoutException("Server response budget exhausted.");
            string configuredName = getSecret(EnvironmentNameSecretName, remaining);
            ThrowIfExpired();
            string normalized = NormalizeEnvironmentName(configuredName);
            if (normalized == null) throw new InvalidOperationException("EnvironmentConfigurationUnavailable");
            EnvironmentName = normalized;
        }

        // Only trusted Cloud Code context selects names. Preserve the deployed
        // verification secrets without reading, copying or rotating their values.
        internal string ResolveSecretName(string logicalName)
        {
            if (EnvironmentName != "verification" && EnvironmentName != "production")
                throw new InvalidOperationException("ContextMismatch");
            switch (logicalName)
            {
                case "FS_TRANSFER_HMAC_V1":
                    return EnvironmentName == "verification" ? "FS_TRANSFER_HMAC_VERIFICATION_V1" : logicalName;
                case "FS_LOG_SINK_HMAC_V1":
                    return EnvironmentName == "verification" ? "FS_LOG_SINK_HMAC_VERIFICATION_V1" : logicalName;
                case "FS_LOG_SINK_URL_V1":
                    return EnvironmentName == "verification" ? "FS_LOG_SINK_URL_VERIFICATION_V1" : logicalName;
                default:
                    throw new InvalidOperationException("UnsupportedSecret");
            }
        }

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
                string project = Uri.EscapeDataString(ProjectId);
                string saveRoot = "https://cloud-save.services.api.unity.com/v1/data/projects/" + project;
                string boardRoot = "https://leaderboards.services.api.unity.com/v1/projects/" + project + "/leaderboards/";
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
                    case "deletePrivateCustomItem":
                        path = saveRoot + "/custom/" + target + "/private/items/" + Uri.EscapeDataString((string)args[2]);
                        if (args.Count > 3 && args[3].Type == JTokenType.String && !string.IsNullOrEmpty((string)args[3]))
                            path += "?writeLock=" + Uri.EscapeDataString((string)args[3]);
                        verb = "DELETE"; break;
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
                    case "getLeaderboardPlayerRange":
                        CheckBoard((string)args[1]);
                        path = boardRoot + target + "/scores/players/" + Uri.EscapeDataString((string)args[2]) +
                            "/range?rangeLimit=" + ReadRangeLimit(args) + "&includeMetadata=true"; break;
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

        // The JavaScript SDK receives an options object, not a positional
        // integer: { params: { rangeLimit: 3, includeMetadata: true } }.
        private static int ReadRangeLimit(JArray args)
        {
            JObject options = args.Count > 3 ? args[3] as JObject : null;
            JObject parameters = options == null ? null : options["params"] as JObject;
            JToken value = parameters == null ? null : parameters["rangeLimit"];
            if (value == null || value.Type != JTokenType.Integer) throw new InvalidOperationException("Invalid range limit.");
            int rangeLimit = (int)value;
            if (rangeLimit < 0 || rangeLimit > 3) throw new InvalidOperationException("Invalid range limit.");
            return rangeLimit;
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
