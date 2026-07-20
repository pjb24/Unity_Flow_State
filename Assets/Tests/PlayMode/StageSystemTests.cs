using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class StageSystemTests
    {
        private GameObject _playerObject;
        private GameObject _goalObject;
        private GameObject _systemObject;
        private Collider _playerCollider;
        private MonoBehaviour _stageGoal;
        private MonoBehaviour _stageSystem;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("StageSystemTests.Player");
            _playerCollider = _playerObject.AddComponent<CapsuleCollider>();

            _goalObject = new GameObject("StageSystemTests.Goal");
            BoxCollider goalCollider = _goalObject.AddComponent<BoxCollider>();
            goalCollider.isTrigger = true;
            _stageGoal = AddComponentByName(_goalObject, "StageGoal");
            SetPrivateField(_stageGoal, "_playerCollider", _playerCollider);

            _systemObject = new GameObject("StageSystemTests.System");
            _stageSystem = AddComponentByName(_systemObject, "StageSystem");
            SetPrivateField(_stageSystem, "_stageGoal", _stageGoal);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_systemObject);
            UnityEngine.Object.DestroyImmediate(_goalObject);
            UnityEngine.Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void Initialize_ValidReferences_PreparesStage()
        {
            bool didInitialize = InvokeBoolMethod(_stageSystem, "Initialize");

            Assert.That(didInitialize, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsInitialized"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void StartStage_SecondRequest_IsRejected()
        {
            InvokeBoolMethod(_stageSystem, "Initialize");
            bool didStart = InvokeBoolMethod(_stageSystem, "StartStage");

            LogAssert.Expect(
                LogType.Warning,
                "[StageSystem] Stage is already running.");
            bool didStartAgain = InvokeBoolMethod(_stageSystem, "StartStage");

            Assert.That(didStart, Is.True);
            Assert.That(didStartAgain, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
        }

        [UnityTest]
        public IEnumerator GoalReached_PlayingStage_ClearsAndEndsOnce()
        {
            InvokeBoolMethod(_stageSystem, "Initialize");
            InvokeBoolMethod(_stageSystem, "StartStage");
            int clearCount = 0;
            int endCount = 0;
            AddListener(_stageSystem, "AddStageClearedListener", () => clearCount++);
            AddListener(_stageSystem, "AddStageEndedListener", () => endCount++);

            InvokeTriggerEnter(_stageGoal, _playerCollider);
            InvokeTriggerEnter(_stageGoal, _playerCollider);
            yield return null;

            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(clearCount, Is.EqualTo(1));
            Assert.That(endCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GoalReached_BeforeStageStart_DoesNotClearStage()
        {
            InvokeBoolMethod(_stageSystem, "Initialize");

            InvokeTriggerEnter(_stageGoal, _playerCollider);
            yield return null;

            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [UnityTest]
        public IEnumerator GoalReached_NonPlayerCollider_DoesNotClearStage()
        {
            GameObject otherObject = new GameObject("StageSystemTests.Other");
            Collider otherCollider = otherObject.AddComponent<BoxCollider>();

            try
            {
                InvokeBoolMethod(_stageSystem, "Initialize");
                InvokeBoolMethod(_stageSystem, "StartStage");

                InvokeTriggerEnter(_stageGoal, otherCollider);
                yield return null;

                Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
                Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(otherObject);
            }
        }

        [UnityTest]
        public IEnumerator StartStage_AfterClear_ResetsStageState()
        {
            InvokeBoolMethod(_stageSystem, "Initialize");
            InvokeBoolMethod(_stageSystem, "StartStage");
            InvokeTriggerEnter(_stageGoal, _playerCollider);

            bool didRestart = InvokeBoolMethod(_stageSystem, "StartStage");
            yield return null;

            Assert.That(didRestart, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        private MonoBehaviour AddComponentByName(
            GameObject gameObject,
            string typeName)
        {
            Type componentType = FindType(typeName);
            Assert.That(componentType, Is.Not.Null);
            return (MonoBehaviour)gameObject.AddComponent(componentType);
        }

        private Type FindType(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(
                    $"FlowState.Runtime.Systems.{typeName}") ??
                    assembly.GetType(
                        $"FlowState.Runtime.Features.{typeName}");

                if (type != null &&
                    typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    return type;
                }
            }

            return null;
        }

        private void SetPrivateField(
            MonoBehaviour target,
            string fieldName,
            object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private bool InvokeBoolMethod(
            MonoBehaviour target,
            string methodName)
        {
            return (bool)InvokeMethod(target, methodName);
        }

        private object InvokeMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }

        private bool GetBoolProperty(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (bool)property.GetValue(target);
        }

        private void AddListener(
            MonoBehaviour target,
            string methodName,
            Action listener)
        {
            InvokeMethod(target, methodName, listener);
        }

        private void InvokeTriggerEnter(
            MonoBehaviour target,
            Collider otherCollider)
        {
            InvokeMethod(target, "OnTriggerEnter", otherCollider);
        }
    }
}
