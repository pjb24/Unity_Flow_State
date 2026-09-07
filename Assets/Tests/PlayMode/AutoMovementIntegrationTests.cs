using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class AutoMovementIntegrationTests
    {
        private const float SpeedTolerance = 0.001f;
        private MonoBehaviour _gameSystem;
        private MonoBehaviour _movementSystem;
        private MonoBehaviour _inputSystem;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _controllerSystem;
        private MonoBehaviour _timerSystem;
        private Rigidbody _playerRigidbody;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            yield return null;
            _gameSystem = FindSystem("GameSystem", "GameSystem");
            _movementSystem = FindSystem("PlayerMovementSystem", "PlayerMovementSystem");
            _inputSystem = FindSystem("PlayerInputSystem", "PlayerInputSystem");
            _runtimeDataSystem = FindSystem("RuntimeDataSystem", "RuntimeDataSystem");
            _controllerSystem = FindSystem("Player", "PlayerControllerSystem");
            _timerSystem = FindSystem("TimerSystem", "TimerSystem");
            _playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Stage);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_gameSystem != null &&
                GetProperty<E_GameState>(_gameSystem, "CurrentGameState") != E_GameState.Ended)
            {
                Invoke(_gameSystem, "EndGame");
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator StageStart_NoMoveInput_AcceleratesFromZero()
        {
            yield return VerifyAutomaticStart(E_GameMode.Stage);
        }

        [UnityTest]
        public IEnumerator InfiniteStart_NoMoveInput_AcceleratesFromZero()
        {
            yield return VerifyAutomaticStart(E_GameMode.Infinite);
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase(E_GameMode.Infinite)]
        public void PlayingStep_BothModes_UsesCommonGroundAcceleration(E_GameMode gameMode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(gameMode);
            GameRuntimeData runtimeData = GetRuntimeData();
            Physics.SyncTransforms();

            Invoke(_movementSystem, "FixedUpdate");

            Assert.That(runtimeData.PlayerMovementRuntimeData.IsGrounded, Is.True);
            Assert.That(_playerRigidbody.linearVelocity.x,
                Is.EqualTo(50.0f * Time.fixedDeltaTime).Within(SpeedTolerance));
            Assert.That(runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed,
                Is.EqualTo(_playerRigidbody.linearVelocity.x).Within(SpeedTolerance));
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Initializing)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Paused)]
        [TestCase(E_GameState.Ending)]
        [TestCase(E_GameState.Ended)]
        public void MovementStep_NonPlayingRuntimeState_DoesNotApplyMovement(E_GameState state)
        {
            GameRuntimeData runtimeData = GetRuntimeData();
            float originalSpeed = runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed;
            runtimeData.SetGameState(state);

            try
            {
                Invoke(_movementSystem, "FixedUpdate");

                Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
                Assert.That(runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed,
                    Is.EqualTo(originalSpeed));
            }
            finally
            {
                runtimeData.SetGameState(E_GameState.Playing);
            }
        }

        [UnityTest]
        public IEnumerator Initialize_DuringJump_PreservesSameRunAndAirborneState()
        {
            SetField(_inputSystem, "_isJumpPressed", true);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            GameRuntimeData runtimeData = GetRuntimeData();
            PlayerMovementRuntimeData movementData = runtimeData.PlayerMovementRuntimeData;
            Vector3 velocity = _playerRigidbody.linearVelocity;
            Assert.That(movementData.CurrentMovementState, Is.EqualTo(E_PlayerMovementState.Airborne));
            Assert.That(GetField<bool>(_movementSystem, "_isJumpSequenceActive"), Is.True);

            Assert.That((bool)Invoke(_movementSystem, "Initialize"), Is.True);

            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            Assert.That(runtimeData.PlayerMovementRuntimeData, Is.SameAs(movementData));
            Assert.That(GetField<bool>(_movementSystem, "_isJumpSequenceActive"), Is.True);
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(velocity));
            Assert.That(movementData.CurrentMovementState, Is.EqualTo(E_PlayerMovementState.Airborne));

            SetField(_inputSystem, "_isJumpPressed", true);
            yield return new WaitForFixedUpdate();
            Assert.That(_playerRigidbody.linearVelocity.y, Is.LessThan(velocity.y));
            Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
        }

        [UnityTest]
        public IEnumerator Initialize_WhilePaused_DoesNotResumeOrResetSpeed()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            GameRuntimeData runtimeData = GetRuntimeData();
            float speed = runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed;
            Assert.That(speed, Is.GreaterThan(0.0f));
            Assert.That((bool)Invoke(_gameSystem, "PauseGame"), Is.True);
            Vector3 position = _playerRigidbody.position;

            Assert.That((bool)Invoke(_movementSystem, "Initialize"), Is.True);
            yield return new WaitForFixedUpdate();

            Assert.That(GetProperty<bool>(_movementSystem, "IsPaused"), Is.True);
            Assert.That(_playerRigidbody.position, Is.EqualTo(position));
            Assert.That(runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed, Is.EqualTo(speed));
            Assert.That((bool)Invoke(_gameSystem, "ResumeGame"), Is.True);
            Assert.That(_playerRigidbody.linearVelocity.x, Is.EqualTo(speed).Within(SpeedTolerance));
        }

        [UnityTest]
        public IEnumerator StartGame_DuplicateRequest_PreservesAutomaticRun()
        {
            yield return new WaitForFixedUpdate();
            GameRuntimeData runtimeData = GetRuntimeData();
            Vector3 velocity = _playerRigidbody.linearVelocity;
            LogAssert.Expect(LogType.Warning, "[GameSystem] Game is already running.");

            Invoke(_gameSystem, "StartGame");

            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(velocity));
            yield return new WaitForFixedUpdate();
            Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
        }

        [UnityTest]
        public IEnumerator EndGame_AfterAutomaticMovement_RemainsStoppedAcrossPhysicsSteps()
        {
            yield return new WaitForFixedUpdate();
            Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
            Invoke(_gameSystem, "EndGame");
            Vector3 endPosition = _playerRigidbody.position;

            for (int step = 0; step < 5; step++)
            {
                yield return new WaitForFixedUpdate();
                Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
                Assert.That(_playerRigidbody.position, Is.EqualTo(endPosition));
            }

            Assert.That(GetProperty<bool>(_movementSystem, "IsRunning"), Is.False);
            Assert.That(GetProperty<E_GameState>(_gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.Ended));
        }

        [UnityTest]
        public IEnumerator StagePauseResume_FreezesPositionTimerAndClearsTransientInput()
        {
            for (int step = 0; step < 8; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            GameRuntimeData runtimeData = GetRuntimeData();
            float runningSpeed =
                runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed;
            Assert.That(runningSpeed, Is.GreaterThan(0.0f));
            Assert.That((bool)Invoke(_gameSystem, "PauseGame"), Is.True);
            Vector3 pausedPosition = _playerRigidbody.position;
            double pausedTime = (double)Invoke(
                _timerSystem,
                "GetElapsedTime",
                E_TimerKey.PlayTimer);
            SetField(_inputSystem, "_isJumpPressed", true);
            SetField(_inputSystem, "_isMomentumLandingPressed", true);

            for (int step = 0; step < 5; step++)
            {
                yield return new WaitForFixedUpdate();
                Assert.That(_playerRigidbody.position, Is.EqualTo(pausedPosition));
                Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
                Assert.That(
                    (double)Invoke(_timerSystem, "GetElapsedTime", E_TimerKey.PlayTimer),
                    Is.EqualTo(pausedTime).Within(0.001));
            }

            Assert.That((bool)Invoke(_gameSystem, "ResumeGame"), Is.True);
            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            Assert.That(_playerRigidbody.linearVelocity.x,
                Is.EqualTo(runningSpeed).Within(SpeedTolerance));
            object inputState = Invoke(_inputSystem, "GetInputState");
            Assert.That(GetObjectProperty<bool>(inputState, "IsJumpPressed"), Is.False);
            Assert.That(GetObjectProperty<bool>(
                inputState,
                "IsMomentumLandingPressed"), Is.False);

            yield return new WaitForFixedUpdate();
            Assert.That(_playerRigidbody.position.x, Is.GreaterThan(pausedPosition.x));
        }

        [UnityTest]
        public IEnumerator InfinitePauseResume_FreezesPositionDistanceAndScore()
        {
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            for (int step = 0; step < 8; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            GameRuntimeData runtimeData = GetRuntimeData();
            InfiniteModeRuntimeData infiniteData = runtimeData.InfiniteModeRuntimeData;
            Assert.That(infiniteData.CurrentDistance, Is.GreaterThan(0.0f));
            Assert.That((bool)Invoke(_gameSystem, "PauseGame"), Is.True);
            Vector3 pausedPosition = _playerRigidbody.position;
            float pausedDistance = infiniteData.CurrentDistance;
            int pausedScore = infiniteData.CurrentScore;

            for (int step = 0; step < 5; step++)
            {
                yield return new WaitForFixedUpdate();
                Assert.That(_playerRigidbody.position, Is.EqualTo(pausedPosition));
                Assert.That(infiniteData.CurrentDistance, Is.EqualTo(pausedDistance));
                Assert.That(infiniteData.CurrentScore, Is.EqualTo(pausedScore));
            }

            Assert.That((bool)Invoke(_gameSystem, "ResumeGame"), Is.True);
            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(_playerRigidbody.position.x, Is.GreaterThan(pausedPosition.x));
            Assert.That(infiniteData.CurrentDistance, Is.GreaterThan(pausedDistance));
            Assert.That(infiniteData.CurrentScore, Is.GreaterThanOrEqualTo(pausedScore));
        }

        [UnityTest]
        public IEnumerator StagePausedRetry_Twice_CreatesIndependentAutomaticRuns()
        {
            GameRuntimeData previousRuntimeData = GetRuntimeData();

            for (int retryIndex = 0; retryIndex < 2; retryIndex++)
            {
                for (int step = 0; step < 8; step++)
                {
                    yield return new WaitForFixedUpdate();
                }

                Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
                Assert.That((bool)Invoke(_gameSystem, "PauseGame"), Is.True);
                Assert.That((bool)Invoke(_gameSystem, "RetryGame"), Is.True);
                GameRuntimeData currentRuntimeData = GetRuntimeData();

                Assert.That(currentRuntimeData, Is.Not.SameAs(previousRuntimeData));
                Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
                Assert.That(GetProperty<float>(
                    _controllerSystem,
                    "CurrentHorizontalAcceleration"), Is.Zero);
                Assert.That(
                    currentRuntimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed,
                    Is.Zero);
                Assert.That(
                    currentRuntimeData.PlayerMovementRuntimeData.IsLastLandingMomentum,
                    Is.False);

                previousRuntimeData = currentRuntimeData;
                yield return new WaitForFixedUpdate();
                Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
            }
        }

        private IEnumerator VerifyAutomaticStart(E_GameMode gameMode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(gameMode);
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
            float startX = _playerRigidbody.position.x;

            for (int step = 0; step < 12; step++)
            {
                yield return new WaitForFixedUpdate();
                Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
                Assert.That(_playerRigidbody.linearVelocity.x, Is.LessThanOrEqualTo(8.0f));
            }

            Assert.That(_playerRigidbody.position.x, Is.GreaterThan(startX));
            Assert.That(_playerRigidbody.linearVelocity.x, Is.EqualTo(8.0f).Within(SpeedTolerance));
            Assert.That(GetRuntimeData().GameMode, Is.EqualTo(gameMode));
        }

        private GameRuntimeData GetRuntimeData()
        {
            return GetProperty<GameRuntimeData>(_runtimeDataSystem, "RuntimeData");
        }

        private MonoBehaviour FindSystem(string objectName, string typeName)
        {
            GameObject target = GameObject.Find(objectName);
            Assert.That(target, Is.Not.Null);

            foreach (MonoBehaviour behaviour in target.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} is missing from {objectName}.");
            return null;
        }

        private object Invoke(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }

        private T GetProperty<T>(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(propertyName);
            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private T GetField<T>(MonoBehaviour target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }

        private T GetObjectProperty<T>(object target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(propertyName);
            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private void SetField(MonoBehaviour target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
