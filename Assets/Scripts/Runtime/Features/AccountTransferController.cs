using System;
using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public sealed class AccountTransferController
    {
        private const string PendingNotice = "This device has pending records. Upload or discard them before transferring your account.";
        private readonly LocalRecordRepository _local;
        private readonly OnlineAccountCoordinator _account;
        private readonly IAccountTransferView _view;
        private readonly Func<Task> _retryPending;
        private int _generation;
        private E_AccountTransferPage _beforeDiscard;
        public AccountTransferScreenState State { get; } = new AccountTransferScreenState();

        public AccountTransferController(LocalRecordRepository local, OnlineAccountCoordinator account,
            IAccountTransferView view, Func<Task> retryPending = null)
        {
            _local = local;
            _account = account;
            _view = view;
            _retryPending = retryPending;
        }

        public async Task OpenAsync()
        {
            if (State.IsBusy || State.IsOpen) return;
            _generation++;
            State.IsOpen = true;
            State.Page = E_AccountTransferPage.Overview;
            ClearCredentials();
            _view.ClearSensitiveInputs();
            State.Message = _account.ViewState.RecoveryNotice;
            Render();
            if (State.HasConsent) await ExecuteAsync(E_AccountTransferAction.Refresh);
        }

        public void Close()
        {
            _generation++;
            State.IsOpen = false;
            State.Page = E_AccountTransferPage.Overview;
            State.NumberText = string.Empty;
            State.Message = string.Empty;
            ClearCredentials();
            _view.ClearSensitiveInputs();
            _view.Render(State);
            // Closing a screen does not cancel a server mutation or invalidate
            // its durable handoff. Only presentation of late secrets is fenced.
        }

        public bool CanExecute(E_AccountTransferAction action)
        {
            if (!State.IsOpen) return false;
            if (action == E_AccountTransferAction.Back) return true;
            if (State.IsBusy) return false;
            bool consent = _local.OnlineAccount.HasConfirmedRecoveryNotice;
            bool pending = _local.CreatePendingSnapshot().Count != 0;
            bool transfer = _account.ViewState.State == E_OnlineAccountDisplayState.TransferPending;
            switch (action)
            {
                case E_AccountTransferAction.Start: return consent && !pending && !transfer &&
                    _account.ViewState.State == E_OnlineAccountDisplayState.Ready &&
                    (State.Page == E_AccountTransferPage.Overview || State.Page == E_AccountTransferPage.Result);
                case E_AccountTransferAction.OpenInput: return consent &&
                    (State.Page == E_AccountTransferPage.Overview || State.Page == E_AccountTransferPage.Result);
                case E_AccountTransferAction.Complete: return consent && !pending && State.Page == E_AccountTransferPage.Input;
                case E_AccountTransferAction.Reissue:
                case E_AccountTransferAction.CancelTransfer: return consent && transfer;
                case E_AccountTransferAction.Refresh: return consent && State.Page != E_AccountTransferPage.DiscardConfirmation;
                case E_AccountTransferAction.ConfirmConsent: return !consent && State.Page == E_AccountTransferPage.Overview;
                case E_AccountTransferAction.RetryPending: return consent && pending && _retryPending != null;
                case E_AccountTransferAction.RequestDiscard: return pending && State.Page != E_AccountTransferPage.DiscardConfirmation;
                case E_AccountTransferAction.ConfirmDiscard:
                case E_AccountTransferAction.CancelDiscard: return State.Page == E_AccountTransferPage.DiscardConfirmation;
                default: return false;
            }
        }

        public async Task ExecuteAsync(E_AccountTransferAction action, string code = "", string verificationValue = "")
        {
            if (action == E_AccountTransferAction.Back) { Close(); return; }
            if (!State.IsOpen || State.IsBusy) return;
            if ((action == E_AccountTransferAction.Start || action == E_AccountTransferAction.Complete) &&
                _local.CreatePendingSnapshot().Count != 0)
            { State.Message = PendingNotice; Render(); return; }
            if (!CanExecute(action)) return;
            if (action == E_AccountTransferAction.OpenInput)
            {
                ClearCredentials(); _view.ClearSensitiveInputs(); State.Page = E_AccountTransferPage.Input;
                State.Message = "Enter the code and 9-digit verification value from your old device. Do not enter your public number.";
                Render(); return;
            }
            if (action == E_AccountTransferAction.RequestDiscard)
            {
                _beforeDiscard = State.Page; ClearCredentials(); _view.ClearSensitiveInputs();
                State.Page = E_AccountTransferPage.DiscardConfirmation;
                State.Message = "Discard all pending records in this environment? Online records will not be deleted.";
                Render(); return;
            }
            if (action == E_AccountTransferAction.CancelDiscard)
            { State.Page = _beforeDiscard; State.Message = PendingNotice; Render(); return; }
            int generation = _generation;
            State.IsBusy = true;
            ClearCredentials();
            if (action == E_AccountTransferAction.Complete) _view.ClearSensitiveInputs();
            State.Message = "Processing... Closing this window does not cancel the server request.";
            Render();
            OnlineAccountResponse response = null;
            try
            {
                if (action == E_AccountTransferAction.ConfirmDiscard)
                {
                    bool saved = _local.TryDiscardPending();
                    State.Page = saved ? E_AccountTransferPage.Result : E_AccountTransferPage.DiscardConfirmation;
                    State.Message = saved ? "Pending records discarded. Online records were kept." : "Save failed. Pending records were kept.";
                }
                else if (action == E_AccountTransferAction.RetryPending)
                {
                    await _retryPending();
                    if (!IsCurrent(generation)) return;
                    State.Message = _local.CreatePendingSnapshot().Count == 0 ? "Pending records processed." : PendingNotice;
                }
                else
                {
                    switch (action)
                    {
                        case E_AccountTransferAction.ConfirmConsent:
                            OnlineAccountState before = _local.OnlineAccount;
                            if (!_local.TrySaveOnlineAccount(new OnlineAccountState(true, before.PlayerId, before.PublicNumberCache)))
                            { State.Message = "Could not save consent. No online request was started."; return; }
                            response = await RefreshAsync(generation); break;
                        case E_AccountTransferAction.Refresh: response = await RefreshAsync(generation); break;
                        case E_AccountTransferAction.Start: response = await _account.StartTransferAsync(); break;
                        case E_AccountTransferAction.Complete: response = await _account.CompleteTransferAsync(code, verificationValue); break;
                        case E_AccountTransferAction.Reissue: response = await _account.ReissueTransferAsync(); break;
                        case E_AccountTransferAction.CancelTransfer:
                            response = await _account.RefreshStatusAsync();
                            if (_account.ViewState.State == E_OnlineAccountDisplayState.TransferPending && IsCurrent(generation))
                                response = await _account.CancelTransferAsync();
                            break;
                    }
                    if (IsCurrent(generation)) ApplyResponse(action, response);
                }
            }
            catch (Exception)
            {
                if (IsCurrent(generation)) State.Message = "Could not verify the request. Select Refresh Status. Offline play is still available.";
            }
            finally
            {
                if (response != null) { response.code = string.Empty; response.verificationValue = string.Empty; }
                code = string.Empty; verificationValue = string.Empty;
                State.IsBusy = false;
                if (IsCurrent(generation)) Render();
            }
        }

        private async Task<OnlineAccountResponse> RefreshAsync(int generation)
        {
            return await _account.RefreshPanelAsync(() => IsCurrent(generation));
        }

        private void ApplyResponse(E_AccountTransferAction action, OnlineAccountResponse response)
        {
            if (response == null) { State.Message = "Could not verify the response. Select Refresh Status."; return; }
            if ((action == E_AccountTransferAction.Start || action == E_AccountTransferAction.Reissue) && response.status == "Success")
            {
                State.Page = E_AccountTransferPage.Issued;
                State.Code = response.code; State.VerificationValue = response.verificationValue;
                State.ExpiresAtMilliseconds = response.expiresAtMilliseconds;
                State.Message = "Enter these details on your new device. Closing clears them. Reissue invalidates the old details without extending expiry.";
            }
            else if (response.status == "TransferPending")
            { State.Page = E_AccountTransferPage.Issued; State.Message = "Transfer pending. The original details cannot be restored. Reissue or cancel the transfer."; }
            else if (response.status == "AlreadyCompleted" || response.status == "Inactive" ||
                response.status == "Success" && action == E_AccountTransferAction.Complete || response.transferStatus == "Completed")
            { State.Page = E_AccountTransferPage.Result; State.Message = response.status == "Inactive" ? "Transfer confirmed. This device now uses a new anonymous account." : "Transfer confirmed. Server personal bests have been applied."; }
            else if (response.status == "Active")
            {
                State.Page = E_AccountTransferPage.Overview;
                State.Message = response.transferStatus == "Cancelled" ? "Transfer cancelled. The existing connection is unchanged." :
                    response.transferStatus == "Expired" ? "Transfer expired. The existing connection is unchanged." : _account.ViewState.RecoveryNotice;
                if (_account.ViewState.CanRetry) State.Message = "Could not verify your public number. Select Refresh Status. Offline play is still available.";
            }
            else
            {
                switch (response.reason)
                {
                    case "PendingMustBeCleared": State.Message = PendingNotice; break;
                    case "InvalidInput":
                    case "InvalidCredential": State.Message = "Invalid code or verification value. Enter them again."; break;
                    case "TooManyRequests": State.Message = "Try again shortly. Wait 5 seconds between checks for the same transfer."; break;
                    default: State.Message = "Could not verify the request. Select Refresh Status. Offline play is still available."; break;
                }
            }
        }

        private bool IsCurrent(int generation) { return State.IsOpen && generation == _generation; }
        private void ClearCredentials() { State.Code = string.Empty; State.VerificationValue = string.Empty; State.ExpiresAtMilliseconds = 0; }
        private void Render()
        {
            State.NumberText = State.IsBusy ? string.Empty : _account.ViewState.NumberText;
            State.PendingCount = _local.CreatePendingSnapshot().Count;
            State.HasConsent = _local.OnlineAccount.HasConfirmedRecoveryNotice;
            State.IsTransferPending = _account.ViewState.State == E_OnlineAccountDisplayState.TransferPending;
            if (State.PendingCount != 0 && State.Message == _account.ViewState.RecoveryNotice)
                State.Message += "\n" + PendingNotice;
            _view.Render(State);
        }
    }
}
