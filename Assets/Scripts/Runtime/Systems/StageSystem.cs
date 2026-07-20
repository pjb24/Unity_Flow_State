using System;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class StageSystem : MonoBehaviour
    {
        [SerializeField] private StageGoal _stageGoal;

        private event Action StageStarted;
        private event Action StageCleared;
        private event Action StageEnded;

        private bool _isInitialized;
        private bool _isPlaying;
        private bool _isCleared;
        private bool _hasEnded;

        public bool IsInitialized => _isInitialized;

        public bool IsPlaying => _isPlaying;

        public bool IsCleared => _isCleared;

        public bool HasEnded => _hasEnded;

        private void OnDestroy()
        {
            if (_stageGoal != null)
            {
                _stageGoal.RemoveListener(HandleGoalReached);
            }
        }

        public bool Initialize()
        {
            if (_stageGoal == null)
            {
                _isInitialized = false;
                Debug.LogError("[StageSystem] Stage Goal is not assigned.");
                return false;
            }

            if (!_stageGoal.Initialize())
            {
                _isInitialized = false;
                return false;
            }

            _stageGoal.AddListener(HandleGoalReached);
            ResetStageState();
            _isInitialized = true;
            return true;
        }

        public bool StartStage()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[StageSystem] StageSystem is not initialized.");
                return false;
            }

            if (_isPlaying)
            {
                Debug.LogWarning("[StageSystem] Stage is already running.");
                return false;
            }

            ResetStageState();
            _stageGoal.ResetGoal();
            _isPlaying = true;
            StageStarted?.Invoke();
            return true;
        }

        public void StopStage()
        {
            if (!_isPlaying)
            {
                return;
            }

            EndStage();
        }

        public void AddStageStartedListener(Action listener)
        {
            StageStarted -= listener;
            StageStarted += listener;
        }

        public void RemoveStageStartedListener(Action listener)
        {
            StageStarted -= listener;
        }

        public void AddStageClearedListener(Action listener)
        {
            StageCleared -= listener;
            StageCleared += listener;
        }

        public void RemoveStageClearedListener(Action listener)
        {
            StageCleared -= listener;
        }

        public void AddStageEndedListener(Action listener)
        {
            StageEnded -= listener;
            StageEnded += listener;
        }

        public void RemoveStageEndedListener(Action listener)
        {
            StageEnded -= listener;
        }

        private void HandleGoalReached()
        {
            if (!_isPlaying || _isCleared || _hasEnded)
            {
                return;
            }

            _isCleared = true;
            StageCleared?.Invoke();
            EndStage();
        }

        private void EndStage()
        {
            if (_hasEnded)
            {
                return;
            }

            _isPlaying = false;
            _hasEnded = true;
            StageEnded?.Invoke();
        }

        private void ResetStageState()
        {
            _isPlaying = false;
            _isCleared = false;
            _hasEnded = false;
        }
    }
}
