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
    }
}
