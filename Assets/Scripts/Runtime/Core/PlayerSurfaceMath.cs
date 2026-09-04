using UnityEngine;

namespace FlowState.Runtime.Core
{
    public static class PlayerSurfaceMath
    {
        private const float AngleTolerance = 0.0001f;

        public const float MaximumGroundAngle = 45.0f;
        public const float MinimumWallAngle = 80.0f;
        public const float MaximumWallAngle = 100.0f;

        public static bool IsValidNormal(Vector3 normal)
        {
            if (!IsFinite(normal.x) ||
                !IsFinite(normal.y) ||
                !IsFinite(normal.z))
            {
                return false;
            }

            float magnitude = normal.magnitude;

            return IsFinite(magnitude) && magnitude > Mathf.Epsilon;
        }

        public static bool IsGroundSurface(Vector3 normal)
        {
            if (!TryGetUpAngle(normal, out float angle))
            {
                return false;
            }

            return angle <= MaximumGroundAngle + AngleTolerance;
        }

        public static bool IsWallSurface(Vector3 normal)
        {
            if (!TryGetUpAngle(normal, out float angle))
            {
                return false;
            }

            return angle >= MinimumWallAngle - AngleTolerance &&
                   angle <= MaximumWallAngle + AngleTolerance;
        }

        private static bool TryGetUpAngle(Vector3 normal, out float angle)
        {
            angle = 0.0f;

            if (!IsValidNormal(normal))
            {
                return false;
            }

            angle = Vector3.Angle(normal, Vector3.up);
            return IsFinite(angle);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
