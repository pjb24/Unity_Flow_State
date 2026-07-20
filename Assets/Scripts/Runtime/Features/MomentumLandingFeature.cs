using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class MomentumLandingFeature : MonoBehaviour
    {
        [SerializeField] private float _momentumLandingWindow = 0.15f;
        [SerializeField] private float _speedMultiplier = 1.15f;
        [SerializeField] private float _maximumHorizontalSpeed = 14.0f;

        private float _remainingWindowTime;
        private bool _isJumpActive;
        private bool _isWindowActive;
        private bool _hasWindowActivated;
        private bool _hasBufferedInput;
        private bool _hasLandingResolved;

        public bool IsWindowActive => _isWindowActive;

        public void Initialize()
        {
            ResetState();
        }

        public void BeginJump()
        {
            ResetState();
            _isJumpActive = true;
        }

        public void UpdateWindow(
            float verticalSpeed,
            in PlayerCollisionState collisionState,
            float deltaTime)
        {
            if (!_isJumpActive || _hasLandingResolved)
            {
                return;
            }

            if (_isWindowActive)
            {
                _remainingWindowTime = Mathf.Max(
                    0.0f,
                    _remainingWindowTime - Mathf.Max(0.0f, deltaTime));
                _isWindowActive = _remainingWindowTime > 0.0f;
            }

            if (_hasWindowActivated || collisionState.IsGrounded || verticalSpeed >= 0.0f)
            {
                return;
            }

            if (!IsValidGroundDistance(collisionState.GroundDistance))
            {
                return;
            }

            float descendingSpeed = -verticalSpeed;
            float timeToGround = collisionState.GroundDistance / descendingSpeed;
            float windowDuration = Mathf.Max(0.0f, _momentumLandingWindow);

            if (collisionState.GroundDistance < 0.0f || timeToGround > windowDuration)
            {
                return;
            }

            _remainingWindowTime = windowDuration;
            _isWindowActive = windowDuration > 0.0f;
            _hasWindowActivated = true;
        }

        public void BufferInput(in PlayerInputState inputState)
        {
            if (_isWindowActive && inputState.IsMomentumLandingPressed)
            {
                _hasBufferedInput = true;
            }
        }

        public bool TryCompleteLanding(
            in PlayerCollisionState collisionState,
            float horizontalSpeed,
            out float resultHorizontalSpeed)
        {
            resultHorizontalSpeed = horizontalSpeed;

            if (!_isJumpActive || _hasLandingResolved || !collisionState.IsGrounded)
            {
                return false;
            }

            _hasLandingResolved = true;
            _isWindowActive = false;

            if (!_hasBufferedInput)
            {
                return false;
            }

            float speedMultiplier = Mathf.Max(1.0f, _speedMultiplier);
            float maximumSpeed = Mathf.Max(0.0f, _maximumHorizontalSpeed);
            float speedMagnitude = Mathf.Min(
                Mathf.Abs(horizontalSpeed) * speedMultiplier,
                maximumSpeed);

            resultHorizontalSpeed = Mathf.Sign(horizontalSpeed) * speedMagnitude;

            return true;
        }

        private bool IsValidGroundDistance(float groundDistance)
        {
            return groundDistance >= 0.0f &&
                   !float.IsInfinity(groundDistance) &&
                   !float.IsNaN(groundDistance);
        }

        private void ResetState()
        {
            _remainingWindowTime = 0.0f;
            _isJumpActive = false;
            _isWindowActive = false;
            _hasWindowActivated = false;
            _hasBufferedInput = false;
            _hasLandingResolved = false;
        }
    }
}
