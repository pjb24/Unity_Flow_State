using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public interface IOnlineAuthenticationGateway
    {
        Task<OnlineAuthenticationResult> TryAuthenticateAsync();
    }
}
