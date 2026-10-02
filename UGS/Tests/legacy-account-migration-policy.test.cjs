// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/legacy-account-migration-policy");
const generated = "8a1c2e30-4f8b-4c5a-9d72-3ba41c2e6f90";

assert.deepEqual(policy.selectInitialLeaderboardOwner({ authenticatedPlayerId: "legacy-player",
  generatedOwnerId: generated, hasLegacyLedger: true }), { kind: "legacy", id: "legacy-player" });
assert.deepEqual(policy.selectInitialLeaderboardOwner({ authenticatedPlayerId: "legacy-player",
  generatedOwnerId: generated, hasStageScore: true }), { kind: "legacy", id: "legacy-player" });
assert.deepEqual(policy.selectInitialLeaderboardOwner({ authenticatedPlayerId: "new-player",
  generatedOwnerId: generated }), { kind: "generated", id: generated });
assert.equal(policy.selectInitialLeaderboardOwner({ authenticatedPlayerId: "new-player",
  generatedOwnerId: "not-a-uuid" }), null);

const ready = { sourceSnapshotRecorded: true, destinationLedgerWritten: true, accountWritten: true,
  playerBindingWritten: true, publicNumberBindingWritten: true, ownerBindingWritten: true };
assert.equal(policy.canActivateMigration(ready), true);
assert.equal(policy.canActivateMigration({ ...ready, destinationLedgerWritten: false }), false);
assert.equal(policy.canRollbackToLegacy({ cutoverCompleted: false, sourceUnchanged: true }), true);
assert.equal(policy.canRollbackToLegacy({ cutoverCompleted: true, sourceUnchanged: true }), false);

console.log("PASS legacy migration policy: preserve existing owner, complete before cutover, safe pre-cutover rollback");
