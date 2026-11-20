"use strict";
// Adoption preflight only. Pure decisions and explicit read-only file inspection; no copy/apply CLI.
const crypto = require("node:crypto");
const audit = require("./dependency-audit.cjs");
const registry = require("./dependency-registry.cjs");
const fail = code => { throw new audit.AuditError(code); };
const sha = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
const guid = value => typeof value === "string" && /^[a-f0-9]{32}$/.test(value) && !/^0+$/.test(value);
const hash = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);
const inside = (p, root) => p.startsWith(root + "/");
function keys(value, names) {
  if (!value || typeof value !== "object" || Array.isArray(value) || Object.keys(value).sort().join("|") !== [...names].sort().join("|")) fail("ADOPTION_SCHEMA");
}
function safe(p) {
  if (typeof p !== "string" || audit.relative(p) !== p || /[\x00-\x1f\x7f:*?#[\]]/.test(p) || /[. ](?:\/|$)/.test(p)) fail("ADOPTION_PATH");
  return p;
}
function planSchema(plan) {
  keys(plan, ["schemaVersion", "asset_id", "version", "supplier", "product", "sourceRoot", "archivePath", "archiveSha256", "selectedSources", "mappings"]);
  if (plan.schemaVersion !== 1 || !/^[a-z][a-z0-9_-]{2,63}$/.test(plan.asset_id) || ![plan.supplier, plan.product].every(v => typeof v === "string" && /^[a-z0-9][a-z0-9_-]{0,63}$/.test(v)) || !hash(plan.archiveSha256)) fail("ADOPTION_SCHEMA");
  if (plan.sourceRoot !== "Ignore/DependencySources/" + plan.asset_id + "/" + plan.archiveSha256 || !inside(safe(plan.archivePath), plan.sourceRoot)) fail("ADOPTION_SOURCE_ROOT");
  if (!Array.isArray(plan.selectedSources) || !plan.selectedSources.length || !Array.isArray(plan.mappings) || !plan.mappings.length) fail("ADOPTION_EMPTY_SELECTION");
  const selected = new Set();
  for (const source of plan.selectedSources) {
    safe(source);
    if (!inside(source, plan.sourceRoot) || source.endsWith(".meta") || selected.has(source.toLowerCase())) fail("ADOPTION_SELECTION");
    selected.add(source.toLowerCase());
  }
  const targets = new Set();
  for (const item of plan.mappings) {
    keys(item, ["source", "target", "kind", "sourceSha256", "sourceMetaSha256", "sourceGuid", "targetGuid", "references", "usageField"]);
    safe(item.source); safe(item.target);
    if (!plan.selectedSources.includes(item.source) || !hash(item.sourceSha256) || !hash(item.sourceMetaSha256) || !guid(item.sourceGuid) || !guid(item.targetGuid) || !Array.isArray(item.references) || item.references.some(v => !guid(v))) fail("ADOPTION_MAPPING");
    if (item.target.endsWith(".meta") || targets.has(item.target.toLowerCase())) fail("ADOPTION_TARGET_COLLISION");
    for (const other of targets) if (inside(item.target.toLowerCase(), other) || inside(other, item.target.toLowerCase())) fail("ADOPTION_TARGET_COLLISION");
    targets.add(item.target.toLowerCase());
    const original = item.kind === "original";
    if (!original && !["derived", "project", "settings", "modified"].includes(item.kind)) fail("ADOPTION_KIND_PENDING");
    const targetRoot = original ? "Assets/ThirdParty/" + plan.supplier + "/" + plan.product : "Assets/ProjectAssets/" + plan.asset_id;
    if (!inside(item.target, targetRoot)) fail("ADOPTION_TARGET_ROOT");
    if (original ? item.targetGuid !== item.sourceGuid : item.targetGuid === item.sourceGuid) fail("ADOPTION_GUID_COPY");
    if (item.usageField !== "game_usage") fail("ADOPTION_USAGE_LINK");
  }
  if (plan.selectedSources.some(p => !plan.mappings.some(m => m.source === p))) fail("ADOPTION_SELECTION_MISSING");
  return plan;
}
function validate(record, input, files, existingMeta) {
  const plan = planSchema(input);
  if (!record || record.id !== plan.asset_id || record.fields.version !== plan.version) fail("ADOPTION_RECORD_VERSION");
  if (!Array.isArray(files) || !Array.isArray(existingMeta)) fail("ADOPTION_INPUT_SCHEMA");
  const known = value => typeof value === "string" && value.trim() && !/^(?:미확인|해당 없음):/.test(value);
  if (record.status.rights.state !== "근거 확인" || !["확정", "적용"].includes(record.status.boundary.state)) fail("ADOPTION_RIGHTS_BOUNDARY_PENDING");
  for (const field of ["official_source", "version", "evidence", "meta_import", "purpose", "game_usage", "references", "restore"])
    if (!known(record.fields[field])) fail("ADOPTION_RECORD_EVIDENCE");
  for (const field of ["notices", "usage_conditions"])
    if (typeof record.fields[field] !== "string" || !record.fields[field].trim() || /^미확인:/.test(record.fields[field])) fail("ADOPTION_CONDITIONS_PENDING");
  if (!record.fields.evidence.includes("sha256:" + plan.archiveSha256)) fail("ADOPTION_ARCHIVE_RECORD_HASH");
  const byPath = new Map();
  for (const file of files) {
    safe(file.path);
    if (byPath.has(file.path.toLowerCase())) fail("ADOPTION_INPUT_DUPLICATE");
    byPath.set(file.path.toLowerCase(), file);
  }
  const archive = byPath.get(plan.archivePath.toLowerCase());
  if (!archive || archive.sha256 !== plan.archiveSha256) fail("ADOPTION_ARCHIVE_HASH");
  const guidOwners = new Map();
  const existingPaths = new Set();
  for (const entry of existingMeta) {
    safe(entry.path);
    if (!entry.path.startsWith("Assets/") || !guid(entry.guid) || guidOwners.has(entry.guid) || existingPaths.has(entry.path.toLowerCase())) fail("ADOPTION_EXISTING_GUID");
    existingPaths.add(entry.path.toLowerCase());
    guidOwners.set(entry.guid, entry.path);
  }
  const proposedGuids = new Set(), mappedSources = new Map();
  const supplierGuids = new Set(files.filter(f => inside(f.path, plan.sourceRoot) && guid(f.guid)).map(f => f.guid));
  for (const item of plan.mappings) {
    const boundary = record.boundaries.find(b => b.path === item.target || inside(item.target, b.path));
    if (!boundary || boundary.kind !== item.kind) fail("ADOPTION_BOUNDARY_LINK");
    if (item.kind !== "original" && boundary.action === "exclude-candidate") fail("ADOPTION_MODIFIED_EXCLUSION");
    if (item.kind !== "original" && supplierGuids.has(item.targetGuid)) fail("ADOPTION_GUID_COPY");
    const source = byPath.get(item.source.toLowerCase());
    if (!source || source.path !== item.source || source.sha256 !== item.sourceSha256 || source.metaSha256 !== item.sourceMetaSha256 || source.guid !== item.sourceGuid) fail("ADOPTION_SOURCE_INPUT");
    if (!Array.isArray(source.references) || source.references.some(r => !item.references.includes(r))) fail("ADOPTION_REFERENCE_UNDECLARED");
    const previousSource = mappedSources.get(item.sourceGuid);
    if (previousSource && previousSource !== item.source) fail("ADOPTION_SOURCE_GUID_DUPLICATE");
    mappedSources.set(item.sourceGuid, item.source);
    if (byPath.has(item.target.toLowerCase()) || byPath.has((item.target + ".meta").toLowerCase()) || existingMeta.some(e => e.path.toLowerCase() === item.target.toLowerCase())) fail("ADOPTION_OVERWRITE");
    if (guidOwners.has(item.targetGuid) || proposedGuids.has(item.targetGuid)) fail("ADOPTION_GUID_COLLISION");
    proposedGuids.add(item.targetGuid);
    // Match planned targets against the production classifier; uncertainty cannot grant exclusion.
    if (boundary.action === "exclude-candidate" && registry.evaluate(record, boundary).direction !== "untracked-candidate") fail("ADOPTION_EXCLUSION_BLOCKED");
  }
  for (const item of plan.mappings) for (const reference of item.references)
    if (!guidOwners.has(reference) && !proposedGuids.has(reference)) fail("ADOPTION_REFERENCE_MISSING");
  return { schemaVersion: 1, preflightValid: true, mappings: plan.mappings.length, applied: false, sourceWrites: false,
    rightsGranted: false, independentRestore: false, unityRequiredBeforeAdoptionCompletion: true,
    limits: ["Declared file/GUID closure only; name/dynamic/binary and fileID/import semantics require separate review", "No copy, extraction, folder/meta creation, Scene edits or license authentication performed"] };
}
function inspect(root, input) {
  const plan = planSchema(input), disk = audit.reader(root), files = [], captured = new Map();
  const git = audit.gitReader(root), before = audit.gitSnapshot(git), indexHash = sha(disk.read(".git/index"));
  const inputs = [plan.archivePath, ...plan.selectedSources.flatMap(p => [p, p + ".meta"])];
  const ignored = audit.ignoreResult(git(["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"], inputs.join("\0") + "\0"), inputs);
  if (inputs.some((p, i) => !ignored[i].ignored || before.tracked.some(t => t.toLowerCase() === p.toLowerCase()))) fail("ADOPTION_STORAGE_NOT_PRIVATE");
  const read = p => { const bytes = disk.read(p); captured.set(p, sha(bytes)); return bytes; };
  files.push({ path: plan.archivePath, sha256: sha(read(plan.archivePath)) });
  for (const p of plan.selectedSources) {
    const body = read(p), metaBytes = read(p + ".meta"), meta = metaBytes.toString("utf8"), matches = [...meta.matchAll(/^guid:\s*([a-f0-9]{32})\s*$/gm)];
    if (matches.length !== 1 || !guid(matches[0][1])) fail("ADOPTION_SOURCE_META");
    const references = [...audit.references(body.toString("utf8"), p, new Map()),
      ...audit.references(body.toString("utf8"), p, new Map(), true), ...audit.references(meta, p + ".meta", new Map())]
      .filter(r => r.status !== "builtin" && r.status !== "null" && r.guid !== matches[0][1]).map(r => r.guid);
    if (references.some(r => !guid(r))) fail("ADOPTION_REFERENCE_GUID");
    files.push({ path: p, sha256: sha(body), metaSha256: sha(metaBytes), guid: matches[0][1], references: [...new Set(references)] });
  }
  // Enumerate Assets to catch target/meta collisions and duplicate GUIDs without Unity interpretation.
  const diagnostics = [], rows = disk.walk("Assets", diagnostics), existingMeta = [];
  if (diagnostics.length) fail("ADOPTION_PARTIAL_ENUMERATION");
  for (const row of rows) {
    files.push({ path: row.path, directory: row.directory });
    if (!row.directory && row.path.endsWith(".meta")) {
      const meta = read(row.path).toString("utf8"), matches = [...meta.matchAll(/^guid:\s*([a-f0-9]{32})\s*$/gm)];
      if (matches.length !== 1) fail("ADOPTION_EXISTING_META");
      existingMeta.push({ path: row.path.slice(0, -5), guid: matches[0][1] });
    }
  }
  for (const [p, digest] of captured) if (sha(disk.read(p)) !== digest) fail("ADOPTION_INPUT_CHANGED");
  const afterDiagnostics = [], afterRows = disk.walk("Assets", afterDiagnostics);
  if (afterDiagnostics.length || JSON.stringify(rows) !== JSON.stringify(afterRows) || sha(disk.read(".git/index")) !== indexHash || JSON.stringify(before) !== JSON.stringify(audit.gitSnapshot(git))) fail("ADOPTION_INPUT_CHANGED");
  return { files, existingMeta, indexUnchanged: true, storageIgnoredUntracked: true, sourceHashes: [...captured].map(([p, digest]) => ({ path: p, sha256: digest })) };
}
module.exports = { planSchema, validate, inspect };
