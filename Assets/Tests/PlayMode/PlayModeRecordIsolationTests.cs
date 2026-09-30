#if UNITY_EDITOR
using System;
using System.Reflection;
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
        [Test]
        public void RecordInitialization_DoesNotUsePersistentSaveOrOnlineServices()
        {
            Type gameSystemType = Type.GetType(
                "FlowState.Runtime.Systems.GameSystem, Assembly-CSharp");
            Assert.That(gameSystemType, Is.Not.Null);
            Type settingsSystemType = Type.GetType(
                "FlowState.Runtime.Systems.SettingsSystem, Assembly-CSharp");
            Assert.That(settingsSystemType, Is.Not.Null);
            GameObject probe = new GameObject("IsolatedRecordInitializationProbe");
            probe.SetActive(false);
            try
            {
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
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
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
