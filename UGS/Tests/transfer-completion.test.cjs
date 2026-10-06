const assert = require("node:assert/strict");
const fs = require("node:fs");
const vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createAccountService } = require("../CloudCode/account-service");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferLifecycleService } = require("../CloudCode/transfer-lifecycle-service");
const { createTransferCompletionService } = require("../CloudCode/transfer-completion-service");
const secret = Buffer.alloc(32, 7).toString("base64"); // Fake local key only.
const tests = []; const test = (name, run) => tests.push({ name, run });
async function ready() {
  const f = fixture(); f.now = 100000; f.b = { ...f.context, playerId: "B" };
  f.store = createAccountStore(f.context, f.save);
  const provision = createAccountProvisioningService(f.store, createLegacyAccountStore(f.context, f.save, f.leaderboard));
  await provision.ensure(f.context); await provision.ensure(f.b);
  f.cId = f.value("fs8-player-A").accountId; f.oldId = f.value("fs8-player-B").accountId;
  f.c = () => f.value(`fs8-account-${f.cId}`); f.old = () => f.value(`fs8-account-${f.oldId}`);
  f.service = (provider = async () => secret) => createTransferCompletionService(f.store, provider, () => f.now);
  f.run = (context = f.b, code = f.credential.code, value = f.credential.verificationValue) => f.service().complete(context, code, value);
  f.credential = await createTransferStartService(f.store, async () => secret, { clock: () => f.now,
    randomBytes: size => { if (size === 8) return Buffer.from([10,11,12,13,14,15,16,17]);
      const bytes = Buffer.alloc(size); bytes.writeUInt32BE(7); return bytes; },
    idGenerator: () => "00000000-0000-4000-8000-000000000001" }).start(f.context);
  f.state.writes = 0; f.history = [];
  f.state.beforeWrite = async op => f.history.push(clone(op.data));
  return f;
}
function statusEndpoint(f, store = f.store) {
  const module = { exports: {} };
  vm.runInNewContext(fs.readFileSync(require.resolve("../CloudCode/get-account-transfer-status.js"), "utf8"), {
    module, require: name => {
      if (name === "./account-store") return { createAccountStore: () => store };
      if (name === "./transfer-lifecycle-service") return {
        createTransferLifecycleService: value => createTransferLifecycleService(value, () => f.now) };
      if (name === "./transfer-completion-service") return {
        createTransferCompletionService: (value, provider) => createTransferCompletionService(value, provider, () => f.now) };
      throw Error("Unexpected dependency");
    }
  });
  return module.exports;
}
test("inactive source endpoint uses two reads, no recovery writes or credentials", async () => {
  const f = await ready(); await f.run(); const before = clone([...f.privateItems]);
  const reads = []; const store = { ...f.store, read: async (kind, id) => {
    reads.push(kind); return f.store.read(kind, id);
  }};
  const result = await statusEndpoint(f, store)({ context: f.context });
  assert.equal(result.status, "Inactive"); assert.equal(result.transferStatus, "Completed");
  assert.deepEqual(reads, ["player", "receipt"]);
  assert.deepEqual(clone([...f.privateItems]), before);
  assert.deepEqual(Object.keys(result).sort(), ["status", "transferId", "transferStatus"]);
});
test("inactive source without receipt falls back to authoritative account with three reads", async () => {
  const f = await ready(); await f.run();
  for (const key of f.privateItems.keys()) if (key.startsWith("fs8-receipt-")) f.privateItems.delete(key);
  const reads = []; const store = { ...f.store, read: async (kind, id) => {
    reads.push(kind); return f.store.read(kind, id);
  }};
  assert.equal((await statusEndpoint(f, store)({ context: f.context })).status, "Inactive");
  assert.deepEqual(reads, ["player", "receipt", "account"]);
});
test("fast status rejects invalid observed bindings and fails closed on read failure", async () => {
  const f = await ready(); await f.run(); const binding = await f.store.read("player", "A");
  for (const changed of [null, { ...binding, id: "B" }, { ...binding, kind: "account" },
    { ...binding, value: { ...binding.value, playerId: "B" } },
    { ...binding, value: { ...binding.value, environmentId: "wrong" } },
    { ...binding, value: { ...binding.value, status: "Active" } },
    { ...binding, value: { ...binding.value, transferOperation: {} } }])
    await assert.rejects(createTransferLifecycleService(f.store).inactiveStatus(f.context, changed));
  const store = { ...f.store, read: async (kind, id) => {
    if (kind === "receipt") throw Error("private-message"); return f.store.read(kind, id);
  }};
  const result = await statusEndpoint(f, store)({ context: f.context });
  assert.equal(result.status, "TransientFailure"); assert.equal(result.reason, "TransferUnavailable");
  assert.equal(JSON.stringify(result).includes("private-message"), false);
});
test("active target still follows committed completion recovery", async () => {
  const f = await ready(); await f.run();
  const result = await statusEndpoint(f)({ context: f.b });
  assert.ok(["Success", "AlreadyCompleted"].includes(result.status));
  assert.equal(result.publicPlayerNumber, f.c().publicPlayerNumber);
});

