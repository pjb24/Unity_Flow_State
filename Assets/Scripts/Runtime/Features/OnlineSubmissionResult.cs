namespace FlowState.Runtime.Features
{
    // Terminal rejections retain the server reason so Result can explain why retry is unavailable.
    public sealed class OnlineSubmissionResult
    {
        public E_RecordSubmissionResult Result { get; }
        public string Reason { get; }

        public OnlineSubmissionResult(E_RecordSubmissionResult result, string reason = "")
        {
            Result = result;
            Reason = reason == null ? string.Empty : reason;
        }
    }
}
