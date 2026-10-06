const assert = require("node:assert/strict");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore, guardKey, projectId, environmentId } = require("../CloudCode/account-store");
const { createLegacyAccountStore, ledgerKey, boards } = require("../CloudCode/legacy-account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const fs = require("node:fs");
const vm = require("node:vm");
const path = require("node:path");
const tests = [];
const test = (name, run) => tests.push({ name, run });
function service(f, playerId = "A", generator) {
  const context = { ...f.context, playerId };
  return { context, api: createAccountProvisioningService(createAccountStore(context, f.save),
    createLegacyAccountStore(context, f.save, f.leaderboard), generator) };
}
async function ensure(f, playerId = "A") {
  const { context, api } = service(f, playerId);
  // Client/server retry after a transient failure; no timing assumptions.
  let last;
  for (let i = 0; i < 40; i++) {
    try { return await api.ensure(context); } catch (error) { last = error; }
  }
  throw last;
}
function account(f, player = "A") {
  const binding = f.value(`fs8-player-${player}`);
  return f.value(`fs8-account-${binding.accountId}`);
}
function oldSubmit(f) {
  const sandbox = { module: { exports: {} }, require: name => {
    if (name === "@unity-services/cloud-save-1.4") return { DataApi: class { constructor() { return f.save; } } };
    if (name === "@unity-services/leaderboards-1.1") return { LeaderboardsApi: class { constructor() { return f.leaderboard; } } };
    throw Error("unexpected dependency");
  }};
  // Retained historical bridge, not the current C endpoint. These tests model
  // the explicitly drained pre-cutover deployment generation.
  vm.runInNewContext(fs.readFileSync(path.join(__dirname, "../Migration/legacy-barrier-submit.js"), "utf8"), sandbox);
  const request = { boardId: boards[0], rulesVersion: 1,
    submissionId: "00000000-0000-4000-8000-000000000001", score: 10,
    runDurationMilliseconds: 0, baseDistanceScore: 0, momentumBonus: 0, distanceScore: 0,
    collectibleScore: 0, totalScore: 0, maximumMomentumMultiplier: 1 };
  return () => sandbox.module.exports({ params: { request: JSON.stringify(request) }, context: f.context });
}
test("new account issues an exact number and independent server UUID owner", async () => {
  const f = fixture(); const result = await ensure(f);
  assert.equal(result.publicPlayerNumber, "0000000001");
  const value = account(f);
  assert.equal(value.status, "Active");
  assert.notEqual(value.accountId, value.leaderboardOwnerId);
  assert.notEqual(value.leaderboardOwnerId, "A");
  assert.match(value.leaderboardOwnerId, /^[0-9a-f-]{36}$/);
  assert.equal(f.scores.size, 0);
});
test("restart returns the confirmed number without another allocation", async () => {
  const f = fixture(); const first = await ensure(f); const writes = f.state.writes;
  assert.deepEqual(await ensure(f), first);
  assert.equal(f.state.writes, writes);
});
test("same-player concurrent creation converges to one Account", async () => {
  const f = fixture(); const results = await Promise.all([ensure(f), ensure(f), ensure(f)]);
  assert.equal(new Set(results.map(r => r.publicPlayerNumber)).size, 1);
  assert.equal([...f.privateItems.keys()].filter(key => key.startsWith("fs8-account-") && key.endsWith("/fs_account_v1")).length, 1);
});
test("different players concurrently receive distinct non-reused numbers", async () => {
  const f = fixture(); const results = await Promise.all([ensure(f, "A"), ensure(f, "B"), ensure(f, "C")]);
  assert.equal(new Set(results.map(r => r.publicPlayerNumber)).size, 3);
  const old = account(f).publicPlayerNumber;
  account(f).currentPlayerId = "other";
  const next = await ensure(f, "D");
  assert.notEqual(next.publicPlayerNumber, old);
  await assert.rejects(service(f).api.ensure(f.context), /ActiveDeviceRequired/);
});
test("existing ledger and rows retain owner, receipts and exact metadata", async () => {
  const f = fixture(); const submissionId = "00000000-0000-4000-8000-000000000001";
  const ledger = { version: 1, active: "", entries: [{ id: submissionId, payload: "original",
    status: "Submitted", reason: "Accepted", acceptedAt: 123 }],
    best: { [boards[0]]: { score: 10, acceptedAt: 123, id: submissionId } } };
  f.seedProtected("A", ledgerKey, ledger);
  const row = { playerId: "A", score: 10, metadata: { submissionId, acceptedAt: 123, extra: "keep" } };
  f.scores.set(`${boards[0]}/A`, clone(row));
  await ensure(f);
  const value = account(f);
  assert.equal(value.leaderboardOwnerId, "A");
  assert.deepEqual(f.value(`fs8-ledger-${value.accountId}`).entries, ledger.entries);
  assert.deepEqual(f.value(`fs8-ledger-${value.accountId}`).best, ledger.best);
  assert.deepEqual(f.scores.get(`${boards[0]}/A`), row);
  assert.deepEqual(f.protectedItems.get(`A/${ledgerKey}`).value.entries, ledger.entries);
});
test("every legacy cutover write resumes while preserving all source receipts", async () => {
  function seedLegacy(f) {
    f.seedProtected("A", ledgerKey, { version: 1, active: "", entries: [
      { id: "old", payload: "original", status: "Submitted", reason: "Accepted", acceptedAt: 123 },
      { id: "rejected", payload: "invalid", status: "Rejected", reason: "InvalidScore", acceptedAt: 124 }
    ], best: { [boards[0]]: { score: 10, acceptedAt: 123, id: "old" } } });
    f.scores.set(`${boards[0]}/A`, { playerId: "A", score: 10,
      metadata: { submissionId: "old", acceptedAt: 123 } });
  }
  const baseline = fixture(); seedLegacy(baseline); await ensure(baseline);
  for (const mode of ["failAt", "loseAt"]) {
    for (let step = 1; step <= baseline.state.writes; step++) {
      const f = fixture(); seedLegacy(f);
      const before = clone(f.protectedItems.get(`A/${ledgerKey}`).value);
      f.state[mode] = step;
      await assert.rejects(service(f).api.ensure(f.context));
      await ensure(f);
      const value = account(f);
      assert.equal(value.leaderboardOwnerId, "A");
      assert.deepEqual(f.value(`fs8-ledger-${value.accountId}`).entries, before.entries);
      assert.deepEqual(f.value(`fs8-ledger-${value.accountId}`).best, before.best);
    }
  }
});
test("unrelated leaderboard rank changes do not invalidate migration snapshots", async () => {
  const f = fixture(); let rank = 0;
  f.leaderboard.getLeaderboardPlayerScore = async (project, board, player) => {
    if (board !== boards[0]) throw Object.assign(Error("absent"), { response: { status: 404 } });
    return { data: { playerId: player, score: 10, rank: rank++, metadata: { submissionId: "old", acceptedAt: 123 } } };
  };
  await ensure(f);
  assert.equal(account(f).status, "Active");
});
test("legacy row with no ledger is lazily provisioned and keeps its owner", async () => {
  const f = fixture(); f.scores.set(`${boards[0]}/A`, { playerId: "A", score: 10,
    metadata: { submissionId: "old", acceptedAt: 123 } });
  await ensure(f);
  assert.equal(account(f).leaderboardOwnerId, "A");
  assert.equal(f.value(`fs8-ledger-${account(f).accountId}`).best[boards[0]].acceptedAt, 123);
});
test("in-flight legacy submissions block cutover and preserve source", async () => {
  const f = fixture(); const value = { version: 1, active: "old", entries: [], best: {} };
  f.seedProtected("A", ledgerKey, value);
  await assert.rejects(service(f).api.ensure(f.context), /EarlierSubmissionPending/);
  assert.deepEqual(f.protectedItems.get(`A/${ledgerKey}`).value, value);
  assert.equal([...f.privateItems.keys()].some(key => key.startsWith("fs8-number-")), false);
});
test("legacy submission winning the source CAS is preserved by cutover retry", async () => {
  const f = fixture(); f.seedProtected("A", ledgerKey, { version: 1, active: "", entries: [], best: {} });
  f.state.beforeWrite = async ({ items, data }) => {
    if (items === f.protectedItems && data.some(item => item.key === ledgerKey && item.value.migration)) {
      f.state.beforeWrite = null;
      assert.equal((await oldSubmit(f)()).status, "Submitted");
    }
  };
  await assert.rejects(service(f).api.ensure(f.context), /cas-conflict/);
  await ensure(f);
  assert.equal(f.value(`fs8-ledger-${account(f).accountId}`).entries[0].status, "Submitted");
  assert.equal(f.value(`fs8-ledger-${account(f).accountId}`).best[boards[0]].score, 10);
});
test("cutover winning the source CAS prevents a stale legacy projection", async () => {
  const f = fixture(); f.seedProtected("A", ledgerKey, { version: 1, active: "", entries: [], best: {} });
  f.state.beforeWrite = async ({ items, data }) => {
    if (items === f.protectedItems && data.some(item => item.key === ledgerKey && item.value.active)) {
      f.state.beforeWrite = null;
      await ensure(f);
    }
  };
  assert.equal((await oldSubmit(f)()).status, "TransientFailure");
  assert.equal(f.scores.size, 0);
  assert.equal(f.value(`fs8-ledger-${account(f).accountId}`).entries.length, 0);
});
test("unknown score metadata and service failures cannot activate an account", async () => {
  const f = fixture(); f.scores.set(`${boards[0]}/A`, { playerId: "A", score: 10 });
  await assert.rejects(service(f).api.ensure(f.context), /MissingServerMetadata/);
  const g = fixture(); g.leaderboard.getLeaderboardPlayerScore = async () => { throw Error("offline"); };
  await assert.rejects(service(g).api.ensure(g.context), /offline/);
});
test("missing board is not mistaken for a player without legacy scores", async () => {
  const f = fixture();
  f.leaderboard.getLeaderboardScores = async () => { throw Object.assign(Error("board-missing"), { response: { status: 404 } }); };
  await assert.rejects(service(f).api.ensure(f.context), /board-missing/);
  assert.equal([...f.privateItems.keys()].some(key => key.startsWith("fs8-number-")), false);
});
test("every write boundary recovers after failure before or after commit", async () => {
  const baseline = fixture(); await ensure(baseline);
  const count = baseline.state.writes;
  for (const mode of ["failAt", "loseAt"]) {
    for (let step = 1; step <= count; step++) {
      const f = fixture(); f.state[mode] = step;
      await assert.rejects(service(f).api.ensure(f.context));
      const result = await ensure(f);
      assert.equal(account(f).status, "Active", `${mode}/${step}`);
      assert.equal(result.publicPlayerNumber, account(f).publicPlayerNumber);
      assert.equal((await ensure(f)).publicPlayerNumber, result.publicPlayerNumber);
      assert.equal(f.scores.size, 0);
    }
  }
});
test("last public number succeeds and exhaustion cannot wrap or reuse", async () => {
  const f = fixture(); f.seedPrivate("fs8-allocator-public", "fs_account_v1",
    { schemaVersion: 1, projectId, environmentId, next: 9999999999 });
  assert.equal((await ensure(f)).publicPlayerNumber, "9999999999");
  await assert.rejects(service(f, "B").api.ensure({ ...f.context, playerId: "B" }), /PublicNumberExhausted/);
});
test("immutable guard initialization arriving late cannot reset numbered data", async () => {
  const f = fixture(); await ensure(f);
  const before = clone(account(f));
  const binding = f.value("fs8-player-A");
  await f.save.setPrivateCustomItem(projectId, `fs8-account-${binding.accountId}`,
    { key: guardKey, value: { schemaVersion: 1, projectId, environmentId } });
  assert.deepEqual(account(f), before);
  assert.equal((await ensure(f)).publicPlayerNumber, before.publicPlayerNumber);
});
test("concurrent guard initialization and create cannot overwrite the winner", async () => {
  const f = fixture(); const store = createAccountStore(f.context, f.save);
  const scope = { schemaVersion: 1, projectId, environmentId };
  const result = await Promise.allSettled([
    store.create("number", "0000000001", { ...scope, accountId: "first" }),
    store.create("number", "0000000001", { ...scope, accountId: "second" })]);
  assert.equal(result.filter(r => r.status === "fulfilled").length, 1);
  const before = clone(f.value("fs8-number-0000000001"));
  await store.create("number", "0000000001", { ...scope, accountId: "third" });
  assert.deepEqual(f.value("fs8-number-0000000001"), before);
});
test("generator collisions and client ownership overrides never mix accounts", async () => {
  const f = fixture(); const { api, context } = service(f, "A", () => "00000000-0000-4000-8000-000000000001");
  await assert.rejects(api.ensure(context), /InvalidGeneratedId/);
  assert.equal(f.state.writes, 0);
  await ensure(f);
  const before = clone(account(f));
  await service(f).api.ensure({ ...f.context, accountId: "other", publicPlayerNumber: "9999999999", leaderboardOwnerId: "other" });
  assert.deepEqual(account(f), before);
});
test("different players receiving colliding generated Account IDs cannot merge", async () => {
  const f = fixture(); const sharedId = "00000000-0000-4000-8000-000000000001";
  let n = 10;
  const generator = () => { n++; return n % 2 ? sharedId : `00000000-0000-4000-8000-${String(n).padStart(12,"0")}`; };
  const a = service(f, "A", generator); await a.api.ensure(a.context);
  const before = clone(account(f));
  const b = service(f, "B", generator);
  await assert.rejects(b.api.ensure(b.context), /ActiveDeviceRequired/);
  assert.deepEqual(account(f), before);
});
(async () => {
  let failed = 0;
  for (const { name, run } of tests) {
    try { await run(); console.log(`PASS ${name}`); }
    catch (error) { failed++; console.error(`FAIL ${name}: ${error.stack}`); }
  }
  console.log(`${tests.length - failed}/${tests.length} passed`);
  process.exitCode = failed ? 1 : 0;
})();
