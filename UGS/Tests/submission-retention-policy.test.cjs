const assert = require("assert").strict;
const policy = require("../CloudCode/submission-retention-policy");

const acceptedAt = 1000;
const expiry = acceptedAt + policy.receiptRetentionMilliseconds;
assert.equal(policy.isTerminalReceiptExpired(acceptedAt, expiry - 1), false);
assert.equal(policy.isTerminalReceiptExpired(acceptedAt, expiry), true);
assert.equal(policy.isTerminalReceiptExpired(acceptedAt, acceptedAt - 1), false);

assert.equal(policy.getPendingDisposition(acceptedAt, expiry - 1), "RetryAllowed");
assert.equal(policy.getPendingDisposition(acceptedAt, expiry), "SubmissionExpired");
assert.equal(policy.getPendingDisposition(-1, acceptedAt), "SubmissionExpired");

const receipt = { terminalAtMilliseconds: acceptedAt };
assert.equal(policy.getReplayDisposition(receipt, true, expiry - 1), "ExistingResult");
assert.equal(policy.getReplayDisposition(receipt, false, expiry - 1), "SubmissionIdConflict");
assert.equal(policy.getReplayDisposition(receipt, false, expiry), "NewSubmission");
assert.equal(policy.getReplayDisposition(null, false, expiry), "NewSubmission");

console.log("PASS submission retention policy: 180-day terminal replay, pending expiry and post-cleanup fresh submit");
