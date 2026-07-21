using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public class TimeRecord
    {
        private ResultData _resultData;

        public bool HasRecord => _resultData != null;

        public ResultData ResultData => _resultData;

        public bool TryRecord(bool isStageCleared, double clearTime)
        {
            if (!isStageCleared || HasRecord)
            {
                return false;
            }

            _resultData = new ResultData(true, clearTime);
            return true;
        }

        public void Reset()
        {
            _resultData = null;
        }
    }
}
