using System;
using FlowState.Input;
using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace FlowState.Runtime.Systems
{
    public class UIInputSystem : MonoBehaviour
    {
        [SerializeField] private InputSystemUIInputModule _inputModule;

        private InputActionAsset _inputActionsAsset;
        private InputActionMap _uiActionMap;
        private Vector2 _navigateInput;
        private Vector2 _pointerPosition;
        private bool _isPointChanged;
        private bool _isSubmitPressed;
        private bool _isCancelPressed;
        private bool _isClickPressed;
        private bool _isInitialized;

        public bool IsUIActionMapEnabled =>
            _uiActionMap != null && _uiActionMap.enabled;

        private void OnDestroy()
        {
            if (!_isInitialized)
            {
                return;
            }

            UnregisterCallbacks();
            _uiActionMap = null;
            _inputActionsAsset = null;
            ResetInputState();
            _isInitialized = false;
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            if (_inputModule == null)
                _inputModule = UnityEngine.Object.FindFirstObjectByType<InputSystemUIInputModule>();

            if (_inputModule == null || _inputModule.actionsAsset == null)
            {
                Debug.LogError("UIInputSystem requires an InputSystemUIInputModule with an actions asset.");
                return;
            }

            _inputActionsAsset = _inputModule.actionsAsset;
            _uiActionMap = _inputActionsAsset.FindActionMap("UI", false);
            if (_uiActionMap == null)
            {
                Debug.LogError("UIInputSystem could not find the UI action map on the InputSystemUIInputModule actions asset.");
                _inputActionsAsset = null;
                return;
            }

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
            if (_isInitialized)
                _uiActionMap.Enable();
        }

        public void DisableUIActionMap()
        {
            if (!_isInitialized)
            {
                return;
            }

            _uiActionMap.Disable();
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

        private void RegisterCallbacks()
        {
            _uiActionMap.FindAction("Navigate", true).performed += OnNavigatePerformed;
            _uiActionMap.FindAction("Navigate", true).canceled += OnNavigateCanceled;
            _uiActionMap.FindAction("Submit", true).performed += OnSubmitPerformed;
            _uiActionMap.FindAction("Cancel", true).performed += OnCancelPerformed;
            _uiActionMap.FindAction("Point", true).performed += OnPointPerformed;
            _uiActionMap.FindAction("Point", true).canceled += OnPointCanceled;
            _uiActionMap.FindAction("Click", true).performed += OnClickPerformed;
        }

        private bool TrySetBindingOverride(
            SettingsBindingTarget target,
            string controlPath,
            bool shouldRemove)
        {
            if (_inputActionsAsset == null ||
                target.ActionMap != E_SettingsActionMap.UI ||
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
            if (_inputActionsAsset == null || target.ActionMap != E_SettingsActionMap.UI) return false;
            action = _inputActionsAsset.FindAction(target.ActionId.ToString(), false);
            if (action == null || action.actionMap.name != "UI") return false;
            for (int i = 0; i < action.bindings.Count; i++)
                if (action.bindings[i].id == target.BindingId) { bindingIndex = i; return true; }
            action = null;
            return false;
        }

        private void UnregisterCallbacks()
        {
            if (_uiActionMap == null)
                return;

            _uiActionMap.FindAction("Navigate", true).performed -= OnNavigatePerformed;
            _uiActionMap.FindAction("Navigate", true).canceled -= OnNavigateCanceled;
            _uiActionMap.FindAction("Submit", true).performed -= OnSubmitPerformed;
            _uiActionMap.FindAction("Cancel", true).performed -= OnCancelPerformed;
            _uiActionMap.FindAction("Point", true).performed -= OnPointPerformed;
            _uiActionMap.FindAction("Point", true).canceled -= OnPointCanceled;
            _uiActionMap.FindAction("Click", true).performed -= OnClickPerformed;
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
