using FlowState.Input;
using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlowState.Runtime.Systems
{
    public class PlayerInputSystem : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        private bool _isJumpPressed;
        private bool _isMomentumLandingPressed;
        private bool _isInitialized;

        public bool IsPlayerActionMapEnabled =>
            _inputActions != null && _inputActions.Player.enabled;

        public bool IsMoveActionEnabled =>
            _inputActions != null && _inputActions.Player.Move.enabled;

        public bool IsJumpActionEnabled =>
            _inputActions != null && _inputActions.Player.Jump.enabled;

        public bool IsMomentumLandingActionEnabled =>
            _inputActions != null &&
            _inputActions.Player.MomentumLanding.enabled;

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
            _inputActions.Player.Move.Disable();
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
            _inputActions.Player.Jump.performed += OnJumpPerformed;
            _inputActions.Player.MomentumLanding.performed += OnMomentumLandingPerformed;
        }

        private void UnregisterCallbacks()
        {
            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.MomentumLanding.performed -= OnMomentumLandingPerformed;
        }

        private void ResetInputState()
        {
            _isJumpPressed = false;
            _isMomentumLandingPressed = false;
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
