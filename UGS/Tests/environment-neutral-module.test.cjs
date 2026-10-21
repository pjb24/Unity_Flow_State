// Offline behavioral coverage of the real stores, services and embedded shim.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const { fixture } = require("./account-sdk-double.cjs");
const { runtime } = require("./phase2-module-runtime.test.cjs");
const { createAccountStore, validateContext } = require("../CloudCode/account-store");
const { createAccountService } = require("../CloudCode/account-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createReceiptStore } = require("../CloudCode/receipt-store");
const { createReceiptJournal } = require("../CloudCode/receipt-journal");
const { createTransferCryptography } = require("../CloudCode/transfer-cryptography");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferCompletionService } = require("../CloudCode/transfer-completion-service");
const { plan } = require("../CloudCode/ledger-v2-migration");
const key = Buffer.alloc(32, 7).toString("base64"); // Synthetic, never a deployment key.
const account = "11111111-1111-4111-8111-111111111111";
const submission = "22222222-2222-4222-8222-222222222222";
const scopeA = { projectId: "synthetic-project", environmentId: "verification-fixture" };
const scopeB = { projectId: "synthetic-project", environmentId: "production-fixture" };

async function main() {
  const fixtures = [fixture(scopeA), fixture(scopeB)];
  await Promise.all(fixtures.map(async f => {
    const store = createAccountStore(f.context, f.save);
    f.store = store;
    assert.ok(Object.isFrozen(store.scope));
    assert.deepEqual(store.scope, { projectId: f.context.projectId, environmentId: f.context.environmentId });
    const scope = { schemaVersion: 1, ...store.scope };
    const provision = createAccountProvisioningService(store,
      createLegacyAccountStore(f.context, f.save, f.leaderboard, store.scope));
    await provision.ensure(f.context);
    const boundAccount = f.value("fs8-player-A").accountId;
    assert.equal(f.value("fs8-account-" + boundAccount).environmentId, f.context.environmentId);
    const receipts = createReceiptStore(f.context, f.save, store.scope);
    const journal = createReceiptJournal(receipts);
    await journal.append(account, "202610", { id: submission, payload: "[]", status: "Submitted",
      reason: "Accepted", acceptedAt: Date.UTC(2026, 9, 1) });
    assert.equal((await journal.find(account, submission)).status, "Submitted");
    assert.equal((await receipts.ensureManifest(account, "202610", 1)).value.environmentId, scope.environmentId);
    const ledger = { ...scope, accountId: account, version: 1, active: "", best: {}, entries: [] };
    assert.equal(plan(ledger, account, store.scope).ledger.environmentId, scope.environmentId);
    const b = { ...f.context, playerId: "B" };
    await provision.ensure(b);
    const clock = () => 100000;
    const credential = await createTransferStartService(store, async () => key, { clock }).start(f.context);
    assert.equal(credential.status, "Success");
    assert.equal((await createTransferCompletionService(store, async () => key, clock)
      .complete(b, credential.code, credential.verificationValue)).status, "Success");
    assert.equal(f.value("fs8-account-" + boundAccount).currentPlayerId, "B");
  }));

  const [a, b] = fixtures;
  const writes = a.state.writes;
  await assert.rejects(createAccountService(a.store).resolve(b.context), /ContextMismatch/);
  assert.throws(() => createReceiptStore(b.context, a.save, a.store.scope), /ContextMismatch/);
  assert.throws(() => createLegacyAccountStore(b.context, a.save, a.leaderboard, a.store.scope), /ContextMismatch/);
  await assert.rejects(a.store.create("account", "cross-scope", { schemaVersion: 1, ...scopeB }), /StoredScopeMismatch/);
  assert.equal(a.state.writes, writes);
  for (const context of [null, { ...a.context, projectId: "" }, { ...a.context, environmentId: " " }])
    assert.throws(() => validateContext(context), /ContextMismatch/);

  const cryptoA = createTransferCryptography(key, undefined, scopeA);
  const cryptoB = createTransferCryptography(key, undefined, scopeB);
  assert.notEqual(cryptoA.codeDigest("ABCDEFGH"), cryptoB.codeDigest("ABCDEFGH"));
  assert.notEqual(cryptoA.credentialHmac("transfer", 1, "ABCDEFGH", "000000007"),
    cryptoB.credentialHmac("transfer", 1, "ABCDEFGH", "000000007"));
  assert.throws(() => createTransferCryptography(key), /ContextMismatch/);
  // Same scope and unchanged HMAC value preserve the previously issued credentials.
  const nodeCrypto = require("node:crypto");
  const priorDigest = nodeCrypto.createHmac("sha256", Buffer.from(key, "base64"))
    .update(JSON.stringify(["fs-transfer-code-v1", scopeA.projectId, scopeA.environmentId, "ABCDEFGH"]))
    .digest("base64url");
  assert.equal(cryptoA.codeDigest("ABCDEFGH"), priorDigest);

  // Each Module invocation owns its Engine/cache; interleave real shim execution.
  await Promise.all([scopeA, scopeB].map(async executionScope => {
    const f = fixture(executionScope), module = runtime(f);
    assert.equal((await module.call("get-public-player-number")).status, "Success");
    const candidate = { boardId: "fs-stage-stage-001-r1", rulesVersion: 1, submissionId: submission,
      score: 60000, runDurationMilliseconds: 0, baseDistanceScore: 0, momentumBonus: 0,
      distanceScore: 0, collectibleScore: 0, totalScore: 0, maximumMomentumMultiplier: 1 };
    assert.equal((await module.call("submit-record", { request: JSON.stringify(candidate) })).status, "Submitted");
    const result = await module.call("query-records", { request: JSON.stringify({
      boardId: candidate.boardId, kind: "me", limit: 1 }) });
    assert.equal(result.status, "Success");
    assert.equal(result.entries[0].score, 60000);
    const binding = f.value("fs8-player-A");
    assert.equal(binding.environmentId, executionScope.environmentId);
    assert.equal((await module.call("get-account-personal-bests")).status, "Success");
  }));

  // Source checks cover the C# boundary, which Node cannot execute.
  const moduleRoot = path.resolve(__dirname, "../Modules/FlowStateVerification");
  const host = fs.readFileSync(path.join(moduleRoot, "ServerServices.cs"), "utf8");
  assert.ok(host.includes("ProjectId = context.ProjectId"));
  assert.ok(host.includes("EnvironmentId = context.EnvironmentId"));
  assert.ok(host.includes("args[0] != ProjectId"));
  // C# name resolution is source-checked; the compiled host is tested by the user.
  for (const name of ["FS_TRANSFER_HMAC", "FS_LOG_SINK_HMAC", "FS_LOG_SINK_URL"]) {
    assert.ok(host.includes(`case "${name}_V1":`));
    assert.ok(host.includes(`return EnvironmentName == "verification" ? "${name}_VERIFICATION_V1" : logicalName;`));
  }
  assert.ok(host.includes('EnvironmentName != "verification" && EnvironmentName != "production"'));
  assert.ok(host.includes('EnvironmentName = NormalizeEnvironmentName(context.EnvironmentName)'));
  assert.ok(host.includes('name.Trim().ToLowerInvariant()'));
  assert.ok(host.includes('normalized == "verification" || normalized == "production" ? normalized : null'));
  assert.ok(host.includes('getSecret(EnvironmentNameSecretName, remaining)'));
  assert.ok(host.includes('internal const string EnvironmentNameSecretName = "FS_ENVIRONMENT_NAME_V1"'));
  assert.ok(host.includes('if (EnvironmentName != null) return;'));
  assert.ok(!host.includes('static string EnvironmentName'));
  const sink = fs.readFileSync(path.join(moduleRoot, "SafeLogSinkClient.cs"), "utf8");
  assert.ok(sink.includes("Environment = _services.EnvironmentName"));
  assert.ok(sink.includes('_services.EnsureEnvironmentName(_getSecret, GetRemainingTelemetryMilliseconds(telemetryClock))'));
  assert.ok(!sink.includes("_services.EnvironmentId"));
  assert.ok(sink.includes("_getSecret(_services.ResolveSecretName(name), remaining)"));
  const runtimeHost = fs.readFileSync(path.join(moduleRoot, "ServerScriptRuntime.cs"), "utf8");
  assert.ok(runtimeHost.includes("using Engine engine = new Engine"));
  assert.ok(!runtimeHost.includes("static Engine"));
  assert.ok(runtimeHost.includes("getSecret(services.ResolveSecretName(name), services.RemainingMilliseconds)"));
  assert.ok(runtimeHost.includes("services.EnsureEnvironmentName(getSecret, services.RemainingMilliseconds)"));
  for (const directory of [path.resolve(__dirname, "../CloudCode"), moduleRoot]) {
    for (const name of fs.readdirSync(directory).filter(n => /\.(js|cs)$/.test(n))) {
      const source = fs.readFileSync(path.join(directory, name), "utf8");
      assert.ok(!/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i.test(source), name + ": fixed scope identifier");
    }
  }
  console.log("PASS environment-neutral Module: concurrent scopes, provisioning, receipt/ledger, transfer, HMAC parity/isolation, embedded submit/query and C# source boundaries; .NET execution not performed.");
}
main().catch(error => { console.error(error); process.exitCode = 1; });
