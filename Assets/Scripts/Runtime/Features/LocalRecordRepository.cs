using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class LocalRecordRepository : ILocalRecordRepository
    {
        private readonly ILocalSaveFileStore _fileStore;
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
                return true;
            }

            if (!_fileStore.TryRead(out string contents) ||
                !LocalSaveJsonCodec.TryDeserialize(contents, out saveData))
            {
                Debug.LogWarning("[LocalRecordRepository] Save recovery used default values.");
                saveData = LocalSaveData.CreateDefault(new LocalSettingsData(100, false, null));
            }

            RestoreMemoryRecords(saveData);

            return true;
        }

        public bool TrySave(LocalSaveData saveData)
        {
            return _fileStore != null && saveData != null &&
                   _fileStore.TryWriteAtomically(LocalSaveJsonCodec.Serialize(saveData));
        }

        public bool TryEnqueuePending(RecordSubmissionCandidate candidate)
        {
            return _memoryRepository.TryEnqueuePending(candidate);
        }

        public bool TryUpdatePersonalBest(RecordSubmissionCandidate candidate)
        {
            return _memoryRepository.TryUpdatePersonalBest(candidate);
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
                _memoryRepository.CreatePersonalBestSnapshot(),
                _memoryRepository.CreatePendingSnapshot());
        }

        private void RestoreMemoryRecords(LocalSaveData saveData)
        {
            for (int i = 0; i < saveData.PersonalBests.Count; i++)
            {
                _memoryRepository.TryUpdatePersonalBest(saveData.PersonalBests[i]);
            }

            for (int i = 0; i < saveData.PendingSubmissions.Count; i++)
            {
                _memoryRepository.TryEnqueuePending(saveData.PendingSubmissions[i]);
            }
        }
    }
}
