// verification only. Never deploy into production.
const { createAccountStore, validateContext, fault } = require("./account-store");
const { createAccountService } = require("./account-service");
const project = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environment = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const stage = "fs-stage-stage-001-r1";
const infinite = "fs-infinite-v2";
const intMax = 2147483647;
const uuid = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

function createRecordSubmissionService(store, boards, clock = Date.now) {
 const accounts = createAccountService(store, clock);
 return async ({ params, context }) => {
  try { validateContext(context); } catch (_) { return reply("TransientFailure", "ContextMismatch"); }
  let p;
  try { p = JSON.parse(params.request); } catch (_) { return reply("Rejected", "InvalidRequest"); }
  if (!p || typeof p.submissionId !== "string" || !uuid.test(p.submissionId))
    return reply("Rejected", "InvalidSubmissionId");
  p.submissionId = p.submissionId.toLowerCase();
  const payload = JSON.stringify([p.boardId, p.rulesVersion, p.submissionId, p.score,
    p.runDurationMilliseconds, p.baseDistanceScore, p.momentumBonus, p.distanceScore,
    p.collectibleScore, p.totalScore, p.maximumMomentumMultiplier]);
  if (payload.length > 2048) return reply("Rejected", "InvalidRequest");
  try {
    const initial = await accounts.resolve(context, p.submissionId);
    const accountId = initial.account.value.accountId;
    const revision = initial.account.value.connectionRevision;
    const ownerId = initial.account.value.leaderboardOwnerId;
    async function authority(reserved = false) {
      const current = await accounts.resolve(context, p.submissionId);
      if (current.account.value.accountId !== accountId || current.account.value.connectionRevision !== revision ||
          current.account.value.leaderboardOwnerId !== ownerId) throw fault("AccountConflict");
      if (reserved && (!current.account.value.onlineOperation ||
          current.account.value.onlineOperation.id !== p.submissionId)) throw fault("AccountBusy");
      return current;
    }
    async function terminal(entry) {
      const current = await authority();
      if (current.account.value.onlineOperation && current.account.value.onlineOperation.id === p.submissionId)
        await accounts.releaseSubmission(context, p.submissionId);
      await authority();
      return reply(entry.status, entry.reason);
    }
    let item = await store.read("ledger", accountId);
    // Missing keys cannot safely be initialized with an unconditional write under concurrency.
    if (!validLedger(item, accountId)) return reply("TransientFailure", "LedgerNotProvisioned");
    let ledger = item.value;
    // The frozen legacy marker is preserved in the C snapshot; it does not
    // prohibit writes to the separate, authoritative C ledger.
    if (ledger.migration && ledger.migration.accountId !== accountId)
      return reply("TransientFailure", "LedgerConflict");
    let entry = ledger.entries.find(e => e.id === p.submissionId);
    if (entry && entry.payload !== payload) return reply("Rejected", "SubmissionIdConflict");
    if (entry && entry.status !== "Pending") return await terminal(entry);
    if (!entry && ledger.active) return reply("TransientFailure", "EarlierSubmissionPending");
    if (!entry && ledger.entries.length >= 128) return reply("TransientFailure", "LedgerCapacity");
    await accounts.reserveSubmission(context, p.submissionId);
    // Re-read after the Account CAS: a concurrent same-ID request may have
    // already committed the ledger. Never append using a pre-reservation token.
    await authority(true);
    item = await store.read("ledger", accountId);
    if (!validLedger(item, accountId)) throw fault("LedgerConflict");
    ledger = item.value;
    entry = ledger.entries.find(e => e.id === p.submissionId);
    if (entry && entry.payload !== payload) return reply("Rejected", "SubmissionIdConflict");
    if (entry && entry.status !== "Pending") return await terminal(entry);
    if (!entry) {
      if (ledger.active) return reply("TransientFailure", "EarlierSubmissionPending");
      if (ledger.entries.length >= 128) return reply("TransientFailure", "LedgerCapacity");
      const reason = validate(p);
      entry = { id: p.submissionId, payload, status: reason ? "Rejected" : "Pending",
        reason: reason || "Accepted", acceptedAt: clock() };
      if (!Number.isSafeInteger(entry.acceptedAt) || entry.acceptedAt <= 0) throw fault("InvalidServerTime");
      ledger.entries.push(entry);
      if (!reason) ledger.active = p.submissionId;
      await store.compareExchange(item, ledger);
      if (reason) return await terminal(entry);
    }
    if (ledger.active !== p.submissionId) return reply("TransientFailure", "LedgerConflict");
    const best = ledger.best[p.boardId];
    const improves = !best || (p.boardId === stage ? p.score < best.score : p.score > best.score);
    if (improves) {
      await authority(true);
      const mapping = await store.read("owner", ownerId);
      if (!mapping || mapping.value.accountId !== accountId) throw fault("AccountConflict");
      await boards.addLeaderboardPlayerScore(project, p.boardId, ownerId,
        { score: p.score, metadata: { submissionId: entry.id, acceptedAt: entry.acceptedAt } });
    }
    // Recovery repeats the same score and timestamp after an ambiguous write response.
    await authority(true);
    item = await store.read("ledger", accountId);
    if (!validLedger(item, accountId)) return reply("TransientFailure", "LedgerConflict");
    ledger = item.value;
    if (ledger.migration && ledger.migration.accountId !== accountId)
      return reply("TransientFailure", "LedgerConflict");
    const finalEntry = ledger.entries.find(e => e.id === p.submissionId);
    if (!finalEntry || finalEntry.payload !== payload) return reply("TransientFailure", "LedgerConflict");
    if (finalEntry.status !== "Pending") return await terminal(finalEntry);
    if (ledger.active !== p.submissionId) return reply("TransientFailure", "LedgerConflict");
    if (improves) ledger.best[p.boardId] = { score: p.score, acceptedAt: entry.acceptedAt, id: entry.id };
    finalEntry.status = "Submitted";
    ledger.active = "";
    await store.compareExchange(item, ledger);
    return await terminal(finalEntry);
  } catch (_) {
    // CAS conflict, timeout, rate limit and service errors keep the local candidate Pending.
    return reply("TransientFailure", "ServiceUnavailable");
  }
 };
}
module.exports = async ({ params, context }) => {
  try { validateContext(context); } catch (_) { return reply("TransientFailure", "ContextMismatch"); }
  try {
    const { LeaderboardsApi } = require("@unity-services/leaderboards-1.1");
    return await createRecordSubmissionService(createAccountStore(context), new LeaderboardsApi(context))({ params, context });
  } catch (_) { return reply("TransientFailure", "ServiceUnavailable"); }
};
module.exports.createRecordSubmissionService = createRecordSubmissionService;
function reply(status, reason) { return { status, reason }; }
function validLedger(item, accountId) {
  return item && item.writeLock && item.value && item.value.version === 1 &&
    item.value.accountId === accountId && Array.isArray(item.value.entries) && item.value.best &&
    typeof item.value.active === "string";
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
module.exports.bundling = true;
