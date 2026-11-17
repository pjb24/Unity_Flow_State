using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class InfiniteHudIntegrationTests
    {
        private MonoBehaviour _uiManagementSystem;
        private GameRuntimeData _runtimeData;
        private TMP_Text _distanceText;
        private TMP_Text _baseDistanceScoreText;
        private TMP_Text _momentumBonusText;
        private TMP_Text _scoreText;
        private TMP_Text _stageCollectibleScoreText;
        private TMP_Text _infiniteCollectibleScoreText;
        private TMP_Text _infiniteTotalScoreText;
        private TMP_Text _infiniteDifficultyText;
        private GameObject _stageHud;
        private GameObject _infiniteHud;
        private GameObject _momentumHud;
        private TMP_Text _momentumMultiplierText;
        private Image _momentumDurationFillImage;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            yield return null;
            _uiManagementSystem = ProductionSceneGameModeTestUtility.FindSceneObject("UIManagementSystem")
                .GetComponent(FindType("FlowState.Runtime.Systems.UIManagementSystem")) as MonoBehaviour;
            Assert.That(_uiManagementSystem, Is.Not.Null);
            _stageHud = GetPrivateField<GameObject>("_stageHud");
            Assert.That(_stageHud, Is.Not.Null);
            _infiniteHud = GetPrivateField<GameObject>("_infiniteHud");
            Assert.That(_infiniteHud, Is.Not.Null);
            _distanceText = GetPrivateField<TMP_Text>("_distanceText");
            Assert.That(_distanceText, Is.Not.Null);
            _baseDistanceScoreText = GetPrivateField<TMP_Text>("_baseDistanceScoreText");
            Assert.That(_baseDistanceScoreText, Is.Not.Null);
            _momentumBonusText = GetPrivateField<TMP_Text>("_momentumBonusText");
            Assert.That(_momentumBonusText, Is.Not.Null);
            _scoreText = GetPrivateField<TMP_Text>("_scoreText");
            Assert.That(_scoreText, Is.Not.Null);
            _stageCollectibleScoreText = GetPrivateField<TMP_Text>("_stageCollectibleScoreText");
            Assert.That(_stageCollectibleScoreText, Is.Not.Null);
            _infiniteCollectibleScoreText = GetPrivateField<TMP_Text>("_infiniteCollectibleScoreText");
            Assert.That(_infiniteCollectibleScoreText, Is.Not.Null);
            _infiniteTotalScoreText = GetPrivateField<TMP_Text>("_infiniteTotalScoreText");
            Assert.That(_infiniteTotalScoreText, Is.Not.Null);
            _infiniteDifficultyText = GetPrivateField<TMP_Text>("_infiniteDifficultyText");
            Assert.That(_infiniteDifficultyText, Is.Not.Null);
            _momentumHud = GetPrivateField<GameObject>("_momentumHud");
            Assert.That(_momentumHud, Is.Not.Null);
            _momentumMultiplierText = GetPrivateField<TMP_Text>("_momentumMultiplierText");
            Assert.That(_momentumMultiplierText, Is.Not.Null);
            _momentumDurationFillImage = GetPrivateField<Image>("_momentumDurationFillImage");
            Assert.That(_momentumDurationFillImage, Is.Not.Null);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return ProductionSceneGameModeTestUtility.UnloadProductionScene();
        }

        [UnityTest]
        public IEnumerator InfiniteRunStart_DisplaysZeroDistanceAndScore()
        {
            StartRun(E_GameMode.Infinite);

            yield return null;

            Assert.That(_infiniteHud.activeSelf, Is.True);
            Assert.That(_stageHud.activeSelf, Is.False);
            Assert.That(_distanceText.text, Is.EqualTo("Distance: 0"));
            Assert.That(_baseDistanceScoreText.text,
                Is.EqualTo("Base Distance Score: 0"));
            Assert.That(_momentumBonusText.text,
                Is.EqualTo("Momentum Bonus: +0"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 0"));
            Assert.That(
                _infiniteCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 0"));
            Assert.That(
                _infiniteTotalScoreText.text,
                Is.EqualTo("Total Score: 0"));
            Assert.That(_infiniteDifficultyText.text,
                Is.EqualTo("Difficulty: D1"));
            Assert.That(_momentumHud.activeSelf, Is.True);
            Assert.That(_momentumMultiplierText.text, Is.EqualTo("x1.00"));
            Assert.That(_momentumDurationFillImage.fillAmount, Is.Zero);
        }

        [UnityTest]
        public IEnumerator MomentumHud_UpdatesAndFreezesAcrossPauseAndResult()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(TryUpdateInfinite(
                10.0f, 100, 25, 1.25, 10.0, 10.0), Is.True);
            yield return null;

            Assert.That(_momentumMultiplierText.text, Is.EqualTo("x1.25"));
            Assert.That(_momentumDurationFillImage.fillAmount, Is.EqualTo(1.0f));
            Assert.That(_momentumDurationFillImage.color, Is.EqualTo(Color.white));
            Assert.That(
                _momentumDurationFillImage.GetComponent<MomentumGradientEffect>()
                    .HasGradient,
                Is.True);

            SetGameState(E_GameState.Paused);
            SetUIState(E_UIState.Pause);
            Assert.That(TryUpdateInfinite(
                20.0f, 200, 50, 1.5, 0.5, 9.5), Is.True);
            yield return null;
            Assert.That(_momentumMultiplierText.text, Is.EqualTo("x1.25"));
            Assert.That(_momentumDurationFillImage.fillAmount, Is.EqualTo(1.0f));

            SetGameState(E_GameState.Ended);
            SetUIState(E_UIState.Result);
            yield return null;
            Assert.That(_momentumHud.activeSelf, Is.False);
            Assert.That(_momentumMultiplierText.text, Is.EqualTo("x1.25"));
        }

        [UnityTest]
        public IEnumerator Difficulty_AdvancesAndPauseResultRetainLastValue()
        {
            StartRun(E_GameMode.Infinite);
            _runtimeData.InfiniteModeRuntimeData.TryUpdateDifficultyLevel(2);
            yield return null;
            Assert.That(_infiniteDifficultyText.text,
                Is.EqualTo("Difficulty: D2"));

            SetGameState(E_GameState.Paused);
            SetUIState(E_UIState.Pause);
            _runtimeData.InfiniteModeRuntimeData.TryUpdateDifficultyLevel(3);
            yield return null;
            Assert.That(_infiniteDifficultyText.text,
                Is.EqualTo("Difficulty: D2"));

            SetGameState(E_GameState.Playing);
            SetUIState(E_UIState.StageHud);
            yield return null;
            Assert.That(_infiniteDifficultyText.text,
                Is.EqualTo("Difficulty: D3"));

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);
            _runtimeData.InfiniteModeRuntimeData.Clear();
            SetGameState(E_GameState.Ended);
            yield return null;
            Assert.That(_infiniteDifficultyText.text,
                Is.EqualTo("Difficulty: D3"));

            StartRun(E_GameMode.Infinite);
            yield return null;
            Assert.That(_infiniteDifficultyText.text,
                Is.EqualTo("Difficulty: D1"));
        }

        [UnityTest]
        public IEnumerator Difficulty_DevelopmentToggleHidesText()
        {
            SetPrivateField("_showDifficultyInDevelopment", false);
            StartRun(E_GameMode.Infinite);
            yield return null;

            Assert.That(_infiniteDifficultyText.gameObject.activeSelf,
                Is.False);
            Assert.That(_distanceText.text, Is.EqualTo("Distance: 0"));
        }

        [UnityTest]
        public IEnumerator RuntimeDataUpdate_UpdatesDisplayedValues()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(
                TryUpdateInfinite(12.999f, 129),
                Is.True);
            AwardCollectible("coin-1");

            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(
                _infiniteCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 10"));
            Assert.That(
                _infiniteTotalScoreText.text,
                Is.EqualTo("Total Score: 139"));
        }

        [UnityTest]
        public IEnumerator RejectedBackwardProgress_KeepsMaximumDisplayedValues()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(
                TryUpdateInfinite(12.999f, 129),
                Is.True);
            yield return null;

            Assert.That(
                TryUpdateInfinite(8.0f, 80),
                Is.False);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 129"));
        }

        [UnityTest]
        public IEnumerator Pause_FreezesHudUntilResume()
        {
            StartRun(E_GameMode.Infinite);
            TryUpdateInfinite(12.999f, 129);
            AwardCollectible("coin-before-pause");
            yield return null;

            SetGameState(E_GameState.Paused);
            SetUIState(E_UIState.Pause);
            TryUpdateInfinite(20.999f, 209);
            AwardCollectible("coin-during-pause");
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(
                _infiniteCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 10"));
            Assert.That(
                _infiniteTotalScoreText.text,
                Is.EqualTo("Total Score: 139"));

            SetGameState(E_GameState.Playing);
            SetUIState(E_UIState.StageHud);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 20"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 209"));
            Assert.That(
                _infiniteCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 20"));
            Assert.That(
                _infiniteTotalScoreText.text,
                Is.EqualTo("Total Score: 229"));
        }

        [UnityTest]
        public IEnumerator EndingAndEnded_KeepLastDisplayedValues()
        {
            StartRun(E_GameMode.Infinite);
            TryUpdateInfinite(12.999f, 129);
            AwardCollectible("coin-before-end");
            yield return null;

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);
            _runtimeData.InfiniteModeRuntimeData.Clear();
            SetGameState(E_GameState.Ended);
            yield return null;

            Assert.That(_infiniteHud.activeSelf, Is.False);
            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(
                _infiniteCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 10"));
            Assert.That(
                _infiniteTotalScoreText.text,
                Is.EqualTo("Total Score: 139"));
        }

        [UnityTest]
        public IEnumerator RejectedStageRuntime_PreservesInfiniteHud()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(TryUpdateInfinite(12.0f, 120), Is.True);
            yield return null;
            string distance = _distanceText.text;
            GameRuntimeData rejected = new GameRuntimeData();
            rejected.Initialize(E_GameMode.Stage);
            LogAssert.Expect(LogType.Warning,
                "[UIManagementSystem] Only created Infinite runtime data can be displayed.");
            InvokePublicMethod("Initialize", rejected);
            yield return null;
            Assert.That(rejected.IsCreated, Is.False);
            Assert.That(_stageHud.activeSelf, Is.False);
            Assert.That(_infiniteHud.activeSelf, Is.True);
            Assert.That(_distanceText.text, Is.EqualTo(distance));
            Assert.That(GetPrivateField<GameRuntimeData>("_runtimeData"), Is.SameAs(_runtimeData));
        }

        [UnityTest]
        public IEnumerator Retry_NewRuntimeDataResetsHudToZero()
        {
            StartRun(E_GameMode.Infinite);
            Assert.That(TryUpdateInfinite(12.999f, 129), Is.True);
            AwardCollectible("coin-before-retry");
            yield return null;
            Assert.That(_distanceText.text, Is.EqualTo("Distance: 12"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 129"));
            Assert.That(_infiniteCollectibleScoreText.text, Is.EqualTo("Collectible Score: 10"));

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);
            _runtimeData.Clear();
            SetGameState(E_GameState.Ended);

            StartRun(E_GameMode.Infinite);
            yield return null;

            Assert.That(_distanceText.text, Is.EqualTo("Distance: 0"));
            Assert.That(_scoreText.text, Is.EqualTo("Distance Score: 0"));
            Assert.That(
                _infiniteCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 0"));
            Assert.That(
                _infiniteTotalScoreText.text,
                Is.EqualTo("Total Score: 0"));
        }

        [UnityTest]
        public IEnumerator InfiniteCollectibles_KeepDeprecatedStageHudHidden()
        {
            StartRun(E_GameMode.Infinite);
            AwardCollectible("infinite-coin");
            yield return null;
            Assert.That(_stageHud.activeSelf, Is.False);
            Assert.That(_infiniteHud.activeSelf, Is.True);
            Assert.That(_infiniteCollectibleScoreText.text, Is.EqualTo("Collectible Score: 10"));
            Assert.That(_infiniteTotalScoreText.text, Is.EqualTo("Total Score: 10"));
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

        private void AwardCollectible(string collectibleId)
        {
            CollectibleRuntimeData collectibleRuntimeData =
                _runtimeData.CollectibleRuntimeData;
            Assert.That(
                collectibleRuntimeData.TryCreateScope(out long scopeId),
                Is.True);
            Assert.That(
                collectibleRuntimeData.TryRegister(scopeId, collectibleId),
                Is.True);
            Assert.That(
                collectibleRuntimeData.TryCollect(scopeId, collectibleId),
                Is.True);

            InfiniteModeRuntimeData infiniteData =
                _runtimeData.InfiniteModeRuntimeData;
            if (infiniteData != null && infiniteData.IsInitialized &&
                !infiniteData.IsFinalized)
            {
                infiniteData.TryUpdate(
                    infiniteData.ScoringVersion,
                    infiniteData.CurrentDistance,
                    infiniteData.BaseDistanceScore,
                    infiniteData.MomentumBonus,
                    infiniteData.CurrentScore,
                    collectibleRuntimeData.CurrentScore,
                    SaturatedAdd(
                        infiniteData.CurrentScore,
                        collectibleRuntimeData.CurrentScore),
                    infiniteData.CurrentMomentumMultiplier,
                    infiniteData.MaximumMomentumMultiplier,
                    infiniteData.MomentumRemainingDuration,
                    infiniteData.MomentumDuration);
            }
        }

        private void SetUIState(E_UIState uiState)
        {
            _runtimeData.SetUIState(uiState);
            InvokePublicMethod("SetUIState", uiState);
        }

        private bool TryUpdateInfinite(float distance, int baseDistanceScore)
        {
            return TryUpdateInfinite(
                distance,
                baseDistanceScore,
                0,
                1.0,
                0.0,
                0.0);
        }

        private bool TryUpdateInfinite(
            float distance,
            int baseDistanceScore,
            int momentumBonus,
            double multiplier,
            double remainingDuration,
            double duration)
        {
            int collectibleScore = _runtimeData.CollectibleRuntimeData.CurrentScore;
            int distanceScore = SaturatedAdd(
                baseDistanceScore,
                momentumBonus);
            return _runtimeData.InfiniteModeRuntimeData.TryUpdate(
                ScoringVersion.Current,
                distance,
                baseDistanceScore,
                momentumBonus,
                distanceScore,
                collectibleScore,
                SaturatedAdd(distanceScore, collectibleScore),
                multiplier,
                multiplier,
                remainingDuration,
                duration);
        }

        private static int SaturatedAdd(int first, int second)
        {
            return first > int.MaxValue - second
                ? int.MaxValue
                : first + second;
        }

        private T GetPrivateField<T>(string name)
        {
            FieldInfo field = _uiManagementSystem.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(_uiManagementSystem);
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
