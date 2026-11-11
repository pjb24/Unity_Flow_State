// Local SDK doubles only. No credentials, network, Unity Editor or Test Runner.
const tests = [];
const test = (name, run) => tests.push({ name, run });
const assert = require("assert").strict;
const projectId = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const stage = "fs-stage-stage-001-r1";
const infinite = "fs-infinite-v2";
const clone = value => JSON.parse(JSON.stringify(value));
const id = n => `00000000-0000-4000-8000-${String(n).padStart(12, "0")}`;
function request(n = 1, overrides = {}) {
  return { boardId: stage, rulesVersion: 1, submissionId: id(n), score: 1000,
    runDurationMilliseconds: 0, baseDistanceScore: 0, momentumBonus: 0,
    distanceScore: 0, collectibleScore: 0, totalScore: 0, maximumMomentumMultiplier: 1, ...overrides };
}
async function fixture(useReceiptStore = false) {
  const f = require("./account-sdk-double.cjs").fixture();
  f.context = { ...f.context, playerId: "test-player" };
  const { createAccountStore } = require("../CloudCode/account-store");
  const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
  const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
  const store = createAccountStore(f.context, f.save);
  const provision = createAccountProvisioningService(store, createLegacyAccountStore(f.context, f.save, f.leaderboard));
  await provision.ensure(f.context);
  const accountId = f.value("fs8-player-test-player").accountId;
  const state = { scores: [], writes: 0, failProjectionOnce: false, failFinalOnce: false, missing: false };
  Object.defineProperty(state, "ledger", { get: () => f.value(`fs8-ledger-${accountId}`) });
  Object.defineProperty(state, "lock", { get: () => f.state.serial });
  const originalRead = store.read.bind(store);
  store.read = async (kind, id) => state.missing && kind === "ledger" ? null : originalRead(kind, id);
  f.state.beforeWrite = async op => {
    if (state.failFinalOnce && op.data.some(e => e.value.entries && e.value.entries.some(r => r.status === "Submitted"))) {
      state.failFinalOnce = false; throw Error("connection lost");
    }
  };
  const boards = {
    async addLeaderboardPlayerScore(project, board, player, value) {
      state.writes++;
      const previous = state.scores.find(e => e.playerId === player && e.board === board);
      if (!previous || (board === stage ? value.score < previous.score : value.score > previous.score)) {
        if (previous) state.scores.splice(state.scores.indexOf(previous), 1);
        state.scores.push({ board, playerId: player, ...clone(value) });
      }
      if (state.failProjectionOnce) { state.failProjectionOnce = false; throw Error("response lost after commit"); }
    },
    async getLeaderboardScores(project, board, offset, limit, options) {
      assert.equal(offset, 0); assert.equal(limit, 10); assert.equal(options.params.includeMetadata, true);
      syncScores();
      return f.leaderboard.getLeaderboardScores(project, board, offset, limit);
    },
    async getLeaderboardPlayerScore(...args) {
      syncScores(); return f.leaderboard.getLeaderboardPlayerScore(...args);
    },
    async getLeaderboardPlayerRange(...args) {
      syncScores(); return f.leaderboard.getLeaderboardPlayerRange(...args);
    }
  };
  function syncScores() {
    f.scores.clear();
    for (const row of state.scores) f.scores.set(`${row.board}/${row.playerId}`, clone(row));
  }
  const receipts = useReceiptStore ? require("../CloudCode/receipt-store").createReceiptStore(f.context, f.save) : null;
  const submit = require("../CloudCode/submit-record").createRecordSubmissionService(store, boards, Date.now, receipts);
  const query = require("../CloudCode/query-records").createRecordQueryService(store, boards, owner => {
    const row = state.scores.find(e => e.playerId === owner);
    if (row) f.scores.set(`${row.board}/${owner}`, clone(row));
    return provision.ensure({ ...f.context, playerId: owner });
  });
  return { ...f, store, accountId, ownerId: f.value(`fs8-account-${accountId}`).leaderboardOwnerId, state,
    submit: (p, context = f.context) => submit({ params: { request: JSON.stringify(p) }, context }),
    query: (p, context = f.context) => query({ params: { request: JSON.stringify(p) }, context }) };
}

