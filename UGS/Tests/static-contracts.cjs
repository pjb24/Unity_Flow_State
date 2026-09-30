// Read-only repository contract checks. Run with Node; no Unity process or network.
const fs = require("fs");
const path = require("path");
const assert = require("assert").strict;
const root = path.resolve(__dirname, "../..");
const read = p => fs.readFileSync(path.join(root, p), "utf8").replace(/^\uFEFF/, "");
const config = read("Assets/Scripts/Runtime/Features/OnlineRecordConfiguration.cs");
const submit = read("UGS/CloudCode/submit-record.js");
const query = read("UGS/CloudCode/query-records.js");
for (const value of ["c76d55cf-7846-494b-9dce-a0797b179b36", "a20a46fa-1edb-4d79-9c35-02f2fed31896",
  "fs-stage-stage-001-r1", "fs-infinite-v2"]) {
  assert.ok(config.includes(value)); assert.ok(submit.includes(value)); assert.ok(query.includes(value));
}
const policy = JSON.parse(read("UGS/AccessControl/project-policy.json"));
for (const resource of ["urn:ugs:leaderboards:/**", "urn:ugs:cloud-save:/**"])
  assert.ok(policy.statements.some(s => s.Resource === resource && s.Principal === "Player" &&
    s.Effect === "Deny" && s.Action.includes("Write")), `Missing Player Write Deny: ${resource}`);
assert.deepEqual(JSON.parse(read("UGS/CloudSave/verification-ledger-seed.json")),
  { version: 1, active: "", entries: [], best: {} });
const manifest = JSON.parse(read("Packages/manifest.json")).dependencies;
assert.equal(manifest["com.unity.services.authentication"], "3.8.0");
assert.equal(manifest["com.unity.services.cloudcode"], "2.10.4");
const references = JSON.parse(read("Assets/Scripts/Runtime/Features/FlowState.Runtime.Features.asmdef")).references;
for (const name of ["Unity.Services.Core", "Unity.Services.Core.Environments", "Unity.Services.Authentication", "Unity.Services.CloudCode"])
  assert.ok(references.includes(name));
assert.ok(!submit.includes("context.accessToken"));
assert.ok(submit.includes("writeLock: item.writeLock"));
assert.ok(!/writeLock:\s*null/.test(submit));
const auth = read("Assets/Scripts/Runtime/Features/UgsOnlineAuthenticationGateway.cs");
assert.ok(auth.includes("SetEnvironmentName")); assert.ok(auth.includes("OnlineRecordConfiguration.ProjectId"));
assert.ok(auth.includes("flow-state-verification"));
assert.ok(!auth.includes("production"));
const repository = read("Assets/Scripts/Runtime/Features/CloudCodeRecordRepository.cs");
assert.ok(repository.includes("submit-record")); assert.ok(repository.includes("query-records"));
assert.ok(!repository.includes("LeaderboardsService"));
const coordinator = read("Assets/Scripts/Runtime/Features/OnlineRecordCoordinator.cs");
assert.ok(coordinator.includes("MaximumAttemptsPerRetryTrigger"));
assert.ok(coordinator.includes("InitialRetryDelayMilliseconds <<"));
assert.ok(coordinator.includes("TryRemovePending"));
const scene = read("Assets/Scenes/SampleScene.unity"); // Inspect only; never edits the scene.
assert.ok(scene.includes("_moveSpeed: 8")); assert.ok(scene.includes("_scorePerUnit: 10"));
const codec = read("Assets/Scripts/Runtime/Features/LocalSaveJsonCodec.cs");
for (const field of ["recoveryNoticeConfirmed", "onlinePlayerId", "pendingSubmissions"])
  assert.ok(codec.includes(field));
for (const field of ["submittedIds", "rejectedIds", "rejectedReceipts"])
  assert.ok(!codec.includes(field));
const files = ["OnlineAccountState", "OnlineRecordConfiguration", "OnlineAuthenticationResult",
  "UgsOnlineAuthenticationGateway", "CloudCodeRecordRepository", "OnlineRecordCoordinator", "UgsOnlineRecordTransport"];
for (const name of files) {
  const content = read(`Assets/Scripts/Runtime/Features/${name}.cs`);
  assert.ok(!content.includes("System.Linq")); assert.ok(!/\?\.|\?\?/.test(content));
  assert.ok(!/eyJ[A-Za-z0-9_-]{20,}\./.test(content));
}
console.log("PASS static contracts: identifiers, policies, seed, packages, asmdef, service token, CAS, verification environment, scoring config, retry, migration, source guards");
