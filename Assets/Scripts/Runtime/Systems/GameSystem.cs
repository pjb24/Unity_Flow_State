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

        private readonly GameState _gameState = new GameState();
        private IApplicationQuitService _applicationQuitService =
            new ApplicationQuitService();
        private GameRuntimeData _runtimeData;

        public E_GameState CurrentGameState => _gameState.CurrentState;

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            switch (CurrentGameState)
            {
                case E_GameState.Playing:
                    ProcessPlayingInput();
                    break;

                case E_GameState.Paused:
                    ProcessPausedInput();
                    break;

                case E_GameState.Ended:
                    ProcessResultMenuInput();
                    break;
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

            if (CurrentGameState == E_GameState.Playing)
            {
                Debug.LogWarning("[GameSystem] Game is already running.");
                return;
            }

            if (!SetGameState(E_GameState.Initializing))
            {
                return;
            }

            _runtimeData = _runtimeDataSystem.CreateRuntimeData(
                _selectedGameMode);
            _runtimeData.SetGameState(CurrentGameState);
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
            _uiInputSystem.EnableUIActionMap();

            if (!SetGameState(E_GameState.Ready))
            {
                AbortGameStart();
                return;
            }
            SetUIState(E_UIState.StageHud);

            _stageSystem.AddStageEndedListener(HandleStageEnded);

            if (!_stageSystem.StartStage())
            {
                AbortGameStart();
                return;
            }

            if (_selectedGameMode == E_GameMode.Stage &&
                (!_timerSystem.CreateTimer(E_TimerKey.PlayTimer) ||
                 !_timerSystem.StartTimer(E_TimerKey.PlayTimer)))
            {
                AbortGameStart();
                return;
            }

            _playerInputSystem.EnablePlayerActionMap();
            _cameraFollow.StartFollowing();
            if (!SetGameState(E_GameState.Playing))
            {
                AbortGameStart();
                return;
            }

            Debug.Log("[GameSystem] Game started.");
        }

        public bool PauseGame()
        {
            if (CurrentGameState != E_GameState.Playing ||
                !PausePlaySystems())
            {
                return false;
            }

            if (!SetGameState(E_GameState.Paused))
            {
                ResumePlaySystems();
                return false;
            }

            _playerInputSystem.DisablePlayerActionMap();
            _uiInputSystem.EnableUIActionMap();
            SetUIState(E_UIState.Pause);
            return true;
        }

        public bool ResumeGame()
        {
            if (CurrentGameState != E_GameState.Paused ||
                !ResumePlaySystems())
            {
                return false;
            }

            if (!SetGameState(E_GameState.Playing))
            {
                PausePlaySystems();
                return false;
            }

            _playerInputSystem.EnablePlayerActionMap();
            _uiInputSystem.EnableUIActionMap();
            SetUIState(E_UIState.StageHud);
            return true;
        }

        private bool PausePlaySystems()
        {
            bool timerPaused = _runtimeData.GameMode != E_GameMode.Stage ||
                               _timerSystem.PauseTimer(E_TimerKey.PlayTimer);
            bool stagePaused = timerPaused && _stageSystem.PauseStage();
            bool infinitePaused = stagePaused &&
                                  (_runtimeData.GameMode != E_GameMode.Infinite ||
                                   _infiniteModeSystem.Pause());
            bool movementPaused = infinitePaused &&
                                  _playerMovementSystem.PauseMovement();
            bool physicsPaused = movementPaused &&
                                 _playerControllerSystem.PausePhysics();

            if (physicsPaused)
            {
                return true;
            }

            if (movementPaused)
            {
                _playerMovementSystem.ResumeMovement();
            }

            if (_runtimeData.GameMode == E_GameMode.Infinite && infinitePaused)
            {
                _infiniteModeSystem.Resume();
            }

            if (stagePaused)
            {
                _stageSystem.ResumeStage();
            }

            if (_runtimeData.GameMode == E_GameMode.Stage && timerPaused)
            {
                _timerSystem.ResumeTimer(E_TimerKey.PlayTimer);
            }

            return false;
        }

        private bool ResumePlaySystems()
        {
            bool physicsResumed = _playerControllerSystem.ResumePhysics();
            bool movementResumed = physicsResumed &&
                                   _playerMovementSystem.ResumeMovement();
            bool infiniteResumed = movementResumed &&
                                   (_runtimeData.GameMode != E_GameMode.Infinite ||
                                    _infiniteModeSystem.Resume());
            bool stageResumed = infiniteResumed && _stageSystem.ResumeStage();
            bool timerResumed = stageResumed &&
                                (_runtimeData.GameMode != E_GameMode.Stage ||
                                 _timerSystem.ResumeTimer(E_TimerKey.PlayTimer));

            return timerResumed;
        }

        public bool RetryGame()
        {
            if (CurrentGameState == E_GameState.Paused)
            {
                EndGame();
            }

            if (CurrentGameState != E_GameState.Ended)
            {
                return false;
            }

            StartGame();
            return CurrentGameState == E_GameState.Playing;
        }

        [ContextMenu("End Game")]
        public void EndGame()
        {
            if (!HasRequiredSystems())
            {
                return;
            }

            if (CurrentGameState == E_GameState.Ending ||
                CurrentGameState == E_GameState.Ended)
            {
                Debug.LogWarning("[GameSystem] Game is already ending or ended.");
                return;
            }

            if (_runtimeData == null || !_runtimeDataSystem.HasRuntimeData)
            {
                Debug.LogWarning("[GameSystem] Runtime Data does not exist.");
                return;
            }

            if (!SetGameState(E_GameState.Ending))
            {
                return;
            }
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

        private void ProcessPlayingInput()
        {
            UIInputState inputState = _uiInputSystem.GetInputState();
            bool shouldPause = inputState.IsCancelPressed;
            _uiInputSystem.ConsumeTransientInput();

            if (shouldPause)
            {
                PauseGame();
            }
        }

        private void ProcessPausedInput()
        {
            UIInputState inputState = _uiInputSystem.GetInputState();
            bool shouldExecuteSelection = false;
            E_PauseMenuSelection selection =
                _uiManagementSystem.CurrentPauseMenuSelection;

            if (inputState.IsCancelPressed)
            {
                shouldExecuteSelection =
                    _uiManagementSystem.TryCancelPauseMenu(out selection);
            }
            else if (inputState.IsClickPressed)
            {
                shouldExecuteSelection =
                    _uiManagementSystem.TryClickPauseMenuSelection(
                        inputState.PointerPosition,
                        out selection);
            }
            else
            {
                if (inputState.IsPointChanged)
                {
                    _uiManagementSystem.TrySetPauseMenuSelectionAtPointer(
                        inputState.PointerPosition);
                }

                if (Mathf.Abs(inputState.NavigateInput.y) >= 0.5f)
                {
                    _uiManagementSystem.MovePauseMenuSelection(
                        inputState.NavigateInput.y);
                }

                if (inputState.IsSubmitPressed)
                {
                    shouldExecuteSelection =
                        _uiManagementSystem.TrySubmitPauseMenuSelection(
                            out selection);
                }
            }

            _uiInputSystem.ConsumeTransientInput();

            if (shouldExecuteSelection)
            {
                ExecutePauseMenuSelection(selection);
            }
        }

        private void ProcessResultMenuInput()
        {

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
            _gameState.Reset();

            Debug.LogError("[GameSystem] Game start was aborted because initialization failed.");
        }

        private void HandleStageEnded()
        {
            if (CurrentGameState != E_GameState.Playing &&
                CurrentGameState != E_GameState.Paused)
            {
                return;
            }

            StopPlayTimer();

            if (_runtimeData.GameMode == E_GameMode.Stage)
            {
                if (_stageSystem.IsCleared &&
                    _resultSystem.CreateResultData(
                        true,
                        _timerSystem.GetElapsedTime(E_TimerKey.PlayTimer)))
                {
                    _uiManagementSystem.SetResultData(
                        _resultSystem.CurrentResultData);
                }
            }
            else if (_runtimeData.GameMode == E_GameMode.Infinite)
            {
                CreateInfiniteResultData();
            }

            EndGame();
        }

        private void CreateInfiniteResultData()
        {
            InfiniteModeRuntimeData infiniteModeRuntimeData =
                _runtimeData.InfiniteModeRuntimeData;

            if (infiniteModeRuntimeData == null)
            {
                Debug.LogError(
                    "[GameSystem] Infinite Mode Runtime Data does not exist.");
                return;
            }

            _resultSystem.CreateInfiniteResultData(
                _runtimeData.GameMode,
                _stageSystem.HasEnded,
                infiniteModeRuntimeData.IsFinalized,
                infiniteModeRuntimeData.CurrentDistance,
                infiniteModeRuntimeData.CurrentScore);
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
                    RetryGame();
                    break;

                case E_ResultMenuSelection.Quit:
                    RequestApplicationQuit();
                    break;
            }
        }

        private void ExecutePauseMenuSelection(E_PauseMenuSelection selection)
        {
            switch (selection)
            {
                case E_PauseMenuSelection.Resume:
                    ResumeGame();
                    break;

                case E_PauseMenuSelection.Retry:
                    RetryGame();
                    break;

                case E_PauseMenuSelection.Quit:
                    RequestApplicationQuit();
                    break;
            }
        }

        private void RequestApplicationQuit()
        {
            _applicationQuitService.RequestQuit();
        }

        private void RemovePlayTimer()
        {
            if (_timerSystem != null &&
                _timerSystem.HasTimer(E_TimerKey.PlayTimer))
            {
                _timerSystem.RemoveTimer(E_TimerKey.PlayTimer);
            }
        }

        private bool SetGameState(E_GameState gameState)
        {
            if (!_gameState.TryTransitionTo(gameState))
            {
                Debug.LogError(
                    $"[GameSystem] Game State transition is invalid: " +
                    $"{CurrentGameState} -> {gameState}.");
                return false;
            }

            if (_runtimeData != null)
            {
                _runtimeData.SetGameState(gameState);
            }

            Debug.Log($"[GameSystem] Game State changed to {CurrentGameState}.");
            return true;
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
