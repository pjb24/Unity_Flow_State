using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
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
        private MonoBehaviour _uiInputSystem;
        private MonoBehaviour _playerMovementSystem;
        private MonoBehaviour _stageSystem;
        private MonoBehaviour _cameraFollow;
        private Rigidbody _playerRigidbody;
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
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Stage);
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
            _uiInputSystem = FindRequiredBehaviour(
                "UIInputSystem",
                "UIInputSystem");
            _playerMovementSystem = FindRequiredBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
            _stageSystem = FindRequiredBehaviour(
                "StageSystem",
                "StageSystem");
            _cameraFollow = FindRequiredBehaviour(
                "CameraRig",
                "CameraFollow");
            _playerRigidbody = FindSceneGameObject("Player")
                .GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);
            _stageHud = FindSceneGameObject("StageHUD");
            _resultPanel = FindSceneGameObject("ResultPanel");
        }

        [UnityTest]
        public IEnumerator EndGame_ClearsRuntimeAndStopsPhase2Systems()
        {
            AssertPlayingState();
            _playerRigidbody.linearVelocity = new Vector3(8.0f, 4.0f, 0.0f);

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
                    _uiInputSystem,
                    "IsUIActionMapEnabled"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerMovementSystem,
                    "IsRunning"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(_stageSystem, "IsPlaying"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(_stageSystem, "HasEnded"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(_cameraFollow, "IsFollowing"),
                Is.False);
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
            Assert.That(_playerRigidbody.angularVelocity, Is.EqualTo(Vector3.zero));
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

        [UnityTest]
        public IEnumerator PlayingState_EnablesUIInputAndKeepsStateEmpty()
        {
            AssertPlayingState();

            object inputState = InvokePublicMethod(
                _uiInputSystem,
                "GetInputState");

            Assert.That(
                GetObjectProperty<Vector2>(
                    inputState,
                    "NavigateInput"),
                Is.EqualTo(Vector2.zero));
            Assert.That(
                GetObjectProperty<bool>(
                    inputState,
                    "IsPointChanged"),
                Is.False);
            Assert.That(
                GetObjectProperty<bool>(
                    inputState,
                    "IsSubmitPressed"),
                Is.False);
            Assert.That(
                GetObjectProperty<bool>(
                    inputState,
                    "IsCancelPressed"),
                Is.False);
            Assert.That(
                GetObjectProperty<bool>(
                    inputState,
                    "IsClickPressed"),
                Is.False);

            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayingState_ConsumesUnusedUITransientInput()
        {
            AssertPlayingState();
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);
            SetPrivateField(_uiInputSystem, "_isClickPressed", true);

            yield return null;

            AssertPlayingState();
            Assert.That(
                GetObjectProperty<bool>(
                    InvokePublicMethod(_uiInputSystem, "GetInputState"),
                    "IsSubmitPressed"),
                Is.False);
            Assert.That(
                GetObjectProperty<bool>(
                    InvokePublicMethod(_uiInputSystem, "GetInputState"),
                    "IsClickPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator CancelInput_PlayingState_PausesOnceAndSwitchesActionMaps()
        {
            AssertPlayingState();
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);

            yield return null;

            AssertPausedInputState();
            Assert.That(
                GetObjectProperty<bool>(
                    InvokePublicMethod(_uiInputSystem, "GetInputState"),
                    "IsCancelPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator CancelInput_PausedState_ResumesAndRestoresActionMaps()
        {
            Assert.That(
                (bool)InvokePublicMethod(_gameSystem, "PauseGame"),
                Is.True);
            AssertPausedInputState();
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);

            yield return null;

            AssertPlayingState();
            Assert.That(
                GetObjectProperty<bool>(
                    InvokePublicMethod(_uiInputSystem, "GetInputState"),
                    "IsCancelPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator PauseGame_DuplicateRequest_IsRejectedWithoutMutation()
        {
            Assert.That(
                (bool)InvokePublicMethod(_gameSystem, "PauseGame"),
                Is.True);

            bool duplicateResult =
                (bool)InvokePublicMethod(_gameSystem, "PauseGame");

            Assert.That(duplicateResult, Is.False);
            AssertPausedInputState();
            yield return null;
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
                    _uiInputSystem,
                    "IsUIActionMapEnabled"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerMovementSystem,
                    "IsRunning"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(_stageSystem, "IsPlaying"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(_stageSystem, "HasEnded"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(_cameraFollow, "IsFollowing"),
                Is.True);
            Assert.That(_stageHud.activeSelf, Is.True);
            Assert.That(_resultPanel.activeSelf, Is.False);
        }

        private void AssertPausedInputState()
        {
            Assert.That(
                GetPropertyValue(_gameSystem, "CurrentGameState").ToString(),
                Is.EqualTo("Paused"));
            Assert.That(
                (bool)GetPropertyValue(
                    _runtimeDataSystem,
                    "HasRuntimeData"),
                Is.True);
            Assert.That(
                (bool)GetPropertyValue(
                    _playerInputSystem,
                    "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(
                (bool)GetPropertyValue(
                    _uiInputSystem,
                    "IsUIActionMapEnabled"),
                Is.True);

            object runtimeData = GetPropertyValue(
                _runtimeDataSystem,
                "RuntimeData");
            Assert.That(
                GetObjectProperty<object>(runtimeData, "GameState").ToString(),
                Is.EqualTo("Paused"));
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

        private T GetObjectProperty<T>(
            object target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private object InvokePublicMethod(
            MonoBehaviour targetBehaviour,
            string methodName)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(targetBehaviour, null);
        }

        private void SetPrivateField(
            MonoBehaviour targetBehaviour,
            string fieldName,
            object value)
        {
            FieldInfo field = targetBehaviour.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(targetBehaviour, value);
        }
    }
}
