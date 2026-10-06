using System;

namespace FlowState.Runtime.Features
{
    // User-local comparison artifact, never a log or account authorization.
    [Serializable]
    public sealed class VerificationRecordBaseline
    {
        public string projectId;
        public string environmentId;
        public string boardId;
        public string publicPlayerNumber;
        public OnlineLeaderboardResult me;
    }
}
