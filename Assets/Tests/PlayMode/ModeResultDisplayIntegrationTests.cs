using System;
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
    public class ModeResultDisplayIntegrationTests
    {
        private const string SceneName = "SampleScene";

        private MonoBehaviour _uiManagementSystem;
        private TMP_Text _resultStatusText;
        private TMP_Text _clearTimeText;
        private TMP_Text _stageResultCollectibleScoreText;
        private TMP_Text _finalDistanceText;
        private TMP_Text _infiniteResultBaseDistanceScoreText;
        private TMP_Text _infiniteResultMomentumBonusText;
        private TMP_Text _finalScoreText;
        private TMP_Text _infiniteResultCollectibleScoreText;
        private TMP_Text _infiniteResultTotalScoreText;
        private TMP_Text _infiniteResultMaximumMomentumText;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            yield return null;
            _uiManagementSystem = UnityEngine.Object.FindFirstObjectByType(
                FindType("FlowState.Runtime.Systems.UIManagementSystem")) as MonoBehaviour;
            Assert.That(_uiManagementSystem, Is.Not.Null);
            _resultStatusText = GetPrivateField<TMP_Text>("_resultStatusText");
            _clearTimeText = GetPrivateField<TMP_Text>("_clearTimeText");
            _stageResultCollectibleScoreText = GetPrivateField<TMP_Text>("_stageResultCollectibleScoreText");
            _finalDistanceText = GetPrivateField<TMP_Text>("_finalDistanceText");
            _infiniteResultBaseDistanceScoreText = GetPrivateField<TMP_Text>("_infiniteResultBaseDistanceScoreText");
            _infiniteResultMomentumBonusText = GetPrivateField<TMP_Text>("_infiniteResultMomentumBonusText");
            _finalScoreText = GetPrivateField<TMP_Text>("_finalScoreText");
            _infiniteResultCollectibleScoreText = GetPrivateField<TMP_Text>("_infiniteResultCollectibleScoreText");
            _infiniteResultTotalScoreText = GetPrivateField<TMP_Text>("_infiniteResultTotalScoreText");
            _infiniteResultMaximumMomentumText = GetPrivateField<TMP_Text>("_infiniteResultMaximumMomentumText");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Scene scene = SceneManager.GetSceneByName(SceneName);
            if (!scene.isLoaded) yield break;
            Scene cleanup = SceneManager.GetSceneByName("GameEntryTestCleanup");
            if (!cleanup.isLoaded) cleanup = SceneManager.CreateScene("GameEntryTestCleanup");
            SceneManager.SetActiveScene(cleanup);
            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [Test]
        public void StageResult_IsRejectedWithoutDisplayingLegacyFields()
        {
            Initialize(E_GameMode.Infinite);
            LogAssert.Expect(LogType.Warning, "[UIManagementSystem] Only Infinite Result Data can be displayed.");
            Assert.That(SetResultData(new ResultData(
                E_StageResultType.Cleared, 12.3456, 30)), Is.False);
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_finalDistanceText.text, Is.Empty);
            Assert.That(_finalScoreText.text, Is.Empty);
        }

        [Test]
        public void FellStageResult_IsRejectedWithoutDisplayingLegacyFields()
        {
            Initialize(E_GameMode.Infinite);
            LogAssert.Expect(LogType.Warning, "[UIManagementSystem] Only Infinite Result Data can be displayed.");
            Assert.That(SetResultData(new ResultData(
                E_StageResultType.Fell, 12.3456, 30)), Is.False);
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_finalDistanceText.text, Is.Empty);
            Assert.That(_finalScoreText.text, Is.Empty);
        }

        [Test]
        public void InfiniteResult_DisplaysOnlyInfiniteResultFields()
        {
            Initialize(E_GameMode.Infinite);

            bool didSetResult = SetResultData(
                CreateInfiniteResult(12.999f, 100, 29, 30, 2.0));

            Assert.That(didSetResult, Is.True);
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
            Assert.That(
                _finalDistanceText.text,
                Is.EqualTo("Final Distance: 12"));
            Assert.That(
                _infiniteResultBaseDistanceScoreText.text,
                Is.EqualTo("Base Distance Score: 100"));
            Assert.That(
                _infiniteResultMomentumBonusText.text,
                Is.EqualTo("Momentum Bonus: +29"));
            Assert.That(_finalScoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(
                _infiniteResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 30"));
            Assert.That(
                _infiniteResultTotalScoreText.text,
                Is.EqualTo("Total Score: 159"));
            Assert.That(
                _infiniteResultMaximumMomentumText.text,
                Is.EqualTo("Max Momentum: x2.00"));
        }

        [Test]
        public void Initialize_AfterResult_ClearsPreviousResultText()
        {
            Initialize(E_GameMode.Infinite);
            Assert.That(SetResultData(CreateInfiniteResult(12.999f, 100, 29, 30, 2.0)), Is.True);
            Assert.That(_finalScoreText.text, Is.EqualTo("Distance Score: 129"));

            Initialize(E_GameMode.Infinite);

            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_finalDistanceText.text, Is.Empty);
            Assert.That(_infiniteResultBaseDistanceScoreText.text, Is.Empty);
            Assert.That(_infiniteResultMomentumBonusText.text, Is.Empty);
            Assert.That(_finalScoreText.text, Is.Empty);
            Assert.That(_infiniteResultCollectibleScoreText.text, Is.Empty);
            Assert.That(_infiniteResultTotalScoreText.text, Is.Empty);
            Assert.That(_infiniteResultMaximumMomentumText.text, Is.Empty);
        }

        [Test]
        public void ConsecutiveInfiniteRuns_DisplayIndependentResults()
        {
            Initialize(E_GameMode.Infinite);
            Assert.That(SetResultData(CreateInfiniteResult(12.999f, 100, 29, 30, 2.0)), Is.True);
            Assert.That(_finalScoreText.text, Is.EqualTo("Distance Score: 129"));

            Initialize(E_GameMode.Infinite);
            Assert.That(SetResultData(CreateInfiniteResult(20.999f, 180, 29, 40, 2.5)), Is.True);

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
        public void StageRuntimeAndResult_AfterInfiniteResult_PreserveCurrentDisplay()
        {
            Initialize(E_GameMode.Infinite);
            Assert.That(SetResultData(CreateInfiniteResult(12.999f, 100, 29, 30, 2.0)), Is.True);
            GameRuntimeData retired = new GameRuntimeData();
            retired.Initialize();
            typeof(GameRuntimeData).GetField("_gameMode", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(retired, E_GameMode.Stage);
            LogAssert.Expect(LogType.Warning, "[UIManagementSystem] Only created Infinite runtime data can be displayed.");
            InvokePublicMethod("Initialize", retired);
            LogAssert.Expect(LogType.Warning, "[UIManagementSystem] Only Infinite Result Data can be displayed.");
            Assert.That(SetResultData(new ResultData(E_StageResultType.Cleared, 5.25, 30)), Is.False);
            Assert.That(_finalDistanceText.text, Is.EqualTo("Final Distance: 12"));
            Assert.That(_finalScoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(_infiniteResultTotalScoreText.text, Is.EqualTo("Total Score: 159"));
            Assert.That(_resultStatusText.text, Is.Empty);
            Assert.That(_clearTimeText.text, Is.Empty);
            Assert.That(_stageResultCollectibleScoreText.text, Is.Empty);
        }

        [Test]
        public void RetiredStageAndPlayButtons_CannotReopenOrKeepFocus()
        {
            GameObject modePanel = GetPrivateField<GameObject>("_modeSelectPanel");
            Button play = GetPrivateField<Button>("_mainMenuPlayButton");
            Button[] buttons = { GetPrivateField<Button>("_modeSelectStageButton"),
                GetPrivateField<Button>("_modeSelectInfiniteButton"), GetPrivateField<Button>("_modeSelectBackButton"),
                GetPrivateField<Button>("_leaderboardStageButton") };
            Initialize(E_GameMode.Infinite);
            foreach (E_NavigationScreen screen in new[] { E_NavigationScreen.ModeSelect,
                E_NavigationScreen.MainMenu, E_NavigationScreen.Playing, E_NavigationScreen.Pause,
                E_NavigationScreen.Result, E_NavigationScreen.Settings, E_NavigationScreen.AutomaticHowToPlay,
                E_NavigationScreen.Leaderboard })
            {
                modePanel.SetActive(true);
                foreach (Button button in buttons)
                {
                    button.gameObject.SetActive(true);
                    button.interactable = true;
                }
                EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
                InvokePublicMethod("SetNavigationScreen", screen, E_NavigationItem.Stage);
                Assert.That(modePanel.activeSelf, Is.False);
                foreach (Button button in buttons)
                {
                    Assert.That(button.gameObject.activeSelf, Is.False);
                    Assert.That(button.interactable, Is.False);
                    Assert.That(button.navigation.mode, Is.EqualTo(Navigation.Mode.None));
                    Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.EqualTo(button.gameObject));
                }
                Assert.That(GetPrivateField<GameObject>("_stageHud").activeSelf, Is.False);
                Assert.That(GetPrivateField<GameObject>("_stageResultContent").activeSelf, Is.False);
                if (screen == E_NavigationScreen.Leaderboard)
                    Assert.That(EventSystem.current.currentSelectedGameObject,
                        Is.SameAs(GetPrivateField<Button>("_leaderboardInfiniteButton").gameObject));
                if (screen == E_NavigationScreen.ModeSelect)
                {
                    Assert.That(GetPrivateField<E_NavigationScreen>("_currentNavigationScreen"),
                        Is.EqualTo(E_NavigationScreen.MainMenu));
                    Assert.That(EventSystem.current.currentSelectedGameObject, Is.SameAs(play.gameObject));
                }
            }
        }

        [Test]
        public void StageResultDescendantFocus_IsClearedWhenDisplayIsApplied()
        {
            Initialize(E_GameMode.Infinite);
            GameObject root = GetPrivateField<GameObject>("_stageResultContent");
            GameObject retired = _resultStatusText.gameObject;
            root.SetActive(true);
            EventSystem.current.SetSelectedGameObject(retired);
            InvokePublicMethod("SetGameState", E_GameState.Ended);
            InvokePublicMethod("SetUIState", E_UIState.Result);
            Assert.That(root.activeSelf, Is.False);
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.EqualTo(retired));
        }

        private T GetPrivateField<T>(string fieldName)
        {
            FieldInfo field = _uiManagementSystem.GetType().GetField(fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(_uiManagementSystem);
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

        private ResultData CreateInfiniteResult(
            float finalDistance,
            int baseDistanceScore,
            int momentumBonus,
            int collectibleScore,
            double maximumMomentumMultiplier)
        {
            int distanceScore = baseDistanceScore + momentumBonus;
            return new ResultData(
                ScoringVersion.Current,
                finalDistance,
                baseDistanceScore,
                momentumBonus,
                distanceScore,
                collectibleScore,
                distanceScore + collectibleScore,
                maximumMomentumMultiplier);
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
