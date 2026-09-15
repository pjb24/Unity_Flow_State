namespace FlowState.Runtime.Core
{
    public enum E_StageResultType
    {
        None = 0,
        Cleared = 1,
        Fell = 2
    }

    public class ResultData
    {
        public ResultData(
            E_StageResultType stageResultType,
            double elapsedTime,
            int collectibleScore)
        {
            GameMode = E_GameMode.Stage;
            HasStageResult = true;
            HasInfiniteModeResult = false;
            StageResultType = stageResultType;
            ElapsedTime = elapsedTime;
            FinalDistance = 0.0f;
            DistanceScore = 0;
            CollectibleScore = collectibleScore;
            TotalScore = 0;
            ScoringVersion = global::FlowState.Runtime.Core.ScoringVersion.None;
            BaseDistanceScore = 0;
            MomentumBonus = 0;
            MaximumMomentumMultiplier = 1.0;
        }

        public ResultData(
            float finalDistance,
            int distanceScore,
            int collectibleScore,
            int totalScore)
        {
            GameMode = E_GameMode.Infinite;
            HasStageResult = false;
            HasInfiniteModeResult = true;
            StageResultType = E_StageResultType.None;
            ElapsedTime = 0.0;
            FinalDistance = finalDistance;
            DistanceScore = distanceScore;
            CollectibleScore = collectibleScore;
            TotalScore = totalScore;
            ScoringVersion = global::FlowState.Runtime.Core.ScoringVersion.LegacyDistanceScore;
            BaseDistanceScore = distanceScore;
            MomentumBonus = 0;
            MaximumMomentumMultiplier = 1.0;
        }

        public ResultData(
            int scoringVersion,
            float finalDistance,
            int baseDistanceScore,
            int momentumBonus,
            int distanceScore,
            int collectibleScore,
            int totalScore,
            double maximumMomentumMultiplier)
        {
            GameMode = E_GameMode.Infinite;
            HasStageResult = false;
            HasInfiniteModeResult = true;
            StageResultType = E_StageResultType.None;
            ElapsedTime = 0.0;
            ScoringVersion = scoringVersion;
            FinalDistance = finalDistance;
            BaseDistanceScore = baseDistanceScore;
            MomentumBonus = momentumBonus;
            DistanceScore = distanceScore;
            CollectibleScore = collectibleScore;
            TotalScore = totalScore;
            MaximumMomentumMultiplier = maximumMomentumMultiplier;
        }

        public E_GameMode GameMode { get; }

        public bool HasStageResult { get; }

        public bool HasInfiniteModeResult { get; }

        public E_StageResultType StageResultType { get; }

        public double ElapsedTime { get; }

        public float FinalDistance { get; }

        public int DistanceScore { get; }

        public int CollectibleScore { get; }

        public int TotalScore { get; }

        public int ScoringVersion { get; }

        public int BaseDistanceScore { get; }

        public int MomentumBonus { get; }

        public double MaximumMomentumMultiplier { get; }
    }
}
