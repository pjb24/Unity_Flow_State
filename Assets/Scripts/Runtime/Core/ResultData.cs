namespace FlowState.Runtime.Core
{
    public class ResultData
    {
        public ResultData(bool isStageCleared, double clearTime)
        {
            GameMode = E_GameMode.Stage;
            HasStageResult = true;
            HasInfiniteModeResult = false;
            IsStageCleared = isStageCleared;
            ClearTime = clearTime;
            FinalDistance = 0.0f;
            FinalScore = 0;
        }

        public ResultData(float finalDistance, int finalScore)
        {
            GameMode = E_GameMode.Infinite;
            HasStageResult = false;
            HasInfiniteModeResult = true;
            IsStageCleared = false;
            ClearTime = 0.0;
            FinalDistance = finalDistance;
            FinalScore = finalScore;
        }

        public E_GameMode GameMode { get; }

        public bool HasStageResult { get; }

        public bool HasInfiniteModeResult { get; }

        public bool IsStageCleared { get; }

        public double ClearTime { get; }

        public float FinalDistance { get; }

        public int FinalScore { get; }
    }
}
