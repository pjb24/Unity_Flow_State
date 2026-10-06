const assert = require("node:assert/strict");
const fs = require("node:fs");
const vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createAccountService } = require("../CloudCode/account-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferVerificationService } = require("../CloudCode/transfer-verification-service");
const { createTransferLifecycleService } = require("../CloudCode/transfer-lifecycle-service");
const policy = require("../CloudCode/transfer-credential-policy");
const secret = Buffer.alloc(32, 7).toString("base64"); // Deliberately fake local test key.
const tests = []; const test = (name, run) => tests.push({ name, run });
async function ready(pending = true) {
  const f = fixture(); f.store = createAccountStore(f.context, f.save); f.now = 100000;
  await createAccountProvisioningService(f.store,
    createLegacyAccountStore(f.context, f.save, f.leaderboard)).ensure(f.context);
  f.accountId = f.value("fs8-player-A").accountId;
  f.account = () => f.value(`fs8-account-${f.accountId}`);
  f.start = (id = "00000000-0000-4000-8000-000000000001") => createTransferStartService(f.store, async () => secret,
    { clock: () => f.now, idGenerator: () => id });
  f.lifecycle = () => createTransferLifecycleService(f.store, () => f.now);
  f.verify = () => createTransferVerificationService(f.store, async () => secret, () => f.now);
  if (pending) { f.credential = await f.start().start(f.context); f.transferId = f.account().transfer.transferId; }
  f.state.writes = 0; return f;
}
function assertPreserved(before, after) {
  for (const name of ["accountId", "currentPlayerId", "connectionRevision", "publicPlayerNumber", "leaderboardOwnerId", "onlineOperation"])
    assert.deepEqual(after[name], before[name]);
}
test("cancel atomically unlocks A, removes verification material and preserves records", async () => {
  const f = await ready(); const before = clone(f.account());
  const ledger = clone(f.value(`fs8-ledger-${f.accountId}`)); const bindings = clone(f.value("fs8-player-A"));
  const result = await f.lifecycle().cancel(f.context, f.transferId);
  assert.deepEqual(result, { status: "Active", transferStatus: "Cancelled", transferId: f.transferId });
  assertPreserved(before, f.account()); assert.equal(f.account().transfer.credentialActive, false);
  assert.equal(f.account().transfer.codeDigest, undefined); assert.equal(f.account().transfer.credentialHmac, undefined);
  assert.equal(f.account().transfer.terminalAtMilliseconds, f.now); assert.equal(f.state.writes, 1);
  assert.deepEqual(f.value(`fs8-ledger-${f.accountId}`), ledger); assert.deepEqual(f.value("fs8-player-A"), bindings);
  assert.equal((await f.verify().verify({ ...f.context, playerId: "B" }, f.credential.code, f.credential.verificationValue)).status,
    "InvalidCredential");
  assert.equal((await createAccountService(f.store).getPublicNumber(f.context)).status, "Success");
});
test("status after restart never returns credentials or internal Account fields", async () => {
  const f = await ready(); const before = clone(f.account());
  const result = await f.lifecycle().status(f.context);
  assert.deepEqual(Object.keys(result).sort(), ["credentialReissueRequired", "expiresAtMilliseconds", "status", "transferId", "transferStatus"]);
  assert.equal(result.credentialReissueRequired, true); assert.deepEqual(f.account(), before); assert.equal(f.state.writes, 0);
  for (const forbidden of ["accountId", "sourcePlayerId", "codeDigest", "credentialHmac", "code", "verificationValue", "publicPlayerNumber"])
    assert.equal(Object.hasOwn(result, forbidden), false);
});
test("Active without transfer can be queried without Secret or writes", async () => {
  const f = await ready(false);
  assert.deepEqual(await f.lifecycle().status(f.context), { status: "Active", transferStatus: "None" });
  assert.equal(f.state.writes, 0);
});
test("90 day just-before/exact/after expiry is decided by server clock", async () => {
  for (const offset of [-1, 0, 1]) {
    const f = await ready(); const before = clone(f.account()); f.now = before.transfer.expiresAtMilliseconds + offset;
    const result = await f.lifecycle().status(f.context);
    assert.equal(result.transferStatus, offset < 0 ? "TransferPending" : "Expired");
    assert.equal(f.state.writes, offset < 0 ? 0 : 1); assertPreserved(before, f.account());
    if (offset >= 0) {
      assert.equal(f.account().transfer.credentialActive, false);
      assert.equal(f.account().transfer.codeDigest, undefined);
      assert.equal((await f.verify().verify({ ...f.context, playerId: "B" }, f.credential.code, f.credential.verificationValue)).status,
        "InvalidCredential");
    }
  }
});
test("cancel and expire are idempotent after response loss and restart", async () => {
  for (const kind of ["cancel", "expire"]) {
    const f = await ready(); if (kind === "expire") f.now = f.account().transfer.expiresAtMilliseconds;
    const first = await f.lifecycle()[kind](f.context, f.transferId); const before = clone(f.account());
    const second = await f.lifecycle()[kind](f.context, f.transferId);
    assert.deepEqual(second, first); assert.deepEqual(await f.lifecycle().status(f.context), first);
    assert.deepEqual(f.account(), before); assert.equal(f.state.writes, 1);
  }
});
test("terminal first winner is never replaced by the other disposition", async () => {
  for (const winner of ["cancel", "expire"]) {
    const f = await ready(); f.now = f.account().transfer.expiresAtMilliseconds;
    const first = await f.lifecycle()[winner](f.context, f.transferId);
    assert.deepEqual(await f.lifecycle()[winner === "cancel" ? "expire" : "cancel"](f.context, f.transferId), first);
    assert.equal(f.state.writes, 1);
  }
});
test("simultaneous cancel/expiry CAS has one winner and recoverable loser", async () => {
  const f = await ready(); f.now = f.account().transfer.expiresAtMilliseconds;
  const results = await Promise.allSettled([f.lifecycle().cancel(f.context, f.transferId), f.lifecycle().expire(f.context, f.transferId)]);
  assert.equal(results.filter(r => r.status === "fulfilled").length, 1);
  assert.ok(["Cancelled", "Expired"].includes(f.account().transfer.status));
  const terminal = f.account().transfer.status;
  assert.equal((await f.lifecycle().status(f.context)).transferStatus, terminal);
});
test("simultaneous expiry status reads have one terminal CAS winner", async () => {
  const f = await ready(); f.now = f.account().transfer.expiresAtMilliseconds;
  const results = await Promise.allSettled([f.lifecycle().status(f.context), f.lifecycle().status(f.context)]);
  assert.equal(results.filter(r => r.status === "fulfilled").length, 1);
  assert.equal((await f.lifecycle().status(f.context)).transferStatus, "Expired");
});
test("every terminal write interruption before/after commit resumes safely", async () => {
  for (const kind of ["cancel", "expire", "status"]) for (const mode of ["failAt", "loseAt"]) {
    const f = await ready(); const before = clone(f.account());
    if (kind !== "cancel") f.now = f.account().transfer.expiresAtMilliseconds;
    f.state[mode] = 1;
    await assert.rejects(f.lifecycle()[kind](f.context, f.transferId)); f.state[mode] = 0;
    assertPreserved(before, f.account());
    if (mode === "failAt") assert.equal(f.account().status, "TransferPending");
    else { assert.equal(f.account().status, "Active"); assert.equal(f.account().transfer.credentialActive, false); }
    const result = await f.lifecycle()[kind](f.context, f.transferId);
    assert.equal(result.transferStatus, kind === "cancel" ? "Cancelled" : "Expired");
    assertPreserved(before, f.account());
  }
});
test("unknown, wrong generation and missing cancellation ID cannot cancel anything", async () => {
  const f = await ready(); const before = clone(f.account());
  for (const id of [undefined, "bad", "00000000-0000-4000-8000-000000000009"])
    await assert.rejects(f.lifecycle().cancel(f.context, id));
  assert.equal(f.state.writes, 0); assert.deepEqual(f.account(), before);
});
test("delayed cancel for terminal predecessor cannot cancel a new transfer", async () => {
  const f = await ready(); await f.lifecycle().cancel(f.context, f.transferId);
  await f.start("00000000-0000-4000-8000-000000000002").start(f.context); const before = clone(f.account());
  await assert.rejects(f.lifecycle().cancel(f.context, f.transferId));
  assert.deepEqual(f.account(), before); assert.equal(f.account().status, "TransferPending");
});
test("fresh transfer after expiry preserves number, connection and independent new lifetime", async () => {
  const f = await ready(); const before = clone(f.account()); f.now = before.transfer.expiresAtMilliseconds;
  await f.lifecycle().status(f.context);
  await f.start("00000000-0000-4000-8000-000000000002").start(f.context);
  assertPreserved(before, f.account()); assert.equal(f.account().transfer.issuedAtMilliseconds, f.now);
  assert.equal(f.account().transfer.expiresAtMilliseconds, f.now + policy.transferLifetimeMilliseconds);
});
test("unactivated issuance failure can be cancelled without Secret or lookup writes", async () => {
  const f = await ready(); f.account().transfer.credentialActive = false;
  assert.equal((await f.lifecycle().status(f.context)).credentialReissueRequired, true);
  assert.equal(f.account().transfer.credentialActive, false);
  await f.lifecycle().cancel(f.context, f.transferId); assert.equal(f.state.writes, 1);
});
test("wrong environment, inactive A and stale binding never unlock an Account", async () => {
  for (const kind of ["cancel", "status", "expire"]) {
    for (const invalid of ["context", "player", "revision", "scope"]) {
      const f = await ready(); let context = f.context;
      if (invalid === "context") context = { ...context, environmentId: "wrong" };
      if (invalid === "player") context = { ...context, playerId: "B" };
      if (invalid === "revision") f.value("fs8-player-A").connectionRevision++;
      if (invalid === "scope") f.account().environmentId = "wrong";
      await assert.rejects(f.lifecycle()[kind](context, f.transferId)); assert.equal(f.state.writes, 0);
    }
  }
});
test("missing/invalid server time, rollback and corrupt transfer fail closed", async () => {
  for (const change of ["undefined", "negative", "rollback", "expiry", "digest", "source", "count"]) {
    const f = await ready();
    if (change === "undefined") f.now = undefined;
    if (change === "negative") f.now = -1;
    if (change === "rollback") f.now--;
    if (change === "expiry") f.account().transfer.expiresAtMilliseconds++;
    if (change === "digest") f.account().transfer.codeDigest = "bad";
    if (change === "source") f.account().transfer.sourcePlayerId = "other";
    if (change === "count") f.account().transfer.failureCount = -1;
    await assert.rejects(f.lifecycle().status(f.context)); assert.equal(f.state.writes, 0);
  }
});
test("unknown and contradictory terminal states cannot be interpreted as restored", async () => {
  for (const change of ["unknown", "activeCredential", "hash", "missingTimestamp"]) {
    const f = await ready(); await f.lifecycle().cancel(f.context, f.transferId); f.state.writes = 0;
    if (change === "unknown") f.account().transfer.status = "Unknown";
    if (change === "activeCredential") f.account().transfer.credentialActive = true;
    if (change === "hash") f.account().transfer.credentialHmac = "bad";
    if (change === "missingTimestamp") delete f.account().transfer.terminalAtMilliseconds;
    await assert.rejects(f.lifecycle().status(f.context)); assert.equal(f.state.writes, 0);
  }
});
test("cancel winning CAS fences stale reissue and verification", async () => {
  for (const kind of ["reissue", "verify"]) {
    const f = await ready(); let raced = false;
    f.state.beforeWrite = async op => {
      if (!raced && op.id.startsWith("fs8-account-")) {
        raced = true; await f.lifecycle().cancel(f.context, f.transferId);
      }
    };
    if (kind === "reissue") await assert.rejects(f.start().reissue(f.context));
    else await assert.rejects(f.verify().verify({ ...f.context, playerId: "B" }, f.credential.code, f.credential.verificationValue));
    assert.equal(f.account().transfer.status, "Cancelled"); assert.equal(f.account().status, "Active");
  }
});
test("reissue winning CAS fences stale cancel then cancellation can resume", async () => {
  const f = await ready(); let raced = false;
  f.state.beforeWrite = async op => {
    if (!raced && op.id.startsWith("fs8-account-")) { raced = true; await f.start().reissue(f.context); }
  };
  await assert.rejects(f.lifecycle().cancel(f.context, f.transferId));
  assert.equal(f.account().transfer.credentialRevision, 2);
  await f.lifecycle().cancel(f.context, f.transferId); assert.equal(f.account().transfer.status, "Cancelled");
});
test("cancellation during late lookup creation cannot activate or return credentials", async () => {
  const f = await ready(); let raced = false;
  f.state.beforeWrite = async op => {
    if (!raced && op.id.startsWith("fs8-t-")) { raced = true; await f.lifecycle().cancel(f.context, f.transferId); }
  };
  await assert.rejects(f.start().reissue(f.context));
  assert.equal(raced, true); assert.equal(f.account().transfer.status, "Cancelled");
  assert.equal(f.account().transfer.credentialHmac, undefined);
});
test("verified token is fenced by expiry before any future connection commit", async () => {
  const f = await ready();
  const result = await f.verify().verify({ ...f.context, playerId: "B" }, f.credential.code, f.credential.verificationValue);
  f.now = f.account().transfer.expiresAtMilliseconds; await f.lifecycle().status(f.context);
  await assert.rejects(f.store.compareExchange(result.account, { ...result.account.value, currentPlayerId: "B", status: "Active" }));
  assert.equal(f.account().currentPlayerId, "A"); assert.equal(f.account().transfer.status, "Expired");
});
test("confirmed terminal response is discarded if a new transfer starts before confirmation", async () => {
  const f = await ready(); const store = { ...f.store }; let reads = 0;
  store.read = async (kind, id) => {
    if (kind === "account" && ++reads === 2) await f.start("00000000-0000-4000-8000-000000000002").start(f.context);
    return f.store.read(kind, id);
  };
  await assert.rejects(createTransferLifecycleService(store, () => f.now).cancel(f.context, f.transferId));
  assert.equal(f.account().transfer.transferId, "00000000-0000-4000-8000-000000000002");
});
test("endpoints use bundled dependencies and fixed safe errors", async () => {
  for (const file of ["cancel-account-transfer", "get-account-transfer-status"]) {
    const logs = []; const module = { exports: {} };
    vm.runInNewContext(fs.readFileSync(require.resolve(`../CloudCode/${file}.js`), "utf8"), {
      module, require: name => name === "./account-store" ? { createAccountStore: () => { throw Error("RAW-SECRET-ID"); } } :
        { createTransferLifecycleService: () => null }
    });
    const response = await module.exports({ context: {}, params: {}, logger: { warning: m => logs.push(m) } });
    assert.equal(response.reason, "TransferUnavailable"); assert.equal(JSON.stringify(logs).includes("RAW-SECRET-ID"), false);
    assert.equal(module.exports.bundling, true);
  }
});
(async () => {
  let passed = 0;
  for (const t of tests) { try { await t.run(); passed++; console.log(`PASS ${t.name}`); }
    catch (error) { console.error(`FAIL ${t.name}`, error); process.exitCode = 1; } }
  console.log(`${passed}/${tests.length} passed`);
})();
