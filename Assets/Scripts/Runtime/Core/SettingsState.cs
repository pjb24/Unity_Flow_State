using System;
using System.Collections.Generic;

namespace FlowState.Runtime.Core
{
    public sealed class SettingsState
    {
        public const int DefaultMasterVolume = 100;
        public const int MinimumMasterVolume = 0;
        public const int MaximumMasterVolume = 100;

        private readonly ISettingsApplication _application;
        private readonly SettingsBindingDefinition[] _definitions;
        private readonly List<SettingsBindingOverride> _overrides =
            new List<SettingsBindingOverride>();
        private readonly bool _defaultFullscreen;
        private SettingsBindingTarget _pendingTarget;

        public int MasterVolume { get; private set; } = DefaultMasterVolume;

        public bool IsFullscreen { get; private set; }

        public E_SettingsRebindingState RebindingState { get; private set; } =
            E_SettingsRebindingState.None;

        public bool HasPendingRebind =>
            RebindingState == E_SettingsRebindingState.WaitingForInput;

        public SettingsBindingTarget PendingTarget => _pendingTarget;

        public int BindingOverrideCount => _overrides.Count;

        public SettingsState(
            bool defaultFullscreen,
            ISettingsApplication application,
            SettingsBindingDefinition[] definitions)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            _application = application;
            _defaultFullscreen = defaultFullscreen;
            _definitions = new SettingsBindingDefinition[definitions.Length];

            for (int i = 0; i < definitions.Length; i++)
            {
                if (FindDefinitionIndex(definitions[i].Target) >= 0)
                {
                    throw new ArgumentException(
                        "Binding targets must be unique.",
                        nameof(definitions));
                }

                _definitions[i] = definitions[i];
            }

            IsFullscreen = defaultFullscreen;
        }

        public bool TrySetMasterVolume(int volumePercent)
        {
            int clampedVolume = ClampMasterVolume(volumePercent);

            if (MasterVolume == clampedVolume)
            {
                return false;
            }

            MasterVolume = clampedVolume;
            _application.ApplyMasterVolume(MasterVolume);
            return true;
        }

        public bool TrySetFullscreen(bool isFullscreen)
        {
            if (IsFullscreen == isFullscreen)
            {
                return false;
            }

            IsFullscreen = isFullscreen;
            _application.ApplyFullscreen(IsFullscreen);
            return true;
        }

        public bool TryBeginRebind(SettingsBindingTarget target)
        {
            if (RebindingState != E_SettingsRebindingState.None || FindDefinitionIndex(target) < 0)
            {
                return false;
            }

            _pendingTarget = target;
            RebindingState = E_SettingsRebindingState.WaitingForInput;
            return true;
        }

        public bool TryCompleteRebind(string controlPath)
        {
            if (!CanCompleteRebind(controlPath))
            {
                return false;
            }

            SetOverride(_pendingTarget, controlPath);
            ClearPendingRebind();
            return true;
        }

        public bool CanCompleteRebind(string controlPath)
        {
            return HasPendingRebind &&
                   !string.IsNullOrEmpty(controlPath) &&
                   !HasConflict(_pendingTarget, controlPath);
        }

        public bool TryCancelRebind()
        {
            if (!HasPendingRebind)
            {
                return false;
            }

            _pendingTarget = default;
            RebindingState = E_SettingsRebindingState.CancelledAwaitingCancelConsumption;
            return true;
        }

        public bool TryConsumeCancelledRebind()
        {
            if (RebindingState != E_SettingsRebindingState.CancelledAwaitingCancelConsumption) return false;
            RebindingState = E_SettingsRebindingState.None;
            return true;
        }

        public bool TryRestoreDefaults()
        {
            bool didChange = MasterVolume != DefaultMasterVolume ||
                             IsFullscreen != _defaultFullscreen ||
                             _overrides.Count > 0 ||
                             HasPendingRebind;

            if (!didChange)
            {
                return false;
            }

            bool shouldApplyVolume = MasterVolume != DefaultMasterVolume;
            bool shouldApplyFullscreen = IsFullscreen != _defaultFullscreen;
            MasterVolume = DefaultMasterVolume;
            IsFullscreen = _defaultFullscreen;
            _overrides.Clear();
            ClearPendingRebind();

            if (shouldApplyVolume)
            {
                _application.ApplyMasterVolume(MasterVolume);
            }

            if (shouldApplyFullscreen)
            {
                _application.ApplyFullscreen(IsFullscreen);
            }

            return true;
        }

        public bool TryGetEffectiveControlPath(
            SettingsBindingTarget target,
            out string controlPath)
        {
            int definitionIndex = FindDefinitionIndex(target);

            if (definitionIndex < 0)
            {
                controlPath = string.Empty;
                return false;
            }

            int overrideIndex = FindOverrideIndex(target);
            controlPath = overrideIndex >= 0
                ? _overrides[overrideIndex].ControlPath
                : _definitions[definitionIndex].DefaultControlPath;
            return true;
        }

        private static int ClampMasterVolume(int volumePercent)
        {
            if (volumePercent < MinimumMasterVolume)
            {
                return MinimumMasterVolume;
            }

            if (volumePercent > MaximumMasterVolume)
            {
                return MaximumMasterVolume;
            }

            return volumePercent;
        }

        private bool HasConflict(
            SettingsBindingTarget target,
            string controlPath)
        {
            for (int i = 0; i < _definitions.Length; i++)
            {
                SettingsBindingTarget candidate = _definitions[i].Target;

                if (candidate.Equals(target) ||
                    candidate.ActionMap != target.ActionMap ||
                    candidate.DeviceGroup != target.DeviceGroup)
                {
                    continue;
                }

                if (string.Equals(
                        GetEffectiveControlPath(candidate),
                        controlPath,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetEffectiveControlPath(SettingsBindingTarget target)
        {
            int overrideIndex = FindOverrideIndex(target);

            if (overrideIndex >= 0)
            {
                return _overrides[overrideIndex].ControlPath;
            }

            return _definitions[FindDefinitionIndex(target)].DefaultControlPath;
        }

        private void SetOverride(SettingsBindingTarget target, string controlPath)
        {
            int overrideIndex = FindOverrideIndex(target);

            if (overrideIndex >= 0)
            {
                _overrides[overrideIndex] = new SettingsBindingOverride(
                    target,
                    controlPath);
                return;
            }

            _overrides.Add(new SettingsBindingOverride(target, controlPath));
        }

        private void ClearPendingRebind()
        {
            _pendingTarget = default;
            RebindingState = E_SettingsRebindingState.None;
        }

        private int FindDefinitionIndex(SettingsBindingTarget target)
        {
            for (int i = 0; i < _definitions.Length; i++)
            {
                if (_definitions[i].Target.Equals(target))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindOverrideIndex(SettingsBindingTarget target)
        {
            for (int i = 0; i < _overrides.Count; i++)
            {
                if (_overrides[i].Target.Equals(target))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
