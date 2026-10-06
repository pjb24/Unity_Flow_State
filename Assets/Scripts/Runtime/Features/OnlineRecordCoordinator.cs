using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class OnlineRecordCoordinator
    {
        private const int InitialRetryDelayMilliseconds = 1000;
        private readonly string _owner;
        private readonly LocalRecordRepository _local;
        private readonly IOnlineAuthenticationGateway _authentication;
        private readonly IOnlineRecordRepository _online;
        private readonly Func<int, Task> _delay;
        private readonly List<RuntimeSubmissionReceipt> _terminalReceipts =
            new List<RuntimeSubmissionReceipt>();
        private readonly System.Threading.SemaphoreSlim _retryGate =
            new System.Threading.SemaphoreSlim(1, 1);

        public OnlineRecordCoordinator(string owner, LocalRecordRepository local,
            IOnlineAuthenticationGateway authentication, IOnlineRecordRepository online,
            Func<int, Task> delay = null)
        {
            _owner = owner;
            _local = local;
            _authentication = authentication;
            _online = online;
            _delay = delay == null ? milliseconds => Task.Delay(milliseconds) : delay;
        }

        public int SubmittedCount => Count(E_RecordSubmissionResult.Submitted);
        public int RejectedCount => Count(E_RecordSubmissionResult.Rejected);

        public void ClearSessionHistory()
        {
            _terminalReceipts.Clear();
        }

        public bool TryGetTerminalResult(string submissionId, out OnlineSubmissionResult result)
        {
            result = null;
            for (int i = 0; i < _terminalReceipts.Count; i++)
            {
                if (_terminalReceipts[i].SubmissionId != submissionId) continue;
                result = _terminalReceipts[i].Result;
                return true;
            }
            return false;
        }

        // Must only be called after the human has explicitly acknowledged the recovery notice.
        public async Task<bool> ConfirmRecoveryNoticeAsync()
        {
            if (string.IsNullOrEmpty(_owner) || _local.AccountId != _owner ||
                !_local.TryCheckpoint() || !_local.CanUseOnlineData) return false;
            OnlineAccountState state = _local.OnlineAccount;
            if (!state.HasConfirmedRecoveryNotice &&
                !_local.TrySaveOnlineAccount(new OnlineAccountState(true))) return false;
            try { return await BindAsync(); }
            catch (Exception)
            {
                Debug.LogWarning("[OnlineRecordCoordinator] Authentication unavailable; consent retained.");
                return false;
            }
        }

        private async Task<bool> BindAsync()
        {
            if (!_local.CanUseOnlineData || string.IsNullOrEmpty(_owner) || _local.AccountId != _owner ||
                !_local.OnlineAccount.HasConfirmedRecoveryNotice) return false;
            OnlineAuthenticationResult result = await _authentication.TryAuthenticateAsync();
            if (!_local.CanUseOnlineData || !result.IsAuthenticated || string.IsNullOrEmpty(result.PlayerId)) return false;
            OnlineAccountState state = _local.OnlineAccount;
            if (!string.IsNullOrEmpty(state.PlayerId)) return state.PlayerId == result.PlayerId;
            return _local.TrySaveOnlineAccount(new OnlineAccountState(true, result.PlayerId));
        }

        // App start, connectivity restored, explicit retry and post-consent authentication.
        // Concurrent triggers coalesce; no timer can start an unbounded retry loop.
        public async Task RetryPendingAsync()
        {
            if (!_local.OnlineAccount.HasConfirmedRecoveryNotice ||
                !await _retryGate.WaitAsync(0)) return;
            try
            {
                await ProcessPendingAsync(null);
            }
            finally { _retryGate.Release(); }
        }

        // A new Result must wait for its own candidate even when recovery is already running.
        public async Task RetrySubmissionAsync(string submissionId)
        {
            if (string.IsNullOrEmpty(submissionId) ||
                !_local.OnlineAccount.HasConfirmedRecoveryNotice) return;
            await _retryGate.WaitAsync();
            try
            {
                await ProcessPendingAsync(submissionId);
            }
            finally { _retryGate.Release(); }
        }

        // User-requested retry of every saved Pending candidate, including older Runs.
        // Unlike background triggers this waits for any retry already in progress.
        public async Task RetryAllPendingAsync()
        {
            if (!_local.OnlineAccount.HasConfirmedRecoveryNotice) return;
            await _retryGate.WaitAsync();
            try
            {
                await ProcessPendingAsync(null, true);
            }
            finally { _retryGate.Release(); }
        }

        private async Task ProcessPendingAsync(string submissionId,
            bool continueAfterTransient = false)
        {
            try
            {
                if (!_local.TryCheckpoint() || !await BindAsync()) return;
                var pending = _local.CreatePendingSnapshot();
                for (int i = 0; i < pending.Count; i++)
                {
                    RecordSubmissionCandidate candidate = pending[i];
                    if (candidate.PlayerId != _owner ||
                        (submissionId != null && candidate.SubmissionId != submissionId)) continue;
                    for (int attempt = 0; attempt < RecordSubmissionQueue.MaximumAttemptsPerRetryTrigger; attempt++)
                    {
                        if (attempt > 0) await _delay(InitialRetryDelayMilliseconds << (attempt - 1));
                        OnlineSubmissionResult result = await _online.SubmitAsync(candidate);
                        if (result == null || (result.Result != E_RecordSubmissionResult.Submitted &&
                            result.Result != E_RecordSubmissionResult.Rejected)) continue;
                        if (!_local.TryRemovePending(candidate.PlayerId, candidate.SubmissionId)) return;
                        RememberTerminalResult(candidate.SubmissionId, result);
                        break;
                    }
                    if (ContainsPending(candidate.SubmissionId) && !continueAfterTransient) return;
                }
            }
            catch (TimeoutException)
            {
                // A bounded explicit retry ends here; unconfirmed records remain Pending.
            }
            catch (Exception)
            {
                Debug.LogWarning("[OnlineRecordCoordinator] Retry deferred; local records retained.");
            }
        }

        private bool ContainsPending(string submissionId)
        {
            var pending = _local.CreatePendingSnapshot();
            for (int i = 0; i < pending.Count; i++)
                if (pending[i].SubmissionId == submissionId) return true;
            return false;
        }

        private void RememberTerminalResult(string submissionId, OnlineSubmissionResult result)
        {
            for (int i = 0; i < _terminalReceipts.Count; i++)
                if (_terminalReceipts[i].SubmissionId == submissionId) return;
            _terminalReceipts.Add(new RuntimeSubmissionReceipt(submissionId, result));
        }

        private int Count(E_RecordSubmissionResult expected)
        {
            int count = 0;
            for (int i = 0; i < _terminalReceipts.Count; i++)
                if (_terminalReceipts[i].Result.Result == expected) count++;
            return count;
        }

        private sealed class RuntimeSubmissionReceipt
        {
            public string SubmissionId { get; }
            public OnlineSubmissionResult Result { get; }

            public RuntimeSubmissionReceipt(string submissionId, OnlineSubmissionResult result)
            {
                SubmissionId = submissionId;
                Result = result;
            }
        }
    }
}
