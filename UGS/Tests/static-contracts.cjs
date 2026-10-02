// Read-only repository contract checks. Run with Node; no Unity process or network.
const fs = require("fs");
const path = require("path");
const assert = require("assert").strict;
const root = path.resolve(__dirname, "../..");
const read = p => fs.readFileSync(path.join(root, p), "utf8").replace(/^\uFEFF/, "");
const config = read("Assets/Scripts/Runtime/Features/OnlineRecordConfiguration.cs");
const submit = read("UGS/CloudCode/submit-record.js");
const query = read("UGS/CloudCode/query-records.js");
const publicNumberPolicy = read("UGS/CloudCode/public-player-number-policy.js");
const leaderboardOwnerPolicy = read("UGS/CloudCode/leaderboard-owner-policy.js");
const legacyMigrationPolicy = read("UGS/CloudCode/legacy-account-migration-policy.js");
const transferCredentialPolicy = read("UGS/CloudCode/transfer-credential-policy.js");
const transferResolutionPolicy = read("UGS/CloudCode/transfer-resolution-policy.js");
const activeConnectionPolicy = read("UGS/CloudCode/active-connection-policy.js");
const transferClientStatePolicy = read("UGS/CloudCode/transfer-client-state-policy.js");
const publicNumberDisplayPolicy = read("UGS/CloudCode/public-number-display-policy.js");
const transferOperationsPolicy = read("UGS/CloudCode/transfer-operations-policy.js");
const submissionRetentionPolicy = read("UGS/CloudCode/submission-retention-policy.js");
const leaderboardQueryPolicy = read("UGS/CloudCode/leaderboard-query-policy.js");
const environmentDataPolicy = read("UGS/CloudCode/environment-data-policy.js");
const operationalSafetyPolicy = read("UGS/CloudCode/operational-safety-policy.js");
const phase1Task = read("AI/90_Tasks/Prototype_8/20260930_01_Phase1ManualSteps.md");
for (const value of ["c76d55cf-7846-494b-9dce-a0797b179b36", "a20a46fa-1edb-4d79-9c35-02f2fed31896",
  "fs-stage-stage-001-r1", "fs-infinite-v2"]) {
  assert.ok(config.includes(value)); assert.ok(submit.includes(value)); assert.ok(query.includes(value));
}
const policy = JSON.parse(read("UGS/AccessControl/project-policy.json"));
for (const resource of ["urn:ugs:leaderboards:/**", "urn:ugs:cloud-save:/**"])
  assert.ok(policy.statements.some(s => s.Resource === resource && s.Principal === "Player" &&
    s.Effect === "Deny" && s.Action.includes("Write")), `Missing Player Write Deny: ${resource}`);
assert.deepEqual(JSON.parse(read("UGS/CloudSave/verification-ledger-seed.json")),
  { version: 1, active: "", entries: [], best: {} });
const manifest = JSON.parse(read("Packages/manifest.json")).dependencies;
assert.equal(manifest["com.unity.services.authentication"], "3.8.0");
assert.equal(manifest["com.unity.services.cloudcode"], "2.10.4");
const references = JSON.parse(read("Assets/Scripts/Runtime/Features/FlowState.Runtime.Features.asmdef")).references;
for (const name of ["Unity.Services.Core", "Unity.Services.Core.Environments", "Unity.Services.Authentication", "Unity.Services.CloudCode"])
  assert.ok(references.includes(name));
assert.ok(!submit.includes("context.accessToken"));
assert.ok(publicNumberPolicy.includes("const width = 10"));
assert.ok(publicNumberPolicy.includes("const minimum = 1"));
assert.ok(publicNumberPolicy.includes("const maximum = 9999999999"));
assert.ok(publicNumberPolicy.includes("tryFormatIssuedNumber"));
assert.ok(publicNumberPolicy.includes("tryNormalizePublicPlayerNumber"));
assert.ok(!publicNumberPolicy.includes("Math.random"));
assert.ok(leaderboardOwnerPolicy.includes("uuidV4"));
assert.ok(leaderboardOwnerPolicy.includes("tryNormalizeGeneratedLeaderboardOwnerId"));
assert.ok(leaderboardOwnerPolicy.includes("tryUseLegacyLeaderboardOwnerId"));
assert.ok(!leaderboardOwnerPolicy.includes("Math.random"));
for (const name of ["selectInitialLeaderboardOwner", "canActivateMigration", "canRollbackToLegacy"])
  assert.ok(legacyMigrationPolicy.includes(name));
for (const name of ["transferLifetimeMilliseconds", "verificationThrottleMilliseconds", "tryNormalizeTransferCode",
  "tryNormalizeVerificationValue", "isTransferExpired", "isVerificationRetryAllowed", "canStartTransfer",
  "transferCodeCapacity", "expectedTransferCodeCollisionPairs"])
  assert.ok(transferCredentialPolicy.includes(name));
assert.ok(!transferCredentialPolicy.includes("Math.random"));
for (const name of ["canUseTransferTarget", "resolveTransfer", "AlreadyCompleted", "Consumed"])
  assert.ok(transferResolutionPolicy.includes(name));
for (const name of ["authorizeActiveConnection", "reserveMutation", "canBeginTransfer", "isQuerySnapshotCurrent"])
  assert.ok(activeConnectionPolicy.includes(name));
for (const name of ["getTransferStartDisposition", "getSourceCompletionActions", "getTargetCompletionActions"])
  assert.ok(transferClientStatePolicy.includes(name));
