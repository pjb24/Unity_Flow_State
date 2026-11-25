"use strict";
// Explicit, digest-bound snapshot API. CLI is read-only; no Unity, install, network or rollback writes.
const fs = require("node:fs"), path = require("node:path"), crypto = require("node:crypto");
const audit = require("./dependency-audit.cjs"), preservation = require("./dependency-preservation.cjs");
const sha = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
const encode = value => Buffer.from(JSON.stringify(value, null, 2) + "\n", "utf8");
const fail = code => { throw new audit.AuditError(code); };
const isHash = v => typeof v === "string" && /^[a-f0-9]{64}$/.test(v);
const reviewed = Object.freeze({
  "UGS/CloudCode/receipt-journal.js": "30ebecad166a359f89d68be278d5c576a40379b95abcc89a98db1905df462cf7",
  "UGS/CloudCode/receipt-store.js": "04bf23a486588072d175aba69e950a131ca7a26c79692178ba24512eec9d1013",
  "UGS/Tests/receipt-journal.test.cjs": "26ea853d95194e671d9e1851f71441f09d6047fb0d8e024df332d462bc1f28fc",
  "UGS/Tests/receipt-store.test.cjs": "ca7a251ba7a1acdad54187405e43f435acc86a7ce4cd808b14996c339a8cb74d",
  "UGS/Verification/New-TransferHmacSecret.ps1": "99ae29f305239d71d49cad34754145a5dcc84c3f6871db81143e5646cf836c52",
  "UGS/Verification/new-transfer-hmac-secret.cjs": "0e2dfa7664594bcd555c194791f9c62b0a206d043bf33f63f85542ea51458813"
});
const sdkBytes = () => encode({ sdk: { version: "9.0.315", rollForward: "disable", allowPrerelease: false } });
function safe(p) {
  if (typeof p !== "string" || audit.relative(p) !== p || /[\x00-\x1f\x7f:*?<>|"#[\]]|[. ](?:\/|$)/.test(p) ||
      p.split("/").some(s => /^(?:con|prn|aux|nul|com[1-9]|lpt[1-9])(?:\.|$)/i.test(s))) fail("SNAPSHOT_PATH");
  return p;
}
function guarded(absolute, io = fs) {
  const full = path.resolve(absolute), parts = full.slice(path.parse(full).root.length).split(path.sep).filter(Boolean);
  let cursor = path.parse(full).root;
  for (let i = 0; i < parts.length; i++) {
    cursor = path.join(cursor, parts[i]);
    let s; try { s = io.lstatSync(cursor); } catch (e) { if (e.code === "ENOENT") break; throw e; }
    if (s.isSymbolicLink()) fail("SNAPSHOT_LINK");
    if (i < parts.length - 1 && !s.isDirectory()) fail("SNAPSHOT_PARENT_FILE");
  }
  return full;
}
function exists(p, io) { try { io.lstatSync(p); return true; } catch (e) { if (e.code === "ENOENT") return false; throw e; } }
function related(a, b) {
  const x = path.resolve(a).toLowerCase(), y = path.resolve(b).toLowerCase();
  return x === y || x.startsWith(y + path.sep) || y.startsWith(x + path.sep);
}
function roots(source, target, io) {
  if (!path.isAbsolute(source) || !path.isAbsolute(target) || related(source, target) || related(path.resolve(__dirname, "../.."), target)) fail("SNAPSHOT_ROOT");
  const from = guarded(source, io), to = guarded(target, io);
  if (!io.lstatSync(from).isDirectory()) fail("SNAPSHOT_ROOT");
  return { from, to };
}
function projectPath(p) {
  safe(p);
  if (p.split("/").some(s => /^(?:\.git|\.aws|\.codex|\.agents|\.vs|\.vscode|node_modules|obj|bin|library|build|builds|temp|logs|usersettings)$/i.test(s))) fail("SNAPSHOT_EXCLUDED_PATH");
  if (/^Assets\/ThirdParty(?:\/|\.|$)/i.test(p) || /^Assets\/Scripts\/Runtime\/Features\/OnlineRecordProductionConfiguration\.cs(?:\.meta)?$/i.test(p)) fail("SNAPSHOT_EXCLUDED_PATH");
  if (!/^(?:AI|Assets|Packages|ProjectSettings|Tools|UGS)\//.test(p) && !["README.md", ".gitignore", ".vsconfig", "package.json", "package-lock.json"].includes(p)) fail("SNAPSHOT_EXCLUDED_PATH");
  if (p === ".vsconfig") fail("SNAPSHOT_EXCLUDED_PATH");
  return p;
}
function readSource(root, p, io = fs) {
  projectPath(p);
  const full = guarded(path.join(root, p), io);
  if (!io.lstatSync(full).isFile()) fail("SNAPSHOT_NOT_FILE");
  if (Object.hasOwn(reviewed, p)) {
    const bytes = io.readFileSync(full);
    if (sha(bytes) !== reviewed[p]) fail("SNAPSHOT_REVIEWED_SOURCE_CHANGED");
    return bytes;
  }
  return audit.reader(root, io).read(p);
}
function privateInputs(root) {
  const disk = audit.reader(root), dir = "AI/90_Tasks/Prototype_10/";
  const pinned = (p, hash) => { const bytes = disk.read(p); if (sha(bytes) !== hash) fail("SNAPSHOT_PRIVATE_EVIDENCE"); return audit.parseJson(bytes.toString("utf8")); };
  const spec = pinned(dir + "20261119_19_Phase2Step6RestoreSpec.json", "11e638789e8ec0f71b453d3ad75910db5588251af7ced6d84ceb022455f65d65");
  const evidence = pinned(dir + "20261119_14_Phase2Step4Check.json", "1529157cb60a4d76d5274dcbb8a4ba20192b07a6eabd55d12b7a2eb1afec2a2c");
  const output = "Ignore/DependencyPreservation/tmp-baseline/" + evidence.expected.manifestSha256;
  preservation.verify(root, output, evidence.expected);
  const manifestBytes = disk.read(output + "/manifest.json"), manifest = audit.parseJson(manifestBytes.toString("utf8"));
  return [{ path: output + "/manifest.json", sha256: sha(manifestBytes) }, ...manifest.files.map(f => ({ path: output + "/files/" + f.path, sha256: f.sha256 })),
    ...spec.npm.map(f => ({ path: "Ignore/DependencyArchives/npm/" + spec.cloudCode.id + "/" + spec.cloudCode.version + "/" + f.sha256 + "/" + f.filename, sha256: f.sha256 }))];
}
function fileRows(rows) {
  if (!Array.isArray(rows)) fail("SNAPSHOT_ROWS");
  const seen = new Set();
  for (const row of rows) {
    if (!row || !isHash(row.sha256)) fail("SNAPSHOT_ROWS");
    const p = safe(row.path), key = p.toLowerCase();
    if (seen.has(key)) fail("SNAPSHOT_DUPLICATE");
    seen.add(key);
  }
  return rows;
}
function gitState(root, io) {
  if (!exists(path.join(root, ".git"), io)) return null;
  // Missing HEAD/index is an error, never a successful empty repository result.
  return { snapshot: audit.gitSnapshot(audit.gitReader(root)), indexSha256: sha(audit.reader(root, io).read(".git/index")) };
}
function sourceRead(root, row, privateRows, io) {
  const bytes = row.path.startsWith("Ignore/") ? (() => {
    if (privateRows.get(row.path) !== row.sha256) fail("SNAPSHOT_PRIVATE_INPUT");
    return audit.reader(root, io).read(row.path);
  })() : readSource(root, row.path, io);
  if (sha(bytes) !== row.sha256) fail("SNAPSHOT_INPUT_HASH");
  return bytes;
}
function prepare(source, manifest, io = fs) {
  if (!manifest || manifest.schemaVersion !== 1 || typeof manifest.candidateRoot !== "string" ||
      typeof manifest.includeSdkPin !== "boolean" || !Array.isArray(manifest.directories) ||
      !Array.isArray(manifest.requiredPaths) || !manifest.requiredPaths.length) fail("SNAPSHOT_SCHEMA");
  const { from, to } = roots(source, manifest.candidateRoot, io);
  if (exists(to, io)) fail("SNAPSHOT_TARGET_EXISTS");
  const files = fileRows(manifest.files).map(f => ({ path: f.path, sha256: f.sha256 })).sort((a, b) => a.path < b.path ? -1 : 1);
  if (!files.length) fail("SNAPSHOT_EMPTY");
  const names = new Set(files.map(f => f.path)), lower = new Set(files.map(f => f.path.toLowerCase()));
  for (const required of manifest.requiredPaths) if (!names.has(safe(required))) fail("SNAPSHOT_REQUIRED_INPUT");
  if (names.has("UGS/Modules/FlowStateVerification/FlowStateVerification.csproj"))
    for (const p of Object.keys(reviewed).filter(p => /\/(?:CloudCode|Tests)\//.test(p))) if (!names.has(p)) fail("SNAPSHOT_REQUIRED_INPUT");
  const initialGit = gitState(from, io), privates = new Map(files.some(f => f.path.startsWith("Ignore/")) ? privateInputs(from).map(f => [f.path, f.sha256]) : []);
  const directories = new Set();
  const addDirectory = p => {
    safe(p); if (lower.has(p.toLowerCase())) fail("SNAPSHOT_PARENT_COLLISION");
    directories.add(p);
    if (p.includes("/")) addDirectory(p.slice(0, p.lastIndexOf("/")));
  };
  for (const p of manifest.directories) { projectPath(p); addDirectory(p); if (!io.lstatSync(guarded(path.join(from, p), io)).isDirectory()) fail("SNAPSHOT_DIRECTORY"); }
  const guids = new Set();
  for (const f of files) {
    if (f.path.includes("/")) addDirectory(f.path.slice(0, f.path.lastIndexOf("/")));
    const bytes = sourceRead(from, f, privates, io);
    if (f.path.endsWith(".meta")) {
      const text = bytes.toString("utf8"), guid = text.match(/^guid:\s*([a-f0-9]{32})\s*$/m);
      // Preserved private payload duplicates project GUIDs intentionally; never imported as Assets.
      if (guid && !f.path.startsWith("Ignore/")) { if (guids.has(guid[1])) fail("SNAPSHOT_GUID_DUPLICATE"); guids.add(guid[1]); }
      if (/^folderAsset:\s*yes\s*$/m.test(text)) {
        const folder = f.path.slice(0, -5); addDirectory(folder);
        if (!io.lstatSync(guarded(path.join(from, folder), io)).isDirectory()) fail("SNAPSHOT_DIRECTORY");
      }
    }
  }
  // Catch case collisions of implicit/explicit directories as well as parent-file conflicts.
  if (new Set([...directories].map(p => p.toLowerCase())).size !== directories.size) fail("SNAPSHOT_DUPLICATE");
  if (manifest.includeSdkPin) { if (lower.has("global.json")) fail("SNAPSHOT_GENERATED_COLLISION"); files.push({ path: "global.json", sha256: sha(sdkBytes()), generated: true }); }
  for (const f of files.filter(f => !f.generated)) sourceRead(from, f, privates, io);
  if (JSON.stringify(gitState(from, io)) !== JSON.stringify(initialGit)) fail("SNAPSHOT_SOURCE_GIT_CHANGED");
  const plan = { schemaVersion: 1, sourceRoot: from, candidateRoot: to, files, directories: [...directories].sort(), sourceGit: initialGit };
  return { ...plan, planSha256: sha(encode(plan)), copied: false, independentRestore: false };
}
function enumerate(root, io) {
  const files = [], directories = [];
  function visit(rel) {
    const full = guarded(path.join(root, rel), io), st = io.lstatSync(full);
    if (st.isDirectory()) {
      if (rel) directories.push(rel);
      for (const name of io.readdirSync(full).sort()) visit(rel ? rel + "/" + name : name);
    } else if (st.isFile()) files.push(safe(rel)); else fail("SNAPSHOT_FILE_KIND");
  }
  visit(""); return { files: files.sort(), directories: directories.sort() };
}
function verify(plan, io = fs) {
  if (!plan || !isHash(plan.planSha256)) fail("SNAPSHOT_PLAN_REQUIRED");
  const identity = { schemaVersion: plan.schemaVersion, sourceRoot: plan.sourceRoot, candidateRoot: plan.candidateRoot,
    files: plan.files, directories: plan.directories, sourceGit: plan.sourceGit };
  if (sha(encode(identity)) !== plan.planSha256) fail("SNAPSHOT_PLAN_CHANGED");
  roots(plan.sourceRoot, plan.candidateRoot, io);
  const actual = enumerate(plan.candidateRoot, io);
  if (JSON.stringify(actual.files) !== JSON.stringify(plan.files.map(f => f.path).sort()) || JSON.stringify(actual.directories) !== JSON.stringify([...plan.directories].sort())) fail("SNAPSHOT_COVERAGE");
  for (const f of plan.files) if (sha(io.readFileSync(guarded(path.join(plan.candidateRoot, f.path), io))) !== f.sha256) fail("SNAPSHOT_TARGET_HASH");
  return { verified: true, files: plan.files.length, directories: plan.directories.length, independentRestore: false };
}
function create(source, manifest, expectedPlanSha256, io = fs) {
  if (!isHash(expectedPlanSha256)) fail("SNAPSHOT_PLAN_REQUIRED");
  const plan = prepare(source, manifest, io);
  if (plan.planSha256 !== expectedPlanSha256) fail("SNAPSHOT_PLAN_CHANGED");
  const target = plan.candidateRoot, privateRows = new Map(plan.files.some(f => f.path.startsWith("Ignore/")) ? privateInputs(source).map(f => [f.path, f.sha256]) : []);
  function mkdir(full) {
    guarded(full, io);
    if (exists(full, io)) { if (!io.lstatSync(full).isDirectory()) fail("SNAPSHOT_PARENT_FILE"); return; }
    mkdir(path.dirname(full)); io.mkdirSync(guarded(full, io));
  }
  // The target itself must be new. Never reuse even an existing empty target.
  mkdir(path.dirname(target)); io.mkdirSync(guarded(target, io));
  for (const p of plan.directories) mkdir(path.join(target, p));
  for (const f of plan.files) {
    const bytes = f.generated ? sdkBytes() : sourceRead(source, f, privateRows, io);
    const dest = guarded(path.join(target, f.path), io);
    io.writeFileSync(dest, bytes, { flag: "wx" });
    if (sha(io.readFileSync(guarded(dest, io))) !== f.sha256) fail("SNAPSHOT_TARGET_HASH");
  }
  for (const f of plan.files.filter(f => !f.generated)) sourceRead(source, f, privateRows, io);
  if (JSON.stringify(gitState(source, io)) !== JSON.stringify(plan.sourceGit)) fail("SNAPSHOT_SOURCE_GIT_CHANGED");
  return { ...verify(plan, io), copied: true, sourceUnchanged: true, planSha256: plan.planSha256, partialOutputsAutoDeleted: false };
}
function compare(before, actual, changes = []) {
  fileRows(before); fileRows(actual);
  if (!Array.isArray(changes)) fail("SNAPSHOT_CHANGES");
  const old = new Map(before.map(f => [f.path, f.sha256])), now = new Map(actual.map(f => [f.path, f.sha256])), expected = new Map(old), seen = new Set();
  for (const c of changes) {
    safe(c.path);
    if (seen.has(c.path.toLowerCase()) || ![c.beforeSha256, c.afterSha256].every(v => v === null || isHash(v)) || c.beforeSha256 !== (old.get(c.path) || null)) fail("SNAPSHOT_CHANGE_BASELINE");
    seen.add(c.path.toLowerCase());
    if (c.afterSha256 === null) expected.delete(c.path); else expected.set(c.path, c.afterSha256);
  }
  const differences = [...new Set([...expected.keys(), ...now.keys()])].sort().filter(p => expected.get(p) !== now.get(p)).map(p => ({ path: p, expectedSha256: expected.get(p) || null, actualSha256: now.get(p) || null }));
  return { matches: !differences.length, differences, independentRestore: false };
}
function rollbackPlan(before, attempt, current, backup) {
  [before, attempt, current, backup].forEach(fileRows);
  if (!compare(before, backup).matches) fail("SNAPSHOT_BACKUP_MISMATCH");
  const b = new Map(before.map(f => [f.path, f.sha256])), t = new Map(attempt.map(f => [f.path, f.sha256])), c = new Map(current.map(f => [f.path, f.sha256]));
  const actions = [], conflicts = [];
  for (const p of [...new Set([...b.keys(), ...t.keys(), ...c.keys()])].sort()) {
    const old = b.get(p), tried = t.get(p), now = c.get(p);
    if (now !== old && now !== tried) { conflicts.push(p); continue; }
    actions.push({ path: p, action: now === old ? "unchanged" : old === undefined ? "retain-new-output" : "restore-verified-backup", beforeSha256: old || null, attemptSha256: tried || null, currentSha256: now || null });
  }
  return { readyForReviewedRollback: !conflicts.length, conflicts, actions, writesPerformed: false, independentRestore: false };
}
function assessResults(required, results) {
  if (!Array.isArray(required) || !required.length || !Array.isArray(results)) fail("SNAPSHOT_RESULTS");
  const names = new Set();
  for (const r of required) { if (!r || !/^[a-z][a-z0-9-]*$/.test(r.name) || !isHash(r.inputSha256) || names.has(r.name)) fail("SNAPSHOT_RESULTS"); names.add(r.name); }
  const seen = new Set();
  for (const r of results) {
    if (!r || !names.has(r.name) || seen.has(r.name) || !isHash(r.inputSha256) || typeof r.executed !== "boolean" || !Number.isInteger(r.exitCode) || !Number.isInteger(r.unexpectedLogCount) || r.unexpectedLogCount < 0) fail("SNAPSHOT_RESULTS");
    seen.add(r.name);
  }
  const pending = required.filter(e => !results.some(r => r.name === e.name && r.executed && r.exitCode === 0 && r.unexpectedLogCount === 0 && r.inputSha256 === e.inputSha256)).map(r => r.name);
  return { requiredResultsSatisfied: !pending.length, pending, independentRestore: false };
}
// All exported failures contain diagnostic codes only, including injected I/O failures.
const boundary = fn => (...args) => { try { return fn(...args); } catch (e) { throw e instanceof audit.AuditError ? e : new audit.AuditError("SNAPSHOT_IO_FAILED_PARTIAL_RETAINED"); } };
function cli(args) {
  try {
    if (args.length !== 2 || args[0] !== "--check") fail("CLI_ARGUMENT");
    const root = path.resolve(__dirname, "../.."), manifest = audit.parseJson(audit.reader(root).read(safe(args[1])).toString("utf8"));
    const plan = boundary(prepare)(root, manifest);
    return { exitCode: 0, report: { planSha256: plan.planSha256, files: plan.files.length, directories: plan.directories.length, copied: false, independentRestore: false } };
  } catch (e) { return { exitCode: 2, report: { copied: false, independentRestore: false, diagnostics: [{ code: e instanceof audit.AuditError ? e.code : "SNAPSHOT_IO_FAILED_PARTIAL_RETAINED" }] } }; }
}
if (require.main === module) { const r = cli(process.argv.slice(2)); process.stdout.write(JSON.stringify(r.report, null, 2) + "\n"); process.exitCode = r.exitCode; }
module.exports = { prepare: boundary(prepare), create: boundary(create), verify: boundary(verify), readSource: boundary(readSource), privateInputs: boundary(privateInputs), compare: boundary(compare), rollbackPlan: boundary(rollbackPlan), assessResults: boundary(assessResults), cli };
