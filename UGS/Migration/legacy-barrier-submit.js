// Historical cutover bridge ONLY. Never publish under a second endpoint name.
// Step 9 uses this as submit-record before draining legacy invocations; replace
// with the C endpoint before new Client activation. Never roll back after cutover.
// verification only. Never deploy into production.
const { DataApi } = require("@unity-services/cloud-save-1.4");
const { LeaderboardsApi } = require("@unity-services/leaderboards-1.1");
const project = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environment = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const key = "fs_submission_ledger_v1";
const stage = "fs-stage-stage-001-r1";
const infinite = "fs-infinite-v2";
const intMax = 2147483647;
const uuid = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

module.exports = async ({ params, context }) => {
  if (context.projectId !== project || context.environmentId !== environment || !context.playerId)
    return reply("TransientFailure", "ContextMismatch");
  let p;
  try { p = JSON.parse(params.request); } catch (_) { return reply("Rejected", "InvalidRequest"); }
  if (!p || typeof p.submissionId !== "string" || !uuid.test(p.submissionId))
    return reply("Rejected", "InvalidSubmissionId");
  p.submissionId = p.submissionId.toLowerCase();
  const payload = JSON.stringify([p.boardId, p.rulesVersion, p.submissionId, p.score,
    p.runDurationMilliseconds, p.baseDistanceScore, p.momentumBonus, p.distanceScore,
    p.collectibleScore, p.totalScore, p.maximumMomentumMultiplier]);
  if (payload.length > 2048) return reply("Rejected", "InvalidRequest");
  const save = new DataApi(context); // serviceToken, never player accessToken
  const boards = new LeaderboardsApi(context);
  try {
    let item = await read(save, context);
    // Missing keys cannot safely be initialized with an unconditional write under concurrency.
    if (!validLedger(item)) return reply("TransientFailure", "LedgerNotProvisioned");
    let ledger = item.value;
    if (ledger.migration) return reply("TransientFailure", "AccountMigrationRequired");
    let entry = ledger.entries.find(e => e.id === p.submissionId);
    if (entry && entry.payload !== payload) return reply("Rejected", "SubmissionIdConflict");
    if (entry && entry.status !== "Pending") return reply(entry.status, entry.reason);
    if (!entry) {
      if (ledger.active) return reply("TransientFailure", "EarlierSubmissionPending");
      if (ledger.entries.length >= 128) return reply("TransientFailure", "LedgerCapacity");
      const reason = validate(p);
      entry = { id: p.submissionId, payload, status: reason ? "Rejected" : "Pending",
        reason: reason || "Accepted", acceptedAt: Date.now() };
      ledger.entries.push(entry);
      if (!reason) ledger.active = p.submissionId;
      await write(save, context, item, ledger);
      if (reason) return reply("Rejected", reason);
    }
    if (ledger.active !== p.submissionId) return reply("TransientFailure", "LedgerConflict");
    const best = ledger.best[p.boardId];
    const improves = !best || (p.boardId === stage ? p.score < best.score : p.score > best.score);
    if (improves) {
      await boards.addLeaderboardPlayerScore(project, p.boardId, context.playerId,
        { score: p.score, metadata: { submissionId: entry.id, acceptedAt: entry.acceptedAt } });
    }
    // Recovery repeats the same score and timestamp after an ambiguous write response.
    item = await read(save, context);
    if (!validLedger(item)) return reply("TransientFailure", "LedgerConflict");
    ledger = item.value;
    if (ledger.migration) return reply("TransientFailure", "AccountMigrationRequired");
    const finalEntry = ledger.entries.find(e => e.id === p.submissionId);
    if (!finalEntry || finalEntry.payload !== payload) return reply("TransientFailure", "LedgerConflict");
    if (finalEntry.status !== "Pending") return reply(finalEntry.status, finalEntry.reason);
    if (ledger.active !== p.submissionId) return reply("TransientFailure", "LedgerConflict");
    if (improves) ledger.best[p.boardId] = { score: p.score, acceptedAt: entry.acceptedAt, id: entry.id };
    finalEntry.status = "Submitted";
    ledger.active = "";
    await write(save, context, item, ledger);
    return reply("Submitted", "Accepted");
  } catch (_) {
    // CAS conflict, timeout, rate limit and service errors keep the local candidate Pending.
    return reply("TransientFailure", "ServiceUnavailable");
  }
};
function reply(status, reason) { return { status, reason }; }
async function read(api, context) {
  const response = await api.getProtectedItems(project, context.playerId, [key]);
  return response.data.results.find(item => item.key === key);
}
function validLedger(item) {
  return item && item.writeLock && item.value && item.value.version === 1 &&
    Array.isArray(item.value.entries) && item.value.best && typeof item.value.active === "string";
}
async function write(api, context, item, value) {
  await api.setProtectedItem(project, context.playerId, { key, value, writeLock: item.writeLock });
}
function integer(value, max = Number.MAX_SAFE_INTEGER) {
  return Number.isSafeInteger(value) && value >= 0 && value <= max;
}
function validate(p) {
  if (!integer(p.score)) return "InvalidScore";
  if (p.boardId === stage) {
    if (p.rulesVersion !== 1) return "InvalidVersion";
    // Stage clear provenance is a client policy, not server-authoritative anti-cheat proof.
    return p.runDurationMilliseconds === 0 && p.baseDistanceScore === 0 &&
      p.momentumBonus === 0 && p.distanceScore === 0 && p.collectibleScore === 0 &&
      p.totalScore === 0 && p.maximumMomentumMultiplier === 1 ? "" : "InvalidStage";
  }
  if (p.boardId !== infinite) return "InvalidBoard";
  if (p.rulesVersion !== 2) return "InvalidVersion";
  if (!integer(p.runDurationMilliseconds) || !integer(p.baseDistanceScore, intMax) ||
      !integer(p.momentumBonus, intMax) || !integer(p.distanceScore, intMax) ||
      !integer(p.collectibleScore, intMax) || !integer(p.totalScore, intMax) ||
      !Number.isFinite(p.maximumMomentumMultiplier) || p.maximumMomentumMultiplier < 1 ||
      p.maximumMomentumMultiplier > 3) return "InvalidComponents";
  if (p.distanceScore !== Math.min(intMax, p.baseDistanceScore + p.momentumBonus) ||
      p.totalScore !== Math.min(intMax, p.distanceScore + p.collectibleScore) ||
      p.score !== p.totalScore) return "InvalidSum";
  // Scoring-v2 server constants: speed 8, points/unit 10, multiplier 3,
  // minimum pattern length 44, max 20 collectibles/pattern, 10 points/item.
  const distance = p.runDurationMilliseconds / 1000 * 8;
  const maximum = Math.min(intMax, Math.floor(distance * 10 * 3 + Math.ceil(distance / 44) * 20 * 10));
  return p.totalScore <= maximum ? "" : "ScoreLimitExceeded";
}
module.exports.params = { request: { type: "String", required: true } };
