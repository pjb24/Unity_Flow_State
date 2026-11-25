"use strict";
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path"), os = require("node:os"), cp = require("node:child_process"), crypto = require("node:crypto");
const snapshot = require("./dependency-snapshot.cjs"), audit = require("./dependency-audit.cjs");
const tests = [], test = (name, run) => tests.push({ name, run }), sha = b => crypto.createHash("sha256").update(b).digest("hex");
const row = (p, b) => ({ path: p, sha256: sha(b) });
function fixture(run) {
  const base = fs.mkdtempSync(path.join(path.resolve(os.tmpdir()), "flowstate-snapshot-test-")), source = path.join(base, "source"), target = path.join(base, "candidate");
  fs.mkdirSync(source);
  const write = (p, bytes) => { const full = path.join(source, p); fs.mkdirSync(path.dirname(full), { recursive: true }); fs.writeFileSync(full, bytes); return row(p, bytes); };
  const file = write("Assets/Folder/Material.mat", 'Material:\n  m_Name: "synthetic material"\n'), meta = write("Assets/Folder/Material.mat.meta", "guid: " + "1".repeat(32) + "\nImporter: preserve\n"), folder = write("Assets/Empty.meta", "guid: " + "2".repeat(32) + "\nfolderAsset: yes\n");
  fs.mkdirSync(path.join(source, "Assets/Empty"));
  const manifest = { schemaVersion: 1, candidateRoot: target, files: [file, meta, folder], requiredPaths: [file.path, meta.path, folder.path], directories: [], includeSdkPin: true };
  const f = { base, source, target, write, manifest, plan(io) { return snapshot.prepare(source, manifest, io); }, create(io) { return snapshot.create(source, manifest, this.plan().planSha256, io); } };
  try { run(f); } finally {
    // Recursive cleanup is confined to this exact mkdtemp fixture, never source workspace/candidate.
    const resolved = fs.realpathSync(base), temp = fs.realpathSync(path.resolve(os.tmpdir()));
    assert.equal(path.dirname(resolved).toLowerCase(), temp.toLowerCase());
    assert(path.basename(resolved).startsWith("flowstate-snapshot-test-")); assert(!fs.lstatSync(base).isSymbolicLink());
    fs.rmSync(base, { recursive: true, force: true });
  }
}
const blocked = (run, code) => assert.throws(run, e => e instanceof audit.AuditError && (!code || e.code === code));
const ioWith = overrides => Object.assign(Object.create(fs), overrides);
test("read-only prepare deterministic and contains no body", () => fixture(f => {
  const a = f.plan(), b = f.plan(); assert.deepEqual(a, b); assert(!fs.existsSync(f.target)); assert(!JSON.stringify(a).includes("synthetic material")); assert.equal(a.copied, false);
}));
test("exact bytes GUID importer names BOM and empty folder preserved with fixed SDK pin", () => fixture(f => {
  f.manifest.files.push(f.write("Packages/manifest.json", '\uFEFF{"dependencies":{}}\n'));
  const r = f.create(); assert.equal(r.copied, true); assert.equal(r.independentRestore, false); assert(fs.statSync(path.join(f.target, "Assets/Empty")).isDirectory());
  for (const p of f.manifest.files) assert(fs.readFileSync(path.join(f.source, p.path)).equals(fs.readFileSync(path.join(f.target, p.path))));
  assert.deepEqual(JSON.parse(fs.readFileSync(path.join(f.target, "global.json"))), { sdk: { version: "9.0.315", rollForward: "disable", allowPrerelease: false } }); assert(!fs.existsSync(path.join(f.source, "global.json")));
}));
test("space and Korean paths copy without shell interpretation", () => fixture(f => { f.manifest.files.push(f.write("Assets/space \uD55C\uAE00/thing.txt", "value")); assert(f.create().verified); }));
for (const kind of ["same", "child", "parent", "case"]) test("source overlap " + kind + " rejected", () => fixture(f => {
  f.manifest.candidateRoot = kind === "same" ? f.source : kind === "child" ? path.join(f.source, "sub") : kind === "parent" ? f.base : f.source.toUpperCase(); blocked(() => f.plan(), "SNAPSHOT_ROOT");
}));
test("actual workspace is protected even if caller selects a fixture source", () => fixture(f => { f.manifest.candidateRoot = path.resolve(__dirname, "../.."); blocked(() => f.plan(), "SNAPSHOT_ROOT"); }));
for (const p of ["../outside", "/absolute", "C:/absolute", "Assets/../bad", "Assets/a\\b", "Assets/A.", "Assets/NUL.txt", "Assets/a:b", "Assets/a\nfile", "Assets/a?b"])
  test("unsafe relative path rejected: " + JSON.stringify(p), () => fixture(f => { f.manifest.files.push(row(p, "x")); blocked(() => f.plan()); }));
