using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class InfiniteHudIntegrationTests
    {
        private readonly List<GameObject> _createdObjects =
            new List<GameObject>();

        private MonoBehaviour _uiManagementSystem;
        private GameRuntimeData _runtimeData;
        private TMP_Text _distanceText;
        private TMP_Text _scoreText;
        private GameObject _stageHud;
        private GameObject _infiniteHud;

        [SetUp]
        public void SetUp()
        {
            GameObject eventSystemObject = CreateObject(
                "InfiniteHudTests.EventSystem");
            eventSystemObject.AddComponent<EventSystem>();

            GameObject systemObject = CreateObject(
                "InfiniteHudTests.UIManagementSystem");
            Type systemType = FindType(
                "FlowState.Runtime.Systems.UIManagementSystem");
            _uiManagementSystem =
                (MonoBehaviour)systemObject.AddComponent(systemType);

            _stageHud = CreateObject("StageHUD");
            _infiniteHud = CreateObject("InfiniteHUD");
            GameObject resultPanel = CreateObject("ResultPanel");
            GameObject pausePanel = CreateObject("PausePanel");
            GameObject stageResultContent = CreateObject("StageResultContent");
            GameObject infiniteResultContent =
                CreateObject("InfiniteResultContent");

            _distanceText = CreateText("DistanceText", _infiniteHud.transform);
            _scoreText = CreateText("ScoreText", _infiniteHud.transform);

            SetPrivateField("_stageHud", _stageHud);
            SetPrivateField("_infiniteHud", _infiniteHud);
            SetPrivateField("_resultPanel", resultPanel);
            SetPrivateField("_pausePanel", pausePanel);
            SetPrivateField("_stageResultContent", stageResultContent);
            SetPrivateField("_infiniteResultContent", infiniteResultContent);
            SetPrivateField("_distanceText", _distanceText);
            SetPrivateField("_scoreText", _scoreText);
            SetPrivateField("_retryButton", CreateButton("ResultRetryButton"));
            SetPrivateField("_quitButton", CreateButton("ResultQuitButton"));
            SetPrivateField("_pauseResumeButton", CreateButton("ResumeButton"));
            SetPrivateField("_pauseRetryButton", CreateButton("PauseRetryButton"));
            SetPrivateField("_pauseQuitButton", CreateButton("PauseQuitButton"));
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = _createdObjects.Count - 1; index >= 0; index--)
            {
                if (_createdObjects[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(_createdObjects[index]);
                }
            }

            _createdObjects.Clear();
        }

        [UnityTest]
        public IEnumerator InfiniteRunStart_DisplaysZeroDistanceAndScore()
        {
            StartRun(E_GameMode.Infinite);

            yield return null;

            Assert.That(_infiniteHud.activeSelf, Is.True);
            Assert.That(_stageHud.activeSelf, Is.False);
            Assert.That(_distanceText.text, Is.EqualTo("Distance: 0"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 0"));
        }

        [UnityTest]
        public IEnumerator RuntimeDataUpdate_UpdatesDisplayedValues()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.TryUpdate(12.999f, 129),
                Is.True);

            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 129"));
        }

        [UnityTest]
        public IEnumerator RejectedBackwardProgress_KeepsMaximumDisplayedValues()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.TryUpdate(12.999f, 129),
                Is.True);
            yield return null;

            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.TryUpdate(8.0f, 80),
                Is.False);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 129"));
        }

        [UnityTest]
        public IEnumerator Pause_FreezesHudUntilResume()
        {
            StartRun(E_GameMode.Infinite);
            _runtimeData.InfiniteModeRuntimeData.TryUpdate(12.999f, 129);
            yield return null;

            SetGameState(E_GameState.Paused);
            SetUIState(E_UIState.Pause);
            _runtimeData.InfiniteModeRuntimeData.TryUpdate(20.999f, 209);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 129"));

            SetGameState(E_GameState.Playing);
            SetUIState(E_UIState.StageHud);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 20"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 209"));
        }

        [UnityTest]
        public IEnumerator EndingAndEnded_KeepLastDisplayedValues()
        {
            StartRun(E_GameMode.Infinite);
            _runtimeData.InfiniteModeRuntimeData.TryUpdate(12.999f, 129);
            yield return null;

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);
            _runtimeData.InfiniteModeRuntimeData.Clear();
            SetGameState(E_GameState.Ended);
            yield return null;

            Assert.That(_infiniteHud.activeSelf, Is.True);
            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 129"));
        }

        [UnityTest]
        public IEnumerator StageMode_DoesNotActivateOrUpdateInfiniteHud()
        {
            StartRun(E_GameMode.Stage);

            yield return null;

            Assert.That(_stageHud.activeSelf, Is.True);
            Assert.That(_infiniteHud.activeSelf, Is.False);
            Assert.That(_distanceText.text, Is.EqualTo("Distance: --"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: --"));
        }

        [UnityTest]
        public IEnumerator Retry_NewRuntimeDataResetsHudToZero()
        {
            StartRun(E_GameMode.Infinite);
            _runtimeData.InfiniteModeRuntimeData.TryUpdate(12.999f, 129);
            yield return null;

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);
            _runtimeData.Clear();
            SetGameState(E_GameState.Ended);

            StartRun(E_GameMode.Infinite);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 0"));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 0"));
        }

        private void StartRun(E_GameMode gameMode)
        {
            _runtimeData = new GameRuntimeData();
            _runtimeData.Initialize(gameMode);
            SetGameState(E_GameState.Initializing);
            InvokePublicMethod("Initialize", _runtimeData);
            SetUIState(E_UIState.None);
            SetGameState(E_GameState.Ready);
            SetUIState(E_UIState.StageHud);
            SetGameState(E_GameState.Playing);
        }

        private void SetGameState(E_GameState gameState)
        {
            _runtimeData.SetGameState(gameState);
            InvokePublicMethod("SetGameState", gameState);
        }

        private void SetUIState(E_UIState uiState)
        {
            _runtimeData.SetUIState(uiState);
            InvokePublicMethod("SetUIState", uiState);
        }

        private TMP_Text CreateText(string objectName, Transform parent)
        {
            GameObject textObject = CreateObject(objectName);
            textObject.transform.SetParent(parent);
            return textObject.AddComponent<TextMeshProUGUI>();
        }

        private Button CreateButton(string objectName)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button));
            _createdObjects.Add(buttonObject);
            return buttonObject.GetComponent<Button>();
        }

        private GameObject CreateObject(string objectName)
        {
            GameObject createdObject = new GameObject(objectName);
            _createdObjects.Add(createdObject);
            return createdObject;
        }

        private Type FindType(string fullTypeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullTypeName);

                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail($"{fullTypeName} was not found.");
            return null;
        }

        private void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = _uiManagementSystem.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(_uiManagementSystem, value);
        }

        private object InvokePublicMethod(
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = _uiManagementSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(_uiManagementSystem, arguments);
        }
    }
}
