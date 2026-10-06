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
assert.ok(submit.includes("store.compareExchange(item, ledger)"));
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

// Phase 2 Step 2 libraries are bundled dependencies, not independent endpoints.
const accountStore = read("UGS/CloudCode/account-store.js");
const accountEntry = read("UGS/CloudCode/get-public-player-number.js");
const legacyStore = read("UGS/CloudCode/legacy-account-store.js");
assert.ok(accountStore.includes("setPrivateCustomItemBatch"));
assert.ok(accountStore.includes("writeLock: guard.writeLock"));
assert.ok(legacyStore.includes("setProtectedItemBatch"));
assert.ok(accountEntry.includes("module.exports.params = {}"));
assert.ok(accountEntry.includes("module.exports.bundling = true"));
assert.ok(submit.includes('store.read("ledger", accountId)'));
assert.ok(legacyStore.includes("migration: { accountId, projectId, environmentId"));
for (const file of ["account-store", "account-service", "account-provisioning-service", "legacy-account-store", "get-public-player-number"]) {
  const content = read(`UGS/CloudCode/${file}.js`);
  assert.ok(!content.includes("Math.random"));
  assert.ok(!content.includes("context.accessToken"));
  assert.ok(!/writeLock:\s*null/.test(content));
}
console.log("PASS Phase 2 Step 2 static contracts: guard batch, cutover barrier, bundled dependencies and secret-free errors");

const transferCrypto = read("UGS/CloudCode/transfer-cryptography.js");
const transferStart = read("UGS/CloudCode/transfer-start-service.js");
const transferEntry = read("UGS/CloudCode/start-account-transfer.js");
assert.ok(transferCrypto.includes('require("crypto")'));
assert.ok(transferCrypto.includes('crypto.createHmac("sha256", secret)'));
assert.ok(transferCrypto.includes("crypto.randomBytes"));
assert.ok(transferCrypto.includes("FS_TRANSFER_HMAC_VERIFICATION_V1"));
assert.ok(transferCrypto.includes("secretManager.getSecret(secretName)"));
assert.ok(transferStart.includes('store.create("t", transfer.codeDigest'));
assert.ok(transferStart.includes("assertTransferCanStart(context)"));
assert.ok(transferStart.includes("credentialActive: false"));
assert.ok(transferEntry.includes("module.exports.params = {}"));
assert.ok(transferEntry.includes("module.exports.bundling = true"));
for (const content of [transferCrypto, transferStart, transferEntry]) {
  assert.ok(!content.includes("Math.random"));
  assert.ok(!content.includes("context.accessToken"));
  assert.ok(!/writeLock:\s*null/.test(content));
  assert.ok(!/logger\.[a-z]+\([^)]*(?:credential|secret|error\.)/i.test(content));
}
console.log("PASS Phase 2 Step 3-1 static contracts: server CSPRNG/HMAC, injected Secret, digest reservation, CAS gate and bundled endpoint");

const transferReissueEntry = read("UGS/CloudCode/reissue-account-transfer.js");
const transferVerification = read("UGS/CloudCode/transfer-verification-service.js");
assert.ok(transferStart.includes("async reissue(context)"));
assert.ok(transferStart.includes("current.credentialRevision + 1"));
assert.ok(transferVerification.includes("crypto.timingSafeEqual"));
assert.ok(transferVerification.includes("policy.isVerificationRetryAllowed"));
assert.ok(transferVerification.includes("store.compareExchange(account"));
assert.ok(transferVerification.includes("Number.MAX_SAFE_INTEGER"));
assert.ok(!transferVerification.includes("module.exports.params"));
assert.ok(transferReissueEntry.includes("module.exports.params = {}"));
assert.ok(transferReissueEntry.includes("module.exports.bundling = true"));
for (const content of [transferReissueEntry, transferVerification]) {
  assert.ok(!content.includes("Math.random"));
  assert.ok(!content.includes("context.accessToken"));
  assert.ok(!/writeLock:\s*null/.test(content));
  assert.ok(!/logger\.[a-z]+\([^)]*(?:credential|secret|error\.)/i.test(content));
}
console.log("PASS Phase 2 Step 3-2 static contracts: revision invalidation, constant-time HMAC, Account CAS throttle and private verification dependency");

