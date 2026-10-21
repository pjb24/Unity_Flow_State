const assert = require("node:assert/strict");
const { main, run, failureLine } = require("../Verification/verification-safe-log-probe.cjs");
const { projectId, environmentId } = require("../Verification/verification-scope.cjs");
async function mainTest() {
  let requests = [];
  const fake = async (url, options) => {
    requests.push({ url, options });
    if (requests.length === 1) return { ok: true, status: 200, json: async () => ({ accessToken: "synthetic-token" }) };
    return { ok: true, status: 200, json: async () => ({ output: { status: "Accepted" } }) };
  };
  const plan = [];
  await main([], fake, line => plan.push(line));
  assert.equal(requests.length, 0);
  assert.ok(plan[0].includes("NoRemoteRequests"));
  assert.equal(await run("synthetic-key-id", "synthetic-key-secret", fake), "Accepted");
  assert.equal(requests.length, 2);
  const tokenUrl = new URL(requests[0].url);
  assert.equal(tokenUrl.searchParams.get("projectId"), projectId);
  assert.equal(tokenUrl.searchParams.get("environmentId"), environmentId);
  assert.ok(requests[1].url.endsWith("/modules/FlowStateVerification/EmitVerificationSafeLogProbe"));
  assert.equal(requests[1].options.headers.Authorization, "Bearer synthetic-token");
  assert.equal(requests[1].options.headers["Content-Type"], "application/json");
  assert.deepEqual(JSON.parse(requests[1].options.body), { params: {} });
  assert.ok(requests.every(r => r.options.redirect === "error"));
  let attempts = 0;
  await assert.rejects(run("synthetic-id", "synthetic-secret", async () => {
    attempts++; return { ok: false, status: 401, json: async () => { throw Error("sensitive-body"); } };
  }), error => {
    const line = failureLine(error);
    assert.ok(line.includes("HTTP=401"));
    assert.ok(!/synthetic-secret|sensitive-body|synthetic-id/.test(line));
    return true;
  });
  assert.equal(attempts, 1);
  await assert.rejects(run("synthetic-id", "synthetic-secret", async () => { throw Error("sensitive-transport-body"); }), error => {
    assert.ok(!failureLine(error).includes("sensitive-transport-body")); return true;
  });
  for (const status of ["Unavailable", "RateLimited"]) {
    let count = 0;
    assert.equal(await run("synthetic-id", "synthetic-secret", async () => ({ ok: true, status: 200,
      json: async () => ++count === 1 ? { accessToken: "synthetic-token" } : { output: { status } } })), status);
  }
  for (const [remotePhase, remoteReason, remoteHttp, expectedPhase, expectedReason, expectedHttp] of [
    ["EnvironmentCheck", "Failed", 0, "EnvironmentCheck", "Failed", 0],
    ["EnvironmentCheck", "EnvironmentConfigurationUnavailable", 0, "EnvironmentCheck", "EnvironmentConfigurationUnavailable", 0],
    ["UrlSecretRead", "Timeout", 0, "UrlSecretRead", "Timeout", 0],
    ["HttpRequest", "HttpRejected", 401, "HttpRequest", "HttpRejected", 401],
    ["sensitive-url", "sensitive-secret", "sensitive-token", "Unknown", "Unknown", 0]
  ]) {
    let count = 0, diagnostic;
    const result = await run("synthetic-id", "synthetic-secret", async () => ({ ok: true, status: 200,
      json: async () => ++count === 1 ? { accessToken: "synthetic-token" } :
        { output: { status: "Unavailable", phase: remotePhase, reason: remoteReason, httpStatus: remoteHttp } } }),
      value => { diagnostic = value; });
    assert.equal(result, "Unavailable");
    assert.deepEqual(diagnostic, { phase: expectedPhase, reason: expectedReason, httpStatus: expectedHttp });
    assert.equal(count, 2);
  }
  assert.ok(!failureLine({ code: "sensitive-code", phase: "sensitive-phase", status: "sensitive-status" }).includes("sensitive"));
  console.log("PASS verification safe-log probe: offline plan, fixed environment/endpoint, JSON contract, sanitized failures, no retries and safe statuses. No remote requests.");
}
mainTest().catch(error => { console.error(error); process.exitCode = 1; });
