namespace FlowState.Runtime.Features
{
    public sealed class RecordSubmissionService
    {
        private readonly IRecordRepository _recordRepository;

        public RecordSubmissionService(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public bool TryStoreCandidate(RecordSubmissionCandidate candidate)
        {
            if (candidate == null || _recordRepository == null ||
                !_recordRepository.TryEnqueuePending(candidate))
            {
                return false;
            }

            _recordRepository.TryUpdatePersonalBest(candidate);
            return true;
        }
    }
}
