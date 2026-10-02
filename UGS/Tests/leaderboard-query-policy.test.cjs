const assert = require("assert").strict;
const policy = require("../CloudCode/leaderboard-query-policy");

assert.equal(policy.getRequestedEntryCount("top"), 10);
assert.equal(policy.getRequestedEntryCount("around"), 7);
assert.equal(policy.getRequestedEntryCount("me"), 0);
assert.equal(policy.hasPlayerPageNavigation(), false);
assert.equal(policy.getTiePageBoundaryDisposition(), "SplitTieGroupKeepGlobalRank");
for (const trigger of ["InitialEntry", "ExplicitRetry", "ExplicitRefresh", "Reentry"])
  assert.equal(policy.canRefreshSnapshot(trigger), true);
assert.equal(policy.canRefreshSnapshot("Timer"), false);
assert.equal(policy.canRefreshSnapshot("SubmissionCompleted"), false);
assert.equal(policy.compareExactPublicNumbers("0000000002", "0000000010"), -1);
assert.equal(policy.compareExactPublicNumbers("0000000010", "0000000002"), 1);
assert.equal(policy.compareExactPublicNumbers("10", "0000000010"), null);

console.log("PASS leaderboard query policy: compact lists, explicit snapshot refresh and public-number final tie order");
