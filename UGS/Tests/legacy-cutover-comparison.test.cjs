// Actual production provisioning against isolated SDK doubles, no remote writes.
const assert = require("node:assert/strict");
const { fixture, clone } = require("./account-sdk-double.cjs");
const { createAccountStore, projectId, environmentId } = require("../CloudCode/account-store");
const { createLegacyAccountStore, ledgerKey, boards } = require("../CloudCode/legacy-account-store");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { beforeReady, compareCutover, compareRestart } = require("../Verification/compare-legacy-cutover.cjs");
let checks = 0;
function check(value, expected) { assert.equal(value, expected); checks++; }
async function snapshots(withLedger) {
  const f = fixture(), scope = { projectId, environmentId };
  const receipt = "11111111-1111-4111-8111-111111111111";
  const oldLedger = { version: 1, active: "", entries: [
    { id: receipt, payload: "synthetic-payload", status: "Submitted", acceptedAt: 123 },
    { id: "rejected", payload: "synthetic-rejected", status: "Rejected", reason: "InvalidScore", acceptedAt: 124 }
  ], best: { [boards[0]]: { score: 60000, acceptedAt: 123, id: receipt } } };
  if (withLedger) f.seedProtected("A", ledgerKey, oldLedger);
  for (const [board, score] of [[boards[0], 60000], [boards[1], 10]])
    f.scores.set(board + "/A", { playerId: "A", score, metadata: { acceptedAt: 123, submissionId: receipt, retainedExtra: "original" } });
  const rows = () => Object.fromEntries(boards.map(board => [board, clone(f.scores.get(board + "/A"))]));
  const before = { ...scope, legacyPlayerId: "A", playerBinding: null, legacyLedger: withLedger ? clone(oldLedger) : null, rows: rows() };
  const provision = createAccountProvisioningService(createAccountStore(f.context, f.save), createLegacyAccountStore(f.context, f.save, f.leaderboard));
  await provision.ensure(f.context);
  const after = () => {
    const binding = clone(f.value("fs8-player-A")), account = clone(f.value("fs8-account-" + binding.accountId));
    return { ...scope, account, playerBinding: binding, sourceLedger: clone(f.protectedItems.get("A/" + ledgerKey).value),
      ledger: clone(f.value("fs8-ledger-" + binding.accountId)), rows: rows() };
  };
  const first = after(), writes = f.state.writes;
  await provision.ensure(f.context); assert.equal(f.state.writes, writes);
  return { before, first, restarted: after() };
}
async function run() {
  const fs = require("node:fs"), path = require("node:path");
  const beforeTemplate = JSON.parse(fs.readFileSync(path.resolve(__dirname, "../Verification/Templates/legacy-before.example.json"), "utf8"));
  const afterTemplate = JSON.parse(fs.readFileSync(path.resolve(__dirname, "../Verification/Templates/legacy-after.example.json"), "utf8"));
  check(beforeReady(beforeTemplate), false); check(compareCutover(beforeTemplate, afterTemplate), false);
  for (const withLedger of [false, true]) {
    const { before, first, restarted } = await snapshots(withLedger);
    check(beforeReady(before), true); check(compareCutover(before, first), true);
    check(compareCutover(before, restarted), true); check(compareRestart(first, restarted), true);
    const reordered = value => Array.isArray(value) ? value.map(reordered) : value && typeof value === "object" ?
      Object.fromEntries(Object.keys(value).sort().reverse().map(key => [key, reordered(value[key])])) : value;
    check(compareCutover(reordered(before), reordered(first)), true);
    for (const mutate of [
      b => { delete b.playerBinding; }, b => { b.playerBinding = {}; }, b => { b.rows = Object.fromEntries(boards.map(board => [board, null])); },
      b => { b.rows[boards[0]].metadata = {}; }, b => { b.environmentId = "different"; }, b => { delete b.rows[boards[1]]; },
      b => { if (b.legacyLedger) b.legacyLedger.migration = {}; else b.legacyLedger = { version: 1, active: "", entries: [], best: {}, migration: {} }; }
    ]) { const bad = clone(before); mutate(bad); check(beforeReady(bad), false); check(compareCutover(bad, first), false); }
    for (const mutate of [
      a => { a.account.status = "Preparing"; }, a => { a.account.initialPlayerId = "other"; }, a => { a.account.connectionRevision = 2; },
      a => { a.account.currentPlayerId = "other"; }, a => { a.account.leaderboardOwnerId = "other"; },
      a => { a.playerBinding.accountId = "other"; }, a => { a.sourceLedger.migration.hadLegacyLedger = !withLedger; },
      a => { a.ledger.entries.push({ status: "Submitted", id: "unexpected" }); }, a => { a.ledger.best[boards[0]].score++; },
      a => { a.ledger.sourceSnapshot.scores[boards[0]].metadata.acceptedAt++; }, a => { a.account.migration.ledger.active = "busy"; },
      a => { a.rows[boards[0]].metadata.retainedExtra = "changed"; }, a => { a.rows[boards[0]].score++; }
    ]) { const bad = clone(first); mutate(bad); check(compareCutover(before, bad), false); }
    for (const mutate of [
      a => { a.account.publicPlayerNumber = "9999999999"; }, a => { a.account.connectionRevision++; },
      a => { a.ledger.best[boards[0]].acceptedAt++; }, a => { a.playerBinding.accountId = "other"; },
      a => { a.rows[boards[1]].metadata.submissionId = "changed"; }
    ]) { const bad = clone(restarted); mutate(bad); check(compareRestart(first, bad), false); }
    const changedRank = clone(restarted); changedRank.rows[boards[0]].rank = 99;
    check(compareCutover(before, changedRank), true); check(compareRestart(first, changedRank), true);
  }
  console.log("PASS legacy cutover comparison: " + checks + " checks, actual production migration/restart, row/receipt/best preservation, no remote calls");
}
run().catch(error => { console.error(error); process.exitCode = 1; });
