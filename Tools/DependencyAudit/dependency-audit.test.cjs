"use strict";
// Synthetic metadata and isolated temporary repositories only. Never runs Unity.
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path");
const os = require("node:os"), cp = require("node:child_process"), crypto = require("node:crypto");
const audit = require("./dependency-audit.cjs");
const tests = [];
const test = (name, run) => tests.push({ name, run });
const throws = (fn, code) => assert.throws(fn, e => e.code === code);
const sha = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
const guid = "1234567890abcdef1234567890abcdef";
function pkg(version = "1.0.0", source = "registry", dependencies = {}) {
  return { version, source, dependencies, url: "https://packages.unity.com" };
}
function g(manifest, deps) { return audit.graph({ dependencies: manifest }, { dependencies: deps }); }
function codes(report) { return report.diagnostics.map(d => d.code); }
function fixture(run) {
  const base = fs.realpathSync(os.tmpdir()), root = fs.mkdtempSync(path.join(base, "flowstate-dependency-audit-"));
  const write = (p, text) => { fs.mkdirSync(path.dirname(path.join(root, p)), { recursive: true }); fs.writeFileSync(path.join(root, p), text); };
  const git = (...args) => cp.execFileSync("git", ["-c", "core.hooksPath=" + path.join(root, "empty-hooks"), "-c", "core.fsmonitor=false", "-C", root, ...args], { encoding: "utf8", stdio: ["ignore", "pipe", "pipe"], windowsHide: true });
  try {
    for (const folder of ["Assets", "Packages", "ProjectSettings", "Tools", "UGS", "Library/PackageCache"]) fs.mkdirSync(path.join(root, folder), { recursive: true });
    write(".gitignore", "/Library/\n/UGS/.env\n");
    write("Assets/Item.asset", "%YAML 1.1\n--- !u!114 &11400000\nMonoBehaviour:\n  value: 1\n");
    write("Assets/Item.asset.meta", "fileFormatVersion: 2\nguid: " + guid + "\n");
    write("Packages/manifest.json", '{"dependencies":{}}');
    write("Packages/packages-lock.json", '{"dependencies":{}}');
    write("AI/99_Templates/ASSET_DEPENDENCY_TEMPLATE.md", audit.requiredFields.map(f => "| " + f + " | fixture | ").join("\n"));
    write("AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json", JSON.stringify({ files: [{ path: "Assets/Item.asset", sha256: sha(fs.readFileSync(path.join(root, "Assets/Item.asset"))) }] }));
    git("init", "--quiet"); git("add", ".");
    git("-c", "user.name=AuditFixture", "-c", "user.email=audit-fixture@example.invalid", "commit", "--quiet", "-m", "synthetic baseline");
    run({ root, write, git });
  } finally {
    // Check the absolute cleanup target before any recursive removal on Windows.
    const resolved = path.resolve(root);
    assert.equal(path.dirname(resolved).toLowerCase(), path.resolve(base).toLowerCase());
    assert(path.basename(resolved).startsWith("flowstate-dependency-audit-"));
    fs.rmSync(resolved, { recursive: true, force: true });
  }
}

test("paths preserve Unicode, spaces, newlines, case and distinct basenames", () => {
  assert.deepEqual(audit.paths(["B/x", "A/한 글\nfile", "B\\x", "A/x", "a/x"]), ["A/x", "A/한 글\nfile", "B/x", "a/x"]);
});
for (const p of ["../outside", "A/../outside", "C:\\outside", "/outside", "\\\\host\\share", "A\0B", "", "."])
  test("scope rejects external/traversal path " + JSON.stringify(p), () => throws(() => audit.relative(p), "PATH_OUTSIDE_SCOPE"));
