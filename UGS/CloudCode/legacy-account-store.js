// Cutover barrier in the SAME Protected item used by the old submit path.
const { projectId, environmentId, validateContext, fault, guardKey } = require("./account-store");
const ledgerKey = "fs_submission_ledger_v1";
const boards = ["fs-stage-stage-001-r1", "fs-infinite-v2"];
function createLegacyAccountStore(context, save, leaderboard) {
  validateContext(context);
  if (!save) {
    const { DataApi } = require("@unity-services/cloud-save-1.4");
    save = new DataApi(context);
  }
  if (!leaderboard) {
    const { LeaderboardsApi } = require("@unity-services/leaderboards-1.1");
    leaderboard = new LeaderboardsApi(context);
  }
  async function read(playerId, requestedKey) {
    const response = await save.getProtectedItems(projectId, playerId, [requestedKey]);
    return response.data.results.find(item => item.key === requestedKey);
  }
  return {
    async freeze(playerId, accountId) {
      let item = await read(playerId, ledgerKey);
      if (!item) {
        let guard = await read(playerId, guardKey);
        if (!guard) await save.setProtectedItem(projectId, playerId,
          { key: guardKey, value: { projectId, environmentId, schemaVersion: 1 } });
        guard = await read(playerId, guardKey);
        if (!guard || !guard.writeLock || guard.value.projectId !== projectId ||
            guard.value.environmentId !== environmentId) throw fault("LedgerConflict");
        item = await read(playerId, ledgerKey);
        if (!item) {
          await save.setProtectedItemBatch(projectId, playerId, { data: [
            { key: guardKey, value: guard.value, writeLock: guard.writeLock },
            { key: ledgerKey, value: { version: 1, active: "", entries: [], best: {},
              migration: { accountId, projectId, environmentId, hadLegacyLedger: false } } }
          ] });
          item = await read(playerId, ledgerKey);
        }
      }
      const ledger = item.value;
      if (!item.writeLock || !ledger || ledger.version !== 1 ||
          !Array.isArray(ledger.entries) || !ledger.best || typeof ledger.active !== "string")
        throw fault("LedgerConflict");
      if (ledger.active || ledger.entries.some(entry => entry.status === "Pending"))
        throw fault("EarlierSubmissionPending");
      if (ledger.migration) {
        if (ledger.migration.accountId !== accountId || ledger.migration.projectId !== projectId ||
            ledger.migration.environmentId !== environmentId) throw fault("LedgerConflict");
      } else {
        await save.setProtectedItem(projectId, playerId, { key: ledgerKey,
          writeLock: item.writeLock, value: { ...ledger,
            migration: { accountId, projectId, environmentId, hadLegacyLedger: true } } });
        item = await read(playerId, ledgerKey);
      }
      const scores = {};
      for (const boardId of boards) {
        try {
          const response = await leaderboard.getLeaderboardPlayerScore(projectId, boardId, playerId,
            { params: { includeMetadata: true } });
          const row = response.data;
          if (row.playerId !== playerId || !Number.isSafeInteger(row.score) || row.score < 0 ||
              !row.metadata || !Number.isSafeInteger(row.metadata.acceptedAt) || row.metadata.acceptedAt <= 0 ||
              typeof row.metadata.submissionId !== "string" || !row.metadata.submissionId)
            throw fault("MissingServerMetadata");
          // Rank can change when OTHER accounts submit. It is not source-owned
          // metadata and must not make a preserved cutover snapshot unstable.
          scores[boardId] = { playerId: row.playerId, score: row.score, metadata: row.metadata };
        } catch (error) {
          // Only a confirmed absent score is absence; network/permission errors
          // must not silently turn an existing account into a new account.
          if (!error.response || error.response.status !== 404) throw error;
          // A missing BOARD also returns 404. Confirm the board exists rather
          // than discarding legacy data because of an environment/config error.
          await leaderboard.getLeaderboardScores(projectId, boardId, 0, 1);
        }
      }
      return { ledger: item.value, scores,
        hasLegacyData: item.value.migration.hadLegacyLedger || Object.keys(scores).length > 0 };
    }
  };
}
module.exports = { createLegacyAccountStore, ledgerKey, boards };
