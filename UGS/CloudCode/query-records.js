// verification only; bundle dependencies. Never expose storage/UGS identities.
const { createAccountStore, validateContext, fault } = require("./account-store");
const { createAccountService } = require("./account-service");
const { tryNormalizePublicPlayerNumber } = require("./public-player-number-policy");
const project = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environment = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const stage = "fs-stage-stage-001-r1";
const infinite = "fs-infinite-v2";
const fail = reason => ({ status: "TransientFailure", reason, entries: [] });
function unavailable(error, queryPhase) {
  const status = error && error.response && error.response.status;
  const allowed = ["ContextMismatch", "InvalidStorageKey", "StoredScopeMismatch", "InvalidStorageResponse",
    "WriteLockUnavailable", "AccountConflict", "AccountUnavailable", "PublicNumberUnavailable",
    "PublicNumberExhausted", "LedgerConflict", "LegacyRecordConflict", "EarlierSubmissionPending",
    "InvalidGeneratedId", "AccountInactive", "ActiveDeviceRequired", "AccountBusy", "LedgerUnavailable", "MissingServerMetadata"];
  const queryFault = error && allowed.includes(error.reason) ? error.reason : "Unknown";
  return { ...fail("ServiceUnavailable"), queryPhase,
    queryFault, serviceStatus: Number.isInteger(status) && status >= 100 && status <= 599 ? status : 0 };
}

