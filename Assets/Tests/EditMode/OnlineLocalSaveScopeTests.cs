using System;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.EditMode
{
    public sealed class OnlineLocalSaveScopeTests
    {
        private const string Owner = "local-owner";
        private const string SubmissionId = "00000000-0000-4000-8000-000000000001";

        private static RecordSubmissionCandidate Candidate(string owner = Owner)
        {
            Assert.That(RecordSubmissionPolicy.TryCreateStageCandidate(owner, SubmissionId,
                "stage-001", 1, E_StageResultType.Cleared, 1.25,
                out RecordSubmissionCandidate candidate), Is.True);
            return candidate;
        }

        private static LocalSaveData Data(int version = LocalSaveData.CurrentVersion,
            bool hasPending = true, OnlineDataScope scope = null)
        {
            return new LocalSaveData(version, Owner, new LocalSettingsData(75, true, null), true,
                new[] { Candidate() }, new OnlineAccountState(true, "ugs-original"),
                hasPending ? new[] { Candidate() } : null, scope);
        }

        private static LocalRecordRepository Load(OnlineTestFileStore store, OnlineDataScope scope = null)
        {
            LocalRecordRepository local = new LocalRecordRepository(store, scope);
            Assert.That(local.TryLoad(out LocalSaveData ignored), Is.True);
            return local;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void LegacyMigration_AssignsVerificationAndKeepsCandidateIdentity(int version)
        {
            string json = LocalSaveJsonCodec.Serialize(Data(version));
            Assert.That(LocalSaveJsonCodec.TryDeserialize(json, out LocalSaveData migrated), Is.True);
            Assert.That(migrated.Version, Is.EqualTo(6));
            Assert.That(migrated.RequiresOnlineMigration, Is.True);
            Assert.That(migrated.OnlineScope.Matches(OnlineDataScope.CreateVerification()), Is.True);
            Assert.That(migrated.PendingSubmissions[0].PlayerId, Is.EqualTo(Owner));
            Assert.That(migrated.PendingSubmissions[0].SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(migrated.PendingSubmissions[0].RankingValue, Is.EqualTo(1250));
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = json };
            LocalRecordRepository local = Load(store);
            Assert.That(local.IsLocalSaveReady, Is.True);
            Assert.That(LocalSaveJsonCodec.TryDeserialize(store.Contents, out LocalSaveData persisted), Is.True);
            Assert.That(persisted.RequiresOnlineMigration, Is.False);
            Assert.That(persisted.Settings.MasterVolume, Is.EqualTo(75));
            Assert.That(persisted.HasCompletedTutorial, Is.True);
        }

        [TestCase("other-project", "other-environment")]
        [TestCase(OnlineRecordConfiguration.ProjectId, "other-environment")]
        [TestCase("other-project", OnlineRecordConfiguration.EnvironmentId)]
        public async Task DifferentScope_ArchivesWithoutSendingOrReassigning(string project, string environment)
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data()) };
            LocalRecordRepository foreign = Load(store, new OnlineDataScope(project, environment));
            Assert.That(foreign.OnlineAccount.PlayerId, Is.Empty);
            Assert.That(foreign.CreatePendingSnapshot(), Is.Empty);
            Assert.That(foreign.TryGetPersonalBest(Owner, Candidate().BoardKey, out RecordSubmissionCandidate ignored), Is.False);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner, () => foreign.OnlineAccount, auth, transport);
            await new OnlineRecordCoordinator(Owner, foreign, auth, online).RetryPendingAsync();
            Assert.That(await new OnlineRecordCoordinator(Owner, foreign, auth, online).ConfirmRecoveryNoticeAsync(), Is.False);
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
            int transferCalls = 0;
            Assert.That(await new AccountTransferPendingGate(foreign).TryRequestAsync(() =>
                { transferCalls++; return Task.FromResult(true); }), Is.False);
            Assert.That(transferCalls, Is.Zero);
            Assert.That(foreign.TryCheckpoint(), Is.True);
            LocalRecordRepository original = Load(store);
            Assert.That(original.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(original.OnlineAccount.PlayerId, Is.EqualTo("ugs-original"));
        }

        [Test]
        public void LegacyOpenedInAnotherEnvironment_IsAssignedToVerificationOnly()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data(5)) };
            LocalRecordRepository other = Load(store, new OnlineDataScope("another-project", "another-environment"));
            Assert.That(other.CreatePendingSnapshot(), Is.Empty);
            Assert.That(LocalSaveJsonCodec.TryDeserialize(store.Contents, out LocalSaveData saved), Is.True);
            Assert.That(saved.InactiveOnlineAreas[0].Scope.Matches(OnlineDataScope.CreateVerification()), Is.True);
            Assert.That(saved.InactiveOnlineAreas[0].PendingSubmissions[0].PlayerId, Is.EqualTo(Owner));
        }

        [Test]
        public void ScopedAreas_RoundTripAndPreserveSettingsTutorialAndBindings()
        {
            SettingsBindingOverride binding = new SettingsBindingOverride(new SettingsBindingTarget(
                E_SettingsActionMap.Player, E_SettingsDeviceGroup.KeyboardAndMouse, Guid.NewGuid(),
                Guid.NewGuid(), string.Empty), "<Keyboard>/q");
            LocalSaveData source = new LocalSaveData(6, Owner, new LocalSettingsData(42, true,
                new[] { binding }), true, new[] { Candidate() }, new OnlineAccountState(true, "ugs-original"),
                new[] { Candidate() });
            OnlineDataScope alternate = new OnlineDataScope("other-project", "other-environment");
            LocalSaveData activeOther = source.SelectOnlineScope(alternate);
            Assert.That(LocalSaveJsonCodec.TryDeserialize(LocalSaveJsonCodec.Serialize(activeOther),
                out LocalSaveData restored), Is.True);
            LocalSaveData original = restored.SelectOnlineScope(OnlineDataScope.CreateVerification());
            Assert.That(original.PendingSubmissions[0].SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(original.PersonalBests[0].RankingValue, Is.EqualTo(1250));
            Assert.That(original.OnlineAccount.PlayerId, Is.EqualTo("ugs-original"));
            Assert.That(original.Settings.MasterVolume, Is.EqualTo(42));
            Assert.That(original.Settings.BindingOverrides[0].ControlPath, Is.EqualTo("<Keyboard>/q"));
            Assert.That(original.HasCompletedTutorial, Is.True);
        }

        [Test]
        public async Task MigrationWriteFailure_KeepsOriginalFilePendingAndBlocksRequests()
        {
            string before = LocalSaveJsonCodec.Serialize(Data(5));
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = before, FailWrite = true };
            LocalRecordRepository local = Load(store);
            Assert.That(local.IsLocalSaveReady, Is.False);
            Assert.That(local.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            Assert.That(store.Contents, Is.EqualTo(before));
            int calls = 0;
            Assert.That(await new AccountTransferPendingGate(local).TryRequestAsync(() =>
                { calls++; return Task.FromResult(true); }), Is.False);
            Assert.That(calls, Is.Zero);
            store.FailWrite = false;
            Assert.That(local.TryCheckpoint(), Is.True);
            Assert.That(local.IsLocalSaveReady, Is.True);
            Assert.That(Load(store).CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
        }

        [TestCase("A")]
        [TestCase("B")]
        [TestCase("foreign-owner")]
        public async Task PendingOnEitherDeviceOrDifferentOwner_MakesZeroTransferRequests(string owner)
        {
            OnlineTestFileStore store = new OnlineTestFileStore();
            LocalRecordRepository local = Load(store);
            local.TrySave(local.CreateSaveData(Owner, new LocalSettingsData(75, true, null), true));
            Assert.That(local.TryEnqueuePending(Candidate(owner)), Is.True);
            int calls = 0;
            Assert.That(await new AccountTransferPendingGate(local).TryRequestAsync(() =>
                { calls++; return Task.FromResult(true); }), Is.False);
            Assert.That(calls, Is.Zero); Assert.That(local.CreatePendingSnapshot(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task ExplicitDiscard_FailedSaveRetainsPendingAndSuccessfulSaveAllowsTransfer()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data()) };
            LocalRecordRepository local = Load(store); string before = store.Contents;
            store.FailWrite = true;
            Assert.That(local.TryDiscardPending(), Is.False);
            Assert.That(local.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            Assert.That(store.Contents, Is.EqualTo(before));
            int calls = 0; AccountTransferPendingGate gate = new AccountTransferPendingGate(local);
            Assert.That(await gate.TryRequestAsync(() => { calls++; return Task.FromResult(true); }), Is.False);
            Assert.That(calls, Is.Zero);
            store.FailWrite = false;
            Assert.That(local.TryDiscardPending(), Is.True);
            Assert.That(await gate.TryRequestAsync(() => { calls++; return Task.FromResult(true); }), Is.True);
            Assert.That(calls, Is.EqualTo(1));
            LocalRecordRepository restarted = Load(store);
            Assert.That(restarted.CreatePendingSnapshot(), Is.Empty);
            Assert.That(restarted.TryGetPersonalBest(Owner, Candidate().BoardKey, out RecordSubmissionCandidate best), Is.True);
            Assert.That(best.SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(restarted.OnlineAccount.PlayerId, Is.EqualTo("ugs-original"));
        }

        [Test]
        public void DiscardCurrentScope_DoesNotRemoveInactiveScopePending()
        {
            LocalSaveData other = Data().SelectOnlineScope(new OnlineDataScope("other-project", "other-environment"));
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(other) };
            LocalRecordRepository local = Load(store, other.OnlineScope);
            local.TryEnqueuePending(Candidate("other-owner")); local.TryCheckpoint();
            Assert.That(local.TryDiscardPending(), Is.True);
            Assert.That(Load(store).CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
        }

        [Test]
        public async Task EmptyPendingButCheckpointFailure_MakesZeroRequestsAndCanRetry()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data(6, false)) };
            LocalRecordRepository local = Load(store); AccountTransferPendingGate gate = new AccountTransferPendingGate(local);
            int calls = 0; store.FailWrite = true;
            Assert.That(await gate.TryRequestAsync(() => { calls++; return Task.FromResult(true); }), Is.False);
            Assert.That(calls, Is.Zero);
            store.FailWrite = false;
            Assert.That(await gate.TryRequestAsync(() => { calls++; return Task.FromResult(true); }), Is.True);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public async Task SharedRepositoryGate_CoalescesAAndBDoubleRequestsAndFencesNewPending()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data(6, false)) };
            LocalRecordRepository local = Load(store);
            TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>(); int calls = 0;
            Task<bool> first = new AccountTransferPendingGate(local).TryRequestAsync(() => { calls++; return completion.Task; });
            Assert.That(await new AccountTransferPendingGate(local).TryRequestAsync(() =>
                { calls++; return Task.FromResult(true); }), Is.False);
            Assert.That(local.TryEnqueuePending(Candidate()), Is.False);
            Assert.That(local.TryDiscardPending(), Is.False);
            completion.SetResult(true); Assert.That(await first, Is.True); Assert.That(calls, Is.EqualTo(1));
            Assert.That(local.TryEnqueuePending(Candidate()), Is.True);
        }

        [Test]
        public async Task CallbackFailure_ReleasesGateWithoutDiscardingState()
        {
            LocalRecordRepository local = Load(new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data(6, false)) });
            AccountTransferPendingGate gate = new AccountTransferPendingGate(local);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await gate.TryRequestAsync(() =>
                Task.FromException<bool>(new InvalidOperationException())));
            Assert.That(await gate.TryRequestAsync(() => Task.FromResult(false)), Is.False);
            Assert.That(await gate.TryRequestAsync(() => Task.FromResult(true)), Is.True);
        }

        [Test]
        public async Task CorruptPending_IsNotSilentlyDroppedToUnlockTransfer()
        {
            string bad = LocalSaveJsonCodec.Serialize(Data()).Replace(SubmissionId, "invalid-id");
            Assert.That(LocalSaveJsonCodec.TryDeserialize(bad, out LocalSaveData ignored), Is.False);
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = bad };
            LogAssert.Expect(LogType.Warning, "[LocalRecordRepository] Save recovery used default values.");
            LocalRecordRepository local = Load(store); int calls = 0;
            Assert.That(await new AccountTransferPendingGate(local).TryRequestAsync(() =>
                { calls++; return Task.FromResult(true); }), Is.False);
            Assert.That(local.TryCheckpoint(), Is.False); Assert.That(calls, Is.Zero);
            Assert.That(store.Contents, Is.EqualTo(bad));
        }

        [Test]
        public void CurrentVersionMissingScopeOrDuplicateScope_IsRejected()
        {
            string missing = "{\"version\":6,\"accountId\":\"owner\"}";
            Assert.That(LocalSaveJsonCodec.TryDeserialize(missing, out LocalSaveData ignored), Is.False);
            LocalSaveData source = Data();
            OnlineLocalSaveData duplicate = new OnlineLocalSaveData(source.OnlineScope, source.OnlineAccount,
                source.PersonalBests, source.PendingSubmissions);
            LocalSaveData invalid = new LocalSaveData(6, Owner, source.Settings, true, source.PersonalBests,
                source.OnlineAccount, source.PendingSubmissions, source.OnlineScope, new[] { duplicate });
            Assert.That(LocalSaveJsonCodec.TryDeserialize(LocalSaveJsonCodec.Serialize(invalid), out ignored), Is.False);
            string duplicateJson = LocalSaveJsonCodec.Serialize(source).Replace("\"inactiveOnlineAreas\":[]",
                "\"inactiveOnlineAreas\":[{\"projectId\":\"" + source.OnlineScope.ProjectId +
                "\",\"environmentId\":\"" + source.OnlineScope.EnvironmentId + "\"}]");
            Assert.That(LocalSaveJsonCodec.TryDeserialize(duplicateJson, out ignored), Is.False);
        }

        [Test]
        public void Reload_DoesNotMergePreviouslyLoadedQueues()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data()) };
            LocalRecordRepository local = Load(store);
            store.Contents = LocalSaveJsonCodec.Serialize(Data(6, false));
            local.TryLoad(out LocalSaveData ignored);
            Assert.That(local.CreatePendingSnapshot(), Is.Empty);
        }

        [Test]
        public void TrySaveForeignScope_IsRejectedWithoutReplacingFile()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data()) };
            LocalRecordRepository local = Load(store); string before = store.Contents;
            Assert.That(local.TrySave(Data(6, true, new OnlineDataScope("another-project", "another-environment"))), Is.False);
            Assert.That(store.Contents, Is.EqualTo(before));
        }

        [Test]
        public void SettingsCheckpointCannotDropInactiveAreas()
        {
            LocalSaveData source = Data().SelectOnlineScope(new OnlineDataScope("other", "other"));
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(source) };
            LocalRecordRepository local = Load(store, source.OnlineScope); string before = store.Contents;
            Assert.That(local.TrySave(new LocalSaveData(6, Owner, source.Settings, true, null,
                null, null, source.OnlineScope)), Is.False);
            Assert.That(store.Contents, Is.EqualTo(before));
            Assert.That(local.TrySave(local.CreateSaveData(Owner, new LocalSettingsData(25, true, null), true)), Is.True);
            Assert.That(Load(store).CreatePendingSnapshot(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task ProductionQueryAndSubmissionGate_BlocksUnpersistedMigrationBeforeAuthentication()
        {
            OnlineTestFileStore store = new OnlineTestFileStore { Contents = LocalSaveJsonCodec.Serialize(Data(5)), FailWrite = true };
            LocalRecordRepository local = Load(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication { Player = "ugs-original" };
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner, () => local.OnlineAccount,
                auth, transport, () => local.CanUseOnlineData);
            Assert.That((await online.GetTopAsync(Candidate().BoardKey)).status, Is.EqualTo("TransientFailure"));
            Assert.That((await online.SubmitAsync(Candidate())).Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
            store.FailWrite = false; Assert.That(local.TryCheckpoint(), Is.True);
            Assert.That((await online.GetTopAsync(Candidate().BoardKey)).status, Is.EqualTo("Success"));
            Assert.That(transport.Calls, Is.EqualTo(1));
        }
    }
}
