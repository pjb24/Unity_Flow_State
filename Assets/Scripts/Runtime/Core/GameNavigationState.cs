namespace FlowState.Runtime.Core
{
    public sealed class GameNavigationState
    {
        public const double NavigateRepeatDelay = 0.5;
        public const double NavigateRepeatInterval = 0.1;

        private E_NavigationScreen _settingsReturnScreen;
        private long _lastProcessedInputSequence = -1;
        private int _navigateDirection;
        private double _nextNavigateRepeatTime;
        private bool _isNavigateNeutral = true;
        private bool _hasLastSelectedMode;

        public E_NavigationScreen CurrentScreen { get; private set; } =
            E_NavigationScreen.Boot;

        public E_NavigationItem CurrentSelection { get; private set; } =
            E_NavigationItem.None;

        public E_GameState CurrentGameState { get; private set; } =
            E_GameState.None;

        public E_GameMode SelectedGameMode { get; private set; } =
            E_GameMode.Stage;

        public bool HasRun { get; private set; }

        public bool IsRunStartRequested { get; private set; }

        public long CurrentRunId { get; private set; }

        public bool HasInitializationFailureNotice { get; private set; }

        public bool IsApplicationQuitRequested { get; private set; }

        public bool IsPlayerInputAllowed =>
            CurrentScreen == E_NavigationScreen.Playing &&
            CurrentGameState == E_GameState.Playing &&
            HasRun;

        public bool IsUIInputAllowed =>
            CurrentScreen != E_NavigationScreen.Boot;

        public bool CompleteBoot()
        {
            if (CurrentScreen != E_NavigationScreen.Boot ||
                CurrentGameState != E_GameState.None)
            {
                return false;
            }

            SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Play);
            return true;
        }

        public bool TryProcessInput(NavigationInput input)
        {
            if (input.Sequence <= _lastProcessedInputSequence)
            {
                return false;
            }

            _lastProcessedInputSequence = input.Sequence;
            bool didChange = TryNavigate(
                input.VerticalDirection,
                input.Time);

            if (input.IsCancelPressed)
            {
                return TryCancel() || didChange;
            }

            if (input.IsSubmitPressed || input.IsClickPressed)
            {
                return TrySubmit() || didChange;
            }

            return didChange;
        }

        public bool TryNavigate(int direction, double time)
        {
            if (direction < -1 || direction > 1)
            {
                return false;
            }

            if (direction == 0)
            {
                _isNavigateNeutral = true;
                _navigateDirection = 0;
                return false;
            }

            if (_isNavigateNeutral || direction != _navigateDirection)
            {
                _isNavigateNeutral = false;
                _navigateDirection = direction;
                _nextNavigateRepeatTime = time + NavigateRepeatDelay;
                return TryMoveSelection(direction);
            }

            if (time < _nextNavigateRepeatTime)
            {
                return false;
            }

            _nextNavigateRepeatTime = time + NavigateRepeatInterval;
            return TryMoveSelection(direction);
        }

        public bool TrySelect(E_NavigationItem item)
        {
            if (!ContainsItem(CurrentScreen, item))
            {
                return false;
            }

            CurrentSelection = item;
            return true;
        }

        public bool TrySubmit()
        {
            switch (CurrentScreen)
            {
                case E_NavigationScreen.MainMenu:
                    return SubmitMainMenu();

                case E_NavigationScreen.ModeSelect:
                    return SubmitModeSelect();

                case E_NavigationScreen.Pause:
                    return SubmitPause();

                case E_NavigationScreen.PauseMainMenuConfirmation:
                    return SubmitPauseMainMenuConfirmation();

                case E_NavigationScreen.Result:
                    return SubmitResult();

                case E_NavigationScreen.LeaderboardUnavailable:
                case E_NavigationScreen.HowToPlay:
                    return ReturnToMainMenu(CurrentSelection);

                case E_NavigationScreen.Settings:
                    return ReturnFromSettings();

                default:
                    return false;
            }
        }

        public bool TryCancel()
        {
            switch (CurrentScreen)
            {
                case E_NavigationScreen.MainMenu:
                case E_NavigationScreen.Result:
                    return false;

                case E_NavigationScreen.ModeSelect:
                    SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Play);
                    return true;

                case E_NavigationScreen.Pause:
                    return ResumeRun();

                case E_NavigationScreen.PauseMainMenuConfirmation:
                    SetScreen(E_NavigationScreen.Pause, E_NavigationItem.MainMenu);
                    return true;

                case E_NavigationScreen.LeaderboardUnavailable:
                case E_NavigationScreen.HowToPlay:
                    return ReturnToMainMenu(CurrentSelection);

                case E_NavigationScreen.Settings:
                    return ReturnFromSettings();

                default:
                    return false;
            }
        }

        public bool CompleteInitialization(bool isSuccess)
        {
            if (CurrentScreen != E_NavigationScreen.Initializing ||
                CurrentGameState != E_GameState.Initializing ||
                !IsRunStartRequested)
            {
                return false;
            }

            IsRunStartRequested = false;

            if (!isSuccess)
            {
                ClearRun();
                HasInitializationFailureNotice = true;
                CurrentGameState = E_GameState.None;
                SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Play);
                return true;
            }

            HasInitializationFailureNotice = false;
            HasRun = true;
            CurrentRunId++;
            CurrentGameState = E_GameState.Playing;
            SetScreen(E_NavigationScreen.Playing, E_NavigationItem.None);
            return true;
        }

        public bool TryPause()
        {
            if (CurrentScreen != E_NavigationScreen.Playing ||
                CurrentGameState != E_GameState.Playing || !HasRun)
            {
                return false;
            }

            CurrentGameState = E_GameState.Paused;
            SetScreen(E_NavigationScreen.Pause, E_NavigationItem.Resume);
            return true;
        }

        public bool TryHandleStageEnded()
        {
            if ((CurrentScreen != E_NavigationScreen.Playing &&
                 CurrentScreen != E_NavigationScreen.Pause) ||
                !HasRun)
            {
                return false;
            }

            ClearRun();
            CurrentGameState = E_GameState.Ended;
            SetScreen(E_NavigationScreen.Result, E_NavigationItem.Retry);
            return true;
        }

        public static bool ShouldShowDifficulty(
            bool isEditor,
            bool isDevelopmentBuild,
            bool isDevelopmentDisplayEnabled)
        {
            return isDevelopmentDisplayEnabled &&
                   (isEditor || isDevelopmentBuild);
        }

        private bool SubmitMainMenu()
        {
            switch (CurrentSelection)
            {
                case E_NavigationItem.Play:
                    SetScreen(
                        E_NavigationScreen.ModeSelect,
                        _hasLastSelectedMode
                            ? ToNavigationItem(SelectedGameMode)
                            : E_NavigationItem.Stage);
                    return true;

                case E_NavigationItem.HowToPlay:
                    SetScreen(E_NavigationScreen.HowToPlay, E_NavigationItem.Back);
                    return true;

                case E_NavigationItem.Leaderboard:
                    SetScreen(
                        E_NavigationScreen.LeaderboardUnavailable,
                        E_NavigationItem.Back);
                    return true;

                case E_NavigationItem.Settings:
                    _settingsReturnScreen = E_NavigationScreen.MainMenu;
                    SetScreen(E_NavigationScreen.Settings, E_NavigationItem.Back);
                    return true;

                case E_NavigationItem.Quit:
                    IsApplicationQuitRequested = true;
                    return true;

                default:
                    return false;
            }
        }

        private bool SubmitModeSelect()
        {
            if (CurrentSelection == E_NavigationItem.Back)
            {
                SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Play);
                return true;
            }

            if (CurrentSelection != E_NavigationItem.Stage &&
                CurrentSelection != E_NavigationItem.Infinite)
            {
                return false;
            }

            SelectedGameMode = CurrentSelection == E_NavigationItem.Stage
                ? E_GameMode.Stage
                : E_GameMode.Infinite;
            _hasLastSelectedMode = true;
            BeginInitialization();
            return true;
        }

        private bool SubmitPause()
        {
            switch (CurrentSelection)
            {
                case E_NavigationItem.Resume:
                    return ResumeRun();

                case E_NavigationItem.Retry:
                    ClearRun();
                    BeginInitialization();
                    return true;

                case E_NavigationItem.Settings:
                    _settingsReturnScreen = E_NavigationScreen.Pause;
                    SetScreen(E_NavigationScreen.Settings, E_NavigationItem.Back);
                    return true;

                case E_NavigationItem.MainMenu:
                    SetScreen(
                        E_NavigationScreen.PauseMainMenuConfirmation,
                        E_NavigationItem.MainMenu);
                    return true;

                default:
                    return false;
            }
        }

        private bool SubmitPauseMainMenuConfirmation()
        {
            if (CurrentSelection == E_NavigationItem.MainMenu)
            {
                ClearRun();
                CurrentGameState = E_GameState.None;
                SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Play);
                return true;
            }

            if (CurrentSelection == E_NavigationItem.Cancel)
            {
                SetScreen(E_NavigationScreen.Pause, E_NavigationItem.MainMenu);
                return true;
            }

            return false;
        }

        private bool SubmitResult()
        {
            if (CurrentSelection == E_NavigationItem.Retry)
            {
                BeginInitialization();
                return true;
            }

            if (CurrentSelection == E_NavigationItem.MainMenu)
            {
                ClearRun();
                CurrentGameState = E_GameState.None;
                SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Play);
                return true;
            }

            return false;
        }

        private bool ResumeRun()
        {
            if (!HasRun || CurrentGameState != E_GameState.Paused)
            {
                return false;
            }

            CurrentGameState = E_GameState.Playing;
            SetScreen(E_NavigationScreen.Playing, E_NavigationItem.None);
            return true;
        }

        private bool ReturnFromSettings()
        {
            if (CurrentSelection != E_NavigationItem.Back)
            {
                return false;
            }

            if (_settingsReturnScreen == E_NavigationScreen.Pause)
            {
                SetScreen(E_NavigationScreen.Pause, E_NavigationItem.Settings);
                return true;
            }

            if (_settingsReturnScreen == E_NavigationScreen.MainMenu)
            {
                SetScreen(E_NavigationScreen.MainMenu, E_NavigationItem.Settings);
                return true;
            }

            return false;
        }

        private bool ReturnToMainMenu(E_NavigationItem item)
        {
            if (item != E_NavigationItem.Back)
            {
                return false;
            }

            E_NavigationItem returnSelection =
                CurrentScreen == E_NavigationScreen.HowToPlay
                    ? E_NavigationItem.HowToPlay
                    : E_NavigationItem.Leaderboard;
            SetScreen(E_NavigationScreen.MainMenu, returnSelection);
            return true;
        }

        private void BeginInitialization()
        {
            ClearRun();
            HasInitializationFailureNotice = false;
            IsRunStartRequested = true;
            CurrentGameState = E_GameState.Initializing;
            SetScreen(E_NavigationScreen.Initializing, E_NavigationItem.None);
        }

        private void ClearRun()
        {
            HasRun = false;
            IsRunStartRequested = false;
        }

        private bool TryMoveSelection(int direction)
        {
            int count = GetItemCount(CurrentScreen);
            int currentIndex = GetItemIndex(CurrentScreen, CurrentSelection);

            if (count == 0 || currentIndex < 0)
            {
                return false;
            }

            int nextIndex = currentIndex + direction;

            if (nextIndex < 0 || nextIndex >= count)
            {
                return false;
            }

            CurrentSelection = GetItemAt(CurrentScreen, nextIndex);
            return true;
        }

        private static E_NavigationItem ToNavigationItem(E_GameMode gameMode)
        {
            return gameMode == E_GameMode.Infinite
                ? E_NavigationItem.Infinite
                : E_NavigationItem.Stage;
        }

        private void SetScreen(
            E_NavigationScreen screen,
            E_NavigationItem selection)
        {
            CurrentScreen = screen;
            CurrentSelection = selection;
            _isNavigateNeutral = true;
            _navigateDirection = 0;
        }

        private static bool ContainsItem(
            E_NavigationScreen screen,
            E_NavigationItem item)
        {
            return GetItemIndex(screen, item) >= 0;
        }

        private static int GetItemCount(E_NavigationScreen screen)
        {
            switch (screen)
            {
                case E_NavigationScreen.MainMenu:
                    return 5;

                case E_NavigationScreen.ModeSelect:
                    return 3;

                case E_NavigationScreen.Pause:
                    return 4;

                case E_NavigationScreen.PauseMainMenuConfirmation:
                case E_NavigationScreen.Result:
                    return 2;

                case E_NavigationScreen.LeaderboardUnavailable:
                case E_NavigationScreen.HowToPlay:
                case E_NavigationScreen.Settings:
                    return 1;

                default:
                    return 0;
            }
        }

        private static int GetItemIndex(
            E_NavigationScreen screen,
            E_NavigationItem item)
        {
            for (int i = 0; i < GetItemCount(screen); i++)
            {
                if (GetItemAt(screen, i) == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private static E_NavigationItem GetItemAt(
            E_NavigationScreen screen,
            int index)
        {
            switch (screen)
            {
                case E_NavigationScreen.MainMenu:
                    switch (index)
                    {
                        case 0: return E_NavigationItem.Play;
                        case 1: return E_NavigationItem.HowToPlay;
                        case 2: return E_NavigationItem.Leaderboard;
                        case 3: return E_NavigationItem.Settings;
                        case 4: return E_NavigationItem.Quit;
                    }
                    break;

                case E_NavigationScreen.ModeSelect:
                    switch (index)
                    {
                        case 0: return E_NavigationItem.Stage;
                        case 1: return E_NavigationItem.Infinite;
                        case 2: return E_NavigationItem.Back;
                    }
                    break;

                case E_NavigationScreen.Pause:
                    switch (index)
                    {
                        case 0: return E_NavigationItem.Resume;
                        case 1: return E_NavigationItem.Retry;
                        case 2: return E_NavigationItem.Settings;
                        case 3: return E_NavigationItem.MainMenu;
                    }
                    break;

                case E_NavigationScreen.PauseMainMenuConfirmation:
                    return index == 0
                        ? E_NavigationItem.MainMenu
                        : E_NavigationItem.Cancel;

                case E_NavigationScreen.Result:
                    return index == 0
                        ? E_NavigationItem.Retry
                        : E_NavigationItem.MainMenu;

                case E_NavigationScreen.LeaderboardUnavailable:
                case E_NavigationScreen.HowToPlay:
                case E_NavigationScreen.Settings:
                    return E_NavigationItem.Back;
            }

            return E_NavigationItem.None;
        }
    }
}
