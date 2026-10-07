const assert = require("node:assert/strict");
const fs = require("node:fs"), path = require("node:path"), vm = require("node:vm");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore } = require("../CloudCode/account-store");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createPersonalBestSnapshotService } = require("../CloudCode/get-account-personal-bests");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferCompletionService } = require("../CloudCode/transfer-completion-service");
const stage = "fs-stage-stage-001-r1", infinite = "fs-infinite-v2";
const id = "11111111-1111-4111-8111-111111111111";
const tests = []; const test = (name, run) => tests.push({ name, run });
async function ready() {
  const f = fixture(); f.store = createAccountStore(f.context, f.save);
  f.provision = createAccountProvisioningService(f.store, createLegacyAccountStore(f.context, f.save, f.leaderboard));
  await f.provision.ensure(f.context);
  f.accountId = f.value("fs8-player-A").accountId;
  f.ledger = () => f.value(`fs8-ledger-${f.accountId}`);
  f.account = () => f.value(`fs8-account-${f.accountId}`);
  f.snapshot = context => createPersonalBestSnapshotService(f.store)(context || f.context);
  return f;
}
test("empty success is explicit and read-only", async () => {
  const f = await ready(), writes = f.state.writes;
  assert.deepEqual(await f.snapshot(), { status: "Success", publicPlayerNumber: f.account().publicPlayerNumber, personalBests: [] });
  assert.equal(f.state.writes, writes);
});
test("both boards are returned as one whole snapshot with no internal owner", async () => {
  const f = await ready();
  f.ledger().best = { [stage]: { score: 9007199254740991, id, acceptedAt: 1 }, [infinite]: { score: 2147483647, id, acceptedAt: 2 } };
  const before = clone(f.ledger()), writes = f.state.writes, result = await f.snapshot();
  assert.equal(result.status, "Success"); assert.equal(result.personalBests.length, 2);
  for (const row of result.personalBests) assert.deepEqual(Object.keys(row).sort(), ["boardId", "score", "submissionId"]);
  assert.deepEqual(f.ledger(), before); assert.equal(f.state.writes, writes);
});
test("v2 ledger snapshot reads preserved bests without a v1 entries array", async () => {
  const f = await ready();
  f.ledger().version = 2; delete f.ledger().entries; f.ledger().pending = null;
  f.ledger().receiptDirectory = { version: 1, buckets: [], cleanup: null };
  f.ledger().migration = { sourceVersion: 1, state: "Complete", cursor: 0 };
  f.ledger().best = { [stage]: { score: 1, id, acceptedAt: 1 } };
  assert.deepEqual((await f.snapshot()).personalBests, [{ boardId: stage, score: 1, submissionId: id }]);
});
test("wrong environment and unauthenticated context never yield a snapshot", async () => {
  const f = await ready();
  assert.equal((await f.snapshot({ ...f.context, environmentId: "other" })).status, "TransientFailure");
  assert.equal((await f.snapshot({ ...f.context, playerId: "" })).status, "TransientFailure");
});
test("unknown player cannot select another account by params", async () => {
  const f = await ready();
  assert.equal((await f.snapshot({ ...f.context, playerId: "X", accountId: f.accountId })).status, "TransientFailure");
});
test("TransferPending is not an empty snapshot", async () => {
  const f = await ready(); f.account().status = "TransferPending";
  assert.equal((await f.snapshot()).status, "TransientFailure");
});
test("missing ledger is failure, not empty", async () => {
  const f = await ready(); const read = f.store.read;
  f.store.read = (kind, key) => kind === "ledger" ? Promise.resolve(null) : read(kind, key);
  assert.equal((await f.snapshot()).status, "TransientFailure");
});
test("pending/reserved submission prevents partial best replacement", async () => {
  const f = await ready(); f.ledger().active = id;
  assert.equal((await f.snapshot()).status, "TransientFailure");
  f.ledger().active = ""; f.ledger().entries.push({ status: "Pending" });
  assert.equal((await f.snapshot()).status, "TransientFailure");
  f.ledger().entries = []; f.account().onlineOperation = { id };
  assert.equal((await f.snapshot()).status, "TransientFailure");
});
test("malformed board/score/id fails whole snapshot", async () => {
  const f = await ready();
  for (const best of [{ score: -1, id, acceptedAt: 1 }, { score: 2147483648, id, acceptedAt: 1 },
    { score: 1, id: "not-uuid", acceptedAt: 1 }, { score: 1, id, acceptedAt: 0 }]) {
    f.ledger().best = { [infinite]: best }; assert.equal((await f.snapshot()).status, "TransientFailure");
  }
  f.ledger().best = { unknown: { score: 1, id, acceptedAt: 1 } };
  assert.equal((await f.snapshot()).status, "TransientFailure");
});
test("connection change during reads prevents stale C response", async () => {
  const f = await ready(); const read = f.store.read;
  f.store.read = async (kind, key) => { const item = await read(kind, key);
    if (kind === "ledger") f.account().currentPlayerId = "other"; return item; };
  assert.equal((await f.snapshot()).status, "TransientFailure");
});
test("ledger token change during reads prevents mixed snapshot", async () => {
  const f = await ready(); const read = f.store.read; let reads = 0;
  f.store.read = async (kind, key) => { const item = await read(kind, key);
    if (kind === "ledger" && ++reads === 2) item.writeLock += "changed"; return item; };
  assert.equal((await f.snapshot()).status, "TransientFailure");
});
test("B reads C not its detached old account; A denied; no leaderboard rewrite", async () => {
  const f = await ready(), b = { ...f.context, playerId: "B" };
  await f.provision.ensure(b); const oldId = f.value("fs8-player-B").accountId;
  f.ledger().best[stage] = { score: 900, id, acceptedAt: 1 };
  f.value(`fs8-ledger-${oldId}`).best[infinite] = { score: 99, id, acceptedAt: 1 };
  const secret = async () => Buffer.alloc(32, 7).toString("base64");
  const credentials = await createTransferStartService(f.store, secret).start(f.context);
  assert.equal((await createTransferCompletionService(f.store, secret).complete(b, credentials.code, credentials.verificationValue)).status, "Success");
  const before = clone(f.value(`fs8-ledger-${oldId}`)), writes = f.state.writes;
  const result = await f.snapshot(b); assert.equal(result.status, "Success");
  assert.deepEqual(result.personalBests, [{ boardId: stage, score: 900, submissionId: id }]);
  assert.equal((await f.snapshot()).status, "TransientFailure");
  assert.deepEqual(f.value(`fs8-ledger-${oldId}`), before); assert.equal(f.state.writes, writes); assert.equal(f.scores.size, 0);
});
function endpoint(name, f) {
  const module = { exports: {} };
  const localRequire = dependency => {
    if (dependency === "./account-store") return { ...require("../CloudCode/account-store"), createAccountStore: () => f.store };
    if (dependency === "./legacy-account-store") return { createLegacyAccountStore: () => null };
    if (dependency === "./account-provisioning-service") return { createAccountProvisioningService: () => f.provision };
    return require(path.resolve(__dirname, "../CloudCode", dependency));
  };
  vm.runInNewContext(fs.readFileSync(path.resolve(__dirname, "../CloudCode", name + ".js"), "utf8"), { module, require: localRequire });
  return module.exports;
}
test("actual snapshot endpoint has empty input and derives only authenticated scope", async () => {
  const f = await ready(), run = endpoint("get-account-personal-bests", f);
  assert.deepEqual(Object.keys(run.params), []); assert.equal(run.bundling, true);
  const result = await run({ context: f.context, params: { accountId: "other", playerId: "other" } });
  assert.equal(result.status, "Success"); assert.equal(result.publicPlayerNumber, f.account().publicPlayerNumber);
});
test("actual status endpoint provisions first-auth missing binding then returns Active", async () => {
  const f = await ready(), run = endpoint("get-account-transfer-status", f), fresh = { ...f.context, playerId: "fresh" };
  assert.equal(await f.store.read("player", "fresh"), null);
  const result = await run({ context: fresh }); assert.equal(result.status, "Active"); assert.equal(result.transferStatus, "None");
  assert.ok(f.value("fs8-player-fresh"));
});
test("actual status endpoint does not provision over inactive source and returns committed terminal", async () => {
  const f = await ready(), b = { ...f.context, playerId: "B" }; await f.provision.ensure(b);
  const secret = async () => Buffer.alloc(32, 7).toString("base64");
  const credentials = await createTransferStartService(f.store, secret).start(f.context);
  await createTransferCompletionService(f.store, secret).complete(b, credentials.code, credentials.verificationValue);
  let provisions = 0; f.provision = { ensure: async () => { provisions++; throw Error("must not provision"); } };
  const result = await endpoint("get-account-transfer-status", f)({ context: f.context });
  assert.equal(result.status, "Inactive"); assert.equal(result.transferStatus, "Completed"); assert.equal(provisions, 0);
});
(async () => { for (const { name, run } of tests) { await run(); console.log(`PASS ${name}`); }
  console.log(`PASS account personal best snapshot: ${tests.length}/${tests.length}`); })().catch(error => { console.error(error); process.exitCode = 1; });
