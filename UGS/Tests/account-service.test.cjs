// SDK double exercises the real store and service. No Unity or remote requests.
const assert = require("node:assert/strict");
const { createAccountStore, projectId, environmentId, key, guardKey } = require("../CloudCode/account-store");
const { createAccountService } = require("../CloudCode/account-service");
const tests = [];
const test = (name, run) => tests.push({ name, run });
const clone = value => JSON.parse(JSON.stringify(value));
const context = { projectId, environmentId, playerId: "A", serviceToken: "local-double" };
const scope = { schemaVersion: 1, projectId, environmentId };
const firstId = "00000000-0000-4000-8000-000000000001";
const secondId = "00000000-0000-4000-8000-000000000002";
function fixture() {
  const rows = new Map();
  const guards = new Map();
  const state = { writes: 0, failBefore: false, loseResponse: false, afterRead: null };
  function seed(kind, id, value) {
    rows.set(`fs8-${kind}-${id}`, { key, writeLock: "1", value: { ...scope, ...value } });
  }
  seed("player", "A", { playerId: "A", accountId: "C", connectionRevision: 1 });
  seed("account", "C", { accountId: "C", currentPlayerId: "A", connectionRevision: 1,
    status: "Active", publicPlayerNumber: "0000000001", leaderboardOwnerId: "fixed-owner" });
  seed("number", "0000000001", { accountId: "C" });
  seed("owner", "fixed-owner", { accountId: "C" });
  seed("ledger", "C", { accountId: "C", active: "", entries: [], best: {} });
  const api = {
    async getPrivateCustomItems(project, customId, keys) {
      assert.equal(project, projectId);
      const item = keys[0] === guardKey ? guards.get(customId) : rows.get(customId);
      const result = item ? [clone(item)] : [];
      if (state.afterRead) state.afterRead(customId);
      return { data: { results: result } };
    },
    async setPrivateCustomItem(project, customId, item) {
      assert.equal(project, projectId);
      if (item.key === guardKey) {
        const oldGuard = guards.get(customId);
        guards.set(customId, { ...clone(item), writeLock: String(oldGuard ? Number(oldGuard.writeLock) + 1 : 1) });
        return;
      }
      assert.ok(item.writeLock);
      state.writes++;
      if (state.failBefore) { state.failBefore = false; throw Error("injected-before-commit"); }
      const old = rows.get(customId);
      if (!old || old.writeLock !== item.writeLock) throw Error("local-cas-conflict");
      rows.set(customId, { ...clone(item), writeLock: String(Number(old.writeLock) + 1) });
      if (state.loseResponse) { state.loseResponse = false; throw Error("injected-after-commit"); }
    },
    async setPrivateCustomItemBatch(project, customId, batch) {
      assert.equal(project, projectId);
      const guard = guards.get(customId);
      assert.equal(batch.data[0].writeLock, guard.writeLock);
      assert.equal(rows.has(customId), false);
      guards.set(customId, { ...guard, writeLock: String(Number(guard.writeLock) + 1) });
      rows.set(customId, { ...clone(batch.data[1]), writeLock: "1" });
      state.writes++;
    }
  };
  function service(ctx = context) { return createAccountService(createAccountStore(ctx, api), () => 42); }
  return { rows, state, seed, api, service, value: (kind, id) => rows.get(`fs8-${kind}-${id}`).value };
}
test("guarded batch creates once and never overwrites the existing entity", async () => {
  const f = fixture();
  const store = createAccountStore(context, f.api);
  await store.create("player", "B", { ...scope, accountId: "B-original" });
  await store.create("player", "B", { ...scope, accountId: "other" });
  assert.equal(f.value("player", "B").accountId, "B-original");
  assert.equal(f.state.writes, 1);
});
test("context mismatch is rejected before storage access", async () => {
  const f = fixture();
  for (const change of [{ projectId: "other" }, { environmentId: "other" }, { playerId: "" }, { serviceToken: "" }])
    assert.throws(() => f.service({ ...context, ...change }), /ContextMismatch/);
});
test("missing binding fails closed without reconstructing another account", async () => {
  const f = fixture(); f.rows.delete("fs8-player-A");
  await assert.rejects(f.service().resolve(context), /AccountUnavailable/);
  assert.equal(f.state.writes, 0);
});
test("stored environment mismatch is rejected", async () => {
  const f = fixture(); f.value("account", "C").environmentId = "other";
  await assert.rejects(f.service().resolve(context), /StoredScopeMismatch/);
});
test("stale reverse binding cannot grant authority", async () => {
  const f = fixture(); f.value("account", "C").currentPlayerId = "B";
  await assert.rejects(f.service().resolve(context), /ActiveDeviceRequired/);
});
test("revision mismatch and malformed revision fail closed", async () => {
  for (const revision of [2, -1, null]) {
    const f = fixture(); f.value("account", "C").connectionRevision = revision;
    await assert.rejects(f.service().resolve(context), /ActiveDeviceRequired|AccountConflict/);
  }
});
test("client selected owner/account does not select storage authority", async () => {
  const f = fixture();
  const resolved = await f.service().resolve({ ...context, accountId: "other", leaderboardOwnerId: "other" });
  assert.equal(resolved.account.value.accountId, "C");
  assert.equal(resolved.account.value.leaderboardOwnerId, "fixed-owner");
});
test("existing public number survives new service instances with exact zeros", async () => {
  const f = fixture();
  assert.deepEqual(await f.service().getPublicNumber(context), { status: "Success", publicPlayerNumber: "0000000001" });
  assert.deepEqual(await f.service().getPublicNumber(context), { status: "Success", publicPlayerNumber: "0000000001" });
  assert.equal(f.state.writes, 0);
});
test("maximum number is returned as a string", async () => {
  const f = fixture(); f.value("account", "C").publicPlayerNumber = "9999999999";
  f.seed("number", "9999999999", { accountId: "C" });
  assert.equal((await f.service().getPublicNumber(context)).publicPlayerNumber, "9999999999");
});
test("missing/malformed number and conflicting mapping return no alternate ID", async () => {
  const f = fixture(); f.value("account", "C").publicPlayerNumber = "1";
  await assert.rejects(f.service().getPublicNumber(context), /PublicNumberUnavailable/);
  f.value("account", "C").publicPlayerNumber = "0000000001";
  f.value("number", "0000000001").accountId = "other";
  await assert.rejects(f.service().getPublicNumber(context), /AccountConflict/);
});
test("number response is discarded when connection changes during lookup", async () => {
  const f = fixture(); f.state.afterRead = id => {
    if (id === "fs8-number-0000000001") f.value("account", "C").currentPlayerId = "B";
  };
  await assert.rejects(f.service().getPublicNumber(context), /ActiveDeviceRequired/);
});
test("two submission reservations have one CAS winner", async () => {
  const f = fixture(); const service = f.service();
  const result = await Promise.allSettled([service.reserveSubmission(context, firstId), service.reserveSubmission(context, secondId)]);
  assert.equal(result.filter(r => r.status === "fulfilled").length, 1);
  assert.equal(f.value("account", "C").onlineOperation.id, firstId);
  await assert.rejects(service.reserveSubmission(context, secondId), /AccountBusy/);
});
test("response lost after reservation recovers same operation after restart", async () => {
  const f = fixture(); f.state.loseResponse = true;
  await assert.rejects(f.service().reserveSubmission(context, firstId), /injected-after-commit/);
  assert.equal((await f.service().reserveSubmission(context, firstId)).disposition, "Resume");
  assert.equal(f.state.writes, 1);
  assert.equal(f.value("account", "C").onlineOperation.reservedAt, 42);
});
test("failure before reservation can retry without inventing committed state", async () => {
  const f = fixture(); f.state.failBefore = true;
  await assert.rejects(f.service().reserveSubmission(context, firstId), /injected-before-commit/);
  assert.equal(f.value("account", "C").onlineOperation, undefined);
  assert.equal((await f.service().reserveSubmission(context, firstId)).disposition, "Reserved");
});
test("reservation and ledger active each prevent transfer start", async () => {
  const f = fixture(); await f.service().reserveSubmission(context, firstId);
  await assert.rejects(f.service().assertTransferCanStart(context), /AccountBusy/);
  const g = fixture(); g.value("ledger", "C").active = firstId;
  await assert.rejects(g.service().assertTransferCanStart(context), /AccountBusy/);
});
test("transfer precheck token is fenced by a competing submission CAS", async () => {
  const f = fixture(); const account = await f.service().assertTransferCanStart(context);
  await f.service().reserveSubmission(context, firstId);
  await assert.rejects(createAccountStore(context, f.api).compareExchange(account,
    { ...account.value, status: "TransferPending" }), /local-cas-conflict/);
  assert.equal(f.value("account", "C").status, "Active");
});
test("TransferPending blocks reads and submission reservations", async () => {
  const f = fixture(); f.value("account", "C").status = "TransferPending";
  await assert.rejects(f.service().getPublicNumber(context), /TransferPending/);
  await assert.rejects(f.service().reserveSubmission(context, firstId), /TransferPending/);
});
test("missing or mixed ledger prevents transfer without writes", async () => {
  const f = fixture(); f.rows.delete("fs8-ledger-C");
  await assert.rejects(f.service().assertTransferCanStart(context), /LedgerUnavailable/);
  f.seed("ledger", "C", { accountId: "other", active: "" });
  await assert.rejects(f.service().assertTransferCanStart(context), /LedgerConflict/);
  assert.equal(f.state.writes, 0);
});
test("missing writeLock cannot become unconditional update", async () => {
  const f = fixture(); f.rows.get("fs8-account-C").writeLock = "";
  await assert.rejects(f.service().reserveSubmission(context, firstId), /WriteLockUnavailable/);
  assert.equal(f.state.writes, 0);
});
test("reservation release requires the matching terminal ledger and is restart safe", async () => {
  const f = fixture(); const service = f.service();
  await service.reserveSubmission(context, firstId);
  await assert.rejects(service.releaseSubmission(context, firstId), /SubmissionNotTerminal/);
  await assert.rejects(service.releaseSubmission(context, secondId), /AccountBusy/);
  f.value("ledger", "C").entries.push({ id: firstId, status: "Submitted" });
  f.state.loseResponse = true;
  await assert.rejects(service.releaseSubmission(context, firstId), /injected-after-commit/);
  assert.equal(await f.service().releaseSubmission(context, firstId), "AlreadyReleased");
  assert.ok(await service.assertTransferCanStart(context));
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
