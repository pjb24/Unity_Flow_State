namespace FlowState.Runtime.Features
{
    public sealed class OnlineDataScope
    {
        public string ProjectId { get; }
        public string EnvironmentId { get; }
        public bool IsValid => !string.IsNullOrEmpty(ProjectId) && !string.IsNullOrEmpty(EnvironmentId);

        public OnlineDataScope(string projectId, string environmentId)
        {
            ProjectId = projectId;
            EnvironmentId = environmentId;
        }

        public bool Matches(OnlineDataScope other)
        {
            return IsValid && other != null && other.IsValid &&
                ProjectId == other.ProjectId && EnvironmentId == other.EnvironmentId;
        }

        public static OnlineDataScope CreateVerification()
        {
            // Historical files belong to verification even in a future build
            // whose explicit environment configuration is different.
            return new OnlineDataScope("c76d55cf-7846-494b-9dce-a0797b179b36",
                "a20a46fa-1edb-4d79-9c35-02f2fed31896");
        }

        public static OnlineDataScope CreateConfigured()
        {
            return new OnlineDataScope(OnlineRecordConfiguration.ProjectId, OnlineRecordConfiguration.EnvironmentId);
        }
    }
}
