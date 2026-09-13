using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternGeometryTests
    {
        private static readonly string[] PatternIds =
        {
            InfinitePatternCatalogFactory.FlatId,
            InfinitePatternCatalogFactory.SingleRiseId,
            InfinitePatternCatalogFactory.LegacyStepsId,
            InfinitePatternCatalogFactory.InternalGapId
        };

        [TestCase("Flat", 0, 0.0f, 0.0f, 40.0f)]
        [TestCase("SingleRise", 0, -14.0f, 0.0f, 12.0f)]
        [TestCase("SingleRise", 1, 1.0f, 1.0f, 14.0f)]
        [TestCase("SingleRise", 2, 15.0f, 0.0f, 10.0f)]
        [TestCase("LegacySteps", 0, -16.0f, 0.0f, 8.0f)]
        [TestCase("LegacySteps", 1, -6.0f, 1.0f, 8.0f)]
        [TestCase("LegacySteps", 2, 4.0f, 1.0f, 8.0f)]
        [TestCase("LegacySteps", 3, 15.0f, 0.0f, 10.0f)]
        [TestCase("InternalGap", 0, -11.5f, 0.0f, 17.0f)]
        [TestCase("InternalGap", 1, 11.5f, 0.0f, 17.0f)]
        public void Surface_ConfirmedTransformAndCollider(
            string patternId,
            int index,
            float expectedX,
            float expectedY,
            float expectedLength)
        {
            Assert.That(
                InfinitePatternGeometry.TryGetSurface(
                    patternId, index,
                    out Vector3 position, out Vector3 size),
                Is.True);
            Assert.That(position, Is.EqualTo(
                new Vector3(expectedX, expectedY, 0.0f)));
            Assert.That(size, Is.EqualTo(
                new Vector3(expectedLength, 1.0f, 4.0f)));
        }

        [TestCase(InfinitePatternCatalogFactory.FlatId, 1)]
        [TestCase(InfinitePatternCatalogFactory.SingleRiseId, 3)]
        [TestCase(InfinitePatternCatalogFactory.LegacyStepsId, 4)]
        [TestCase(InfinitePatternCatalogFactory.InternalGapId, 2)]
        public void Surfaces_ConfirmedCountAndBounds(
            string patternId,
            int expectedCount)
        {
            Assert.That(
                InfinitePatternGeometry.TryGetSurfaceCount(
                    patternId, out int count),
                Is.True);
            Assert.That(count, Is.EqualTo(expectedCount));
            Assert.That(
                InfinitePatternGeometry.TryGetSurface(
                    patternId, 0, out Vector3 first, out Vector3 firstSize),
                Is.True);
            Assert.That(
                InfinitePatternGeometry.TryGetSurface(
                    patternId, count - 1,
                    out Vector3 last, out Vector3 lastSize),
                Is.True);
            Assert.That(first.x - firstSize.x * 0.5f, Is.EqualTo(-20.0f));
            Assert.That(last.x + lastSize.x * 0.5f, Is.EqualTo(20.0f));
            Assert.That(first.y + firstSize.y * 0.5f, Is.EqualTo(0.5f));
            Assert.That(last.y + lastSize.y * 0.5f, Is.EqualTo(0.5f));
            Assert.That(
                InfinitePatternGeometry.TryGetSurface(
                    patternId, count, out _, out _),
                Is.False);
        }

        [Test]
        public void AllInternalTransitions_PassExistingTraversalContract()
        {
            for (int i = 0; i < PatternIds.Length; i++)
            {
                Assert.That(
                    InfinitePatternGeometry.CanTraverseInternalSurfaces(
                        PatternIds[i]),
                    Is.True,
                    PatternIds[i]);
            }
        }

        [Test]
        public void EveryTransition_ProvidesMinimumWindowAtBothSpeeds()
        {
            for (int patternIndex = 0;
                 patternIndex < PatternIds.Length;
                 patternIndex++)
            {
                string patternId = PatternIds[patternIndex];
                InfinitePatternGeometry.TryGetSurfaceCount(
                    patternId, out int count);

                for (int surfaceIndex = 0;
                     surfaceIndex < count - 1;
                     surfaceIndex++)
                {
                    InfinitePatternGeometry.TryGetSurface(
                        patternId, surfaceIndex,
                        out Vector3 from, out Vector3 fromSize);
                    InfinitePatternGeometry.TryGetSurface(
                        patternId, surfaceIndex + 1,
                        out Vector3 to, out Vector3 toSize);
                    float gap = to.x - toSize.x * 0.5f -
                                (from.x + fromSize.x * 0.5f);
                    float heightDifference =
                        to.y + toSize.y * 0.5f -
                        (from.y + fromSize.y * 0.5f);

                    AssertWindow(
                        patternId, surfaceIndex, gap,
                        heightDifference, fromSize.x, toSize.x,
                        InfinitePatternTraversalMath.BaseHorizontalSpeed);
                    AssertWindow(
                        patternId, surfaceIndex, gap,
                        heightDifference, fromSize.x, toSize.x,
                        InfinitePatternTraversalMath.MaximumHorizontalSpeed);
                }
            }
        }

        [Test]
        public void AllPatternPairs_PassBoundaryGroundAndJumpContracts()
        {
            for (int previousIndex = 0;
                 previousIndex < PatternIds.Length;
                 previousIndex++)
            {
                string previousId = PatternIds[previousIndex];
                InfinitePatternGeometry.TryGetSurfaceCount(
                    previousId, out int previousCount);
                InfinitePatternGeometry.TryGetSurface(
                    previousId, previousCount - 1,
                    out Vector3 previous, out Vector3 previousSize);

                for (int nextIndex = 0;
                     nextIndex < PatternIds.Length;
                     nextIndex++)
                {
                    string nextId = PatternIds[nextIndex];
                    InfinitePatternGeometry.TryGetSurface(
                        nextId, 0, out Vector3 next, out Vector3 nextSize);
                    float boundaryGap =
                        InfinitePatternDefinition.PatternLength +
                        next.x - nextSize.x * 0.5f -
                        (previous.x + previousSize.x * 0.5f);
                    float heightDifference =
                        next.y + nextSize.y * 0.5f -
                        (previous.y + previousSize.y * 0.5f);

                    Assert.That(
                        boundaryGap,
                        Is.EqualTo(InfinitePatternDefinition.GroundGap),
                        previousId + " -> " + nextId);
                    Assert.That(heightDifference, Is.Zero);
                    Assert.That(
                        InfinitePatternTraversalMath.CanTraverseJump(
                            boundaryGap,
                            heightDifference,
                            previousSize.x,
                            nextSize.x),
                        Is.True,
                        previousId + " -> " + nextId);
                }
            }
        }

        [TestCase(InfinitePatternCatalogFactory.FlatId, -4.0f)]
        [TestCase(InfinitePatternCatalogFactory.SingleRiseId, 0.0f)]
        [TestCase(InfinitePatternCatalogFactory.LegacyStepsId, 2.0f)]
        [TestCase(InfinitePatternCatalogFactory.InternalGapId, 4.0f)]
        public void AdvanceBoundaryPoint_IsPatternSpecific(
            string patternId,
            float expectedX)
        {
            Assert.That(
                InfinitePatternGeometry.TryGetAdvanceBoundaryPoint(
                    patternId, out Vector3 point),
                Is.True);
            Assert.That(point, Is.EqualTo(
                new Vector3(expectedX, 5.5f, 0.0f)));
        }

        [Test]
        public void InvalidPatternAndIndex_AreRejected()
        {
            Assert.That(
                InfinitePatternGeometry.TryGetSurfaceCount(
                    "Unknown", out _),
                Is.False);
            Assert.That(
                InfinitePatternGeometry.TryGetSurface(
                    "Flat", -1, out _, out _),
                Is.False);
            Assert.That(
                InfinitePatternGeometry.TryGetAdvanceBoundaryPoint(
                    "Unknown", out _),
                Is.False);
            Assert.That(
                InfinitePatternGeometry.CanTraverseInternalSurfaces(
                    "Unknown"),
                Is.False);
        }

        [Test]
        public void JustOutsideTraversalContract_IsRejected()
        {
            Assert.That(
                InfinitePatternTraversalMath.CanTraverseJump(
                    4.0f, 3.011f, 8.0f, 8.0f),
                Is.False);
            Assert.That(
                InfinitePatternTraversalMath.CanTraverseJump(
                    4.0f, 0.0f, 4.0f, 4.0f),
                Is.False);
        }

        private static void AssertWindow(
            string patternId,
            int transitionIndex,
            float gap,
            float heightDifference,
            float runwayLength,
            float landingLength,
            float speed)
        {
            Assert.That(
                InfinitePatternTraversalMath.TryCalculateJumpInputWindow(
                    gap, heightDifference, runwayLength,
                    landingLength, speed, out float duration),
                Is.True,
                patternId + "/" + transitionIndex + "/" + speed);
            Assert.That(
                duration,
                Is.GreaterThanOrEqualTo(
                    InfinitePatternTraversalMath.MinimumJumpInputWindow),
                patternId + "/" + transitionIndex + "/" + speed);
        }
    }
}
