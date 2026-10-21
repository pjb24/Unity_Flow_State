// Phase 4 Step 2 local unit tests. No Unity, UGS credentials, or network calls.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");
const policy = require("../CloudCode/operational-safety-policy");
const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8");

const verification = { projectId: "verification-project", environmentId: "verification-environment" };
const production = { projectId: "production-project", environmentId: "production-environment" };

for (const [left, right, expected] of [
  [verification, production, true],
  [verification, verification, false],
  [verification, { projectId: "verification-project", environmentId: "" }, false],
  [null, production, false]
]) assert.equal(policy.isDistinctEnvironmentPair(left, right), expected);

assert.equal(policy.isFixedBuildScope(production, null), true);
assert.equal(policy.isFixedBuildScope(production, production), true);
assert.equal(policy.isFixedBuildScope(production, verification), false);
assert.equal(policy.hasIsolatedOnlineLocalScope(production, production), true);
assert.equal(policy.hasIsolatedOnlineLocalScope(verification, production), false);

const authentication = read("Assets/Scripts/Runtime/Features/UgsOnlineAuthenticationGateway.cs");
const localRepository = read("Assets/Scripts/Runtime/Features/LocalRecordRepository.cs");
const localCodec = read("Assets/Scripts/Runtime/Features/LocalSaveJsonCodec.cs");
const onlineConfiguration = read("Assets/Scripts/Runtime/Features/OnlineRecordConfiguration.cs");
const onlineScope = read("Assets/Scripts/Runtime/Features/OnlineDataScope.cs");
const productionTemplate = read("Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs.example");
const gitignore = read(".gitignore");
assert.ok(onlineConfiguration.includes("VerificationEnvironmentName"));
assert.ok(!authentication.includes("RemoteConfig"));
assert.ok(localRepository.includes("_activeScope.Matches(OnlineDataScope.CreateConfigured())"));
assert.ok(localCodec.includes("new OnlineDataScope(file.onlineProjectId, file.onlineEnvironmentId)"));
assert.ok(onlineConfiguration.includes("#if FLOW_STATE_PRODUCTION"));
assert.ok(onlineConfiguration.includes("OnlineRecordProductionConfiguration"));
assert.ok(onlineConfiguration.includes("HasValidActiveConfiguration"));
assert.ok(authentication.includes("ActiveEnvironmentName"));
assert.ok(authentication.includes("InvalidBuildConfiguration"));
assert.ok(onlineScope.includes("ActiveProjectId"));
assert.ok(onlineScope.includes("ActiveEnvironmentId"));
assert.ok(productionTemplate.includes("public const string ProjectId = \"\""));
assert.ok(productionTemplate.includes("public const string EnvironmentId = \"\""));
assert.ok(gitignore.includes("OnlineRecordProductionConfiguration.cs"));

const currentManifest = JSON.parse(read("Assets/CloudCode/FlowStateVerification.ccmr"));
assert.equal(policy.isSingleCSharpModuleManifest(currentManifest), true);
assert.equal(policy.isSingleCSharpModuleManifest({ modulePath: currentManifest.modulePath, deployAll: false }), false);
assert.equal(policy.isSingleCSharpModuleManifest({ modulePath: "UGS/CloudCode/query-records.js" }), false);

const currentPolicy = JSON.parse(read("UGS/AccessControl/project-policy.json"));
assert.equal(policy.hasSafePlayerWritePolicy(currentPolicy), true);
assert.equal(policy.hasSafePlayerWritePolicy({ statements: currentPolicy.statements.concat({
  Sid: "AllowPlayerWrites", Action: ["Write"], Effect: "Allow", Principal: "Player", Resource: "urn:ugs:cloud-save:/**"
}) }), false);
assert.equal(policy.hasSafePlayerWritePolicy({ statements: currentPolicy.statements.concat({
  ...currentPolicy.statements[0]
}) }), false);

const safeLog = {
  environment: "production", moduleReference: "module-reference", outcome: "Rejected", reason: "RateLimited",
  durationBucket: 500, correlationId: "correlation", rateLimitResult: "TooManyRequests"
};
assert.equal(policy.isSafePhase4StructuredLog(safeLog), true);
assert.equal(policy.isSafePhase4StructuredLog({ ...safeLog, playerId: "forbidden" }), false);
assert.equal(policy.isSafePhase4StructuredLog({ ...safeLog, payload: "forbidden" }), false);
assert.equal(policy.isSafePhase4StructuredLog({ ...safeLog, durationBucket: -1 }), false);

const ready = {
  verification, production, deploymentKind: "Initial", inventoryCaptured: true, initialDeploymentRecoveryApproved: true, accessPolicyReviewed: true,
  secretAccessReviewed: true, logRetentionReady: true, logDeletionReady: true, compatibilityApproved: true,
  verificationRehearsed: true
};
assert.equal(policy.getProductionDeployGate(ready), "ReadyForApproval");
for (const requirement of ["inventoryCaptured", "initialDeploymentRecoveryApproved", "accessPolicyReviewed", "secretAccessReviewed",
  "logRetentionReady", "logDeletionReady", "compatibilityApproved", "verificationRehearsed"])
  assert.equal(policy.getProductionDeployGate({ ...ready, [requirement]: false }), "Blocked");
assert.equal(policy.getProductionDeployGate({ ...ready, production: verification }), "BlockedEnvironment");
assert.equal(policy.getProductionDeployGate({ ...ready, deploymentKind: undefined }), "Blocked");
assert.equal(policy.getProductionDeployGate({ ...ready, deploymentKind: "Update" }), "Blocked");
assert.equal(policy.getProductionDeployGate({ ...ready, deploymentKind: "Update", rollbackReferenceCaptured: true,
  initialDeploymentRecoveryApproved: false }), "ReadyForApproval");

const rollback = { target: production, previous: { ...production, moduleReference: "previous-reference" } };
assert.equal(policy.getPhase4RollbackDisposition(rollback), "RepublishPreviousModule");
assert.equal(policy.getPhase4RollbackDisposition({ ...rollback, deletesData: true }), "Blocked");
assert.equal(policy.getPhase4RollbackDisposition({ ...rollback, copiesEnvironmentData: true }), "Blocked");
assert.equal(policy.getPhase4RollbackDisposition({ ...rollback, previous: { ...verification, moduleReference: "previous-reference" } }), "Blocked");

for (const [contract, expected] of [
  [{ requestBackwardCompatible: true, responseBackwardCompatible: true, hasLocalSaveMigrationMarker: false, reintroducesLegacyWriter: false }, true],
  [{ requestBackwardCompatible: false, responseBackwardCompatible: true, hasLocalSaveMigrationMarker: false, reintroducesLegacyWriter: false }, false],
  [{ requestBackwardCompatible: true, responseBackwardCompatible: false, hasLocalSaveMigrationMarker: false, reintroducesLegacyWriter: false }, false],
  [{ requestBackwardCompatible: true, responseBackwardCompatible: true, hasLocalSaveMigrationMarker: true, reintroducesLegacyWriter: true }, false]
]) assert.equal(policy.isBackwardCompatibleContract(contract), expected);

console.log("PASS Phase 4 Step 2 policy: environment isolation, fixed build scope, module manifest, access policy, safe logs, deploy/rollback gates, and compatibility table.");
