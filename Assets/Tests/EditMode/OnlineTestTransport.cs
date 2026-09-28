using System.Threading.Tasks;
using FlowState.Runtime.Features;

namespace FlowState.Tests.EditMode
{
    internal sealed class OnlineTestTransport : IOnlineRecordTransport
    {
        public int Calls;
        public string Endpoint;
        public string Request;
        public string Status = "Submitted";
        public bool Timeout;
        public Task<T> CallAsync<T>(string endpoint, string request)
        {
            Calls++; Endpoint = endpoint; Request = request;
            if (Timeout) return Task.FromException<T>(new System.TimeoutException());
            object value = endpoint == "submit-record"
                ? (object)new OnlineSubmissionResponse { status = Status }
                : new OnlineLeaderboardResult { status = "Success" };
            return Task.FromResult((T)value);
        }
    }
}
