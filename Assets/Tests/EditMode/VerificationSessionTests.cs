using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class VerificationSessionTests
    {
        [Test]
        public void AccountTimingDiagnostics_DefaultToUnknownAndDeserializeWithoutAuthorityChanges()
        {
            OnlineAccountResponse missing = UnityEngine.JsonUtility.FromJson<OnlineAccountResponse>("{\"status\":\"Inactive\"}");
            // Do not use missing diagnostics as a zero-duration measurement.
            OnlineAccountResponse response = UnityEngine.JsonUtility.FromJson<OnlineAccountResponse>(
                "{\"status\":\"Inactive\",\"serverElapsedMilliseconds\":123,\"serverServiceCalls\":2,\"serverTimingPresent\":true}");
            Assert.That(response.status, Is.EqualTo(missing.status));
            Assert.That(response.serverElapsedMilliseconds, Is.EqualTo(123));
            Assert.That(response.serverServiceCalls, Is.EqualTo(2));
            Assert.That(response.serverTimingPresent, Is.True);
            Assert.That(missing.serverTimingPresent, Is.False);
            Assert.That(response.publicPlayerNumber, Is.Null.Or.Empty);
            Assert.That(response.code, Is.Null.Or.Empty);
            Assert.That(response.verificationValue, Is.Null.Or.Empty);
            Assert.That(new OnlineAccountResponse().serverElapsedMilliseconds, Is.EqualTo(-1));
            Assert.That(new OnlineAccountResponse().serverServiceCalls, Is.EqualTo(-1));
        }

        private sealed class Boundary : IOnlineAuthenticationSession, IOnlineRecordTransport, IOnlineAccountTransport
        {
            public int Calls;
            public TaskCompletionSource<OnlineAuthenticationResult> Delayed;
            public TaskCompletionSource<object> DelayedTransport;
            public TaskCompletionSource<bool> DelayedClear;
            public Task<OnlineAuthenticationResult> TryAuthenticateAsync()
            { Calls++; return Delayed == null ? Task.FromResult(new OnlineAuthenticationResult(true, "fake-player")) : Delayed.Task; }
            public Task<bool> TryClearSessionAsync(string expectedPlayerId) { Calls++; return DelayedClear == null ? Task.FromResult(true) : DelayedClear.Task; }
            public async Task<T> CallAsync<T>(string endpoint, string request) { Calls++; return DelayedTransport == null ? default(T) : (T)await DelayedTransport.Task; }
            public async Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters) { Calls++; return DelayedTransport == null ? default(T) : (T)await DelayedTransport.Task; }
        }

        [TestCase("authentication")]
        [TestCase("account")]
        [TestCase("records")]
        [TestCase("clear")]
        [TestCase("retryDelay")]
        public async Task WindowBudget_BoundsEveryBoundaryAndDiscardsLateResponse(string boundary)
        {
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> retryDelay = new TaskCompletionSource<bool>();
            Boundary fake = new Boundary { Delayed = new TaskCompletionSource<OnlineAuthenticationResult>(),
                DelayedTransport = new TaskCompletionSource<object>(), DelayedClear = new TaskCompletionSource<bool>() };
            VerificationRemoteGuard guard = new VerificationRemoteGuard(() => true, fake, fake, fake,
                milliseconds => { Assert.That(milliseconds, Is.EqualTo(5000)); return deadline.Task; });
            guard.BeginOperation();
            Task work;
            switch (boundary)
            {
                case "authentication": work = guard.TryAuthenticateAsync(); break;
                case "account": work = guard.CallAsync<string>("status", new Dictionary<string, string>()); break;
                case "records": work = guard.CallAsync<string>("query", "{}"); break;
                case "clear": work = guard.TryClearSessionAsync("fake-player"); break;
                default: work = guard.WaitWithinOperationAsync(() => retryDelay.Task); break;
            }
            deadline.SetResult(true);
            Assert.ThrowsAsync<TimeoutException>(async () => await work);
            int calls = fake.Calls;
            fake.Delayed.SetResult(new OnlineAuthenticationResult(true, "fake-player"));
            fake.DelayedTransport.SetResult("late"); fake.DelayedClear.SetResult(true); retryDelay.SetResult(true);
            await Task.Yield();
            Assert.That(work.IsFaulted, Is.True);
            Assert.That(guard.OperationTimedOut, Is.True);
            Assert.ThrowsAsync<TimeoutException>(async () => await guard.CallAsync<string>("followup", "{}"));
            Assert.That(fake.Calls, Is.EqualTo(calls));
            guard.EndOperation();
        }

        [Test]
        public async Task WindowBudget_IsSharedAcrossCallsAndOnlyExplicitNextOperationResetsIt()
        {
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>(); int budgets = 0;
            Boundary fake = new Boundary();
            VerificationRemoteGuard guard = new VerificationRemoteGuard(() => true, fake, fake, fake,
                milliseconds => { budgets++; return deadline.Task; });
            guard.BeginOperation();
            Assert.That((await guard.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            await guard.CallAsync<string>("first", "{}");
            Assert.That(budgets, Is.EqualTo(1)); deadline.SetResult(true);
            Assert.ThrowsAsync<TimeoutException>(async () => await guard.CallAsync<string>("second", "{}"));
            Assert.That(fake.Calls, Is.EqualTo(2)); guard.EndOperation();
            deadline = new TaskCompletionSource<bool>(); guard.BeginOperation();
            Assert.That((await guard.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(budgets, Is.EqualTo(2)); Assert.That(deadline.Task.IsCompleted, Is.False); guard.EndOperation();
        }

        [Test]
        public async Task WindowBudget_ExpiredRetryDelayKeepsPendingAndStartsNoFollowupSubmission()
        {
            OnlineTestFileStore file = new OnlineTestFileStore();
            file.Contents = LocalSaveJsonCodec.Serialize(new LocalSaveData(6, "owner", new LocalSettingsData(60, true, null),
                true, null, new OnlineAccountState(true, "fake-player")));
            LocalRecordRepository local = new LocalRecordRepository(file); local.TryLoad(out LocalSaveData ignored);
            Assert.That(RecordSubmissionPolicy.TryCreateStageCandidate("owner", "11111111-1111-4111-8111-111111111111",
                "stage-001", 1, FlowState.Runtime.Core.E_StageResultType.Cleared, 60, out RecordSubmissionCandidate candidate), Is.True);
            Assert.That(local.TryEnqueuePending(candidate), Is.True);
            Boundary fake = new Boundary { DelayedTransport = new TaskCompletionSource<object>() };
            fake.DelayedTransport.SetResult(new OnlineSubmissionResponse { status = "TransientFailure", reason = "Timeout" });
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>(), retryDelay = new TaskCompletionSource<bool>();
            VerificationRemoteGuard guard = new VerificationRemoteGuard(() => true, fake, fake, fake, milliseconds => deadline.Task);
            CloudCodeRecordRepository records = new CloudCodeRecordRepository("owner", () => local.OnlineAccount, guard, guard);
            OnlineRecordCoordinator pending = new OnlineRecordCoordinator("owner", local, guard, records,
                milliseconds => guard.WaitWithinOperationAsync(() => retryDelay.Task));
            guard.BeginOperation(); Task work = pending.RetryAllPendingAsync(); int calls = fake.Calls;
            deadline.SetResult(true); await work;
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(pending.SubmittedCount, Is.Zero); Assert.That(fake.Calls, Is.EqualTo(calls));
            retryDelay.SetResult(true); await Task.Yield(); Assert.That(fake.Calls, Is.EqualTo(calls)); guard.EndOperation();
        }

        [TestCase(false, false, false, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, false, false, false)]
        public void ConsentTestBatchAndPlayGuards_BlockRemote(bool consent, bool test, bool batch, bool play)
        { Assert.That(VerificationSessionConfiguration.CanExecuteRemote(consent, test, batch, play, OnlineRecordConfiguration.ProjectId, true), Is.False); }

        [Test]
        public void ExplicitUserPlayModeExecution_RequiresIsolationAndExactProject()
        {
            Assert.That(VerificationSessionConfiguration.CanExecuteRemote(true, false, false, true, "other", true), Is.False);
            Assert.That(VerificationSessionConfiguration.CanExecuteRemote(true, false, false, true, OnlineRecordConfiguration.ProjectId), Is.False);
            Assert.That(VerificationSessionConfiguration.CanExecuteRemote(true, false, false, true, OnlineRecordConfiguration.ProjectId, true), Is.True);
        }
        [Test]
        public void ABAndLegacy_AreSeparatePathsAndProfiles_WithoutFilesystemWrites()
        {
            VerificationSessionConfiguration a = new VerificationSessionConfiguration("isolated-root", E_VerificationSession.A);
            VerificationSessionConfiguration b = new VerificationSessionConfiguration("isolated-root", E_VerificationSession.B);
            VerificationSessionConfiguration legacy = new VerificationSessionConfiguration("isolated-root", E_VerificationSession.Legacy);
            Assert.That(a.SavePath, Is.Not.EqualTo(b.SavePath)); Assert.That(a.Profile, Is.Not.EqualTo(b.Profile));
            Assert.That(legacy.SavePath, Is.Not.EqualTo(a.SavePath)); Assert.That(legacy.Profile, Is.EqualTo("flow-state-verification"));
            Assert.That(a.SavePath, Does.Contain("Prototype8Verification"));
        }
        [TestCase("A", E_VerificationSession.A)]
        [TestCase("B", E_VerificationSession.B)]
        [TestCase("Legacy", E_VerificationSession.Legacy)]
        public void ExplicitLaunchArgument_SelectsSameConfigurationAsTool(string argument, E_VerificationSession session)
        {
            VerificationSessionConfiguration selected = VerificationSessionConfiguration.FromArguments("root", new[] { "program", "--fs-verification-session=" + argument });
            Assert.That(selected.Profile, Is.EqualTo(new VerificationSessionConfiguration("root", session).Profile));
            Assert.That(selected.SavePath, Is.EqualTo(new VerificationSessionConfiguration("root", session).SavePath));
        }
        [Test]
        public void AbsentFlag_KeepsExistingDefaultConfiguration()
        { Assert.That(VerificationSessionConfiguration.FromArguments("root", new[] { "program" }), Is.Null); }
        [TestCase("invalid")]
        [TestCase("../A")]
        [TestCase("")]
        public void InvalidSessionArgument_FailsClosed(string value)
        { Assert.Throws<InvalidOperationException>(() => VerificationSessionConfiguration.FromArguments("root", new[] { "--fs-verification-session=" + value })); }
        [Test]
        public void DuplicateSessionArguments_FailClosed()
        { Assert.Throws<InvalidOperationException>(() => VerificationSessionConfiguration.FromArguments("root", new[] { "--fs-verification-session=A", "--fs-verification-session=B" })); }
        [Test]
        public async Task DefaultDisabledGuard_MakesZeroAuthenticationTokenOrTransportCalls()
        {
            Boundary fake = new Boundary(); VerificationRemoteGuard guard = new VerificationRemoteGuard(null, fake, fake, fake);
            Assert.That((await guard.TryAuthenticateAsync()).IsAuthenticated, Is.False); Assert.That(await guard.TryClearSessionAsync("fake-player"), Is.False);
            Assert.Throws<InvalidOperationException>(() => guard.CallAsync<object>("submit-record", "{}"));
            Assert.Throws<InvalidOperationException>(() => guard.CallAsync<object>("start-account-transfer", new Dictionary<string, string>()));
            Assert.That(fake.Calls, Is.Zero);
        }
        [Test]
        public async Task PermissionRevokedWhileAuthenticating_BlocksResultAndFollowupCalls()
        {
            bool allowed = true; Boundary fake = new Boundary { Delayed = new TaskCompletionSource<OnlineAuthenticationResult>() };
            VerificationRemoteGuard guard = new VerificationRemoteGuard(() => allowed, fake, fake, fake);
            Task<OnlineAuthenticationResult> request = guard.TryAuthenticateAsync(); allowed = false;
            fake.Delayed.SetResult(new OnlineAuthenticationResult(true, "fake-player")); Assert.That((await request).IsAuthenticated, Is.False);
            Assert.Throws<InvalidOperationException>(() => guard.CallAsync<object>("complete-account-transfer", new Dictionary<string, string>()));
            Assert.That(fake.Calls, Is.EqualTo(1));
        }

        private static OnlineLeaderboardResult Me(long score = 60000, long accepted = 100, string number = "0000000001")
        { return new OnlineLeaderboardResult { status = "Success", entries = new[] { new OnlineLeaderboardEntry { publicPlayerNumber = number, isMe = true, score = score, acceptedAt = accepted, rank = 1 } } }; }

        private static LocalRecordRepository PendingLocal(bool consent = true)
        {
            LocalRecordRepository local = new LocalRecordRepository(new OnlineTestFileStore());
            Assert.That(local.TryLoad(out LocalSaveData ignored), Is.True);
            // An empty file loads with an empty local owner, not a generated
            // account. Seed the local owner before constructing its candidate.
            Assert.That(local.TrySave(local.CreateSaveData("verification-local-owner",
                new LocalSettingsData(100, false, null), false)), Is.True);
            Assert.That(local.AccountId, Is.Not.Empty);
            if (consent)
                Assert.That(local.TrySaveOnlineAccount(new OnlineAccountState(true, "ugs-player")), Is.True);
            Assert.That(RecordSubmissionPolicy.TryCreateStageCandidate(local.AccountId,
                "00000000-0000-4000-8000-000000000001", "stage-001", 1,
                FlowState.Runtime.Core.E_StageResultType.Cleared, 60, out RecordSubmissionCandidate candidate), Is.True);
            Assert.That(local.TryEnqueuePending(candidate), Is.True);
            Assert.That(local.TryCheckpoint(), Is.True);
            return local;
        }

        [TestCase("Submitted", "PENDING_SUBMISSION_CONFIRMED", 0)]
        [TestCase("TransientFailure", "PENDING_NOT_CONFIRMED", 1)]
        [TestCase("Rejected", "PENDING_NOT_CONFIRMED", 1)]
        public async Task VerificationRetry_ReplaysOnceAndRemovesOnlyConfirmedSubmission(string status, string expected, int remaining)
        {
            LocalRecordRepository local = PendingLocal();
            OnlineTestTransport transport = new OnlineTestTransport { Status = status };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(local.AccountId,
                () => local.OnlineAccount, new OnlineTestAuthentication(), transport, () => local.CanUseOnlineData);
            string submissionId = local.CreatePendingSnapshot()[0].SubmissionId;
            Assert.That(await VerificationPendingRetry.RetryOnceAsync(local, online), Is.EqualTo(expected));
            Assert.That(transport.Calls, Is.EqualTo(1));
            Assert.That(transport.Request, Does.Contain(submissionId));
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(remaining));
            if (remaining != 0) Assert.That(local.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(submissionId));
        }

        [Test]
        public async Task VerificationRetry_ExplicitSecondAttemptUsesIdenticalSubmissionId()
        {
            LocalRecordRepository local = PendingLocal();
            OnlineTestTransport transport = new OnlineTestTransport { Status = "TransientFailure" };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(local.AccountId,
                () => local.OnlineAccount, new OnlineTestAuthentication(), transport);
            await VerificationPendingRetry.RetryOnceAsync(local, online);
            string originalRequest = transport.Request;
            transport.Status = "Submitted";
            Assert.That(await VerificationPendingRetry.RetryOnceAsync(local, online), Is.EqualTo("PENDING_SUBMISSION_CONFIRMED"));
            Assert.That(transport.Request, Is.EqualTo(originalRequest));
            Assert.That(transport.Calls, Is.EqualTo(2));
        }

        [Test]
        public async Task VerificationRetry_NoConsentDoesNotSendOrRemovePending()
        {
            // Build an unbound fixture instead of attempting to erase an
            // existing authenticated binding through the ordinary save API.
            LocalRecordRepository local = PendingLocal(false);
            OnlineTestTransport transport = new OnlineTestTransport();
            CloudCodeRecordRepository online = new CloudCodeRecordRepository(local.AccountId,
                () => local.OnlineAccount, new OnlineTestAuthentication(), transport);
            Assert.That(await VerificationPendingRetry.RetryOnceAsync(local, online), Is.EqualTo("PENDING_LOCAL_NOT_READY"));
            Assert.That(transport.Calls, Is.Zero);
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
        }
        private static VerificationRecordBaseline Baseline(OnlineLeaderboardResult me)
        { return new VerificationRecordBaseline { projectId = OnlineRecordConfiguration.ProjectId, environmentId = OnlineRecordConfiguration.EnvironmentId,
            boardId = "fs-stage-stage-001-r1", publicPlayerNumber = "0000000001", me = me }; }
        [Test]
        public void PreservedNumberScoreAndAcceptedAt_PassesDespiteRankChanging()
        {
            OnlineLeaderboardResult current = Me(); current.entries[0].rank = 2;
            Assert.That(VerificationRecordComparison.ArePreserved(Baseline(Me()), current, "0000000001", "fs-stage-stage-001-r1"), Is.True);
            Assert.That(VerificationRecordComparison.GetPreservationFailureReason(Baseline(Me()), current,
                "0000000001", "fs-stage-stage-001-r1"), Is.Empty);
        }

        [TestCase("score", "SCORE_MISMATCH")]
        [TestCase("accepted", "ACCEPTED_AT_MISMATCH")]
        [TestCase("number", "ROW_NUMBER_MISMATCH")]
        [TestCase("empty", "ROW_COUNT_MISMATCH")]
        public void PreservationMismatch_ReportsOnlySafeDifferenceCategory(string mutation, string expected)
        {
            OnlineLeaderboardResult current = Me();
            if (mutation == "score") current.entries[0].score++;
            if (mutation == "accepted") current.entries[0].acceptedAt++;
            if (mutation == "number") current.entries[0].publicPlayerNumber = "0000000002";
            if (mutation == "empty") current.entries = new OnlineLeaderboardEntry[0];
            Assert.That(VerificationRecordComparison.GetPreservationFailureReason(Baseline(Me()), current,
                "0000000001", "fs-stage-stage-001-r1"), Is.EqualTo(expected));
        }
        [TestCase("number")]
        [TestCase("score")]
        [TestCase("accepted")]
        [TestCase("isMe")]
        [TestCase("project")]
        [TestCase("environment")]
        [TestCase("board")]
        [TestCase("null")]
        [TestCase("error")]
        public void ChangedOrInvalidBaseline_FailsInsteadOfReportingPreservation(string mutation)
        {
            VerificationRecordBaseline before = Baseline(Me()); OnlineLeaderboardResult current = Me();
            switch (mutation)
            {
                case "number": current.entries[0].publicPlayerNumber = "0000000002"; break;
                case "score": current.entries[0].score++; break;
                case "accepted": current.entries[0].acceptedAt++; break;
                case "isMe": current.entries[0].isMe = false; break;
                case "project": before.projectId = "other"; break;
                case "environment": before.environmentId = "production"; break;
                case "board": before.boardId = "fs-infinite-v2"; break;
                case "null": current.entries = null; break;
                case "error": current.status = "TransientFailure"; break;
            }
            Assert.That(VerificationRecordComparison.ArePreserved(before, current, "0000000001", "fs-stage-stage-001-r1"), Is.False);
        }
        [Test]
        public void EmptySuccess_StillRequiresSameConfirmedNumber()
        {
            OnlineLeaderboardResult empty = new OnlineLeaderboardResult { status = "Success" };
            VerificationRecordBaseline before = Baseline(empty);
            Assert.That(VerificationRecordComparison.ArePreserved(before, empty, "0000000001", "fs-stage-stage-001-r1"), Is.True);
            Assert.That(VerificationRecordComparison.ArePreserved(before, empty, "0000000002", "fs-stage-stage-001-r1"), Is.False);
        }

        [TestCase("empty", "")]
        [TestCase("existing", "STAGE_PROBE_REQUIRES_EMPTY_ME")]
        [TestCase("invalid", "STAGE_PROBE_INVALID_ME")]
        [TestCase("error", "STAGE_PROBE_QUERY_FAILED")]
        [TestCase("null", "STAGE_PROBE_QUERY_FAILED")]
        public void StageProbe_DistinguishesExistingRowFromFailedOrInvalidQuery(string kind, string expected)
        {
            OnlineLeaderboardResult result = Me();
            if (kind == "empty") result.entries = new OnlineLeaderboardEntry[0];
            if (kind == "invalid") result.entries[0].isMe = false;
            if (kind == "error") result.status = "TransientFailure";
            if (kind == "null") result = null;
            Assert.That(VerificationRecordComparison.GetStageProbeBlockReason(result, true), Is.EqualTo(expected));
            if (kind == "existing")
                Assert.That(VerificationRecordComparison.GetStageProbeBlockReason(result, false), Is.EqualTo(string.Empty));
        }

        [Test]
        public void SubmittedMe_RequiresExpectedScoreNumberAndServerAcceptance()
        {
            Assert.That(VerificationRecordComparison.IsSubmittedMe(Me(), "0000000001", 60000), Is.True);
        }

        [TestCase("number")]
        [TestCase("score")]
        [TestCase("accepted")]
        [TestCase("empty")]
        [TestCase("multiple")]
        [TestCase("isMe")]
        [TestCase("error")]
        [TestCase("null")]
        public void SubmittedMe_InvalidRemoteRowCannotPass(string mutation)
        {
            OnlineLeaderboardResult current = Me();
            switch (mutation)
            {
                case "number": current.entries[0].publicPlayerNumber = "0000000002"; break;
                case "score": current.entries[0].score++; break;
                case "accepted": current.entries[0].acceptedAt = 0; break;
                case "empty": current.entries = new OnlineLeaderboardEntry[0]; break;
                case "multiple": current.entries = new[] { current.entries[0], current.entries[0] }; break;
                case "isMe": current.entries[0].isMe = false; break;
                case "error": current.status = "TransientFailure"; break;
                case "null": current = null; break;
            }
            Assert.That(VerificationRecordComparison.IsSubmittedMe(current, "0000000001", 60000), Is.False);
        }
    }
}
