using FlowState.Runtime.Core;

namespace FlowState.Tests.EditMode
{
    public sealed class SettingsApplicationSpy : ISettingsApplication
    {
        public int MasterVolumeApplyCount { get; private set; }

        public int FullscreenApplyCount { get; private set; }

        public int LastMasterVolume { get; private set; }

        public bool LastFullscreen { get; private set; }

        public void ApplyMasterVolume(int volumePercent)
        {
            MasterVolumeApplyCount++;
            LastMasterVolume = volumePercent;
        }

        public void ApplyFullscreen(bool isFullscreen)
        {
            FullscreenApplyCount++;
            LastFullscreen = isFullscreen;
        }
    }
}
