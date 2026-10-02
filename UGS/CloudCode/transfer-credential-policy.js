// Pure policy only. Generation, HMAC storage, locking, and service calls require server code.
// Crockford Base32. Generated values omit I, L, O and U; input aliases O->0, I/L->1.
const transferCodeSymbols = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
const transferCodeLength = 8;
const verificationValueLength = 9;
const transferLifetimeMilliseconds = 90 * 24 * 60 * 60 * 1000;
const verificationThrottleMilliseconds = 5 * 1000;
const transferCode = new RegExp(`^[${transferCodeSymbols}]{${transferCodeLength}}$`);
const verificationValue = new RegExp(`^\\d{${verificationValueLength}}$`);
const transferCodeCapacity = transferCodeSymbols.length ** transferCodeLength;

function expectedTransferCodeCollisionPairs(activeCodeCount) {
  if (!Number.isSafeInteger(activeCodeCount) || activeCodeCount < 0 ||
      activeCodeCount > transferCodeCapacity) return null;
  return activeCodeCount * (activeCodeCount - 1) / (2 * transferCodeCapacity);
}

function tryNormalizeTransferCode(value) {
  if (typeof value !== "string") return null;
  const normalized = value.toUpperCase().replace(/-/g, "").replace(/O/g, "0")
    .replace(/[IL]/g, "1");
  return transferCode.test(normalized) ? normalized : null;
}

function formatTransferCode(value) {
  const normalized = tryNormalizeTransferCode(value);
  return normalized ? normalized.match(/.{4}/g).join("-") : null;
}

function tryNormalizeVerificationValue(value) {
  return typeof value === "string" && verificationValue.test(value) ? value : null;
}

function isTransferExpired(issuedAtMilliseconds, nowMilliseconds) {
  return !Number.isSafeInteger(issuedAtMilliseconds) || !Number.isSafeInteger(nowMilliseconds) ||
    nowMilliseconds < issuedAtMilliseconds || nowMilliseconds - issuedAtMilliseconds >=
    transferLifetimeMilliseconds;
}

function isVerificationRetryAllowed(lastAttemptAtMilliseconds, nowMilliseconds) {
  if (lastAttemptAtMilliseconds === null || lastAttemptAtMilliseconds === undefined) return true;
  return Number.isSafeInteger(lastAttemptAtMilliseconds) && Number.isSafeInteger(nowMilliseconds) &&
    nowMilliseconds >= lastAttemptAtMilliseconds && nowMilliseconds - lastAttemptAtMilliseconds >=
    verificationThrottleMilliseconds;
}

function canStartTransfer({ isActiveConnection, hasPendingSubmissions, isTransferPending }) {
  return isActiveConnection === true && hasPendingSubmissions === false && isTransferPending === false;
}

function canReissueTransferCredential({ isActiveConnection, isTransferPending, isExpired }) {
  return isActiveConnection === true && isTransferPending === true && isExpired === false;
}

module.exports = {
  transferCodeSymbols,
  transferCodeLength,
  verificationValueLength,
  transferLifetimeMilliseconds,
  verificationThrottleMilliseconds,
  transferCodeCapacity,
  expectedTransferCodeCollisionPairs,
  tryNormalizeTransferCode,
  formatTransferCode,
  tryNormalizeVerificationValue,
  isTransferExpired,
  isVerificationRetryAllowed,
  canStartTransfer,
  canReissueTransferCredential
};
