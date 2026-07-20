using UnityEngine;

namespace FlowState.Runtime.Core
{
    public readonly struct PlayerMovementResult
    {
        public Vector3 Velocity { get; }

        public E_PlayerMovementState MovementState { get; }

        public bool IsJumpStarted { get; }

        public bool IsLandingOccurred { get; }

        public E_PlayerMovementState LandingState { get; }

        public PlayerMovementResult(
            Vector3 velocity,
            E_PlayerMovementState movementState,
            bool isJumpStarted,
            bool isLandingOccurred,
            E_PlayerMovementState landingState)
        {
            Velocity = velocity;
            MovementState = movementState;
            IsJumpStarted = isJumpStarted;
            IsLandingOccurred = isLandingOccurred;
            LandingState = landingState;
        }
    }
}
