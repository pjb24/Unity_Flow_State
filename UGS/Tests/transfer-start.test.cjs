// Production service + actual crypto + injected SDK, clock and entropy.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore, projectId, environmentId, key } = require("../CloudCode/account-store");
const { createAccountService } = require("../CloudCode/account-service");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createTransferStartService, maximumCodeAttempts } = require("../CloudCode/transfer-start-service");
const { createTransferCryptography, createTransferSecretProvider, secretName } = require("../CloudCode/transfer-cryptography");
const policy = require("../CloudCode/transfer-credential-policy");
const tests = [];
const test = (name, run) => tests.push({ name, run });
// Deliberately fake, test-only key; no Secret Manager or remote access.
const secret = Buffer.alloc(32, 7).toString("base64");
const transferId = "00000000-0000-4000-8000-000000000001";
async function ready() {
  const f = fixture();
  const store = createAccountStore(f.context, f.save);
  await createAccountProvisioningService(store, createLegacyAccountStore(f.context, f.save, f.leaderboard)).ensure(f.context);
  f.accountId = f.value("fs8-player-A").accountId;
  f.state.writes = 0;
  f.now = 100000;
  f.history = [];
  f.state.beforeWrite = async operation => { f.history.push(clone(operation.data)); };
  f.account = () => f.value(`fs8-account-${f.accountId}`);
  f.ledger = () => f.value(`fs8-ledger-${f.accountId}`);
  f.store = store;
  return f;
}
function randomForCode(code = "ABCDEFGH", verification = 7) {
  return size => {
    if (size === 8) return Buffer.from([...code].map(char => policy.transferCodeSymbols.indexOf(char)));
    const bytes = Buffer.alloc(size); bytes.writeUInt32BE(verification, 0); return bytes;
  };
}
function service(f, options = {}, provider = async () => secret) {
  return createTransferStartService(f.store, provider,
    { clock: () => f.now, idGenerator: () => transferId, randomBytes: randomForCode(), ...options });
}
function occupied(f, code) {
  const digest = createTransferCryptography(secret).codeDigest(code);
  f.seedPrivate(`fs8-t-${digest}`, key, { schemaVersion: 1, projectId, environmentId,
    accountId: "other", transferId: "other", credentialRevision: 1 });
  return digest;
}
test("start locks the Account and activates only the reserved HMAC lookup", async () => {
  const f = await ready(); const before = clone(f.account());
  const response = await service(f).start(f.context);
  assert.deepEqual(response, { status: "Success", code: "ABCD-EFGH", verificationValue: "000000007",
    expiresAtMilliseconds: f.now + policy.transferLifetimeMilliseconds });
  const account = f.account(); const transfer = account.transfer;
  assert.equal(account.status, "TransferPending");
  assert.equal(account.currentPlayerId, before.currentPlayerId);
  assert.equal(account.publicPlayerNumber, before.publicPlayerNumber);
  assert.equal(account.leaderboardOwnerId, before.leaderboardOwnerId);
  assert.equal(account.connectionRevision, before.connectionRevision);
  assert.equal(transfer.credentialActive, true);
  assert.equal(f.value(`fs8-t-${transfer.codeDigest}`).transferId, transfer.transferId);
  assert.equal(f.value(`fs8-t-${transfer.codeDigest}`).accountId, f.accountId);
  assert.match(transfer.codeDigest, /^[A-Za-z0-9_-]{43}$/);
  assert.match(transfer.credentialHmac, /^[a-f0-9]{64}$/);
  const saved = JSON.stringify([...f.privateItems.values(), ...f.history]);
  for (const raw of ["ABCDEFGH", "ABCD-EFGH", "000000007", secret]) assert.equal(saved.includes(raw), false);
});
test("repeated start/restart never returns raw credentials or creates another transfer", async () => {
  const f = await ready(); await service(f).start(f.context); const first = clone(f.account().transfer);
  const result = await service(f).start(f.context);
  assert.deepEqual(result, { status: "TransferPending", reason: "CredentialReissueRequired" });
  assert.deepEqual(f.account().transfer, first);
});
test("Secret missing, wrong format, and provider failure leave the Account Active", async () => {
  for (const provider of [null, async () => "", async () => "weak", async () => { throw Error("test-provider-failure"); }]) {
    const f = await ready(); const before = clone(f.account());
    await assert.rejects(service(f, {}, provider).start(f.context));
    assert.deepEqual(f.account(), before);
    assert.equal(f.state.writes, 0);
  }
});
test("Secret Manager adapter uses the exact name and hides SDK error contents", async () => {
  const provider = createTransferSecretProvider({ async getSecret(name) {
    assert.equal(name, secretName); return { value: secret };
  }});
  assert.equal(await provider(), secret);
  await assert.rejects(createTransferSecretProvider({ async getSecret() { throw Error(secret); } })(), error => {
    assert.equal(error.message, "TransferSecretUnavailable"); return true;
  });
  await assert.rejects(createTransferSecretProvider(null)(), /TransferSecretUnavailable/);
});
test("unavailable entropy never locks an account or uses a fallback generator", async () => {
  const f = await ready();
  await assert.rejects(service(f, { randomBytes: () => { throw Error("entropy-unavailable"); } }).start(f.context));
  assert.equal(f.account().status, "Active"); assert.equal(f.state.writes, 0);
});
test("uniform decimal generation rejects out-of-range samples then retains leading zero", () => {
  let calls = 0;
  const cryptography = createTransferCryptography(secret, size => {
    if (size === 8) return Buffer.alloc(8, 31);
    const bytes = Buffer.alloc(4); bytes.writeUInt32BE(calls++ === 0 ? 4294967295 : 7); return bytes;
  });
  assert.deepEqual(cryptography.generateCredential(), { code: "ZZZZZZZZ", verificationValue: "000000007" });
  assert.equal(calls, 2);
});
test("repeated rejected entropy is bounded and fails before storage", async () => {
  const f = await ready();
  await assert.rejects(service(f, { randomBytes: size => Buffer.alloc(size, 255) }).start(f.context), /RandomSourceUnavailable/);
  assert.equal(f.state.writes, 0);
});
test("real CSPRNG emits valid format and distinct credentials", () => {
  const cryptography = createTransferCryptography(secret);
  const values = new Set();
  for (let i = 0; i < 64; i++) {
    const value = cryptography.generateCredential();
    assert.equal(policy.tryNormalizeTransferCode(value.code), value.code);
    assert.equal(policy.tryNormalizeVerificationValue(value.verificationValue), value.verificationValue);
    values.add(`${value.code}/${value.verificationValue}`);
  }
  assert.equal(values.size, 64);
});
test("HMAC binds code aliases, credential, transfer, revision and key", () => {
  const cryptography = createTransferCryptography(secret);
  assert.equal(cryptography.codeDigest("ABCD-EFGH"), cryptography.codeDigest("abcdefgh"));
  const original = cryptography.credentialHmac(transferId, 1, "ABCDEFGH", "000000007");
  assert.notEqual(original, cryptography.credentialHmac(transferId, 1, "ABCDEFGH", "000000008"));
  assert.notEqual(original, cryptography.credentialHmac(transferId, 2, "ABCDEFGH", "000000007"));
  assert.notEqual(original, cryptography.credentialHmac("other", 1, "ABCDEFGH", "000000007"));
  assert.notEqual(cryptography.codeDigest("ABCDEFGH"),
    createTransferCryptography(Buffer.alloc(32, 8).toString("base64")).codeDigest("ABCDEFGH"));
});
test("occupied code is regenerated without replacing another account's lookup", async () => {
  const f = await ready(); const digest = occupied(f, "ABCDEFGH"); const before = clone(f.value(`fs8-t-${digest}`));
  let codes = 0;
  const response = await service(f, { randomBytes: size => {
    if (size === 8) return randomForCode(codes++ === 0 ? "ABCDEFGH" : "23456789")(size);
    return randomForCode()(size);
  }}).start(f.context);
  assert.equal(response.code, "2345-6789"); assert.equal(codes, 2);
  assert.deepEqual(f.value(`fs8-t-${digest}`), before);
});
test("collision retry exhaustion stays locked and cannot activate another account's code", async () => {
  const f = await ready(); occupied(f, "ABCDEFGH"); let generated = 0;
  await assert.rejects(service(f, { randomBytes: size => {
    if (size === 8) generated++; return randomForCode()(size);
  }}).start(f.context), /TransferCodeUnavailable/);
  assert.equal(generated, maximumCodeAttempts);
  assert.equal(f.account().status, "TransferPending");
  assert.equal(f.account().transfer.credentialActive, false);
  assert.deepEqual(await service(f).start(f.context), { status: "TransferPending", reason: "CredentialReissueRequired" });
});
test("different accounts racing for the same code regenerate the loser's credential", async () => {
  const f = await ready(); const bContext = { ...f.context, playerId: "B" };
  const bStore = createAccountStore(bContext, f.save);
  await createAccountProvisioningService(bStore, createLegacyAccountStore(bContext, f.save, f.leaderboard)).ensure(bContext);
  function rotatingEntropy() {
    let generated = 0;
    return size => randomForCode(size === 8 && generated++ > 0 ? "23456789" : "ABCDEFGH")(size);
  }
  const aService = service(f, { randomBytes: rotatingEntropy() });
  const bService = createTransferStartService(bStore, async () => secret,
    { clock: () => f.now, randomBytes: rotatingEntropy(),
      idGenerator: () => "00000000-0000-4000-8000-000000000002" });
  const result = await Promise.all([aService.start(f.context), bService.start(bContext)]);
  assert.equal(new Set(result.map(value => value.code)).size, 2);
  assert.ok(result.every(value => value.status === "Success"));
  const bAccountId = f.value("fs8-player-B").accountId;
  const bAccount = f.value(`fs8-account-${bAccountId}`);
  assert.notEqual(f.account().transfer.codeDigest, bAccount.transfer.codeDigest);
  assert.equal(f.value(`fs8-t-${bAccount.transfer.codeDigest}`).accountId, bAccountId);
});
test("simultaneous starts have a single CAS winner and no second transfer", async () => {
  const f = await ready(); const result = await Promise.allSettled([service(f).start(f.context), service(f).start(f.context)]);
  assert.equal(result.filter(r => r.status === "fulfilled" && r.value.status === "Success").length, 1);
  assert.equal(f.account().transfer.transferId, transferId);
  assert.equal(f.account().transfer.credentialActive, true);
});
test("submission reservation and legacy Pending each block transfer start", async () => {
  const f = await ready(); await createAccountService(f.store).reserveSubmission(f.context, transferId);
  const before = clone(f.account()); await assert.rejects(service(f).start(f.context), /AccountBusy/);
  assert.deepEqual(f.account(), before);
  for (const pending of [true, false]) {
    const g = await ready();
    if (pending) g.ledger().active = "pending";
    else g.ledger().entries.push({ id: "pending", status: "Pending" });
    await assert.rejects(service(g).start(g.context), /AccountBusy/);
    assert.equal(g.account().status, "Active");
  }
});
test("submission winning the Account CAS fences a stale transfer precheck", async () => {
  const f = await ready(); f.state.beforeWrite = async ({ data }) => {
    if (data.some(item => item.value.status === "TransferPending")) {
      f.state.beforeWrite = null;
      await createAccountService(f.store).reserveSubmission(f.context, transferId);
    }
  };
  await assert.rejects(service(f).start(f.context), /cas-conflict/);
  assert.equal(f.account().status, "Active"); assert.equal(f.account().onlineOperation.id, transferId);
});
test("transfer winning the Account CAS fences a stale submission reservation", async () => {
  const f = await ready(); f.state.beforeWrite = async ({ data }) => {
    if (data.some(item => item.value.onlineOperation)) {
      f.state.beforeWrite = null; await service(f).start(f.context);
    }
  };
  await assert.rejects(createAccountService(f.store).reserveSubmission(f.context, transferId), /cas-conflict/);
  assert.equal(f.account().status, "TransferPending"); assert.equal(f.account().onlineOperation, undefined);
});
test("every start write interruption either keeps Active or resumes without raw credentials", async () => {
  const baseline = await ready(); await service(baseline).start(baseline.context);
  for (const mode of ["failAt", "loseAt"]) {
    for (let step = 1; step <= baseline.state.writes; step++) {
      const f = await ready(); f.state[mode] = step;
      await assert.rejects(service(f).start(f.context));
      const pending = f.account().status === "TransferPending";
      const result = await service(f).start(f.context);
      if (pending) {
        assert.equal(result.reason, "CredentialReissueRequired");
        assert.equal(Object.hasOwn(result, "code"), false);
      } else assert.equal(result.status, "Success");
      assert.equal(f.account().transfer.credentialActive, true);
      const saved = JSON.stringify([...f.privateItems.values(), ...f.history]);
      for (const raw of ["ABCDEFGH", "000000007", secret]) assert.equal(saved.includes(raw), false);
    }
  }
});
test("inactive player, stale binding and wrong environment cannot lock an account", async () => {
  for (const change of ["inactive", "revision", "scope"]) {
    const f = await ready();
    if (change === "inactive") f.account().currentPlayerId = "B";
    if (change === "revision") f.account().connectionRevision++;
    if (change === "scope") f.account().environmentId = "other";
    await assert.rejects(service(f).start(f.context), /ActiveDeviceRequired|StoredScopeMismatch/);
    assert.equal(f.state.writes, 0);
  }
  const f = await ready(); await assert.rejects(service(f).start({ ...f.context, environmentId: "production" }), /ContextMismatch/);
});
test("expiry or clock reversal before activation cannot return credentials", async () => {
  for (const shift of [-1, policy.transferLifetimeMilliseconds]) {
    const f = await ready(); f.state.beforeWrite = async ({ data }) => {
      if (data.some(item => item.value.status === "TransferPending")) f.now = 100000 + shift;
    };
    await assert.rejects(service(f).start(f.context), /TransferExpired/);
    assert.equal(f.account().status, "TransferPending"); assert.equal(f.account().transfer.credentialActive, false);
  }
});
test("generic endpoint logs and errors cannot leak Secret SDK messages", async () => {
  const f = await ready(); const logs = [];
  const sandbox = { module: { exports: {} }, require: name => {
    if (name === "./account-store") return { createAccountStore: () => f.store };
    if (name === "./transfer-cryptography") return { createTransferSecretProvider };
    if (name === "./transfer-start-service") return { createTransferStartService };
    throw Error("unexpected dependency");
  }};
  vm.runInNewContext(fs.readFileSync(path.join(__dirname, "../CloudCode/start-account-transfer.js"), "utf8"), sandbox);
  const result = await sandbox.module.exports({ context: f.context,
    secretManager: { async getSecret() { throw Error(`${secret}/ABCDEFGH/000000007`); } },
    logger: { warning: message => logs.push(message) } });
  assert.equal(result.reason, "TransferUnavailable");
  for (const raw of [secret, "ABCDEFGH", "000000007"]) assert.equal(JSON.stringify({ result, logs }).includes(raw), false);
  assert.equal(f.state.writes, 0);
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
