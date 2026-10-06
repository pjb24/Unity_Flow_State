using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    // Verification-only explicit replay. Never creates a candidate or retry loop.
    public static class VerificationPendingRetry
    {
        public static async Task<string> RetryOnceAsync(LocalRecordRepository local, IOnlineRecordRepository online)
        {
            if (local == null || online == null || !local.TryCheckpoint() || !local.CanUseOnlineData ||
                !local.OnlineAccount.HasConfirmedRecoveryNotice) return "PENDING_LOCAL_NOT_READY";
            var pending = local.CreatePendingSnapshot();
            if (pending.Count != 1) return "PENDING_REQUIRES_EXACTLY_ONE";
            RecordSubmissionCandidate candidate = pending[0];
            if (candidate == null || candidate.PlayerId != local.AccountId) return "PENDING_OWNER_MISMATCH";
            OnlineAccountState binding = local.OnlineAccount;
            OnlineSubmissionResult result = await online.SubmitAsync(candidate);
            if (!local.CanUseOnlineData || !ReferenceEquals(binding, local.OnlineAccount)) return "PENDING_STALE_RESULT";
            if (result == null || result.Result != E_RecordSubmissionResult.Submitted)
                return "PENDING_NOT_CONFIRMED";
            return local.TryRemovePending(candidate.PlayerId, candidate.SubmissionId) ?
                "PENDING_SUBMISSION_CONFIRMED" : "PENDING_LOCAL_COMMIT_FAILED";
        }
    }
}
