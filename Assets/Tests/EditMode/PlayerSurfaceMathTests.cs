using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class PlayerSurfaceMathTests
    {
        [Test]
        public void IsGroundSurface_UpwardNormal_ReturnsTrue()
        {
            Assert.That(
                PlayerSurfaceMath.IsGroundSurface(Vector3.up),
                Is.True);
        }

        [Test]
        public void IsGroundSurface_MaximumAngleBoundary_ReturnsTrue()
        {
            Vector3 normal = CreateNormalAtAngle(45.0f);

            Assert.That(PlayerSurfaceMath.IsGroundSurface(normal), Is.True);
        }

        [Test]
        public void IsGroundSurface_OutsideMaximumAngle_ReturnsFalse()
        {
            Vector3 normal = CreateNormalAtAngle(45.1f);

            Assert.That(PlayerSurfaceMath.IsGroundSurface(normal), Is.False);
        }

        [TestCase(80.0f)]
        [TestCase(90.0f)]
        [TestCase(100.0f)]
        public void IsWallSurface_WallAngleRange_ReturnsTrue(float angle)
        {
            Vector3 normal = CreateNormalAtAngle(angle);

            Assert.That(PlayerSurfaceMath.IsWallSurface(normal), Is.True);
        }

        [TestCase(79.9f)]
        [TestCase(100.1f)]
        public void IsWallSurface_OutsideWallAngleRange_ReturnsFalse(float angle)
        {
            Vector3 normal = CreateNormalAtAngle(angle);

            Assert.That(PlayerSurfaceMath.IsWallSurface(normal), Is.False);
        }

        [Test]
        public void SurfaceClassification_SteepSlope_IsNeitherGroundNorWall()
        {
            Vector3 normal = CreateNormalAtAngle(60.0f);

            Assert.That(PlayerSurfaceMath.IsGroundSurface(normal), Is.False);
            Assert.That(PlayerSurfaceMath.IsWallSurface(normal), Is.False);
        }

        [Test]
        public void SurfaceClassification_NonNormalizedNormal_UsesDirection()
        {
            Vector3 groundNormal = CreateNormalAtAngle(30.0f) * 5.0f;
            Vector3 wallNormal = Vector3.right * 5.0f;

            Assert.That(
                PlayerSurfaceMath.IsGroundSurface(groundNormal),
                Is.True);
            Assert.That(
                PlayerSurfaceMath.IsWallSurface(wallNormal),
                Is.True);
        }

        [Test]
        public void SurfaceClassification_ZeroNormal_IsInvalid()
        {
            AssertInvalidNormal(Vector3.zero);
        }

        [Test]
        public void SurfaceClassification_NaNNormal_IsInvalid()
        {
            AssertInvalidNormal(new Vector3(float.NaN, 1.0f, 0.0f));
        }

        [Test]
        public void SurfaceClassification_InfinityNormal_IsInvalid()
        {
            AssertInvalidNormal(
                new Vector3(float.PositiveInfinity, 1.0f, 0.0f));
        }

        private Vector3 CreateNormalAtAngle(float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
        }

        private void AssertInvalidNormal(Vector3 normal)
        {
            Assert.That(PlayerSurfaceMath.IsValidNormal(normal), Is.False);
            Assert.That(PlayerSurfaceMath.IsGroundSurface(normal), Is.False);
            Assert.That(PlayerSurfaceMath.IsWallSurface(normal), Is.False);
        }
    }
}
