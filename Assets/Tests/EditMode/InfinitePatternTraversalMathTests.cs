using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternTraversalMathTests
    {
        private const float DurationTolerance = 0.0001f;

        [TestCase(0.0f, 0.0f, true)]
        [TestCase(0.01f, -0.01f, true)]
        [TestCase(0.011f, 0.0f, false)]
        [TestCase(0.0f, 0.011f, false)]
        public void IsContinuousGround_UsesPositionTolerance(
            float horizontalGap,
            float heightDifference,
            bool expected)
        {
            Assert.That(
                InfinitePatternTraversalMath.IsContinuousGround(
                    horizontalGap,
                    heightDifference),
                Is.EqualTo(expected));
        }

        [Test]
        public void CanTraverseJump_SelectedBoundaryGap_IsAcceptedAtBothSpeeds()
        {
            bool canTraverse = InfinitePatternTraversalMath.CanTraverseJump(
                4.0f,
                0.0f,
                8.0f,
                8.0f);

            Assert.That(canTraverse, Is.True);
        }

        [Test]
        public void CanTraverseJump_MaximumHeightAboveJumpApex_IsRejected()
        {
            bool canTraverse = InfinitePatternTraversalMath.CanTraverseJump(
                4.0f,
                3.011f,
                8.0f,
                8.0f);

            Assert.That(canTraverse, Is.False);
        }

        [Test]
        public void CanTraverseJump_ShortLandingAtMaximumSpeed_IsRejected()
        {
            bool canTraverse = InfinitePatternTraversalMath.CanTraverseJump(
                4.0f,
                0.0f,
                4.0f,
                4.0f);

            Assert.That(canTraverse, Is.False);
        }

        [Test]
        public void TryCalculateJumpInputWindow_ValidGap_ReturnsExpectedDuration()
        {
            bool didCalculate =
                InfinitePatternTraversalMath.TryCalculateJumpInputWindow(
                    4.0f,
                    0.0f,
                    8.0f,
                    8.0f,
                    8.0f,
                    out float duration);

            Assert.That(didCalculate, Is.True);
            Assert.That(duration, Is.EqualTo(0.417296f).Within(DurationTolerance));
        }

        [Test]
        public void TryCalculateJumpInputWindow_LandingNarrowerThanPlayer_IsRejected()
        {
            bool didCalculate =
                InfinitePatternTraversalMath.TryCalculateJumpInputWindow(
                    4.0f,
                    0.0f,
                    8.0f,
                    0.999f,
                    8.0f,
                    out _);

            Assert.That(didCalculate, Is.False);
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(-0.001f)]
        public void TryCalculateJumpInputWindow_InvalidGap_IsRejected(float gap)
        {
            bool didCalculate =
                InfinitePatternTraversalMath.TryCalculateJumpInputWindow(
                    gap,
                    0.0f,
                    8.0f,
                    8.0f,
                    8.0f,
                    out _);

            Assert.That(didCalculate, Is.False);
        }

        [Test]
        public void CanClearObstacle_ObstacleBelowPlayerPath_ReturnsTrue()
        {
            bool canClear = InfinitePatternTraversalMath.CanClearObstacle(
                2.0f,
                2.0f,
                2.9f,
                8.0f);

            Assert.That(canClear, Is.True);
        }

        [Test]
        public void CanClearObstacle_ObstacleAbovePlayerPath_ReturnsFalse()
        {
            bool canClear = InfinitePatternTraversalMath.CanClearObstacle(
                2.0f,
                2.0f,
                3.1f,
                8.0f);

            Assert.That(canClear, Is.False);
        }

        [Test]
        public void CanClearObstacle_InvalidGeometry_IsRejected()
        {
            Assert.That(
                InfinitePatternTraversalMath.CanClearObstacle(
                    -0.001f,
                    2.0f,
                    1.0f,
                    8.0f),
                Is.False);
        }
    }
}
