namespace FlowState.Runtime.Core
{
    // Deprecated Stage-only identity. Preserve existing values without creating new
    // Stage play paths; record access/submission guards belong to Roadmap 9 Phase 3.
    public static class StageRecordIdentity
    {
        public const string CurrentStageId = "stage-001";
        public const int CurrentRulesVersion = 1;
    }
}
