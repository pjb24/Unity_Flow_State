using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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
    public sealed class AccountTransferUIIsolationTests
    {
        private sealed class FileStore : ILocalSaveFileStore
        {
            public string Contents;
            public bool HasSave => Contents != null;
            public bool TryRead(out string contents) { contents = Contents; return HasSave; }
            public bool TryWriteAtomically(string contents) { Contents = contents; return true; }
        }
        private sealed class Authentication : IOnlineAuthenticationGateway
        {
            public Task<OnlineAuthenticationResult> TryAuthenticateAsync()
            { return Task.FromResult(new OnlineAuthenticationResult(true, "ugs-player")); }
        }
        private sealed class Transport : IOnlineAccountTransport
        {
            public int Starts;
            public int Completes;
            public bool Pending;
            public TaskCompletionSource<OnlineAccountResponse> Delayed;
            public async Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters)
            {
                if (endpoint == "get-public-player-number") return (T)(object)new OnlineAccountResponse
                    { status = "Success", publicPlayerNumber = "0000000001" };
                if (endpoint == "start-account-transfer" || endpoint == "reissue-account-transfer")
                {
                    Starts++; Pending = true;
                    if (Delayed != null) return (T)(object)await Delayed.Task;
                    return (T)(object)Issued();
                }
                if (endpoint == "complete-account-transfer") Completes++;
                if (Pending) return (T)(object)new OnlineAccountResponse { status = "TransferPending", transferStatus = "TransferPending",
                    transferId = "11111111-1111-4111-8111-111111111111", expiresAtMilliseconds = 100000 };
                return (T)(object)new OnlineAccountResponse { status = "Active", transferStatus = "None" };
            }
            public static OnlineAccountResponse Issued()
            { return new OnlineAccountResponse { status = "Success", code = "ABCD-2345", verificationValue = "000000007", expiresAtMilliseconds = 100000 }; }
        }
        private const string SceneName = "SampleScene";
        private const int BootFrameLimit = 120;
        private GameObject _host;
        private GameObject _root;
        private MonoBehaviour _sceneBinding;
        private bool _bindingWasEnabled;
        private CanvasGroup _background;
        private MonoBehaviour _view;
        private Button _open;
        private Button[] _actions;
        private TMP_Text _credentials;
        private TMP_Text _account;
        private TMP_InputField _code;
        private TMP_InputField _verification;
        private Transport _transport;
        private AccountTransferController _controller;
        private LocalRecordRepository _local;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _host = null; _root = null; _view = null; _sceneBinding = null;
            _controller = null; _transport = null; _bindingWasEnabled = false;
            // Verify the existing namespace guard before loading any production behaviour.
            Type gameType = Type.GetType("FlowState.Runtime.Systems.GameSystem, Assembly-CSharp");
            Assert.That(gameType, Is.Not.Null);
            FieldInfo isolation = gameType.GetField("UseIsolatedRecordsForPlayModeTests", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(isolation, Is.Not.Null);
            Assert.That(isolation.GetValue(null), Is.EqualTo(true), "PlayModeRecordIsolationSetup must enable memory-only records before Scene loading.");
            Assert.That(Application.CanStreamedLevelBeLoaded(SceneName), Is.True,
                "Step 8: keep SampleScene in the active Scene List; this test does not create or repair a Scene.");
            yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            Scene scene = SceneManager.GetActiveScene();

            Type viewType = Type.GetType("FlowState.Runtime.Systems.AccountTransferView, Assembly-CSharp"); Assert.That(viewType, Is.Not.Null);
            _view = FindSceneBehaviour(scene, viewType);
            _host = _view.gameObject;
            Type bindingType = Type.GetType("FlowState.Runtime.Systems.AccountTransferUIController, Assembly-CSharp");
            Assert.That(bindingType, Is.Not.Null);
            _sceneBinding = (MonoBehaviour)_host.GetComponent(bindingType);
            Assert.That(_sceneBinding, Is.Not.Null, "Step 8: add AccountTransferUIController to the same host as AccountTransferView.");
            Assert.That(ReadField<MonoBehaviour>(_sceneBinding, "_view"), Is.SameAs(_view), "Step 8: connect the controller's View in Inspector.");
            MonoBehaviour game = ReadField<MonoBehaviour>(_sceneBinding, "_gameSystem");
            Assert.That(game.GetType(), Is.EqualTo(gameType), "Step 8: connect the existing GameSystem.");
            Assert.That(game.gameObject.scene, Is.EqualTo(scene));
            _bindingWasEnabled = _sceneBinding.enabled;
            Assert.That(_bindingWasEnabled, Is.True, "Step 8: enable AccountTransferUIController.");
            // Only the backend binding is replaced; serialized UI references stay untouched.
            _sceneBinding.enabled = false;
            PropertyInfo navigation = game.GetType().GetProperty("CurrentNavigationScreen");
            Assert.That(navigation, Is.Not.Null);
            int frame = 0;
            while (navigation.GetValue(game).ToString() != "MainMenu" && frame++ < BootFrameLimit) yield return null;
            Assert.That(navigation.GetValue(game).ToString(), Is.EqualTo("MainMenu"), "SampleScene did not finish boot.");
            Assert.That(game.GetType().GetMethod("SelectSettings"), Is.Not.Null);
            game.GetType().GetMethod("SelectSettings").Invoke(game, null);
            yield return null;
            Assert.That(_view.isActiveAndEnabled, Is.True, "Step 8: keep AccountTransferHost active when Settings is open.");
            _root = ReadField<GameObject>(_view, "_root");
            _background = ReadField<CanvasGroup>(_view, "_settingsContent");
            _account = ReadField<TMP_Text>(_view, "_accountText");
            _credentials = ReadField<TMP_Text>(_view, "_credentialText");
            _code = ReadField<TMP_InputField>(_view, "_codeInput");
            _verification = ReadField<TMP_InputField>(_view, "_verificationInput");
            _open = ReadField<Button>(_view, "_openButton");
            _actions = ReadField<Button[]>(_view, "_actionButtons");
            TMP_Text status = ReadField<TMP_Text>(_view, "_statusText"); ReadField<GameObject[]>(_view, "_pages");
            Assert.That(_view.GetType().GetMethod("HasValidReferences", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_view, null),
                Is.EqualTo(true), "Step 8: check the View Inspector table, 5 pages, 12 common action buttons and empty persistent OnClick lists.");
            Assert.That(_root.activeSelf, Is.False, "Step 8: save AccountTransferRoot inactive; the test must not repair its initial state.");
            Assert.That(_background.interactable && _background.blocksRaycasts, Is.True);
            Assert.That(_open.isActiveAndEnabled, Is.True);
            Assert.That(_host.GetComponentInParent<Canvas>(), Is.Not.Null, "Step 8: place the host under the existing Canvas.");
            Assert.That(_host.GetComponentInParent<GraphicRaycaster>(), Is.Not.Null);
            ValidateInput(_code); ValidateInput(_verification);
            ValidateText(_account); ValidateText(status); ValidateText(_credentials);
            Assert.That(EventSystem.current, Is.Not.Null, "Step 8: reuse the existing EventSystem.");
            Assert.That(EventSystem.current.gameObject.scene, Is.EqualTo(scene));
            FileStore file = new FileStore { Contents = LocalSaveJsonCodec.Serialize(new LocalSaveData(6, "owner",
                new LocalSettingsData(100, false, null), true, null, new OnlineAccountState(true, "ugs-player"))) };
            LocalRecordRepository local = new LocalRecordRepository(file); local.TryLoad(out LocalSaveData ignored);
            _local = local;
            _transport = new Transport(); OnlineAccountCoordinator account = new OnlineAccountCoordinator(local, new Authentication(), _transport, () => true);
            _controller = new AccountTransferController(local, account, (IAccountTransferView)_view);
            Assert.That(_view.GetType().GetMethod("Initialize").Invoke(_view, new object[] { _controller, null }), Is.EqualTo(true));
            EventSystem.current.SetSelectedGameObject(_open.gameObject);
        }

        [TearDown]
        public void TearDown()
        {
            if (_host != null) _host.SetActive(false);
            if (_controller != null) _controller.Close();
            if (_transport != null && _transport.Delayed != null) _transport.Delayed.TrySetCanceled();
            if (_sceneBinding != null) _sceneBinding.enabled = _bindingWasEnabled;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            // Like other SampleScene fixtures, the next Scene load replaces this test instance.
        }

        [UnityTest]
        public IEnumerator InitialRootIsHidden_OpeningShowsExactNumberAndBlocksOnlyBackground()
        {
            Assert.That(_root.activeSelf, Is.False); _open.onClick.Invoke(); yield return null;
            Assert.That(_root.activeSelf, Is.True); Assert.That(_account.text, Does.Contain("0000000001"));
            Assert.That(_background.interactable, Is.False); Assert.That(_background.blocksRaycasts, Is.False);
            _actions[(int)E_AccountTransferAction.Back].onClick.Invoke();
            Assert.That(_background.interactable, Is.True); Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(_open.gameObject));
        }

        [UnityTest]
        public IEnumerator ReenableAndReinitialize_DoNotDuplicateButtonListeners()
        {
            _host.SetActive(false); _host.SetActive(true);
            _view.GetType().GetMethod("Initialize").Invoke(_view, new object[] { _controller, null });
            _open.onClick.Invoke(); yield return null;
            _actions[(int)E_AccountTransferAction.Start].onClick.Invoke(); yield return null;
            Assert.That(_transport.Starts, Is.EqualTo(1)); Assert.That(_credentials.text, Does.Contain("000000007"));
        }

        [UnityTest]
        public IEnumerator DisableClosesScreenAndClearsTextAndInputBuffers()
        {
            _open.onClick.Invoke(); yield return null;
            _actions[(int)E_AccountTransferAction.Start].onClick.Invoke(); yield return null;
            _code.SetTextWithoutNotify("ABCD-2345"); _verification.SetTextWithoutNotify("000000007");
            _host.SetActive(false);
            Assert.That(_root.activeSelf, Is.False); Assert.That(_credentials.text, Is.Empty);
            Assert.That(_code.text, Is.Empty); Assert.That(_verification.text, Is.Empty); Assert.That(_controller.State.Code, Is.Empty);
            _actions[(int)E_AccountTransferAction.Start].onClick.Invoke(); Assert.That(_transport.Starts, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator DoublePointerAndKeyboardSubmit_IssueOnlyOnePendingRequest()
        {
            _open.onClick.Invoke(); yield return null; _transport.Delayed = new TaskCompletionSource<OnlineAccountResponse>();
            Button start = _actions[(int)E_AccountTransferAction.Start]; start.onClick.Invoke();
            start.onClick.Invoke(); ExecuteEvents.Execute(start.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            Assert.That(_transport.Starts, Is.EqualTo(1)); Assert.That(start.interactable, Is.False);
            _transport.Delayed.SetResult(Transport.Issued()); yield return null;
            Assert.That(_controller.State.IsBusy, Is.False);
        }

        [UnityTest]
        public IEnumerator CloseDuringRequest_DelayedSecretNeverReappears()
        {
            _open.onClick.Invoke(); yield return null; _transport.Delayed = new TaskCompletionSource<OnlineAccountResponse>();
            _actions[(int)E_AccountTransferAction.Start].onClick.Invoke(); _actions[(int)E_AccountTransferAction.Back].onClick.Invoke();
            _transport.Delayed.SetResult(Transport.Issued()); yield return null;
            Assert.That(_root.activeSelf, Is.False); Assert.That(_credentials.text, Is.Empty); Assert.That(_controller.State.Code, Is.Empty);
            _open.onClick.Invoke(); yield return null;
            Assert.That(_controller.State.IsTransferPending, Is.True); Assert.That(_credentials.text, Is.Empty);
        }

        [UnityTest]
        public IEnumerator InputKeepsLeadingZerosAndClearsBeforeInvalidComplete()
        {
            _open.onClick.Invoke(); yield return null; _actions[(int)E_AccountTransferAction.OpenInput].onClick.Invoke();
            _code.SetTextWithoutNotify("invalid"); _verification.SetTextWithoutNotify("000000007");
            Assert.That(_verification.text, Is.EqualTo("000000007"));
            _actions[(int)E_AccountTransferAction.Complete].onClick.Invoke(); yield return null;
            Assert.That(_code.text, Is.Empty); Assert.That(_verification.text, Is.Empty); Assert.That(_transport.Completes, Is.Zero);
        }

        [UnityTest]
        public IEnumerator ExplicitNavigation_IsClampedAndBusyFocusMovesToBack()
        {
            _open.onClick.Invoke(); yield return null;
            Button start = _actions[(int)E_AccountTransferAction.Start], back = _actions[(int)E_AccountTransferAction.Back];
            Assert.That(start.navigation.selectOnUp, Is.EqualTo(start)); Assert.That(back.navigation.selectOnDown, Is.EqualTo(back));
            EventSystem.current.SetSelectedGameObject(start.gameObject); _transport.Delayed = new TaskCompletionSource<OnlineAccountResponse>();
            start.onClick.Invoke(); Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(back.gameObject));
            _transport.Delayed.SetResult(Transport.Issued()); yield return null;
        }

        [UnityTest]
        public IEnumerator SuccessfulDiscard_KeepsResultAndCompletionMessageVisibleUntilBack()
        {
            Assert.That(RecordSubmissionPolicy.TryCreateStageCandidate("owner",
                "22222222-2222-4222-8222-222222222222", "stage-001", 1,
                FlowState.Runtime.Core.E_StageResultType.Cleared, 1, out RecordSubmissionCandidate candidate), Is.True);
            Assert.That(_local.TryEnqueuePending(candidate), Is.True);
            Assert.That(_local.TryCheckpoint(), Is.True);
            _open.onClick.Invoke(); yield return null;
            _actions[(int)E_AccountTransferAction.RequestDiscard].onClick.Invoke();
            _actions[(int)E_AccountTransferAction.ConfirmDiscard].onClick.Invoke();
            TMP_Text status = ReadField<TMP_Text>(_view, "_statusText");
            GameObject[] pages = ReadField<GameObject[]>(_view, "_pages");
            for (int frame = 0; frame < 3; frame++)
            {
                yield return null;
                Assert.That(_controller.State.Page, Is.EqualTo(E_AccountTransferPage.Result));
                Assert.That(pages[(int)E_AccountTransferPage.Result].activeInHierarchy, Is.True);
                Assert.That(status.isActiveAndEnabled, Is.True);
                Assert.That(status.text, Is.EqualTo("Pending records discarded. Online records were kept."));
                Assert.That(_actions[(int)E_AccountTransferAction.RequestDiscard].gameObject.activeSelf, Is.False);
                Assert.That(_local.CreatePendingSnapshot(), Is.Empty);
                Assert.That(_transport.Starts, Is.Zero);
            }
            _actions[(int)E_AccountTransferAction.Back].onClick.Invoke();
            Assert.That(_root.activeSelf, Is.False);
            Assert.That(status.text, Is.Empty);
        }

        [UnityTest]
        public IEnumerator CopyButtons_CopyExactIndividualValuesAndRejectClosedOrBusyPage()
        {
            Button copyCode = ReadField<Button>(_view, "_copyCodeButton");
            Button copyValue = ReadField<Button>(_view, "_copyVerificationButton");
            TMP_Text status = ReadField<TMP_Text>(_view, "_statusText");
            string previousClipboard = GUIUtility.systemCopyBuffer;
            try
            {
                _open.onClick.Invoke(); yield return null;
                Assert.That(copyCode.gameObject.activeSelf, Is.False);
                Assert.That(copyValue.gameObject.activeSelf, Is.False);
                _actions[(int)E_AccountTransferAction.Start].onClick.Invoke(); yield return null;
                Assert.That(copyCode.isActiveAndEnabled && copyCode.interactable, Is.True);
                Assert.That(copyValue.isActiveAndEnabled && copyValue.interactable, Is.True);
                copyCode.onClick.Invoke();
                Assert.That(GUIUtility.systemCopyBuffer == "ABCD-2345", Is.True, "Clipboard contents withheld.");
                Assert.That(status.text, Does.Contain("Transfer code copied."));
                copyValue.onClick.Invoke();
                Assert.That(GUIUtility.systemCopyBuffer == "000000007", Is.True, "Clipboard contents withheld.");
                Assert.That(status.text, Does.Contain("Verification value copied."));
                _transport.Delayed = new TaskCompletionSource<OnlineAccountResponse>();
                _actions[(int)E_AccountTransferAction.Reissue].onClick.Invoke();
                GUIUtility.systemCopyBuffer = "test-sentinel";
                copyCode.onClick.Invoke(); copyValue.onClick.Invoke();
                Assert.That(GUIUtility.systemCopyBuffer == "test-sentinel", Is.True);
                _transport.Delayed.SetResult(Transport.Issued()); yield return null;
                copyValue.onClick.Invoke();
                _actions[(int)E_AccountTransferAction.Back].onClick.Invoke();
                Assert.That(GUIUtility.systemCopyBuffer == "000000007", Is.True);
                GUIUtility.systemCopyBuffer = "test-sentinel";
                copyCode.onClick.Invoke(); copyValue.onClick.Invoke();
                Assert.That(GUIUtility.systemCopyBuffer == "test-sentinel", Is.True);
                _transport.Pending = false;
                _transport.Delayed = null;
                _open.onClick.Invoke(); yield return null;
                _actions[(int)E_AccountTransferAction.OpenInput].onClick.Invoke(); yield return null;
                Assert.That(copyCode.gameObject.activeSelf, Is.False);
                Assert.That(copyValue.gameObject.activeSelf, Is.False);
                Assert.That(_code.text, Is.Empty); Assert.That(_verification.text, Is.Empty);
                Assert.That(_transport.Completes, Is.Zero);
            }
            finally { GUIUtility.systemCopyBuffer = previousClipboard; }
        }

        private static T ReadField<T>(MonoBehaviour component, string name) where T : class
        {
            FieldInfo field = component.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, name);
            T value = field.GetValue(component) as T;
            Assert.That(value, Is.Not.Null, "Step 8: Inspector reference missing: " + component.GetType().Name + "." + name);
            return value;
        }

        private static MonoBehaviour FindSceneBehaviour(Scene scene, Type type)
        {
            MonoBehaviour match = null;
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Component component in root.GetComponentsInChildren(type, true))
                {
                    Assert.That(match, Is.Null, "Step 8: SampleScene must contain exactly one AccountTransferView.");
                    match = (MonoBehaviour)component;
                }
            Assert.That(match, Is.Not.Null, "Step 8: add and connect AccountTransferView in SampleScene before running these 7 tests.");
            return match;
        }

        private static void ValidateInput(TMP_InputField input)
        {
            Assert.That(input.enabled, Is.True);
            Assert.That(input.textViewport, Is.Not.Null, "Step 8: connect TMP InputField Text Viewport.");
            Assert.That(input.textComponent, Is.Not.Null, "Step 8: connect TMP InputField Text Component.");
            ValidateText(input.textComponent);
        }

        private static void ValidateText(TMP_Text text)
        {
            Assert.That(text.enabled, Is.True, "Step 8: do not disable TMP components to hide rendering errors.");
            Assert.That(text.font, Is.Not.Null, "Step 8: connect a TMP Font Asset in Inspector.");
        }
    }
}
