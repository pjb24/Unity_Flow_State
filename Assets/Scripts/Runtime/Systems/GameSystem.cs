using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class GameSystem : MonoBehaviour
    {
        private const int ResultOnlineWaitTimeoutMilliseconds = 8000;
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
        private LocalRecordRepository _localRecordRepository;
        private RecordSubmissionService _recordSubmissionService;
        private LocalSaveData _localSaveData;
        private OnlineRecordCoordinator _onlineRecords;
        private IOnlineRecordRepository _onlineRepository;
        private readonly LeaderboardViewState _leaderboardViewState =
            new LeaderboardViewState();
        private ResultRecordPresentation _resultRecordPresentation =
            new ResultRecordPresentation(false, null, E_ResultSubmissionState.NotAvailable,
                string.Empty, null);
        private NetworkReachability _lastReachability;
        private bool _shouldEnterLeaderboardAfterConsent;
        private bool _isResultPresentationDeferred;
        private bool _isCurrentResultRetryRunning;
        private bool _isLeaderboardPendingRetryRunning;
        private string _leaderboardPendingRetryStatus = "No manual retry yet";
        private float _resultLoadingDeadline;
        private int _lastResultLoadingSeconds = -1;
#if UNITY_EDITOR
        internal static bool UseIsolatedRecordsForPlayModeTests;

        private sealed class TestMemorySaveFileStore : ILocalSaveFileStore
        {
            private string _contents;

            public bool HasSave => _contents != null;

            public bool TryRead(out string contents)
            {
                contents = _contents;
                return HasSave;
            }

            public bool TryWriteAtomically(string contents)
            {
                if (string.IsNullOrEmpty(contents)) return false;
                _contents = contents;
                return true;
            }
        }
#endif
        public IOnlineRecordRepository OnlineRecords => _onlineRepository;
        public LeaderboardViewState LeaderboardViewState => _leaderboardViewState;
        public ResultRecordPresentation ResultRecordPresentation => _resultRecordPresentation;
        public string OnlinePlayerId => _localRecordRepository == null
            ? string.Empty : _localRecordRepository.OnlineAccount.PlayerId;
        public int PendingOnlineRecordCount => _localRecordRepository == null
            ? 0 : _localRecordRepository.CreatePendingSnapshot().Count;
        public int SubmittedOnlineRecordCount => _localRecordRepository == null
            ? 0 : _onlineRecords == null ? 0 : _onlineRecords.SubmittedCount;
        public int RejectedOnlineRecordCount => _localRecordRepository == null
            ? 0 : _onlineRecords == null ? 0 : _onlineRecords.RejectedCount;
        private GameRuntimeData _candidateRuntimeData;

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
            NetworkReachability reachability = Application.internetReachability;
            if (_lastReachability == NetworkReachability.NotReachable &&
                reachability != NetworkReachability.NotReachable) RetryOnlineRecords();
            _lastReachability = reachability;
            if (_isResultPresentationDeferred)
            {
                UpdateResultSubmissionLoadingCountdown();
                if (_uiInputSystem != null) _uiInputSystem.ConsumeTransientInput();
                return;
            }
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
            if (_settingsSystem != null)
            {
                _settingsSystem.SettingsChanged -= SaveLocalState;
            }

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
            if (_isResultPresentationDeferred) return;
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

                case E_NavigationScreen.Leaderboard:
                    HandleLeaderboardSelection(item);
                    break;

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
                        SaveLocalState();
                        StartRequestedRun();
                    }
                    break;
            }
        }

        public void RequestNavigationCancel()
        {
            if (_isResultPresentationDeferred) return;
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
        public void SelectLeaderboardTop() => RequestNavigationSelection(E_NavigationItem.LeaderboardTop);
        public void SelectLeaderboardAround() => RequestNavigationSelection(E_NavigationItem.LeaderboardAround);
        public void SelectLeaderboardRetry() => RequestNavigationSelection(E_NavigationItem.LeaderboardRetry);
        public void SelectResultLeaderboard() => RequestNavigationSelection(E_NavigationItem.ResultLeaderboard);
        public void SelectLeaderboardPendingRetry() =>
            RequestNavigationSelection(E_NavigationItem.LeaderboardPendingRetry);

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
            InitializeLocalState();
            _uiManagementSystem.InitializeMenu();

            if (_navigationState.CompleteBoot())
            {
                ApplyNavigationState();
            }
        }

        private void InitializeLocalState()
        {
            ILocalSaveFileStore saveFileStore;
#if UNITY_EDITOR
            saveFileStore = UseIsolatedRecordsForPlayModeTests
                ? new TestMemorySaveFileStore()
                : new PersistentLocalSaveFileStore();
#else
            saveFileStore = new PersistentLocalSaveFileStore();
#endif
            _localRecordRepository = new LocalRecordRepository(saveFileStore);
            _recordSubmissionService = new RecordSubmissionService(
                _localRecordRepository);
            _localRecordRepository.TryLoad(out _localSaveData);

            if (_localSaveData == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_localSaveData.AccountId))
            {
                _localSaveData = new LocalSaveData(
                    LocalSaveData.CurrentVersion,
                    Guid.NewGuid().ToString("D"),
                    _localSaveData.Settings,
                    _localSaveData.HasCompletedTutorial,
                    _localSaveData.PersonalBests);
                _localRecordRepository.TrySave(_localSaveData);
            }

            _settingsSystem.ApplyLocalSettings(_localSaveData.Settings);
            _navigationState.RestoreAutomaticHowToPlayCompleted(
                _localSaveData.HasCompletedTutorial);
            _settingsSystem.SettingsChanged += SaveLocalState;
