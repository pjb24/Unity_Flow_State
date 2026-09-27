using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class LocalPersistenceIntegrationTests
    {
        [UnityTest]
        public IEnumerator RestoredSettingsAndTutorial_AreAppliedToNewRuntimeState()
        {
            GameObject firstObject = new GameObject("FirstSettingsSystem");
            MonoBehaviour firstSettings = AddSettingsSystem(firstObject);
            InvokePublicMethod(firstSettings, "TrySetMasterVolume", 73);
            LocalSaveData savedData = new LocalSaveData(
                LocalSaveData.CurrentVersion,
                string.Empty,
                (LocalSettingsData)InvokePublicMethod(
                    firstSettings,
                    "CreateLocalSettingsData"),
                true,
                null,
                null);
            UnityEngine.Object.Destroy(firstObject);
            yield return null;

            GameObject secondObject = new GameObject("SecondSettingsSystem");
            MonoBehaviour secondSettings = AddSettingsSystem(secondObject);
            InvokePublicMethod(secondSettings, "ApplyLocalSettings", savedData.Settings);
            GameNavigationState navigationState = new GameNavigationState();
            navigationState.RestoreAutomaticHowToPlayCompleted(
                savedData.HasCompletedTutorial);

            object state = GetPublicProperty(secondSettings, "State");
            Assert.That(GetPublicProperty(state, "MasterVolume"), Is.EqualTo(73));
            Assert.That(navigationState.HasAutomaticHowToPlayCompleted, Is.True);

            UnityEngine.Object.Destroy(secondObject);
        }

        private static MonoBehaviour AddSettingsSystem(GameObject target)
        {
            Type settingsSystemType = Type.GetType(
                "FlowState.Runtime.Systems.SettingsSystem, Assembly-CSharp");
            Assert.That(settingsSystemType, Is.Not.Null);
            return target.AddComponent(settingsSystemType) as MonoBehaviour;
        }

        private static object InvokePublicMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null, methodName + " was not found.");
            return method.Invoke(target, arguments);
        }

        private static object GetPublicProperty(object target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null, propertyName + " was not found.");
            return property.GetValue(target);
        }
    }
}
