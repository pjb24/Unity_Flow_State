"use strict";
// Uses the production parser/evaluator; all populated product data is synthetic.
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path"), os = require("node:os"), cp = require("node:child_process");
const audit = require("./dependency-audit.cjs"), registry = require("./dependency-registry.cjs");
const tests = [], test = (name, run) => tests.push({ name, run });
const throws = (fn, code) => assert.throws(fn, e => e.code === code);
const cell = value => String(value).replace(/\\/g, "\\\\").replace(/\|/g, "\\|");
const json = value => "`" + JSON.stringify(value) + "`";
const policy = () => ({ schemaVersion: 1, unregisteredRoots: ["Assets/ThirdParty"], entries: [] });
function fixture(options = {}) {
  const id = options.id || "synthetic-asset", p = options.path || "Assets/ThirdParty/Synthetic/Item";
  const states = { adoption: { state: "후보", evidence: "미확인: candidate only; review before import" },
    rights: { state: "미확인", evidence: "미확인: terms pending; review before import" },
    boundary: { state: "초안", evidence: "미확인: path evidence pending; review before exclusion" },
    restore: { state: "미검증", evidence: "미확인: independent execution pending" },
    decision: { state: "pending", evidence: "미확인: user decision pending" } };
  if (options.approved) {
    states.rights = { state: "근거 확인", evidence: "synthetic user-reviewed terms task" };
    states.boundary = { state: "확정", evidence: "synthetic boundary task" };
    states.decision = { state: "exclude-candidate", evidence: "synthetic explicit user decision task" };
  }
  if (options.states) for (const key of Object.keys(options.states)) states[key] = options.states[key];
  const fields = Object.fromEntries(audit.requiredFields.map(f => [f, "synthetic evidence for " + f]));
  Object.assign(fields, { asset_id: id, record_kind: "Third Party 애셋", product: "Synthetic supplier product", version: "1.0.0",
    official_source: "https://example.invalid/distribution", evidence: "sha256:" + "a".repeat(64) + " synthetic task",
    import_path: json([p]), status: json(states), ...(options.fields || {}) });
  const kind = options.kind || "original", action = options.action || "preserve";
  const meta = { asset: "preserve", folder: "preserve", evidence: "synthetic meta import reproduction task" };
  const row = [p, json({ kind, evidence: "synthetic source comparison task" }), "untracked / ignored", action, json(meta), "synthetic terms", "synthetic usage and restoration task"];
  const record = "## " + id + "\n\n| 필드 | 값 |\n| --- | --- |\n" + Object.entries(fields).map(([k, v]) => "| " + k + " | " + cell(v) + " |").join("\n") +
    "\n\n### 경로별 경계 표\n\n| 경로/구성요소 | 역할 | 추적 | 방향 | meta | 조건 | 후속 |\n| --- | --- | --- | --- | --- | --- | --- |\n| " + row.map(cell).join(" | ") + " |\n\n### 변경/검증 이력\n\n| 날짜 | 버전/경로 | 변경 | 근거 | 검증 | 의무 |\n| --- | --- | --- | --- | --- | --- |\n";
  const indexRow = [id, fields.product + " / " + fields.version, ...["adoption", "rights", "boundary", "restore"].map(a => states[a].state), "[" + id + "](#" + id + ")"];
  const document = "# Synthetic registry\n\n## 등록 목록\n\n| asset_id | 제품 | 도입 | 조건 | 경계 | 복원 | 링크 |\n| --- | --- | --- | --- | --- | --- | --- |\n| " + indexRow.map(cell).join(" | ") + " |\n\n## 항목\n\n" + record;
  const entry = { asset_id: id, path: p, kind, action, meta: { asset: meta.asset, folder: meta.folder }, evidenceFields: ["evidence", "meta_import", "restore", "owner_review"] };
  return { document, policy: { ...policy(), entries: [entry] }, fields, states, record, indexRow };
}
test("actual empty registry and policy validate without fabricated adoption", () => {
  const result = registry.cli(["--check"]); assert.equal(result.exitCode, 0); assert.equal(result.report.recordCount, 0);
  assert.equal(result.report.gitIngressChecked, false); assert.equal(result.report.rightsGranted, false);
});
test("candidate with pending rights stays preserved", () => {
  const f = fixture(); const result = registry.check(f.document, f.policy);
  assert.equal(result.boundaries[0].direction, "preserve-pending"); assert(result.boundaries[0].reasons.includes("USER_DECISION_PENDING"));
});
test("approved original becomes candidate only; independent states are not promoted", () => {
  const f = fixture({ approved: true, action: "exclude-candidate" }); const result = registry.check(f.document, f.policy);
  assert.equal(result.boundaries[0].direction, "untracked-candidate");
  for (const flag of ["applied", "rightsGranted", "independentRestore"]) assert.equal(result.boundaries[0][flag], false);
  const r = registry.parseRegistry(f.document)[0]; assert.equal(r.status.adoption.state, "후보"); assert.equal(r.status.restore.state, "미검증");
});
test("explicit decision before and after is checked through same evaluator", () => {
  const f = fixture({ approved: true, action: "exclude-candidate", states: { decision: { state: "pending", evidence: "미확인: user decision required" } } });
  const r = registry.parseRegistry(f.document)[0]; assert.equal(registry.evaluate(r, r.boundaries[0]).direction, "preserve-pending");
  throws(() => registry.check(f.document, f.policy), "POLICY_EXCLUSION_BLOCKED");
});
for (const kind of ["modified", "derived", "project", "mixed", "unknown", "settings"]) test("cannot exclude " + kind, () => {
  const f = fixture({ kind, approved: true, action: "exclude-candidate" });
  throws(() => registry.check(f.document, f.policy), ["mixed", "unknown"].includes(kind) ? "REGISTRY_BOUNDARY_UNRESOLVED" : "POLICY_EXCLUSION_BLOCKED");
});
test("unknown content with preserve remains valid pending", () => { const f = fixture({ kind: "unknown" }); assert.equal(registry.check(f.document, f.policy).boundaries[0].direction, "preserve-pending"); });
test("modified content cannot claim applied state", () => { const f = fixture({ kind: "modified", approved: true, states: { boundary: { state: "적용", evidence: "synthetic task" } } }); throws(() => registry.check(f.document, f.policy), "REGISTRY_APPLIED_CONTENT_BLOCKED"); });
test("missing required fields rejected", () => { const f = fixture(); throws(() => registry.parseRegistry(f.document.replace(/^\| version .*\n/m, "")), "REGISTRY_FIELD_MISSING"); });
test("duplicate fields rejected", () => { const f = fixture(); throws(() => registry.parseRegistry(f.document.replace("| version |", "| product |")), "REGISTRY_FIELD"); });
test("duplicate item IDs rejected", () => { const f = fixture(); throws(() => registry.parseRegistry(f.document + "\n" + f.record), "REGISTRY_ID"); });
test("heading/field ID mismatch rejected", () => { const f = fixture({ fields: { asset_id: "different-id" } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_ID_MISMATCH"); });
test("duplicate and overlapping paths are rejected across products", () => {
  const f = fixture(), g = fixture({ id: "second-asset", path: "Assets/ThirdParty/Synthetic/Item/Child" });
  const document = f.document.replace("\n\n## 항목", "\n| " + g.indexRow.map(cell).join(" | ") + " |\n\n## 항목") + "\n" + g.record;
  throws(() => registry.parseRegistry(document), "REGISTRY_PATH_OVERLAP");
});
for (const p of ["../outside", "C:/outside", "Assets/../outside", "Assets/Item\nOther", "Assets/item*"]) test("reject unsafe path " + JSON.stringify(p), () => {
  const f = fixture({ path: p }); assert.throws(() => registry.parseRegistry(f.document));
});
test("pipe/backslash escaping round trips and unescaped pipes fail", () => {
  const f = fixture({ fields: { purpose: "synthetic A | B \\ C" } }); assert.equal(registry.parseRegistry(f.document)[0].fields.purpose, "synthetic A | B \\ C");
  throws(() => registry.parseRegistry(f.document.replace("A \\| B", "A | B")), "REGISTRY_TABLE");
});
test("unknown escape and literal continuation fail", () => {
  const f = fixture(); throws(() => registry.parseRegistry(f.document.replace("synthetic evidence for purpose", "bad\\q")), "REGISTRY_ESCAPE");
  throws(() => registry.parseRegistry(f.document.replace("| purpose |", "continuation\n| purpose |")), "REGISTRY_TABLE_CONTINUATION");
});
test("bare unknown / not applicable require reasons", () => { for (const value of ["미확인", "해당 없음:"]) { const f = fixture({ fields: { acquisition: value } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_REASON_REQUIRED"); } });
test("unknown values with reasons remain valid", () => { const f = fixture({ fields: { acquisition: "미확인: network access needed; review at adoption" } }); assert.equal(registry.parseRegistry(f.document).length, 1); });
test("on-hold and pending-boundary states accept explicit unknown reasons", () => {
  const f = fixture({ states: { adoption: { state: "보류", evidence: "미확인: acquisition pending; next review before import" }, boundary: { state: "판정 대기", evidence: "미확인: ownership pending; next review before exclusion" } } });
  assert.equal(registry.check(f.document, f.policy).boundaries[0].direction, "preserve-pending");
});
test("bad state and asserted state without evidence fail", () => {
  const f = fixture({ states: { rights: { state: "PASS", evidence: "synthetic task" } } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_STATE");
  const g = fixture({ states: { rights: { state: "근거 확인", evidence: "미확인: no verification" } } }); throws(() => registry.parseRegistry(g.document), "REGISTRY_STATE_EVIDENCE");
});
for (const url of ["https://user:password@example.invalid/file", "https://example.invalid/file?token=hidden", "https://example.invalid/accounts/person", "ssh://example.invalid/file", "mailto:user@example.invalid"]) test("private URL rejected without value echo", () => {
  const f = fixture({ fields: { official_source: url } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_PRIVATE_URL");
});
test("rights confirmation cannot override unknown terms", () => { const f = fixture({ approved: true, fields: { license: "미확인: terms pending" } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_RIGHTS_EVIDENCE"); });
test("adoption and restore claims require their own inputs", () => {
  const f = fixture({ states: { adoption: { state: "도입", evidence: "synthetic import" } } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_ADOPTION_EVIDENCE");
  const g = fixture({ states: { restore: { state: "정적 확인", evidence: "synthetic static" } }, fields: { verification: "미확인: no execution" } }); throws(() => registry.parseRegistry(g.document), "REGISTRY_RESTORE_EVIDENCE");
});
test("invalid hash and disconnected import path fail", () => {
  const f = fixture({ fields: { evidence: "sha256:bad" } }); throws(() => registry.parseRegistry(f.document), "REGISTRY_HASH");
  const g = fixture({ fields: { import_path: json(["Assets/Different"]) } }); throws(() => registry.parseRegistry(g.document), "REGISTRY_IMPORT_PATH_LINK");
});
test("summary status/link mismatch fails", () => { const f = fixture(); throws(() => registry.parseRegistry(f.document.replace("[synthetic-asset](#synthetic-asset)", "[synthetic-asset](#missing)")), "REGISTRY_INDEX_MISMATCH"); });
test("policy schema cannot duplicate product or rights truth", () => { const f = fixture(); f.policy.entries[0].license = "extra truth"; throws(() => registry.check(f.document, f.policy), "REGISTRY_SCHEMA"); });
test("policy identity/path/meta/action disagreement fails", () => {
  for (const [key, value] of [["asset_id", "unknown-id"], ["kind", "project"], ["action", "pending"]]) { const f = fixture(); f.policy.entries[0][key] = value; throws(() => registry.check(f.document, f.policy), key === "asset_id" ? "POLICY_REGISTRY_LINK" : "POLICY_BOUNDARY_CONFLICT"); }
  const f = fixture(); f.policy.entries[0].meta.folder = "reproduce"; throws(() => registry.check(f.document, f.policy), "POLICY_BOUNDARY_CONFLICT");
});
test("policy duplicate/omitted/orphan/evidence links fail", () => {
  const f = fixture(); throws(() => registry.check(f.document, policy()), "POLICY_COVERAGE");
  f.policy.entries.push(f.policy.entries[0]); throws(() => registry.check(f.document, f.policy), "POLICY_REGISTRY_LINK");
  const g = fixture(); g.policy.entries[0].evidenceFields = ["license"]; throws(() => registry.check(g.document, g.policy), "POLICY_EVIDENCE_LINK");
});
test("policy revision never changes registry states", () => { const f = fixture(); const before = JSON.stringify(registry.parseRegistry(f.document)); f.policy.entries[0].evidenceFields.push("license"); registry.check(f.document, f.policy); assert.equal(JSON.stringify(registry.parseRegistry(f.document)), before); });
test("changed version/source requires fresh evidence and decision", () => {
  const old = fixture({ approved: true, action: "exclude-candidate" }), next = fixture({ approved: true, action: "exclude-candidate", fields: { version: "2.0.0" } });
  throws(() => registry.check(next.document, next.policy, old.document), "REGISTRY_TRANSITION_EVIDENCE");
  const changed = fixture({ approved: true, action: "exclude-candidate", fields: { version: "2.0.0" }, states: {
    rights: { state: "근거 확인", evidence: "fresh terms task" }, boundary: { state: "확정", evidence: "fresh boundary task" }
  } }); throws(() => registry.check(changed.document, changed.policy, old.document), "REGISTRY_DECISION_REVIEW");
});
test("changed dependency may reset axes without inheriting clearance", () => { const before = fixture({ approved: true }), after = fixture({ fields: { version: "2.0.0" } }); assert.equal(registry.check(after.document, after.policy, before.document).transitionCompared, true); });
test("history cannot silently drop previous product", () => { const f = fixture(); throws(() => registry.validateTransition(registry.parseRegistry(f.document), []), "REGISTRY_HISTORY_REMOVED"); });
test("conditions, notices, use and Import changes invalidate inherited confirmed states", () => {
  for (const field of ["usage_conditions", "notices", "purpose", "game_usage", "references", "meta_import"]) {
    const before = fixture({ approved: true }), after = fixture({ approved: true, fields: { [field]: "new reviewed input required" } });
    throws(() => registry.check(after.document, after.policy, before.document), "REGISTRY_TRANSITION_EVIDENCE");
  }
});
test("project settings cannot masquerade as provider originals", () => {
  const f = fixture({ path: "Assets/Settings/Synthetic.asset", approved: true, action: "exclude-candidate" });
  throws(() => registry.check(f.document, f.policy), "REGISTRY_SETTINGS_ROLE");
});
test("require performs no IO/process actions", () => {
  const script = "const fs=require('node:fs'),cp=require('node:child_process'); for(const k of ['writeFileSync','appendFileSync','mkdirSync','rmSync','unlinkSync','renameSync'])fs[k]=()=>{throw Error('unexpected write')}; for(const k of ['spawnSync','execSync','execFileSync','spawn'])cp[k]=()=>{throw Error('unexpected process')}; require(process.argv[1]);";
  const run = cp.spawnSync(process.execPath, ["-e", script, path.join(__dirname, "dependency-registry.cjs")], { encoding: "utf8", windowsHide: true });
  assert.equal(run.status, 0, run.stderr); assert.equal(run.stdout, "");
});
test("BOM/CRLF supported, malformed status/table and hidden linebreak rejected", () => {
  const f = fixture(); assert.equal(registry.parseRegistry("\ufeff" + f.document.replace(/\n/g, "\r\n")).length, 1);
  throws(() => registry.parseRegistry(f.document.replace('| status | `{"adoption"', '| status | `{"broken"')), "REGISTRY_SCHEMA");
  throws(() => registry.parseRegistry(f.document.replace("| --- | --- |\n", "| --- |\n")), "REGISTRY_TABLE");
  const g = fixture({ fields: { purpose: "first<br>second" } }); throws(() => registry.parseRegistry(g.document), "REGISTRY_VALUE");
});
test("duplicate JSON keys and missing item heading never become empty success", () => {
  const f = fixture();
  throws(() => registry.parseRegistry(f.document.replace('"adoption":{', '"decision":{},"adoption":{')), "REGISTRY_JSON_DUPLICATE_KEY");
  throws(() => registry.parseRegistry(f.document.replace("## synthetic-asset\n", "synthetic-asset\n")), "REGISTRY_ITEM_HEADING");
});
test("CLI require/check has no writes; invalid JSON and arguments are sanitized", () => {
  const base = fs.realpathSync(os.tmpdir()), root = fs.mkdtempSync(path.join(base, "flowstate-registry-"));
  try {
    fs.mkdirSync(path.join(root, ".git")); fs.writeFileSync(path.join(root, ".git/index"), "synthetic index");
    for (const dir of ["AI/00_Project", "Tools/DependencyAudit"]) fs.mkdirSync(path.join(root, dir), { recursive: true });
    const f = fixture(); fs.writeFileSync(path.join(root, "AI/00_Project/ASSET_DEPENDENCIES.md"), f.document);
    const policyFile = path.join(root, "Tools/DependencyAudit/dependency-policy.json"); fs.writeFileSync(policyFile, JSON.stringify(f.policy));
    const before = fs.readFileSync(path.join(root, ".git/index")); const success = registry.cli(["--check", "--root", root]); assert.equal(success.exitCode, 0); assert.deepEqual(fs.readFileSync(path.join(root, ".git/index")), before);
    const run = cp.spawnSync(process.execPath, [path.join(__dirname, "dependency-registry.cjs"), "--root", root], { encoding: "utf8", windowsHide: true }); assert.equal(run.status, 0); assert.equal(JSON.parse(run.stdout).recordCount, 1);
    fs.writeFileSync(policyFile, '{"hidden-token":'); const failure = registry.cli(["--root", root]); assert.equal(failure.exitCode, 2); assert(!JSON.stringify(failure).includes("hidden-token"));
    assert.equal(registry.cli(["--unexpected-token"]).exitCode, 2);
    assert.equal(registry.cli(["--previous", "../escape"]).exitCode, 2);
    fs.writeFileSync(policyFile, JSON.stringify(f.policy)); fs.unlinkSync(path.join(root, "AI/00_Project/ASSET_DEPENDENCIES.md")); assert.equal(registry.cli(["--root", root]).exitCode, 2);
  } finally { const resolved = path.resolve(root); assert.equal(path.dirname(resolved).toLowerCase(), path.resolve(base).toLowerCase()); assert(path.basename(resolved).startsWith("flowstate-registry-")); fs.rmSync(resolved, { recursive: true, force: true }); }
});
let passed = 0;
for (const item of tests) { try { item.run(); passed++; } catch (e) { console.error("FAIL " + item.name); throw e; } }
console.log(JSON.stringify({ suite: "DependencyRegistry", passed, failed: 0, unityExecuted: false }));
