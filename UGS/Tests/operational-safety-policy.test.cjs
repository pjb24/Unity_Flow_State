const assert = require("assert").strict;
const policy = require("../CloudCode/operational-safety-policy");

assert.equal(policy.getThirtyDayLogStorageDisposition(7 * 24 * 60 * 60 * 1000), "ExternalThirtyDaySinkRequired");
assert.equal(policy.getThirtyDayLogStorageDisposition(policy.structuredLogRetentionMilliseconds), "Ready");
assert.equal(policy.isSafeStructuredLog({ environmentId: "production", scriptVersion: "v1", outcomeCategory: "Rejected", reasonCategory: "InvalidScore", durationBucketMilliseconds: 500 }), true);
assert.equal(policy.isSafeStructuredLog({ environmentId: "production", scriptVersion: "v1", outcomeCategory: "Rejected", reasonCategory: "InvalidScore", durationBucketMilliseconds: 500, playerId: "secret" }), false);
assert.equal(policy.isMinimumIntervalAllowed(null, 1000, policy.queryMinimumIntervalMilliseconds), true);
assert.equal(policy.isMinimumIntervalAllowed(1000, 5999, policy.queryMinimumIntervalMilliseconds), false);
assert.equal(policy.isMinimumIntervalAllowed(1000, 6000, policy.queryMinimumIntervalMilliseconds), true);
assert.equal(policy.isNewSubmissionAllowed(1000, 3, 60999), false);
assert.equal(policy.isNewSubmissionAllowed(1000, 3, 61000), true);
assert.equal(policy.isNewSubmissionAllowed(1000, 2, 1001), true);
assert.equal(policy.getRateLimitResponse(false), "TooManyRequests");
assert.equal(policy.shouldAutoRetryAfterRateLimit(), false);

console.log("PASS operational safety policy: 30-day safe logs, moderate server limits and no rate-limit auto retry");
