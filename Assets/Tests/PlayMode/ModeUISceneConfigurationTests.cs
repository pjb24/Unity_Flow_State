using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class ModeUISceneConfigurationTests
    {
        private const string SceneName = "SampleScene";

        [UnityTest]
        public IEnumerator ModeUI_HasRequiredHierarchyComponentsAndReferences()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            GameObject uiRoot = FindUniqueSceneObject("UIRoot");
            GameObject stageHud = FindDirectChild(uiRoot, "StageHUD");
            GameObject infiniteHud = FindDirectChild(uiRoot, "InfiniteHUD");
            GameObject resultPanel = FindDirectChild(uiRoot, "ResultPanel");
            GameObject pausePanel = FindDirectChild(uiRoot, "PausePanel");

            GameObject stageCanvas = RequireDirectCanvas(stageHud);
            Image stageHudBackground = FindDirectComponent<Image>(
                stageCanvas,
                "Image");
            TMP_Text stageCollectibleScoreText =
                FindDirectComponent<TMP_Text>(
                    stageHudBackground.gameObject,
                    "StageCollectibleScoreText");
            GameObject infiniteCanvas = RequireDirectCanvas(infiniteHud);
            Image infiniteHudBackground = FindDirectComponent<Image>(
                infiniteCanvas,
                "Image");
            TMP_Text distanceText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "DistanceText");
            TMP_Text scoreText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "ScoreText");
            TMP_Text infiniteCollectibleScoreText =
                FindDirectComponent<TMP_Text>(
                    infiniteHudBackground.gameObject,
                    "InfiniteCollectibleScoreText");
            TMP_Text infiniteTotalScoreText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "InfiniteTotalScoreText");

            GameObject resultCanvas = RequireDirectCanvas(resultPanel);
            GameObject resultContainer = FindDirectChild(resultCanvas, "Panel");
            GameObject stageResultContent = FindDirectChild(
                resultContainer,
                "StageResultContent");
            GameObject infiniteResultContent = FindDirectChild(
                resultContainer,
                "InfiniteResultContent");
            TMP_Text clearTimeText = FindUniqueDescendantComponent<TMP_Text>(
                stageResultContent,
                "ClearTimeText");
            TMP_Text resultStatusText =
                FindUniqueDescendantComponent<TMP_Text>(
                    stageResultContent,
                    "StageResultStatusText");
            TMP_Text stageResultCollectibleScoreText =
                FindUniqueDescendantComponent<TMP_Text>(
                    stageResultContent,
                    "StageResultCollectibleScoreText");
            Image infiniteResultBackground = FindDirectComponent<Image>(
                infiniteResultContent,
                "Infinite Result Image");
            TMP_Text finalDistanceText = FindDirectComponent<TMP_Text>(
                infiniteResultBackground.gameObject,
                "FinalDistanceText");
            TMP_Text finalScoreText = FindDirectComponent<TMP_Text>(
                infiniteResultBackground.gameObject,
                "FinalScoreText");
            TMP_Text infiniteResultCollectibleScoreText =
                FindDirectComponent<TMP_Text>(
                    infiniteResultBackground.gameObject,
                    "InfiniteResultCollectibleScoreText");
            TMP_Text infiniteResultTotalScoreText =
                FindDirectComponent<TMP_Text>(
                    infiniteResultBackground.gameObject,
                    "InfiniteResultTotalScoreText");
            Button resultRetryButton = FindDirectComponent<Button>(
                resultContainer,
                "RetryButton");
            Button resultQuitButton = FindDirectComponent<Button>(
                resultContainer,
                "QuitButton");

            GameObject pauseCanvas = RequireDirectCanvas(pausePanel);
            GameObject pauseContainer = FindDirectChild(pauseCanvas, "Panel");
            FindUniqueDescendantComponent<TMP_Text>(
                pauseContainer,
                "Pause Title Text");
            Button pauseResumeButton = FindDirectComponent<Button>(
                pauseContainer,
                "ResumeButton");
            Button pauseRetryButton = FindDirectComponent<Button>(
                pauseContainer,
                "RetryButton");
            Button pauseQuitButton = FindDirectComponent<Button>(
                pauseContainer,
                "QuitButton");

            MonoBehaviour uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            AssertSerializedReference(uiManagementSystem, "_stageHud", stageHud);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteHud",
                infiniteHud);
            AssertSerializedReference(
                uiManagementSystem,
                "_resultPanel",
                resultPanel);
            AssertSerializedReference(
                uiManagementSystem,
                "_pausePanel",
                pausePanel);
            AssertSerializedReference(
                uiManagementSystem,
                "_stageResultContent",
                stageResultContent);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteResultContent",
                infiniteResultContent);
            AssertSerializedReference(
                uiManagementSystem,
                "_stageCollectibleScoreText",
                stageCollectibleScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_resultStatusText",
                resultStatusText);
            AssertSerializedReference(
                uiManagementSystem,
                "_clearTimeText",
                clearTimeText);
            AssertSerializedReference(
                uiManagementSystem,
                "_stageResultCollectibleScoreText",
                stageResultCollectibleScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_distanceText",
                distanceText);
            AssertSerializedReference(
                uiManagementSystem,
                "_scoreText",
                scoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteCollectibleScoreText",
                infiniteCollectibleScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteTotalScoreText",
                infiniteTotalScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_finalDistanceText",
                finalDistanceText);
            AssertSerializedReference(
                uiManagementSystem,
                "_finalScoreText",
                finalScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteResultCollectibleScoreText",
                infiniteResultCollectibleScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteResultTotalScoreText",
                infiniteResultTotalScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_retryButton",
                resultRetryButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_quitButton",
                resultQuitButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseResumeButton",
                pauseResumeButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseRetryButton",
                pauseRetryButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseQuitButton",
                pauseQuitButton);

            EventSystem[] eventSystems = FindSceneComponents<EventSystem>();
            Assert.That(eventSystems, Has.Length.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator ModeUI_MapsPlayingPauseAndResultContent()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            MonoBehaviour uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            GameObject stageHud = FindUniqueSceneObject("StageHUD");
            GameObject infiniteHud = FindUniqueSceneObject("InfiniteHUD");
            GameObject resultPanel = FindUniqueSceneObject("ResultPanel");
            GameObject pausePanel = FindUniqueSceneObject("PausePanel");
            GameObject stageResultContent =
                FindUniqueSceneObject("StageResultContent");
            GameObject infiniteResultContent =
                FindUniqueSceneObject("InfiniteResultContent");

            GameRuntimeData stageRuntimeData = new GameRuntimeData();
            stageRuntimeData.Initialize(E_GameMode.Stage);
            InvokePublicMethod(
                uiManagementSystem,
                "Initialize",
                stageRuntimeData);
            SetUIState(
                uiManagementSystem,
                E_GameState.Playing,
                E_UIState.StageHud);
            AssertVisibility(
                stageHud,
                infiniteHud,
                resultPanel,
                pausePanel,
                true,
                false,
                false,
                false);

            SetUIState(
                uiManagementSystem,
                E_GameState.Paused,
                E_UIState.Pause);
            AssertVisibility(
                stageHud,
                infiniteHud,
                resultPanel,
                pausePanel,
                true,
                false,
                false,
                true);

            ResultData stageResultData = new ResultData(
                E_StageResultType.Cleared,
                12.345,
                30);
            Assert.That(
                (bool)InvokePublicMethod(
                    uiManagementSystem,
                    "SetResultData",
                    stageResultData),
                Is.True);
            SetUIState(
                uiManagementSystem,
                E_GameState.Ended,
                E_UIState.Result);
            AssertVisibility(
                stageHud,
                infiniteHud,
                resultPanel,
                pausePanel,
                true,
                false,
                true,
                false);
            Assert.That(stageResultContent.activeSelf, Is.True);
            Assert.That(infiniteResultContent.activeSelf, Is.False);
            AssertStageResultText(uiManagementSystem, stageResultData);

            GameRuntimeData infiniteRuntimeData = new GameRuntimeData();
            infiniteRuntimeData.Initialize(E_GameMode.Infinite);
            InvokePublicMethod(
                uiManagementSystem,
                "Initialize",
                infiniteRuntimeData);
            SetUIState(
                uiManagementSystem,
                E_GameState.Playing,
                E_UIState.StageHud);
            AssertVisibility(
                stageHud,
                infiniteHud,
                resultPanel,
                pausePanel,
                false,
                true,
                false,
                false);

            SetUIState(
                uiManagementSystem,
                E_GameState.Paused,
                E_UIState.Pause);
            AssertVisibility(
                stageHud,
                infiniteHud,
                resultPanel,
                pausePanel,
                false,
                true,
                false,
                true);

            ResultData infiniteResultData = new ResultData(
                12.9f,
                120,
                30,
                150);
            Assert.That(
                (bool)InvokePublicMethod(
                    uiManagementSystem,
                    "SetResultData",
                    infiniteResultData),
                Is.True);
            SetUIState(
                uiManagementSystem,
                E_GameState.Ended,
                E_UIState.Result);
            AssertVisibility(
                stageHud,
                infiniteHud,
                resultPanel,
                pausePanel,
                false,
                true,
                true,
                false);
            Assert.That(stageResultContent.activeSelf, Is.False);
            Assert.That(infiniteResultContent.activeSelf, Is.True);
            AssertInfiniteResultText(
                uiManagementSystem,
                infiniteResultData);
        }

        private GameObject RequireDirectCanvas(GameObject parent)
        {
            GameObject canvasObject = FindDirectChild(parent, "Canvas");
            Assert.That(canvasObject.GetComponent<Canvas>(), Is.Not.Null);
            Assert.That(canvasObject.GetComponent<CanvasScaler>(), Is.Not.Null);
            Assert.That(
                canvasObject.GetComponent<GraphicRaycaster>(),
                Is.Not.Null);
            return canvasObject;
        }

        private void SetUIState(
            MonoBehaviour uiManagementSystem,
            E_GameState gameState,
            E_UIState uiState)
        {
            InvokePublicMethod(
                uiManagementSystem,
                "SetGameState",
                gameState);
            InvokePublicMethod(
                uiManagementSystem,
                "SetUIState",
                uiState);
        }

        private void AssertVisibility(
            GameObject stageHud,
            GameObject infiniteHud,
            GameObject resultPanel,
            GameObject pausePanel,
            bool isStageHudVisible,
            bool isInfiniteHudVisible,
            bool isResultPanelVisible,
            bool isPausePanelVisible)
        {
            Assert.That(stageHud.activeSelf, Is.EqualTo(isStageHudVisible));
            Assert.That(
                infiniteHud.activeSelf,
                Is.EqualTo(isInfiniteHudVisible));
            Assert.That(
                resultPanel.activeSelf,
                Is.EqualTo(isResultPanelVisible));
            Assert.That(
                pausePanel.activeSelf,
                Is.EqualTo(isPausePanelVisible));
        }

        private void AssertStageResultText(
            MonoBehaviour uiManagementSystem,
            ResultData resultData)
        {
            Assert.That(
                ResultTextFormatter.TryFormatStageResult(
                    resultData,
                    out string expectedStatus,
                    out string expectedElapsedTime,
                    out string expectedCollectibleScore),
                Is.True);
            Assert.That(
                GetSerializedText(uiManagementSystem, "_resultStatusText"),
                Is.EqualTo(expectedStatus));
            Assert.That(
                GetSerializedText(uiManagementSystem, "_clearTimeText"),
                Is.EqualTo(expectedElapsedTime));
            Assert.That(
                GetSerializedText(
                    uiManagementSystem,
                    "_stageResultCollectibleScoreText"),
                Is.EqualTo(expectedCollectibleScore));
        }

        private void AssertInfiniteResultText(
            MonoBehaviour uiManagementSystem,
            ResultData resultData)
        {
            Assert.That(
                ResultTextFormatter.TryFormatInfiniteResult(
                    resultData,
                    out string expectedDistance,
                    out string expectedDistanceScore,
                    out string expectedCollectibleScore,
                    out string expectedTotalScore),
                Is.True);
            Assert.That(
                GetSerializedText(uiManagementSystem, "_finalDistanceText"),
                Is.EqualTo(expectedDistance));
            Assert.That(
                GetSerializedText(uiManagementSystem, "_finalScoreText"),
                Is.EqualTo(expectedDistanceScore));
            Assert.That(
                GetSerializedText(
                    uiManagementSystem,
                    "_infiniteResultCollectibleScoreText"),
                Is.EqualTo(expectedCollectibleScore));
            Assert.That(
                GetSerializedText(
                    uiManagementSystem,
                    "_infiniteResultTotalScoreText"),
                Is.EqualTo(expectedTotalScore));
        }

        private string GetSerializedText(
            MonoBehaviour uiManagementSystem,
            string fieldName)
        {
            FieldInfo field = uiManagementSystem.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            TMP_Text text = (TMP_Text)field.GetValue(uiManagementSystem);
            Assert.That(text, Is.Not.Null);
            return text.text;
        }

        private object InvokePublicMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = null;

            foreach (MethodInfo candidate in target.GetType().GetMethods(
                         BindingFlags.Instance | BindingFlags.Public))
            {
                if (candidate.Name == methodName &&
                    candidate.GetParameters().Length == arguments.Length)
                {
                    method = candidate;
                    break;
                }
            }

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }

        private GameObject FindUniqueSceneObject(string objectName)
        {
            GameObject[] matches = FindSceneObjects(objectName);
            Assert.That(
                matches,
                Has.Length.EqualTo(1),
                $"Scene must contain exactly one {objectName}.");
            return matches[0];
        }

        private GameObject FindDirectChild(GameObject parent, string childName)
        {
            GameObject match = null;

            foreach (Transform child in parent.transform)
            {
                if (child.name != childName)
                {
                    continue;
                }

                Assert.That(
                    match,
                    Is.Null,
                    $"{parent.name} has duplicate direct child {childName}.");
                match = child.gameObject;
            }

            Assert.That(
                match,
                Is.Not.Null,
                $"{childName} must be a direct child of {parent.name}.");
            return match;
        }

        private T FindDirectComponent<T>(GameObject parent, string childName)
            where T : Component
        {
            GameObject child = FindDirectChild(parent, childName);
            T component = child.GetComponent<T>();
            Assert.That(
                component,
                Is.Not.Null,
                $"{childName} must have {typeof(T).Name}.");
            return component;
        }

        private T FindUniqueDescendantComponent<T>(
            GameObject parent,
            string descendantName)
            where T : Component
        {
            T match = null;

            foreach (T component in parent.GetComponentsInChildren<T>(true))
            {
                if (component.name != descendantName)
                {
                    continue;
                }

                Assert.That(
                    match,
                    Is.Null,
                    $"{parent.name} has duplicate {descendantName}.");
                match = component;
            }

            Assert.That(match, Is.Not.Null);
            return match;
        }

        private GameObject[] FindSceneObjects(string objectName)
        {
            System.Collections.Generic.List<GameObject> matches =
                new System.Collections.Generic.List<GameObject>();

            foreach (GameObject gameObject in
                     Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == objectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    matches.Add(gameObject);
                }
            }

            return matches.ToArray();
        }

        private T[] FindSceneComponents<T>() where T : Component
        {
            System.Collections.Generic.List<T> matches =
                new System.Collections.Generic.List<T>();

            foreach (T component in Resources.FindObjectsOfTypeAll<T>())
            {
                if (component.gameObject.scene.IsValid() &&
                    component.gameObject.scene.isLoaded)
                {
                    matches.Add(component);
                }
            }

            return matches.ToArray();
        }

        private MonoBehaviour FindRequiredBehaviour(
            string objectName,
            string typeName)
        {
            GameObject target = FindUniqueSceneObject(objectName);

            foreach (MonoBehaviour behaviour in
                     target.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }

        private void AssertSerializedReference<T>(
            MonoBehaviour target,
            string fieldName,
            T expected)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            Assert.That(field.GetValue(target), Is.EqualTo(expected));
        }
    }
}
