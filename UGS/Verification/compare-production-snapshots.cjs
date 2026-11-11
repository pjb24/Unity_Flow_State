"use strict";
// Offline, read-only comparison. Private files never enter stdout/stderr.
const fs = require("node:fs"), { isDeepStrictEqual: same } = require("node:util");
const { loadScope, validateScope } = require("./production-scope.cjs");
const boards = ["fs-stage-stage-001-r1", "fs-infinite-v2"];
const text = x => typeof x === "string" && x.length > 0;
const number = x => Number.isSafeInteger(x) && x >= 0;
const inScope = (x, scope) => !!x && x.projectId === scope.projectId && x.environmentId === scope.environmentId;
function validSave(x, scope) {
  return !!x && x.version === 7 && x.onlineProjectId === scope.projectId && x.onlineEnvironmentId === scope.environmentId &&
    text(x.accountId) && x.recoveryNoticeConfirmed === true && text(x.onlinePlayerId) && /^\d{10}$/.test(x.publicPlayerNumber) &&
    Array.isArray(x.personalBests) && Array.isArray(x.pendingSubmissions) &&
    x.pendingSubmissions.every(p => p && text(p.playerId) && text(p.submissionId) &&
      Number.isSafeInteger(p.createdAtMilliseconds) && p.createdAtMilliseconds > 0) &&
    new Set(x.pendingSubmissions.map(p => p.playerId + ":" + p.submissionId)).size === x.pendingSubmissions.length;
}
function validServer(x, scope, detached = false) {
  const a = x && x.account;
  return inScope(x, scope) && inScope(a, scope) && text(a.accountId) && text(a.leaderboardOwnerId) &&
    (detached ? a.status === "Detached" && a.currentPlayerId === null && text(a.detachedByTransferId) :
      a.status === "Active" && text(a.currentPlayerId)) &&
    /^\d{10}$/.test(a.publicPlayerNumber) && number(a.connectionRevision) && a.connectionRevision > 0 &&
    x.rows && boards.every(b => Object.hasOwn(x.rows, b) && (x.rows[b] === null ||
      (x.rows[b].playerId === a.leaderboardOwnerId && number(x.rows[b].score) && x.rows[b].metadata &&
       number(x.rows[b].metadata.acceptedAt) && x.rows[b].metadata.acceptedAt > 0 && text(x.rows[b].metadata.submissionId))));
}
function compare(mode, before, after, scope) {
  validateScope(scope);
  if (mode === "server-restart" || mode === "transfer" || mode === "recipient-detached") {
    if (!validServer(before, scope) || !validServer(after, scope, mode === "recipient-detached")) return "SERVER_INPUT_INVALID";
    const a = before.account, b = after.account;
    if (a.accountId !== b.accountId || a.publicPlayerNumber !== b.publicPlayerNumber || a.leaderboardOwnerId !== b.leaderboardOwnerId)
      return "ACCOUNT_CHANGED";
    if (!boards.every(board => {
      const left = before.rows[board], right = after.rows[board];
      return left === null && right === null || left !== null && right !== null && left.playerId === right.playerId &&
        left.score === right.score && same(left.metadata, right.metadata);
    })) return "SERVER_ROWS_CHANGED";
    if (mode === "recipient-detached") return b.connectionRevision === a.connectionRevision + 1 ? "PRESERVED" : "CONNECTION_CHANGED";
    if (mode === "server-restart") return a.currentPlayerId === b.currentPlayerId && a.connectionRevision === b.connectionRevision ? "PRESERVED" : "CONNECTION_CHANGED";
    const source = after.sourceBinding, target = after.targetBinding;
    return a.currentPlayerId !== b.currentPlayerId && b.connectionRevision === a.connectionRevision + 1 &&
      inScope(source, scope) && inScope(target, scope) && source.playerId === a.currentPlayerId && target.playerId === b.currentPlayerId &&
      source.accountId === b.accountId && target.accountId === b.accountId && source.status === "Inactive" && target.status === "Active" &&
      target.connectionRevision === b.connectionRevision ? "PRESERVED" : "CONNECTION_CHANGED";
  }
  if (!validSave(before, scope) || !validSave(after, scope)) return "SAVE_INPUT_INVALID";
  const fields = ["accountId", "onlinePlayerId", "publicPlayerNumber", "recoveryNoticeConfirmed"];
  if (!fields.every(f => same(before[f], after[f]))) return "ACCOUNT_CHANGED";
  if (mode === "restart" || mode === "pending-restored") {
    if (mode === "pending-restored" && before.pendingSubmissions.length === 0) return "PENDING_BASELINE_EMPTY";
    return same(before, after) ? "PRESERVED" : "SAVE_CHANGED";
  }
  if (mode === "pending-removed") {
    // Confirms only removal. Submitted/Rejected must independently be observed in the Player/server.
    if (before.pendingSubmissions.length === 0) return "PENDING_BASELINE_EMPTY";
    if (after.pendingSubmissions.length !== 0) return "PENDING_REMAINS";
    return "REMOVED_ONLY";
  }
  return "MODE_INVALID";
}
function main(args = process.argv.slice(2), report = console.log, scopeLoader = loadScope, reader = p => fs.readFileSync(p, "utf8")) {
  try {
    if (args.length !== 3 || !["restart", "pending-restored", "pending-removed", "server-restart", "transfer", "recipient-detached"].includes(args[0])) throw Error();
    const parse = p => JSON.parse(reader(p).replace(/^\uFEFF/, ""));
    const result = compare(args[0], parse(args[1]), parse(args[2]), scopeLoader());
    const ok = result === "PRESERVED" || result === "REMOVED_ONLY";
    report(`${ok ? "PASS" : "FAIL"} / PRODUCTION_COMPARE / ${result}`);
    return ok;
  } catch (_) { report("FAIL / PRODUCTION_COMPARE / INPUT_INVALID"); return false; }
}
if (require.main === module && !main()) process.exitCode = 1;
module.exports = { compare, main, validServer };
