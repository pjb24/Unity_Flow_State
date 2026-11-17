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
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
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
        public void Play_TutorialIncomplete_OpensInfiniteGuideWithoutRun()
        {
            OpenAutomaticGuide();
            AssertState(E_NavigationScreen.AutomaticHowToPlay,
                E_GameState.None, E_NavigationItem.StartRun);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.HasPendingGameMode, Is.True);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.False);
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.CurrentRunId, Is.Zero);
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);
            Assert.That(_state.IsUIInputAllowed, Is.True);
        }

        [Test]
        public void Play_TutorialRestored_RequestsInfiniteWithoutModeSelectOrGuide()
        {
            _state.RestoreAutomaticHowToPlayCompleted(true);
            CompleteBoot();
            Assert.That(_state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.Initializing,
                E_GameState.Initializing, E_NavigationItem.None);
            Assert.That(_state.IsRunStartRequested, Is.True);
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.HasPendingGameMode, Is.False);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.True);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.CurrentRunId, Is.Zero);
        }

        [TestCase(E_NavigationItem.Stage)]
        [TestCase((E_NavigationItem)999)]
        public void ExplicitPlaySelection_IsRejectedWithoutChangingGuide(E_NavigationItem item)
        {
            OpenAutomaticGuide();
            Assert.That(_state.TrySelect(item), Is.False);
            AssertState(E_NavigationScreen.AutomaticHowToPlay,
                E_GameState.None, E_NavigationItem.StartRun);
            Assert.That(_state.HasPendingGameMode, Is.True);
            Assert.That(_state.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.CurrentRunId, Is.Zero);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.False);
        }

        [Test]
        public void AutomaticHowToPlay_StartRun_RequestsInfiniteOnce()
        {
            OpenAutomaticGuide();
            Assert.That(_state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.Initializing,
                E_GameState.Initializing, E_NavigationItem.None);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.HasPendingGameMode, Is.False);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.True);
            Assert.That(_state.IsRunStartRequested, Is.True);
            Assert.That(_state.TrySubmit(), Is.False);
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.CompleteInitialization(true), Is.False);
            Assert.That(_state.CurrentRunId, Is.EqualTo(1));
        }

        [Test]
        public void AutomaticHowToPlay_Cancel_IsRejectedWithoutMutation()
        {
            OpenAutomaticGuide();
            Assert.That(_state.TryCancel(), Is.False);
            AssertState(E_NavigationScreen.AutomaticHowToPlay,
                E_GameState.None, E_NavigationItem.StartRun);
            Assert.That(_state.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.HasPendingGameMode, Is.True);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.False);
        }

        [Test]
        public void InitializationFailure_PreservesCompletionAndReentryRequestsInfinite()
        {
            OpenAutomaticGuide();
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CompleteInitialization(false), Is.True);
            AssertState(E_NavigationScreen.MainMenu, E_GameState.None, E_NavigationItem.Play);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.True);
            Assert.That(_state.HasPendingGameMode, Is.False);
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.HasRun, Is.False);
            Assert.That(_state.CurrentRunId, Is.Zero);
            Assert.That(_state.HasInitializationFailureNotice, Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.Initializing,
                E_GameState.Initializing, E_NavigationItem.None);
            Assert.That(_state.HasInitializationFailureNotice, Is.False);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(1));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void MainMenuHowToPlay_Return_PreservesTutorialAndSelection(bool completed, bool cancel)
        {
            _state.RestoreAutomaticHowToPlayCompleted(completed);
            CompleteBoot();
            Assert.That(_state.TrySelect(E_NavigationItem.HowToPlay), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(cancel ? _state.TryCancel() : _state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.MainMenu,
                E_GameState.None, E_NavigationItem.HowToPlay);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.EqualTo(completed));
            Assert.That(_state.HasPendingGameMode, Is.False);
            Assert.That(_state.IsRunStartRequested, Is.False);
        }

        [Test]
        public void LegacyStageSelection_Play_NormalizesToInfiniteGuide()
        {
            SetLegacyProperty("SelectedGameMode", E_GameMode.Stage);
            SetLegacyField("_pendingGameMode", E_GameMode.Stage);
            OpenAutomaticGuide();
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.CurrentRunId, Is.Zero);
        }

        [Test]
        public void LegacyStagePending_StartRun_NormalizesBeforeRequest()
        {
            OpenAutomaticGuide();
            SetLegacyField("_pendingGameMode", E_GameMode.Stage);
            SetLegacyProperty("SelectedGameMode", E_GameMode.Stage);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.IsRunStartRequested, Is.True);
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(1));
        }

        [Test]
        public void UnknownPending_StartRun_IsRejectedWithoutMutation()
        {
            OpenAutomaticGuide();
            SetLegacyField("_pendingGameMode", (E_GameMode)999);
            Assert.That(_state.TrySubmit(), Is.False);
            AssertState(E_NavigationScreen.AutomaticHowToPlay,
                E_GameState.None, E_NavigationItem.StartRun);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.False);
            Assert.That(_state.PendingGameMode, Is.EqualTo((E_GameMode)999));
            Assert.That(_state.HasPendingGameMode, Is.True);
            Assert.That(_state.IsRunStartRequested, Is.False);
        }

        [Test]
        public void LegacyModeSelect_StageSubmit_IsRejectedWithoutMutation()
        {
            CompleteBoot();
            SetLegacyProperty("CurrentScreen", E_NavigationScreen.ModeSelect);
            SetLegacyProperty("CurrentSelection", E_NavigationItem.Stage);
            Assert.That(_state.TrySelect(E_NavigationItem.Stage), Is.False);
            Assert.That(_state.TrySubmit(), Is.False);
            AssertState(E_NavigationScreen.ModeSelect, E_GameState.None, E_NavigationItem.Stage);
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.HasPendingGameMode, Is.False);
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.False);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void LegacyModeSelect_BackOrCancel_ReturnsToPlay(bool cancel)
        {
            CompleteBoot();
            SetLegacyProperty("CurrentScreen", E_NavigationScreen.ModeSelect);
            Assert.That(_state.TrySelect(E_NavigationItem.Back), Is.True);
            Assert.That(cancel ? _state.TryCancel() : _state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.MainMenu, E_GameState.None, E_NavigationItem.Play);
            Assert.That(_state.HasRun, Is.False);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void LegacyModeSelect_Infinite_UsesCurrentTutorialContract(bool completed)
        {
            CompleteBoot();
            _state.RestoreAutomaticHowToPlayCompleted(completed);
            SetLegacyProperty("CurrentScreen", E_NavigationScreen.ModeSelect);
            Assert.That(_state.TrySelect(E_NavigationItem.Infinite), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(completed
                ? E_NavigationScreen.Initializing : E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.IsRunStartRequested, Is.EqualTo(completed));
            Assert.That(_state.HasPendingGameMode, Is.EqualTo(!completed));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void LegacyStageSelection_Retry_RequestsInfinite(bool pause)
        {
            StartRun();
            long previousRunId = _state.CurrentRunId;
            Assert.That(pause ? _state.TryPause() : _state.TryHandleStageEnded(), Is.True);
            SetLegacyProperty("SelectedGameMode", E_GameMode.Stage);
            Assert.That(_state.TrySelect(E_NavigationItem.Retry), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            if (pause)
            {
                Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.PauseRetryConfirmation));
                Assert.That(_state.IsRunStartRequested, Is.False);
                Assert.That(_state.TrySelect(E_NavigationItem.Retry), Is.True);
                Assert.That(_state.TrySubmit(), Is.True);
            }
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.IsRunStartRequested, Is.True);
            Assert.That(_state.TrySubmit(), Is.False);
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(previousRunId + 1));
        }

        [Test]
        public void PauseAndResume_PreserveSameRun()
        {
            StartRun();
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

            StartRun();
            Assert.That(_state.IsUIInputAllowed, Is.True);
            Assert.That(_state.IsPlayerInputAllowed, Is.True);

            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.IsUIInputAllowed, Is.True);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);
        }

        [Test]
        public void PauseSettings_Back_ReturnsToPauseAndPreservesRun()
        {
            StartRun();
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
            StartRun();
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

        [TestCase(false)]
        [TestCase(true)]
        public void PauseRetryConfirmation_CancelPreservesRunAndReturnsToRetry(bool cancelInput)
        {
            StartRun();
            long runId = _state.CurrentRunId;
            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.Retry), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.PauseRetryConfirmation, E_GameState.Paused, E_NavigationItem.Cancel);
            Assert.That(_state.HasRun, Is.True);
            Assert.That(_state.IsRunStartRequested, Is.False);
            Assert.That(_state.IsPlayerInputAllowed, Is.False);
            Assert.That(cancelInput ? _state.TryCancel() : _state.TrySubmit(), Is.True);
            AssertState(E_NavigationScreen.Pause, E_GameState.Paused, E_NavigationItem.Retry);
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId));
            Assert.That(_state.HasRun, Is.True);
            Assert.That(_state.IsRunStartRequested, Is.False);
        }

        [Test]
        public void PauseRetryConfirmation_ConfirmRequestsExactlyOneNewRun()
        {
            StartRun();
            long runId = _state.CurrentRunId;
            Assert.That(_state.TryPause(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.Retry), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.TryNavigate(-1, 0.0), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Retry));
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId));
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.IsRunStartRequested, Is.True);
            Assert.That(_state.TrySubmit(), Is.False);
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId + 1));
        }

        [Test]
        public void PauseMainMenuConfirmation_DefaultSubmitClearsRunWithoutResult()
        {
            StartRun();
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
            StartRun();
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
            StartRun();
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
            StartRun();
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
                Is.EqualTo(E_NavigationScreen.Leaderboard));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Infinite));
            Assert.That(_state.TrySelect(E_NavigationItem.Back), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Leaderboard));

            Assert.That(_state.TrySelect(E_NavigationItem.HowToPlay), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.TryCancel(), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.HowToPlay));
        }

        [Test]
        public void Leaderboard_ResultEntryBack_ReturnsToResultSelection()
        {
            StartRun();
            Assert.That(_state.TryHandleStageEnded(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.ResultLeaderboard), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Leaderboard));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Infinite));
            Assert.That(_state.TryCancel(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Result));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.ResultLeaderboard));
        }

        [Test]
        public void ResultSubmissionRetry_IsNavigableOnlyWhilePending()
        {
            StartRun();
            Assert.That(_state.TryHandleStageEnded(), Is.True);
            Assert.That(_state.SetResultSubmissionRetryAvailable(true), Is.True);
            Assert.That(_state.TryNavigate(1, 0.0), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.ResultLeaderboard));
            ReleaseNavigate(0.01);
            Assert.That(_state.TryNavigate(1, 0.02), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.SubmissionRetry));
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Result));
            Assert.That(_state.SetResultSubmissionRetryAvailable(false), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.ResultLeaderboard));
            ReleaseNavigate(0.03);
            Assert.That(_state.TryNavigate(1, 0.04), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.MainMenu));
        }

        [Test]
        public void Leaderboard_StageSelectionIsRejectedAndInfiniteDoesNotStartRun()
        {
            CompleteBoot();
            Assert.That(_state.TrySelect(E_NavigationItem.Leaderboard), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Infinite));
            Assert.That(_state.TrySelect(E_NavigationItem.Stage), Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Infinite));
            Assert.That(_state.TryNavigate(-1, 0.0), Is.False);
            Assert.That(_state.TrySelect(E_NavigationItem.Infinite), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);

            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Leaderboard));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Infinite));
            Assert.That(_state.IsRunStartRequested, Is.False);
        }

        [Test]
        public void Leaderboard_PendingRetry_IsSelectableWithoutStartingRun()
        {
            CompleteBoot();
            Assert.That(_state.TrySelect(E_NavigationItem.Leaderboard), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.SetLeaderboardPendingRetryAvailable(true), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.LeaderboardPendingRetry), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.Leaderboard));
            Assert.That(_state.IsRunStartRequested, Is.False);
        }

        [Test]
        public void Leaderboard_KeyboardNavigation_ReachesPendingRetryAndBack()
        {
            CompleteBoot();
            Assert.That(_state.TrySelect(E_NavigationItem.Leaderboard), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.SetLeaderboardPendingRetryAvailable(true), Is.True);
            Assert.That(_state.TryNavigate(1, 0.0), Is.True);
            ReleaseNavigate(0.01);
            Assert.That(_state.TryNavigate(1, 0.02), Is.True);
            Assert.That(_state.CurrentSelection,
                Is.EqualTo(E_NavigationItem.LeaderboardPendingRetry));
            ReleaseNavigate(0.03);
            Assert.That(_state.TryNavigate(1, 0.04), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Back));
        }

        [Test]
        public void Leaderboard_PendingRetry_IsSkippedWhenNoPendingRemains()
        {
            CompleteBoot();
            Assert.That(_state.TrySelect(E_NavigationItem.Leaderboard), Is.True);
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.LeaderboardPendingRetry), Is.False);
            Assert.That(_state.SetLeaderboardPendingRetryAvailable(true), Is.True);
            Assert.That(_state.TrySelect(E_NavigationItem.LeaderboardPendingRetry), Is.True);
            Assert.That(_state.SetLeaderboardPendingRetryAvailable(false), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.LeaderboardRetry));
            Assert.That(_state.TrySelect(E_NavigationItem.LeaderboardPendingRetry), Is.False);
            ReleaseNavigate(0.01);
            Assert.That(_state.TryNavigate(1, 0.02), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_NavigationItem.Back));
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
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(_state.TryProcessInput(input), Is.False);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(_state.HasRun, Is.False);
        }

        [TestCase(0.0)]
        [TestCase(0.016)]
        public void StartRun_RepeatedSequenceAndAdjacentInput_CreateOneRun(double delay)
        {
            OpenAutomaticGuide();
            NavigationInput first = new NavigationInput(1, 0, true, false, true, 0.0);
            NavigationInput next = new NavigationInput(2, 0, true, false, true, delay);
            Assert.That(_state.TryProcessInput(first), Is.True);
            Assert.That(_state.TryProcessInput(first), Is.False);
            Assert.That(_state.TryProcessInput(next), Is.False);
            Assert.That(_state.CurrentRunId, Is.Zero);
            Assert.That(_state.IsRunStartRequested, Is.True);
            Assert.That(_state.CompleteInitialization(true), Is.True);
            Assert.That(_state.TryProcessInput(next), Is.False);
            Assert.That(_state.TryProcessInput(
                new NavigationInput(3, 0, true, false, true, delay + 0.016)), Is.False);
            Assert.That(_state.CompleteInitialization(true), Is.False);
            Assert.That(_state.CurrentRunId, Is.EqualTo(1));
            Assert.That(_state.SelectedGameMode, Is.EqualTo(E_GameMode.Infinite));
        }

        [TestCase(E_NavigationScreen.MainMenu)]
        [TestCase(E_NavigationScreen.AutomaticHowToPlay)]
        [TestCase(E_NavigationScreen.Initializing)]
        [TestCase(E_NavigationScreen.Playing)]
        [TestCase(E_NavigationScreen.Pause)]
        [TestCase(E_NavigationScreen.Result)]
        public void StagePlaySelection_IsRejectedAndPreservesExistingState(E_NavigationScreen screen)
        {
            CompleteBoot();
            if (screen != E_NavigationScreen.MainMenu) Assert.That(_state.TrySubmit(), Is.True);
            if (screen != E_NavigationScreen.MainMenu &&
                screen != E_NavigationScreen.AutomaticHowToPlay)
                Assert.That(_state.TrySubmit(), Is.True);
            if (screen == E_NavigationScreen.Playing || screen == E_NavigationScreen.Pause ||
                screen == E_NavigationScreen.Result)
                Assert.That(_state.CompleteInitialization(true), Is.True);
            if (screen == E_NavigationScreen.Pause) Assert.That(_state.TryPause(), Is.True);
            if (screen == E_NavigationScreen.Result) Assert.That(_state.TryHandleStageEnded(), Is.True);

            E_NavigationItem selection = _state.CurrentSelection;
            E_GameState gameState = _state.CurrentGameState;
            long runId = _state.CurrentRunId;
            bool hasRun = _state.HasRun;
            bool hasRequest = _state.IsRunStartRequested;
            bool completed = _state.HasAutomaticHowToPlayCompleted;
            Assert.That(_state.TrySelect(E_NavigationItem.Stage), Is.False);
            AssertState(screen, gameState, selection);
            Assert.That(_state.CurrentRunId, Is.EqualTo(runId));
            Assert.That(_state.HasRun, Is.EqualTo(hasRun));
            Assert.That(_state.IsRunStartRequested, Is.EqualTo(hasRequest));
            Assert.That(_state.HasAutomaticHowToPlayCompleted, Is.EqualTo(completed));
        }

        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, true, true)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, true)]
        public void ShouldShowDifficulty_RequiresDevelopmentContextAndSetting(
            bool isEditor,
            bool isDevelopmentBuild,
            bool isDevelopmentDisplayEnabled,
            bool expected)
        {
            bool result = GameNavigationState.ShouldShowDifficulty(
                isEditor,
                isDevelopmentBuild,
                isDevelopmentDisplayEnabled);

            Assert.That(result, Is.EqualTo(expected));
        }

        private void CompleteBoot()
        {
            Assert.That(_state.CompleteBoot(), Is.True);
        }

        private void OpenAutomaticGuide()
        {
            CompleteBoot();
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CurrentScreen, Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
        }

        private void StartRun()
        {
            OpenAutomaticGuide();
            Assert.That(_state.TrySubmit(), Is.True);
            Assert.That(_state.CompleteInitialization(true), Is.True);
        }

        private void SetLegacyProperty(string name, object value)
        {
            System.Reflection.PropertyInfo property = typeof(GameNavigationState).GetProperty(name);
            Assert.That(property, Is.Not.Null);
            property.GetSetMethod(true).Invoke(_state, new[] { value });
        }

        private void SetLegacyField(string name, object value)
        {
            System.Reflection.FieldInfo field = typeof(GameNavigationState).GetField(name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(_state, value);
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
