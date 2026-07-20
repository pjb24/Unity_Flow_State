namespace FlowState.Runtime.Core
{
    public readonly struct PlayerInputState
    {
        public float HorizontalInput { get; }

        public bool IsJumpPressed { get; }

        public bool IsMomentumLandingPressed { get; }

        public PlayerInputState(
            float horizontalInput,
            bool isJumpPressed,
            bool isMomentumLandingPressed)
        {
            HorizontalInput = horizontalInput;
            IsJumpPressed = isJumpPressed;
            IsMomentumLandingPressed = isMomentumLandingPressed;
        }
    }
}
