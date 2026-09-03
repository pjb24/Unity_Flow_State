using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class ResultMenuIntegrationTests
    {
        private const string SceneName = "SampleScene";

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _uiInputSystem;
        private MonoBehaviour _uiManagementSystem;
        private MonoBehaviour _runtimeDataSystem;

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

            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _uiInputSystem = FindRequiredBehaviour(
                "UIInputSystem",
                "UIInputSystem");
            _uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");

            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;
        }

        [UnityTest]
        public IEnumerator ResultMenu_DefaultsToRetryAndMovesVertically()
        {
            AssertCurrentSelection("Retry");
            AssertSelectedGameObject("RetryButton");

            InvokePublicMethod(
                _uiManagementSystem,
                "MoveResultMenuSelection",
                -1.0f);
            AssertCurrentSelection("Quit");
            AssertSelectedGameObject("QuitButton");

            InvokePublicMethod(
                _uiManagementSystem,
                "MoveResultMenuSelection",
                1.0f);
            AssertCurrentSelection("Retry");
            AssertSelectedGameObject("RetryButton");

            yield return null;
        }

        [UnityTest]
        public IEnumerator ResultMenu_PointerSelectsRetryAndQuit()
        {
            GameObject quitButton = FindResultMenuButton("QuitButton");
            Vector2 quitPosition = RectTransformUtility.WorldToScreenPoint(
                null,
                quitButton.transform.position);

            object quitResult = InvokePublicMethod(
                _uiManagementSystem,
                "TrySetResultMenuSelectionAtPointer",
                quitPosition);

            Assert.That((bool)quitResult, Is.True);
            AssertCurrentSelection("Quit");
            AssertSelectedGameObject("QuitButton");

            GameObject retryButton = FindResultMenuButton("RetryButton");
            Vector2 retryPosition = RectTransformUtility.WorldToScreenPoint(
                null,
                retryButton.transform.position);

            object retryResult = InvokePublicMethod(
                _uiManagementSystem,
                "TrySetResultMenuSelectionAtPointer",
                retryPosition);

            Assert.That((bool)retryResult, Is.True);
            AssertCurrentSelection("Retry");
            AssertSelectedGameObject("RetryButton");

            yield return null;
        }

        [UnityTest]
        public IEnumerator ResultMenu_CancelInput_DoesNotExecuteSelection()
        {
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);

            yield return null;

            AssertGameState("Ended");
            AssertCurrentSelection("Retry");
            Assert.That(
                GetInputStateProperty<bool>("IsCancelPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator ResultMenu_SubmitRetry_StartsNewStageOnce()
        {
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);

            yield return null;

            AssertGameState("Playing");
            Assert.That(
                GetInputStateProperty<bool>("IsSubmitPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator ResultMenu_MouseClickRetry_StartsNewStageOnce()
        {
            GameObject retryButton = FindResultMenuButton("RetryButton");
            Vector2 retryPosition = RectTransformUtility.WorldToScreenPoint(
                null,
                retryButton.transform.position);
            SetPrivateField(
                _uiInputSystem,
                "_pointerPosition",
                retryPosition);
            SetPrivateField(_uiInputSystem, "_isPointChanged", true);
            SetPrivateField(_uiInputSystem, "_isClickPressed", true);

            yield return null;

            AssertGameState("Playing");
            Assert.That(
                GetInputStateProperty<bool>("IsClickPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator InfiniteResult_KeyboardSubmitRetry_StartsInfiniteRun()
        {
            RestartEndedInMode(E_GameMode.Infinite);
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);

            yield return null;

            AssertGameState("Playing");
            AssertRuntimeMode(E_GameMode.Infinite);
            Assert.That(
                GetInputStateProperty<bool>("IsSubmitPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator InfiniteResult_MouseClickRetry_StartsInfiniteRun()
        {
            RestartEndedInMode(E_GameMode.Infinite);
            GameObject retryButton = FindResultMenuButton("RetryButton");
            Vector2 retryPosition = RectTransformUtility.WorldToScreenPoint(
                null,
                retryButton.transform.position);
            SetPrivateField(_uiInputSystem, "_pointerPosition", retryPosition);
            SetPrivateField(_uiInputSystem, "_isPointChanged", true);
            SetPrivateField(_uiInputSystem, "_isClickPressed", true);

            yield return null;

            AssertGameState("Playing");
            AssertRuntimeMode(E_GameMode.Infinite);
            Assert.That(
                GetInputStateProperty<bool>("IsClickPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator ResultClickAndSubmit_RetryExecutesOnlyOnce()
        {
            GameObject retryButton = FindResultMenuButton("RetryButton");
            Vector2 retryPosition = RectTransformUtility.WorldToScreenPoint(
                null,
                retryButton.transform.position);
            SetPrivateField(_uiInputSystem, "_pointerPosition", retryPosition);
            SetPrivateField(_uiInputSystem, "_isPointChanged", true);
            SetPrivateField(_uiInputSystem, "_isClickPressed", true);
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);

            yield return null;

            object restartedRuntimeData = GetPropertyValue(
                _runtimeDataSystem,
                "RuntimeData");
            AssertGameState("Playing");

            yield return null;

            AssertGameState("Playing");
            Assert.That(
                GetPropertyValue(_runtimeDataSystem, "RuntimeData"),
                Is.SameAs(restartedRuntimeData));
            Assert.That(
                GetInputStateProperty<bool>("IsClickPressed"),
                Is.False);
            Assert.That(
                GetInputStateProperty<bool>("IsSubmitPressed"),
                Is.False);
        }

        private void RestartEndedInMode(E_GameMode gameMode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(gameMode);
            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _uiInputSystem = FindRequiredBehaviour(
                "UIInputSystem",
                "UIInputSystem");
            _uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            InvokePublicMethod(_gameSystem, "EndGame");
        }

        private void AssertRuntimeMode(E_GameMode expectedMode)
        {
            object runtimeData = GetPropertyValue(
                _runtimeDataSystem,
                "RuntimeData");
            Assert.That(runtimeData, Is.Not.Null);
            Assert.That(
                GetObjectPropertyValue(runtimeData, "GameMode"),
                Is.EqualTo(expectedMode));
        }

        private object GetObjectPropertyValue(
            object target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private void AssertCurrentSelection(string expectedSelection)
        {
            object selection = GetPropertyValue(
                _uiManagementSystem,
                "CurrentResultMenuSelection");

            Assert.That(selection.ToString(), Is.EqualTo(expectedSelection));
        }

        private void AssertGameState(string expectedState)
        {
            object gameState = GetPropertyValue(
                _gameSystem,
                "CurrentGameState");

            Assert.That(gameState.ToString(), Is.EqualTo(expectedState));
        }

        private void AssertSelectedGameObject(string expectedName)
        {
            Assert.That(EventSystem.current, Is.Not.Null);
            Assert.That(
                EventSystem.current.currentSelectedGameObject,
                Is.Not.Null);
            Assert.That(
                EventSystem.current.currentSelectedGameObject.name,
                Is.EqualTo(expectedName));
        }

        private T GetInputStateProperty<T>(string propertyName)
        {
            object inputState = InvokePublicMethod(
                _uiInputSystem,
                "GetInputState");
            PropertyInfo property = inputState.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(inputState);
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

        private GameObject FindResultMenuButton(string buttonName)
        {
            GameObject resultPanel = FindSceneGameObject("ResultPanel");

            foreach (Transform child in
                     resultPanel.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == buttonName)
                {
                    return child.gameObject;
                }
            }

            Assert.Fail($"{buttonName} was not found under ResultPanel.");
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

            Assert.Fail($"{typeName} was not found on {gameObjectName}.");
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

        private object InvokePublicMethod(
            MonoBehaviour targetBehaviour,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(targetBehaviour, arguments);
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
