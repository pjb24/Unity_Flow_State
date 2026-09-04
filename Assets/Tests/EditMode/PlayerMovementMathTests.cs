using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class PlayerMovementMathTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void CalculateJumpVerticalSpeed_DefaultSettings_ReturnsExpectedSpeed()
        {
            float verticalSpeed =
                PlayerMovementMath.CalculateJumpVerticalSpeed(3.0f, 25.0f);

            Assert.That(verticalSpeed, Is.EqualTo(12.247449f).Within(Tolerance));
        }

        [TestCase(10.0f)]
        [TestCase(25.0f)]
        [TestCase(40.0f)]
        public void CalculateJumpVerticalSpeed_GravityChanges_PreservesJumpHeight(
            float gravityAcceleration)
        {
            const float jumpHeight = 3.0f;
            float verticalSpeed =
                PlayerMovementMath.CalculateJumpVerticalSpeed(
                    jumpHeight,
                    gravityAcceleration);
            float calculatedHeight =
                verticalSpeed * verticalSpeed /
                (2.0f * gravityAcceleration);

            Assert.That(calculatedHeight, Is.EqualTo(jumpHeight).Within(Tolerance));
        }

        [Test]
        public void CalculateHorizontalSpeed_GroundedRightInput_AcceleratesRight()
        {
            float speed = CalculateHorizontalSpeed(0.0f, 1.0f, true);

            Assert.That(speed, Is.EqualTo(1.0f).Within(Tolerance));
        }

        [Test]
        public void CalculateHorizontalSpeed_AirborneRightInput_UsesAirAcceleration()
        {
            float speed = CalculateHorizontalSpeed(0.0f, 1.0f, false);

            Assert.That(speed, Is.EqualTo(0.5f).Within(Tolerance));
        }

        [TestCase(8.0f, 0.0f, 7.0f)]
        [TestCase(-8.0f, 0.0f, -7.0f)]
        public void CalculateHorizontalSpeed_NoInput_DeceleratesTowardZero(
            float currentSpeed,
            float input,
            float expectedSpeed)
        {
            float speed = CalculateHorizontalSpeed(currentSpeed, input, true);

            Assert.That(speed, Is.EqualTo(expectedSpeed).Within(Tolerance));
        }

        [TestCase(8.0f, 7.0f, -50.0f)]
        [TestCase(-8.0f, -7.0f, 50.0f)]
        public void CalculateSignedHorizontalAcceleration_ReturnsDirectionAwareSign(
            float previousSpeed,
            float currentSpeed,
            float expectedAcceleration)
        {
            float acceleration =
                PlayerMovementMath.CalculateSignedHorizontalAcceleration(
                    previousSpeed,
                    currentSpeed,
                    0.02f);

            Assert.That(
                acceleration,
                Is.EqualTo(expectedAcceleration).Within(Tolerance));
        }

        [Test]
        public void ConstrainVelocity_RightWallInwardSpeed_RemovesHorizontalSpeed()
        {
            Vector3 velocity = new Vector3(8.0f, -5.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                false,
                CreateRightWallContacts());

            Assert.That(result.x, Is.EqualTo(0.0f).Within(Tolerance));
        }

        [Test]
        public void ConstrainVelocity_RightWallOutwardSpeed_PreservesVelocity()
        {
            Vector3 velocity = new Vector3(-8.0f, -5.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                false,
                CreateRightWallContacts());

            Assert.That(result, Is.EqualTo(velocity));
        }

        [Test]
        public void ConstrainVelocity_LeftWallInwardSpeed_RemovesHorizontalSpeed()
        {
            Vector3 velocity = new Vector3(-8.0f, -5.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                false,
                CreateLeftWallContacts());

            Assert.That(result.x, Is.EqualTo(0.0f).Within(Tolerance));
        }

        [Test]
        public void ConstrainVelocity_DescendingAtWall_PreservesVerticalSpeed()
        {
            Vector3 velocity = new Vector3(8.0f, -5.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                false,
                CreateRightWallContacts());

            Assert.That(result.y, Is.EqualTo(velocity.y).Within(Tolerance));
        }

        [Test]
        public void ConstrainVelocity_AscendingAtWall_PreservesVerticalSpeed()
        {
            Vector3 velocity = new Vector3(8.0f, 12.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                false,
                CreateRightWallContacts());

            Assert.That(result.y, Is.EqualTo(velocity.y).Within(Tolerance));
        }

        [TestCase(-8.0f)]
        [TestCase(8.0f)]
        public void ConstrainVelocity_WallsOnBothSides_RemovesHorizontalSpeed(
            float horizontalSpeed)
        {
            PlayerWallContactState wallContacts = new PlayerWallContactState(
                true,
                Vector3.right,
                true,
                Vector3.left);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(horizontalSpeed, -5.0f, 0.0f),
                false,
                wallContacts);

            Assert.That(result.x, Is.EqualTo(0.0f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(-5.0f).Within(Tolerance));
        }

        [Test]
        public void ConstrainVelocity_GroundedAtWallMovingAway_PreservesMovement()
        {
            PlayerCollisionState collisionState = new PlayerCollisionState(
                true,
                0.0f,
                Vector3.zero,
                Vector3.up,
                CreateRightWallContacts());
            Vector3 velocity = new Vector3(-8.0f, 0.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                collisionState.IsGrounded,
                collisionState.WallContacts);

            Assert.That(collisionState.IsGrounded, Is.True);
            Assert.That(result, Is.EqualTo(velocity));
        }

        [Test]
        public void ConstrainVelocity_GroundedAtWallMovingIntoWall_PreservesMovement()
        {
            PlayerCollisionState collisionState = new PlayerCollisionState(
                true,
                0.0f,
                Vector3.zero,
                Vector3.up,
                CreateRightWallContacts());
            Vector3 velocity = new Vector3(8.0f, 0.0f, 0.0f);

            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                velocity,
                collisionState.IsGrounded,
                collisionState.WallContacts);

            Assert.That(result, Is.EqualTo(velocity));
        }

        [Test]
        public void CalculateVerticalSpeed_Airborne_AccumulatesGravity()
        {
            float speed = PlayerMovementMath.CalculateVerticalSpeed(
                -5.0f,
                false,
                25.0f,
                0.02f);

            Assert.That(speed, Is.EqualTo(-5.5f).Within(Tolerance));
        }

        [Test]
        public void CalculateVerticalSpeed_Grounded_ReturnsZero()
        {
            float speed = PlayerMovementMath.CalculateVerticalSpeed(
                -5.0f,
                true,
                25.0f,
                0.02f);

            Assert.That(speed, Is.EqualTo(0.0f).Within(Tolerance));
        }

        private float CalculateHorizontalSpeed(
            float currentSpeed,
            float input,
            bool isGrounded)
        {
            return PlayerMovementMath.CalculateHorizontalSpeed(
                currentSpeed,
                input,
                isGrounded,
                0.02f,
                8.0f,
                50.0f,
                25.0f,
                14.0f);
        }

        private PlayerWallContactState CreateLeftWallContacts()
        {
            return new PlayerWallContactState(
                true,
                Vector3.right,
                false,
                Vector3.zero);
        }

        private PlayerWallContactState CreateRightWallContacts()
        {
            return new PlayerWallContactState(
                false,
                Vector3.zero,
                true,
                Vector3.left);
        }
    }
}
