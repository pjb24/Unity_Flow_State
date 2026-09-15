using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class MomentumScoreStateTests
    {
        private const double Tolerance = 0.0000001;

        private MomentumScoreState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new MomentumScoreState();
        }

        [Test]
        public void StartRun_CurrentVersion_StartsAtBaseMultiplier()
        {
            Assert.That(_state.StartRun(ScoringVersion.Current), Is.True);
            Assert.That(_state.SuccessCount, Is.Zero);
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.0));
            Assert.That(_state.MaximumReachedMultiplier, Is.EqualTo(1.0));
            Assert.That(_state.RemainingDuration, Is.Zero);
            Assert.That(_state.RemainingRatio, Is.Zero);
        }

        [TestCase(1, 1.25, 10.0)]
        [TestCase(2, 1.50, 9.5)]
        [TestCase(3, 1.75, 9.0)]
        [TestCase(4, 2.00, 8.5)]
        [TestCase(5, 2.25, 8.0)]
        [TestCase(6, 2.50, 7.5)]
        [TestCase(7, 2.75, 7.0)]
        [TestCase(8, 3.00, 6.5)]
        public void TryStep_ConsecutiveSuccess_UsesExpectedStage(
            int successCount,
            double expectedMultiplier,
            double expectedDuration)
        {
            StartRun();

            for (int index = 1; index <= successCount; index++)
            {
                Assert.That(
                    _state.TryStep(ScoringVersion.Current, 100.0, true, index),
                    Is.True);
            }

            Assert.That(_state.CurrentMultiplier,
                Is.EqualTo(expectedMultiplier).Within(Tolerance));
            Assert.That(_state.RemainingDuration,
                Is.EqualTo(expectedDuration).Within(Tolerance));
            Assert.That(_state.RemainingRatio, Is.EqualTo(1.0));
        }

        [Test]
        public void TryStep_AtMaximum_RefreshesDurationWithoutIncreasingMultiplier()
        {
            StartRun();

            for (int index = 1; index <= 8; index++)
            {
                _state.TryStep(ScoringVersion.Current, 0.0, true, index);
            }

            _state.TryStep(ScoringVersion.Current, 3.0, false, 0);
            Assert.That(
                _state.TryStep(ScoringVersion.Current, 100.0, true, 9),
                Is.True);
            Assert.That(_state.SuccessCount, Is.EqualTo(8));
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(3.0));
            Assert.That(_state.RemainingDuration, Is.EqualTo(6.5));
        }

        [Test]
        public void TryStep_DuplicateSuccess_IsRejectedWithoutChangingState()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);

            Assert.That(
                _state.TryStep(ScoringVersion.Current, 0.0, true, 1),
                Is.False);
            Assert.That(_state.SuccessCount, Is.EqualTo(1));
            Assert.That(_state.RemainingDuration, Is.EqualTo(10.0));
        }

        [Test]
        public void TryStep_AtExactExpiry_ResetsMultiplier()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);

            Assert.That(
                _state.TryStep(ScoringVersion.Current, 10.0, false, 0),
                Is.True);
            Assert.That(_state.SuccessCount, Is.Zero);
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.0));
            Assert.That(_state.RemainingDuration, Is.Zero);
        }

        [Test]
        public void TryStep_SuccessAtExpiry_PrioritizesSuccessAndRefreshesDuration()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);

            Assert.That(
                _state.TryStep(ScoringVersion.Current, 10.0, true, 2),
                Is.True);
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.5));
            Assert.That(_state.RemainingDuration, Is.EqualTo(9.5));
        }

        [Test]
        public void PauseResume_PreservesRemainingDuration()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);
            _state.TryStep(ScoringVersion.Current, 2.0, false, 0);
            Assert.That(_state.Pause(), Is.True);

            Assert.That(
                _state.TryStep(ScoringVersion.Current, 5.0, false, 0),
                Is.False);
            Assert.That(_state.RemainingDuration, Is.EqualTo(8.0));
            Assert.That(_state.Resume(), Is.True);
            Assert.That(
                _state.TryStep(ScoringVersion.Current, 1.0, false, 0),
                Is.True);
            Assert.That(_state.RemainingDuration, Is.EqualTo(7.0));
        }

        [Test]
        public void NormalLandingAndWallContact_DoNotResetMultiplierOrTimer()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);
            _state.TryStep(ScoringVersion.Current, 2.0, false, 0);

            Assert.That(
                _state.TryNotifyNormalLanding(ScoringVersion.Current),
                Is.True);
            Assert.That(
                _state.TryNotifyWallContact(ScoringVersion.Current),
                Is.True);
            Assert.That(_state.SuccessCount, Is.EqualTo(1));
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.25));
            Assert.That(_state.RemainingDuration, Is.EqualTo(8.0));
        }

        [Test]
        public void FinalizeRun_PreservesStateAndRejectsFurtherUpdates()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);

            Assert.That(_state.FinalizeRun(), Is.True);
            Assert.That(
                _state.TryStep(ScoringVersion.Current, 1.0, false, 0),
                Is.False);
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.25));
            Assert.That(_state.RemainingDuration, Is.EqualTo(10.0));
        }

        [Test]
        public void ResetThenStartRun_ClearsPreviousRunState()
        {
            StartRun();
            _state.TryStep(ScoringVersion.Current, 0.0, true, 1);
            _state.Reset();

            Assert.That(_state.StartRun(ScoringVersion.Current), Is.True);
            Assert.That(_state.SuccessCount, Is.Zero);
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.0));
            Assert.That(_state.MaximumReachedMultiplier, Is.EqualTo(1.0));
        }

        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void TryStep_InvalidTime_IsRejected(double deltaTime)
        {
            StartRun();

            Assert.That(
                _state.TryStep(
                    ScoringVersion.Current,
                    deltaTime,
                    false,
                    0),
                Is.False);
            Assert.That(_state.CurrentMultiplier, Is.EqualTo(1.0));
        }

        [Test]
        public void VersionMismatch_IsRejected()
        {
            Assert.That(
                _state.StartRun(ScoringVersion.LegacyDistanceScore),
                Is.False);
            StartRun();
            Assert.That(
                _state.TryStep(
                    ScoringVersion.LegacyDistanceScore,
                    0.0,
                    true,
                    1),
                Is.False);
        }

        private void StartRun()
        {
            Assert.That(_state.StartRun(ScoringVersion.Current), Is.True);
        }
    }
}
