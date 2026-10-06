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
        private const string NewSubmissionId = "00000000-0000-4000-8000-000000000002";

        private static RecordSubmissionCandidate Candidate(string owner = Owner,
            string submissionId = SubmissionId)
        {
            Assert.That(RecordSubmissionPolicy.TryCreateStageCandidate(owner, submissionId,
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
            Assert.That((await online.SubmitAsync(Candidate())).Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
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
            Assert.That((await online.SubmitAsync(Candidate())).Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
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
            Assert.That((await online.SubmitAsync(Candidate())).Result, Is.EqualTo(expected));
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
            Assert.That((await online.SubmitAsync(Candidate("different-owner"))).Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
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

        [Test]
        public async Task RetrySubmission_OnlyAttemptsTheCurrentResult()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            Assert.That(local.TryEnqueuePending(Candidate(Owner, NewSubmissionId)), Is.True);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport));
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);

            await coordinator.RetrySubmissionAsync(NewSubmissionId);

            Assert.That(transport.Calls, Is.EqualTo(1));
            Assert.That(transport.Request, Does.Contain(NewSubmissionId));
            Assert.That(local.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            Assert.That(local.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(coordinator.TryGetTerminalResult(NewSubmissionId,
                out OnlineSubmissionResult result), Is.True);
            Assert.That(result.Result, Is.EqualTo(E_RecordSubmissionResult.Submitted));
        }

        [Test]
        public async Task RetrySubmission_WaitsForRecoveryThenAttemptsCurrentResult()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            Assert.That(local.TryEnqueuePending(Candidate(Owner, NewSubmissionId)), Is.True);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            BlockingOldSubmissionRepository online = new BlockingOldSubmissionRepository();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                online, milliseconds => Task.CompletedTask);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);

            Task recovery = coordinator.RetryPendingAsync();
            Task currentResult = coordinator.RetrySubmissionAsync(NewSubmissionId);
            Assert.That(currentResult.IsCompleted, Is.False);
            online.ReleaseOldSubmission.SetResult(
                new OnlineSubmissionResult(E_RecordSubmissionResult.TransientFailure));
            await recovery;
            await currentResult;

            Assert.That(online.NewSubmissionCalls, Is.EqualTo(1));
            Assert.That(coordinator.TryGetTerminalResult(NewSubmissionId,
                out OnlineSubmissionResult result), Is.True);
            Assert.That(result.Result, Is.EqualTo(E_RecordSubmissionResult.Submitted));
        }

        [Test]
        public async Task RetryAllPending_ContinuesAfterOlderTransientFailure()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            Assert.That(local.TryEnqueuePending(Candidate(Owner, NewSubmissionId)), Is.True);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            BlockingOldSubmissionRepository online = new BlockingOldSubmissionRepository();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                online, milliseconds => Task.CompletedTask);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);

            Task retry = coordinator.RetryAllPendingAsync();
            online.ReleaseOldSubmission.SetResult(
                new OnlineSubmissionResult(E_RecordSubmissionResult.TransientFailure));
            await retry;

            Assert.That(online.NewSubmissionCalls, Is.EqualTo(1));
            Assert.That(local.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            Assert.That(local.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
        }

        private sealed class BlockingOldSubmissionRepository : IOnlineRecordRepository
        {
            public readonly TaskCompletionSource<OnlineSubmissionResult> ReleaseOldSubmission =
                new TaskCompletionSource<OnlineSubmissionResult>();
            public int NewSubmissionCalls;

            public Task<OnlineSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate)
            {
                if (candidate.SubmissionId == SubmissionId) return ReleaseOldSubmission.Task;
                NewSubmissionCalls++;
                return Task.FromResult(new OnlineSubmissionResult(E_RecordSubmissionResult.Submitted));
            }

            public Task<OnlineLeaderboardResult> GetTopAsync(RecordBoardKey boardKey, int limit = 20)
            {
                return Task.FromResult(new OnlineLeaderboardResult());
            }

            public Task<OnlineLeaderboardResult> GetAroundAsync(RecordBoardKey boardKey, int limit = 20)
            {
                return Task.FromResult(new OnlineLeaderboardResult());
            }

            public Task<OnlineLeaderboardResult> GetPersonalBestAsync(RecordBoardKey boardKey)
            {
                return Task.FromResult(new OnlineLeaderboardResult());
            }
        }

        [TestCase("Submitted")]
        [TestCase("Rejected")]
        public async Task TerminalResultIsKeptOnlyForCurrentSession(string status)
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport { Status = status };
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport));
            await coordinator.ConfirmRecoveryNoticeAsync(); await coordinator.RetryPendingAsync();
            Assert.That(local.CreatePendingSnapshot(), Is.Empty);
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult result), Is.True);
            Assert.That(result.Result.ToString(), Is.EqualTo(status));
            await coordinator.RetryPendingAsync(); Assert.That(transport.Calls, Is.EqualTo(1));
            LocalRecordRepository restored = new LocalRecordRepository(store);
            restored.TryLoad(out LocalSaveData data);
            Assert.That(restored.CreatePendingSnapshot(), Is.Empty);
            Assert.That(data.Settings.MasterVolume, Is.EqualTo(75));
            Assert.That(data.HasCompletedTutorial, Is.True);
            Assert.That(data.OnlineAccount.PlayerId, Is.EqualTo("ugs-player"));
            Assert.That(store.Contents, Does.Not.Contain("submittedIds"));
            Assert.That(store.Contents, Does.Not.Contain("rejectedReceipts"));
        }

        [Test]
        public async Task TransientFailure_RestartRetriesOriginalPendingId()
        {
            OnlineTestFileStore store = new OnlineTestFileStore();
            LocalRecordRepository local = Local(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport { Status = "TransientFailure" };
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport),
                milliseconds => Task.CompletedTask);
            await coordinator.ConfirmRecoveryNoticeAsync();
            await coordinator.RetryPendingAsync();

            LocalRecordRepository restored = new LocalRecordRepository(store);
            restored.TryLoad(out LocalSaveData ignored);
            Assert.That(restored.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            transport.Status = "Submitted";
            OnlineRecordCoordinator restarted = new OnlineRecordCoordinator(Owner, restored, auth,
                new CloudCodeRecordRepository(Owner, () => restored.OnlineAccount, auth, transport));
            Assert.That(restarted.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult result), Is.False);
            await restarted.RetryPendingAsync();
            Assert.That(transport.Request, Does.Contain(SubmissionId));
            Assert.That(restored.CreatePendingSnapshot(), Is.Empty);
            LocalRecordRepository nextRestart = new LocalRecordRepository(store);
            nextRestart.TryLoad(out ignored);
            Assert.That(nextRestart.CreatePendingSnapshot(), Is.Empty);
        }

        [Test]
        public void PendingRemoval_FailedWriteRetainsCandidateAndOtherSaveData()
        {
            OnlineTestFileStore store = new OnlineTestFileStore();
            LocalRecordRepository local = Local(store);
            string original = store.Contents;
            store.FailWrite = true;
            Assert.That(local.TryRemovePending(Owner, SubmissionId), Is.False);
            Assert.That(store.Contents, Is.EqualTo(original));
            Assert.That(local.CreatePendingSnapshot(), Has.Count.EqualTo(1));
            store.FailWrite = false;
            Assert.That(local.TryRemovePending(Owner, SubmissionId), Is.True);
            LocalRecordRepository restored = new LocalRecordRepository(store);
            restored.TryLoad(out LocalSaveData data);
            Assert.That(restored.CreatePendingSnapshot(), Is.Empty);
            Assert.That(data.Settings.MasterVolume, Is.EqualTo(75));
            Assert.That(data.HasCompletedTutorial, Is.True);
        }

        [Test]
        public async Task RejectedResult_KeepsServerReasonOnlyForCurrentSession()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport { Status = "Rejected", Reason = "InvalidScore" };
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport));

            await coordinator.ConfirmRecoveryNoticeAsync(); await coordinator.RetryPendingAsync();
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult result), Is.True);
            Assert.That(result.Reason, Is.EqualTo("InvalidScore"));
            await coordinator.RetryPendingAsync();
            Assert.That(transport.Calls, Is.EqualTo(1));

            LocalRecordRepository restored = new LocalRecordRepository(store);
            restored.TryLoad(out LocalSaveData ignored);
            Assert.That(restored.CreatePendingSnapshot(), Is.Empty);
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
        public void VersionOneMigration_PreservesNoImplicitConsent()
        {
            LocalSaveData source = new LocalSaveData(1, Owner, new LocalSettingsData(100, false, null),
                false, null);
            Assert.That(LocalSaveJsonCodec.TryDeserialize(LocalSaveJsonCodec.Serialize(source), out LocalSaveData migrated), Is.True);
            Assert.That(migrated.Version, Is.EqualTo(LocalSaveData.CurrentVersion));
            Assert.That(migrated.OnlineAccount.HasConfirmedRecoveryNotice, Is.False);
            Assert.That(migrated.PersonalBests, Is.Empty);
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
        public async Task QuerySuccessWithNoEntries_IsKeptAsAnEmptySuccessfulResponse()
        {
            OnlineTestTransport transport = new OnlineTestTransport
            {
                QueryResponse = new OnlineLeaderboardResult
                {
                    status = "Success", entries = new OnlineLeaderboardEntry[0]
                }
            };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key);

            OnlineLeaderboardResult result = await online.GetTopAsync(key);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.entries, Is.Empty);
        }

        [Test]
        public async Task AuthenticationFailure_DoesNotSendCandidate()
        {
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"),
                new OnlineTestAuthentication { Success = false }, transport);
            Assert.That((await online.SubmitAsync(Candidate())).Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(transport.Calls, Is.Zero);
        }

        [Test]
        public async Task SubmissionTimeout_RetryRetainsOriginalPendingAcrossRestart()
        {
            OnlineTestFileStore store = new OnlineTestFileStore();
            LocalRecordRepository local = Local(store);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport { Timeout = true };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => local.OnlineAccount, auth, transport);
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(
                Owner, local, auth, online, milliseconds => Task.CompletedTask);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(local.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            for (int attempt = 0; attempt < RecordSubmissionQueue.MaximumAttemptsPerRetryTrigger; attempt++)
            {
                UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning,
                    "[CloudCodeRecordRepository] Submission unavailable; pending retained.");
            }

            await coordinator.RetryPendingAsync();

            Assert.That(transport.Calls, Is.EqualTo(RecordSubmissionQueue.MaximumAttemptsPerRetryTrigger));
            Assert.That(transport.Endpoint, Is.EqualTo("submit-record"));
            Assert.That(transport.Request, Does.Contain(SubmissionId));
            Assert.That(coordinator.SubmittedCount, Is.Zero);
            Assert.That(coordinator.RejectedCount, Is.Zero);
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult ignoredResult), Is.False);
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(local.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            LocalRecordRepository restored = new LocalRecordRepository(store);
            Assert.That(restored.TryLoad(out LocalSaveData ignoredSave), Is.True);
            Assert.That(restored.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(restored.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
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
        public async Task QueryOfflineFailure_HasClientFailureReason()
        {
            OnlineTestTransport transport = new OnlineTestTransport
            {
                Failure = new Exception("network unavailable")
            };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), new OnlineTestAuthentication(), transport);
            RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key);
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning,
                "[CloudCodeRecordRepository] Query unavailable.");

            OnlineLeaderboardResult result = await online.GetTopAsync(key);

            Assert.That(result.status, Is.EqualTo("TransientFailure"));
            Assert.That(result.reason, Is.EqualTo("ClientFailure"));
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
