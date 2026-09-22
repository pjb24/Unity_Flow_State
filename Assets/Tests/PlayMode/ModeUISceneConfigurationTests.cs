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
            GameObject momentumHud = FindDirectChild(uiRoot, "MomentumHUD");
            GameObject resultPanel = FindDirectChild(uiRoot, "ResultPanel");
            GameObject pausePanel = FindDirectChild(uiRoot, "PausePanel");
            MonoBehaviour uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");

            GameObject stageCanvas = RequireDirectCanvas(stageHud);
            Image stageHudBackground =
                FindUniqueDirectChildComponent<Image>(stageCanvas);
            TMP_Text stageCollectibleScoreText = GetSerializedValue<TMP_Text>(
                uiManagementSystem,
                "_stageCollectibleScoreText");
            Assert.That(stageCollectibleScoreText, Is.Not.Null);
            Assert.That(
                stageCollectibleScoreText.transform.parent,
                Is.EqualTo(stageHudBackground.transform),
                "Stage collectible text must be a direct child of the Stage HUD panel.");
            Assert.That(stageHudBackground.raycastTarget, Is.False);
            Assert.That(stageCollectibleScoreText.raycastTarget, Is.False);
            Assert.That(
                stageCollectibleScoreText.textWrappingMode,
                Is.EqualTo(TextWrappingModes.NoWrap));
            GameObject infiniteCanvas = RequireDirectCanvas(infiniteHud);
            Image infiniteHudBackground =
                FindUniqueDirectChildComponent<Image>(infiniteCanvas);
            TMP_Text distanceText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "DistanceText");
            TMP_Text scoreText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "ScoreText");
            TMP_Text baseDistanceScoreText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "BaseDistanceScoreText");
            TMP_Text momentumBonusText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "MomentumBonusText");
            TMP_Text infiniteCollectibleScoreText =
                FindDirectComponent<TMP_Text>(
                    infiniteHudBackground.gameObject,
                    "InfiniteCollectibleScoreText");
            TMP_Text infiniteTotalScoreText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "InfiniteTotalScoreText");
            TMP_Text infiniteDifficultyText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "InfiniteDifficultyText");
            AssertInfiniteHudPresentation(
                infiniteHudBackground,
                distanceText,
                baseDistanceScoreText,
                momentumBonusText,
                scoreText,
                infiniteCollectibleScoreText,
                infiniteTotalScoreText,
                infiniteDifficultyText);

            GameObject resultCanvas = RequireDirectCanvas(resultPanel);
            Image resultContainerImage =
                FindUniqueDirectChildComponent<Image>(resultCanvas);
            GameObject resultContainer = resultContainerImage.gameObject;
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
            Image infiniteResultBackground =
                FindUniqueDirectChildComponent<Image>(infiniteResultContent);
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
            TMP_Text infiniteResultBaseDistanceScoreText =
                FindDirectComponent<TMP_Text>(
                    infiniteResultBackground.gameObject,
                    "InfiniteResultBaseDistanceScoreText");
            TMP_Text infiniteResultMomentumBonusText =
                FindDirectComponent<TMP_Text>(
                    infiniteResultBackground.gameObject,
                    "InfiniteResultMomentumBonusText");
            TMP_Text infiniteResultMaximumMomentumText =
                FindDirectComponent<TMP_Text>(
                    infiniteResultBackground.gameObject,
                    "InfiniteResultMaximumMomentumText");
            Button resultRetryButton = FindDirectComponent<Button>(
                resultContainer,
                "RetryButton");
            Button resultMainMenuButton = FindDirectComponent<Button>(
                resultContainer,
                "MainMenuButton");
            AssertStageResultPresentation(
                resultContainerImage,
                stageResultContent,
                resultStatusText,
                clearTimeText,
                stageResultCollectibleScoreText,
                resultRetryButton,
                resultMainMenuButton);
            AssertInfiniteResultPresentation(
                infiniteResultContent,
                finalDistanceText,
                infiniteResultBaseDistanceScoreText,
                infiniteResultMomentumBonusText,
                finalScoreText,
                infiniteResultCollectibleScoreText,
                infiniteResultTotalScoreText,
                infiniteResultMaximumMomentumText);

            GameObject pauseCanvas = RequireDirectCanvas(pausePanel);
            Image pauseContainerImage =
                FindUniqueDirectChildComponent<Image>(pauseCanvas);
            GameObject pauseContainer = pauseContainerImage.gameObject;
            TMP_Text pauseTitleText = FindUniqueDescendantComponent<TMP_Text>(
                pauseContainer,
                "Pause Title Text");
            Button pauseResumeButton = FindDirectComponent<Button>(
                pauseContainer,
                "ResumeButton");
            Button pauseRetryButton = FindDirectComponent<Button>(
                pauseContainer,
                "RetryButton");
            Button pauseSettingsButton = FindDirectComponent<Button>(
                pauseContainer,
                "SettingsButton");
            Button pauseMainMenuButton = FindDirectComponent<Button>(
                pauseContainer,
                "MainMenuButton");
            AssertPausePresentation(
                pauseContainerImage,
                pauseTitleText,
                pauseResumeButton,
                pauseRetryButton,
                pauseSettingsButton,
                pauseMainMenuButton);

            GameObject momentumCanvas = RequireDirectCanvas(momentumHud);
            TMP_Text momentumMultiplierText = GetSerializedValue<TMP_Text>(
                uiManagementSystem,
                "_momentumMultiplierText");
            Image momentumDurationFillImage = GetSerializedValue<Image>(
                uiManagementSystem,
                "_momentumDurationFillImage");
            Assert.That(momentumMultiplierText, Is.Not.Null);
            Assert.That(momentumDurationFillImage, Is.Not.Null);
            Image momentumMultiplierContainer =
                momentumMultiplierText.transform.parent?.GetComponent<Image>();
            Image momentumDurationBackground =
                momentumDurationFillImage.transform.parent?.GetComponent<Image>();
            Assert.That(momentumMultiplierContainer, Is.Not.Null);
            Assert.That(momentumDurationBackground, Is.Not.Null);
            Assert.That(
                momentumMultiplierContainer.transform.parent,
                Is.EqualTo(momentumCanvas.transform));
            Assert.That(
                momentumDurationBackground.transform.parent,
                Is.EqualTo(momentumCanvas.transform));
            MomentumGradientEffect momentumGradientEffect =
                momentumDurationFillImage.GetComponent<MomentumGradientEffect>();
            Assert.That(momentumGradientEffect, Is.Not.Null);
            Assert.That(momentumDurationFillImage.type, Is.EqualTo(Image.Type.Filled));
            Assert.That(momentumDurationFillImage.fillMethod,
                Is.EqualTo(Image.FillMethod.Horizontal));
            Assert.That(momentumDurationFillImage.fillOrigin, Is.Zero);
            Assert.That(momentumDurationFillImage.fillAmount, Is.Zero);
            Assert.That(momentumDurationFillImage.color, Is.EqualTo(Color.white));
            Assert.That(momentumDurationFillImage.raycastTarget, Is.False);
            AssertMomentumHudPresentation(
                momentumMultiplierContainer,
                momentumMultiplierText,
                momentumDurationBackground,
                momentumDurationFillImage);

            AssertSerializedReference(uiManagementSystem, "_stageHud", stageHud);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteHud",
                infiniteHud);
            AssertSerializedReference(
                uiManagementSystem,
                "_momentumHud",
                momentumHud);
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
                "_baseDistanceScoreText",
                baseDistanceScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_momentumBonusText",
                momentumBonusText);
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
                "_infiniteDifficultyText",
                infiniteDifficultyText);
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
                "_infiniteResultBaseDistanceScoreText",
                infiniteResultBaseDistanceScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteResultMomentumBonusText",
                infiniteResultMomentumBonusText);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteResultMaximumMomentumText",
                infiniteResultMaximumMomentumText);
            AssertSerializedReference(
                uiManagementSystem,
                "_momentumMultiplierText",
                momentumMultiplierText);
            AssertSerializedReference(
                uiManagementSystem,
                "_momentumDurationFillImage",
                momentumDurationFillImage);
            Assert.That(
                GetSerializedValue<Gradient>(
                    uiManagementSystem,
                    "_momentumDurationGradient"),
                Is.Not.Null);
            // Boot intentionally has no GameRuntimeData. The runtime gradient is
            // applied by Initialize(GameRuntimeData) when a run starts.
            Assert.That(momentumGradientEffect.HasGradient, Is.False);
            AssertSerializedReference(
                uiManagementSystem,
                "_retryButton",
                resultRetryButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_resultMainMenuButton",
                resultMainMenuButton);
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
                "_pauseSettingsButton",
                pauseSettingsButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseMainMenuButton",
                pauseMainMenuButton);

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
            GameObject momentumHud = FindUniqueSceneObject("MomentumHUD");
            GameObject resultPanel = FindUniqueSceneObject("ResultPanel");
            GameObject pausePanel = FindUniqueSceneObject("PausePanel");
            GameObject stageResultContent =
                FindUniqueSceneObject("StageResultContent");
            GameObject infiniteResultContent =
                FindUniqueSceneObject("InfiniteResultContent");
            RectTransform resultWindow =
                stageResultContent.transform.parent as RectTransform;
            Assert.That(resultWindow, Is.Not.Null);
            Button resultRetryButton = GetSerializedValue<Button>(
                uiManagementSystem,
                "_retryButton");
            Button resultMainMenuButton = GetSerializedValue<Button>(
                uiManagementSystem,
                "_resultMainMenuButton");
            Assert.That(resultRetryButton, Is.Not.Null);
            Assert.That(resultMainMenuButton, Is.Not.Null);

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
            Assert.That(momentumHud.activeSelf, Is.False);

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
            Assert.That(momentumHud.activeSelf, Is.False);

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
                false,
                false,
                true,
                false);
            Assert.That(momentumHud.activeSelf, Is.False);
            Assert.That(stageResultContent.activeSelf, Is.True);
            Assert.That(infiniteResultContent.activeSelf, Is.False);
            AssertResultWindowRect(resultWindow, new Vector2(520.0f, 400.0f));
            AssertResultButtonRect(resultRetryButton, -224.0f);
            AssertResultButtonRect(resultMainMenuButton, -282.0f);
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
            Assert.That(momentumHud.activeSelf, Is.True);

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
            Assert.That(momentumHud.activeSelf, Is.True);

            ResultData infiniteResultData = new ResultData(
                ScoringVersion.Current,
                12.9f,
                100,
                20,
                120,
                30,
                150,
                2.0);
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
                false,
                true,
                false);
            Assert.That(momentumHud.activeSelf, Is.False);
            Assert.That(stageResultContent.activeSelf, Is.False);
            Assert.That(infiniteResultContent.activeSelf, Is.True);
            AssertResultWindowRect(resultWindow, new Vector2(520.0f, 520.0f));
            AssertResultButtonRect(resultRetryButton, -334.0f);
            AssertResultButtonRect(resultMainMenuButton, -392.0f);
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

        private static void AssertResultWindowRect(
            RectTransform resultWindow,
            Vector2 size)
        {
            Assert.That(resultWindow.anchorMin, Is.EqualTo(new Vector2(0.5f, 0.5f)));
            Assert.That(resultWindow.anchorMax, Is.EqualTo(new Vector2(0.5f, 0.5f)));
            Assert.That(resultWindow.pivot, Is.EqualTo(new Vector2(0.5f, 0.5f)));
            Assert.That(resultWindow.anchoredPosition, Is.EqualTo(Vector2.zero));
            Assert.That(resultWindow.sizeDelta, Is.EqualTo(size));
        }

        private static void AssertResultButtonRect(Button button, float yPosition)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Assert.That(rectTransform.anchorMin, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.anchorMax, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.pivot, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.anchoredPosition,
                Is.EqualTo(new Vector2(0.0f, yPosition)));
            Assert.That(rectTransform.sizeDelta, Is.EqualTo(new Vector2(472.0f, 48.0f)));
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
                    out string expectedBaseDistanceScore,
                    out string expectedMomentumBonus,
                    out string expectedDistanceScore,
                    out string expectedCollectibleScore,
                    out string expectedTotalScore,
                    out string expectedMaximumMomentum),
                Is.True);
            Assert.That(
                GetSerializedText(uiManagementSystem, "_finalDistanceText"),
                Is.EqualTo(expectedDistance));
            Assert.That(
                GetSerializedText(
                    uiManagementSystem,
                    "_infiniteResultBaseDistanceScoreText"),
                Is.EqualTo(expectedBaseDistanceScore));
            Assert.That(
                GetSerializedText(
                    uiManagementSystem,
                    "_infiniteResultMomentumBonusText"),
                Is.EqualTo(expectedMomentumBonus));
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
            Assert.That(
                GetSerializedText(
                    uiManagementSystem,
                    "_infiniteResultMaximumMomentumText"),
                Is.EqualTo(expectedMaximumMomentum));
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

        private static T FindUniqueDirectChildComponent<T>(GameObject parent)
            where T : Component
        {
            T match = null;

            foreach (Transform child in parent.transform)
            {
                T component = child.GetComponent<T>();
                if (component == null)
                {
                    continue;
                }

                Assert.That(
                    match,
                    Is.Null,
                    $"{parent.name} must have exactly one direct {typeof(T).Name} child.");
                match = component;
            }

            Assert.That(
                match,
                Is.Not.Null,
                $"{parent.name} must have one direct {typeof(T).Name} child.");
            return match;
        }

        private static void AssertInfiniteHudPresentation(
            Image panel,
            TMP_Text distanceText,
            TMP_Text baseDistanceScoreText,
            TMP_Text momentumBonusText,
            TMP_Text scoreText,
            TMP_Text collectibleScoreText,
            TMP_Text totalScoreText,
            TMP_Text difficultyText)
        {
            AssertHudPanel(
                panel,
                new Vector2(1.0f, 1.0f),
                new Vector2(-24.0f, -24.0f),
                new Vector2(300.0f, 238.0f),
                new Color(11.0f / 255.0f, 20.0f / 255.0f, 38.0f / 255.0f, 0.8f));
            VerticalLayoutGroup layoutGroup = panel.GetComponent<VerticalLayoutGroup>();
            Assert.That(layoutGroup, Is.Not.Null);
            Assert.That(layoutGroup.padding.left, Is.EqualTo(16));
            Assert.That(layoutGroup.padding.right, Is.EqualTo(16));
            Assert.That(layoutGroup.padding.top, Is.EqualTo(14));
            Assert.That(layoutGroup.padding.bottom, Is.EqualTo(14));
            Assert.That(layoutGroup.spacing, Is.EqualTo(4.0f));
            Assert.That(layoutGroup.childControlWidth, Is.True);
            Assert.That(layoutGroup.childControlHeight, Is.True);

            TMP_Text[] standardTexts =
            {
                distanceText,
                baseDistanceScoreText,
                momentumBonusText,
                scoreText,
                collectibleScoreText
            };
            foreach (TMP_Text text in standardTexts)
            {
                Assert.That(text.raycastTarget, Is.False);
                Assert.That(text.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
                Assert.That(text.fontSize, Is.EqualTo(20.0f));
            }

            Assert.That(totalScoreText.raycastTarget, Is.False);
            Assert.That(totalScoreText.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
            Assert.That(totalScoreText.fontSize, Is.EqualTo(24.0f));
            Assert.That(difficultyText.raycastTarget, Is.False);
            Assert.That(difficultyText.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
            Assert.That(difficultyText.fontSize, Is.EqualTo(16.0f));
        }

        private static void AssertMomentumHudPresentation(
            Image multiplierPanel,
            TMP_Text multiplierText,
            Image durationBackground,
            Image durationFill)
        {
            AssertHudPanel(
                multiplierPanel,
                new Vector2(1.0f, 0.0f),
                new Vector2(-24.0f, 76.0f),
                new Vector2(220.0f, 36.0f),
                new Color(11.0f / 255.0f, 20.0f / 255.0f, 38.0f / 255.0f, 0.8f));
            Assert.That(multiplierText.raycastTarget, Is.False);
            Assert.That(multiplierText.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
            Assert.That(multiplierText.fontSize, Is.EqualTo(22.0f));

            AssertHudPanel(
                durationBackground,
                new Vector2(1.0f, 0.0f),
                new Vector2(-24.0f, 32.0f),
                new Vector2(220.0f, 18.0f),
                new Color(11.0f / 255.0f, 20.0f / 255.0f, 38.0f / 255.0f, 0.6f));
            Assert.That(durationBackground.raycastTarget, Is.False);
            Assert.That(durationFill.rectTransform.anchorMin, Is.EqualTo(Vector2.zero));
            Assert.That(durationFill.rectTransform.anchorMax, Is.EqualTo(Vector2.one));
            Assert.That(durationFill.rectTransform.anchoredPosition, Is.EqualTo(Vector2.zero));
            Assert.That(durationFill.rectTransform.sizeDelta, Is.EqualTo(new Vector2(-8.0f, -8.0f)));
        }

        private static void AssertPausePresentation(
            Image pauseWindow,
            TMP_Text titleText,
            Button resumeButton,
            Button retryButton,
            Button settingsButton,
            Button mainMenuButton)
        {
            AssertHudPanel(
                pauseWindow,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(420.0f, 360.0f),
                new Color(
                    11.0f / 255.0f,
                    20.0f / 255.0f,
                    38.0f / 255.0f,
                    230.0f / 255.0f));
            Assert.That(pauseWindow.GetComponent<VerticalLayoutGroup>(), Is.Null,
                "Pause window uses explicit, stable control positions.");

            Image titleContainer = titleText.transform.parent?.GetComponent<Image>();
            Assert.That(titleContainer, Is.Not.Null);
            Assert.That(titleContainer.raycastTarget, Is.False);
            Assert.That(titleContainer.color.a, Is.Zero.Within(0.001f));
            Assert.That(titleContainer.rectTransform.anchorMin,
                Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(titleContainer.rectTransform.anchorMax,
                Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(titleContainer.rectTransform.anchoredPosition,
                Is.EqualTo(new Vector2(0.0f, -50.0f)));
            Assert.That(titleContainer.rectTransform.sizeDelta,
                Is.EqualTo(new Vector2(372.0f, 48.0f)));
            Assert.That(titleText.raycastTarget, Is.False);
            Assert.That(titleText.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
            Assert.That(titleText.fontSize, Is.EqualTo(36.0f));

            AssertPauseButtonPresentation(resumeButton, -116.0f);
            AssertPauseButtonPresentation(retryButton, -174.0f);
            AssertPauseButtonPresentation(settingsButton, -232.0f);
            AssertPauseButtonPresentation(mainMenuButton, -290.0f);
        }

        private static void AssertStageResultPresentation(
            Image resultWindow,
            GameObject stageResultContent,
            TMP_Text statusText,
            TMP_Text clearTimeText,
            TMP_Text collectibleScoreText,
            Button retryButton,
            Button mainMenuButton)
        {
            AssertHudPanel(
                resultWindow,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(520.0f, 400.0f),
                new Color(
                    11.0f / 255.0f,
                    20.0f / 255.0f,
                    38.0f / 255.0f,
                    230.0f / 255.0f));
            Assert.That(resultWindow.GetComponent<VerticalLayoutGroup>(), Is.Null,
                "Result window uses explicit, stable control positions.");

            RectTransform stageContentRect = stageResultContent.GetComponent<RectTransform>();
            Assert.That(stageContentRect.anchorMin, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(stageContentRect.anchorMax, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(stageContentRect.pivot, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(stageContentRect.anchoredPosition,
                Is.EqualTo(new Vector2(0.0f, -36.0f)));
            Assert.That(stageContentRect.sizeDelta, Is.EqualTo(new Vector2(472.0f, 150.0f)));

            Image stageContentBackground =
                FindUniqueDirectChildComponent<Image>(stageResultContent);
            Assert.That(stageContentBackground.raycastTarget, Is.False);
            Assert.That(stageContentBackground.color.a, Is.Zero.Within(0.001f));

            AssertStageResultTextPresentation(statusText, -8.0f, 44.0f, 34.0f);
            AssertStageResultTextPresentation(clearTimeText, -58.0f, 36.0f, 22.0f);
            AssertStageResultTextPresentation(
                collectibleScoreText,
                -100.0f,
                36.0f,
                22.0f);
            AssertResultButtonPresentation(retryButton);
            AssertResultButtonPresentation(mainMenuButton);
        }

        private static void AssertStageResultTextPresentation(
            TMP_Text text,
            float yPosition,
            float height,
            float fontSize)
        {
            RectTransform rectTransform = text.rectTransform;
            Assert.That(rectTransform.anchorMin, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.anchorMax, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.anchoredPosition,
                Is.EqualTo(new Vector2(0.0f, yPosition)));
            Assert.That(rectTransform.sizeDelta, Is.EqualTo(new Vector2(440.0f, height)));
            Assert.That(text.raycastTarget, Is.False);
            Assert.That(text.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
            Assert.That(text.fontSize, Is.EqualTo(fontSize));
        }

        private static void AssertInfiniteResultPresentation(
            GameObject infiniteResultContent,
            TMP_Text finalDistanceText,
            TMP_Text baseDistanceScoreText,
            TMP_Text momentumBonusText,
            TMP_Text finalScoreText,
            TMP_Text collectibleScoreText,
            TMP_Text totalScoreText,
            TMP_Text maximumMomentumText)
        {
            RectTransform contentRect = infiniteResultContent.GetComponent<RectTransform>();
            Assert.That(contentRect.anchorMin, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(contentRect.anchorMax, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(contentRect.pivot, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(contentRect.anchoredPosition,
                Is.EqualTo(new Vector2(0.0f, -36.0f)));
            Assert.That(contentRect.sizeDelta, Is.EqualTo(new Vector2(472.0f, 278.0f)));

            Image contentBackground =
                FindUniqueDirectChildComponent<Image>(infiniteResultContent);
            Assert.That(contentBackground.raycastTarget, Is.False);
            Assert.That(contentBackground.color.a, Is.Zero.Within(0.001f));
            Assert.That(contentBackground.GetComponent<VerticalLayoutGroup>(), Is.Null);
            Assert.That(contentBackground.rectTransform.anchorMin, Is.EqualTo(Vector2.zero));
            Assert.That(contentBackground.rectTransform.anchorMax, Is.EqualTo(Vector2.one));
            Assert.That(contentBackground.rectTransform.anchoredPosition, Is.EqualTo(Vector2.zero));
            Assert.That(contentBackground.rectTransform.sizeDelta, Is.EqualTo(Vector2.zero));

            AssertStageResultTextPresentation(finalDistanceText, -8.0f, 34.0f, 26.0f);
            AssertStageResultTextPresentation(baseDistanceScoreText, -48.0f, 30.0f, 20.0f);
            AssertStageResultTextPresentation(momentumBonusText, -82.0f, 30.0f, 20.0f);
            AssertStageResultTextPresentation(finalScoreText, -116.0f, 30.0f, 20.0f);
            AssertStageResultTextPresentation(collectibleScoreText, -150.0f, 30.0f, 20.0f);
            AssertStageResultTextPresentation(totalScoreText, -184.0f, 34.0f, 24.0f);
            AssertStageResultTextPresentation(maximumMomentumText, -226.0f, 30.0f, 20.0f);
        }

        private static void AssertResultButtonPresentation(Button button)
        {
            Assert.That(button.transition, Is.EqualTo(Selectable.Transition.ColorTint));
            Assert.That(button.targetGraphic, Is.TypeOf<Image>());
        }

        private static void AssertPauseButtonPresentation(Button button, float yPosition)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Assert.That(rectTransform.anchorMin, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.anchorMax, Is.EqualTo(new Vector2(0.5f, 1.0f)));
            Assert.That(rectTransform.anchoredPosition,
                Is.EqualTo(new Vector2(0.0f, yPosition)));
            Assert.That(rectTransform.sizeDelta, Is.EqualTo(new Vector2(372.0f, 48.0f)));
            Assert.That(button.transition, Is.EqualTo(Selectable.Transition.ColorTint));
            Assert.That(button.targetGraphic, Is.TypeOf<Image>());
            Assert.That(button.colors.colorMultiplier, Is.EqualTo(1.0f));
            Assert.That(button.colors.fadeDuration, Is.EqualTo(0.1f));
        }

        private static void AssertHudPanel(
            Image panel,
            Vector2 anchor,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            Assert.That(panel.raycastTarget, Is.False);
            Assert.That(panel.rectTransform.anchorMin, Is.EqualTo(anchor));
            Assert.That(panel.rectTransform.anchorMax, Is.EqualTo(anchor));
            Assert.That(panel.rectTransform.anchoredPosition, Is.EqualTo(anchoredPosition));
            Assert.That(panel.rectTransform.sizeDelta, Is.EqualTo(size));
            Assert.That(panel.color.r, Is.EqualTo(color.r).Within(0.001f));
            Assert.That(panel.color.g, Is.EqualTo(color.g).Within(0.001f));
            Assert.That(panel.color.b, Is.EqualTo(color.b).Within(0.001f));
            Assert.That(panel.color.a, Is.EqualTo(color.a).Within(0.001f));
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

        private T GetSerializedValue<T>(
            MonoBehaviour target,
            string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }
    }
}
