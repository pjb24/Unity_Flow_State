using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class GameSystem : MonoBehaviour
    {
        [SerializeField] private E_GameMode _selectedGameMode = E_GameMode.Stage;
        [SerializeField] private RuntimeDataSystem _runtimeDataSystem;
        [SerializeField] private UIManagementSystem _uiManagementSystem;
        [SerializeField] private PlayerInputSystem _playerInputSystem;
        [SerializeField] private UIInputSystem _uiInputSystem;
        [SerializeField] private PlayerMovementSystem _playerMovementSystem;
        [SerializeField] private PlayerControllerSystem _playerControllerSystem;
        [SerializeField] private CollisionSystem _collisionSystem;
        [SerializeField] private StageSystem _stageSystem;
        [SerializeField] private InfiniteModeSystem _infiniteModeSystem;
        [SerializeField] private TimerSystem _timerSystem;
        [SerializeField] private ResultSystem _resultSystem;
        [SerializeField] private CameraSystem _cameraSystem;
        [SerializeField] private CameraFollow _cameraFollow;

        private GameRuntimeData _runtimeData;
        private E_GameState _currentGameState;

        public E_GameState CurrentGameState => _currentGameState;

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (_currentGameState != E_GameState.Ended)
            {
                return;
            }

            UIInputState inputState = _uiInputSystem.GetInputState();
            bool isPointerOverResultMenu = false;

            if (inputState.IsPointChanged || inputState.IsClickPressed)
            {
                isPointerOverResultMenu =
                    _uiManagementSystem.TrySetResultMenuSelectionAtPointer(
                        inputState.PointerPosition);
            }

            if (Mathf.Abs(inputState.NavigateInput.y) >= 0.5f)
            {
                _uiManagementSystem.MoveResultMenuSelection(
                    inputState.NavigateInput.y);
            }

            bool shouldExecuteSelection = inputState.IsSubmitPressed ||
                                          (inputState.IsClickPressed &&
                                           isPointerOverResultMenu);

            _uiInputSystem.ConsumeTransientInput();

            if (shouldExecuteSelection)
            {
                ExecuteResultMenuSelection();
            }
        }

        private void OnDisable()
        {
            if (_stageSystem != null)
            {
                _stageSystem.RemoveStageEndedListener(HandleStageEnded);
            }
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            if (!HasRequiredSystems())
            {
                return;
            }

            if (_currentGameState == E_GameState.Playing)
            {
                Debug.LogWarning("[GameSystem] Game is already running.");
                return;
            }

            SetGameState(E_GameState.Initializing);

            _runtimeData = _runtimeDataSystem.CreateRuntimeData(
                _selectedGameMode);
            _runtimeData.SetGameState(_currentGameState);
            _runtimeData.PlayerMovementRuntimeData.Initialize();

            _uiManagementSystem.Initialize();
            _resultSystem.Initialize();
            SetUIState(E_UIState.None);

            if (!_playerControllerSystem.Initialize() ||
                !_collisionSystem.Initialize() ||
                !_stageSystem.Initialize(_selectedGameMode) ||
                !_playerMovementSystem.Initialize() ||
                !_infiniteModeSystem.Initialize(_selectedGameMode) ||
                !_cameraSystem.Initialize())
            {
                AbortGameStart();
                return;
            }

            _playerInputSystem.Initialize();
            _uiInputSystem.Initialize();
            _uiInputSystem.DisableUIActionMap();

            SetGameState(E_GameState.Ready);
            SetUIState(E_UIState.StageHud);

            _stageSystem.AddStageEndedListener(HandleStageEnded);

            if (!_stageSystem.StartStage())
            {
                AbortGameStart();
                return;
            }

            if (!_timerSystem.CreateTimer(E_TimerKey.PlayTimer) ||
                !_timerSystem.StartTimer(E_TimerKey.PlayTimer))
            {
                AbortGameStart();
                return;
            }

            _playerInputSystem.EnablePlayerActionMap();
            _cameraFollow.StartFollowing();
            SetGameState(E_GameState.Playing);

            Debug.Log("[GameSystem] Game started.");
        }

        [ContextMenu("End Game")]
        public void EndGame()
        {
            if (!HasRequiredSystems())
            {
                return;
            }

            if (_currentGameState == E_GameState.Ending ||
                _currentGameState == E_GameState.Ended)
            {
                Debug.LogWarning("[GameSystem] Game is already ending or ended.");
                return;
            }

            if (_runtimeData == null || !_runtimeDataSystem.HasRuntimeData)
            {
                Debug.LogWarning("[GameSystem] Runtime Data does not exist.");
                return;
            }

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);

            StopPlayTimer();
            _stageSystem.StopStage();
            _playerInputSystem.DisablePlayerActionMap();
            _uiInputSystem.EnableUIActionMap();
            _cameraFollow.StopFollowing();
            _infiniteModeSystem.Stop();
            _playerMovementSystem.StopMovement();
            RemovePlayTimer();

            _runtimeDataSystem.ClearRuntimeData();
            _runtimeData = null;

            SetGameState(E_GameState.Ended);

            Debug.Log("[GameSystem] Game ended.");
        }

        private bool HasRequiredSystems()
        {
            bool hasRequiredSystems = true;

            if (_runtimeDataSystem == null)
            {
                Debug.LogError("[GameSystem] RuntimeDataSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_uiManagementSystem == null)
            {
                Debug.LogError("[GameSystem] UIManagementSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_playerInputSystem == null)
            {
                Debug.LogError("[GameSystem] PlayerInputSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_uiInputSystem == null)
            {
                Debug.LogError("[GameSystem] UIInputSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_playerMovementSystem == null)
            {
                Debug.LogError("[GameSystem] PlayerMovementSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_playerControllerSystem == null)
            {
                Debug.LogError("[GameSystem] PlayerControllerSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_collisionSystem == null)
            {
                Debug.LogError("[GameSystem] CollisionSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_stageSystem == null)
            {
                Debug.LogError("[GameSystem] StageSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_timerSystem == null)
            {
                Debug.LogError("[GameSystem] TimerSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_infiniteModeSystem == null)
            {
                Debug.LogError("[GameSystem] InfiniteModeSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_resultSystem == null)
            {
                Debug.LogError("[GameSystem] ResultSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_cameraSystem == null)
            {
                Debug.LogError("[GameSystem] CameraSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_cameraFollow == null)
            {
                Debug.LogError("[GameSystem] CameraFollow is not assigned.");
                hasRequiredSystems = false;
            }

            return hasRequiredSystems;
        }

        private void AbortGameStart()
        {
            _stageSystem.StopStage();
            _playerInputSystem.DisablePlayerActionMap();
            _uiInputSystem.DisableUIActionMap();
            _cameraFollow.StopFollowing();
            _infiniteModeSystem.Stop();
            _playerMovementSystem.StopMovement();
            RemovePlayTimer();
            _runtimeDataSystem.ClearRuntimeData();
            _runtimeData = null;
            _currentGameState = E_GameState.None;

            Debug.LogError("[GameSystem] Game start was aborted because initialization failed.");
        }

        private void HandleStageEnded()
        {
            if (_currentGameState != E_GameState.Playing)
            {
                return;
            }

            StopPlayTimer();

            if (_stageSystem.IsCleared)
            {
                if (_resultSystem.CreateResultData(
                        true,
                        _timerSystem.GetElapsedTime(E_TimerKey.PlayTimer)))
                {
                    _uiManagementSystem.SetResultData(
                        _resultSystem.CurrentResultData);
                }
            }

            EndGame();
        }

        private void StopPlayTimer()
        {
            if (!_timerSystem.HasTimer(E_TimerKey.PlayTimer) ||
                !_timerSystem.TryGetTimerState(
                    E_TimerKey.PlayTimer,
                    out E_TimerState timerState) ||
                timerState == E_TimerState.Stopped)
            {
                return;
            }

            _timerSystem.StopTimer(E_TimerKey.PlayTimer);
        }

        private void ExecuteResultMenuSelection()
        {
            switch (_uiManagementSystem.CurrentResultMenuSelection)
            {
                case E_ResultMenuSelection.Retry:
                    StartGame();
                    break;

                case E_ResultMenuSelection.Quit:
                    Application.Quit();
                    break;
            }
        }

        private void RemovePlayTimer()
        {
            if (_timerSystem != null &&
                _timerSystem.HasTimer(E_TimerKey.PlayTimer))
            {
                _timerSystem.RemoveTimer(E_TimerKey.PlayTimer);
            }
        }

        private void SetGameState(E_GameState gameState)
        {
            _currentGameState = gameState;

            if (_runtimeData != null)
            {
                _runtimeData.SetGameState(gameState);
            }

            Debug.Log($"[GameSystem] Game State changed to {_currentGameState}.");
        }

        private void SetUIState(E_UIState uiState)
        {
            if (_runtimeData != null)
            {
                _runtimeData.SetUIState(uiState);
            }

            _uiManagementSystem.SetUIState(uiState);
        }
    }
}
