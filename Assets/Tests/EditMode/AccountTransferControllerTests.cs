using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class AccountTransferControllerTests
    {
        private const string Id = "11111111-1111-4111-8111-111111111111";
        private sealed class View : IAccountTransferView
        {
            public string LastCode = "";
            public string LastMessage = "";
            public string CodeInput = "";
            public string ValueInput = "";
            public int Clears;
            public int Renders;
            public void Render(AccountTransferScreenState state) { Renders++; LastCode = state.Code; LastMessage = state.Message; }
            public void ClearSensitiveInputs() { CodeInput = ""; ValueInput = ""; Clears++; }
        }
        private sealed class Transport : IOnlineAccountTransport
        {
            public readonly List<string> Calls = new List<string>();
            public Func<string, Task<OnlineAccountResponse>> Respond;
            public bool Pending;
            public bool Completed;
            public int Issuances;
            public async Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters)
            {
                Calls.Add(endpoint);
                if (endpoint == "get-account-personal-bests") return (T)(object)new OnlinePersonalBestSnapshot
                    { status = "Success", publicPlayerNumber = "0000000002", personalBests = new OnlinePersonalBestEntry[0] };
                if (Respond != null) return (T)(object)await Respond(endpoint);
                if (endpoint == "get-public-player-number") return (T)(object)new OnlineAccountResponse
                    { status = "Success", publicPlayerNumber = Completed ? "0000000002" : "0000000001" };
                if (endpoint == "start-account-transfer" || endpoint == "reissue-account-transfer")
                {
                    Pending = true; Issuances++;
                    return (T)(object)new OnlineAccountResponse { status = "Success", code = "ABCD-2345",
                        verificationValue = Issuances == 1 ? "000000007" : "000000008", expiresAtMilliseconds = 100000 };
                }
                if (endpoint == "complete-account-transfer")
                { Completed = true; return (T)(object)new OnlineAccountResponse { status = "Success", publicPlayerNumber = "0000000002" }; }
                if (endpoint == "cancel-account-transfer")
                { Pending = false; Assert.That(parameters["transferId"], Is.EqualTo(Id)); return (T)(object)new OnlineAccountResponse
                    { status = "Active", transferStatus = "Cancelled", transferId = Id }; }
                if (Pending) return (T)(object)new OnlineAccountResponse { status = "TransferPending", transferStatus = "TransferPending",
                    transferId = Id, expiresAtMilliseconds = 100000, credentialReissueRequired = true };
                return (T)(object)new OnlineAccountResponse { status = "Active", transferStatus = "None" };
            }
        }
        private sealed class Fixture
        {
            public readonly OnlineTestFileStore File = new OnlineTestFileStore();
            public readonly OnlineTestAuthentication Auth = new OnlineTestAuthentication();
            public readonly Transport Service = new Transport();
            public readonly View Screen = new View();
            public readonly LocalRecordRepository Local;
            public readonly OnlineAccountCoordinator Account;
            public readonly AccountTransferController Controller;
            public Fixture(bool consent = true, bool reachable = true, Func<Task> retry = null,
                Func<int, Task> panelDelay = null, IOnlineAuthenticationGateway authentication = null)
            {
                File.Contents = LocalSaveJsonCodec.Serialize(new LocalSaveData(6, "owner", new LocalSettingsData(60, true, null),
                    true, null, new OnlineAccountState(consent, consent ? "ugs-player" : "")));
                Local = new LocalRecordRepository(File); Local.TryLoad(out LocalSaveData ignored);
                Account = new OnlineAccountCoordinator(Local, authentication == null ? Auth : authentication, Service, () => reachable,
                    panelDelay: panelDelay);
                Controller = new AccountTransferController(Local, Account, Screen, retry);
            }
        }
        private static RecordSubmissionCandidate Candidate(string owner = "owner")
        {
            RecordSubmissionPolicy.TryCreateStageCandidate(owner, Id, "stage-001", 1, E_StageResultType.Cleared, 1,
                out RecordSubmissionCandidate candidate); return candidate;
        }

        private sealed class DelayedAuthentication : IOnlineAuthenticationGateway
        {
            public readonly TaskCompletionSource<OnlineAuthenticationResult> Response = new TaskCompletionSource<OnlineAuthenticationResult>();
            public Task<OnlineAuthenticationResult> TryAuthenticateAsync() { return Response.Task; }
        }

        [Test]
        public async Task PanelStatusTimeout_ReleasesBusyAndIgnoresLateResponseThenExplicitRetryWorks()
        {
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>();
            TaskCompletionSource<OnlineAccountResponse> response = new TaskCompletionSource<OnlineAccountResponse>();
            Fixture f = new Fixture(panelDelay: milliseconds => { Assert.That(milliseconds, Is.EqualTo(5000)); return deadline.Task; });
            f.Service.Respond = endpoint => response.Task;
            Task opening = f.Controller.OpenAsync();
            Assert.That(f.Controller.State.IsBusy, Is.True);
            deadline.SetResult(true); await opening;
            Assert.That(f.Controller.State.IsBusy, Is.False);
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
            Assert.That(f.Service.Calls.Count, Is.EqualTo(1));
            response.SetResult(new OnlineAccountResponse { status = "Active", transferStatus = "None" });
            await Task.Yield();
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
            Assert.That(f.Local.OnlineAccount.PublicNumberCache, Is.Empty);
            deadline = new TaskCompletionSource<bool>(); f.Service.Respond = null;
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Refresh);
            Assert.That(f.Account.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.Ready));
        }

        [Test]
        public async Task PanelAuthenticationTimeout_DoesNotStartRemoteRequestsAfterLateAuthentication()
        {
            DelayedAuthentication authentication = new DelayedAuthentication();
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>();
            Fixture f = new Fixture(panelDelay: milliseconds => deadline.Task, authentication: authentication);
            Task opening = f.Controller.OpenAsync(); deadline.SetResult(true); await opening;
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
            Assert.That(f.Controller.State.IsBusy, Is.False);
            authentication.Response.SetResult(new OnlineAuthenticationResult(true, "ugs-player")); await Task.Yield();
            Assert.That(f.Service.Calls, Is.Empty);
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
        }

        [Test]
        public async Task PanelNumberTimeout_UsesSingleDeadlineAcrossStatusAndNumberAndDoesNotCacheLateNumber()
        {
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>();
            TaskCompletionSource<OnlineAccountResponse> number = new TaskCompletionSource<OnlineAccountResponse>();
            int budgets = 0;
            Fixture f = new Fixture(panelDelay: milliseconds => { budgets++; Assert.That(milliseconds, Is.EqualTo(5000)); return deadline.Task; });
            f.Service.Respond = endpoint => endpoint == "get-public-player-number" ? number.Task :
                Task.FromResult(new OnlineAccountResponse { status = "Active", transferStatus = "None" });
            Task opening = f.Controller.OpenAsync();
            Assert.That(f.Service.Calls, Does.Contain("get-public-player-number"));
            deadline.SetResult(true); await opening;
            Assert.That(budgets, Is.EqualTo(1));
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
            number.SetResult(new OnlineAccountResponse { status = "Success", publicPlayerNumber = "0000000001" }); await Task.Yield();
            Assert.That(f.Local.OnlineAccount.PublicNumberCache, Is.Empty);
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
        }

        [Test]
        public async Task PanelFastSuccess_DoesNotWaitForDeadline()
        {
            TaskCompletionSource<bool> deadline = new TaskCompletionSource<bool>();
            Fixture f = new Fixture(panelDelay: milliseconds => deadline.Task); await f.Controller.OpenAsync();
            Assert.That(deadline.Task.IsCompleted, Is.False);
            Assert.That(f.Account.ViewState.State, Is.EqualTo(E_OnlineAccountDisplayState.Ready));
            Assert.That(f.Controller.State.IsBusy, Is.False);
        }

        [Test]
        public async Task PanelExpiredBudget_StartsNoAuthenticationOrRemoteRequests()
        {
            Fixture f = new Fixture(panelDelay: milliseconds => Task.CompletedTask); await f.Controller.OpenAsync();
            Assert.That(f.Auth.Calls, Is.Zero); Assert.That(f.Service.Calls, Is.Empty);
            Assert.That(f.Account.ViewState.Reason, Is.EqualTo("Timeout"));
        }

        [Test]
        public async Task NoConsentOpeningAndClosing_MakesZeroRequests()
        {
            Fixture f = new Fixture(false); await f.Controller.OpenAsync();
            Assert.That(f.Controller.State.HasConsent, Is.False); Assert.That(f.Controller.CanExecute(E_AccountTransferAction.Start), Is.False);
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Start); f.Controller.Close();
            Assert.That(f.Auth.Calls, Is.Zero); Assert.That(f.Service.Calls, Is.Empty); Assert.That(f.Screen.LastCode, Is.Empty);
        }
        [Test]
        public async Task ExplicitConsent_IsSavedBeforeFirstAuthentication()
        {
            Fixture f = new Fixture(false); await f.Controller.OpenAsync();
            f.Service.Respond = endpoint => {
                Assert.That(LocalSaveJsonCodec.TryDeserialize(f.File.Contents, out LocalSaveData saved), Is.True);
                Assert.That(saved.OnlineAccount.HasConfirmedRecoveryNotice, Is.True);
                return Task.FromResult(endpoint == "get-public-player-number" ? new OnlineAccountResponse
                    { status = "Success", publicPlayerNumber = "0000000001" } : new OnlineAccountResponse { status = "Active", transferStatus = "None" }); };
            await f.Controller.ExecuteAsync(E_AccountTransferAction.ConfirmConsent);
            Assert.That(f.Controller.State.NumberText, Is.EqualTo("0000000001"));
        }
        [Test]
        public async Task FailedConsentSave_MakesZeroAuthenticationCalls()
        {
            Fixture f = new Fixture(false); await f.Controller.OpenAsync(); f.File.FailWrite = true;
            await f.Controller.ExecuteAsync(E_AccountTransferAction.ConfirmConsent);
            Assert.That(f.Auth.Calls, Is.Zero); Assert.That(f.Service.Calls, Is.Empty);
            Assert.That(f.Controller.State.Message, Does.Contain("Could not save consent"));
        }
        [Test]
        public async Task OfflineOpening_AllowsCloseAndMakesZeroRemoteCalls()
        {
            Fixture f = new Fixture(reachable: false); await f.Controller.OpenAsync();
            Assert.That(f.Service.Calls, Is.Empty); Assert.That(f.Auth.Calls, Is.Zero);
            Assert.That(f.Account.ViewState.IsOfflinePlayAllowed, Is.True); f.Controller.Close(); Assert.That(f.Controller.State.IsOpen, Is.False);
        }
        [Test]
        public async Task StartAndReissue_KeepOnlyLatestRuntimeSecretsAndOriginalExpiry()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); await f.Controller.ExecuteAsync(E_AccountTransferAction.Start);
            Assert.That(f.Controller.State.Code, Is.EqualTo("ABCD-2345")); Assert.That(f.Controller.State.VerificationValue, Is.EqualTo("000000007"));
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Reissue);
            Assert.That(f.Controller.State.VerificationValue, Is.EqualTo("000000008")); Assert.That(f.Controller.State.ExpiresAtMilliseconds, Is.EqualTo(100000));
            Assert.That(f.File.Contents, Does.Not.Contain("ABCD-2345")); Assert.That(f.File.Contents, Does.Not.Contain("verificationValue"));
            f.Controller.Close(); Assert.That(f.Controller.State.Code, Is.Empty); Assert.That(f.Controller.State.VerificationValue, Is.Empty);
        }
        [Test]
        public async Task ReopenPending_DoesNotRestoreSecretsOrStartNewTransfer()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); await f.Controller.ExecuteAsync(E_AccountTransferAction.Start); f.Controller.Close();
            await f.Controller.OpenAsync(); Assert.That(f.Controller.State.Page, Is.EqualTo(E_AccountTransferPage.Issued));
            Assert.That(f.Controller.State.Code, Is.Empty); Assert.That(f.Service.Issuances, Is.EqualTo(1));
        }
        [Test]
        public async Task CloseDuringIssue_LateResponseCannotReopenOrRetainCredentials()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); TaskCompletionSource<OnlineAccountResponse> delayed = new TaskCompletionSource<OnlineAccountResponse>();
            f.Service.Respond = _ => delayed.Task; Task task = f.Controller.ExecuteAsync(E_AccountTransferAction.Start);
            f.Controller.Close(); await f.Controller.OpenAsync();
            int rendersAfterClose = f.Screen.Renders;
            OnlineAccountResponse raw = new OnlineAccountResponse { status = "Success", code = "ABCD-2345", verificationValue = "000000007", expiresAtMilliseconds = 100000 };
            delayed.SetResult(raw); await task;
            Assert.That(f.Controller.State.IsOpen, Is.False); Assert.That(f.Controller.State.Code, Is.Empty); Assert.That(f.Screen.LastCode, Is.Empty);
            Assert.That(f.Screen.Renders, Is.EqualTo(rendersAfterClose));
            Assert.That(raw.code, Is.Empty); Assert.That(raw.verificationValue, Is.Empty);
            Assert.That(f.Controller.State.IsBusy, Is.False);
        }
        [Test]
        public async Task DoubleClicks_CoalesceUntilRequestSettles()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); TaskCompletionSource<OnlineAccountResponse> delayed = new TaskCompletionSource<OnlineAccountResponse>();
            f.Service.Respond = _ => delayed.Task; int baseline = f.Service.Calls.Count;
            Task first = f.Controller.ExecuteAsync(E_AccountTransferAction.Start);
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Start); await f.Controller.ExecuteAsync(E_AccountTransferAction.Refresh);
            Assert.That(f.Service.Calls.Count - baseline, Is.EqualTo(1)); Assert.That(f.Controller.State.NumberText, Is.Empty);
            delayed.SetResult(new OnlineAccountResponse { status = "Success", code = "ABCD-2345", verificationValue = "000000007", expiresAtMilliseconds = 100000 }); await first;
        }
        [TestCase(E_AccountTransferAction.Start)]
        [TestCase(E_AccountTransferAction.Complete)]
        public async Task PendingForAnyOwner_BlocksMutationBeforeAuthentication(E_AccountTransferAction action)
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync();
            if (action == E_AccountTransferAction.Complete) await f.Controller.ExecuteAsync(E_AccountTransferAction.OpenInput);
            f.Local.TryEnqueuePending(Candidate("foreign")); int auth = f.Auth.Calls, calls = f.Service.Calls.Count;
            await f.Controller.ExecuteAsync(action, "ABCD-2345", "000000007");
            Assert.That(f.Auth.Calls, Is.EqualTo(auth)); Assert.That(f.Service.Calls.Count, Is.EqualTo(calls)); Assert.That(f.Controller.State.Message, Does.Contain("pending records"));
        }
        [Test]
        public async Task DiscardRequiresConfirmation_CancelAndFailedSavePreservePending()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); f.Local.TryEnqueuePending(Candidate()); f.Local.TryCheckpoint();
            await f.Controller.ExecuteAsync(E_AccountTransferAction.ConfirmDiscard); Assert.That(f.Local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            await f.Controller.ExecuteAsync(E_AccountTransferAction.RequestDiscard); await f.Controller.ExecuteAsync(E_AccountTransferAction.CancelDiscard);
            Assert.That(f.Local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            await f.Controller.ExecuteAsync(E_AccountTransferAction.RequestDiscard); f.File.FailWrite = true;
            await f.Controller.ExecuteAsync(E_AccountTransferAction.ConfirmDiscard);
            Assert.That(f.Local.CreatePendingSnapshot().Count, Is.EqualTo(1)); Assert.That(f.Controller.State.Message, Does.Contain("Save failed"));
            Assert.That(f.Controller.State.Page, Is.EqualTo(E_AccountTransferPage.DiscardConfirmation));
        }
        [Test]
        public async Task SuccessfulDiscard_DoesNotAutomaticallyStartTransferOrTouchBest()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); RecordSubmissionCandidate best = Candidate();
            f.Local.TryEnqueuePending(best); f.Local.TryUpdatePersonalBest(best); f.Local.TryCheckpoint(); int calls = f.Service.Calls.Count;
            await f.Controller.ExecuteAsync(E_AccountTransferAction.RequestDiscard); await f.Controller.ExecuteAsync(E_AccountTransferAction.ConfirmDiscard);
            Assert.That(f.Local.CreatePendingSnapshot(), Is.Empty); Assert.That(f.Service.Calls.Count, Is.EqualTo(calls));
            Assert.That(f.Local.TryGetPersonalBest("owner", best.BoardKey, out RecordSubmissionCandidate kept), Is.True); Assert.That(kept.SubmissionId, Is.EqualTo(Id));
            Assert.That(f.Controller.State.Page, Is.EqualTo(E_AccountTransferPage.Result));
            Assert.That(f.Controller.State.IsOpen, Is.True);
            Assert.That(f.Screen.LastMessage, Is.EqualTo("Pending records discarded. Online records were kept."));
            Assert.That(f.Controller.CanExecute(E_AccountTransferAction.Start), Is.True);
            Assert.That(f.Controller.CanExecute(E_AccountTransferAction.RequestDiscard), Is.False);
            Assert.That(f.Screen.LastMessage, Is.EqualTo("Pending records discarded. Online records were kept."));
        }
        [Test]
        public async Task CompleteClearsInputsBeforeTransportAndShowsAppliedResult()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); await f.Controller.ExecuteAsync(E_AccountTransferAction.OpenInput);
            f.Screen.CodeInput = "ABCD-2345"; f.Screen.ValueInput = "000000007";
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Complete, f.Screen.CodeInput, f.Screen.ValueInput);
            Assert.That(f.Screen.CodeInput, Is.Empty); Assert.That(f.Screen.ValueInput, Is.Empty);
            Assert.That(f.Controller.State.Page, Is.EqualTo(E_AccountTransferPage.Result)); Assert.That(f.Controller.State.NumberText, Is.EqualTo("0000000002"));
            Assert.That(f.Controller.CanExecute(E_AccountTransferAction.Start), Is.True); Assert.That(f.Controller.State.Code, Is.Empty);
        }
        [Test]
        public async Task CancelObtainsCurrentTransferIdAndClearsIssuedValues()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); await f.Controller.ExecuteAsync(E_AccountTransferAction.Start);
            await f.Controller.ExecuteAsync(E_AccountTransferAction.CancelTransfer);
            Assert.That(f.Service.Pending, Is.False); Assert.That(f.Controller.State.Code, Is.Empty); Assert.That(f.Controller.State.Message, Does.Contain("cancelled"));
        }
        [Test]
        public async Task TimeoutAndRawServiceReason_AreSafeAndPreserveLocalData()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); string before = f.File.Contents;
            f.Service.Respond = _ => Task.FromException<OnlineAccountResponse>(new TimeoutException("private-token-secret"));
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Start);
            Assert.That(f.File.Contents, Is.EqualTo(before)); Assert.That(f.Controller.State.Message, Does.Not.Contain("private-token"));
            Assert.That(f.Controller.State.Message, Does.Contain("Refresh Status"));
        }
        [Test]
        public async Task InvalidComplete_DoesNotSendOrEchoRawInputs()
        {
            Fixture f = new Fixture(); await f.Controller.OpenAsync(); await f.Controller.ExecuteAsync(E_AccountTransferAction.OpenInput); int before = f.Service.Calls.Count;
            await f.Controller.ExecuteAsync(E_AccountTransferAction.Complete, "private-secret", "bad");
            Assert.That(f.Service.Calls.Count, Is.EqualTo(before)); Assert.That(f.Controller.State.Message, Does.Not.Contain("private-secret"));
            Assert.That(f.Controller.State.Code, Is.Empty);
        }

        [Test]
        public async Task RetryPending_UsesInjectedCoordinatorWithoutImplicitDiscardOrStart()
        {
            int retries = 0;
            Fixture f = new Fixture(retry: () => { retries++; return Task.CompletedTask; });
            await f.Controller.OpenAsync(); f.Local.TryEnqueuePending(Candidate()); f.Local.TryCheckpoint();
            int calls = f.Service.Calls.Count;
            await f.Controller.ExecuteAsync(E_AccountTransferAction.RetryPending);
            Assert.That(retries, Is.EqualTo(1)); Assert.That(f.Local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(f.Service.Calls.Count, Is.EqualTo(calls)); Assert.That(f.Controller.State.Message, Does.Contain("pending records"));
        }
    }
}
