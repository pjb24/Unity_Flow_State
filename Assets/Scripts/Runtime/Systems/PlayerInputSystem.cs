using System;
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

        public bool TryApplyBindingOverride(
            SettingsBindingTarget target,
            string controlPath)
        {
            return TrySetBindingOverride(target, controlPath, false);
        }

        public bool TryRemoveBindingOverride(SettingsBindingTarget target)
        {
            return TrySetBindingOverride(target, string.Empty, true);
        }

        public bool TryGetBindingAction(
            SettingsBindingTarget target,
            out InputAction action,
            out int bindingIndex)
        {
            return TryFindBinding(target, out action, out bindingIndex);
        }

        public bool TryGetPlayerAction(string actionName, out InputAction action)
        {
            action = null;

            if (_inputActions == null || string.IsNullOrEmpty(actionName))
            {
                return false;
            }

            action = _inputActions.asset.FindAction(
                $"Player/{actionName}",
                false);
            return action != null;
        }

        private void RegisterCallbacks()
        {
            _inputActions.Player.Jump.performed += OnJumpPerformed;
            _inputActions.Player.MomentumLanding.performed += OnMomentumLandingPerformed;
        }

        private bool TrySetBindingOverride(
            SettingsBindingTarget target,
            string controlPath,
            bool shouldRemove)
        {
            if (_inputActions == null ||
                target.ActionMap != E_SettingsActionMap.Player ||
                (!shouldRemove && string.IsNullOrEmpty(controlPath)))
            {
                return false;
            }

            if (!TryFindBinding(target, out InputAction action, out int bindingIndex))
            {
                return false;
            }

            if (shouldRemove) action.RemoveBindingOverride(bindingIndex);
            else action.ApplyBindingOverride(bindingIndex, controlPath);
            return true;
        }

        private bool TryFindBinding(SettingsBindingTarget target, out InputAction action, out int bindingIndex)
        {
            action = null;
            bindingIndex = -1;
            if (_inputActions == null || target.ActionMap != E_SettingsActionMap.Player) return false;
            action = _inputActions.asset.FindAction(target.ActionId.ToString(), false);
            if (action == null || action.actionMap.name != "Player") return false;
            for (int i = 0; i < action.bindings.Count; i++)
                if (action.bindings[i].id == target.BindingId) { bindingIndex = i; return true; }
            action = null;
            return false;
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