const transferLifecycle = read("UGS/CloudCode/transfer-lifecycle-service.js");
const transferState = read("UGS/CloudCode/transfer-state.js");
assert.ok(transferStart.includes('require("./transfer-state")'));
assert.ok(transferLifecycle.includes("store.compareExchange(account"));
assert.ok(transferLifecycle.includes("delete terminal.codeDigest"));
assert.ok(transferLifecycle.includes("delete terminal.credentialHmac"));
assert.ok(transferLifecycle.includes("account.value.transfer.transferId !== expectedTransferId"));
assert.ok(transferState.includes("validateContext(context)"));
assert.ok(transferState.includes("binding.value.connectionRevision"));
for (const file of ["transfer-state", "transfer-lifecycle-service", "cancel-account-transfer", "get-account-transfer-status"]) {
  const content = read(`UGS/CloudCode/${file}.js`);
  assert.ok(!content.includes("Math.random"));
  assert.ok(!content.includes("context.accessToken"));
  assert.ok(!/writeLock:\s*null/.test(content));
  assert.ok(!content.includes("getSecret"));
  assert.ok(!/logger\.[a-z]+\([^)]*(?:credential|secret|error\.)/i.test(content));
}
for (const file of ["cancel-account-transfer", "get-account-transfer-status"])
  assert.ok(read(`UGS/CloudCode/${file}.js`).includes("module.exports.bundling = true"));
console.log("PASS Phase 2 Step 3-3 static contracts: terminal CAS, generation-fenced cancel, Secret-free status/expiry and credential disposal");

const completion = read("UGS/CloudCode/transfer-completion-service.js");
const completionEntry = read("UGS/CloudCode/complete-account-transfer.js");
const accountService = read("UGS/CloudCode/account-service.js");
for (const marker of ["TransferJoining", "TransferCompleting", "Detached", "transferOperation", "AlreadyCompleted",
  'store.create("receipt"', "crypto.timingSafeEqual", "validatePendingTransfer(target, now())"])
  assert.ok(completion.includes(marker));
assert.ok(accountService.includes("binding.value.transferOperation"));
assert.ok(accountService.includes("resumeSubmissionId"));
assert.ok(completionEntry.includes("module.exports.bundling = true"));
assert.ok(completionEntry.includes('verificationValue: { type: "String", required: true }'));
assert.ok(!completion.includes("addLeaderboardPlayerScore"));
for (const content of [completion, completionEntry]) {
  assert.ok(!content.includes("Math.random"));
  assert.ok(!content.includes("context.accessToken"));
  assert.ok(!/writeLock:\s*null/.test(content));
  assert.ok(!/logger\.[a-z]+\([^)]*(?:credential|secret|error\.)/i.test(content));
}
console.log("PASS Phase 2 Step 4-1 static contracts: fenced completion saga, detached old account, completion receipt and safe bundled endpoint");

assert.ok(completion.includes("async recover(context)"));
assert.ok(completion.includes("releaseOwnPendingFence"));
assert.ok(completion.includes("revokedJoiningTransfer: op"));
assert.ok(completion.includes("sameOperation(old.value.revokedJoiningTransfer, op)"));
assert.ok(completion.includes("confirmedBinding.value.connectionRevision"));
assert.ok(read("UGS/CloudCode/get-account-transfer-status.js").includes("createTransferCompletionService(store, null).recover(context)"));
assert.ok(transferLifecycle.includes("inactiveByTransferId"));
assert.ok(read("UGS/Tests/transfer-races.test.cjs").includes("all cancellation and rollback read failures"));
console.log("PASS Phase 2 Step 4-2 static contracts: authenticated Secret-free recovery, own-transfer fence release and stale receipt reply guard");

const legacyBridge = read("UGS/Migration/legacy-barrier-submit.js");
assert.ok(legacyBridge.includes('if (ledger.migration) return reply("TransientFailure", "AccountMigrationRequired")'));
assert.ok(legacyBridge.includes("writeLock: item.writeLock"));
assert.ok(submit.includes("reserveSubmission(context, p.submissionId)"));
assert.ok(submit.includes("releaseSubmission(context, p.submissionId)"));
assert.ok(submit.includes("p.boardId, ownerId,"));
assert.ok(!submit.includes("setProtectedItem"));
assert.ok(query.includes("publicPlayerNumber: number, isMe: account.value.accountId === initial.accountId"));
assert.ok(query.includes("current.connectionRevision !== initial.connectionRevision"));
assert.ok(!query.includes("playerId: e.playerId"));
for (const content of [submit, query]) {
  assert.ok(content.includes("module.exports.bundling = true"));
  assert.ok(!content.includes("context.accessToken"));
  assert.ok(!content.includes("Math.random"));
}
console.log("PASS Phase 2 Step 5 static contracts: C CAS reservations, fixed owner, public-only response, final authority fence and historical migration bridge");
