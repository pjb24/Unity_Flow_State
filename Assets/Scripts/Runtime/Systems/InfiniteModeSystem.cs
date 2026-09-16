using System;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    [DefaultExecutionOrder(0)]
    public class InfiniteModeSystem : MonoBehaviour
    {
        [SerializeField] private RuntimeDataSystem _runtimeDataSystem;
        [SerializeField] private StageSystem _stageSystem;
        [SerializeField] private CollisionSystem _collisionSystem;
        [SerializeField] private Transform _player;
        [SerializeField] private float _fallThresholdY = -3.0f;
        [SerializeField] private float _minimumHorizontalSpeed = 2.0f;
        [SerializeField] private float _startGraceDuration = 1.0f;
        [SerializeField] private float _belowSpeedGraceDuration = 0.5f;
        [SerializeField] private float _wallSpeedGraceDuration = 1.0f;
        [SerializeField] private float _scorePerUnit = 10.0f;

        private readonly InfiniteModeState _state = new InfiniteModeState();
        private readonly InfiniteDistanceState _distanceState =
            new InfiniteDistanceState();
        private readonly InfiniteScoreState _scoreState =
            new InfiniteScoreState();
        private readonly InfiniteDifficultyState _difficultyState =
            new InfiniteDifficultyState();
        private readonly InfinitePatternSelectionState _patternSelectionState =
            new InfinitePatternSelectionState();
        private readonly MomentumScoreState _momentumState =
            new MomentumScoreState();

        private GameRuntimeData _gameRuntimeData;
        private PlayerMovementRuntimeData _movementRuntimeData;
        private InfiniteModeRuntimeData _infiniteModeRuntimeData;
        private Rigidbody _playerRigidbody;
        private bool _isInitialized;
        private bool _isPaused;
        private int _previousRunSeed;
        private int _lastAcceptedPatternRequestId;
        private int _lastObservedPatternAdvanceCount;
        private bool _hasPreviousRunSeed;
        private bool _hasPendingPatternRequest;
        private long _lastProcessedMomentumSuccessId;

        public bool IsPlaying => _state.IsPlaying;

        public bool HasEnded => _state.HasEnded;

        public bool IsPaused => _isPaused;

        public double CurrentMomentumMultiplier => _momentumState.CurrentMultiplier;

        public double MaximumMomentumMultiplier =>
            _momentumState.MaximumReachedMultiplier;

        public double MomentumRemainingDuration => _momentumState.RemainingDuration;

        public double MomentumDuration => _momentumState.CurrentDuration;

        public double MomentumRemainingRatio => _momentumState.RemainingRatio;

        private void FixedUpdate()
        {
            if (!_isInitialized ||
                _isPaused ||
                !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite)
            {
                return;
            }

            ProcessRunMetrics();
            ProcessMomentumStep(Time.fixedDeltaTime);
            PublishRunMetrics();
            ProcessProgress(Time.fixedDeltaTime);
            ProcessFallThreshold();
            ProcessPatternProgression();
        }

        public bool Initialize(E_GameMode gameMode)
        {
            if (_isInitialized && _gameRuntimeData != null &&
                _gameRuntimeData.IsCreated && _gameRuntimeData.GameMode == gameMode &&
                _state.GameMode == gameMode && _runtimeDataSystem != null &&
                _runtimeDataSystem.RuntimeData == _gameRuntimeData)
            {
                return true;
            }

            _isInitialized = false;
            _isPaused = false;
            _movementRuntimeData = null;
            _gameRuntimeData = null;
            _momentumState.Reset();
            _lastProcessedMomentumSuccessId = 0;
            ResetRunMetrics();

            if (!HasRequiredReferences())
            {
                return false;
            }

            GameRuntimeData runtimeData = _runtimeDataSystem.GetRuntimeData();

            if (runtimeData == null ||
                runtimeData.PlayerMovementRuntimeData == null ||
                runtimeData.GameMode != gameMode)
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Runtime Data does not match the requested game mode.");
                return false;
            }

            if (!_state.Initialize(
                    _minimumHorizontalSpeed,
                    _startGraceDuration,
                    _belowSpeedGraceDuration,
                    _wallSpeedGraceDuration) ||
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

            if (gameMode == E_GameMode.Infinite &&
                !_momentumState.StartRun(ScoringVersion.Current))
            {
                ResetRunMetrics();
                return false;
            }

            _gameRuntimeData = runtimeData;
            _movementRuntimeData = runtimeData.PlayerMovementRuntimeData;

            if (!_state.Start())
            {
                _movementRuntimeData = null;
                _gameRuntimeData = null;
                _momentumState.Reset();
                ResetRunMetrics();
                return false;
            }

            if (gameMode == E_GameMode.Infinite &&
                !InitializePatternSelection())
            {
                _state.Reset();
                _movementRuntimeData = null;
                _gameRuntimeData = null;
                _momentumState.Reset();
                ResetRunMetrics();
                return false;
            }

            _isInitialized = true;
            return true;
        }

        public void Stop()
        {
            FinalizeMomentum();
            EndPatternSelection();
            _state.Reset();
            _gameRuntimeData = null;
            _movementRuntimeData = null;
            ResetRunMetrics();
            _isInitialized = false;
            _isPaused = false;
        }

        public bool Pause()
        {
            if (!_isInitialized ||
                _isPaused ||
                !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite)
            {
                return false;
            }

            if (!_momentumState.Pause())
            {
                return false;
            }

            _isPaused = true;
            _difficultyState.Pause();
            _patternSelectionState.Pause();
            return true;
        }

        public bool Resume()
        {
            if (!_isInitialized ||
                !_isPaused ||
                !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite)
            {
                return false;
            }

            if (!_momentumState.Resume())
            {
                return false;
            }

            _isPaused = false;
            _difficultyState.Resume();
            _patternSelectionState.Resume();
            return true;
        }

        private bool ProcessMomentumStep(double deltaTime)
        {
            if (!_isInitialized || _isPaused || !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite ||
                _gameRuntimeData == null || !_gameRuntimeData.IsCreated ||
                _gameRuntimeData.GameMode != E_GameMode.Infinite ||
                _gameRuntimeData.GameState != E_GameState.Playing ||
                _movementRuntimeData == null)
            {
                return false;
            }

            long successId = _movementRuntimeData.MomentumLandingSuccessId;
            bool hasSuccess = successId > _lastProcessedMomentumSuccessId;

            if (!_momentumState.TryStep(
                    _momentumState.ScoringVersion,
                    deltaTime,
                    hasSuccess,
                    hasSuccess ? successId : 0))
            {
                return false;
            }

            if (hasSuccess)
            {
                _lastProcessedMomentumSuccessId = successId;
            }

            return true;
        }

        private void FinalizeMomentum()
        {
            if (_momentumState.IsRunning && !_momentumState.IsFinalized)
            {
                _momentumState.FinalizeRun();
            }
        }

        private bool ProcessRunMetrics()
        {
            if (!_isInitialized ||
                _isPaused ||
                !_state.IsPlaying ||
                _state.GameMode != E_GameMode.Infinite)
            {
                return false;
            }

            return UpdateRunMetrics() && PublishRunMetrics();
        }

        private void ProcessProgress(float deltaTime)
        {
            if (_movementRuntimeData == null)
            {
                return;
            }

            if (_state.UpdateProgress(
                    Mathf.Max(0.0f, _playerRigidbody.linearVelocity.x),
                    _collisionSystem.GetCollisionState().WallContacts.HasWallContact,
                    deltaTime))
            {
                FinalizeRunMetrics();
                EndPatternSelection();
                _stageSystem.TryEndInfiniteStage();
            }
        }

        private void ProcessFallThreshold()
        {
            if (_playerRigidbody.position.y <= _fallThresholdY &&
                _state.NotifyFallThresholdReached())
            {
                FinalizeRunMetrics();
                EndPatternSelection();
                _stageSystem.TryEndInfiniteStage();
            }
        }

        private bool InitializePatternSelection()
        {
            if (!InfinitePatternCatalogFactory.TryCreate(
                    out InfinitePatternCatalog catalog) ||
                !_patternSelectionState.Initialize(catalog))
            {
                return false;
            }

            _difficultyState.Initialize();
            int seed = Guid.NewGuid().GetHashCode();

            if (_hasPreviousRunSeed && seed == _previousRunSeed)
            {
                seed = unchecked(seed + 1);
            }

            if (!_difficultyState.StartRun() ||
                !_patternSelectionState.StartRun(seed))
            {
                return false;
            }

            _previousRunSeed = seed;
            _hasPreviousRunSeed = true;
            _lastAcceptedPatternRequestId = 0;
            _lastObservedPatternAdvanceCount = 0;
            _hasPendingPatternRequest = false;
            return true;
        }

        private void ProcessPatternProgression()
        {
            if (!_state.IsPlaying || !_stageSystem.IsPlaying ||
                !_difficultyState.IsRunning ||
                !_patternSelectionState.IsRunning)
            {
                return;
            }

            if (!_difficultyState.TryUpdate(_distanceState.CurrentDistance))
            {
                return;
            }

            if (!_infiniteModeRuntimeData.TryUpdateDifficultyLevel(
                    (int)_difficultyState.CurrentDifficulty))
            {
                return;
            }

            InfiniteMapPattern mapPattern = _stageSystem.InfiniteMapPattern;

            if (mapPattern == null || !mapPattern.IsInitialized)
            {
                return;
            }

            if (mapPattern.AdvanceCount != _lastObservedPatternAdvanceCount)
            {
                _lastObservedPatternAdvanceCount = mapPattern.AdvanceCount;
                _hasPendingPatternRequest = false;
            }

            if (_hasPendingPatternRequest ||
                _lastAcceptedPatternRequestId == int.MaxValue)
            {
                return;
            }

            int requestId = _lastAcceptedPatternRequestId + 1;

            if (!_patternSelectionState.TrySelectNext(
                    requestId,
                    _difficultyState.CurrentDifficulty,
                    out string patternId))
            {
                return;
            }

            if (!mapPattern.TryRequestNextPattern(requestId, patternId))
            {
                _patternSelectionState.TryRevertLastSelection(requestId);
                return;
            }

            _lastAcceptedPatternRequestId = requestId;
            _hasPendingPatternRequest = true;
        }

        private void EndPatternSelection()
        {
            if (_difficultyState.IsRunning)
            {
                _difficultyState.EndRun();
            }

            if (_patternSelectionState.IsRunning)
            {
                _patternSelectionState.EndRun();
            }
        }

        private bool InitializeRunMetrics(GameRuntimeData runtimeData)
        {
            if (runtimeData.InfiniteModeRuntimeData == null ||
                runtimeData.InfiniteModeRuntimeData.ScoringVersion !=
                    ScoringVersion.Current ||
                !_distanceState.Initialize(_playerRigidbody.position.x) ||
                !_scoreState.Initialize(ScoringVersion.Current, _scorePerUnit))
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
                !_distanceState.TryUpdate(_playerRigidbody.position.x) ||
                !_scoreState.TryUpdate(
                    _infiniteModeRuntimeData.ScoringVersion,
                    _distanceState.CurrentDistance,
                    _momentumState.CurrentMultiplier))
            {
                return false;
            }

            return true;
        }

        private bool PublishRunMetrics()
        {
            if (_infiniteModeRuntimeData == null || _gameRuntimeData == null ||
                _gameRuntimeData.CollectibleRuntimeData == null ||
                !_scoreState.TryCalculateTotalScore(
                    _infiniteModeRuntimeData.ScoringVersion,
                    _gameRuntimeData.CollectibleRuntimeData.CurrentScore,
                    out int totalScore))
            {
                return false;
            }

            return _infiniteModeRuntimeData.TryUpdate(
                _scoreState.ScoringVersion,
                _distanceState.CurrentDistance,
                _scoreState.BaseDistanceScore,
                _scoreState.MomentumBonus,
                _scoreState.DistanceScore,
                _gameRuntimeData.CollectibleRuntimeData.CurrentScore,
                totalScore,
                _momentumState.CurrentMultiplier,
                _momentumState.MaximumReachedMultiplier,
                _momentumState.RemainingDuration,
                _momentumState.CurrentDuration);
        }

        private void FinalizeRunMetrics()
        {
            if (!UpdateRunMetrics() ||
                !PublishRunMetrics() ||
                !_distanceState.TryFinalize() ||
                !_scoreState.TryFinalize(
                    _infiniteModeRuntimeData.ScoringVersion) ||
                !FinalizeMomentumState() ||
                !_infiniteModeRuntimeData.TryFinalize())
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Infinite Mode run metrics could not be finalized.");
            }
        }

        private void ResetRunMetrics()
        {
            _distanceState.Reset();
            _scoreState.Reset();
            _infiniteModeRuntimeData = null;
        }

        private bool FinalizeMomentumState()
        {
            if (!_momentumState.IsRunning)
            {
                return false;
            }

            return _momentumState.IsFinalized || _momentumState.FinalizeRun();
        }

        private bool HasRequiredReferences()
        {
            if (_runtimeDataSystem == null ||
                _stageSystem == null ||
                _collisionSystem == null ||
                _player == null)
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Required reference is missing.");
                return false;
            }

            if (!_player.TryGetComponent(out _playerRigidbody))
            {
                Debug.LogError(
                    "[InfiniteModeSystem] Player Rigidbody does not exist.");
                return false;
            }

            return true;
        }
    }
}
