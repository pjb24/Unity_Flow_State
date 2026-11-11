"use strict";
const test = require("node:test"), assert = require("node:assert/strict");
const { parseScope } = require("../Verification/production-scope.cjs");
const probe = require("../Verification/production-safe-log-probe.cjs");
const comparison = require("../Verification/compare-production-snapshots.cjs");
const { assemble } = require("../Verification/capture-production-snapshot.cjs");
const scope = { projectId: "11111111-1111-4111-8111-111111111111", environmentId: "22222222-2222-4222-8222-222222222222", environmentName: "production" };
const config = Object.entries({ ProjectId: scope.projectId, EnvironmentId: scope.environmentId, EnvironmentName: "production", AuthenticationProfile: "flow-state-production" })
  .map(([k, v]) => `public const string ${k} = "${v}";`).join("\n");
test("local scope fixes Production independently of Editor selector; rejects wrong project/profile/environment and malformed config", () => {
  const parse = (c = config, p = `cloudProjectId: ${scope.projectId}`, v = { environmentId: "verification-fixture" }) => parseScope(c, p, v);
  assert.deepEqual(parse(), scope);
  for (const action of [() => parse(config.replace('EnvironmentName = "production"', 'EnvironmentName = "verification"')),
    () => parse(config, "cloudProjectId: wrong"),
    () => parse(config.replace("flow-state-production", "flow-state-verification")),
    () => parse(config, `cloudProjectId: ${scope.projectId}`, scope),
    () => parse(config.replace(scope.projectId, "")), () => parse(config + '\npublic const string EnvironmentName = "production";')])
    assert.throws(action);
});
test("Production plan does not request network; explicit run requires confirmation and clears credentials", async () => {
  let calls = 0;
  const fetcher = () => { calls++; throw Error("must not fetch"); };
  await probe.main([], fetcher, () => {}, () => scope);
  process.env.FS_PRODUCTION_PROBE_KEY_ID = "private-id";
  process.env.FS_PRODUCTION_PROBE_KEY_SECRET = "private-secret";
  await assert.rejects(probe.main(["--run"], fetcher, () => {}, () => scope));
  await assert.rejects(probe.main(["--run", "--confirm-production"], fetcher, () => {}, () => { throw Error("PRODUCTION_SCOPE_MISMATCH"); }));
  assert.ok(probe.failureLine(Error("PRODUCTION_SCOPE_MISMATCH")).includes("Reason=PRODUCTION_SCOPE_MISMATCH"));
  assert.equal(process.env.FS_PRODUCTION_PROBE_KEY_ID, undefined);
  assert.equal(process.env.FS_PRODUCTION_PROBE_KEY_SECRET, undefined);
  assert.equal(calls, 0);
});
function fake(output, requests, failAt = -1) {
  return async (url, options) => {
    requests.push({ url, options });
    if (requests.length === failAt) throw Error("private-response-body-and-secret");
    return { ok: true, status: 200, json: async () => requests.length === 1 ? { accessToken: "private-token" } : { output } };
  };
}
test("Production sends only scoped token exchange and one fixed Module invocation", async () => {
  const requests = [];
  assert.equal(await probe.run(scope, "fake-id", "fake-secret", fake({ status: "Accepted" }, requests)), "Accepted");
  assert.equal(requests.length, 2);
  const url = new URL(requests[0].url);
  assert.equal(url.searchParams.get("environmentId"), scope.environmentId);
  assert.equal(url.searchParams.get("projectId"), scope.projectId);
  assert.equal(requests[1].url, `https://cloud-code.services.api.unity.com/v1/projects/${scope.projectId}/modules/FlowStateVerification/EmitVerificationSafeLogProbe`);
  assert.deepEqual(JSON.parse(requests[1].options.body), { params: {} });
  assert.equal(requests[1].options.headers.Authorization, "Bearer private-token");
  assert.ok(requests.every(r => r.options.redirect === "error" && r.options.signal instanceof AbortSignal));
});
test("Production errors and malformed statuses do not leak secrets or retry", async () => {
  for (const failAt of [1, 2]) {
    const requests = [];
    await assert.rejects(probe.run(scope, "fake-id", "fake-secret", fake({}, requests, failAt)), error => {
      assert.ok(!/private|fake-secret/.test(probe.failureLine(error))); return true;
    });
    assert.equal(requests.length, failAt);
  }
  const requests = [];
  await assert.rejects(probe.run(scope, "fake-id", "fake-secret", fake({ status: "private-body" }, requests)));
  assert.equal(requests.length, 2);
});
test("only Accepted Completed None 202 passes; RateLimited and unknown diagnostics fail safely", async () => {
  for (const output of [{ status: "Accepted", phase: "Completed", reason: "None", httpStatus: 202 },
    { status: "Accepted", phase: "Completed", reason: "None", httpStatus: 200 }, { status: "RateLimited" },
    { status: "Unavailable", phase: "private-url", reason: "private-secret", httpStatus: "private-body" }]) {
    process.env.FS_PRODUCTION_PROBE_KEY_ID = "fake-id"; process.env.FS_PRODUCTION_PROBE_KEY_SECRET = "fake-secret";
    const requests = [], lines = [];
    const result = await probe.main(["--run", "--confirm-production"], fake(output, requests), line => lines.push(line), () => scope);
    assert.equal(result, output.status === "Accepted" && output.httpStatus === 202);
    assert.equal(requests.length, 2);
    assert.ok(!lines.join().includes("private"));
  }
});
function save() {
  return { version: 7, accountId: "device", onlineProjectId: scope.projectId, onlineEnvironmentId: scope.environmentId,
    onlinePlayerId: "player", publicPlayerNumber: "0000000001", recoveryNoticeConfirmed: true,
    personalBests: [], pendingSubmissions: [{ playerId: "player", submissionId: "run", createdAtMilliseconds: 1 }],
    masterVolume: 100, isFullscreen: false, bindingOverrides: [], hasCompletedTutorial: true };
}
test("raw v7 save comparison preserves Pending payload/ID/time/owner, settings and tutorial", () => {
  const a = save();
  assert.equal(comparison.compare("restart", a, structuredClone(a), scope), "PRESERVED");
  assert.equal(comparison.compare("pending-restored", a, structuredClone(a), scope), "PRESERVED");
  for (const mutate of [b => b.pendingSubmissions[0].createdAtMilliseconds++, b => b.pendingSubmissions[0].submissionId = "other",
    b => b.pendingSubmissions[0].playerId = "other", b => b.masterVolume--, b => b.hasCompletedTutorial = false,
    b => b.bindingOverrides.push({ controlPath: "changed" })]) {
    const b = structuredClone(a); mutate(b); assert.notEqual(comparison.compare("restart", a, b, scope), "PRESERVED");
  }
  const b = structuredClone(a); b.pendingSubmissions = [];
  assert.equal(comparison.compare("pending-removed", a, b, scope), "REMOVED_ONLY");
  assert.equal(comparison.compare("pending-restored", b, b, scope), "PENDING_BASELINE_EMPTY");
  b.onlineEnvironmentId = "verification";
  assert.equal(comparison.compare("restart", b, b, scope), "SAVE_INPUT_INVALID");
});
function server() {
  return { ...scope, account: { ...scope, accountId: "account", currentPlayerId: "source", leaderboardOwnerId: "fixed",
    publicPlayerNumber: "0000000001", connectionRevision: 1, status: "Active" },
    rows: { "fs-stage-stage-001-r1": { playerId: "fixed", score: 50, metadata: { acceptedAt: 1, submissionId: "run" } }, "fs-infinite-v2": null } };
}
test("server restart and transfer compare account, fixed row, metadata and changed bindings", () => {
  const a = server(), b = structuredClone(a);
  assert.equal(comparison.compare("server-restart", a, b, scope), "PRESERVED");
  b.account.currentPlayerId = "recipient"; b.account.connectionRevision++;
  b.sourceBinding = { ...scope, playerId: "source", accountId: "account", status: "Inactive" };
  b.targetBinding = { ...scope, playerId: "recipient", accountId: "account", status: "Active", connectionRevision: 2 };
  assert.equal(comparison.compare("transfer", a, b, scope), "PRESERVED");
  for (const mutate of [c => c.rows["fs-stage-stage-001-r1"].metadata.acceptedAt++, c => c.rows["fs-stage-stage-001-r1"].score++,
    c => c.account.publicPlayerNumber = "0000000002", c => c.targetBinding.status = "Inactive",
    c => c.account.leaderboardOwnerId = "other", c => c.environmentId = "other"]) {
    const c = structuredClone(b); mutate(c); assert.notEqual(comparison.compare("transfer", a, c, scope), "PRESERVED");
  }
});
test("detached recipient comparison preserves its old account and rows without treating it as active", () => {
  const a = server(), b = structuredClone(a);
  b.account.status = "Detached"; b.account.currentPlayerId = null;
  b.account.connectionRevision++; b.account.detachedByTransferId = "synthetic-transfer";
  assert.equal(comparison.compare("recipient-detached", a, b, scope), "PRESERVED");
  assert.equal(comparison.compare("server-restart", a, b, scope), "SERVER_INPUT_INVALID");
  const manifest = { accountFile: "account.json", rowFiles: { "fs-stage-stage-001-r1": "stage.json", "fs-infinite-v2": null } };
  const read = p => p === "account.json" ? b.account : b.rows["fs-stage-stage-001-r1"];
  assert.deepEqual(assemble("server-detached", manifest, scope, read), b);
  assert.throws(() => assemble("server", manifest, scope, read));
  for (const mutate of [c => c.account.status = "Active", c => c.account.currentPlayerId = "other",
    c => delete c.account.detachedByTransferId, c => c.account.connectionRevision++,
    c => c.account.accountId = "other", c => c.account.publicPlayerNumber = "0000000002",
    c => c.account.leaderboardOwnerId = "other", c => c.environmentId = "other",
    c => c.rows["fs-stage-stage-001-r1"].metadata.acceptedAt++,
    c => c.rows["fs-stage-stage-001-r1"].metadata.submissionId = "other",
    c => c.rows["fs-stage-stage-001-r1"].score++, c => c.rows["fs-stage-stage-001-r1"] = null]) {
    const c = structuredClone(b); mutate(c);
    assert.notEqual(comparison.compare("recipient-detached", a, c, scope), "PRESERVED");
  }
  const lines = [];
  assert.equal(comparison.main(["recipient-detached", "before", "after"], x => lines.push(x),
    () => scope, p => JSON.stringify(p === "before" ? a : b)), true);
  assert.deepEqual(lines, ["PASS / PRODUCTION_COMPARE / PRESERVED"]);
});
test("comparison CLI strips BOM, hides file paths/private JSON/invalid input and rejects invalid mode", () => {
  const lines = [], a = save();
  assert.equal(comparison.main(["restart", "private-path-a", "private-path-b"], x => lines.push(x), () => scope, () => '\uFEFF' + JSON.stringify(a)), true);
  assert.equal(comparison.main(["restart", "private-path-a", "private-path-b"], x => lines.push(x), () => scope, () => { throw Error("private-error"); }), false);
  assert.ok(!lines.join().includes("private"));
});
test("offline capture assembles existing raw exports and refuses missing rows or wrong scope", () => {
  const s = server();
  const manifest = { accountFile: "account.json", rowFiles: { "fs-stage-stage-001-r1": "stage.json", "fs-infinite-v2": null } };
  const read = p => p === "account.json" ? { value: s.account } : s.rows["fs-stage-stage-001-r1"];
  assert.deepEqual(assemble("server", manifest, scope, read).rows, s.rows);
  assert.deepEqual(assemble("save", save(), scope, () => { throw Error(); }), save());
  assert.throws(() => assemble("server", { ...manifest, rowFiles: {} }, scope, read));
  assert.throws(() => assemble("save", { ...save(), onlineEnvironmentId: "verification" }, scope, read));
});
