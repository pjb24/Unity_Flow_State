using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class InfiniteModeSystem : MonoBehaviour
    {
        [SerializeField] private RuntimeDataSystem _runtimeDataSystem;
        [SerializeField] private StageSystem _stageSystem;
        [SerializeField] private Transform _player;
        [SerializeField] private float _fallThresholdY = -3.0f;
        [SerializeField] private float _minimumHorizontalSpeed = 5.0f;
        [SerializeField] private float _startGraceDuration = 1.0f;
        [SerializeField] private float _belowSpeedGraceDuration = 0.5f;
        [SerializeField] private float _scorePerUnit = 10.0f;

        private readonly InfiniteModeState _state = new InfiniteModeState();
        private readonly InfiniteDistanceState _distanceState =
            new InfiniteDistanceState();
        private readonly ScoreCalculator _scoreCalculator =
            new ScoreCalculator();

        private PlayerMovementRuntimeData _movementRuntimeData;
        private InfiniteModeRuntimeData _infiniteModeRuntimeData;
        private bool _isInitialized;

        public bool IsPlaying => _state.IsPlaying;

        public bool HasEnded => _state.HasEnded;

        private void FixedUpdate()
        {
            if (!_isInitialized ||
                !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite)
            {
                return;
            }

            ProcessRunMetrics();
            ProcessProgress(Time.fixedDeltaTime);
            ProcessFallThreshold();
        }

        public bool Initialize(E_GameMode gameMode)
        {
            _isInitialized = false;
            _movementRuntimeData = null;
            ResetRunMetrics();

            if (!HasRequiredReferences())
            {
                return false;
            }

            GameRuntimeData runtimeData = _runtimeDataSystem.GetRuntimeData();

            if (runtimeData == null ||
                runtimeData.PlayerMovementRuntimeData == null)
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Player Movement Runtime Data does not exist.");
                return false;
            }

            if (!_state.Initialize(
                    _minimumHorizontalSpeed,
                    _startGraceDuration,
                    _belowSpeedGraceDuration) ||
                !_state.SetGameMode(gameMode))
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Infinite Mode settings are invalid.");
                return false;
            }

            if (gameMode == E_GameMode.Infinite &&
                !InitializeRunMetrics(runtimeData))
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Infinite Mode run metrics could not be initialized.");
                return false;
            }

            _movementRuntimeData = runtimeData.PlayerMovementRuntimeData;

            if (!_state.Start())
            {
                _movementRuntimeData = null;
                ResetRunMetrics();
                return false;
            }

            _isInitialized = true;
            return true;
        }

        public void Stop()
        {
            _state.Reset();
            _movementRuntimeData = null;
            ResetRunMetrics();
            _isInitialized = false;
        }

        private bool ProcessRunMetrics()
        {
            if (!_isInitialized ||
                !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite)
            {
                return false;
            }

            return UpdateRunMetrics();
        }

        private void ProcessProgress(float deltaTime)
        {
            if (_movementRuntimeData == null)
            {
                return;
            }

            if (_state.UpdateProgress(
                    _movementRuntimeData.CurrentHorizontalSpeed,
                    deltaTime))
            {
                FinalizeRunMetrics();
                _stageSystem.TryEndInfiniteStage();
            }
        }

        private void ProcessFallThreshold()
        {
            if (_player.position.y <= _fallThresholdY &&
                _state.NotifyFallThresholdReached())
            {
                FinalizeRunMetrics();
                _stageSystem.TryEndInfiniteStage();
            }
        }

        private bool InitializeRunMetrics(GameRuntimeData runtimeData)
        {
            if (runtimeData.InfiniteModeRuntimeData == null ||
                !_distanceState.Initialize(_player.position.x) ||
                !_scoreCalculator.Initialize(_scorePerUnit) ||
                !_scoreCalculator.TryCalculate(0.0f, out int initialScore) ||
                !runtimeData.InfiniteModeRuntimeData.TryUpdate(
                    0.0f,
                    initialScore))
            {
                ResetRunMetrics();
                return false;
            }

            _infiniteModeRuntimeData = runtimeData.InfiniteModeRuntimeData;
            return true;
        }

        private bool UpdateRunMetrics()
        {
            if (_infiniteModeRuntimeData == null ||
                !_distanceState.TryUpdate(_player.position.x) ||
                !_scoreCalculator.TryCalculate(
                    _distanceState.CurrentDistance,
                    out int currentScore) ||
                !_infiniteModeRuntimeData.TryUpdate(
                    _distanceState.CurrentDistance,
                    currentScore))
            {
                return false;
            }

            return true;
        }

        private void FinalizeRunMetrics()
        {
            if (!UpdateRunMetrics() ||
                !_distanceState.TryFinalize() ||
                !_infiniteModeRuntimeData.TryFinalize())
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Infinite Mode run metrics could not be finalized.");
            }
        }

        private void ResetRunMetrics()
        {
            _distanceState.Reset();
            _scoreCalculator.Reset();
            _infiniteModeRuntimeData = null;
        }

        private bool HasRequiredReferences()
        {
            if (_runtimeDataSystem == null ||
                _stageSystem == null ||
                _player == null)
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Required reference is missing.");
                return false;
            }

            return true;
        }
    }
}
