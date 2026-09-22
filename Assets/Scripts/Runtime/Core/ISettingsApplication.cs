namespace FlowState.Runtime.Core
{
    public interface ISettingsApplication
    {
        void ApplyMasterVolume(int volumePercent);

        void ApplyFullscreen(bool isFullscreen);
    }
}
