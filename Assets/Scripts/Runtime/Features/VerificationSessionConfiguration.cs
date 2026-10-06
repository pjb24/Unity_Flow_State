using System.IO;

namespace FlowState.Runtime.Features
{
    public enum E_VerificationSession { A, B, Legacy }

    public sealed class VerificationSessionConfiguration
    {
        public const string EditorIsolationKey = "FlowState.Phase2.EditorIsolation";
        public static bool IsEditorIsolationArmed
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.SessionState.GetBool(EditorIsolationKey, false);
#else
                return false;
#endif
            }
        }
        public E_VerificationSession Session { get; }
        public string Profile { get; }
        public string DirectoryPath { get; }
        public string SavePath => Path.Combine(DirectoryPath, "flow-state-save.json");
        public VerificationSessionConfiguration(string persistentPath, E_VerificationSession session)
        {
            if (session != E_VerificationSession.A && session != E_VerificationSession.B && session != E_VerificationSession.Legacy)
                throw new System.ArgumentOutOfRangeException(nameof(session));
            Session = session;
            Profile = session == E_VerificationSession.Legacy ? "flow-state-verification" :
                session == E_VerificationSession.A ? "flow-state-phase2-a" : "flow-state-phase2-b";
            DirectoryPath = Path.Combine(persistentPath, "Prototype8Verification", session.ToString());
        }

        public static bool CanExecuteRemote(bool explicitConsent, bool isAutomatedTest, bool isBatchMode,
            bool isPlaying, string projectId, bool isIsolated = false)
        {
            return explicitConsent && !isAutomatedTest && !isBatchMode && isPlaying && isIsolated &&
                projectId == OnlineRecordConfiguration.ProjectId;
        }

        public static VerificationSessionConfiguration FromArguments(string persistentPath, string[] arguments)
        {
            VerificationSessionConfiguration selected = null;
            if (arguments == null) return null;
            foreach (string argument in arguments)
            {
                const string prefix = "--fs-verification-session=";
                if (argument == null || !argument.StartsWith(prefix, System.StringComparison.Ordinal)) continue;
                if (selected != null) throw new System.InvalidOperationException("Duplicate verification session.");
                string value = argument.Substring(prefix.Length);
                if (value != "A" && value != "B" && value != "Legacy") throw new System.InvalidOperationException("Invalid verification session.");
                E_VerificationSession session = value == "Legacy" ? E_VerificationSession.Legacy :
                    value == "A" ? E_VerificationSession.A : E_VerificationSession.B;
                selected = new VerificationSessionConfiguration(persistentPath, session);
            }
            return selected;
        }

    }
}
