using UnityEngine;

namespace FlowState.Runtime.Core
{
    public readonly struct PlayerCollisionState
    {
        public bool IsGrounded { get; }

        public float GroundDistance { get; }

        public Vector3 ContactPoint { get; }

        public Vector3 SurfaceNormal { get; }

        public PlayerCollisionState(
            bool isGrounded,
            float groundDistance,
            Vector3 contactPoint,
            Vector3 surfaceNormal)
        {
            IsGrounded = isGrounded;
            GroundDistance = groundDistance;
            ContactPoint = contactPoint;
            SurfaceNormal = surfaceNormal;
        }
    }
}
