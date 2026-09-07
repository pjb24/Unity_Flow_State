namespace FlowState.Runtime.Core
{
    public readonly struct PlayerInputState
    {
        public bool IsJumpPressed { get; }

        public bool IsMomentumLandingPressed { get; }

        public PlayerInputState(
            bool isJumpPressed,
            bool isMomentumLandingPressed)
        {
            IsJumpPressed = isJumpPressed;
            IsMomentumLandingPressed = isMomentumLandingPressed;
        }
    }
}
