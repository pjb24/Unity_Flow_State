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
            int finalScore)
        {
            if (gameMode != E_GameMode.Infinite ||
                !hasStageEnded ||
                !isFinalized ||
                !IsFinite(finalDistance) ||
                finalDistance < 0.0f ||
                finalScore < 0 ||
                HasRecord)
            {
                return false;
            }

            _resultData = new ResultData(finalDistance, finalScore);
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
