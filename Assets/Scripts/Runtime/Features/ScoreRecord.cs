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
    }
}
