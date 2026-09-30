using System.Collections.Generic;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class LocalRecordRepository : ILocalRecordRepository
    {
        private readonly ILocalSaveFileStore _fileStore;
        private LocalSaveData _lastSave;
        public string AccountId => _lastSave == null ? string.Empty : _lastSave.AccountId;
        public OnlineAccountState OnlineAccount => _lastSave == null
            ? new OnlineAccountState() : _lastSave.OnlineAccount;
        private readonly MemoryRecordRepository _memoryRepository =
            new MemoryRecordRepository();

        public LocalRecordRepository(ILocalSaveFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        public bool TryLoad(out LocalSaveData saveData)
        {
            saveData = LocalSaveData.CreateDefault(new LocalSettingsData(100, false, null));

            if (_fileStore == null || !_fileStore.HasSave)
            {
                _lastSave = saveData;
                return true;
            }

            if (!_fileStore.TryRead(out string contents) ||
                !LocalSaveJsonCodec.TryDeserialize(contents, out saveData))
            {
                Debug.LogWarning("[LocalRecordRepository] Save recovery used default values.");
                saveData = LocalSaveData.CreateDefault(new LocalSettingsData(100, false, null));
            }

            RestoreMemoryRecords(saveData);
            _lastSave = saveData;

            return true;
        }

        public bool TrySave(LocalSaveData saveData)
        {
            if (_fileStore == null || saveData == null ||
                !_fileStore.TryWriteAtomically(LocalSaveJsonCodec.Serialize(saveData))) return false;
            _lastSave = saveData;
            return true;
        }

        public bool TrySaveOnlineAccount(OnlineAccountState state)
        {
            if (_lastSave == null || state == null) return false;
            if (!string.IsNullOrEmpty(OnlineAccount.PlayerId) && state.PlayerId != OnlineAccount.PlayerId)
                return false;
            return TrySave(new LocalSaveData(LocalSaveData.CurrentVersion, _lastSave.AccountId,
                _lastSave.Settings, _lastSave.HasCompletedTutorial,
                _memoryRepository.CreatePersonalBestSnapshot(), state, CreatePendingSnapshot()));
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
            return _memoryRepository.TryEnqueuePending(candidate);
        }

        public bool TryUpdatePersonalBest(RecordSubmissionCandidate candidate)
        {
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
                    _memoryRepository.CreatePersonalBestSnapshot(), OnlineAccount, remaining))) return false;
            return _memoryRepository.TryRemovePending(playerId, submissionId);
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
                _memoryRepository.CreatePersonalBestSnapshot(), OnlineAccount, CreatePendingSnapshot());
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
