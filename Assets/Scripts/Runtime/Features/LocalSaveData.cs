using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public sealed class LocalSaveData
    {
        public const int CurrentVersion = 5;
        public OnlineAccountState OnlineAccount { get; }

        private readonly List<RecordSubmissionCandidate> _personalBests;
        private readonly List<RecordSubmissionCandidate> _pendingSubmissions;
        public IReadOnlyList<RecordSubmissionCandidate> PendingSubmissions => _pendingSubmissions;

        public int Version { get; }

        public string AccountId { get; }

        public LocalSettingsData Settings { get; }

        public bool HasCompletedTutorial { get; }

        public IReadOnlyList<RecordSubmissionCandidate> PersonalBests =>
            _personalBests;

        public LocalSaveData(
            int version,
            string accountId,
            LocalSettingsData settings,
            bool hasCompletedTutorial,
            IReadOnlyList<RecordSubmissionCandidate> personalBests,
            OnlineAccountState onlineAccount = null,
            IReadOnlyList<RecordSubmissionCandidate> pendingSubmissions = null)
        {
            Version = version;
            AccountId = accountId == null ? string.Empty : accountId;
            OnlineAccount = onlineAccount == null ? new OnlineAccountState() : onlineAccount;
            Settings = settings;
            HasCompletedTutorial = hasCompletedTutorial;
            _personalBests = CopyCandidates(personalBests);
            _pendingSubmissions = CopyCandidates(pendingSubmissions);
        }

        public static LocalSaveData CreateDefault(LocalSettingsData settings)
        {
            return new LocalSaveData(
                CurrentVersion,
                string.Empty,
                settings,
                false,
                null);
        }

        private static List<RecordSubmissionCandidate> CopyCandidates(
            IReadOnlyList<RecordSubmissionCandidate> candidates)
        {
            List<RecordSubmissionCandidate> copiedCandidates =
                new List<RecordSubmissionCandidate>();

            if (candidates == null)
            {
                return copiedCandidates;
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i] != null)
                {
                    copiedCandidates.Add(candidates[i]);
                }
            }

            return copiedCandidates;
        }
    }
}
