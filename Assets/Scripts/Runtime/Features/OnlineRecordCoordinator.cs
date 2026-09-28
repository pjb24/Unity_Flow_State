using System;
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
        private bool _running;

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

        // Must only be called after the human has explicitly acknowledged the recovery notice.
        public async Task<bool> ConfirmRecoveryNoticeAsync()
        {
            if (string.IsNullOrEmpty(_owner) || _local.AccountId != _owner) return false;
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
            if (string.IsNullOrEmpty(_owner) || _local.AccountId != _owner ||
                !_local.OnlineAccount.HasConfirmedRecoveryNotice) return false;
            OnlineAuthenticationResult result = await _authentication.TryAuthenticateAsync();
            if (!result.IsAuthenticated || string.IsNullOrEmpty(result.PlayerId)) return false;
            OnlineAccountState state = _local.OnlineAccount;
            if (!string.IsNullOrEmpty(state.PlayerId)) return state.PlayerId == result.PlayerId;
            return _local.TrySaveOnlineAccount(new OnlineAccountState(true, result.PlayerId,
                state.SubmittedIds, state.RejectedIds));
        }

        // App start, connectivity restored, explicit retry and post-consent authentication.
        // Concurrent triggers coalesce; no timer can start an unbounded retry loop.
        public async Task RetryPendingAsync()
        {
            if (_running || !_local.OnlineAccount.HasConfirmedRecoveryNotice) return;
            _running = true;
            try
            {
                if (!await BindAsync() || !_local.TryCheckpoint()) return;
                var pending = _local.CreatePendingSnapshot();
                for (int i = 0; i < pending.Count; i++)
                {
                    RecordSubmissionCandidate candidate = pending[i];
                    if (candidate.PlayerId != _owner) continue;
                    for (int attempt = 0; attempt < RecordSubmissionQueue.MaximumAttemptsPerRetryTrigger; attempt++)
                    {
                        if (attempt > 0) await _delay(InitialRetryDelayMilliseconds << (attempt - 1));
                        E_RecordSubmissionResult result = await _online.SubmitAsync(candidate);
                        if (result == E_RecordSubmissionResult.TransientFailure) continue;
                        // Persist the receipt before dropping Pending. A disk failure safely replays the same ID.
                        if (!_local.TrySaveOnlineAccount(_local.OnlineAccount.WithResult(candidate.SubmissionId, result)))
                            return;
                        break;
                    }
                    // Preserve order when a prior journal operation remains in doubt.
                    if (!_local.OnlineAccount.HasFinished(candidate.SubmissionId)) return;
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("[OnlineRecordCoordinator] Retry deferred; local records retained.");
            }
            finally { _running = false; }
        }
    }
}
