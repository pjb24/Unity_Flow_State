using System;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlinePersonalBestSnapshot
    {
        public string status;
        public string publicPlayerNumber;
        public OnlinePersonalBestEntry[] personalBests;
    }
}
