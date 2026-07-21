namespace FlowState.Runtime.Core
{
    public class ResultData
    {
        public ResultData(bool isStageCleared, double clearTime)
        {
            IsStageCleared = isStageCleared;
            ClearTime = clearTime;
        }

        public bool IsStageCleared { get; }

        public double ClearTime { get; }
    }
}
