using FlowState.Input;
using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlowState.Runtime.Systems
{
    public class UIInputSystem : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        private Vector2 _navigateInput;
        private Vector2 _pointerPosition;
        private bool _isPointChanged;
        private bool _isSubmitPressed;
        private bool _isCancelPressed;
        private bool _isClickPressed;
        private bool _isInitialized;

        public bool IsUIActionMapEnabled =>
            _inputActions != null && _inputActions.UI.enabled;

        private void OnDestroy()
        {
            if (_inputActions == null)
            {
                return;
            }

            _inputActions.Disable();
            UnregisterCallbacks();
            _inputActions.Dispose();
            _inputActions = null;
            ResetInputState();
            _isInitialized = false;
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _inputActions = new InputSystem_Actions();
            RegisterCallbacks();
            ResetInputState();
            _isInitialized = true;
        }

        public void EnableUIActionMap()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            ResetInputState();
            _inputActions.UI.Enable();
        }

        public void DisableUIActionMap()
        {
            if (!_isInitialized)
            {
                return;
            }

            _inputActions.UI.Disable();
            ResetInputState();
        }

        public UIInputState GetInputState()
        {
            return new UIInputState(
                _navigateInput,
                _pointerPosition,
                _isPointChanged,
                _isSubmitPressed,
                _isCancelPressed,
                _isClickPressed);
        }

        public void ConsumeTransientInput()
        {
            _navigateInput = Vector2.zero;
            _isPointChanged = false;
            _isSubmitPressed = false;
            _isCancelPressed = false;
            _isClickPressed = false;
        }

        private void RegisterCallbacks()
        {
            _inputActions.UI.Navigate.performed += OnNavigatePerformed;
            _inputActions.UI.Navigate.canceled += OnNavigateCanceled;
            _inputActions.UI.Submit.performed += OnSubmitPerformed;
            _inputActions.UI.Cancel.performed += OnCancelPerformed;
            _inputActions.UI.Point.performed += OnPointPerformed;
            _inputActions.UI.Point.canceled += OnPointCanceled;
            _inputActions.UI.Click.performed += OnClickPerformed;
        }

        private void UnregisterCallbacks()
        {
            _inputActions.UI.Navigate.performed -= OnNavigatePerformed;
            _inputActions.UI.Navigate.canceled -= OnNavigateCanceled;
            _inputActions.UI.Submit.performed -= OnSubmitPerformed;
            _inputActions.UI.Cancel.performed -= OnCancelPerformed;
            _inputActions.UI.Point.performed -= OnPointPerformed;
            _inputActions.UI.Point.canceled -= OnPointCanceled;
            _inputActions.UI.Click.performed -= OnClickPerformed;
        }

        private void ResetInputState()
        {
            _navigateInput = Vector2.zero;
            _pointerPosition = Vector2.zero;
            _isPointChanged = false;
            _isSubmitPressed = false;
            _isCancelPressed = false;
            _isClickPressed = false;
        }

        private void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            _navigateInput = context.ReadValue<Vector2>();
        }

        private void OnNavigateCanceled(InputAction.CallbackContext context)
        {
            _navigateInput = Vector2.zero;
        }

        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            _isSubmitPressed = true;
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            _isCancelPressed = true;
        }

        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            _pointerPosition = context.ReadValue<Vector2>();
            _isPointChanged = true;
        }

        private void OnPointCanceled(InputAction.CallbackContext context)
        {
            _pointerPosition = Vector2.zero;
            _isPointChanged = false;
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                _isClickPressed = true;
            }
        }
    }
}
