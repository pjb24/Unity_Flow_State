// Read-only policy/document handoff checks. Run with Node; no Unity process or network.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");
const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8").replace(/^\uFEFF/, "");
const task = read("AI/90_Tasks/Prototype_8/20260930_01_Phase1ManualSteps.md");
const roadmap = read("AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md");

for (const marker of ["## Step 7. 결정 사항을 문서와 Unit Test 계약으로 고정한다",
  "##### Phase 1 정책–Test–후속 구현 대응표", "## Step 8. Phase 1 완료를 판정하고 Phase 2에 인계한다",
  "Phase 1 판정 및 Phase 2 인계", "Phase 2", "Phase 3", "Phase 4", "Phase 5"])
  assert.ok(task.includes(marker), `Missing handoff marker: ${marker}`);
for (const file of [
  "public-player-number-policy.test.cjs", "leaderboard-owner-policy.test.cjs",
  "legacy-account-migration-policy.test.cjs", "transfer-credential-policy.test.cjs",
  "transfer-resolution-policy.test.cjs", "active-connection-policy.test.cjs",
  "transfer-client-state-policy.test.cjs", "public-number-display-policy.test.cjs",
  "transfer-operations-policy.test.cjs", "submission-retention-policy.test.cjs",
  "leaderboard-query-policy.test.cjs", "environment-data-policy.test.cjs",
  "operational-safety-policy.test.cjs"
]) assert.ok(fs.existsSync(path.join(root, "UGS/Tests", file)), `Missing policy test: ${file}`);
for (const marker of ["C별 시간 분할 terminal receipt", "상위 10·내 주변 7", "30일 로그 sink",
  "완료 — 2026-10-03"])
  assert.ok(roadmap.includes(marker), `Missing roadmap handoff: ${marker}`);

console.log("PASS Phase 1 policy contracts: documents, policy tests, Phase 1 completion and Phase 2-5 handoff mapping");
