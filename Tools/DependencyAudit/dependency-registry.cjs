"use strict";
// Pure registry/policy validation plus a read-only CLI. Never imports Unity or writes files.
const path = require("node:path");
const crypto = require("node:crypto");
const audit = require("./dependency-audit.cjs");
const registryPath = "AI/00_Project/ASSET_DEPENDENCIES.md";
const policyPath = "Tools/DependencyAudit/dependency-policy.json";
const axes = {
  adoption: ["후보", "보류", "도입", "폐기"],
  rights: ["미확인", "근거 확인", "제한·차단"],
  boundary: ["초안", "판정 대기", "확정", "적용"],
  restore: ["미검증", "정적 확인", "독립 복원 확인"]
};
const kinds = ["original", "modified", "derived", "project", "mixed", "unknown", "settings"];
const actions = ["preserve", "pending", "exclude-candidate"];
const requiresConfirmedEvidence = (axis, state) => ({ adoption: ["도입", "폐기"], rights: ["근거 확인"], boundary: ["확정", "적용"], restore: ["정적 확인", "독립 복원 확인"] })[axis].includes(state);
const object = v => v !== null && typeof v === "object" && !Array.isArray(v);
const fail = code => { throw new audit.AuditError(code); };
function exact(value, keys) {
  if (!object(value) || Object.keys(value).sort().join("|") !== [...keys].sort().join("|")) fail("REGISTRY_SCHEMA");
}
function text(value) {
  if (typeof value !== "string" || !value.trim() || /[\x00-\x1f\x7f]|<br\s*\/?\s*>|\\[nr]/i.test(value)) fail("REGISTRY_VALUE");
  if (/^(?:미확인|해당 없음)\s*(?::\s*)?$/.test(value.trim())) fail("REGISTRY_REASON_REQUIRED");
  if (/^(?:미확인|해당 없음)/.test(value) && !/^(?:미확인|해당 없음):\s*\S.+/.test(value)) fail("REGISTRY_REASON_REQUIRED");
  // Reject private links rather than echoing them in diagnostics or reports.
  for (const match of value.matchAll(/(?:[a-z][a-z0-9+.-]*:\/\/|mailto:)[^\s<>"`|]+/gi)) {
    let url;
    try { url = new URL(match[0].replace(/[)\],;]+$/, "")); } catch (_) { fail("REGISTRY_PRIVATE_URL"); }
    if (!/^https?:$/.test(url.protocol) || url.username || url.password || url.search || url.hash ||
        /(?:^|[/.@_-])(?:account|accounts|login|signin|credential|token|secret|receipt)(?:[/.@_-]|$)/i.test(url.hostname + url.pathname)) fail("REGISTRY_PRIVATE_URL");
  }
  return value.trim();
}
const known = value => !/^(?:미확인|해당 없음):/.test(value);
function scoped(value) {
  text(value);
  const normalized = audit.relative(value);
  if (value !== normalized || /[\x00-\x1f\x7f]|[*?#[\]]/.test(value) || !value.startsWith("Assets/")) fail("REGISTRY_PATH");
  return normalized;
}
function cells(line) {
  if (!line.startsWith("|") || !line.endsWith("|")) fail("REGISTRY_TABLE");
  const out = []; let part = "";
  for (let i = 1; i < line.length - 1; i++) {
    if (line[i] === "\\") {
      const next = line[++i];
      if (next !== "|" && next !== "\\") fail("REGISTRY_ESCAPE");
      part += next;
    } else if (line[i] === "|") { out.push(part.trim()); part = ""; }
    else part += line[i];
  }
  out.push(part.trim());
  return out;
}
function table(section, width, strict = true) {
  if (strict && section.split("\n").some(line => line.trim() && !line.startsWith("|"))) fail("REGISTRY_TABLE_CONTINUATION");
  const lines = section.split("\n").filter(line => line.startsWith("|"));
  if (lines.length < 2) fail("REGISTRY_TABLE");
  const rows = lines.map(cells);
  if (rows.some(row => row.length !== width) || !rows[1].every(v => /^:?-{3,}:?$/.test(v))) fail("REGISTRY_TABLE");
  return rows.slice(2);
}
function jsonCell(value) {
  if (!value.startsWith("`") || !value.endsWith("`")) fail("REGISTRY_JSON_CELL");
  return uniqueJson(value.slice(1, -1));
}
function uniqueJson(source) {
  const parsed = audit.parseJson(source), stack = [];
  for (const token of source.matchAll(/"(?:\\.|[^"\\])*"|[{}\[\]]/g)) {
    const value = token[0];
    if (value === "{") stack.push(new Set());
    else if (value === "[") stack.push(null);
    else if (value === "}" || value === "]") stack.pop();
    else if (/^\s*:/.test(source.slice(token.index + value.length))) {
      const key = JSON.parse(value), keys = stack[stack.length - 1];
      if (keys.has(key)) fail("REGISTRY_JSON_DUPLICATE_KEY");
      keys.add(key);
    }
  }
  return parsed;
}
function states(value) {
  const result = jsonCell(value);
  exact(result, [...Object.keys(axes), "decision"]);
  for (const [axis, allowed] of Object.entries(axes)) {
    exact(result[axis], ["state", "evidence"]);
    if (!allowed.includes(result[axis].state)) fail("REGISTRY_STATE");
    text(result[axis].evidence);
    if (requiresConfirmedEvidence(axis, result[axis].state) && !known(result[axis].evidence)) fail("REGISTRY_STATE_EVIDENCE");
  }
  exact(result.decision, ["state", "evidence"]);
  if (!["pending", "exclude-candidate"].includes(result.decision.state)) fail("REGISTRY_DECISION");
  text(result.decision.evidence);
  if (result.decision.state === "exclude-candidate" && !known(result.decision.evidence)) fail("REGISTRY_STATE_EVIDENCE");
  return result;
}
function parseRegistry(input) {
  if (typeof input !== "string" || input.includes("\uFFFD") || input.includes("\0")) fail("REGISTRY_ENCODING");
  const source = input.replace(/^\uFEFF/, "").replace(/\r\n/g, "\n");
  if (source.includes("\r")) fail("REGISTRY_ENCODING");
  const split = source.split(/^## 항목\s*$/m);
  if (split.length !== 2) fail("REGISTRY_ITEMS_SECTION");
  const listing = split[0].split(/^## 등록 목록\s*$/m);
  if (listing.length !== 2) fail("REGISTRY_INDEX");
  const index = table(listing[1], 7, false);
  const blocks = split[1].split(/^## /m), records = [], ids = new Set(), paths = [];
  if (/^\||^#{1,}/m.test(blocks[0])) fail("REGISTRY_ITEM_HEADING");
  for (const block of blocks.slice(1)) {
    const newline = block.indexOf("\n"), id = block.slice(0, newline).trim();
    if (!/^[a-z][a-z0-9_-]{2,63}$/.test(id) || ids.has(id)) fail("REGISTRY_ID");
    ids.add(id);
    const parts = block.slice(newline + 1).split(/^### /m);
    if (parts.length !== 3 || !parts[1].startsWith("경로별 경계 표\n") || !parts[2].startsWith("변경/검증 이력\n")) fail("REGISTRY_SECTIONS");
    const fields = {};
    for (const row of table(parts[0], 2)) {
      const [field, value] = row;
      if (!audit.requiredFields.includes(field) || Object.hasOwn(fields, field)) fail("REGISTRY_FIELD");
      fields[field] = text(value);
    }
    if (audit.requiredFields.some(f => !Object.hasOwn(fields, f))) fail("REGISTRY_FIELD_MISSING");
    if (fields.asset_id !== id) fail("REGISTRY_ID_MISMATCH");
    if (!["Third Party 애셋", "UPM·기술 의존성", "프로젝트 소유 연결 자료"].includes(fields.record_kind)) fail("REGISTRY_KIND");
    const status = states(fields.status), boundaries = [];
    for (const row of table(parts[1].slice("경로별 경계 표\n".length), 7)) {
      row.forEach(text);
      const p = scoped(row[0]), role = jsonCell(row[1]), meta = jsonCell(row[4]);
      exact(role, ["kind", "evidence"]); exact(meta, ["asset", "folder", "evidence"]);
      if (!kinds.includes(role.kind) || !actions.includes(row[3]) || !["preserve", "reproduce"].includes(meta.asset) || !["preserve", "reproduce"].includes(meta.folder)) fail("REGISTRY_BOUNDARY");
      if (/^Assets\/Settings(?:\/|$)/i.test(p) && role.kind !== "settings" && role.kind !== "project") fail("REGISTRY_SETTINGS_ROLE");
      text(role.evidence); text(meta.evidence);
      for (const prior of paths) if (p.toLowerCase() === prior.toLowerCase() || p.toLowerCase().startsWith(prior.toLowerCase() + "/") || prior.toLowerCase().startsWith(p.toLowerCase() + "/")) fail("REGISTRY_PATH_OVERLAP");
      paths.push(p);
      boundaries.push({ path: p, kind: role.kind, evidence: role.evidence, action: row[3], meta, conditions: row[5], usage: row[6] });
    }
    if (!boundaries.length) fail("REGISTRY_BOUNDARY_MISSING");
    const imports = jsonCell(fields.import_path);
    if (!Array.isArray(imports) || imports.some(p => !boundaries.some(b => b.path === p)) || new Set(imports).size !== imports.length || imports.length !== boundaries.length) fail("REGISTRY_IMPORT_PATH_LINK");
    for (const row of table(parts[2].slice("변경/검증 이력\n".length), 6)) row.forEach(text);
    if (!/sha256:[a-f0-9]{64}\b/.test(fields.evidence) && known(fields.evidence)) fail("REGISTRY_HASH");
    if (status.rights.state === "근거 확인" && ["license", "usage_conditions", "official_source"].some(f => !known(fields[f]))) fail("REGISTRY_RIGHTS_EVIDENCE");
    if (["확정", "적용"].includes(status.boundary.state) && (["official_source", "meta_import", "git_boundary"].some(f => !known(fields[f])) || boundaries.some(v => ["unknown", "mixed"].includes(v.kind) || !known(v.evidence) || !known(v.meta.evidence)))) fail("REGISTRY_BOUNDARY_UNRESOLVED");
    if (status.boundary.state === "적용" && boundaries.some(v => ["modified", "derived", "settings"].includes(v.kind))) fail("REGISTRY_APPLIED_CONTENT_BLOCKED");
    if (status.adoption.state === "도입" && (!known(fields.version) || !known(fields.game_usage) || status.rights.state !== "근거 확인")) fail("REGISTRY_ADOPTION_EVIDENCE");
    if (status.restore.state !== "미검증" && ["restore", "verification", "version", "evidence"].some(f => !known(fields[f]))) fail("REGISTRY_RESTORE_EVIDENCE");
    records.push({ id, fields, status, boundaries });
  }
  if (index.length !== records.length) fail("REGISTRY_INDEX_MISMATCH");
  const listed = new Set();
  for (const row of index) {
    for (const i of [0, 1, 6]) text(row[i]);
    const record = records.find(r => r.id === row[0]);
    if (!record || listed.has(record.id) || row[1] !== record.fields.product + " / " + record.fields.version || row[6] !== "[" + record.id + "](#" + record.id + ")") fail("REGISTRY_INDEX_MISMATCH");
    for (const [i, axis] of Object.keys(axes).entries()) if (row[i + 2] !== record.status[axis].state) fail("REGISTRY_INDEX_MISMATCH");
    listed.add(record.id);
  }
  return records;
}
function evaluate(record, boundary) {
  const reasons = [];
  const { fields, status } = record;
  if (boundary.kind !== "original") reasons.push("NOT_UNMODIFIED_ORIGINAL");
  if (status.rights.state !== "근거 확인") reasons.push("RIGHTS_PENDING");
  if (!["확정", "적용"].includes(status.boundary.state)) reasons.push("BOUNDARY_PENDING");
  if (status.decision.state !== "exclude-candidate") reasons.push("USER_DECISION_PENDING");
  for (const field of ["version", "official_source", "license", "usage_conditions", "evidence", "meta_import", "restore", "owner_review"])
    if (!known(fields[field])) reasons.push("EVIDENCE_PENDING_" + field.toUpperCase());
  if (!known(boundary.evidence) || !known(boundary.meta.evidence) || !known(boundary.conditions) || !known(boundary.usage)) reasons.push("PATH_EVIDENCE_PENDING");
  if (boundary.action !== "exclude-candidate") reasons.push("PRESERVATION_REQUESTED");
  return { direction: reasons.length ? "preserve-pending" : "untracked-candidate", reasons, applied: false, rightsGranted: false, independentRestore: false };
}
function validatePolicy(records, policy) {
  exact(policy, ["schemaVersion", "unregisteredRoots", "entries"]);
  if (policy.schemaVersion !== 1 || !Array.isArray(policy.entries) || JSON.stringify(policy.unregisteredRoots) !== '["Assets/ThirdParty"]') fail("POLICY_SCHEMA");
  const results = [], seen = new Set();
  for (const entry of policy.entries) {
    exact(entry, ["asset_id", "path", "kind", "action", "meta", "evidenceFields"]);
    scoped(entry.path);
    const record = records.find(r => r.id === entry.asset_id);
    const boundary = record && record.boundaries.find(v => v.path === entry.path);
    if (!boundary || seen.has(entry.path.toLowerCase())) fail("POLICY_REGISTRY_LINK");
    seen.add(entry.path.toLowerCase());
    exact(entry.meta, ["asset", "folder"]);
    if (entry.kind !== boundary.kind || entry.action !== boundary.action || entry.meta.asset !== boundary.meta.asset || entry.meta.folder !== boundary.meta.folder) fail("POLICY_BOUNDARY_CONFLICT");
    const required = ["evidence", "meta_import", "restore", "owner_review"];
    if (!Array.isArray(entry.evidenceFields) || new Set(entry.evidenceFields).size !== entry.evidenceFields.length || entry.evidenceFields.some(f => !audit.requiredFields.includes(f)) || required.some(f => !entry.evidenceFields.includes(f))) fail("POLICY_EVIDENCE_LINK");
    const result = evaluate(record, boundary);
    if (entry.action === "exclude-candidate" && result.direction !== "untracked-candidate") {
      const error = new audit.AuditError("POLICY_EXCLUSION_BLOCKED");
      error.pending = { asset_id: record.id, path: boundary.path, ...result };
      throw error;
    }
    results.push({ asset_id: record.id, path: boundary.path, ...result });
  }
  if (results.length !== records.reduce((sum, r) => sum + r.boundaries.length, 0)) fail("POLICY_COVERAGE");
  return results;
}
function validateTransition(previous, current) {
  for (const next of current) {
    const before = previous.find(r => r.id === next.id);
    if (!before) continue;
    const changedInput = ["version", "official_source", "license", "evidence", "usage_conditions", "notices", "modifications", "purpose", "import_path", "game_usage", "references", "meta_import", "git_boundary", "restore", "lifecycle"].some(f => before.fields[f] !== next.fields[f]);
    for (const axis of Object.keys(axes)) {
      const advanced = requiresConfirmedEvidence(axis, next.status[axis].state);
      if (advanced && (changedInput || before.status[axis].state !== next.status[axis].state) && next.status[axis].evidence === before.status[axis].evidence) fail("REGISTRY_TRANSITION_EVIDENCE");
    }
    if (changedInput && next.status.decision.state === "exclude-candidate" && before.status.decision.evidence === next.status.decision.evidence) fail("REGISTRY_DECISION_REVIEW");
  }
  if (previous.some(r => !current.some(n => n.id === r.id))) fail("REGISTRY_HISTORY_REMOVED");
  return { checked: true, rightsGranted: false, independentRestore: false };
}
function check(registry, policy, previous) {
  const records = parseRegistry(registry);
  if (previous !== undefined) validateTransition(parseRegistry(previous), records);
  const boundaries = validatePolicy(records, policy);
  return { schemaVersion: 1, registryValid: true, policyValid: true, recordCount: records.length, boundaries,
    transitionCompared: previous !== undefined, gitIngressChecked: false, rightsGranted: false, independentRestore: false,
    limitations: ["Metadata assertions are not license authentication or user identity verification", "Git ingress enforcement is Step 5", "Unity and independent restoration are not executed"] };
}
function cli(args) {
  try {
    let root = path.resolve(__dirname, "../.."), previousPath;
    for (let i = 0; i < args.length; i++) {
      if (args[i] === "--root" && args[i + 1]) root = path.resolve(args[++i]);
      else if (args[i] === "--previous" && args[i + 1]) previousPath = audit.relative(args[++i]);
      else if (args[i] !== "--check") fail("CLI_ARGUMENT");
    }
    const disk = audit.reader(root), sha = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
    const indexBefore = sha(disk.read(".git/index"));
    const inputs = [registryPath, policyPath, ...(previousPath ? [previousPath] : [])];
    const bytes = inputs.map(p => disk.read(p));
    const decode = value => new TextDecoder("utf-8", { fatal: true }).decode(value);
    const report = check(decode(bytes[0]), uniqueJson(decode(bytes[1])), previousPath ? decode(bytes[2]) : undefined);
    if (indexBefore !== sha(disk.read(".git/index")) || inputs.some((p, i) => sha(disk.read(p)) !== sha(bytes[i]))) fail("REGISTRY_INPUT_CHANGED");
    return { exitCode: 0, report: { ...report, indexUnchanged: true, inputs: inputs.map((p, i) => ({ path: p, sha256: sha(bytes[i]) })) } };
  } catch (error) {
    return { exitCode: 2, report: { schemaVersion: 1, registryValid: false, policyValid: false, rightsGranted: false, independentRestore: false,
      direction: "preserve-pending", pending: error instanceof audit.AuditError && error.pending ? [error.pending] : [],
      diagnostics: [{ code: error instanceof audit.AuditError ? error.code : "REGISTRY_READ_FAILED" }] } };
  }
}
if (require.main === module) {
  const result = cli(process.argv.slice(2));
  process.stdout.write(JSON.stringify(result.report, null, 2) + "\n"); process.exitCode = result.exitCode;
}
module.exports = { cells, parseRegistry, evaluate, validatePolicy, validateTransition, check, cli };
