using System;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlinePersonalBestEntry
    {
        public string boardId;
        public long score;
        public string submissionId;
    }
}
