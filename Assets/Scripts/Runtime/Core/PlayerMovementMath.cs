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

        public static float CalculateFixedHorizontalSpeed(float moveSpeed)
        {
            return SanitizeNonNegative(moveSpeed);
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

        private static float SanitizeNonNegative(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return 0.0f;
            }

            return Mathf.Max(0.0f, value);
        }
    }
}
