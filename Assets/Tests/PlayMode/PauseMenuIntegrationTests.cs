using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class PauseMenuIntegrationTests
    {
        private const string SceneName = "SampleScene";

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _uiInputSystem;
        private MonoBehaviour _uiManagementSystem;
        private MonoBehaviour _runtimeDataSystem;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);
            while (!operation.isDone)
            {
                yield return null;
            }

            yield return null;
            RestartInMode(E_GameMode.Stage);
        }

        [UnityTest]
        public IEnumerator Cancel_ResumesSameRunAndClosesPausePanel()
        {
            object runtimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);

            yield return null;

            AssertState(E_GameState.Playing, E_UIState.StageHud);
            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
        }

        [UnityTest]
        public IEnumerator RetryButton_StartsOneIndependentRun()
        {
            object previousRuntimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            FindPauseButton("RetryButton").onClick.Invoke();

            yield return null;
            yield return null;

            AssertState(E_GameState.Playing, E_UIState.StageHud);
            Assert.That(GetRuntimeData(), Is.Not.SameAs(previousRuntimeData));
        }

        [UnityTest]
        public IEnumerator RetryButtonClick_MatchesDirectRetry()
        {
            object previousRuntimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            yield return null;

            FindPauseButton("RetryButton").onClick.Invoke();

            AssertState(E_GameState.Playing, E_UIState.StageHud);
            Assert.That(GetRuntimeData(), Is.Not.SameAs(previousRuntimeData));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PauseMainMenu_OpensConfirmationWithoutQuitting()
        {
            FakeApplicationQuitService quitService = InjectFakeQuitService();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            FindPauseButton("MainMenuButton").onClick.Invoke();

            yield return null;

            Assert.That(quitService.RequestCount, Is.Zero);
            Assert.That(GetProperty<object>(
                _gameSystem,
                "CurrentNavigationScreen").ToString(),
                Is.EqualTo("PauseMainMenuConfirmation"));
        }

        [UnityTest]
        public IEnumerator ResultMainMenu_ReturnsToMainMenuWithoutQuitting()
        {
            FakeApplicationQuitService quitService = InjectFakeQuitService();
            InvokePublic(_gameSystem, "EndGame");
            FindResultButton("MainMenuButton").onClick.Invoke();

            yield return null;

            Assert.That(quitService.RequestCount, Is.Zero);
            Assert.That(GetProperty<object>(
                _gameSystem,
                "CurrentNavigationScreen").ToString(),
                Is.EqualTo("MainMenu"));
        }

        [UnityTest]
        public IEnumerator InfiniteRetry_ResetsRunMetricsAndFinalization()
        {
            RestartInMode(E_GameMode.Infinite);
            object previousRuntimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            FindPauseButton("RetryButton").onClick.Invoke();

            yield return null;

            object currentRuntimeData = GetRuntimeData();
            object infiniteData = GetObjectProperty<object>(
                currentRuntimeData,
                "InfiniteModeRuntimeData");
            Assert.That(currentRuntimeData, Is.Not.SameAs(previousRuntimeData));
            Assert.That(GetObjectProperty<float>(infiniteData, "CurrentDistance"),
                Is.EqualTo(0.0f));
            Assert.That(GetObjectProperty<int>(infiniteData, "CurrentScore"),
                Is.EqualTo(0));
            Assert.That(GetObjectProperty<bool>(infiniteData, "IsFinalized"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator InfiniteHud_Cancel_OpensPausePanel()
        {
            RestartInMode(E_GameMode.Infinite);
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);

            yield return null;

            AssertState(E_GameState.Paused, E_UIState.Pause);
            Assert.That(GetInputStateBool("IsCancelPressed"), Is.False);
        }

        [UnityTest]
        public IEnumerator PlayingCancelAndSubmit_ConsumesSubmitAtPauseBoundary()
        {
            object runtimeData = GetRuntimeData();
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);

            yield return null;
            yield return null;

            AssertState(E_GameState.Paused, E_UIState.Pause);
            Assert.That(GetRuntimeData(), Is.SameAs(runtimeData));
            Assert.That(GetInputStateBool("IsCancelPressed"), Is.False);
            Assert.That(GetInputStateBool("IsSubmitPressed"), Is.False);
        }

        [UnityTest]
        public IEnumerator PauseClickAndSubmit_RetryExecutesOnlyOnce()
        {
            object previousRuntimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            yield return null;

            FindPauseButton("RetryButton").onClick.Invoke();

            object restartedRuntimeData = GetRuntimeData();
            AssertState(E_GameState.Playing, E_UIState.StageHud);
            Assert.That(restartedRuntimeData, Is.Not.SameAs(previousRuntimeData));

            yield return null;

            AssertState(E_GameState.Playing, E_UIState.StageHud);
            Assert.That(GetRuntimeData(), Is.SameAs(restartedRuntimeData));
        }

        private void RestartInMode(E_GameMode mode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(mode);
            _gameSystem = FindBehaviour("GameSystem", "GameSystem");
            _uiInputSystem = FindBehaviour("UIInputSystem", "UIInputSystem");
            _uiManagementSystem = FindBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            _runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
        }

        private FakeApplicationQuitService InjectFakeQuitService()
        {
            FakeApplicationQuitService service = new FakeApplicationQuitService();
            SetPrivateField(_gameSystem, "_applicationQuitService", service);
            return service;
        }

        private Button FindPauseButton(string buttonName)
        {
            GameObject pausePanel = FindSceneObject("PausePanel");
            foreach (Button button in pausePanel.GetComponentsInChildren<Button>(true))
            {
                if (button.name == buttonName)
                {
                    return button;
                }
            }

            Assert.Fail($"{buttonName} was not found under PausePanel.");
            return null;
        }

        private Button FindResultButton(string buttonName)
        {
            GameObject resultPanel = FindSceneObject("ResultPanel");
            foreach (Button button in resultPanel.GetComponentsInChildren<Button>(true))
            {
                if (button.name == buttonName)
                {
                    return button;
                }
            }

            Assert.Fail($"{buttonName} was not found under ResultPanel.");
            return null;
        }

        private Vector2 GetButtonScreenPosition(Button button)
        {
            Canvas canvas = button.GetComponentInParent<Canvas>();
            Camera eventCamera = canvas != null &&
                                 canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
            return RectTransformUtility.WorldToScreenPoint(
                eventCamera,
                button.transform.position);
        }

        private void AssertState(E_GameState gameState, E_UIState uiState)
        {
            Assert.That(GetProperty<E_GameState>(_gameSystem, "CurrentGameState"),
                Is.EqualTo(gameState));
            Assert.That(GetProperty<E_UIState>(_uiManagementSystem, "CurrentUIState"),
                Is.EqualTo(uiState));
        }

        private bool GetInputStateBool(string propertyName)
        {
            object inputState = InvokePublic(_uiInputSystem, "GetInputState");
            return GetObjectProperty<bool>(inputState, propertyName);
        }

        private object GetRuntimeData()
        {
            return GetProperty<object>(_runtimeDataSystem, "RuntimeData");
        }

        private MonoBehaviour FindBehaviour(string objectName, string typeName)
        {
            GameObject target = FindSceneObject(objectName);
            foreach (MonoBehaviour behaviour in target.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }

        private GameObject FindSceneObject(string objectName)
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == objectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    return gameObject;
                }
            }

            Assert.Fail($"{objectName} was not found.");
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

        private void InvokeNonPublic(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
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

        private void SetPrivateField(MonoBehaviour target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private sealed class FakeApplicationQuitService : IApplicationQuitService
        {
            public int RequestCount { get; private set; }

            public void RequestQuit()
            {
                RequestCount++;
            }
        }
    }
}
