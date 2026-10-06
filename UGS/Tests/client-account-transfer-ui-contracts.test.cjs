"use strict";
// Source contracts only: this does not compile C# or execute Unity tests.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const read = file => fs.readFileSync(path.resolve(__dirname, "../..", file), "utf8");
const feature = name => read(`Assets/Scripts/Runtime/Features/${name}.cs`);
const controller = feature("AccountTransferController");
const state = feature("AccountTransferScreenState");
const view = read("Assets/Scripts/Runtime/Systems/AccountTransferView.cs");
const binding = read("Assets/Scripts/Runtime/Systems/AccountTransferUIController.cs");
const game = read("Assets/Scripts/Runtime/Systems/GameSystem.cs");
for (const method of ["StartTransferAsync", "CompleteTransferAsync(code, verificationValue)", "ReissueTransferAsync", "CancelTransferAsync", "RefreshStatusAsync", "RefreshPanelAsync"])
  assert.ok(controller.includes(`_account.${method}`), method);
assert.ok(controller.indexOf("_local.CreatePendingSnapshot().Count != 0") < controller.indexOf("await _account.StartTransferAsync"));
const consent = controller.slice(controller.indexOf("case E_AccountTransferAction.ConfirmConsent:", controller.indexOf("public async Task ExecuteAsync")));
assert.ok(consent.indexOf("_local.TrySaveOnlineAccount") < consent.indexOf("await RefreshAsync"));
assert.ok(controller.includes("if (!State.IsOpen || State.IsBusy) return;"));
assert.ok(controller.includes("if (State.IsBusy || State.IsOpen) return;"));
assert.ok(controller.includes("if (IsCurrent(generation)) ApplyResponse"));
assert.ok(!controller.includes("else if (!State.IsOpen) _view.Render"));
assert.ok(controller.includes("_generation++"));
assert.ok(controller.includes("_local.TryDiscardPending()"));
assert.ok(controller.includes("State.Page = saved ? E_AccountTransferPage.Result : E_AccountTransferPage.DiscardConfirmation;"));
assert.ok(controller.includes("Pending records discarded. Online records were kept."));
assert.ok(controller.includes("E_AccountTransferPage.DiscardConfirmation"));
assert.ok(controller.includes("response.code = string.Empty; response.verificationValue = string.Empty;"));
assert.ok(feature("OnlineAccountCoordinator").includes("response.code = string.Empty; response.verificationValue = string.Empty;"));
assert.ok(!/AuthenticationService|UnityEngine|JsonUtility|Debug\.|\.Invalidate\(/.test(controller));
const account = feature("OnlineAccountCoordinator");
assert.ok(account.includes("PanelTimeoutMilliseconds = 5000"));
const panel = account.slice(account.indexOf("public async Task<OnlineAccountResponse> RefreshPanelAsync"), account.indexOf("public Task<OnlineAccountResponse> StartTransferAsync"));
assert.equal([...panel.matchAll(/_panelDelay\(PanelTimeoutMilliseconds\)/g)].length, 1);
assert.ok(panel.includes("finally { _panelDeadline = null; }"));
assert.ok(panel.includes("Task.WhenAny(request, deadline)"));
assert.ok(panel.indexOf("deadline.IsCompleted") < panel.indexOf("Task<T> request = start()"));
assert.ok(panel.includes("ObserveDiscardedRequestAsync(request)"));
assert.equal(/await (?:_transport\.CallAsync|_authentication\.TryAuthenticateAsync|session\.TryClearSessionAsync)/.test(account), false);
assert.ok(read("Assets/Editor/OnlineRecordVerificationWindow.cs").includes("AccountPanelTimeoutMs="));
assert.ok(!/Serializable|SerializeField/.test(state));
for (const member of ["_root", "_pages", "_settingsContent", "_accountText", "_statusText", "_credentialText", "_codeInput", "_verificationInput", "_openButton", "_actionButtons"])
  assert.match(view, new RegExp(`\\[SerializeField\\] private [^;]+ ${member};`));
assert.ok(view.includes("PageCount = 5") && view.includes("ActionCount = 12"));
for (const handler of ["HandleOpen", "_handlers[i]"]) {
  assert.ok(view.includes(`AddListener(${handler})`)); assert.ok(view.includes(`RemoveListener(${handler})`));
}
assert.ok(!/AddListener\(\s*\(/.test(view));
assert.ok(view.includes("if (_areListenersRegistered) return;"));
assert.ok(view.includes("GetPersistentEventCount() != 0"));
assert.ok(view.includes("Navigation.Mode.Explicit"));
assert.ok(view.includes("_settingsContent.interactable = false"));
assert.ok(view.includes("_verificationInput.characterLimit = 9"));
assert.ok(view.includes("SetTextWithoutNotify(string.Empty)"));
assert.ok(!/AuthenticationService|UnityServices|new LocalRecordRepository/.test(binding));
assert.ok(binding.includes("_gameSystem.CreateAccountTransferController(_view)"));
assert.ok(game.includes("new AccountTransferController(_localRecordRepository, _onlineAccount, view"));
assert.ok(game.includes("() => _onlineRecords.RetryAllPendingAsync()"));
assert.ok(game.includes("_accountTransferUI.IsOpen"));
const edit = read("Assets/Tests/EditMode/AccountTransferControllerTests.cs");
const play = read("Assets/Tests/PlayMode/AccountTransferUIIsolationTests.cs");
assert.ok(play.includes("CopyButtons_CopyExactIndividualValuesAndRejectClosedOrBusyPage"));
for (const handler of ["HandleCopyCode", "HandleCopyVerification"]) {
  assert.ok(view.includes(`AddListener(${handler})`));
  assert.ok(view.includes(`RemoveListener(${handler})`));
}
assert.ok(view.includes("GUIUtility.systemCopyBuffer ="));
assert.ok(view.includes("if (!CanCopyCredentials()) return;"));
assert.ok(view.includes("Treat it as a private credential."));
assert.ok(view.includes("copyCode ? _controller.State.Code : _controller.State.VerificationValue"));
assert.ok(!view.includes("HandlePasteDetails"));
assert.ok(!view.includes("ShowClipboardError"));
assert.ok(play.includes("finally { GUIUtility.systemCopyBuffer = previousClipboard; }"));
assert.ok(play.includes("SuccessfulDiscard_KeepsResultAndCompletionMessageVisibleUntilBack"));
// No Korean font is planned: all generated account-transfer copy must be ASCII.
for (const source of [controller, view, feature("OnlineAccountViewState")])
  for (const literal of source.matchAll(/"(?:\\.|[^"\\])*"/g))
    assert.ok(!/[^\x00-\x7f]/.test(literal[0]), "Account transfer UI string must be ASCII: " + literal[0]);
assert.ok(view.includes('Public number: unavailable'));
assert.ok(view.includes('Transfer code: '));
assert.ok(feature("OnlineAccountViewState").includes('A public number cannot recover your account.'));
for (const name of ["CloseDuringIssue_LateResponseCannotReopenOrRetainCredentials", "DoubleClicks_CoalesceUntilRequestSettles", "DiscardRequiresConfirmation_CancelAndFailedSavePreservePending", "InvalidComplete_DoesNotSendOrEchoRawInputs"])
  assert.ok(edit.includes(name));
for (const name of ["ReenableAndReinitialize_DoNotDuplicateButtonListeners", "DisableClosesScreenAndClearsTextAndInputBuffers", "DoublePointerAndKeyboardSubmit_IssueOnlyOnePendingRequest", "CloseDuringRequest_DelayedSecretNeverReappears", "ExplicitNavigation_IsClampedAndBusyFocusMovesToBack"])
  assert.ok(play.includes(name));
assert.ok(play.includes("AccountTransferView, Assembly-CSharp"));
assert.ok(!/AuthenticationService|UnityServices/.test(play));
assert.ok(play.includes('[UnitySetUp]'));
assert.ok(play.includes('SceneName = "SampleScene"'));
assert.ok(play.includes('SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single)'));
assert.ok(play.includes('FindSceneBehaviour(scene, viewType)'));
assert.ok(play.includes('ReadField<Button[]>(_view, "_actionButtons")'));
assert.ok(play.includes('ReadField<TMP_InputField>(_view, "_verificationInput")'));
assert.ok(play.includes('_sceneBinding.enabled = false;'));
assert.ok(play.includes('_sceneBinding.enabled = _bindingWasEnabled;'));
assert.ok(play.indexOf('isolation.GetValue(null)') < play.indexOf('SceneManager.LoadSceneAsync'));
assert.ok(play.includes('Assert.That(_root.activeSelf, Is.False'));
assert.ok(play.includes('ValidateText(input.textComponent)'));
assert.ok(play.includes('text.enabled, Is.True'));
assert.ok(!/new GameObject|AddComponent|\.SetValue\(|Assert\.Ignore|Assert\.Inconclusive|ignoreFailingMessages|\.enabled = false.*text/i.test(play));
assert.equal([...play.matchAll(/\[UnityTest\]/g)].length, 9);
const files = ["Assets/Scripts/Runtime/Features/AccountTransferController.cs", "Assets/Scripts/Runtime/Features/AccountTransferScreenState.cs", "Assets/Scripts/Runtime/Features/IAccountTransferView.cs", "Assets/Scripts/Runtime/Systems/AccountTransferView.cs", "Assets/Scripts/Runtime/Systems/AccountTransferUIController.cs", "Assets/Tests/EditMode/AccountTransferControllerTests.cs", "Assets/Tests/PlayMode/AccountTransferUIIsolationTests.cs"];
const guids = [];
for (const file of files) {
  const source = read(file); assert.ok(!/System\.Linq|\?\.|\?\?/.test(source), file);
  const plain = source.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").replace(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/g, '""');
  const stack = [], pairs = { ")": "(", "}": "{", "]": "[" };
  for (const char of plain) { if ("({[".includes(char)) stack.push(char); if (pairs[char]) assert.equal(stack.pop(), pairs[char], file); }
  assert.equal(stack.length, 0, file);
  const match = read(`${file}.meta`).match(/guid: ([a-f0-9]{32})/); assert.ok(match); guids.push(match[1]);
}
assert.equal(new Set(guids).size, files.length);
console.log(`Account transfer UI source contracts passed; prepared only: Edit ${[...edit.matchAll(/\[Test\]|\[TestCase\(/g)].length}, Play ${[...play.matchAll(/\[UnityTest\]/g)].length}. Unity execution NOT performed.`);
