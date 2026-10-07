using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class LocalRecordRepository : ILocalRecordRepository
    {
        public const long PendingRetentionMilliseconds = 180L * 24L * 60L * 60L * 1000L;
        private readonly ILocalSaveFileStore _fileStore;
        private readonly OnlineDataScope _activeScope;
        private readonly Func<long> _now;
        private LocalSaveData _lastSave;
        private bool _hasReliableLoad = true;
        private bool _isTransferRequestInProgress;
        private bool _isAccountTransitionInProgress;
        public bool IsLocalSaveReady { get; private set; }
        public OnlineDataScope ActiveScope => _activeScope;
        public bool CanUseOnlineData => IsLocalSaveReady && _activeScope.Matches(OnlineDataScope.CreateConfigured());
        public string AccountId => _lastSave == null ? string.Empty : _lastSave.AccountId;
        public OnlineAccountState OnlineAccount => _lastSave == null
            ? new OnlineAccountState() : _lastSave.OnlineAccount;
        private MemoryRecordRepository _memoryRepository =
            new MemoryRecordRepository();

        public LocalRecordRepository(ILocalSaveFileStore fileStore, OnlineDataScope activeScope = null,
            Func<long> now = null)
        {
            _fileStore = fileStore;
            _activeScope = activeScope == null ? OnlineDataScope.CreateConfigured() : activeScope;
            _now = now == null ? () => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() : now;
        }

        public bool TryLoad(out LocalSaveData saveData)
        {
            saveData = LocalSaveData.CreateDefault(new LocalSettingsData(100, false, null));
            saveData = saveData.SelectOnlineScope(_activeScope);
            IsLocalSaveReady = false;
            _memoryRepository = new MemoryRecordRepository();

            if (_fileStore == null || !_fileStore.HasSave)
            {
                _lastSave = saveData;
                _hasReliableLoad = _fileStore != null && _activeScope.IsValid;
                IsLocalSaveReady = _hasReliableLoad;
                return true;
            }

            if (!_fileStore.TryRead(out string contents) ||
                !LocalSaveJsonCodec.TryDeserialize(contents, out saveData))
            {
                Debug.LogWarning("[LocalRecordRepository] Save recovery used default values.");
                saveData = LocalSaveData.CreateDefault(new LocalSettingsData(100, false, null));
                saveData = saveData.SelectOnlineScope(_activeScope);
                _lastSave = saveData;
                _hasReliableLoad = false;
                return true;
            }
            bool requiresSave = saveData.RequiresOnlineMigration || !saveData.OnlineScope.Matches(_activeScope);
            saveData = saveData.SelectOnlineScope(_activeScope);
            RestoreMemoryRecords(saveData);
            _lastSave = saveData;
            _hasReliableLoad = _activeScope.IsValid;
            IsLocalSaveReady = _hasReliableLoad;
            if (requiresSave && !TrySave(saveData)) IsLocalSaveReady = false;
            return true;
        }

        public bool TrySave(LocalSaveData saveData)
        {
            if (!_hasReliableLoad || _fileStore == null || saveData == null ||
                !saveData.HasValidOnlineScopes || !saveData.OnlineScope.Matches(_activeScope)) return false;
            // Ordinary settings/account checkpoints cannot remove or replace
            // inactive environment data. Environment selection happens on load.
            if (_lastSave != null && !KeepsInactiveAreas(saveData)) return false;
            if (!_fileStore.TryWriteAtomically(LocalSaveJsonCodec.Serialize(saveData)))
            {
                IsLocalSaveReady = false;
                return false;
            }
            _lastSave = saveData;
            IsLocalSaveReady = true;
            return true;
        }

        public bool TrySaveOnlineAccount(OnlineAccountState state)
        {
            if (_lastSave == null || state == null) return false;
            if (!string.IsNullOrEmpty(OnlineAccount.PlayerId) && state.PlayerId != OnlineAccount.PlayerId)
                return false;
            return TrySave(new LocalSaveData(LocalSaveData.CurrentVersion, _lastSave.AccountId,
                _lastSave.Settings, _lastSave.HasCompletedTutorial,
                _memoryRepository.CreatePersonalBestSnapshot(), state, CreatePendingSnapshot(),
                _activeScope, _lastSave.InactiveOnlineAreas));
        }

        public bool TryCheckpoint()
        {
            return TrySaveOnlineAccount(OnlineAccount);
        }

        public IReadOnlyList<RecordSubmissionCandidate> CreatePendingSnapshot()
        {
            return _memoryRepository.CreatePendingSnapshot();
        }

        public bool TryEnqueuePending(RecordSubmissionCandidate candidate)
        {
            if (_isTransferRequestInProgress || _isAccountTransitionInProgress) return false;
            if (candidate == null) return false;
            if (candidate.CreatedAtMilliseconds <= 0)
                candidate = new RecordSubmissionCandidate(candidate.PlayerId, candidate.SubmissionId,
                    candidate.BoardKey, candidate.RankingValue, candidate.RunDurationMilliseconds,
                    candidate.BaseDistanceScore, candidate.MomentumBonus, candidate.DistanceScore,
                    candidate.CollectibleScore, candidate.TotalScore, candidate.MaximumMomentumMultiplier,
                    _now());
            return _memoryRepository.TryEnqueuePending(candidate);
        }

        public bool TryUpdatePersonalBest(RecordSubmissionCandidate candidate)
        {
            if (_isAccountTransitionInProgress) return false;
            return _memoryRepository.TryUpdatePersonalBest(candidate);
        }

        public bool TryRemovePending(string playerId, string submissionId)
        {
            if (_lastSave == null) return false;
            List<RecordSubmissionCandidate> remaining = new List<RecordSubmissionCandidate>();
            bool hasTarget = false;
            IReadOnlyList<RecordSubmissionCandidate> pending = CreatePendingSnapshot();
            for (int i = 0; i < pending.Count; i++)
            {
                if (pending[i].PlayerId == playerId && pending[i].SubmissionId == submissionId)
                    hasTarget = true;
                else remaining.Add(pending[i]);
            }
            if (!hasTarget) return false;
            // Commit removal first. Failed writes retain the same ID for safe server replay.
            if (!TrySave(new LocalSaveData(LocalSaveData.CurrentVersion, _lastSave.AccountId,
                    _lastSave.Settings, _lastSave.HasCompletedTutorial,
                    _memoryRepository.CreatePersonalBestSnapshot(), OnlineAccount, remaining,
                    _activeScope, _lastSave.InactiveOnlineAreas))) return false;
            return _memoryRepository.TryRemovePending(playerId, submissionId);
        }

        // Save first: a failed write must retain every Pending candidate.
        public bool TryExpirePending(long nowMilliseconds, out int expiredCount)
        {
            expiredCount = 0;
            if (_lastSave == null || nowMilliseconds <= 0) return false;
            List<RecordSubmissionCandidate> remaining = new List<RecordSubmissionCandidate>();
            IReadOnlyList<RecordSubmissionCandidate> pending = CreatePendingSnapshot();
            for (int i = 0; i < pending.Count; i++)
            {
                RecordSubmissionCandidate candidate = pending[i];
                if (candidate.CreatedAtMilliseconds > 0 && nowMilliseconds >= candidate.CreatedAtMilliseconds + PendingRetentionMilliseconds)
                    expiredCount++;
                else remaining.Add(candidate);
            }
            if (expiredCount == 0) return true;
            if (!TrySave(new LocalSaveData(LocalSaveData.CurrentVersion, _lastSave.AccountId,
                _lastSave.Settings, _lastSave.HasCompletedTutorial,
                _memoryRepository.CreatePersonalBestSnapshot(), OnlineAccount, remaining,
                _activeScope, _lastSave.InactiveOnlineAreas))) { expiredCount = 0; return false; }
            _memoryRepository = new MemoryRecordRepository();
            RestoreMemoryRecords(_lastSave);
            return true;
        }

        public bool TryGetPersonalBest(
            string playerId,
            RecordBoardKey boardKey,
            out RecordSubmissionCandidate candidate)
        {
            return _memoryRepository.TryGetPersonalBest(
                playerId,
                boardKey,
                out candidate);
        }

        public LocalSaveData CreateSaveData(
            string accountId,
            LocalSettingsData settings,
            bool hasCompletedTutorial)
        {
            return new LocalSaveData(
                LocalSaveData.CurrentVersion,
                accountId,
                settings,
                hasCompletedTutorial,
                _memoryRepository.CreatePersonalBestSnapshot(), OnlineAccount, CreatePendingSnapshot(),
                _activeScope, _lastSave == null ? null : _lastSave.InactiveOnlineAreas);
        }

        // Explicit user action only. Persist first so a restart cannot resurrect
        // a queue that was considered empty when authorizing a transfer.
        public bool TryDiscardPending()
        {
            if (_lastSave == null || _isTransferRequestInProgress || _isAccountTransitionInProgress || !_hasReliableLoad) return false;
            if (!TrySave(new LocalSaveData(LocalSaveData.CurrentVersion, _lastSave.AccountId,
                _lastSave.Settings, _lastSave.HasCompletedTutorial,
                _memoryRepository.CreatePersonalBestSnapshot(), OnlineAccount,
                new List<RecordSubmissionCandidate>(), _activeScope, _lastSave.InactiveOnlineAreas))) return false;
            _memoryRepository = new MemoryRecordRepository();
            RestoreMemoryRecords(_lastSave);
            return true;
        }

        internal bool TryBeginTransferRequest()
        {
            if (_isTransferRequestInProgress || _isAccountTransitionInProgress || !_hasReliableLoad ||
                !_activeScope.Matches(OnlineDataScope.CreateConfigured()) || CreatePendingSnapshot().Count != 0)
                return false;
            if (!TryCheckpoint() || !CanUseOnlineData || CreatePendingSnapshot().Count != 0) return false;
            _isTransferRequestInProgress = true;
            return true;
        }

        internal void EndTransferRequest()
        {
            _isTransferRequestInProgress = false;
        }

        internal bool TryBeginAccountTransition(bool ownsTransferGate)
        {
            if (_isAccountTransitionInProgress || (_isTransferRequestInProgress && !ownsTransferGate) ||
                !CanUseOnlineData || CreatePendingSnapshot().Count != 0) return false;
            _isAccountTransitionInProgress = true;
            return true;
        }

        internal void EndAccountTransition() { _isAccountTransitionInProgress = false; }

        internal bool TryApplyAccountTransition(OnlineAccountState expected, OnlineAccountState next,
            IReadOnlyList<RecordSubmissionCandidate> personalBests)
        {
            if (!_isAccountTransitionInProgress || !ReferenceEquals(expected, OnlineAccount) ||
                CreatePendingSnapshot().Count != 0 || next == null || personalBests == null) return false;
            // Replace the entire current area in one durable write. Other scopes
            // and device settings are untouched. Never use best-only merge here.
            if (!TrySave(new LocalSaveData(LocalSaveData.CurrentVersion, _lastSave.AccountId,
                _lastSave.Settings, _lastSave.HasCompletedTutorial, personalBests, next,
                CreatePendingSnapshot(), _activeScope, _lastSave.InactiveOnlineAreas))) return false;
            _memoryRepository = new MemoryRecordRepository();
            RestoreMemoryRecords(_lastSave);
            return true;
        }

        private bool KeepsInactiveAreas(LocalSaveData next)
        {
            if (next.InactiveOnlineAreas.Count != _lastSave.InactiveOnlineAreas.Count) return false;
            for (int i = 0; i < next.InactiveOnlineAreas.Count; i++)
                if (!ReferenceEquals(next.InactiveOnlineAreas[i], _lastSave.InactiveOnlineAreas[i])) return false;
            return true;
        }

        private void RestoreMemoryRecords(LocalSaveData saveData)
        {
            for (int i = 0; i < saveData.PersonalBests.Count; i++)
            {
                _memoryRepository.TryUpdatePersonalBest(saveData.PersonalBests[i]);
            }
            for (int i = 0; i < saveData.PendingSubmissions.Count; i++)
                _memoryRepository.TryEnqueuePending(saveData.PendingSubmissions[i]);
        }
    }
}
