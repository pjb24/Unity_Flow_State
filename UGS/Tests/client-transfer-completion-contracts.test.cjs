// Source/SDK contracts only. Does not compile or execute C# or contact UGS.
const fs = require("node:fs"), path = require("node:path"), assert = require("node:assert/strict");
const read = file => fs.readFileSync(path.resolve(__dirname, "../..", file), "utf8");
const feature = name => read(`Assets/Scripts/Runtime/Features/${name}.cs`);
const account = feature("OnlineAccountCoordinator"), local = feature("LocalRecordRepository");
const auth = feature("UgsOnlineAuthenticationGateway");
assert.ok(account.includes('"get-account-personal-bests", new Dictionary<string, string>()'));
assert.ok(account.indexOf('"get-account-transfer-status", new Dictionary<string, string>()') < account.indexOf("bool ready = await FetchNumberAsync"));
assert.ok(account.includes("snapshot.publicPlayerNumber != expectedNumber"));
assert.ok(account.includes("beforeNumber.PublicNumberCache == expectedNumber"));
assert.ok(account.includes("snapshot.personalBests == null"));
assert.ok(account.includes("bests[j].BoardKey.Equals(board)"));
assert.ok(account.includes("ReferenceEquals(blank, _local.OnlineAccount)"));
assert.ok(account.includes("finally { _local.EndAccountTransition(); }"));
assert.ok(account.includes("fresh.PlayerId == player"));
assert.equal(account.split("binding.PublicNumberCache != response.publicPlayerNumber").length - 1, 1);
const finish = account.slice(account.indexOf("private async Task<bool> FinishCompletionAsync"));
assert.ok(finish.indexOf("_local.TryApplyAccountTransition") < finish.indexOf("session.TryClearSessionAsync(player)"));
assert.ok(local.includes("(_isTransferRequestInProgress && !ownsTransferGate)"));
const transition = local.slice(local.indexOf("internal bool TryApplyAccountTransition"), local.indexOf("private bool KeepsInactiveAreas"));
assert.ok(transition.includes("CreatePendingSnapshot().Count != 0"));
assert.ok(transition.indexOf("if (!TrySave(") < transition.indexOf("_memoryRepository = new MemoryRecordRepository()"));
assert.ok(transition.includes("_lastSave.Settings, _lastSave.HasCompletedTutorial"));
assert.ok(transition.includes("_lastSave.InactiveOnlineAreas"));
assert.ok(auth.includes("IOnlineAuthenticationSession"));
assert.ok(auth.includes("AuthenticationService.Instance.SignOut(true)"));
assert.ok(auth.includes("AuthenticationService.Instance.ClearSessionToken()"));
assert.ok(auth.includes("AuthenticationService.Instance.PlayerId != expectedPlayerId"));
const cachePath = path.resolve(__dirname, "../../Library/PackageCache");
if (fs.existsSync(cachePath)) {
  const packages = fs.readdirSync(cachePath).filter(name => name.startsWith("com.unity.services.authentication@"));
  for (const name of packages) {
    const sdk = fs.readFileSync(path.join(cachePath, name, "Runtime/IAuthenticationService.cs"), "utf8");
    assert.ok(sdk.includes("void SignOut(bool clearCredentials = false)"));
    assert.ok(sdk.includes("void ClearSessionToken()"));
    assert.ok(sdk.includes("bool SessionTokenExists"));
  }
}
const game = read("Assets/Scripts/Runtime/Systems/GameSystem.cs");
assert.ok(game.includes("ClearAccountPresentation")); assert.ok(game.includes("_leaderboardViewState.Invalidate()"));
assert.ok(game.includes("_onlineRecords.ClearSessionHistory()"));
assert.ok(feature("OnlineRecordCoordinator").includes("_terminalReceipts.Clear()"));
assert.ok(feature("LeaderboardViewState").includes("public void Invalidate()"));
assert.ok(feature("CloudCodeRecordRepository").includes("current.PublicNumberCache == binding.PublicNumberCache"));
const endpoint = read("UGS/CloudCode/get-account-personal-bests.js");
assert.ok(endpoint.includes("module.exports.params = {}")); assert.ok(endpoint.includes("module.exports.bundling = true"));
assert.ok(endpoint.includes("latest.writeLock !== ledger.writeLock"));
assert.ok(!endpoint.includes("addLeaderboardPlayerScore"));
const codec = feature("LocalSaveJsonCodec");
for (const field of ["transferStatus", "transferId", "verificationValue", "_sessionToClear"])
  assert.ok(!codec.includes(field));
const tests = read("Assets/Tests/EditMode/AccountTransferCompletionTests.cs");
for (const name of ["BCompletion_ReplacesEvenBetterLocalBestAndPersistsNumberTogether", "BEmptySuccess_ClearsAllPreviousBests",
  "BWriteFailure_PreservesOldMemoryAndFile_StatusRetryApplies", "LostCompletionResponse_PreservesUntilRestartStatusConfirms",
  "AInactive_ClearsCurrentBestsBeforeTokensAndStartsDifferentAnonymous", "ACrashAfterBlankSave_RestartRecoversWithOldOrNewSdkSession",
  "RecoveryWithNewPending_PreservesItAndRequiresExplicitResolution", "AlreadyAppliedCompletion_AfterRestartRetainsOfflineBestAndPending",
  "LateSnapshotAfterInvalidate_DoesNotReplaceBestOrCache", "BindingReplacedDuringSnapshot_IsIgnoredEvenWithSamePlayer",
  "AccountPresentationInvalidation_RejectsDelayedLeaderboardSections"])
  assert.ok(tests.includes(name));
const paths = ["IOnlineAuthenticationSession", "OnlinePersonalBestEntry", "OnlinePersonalBestSnapshot", "OnlineAccountCoordinator",
  "UgsOnlineAuthenticationGateway", "LocalRecordRepository", "CloudCodeRecordRepository", "LeaderboardViewState"]
  .map(name => `Assets/Scripts/Runtime/Features/${name}.cs`);
paths.push("Assets/Tests/EditMode/AccountTransferCompletionTests.cs");
const guids = [];
for (const file of paths) {
  const code = read(file);
  assert.ok(!/System\.Linq|\?\.|\?\?|sessionPlaceholder/.test(code), file);
  const plain = code.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").replace(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/g, '""');
  const stack = [], pairs = { ")": "(", "}": "{", "]": "[" };
  for (const char of plain) {
    if ("({[".includes(char)) stack.push(char);
    if (pairs[char]) assert.equal(stack.pop(), pairs[char], file);
  }
  assert.equal(stack.length, 0, file);
  const guid = read(`${file}.meta`).match(/guid: ([a-f0-9]{32})/); assert.ok(guid); guids.push(guid[1]);
}
assert.equal(new Set(guids).size, guids.length);
const cases = (tests.match(/\[Test\]/g) || []).length + (tests.match(/\[TestCase\(/g) || []).length;
console.log(`PASS transfer completion client static contracts: atomic whole-best replacement, fenced session reset/restart, scoped idempotency, SDK API and presentation invalidation; ${cases} Edit Mode cases prepared (not executed)`);
