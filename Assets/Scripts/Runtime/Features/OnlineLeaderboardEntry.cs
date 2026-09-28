using System;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlineLeaderboardEntry
    {
        public string playerId;
        public long score;
        public long acceptedAt;
        public int rank;
    }
}