test("first submit, duplicate and changed payload", async () => {
  const f = await fixture();
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.writes, 1);
  assert.equal((await f.submit(request(1, { score: 999 }))).reason, "SubmissionIdConflict");
  assert.equal(f.state.writes, 1);
});
test("same/worse score never overwrites best timestamp", async () => {
  const f = await fixture(); await f.submit(request()); const time = f.state.scores[0].metadata.acceptedAt;
  await f.submit(request(2)); await f.submit(request(3, { score: 1100 }));
  assert.equal(f.state.writes, 1); assert.equal(f.state.scores[0].metadata.acceptedAt, time);
  await f.submit(request(4, { score: 900 })); assert.equal(f.state.writes, 2);
});
test("lost projection response retains reservation then recovers", async () => {
  const f = await fixture(); f.state.failProjectionOnce = true;
  assert.equal((await f.submit(request())).status, "TransientFailure");
  const time = f.state.ledger.entries[0].acceptedAt;
  assert.equal((await f.submit(request(2))).reason, "EarlierSubmissionPending");
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.scores[0].metadata.acceptedAt, time); assert.equal(f.state.ledger.active, "");
});
test("final journal write failure safely replays", async () => {
  const f = await fixture(); f.state.failFinalOnce = true;
  assert.equal((await f.submit(request())).status, "TransientFailure");
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.scores.length, 1);
});
test("CAS concurrent different IDs never overwrite the journal", async () => {
  const f = await fixture(); const results = await Promise.all([f.submit(request()), f.submit(request(2))]);
  assert.equal(results.filter(r => r.status === "Submitted").length, 1);
  assert.equal(f.state.ledger.entries.length, 1);
  assert.equal((await f.submit(request(2))).status, "Submitted");
});
test("concurrent identical requests converge", async () => {
  const f = await fixture(); await Promise.all([f.submit(request()), f.submit(request())]);
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.ledger.entries.length, 1);
});
test("missing ledger fails closed without leaderboard writes", async () => {
  const f = await fixture(); f.state.missing = true;
  assert.equal((await f.submit(request())).reason, "LedgerNotProvisioned"); assert.equal(f.state.writes, 0);
});
test("C submissions leave the frozen legacy source unchanged", async () => {
  const f = await fixture(); await f.submit(request());
  const source = clone([...f.protectedItems]);
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal((await f.submit(request(2))).status, "Submitted");
  assert.deepEqual([...f.protectedItems], source);
});
test("environment/project/auth guards", async () => {
  const f = await fixture();
  for (const change of [{ environmentId: "production" }, { projectId: "wrong" }, { playerId: "" }])
    assert.equal((await f.submit(request(), { ...f.context, ...change })).reason, "ContextMismatch");
  assert.equal(f.state.writes, 0);
});
for (const [name, changes] of Object.entries({ negative: { score: -1 }, fraction: { score: 1.5 },
  unsafe: { score: Number.MAX_SAFE_INTEGER + 1 }, version: { rulesVersion: 2 },
  board: { boardId: "other" }, uuid: { submissionId: "invalid" }, stage: { totalScore: 20 } })) {
  test(`reject ${name}`, async () => {
    const f = await fixture(); assert.equal((await f.submit(request(1, changes))).status, "Rejected");
    assert.equal(f.state.writes, 0);
  });
}
test("rejected payload has stable persistent result", async () => {
  const f = await fixture(); const p = request(1, { score: -1 });
  assert.equal((await f.submit(p)).status, "Rejected");
  const lock = f.state.lock;
  assert.equal((await f.submit(p)).status, "Rejected"); assert.equal(f.state.lock, lock);
});
test("infinite exact maximum and over-limit, sum, multiplier, missing fields", async () => {
  const base = request(1, { boardId: infinite, rulesVersion: 2, runDurationMilliseconds: 1000,
    score: 440, baseDistanceScore: 80, momentumBonus: 160, distanceScore: 240,
    collectibleScore: 200, totalScore: 440, maximumMomentumMultiplier: 3 });
  assert.equal((await (await fixture()).submit(base)).status, "Submitted");
  for (const change of [{ totalScore: 441, score: 441, collectibleScore: 201 }, { distanceScore: 239 },
    { maximumMomentumMultiplier: 3.1 }, { runDurationMilliseconds: undefined }, { totalScore: -1 }])
    assert.equal((await (await fixture()).submit({ ...base, ...change })).status, "Rejected");
});
// Infinite-only fixtures exercise the production service without a Stage run.
function infiniteRequest(n = 1, score = 1000) {
  return request(n, { boardId: infinite, rulesVersion: 2, score,
    runDurationMilliseconds: 60000, baseDistanceScore: score, momentumBonus: 0,
    distanceScore: score, collectibleScore: 0, totalScore: score, maximumMomentumMultiplier: 1 });
}

