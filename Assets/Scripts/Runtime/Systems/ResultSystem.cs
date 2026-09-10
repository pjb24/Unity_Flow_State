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

        public bool CreateStageResultData(
            E_StageResultType stageResultType,
            double elapsedTime,
            int collectibleScore)
        {
            if (HasResultData ||
                !_timeRecord.TryRecord(
                    stageResultType,
                    elapsedTime,
                    collectibleScore))
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
            int distanceScore,
            int collectibleScore)
        {
            if (HasResultData ||
                !_scoreRecord.TryRecord(
                    gameMode,
                    hasStageEnded,
                    isFinalized,
                    finalDistance,
                    distanceScore,
                    collectibleScore))
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
