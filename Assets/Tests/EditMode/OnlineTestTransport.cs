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
        public string Reason = "InvalidScore";
        public bool Timeout;
        public System.Exception Failure;
        public OnlineLeaderboardResult QueryResponse = new OnlineLeaderboardResult
        {
            status = "Success",
            entries = new OnlineLeaderboardEntry[0]
        };
        public Task<T> CallAsync<T>(string endpoint, string request)
        {
            Calls++; Endpoint = endpoint; Request = request;
            if (Timeout) return Task.FromException<T>(new System.TimeoutException());
            if (Failure != null) return Task.FromException<T>(Failure);
            object value = endpoint == "submit-record"
                ? (object)new OnlineSubmissionResponse { status = Status, reason = Reason }
                : QueryResponse;
            return Task.FromResult((T)value);
        }
    }
}
