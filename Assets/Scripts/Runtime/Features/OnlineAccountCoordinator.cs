using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    // One runtime session. Neither cached numbers nor response statuses grant
    // authority; every server request uses the authenticated player context.
    public sealed class OnlineAccountCoordinator : IOnlineAuthenticationGateway
    {
        private const int MaximumSnapshotBoards = 2;
        public const int PanelTimeoutMilliseconds = 5000;
        private const long MaximumServerRankingValue = 9007199254740991L;
        private readonly LocalRecordRepository _local;
        private readonly IOnlineAuthenticationGateway _authentication;
        private readonly IOnlineAccountTransport _transport;
        private readonly Func<bool> _isReachable;
        private readonly AccountTransferPendingGate _pendingGate;
        private readonly Action _onAccountChanged;
        private int _generation;
        private bool _isRequestRunning;
        private Task<OnlineAuthenticationResult> _authenticationRequest;
        private Task _authenticationDeadline;
        private OnlineAccountResponse _authenticationPanelResponse;
        private int _authenticationGeneration;
        private bool _completionAwaitingHandoff;
        private string _sessionToClear = string.Empty;
        private readonly Func<int, Task> _panelDelay;
        private Task _panelDeadline;
        public OnlineAccountViewState ViewState { get; } = new OnlineAccountViewState();

        public OnlineAccountCoordinator(LocalRecordRepository local, IOnlineAuthenticationGateway authentication,
            IOnlineAccountTransport transport, Func<bool> isReachable, Action onAccountChanged = null,
            Func<int, Task> panelDelay = null)
        {
            _local = local;
            _authentication = authentication;
            _transport = transport;
            _isReachable = isReachable;
            _pendingGate = new AccountTransferPendingGate(local);
            _onAccountChanged = onAccountChanged;
            _panelDelay = panelDelay == null ? (milliseconds => Task.Delay(milliseconds)) : panelDelay;
        }

        public void Invalidate()
        {
            _generation++;
            ViewState.Set(E_OnlineAccountDisplayState.NotRequested);
        }

        public async Task<OnlineAuthenticationResult> TryAuthenticateAsync()
        {
            // Top/around queries share the same in-flight identity check. Mutating
            // account actions still use the exclusive request gate below.
            if (_authenticationRequest != null) return await _authenticationRequest;
            bool ownsDeadline = _panelDeadline == null;
            if (ownsDeadline) _authenticationDeadline = _panelDelay(PanelTimeoutMilliseconds);
            _authenticationRequest = AuthenticateForRecordsAsync();
            try { return await _authenticationRequest; }
            finally
            {
                _authenticationRequest = null;
                if (ownsDeadline) _authenticationDeadline = null;
            }
        }

        private async Task<OnlineAuthenticationResult> AuthenticateForRecordsAsync()
        {
            if (_isRequestRunning || _completionAwaitingHandoff)
                return new OnlineAuthenticationResult(false, string.Empty);
            _isRequestRunning = true;
            int generation = _generation;
            _authenticationGeneration = generation;
            _authenticationPanelResponse = null;
            try
            {
                string player = await AuthenticateAsync(generation);
                if (player == null) return new OnlineAuthenticationResult(false, string.Empty);
                OnlineAccountState binding = _local.OnlineAccount;
                OnlineAccountResponse status = await AwaitPanelRequestAsync(() => _transport.CallAsync<OnlineAccountResponse>(
                    "get-account-transfer-status", new Dictionary<string, string>()));
                if (!IsCurrent(player, generation) || !ReferenceEquals(binding, _local.OnlineAccount))
                { Fail(generation, "StaleResponse"); return new OnlineAuthenticationResult(false, string.Empty); }
                OnlineAccountResponse accepted = ApplyResponse("get-account-transfer-status", status, generation);
                _authenticationPanelResponse = accepted;
                if (accepted.status == "TransientFailure" || accepted.status == "TransferPending")
                    return new OnlineAuthenticationResult(false, string.Empty);
                if (_completionAwaitingHandoff)
                {
                    if (!await FinishCompletionAsync(player, generation, false))
                        return new OnlineAuthenticationResult(false, string.Empty);
                    player = _local.OnlineAccount.PlayerId;
                    if (ViewState.State == E_OnlineAccountDisplayState.Ready)
                        return new OnlineAuthenticationResult(true, player);
                    if (ViewState.State == E_OnlineAccountDisplayState.Ready)
                        return new OnlineAuthenticationResult(true, player);
                }
                if (ViewState.State == E_OnlineAccountDisplayState.TransferPending ||
                    ViewState.State == E_OnlineAccountDisplayState.Inactive ||
                    ViewState.State == E_OnlineAccountDisplayState.Completed)
                    return new OnlineAuthenticationResult(false, string.Empty);
                ViewState.Set(E_OnlineAccountDisplayState.Loading);
                bool ready = await FetchNumberAsync(player, generation);
                return new OnlineAuthenticationResult(ready, ready ? player : string.Empty);
            }
            catch (TimeoutException) { Fail(generation, "Timeout"); }
            catch (Exception) { Fail(generation, "ServiceUnavailable"); }
            finally { _isRequestRunning = false; }
            return new OnlineAuthenticationResult(false, string.Empty);
        }

        public Task<OnlineAuthenticationResult> RetryPublicNumberAsync()
        {
            if (_authenticationRequest != null) return _authenticationRequest;
            if (_isRequestRunning || _completionAwaitingHandoff)
                return Task.FromResult(new OnlineAuthenticationResult(false, string.Empty));
            Invalidate();
            return TryAuthenticateAsync();
        }

        public Task<OnlineAccountResponse> RefreshStatusAsync()
        {
            return RequestAsync("get-account-transfer-status", new Dictionary<string, string>(), false);
        }

        public async Task<OnlineAccountResponse> RefreshPanelAsync(Func<bool> canContinue = null)
        {
            if (_panelDeadline != null) return Error("RequestInProgress");
            // One budget for authentication, status and public number, not five seconds per call.
            _panelDeadline = _panelDelay(PanelTimeoutMilliseconds);
            try
            {
                Task<OnlineAuthenticationResult> startup = _authenticationRequest;
                if (startup != null)
                {
                    // Join the already-running startup recovery instead of issuing
                    // another status/number chain or reporting a false busy error.
                    if (await Task.WhenAny(startup, _panelDeadline) != startup) return Error("Timeout");
                    OnlineAuthenticationResult authenticated = await startup;
                    if (canContinue != null && !canContinue()) return Error("StaleResponse");
                    if (_authenticationGeneration != _generation) return Error("StaleResponse");
                    if (authenticated.IsAuthenticated && _local.CanUseOnlineData &&
                        authenticated.PlayerId == _local.OnlineAccount.PlayerId &&
                        ViewState.State == E_OnlineAccountDisplayState.Ready && _authenticationPanelResponse != null)
                        return _authenticationPanelResponse;
                    if (ViewState.State == E_OnlineAccountDisplayState.TransferPending && _authenticationPanelResponse != null)
                        return _authenticationPanelResponse;
                    return Error(string.IsNullOrEmpty(ViewState.Reason) ? "ServiceUnavailable" : ViewState.Reason);
                }
                if (_isRequestRunning) return Error("RequestInProgress");
                OnlineAccountResponse response = await RefreshStatusAsync();
                if ((canContinue == null || canContinue()) && response.status == "Active" &&
                    ViewState.State != E_OnlineAccountDisplayState.Ready && response.transferStatus != "Completed")
                {
                    _isRequestRunning = true;
                    int generation = _generation;
                    try { await FetchNumberAsync(_local.OnlineAccount.PlayerId, generation); }
                    catch (TimeoutException) { Fail(generation, "Timeout"); }
                    catch (Exception) { Fail(generation, "ServiceUnavailable"); }
                    finally { _isRequestRunning = false; }
                }
                return response;
            }
            finally { _panelDeadline = null; }
        }

        private async Task<T> AwaitPanelRequestAsync<T>(Func<Task<T>> start)
        {
            Task deadline = _authenticationDeadline != null ? _authenticationDeadline : _panelDeadline;
            if (deadline != null && deadline.IsCompleted) throw new TimeoutException();
            Task<T> request = start();
            if (deadline != null && await Task.WhenAny(request, deadline) != request)
            {
                ObserveDiscardedRequestAsync(request);
                throw new TimeoutException();
            }
            return await request;
        }

        private static async void ObserveDiscardedRequestAsync<T>(Task<T> request)
        {
            // SDK/remote work is not cancelled. Never apply late data or automatically retry.
            try { await request; } catch (Exception) { }
        }

        public Task<OnlineAccountResponse> StartTransferAsync()
        {
            if (_completionAwaitingHandoff) return Task.FromResult(Error("CompletionHandoffRequired"));
            if (ViewState.State == E_OnlineAccountDisplayState.TransferPending)
                return Task.FromResult(Error("TransferPending"));
            return RequestAsync("start-account-transfer", new Dictionary<string, string>(), true);
        }

        public Task<OnlineAccountResponse> ReissueTransferAsync()
        {
            if (ViewState.State != E_OnlineAccountDisplayState.TransferPending)
                return Task.FromResult(Error("InvalidState"));
            return RequestAsync("reissue-account-transfer", new Dictionary<string, string>(), false);
        }

        public Task<OnlineAccountResponse> CancelTransferAsync()
        {
            string transferId = ViewState.TransferId;
            if (ViewState.State != E_OnlineAccountDisplayState.TransferPending || !IsTransferId(transferId))
                return Task.FromResult(Error("StatusRequired"));
            return RequestAsync("cancel-account-transfer", new Dictionary<string, string> { { "transferId", transferId } }, false);
        }

        public Task<OnlineAccountResponse> CompleteTransferAsync(string code, string verificationValue)
        {
            if (_completionAwaitingHandoff) return Task.FromResult(Error("CompletionHandoffRequired"));
            string normalized = NormalizeCode(code);
            if (normalized == null || !IsDigits(verificationValue, 9)) return Task.FromResult(Error("InvalidInput"));
            return RequestAsync("complete-account-transfer", new Dictionary<string, string>
                { { "code", normalized }, { "verificationValue", verificationValue } }, true);
        }

        private async Task<string> AuthenticateAsync(int generation)
        {
            if (_local == null || !_local.ActiveScope.Matches(OnlineDataScope.CreateConfigured()) ||
                !_local.OnlineAccount.HasConfirmedRecoveryNotice)
            {
                Fail(generation, "LocalAccountUnavailable"); return null;
            }
            if (!_local.CanUseOnlineData && !_local.TryCheckpoint())
            { Fail(generation, "LocalSaveUnavailable"); return null; }
            if (_isReachable != null && !_isReachable())
            {
                if (generation == _generation) ViewState.Set(E_OnlineAccountDisplayState.Offline, "NotReachable");
                return null;
            }
            OnlineAccountState before = _local.OnlineAccount;
            if (!string.IsNullOrEmpty(_sessionToClear))
            {
                IOnlineAuthenticationSession session = _authentication as IOnlineAuthenticationSession;
                if (session == null || !await AwaitPanelRequestAsync(() => session.TryClearSessionAsync(_sessionToClear)))
                { Fail(generation, "SessionResetUnavailable"); return null; }
                _sessionToClear = string.Empty;
                if (generation != _generation || !ReferenceEquals(before, _local.OnlineAccount)) return null;
            }
            OnlineAuthenticationResult result = await AwaitPanelRequestAsync(() => _authentication.TryAuthenticateAsync());
            if (generation != _generation || !ReferenceEquals(before, _local.OnlineAccount)) return null;
            if (!_local.CanUseOnlineData || !result.IsAuthenticated || string.IsNullOrEmpty(result.PlayerId))
            { Fail(generation, "AuthenticationUnavailable"); return null; }
            if (!string.IsNullOrEmpty(before.PlayerId) && before.PlayerId != result.PlayerId)
            { Fail(generation, "AccountMismatch"); return null; }
            if (string.IsNullOrEmpty(before.PlayerId) &&
                !_local.TrySaveOnlineAccount(new OnlineAccountState(true, result.PlayerId)))
            { Fail(generation, "LocalSaveUnavailable"); return null; }
            return result.PlayerId;
        }

        private bool IsCurrent(string player, int generation)
        {
            return generation == _generation && _local.CanUseOnlineData && _local.OnlineAccount.PlayerId == player;
        }

        private async Task<bool> FetchNumberAsync(string player, int generation)
        {
            OnlineAccountState binding = _local.OnlineAccount;
            OnlineAccountResponse response = await AwaitPanelRequestAsync(() => _transport.CallAsync<OnlineAccountResponse>(
                "get-public-player-number", new Dictionary<string, string>()));
            if (!IsCurrent(player, generation) || !ReferenceEquals(binding, _local.OnlineAccount))
            { Fail(generation, "StaleResponse"); return false; }
            if (response == null || response.status != "Success" || !PublicPlayerNumber.IsValid(response.publicPlayerNumber))
            { Fail(generation, "PublicNumberUnavailable"); return false; }
            // A later transfer/cancel may have replaced the latest terminal
            // status before this app resumes. Never overwrite an old C number
            // without replacing that C's whole local best set as well.
            if (!string.IsNullOrEmpty(binding.PublicNumberCache) && binding.PublicNumberCache != response.publicPlayerNumber)
            {
                _completionAwaitingHandoff = true;
                return await FinishCompletionAsync(player, generation, false);
            }
            if (!_local.TrySaveOnlineAccount(new OnlineAccountState(true, player, response.publicPlayerNumber)))
            { Fail(generation, "LocalSaveUnavailable"); return false; }
            ViewState.Set(E_OnlineAccountDisplayState.Ready, number: response.publicPlayerNumber);
            return true;
        }

        private async Task<OnlineAccountResponse> RequestAsync(string endpoint, Dictionary<string, string> arguments, bool requiresEmptyPending)
        {
            if (_isRequestRunning) return Error("RequestInProgress");
            _isRequestRunning = true;
            int generation = ++_generation;
            OnlineAccountResponse accepted = Error("LocalSaveUnavailable");
            try
            {
                async Task<bool> SendAsync()
                {
                    string player = await AuthenticateAsync(generation);
                    if (player == null) { accepted = Error(ViewState.Reason); return false; }
                    if ((endpoint == "start-account-transfer" || endpoint == "complete-account-transfer") &&
                        string.IsNullOrEmpty(_local.OnlineAccount.PublicNumberCache))
                        if (!await FetchNumberAsync(player, generation))
                        { accepted = Error(ViewState.Reason); return false; }
                    // A request may commit despite a lost response. Stop using
                    // an old Ready state before issuing any mutating endpoint.
                    ViewState.Set(E_OnlineAccountDisplayState.Loading);
                    OnlineAccountState binding = _local.OnlineAccount;
                    OnlineAccountResponse response = await AwaitPanelRequestAsync(() => _transport.CallAsync<OnlineAccountResponse>(endpoint, arguments));
                    try
                    {
                        if (!IsCurrent(player, generation) || !ReferenceEquals(binding, _local.OnlineAccount))
                        { accepted = Error("StaleResponse"); Fail(generation, "StaleResponse"); return false; }
                        accepted = ApplyResponse(endpoint, response, generation);
                        if (_completionAwaitingHandoff && accepted.status != "TransientFailure" &&
                            !await FinishCompletionAsync(player, generation, requiresEmptyPending))
                            accepted = Error(ViewState.Reason);
                        return accepted.status != "TransientFailure";
                    }
                    finally
                    {
                        if (response != null) { response.code = string.Empty; response.verificationValue = string.Empty; }
                    }
                }
                if (requiresEmptyPending)
                {
                    bool sent = await _pendingGate.TryRequestAsync(SendAsync);
                    if (!sent && accepted.reason == "LocalSaveUnavailable")
                    {
                        string reason = _local != null && _local.CreatePendingSnapshot().Count > 0
                            ? "PendingMustBeCleared" : "LocalSaveUnavailable";
                        Fail(generation, reason); accepted = Error(reason);
                    }
                }
                else await SendAsync();
                return accepted;
            }
            catch (TimeoutException) { Fail(generation, "Timeout"); return Error("Timeout"); }
            catch (Exception) { Fail(generation, "ServiceUnavailable"); return Error("ServiceUnavailable"); }
            finally { _isRequestRunning = false; }
        }

        private OnlineAccountResponse ApplyResponse(string endpoint, OnlineAccountResponse response, int generation)
        {
            if (response == null) { Fail(generation, "EmptyResponse"); return Error("EmptyResponse"); }
            if ((endpoint == "start-account-transfer" || endpoint == "reissue-account-transfer") && response.status == "Success")
            {
                string code = NormalizeCode(response.code);
                if (code == null || !IsDigits(response.verificationValue, 9) || response.expiresAtMilliseconds <= 0)
                { Fail(generation, "InvalidResponse"); return Error("InvalidResponse"); }
                ViewState.Set(E_OnlineAccountDisplayState.TransferPending);
                // Original values are returned only to this request's caller.
                return new OnlineAccountResponse { status = "Success", code = code.Insert(4, "-"),
                    verificationValue = response.verificationValue, expiresAtMilliseconds = response.expiresAtMilliseconds };
            }
            if ((response.status == "Success" || response.status == "AlreadyCompleted") &&
                (endpoint == "complete-account-transfer" || endpoint == "get-account-transfer-status"))
            {
                if (!PublicPlayerNumber.IsValid(response.publicPlayerNumber))
                { Fail(generation, "InvalidResponse"); return Error("InvalidResponse"); }
                _completionAwaitingHandoff = true;
                ViewState.Set(E_OnlineAccountDisplayState.Completed);
                return new OnlineAccountResponse { status = response.status, publicPlayerNumber = response.publicPlayerNumber };
            }
            if (response.status == "TransferPending")
            {
                if (endpoint == "get-account-transfer-status" && (!IsTransferId(response.transferId) ||
                    response.transferStatus != "TransferPending" || response.expiresAtMilliseconds <= 0))
                { Fail(generation, "InvalidResponse"); return Error("InvalidResponse"); }
                ViewState.Set(E_OnlineAccountDisplayState.TransferPending, transferId: response.transferId);
                return new OnlineAccountResponse { status = "TransferPending", transferStatus = "TransferPending",
                    transferId = ViewState.TransferId, expiresAtMilliseconds = response.expiresAtMilliseconds, credentialReissueRequired = true };
            }
            if (endpoint == "get-account-transfer-status" && response.status == "Inactive" &&
                response.transferStatus == "Completed" && IsTransferId(response.transferId))
            {
                _completionAwaitingHandoff = true;
                ViewState.Set(E_OnlineAccountDisplayState.Inactive, transferId: response.transferId);
                return new OnlineAccountResponse { status = "Inactive", transferStatus = "Completed", transferId = response.transferId };
            }
            if ((endpoint == "get-account-transfer-status" || endpoint == "cancel-account-transfer") &&
                response.status == "Active" && (response.transferStatus == "None" ||
                (IsTransferId(response.transferId) && (response.transferStatus == "Cancelled" ||
                response.transferStatus == "Expired" || response.transferStatus == "Completed"))))
            {
                if (response.transferStatus == "Completed") _completionAwaitingHandoff = true;
                ViewState.Set(response.transferStatus == "Completed" ? E_OnlineAccountDisplayState.Completed :
                    E_OnlineAccountDisplayState.NotRequested, transferId: response.transferId);
                return new OnlineAccountResponse { status = "Active", transferStatus = response.transferStatus, transferId = response.transferId };
            }
            string reason = response.status == "InvalidCredential" ? "InvalidCredential" :
                response.status == "TooManyRequests" ? "TooManyRequests" : "TransferUnavailable";
            Fail(generation, reason);
            return Error(reason);
        }

        private async Task<bool> FinishCompletionAsync(string player, int generation, bool ownsTransferGate)
        {
            bool isSource = ViewState.State == E_OnlineAccountDisplayState.Inactive;
            IOnlineAuthenticationSession session = _authentication as IOnlineAuthenticationSession;
            if (isSource && session == null) { Fail(generation, "SessionResetUnavailable"); return false; }
            string expectedNumber = string.Empty;
            if (!isSource)
            {
                OnlineAccountState beforeNumber = _local.OnlineAccount;
                OnlineAccountResponse number = await AwaitPanelRequestAsync(() => _transport.CallAsync<OnlineAccountResponse>(
                    "get-public-player-number", new Dictionary<string, string>()));
                if (!IsCurrent(player, generation) || !ReferenceEquals(beforeNumber, _local.OnlineAccount))
                { Fail(generation, "StaleResponse"); return false; }
                if (number == null || number.status != "Success" || !PublicPlayerNumber.IsValid(number.publicPlayerNumber))
                { Fail(generation, "PublicNumberUnavailable"); return false; }
                expectedNumber = number.publicPlayerNumber;
                if (beforeNumber.PublicNumberCache == expectedNumber)
                {
                    ViewState.Set(E_OnlineAccountDisplayState.Ready, number: expectedNumber);
                    _completionAwaitingHandoff = false;
                    return true;
                }
            }
            if (!_local.TryBeginAccountTransition(ownsTransferGate))
            { Fail(generation, "PendingMustBeCleared"); return false; }
            try
            {
                OnlineAccountState binding = _local.OnlineAccount;
                if (isSource)
                {
                    // Commit the blank binding BEFORE clearing SDK credentials.
                    // On a crash, an old token leads back to authenticated Inactive
                    // recovery; a cleared token can safely bind a new player.
                    if (!_local.TryApplyAccountTransition(binding, new OnlineAccountState(true),
                        new List<RecordSubmissionCandidate>()))
                    { Fail(generation, "LocalSaveUnavailable"); return false; }
                    _sessionToClear = player;
                    if (_onAccountChanged != null) _onAccountChanged();
                    if (!await AwaitPanelRequestAsync(() => session.TryClearSessionAsync(player)))
                    { Fail(generation, "SessionResetUnavailable"); return false; }
                    _sessionToClear = string.Empty;
                    if (generation != _generation) return false;
                    OnlineAccountState blank = _local.OnlineAccount;
                    OnlineAuthenticationResult fresh = await AwaitPanelRequestAsync(() => _authentication.TryAuthenticateAsync());
                    if (generation != _generation || !ReferenceEquals(blank, _local.OnlineAccount)) return false;
                    if (!fresh.IsAuthenticated || string.IsNullOrEmpty(fresh.PlayerId) || fresh.PlayerId == player ||
                        !_local.TrySaveOnlineAccount(new OnlineAccountState(true, fresh.PlayerId)))
                    { Fail(generation, "NewAuthenticationUnavailable"); return false; }
                    player = fresh.PlayerId;
                }
                else
                {
                    // Refresh both C's number and its whole best set under server
                    // authority. A failure is not an empty account.
                    OnlinePersonalBestSnapshot snapshot = await AwaitPanelRequestAsync(() => _transport.CallAsync<OnlinePersonalBestSnapshot>(
                        "get-account-personal-bests", new Dictionary<string, string>()));
                    if (!IsCurrent(player, generation) || !ReferenceEquals(binding, _local.OnlineAccount))
                    { Fail(generation, "StaleResponse"); return false; }
                    if (!TryReadSnapshot(snapshot, out List<RecordSubmissionCandidate> bests) ||
                        snapshot.publicPlayerNumber != expectedNumber)
                    { Fail(generation, "PersonalBestUnavailable"); return false; }
                    // The scoped number is committed together with the best set.
                    // Repeated Completed status after restart must not erase new
                    // offline Runs belonging to the already-applied connection.
                    if (binding.PublicNumberCache != snapshot.publicPlayerNumber)
                    {
                        if (!_local.TryApplyAccountTransition(binding,
                            new OnlineAccountState(true, player, snapshot.publicPlayerNumber), bests))
                        { Fail(generation, "LocalSaveUnavailable"); return false; }
                        if (_onAccountChanged != null) _onAccountChanged();
                    }
                }
                ViewState.Set(E_OnlineAccountDisplayState.Loading);
                if (!await FetchNumberAsync(player, generation)) return false;
                _completionAwaitingHandoff = false;
                return true;
            }
            finally { _local.EndAccountTransition(); }
        }

        private bool TryReadSnapshot(OnlinePersonalBestSnapshot snapshot, out List<RecordSubmissionCandidate> bests)
        {
            bests = new List<RecordSubmissionCandidate>();
            if (snapshot == null || snapshot.status != "Success" || !PublicPlayerNumber.IsValid(snapshot.publicPlayerNumber) ||
                snapshot.personalBests == null || snapshot.personalBests.Length > MaximumSnapshotBoards) return false;
            for (int i = 0; i < snapshot.personalBests.Length; i++)
            {
                OnlinePersonalBestEntry entry = snapshot.personalBests[i];
                if (entry == null || entry.score < 0 || entry.score > MaximumServerRankingValue || !IsTransferId(entry.submissionId)) return false;
                RecordBoardKey board;
                if (entry.boardId == "fs-stage-stage-001-r1")
                    RecordBoardKey.TryCreateStage("stage-001", 1, out board);
                else if (entry.boardId == "fs-infinite-v2" && entry.score <= int.MaxValue)
                    RecordBoardKey.TryCreateInfinite(2, out board);
                else return false;
                for (int j = 0; j < bests.Count; j++) if (bests[j].BoardKey.Equals(board)) return false;
                // Ranking cache only: missing Run components are not fabricated
                // or submitted. Only newly confirmed Runs enter Pending.
                bests.Add(new RecordSubmissionCandidate(_local.AccountId, entry.submissionId, board,
                    entry.score, 0, 0, 0, 0, 0, 0, 1));
            }
            return true;
        }

        private void Fail(int generation, string reason)
        {
            if (generation == _generation) ViewState.Set(E_OnlineAccountDisplayState.Error, reason);
        }

        private static OnlineAccountResponse Error(string reason)
        {
            return new OnlineAccountResponse { status = "TransientFailure", reason = reason };
        }

        private static bool IsTransferId(string id)
        {
            return Guid.TryParse(id, out Guid parsed) && parsed.ToString("D") == id && parsed.ToString("D")[14] == '4';
        }

        private static bool IsDigits(string value, int length)
        {
            if (value == null || value.Length != length) return false;
            for (int i = 0; i < value.Length; i++) if (value[i] < '0' || value[i] > '9') return false;
            return true;
        }

        private static string NormalizeCode(string value)
        {
            if (value == null) return null;
            string code = value.Trim().ToUpperInvariant();
            if (code.Length == 9 && code[4] == '-') code = code.Remove(4, 1);
            if (code.Length != 8) return null;
            code = code.Replace('O', '0').Replace('I', '1').Replace('L', '1');
            const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
            for (int i = 0; i < code.Length; i++) if (Alphabet.IndexOf(code[i]) < 0) return null;
            return code;
        }
    }
}
