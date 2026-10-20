using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace FlowState.Server
{
    // This boundary owns the only external telemetry request. It never receives
    // a player value, request body, response body, or service token.
    internal sealed class SafeLogSinkClient
    {
        internal const string LogSinkHmacSecretName = "FS_LOG_SINK_HMAC_VERIFICATION_V1";
        internal const string LogSinkUrlSecretName = "FS_LOG_SINK_URL_VERIFICATION_V1";
        internal const int NormalTelemetryMilliseconds = 250;
        internal const int SyntheticProbeTelemetryMilliseconds = 4000;
        private const int MaximumDurationBucketMilliseconds = 5000;
        private const string LogSinkPath = "/v1/events";
        private static readonly HttpClient SharedHttp = new HttpClient(new SocketsHttpHandler {
            AllowAutoRedirect = false, PooledConnectionLifetime = TimeSpan.FromMinutes(5) });
        private readonly ServerServices _services;
        private readonly Func<string, int, string> _getSecret;
        private readonly int _maximumTelemetryMilliseconds;

        internal SafeLogSinkClient(ServerServices services, Func<string, int, string> getSecret,
            int maximumTelemetryMilliseconds)
        {
            _services = services;
            _getSecret = getSecret;
            _maximumTelemetryMilliseconds = maximumTelemetryMilliseconds;
        }

        internal bool TryEmit(string outcome, string reason, string rateLimitResult)
        {
            try
            {
                Stopwatch telemetryClock = Stopwatch.StartNew();
                string sinkUrl = ReadSecret(LogSinkUrlSecretName, telemetryClock);
                string hmacSecret = ReadSecret(LogSinkHmacSecretName, telemetryClock);
                Uri endpoint = CreateEndpoint(sinkUrl);
                if (endpoint == null || string.IsNullOrEmpty(hmacSecret) || hmacSecret.Length < 32)
                    throw new InvalidOperationException("SafeLogSinkConfigurationUnavailable");

                SafeLogEvent safeEvent = new SafeLogEvent {
                    Environment = "verification",
                    ModuleReference = "FlowStateVerification",
                    Outcome = outcome,
                    Reason = reason,
                    DurationBucket = GetDurationBucket(_services.ElapsedMilliseconds),
                    CorrelationId = Guid.NewGuid().ToString("N"),
                    RateLimitResult = rateLimitResult
                };
                string body = JsonConvert.SerializeObject(safeEvent, Formatting.None);
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);
                string signature = CreateSignature(hmacSecret, timestamp, body);
                int remaining = GetRemainingTelemetryMilliseconds(telemetryClock);
                if (remaining <= 0) throw new TimeoutException("SafeLogSinkBudgetExhausted");

                using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(_services.DeadlineToken);
                timeout.CancelAfter(remaining);
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("X-Flow-State-Timestamp", timestamp);
                request.Headers.Add("X-Flow-State-Signature", signature);
                request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(body));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using HttpResponseMessage response = SharedHttp.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    timeout.Token).GetAwaiter().GetResult();
                if (response.StatusCode != HttpStatusCode.Accepted)
                    throw new InvalidOperationException("SafeLogSinkRejected");
                return true;
            }
            catch (Exception)
            {
                // The message is intentionally constant: sink failures must be visible
                // without exposing configuration, credentials, or event content.
                Console.Error.WriteLine("SafeLogSinkUnavailable");
                return false;
            }
        }

        private string ReadSecret(string name, Stopwatch telemetryClock)
        {
            int remaining = GetRemainingTelemetryMilliseconds(telemetryClock);
            if (remaining <= 0) throw new TimeoutException("SafeLogSinkBudgetExhausted");
            return _getSecret(name, remaining);
        }

        private int GetRemainingTelemetryMilliseconds(Stopwatch telemetryClock)
        {
            int moduleRemaining = _services.RemainingMilliseconds;
            int telemetryRemaining = _maximumTelemetryMilliseconds - (int)telemetryClock.ElapsedMilliseconds;
            return Math.Max(0, Math.Min(moduleRemaining, telemetryRemaining));
        }

        private static Uri CreateEndpoint(string configuredUrl)
        {
            if (string.IsNullOrEmpty(configuredUrl) || configuredUrl != configuredUrl.Trim()) return null;
            Uri baseUri;
            if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out baseUri) ||
                baseUri.Scheme != Uri.UriSchemeHttps || !baseUri.IsDefaultPort ||
                !string.IsNullOrEmpty(baseUri.UserInfo) || !string.IsNullOrEmpty(baseUri.Query) ||
                !string.IsNullOrEmpty(baseUri.Fragment) ||
                (baseUri.AbsolutePath != "/" && baseUri.AbsolutePath != "") ||
                !baseUri.Host.EndsWith(".run.app", StringComparison.Ordinal)) return null;
            return new Uri(baseUri, LogSinkPath);
        }

        private static int GetDurationBucket(int elapsedMilliseconds)
        {
            int bounded = Math.Max(0, Math.Min(MaximumDurationBucketMilliseconds, elapsedMilliseconds));
            return bounded / 250 * 250;
        }

        private static string CreateSignature(string hmacSecret, string timestamp, string body)
        {
            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacSecret));
            byte[] data = Encoding.UTF8.GetBytes(timestamp + "." + body);
            return Convert.ToBase64String(hmac.ComputeHash(data));
        }
    }
}
