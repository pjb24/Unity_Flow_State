namespace FlowState.Runtime.Core
{
    public readonly struct NavigationInput
    {
        public long Sequence { get; }

        public int VerticalDirection { get; }

        public bool IsSubmitPressed { get; }

        public bool IsCancelPressed { get; }

        public bool IsClickPressed { get; }

        public double Time { get; }

        public NavigationInput(
            long sequence,
            int verticalDirection,
            bool isSubmitPressed,
            bool isCancelPressed,
            bool isClickPressed,
            double time)
        {
            Sequence = sequence;
            VerticalDirection = verticalDirection;
            IsSubmitPressed = isSubmitPressed;
            IsCancelPressed = isCancelPressed;
            IsClickPressed = isClickPressed;
            Time = time;
        }
    }
}
