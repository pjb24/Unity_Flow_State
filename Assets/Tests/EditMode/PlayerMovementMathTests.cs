using FlowState.Runtime.Core;
using NUnit.Framework;

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
    }
}