test("NUL decoding preserves newlines and rejects truncated input", () => {
  assert.deepEqual(audit.nul("A\nB\0한 글\0"), ["A\nB", "한 글"]);
  assert.deepEqual(audit.nul(""), []);
  throws(() => audit.nul("not-terminated"), "GIT_TRUNCATED_NUL");
});
test("staged add/delete/rename/copy preserve both paths", () => {
  assert.deepEqual(audit.stagedChanges("A\0new\0D\0old\0R100\0a b\0한 글\0C075\0old\0copy\0"), [
    { status: "A", path: "new" }, { status: "D", path: "old" }, { status: "R100", from: "a b", path: "한 글" }, { status: "C075", from: "old", path: "copy" }
  ]);
  throws(() => audit.stagedChanges("R100\0only-one\0"), "GIT_STATUS_FORMAT");
});
test("case differences are diagnosed without lowercasing paths", () => assert.deepEqual(audit.caseCollisions(["Assets/X", "Assets/x"]), [{ first: "Assets/X", second: "Assets/x" }]));
test("ignore tracked-match and nested negative exception stay distinct", () => {
  const r = audit.ignoreResult({ status: 0, stdout: ".gitignore\0" + "1\0*.csproj\0A/a.csproj\0A/.gitignore\0" + "2\0!*.csproj\0A/b.csproj\0" }, ["A/a.csproj", "A/b.csproj"]);
  assert.equal(r[0].ignored, true); assert.equal(r[1].ignored, false); assert.equal(r[1].pattern, "!*.csproj");
});
test("ignore exit 1 is no match; real exit/error and partial output fail", () => {
  assert.equal(audit.ignoreResult({ status: 1, stdout: "\0\0\0A\0" }, ["A"])[0].ignored, false);
  throws(() => audit.ignoreResult({ status: 128, stdout: "" }, []), "GIT_IGNORE_FAILED");
  throws(() => audit.ignoreResult({ status: 0, stdout: "" }, ["A"]), "GIT_IGNORE_FORMAT");
  throws(() => audit.ignoreResult({ status: 0, stdout: "x\0NaN\0p\0A\0" }, ["A"]), "GIT_IGNORE_FORMAT");
});
test("JSON BOM accepted; malformed JSON fails without exposing body", () => {
  assert.deepEqual(audit.parseJson('\ufeff{"a":1}'), { a: 1 });
  throws(() => audit.parseJson('{"credential":"synthetic-hidden-value"'), "JSON_INVALID");
});
test("normal direct/transitive registry+builtin graph", () => {
  const r = g({ "com.test.a": "1.0.0" }, { "com.test.a": pkg("1.0.0", "registry", { "com.test.b": "1.0.0" }), "com.test.b": pkg("1.0.0", "builtin") });
  assert.equal(r.nodes.length, 2); assert.equal(r.nodes[1].direct, false); assert.equal(r.edges.length, 1); assert.deepEqual(r.diagnostics, []);
});
for (const [source, version] of [["local", "file:Packages/a"], ["git", "https://example.invalid/a.git"]])
  test("local/git graph source " + source, () => assert.deepEqual(g({ "com.test.a": version }, { "com.test.a": pkg(version, source) }).diagnostics, []));
