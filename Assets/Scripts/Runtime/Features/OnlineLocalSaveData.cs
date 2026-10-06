using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public sealed class OnlineLocalSaveData
    {
        public OnlineDataScope Scope { get; }
        public OnlineAccountState Account { get; }
        public IReadOnlyList<RecordSubmissionCandidate> PersonalBests { get; }
        public IReadOnlyList<RecordSubmissionCandidate> PendingSubmissions { get; }

        public OnlineLocalSaveData(OnlineDataScope scope, OnlineAccountState account,
            IReadOnlyList<RecordSubmissionCandidate> personalBests,
            IReadOnlyList<RecordSubmissionCandidate> pendingSubmissions)
        {
            Scope = scope;
            Account = account == null ? new OnlineAccountState() : account;
            PersonalBests = Copy(personalBests);
            PendingSubmissions = Copy(pendingSubmissions);
        }

        private static IReadOnlyList<RecordSubmissionCandidate> Copy(IReadOnlyList<RecordSubmissionCandidate> source)
        {
            List<RecordSubmissionCandidate> result = new List<RecordSubmissionCandidate>();
            if (source != null)
                for (int i = 0; i < source.Count; i++) result.Add(source[i]);
            return result.AsReadOnly();
        }
    }
}
