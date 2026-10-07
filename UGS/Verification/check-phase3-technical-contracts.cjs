"use strict";
// Read-only Phase 3 technical-contract reporter. It never starts Unity or calls UGS.
// `--require-adapter` turns the planned adapter capabilities into required checks.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8").replace(/^\uFEFF/, "");
const cloudSaveOpenApi = "https://docs.unity.com/en-us/oas-cloud-save/1.0.0";
const leaderboardOpenApi = "https://docs.unity.com/en-us/oas-leaderboards/1.0.0";
const contractCheckedOn = "2026-10-06";
const cloudSave = Object.freeze({ privateCustomKeyLimit: 2000, privateCustomTotalBytes: 5 * 1024 * 1024, batchItemLimit: 20 });
const receiptLayout = Object.freeze({ maximumReceiptBytes: 1024, targetShardBytes: 256 * 1024, targetContainerBytes: 4 * 1024 * 1024, targetContainerKeys: 1600 });
const maximumSubmissionId = "ffffffff-ffff-4fff-bfff-ffffffffffff";
const maximumPayload = JSON.stringify(["fs-stage-stage-001-r1", 1, maximumSubmissionId,
  Number.MAX_SAFE_INTEGER, Number.MAX_SAFE_INTEGER, 2147483647, 2147483647, 2147483647,
  2147483647, 2147483647, 3]);
const maximumReceiptEnvelope = JSON.stringify({ id: maximumSubmissionId, payload: maximumPayload,
  status: "Rejected", reason: "ScoreLimitExceeded", acceptedAt: Number.MAX_SAFE_INTEGER });
const receiptObservedBytes = Object.freeze({ payload: Buffer.byteLength(maximumPayload, "utf8"), envelope: Buffer.byteLength(maximumReceiptEnvelope, "utf8") });

const project = read("UGS/Modules/FlowStateVerification/FlowStateVerification.csproj");
const services = read("UGS/Modules/FlowStateVerification/ServerServices.cs");
const runtime = read("UGS/Modules/FlowStateVerification/runtime.js");
const task = read("AI/90_Tasks/Prototype_8/20261006_08_Phase3ManualSteps.md");
const submit = read("UGS/CloudCode/submit-record.js");
const packageVersion = name => {
  const match = project.match(new RegExp(`<PackageReference\\s+Include="${name}"\\s+Version="([^"]+)"\\s*/>`));
  assert.ok(match, `Missing PackageReference: ${name}`);
  return match[1];
};
const supports = name => services.includes(`case "${name}":`) && runtime.includes(`"${name}"`);
const capabilities = {
  privateCustomRead: supports("getPrivateCustomItems"),
  privateCustomSet: supports("setPrivateCustomItem"),
  privateCustomBatchSet: supports("setPrivateCustomItemBatch"),
  privateCustomDelete: supports("deletePrivateCustomItem"),
  leaderboardPage: supports("getLeaderboardScores"),
  leaderboardPlayerScore: supports("getLeaderboardPlayerScore"),
  leaderboardPlayerRange: supports("getLeaderboardPlayerRange")
};

assert.equal(packageVersion("Com.Unity.Services.CloudCode.Core"), "0.0.7");
assert.equal(packageVersion("Com.Unity.Services.CloudCode.Apis"), "0.0.27");
assert.ok(task.includes(cloudSaveOpenApi), "Phase 3 task must retain the Cloud Save source");
assert.ok(task.includes(leaderboardOpenApi), "Phase 3 task must retain the Leaderboard source");
assert.ok(task.includes("2,000 key") && task.includes("5 MiB") && task.includes("최대 20 item"),
  "Phase 3 task must retain the confirmed Cloud Save limits");
assert.ok(submit.includes("const payload = JSON.stringify([p.boardId"), "Submission payload contract changed; recalculate receipt byte budget");
assert.ok(task.includes("256 KiB") && task.includes("4 MiB") && task.includes("1 KiB"),
  "Phase 3 task must retain the selected receipt byte budget");
assert.ok(receiptLayout.targetContainerBytes < cloudSave.privateCustomTotalBytes);
assert.ok(receiptLayout.targetContainerKeys < cloudSave.privateCustomKeyLimit);
assert.ok(receiptObservedBytes.envelope <= receiptLayout.maximumReceiptBytes,
  "Current maximum receipt envelope exceeds the selected 1 KiB ceiling");
for (const required of ["privateCustomRead", "privateCustomSet", "privateCustomBatchSet", "leaderboardPage", "leaderboardPlayerScore"])
  assert.equal(capabilities[required], true, `Existing adapter capability missing: ${required}`);

const planned = ["privateCustomDelete", "leaderboardPlayerRange"];
if (process.argv.includes("--require-adapter")) {
  for (const name of planned) assert.equal(capabilities[name], true, `Phase 3 adapter capability missing: ${name}`);
}

const report = {
  mode: process.argv.includes("--require-adapter") ? "require-adapter" : "baseline",
  readOnly: true,
  remoteUgsCalled: false,
  officialContract: { checkedOn: contractCheckedOn, cloudSaveOpenApi, leaderboardOpenApi, cloudSave },
  receiptLayout: { ...receiptLayout,
    minimumReceiptsPerShard: Math.floor(receiptLayout.targetShardBytes / receiptLayout.maximumReceiptBytes),
    minimumReceiptsPerContainer: Math.floor(receiptLayout.targetContainerBytes / receiptLayout.maximumReceiptBytes),
    observedCurrentPayloadBytes: receiptObservedBytes.payload,
    observedCurrentReceiptEnvelopeBytes: receiptObservedBytes.envelope },
  modulePackages: {
    cloudCodeCore: packageVersion("Com.Unity.Services.CloudCode.Core"),
    cloudCodeApis: packageVersion("Com.Unity.Services.CloudCode.Apis")
  },
  capabilities,
  pending: planned.filter(name => !capabilities[name]),
  limitations: [
    "Does not download or authenticate against current Unity documentation.",
    "Does not prove deployed Module behavior, service-token authorization, remote limits, or snapshot semantics.",
    "Use --require-adapter after implementation to require delete and player-range routing."
  ]
};

if (process.argv.includes("--json")) console.log(JSON.stringify(report, null, 2));
else {
  console.log(`PASS Phase 3 technical baseline: Cloud Save ${cloudSave.privateCustomKeyLimit} keys / ${cloudSave.privateCustomTotalBytes} bytes / batch ${cloudSave.batchItemLimit}; Cloud Code Core ${report.modulePackages.cloudCodeCore}, APIs ${report.modulePackages.cloudCodeApis}.`);
  console.log(`PASS receipt layout: ${receiptLayout.maximumReceiptBytes}-byte receipt ceiling, ${receiptLayout.targetShardBytes}-byte shard, ${receiptLayout.targetContainerBytes}-byte/${receiptLayout.targetContainerKeys}-key container; minimum ${report.receiptLayout.minimumReceiptsPerShard} receipts/shard, ${report.receiptLayout.minimumReceiptsPerContainer} receipts/container.`);
  console.log(`PASS current receipt budget: payload ${receiptObservedBytes.payload} bytes, envelope ${receiptObservedBytes.envelope} bytes.`);
  for (const [name, supported] of Object.entries(capabilities)) console.log(`${supported ? "PASS" : "PENDING"} ${name}`);
  console.log(`SOURCE ${contractCheckedOn}: ${cloudSaveOpenApi}; ${leaderboardOpenApi}`);
  console.log("Read-only local inspection only; remote UGS, Unity, deployment and runtime behavior NOT verified.");
}
