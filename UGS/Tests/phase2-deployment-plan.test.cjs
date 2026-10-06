"use strict";
const assert = require("node:assert/strict"), path = require("node:path");
const { execFileSync } = require("node:child_process");
const root = path.resolve(__dirname, "../..");
const output = execFileSync(process.execPath, ["UGS/Verification/check-phase2-deployment.cjs", "--json"],
  { cwd: root, encoding: "utf8" });
const report = JSON.parse(output);
assert.equal(report.verificationOnly, true);
assert.equal(report.remoteInventoryVerified, false);
assert.equal(report.remoteBackupVerified, false);
assert.equal(report.endpoints.length, 9);
assert.equal(new Set(report.endpoints.map(e => e.name)).size, 9);
assert.equal(report.endpoints.filter(e => e.secret !== null).length, 3);
assert.equal(report.bridge.name, "submit-record");
assert.equal(report.bridge.bundled, false);
assert.deepEqual(report.bridge.dependencies, []);
assert.deepEqual(report.bridge.params, report.endpoints.find(e => e.name === "submit-record").params);
for (const endpoint of report.endpoints) {
  assert.equal(endpoint.bundled, true);
  assert.ok(endpoint.dependencies.length > 0);
  assert.equal(new Set(endpoint.dependencies).size, endpoint.dependencies.length);
  assert.ok(endpoint.dependencies.every(file => file.startsWith("UGS/CloudCode/")));
  assert.match(endpoint.closureSha256, /^[a-f0-9]{64}$/);
  for (const input of Object.values(endpoint.params)) {
    assert.equal(input.type, "String"); assert.equal(input.required, true);
    assert.ok(!Object.hasOwn(input, "defaultValue"));
  }
}
assert.ok(report.note.includes("not deployed bundles or remote versions"));
const secretSelfTest = execFileSync(process.execPath, ["UGS/Verification/new-transfer-hmac-secret.cjs", "--self-test"],
  { cwd: root, encoding: "utf8" });
assert.equal(secretSelfTest.trim(), "PASS: production format validation with synthetic values. No deployment secret generated; clipboard untouched.");
console.log("PASS Phase 2 deployment plan: local-only report, 9 endpoint contracts, same-name bridge, 3 Secret consumers, dependency closures and hashes. No remote/Unity execution.");
