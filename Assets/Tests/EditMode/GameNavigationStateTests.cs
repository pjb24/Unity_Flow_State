using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class GameNavigationStateTests
    {
        private GameNavigationState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new GameNavigationState();
        }

        [Test]
        public void CompleteBoot_InitialState_ShowsMainMenuWithoutRun()
        {
            Assert.That(_state.CompleteBoot(), Is.True);

            AssertState(
                E_NavigationScreen.MainMenu,
                E_GameState.None,
                E_NavigationItem.Play);
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);
        }

        [Test]
        public void CompleteBoot_DuplicateRequest_IsRejectedWithoutMutation()
        {
            Assert.That(_state.CompleteBoot(), Is.True);

            Assert.That(_state.CompleteBoot(), Is.False);

            AssertState(
                E_NavigationScreen.MainMenu,
                E_GameState.None,
                E_NavigationItem.Play);
        }

        [Test]
        public void MainMenu_Navigate_UsesHowToPlaySecondAndClampBoundaries()
        {
            CompleteBoot();

            Assert.That(_state.TryNavigate(-1, 0.0), Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Play));
            ReleaseNavigate(0.01);

            Assert.That(_state.TryNavigate(1, 0.02), Is.True);
            Assert.That(
                _state.CurrentSelection,
                Is.EqualTo(E_NavigationItem.HowToPlay));

            Assert.That(_state.TrySelect(E_NavigationItem.Quit), Is.True);
            ReleaseNavigate(0.03);
            Assert.That(_state.TryNavigate(1, 0.04), Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Quit));
        }

        [Test]
        public void ModeSelect_SubmitStage_RequestsRunBeforeCreatingIt()
        {
            OpenModeSelect();

            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.Initializing,
                E_GameState.Initializing,
                E_NavigationItem.None);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.IsRunStartRequested, Is.True);
        }

        [Test]
        public void ModeSelect_InvalidSelection_IsRejectedWithoutMutation()
        {
            OpenModeSelect();

            Assert.That(_state.TrySelect(E_NavigationItem.Quit), Is.False);
            AssertState(
                E_NavigationScreen.ModeSelect,
                E_GameState.None,
                E_NavigationItem.Stage);
            Assert.That(_state.TrySubmit(), Is.True);

            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Initializing));
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Stage));
        }

        [Test]
        public void ModeSelect_BackAndCancel_ReturnToMainMenuWithoutRun()
        {
            OpenModeSelect();
            Assert.That(_state.TrySelect(E_NavigationItem.Back), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.MainMenu,
                E_GameState.None,
                E_NavigationItem.Play);
            Assert.That(_state.HasRun, Is.False);

            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.ModeSelect));
            Assert.That(_state.TryCancel(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.MainMenu));
        }

        [Test]
        public void ModeSelect_LastModeIsRememberedDuringApplicationRun()
        {
            OpenModeSelect();
            Assert.That(_state.TrySelect(E_NavigationItem.Infinite), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.TryHandleStageEnded(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.MainMenu), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.ModeSelect));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Infinite));
        }

        [Test]
        public void InitializationFailure_ClearsRequestAndReturnsToMainMenu()
        {
            OpenModeSelect();
            Assert.That(_state.TrySubmit(), Is.True);

            Assert.That(_state.CompleteInitialization(false), Is.True);

            AssertState(
                E_NavigationScreen.MainMenu,
                E_GameState.None,
                E_NavigationItem.Play);
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.HasInitializationFailureNotice, Is.True);
        }

        [Test]
        public void PauseAndResume_PreserveSameRun()
        {
            StartRun(E_NavigationItem.Infinite);
            long runId = _state.CurrentRunId;

            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.Playing,
                E_GameState.Playing,
                E_NavigationItem.None);
            Assert.That(_state.HasRun, Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId));
        }

        [Test]
        public void InputPolicy_AllowsPlayerOnlyDuringPlaying()
        {
            Assert.That(_state.IsUIInputAllowed, Is.False);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);

            StartRun(E_NavigationItem.Stage);
            Assert.That(_state.IsUIInputAllowed, Is.True);
            Assert.That(_state.IsPlayerInputAllowed, Is.True);

            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.IsUIInputAllowed, Is.True);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);
        }

        [Test]
        public void PauseSettings_Back_ReturnsToPauseAndPreservesRun()
        {
            StartRun(E_NavigationItem.Stage);
            long runId = _state.CurrentRunId;
            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.Settings), Is.True);

            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Settings));
            Assert.That(_state.CurrentGameState, Is.EqualTo(E_GameState.Paused));
            Assert.That(_state.HasRun, Is.True);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);

            Assert.That(_state.TrySubmit(), Is.True);
            AssertState(
                E_NavigationScreen.Pause,
                E_GameState.Paused,
                E_NavigationItem.Settings);
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId));
        }

        [Test]
        public void PauseMainMenuConfirmation_CancelPreservesRun()
        {
            StartRun(E_NavigationItem.Stage);
            long runId = _state.CurrentRunId;
            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.MainMenu), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.PauseMainMenuConfirmation,
                E_GameState.Paused,
                E_NavigationItem.MainMenu);
            Assert.That(_state.TrySelect(E_NavigationItem.Cancel), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.Pause,
                E_GameState.Paused,
                E_NavigationItem.MainMenu);
            Assert.That(_state.HasRun, Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId));
        }

        [Test]
        public void PauseMainMenuConfirmation_DefaultSubmitClearsRunWithoutResult()
        {
            StartRun(E_NavigationItem.Stage);
            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.MainMenu), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.MainMenu,
                E_GameState.None,
                E_NavigationItem.Play);
            Assert.That(_state.HasRun, Is.False);
        }

        [Test]
        public void StageEndFromPaused_OverridesPauseAndShowsResult()
        {
            StartRun(E_NavigationItem.Stage);
            Assert.That(_state.TryPause(), Is.True);

            Assert.That(_state.TryHandleStageEnded(), Is.True);
            Assert.That(_state.TryPause(), Is.False);

            AssertState(
                E_NavigationScreen.Result,
                E_GameState.Ended,
                E_NavigationItem.Retry);
            Assert.That(_state.HasRun, Is.False);
        }

        [Test]
        public void ResultRetry_CreatesNewRunInSameModeOnlyAfterSuccess()
        {
            StartRun(E_NavigationItem.Infinite);
            long previousRunId = _state.CurrentRunId;
            Assert.That(_state.TryHandleStageEnded(), Is.True);

            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.CompleteInitialization(true), Is.True);

            Assert.That(_state.HasRun, Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(previousRunId + 1));
        }

        [Test]
        public void ResultMainMenuAndCancel_RespectResultContract()
        {
            StartRun(E_NavigationItem.Stage);
            Assert.That(_state.TryHandleStageEnded(), Is.True);

            Assert.That(_state.TryCancel(), Is.False);
            Assert.That(_state.TrySelect(E_NavigationItem.MainMenu), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            AssertState(
                E_NavigationScreen.MainMenu,
                E_GameState.None,
                E_NavigationItem.Play);
        }

        [Test]
        public void FeatureScreens_BackSupportsSubmitAndCancel()
        {
            CompleteBoot();
            Assert.That(_state.TrySelect(E_NavigationItem.Leaderboard), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(
                _state.CurrentScreen,
                Is.EqualTo(E_NavigationScreen.LeaderboardUnavailable));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Back));
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Leaderboard));

            Assert.That(_state.TrySelect(E_NavigationItem.HowToPlay), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.TryCancel(), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.HowToPlay));
        }

        [Test]
        public void NavigationRepeat_UsesConfiguredDelayIntervalAndNeutralReset()
        {
            CompleteBoot();

            Assert.That(_state.TryNavigate(1, 0.0), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.HowToPlay));
            Assert.That(_state.TryNavigate(1, 0.49), Is.False);
            Assert.That(_state.TryNavigate(1, 0.5), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Leaderboard));
            Assert.That(_state.TryNavigate(1, 0.59), Is.False);
            Assert.That(_state.TryNavigate(1, 0.6), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Settings));
            ReleaseNavigate(0.61);
            Assert.That(_state.TryNavigate(1, 0.62), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Quit));
        }

        [Test]
        public void SameInputSequence_SubmitAndClick_ExecutesOnlyOnce()
        {
            CompleteBoot();
            NavigationInput input = new NavigationInput(
                1,
                0,
                true,
                false,
                true,
                0.0);

            Assert.That(_state.TryProcessInput(input), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.ModeSelect));
            Assert.That(_state.TryProcessInput(input), Is.False);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.ModeSelect));
            Assert.That(_state.HasRun, Is.False);
        }

        [TestCase(false, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, true)]
        public void ShouldShowDifficulty_RequiresDevelopmentContextAndSetting(
            bool isEditor,
            bool isDevelopmentBuild,
            bool isDevelopmentDisplayEnabled)
        {
            bool result = GameNavigationState.ShouldShowDifficulty(
                isEditor,
                isDevelopmentBuild,
                isDevelopmentDisplayEnabled);

            bool expected = isDevelopmentDisplayEnabled &&
                            (isEditor || isDevelopmentBuild);
            Assert.That(result, Is.EqualTo(expected));
        }

        private void CompleteBoot()
        {
            Assert.That(_state.CompleteBoot(), Is.True);
        }

        private void OpenModeSelect()
        {
            CompleteBoot();
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.ModeSelect));
        }

        private void StartRun(E_NavigationItem mode)
        {
            OpenModeSelect();
            Assert.That(_state.TrySelect(mode), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CompleteInitialization(true), Is.True);
        }

        private void ReleaseNavigate(double time)
        {
            Assert.That(_state.TryNavigate(0, time), Is.False);
        }

        private void AssertState(
            E_NavigationScreen screen,
            E_GameState gameState,
            E_NavigationItem selection)
        {
            Assert.That(_state.CurrentScreen, Is.EqualTo(screen));
            Assert.That(_state.CurrentGameState, Is.EqualTo(gameState));
            Assert.That(_state.CurrentSelection, Is.EqualTo(selection));
        }
    }
}