test("UPM resolved upgrade is a recorded edge difference, not exact-string failure", () => {
  const r = g({ "com.test.a": "1.0.0" }, { "com.test.a": pkg("1.0.0", "registry", { "com.test.b": "1.0.0" }), "com.test.b": pkg("2.0.0", "builtin") });
  assert.equal(r.edges[0].versionDifference, true); assert.deepEqual(r.diagnostics, []);
});
test("missing direct and transitive dependencies are explicit", () => {
  assert(codes(g({ "com.test.a": "1.0.0" }, {})).includes("PACKAGE_DIRECT_MISSING"));
  assert(codes(g({}, { "com.test.a": pkg("1.0.0", "registry", { "com.test.b": "1.0.0" }) })).includes("PACKAGE_TRANSITIVE_MISSING"));
});
test("version/source mismatch and unknown source are explicit", () => {
  assert(codes(g({ "com.test.a": "1.0.0" }, { "com.test.a": pkg("2.0.0") })).includes("PACKAGE_DIRECT_VERSION_DIFFERENCE"));
  assert(codes(g({ "com.test.a": "file:Packages/a" }, { "com.test.a": pkg() })).includes("PACKAGE_SOURCE_MISMATCH"));
  assert(codes(g({}, { "com.test.a": pkg("1.0.0", "unknown") })).includes("PACKAGE_SOURCE_UNKNOWN"));
});
test("schema errors and cycles do not become empty successful graphs", () => {
  throws(() => audit.graph({}, {}), "PACKAGE_SCHEMA");
  throws(() => g({}, { "com.test.a": { version: "1.0.0" } }), "PACKAGE_SCHEMA");
  const r = g({}, { "com.test.a": pkg("1.0.0", "registry", { "com.test.b": "1.0.0" }), "com.test.b": pkg("1.0.0", "registry", { "com.test.a": "1.0.0" }) });
  assert(codes(r).includes("PACKAGE_CYCLE"));
});
test("unknown resolved version and transitive downgrade remain explicit diagnostics", () => {
  assert(codes(g({}, { "com.test.a": pkg("unknown") })).includes("PACKAGE_VERSION_UNRESOLVED"));
  const r = g({}, { "com.test.a": pkg("1.0.0", "registry", { "com.test.b": "2.0.0" }), "com.test.b": pkg("1.0.0") });
  assert(codes(r).includes("PACKAGE_TRANSITIVE_BELOW_REQUEST"));
});
test("GUID map includes ignored TMP assets and folder meta", () => {
  const files = new Map([["Assets/TextMesh Pro.meta", "guid: " + guid + "\nfolderAsset: yes\n"], ["Assets/A.meta", "guid: " + "a".repeat(32) + "\n"]]);
  const r = audit.metaIndex([...files.keys()], p => Buffer.from(files.get(p)));
  assert.equal(r.map.get(guid).folder, true); assert.equal(r.entries.length, 2); assert.deepEqual(r.diagnostics, []);
});
test("duplicate and broken meta GUIDs are diagnosed", () => {
  const r = audit.metaIndex(["a.meta", "b.meta", "c.meta"], p => Buffer.from(p === "c.meta" ? "guid: broken\n" : "guid: " + guid + "\n"));
  assert(codes(r).includes("META_GUID_DUPLICATE")); assert(codes(r).includes("META_GUID_INVALID"));
});
test("builtin/null/package and unresolved references stay distinct", () => {
  const index = new Map([[guid, { path: "Library/PackageCache/test/A.cs" }]]);
  const r = audit.references("guid: " + guid + "\nguid: 0000000000000000e000000000000000\nguid: " + "0".repeat(32) + "\nguid: " + "b".repeat(32), "asset", index);
  assert.deepEqual(r.map(x => x.status), ["resolved", "builtin", "null", "unresolved"]);
});
test("nested ShaderGraph JSON and malformed GUIDs diagnosed", () => {
  assert.equal(audit.references('{\\"guid\\":\\"' + guid + '\\"}', "graph", new Map(), true)[0].status, "unresolved");
  assert.equal(audit.references("guid: malformed", "asset", new Map())[0].status, "invalid");
});
for (const [p, bytes, reason] of [["a.png", Buffer.from("image"), "BINARY_INTERPRETATION_REQUIRED"], ["a.asset", Buffer.from([0, 1]), "BINARY_INTERPRETATION_REQUIRED"], ["a.cs", Buffer.from('Resources.Load("named"); Shader.Find("shader");'), "NAME_OR_DYNAMIC_REFERENCE"], ["b.cs", Buffer.from("Type.GetType(x); Instantiate(prefab);"), "NAME_OR_DYNAMIC_REFERENCE"]])
  test("name/binary reference cannot prove unused or restore: " + p, () => {
    const r = audit.referenceLimits(p, bytes); assert(r.reasons.includes(reason)); assert.equal(r.unused, false); assert.equal(r.independentRestore, false);
  });
