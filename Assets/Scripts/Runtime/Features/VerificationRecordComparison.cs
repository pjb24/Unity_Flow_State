namespace FlowState.Runtime.Features
{
    public static class VerificationRecordComparison
    {
        // Compares only authenticated public me rows. It does not prove the
        // private fixed-owner UUID or server CAS atomicity.
        public static bool ArePreserved(VerificationRecordBaseline baseline, OnlineLeaderboardResult current, string number, string boardId)
        {
            if (baseline == null || baseline.projectId != OnlineRecordConfiguration.ProjectId ||
                baseline.environmentId != OnlineRecordConfiguration.EnvironmentId || baseline.boardId != boardId ||
                !PublicPlayerNumber.IsValid(number) || baseline.publicPlayerNumber != number ||
                !IsValidMe(baseline.me) || !IsValidMe(current) || baseline.me.entries.Length != current.entries.Length) return false;
            if (baseline.me.entries.Length == 0) return true;
            OnlineLeaderboardEntry before = baseline.me.entries[0], after = current.entries[0];
            if (before.publicPlayerNumber != number || after.publicPlayerNumber != number) return false;
            return before.publicPlayerNumber == after.publicPlayerNumber && before.score == after.score &&
                before.acceptedAt == after.acceptedAt;
        }
        public static bool IsValidMe(OnlineLeaderboardResult result)
        {
            if (result == null || !result.IsSuccess || result.entries == null || result.entries.Length > 1) return false;
            if (result.entries.Length == 0) return true;
            OnlineLeaderboardEntry entry = result.entries[0];
            return entry != null && entry.isMe && PublicPlayerNumber.IsValid(entry.publicPlayerNumber) &&
                entry.score >= 0 && entry.acceptedAt >= 0 && entry.rank > 0;
        }

        public static string GetPreservationFailureReason(VerificationRecordBaseline baseline,
            OnlineLeaderboardResult current, string number, string boardId)
        {
            if (ArePreserved(baseline, current, number, boardId)) return string.Empty;
            if (baseline == null) return "BASELINE_MISSING";
            if (baseline.projectId != OnlineRecordConfiguration.ProjectId ||
                baseline.environmentId != OnlineRecordConfiguration.EnvironmentId) return "SCOPE_MISMATCH";
            if (baseline.boardId != boardId) return "BOARD_MISMATCH";
            if (!PublicPlayerNumber.IsValid(number)) return "CURRENT_NUMBER_INVALID";
            if (baseline.publicPlayerNumber != number) return "PUBLIC_NUMBER_MISMATCH";
            if (!IsValidMe(baseline.me)) return "BASELINE_ME_INVALID";
            if (!IsValidMe(current)) return "CURRENT_ME_INVALID";
            if (baseline.me.entries.Length != current.entries.Length) return "ROW_COUNT_MISMATCH";
            if (baseline.me.entries.Length == 1)
            {
                OnlineLeaderboardEntry before = baseline.me.entries[0], after = current.entries[0];
                if (before.publicPlayerNumber != number || after.publicPlayerNumber != number) return "ROW_NUMBER_MISMATCH";
                if (before.score != after.score) return "SCORE_MISMATCH";
                if (before.acceptedAt != after.acceptedAt) return "ACCEPTED_AT_MISMATCH";
            }
            return "PRESERVATION_NOT_VERIFIED";
        }

        public static string GetStageProbeBlockReason(OnlineLeaderboardResult result, bool requiresEmpty)
        {
            if (result == null || !result.IsSuccess) return "STAGE_PROBE_QUERY_FAILED";
            if (!IsValidMe(result)) return "STAGE_PROBE_INVALID_ME";
            if (requiresEmpty && result.entries.Length != 0) return "STAGE_PROBE_REQUIRES_EMPTY_ME";
            return string.Empty;
        }

        public static bool IsSubmittedMe(OnlineLeaderboardResult result, string number, long expectedScore)
        {
            if (!PublicPlayerNumber.IsValid(number) || expectedScore < 0 || !IsValidMe(result) || result.entries.Length != 1)
                return false;
            OnlineLeaderboardEntry entry = result.entries[0];
            return entry.publicPlayerNumber == number && entry.score == expectedScore && entry.acceptedAt > 0;
        }
    }
}
