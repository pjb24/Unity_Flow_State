using System;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public static class RecordSubmissionPolicy
    {
        public static bool TryCreateStageCandidate(
            string playerId,
            string submissionId,
            string stageId,
            int stageRulesVersion,
            E_StageResultType stageResultType,
            double elapsedTime,
            out RecordSubmissionCandidate candidate)
        {
            candidate = null;

            if (!HasIdentity(playerId, submissionId) ||
                stageResultType != E_StageResultType.Cleared ||
                !TryConvertToMilliseconds(elapsedTime, out long clearTimeMilliseconds) ||
                !RecordBoardKey.TryCreateStage(
                    stageId,
                    stageRulesVersion,
                    out RecordBoardKey boardKey))
            {
                return false;
            }

            candidate = new RecordSubmissionCandidate(
                playerId,
                submissionId,
                boardKey,
                clearTimeMilliseconds,
                0,
                0,
                0,
                0,
                0,
                0,
                1.0);
            return true;
        }

        public static bool TryCreateInfiniteCandidate(
            string playerId,
            string submissionId,
            int scoringVersion,
            long runDurationMilliseconds,
            int baseDistanceScore,
            int momentumBonus,
            int distanceScore,
            int collectibleScore,
            int totalScore,
            double maximumMomentumMultiplier,
            InfiniteScoreLimit scoreLimit,
            out RecordSubmissionCandidate candidate)
        {
            candidate = null;

            if (!HasIdentity(playerId, submissionId) ||
                scoringVersion != ScoringVersion.Current ||
                scoreLimit == null ||
                scoreLimit.ScoringVersion != scoringVersion ||
                runDurationMilliseconds < 0 ||
                baseDistanceScore < 0 ||
                momentumBonus < 0 ||
                distanceScore < 0 ||
                collectibleScore < 0 ||
                totalScore < 0 ||
                !IsFinite(maximumMomentumMultiplier) ||
                maximumMomentumMultiplier < 1.0 ||
                maximumMomentumMultiplier > scoreLimit.MaximumMomentumMultiplier ||
                !TryCalculateSaturatedSum(
                    baseDistanceScore,
                    momentumBonus,
                    out int expectedDistanceScore) ||
                distanceScore != expectedDistanceScore ||
                !TryCalculateSaturatedSum(
                    distanceScore,
                    collectibleScore,
                    out int expectedTotalScore) ||
                totalScore != expectedTotalScore ||
                !scoreLimit.TryCalculateMaximumTotalScore(
                    runDurationMilliseconds,
                    out int maximumTotalScore) ||
                totalScore > maximumTotalScore ||
                !RecordBoardKey.TryCreateInfinite(
                    scoringVersion,
                    out RecordBoardKey boardKey))
            {
                return false;
            }

            candidate = new RecordSubmissionCandidate(
                playerId,
                submissionId,
                boardKey,
                totalScore,
                runDurationMilliseconds,
                baseDistanceScore,
                momentumBonus,
                distanceScore,
                collectibleScore,
                totalScore,
                maximumMomentumMultiplier);
            return true;
        }

        public static bool TryCalculateSaturatedSum(
            int first,
            int second,
            out int sum)
        {
            sum = 0;

            if (first < 0 || second < 0)
            {
                return false;
            }

            sum = first > int.MaxValue - second
                ? int.MaxValue
                : first + second;
            return true;
        }

        private static bool HasIdentity(string playerId, string submissionId)
        {
            if (string.IsNullOrEmpty(playerId) ||
                !Guid.TryParse(submissionId, out Guid submissionGuid))
            {
                return false;
            }

            return submissionGuid.ToString("D")[14] == '4';
        }

        private static bool TryConvertToMilliseconds(
            double elapsedTime,
            out long clearTimeMilliseconds)
        {
            clearTimeMilliseconds = 0;

            if (!IsFinite(elapsedTime) || elapsedTime < 0.0 ||
                elapsedTime > long.MaxValue / 1000.0)
            {
                return false;
            }

            clearTimeMilliseconds = (long)Math.Round(
                elapsedTime * 1000.0,
                MidpointRounding.AwayFromZero);
            return true;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
