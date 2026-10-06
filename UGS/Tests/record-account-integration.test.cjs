// Production endpoints/services with isolated SDK doubles. No remote requests.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { ready } = require("./transfer-completion.test.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const locator = (kind, value) => `fs8-${kind}-${value}`;
const { createAccountService } = require("../CloudCode/account-service");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferLifecycleService } = require("../CloudCode/transfer-lifecycle-service");
const { createRecordSubmissionService } = require("../CloudCode/submit-record");
const { createRecordQueryService } = require("../CloudCode/query-records");
const stage = "fs-stage-stage-001-r1";
const id = n => `11111111-1111-4111-8111-${String(n).padStart(12, "0")}`;
const request = (n = 1, score = 1000) => ({ boardId: stage, rulesVersion: 1, submissionId: id(n), score,
  runDurationMilliseconds: 0, baseDistanceScore: 0, momentumBonus: 0, distanceScore: 0,
  collectibleScore: 0, totalScore: 0, maximumMomentumMultiplier: 1 });
const query = (kind = "top", boardId = stage) => ({ boardId, kind, limit: 10 });
const invoke = (run, p, context) => run({ params: { request: JSON.stringify(p) }, context });
const tests = []; const test = (name, run) => tests.push({ name, run });
async function configured() {
  const f = fixture(); f.store = createAccountStore(f.context, f.save);
  f.provision = createAccountProvisioningService(f.store, createLegacyAccountStore(f.context, f.save, f.leaderboard));
  await f.provision.ensure(f.context);
  f.accountId = f.value("fs8-player-A").accountId;
  f.account = () => f.value(locator("account", f.accountId));
  f.ledger = () => f.value(locator("ledger", f.accountId));
  connect(f); return f;
}
function connect(f) {
  f.submit = (p = request(), context = f.context) => invoke(createRecordSubmissionService(f.store, f.leaderboard), p, context);
  f.query = (p = query(), context = f.context) => invoke(createRecordQueryService(f.store, f.leaderboard,
    owner => f.provision.ensure({ ...context, playerId: owner })), p, context);
}
function loadEndpoint(name, f) {
  const cache = new Map();
  function load(file) {
    if (cache.has(file)) return cache.get(file).exports;
    const module = { exports: {} }; cache.set(file, module);
    const localRequire = name => {
      if (name === "@unity-services/cloud-save-1.4") return { DataApi: class { constructor() { return f.save; } } };
      if (name === "@unity-services/leaderboards-1.1") return { LeaderboardsApi: class { constructor() { return f.leaderboard; } } };
      if (name.startsWith("./")) return load(path.resolve(path.dirname(file), name + ".js"));
      if (name === "crypto") return require("node:crypto");
      throw Error("Unexpected dependency");
    };
    vm.runInNewContext(fs.readFileSync(file, "utf8"), { module, require: localRequire, Buffer, console });
    return module.exports;
  }
  return load(path.resolve(__dirname, "../CloudCode", name + ".js"));
}
test("bundled actual submit/query endpoints use fixed owner and safe public fields", async () => {
  const f = await configured();
  const submit = loadEndpoint("submit-record", f), get = loadEndpoint("query-records", f);
  assert.equal((await invoke(submit, request(), f.context)).status, "Submitted");
  const result = await invoke(get, query(), f.context);
  assert.equal(result.status, "Success"); assert.equal(result.entries.length, 1);
  const row = result.entries[0]; assert.equal(row.isMe, true);
  assert.equal(row.publicPlayerNumber, f.account().publicPlayerNumber);
  assert.deepEqual(Object.keys(row).sort(), ["acceptedAt", "isMe", "publicPlayerNumber", "rank", "score"]);
  assert.ok(f.scores.has(`${stage}/${f.account().leaderboardOwnerId}`));
  assert.ok(!f.scores.has(`${stage}/A`));
});
test("B retains the same row, number and timestamp then submits to that row; A is denied", async () => {
  const f = await ready();
  await createTransferLifecycleService(f.store, () => f.now).cancel(f.context, f.c().transfer.transferId);
  f.provision = createAccountProvisioningService(f.store, createLegacyAccountStore(f.context, f.save, f.leaderboard));
  connect(f); assert.equal((await f.submit()).status, "Submitted");
  const owner = f.c().leaderboardOwnerId, number = f.c().publicPlayerNumber;
  const before = clone(f.scores.get(`${stage}/${owner}`));
  f.now += 1;
  f.credential = await createTransferStartService(f.store, async () => Buffer.alloc(32, 7).toString("base64"),
    { clock: () => f.now }).start(f.context);
  assert.equal((await f.submit(request(2))).status, "TransientFailure");
  assert.equal((await f.query()).status, "TransientFailure");
  assert.equal((await f.run()).status, "Success");
  const me = await f.query(query("me"), f.b);
  assert.equal(me.entries[0].publicPlayerNumber, number); assert.equal(me.entries[0].isMe, true);
  assert.deepEqual(f.scores.get(`${stage}/${owner}`), before);
  assert.equal((await f.submit(request(2), f.b)).status, "Submitted");
  assert.deepEqual(f.scores.get(`${stage}/${owner}`), before);
  assert.equal((await f.submit(request(3, 900), f.b)).status, "Submitted");
  assert.equal(f.scores.get(`${stage}/${owner}`).score, 900); assert.equal(f.scores.size, 1);
  assert.equal((await f.submit(request(), f.context)).status, "TransientFailure");
  assert.equal((await f.query(query("me"), f.context)).status, "TransientFailure");
});
test("legacy owner lazy issuance preserves exact rows and is not caller authority", async () => {
  const f = await configured();
  const legacy = { playerId: "legacy-X", score: 500, metadata: { acceptedAt: 10, submissionId: id(90) } };
  f.scores.set(`${stage}/legacy-X`, clone(legacy));
  const result = await f.query(); assert.equal(result.status, "Success");
  assert.equal(result.entries[0].isMe, false); assert.match(result.entries[0].publicPlayerNumber, /^\d{10}$/);
  assert.deepEqual(f.scores.get(`${stage}/legacy-X`), legacy);
  const binding = f.value("fs8-player-legacy-X");
  assert.equal(f.value(locator("account", binding.accountId)).leaderboardOwnerId, "legacy-X");
  assert.equal((await f.submit({ ...request(), playerId: "legacy-X", accountId: binding.accountId,
    leaderboardOwnerId: "legacy-X" })).status, "Submitted");
  assert.equal(f.scores.get(`${stage}/legacy-X`).score, 500);
  assert.equal((await f.query()).entries.filter(e => e.isMe).length, 1);
});
test("failed lazy number issuance returns an empty safe error and retry recovers", async () => {
  const f = await configured();
  f.scores.set(`${stage}/legacy-X`, { playerId: "legacy-X", score: 10, metadata: { acceptedAt: 1, submissionId: id(9) } });
  f.state.failAt = f.state.writes + 1;
  const failed = await f.query(); assert.equal(failed.status, "TransientFailure"); assert.deepEqual(failed.entries, []);
  assert.ok(!JSON.stringify(failed).includes("legacy-X")); assert.ok(!JSON.stringify(failed).includes(f.accountId));
  f.state.failAt = 0; assert.equal((await f.query()).status, "Success");
});
test("late A submission before Account reservation loses to transfer and never writes a row", async () => {
  const f = await configured(); let entered = false;
  f.state.beforeWrite = async op => {
    if (entered || !op.data.some(e => e.value.onlineOperation)) return;
    entered = true; f.state.beforeWrite = null;
    await createTransferStartService(f.store, async () => Buffer.alloc(32, 7).toString("base64")).start(f.context);
  };
  assert.equal((await f.submit()).status, "TransientFailure"); assert.equal(f.scores.size, 0);
  assert.equal(f.account().status, "TransferPending");
});
test("reserved submission fences transfer until terminal release", async () => {
  const f = await configured(); let checked = false;
  const original = f.leaderboard.addLeaderboardPlayerScore;
  f.leaderboard.addLeaderboardPlayerScore = async (...args) => {
    checked = true;
    await assert.rejects(createAccountService(f.store).assertTransferCanStart(f.context));
    return original(...args);
  };
  assert.equal((await f.submit()).status, "Submitted"); assert.ok(checked);
  assert.equal(f.account().onlineOperation, null);
  await createAccountService(f.store).assertTransferCanStart(f.context);
});
test("all four submission write boundaries recover before/after commit without duplicate receipts", async () => {
  for (const mode of ["failAt", "loseAt"]) for (let n = 1; n <= 4; n++) {
    const f = await configured(); f.state[mode] = f.state.writes + n;
    assert.equal((await f.submit()).status, "TransientFailure", `${mode}/${n}`);
    f.state[mode] = 0;
    assert.equal((await f.submit()).status, "Submitted", `${mode}/${n}`);
    assert.equal(f.ledger().entries.length, 1); assert.equal(f.ledger().active, "");
    assert.equal(f.account().onlineOperation, null); assert.equal(f.scores.size, 1);
  }
});
test("lost leaderboard response preserves reservation and timestamp", async () => {
  const f = await configured(); const original = f.leaderboard.addLeaderboardPlayerScore; let first = true;
  f.leaderboard.addLeaderboardPlayerScore = async (...args) => {
    await original(...args); if (first) { first = false; throw Error("lost-response"); }
  };
  assert.equal((await f.submit()).status, "TransientFailure"); const before = clone([...f.scores]);
  await assert.rejects(createAccountService(f.store).assertTransferCanStart(f.context));
  assert.equal((await f.submit()).status, "Submitted"); assert.deepEqual([...f.scores], before);
});
test("stale query after authority revision changes returns no entries", async () => {
  const f = await configured(); await f.submit();
  const original = f.leaderboard.getLeaderboardScores;
  f.leaderboard.getLeaderboardScores = async (...args) => {
    const result = await original(...args); f.account().connectionRevision++; return result;
  };
  const result = await f.query(); assert.equal(result.status, "TransientFailure"); assert.deepEqual(result.entries, []);
});
test("query begun before transfer activation is rejected after projection/mapping work", async () => {
  const f = await configured(); await f.submit(); const original = f.leaderboard.getLeaderboardScores;
  f.leaderboard.getLeaderboardScores = async (...args) => {
    const result = await original(...args);
    await createTransferStartService(f.store, async () => Buffer.alloc(32, 7).toString("base64")).start(f.context);
    return result;
  };
  assert.equal((await f.query()).status, "TransientFailure");
});
test("public-number final tie order and competition ranks are independent of owner ID", async () => {
  const f = await configured(); await f.provision.ensure({ ...f.context, playerId: "Z" });
  const other = f.value(locator("account", f.value("fs8-player-Z").accountId));
  for (const owner of [other.leaderboardOwnerId, f.account().leaderboardOwnerId])
    f.scores.set(`${stage}/${owner}`, { playerId: owner, score: 100, metadata: { acceptedAt: 5 } });
  const result = await f.query(); assert.equal(result.status, "Success");
  assert.deepEqual(result.entries.map(e => e.publicPlayerNumber), [f.account().publicPlayerNumber, other.publicPlayerNumber]);
  assert.deepEqual(result.entries.map(e => e.rank), [1, 1]);
});
test("detached old B rows remain public but never become B's own rows", async () => {
  const f = await ready(); const old = f.old();
  f.scores.set(`${stage}/${old.leaderboardOwnerId}`, { playerId: old.leaderboardOwnerId, score: 100, metadata: { acceptedAt: 5 } });
  await f.run();
  const result = await invoke(createRecordQueryService(f.store, f.leaderboard), query(), f.b);
  assert.equal(result.status, "Success"); assert.equal(result.entries[0].isMe, false);
  assert.equal(result.entries[0].publicPlayerNumber, old.publicPlayerNumber);
});
test("mismatched owner/number mappings and scope never leak identifiers or other numbers", async () => {
  for (const corruption of ["owner", "number", "scope", "numberValue"]) {
    const f = await configured(); await f.submit(); const a = f.account();
    if (corruption === "owner") f.value(locator("owner", a.leaderboardOwnerId)).accountId = id(99);
    if (corruption === "number") f.value(locator("number", a.publicPlayerNumber)).accountId = id(99);
    if (corruption === "scope") a.environmentId = "production";
    if (corruption === "numberValue") a.publicPlayerNumber = "9999999999";
    const result = await f.query(); assert.equal(result.status, "TransientFailure"); assert.deepEqual(result.entries, []);
    for (const privateValue of [f.accountId, a.leaderboardOwnerId, a.publicPlayerNumber])
      assert.ok(!JSON.stringify(result).includes(privateValue));
  }
});
test("query errors discard partial rows when a later row fails number lookup", async () => {
  const f = await configured(); await f.submit();
  f.scores.set(`${stage}/broken`, { playerId: "broken", score: 2000, metadata: { acceptedAt: 1 } });
  assert.deepEqual((await f.query()).entries, []);
});
test("context/project/environment/service token guards make zero writes", async () => {
  const f = await configured(); const before = f.state.writes;
  for (const change of [{ serviceToken: "" }, { playerId: "" }, { projectId: "wrong" }, { environmentId: "production" }]) {
    const context = { ...f.context, ...change };
    assert.equal((await f.submit(request(), context)).reason, "ContextMismatch");
    assert.equal((await f.query(query(), context)).reason, "ContextMismatch");
  }
  assert.equal(f.state.writes, before); assert.equal(f.scores.size, 0);
});
test("all submission SDK read failures retry without changing the accepted timestamp", async () => {
  const baseline = await configured(); let reads = 0;
  baseline.state.afterRead = async () => reads++;
  assert.equal((await baseline.submit()).status, "Submitted");
  for (let n = 1; n <= reads; n++) {
    const f = await configured(); let current = 0;
    f.state.afterRead = async () => { if (++current === n) throw Error("injected-read"); };
    assert.equal((await f.submit()).status, "TransientFailure", String(n));
    f.state.afterRead = null;
    const pendingTime = f.ledger().entries[0] && f.ledger().entries[0].acceptedAt;
    assert.equal((await f.submit()).status, "Submitted", String(n));
    if (pendingTime) assert.equal(f.ledger().entries[0].acceptedAt, pendingTime);
    assert.equal(f.ledger().entries.length, 1); assert.equal(f.account().onlineOperation, null);
  }
});
test("rejected receipt commit/release failures recover without projection writes", async () => {
  for (const mode of ["failAt", "loseAt"]) for (let n = 1; n <= 3; n++) {
    const f = await configured(); f.state[mode] = f.state.writes + n;
    assert.equal((await f.submit(request(1, -1))).status, "TransientFailure");
    f.state[mode] = 0;
    assert.equal((await f.submit(request(1, -1))).status, "Rejected");
    assert.equal(f.account().onlineOperation, null); assert.equal(f.scores.size, 0);
    assert.equal(f.ledger().entries.length, 1);
  }
});
test("missing caller binding and capacity guards never create authority or leave reservations", async () => {
  const f = await configured(); const before = f.state.writes;
  assert.equal((await f.submit(request(), { ...f.context, playerId: "unknown" })).status, "TransientFailure");
  assert.equal(f.state.writes, before);
  f.ledger().entries = Array.from({ length: 128 }, (_, n) => ({ id: id(n + 100), status: "Rejected" }));
  assert.equal((await f.submit()).reason, "LedgerCapacity");
  assert.ok(!f.account().onlineOperation); assert.equal(f.state.writes, before);
});
test("malformed request and invalid query keep safe endpoint response shapes", async () => {
  const f = await configured(); const submit = loadEndpoint("submit-record", f), get = loadEndpoint("query-records", f);
  assert.equal((await submit({ params: { request: "{" }, context: f.context })).reason, "InvalidRequest");
  assert.equal((await get({ params: { request: "{" }, context: f.context })).reason, "InvalidRequest");
  for (const change of [{ boardId: "invalid" }, { kind: "invalid" }, { limit: 0 }, { limit: 21 }])
    assert.equal((await invoke(get, { ...query(), ...change }, f.context)).reason, "InvalidQuery");
});
for (const code of [403, 404, 503, "private-response-placeholder"]) {
  test("query failure has safe phase/status only: " + (typeof code === "number" ? code : "invalid"), async () => {
    const f = await configured();
    f.leaderboard.getLeaderboardScores = async () => {
      const error = new Error("private-response-placeholder");
      error.response = { status: code, data: { token: "private-response-placeholder" } };
      throw error;
    };
    const result = await f.query(query("me"));
    assert.equal(result.status, "TransientFailure");
    assert.equal(result.reason, "ServiceUnavailable");
    assert.equal(result.queryPhase, "ReadLeaderboard");
    assert.equal(result.serviceStatus, typeof code === "number" ? code : 0);
    assert.equal(JSON.stringify(result).includes("private-response-placeholder"), false);
  });
}
for (const [kind, phase] of [["owner", "ReadOwnerMapping"], ["account", "ReadRowAccount"], ["number", "ReadNumberMapping"]]) {
  test("row mapping failure identifies " + phase + " without identities", async () => {
    const f = await configured(); await f.submit();
    const scores = f.leaderboard.getLeaderboardScores;
    let boardRead = false;
    f.leaderboard.getLeaderboardScores = async (...args) => { const result = await scores(...args); boardRead = true; return result; };
    const read = f.store.read;
    f.store.read = async (...args) => {
      if (boardRead && args[0] === kind) throw Object.assign(new Error("private-error-placeholder"), { reason: "StoredScopeMismatch" });
      return read(...args);
    };
    const result = await f.query(query("me"));
    assert.equal(result.queryPhase, phase); assert.equal(result.queryFault, "StoredScopeMismatch");
    assert.equal(result.serviceStatus, 0); assert.equal(result.status, "TransientFailure");
    assert.deepEqual(result.entries, []);
    for (const value of [f.accountId, f.account().leaderboardOwnerId, "private-error-placeholder"])
      assert.equal(JSON.stringify(result).includes(value), false);
  });
}
for (const reason of ["LedgerConflict", "private-error-placeholder"]) {
  test("legacy projection fault is safe: " + (reason === "LedgerConflict" ? reason : "unknown"), async () => {
    const f = await configured();
    f.scores.set(stage + "/legacy-X", { playerId: "legacy-X", score: 10, metadata: { acceptedAt: 1, submissionId: id(9) } });
    const run = createRecordQueryService(f.store, f.leaderboard, async () => {
      throw Object.assign(new Error("private-error-placeholder"), { reason });
    });
    const result = await invoke(run, query("me"), f.context);
    assert.equal(result.queryPhase, "ProvisionLegacyRow");
    assert.equal(result.queryFault, reason === "LedgerConflict" ? reason : "Unknown");
    assert.equal(JSON.stringify(result).includes("private-error-placeholder"), false);
    assert.equal(JSON.stringify(result).includes("legacy-X"), false);
  });
}
(async () => {
  let passed = 0;
  for (const { name, run } of tests) {
    try { await run(); passed++; console.log(`PASS ${name}`); }
    catch (error) { console.error(`FAIL ${name}: ${error.stack}`); process.exitCode = 1; }
  }
  console.log(`${passed}/${tests.length} passed`);
})();