test("completion transfers only connection, preserving both accounts and mapping numbers", async () => {
  const f = await ready(); const cBefore = clone(f.c()); const bBefore = clone(f.old());
  const ledgers = clone([f.value(`fs8-ledger-${f.cId}`), f.value(`fs8-ledger-${f.oldId}`)]);
  const response = await f.run();
  assert.deepEqual(response, { status: "Success", publicPlayerNumber: cBefore.publicPlayerNumber });
  assert.equal(f.c().status, "Active"); assert.equal(f.c().currentPlayerId, "B");
  assert.equal(f.c().connectionRevision, cBefore.connectionRevision + 1);
  assert.equal(f.c().transfer.status, "Completed"); assert.equal(f.c().transfer.credentialActive, false);
  assert.equal(f.old().status, "Detached"); assert.equal(f.old().currentPlayerId, null);
  for (const name of ["publicPlayerNumber", "leaderboardOwnerId", "migration"])
    { assert.deepEqual(f.c()[name], cBefore[name]); assert.deepEqual(f.old()[name], bBefore[name]); }
  assert.deepEqual([f.value(`fs8-ledger-${f.cId}`), f.value(`fs8-ledger-${f.oldId}`)], ledgers);
  assert.equal(f.value("fs8-player-B").accountId, f.cId);
  assert.equal(f.value("fs8-player-A").status, "Inactive");
  assert.equal(f.value(`fs8-number-${bBefore.publicPlayerNumber}`).accountId, f.oldId);
  assert.equal(f.value(`fs8-owner-${bBefore.leaderboardOwnerId}`).accountId, f.oldId);
  assert.equal((await createAccountService(f.store).getPublicNumber(f.b)).publicPlayerNumber, cBefore.publicPlayerNumber);
  await assert.rejects(createAccountService(f.store).getPublicNumber(f.context));
});
test("existing B receipts, best, leaderboard rows and exact metadata are untouched", async () => {
  const f = await ready(); const ledger = f.value(`fs8-ledger-${f.oldId}`);
  ledger.entries.push({ id: "old-receipt", status: "Submitted", acceptedAt: 55 });
  ledger.best["fs-infinite-v2"] = { score: 123, acceptedAt: 55, id: "old-receipt" };
  f.scores.set(`fs-infinite-v2/${f.old().leaderboardOwnerId}`, { score: 123, metadata: { acceptedAt: 55, submissionId: "old-receipt" } });
  const before = clone(ledger); const scores = clone([...f.scores]);
  await f.run(); assert.deepEqual(f.value(`fs8-ledger-${f.oldId}`), before); assert.deepEqual([...f.scores], scores);
});
test("same B retry returns AlreadyCompleted, no new writes, throttle or revision changes", async () => {
  const f = await ready(); await f.run(); const before = clone(f.c()); const writes = f.state.writes;
  const repeated = await f.run();
  assert.deepEqual(repeated, { status: "AlreadyCompleted", publicPlayerNumber: before.publicPlayerNumber });
  assert.equal(f.state.writes, writes); assert.deepEqual(f.c(), before);
  f.now = before.transfer.expiresAtMilliseconds + 1;
  assert.equal((await f.run()).status, "AlreadyCompleted");
});
test("other B or wrong credential cannot recover or disclose completed number", async () => {
  const f = await ready(); await f.run(); const before = clone(f.c());
  for (const [context, value] of [[{ ...f.b, playerId: "B2" }, f.credential.verificationValue], [f.b, "000000999"]])
    assert.deepEqual(await f.run(context, f.credential.code, value), { status: "InvalidCredential" });
  assert.deepEqual(f.c(), before);
});
test("completed A and B status are safe and A cannot cancel or restart C", async () => {
  const f = await ready(); await f.run();
  const lifecycle = createTransferLifecycleService(f.store, () => f.now);
  assert.deepEqual(await lifecycle.status(f.context), { status: "Inactive", transferStatus: "Completed", transferId: f.c().transfer.transferId });
  assert.equal((await lifecycle.status(f.b)).transferStatus, "Completed");
  await assert.rejects(lifecycle.cancel(f.context, f.c().transfer.transferId));
  await assert.rejects(createTransferStartService(f.store, async () => secret).start(f.context));
});
test("invalid authentication, environment, unknown code and Secret failure never write", async () => {
  const f = await ready();
  await assert.rejects(f.run({ ...f.b, environmentId: "other" }));
  await assert.rejects(f.run({ ...f.b, serviceToken: "" }));
  assert.equal((await f.run(f.b, "ZZZZ-ZZZZ")).status, "InvalidCredential");
  await assert.rejects(f.service(async () => "weak").complete(f.b, f.credential.code, f.credential.verificationValue));
  assert.equal(f.state.writes, 0);
});
test("A cannot become its own transfer target", async () => {
  const f = await ready(); assert.equal((await f.run(f.context)).status, "InvalidCredential"); assert.equal(f.state.writes, 0);
});
test("known-code wrong/malformed value uses the shared five-second attempt gate", async () => {
  const f = await ready(); assert.equal((await f.run(f.b, f.credential.code, 0)).status, "InvalidCredential");
  assert.equal(f.c().transfer.failureCount, 1); assert.equal((await f.run()).status, "TooManyRequests");
  f.now += 5000; assert.equal((await f.run()).status, "Success");
});
test("missing B account or ongoing B submission cannot abandon the old account", async () => {
  for (const kind of ["binding", "reservation", "ledger", "pending", "transfer"]) {
    const f = await ready();
    if (kind === "binding") f.privateItems.delete("fs8-player-B/fs_account_v1");
    if (kind === "reservation") f.old().onlineOperation = { id: "running" };
    if (kind === "ledger") f.value(`fs8-ledger-${f.oldId}`).active = "running";
    if (kind === "pending") f.value(`fs8-ledger-${f.oldId}`).entries.push({ status: "Pending" });
    if (kind === "transfer") f.old().status = "TransferPending";
    const before = clone(f.old()); await assert.rejects(f.run());
    assert.deepEqual(f.old(), before); assert.equal(f.c().currentPlayerId, "A");
    if (kind !== "binding") assert.equal(f.value("fs8-player-B").transferOperation, undefined);
  }
});
test("expired credential completes no connection and restores A via terminal CAS", async () => {
  const f = await ready(); f.now = f.c().transfer.expiresAtMilliseconds;
  assert.equal((await f.run()).status, "InvalidCredential"); assert.equal(f.c().transfer.status, "Expired");
  assert.equal(f.c().currentPlayerId, "A"); assert.equal(f.old().currentPlayerId, "B");
});
test("reservation, detach and partial binding cannot grant B online authority", async () => {
  const f = await ready(); let gated = 0;
  f.state.beforeWrite = async op => {
    if (f.value("fs8-player-B").transferOperation || f.c().status === "TransferCompleting") {
      await assert.rejects(createAccountService(f.store).resolve(f.b)); gated++;
    }
  };
  await f.run(); assert.ok(gated >= 4);
});
test("every completion write failure before/after commit resumes without wrong authority", async () => {
  const baseline = await ready(); await baseline.run(); const count = baseline.state.writes;
  for (const mode of ["failAt", "loseAt"]) for (let at = 1; at <= count; at++) {
    const f = await ready(); const cNumber = f.c().publicPlayerNumber; const bNumber = f.old().publicPlayerNumber;
    f.state[mode] = at; await assert.rejects(f.run()); f.state[mode] = 0;
    if (f.c().transfer.status === "Completed") {
      await assert.rejects(createAccountService(f.store).resolve(f.context));
    }
    f.now += 5000;
    const result = await f.run(); assert.ok(["Success", "AlreadyCompleted"].includes(result.status), `${mode}:${at}`);
    assert.equal(result.publicPlayerNumber, cNumber); assert.equal(f.old().publicPlayerNumber, bNumber);
    assert.equal(f.c().currentPlayerId, "B"); assert.equal(f.old().currentPlayerId, null);
    assert.equal(f.value("fs8-player-B").accountId, f.cId);
    const saved = JSON.stringify([...f.privateItems.values(), ...f.history]);
    for (const raw of [f.credential.code, "ABCDEFGH", f.credential.verificationValue, secret]) assert.equal(saved.includes(raw), false);
  }
});
test("cancel winning terminal CAS rolls back only B reservation and preserves B records", async () => {
  const f = await ready(); let raced = false;
  f.state.beforeWrite = async op => {
    if (!raced && op.id === `fs8-account-${f.cId}` && op.data[0].value.status === "TransferCompleting") {
      raced = true; await createTransferLifecycleService(f.store, () => f.now).cancel(f.context, f.c().transfer.transferId);
    }
  };
  await assert.rejects(f.run()); assert.equal(f.c().transfer.status, "Cancelled"); assert.equal(f.old().status, "Active");
  assert.equal(f.value("fs8-player-B").transferOperation, undefined);
});
test("B submission winning the reservation race can finish before completion resumes", async () => {
  const f = await ready(); let raced = false;
  const id = "00000000-0000-4000-8000-000000000009";
  f.state.beforeWrite = async op => {
    if (!raced && op.id === "fs8-player-B" && op.data[0].value.transferOperation) {
      raced = true; await createAccountService(f.store, () => f.now).reserveSubmission(f.b, id);
    }
  };
  await assert.rejects(f.run());
  await assert.rejects(createAccountService(f.store).getPublicNumber(f.b));
  assert.equal((await createAccountService(f.store).reserveSubmission(f.b, id)).disposition, "Resume");
  await assert.rejects(createAccountService(f.store).reserveSubmission(f.b, "00000000-0000-4000-8000-000000000008"));
  f.value(`fs8-ledger-${f.oldId}`).entries.push({ id, status: "Submitted" });
  assert.equal(await createAccountService(f.store).releaseSubmission(f.b, id), "Released");
  f.now += 5000; assert.equal((await f.run()).status, "Success");
});
test("receipt fixes prior completion retry even after B starts a new transfer", async () => {
  const f = await ready(); await f.run(); const writes = f.state.writes;
  await createTransferStartService(f.store, async () => secret, { clock: () => f.now,
    idGenerator: () => "00000000-0000-4000-8000-000000000002" }).start(f.b);
  const before = clone(f.c()); const nowWrites = f.state.writes;
  assert.equal((await f.run()).status, "AlreadyCompleted");
  assert.deepEqual(f.c(), before); assert.equal(f.state.writes, nowWrites); assert.ok(nowWrites > writes);
});
test("reservation response loss followed by A cancel can release B using the old lookup", async () => {
  const f = await ready();
  // Verification is write 1; B binding reservation is write 2.
  f.state.loseAt = 2; await assert.rejects(f.run()); f.state.loseAt = 0;
  await createTransferLifecycleService(f.store, () => f.now).cancel(f.context, f.c().transfer.transferId);
  assert.equal((await f.run()).status, "InvalidCredential");
  assert.equal(f.value("fs8-player-B").transferOperation, undefined); assert.equal(f.old().status, "Active");
});
test("partial completion receipt cannot authorize another authenticated B", async () => {
  const f = await ready(); await f.run();
  const receipt = f.value(`fs8-receipt-${f.c().transfer.transferId}`);
  assert.equal(receipt.completedPlayerId, "B"); assert.equal(receipt.credentialHmac, f.c().transfer.credentialHmac);
  const result = await f.run({ ...f.b, playerId: "other" });
  assert.deepEqual(result, { status: "InvalidCredential" });
});
test("completion endpoint exposes no service errors, credentials or Account identifiers", async () => {
  const module = { exports: {} }; const logs = [];
  vm.runInNewContext(fs.readFileSync(require.resolve("../CloudCode/complete-account-transfer.js"), "utf8"), {
    module, require: name => name === "./account-store" ? { createAccountStore: () => { throw Error("RAW-TOKEN-CREDENTIAL"); } } :
      name === "./transfer-cryptography" ? { createTransferSecretProvider: () => null } : { createTransferCompletionService: () => null }
  });
  const result = await module.exports({ context: {}, logger: { warning: m => logs.push(m) } });
  assert.deepEqual(JSON.parse(JSON.stringify(result)), { status: "TransientFailure", reason: "TransferUnavailable" });
  assert.equal(JSON.stringify(logs).includes("RAW-TOKEN-CREDENTIAL"), false); assert.equal(module.exports.bundling, true);
});
module.exports = { ready };
if (require.main === module) (async () => {
  let passed = 0;
  for (const t of tests) { try { await t.run(); passed++; console.log(`PASS ${t.name}`); }
    catch (error) { console.error(`FAIL ${t.name}`, error); process.exitCode = 1; } }
  console.log(`${passed}/${tests.length} passed`);
})();
