using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public class TimeRecord
    {
        private ResultData _resultData;

        public bool HasRecord => _resultData != null;

        public ResultData ResultData => _resultData;

        public bool TryRecord(
            E_StageResultType stageResultType,
            double elapsedTime,
            int collectibleScore)
        {
            if ((stageResultType != E_StageResultType.Cleared &&
                 stageResultType != E_StageResultType.Fell) ||
                double.IsNaN(elapsedTime) ||
                double.IsInfinity(elapsedTime) ||
                elapsedTime < 0.0 ||
                collectibleScore < 0 ||
                HasRecord)
            {
                return false;
            }

            _resultData = new ResultData(
                stageResultType,
                elapsedTime,
                collectibleScore);
            return true;
        }

        public void Reset()
        {
            _resultData = null;
        }
    }
}
