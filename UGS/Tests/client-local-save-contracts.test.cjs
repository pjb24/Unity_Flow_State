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

// Phase 3 Step 2: source ordering complements the prepared C# counter/state tests.
function methodBody(source, signature) {
  const start = source.indexOf(signature);
  assert.ok(start >= 0, signature);
  const open = source.indexOf("{", start);
  let end = open, depth = 1;
  while (depth && ++end < source.length) {
    if (source[end] === "{") depth++;
    if (source[end] === "}") depth--;
  }
  assert.equal(depth, 0, signature);
  return source.slice(open + 1, end);
}
const stageFactory = methodBody(feature("RecordSubmissionPolicy"), "public static bool TryCreateStageCandidate(");
assert.ok(stageFactory.includes("candidate = null;"));
assert.ok(stageFactory.includes("return false;"));
assert.ok(!stageFactory.includes("new RecordSubmissionCandidate") && !stageFactory.includes("return true;"));
for (const [source, signature, firstWrite] of [
  [feature("RecordSubmissionService"), "public bool TryStoreCandidate(", "_recordRepository.TryEnqueuePending"],
  [local, "public bool TryEnqueuePending(", "_now()"],
  [local, "public bool TryUpdatePersonalBest(", "_memoryRepository.TryUpdatePersonalBest"]
]) {
  const body = methodBody(source, signature);
  for (const guard of ["candidate.GameMode != E_GameMode.Infinite", "candidate.BoardKey.RulesVersion != ScoringVersion.Current", "candidate.BoardKey.StageId != string.Empty"])
    assert.ok(body.indexOf(guard) >= 0 && body.indexOf(guard) < body.indexOf(firstWrite), signature + " before " + firstWrite);
}
const storeResult = methodBody(game, "private void TryStoreCurrentResultCandidate(");
assert.ok(storeResult.indexOf("resultData.GameMode != E_GameMode.Infinite") < storeResult.indexOf("Guid.NewGuid()"));
assert.ok(storeResult.indexOf("resultData.GameMode != E_GameMode.Infinite") >= 0);
const restore = methodBody(local, "private void RestoreMemoryRecords(");
assert.ok(restore.includes("_memoryRepository.TryUpdatePersonalBest") && restore.includes("_memoryRepository.TryEnqueuePending"));
assert.ok(!restore.includes("E_GameMode.Infinite"));
assert.ok(!codec.includes("TryCreateStageCandidate"));
const policyTests = read("Assets/Tests/EditMode/RecordSubmissionPolicyTests.cs");
for (const testName of ["StoreCandidate_UnsupportedRecord_DoesNotCallRepository", "LocalNewWrites_UnsupportedRecord_KeepClockMemoryAndFileUnchanged", "LocalNewWrites_Null_DoNotReadClockOrWriteFile"])
  assert.ok(policyTests.includes(testName));
assert.ok(read("Assets/Tests/EditMode/LocalSaveJsonCodecTests.cs").includes("InternalStageFormat_LoadAndCheckpoint_PreserveWholeRecordsWithoutNewStageWrites"));
assert.ok(read("Assets/Tests/PlayMode/ResultMenuIntegrationTests.cs").includes("DirectResultCandidate_UnsupportedMode_KeepsRecordsAndPresentation"));
for (const file of ["Assets/Tests/EditMode/VerificationSessionTests.cs", "Assets/Tests/PlayMode/AccountTransferUIIsolationTests.cs"])
  assert.ok(read(file).includes("using FlowState.Runtime.Core;"));
console.log("PASS Phase 3 Step 2 source contracts: retired factory, new-write/ID guard ordering, unfiltered internal restore, prepared counter/state tests; NOT C# execution.");

const phase4Coordinator = feature("OnlineRecordCoordinator");
const phase4Process = methodBody(phase4Coordinator, "private async Task ProcessPendingAsync(");
const phase4Gate = methodBody(phase4Coordinator, "private bool CanProcessPending(");
for (const effect of ["_local.TryExpirePending(", "_local.TryCheckpoint()", "await BindAsync()"])
  assert.ok(phase4Process.indexOf("if (!CanProcessPending()) return;") >= 0 &&
    phase4Process.indexOf("if (!CanProcessPending()) return;") < phase4Process.indexOf(effect));
for (const guard of ["_local.AccountId != _owner", "_local.ActiveScope.Matches", "candidate.PlayerId != _owner",
  "candidate.BoardKey.GameMode != E_GameMode.Infinite", "candidate.BoardKey.RulesVersion != ScoringVersion.Current", "candidate.BoardKey.StageId != string.Empty"])
  assert.ok(phase4Gate.includes(guard));
assert.ok(phase4Process.includes("TryExpirePending(_now()"));
assert.ok(phase4Coordinator.includes("Func<long> now = null"));
assert.ok(phase4Coordinator.includes("DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()"));
assert.ok(phase4Process.indexOf("!_local.CanUseOnlineData") < phase4Process.indexOf("await _online.SubmitAsync(candidate)"));
assert.ok(phase4Process.indexOf("_local.TryRemovePending") < phase4Process.indexOf("RememberTerminalResult"));
assert.ok(phase4Process.includes("RecordSubmissionQueue.MaximumAttemptsPerRetryTrigger"));
assert.ok(phase4Process.includes("InitialRetryDelayMilliseconds << (attempt - 1)"));
for (const name of ["UnsupportedPending_AllRetryEntries_KeepDataWithoutClockSaveAuthenticationOrTransport",
  "TerminalResponse_FailedLocalCommit_RetainsIdAndCountsOnlyAfterDurableRetry", "DuplicateBackgroundRetry_CoalescesWhileSubmissionIsInFlight"])
  assert.ok(read("Assets/Tests/EditMode/OnlineRecordRepositoryTests.cs").includes(name));
console.log("PASS Phase 3 Step 4 source contracts: pure whole-queue gate before expiry/checkpoint/bind, injected clock, bounded retry, durable terminal commit. NOT C# execution.");
