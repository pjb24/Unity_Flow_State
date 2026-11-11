"use strict";
// Generic bootstrap conflict recovery: synthetic identities/storage, no network.
const test = require("node:test"), assert = require("node:assert/strict");
const fs = require("node:fs"), vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore, ledgerKey } = require("../CloudCode/legacy-account-store");
const { createTransferCompletionService } = require("../CloudCode/transfer-completion-service");
const { createTransferLifecycleService } = require("../CloudCode/transfer-lifecycle-service");
const { createAccountService } = require("../CloudCode/account-service");
const uuid = n => `20000000-0000-4000-8000-${String(n).padStart(12, "0")}`;
async function scenario(seed = 1, legacy = false, allocated = true) {
  const f = fixture({ projectId: uuid(100000 + seed), environmentId: uuid(200000 + seed) });
  f.context.playerId = "bootstrap-player-" + seed;
  f.store = createAccountStore(f.context, f.save); f.generated = seed * 100;
  if (legacy) for (const [board, score] of [["fs-stage-stage-001-r1", 1234], ["fs-infinite-v2", 5678]])
    f.scores.set(board + "/" + f.context.playerId, { playerId: f.context.playerId, score,
      metadata: { acceptedAt: seed + 100, submissionId: uuid(300000 + seed), extra: "synthetic-metadata" } });
  if (legacy) f.seedProtected(f.context.playerId, ledgerKey, { version: 1, active: "", best: {},
    entries: [{ id: uuid(400000 + seed), status: "Rejected", reason: "InvalidScore", acceptedAt: seed + 200 }] });
  const module = { exports: {} };
  vm.runInNewContext(fs.readFileSync(require.resolve("../CloudCode/get-account-transfer-status.js"), "utf8"), {
    module, require: name => {
      if (name === "./account-store") return { createAccountStore: () => f.store };
      if (name === "./leaderboard-owner-policy") return require("../CloudCode/leaderboard-owner-policy");
      if (name === "./transfer-completion-service") return { createTransferCompletionService };
      if (name === "./transfer-lifecycle-service") return { createTransferLifecycleService };
      if (name === "./legacy-account-store") return {
        createLegacyAccountStore: context => createLegacyAccountStore(context, f.save, f.leaderboard) };
      if (name === "./account-provisioning-service") return {
        createAccountProvisioningService: (store, source) => createAccountProvisioningService(store, source, () => uuid(++f.generated)) };
      throw Error("Unexpected dependency");
    }
  });
  f.run = () => module.exports({ context: f.context, logger: { warning() {} } });
  if (!allocated) f.state.beforeWrite = async ({ id }) => {
    if (id === "fs8-allocator-public") {
      f.state.beforeWrite = null; throw Error("synthetic interruption before number allocation");
    }
  };
  assert.equal((await f.run()).status, allocated ? "Active" : "TransientFailure");
  f.bindingId = "fs8-player-" + f.context.playerId;
  const binding = clone(f.value(f.bindingId));
  f.accountId = binding.accountId; f.accountContainer = "fs8-account-" + f.accountId;
  f.originalAccount = clone(f.value(f.accountContainer));
  f.originalLedger = clone(f.value("fs8-ledger-" + f.accountId));
  f.originalProtected = clone([...f.protectedItems]); f.originalScores = clone([...f.scores]);
  f.originalAllocator = allocated ? clone(f.value("fs8-allocator-public")) : null;
  f.orphanId = uuid(seed * 100 + 90);
  f.badBinding = { ...binding, accountId: f.orphanId, generatedOwner: uuid(seed * 100 + 91) };
  // Inject a durable bootstrap interruption, never edit real QA storage.
  f.seedPrivate(f.accountContainer, "fs_account_v1", { ...f.originalAccount, status: "Preparing" });
  f.seedPrivate(f.bindingId, "fs_account_v1", f.badBinding);
  f.state.writes = 0;
  return f;
}
async function finish(f) {
  for (let i = 0; i < 6; i++) if ((await f.run()).status === "Active") return;
  assert.fail("Synthetic bootstrap recovery did not finish");
}
function assertPreserved(f) {
  const binding = f.value(f.bindingId);
  assert.equal(binding.accountId, f.accountId); assert.equal(binding.playerId, f.context.playerId);
  assert.equal(binding.connectionRevision, 1);
  assert.deepEqual(f.value(f.accountContainer), f.originalAccount);
  assert.deepEqual(f.value("fs8-ledger-" + f.accountId), f.originalLedger);
  assert.deepEqual(clone([...f.protectedItems]), f.originalProtected);
  assert.deepEqual(clone([...f.scores]), f.originalScores);
  assert.deepEqual(f.value("fs8-allocator-public"), f.originalAllocator);
  assert.equal(f.privateItems.has("fs8-account-" + f.orphanId + "/fs_account_v1"), false);
}
test("same-player Preparing identity recovery preserves issued number and all data across different scopes and legacy states", async () => {
  for (let seed = 1; seed <= 6; seed++) for (const legacy of [false, true]) {
    const f = await scenario(seed, legacy), generated = f.generated;
    await finish(f); assertPreserved(f); assert.equal(f.generated, generated);
    const number = await createAccountService(f.store).getPublicNumber(f.context);
    assert.equal(number.publicPlayerNumber, f.originalAccount.publicPlayerNumber);
    const before = clone([...f.privateItems]), writes = f.state.writes;
    await finish(f); assert.deepEqual(clone([...f.privateItems]), before); assert.equal(f.state.writes, writes);
  }
});
test("both recovery write boundaries resume before/after commit without duplicate identity or number allocation", async () => {
  const baseline = await scenario(20); await finish(baseline);
  assert.equal(baseline.state.writes, 2, "Only binding CAS and initial activation may write");
  for (const mode of ["failAt", "loseAt"]) for (let step = 1; step <= 2; step++) {
    const f = await scenario(20 + step); f.state[mode] = step;
    await f.run(); f.state[mode] = 0; await finish(f); assertPreserved(f);
  }
});
test("concurrent recovery converges to the same original Preparing Account", async () => {
  const f = await scenario(30);
  await Promise.all([f.run(), f.run()]); await finish(f); assertPreserved(f);
});
test("identity, ownership, source, mapping and target conflicts are rejected without recovery writes", async () => {
  const mutateAccount = change => f => {
    const a = clone(f.value(f.accountContainer)); change(a, f); f.seedPrivate(f.accountContainer, "fs_account_v1", a);
  };
  const mutateBinding = change => f => {
    const b = clone(f.value(f.bindingId)); change(b, f); f.seedPrivate(f.bindingId, "fs_account_v1", b);
  };
  const mutations = [
    mutateAccount(a => a.status = "Active"), mutateAccount(a => a.status = "Detached"),
    mutateAccount(a => a.status = "TransferPending"), mutateAccount(a => a.initialPlayerId = "other"),
    mutateAccount(a => a.currentPlayerId = "other"), mutateAccount(a => a.connectionRevision = 2),
    mutateAccount(a => a.transfer = { status: "Pending" }), mutateAccount(a => a.onlineOperation = { id: "other" }),
    mutateAccount(a => a.joiningTransfer = {}), mutateAccount(a => a.revokedJoiningTransfer = {}),
    mutateAccount(a => a.detachedByTransferId = "other"),
    mutateAccount(a => a.environmentId = "other"), mutateAccount(a => a.publicPlayerNumber = "invalid"),
    mutateAccount(a => a.migration.ledger.entries.push({ status: "Submitted" })),
    mutateAccount(a => a.migration.scores.unexpected = {}), mutateAccount(a => a.leaderboardOwnerId = "other"),
    mutateBinding(b => b.status = "Inactive"), mutateBinding(b => b.status = "Active"),
    mutateBinding(b => b.connectionRevision = 2), mutateBinding(b => b.transferOperation = { targetAccountId: uuid(999) }),
    mutateBinding(b => b.inactiveByTransferId = "other"),
    f => f.privateItems.delete(f.accountContainer + "/fs_account_v1"),
    f => f.privateItems.delete("fs8-ledger-" + f.accountId + "/fs_account_v1"),
    f => f.seedPrivate("fs8-number-" + f.originalAccount.publicPlayerNumber, "fs_account_v1",
      { schemaVersion: 1, ...f.store.scope, accountId: "other" }),
    f => f.seedPrivate("fs8-owner-" + f.originalAccount.leaderboardOwnerId, "fs_account_v1",
      { schemaVersion: 1, ...f.store.scope, accountId: "other" }),
    f => f.seedPrivate("fs8-owner-" + f.badBinding.generatedOwner, "fs_account_v1",
      { schemaVersion: 1, ...f.store.scope, accountId: f.orphanId }),
    f => f.seedPrivate("fs8-ledger-" + f.accountId, "fs_account_v1", { ...f.originalLedger, active: "other" }),
    f => f.seedPrivate("fs8-account-" + f.orphanId, "fs_account_v1",
      { ...f.originalAccount, accountId: f.orphanId, status: "Preparing", leaderboardOwnerId: f.badBinding.generatedOwner }),
    f => {
      const l = clone(f.protectedItems.get(f.context.playerId + "/" + ledgerKey).value);
      l.migration.environmentId = "other"; f.seedProtected(f.context.playerId, ledgerKey, l);
    },
    f => {
      const l = clone(f.protectedItems.get(f.context.playerId + "/" + ledgerKey).value);
      l.best = []; f.seedProtected(f.context.playerId, ledgerKey, l);
      const a = clone(f.value(f.accountContainer)); a.migration.ledger = clone(l);
      f.seedPrivate(f.accountContainer, "fs_account_v1", a);
    },
    f => {
      const l = clone(f.protectedItems.get(f.context.playerId + "/" + ledgerKey).value);
      l.entries.push({ status: "Unknown" }); f.seedProtected(f.context.playerId, ledgerKey, l);
      const a = clone(f.value(f.accountContainer)); a.migration.ledger = clone(l);
      f.seedPrivate(f.accountContainer, "fs_account_v1", a);
    },
    f => f.scores.set("fs-stage-stage-001-r1/" + f.context.playerId,
      { playerId: f.context.playerId, score: 1, metadata: { acceptedAt: 1, submissionId: "unexpected-run" } })
  ];
  for (let i = 0; i < mutations.length; i++) {
    const f = await scenario(40 + i); mutations[i](f);
    const before = clone([...f.privateItems]), protectedBefore = clone([...f.protectedItems]), rows = clone([...f.scores]);
    const result = await f.run();
    assert.equal(result.status, "TransientFailure", "conflict " + i);
    assert.equal(f.state.writes, 0, "conflict " + i);
    assert.deepEqual(clone([...f.privateItems]), before);
    assert.deepEqual(clone([...f.protectedItems]), protectedBefore); assert.deepEqual(clone([...f.scores]), rows);
  }
});
test("read failures never authorize a repair", async () => {
  for (const kind of ["account", "ledger", "owner", "number"]) for (const status of [403, 404, 503]) {
    const f = await scenario(80), read = f.store.read;
    f.store.read = async (requested, id) => {
      if (requested === kind && !(kind === "account" && id === f.orphanId))
        throw Object.assign(Error("synthetic read failure"), { response: { status } });
      return read(requested, id);
    };
    assert.equal((await f.run()).status, "TransientFailure"); assert.equal(f.state.writes, 0);
    assert.deepEqual(f.value(f.bindingId), f.badBinding);
  }
});
test("binding CAS rejects a concurrent Inactive replacement", async () => {
  const f = await scenario(90);
  f.state.beforeWrite = async ({ id, data }) => {
    if (id === f.bindingId && data.some(x => x.value && x.value.accountId === f.accountId)) {
      f.state.beforeWrite = null;
      f.seedPrivate(f.bindingId, "fs_account_v1", { ...f.badBinding, status: "Inactive" });
    }
  };
  assert.equal((await f.run()).status, "TransientFailure");
  assert.equal(f.value(f.bindingId).status, "Inactive"); assert.equal(f.value(f.accountContainer).status, "Preparing");
  assert.deepEqual(clone([...f.protectedItems]), f.originalProtected);
});
test("source mutation during validation is rejected before the binding CAS", async () => {
  const f = await scenario(91); let reads = 0;
  f.state.afterRead = async ({ items, id, keys }) => {
    if (items === f.protectedItems && id === f.context.playerId && keys.includes(ledgerKey) && ++reads === 3) {
      const l = clone(f.protectedItems.get(id + "/" + ledgerKey).value);
      l.migration.accountId = uuid(99999); f.seedProtected(id, ledgerKey, l);
    }
  };
  assert.equal((await f.run()).status, "TransientFailure"); assert.equal(f.state.writes, 0);
  assert.deepEqual(f.value(f.bindingId), f.badBinding); assert.equal(f.value(f.accountContainer).status, "Preparing");
});
test("initial recovery also handles interruption before number allocation or destination ledger creation", async () => {
  for (const missingLedger of [false, true]) {
    const f = await scenario(95, false, false);
    assert.equal(f.originalAccount.publicPlayerNumber, "");
    if (missingLedger) f.privateItems.delete("fs8-ledger-" + f.accountId + "/fs_account_v1");
    await finish(f);
    assert.equal(f.value(f.bindingId).accountId, f.accountId);
    const a = f.value(f.accountContainer);
    assert.equal(a.status, "Active"); assert.equal(a.leaderboardOwnerId, f.originalAccount.leaderboardOwnerId);
    assert.match(a.publicPlayerNumber, /^\d{10}$/);
    assert.deepEqual(clone([...f.protectedItems]), f.originalProtected);
    assert.deepEqual(f.value("fs8-ledger-" + f.accountId), f.originalLedger);
    const before = clone([...f.privateItems]); await finish(f); assert.deepEqual(clone([...f.privateItems]), before);
  }
});
test("service JSON key order is not used to decide identity or snapshot equality", async () => {
  const f = await scenario(96, true);
  const reorder = x => Array.isArray(x) ? x.map(reorder) : x && typeof x === "object" ?
    Object.fromEntries(Object.keys(x).reverse().map(k => [k, reorder(x[k])])) : x;
  for (const method of ["getPrivateCustomItems", "getProtectedItems"]) {
    const original = f.save[method]; f.save[method] = async (...args) => reorder(await original(...args));
  }
  await finish(f); assertPreserved(f);
});
test("actual embedded module shim executes the repair and fails closed on invalid snapshots", async () => {
  const { runtime } = require("./phase2-module-runtime.test.cjs");
  const f = await scenario(97, true), r = runtime(f);
  assert.equal((await r.call("get-account-transfer-status")).status, "Active"); assertPreserved(f);
  const g = await scenario(98), account = clone(g.value(g.accountContainer));
  account.migration.ledger.best.unexpected = {};
  g.seedPrivate(g.accountContainer, "fs_account_v1", account);
  assert.equal((await runtime(g).call("get-account-transfer-status")).status, "TransientFailure");
  assert.equal(g.state.writes, 0); assert.deepEqual(g.value(g.bindingId), g.badBinding);
});
