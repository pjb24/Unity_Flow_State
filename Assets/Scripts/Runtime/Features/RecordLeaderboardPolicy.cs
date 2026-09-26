using System.Collections.Generic;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public static class RecordLeaderboardPolicy
    {
        public static bool TryCompare(
            RecordLeaderboardEntry first,
            RecordLeaderboardEntry second,
            out int comparison)
        {
            comparison = 0;

            if (!CanCompare(first, second))
            {
                return false;
            }

            comparison = CompareRankingValue(
                first.Candidate,
                second.Candidate);

            if (comparison != 0)
            {
                return true;
            }

            comparison = first.ServerAcceptedAtTicks.CompareTo(
                second.ServerAcceptedAtTicks);
            return true;
        }

        public static bool TryGetCompetitionRank(
            IList<RecordLeaderboardEntry> entries,
            RecordLeaderboardEntry target,
            out int rank)
        {
            rank = 0;

            if (entries == null || target == null || target.Candidate == null)
            {
                return false;
            }

            int betterEntryCount = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                RecordLeaderboardEntry current = entries[i];

                if (!TryCompare(current, target, out int comparison))
                {
                    return false;
                }

                if (comparison < 0)
                {
                    betterEntryCount++;
                }
            }

            rank = betterEntryCount + 1;
            return true;
        }

        public static bool ShouldReplaceBest(
            RecordSubmissionCandidate currentBest,
            RecordSubmissionCandidate candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            if (currentBest == null)
            {
                return true;
            }

            if (currentBest.PlayerId != candidate.PlayerId ||
                !currentBest.BoardKey.Equals(candidate.BoardKey))
            {
                return false;
            }

            return CompareRankingValue(candidate, currentBest) < 0;
        }

        private static bool CanCompare(
            RecordLeaderboardEntry first,
            RecordLeaderboardEntry second)
        {
            return first != null && second != null &&
                   first.Candidate != null && second.Candidate != null &&
                   first.Candidate.BoardKey.Equals(second.Candidate.BoardKey);
        }

        private static int CompareRankingValue(
            RecordSubmissionCandidate first,
            RecordSubmissionCandidate second)
        {
            return first.GameMode == E_GameMode.Stage
                ? first.RankingValue.CompareTo(second.RankingValue)
                : second.RankingValue.CompareTo(first.RankingValue);
        }
    }
}