const verified = { kind: "original", sourceVerified: true, rightsVerified: true, metaReproducible: true, importReproducible: true };
for (const kind of ["unknown", "mixed", "modified", "derived", "project"])
  test("non-original ownership stays preserve/pending: " + kind, () => assert.equal(audit.classification({ ...verified, kind }).direction, "preserve-pending"));
for (const field of ["sourceVerified", "rightsVerified", "metaReproducible", "importReproducible"])
  test("unproven exclusion evidence blocks: " + field, () => assert.equal(audit.classification({ ...verified, [field]: false }).direction, "preserve-pending"));
test("settings preserve and user decision before/after cannot imply application", () => {
  assert.equal(audit.classification({ ...verified, projectSettings: true }).direction, "preserve-pending");
  assert.equal(audit.classification({ ...verified, requiresUserDecision: true }).direction, "preserve-pending");
  const r = audit.classification({ ...verified, requiresUserDecision: true, userDecision: "exclude-candidate" });
  assert.equal(r.direction, "untracked-candidate"); assert.equal(r.applied, false); assert.equal(r.rightsGranted, false);
});
test("credential URLs and invalid payload/spec never enter reports", () => {
  const hidden = "synthetic-private-token";
  assert.equal(audit.publicUrl("https://user:" + hidden + "@example.invalid/x"), "<redacted-url>");
  assert.equal(audit.publicUrl("https://example.invalid/x?token=" + hidden), "<redacted-url>");
  const r = g({ "com.test.a": "https://user:" + hidden + "@example.invalid/a.git" }, { "com.test.a": pkg("https://user:" + hidden + "@example.invalid/a.git", "git") });
  assert(!JSON.stringify(r).includes(hidden));
  throws(() => g({}, { "com.test.a": pkg("1.0.0", "registry", { [hidden + ":bad"]: "1.0.0" }) }), "PACKAGE_SCHEMA");
});
test("registration format missing/duplicate field detection", () => {
  assert(audit.registrationTemplate("| asset_id | x |\n| asset_id | y |\n").duplicates.includes("asset_id"));
  assert(audit.registrationTemplate("").missing.includes("game_usage"));
});
test("production Git driver blocks mutation and output-file flags", () => {
  const run = audit.gitReader(process.cwd());
  throws(() => run(["add", "."]), "GIT_COMMAND_BLOCKED");
  throws(() => run(["diff", "--output=forbidden"]), "GIT_COMMAND_BLOCKED");
});
test("import has no CLI execution or output", () => {
  const r = cp.spawnSync(process.execPath, ["-e", "require(" + JSON.stringify(path.join(__dirname, "dependency-audit.cjs")) + ")"], { encoding: "utf8", windowsHide: true });
  assert.equal(r.status, 0); assert.equal(r.stdout, ""); assert.equal(r.stderr, "");
});
test("scope walker rejects junction/symlink using injected metadata", () => fixture(({ root }) => {
  const fake = new Proxy(fs, { get(target, key) { if (key === "lstatSync") return p => p === path.join(root, "Assets") ? { isSymbolicLink: () => true } : fs.lstatSync(p); return target[key]; } });
  const diagnostics = []; assert.deepEqual(audit.reader(root, fake).walk("Assets", diagnostics), []); assert.equal(diagnostics[0].code, "SCOPE_LINK");
}));
test("partial enumeration and denied reads never succeed as empty inventory", () => fixture(({ root }) => {
  const fake = new Proxy(fs, { get(target, key) { if (key === "readdirSync") return p => { if (p === path.join(root, "Assets")) throw Error("denied synthetic-private-token"); return fs.readdirSync(p); }; return target[key]; } });
  const result = audit.cli(["--check", "--root", root], { fs: fake });
  assert.equal(result.exitCode, 2); assert.equal(result.report.collectionComplete, false); assert(!JSON.stringify(result).includes("synthetic-private-token"));
  const denied = new Proxy(fs, { get(target, key) { if (key === "readFileSync") return () => { throw Error("private read failure"); }; return target[key]; } });
  throws(() => audit.reader(root, denied).read("Assets/Item.asset"), "IO_READ_FAILED");
}));
test("sensitive data bodies are blocked and never emitted", () => fixture(({ root, write }) => {
  write("UGS/.env", "TOKEN=synthetic-private-token");
  throws(() => audit.reader(root).read("UGS/.env"), "SENSITIVE_READ_BLOCKED");
  const r = audit.cli(["--check", "--root", root]); assert(!JSON.stringify(r).includes("synthetic-private-token"));
}));
test("actual CLI read-only integration and deterministic output", () => fixture(({ root }) => {
  const before = sha(fs.readFileSync(path.join(root, ".git/index"))), asset = sha(fs.readFileSync(path.join(root, "Assets/Item.asset")));
  const command = [path.join(__dirname, "dependency-audit.cjs"), "--check", "--root", root];
  const first = cp.spawnSync(process.execPath, command, { encoding: "utf8", windowsHide: true });
  const second = cp.spawnSync(process.execPath, command, { encoding: "utf8", windowsHide: true });
  assert.equal(first.status, 0, first.stdout); assert.equal(first.stdout, second.stdout); assert.equal(first.stderr, "");
  assert.equal(sha(fs.readFileSync(path.join(root, ".git/index"))), before); assert.equal(sha(fs.readFileSync(path.join(root, "Assets/Item.asset"))), asset);
  const r = JSON.parse(first.stdout); assert.equal(r.collectionComplete, true); assert.equal(r.independentRestore, false); assert.equal(r.ownership[0].direction, "preserve-pending");
}));
test("actual staged rename/add/delete and tracked ignore exception integration", () => fixture(({ root, write, git }) => {
  write(".gitignore", "/Library/\n*.txt\n"); write("UGS/.gitignore", "!keep.txt\n"); write("UGS/keep.txt", "safe");
  write("UGS/ignored.txt", "safe"); write("ProjectSettings/tracked.txt", "safe"); git("add", "-f", "ProjectSettings/tracked.txt");
  git("mv", "Assets/Item.asset", "Assets/Renamed.asset"); write("Assets/new.cs", "class New {}"); git("add", "Assets/new.cs"); git("rm", "Assets/Item.asset.meta");
  const r = audit.collect(root); assert(r.git.staged.some(x => x.status.startsWith("R") && x.from === "Assets/Item.asset"));
  assert(r.git.staged.some(x => x.status === "D")); assert(r.git.staged.some(x => x.status === "A"));
  const tracked = r.inventory.find(x => x.path === "ProjectSettings/tracked.txt"); assert.equal(tracked.indexTracked, true); assert.equal(tracked.ignore.ignored, true);
  assert.equal(r.inventory.find(x => x.path === "UGS/keep.txt").ignore.ignored, false);
  assert.equal(r.inventory.find(x => x.path === "UGS/ignored.txt").ignore.ignored, true);
}));
test("ignored TMP metadata is actually searched; missing/orphan/unresolved fail check", () => fixture(({ root, write }) => {
  write(".gitignore", "/Library/\n/Assets/TextMesh Pro/\n"); write("Assets/TextMesh Pro/A.asset", "%YAML 1.1\n--- !u!114 &11400000\nvalue: 1\n");
  write("Assets/TextMesh Pro/A.asset.meta", "guid: " + "a".repeat(32) + "\n");
  write("Assets/Ref.asset", "value: {fileID: 11400000, guid: " + "a".repeat(32) + ", type: 2}\nother: {guid: " + "b".repeat(32) + "}\n");
  write("Assets/orphan.meta", "guid: " + "c".repeat(32) + "\n");
  const result = audit.cli(["--check", "--root", root]); assert.equal(result.exitCode, 1); assert.equal(result.report.collectionComplete, true);
  assert(result.report.references.some(x => x.guid === "a".repeat(32) && x.status === "resolved"));
  assert(codes(result.report).includes("ORPHAN_META")); assert(codes(result.report).includes("ASSET_META_MISSING")); assert(codes(result.report).includes("REFERENCE_UNRESOLVED"));
}));
test("adapter/CLI errors are nonzero and sanitized", () => fixture(({ root }) => {
  const result = audit.cli(["--check", "--root", root], { git: () => ({ status: 128, stdout: "", stderr: "synthetic-private-token" }) });
  assert.equal(result.exitCode, 2); assert.equal(result.report.collectionComplete, false); assert(!JSON.stringify(result).includes("synthetic-private-token"));
  assert.equal(audit.cli(["--unexpected-synthetic-private-token"]).exitCode, 2);
}));
test("explicit baseline detects changes; path cannot escape selected root", () => fixture(({ root, write }) => {
  const baseline = "AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json";
  assert.equal(audit.cli(["--check", "--root", root, "--baseline", baseline]).exitCode, 0);
  write("Assets/Item.asset", "changed baseline input");
  const r = audit.cli(["--check", "--root", root, "--baseline", baseline]);
  assert.equal(r.exitCode, 2); assert(codes(r.report).includes("BASELINE_HASH_CHANGED"));
  assert.equal(audit.cli(["--root", root, "--baseline", "../outside"]).exitCode, 2);
}));
test("optional root metadata permission failure is not treated as absence", () => fixture(({ root }) => {
  const fake = new Proxy(fs, { get(target, key) { if (key === "lstatSync") return p => { if (p === path.join(root, "package.json")) { const e = Error("denied"); e.code = "EACCES"; throw e; } return fs.lstatSync(p); }; return target[key]; } });
  const r = audit.cli(["--root", root], { fs: fake }); assert.equal(r.exitCode, 2); assert(codes(r.report).includes("IO_STAT_FAILED"));
}));

