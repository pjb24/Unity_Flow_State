using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class NormalLandingFeature : MonoBehaviour
    {
        private bool _isJumpActive;
        private bool _hasLandingResolved;

        public void Initialize()
        {
            ResetState();
        }

        public void BeginJump()
        {
            ResetState();
            _isJumpActive = true;
        }

        public bool TryCompleteLanding(
            in PlayerCollisionState collisionState,
            bool isMomentumLandingCompleted)
        {
            if (!_isJumpActive || _hasLandingResolved || !collisionState.IsGrounded)
            {
                return false;
            }

            _hasLandingResolved = true;

            return !isMomentumLandingCompleted;
        }

        private void ResetState()
        {
            _isJumpActive = false;
            _hasLandingResolved = false;
        }
    }
}
