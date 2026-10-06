const assert = require("node:assert/strict");
const fs = require("node:fs");
const vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore, key } = require("../CloudCode/account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferVerificationService } = require("../CloudCode/transfer-verification-service");
const policy = require("../CloudCode/transfer-credential-policy");
const secret = Buffer.alloc(32, 7).toString("base64"); // Test-only, never remote.
const tests = [];
const test = (name, run) => tests.push({ name, run });
function entropy(code, value = 7) {
  return size => {
    if (size === 8) return Buffer.from([...code].map(char => policy.transferCodeSymbols.indexOf(char)));
    const bytes = Buffer.alloc(size); bytes.writeUInt32BE(value); return bytes;
  };
}
async function ready(code = "ABCDEFGH") {
  const f = fixture(); f.store = createAccountStore(f.context, f.save); f.now = 100000;
  await createAccountProvisioningService(f.store,
    createLegacyAccountStore(f.context, f.save, f.leaderboard)).ensure(f.context);
  f.accountId = f.value("fs8-player-A").accountId;
  f.account = () => f.value(`fs8-account-${f.accountId}`);
  f.start = (newCode = "23456789", value = 8, provider = async () => secret) =>
    createTransferStartService(f.store, provider, { clock: () => f.now,
      randomBytes: entropy(newCode, value), idGenerator: () => "00000000-0000-4000-8000-000000000001" });
  f.verify = (provider = async () => secret) => createTransferVerificationService(f.store, provider, () => f.now);
  f.b = { ...f.context, playerId: "B" };
  f.credential = await f.start(code, 7).start(f.context);
  f.state.writes = 0; f.history = [];
  f.state.beforeWrite = async op => { f.history.push(clone(op.data)); };
  return f;
}
test("reissue keeps transfer, connection, original expiry and counters but changes revision", async () => {
  const f = await ready();
  await f.verify().verify(f.b, f.credential.code, "000000999");
  const before = clone(f.account()); f.now++;
  const response = await f.start().reissue(f.context); const after = f.account();
  assert.equal(response.status, "Success"); assert.equal(response.verificationValue, "000000008");
  assert.equal(after.status, "TransferPending");
  for (const name of ["accountId", "currentPlayerId", "connectionRevision", "publicPlayerNumber", "leaderboardOwnerId"])
    assert.equal(after[name], before[name]);
  for (const name of ["transferId", "issuedAtMilliseconds", "expiresAtMilliseconds", "lastAttemptAtMilliseconds", "failureCount"])
    assert.equal(after.transfer[name], before.transfer[name]);
  assert.equal(after.transfer.credentialRevision, 2); assert.equal(after.transfer.credentialActive, true);
  assert.notEqual(after.transfer.codeDigest, before.transfer.codeDigest);
  assert.equal((await f.verify().verify(f.b, f.credential.code, f.credential.verificationValue)).status, "InvalidCredential");
  assert.equal((await f.verify().verify(f.b, response.code, response.verificationValue)).status, "TooManyRequests");
  f.now += 5000;
  assert.equal((await f.verify().verify(f.b, response.code, response.verificationValue)).status, "Verified");
});
test("old lookup is not authority even during new credential reservation", async () => {
  const f = await ready(); let checked = false;
  f.state.beforeWrite = async op => {
    if (!checked && op.id.startsWith("fs8-t-")) {
      checked = true;
      assert.equal(f.account().transfer.credentialActive, false);
      assert.equal((await f.verify().verify(f.b, f.credential.code, f.credential.verificationValue)).status, "InvalidCredential");
    }
  };
  await f.start().reissue(f.context); assert.equal(checked, true);
});
test("reissue code collision cannot recycle the old code and regenerates", async () => {
  const f = await ready(); let calls = 0;
  const randomBytes = size => entropy(calls++ < 2 ? "ABCDEFGH" : "23456789", 8)(size);
  const service = createTransferStartService(f.store, async () => secret, { clock: () => f.now, randomBytes });
  assert.equal((await service.reissue(f.context)).code, "2345-6789");
});
test("reissue collision exhaustion invalidates old credentials and remains recoverable", async () => {
  const f = await ready();
  await assert.rejects(f.start("ABCDEFGH").reissue(f.context));
  assert.equal(f.account().transfer.credentialActive, false);
  assert.equal((await f.verify().verify(f.b, f.credential.code, f.credential.verificationValue)).status, "InvalidCredential");
  assert.equal((await f.start().reissue(f.context)).status, "Success");
  assert.equal(f.account().transfer.credentialRevision, 3);
});
test("5 second throttle is shared across B players and exact boundary is accepted", async () => {
  const f = await ready();
  assert.equal((await f.verify().verify(f.b, f.credential.code, "000000999")).status, "InvalidCredential");
  f.now += 4999;
  const result = await f.verify().verify({ ...f.b, playerId: "B2" }, f.credential.code, f.credential.verificationValue);
  assert.deepEqual(result, { status: "TooManyRequests", retryAtMilliseconds: 105000 });
  f.now++;
  assert.equal((await f.verify().verify(f.b, f.credential.code, f.credential.verificationValue)).status, "Verified");
  f.now += 5001;
  assert.equal((await f.verify().verify(f.b, f.credential.code, f.credential.verificationValue)).status, "Verified");
});
test("success attempts are throttled too and malformed value charges a known request", async () => {
  const f = await ready();
  assert.equal((await f.verify().verify(f.b, f.credential.code, 7)).status, "InvalidCredential");
  assert.equal(f.account().transfer.failureCount, 1);
  f.now += 5000;
  assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "Verified");
  assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "TooManyRequests");
});
test("normalization aliases and leading zero use production HMAC", async () => {
  const f = await ready("01234567");
  assert.equal((await f.verify().verify(f.b, "ol23-4567", "000000007")).status, "Verified");
  f.now += 5000;
  assert.equal((await f.verify().verify(f.b, "oi23-4567", "000000007")).status, "Verified");
});
test("invalid and unknown code, wrong context and Secret failure never mutate", async () => {
  const f = await ready(); const before = clone(f.account());
  for (const code of [null, "short", "UUUUUUUU", "ZZZZZZZZ"])
    assert.equal((await f.verify().verify(f.b, code, "000000007")).status, "InvalidCredential");
  await assert.rejects(f.verify().verify({ ...f.b, environmentId: "wrong" }, f.credential.code, "000000007"));
  await assert.rejects(f.verify(async () => "weak").verify(f.b, f.credential.code, "000000007"));
  assert.deepEqual(f.account(), before); assert.equal(f.state.writes, 0);
});
test("missing/invalid clock or attempt time fails closed and rollback is throttled", async () => {
  for (const now of [undefined, NaN, -1]) {
    const f = await ready(); f.now = now;
    await assert.rejects(f.verify().verify(f.b, f.credential.code, "000000007"));
    assert.equal(f.state.writes, 0);
  }
  for (const last of [undefined, "100000", -1]) {
    const f = await ready(); f.account().transfer.lastAttemptAtMilliseconds = last;
    await assert.rejects(f.verify().verify(f.b, f.credential.code, "000000007"));
    assert.equal(f.state.writes, 0);
  }
  const f = await ready(); f.now += 5000;
  await f.verify().verify(f.b, f.credential.code, "000000007"); f.now--;
  assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "TooManyRequests");
});
test("unlimited failures never permanently block, even with saturated audit count", async () => {
  const f = await ready();
  for (let i = 0; i < 30; i++) {
    assert.equal((await f.verify().verify(f.b, f.credential.code, "000000999")).status, "InvalidCredential"); f.now += 5000;
  }
  assert.equal(f.account().transfer.failureCount, 30);
  f.account().transfer.failureCount = Number.MAX_SAFE_INTEGER;
  await f.verify().verify(f.b, f.credential.code, "000000999"); f.now += 5000;
  assert.equal(f.account().transfer.failureCount, Number.MAX_SAFE_INTEGER);
  assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "Verified");
});
test("concurrent verification has one CAS winner and one audit increment", async () => {
  const f = await ready();
  const results = await Promise.allSettled([f.verify().verify(f.b, f.credential.code, "000000999"),
    f.verify().verify({ ...f.b, playerId: "B2" }, f.credential.code, "000000999")]);
  assert.equal(results.filter(r => r.status === "fulfilled").length, 1);
  assert.equal(f.account().transfer.failureCount, 1);
});
test("simultaneous reissue has one Account CAS winner", async () => {
  const f = await ready();
  const results = await Promise.allSettled([f.start().reissue(f.context), f.start("3456789A").reissue(f.context)]);
  assert.equal(results.filter(r => r.status === "fulfilled").length, 1);
  assert.equal(f.account().transfer.credentialRevision, 2);
});
test("reissue winning CAS fences verification of an old credential", async () => {
  const f = await ready(); let raced = false;
  f.state.beforeWrite = async op => {
    if (!raced && op.id.startsWith("fs8-account-") && op.data[0].value.transfer.lastAttemptAtMilliseconds !== null) {
      raced = true; await f.start().reissue(f.context);
    }
  };
  await assert.rejects(f.verify().verify(f.b, f.credential.code, "000000007"));
  assert.equal(f.account().transfer.lastAttemptAtMilliseconds, null);
  assert.equal(f.account().transfer.credentialRevision, 2);
});
test("verification winning CAS fences a stale reissue and retains its throttle", async () => {
  const f = await ready(); let raced = false;
  f.state.beforeWrite = async op => {
    if (!raced && op.id.startsWith("fs8-account-") && op.data[0].value.transfer.credentialRevision === 2) {
      raced = true; await f.verify().verify(f.b, f.credential.code, "000000007");
    }
  };
  await assert.rejects(f.start().reissue(f.context));
  assert.equal(f.account().transfer.credentialRevision, 1);
  await f.start().reissue(f.context);
  assert.equal(f.account().transfer.lastAttemptAtMilliseconds, f.now);
});
test("verified snapshot cannot authorize a commit after reissue", async () => {
  const f = await ready(); const verified = await f.verify().verify(f.b, f.credential.code, "000000007");
  await f.start().reissue(f.context);
  await assert.rejects(f.store.compareExchange(verified.account,
    { ...verified.account.value, status: "Active", currentPlayerId: "B" }));
  assert.equal(f.account().currentPlayerId, "A");
});
test("reissue after the confirmation snapshot still fences its commit token", async () => {
  const f = await ready(); let reads = 0;
  f.state.afterRead = async op => {
    if (op.id === `fs8-account-${f.accountId}` && ++reads === 2) {
      f.state.afterRead = null; await f.start().reissue(f.context);
    }
  };
  // SDK returned the old snapshot: its write lock is fenced even if the
  // reissue lands after the confirmation read's snapshot was taken.
  const result = await f.verify().verify(f.b, f.credential.code, "000000007");
  assert.equal(result.status, "Verified");
  await assert.rejects(f.store.compareExchange(result.account, result.account.value));
});
test("reissue before the confirmation snapshot rejects the old verification result", async () => {
  const f = await ready(); const store = { ...f.store }; let reads = 0;
  store.read = async (kind, id) => {
    if (kind === "account" && ++reads === 2) await f.start().reissue(f.context);
    return f.store.read(kind, id);
  };
  const verifier = createTransferVerificationService(store, async () => secret, () => f.now);
  assert.equal((await verifier.verify(f.b, f.credential.code, "000000007")).status, "InvalidCredential");
});
test("expiry during Secret retrieval cannot invalidate old credentials or extend the request", async () => {
  const f = await ready(); const before = clone(f.account());
  await assert.rejects(f.start("23456789", 8, async () => {
    f.now = before.transfer.expiresAtMilliseconds; return secret;
  }).reissue(f.context));
  assert.deepEqual(f.account(), before); assert.equal(f.state.writes, 0);
});
test("expiry after attempt commit cannot return a verified snapshot", async () => {
  const f = await ready(); const store = { ...f.store }; let reads = 0;
  store.read = async (kind, id) => {
    if (kind === "account" && ++reads === 2) f.now = f.account().transfer.expiresAtMilliseconds;
    return f.store.read(kind, id);
  };
  const verifier = createTransferVerificationService(store, async () => secret, () => f.now);
  assert.equal((await verifier.verify(f.b, f.credential.code, "000000007")).status, "InvalidCredential");
});
test("every reissue write failure before/after commit keeps old validity or safe new lock", async () => {
  const baseline = await ready(); await baseline.start().reissue(baseline.context);
  const count = baseline.state.writes;
  for (const mode of ["failAt", "loseAt"]) for (let at = 1; at <= count; at++) {
    const f = await ready(); f.state[mode] = at;
    await assert.rejects(f.start().reissue(f.context)); f.state[mode] = 0;
    assert.equal(f.account().status, "TransferPending");
    if (f.account().transfer.credentialRevision === 1) {
      assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "Verified");
    } else {
      assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "InvalidCredential");
      const resumed = await f.start().start(f.context);
      assert.deepEqual(resumed, { status: "TransferPending", reason: "CredentialReissueRequired" });
    }
    const saved = JSON.stringify([...f.privateItems.values(), ...f.history]);
    for (const raw of ["ABCD-EFGH", "ABCDEFGH", "23456789", "2345-6789", "000000007", "000000008", secret])
      assert.equal(saved.includes(raw), false);
  }
});
test("attempt write failure before commit retries, response loss preserves throttle", async () => {
  for (const mode of ["failAt", "loseAt"]) {
    const f = await ready(); f.state[mode] = 1;
    await assert.rejects(f.verify().verify(f.b, f.credential.code, "000000999")); f.state[mode] = 0;
    const result = await f.verify().verify(f.b, f.credential.code, "000000999");
    assert.equal(result.status, mode === "failAt" ? "InvalidCredential" : "TooManyRequests");
    assert.equal(f.account().transfer.failureCount, 1);
  }
});
test("inactive A or Secret/entropy failure cannot invalidate the old credential", async () => {
  const f = await ready(); const before = clone(f.account());
  await assert.rejects(f.start().reissue(f.b));
  await assert.rejects(f.start("23456789", 8, async () => "weak").reissue(f.context));
  await assert.rejects(createTransferStartService(f.store, async () => secret, { clock: () => f.now,
    randomBytes: () => { throw Error("entropy-failure"); } }).reissue(f.context));
  assert.equal(f.state.writes, 0); assert.deepEqual(f.account(), before);
});
test("original expiry just before allows reissue; exact/after reject without extension", async () => {
  for (const offset of [-1, 0, 1]) {
    const f = await ready(); const expires = f.account().transfer.expiresAtMilliseconds; f.now = expires + offset;
    if (offset < 0) {
      assert.equal((await f.start().reissue(f.context)).expiresAtMilliseconds, expires);
    } else {
      await assert.rejects(f.start().reissue(f.context)); assert.equal(f.state.writes, 0);
      assert.equal((await f.verify().verify(f.b, f.credential.code, "000000007")).status, "InvalidCredential");
    }
  }
});
test("reissue endpoint does not leak Secret or SDK errors", async () => {
  const logs = []; const module = { exports: {} };
  vm.runInNewContext(fs.readFileSync(require.resolve("../CloudCode/reissue-account-transfer.js"), "utf8"), {
    module, require: name => name === "./account-store" ? { createAccountStore: () => { throw Error("SECRET-RAW"); } } :
      name === "./transfer-cryptography" ? { createTransferSecretProvider: () => null } : { createTransferStartService: () => null }
  });
  const result = await module.exports({ context: {}, logger: { warning: m => logs.push(m) } });
  assert.deepEqual(JSON.parse(JSON.stringify(result)), { status: "TransientFailure", reason: "TransferUnavailable" });
  assert.equal(JSON.stringify(logs).includes("SECRET-RAW"), false);
  assert.equal(module.exports.bundling, true);
});
(async () => {
  let passed = 0;
  for (const t of tests) { try { await t.run(); passed++; console.log(`PASS ${t.name}`); }
    catch (error) { console.error(`FAIL ${t.name}`, error); process.exitCode = 1; } }
  console.log(`${passed}/${tests.length} passed`);
})();
