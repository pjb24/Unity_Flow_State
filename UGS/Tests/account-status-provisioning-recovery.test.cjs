"use strict";
// Actual status endpoint and provisioning with synthetic SDK storage only.
const test = require("node:test"), assert = require("node:assert/strict");
const fs = require("node:fs"), vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createTransferCompletionService } = require("../CloudCode/transfer-completion-service");
const { createTransferLifecycleService } = require("../CloudCode/transfer-lifecycle-service");
const { createAccountService } = require("../CloudCode/account-service");
const accountId = "00000000-0000-4000-8000-000000000001";
const ownerId = "00000000-0000-4000-8000-000000000002";
function setup(executionScope) {
  const f = fixture(executionScope); f.store = createAccountStore(f.context, f.save);
  f.provisioningCalls = 0; f.generated = 10;
  const module = { exports: {} };
  vm.runInNewContext(fs.readFileSync(require.resolve("../CloudCode/get-account-transfer-status.js"), "utf8"), {
    module, require: name => {
      if (name === "./account-store") return { createAccountStore: () => f.store };
      if (name === "./leaderboard-owner-policy") return require("../CloudCode/leaderboard-owner-policy");
      if (name === "./transfer-completion-service") return { createTransferCompletionService };
      if (name === "./transfer-lifecycle-service") return { createTransferLifecycleService };
      if (name === "./legacy-account-store") return {
        createLegacyAccountStore: context => createLegacyAccountStore(context, f.save, f.leaderboard) };
      if (name === "./account-provisioning-service") return { createAccountProvisioningService: (store, legacy) => {
        const service = createAccountProvisioningService(store, legacy,
          () => `00000000-0000-4000-8000-${String(++f.generated).padStart(12, "0")}`);
        return { ensure: async context => { f.provisioningCalls++; return service.ensure(context); } };
      } };
      throw Error("Unexpected dependency");
    }
  });
  f.run = () => module.exports({ context: f.context, logger: { warning() {} } });
  f.initialBinding = () => ({ schemaVersion: 1, ...f.store.scope, playerId: f.context.playerId,
    accountId, generatedOwner: ownerId, connectionRevision: 1 });
  f.seedBinding = value => f.seedPrivate("fs8-player-" + f.context.playerId, "fs_account_v1", value);
  return f;
}
async function ready(f) {
  let response;
  for (let i = 0; i < 6; i++) {
    response = await f.run();
    if (response.status === "Active") return response;
  }
  assert.fail("Synthetic status recovery did not reach Active");
}
test("status provisions a new caller and repeated status does not mutate ready data", async () => {
  const f = setup(); await ready(f);
  const before = clone([...f.privateItems]), writes = f.state.writes, calls = f.provisioningCalls;
  assert.equal((await f.run()).status, "Active");
  assert.equal(f.state.writes, writes); assert.equal(f.provisioningCalls, calls);
  assert.deepEqual(clone([...f.privateItems]), before);
  assert.match((await createAccountService(f.store).getPublicNumber(f.context)).publicPlayerNumber, /^\d{10}$/);
});
test("orphan initial binding resumes the same Account and owner without generating replacement identities", async () => {
  const f = setup(), binding = f.initialBinding(); f.seedBinding(binding);
  await ready(f);
  assert.equal(f.generated, 10); assert.equal(f.provisioningCalls, 1);
  assert.deepEqual(f.value("fs8-player-" + f.context.playerId), binding);
  const account = f.value("fs8-account-" + accountId);
  assert.equal(account.accountId, accountId); assert.equal(account.leaderboardOwnerId, ownerId);
  assert.equal(account.status, "Active"); assert.equal(account.currentPlayerId, f.context.playerId);
  assert.equal((await createAccountService(f.store).getPublicNumber(f.context)).publicPlayerNumber, account.publicPlayerNumber);
});
test("status resumes every before/after-commit provisioning interruption including Preparing and allocated numbers", async t => {
  const baseline = setup(); await ready(baseline);
  const boundaries = baseline.state.writes;
  let missing = 0, preparing = 0, numberedPreparing = 0;
  for (const mode of ["failAt", "loseAt"]) for (let step = 1; step <= boundaries; step++) {
    const f = setup(); f.state[mode] = step;
    await f.run();
    const item = f.privateItems.get("fs8-player-" + f.context.playerId + "/fs_account_v1");
    const binding = item && clone(item.value);
    const accountItem = binding && f.privateItems.get("fs8-account-" + binding.accountId + "/fs_account_v1");
    const account = accountItem && clone(accountItem.value);
    if (binding && !account) missing++;
    if (account && account.status === "Preparing") preparing++;
    if (account && account.status === "Preparing" && account.publicPlayerNumber) numberedPreparing++;
    f.state[mode] = 0; await ready(f);
    const finalBinding = f.value("fs8-player-" + f.context.playerId);
    if (binding) assert.deepEqual(finalBinding, binding, mode + "/" + step);
    const final = f.value("fs8-account-" + finalBinding.accountId);
    if (account && account.publicPlayerNumber) assert.equal(final.publicPlayerNumber, account.publicPlayerNumber);
    assert.equal(final.status, "Active"); assert.equal(f.scores.size, 0);
    const result = await createAccountService(f.store).getPublicNumber(f.context);
    assert.equal(result.publicPlayerNumber, final.publicPlayerNumber);
    const before = clone([...f.privateItems]), writes = f.state.writes;
    await ready(f); assert.equal(f.state.writes, writes); assert.deepEqual(clone([...f.privateItems]), before);
  }
  assert.ok(missing > 0 && preparing > 0 && numberedPreparing > 0);
  t.diagnostic(`Exercised ${boundaries} write boundaries in both before/after-commit modes.`);
});
test("Preparing legacy cutover resumes without changing the original row, owner or allocated number", async () => {
  const f = setup(); f.seedBinding(f.initialBinding());
  const row = { playerId: f.context.playerId, score: 123,
    metadata: { acceptedAt: 456, submissionId: "synthetic-legacy-run" } };
  f.scores.set("fs-stage-stage-001-r1/" + f.context.playerId, clone(row));
  f.state.beforeWrite = async ({ id, data }) => {
    if (id === "fs8-account-" + accountId && data.some(item => item.value && item.value.status === "Active")) {
      f.state.beforeWrite = null; throw Error("synthetic interruption before activation");
    }
  };
  assert.equal((await f.run()).status, "TransientFailure");
  const before = clone(f.value("fs8-account-" + accountId));
  assert.equal(before.status, "Preparing"); assert.match(before.publicPlayerNumber, /^\d{10}$/);
  await ready(f);
  const after = f.value("fs8-account-" + accountId);
  assert.equal(after.leaderboardOwnerId, f.context.playerId);
  assert.equal(after.publicPlayerNumber, before.publicPlayerNumber);
  assert.deepEqual(f.scores.get("fs-stage-stage-001-r1/" + f.context.playerId), row);
  assert.equal(f.value("fs8-ledger-" + accountId).best["fs-stage-stage-001-r1"].acceptedAt, row.metadata.acceptedAt);
});
test("concurrent status recovery keeps the original binding and creates only its Account", async () => {
  const f = setup(), binding = f.initialBinding(); f.seedBinding(binding);
  await Promise.all([f.run(), f.run()]); await ready(f);
  assert.deepEqual(f.value("fs8-player-" + f.context.playerId), binding);
  assert.equal(f.value("fs8-account-" + accountId).leaderboardOwnerId, ownerId);
  assert.equal([...f.privateItems.keys()].filter(k => k.startsWith("fs8-account-") && k.endsWith("/fs_account_v1")).length, 1);
  assert.equal(f.generated, 10);
});
test("inactive, transferred, malformed or mismatched bindings never create a missing Account", async () => {
  for (const mutate of [b => b.status = "Inactive", b => b.status = "Active", b => b.status = "Preparing",
    b => b.status = null, b => b.connectionRevision = 2, b => b.playerId = "other",
    b => b.transferOperation = { targetAccountId: accountId }, b => b.accountId = "invalid",
    b => b.generatedOwner = "invalid", b => b.generatedOwner = b.accountId,
    b => b.environmentId = "other", b => delete b.generatedOwner, b => b.generatedOwner = null,
    b => b.accountId = null, b => b.schemaVersion = 2]) {
    const f = setup(), binding = f.initialBinding(); mutate(binding); f.seedBinding(binding);
    const before = clone([...f.privateItems]);
    assert.equal((await f.run()).status, "TransientFailure");
    assert.equal(f.provisioningCalls, 0); assert.equal(f.state.writes, 0);
    assert.deepEqual(clone([...f.privateItems]), before);
  }
});
test("Preparing Accounts with changed identity, revision, owner or operations are not provisioned", async () => {
  for (const mutate of [a => a.initialPlayerId = "other", a => a.currentPlayerId = "other",
    a => a.connectionRevision = 2, a => a.accountId = ownerId, a => a.leaderboardOwnerId = "other",
    a => a.transfer = { status: "Pending" }, a => a.onlineOperation = { id: "other" },
    a => a.status = "Detached", a => a.status = "TransferJoining"]) {
    const f = setup(); f.seedBinding(f.initialBinding());
    const account = { schemaVersion: 1, ...f.store.scope, accountId, initialPlayerId: f.context.playerId,
      currentPlayerId: f.context.playerId, connectionRevision: 1, status: "Preparing", leaderboardOwnerId: ownerId };
    mutate(account); f.seedPrivate("fs8-account-" + accountId, "fs_account_v1", account);
    const before = clone([...f.privateItems]);
    assert.equal((await f.run()).status, "TransientFailure");
    assert.equal(f.provisioningCalls, 0); assert.equal(f.state.writes, 0);
    assert.deepEqual(clone([...f.privateItems]), before);
  }
});
test("Account read failure is not treated as absence and never starts provisioning", async () => {
  for (const status of [403, 404, 503]) {
    const f = setup(); f.seedBinding(f.initialBinding());
    const read = f.store.read;
    f.store.read = async (kind, id) => {
      if (kind === "account") throw Object.assign(Error("synthetic denied"), { response: { status } });
      return read(kind, id);
    };
    assert.equal((await f.run()).status, "TransientFailure");
    assert.equal(f.provisioningCalls, 0); assert.equal(f.state.writes, 0);
  }
});
test("migration destination conflicts fail closed across different players, Account IDs and execution scopes", async () => {
  const uuid = n => `10000000-0000-4000-8000-${String(n).padStart(12, "0")}`;
  for (let n = 1; n <= 6; n++) for (const mismatch of [false, true]) {
    const f = setup({ projectId: uuid(100 + n), environmentId: uuid(200 + n) });
    f.context.playerId = "synthetic-player-" + n;
    const binding = { ...f.initialBinding(), accountId: uuid(n * 3), generatedOwner: uuid(n * 3 + 1) };
    f.seedBinding(binding);
    f.seedProtected(f.context.playerId, "fs_submission_ledger_v1", { version: 1, active: "", entries: [], best: {},
      migration: { ...f.store.scope, accountId: mismatch ? uuid(n * 3 + 2) : binding.accountId, hadLegacyLedger: false } });
    const originalPrivate = clone([...f.privateItems]), originalProtected = clone([...f.protectedItems]);
    if (mismatch) {
      for (let attempt = 0; attempt < 2; attempt++) {
        const result = await f.run();
        assert.equal(result.status, "TransientFailure"); assert.equal(result.reason, "TransferUnavailable");
      }
      assert.equal(f.state.writes, 0);
      assert.deepEqual(clone([...f.privateItems]), originalPrivate);
      assert.deepEqual(clone([...f.protectedItems]), originalProtected);
    } else {
      await ready(f);
      assert.deepEqual(f.value("fs8-player-" + f.context.playerId), binding);
      assert.equal(f.value("fs8-account-" + binding.accountId).status, "Active");
      assert.equal(f.value("fs8-account-" + binding.accountId).leaderboardOwnerId, binding.generatedOwner);
    }
  }
});
