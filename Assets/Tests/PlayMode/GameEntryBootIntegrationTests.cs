using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class GameEntryBootIntegrationTests
    {
        private const string SceneName = "SampleScene";

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

        [UnityTest]
        public IEnumerator Boot_OpensMainMenuWithoutCreatingRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            MonoBehaviour playerInputSystem = FindBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            MonoBehaviour stageSystem = FindBehaviour("StageSystem", "StageSystem");
            MonoBehaviour movementSystem = FindBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
            MonoBehaviour cameraFollow = FindBehaviour("CameraRig", "CameraFollow");

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.MainMenu));
            Assert.That(
                GetProperty<E_GameState>(gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.None));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);
            Assert.That(
                GetProperty<bool>(playerInputSystem, "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(GetProperty<bool>(stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetProperty<bool>(movementSystem, "IsRunning"), Is.False);
            Assert.That(GetProperty<bool>(cameraFollow, "IsFollowing"), Is.False);
        }

        [UnityTest]
        public IEnumerator Play_CreatesInfiniteOnlyAfterGuideStartRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            RestoreTutorialIncomplete(gameSystem);

            InvokeNavigationSelection(gameSystem, E_NavigationItem.Play);
            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);

            // Retired play selections cannot bypass the guide or start a Stage Run.
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Stage);
            InvokeNavigationSelection(gameSystem, (E_NavigationItem)999);
            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(
                GetProperty<E_GameState>(gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.None));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);

            InvokeNavigationSelection(gameSystem, E_NavigationItem.StartRun);

            Assert.That(
                GetProperty<E_GameState>(gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.Playing));
            object runtimeData = GetProperty<object>(
                runtimeDataSystem,
                "RuntimeData");
            Assert.That(runtimeData, Is.Not.Null);
            Assert.That(
                GetObjectProperty<E_GameMode>(runtimeData, "GameMode"),
                Is.EqualTo(E_GameMode.Infinite));
        }

        [UnityTest]
        public IEnumerator CompletedTutorial_Play_StartsInfiniteOnceWithLegacyInspectorSelection()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtime = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            GameNavigationState navigation = (GameNavigationState)GetField(game, "_navigationState");
            navigation.RestoreAutomaticHowToPlayCompleted(true);
            SetField(game, "_selectedGameMode", E_GameMode.Stage);

            InvokePublic(game, "SelectPlay");
            object run = GetProperty<object>(runtime, "RuntimeData");
            Assert.That(run, Is.Not.Null);
            Assert.That(GetObjectProperty<E_GameMode>(run, "GameMode"), Is.EqualTo(E_GameMode.Infinite));
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.Playing));
            Assert.That(navigation.CurrentRunId, Is.EqualTo(1));
            Assert.That(navigation.HasAutomaticHowToPlayCompleted, Is.True);

            InvokePublic(game, "SelectPlay");
            InvokePublic(game, "SelectStartRun");
            InvokePublic(game, "SelectStage");
            yield return null;
            InvokePublic(game, "SelectPlay");
            InvokePublic(game, "SelectStartRun");
            Assert.That(GetProperty<object>(runtime, "RuntimeData"), Is.SameAs(run));
            Assert.That(navigation.CurrentRunId, Is.EqualTo(1));
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.Playing));
        }

        [UnityTest]
        public IEnumerator StartGame_LegacyInspector_OpensGuideAndPersistsCompletionOnce()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtime = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            GameNavigationState navigation = (GameNavigationState)GetField(game, "_navigationState");
            navigation.RestoreAutomaticHowToPlayCompleted(false);
            SetField(game, "_selectedGameMode", E_GameMode.Stage);
            InvokePublic(game, "StartGame");
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(navigation.PendingGameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(GetProperty<bool>(runtime, "HasRuntimeData"), Is.False);
            Assert.That(navigation.CurrentRunId, Is.Zero);
            InvokePublic(game, "SelectStartRun");
            object run = GetProperty<object>(runtime, "RuntimeData");
            Assert.That(run, Is.Not.Null);
            Assert.That(GetObjectProperty<E_GameMode>(run, "GameMode"), Is.EqualTo(E_GameMode.Infinite));
            object save = GetField(game, "_localSaveData");
            Assert.That(GetObjectProperty<bool>(save, "HasCompletedTutorial"), Is.True);
            LocalRecordRepository repository = (LocalRecordRepository)GetField(game, "_localRecordRepository");
            Assert.That(repository.TryLoad(out LocalSaveData restoredSave), Is.True);
            Assert.That(restoredSave.HasCompletedTutorial, Is.True);
            InvokePublic(game, "SelectStartRun");
            yield return null;
            InvokePublic(game, "SelectStartRun");
            Assert.That(navigation.CurrentRunId, Is.EqualTo(1));
            Assert.That(GetProperty<object>(runtime, "RuntimeData"), Is.SameAs(run));
            Assert.That(navigation.HasAutomaticHowToPlayCompleted, Is.True);
        }

        [UnityTest]
        public IEnumerator ExplicitStageAndUnknownRequest_DoNotChangeMenuOrCreateRuntime()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtime = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            GameNavigationState navigation = (GameNavigationState)GetField(game, "_navigationState");
            foreach (E_GameMode mode in new[] { E_GameMode.Stage, (E_GameMode)999 })
            {
                Assert.That((bool)InvokePrivate(game, "BeginRunFromMenu", mode), Is.False);
                Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.MainMenu));
                Assert.That(navigation.CurrentSelection, Is.EqualTo(E_NavigationItem.Play));
                Assert.That(navigation.IsRunStartRequested, Is.False);
                Assert.That(navigation.HasPendingGameMode, Is.False);
                Assert.That(navigation.CurrentRunId, Is.Zero);
                Assert.That(GetProperty<bool>(runtime, "HasRuntimeData"), Is.False);
            }
            // A direct initialization call without a navigation request has no effects.
            InvokePrivate(game, "StartRequestedRun");
            Assert.That(GetProperty<bool>(runtime, "HasRuntimeData"), Is.False);
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.MainMenu));

            SetField(game, "_selectedGameMode", (E_GameMode)999);
            LogAssert.Expect(LogType.Warning, "[GameSystem] Game Mode is invalid.");
            InvokePublic(game, "StartGame");
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.MainMenu));
            Assert.That(navigation.CurrentRunId, Is.Zero);
            Assert.That(GetProperty<bool>(runtime, "HasRuntimeData"), Is.False);
        }

        [UnityTest]
        public IEnumerator PauseSettingsBack_PreservesTheCurrentRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            StartInfiniteRun(gameSystem);
            object runtimeData = GetProperty<object>(
                runtimeDataSystem,
                "RuntimeData");

            InvokePublic(gameSystem, "PauseGame");
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Settings);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.Settings));
            Assert.That(
                GetProperty<object>(runtimeDataSystem, "RuntimeData"),
                Is.SameAs(runtimeData));

            InvokeNavigationSelection(gameSystem, E_NavigationItem.Back);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.Pause));
            Assert.That(
                GetProperty<E_NavigationItem>(
                    gameSystem,
                    "CurrentNavigationSelection"),
                Is.EqualTo(E_NavigationItem.Settings));
        }

        [UnityTest]
        public IEnumerator StageRequests_WhilePlayingAndPaused_PreserveProductionPatternAndRun()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeSystem = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            MonoBehaviour stage = (MonoBehaviour)GetField(game, "_stageSystem");
            MonoBehaviour infinite = (MonoBehaviour)GetField(game, "_infiniteModeSystem");
            StartInfiniteRun(game);
            GameRuntimeData runtime = GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData");
            GameNavigationState navigation = (GameNavigationState)GetField(game, "_navigationState");
            long runId = navigation.CurrentRunId;
            GameObject stageRoot = (GameObject)GetField(stage, "_stageModeRoot");
            GameObject infiniteRoot = (GameObject)GetField(stage, "_infiniteModeRoot");
            object pattern = GetField(stage, "_infiniteMapPattern");
            Assert.That(pattern, Is.Not.Null);

            foreach (bool paused in new[] { false, true })
            {
                if (paused) InvokePublic(game, "PauseGame");
                int scopes = runtime.CollectibleRuntimeData.ActiveScopeCount;
                int registered = runtime.CollectibleRuntimeData.RegisteredCount;
                int score = runtime.CollectibleRuntimeData.CurrentScore;
                object endedListeners = GetField(stage, "StageEnded");

                MethodInfo create = runtimeSystem.GetType().GetMethod("CreateRuntimeData", new[] { typeof(E_GameMode) });
                Assert.That(create, Is.Not.Null);
                LogAssert.Expect(LogType.Warning, "[RuntimeDataSystem] Only Infinite Mode runtime data can be created.");
                Assert.That(create.Invoke(runtimeSystem, new object[] { E_GameMode.Stage }), Is.Null);
                MethodInfo initialize = stage.GetType().GetMethod("Initialize", new[] { typeof(E_GameMode) });
                Assert.That(initialize, Is.Not.Null);
                LogAssert.Expect(LogType.Warning, "[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.");
                Assert.That((bool)initialize.Invoke(stage, new object[] { E_GameMode.Stage }), Is.False);
                MethodInfo initializeInfinite = infinite.GetType().GetMethod("Initialize", new[] { typeof(E_GameMode) });
                Assert.That(initializeInfinite, Is.Not.Null);
                LogAssert.Expect(LogType.Warning, "[InfiniteModeSystem] Only Infinite Mode can be initialized.");
                Assert.That((bool)initializeInfinite.Invoke(infinite, new object[] { E_GameMode.Stage }), Is.False);
                InvokePrivate(game, "StartRequestedRun");

                Assert.That(GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData"), Is.SameAs(runtime));
                Assert.That(runtime.GameState, Is.EqualTo(paused ? E_GameState.Paused : E_GameState.Playing));
                Assert.That(navigation.CurrentRunId, Is.EqualTo(runId));
                Assert.That(GetField(stage, "_infiniteMapPattern"), Is.SameAs(pattern));
                Assert.That(GetField(stage, "StageEnded"), Is.EqualTo(endedListeners));
                Assert.That(stageRoot.activeSelf, Is.False);
                Assert.That(infiniteRoot.activeSelf, Is.True);
                Assert.That(GetProperty<bool>(stage, "IsPlaying"), Is.True);
                Assert.That(GetProperty<bool>(stage, "IsPaused"), Is.EqualTo(paused));
                Assert.That(GetProperty<bool>(infinite, "IsPlaying"), Is.True);
                Assert.That(GetProperty<bool>(infinite, "IsPaused"), Is.EqualTo(paused));
                Assert.That(runtime.CollectibleRuntimeData.ActiveScopeCount, Is.EqualTo(scopes));
                Assert.That(runtime.CollectibleRuntimeData.RegisteredCount, Is.EqualTo(registered));
                Assert.That(runtime.CollectibleRuntimeData.CurrentScore, Is.EqualTo(score));
            }
            InvokePublic(game, "ResumeGame");
            Assert.That(runtime.GameState, Is.EqualTo(E_GameState.Playing));
            Assert.That(navigation.CurrentRunId, Is.EqualTo(runId));
        }

        [UnityTest]
        public IEnumerator PauseMainMenuConfirmation_CancelPreservesAndConfirmClearsRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            StartInfiniteRun(gameSystem);
            object runtimeData = GetProperty<object>(
                runtimeDataSystem,
                "RuntimeData");

            InvokePublic(gameSystem, "PauseGame");
            InvokeNavigationSelection(gameSystem, E_NavigationItem.MainMenu);
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Cancel);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.Pause));
            Assert.That(
                GetProperty<object>(runtimeDataSystem, "RuntimeData"),
                Is.SameAs(runtimeData));

            InvokeNavigationSelection(gameSystem, E_NavigationItem.MainMenu);
            InvokeNavigationSelection(gameSystem, E_NavigationItem.MainMenu);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.MainMenu));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator ProductionHelper_RejectsRetiredModeBeforeTouchingCurrentRun()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeSystem = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            object runtime = GetProperty<object>(runtimeSystem, "RuntimeData");
            GameNavigationState navigation = (GameNavigationState)GetField(game, "_navigationState");
            long runId = navigation.CurrentRunId;
            Assert.Throws<AssertionException>(() => ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Stage));
            Assert.Throws<AssertionException>(() => ProductionSceneGameModeTestUtility.RestartInMode((E_GameMode)999));
            Assert.That(GetProperty<object>(runtimeSystem, "RuntimeData"), Is.SameAs(runtime));
            Assert.That(navigation.CurrentRunId, Is.EqualTo(runId));
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.Playing));
        }

        [UnityTest]
        public IEnumerator PauseAndResultRetryAndContextStart_KeepInfiniteAndReplaceTimerAndRun()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeSystem = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            MonoBehaviour timerSystem = (MonoBehaviour)GetField(game, "_timerSystem");
            MonoBehaviour resultSystem = (MonoBehaviour)GetField(game, "_resultSystem");
            MonoBehaviour infinite = (MonoBehaviour)GetField(game, "_infiniteModeSystem");
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            MonoBehaviour ui = (MonoBehaviour)GetField(game, "_uiManagementSystem");
            GameNavigationState navigation = (GameNavigationState)GetField(game, "_navigationState");
            GameRuntimeData previous = GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData");
            Assert.That(previous.CollectibleRuntimeData.TryCreateScope(out long scope), Is.True);
            Assert.That(previous.CollectibleRuntimeData.TryRegister(scope, "retry-reset"), Is.True);
            Assert.That(previous.CollectibleRuntimeData.TryCollect(scope, "retry-reset"), Is.True);
            IDictionary timers = (IDictionary)GetField(timerSystem, "_timers");
            TimerRuntimeData oldTimer = (TimerRuntimeData)timers[E_TimerKey.InfiniteRunTimer];
            long runId = navigation.CurrentRunId;
            InvokePublic(game, "PauseGame");
            Assert.That(oldTimer.State, Is.EqualTo(E_TimerState.Paused));
            InvokePublic(game, "SelectRetry");
            Assert.That(GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData"), Is.SameAs(previous));
            Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.PauseRetryConfirmation));
            InvokePublic(game, "SelectPauseConfirmation");
            GameRuntimeData current = GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData");
            Assert.That(current, Is.Not.SameAs(previous));
            Assert.That(current.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(current.CollectibleRuntimeData.CurrentScore, Is.Zero);
            Assert.That(previous.IsCreated, Is.False);
            Assert.That(navigation.CurrentRunId, Is.EqualTo(++runId));
            Assert.That(oldTimer.State, Is.EqualTo(E_TimerState.Stopped));
            Assert.That(timers[E_TimerKey.InfiniteRunTimer], Is.Not.SameAs(oldTimer));

            foreach (string restart in new[] { "SelectRetry", "StartGame" })
            {
                oldTimer = (TimerRuntimeData)timers[E_TimerKey.InfiniteRunTimer];
                UnityEngine.Rigidbody body = (UnityEngine.Rigidbody)GetField(infinite, "_playerRigidbody");
                body.position = new Vector3(body.position.x, (float)GetField(infinite, "_fallThresholdY") - 1.0f, body.position.z);
                InvokePrivate(infinite, "ProcessFallThreshold");
                Assert.That(navigation.CurrentScreen, Is.EqualTo(E_NavigationScreen.Result));
                ResultData result = GetProperty<ResultData>(resultSystem, "CurrentResultData");
                Assert.That(result, Is.Not.Null);
                Assert.That(result.GameMode, Is.EqualTo(E_GameMode.Infinite));
                Assert.That(GetObjectProperty<string>(GetField(ui, "_infiniteResultTotalScoreText"), "text"),
                    Is.EqualTo(ResultTextFormatter.FormatTotalScore(result.TotalScore)));
                Assert.That(timers.Contains(E_TimerKey.InfiniteRunTimer), Is.False);
                Assert.That(oldTimer.State, Is.EqualTo(E_TimerState.Stopped));
                InvokePublic(game, restart);
                current = GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData");
                Assert.That(current.GameMode, Is.EqualTo(E_GameMode.Infinite));
                Assert.That(current.InfiniteModeRuntimeData.TotalScore, Is.Zero);
                Assert.That(GetProperty<bool>(resultSystem, "HasResultData"), Is.False);
                Assert.That(GetObjectProperty<string>(GetField(ui, "_infiniteResultTotalScoreText"), "text"), Is.Empty);
                Assert.That(navigation.CurrentRunId, Is.EqualTo(++runId));
                Assert.That(((TimerRuntimeData)timers[E_TimerKey.InfiniteRunTimer]).State, Is.EqualTo(E_TimerState.Running));
                Assert.That(timers[E_TimerKey.InfiniteRunTimer], Is.Not.SameAs(oldTimer));
                Assert.That(timers.Contains(E_TimerKey.PlayTimer), Is.False);
            }
        }

        [UnityTest]
        public IEnumerator RetiredRuntimeEndCallback_IsRejectedBeforeTimerAndResultChanges()
        {
            yield return LoadProductionScene();
            MonoBehaviour game = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeSystem = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            StartInfiniteRun(game);
            GameRuntimeData runtime = GetProperty<GameRuntimeData>(runtimeSystem, "RuntimeData");
            MonoBehaviour timerSystem = (MonoBehaviour)GetField(game, "_timerSystem");
            MonoBehaviour resultSystem = (MonoBehaviour)GetField(game, "_resultSystem");
            IDictionary timers = (IDictionary)GetField(timerSystem, "_timers");
            object timer = timers[E_TimerKey.InfiniteRunTimer];
            FieldInfo mode = typeof(GameRuntimeData).GetField("_gameMode", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(mode, Is.Not.Null);
            try
            {
                mode.SetValue(runtime, E_GameMode.Stage);
                LogAssert.Expect(LogType.Warning, "[GameSystem] Only an Infinite Run can produce a gameplay result.");
                InvokePrivate(game, "HandleStageEnded");
                Assert.That(GetProperty<E_GameState>(game, "CurrentGameState"), Is.EqualTo(E_GameState.Playing));
                Assert.That(GetProperty<bool>(resultSystem, "HasResultData"), Is.False);
                Assert.That(timers[E_TimerKey.InfiniteRunTimer], Is.SameAs(timer));
                Assert.That(((TimerRuntimeData)timer).State, Is.EqualTo(E_TimerState.Running));
                Assert.That(runtime.IsCreated, Is.True);
            }
            finally { mode.SetValue(runtime, E_GameMode.Infinite); }
        }

        private IEnumerator LoadProductionScene()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!operation.isDone)
            {
                yield return null;
            }

            yield return null;
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

            Assert.Fail($"{objectName} was not found in the loaded Scene.");
            return null;
        }

        private void InvokeNavigationSelection(
            MonoBehaviour gameSystem,
            E_NavigationItem item)
        {
            MethodInfo method = gameSystem.GetType().GetMethod(
                "RequestNavigationSelection",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(E_NavigationItem) },
                null);
            Assert.That(method, Is.Not.Null);
            method.Invoke(gameSystem, new object[] { item });
        }

        private void StartInfiniteRun(MonoBehaviour gameSystem)
        {
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Play);

            if (GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen") ==
                E_NavigationScreen.AutomaticHowToPlay)
            {
                InvokeNavigationSelection(gameSystem, E_NavigationItem.StartRun);
            }

            Assert.That(
                GetProperty<E_GameState>(gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.Playing));
        }

        private void InvokePublic(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
        }

        private object GetField(MonoBehaviour target, string name)
        {
            FieldInfo field = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return field.GetValue(target);
        }

        private void SetField(MonoBehaviour target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private object InvokePrivate(MonoBehaviour target, string name, params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }

        private void RestoreTutorialIncomplete(MonoBehaviour gameSystem)
        {
            FieldInfo field = gameSystem.GetType().GetField(
                "_navigationState",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            GameNavigationState navigationState =
                (GameNavigationState)field.GetValue(gameSystem);
            navigationState.RestoreAutomaticHowToPlayCompleted(false);
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
    }
}
