// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/transfer-resolution-policy");
const base = { status: policy.pending, credentialActive: true, sourcePlayerId: "A",
  connectionRevision: 7, expiresAtMilliseconds: 1000 };
const eligibleB = { playerId: "B", hasOnlineData: false, hasLocalPending: false };

assert.equal(policy.canUseTransferTarget(eligibleB), true);
assert.equal(policy.canUseTransferTarget({ playerId: "B", hasOnlineData: true, hasLocalPending: false }), true);
assert.equal(policy.canUseTransferTarget({ playerId: "B", hasOnlineData: false, hasLocalPending: true }), false);
const completion = policy.resolveTransfer(base, { kind: "complete", playerId: "B", target: eligibleB,
  credentialsValid: true, retryAllowed: true, nowMilliseconds: 999 });
assert.equal(completion.outcome, "Completed");
assert.equal(completion.transfer.credentialActive, false);
assert.equal(completion.transfer.completedConnectionRevision, 8);
assert.equal(policy.resolveTransfer(completion.transfer, { kind: "complete", playerId: "B" }).outcome,
  "AlreadyCompleted");
assert.equal(policy.resolveTransfer(completion.transfer, { kind: "complete", playerId: "B2" }).outcome,
  "Consumed");
assert.equal(policy.resolveTransfer(completion.transfer, { kind: "cancel", playerId: "A" }).outcome,
  "Unavailable");
assert.equal(policy.resolveTransfer(base, { kind: "cancel", playerId: "A" }).outcome, "Cancelled");
assert.equal(policy.resolveTransfer(base, { kind: "complete", playerId: "B", target: eligibleB,
  credentialsValid: true, retryAllowed: true, nowMilliseconds: 1000 }).outcome, "Expired");
assert.equal(policy.resolveTransfer(base, { kind: "expire", nowMilliseconds: 1000 }).outcome, "Expired");
assert.equal(policy.resolveTransfer(base, { kind: "complete", playerId: "B", target: eligibleB,
  credentialsValid: true, retryAllowed: true }).outcome, "Invalid");
assert.equal(policy.resolveTransfer(base, { kind: "complete", playerId: "B", target: eligibleB,
  credentialsValid: true, retryAllowed: true, nowMilliseconds: 999 }).outcome, "Completed");

console.log("PASS transfer resolution policy: B pending gate, terminal winner, idempotence and expiry boundaries");
