// Phase 4 Step 1 read-only contracts. This script never starts Unity or contacts UGS.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8").replace(/^\uFEFF/, "");

const deployment = read("UGS/VERIFICATION_DEPLOYMENT.md");
const task = read("AI/90_Tasks/Prototype_8/20261007_09_Phase4ManualSteps.md");
const manifest = JSON.parse(read("Assets/CloudCode/FlowStateVerification.ccmr"));
const policy = JSON.parse(read("UGS/AccessControl/project-policy.json"));
const moduleProject = read("UGS/Modules/FlowStateVerification/FlowStateVerification.csproj");
const moduleSource = read("UGS/Modules/FlowStateVerification/FlowStateVerification.cs");
const moduleApiSpec = read("UGS/FlowStateVerification.yaml");
const runtime = read("UGS/Modules/FlowStateVerification/ServerScriptRuntime.cs");
const services = read("UGS/Modules/FlowStateVerification/ServerServices.cs");
const clientConfiguration = read("Assets/Scripts/Runtime/Features/OnlineRecordConfiguration.cs");

assert.deepEqual(Object.keys(manifest), ["modulePath"]);
assert.equal(manifest.modulePath, "../../UGS/Modules/FlowStateVerification/FlowStateVerification.sln");
assert.ok(moduleProject.includes('<EmbeddedResource Include="../../CloudCode/*.js"'));

const functionNames = [...moduleSource.matchAll(/\[CloudCodeFunction\("([A-Za-z]+)"\)\]/g)].map(match => match[1]);
assert.deepEqual(functionNames, [
  "GetPublicPlayerNumber", "GetAccountTransferStatus", "GetAccountPersonalBests",
  "StartAccountTransfer", "ReissueAccountTransfer", "CancelAccountTransfer",
  "CompleteAccountTransfer", "SubmitRecord", "QueryRecords", "EmitVerificationSafeLogProbe"
]);
for (const functionName of functionNames)
  assert.ok(moduleApiSpec.includes(`  /${functionName}:`), `Downloaded API spec is missing ${functionName}`);
assert.ok(runtime.includes('name != "FS_TRANSFER_HMAC_VERIFICATION_V1"'));
assert.ok(!runtime.includes("GetSecret(context, name).Result"));

for (const resource of ["urn:ugs:leaderboards:/**", "urn:ugs:cloud-save:/**"]) {
  const matches = policy.statements.filter(statement => statement.Resource === resource);
  assert.equal(matches.length, 1, `Expected one policy statement for ${resource}`);
  assert.deepEqual(matches[0], {
    Sid: resource.includes("leaderboards") ? "DenyPlayerLeaderboardWrites" : "DenyPlayerCloudSaveWrites",
    Action: ["Write"], Effect: "Deny", Principal: "Player", Resource: resource
  });
}
assert.ok(!policy.statements.some(statement => statement.Principal === "Player" && statement.Effect === "Allow"));

const verificationProject = "c76d55cf-7846-494b-9dce-a0797b179b36";
const verificationEnvironment = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
for (const source of [services, clientConfiguration, read("UGS/CloudCode/account-store.js")]) {
  assert.ok(source.includes(verificationProject));
  assert.ok(source.includes(verificationEnvironment));
  assert.ok(!source.includes("ProductionProjectId"));
  assert.ok(!source.includes("ProductionEnvironmentId"));
}

assert.ok(deployment.indexOf("## 현재 적용 방식 — C# Module") < deployment.indexOf("## 과거 JavaScript 게시 절차 — 참고 이력"));
assert.ok(deployment.includes("## 과거 Phase 4의 제한된 원격 변경 — 참고 이력"));
assert.ok(deployment.includes("현재 배포 단위는 이 문서 맨 앞의"));
assert.ok(task.includes("### Step 1 수동 확인 기록표"));
for (const forbiddenField of ["Secret 값", "Player ID", "원격 source 본문"])
  assert.ok(task.includes(forbiddenField));

console.log("PASS Phase 4 Step 1 static contracts: single C# Module, historical JS separation, verification-only scope, secret allowlist, and Player Write Deny.");