if (process.argv.length > 2) {
  assert.equal(process.argv.length, 4, "Use --policy <repo-relative-metadata-path>");
  assert.equal(process.argv[2], "--policy");
  const policyPath = audit.relative(process.argv[3]);
  test("explicit preservation policy uses real classifier without granting rights or exclusions", () => {
    const policy = audit.parseJson(audit.reader(path.resolve(__dirname, "../..")).read(policyPath).toString("utf8"));
    assert.equal(policy.schemaVersion, 1); assert.equal(policy.mode, "preservation-only");
    assert.equal(policy.rightsClearance, false); assert.equal(policy.phase1Complete, false);
    assert.equal(policy.userInputRequired, false); assert.deepEqual(policy.userAnswers, []);
    assert.deepEqual(policy.phase1ExclusionCandidates, []); assert.deepEqual(policy.requestedGitChanges, []); assert.deepEqual(policy.requestedAssetChanges, []);
    assert(Array.isArray(policy.subjects) && policy.subjects.length > 0);
    for (const item of policy.subjects) {
      assert.equal(item.requiresNewUserDecision, false); assert(item.reason && item.missing && item.next);
      const result = audit.classification(item.classificationInput);
      assert.equal(result.direction, "preserve-pending", item.id); assert.equal(result.applied, false); assert.equal(result.rightsGranted, false);
      // Modified/configuration content must stay preserved even if evidence is later filled.
      if (item.classificationInput.kind === "modified" || item.classificationInput.projectSettings === true) {
        const proof = { ...item.classificationInput, sourceVerified: true, rightsVerified: true, metaReproducible: true, importReproducible: true };
        assert.equal(audit.classification(proof).direction, "preserve-pending", item.id);
      }
    }
  });
}

let passed = 0;
for (const item of tests) {
  try { item.run(); passed++; }
  catch (error) { console.error("FAIL " + item.name); throw error; }
}
console.log(JSON.stringify({ suite: "DependencyAudit", passed, failed: 0, unityExecuted: false, realIndexChanged: false }));
