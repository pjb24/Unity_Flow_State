using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public interface IOnlineRecordTransport
    {
        Task<T> CallAsync<T>(string endpoint, string request);
    }
}
