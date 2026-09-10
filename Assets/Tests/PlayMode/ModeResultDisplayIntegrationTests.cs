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
    public class ModeResultDisplayIntegrationTests
    {
        private readonly List<GameObject> _createdObjects =
            new List<GameObject>();

        private MonoBehaviour _uiManagementSystem;
        private TMP_Text _resultStatusText;
        private TMP_Text _clearTimeText;
        private TMP_Text _stageResultCollectibleScoreText;
        private TMP_Text _finalDistanceText;
        private TMP_Text _finalScoreText;
        private TMP_Text _infiniteResultCollectibleScoreText;
        private TMP_Text _infiniteResultTotalScoreText;

        [SetUp]
        public void SetUp()
        {
            CreateObject("ModeResultTests.EventSystem")
                .AddComponent<EventSystem>();

            GameObject systemObject = CreateObject(
                "ModeResultTests.UIManagementSystem");
            Type systemType = FindType(
                "FlowState.Runtime.Systems.UIManagementSystem");
            _uiManagementSystem =
                (MonoBehaviour)systemObject.AddComponent(systemType);

            SetPrivateField("_stageHud", CreateObject("StageHUD"));
            SetPrivateField("_infiniteHud", CreateObject("InfiniteHUD"));
            SetPrivateField("_resultPanel", CreateObject("ResultPanel"));
            SetPrivateField("_pausePanel", CreateObject("PausePanel"));
            SetPrivateField(
                "_stageResultContent",
                CreateObject("StageResultContent"));
            SetPrivateField(
                "_infiniteResultContent",
                CreateObject("InfiniteResultContent"));

            _resultStatusText = CreateText("ResultStatusText");
            _clearTimeText = CreateText("ClearTimeText");
            _stageResultCollectibleScoreText = CreateText(
                "StageResultCollectibleScoreText");
            _finalDistanceText = CreateText("FinalDistanceText");
            _finalScoreText = CreateText("FinalScoreText");
            _infiniteResultCollectibleScoreText = CreateText(
                "InfiniteResultCollectibleScoreText");
            _infiniteResultTotalScoreText = CreateText(
                "InfiniteResultTotalScoreText");
            SetPrivateField("_resultStatusText", _resultStatusText);
            SetPrivateField("_clearTimeText", _clearTimeText);
            SetPrivateField(
                "_stageResultCollectibleScoreText",
                _stageResultCollectibleScoreText);
            SetPrivateField("_distanceText", CreateText("DistanceText"));
            SetPrivateField("_scoreText", CreateText("ScoreText"));
            SetPrivateField("_finalDistanceText", _finalDistanceText);
            SetPrivateField("_finalScoreText", _finalScoreText);
            SetPrivateField(
                "_infiniteResultCollectibleScoreText",
                _infiniteResultCollectibleScoreText);
            SetPrivateField(
                "_infiniteResultTotalScoreText",
                _infiniteResultTotalScoreText);
            SetPrivateField("_retryButton", CreateButton("RetryButton"));
            SetPrivateField("_quitButton", CreateButton("QuitButton"));
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

        [Test]
        public void StageResult_DisplaysOnlyStageResultFields()
        {
            Initialize(E_GameMode.Stage);

            bool didSetResult = SetResultData(new ResultData(
                E_StageResultType.Cleared,
                12.3456,
                30));

            Assert.That(didSetResult, Is.True);
            Assert.That(_resultStatusText.text, Is.EqualTo("Result: Stage Clear"));
            Assert.That(_clearTimeText.text, Is.EqualTo("Clear Time: 12.346 s"));
            Assert.That(
                _stageResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 30"));
            Assert.That(_finalDistanceText.text, Is.Empty);
            Assert.That(_finalScoreText.text, Is.Empty);
            Assert.That(_infiniteResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_infiniteResultTotalScoreText.text, Is.Empty);
        }

        [Test]
        public void FellStageResult_DisplaysFailureStatusAndRunTime()
        {
            Initialize(E_GameMode.Stage);

            bool didSetResult = SetResultData(new ResultData(
                E_StageResultType.Fell,
                8.25,
                20));

            Assert.That(didSetResult, Is.True);
            Assert.That(_resultStatusText.text, Is.EqualTo("Result: Stage Failed"));
            Assert.That(_clearTimeText.text, Is.EqualTo("Run Time: 8.250 s"));
            Assert.That(
                _stageResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 20"));
        }

        [Test]
        public void InfiniteResult_DisplaysOnlyInfiniteResultFields()
        {
            Initialize(E_GameMode.Infinite);

            bool didSetResult = SetResultData(
                new ResultData(12.999f, 129, 30, 159));

            Assert.That(didSetResult, Is.True);
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
            Assert.That(
                _finalDistanceText.text,
                Is.EqualTo("Final Distance: 12"));
            Assert.That(_finalScoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(
                _infiniteResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 30"));
            Assert.That(
                _infiniteResultTotalScoreText.text,
                Is.EqualTo("Total Score: 159"));
        }

        [Test]
        public void Initialize_AfterResult_ClearsPreviousResultText()
        {
            Initialize(E_GameMode.Infinite);
            SetResultData(new ResultData(12.999f, 129, 30, 159));

            Initialize(E_GameMode.Infinite);

            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_finalDistanceText.text, Is.Empty);
            Assert.That(_finalScoreText.text, Is.Empty);
            Assert.That(_infiniteResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_infiniteResultTotalScoreText.text, Is.Empty);
        }

        [Test]
        public void ConsecutiveInfiniteRuns_DisplayIndependentResults()
        {
            Initialize(E_GameMode.Infinite);
            SetResultData(new ResultData(12.999f, 129, 30, 159));

            Initialize(E_GameMode.Infinite);
            SetResultData(new ResultData(20.999f, 209, 40, 249));

            Assert.That(
                _finalDistanceText.text,
                Is.EqualTo("Final Distance: 20"));
            Assert.That(_finalScoreText.text, Is.EqualTo("Distance Score: 209"));
            Assert.That(
                _infiniteResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 40"));
            Assert.That(
                _infiniteResultTotalScoreText.text,
                Is.EqualTo("Total Score: 249"));
        }

        [Test]
        public void StageRunAfterInfiniteRun_DoesNotKeepInfiniteResultText()
        {
            Initialize(E_GameMode.Infinite);
            SetResultData(new ResultData(12.999f, 129, 30, 159));

            Initialize(E_GameMode.Stage);
            SetResultData(new ResultData(
                E_StageResultType.Cleared,
                5.25,
                30));

            Assert.That(_resultStatusText.text, Is.EqualTo("Result: Stage Clear"));
            Assert.That(_clearTimeText.text, Is.EqualTo("Clear Time: 5.250 s"));
            Assert.That(
                _stageResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 30"));
            Assert.That(_finalDistanceText.text, Is.Empty);
            Assert.That(_finalScoreText.text, Is.Empty);
            Assert.That(_infiniteResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_infiniteResultTotalScoreText.text, Is.Empty);
        }

        [UnityTest]
        public IEnumerator ResultMenuSelection_RemainsSharedAcrossModesAndResets()
        {
            Initialize(E_GameMode.Stage);
            SetEndedResultState();
            Assert.That(GetCurrentResultSelection(), Is.EqualTo("Retry"));
            Assert.That(MoveResultSelection(-1.0f), Is.True);
            Assert.That(GetCurrentResultSelection(), Is.EqualTo("Quit"));

            Initialize(E_GameMode.Infinite);
            SetEndedResultState();

            Assert.That(GetCurrentResultSelection(), Is.EqualTo("Retry"));
            yield return null;
        }

        private void Initialize(E_GameMode gameMode)
        {
            GameRuntimeData runtimeData = new GameRuntimeData();
            runtimeData.Initialize(gameMode);
            InvokePublicMethod("SetGameState", E_GameState.Initializing);
            InvokePublicMethod("Initialize", runtimeData);
        }

        private void SetEndedResultState()
        {
            InvokePublicMethod("SetGameState", E_GameState.Ended);
            InvokePublicMethod("SetUIState", E_UIState.Result);
        }

        private bool SetResultData(ResultData resultData)
        {
            return (bool)InvokePublicMethod("SetResultData", resultData);
        }

        private bool MoveResultSelection(float verticalInput)
        {
            return (bool)InvokePublicMethod(
                "MoveResultMenuSelection",
                verticalInput);
        }

        private string GetCurrentResultSelection()
        {
            PropertyInfo property = _uiManagementSystem.GetType().GetProperty(
                "CurrentResultMenuSelection",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(_uiManagementSystem).ToString();
        }

        private TMP_Text CreateText(string objectName)
        {
            return CreateObject(objectName).AddComponent<TextMeshProUGUI>();
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

        private Type FindType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);

                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail($"{fullName} was not found.");
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
