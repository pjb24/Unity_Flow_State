"use strict";
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path"), os = require("node:os"), cp = require("node:child_process"), crypto = require("node:crypto");
const ingress = require("./dependency-ingress.cjs"), audit = require("./dependency-audit.cjs");
const tests = [], test = (name, run) => tests.push({ name, run }), sha = b => crypto.createHash("sha256").update(b).digest("hex");
const payload = "synthetic vendor payload\n", guid = "a".repeat(32), evidencePath = "catalog.json";
const empty = "## 등록 목록\n\n| asset_id | 제품 | 도입 | 조건 | 경계 | 복원 | 링크 |\n| --- | --- | --- | --- | --- | --- | --- |\n\n## 항목\n";
const esc = v => v.replace(/\\/g, "\\\\").replace(/\|/g, "\\|");
function record(kind = "original", target = "Assets/ThirdParty/vendor/product", rights = true) {
  const id = "synthetic-asset", fields = Object.fromEntries(audit.requiredFields.map(f => [f, "synthetic reviewed evidence for " + f]));
  const status = { adoption: { state: "후보", evidence: "미확인: import pending" }, rights: { state: rights ? "근거 확인" : "미확인", evidence: rights ? "synthetic review" : "미확인: rights pending" }, boundary: { state: ["mixed", "unknown"].includes(kind) ? "판정 대기" : "확정", evidence: "synthetic boundary" }, restore: { state: "미검증", evidence: "미확인: execution pending" }, decision: { state: "pending", evidence: "미확인: exclusion decision pending" } };
  Object.assign(fields, { asset_id: id, record_kind: "Third Party 애셋", product: "Synthetic", version: "1.0.0", official_source: "https://example.invalid/distribution", evidence: "sha256:" + "1".repeat(64), import_path: "`" + JSON.stringify([target]) + "`", status: "`" + JSON.stringify(status) + "`" });
  const summary = [id, "Synthetic / 1.0.0", ...["adoption", "rights", "boundary", "restore"].map(a => status[a].state), "[" + id + "](#" + id + ")"];
  const row = [target, "`" + JSON.stringify({ kind, evidence: "synthetic comparison" }) + "`", "fixture state", "preserve", "`" + JSON.stringify({ asset: "preserve", folder: "preserve", evidence: "synthetic meta" }) + "`", "synthetic terms", "synthetic usage"];
  const text = empty.replace("\n\n## 항목", "\n| " + summary.map(esc).join(" | ") + " |\n\n## 항목") + "\n## " + id + "\n\n| 필드 | 값 |\n| --- | --- |\n" + Object.entries(fields).map(([k, v]) => "| " + k + " | " + esc(v) + " |").join("\n") +
    "\n\n### 경로별 경계 표\n\n| 경로 | 역할 | 추적 | 방향 | meta | 조건 | 사용 |\n| --- | --- | --- | --- | --- | --- | --- |\n| " + row.map(esc).join(" | ") + " |\n\n### 변경/검증 이력\n\n| 날짜 | 버전 | 변경 | 근거 | 검증 | 의무 |\n| --- | --- | --- | --- | --- | --- |\n";
  return { text, policy: { schemaVersion: 1, unregisteredRoots: ["Assets/ThirdParty"], entries: [{ asset_id: id, path: target, kind, action: "preserve", meta: { asset: "preserve", folder: "preserve" }, evidenceFields: ["evidence", "meta_import", "restore", "owner_review"] }] } };
}
function fixture(run) {
  const base = fs.realpathSync(os.tmpdir()), root = fs.mkdtempSync(path.join(base, "flowstate-ingress-"));
  const write = (p, bytes) => { fs.mkdirSync(path.dirname(path.join(root, p)), { recursive: true }); fs.writeFileSync(path.join(root, p), bytes); };
  const git = (...args) => cp.execFileSync("git", ["-c", "core.hooksPath=" + path.join(root, "empty-hooks"), "-c", "core.fsmonitor=false", "-C", root, ...args], { encoding: "utf8", windowsHide: true, stdio: ["ignore", "pipe", "pipe"] });
  const evidence = { schemaVersion: 1, evidence: [{ path: "evidence.json", sha256: sha('{"synthetic":true}') }], origins: [{ asset_id: "technical-tmp-baseline", path: "Assets/TextMesh Pro/Original.bin", sha256: sha(payload), textSha256: sha(payload), guid, kind: "original" }], preserveExceptions: [] };
  const saveEvidence = () => write(evidencePath, JSON.stringify(evidence));
  const setRecord = (...args) => { const r = record(...args); write("AI/00_Project/ASSET_DEPENDENCIES.md", r.text); write("Tools/DependencyAudit/dependency-policy.json", JSON.stringify(r.policy)); };
  try {
    write(".gitignore", "/Assets/ThirdParty/\n/Assets/ThirdParty.meta\n/Assets/TextMesh Pro/\n/Ignore/\n"); write("README.md", "synthetic baseline\n");
    write("AI/00_Project/ASSET_DEPENDENCIES.md", empty); write("Tools/DependencyAudit/dependency-policy.json", JSON.stringify({ schemaVersion: 1, unregisteredRoots: ["Assets/ThirdParty"], entries: [] })); write("evidence.json", '{"synthetic":true}'); saveEvidence();
    git("init", "--quiet"); git("-c", "core.autocrlf=false", "add", "."); git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "synthetic baseline");
    const check = (mode = "--check-managed") => {
      const before = fs.readFileSync(path.join(root, ".git/index")); const r = ingress.cli([mode, "--root", root, "--evidence", evidencePath]); assert.deepEqual(fs.readFileSync(path.join(root, ".git/index")), before); return r;
    };
    run({ root, write, git, evidence, saveEvidence, setRecord, check });
  } finally { const absolute = path.resolve(root); assert.equal(path.dirname(absolute).toLowerCase(), base.toLowerCase()); assert(path.basename(absolute).startsWith("flowstate-ingress-")); fs.rmSync(absolute, { recursive: true, force: true }); }
}
const issues = r => r.report.indexResult.issues.map(i => i.code);
test("empty actual adoption and no staged changes can pass with explicit known evidence", () => fixture(({ check }) => { const r = check(); assert.equal(r.exitCode, 0); assert.equal(r.report.referenceHealthPass, false); assert.equal(r.report.indexUnchanged, true); }));
test("pinned Cloud Code technical archive evidence does not fabricate product adoption", () => fixture(({ evidence, saveEvidence, check }) => {
  evidence.origins[0].asset_id = "technical-cloudcode-baseline"; saveEvidence(); const r = check(); assert.equal(r.exitCode, 0); assert.equal(r.report.registeredRecords, 0);
}));
test("force-staged unregistered vendor path blocked even when ignored", () => fixture(({ write, git, check }) => {
  write("Assets/ThirdParty/vendor/new.bin", "unseen payload"); git("add", "-f", "Assets/ThirdParty/vendor/new.bin"); const r = check(); assert.equal(r.exitCode, 1); assert(issues(r).includes("UNREGISTERED_THIRDPARTY_TRACKED")); assert(r.report.trackedIgnore.find(v => v.path.endsWith("new.bin")).ignored);
}));
test("registered original path and child/root meta are blocked", () => fixture(({ write, git, setRecord, evidence, saveEvidence, check }) => {
  setRecord(); evidence.origins[0].asset_id = "synthetic-asset"; evidence.origins[0].path = "Assets/ThirdParty/vendor/product/Original.bin"; saveEvidence();
  write("Assets/ThirdParty/vendor/product.meta", "guid: " + "b".repeat(32) + "\nfolderAsset: yes\n"); git("add", "-f", "Assets/ThirdParty/vendor/product.meta"); const r = check(); assert.equal(r.exitCode, 1); assert(issues(r).includes("REGISTERED_ORIGINAL_TRACKED"));
}));
test("unregistered ThirdParty root meta and case variants blocked", () => fixture(({ write, git, check }) => { write("Assets/thirdparty.meta", "guid: " + "c".repeat(32) + "\n"); git("add", "-f", "Assets/thirdparty.meta"); assert(issues(check()).includes("UNREGISTERED_THIRDPARTY_TRACKED")); }));
test("known origin bytes outside declared namespace blocked", () => fixture(({ write, git, check }) => { write("Other/copied.bin", payload); git("add", "Other/copied.bin"); assert(issues(check()).includes("KNOWN_ORIGINAL_BYTES")); }));
test("Git newline conversion cannot hide a known source", () => fixture(({ write, git, evidence, saveEvidence, check }) => {
  evidence.origins[0].sha256 = sha("synthetic vendor payload\r\n"); saveEvidence(); write("Other/copy.txt", "synthetic vendor payload\r\n"); git("-c", "core.autocrlf=true", "add", "Other/copy.txt"); assert(issues(check()).includes("KNOWN_ORIGINAL_BYTES"));
}));
test("known GUID in changed metadata outside namespace blocked", () => fixture(({ write, git, check }) => { write("Other/changed.meta", "guid: " + guid + "\nImporter: different\n"); git("add", "Other/changed.meta"); assert(issues(check()).includes("KNOWN_ORIGIN_GUID")); }));
test("index bytes are inspected even if unstaged worktree is clean", () => fixture(({ write, git, check }) => { write("Other/a.bin", payload); git("add", "Other/a.bin"); write("Other/a.bin", "innocent worktree bytes"); assert(issues(check()).includes("KNOWN_ORIGINAL_BYTES")); }));
test("unstaged original bytes do not become a staged candidate", () => fixture(({ write, git, check }) => { write("Other/a.bin", "innocent staged bytes"); git("add", "Other/a.bin"); write("Other/a.bin", payload); assert.equal(check("--check-staged").exitCode, 0); }));
test("HEAD and projected index results stay independent on staged deletion", () => fixture(({ write, git, check }) => {
  write("Other/a.bin", payload); git("add", "Other/a.bin"); git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "synthetic original"); git("rm", "--quiet", "Other/a.bin");
  const staged = check("--check-staged"), managed = check(); assert.equal(staged.exitCode, 0); assert.equal(staged.report.headResult.blocked, true); assert.equal(managed.exitCode, 1); assert(staged.report.stagedChanges.some(c => c.status === "D"));
}));
test("rename/copy detection preserves paths and catches payload", () => fixture(({ write, git, check }) => {
  write("Other/from.bin", payload); git("add", "Other/from.bin"); git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "synthetic"); git("mv", "Other/from.bin", "Other/renamed.bin"); write("Other/copied.bin", payload); git("add", "Other/copied.bin");
  const r = check("--check-staged"); assert.equal(r.exitCode, 1); assert(r.report.stagedChanges.some(v => /^[RC]/.test(v.status))); assert(r.report.indexResult.issues.some(v => v.path === "Other/renamed.bin"));
}));
test("Unicode and spaces in real Windows staged paths survive NUL parsing", () => fixture(({ write, git, check }) => {
  const name = "Other/한 글 with space.bin"; write(name, payload); git("add", name); const r = check(); assert(r.report.indexResult.issues.some(i => i.path === name)); assert(r.report.stagedChanges.some(i => i.path === name));
}));
test("nested ignore exception does not grant original tracking permission", () => fixture(({ write, git, check }) => {
  write("Other/.gitignore", "*.bin\n!allowed.bin\n"); write("Other/allowed.bin", payload); git("add", "Other/allowed.bin"); const r = check(); assert.equal(r.exitCode, 1); assert.equal(r.report.trackedIgnore.find(i => i.path === "Other/allowed.bin").ignored, false);
}));
test("duplicate GUIDs fail incomplete rather than passing an empty set", () => fixture(({ write, git, check }) => { for (const name of ["A", "B"]) write("Other/" + name + ".meta", "guid: " + "b".repeat(32) + "\n"); git("add", "Other"); const r = check(); assert.equal(r.exitCode, 2); assert.equal(r.report.diagnostics[0].code, "INGRESS_DUPLICATE_GUID"); }));
test("unmerged index / case collision / truncated blob / unsupported links fail", () => {
  const id = "1".repeat(40); assert.throws(() => ingress.parseEntries(Buffer.from("100644 " + id + " 1\tAssets/A\0"), "index"));
  const unusual = "Assets/한 글\nwith\ttab.bin";
  assert.equal(ingress.parseEntries(Buffer.from("100644 " + id + " 0\t" + unusual + "\0"), "index")[0].path, unusual);
  assert.equal(audit.stagedChanges("A\0" + unusual + "\0")[0].path, unusual);
  assert.throws(() => ingress.parseEntries(Buffer.from("100644 " + id + " 0\tAssets/A\0" + "100644 " + id + " 0\tAssets/a\0"), "index"));
  assert.throws(() => ingress.parseEntries(Buffer.from("120000 " + id + " 0\tAssets/Link\0"), "index"));
  assert.throws(() => ingress.batch(Buffer.from(id + " blob 99\nx\n"), [id]));
});
test("unknown/mixed boundary stays pending", () => fixture(({ setRecord, write, git, check }) => { setRecord("unknown"); write("Assets/ThirdParty/vendor/product/A.bin", "unseen"); git("add", "-f", "Assets/ThirdParty/vendor/product/A.bin"); assert(issues(check()).includes("BOUNDARY_PENDING_TRACKED")); }));
test("project preservation requires explicit reviewed exception", () => fixture(({ setRecord, write, git, evidence, saveEvidence, check }) => {
  const target = "Assets/ProjectAssets/synthetic-asset/Changed.bin"; setRecord("modified", target); evidence.origins[0].kind = "modified"; saveEvidence(); write(target, payload); git("add", target);
  assert.equal(check().exitCode, 1); evidence.preserveExceptions.push({ asset_id: "synthetic-asset", path: target }); saveEvidence(); const r = check(); assert.equal(r.exitCode, 0); assert.equal(r.report.indexResult.preserved.length, 1);
}));
test("unverified preservation exception cannot grant rights", () => fixture(({ setRecord, evidence, saveEvidence, check }) => {
  const target = "Assets/ProjectAssets/synthetic-asset/Changed.bin"; setRecord("modified", target, false); evidence.preserveExceptions.push({ asset_id: "synthetic-asset", path: target }); saveEvidence(); assert.equal(check().exitCode, 2);
}));
test("original bytes remain blocked even with a project exception", () => fixture(({ setRecord, write, git, evidence, saveEvidence, check }) => {
  const target = "Assets/ProjectAssets/synthetic-asset/Changed.bin"; setRecord("modified", target); evidence.preserveExceptions.push({ asset_id: "synthetic-asset", path: target }); saveEvidence(); write(target, payload); git("add", target); assert(issues(check()).includes("KNOWN_ORIGINAL_BYTES"));
}));
test("missing original evidence / orphan policy / stale evidence fail", () => fixture(({ setRecord, check, write }) => {
  setRecord(); assert.equal(check().report.diagnostics[0].code, "INGRESS_ORIGIN_COVERAGE");
  write("AI/00_Project/ASSET_DEPENDENCIES.md", empty); assert.equal(check().exitCode, 2);
}));
test("evidence pin mismatch fails closed", () => fixture(({ write, check }) => { write("evidence.json", "changed metadata"); const r = check(); assert.equal(r.exitCode, 2); assert.equal(r.report.diagnostics[0].code, "INGRESS_EVIDENCE_CHANGED"); }));
test("sensitive bodies never enter blob batch or output", () => fixture(({ write, git, check }) => {
  write("Other/.env", "synthetic-hidden-payload"); git("add", "Other/.env"); const r = check(); assert.equal(r.exitCode, 0); assert(!JSON.stringify(r).includes("synthetic-hidden-payload")); assert(r.report.indexResult.sensitiveBodiesOmitted.includes("Other/.env"));
}));
test("reviewed changed metadata may preserve a supplier GUID without granting rights", () => fixture(({ setRecord, write, git, evidence, saveEvidence, check }) => {
  const target = "Assets/ProjectAssets/synthetic-asset/Changed.meta"; setRecord("modified", target); evidence.preserveExceptions.push({ asset_id: "synthetic-asset", path: target }); saveEvidence(); write(target, "guid: " + guid + "\nImporter: changed with synthetic review\n"); git("add", target);
  const r = check(); assert.equal(r.exitCode, 0); assert.equal(r.report.rightsGranted, false);
}));
test("explicit folder meta boundary overrides inherited original folder boundary", () => {
  const p = "Assets/ThirdParty/vendor/product.meta", rows = [{ path: p, oid: "1".repeat(40) }];
  const r = { boundaries: [{ path: "Assets/ThirdParty/vendor/product", kind: "original" }, { path: p, kind: "project" }] };
  const result = ingress.evaluate(rows, new Map([[rows[0].oid, Buffer.from("guid: " + "b".repeat(32) + "\n")]]), [r], { unregisteredRoots: ["Assets/ThirdParty"] }, { origins: [] }, new Map([[p.toLowerCase(), { path: p }]]));
  assert.equal(result.blocked, false); assert.equal(result.preserved.length, 1);
});
test("Git command allowlist refuses mutation and batch handles binary NULs", () => {
  assert.throws(() => ingress.gitReader(process.cwd())(["add", "."]));
  const id = "1".repeat(40), binary = Buffer.from([0, 255, 10, 0]);
  const result = ingress.batch(Buffer.concat([Buffer.from(id + " blob 4\n"), binary, Buffer.from("\n")]), [id]); assert.deepEqual(result.get(id), binary);
});
test("real CLI exits nonzero for actual staged payload", () => fixture(({ root, write, git }) => {
  write("Other/copied.bin", payload); git("add", "Other/copied.bin"); const r = cp.spawnSync(process.execPath, [path.join(__dirname, "dependency-ingress.cjs"), "--check-staged", "--root", root, "--evidence", evidencePath], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 1); assert.equal(JSON.parse(r.stdout).passed, false);
}));
test("require is inert and missing/invalid arguments are sanitized", () => {
  const script = "const fs=require('fs'),cp=require('child_process');for(const k of ['writeFileSync','mkdirSync','rmSync'])fs[k]=()=>{throw Error('write')};for(const k of ['spawnSync','execFileSync'])cp[k]=()=>{throw Error('process')};require(process.argv[1]);";
  const r = cp.spawnSync(process.execPath, ["-e", script, path.join(__dirname, "dependency-ingress.cjs")], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 0, r.stderr); assert.equal(r.stdout, "");
  assert.equal(ingress.cli(["--check-managed"]).report.diagnostics[0].code, "INGRESS_EVIDENCE_REQUIRED"); assert.equal(ingress.cli(["--private-token"]).exitCode, 2);
});
let passed = 0; for (const item of tests) { try { item.run(); passed++; } catch (e) { console.error("FAIL " + item.name); throw e; } }
console.log(JSON.stringify({ suite: "DependencyIngress", passed, failed: 0, unityExecuted: false, realIndexChanged: false }));
