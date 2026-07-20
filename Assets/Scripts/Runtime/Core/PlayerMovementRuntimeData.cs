namespace FlowState.Runtime.Core
{
    public class PlayerMovementRuntimeData
    {
        private E_PlayerMovementState _currentMovementState;
        private float _currentHorizontalSpeed;
        private float _currentVerticalSpeed;
        private bool _isGrounded;
        private bool _isMomentumLandingWindowActive;
        private bool _isLastLandingMomentum;

        public E_PlayerMovementState CurrentMovementState => _currentMovementState;

        public float CurrentHorizontalSpeed => _currentHorizontalSpeed;

        public float CurrentVerticalSpeed => _currentVerticalSpeed;

        public bool IsGrounded => _isGrounded;

        public bool IsMomentumLandingWindowActive => _isMomentumLandingWindowActive;

        public bool IsLastLandingMomentum => _isLastLandingMomentum;

        public void Initialize()
        {
            _currentMovementState = E_PlayerMovementState.None;
            _currentHorizontalSpeed = 0.0f;
            _currentVerticalSpeed = 0.0f;
            _isGrounded = false;
            _isMomentumLandingWindowActive = false;
            _isLastLandingMomentum = false;
        }

        public void UpdateState(
            E_PlayerMovementState movementState,
            float horizontalSpeed,
            float verticalSpeed,
            bool isGrounded,
            bool isMomentumLandingWindowActive,
            bool isLastLandingMomentum)
        {
            _currentMovementState = movementState;
            _currentHorizontalSpeed = horizontalSpeed;
            _currentVerticalSpeed = verticalSpeed;
            _isGrounded = isGrounded;
            _isMomentumLandingWindowActive = isMomentumLandingWindowActive;
            _isLastLandingMomentum = isLastLandingMomentum;
        }
    }
}
