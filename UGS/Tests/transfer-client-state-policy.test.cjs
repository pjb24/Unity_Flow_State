// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/transfer-client-state-policy");

assert.equal(policy.getTransferStartDisposition(0, true), "Ready");
assert.equal(policy.getTransferStartDisposition(1, true), "PendingMustBeCleared");
assert.equal(policy.getTransferStartDisposition(0, false), "LocalSaveUnavailable");
assert.equal(policy.getTransferStartDisposition(-1, true), "Invalid");
assert.deepEqual(policy.getSourceCompletionActions(true),
  ["ClearAccountScopedCache", "ClearPersonalBests", "ClearAuthenticationSession", "AuthenticateNewAnonymous"]);
assert.deepEqual(policy.getSourceCompletionActions(false), []);
assert.deepEqual(policy.getTargetCompletionActions(0, true),
  ["ClearAccountScopedCache", "ReplacePersonalBestsFromConnectedAccount", "RefreshConnectedAccount"]);
assert.deepEqual(policy.getTargetCompletionActions(1, true), []);
for (const value of ["Settings", "Tutorial"])
  assert.equal(policy.preservesLocalOnlyData(value), true);
for (const value of ["PersonalBests", "Pending"])
  assert.equal(policy.preservesLocalOnlyData(value), false);

console.log("PASS transfer client state policy: pending gate, source reset and connected-account best replacement");
