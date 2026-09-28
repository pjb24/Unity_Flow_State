using System.Threading.Tasks;
using FlowState.Runtime.Features;

namespace FlowState.Tests.EditMode
{
    internal sealed class OnlineTestAuthentication : IOnlineAuthenticationGateway
    {
        public string Player = "ugs-player";
        public bool Success = true;
        public int Calls;
        public Task<OnlineAuthenticationResult> TryAuthenticateAsync()
        {
            Calls++;
            return Task.FromResult(new OnlineAuthenticationResult(Success, Player));
        }
    }
}
