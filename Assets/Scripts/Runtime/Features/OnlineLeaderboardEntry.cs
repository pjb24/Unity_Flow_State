using System;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlineLeaderboardEntry
    {
        public string publicPlayerNumber;
        public bool isMe;
        public long score;
        public long acceptedAt;
        public int rank;
    }
}
