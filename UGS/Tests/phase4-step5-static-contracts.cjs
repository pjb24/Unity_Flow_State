// Phase 4 Step 5 local static checks. No Unity, UGS credentials, or remote calls.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8");
const policy = JSON.parse(read("UGS/AccessControl/project-policy.json"));
const runtime = read("UGS/Modules/FlowStateVerification/ServerScriptRuntime.cs");
const moduleSource = read("UGS/Modules/FlowStateVerification/FlowStateVerification.cs");
const transferCrypto = read("UGS/CloudCode/transfer-cryptography.js");
const verificationWindow = read("Assets/Editor/OnlineRecordVerificationWindow.cs");

assert.deepEqual(policy.statements, [
  { Sid: "DenyPlayerLeaderboardWrites", Action: ["Write"], Effect: "Deny", Principal: "Player",
    Resource: "urn:ugs:leaderboards:/**" },
  { Sid: "DenyPlayerCloudSaveWrites", Action: ["Write"], Effect: "Deny", Principal: "Player",
    Resource: "urn:ugs:cloud-save:/**" }
]);
assert.ok(!policy.statements.some(statement => statement.Principal === "Player" && statement.Effect === "Allow"));

const secretName = "FS_TRANSFER_HMAC_V1";
assert.ok(runtime.includes('name != "' + secretName + '"'));
assert.ok(runtime.includes("getSecret(services.ResolveSecretName(name), services.RemainingMilliseconds)"));
assert.ok(moduleSource.includes("client.SecretManager.GetSecret(context, name)"));
assert.ok(transferCrypto.includes('const secretName = "' + secretName + '"'));
assert.ok(transferCrypto.includes("secretManager.getSecret(secretName)"));
assert.ok(!transferCrypto.includes("console.log"));

assert.ok(verificationWindow.includes('MenuItem("Flow State/Online Record Verification")'));
assert.ok(verificationWindow.includes('"Player 직접 Write 403 검증"'));
assert.ok(verificationWindow.includes("PLAYER_WRITE_DENIED"));
assert.ok(verificationWindow.includes("Leaderboards=403 / CloudSave=403"));

console.log("PASS Phase 4 Step 5 static contracts: exact Player Write Deny policy, Cloud Code-only secret allowlist, and verification 403 probe path.");
