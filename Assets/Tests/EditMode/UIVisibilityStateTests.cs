using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class UIVisibilityStateTests
    {
        private UIVisibilityState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new UIVisibilityState();
        }

        [Test]
        public void NewState_HidesEveryUI()
        {
            AssertEveryUIHidden();
        }

        [Test]
        public void Apply_StagePlaying_ShowsOnlyStageHud()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Stage,
                    E_GameState.Playing,
                    E_UIState.StageHud),
                Is.True);

            Assert.That(_state.IsStageHudVisible, Is.True);
            Assert.That(_state.IsInfiniteHudVisible, Is.False);
            AssertPanelsHidden();
        }

        [Test]
        public void Apply_InfinitePlaying_ShowsOnlyInfiniteHud()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Infinite,
                    E_GameState.Playing,
                    E_UIState.StageHud),
                Is.True);

            Assert.That(_state.IsStageHudVisible, Is.False);
            Assert.That(_state.IsInfiniteHudVisible, Is.True);
            AssertPanelsHidden();
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase(E_GameMode.Infinite)]
        public void Apply_Paused_ShowsCurrentModeHudAndPausePanel(
            E_GameMode gameMode)
        {
            Assert.That(
                _state.Apply(
                    gameMode,
                    E_GameState.Paused,
                    E_UIState.Pause),
                Is.True);

            AssertCurrentModeHud(gameMode);
            Assert.That(_state.IsPausePanelVisible, Is.True);
            Assert.That(_state.IsResultPanelVisible, Is.False);
            Assert.That(_state.IsStageResultContentVisible, Is.False);
            Assert.That(_state.IsInfiniteResultContentVisible, Is.False);
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase(E_GameMode.Infinite)]
        public void Apply_Ending_KeepsOnlyCurrentModeHud(E_GameMode gameMode)
        {
            Assert.That(
                _state.Apply(
                    gameMode,
                    E_GameState.Ending,
                    E_UIState.Result),
                Is.True);

            AssertCurrentModeHud(gameMode);
            AssertPanelsHidden();
        }

        [Test]
        public void Apply_StageEnded_ShowsStageHudAndStageResult()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Stage,
                    E_GameState.Ended,
                    E_UIState.Result),
                Is.True);

            Assert.That(_state.IsStageHudVisible, Is.True);
            Assert.That(_state.IsInfiniteHudVisible, Is.False);
            Assert.That(_state.IsPausePanelVisible, Is.False);
            Assert.That(_state.IsResultPanelVisible, Is.True);
            Assert.That(_state.IsStageResultContentVisible, Is.True);
            Assert.That(_state.IsInfiniteResultContentVisible, Is.False);
        }

        [Test]
        public void Apply_InfiniteEnded_ShowsInfiniteHudAndInfiniteResult()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Infinite,
                    E_GameState.Ended,
                    E_UIState.Result),
                Is.True);

            Assert.That(_state.IsStageHudVisible, Is.False);
            Assert.That(_state.IsInfiniteHudVisible, Is.True);
            Assert.That(_state.IsPausePanelVisible, Is.False);
            Assert.That(_state.IsResultPanelVisible, Is.True);
            Assert.That(_state.IsStageResultContentVisible, Is.False);
            Assert.That(_state.IsInfiniteResultContentVisible, Is.True);
        }

        [Test]
        public void Apply_EndedWithoutResultState_KeepsHudAndHidesResult()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Infinite,
                    E_GameState.Ended,
                    E_UIState.None),
                Is.True);

            Assert.That(_state.IsInfiniteHudVisible, Is.True);
            AssertPanelsHidden();
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Initializing)]
        [TestCase(E_GameState.Ready)]
        public void Apply_PrePlayState_HidesEveryUI(E_GameState gameState)
        {
            _state.Apply(
                E_GameMode.Stage,
                E_GameState.Ended,
                E_UIState.Result);

            Assert.That(
                _state.Apply(E_GameMode.Infinite, gameState, E_UIState.None),
                Is.True);

            AssertEveryUIHidden();
        }

        [Test]
        public void Apply_NewRunAfterResult_ResetsPreviousModeVisibility()
        {
            _state.Apply(
                E_GameMode.Stage,
                E_GameState.Ended,
                E_UIState.Result);

            Assert.That(
                _state.Apply(
                    E_GameMode.Infinite,
                    E_GameState.Playing,
                    E_UIState.StageHud),
                Is.True);

            Assert.That(_state.IsStageHudVisible, Is.False);
            Assert.That(_state.IsInfiniteHudVisible, Is.True);
            AssertPanelsHidden();
        }

        [Test]
        public void Reset_VisibleState_HidesEveryUI()
        {
            _state.Apply(
                E_GameMode.Stage,
                E_GameState.Ended,
                E_UIState.Result);

            _state.Reset();

            AssertEveryUIHidden();
        }

        [Test]
        public void Apply_InvalidMode_IsRejectedAndHidesEveryUI()
        {
            _state.Apply(
                E_GameMode.Stage,
                E_GameState.Ended,
                E_UIState.Result);

            Assert.That(
                _state.Apply(
                    (E_GameMode)999,
                    E_GameState.Playing,
                    E_UIState.StageHud),
                Is.False);

            AssertEveryUIHidden();
        }

        [Test]
        public void Apply_InvalidGameState_IsRejectedAndHidesEveryUI()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Stage,
                    (E_GameState)999,
                    E_UIState.StageHud),
                Is.False);

            AssertEveryUIHidden();
        }

        [Test]
        public void Apply_InvalidUIState_IsRejectedAndHidesEveryUI()
        {
            Assert.That(
                _state.Apply(
                    E_GameMode.Stage,
                    E_GameState.Playing,
                    (E_UIState)999),
                Is.False);

            AssertEveryUIHidden();
        }

        private void AssertCurrentModeHud(E_GameMode gameMode)
        {
            Assert.That(
                _state.IsStageHudVisible,
                Is.EqualTo(gameMode == E_GameMode.Stage));
            Assert.That(
                _state.IsInfiniteHudVisible,
                Is.EqualTo(gameMode == E_GameMode.Infinite));
        }

        private void AssertPanelsHidden()
        {
            Assert.That(_state.IsPausePanelVisible, Is.False);
            Assert.That(_state.IsResultPanelVisible, Is.False);
            Assert.That(_state.IsStageResultContentVisible, Is.False);
            Assert.That(_state.IsInfiniteResultContentVisible, Is.False);
        }

        private void AssertEveryUIHidden()
        {
            Assert.That(_state.IsStageHudVisible, Is.False);
            Assert.That(_state.IsInfiniteHudVisible, Is.False);
            AssertPanelsHidden();
        }
    }
}
