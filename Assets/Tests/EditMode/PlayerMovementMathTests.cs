using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class PlayerMovementMathTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void CalculateFixedHorizontalSpeed_ValidSerializedSpeed_ReturnsSpeed()
        {
            float speed = PlayerMovementMath.CalculateFixedHorizontalSpeed(8.0f);

            Assert.That(speed, Is.EqualTo(8.0f).Within(Tolerance));
        }

        [TestCase(-1.0f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void CalculateFixedHorizontalSpeed_InvalidSerializedSpeed_ReturnsZero(
            float serializedSpeed)
        {
            float speed = PlayerMovementMath.CalculateFixedHorizontalSpeed(
                serializedSpeed);

            Assert.That(speed, Is.Zero);
        }

        [Test]
        public void CalculateJumpVerticalSpeed_DefaultSettings_ReturnsExpectedSpeed()
        {
            float verticalSpeed =
                PlayerMovementMath.CalculateJumpVerticalSpeed(3.0f, 25.0f);

            Assert.That(verticalSpeed, Is.EqualTo(12.247449f).Within(Tolerance));
        }

        [Test]
        public void ConstrainVelocity_RightWallInwardSpeed_RemovesHorizontalSpeed()
        {
            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(8.0f, -5.0f, 0.0f),
                false,
                CreateRightWallContacts());

            Assert.That(result.x, Is.Zero);
            Assert.That(result.y, Is.EqualTo(-5.0f));
        }

        [Test]
        public void ConstrainVelocity_RightWallOutwardSpeed_PreservesVelocity()
        {
            Vector3 velocity = new Vector3(-8.0f, -5.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity, false, CreateRightWallContacts());

            Assert.That(result, Is.EqualTo(velocity));
        }

        [Test]
        public void ConstrainVelocity_GroundedAtWall_PreservesHorizontalSpeed()
        {
            Vector3 velocity = new Vector3(8.0f, 0.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity, true, CreateRightWallContacts());

            Assert.That(result, Is.EqualTo(velocity));
        }

        [Test]
        public void CalculateVerticalSpeed_Airborne_AppliesGravity()
        {
            float speed = PlayerMovementMath.CalculateVerticalSpeed(
                -5.0f, false, 25.0f, 0.02f);

            Assert.That(speed, Is.EqualTo(-5.5f).Within(Tolerance));
        }

        private static PlayerWallContactState CreateRightWallContacts()
        {
            return new PlayerWallContactState(
                false,
                Vector3.zero,
                true,
                Vector3.left);
        }
    }
}
