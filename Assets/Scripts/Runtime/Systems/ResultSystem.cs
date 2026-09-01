using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class ResultSystem : MonoBehaviour
    {
        private readonly TimeRecord _timeRecord = new TimeRecord();
        private readonly ScoreRecord _scoreRecord = new ScoreRecord();

        private ResultData _currentResultData;

        public bool HasResultData => _currentResultData != null;

        public ResultData CurrentResultData => _currentResultData;

        public void Initialize()
        {
            _timeRecord.Reset();
            _scoreRecord.Reset();
            _currentResultData = null;
        }

        public bool CreateResultData(bool isStageCleared, double clearTime)
        {
            if (HasResultData ||
                !_timeRecord.TryRecord(isStageCleared, clearTime))
            {
                Debug.LogWarning(
                    "[ResultSystem] Result Data was not created.");
                return false;
            }

            _currentResultData = _timeRecord.ResultData;
            return true;
        }

        public bool CreateInfiniteResultData(
            E_GameMode gameMode,
            bool hasStageEnded,
            bool isFinalized,
            float finalDistance,
            int finalScore)
        {
            if (HasResultData ||
                !_scoreRecord.TryRecord(
                    gameMode,
                    hasStageEnded,
                    isFinalized,
                    finalDistance,
                    finalScore))
            {
                Debug.LogWarning(
                    "[ResultSystem] Infinite Result Data was not created.");
                return false;
            }

            _currentResultData = _scoreRecord.ResultData;
            return true;
        }
    }
}
