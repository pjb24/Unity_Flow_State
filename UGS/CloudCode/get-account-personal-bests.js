// Authenticated whole-account snapshot, not a leaderboard UI query. No writes.
const { createAccountStore, validateContext, validateScope, fault } = require("./account-store");
const { createAccountService } = require("./account-service");
const stage = "fs-stage-stage-001-r1", infinite = "fs-infinite-v2";
const uuid = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
function createPersonalBestSnapshotService(store) {
  const accounts = createAccountService(store);
  return async context => {
    try {
      validateContext(context);
      const initial = (await accounts.resolve(context)).account.value;
      const number = await accounts.getPublicNumber(context);
      const ledger = await store.read("ledger", initial.accountId);
      if (!ledger) throw fault("LedgerUnavailable");
      validateScope(ledger.value);
      const value = ledger.value;
      const pending = value.version === 1 ? Array.isArray(value.entries) && value.entries.some(e => e.status === "Pending") :
        value.version === 2 && value.pending !== null;
      if (value.accountId !== initial.accountId || ![1, 2].includes(value.version) || value.active !== "" || pending ||
          initial.onlineOperation || !value.best || Array.isArray(value.best) || typeof value.best !== "object")
        throw fault("LedgerConflict");
      const personalBests = [];
      for (const [boardId, best] of Object.entries(value.best)) {
        if (![stage, infinite].includes(boardId) || !best || !Number.isSafeInteger(best.score) || best.score < 0 ||
            (boardId === infinite && best.score > 2147483647) || !uuid.test(best.id) ||
            !Number.isSafeInteger(best.acceptedAt) || best.acceptedAt <= 0) throw fault("InvalidBest");
        personalBests.push({ boardId, score: best.score, submissionId: best.id });
      }
      const current = (await accounts.resolve(context)).account.value;
      const latest = await store.read("ledger", initial.accountId);
      if (!latest || latest.writeLock !== ledger.writeLock || current.accountId !== initial.accountId ||
          current.connectionRevision !== initial.connectionRevision || current.onlineOperation ||
          current.publicPlayerNumber !== number.publicPlayerNumber ||
          current.leaderboardOwnerId !== initial.leaderboardOwnerId) throw fault("StaleSnapshot");
      // Authority is rechecked after the final ledger read too.
      const final = (await accounts.resolve(context)).account.value;
      if (final.accountId !== initial.accountId || final.connectionRevision !== initial.connectionRevision ||
          final.onlineOperation) throw fault("StaleSnapshot");
      return { status: "Success", publicPlayerNumber: number.publicPlayerNumber, personalBests };
    } catch (_) { return { status: "TransientFailure", reason: "PersonalBestUnavailable" }; }
  };
}
module.exports = async ({ context }) => {
  try { return await createPersonalBestSnapshotService(createAccountStore(context))(context); }
  catch (_) { return { status: "TransientFailure", reason: "PersonalBestUnavailable" }; }
};
module.exports.createPersonalBestSnapshotService = createPersonalBestSnapshotService;
module.exports.params = {};
module.exports.bundling = true;
