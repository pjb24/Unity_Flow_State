// Internal server service; no client-supplied account/owner ID selects authority.
const { validateContext, validateScope, fault } = require("./account-store");
const connection = require("./active-connection-policy");
const number = require("./public-player-number-policy");

function createAccountService(store, clock = Date.now) {
  async function resolve(context, resumeSubmissionId = null) {
    validateContext(context);
    const binding = await store.read("player", context.playerId);
    if (!binding) throw fault("AccountUnavailable");
    validateScope(binding.value);
    const account = await store.read("account", binding.value.accountId);
    if (!account) throw fault("AccountUnavailable");
    validateScope(account.value);
    if (binding.value.status === "Inactive" || binding.value.transferOperation &&
        (!resumeSubmissionId || !account.value.onlineOperation || account.value.onlineOperation.id !== resumeSubmissionId))
      throw fault("ActiveDeviceRequired");
    if (account.value.accountId !== binding.value.accountId ||
        !Number.isSafeInteger(account.value.connectionRevision) || account.value.connectionRevision < 1)
      throw fault("AccountConflict");
    const reason = connection.authorizeActiveConnection(account.value, binding.value, context.playerId);
    if (reason !== "Authorized") throw fault(reason);
    return { account, binding };
  }
  return {
    resolve,
    async getPublicNumber(context) {
      const { account } = await resolve(context);
      const value = account.value.publicPlayerNumber;
      if (!number.tryNormalizePublicPlayerNumber(value)) throw fault("PublicNumberUnavailable");
      const mapping = await store.read("number", value);
      if (!mapping || mapping.value.accountId !== account.value.accountId)
        throw fault("AccountConflict");
      validateScope(mapping.value);
      const ownerBinding = await store.read("owner", account.value.leaderboardOwnerId);
      if (!ownerBinding || ownerBinding.value.accountId !== account.value.accountId)
        throw fault("AccountConflict");
      validateScope(ownerBinding.value);
      // Re-read authority after the mapping fetch; a late response may be stale.
      const current = await resolve(context);
      if (current.account.value.accountId !== account.value.accountId ||
          current.account.value.connectionRevision !== account.value.connectionRevision ||
          current.account.value.publicPlayerNumber !== value) throw fault("AccountConflict");
      return { status: "Success", publicPlayerNumber: value };
    },
    async reserveSubmission(context, submissionId) {
      if (typeof submissionId !== "string" ||
          !/^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(submissionId))
        throw fault("InvalidSubmissionId");
      const { account, binding } = await resolve(context, submissionId);
      const operation = { id: submissionId.toLowerCase(), playerId: context.playerId,
        connectionRevision: account.value.connectionRevision };
      const disposition = connection.reserveMutation(account.value, binding.value, context.playerId, operation);
      if (disposition === "Resume") return { disposition, operation };
      if (disposition !== "Reserved") throw fault("AccountBusy");
      const reservedAt = clock();
      if (!Number.isSafeInteger(reservedAt) || reservedAt < 0) throw fault("InvalidServerTime");
      await store.compareExchange(account, { ...account.value, onlineOperation: { ...operation, reservedAt } });
      return { disposition, operation };
    },
    async releaseSubmission(context, submissionId) {
      const { account } = await resolve(context, submissionId);
      const operation = account.value.onlineOperation;
      if (!operation) return "AlreadyReleased";
      if (operation.id !== submissionId || operation.playerId !== context.playerId ||
          operation.connectionRevision !== account.value.connectionRevision) throw fault("AccountBusy");
      const ledger = await store.read("ledger", account.value.accountId);
      if (!ledger) throw fault("LedgerUnavailable");
      validateScope(ledger.value);
      if (ledger.value.accountId !== account.value.accountId || ledger.value.active !== "" ||
          !Array.isArray(ledger.value.entries)) throw fault("LedgerConflict");
      const terminal = ledger.value.entries.find(entry => entry.id === operation.id);
      if (!terminal || !["Submitted", "Rejected"].includes(terminal.status)) throw fault("SubmissionNotTerminal");
      await store.compareExchange(account, { ...account.value, onlineOperation: null });
      return "Released";
    },
    async assertTransferCanStart(context) {
      const { account, binding } = await resolve(context);
      const ledger = await store.read("ledger", account.value.accountId);
      if (!ledger) throw fault("LedgerUnavailable");
      validateScope(ledger.value);
      if (ledger.value.accountId !== account.value.accountId || typeof ledger.value.active !== "string" ||
          !Array.isArray(ledger.value.entries))
        throw fault("LedgerConflict");
      const hasPending = !!ledger.value.active || ledger.value.entries.some(entry => entry.status === "Pending");
      if (!connection.canBeginTransfer(account.value, binding.value, context.playerId, hasPending))
        throw fault("AccountBusy");
      // Step 3 must CAS this exact Account token into TransferPending. A precheck
      // alone never grants a transfer lock, nor releases a submission reservation.
      return account;
    }
  };
}

module.exports = { createAccountService };
