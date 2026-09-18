using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class InfiniteModeIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const float FallThresholdY = -3.0f;

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _uiInputSystem;
        private MonoBehaviour _playerMovementSystem;
        private MonoBehaviour _stageSystem;
        private MonoBehaviour _infiniteModeSystem;
        private MonoBehaviour _timerSystem;
        private MonoBehaviour _resultSystem;
        private MonoBehaviour _uiManagementSystem;
        private MonoBehaviour _cameraFollow;
        private StageGoal _stageGoal;
        private InfiniteMapPattern _mapPattern;
        private InfinitePatternBoundary _secondBoundary;
        private GameObject _player;
        private GameObject _startPoint;
        private GameObject _stageModeRoot;
        private GameObject _infiniteModeRoot;
        private GameObject _cameraRig;
        private Rigidbody _playerRigidbody;
        private Collider _playerCollider;
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
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return new WaitForFixedUpdate();

            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
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
            _infiniteModeSystem = FindRequiredBehaviour(
                "InfiniteModeSystem",
                "InfiniteModeSystem");
            _timerSystem = FindRequiredBehaviour(
                "TimerSystem",
                "TimerSystem");
            _resultSystem = FindRequiredBehaviour(
                "ResultSystem",
                "ResultSystem");
            _uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            _cameraFollow = FindRequiredBehaviour(
                "CameraRig",
                "CameraFollow");
            _stageGoal = FindSceneGameObject("Goal")
                .GetComponent<StageGoal>();
            _mapPattern = FindSceneGameObject("InfiniteMapPattern")
                .GetComponent<InfiniteMapPattern>();
            _secondBoundary = FindSceneGameObject("Slot_1")
                .GetComponentInChildren<InfinitePatternBoundary>(true);
            _player = FindSceneGameObject("Player");
            _startPoint = FindSceneGameObject("StartPoint");
            _stageModeRoot = FindSceneGameObject("StageModeRoot");
            _infiniteModeRoot = FindSceneGameObject("InfiniteModeRoot");
            _cameraRig = FindSceneGameObject("CameraRig");
            _playerRigidbody = _player.GetComponent<Rigidbody>();
            _playerCollider = _player.GetComponent<Collider>();
            _finalDistanceText = new GameObject(
                "InfiniteModeIntegrationTests.FinalDistanceText")
                .AddComponent<TextMeshProUGUI>();
            _finalScoreText = new GameObject(
                "InfiniteModeIntegrationTests.FinalScoreText")
                .AddComponent<TextMeshProUGUI>();
            _infiniteResultBaseDistanceScoreText = new GameObject(
                "InfiniteModeIntegrationTests.BaseDistanceScoreText")
                .AddComponent<TextMeshProUGUI>();
            _infiniteResultMomentumBonusText = new GameObject(
                "InfiniteModeIntegrationTests.MomentumBonusText")
                .AddComponent<TextMeshProUGUI>();
            _infiniteResultCollectibleScoreText = new GameObject(
                "InfiniteModeIntegrationTests.CollectibleScoreText")
                .AddComponent<TextMeshProUGUI>();
            _infiniteResultTotalScoreText = new GameObject(
                "InfiniteModeIntegrationTests.TotalScoreText")
                .AddComponent<TextMeshProUGUI>();
            _infiniteResultMaximumMomentumText = new GameObject(
                "InfiniteModeIntegrationTests.MaximumMomentumText")
                .AddComponent<TextMeshProUGUI>();
            SetPrivateField(
                _uiManagementSystem,
                "_finalDistanceText",
                _finalDistanceText);
            SetPrivateField(
                _uiManagementSystem,
                "_finalScoreText",
                _finalScoreText);
            SetPrivateField(
                _uiManagementSystem,
                "_infiniteResultBaseDistanceScoreText",
                _infiniteResultBaseDistanceScoreText);
            SetPrivateField(
                _uiManagementSystem,
                "_infiniteResultMomentumBonusText",
                _infiniteResultMomentumBonusText);
            SetPrivateField(
                _uiManagementSystem,
                "_infiniteResultCollectibleScoreText",
                _infiniteResultCollectibleScoreText);
            SetPrivateField(
                _uiManagementSystem,
                "_infiniteResultTotalScoreText",
                _infiniteResultTotalScoreText);
            SetPrivateField(
                _uiManagementSystem,
                "_infiniteResultMaximumMomentumText",
                _infiniteResultMaximumMomentumText);

            Assert.That(_stageGoal, Is.Not.Null);
            Assert.That(_mapPattern, Is.Not.Null);
            Assert.That(_secondBoundary, Is.Not.Null);
            Assert.That(_playerRigidbody, Is.Not.Null);
            Assert.That(_playerCollider, Is.Not.Null);

            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;

            SetPrivateField(
                _gameSystem,
                "_selectedGameMode",
                E_GameMode.Infinite);
            SetInfiniteTiming(100.0f, 0.5f);
            InvokePublicMethod(_gameSystem, "StartGame");
            yield return null;
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator InfiniteStart_DoesNotCreatePlayTimer()
        {
            bool hasPlayTimer = (bool)InvokePublicMethod(
                _timerSystem,
                "HasTimer",
                E_TimerKey.PlayTimer);

            Assert.That(hasPlayTimer, Is.False);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InfiniteStart_GoalAndPattern_DoNotEndRun()
        {
            AssertInfinitePlayingState();

            Assert.That(GetPrivateField<bool>(
                _mapPattern, "_hasPendingRequest"), Is.True);
            _playerRigidbody.position = new Vector3(24.0f, 1.5f, 0.0f);
            Physics.SyncTransforms();

            InvokePrivateMethod(
                _stageGoal,
                "OnTriggerEnter",
                _playerCollider);
            InvokePrivateMethod(
                _secondBoundary,
                "OnTriggerEnter",
                _playerCollider);
            yield return null;

            AssertInfinitePlayingState();
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
            Assert.That(
                GetBoolProperty(_resultSystem, "HasResultData"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator WorldRebase_ProductionScenePreservesRunStateAndRelativePositions()
        {
            const float threshold = 880.0f;
            Vector3 playerPosition = new Vector3(threshold, 1.5f, 0.0f);
            _playerRigidbody.position = playerPosition;
            _playerRigidbody.linearVelocity = new Vector3(8.0f, -2.0f, 0.0f);
            Physics.SyncTransforms();
            InvokePrivateMethod(_infiniteModeSystem, "ProcessRunMetrics");

            InfiniteModeRuntimeData data =
                GetRuntimeData().InfiniteModeRuntimeData;
            float distance = data.CurrentDistance;
            int score = data.CurrentScore;
            int difficulty = data.CurrentDifficultyLevel;
            Vector3 playerVelocity = _playerRigidbody.linearVelocity;
            Vector3 worldOffset = _infiniteModeRoot.transform.position -
                                  _playerRigidbody.position;
            Vector3 cameraOffset = _cameraRig.transform.position -
                                   _playerRigidbody.position;
            string currentPatternId = _mapPattern.CurrentPatternId;
            int advanceCount = _mapPattern.AdvanceCount;

            Assert.That(InvokePrivateBoolean(
                _infiniteModeSystem, "ProcessWorldRebase"), Is.True);

            Assert.That(_playerRigidbody.position.x,
                Is.EqualTo(0.0f).Within(0.001f));
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(playerVelocity));
            Assert.That(_infiniteModeRoot.transform.position -
                _playerRigidbody.position, Is.EqualTo(worldOffset));
            Assert.That(_cameraRig.transform.position -
                _playerRigidbody.position, Is.EqualTo(cameraOffset));
            Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDistance,
                Is.EqualTo(distance));
            Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(score));
            Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDifficultyLevel,
                Is.EqualTo(difficulty));
            Assert.That(_mapPattern.CurrentPatternId, Is.EqualTo(currentPatternId));
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(advanceCount));
            Assert.That((double)GetProperty(
                _infiniteModeSystem, "CumulativeRebaseOffset"),
                Is.EqualTo(threshold));
            yield return null;
        }

        [UnityTest]
        public IEnumerator RepeatedWorldRebase_ProductionScenePreservesRunState()
        {
            const float threshold = 880.0f;
            string currentPatternId = _mapPattern.CurrentPatternId;
            int advanceCount = _mapPattern.AdvanceCount;

            for (int rebaseIndex = 1; rebaseIndex <= 2; rebaseIndex++)
            {
                _playerRigidbody.position = new Vector3(
                    threshold,
                    1.5f,
                    0.0f);
                _playerRigidbody.linearVelocity =
                    new Vector3(8.0f, -2.0f, 0.0f);
                Physics.SyncTransforms();
                InvokePrivateMethod(_infiniteModeSystem, "ProcessRunMetrics");

                Vector3 worldOffset = _infiniteModeRoot.transform.position -
                                      _playerRigidbody.position;
                Vector3 cameraOffset = _cameraRig.transform.position -
                                       _playerRigidbody.position;

                InfiniteModeRuntimeData data =
                    GetRuntimeData().InfiniteModeRuntimeData;
                float distance = data.CurrentDistance;
                int score = data.CurrentScore;
                int difficulty = data.CurrentDifficultyLevel;

                Assert.That(InvokePrivateBoolean(
                    _infiniteModeSystem, "ProcessWorldRebase"), Is.True);
                Assert.That(_playerRigidbody.position.x,
                    Is.EqualTo(0.0f).Within(0.001f));
                Assert.That(_infiniteModeRoot.transform.position -
                    _playerRigidbody.position, Is.EqualTo(worldOffset));
                Assert.That(_cameraRig.transform.position -
                    _playerRigidbody.position, Is.EqualTo(cameraOffset));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDistance,
                    Is.EqualTo(distance));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentScore,
                    Is.EqualTo(score));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDifficultyLevel,
                    Is.EqualTo(difficulty));
                Assert.That(_mapPattern.CurrentPatternId,
                    Is.EqualTo(currentPatternId));
                Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(advanceCount));
                Assert.That((double)GetProperty(
                    _infiniteModeSystem, "CumulativeRebaseOffset"),
                    Is.EqualTo(threshold * rebaseIndex));
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator RebaseStressCycle_ProductionSceneKeepsScopesAndRunStateBounded()
        {
            const float threshold = 880.0f;
            const int rebaseCount = 100;
            GameRuntimeData runtimeData = GetRuntimeData();
            int activeScopeCount =
                runtimeData.CollectibleRuntimeData.ActiveScopeCount;
            int registeredCount =
                runtimeData.CollectibleRuntimeData.RegisteredCount;
            string currentPatternId = _mapPattern.CurrentPatternId;
            int advanceCount = _mapPattern.AdvanceCount;

            for (int rebaseIndex = 1;
                 rebaseIndex <= rebaseCount;
                 rebaseIndex++)
            {
                _playerRigidbody.position = new Vector3(
                    threshold,
                    1.5f,
                    0.0f);
                _playerRigidbody.linearVelocity =
                    new Vector3(8.0f, 0.0f, 0.0f);
                Physics.SyncTransforms();
                InvokePrivateMethod(_infiniteModeSystem, "ProcessRunMetrics");

                Vector3 worldOffset = _infiniteModeRoot.transform.position -
                                      _playerRigidbody.position;
                Vector3 cameraOffset = _cameraRig.transform.position -
                                       _playerRigidbody.position;
                InfiniteModeRuntimeData data =
                    GetRuntimeData().InfiniteModeRuntimeData;
                float distance = data.CurrentDistance;
                int score = data.CurrentScore;
                int difficulty = data.CurrentDifficultyLevel;

                Assert.That(InvokePrivateBoolean(
                    _infiniteModeSystem, "ProcessWorldRebase"), Is.True);
                Assert.That(_playerRigidbody.position.x,
                    Is.EqualTo(0.0f).Within(0.001f));
                Assert.That(_infiniteModeRoot.transform.position -
                    _playerRigidbody.position, Is.EqualTo(worldOffset));
                Assert.That(_cameraRig.transform.position -
                    _playerRigidbody.position, Is.EqualTo(cameraOffset));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDistance,
                    Is.EqualTo(distance));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentScore,
                    Is.EqualTo(score));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDifficultyLevel,
                    Is.EqualTo(difficulty));
                Assert.That(_mapPattern.CurrentPatternId,
                    Is.EqualTo(currentPatternId));
                Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(advanceCount));
                Assert.That(GetRuntimeData().CollectibleRuntimeData.ActiveScopeCount,
                    Is.EqualTo(activeScopeCount));
                Assert.That(GetRuntimeData().CollectibleRuntimeData.RegisteredCount,
                    Is.EqualTo(registeredCount));
                Assert.That((double)GetProperty(
                    _infiniteModeSystem, "CumulativeRebaseOffset"),
                    Is.EqualTo(threshold * rebaseIndex));
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator AutomaticPatternRequest_PhysicalBoundary_AdvancesAndRequestsAgain()
        {
            InfinitePatternSelectionState selection = GetPrivateField<
                InfinitePatternSelectionState>(
                _infiniteModeSystem, "_patternSelectionState");
            Assert.That(_mapPattern.AdvanceCount, Is.Zero);
            Assert.That(GetPrivateField<bool>(
                _mapPattern, "_hasPendingRequest"), Is.True);
            Assert.That(GetPrivateField<long>(
                _mapPattern, "_pendingRequestId"), Is.EqualTo(1));
            Assert.That(GetPrivateField<string>(
                _mapPattern, "_pendingPatternId"),
                Is.EqualTo(selection.CurrentPatternId));
            string firstSelectedId = selection.CurrentPatternId;

            _playerRigidbody.position = new Vector3(38.0f, 1.5f, 0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            _playerRigidbody.linearVelocity = new Vector3(8.0f, 0.0f, 0.0f);

            for (int step = 0; step < 40 &&
                 _mapPattern.AdvanceCount == 0; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
            Assert.That(_secondBoundary.IsTriggered, Is.True);
            Assert.That(_mapPattern.CurrentPatternId,
                Is.EqualTo(firstSelectedId));
            yield return new WaitForFixedUpdate();
            Assert.That(GetPrivateField<bool>(
                _mapPattern, "_hasPendingRequest"), Is.True);
            Assert.That(GetPrivateField<long>(
                _mapPattern, "_pendingRequestId"), Is.EqualTo(2));
            Assert.That(_mapPattern.TryAdvance(1), Is.False);
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator ControlledSeed_ProductionSelection_ActivatesDifferentPattern()
        {
            Assert.That(InfinitePatternCatalogFactory.TryCreate(
                out InfinitePatternCatalog catalog), Is.True);
            InfinitePatternSelectionState probe =
                new InfinitePatternSelectionState();
            Assert.That(probe.Initialize(catalog), Is.True);
            int selectedSeed = 0;

            for (int seed = 1; seed <= 32; seed++)
            {
                Assert.That(probe.StartRun(seed), Is.True);
                Assert.That(probe.TrySelectNext(
                    1,
                    E_InfinitePatternDifficulty.D1,
                    out string selectedId), Is.True);
                Assert.That(probe.EndRun(), Is.True);

                if (selectedId == InfinitePatternCatalogFactory.SingleRiseId)
                {
                    selectedSeed = seed;
                    break;
                }
            }

            Assert.That(selectedSeed, Is.Not.Zero);
            InfinitePatternSelectionState selection = GetPrivateField<
                InfinitePatternSelectionState>(
                _infiniteModeSystem, "_patternSelectionState");
            Assert.That(selection.EndRun(), Is.True);
            Assert.That(selection.StartRun(selectedSeed), Is.True);
            Assert.That(_mapPattern.ResetPatterns(), Is.True);
            SetPrivateField(
                _infiniteModeSystem, "_lastAcceptedPatternRequestId", 0);
            SetPrivateField(
                _infiniteModeSystem, "_lastObservedPatternAdvanceCount", 0);
            SetPrivateField(
                _infiniteModeSystem, "_hasPendingPatternRequest", false);
            InvokePrivateMethod(
                _infiniteModeSystem, "ProcessPatternProgression");
            Assert.That(GetPrivateField<string>(
                _mapPattern, "_pendingPatternId"),
                Is.EqualTo(InfinitePatternCatalogFactory.SingleRiseId));

            _playerRigidbody.position = new Vector3(38.0f, 1.5f, 0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            _playerRigidbody.linearVelocity = new Vector3(8.0f, 0.0f, 0.0f);

            for (int step = 0; step < 40 &&
                 _mapPattern.AdvanceCount == 0; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
            Assert.That(_mapPattern.CurrentPatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.SingleRiseId));

            // Hold a view of the joined terrain for the manual visual check.
            RigidbodyConstraints previousConstraints =
                _playerRigidbody.constraints;
            _playerRigidbody.position = new Vector3(73.0f, 1.5f, 0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Physics.SyncTransforms();
            yield return new WaitForSecondsRealtime(5.0f);
            _playerRigidbody.constraints = previousConstraints;
        }

        [UnityTest]
        public IEnumerator PauseResultRetry_PreservesThenResetsAutomaticSelection()
        {
            InfiniteDifficultyState difficulty = GetPrivateField<
                InfiniteDifficultyState>(
                _infiniteModeSystem, "_difficultyState");
            InfinitePatternSelectionState selection = GetPrivateField<
                InfinitePatternSelectionState>(
                _infiniteModeSystem, "_patternSelectionState");
            string selectedId = selection.CurrentPatternId;
            int seed = GetPrivateField<int>(
                _infiniteModeSystem, "_previousRunSeed");

            Assert.That((bool)InvokePublicMethod(
                _gameSystem, "PauseGame"), Is.True);
            InvokePrivateMethod(
                _infiniteModeSystem, "ProcessPatternProgression");
            Assert.That(selection.IsPaused, Is.True);
            Assert.That(difficulty.IsPaused, Is.True);
            Assert.That(selection.CurrentPatternId, Is.EqualTo(selectedId));
            Assert.That(GetPrivateField<long>(
                _mapPattern, "_pendingRequestId"), Is.EqualTo(1));

            Assert.That((bool)InvokePublicMethod(
                _gameSystem, "ResumeGame"), Is.True);
            Assert.That(selection.IsPaused, Is.False);
            Assert.That(difficulty.IsPaused, Is.False);
            Assert.That(selection.CurrentPatternId, Is.EqualTo(selectedId));

            _playerRigidbody.position = new Vector3(
                10000.0f, FallThresholdY - 0.01f, 0.0f);
            Physics.SyncTransforms();
            InvokePrivateMethod(
                _infiniteModeSystem, "ProcessFallThreshold");
            yield return null;
            AssertInfiniteEndedState();
            Assert.That(selection.HasEnded, Is.True);
            Assert.That(difficulty.HasEnded, Is.True);
            int requestIdAtResult = GetPrivateField<int>(
                _infiniteModeSystem, "_lastAcceptedPatternRequestId");
            InvokePrivateMethod(
                _infiniteModeSystem, "ProcessPatternProgression");
            Assert.That(GetPrivateField<int>(
                _infiniteModeSystem, "_lastAcceptedPatternRequestId"),
                Is.EqualTo(requestIdAtResult));

            Assert.That((bool)InvokePublicMethod(
                _gameSystem, "RetryGame"), Is.True);
            AssertInfinitePlayingState();
            Assert.That(_mapPattern.AdvanceCount, Is.Zero);
            Assert.That(_mapPattern.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(_mapPattern.TrailingPatternId, Is.EqualTo("Flat"));
            Assert.That(difficulty.CurrentDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D1));
            Assert.That(difficulty.MaximumForwardDistance, Is.Zero);
            Assert.That(selection.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(selection.ConsecutiveSelectionCount, Is.EqualTo(1));
            Assert.That(GetPrivateField<int>(
                _infiniteModeSystem, "_lastAcceptedPatternRequestId"),
                Is.Zero);
            Assert.That(GetPrivateField<int>(
                _infiniteModeSystem, "_previousRunSeed"), Is.Not.EqualTo(seed));
            yield return new WaitForFixedUpdate();
            Assert.That(GetPrivateField<long>(
                _mapPattern, "_pendingRequestId"), Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator BelowMinimumSpeed_CreatesInfiniteResultData()
        {
            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;

            SetInfiniteTiming(0.0f, Time.fixedDeltaTime);
            InvokePublicMethod(_gameSystem, "StartGame");
            yield return new WaitForFixedUpdate();
            yield return null;

            AssertInfiniteEndedState();
            AssertInfiniteResultData();
        }

        [UnityTest]
        public IEnumerator PlayerFallsAtLargeX_EndsAndStopsPlaySystems()
        {
            _playerRigidbody.position = new Vector3(
                10000.0f,
                FallThresholdY - 0.01f,
                0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
            yield return null;

            AssertInfiniteEndedState();
            ResultData resultData = AssertInfiniteResultData();
            Assert.That(
                resultData.FinalDistance,
                Is.EqualTo(
                    Mathf.Max(
                        0.0f,
                        10000.0f - _startPoint.transform.position.x)));
            Assert.That(resultData.DistanceScore, Is.GreaterThan(0));
            Assert.That(
                resultData.TotalScore,
                Is.EqualTo(
                    resultData.DistanceScore + resultData.CollectibleScore));
        }

        [UnityTest]
        public IEnumerator CollectedCoin_AfterPatternAdvance_ReachesResultScores()
        {
            InfinitePatternSlot first = FindSceneGameObject("Slot_0")
                .GetComponent<InfinitePatternSlot>();
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring pattern), Is.True);
            ScoreCollectible coin = pattern.CollectibleRoot
                .Find("entry-land-01").GetComponent<ScoreCollectible>();
            _playerRigidbody.position = coin.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(coin.IsCollected, Is.True);
            Assert.That(GetRuntimeData().CollectibleRuntimeData.CurrentScore,
                Is.EqualTo(10));

            _playerRigidbody.position = new Vector3(24.0f, 1.5f, 0.0f);
            Physics.SyncTransforms();
            InvokePrivateMethod(
                _secondBoundary, "OnTriggerEnter", _playerCollider);
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
            Assert.That(GetRuntimeData().CollectibleRuntimeData.CurrentScore,
                Is.EqualTo(10));

            _playerRigidbody.position = new Vector3(
                1000.0f, FallThresholdY - 0.01f, 0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
            yield return null;

            ResultData result = AssertInfiniteResultData();
            Assert.That(result.CollectibleScore, Is.EqualTo(10));
            Assert.That(result.TotalScore,
                Is.EqualTo(result.DistanceScore + 10));
            Assert.That(_infiniteResultCollectibleScoreText.text,
                Is.EqualTo("Collectible Score: 10"));
            Assert.That(_infiniteResultTotalScoreText.text,
                Is.EqualTo("Total Score: " + result.TotalScore));
        }

        [UnityTest]
        public IEnumerator SequentialEndRequests_KeepSingleInfiniteResultData()
        {
            _playerRigidbody.position = new Vector3(
                10000.0f,
                FallThresholdY - 0.01f,
                0.0f);
            InvokePrivateMethod(
                _infiniteModeSystem,
                "ProcessFallThreshold");
            ResultData firstResultData = AssertInfiniteResultData();

            InvokePrivateMethod(
                _infiniteModeSystem,
                "ProcessFallThreshold");
            yield return null;

            ResultData secondResultData = AssertInfiniteResultData();
            Assert.That(secondResultData, Is.SameAs(firstResultData));
        }

        [Test]
        public void PatternGroundConnections_ProvideJumpGap()
        {
            InfinitePatternSlot firstSlot = FindSceneGameObject("Slot_0")
                .GetComponent<InfinitePatternSlot>();
            InfinitePatternSlot secondSlot = FindSceneGameObject("Slot_1")
                .GetComponent<InfinitePatternSlot>();
            Assert.That(firstSlot.TryGetCurrentPattern(
                out InfinitePatternAuthoring firstPattern), Is.True);
            Assert.That(secondSlot.TryGetCurrentPattern(
                out InfinitePatternAuthoring secondPattern), Is.True);
            Transform firstEndAnchor = firstPattern.EndAnchor;
            Transform secondStartAnchor = secondPattern.StartAnchor;
            Assert.That(firstPattern.TryGetTerrainCollider(
                firstPattern.TerrainColliderCount - 1,
                out Collider firstGround), Is.True);
            Assert.That(secondPattern.TryGetTerrainCollider(
                0, out Collider secondGround), Is.True);

            float groundGap =
                secondGround.bounds.min.x - firstGround.bounds.max.x;
            float firstAnchorClearance =
                firstEndAnchor.position.x - firstGround.bounds.max.x;
            float secondAnchorClearance =
                secondGround.bounds.min.x - secondStartAnchor.position.x;

            Assert.That(groundGap, Is.EqualTo(4.0f).Within(0.01f));
            Assert.That(
                firstAnchorClearance,
                Is.EqualTo(2.0f).Within(0.01f));
            Assert.That(
                secondAnchorClearance,
                Is.EqualTo(2.0f).Within(0.01f));
            Assert.That(
                firstEndAnchor.position,
                Is.EqualTo(secondStartAnchor.position));
        }

        [UnityTest]
        public IEnumerator ResultMenuRetry_Twice_RestoresIndependentInfiniteRuns()
        {
            Vector3 firstPatternPosition =
                FindSceneGameObject("Slot_0").transform.position;
            Vector3 secondPatternPosition =
                FindSceneGameObject("Slot_1").transform.position;

            ResultData previousResultData = null;

            for (int retryIndex = 0; retryIndex < 2; retryIndex++)
            {
                _playerRigidbody.position = new Vector3(
                    10000.0f + retryIndex * 1000.0f,
                    FallThresholdY - 0.01f,
                    0.0f);
                InvokePrivateMethod(
                    _infiniteModeSystem,
                    "ProcessFallThreshold");
                yield return null;

                AssertInfiniteEndedState();
                ResultData currentResultData = AssertInfiniteResultData();

                if (previousResultData != null)
                {
                    Assert.That(currentResultData, Is.Not.SameAs(previousResultData));
                    Assert.That(
                        currentResultData.FinalDistance,
                        Is.Not.EqualTo(previousResultData.FinalDistance));
                }

                previousResultData = currentResultData;
                SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);
                // Inspect reset before automatic movement advances the new run.
                InvokePrivateMethod(_gameSystem, "ProcessResultMenuInput");

                AssertInfinitePlayingState();
                Assert.That(
                    GetRuntimeData().GameMode,
                    Is.EqualTo(E_GameMode.Infinite));
                Assert.That(
                    GetRuntimeData().InfiniteModeRuntimeData.CurrentDistance,
                    Is.Zero);
                Assert.That(
                    GetRuntimeData().InfiniteModeRuntimeData.CurrentScore,
                    Is.Zero);
                Assert.That(
                    GetRuntimeData().InfiniteModeRuntimeData.IsFinalized,
                    Is.False);
                Assert.That(
                    _playerRigidbody.position,
                    Is.EqualTo(_startPoint.transform.position));
                Assert.That(
                    _playerRigidbody.linearVelocity,
                    Is.EqualTo(Vector3.zero));
                Assert.That(_mapPattern.AdvanceCount, Is.Zero);
                Assert.That(
                    FindSceneGameObject("Slot_0").transform.position,
                    Is.EqualTo(firstPatternPosition));
                Assert.That(
                    FindSceneGameObject("Slot_1").transform.position,
                    Is.EqualTo(secondPatternPosition));
                Assert.That(
                    GetBoolProperty(_resultSystem, "HasResultData"),
                    Is.False);
                yield return new WaitForFixedUpdate();
                Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
            }
        }

        [UnityTest]
        public IEnumerator RestartAfterMovement_InterpolatedBody_UsesPhysicsOriginAndDistance()
        {
            yield return VerifyPhysicsPositionAfterRestart(RigidbodyInterpolation.Interpolate);
        }

        [UnityTest]
        public IEnumerator RestartAfterMovement_NonInterpolatedBody_UsesPhysicsOriginAndDistance()
        {
            yield return VerifyPhysicsPositionAfterRestart(RigidbodyInterpolation.None);
        }

        private IEnumerator VerifyPhysicsPositionAfterRestart(RigidbodyInterpolation interpolation)
        {
            RigidbodyInterpolation previousInterpolation = _playerRigidbody.interpolation;

            try
            {
                _playerRigidbody.interpolation = interpolation;
                for (int step = 0; step < 5; step++)
                {
                    yield return new WaitForFixedUpdate();
                }

                yield return null;
                Assert.That(_playerRigidbody.position.x,
                    Is.GreaterThan(_startPoint.transform.position.x));
                InvokePublicMethod(_gameSystem, "EndGame");
                InvokePublicMethod(_gameSystem, "StartGame");

                FieldInfo distanceField = _infiniteModeSystem.GetType().GetField(
                    "_distanceState", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(distanceField, Is.Not.Null);
                InfiniteDistanceState distanceState =
                    (InfiniteDistanceState)distanceField.GetValue(_infiniteModeSystem);
                float originX = _startPoint.transform.position.x;
                Assert.That(_playerRigidbody.position.x, Is.EqualTo(originX));
                Assert.That(distanceState.OriginWorldX, Is.EqualTo(originX));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDistance, Is.Zero);

                // Read gameplay metrics before a rendered Transform update can mask the source.
                _playerRigidbody.position = new Vector3(originX + 10.0f, 1.5f, 0.0f);
                InvokePrivateMethod(_infiniteModeSystem, "ProcessRunMetrics");
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentDistance,
                    Is.EqualTo(10.0f));
                Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentScore, Is.EqualTo(100));

                _playerRigidbody.position = new Vector3(originX + 10000.0f, FallThresholdY - 0.01f, 0.0f);
                InvokePrivateMethod(_infiniteModeSystem, "ProcessFallThreshold");
                AssertInfiniteEndedState();
                ResultData result = AssertInfiniteResultData();
                Assert.That(result.FinalDistance, Is.EqualTo(10000.0f));
                Assert.That(result.DistanceScore, Is.EqualTo(100000));
                Assert.That(result.TotalScore, Is.EqualTo(100000));
            }
            finally
            {
                _playerRigidbody.interpolation = previousInterpolation;
            }
        }

        private void AssertInfinitePlayingState()
        {
            Assert.That(GetStateName(_gameSystem, "CurrentGameState"), Is.EqualTo("Playing"));
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
            Assert.That(GetBoolProperty(_infiniteModeSystem, "IsPlaying"), Is.True);
            Assert.That(
                GetBoolProperty(_playerInputSystem, "IsPlayerActionMapEnabled"),
                Is.True);
            Assert.That(
                GetBoolProperty(_uiInputSystem, "IsUIActionMapEnabled"),
                Is.True);
            Assert.That(
                GetBoolProperty(_playerMovementSystem, "IsRunning"),
                Is.True);
            Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.True);
            Assert.That(_stageModeRoot.activeSelf, Is.False);
            Assert.That(_infiniteModeRoot.activeSelf, Is.True);
        }

        private void AssertInfiniteEndedState()
        {
            Assert.That(GetStateName(_gameSystem, "CurrentGameState"), Is.EqualTo("Ended"));
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(
                GetBoolProperty(_playerInputSystem, "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(
                GetBoolProperty(_uiInputSystem, "IsUIActionMapEnabled"),
                Is.True);
            Assert.That(
                GetBoolProperty(_playerMovementSystem, "IsRunning"),
                Is.False);
            Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.False);
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
            Assert.That(_playerRigidbody.angularVelocity, Is.EqualTo(Vector3.zero));
            Assert.That(GetBoolProperty(_resultSystem, "HasResultData"), Is.True);
        }

        private ResultData AssertInfiniteResultData()
        {
            ResultData resultData = (ResultData)GetProperty(
                _resultSystem,
                "CurrentResultData");

            Assert.That(resultData, Is.Not.Null);
            Assert.That(resultData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(resultData.HasInfiniteModeResult, Is.True);
            Assert.That(resultData.HasStageResult, Is.False);
            Assert.That(
                resultData.StageResultType,
                Is.EqualTo(E_StageResultType.None));
            Assert.That(resultData.ElapsedTime, Is.Zero);
            Assert.That(resultData.ScoringVersion,
                Is.EqualTo(ScoringVersion.Current));
            Assert.That(resultData.FinalDistance, Is.GreaterThanOrEqualTo(0.0f));
            Assert.That(resultData.BaseDistanceScore, Is.GreaterThanOrEqualTo(0));
            Assert.That(resultData.MomentumBonus, Is.GreaterThanOrEqualTo(0));
            Assert.That(resultData.DistanceScore, Is.GreaterThanOrEqualTo(0));
            Assert.That(resultData.CollectibleScore, Is.GreaterThanOrEqualTo(0));
            Assert.That(resultData.TotalScore, Is.GreaterThanOrEqualTo(0));
            Assert.That(resultData.DistanceScore,
                Is.EqualTo(resultData.BaseDistanceScore + resultData.MomentumBonus));
            Assert.That(resultData.TotalScore,
                Is.EqualTo(resultData.DistanceScore + resultData.CollectibleScore));
            Assert.That(
                _finalDistanceText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatFinalDistance(
                        resultData.FinalDistance)));
            Assert.That(
                _infiniteResultBaseDistanceScoreText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatBaseDistanceScore(
                        resultData.BaseDistanceScore)));
            Assert.That(
                _infiniteResultMomentumBonusText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatMomentumBonus(
                        resultData.MomentumBonus)));
            Assert.That(
                _finalScoreText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatDistanceScore(
                        resultData.DistanceScore)));
            Assert.That(
                _infiniteResultCollectibleScoreText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatCollectibleScore(
                        resultData.CollectibleScore)));
            Assert.That(
                _infiniteResultTotalScoreText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatTotalScore(
                        resultData.TotalScore)));
            Assert.That(
                _infiniteResultMaximumMomentumText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatMaximumMomentumMultiplier(
                        resultData.MaximumMomentumMultiplier)));
            return resultData;
        }

        private void SetInfiniteTiming(
            float startGraceDuration,
            float belowSpeedGraceDuration)
        {
            SetPrivateField(
                _infiniteModeSystem,
                "_startGraceDuration",
                startGraceDuration);
            SetPrivateField(
                _infiniteModeSystem,
                "_belowSpeedGraceDuration",
                belowSpeedGraceDuration);
        }

        private GameRuntimeData GetRuntimeData()
        {
            return (GameRuntimeData)InvokePublicMethod(
                _runtimeDataSystem,
                "GetRuntimeData");
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

            Assert.Fail($"{typeName} was not found on {gameObjectName}.");
            return null;
        }

        private string GetStateName(
            MonoBehaviour target,
            string propertyName)
        {
            return GetProperty(target, propertyName).ToString();
        }

        private bool GetBoolProperty(
            MonoBehaviour target,
            string propertyName)
        {
            return (bool)GetProperty(target, propertyName);
        }

        private object GetProperty(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private object InvokePublicMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }

        private void InvokePrivateMethod(
            object target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(method, Is.Not.Null);
            method.Invoke(target, arguments);
        }

        private bool InvokePrivateBoolean(
            object target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(method, Is.Not.Null);
            return (bool)method.Invoke(target, arguments);
        }

        private void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private T GetPrivateField<T>(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }
    }
}