test("Infinite submission is shared by top, around, me and whole-account snapshot", async () => {
  const f = await fixture();
  assert.equal((await f.submit(infiniteRequest())).status, "Submitted");
  for (const [kind, limit] of [["top", 10], ["around", 7], ["me", 1]]) {
    const result = await f.query({ boardId: infinite, kind, limit });
    assert.equal(result.status, "Success"); assert.equal(result.entries.length, 1);
    assert.equal(result.entries[0].score, 1000); assert.equal(result.entries[0].rank, 1);
    assert.equal(result.entries[0].isMe, true);
  }
  const snapshot = await require("../CloudCode/get-account-personal-bests")
    .createPersonalBestSnapshotService(f.store)(f.context);
  assert.equal(snapshot.status, "Success");
  assert.deepEqual(snapshot.personalBests, [{ boardId: infinite, score: 1000, submissionId: id(1) }]);
  assert.equal(f.state.scores.length, 1); assert.equal(f.state.scores[0].board, infinite);
});

test("Infinite v2 receipt submission replays without another projection and remains queryable", async () => {
  const f = await fixture(true);
  assert.equal((await f.submit(infiniteRequest())).status, "Submitted");
  assert.equal(f.state.ledger.version, 2); assert.equal(f.state.ledger.pending, null);
  assert.equal((await f.submit(infiniteRequest())).status, "Submitted");
  assert.equal((await f.submit(infiniteRequest(1, 1100))).reason, "SubmissionIdConflict");
  assert.equal(f.state.writes, 1); assert.equal(f.state.scores.length, 1);
  const me = await f.query({ boardId: infinite, kind: "me", limit: 1 });
  assert.equal(me.status, "Success"); assert.equal(me.entries[0].score, 1000);
  const snapshot = await require("../CloudCode/get-account-personal-bests")
    .createPersonalBestSnapshotService(f.store)(f.context);
  assert.equal(snapshot.status, "Success");
  assert.deepEqual(snapshot.personalBests, [{ boardId: infinite, score: 1000, submissionId: id(1) }]);
});

test("Infinite duplicate and changed payload keep one projection and original receipt", async () => {
  const f = await fixture(), original = infiniteRequest();
  assert.equal((await f.submit(original)).status, "Submitted");
  const before = clone(f.state.ledger);
  assert.equal((await f.submit(original)).status, "Submitted");
  assert.equal((await f.submit(infiniteRequest(1, 1100))).reason, "SubmissionIdConflict");
  assert.equal(f.state.writes, 1); assert.deepEqual(f.state.ledger, before);
});

test("Infinite worse score cannot replace best and higher score uses the same fixed owner", async () => {
  const f = await fixture();
  assert.equal((await f.submit(infiniteRequest(1, 1000))).status, "Submitted");
  assert.equal((await f.submit(infiniteRequest(2, 900))).status, "Submitted");
  assert.equal(f.state.writes, 1); assert.equal(f.state.ledger.best[infinite].score, 1000);
  assert.equal((await f.submit(infiniteRequest(3, 1100))).status, "Submitted");
  assert.equal(f.state.writes, 2); assert.equal(f.state.scores.length, 1);
  assert.equal(f.state.scores[0].playerId, f.ownerId);
  assert.equal(f.state.ledger.best[infinite].score, 1100);
});

