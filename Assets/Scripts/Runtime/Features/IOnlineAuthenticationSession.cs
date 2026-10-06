using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public interface IOnlineAuthenticationSession : IOnlineAuthenticationGateway
    {
        Task<bool> TryClearSessionAsync(string expectedPlayerId);
    }
}
