// Pure policy only. It does not read, write, copy, delete, or deploy service data.
const owner = require("./leaderboard-owner-policy");

function selectInitialLeaderboardOwner(input) {
  if (!input || typeof input.authenticatedPlayerId !== "string" || !input.authenticatedPlayerId)
    return null;
  const hasLegacyData = input.hasLegacyLedger === true || input.hasStageScore === true ||
    input.hasInfiniteScore === true;
  if (hasLegacyData) {
    const id = owner.tryUseLegacyLeaderboardOwnerId(input.authenticatedPlayerId,
      input.authenticatedPlayerId);
    return id ? { kind: "legacy", id } : null;
  }
  const id = owner.tryNormalizeGeneratedLeaderboardOwnerId(input.generatedOwnerId);
  return id ? { kind: "generated", id } : null;
}

function canActivateMigration(state) {
  return !!state && state.sourceSnapshotRecorded === true && state.destinationLedgerWritten === true &&
    state.accountWritten === true && state.playerBindingWritten === true &&
    state.publicNumberBindingWritten === true && state.ownerBindingWritten === true;
}

function canRollbackToLegacy(state) {
  return !!state && state.cutoverCompleted !== true && state.sourceUnchanged === true;
}

module.exports = { selectInitialLeaderboardOwner, canActivateMigration, canRollbackToLegacy };
