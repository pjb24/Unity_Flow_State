using UnityEngine;

namespace FlowState.Runtime.Core
{
    public readonly struct UIInputState
    {
        public Vector2 NavigateInput { get; }

        public Vector2 PointerPosition { get; }

        public bool IsPointChanged { get; }

        public bool IsSubmitPressed { get; }

        public bool IsCancelPressed { get; }

        public bool IsClickPressed { get; }

        public UIInputState(
            Vector2 navigateInput,
            Vector2 pointerPosition,
            bool isPointChanged,
            bool isSubmitPressed,
            bool isCancelPressed,
            bool isClickPressed)
        {
            NavigateInput = navigateInput;
            PointerPosition = pointerPosition;
            IsPointChanged = isPointChanged;
            IsSubmitPressed = isSubmitPressed;
            IsCancelPressed = isCancelPressed;
            IsClickPressed = isClickPressed;
        }
    }
}
