using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class JumpFeature : MonoBehaviour
    {
        [SerializeField] private float _jumpHeight = 3.0f;
        [SerializeField] private float _coyoteTime = 0.1f;

        private float _remainingCoyoteTime;
        private bool _hasJumpStarted;

        public bool CanJump => !_hasJumpStarted && _remainingCoyoteTime > 0.0f;

        public void Initialize()
        {
            _remainingCoyoteTime = 0.0f;
            _hasJumpStarted = false;
        }

        public void UpdateCoyoteTime(
            E_PlayerMovementState movementState,
            float deltaTime)
        {
            if (movementState == E_PlayerMovementState.Grounded)
            {
                _remainingCoyoteTime = Mathf.Max(0.0f, _coyoteTime);
                return;
            }

            _remainingCoyoteTime = Mathf.Max(
                0.0f,
                _remainingCoyoteTime - Mathf.Max(0.0f, deltaTime));
        }

        public bool TryStartJump(
            in PlayerInputState inputState,
            float gravityAcceleration,
            out float verticalSpeed)
        {
            verticalSpeed = 0.0f;

            if (!inputState.IsJumpPressed || !CanJump)
            {
                return false;
            }

            verticalSpeed = PlayerMovementMath.CalculateJumpVerticalSpeed(
                _jumpHeight,
                gravityAcceleration);

            _remainingCoyoteTime = 0.0f;
            _hasJumpStarted = true;

            return true;
        }

        public void CompleteLanding()
        {
            _remainingCoyoteTime = Mathf.Max(0.0f, _coyoteTime);
            _hasJumpStarted = false;
        }
    }
}
