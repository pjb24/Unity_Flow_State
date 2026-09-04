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

        public static float CalculateVerticalSpeed(
            float currentVerticalSpeed,
            bool isGrounded,
            float gravityAcceleration,
            float deltaTime)
        {
            if (isGrounded)
            {
                return 0.0f;
            }

            return currentVerticalSpeed -
                   Mathf.Max(0.0f, gravityAcceleration) *
                   Mathf.Max(0.0f, deltaTime);
        }

        public static Vector3 ConstrainVelocityByWalls(
            Vector3 velocity,
            bool isGrounded,
            in PlayerWallContactState wallContacts)
        {
            if (isGrounded)
            {
                return velocity;
            }

            bool isMovingIntoLeftWall =
                velocity.x < 0.0f && wallContacts.HasLeftWall;
            bool isMovingIntoRightWall =
                velocity.x > 0.0f && wallContacts.HasRightWall;

            if (isMovingIntoLeftWall || isMovingIntoRightWall)
            {
                velocity.x = 0.0f;
            }

            return velocity;
        }
    }
}
