using FlowState.Runtime.Core;

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
            // New writes are Infinite-only; shared memory/codec restoration is separate.
            if (candidate == null || candidate.GameMode != E_GameMode.Infinite ||
                candidate.BoardKey.RulesVersion != ScoringVersion.Current ||
                candidate.BoardKey.StageId != string.Empty || _recordRepository == null ||
                !_recordRepository.TryEnqueuePending(candidate))
            {
                return false;
            }

            _recordRepository.TryUpdatePersonalBest(candidate);
            return true;
        }
    }
}
