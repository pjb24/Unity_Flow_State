using System.Collections.Generic;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfiniteCollectibleLayoutTests
    {
        private static readonly string[] PatternIds =
        {
            InfinitePatternCatalogFactory.FlatId,
            InfinitePatternCatalogFactory.SingleRiseId,
            InfinitePatternCatalogFactory.LegacyStepsId,
            InfinitePatternCatalogFactory.InternalGapId
        };

        [Test]
        public void EveryPattern_HasUniqueFinitePointsAndCompleteJumpGroups()
        {
            for (int patternIndex = 0;
                 patternIndex < PatternIds.Length; patternIndex++)
            {
                string patternId = PatternIds[patternIndex];
                Assert.That(InfinitePatternGeometry.TryGetSurfaceCount(
                    patternId, out int surfaceCount), Is.True);
                Assert.That(InfiniteCollectibleLayout.TryGetCount(
                    patternId, out int count), Is.True);
                Assert.That(count, Is.EqualTo(5 + 5 * (surfaceCount - 1)));
                HashSet<string> ids = new HashSet<string>();

                for (int index = 0; index < count; index++)
                {
                    Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                        patternId, index, out string id,
                        out Vector3 position), Is.True);
                    Assert.That(string.IsNullOrWhiteSpace(id), Is.False);
                    Assert.That(ids.Add(id), Is.True,
                        patternId + " duplicate ID: " + id);
                    Assert.That(float.IsNaN(position.x) ||
                                float.IsInfinity(position.x) ||
                                float.IsNaN(position.y) ||
                                float.IsInfinity(position.y) ||
                                position.z != 0.0f, Is.False);
                }

                Assert.That(ids.Contains("entry-land-01"), Is.True);
                Assert.That(ids.Contains("exit-pre-01"), Is.True);
                for (int jump = 1; jump < surfaceCount; jump++)
                {
                    string prefix = "jump-" + jump.ToString("00");
                    Assert.That(ids.Contains(prefix + "-pre-01"), Is.True);
                    Assert.That(ids.Contains(prefix + "-air-01"), Is.True);
                    Assert.That(ids.Contains(prefix + "-air-02"), Is.True);
                    Assert.That(ids.Contains(prefix + "-air-03"), Is.True);
                    Assert.That(ids.Contains(prefix + "-land-01"), Is.True);
                }
            }
        }

        [Test]
        public void InternalGroups_FollowTraversableGeometryAndBaseJumpArc()
        {
            for (int patternIndex = 0;
                 patternIndex < PatternIds.Length; patternIndex++)
            {
                string patternId = PatternIds[patternIndex];
                Assert.That(InfinitePatternGeometry.TryGetSurfaceCount(
                    patternId, out int surfaceCount), Is.True);
                Assert.That(InfinitePatternGeometry.CanTraverseInternalSurfaces(
                    patternId), Is.True);

                for (int jump = 0; jump < surfaceCount - 1; jump++)
                {
                    Assert.That(InfinitePatternGeometry.TryGetSurface(
                        patternId, jump,
                        out Vector3 from, out Vector3 fromSize), Is.True);
                    Assert.That(InfinitePatternGeometry.TryGetSurface(
                        patternId, jump + 1,
                        out Vector3 to, out Vector3 toSize), Is.True);
                    float edge = from.x + fromSize.x * 0.5f;
                    float landingStart = to.x - toSize.x * 0.5f;
                    float landingEnd = to.x + toSize.x * 0.5f;
                    float gap = landingStart - edge;
                    float height = to.y - from.y;
                    Assert.That(InfinitePatternTraversalMath.CanTraverseJump(
                        gap, height, fromSize.x, toSize.x), Is.True);

                    int firstIndex = 1 + 5 * jump;
                    Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                        patternId, firstIndex, out _,
                        out Vector3 pre), Is.True);
                    Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                        patternId, firstIndex + 4, out _,
                        out Vector3 land), Is.True);
                    Assert.That(pre.x, Is.GreaterThanOrEqualTo(
                        from.x - fromSize.x * 0.5f));
                    Assert.That(pre.x, Is.LessThan(edge));
                    Assert.That(land.x, Is.GreaterThanOrEqualTo(
                        landingStart + InfinitePatternTraversalMath.PlayerRadius));
                    Assert.That(land.x, Is.LessThanOrEqualTo(
                        landingEnd - InfinitePatternTraversalMath.PlayerRadius));
                    Assert.That(land.y, Is.EqualTo(
                        to.y + toSize.y * 0.5f + 1.0f).Within(0.01f));

                    float previousX = pre.x;
                    for (int air = 1; air <= 3; air++)
                    {
                        Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                            patternId, firstIndex + air, out _,
                            out Vector3 point), Is.True);
                        Assert.That(point.x, Is.GreaterThan(previousX));
                        Assert.That(point.x, Is.LessThan(land.x));
                        Assert.That(point.y, Is.GreaterThan(from.y +
                            fromSize.y * 0.5f + 1.0f));
                        previousX = point.x;
                    }
                }
            }
        }

        [Test]
        public void AllSixteenConnections_KeepExitAirAndEntryLandingInOrder()
        {
            Assert.That(InfinitePatternCatalogFactory.TryCreate(
                out InfinitePatternCatalog catalog), Is.True);
            for (int previous = 0; previous < PatternIds.Length; previous++)
            {
                string previousId = PatternIds[previous];
                Assert.That(InfiniteCollectibleLayout.TryGetCount(
                    previousId, out int count), Is.True);
                Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                    previousId, count - 4, out _,
                    out Vector3 pre), Is.True);
                Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                    previousId, count - 1, out _,
                    out Vector3 lastAir), Is.True);

                for (int next = 0; next < PatternIds.Length; next++)
                {
                    string nextId = PatternIds[next];
                    Assert.That(catalog.CanConnect(previousId, nextId), Is.True);
                    Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                        nextId, 0, out _, out Vector3 entry), Is.True);
                    float entryWorldX = 44.0f + entry.x;
                    Assert.That(pre.x, Is.LessThan(20.0f));
                    Assert.That(lastAir.x, Is.GreaterThan(20.0f));
                    Assert.That(lastAir.x, Is.LessThan(entryWorldX));
                    Assert.That(entryWorldX, Is.GreaterThan(24.5f));
                    Assert.That(entryWorldX, Is.LessThan(39.5f));
                }
            }
        }

        [Test]
        public void InvalidPatternAndIndex_AreRejected()
        {
            Assert.That(InfiniteCollectibleLayout.TryGetCount(null, out _),
                Is.False);
            Assert.That(InfiniteCollectibleLayout.TryGetCount(
                "Unknown", out _), Is.False);
            Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                InfinitePatternCatalogFactory.FlatId, -1,
                out _, out _), Is.False);
            Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                InfinitePatternCatalogFactory.FlatId, 5,
                out _, out _), Is.False);
            Assert.That(InfiniteCollectibleLayout.TriggerRadius,
                Is.GreaterThan(0.0f));
        }
    }
}
