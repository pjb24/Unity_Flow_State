using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public sealed class LocalSaveData
    {
        public const int CurrentVersion = 6;
        public OnlineAccountState OnlineAccount { get; }
        public OnlineDataScope OnlineScope { get; }
        public IReadOnlyList<OnlineLocalSaveData> InactiveOnlineAreas { get; }
        public bool RequiresOnlineMigration { get; }
        public bool HasValidOnlineScopes
        {
            get
            {
                if (!OnlineScope.IsValid) return false;
                for (int i = 0; i < InactiveOnlineAreas.Count; i++)
                {
                    OnlineLocalSaveData area = InactiveOnlineAreas[i];
                    if (area == null || area.Scope == null || !area.Scope.IsValid || area.Scope.Matches(OnlineScope))
                        return false;
                    for (int j = 0; j < i; j++)
                        if (area.Scope.Matches(InactiveOnlineAreas[j].Scope)) return false;
                }
                return true;
            }
        }

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
            IReadOnlyList<RecordSubmissionCandidate> pendingSubmissions = null,
            OnlineDataScope onlineScope = null,
            IReadOnlyList<OnlineLocalSaveData> inactiveOnlineAreas = null,
            bool requiresOnlineMigration = false)
        {
            Version = version;
            AccountId = accountId == null ? string.Empty : accountId;
            OnlineAccount = onlineAccount == null ? new OnlineAccountState() : onlineAccount;
            Settings = settings;
            HasCompletedTutorial = hasCompletedTutorial;
            _personalBests = CopyCandidates(personalBests);
            _pendingSubmissions = CopyCandidates(pendingSubmissions);
            OnlineScope = onlineScope == null ? OnlineDataScope.CreateConfigured() : onlineScope;
            List<OnlineLocalSaveData> areas = new List<OnlineLocalSaveData>();
            if (inactiveOnlineAreas != null)
                for (int i = 0; i < inactiveOnlineAreas.Count; i++) areas.Add(inactiveOnlineAreas[i]);
            InactiveOnlineAreas = areas.AsReadOnly();
            RequiresOnlineMigration = requiresOnlineMigration;
        }

        public LocalSaveData SelectOnlineScope(OnlineDataScope scope)
        {
            if (OnlineScope.Matches(scope)) return this;
            OnlineLocalSaveData selected = null;
            List<OnlineLocalSaveData> inactive = new List<OnlineLocalSaveData>();
            inactive.Add(new OnlineLocalSaveData(OnlineScope, OnlineAccount, PersonalBests, PendingSubmissions));
            for (int i = 0; i < InactiveOnlineAreas.Count; i++)
            {
                OnlineLocalSaveData area = InactiveOnlineAreas[i];
                if (area.Scope.Matches(scope)) selected = area;
                else inactive.Add(area);
            }
            return new LocalSaveData(CurrentVersion, AccountId, Settings, HasCompletedTutorial,
                selected == null ? null : selected.PersonalBests,
                selected == null ? null : selected.Account,
                selected == null ? null : selected.PendingSubmissions,
                scope, inactive, RequiresOnlineMigration);
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
