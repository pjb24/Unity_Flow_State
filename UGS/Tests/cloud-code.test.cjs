// Local SDK doubles only. No credentials, network, Unity Editor or Test Runner.
const tests = [];
const test = (name, run) => tests.push({ name, run });
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");
const vm = require("vm");
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
function fixture() {
  const state = { ledger: { version: 1, active: "", entries: [], best: {} }, lock: 1,
    scores: [], writes: 0, failProjectionOnce: false, failFinalOnce: false, missing: false };
  class DataApi {
    constructor(context) { assert.ok(context.serviceToken); }
    async getProtectedItems(project, player, keys) {
      assert.equal(project, projectId); assert.equal(player, "test-player");
      return { data: { results: state.missing ? [] : [{ key: keys[0], value: clone(state.ledger), writeLock: String(state.lock) }] } };
    }
    async setProtectedItem(project, player, item) {
      assert.equal(item.writeLock, String(state.lock));
      if (state.failFinalOnce && item.value.entries.some(e => e.status === "Submitted")) {
        state.failFinalOnce = false; throw Error("connection lost");
      }
      state.lock++; state.ledger = clone(item.value);
    }
  }
  class LeaderboardsApi {
    constructor(context) { assert.ok(context.serviceToken); }
    async addLeaderboardPlayerScore(project, board, player, value) {
      state.writes++;
      const old = state.scores.find(e => e.playerId === player && e.board === board);
      if (!old || (board === stage ? value.score < old.score : value.score > old.score)) {
        if (old) state.scores.splice(state.scores.indexOf(old), 1);
        state.scores.push({ board, playerId: player, ...clone(value) });
      }
      if (state.failProjectionOnce) { state.failProjectionOnce = false; throw Error("response lost after commit"); }
    }
    async getLeaderboardScores(project, board, offset, limit, options) {
      assert.equal(offset, 0); assert.equal(limit, 100); assert.equal(options.params.includeMetadata, true);
      const scores = state.scores.filter(e => e.board === board);
      return { data: { total: scores.length, results: clone(scores.slice(0, 100)) } };
    }
  }
  function load(file) {
    const sandbox = { module: { exports: {} }, require: name => {
      if (name === "@unity-services/cloud-save-1.4") return { DataApi };
      if (name === "@unity-services/leaderboards-1.1") return { LeaderboardsApi };
      throw Error("Unexpected dependency");
    }};
    vm.runInNewContext(fs.readFileSync(path.join(__dirname, "../CloudCode", file), "utf8"), sandbox);
    return sandbox.module.exports;
  }
  const context = { projectId, environmentId, playerId: "test-player", serviceToken: "local-double" };
  const submit = load("submit-record.js"); const query = load("query-records.js");
  return { state, context, submit: (p, ctx = context) => submit({ params: { request: JSON.stringify(p) }, context: ctx }),
    query: (p, ctx = context) => query({ params: { request: JSON.stringify(p) }, context: ctx }) };
}
test("first submit, duplicate and changed payload", async () => {
  const f = fixture();
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.writes, 1);
  assert.equal((await f.submit(request(1, { score: 999 }))).reason, "SubmissionIdConflict");
  assert.equal(f.state.writes, 1);
});
test("same/worse score never overwrites best timestamp", async () => {
  const f = fixture(); await f.submit(request()); const time = f.state.scores[0].metadata.acceptedAt;
  await f.submit(request(2)); await f.submit(request(3, { score: 1100 }));
  assert.equal(f.state.writes, 1); assert.equal(f.state.scores[0].metadata.acceptedAt, time);
  await f.submit(request(4, { score: 900 })); assert.equal(f.state.writes, 2);
});
test("lost projection response retains reservation then recovers", async () => {
  const f = fixture(); f.state.failProjectionOnce = true;
  assert.equal((await f.submit(request())).status, "TransientFailure");
  const time = f.state.ledger.entries[0].acceptedAt;
  assert.equal((await f.submit(request(2))).reason, "EarlierSubmissionPending");
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.scores[0].metadata.acceptedAt, time); assert.equal(f.state.ledger.active, "");
});
test("final journal write failure safely replays", async () => {
  const f = fixture(); f.state.failFinalOnce = true;
  assert.equal((await f.submit(request())).status, "TransientFailure");
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.scores.length, 1);
});
test("CAS concurrent different IDs never overwrite the journal", async () => {
  const f = fixture(); const results = await Promise.all([f.submit(request()), f.submit(request(2))]);
  assert.equal(results.filter(r => r.status === "Submitted").length, 1);
  assert.equal(f.state.ledger.entries.length, 1);
  assert.equal((await f.submit(request(2))).status, "Submitted");
});
test("concurrent identical requests converge", async () => {
  const f = fixture(); await Promise.all([f.submit(request()), f.submit(request())]);
  assert.equal((await f.submit(request())).status, "Submitted");
  assert.equal(f.state.ledger.entries.length, 1);
});
test("missing ledger fails closed without leaderboard writes", async () => {
  const f = fixture(); f.state.missing = true;
  assert.equal((await f.submit(request())).reason, "LedgerNotProvisioned"); assert.equal(f.state.writes, 0);
});
test("environment/project/auth guards", async () => {
  const f = fixture();
  for (const change of [{ environmentId: "production" }, { projectId: "wrong" }, { playerId: "" }])
    assert.equal((await f.submit(request(), { ...f.context, ...change })).reason, "ContextMismatch");
  assert.equal(f.state.writes, 0);
});
for (const [name, changes] of Object.entries({ negative: { score: -1 }, fraction: { score: 1.5 },
  unsafe: { score: Number.MAX_SAFE_INTEGER + 1 }, version: { rulesVersion: 2 },
  board: { boardId: "other" }, uuid: { submissionId: "invalid" }, stage: { totalScore: 20 } })) {
  test(`reject ${name}`, async () => {
    const f = fixture(); assert.equal((await f.submit(request(1, changes))).status, "Rejected");
    assert.equal(f.state.writes, 0);
  });
}
test("rejected payload has stable persistent result", async () => {
  const f = fixture(); const p = request(1, { score: -1 });
  assert.equal((await f.submit(p)).status, "Rejected");
  const lock = f.state.lock;
  assert.equal((await f.submit(p)).status, "Rejected"); assert.equal(f.state.lock, lock);
});
test("infinite exact maximum and over-limit, sum, multiplier, missing fields", async () => {
  const base = request(1, { boardId: infinite, rulesVersion: 2, runDurationMilliseconds: 1000,
    score: 440, baseDistanceScore: 80, momentumBonus: 160, distanceScore: 240,
    collectibleScore: 200, totalScore: 440, maximumMomentumMultiplier: 3 });
  assert.equal((await fixture().submit(base)).status, "Submitted");
  for (const change of [{ totalScore: 441, score: 441, collectibleScore: 201 }, { distanceScore: 239 },
    { maximumMomentumMultiplier: 3.1 }, { runDurationMilliseconds: undefined }, { totalScore: -1 }])
    assert.equal((await fixture().submit({ ...base, ...change })).status, "Rejected");
});
test("ledger capacity retains old receipts without eviction", async () => {
  const f = fixture(); f.state.ledger.entries = Array.from({ length: 128 }, (_, i) => ({ id: id(i + 10) }));
  assert.equal((await f.submit(request())).reason, "LedgerCapacity"); assert.equal(f.state.writes, 0);
});
test("rank uses score then timestamp and shared competition rank", async () => {
  const f = fixture();
  f.state.scores = [["a",100,10],["b",100,10],["test-player",100,20],["d",200,5]].map(
    ([playerId,score,acceptedAt]) => ({ board:stage, playerId,score,metadata:{acceptedAt} }));
  const top = await f.query({boardId:stage,kind:"top",limit:20});
  assert.equal(top.status,"Success"); assert.equal(JSON.stringify(top.entries.map(e=>e.rank)),"[1,1,1,4]");
  const me = await f.query({boardId:stage,kind:"me",limit:1}); assert.equal(me.entries[0].rank,1);
  const around = await f.query({boardId:stage,kind:"around",limit:3}); assert.equal(around.entries.length,3);
});
test("empty boards succeed; unknown metadata and partial boards fail closed", async () => {
  const f = fixture(); const p = {boardId:stage,kind:"top",limit:20};
  assert.equal((await f.query(p)).status,"Success");
  f.state.scores = [{board:stage,playerId:"a",score:10}];
  assert.equal((await f.query(p)).reason,"MissingServerMetadata");
  f.state.scores = Array.from({length:101}, (_,i)=>({board:stage,playerId:String(i),score:i,metadata:{acceptedAt:1}}));
  assert.equal((await f.query(p)).reason,"VerificationBoardCapacity");
});
test("query guards and absent own score", async () => {
  const f = fixture();
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
