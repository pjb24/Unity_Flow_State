const { validateContext, validateScope, fault, projectId, environmentId } = require("./account-store");
const number = require("./public-player-number-policy");
const owner = require("./leaderboard-owner-policy");
const { createAccountService } = require("./account-service");
const { sameJsonValue } = require("./json-value-policy");
const scope = { schemaVersion: 1, projectId, environmentId };
const clone = value => JSON.parse(JSON.stringify(value));

function generateId() {
  // Node 14 compatible. Fail closed if the remote runtime lacks crypto support.
  const bytes = require("crypto").randomBytes(16);
  bytes[6] = (bytes[6] & 15) | 64;
  bytes[8] = (bytes[8] & 63) | 128;
  const hex = bytes.toString("hex");
  return `${hex.slice(0,8)}-${hex.slice(8,12)}-${hex.slice(12,16)}-${hex.slice(16,20)}-${hex.slice(20)}`;
}

function createAccountProvisioningService(store, legacy, idGenerator = generateId) {
  async function ensureMapping(kind, id, accountId) {
    const mapping = await store.create(kind, id, { ...scope, accountId });
    if (!mapping || mapping.value.accountId !== accountId) throw fault("AccountConflict");
  }
  async function issueNumber(accountId) {
    const allocator = await store.create("allocator", "public", { ...scope, next: 1 });
    const next = allocator.value.next;
    const value = number.tryFormatIssuedNumber(next);
    if (!value) throw fault("PublicNumberExhausted");
    // Increment before assigning: ambiguous results can burn a number but can
    // never reuse it. Allocator state is never reset by guard initialization.
    await store.compareExchange(allocator, { ...allocator.value, next: next + 1 });
    await ensureMapping("number", value, accountId);
    const current = await store.read("account", accountId);
    if (!current) throw fault("AccountUnavailable");
    if (current.value.publicPlayerNumber) return current;
    await store.compareExchange(current, { ...current.value, publicPlayerNumber: value });
    return store.read("account", accountId);
  }
  return {
    async ensure(context) {
      validateContext(context);
      let binding = await store.read("player", context.playerId);
      if (!binding) {
        const accountId = owner.tryNormalizeGeneratedLeaderboardOwnerId(idGenerator());
        const generatedOwner = owner.tryNormalizeGeneratedLeaderboardOwnerId(idGenerator());
        if (!accountId || !generatedOwner || accountId === generatedOwner ||
            accountId === context.playerId || generatedOwner === context.playerId)
          throw fault("InvalidGeneratedId");
        binding = await store.create("player", context.playerId, { ...scope,
          playerId: context.playerId, accountId, connectionRevision: 1, generatedOwner });
      }
      validateScope(binding.value);
      if (binding.value.playerId !== context.playerId) throw fault("AccountConflict");
      const accountId = binding.value.accountId;
      let account = await store.read("account", accountId);
      if (account && account.value.status !== "Preparing")
        return createAccountService(store).getPublicNumber(context);

      // Freezing the legacy ledger blocks old writes before the snapshot copy.
      // Reservations created before this barrier must finish first.
      const snapshot = await legacy.freeze(context.playerId, accountId);
      const selectedOwner = snapshot.hasLegacyData ? context.playerId : binding.value.generatedOwner;
      if (!selectedOwner || (!snapshot.hasLegacyData &&
          !owner.tryNormalizeGeneratedLeaderboardOwnerId(selectedOwner))) throw fault("InvalidGeneratedId");
      const ledger = clone(snapshot.ledger);
      for (const [boardId, row] of Object.entries(snapshot.scores)) {
        const best = ledger.best[boardId];
        if (best && (best.score !== row.score || best.acceptedAt !== row.metadata.acceptedAt ||
            best.id !== row.metadata.submissionId)) throw fault("LegacyRecordConflict");
        if (!best) ledger.best[boardId] = { score: row.score,
          acceptedAt: row.metadata.acceptedAt, id: row.metadata.submissionId };
      }
      if (Object.keys(ledger.best).some(boardId => !snapshot.scores[boardId]))
        throw fault("LegacyRecordConflict");
      const preserved = { ledger: clone(snapshot.ledger), scores: clone(snapshot.scores) };
      account = await store.create("account", accountId, { ...scope, accountId,
        initialPlayerId: context.playerId, currentPlayerId: context.playerId, connectionRevision: 1,
        status: "Preparing", publicPlayerNumber: "", leaderboardOwnerId: selectedOwner,
        migration: preserved });
      if (account.value.initialPlayerId !== context.playerId ||
          account.value.leaderboardOwnerId !== selectedOwner ||
          !sameJsonValue(account.value.migration, preserved))
        throw fault("AccountConflict");
      const destination = await store.create("ledger", accountId,
        { ...scope, accountId, ...ledger, sourceSnapshot: preserved });
      if (destination.value.accountId !== accountId ||
          !sameJsonValue(destination.value.sourceSnapshot, preserved) ||
          !sameJsonValue(destination.value.entries, ledger.entries) ||
          !sameJsonValue(destination.value.best, ledger.best) || destination.value.active !== "")
        throw fault("LedgerConflict");
      if (!account.value.publicPlayerNumber) account = await issueNumber(accountId);
      await ensureMapping("owner", selectedOwner, accountId);
      await ensureMapping("number", account.value.publicPlayerNumber, accountId);
      const currentBinding = await store.read("player", context.playerId);
      if (!currentBinding || currentBinding.value.accountId !== accountId ||
          currentBinding.value.connectionRevision !== 1) throw fault("AccountConflict");
      const source = await legacy.freeze(context.playerId, accountId);
      if (!sameJsonValue({ ledger: source.ledger, scores: source.scores }, preserved))
        throw fault("LegacyRecordConflict");
      account = await store.read("account", accountId);
      if (account.value.status === "Preparing")
        await store.compareExchange(account, { ...account.value, status: "Active" });
      return createAccountService(store).getPublicNumber(context);
    }
  };
}
module.exports = { createAccountProvisioningService, generateId };
