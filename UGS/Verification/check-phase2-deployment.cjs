"use strict";
// Read-only plan inspection. No bundler, Unity, SDK, network or remote writes.
const fs = require("node:fs"), path = require("node:path"), crypto = require("node:crypto");
const vm = require("node:vm"), assert = require("node:assert/strict");
const root = path.resolve(__dirname, "../..");
const projectId = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const secretName = "FS_TRANSFER_HMAC_VERIFICATION_V1";
const expected = {
  "get-public-player-number": {},
  "get-account-transfer-status": {},
  "get-account-personal-bests": {},
  "start-account-transfer": {},
  "reissue-account-transfer": {},
  "cancel-account-transfer": { transferId: { type: "String", required: true } },
  "complete-account-transfer": { code: { type: "String", required: true }, verificationValue: { type: "String", required: true } },
  "submit-record": { request: { type: "String", required: true } },
  "query-records": { request: { type: "String", required: true } }
};
const secretConsumers = new Set(["start-account-transfer", "reissue-account-transfer", "complete-account-transfer"]);
const allowedExternal = new Set(["crypto", "@unity-services/cloud-save-1.4", "@unity-services/leaderboards-1.1"]);
const read = file => fs.readFileSync(path.join(root, file), "utf8");
const relative = file => path.relative(root, file).split(path.sep).join("/");

function inspect(file, name, bundled) {
  const files = new Map(), external = new Set();
  function walk(filename) {
    const absolute = path.resolve(root, filename);
    assert.ok(absolute.startsWith(path.join(root, "UGS") + path.sep), "Dependency escaped UGS");
    const local = relative(absolute);
    if (files.has(local)) return;
    const source = read(local);
    new vm.Script(source, { filename: local }); // Syntax only; endpoint is never invoked.
    files.set(local, source);
    for (const match of source.matchAll(/require\(\s*["']([^"']+)["']\s*\)/g)) {
      const dependency = match[1];
      if (dependency.startsWith(".")) {
        let resolved = path.resolve(path.dirname(absolute), dependency);
        if (!path.extname(resolved)) resolved += ".js";
        walk(relative(resolved));
      } else {
        assert.ok(allowedExternal.has(dependency), "Unexpected external dependency: " + dependency);
        external.add(dependency);
      }
    }
  }
  walk(file);
  // Read only the literal exported metadata, not executable endpoint code.
  const source = files.get(file);
  const metadata = source.match(/module\.exports\.params\s*=\s*([^;]+);/);
  assert.ok(metadata, file + " missing params");
  const params = JSON.parse(JSON.stringify(vm.runInNewContext("(" + metadata[1] + ")", {}, { timeout: 1000 })));
  assert.deepEqual(params, expected[name], file + " params mismatch");
  assert.equal(/module\.exports\.bundling\s*=\s*true\s*;/.test(source), bundled, file + " bundling mismatch");
  const dependencies = [...files.keys()].filter(p => p !== file).sort();
  if (!bundled) assert.equal(dependencies.length, 0, "Bridge must be standalone");
  const hash = crypto.createHash("sha256");
  for (const [filename, contents] of [...files].sort(([a], [b]) => a.localeCompare(b)))
    hash.update(filename + "\0" + contents.replace(/\r\n/g, "\n") + "\0");
  return { name, file, params, bundled, secret: secretConsumers.has(name) && bundled ? secretName : null,
    dependencies, external: [...external].sort(), closureSha256: hash.digest("hex") };
}

const endpoints = Object.keys(expected).map(name => inspect("UGS/CloudCode/" + name + ".js", name, true));
const bridge = inspect("UGS/Migration/legacy-barrier-submit.js", "submit-record", false);
const store = read("UGS/CloudCode/account-store.js");
const config = read("Assets/Scripts/Runtime/Features/OnlineRecordConfiguration.cs");
for (const source of [store, config]) { assert.ok(source.includes(projectId)); assert.ok(source.includes(environmentId)); }
assert.ok(read("UGS/CloudCode/transfer-cryptography.js").includes(secretName));
const policy = JSON.parse(read("UGS/AccessControl/project-policy.json"));
for (const sid of ["DenyPlayerLeaderboardWrites", "DenyPlayerCloudSaveWrites"])
  assert.ok(policy.statements.some(s => s.Sid === sid && s.Principal === "Player" && s.Effect === "Deny" && s.Action.includes("Write")));
const client = read("Assets/Scripts/Runtime/Features/OnlineAccountCoordinator.cs") +
  read("Assets/Scripts/Runtime/Features/CloudCodeRecordRepository.cs");
for (const name of Object.keys(expected)) assert.ok(client.includes('"' + name + '"'), "Client endpoint missing: " + name);
assert.ok(bridge.file !== endpoints.find(e => e.name === "submit-record").file);
const report = { verificationOnly: true, projectId, environmentId, environmentName: "verification", secretName,
  endpoints, bridge, remoteInventoryVerified: false, remoteBackupVerified: false,
  note: "Hashes identify local LF-normalized source closures, not deployed bundles or remote versions. Bundling and publication NOT executed." };
if (process.argv.includes("--json")) console.log(JSON.stringify(report, null, 2));
else {
  for (const endpoint of [bridge, ...endpoints]) console.log(`${endpoint.name} / ${endpoint.file} / params ${JSON.stringify(endpoint.params)} / local dependencies ${endpoint.dependencies.length} / ${endpoint.closureSha256}`);
  console.log("PASS local deployment contracts: 9 bundled endpoints, 1 standalone same-name bridge, params, dependency closure/syntax, SDK names, context, Client and Player-deny policy. Remote target/backup, bundling, Unity and publication NOT verified/executed.");
}
