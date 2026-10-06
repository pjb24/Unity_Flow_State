using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FlowState.Server
{
    public sealed class ModuleSetup : ICloudCodeSetup
    {
        public void Setup(ICloudCodeConfig config)
        {
            config.Dependencies.AddSingleton(GameApiClient.Create());
        }
    }
}
