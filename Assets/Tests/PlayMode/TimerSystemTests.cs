using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class TimerSystemTests
    {
        private GameObject _systemObject;
        private MonoBehaviour _timerSystem;

        [SetUp]
        public void SetUp()
        {
            _systemObject = new GameObject("TimerSystemTests.System");
            _timerSystem = AddComponentByName(
                _systemObject,
                "TimerSystem");
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_systemObject);
        }

        [UnityTest]
        public IEnumerator TimerRequests_ValidSequence_ManageMeasuredTime()
        {
            Assert.That(InvokeBool("CreateTimer"), Is.True);
            Assert.That(InvokeBool("StartTimer"), Is.True);
            yield return null;

            double runningTime = InvokeDouble("GetElapsedTime");
            Assert.That(runningTime, Is.GreaterThan(0.0));

            Assert.That(InvokeBool("PauseTimer"), Is.True);
            double pausedTime = InvokeDouble("GetElapsedTime");
            yield return null;
            Assert.That(
                InvokeDouble("GetElapsedTime"),
                Is.EqualTo(pausedTime));

            Assert.That(InvokeBool("ResumeTimer"), Is.True);
            yield return null;
            Assert.That(
                InvokeDouble("GetElapsedTime"),
                Is.GreaterThan(pausedTime));

            Assert.That(InvokeBool("StopTimer"), Is.True);
            double finalTime = InvokeDouble("GetElapsedTime");
            yield return null;
            Assert.That(
                InvokeDouble("GetElapsedTime"),
                Is.EqualTo(finalTime));

            Assert.That(InvokeBool("RemoveTimer"), Is.True);
            Assert.That(InvokeBool("HasTimer"), Is.False);
        }

        [Test]
        public void TimerRequests_DuplicateAndMissingKey_AreRejected()
        {
            Assert.That(InvokeBool("CreateTimer"), Is.True);

            LogAssert.Expect(
                LogType.Warning,
                "[TimerSystem] Timer already exists: PlayTimer.");
            Assert.That(InvokeBool("CreateTimer"), Is.False);

            Assert.That(InvokeBool("RemoveTimer"), Is.True);

            LogAssert.Expect(
                LogType.Warning,
                "[TimerSystem] Timer does not exist: PlayTimer.");
            Assert.That(InvokeBool("StartTimer"), Is.False);
        }

        private MonoBehaviour AddComponentByName(
            GameObject gameObject,
            string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type componentType = assembly.GetType(
                    $"FlowState.Runtime.Systems.{typeName}");

                if (componentType != null &&
                    typeof(MonoBehaviour).IsAssignableFrom(componentType))
                {
                    return (MonoBehaviour)gameObject.AddComponent(componentType);
                }
            }

            Assert.Fail($"{typeName} type was not found.");
            return null;
        }

        private bool InvokeBool(string methodName)
        {
            return (bool)InvokeMethod(methodName);
        }

        private double InvokeDouble(string methodName)
        {
            return (double)InvokeMethod(methodName);
        }

        private object InvokeMethod(string methodName)
        {
            MethodInfo method = _timerSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(
                _timerSystem,
                new object[] { E_TimerKey.PlayTimer });
        }
    }
}
