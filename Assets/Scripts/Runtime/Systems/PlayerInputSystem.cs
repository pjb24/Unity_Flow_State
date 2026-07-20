using FlowState.Input;
using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlowState.Runtime.Systems
{
    public class PlayerInputSystem : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        private Vector2 _moveInput;
        private bool _isJumpPressed;
        private bool _isMomentumLandingPressed;
        private bool _isInitialized;

        public bool IsPlayerActionMapEnabled =>
            _inputActions != null && _inputActions.Player.enabled;

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

        public void EnablePlayerActionMap()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            ResetInputState();
            _inputActions.Player.Enable();
        }

        public void DisablePlayerActionMap()
        {
            if (!_isInitialized)
            {
                return;
            }

            _inputActions.Player.Disable();
            ResetInputState();
        }

        public PlayerInputState GetInputState()
        {
            return new PlayerInputState(
                _moveInput.x,
                _isJumpPressed,
                _isMomentumLandingPressed);
        }

        public void ConsumeTransientInput()
        {
            _isJumpPressed = false;
            _isMomentumLandingPressed = false;
        }

        private void RegisterCallbacks()
        {
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Jump.performed += OnJumpPerformed;
            _inputActions.Player.MomentumLanding.performed += OnMomentumLandingPerformed;
        }

        private void UnregisterCallbacks()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.MomentumLanding.performed -= OnMomentumLandingPerformed;
        }

        private void ResetInputState()
        {
            _moveInput = Vector2.zero;
            _isJumpPressed = false;
            _isMomentumLandingPressed = false;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _isJumpPressed = true;
        }

        private void OnMomentumLandingPerformed(InputAction.CallbackContext context)
        {
            _isMomentumLandingPressed = true;
        }
    }
}
