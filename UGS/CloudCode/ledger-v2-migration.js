// Pure lazy-migration planner. Persistence is intentionally separate so every
// write boundary can be retried with its own CAS/re-read recovery.
const { fault, projectId, environmentId } = require("./account-store");
const terminal = new Set(["Submitted", "Rejected"]);
const uuid = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

function bucketFor(acceptedAt) {
  if (!Number.isSafeInteger(acceptedAt) || acceptedAt <= 0) throw fault("InvalidServerTime");
  const date = new Date(acceptedAt);
  const year = date.getUTCFullYear();
  if (year < 2000 || year > 9999) throw fault("InvalidServerTime");
  return `${year}${String(date.getUTCMonth() + 1).padStart(2, "0")}`;
}
function isV1(value, accountId) {
  return value && value.schemaVersion === 1 && value.projectId === projectId && value.environmentId === environmentId &&
    value.version === 1 && value.accountId === accountId && Array.isArray(value.entries) && value.best &&
    !Array.isArray(value.best) && typeof value.active === "string";
}
function plan(value, accountId) {
  if (!isV1(value, accountId)) throw fault("LedgerConflict");
  const receipts = [];
  let pending = null;
  for (const entry of value.entries) {
    if (!entry || !uuid.test(entry.id) || typeof entry.payload !== "string" || !Number.isSafeInteger(entry.acceptedAt) || entry.acceptedAt <= 0)
      throw fault("LedgerConflict");
    if (terminal.has(entry.status)) receipts.push({ id: entry.id.toLowerCase(), payload: entry.payload, status: entry.status,
      reason: entry.reason, acceptedAt: entry.acceptedAt, bucket: bucketFor(entry.acceptedAt) });
    else if (entry.status === "Pending" && !pending) pending = { ...entry, id: entry.id.toLowerCase() };
    else throw fault("LedgerConflict");
  }
  if ((value.active === "") !== (pending === null) || (pending && value.active.toLowerCase() !== pending.id)) throw fault("LedgerConflict");
  return {
    ledger: { schemaVersion: 1, projectId, environmentId, version: 2, accountId, best: value.best,
      active: pending ? pending.id : "", pending, receiptDirectory: { version: 1, buckets: [], cleanup: null },
      migration: { sourceVersion: 1, state: receipts.length ? "CopyReceipts" : "Complete", cursor: 0 } },
    receipts
  };
}
module.exports = { bucketFor, isV1, plan };