function createRecordQueryService(store, boards, provisionLegacyRow) {
  const accounts = createAccountService(store);
  return async ({ params, context }) => {
    try { validateContext(context); } catch (_) { return fail("ContextMismatch"); }
    let p;
    try { p = JSON.parse(params.request); } catch (_) { return fail("InvalidRequest"); }
    if (!p || ![stage, infinite].includes(p.boardId) || !["top", "around", "me"].includes(p.kind) ||
        !Number.isInteger(p.limit) || p.limit < 1 || p.limit > 20) return fail("InvalidQuery");
    let queryPhase = "ResolveAccount";
    try {
      const initial = (await accounts.resolve(context)).account.value;
      queryPhase = "ReadPublicNumber";
      await accounts.getPublicNumber(context);
      queryPhase = "ReadLeaderboard";
      const response = await boards.getLeaderboardScores(project, p.boardId, 0, 100,
        { params: { includeMetadata: true } });
      const data = response.data;
      if (!Number.isInteger(data.total) || data.total < 0 || data.total > 100 ||
          !Array.isArray(data.results) || data.results.length !== data.total)
        return fail("VerificationBoardCapacity");
      queryPhase = "ResolvePublicRows";
      const entries = [];
      const seenOwners = new Set(); const seenNumbers = new Set();
      for (const row of data.results) {
        if (typeof row.playerId !== "string" || !row.playerId || seenOwners.has(row.playerId) ||
            !Number.isSafeInteger(row.score) || row.score < 0 || !row.metadata ||
            !Number.isSafeInteger(row.metadata.acceptedAt) || row.metadata.acceptedAt <= 0)
          return fail("MissingServerMetadata");
        seenOwners.add(row.playerId);
        queryPhase = "ReadOwnerMapping";
        let mapping = await store.read("owner", row.playerId);
        if (!mapping) {
          if (!provisionLegacyRow) throw fault("PublicNumberUnavailable");
          // Only a service-fetched row selects legacy provisioning. This ID
          // never comes from params and never grants the caller that authority.
          queryPhase = "ProvisionLegacyRow";
          await provisionLegacyRow(row.playerId);
          queryPhase = "ReadProvisionedOwner";
          mapping = await store.read("owner", row.playerId);
        }
        if (!mapping) throw fault("PublicNumberUnavailable");
        queryPhase = "ReadRowAccount";
        let account = await store.read("account", mapping.value.accountId);
        if (account && account.value.status === "Preparing" && provisionLegacyRow &&
            account.value.accountId === mapping.value.accountId &&
            account.value.initialPlayerId === row.playerId && account.value.leaderboardOwnerId === row.playerId) {
          // Owner mapping can commit before activation. Resume ONLY this server-fetched legacy owner.
          const expectedAccountId = mapping.value.accountId;
          queryPhase = "ResumeLegacyRow";
          await provisionLegacyRow(row.playerId);
          queryPhase = "ReadProvisionedOwner";
          mapping = await store.read("owner", row.playerId);
          if (!mapping || mapping.value.accountId !== expectedAccountId) throw fault("AccountConflict");
          queryPhase = "ReadRowAccount";
          account = await store.read("account", expectedAccountId);
        }
        queryPhase = "ValidateRowAccount";
        if (!account || account.value.accountId !== mapping.value.accountId ||
            account.value.leaderboardOwnerId !== row.playerId || account.value.status === "Preparing")
          throw fault("AccountConflict");
        queryPhase = "ValidatePublicNumber";
        const number = account.value.publicPlayerNumber;
        if (!tryNormalizePublicPlayerNumber(number) || seenNumbers.has(number)) throw fault("PublicNumberUnavailable");
        queryPhase = "ReadNumberMapping";
        const numberMapping = await store.read("number", number);
        queryPhase = "ValidateNumberMapping";
        if (!numberMapping || numberMapping.value.accountId !== account.value.accountId) throw fault("AccountConflict");
        seenNumbers.add(number);
        entries.push({ publicPlayerNumber: number, isMe: account.value.accountId === initial.accountId,
          score: row.score, acceptedAt: row.metadata.acceptedAt, rank: 0 });
      }
      queryPhase = "SortPublicRows";
      entries.sort((a, b) => (p.boardId === stage ? a.score - b.score : b.score - a.score) ||
        a.acceptedAt - b.acceptedAt || (a.publicPlayerNumber < b.publicPlayerNumber ? -1 :
          a.publicPlayerNumber > b.publicPlayerNumber ? 1 : 0));
      for (let i = 0; i < entries.length; i++)
        entries[i].rank = i > 0 && entries[i].score === entries[i - 1].score ? entries[i - 1].rank : i + 1;
      const me = entries.findIndex(e => e.isMe);
      let selected;
      if (p.kind === "top") selected = entries.slice(0, p.limit);
      else if (p.kind === "me") selected = me < 0 ? [] : [entries[me]];
      else {
        const start = Math.max(0, Math.min(me - Math.floor(p.limit / 2), entries.length - p.limit));
        selected = me < 0 ? [] : entries.slice(start, start + p.limit);
      }
      queryPhase = "RecheckAccount";
      const current = (await accounts.resolve(context)).account.value;
      if (current.accountId !== initial.accountId || current.connectionRevision !== initial.connectionRevision ||
          current.publicPlayerNumber !== initial.publicPlayerNumber || current.leaderboardOwnerId !== initial.leaderboardOwnerId)
        throw fault("AccountConflict");
      return { status: "Success", reason: "", entries: selected };
    } catch (error) { return unavailable(error, queryPhase); }
  };
}
module.exports = async ({ params, context }) => {
  try { validateContext(context); } catch (_) { return fail("ContextMismatch"); }
  try {
    const { LeaderboardsApi } = require("@unity-services/leaderboards-1.1");
    const { createLegacyAccountStore } = require("./legacy-account-store");
    const { createAccountProvisioningService } = require("./account-provisioning-service");
    const store = createAccountStore(context);
    const provision = createAccountProvisioningService(store, createLegacyAccountStore(context));
    return await createRecordQueryService(store, new LeaderboardsApi(context),
      ownerId => provision.ensure({ ...context, playerId: ownerId }))({ params, context });
  } catch (error) { return unavailable(error, "EndpointSetup"); }
};
module.exports.createRecordQueryService = createRecordQueryService;
module.exports.params = { request: { type: "String", required: true } };
module.exports.bundling = true;
