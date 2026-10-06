#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public sealed class VerificationToolIsolationTests
    {
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