for (const p of [".git/index", "Library/a", "Build/a", "Assets/node_modules/a", "UGS/LogSink/.env", "UGS/a-secret.txt", "Assets/ThirdParty/vendor/a", "Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs", "Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs.meta", ".vscode/settings.json", ".vsconfig", "global.json"])
  test("excluded production/cache/secret path rejected: " + p, () => fixture(f => { f.manifest.files.push(f.write(p, "must not transfer")); blocked(() => f.plan()); assert(!fs.existsSync(f.target)); }));
test("unapproved private original input rejected before body read", () => fixture(f => { f.manifest.files.push(row("Ignore/paid/original.bin", "secret body")); blocked(() => f.plan()); }));
for (const nonempty of [false, true]) test("existing " + (nonempty ? "nonempty" : "empty") + " destination rejected", () => fixture(f => {
  fs.mkdirSync(f.target); if (nonempty) fs.writeFileSync(path.join(f.target, "keep"), "unchanged"); blocked(() => f.create(), "SNAPSHOT_TARGET_EXISTS"); if (nonempty) assert.equal(fs.readFileSync(path.join(f.target, "keep"), "utf8"), "unchanged");
}));
test("target parent file rejected", () => fixture(f => { fs.writeFileSync(path.join(f.base, "file"), "keep"); f.manifest.candidateRoot = path.join(f.base, "file", "candidate"); blocked(() => f.plan(), "SNAPSHOT_PARENT_FILE"); }));
for (const kind of ["duplicate", "case", "parent-file"]) test("path collision " + kind, () => fixture(f => {
  const p = f.manifest.files[0]; f.manifest.files.push(kind === "parent-file" ? row(p.path + "/child", "x") : { ...p, path: kind === "case" ? p.path.toUpperCase() : p.path }); blocked(() => f.plan());
}));
test("implicit directory case collision rejected", () => fixture(f => { f.manifest.files.push(f.write("Assets/folder/x", "x")); blocked(() => f.plan(), "SNAPSHOT_DUPLICATE"); }));
test("missing required metadata cannot become empty success", () => fixture(f => { f.manifest.requiredPaths.push("Assets/missing.meta"); blocked(() => f.plan(), "SNAPSHOT_REQUIRED_INPUT"); }));
test("empty file selection rejected", () => fixture(f => { f.manifest.files = []; blocked(() => f.plan(), "SNAPSHOT_EMPTY"); }));
test("empty required selection rejected", () => fixture(f => { f.manifest.requiredPaths = []; blocked(() => f.plan(), "SNAPSHOT_SCHEMA"); }));
test("Module restore must include both receipt implementations and regression inputs", () => fixture(f => {
  f.manifest.files.push(f.write("UGS/Modules/FlowStateVerification/FlowStateVerification.csproj", "synthetic")); blocked(() => f.plan(), "SNAPSHOT_REQUIRED_INPUT");
}));
test("missing source body is a hard failure", () => fixture(f => { fs.unlinkSync(path.join(f.source, f.manifest.files[0].path)); blocked(() => f.plan()); }));
test("source digest contamination rejected before target creation", () => fixture(f => { f.manifest.files[0].sha256 = "f".repeat(64); blocked(() => f.plan(), "SNAPSHOT_INPUT_HASH"); assert(!fs.existsSync(f.target)); }));
test("duplicate GUID version rejected", () => fixture(f => { f.manifest.files.push(f.write("Assets/Other.meta", "guid: " + "1".repeat(32) + "\n")); blocked(() => f.plan(), "SNAPSHOT_GUID_DUPLICATE"); }));
test("missing empty-folder partner fails", () => fixture(f => { fs.rmdirSync(path.join(f.source, "Assets/Empty")); blocked(() => f.plan()); }));
test("explicit empty directory retained", () => fixture(f => { fs.mkdirSync(path.join(f.source, "Assets/ManualEmpty")); f.manifest.directories.push("Assets/ManualEmpty"); f.create(); assert(fs.statSync(path.join(f.target, "Assets/ManualEmpty")).isDirectory()); }));
test("source read denial fails without leaking raw error/body", () => fixture(f => {
  const io = ioWith({ readFileSync() { throw Error("private-value-body"); } }); blocked(() => f.plan(io), "IO_READ_FAILED");
}));
test("permission denied on destination leaves failed status, no cleanup", () => fixture(f => {
  const io = ioWith({ mkdirSync() { const e = Error("private-permission"); e.code = "EACCES"; throw e; } }); blocked(() => f.create(io), "SNAPSHOT_IO_FAILED_PARTIAL_RETAINED"); assert(!fs.existsSync(f.target));
}));
test("exclusive write and partial-copy failure retain partial output", () => fixture(f => {
  let n = 0; const io = ioWith({ writeFileSync(p, bytes, flags) { assert.equal(flags.flag, "wx"); if (++n === 2) throw Error("body must not leak"); return fs.writeFileSync(p, bytes, flags); } });
  blocked(() => f.create(io), "SNAPSHOT_IO_FAILED_PARTIAL_RETAINED"); assert(fs.existsSync(f.target)); assert.equal(n, 2); blocked(() => f.plan(), "SNAPSHOT_TARGET_EXISTS");
}));
test("late source mutation after plan rejects digest or hash", () => fixture(f => {
  const plan = f.plan(); f.manifest.files.push(f.write("Assets/late", "late")); blocked(() => snapshot.create(f.source, f.manifest, plan.planSha256), "SNAPSHOT_PLAN_CHANGED"); assert(!fs.existsSync(f.target));
}));
test("missing or wrong approved plan digest refuses writes", () => fixture(f => { blocked(() => snapshot.create(f.source, f.manifest), "SNAPSHOT_PLAN_REQUIRED"); blocked(() => snapshot.create(f.source, f.manifest, "0".repeat(64)), "SNAPSHOT_PLAN_CHANGED"); }));
test("copy-time source mutation preserves partial evidence and fails", () => fixture(f => {
  let n = 0; const io = ioWith({ writeFileSync(p, bytes, flags) { const r = fs.writeFileSync(p, bytes, flags); if (++n === 1) fs.writeFileSync(path.join(f.source, f.manifest.files[0].path), "late-user-change"); return r; } });
  blocked(() => f.create(io), "SNAPSHOT_INPUT_HASH"); assert(fs.existsSync(f.target)); assert.equal(fs.readFileSync(path.join(f.source, f.manifest.files[0].path), "utf8"), "late-user-change");
}));
test("target corruption fails hash comparison", () => fixture(f => {
  const plan = f.plan(); f.create(); fs.writeFileSync(path.join(f.target, f.manifest.files[0].path), "corruption"); blocked(() => snapshot.verify(plan), "SNAPSHOT_TARGET_HASH");
}));
test("verification cannot replace expected digests with corrupted candidate hashes", () => fixture(f => {
  const plan = f.plan(); f.create(); plan.files[0].sha256 = sha("forged"); blocked(() => snapshot.verify(plan), "SNAPSHOT_PLAN_CHANGED");
}));
test("a source Git change during copy fails and retains output", () => fixture(f => {
  cp.execFileSync("git", ["init", "--quiet", f.source], { windowsHide: true });
  const git = args => cp.execFileSync("git", ["-c", "core.autocrlf=false", "-c", "commit.gpgsign=false", "-C", f.source, ...args], { windowsHide: true, stdio: "pipe" });
  git(["add", "Assets"]); git(["-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "fixture"]);
  let touched = false; const io = ioWith({ writeFileSync(p, bytes, flags) { const r = fs.writeFileSync(p, bytes, flags); if (!touched) { touched = true; fs.writeFileSync(path.join(f.source, "AI-late-user-file"), "late"); } return r; } });
  blocked(() => f.create(io), "SNAPSHOT_SOURCE_GIT_CHANGED"); assert(fs.existsSync(f.target));
}));
for (const change of ["extra-file", "missing-file", "extra-dir", "missing-empty-dir"]) test("candidate coverage rejects " + change, () => fixture(f => {
  const p = f.plan(); f.create(); if (change === "extra-file") fs.writeFileSync(path.join(f.target, "extra"), "unread-extra");
  if (change === "missing-file") fs.unlinkSync(path.join(f.target, f.manifest.files[0].path));
  if (change === "extra-dir") fs.mkdirSync(path.join(f.target, "extra")); if (change === "missing-empty-dir") fs.rmdirSync(path.join(f.target, "Assets/Empty"));
  blocked(() => snapshot.verify(p), "SNAPSHOT_COVERAGE");
}));
test("partial enumeration cannot silently pass", () => fixture(f => {
  const p = f.plan(); f.create(); const io = ioWith({ readdirSync(p) { return fs.readdirSync(p).filter(x => x !== "global.json"); } }); blocked(() => snapshot.verify(p, io), "SNAPSHOT_COVERAGE");
}));
test("enumeration denial fails with sanitized diagnostic", () => fixture(f => { const p = f.plan(); f.create(); blocked(() => snapshot.verify(p, ioWith({ readdirSync() { throw Error("private"); } })), "SNAPSHOT_IO_FAILED_PARTIAL_RETAINED"); }));
for (const kind of ["source-parent", "target-parent", "file-link"]) test("junction/symlink rejected: " + kind, () => fixture(f => {
  const linked = path.join(f.base, "linked"); fs.symlinkSync(f.source, linked, process.platform === "win32" ? "junction" : "dir");
  if (kind === "source-parent") blocked(() => snapshot.prepare(linked, f.manifest), "SNAPSHOT_LINK");
  if (kind === "target-parent") { f.manifest.candidateRoot = path.join(linked, "new"); blocked(() => f.plan(), "SNAPSHOT_LINK"); }
  if (kind === "file-link") { f.manifest.files.push(row("Assets/link/a", "x")); fs.symlinkSync(f.source, path.join(f.source, "Assets/link"), process.platform === "win32" ? "junction" : "dir"); blocked(() => f.plan(), "SNAPSHOT_LINK"); }
}));
test("reviewed six-source exception is exact and digest-pinned; global reader remains blocked", () => {
  const root = path.resolve(__dirname, "../.."), input = JSON.parse(fs.readFileSync(path.join(root, "AI/90_Tasks/Prototype_10/20261120_08_Phase3Step2InputAudit.json")));
  for (const f of input.narrowReadReview.rows) { blocked(() => audit.reader(root).read(f.path), "SENSITIVE_READ_BLOCKED"); assert.equal(sha(snapshot.readSource(root, f.path)), f.sha256); }
  blocked(() => snapshot.readSource(root, "UGS/CloudCode/not-reviewed-secret.js"));
});
test("reviewed source replacement cannot reuse exception", () => fixture(f => { f.write("UGS/CloudCode/receipt-store.js", "synthetic poisoned source"); blocked(() => snapshot.readSource(f.source, "UGS/CloudCode/receipt-store.js"), "SNAPSHOT_REVIEWED_SOURCE_CHANGED"); }));
test("private inputs limited to pinned schema2 bundle and 27 exact tgz, not whole caches", () => {
  const rows = snapshot.privateInputs(path.resolve(__dirname, "../..")); assert.equal(rows.length, 111); assert.equal(rows.filter(r => r.path.endsWith(".tgz")).length, 27);
  assert(rows.every(r => r.path.startsWith("Ignore/DependencyPreservation/tmp-baseline/29ef") || r.path.startsWith("Ignore/DependencyArchives/npm/com.unity.services.cloudcode/2.10.4/")));
});
test("private catalog refuses unpinned evidence", () => fixture(f => { f.write("AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json", "{}"); blocked(() => snapshot.privateInputs(f.source), "SNAPSHOT_PRIVATE_EVIDENCE"); }));
test("source HEAD/index and unstaged/untracked bytes preserved by prepare/create", () => fixture(f => {
  const git = args => cp.execFileSync("git", ["-c", "core.autocrlf=false", "-c", "commit.gpgsign=false", "-C", f.source, ...args], { windowsHide: true, stdio: "pipe" });
  git(["init", "--quiet"]); git(["add", "Assets"]); git(["-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "fixture"]);
  f.manifest.files.push(f.write("AI/untracked.md", "uncommitted evidence"));
  const before = sha(fs.readFileSync(path.join(f.source, ".git/index"))), head = git(["rev-parse", "HEAD"]).toString(); f.create();
  assert.equal(sha(fs.readFileSync(path.join(f.source, ".git/index"))), before); assert.equal(git(["rev-parse", "HEAD"]).toString(), head); assert.equal(fs.readFileSync(path.join(f.target, "AI/untracked.md"), "utf8"), "uncommitted evidence");
}));
test("unborn source repository is not empty successful Git verification", () => fixture(f => { cp.execFileSync("git", ["init", "--quiet", f.source], { windowsHide: true }); blocked(() => f.plan(), "GIT_READ_FAILED"); }));
test("read-only require performs no filesystem writes or processes", () => {
  const script = "const fs=require('fs'),cp=require('child_process');for(const k of ['writeFileSync','mkdirSync','rmSync','unlinkSync'])fs[k]=()=>{throw Error('write')};for(const k of ['spawnSync','execFileSync'])cp[k]=()=>{throw Error('process')};require(process.argv[1]);";
  const r = cp.spawnSync(process.execPath, ["-e", script, path.join(__dirname, "dependency-snapshot.cjs")], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 0, r.stderr); assert.equal(r.stdout, "");
});
test("prepare never invokes writes even when APIs are injected to fail", () => fixture(f => { const io = ioWith({ mkdirSync() { throw Error("write"); }, writeFileSync() { throw Error("write"); } }); assert(f.plan(io).planSha256); }));
test("CLI rejects write modes and process failure is nonzero without body/input disclosure", () => {
  const r = cp.spawnSync(process.execPath, [path.join(__dirname, "dependency-snapshot.cjs"), "--create", "private-body"], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 2); assert(!r.stdout.includes("private-body")); assert.equal(JSON.parse(r.stdout).diagnostics[0].code, "CLI_ARGUMENT");
});
test("permitted modifications/additions/deletions compared against exact before/after digests", () => {
  const b = [row("package.json", "old"), row("old.txt", "old")], now = [row("package.json", "new"), row("new.txt", "added")];
  const c = [{ path: "package.json", beforeSha256: sha("old"), afterSha256: sha("new") }, { path: "old.txt", beforeSha256: sha("old"), afterSha256: null }, { path: "new.txt", beforeSha256: null, afterSha256: sha("added") }];
  assert(snapshot.compare(b, now, c).matches); now[0].sha256 = sha("late"); assert(!snapshot.compare(b, now, c).matches);
});
test("undeclared extra/missing or wrong baseline changes never pass", () => {
  const b = [row("a", "a")]; assert(!snapshot.compare(b, []).matches); assert(!snapshot.compare(b, [...b, row("b", "b")]).matches);
  blocked(() => snapshot.compare(b, b, [{ path: "a", beforeSha256: sha("wrong"), afterSha256: sha("a") }]), "SNAPSHOT_CHANGE_BASELINE"); blocked(() => snapshot.compare(b, [...b, ...b]), "SNAPSHOT_DUPLICATE");
});
test("rollback plan names exact partial restores, retains new files and never writes", () => {
  const before = [row("a", "old"), row("b", "same")], attempt = [row("a", "attempt"), row("b", "same"), row("c", "new")], current = [...attempt];
  const r = snapshot.rollbackPlan(before, attempt, current, before); assert(r.readyForReviewedRollback); assert.equal(r.actions.find(a => a.path === "a").action, "restore-verified-backup"); assert.equal(r.actions.find(a => a.path === "b").action, "unchanged"); assert.equal(r.actions.find(a => a.path === "c").action, "retain-new-output"); assert.equal(r.writesPerformed, false);
});
test("late user changes and wrong backup block rollback", () => {
  const b = [row("a", "old")], t = [row("a", "attempt")]; assert(!snapshot.rollbackPlan(b, t, [row("a", "late")], b).readyForReviewedRollback); blocked(() => snapshot.rollbackPlan(b, t, t, t), "SNAPSHOT_BACKUP_MISMATCH");
});
test("required user outcomes need execution same input zero exit and no unexpected logs", () => {
  const required = [{ name: "npm-install", inputSha256: sha("input") }], good = { ...required[0], executed: true, exitCode: 0, unexpectedLogCount: 0 };
  assert(snapshot.assessResults(required, [good]).requiredResultsSatisfied); assert(!snapshot.assessResults(required, []).requiredResultsSatisfied);
  for (const change of [{ executed: false }, { exitCode: 1 }, { inputSha256: sha("other") }, { unexpectedLogCount: 1 }]) assert(!snapshot.assessResults(required, [{ ...good, ...change }]).requiredResultsSatisfied);
  blocked(() => snapshot.assessResults(required, [good, good]), "SNAPSHOT_RESULTS"); blocked(() => snapshot.assessResults(required, [{ ...good, name: "unknown" }]), "SNAPSHOT_RESULTS");
});
let passed = 0; for (const item of tests) { try { item.run(); passed++; } catch (e) { console.error("FAIL " + item.name); throw e; } }
console.log(JSON.stringify({ suite: "DependencySnapshot", passed, failed: 0, unityExecuted: false, realCandidateCreated: false, realIndexChanged: false }));
