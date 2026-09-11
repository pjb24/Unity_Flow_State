using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfiniteDifficultyStateTests
    {
        private InfiniteDifficultyState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new InfiniteDifficultyState();
        }

        [Test]
        public void Initialize_PreparesD1WithoutStartingRun()
        {
            _state.Initialize();

            Assert.That(_state.IsInitialized, Is.True);
            Assert.That(_state.IsRunning, Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.Zero);
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D1));
        }

        [Test]
        public void TryUpdate_BeforeStart_IsRejected()
        {
            _state.Initialize();

            Assert.That(_state.TryUpdate(220.0f), Is.False);
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D1));
        }

        [TestCase(0.0f, E_InfinitePatternDifficulty.D1)]
        [TestCase(219.999f, E_InfinitePatternDifficulty.D1)]
        [TestCase(220.0f, E_InfinitePatternDifficulty.D2)]
        [TestCase(220.001f, E_InfinitePatternDifficulty.D2)]
        [TestCase(439.999f, E_InfinitePatternDifficulty.D2)]
        [TestCase(440.0f, E_InfinitePatternDifficulty.D3)]
        [TestCase(440.001f, E_InfinitePatternDifficulty.D3)]
        [TestCase(float.MaxValue, E_InfinitePatternDifficulty.D3)]
        public void TryUpdate_DistanceBoundary_SelectsExpectedDifficulty(
            float distance,
            E_InfinitePatternDifficulty expectedDifficulty)
        {
            StartRun();

            bool didUpdate = _state.TryUpdate(distance);

            Assert.That(didUpdate, Is.True);
            Assert.That(_state.CurrentDifficulty, Is.EqualTo(expectedDifficulty));
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(distance));
        }

        [Test]
        public void TryUpdate_LargeIncrease_SkipsDirectlyToD3()
        {
            StartRun();

            bool didUpdate = _state.TryUpdate(1000.0f);

            Assert.That(didUpdate, Is.True);
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D3));
        }

        [Test]
        public void TryUpdate_DecreasedDistance_IsRejectedWithoutRegression()
        {
            StartRun();
            _state.TryUpdate(300.0f);

            bool didUpdate = _state.TryUpdate(100.0f);

            Assert.That(didUpdate, Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(300.0f));
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D2));
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void TryUpdate_InvalidDistance_IsRejected(float distance)
        {
            StartRun();

            bool didUpdate = _state.TryUpdate(distance);

            Assert.That(didUpdate, Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.Zero);
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D1));
        }

        [Test]
        public void Pause_BlocksProgressUntilResume()
        {
            StartRun();
            _state.TryUpdate(219.0f);

            Assert.That(_state.Pause(), Is.True);
            Assert.That(_state.TryUpdate(220.0f), Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(219.0f));
            Assert.That(_state.Resume(), Is.True);
            Assert.That(_state.TryUpdate(220.0f), Is.True);
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D2));
        }

        [Test]
        public void EndRun_BlocksProgressAndPreservesFinalDifficulty()
        {
            StartRun();
            _state.TryUpdate(440.0f);

            Assert.That(_state.EndRun(), Is.True);
            Assert.That(_state.TryUpdate(500.0f), Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(440.0f));
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D3));
        }

        [Test]
        public void StartRun_AfterEnd_ResetsDifficultyAndProgress()
        {
            StartRun();
            _state.TryUpdate(440.0f);
            _state.EndRun();

            bool didStart = _state.StartRun();

            Assert.That(didStart, Is.True);
            Assert.That(_state.IsRunning, Is.True);
            Assert.That(_state.MaximumForwardDistance, Is.Zero);
            Assert.That(
                _state.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D1));
        }

        [Test]
        public void DuplicateLifecycleRequests_AreRejected()
        {
            _state.Initialize();

            Assert.That(_state.StartRun(), Is.True);
            Assert.That(_state.StartRun(), Is.False);
            Assert.That(_state.Pause(), Is.True);
            Assert.That(_state.Pause(), Is.False);
            Assert.That(_state.Resume(), Is.True);
            Assert.That(_state.Resume(), Is.False);
            Assert.That(_state.EndRun(), Is.True);
            Assert.That(_state.EndRun(), Is.False);
        }

        private void StartRun()
        {
            _state.Initialize();
            Assert.That(_state.StartRun(), Is.True);
        }
    }
}
