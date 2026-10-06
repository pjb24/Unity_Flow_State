"use strict";
const { test } = require("node:test");
const assert = require("node:assert/strict");
const { adapter, exchange, run, main, failureLine } = require("../Verification/verify-live-transfer-storage.cjs");
const { projectId, environmentId } = require("../CloudCode/account-store");
const runId = "0123456789abcdef0123456789abcdef";
function network() {
  const entities = new Map(), calls = []; let serial = 0;
  return { entities, calls, fetch: async (url, options) => {
    calls.push({ url, options });
    const match = new URL(url).pathname.match(/\/custom\/(p2v-[a-f0-9]{32}-[a-f0-9]{12})\/private\/(items|item-batch)$/);
    assert.ok(match, "Probe must only address its isolated Private Custom namespace");
    assert.equal(new URL(url).hostname, "cloud-save.services.api.unity.com");
    assert.ok(url.includes(projectId)); assert.equal(options.redirect, "error");
    const id = match[1];
    if (!entities.has(id)) entities.set(id, new Map());
    const data = entities.get(id);
    if (options.method === "GET") {
      const keys = new URL(url).searchParams.getAll("keys");
      return { ok: true, json: async () => ({ results: structuredClone([...data.values()].filter(v => !keys.length || keys.includes(v.key))) }) };
    }
    assert.equal(options.method, "POST");
    const body = JSON.parse(options.body), items = body.data || [body];
    if (items.some(item => item.writeLock && data.get(item.key)?.writeLock !== item.writeLock))
      return { ok: false, status: 409 };
    for (const item of items) data.set(item.key, { key: item.key, value: item.value, writeLock: String(++serial) });
    return { ok: true, json: async () => ({ results: [] }) };
  } };
}
test("plan and invalid flags never make a network call", async () => {
  await main(["--plan"]);
  await assert.rejects(main(["--run"]), { code: "EXPLICIT_WRITE_CONFIRMATION_REQUIRED" });
  await assert.rejects(main(["--run", "--confirm-verification-writes", "production"]), { code: "EXPLICIT_WRITE_CONFIRMATION_REQUIRED" });
});
test("token exchange pins both verification identifiers and suppresses raw failures", async () => {
  let called = false;
  assert.equal(await exchange("key-id", "private-key", async (url, options) => {
    called = true; const parsed = new URL(url);
    assert.equal(parsed.hostname, "services.api.unity.com");
    assert.equal(parsed.searchParams.get("projectId"), projectId);
    assert.equal(parsed.searchParams.get("environmentId"), environmentId);
    assert.equal(options.redirect, "error");
    return { ok: true, json: async () => ({ accessToken: "private-token" }) };
  }), "private-token");
  assert.ok(called);
  await assert.rejects(exchange("key-id", "private-key", async () => { throw Error("private-key"); }),
    error => error.code === "REQUEST_UNCONFIRMED" && !error.message.includes("private-key"));
});
test("adapter refuses project/namespace overrides and pre-existing namespaces", async () => {
  const n = network(), live = adapter("token", runId, "safety", n.fetch);
  await assert.rejects(live.api.getPrivateCustomItems("other-project", "fs8-player-A", []), { code: "SCOPE_DENIED" });
  assert.equal(n.calls.length, 0);
  assert.throws(() => adapter("token", "../", "safety", n.fetch), { code: "INVALID_RUN_SCOPE" });
  const mapped = live.target(projectId, "fs8-player-A");
  n.entities.set(mapped, new Map([["existing", { key: "existing", value: 1 }]]));
  await assert.rejects(live.api.setPrivateCustomItem(projectId, "fs8-player-A", { key: "x", value: 2 }), { code: "NAMESPACE_ALREADY_EXISTS" });
  assert.equal(n.calls.length, 1); assert.equal(n.entities.get(mapped).size, 1);
});
test("stale lock batch rejects every member with HTTP409, never retries", async () => {
  const n = network(), live = adapter("token", runId, "cas", n.fetch);
  await live.api.setPrivateCustomItem(projectId, "fs8-account-probe", { key: "guard", value: 1 });
  const before = n.calls.length;
  await assert.rejects(live.api.setPrivateCustomItemBatch(projectId, "fs8-account-probe", { data: [
    { key: "guard", value: 2, writeLock: "stale" }, { key: "target", value: "overwrite" }
  ] }), error => error.response.status === 409);
  assert.equal(n.calls.length, before + 1);
  assert.equal(n.entities.get(live.target(projectId, "fs8-account-probe")).has("target"), false);
});
test("every completion write before/after fault and two-target race runs over production services locally", async () => {
  const n = network(), output = []; let now = 100000;
  await run("synthetic-service-token", runId, value => output.push(value), n.fetch,
    { clock: () => now, wait: async ms => { assert.equal(ms, 5100); now += ms; } });
  assert.ok(output.some(line => line.includes("STORAGE_SINGLE_WINNER_RACE")));
  assert.ok(output.some(line => line.includes("STORAGE_SUBMISSION_TRANSFER_FENCE")));
  assert.ok(output.some(line => line.includes("LIVE_STORAGE_PROBE")));
  const before = output.filter(line => line.includes("STORAGE_RECOVERY_BEFORE"));
  const after = output.filter(line => line.includes("STORAGE_RECOVERY_AFTER"));
  assert.ok(before.length >= 6); assert.equal(before.length, after.length);
  assert.ok(output.every(line => !/synthetic-service-token|probeA|probeB|probeC/.test(line)));
  assert.ok(n.calls.every(call => !/leaderboards|\/players\//.test(call.url)));
  console.log(`LOCAL_ONLY / ProbeFaultCases=${before.length + after.length} / NoRemoteRequests`);
});
test("remaining selection only executes after10 and two races, never declares full run pass", async () => {
  const n = network(), output = []; let now = 100000;
  await run("token", runId, line => output.push(line), n.fetch,
    { selection: "remaining", clock: () => now, wait: async ms => { now += ms; } });
  assert.deepEqual(output.filter(line => line.includes("STORAGE_RECOVERY")), ["PASS / STORAGE_RECOVERY_AFTER_10"]);
  assert.equal(output.filter(line => line.includes("STORAGE_SINGLE_WINNER_RACE")).length, 1);
  assert.equal(output.filter(line => line.includes("STORAGE_SUBMISSION_TRANSFER_FENCE")).length, 1);
  assert.ok(output.at(-1).includes("LIVE_STORAGE_REMAINDER"));
  assert.ok(output.at(-1).includes("FullRun=False"));
  assert.ok(!output.some(line => line.includes("BASELINE_STORAGE_TRANSFER") || line.includes("LIVE_BATCH_ATOMICITY")));
});
test("setup timeout reports precise safe case, phase and operation without automatic retry", async () => {
  let calls = 0;
  await assert.rejects(run("hidden-token", runId, () => {}, async () => {
    calls++; throw Object.assign(Error("hidden-token and remote body"), { name: "TimeoutError" });
  }, { selection: "remaining" }), error => {
    const line = failureLine(error);
    assert.ok(line.includes("Case=after-10 / Phase=SETUP / Operation=NAMESPACE_READ / RequestOrdinal=1"));
    assert.ok(line.includes("HTTP=0") && line.includes("NetworkFault=TIMEOUT"));
    assert.ok(!line.includes("hidden-token") && !line.includes("remote body"));
    return true;
  });
  assert.equal(calls, 1);
});
test("invalid JSON retains received status and safe classification, not raw response", async () => {
  const live = adapter("token", runId, "diagnostic", async () => ({ ok: true, status: 200,
    json: async () => { throw new SyntaxError("private response"); } }));
  await assert.rejects(live.api.getPrivateCustomItems(projectId, "fs8-account-probe", []), error => {
    assert.ok(failureLine(error).includes("HTTP=200"));
    assert.ok(failureLine(error).includes("NetworkFault=INVALID_JSON"));
    assert.ok(!failureLine(error).includes("private response"));
    return true;
  });
});
test("unexpected network failure during injection is not swallowed or converted to injection success", async () => {
  const n = network(); let started = false;
  await assert.rejects(run("token", runId, () => {}, async (url, options) => {
    if (options.method === "POST" && JSON.parse(options.body).value?.transfer?.status === "Completed") {
      started = true; throw Object.assign(Error("private transport"), { cause: { code: "ECONNRESET" } });
    }
    return n.fetch(url, options);
  }, { selection: "remaining" }), error => {
    assert.equal(error.code, "REQUEST_UNCONFIRMED");
    assert.ok(failureLine(error).includes("Phase=INJECT_FAULT"));
    assert.ok(failureLine(error).includes("NetworkFault=ECONNRESET"));
    return true;
  });
  assert.ok(started);
});
