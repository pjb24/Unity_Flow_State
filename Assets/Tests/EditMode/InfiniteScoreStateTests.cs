using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfiniteScoreStateTests
    {
        private InfiniteScoreState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new InfiniteScoreState();
            Assert.That(
                _state.Initialize(ScoringVersion.Current, 10.0),
                Is.True);
        }

        [Test]
        public void TryUpdate_BaseMultiplier_ProducesOnlyBaseDistanceScore()
        {
            Assert.That(
                _state.TryUpdate(ScoringVersion.Current, 12.39, 1.0),
                Is.True);
            Assert.That(_state.BaseDistanceScore, Is.EqualTo(123));
            Assert.That(_state.MomentumBonus, Is.Zero);
            Assert.That(_state.DistanceScore, Is.EqualTo(123));
        }

        [Test]
        public void TryUpdate_Multiplier_AppliesOnlyToNewDistance()
        {
            _state.TryUpdate(ScoringVersion.Current, 10.0, 1.0);
            _state.TryUpdate(ScoringVersion.Current, 14.0, 1.5);

            Assert.That(_state.BaseDistanceScore, Is.EqualTo(140));
            Assert.That(_state.MomentumBonus, Is.EqualTo(20));
            Assert.That(_state.DistanceScore, Is.EqualTo(160));
        }

        [Test]
        public void TryUpdate_FractionalBonus_IsIndependentOfFramePartition()
        {
            InfiniteScoreState partitioned = new InfiniteScoreState();
            partitioned.Initialize(ScoringVersion.Current, 10.0);

            _state.TryUpdate(ScoringVersion.Current, 1.0, 1.25);
            partitioned.TryUpdate(ScoringVersion.Current, 0.4, 1.25);
            partitioned.TryUpdate(ScoringVersion.Current, 0.7, 1.25);
            partitioned.TryUpdate(ScoringVersion.Current, 1.0, 1.25);

            Assert.That(partitioned.PreciseMomentumBonus,
                Is.EqualTo(_state.PreciseMomentumBonus).Within(0.0000001));
            Assert.That(partitioned.MomentumBonus,
                Is.EqualTo(_state.MomentumBonus));
            Assert.That(partitioned.DistanceScore,
                Is.EqualTo(_state.DistanceScore));
        }

        [Test]
        public void TryCalculateTotalScore_CollectibleIsNotMultiplied()
        {
            _state.TryUpdate(ScoringVersion.Current, 10.0, 2.0);

            Assert.That(
                _state.TryCalculateTotalScore(
                    ScoringVersion.Current,
                    30,
                    out int totalScore),
                Is.True);
            Assert.That(_state.BaseDistanceScore, Is.EqualTo(100));
            Assert.That(_state.MomentumBonus, Is.EqualTo(100));
            Assert.That(totalScore, Is.EqualTo(230));
        }

        [Test]
        public void Scores_Overflow_SaturatesAtMaximum()
        {
            Assert.That(
                _state.TryUpdate(
                    ScoringVersion.Current,
                    double.MaxValue,
                    3.0),
                Is.True);
            Assert.That(_state.BaseDistanceScore, Is.EqualTo(int.MaxValue));
            Assert.That(_state.MomentumBonus, Is.EqualTo(int.MaxValue));
            Assert.That(_state.DistanceScore, Is.EqualTo(int.MaxValue));
            Assert.That(
                _state.TryCalculateTotalScore(
                    ScoringVersion.Current,
                    int.MaxValue,
                    out int totalScore),
                Is.True);
            Assert.That(totalScore, Is.EqualTo(int.MaxValue));
        }

        [TestCase(-1.0, 1.0)]
        [TestCase(double.NaN, 1.0)]
        [TestCase(double.PositiveInfinity, 1.0)]
        [TestCase(1.0, 0.99)]
        [TestCase(1.0, 3.01)]
        [TestCase(1.0, double.NaN)]
        public void TryUpdate_InvalidInput_IsRejected(
            double distance,
            double multiplier)
        {
            Assert.That(
                _state.TryUpdate(
                    ScoringVersion.Current,
                    distance,
                    multiplier),
                Is.False);
            Assert.That(_state.CurrentDistance, Is.Zero);
        }

        [Test]
        public void TryUpdate_DecreasedDistance_IsRejected()
        {
            _state.TryUpdate(ScoringVersion.Current, 10.0, 1.0);

            Assert.That(
                _state.TryUpdate(ScoringVersion.Current, 9.0, 1.0),
                Is.False);
            Assert.That(_state.CurrentDistance, Is.EqualTo(10.0));
        }

        [Test]
        public void VersionMismatch_IsRejectedWithoutChangingScore()
        {
            Assert.That(
                _state.TryUpdate(
                    ScoringVersion.LegacyDistanceScore,
                    10.0,
                    1.0),
                Is.False);
            Assert.That(_state.DistanceScore, Is.Zero);
        }

        [Test]
        public void FinalizeThenReset_PreservesThenClearsState()
        {
            _state.TryUpdate(ScoringVersion.Current, 10.0, 1.5);
            Assert.That(_state.TryFinalize(ScoringVersion.Current), Is.True);
            Assert.That(
                _state.TryUpdate(ScoringVersion.Current, 20.0, 2.0),
                Is.False);
            Assert.That(_state.DistanceScore, Is.EqualTo(150));

            _state.Reset();
            Assert.That(_state.DistanceScore, Is.Zero);
            Assert.That(_state.IsInitialized, Is.False);
        }
    }
}
