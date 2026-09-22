using System;

namespace FlowState.Runtime.Core
{
    public readonly struct SettingsBindingDefinition
    {
        public SettingsBindingTarget Target { get; }

        public string DefaultControlPath { get; }

        public SettingsBindingDefinition(
            SettingsBindingTarget target,
            string defaultControlPath)
        {
            if (string.IsNullOrEmpty(defaultControlPath))
            {
                throw new ArgumentException(
                    "Default control path is required.",
                    nameof(defaultControlPath));
            }

            Target = target;
            DefaultControlPath = defaultControlPath;
        }
    }
}