test("Infinite lost projection response recovers the same ID without losing accepted time", async () => {
  const f = await fixture(); f.state.failProjectionOnce = true;
  assert.equal((await f.submit(infiniteRequest())).status, "TransientFailure");
  const acceptedAt = f.state.ledger.entries[0].acceptedAt;
  assert.equal((await f.submit(infiniteRequest(2))).reason, "EarlierSubmissionPending");
  assert.equal((await f.submit(infiniteRequest())).status, "Submitted");
  assert.equal(f.state.scores.length, 1);
  assert.equal(f.state.scores[0].metadata.acceptedAt, acceptedAt);
  assert.equal(f.state.ledger.entries.length, 1); assert.equal(f.state.ledger.active, "");
});

test("Infinite concurrent same-ID submissions converge to one terminal entry", async () => {
  const f = await fixture();
  await Promise.all([f.submit(infiniteRequest()), f.submit(infiniteRequest())]);
  assert.equal((await f.submit(infiniteRequest())).status, "Submitted");
  assert.equal(f.state.ledger.entries.length, 1); assert.equal(f.state.scores.length, 1);
  assert.equal(f.state.ledger.entries[0].status, "Submitted");
});

test("ledger capacity retains old receipts without eviction", async () => {
  const f = await fixture(); f.state.ledger.entries = Array.from({ length: 128 }, (_, i) => ({ id: id(i + 10) }));
  assert.equal((await f.submit(request())).reason, "LedgerCapacity"); assert.equal(f.state.writes, 0);
});
test("rank uses score then timestamp and shared competition rank", async () => {
  const f = await fixture();
  f.state.scores = [["a",100,10],["b",100,10],[f.ownerId,100,20],["d",200,5]].map(
    ([playerId,score,acceptedAt]) => ({ board:stage, playerId,score,metadata:{acceptedAt,submissionId:id(100)} }));
  const top = await f.query({boardId:stage,kind:"top",limit:10});
  assert.equal(top.status,"Success"); assert.equal(JSON.stringify(top.entries.map(e=>e.rank)),"[1,1,1,4]");
  const me = await f.query({boardId:stage,kind:"me",limit:1}); assert.equal(me.entries[0].rank,1);
  const around = await f.query({boardId:stage,kind:"around",limit:3}); assert.equal(around.entries.length,3);
});
test("empty boards succeed; unknown metadata and partial boards fail closed", async () => {
  const f = await fixture(); const p = {boardId:stage,kind:"top",limit:10};
  assert.equal((await f.query(p)).status,"Success");
  f.state.scores = [{board:stage,playerId:"a",score:10}];
  assert.equal((await f.query(p)).reason,"MissingServerMetadata");
  f.state.scores = Array.from({length:101}, (_,i)=>({board:stage,playerId:String(i),score:i,metadata:{acceptedAt:1}}));
  const missingLegacyMetadata = await f.query(p);
  assert.equal(missingLegacyMetadata.reason,"ServiceUnavailable");
  assert.equal(missingLegacyMetadata.queryFault,"MissingServerMetadata");
  assert.deepEqual(missingLegacyMetadata.entries,[]);
});
test("query guards and absent own score", async () => {
  const f = await fixture();
  assert.equal((await f.query({boardId:stage,kind:"me",limit:1})).entries.length,0);
  assert.equal((await f.query({boardId:stage,kind:"top",limit:21})).reason,"InvalidQuery");
  assert.equal((await f.query({boardId:stage,kind:"top",limit:1},{...f.context,environmentId:"wrong"})).reason,"ContextMismatch");
});

(async () => {
  let failed = 0;
  for (const entry of tests) {
    try { await entry.run(); console.log(`PASS ${entry.name}`); }
    catch (error) { failed++; console.error(`FAIL ${entry.name}: ${error.stack}`); }
  }
  console.log(`${tests.length - failed}/${tests.length} passed`);
  process.exitCode = failed ? 1 : 0;
})();
