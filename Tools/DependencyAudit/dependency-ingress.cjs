"use strict";
// Git object inspection only: no staging, hooks, settings, extraction or filesystem writes.
const path = require("node:path"), crypto = require("node:crypto"), cp = require("node:child_process");
const audit = require("./dependency-audit.cjs"), registry = require("./dependency-registry.cjs");
const sha = b => crypto.createHash("sha256").update(b).digest("hex");
const fail = code => { throw new audit.AuditError(code); };
const decode = bytes => new TextDecoder("utf-8", { fatal: true }).decode(bytes);
const oid = value => /^[a-f0-9]{40}(?:[a-f0-9]{24})?$/.test(value);
const match = (p, root) => p.toLowerCase() === root.toLowerCase() || p.toLowerCase() === (root + ".meta").toLowerCase() || p.toLowerCase().startsWith(root.toLowerCase() + "/");
const sensitive = p => !/\.meta$/i.test(p) && (/(?:^|\/)(?:\.env(?:\..*)?|.*(?:credentials|secret|receipt|account[-_]data).*)$/i.test(p) || /\/OnlineRecordProductionConfiguration\.cs$/i.test(p));
function textHash(bytes) {
  if (bytes.includes(0)) return null;
  try { return sha(Buffer.from(decode(bytes).replace(/\r\n/g, "\n"))); } catch (_) { return null; }
}
function gitReader(root) {
  const allowed = [
    ["rev-parse", "HEAD"], ["ls-tree", "-r", "-z", "HEAD"], ["ls-files", "--stage", "-z"], ["cat-file", "--batch"],
    ["diff", "--no-ext-diff", "--no-textconv", "--cached", "--find-renames", "--find-copies", "--find-copies-harder", "--name-status", "-z"]
  ];
  return (args, input) => {
    if (!allowed.some(v => JSON.stringify(v) === JSON.stringify(args))) fail("INGRESS_GIT_COMMAND");
    const r = cp.spawnSync("git", ["--no-optional-locks", "-c", "core.fsmonitor=false", "-C", root, ...args], { input, windowsHide: true, maxBuffer: 128 * 1024 * 1024 });
    if (r.error || r.status !== 0) fail("INGRESS_GIT_READ");
    return r.stdout;
  };
}
function entries(bytes) {
  const rows = audit.nul(decode(bytes)).map(row => {
    const tab = row.indexOf("\t");
    if (tab < 0) fail("INGRESS_ENTRY_FORMAT");
    const fields = row.slice(0, tab).split(" "), p = row.slice(tab + 1);
    audit.relative(p);
    if (audit.relative(p) !== p || fields.length !== 3 || !oid(fields[1])) fail("INGRESS_ENTRY_FORMAT");
    if (fields[2] !== "0") fail("INGRESS_UNMERGED_INDEX");
    if (!["100644", "100755"].includes(fields[0])) fail("INGRESS_UNSUPPORTED_ENTRY");
    return { path: p, oid: fields[1], mode: fields[0] };
  });
  if (audit.caseCollisions(rows.map(r => r.path)).length || new Set(rows.map(r => r.path)).size !== rows.length) fail("INGRESS_CASE_COLLISION");
  return rows;
}
function parseEntries(bytes, kind) {
  if (kind === "index") return entries(bytes);
  // Normalize the tree header to the shared parser while retaining raw paths, including tabs/newlines.
  const rows = audit.nul(decode(bytes)).map(row => {
    const tab = row.indexOf("\t"), fields = row.slice(0, tab).split(" ");
    if (tab < 0 || fields.length !== 3 || fields[1] !== "blob" || !oid(fields[2]) || !["100644", "100755"].includes(fields[0])) fail("INGRESS_UNSUPPORTED_ENTRY");
    const p = row.slice(tab + 1); if (audit.relative(p) !== p) fail("INGRESS_ENTRY_FORMAT");
    return { path: p, oid: fields[2], mode: fields[0] };
  });
  if (audit.caseCollisions(rows.map(r => r.path)).length || new Set(rows.map(r => r.path)).size !== rows.length) fail("INGRESS_CASE_COLLISION");
  return rows;
}
function batch(bytes, ids) {
  const result = new Map(); let offset = 0;
  for (const id of ids) {
    const end = bytes.indexOf(10, offset); if (end < 0) fail("INGRESS_BLOB_TRUNCATED");
    const header = bytes.subarray(offset, end).toString("ascii").split(" "), size = Number(header[2]);
    if (header[0] !== id || header[1] !== "blob" || !Number.isSafeInteger(size) || size < 0 || end + size + 1 >= bytes.length || bytes[end + size + 1] !== 10) fail("INGRESS_BLOB_FORMAT");
    result.set(id, bytes.subarray(end + 1, end + 1 + size)); offset = end + size + 2;
  }
  if (offset !== bytes.length) fail("INGRESS_BLOB_TRAILING_DATA");
  return result;
}
function catalog(value, records) {
  if (!value || value.schemaVersion !== 1 || !Array.isArray(value.origins) || !Array.isArray(value.evidence) || !value.evidence.length || !Array.isArray(value.preserveExceptions)) fail("INGRESS_CATALOG_SCHEMA");
  const seen = new Set();
  for (const origin of value.origins) {
    if (!origin || !/^[a-z][a-z0-9_-]{2,63}$/.test(origin.asset_id) || typeof origin.path !== "string" || audit.relative(origin.path) !== origin.path || !/^[a-f0-9]{64}$/.test(origin.sha256) || !["original", "modified"].includes(origin.kind) || (origin.guid !== null && !/^[a-f0-9]{32}$/.test(origin.guid))) fail("INGRESS_ORIGIN_SCHEMA");
    if (origin.textSha256 !== undefined && origin.textSha256 !== null && !/^[a-f0-9]{64}$/.test(origin.textSha256)) fail("INGRESS_ORIGIN_SCHEMA");
    const key = origin.path.toLowerCase(); if (seen.has(key)) fail("INGRESS_ORIGIN_DUPLICATE"); seen.add(key);
    if (!["technical-tmp-baseline", "technical-cloudcode-baseline"].includes(origin.asset_id) && !records.some(r => r.id === origin.asset_id)) fail("INGRESS_ORIGIN_RECORD_LINK");
  }
  for (const record of records) for (const boundary of record.boundaries.filter(b => b.kind === "original"))
    if (!value.origins.some(o => o.asset_id === record.id && match(o.path, boundary.path))) fail("INGRESS_ORIGIN_COVERAGE");
  const exceptions = new Map();
  for (const item of value.preserveExceptions) {
    if (!item || typeof item.path !== "string" || audit.relative(item.path) !== item.path || exceptions.has(item.path.toLowerCase())) fail("INGRESS_EXCEPTION_SCHEMA");
    const record = records.find(r => r.id === item.asset_id), boundary = record && record.boundaries.find(b => b.path === item.path);
    const known = v => typeof v === "string" && v.trim() && !/^(?:미확인|해당 없음):/.test(v);
    if (!boundary || !["modified", "derived", "project", "settings"].includes(boundary.kind) || boundary.action !== "preserve" || record.status.rights.state !== "근거 확인" || !["확정", "적용"].includes(record.status.boundary.state) ||
        ["git_boundary", "modifications", "meta_import", "license", "evidence", "owner_review"].some(f => !known(record.fields[f])) || !known(boundary.evidence) || !known(boundary.meta.evidence)) fail("INGRESS_EXCEPTION_EVIDENCE");
    exceptions.set(item.path.toLowerCase(), item);
  }
  return exceptions;
}
function evaluate(rows, blobs, records, policy, evidence, exceptions) {
  const issues = [], preserved = [], guids = new Map(), omitted = [];
  for (const row of rows) {
    const matches = records.flatMap(r => r.boundaries.filter(b => match(row.path, b.path)).map(b => ({ record: r, boundary: b })));
    const exact = matches.filter(v => v.boundary.path.toLowerCase() === row.path.toLowerCase());
    const boundaries = exact.length ? exact : matches;
    if (boundaries.length > 1) fail("INGRESS_AMBIGUOUS_BOUNDARY");
    const boundary = boundaries.length ? boundaries[0].boundary : null;
    const exempt = exceptions.has(row.path.toLowerCase());
    if (boundary && boundary.kind === "original") issues.push({ code: "REGISTERED_ORIGINAL_TRACKED", path: row.path });
    else if (boundary && ["unknown", "mixed"].includes(boundary.kind)) issues.push({ code: "BOUNDARY_PENDING_TRACKED", path: row.path });
    else if (policy.unregisteredRoots.some(root => match(row.path, root)) && !boundary) issues.push({ code: "UNREGISTERED_THIRDPARTY_TRACKED", path: row.path });
    else if (boundary && (["modified", "derived", "settings"].includes(boundary.kind) || policy.unregisteredRoots.some(root => match(row.path, root))) && !exempt) issues.push({ code: "PRESERVATION_REVIEW_REQUIRED", path: row.path });
    if (sensitive(row.path)) { omitted.push(row.path); continue; }
    const bytes = blobs.get(row.oid); if (!bytes) fail("INGRESS_BLOB_MISSING");
    const digest = sha(bytes), normalized = textHash(bytes), hashMatches = evidence.origins.filter(o => o.sha256 === digest || (o.textSha256 && o.textSha256 === normalized));
    if (hashMatches.some(o => o.kind === "original")) issues.push({ code: "KNOWN_ORIGINAL_BYTES", path: row.path });
    else if (hashMatches.length && !exempt) issues.push({ code: "KNOWN_UNCLEARED_MODIFIED_BYTES", path: row.path });
    if (/\.meta$/i.test(row.path)) {
      const matches = [...decode(bytes).matchAll(/^guid:\s*([a-fA-F0-9]{32})\s*$/gm)];
      if (matches.length !== 1 || /^0+$/.test(matches[0][1])) fail("INGRESS_META_GUID_INVALID");
      const guid = matches[0][1].toLowerCase();
      if (guids.has(guid)) fail("INGRESS_DUPLICATE_GUID"); guids.set(guid, row.path);
      if (evidence.origins.some(o => o.guid === guid) && !exempt) issues.push({ code: "KNOWN_ORIGIN_GUID", path: row.path });
    }
    if (exempt) preserved.push({ path: row.path, code: "EXPLICIT_PROJECT_PRESERVATION" });
  }
  return { blocked: issues.length > 0, issues, preserved, sensitiveBodiesOmitted: omitted, entries: rows.length };
}
function check(root, mode, evidencePath, adapters = {}) {
  const disk = audit.reader(root), git = adapters.git || gitReader(root), smallGit = audit.gitReader(root);
  const initialIndex = sha(disk.read(".git/index"));
  const inputPaths = ["AI/00_Project/ASSET_DEPENDENCIES.md", "Tools/DependencyAudit/dependency-policy.json", audit.relative(evidencePath)];
  const inputs = inputPaths.map(p => disk.read(p));
  const records = registry.parseRegistry(decode(inputs[0])), policy = audit.parseJson(decode(inputs[1])), evidence = audit.parseJson(decode(inputs[2]));
  registry.validatePolicy(records, policy); const exceptions = catalog(evidence, records);
  for (const source of evidence.evidence) {
    if (!source || audit.relative(source.path) !== source.path || !/^[a-f0-9]{64}$/.test(source.sha256) || sha(disk.read(source.path)) !== source.sha256) fail("INGRESS_EVIDENCE_CHANGED");
  }
  const head = decode(git(["rev-parse", "HEAD"])).trim(), headRaw = git(["ls-tree", "-r", "-z", "HEAD"]), indexRaw = git(["ls-files", "--stage", "-z"]);
  const headRows = parseEntries(headRaw, "head"), indexRows = parseEntries(indexRaw, "index");
  const changes = audit.stagedChanges(decode(git(["diff", "--no-ext-diff", "--no-textconv", "--cached", "--find-renames", "--find-copies", "--find-copies-harder", "--name-status", "-z"])));
  const ids = [...new Set([...headRows, ...indexRows].filter(r => !sensitive(r.path)).map(r => r.oid))];
  const blobs = ids.length ? batch(git(["cat-file", "--batch"], ids.join("\n") + "\n"), ids) : new Map();
  const paths = [...new Set([...headRows, ...indexRows].map(r => r.path))];
  const ignores = paths.length ? audit.ignoreResult(smallGit(["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"], paths.join("\0") + "\0"), paths) : [];
  const headResult = evaluate(headRows, blobs, records, policy, evidence, exceptions), indexResult = evaluate(indexRows, blobs, records, policy, evidence, exceptions);
  if (headRaw.compare(git(["ls-tree", "-r", "-z", "HEAD"])) || indexRaw.compare(git(["ls-files", "--stage", "-z"])) || head !== decode(git(["rev-parse", "HEAD"])).trim() || sha(disk.read(".git/index")) !== initialIndex || inputPaths.some((p, i) => sha(disk.read(p)) !== sha(inputs[i]))) fail("INGRESS_INPUT_CHANGED");
  if (paths.length && JSON.stringify(ignores) !== JSON.stringify(audit.ignoreResult(smallGit(["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"], paths.join("\0") + "\0"), paths))) fail("INGRESS_IGNORE_CHANGED");
  for (const source of evidence.evidence) if (sha(disk.read(source.path)) !== source.sha256) fail("INGRESS_EVIDENCE_CHANGED");
  const blocked = indexResult.blocked || (mode === "--check-managed" && headResult.blocked);
  return { exitCode: blocked ? 1 : 0, report: { schemaVersion: 1, gate: "git-original-ingress", mode, complete: true, passed: !blocked, head, indexUnchanged: true,
    registeredRecords: records.length, knownOrigins: evidence.origins.length, headResult, indexResult, stagedChanges: changes,
    trackedIgnore: paths.map((p, i) => ({ path: p, headTracked: headRows.some(r => r.path === p), indexTracked: indexRows.some(r => r.path === p), ...ignores[i] })),
    inputs: inputPaths.map((p, i) => ({ path: p, sha256: sha(inputs[i]) })), referenceHealthPass: false, rightsGranted: false, independentRestore: false,
    limits: ["Current worktree ignore rules are reported; HEAD/index ignore rules are not emulated. Ignore never authorizes original tracking.", "Opaque bytes/GUID match only known evidence; unknown origin outside declared boundaries cannot be fully identified.", "Registry/policy/catalog are explicit worktree inputs, not a commit hook or a guarantee about different staged policy versions.", "Sensitive bodies, LFS-expanded content, Unity types/fileID and runtime references are not inspected."] } };
}
function cli(args) {
  try {
    const mode = args[0]; if (!["--check-managed", "--check-staged"].includes(mode)) fail("CLI_ARGUMENT");
    let root = path.resolve(__dirname, "../.."), evidencePath;
    for (let i = 1; i < args.length; i++) {
      if (args[i] === "--root" && args[i + 1]) root = path.resolve(args[++i]);
      else if (args[i] === "--evidence" && args[i + 1]) evidencePath = audit.relative(args[++i]);
      else fail("CLI_ARGUMENT");
    }
    if (!evidencePath) fail("INGRESS_EVIDENCE_REQUIRED");
    return check(root, mode, evidencePath);
  } catch (error) { return { exitCode: 2, report: { schemaVersion: 1, gate: "git-original-ingress", complete: false, passed: false, referenceHealthPass: false, rightsGranted: false, independentRestore: false, diagnostics: [{ code: error instanceof audit.AuditError ? error.code : "INGRESS_READ_FAILED" }] } }; }
}
if (require.main === module) { const result = cli(process.argv.slice(2)); process.stdout.write(JSON.stringify(result.report, null, 2) + "\n"); process.exitCode = result.exitCode; }
module.exports = { gitReader, parseEntries, batch, catalog, evaluate, check, cli, textHash };
