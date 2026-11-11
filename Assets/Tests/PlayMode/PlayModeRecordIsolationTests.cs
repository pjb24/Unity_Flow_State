#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Threading.Tasks;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.PlayMode
{
    [SetUpFixture]
    public sealed class PlayModeRecordIsolationSetup
    {
        private static FieldInfo IsolationField => Type.GetType(
            "FlowState.Runtime.Systems.GameSystem, Assembly-CSharp")?.GetField(
            "UseIsolatedRecordsForPlayModeTests",
            BindingFlags.Static | BindingFlags.NonPublic);

        [OneTimeSetUp]
        public void EnableIsolation()
        {
            Assert.That(IsolationField, Is.Not.Null);
            IsolationField.SetValue(null, true);
        }

        [OneTimeTearDown]
        public void DisableIsolation()
        {
            if (IsolationField != null) IsolationField.SetValue(null, false);
        }
    }

    public sealed class PlayModeRecordIsolationTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void RecordInitialization_DoesNotUsePersistentSaveOrOnlineServices(bool useEditorReservation)
        {
            Type gameSystemType = Type.GetType(
                "FlowState.Runtime.Systems.GameSystem, Assembly-CSharp");
            Assert.That(gameSystemType, Is.Not.Null);
            Type settingsSystemType = Type.GetType(
                "FlowState.Runtime.Systems.SettingsSystem, Assembly-CSharp");
            Assert.That(settingsSystemType, Is.Not.Null);
            FieldInfo isolation = gameSystemType.GetField("UseIsolatedRecordsForPlayModeTests",
                BindingFlags.Static | BindingFlags.NonPublic);
            bool previousIsolation = (bool)isolation.GetValue(null);
            string isolationKey = VerificationSessionConfiguration.EditorIsolationKey;
            bool previousReservation = UnityEditor.SessionState.GetBool(isolationKey, false);
            GameObject probe = new GameObject("IsolatedRecordInitializationProbe");
            probe.SetActive(false);
            try
            {
                isolation.SetValue(null, !useEditorReservation);
                UnityEditor.SessionState.SetBool(isolationKey, useEditorReservation);
                MonoBehaviour settings = (MonoBehaviour)probe.AddComponent(settingsSystemType);
                MonoBehaviour gameSystem = (MonoBehaviour)probe.AddComponent(gameSystemType);
                SetField(gameSystem, "_settingsSystem", settings);
                InvokePrivateMethod(gameSystem, "InitializeLocalState");

                LocalRecordRepository local = (LocalRecordRepository)GetField(
                    gameSystem, "_localRecordRepository");
                Assert.That(local, Is.Not.Null);
                object fileStore = GetField(local, "_fileStore");
                Assert.That(fileStore, Is.InstanceOf<ILocalSaveFileStore>());
                Assert.That(fileStore, Is.Not.InstanceOf<PersistentLocalSaveFileStore>());
                Assert.That(GetField(gameSystem, "_onlineRepository"), Is.Null);
                Assert.That(GetField(gameSystem, "_onlineRecords"), Is.Null);
                Assert.That(local.OnlineAccount.HasConfirmedRecoveryNotice, Is.False);
                Assert.That(local.CreatePendingSnapshot().Count, Is.Zero);

                MethodInfo confirm = gameSystemType.GetMethod("ConfirmOnlineRecoveryNoticeAsync");
                Assert.That(confirm, Is.Not.Null);
                Task<bool> confirmation = (Task<bool>)confirm.Invoke(gameSystem, null);
                Assert.That(confirmation.IsCompleted, Is.True);
                Assert.That(confirmation.Result, Is.True);
                Assert.That(local.OnlineAccount.HasConfirmedRecoveryNotice, Is.True);
                Assert.That(local.OnlineAccount.PlayerId, Is.Empty);
                Assert.That(local.OnlineAccount.PublicNumberCache, Is.Empty);
                Assert.That(GetField(gameSystem, "_onlineRecords"), Is.Null);
                Assert.That(((Task<bool>)confirm.Invoke(gameSystem, null)).Result, Is.True);
                Assert.That((bool)gameSystemType.GetMethod("HasConfirmedOnlineRecoveryNotice",
                    BindingFlags.Instance | BindingFlags.NonPublic).Invoke(gameSystem, null), Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
                isolation.SetValue(null, previousIsolation);
                UnityEditor.SessionState.SetBool(isolationKey, previousReservation);
            }
        }

        [Test]
        public void UninitializedNormalGame_ConsentCannotBypassOnlineCoordinator()
        {
            Type gameSystemType = Type.GetType("FlowState.Runtime.Systems.GameSystem, Assembly-CSharp");
            GameObject probe = new GameObject("UninitializedConsentProbe");
            probe.SetActive(false);
            try
            {
                MonoBehaviour gameSystem = (MonoBehaviour)probe.AddComponent(gameSystemType);
                Task<bool> result = (Task<bool>)gameSystemType.GetMethod("ConfirmOnlineRecoveryNoticeAsync").Invoke(gameSystem, null);
                Assert.That(result.IsCompleted, Is.True);
                Assert.That(result.Result, Is.False);
                Assert.That(GetField(gameSystem, "_localRecordRepository"), Is.Null);
            }
            finally { UnityEngine.Object.DestroyImmediate(probe); }
        }

        private static object GetField(object target, string name)
        {
            FieldInfo field = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return field.GetValue(target);
        }

        private static void SetField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private static void InvokePrivateMethod(object target, string name)
        {
            MethodInfo method = target.GetType().GetMethod(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
        }
    }
}
#endif
