using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public static class OnlineRecordConfiguration
    {
        public const string VerificationEnvironmentName = "verification";
        public const string ProjectId = "c76d55cf-7846-494b-9dce-a0797b179b36";
        public const string EnvironmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";

        private const string StageLeaderboardId = "fs-stage-stage-001-r1";
        private const string InfiniteLeaderboardId = "fs-infinite-v2";

        public static bool TryGetLeaderboardId(
            RecordBoardKey boardKey,
            out string leaderboardId)
        {
            leaderboardId = string.Empty;

            if (boardKey.GameMode == E_GameMode.Stage &&
                boardKey.StageId == "stage-001" &&
                boardKey.RulesVersion == 1)
            {
                leaderboardId = StageLeaderboardId;
                return true;
            }

            if (boardKey.GameMode == E_GameMode.Infinite &&
                boardKey.StageId == string.Empty &&
                boardKey.RulesVersion == 2)
            {
                leaderboardId = InfiniteLeaderboardId;
                return true;
            }

            return false;
        }
    }
}
