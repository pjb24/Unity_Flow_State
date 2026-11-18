using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class ResultMenuIntegrationTests
    {
        private const string SceneName = "SampleScene";

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _uiInputSystem;
        private MonoBehaviour _uiManagementSystem;
        private MonoBehaviour _runtimeDataSystem;

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
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);

            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _uiInputSystem = FindRequiredBehaviour(
                "UIInputSystem",
                "UIInputSystem");
            _uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");

            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return ProductionSceneGameModeTestUtility.UnloadProductionScene();
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase((E_GameMode)999)]
        public void DirectResultCandidate_UnsupportedMode_KeepsRecordsAndPresentation(E_GameMode mode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            System.Type type = _gameSystem.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            object runtime = type.GetField("_runtimeData", flags).GetValue(_gameSystem);
            Assert.That(runtime, Is.Not.Null);
            Assert.That(type.GetField("_localSaveData", flags).GetValue(_gameSystem), Is.Not.Null);
            MemoryRecordRepository memory = new MemoryRecordRepository();
            SetPrivateField(_gameSystem, "_recordSubmissionService", new RecordSubmissionService(memory));
            SetPrivateField(_gameSystem, "_candidateRuntimeData", null);
            object before = GetPropertyValue(_gameSystem, "ResultRecordPresentation");
            ResultData retired = new ResultData(E_StageResultType.Cleared, 12, 0);
            FieldInfo modeField = typeof(ResultData).GetField("<GameMode>k__BackingField", flags);
            Assert.That(modeField, Is.Not.Null);
            modeField.SetValue(retired, mode);
            MethodInfo store = type.GetMethod("TryStoreCurrentResultCandidate", flags);
            Assert.That(store, Is.Not.Null);
            store.Invoke(_gameSystem, new object[] { retired });
            Assert.That(memory.CreatePendingSnapshot(), Is.Empty);
            Assert.That(memory.CreatePersonalBestSnapshot(), Is.Empty);
            Assert.That(GetPropertyValue(_gameSystem, "ResultRecordPresentation"), Is.SameAs(before));
            Assert.That(type.GetField("_candidateRuntimeData", flags).GetValue(_gameSystem), Is.Null);
            Assert.That(type.GetField("_runtimeData", flags).GetValue(_gameSystem), Is.SameAs(runtime));
        }

        [TestCase(E_GameMode.Stage, 1)]
        [TestCase(E_GameMode.Infinite, 1)]
        [TestCase((E_GameMode)999, 2)]
        public async Task UnsupportedRecordPresentation_CannotDisplayRetryQueryOrRefresh(E_GameMode mode, int version)
        {
            RecordSubmissionCandidate candidate = CreateRecordCandidate(mode, version);
            ResultRecordPresentation original = new ResultRecordPresentation(false, null,
                E_ResultSubmissionState.NotAvailable, string.Empty, null);
            InvokePublicMethod(_uiManagementSystem, "SetResultRecordPresentation", original);
            TMPro.TMP_Text status = ReadPrivate<TMPro.TMP_Text>(_uiManagementSystem, "_resultRecordStatusText");
            UnityEngine.UI.Button retry = ReadPrivate<UnityEngine.UI.Button>(_uiManagementSystem, "_resultSubmissionRetryButton");
            string before = status.text; bool active = retry.gameObject.activeSelf;
            object selection = GetPropertyValue(_gameSystem, "CurrentNavigationSelection");
            ResultRecordPresentation unsupported = new ResultRecordPresentation(true, candidate,
                E_ResultSubmissionState.Pending, "StageModeUnsupported", Best(60000));
            SetPrivateField(_gameSystem, "_resultRecordPresentation", unsupported);
            DeferredRecords fake = new DeferredRecords();
            SetPrivateField(_gameSystem, "_onlineRepository", fake);
            InvokePublicMethod(_uiManagementSystem, "SetResultRecordPresentation", unsupported);
            InvokePublicMethod(_gameSystem, "RetryPendingSubmission");
            InvokePrivate(_gameSystem, "RefreshResultRecordPresentation");
            InvokePrivate(_gameSystem, "ApplyResultRecordPresentation");
            await (Task)InvokePrivate(_gameSystem, "RefreshResultOnlineBestAsync", new object[] { null });
            Assert.That(fake.Queries, Is.Zero);
            Assert.That(status.text, Is.EqualTo(before));
            Assert.That(retry.gameObject.activeSelf, Is.EqualTo(active));
            Assert.That(GetPropertyValue(_gameSystem, "CurrentNavigationSelection"), Is.EqualTo(selection));
            Assert.That(GetPropertyValue(_gameSystem, "ResultRecordPresentation"), Is.SameAs(unsupported));
            Assert.That(ReadPrivate<bool>(_gameSystem, "_isCurrentResultRetryRunning"), Is.False);
        }

        [Test]
        public void InfiniteRecordPresentation_KeepsPendingRetryAndSeparateOnlineBest()
        {
            RecordSubmissionCandidate candidate = CreateRecordCandidate(E_GameMode.Infinite, ScoringVersion.Current);
            TMPro.TMP_Text status = ReadPrivate<TMPro.TMP_Text>(_uiManagementSystem, "_resultRecordStatusText");
            UnityEngine.UI.Button retry = ReadPrivate<UnityEngine.UI.Button>(_uiManagementSystem, "_resultSubmissionRetryButton");
            InvokePublicMethod(_uiManagementSystem, "SetResultRecordPresentation", new ResultRecordPresentation(
                true, candidate, E_ResultSubmissionState.Pending, string.Empty, Best(321)));
            Assert.That(status.text, Does.Contain("Local best: New"));
            Assert.That(status.text, Does.Contain("Submission: Pending (retry available)"));
            Assert.That(status.text, Does.Contain("Online best: #1 321"));
            Assert.That(retry.gameObject.activeSelf, Is.True);
            InvokePublicMethod(_uiManagementSystem, "SetResultRecordPresentation", new ResultRecordPresentation(
                true, candidate, E_ResultSubmissionState.Submitted, string.Empty, Best(321)));
            Assert.That(status.text, Does.Contain("Submission: Submitted"));
            Assert.That(status.text, Does.Contain("Online best: #1 321"));
            Assert.That(retry.gameObject.activeSelf, Is.False);
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase((E_GameMode)999)]
        public async Task UnsupportedCurrentMode_CannotDisplayOrQueryRecord(E_GameMode mode)
        {
            DeferredRecords fake = new DeferredRecords();
            SetPrivateField(_gameSystem, "_onlineRepository", fake);
            SetPrivateField(_gameSystem, "_resultRecordPresentation", new ResultRecordPresentation(
                false, null, E_ResultSubmissionState.NotAvailable, string.Empty, null));
            SetPrivateField(_gameSystem, "_selectedGameMode", mode);
            // The ResultSystem is detached only in this direct old-selection fixture.
            SetPrivateField(_gameSystem, "_resultSystem", null);
            SetPrivateField(_uiManagementSystem, "_currentGameMode", mode);
            TMPro.TMP_Text status = ReadPrivate<TMPro.TMP_Text>(_uiManagementSystem, "_resultRecordStatusText");
            string before = status.text;
            InvokePublicMethod(_uiManagementSystem, "SetResultRecordPresentation", new ResultRecordPresentation(
                true, null, E_ResultSubmissionState.Submitted, string.Empty, Best(60000)));
            await (Task)InvokePrivate(_gameSystem, "RefreshResultOnlineBestAsync", new object[] { null });
            Assert.That(status.text, Is.EqualTo(before));
            Assert.That(fake.Queries, Is.Zero);
        }

        [Test]
        public async Task MismatchedExpectedCandidate_DoesNotInvalidateCurrentPersonalBest()
        {
            DeferredRecords fake = new DeferredRecords();
            SetPrivateField(_gameSystem, "_onlineRepository", fake);
            LeaderboardViewState state = (LeaderboardViewState)GetPropertyValue(_gameSystem, "LeaderboardViewState");
            OnlineLeaderboardResult original = Best(321);
            state.CompletePersonalBestRequest(state.BeginPersonalBestRequest(), original);
            RecordSubmissionCandidate stale = CreateRecordCandidate(E_GameMode.Infinite, ScoringVersion.Current);
            await (Task)InvokePrivate(_gameSystem, "RefreshResultOnlineBestAsync", new object[] { stale });
            Assert.That(fake.Queries, Is.Zero);
            Assert.That(state.PersonalBestResult, Is.SameAs(original));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task DelayedPersonalBest_AfterResultChangesOrNavigation_IsDiscarded(bool leaveScreen)
        {
            DeferredRecords fake = new DeferredRecords();
            SetPrivateField(_gameSystem, "_onlineRepository", fake);
            LeaderboardViewState state = (LeaderboardViewState)GetPropertyValue(_gameSystem, "LeaderboardViewState");
            state.Invalidate();
            Task request = (Task)InvokePrivate(_gameSystem, "RefreshResultOnlineBestAsync", new object[] { null });
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                await request;
                Assert.That(fake.Queries, Is.Zero);
                Assert.That(state.PersonalBestResult.reason, Is.EqualTo("NotReachable"));
                return;
            }
            Assert.That(fake.Queries, Is.EqualTo(1)); Assert.That(request.IsCompleted, Is.False);
            if (leaveScreen) InvokePublicMethod(_gameSystem, "SelectMainMenu");
            else SetPrivateField(_gameSystem, "_resultRecordPresentation", new ResultRecordPresentation(
                false, CreateRecordCandidate(E_GameMode.Infinite, ScoringVersion.Current),
                E_ResultSubmissionState.Pending, string.Empty, null));
            object before = GetPropertyValue(_gameSystem, "ResultRecordPresentation");
            fake.Completion.SetResult(Best(999));
            await request;
            Assert.That(state.PersonalBestResult, Is.Null);
            Assert.That(GetPropertyValue(_gameSystem, "ResultRecordPresentation"), Is.SameAs(before));
            if (leaveScreen) AssertNavigationScreen("MainMenu");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task DelayedLeaderboard_AfterNavigation_DoesNotCompleteHiddenSection(bool top)
        {
            DeferredRecords fake = new DeferredRecords();
            SetPrivateField(_gameSystem, "_onlineRepository", fake);
            LeaderboardViewState state = (LeaderboardViewState)GetPropertyValue(_gameSystem, "LeaderboardViewState");
            state.Invalidate();
            InvokePublicMethod(_gameSystem, top ? "RefreshLeaderboardTop" : "RefreshLeaderboardAround");
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Assert.That(fake.Queries, Is.Zero);
                Assert.That(top ? state.TopState : state.AroundState, Is.EqualTo(E_LeaderboardQueryState.Offline));
                return;
            }
            Assert.That(fake.Queries, Is.EqualTo(1));
            InvokePublicMethod(_gameSystem, "SelectMainMenu");
            fake.Completion.SetResult(Best(999));
            await Task.Yield();
            Assert.That(top ? state.TopResult : state.AroundResult, Is.Null);
            AssertNavigationScreen("MainMenu");
        }

        private sealed class DeferredRecords : IOnlineRecordRepository
        {
            public int Queries;
            public readonly TaskCompletionSource<OnlineLeaderboardResult> Completion = new TaskCompletionSource<OnlineLeaderboardResult>();
            public Task<OnlineSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate)
            { Assert.Fail("Presentation tests must never submit records."); return null; }
            public Task<OnlineLeaderboardResult> GetPersonalBestAsync(RecordBoardKey key) { return Query(key); }
            public Task<OnlineLeaderboardResult> GetTopAsync(RecordBoardKey key, int limit = 10)
            { Assert.That(limit, Is.EqualTo(10)); return Query(key); }
            public Task<OnlineLeaderboardResult> GetAroundAsync(RecordBoardKey key, int limit = 7)
            { Assert.That(limit, Is.EqualTo(7)); return Query(key); }
            private Task<OnlineLeaderboardResult> Query(RecordBoardKey key)
            {
                Assert.That(key.GameMode, Is.EqualTo(E_GameMode.Infinite));
                Assert.That(key.RulesVersion, Is.EqualTo(ScoringVersion.Current));
                Queries++; return Completion.Task;
            }
        }

        private static OnlineLeaderboardResult Best(long score)
        {
            return new OnlineLeaderboardResult { status = "Success", entries = new[] {
                new OnlineLeaderboardEntry { rank = 1, score = score } } };
        }

        private static RecordSubmissionCandidate CreateRecordCandidate(E_GameMode mode, int version)
        {
            const string id = "00000000-0000-4000-8000-000000000007";
            if (mode == E_GameMode.Infinite && version == ScoringVersion.Current)
            {
                Assert.That(RecordSubmissionPolicy.TryCreateInfiniteCandidate("synthetic-device", id,
                    ScoringVersion.Current, 60000, 60, 0, 60, 0, 60, 1.0,
                    new InfiniteScoreLimit(ScoringVersion.Current, 2, 10, 3, 2, 1, 5), out RecordSubmissionCandidate valid), Is.True);
                return valid;
            }
            RecordBoardKey board;
            if (mode == E_GameMode.Stage) Assert.That(RecordBoardKey.TryCreateStage("stage-001", version, out board), Is.True);
            else Assert.That(RecordBoardKey.TryCreateInfinite(version, out board), Is.True);
            RecordSubmissionCandidate archived = (RecordSubmissionCandidate)System.Activator.CreateInstance(
                typeof(RecordSubmissionCandidate), BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { "synthetic-device", id, board, 60L, 60000L, 60, 0, 60, 0, 60, 1.0, 1000L }, null);
            if (mode != E_GameMode.Stage && mode != E_GameMode.Infinite)
                typeof(RecordSubmissionCandidate).GetField("<GameMode>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(archived, mode);
            return archived;
        }

        private static T ReadPrivate<T>(object target, string name)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, name); return (T)field.GetValue(target);
        }

        private static object InvokePrivate(object target, string name, params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null, name); return method.Invoke(target, arguments);
        }

        [UnityTest]
        public IEnumerator ResultMenu_DefaultsToRetryAndMainMenuButtonReturnsToMenu()
        {
            AssertNavigationSelection("Retry");
            AssertSelectedGameObject("RetryButton");

            FindResultMenuButton("MainMenuButton").GetComponent<
                UnityEngine.UI.Button>().onClick.Invoke();
            AssertNavigationScreen("MainMenu");

            yield return null;
        }


        [UnityTest]
        public IEnumerator ResultMenu_CancelInput_DoesNotExecuteSelection()
        {
            SetPrivateField(_uiInputSystem, "_isCancelPressed", true);

            yield return null;

            AssertGameState("Ended");
            AssertNavigationSelection("Retry");
            Assert.That(
                GetInputStateProperty<bool>("IsCancelPressed"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator ResultMenu_SubmitRetry_StartsNewInfiniteRunOnce()
        {
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);
            SubmitSelectedRetryButton();

            yield return null;

            AssertGameState("Playing");
            AssertRuntimeMode(E_GameMode.Infinite);
            Assert.That(GetInputStateProperty<bool>("IsSubmitPressed"), Is.False);
            object restartedRuntimeData = GetPropertyValue(_runtimeDataSystem, "RuntimeData");
            yield return null;
            Assert.That(GetPropertyValue(_runtimeDataSystem, "RuntimeData"),
                Is.SameAs(restartedRuntimeData));
        }

        [UnityTest]
        public IEnumerator ResultMenu_MouseClickRetry_StartsNewInfiniteRunOnce()
        {
            GameObject retryButton = FindResultMenuButton("RetryButton");
            retryButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            AssertGameState("Playing");
            AssertRuntimeMode(E_GameMode.Infinite);
            object restartedRuntimeData = GetPropertyValue(_runtimeDataSystem, "RuntimeData");

            yield return null;

            AssertGameState("Playing");
            Assert.That(GetPropertyValue(_runtimeDataSystem, "RuntimeData"),
                Is.SameAs(restartedRuntimeData));
        }

        [UnityTest]
        public IEnumerator InfiniteResult_UISubmitRetry_StartsInfiniteRun()
        {
            RestartEndedInMode(E_GameMode.Infinite);
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);
            SubmitSelectedRetryButton();

            yield return null;

            AssertGameState("Playing");
            AssertRuntimeMode(E_GameMode.Infinite);
            Assert.That(GetInputStateProperty<bool>("IsSubmitPressed"), Is.False);
        }

        [UnityTest]
        public IEnumerator InfiniteResult_MouseClickRetry_StartsInfiniteRun()
        {
            RestartEndedInMode(E_GameMode.Infinite);
            GameObject retryButton = FindResultMenuButton("RetryButton");
            retryButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();

            yield return null;

            AssertGameState("Playing");
            AssertRuntimeMode(E_GameMode.Infinite);
        }

        [UnityTest]
        public IEnumerator ResultClickAndSubmit_RetryExecutesOnlyOnce()
        {
            GameObject retryButton = FindResultMenuButton("RetryButton");
            SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);
            retryButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();

            object restartedRuntimeData = GetPropertyValue(
                _runtimeDataSystem,
                "RuntimeData");
            AssertGameState("Playing");
            ExecuteEvents.Execute(retryButton, new BaseEventData(EventSystem.current),
                ExecuteEvents.submitHandler);

            yield return null;

            AssertGameState("Playing");
            Assert.That(
                GetPropertyValue(_runtimeDataSystem, "RuntimeData"),
                Is.SameAs(restartedRuntimeData));
            Assert.That(GetInputStateProperty<bool>("IsSubmitPressed"), Is.False);
        }

        private void SubmitSelectedRetryButton()
        {
            Assert.That(EventSystem.current, Is.Not.Null);
            GameObject retryButton = FindResultMenuButton("RetryButton");
            Assert.That(EventSystem.current.currentSelectedGameObject,
                Is.SameAs(retryButton));
            Assert.That(ExecuteEvents.Execute(
                EventSystem.current.currentSelectedGameObject,
                new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler), Is.True);
        }

        private void RestartEndedInMode(E_GameMode gameMode)
        {
            ProductionSceneGameModeTestUtility.RestartInMode(gameMode);
            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _uiInputSystem = FindRequiredBehaviour(
                "UIInputSystem",
                "UIInputSystem");
            _uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            InvokePublicMethod(_gameSystem, "EndGame");
        }

        private void AssertRuntimeMode(E_GameMode expectedMode)
        {
            object runtimeData = GetPropertyValue(
                _runtimeDataSystem,
                "RuntimeData");
            Assert.That(runtimeData, Is.Not.Null);
            Assert.That(
                GetObjectPropertyValue(runtimeData, "GameMode"),
                Is.EqualTo(expectedMode));
        }

        private object GetObjectPropertyValue(
            object target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private void AssertNavigationSelection(string expectedSelection)
        {
            Assert.That(
                GetPropertyValue(_gameSystem, "CurrentNavigationSelection")
                    .ToString(),
                Is.EqualTo(expectedSelection));
        }

        private void AssertNavigationScreen(string expectedScreen)
        {
            Assert.That(
                GetPropertyValue(_gameSystem, "CurrentNavigationScreen")
                    .ToString(),
                Is.EqualTo(expectedScreen));
        }

        private void AssertGameState(string expectedState)
        {
            object gameState = GetPropertyValue(
                _gameSystem,
                "CurrentGameState");

            Assert.That(gameState.ToString(), Is.EqualTo(expectedState));
        }

        private void AssertSelectedGameObject(string expectedName)
        {
            Assert.That(EventSystem.current, Is.Not.Null);
            Assert.That(
                EventSystem.current.currentSelectedGameObject,
                Is.Not.Null);
            Assert.That(
                EventSystem.current.currentSelectedGameObject.name,
                Is.EqualTo(expectedName));
        }

        private T GetInputStateProperty<T>(string propertyName)
        {
            object inputState = InvokePublicMethod(
                _uiInputSystem,
                "GetInputState");
            PropertyInfo property = inputState.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(inputState);
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

        private GameObject FindResultMenuButton(string buttonName)
        {
            GameObject resultPanel = FindSceneGameObject("ResultPanel");

            foreach (Transform child in
                     resultPanel.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == buttonName)
                {
                    return child.gameObject;
                }
            }

            Assert.Fail($"{buttonName} was not found under ResultPanel.");
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

        private object GetPropertyValue(
            MonoBehaviour targetBehaviour,
            string propertyName)
        {
            PropertyInfo property = targetBehaviour.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return property.GetValue(targetBehaviour);
        }

        private object InvokePublicMethod(
            MonoBehaviour targetBehaviour,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(targetBehaviour, arguments);
        }

        private void SetPrivateField(
            MonoBehaviour targetBehaviour,
            string fieldName,
            object value)
        {
            FieldInfo field = targetBehaviour.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(targetBehaviour, value);
        }
    }
}
