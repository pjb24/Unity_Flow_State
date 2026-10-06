// Offline reproduction of JSON service responses. No .NET/Unity build or remote calls.
const assert = require("node:assert/strict");
const { fixture } = require("./account-sdk-double.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createRecordQueryService } = require("../CloudCode/query-records");
const { sameJsonValue } = require("../CloudCode/json-value-policy");

function reorder(value) {
  if (Array.isArray(value)) return value.map(reorder);
  if (value === null || typeof value !== "object") return value;
  const result = {};
  for (const key of Object.keys(value).sort()) result[key] = reorder(value[key]);
  return result;
}
async function run() {
  const snapshot = { ledger: { entries: [{ id: "synthetic-id", acceptedAt: 100 }], best: {} },
    scores: { stage: { score: 60000, metadata: { acceptedAt: 100, submissionId: "synthetic-id" } } } };
  assert.equal(sameJsonValue(snapshot, reorder(snapshot)), true);
  for (const mutate of [
    value => { value.scores.stage.score++; },
    value => { value.scores.stage.metadata.acceptedAt++; },
    value => { value.scores.stage.metadata.submissionId = "different"; },
    value => { delete value.ledger.best; },
    value => { value.extra = true; },
    value => { value.ledger.entries.push({ id: "new" }); }
  ]) {
    const value = JSON.parse(JSON.stringify(snapshot)); mutate(value);
    assert.equal(sameJsonValue(snapshot, value), false, "changed source must still be rejected");
  }
  for (const [left, right] of [[1, "1"], [null, {}], [[], {}], [[1, 2], [2, 1]],
    [{ a: undefined }, {}], [NaN, NaN], [new Date(0), {}]]) assert.equal(sameJsonValue(left, right), false);
  assert.equal(sameJsonValue(JSON.parse('{"__proto__":{"a":1}}'), JSON.parse('{"__proto__":{"a":2}}')), false);
  for (const legacy of [false, true]) {
    const f = fixture();
    for (const method of ["getPrivateCustomItems", "getProtectedItems"]) {
      const original = f.save[method];
      f.save[method] = async (...args) => reorder(await original(...args));
    }
    if (legacy) {
      for (const [board, score] of [["fs-stage-stage-001-r1", 60000], ["fs-infinite-v2", 10]])
        f.scores.set(board + "/old-player", { playerId: "old-player", score,
          metadata: { submissionId: "11111111-1111-4111-8111-111111111111", acceptedAt: 100 } });
    }
    const context = f.context;
    const store = createAccountStore(context, f.save);
    const provision = createAccountProvisioningService(store, createLegacyAccountStore(context, f.save, f.leaderboard));
    await provision.ensure(context);
    const before = JSON.stringify([...f.scores]);
    const query = createRecordQueryService(store, f.leaderboard,
      playerId => provision.ensure({ ...context, playerId }));
    const result = await query({ context, params: { request: JSON.stringify({
      boardId: "fs-stage-stage-001-r1", kind: "me", limit: 1 }) } });
    assert.equal(result.status, "Success", "JSON key ordering must not invalidate provisioning/query: " + JSON.stringify(result));
    assert.deepEqual(result.entries, []);
    assert.equal(JSON.stringify([...f.scores]), before, "legacy scores/metadata must not change");
  }
  // Resume every lazy-provisioning write boundary with real JSON key reordering.
  async function prepare() {
    const f = fixture();
    for (const method of ["getPrivateCustomItems", "getProtectedItems"]) {
      const original = f.save[method];
      f.save[method] = async (...args) => reorder(await original(...args));
    }
    f.scores.set("fs-stage-stage-001-r1/old-player", { playerId: "old-player", score: 60000,
      metadata: { submissionId: "11111111-1111-4111-8111-111111111111", acceptedAt: 100 } });
    const store = createAccountStore(f.context, f.save);
    const provision = createAccountProvisioningService(store, createLegacyAccountStore(f.context, f.save, f.leaderboard));
    await provision.ensure(f.context);
    const query = createRecordQueryService(store, f.leaderboard, playerId => provision.ensure({ ...f.context, playerId }));
    return { f, call: () => query({ context: f.context, params: { request:
      JSON.stringify({ boardId: "fs-stage-stage-001-r1", kind: "me", limit: 1 }) } }) };
  }
  const baseline = await prepare(), initialWrites = baseline.f.state.writes;
  assert.equal((await baseline.call()).status, "Success");
  const writes = baseline.f.state.writes - initialWrites;
  for (const failure of ["failAt", "loseAt"]) for (let boundary = 1; boundary <= writes; boundary++) {
    const { f, call } = await prepare();
    const scores = JSON.stringify([...f.scores]);
    f.state[failure] = f.state.writes + boundary;
    await call(); f.state[failure] = 0;
    const resumed = await call();
    assert.equal(resumed.status, "Success", "partial lazy projection must resume after " + failure + " at write " + boundary + ": " + JSON.stringify(resumed));
    assert.equal(JSON.stringify([...f.scores]), scores);
  }
  // A partial account is resumable only for its original service-fetched owner.
  for (const scenario of ["nonlegacy", "changedMapping", "changedOwner", "valid"]) {
    const { f, call } = await prepare();
    assert.equal((await call()).status, "Success");
    const mapping = f.value("fs8-owner-old-player");
    const account = f.value("fs8-account-" + mapping.accountId);
    account.status = "Preparing";
    if (scenario === "nonlegacy") account.initialPlayerId = "different-original-player";
    let resumed = 0;
    const store = createAccountStore(f.context, f.save);
    const query = createRecordQueryService(store, f.leaderboard, async owner => {
      assert.equal(owner, "old-player"); resumed++;
      account.status = "Active";
      if (scenario === "changedMapping") mapping.accountId = "different-account";
      if (scenario === "changedOwner") account.leaderboardOwnerId = "different-owner";
    });
    const scores = JSON.stringify([...f.scores]);
    const result = await query({ context: f.context, params: { request:
      JSON.stringify({ boardId: "fs-stage-stage-001-r1", kind: "top", limit: 10 }) } });
    assert.equal(resumed, scenario === "nonlegacy" ? 0 : 1);
    assert.equal(result.status, scenario === "valid" ? "Success" : "TransientFailure", scenario);
    if (scenario !== "valid") {
      assert.equal(result.queryFault, "AccountConflict");
      assert.deepEqual(result.entries, []);
    }
    assert.equal(JSON.stringify([...f.scores]), scores);
  }
  console.log("PASS JSON service roundtrip: reordered nested objects, fresh caller, both legacy boards, " + (writes * 2) + " interrupted write boundaries, no score writes");
}
run().catch(error => { console.error(error); process.exitCode = 1; });
