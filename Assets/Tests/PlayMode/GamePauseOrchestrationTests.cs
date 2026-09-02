using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class GamePauseOrchestrationTests
    {
        private const string SceneName = "SampleScene";

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _stageSystem;
        private MonoBehaviour _timerSystem;
        private MonoBehaviour _playerMovementSystem;
        private MonoBehaviour _playerControllerSystem;
        private MonoBehaviour _infiniteModeSystem;
        private Rigidbody _playerRigidbody;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            RestartInMode(E_GameMode.Stage);
        }

        [UnityTest]
        public IEnumerator StageMode_PauseAndResume_PreservesSameRun()
        {
            object runtimeData = GetRuntimeData();

            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            AssertPausedWithSameRun(runtimeData, E_GameMode.Stage);

            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);

            AssertPlayingWithSameRun(runtimeData, E_GameMode.Stage);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InfiniteMode_PauseAndResume_PreservesSameRun()
        {
            RestartInMode(E_GameMode.Infinite);
            object runtimeData = GetRuntimeData();

            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            AssertPausedWithSameRun(runtimeData, E_GameMode.Infinite);

            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);

            AssertPlayingWithSameRun(runtimeData, E_GameMode.Infinite);
            yield return null;
        }

        [UnityTest]
        public IEnumerator StageMode_Pause_StopsTimerAndPlayerPhysicsUntilResume()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            double elapsedBeforePause = GetPlayTimerElapsedTime();
            _playerRigidbody.linearVelocity = new Vector3(4.0f, 0.0f, 0.0f);

            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            Vector3 pausedPosition = _playerRigidbody.position;
            double pausedElapsedTime = GetPlayTimerElapsedTime();

            yield return new WaitForSecondsRealtime(0.05f);
            yield return new WaitForFixedUpdate();

            Assert.That(pausedElapsedTime, Is.GreaterThanOrEqualTo(elapsedBeforePause));
            Assert.That(GetPlayTimerElapsedTime(), Is.EqualTo(pausedElapsedTime).Within(0.001d));
            Assert.That(_playerRigidbody.position, Is.EqualTo(pausedPosition));
            Assert.That(GetProperty<bool>(_playerMovementSystem, "IsPaused"), Is.True);
            Assert.That(GetProperty<bool>(_playerControllerSystem, "IsPaused"), Is.True);
            Assert.That(GetProperty<bool>(_stageSystem, "IsPaused"), Is.True);

            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);
            yield return new WaitForSecondsRealtime(0.05f);

            Assert.That(GetPlayTimerElapsedTime(), Is.GreaterThan(pausedElapsedTime));
            Assert.That(GetProperty<bool>(_playerMovementSystem, "IsPaused"), Is.False);
            Assert.That(GetProperty<bool>(_playerControllerSystem, "IsPaused"), Is.False);
            Assert.That(GetProperty<bool>(_stageSystem, "IsPaused"), Is.False);
        }

        [UnityTest]
        public IEnumerator InfiniteMode_Pause_StopsRunMetricsAndEndChecksUntilResume()
        {
            RestartInMode(E_GameMode.Infinite);
            object runtimeData = GetRuntimeData();
            object infiniteRuntimeData = GetObjectProperty<object>(
                runtimeData,
                "InfiniteModeRuntimeData");

            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            float pausedDistance = GetObjectProperty<float>(
                infiniteRuntimeData,
                "CurrentDistance");
            int pausedScore = GetObjectProperty<int>(
                infiniteRuntimeData,
                "CurrentScore");
            _playerRigidbody.position = new Vector3(100.0f, -100.0f, 0.0f);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.That(GetProperty<bool>(_infiniteModeSystem, "IsPaused"), Is.True);
            Assert.That(GetObjectProperty<float>(infiniteRuntimeData, "CurrentDistance"),
                Is.EqualTo(pausedDistance));
            Assert.That(GetObjectProperty<int>(infiniteRuntimeData, "CurrentScore"),
                Is.EqualTo(pausedScore));
            Assert.That(GetProperty<bool>(_stageSystem, "HasEnded"), Is.False);

            _playerRigidbody.position = new Vector3(0.0f, 0.0f, 0.0f);
            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);
            Assert.That(GetProperty<bool>(_infiniteModeSystem, "IsPaused"), Is.False);
        }

        [UnityTest]
        public IEnumerator ResultState_PauseRequest_IsRejectedWithoutMutation()
        {
            InvokePublic(_gameSystem, "EndGame");
            AssertGameState(E_GameState.Ended);

            bool result = InvokeBool(_gameSystem, "PauseGame");

            Assert.That(result, Is.False);
            AssertGameState(E_GameState.Ended);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PausedState_EndGame_RemovesPauseAndRuntimeData()
        {
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);

            InvokePublic(_gameSystem, "EndGame");

            AssertGameState(E_GameState.Ended);
            Assert.That(
                GetProperty<bool>(_runtimeDataSystem, "HasRuntimeData"),
                Is.False);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PausedState_Retry_StartsIndependentRunInSameMode()
        {
            object previousRuntimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);

            bool result = InvokeBool(_gameSystem, "RetryGame");

            Assert.That(result, Is.True);
            AssertPlayingWithSameRun(GetRuntimeData(), E_GameMode.Stage);
            Assert.That(GetRuntimeData(), Is.Not.SameAs(previousRuntimeData));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PausedState_StageEndRequest_PrioritizesSingleEndFlow()
        {
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);

            InvokePublic(_stageSystem, "StopStage");
            InvokePublic(_stageSystem, "StopStage");

            AssertGameState(E_GameState.Ended);
            Assert.That(
                GetProperty<bool>(_runtimeDataSystem, "HasRuntimeData"),
                Is.False);
            yield return null;
        }

        private void RestartInMode(E_GameMode gameMode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(gameMode);
            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            _stageSystem = FindRequiredBehaviour("StageSystem", "StageSystem");
            _timerSystem = FindRequiredBehaviour("TimerSystem", "TimerSystem");
            _playerMovementSystem = FindRequiredBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
            _playerControllerSystem = FindRequiredBehaviour(
                "Player",
                "PlayerControllerSystem");
            _infiniteModeSystem = FindRequiredBehaviour(
                "InfiniteModeSystem",
                "InfiniteModeSystem");
            GameObject player = GameObject.Find("Player");
            Assert.That(player, Is.Not.Null);
            _playerRigidbody = player.GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);
        }

        private void AssertPausedWithSameRun(
            object runtimeData,
            E_GameMode gameMode)
        {
            AssertGameState(E_GameState.Paused);
            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            Assert.That(
                GetObjectProperty<E_GameMode>(runtimeData, "GameMode"),
                Is.EqualTo(gameMode));
            Assert.That(
                GetObjectProperty<E_GameState>(runtimeData, "GameState"),
                Is.EqualTo(E_GameState.Paused));
        }

        private void AssertPlayingWithSameRun(
            object runtimeData,
            E_GameMode gameMode)
        {
            AssertGameState(E_GameState.Playing);
            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            Assert.That(
                GetObjectProperty<E_GameMode>(runtimeData, "GameMode"),
                Is.EqualTo(gameMode));
            Assert.That(
                GetObjectProperty<E_GameState>(runtimeData, "GameState"),
                Is.EqualTo(E_GameState.Playing));
        }

        private void AssertGameState(E_GameState expectedState)
        {
            Assert.That(
                GetProperty<E_GameState>(_gameSystem, "CurrentGameState"),
                Is.EqualTo(expectedState));
        }

        private object GetRuntimeData()
        {
            return GetProperty<object>(_runtimeDataSystem, "RuntimeData");
        }

        private double GetPlayTimerElapsedTime()
        {
            MethodInfo method = _timerSystem.GetType().GetMethod(
                "GetElapsedTime",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return (double)method.Invoke(
                _timerSystem,
                new object[] { E_TimerKey.PlayTimer });
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = GameObject.Find(gameObjectName);
            Assert.That(targetObject, Is.Not.Null);

            foreach (MonoBehaviour behaviour in
                     targetObject.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {gameObjectName}.");
            return null;
        }

        private bool InvokeBool(MonoBehaviour target, string methodName)
        {
            return (bool)InvokePublic(target, methodName);
        }

        private object InvokePublic(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, null);
        }

        private T GetProperty<T>(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private T GetObjectProperty<T>(object target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }
    }
}
