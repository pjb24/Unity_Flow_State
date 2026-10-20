using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public static class OnlineRecordConfiguration
    {
        public const string VerificationEnvironmentName = "verification";
        public const string ProjectId = "c76d55cf-7846-494b-9dce-a0797b179b36";
        public const string EnvironmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";

#if FLOW_STATE_PRODUCTION
        public static bool IsProductionBuild => true;
        public static string ActiveProjectId => OnlineRecordProductionConfiguration.ProjectId;
        public static string ActiveEnvironmentId => OnlineRecordProductionConfiguration.EnvironmentId;
        public static string ActiveEnvironmentName => OnlineRecordProductionConfiguration.EnvironmentName;
        public static string ActiveAuthenticationProfile => OnlineRecordProductionConfiguration.AuthenticationProfile;
#else
        public static bool IsProductionBuild => false;
        public static string ActiveProjectId => ProjectId;
        public static string ActiveEnvironmentId => EnvironmentId;
        public static string ActiveEnvironmentName => VerificationEnvironmentName;
        public static string ActiveAuthenticationProfile => "flow-state-verification";
#endif

        private const string StageLeaderboardId = "fs-stage-stage-001-r1";
        private const string InfiniteLeaderboardId = "fs-infinite-v2";

        public static bool HasValidActiveConfiguration
        {
            get
            {
                return !string.IsNullOrEmpty(ActiveProjectId) &&
                    !string.IsNullOrEmpty(ActiveEnvironmentId) &&
                    !string.IsNullOrEmpty(ActiveEnvironmentName) &&
                    !string.IsNullOrEmpty(ActiveAuthenticationProfile);
            }
        }

        public static bool IsAllowedAuthenticationProfile(string profile)
        {
            if (string.IsNullOrEmpty(profile)) return false;
#if FLOW_STATE_PRODUCTION
            return profile == ActiveAuthenticationProfile;
#else
            return profile == "flow-state-verification" || profile == "flow-state-phase2-a" ||
                profile == "flow-state-phase2-b" || profile == "flow-state-phase3-new";
#endif
        }

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
