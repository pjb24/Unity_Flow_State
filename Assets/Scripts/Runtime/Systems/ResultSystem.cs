using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class ResultSystem : MonoBehaviour
    {
        private readonly TimeRecord _timeRecord = new TimeRecord();

        public bool HasResultData => _timeRecord.HasRecord;

        public ResultData CurrentResultData => _timeRecord.ResultData;

        public void Initialize()
        {
            _timeRecord.Reset();
        }

        public bool CreateResultData(bool isStageCleared, double clearTime)
        {
            if (!_timeRecord.TryRecord(isStageCleared, clearTime))
            {
                Debug.LogWarning(
                    "[ResultSystem] Result Data was not created.");
                return false;
            }

            return true;
        }
    }
}
