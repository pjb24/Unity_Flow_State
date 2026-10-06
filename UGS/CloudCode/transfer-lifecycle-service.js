const { fault, validateContext, validateScope } = require("./account-store");
const { readSourceAccount, validatePendingTransfer } = require("./transfer-state");
const resolution = require("./transfer-resolution-policy");
const ownerPolicy = require("./leaderboard-owner-policy");
const credentialPolicy = require("./transfer-credential-policy");
function createTransferLifecycleService(store, clock = Date.now) {
  function time() {
    const now = clock();
    if (!Number.isSafeInteger(now) || now < 0) throw fault("InvalidServerTime");
    return now;
  }
  function reply(account) {
    const t = account.value.transfer;
    if (!t) {
      if (account.value.status !== "Active") throw fault("TransferUnavailable");
      return { status: "Active", transferStatus: "None" };
    }
    if (account.value.status === "TransferPending") {
      validatePendingTransfer(account, time());
      return { status: "TransferPending", transferStatus: "TransferPending", transferId: t.transferId,
        expiresAtMilliseconds: t.expiresAtMilliseconds, credentialReissueRequired: true };
    }
    if (account.value.status === "Active" && t.status === "Completed" && t.credentialActive === false &&
        t.completedPlayerId === account.value.currentPlayerId &&
        t.completedConnectionRevision === account.value.connectionRevision &&
        t.completedConnectionRevision === t.connectionRevision + 1 &&
        ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(t.transferId))
      return { status: "Active", transferStatus: "Completed", transferId: t.transferId };
    // Never interpret an unknown terminal state as restored source authority.
    if (account.value.status !== "Active" || !["Cancelled", "Expired"].includes(t.status) ||
        !ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(t.transferId) || t.credentialActive !== false ||
        t.sourcePlayerId !== account.value.currentPlayerId || t.connectionRevision !== account.value.connectionRevision ||
        !Number.isSafeInteger(t.terminalAtMilliseconds) || t.terminalAtMilliseconds < t.issuedAtMilliseconds ||
        !Number.isSafeInteger(t.issuedAtMilliseconds) || !Number.isSafeInteger(t.expiresAtMilliseconds) ||
        t.expiresAtMilliseconds !== t.issuedAtMilliseconds + credentialPolicy.transferLifetimeMilliseconds ||
        (t.status === "Expired" && t.terminalAtMilliseconds < t.expiresAtMilliseconds) ||
        !Number.isSafeInteger(t.credentialRevision) || t.credentialRevision < 1 ||
        !Number.isSafeInteger(t.failureCount) || t.failureCount < 0 ||
        t.codeDigest !== undefined || t.credentialHmac !== undefined) throw fault("TransferUnavailable");
    if (time() < t.terminalAtMilliseconds) throw fault("InvalidServerTime");
    return { status: "Active", transferStatus: t.status, transferId: t.transferId };
  }
  async function transition(context, expectedTransferId, kind) {
    time();
    let account = await readSourceAccount(store, context);
    if (expectedTransferId !== undefined && (!ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(expectedTransferId) ||
        !account.value.transfer || account.value.transfer.transferId !== expectedTransferId))
      throw fault("TransferUnavailable");
    if (account.value.status !== "TransferPending") return reply(account);
    const t = validatePendingTransfer(account, time(), true);
    const now = time();
    if (now < t.issuedAtMilliseconds) throw fault("InvalidServerTime");
    if (kind === "expire" && now < t.expiresAtMilliseconds) return reply(account);
    const result = resolution.resolveTransfer(t, { kind, playerId: context.playerId, nowMilliseconds: now });
    if (!["Cancelled", "Expired"].includes(result.outcome)) throw fault("TransferUnavailable");
    const terminal = { ...result.transfer, terminalAtMilliseconds: now };
    delete terminal.codeDigest;
    delete terminal.credentialHmac;
    // One CAS restores A and consumes the credential. Lookup tombstones do not
    // need a second write to revoke authority; stale lookup is never authority.
    await store.compareExchange(account, { ...account.value, status: "Active", transfer: terminal });
    account = await readSourceAccount(store, context);
    if (!account.value.transfer || account.value.transfer.transferId !== t.transferId ||
        account.value.transfer.status !== terminal.status) throw fault("TransferUnavailable");
    return reply(account);
  }
  async function readStatus(context, observedBinding) {
    // Lazy server expiry, no timer/Unity/Secret needed. No raw reconstruction
    // or credential activation on status reads, including inactive issuance.
    validateContext(context);
    const binding = observedBinding === undefined ? await store.read("player", context.playerId) : observedBinding;

    if (binding && !binding.value.transferOperation) {
      if (binding.value.status === "Inactive" && binding.value.inactiveByTransferId) {
        const receipt = await store.read("receipt", binding.value.inactiveByTransferId);
        if (receipt && receipt.value.sourcePlayerId === context.playerId && receipt.value.accountId === binding.value.accountId &&
            receipt.value.transferId === binding.value.inactiveByTransferId)
          return { status: "Inactive", transferStatus: "Completed", transferId: receipt.value.transferId };
      }
      const account = await store.read("account", binding.value.accountId);
      const t = account && account.value.transfer;
      if (t && t.status === "Completed" && t.credentialActive === false &&
          t.sourcePlayerId === context.playerId && t.connectionRevision === binding.value.connectionRevision &&
          binding.value.playerId === context.playerId && account.value.accountId === binding.value.accountId &&
          account.value.currentPlayerId === t.completedPlayerId &&
          account.value.currentPlayerId !== context.playerId &&
          account.value.connectionRevision === t.completedConnectionRevision &&
          t.completedConnectionRevision === t.connectionRevision + 1 &&
          ["Active", "TransferCompleting"].includes(account.value.status) &&
          ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(t.transferId))
        return { status: "Inactive", transferStatus: "Completed", transferId: t.transferId };
    }
    return transition(context, undefined, "expire");
  }
  return {
    async cancel(context, transferId) {
      // Required generation fence prevents a delayed old Cancel click from
      // cancelling a later transfer. The ID is correlation, never authority.
      if (!ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(transferId)) throw fault("TransferUnavailable");
      return transition(context, transferId, "cancel");
    },
    async expire(context, transferId) { return transition(context, transferId, "expire"); },
    async status(context) { return readStatus(context); },
    async inactiveStatus(context, observedBinding) {
      validateContext(context);
      const binding = observedBinding;
      if (!binding || binding.kind !== "player" || binding.id !== context.playerId ||
          binding.value.playerId !== context.playerId || binding.value.status !== "Inactive" ||
          binding.value.transferOperation) throw fault("AccountConflict");
      validateScope(binding.value);
      return readStatus(context, binding);
    }
  };
}
module.exports = { createTransferLifecycleService };
