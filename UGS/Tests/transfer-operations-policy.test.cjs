const assert = require("assert").strict;
const policy = require("../CloudCode/transfer-operations-policy");

const verification = { projectId: "project", environmentId: "verification" };
const production = { projectId: "project", environmentId: "production" };
assert.equal(policy.isSameEnvironment(verification, verification, verification), true);
assert.equal(policy.isSameEnvironment(verification, verification, production), false);
assert.equal(policy.isSameEnvironment({ projectId: "other", environmentId: "verification" }, verification, verification), false);

const event = policy.createSafeAuditEvent({
  event: "VerificationFailed", outcome: "Rejected", transferId: "transfer-opaque-id",
  environmentId: "verification", occurredAtUnixMilliseconds: 1, failureCount: 3,
  reasonCategory: "InvalidCredential"
});
assert.deepEqual(event, {
  event: "VerificationFailed", outcome: "Rejected", transferId: "transfer-opaque-id",
  environmentId: "verification", occurredAtUnixMilliseconds: 1, failureCount: 3,
  reasonCategory: "InvalidCredential"
});
assert.equal(Object.hasOwn(event, "verificationValue"), false);
assert.equal(policy.createSafeAuditEvent({ event: "Unknown", outcome: "Accepted", transferId: "t", failureCount: 0, occurredAtUnixMilliseconds: 0 }), null);
assert.equal(policy.createSafeAuditEvent({ event: "Requested", outcome: "Accepted", transferId: "t", failureCount: 0, occurredAtUnixMilliseconds: 0 }), null);
assert.equal(policy.isSafeForGeneralError("Transfer is unavailable. Retry later."), true);
assert.equal(policy.isSafeForGeneralError("Invalid verification value: 000000000"), false);
assert.equal(policy.isSafeForGeneralError("playerId mismatch"), false);

assert.equal(policy.getDeploymentDisposition({
  sourceEnvironment: "verification", targetEnvironment: "production", localTestsPassed: true,
  remoteBackupCaptured: true, accessPolicyReviewed: true, requiredSecretsAvailable: true,
  targetEnvironmentExplicit: true
}), "Ready");
assert.equal(policy.getDeploymentDisposition({ sourceEnvironment: "verification", targetEnvironment: "production" }), "Blocked");
assert.equal(policy.getDeploymentDisposition({}), "Blocked");
assert.equal(policy.getDeploymentDisposition({ sourceEnvironment: "verification", targetEnvironment: "verification" }), "RejectSameEnvironment");
assert.equal(policy.getRollbackDisposition({ previousVersionCaptured: true, targetEnvironmentExplicit: true }), "RepublishPreviousVersion");
assert.equal(policy.getRollbackDisposition({ previousVersionCaptured: false, targetEnvironmentExplicit: true }), "Blocked");

console.log("PASS transfer operations policy: environment boundary, safe audit fields and deployment rollback gates");
