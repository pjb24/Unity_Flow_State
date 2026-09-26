namespace FlowState.Runtime.Features
{
    public sealed class RecordSubmissionEntry
    {
        private E_RecordSubmissionStatus _status;
        private int _attemptCountInRetryTrigger;
        private bool _isSubmitting;

        public RecordSubmissionCandidate Candidate { get; }

        public E_RecordSubmissionStatus Status => _status;

        public int AttemptCountInRetryTrigger => _attemptCountInRetryTrigger;

        public bool IsSubmitting => _isSubmitting;

        internal RecordSubmissionEntry(RecordSubmissionCandidate candidate)
        {
            Candidate = candidate;
            _status = E_RecordSubmissionStatus.Pending;
        }

        internal void ResetRetryTrigger()
        {
            if (_status == E_RecordSubmissionStatus.Pending && !_isSubmitting)
            {
                _attemptCountInRetryTrigger = 0;
            }
        }

        internal bool TryBeginSubmission(string currentPlayerId, int maximumAttempts)
        {
            if (_status != E_RecordSubmissionStatus.Pending ||
                _isSubmitting ||
                Candidate.PlayerId != currentPlayerId ||
                _attemptCountInRetryTrigger >= maximumAttempts)
            {
                return false;
            }

            _attemptCountInRetryTrigger++;
            _isSubmitting = true;
            return true;
        }

        internal bool TryCompleteSubmission(E_RecordSubmissionResult result)
        {
            if (!_isSubmitting)
            {
                return false;
            }

            _isSubmitting = false;

            switch (result)
            {
                case E_RecordSubmissionResult.Submitted:
                    _status = E_RecordSubmissionStatus.Submitted;
                    return true;

                case E_RecordSubmissionResult.Rejected:
                    _status = E_RecordSubmissionStatus.Rejected;
                    return true;

                case E_RecordSubmissionResult.TransientFailure:
                    return true;

                default:
                    return false;
            }
        }
    }
}
