using Newtonsoft.Json;

namespace FlowState.Server
{
    internal sealed class SafeLogEvent
    {
        [JsonProperty("environment")]
        internal string Environment { get; set; }

        [JsonProperty("moduleReference")]
        internal string ModuleReference { get; set; }

        [JsonProperty("outcome")]
        internal string Outcome { get; set; }

        [JsonProperty("reason")]
        internal string Reason { get; set; }

        [JsonProperty("durationBucket")]
        internal int DurationBucket { get; set; }

        [JsonProperty("correlationId")]
        internal string CorrelationId { get; set; }

        [JsonProperty("rateLimitResult")]
        internal string RateLimitResult { get; set; }
    }
}
