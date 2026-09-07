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

        [TestCase(true, 1.0f)]
        [TestCase(false, 0.5f)]
        public void CalculateAutoHorizontalSpeed_AtRest_UsesGroundOrAirAcceleration(
            bool isGrounded,
            float expectedSpeed)
        {
            float speed = CalculateAutoHorizontalSpeed(0.0f, isGrounded);

            Assert.That(speed, Is.EqualTo(expectedSpeed).Within(Tolerance));
        }

        [TestCase(7.9f, 8.0f)]
        [TestCase(8.0f, 8.0f)]
        [TestCase(9.2f, 9.2f)]
        [TestCase(14.0f, 14.0f)]
        [TestCase(20.0f, 14.0f)]
        public void CalculateAutoHorizontalSpeed_TargetAndMomentum_RespectSeparateLimits(
            float currentSpeed,
            float expectedSpeed)
        {
            float speed = CalculateAutoHorizontalSpeed(currentSpeed, true);

            Assert.That(speed, Is.EqualTo(expectedSpeed).Within(Tolerance));
        }

        [TestCase(true, -7.0f)]
        [TestCase(false, -7.5f)]
        public void CalculateAutoHorizontalSpeed_NegativeVelocity_AcceleratesRight(
            bool isGrounded,
            float expectedSpeed)
        {
            float speed = CalculateAutoHorizontalSpeed(-8.0f, isGrounded);

            Assert.That(speed, Is.EqualTo(expectedSpeed).Within(Tolerance));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CalculateAutoHorizontalSpeed_GroundAirTransition_PreservesMomentum(
            bool startsGrounded)
        {
            float speed = CalculateAutoHorizontalSpeed(9.2f, startsGrounded);
            speed = CalculateAutoHorizontalSpeed(speed, !startsGrounded);

            Assert.That(speed, Is.EqualTo(9.2f).Within(Tolerance));
        }

        [Test]
        public void CalculateAutoHorizontalSpeed_NegativeVelocity_EventuallyReachesPositiveTarget()
        {
            float speed = -8.0f;

            for (int step = 0; step < 40; step++)
            {
                speed = CalculateAutoHorizontalSpeed(speed, false);
            }

            Assert.That(speed, Is.EqualTo(8.0f).Within(Tolerance));
        }

        [TestCase(0.0f)]
        [TestCase(-0.02f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void CalculateAutoHorizontalSpeed_InvalidDeltaTime_DoesNotAdvance(
            float deltaTime)
        {
            float speed = PlayerMovementMath.CalculateAutoHorizontalSpeed(
                -4.0f, true, deltaTime, 8.0f, 50.0f, 25.0f, 14.0f);

            Assert.That(speed, Is.EqualTo(-4.0f));
        }

        [TestCase(-1.0f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void CalculateAutoHorizontalSpeed_InvalidSettings_UseZeroForInvalidValue(
            float invalidValue)
        {
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    0.0f, true, 0.02f, invalidValue, 50.0f, 25.0f, 14.0f),
                Is.Zero, "An invalid base speed must not start acceleration.");
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    4.0f, true, 0.02f, 8.0f, invalidValue, 25.0f, 14.0f),
                Is.EqualTo(4.0f), "An invalid ground acceleration must not advance speed.");
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    4.0f, false, 0.02f, 8.0f, 50.0f, invalidValue, 14.0f),
                Is.EqualTo(4.0f), "An invalid air acceleration must not advance speed.");
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    4.0f, true, 0.02f, 8.0f, 50.0f, 25.0f, invalidValue),
                Is.Zero, "An invalid maximum speed must enforce a zero limit.");
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void CalculateAutoHorizontalSpeed_NonFiniteVelocity_RestartsFromZero(
            float currentSpeed)
        {
            Assert.That(
                CalculateAutoHorizontalSpeed(currentSpeed, true),
                Is.EqualTo(1.0f).Within(Tolerance));
        }

        [Test]
        public void CalculateAutoHorizontalSpeed_UnusedAcceleration_DoesNotAffectSelectedState()
        {
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    0.0f, true, 0.02f, 8.0f, 50.0f, float.NaN, 14.0f),
                Is.EqualTo(1.0f).Within(Tolerance));
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    0.0f, false, 0.02f, 8.0f, float.NaN, 25.0f, 14.0f),
                Is.EqualTo(0.5f).Within(Tolerance));
        }

        [Test]
        public void CalculateAutoHorizontalSpeed_BaseAboveMaximum_StopsAtMaximum()
        {
            float speed = PlayerMovementMath.CalculateAutoHorizontalSpeed(
                0.0f, true, 1.0f, 20.0f, 50.0f, 25.0f, 14.0f);

            Assert.That(speed, Is.EqualTo(14.0f));
        }

        [TestCase(-20.0f, -14.0f)]
        [TestCase(20.0f, 14.0f)]
        public void CalculateAutoHorizontalSpeed_ZeroTime_StillEnforcesMaximum(
            float currentSpeed,
            float expectedSpeed)
        {
            float speed = PlayerMovementMath.CalculateAutoHorizontalSpeed(
                currentSpeed, true, 0.0f, 8.0f, 50.0f, 25.0f, 14.0f);

            Assert.That(speed, Is.EqualTo(expectedSpeed));
        }

        [Test]
        public void CalculateAutoHorizontalSpeed_ZeroSettings_PreserveMomentumWithinLimit()
        {
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    9.2f, true, 0.02f, 0.0f, 50.0f, 25.0f, 14.0f),
                Is.EqualTo(9.2f).Within(Tolerance));
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    -4.0f, true, 0.02f, 8.0f, 0.0f, 25.0f, 14.0f),
                Is.EqualTo(-4.0f));
            Assert.That(
                PlayerMovementMath.CalculateAutoHorizontalSpeed(
                    9.2f, true, 0.02f, 8.0f, 50.0f, 25.0f, 0.0f),
                Is.Zero);
        }

        [Test]
        public void CalculateAutoHorizontalSpeed_LargeFiniteStep_ReturnsFiniteTarget()
        {
            float speed = PlayerMovementMath.CalculateAutoHorizontalSpeed(
                -float.MaxValue, true, float.MaxValue,
                float.MaxValue, float.MaxValue, 25.0f, float.MaxValue);

            Assert.That(speed, Is.EqualTo(float.MaxValue));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CalculateAutoHorizontalSpeed_EqualElapsedTime_ProducesSameSpeed(
            bool isGrounded)
        {
            float singleStep = PlayerMovementMath.CalculateAutoHorizontalSpeed(
                -4.0f, isGrounded, 0.2f, 8.0f, 50.0f, 25.0f, 14.0f);
            float multipleSteps = -4.0f;

            for (int step = 0; step < 10; step++)
            {
                multipleSteps = CalculateAutoHorizontalSpeed(multipleSteps, isGrounded);
            }

            Assert.That(multipleSteps, Is.EqualTo(singleStep).Within(Tolerance));
            Assert.That(
                singleStep,
                Is.EqualTo(isGrounded ? 6.0f : 1.0f).Within(Tolerance));
        }

        [Test]
        public void AutoMovement_RightWallThenExit_ResumesAirAccelerationAndGravity()
        {
            float speed = CalculateAutoHorizontalSpeed(8.0f, false);
            float verticalSpeed = PlayerMovementMath.CalculateVerticalSpeed(
                -5.0f, false, 25.0f, 0.02f);
            Vector3 blocked = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(speed, verticalSpeed, 0.0f), false, CreateRightWallContacts());

            Assert.That(blocked.x, Is.Zero);
            Assert.That(blocked.y, Is.EqualTo(-5.5f).Within(Tolerance));

            float recoveredSpeed = CalculateAutoHorizontalSpeed(blocked.x, false);
            Vector3 released = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(recoveredSpeed, blocked.y, 0.0f), false, default);

            Assert.That(released.x, Is.EqualTo(0.5f).Within(Tolerance));
            Assert.That(released.y, Is.EqualTo(blocked.y));
        }

        [Test]
        public void AutoMovement_GroundAndWall_PreservesGroundAcceleration()
        {
            float speed = CalculateAutoHorizontalSpeed(0.0f, true);
            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(speed, 0.0f, 0.0f), true, CreateRightWallContacts());

            Assert.That(result.x, Is.EqualTo(1.0f).Within(Tolerance));
        }

        [Test]
        public void AutoMovement_LeftWall_DoesNotBlockPositiveAcceleration()
        {
            float speed = CalculateAutoHorizontalSpeed(0.0f, false);
            Vector3 result = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(speed, -5.0f, 0.0f), false, CreateLeftWallContacts());

            Assert.That(result.x, Is.EqualTo(0.5f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(-5.0f));
        }

        private float CalculateAutoHorizontalSpeed(float currentSpeed, bool isGrounded)
        {
            return PlayerMovementMath.CalculateAutoHorizontalSpeed(
                currentSpeed, isGrounded, 0.02f, 8.0f, 50.0f, 25.0f, 14.0f);
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
