using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public sealed class RecordSubmissionCandidate
    {
        public string PlayerId { get; }

        public string SubmissionId { get; }

        public RecordBoardKey BoardKey { get; }

        public E_GameMode GameMode { get; }

        public long RankingValue { get; }

        public long RunDurationMilliseconds { get; }

        public int BaseDistanceScore { get; }

        public int MomentumBonus { get; }

        public int DistanceScore { get; }

        public int CollectibleScore { get; }

        public int TotalScore { get; }

        public double MaximumMomentumMultiplier { get; }

        internal RecordSubmissionCandidate(
            string playerId,
            string submissionId,
            RecordBoardKey boardKey,
            long rankingValue,
            long runDurationMilliseconds,
            int baseDistanceScore,
            int momentumBonus,
            int distanceScore,
            int collectibleScore,
            int totalScore,
            double maximumMomentumMultiplier)
        {
            PlayerId = playerId;
            SubmissionId = submissionId;
            BoardKey = boardKey;
            GameMode = boardKey.GameMode;
            RankingValue = rankingValue;
            RunDurationMilliseconds = runDurationMilliseconds;
            BaseDistanceScore = baseDistanceScore;
            MomentumBonus = momentumBonus;
            DistanceScore = distanceScore;
            CollectibleScore = collectibleScore;
            TotalScore = totalScore;
            MaximumMomentumMultiplier = maximumMomentumMultiplier;
        }
    }
}
