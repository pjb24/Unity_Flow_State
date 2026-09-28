using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class OnlineRecordRepositoryTests
    {
        private const string Owner = "local-owner";
        private const string SubmissionId = "00000000-0000-4000-8000-000000000001";

        private static RecordSubmissionCandidate Candidate(string owner = Owner)
        {
            Assert.That(RecordSubmissionPolicy.TryCreateStageCandidate(owner, SubmissionId,
                "stage-001", 1, E_StageResultType.Cleared, 1.25, out RecordSubmissionCandidate candidate), Is.True);
            return candidate;
        }

        private static LocalRecordRepository Local(OnlineTestFileStore store)
        {
            LocalRecordRepository local = new LocalRecordRepository(store);
            local.TryLoad(out LocalSaveData ignored);
            local.TryEnqueuePending(Candidate());
            local.TrySave(local.CreateSaveData(Owner, new LocalSettingsData(75, true, null), true));
            return local;
        }

        [Test]
        public async Task NoConsent_DoesNotAuthenticateOrSubmit()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport);
            await new OnlineRecordCoordinator(Owner, local, auth, online).RetryPendingAsync();
            Assert.That(await online.SubmitAsync(Candidate()), Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
        }

        [Test]
        public async Task ConsentBindsOnce_AndNeverTransfersToDifferentAccount()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport);
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth, online);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);
            auth.Player = "different-player";
            await coordinator.RetryPendingAsync();
            Assert.That(await online.SubmitAsync(Candidate()), Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(local.OnlineAccount.PlayerId, Is.EqualTo("ugs-player"));
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1)); Assert.That(transport.Calls, Is.Zero);
        }

        [TestCase("Submitted", E_RecordSubmissionResult.Submitted)]
        [TestCase("Rejected", E_RecordSubmissionResult.Rejected)]
        [TestCase("TransientFailure", E_RecordSubmissionResult.TransientFailure)]
        [TestCase("Unknown", E_RecordSubmissionResult.TransientFailure)]
        public async Task SubmissionMapsStatuses_AndOmitsClientPlayerId(string status, E_RecordSubmissionResult expected)
        {
            OnlineTestTransport transport = new OnlineTestTransport { Status = status };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            Assert.That(await online.SubmitAsync(Candidate()), Is.EqualTo(expected));
            Assert.That(transport.Endpoint, Is.EqualTo("submit-record"));
            Assert.That(transport.Request, Does.Contain(SubmissionId));
            Assert.That(transport.Request, Does.Not.Contain(Owner));
            Assert.That(transport.Request, Does.Not.Contain("playerId"));
        }

        [Test]
        public async Task ForeignLocalOwner_IsNeverSent()
        {
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            Assert.That(await online.SubmitAsync(Candidate("different-owner")), Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(transport.Calls, Is.Zero);
        }

        [Test]
        public async Task RetryHasThreeAttemptsWithBackoff_AndNextTriggerResetsBudget()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport { Status = "TransientFailure" };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport);
            List<int> delays = new List<int>();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth, online,
                milliseconds => { delays.Add(milliseconds); return Task.CompletedTask; });
            await coordinator.ConfirmRecoveryNoticeAsync(); await coordinator.RetryPendingAsync();
            Assert.That(transport.Calls, Is.EqualTo(3)); Assert.That(delays, Is.EqualTo(new[] {1000, 2000}));
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            await coordinator.RetryPendingAsync(); Assert.That(transport.Calls, Is.EqualTo(6));
        }

        [TestCase("Submitted")]
        [TestCase("Rejected")]
        public async Task TerminalReceiptSurvivesRestart_AndPreservesSettings(string status)
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport { Status = status };
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport));
            await coordinator.ConfirmRecoveryNoticeAsync(); await coordinator.RetryPendingAsync();
            LocalRecordRepository restored = new LocalRecordRepository(store);
            restored.TryLoad(out LocalSaveData data);
            Assert.That(restored.CreatePendingSnapshot(), Is.Empty);
            Assert.That(data.OnlineAccount.HasFinished(SubmissionId), Is.True);
            Assert.That(data.Settings.MasterVolume, Is.EqualTo(75)); Assert.That(data.HasCompletedTutorial, Is.True);
            await coordinator.RetryPendingAsync(); Assert.That(transport.Calls, Is.EqualTo(1));
        }

        [Test]
        public async Task ConsentDiskFailure_DoesNotAuthenticate()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store); store.FailWrite = true;
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth, null);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.False);
            Assert.That(auth.Calls, Is.Zero); Assert.That(local.OnlineAccount.HasConfirmedRecoveryNotice, Is.False);
        }

        [Test]
        public void VersionOneMigrationPreservesCandidate_WithoutImplicitConsent()
        {
            LocalSaveData source = new LocalSaveData(1, Owner, new LocalSettingsData(100, false, null),
                false, null, new[] { Candidate() });
            Assert.That(LocalSaveJsonCodec.TryDeserialize(LocalSaveJsonCodec.Serialize(source), out LocalSaveData migrated), Is.True);
            Assert.That(migrated.Version, Is.EqualTo(2));
            Assert.That(migrated.OnlineAccount.HasConfirmedRecoveryNotice, Is.False);
            Assert.That(migrated.PendingSubmissions[0].SubmissionId, Is.EqualTo(SubmissionId));
        }

        [Test]
        public async Task QueryContractsUseSameAuthenticatedEndpoint()
        {
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key);
            Assert.That((await online.GetTopAsync(key)).IsSuccess, Is.True);
            Assert.That((await online.GetAroundAsync(key)).IsSuccess, Is.True);
            Assert.That((await online.GetPersonalBestAsync(key)).IsSuccess, Is.True);
            Assert.That(transport.Endpoint, Is.EqualTo("query-records"));
            Assert.That(transport.Calls, Is.EqualTo(3));
        }

        [Test]
        public async Task AuthenticationFailure_DoesNotSendCandidate()
        {
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"),
                new OnlineTestAuthentication { Success = false }, transport);
            Assert.That(await online.SubmitAsync(Candidate()), Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(transport.Calls, Is.Zero);
        }

        [Test]
        public async Task Timeout_RemainsPending()
        {
            OnlineTestTransport transport = new OnlineTestTransport { Timeout = true };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning,
                "[CloudCodeRecordRepository] Submission unavailable; pending retained.");
            Assert.That(await online.SubmitAsync(Candidate()), Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
        }

        [Test]
        public async Task PendingCheckpointFailure_PreventsRemoteSideEffect()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport));
            await coordinator.ConfirmRecoveryNoticeAsync(); store.FailWrite = true;
            await coordinator.RetryPendingAsync(); Assert.That(transport.Calls, Is.Zero);
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
        }

        [Test]
        public async Task QueryTimeout_HasSpecificReason_AndNextQueryCanSucceed()
        {
            OnlineTestTransport transport = new OnlineTestTransport { Timeout = true };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key);
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning,
                "[CloudCodeRecordRepository] Query timed out.");
            OnlineLeaderboardResult top = await online.GetTopAsync(key);
            Assert.That(top.status, Is.EqualTo("TransientFailure"));
            Assert.That(top.reason, Is.EqualTo("Timeout"));
            transport.Timeout = false;
            Assert.That((await online.GetAroundAsync(key)).IsSuccess, Is.True);
            Assert.That((await online.GetPersonalBestAsync(key)).IsSuccess, Is.True);
        }

        [Test]
        public async Task QueryAuthenticationFailure_HasSpecificReason_WithoutCallingTransport()
        {
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"),
                new OnlineTestAuthentication { Success = false }, transport);
            RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key);
            Assert.That((await online.GetTopAsync(key)).reason, Is.EqualTo("AuthenticationUnavailable"));
            Assert.That(transport.Calls, Is.Zero);
        }
    }
}
