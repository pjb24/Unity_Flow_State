// Read-only C# and endpoint contracts. Does not compile/run Unity or call UGS.
const fs = require("node:fs"), path = require("node:path"), assert = require("node:assert/strict");
const read = file => fs.readFileSync(path.resolve(__dirname, "../..", file), "utf8");
const feature = name => read(`Assets/Scripts/Runtime/Features/${name}.cs`);
const coordinator = feature("OnlineAccountCoordinator"), transport = feature("UgsOnlineRecordTransport");
const codec = feature("LocalSaveJsonCodec"), number = feature("PublicPlayerNumber");
const state = feature("OnlineAccountViewState"), dto = feature("OnlineLeaderboardEntry");
const tests = read("Assets/Tests/EditMode/OnlineAccountCoordinatorTests.cs");
for (const endpoint of ["get-public-player-number", "get-account-transfer-status", "start-account-transfer",
  "reissue-account-transfer", "cancel-account-transfer", "complete-account-transfer"]) {
  assert.ok(coordinator.includes(`"${endpoint}"`));
  const source = read(`UGS/CloudCode/${endpoint}.js`);
  assert.ok(source.includes("module.exports.params"));
  if (["get-public-player-number", "get-account-transfer-status", "start-account-transfer", "reissue-account-transfer"].includes(endpoint))
    assert.ok(source.includes("module.exports.params = {}"));
}
assert.ok(coordinator.includes('{ { "transferId", transferId } }'));
assert.ok(coordinator.includes('{ { "code", normalized }, { "verificationValue", verificationValue } }'));
assert.ok(coordinator.includes("_pendingGate.TryRequestAsync(SendAsync)"));
assert.ok(coordinator.includes("ReferenceEquals(binding, _local.OnlineAccount)"));
assert.ok(coordinator.includes("generation == _generation"));
assert.ok(coordinator.includes("_completionAwaitingHandoff"));
assert.ok(!coordinator.includes("response.reason"));
assert.ok(!coordinator.includes("Debug.Log"));
assert.ok(state.includes("IsOfflinePlayAllowed => true"));
assert.ok(state.includes("State == E_OnlineAccountDisplayState.Ready ? PublicNumber : string.Empty"));
assert.ok(!state.includes("verificationValue"));
for (const field of ["verificationValue", "transferId", "credentialHmac", "codeDigest", "transferStatus"])
  assert.ok(!codec.includes(field));
assert.ok(codec.includes("PublicNumberCache"));
assert.ok(number.includes('value.Length != 10 || value == "0000000000"'));
assert.ok(number.includes("value[i] < '0' || value[i] > '9'"));
assert.ok(dto.includes("string publicPlayerNumber")); assert.ok(dto.includes("bool isMe"));
assert.ok(!dto.includes("playerId"));
const ui = read("Assets/Scripts/Runtime/Systems/UIManagementSystem.cs");
assert.ok(ui.includes('A public number alone cannot recover your account.'));
assert.ok(!/[가-힣]/.test(ui), "Runtime UI must use English with the current font assets");
assert.ok(coordinator.includes("if (_authenticationRequest != null) return await _authenticationRequest;"));
assert.ok(coordinator.includes("_authenticationRequest = AuthenticateForRecordsAsync();"));
assert.ok(coordinator.includes("_authenticationRequest = null;"));
assert.ok(coordinator.includes("if (ownsDeadline) _authenticationDeadline = _panelDelay(PanelTimeoutMilliseconds);"));
assert.ok(coordinator.includes("if (ownsDeadline) _authenticationDeadline = null;"));
assert.ok(coordinator.includes("_authenticationGeneration != _generation"));
assert.ok(coordinator.includes("Task.WhenAny(startup, _panelDeadline)"));
assert.ok(coordinator.includes("_authenticationDeadline != null ? _authenticationDeadline : _panelDeadline"));
assert.ok(tests.includes("ConcurrentTopAndAround_ShareAuthenticationAndAllowExplicitNextQuery"));
assert.ok(tests.includes("ConcurrentTopAndAround_InvalidateRejectsBothBeforeRecordQueries"));
assert.ok(!ui.includes("MaskPlayerId")); assert.ok(!ui.includes("entry.playerId"));
assert.ok(ui.includes("PublicPlayerNumber.Format(entry.publicPlayerNumber, entry.isMe)"));
const game = read("Assets/Scripts/Runtime/Systems/GameSystem.cs");
assert.ok(game.includes("new OnlineAccountCoordinator(_localRecordRepository, authentication, transport"));
assert.ok(game.includes("_localRecordRepository, _onlineAccount, _onlineRepository"));
assert.ok(transport.includes("IOnlineAccountTransport"));
assert.ok(transport.includes("IReadOnlyDictionary<string, string> parameters"));
assert.ok(transport.includes("CallEndpointAsync<T>(endpoint, arguments)"));
assert.ok(transport.includes("Task.WhenAny"));
assert.ok(transport.includes("ObserveLateResultAsync(call, sequence, function, elapsed)"));
const runtimeNames = ["PublicPlayerNumber", "IOnlineAccountTransport", "OnlineAccountResponse", "OnlineAccountViewState", "OnlineAccountCoordinator"];
const guids = [];
for (const name of runtimeNames) {
  const code = feature(name);
  assert.ok(!/System\.Linq|\?\.|\?\?/.test(code));
  const plain = code.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").replace(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/g, '""');
  const stack = [], pairs = { ")": "(", "}": "{", "]": "[" };
  for (const char of plain) {
    if ("({[".includes(char)) stack.push(char);
    if (pairs[char]) assert.equal(stack.pop(), pairs[char], name);
  }
  assert.equal(stack.length, 0, name);
  const guid = read(`Assets/Scripts/Runtime/Features/${name}.cs.meta`).match(/guid: ([a-f0-9]{32})/);
  assert.ok(guid); guids.push(guid[1]);
}
assert.equal(new Set(guids).size, guids.length);
for (const test of ["AuthenticationImmediatelyIssuesNumberAndPersistsExactScopedCache", "NoConsentAndOffline_MakeZeroAuthenticationOrTransportCalls",
  "LateResponseAfterInvalidate_IsIgnoredWithoutSavingOldNumber", "AccountMismatch_NeverRequestsOrDisplaysAnotherAccountsNumber",
  "PendingBlocksStartAndCompleteBeforeAuthentication", "StatusUsesNoParameters_CancelUsesOnlyCurrentTransferId",
  "CompleteNormalizesCodeKeepsLeadingZerosAndAppliesConfirmedHandoff", "TimeoutPreservesBindingAndCache_AndStatusRetryCanRecoverPending",
  "LeaderboardUsesPublicNumberAndServerIsMe_RejectsMissingNumber"])
  assert.ok(tests.includes(test));
const cases = (tests.match(/\[Test\]/g) || []).length + (tests.match(/\[TestCase\(/g) || []).length;
assert.ok(tests.includes('publicPlayerNumber = HasCompleted ? "9999999999" : Number'));
const retryTest = tests.slice(tests.indexOf("public async Task InvalidNumberOrUnknownReason_HidesCacheAndSupportsExplicitRetry"), tests.indexOf("public async Task Restart_DoesNotDisplaySavedNumberUntilServerConfirms"));
assert.ok(retryTest.includes('Does.Contain("get-account-personal-bests")'));
assert.ok(retryTest.includes("local.OnlineAccount.PublicNumberCache, Is.EqualTo(Number)"));
console.log(`PASS client account static contracts: scoped cache, safe public display, exact params, authenticated/gated requests, stale/terminal fences; ${cases} Edit Mode cases prepared (not executed)`);
