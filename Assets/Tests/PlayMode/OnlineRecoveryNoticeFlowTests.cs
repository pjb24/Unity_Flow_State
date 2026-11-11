using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public sealed class OnlineRecoveryNoticeFlowTests
    {
        private sealed class Store : ILocalSaveFileStore
        {
            public string Contents;
            public bool FailWrite;
            public bool HasSave => Contents != null;
            public bool TryRead(out string contents) { contents = Contents; return HasSave; }
            public bool TryWriteAtomically(string contents)
            { if (FailWrite) return false; Contents = contents; return true; }
        }
        private sealed class Authentication : IOnlineAuthenticationGateway
        {
            public int Calls;
            public readonly TaskCompletionSource<OnlineAuthenticationResult> Completion = new TaskCompletionSource<OnlineAuthenticationResult>();
            public Task<OnlineAuthenticationResult> TryAuthenticateAsync() { Calls++; return Completion.Task; }
        }
        private sealed class Records : IOnlineRecordRepository
        {
            public int Queries;
            public Task<OnlineSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate) { throw new InvalidOperationException(); }
            public Task<OnlineLeaderboardResult> GetPersonalBestAsync(RecordBoardKey key) { throw new InvalidOperationException(); }
            public Task<OnlineLeaderboardResult> GetTopAsync(RecordBoardKey key, int limit = 10)
            { Queries++; return Task.FromResult(new OnlineLeaderboardResult { status = "Success" }); }
            public Task<OnlineLeaderboardResult> GetAroundAsync(RecordBoardKey key, int limit = 7)
            { return GetTopAsync(key, limit); }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ContinueClosesBeforeAuthentication_QueriesWaitAndRepeatedContinueJoins(bool authenticates)
        {
            GameObject host = new GameObject("DelayedNoticeProbe"); host.SetActive(false);
            GameObject panel = new GameObject("MemoryNoticePanel");
            try
            {
                MonoBehaviour game = Add(host, "GameSystem"), ui = Add(host, "UIManagementSystem");
                Store store = new Store(); LocalRecordRepository local = CreateLocal(store);
                Authentication auth = new Authentication(); Records records = new Records();
                Set(ui, "_onlineRecoveryNoticePanel", panel);
                Set(game, "_uiManagementSystem", ui); Set(game, "_localRecordRepository", local);
                Set(game, "_onlineRepository", records);
                Set(game, "_onlineRecords", new OnlineRecordCoordinator(local.AccountId, local, auth, records));
                Call(game, "OpenOnlineRecoveryNoticeFromSettings");
                Assert.That(panel.activeSelf, Is.True);
                Call(game, "ConfirmOnlineRecoveryNotice");
                Assert.That(panel.activeSelf, Is.False, "The notice must close before the network response.");
                Assert.That(local.OnlineAccount.HasConfirmedRecoveryNotice, Is.True);
                Assert.That(auth.Calls, Is.EqualTo(1));
                Task<bool> preparation = (Task<bool>)Get(game, "_onlineRecoveryPreparation");
                Assert.That(preparation.IsCompleted, Is.False);
                Task<OnlineLeaderboardResult> query = (Task<OnlineLeaderboardResult>)game.GetType()
                    .GetMethod("QueryLeaderboardAsync", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(game, new object[] { true });
                Assert.That(query.IsCompleted, Is.False); Assert.That(records.Queries, Is.Zero);
                Call(game, "OpenOnlineRecoveryNoticeFromSettings"); Call(game, "ConfirmOnlineRecoveryNotice");
                Assert.That(panel.activeSelf, Is.False); Assert.That(auth.Calls, Is.EqualTo(1));
                auth.Completion.SetResult(new OnlineAuthenticationResult(authenticates, authenticates ? "synthetic-player" : string.Empty));
                OnlineLeaderboardResult result = await query;
                bool reachable = Application.internetReachability != NetworkReachability.NotReachable;
                Assert.That(result.IsSuccess, Is.EqualTo(authenticates && reachable));
                Assert.That(records.Queries, Is.EqualTo(authenticates && reachable ? 1 : 0));
                Assert.That(panel.activeSelf, Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); UnityEngine.Object.DestroyImmediate(panel); }
        }

        [Test]
        public void FailedConsentSaveKeepsNoticeAndStartsNoAuthentication()
        {
            GameObject host = new GameObject("FailedConsentNoticeProbe"); host.SetActive(false);
            GameObject panel = new GameObject("MemoryNoticePanel");
            try
            {
                MonoBehaviour game = Add(host, "GameSystem"), ui = Add(host, "UIManagementSystem");
                Store store = new Store(); LocalRecordRepository local = CreateLocal(store); store.FailWrite = true;
                Authentication auth = new Authentication();
                Set(ui, "_onlineRecoveryNoticePanel", panel); Set(game, "_uiManagementSystem", ui);
                Set(game, "_localRecordRepository", local);
                Set(game, "_onlineRecords", new OnlineRecordCoordinator(local.AccountId, local, auth, new Records()));
                Call(game, "OpenOnlineRecoveryNoticeFromSettings"); Call(game, "ConfirmOnlineRecoveryNotice");
                Assert.That(panel.activeSelf, Is.True); Assert.That(auth.Calls, Is.Zero);
                Assert.That(local.OnlineAccount.HasConfirmedRecoveryNotice, Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); UnityEngine.Object.DestroyImmediate(panel); }
        }

        [UnityTest]
        public IEnumerator FirstLeaderboardContinueClosesNoticeAndEntersLoadingBeforeAuthentication()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            yield return null;
            Type type = Type.GetType("FlowState.Runtime.Systems.GameSystem, Assembly-CSharp");
            MonoBehaviour game = (MonoBehaviour)GameObject.Find("GameSystem").GetComponent(type);
            Assert.That(Get(game, "_onlineRecords"), Is.Null, "Scene must boot under the existing PlayMode isolation fixture.");
            LocalRecordRepository local = (LocalRecordRepository)Get(game, "_localRecordRepository");
            Authentication auth = new Authentication(); Records records = new Records();
            Set(game, "_hasIsolatedLocalState", false);
            Set(game, "_onlineRecords", new OnlineRecordCoordinator(local.AccountId, local, auth, records));
            Set(game, "_onlineRepository", records);
            MonoBehaviour ui = (MonoBehaviour)Get(game, "_uiManagementSystem");
            GameObject panel = (GameObject)Get(ui, "_onlineRecoveryNoticePanel");
            try
            {
                type.GetMethod("RequestNavigationSelection").Invoke(game, new object[] { FlowState.Runtime.Core.E_NavigationItem.Leaderboard });
                Assert.That(panel.activeSelf, Is.True);
                Call(game, "ConfirmOnlineRecoveryNotice");
                Assert.That(panel.activeSelf, Is.False);
                FlowState.Runtime.Core.GameNavigationState navigation = (FlowState.Runtime.Core.GameNavigationState)Get(game, "_navigationState");
                Assert.That(navigation.CurrentScreen, Is.EqualTo(FlowState.Runtime.Core.E_NavigationScreen.Leaderboard));
                LeaderboardViewState state = (LeaderboardViewState)Get(game, "_leaderboardViewState");
                Assert.That(state.TopState, Is.EqualTo(E_LeaderboardQueryState.Loading));
                Assert.That(state.AroundState, Is.EqualTo(E_LeaderboardQueryState.Loading));
                Assert.That(auth.Calls, Is.EqualTo(1)); Assert.That(records.Queries, Is.Zero);
                auth.Completion.SetResult(new OnlineAuthenticationResult(true, "synthetic-player"));
                yield return null;
                Assert.That(panel.activeSelf, Is.False);
            }
            finally
            {
                if (!auth.Completion.Task.IsCompleted) auth.Completion.SetResult(new OnlineAuthenticationResult(false, string.Empty));
            }
        }

        [TestCase("NoPersonalRecord", "No online record yet.")]
        [TestCase("", "No records.")]
        public void EmptySectionDistinguishesPersonalAbsenceFromEmptyBoard(string reason, string expected)
        {
            // Resolve the actual production UI type without a compile-time Assembly-CSharp reference.
            Type ui = Type.GetType("FlowState.Runtime.Systems.UIManagementSystem, Assembly-CSharp");
            string result = (string)ui.GetMethod("FormatLeaderboardSection", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { "AROUND YOU", E_LeaderboardQueryState.Empty,
                    new OnlineLeaderboardResult { status = "Success", reason = reason }, FlowState.Runtime.Core.E_GameMode.Stage });
            Assert.That(result, Does.Contain(expected)); Assert.That(result, Does.Not.Contain("Error:"));
        }

        private static LocalRecordRepository CreateLocal(Store store)
        {
            LocalRecordRepository local = new LocalRecordRepository(store); local.TryLoad(out LocalSaveData ignored);
            Assert.That(local.TrySave(local.CreateSaveData("synthetic-device", new LocalSettingsData(100, false, null), false)), Is.True);
            return local;
        }
        private static MonoBehaviour Add(GameObject host, string name)
        { return (MonoBehaviour)host.AddComponent(Type.GetType("FlowState.Runtime.Systems." + name + ", Assembly-CSharp")); }
        private static object Get(object target, string name)
        { return target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target); }
        private static void Set(object target, string name, object value)
        { target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value); }
        private static void Call(object target, string name) { target.GetType().GetMethod(name).Invoke(target, null); }
    }
}
