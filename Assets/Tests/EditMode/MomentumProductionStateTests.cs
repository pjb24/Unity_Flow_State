using System;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class MomentumProductionStateTests
    {
        private const BindingFlags InstanceMembers =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private GameObject _systemsObject;
        private GameObject _playerObject;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _infiniteModeSystem;
        private MonoBehaviour _movementSystem;
        private MomentumLandingFeature _landingFeature;
        private GameRuntimeData _runtimeData;

        private double Multiplier => GetDouble("CurrentMomentumMultiplier");

        private double RemainingDuration => GetDouble("MomentumRemainingDuration");

        [SetUp]
        public void SetUp()
        {
            _systemsObject = new GameObject(nameof(MomentumProductionStateTests));
            _playerObject = new GameObject("MomentumProductionStateTests.Player");
            _playerObject.AddComponent<Rigidbody>().useGravity = false;
            _runtimeDataSystem = AddSystem("RuntimeDataSystem");
            MonoBehaviour stageSystem = AddSystem("StageSystem");
            MonoBehaviour collisionSystem = AddSystem("CollisionSystem");
            _infiniteModeSystem = AddSystem("InfiniteModeSystem");
            _movementSystem = AddSystem("PlayerMovementSystem");
            _landingFeature = _systemsObject.AddComponent<MomentumLandingFeature>();
            NormalLandingFeature normalLanding =
                _systemsObject.AddComponent<NormalLandingFeature>();
            JumpFeature jump = _systemsObject.AddComponent<JumpFeature>();

            SetField(_infiniteModeSystem, "_runtimeDataSystem", _runtimeDataSystem);
            SetField(_infiniteModeSystem, "_stageSystem", stageSystem);
            SetField(_infiniteModeSystem, "_collisionSystem", collisionSystem);
            SetField(_infiniteModeSystem, "_player", _playerObject.transform);
            SetField(_movementSystem, "_momentumLandingFeature", _landingFeature);
            SetField(_movementSystem, "_normalLandingFeature", normalLanding);
            SetField(_movementSystem, "_jumpFeature", jump);
            StartRun(E_GameMode.Infinite);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_systemsObject);
            UnityEngine.Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void ResolveLanding_PublishesOneIdAndPreservesSpeed()
        {
            Assert.That(CompleteLanding(true), Is.EqualTo(8.0f));
            Assert.That(_runtimeData.PlayerMovementRuntimeData.MomentumLandingSuccessId,
                Is.EqualTo(1));
            Assert.That(Step(0.02), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(10.0));

            // Reading the same successful landing again must not refresh its reward.
            Assert.That(Step(1.0), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(9.0));
            Assert.That(_runtimeData.PlayerMovementRuntimeData.TryRecordMomentumLanding(1),
                Is.False);
            Assert.That(_runtimeData.PlayerMovementRuntimeData.TryRecordMomentumLanding(0),
                Is.False);
            Assert.That(_runtimeData.PlayerMovementRuntimeData.TryRecordMomentumLanding(-1),
                Is.False);
        }

        [Test]
        public void FixedUpdate_ConsumesPublishedLandingAndThenAdvancesTimer()
        {
            CompleteLanding(true);
            Invoke(_infiniteModeSystem, "FixedUpdate");
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(10.0));
            Invoke(_infiniteModeSystem, "FixedUpdate");
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration,
                Is.EqualTo(10.0 - Time.fixedDeltaTime).Within(0.000001));
        }

        [Test]
        public void ProductionScore_AppliesMultiplierOnlyToDistanceAfterSuccess()
        {
            SetPlayerX(10.0f);
            Assert.That(Invoke(_infiniteModeSystem, "ProcessRunMetrics"), Is.True);
            AssertScore(100, 0, 100, 0, 100);

            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            SetPlayerX(20.0f);
            Assert.That(Invoke(_infiniteModeSystem, "ProcessRunMetrics"), Is.True);
            AssertScore(200, 25, 225, 0, 225);
        }

        [Test]
        public void ProductionScore_CollectibleIsAddedWithoutMomentumMultiplier()
        {
            SetPlayerX(10.0f);
            Assert.That(Invoke(_infiniteModeSystem, "ProcessRunMetrics"), Is.True);
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(_runtimeData.CollectibleRuntimeData.TryCreateScope(
                out long scopeId), Is.True);
            Assert.That(_runtimeData.CollectibleRuntimeData.TryRegister(
                scopeId, "coin"), Is.True);
            Assert.That(_runtimeData.CollectibleRuntimeData.TryCollect(
                scopeId, "coin"), Is.True);
            SetPlayerX(20.0f);

            Assert.That(Invoke(_infiniteModeSystem, "ProcessRunMetrics"), Is.True);

            AssertScore(200, 25, 225, 10, 235);
        }

        [Test]
        public void Finalize_TransfersVersionAndCompleteScoreToResultSystem()
        {
            SetPlayerX(10.0f);
            Assert.That(Invoke(_infiniteModeSystem, "ProcessRunMetrics"), Is.True);
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            SetPlayerX(20.0f);
            Invoke(_infiniteModeSystem, "FinalizeRunMetrics");
            InfiniteModeRuntimeData data = _runtimeData.InfiniteModeRuntimeData;
            Assert.That(data.IsFinalized, Is.True);
            Assert.That(data.ScoringVersion, Is.EqualTo(ScoringVersion.Current));

            MonoBehaviour resultSystem = AddSystem("ResultSystem");
            Invoke(resultSystem, "Initialize");
            Assert.That(Invoke(
                resultSystem,
                "CreateInfiniteResultData",
                data.ScoringVersion,
                E_GameMode.Infinite,
                true,
                data.IsFinalized,
                data.CurrentDistance,
                data.BaseDistanceScore,
                data.MomentumBonus,
                data.CurrentScore,
                data.CollectibleScore,
                data.MaximumMomentumMultiplier), Is.True);
            ResultData result = (ResultData)GetProperty(
                resultSystem, "CurrentResultData");
            Assert.That(result.ScoringVersion, Is.EqualTo(ScoringVersion.Current));
            Assert.That(result.BaseDistanceScore, Is.EqualTo(200));
            Assert.That(result.MomentumBonus, Is.EqualTo(25));
            Assert.That(result.DistanceScore, Is.EqualTo(225));
            Assert.That(result.CollectibleScore, Is.Zero);
            Assert.That(result.TotalScore, Is.EqualTo(225));
            Assert.That(result.MaximumMomentumMultiplier, Is.EqualTo(1.25));
        }

        [Test]
        public void Finalize_DuplicateRequestDoesNotChangeRuntimeValues()
        {
            SetPlayerX(20.0f);
            Invoke(_infiniteModeSystem, "FinalizeRunMetrics");
            InfiniteModeRuntimeData data = _runtimeData.InfiniteModeRuntimeData;
            int score = data.CurrentScore;
            SetPlayerX(40.0f);

            Assert.That(Invoke(_infiniteModeSystem, "ProcessRunMetrics"), Is.False);

            Assert.That(data.CurrentDistance, Is.EqualTo(20.0f));
            Assert.That(data.CurrentScore, Is.EqualTo(score));
        }

        [Test]
        public void LastLandingFlag_WithoutNewSuccessId_DoesNotApplyReward()
        {
            _runtimeData.PlayerMovementRuntimeData.UpdateState(
                E_PlayerMovementState.MomentumLanding, 8.0f, 0.0f,
                true, false, true);
            Assert.That(Step(0.02), Is.True);
            Assert.That(Step(0.02), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.0));
            Assert.That(RemainingDuration, Is.Zero);
        }

        [Test]
        public void ResolveLanding_SecondSuccessIsProcessedOnce()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            CompleteLanding(true);
            Assert.That(_runtimeData.PlayerMovementRuntimeData.MomentumLandingSuccessId,
                Is.EqualTo(2));
            Assert.That(Step(0.02), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.50));
            Assert.That(RemainingDuration, Is.EqualTo(9.5));
            Assert.That(Step(0.5), Is.True);
            Assert.That(RemainingDuration, Is.EqualTo(9.0));
        }

        [Test]
        public void StageLanding_DoesNotStartOrAdvanceMomentum()
        {
            StartRun(E_GameMode.Stage);
            Assert.That(CompleteLanding(true), Is.EqualTo(8.0f));
            Assert.That(Step(1.0), Is.False);
            Assert.That(Multiplier, Is.EqualTo(1.0));
            Assert.That(RemainingDuration, Is.Zero);
        }

        [Test]
        public void NormalLandingAndWallConstraint_DoNotResetMomentum()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(CompleteLanding(false), Is.EqualTo(8.0f));
            Assert.That(_runtimeData.PlayerMovementRuntimeData.MomentumLandingSuccessId,
                Is.EqualTo(1));
            Assert.That(Step(0.0), Is.True);

            PlayerWallContactState wall = new PlayerWallContactState(
                false, Vector3.zero, true, Vector3.left);
            Vector3 constrained = PlayerMovementMath.ConstrainVelocityByWalls(
                new Vector3(8.0f, -2.0f, 0.0f), false, wall);
            Assert.That(constrained.x, Is.Zero);
            _runtimeData.PlayerMovementRuntimeData.UpdateState(
                E_PlayerMovementState.Airborne, constrained.x, constrained.y,
                false, false, false);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(10.0));
            Assert.That(Step(1.0), Is.True);
            Assert.That(RemainingDuration, Is.EqualTo(9.0));
        }

        [TestCase(1, 1.25, 10.0)]
        [TestCase(2, 1.50, 9.5)]
        [TestCase(3, 1.75, 9.0)]
        [TestCase(4, 2.00, 8.5)]
        [TestCase(5, 2.25, 8.0)]
        [TestCase(6, 2.50, 7.5)]
        [TestCase(7, 2.75, 7.0)]
        [TestCase(8, 3.00, 6.5)]
        [TestCase(9, 3.00, 6.5)]
        public void ConsecutiveLandings_ExpireAtEachStageBoundary(
            int count, double multiplier, double duration)
        {
            for (int index = 0; index < count; index++)
            {
                CompleteLanding(true);
                Assert.That(Step(0.0), Is.True);
            }

            Assert.That(Multiplier, Is.EqualTo(multiplier));
            Assert.That(RemainingDuration, Is.EqualTo(duration));
            Assert.That(Step(duration - 0.25), Is.True);
            Assert.That(Multiplier, Is.EqualTo(multiplier));
            Assert.That(RemainingDuration, Is.EqualTo(0.25));
            Assert.That(Step(0.25), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.0));
            Assert.That(RemainingDuration, Is.Zero);
            Assert.That(GetDouble("MaximumMomentumMultiplier"), Is.EqualTo(multiplier));
        }

        [Test]
        public void SuccessAtExpiry_RefreshesNextStageBeforeTimerExpires()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Step(9.5), Is.True);
            CompleteLanding(true);
            Assert.That(Step(0.5), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.50));
            Assert.That(RemainingDuration, Is.EqualTo(9.5));
        }

        [Test]
        public void PauseResume_PreservesTimerAndConsumedSuccessId()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Step(2.0), Is.True);
            Assert.That(Invoke(_infiniteModeSystem, "Pause"), Is.True);
            _runtimeData.SetGameState(E_GameState.Paused);
            Assert.That(Step(20.0), Is.False);
            Assert.That(RemainingDuration, Is.EqualTo(8.0));
            Assert.That(Invoke(_infiniteModeSystem, "Resume"), Is.True);
            _runtimeData.SetGameState(E_GameState.Playing);
            Assert.That(Step(1.0), Is.True);
            Assert.That(RemainingDuration, Is.EqualTo(7.0));
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Initializing)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Paused)]
        [TestCase(E_GameState.Ending)]
        [TestCase(E_GameState.Ended)]
        public void NonPlayingState_DoesNotConsumeSuccessOrAdvanceTimer(E_GameState state)
        {
            CompleteLanding(true);
            _runtimeData.SetGameState(state);
            Assert.That(Step(1.0), Is.False);
            Assert.That(Multiplier, Is.EqualTo(1.0));
            _runtimeData.SetGameState(E_GameState.Playing);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
        }

        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void InvalidDeltaTime_DoesNotConsumePendingSuccess(double deltaTime)
        {
            CompleteLanding(true);
            Assert.That(Step(deltaTime), Is.False);
            Assert.That(Multiplier, Is.EqualTo(1.0));
            Assert.That(Step(0.0), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(Step(deltaTime), Is.False);
            Assert.That(RemainingDuration, Is.EqualTo(10.0));
        }

        [Test]
        public void FinalizeThenStopAndClear_PreservesMomentumUntilNewRun()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Step(2.0), Is.True);
            Invoke(_infiniteModeSystem, "FinalizeRunMetrics");
            Assert.That(_runtimeData.InfiniteModeRuntimeData.IsFinalized, Is.True);
            Assert.That(Step(100.0), Is.False);
            Invoke(_infiniteModeSystem, "Stop");
            Invoke(_runtimeDataSystem, "ClearRuntimeData");
            Assert.That(Step(100.0), Is.False);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(8.0));

            StartRun(E_GameMode.Infinite);
            Assert.That(Multiplier, Is.EqualTo(1.0));
            Assert.That(RemainingDuration, Is.Zero);
            Assert.That(_runtimeData.PlayerMovementRuntimeData.MomentumLandingSuccessId,
                Is.Zero);
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
        }

        [Test]
        public void PauseThenStop_PreservesFinalStateAndRejectsResume()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Invoke(_infiniteModeSystem, "Pause"), Is.True);
            Invoke(_infiniteModeSystem, "Stop");
            Invoke(_infiniteModeSystem, "Stop");
            Assert.That(Invoke(_infiniteModeSystem, "Resume"), Is.False);
            Assert.That(Step(1.0), Is.False);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(10.0));
        }

        [Test]
        public void Initialize_SamePausedRun_PreservesMomentumAndConsumedId()
        {
            CompleteLanding(true);
            Assert.That(Step(0.0), Is.True);
            Assert.That(Step(2.0), Is.True);
            Assert.That(Invoke(_infiniteModeSystem, "Pause"), Is.True);
            Assert.That(Invoke(_infiniteModeSystem, "Initialize", E_GameMode.Infinite), Is.True);
            Assert.That(GetProperty(_infiniteModeSystem, "IsPaused"), Is.True);
            Assert.That(Multiplier, Is.EqualTo(1.25));
            Assert.That(RemainingDuration, Is.EqualTo(8.0));
            Assert.That(Invoke(_infiniteModeSystem, "Resume"), Is.True);
            Assert.That(Step(1.0), Is.True);
            Assert.That(RemainingDuration, Is.EqualTo(7.0));
        }

        [Test]
        public void ExecutionOrder_MovementPublishesBeforeInfiniteMode()
        {
            DefaultExecutionOrder movementOrder =
                _movementSystem.GetType().GetCustomAttribute<DefaultExecutionOrder>();
            DefaultExecutionOrder infiniteOrder =
                _infiniteModeSystem.GetType().GetCustomAttribute<DefaultExecutionOrder>();
            Assert.That(movementOrder, Is.Not.Null);
            Assert.That(infiniteOrder, Is.Not.Null);
            Assert.That(movementOrder.order, Is.LessThan(infiniteOrder.order));
        }

        private void StartRun(E_GameMode mode)
        {
            Invoke(_infiniteModeSystem, "Stop");
            if ((bool)GetProperty(_runtimeDataSystem, "HasRuntimeData"))
            {
                Invoke(_runtimeDataSystem, "ClearRuntimeData");
            }

            _runtimeData = (GameRuntimeData)Invoke(
                _runtimeDataSystem, "CreateRuntimeData", mode);
            _runtimeData.SetGameState(E_GameState.Playing);
            _landingFeature.Initialize();
            SetField(_movementSystem, "_runtimeData", _runtimeData.PlayerMovementRuntimeData);
            Assert.That(Invoke(_infiniteModeSystem, "Initialize", mode), Is.True);
        }

        private float CompleteLanding(bool hasBufferedInput)
        {
            _landingFeature.BeginJump();
            NormalLandingFeature normalLanding =
                _systemsObject.GetComponent<NormalLandingFeature>();
            normalLanding.BeginJump();
            _landingFeature.UpdateWindow(
                -10.0f,
                new PlayerCollisionState(false, 1.0f, Vector3.zero, Vector3.up),
                0.02f);
            if (hasBufferedInput)
            {
                _landingFeature.BufferInput(new PlayerInputState(false, true));
            }

            SetField(_movementSystem, "_hasJumpLeftGround", true);
            Type calculationType = _movementSystem.GetType().GetNestedType(
                "MovementCalculation", BindingFlags.NonPublic);
            Assert.That(calculationType, Is.Not.Null);
            object calculation = Activator.CreateInstance(calculationType);
            calculationType.GetProperty("HorizontalSpeed").SetValue(calculation, 8.0f);
            calculationType.GetProperty("VerticalSpeed").SetValue(calculation, -1.0f);
            object[] arguments =
            {
                new PlayerCollisionState(true, 0.0f, Vector3.zero, Vector3.up),
                calculation
            };
            Invoke(_movementSystem, "ResolveLanding", arguments);
            return (float)calculationType.GetProperty("HorizontalSpeed").GetValue(arguments[1]);
        }

        private bool Step(double deltaTime)
        {
            return (bool)Invoke(_infiniteModeSystem, "ProcessMomentumStep", deltaTime);
        }

        private double GetDouble(string propertyName)
        {
            return (double)GetProperty(_infiniteModeSystem, propertyName);
        }

        private void SetPlayerX(float x)
        {
            Rigidbody body = _playerObject.GetComponent<Rigidbody>();
            body.position = new Vector3(x, 0.0f, 0.0f);
        }

        private void AssertScore(
            int baseDistanceScore,
            int momentumBonus,
            int distanceScore,
            int collectibleScore,
            int totalScore)
        {
            InfiniteModeRuntimeData data = _runtimeData.InfiniteModeRuntimeData;
            Assert.That(data.BaseDistanceScore, Is.EqualTo(baseDistanceScore));
            Assert.That(data.MomentumBonus, Is.EqualTo(momentumBonus));
            Assert.That(data.CurrentScore, Is.EqualTo(distanceScore));
            Assert.That(data.CollectibleScore, Is.EqualTo(collectibleScore));
            Assert.That(data.TotalScore, Is.EqualTo(totalScore));
        }

        private MonoBehaviour AddSystem(string typeName)
        {
            string fullName = "FlowState.Runtime.Systems." + typeName;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);
                if (type != null)
                {
                    return (MonoBehaviour)_systemsObject.AddComponent(type);
                }
            }

            Assert.Fail("Production System type was not found: " + fullName);
            return null;
        }

        private static object GetProperty(object target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(propertyName, InstanceMembers);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, InstanceMembers);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private static object Invoke(object target, string methodName, params object[] arguments)
        {
            Type[] argumentTypes = new Type[arguments.Length];
            for (int index = 0; index < arguments.Length; index++)
            {
                argumentTypes[index] = arguments[index].GetType();
            }

            MethodInfo method = target.GetType().GetMethod(
                methodName, InstanceMembers, null, argumentTypes, null);
            if (method == null)
            {
                // ResolveLanding has in/ref parameters whose boxed values are passed here.
                method = target.GetType().GetMethod(methodName, InstanceMembers);
            }

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }
    }
}
