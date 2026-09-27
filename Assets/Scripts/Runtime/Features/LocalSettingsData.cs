using System.Collections.Generic;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public sealed class LocalSettingsData
    {
        private readonly List<SettingsBindingOverride> _bindingOverrides;

        public int MasterVolume { get; }

        public bool IsFullscreen { get; }

        public IReadOnlyList<SettingsBindingOverride> BindingOverrides =>
            _bindingOverrides;

        public LocalSettingsData(
            int masterVolume,
            bool isFullscreen,
            IReadOnlyList<SettingsBindingOverride> bindingOverrides)
        {
            MasterVolume = masterVolume;
            IsFullscreen = isFullscreen;
            _bindingOverrides = new List<SettingsBindingOverride>();

            if (bindingOverrides == null)
            {
                return;
            }

            for (int i = 0; i < bindingOverrides.Count; i++)
            {
                _bindingOverrides.Add(bindingOverrides[i]);
            }
        }
    }
}
