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
            Assert.That(RecordSubmissionPolicy.TryCreateInfiniteCandidate(owner, submissionId,
                ScoringVersion.Current, 60000, 1250, 0, 1250, 0, 1250, 1.0,
                new InfiniteScoreLimit(ScoringVersion.Current, 2, 10, 3, 2, 1, 5), out RecordSubmissionCandidate candidate), Is.True);
            return candidate;
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task RetiredStage_DirectRequests_DoNotAuthenticateTransmitOrMutateLocal(bool consent)
        {
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = Local(file);
            if (consent) Assert.That(local.TrySaveOnlineAccount(new OnlineAccountState(true, "ugs-player")), Is.True);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport);
            if (consent)
            {
                Assert.That((await online.GetTopAsync(Candidate().BoardKey)).IsSuccess, Is.True);
                Assert.That(auth.Calls, Is.EqualTo(1));
                auth.Calls = 0; transport.Calls = 0;
            }
            string before = file.Contents;
            int writes = file.WriteAttempts;
            Assert.That(RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey stage), Is.True);
            foreach (OnlineLeaderboardResult result in new[] { await online.GetTopAsync(stage),
                await online.GetAroundAsync(stage), await online.GetPersonalBestAsync(stage) })
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.status, Is.EqualTo("TransientFailure"));
                Assert.That(result.reason, Is.EqualTo("StageModeUnsupported"));
            }
            OnlineSubmissionResult submitted = await online.SubmitAsync(SyntheticCandidate(stage));
            Assert.That(submitted.Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            Assert.That(submitted.Reason, Is.EqualTo("StageModeUnsupported"));
            Assert.That(local.TryGetPersonalBest(Owner, stage, out RecordSubmissionCandidate hidden), Is.False);
            Assert.That(hidden, Is.Null);
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
            Assert.That(file.WriteAttempts, Is.EqualTo(writes)); Assert.That(file.Contents, Is.EqualTo(before));
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(LocalSaveJsonCodec.Serialize(local.CreateSaveData(Owner,
                new LocalSettingsData(75, true, null), true)), Is.EqualTo(before));
        }

        [TestCase((E_GameMode)999, 2, "")]
        [TestCase(E_GameMode.Infinite, 1, "")]
        [TestCase(E_GameMode.Infinite, 2, "stage-001")]
        public async Task InvalidBoard_DirectRequests_DoNotAuthenticateOrTransmit(E_GameMode mode, int version, string stageId)
        {
            RecordBoardKey board = (RecordBoardKey)Activator.CreateInstance(typeof(RecordBoardKey),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null,
                new object[] { mode, stageId, version }, null);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), auth, transport);
            foreach (OnlineLeaderboardResult result in new[] { await online.GetTopAsync(board),
                await online.GetAroundAsync(board), await online.GetPersonalBestAsync(board) })
                Assert.That(result.reason, Is.EqualTo("InvalidQuery"));
            OnlineSubmissionResult resultSubmit = await online.SubmitAsync(SyntheticCandidate(board));
            Assert.That(resultSubmit.Result, Is.EqualTo(E_RecordSubmissionResult.Rejected));
            Assert.That(resultSubmit.Reason, Is.EqualTo("InvalidBoard"));
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = Local(file);
            string before = file.Contents;
            int writes = file.WriteAttempts;
            Assert.That(local.TryGetPersonalBest(Owner, board, out RecordSubmissionCandidate hidden), Is.False);
            Assert.That(hidden, Is.Null);
            Assert.That(file.Contents, Is.EqualTo(before)); Assert.That(file.WriteAttempts, Is.EqualTo(writes));
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
        }

        [Test]
        public async Task InfiniteRequests_KeepSubmissionPayloadAndIndependentQueryWindows()
        {
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(Owner,
                () => new OnlineAccountState(true, "ugs-player"), auth, transport);
            RecordSubmissionCandidate candidate = Candidate();
            Assert.That((await online.SubmitAsync(candidate)).Result, Is.EqualTo(E_RecordSubmissionResult.Submitted));
            Assert.That(transport.Endpoint, Is.EqualTo("submit-record"));
            OnlineRecordRequest request = UnityEngine.JsonUtility.FromJson<OnlineRecordRequest>(transport.Request);
            Assert.That(request.boardId, Is.EqualTo("fs-infinite-v2"));
            Assert.That(request.rulesVersion, Is.EqualTo(ScoringVersion.Current));
            Assert.That(request.submissionId, Is.EqualTo(candidate.SubmissionId));
            Assert.That(request.score, Is.EqualTo(candidate.RankingValue));
            Assert.That(request.runDurationMilliseconds, Is.EqualTo(candidate.RunDurationMilliseconds));
            Assert.That(request.baseDistanceScore, Is.EqualTo(candidate.BaseDistanceScore));
            Assert.That(request.momentumBonus, Is.EqualTo(candidate.MomentumBonus));
            Assert.That(request.distanceScore, Is.EqualTo(candidate.DistanceScore));
            Assert.That(request.collectibleScore, Is.EqualTo(candidate.CollectibleScore));
            Assert.That(request.totalScore, Is.EqualTo(candidate.TotalScore));
            Assert.That(request.maximumMomentumMultiplier, Is.EqualTo(candidate.MaximumMomentumMultiplier));
            await online.GetTopAsync(candidate.BoardKey);
            request = UnityEngine.JsonUtility.FromJson<OnlineRecordRequest>(transport.Request);
            Assert.That(request.kind, Is.EqualTo("top")); Assert.That(request.limit, Is.EqualTo(10));
            await online.GetAroundAsync(candidate.BoardKey);
            request = UnityEngine.JsonUtility.FromJson<OnlineRecordRequest>(transport.Request);
            Assert.That(request.kind, Is.EqualTo("around")); Assert.That(request.limit, Is.EqualTo(7));
            await online.GetPersonalBestAsync(candidate.BoardKey);
            request = UnityEngine.JsonUtility.FromJson<OnlineRecordRequest>(transport.Request);
            Assert.That(request.kind, Is.EqualTo("me")); Assert.That(request.limit, Is.EqualTo(1));
            Assert.That(request.boardId, Is.EqualTo("fs-infinite-v2"));
            Assert.That(transport.Endpoint, Is.EqualTo("query-records")); Assert.That(transport.Calls, Is.EqualTo(4));
        }

        private static RecordSubmissionCandidate SyntheticCandidate(RecordBoardKey board, string owner = Owner, long createdAt = 0)
        {
            return (RecordSubmissionCandidate)Activator.CreateInstance(typeof(RecordSubmissionCandidate),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null,
                new object[] { owner, SubmissionId, board, 1250L, 60000L, 1250, 0, 1250, 0, 1250, 1.0, createdAt }, null);
        }

        private static LocalRecordRepository RestoredPending(OnlineTestFileStore file,
            RecordSubmissionCandidate candidate, OnlineDataScope scope = null)
        {
            OnlineDataScope active = scope == null ? OnlineDataScope.CreateConfigured() : scope;
            file.Contents = LocalSaveJsonCodec.Serialize(new LocalSaveData(LocalSaveData.CurrentVersion,
                Owner, new LocalSettingsData(75, true, null), true, new[] { candidate },
                new OnlineAccountState(true, "ugs-player"), new[] { candidate }, active));
            LocalRecordRepository local = new LocalRecordRepository(file, active);
            Assert.That(local.TryLoad(out LocalSaveData loaded), Is.True);
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            return local;
        }

        [TestCase("background", true)]
        [TestCase("single", true)]
        [TestCase("all", true)]
        [TestCase("background", false)]
        [TestCase("single", false)]
        [TestCase("all", false)]
        public async Task UnsupportedPending_AllRetryEntries_KeepDataWithoutClockSaveAuthenticationOrTransport(
            string trigger, bool stage)
        {
            RecordBoardKey board;
            if (stage) Assert.That(RecordBoardKey.TryCreateStage("stage-001", 1, out board), Is.True);
            else Assert.That(RecordBoardKey.TryCreateInfinite(ScoringVersion.LegacyDistanceScore, out board), Is.True);
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = RestoredPending(file, SyntheticCandidate(board, createdAt: 1000));
            string before = file.Contents; int writes = file.WriteAttempts, clockCalls = 0;
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport),
                milliseconds => { Assert.Fail("No retry delay expected."); return Task.CompletedTask; },
                () => { clockCalls++; return 1000 + LocalRecordRepository.PendingRetentionMilliseconds; });
            if (trigger == "background") await coordinator.RetryPendingAsync();
            else if (trigger == "single") await coordinator.RetrySubmissionAsync(SubmissionId);
            else await coordinator.RetryAllPendingAsync();
            Assert.That(clockCalls, Is.Zero); Assert.That(file.WriteAttempts, Is.EqualTo(writes));
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
            Assert.That(file.Contents, Is.EqualTo(before)); Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(coordinator.SubmittedCount, Is.Zero); Assert.That(coordinator.RejectedCount, Is.Zero);
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult terminal), Is.False);
            LocalRecordRepository restarted = new LocalRecordRepository(file);
            Assert.That(restarted.TryLoad(out LocalSaveData restored), Is.True);
            Assert.That(restored.PendingSubmissions[0].SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(restored.PersonalBests[0].BoardKey, Is.EqualTo(board));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task ForeignOwnerOrScope_RetryKeepsExpiredPendingBeforeAnySideEffect(bool foreignScope)
        {
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey board);
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = RestoredPending(file,
                SyntheticCandidate(board, foreignScope ? Owner : "other-owner", 1000),
                foreignScope ? new OnlineDataScope("other-project", "other-environment") : null);
            string before = file.Contents; int writes = file.WriteAttempts, clockCalls = 0;
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport), now:
                () => { clockCalls++; return 1000 + LocalRecordRepository.PendingRetentionMilliseconds; });
            await coordinator.RetryAllPendingAsync();
            Assert.That(clockCalls, Is.Zero); Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Calls, Is.Zero);
            Assert.That(file.Contents, Is.EqualTo(before)); Assert.That(file.WriteAttempts, Is.EqualTo(writes));
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TransientThenSuccess_ReusesSameIdAndCommitsOnce()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineTestTransport transport = new OnlineTestTransport { Status = "TransientFailure" };
            List<int> delays = new List<int>(); string first = null;
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport), milliseconds =>
                { delays.Add(milliseconds); first = transport.Request; transport.Status = "Submitted"; return Task.CompletedTask; });
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);
            await coordinator.RetryAllPendingAsync();
            Assert.That(transport.Calls, Is.EqualTo(2)); Assert.That(delays, Is.EqualTo(new[] { 1000 }));
            Assert.That(transport.Request, Is.EqualTo(first)); Assert.That(local.CreatePendingSnapshot(), Is.Empty);
            Assert.That(coordinator.SubmittedCount, Is.EqualTo(1)); Assert.That(coordinator.RejectedCount, Is.Zero);
        }

        [Test]
        public async Task DuplicateBackgroundRetry_CoalescesWhileSubmissionIsInFlight()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            BlockingOldSubmissionRepository online = new BlockingOldSubmissionRepository();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth, online);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);
            Task first = coordinator.RetryPendingAsync();
            Task duplicate = coordinator.RetryPendingAsync();
            Assert.That(first.IsCompleted, Is.False); Assert.That(duplicate.IsCompleted, Is.True);
            Assert.That(online.OldSubmissionCalls, Is.EqualTo(1));
            online.ReleaseOldSubmission.SetResult(new OnlineSubmissionResult(E_RecordSubmissionResult.Submitted));
            await first; await duplicate;
            Assert.That(online.OldSubmissionCalls, Is.EqualTo(1)); Assert.That(coordinator.SubmittedCount, Is.EqualTo(1));
        }

        [TestCase(E_RecordSubmissionResult.Submitted)]
        [TestCase(E_RecordSubmissionResult.Rejected)]
        public async Task TerminalResponse_FailedLocalCommit_RetainsIdAndCountsOnlyAfterDurableRetry(E_RecordSubmissionResult terminal)
        {
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = Local(file);
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            BlockingOldSubmissionRepository online = new BlockingOldSubmissionRepository();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth, online);
            Assert.That(await coordinator.ConfirmRecoveryNoticeAsync(), Is.True);
            Task retry = coordinator.RetryPendingAsync();
            Assert.That(retry.IsCompleted, Is.False);
            string before = file.Contents; file.FailWrite = true;
            online.ReleaseOldSubmission.SetResult(new OnlineSubmissionResult(terminal, "InvalidScore"));
            await retry;
            Assert.That(file.Contents, Is.EqualTo(before)); Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(coordinator.SubmittedCount, Is.Zero); Assert.That(coordinator.RejectedCount, Is.Zero);
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult ignored), Is.False);
            LocalRecordRepository restored = new LocalRecordRepository(file);
            Assert.That(restored.TryLoad(out LocalSaveData loaded), Is.True);
            Assert.That(restored.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            file.FailWrite = false;
            await coordinator.RetryPendingAsync();
            Assert.That(online.OldSubmissionCalls, Is.EqualTo(2));
            Assert.That(local.CreatePendingSnapshot(), Is.Empty);
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult result), Is.True);
            Assert.That(result.Result, Is.EqualTo(terminal));
            Assert.That(coordinator.SubmittedCount + coordinator.RejectedCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ExpiredInfinitePending_EmptyQueueDoesNotCreateTerminalSuccess()
        {
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey board);
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = RestoredPending(file, SyntheticCandidate(board, createdAt: 1000));
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); OnlineTestTransport transport = new OnlineTestTransport();
            OnlineRecordCoordinator coordinator = new OnlineRecordCoordinator(Owner, local, auth,
                new CloudCodeRecordRepository(Owner, () => local.OnlineAccount, auth, transport),
                now: () => 1000 + LocalRecordRepository.PendingRetentionMilliseconds);
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning,
                "[OnlineRecordCoordinator] SubmissionExpired: old Pending records were removed.");
            await coordinator.RetryPendingAsync();
            Assert.That(local.CreatePendingSnapshot(), Is.Empty); Assert.That(transport.Calls, Is.Zero);
            Assert.That(coordinator.SubmittedCount, Is.Zero); Assert.That(coordinator.RejectedCount, Is.Zero);
            Assert.That(coordinator.TryGetTerminalResult(SubmissionId, out OnlineSubmissionResult terminal), Is.False);
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
            public int OldSubmissionCalls;

            public Task<OnlineSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate)
            {
                if (candidate.SubmissionId == SubmissionId) { OldSubmissionCalls++; return ReleaseOldSubmission.Task; }
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
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey key);
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
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey key);

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
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey key);
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
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey key);
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
            RecordBoardKey.TryCreateInfinite(ScoringVersion.Current, out RecordBoardKey key);
            Assert.That((await online.GetTopAsync(key)).reason, Is.EqualTo("AuthenticationUnavailable"));
            Assert.That(transport.Calls, Is.Zero);
        }
    }
}
