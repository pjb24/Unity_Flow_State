"use strict";
const fs = require("node:fs"), path = require("node:path"), os = require("node:os"), crypto = require("node:crypto"), cp = require("node:child_process"), assert = require("node:assert/strict");
const p = require("./dependency-preservation.cjs"), a = require("./dependency-audit.cjs");
const tests = [], test = (name, run) => tests.push({ name, run }), hash = b => crypto.createHash("sha256").update(b).digest("hex");
const throws = (fn, code) => assert.throws(fn, e => e.code === code);
function cleanup(root, prefix) {
  const base = fs.realpathSync(os.tmpdir()), full = path.resolve(root);
  assert.equal(path.dirname(full).toLowerCase(), base.toLowerCase()); assert(path.basename(full).startsWith(prefix));
  fs.rmSync(full, { recursive: true, force: true });
}
function fixture(run) {
  const root = fs.mkdtempSync(path.join(fs.realpathSync(os.tmpdir()), "flowstate-preservation-test-"));
  const write = (rel, data) => { fs.mkdirSync(path.dirname(path.join(root, rel)), { recursive: true }); fs.writeFileSync(path.join(root, rel), data); };
  const git = (...args) => cp.execFileSync("git", ["-c", "core.fsmonitor=false", "-c", "core.hooksPath=" + path.join(root, "empty-hooks"), "-C", root, ...args], { encoding: "utf8", windowsHide: true, stdio: ["ignore", "pipe", "pipe"] });
  try {
    write(".gitignore", "/Ignore/\n/Library/\n/Assets/TextMesh Pro/\n/Assets/TextMesh Pro.meta\n");
    write("ProjectSettings/ProjectVersion.txt", "m_EditorVersion: synthetic-editor\n");
    const providerRoot = "Library/PackageCache/synthetic-provider", archivePath = providerRoot + "/Package Resources/TMP Essential Resources.unitypackage";
    write(providerRoot + "/package.json", JSON.stringify({ name: "com.unity.ugui", version: "2.0.0" })); write(archivePath, "synthetic archive");
    const content = { "Assets/TextMesh Pro.meta": "guid: " + "1".repeat(32) + "\nfolderAsset: yes\n", "Assets/TextMesh Pro/Resources.meta": "guid: " + "2".repeat(32) + "\nfolderAsset: yes\n",
      "Assets/TextMesh Pro/Resources/Font.asset": "font: synthetic changed bytes\n", "Assets/TextMesh Pro/Resources/Font.asset.meta": "guid: " + "3".repeat(32) + "\nImporter: synthetic\n",
      "Assets/TextMesh Pro/Resources/TMP Settings.asset": "settings: synthetic\n", "Assets/TextMesh Pro/Resources/TMP Settings.asset.meta": "guid: " + "4".repeat(32) + "\n",
      "Assets/TextMesh Pro/Resources/Material.mat": "material: synthetic\n", "Assets/TextMesh Pro/Resources/Material.mat.meta": "guid: " + "5".repeat(32) + "\n",
      "Assets/TextMesh Pro/Shader.shader": 'Shader "Synthetic/StableName" {}\n', "Assets/TextMesh Pro/Shader.shader.meta": "guid: " + "6".repeat(32) + "\n" };
    for (const [rel, bytes] of Object.entries(content)) write(rel, bytes);
    const spec = { schemaVersion: 1, id: "tmp-fixture", editorVersion: "synthetic-editor", provider: { id: "com.unity.ugui", version: "2.0.0", archivePath, archiveSha256: hash("synthetic archive") }, files: [...Object.keys(content), archivePath].map(rel => ({ path: rel, sha256: hash(fs.readFileSync(path.join(root, rel))) })) };
    git("init", "--quiet"); git("add", "."); git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "synthetic");
    run({ root, write, git, spec, create() { const prepared = p.prepare(root, spec); p.create(root, spec, prepared.output); return prepared; } });
  } finally { cleanup(root, "flowstate-preservation-test-"); }
}
test("prepare is read-only; explicit create preserves source/index and names", () => fixture(({ root, spec }) => {
  const before = fs.readFileSync(path.join(root, ".git/index")), prepared = p.prepare(root, spec);
  assert.equal(fs.existsSync(path.join(root, prepared.output)), false);
  const r = p.create(root, spec, prepared.output); assert.equal(r.verified, true); assert.equal(r.independentRestore, false);
  for (const f of spec.files) assert.equal(hash(fs.readFileSync(path.join(root, f.path))), f.sha256);
  assert.deepEqual(fs.readFileSync(path.join(root, ".git/index")), before);
  assert(prepared.manifest.files.some(f => f.resourcesName === "Font")); assert(prepared.manifest.files.some(f => f.shaderNames.includes("Synthetic/StableName")));
}));
test("missing/dirty preserved files fail hash/coverage", () => fixture(({ root, create }) => {
  const prepared = create(), rel = prepared.output + "/files/Assets/TextMesh Pro/Resources/Font.asset";
  fs.writeFileSync(path.join(root, rel), "corrupt"); throws(() => p.verify(root, prepared.output, prepared.expected), "PRESERVATION_PAYLOAD_CHANGED");
  fs.unlinkSync(path.join(root, rel)); throws(() => p.verify(root, prepared.output, prepared.expected), "PRESERVATION_BUNDLE_COVERAGE");
}));
test("wrong version/ID or manifest GUID tampering rejected", () => fixture(({ root, create }) => {
  const prepared = create(); throws(() => p.verify(root, prepared.output, { ...prepared.expected, providerVersion: "9.0.0" }), "PRESERVATION_IDENTITY");
  throws(() => p.verify(root, prepared.output, { ...prepared.expected, id: "different-id" }), "PRESERVATION_EXPECTED_OUTPUT");
  const manifestPath = path.join(root, prepared.output, "manifest.json"); const m = JSON.parse(fs.readFileSync(manifestPath)); m.files.find(f => f.guid).guid = "f".repeat(32); fs.writeFileSync(manifestPath, JSON.stringify(m));
  throws(() => p.verify(root, prepared.output, prepared.expected), "PRESERVATION_MANIFEST_HASH");
}));
test("same ID different bytes cannot reuse existing output", () => fixture(({ root, spec, write, create }) => {
  const prepared = create(), font = spec.files.find(f => f.path.endsWith("Font.asset")); write(font.path, "new synthetic content"); font.sha256 = hash("new synthetic content");
  throws(() => p.create(root, spec, prepared.output), "PRESERVATION_EXPECTED_OUTPUT");
  assert.notEqual(p.prepare(root, spec).output, prepared.output);
}));
test("no overwrite on repeated create", () => fixture(({ root, spec, create }) => { const prepared = create(); throws(() => p.create(root, spec, prepared.output), "PRESERVATION_OVERWRITE"); }));
test("settings/material omitted from spec fail completeness", () => fixture(({ root, spec }) => { spec.files = spec.files.filter(f => !f.path.endsWith("Material.mat")); throws(() => p.prepare(root, spec), "PRESERVATION_COVERAGE"); }));
test("source/settings/name changes detected before preservation", () => fixture(({ root, spec, write }) => { write("Assets/TextMesh Pro/Shader.shader", 'Shader "Synthetic/ChangedName" {}'); throws(() => p.prepare(root, spec), "PRESERVATION_SOURCE_HASH"); }));
test("wrong Editor/provider/archive inputs rejected", () => fixture(({ root, spec }) => {
  const bad = structuredClone(spec); bad.editorVersion = "wrong"; throws(() => p.prepare(root, bad), "PRESERVATION_EDITOR_VERSION");
  const bad2 = structuredClone(spec); bad2.provider.version = "9.0.0"; throws(() => p.prepare(root, bad2), "PRESERVATION_PROVIDER_VERSION");
  const bad3 = structuredClone(spec); bad3.provider.archiveSha256 = "a".repeat(64); throws(() => p.prepare(root, bad3), "PRESERVATION_ARCHIVE_HASH");
}));
test("temporary reapply survives missing originals and preserves exact metadata", () => fixture(({ root, create }) => {
  const prepared = create(), target = fs.mkdtempSync(path.join(fs.realpathSync(os.tmpdir()), "flowstate-preservation-reapply-"));
  try {
    fs.unlinkSync(path.join(root, "Assets/TextMesh Pro/Resources/Font.asset"));
    assert.equal(p.verify(root, prepared.output, prepared.expected).verified, true);
    const r = p.reapplyToTemporary(root, prepared.output, prepared.expected, target); assert.equal(r.reappliedToTemporary, true); assert.equal(r.independentRestore, false);
    throws(() => p.reapplyToTemporary(root, prepared.output, prepared.expected, target), "PRESERVATION_TEMP_TARGET");
  } finally { cleanup(target, "flowstate-preservation-reapply-"); }
}));
test("permission/partial copy fails without complete manifest or overwrite retry", () => fixture(({ root, spec }) => {
  const prepared = p.prepare(root, spec); let writes = 0;
  const io = new Proxy(fs, { get(target, key) { if (key === "writeFileSync") return (...args) => { if (++writes === 2) throw Error("synthetic permission failure"); return fs.writeFileSync(...args); }; return target[key]; } });
  throws(() => p.create(root, spec, prepared.output, io), "PRESERVATION_COPY_FAILED_PARTIAL_RETAINED");
  assert.equal(fs.existsSync(path.join(root, prepared.output, "manifest.json")), false);
  assert.throws(() => p.verify(root, prepared.output, prepared.expected)); throws(() => p.create(root, spec, prepared.output), "PRESERVATION_OVERWRITE");
  for (const f of spec.files) assert.equal(hash(fs.readFileSync(path.join(root, f.path))), f.sha256);
}));
test("temporary reapply partial failure keeps source bundle verifiable", () => fixture(({ root, create }) => {
  const prepared = create(), target = fs.mkdtempSync(path.join(fs.realpathSync(os.tmpdir()), "flowstate-preservation-reapply-"));
  try { const io = new Proxy(fs, { get(obj, key) { if (key === "writeFileSync") return () => { throw Error("denied"); }; return obj[key]; } });
    throws(() => p.reapplyToTemporary(root, prepared.output, prepared.expected, target, io), "PRESERVATION_REAPPLY_FAILED_PARTIAL_RETAINED"); assert.equal(p.verify(root, prepared.output, prepared.expected).verified, true);
  } finally { cleanup(target, "flowstate-preservation-reapply-"); }
}));
test("existing output parent junction rejected", () => fixture(({ root, spec }) => {
  const prepared = p.prepare(root, spec), ignore = path.join(root, "Ignore"), other = path.join(root, "Other"); fs.mkdirSync(ignore); fs.mkdirSync(other); fs.symlinkSync(other, path.join(ignore, "DependencyPreservation"), "junction");
  throws(() => p.create(root, spec, prepared.output), "PRESERVATION_LINK");
}));
test("source parent junction and malformed/duplicate source GUID rejected", () => fixture(({ root, spec, write }) => {
  const meta = "Assets/TextMesh Pro/Resources/Font.asset.meta"; write(meta, "guid: " + "4".repeat(32) + "\n"); spec.files.find(f => f.path === meta).sha256 = hash(fs.readFileSync(path.join(root, meta)));
  throws(() => p.prepare(root, spec), "PRESERVATION_DUPLICATE_GUID");
  const resources = path.join(root, "Assets/TextMesh Pro/Resources"), actual = path.join(root, "Actual"); fs.renameSync(resources, actual); fs.symlinkSync(actual, resources, "junction"); throws(() => p.prepare(root, spec), "PRESERVATION_ENUMERATION");
}));
test("ignored output cannot contain force-staged files", () => fixture(({ root, spec, git, write }) => {
  const prepared = p.prepare(root, spec); write(prepared.output + "/forced.txt", "synthetic"); git("add", "-f", prepared.output + "/forced.txt"); throws(() => p.prepare(root, spec), "PRESERVATION_NOT_PRIVATE");
}));
test("unignored storage rejected", () => fixture(({ root, spec, write }) => { write(".gitignore", "/Library/\n/Assets/TextMesh Pro/\n"); throws(() => p.prepare(root, spec), "PRESERVATION_NOT_PRIVATE"); }));
test("external/traversal output and production reapply target rejected", () => fixture(({ root, spec, create }) => {
  const prepared = create(); for (const rel of ["../outside", "C:/outside", "Assets/Preservation"]) assert.throws(() => p.create(root, spec, rel));
  throws(() => p.reapplyToTemporary(root, prepared.output, prepared.expected, root), "PRESERVATION_TEMP_TARGET");
}));
test("extra bundle payload is not silently accepted", () => fixture(({ root, write, create }) => { const prepared = create(); write(prepared.output + "/extra.bin", "synthetic"); throws(() => p.verify(root, prepared.output, prepared.expected), "PRESERVATION_BUNDLE_COVERAGE"); }));
test("empty folder meta restores its actual directory and missing preserved folder fails", () => fixture(({ root, spec, write, create }) => {
  const folder = "Assets/TextMesh Pro/Resources/Empty";
  fs.mkdirSync(path.join(root, folder)); write(folder + ".meta", "guid: " + "7".repeat(32) + "\nfolderAsset: yes\n");
  spec.files.push({ path: folder + ".meta", sha256: hash(fs.readFileSync(path.join(root, folder + ".meta"))) });
  const prepared = create(), target = fs.mkdtempSync(path.join(fs.realpathSync(os.tmpdir()), "flowstate-preservation-reapply-"));
  try { p.reapplyToTemporary(root, prepared.output, prepared.expected, target); assert(fs.lstatSync(path.join(target, folder)).isDirectory()); }
  finally { cleanup(target, "flowstate-preservation-reapply-"); }
  fs.rmdirSync(path.join(root, prepared.output, "files", folder)); throws(() => p.verify(root, prepared.output, prepared.expected), "PRESERVATION_FOLDER_MISSING");
}));
test("orphan source folder meta cannot be treated as successful preservation", () => fixture(({ root, spec, write }) => {
  const meta = "Assets/TextMesh Pro/Orphan.meta"; write(meta, "guid: " + "8".repeat(32) + "\nfolderAsset: yes\n"); spec.files.push({ path: meta, sha256: hash(fs.readFileSync(path.join(root, meta))) });
  throws(() => p.prepare(root, spec), "PRESERVATION_META_PAIR");
}));
test("require has no side effects and CLI errors never echo input values", () => {
  const script = "const fs=require('fs'),cp=require('child_process');for(const k of ['writeFileSync','mkdirSync','rmSync','unlinkSync'])fs[k]=()=>{throw Error('write')};for(const k of ['spawnSync','execFileSync'])cp[k]=()=>{throw Error('process')};require(process.argv[1]);";
  const r = cp.spawnSync(process.execPath, ["-e", script, path.join(__dirname, "dependency-preservation.cjs")], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 0, r.stderr); assert.equal(r.stdout, "");
  const failure = p.cli(["--private-value"]); assert.equal(failure.exitCode, 2); assert(!JSON.stringify(failure).includes("private-value"));
});
let passed = 0; for (const item of tests) { try { item.run(); passed++; } catch (e) { console.error("FAIL " + item.name); throw e; } }
console.log(JSON.stringify({ suite: "DependencyPreservation", passed, failed: 0, unityExecuted: false, realIndexChanged: false }));
