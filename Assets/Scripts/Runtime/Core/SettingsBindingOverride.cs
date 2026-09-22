using System;

namespace FlowState.Runtime.Core
{
    public readonly struct SettingsBindingOverride
    {
        public SettingsBindingTarget Target { get; }

        public string ControlPath { get; }

        public SettingsBindingOverride(
            SettingsBindingTarget target,
            string controlPath)
        {
            if (string.IsNullOrEmpty(controlPath))
            {
                throw new ArgumentException(
                    "Control path is required.",
                    nameof(controlPath));
            }

            Target = target;
            ControlPath = controlPath;
        }
    }
}
