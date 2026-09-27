namespace FlowState.Runtime.Features
{
    public interface IRecordRepository
    {
        bool TryEnqueuePending(RecordSubmissionCandidate candidate);

        bool TryUpdatePersonalBest(RecordSubmissionCandidate candidate);

        bool TryGetPersonalBest(
            string playerId,
            RecordBoardKey boardKey,
            out RecordSubmissionCandidate candidate);
    }
}
