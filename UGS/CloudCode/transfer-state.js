// Shared source authority and persisted Pending validation; no Client authority.
const { validateContext, validateScope, fault } = require("./account-store");
const policy = require("./transfer-credential-policy");
const ownerPolicy = require("./leaderboard-owner-policy");
async function readSourceAccount(store, context) {
  validateContext(context);
  const binding = await store.read("player", context.playerId);
  if (!binding) throw fault("AccountUnavailable");
  validateScope(binding.value);
  if (binding.value.transferOperation || binding.value.status === "Inactive") throw fault("ActiveDeviceRequired");
  const account = await store.read("account", binding.value.accountId);
  if (!account) throw fault("AccountUnavailable");
  validateScope(account.value);
  if (binding.value.playerId !== context.playerId || account.value.accountId !== binding.value.accountId ||
      account.value.currentPlayerId !== context.playerId ||
      account.value.connectionRevision !== binding.value.connectionRevision ||
      !Number.isSafeInteger(account.value.connectionRevision) || account.value.connectionRevision < 1)
    throw fault("ActiveDeviceRequired");
  if (!["Active", "TransferPending"].includes(account.value.status)) throw fault("AccountUnavailable");
  return account;
}
function validatePendingTransfer(account, now, allowExpired = false) {
  const t = account.value.transfer;
  if (account.value.status !== "TransferPending" || !t || t.status !== "TransferPending" ||
      t.sourcePlayerId !== account.value.currentPlayerId || t.connectionRevision !== account.value.connectionRevision ||
      !Number.isSafeInteger(t.credentialRevision) || t.credentialRevision < 1 ||
      !ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(t.transferId) ||
      typeof t.codeDigest !== "string" || !/^[A-Za-z0-9_-]{43}$/.test(t.codeDigest) ||
      typeof t.credentialHmac !== "string" || !/^[a-f0-9]{64}$/.test(t.credentialHmac) ||
      !Number.isSafeInteger(t.issuedAtMilliseconds) || t.issuedAtMilliseconds < 0 ||
      !Number.isSafeInteger(t.expiresAtMilliseconds) ||
      t.expiresAtMilliseconds !== t.issuedAtMilliseconds + policy.transferLifetimeMilliseconds ||
      typeof t.credentialActive !== "boolean" ||
      (t.lastAttemptAtMilliseconds !== null && (!Number.isSafeInteger(t.lastAttemptAtMilliseconds) ||
        t.lastAttemptAtMilliseconds < t.issuedAtMilliseconds)) ||
      !Number.isSafeInteger(t.failureCount) || t.failureCount < 0) throw fault("TransferUnavailable");
  if (!allowExpired && policy.isTransferExpired(t.issuedAtMilliseconds, now)) throw fault("TransferExpired");
  if (!Number.isSafeInteger(now) || now < t.issuedAtMilliseconds) throw fault("InvalidServerTime");
  return t;
}
module.exports = { readSourceAccount, validatePendingTransfer };
