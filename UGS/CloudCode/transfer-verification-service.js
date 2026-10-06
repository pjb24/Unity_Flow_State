// Internal Step 4 dependency, not a standalone credential-oracle endpoint.
// A verified snapshot is not authorization: completion MUST CAS this exact
// Account token and recheck expiry/connection/credential at the commit boundary.
const crypto = require("crypto");
const { validateContext, validateScope, fault } = require("./account-store");
const { createTransferCryptography } = require("./transfer-cryptography");
const policy = require("./transfer-credential-policy");
function createTransferVerificationService(store, secretProvider, clock = Date.now) {
  function current(account, lookup, digest, now) {
    if (!account) throw fault("InvalidTransferCredential");
    validateScope(account.value);
    const t = account.value.transfer;
    if (account.value.status !== "TransferPending" || !t || t.status !== "TransferPending" ||
        t.credentialActive !== true || t.codeDigest !== digest ||
        t.transferId !== lookup.value.transferId || t.credentialRevision !== lookup.value.credentialRevision ||
        account.value.accountId !== lookup.value.accountId ||
        t.sourcePlayerId !== account.value.currentPlayerId ||
        t.connectionRevision !== account.value.connectionRevision ||
        !Number.isSafeInteger(t.connectionRevision) || t.connectionRevision < 1 ||
        !Number.isSafeInteger(t.credentialRevision) || t.credentialRevision < 1 ||
        typeof t.credentialHmac !== "string" || !/^[a-f0-9]{64}$/.test(t.credentialHmac) ||
        t.expiresAtMilliseconds !== t.issuedAtMilliseconds + policy.transferLifetimeMilliseconds ||
        policy.isTransferExpired(t.issuedAtMilliseconds, now)) throw fault("InvalidTransferCredential");
    if ((t.lastAttemptAtMilliseconds !== null &&
        (!Number.isSafeInteger(t.lastAttemptAtMilliseconds) || t.lastAttemptAtMilliseconds < t.issuedAtMilliseconds)) ||
        !Number.isSafeInteger(t.failureCount) || t.failureCount < 0) throw fault("TransferUnavailable");
    return t;
  }
  return {
    async verify(context, code, verificationValue) {
      validateContext(context);
      const normalized = policy.tryNormalizeTransferCode(code);
      if (!normalized) return { status: "InvalidCredential" };
      const cryptography = createTransferCryptography(await secretProvider());
      const digest = cryptography.codeDigest(normalized);
      const lookup = await store.read("t", digest);
      if (!lookup) return { status: "InvalidCredential" };
      validateScope(lookup.value);
      const account = await store.read("account", lookup.value.accountId);
      const now = clock();
      if (!Number.isSafeInteger(now) || now < 0) throw fault("InvalidServerTime");
      let t;
      try { t = current(account, lookup, digest, now); }
      catch (error) {
        if (error.reason === "InvalidTransferCredential") return { status: "InvalidCredential" };
        throw error;
      }
      if (!policy.isVerificationRetryAllowed(t.lastAttemptAtMilliseconds, now))
        return { status: "TooManyRequests", retryAtMilliseconds:
          t.lastAttemptAtMilliseconds + policy.verificationThrottleMilliseconds };
      const input = policy.tryNormalizeVerificationValue(verificationValue);
      const matches = input !== null && crypto.timingSafeEqual(Buffer.from(t.credentialHmac, "hex"),
        Buffer.from(cryptography.credentialHmac(t.transferId, t.credentialRevision, normalized, input), "hex"));
      // The attempt, its outcome and throttle share Account's CAS with reissue
      // and future completion. Never retry a stale comparison after conflict.
      await store.compareExchange(account, { ...account.value, transfer: { ...t,
        lastAttemptAtMilliseconds: now,
        failureCount: matches ? t.failureCount : Math.min(Number.MAX_SAFE_INTEGER, t.failureCount + 1) } });
      const confirmed = await store.read("account", lookup.value.accountId);
      const confirmedTime = clock();
      if (!Number.isSafeInteger(confirmedTime) || confirmedTime < now) throw fault("InvalidServerTime");
      let confirmedTransfer;
      try { confirmedTransfer = current(confirmed, lookup, digest, confirmedTime); }
      catch (error) {
        if (error.reason === "InvalidTransferCredential") return { status: "InvalidCredential" };
        throw error;
      }
      if (confirmedTransfer.lastAttemptAtMilliseconds !== now) throw fault("TransferUnavailable");
      return matches ? { status: "Verified", account: confirmed } : { status: "InvalidCredential" };
    }
  };
}
module.exports = { createTransferVerificationService };
