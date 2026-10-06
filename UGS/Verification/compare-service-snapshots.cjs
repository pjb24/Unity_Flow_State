"use strict";
// Offline comparisons of user-local Dashboard exports. No SDK/network/secrets.
const fs = require("node:fs");
const project = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environment = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const boards = ["fs-stage-stage-001-r1", "fs-infinite-v2"];
const identity = value => typeof value === "string" && value.length > 0;
const scope = value => value && value.projectId === project && value.environmentId === environment;
const rowValid = row => row === null || !!(row && identity(row.playerId) && Number.isSafeInteger(row.score) && row.score >= 0 &&
  row.metadata && Number.isSafeInteger(row.metadata.acceptedAt) && row.metadata.acceptedAt > 0 && identity(row.metadata.submissionId));
const rowSame = (a, b) => a === null && b === null || a !== null && b !== null && a.playerId === b.playerId && a.score === b.score &&
  a.metadata.acceptedAt === b.metadata.acceptedAt && a.metadata.submissionId === b.metadata.submissionId;
function rowsPreserved(before, after) {
  if (!before.rows || !after.rows) return false;
  return boards.every(board => Object.hasOwn(before.rows, board) && Object.hasOwn(after.rows, board) &&
    rowValid(before.rows[board]) && rowValid(after.rows[board]) && rowSame(before.rows[board], after.rows[board]));
}
function accountValid(account) {
  return scope(account) && identity(account.accountId) && identity(account.leaderboardOwnerId) && identity(account.currentPlayerId) &&
    /^\d{10}$/.test(account.publicPlayerNumber) && Number.isSafeInteger(account.connectionRevision) && account.connectionRevision >= 1;
}
function ownedRows(snapshot) {
  return boards.every(board => snapshot.rows[board] === null || snapshot.rows[board].playerId === snapshot.account.leaderboardOwnerId);
}
function compareLegacy(before, after) {
  if (!scope(before) || !scope(after) || !identity(before.legacyPlayerId) || !accountValid(after.account) ||
      after.account.leaderboardOwnerId !== before.legacyPlayerId || after.account.currentPlayerId !== before.legacyPlayerId ||
      after.account.status !== "Active" || !rowsPreserved(before, after) || !ownedRows(after)) return false;
  return boards.every(board => before.rows[board] === null || before.rows[board].playerId === before.legacyPlayerId);
}
function compareTransfer(before, after) {
  if (!scope(before) || !scope(after) || !accountValid(before.account) || !accountValid(after.account) ||
      before.account.status !== "Active" || after.account.status !== "Active" ||
      before.account.accountId !== after.account.accountId || before.account.publicPlayerNumber !== after.account.publicPlayerNumber ||
      before.account.leaderboardOwnerId !== after.account.leaderboardOwnerId || before.account.currentPlayerId === after.account.currentPlayerId ||
      after.account.connectionRevision !== before.account.connectionRevision + 1 || !rowsPreserved(before, after) || !ownedRows(before) || !ownedRows(after)) return false;
  const a = after.sourceBinding, b = after.targetBinding;
  return !!(scope(a) && scope(b) && a.status === "Inactive" && b.status === "Active" &&
    a.playerId === before.account.currentPlayerId && b.playerId === after.account.currentPlayerId &&
    a.accountId === after.account.accountId && b.accountId === after.account.accountId && b.connectionRevision === after.account.connectionRevision);
}
function compareDistinct(a, b) {
  return !!(scope(a) && scope(b) && a.boardId === b.boardId && boards.includes(a.boardId) &&
    /^\d{10}$/.test(a.publicPlayerNumber) && /^\d{10}$/.test(b.publicPlayerNumber) && a.publicPlayerNumber !== b.publicPlayerNumber);
}
function parseSnapshot(text) {
  // Editor-exported UTF-8 files may start with a BOM. Do not rewrite user files.
  return JSON.parse(text.replace(/^\uFEFF/, ""));
}
module.exports = { compareLegacy, compareTransfer, compareDistinct, parseSnapshot };
if (require.main === module) {
  const [mode, beforePath, afterPath] = process.argv.slice(2);
  try {
    if (!["legacy", "transfer", "distinct"].includes(mode) || !beforePath || !afterPath) throw new Error();
    const before = parseSnapshot(fs.readFileSync(beforePath, "utf8"));
    const after = parseSnapshot(fs.readFileSync(afterPath, "utf8"));
    const passed = mode === "legacy" ? compareLegacy(before, after) : mode === "distinct" ? compareDistinct(before, after) : compareTransfer(before, after);
    const category = mode === "legacy" ? "LEGACY_FIXED_ROW_PRESERVED" : mode === "distinct" ? "AB_NUMBERS_DISTINCT" : "TRANSFER_FIXED_ROW_BINDING_PRESERVED";
    console.log(`${passed ? "PASS" : "FAIL"} / ${category} / ${new Date().toISOString()}`);
    process.exitCode = passed ? 0 : 1;
  } catch (_) { console.log("FAIL / SNAPSHOT_INPUT_INVALID / No private values printed"); process.exitCode = 1; }
}
