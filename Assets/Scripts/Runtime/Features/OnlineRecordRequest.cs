using System;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlineRecordRequest
    {
        public string boardId;
        public int rulesVersion;
        public string submissionId;
        public long score;
        public long runDurationMilliseconds;
        public int baseDistanceScore;
        public int momentumBonus;
        public int distanceScore;
        public int collectibleScore;
        public int totalScore;
        public double maximumMomentumMultiplier;
        public string kind;
        public int limit;
    }
}
