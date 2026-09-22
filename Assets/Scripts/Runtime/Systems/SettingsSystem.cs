using System;
using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlowState.Runtime.Systems
{
    public class SettingsSystem : MonoBehaviour, ISettingsApplication
    {
        [SerializeField] private PlayerInputSystem _playerInputSystem;
        [SerializeField] private UIInputSystem _uiInputSystem;

        private SettingsState _state;
        private SettingsBindingDefinition[] _definitions;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;
        private bool _isRestoreConfirmationOpen;
        private InputAction _rebindAction;
        private bool _wasRebindActionEnabled;

        public SettingsState State => _state;
        public bool IsRebinding => _rebindOperation != null;
        public bool IsRestoreConfirmationOpen => _isRestoreConfirmationOpen;
        public event Action<SettingsBindingTarget> RebindStarted;
        public event Action<SettingsBindingTarget, bool, string> RebindFinished;
        public event Action RestoreConfirmationCancelRequested;

        private void Awake()
        {
            Initialize();
        }

        private void OnDisable()
        {
            if (_rebindOperation != null)
            {
                _rebindOperation.Cancel();
            }
        }


        public void Initialize()
        {
            if (_state != null)
            {
                return;
            }

            _definitions = CreateDefinitions();
            _state = new SettingsState(Screen.fullScreen, this, _definitions);
        }

        public bool TrySetMasterVolume(int volumePercent)
        {
            return _state != null && _state.TrySetMasterVolume(volumePercent);
        }

        public bool TrySetFullscreen(bool isFullscreen)
        {
            return _state != null && _state.TrySetFullscreen(isFullscreen);
        }

        public bool TryBeginRebind(SettingsBindingTarget target)
        {
            return _state != null && _state.TryBeginRebind(target);
        }

        public bool TryCompleteRebind(string controlPath)
        {
            if (_state == null || !_state.HasPendingRebind)
            {
                return false;
            }

            SettingsBindingTarget target = _state.PendingTarget;

            if (!_state.CanCompleteRebind(controlPath))
            {
                return false;
            }

            if (!ApplyBindingOverride(target, controlPath))
            {
                return false;
            }

            return _state.TryCompleteRebind(controlPath);
        }

        public bool TryCancelRebind()
        {
            if (_rebindOperation != null)
            {
                _rebindOperation.Cancel();
                return true;
            }
            return _state != null && _state.TryCancelRebind();
        }

        public bool ConsumeRebindCancelSuppression()
        {
            return _state != null && _state.TryConsumeCancelledRebind();
        }

        public void SetRestoreConfirmationOpen(bool isOpen)
        {
            _isRestoreConfirmationOpen = isOpen;
        }

        public bool TryCancelRestoreConfirmation()
        {
            if (!_isRestoreConfirmationOpen) return false;
            RestoreConfirmationCancelRequested?.Invoke();
            return true;
        }

        // Unity Button.onClick exposes public void methods in the Inspector.
        public void BeginJumpRebind() => TryBeginInteractiveRebind(0);
        public void BeginMomentumLandingRebind() => TryBeginInteractiveRebind(1);
        public void BeginNavigateUpRebind() => TryBeginInteractiveRebind(2);
        public void BeginNavigateDownRebind() => TryBeginInteractiveRebind(3);
        public void BeginNavigateLeftRebind() => TryBeginInteractiveRebind(4);
        public void BeginNavigateRightRebind() => TryBeginInteractiveRebind(5);

        private bool TryBeginInteractiveRebind(int definitionIndex)
        {
            if (_definitions == null || definitionIndex < 0 || definitionIndex >= _definitions.Length)
                return false;

            SettingsBindingTarget target = _definitions[definitionIndex].Target;
            if (_rebindOperation != null)
            {
                RebindFinished?.Invoke(target, false, "Another rebind is already waiting for input.");
                return false;
            }

            if (!TryBeginRebind(target) || !TryGetBindingAction(target, out InputAction action, out int bindingIndex))
            {
                _state?.TryCancelRebind();
                _state?.TryConsumeCancelledRebind();
                RebindFinished?.Invoke(target, false, "Unable to start rebind. The input action is unavailable.");
                return false;
            }

            _rebindAction = action;
            _wasRebindActionEnabled = action.enabled;
            try
            {
                if (_wasRebindActionEnabled) action.Disable();
                _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsHavingToMatchPath("<Keyboard>/*")
                    .WithCancelingThrough("<Keyboard>/escape")
                    .OnCancel(operation => FinishInteractiveRebind(target, false, string.Empty))
                    .OnComplete(operation => FinishInteractiveRebind(
                        target,
                        true,
                        GetCanonicalBindingPath(operation.selectedControl)));
                _rebindOperation.Start();
            }
            catch (InvalidOperationException exception)
            {
                _rebindOperation?.Dispose();
                _rebindOperation = null;
                if (_rebindAction != null && _wasRebindActionEnabled) _rebindAction.Enable();
                _rebindAction = null;
                _wasRebindActionEnabled = false;
                _state?.TryCancelRebind();
                _state?.TryConsumeCancelledRebind();
                RebindFinished?.Invoke(target, false, exception.Message);
                return false;
            }

            RebindStarted?.Invoke(target);
            return true;
        }

        public bool TryGetBindingDisplayPath(int definitionIndex, out string controlPath)
        {
            controlPath = string.Empty;
            return _state != null && definitionIndex >= 0 &&
                   definitionIndex < _definitions.Length &&
                   _state.TryGetEffectiveControlPath(_definitions[definitionIndex].Target, out controlPath);
        }

        public bool TryGetBindingTarget(int definitionIndex, out SettingsBindingTarget target)
        {
            target = default;
            if (_definitions == null || definitionIndex < 0 || definitionIndex >= _definitions.Length) return false;
            target = _definitions[definitionIndex].Target;
            return true;
        }

        private bool TryGetBindingAction(SettingsBindingTarget target, out InputAction action, out int bindingIndex)
        {
            action = null;
            bindingIndex = -1;
            if (target.ActionMap == E_SettingsActionMap.Player)
            {
                return _playerInputSystem != null &&
                       _playerInputSystem.TryGetBindingAction(target, out action, out bindingIndex);
            }

            return _uiInputSystem != null &&
                   _uiInputSystem.TryGetBindingAction(target, out action, out bindingIndex);
        }

        private void FinishInteractiveRebind(SettingsBindingTarget target, bool completed, string controlPath)
        {
            _rebindOperation.Dispose();
            _rebindOperation = null;
            if (_rebindAction != null && _wasRebindActionEnabled) _rebindAction.Enable();
            _rebindAction = null;
            _wasRebindActionEnabled = false;
            bool didApply = completed && TryCompleteRebind(controlPath);
            if (!didApply)
            {
                _state.TryCancelRebind();
            }
            RebindFinished?.Invoke(target, didApply, didApply ? controlPath : "Rebind cancelled or conflicts with an existing binding.");
        }

        private static string GetCanonicalBindingPath(InputControl control)
        {
            if (control == null || control.device == null ||
                string.IsNullOrEmpty(control.path) ||
                string.IsNullOrEmpty(control.device.path) ||
                string.IsNullOrEmpty(control.device.layout))
            {
                return string.Empty;
            }

            string devicePath = control.device.path;
            if (!control.path.StartsWith(devicePath, StringComparison.OrdinalIgnoreCase))
            {
                return control.path;
            }

            string relativeControlPath = control.path.Substring(devicePath.Length).TrimStart('/');
            return string.IsNullOrEmpty(relativeControlPath)
                ? string.Empty
                : $"<{control.device.layout}>/{relativeControlPath}";
        }

        public bool TryRestoreDefaults()
        {
            if (_state == null || !_state.TryRestoreDefaults())
            {
                return false;
            }

            for (int i = 0; i < _definitions.Length; i++)
            {
                RemoveBindingOverride(_definitions[i].Target);
            }

            return true;
        }

        public void ApplyMasterVolume(int volumePercent)
        {
            AudioListener.volume = volumePercent / 100.0f;
        }

        public void ApplyFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        private bool ApplyBindingOverride(
            SettingsBindingTarget target,
            string controlPath)
        {
            return target.ActionMap == E_SettingsActionMap.Player
                ? _playerInputSystem != null &&
                  _playerInputSystem.TryApplyBindingOverride(target, controlPath)
                : _uiInputSystem != null &&
                  _uiInputSystem.TryApplyBindingOverride(target, controlPath);
        }

        private void RemoveBindingOverride(SettingsBindingTarget target)
        {
            if (target.ActionMap == E_SettingsActionMap.Player)
            {
                if (_playerInputSystem != null)
                {
                    _playerInputSystem.TryRemoveBindingOverride(target);
                }

                return;
            }

            if (_uiInputSystem != null)
            {
                _uiInputSystem.TryRemoveBindingOverride(target);
            }
        }

        private static SettingsBindingDefinition[] CreateDefinitions()
        {
            return new[]
            {
                CreateDefinition(
                    E_SettingsActionMap.Player,
                    "f1ba0d36-48eb-4cd5-b651-1c94a6531f70",
                    "eb40bb66-4559-4dfa-9a2f-820438abb426",
                    string.Empty,
                    "<Keyboard>/space"),
                CreateDefinition(
                    E_SettingsActionMap.Player,
                    "19afd3be-8bb6-4a1d-ac02-f7dbd804ffe5",
                    "fcb81b12-bda8-4c0f-ab0d-17db18df3193",
                    string.Empty,
                    "<Keyboard>/leftShift"),
                CreateDefinition(
                    E_SettingsActionMap.UI,
                    "c95b2375-e6d9-4b88-9c4c-c5e76515df4b",
                    "563fbfdd-0f09-408d-aa75-8642c4f08ef0",
                    "up",
                    "<Keyboard>/w"),
                CreateDefinition(
                    E_SettingsActionMap.UI,
                    "c95b2375-e6d9-4b88-9c4c-c5e76515df4b",
                    "2bf42165-60bc-42ca-8072-8c13ab40239b",
                    "down",
                    "<Keyboard>/s"),
                CreateDefinition(
                    E_SettingsActionMap.UI,
                    "c95b2375-e6d9-4b88-9c4c-c5e76515df4b",
                    "74214943-c580-44e4-98eb-ad7eebe17902",
                    "left",
                    "<Keyboard>/a"),
                CreateDefinition(
                    E_SettingsActionMap.UI,
                    "c95b2375-e6d9-4b88-9c4c-c5e76515df4b",
                    "8607c725-d935-4808-84b1-8354e29bab63",
                    "right",
                    "<Keyboard>/d")
            };
        }

        private static SettingsBindingDefinition CreateDefinition(
            E_SettingsActionMap actionMap,
            string actionId,
            string bindingId,
            string compositePartName,
            string defaultControlPath)
        {
            return new SettingsBindingDefinition(
                new SettingsBindingTarget(
                    actionMap,
                    E_SettingsDeviceGroup.KeyboardAndMouse,
                    new Guid(actionId),
                    new Guid(bindingId),
                    compositePartName),
                defaultControlPath);
        }
    }
}
