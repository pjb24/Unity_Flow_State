using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class GameLifecycleIntegrationTests
    {
        private const string SceneName = "SampleScene";

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _playerMovementSystem;
        private MonoBehaviour _cameraFollow;
        private GameObject _stageHud;
        private GameObject _resultPanel;

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
            yield return new WaitForFixedUpdate();

            _gameSystem = FindRequiredBehaviour(
                "GameSystem",
                "GameSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            _playerInputSystem = FindRequiredBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            _playerMovementSystem = FindRequiredBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
            _cameraFollow = FindRequiredBehaviour(
                "CameraRig",
                "CameraFollow");
            _stageHud = FindSceneGameObject("StageHUD");
            _resultPanel = FindSceneGameObject("ResultPanel");
        }

        [UnityTest]
        public IEnumerator EndGame_ClearsRuntimeAndStopsPhase2Systems()
        {
            AssertPlayingState();

            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;

            Assert.That(
                GetPropertyValue(_gameSystem, "CurrentGameState").ToString(),
                Is.EqualTo("Ended"));
            Assert.That(
                (bool)GetPropertyValue(
                    _runtimeDataSystem,
                    "HasRuntimeData"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerInputSystem,
                    "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerMovementSystem,
                    "IsRunning"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(_cameraFollow, "IsFollowing"),
                Is.False);
            Assert.That(_stageHud.activeSelf, Is.False);
            Assert.That(_resultPanel.activeSelf, Is.True);
        }

        [UnityTest]
        public IEnumerator StartGame_AfterEndGame_RestoresPlayingState()
        {
            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;

            InvokePublicMethod(_gameSystem, "StartGame");
            yield return null;
            yield return new WaitForFixedUpdate();

            AssertPlayingState();
        }

        private void AssertPlayingState()
        {
            Assert.That(
                GetPropertyValue(_gameSystem, "CurrentGameState").ToString(),
                Is.EqualTo("Playing"));
            Assert.That(
                (bool)GetPropertyValue(
                    _runtimeDataSystem,
                    "HasRuntimeData"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerInputSystem,
                    "IsPlayerActionMapEnabled"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerMovementSystem,
                    "IsRunning"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(_cameraFollow, "IsFollowing"),
                Is.True);
            Assert.That(_stageHud.activeSelf, Is.True);
            Assert.That(_resultPanel.activeSelf, Is.False);
        }

        private GameObject FindSceneGameObject(string gameObjectName)
        {
            GameObject[] gameObjects =
                Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name == gameObjectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    return gameObject;
                }
            }

            Assert.Fail($"{gameObjectName} was not found in the loaded Scene.");
            return null;
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = FindSceneGameObject(gameObjectName);

            foreach (MonoBehaviour behaviour in
                     targetObject.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail(
                $"{typeName} was not found on {gameObjectName}.");
            return null;
        }

        private object GetPropertyValue(
            MonoBehaviour targetBehaviour,
            string propertyName)
        {
            PropertyInfo property = targetBehaviour.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return property.GetValue(targetBehaviour);
        }

        private void InvokePublicMethod(
            MonoBehaviour targetBehaviour,
            string methodName)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            method.Invoke(targetBehaviour, null);
        }
    }
}
