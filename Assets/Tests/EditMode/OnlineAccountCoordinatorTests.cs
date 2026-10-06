using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class OnlineAccountCoordinatorTests
    {
        private const string Number = "0000000007";
        private const string TransferId = "00000000-0000-4000-8000-000000000001";

        private sealed class Transport : IOnlineAccountTransport
        {
            public readonly List<string> Endpoints = new List<string>();
            public readonly List<Dictionary<string, string>> Inputs = new List<Dictionary<string, string>>();
            public Func<string, Task<OnlineAccountResponse>> Respond;
            public bool HasCompleted;

            public async Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters)
            {
                Endpoints.Add(endpoint); Dictionary<string, string> input = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> value in parameters) input.Add(value.Key, value.Value);
                Inputs.Add(input);
                if (endpoint == "get-account-personal-bests") return (T)(object)new OnlinePersonalBestSnapshot
                    { status = "Success", publicPlayerNumber = HasCompleted ? "9999999999" : Number, personalBests = new OnlinePersonalBestEntry[0] };
                OnlineAccountResponse response = Respond == null ? Default(endpoint) : await Respond(endpoint);
                if (endpoint == "complete-account-transfer") HasCompleted = true;
                if (Respond == null && HasCompleted && endpoint == "get-public-player-number")
                    response.publicPlayerNumber = "9999999999";
                return (T)(object)response;
            }

            public static OnlineAccountResponse Default(string endpoint)
            {
                if (endpoint == "get-public-player-number") return new OnlineAccountResponse { status = "Success", publicPlayerNumber = Number };
                if (endpoint == "get-account-transfer-status") return new OnlineAccountResponse { status = "Active", transferStatus = "None" };
                if (endpoint == "cancel-account-transfer") return new OnlineAccountResponse { status = "Active", transferStatus = "Cancelled", transferId = TransferId };
                if (endpoint == "complete-account-transfer") return new OnlineAccountResponse { status = "Success", publicPlayerNumber = "9999999999" };
                return new OnlineAccountResponse { status = "Success", code = "ABCD-EFGH", verificationValue = "000000007", expiresAtMilliseconds = 100000 };
            }
        }

        private static LocalRecordRepository Local(OnlineTestFileStore store, bool consent = true, string player = "ugs-player", string cache = "")
        {
            store.Contents = LocalSaveJsonCodec.Serialize(new LocalSaveData(6, "owner", new LocalSettingsData(75, true, null),
                true, null, new OnlineAccountState(consent, player, cache)));
            LocalRecordRepository local = new LocalRecordRepository(store);
            local.TryLoad(out LocalSaveData ignored); return local;
        }

        private static OnlineAccountResponse Pending()
        {
            return new OnlineAccountResponse { status = "TransferPending", transferStatus = "TransferPending", transferId = TransferId,
                expiresAtMilliseconds = 100000, credentialReissueRequired = true };
        }

        private static RecordSubmissionCandidate Candidate()
        {
            RecordSubmissionPolicy.TryCreateStageCandidate("owner", TransferId, "stage-001", 1,
                E_StageResultType.Cleared, 1, out RecordSubmissionCandidate result);
            return result;
        }

        [TestCase("0000000001", true)]
        [TestCase("9999999999", true)]
        [TestCase("0000000000", false)]
        [TestCase("1", false)]
        [TestCase("１２３４５６７８９０", false)]
        [TestCase("000000001x", false)]
        public void PublicNumber_UsesExactAsciiString(string value, bool valid)
        {
            Assert.That(PublicPlayerNumber.IsValid(value), Is.EqualTo(valid));
            if (valid) Assert.That(PublicPlayerNumber.Format(value, true), Is.EqualTo(value + " (You)"));
            else Assert.That(PublicPlayerNumber.Format(value, false), Is.Empty);
        }

        [Test]
        public async Task AuthenticationImmediatelyIssuesNumberAndPersistsExactScopedCache()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, player: "");
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); Transport transport = new Transport();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, auth, transport, () => true);
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(transport.Endpoints, Is.EqualTo(new[] { "get-account-transfer-status", "get-public-player-number" }));
            Assert.That(transport.Inputs[0], Is.Empty);
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
            Assert.That(local.OnlineAccount.PlayerId, Is.EqualTo("ugs-player"));
            Assert.That(LocalSaveJsonCodec.TryDeserialize(store.Contents, out LocalSaveData restored), Is.True);
            Assert.That(restored.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
            Assert.That(restored.OnlineScope.Matches(OnlineDataScope.CreateVerification()), Is.True);
        }

        [Test]
        public async Task NoConsentAndOffline_MakeZeroAuthenticationOrTransportCalls()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, false);
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); Transport transport = new Transport();
            OnlineAccountCoordinator noConsent = new OnlineAccountCoordinator(local, auth, transport, () => true);
            Assert.That((await noConsent.TryAuthenticateAsync()).IsAuthenticated, Is.False);
            local.TrySaveOnlineAccount(new OnlineAccountState(true, "ugs-player"));
            OnlineAccountCoordinator offline = new OnlineAccountCoordinator(local, auth, transport, () => false);
            Assert.That((await offline.TryAuthenticateAsync()).IsAuthenticated, Is.False);
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Endpoints, Is.Empty);
            Assert.That(offline.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.Offline));
            Assert.That(offline.ViewState.IsOfflinePlayAllowed, Is.True); Assert.That(offline.ViewState.CanRetry, Is.True);
        }

        [Test]
        public async Task AccountMismatch_NeverRequestsOrDisplaysAnotherAccountsNumber()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); Transport transport = new Transport();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local,
                new OnlineTestAuthentication { Player = "other-player" }, transport, () => true);
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.False);
            Assert.That(transport.Endpoints, Is.Empty); Assert.That(service.ViewState.NumberText, Is.Empty);
            Assert.That(service.ViewState.Reason, Is.EqualTo("AccountMismatch"));
        }

        [Test]
        public async Task InvalidNumberOrUnknownReason_HidesCacheAndSupportsExplicitRetry()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: "9999999999");
            Transport transport = new Transport { Respond = endpoint => Task.FromResult(endpoint == "get-account-transfer-status"
                ? Transport.Default(endpoint) : new OnlineAccountResponse
                { status = "Success", publicPlayerNumber = "7", reason = "private-player-token-secret" }) };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.False);
            Assert.That(service.ViewState.NumberText, Is.Empty); Assert.That(service.ViewState.Reason, Is.EqualTo("PublicNumberUnavailable"));
            Assert.That(service.ViewState.CanRetry, Is.True); Assert.That(service.ViewState.IsOfflinePlayAllowed, Is.True);
            transport.Respond = null;
            Assert.That((await service.RetryPublicNumberAsync()).IsAuthenticated, Is.True);
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
            Assert.That(transport.Endpoints, Does.Contain("get-account-personal-bests"));
            Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
        }

        [Test]
        public async Task Restart_DoesNotDisplaySavedNumberUntilServerConfirms()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number);
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), new Transport(), () => true);
            Assert.That(service.ViewState.NumberText, Is.Empty);
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
        }

        [Test]
        public async Task LateResponseAfterInvalidate_IsIgnoredWithoutSavingOldNumber()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); TaskCompletionSource<OnlineAccountResponse> completion = new TaskCompletionSource<OnlineAccountResponse>();
            Transport transport = new Transport { Respond = _ => completion.Task };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Task<OnlineAuthenticationResult> first = service.TryAuthenticateAsync(); service.Invalidate();
            completion.SetResult(new OnlineAccountResponse { status = "Success", publicPlayerNumber = Number });
            Assert.That((await first).IsAuthenticated, Is.False); Assert.That(service.ViewState.NumberText, Is.Empty);
            Assert.That(local.OnlineAccount.PublicNumberCache, Is.Empty);
        }

        [Test]
        public async Task BindingReplacedDuringResponse_EvenWithSamePlayer_DiscardsOldCache()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); TaskCompletionSource<OnlineAccountResponse> completion = new TaskCompletionSource<OnlineAccountResponse>();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(),
                new Transport { Respond = _ => completion.Task }, () => true);
            Task<OnlineAuthenticationResult> first = service.TryAuthenticateAsync();
            local.TrySaveOnlineAccount(new OnlineAccountState(true, "ugs-player", "9999999999"));
            completion.SetResult(new OnlineAccountResponse { status = "Success", publicPlayerNumber = Number });
            Assert.That((await first).IsAuthenticated, Is.False);
            Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo("9999999999")); Assert.That(service.ViewState.NumberText, Is.Empty);
        }

        [Test]
        public async Task PendingBlocksStartAndCompleteBeforeAuthentication()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); local.TryEnqueuePending(Candidate());
            OnlineTestAuthentication auth = new OnlineTestAuthentication(); Transport transport = new Transport();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, auth, transport, () => true);
            Assert.That((await service.StartTransferAsync()).reason, Is.EqualTo("PendingMustBeCleared"));
            Assert.That((await service.CompleteTransferAsync("ABCD-EFGH", "000000007")).status, Is.EqualTo("TransientFailure"));
            Assert.That(auth.Calls, Is.Zero); Assert.That(transport.Endpoints, Is.Empty);
        }

        [Test]
        public async Task StartAndReissue_ReturnCredentialsOnlyToCaller_NotSaveOrViewState()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store);
            Transport transport = new Transport(); OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            OnlineAccountResponse first = await service.StartTransferAsync();
            Assert.That(first.code, Is.EqualTo("ABCD-EFGH")); Assert.That(first.verificationValue, Is.EqualTo("000000007"));
            Assert.That(service.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.TransferPending));
            Assert.That(service.ViewState.NumberText, Is.Empty);
            Assert.That((await service.ReissueTransferAsync()).status, Is.EqualTo("Success"));
            Assert.That(store.Contents, Does.Not.Contain("ABCD-EFGH")); Assert.That(store.Contents, Does.Not.Contain("\"000000007\""));
            Assert.That(store.Contents, Does.Not.Contain("verificationValue"));
            Assert.That(store.Contents, Does.Not.Contain("TransferPending"));
        }

        [Test]
        public async Task StatusUsesNoParameters_CancelUsesOnlyCurrentTransferId()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number); Transport transport = new Transport
                { Respond = endpoint => Task.FromResult(endpoint == "get-account-transfer-status" ? Pending() : Transport.Default(endpoint)) };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Assert.That((await service.CancelTransferAsync()).reason, Is.EqualTo("StatusRequired"));
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("TransferPending"));
            Assert.That(transport.Inputs[0], Is.Empty);
            Assert.That((await service.CancelTransferAsync()).transferStatus, Is.EqualTo("Cancelled"));
            Assert.That(transport.Inputs[1].Count, Is.EqualTo(1)); Assert.That(transport.Inputs[1]["transferId"], Is.EqualTo(TransferId));
        }

        [Test]
        public async Task CompleteNormalizesCodeKeepsLeadingZerosAndAppliesConfirmedHandoff()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number); Transport transport = new Transport();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Assert.That((await service.CompleteTransferAsync(" oiLA-2345 ", "000000007")).status, Is.EqualTo("Success"));
            Assert.That(transport.Inputs[0].Count, Is.EqualTo(2)); Assert.That(transport.Inputs[0]["code"], Is.EqualTo("011A2345"));
            Assert.That(transport.Inputs[0]["verificationValue"], Is.EqualTo("000000007"));
            Assert.That(local.OnlineAccount.PublicNumberCache, Is.EqualTo("9999999999"));
            Assert.That(service.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.Ready));
            Assert.That((await service.RetryPublicNumberAsync()).IsAuthenticated, Is.True);
        }

        [Test]
        public async Task InvalidInputsAndInvalidStatus_DoNotExposeRawReasonOrIdentifiers()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number); Transport transport = new Transport
                { Respond = _ => Task.FromResult(new OnlineAccountResponse { status = "invented", reason = "private-secret", publicPlayerNumber = Number }) };
            OnlineTestAuthentication auth = new OnlineTestAuthentication();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, auth, transport, () => true);
            Assert.That((await service.CompleteTransferAsync("bad", "000000007")).reason, Is.EqualTo("InvalidInput"));
            Assert.That(auth.Calls, Is.Zero);
            OnlineAccountResponse response = await service.RefreshStatusAsync();
            Assert.That(response.reason, Is.EqualTo("TransferUnavailable")); Assert.That(response.publicPlayerNumber, Is.Null);
            Assert.That(service.ViewState.NumberText, Is.Empty);
        }

        [Test]
        public async Task TimeoutPreservesBindingAndCache_AndStatusRetryCanRecoverPending()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, cache: Number); string before = store.Contents;
            Transport transport = new Transport { Respond = _ => Task.FromException<OnlineAccountResponse>(new TimeoutException("private-secret")) };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Assert.That((await service.StartTransferAsync()).reason, Is.EqualTo("Timeout"));
            Assert.That(store.Contents, Is.EqualTo(before)); Assert.That(service.ViewState.NumberText, Is.Empty);
            transport.Respond = _ => Task.FromResult(Pending());
            Assert.That((await service.RefreshStatusAsync()).status, Is.EqualTo("TransferPending"));
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.False);
        }

        [Test]
        public async Task InactiveCompletionWithoutSessionGateway_PreservesDataAndBlocksOnline()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, cache: Number); string before = store.Contents;
            Transport transport = new Transport { Respond = _ => Task.FromResult(new OnlineAccountResponse
                { status = "Inactive", transferStatus = "Completed", transferId = TransferId }) };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Assert.That((await service.RefreshStatusAsync()).reason, Is.EqualTo("SessionResetUnavailable"));
            Assert.That(service.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.Error));
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.False); Assert.That(store.Contents, Is.EqualTo(before));
        }

        [Test]
        public async Task ScopedCacheRemainsInOriginalAreaAndFailedSaveNeverMarksReady()
        {
            OnlineTestFileStore store = new OnlineTestFileStore(); LocalRecordRepository local = Local(store, cache: Number);
            LocalRecordRepository other = new LocalRecordRepository(store, new OnlineDataScope("other", "other")); other.TryLoad(out LocalSaveData ignored);
            Assert.That(other.OnlineAccount.PublicNumberCache, Is.Empty);
            LocalRecordRepository restored = new LocalRecordRepository(store); restored.TryLoad(out ignored);
            Assert.That(restored.OnlineAccount.PublicNumberCache, Is.EqualTo(Number));
            store.FailWrite = true;
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(restored, new OnlineTestAuthentication(), new Transport(), () => true);
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.False);
            Assert.That(service.ViewState.NumberText, Is.Empty); Assert.That(service.ViewState.Reason, Is.EqualTo("LocalSaveUnavailable"));
        }

        [Test]
        public async Task ConcurrentAuthenticationAndTransferCallsDoNotDuplicateRequests()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore()); TaskCompletionSource<OnlineAccountResponse> completion = new TaskCompletionSource<OnlineAccountResponse>();
            Transport transport = new Transport { Respond = _ => completion.Task };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), transport, () => true);
            Task<OnlineAuthenticationResult> first = service.TryAuthenticateAsync();
            Task<OnlineAuthenticationResult> second = service.TryAuthenticateAsync();
            Assert.That(second.IsCompleted, Is.False);
            Assert.That((await service.StartTransferAsync()).reason, Is.EqualTo("RequestInProgress"));
            transport.Respond = null;
            completion.SetResult(new OnlineAccountResponse { status = "Active", transferStatus = "None" });
            Assert.That((await first).IsAuthenticated, Is.True);
            Assert.That((await second).IsAuthenticated, Is.True);
            Assert.That(transport.Endpoints.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task StartupAuthentication_UsesOneBudgetAndIgnoresLateStatusThenExplicitRetrySucceeds()
        {
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>();
            TaskCompletionSource<OnlineAccountResponse> status = new TaskCompletionSource<OnlineAccountResponse>();
            int budgets = 0;
            Transport transport = new Transport { Respond = _ => status.Task };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(Local(new OnlineTestFileStore()),
                new OnlineTestAuthentication(), transport, () => true,
                panelDelay: milliseconds => { Assert.That(milliseconds, Is.EqualTo(5000)); budgets++;
                    return budgets == 1 ? deadline.Task : new TaskCompletionSource<bool>().Task; });
            Task<OnlineAuthenticationResult> startup = service.TryAuthenticateAsync();
            Assert.That(budgets, Is.EqualTo(1));
            deadline.SetResult(true);
            Assert.That((await startup).IsAuthenticated, Is.False);
            Assert.That(service.ViewState.Reason, Is.EqualTo("Timeout"));
            status.SetResult(Transport.Default("get-account-transfer-status")); await Task.Yield();
            Assert.That(service.ViewState.NumberText, Is.Empty);
            Assert.That(transport.Endpoints.Count, Is.EqualTo(1));
            transport.Respond = null;
            Assert.That((await service.TryAuthenticateAsync()).IsAuthenticated, Is.True);
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
        }

        [Test]
        public async Task RefreshDuringStartup_JoinsVerifiedResultWithoutDuplicateStatusOrNumber()
        {
            TaskCompletionSource<OnlineAccountResponse> status = new TaskCompletionSource<OnlineAccountResponse>();
            Transport transport = new Transport { Respond = _ => status.Task };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(Local(new OnlineTestFileStore()),
                new OnlineTestAuthentication(), transport, () => true,
                panelDelay: _ => new TaskCompletionSource<bool>().Task);
            Task<OnlineAuthenticationResult> startup = service.TryAuthenticateAsync();
            Task<OnlineAccountResponse> refresh = service.RefreshPanelAsync();
            Assert.That(refresh.IsCompleted, Is.False);
            Assert.That(transport.Endpoints.Count, Is.EqualTo(1));
            transport.Respond = null; status.SetResult(Transport.Default("get-account-transfer-status"));
            Assert.That((await startup).IsAuthenticated, Is.True);
            Assert.That((await refresh).status, Is.EqualTo("Active"));
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
            Assert.That(transport.Endpoints, Is.EqualTo(new[] { "get-account-transfer-status", "get-public-player-number" }));
        }

        [Test]
        public async Task RefreshAfterActiveStatus_FetchesNumberWithoutRepeatingStatus()
        {
            Transport transport = new Transport();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(Local(new OnlineTestFileStore()),
                new OnlineTestAuthentication(), transport, () => true);
            Assert.That((await service.RefreshPanelAsync()).status, Is.EqualTo("Active"));
            Assert.That(service.ViewState.NumberText, Is.EqualTo(Number));
            Assert.That(transport.Endpoints, Is.EqualTo(new[] { "get-account-transfer-status", "get-public-player-number" }));
        }

        [Test]
        public async Task RefreshWaitTimeout_DoesNotCancelOrRestartIndependentStartup()
        {
            TaskCompletionSource<bool> panelDeadline = new TaskCompletionSource<bool>();
            TaskCompletionSource<OnlineAccountResponse> status = new TaskCompletionSource<OnlineAccountResponse>();
            int budgets = 0;
            Transport transport = new Transport { Respond = _ => status.Task };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(Local(new OnlineTestFileStore()),
                new OnlineTestAuthentication(), transport, () => true,
                panelDelay: _ => ++budgets == 2 ? panelDeadline.Task : new TaskCompletionSource<bool>().Task);
            Task<OnlineAuthenticationResult> startup = service.TryAuthenticateAsync();
            Task<OnlineAccountResponse> refresh = service.RefreshPanelAsync();
            panelDeadline.SetResult(true);
            Assert.That((await refresh).reason, Is.EqualTo("Timeout"));
            Assert.That(startup.IsCompleted, Is.False);
            Assert.That(transport.Endpoints.Count, Is.EqualTo(1));
            transport.Respond = null; status.SetResult(Transport.Default("get-account-transfer-status"));
            Assert.That((await startup).IsAuthenticated, Is.True);
            Assert.That(transport.Endpoints.Count, Is.EqualTo(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ConcurrentTopAndAround_ShareAuthenticationAndAllowExplicitNextQuery(bool succeeds)
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number);
            TaskCompletionSource<OnlineAccountResponse> completion = new TaskCompletionSource<OnlineAccountResponse>();
            Transport accountTransport = new Transport { Respond = _ => completion.Task };
            OnlineTestAuthentication authentication = new OnlineTestAuthentication();
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, authentication, accountTransport, () => true);
            OnlineTestTransport records = new OnlineTestTransport();
            CloudCodeRecordRepository repository = new CloudCodeRecordRepository("owner", () => local.OnlineAccount,
                service, records, () => local.CanUseOnlineData);
            Assert.That(RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key), Is.True);
            Task<OnlineLeaderboardResult> top = repository.GetTopAsync(key);
            Task<OnlineLeaderboardResult> around = repository.GetAroundAsync(key);
            Assert.That(top.IsCompleted, Is.False);
            Assert.That(around.IsCompleted, Is.False);
            Assert.That(authentication.Calls, Is.EqualTo(1));
            Assert.That(accountTransport.Endpoints.Count, Is.EqualTo(1));
            Assert.That(records.Calls, Is.Zero);
            accountTransport.Respond = null;
            completion.SetResult(succeeds ? Transport.Default("get-account-transfer-status") :
                new OnlineAccountResponse { status = "TransientFailure" });
            OnlineLeaderboardResult topResult = await top;
            OnlineLeaderboardResult aroundResult = await around;
            Assert.That(topResult.IsSuccess, Is.EqualTo(succeeds));
            Assert.That(aroundResult.IsSuccess, Is.EqualTo(succeeds));
            Assert.That(records.Calls, Is.EqualTo(succeeds ? 2 : 0));
            Assert.That(authentication.Calls, Is.EqualTo(1));
            if (!succeeds)
            {
                Assert.That(topResult.reason, Is.EqualTo("AuthenticationUnavailable"));
                Assert.That(aroundResult.reason, Is.EqualTo("AuthenticationUnavailable"));
            }
            // A finished success or failure is not cached as authority for a new query.
            Assert.That((await repository.GetAroundAsync(key)).IsSuccess, Is.True);
            Assert.That(authentication.Calls, Is.EqualTo(2));
            Assert.That(records.Calls, Is.EqualTo(succeeds ? 3 : 1));
        }

        [Test]
        public async Task ConcurrentTopAndAround_InvalidateRejectsBothBeforeRecordQueries()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number);
            TaskCompletionSource<OnlineAccountResponse> completion = new TaskCompletionSource<OnlineAccountResponse>();
            Transport accountTransport = new Transport { Respond = _ => completion.Task };
            OnlineAccountCoordinator service = new OnlineAccountCoordinator(local, new OnlineTestAuthentication(), accountTransport, () => true);
            OnlineTestTransport records = new OnlineTestTransport();
            CloudCodeRecordRepository repository = new CloudCodeRecordRepository("owner", () => local.OnlineAccount,
                service, records, () => local.CanUseOnlineData);
            Assert.That(RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key), Is.True);
            Task<OnlineLeaderboardResult> top = repository.GetTopAsync(key);
            Task<OnlineLeaderboardResult> around = repository.GetAroundAsync(key);
            service.Invalidate();
            completion.SetResult(Transport.Default("get-account-transfer-status"));
            Assert.That((await top).IsSuccess, Is.False);
            Assert.That((await around).IsSuccess, Is.False);
            Assert.That(records.Calls, Is.Zero);
            Assert.That(accountTransport.Endpoints.Count, Is.EqualTo(1));
            Assert.That(service.ViewState.NumberText, Is.Empty);
        }

        [Test]
        public async Task LeaderboardUsesPublicNumberAndServerIsMe_RejectsMissingNumber()
        {
            LocalRecordRepository local = Local(new OnlineTestFileStore(), cache: Number);
            OnlineTestTransport transport = new OnlineTestTransport { QueryResponse = new OnlineLeaderboardResult
                { status = "Success", entries = new[] { new OnlineLeaderboardEntry { publicPlayerNumber = Number, isMe = true, rank = 1 } } } };
            CloudCodeRecordRepository online = new CloudCodeRecordRepository("owner", () => local.OnlineAccount,
                new OnlineTestAuthentication(), transport, () => local.CanUseOnlineData);
            Assert.That((await online.GetTopAsync(Candidate().BoardKey)).entries[0].isMe, Is.True);
            transport.QueryResponse.entries[0].publicPlayerNumber = "invalid";
            Assert.That((await online.GetTopAsync(Candidate().BoardKey)).reason, Is.EqualTo("PublicNumberUnavailable"));
        }
    }
}
