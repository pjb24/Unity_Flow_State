using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    // Authoring contract for the four fixed Pattern Prefabs. Runtime pickup uses
    // the serialized Prefab children; tests compare those children to this layout.
    public static class InfiniteCollectibleLayout
    {
        public const float TriggerRadius = 0.5f;
        private const float PlayerCenterAboveSurface = 1.0f;
        private const float InternalTakeoffInset = 0.5f;
        private const float InternalPreInset = 2.0f;
        private const float ExitEdgeX = 20.0f;
        private const float ExitTakeoffX = 19.5f;
        private const float PatternLength = 44.0f;

        public static bool TryGetCount(string patternId, out int count)
        {
            count = 0;
            if (!InfinitePatternGeometry.TryGetSurfaceCount(
                    patternId, out int surfaceCount))
            {
                return false;
            }

            count = 5 + 5 * (surfaceCount - 1);
            return true;
        }

        public static bool TryGetPoint(
            string patternId,
            int index,
            out string id,
            out Vector3 localPosition)
        {
            id = null;
            localPosition = Vector3.zero;
            if (!TryGetCount(patternId, out int count) ||
                index < 0 || index >= count)
            {
                return false;
            }

            float verticalSpeed = PlayerMovementMath.CalculateJumpVerticalSpeed(
                InfinitePatternTraversalMath.JumpHeight,
                InfinitePatternTraversalMath.GravityAcceleration);
            float baseSpeed = InfinitePatternTraversalMath.BaseHorizontalSpeed;
            float gravity = InfinitePatternTraversalMath.GravityAcceleration;

            if (index == 0)
            {
                float flightTime = 2.0f * verticalSpeed / gravity;
                id = "entry-land-01";
                localPosition = new Vector3(
                    Round2(ExitTakeoffX + baseSpeed * flightTime - PatternLength),
                    InfinitePatternGeometry.SurfaceTopBase +
                    PlayerCenterAboveSurface,
                    0.0f);
                return true;
            }

            int internalPointCount = count - 5;
            int offset = index - 1;
            if (offset < internalPointCount)
            {
                int jumpIndex = offset / 5;
                int pointIndex = offset % 5;
                if (!TryGetInternalJump(
                        patternId, jumpIndex, verticalSpeed,
                        out float edge,
                        out float fromTop,
                        out float toTop,
                        out float landingX))
                {
                    return false;
                }

                string prefix = "jump-" + (jumpIndex + 1).ToString("00");
                if (pointIndex == 0)
                {
                    id = prefix + "-pre-01";
                    localPosition = new Vector3(
                        edge - InternalPreInset,
                        fromTop + PlayerCenterAboveSurface,
                        0.0f);
                }
                else if (pointIndex == 4)
                {
                    id = prefix + "-land-01";
                    localPosition = new Vector3(
                        Round2(landingX),
                        toTop + PlayerCenterAboveSurface,
                        0.0f);
                }
                else
                {
                    float takeoffX = edge - InternalTakeoffInset;
                    float x = takeoffX +
                              (landingX - takeoffX) * pointIndex / 4.0f;
                    float time = (x - takeoffX) / baseSpeed;
                    id = prefix + "-air-" + pointIndex.ToString("00");
                    localPosition = new Vector3(
                        Round2(x),
                        Round2(fromTop + PlayerCenterAboveSurface +
                               verticalSpeed * time -
                               0.5f * gravity * time * time),
                        0.0f);
                }

                return true;
            }

            int exitIndex = offset - internalPointCount;
            if (exitIndex == 0)
            {
                id = "exit-pre-01";
                localPosition = new Vector3(
                    ExitEdgeX - 3.0f,
                    InfinitePatternGeometry.SurfaceTopBase +
                    PlayerCenterAboveSurface,
                    0.0f);
            }
            else
            {
                float x = ExitEdgeX + (2 * exitIndex - 1);
                float time = (x - ExitTakeoffX) / baseSpeed;
                id = "exit-air-" + exitIndex.ToString("00");
                localPosition = new Vector3(
                    x,
                    Round2(InfinitePatternGeometry.SurfaceTopBase +
                           PlayerCenterAboveSurface +
                           verticalSpeed * time -
                           0.5f * gravity * time * time),
                    0.0f);
            }

            return true;
        }

        private static bool TryGetInternalJump(
            string patternId,
            int jumpIndex,
            float verticalSpeed,
            out float edge,
            out float fromTop,
            out float toTop,
            out float landingX)
        {
            edge = 0.0f;
            fromTop = 0.0f;
            toTop = 0.0f;
            landingX = 0.0f;
            if (!InfinitePatternGeometry.TryGetSurface(
                    patternId, jumpIndex,
                    out Vector3 fromPosition, out Vector3 fromSize) ||
                !InfinitePatternGeometry.TryGetSurface(
                    patternId, jumpIndex + 1,
                    out Vector3 toPosition, out Vector3 toSize))
            {
                return false;
            }

            edge = fromPosition.x + fromSize.x * 0.5f;
            fromTop = fromPosition.y + fromSize.y * 0.5f;
            toTop = toPosition.y + toSize.y * 0.5f;
            float gravity = InfinitePatternTraversalMath.GravityAcceleration;
            float heightDifference = toTop - fromTop;
            float discriminant = verticalSpeed * verticalSpeed -
                                 2.0f * gravity * heightDifference;
            if (discriminant < 0.0f)
            {
                return false;
            }

            float descendingTime =
                (verticalSpeed + Mathf.Sqrt(discriminant)) / gravity;
            landingX = edge - InternalTakeoffInset +
                       InfinitePatternTraversalMath.BaseHorizontalSpeed *
                       descendingTime;
            float landingStart = toPosition.x - toSize.x * 0.5f +
                                 InfinitePatternTraversalMath.PlayerRadius;
            float landingEnd = toPosition.x + toSize.x * 0.5f -
                               InfinitePatternTraversalMath.PlayerRadius;
            return landingX >= landingStart && landingX <= landingEnd;
        }

        private static float Round2(float value)
        {
            return Mathf.Round(value * 100.0f) / 100.0f;
        }
    }
}
