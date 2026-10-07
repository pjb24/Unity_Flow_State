const { fault } = require("./account-store");
const { createAccountService } = require("./account-service");
const { createReceiptJournal } = require("./receipt-journal");
const { bucketFor } = require("./ledger-v2-migration");
const stage = "fs-stage-stage-001-r1";

function valid(ledger, accountId) {
  return ledger && ledger.version === 2 && ledger.accountId === accountId && ledger.best &&
    typeof ledger.active === "string" && (ledger.pending === null || typeof ledger.pending === "object");
}
function createV2SubmissionService(store, receipts, boards, project, clock, validate, reply) {
  const accounts = createAccountService(store, clock), journal = createReceiptJournal(receipts);
  return async (p, payload, context) => {
    const initial = await accounts.resolve(context, p.submissionId), accountId = initial.account.value.accountId;
    const ownerId = initial.account.value.leaderboardOwnerId, revision = initial.account.value.connectionRevision;
    const authority = async reserved => {
      const current = await accounts.resolve(context, p.submissionId);
      if (current.account.value.accountId !== accountId || current.account.value.connectionRevision !== revision ||
          current.account.value.leaderboardOwnerId !== ownerId) throw fault("AccountConflict");
      if (reserved && (!current.account.value.onlineOperation || current.account.value.onlineOperation.id !== p.submissionId)) throw fault("AccountBusy");
      return current;
    };
    const release = async entry => { await accounts.releaseSubmission(context, p.submissionId); return reply(entry.status, entry.reason); };
    let item = await store.read("ledger", accountId);
    if (!valid(item && item.value, accountId)) throw fault("LedgerConflict");
    const directory = item.value.receiptDirectory;
    if (directory && Array.isArray(directory.buckets) && directory.buckets.length) {
      // One oldest bucket/shard per request: bounded opportunistic cleanup.
      await journal.cleanupOneShard(accountId, directory.buckets[0], clock());
    }
    let receipt = await journal.find(accountId, p.submissionId);
    if (receipt) return receipt.payload === payload ? release(receipt) : reply("Rejected", "SubmissionIdConflict");
    if (item.value.active && item.value.active !== p.submissionId) return reply("TransientFailure", "EarlierSubmissionPending");
    await accounts.reserveSubmission(context, p.submissionId); await authority(true);
    item = await store.read("ledger", accountId);
    if (!valid(item && item.value, accountId)) throw fault("LedgerConflict");
    let entry = item.value.pending;
    if (entry && (entry.id !== p.submissionId || entry.payload !== payload)) throw fault("LedgerConflict");
    if (!entry) {
      const reason = validate(p), acceptedAt = clock();
      if (!Number.isSafeInteger(acceptedAt) || acceptedAt <= 0) throw fault("InvalidServerTime");
      entry = { id: p.submissionId, payload, status: reason ? "Rejected" : "Pending", reason: reason || "Accepted", acceptedAt };
      await store.compareExchange(item, { ...item.value, active: p.submissionId, pending: entry });
    }
    if (entry.status === "Pending") {
      const best = item.value.best[p.boardId], improves = !best || (p.boardId === stage ? p.score < best.score : p.score > best.score);
      if (improves) {
        await authority(true); const mapping = await store.read("owner", ownerId);
        if (!mapping || mapping.value.accountId !== accountId) throw fault("AccountConflict");
        await boards.addLeaderboardPlayerScore(project, p.boardId, ownerId, { score: p.score, metadata: { submissionId: entry.id, acceptedAt: entry.acceptedAt } });
      }
      entry = { ...entry, status: "Submitted" };
      item = await store.read("ledger", accountId);
      if (!valid(item && item.value, accountId) || !item.value.pending || item.value.pending.id !== entry.id) throw fault("LedgerConflict");
      await journal.append(accountId, bucketFor(entry.acceptedAt), entry);
      const bucket = bucketFor(entry.acceptedAt), buckets = [...new Set([...(item.value.receiptDirectory && item.value.receiptDirectory.buckets || []), bucket])].sort();
      await store.compareExchange(item, { ...item.value, active: "", pending: null,
        receiptDirectory: { ...(item.value.receiptDirectory || { version: 1, cleanup: null }), buckets },
        best: improves ? { ...item.value.best, [p.boardId]: { score: p.score, acceptedAt: entry.acceptedAt, id: entry.id } } : item.value.best });
    } else {
      await journal.append(accountId, bucketFor(entry.acceptedAt), entry);
      item = await store.read("ledger", accountId);
      const bucket = bucketFor(entry.acceptedAt), buckets = [...new Set([...(item.value.receiptDirectory && item.value.receiptDirectory.buckets || []), bucket])].sort();
      await store.compareExchange(item, { ...item.value, active: "", pending: null,
        receiptDirectory: { ...(item.value.receiptDirectory || { version: 1, cleanup: null }), buckets } });
    }
    return release(entry);
  };
}
module.exports = { createV2SubmissionService, valid };
