"use strict";
// Explicit preservation writes only. Verification/prepare/require never copy or modify input files.
const fs = require("node:fs"), path = require("node:path"), os = require("node:os"), crypto = require("node:crypto");
const audit = require("./dependency-audit.cjs");
const hash = b => crypto.createHash("sha256").update(b).digest("hex");
const encode = v => Buffer.from(JSON.stringify(v, null, 2) + "\n", "utf8");
const fail = code => { throw new audit.AuditError(code); };
const isHash = v => typeof v === "string" && /^[a-f0-9]{64}$/.test(v);
function safe(p) {
  if (audit.relative(p) !== p || /[\x00-\x1f\x7f:*?#[\]]|[. ](?:\/|$)/.test(p)) fail("PRESERVATION_PATH");
  return p;
}
function guard(root, p) {
  const absolute = path.resolve(root), full = path.resolve(absolute, safe(p));
  if (!full.startsWith(absolute + path.sep) || fs.lstatSync(absolute).isSymbolicLink()) fail("PRESERVATION_PATH");
  let cursor = absolute;
  const parts = p.split("/");
  for (let i = 0; i < parts.length; i++) {
    cursor = path.join(cursor, parts[i]);
    let stat;
    try { stat = fs.lstatSync(cursor); } catch (e) { if (e.code === "ENOENT") break; throw e; }
    if (stat.isSymbolicLink()) fail("PRESERVATION_LINK");
    if (i < parts.length - 1 && !stat.isDirectory()) fail("PRESERVATION_PARENT_FILE");
  }
  return full;
}
function mkdirParents(root, p, io) {
  const pieces = safe(p).split("/");
  for (let i = 1; i < pieces.length; i++) {
    const rel = pieces.slice(0, i).join("/"), full = guard(root, rel);
    if (!fs.existsSync(full)) io.mkdirSync(full);
    if (!fs.lstatSync(full).isDirectory()) fail("PRESERVATION_PARENT_FILE");
  }
}
function write(root, p, bytes, io) {
  mkdirParents(root, p, io);
  io.writeFileSync(guard(root, p), bytes, { flag: "wx" });
}
function state(root) {
  const disk = audit.reader(root);
  return { git: audit.gitSnapshot(audit.gitReader(root)), index: hash(disk.read(".git/index")) };
}
function storageGate(root, output, files = []) {
  safe(output);
  if (!/^Ignore\/DependencyPreservation\/[a-z][a-z0-9_-]{2,63}\/[a-f0-9]{64}$/.test(output)) fail("PRESERVATION_OUTPUT_ROOT");
  guard(root, output);
  const git = audit.gitReader(root), snapshot = audit.gitSnapshot(git), query = [output + "/", output + "/manifest.json", ...files.map(p => output + "/files/" + safe(p))];
  const result = audit.ignoreResult(git(["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"], query.join("\0") + "\0"), query);
  if (result.some(r => !r.ignored) || snapshot.tracked.some(p => p.toLowerCase() === output.toLowerCase() || p.toLowerCase().startsWith(output.toLowerCase() + "/"))) fail("PRESERVATION_NOT_PRIVATE");
}
function sourceAllowed(p, provider) {
  return p === "Assets/TextMesh Pro.meta" || p.startsWith("Assets/TextMesh Pro/") || p === provider.archivePath;
}
function metadata(p, bytes) {
  let guid = null;
  if (p.endsWith(".meta")) {
    const matches = [...bytes.toString("utf8").matchAll(/^guid:\s*([a-f0-9]{32})\s*$/gm)];
    if (matches.length !== 1 || /^0+$/.test(matches[0][1])) fail("PRESERVATION_META");
    guid = matches[0][1];
  }
  const resource = p.match(/\/Resources\/(.+)\.[^/.]+$/);
  return { path: p, bytes: bytes.length, sha256: hash(bytes), guid, folderMeta: p.endsWith(".meta") && /^folderAsset:\s*yes\s*$/m.test(bytes.toString("utf8")),
    resourcesName: resource && !p.endsWith(".meta") ? resource[1] : null,
    shaderNames: /\.shader$/.test(p) ? [...bytes.toString("utf8").matchAll(/^\s*Shader\s+"([^"]+)"/gm)].map(m => m[1]) : [] };
}
function expectation(manifest, digest) {
  return { id: manifest.id, manifestSha256: digest, editorVersion: manifest.editorVersion,
    providerId: manifest.provider.id, providerVersion: manifest.provider.version, archiveSha256: manifest.provider.archiveSha256 };
}
function prepare(root, spec) {
  if (!spec || spec.schemaVersion !== 1 || !/^[a-z][a-z0-9_-]{2,63}$/.test(spec.id) || !spec.provider || !Array.isArray(spec.files) || !spec.files.length) fail("PRESERVATION_SPEC");
  const provider = spec.provider;
  safe(provider.archivePath);
  if (provider.id !== "com.unity.ugui" || !/^\d+\.\d+\.\d+$/.test(provider.version) || !isHash(provider.archiveSha256) || !/^Library\/PackageCache\/[^/]+\/Package Resources\/TMP Essential Resources\.unitypackage$/.test(provider.archivePath)) fail("PRESERVATION_PROVIDER");
  const disk = audit.reader(root), initial = state(root), diagnostics = [], rows = disk.walk("Assets/TextMesh Pro", diagnostics);
  if (diagnostics.length) fail("PRESERVATION_ENUMERATION");
  const required = [...rows.filter(r => !r.directory).map(r => r.path), "Assets/TextMesh Pro.meta", provider.archivePath].sort();
  const supplied = spec.files.map(f => safe(f.path)).sort();
  if (new Set(supplied.map(p => p.toLowerCase())).size !== supplied.length || JSON.stringify(required) !== JSON.stringify(supplied)) fail("PRESERVATION_COVERAGE");
  const editor = disk.read("ProjectSettings/ProjectVersion.txt").toString("utf8").match(/^m_EditorVersion:\s*(\S+)/m);
  if (!editor || editor[1] !== spec.editorVersion) fail("PRESERVATION_EDITOR_VERSION");
  const providerJsonPath = provider.archivePath.split("/Package Resources/")[0] + "/package.json";
  const installed = audit.parseJson(disk.read(providerJsonPath).toString("utf8"));
  if (installed.name !== provider.id || installed.version !== provider.version) fail("PRESERVATION_PROVIDER_VERSION");
  const files = [];
  for (const p of supplied) {
    const entry = spec.files.find(f => f.path === p);
    if (!sourceAllowed(p, provider) || !isHash(entry.sha256)) fail("PRESERVATION_INPUT_SCOPE");
    const info = metadata(p, disk.read(p));
    if (info.sha256 !== entry.sha256) fail("PRESERVATION_SOURCE_HASH");
    files.push(info);
  }
  const archive = files.find(f => f.path === provider.archivePath);
  if (!archive || archive.sha256 !== provider.archiveSha256) fail("PRESERVATION_ARCHIVE_HASH");
  const guids = files.filter(f => f.guid).map(f => f.guid);
  if (new Set(guids).size !== guids.length) fail("PRESERVATION_DUPLICATE_GUID");
  for (const f of files.filter(f => f.guid)) {
    const paired = guard(root, f.path.slice(0, -5));
    if (!fs.existsSync(paired) || (f.folderMeta ? !fs.lstatSync(paired).isDirectory() : !fs.lstatSync(paired).isFile())) fail("PRESERVATION_META_PAIR");
  }
  for (const f of files) if (hash(disk.read(f.path)) !== f.sha256) fail("PRESERVATION_INPUT_CHANGED");
  if (JSON.stringify(initial) !== JSON.stringify(state(root))) fail("PRESERVATION_INDEX_CHANGED");
  const manifest = { schemaVersion: 2, id: spec.id, editorVersion: spec.editorVersion, provider: { ...provider }, files };
  const digest = hash(encode(manifest)), output = "Ignore/DependencyPreservation/" + spec.id + "/" + digest;
  storageGate(root, output, files.map(f => f.path));
  return { manifest, output, expected: expectation(manifest, digest), indexUnchanged: true, files: files.length, applied: false, independentRestore: false };
}
function manifestAt(root, output, expected) {
  if (!expected || !isHash(expected.manifestSha256) || output !== "Ignore/DependencyPreservation/" + expected.id + "/" + expected.manifestSha256) fail("PRESERVATION_EXPECTED_OUTPUT");
  storageGate(root, output);
  const disk = audit.reader(root), bytes = disk.read(output + "/manifest.json");
  if (hash(bytes) !== expected.manifestSha256) fail("PRESERVATION_MANIFEST_HASH");
  const manifest = audit.parseJson(bytes.toString("utf8"));
  if (manifest.schemaVersion !== 2 || manifest.id !== expected.id || manifest.editorVersion !== expected.editorVersion || !manifest.provider || manifest.provider.id !== expected.providerId || manifest.provider.version !== expected.providerVersion || manifest.provider.archiveSha256 !== expected.archiveSha256 || !Array.isArray(manifest.files) || !manifest.files.length) fail("PRESERVATION_IDENTITY");
  const paths = new Set();
  for (const file of manifest.files) {
    safe(file.path);
    if (!sourceAllowed(file.path, manifest.provider) || !isHash(file.sha256) || paths.has(file.path.toLowerCase())) fail("PRESERVATION_MANIFEST_SCHEMA");
    paths.add(file.path.toLowerCase());
  }
  storageGate(root, output, manifest.files.map(f => f.path));
  return manifest;
}
function verify(root, output, expected) {
  const manifest = manifestAt(root, output, expected), disk = audit.reader(root), diagnostics = [], rows = disk.walk(output, diagnostics, true);
  if (diagnostics.length) fail("PRESERVATION_ENUMERATION");
  const wanted = [output + "/manifest.json", ...manifest.files.map(f => output + "/files/" + f.path)].sort();
  if (JSON.stringify(rows.filter(r => !r.directory).map(r => r.path).sort()) !== JSON.stringify(wanted)) fail("PRESERVATION_BUNDLE_COVERAGE");
  for (const file of manifest.files) {
    const bytes = disk.read(output + "/files/" + file.path), actual = metadata(file.path, bytes);
    if (JSON.stringify(actual) !== JSON.stringify(file)) fail("PRESERVATION_PAYLOAD_CHANGED");
    if (file.folderMeta) {
      const paired = guard(root, output + "/files/" + file.path.slice(0, -5));
      if (!fs.existsSync(paired) || !fs.lstatSync(paired).isDirectory()) fail("PRESERVATION_FOLDER_MISSING");
    }
  }
  return { verified: true, files: manifest.files.length, manifestSha256: expected.manifestSha256, storageIgnoredUntracked: true, independentRestore: false, rightsGranted: false };
}
function create(root, spec, output, io = fs) {
  const prepared = prepare(root, spec);
  if (output !== prepared.output) fail("PRESERVATION_EXPECTED_OUTPUT");
  const full = guard(root, output);
  if (fs.existsSync(full)) fail("PRESERVATION_OVERWRITE");
  const initial = state(root), disk = audit.reader(root);
  try {
    mkdirParents(root, output, io); io.mkdirSync(guard(root, output));
    for (const file of prepared.manifest.files) {
      const bytes = disk.read(file.path);
      if (hash(bytes) !== file.sha256) fail("PRESERVATION_INPUT_CHANGED");
      write(root, output + "/files/" + file.path, bytes, io);
    }
    for (const file of prepared.manifest.files.filter(f => f.folderMeta)) mkdirParents(root, output + "/files/" + file.path.slice(0, -5) + "/.folder-sentinel", io);
    // The manifest is written last; a partial directory cannot masquerade as a completed bundle.
    if (JSON.stringify(prepare(root, spec).manifest) !== JSON.stringify(prepared.manifest) || JSON.stringify(initial) !== JSON.stringify(state(root))) fail("PRESERVATION_INPUT_CHANGED");
    write(root, output + "/manifest.json", encode(prepared.manifest), io);
    const verified = verify(root, output, prepared.expected);
    return { ...verified, output, sourceUnchanged: true, indexUnchanged: true };
  } catch (error) {
    if (error instanceof audit.AuditError) throw error;
    fail("PRESERVATION_COPY_FAILED_PARTIAL_RETAINED");
  }
}
function reapplyToTemporary(root, output, expected, target, io = fs) {
  verify(root, output, expected);
  const tempBase = fs.realpathSync(os.tmpdir()), absolute = path.resolve(target);
  if (path.dirname(absolute).toLowerCase() !== tempBase.toLowerCase() || !path.basename(absolute).startsWith("flowstate-preservation-reapply-") || fs.lstatSync(absolute).isSymbolicLink() || fs.readdirSync(absolute).length) fail("PRESERVATION_TEMP_TARGET");
  const manifest = manifestAt(root, output, expected), disk = audit.reader(root);
  try {
    for (const file of manifest.files) write(absolute, file.path, disk.read(output + "/files/" + file.path), io);
    for (const file of manifest.files.filter(f => f.folderMeta)) mkdirParents(absolute, file.path.slice(0, -5) + "/.folder-sentinel", io);
    const reapplied = audit.reader(absolute), diagnostics = [], actual = [];
    for (const folder of ["Assets", "Library"]) actual.push(...reapplied.walk(folder, diagnostics, true));
    if (diagnostics.length || JSON.stringify(actual.filter(r => !r.directory).map(r => r.path).sort()) !== JSON.stringify(manifest.files.map(f => f.path).sort())) fail("PRESERVATION_REAPPLY_COVERAGE");
    for (const file of manifest.files) {
      if (JSON.stringify(metadata(file.path, reapplied.read(file.path))) !== JSON.stringify(file)) fail("PRESERVATION_REAPPLY_HASH");
      if (file.folderMeta && !fs.lstatSync(guard(absolute, file.path.slice(0, -5))).isDirectory()) fail("PRESERVATION_FOLDER_MISSING");
    }
    verify(root, output, expected);
    return { reappliedToTemporary: true, files: manifest.files.length, contentHashesGuidsImportAndNamesMatch: true, unityExecuted: false, independentRestore: false };
  } catch (error) {
    if (error instanceof audit.AuditError) throw error;
    fail("PRESERVATION_REAPPLY_FAILED_PARTIAL_RETAINED");
  }
}
function cli(args) {
  try {
    const mode = args[0];
    if (!["--check", "--create", "--verify"].includes(mode) || args.length !== (mode === "--check" ? 2 : 3)) fail("CLI_ARGUMENT");
    const root = path.resolve(__dirname, "../.."), disk = audit.reader(root), input = audit.parseJson(disk.read(safe(args[1])).toString("utf8"));
    let result;
    if (mode === "--verify") result = verify(root, args[2], input.expected || input);
    else result = mode === "--check" ? prepare(root, input) : create(root, input, args[2]);
    return { exitCode: 0, report: result };
  } catch (error) { return { exitCode: 2, report: { ok: false, independentRestore: false, diagnostics: [{ code: error instanceof audit.AuditError ? error.code : "PRESERVATION_IO_FAILED" }] } }; }
}
if (require.main === module) { const result = cli(process.argv.slice(2)); process.stdout.write(JSON.stringify(result.report, null, 2) + "\n"); process.exitCode = result.exitCode; }
module.exports = { prepare, verify, create, reapplyToTemporary, cli };
