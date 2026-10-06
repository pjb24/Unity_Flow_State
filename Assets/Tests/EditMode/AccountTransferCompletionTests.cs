using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class AccountTransferCompletionTests
    {
        private const string OldNumber = "0000000001";
        private const string Number = "0000000002";
        private const string Id = "11111111-1111-4111-8111-111111111111";

        private sealed class Session : IOnlineAuthenticationSession
        {
            public string Player = "B";
            public bool CanReset = true;
            public bool CanAuthenticate = true;
            public int Resets;
            public Func<bool> BeforeReset;
            public Task<OnlineAuthenticationResult> TryAuthenticateAsync()
            { return Task.FromResult(new OnlineAuthenticationResult(CanAuthenticate, Player)); }
            public Task<bool> TryClearSessionAsync(string expectedPlayerId)
            {
                Resets++;
                if (BeforeReset != null) Assert.That(BeforeReset(), Is.True);
                if (!CanReset || expectedPlayerId != Player) return Task.FromResult(false);
                Player = "new-A"; return Task.FromResult(true);
            }
        }

        private sealed class Transport : IOnlineAccountTransport
        {
            public readonly List<string> Calls = new List<string>();
            public OnlineAccountResponse Status = Completed();
            public OnlinePersonalBestSnapshot Snapshot = BestSnapshot();
            public Func<Task<OnlinePersonalBestSnapshot>> ReadSnapshot;
            public string CurrentNumber = Number;
            public bool TimeoutCompletion;
            public async Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters)
            {
                Calls.Add(endpoint);
                if (endpoint != "complete-account-transfer") Assert.That(parameters.Count, Is.Zero);
                if (endpoint == "get-account-personal-bests")
                    return (T)(object)(ReadSnapshot == null ? Snapshot : await ReadSnapshot());
                if (endpoint == "get-public-player-number")
                    return (T)(object)new OnlineAccountResponse { status = "Success", publicPlayerNumber = CurrentNumber };
                if (endpoint == "complete-account-transfer")
                {
                    if (TimeoutCompletion) throw new TimeoutException("private-secret");
                    return (T)(object)new OnlineAccountResponse { status = "Success", publicPlayerNumber = Number };
                }
                return (T)(object)Status;
            }
        }

        private sealed class DelayedRecords : IOnlineRecordTransport
        {
            public readonly TaskCompletionSource<OnlineSubmissionResponse> Submission = new TaskCompletionSource<OnlineSubmissionResponse>();
            public readonly TaskCompletionSource<OnlineLeaderboardResult> Query = new TaskCompletionSource<OnlineLeaderboardResult>();
            public async Task<T> CallAsync<T>(string endpoint, string request)
            {
                if (endpoint == "submit-record") return (T)(object)await Submission.Task;
                return (T)(object)await Query.Task;
            }
        }

        private static OnlineAccountResponse Completed()
        { return new OnlineAccountResponse { status = "AlreadyCompleted", publicPlayerNumber = Number }; }

        private static OnlineAccountResponse Inactive()
        { return new OnlineAccountResponse { status = "Inactive", transferStatus = "Completed", transferId = Id }; }

        private static OnlinePersonalBestSnapshot BestSnapshot()
        {
            return new OnlinePersonalBestSnapshot { status = "Success", publicPlayerNumber = Number,
                personalBests = new[] { new OnlinePersonalBestEntry
                    { boardId = "fs-stage-stage-001-r1", score = 5000, submissionId = Id } } };
        }

        private static RecordSubmissionCandidate Candidate(string owner = "owner", double seconds = 1)
        {
            RecordSubmissionPolicy.TryCreateStageCandidate(owner, Id, "stage-001", 1,
                E_StageResultType.Cleared, seconds, out RecordSubmissionCandidate candidate);
            return candidate;
        }

        private static LocalRecordRepository Local(OnlineTestFileStore store, string player = "B", string number = OldNumber)
        {
            OnlineLocalSaveData other = new OnlineLocalSaveData(new OnlineDataScope("other-project", "other-environment"),
                new OnlineAccountState(true, "other-player", "0000000009"), new[] { Candidate("other") }, new[] { Candidate("other") });
            store.Contents = LocalSaveJsonCodec.Serialize(new LocalSaveData(6, "owner", new LocalSettingsData(63, true, null),
                true, new[] { Candidate() }, new OnlineAccountState(true, player, number), null,
                OnlineDataScope.CreateConfigured(), new[] { other }));
            LocalRecordRepository local = new LocalRecordRepository(store); local.TryLoad(out LocalSaveData ignored); return local;
        }

        private static RecordBoardKey Stage()
        { RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey board); return board; }

        private static void AssertPreservedDeviceData(OnlineTestFileStore store)
        {
            Assert.That(LocalSaveJsonCodec.TryDeserialize(store.Contents, out LocalSaveData saved), Is.True);
            Assert.That(saved.Settings.MasterVolume, Is.EqualTo(63)); Assert.That(saved.Settings.IsFullscreen, Is.True);
            Assert.That(saved.HasCompletedTutorial, Is.True); Assert.That(saved.AccountId, Is.EqualTo("owner"));
            Assert.That(saved.InactiveOnlineAreas.Count, Is.EqualTo(1));
            Assert.That(saved.InactiveOnlineAreas[0].PendingSubmissions.Count, Is.EqualTo(1));
            Assert.That(saved.InactiveOnlineAreas[0].Account.PlayerId, Is.EqualTo("other-player"));
        }

        [Test]
        public async Task BCompletion_ReplacesEvenBetterLocalBestAndPersistsNumberTogether()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store);
            Transport transport = new Transport(); int changes = 0;
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), transport, () => true, () => changes++);
            Assert.That((await service.CompleteTransferAsync("ABCD-EFGH", "000000007")).status, Is.EqualTo("Success"));
            Assert.That(local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate best), Is.True);
            Assert.That(best.RankingValue, Is.EqualTo(5000)); Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
            Assert.That(local.CreatePendingSnapshot(), Is.Empty); Assert.That(changes, Is.EqualTo(1));
            Assert.That(transport.Calls, Does.Not.Contain("submit-record")); AssertPreservedDeviceData(store);
            LocalRecordRepository restarted = new LocalRecordRepository(store); restarted.TryLoad(out LocalSaveData ignored);
            Assert.That(restarted.TryGetPersonalBest("owner", Stage(), out best), Is.True); Assert.That(best.RankingValue, Is.EqualTo(5000));
        }

        [Test]
        public async Task BEmptySuccess_ClearsAllPreviousBests()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); Transport transport = new Transport();
            transport.Snapshot.personalBests = new OnlinePersonalBestEntry[0];
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), transport, () => true);
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("AlreadyCompleted"));
            Assert.That(local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate ignored), Is.False);
        }

        [Test]
        public async Task BWholeSnapshot_IncludesInfiniteAndRemovesAbsentStage()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); Transport transport = new Transport();
            transport.Snapshot.personalBests = new[] { new OnlinePersonalBestEntry { boardId = "fs-infinite-v2", score = 42, submissionId = Id } };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), transport, () => true);
            await service.RefreshStatusAsync();
            Assert.That(local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate ignored), Is.False);
            RecordBoardKey.TryCreateInfinite(2, out RecordBoardKey infinite);
            Assert.That(local.TryGetPersonalBest("owner", infinite, out RecordSubmissionCandidate best), Is.True);
            Assert.That(best.RankingValue, Is.EqualTo(42)); Assert.That(local.CreatePendingSnapshot(), Is.Empty);
        }

        [Test]
        public async Task BWriteFailure_PreservesOldMemoryAndFile_StatusRetryApplies()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store); string before = store.Contents;
            Transport transport = new Transport(); OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), transport, () => true);
            store.FailWrite = true;
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("TransientFailure"));
            Assert.That(store.Contents, Is.EqualTo(before)); Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo(OldNumber));
            Assert.That(local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate best), Is.True);
            Assert.That(best.RankingValue, Is.EqualTo(1000)); Assert.That((await service.RetryPublicNumberAsync()).IsAuthenticated, Is.False);
            store.FailWrite = false; Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("AlreadyCompleted"));
            Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
        }

        [Test]
        public async Task LostCompletionResponse_PreservesUntilRestartStatusConfirms()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store); string before = store.Contents;
            Transport transport = new Transport { TimeoutCompletion = true };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), transport, () => true);
            Assert.That((await service.CompleteTransferAsync("ABCD-EFGH", "000000007")).reason, Is.EqualTo("Timeout"));
            Assert.That(store.Contents, Is.EqualTo(before)); Assert.That(service.ViewState.IsOfflinePlayAllowed, Is.True);
            LocalRecordRepository restarted = new LocalRecordRepository(store); restarted.TryLoad(out LocalSaveData ignored);
            OnlineAccountCoordinator resumed = new OnlineAccountCoordinator(restarted, new Session(), transport, () => true);
            Assert.That((await resumed.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(restarted.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
            Assert.That(transport.Calls, Does.Not.Contain("submit-record"));
        }

        [Test]
        public async Task AInactive_ClearsCurrentBestsBeforeTokensAndStartsDifferentAnonymous()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, "A");
            Session session = new Session { Player = "A", BeforeReset = () => local.OnlineAccount.PlayerId == "" &&
                !local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate ignored) };
            Transport transport = new Transport { Status = Inactive() }; int changes = 0;
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, session, transport, () => true, () => changes++);
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("Inactive"));
            Assert.That(session.Resets, Is.EqualTo(1)); Assert.That(local.OnlineAccount.PlayerId, Is.EqualTo("new-A"));
            Assert.That(service.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.Ready)); Assert.That(changes, Is.EqualTo(1));
            AssertPreservedDeviceData(store);
        }

        [Test]
        public async Task StartupInactiveRecovery_FetchesFreshNumberOnceWithinOneBudget()
        {
            OnlineTestFileStore store = new OnlineTestFileStore();
            LocalRecordRepository local = Local(store, "A");
            Session session = new Session { Player = "A" };
            Transport transport = new Transport { Status = Inactive() };
            int budgets = 0;
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, session, transport, () => true,
                panelDelay: milliseconds => { Assert.That(milliseconds, Is.EqualTo(5000)); budgets++;
                    return new TaskCompletionSource<bool>().Task; });
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(budgets, Is.EqualTo(1));
            Assert.That(session.Resets, Is.EqualTo(1));
            Assert.That(local.OnlineAccount.PlayerId, Is.EqualTo("new-A"));
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
            Assert.That(transport.Calls, Is.EqualTo(new[] { "get-account-transfer-status", "get-public-player-number" }));
            AssertPreservedDeviceData(store);
        }

        [Test]
        public async Task AWriteFailure_DoesNotRemoveSessionOrOldBests()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, "A"); string before = store.Contents;
            store.FailWrite = true; Session session = new Session { Player = "A" };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, session, new Transport { Status = Inactive() }, () => true);
            Assert.That((await service.RefreshStatusAsync()).reason, Is.EqualTo("LocalSaveUnavailable"));
            Assert.That(session.Resets, Is.Zero); Assert.That(store.Contents, Is.EqualTo(before)); Assert.That(local.OnlineAccount.PlayerId, Is.EqualTo("A"));
        }

        [Test]
        public async Task ASessionResetFailure_DoesNotBindOldTokenToNewData_ExplicitStatusRetryRecovers()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, "A");
            Session session = new Session { Player = "A", CanReset = false }; Transport transport = new Transport { Status = Inactive() };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, session, transport, () => true);
            Assert.That((await service.RefreshStatusAsync()).reason, Is.EqualTo("SessionResetUnavailable"));
            Assert.That(local.OnlineAccount.PlayerId, Is.Empty); Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.False);
            session.CanReset = true; transport.Status = new OnlineAccountResponse { status = "Active", transferStatus = "None" };
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("Active"));
            Assert.That(local.OnlineAccount.PlayerId, Is.EqualTo("new-A")); AssertPreservedDeviceData(store);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task ACrashAfterBlankSave_RestartRecoversWithOldOrNewSdkSession(bool hasClearedToken)
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, "A");
            Session session = new Session { Player = "A", CanReset = false };
            await new OnlineAccountCoordinator(local, session, new Transport { Status = Inactive() }, () => true).RefreshStatusAsync();
            LocalRecordRepository restarted = new LocalRecordRepository(store); restarted.TryLoad(out LocalSaveData ignored);
            Session sdk = new Session { Player = hasClearedToken ? "new-A" : "A" };
            Transport transport = new Transport { Status = hasClearedToken
                ? new OnlineAccountResponse { status = "Active", transferStatus = "None" } : Inactive() };
            Assert.That((await new OnlineAccountCoordinator(restarted, sdk, transport, () => true).TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(restarted.OnlineAccount.PlayerId, Is.EqualTo("new-A")); Assert.That(sdk.Resets, Is.EqualTo(hasClearedToken ? 0 : 1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task RecoveryWithNewPending_PreservesItAndRequiresExplicitResolution(bool isSource)
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, isSource ? "A" : "B");
            Assert.That(local.TryEnqueuePending(Candidate("foreign-owner")), Is.True); local.TryCheckpoint(); string before = store.Contents;
            Session sdk = new Session { Player = isSource ? "A" : "B" };
            Transport transport = new Transport { Status = isSource ? Inactive() : Completed() };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, sdk, transport, () => true);
            Assert.That((await service.RefreshStatusAsync()).reason, Is.EqualTo("PendingMustBeCleared"));
            Assert.That(store.Contents, Is.EqualTo(before)); Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(sdk.Resets, Is.Zero); Assert.That(local.TryDiscardPending(), Is.True);
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo(isSource ? "Inactive" : "AlreadyCompleted"));
        }

        [Test]
        public async Task SnapshotTimeout_IsNotEmptyAndNeverSwitchesBinding()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store); string before = store.Contents;
            Transport transport = new Transport { ReadSnapshot = () => Task.FromException<OnlinePersonalBestSnapshot>(new TimeoutException()) };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), transport, () => true);
            Assert.That((await service.RefreshStatusAsync()).reason, Is.EqualTo("Timeout")); Assert.That(store.Contents, Is.EqualTo(before));
            Assert.That((await service.RetryPublicNumberAsync()).IsAuthenticated, Is.False);
        }

        [Test]
        public async Task ANewAuthenticationFailure_RestartWithClearedTokenCanBindNewPlayer()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, "A");
            Session session = new Session { Player = "A" };
            // Authentication succeeds before status, then fails only after reset.
            session.BeforeReset = () => { session.CanAuthenticate = false; return true; };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, session, new Transport { Status = Inactive() }, () => true);
            Assert.That((await service.RefreshStatusAsync()).reason, Is.EqualTo("NewAuthenticationUnavailable"));
            Assert.That(local.OnlineAccount.PlayerId, Is.Empty); Assert.That(session.Resets, Is.EqualTo(1));
            LocalRecordRepository restarted = new LocalRecordRepository(store); restarted.TryLoad(out LocalSaveData ignored);
            Transport transport = new Transport { Status = new OnlineAccountResponse { status = "Active", transferStatus = "None" } };
            Assert.That((await new OnlineAccountCoordinator(restarted, new Session { Player = "new-A" }, transport, () => true).TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(restarted.OnlineAccount.PlayerId, Is.EqualTo("new-A")); AssertPreservedDeviceData(store);
        }

        [Test]
        public async Task ChangedCWithLaterCancelledStatus_StillReplacesBestBeforeNumber()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); Transport transport = new Transport
                { Status = new OnlineAccountResponse { status = "Active", transferStatus = "Cancelled", transferId = Id } };
            Assert.That((await new OnlineAccountCoordinator(local, new Session(), transport, () => true).TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate best), Is.True);
            Assert.That(best.RankingValue, Is.EqualTo(5000)); Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
        }

        [Test]
        public async Task BindingReplacedDuringSnapshot_IsIgnoredEvenWithSamePlayer()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            TaskCompletionSource<OnlinePersonalBestSnapshot> delayed = new TaskCompletionSource<OnlinePersonalBestSnapshot>();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), new Transport { ReadSnapshot = () => delayed.Task }, () => true);
            Task<OnlineAccountResponse> task = service.RefreshStatusAsync();
            local.TrySaveOnlineAccount(new OnlineAccountState(true, "B", "0000000008")); delayed.SetResult(BestSnapshot());
            Assert.That((await task).reason, Is.EqualTo("StaleResponse")); Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo("0000000008"));
        }

        [Test]
        public async Task LateSnapshotAfterInvalidate_DoesNotReplaceBestOrCache()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store); string before = store.Contents;
            TaskCompletionSource<OnlinePersonalBestSnapshot> delayed = new TaskCompletionSource<OnlinePersonalBestSnapshot>();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), new Transport { ReadSnapshot = () => delayed.Task }, () => true);
            Task<OnlineAccountResponse> task = service.RefreshStatusAsync(); service.Invalidate(); delayed.SetResult(BestSnapshot());
            Assert.That((await task).status, Is.EqualTo("TransientFailure")); Assert.That(store.Contents, Is.EqualTo(before));
        }

        [Test]
        public async Task TransitionBlocksNewPendingAndBestWritesUntilWholeSnapshotArrives()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            TaskCompletionSource<OnlinePersonalBestSnapshot> delayed = new TaskCompletionSource<OnlinePersonalBestSnapshot>();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), new Transport { ReadSnapshot = () => delayed.Task }, () => true);
            Task<OnlineAccountResponse> task = service.RefreshStatusAsync();
            Assert.That(local.TryEnqueuePending(Candidate()), Is.False); Assert.That(local.TryUpdatePersonalBest(Candidate()), Is.False);
            Assert.That(local.TryDiscardPending(), Is.False); delayed.SetResult(BestSnapshot()); await task;
            Assert.That(local.TryEnqueuePending(Candidate()), Is.True);
        }

        [TestCase("failure")]
        [TestCase("null-array")]
        [TestCase("duplicate")]
        [TestCase("unknown-board")]
        [TestCase("negative")]
        [TestCase("number-mismatch")]
        public async Task InvalidSnapshot_PreservesWholeLocalArea(string invalid)
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store); string before = store.Contents;
            Transport transport = new Transport(); OnlinePersonalBestSnapshot snapshot = transport.Snapshot;
            if (invalid == "failure") snapshot.status = "TransientFailure";
            if (invalid == "null-array") snapshot.personalBests = null;
            if (invalid == "duplicate") snapshot.personalBests = new[] { snapshot.personalBests[0], snapshot.personalBests[0] };
            if (invalid == "unknown-board") snapshot.personalBests[0].boardId = "other";
            if (invalid == "negative") snapshot.personalBests[0].score = -1;
            if (invalid == "number-mismatch") snapshot.publicPlayerNumber = "0000000003";
            Assert.That((await new OnlineAccountCoordinator(local, new Session(), transport, () => true).RefreshStatusAsync()).reason,
                Is.EqualTo("PersonalBestUnavailable")); Assert.That(store.Contents, Is.EqualTo(before));
        }

        [Test]
        public async Task AlreadyAppliedCompletion_AfterRestartRetainsOfflineBestAndPending()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, number: Number);
            local.TryEnqueuePending(Candidate()); local.TryCheckpoint(); string before = store.Contents;
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new Session(), new Transport(), () => true);
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(local.TryGetPersonalBest("owner", Stage(), out RecordSubmissionCandidate best), Is.True);
            Assert.That(best.RankingValue, Is.EqualTo(1000)); Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(store.Contents, Is.EqualTo(before));
        }

        [Test]
        public void AccountPresentationInvalidation_RejectsDelayedLeaderboardSections()
        {
            LeaderboardViewState view = new LeaderboardViewState(); int top = view.BeginTopRequest(), me = view.BeginPersonalBestRequest();
            view.Invalidate(); OnlineLeaderboardResult old = new OnlineLeaderboardResult { status = "Success", entries = new OnlineLeaderboardEntry[0] };
            view.CompleteTopRequest(top, old); view.CompletePersonalBestRequest(me, old);
            Assert.That(view.TopResult, Is.Null); Assert.That(view.PersonalBestResult, Is.Null);
            Assert.That(view.TopState, Is.EqualTo(E_LeaderboardQueryState.NotRequested));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task RecordResponseAfterCChange_IsNotAppliedToNewConnection(bool isSubmission)
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); DelayedRecords transport = new DelayedRecords();
            CloudCodeRecordRepository repository = new CloudCodeRecordRepository("owner", () => local.OnlineAccount, new Session(), transport);
            if (isSubmission)
            {
                Task<OnlineSubmissionResult> task = repository.SubmitAsync(Candidate());
                local.TrySaveOnlineAccount(new OnlineAccountState(true, "B", Number));
                transport.Submission.SetResult(new OnlineSubmissionResponse { status = "Submitted" });
                Assert.That((await task).Result, Is.EqualTo(E_RecordSubmissionResult.TransientFailure));
            }
            else
            {
                Task<OnlineLeaderboardResult> task = repository.GetTopAsync(Stage());
                local.TrySaveOnlineAccount(new OnlineAccountState(true, "B", Number));
                transport.Query.SetResult(new OnlineLeaderboardResult { status = "Success", entries = new OnlineLeaderboardEntry[0] });
                Assert.That((await task).reason, Is.EqualTo("StaleResponse"));
            }
        }

        [Test]
        public async Task ClearSessionHistory_RemovesRuntimeTerminalReceiptsAndCounters()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore());
            Assert.That(local.TryEnqueuePending(Candidate()), Is.True);
            Session session = new Session(); CloudCodeRecordRepository repository = new CloudCodeRecordRepository("owner",
                () => local.OnlineAccount, session, new OnlineTestTransport());
            OnlineRecordCoordinator records = new OnlineRecordCoordinator("owner", local, session, repository);
            await records.RetryPendingAsync(); Assert.That(records.SubmittedCount, Is.EqualTo(1));
            Assert.That(records.TryGetTerminalResult(Id, out OnlineSubmissionResult submitted), Is.True);
            Assert.That(submitted.Result, Is.EqualTo(E_RecordSubmissionResult.Submitted));
            records.ClearSessionHistory(); Assert.That(records.SubmittedCount, Is.Zero); Assert.That(records.RejectedCount, Is.Zero);
            Assert.That(records.TryGetTerminalResult(Id, out OnlineSubmissionResult ignored), Is.False);
        }
    }
}
