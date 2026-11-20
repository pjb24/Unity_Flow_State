"use strict";
const fs = require("node:fs"), path = require("node:path"), os = require("node:os"), cp = require("node:child_process"), zlib = require("node:zlib"), crypto = require("node:crypto"), assert = require("node:assert/strict");
const restore = require("./dependency-restore.cjs");
const tests = [], test = (name, run) => tests.push({ name, run }), hash = b => crypto.createHash("sha256").update(b).digest("hex");
const throws = (fn, code) => assert.throws(fn, e => e.code === code);
function tar(files) {
  const rows = [];
  for (const file of files) {
    const body = Buffer.from(file.body), h = Buffer.alloc(512); h.write(file.name); h.write("0000644\0", 100); h.write("0000000\0", 108); h.write("0000000\0", 116); h.write(body.length.toString(8).padStart(11, "0") + "\0", 124); h.write("00000000000\0", 136); h.fill(32, 148, 156); h[156] = (file.type || "0").charCodeAt(0);
    const sum = h.reduce((n, b) => n + b, 0); h.write(sum.toString(8).padStart(6, "0") + "\0 ", 148);
    rows.push(h, body, Buffer.alloc((512 - body.length % 512) % 512));
  }
  rows.push(Buffer.alloc(1024)); return zlib.gzipSync(Buffer.concat(rows));
}
function fixture(run) {
  const base = fs.realpathSync(os.tmpdir()), root = fs.mkdtempSync(path.join(base, "flowstate-restore-"));
  const write = (p, data) => { fs.mkdirSync(path.dirname(path.join(root, p)), { recursive: true }); fs.writeFileSync(path.join(root, p), data); };
  const readJson = p => JSON.parse(fs.readFileSync(path.join(root, p), "utf8"));
  const git = (...args) => cp.execFileSync("git", ["-c", "core.fsmonitor=false", "-c", "core.hooksPath=" + path.join(root, "empty-hooks"), "-C", root, ...args], { encoding: "utf8", windowsHide: true, stdio: ["ignore", "pipe", "pipe"] });
  try {
    write(".gitignore", "/Library/\n/Ignore/\n"); write("ProjectSettings/ProjectVersion.txt", "m_EditorVersion: fixture-editor\n");
    const tmpRoot = "Library/PackageCache/com.unity.ugui@fixture-old", cloudRoot = "Library/PackageCache/com.unity.services.cloudcode@fixture-old", providerDeps = { "com.unity.ugui": "2.0.0", "com.unity.services.cloudcode": "2.10.4" };
    for (const [folder, id, version] of [[tmpRoot, "com.unity.ugui", "2.0.0"], [cloudRoot, "com.unity.services.cloudcode", "2.10.4"]]) write(folder + "/package.json", JSON.stringify({ name: id, version, dependencies: {} }));
    write("Packages/manifest.json", JSON.stringify({ dependencies: providerDeps })); write("Packages/packages-lock.json", JSON.stringify({ dependencies: Object.fromEntries(Object.entries(providerDeps).map(([id, version]) => [id, { version, source: "registry", url: "https://packages.unity.com", dependencies: {} }])) }));
    const tmpArchive = tmpRoot + "/Package Resources/TMP Essential Resources.unitypackage"; write(tmpArchive, "fixture TMP archive");
    const deps = { "lodash-alias": "npm:lodash@^4.17" }, packages = { "node_modules/lodash-alias": { name: "lodash", version: "4.18.1", resolved: "https://registry.npmjs.org/lodash/-/lodash-4.18.1.tgz", integrity: "fixture untouched registry integrity" } }, pins = [];
    for (const version of ["0.0.5", "1.0.1"]) {
      const alias = version === "0.0.5" ? "@unity-services/cloud-save" : "@unity-services/cloud-save-1.0", filename = "cloud-save-" + version + ".tgz", metadata = { name: "@unity-services/cloud-save", version, dependencies: { axios: "^0.21.1" } }, bytes = tar([{ name: "package/package.json", body: JSON.stringify(metadata) }]), rel = cloudRoot + "/UnityServicesNpmLocal~/" + filename;
      write(rel, bytes); const integrity = "sha512-" + crypto.createHash("sha512").update(bytes).digest("base64"); deps[alias] = "file:" + rel.replace(/\//g, "\\"); packages["node_modules/" + alias] = { name: metadata.name, version, resolved: "file:" + rel, integrity, dependencies: metadata.dependencies };
      pins.push({ alias, group: "dependencies", filename, name: metadata.name, version, sha256: hash(bytes), integrity });
    }
    write("package.json", JSON.stringify({ name: "fixture", ccaVersion: "2.10.4", dependencies: deps })); write("package-lock.json", JSON.stringify({ lockfileVersion: 3, packages: { "": { dependencies: deps }, ...packages } }));
    write("UGS/Modules/FlowStateVerification/FlowStateVerification.csproj", '<Project><PropertyGroup><TargetFramework>net9.0</TargetFramework></PropertyGroup><ItemGroup>' + [0, 1, 2, 3].map(i => '<PackageReference Include="Fixture' + i + '" Version="1.0.0" />').join("") + '</ItemGroup><EmbeddedResource Include="../../CloudCode/*.js" LogicalName="server/%(Filename)%(Extension)" /><EmbeddedResource Include="runtime.js" LogicalName="runtime.js" /></Project>');
    write("UGS/CloudCode/fixture.js", "// synthetic fixture source\n"); write("UGS/Modules/FlowStateVerification/runtime.js", "// synthetic runtime\n");
    write("UGS/LogSink/package.json", '{"name":"fixture-sink","private":true}'); write("UGS/LogSink/Dockerfile", "FROM node:22-alpine\n");
    const paths = ["ProjectSettings/ProjectVersion.txt", "Packages/manifest.json", "Packages/packages-lock.json", "package.json", "package-lock.json", "UGS/Modules/FlowStateVerification/FlowStateVerification.csproj", "UGS/LogSink/package.json", "UGS/LogSink/Dockerfile"];
    const spec = { schemaVersion: 1, editorVersion: "fixture-editor", tmp: { id: "com.unity.ugui", version: "2.0.0", sha256: hash("fixture TMP archive") }, cloudCode: { id: "com.unity.services.cloudcode", version: "2.10.4" }, npm: pins, hashes: [] };
    const refresh = () => { spec.hashes = paths.map(p => ({ path: p, sha256: hash(fs.readFileSync(path.join(root, p))) })); }; refresh();
    git("init", "--quiet"); git("add", "."); git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "--quiet", "-m", "synthetic baseline");
    run({ root, spec, write, readJson, refresh, git, tmpRoot, cloudRoot });
  } finally { const absolute = path.resolve(root); assert.equal(path.dirname(absolute).toLowerCase(), base.toLowerCase()); assert(path.basename(absolute).startsWith("flowstate-restore-")); fs.rmSync(absolute, { recursive: true, force: true }); }
}
test("tar metadata BOM accepted without extraction", () => { const bytes = tar([{ name: "package/package.json", body: '\ufeff{"name":"fixture","version":"1.0.0"}' }]); assert.equal(restore.tarPackage(bytes).name, "fixture"); });
test("bad gzip, traversal, link, duplicate metadata and absent JSON fail", () => {
  assert.throws(() => restore.tarPackage(Buffer.from("not gzip")));
  for (const files of [[{ name: "../outside", body: "x" }], [{ name: "package/package.json", type: "2", body: "x" }], [{ name: "package/a", body: "x" }], [{ name: "package/package.json", body: '{}' }, { name: "package/package.json", body: '{}' }]]) assert.throws(() => restore.tarPackage(tar(files)));
});
test("tar checksum tampering cannot pass metadata scan", () => { const bytes = zlib.gunzipSync(tar([{ name: "package/package.json", body: '{"name":"x","version":"1.0.0"}' }])); bytes[0] = 88; assert.throws(() => restore.tarPackage(zlib.gzipSync(bytes))); });
test("normal check preserves manifest/lock and aliases/integrity", () => fixture(({ root, spec }) => {
  const before = fs.readFileSync(path.join(root, "package-lock.json")), r = restore.check(root, spec); assert.equal(r.archives.length, 2); assert.equal(r.proposal.originalAliasesVersionsIntegrityPreserved, true); assert.equal(r.proposal.requiresUserLockRegenerationBeforeApplying, true); assert.equal(r.proposal.proposedManifest.dependencies["lodash-alias"], "npm:lodash@^4.17"); assert.deepEqual(fs.readFileSync(path.join(root, "package-lock.json")), before);
}));
test("cache suffix rename is discovered by provider ID/version", () => fixture(({ root, spec, cloudRoot }) => {
  fs.renameSync(path.join(root, cloudRoot), path.join(root, "Library/PackageCache/renamed-provider-without-old-suffix")); const r = restore.check(root, spec); assert(r.providers.some(v => v.discoveredPath.endsWith("renamed-provider-without-old-suffix"))); assert.equal(r.archives.length, 2);
}));
test("duplicate provider ID rejected", () => fixture(({ root, spec, write }) => { write("Library/PackageCache/duplicate/package.json", '{"name":"com.unity.ugui","version":"2.0.0"}'); throws(() => restore.check(root, spec), "RESTORE_PROVIDER_DUPLICATE"); }));
test("missing provider and wrong version fail explicitly", () => fixture(({ root, spec, write, tmpRoot }) => {
  write(tmpRoot + "/package.json", '{"name":"com.unity.ugui","version":"9.0.0"}'); throws(() => restore.check(root, spec), "RESTORE_PROVIDER_VERSION"); write(tmpRoot + "/package.json", '{"name":"other.provider","version":"2.0.0"}'); throws(() => restore.check(root, spec), "RESTORE_PROVIDER_MISSING");
}));
test("Editor and declaration hash drift rejected", () => fixture(({ root, spec, write, refresh }) => { write("ProjectSettings/ProjectVersion.txt", "m_EditorVersion: changed\n"); throws(() => restore.check(root, spec), "RESTORE_DECLARATION_CHANGED"); refresh(); throws(() => restore.check(root, spec), "RESTORE_EDITOR_VERSION"); }));
test("missing graph dependency is not empty success", () => fixture(({ root, spec, write, refresh, readJson }) => { const lock = readJson("Packages/packages-lock.json"); lock.dependencies["com.unity.ugui"].dependencies["missing.provider"] = "1.0.0"; write("Packages/packages-lock.json", JSON.stringify(lock)); refresh(); throws(() => restore.check(root, spec), "RESTORE_UPM_GRAPH"); }));
test("cache dependency graph must match lock even at same provider version", () => fixture(({ root, spec, write, tmpRoot }) => {
  write(tmpRoot + "/package.json", JSON.stringify({ name: "com.unity.ugui", version: "2.0.0", dependencies: { "unrecorded.dependency": "1.0.0" } })); throws(() => restore.check(root, spec), "RESTORE_PROVIDER_GRAPH");
}));
test("TMP archive pollution fails", () => fixture(({ root, spec, write, tmpRoot }) => { write(tmpRoot + "/Package Resources/TMP Essential Resources.unitypackage", "bad bytes"); throws(() => restore.check(root, spec), "RESTORE_TMP_ARCHIVE_HASH"); }));
test("npm archive pollution/version/alias identity fail", () => fixture(({ root, spec, write, cloudRoot }) => { write(cloudRoot + "/UnityServicesNpmLocal~/" + spec.npm[0].filename, tar([{ name: "package/package.json", body: '{"name":"other","version":"9.0.0"}' }])); assert.throws(() => restore.check(root, spec)); }));
test("lock/root and alias version/integrity mismatches fail", () => fixture(({ root, spec, readJson, write, refresh }) => {
  const lock = readJson("package-lock.json"), key = "node_modules/" + spec.npm[0].alias; lock.packages[key].integrity = "sha512-bad"; write("package-lock.json", JSON.stringify(lock)); refresh(); throws(() => restore.check(root, spec), "RESTORE_NPM_INTEGRITY_GRAPH"); lock.packages[key].integrity = spec.npm[0].integrity; lock.packages[key].version = "9.0.0"; write("package-lock.json", JSON.stringify(lock)); refresh(); throws(() => restore.check(root, spec), "RESTORE_NPM_VERSION_ALIAS");
}));
test("BOM manifest and separator normalization do not alter version identity", () => fixture(({ root, spec, write, readJson, refresh }) => { const m = readJson("package.json"); write("package.json", '\ufeff' + JSON.stringify(m)); refresh(); assert.equal(restore.check(root, spec).archives.length, 2); }));
test("local path traversal/source mismatch and lock path mismatch fail", () => fixture(({ root, spec, readJson, write, refresh }) => {
  const lock = readJson("package-lock.json"); lock.packages["node_modules/" + spec.npm[0].alias].resolved = "file:../outside.tgz"; write("package-lock.json", JSON.stringify(lock)); refresh(); assert.throws(() => restore.check(root, spec));
}));
test("explicit preparation copies once and then only verifies matching targets", () => fixture(({ root, spec }) => {
  const before = fs.readFileSync(path.join(root, ".git/index")); const first = restore.prepareArchives(root, spec); assert.equal(first.created, 2); const again = restore.prepareArchives(root, spec); assert.equal(again.created, 0); assert.equal(again.reused, 2); assert.deepEqual(fs.readFileSync(path.join(root, ".git/index")), before);
}));
test("target corruption is refused rather than overwritten", () => fixture(({ root, spec, write }) => { restore.prepareArchives(root, spec); const target = restore.check(root, spec).archives[0].target; write(target, "corrupt target"); throws(() => restore.prepareArchives(root, spec), "RESTORE_TARGET_CONFLICT"); }));
test("partial permission failure preserves inputs and permits verified resume", () => fixture(({ root, spec }) => {
  let writes = 0; const io = new Proxy(fs, { get(target, key) { if (key === "writeFileSync") return (...args) => { if (++writes === 2) throw Error("synthetic denied"); return fs.writeFileSync(...args); }; return target[key]; } });
  throws(() => restore.prepareArchives(root, spec, io), "RESTORE_COPY_FAILED_PARTIAL_RETAINED"); assert.equal(restore.prepareArchives(root, spec).files, 2);
}));
test("unignored or force-staged archive preparation rejected", () => fixture(({ root, spec, write, git }) => {
  const r = restore.check(root, spec); write(r.archives[0].target, "synthetic"); git("add", "-f", r.archives[0].target); throws(() => restore.prepareArchives(root, spec), "RESTORE_ARCHIVES_NOT_PRIVATE");
}));
test("cache and output junctions rejected", () => fixture(({ root, spec, cloudRoot }) => {
  const actual = path.join(root, "actual-provider"); fs.renameSync(path.join(root, cloudRoot), actual); fs.symlinkSync(actual, path.join(root, cloudRoot), "junction"); throws(() => restore.check(root, spec), "RESTORE_CACHE_LINK");
}));
test("network/offline/entitlement restrictions are explicit without auto install", () => {
  assert.equal(restore.availability({ archivePresent: false, network: "offline" }).code, "ACQUISITION_OFFLINE_MISSING");
  assert.equal(restore.availability({ archivePresent: false, network: "online", requiresAccount: true, entitlementVerified: false }).code, "ACQUISITION_ENTITLEMENT_REQUIRED");
  assert.equal(restore.availability({ archivePresent: true, network: "offline" }).remoteAcquisitionVerified, false);
});
test("NuGet/EmbeddedResource/LogSink declarations remain separate preserved inputs", () => fixture(({ root, spec }) => { const r = restore.check(root, spec); assert.equal(r.auxiliary.nuget.packageReferences.length, 4); assert.equal(r.auxiliary.nuget.resources.length, 2); assert.equal(r.auxiliary.logSink.directExternalDependencies, 0); assert.equal(r.auxiliary.nuget.resolvedGraph.packages.length, 0); }));
test("missing EmbeddedResource source is reported", () => fixture(({ root, spec }) => { fs.unlinkSync(path.join(root, "UGS/CloudCode/fixture.js")); throws(() => restore.check(root, spec), "RESTORE_EMBEDDED_RESOURCE_MISSING"); }));
test("unexpected LogSink dependency requires explicit review", () => fixture(({ root, spec, write, refresh }) => { write("UGS/LogSink/package.json", '{"dependencies":{"unexpected":"1.0.0"}}'); refresh(); throws(() => restore.check(root, spec), "RESTORE_LOGSINK_INPUT"); }));
test("missing exact npm archive names the unavailable input", () => fixture(({ root, spec, cloudRoot }) => {
  const rel = cloudRoot + "/UnityServicesNpmLocal~/" + spec.npm[0].filename; fs.unlinkSync(path.join(root, rel));
  assert.throws(() => restore.check(root, spec), e => e.code === "RESTORE_INPUT_UNAVAILABLE" && e.target === rel);
}));
test("NuGet generated graph cannot silently disagree with direct versions", () => fixture(({ root, spec, write }) => {
  write("UGS/Modules/FlowStateVerification/obj/project.assets.json", JSON.stringify({ libraries: { "Fixture0/9.0.0": { type: "package" } }, project: { restore: { sources: { "https://api.nuget.org/v3/index.json": {} } } } }));
  throws(() => restore.check(root, spec), "RESTORE_NUGET_GRAPH_VERSION");
}));
test("pinned NuGet transitive graph drift is not hidden by matching direct versions", () => fixture(({ root, spec, write }) => {
  const direct = [0, 1, 2, 3].map(i => "Fixture" + i + "/1.0.0"); spec.nuget = { resolvedPackages: [...direct, "Transitive/1.0.0"].sort() };
  write("UGS/Modules/FlowStateVerification/obj/project.assets.json", JSON.stringify({ libraries: Object.fromEntries([...direct, "Transitive/2.0.0"].map(v => [v, { type: "package" }])), project: { restore: { sources: {} } } }));
  throws(() => restore.check(root, spec), "RESTORE_NUGET_GRAPH_DRIFT");
}));
test("regenerated candidate metadata must contain only planned path changes", () => fixture(({ root, spec, readJson }) => {
  const result = restore.check(root, spec), oldManifest = readJson("package.json"), oldLock = readJson("package-lock.json"), candidate = structuredClone(oldLock);
  for (const op of result.proposal.lockOperations) if (op.section === "root") candidate.packages[""][op.group][op.alias] = op.to; else candidate.packages["node_modules/" + op.alias].resolved = op.to;
  assert.equal(restore.validateMigration(oldManifest, oldLock, result.proposal.proposedManifest, candidate, result.proposal).migrationMetadataValid, true);
  candidate.packages["node_modules/" + spec.npm[0].alias].integrity = "changed";
  throws(() => restore.validateMigration(oldManifest, oldLock, result.proposal.proposedManifest, candidate, result.proposal), "RESTORE_MIGRATION_GRAPH_CHANGED");
}));
test("archive output parent junction and nonignored output fail", () => fixture(({ root, spec, write }) => {
  write(".gitignore", "/Library/\n"); throws(() => restore.prepareArchives(root, spec), "RESTORE_ARCHIVES_NOT_PRIVATE");
  write(".gitignore", "/Library/\n/Ignore/\n"); fs.mkdirSync(path.join(root, "Ignore")); fs.mkdirSync(path.join(root, "redirect")); fs.symlinkSync(path.join(root, "redirect"), path.join(root, "Ignore/DependencyArchives"), "junction");
  throws(() => restore.prepareArchives(root, spec), "RESTORE_OUTPUT_LINK");
}));
test("require is inert and CLI failures do not echo input tokens", () => {
  const script = "const fs=require('fs'),cp=require('child_process');for(const k of ['writeFileSync','mkdirSync','rmSync'])fs[k]=()=>{throw Error('write')};for(const k of ['spawnSync','execFileSync'])cp[k]=()=>{throw Error('process')};require(process.argv[1]);";
  const r = cp.spawnSync(process.execPath, ["-e", script, path.join(__dirname, "dependency-restore.cjs")], { encoding: "utf8", windowsHide: true }); assert.equal(r.status, 0, r.stderr); assert.equal(r.stdout, ""); const bad = restore.cli(["--private-token"]); assert.equal(bad.exitCode, 2); assert(!JSON.stringify(bad).includes("private-token"));
});
let passed = 0; for (const item of tests) { try { item.run(); passed++; } catch (e) { console.error("FAIL " + item.name); throw e; } }
console.log(JSON.stringify({ suite: "DependencyRestore", passed, failed: 0, unityExecuted: false, realIndexChanged: false }));
