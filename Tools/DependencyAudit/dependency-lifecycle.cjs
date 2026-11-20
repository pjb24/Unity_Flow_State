"use strict";
// Metadata preflight only. Never updates/removes/restores actual assets, declarations or registry entries.
const path = require("node:path"), crypto = require("node:crypto");
const audit = require("./dependency-audit.cjs"), registry = require("./dependency-registry.cjs");
const sha = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
const stable = value => JSON.stringify(value, (_, v) => v && typeof v === "object" && !Array.isArray(v) ? Object.fromEntries(Object.keys(v).sort().map(k => [k, v[k]])) : v);
const fail = code => { throw new audit.AuditError(code); };
const isHash = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);
const known = value => typeof value === "string" && value.trim() && !/^(?:미확인|해당 없음):/.test(value);
const reviewKeys = ["rights", "attribution", "references", "guidImport", "names", "restore"];
function safe(p) {
  if (typeof p !== "string" || audit.relative(p) !== p || /[\x00-\x1f\x7f:*?#[\]]|[. ](?:\/|$)/.test(p)) fail("LIFECYCLE_PATH");
  return p;
}
function rows(files, nullable) {
  if (!Array.isArray(files)) fail("LIFECYCLE_FILES_SCHEMA");
  const seen = new Set();
  for (const file of files) {
    safe(file.path);
    if (seen.has(file.path.toLowerCase()) || !(isHash(file.sha256) || (nullable && file.sha256 === null))) fail("LIFECYCLE_FILES_SCHEMA");
    seen.add(file.path.toLowerCase());
  }
  return new Map(files.map(f => [f.path, f]));
}
function scopeDigest(before, after, context) {
  return sha(Buffer.from(stable({ before, after, operation: context.operation, baseline: context.baseline, changes: context.changes,
    backup: context.backup, referenceReview: context.referenceReview, resourceIdentity: context.resourceIdentity,
    sceneApplication: context.sceneApplication || null, moves: context.moves || [], moveApplication: context.moveApplication || null, failureEvidence: context.failureEvidence || null,
    reviews: Object.fromEntries(reviewKeys.map(k => [k, context.reviews && context.reviews[k] ? { state: context.reviews[k].state, evidence: context.reviews[k].evidence } : null])),
    decisionEvidence: context.decision ? context.decision.evidence : null })));
}
function evaluate(before, after, context) {
  if (!before || !after || before.id !== after.id || !context || context.schemaVersion !== 1 || !["update", "relocate", "remove", "rollback"].includes(context.operation) || !Array.isArray(context.changes)) fail("LIFECYCLE_SCHEMA");
  const reasons = [], reason = code => { if (!reasons.includes(code)) reasons.push(code); };
  const baseline = rows(context.baseline, true), observed = rows(context.current, true);
  if (!baseline.size) reason("BASELINE_REQUIRED");
  for (const file of context.baseline.filter(f => f.sha256 !== null && f.path.startsWith("Assets/") && !/\.meta$/i.test(f.path)))
    if (!baseline.has(file.path + ".meta") || baseline.get(file.path + ".meta").sha256 === null) reason("ASSET_META_BASELINE_REQUIRED");
  const digest = scopeDigest(before, after, context);
  if (!context.decision || context.decision.operation !== context.operation || context.decision.digest !== digest || !known(context.decision.evidence)) reason("CURRENT_OWNER_DECISION_REQUIRED");
  for (const key of reviewKeys) {
    const item = context.reviews && context.reviews[key];
    if (!item || item.state !== "confirmed" || !known(item.evidence) || item.digest !== digest) reason("REVIEW_REQUIRED_" + key.toUpperCase());
  }
  const backup = context.backup;
  if (!backup || backup.verified !== true || !isHash(backup.manifestSha256) || !known(backup.evidence)) reason("PINNED_BACKUP_REQUIRED");
  const backupFiles = backup && Array.isArray(backup.files) ? rows(backup.files, false) : new Map();
  const moves = context.moves || [], retiredMovePaths = new Set();
  if (!Array.isArray(moves)) fail("LIFECYCLE_MOVE_SCHEMA");
  for (const move of moves) {
    safe(move.from); safe(move.to);
    if (move.from.toLowerCase() === move.to.toLowerCase() || !isHash(move.sha256) || retiredMovePaths.has(move.from)) fail("LIFECYCLE_MOVE_SCHEMA");
    const source = context.changes.find(c => c.path === move.from), target = context.changes.find(c => c.path === move.to);
    if (context.operation !== "relocate" || !source || !target || source.beforeSha256 !== move.sha256 || source.afterSha256 !== null || target.beforeSha256 !== null || target.afterSha256 !== move.sha256) reason("MOVE_PAIR_MISMATCH");
    else retiredMovePaths.add(move.from);
  }
  if (moves.length && (!context.moveApplication || !known(context.moveApplication.evidence) || !known(context.moveApplication.table))) reason("USER_MOVE_APPLICATION_TABLE_REQUIRED");
  const changes = new Map();
  for (const change of context.changes) {
    safe(change.path);
    if (changes.has(change.path.toLowerCase()) || !(isHash(change.beforeSha256) || change.beforeSha256 === null) || !(isHash(change.afterSha256) || change.afterSha256 === null)) fail("LIFECYCLE_CHANGE_SCHEMA");
    if (!/^(?:Assets\/|Packages\/|ProjectSettings\/|package(?:-lock)?\.json$)/.test(change.path) && change.path !== "AI/00_Project/ASSET_DEPENDENCIES.md") reason("SOURCE_OR_OUTSIDE_WRITE_FORBIDDEN");
    changes.set(change.path.toLowerCase(), change);
    const previous = baseline.get(change.path), current = observed.get(change.path);
    if (!previous || previous.sha256 !== change.beforeSha256 || !current) reason("CHANGE_NOT_BOUND_TO_BASELINE");
    if (change.beforeSha256 !== null && (!backupFiles.has(change.path) || backupFiles.get(change.path).sha256 !== change.beforeSha256)) reason("BACKUP_COVERAGE_MISMATCH");
    if (context.operation !== "rollback" && change.afterSha256 === null && !retiredMovePaths.has(change.path)) reason("DELETION_NOT_AUTOMATED");
    if (/\.(?:unity|prefab)$/i.test(change.path) && (!context.sceneApplication || !known(context.sceneApplication.evidence) || !known(context.sceneApplication.table))) reason("USER_SCENE_APPLICATION_TABLE_REQUIRED");
    if (current) {
      if (context.operation === "rollback") {
        if (current.sha256 !== change.beforeSha256 && current.sha256 !== change.afterSha256) reason("UNRELATED_OR_LATE_CHANGE");
      } else if (current.sha256 !== change.beforeSha256) reason("FORWARD_PARTIAL_OR_LATE_STATE");
    }
  }
  // Preserve metadata/configuration and declarations even when they were not changed by this attempt.
  for (const file of context.baseline) {
    const current = observed.get(file.path);
    if (!current) reason("OBSERVATION_INCOMPLETE");
    else if (!changes.has(file.path.toLowerCase()) && current.sha256 !== file.sha256) reason("UNRELATED_OR_LATE_CHANGE");
    if (file.sha256 !== null && (!backupFiles.has(file.path) || backupFiles.get(file.path).sha256 !== file.sha256)) reason("BACKUP_COVERAGE_MISMATCH");
  }
  if (context.current.some(f => !baseline.has(f.path))) reason("UNDECLARED_OBSERVATION_SCOPE");
  const refs = context.referenceReview;
  if (!refs || refs.complete !== true || !known(refs.evidence) || !Array.isArray(refs.incoming)) reason("REFERENCE_SCOPE_INCOMPLETE");
  if (refs) {
    for (const key of ["dynamic", "binary"]) {
      const limit = refs[key]; if (!limit || !["resolved", "not-applicable"].includes(limit.status) || !known(limit.evidence)) reason("REFERENCE_LIMIT_PENDING_" + key.toUpperCase());
    }
    if (Array.isArray(refs.incoming)) for (const edge of refs.incoming) {
      safe(edge.from); safe(edge.target);
      if (!["guid", "fileID", "resources", "shader", "asmdef"].includes(edge.kind)) fail("LIFECYCLE_REFERENCE_SCHEMA");
      if (context.operation === "remove" ? edge.detachedAfter !== true : edge.resolvedAfter !== true) reason("REFERENCE_NOT_RECONCILED");
      if (context.operation === "remove" && !changes.has(edge.from.toLowerCase())) reason("DETACHMENT_NOT_IN_CHANGE_SCOPE");
    }
  }
  const identities = context.resourceIdentity;
  if (!Array.isArray(identities)) reason("IDENTITY_REVIEW_REQUIRED");
  else for (const item of identities) {
    safe(item.path);
    const resource = item.path.match(/\/Resources\/(.+)\.[^/.]+$/), logicalName = resource && !/\.meta$/i.test(item.path) ? resource[1] : null;
    if ([item.before, item.after].some(identity => identity !== null && (!identity || identity.resourcesName !== logicalName))) reason("RESOURCE_PATH_NAME_MISMATCH");
    const expected = context.operation === "rollback" ? item.before : item.after;
    const allowedAbsent = expected === null && ((context.operation === "rollback" && baseline.has(item.path) && baseline.get(item.path).sha256 === null) || retiredMovePaths.has(item.path));
    if ((!expected && !allowedAbsent) || stable(expected) !== stable(item.planned)) reason("GUID_IMPORT_OR_NAME_MISMATCH");
    if (!baseline.has(item.path) || !known(item.evidence)) reason("IDENTITY_REVIEW_REQUIRED");
    const current = observed.get(item.path), change = changes.get(item.path.toLowerCase());
    if (/\.meta$/i.test(item.path)) {
      if (item.before && (!baseline.has(item.path) || item.before.importerSha256 !== baseline.get(item.path).sha256)) reason("IMPORT_HASH_NOT_BOUND_TO_INPUT");
      if (item.after && item.after.importerSha256 !== (change ? change.afterSha256 : baseline.has(item.path) ? baseline.get(item.path).sha256 : undefined)) reason("IMPORT_HASH_NOT_BOUND_TO_INPUT");
    }
    if (context.operation !== "rollback" && !change && stable(item.before) !== stable(item.after)) reason("IDENTITY_CHANGE_NOT_DECLARED");
    if (current && Object.hasOwn(current, "identity")) {
      const expectedObserved = baseline.has(item.path) && current.sha256 === baseline.get(item.path).sha256 ? item.before : change && current.sha256 === change.afterSha256 ? item.after : undefined;
      if (stable(current.identity) !== stable(expectedObserved)) reason("OBSERVED_GUID_IMPORT_OR_NAME_MISMATCH");
    }
  }
  if (Array.isArray(identities)) for (const file of context.baseline.filter(f => /\.meta$|\.shader$|\/Resources\//i.test(f.path)))
    if (!identities.some(v => v.path === file.path)) reason("IDENTITY_REVIEW_REQUIRED");
  if (context.operation === "update" && context.changes.some(change => before.boundaries.some(b => b.kind === "original" && (change.path === b.path || change.path === b.path + ".meta" || change.path.startsWith(b.path + "/")))) &&
      ["version", "official_source", "evidence"].every(f => before.fields[f] === after.fields[f])) reason("ORIGINAL_MUTATION_REQUIRES_SOURCE_EVIDENCE");
  if (context.operation === "rollback") {
    if (!known(context.failureEvidence)) reason("FAILURE_OR_INTERRUPTION_EVIDENCE_REQUIRED");
  } else {
    if (["relocate", "remove"].includes(context.operation) && ["version", "official_source", "evidence"].some(f => before.fields[f] !== after.fields[f])) reason("OPERATION_SOURCE_IDENTITY_CHANGED");
    try { registry.validateTransition([before], [after]); } catch (error) { reason(error instanceof audit.AuditError ? error.code : "REGISTRY_TRANSITION_FAILED"); }
    if (after.status.rights.state !== "근거 확인") reason("TARGET_RIGHTS_PENDING");
    if (context.operation === "remove" && after.status.adoption.state !== "폐기") reason("RETIRED_REGISTRY_RECORD_REQUIRED");
  }
  for (const move of moves.filter(m => /\.meta$/i.test(m.from))) {
    const source = Array.isArray(identities) && identities.find(v => v.path === move.from), target = Array.isArray(identities) && identities.find(v => v.path === move.to);
    if (!source || !target || !source.before || !target.after || source.before.guid !== target.after.guid || source.before.importerSha256 !== target.after.importerSha256) reason("MOVE_GUID_IMPORT_MISMATCH");
  }
  const rollback = context.operation === "rollback" ? context.changes.map(change => ({ path: change.path,
    action: change.beforeSha256 === null ? "retain-created-output-for-review" : observed.get(change.path) && observed.get(change.path).sha256 === change.beforeSha256 ? "already-baseline" : "restore-verified-preserved-bytes",
    expectedSha256: change.beforeSha256, userApplication: /\.(?:unity|prefab)$/i.test(change.path) })) : [];
  return { schemaVersion: 1, asset_id: before.id, operation: context.operation, scopeDigest: digest, readyForReviewedPreparation: reasons.length === 0,
    direction: reasons.length ? "preserve-pending" : "reviewed-plan", reasons, rollback,
    candidateVersion: context.operation === "rollback" ? before.fields.version : after.fields.version,
    applyOrder: ["preserve current failure snapshot and exact old inputs", "prepare exact original/version and verified changed/config/meta bytes", "owner applies explicit move/Scene/Prefab table if needed", "restore only named declaration/lock candidates", "recheck GUID/Import/name/reference/notice and registry history"],
    applied: false, filesDeleted: false, registryWritten: false, rightsGranted: false, independentRestore: false,
    requiresReverificationAfterApplication: true, limits: ["Metadata evidence/owner assertions are not authenticated permissions or actual Unity execution", "No reset/clean/cache cleanup, asset application or automatic history removal"] };
}
function observe(root, paths) {
  const disk = audit.reader(root), files = [];
  for (const p of paths) {
    safe(p);
    try {
      const bytes = disk.read(p), text = bytes.toString("utf8"); let guid = null;
      if (/\.meta$/i.test(p)) {
        const matches = [...text.matchAll(/^guid:\s*([a-fA-F0-9]{32})\s*$/gm)]; if (matches.length !== 1 || /^0+$/.test(matches[0][1])) fail("LIFECYCLE_META_GUID_INVALID"); guid = matches[0][1].toLowerCase();
        const paired = disk.stat(p.slice(0, -5));
        if (paired.isSymbolicLink() || (/^folderAsset:\s*yes\s*$/m.test(text) ? !paired.isDirectory() : !paired.isFile())) fail("LIFECYCLE_META_PAIR");
      }
      const resource = p.match(/\/Resources\/(.+)\.[^/.]+$/);
      files.push({ path: p, sha256: sha(bytes), identity: { guid, importerSha256: /\.meta$/i.test(p) ? sha(bytes) : null, resourcesName: resource && !/\.meta$/i.test(p) ? resource[1] : null, shaderNames: /\.shader$/i.test(p) ? [...text.matchAll(/^\s*Shader\s+"([^"]+)"/gm)].map(m => m[1]) : [] } });
    }
    catch (error) {
      if (error.code !== "IO_READ_FAILED") throw error;
      const parts = p.split("/"); let cursor = root, missing = false;
      for (let i = 0; i < parts.length; i++) {
        cursor = path.join(cursor, parts[i]); let stat;
        try { stat = disk.io.lstatSync(cursor); } catch (e) { if (e.code === "ENOENT") { missing = true; break; } throw error; }
        if (stat.isSymbolicLink()) fail("LIFECYCLE_LINK");
        if (i < parts.length - 1 && !stat.isDirectory()) throw error;
      }
      if (!missing) throw error; files.push({ path: p, sha256: null, identity: null });
    }
  }
  return files;
}
function cli(args) {
  try {
    let root = path.resolve(__dirname, "../.."), requestPath;
    if (args[0] !== "--check" && args[0] !== "--plan") fail("CLI_ARGUMENT");
    let i = 1; if (args[0] === "--plan") { if (!args[i]) fail("CLI_ARGUMENT"); requestPath = safe(args[i++]); }
    for (; i < args.length; i++) { if (args[i] === "--root" && args[i + 1]) root = path.resolve(args[++i]); else fail("CLI_ARGUMENT"); }
    const disk = audit.reader(root), index = sha(disk.read(".git/index")), inputs = new Map();
    const read = p => { const bytes = disk.read(safe(p)); inputs.set(p, sha(bytes)); return new TextDecoder("utf-8", { fatal: true }).decode(bytes); };
    const records = registry.parseRegistry(read("AI/00_Project/ASSET_DEPENDENCIES.md")), policy = audit.parseJson(read("Tools/DependencyAudit/dependency-policy.json")); registry.validatePolicy(records, policy);
    let report;
    if (requestPath) {
      const request = audit.parseJson(read(requestPath));
      const previous = registry.parseRegistry(read(request.previousRegistry)), candidate = registry.parseRegistry(read(request.candidateRegistry));
      if (stable(previous.filter(r => r.id !== request.asset_id)) !== stable(candidate.filter(r => r.id !== request.asset_id))) fail("LIFECYCLE_UNRELATED_RECORD_CHANGE");
      const before = previous.find(r => r.id === request.asset_id), after = candidate.find(r => r.id === request.asset_id);
      if (!before || !after) fail("LIFECYCLE_RECORD_REQUIRED");
      const context = request.context; rows(context.baseline, true);
      const live = records.find(r => r.id === request.asset_id);
      if (!live) fail("LIFECYCLE_LIVE_RECORD_REQUIRED");
      if (stable(records.filter(r => r.id !== request.asset_id)) !== stable(previous.filter(r => r.id !== request.asset_id))) fail("LIFECYCLE_UNRELATED_RECORD_CHANGE");
      if (stable(live) !== stable(before) && (context.operation !== "rollback" || stable(live) !== stable(after))) fail("LIFECYCLE_LATE_REGISTRY_STATE");
      registry.validatePolicy(candidate, request.candidatePolicy ? audit.parseJson(read(request.candidatePolicy)) : policy);
      const observed = observe(root, context.baseline.map(f => f.path));
      report = evaluate(before, after, { ...context, current: observed });
      if (stable(observed) !== stable(observe(root, context.baseline.map(f => f.path)))) fail("LIFECYCLE_INPUT_CHANGED");
    } else report = { schemaVersion: 1, registryValid: true, recordCount: records.length, actualLifecycleActionsPlanned: 0, note: "No lifecycle request selected; registry check is not an executed update/removal/rollback", applied: false, filesDeleted: false, rightsGranted: false, independentRestore: false };
    if (sha(disk.read(".git/index")) !== index || [...inputs].some(([p, digest]) => sha(disk.read(p)) !== digest)) fail("LIFECYCLE_INPUT_CHANGED");
    return { exitCode: report.readyForReviewedPreparation === false ? 1 : 0, report: { ...report, indexUnchanged: true, inputs: [...inputs].map(([p, digest]) => ({ path: p, sha256: digest })) } };
  } catch (error) { return { exitCode: 2, report: { schemaVersion: 1, readyForReviewedPreparation: false, applied: false, filesDeleted: false, rightsGranted: false, independentRestore: false, diagnostics: [{ code: error instanceof audit.AuditError ? error.code : "LIFECYCLE_READ_FAILED" }] } }; }
}
if (require.main === module) { const result = cli(process.argv.slice(2)); process.stdout.write(JSON.stringify(result.report, null, 2) + "\n"); process.exitCode = result.exitCode; }
module.exports = { scopeDigest, evaluate, observe, cli };