#if UNITY_EDITOR
            if (UseIsolatedRecordsForPlayModeTests)
            {
                _lastReachability = Application.internetReachability;
                return;
            }
#endif
            IOnlineAuthenticationGateway authentication = new UgsOnlineAuthenticationGateway();
            _onlineRepository = new CloudCodeRecordRepository(_localSaveData.AccountId,
                () => _localRecordRepository.OnlineAccount, authentication, new UgsOnlineRecordTransport());
            _onlineRecords = new OnlineRecordCoordinator(_localSaveData.AccountId,
                _localRecordRepository, authentication, _onlineRepository);
            _lastReachability = Application.internetReachability;
            RetryOnlineRecords();
        }

        public async Task<bool> ConfirmOnlineRecoveryNoticeAsync()
        {
            if (_onlineRecords == null) return false;
            bool confirmed = await _onlineRecords.ConfirmRecoveryNoticeAsync();
            if (confirmed)
            {
                await _onlineRecords.RetryPendingAsync();
                RefreshResultRecordPresentation();
            }
            return confirmed;
        }

        // Inspector adapter: explicit user consent is required before any online request.
        public async void ConfirmOnlineRecoveryNotice()
        {
            bool didConfirm = await ConfirmOnlineRecoveryNoticeAsync();
            SetOnlineRecoveryNoticeVisible(false);
            if (didConfirm && _shouldEnterLeaderboardAfterConsent)
            {
                _shouldEnterLeaderboardAfterConsent = false;
                RequestNavigationSelection(_navigationState.CurrentSelection);
                return;
            }
            _shouldEnterLeaderboardAfterConsent = false;
        }

        // Inspector adapter: cancellation performs no authentication or network request.
        public void CancelOnlineRecoveryNotice()
        {
            _shouldEnterLeaderboardAfterConsent = false;
            SetOnlineRecoveryNoticeVisible(false);
        }

        public void OpenOnlineRecoveryNoticeFromSettings()
        {
            _shouldEnterLeaderboardAfterConsent = false;
            SetOnlineRecoveryNoticeVisible(true);
        }

        // Only the active Result's Pending submission is eligible for an explicit retry.
        public void RetryPendingSubmission()
        {
            if (_resultRecordPresentation.SubmissionState == E_ResultSubmissionState.Pending)
                RetryCurrentResultSubmission();
        }

        private async void RetryCurrentResultSubmission()
        {
            RecordSubmissionCandidate candidate = _resultRecordPresentation.Candidate;
            if (_isCurrentResultRetryRunning || candidate == null || _onlineRecords == null ||
                Application.internetReachability == NetworkReachability.NotReachable) return;
            _isCurrentResultRetryRunning = true;
            try
            {
                await _onlineRecords.RetrySubmissionAsync(candidate.SubmissionId);
                if (this == null || _resultRecordPresentation.Candidate != candidate) return;
                RefreshResultRecordPresentation();
                if (_resultRecordPresentation.SubmissionState == E_ResultSubmissionState.Submitted)
                    RefreshResultOnlineBest();
            }
            finally { if (this != null) _isCurrentResultRetryRunning = false; }
        }

        public async void RetryOnlineRecords()
        {
            await RetryOnlineRecordsAsync();
        }

        public async Task RetryOnlineRecordsAsync()
        {
            if (_onlineRecords != null &&
                Application.internetReachability != NetworkReachability.NotReachable)
            {
                await _onlineRecords.RetryPendingAsync();
                RefreshResultRecordPresentation();
                ApplyLeaderboardPresentation();
            }
        }

        private void SaveLocalState()
        {
            if (_localRecordRepository == null || _settingsSystem == null)
            {
                return;
            }

            _localSaveData = _localRecordRepository.CreateSaveData(
                _localSaveData == null ? string.Empty : _localSaveData.AccountId,
                _settingsSystem.CreateLocalSettingsData(),
                _navigationState.HasAutomaticHowToPlayCompleted);
            _localRecordRepository.TrySave(_localSaveData);
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

            _resultRecordPresentation = new ResultRecordPresentation(false, null,
                E_ResultSubmissionState.NotAvailable, string.Empty, null);
            ApplyResultRecordPresentation();

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

            if (!_timerSystem.CreateTimer(GetRunTimerKey()) ||
                !_timerSystem.StartTimer(GetRunTimerKey()))
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
            bool timerPaused = _timerSystem.PauseTimer(GetRunTimerKey());
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

            if (timerPaused)
            {
                _timerSystem.ResumeTimer(GetRunTimerKey());
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
                                _timerSystem.ResumeTimer(GetRunTimerKey());

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
            SetUIState(shouldShowResult && !_isResultPresentationDeferred
                ? E_UIState.Result : E_UIState.None);

            StopRunTimer();
            _stageSystem.StopStage();
            _playerInputSystem.DisablePlayerActionMap();
            _uiInputSystem.EnableUIActionMap();
            _cameraFollow.StopFollowing();
            _infiniteModeSystem.Stop();
            _playerMovementSystem.StopMovement();
            RemoveRunTimer();

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

            if (!_isResultPresentationDeferred) ApplyNavigationState();

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

            if (_navigationState.CurrentSelection == E_NavigationItem.Leaderboard &&
                !HasConfirmedOnlineRecoveryNotice())
            {
                _shouldEnterLeaderboardAfterConsent = true;
                SetOnlineRecoveryNoticeVisible(true);
                return;
            }

            if (_navigationState.TrySubmit())
            {
                ApplyNavigationState();
                if (_navigationState.CurrentScreen == E_NavigationScreen.Leaderboard)
                {
                    _leaderboardViewState.SelectGameMode(E_GameMode.Stage);
                    RefreshLeaderboardTop();
                    RefreshLeaderboardAround();
                }
            }
        }

        private void HandleLeaderboardSelection(E_NavigationItem selection)
        {
            if (selection == E_NavigationItem.Back)
            {
                if (_navigationState.TrySubmit()) ApplyNavigationState();
                return;
            }

            if (selection == E_NavigationItem.LeaderboardPendingRetry)
            {
                if (_navigationState.TrySubmit()) RetryPendingFromLeaderboard();
                return;
            }

            if (selection == E_NavigationItem.LeaderboardTop ||
                selection == E_NavigationItem.LeaderboardRetry)
            {
                RefreshLeaderboardTop();
            }

            if (selection == E_NavigationItem.LeaderboardAround ||
                selection == E_NavigationItem.LeaderboardRetry)
            {
                RefreshLeaderboardAround();
            }

            if (selection == E_NavigationItem.Stage ||
                selection == E_NavigationItem.Infinite)
            {
                if (!_navigationState.TrySubmit()) return;
                _leaderboardViewState.SelectGameMode(
                    selection == E_NavigationItem.Infinite
                        ? E_GameMode.Infinite : E_GameMode.Stage);
                ApplyLeaderboardPresentation();
                RefreshLeaderboardTop();
                RefreshLeaderboardAround();
            }
        }

        public async void RefreshLeaderboardTop()
        {
            int version = _leaderboardViewState.BeginTopRequest();
            ApplyLeaderboardPresentation();
            _leaderboardViewState.CompleteTopRequest(version,
                await QueryLeaderboardAsync(true));
            ApplyLeaderboardPresentation();
        }

        public async void RefreshLeaderboardAround()
        {
            int version = _leaderboardViewState.BeginAroundRequest();
            ApplyLeaderboardPresentation();
            _leaderboardViewState.CompleteAroundRequest(version,
                await QueryLeaderboardAsync(false));
            ApplyLeaderboardPresentation();
        }

        private async void RefreshResultOnlineBest()
        {
            await RefreshResultOnlineBestAsync();
        }

        private async Task RefreshResultOnlineBestAsync(
            RecordSubmissionCandidate expectedCandidate = null)
        {
            int version = _leaderboardViewState.BeginPersonalBestRequest();
            if (_onlineRepository == null ||
                Application.internetReachability == NetworkReachability.NotReachable)
            {
                _leaderboardViewState.CompletePersonalBestRequest(version,
                    new OnlineLeaderboardResult { status = "TransientFailure", reason = "NotReachable" });
                RefreshResultRecordPresentation();
                return;
            }

            RecordBoardKey key;
            E_GameMode resultMode = expectedCandidate == null
                ? GetCurrentResultGameMode() : expectedCandidate.BoardKey.GameMode;
            bool hasKey = resultMode == E_GameMode.Stage
                ? RecordBoardKey.TryCreateStage("stage-001", 1, out key)
                : RecordBoardKey.TryCreateInfinite(2, out key);
            if (!hasKey) return;
            OnlineLeaderboardResult result = await _onlineRepository.GetPersonalBestAsync(key);
            if (expectedCandidate != null &&
                _resultRecordPresentation.Candidate != expectedCandidate) return;
            _leaderboardViewState.CompletePersonalBestRequest(version,
                result);
            RefreshResultRecordPresentation();
        }

        private async Task<OnlineLeaderboardResult> QueryLeaderboardAsync(bool isTop)
        {
            if (_onlineRepository == null)
                return new OnlineLeaderboardResult { status = "TransientFailure", reason = "AuthenticationUnavailable" };
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return new OnlineLeaderboardResult { status = "TransientFailure", reason = "NotReachable" };
            RecordBoardKey key;
            bool hasKey = _leaderboardViewState.GameMode == E_GameMode.Stage
                ? RecordBoardKey.TryCreateStage("stage-001", 1, out key)
                : RecordBoardKey.TryCreateInfinite(2, out key);
            if (!hasKey) return new OnlineLeaderboardResult { status = "TransientFailure", reason = "InvalidQuery" };
            return isTop ? await _onlineRepository.GetTopAsync(key) : await _onlineRepository.GetAroundAsync(key);
        }

        private void ApplyLeaderboardPresentation()
        {
            E_NavigationItem previousSelection = _navigationState.CurrentSelection;
            _navigationState.SetLeaderboardPendingRetryAvailable(
                PendingOnlineRecordCount > 0 && !_isLeaderboardPendingRetryRunning);
            if (_uiManagementSystem != null)
            {
                _uiManagementSystem.SetLeaderboardViewState(
                    _leaderboardViewState, OnlinePlayerId,
                    PendingOnlineRecordCount, _isLeaderboardPendingRetryRunning,
                    _leaderboardPendingRetryStatus);
                if (previousSelection != _navigationState.CurrentSelection)
                    ApplyNavigationState();
            }
        }

        private async void RetryPendingFromLeaderboard()
        {
            if (_isLeaderboardPendingRetryRunning || _onlineRecords == null ||
                PendingOnlineRecordCount == 0 ||
                Application.internetReachability == NetworkReachability.NotReachable) return;
            _isLeaderboardPendingRetryRunning = true;
            _leaderboardPendingRetryStatus = "Submitting saved records...";
            ApplyLeaderboardPresentation();
            int submittedBefore = _onlineRecords.SubmittedCount;
            int rejectedBefore = _onlineRecords.RejectedCount;
            try
            {
                await _onlineRecords.RetryAllPendingAsync();
                if (this == null) return;
                RefreshResultRecordPresentation();
                _leaderboardPendingRetryStatus = "Last retry: " +
                    (_onlineRecords.SubmittedCount - submittedBefore) + " submitted, " +
                    (_onlineRecords.RejectedCount - rejectedBefore) + " rejected, " +
                    PendingOnlineRecordCount + " pending";
            }
            catch (Exception)
            {
                _leaderboardPendingRetryStatus = "Retry unavailable; pending retained";
                Debug.LogWarning("[GameSystem] Manual pending retry unavailable.");
            }
            finally
            {
                if (this != null)
                {
                    _isLeaderboardPendingRetryRunning = false;
                    ApplyLeaderboardPresentation();
                }
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

            if (selection == E_NavigationItem.ResultLeaderboard &&
                !HasConfirmedOnlineRecoveryNotice())
            {
                _shouldEnterLeaderboardAfterConsent = true;
                SetOnlineRecoveryNoticeVisible(true);
                return;
            }

            if (selection == E_NavigationItem.ResultLeaderboard &&
                _navigationState.TrySubmit())
            {
                E_GameMode resultMode = GetCurrentResultGameMode();
                _navigationState.TrySelect(resultMode == E_GameMode.Stage
                    ? E_NavigationItem.Stage : E_NavigationItem.Infinite);
                ApplyNavigationState();
                _leaderboardViewState.SelectGameMode(resultMode);
                RefreshResultOnlineBest();
                RefreshLeaderboardTop();
                RefreshLeaderboardAround();
                return;
            }

            if (selection == E_NavigationItem.SubmissionRetry &&
                _navigationState.TrySubmit())
            {
                RetryPendingSubmission();
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
            RemoveRunTimer();
            _runtimeDataSystem.ClearRuntimeData();
            _runtimeData = null;
            _gameState.Reset();
            _uiManagementSystem.SetGameState(E_GameState.None);
            SetUIState(E_UIState.None);
            CompleteRunInitialization(false);

            Debug.LogError("[GameSystem] Game start was aborted because initialization failed.");
        }

        private async void HandleStageEnded()
        {
            if (CurrentGameState != E_GameState.Playing &&
                CurrentGameState != E_GameState.Paused)
            {
                return;
            }

            StopRunTimer();
            _selectedGameMode = _runtimeData.GameMode;
            _leaderboardViewState.SelectGameMode(_selectedGameMode);

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
                    TryStoreCurrentResultCandidate(
                        _resultSystem.CurrentResultData);
                }
            }
            else if (_runtimeData.GameMode == E_GameMode.Infinite)
            {
                CreateInfiniteResultData();
            }

            RecordSubmissionCandidate candidate = _candidateRuntimeData == _runtimeData
                ? _resultRecordPresentation.Candidate : null;
            bool shouldSubmitBeforeResult = candidate != null && _onlineRecords != null &&
                HasConfirmedOnlineRecoveryNotice() &&
                Application.internetReachability != NetworkReachability.NotReachable;
            _isResultPresentationDeferred = shouldSubmitBeforeResult;
            if (shouldSubmitBeforeResult)
            {
                _resultLoadingDeadline = Time.realtimeSinceStartup +
                    ResultOnlineWaitTimeoutMilliseconds / 1000f;
                _lastResultLoadingSeconds = -1;
                UpdateResultSubmissionLoadingCountdown();
                _uiManagementSystem.SetResultSubmissionLoading(true);
            }
            EndGame();
            if (CurrentGameState != E_GameState.Ended)
            {
                _isResultPresentationDeferred = false;
                if (shouldSubmitBeforeResult)
                    _uiManagementSystem.SetResultSubmissionLoading(false);
                return;
            }

            if (shouldSubmitBeforeResult)
            {
                try
                {
                    Task onlineWork = CompleteCurrentResultOnlineAsync(candidate);
                    Task completed = await Task.WhenAny(onlineWork,
                        Task.Delay(ResultOnlineWaitTimeoutMilliseconds));
                    if (this == null) return;
                    if (completed != onlineWork)
                    {
                        Debug.LogWarning("[GameSystem] Result online wait timed out; status may update later.");
                        RefreshResultRecordPresentation();
                    }
                }
                catch (Exception)
                {
                    Debug.LogWarning("[GameSystem] Result online update unavailable.");
                }
                finally
                {
                    if (this != null)
                    {
                        _isResultPresentationDeferred = false;
                        _uiInputSystem.ConsumeTransientInput();
                        SetUIState(E_UIState.Result);
                        ApplyNavigationState();
                        _uiManagementSystem.SetResultSubmissionLoading(false);
                    }
                }
                return;
            }

            if (candidate != null) RefreshResultOnlineBest();
        }

        private void UpdateResultSubmissionLoadingCountdown()
        {
            int remainingSeconds = Mathf.CeilToInt(Mathf.Max(0f,
                _resultLoadingDeadline - Time.realtimeSinceStartup));
            if (remainingSeconds == _lastResultLoadingSeconds) return;
            _lastResultLoadingSeconds = remainingSeconds;
            _uiManagementSystem.SetResultSubmissionLoadingRemainingSeconds(remainingSeconds);
        }

        private async Task CompleteCurrentResultOnlineAsync(RecordSubmissionCandidate candidate)
        {
            try
            {
                await _onlineRecords.RetrySubmissionAsync(candidate.SubmissionId);
                if (this == null || _resultRecordPresentation.Candidate != candidate) return;
                RefreshResultRecordPresentation();
                await RefreshResultOnlineBestAsync(candidate);
            }
            catch (Exception)
            {
                Debug.LogWarning("[GameSystem] Result online update unavailable.");
            }
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
                TryStoreCurrentResultCandidate(_resultSystem.CurrentResultData);
            }
        }

        private void TryStoreCurrentResultCandidate(ResultData resultData)
        {
            if (resultData == null || _runtimeData == null ||
                _candidateRuntimeData == _runtimeData ||
                _recordSubmissionService == null || _localSaveData == null ||
                string.IsNullOrEmpty(_localSaveData.AccountId))
            {
                return;
            }

            string submissionId = Guid.NewGuid().ToString("D");
            bool isCandidateCreated;
            RecordSubmissionCandidate candidate;

            if (resultData.GameMode == E_GameMode.Stage)
            {
                isCandidateCreated = RecordSubmissionPolicy.TryCreateStageCandidate(
                    _localSaveData.AccountId,
                    submissionId,
                    StageRecordIdentity.CurrentStageId,
                    StageRecordIdentity.CurrentRulesVersion,
                    resultData.StageResultType,
                    resultData.ElapsedTime,
                    out candidate);
            }
            else if (resultData.GameMode == E_GameMode.Infinite &&
                     TryCreateInfiniteScoreLimit(
                         resultData,
                         out InfiniteScoreLimit scoreLimit))
            {
                isCandidateCreated = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                    _localSaveData.AccountId,
                    submissionId,
                    resultData.ScoringVersion,
                    GetPlayDurationMilliseconds(),
                    resultData.BaseDistanceScore,
                    resultData.MomentumBonus,
                    resultData.DistanceScore,
                    resultData.CollectibleScore,
                    resultData.TotalScore,
                    resultData.MaximumMomentumMultiplier,
                    scoreLimit,
                    out candidate);
            }
            else
            {
                return;
            }

            if (!isCandidateCreated)
            {
                return;
            }

            bool hasPreviousBest = _localRecordRepository.TryGetPersonalBest(
                candidate.PlayerId, candidate.BoardKey, out RecordSubmissionCandidate previousBest);
            bool isNewLocalBest = !hasPreviousBest ||
                RecordLeaderboardPolicy.ShouldReplaceBest(previousBest, candidate);
            if (!_recordSubmissionService.TryStoreCandidate(candidate)) return;

            _candidateRuntimeData = _runtimeData;
            _resultRecordPresentation = new ResultRecordPresentation(isNewLocalBest, candidate,
                E_ResultSubmissionState.Pending, string.Empty,
                null);
            ApplyResultRecordPresentation();
            SaveLocalState();
        }

        private void RefreshResultRecordPresentation()
        {
            RecordSubmissionCandidate candidate = _resultRecordPresentation.Candidate;
            if (candidate == null || _localRecordRepository == null) return;

            E_ResultSubmissionState state = E_ResultSubmissionState.NotAvailable;
            string reason = string.Empty;
            if (ContainsPendingSubmission(candidate.SubmissionId)) state = E_ResultSubmissionState.Pending;
            else if (_onlineRecords != null && _onlineRecords.TryGetTerminalResult(
                         candidate.SubmissionId, out OnlineSubmissionResult result))
            {
                state = result.Result == E_RecordSubmissionResult.Submitted
                    ? E_ResultSubmissionState.Submitted : E_ResultSubmissionState.Rejected;
                reason = result.Reason;
            }
            _resultRecordPresentation = new ResultRecordPresentation(
                _resultRecordPresentation.IsNewLocalBest, candidate, state, reason,
                _leaderboardViewState.PersonalBestResult);
            ApplyResultRecordPresentation();
        }

        private E_GameMode GetCurrentResultGameMode()
        {
            RecordSubmissionCandidate candidate = _resultRecordPresentation.Candidate;
            if (candidate != null) return candidate.BoardKey.GameMode;
            if (_resultSystem != null && _resultSystem.CurrentResultData != null)
                return _resultSystem.CurrentResultData.GameMode;
            return _selectedGameMode;
        }

        private bool ContainsPendingSubmission(string submissionId)
        {
            if (_localRecordRepository == null) return false;
            var pending = _localRecordRepository.CreatePendingSnapshot();
            for (int i = 0; i < pending.Count; i++)
                if (pending[i].SubmissionId == submissionId) return true;
            return false;
        }

        private void ApplyResultRecordPresentation()
        {
            bool didChangeNavigation = _navigationState.SetResultSubmissionRetryAvailable(
                _resultRecordPresentation.SubmissionState == E_ResultSubmissionState.Pending);
            if (_uiManagementSystem != null)
                _uiManagementSystem.SetResultRecordPresentation(_resultRecordPresentation);
            if (didChangeNavigation && !_isResultPresentationDeferred) ApplyNavigationState();
        }

        private bool HasConfirmedOnlineRecoveryNotice()
        {
            return _localRecordRepository != null &&
                   _localRecordRepository.OnlineAccount.HasConfirmedRecoveryNotice;
        }

        private void SetOnlineRecoveryNoticeVisible(bool isVisible)
        {
            if (_uiManagementSystem != null)
                _uiManagementSystem.SetOnlineRecoveryNoticeVisible(isVisible);
        }

        private bool TryCreateInfiniteScoreLimit(
            ResultData resultData,
            out InfiniteScoreLimit scoreLimit)
        {
            scoreLimit = null;

            if (_playerMovementSystem == null || _infiniteModeSystem == null ||
                _runtimeData.CollectibleRuntimeData == null ||
                !InfiniteCollectibleLayout.TryGetMaximumCount(
                    out int maximumCollectiblesPerPattern))
            {
                return false;
            }

            scoreLimit = new InfiniteScoreLimit(
                resultData.ScoringVersion,
                _playerMovementSystem.FixedHorizontalSpeed,
                _infiniteModeSystem.ScorePerUnit,
                MomentumScoreState.MaximumMultiplier,
                InfinitePatternDefinition.PatternLength,
                maximumCollectiblesPerPattern,
                _runtimeData.CollectibleRuntimeData.ScorePerCollectible);
            return true;
        }

        private long GetPlayDurationMilliseconds()
        {
            if (_timerSystem == null || !_timerSystem.HasTimer(GetRunTimerKey()))
            {
                Debug.LogWarning("[GameSystem] Run timer missing; record candidate was not stored.");
                return -1;
            }
            double elapsedTime = _timerSystem.GetElapsedTime(GetRunTimerKey());

            if (double.IsNaN(elapsedTime) || double.IsInfinity(elapsedTime) ||
                elapsedTime < 0.0 || elapsedTime > long.MaxValue / 1000.0)
            {
                return -1;
            }

            return (long)Math.Round(
                elapsedTime * 1000.0,
                MidpointRounding.AwayFromZero);
        }

        private void StopRunTimer()
        {
            if (!_timerSystem.HasTimer(GetRunTimerKey()) ||
                !_timerSystem.TryGetTimerState(
                    GetRunTimerKey(),
                    out E_TimerState timerState) ||
                timerState == E_TimerState.Stopped)
            {
                return;
            }

            _timerSystem.StopTimer(GetRunTimerKey());
        }

        private E_TimerKey GetRunTimerKey()
        {
            return _runtimeData != null && _runtimeData.GameMode == E_GameMode.Infinite
                ? E_TimerKey.InfiniteRunTimer : E_TimerKey.PlayTimer;
        }

        private void RequestApplicationQuit()
        {
            _applicationQuitService.RequestQuit();
        }

        private void RemoveRunTimer()
        {
            if (_timerSystem != null &&
                _timerSystem.HasTimer(GetRunTimerKey()))
            {
                _timerSystem.RemoveTimer(GetRunTimerKey());
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
