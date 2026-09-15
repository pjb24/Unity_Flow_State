using FlowState.Runtime.Features;
using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class WorldRebaseStateTests
    {
        private WorldRebaseState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new WorldRebaseState();
            Assert.That(_state.StartRun(0.0), Is.True);
        }

        [TestCase(879.999, 0.0)]
        [TestCase(880.0, 880.0)]
        [TestCase(880.001, 880.0)]
        [TestCase(1759.999, 880.0)]
        [TestCase(1760.0, 1760.0)]
        [TestCase(1850.0, 1760.0)]
        public void TryGetRebaseOffset_Boundary_ReturnsExpectedOffset(
            double worldX,
            double expectedOffset)
        {
            Assert.That(
                _state.TryGetRebaseOffset(worldX, out double offset),
                Is.True);
            Assert.That(offset, Is.EqualTo(expectedOffset).Within(0.0000001));
        }

        [Test]
        public void Rebase_PreservesLogicalDistance()
        {
            Assert.That(_state.TryUpdate(880.0), Is.True);
            double beforeRebase = _state.MaximumForwardDistance;
            Assert.That(_state.TryApplyRebaseOffset(880.0), Is.True);
            Assert.That(_state.TryUpdate(0.0), Is.True);

            Assert.That(_state.MaximumForwardDistance,
                Is.EqualTo(beforeRebase));
            Assert.That(_state.CumulativeRebaseOffset, Is.EqualTo(880.0));
        }

        [Test]
        public void RepeatedRebase_AccumulatesMonotonicDistance()
        {
            _state.TryUpdate(880.0);
            _state.TryApplyRebaseOffset(880.0);
            _state.TryUpdate(0.0);
            _state.TryUpdate(880.0);
            _state.TryApplyRebaseOffset(880.0);
            _state.TryUpdate(0.0);

            Assert.That(_state.CumulativeRebaseOffset, Is.EqualTo(1760.0));
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(1760.0));
        }

        [Test]
        public void RebasePreservedDistance_ProducesSameScoreAndDifficultyInput()
        {
            InfiniteScoreState score = new InfiniteScoreState();
            InfiniteDifficultyState difficulty = new InfiniteDifficultyState();
            score.Initialize(ScoringVersion.Current, 10.0);
            difficulty.Initialize();
            difficulty.StartRun();

            _state.TryUpdate(880.0);
            double beforeDistance = _state.MaximumForwardDistance;
            score.TryUpdate(ScoringVersion.Current, beforeDistance, 1.5);
            difficulty.TryUpdate((float)beforeDistance);
            int beforeScore = score.DistanceScore;
            E_InfinitePatternDifficulty beforeDifficulty =
                difficulty.CurrentDifficulty;

            _state.TryApplyRebaseOffset(880.0);
            _state.TryUpdate(0.0);
            score.TryUpdate(
                ScoringVersion.Current,
                _state.MaximumForwardDistance,
                1.5);
            difficulty.TryUpdate((float)_state.MaximumForwardDistance);

            Assert.That(_state.MaximumForwardDistance,
                Is.EqualTo(beforeDistance));
            Assert.That(score.DistanceScore, Is.EqualTo(beforeScore));
            Assert.That(difficulty.CurrentDifficulty,
                Is.EqualTo(beforeDifficulty));
        }

        [Test]
        public void BackwardMovement_DoesNotReduceMaximumDistance()
        {
            _state.TryUpdate(500.0);
            _state.TryUpdate(100.0);

            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(500.0));
        }

        [TestCase(-880.0)]
        [TestCase(0.0)]
        [TestCase(440.0)]
        [TestCase(880.1)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void TryApplyRebaseOffset_InvalidOffset_IsRejected(double offset)
        {
            Assert.That(_state.TryApplyRebaseOffset(offset), Is.False);
            Assert.That(_state.CumulativeRebaseOffset, Is.Zero);
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void InvalidWorldX_IsRejected(double worldX)
        {
            Assert.That(_state.TryUpdate(worldX), Is.False);
            Assert.That(
                _state.TryGetRebaseOffset(worldX, out double offset),
                Is.False);
            Assert.That(offset, Is.Zero);
            Assert.That(_state.MaximumForwardDistance, Is.Zero);
        }

        [Test]
        public void OffsetOverflow_IsRejectedWithoutChangingState()
        {
            Assert.That(
                _state.TryApplyRebaseOffset(double.MaxValue),
                Is.False);
            Assert.That(_state.CumulativeRebaseOffset, Is.Zero);
        }

        [Test]
        public void PauseResume_BlocksThenContinuesUpdates()
        {
            _state.TryUpdate(100.0);
            Assert.That(_state.Pause(), Is.True);
            Assert.That(_state.TryUpdate(200.0), Is.False);
            Assert.That(_state.TryApplyRebaseOffset(880.0), Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(100.0));

            Assert.That(_state.Resume(), Is.True);
            Assert.That(_state.TryUpdate(200.0), Is.True);
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(200.0));
        }

        [Test]
        public void FinalizeRun_BlocksChangesAndPreservesDistance()
        {
            _state.TryUpdate(880.0);
            Assert.That(_state.FinalizeRun(), Is.True);

            Assert.That(_state.TryUpdate(1000.0), Is.False);
            Assert.That(_state.TryApplyRebaseOffset(880.0), Is.False);
            Assert.That(_state.MaximumForwardDistance, Is.EqualTo(880.0));
        }

        [Test]
        public void ResetThenStartRun_ClearsOffsetAndDistance()
        {
            _state.TryUpdate(880.0);
            _state.TryApplyRebaseOffset(880.0);
            _state.Reset();

            Assert.That(_state.StartRun(100.0), Is.True);
            Assert.That(_state.OriginLogicalX, Is.EqualTo(100.0));
            Assert.That(_state.CumulativeRebaseOffset, Is.Zero);
            Assert.That(_state.MaximumForwardDistance, Is.Zero);
        }

        [Test]
        public void DuplicateLifecycleRequests_AreRejected()
        {
            Assert.That(_state.StartRun(0.0), Is.False);
            Assert.That(_state.Pause(), Is.True);
            Assert.That(_state.Pause(), Is.False);
            Assert.That(_state.Resume(), Is.True);
            Assert.That(_state.Resume(), Is.False);
            Assert.That(_state.FinalizeRun(), Is.True);
            Assert.That(_state.FinalizeRun(), Is.False);
        }
    }
}
