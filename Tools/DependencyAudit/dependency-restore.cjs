"use strict";
// Offline metadata/recipe inspection. Only --prepare-archives explicitly copies pinned tgz inputs.
const fs = require("node:fs"), path = require("node:path"), crypto = require("node:crypto"), zlib = require("node:zlib");
const audit = require("./dependency-audit.cjs");
const sha = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
const stable = value => JSON.stringify(value, (_, v) => v && typeof v === "object" && !Array.isArray(v) ? Object.fromEntries(Object.keys(v).sort().map(k => [k, v[k]])) : v);
const fail = (code, target) => { const error = new audit.AuditError(code); if (target) error.target = target; throw error; };
function relative(p) {
  const result = audit.relative(p);
  if (result !== p || /[\x00-\x1f\x7f:*?#[\]]|[. ](?:\/|$)/.test(p)) fail("RESTORE_PATH");
  return result;
}
function tarPackage(bytes) {
  let tar;
  try { tar = zlib.gunzipSync(bytes, { maxOutputLength: 64 * 1024 * 1024 }); } catch (_) { fail("RESTORE_ARCHIVE_GZIP"); }
  let metadata = null, ended = false, offset = 0; const seen = new Set();
  const field = bytes => bytes.toString("utf8").split("\0")[0];
  function octal(bytes) { const text = field(bytes).trim(); if (!/^[0-7]+$/.test(text)) fail("RESTORE_TAR_NUMBER"); return parseInt(text, 8); }
  while (offset + 512 <= tar.length) {
    const h = tar.subarray(offset, offset + 512);
    if (h.every(v => v === 0)) { if (!tar.subarray(offset).every(v => v === 0)) fail("RESTORE_TAR_TRAILING"); ended = true; break; }
    const name = (field(h.subarray(345, 500)) ? field(h.subarray(345, 500)) + "/" : "") + field(h.subarray(0, 100));
    const normalized = name.replace(/\/$/, "");
    if (!(normalized === "package" || normalized.startsWith("package/")) || relative(normalized) !== normalized || seen.has(normalized)) fail("RESTORE_TAR_PATH"); seen.add(normalized);
    const size = octal(h.subarray(124, 136)), checksum = octal(h.subarray(148, 156));
    let sum = 0; for (let i = 0; i < 512; i++) sum += i >= 148 && i < 156 ? 32 : h[i];
    if (sum !== checksum || ![0, 48, 53].includes(h[156]) || offset + 512 + size > tar.length) fail("RESTORE_TAR_HEADER");
    if (name === "package/package.json") { if (h[156] === 53) fail("RESTORE_TAR_HEADER"); metadata = audit.parseJson(tar.subarray(offset + 512, offset + 512 + size).toString("utf8")); }
    offset += 512 + Math.ceil(size / 512) * 512;
  }
  if (!ended || !metadata || typeof metadata.name !== "string" || typeof metadata.version !== "string") fail("RESTORE_ARCHIVE_METADATA");
  return metadata;
}
function discover(root, disk, read) {
  const cache = "Library/PackageCache", found = new Map();
  // reader validates all parents and rejects a cache root or package junction before reading metadata.
  if (disk.stat("Library").isSymbolicLink() || disk.stat(cache).isSymbolicLink()) fail("RESTORE_CACHE_LINK");
  for (const name of fs.readdirSync(path.join(root, cache)).sort()) {
    const rel = relative(cache + "/" + name);
    if (disk.stat(rel).isSymbolicLink()) fail("RESTORE_CACHE_LINK");
    const metadata = audit.parseJson(read(rel + "/package.json").toString("utf8"));
    if (typeof metadata.name !== "string" || !/^[a-z0-9][a-z0-9._-]+$/.test(metadata.name) || typeof metadata.version !== "string") fail("RESTORE_CACHE_METADATA");
    if (found.has(metadata.name)) fail("RESTORE_PROVIDER_DUPLICATE");
    found.set(metadata.name, { path: rel, metadata });
  }
  return found;
}
function fileSpec(value) {
  if (typeof value !== "string" || !value.startsWith("file:")) fail("RESTORE_LOCAL_SPEC");
  return relative(value.slice(5).replace(/\\/g, "/"));
}
function npmPlan(manifest, lock, spec, provider, read) {
  if (!manifest || !lock || lock.lockfileVersion !== 3 || !lock.packages || !lock.packages[""] || !Array.isArray(spec.npm) || !spec.npm.length) fail("RESTORE_NPM_SCHEMA");
  for (const group of ["dependencies", "devDependencies", "optionalDependencies"])
    if (stable(manifest[group] || {}) !== stable(lock.packages[""][group] || {})) fail("RESTORE_NPM_ROOT_DRIFT");
  if (manifest.ccaVersion !== spec.cloudCode.version) fail("RESTORE_CCA_VERSION");
  const local = [];
  for (const group of ["dependencies", "devDependencies", "optionalDependencies"]) for (const [alias, requested] of Object.entries(manifest[group] || {})) if (requested.startsWith("file:")) local.push({ group, alias, requested });
  if (local.length !== spec.npm.length || new Set(spec.npm.map(v => v.alias)).size !== spec.npm.length) fail("RESTORE_NPM_COVERAGE");
  const archives = [], manifestOperations = [], lockOperations = [], clone = structuredClone(manifest), proposedLock = structuredClone(lock);
  for (const row of local) {
    const pin = spec.npm.find(v => v.alias === row.alias && v.group === row.group), node = lock.packages["node_modules/" + row.alias];
    if (!pin || !node || !/^[a-f0-9]{64}$/.test(pin.sha256) || !/^[a-z0-9.-]+\.tgz$/.test(pin.filename)) fail("RESTORE_NPM_PIN");
    const declaredPath = fileSpec(row.requested), resolvedPath = fileSpec(node.resolved);
    if (declaredPath !== resolvedPath || path.posix.basename(declaredPath) !== pin.filename) fail("RESTORE_NPM_PATH_DRIFT");
    const currentSource = provider.path + "/UnityServicesNpmLocal~/" + pin.filename;
    const destination = "Ignore/DependencyArchives/npm/" + spec.cloudCode.id + "/" + spec.cloudCode.version + "/" + pin.sha256 + "/" + pin.filename;
    const parts = declaredPath.split("/"), providerFolder = parts[2] || "";
    const fromProvider = parts.length === 5 && parts[0] === "Library" && parts[1] === "PackageCache" && (providerFolder === spec.cloudCode.id || providerFolder.startsWith(spec.cloudCode.id + "@")) && parts[3] === "UnityServicesNpmLocal~" && parts[4] === pin.filename;
    if (!fromProvider && declaredPath !== destination) fail("RESTORE_NPM_SOURCE");
    const bytes = read(currentSource), metadata = tarPackage(bytes), integrity = "sha512-" + crypto.createHash("sha512").update(bytes).digest("base64");
    if (sha(bytes) !== pin.sha256) fail("RESTORE_NPM_ARCHIVE_HASH");
    if (metadata.name !== pin.name || metadata.version !== pin.version || node.version !== pin.version || (node.name && node.name !== pin.name)) fail("RESTORE_NPM_VERSION_ALIAS");
    if (node.integrity !== integrity || pin.integrity !== integrity || stable(node.dependencies || {}) !== stable(metadata.dependencies || {})) fail("RESTORE_NPM_INTEGRITY_GRAPH");
    const value = "file:" + destination;
    const operation = { group: row.group, alias: row.alias, from: row.requested, to: value };
    manifestOperations.push(operation); clone[row.group][row.alias] = value;
    lockOperations.push({ section: "root", group: row.group, alias: row.alias, from: lock.packages[""][row.group][row.alias], to: value }, { section: "package", alias: row.alias, from: node.resolved, to: value });
    proposedLock.packages[""][row.group][row.alias] = value; proposedLock.packages["node_modules/" + row.alias].resolved = value;
    archives.push({ alias: row.alias, name: metadata.name, version: metadata.version, source: currentSource, target: destination, sha256: pin.sha256, integrity, bytes: bytes.length });
  }
  // Reverse only planned path fields to prove aliases, versions, integrity and all other data are unchanged.
  const reverseManifest = structuredClone(clone), reverseLock = structuredClone(proposedLock);
  for (const op of manifestOperations) reverseManifest[op.group][op.alias] = op.from;
  for (const op of lockOperations) if (op.section === "root") reverseLock.packages[""][op.group][op.alias] = op.from; else reverseLock.packages["node_modules/" + op.alias].resolved = op.from;
  if (stable(reverseManifest) !== stable(manifest) || stable(reverseLock) !== stable(lock)) fail("RESTORE_UNEXPECTED_PROPOSAL_CHANGE");
  return { archives, proposal: { status: "path-mapping-only; not an installable regenerated lock", manifestOperations, lockOperations, proposedManifest: clone, originalAliasesVersionsIntegrityPreserved: true, requiresUserLockRegenerationBeforeApplying: true, installed: false } };
}
function availability(input) {
  if (input.archivePresent) return { status: "local-input-present", remoteAcquisitionVerified: false };
  if (input.requiresAccount && !input.entitlementVerified) return { status: "blocked", code: "ACQUISITION_ENTITLEMENT_REQUIRED", next: "Owner verifies entitlement for exact provider/version; no alternate version installation" };
  if (input.network === "offline") return { status: "blocked", code: "ACQUISITION_OFFLINE_MISSING", next: "Prepare the exact pinned archive from an authorized source; preserve current inputs" };
  return { status: "pending", code: "ACQUISITION_UNVERIFIED", next: "Owner verifies source/account/network when actual reacquisition is needed" };
}
function validateMigration(originalManifest, originalLock, manifest, lock, proposal) {
  if (stable(manifest) !== stable(proposal.proposedManifest) || lock.lockfileVersion !== originalLock.lockfileVersion) fail("RESTORE_MIGRATION_MANIFEST");
  const reverse = structuredClone(lock);
  for (const op of proposal.lockOperations) {
    const holder = op.section === "root" ? reverse.packages[""][op.group] : reverse.packages["node_modules/" + op.alias];
    const key = op.section === "root" ? op.alias : "resolved";
    if (!holder || holder[key] !== op.to) fail("RESTORE_MIGRATION_PATH"); holder[key] = op.from;
  }
  if (stable(reverse) !== stable(originalLock)) fail("RESTORE_MIGRATION_GRAPH_CHANGED");
  return { migrationMetadataValid: true, aliasVersionsIntegrityGraphPreserved: true, installed: false, independentRestore: false };
}
function check(root, spec) {
  if (!spec || spec.schemaVersion !== 1 || !spec.tmp || !spec.cloudCode || !spec.hashes) fail("RESTORE_SPEC");
  const disk = audit.reader(root), gitBefore = audit.gitSnapshot(audit.gitReader(root)), index = sha(disk.read(".git/index")), inputs = new Map();
  const read = p => { relative(p); let bytes; try { bytes = disk.read(p); } catch (e) { if (e.code === "IO_READ_FAILED") fail("RESTORE_INPUT_UNAVAILABLE", p); throw e; } inputs.set(p, sha(bytes)); return bytes; };
  for (const pin of spec.hashes) if (sha(read(pin.path)) !== pin.sha256) fail("RESTORE_DECLARATION_CHANGED");
  const editor = read("ProjectSettings/ProjectVersion.txt").toString("utf8").match(/^m_EditorVersion:\s*(\S+)/m);
  if (!editor || editor[1] !== spec.editorVersion) fail("RESTORE_EDITOR_VERSION");
  const manifest = audit.parseJson(read("Packages/manifest.json").toString("utf8")), lock = audit.parseJson(read("Packages/packages-lock.json").toString("utf8")), graph = audit.graph(manifest, lock);
  if (graph.diagnostics.some(v => v.severity === "error")) fail("RESTORE_UPM_GRAPH");
  const found = discover(root, disk, read);
  for (const node of graph.nodes) {
    const installed = found.get(node.id); if (!installed) fail("RESTORE_PROVIDER_MISSING"); if (installed.metadata.version !== node.version) fail("RESTORE_PROVIDER_VERSION");
    if (stable(installed.metadata.dependencies || {}) !== stable(lock.dependencies[node.id].dependencies || {})) fail("RESTORE_PROVIDER_GRAPH");
  }
  const provider = pin => { const item = found.get(pin.id); if (!item) fail("RESTORE_PROVIDER_MISSING"); if (item.metadata.version !== pin.version) fail("RESTORE_PROVIDER_VERSION"); return item; };
  const tmp = provider(spec.tmp), cloud = provider(spec.cloudCode), tmpPath = tmp.path + "/Package Resources/TMP Essential Resources.unitypackage";
  if (sha(read(tmpPath)) !== spec.tmp.sha256) fail("RESTORE_TMP_ARCHIVE_HASH");
  const npm = npmPlan(audit.parseJson(read("package.json").toString("utf8")), audit.parseJson(read("package-lock.json").toString("utf8")), spec, cloud, read);
  const csprojPath = "UGS/Modules/FlowStateVerification/FlowStateVerification.csproj", csproj = read(csprojPath).toString("utf8");
  const packageReferences = [...csproj.matchAll(/<PackageReference\s+Include="([^"]+)"\s+Version="([^"]+)"\s*\/>/g)].map(m => ({ id: m[1], version: m[2] }));
  const resources = [...csproj.matchAll(/<EmbeddedResource\s+Include="([^"]+)"\s+LogicalName="([^"]+)"\s*\/>/g)].map(m => ({ include: m[1], logicalName: m[2] }));
  if (packageReferences.length !== 4 || resources.length !== 2 || !csproj.includes("<TargetFramework>net9.0</TargetFramework>")) fail("RESTORE_NUGET_DECLARATION");
  const resourceDiagnostics = [], resourceFiles = disk.walk("UGS/CloudCode", resourceDiagnostics).filter(r => !r.directory && r.path.endsWith(".js")).map(r => r.path);
  if (resourceDiagnostics.length || !resourceFiles.length) fail("RESTORE_EMBEDDED_RESOURCE_MISSING");
  read("UGS/Modules/FlowStateVerification/runtime.js");
  const assetsPath = "UGS/Modules/FlowStateVerification/obj/project.assets.json"; let resolvedGraph;
  try { const assets = audit.parseJson(read(assetsPath).toString("utf8")); if (!assets.libraries || !Object.keys(assets.libraries).length) fail("RESTORE_NUGET_GRAPH_EMPTY"); resolvedGraph = { status: "local generated graph only; not a committed lock or fresh restore", packages: Object.entries(assets.libraries).filter(([, v]) => v.type === "package").map(([id]) => id).sort(), feeds: Object.keys(assets.project.restore.sources || {}).map(audit.publicUrl) }; }
  catch (e) { if (disk.io.existsSync(path.join(root, assetsPath))) throw e; resolvedGraph = { status: "missing; owner obtains graph with isolated dotnet restore --use-lock-file", packages: [], feeds: [] }; }
  if (resolvedGraph.packages.length && packageReferences.some(p => !resolvedGraph.packages.some(v => v.toLowerCase() === (p.id + "/" + p.version).toLowerCase()))) fail("RESTORE_NUGET_GRAPH_VERSION");
  if (resolvedGraph.packages.length && spec.nuget && stable(resolvedGraph.packages) !== stable(spec.nuget.resolvedPackages)) fail("RESTORE_NUGET_GRAPH_DRIFT");
  const logManifest = audit.parseJson(read("UGS/LogSink/package.json").toString("utf8")), docker = read("UGS/LogSink/Dockerfile").toString("utf8");
  if (Object.keys(logManifest.dependencies || {}).length || Object.keys(logManifest.devDependencies || {}).length || !docker.includes("FROM node:22-alpine")) fail("RESTORE_LOGSINK_INPUT");
  for (const [p, digest] of inputs) if (sha(disk.read(p)) !== digest) fail("RESTORE_INPUT_CHANGED");
  if (sha(disk.read(".git/index")) !== index || stable(gitBefore) !== stable(audit.gitSnapshot(audit.gitReader(root)))) fail("RESTORE_INDEX_CHANGED");
  return { schemaVersion: 1, staticInputsValid: true, editorVersion: spec.editorVersion, upm: { packages: graph.nodes.length, edges: graph.edges.length, metadataMatched: true },
    providers: [{ id: spec.tmp.id, version: spec.tmp.version, discoveredPath: tmp.path }, { id: spec.cloudCode.id, version: spec.cloudCode.version, discoveredPath: cloud.path }], tmpArchive: { path: tmpPath, sha256: spec.tmp.sha256 }, ...npm,
    auxiliary: { nuget: { targetFramework: "net9.0", packageReferences, resources, embeddedSourcePaths: resourceFiles, sourceBodiesNotInspected: true, resolvedGraph, lockStatus: "No committed packages.lock.json; exact graph/lock generation is user work in isolated environment" }, logSink: { name: logManifest.name, directExternalDependencies: 0, containerBase: "node:22-alpine", immutableImageDigestVerified: false } },
    acquisition: availability({ archivePresent: true }), inputs: [...inputs].map(([p, digest]) => ({ path: p, sha256: digest })), indexUnchanged: true,
    installed: false, lockRegenerated: false, unityExecuted: false, rightsGranted: false, independentRestore: false };
}
function guard(root, rel) {
  relative(rel); const absolute = path.resolve(root); let cursor = absolute;
  if (fs.lstatSync(absolute).isSymbolicLink()) fail("RESTORE_OUTPUT_LINK");
  for (const part of rel.split("/")) { cursor = path.join(cursor, part); try { if (fs.lstatSync(cursor).isSymbolicLink()) fail("RESTORE_OUTPUT_LINK"); } catch (e) { if (e.code === "ENOENT") break; throw e; } }
  return path.join(absolute, rel);
}
function prepareArchives(root, spec, io = fs) {
  const report = check(root, spec), disk = audit.reader(root), git = audit.gitReader(root), snapshot = audit.gitSnapshot(git), indexHash = sha(disk.read(".git/index")), paths = report.archives.map(r => r.target);
  const ignored = audit.ignoreResult(git(["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"], paths.join("\0") + "\0"), paths);
  if (paths.some((p, i) => !ignored[i].ignored || snapshot.tracked.some(t => t.toLowerCase() === p.toLowerCase()))) fail("RESTORE_ARCHIVES_NOT_PRIVATE");
  let created = 0, reused = 0;
  try {
    for (const row of report.archives) {
      if (!row.target.startsWith("Ignore/DependencyArchives/npm/")) fail("RESTORE_OUTPUT_ROOT");
      const bytes = disk.read(row.source); if (sha(bytes) !== row.sha256) fail("RESTORE_ARCHIVE_CHANGED");
      const target = guard(root, row.target);
      if (fs.existsSync(target)) { if (sha(disk.read(row.target)) !== row.sha256) fail("RESTORE_TARGET_CONFLICT"); reused++; continue; }
      const parts = row.target.split("/"); for (let i = 1; i < parts.length; i++) { const dir = guard(root, parts.slice(0, i).join("/")); if (!fs.existsSync(dir)) io.mkdirSync(dir); }
      io.writeFileSync(guard(root, row.target), bytes, { flag: "wx" }); if (sha(disk.read(row.target)) !== row.sha256) fail("RESTORE_TARGET_HASH"); created++;
    }
    for (const input of report.inputs) if (sha(disk.read(input.path)) !== input.sha256) fail("RESTORE_INPUT_CHANGED");
    if (stable(snapshot) !== stable(audit.gitSnapshot(git)) || sha(disk.read(".git/index")) !== indexHash) fail("RESTORE_INDEX_CHANGED");
    return { prepared: true, created, reused, files: paths.length, archiveHashesMatch: true, ignoredUntracked: true, installed: false, lockRegenerated: false, independentRestore: false };
  } catch (error) { if (error instanceof audit.AuditError) throw error; fail("RESTORE_COPY_FAILED_PARTIAL_RETAINED"); }
}
function cli(args) {
  try {
    if (!["--check", "--prepare-archives", "--verify-migration"].includes(args[0]) || args.length !== (args[0] === "--verify-migration" ? 4 : 2)) fail("CLI_ARGUMENT");
    const root = path.resolve(__dirname, "../.."), disk = audit.reader(root), parse = p => audit.parseJson(disk.read(relative(p)).toString("utf8")), spec = parse(args[1]);
    let report;
    if (args[0] === "--verify-migration") report = validateMigration(parse("package.json"), parse("package-lock.json"), parse(args[2]), parse(args[3]), check(root, spec).proposal);
    else report = args[0] === "--check" ? check(root, spec) : prepareArchives(root, spec);
    return { exitCode: 0, report };
  } catch (error) {
    const code = error instanceof audit.AuditError ? error.code : "RESTORE_READ_FAILED";
    const actions = { RESTORE_PROVIDER_MISSING: "Owner resolves pinned manifest/lock using the pinned Unity Editor, then reruns discovery; do not install another version", RESTORE_PROVIDER_VERSION: "Reacquire the exact provider version; preserve current workspace/cache", RESTORE_INPUT_UNAVAILABLE: "Check the named input exists and is readable; reacquire exact provider/archive without replacing source declarations", RESTORE_NPM_ARCHIVE_HASH: "Reacquire the exact Cloud Code provider archive; stop on changed bytes", RESTORE_NPM_PATH_DRIFT: "Review manifest/lock path mapping; regenerate lock with user-run npm only after proposed manifest review", RESTORE_COPY_FAILED_PARTIAL_RETAINED: "Inspect partial ignored archive targets; retry only when existing bytes match; never overwrite or auto-delete" };
    return { exitCode: 2, report: { staticInputsValid: false, installed: false, independentRestore: false, diagnostics: [{ code, target: error.target || null, next: actions[code] || "Preserve inputs; review exact provider/version/archive/declaration. Reacquisition/install is user work, never auto-select another version." }] } };
  }
}
if (require.main === module) { const result = cli(process.argv.slice(2)); process.stdout.write(JSON.stringify(result.report, null, 2) + "\n"); process.exitCode = result.exitCode; }
module.exports = { tarPackage, discover, fileSpec, npmPlan, availability, validateMigration, check, prepareArchives, cli };
