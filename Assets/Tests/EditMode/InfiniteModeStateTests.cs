using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfiniteModeStateTests
    {
        private const float MinimumHorizontalSpeed = 5.0f;
        private const float StartGraceDuration = 1.0f;
        private const float BelowSpeedGraceDuration = 0.5f;

        private InfiniteModeState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new InfiniteModeState();
        }

        [Test]
        public void NewState_DefaultMode_IsStage()
        {
            Assert.That(_state.GameMode, Is.EqualTo(E_GameMode.Stage));
        }

        [Test]
        public void SetGameMode_BeforeStart_ChangesMode()
        {
            _state.Initialize(
                MinimumHorizontalSpeed,
                StartGraceDuration,
                BelowSpeedGraceDuration);

            bool didChange = _state.SetGameMode(E_GameMode.Infinite);

            Assert.That(didChange, Is.True);
            Assert.That(_state.GameMode, Is.EqualTo(E_GameMode.Infinite));
        }

        [Test]
        public void SetGameMode_DuringPlay_IsRejected()
        {
            InitializeAndStart(E_GameMode.Infinite);

            bool didChange = _state.SetGameMode(E_GameMode.Stage);

            Assert.That(didChange, Is.False);
            Assert.That(_state.GameMode, Is.EqualTo(E_GameMode.Infinite));
        }

        [TestCase(4.999f, true)]
        [TestCase(5.0f, false)]
        [TestCase(5.001f, false)]
        [TestCase(-4.999f, true)]
        [TestCase(-5.0f, false)]
        [TestCase(-5.001f, false)]
        public void UpdateProgress_HorizontalSpeedBoundary_UsesAbsoluteValue(
            float horizontalSpeed,
            bool expectedEnd)
        {
            InitializeAndStart(E_GameMode.Infinite, 0.0f);

            bool didEnd = _state.UpdateProgress(
                horizontalSpeed,
                BelowSpeedGraceDuration);

            Assert.That(didEnd, Is.EqualTo(expectedEnd));
        }

        [Test]
        public void UpdateProgress_BelowSpeedBeforeGraceExpires_ContinuesPlay()
        {
            InitializeAndStart(E_GameMode.Infinite, 0.0f);

            bool didEnd = _state.UpdateProgress(
                0.0f,
                BelowSpeedGraceDuration - 0.001f);

            Assert.That(didEnd, Is.False);
            Assert.That(_state.IsPlaying, Is.True);
        }

        [Test]
        public void UpdateProgress_BelowSpeedAtGraceBoundary_EndsOnce()
        {
            InitializeAndStart(E_GameMode.Infinite, 0.0f);

            bool didEnd = _state.UpdateProgress(
                0.0f,
                BelowSpeedGraceDuration);

            Assert.That(didEnd, Is.True);
            Assert.That(_state.HasEnded, Is.True);
        }

        [Test]
        public void UpdateProgress_SpeedRecovers_ResetsBelowSpeedDuration()
        {
            InitializeAndStart(E_GameMode.Infinite, 0.0f);
            _state.UpdateProgress(0.0f, BelowSpeedGraceDuration - 0.1f);

            _state.UpdateProgress(MinimumHorizontalSpeed, 0.1f);
            bool didEnd = _state.UpdateProgress(
                0.0f,
                BelowSpeedGraceDuration - 0.1f);

            Assert.That(didEnd, Is.False);
            Assert.That(_state.IsPlaying, Is.True);
        }

        [Test]
        public void UpdateProgress_DuringStartGrace_DoesNotEndBySpeed()
        {
            InitializeAndStart(E_GameMode.Infinite);

            bool didEnd = _state.UpdateProgress(
                0.0f,
                StartGraceDuration);

            Assert.That(didEnd, Is.False);
            Assert.That(_state.IsPlaying, Is.True);
        }

        [Test]
        public void NotifyFallThresholdReached_DuringStartGrace_EndsImmediately()
        {
            InitializeAndStart(E_GameMode.Infinite);

            bool didEnd = _state.NotifyFallThresholdReached();

            Assert.That(didEnd, Is.True);
            Assert.That(_state.HasEnded, Is.True);
        }

        [Test]
        public void NotifyGoalReached_InfiniteMode_DoesNotEnd()
        {
            InitializeAndStart(E_GameMode.Infinite);

            bool didEnd = _state.NotifyGoalReached();

            Assert.That(didEnd, Is.False);
            Assert.That(_state.IsPlaying, Is.True);
        }

        [Test]
        public void InfiniteEnd_SecondCondition_DoesNotEndAgain()
        {
            InitializeAndStart(E_GameMode.Infinite, 0.0f);
            bool didEndByFall = _state.NotifyFallThresholdReached();

            bool didEndBySpeed = _state.UpdateProgress(
                0.0f,
                BelowSpeedGraceDuration);

            Assert.That(didEndByFall, Is.True);
            Assert.That(didEndBySpeed, Is.False);
        }

        [Test]
        public void UpdateProgress_BeforeStart_IsIgnored()
        {
            Initialize(E_GameMode.Infinite);

            bool didEndBySpeed = _state.UpdateProgress(
                0.0f,
                StartGraceDuration + BelowSpeedGraceDuration);

            Assert.That(didEndBySpeed, Is.False);
            Assert.That(_state.HasEnded, Is.False);
        }

        [Test]
        public void NotifyFallThresholdReached_BeforeStart_IsIgnored()
        {
            Initialize(E_GameMode.Infinite);

            bool didEnd = _state.NotifyFallThresholdReached();

            Assert.That(didEnd, Is.False);
            Assert.That(_state.HasEnded, Is.False);
        }

        [Test]
        public void Reset_AfterEnd_ClearsRunStateAndRetainsMode()
        {
            InitializeAndStart(E_GameMode.Infinite);
            _state.TryRequestPatternAdvance(1);
            _state.NotifyFallThresholdReached();

            _state.Reset();

            Assert.That(_state.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_state.IsPlaying, Is.False);
            Assert.That(_state.HasEnded, Is.False);
            Assert.That(_state.TryRequestPatternAdvance(1), Is.False);

            Assert.That(_state.Start(), Is.True);
            Assert.That(_state.TryRequestPatternAdvance(1), Is.True);
        }

        [Test]
        public void TryRequestPatternAdvance_SameBoundary_IsAcceptedOnce()
        {
            InitializeAndStart(E_GameMode.Infinite);

            bool didRequest = _state.TryRequestPatternAdvance(1);
            bool didRequestAgain = _state.TryRequestPatternAdvance(1);

            Assert.That(didRequest, Is.True);
            Assert.That(didRequestAgain, Is.False);
        }

        [Test]
        public void UpdateProgress_StageMode_IsIgnored()
        {
            InitializeAndStart(E_GameMode.Stage, 0.0f);

            bool didEnd = _state.UpdateProgress(
                0.0f,
                BelowSpeedGraceDuration);

            Assert.That(didEnd, Is.False);
            Assert.That(_state.IsPlaying, Is.True);
        }

        [Test]
        public void NotifyFallThresholdReached_StageMode_IsIgnored()
        {
            InitializeAndStart(E_GameMode.Stage);

            bool didEnd = _state.NotifyFallThresholdReached();

            Assert.That(didEnd, Is.False);
            Assert.That(_state.IsPlaying, Is.True);
        }

        [Test]
        public void TryRequestPatternAdvance_StageMode_IsIgnored()
        {
            InitializeAndStart(E_GameMode.Stage);

            bool didRequest = _state.TryRequestPatternAdvance(1);

            Assert.That(didRequest, Is.False);
        }

        private void InitializeAndStart(
            E_GameMode gameMode,
            float startGraceDuration = StartGraceDuration)
        {
            Initialize(gameMode, startGraceDuration);
            Assert.That(_state.Start(), Is.True);
        }

        private void Initialize(
            E_GameMode gameMode,
            float startGraceDuration = StartGraceDuration)
        {
            bool didInitialize = _state.Initialize(
                MinimumHorizontalSpeed,
                startGraceDuration,
                BelowSpeedGraceDuration);

            Assert.That(didInitialize, Is.True);
            Assert.That(_state.SetGameMode(gameMode), Is.True);
        }
    }
}
