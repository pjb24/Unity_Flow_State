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
        [SerializeField] private SettingsSystem _settingsSystem;
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
        private readonly GameNavigationState _navigationState =
            new GameNavigationState();
        private IApplicationQuitService _applicationQuitService =
            new ApplicationQuitService();
        private GameRuntimeData _runtimeData;

        public E_GameState CurrentGameState => _gameState.CurrentState;

        public E_NavigationScreen CurrentNavigationScreen =>
            _navigationState.CurrentScreen;

        public E_NavigationItem CurrentNavigationSelection =>
            _navigationState.CurrentSelection;

        private void Start()
        {
            InitializeBoot();
        }

        private void Update()
        {
            if (CurrentGameState != E_GameState.Playing &&
                CurrentGameState != E_GameState.Paused &&
                CurrentGameState != E_GameState.Ended)
            {
                ProcessNavigationCancelInput();
                return;
            }

            switch (CurrentGameState)
            {
                case E_GameState.Playing:
                    ProcessPlayingInput();
                    break;

                case E_GameState.Paused:
                    ProcessNavigationCancelInput();
                    break;

                case E_GameState.Ended:
                    ProcessNavigationCancelInput();
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
            if (CurrentGameState == E_GameState.Playing)
            {
                Debug.LogWarning("[GameSystem] Game is already running.");
                return;
            }

            bool hasRunRequest = _navigationState.CurrentScreen ==
                                 E_NavigationScreen.Result
                ? BeginRunFromResult()
                : BeginRunFromMenu(_selectedGameMode);

            if (!hasRunRequest)
            {
                return;
            }

            StartRequestedRun();
        }

        public void RequestNavigationSelection(E_NavigationItem item)
        {
            if (!_navigationState.TrySelect(item))
            {
                return;
            }

            E_NavigationScreen screen = _navigationState.CurrentScreen;

            switch (screen)
            {
                case E_NavigationScreen.MainMenu:
                    HandleMainMenuSelection();
                    break;

                case E_NavigationScreen.ModeSelect:
                    HandleModeSelectSelection();
                    break;

                case E_NavigationScreen.Pause:
                    HandlePauseSelection(item);
                    break;

                case E_NavigationScreen.PauseMainMenuConfirmation:
                    HandlePauseMainMenuConfirmation(item);
                    break;

                case E_NavigationScreen.Result:
                    HandleResultSelection(item);
                    break;

                case E_NavigationScreen.LeaderboardUnavailable:
                case E_NavigationScreen.HowToPlay:
                case E_NavigationScreen.Settings:
                    if (_navigationState.TrySubmit())
                    {
                        ApplyNavigationState();
                    }
                    break;

                case E_NavigationScreen.AutomaticHowToPlay:
                    if (_navigationState.TrySubmit())
                    {
                        StartRequestedRun();
                    }
                    break;
            }
        }

        public void RequestNavigationCancel()
        {
            switch (_navigationState.CurrentScreen)
            {
                case E_NavigationScreen.Playing:
                    PauseGame();
                    return;

                case E_NavigationScreen.Pause:
                    ResumeGame();
                    return;
            }

            if (_navigationState.TryCancel())
            {
                ApplyNavigationState();
            }
        }

        // Unity Button.onClick only exposes parameterless methods reliably in the Inspector.
        // Keep these adapters as the single Inspector-facing entry points; the navigation
        // state still receives the strongly typed enum value through the method above.
        public void SelectPlay() => RequestNavigationSelection(E_NavigationItem.Play);
        public void SelectHowToPlay() => RequestNavigationSelection(E_NavigationItem.HowToPlay);
        public void SelectLeaderboard() => RequestNavigationSelection(E_NavigationItem.Leaderboard);
        public void SelectSettings() => RequestNavigationSelection(E_NavigationItem.Settings);
        public void SelectQuit() => RequestNavigationSelection(E_NavigationItem.Quit);
        public void SelectStage() => RequestNavigationSelection(E_NavigationItem.Stage);
        public void SelectInfinite() => RequestNavigationSelection(E_NavigationItem.Infinite);
        public void SelectBack() => RequestNavigationSelection(E_NavigationItem.Back);
        public void SelectStartRun() => RequestNavigationSelection(E_NavigationItem.StartRun);
        public void SelectResume() => RequestNavigationSelection(E_NavigationItem.Resume);
        public void SelectRetry() => RequestNavigationSelection(E_NavigationItem.Retry);
        public void SelectMainMenu() => RequestNavigationSelection(E_NavigationItem.MainMenu);
        public void SelectCancel() => RequestNavigationSelection(E_NavigationItem.Cancel);

        public void SetSettingsMasterVolume(float normalizedVolume)
        {
            if (_settingsSystem == null)
            {
                return;
            }

            _settingsSystem.TrySetMasterVolume(
                Mathf.RoundToInt(normalizedVolume * 100.0f));
        }

        public void SetSettingsFullscreen(bool isFullscreen)
        {
            if (_settingsSystem != null)
            {
                _settingsSystem.TrySetFullscreen(isFullscreen);
            }
        }

        public void RestoreSettingsDefaults()
        {
            if (_settingsSystem != null)
            {
                _settingsSystem.TryRestoreDefaults();
            }
        }

        private void InitializeBoot()
        {
            if (!HasRequiredSystems())
            {
                return;
            }

            _playerInputSystem.Initialize();
            _playerInputSystem.DisablePlayerActionMap();
            _uiInputSystem.Initialize();
            _uiInputSystem.EnableUIActionMap();
            _uiManagementSystem.InitializeMenu();

            if (_navigationState.CompleteBoot())
            {
                ApplyNavigationState();
            }
        }

        private bool BeginRunFromMenu(E_GameMode gameMode)
        {
            if (_navigationState.CurrentScreen == E_NavigationScreen.Boot)
            {
                _navigationState.CompleteBoot();
            }

            if (_navigationState.CurrentScreen != E_NavigationScreen.MainMenu ||
                !_navigationState.TrySelect(E_NavigationItem.Play) ||
                !_navigationState.TrySubmit())
            {
                return false;
            }

            E_NavigationItem modeItem = gameMode == E_GameMode.Infinite
                ? E_NavigationItem.Infinite
                : E_NavigationItem.Stage;

            if (!_navigationState.TrySelect(modeItem) ||
                !_navigationState.TrySubmit())
            {
                return false;
            }

            _selectedGameMode = gameMode;
            return _navigationState.IsRunStartRequested;
        }

        private void StartRequestedRun()
        {
            if (!HasRequiredSystems())
            {
                CompleteRunInitialization(false);
                return;
            }

            if (CurrentGameState == E_GameState.Playing)
            {
                Debug.LogWarning("[GameSystem] Game is already running.");
                return;
            }

            if (!SetGameState(E_GameState.Initializing))
            {
                CompleteRunInitialization(false);
                return;
            }

            _runtimeData = _runtimeDataSystem.CreateRuntimeData(
                _selectedGameMode);
            _runtimeData.SetGameState(CurrentGameState);
            _runtimeData.PlayerMovementRuntimeData.Initialize();

            _uiManagementSystem.Initialize(_runtimeData);
            _resultSystem.Initialize();
            SetUIState(E_UIState.None);

            if (!_playerControllerSystem.Initialize() ||
                !_collisionSystem.Initialize() ||
                !_stageSystem.Initialize(_selectedGameMode) ||
                !_stageSystem.ConfigureCollectibles(
                    _runtimeData,
                    _playerControllerSystem.PlayerRigidbody) ||
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
            CompleteRunInitialization(true);
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
            _navigationState.TryPause();
            ApplyNavigationState();
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
            _stageSystem.RecheckCollectibleOverlaps();
            _navigationState.TryCancel();
            ApplyNavigationState();
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
            if (_navigationState.CurrentScreen == E_NavigationScreen.Pause &&
                _navigationState.TrySelect(E_NavigationItem.Retry) &&
                _navigationState.TrySubmit())
            {
                EndRun(false);
                StartRequestedRun();
                return CurrentGameState == E_GameState.Playing;
            }

            if (CurrentGameState != E_GameState.Ended ||
                !BeginRunFromResult())
            {
                return false;
            }

            StartRequestedRun();
            return CurrentGameState == E_GameState.Playing;
        }

        [ContextMenu("End Game")]
        public void EndGame()
        {
            EndRun(true);
        }

        private void EndRun(bool shouldShowResult)
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
            SetUIState(shouldShowResult ? E_UIState.Result : E_UIState.None);

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

            if (shouldShowResult)
            {
                _navigationState.TryHandleStageEnded();
            }
            else
            {
                _navigationState.TryCancel();
            }

            ApplyNavigationState();

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

        private void ProcessNavigationCancelInput()
        {
            UIInputState inputState = _uiInputSystem.GetInputState();
            bool shouldCancel = inputState.IsCancelPressed;
            _uiInputSystem.ConsumeTransientInput();

            if (_settingsSystem != null && _settingsSystem.ConsumeRebindCancelSuppression())
            {
                return;
            }

            if (_settingsSystem != null && _settingsSystem.IsRebinding)
            {
                return;
            }

            if (shouldCancel && _settingsSystem != null &&
                _settingsSystem.TryCancelRestoreConfirmation())
            {
                return;
            }

            if (shouldCancel)
            {
                RequestNavigationCancel();
            }
        }

        private void HandleMainMenuSelection()
        {
            if (_navigationState.CurrentSelection == E_NavigationItem.Quit)
            {
                if (_navigationState.TrySubmit())
                {
                    RequestApplicationQuit();
                }

                return;
            }

            if (_navigationState.TrySubmit())
            {
                ApplyNavigationState();
            }
        }

        private void HandleModeSelectSelection()
        {
            E_NavigationItem selection = _navigationState.CurrentSelection;

            if (!_navigationState.TrySubmit())
            {
                return;
            }

            if (selection == E_NavigationItem.Stage ||
                selection == E_NavigationItem.Infinite)
            {
                _selectedGameMode = selection == E_NavigationItem.Infinite
                    ? E_GameMode.Infinite
                    : E_GameMode.Stage;

                if (_navigationState.IsRunStartRequested)
                {
                    StartRequestedRun();
                    return;
                }

                ApplyNavigationState();
                return;
            }

            ApplyNavigationState();
        }

        private void HandlePauseSelection(E_NavigationItem selection)
        {
            switch (selection)
            {
                case E_NavigationItem.Resume:
                    ResumeGame();
                    break;

                case E_NavigationItem.Retry:
                    RetryGame();
                    break;

                case E_NavigationItem.Settings:
                case E_NavigationItem.MainMenu:
                    if (_navigationState.TrySubmit())
                    {
                        ApplyNavigationState();
                    }
                    break;
            }
        }

        private void HandlePauseMainMenuConfirmation(E_NavigationItem selection)
        {
            if (selection == E_NavigationItem.MainMenu)
            {
                if (_navigationState.TrySubmit())
                {
                    EndRun(false);
                    ResetToMainMenu();
                }

                return;
            }

            if (_navigationState.TrySubmit())
            {
                ApplyNavigationState();
            }
        }

        private void HandleResultSelection(E_NavigationItem selection)
        {
            if (selection == E_NavigationItem.Retry)
            {
                RetryGame();
                return;
            }

            if (selection == E_NavigationItem.MainMenu &&
                _navigationState.TrySubmit())
            {
                ResetToMainMenu();
            }
        }

        private bool BeginRunFromResult()
        {
            if (_navigationState.CurrentScreen != E_NavigationScreen.Result ||
                !_navigationState.TrySelect(E_NavigationItem.Retry) ||
                !_navigationState.TrySubmit())
            {
                return false;
            }

            return _navigationState.IsRunStartRequested;
        }

        private void ResetToMainMenu()
        {
            _uiInputSystem.EnableUIActionMap();
            ApplyNavigationState();
        }

        private void CompleteRunInitialization(bool isSuccess)
        {
            if (_navigationState.CompleteInitialization(isSuccess))
            {
                ApplyNavigationState();
            }
        }

        private void ApplyNavigationState()
        {
            _uiManagementSystem.SetNavigationScreen(
                _navigationState.CurrentScreen,
                _navigationState.CurrentSelection);
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
            _uiManagementSystem.SetGameState(E_GameState.None);
            SetUIState(E_UIState.None);
            CompleteRunInitialization(false);

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
                E_StageResultType stageResultType = _stageSystem.IsCleared
                    ? E_StageResultType.Cleared
                    : E_StageResultType.Fell;

                if (_resultSystem.CreateStageResultData(
                        stageResultType,
                        _timerSystem.GetElapsedTime(E_TimerKey.PlayTimer),
                        _runtimeData.CollectibleRuntimeData.CurrentScore))
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

            if (_resultSystem.CreateInfiniteResultData(
                    infiniteModeRuntimeData.ScoringVersion,
                    _runtimeData.GameMode,
                    _stageSystem.HasEnded,
                    infiniteModeRuntimeData.IsFinalized,
                    infiniteModeRuntimeData.CurrentDistance,
                    infiniteModeRuntimeData.BaseDistanceScore,
                    infiniteModeRuntimeData.MomentumBonus,
                    infiniteModeRuntimeData.CurrentScore,
                    infiniteModeRuntimeData.CollectibleScore,
                    infiniteModeRuntimeData.MaximumMomentumMultiplier))
            {
                _uiManagementSystem.SetResultData(
                    _resultSystem.CurrentResultData);
            }
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

            _uiManagementSystem.SetGameState(gameState);

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
