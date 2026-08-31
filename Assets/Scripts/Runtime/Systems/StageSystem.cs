using System;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class StageSystem : MonoBehaviour
    {
        [SerializeField] private StageGoal _stageGoal;
        [SerializeField] private GameObject _stageModeRoot;
        [SerializeField] private GameObject _infiniteModeRoot;

        private event Action StageStarted;
        private event Action StageCleared;
        private event Action StageEnded;

        private E_GameMode _currentGameMode;
        private bool _isInitialized;
        private bool _isPlaying;
        private bool _isCleared;
        private bool _hasEnded;

        public bool IsInitialized => _isInitialized;

        public E_GameMode CurrentGameMode => _currentGameMode;

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
            return Initialize(E_GameMode.Stage);
        }

        public bool Initialize(E_GameMode gameMode)
        {
            if (_stageGoal != null)
            {
                _stageGoal.RemoveListener(HandleGoalReached);
            }

            _isInitialized = false;

            if (gameMode != E_GameMode.Stage &&
                gameMode != E_GameMode.Infinite)
            {
                Debug.LogError("[StageSystem] Game Mode is invalid.");
                return false;
            }

            _currentGameMode = gameMode;

            if (!ApplyModeRootState())
            {
                return false;
            }

            if (_currentGameMode == E_GameMode.Stage &&
                !InitializeStageGoal())
            {
                return false;
            }

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

            if (_currentGameMode == E_GameMode.Stage)
            {
                _stageGoal.ResetGoal();
            }

            _isPlaying = true;

            if (StageStarted != null)
            {
                StageStarted();
            }

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

        public bool TryEndInfiniteStage()
        {
            if (!_isInitialized ||
                _currentGameMode != E_GameMode.Infinite ||
                !_isPlaying ||
                _hasEnded)
            {
                return false;
            }

            EndStage();
            return true;
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
            if (_currentGameMode != E_GameMode.Stage ||
                !_isPlaying ||
                _isCleared ||
                _hasEnded)
            {
                return;
            }

            _isCleared = true;

            if (StageCleared != null)
            {
                StageCleared();
            }

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

            if (StageEnded != null)
            {
                StageEnded();
            }
        }

        private void ResetStageState()
        {
            _isPlaying = false;
            _isCleared = false;
            _hasEnded = false;
        }

        private bool InitializeStageGoal()
        {
            if (_stageGoal == null)
            {
                Debug.LogError(
                    "[StageSystem] Stage Goal is not assigned for Stage Mode.");
                return false;
            }

            if (!_stageGoal.Initialize())
            {
                return false;
            }

            _stageGoal.AddListener(HandleGoalReached);
            return true;
        }

        private bool ApplyModeRootState()
        {
            if (_stageModeRoot == null && _infiniteModeRoot == null)
            {
                return true;
            }

            if (_stageModeRoot == null || _infiniteModeRoot == null)
            {
                Debug.LogError(
                    "[StageSystem] Both Mode Roots must be assigned together.");
                return false;
            }

            bool isStageMode = _currentGameMode == E_GameMode.Stage;
            _stageModeRoot.SetActive(isStageMode);

            if (isStageMode)
            {
                _infiniteModeRoot.SetActive(false);
                return true;
            }

            if (_infiniteModeRoot.activeSelf)
            {
                _infiniteModeRoot.SetActive(false);
            }

            _infiniteModeRoot.SetActive(true);
            return true;
        }
    }
}
