// Read-only C# source contracts, NOT a C# compile or Edit Mode execution.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const root = path.resolve(__dirname, "../..");
const read = p => fs.readFileSync(path.join(root, p), "utf8");
const feature = name => read(`Assets/Scripts/Runtime/Features/${name}.cs`);
const scope = feature("OnlineDataScope"), data = feature("LocalSaveData");
const codec = feature("LocalSaveJsonCodec"), local = feature("LocalRecordRepository");
const gate = feature("AccountTransferPendingGate");
const test = read("Assets/Tests/EditMode/OnlineLocalSaveScopeTests.cs");
assert.ok(data.includes("CurrentVersion = 7"));
for (const field of ["onlineProjectId", "onlineEnvironmentId", "inactiveOnlineAreas"])
  assert.ok(codec.includes(field));
assert.ok(codec.includes("file.version < 6 ? OnlineDataScope.CreateVerification()"));
assert.ok(codec.includes("ReadPendingCandidates(file.pendingSubmissions)"));
assert.ok(codec.includes("createdAtMilliseconds"));
assert.ok(codec.includes("CopyWithCreatedAt"));
assert.ok(local.includes("DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()"));
assert.ok(local.includes("TryExpirePending"));
assert.ok(local.includes("PendingRetentionMilliseconds"));
assert.ok(feature("OnlineRecordCoordinator").includes("SubmissionExpired"));
assert.ok(codec.includes("result.Count != files.Length"));
assert.ok(codec.includes("Duplicate online scope."));
assert.ok(scope.includes("ProjectId == other.ProjectId && EnvironmentId == other.EnvironmentId"));
assert.ok(scope.includes('"a20a46fa-1edb-4d79-9c35-02f2fed31896"'));
assert.ok(local.includes("saveData.SelectOnlineScope(_activeScope)"));
assert.ok(local.includes("KeepsInactiveAreas(saveData)"));
assert.ok(local.includes("if (requiresSave && !TrySave(saveData)) IsLocalSaveReady = false"));
assert.ok(local.includes("!saveData.OnlineScope.Matches(_activeScope)"));
const discard = local.slice(local.indexOf("public bool TryDiscardPending()"), local.indexOf("internal bool TryBeginTransferRequest()"));
assert.ok(discard.indexOf("if (!TrySave(") < discard.indexOf("_memoryRepository = new MemoryRecordRepository()"));
assert.ok(gate.indexOf("_local.TryBeginTransferRequest()") < gate.indexOf("await request()"));
assert.ok(gate.includes("finally { _local.EndTransferRequest(); }"));
assert.ok(local.includes("CreatePendingSnapshot().Count != 0"));
assert.ok(local.includes("if (_isTransferRequestInProgress || _isAccountTransitionInProgress) return false;"));
assert.ok(feature("OnlineRecordCoordinator").includes("!_local.CanUseOnlineData"));
assert.ok(feature("CloudCodeRecordRepository").includes("if (!_canUseOnlineData()) return false"));
const game = read("Assets/Scripts/Runtime/Systems/GameSystem.cs");
assert.ok(game.includes("_localSaveData.PendingSubmissions, _localSaveData.OnlineScope"));
assert.ok(game.includes("() => _localRecordRepository.CanUseOnlineData"));
for (const name of ["LegacyMigration_AssignsVerificationAndKeepsCandidateIdentity",
  "DifferentScope_ArchivesWithoutSendingOrReassigning", "LegacyOpenedInAnotherEnvironment_IsAssignedToVerificationOnly",
  "MigrationWriteFailure_KeepsOriginalFilePendingAndBlocksRequests", "PendingOnEitherDeviceOrDifferentOwner_MakesZeroTransferRequests",
  "ExplicitDiscard_FailedSaveRetainsPendingAndSuccessfulSaveAllowsTransfer", "DiscardCurrentScope_DoesNotRemoveInactiveScopePending",
  "SharedRepositoryGate_CoalescesAAndBDoubleRequestsAndFencesNewPending", "CorruptPending_IsNotSilentlyDroppedToUnlockTransfer",
  "EmptyPendingButCheckpointFailure_MakesZeroRequestsAndCanRetry", "SettingsCheckpointCannotDropInactiveAreas",
  "PendingCreatedAt_IsPersistedAndDoesNotChangeAfterRestart", "PendingExpiry_AtBoundaryRemovesOnlyCurrentScopeAfterSuccessfulSave"])
  assert.ok(test.includes(name));
for (const name of ["OnlineDataScope", "OnlineLocalSaveData", "AccountTransferPendingGate", "LocalSaveData", "LocalSaveJsonCodec", "LocalRecordRepository"]) {
  const content = feature(name);
  assert.ok(!/System\.Linq|\?\.|\?\?/.test(content), name);
  const code = content.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").replace(/"(?:\\.|[^"\\])*"/g, '""');
  let depth = 0;
  for (const character of code) {
    if (character === "{") depth++;
    if (character === "}") depth--;
    assert.ok(depth >= 0, `${name}: braces`);
  }
  assert.equal(depth, 0, `${name}: braces`);
}
const guids = [];
for (const file of ["Assets/Scripts/Runtime/Features/OnlineDataScope.cs", "Assets/Scripts/Runtime/Features/OnlineLocalSaveData.cs",
  "Assets/Scripts/Runtime/Features/AccountTransferPendingGate.cs", "Assets/Tests/EditMode/OnlineLocalSaveScopeTests.cs"]) {
  const meta = read(`${file}.meta`); const match = meta.match(/guid: ([0-9a-f]{32})/);
  assert.ok(match); guids.push(match[1]);
}
assert.equal(new Set(guids).size, guids.length);
for (const forbidden of ["verificationValue", "credentialHmac", "codeDigest", "transferCode"])
  assert.ok(!codec.includes(forbidden));
const plain = (test.match(/\[Test\]/g) || []).length;
const parameterized = (test.match(/\[TestCase\(/g) || []).length;
console.log(`PASS client Local Save static contracts: scoped v7, created-at migration, fail-closed pending, persisted discard/gate, meta; ${plain + parameterized} Edit Mode cases prepared (not executed)`);
