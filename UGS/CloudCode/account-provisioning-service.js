const { validateContext, validateScope, fault } = require("./account-store");
const number = require("./public-player-number-policy");
const owner = require("./leaderboard-owner-policy");
const { createAccountService } = require("./account-service");
const { sameJsonValue } = require("./json-value-policy");
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
  const scope = { schemaVersion: 1, ...store.scope };
  function initialBinding(value, player) {
    const accountId = owner.tryNormalizeGeneratedLeaderboardOwnerId(value.accountId);
    const generatedOwner = owner.tryNormalizeGeneratedLeaderboardOwnerId(value.generatedOwner);
    return value.playerId === player && value.status === undefined && !value.transferOperation && !value.inactiveByTransferId &&
      value.connectionRevision === 1 && !!accountId && accountId === value.accountId &&
      !!generatedOwner && generatedOwner === value.generatedOwner &&
      value.accountId !== value.generatedOwner && value.accountId !== player && value.generatedOwner !== player;
  }
  function prepareLedger(snapshot) {
    const ledger = clone(snapshot.ledger);
    for (const [boardId, row] of Object.entries(snapshot.scores)) {
      const best = ledger.best[boardId];
      if (best && (best.score !== row.score || best.acceptedAt !== row.metadata.acceptedAt ||
          best.id !== row.metadata.submissionId)) throw fault("LegacyRecordConflict");
      if (!best) ledger.best[boardId] = { score: row.score,
        acceptedAt: row.metadata.acceptedAt, id: row.metadata.submissionId };
    }
    if (Object.keys(ledger.best).some(boardId => !snapshot.scores[boardId])) throw fault("LegacyRecordConflict");
    return ledger;
  }
  async function restoreInitialBinding(context, binding) {
    // The server-owned frozen source, never caller-supplied IDs, selects the
    // already-created Account. A mismatch alone never authorizes reassignment.
    if (!initialBinding(binding.value, context.playerId) || typeof legacy.readFrozenLedger !== "function")
      throw fault("LedgerConflict");
    const frozen = await legacy.readFrozenLedger(context.playerId);
    const source = frozen && frozen.value, marker = source && source.migration;
    if (!frozen || !frozen.writeLock || !source || source.version !== 1 || source.active !== "" ||
        !Array.isArray(source.entries) || source.entries.some(e => !e || typeof e !== "object" ||
          !["Submitted", "Rejected"].includes(e.status)) ||
        Object.prototype.toString.call(source.best) !== "[object Object]" ||
        !marker || marker.projectId !== scope.projectId || marker.environmentId !== scope.environmentId ||
        typeof marker.hadLegacyLedger !== "boolean" || !owner.tryNormalizeGeneratedLeaderboardOwnerId(marker.accountId) ||
        owner.tryNormalizeGeneratedLeaderboardOwnerId(marker.accountId) !== marker.accountId ||
        marker.accountId === binding.value.accountId) throw fault("LedgerConflict");
    const target = await store.read("account", marker.accountId);
    const a = target && target.value;
    if (!a || a.accountId !== marker.accountId || a.accountId === context.playerId || a.status !== "Preparing" || a.connectionRevision !== 1 ||
        a.initialPlayerId !== context.playerId || a.currentPlayerId !== context.playerId || a.transfer || a.onlineOperation ||
        a.joiningTransfer || a.revokedJoiningTransfer || a.detachedByTransferId ||
        typeof a.publicPlayerNumber !== "string" || a.publicPlayerNumber !== "" && !number.tryNormalizePublicPlayerNumber(a.publicPlayerNumber) ||
        !a.migration || !sameJsonValue(a.migration.ledger, source)) throw fault("AccountConflict");
    const snapshot = await legacy.freeze(context.playerId, a.accountId);
    if (!sameJsonValue({ ledger: snapshot.ledger, scores: snapshot.scores }, a.migration) ||
        (snapshot.hasLegacyData ? a.leaderboardOwnerId !== context.playerId || binding.value.generatedOwner === a.accountId :
          !owner.tryNormalizeGeneratedLeaderboardOwnerId(a.leaderboardOwnerId) ||
          owner.tryNormalizeGeneratedLeaderboardOwnerId(a.leaderboardOwnerId) !== a.leaderboardOwnerId ||
          a.leaderboardOwnerId === a.accountId || a.leaderboardOwnerId === context.playerId))
      throw fault("LegacyRecordConflict");
    const ledger = await store.read("ledger", a.accountId), expectedLedger = prepareLedger(snapshot);
    if (!ledger && a.publicPlayerNumber || ledger && (ledger.value.accountId !== a.accountId ||
        ledger.value.version !== expectedLedger.version || ledger.value.active !== "" ||
        !sameJsonValue(ledger.value.sourceSnapshot, a.migration) ||
        !sameJsonValue(ledger.value.entries, expectedLedger.entries) || !sameJsonValue(ledger.value.best, expectedLedger.best)))
      throw fault("LedgerConflict");
    if (a.publicPlayerNumber) {
      const mapping = await store.read("number", a.publicPlayerNumber);
      if (!mapping || mapping.value.accountId !== a.accountId) throw fault("AccountConflict");
    }
    const mapping = await store.read("owner", a.leaderboardOwnerId);
    if (mapping && (mapping.value.accountId !== a.accountId || !a.publicPlayerNumber)) throw fault("AccountConflict");
    if (binding.value.generatedOwner !== a.leaderboardOwnerId &&
        await store.read("owner", binding.value.generatedOwner)) throw fault("AccountConflict");
    // Recheck durable evidence immediately before the single binding CAS.
    const currentSource = await legacy.readFrozenLedger(context.playerId);
    const currentAccount = await store.read("account", a.accountId);
    const currentBinding = await store.read("player", context.playerId);
    if (!currentSource || currentSource.writeLock !== frozen.writeLock || !sameJsonValue(currentSource.value, source) ||
        !currentAccount || currentAccount.writeLock !== target.writeLock || !sameJsonValue(currentAccount.value, a) ||
        !currentBinding || currentBinding.writeLock !== binding.writeLock || !sameJsonValue(currentBinding.value, binding.value) ||
        await store.read("account", binding.value.accountId)) throw fault("AccountConflict");
    await store.compareExchange(currentBinding, { ...currentBinding.value, accountId: a.accountId,
      generatedOwner: snapshot.hasLegacyData ? currentBinding.value.generatedOwner : a.leaderboardOwnerId });
    const restored = await store.read("player", context.playerId);
    if (!restored || restored.value.accountId !== a.accountId || !initialBinding(restored.value, context.playerId))
      throw fault("AccountConflict");
    return { binding: restored, account: currentAccount, snapshot };
  }
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
      validateContext(context, store.scope);
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
      validateScope(binding.value, store.scope);
      if (binding.value.playerId !== context.playerId) throw fault("AccountConflict");
      let accountId = binding.value.accountId;
      let account = await store.read("account", accountId);
      if (account && account.value.status !== "Preparing")
        return createAccountService(store).getPublicNumber(context);

      // Freezing the legacy ledger blocks old writes before the snapshot copy.
      // Reservations created before this barrier must finish first.
      let snapshot;
      try { snapshot = await legacy.freeze(context.playerId, accountId); }
      catch (error) {
        if (account || !error || error.reason !== "LedgerConflict") throw error;
        const recovered = await restoreInitialBinding(context, binding);
        binding = recovered.binding; account = recovered.account; snapshot = recovered.snapshot;
        accountId = binding.value.accountId;
      }
      const selectedOwner = snapshot.hasLegacyData ? context.playerId : binding.value.generatedOwner;
      if (!selectedOwner || (!snapshot.hasLegacyData &&
          !owner.tryNormalizeGeneratedLeaderboardOwnerId(selectedOwner))) throw fault("InvalidGeneratedId");
      const ledger = prepareLedger(snapshot);
      const preserved = { ledger: clone(snapshot.ledger), scores: clone(snapshot.scores) };
      account = await store.create("account", accountId, { ...scope, accountId,
        initialPlayerId: context.playerId, currentPlayerId: context.playerId, connectionRevision: 1,
        status: "Preparing", publicPlayerNumber: "", leaderboardOwnerId: selectedOwner,
        migration: preserved });
      if (account.value.accountId !== accountId || account.value.initialPlayerId !== context.playerId ||
          account.value.currentPlayerId !== context.playerId || account.value.connectionRevision !== 1 ||
          !["Preparing", "Active"].includes(account.value.status) || account.value.transfer || account.value.onlineOperation ||
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
