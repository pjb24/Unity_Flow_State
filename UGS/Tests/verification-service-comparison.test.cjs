"use strict";
const assert = require("node:assert/strict");
const { compareLegacy, compareTransfer, compareDistinct, parseSnapshot } = require("../Verification/compare-service-snapshots.cjs");
const projectId = "c76d55cf-7846-494b-9dce-a0797b179b36", environmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const scope = { projectId, environmentId };
function fixture() {
  const before = { ...scope, account: { ...scope, accountId: "c", leaderboardOwnerId: "fixed", publicPlayerNumber: "0000000001", currentPlayerId: "a", connectionRevision: 1, status: "Active" },
    rows: { "fs-stage-stage-001-r1": { playerId: "fixed", score: 60000, metadata: { acceptedAt: 100, submissionId: "run" } }, "fs-infinite-v2": null } };
  const after = structuredClone(before); after.account.currentPlayerId = "b"; after.account.connectionRevision = 2;
  after.sourceBinding = { ...scope, playerId: "a", accountId: "c", status: "Inactive", connectionRevision: 1 };
  after.targetBinding = { ...scope, playerId: "b", accountId: "c", status: "Active", connectionRevision: 2 };
  return { before, after };
}
let cases = 0;
function check(fn) { fn(); cases++; }
check(() => { const { before, after } = fixture(); assert.equal(compareTransfer(before, after), true); });
for (const mutate of [s => s.account.leaderboardOwnerId = "other", s => s.account.publicPlayerNumber = "0000000002", s => s.account.connectionRevision = 3,
  s => s.account.currentPlayerId = "a", s => s.account.accountId = "other", s => s.sourceBinding.status = "Active", s => s.targetBinding.playerId = "other",
  s => s.environmentId = "production", s => s.rows["fs-stage-stage-001-r1"].metadata.acceptedAt++, s => s.rows["fs-stage-stage-001-r1"].metadata.submissionId = "other",
  s => s.rows["fs-stage-stage-001-r1"].score++, s => delete s.rows["fs-infinite-v2"], s => s.rows["fs-stage-stage-001-r1"].playerId = "b"])
  check(() => { const { before, after } = fixture(); mutate(after); assert.equal(compareTransfer(before, after), false); });
check(() => { const { before, after } = fixture(); before.rows["fs-stage-stage-001-r1"] = null; after.rows["fs-stage-stage-001-r1"] = null; assert.equal(compareTransfer(before, after), true); });
check(() => {
  const { before } = fixture(); before.account.currentPlayerId = "fixed";
  const legacy = { ...scope, legacyPlayerId: "fixed", rows: structuredClone(before.rows) };
  assert.equal(compareLegacy(legacy, before), true); before.rows["fs-stage-stage-001-r1"].metadata.acceptedAt++; assert.equal(compareLegacy(legacy, before), false);
});
check(() => {
  const { before } = fixture(); const legacy = { ...scope, legacyPlayerId: "different", rows: structuredClone(before.rows) };
  assert.equal(compareLegacy(legacy, before), false);
});
check(() => {
  const a = { ...scope, boardId: "fs-stage-stage-001-r1", publicPlayerNumber: "0000000001" }, b = { ...a, publicPlayerNumber: "0000000002" };
  assert.equal(compareDistinct(a, b), true); b.publicPlayerNumber = a.publicPlayerNumber; assert.equal(compareDistinct(a, b), false);
});
check(() => {
  const a = { ...scope, boardId: "fs-stage-stage-001-r1", publicPlayerNumber: "0000000001" }, b = { ...a, environmentId: "production", publicPlayerNumber: "0000000002" };
  assert.equal(compareDistinct(a, b), false);
});
check(() => { const { before } = fixture(); assert.deepEqual(parseSnapshot(JSON.stringify(before)), before); });
check(() => { const { before } = fixture(); assert.deepEqual(parseSnapshot("\uFEFF" + JSON.stringify(before)), before); });
check(() => { assert.throws(() => parseSnapshot("\uFEFF{broken}")); });
check(() => { assert.throws(() => parseSnapshot('{"value":"unexpected\uFEFF"}\uFEFF')); });
console.log(`PASS verification service snapshot comparison: ${cases} local cases; no remote requests.`);
