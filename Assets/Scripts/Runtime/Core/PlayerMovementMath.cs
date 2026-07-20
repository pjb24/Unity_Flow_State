using UnityEngine;

namespace FlowState.Runtime.Core
{
    public static class PlayerMovementMath
    {
        public static float CalculateJumpVerticalSpeed(
            float jumpHeight,
            float gravityAcceleration)
        {
            jumpHeight = Mathf.Max(0.0f, jumpHeight);
            gravityAcceleration = Mathf.Max(0.0f, gravityAcceleration);

            return Mathf.Sqrt(
                2.0f * gravityAcceleration * jumpHeight);
        }

        public static float CalculateHorizontalSpeed(
            float currentSpeed,
            float horizontalInput,
            bool isGrounded,
            float deltaTime,
            float moveSpeed,
            float groundAcceleration,
            float airAcceleration,
            float maximumHorizontalSpeed)
        {
            float clampedInput = Mathf.Clamp(horizontalInput, -1.0f, 1.0f);
            float targetSpeed = clampedInput * Mathf.Max(0.0f, moveSpeed);
            bool isContinuingDirection =
                !Mathf.Approximately(clampedInput, 0.0f) &&
                Mathf.Sign(clampedInput) == Mathf.Sign(currentSpeed);

            if (isContinuingDirection)
            {
                targetSpeed = Mathf.Sign(clampedInput) * Mathf.Max(
                    Mathf.Abs(targetSpeed),
                    Mathf.Abs(currentSpeed));
            }

            float acceleration = isGrounded
                ? Mathf.Max(0.0f, groundAcceleration)
                : Mathf.Max(0.0f, airAcceleration);
            float speed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                acceleration * Mathf.Max(0.0f, deltaTime));
            float maximumSpeed = Mathf.Max(0.0f, maximumHorizontalSpeed);

            return Mathf.Clamp(speed, -maximumSpeed, maximumSpeed);
        }

        public static float CalculateSignedHorizontalAcceleration(
            float previousSpeed,
            float currentSpeed,
            float deltaTime)
        {
            float safeDeltaTime = Mathf.Max(deltaTime, Mathf.Epsilon);

            return (currentSpeed - previousSpeed) / safeDeltaTime;
        }
    }
}
