using System.Collections.Generic;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class LocalSaveJsonCodecTests
    {
        private const string PlayerId = "player-a";
        private const string SubmissionId = "00000000-0000-4000-8000-000000000001";

        [Test]
        public void SerializeThenDeserialize_PreservesSaveData()
        {
            RecordSubmissionCandidate candidate = CreateStageCandidate();
            SettingsBindingOverride binding = new SettingsBindingOverride(
                new SettingsBindingTarget(
                    E_SettingsActionMap.Player,
                    E_SettingsDeviceGroup.KeyboardAndMouse,
                    new System.Guid("f1ba0d36-48eb-4cd5-b651-1c94a6531f70"),
                    new System.Guid("eb40bb66-4559-4dfa-9a2f-820438abb426"),
                    string.Empty),
                "<Keyboard>/q");
            LocalSaveData original = new LocalSaveData(
                LocalSaveData.CurrentVersion,
                PlayerId,
                new LocalSettingsData(75, true, new List<SettingsBindingOverride> { binding }),
                true,
                new List<RecordSubmissionCandidate> { candidate });

            bool didDeserialize = LocalSaveJsonCodec.TryDeserialize(
                LocalSaveJsonCodec.Serialize(original),
                out LocalSaveData restored);

            Assert.That(didDeserialize, Is.True);
            Assert.That(restored.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
            Assert.That(restored.AccountId, Is.EqualTo(PlayerId));
            Assert.That(restored.Settings.MasterVolume, Is.EqualTo(75));
            Assert.That(restored.Settings.IsFullscreen, Is.True);
            Assert.That(restored.Settings.BindingOverrides[0].ControlPath, Is.EqualTo("<Keyboard>/q"));
            Assert.That(restored.HasCompletedTutorial, Is.True);
            Assert.That(restored.PersonalBests[0].SubmissionId, Is.EqualTo(SubmissionId));
        }

        [Test]
        public void TryDeserialize_LegacyVersion_RecoversDefaultSave()
        {
            bool didDeserialize = LocalSaveJsonCodec.TryDeserialize(
                "{\"version\":0}",
                out LocalSaveData saveData);

            Assert.That(didDeserialize, Is.True);
            Assert.That(saveData.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
            Assert.That(saveData.Settings.MasterVolume, Is.EqualTo(100));
            Assert.That(saveData.PersonalBests, Is.Empty);
        }

        [Test]
        public void TryDeserialize_CorruptOrUnsupportedSave_IsRejected()
        {
            Assert.That(LocalSaveJsonCodec.TryDeserialize("{", out LocalSaveData corrupt), Is.False);
            Assert.That(corrupt.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
            Assert.That(LocalSaveJsonCodec.TryDeserialize("{\"version\":999}", out LocalSaveData unsupported), Is.False);
            Assert.That(unsupported.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
        }

        [Test]
        public void TrySave_FailedAtomicWrite_PreservesExistingSave()
        {
            InMemorySaveFileStore fileStore = new InMemorySaveFileStore("old-save")
            {
                ShouldFailWrite = true
            };
            LocalRecordRepository repository = new LocalRecordRepository(fileStore);

            bool didSave = repository.TrySave(LocalSaveData.CreateDefault(
                new LocalSettingsData(100, false, null)));

            Assert.That(didSave, Is.False);
            Assert.That(fileStore.Contents, Is.EqualTo("old-save"));
        }

        [Test]
        public void TryLoad_CorruptSave_UsesSafeDefault()
        {
            LocalRecordRepository repository = new LocalRecordRepository(
                new InMemorySaveFileStore("{"));

            Assert.That(repository.TryLoad(out LocalSaveData saveData), Is.True);
            Assert.That(saveData.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
            Assert.That(saveData.Settings.MasterVolume, Is.EqualTo(100));
        }

        [Test]
        public void TryLoad_PersonalBest_IsAvailableFromRepository()
        {
            RecordSubmissionCandidate candidate = CreateStageCandidate();
            LocalSaveData savedData = new LocalSaveData(
                LocalSaveData.CurrentVersion,
                PlayerId,
                new LocalSettingsData(100, false, null),
                false,
                new List<RecordSubmissionCandidate> { candidate });
            LocalRecordRepository repository = new LocalRecordRepository(
                new InMemorySaveFileStore(LocalSaveJsonCodec.Serialize(savedData)));

            Assert.That(repository.TryLoad(out LocalSaveData loadedData), Is.True);
            Assert.That(repository.TryGetPersonalBest(
                PlayerId,
                candidate.BoardKey,
                out RecordSubmissionCandidate restored), Is.True);
            Assert.That(restored.SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(loadedData.PersonalBests, Has.Count.EqualTo(1));
        }

        [Test]
        public void StoreCandidate_PendingSurvivesRestartWithSameId()
        {
            LocalRecordRepository repository = new LocalRecordRepository(
                new InMemorySaveFileStore(null));
            RecordSubmissionService service = new RecordSubmissionService(repository);
            RecordSubmissionCandidate candidate = CreateStageCandidate();

            Assert.That(service.TryStoreCandidate(candidate), Is.True);
            Assert.That(service.TryStoreCandidate(candidate), Is.False);

            LocalSaveData saveData = repository.CreateSaveData(
                PlayerId,
                new LocalSettingsData(100, false, null),
                false);

            Assert.That(saveData.PersonalBests, Has.Count.EqualTo(1));
            Assert.That(repository.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            LocalRecordRepository restored = new LocalRecordRepository(
                new InMemorySaveFileStore(LocalSaveJsonCodec.Serialize(saveData)));
            restored.TryLoad(out LocalSaveData ignored);
            Assert.That(restored.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            Assert.That(restored.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
        }

        private static RecordSubmissionCandidate CreateStageCandidate()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId, SubmissionId, "stage-001", 1, E_StageResultType.Cleared,
                12.0, out RecordSubmissionCandidate candidate);
            Assert.That(didCreate, Is.True);
            return candidate;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void LegacySave_PreservesPendingWhenPresentAndDropsTerminalHistory(int version)
        {
            LocalSaveData source = new LocalSaveData(version, PlayerId,
                new LocalSettingsData(75, false, null), true,
                new[] { CreateStageCandidate() }, null, new[] { CreateStageCandidate() });
            string json = LocalSaveJsonCodec.Serialize(source);
            json = json.Insert(json.Length - 1,
                ",\"submittedIds\":[\"old-id\"],\"rejectedIds\":[\"old-id\"],\"rejectedReceipts\":[]");
            Assert.That(LocalSaveJsonCodec.TryDeserialize(json, out LocalSaveData restored), Is.True);
            Assert.That(restored.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
            Assert.That(restored.PendingSubmissions[0].SubmissionId, Is.EqualTo(SubmissionId));
            string updated = LocalSaveJsonCodec.Serialize(restored);
            Assert.That(updated, Does.Not.Contain("submittedIds"));
            Assert.That(updated, Does.Not.Contain("rejectedIds"));
            Assert.That(updated, Does.Not.Contain("rejectedReceipts"));
        }

        private sealed class InMemorySaveFileStore : ILocalSaveFileStore
        {
            public bool ShouldFailWrite { get; set; }

            public string Contents { get; private set; }

            public bool HasSave => !string.IsNullOrEmpty(Contents);

            public InMemorySaveFileStore(string contents)
            {
                Contents = contents;
            }

            public bool TryRead(out string contents)
            {
                contents = Contents;
                return HasSave;
            }

            public bool TryWriteAtomically(string contents)
            {
                if (ShouldFailWrite)
                {
                    return false;
                }

                Contents = contents;
                return true;
            }
        }
    }
}
