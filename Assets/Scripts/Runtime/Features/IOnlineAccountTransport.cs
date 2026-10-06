using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public interface IOnlineAccountTransport
    {
        Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters);
    }
}
