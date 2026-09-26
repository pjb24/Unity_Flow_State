namespace FlowState.Runtime.Features
{
    public sealed class RecordLeaderboardEntry
    {
        public RecordSubmissionCandidate Candidate { get; }

        public long ServerAcceptedAtTicks { get; }

        public RecordLeaderboardEntry(
            RecordSubmissionCandidate candidate,
            long serverAcceptedAtTicks)
        {
            Candidate = candidate;
            ServerAcceptedAtTicks = serverAcceptedAtTicks;
        }
    }
}
