using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public sealed class RecordSubmissionQueue
    {
        public const int MaximumAttemptsPerRetryTrigger = 3;

        private readonly List<RecordSubmissionEntry> _entries =
            new List<RecordSubmissionEntry>();

        public int Count => _entries.Count;

        public bool TryEnqueue(RecordSubmissionCandidate candidate)
        {
            if (candidate == null || HasDuplicateSubmission(candidate))
            {
                return false;
            }

            _entries.Add(new RecordSubmissionEntry(candidate));
            return true;
        }

        public bool TryGetEntry(
            string playerId,
            string submissionId,
            out RecordSubmissionEntry entry)
        {
            entry = null;

            for (int i = 0; i < _entries.Count; i++)
            {
                RecordSubmissionEntry current = _entries[i];

                if (current.Candidate.PlayerId == playerId &&
                    current.Candidate.SubmissionId == submissionId)
                {
                    entry = current;
                    return true;
                }
            }

            return false;
        }

        public void BeginRetryTrigger()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                _entries[i].ResetRetryTrigger();
            }
        }

        public bool TryBeginSubmission(
            string currentPlayerId,
            string submissionId)
        {
            return TryGetEntry(
                       currentPlayerId,
                       submissionId,
                       out RecordSubmissionEntry entry) &&
                   entry.TryBeginSubmission(
                       currentPlayerId,
                       MaximumAttemptsPerRetryTrigger);
        }

        public bool TryCompleteSubmission(
            string playerId,
            string submissionId,
            E_RecordSubmissionResult result)
        {
            return TryGetEntry(playerId, submissionId, out RecordSubmissionEntry entry) &&
                   entry.TryCompleteSubmission(result);
        }

        private bool HasDuplicateSubmission(RecordSubmissionCandidate candidate)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                RecordSubmissionCandidate current = _entries[i].Candidate;

                if (current.PlayerId == candidate.PlayerId &&
                    current.SubmissionId == candidate.SubmissionId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
