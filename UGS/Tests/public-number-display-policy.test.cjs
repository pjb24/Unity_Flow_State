// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/public-number-display-policy");

assert.equal(policy.formatLeaderboardAccount("0000000042", false), "0000000042");
assert.equal(policy.formatLeaderboardAccount("0000000042", true), "0000000042 (You)");
assert.equal(policy.formatLeaderboardAccount("42", true), null);
assert.deepEqual(policy.getPublicNumberPanelState({ isLoading: true, numberValue: "0000000042" }),
  { state: "Loading", text: "Loading public number...", retry: false });
assert.deepEqual(policy.getPublicNumberPanelState({ numberValue: "0000000042" }),
  { state: "Ready", text: "0000000042 (You)", retry: false });
assert.deepEqual(policy.getPublicNumberPanelState({ numberValue: "42" }),
  { state: "Error", text: "Could not load public number.", retry: true });
assert.deepEqual(policy.getLeaderboardFailureState(),
  { state: "Error", text: "Could not load leaderboard.", retry: true });
assert.ok(policy.getPublicNumberRecoveryNotice().includes("cannot recover"));
assert.equal(policy.isOfflinePlayAllowed(), true);

console.log("PASS public number display policy: exact number, safe error and offline allowance");
