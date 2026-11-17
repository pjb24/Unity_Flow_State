using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class StageSystemTests
    {
        private GameObject _playerObject;
        private GameObject _goalObject;
        private GameObject _systemObject;
        private Collider _playerCollider;
        private MonoBehaviour _stageGoal;
        private MonoBehaviour _stageSystem;

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Playing)]
        [TestCase(E_GameState.Paused)]
        [TestCase(E_GameState.Ended)]
        public void UnsupportedInitializeAndStart_PreserveRunRootsListenersAndCollectibles(E_GameState state)
        {
            GameRuntimeData runtime = null;
            ScoreCollectible collectible = null;
            long scope = 0;
            if (state != E_GameState.None)
            {
                Assert.That(InvokeBoolMethod(_stageSystem, "Initialize"), Is.True);
                runtime = new GameRuntimeData();
                runtime.Initialize();
                runtime.SetGameState(state);
                runtime.SetUIState(state == E_GameState.Ended ? E_UIState.Result : E_UIState.StageHud);
                Rigidbody body = _playerObject.AddComponent<Rigidbody>();
                body.useGravity = false;
                Assert.That(InvokeBoolMethod(_stageSystem, "ConfigureCollectibles", runtime, body), Is.True);
                if (state != E_GameState.Ready)
                    Assert.That(InvokeBoolMethod(_stageSystem, "StartStage"), Is.True);

                // An actual bound component/scope makes premature release observable.
                GameObject coin = new GameObject("StageSystemTests.Coin");
                coin.transform.SetParent(_systemObject.transform);
                // ScoreCollectible binding/overlap checks require a sphere trigger.
                SphereCollider trigger = coin.AddComponent<SphereCollider>();
                trigger.isTrigger = true;
                MeshRenderer visual = coin.AddComponent<MeshRenderer>();
                collectible = coin.AddComponent<ScoreCollectible>();
                SetPrivateField(collectible, "_collectibleId", "scope-coin");
                SetPrivateField(collectible, "_triggerCollider", trigger);
                SetPrivateField(collectible, "_visual", visual);
                Assert.That(runtime.CollectibleRuntimeData.TryCreateScope(out scope), Is.True);
                Assert.That(collectible.Bind(runtime, body, scope), Is.True);
                ((IList)GetPrivateField(_stageSystem, "_collectibles")).Add(collectible);
                SetPrivateField(_stageSystem, "_collectibleScopeId", scope);
                if (state == E_GameState.Paused)
                    Assert.That(InvokeBoolMethod(_stageSystem, "PauseStage"), Is.True);
                if (state == E_GameState.Ended)
                    Assert.That(InvokeBoolMethod(_stageSystem, "TryEndInfiniteStage"), Is.True);
            }

            GameObject stageRoot = new GameObject("StageSystemTests.PreservedStageRoot");
            GameObject infiniteRoot = new GameObject("StageSystemTests.PreservedInfiniteRoot");
            stageRoot.transform.SetParent(_systemObject.transform);
            infiniteRoot.transform.SetParent(_systemObject.transform);
            stageRoot.SetActive(false);
            SetPrivateField(_stageSystem, "_stageModeRoot", stageRoot);
            SetPrivateField(_stageSystem, "_infiniteModeRoot", infiniteRoot);
            Assert.That(InvokeBoolMethod(_stageGoal, "Initialize"), Is.True);
            MethodInfo handler = _stageSystem.GetType().GetMethod("HandleGoalReached",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Action goalListener = (Action)Delegate.CreateDelegate(typeof(Action), _stageSystem, handler);
            InvokeMethod(_stageGoal, "AddListener", goalListener);
            object goalListeners = GetPrivateField(_stageGoal, "GoalReached");
            int scopeCount = runtime == null ? 0 : runtime.CollectibleRuntimeData.ActiveScopeCount;
            int registeredCount = runtime == null ? 0 : runtime.CollectibleRuntimeData.RegisteredCount;
            int bindingCount = ((IList)GetPrivateField(_stageSystem, "_collectibles")).Count;

            foreach (E_GameMode mode in new[] { E_GameMode.Stage, (E_GameMode)999 })
            {
                Dictionary<string, object> snapshot = CaptureStageFields();
                LogAssert.Expect(mode == E_GameMode.Stage ? LogType.Warning : LogType.Error,
                    mode == E_GameMode.Stage
                        ? "[StageSystem] Stage Mode is no longer supported. Use Infinite Mode."
                        : "[StageSystem] Game Mode is invalid.");
                Assert.That(InvokeBoolMethod(_stageSystem, "Initialize", mode), Is.False);
                AssertStageFields(snapshot);

                SetPrivateField(_stageSystem, "_currentGameMode", mode);
                Dictionary<string, object> staleModeSnapshot = CaptureStageFields();
                LogAssert.Expect(LogType.Warning, "[StageSystem] Only Infinite Mode can be started.");
                Assert.That(InvokeBoolMethod(_stageSystem, "StartStage"), Is.False);
                AssertStageFields(staleModeSnapshot);
                SetPrivateField(_stageSystem, "_currentGameMode", E_GameMode.Infinite);

                Assert.That(stageRoot.activeSelf, Is.False);
                Assert.That(infiniteRoot.activeSelf, Is.True);
                Assert.That(GetPrivateField(_stageGoal, "GoalReached"), Is.EqualTo(goalListeners));
                Assert.That(((IList)GetPrivateField(_stageSystem, "_collectibles")).Count, Is.EqualTo(bindingCount));
                if (runtime != null)
                {
                    Assert.That(runtime.GameState, Is.EqualTo(state));
                    Assert.That(runtime.UIState, Is.EqualTo(state == E_GameState.Ended ? E_UIState.Result : E_UIState.StageHud));
                    Assert.That(runtime.IsCreated, Is.True);
                    Assert.That(runtime.CollectibleRuntimeData.ActiveScopeCount, Is.EqualTo(scopeCount));
                    Assert.That(runtime.CollectibleRuntimeData.RegisteredCount, Is.EqualTo(registeredCount));
                    Assert.That(collectible.IsBound, Is.EqualTo(state != E_GameState.Ended));
                }
            }
            if (runtime != null && state != E_GameState.Ended)
                Assert.That(runtime.CollectibleRuntimeData.TryCollect(scope, "scope-coin"), Is.True);
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Playing)]
        [TestCase(E_GameState.Paused)]
        [TestCase(E_GameState.Ended)]
        public void RuntimeDataSystem_UnsupportedCreation_ReturnsNullAndPreservesExistingRun(E_GameState state)
        {
            MonoBehaviour system = AddComponentByName(_systemObject, "RuntimeDataSystem");
            GameRuntimeData runtime = null;
            long scope = 0;
            if (state != E_GameState.None)
            {
                runtime = (GameRuntimeData)InvokeMethod(system, "CreateRuntimeData");
                Assert.That(runtime.GameMode, Is.EqualTo(E_GameMode.Infinite));
                runtime.SetGameState(state);
                runtime.SetUIState(state == E_GameState.Ended ? E_UIState.Result : E_UIState.StageHud);
                Assert.That(runtime.CollectibleRuntimeData.TryCreateScope(out scope), Is.True);
                Assert.That(runtime.CollectibleRuntimeData.TryRegister(scope, "preserved"), Is.True);
            }
            foreach (E_GameMode mode in new[] { E_GameMode.Stage, (E_GameMode)999 })
            {
                LogAssert.Expect(LogType.Warning, "[RuntimeDataSystem] Only Infinite Mode runtime data can be created.");
                Assert.That(InvokeMethod(system, "CreateRuntimeData", mode), Is.Null);
                Assert.That(GetPrivateField(system, "_runtimeData"), Is.SameAs(runtime));
                Assert.That(GetBoolProperty(system, "HasRuntimeData"), Is.EqualTo(runtime != null));
                if (runtime != null)
                {
                    Assert.That(runtime.GameState, Is.EqualTo(state));
                    Assert.That(runtime.UIState, Is.EqualTo(state == E_GameState.Ended ? E_UIState.Result : E_UIState.StageHud));
                    Assert.That(runtime.CollectibleRuntimeData.ActiveScopeCount, Is.EqualTo(1));
                    Assert.That(runtime.CollectibleRuntimeData.RegisteredCount, Is.EqualTo(1));
                }
            }
            if (runtime != null)
            {
                Assert.That(runtime.CollectibleRuntimeData.TryCollect(scope, "preserved"), Is.True);
                Assert.That(InvokeMethod(system, "CreateRuntimeData", E_GameMode.Infinite), Is.SameAs(runtime));
            }
            else
            {
                runtime = (GameRuntimeData)InvokeMethod(system, "CreateRuntimeData");
                Assert.That(runtime.IsCreated, Is.True);
                Assert.That(runtime.GameMode, Is.EqualTo(E_GameMode.Infinite));
            }
        }

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("StageSystemTests.Player");
            _playerCollider = _playerObject.AddComponent<CapsuleCollider>();

            _goalObject = new GameObject("StageSystemTests.Goal");
            BoxCollider goalCollider = _goalObject.AddComponent<BoxCollider>();
            goalCollider.isTrigger = true;
            _stageGoal = AddComponentByName(_goalObject, "StageGoal");
            SetPrivateField(_stageGoal, "_playerCollider", _playerCollider);

            _systemObject = new GameObject("StageSystemTests.System");
            _stageSystem = AddComponentByName(_systemObject, "StageSystem");
            SetPrivateField(_stageSystem, "_stageGoal", _stageGoal);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_systemObject);
            UnityEngine.Object.DestroyImmediate(_goalObject);
            UnityEngine.Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void Initialize_WithoutMode_PreparesInfinite()
        {
            bool didInitialize = InvokeBoolMethod(_stageSystem, "Initialize");

            Assert.That(didInitialize, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsInitialized"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
            Assert.That(GetEnumProperty<E_GameMode>(_stageSystem, "CurrentGameMode"), Is.EqualTo(E_GameMode.Infinite));
            Assert.That(GetPrivateField(_stageGoal, "_isInitialized"), Is.False);
        }

        [Test]
        public void Initialize_StageModeWithoutGoal_IsRejectedBeforeGoalValidation()
        {
            SetPrivateField(_stageSystem, "_stageGoal", null);
            LogAssert.Expect(
                LogType.Warning,
                "[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.");

            bool didInitialize = InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Stage);

            Assert.That(didInitialize, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsInitialized"), Is.False);
        }

        [Test]
        public void Initialize_InfiniteModeWithoutGoal_PreparesStage()
        {
            SetPrivateField(_stageSystem, "_stageGoal", null);

            bool didInitialize = InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);

            Assert.That(didInitialize, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsInitialized"), Is.True);
            Assert.That(GetEnumProperty<E_GameMode>(
                _stageSystem,
                "CurrentGameMode"), Is.EqualTo(E_GameMode.Infinite));
        }

        [Test]
        public void Initialize_StageModeWithRoots_DoesNotChangeRoots()
        {
            GameObject stageRoot = new GameObject("StageSystemTests.StageRoot");
            GameObject infiniteRoot =
                new GameObject("StageSystemTests.InfiniteRoot");

            try
            {
                stageRoot.SetActive(false);
                infiniteRoot.SetActive(true);
                SetPrivateField(_stageSystem, "_stageModeRoot", stageRoot);
                SetPrivateField(
                    _stageSystem,
                    "_infiniteModeRoot",
                    infiniteRoot);

                LogAssert.Expect(LogType.Warning,
                    "[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.");
                bool didInitialize = InvokeBoolMethod(
                    _stageSystem,
                    "Initialize",
                    E_GameMode.Stage);

                Assert.That(didInitialize, Is.False);
                Assert.That(stageRoot.activeSelf, Is.False);
                Assert.That(infiniteRoot.activeSelf, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(stageRoot);
                UnityEngine.Object.DestroyImmediate(infiniteRoot);
            }
        }

        [Test]
        public void Initialize_InfiniteModeWithRoots_ActivatesOnlyInfiniteRoot()
        {
            GameObject stageRoot = new GameObject("StageSystemTests.StageRoot");
            GameObject infiniteRoot =
                new GameObject("StageSystemTests.InfiniteRoot");

            try
            {
                stageRoot.SetActive(true);
                infiniteRoot.SetActive(false);
                SetPrivateField(_stageSystem, "_stageModeRoot", stageRoot);
                SetPrivateField(
                    _stageSystem,
                    "_infiniteModeRoot",
                    infiniteRoot);

                bool didInitialize = InvokeBoolMethod(
                    _stageSystem,
                    "Initialize",
                    E_GameMode.Infinite);

                Assert.That(didInitialize, Is.True);
                Assert.That(stageRoot.activeSelf, Is.False);
                Assert.That(infiniteRoot.activeSelf, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(stageRoot);
                UnityEngine.Object.DestroyImmediate(infiniteRoot);
            }
        }

        [Test]
        public void StartStage_SecondRequest_IsRejected()
        {
            InvokeBoolMethod(_stageSystem, "Initialize");
            bool didStart = InvokeBoolMethod(_stageSystem, "StartStage");

            LogAssert.Expect(
                LogType.Warning,
                "[StageSystem] Stage is already running.");
            bool didStartAgain = InvokeBoolMethod(_stageSystem, "StartStage");

            Assert.That(didStart, Is.True);
            Assert.That(didStartAgain, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
        }

        [UnityTest]
        public IEnumerator GoalReached_PlayingStage_ClearsAndEndsOnce()
        {
            PrepareDeprecatedStageSnapshot(true);
            int clearCount = 0;
            int endCount = 0;
            AddListener(_stageSystem, "AddStageClearedListener", () => clearCount++);
            AddListener(_stageSystem, "AddStageEndedListener", () => endCount++);

            InvokeTriggerEnter(_stageGoal, _playerCollider);
            InvokeTriggerEnter(_stageGoal, _playerCollider);
            yield return null;

            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(clearCount, Is.EqualTo(1));
            Assert.That(endCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GoalReached_BeforeStageStart_DoesNotClearStage()
        {
            PrepareDeprecatedStageSnapshot(false);

            InvokeTriggerEnter(_stageGoal, _playerCollider);
            yield return null;

            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [UnityTest]
        public IEnumerator GoalReached_NonPlayerCollider_DoesNotClearStage()
        {
            GameObject otherObject = new GameObject("StageSystemTests.Other");
            Collider otherCollider = otherObject.AddComponent<BoxCollider>();

            try
            {
                PrepareDeprecatedStageSnapshot(true);

                InvokeTriggerEnter(_stageGoal, otherCollider);
                yield return null;

                Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
                Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(otherObject);
            }
        }

        [UnityTest]
        public IEnumerator StartStage_DeprecatedClearedSnapshot_IsRejectedWithoutReset()
        {
            PrepareDeprecatedStageSnapshot(true);
            InvokeTriggerEnter(_stageGoal, _playerCollider);

            LogAssert.Expect(LogType.Warning, "[StageSystem] Only Infinite Mode can be started.");
            bool didRestart = InvokeBoolMethod(_stageSystem, "StartStage");
            yield return null;
            Assert.That(didRestart, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
        }

        [UnityTest]
        public IEnumerator GoalReached_InfiniteMode_DoesNotEndStage()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);
            InvokeBoolMethod(_stageSystem, "StartStage");

            InvokeTriggerEnter(_stageGoal, _playerCollider);
            yield return null;

            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void TryEndInfiniteStage_BeforeStart_IsRejected()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);

            bool didEnd = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");

            Assert.That(didEnd, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void TryEndInfiniteStage_StageMode_IsRejected()
        {
            PrepareDeprecatedStageSnapshot(true);

            bool didEnd = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");

            Assert.That(didEnd, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void TryEndInfiniteStage_PlayingStage_EndsOnce()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);
            InvokeBoolMethod(_stageSystem, "StartStage");
            int endCount = 0;
            AddListener(
                _stageSystem,
                "AddStageEndedListener",
                () => endCount++);

            bool didEnd = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");
            bool didEndAgain = InvokeBoolMethod(
                _stageSystem,
                "TryEndInfiniteStage");

            Assert.That(didEnd, Is.True);
            Assert.That(didEndAgain, Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(endCount, Is.EqualTo(1));
        }

        [Test]
        public void StartStage_AfterInfiniteEnd_ResetsStageState()
        {
            InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Infinite);
            InvokeBoolMethod(_stageSystem, "StartStage");
            InvokeBoolMethod(_stageSystem, "TryEndInfiniteStage");

            bool didRestart = InvokeBoolMethod(_stageSystem, "StartStage");

            Assert.That(didRestart, Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void PauseStage_PlayingStage_BlocksGoalUntilResume()
        {
            PrepareDeprecatedStageSnapshot(true);

            Assert.That(InvokeBoolMethod(_stageSystem, "PauseStage"), Is.True);
            InvokeMethod(_stageSystem, "HandleGoalReached");

            Assert.That(GetBoolProperty(_stageSystem, "IsPaused"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);

            Assert.That(InvokeBoolMethod(_stageSystem, "ResumeStage"), Is.True);
            InvokeMethod(_stageSystem, "HandleGoalReached");

            Assert.That(GetBoolProperty(_stageSystem, "IsPaused"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
        }

        [TestCase(-2.999f, false)]
        [TestCase(-3.0f, true)]
        [TestCase(-3.001f, true)]
        public void FallThreshold_UsesInclusiveRigidbodyPositionBoundary(
            float playerY,
            bool shouldRequestFall)
        {
            PrepareStageFallCheck(playerY);

            InvokeMethod(_stageSystem, "FixedUpdate");

            Assert.That(
                GetBoolProperty(_stageSystem, "HasPendingFall"),
                Is.EqualTo(shouldRequestFall));
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);

            InvokeMethod(_stageSystem, "FixedUpdate");

            Assert.That(
                GetBoolProperty(_stageSystem, "HasEnded"),
                Is.EqualTo(shouldRequestFall));
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
        }

        [Test]
        public void FallThreshold_PausedStage_WaitsUntilResume()
        {
            PrepareStageFallCheck(-3.0f);
            Assert.That(InvokeBoolMethod(_stageSystem, "PauseStage"), Is.True);

            InvokeMethod(_stageSystem, "FixedUpdate");
            InvokeMethod(_stageSystem, "FixedUpdate");

            Assert.That(GetBoolProperty(_stageSystem, "HasPendingFall"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);

            Assert.That(InvokeBoolMethod(_stageSystem, "ResumeStage"), Is.True);
            InvokeMethod(_stageSystem, "FixedUpdate");
            InvokeMethod(_stageSystem, "FixedUpdate");

            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
        }

        [Test]
        public void GoalReached_AfterFallRequestBeforeConfirmation_ClearsStage()
        {
            PrepareStageFallCheck(-3.0f);
            int endCount = 0;
            AddListener(
                _stageSystem,
                "AddStageEndedListener",
                () => endCount++);

            InvokeMethod(_stageSystem, "FixedUpdate");
            InvokeMethod(_stageSystem, "HandleGoalReached");
            InvokeMethod(_stageSystem, "FixedUpdate");

            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(
                GetBoolProperty(_stageSystem, "HasPendingFall"),
                Is.False);
            Assert.That(endCount, Is.EqualTo(1));
        }

        [Test]
        public void FallThreshold_InfiniteMode_DoesNotEndStage()
        {
            PrepareStageFallCheck(-3.0f, E_GameMode.Infinite);

            InvokeMethod(_stageSystem, "FixedUpdate");
            InvokeMethod(_stageSystem, "FixedUpdate");

            Assert.That(GetBoolProperty(_stageSystem, "HasPendingFall"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
        }

        [Test]
        public void StartStage_DeprecatedFallSnapshot_CannotRestart()
        {
            PrepareStageFallCheck(-3.0f);
            InvokeMethod(_stageSystem, "FixedUpdate");
            InvokeMethod(_stageSystem, "FixedUpdate");
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            LogAssert.Expect(LogType.Warning, "[StageSystem] Only Infinite Mode can be started.");
            Assert.That(InvokeBoolMethod(_stageSystem, "StartStage"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Initialize_StageMode_IsRejectedBeforeFallThresholdValidation(float threshold)
        {
            SetPrivateField(_stageSystem, "_fallThresholdY", threshold);
            LogAssert.Expect(
                LogType.Warning,
                "[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.");

            bool didInitialize = InvokeBoolMethod(
                _stageSystem,
                "Initialize",
                E_GameMode.Stage);

            Assert.That(didInitialize, Is.False);
        }

        private void PrepareStageFallCheck(
            float playerY,
            E_GameMode gameMode = E_GameMode.Stage)
        {
            Rigidbody playerRigidbody =
                _playerObject.GetComponent<Rigidbody>();

            if (playerRigidbody == null)
            {
                playerRigidbody = _playerObject.AddComponent<Rigidbody>();
                playerRigidbody.useGravity = false;
            }

            playerRigidbody.position = new Vector3(0.0f, playerY, 0.0f);
            if (gameMode == E_GameMode.Stage)
            {
                // Observe the retired fall component without creating/starting a Stage Run.
                PrepareDeprecatedStageSnapshot(true);
                SetPrivateField(_stageSystem, "_playerRigidbody", playerRigidbody);
                return;
            }

            GameRuntimeData runtimeData = new GameRuntimeData();
            runtimeData.Initialize(gameMode);

            Assert.That(
                InvokeBoolMethod(_stageSystem, "Initialize", gameMode),
                Is.True);
            Assert.That(
                InvokeBoolMethod(
                    _stageSystem,
                    "ConfigureCollectibles",
                    runtimeData,
                    playerRigidbody),
                Is.True);
            Assert.That(
                InvokeBoolMethod(_stageSystem, "StartStage"),
                Is.True);
        }

        private void PrepareDeprecatedStageSnapshot(bool playing)
        {
            // A synthetic observation of the retired component, never a production Run.
            // Stage Initialize/Start rejection is tested independently below.
            Assert.That(InvokeBoolMethod(_stageGoal, "Initialize"), Is.True);
            MethodInfo handler = _stageSystem.GetType().GetMethod("HandleGoalReached",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Action listener = (Action)Delegate.CreateDelegate(typeof(Action), _stageSystem, handler);
            InvokeMethod(_stageGoal, "AddListener", listener);
            SetPrivateField(_stageSystem, "_currentGameMode", E_GameMode.Stage);
            SetPrivateField(_stageSystem, "_isInitialized", true);
            SetPrivateField(_stageSystem, "_isPlaying", playing);
            SetPrivateField(_stageSystem, "_isPaused", false);
            SetPrivateField(_stageSystem, "_isCleared", false);
            SetPrivateField(_stageSystem, "_hasEnded", false);
            SetPrivateField(_stageSystem, "_hasPendingFall", false);
        }

        private MonoBehaviour AddComponentByName(
            GameObject gameObject,
            string typeName)
        {
            Type componentType = FindType(typeName);
            Assert.That(componentType, Is.Not.Null);
            return (MonoBehaviour)gameObject.AddComponent(componentType);
        }

        private Type FindType(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(
                    $"FlowState.Runtime.Systems.{typeName}");

                if (type == null)
                {
                    type = assembly.GetType(
                        $"FlowState.Runtime.Features.{typeName}");
                }

                if (type != null &&
                    typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    return type;
                }
            }

            return null;
        }

        private void SetPrivateField(
            MonoBehaviour target,
            string fieldName,
            object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private object GetPrivateField(MonoBehaviour target, string name)
        {
            FieldInfo field = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return field.GetValue(target);
        }

        private Dictionary<string, object> CaptureStageFields()
        {
            Dictionary<string, object> snapshot = new Dictionary<string, object>();
            foreach (FieldInfo field in _stageSystem.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                snapshot.Add(field.Name, field.GetValue(_stageSystem));
            return snapshot;
        }

        private void AssertStageFields(Dictionary<string, object> snapshot)
        {
            foreach (KeyValuePair<string, object> entry in snapshot)
                Assert.That(GetPrivateField(_stageSystem, entry.Key), Is.EqualTo(entry.Value), entry.Key);
        }

        private bool InvokeBoolMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            return (bool)InvokeMethod(target, methodName, arguments);
        }

        private object InvokeMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = null;

            foreach (MethodInfo candidate in target.GetType().GetMethods(
                         BindingFlags.Instance |
                         BindingFlags.Public |
                         BindingFlags.NonPublic))
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

        private bool GetBoolProperty(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (bool)property.GetValue(target);
        }

        private T GetEnumProperty<T>(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private void AddListener(
            MonoBehaviour target,
            string methodName,
            Action listener)
        {
            InvokeMethod(target, methodName, listener);
        }

        private void InvokeTriggerEnter(
            MonoBehaviour target,
            Collider otherCollider)
        {
            InvokeMethod(target, "OnTriggerEnter", otherCollider);
        }
    }
}
