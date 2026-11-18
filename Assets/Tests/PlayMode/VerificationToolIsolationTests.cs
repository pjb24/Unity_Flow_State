#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public sealed class VerificationToolIsolationTests
    {
        [TestCase("Query", -1)]
        [TestCase("VerifyServerValidation", -1)]
        [TestCase("VerifyDirectWriteDenied", -1)]
        [TestCase("VerifyAccountMismatch", -1)]
        [TestCase("ProbePhase2", 0)]
        [TestCase("ProbePhase2", 1)]
        [TestCase("ProbePhase2", 2)]
        [TestCase("ProbePhase2", 3)]
        [TestCase("ProbePhase2", 5)]
        [TestCase("ProbePhase2", 6)]
        [TestCase("ProbePhase2", 7)]
        [TestCase("ProbePhase2", -1)]
        [TestCase("ProbePhase2", 9)]
        [TestCase("ProbePhase2DirectWrites", -1)]
        public void RetiredEditorEntry_KeepsStateAndMakesZeroBoundaryCalls(string methodName, int probe)
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            string key = VerificationSessionConfiguration.EditorIsolationKey;
            bool armed = UnityEditor.SessionState.GetBool(key, false);
            ToolFile file = new ToolFile(); LocalRecordRepository local = new LocalRecordRepository(file);
            Assert.That(local.TryLoad(out LocalSaveData ignored), Is.True);
            Assert.That(local.TrySave(local.CreateSaveData("tool-owner", new LocalSettingsData(100, false, null), false)), Is.True);
            Assert.That(local.TrySaveOnlineAccount(new OnlineAccountState(true, "fake-player")), Is.True);
            ToolBoundary fake = new ToolBoundary(); int deadlines = 0;
            VerificationRemoteGuard guard = new VerificationRemoteGuard(() => true, fake, fake, fake,
                milliseconds => { deadlines++; return new TaskCompletionSource<bool>().Task; });
            int writes = file.Writes; string contents = file.Contents;
            ScriptableObject window = ScriptableObject.CreateInstance(type);
            try
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                bool infinite = methodName != "ProbePhase2" || (probe != 0 && probe != 1 && probe != 7);
                type.GetField("_infinite", flags).SetValue(window, infinite);
                type.GetField("_confirmed", flags).SetValue(window, true);
                type.GetField("_remoteConsent", flags).SetValue(window, true);
                type.GetField("_sessionGeneration", flags).SetValue(window, 42);
                type.GetField("_summary", flags).SetValue(window, "seed-summary");
                type.GetField("_baselineInput", flags).SetValue(window, "seed-input");
                type.GetField("_baselineExport", flags).SetValue(window, "seed-export");
                type.GetField("_isolatedLocal", flags).SetValue(window, local);
                type.GetField("_guard", flags).SetValue(window, guard);
                type.GetField("_account", flags).SetValue(window, new OnlineAccountCoordinator(local, fake, fake, () => true));
                Assert.That(type.GetMethod("IsStageRecordToolRetired", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null), Is.EqualTo(true));
                MethodInfo method = type.GetMethod(methodName, flags);
                Assert.That(method, Is.Not.Null);
                object[] arguments = new object[] { null };
                if (methodName == "ProbePhase2") arguments = new object[] { probe };
                else if (methodName == "Query") arguments = new object[] { null, false };
                else if (methodName == "ProbePhase2DirectWrites") arguments = null;
                object result = method.Invoke(window, arguments);
                if (result is Task) { Assert.That(((Task)result).IsCompleted, Is.True); ((Task)result).GetAwaiter().GetResult(); }
                Assert.That(type.GetField("_summary", flags).GetValue(window), Is.EqualTo("seed-summary"));
                Assert.That(type.GetField("_baselineInput", flags).GetValue(window), Is.EqualTo("seed-input"));
                Assert.That(type.GetField("_baselineExport", flags).GetValue(window), Is.EqualTo("seed-export"));
                Assert.That(type.GetField("_busy", flags).GetValue(window), Is.EqualTo(false));
                Assert.That(type.GetField("_infinite", flags).GetValue(window), Is.EqualTo(infinite));
                Assert.That(type.GetField("_sessionGeneration", flags).GetValue(window), Is.EqualTo(42));
                Assert.That(type.GetField("_configuration", flags).GetValue(window), Is.Null);
                Assert.That(type.GetField("_authenticationGateway", flags).GetValue(window), Is.Null);
                Assert.That(type.GetField("_moduleTransport", flags).GetValue(window), Is.Null);
                Assert.That(guard.OperationDeadline, Is.Null);
                Assert.That(fake.Calls, Is.Zero); Assert.That(deadlines, Is.Zero);
                Assert.That(file.Writes, Is.EqualTo(writes)); Assert.That(file.Contents, Is.EqualTo(contents));
                Assert.That(local.CreatePendingSnapshot(), Is.Empty);
            }
            finally { UnityEngine.Object.DestroyImmediate(window); UnityEditor.SessionState.SetBool(key, armed); }
        }

        private sealed class ToolFile : ILocalSaveFileStore
        {
            public string Contents;
            public int Writes;
            public bool HasSave => Contents != null;
            public bool TryRead(out string contents) { contents = Contents; return HasSave; }
            public bool TryWriteAtomically(string contents) { Writes++; Contents = contents; return true; }
        }

        private sealed class ToolBoundary : IOnlineAuthenticationSession, IOnlineRecordTransport, IOnlineAccountTransport
        {
            public int Calls;
            public Task<OnlineAuthenticationResult> TryAuthenticateAsync() { Calls++; return Task.FromResult(new OnlineAuthenticationResult(true, "fake-player")); }
            public Task<bool> TryClearSessionAsync(string expectedPlayerId) { Calls++; return Task.FromResult(true); }
            public Task<T> CallAsync<T>(string endpoint, string request) { Calls++; return Task.FromResult(default(T)); }
            public Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters) { Calls++; return Task.FromResult(default(T)); }
        }

        [Test]
        public void LegacyStageHttpHelper_RejectsBeforeAutomatedPermissionOrHttpRequest()
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            MethodInfo method = type.GetMethod("PostJsonAsync", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            Task<long> request = (Task<long>)method.Invoke(null, new object[] {
                "https://leaderboards.services.api.unity.com/v1/projects/synthetic/leaderboards/fs-stage-stage-001-r1/scores/players/synthetic",
                "{}", "", null });
            NotSupportedException error = Assert.ThrowsAsync<NotSupportedException>(async () => await request);
            Assert.That(error.Message, Is.EqualTo("StageModeUnsupported"));
        }

        [UnityTest]
        public IEnumerator VerificationWindow_AutomaticTestCannotPrepareSdkOrPersistentSession()
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            ScriptableObject window = ScriptableObject.CreateInstance(type);
            try
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                Assert.That((Vector2)type.GetProperty("minSize").GetValue(window), Is.EqualTo(new Vector2(520f, 420f)));
                type.GetField("_remoteConsent", flags).SetValue(window, true);
                Assert.That(type.GetMethod("CanRunPhase2", flags).Invoke(window, null), Is.EqualTo(false));
                type.GetMethod("PreparePhase2", flags).Invoke(window, null);
                Assert.That(type.GetField("_configuration", flags).GetValue(window), Is.Null);
                Assert.That(type.GetField("_guard", flags).GetValue(window), Is.Null);
                Assert.That(type.GetField("_account", flags).GetValue(window), Is.Null);
                yield return null;
            }
            finally { UnityEngine.Object.DestroyImmediate(window); }
        }

        [Test]
        public void VerificationWindow_AutomaticTestCannotArmIsolation_AndStopClearsPermission()
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            string key = FlowState.Runtime.Features.VerificationSessionConfiguration.EditorIsolationKey;
            bool previous = UnityEditor.SessionState.GetBool(key, false);
            ScriptableObject window = ScriptableObject.CreateInstance(type);
            try
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                UnityEditor.SessionState.SetBool(key, false);
                type.GetMethod("ArmEditorIsolation", flags).Invoke(window, null);
                Assert.That(UnityEditor.SessionState.GetBool(key, false), Is.False);
                UnityEditor.SessionState.SetBool(key, true);
                type.GetField("_remoteConsent", flags).SetValue(window, true);
                Assert.That(type.GetMethod("CanRunPhase2", flags).Invoke(window, null), Is.EqualTo(false));
                type.GetMethod("HandlePlayModeStateChanged", flags).Invoke(window,
                    new object[] { UnityEditor.PlayModeStateChange.ExitingPlayMode });
                Assert.That(UnityEditor.SessionState.GetBool(key, false), Is.False);
                Assert.That(type.GetField("_remoteConsent", flags).GetValue(window), Is.EqualTo(false));
                Assert.That(type.GetField("_configuration", flags).GetValue(window), Is.Null);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(window);
                UnityEditor.SessionState.SetBool(key, previous);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void VerificationWindow_MessageReportIncludesOnlyPermittedContent(bool includeSensitive)
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            ScriptableObject window = ScriptableObject.CreateInstance(type);
            try
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                MethodInfo remember = type.GetMethod("RememberVisibleMessage", flags);
                remember.Invoke(window, new object[] { "State=Error / AuthFailure=SdkFailure", false });
                remember.Invoke(window, new object[] { "private-credential-placeholder", true });
                string report = (string)type.GetMethod("BuildVisibleMessageReport", flags).Invoke(
                    window, new object[] { includeSensitive });
                Assert.That(report, Does.Contain("State=Error / AuthFailure=SdkFailure"));
                Assert.That(report.Contains("private-credential-placeholder"), Is.EqualTo(includeSensitive));
                Assert.That(report.Contains("[민감 정보 제외]"), Is.EqualTo(!includeSensitive));
                Assert.That(type.GetField("_authenticationGateway", flags).GetValue(window), Is.Null);
                type.GetMethod("OnDisable", flags).Invoke(window, null);
                Assert.That(((IList)type.GetField("_visibleMessages", flags).GetValue(window)).Count, Is.Zero);
                Assert.That(((IList)type.GetField("_sensitiveMessages", flags).GetValue(window)).Count, Is.Zero);
            }
            finally { UnityEngine.Object.DestroyImmediate(window); }
        }

        [TestCase("ReadLeaderboard", "ReadLeaderboard")]
        [TestCase("private-response-placeholder", "Unknown")]
        [TestCase(null, "None")]
        public void VerificationWindow_QueryPhaseDoesNotExposeArbitraryText(string phase, string expected)
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            MethodInfo method = type.GetMethod("SafeQueryPhase", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            Assert.That(method.Invoke(null, new object[] { phase }), Is.EqualTo(expected));
        }

        [TestCase("LedgerConflict", "LedgerConflict")]
        [TestCase("private-error-placeholder", "Unknown")]
        [TestCase(null, "None")]
        public void VerificationWindow_QueryFaultDoesNotExposeArbitraryText(string fault, string expected)
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            MethodInfo method = type.GetMethod("SafeQueryFault", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            Assert.That(method.Invoke(null, new object[] { fault }), Is.EqualTo(expected));
        }

        [TestCase("TransferUnavailable", "TransferUnavailable")]
        [TestCase("AccountMismatch", "AccountMismatch")]
        [TestCase("", "None")]
        [TestCase("secret-or-player-identity", "Unknown")]
        public void VerificationWindow_DiagnosticReasonDoesNotCopyArbitraryText(string reason, string expected)
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            MethodInfo method = type.GetMethod("SafeDiagnosticReason", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            Assert.That(method.Invoke(null, new object[] { reason }), Is.EqualTo(expected));
        }
    }
}
#endif
