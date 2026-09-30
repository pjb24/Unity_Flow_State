namespace FlowState.Runtime.Features
{
    // Result owns its local result state; online best remains a separately queried value.
    public sealed class ResultRecordPresentation
    {
        public bool IsNewLocalBest { get; }
        public RecordSubmissionCandidate Candidate { get; }
        public E_ResultSubmissionState SubmissionState { get; }
        public string RejectionReason { get; }
        public OnlineLeaderboardResult OnlineBest { get; }

        public ResultRecordPresentation(bool isNewLocalBest,
            RecordSubmissionCandidate candidate,
            E_ResultSubmissionState submissionState,
            string rejectionReason,
            OnlineLeaderboardResult onlineBest)
        {
            IsNewLocalBest = isNewLocalBest;
            Candidate = candidate;
            SubmissionState = submissionState;
            RejectionReason = rejectionReason == null ? string.Empty : rejectionReason;
            OnlineBest = onlineBest;
        }
    }
}
