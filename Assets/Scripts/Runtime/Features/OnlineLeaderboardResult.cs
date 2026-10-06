using System;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlineLeaderboardResult
    {
        public string status;
        public string reason;
        public string queryPhase;
        public string queryFault;
        public int serviceStatus;
        public OnlineLeaderboardEntry[] entries = new OnlineLeaderboardEntry[0];
        public bool IsSuccess => status == "Success";
    }
}
