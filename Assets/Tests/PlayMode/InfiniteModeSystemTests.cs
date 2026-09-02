using System;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.PlayMode
{
    public class InfiniteModeSystemTests
    {
        private const float MinimumHorizontalSpeed = 5.0f;
        private const float BelowSpeedGraceDuration = 0.5f;
        private const float FallThresholdY = -3.0f;
        private const float ScorePerUnit = 10.0f;

        private GameObject _playerObject;
        private GameObject _systemsObject;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _stageSystem;
        private MonoBehaviour _infiniteModeSystem;
        private GameRuntimeData _runtimeData;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("InfiniteModeSystemTests.Player");
            _playerObject.transform.position = Vector3.zero;

            _systemsObject = new GameObject("InfiniteModeSystemTests.Systems");
            _runtimeDataSystem = AddComponentByName(
                _systemsObject,
                "RuntimeDataSystem");
            _stageSystem = AddComponentByName(
                _systemsObject,
                "StageSystem");
            _infiniteModeSystem = AddComponentByName(
                _systemsObject,
                "InfiniteModeSystem");

            SetPrivateField(
                _infiniteModeSystem,
                "_runtimeDataSystem",
                _runtimeDataSystem);
            SetPrivateField(
                _infiniteModeSystem,
                "_stageSystem",
                _stageSystem);
            SetPrivateField(
                _infiniteModeSystem,
                "_player",
                _playerObject.transform);
            SetPrivateField(
                _infiniteModeSystem,
                "_fallThresholdY",
                FallThresholdY);
            SetPrivateField(
                _infiniteModeSystem,
                "_minimumHorizontalSpeed",
                MinimumHorizontalSpeed);
            SetPrivateField(
                _infiniteModeSystem,
                "_startGraceDuration",
                0.0f);
            SetPrivateField(
                _infiniteModeSystem,
                "_belowSpeedGraceDuration",
                BelowSpeedGraceDuration);
            SetPrivateField(
                _infiniteModeSystem,
                "_scorePerUnit",
                ScorePerUnit);

            _runtimeData = (GameRuntimeData)InvokeMethod(
                _runtimeDataSystem,
                "CreateRuntimeData",
                E_GameMode.Infinite);
            Assert.That(InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite), Is.True);
            Assert.That(InvokeBoolMethod(
                _stageSystem,
                "StartStage"), Is.True);
            Assert.That(InvokeBoolMethod(
                _infiniteModeSystem,
                "Initialize",
                E_GameMode.Infinite), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_systemsObject);
            UnityEngine.Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void Initialize_InfiniteRun_CreatesZeroProgressRuntimeData()
        {
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Not.Null);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.Zero);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentScore,
                Is.Zero);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.IsFinalized,
                Is.False);
        }

        [Test]
        public void Pause_InfiniteRun_StopsMetricsAndEndChecksUntilResume()
        {
            _runtimeData.PlayerMovementRuntimeData.UpdateState(
                E_PlayerMovementState.Grounded,
                MinimumHorizontalSpeed,
                0.0f,
                true,
                false,
                false);
            _playerObject.transform.position = new Vector3(10.0f, 0.0f, 0.0f);
            InvokeMethod(_infiniteModeSystem, "FixedUpdate");
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(10.0f));

            Assert.That(
                InvokeBoolMethod(_infiniteModeSystem, "Pause"),
                Is.True);
            _playerObject.transform.position =
                new Vector3(100.0f, FallThresholdY - 1.0f, 0.0f);

            for (int i = 0; i < 40; i++)
            {
                InvokeMethod(_infiniteModeSystem, "FixedUpdate");
            }

            Assert.That(GetBoolProperty(_infiniteModeSystem, "IsPaused"), Is.True);
            Assert.That(GetBoolProperty(_infiniteModeSystem, "HasEnded"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(10.0f));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(100));

            _playerObject.transform.position = new Vector3(20.0f, 0.0f, 0.0f);
            Assert.That(
                InvokeBoolMethod(_infiniteModeSystem, "Resume"),
                Is.True);
            InvokeMethod(_infiniteModeSystem, "FixedUpdate");

            Assert.That(GetBoolProperty(_infiniteModeSystem, "IsPaused"), Is.False);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(20.0f));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(200));
        }

        [Test]
        public void ProcessRunMetrics_PlayerWorldX_UpdatesDistanceAndScore()
        {
            _playerObject.transform.position = new Vector3(12.5f, 0.0f, 0.0f);

            bool didProcess = InvokeBoolMethod(
                _infiniteModeSystem,
                "ProcessRunMetrics");

            Assert.That(didProcess, Is.True);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(12.5f));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(125));
        }

        [Test]
        public void ProcessRunMetrics_BackwardMovement_KeepsMaximumProgress()
        {
            _playerObject.transform.position = new Vector3(12.5f, 0.0f, 0.0f);
            InvokeMethod(_infiniteModeSystem, "ProcessRunMetrics");

            _playerObject.transform.position = new Vector3(5.0f, 0.0f, 0.0f);
            bool didProcess = InvokeBoolMethod(
                _infiniteModeSystem,
                "ProcessRunMetrics");

            Assert.That(didProcess, Is.True);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(12.5f));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(125));
        }

        [Test]
        public void ProcessRunMetrics_LargeWorldX_WorksWithoutPatternData()
        {
            _playerObject.transform.position = new Vector3(10000.0f, 0.0f, 0.0f);

            bool didProcess = InvokeBoolMethod(
                _infiniteModeSystem,
                "ProcessRunMetrics");

            Assert.That(didProcess, Is.True);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(10000.0f));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(100000));
        }

        [Test]
        public void Initialize_StageMode_DoesNotCreateOrUpdateInfiniteProgress()
        {
            InvokeMethod(_infiniteModeSystem, "Stop");
            InvokeMethod(_runtimeDataSystem, "ClearRuntimeData");
            _runtimeData = (GameRuntimeData)InvokeMethod(
                _runtimeDataSystem,
                "CreateRuntimeData",
                E_GameMode.Stage);

            bool didInitialize = InvokeBoolMethod(
                _infiniteModeSystem,
                "Initialize",
                E_GameMode.Stage);
            _playerObject.transform.position = new Vector3(100.0f, 0.0f, 0.0f);
            bool didProcess = InvokeBoolMethod(
                _infiniteModeSystem,
                "ProcessRunMetrics");

            Assert.That(didInitialize, Is.True);
            Assert.That(didProcess, Is.False);
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
        }

        [Test]
        public void Progress_AtMinimumSpeed_KeepsInfiniteStagePlaying()
        {
            SetHorizontalSpeed(MinimumHorizontalSpeed);

            InvokeMethod(
                _infiniteModeSystem,
                "ProcessProgress",
                BelowSpeedGraceDuration);

            Assert.That(GetBoolProperty(_infiniteModeSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void Progress_BelowMinimumForGrace_EndsInfiniteStageOnce()
        {
            int endCount = 0;
            AddListener(
                _stageSystem,
                "AddStageEndedListener",
                () => endCount++);
            SetHorizontalSpeed(MinimumHorizontalSpeed - 0.001f);

            InvokeMethod(
                _infiniteModeSystem,
                "ProcessProgress",
                BelowSpeedGraceDuration);
            InvokeMethod(
                _infiniteModeSystem,
                "ProcessProgress",
                BelowSpeedGraceDuration);

            Assert.That(GetBoolProperty(_infiniteModeSystem, "HasEnded"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(endCount, Is.EqualTo(1));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.IsFinalized,
                Is.True);
        }

        [Test]
        public void FallThreshold_PlayerAtLargeXAndBelowThreshold_EndsWithoutClear()
        {
            _playerObject.transform.position = new Vector3(
                10000.0f,
                FallThresholdY - 0.001f,
                0.0f);
            InvokeMethod(_infiniteModeSystem, "ProcessFallThreshold");

            Assert.That(GetBoolProperty(_infiniteModeSystem, "HasEnded"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.IsFinalized,
                Is.True);
        }

        [Test]
        public void FallThreshold_PlayerAboveThreshold_DoesNotEndInfiniteStage()
        {
            _playerObject.transform.position = new Vector3(
                10000.0f,
                FallThresholdY + 0.001f,
                0.0f);
            InvokeMethod(_infiniteModeSystem, "ProcessFallThreshold");

            Assert.That(GetBoolProperty(_infiniteModeSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
        }

        [Test]
        public void SpeedAndFallThreshold_SequentialRequests_EndStageOnce()
        {
            int endCount = 0;
            AddListener(
                _stageSystem,
                "AddStageEndedListener",
                () => endCount++);
            SetHorizontalSpeed(0.0f);

            _playerObject.transform.position = new Vector3(
                10000.0f,
                FallThresholdY,
                0.0f);
            InvokeMethod(_infiniteModeSystem, "ProcessFallThreshold");
            InvokeMethod(
                _infiniteModeSystem,
                "ProcessProgress",
                BelowSpeedGraceDuration);

            Assert.That(endCount, Is.EqualTo(1));
        }

        private void SetHorizontalSpeed(float horizontalSpeed)
        {
            _runtimeData.PlayerMovementRuntimeData.UpdateState(
                E_PlayerMovementState.Grounded,
                horizontalSpeed,
                0.0f,
                true,
                false,
                false);
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
            Type[] argumentTypes = new Type[arguments.Length];

            for (int index = 0; index < arguments.Length; index++)
            {
                Assert.That(arguments[index], Is.Not.Null);
                argumentTypes[index] = arguments[index].GetType();
            }

            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic,
                null,
                argumentTypes,
                null);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }
    }
}
