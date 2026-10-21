// Static-only Phase 4 Step 6 contract checks. No Unity, UGS, sink, credential, or network call.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");
const policy = require("../CloudCode/operational-safety-policy");
const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8");

const contract = read("UGS/SAFE_LOG_SINK_CONTRACT.md");
const inventory = read("UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md");
const task = read("AI/90_Tasks/Prototype_8/20261007_09_Phase4ManualSteps.md");
const staging = read("UGS/LogSink/CLOUD_RUN_STAGING_SETUP.md");
const secretIam = read("UGS/LogSink/CLOUD_RUN_SECRET_AND_IAM.md");
const safeLogSink = read("UGS/Modules/FlowStateVerification/SafeLogSinkClient.cs");
const safeLogEvent = read("UGS/Modules/FlowStateVerification/SafeLogEvent.cs");
const runtime = read("UGS/Modules/FlowStateVerification/ServerScriptRuntime.cs");
const moduleSource = read("UGS/Modules/FlowStateVerification/FlowStateVerification.cs");
const probeLimiter = read("UGS/Modules/FlowStateVerification/SyntheticLogProbeLimiter.cs");

for (const marker of [
  "ExternalThirtyDaySinkRequired", "정확히 30일", "자동 삭제", "archive", "backup",
  "environment", "moduleReference", "outcome", "reason", "durationBucket", "correlationId", "rateLimitResult",
  "Player ID", "raw request/response body", "송신 경계", "PRODUCTION_DEPLOY_BLOCKED"
]) assert.ok(contract.includes(marker), `missing log-sink contract marker: ${marker}`);

assert.equal(policy.getThirtyDayLogStorageDisposition(7 * 24 * 60 * 60 * 1000), "ExternalThirtyDaySinkRequired");
assert.equal(policy.getThirtyDayLogStorageDisposition(policy.structuredLogRetentionMilliseconds), "Ready");
const safe = {
  environment: "verification", moduleReference: "safe-reference", outcome: "Accepted", reason: "None",
  durationBucket: 500, correlationId: "opaque-server-id", rateLimitResult: "Allowed"
};
assert.equal(policy.isSafePhase4StructuredLog(safe), true);
for (const forbidden of ["playerId", "payload", "token", "secret", "requestBody", "publicNumber"])
  assert.equal(policy.isSafePhase4StructuredLog({ ...safe, [forbidden]: "forbidden" }), false, forbidden);

assert.ok(inventory.includes("Unity Cloud Code 기본 로그는 7일 보존"));
assert.ok(inventory.includes("SAFE_LOG_SINK_CONTRACT.md"));
assert.ok(task.includes("Step 6 진행 기록 — 2026-10-09"));
assert.ok(task.includes("`PRODUCTION_DEPLOY_BLOCKED`를 유지한다."));
for (const marker of ["6-3.1", "run.googleapis.com", "logging.googleapis.com", "secretmanager.googleapis.com", "artifactregistry.googleapis.com", "No service/repository/secret/IAM/Module deployment performed"])
  assert.ok(staging.includes(marker), `missing staging setup marker: ${marker}`);
for (const marker of ["같은 `V`", "FS_LOG_SINK_HMAC_VERIFICATION_V1", "Secret Manager Secret Accessor", "Cloud Code만", "Logging Writer", "service account key(JSON key)는 만들거나 다운로드하지 않는다"])
  assert.ok(secretIam.includes(marker), `missing secret/IAM marker: ${marker}`);

for (const name of ["FS_LOG_SINK_HMAC_V1", "FS_LOG_SINK_URL_V1"])
  assert.ok(safeLogSink.includes(name), `missing LogSink secret name: ${name}`);
for (const field of ["environment", "moduleReference", "outcome", "reason", "durationBucket", "correlationId", "rateLimitResult"])
  assert.ok(safeLogEvent.includes(`[JsonProperty(\"${field}\")]`), `missing exact safe field: ${field}`);
assert.ok(safeLogSink.includes('new Uri(baseUri, LogSinkPath)'));
assert.ok(safeLogSink.includes('"/v1/events"'));
assert.ok(safeLogSink.includes('"X-Flow-State-Timestamp"'));
assert.ok(safeLogSink.includes('"X-Flow-State-Signature"'));
assert.ok(safeLogSink.includes('HttpStatusCode.Accepted'));
assert.ok(safeLogSink.includes('NormalTelemetryMilliseconds = 250'));
assert.ok(safeLogSink.includes('SyntheticProbeTelemetryMilliseconds = 4000'));
assert.ok(safeLogSink.includes('Console.Error.WriteLine("SafeLogSinkUnavailable")'));
assert.ok(!safeLogSink.includes("PlayerId"));
assert.ok(!safeLogSink.includes("ServiceToken"));
assert.ok(!safeLogSink.includes("requestBody"));
assert.ok(runtime.includes('SafeLogSinkClient.NormalTelemetryMilliseconds'));
assert.ok(runtime.includes('TryEmit("Completed", "ModuleInvocation", "NotApplied")'));
assert.ok(runtime.includes('TryEmit("Failed", "ModuleFailure", "NotApplied")'));
assert.ok(runtime.includes('if (name != "FS_TRANSFER_HMAC_V1")'));
assert.ok(!runtime.includes("FS_LOG_SINK_HMAC_V1"));
assert.ok(!runtime.includes("FS_LOG_SINK_URL_V1"));
assert.ok(moduleSource.includes('[CloudCodeFunction("EmitVerificationSafeLogProbe")]'));
assert.ok(moduleSource.includes('TryEmit("Accepted", "SyntheticProbe", "Allowed",'));
assert.ok(moduleSource.includes('phase = probePhase, reason = probeReason, httpStatus = probeHttpStatus'));
for (const stage of ["EnvironmentCheck", "UrlSecretRead", "HmacSecretRead", "UrlValidation", "HmacValidation", "EventPreparation", "HttpRequest", "Completed"])
  assert.ok(safeLogSink.includes(`phase = "${stage}"`));
assert.ok(!safeLogSink.includes("error.Message") && !safeLogSink.includes("error.ToString()"));
assert.ok(moduleSource.includes('SafeLogSinkClient.SyntheticProbeTelemetryMilliseconds'));
assert.ok(!moduleSource.includes("PlayerId"));
assert.ok(!moduleSource.includes("ServiceToken"));
assert.ok(probeLimiter.includes("MinimumIntervalMilliseconds = 60000"));
assert.ok(probeLimiter.includes("Interlocked.CompareExchange"));

console.log("PASS Phase 4 Step 6 static contracts: closed safe-log schema, signed Cloud Code sender boundary, 7-day default-log rejection, external 30-day sink gate, and deletion boundary.");