for (const name of ["formatLeaderboardAccount", "getPublicNumberPanelState", "getLeaderboardFailureState",
  "getPublicNumberRecoveryNotice", "isOfflinePlayAllowed"])
  assert.ok(publicNumberDisplayPolicy.includes(name));
assert.ok(!publicNumberDisplayPolicy.includes("playerId"));
for (const name of ["isSameEnvironment", "createSafeAuditEvent", "isSafeForGeneralError",
  "getDeploymentDisposition", "getRollbackDisposition"])
  assert.ok(transferOperationsPolicy.includes(name));
for (const forbidden of ["verificationValue", "accessToken", "serviceToken", "hmac", "digest"])
  assert.ok(!transferOperationsPolicy.includes(`return { ${forbidden}:`));
for (const name of ["receiptRetentionMilliseconds", "isTerminalReceiptExpired", "getPendingDisposition", "getReplayDisposition"])
  assert.ok(submissionRetentionPolicy.includes(name));
assert.ok(submissionRetentionPolicy.includes("180 * 24 * 60 * 60 * 1000"));
for (const name of ["topEntryCount", "aroundEntryCount", "getRequestedEntryCount", "hasPlayerPageNavigation",
  "getTiePageBoundaryDisposition", "canRefreshSnapshot", "compareExactPublicNumbers"])
  assert.ok(leaderboardQueryPolicy.includes(name));
assert.ok(leaderboardQueryPolicy.includes("const topEntryCount = 10"));
assert.ok(leaderboardQueryPolicy.includes("const aroundEntryCount = 7"));
for (const name of ["tryCreateOnlineScope", "isSameOnlineScope", "canUseOnlineLocalData",
  "getLegacyOnlineDataMigration", "canCopyEnvironmentData", "isProductionTestDataAllowed"])
  assert.ok(environmentDataPolicy.includes(name));
assert.ok(environmentDataPolicy.includes("return false"));
for (const name of ["structuredLogRetentionMilliseconds", "queryMinimumIntervalMilliseconds",
  "repeatSubmissionMinimumIntervalMilliseconds", "newSubmissionWindowMilliseconds",
  "maximumNewSubmissionsPerWindow", "getThirtyDayLogStorageDisposition", "isSafeStructuredLog",
  "isMinimumIntervalAllowed", "isNewSubmissionAllowed", "getRateLimitResponse", "shouldAutoRetryAfterRateLimit"])
  assert.ok(operationalSafetyPolicy.includes(name));
assert.ok(operationalSafetyPolicy.includes("30 * 24 * 60 * 60 * 1000"));
for (const marker of ["## Step 7. 결정 사항을 문서와 Unit Test 계약으로 고정한다",
  "##### Phase 1 정책–Test–후속 구현 대응표", "Step 8. Phase 1 완료를 판정하고 Phase 2에 인계한다",
  "Phase 1 판정 및 Phase 2 인계"])
  assert.ok(phase1Task.includes(marker));
assert.ok(submit.includes("writeLock: item.writeLock"));
assert.ok(!/writeLock:\s*null/.test(submit));
const auth = read("Assets/Scripts/Runtime/Features/UgsOnlineAuthenticationGateway.cs");
assert.ok(auth.includes("SetEnvironmentName")); assert.ok(auth.includes("OnlineRecordConfiguration.ProjectId"));
assert.ok(auth.includes("flow-state-verification"));
assert.ok(!auth.includes("production"));
const repository = read("Assets/Scripts/Runtime/Features/CloudCodeRecordRepository.cs");
assert.ok(repository.includes("submit-record")); assert.ok(repository.includes("query-records"));
assert.ok(!repository.includes("LeaderboardsService"));
const coordinator = read("Assets/Scripts/Runtime/Features/OnlineRecordCoordinator.cs");
assert.ok(coordinator.includes("MaximumAttemptsPerRetryTrigger"));
assert.ok(coordinator.includes("InitialRetryDelayMilliseconds <<"));
assert.ok(coordinator.includes("TryRemovePending"));
const scene = read("Assets/Scenes/SampleScene.unity"); // Inspect only; never edits the scene.
assert.ok(scene.includes("_moveSpeed: 8")); assert.ok(scene.includes("_scorePerUnit: 10"));
const codec = read("Assets/Scripts/Runtime/Features/LocalSaveJsonCodec.cs");
for (const field of ["recoveryNoticeConfirmed", "onlinePlayerId", "pendingSubmissions"])
  assert.ok(codec.includes(field));
for (const field of ["submittedIds", "rejectedIds", "rejectedReceipts"])
  assert.ok(!codec.includes(field));
const files = ["OnlineAccountState", "OnlineRecordConfiguration", "OnlineAuthenticationResult",
  "UgsOnlineAuthenticationGateway", "CloudCodeRecordRepository", "OnlineRecordCoordinator", "UgsOnlineRecordTransport"];
for (const name of files) {
  const content = read(`Assets/Scripts/Runtime/Features/${name}.cs`);
  assert.ok(!content.includes("System.Linq")); assert.ok(!/\?\.|\?\?/.test(content));
  assert.ok(!/eyJ[A-Za-z0-9_-]{20,}\./.test(content));
}
console.log("PASS static contracts: identifiers, policies, seed, packages, asmdef, service token, CAS, verification environment, scoring config, retry, migration, source guards");
