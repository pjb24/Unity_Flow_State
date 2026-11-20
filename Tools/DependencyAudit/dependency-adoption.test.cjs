"use strict";
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path"), os = require("node:os"), crypto = require("node:crypto"), cp = require("node:child_process");
const adoption = require("./dependency-adoption.cjs"), audit = require("./dependency-audit.cjs");
const tests = [], test = (name, run) => tests.push({ name, run }), sha = b => crypto.createHash("sha256").update(b).digest("hex");
const throws = (fn, code) => assert.throws(fn, e => e.code === code);
const guidA = "a".repeat(32), guidB = "b".repeat(32), guidC = "c".repeat(32);
function fixture() {
  const archiveHash = sha("synthetic archive"), sourceRoot = "Ignore/DependencySources/synthetic-asset/" + archiveHash;
  const source = sourceRoot + "/payload/Item.asset", target = "Assets/ThirdParty/synthetic/product/Item.asset";
  const plan = { schemaVersion: 1, asset_id: "synthetic-asset", version: "1.0.0", supplier: "synthetic", product: "product", sourceRoot, archivePath: sourceRoot + "/source.archive", archiveSha256: archiveHash,
    selectedSources: [source], mappings: [{ source, target, kind: "original", sourceSha256: sha("synthetic asset"), sourceMetaSha256: sha("guid: " + guidA + "\n"), sourceGuid: guidA, targetGuid: guidA, references: [], usageField: "game_usage" }] };
  const fields = Object.fromEntries(audit.requiredFields.map(k => [k, "synthetic review task for " + k]));
  fields.version = "1.0.0"; fields.evidence = "sha256:" + archiveHash;
  const record = { id: "synthetic-asset", fields, status: { rights: { state: "근거 확인" }, boundary: { state: "확정" }, decision: { state: "exclude-candidate" } }, boundaries: [{ path: "Assets/ThirdParty/synthetic/product", kind: "original", action: "preserve", evidence: "synthetic source comparison", meta: { evidence: "synthetic meta contract" }, conditions: "synthetic terms", usage: "synthetic use" }] };
  const files = [{ path: plan.archivePath, sha256: archiveHash }, { path: source, sha256: sha("synthetic asset"), metaSha256: plan.mappings[0].sourceMetaSha256, guid: guidA, references: [] }];
  return { plan, record, files, existingMeta: [], check() { return adoption.validate(this.record, this.plan, this.files, this.existingMeta); } };
}
test("original selection preflight does not copy/apply or grant rights", () => { const f = fixture(), r = f.check(); assert.equal(r.preflightValid, true); for (const k of ["applied", "sourceWrites", "rightsGranted", "independentRestore"]) assert.equal(r[k], false); });
test("project derivative uses a distinct root and new GUID", () => { const f = fixture(); Object.assign(f.plan.mappings[0], { target: "Assets/ProjectAssets/synthetic-asset/Preset.asset", kind: "derived", targetGuid: guidB }); Object.assign(f.record.boundaries[0], { path: "Assets/ProjectAssets/synthetic-asset", kind: "derived" }); assert.equal(f.check().preflightValid, true); });
test("derivative cannot clone supplier GUID", () => { const f = fixture(); Object.assign(f.plan.mappings[0], { target: "Assets/ProjectAssets/synthetic-asset/Preset.asset", kind: "derived" }); throws(() => f.check(), "ADOPTION_GUID_COPY"); });
test("derivative cannot clone a different supplier component GUID", () => {
  const f = fixture(); f.files.push({ path: f.plan.sourceRoot + "/payload/Other.asset", guid: guidB });
  Object.assign(f.plan.mappings[0], { target: "Assets/ProjectAssets/synthetic-asset/Preset.asset", kind: "derived", targetGuid: guidB });
  Object.assign(f.record.boundaries[0], { path: "Assets/ProjectAssets/synthetic-asset", kind: "derived" }); throws(() => f.check(), "ADOPTION_GUID_COPY");
});
test("original GUID cannot be silently replaced", () => { const f = fixture(); f.plan.mappings[0].targetGuid = guidB; throws(() => f.check(), "ADOPTION_GUID_COPY"); });
test("cannot write back to source archive/storage", () => { const f = fixture(); f.plan.mappings[0].target = f.plan.selectedSources[0]; throws(() => f.check(), "ADOPTION_TARGET_ROOT"); });
test("original cannot go into project extension root", () => { const f = fixture(); f.plan.mappings[0].target = "Assets/ProjectAssets/synthetic-asset/Original.asset"; throws(() => f.check(), "ADOPTION_TARGET_ROOT"); });
test("cannot hide modified content under provider root", () => { const f = fixture(); f.plan.mappings[0].kind = "modified"; throws(() => f.check(), "ADOPTION_TARGET_ROOT"); });
test("unknown content cannot be imported by assumption", () => { const f = fixture(); f.plan.mappings[0].kind = "unknown"; throws(() => f.check(), "ADOPTION_KIND_PENDING"); });
test("selected source omission rejected", () => { const f = fixture(); f.plan.selectedSources.push(f.plan.sourceRoot + "/payload/Needed.asset"); throws(() => f.check(), "ADOPTION_SELECTION_MISSING"); });
test("mapping outside explicit selection rejected", () => { const f = fixture(); f.plan.mappings[0].source += "other"; throws(() => f.check(), "ADOPTION_MAPPING"); });
test("empty selection rejected", () => { const f = fixture(); f.plan.selectedSources = []; throws(() => f.check(), "ADOPTION_EMPTY_SELECTION"); });
test("archive and source hashes pinned to current bytes and record", () => {
  const f = fixture(); f.files[0].sha256 = "b".repeat(64); throws(() => f.check(), "ADOPTION_ARCHIVE_HASH");
  const g = fixture(); g.files[1].sha256 = "b".repeat(64); throws(() => g.check(), "ADOPTION_SOURCE_INPUT");
  const h = fixture(); h.record.fields.evidence = "sha256:" + "c".repeat(64); throws(() => h.check(), "ADOPTION_ARCHIVE_RECORD_HASH");
});
test("version or record ID mismatch rejected", () => { const f = fixture(); f.plan.version = "2.0.0"; throws(() => f.check(), "ADOPTION_RECORD_VERSION"); });
test("unchanged GUID cannot hide modified Import metadata", () => { const f = fixture(); f.files[1].metaSha256 = "b".repeat(64); throws(() => f.check(), "ADOPTION_SOURCE_INPUT"); });
test("observed source/Importer references cannot be omitted from plan", () => { const f = fixture(); f.files[1].references.push(guidB); throws(() => f.check(), "ADOPTION_REFERENCE_UNDECLARED"); });
test("unknown terms/boundary and missing use/notices rejected", () => {
  const f = fixture(); f.record.status.rights.state = "미확인"; throws(() => f.check(), "ADOPTION_RIGHTS_BOUNDARY_PENDING");
  for (const field of ["purpose", "game_usage", "references"]) { const g = fixture(); g.record.fields[field] = "미확인: pending"; throws(() => g.check(), "ADOPTION_RECORD_EVIDENCE"); }
  const h = fixture(); h.record.fields.notices = "미확인: attribution pending"; throws(() => h.check(), "ADOPTION_CONDITIONS_PENDING");
});
test("record path/role must match target component", () => { const f = fixture(); f.record.boundaries[0].kind = "project"; throws(() => f.check(), "ADOPTION_BOUNDARY_LINK"); });
test("unapproved modified exclusion rejected", () => { const f = fixture(); Object.assign(f.plan.mappings[0], { target: "Assets/ProjectAssets/synthetic-asset/Changed.asset", kind: "modified", targetGuid: guidB }); Object.assign(f.record.boundaries[0], { path: "Assets/ProjectAssets/synthetic-asset", kind: "modified", action: "exclude-candidate" }); throws(() => f.check(), "ADOPTION_MODIFIED_EXCLUSION"); });
test("exclusion candidate depends on production registry evaluator", () => { const f = fixture(); f.record.boundaries[0].action = "exclude-candidate"; f.record.status.decision.state = "pending"; throws(() => f.check(), "ADOPTION_EXCLUSION_BLOCKED"); });
test("second version already using same GUID rejected", () => { const f = fixture(); f.existingMeta.push({ path: "Assets/ThirdParty/old/product/Item.asset", guid: guidA }); throws(() => f.check(), "ADOPTION_GUID_COLLISION"); });
test("pre-existing duplicate GUIDs block complete preflight", () => { const f = fixture(); f.existingMeta.push({ path: "Assets/A.asset", guid: guidC }, { path: "Assets/B.asset", guid: guidC }); throws(() => f.check(), "ADOPTION_EXISTING_GUID"); });
test("existing asset or orphan target meta cannot be overwritten", () => { for (const suffix of ["", ".meta"]) { const f = fixture(); f.files.push({ path: f.plan.mappings[0].target + suffix }); throws(() => f.check(), "ADOPTION_OVERWRITE"); } });
test("duplicate and case-colliding destinations rejected", () => { const f = fixture(); f.plan.mappings.push({ ...f.plan.mappings[0], target: f.plan.mappings[0].target.toLowerCase() }); throws(() => f.check(), "ADOPTION_TARGET_COLLISION"); });
test("unselected required reference fails; mapped/existing reference passes", () => { const f = fixture(); f.plan.mappings[0].references.push(guidB); throws(() => f.check(), "ADOPTION_REFERENCE_MISSING"); f.existingMeta.push({ path: "Assets/ProjectAssets/Shared.asset", guid: guidB }); assert.equal(f.check().preflightValid, true); });
for (const bad of ["../outside", "C:/outside", "Assets/../outside", "Assets/ThirdParty/synthetic/product/trailing. ", "Assets/ThirdParty/synthetic/product/*"]) test("unsafe destination " + bad, () => { const f = fixture(); f.plan.mappings[0].target = bad; assert.throws(() => f.check()); });
function onDisk(run) {
  const base = fs.realpathSync(os.tmpdir()), root = fs.mkdtempSync(path.join(base, "flowstate-adoption-"));
  const write = (p, bytes) => { fs.mkdirSync(path.dirname(path.join(root, p)), { recursive: true }); fs.writeFileSync(path.join(root, p), bytes); };
  const git = (...args) => cp.execFileSync("git", ["-c", "core.fsmonitor=false", "-c", "core.hooksPath=" + path.join(root, "empty-hooks"), "-C", root, ...args], { encoding: "utf8", windowsHide: true, stdio: ["ignore", "pipe", "pipe"] });
  try {
    const f = fixture(); fs.mkdirSync(path.join(root, "Assets")); write(".gitignore", "/Ignore/\n"); write("README.md", "synthetic fixture"); git("init", "--quiet"); git("add", "."); git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "synthetic");
    write(f.plan.archivePath, "synthetic archive"); write(f.plan.selectedSources[0], "synthetic asset"); write(f.plan.selectedSources[0] + ".meta", "guid: " + guidA + "\n");
    run({ f, root, write, git });
  } finally { const resolved = path.resolve(root); assert.equal(path.dirname(resolved).toLowerCase(), path.resolve(base).toLowerCase()); assert(path.basename(resolved).startsWith("flowstate-adoption-")); fs.rmSync(resolved, { recursive: true, force: true }); }
}
test("temporary file inspection reads actual hash/meta and leaves source/index unchanged", () => onDisk(({ f, root }) => {
  const before = fs.readFileSync(path.join(root, ".git/index")), inputs = adoption.inspect(root, f.plan);
  assert.equal(adoption.validate(f.record, f.plan, inputs.files, inputs.existingMeta).preflightValid, true);
  assert.equal(inputs.storageIgnoredUntracked, true); assert.deepEqual(fs.readFileSync(path.join(root, ".git/index")), before);
  assert.equal(fs.existsSync(path.join(root, f.plan.mappings[0].target)), false);
}));
test("missing source meta and duplicate meta declarations fail", () => onDisk(({ f, root, write }) => {
  write(f.plan.selectedSources[0] + ".meta", "guid: " + guidA + "\nguid: " + guidB + "\n"); throws(() => adoption.inspect(root, f.plan), "ADOPTION_SOURCE_META");
  fs.unlinkSync(path.join(root, f.plan.selectedSources[0] + ".meta")); assert.throws(() => adoption.inspect(root, f.plan));
}));
test("actual serialized source GUIDs are read and compared with declared selection", () => onDisk(({ f, root, write }) => {
  const body = "%YAML 1.1\nvalue: {fileID: 11400000, guid: " + guidB + ", type: 2}\n";
  write(f.plan.selectedSources[0], body); f.plan.mappings[0].sourceSha256 = sha(body);
  write("Assets/Shared.asset", "synthetic shared"); write("Assets/Shared.asset.meta", "guid: " + guidB + "\n");
  const inputs = adoption.inspect(root, f.plan);
  throws(() => adoption.validate(f.record, f.plan, inputs.files, inputs.existingMeta), "ADOPTION_REFERENCE_UNDECLARED");
  f.plan.mappings[0].references.push(guidB);
  assert.equal(adoption.validate(f.record, f.plan, inputs.files, inputs.existingMeta).preflightValid, true);
}));
test("storage must be ignored and not force-staged", () => onDisk(({ f, root, git, write }) => {
  git("add", "-f", f.plan.archivePath); throws(() => adoption.inspect(root, f.plan), "ADOPTION_STORAGE_NOT_PRIVATE");
  git("reset", "--quiet", "HEAD", "--", f.plan.archivePath); write(".gitignore", ""); throws(() => adoption.inspect(root, f.plan), "ADOPTION_STORAGE_NOT_PRIVATE");
}));
test("source parent junction is rejected", () => onDisk(({ f, root }) => {
  const parent = path.join(root, f.plan.sourceRoot), actual = path.join(parent, "actual"); fs.renameSync(path.join(parent, "payload"), actual);
  fs.symlinkSync(actual, path.join(parent, "payload"), "junction"); throws(() => adoption.inspect(root, f.plan), "SCOPE_LINK");
}));
test("require has no process or filesystem mutation side effects", () => {
  const script = "const fs=require('fs'),cp=require('child_process'); for(const k of ['writeFileSync','mkdirSync','rmSync','unlinkSync','renameSync'])fs[k]=()=>{throw Error('write')}; for(const k of ['spawnSync','execFileSync','spawn'])cp[k]=()=>{throw Error('process')};require(process.argv[1]);";
  const r = cp.spawnSync(process.execPath, ["-e", script, path.join(__dirname, "dependency-adoption.cjs")], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 0, r.stderr); assert.equal(r.stdout, "");
});
let passed = 0; for (const item of tests) { try { item.run(); passed++; } catch (e) { console.error("FAIL " + item.name); throw e; } }
console.log(JSON.stringify({ suite: "DependencyAdoption", passed, failed: 0, unityExecuted: false, realIndexChanged: false }));
