using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public class ScoreRecord
    {
        private ResultData _resultData;

        public bool HasRecord => _resultData != null;

        public ResultData ResultData => _resultData;

        public bool TryRecord(
            E_GameMode gameMode,
            bool hasStageEnded,
            bool isFinalized,
            float finalDistance,
            int distanceScore,
            int collectibleScore)
        {
            if (gameMode != E_GameMode.Infinite ||
                !hasStageEnded ||
                !isFinalized ||
                !IsFinite(finalDistance) ||
                finalDistance < 0.0f ||
                !TryCalculateTotalScore(
                    distanceScore,
                    collectibleScore,
                    out int totalScore) ||
                HasRecord)
            {
                return false;
            }

            _resultData = new ResultData(
                finalDistance,
                distanceScore,
                collectibleScore,
                totalScore);
            return true;
        }

        public bool TryRecord(
            int scoringVersion,
            E_GameMode gameMode,
            bool hasStageEnded,
            bool isFinalized,
            float finalDistance,
            int baseDistanceScore,
            int momentumBonus,
            int distanceScore,
            int collectibleScore,
            double maximumMomentumMultiplier)
        {
            if (scoringVersion != ScoringVersion.Current ||
                gameMode != E_GameMode.Infinite ||
                !hasStageEnded ||
                !isFinalized ||
                !IsFinite(finalDistance) ||
                finalDistance < 0.0f ||
                baseDistanceScore < 0 ||
                momentumBonus < 0 ||
                distanceScore < 0 ||
                !TryCalculateTotalScore(
                    baseDistanceScore,
                    momentumBonus,
                    out int expectedDistanceScore) ||
                distanceScore != expectedDistanceScore ||
                !TryCalculateTotalScore(
                    distanceScore,
                    collectibleScore,
                    out int totalScore) ||
                !IsValidMaximumMultiplier(maximumMomentumMultiplier) ||
                HasRecord)
            {
                return false;
            }

            _resultData = new ResultData(
                scoringVersion,
                finalDistance,
                baseDistanceScore,
                momentumBonus,
                distanceScore,
                collectibleScore,
                totalScore,
                maximumMomentumMultiplier);
            return true;
        }

        public static bool TryCalculateTotalScore(
            int distanceScore,
            int collectibleScore,
            out int totalScore)
        {
            totalScore = 0;

            if (distanceScore < 0 || collectibleScore < 0)
            {
                return false;
            }

            totalScore = distanceScore > int.MaxValue - collectibleScore
                ? int.MaxValue
                : distanceScore + collectibleScore;
            return true;
        }

        public void Reset()
        {
            _resultData = null;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static bool IsValidMaximumMultiplier(double multiplier)
        {
            if (double.IsNaN(multiplier) ||
                double.IsInfinity(multiplier) ||
                multiplier < MomentumScoreState.BaseMultiplier ||
                multiplier > MomentumScoreState.MaximumMultiplier)
            {
                return false;
            }

            double level =
                (multiplier - MomentumScoreState.BaseMultiplier) /
                MomentumScoreState.MultiplierStep;
            return System.Math.Abs(level - System.Math.Round(level)) <=
                   0.0000001;
        }
    }
}
