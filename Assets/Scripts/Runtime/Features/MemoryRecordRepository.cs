using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public sealed class MemoryRecordRepository : IRecordRepository
    {
        private readonly List<RecordSubmissionCandidate> _personalBests =
            new List<RecordSubmissionCandidate>();
        private readonly RecordSubmissionQueue _pendingSubmissions =
            new RecordSubmissionQueue();

        public bool TryEnqueuePending(RecordSubmissionCandidate candidate)
        {
            return _pendingSubmissions.TryEnqueue(candidate);
        }

        public bool TryUpdatePersonalBest(RecordSubmissionCandidate candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            for (int i = 0; i < _personalBests.Count; i++)
            {
                RecordSubmissionCandidate current = _personalBests[i];

                if (current.PlayerId != candidate.PlayerId ||
                    !current.BoardKey.Equals(candidate.BoardKey))
                {
                    continue;
                }

                if (!RecordLeaderboardPolicy.ShouldReplaceBest(current, candidate))
                {
                    return false;
                }

                _personalBests[i] = candidate;
                return true;
            }

            _personalBests.Add(candidate);
            return true;
        }

        public bool TryGetPersonalBest(
            string playerId,
            RecordBoardKey boardKey,
            out RecordSubmissionCandidate candidate)
        {
            candidate = null;

            for (int i = 0; i < _personalBests.Count; i++)
            {
                RecordSubmissionCandidate current = _personalBests[i];

                if (current.PlayerId == playerId &&
                    current.BoardKey.Equals(boardKey))
                {
                    candidate = current;
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyList<RecordSubmissionCandidate> CreatePersonalBestSnapshot()
        {
            return new List<RecordSubmissionCandidate>(_personalBests);
        }

        public IReadOnlyList<RecordSubmissionCandidate> CreatePendingSnapshot()
        {
            return _pendingSubmissions.CreatePendingSnapshot();
        }
    }
}
