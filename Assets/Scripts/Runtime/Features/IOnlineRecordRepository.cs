namespace FlowState.Runtime.Features
{
    public interface IOnlineRecordRepository
    {
        bool TrySubmit(
            RecordSubmissionCandidate candidate,
            out E_RecordSubmissionResult result);
    }
}
