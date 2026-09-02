using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
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
        public void Initialize_StageModeWithoutGoal_IsRejected()
        {
            SetPrivateField(_stageSystem, "_stageGoal", null);
            LogAssert.Expect(
                LogType.Error,
                "[StageSystem] Stage Goal is not assigned for Stage Mode.");

            bool didInitialize = InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Stage);

            Assert.That(didInitialize, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsInitialized"), Is.False);
        }

        [Test]
        public void Initialize_InfiniteModeWithoutGoal_PreparesStage()
        {
            SetPrivateField(_stageSystem, "_stageGoal", null);

            bool didInitialize = InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);

            Assert.That(didInitialize, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsInitialized"), Is.True);
            Assert.That(GetEnumProperty<E_GameMode>(
                _stageSystem,
                "CurrentGameMode"), Is.EqualTo(E_GameMode.Infinite));
        }

        [Test]
        public void Initialize_StageModeWithRoots_ActivatesOnlyStageRoot()
        {
            GameObject stageRoot = new GameObject("StageSystemTests.StageRoot");
            GameObject infiniteRoot =
                new GameObject("StageSystemTests.InfiniteRoot");

            try
            {
                stageRoot.SetActive(false);
                infiniteRoot.SetActive(true);
                SetPrivateField(_stageSystem, "_stageModeRoot", stageRoot);
                SetPrivateField(
                    _stageSystem,
                    "_infiniteModeRoot",
                    infiniteRoot);

                bool didInitialize = InvokeBoolMethod(
                    _stageSystem,
                    "Initialize",
                    E_GameMode.Stage);

                Assert.That(didInitialize, Is.True);
                Assert.That(stageRoot.activeSelf, Is.True);
                Assert.That(infiniteRoot.activeSelf, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(stageRoot);
                UnityEngine.Object.DestroyImmediate(infiniteRoot);
            }
        }

        [Test]
        public void Initialize_InfiniteModeWithRoots_ActivatesOnlyInfiniteRoot()
        {
            GameObject stageRoot = new GameObject("StageSystemTests.StageRoot");
            GameObject infiniteRoot =
                new GameObject("StageSystemTests.InfiniteRoot");

            try
            {
                stageRoot.SetActive(true);
                infiniteRoot.SetActive(false);
                SetPrivateField(_stageSystem, "_stageModeRoot", stageRoot);
                SetPrivateField(
                    _stageSystem,
                    "_infiniteModeRoot",
                    infiniteRoot);

                bool didInitialize = InvokeBoolMethod(
                    _stageSystem,
                    "Initialize",
                    E_GameMode.Infinite);

                Assert.That(didInitialize, Is.True);
                Assert.That(stageRoot.activeSelf, Is.False);
                Assert.That(infiniteRoot.activeSelf, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(stageRoot);
                UnityEngine.Object.DestroyImmediate(infiniteRoot);
            }
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

        [UnityTest]
        public IEnumerator GoalReached_InfiniteMode_DoesNotEndStage()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);
            InvokeBoolMethod(_stageSystem, "StartStage");

            InvokeTriggerEnter(_stageGoal, _playerCollider);
            yield return null;

            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void TryEndInfiniteStage_BeforeStart_IsRejected()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);

            bool didEnd = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");

            Assert.That(didEnd, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void TryEndInfiniteStage_StageMode_IsRejected()
        {
            InvokeBoolMethod(_stageSystem, "Initialize");
            InvokeBoolMethod(_stageSystem, "StartStage");

            bool didEnd = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");

            Assert.That(didEnd, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void TryEndInfiniteStage_PlayingStage_EndsOnce()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);
            InvokeBoolMethod(_stageSystem, "StartStage");
            int endCount = 0;
            AddListener(
                _stageSystem,
                "AddStageEndedListener",
                () => endCount++);

            bool didEnd = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");
            bool didEndAgain = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");

            Assert.That(didEnd, Is.True);
            Assert.That(didEndAgain, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(endCount, Is.EqualTo(1));
        }

        [Test]
        public void StartStage_AfterInfiniteEnd_ResetsStageState()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);
            InvokeBoolMethod(_stageSystem, "StartStage");
            InvokeBoolMethod(_stageSystem, "TryEndInfiniteStage");

            bool didRestart = InvokeBoolMethod(_stageSystem, "StartStage");

            Assert.That(didRestart, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void PauseStage_PlayingStage_BlocksGoalUntilResume()
        {
            Assert.That(InvokeBoolMethod(_stageSystem, "Initialize"), Is.True);
            Assert.That(InvokeBoolMethod(_stageSystem, "StartStage"), Is.True);

            Assert.That(InvokeBoolMethod(_stageSystem, "PauseStage"), Is.True);
            InvokeMethod(_stageSystem, "HandleGoalReached");

            Assert.That(GetBoolProperty(_stageSystem, "IsPaused"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);

            Assert.That(InvokeBoolMethod(_stageSystem, "ResumeStage"), Is.True);
            InvokeMethod(_stageSystem, "HandleGoalReached");

            Assert.That(GetBoolProperty(_stageSystem, "IsPaused"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
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
                    $"FlowState.Runtime.Systems.{typeName}");

                if (type == null)
                {
                    type = assembly.GetType(
                        $"FlowState.Runtime.Features.{typeName}");
                }

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
            string methodName,
            params object[] arguments)
        {
            return (bool)InvokeMethod(target, methodName, arguments);
        }

        private object InvokeMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = null;

            foreach (MethodInfo candidate in target.GetType().GetMethods(
                         BindingFlags.Instance |
                         BindingFlags.Public |
                         BindingFlags.NonPublic))
            {
                if (candidate.Name == methodName &&
                    candidate.GetParameters().Length == arguments.Length)
                {
                    method = candidate;
                    break;
                }
            }

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

        private T GetEnumProperty<T>(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
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
