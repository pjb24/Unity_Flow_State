using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public static class InfinitePatternTraversalMath
    {
        public const float BaseHorizontalSpeed = 8.0f;
        public const float MaximumHorizontalSpeed = 14.0f;
        public const float JumpHeight = 3.0f;
        public const float GravityAcceleration = 25.0f;
        public const float PlayerRadius = 0.5f;
        public const float PlayerHeight = 2.0f;
        public const float MinimumJumpInputWindow = 0.10f;
        public const float PositionTolerance = 0.01f;

        public static bool IsContinuousGround(
            float horizontalGap,
            float heightDifference)
        {
            return IsFinite(horizontalGap) &&
                   IsFinite(heightDifference) &&
                   Mathf.Abs(horizontalGap) <= PositionTolerance &&
                   Mathf.Abs(heightDifference) <= PositionTolerance;
        }

        public static bool CanTraverseJump(
            float gapWidth,
            float landingHeightDifference,
            float takeoffRunwayLength,
            float landingSurfaceLength)
        {
            if (!TryCalculateJumpInputWindow(
                    gapWidth,
                    landingHeightDifference,
                    takeoffRunwayLength,
                    landingSurfaceLength,
                    BaseHorizontalSpeed,
                    out float baseSpeedWindow) ||
                !TryCalculateJumpInputWindow(
                    gapWidth,
                    landingHeightDifference,
                    takeoffRunwayLength,
                    landingSurfaceLength,
                    MaximumHorizontalSpeed,
                    out float maximumSpeedWindow))
            {
                return false;
            }

            return baseSpeedWindow >= MinimumJumpInputWindow &&
                   maximumSpeedWindow >= MinimumJumpInputWindow;
        }

        public static bool TryCalculateJumpInputWindow(
            float gapWidth,
            float landingHeightDifference,
            float takeoffRunwayLength,
            float landingSurfaceLength,
            float horizontalSpeed,
            out float inputWindowDuration)
        {
            inputWindowDuration = 0.0f;

            if (!IsFinite(gapWidth) ||
                !IsFinite(landingHeightDifference) ||
                !IsFinite(takeoffRunwayLength) ||
                !IsFinite(landingSurfaceLength) ||
                !IsFinite(horizontalSpeed) ||
                gapWidth < 0.0f ||
                takeoffRunwayLength < 0.0f ||
                landingSurfaceLength < PlayerRadius * 2.0f ||
                horizontalSpeed <= 0.0f)
            {
                return false;
            }

            float verticalSpeed =
                PlayerMovementMath.CalculateJumpVerticalSpeed(
                    JumpHeight,
                    GravityAcceleration);
            float discriminant =
                verticalSpeed * verticalSpeed -
                2.0f * GravityAcceleration * landingHeightDifference;

            if (discriminant < 0.0f)
            {
                return false;
            }

            float descendingLandingTime =
                (verticalSpeed + Mathf.Sqrt(discriminant)) /
                GravityAcceleration;
            float horizontalTravel = horizontalSpeed * descendingLandingTime;
            float firstLandingCenter = gapWidth + PlayerRadius;
            float lastLandingCenter =
                gapWidth + landingSurfaceLength - PlayerRadius;
            float earliestTakeoffDistance = Mathf.Max(
                0.0f,
                horizontalTravel - lastLandingCenter);
            float latestTakeoffDistance = Mathf.Min(
                takeoffRunwayLength,
                horizontalTravel - firstLandingCenter);
            float validTakeoffDistance = Mathf.Max(
                0.0f,
                latestTakeoffDistance - earliestTakeoffDistance);

            inputWindowDuration = validTakeoffDistance / horizontalSpeed;
            return inputWindowDuration > 0.0f;
        }

        public static bool CanClearObstacle(
            float takeoffDistanceBeforeEdge,
            float obstacleDistanceAfterEdge,
            float obstacleTopHeight,
            float horizontalSpeed)
        {
            if (!IsFinite(takeoffDistanceBeforeEdge) ||
                !IsFinite(obstacleDistanceAfterEdge) ||
                !IsFinite(obstacleTopHeight) ||
                !IsFinite(horizontalSpeed) ||
                takeoffDistanceBeforeEdge < 0.0f ||
                obstacleDistanceAfterEdge < 0.0f ||
                obstacleTopHeight < 0.0f ||
                horizontalSpeed <= 0.0f)
            {
                return false;
            }

            float horizontalDistance =
                takeoffDistanceBeforeEdge + obstacleDistanceAfterEdge;
            float time = horizontalDistance / horizontalSpeed;
            float verticalSpeed =
                PlayerMovementMath.CalculateJumpVerticalSpeed(
                    JumpHeight,
                    GravityAcceleration);
            float playerBottomHeight =
                verticalSpeed * time -
                0.5f * GravityAcceleration * time * time;

            return playerBottomHeight + PositionTolerance >= obstacleTopHeight;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
