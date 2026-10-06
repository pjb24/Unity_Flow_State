"use strict";
// Read user-local exports only. No SDK/network, secret reads or file writes.
const fs = require("node:fs");
const { parseSnapshot, compareLegacy } = require("./compare-service-snapshots.cjs");
const { sameJsonValue } = require("../CloudCode/json-value-policy");
const { projectId, environmentId } = require("../CloudCode/account-store");
const { boards } = require("../CloudCode/legacy-account-store");
const scoped = value => !!(value && value.projectId === projectId && value.environmentId === environmentId);
const object = value => !!(value && typeof value === "object" && !Array.isArray(value));
const rowValid = (row, player) => row === null || !!(row && row.playerId === player &&
  Number.isSafeInteger(row.score) && row.score >= 0 && object(row.metadata) &&
  Number.isSafeInteger(row.metadata.acceptedAt) && row.metadata.acceptedAt > 0 &&
  typeof row.metadata.submissionId === "string" && row.metadata.submissionId.length > 0);
function rowsValid(value, player) {
  return object(value) && boards.every(board => Object.hasOwn(value, board) && rowValid(value[board], player));
}
function ledgerValid(value) {
  return !!(object(value) && value.version === 1 && value.active === "" && Array.isArray(value.entries) &&
    value.entries.every(entry => object(entry) && ["Submitted", "Rejected"].includes(entry.status)) && object(value.best));
}
function beforeReady(before) {
  if (!scoped(before) || typeof before.legacyPlayerId !== "string" || !before.legacyPlayerId ||
      !Object.hasOwn(before, "playerBinding") || before.playerBinding !== null ||
      !Object.hasOwn(before, "legacyLedger") || !rowsValid(before.rows, before.legacyPlayerId) ||
      !boards.some(board => before.rows[board] !== null)) return false;
  if (before.legacyLedger !== null && (!ledgerValid(before.legacyLedger) ||
      Object.hasOwn(before.legacyLedger, "migration"))) return false;
  const best = before.legacyLedger === null ? {} : before.legacyLedger.best;
  return Object.keys(best).every(board => boards.includes(board) && object(best[board]) && before.rows[board] &&
    best[board].score === before.rows[board].score && best[board].acceptedAt === before.rows[board].metadata.acceptedAt &&
    best[board].id === before.rows[board].metadata.submissionId);
}
function sameRows(left, right) {
  return boards.every(board => left[board] === null ? right[board] === null :
    right[board] && left[board].playerId === right[board].playerId && left[board].score === right[board].score &&
    sameJsonValue(left[board].metadata, right[board].metadata)); // Rank is not source-owned.
}
function compareCutover(before, after) {
  if (!beforeReady(before) || !compareLegacy(before, after) ||
      !rowsValid(after.rows, before.legacyPlayerId) || !sameRows(before.rows, after.rows)) return false;
  const account = after.account, binding = after.playerBinding, ledger = after.ledger;
  if (account.initialPlayerId !== before.legacyPlayerId || account.connectionRevision !== 1 ||
      !scoped(binding) || binding.playerId !== before.legacyPlayerId || binding.accountId !== account.accountId ||
      binding.connectionRevision !== 1 || binding.status === "Inactive" ||
      !scoped(ledger) || ledger.accountId !== account.accountId || !ledgerValid(ledger)) return false;
  const source = before.legacyLedger === null ? { version: 1, active: "", entries: [], best: {} } : before.legacyLedger;
  const frozen = { ...source, migration: { accountId: account.accountId, projectId, environmentId,
    hadLegacyLedger: before.legacyLedger !== null } };
  const scores = {}, best = { ...source.best };
  for (const board of boards) if (before.rows[board] !== null) {
    const row = before.rows[board];
    scores[board] = { playerId: row.playerId, score: row.score, metadata: row.metadata };
    if (!best[board]) best[board] = { score: row.score, acceptedAt: row.metadata.acceptedAt, id: row.metadata.submissionId };
  }
  const snapshot = { ledger: frozen, scores };
  return sameJsonValue(after.sourceLedger, frozen) && sameJsonValue(account.migration, snapshot) &&
    sameJsonValue(ledger.sourceSnapshot, snapshot) && sameJsonValue(ledger.entries, source.entries) &&
    sameJsonValue(ledger.best, best);
}
function compareRestart(first, restarted) {
  if (!scoped(first) || !scoped(restarted) || !object(first.account) || !object(restarted.account)) return false;
  const a = first.account, b = restarted.account;
  return scoped(a) && scoped(b) && a.status === "Active" && b.status === "Active" &&
    /^\d{10}$/.test(a.publicPlayerNumber) && a.publicPlayerNumber === b.publicPlayerNumber &&
    ["accountId", "initialPlayerId", "currentPlayerId", "leaderboardOwnerId", "connectionRevision"].every(key =>
      a[key] !== undefined && a[key] === b[key]) && rowsValid(first.rows, a.leaderboardOwnerId) &&
    rowsValid(restarted.rows, b.leaderboardOwnerId) && sameRows(first.rows, restarted.rows) &&
    sameJsonValue(first.playerBinding, restarted.playerBinding) && sameJsonValue(first.sourceLedger, restarted.sourceLedger) &&
    sameJsonValue(a.migration, b.migration) && sameJsonValue(first.ledger, restarted.ledger);
}
module.exports = { beforeReady, compareCutover, compareRestart };
if (require.main === module) {
  try {
    const [mode, firstPath, secondPath] = process.argv.slice(2);
    if (!["before", "cutover", "restart"].includes(mode) || !firstPath || (mode !== "before" && !secondPath)) throw new Error();
    const first = parseSnapshot(fs.readFileSync(firstPath, "utf8"));
    const second = mode === "before" ? null : parseSnapshot(fs.readFileSync(secondPath, "utf8"));
    const passed = mode === "before" ? beforeReady(first) : mode === "cutover" ? compareCutover(first, second) : compareRestart(first, second);
    const category = mode === "before" ? "LEGACY_BEFORE_READY" : mode === "cutover" ? "LEGACY_CUTOVER_ROWS_AND_LEDGER_PRESERVED" : "LEGACY_RESTART_PRESERVED";
    console.log(`${passed ? "PASS" : "FAIL"} / ${category} / No private values printed`);
    process.exitCode = passed ? 0 : 1;
  } catch (_) { console.log("FAIL / SNAPSHOT_INPUT_INVALID / No private values printed"); process.exitCode = 1; }
}
