// Pure operational policy. It does not store logs, run rate limits, or call UGS.
const structuredLogRetentionMilliseconds = 30 * 24 * 60 * 60 * 1000;
const queryMinimumIntervalMilliseconds = 5 * 1000;
const repeatSubmissionMinimumIntervalMilliseconds = 5 * 1000;
const newSubmissionWindowMilliseconds = 60 * 1000;
const maximumNewSubmissionsPerWindow = 3;

function getThirtyDayLogStorageDisposition(availableRetentionMilliseconds) {
  return Number.isSafeInteger(availableRetentionMilliseconds) &&
    availableRetentionMilliseconds >= structuredLogRetentionMilliseconds ? "Ready" :
    "ExternalThirtyDaySinkRequired";
}

function isSafeStructuredLog(input) {
  if (!input || typeof input.environmentId !== "string" || input.environmentId.length === 0 ||
      typeof input.scriptVersion !== "string" || input.scriptVersion.length === 0 ||
      typeof input.outcomeCategory !== "string" || input.outcomeCategory.length === 0 ||
      typeof input.reasonCategory !== "string" || input.reasonCategory.length === 0 ||
      !Number.isSafeInteger(input.durationBucketMilliseconds) || input.durationBucketMilliseconds < 0) return false;
  return !Object.keys(input).some(key => /player|public.?number|code|verification|token|hmac|digest|payload/i.test(key));
}

function isMinimumIntervalAllowed(lastAcceptedAtMilliseconds, nowMilliseconds, minimumIntervalMilliseconds) {
  if (!Number.isSafeInteger(nowMilliseconds) || nowMilliseconds < 0 ||
      !Number.isSafeInteger(minimumIntervalMilliseconds) || minimumIntervalMilliseconds < 0) return false;
  if (lastAcceptedAtMilliseconds === null) return true;
  return Number.isSafeInteger(lastAcceptedAtMilliseconds) && nowMilliseconds >= lastAcceptedAtMilliseconds &&
    nowMilliseconds - lastAcceptedAtMilliseconds >= minimumIntervalMilliseconds;
}

function isNewSubmissionAllowed(windowStartedAtMilliseconds, acceptedCount, nowMilliseconds) {
  if (!Number.isSafeInteger(nowMilliseconds) || nowMilliseconds < 0 ||
      !Number.isSafeInteger(acceptedCount) || acceptedCount < 0) return false;
  if (windowStartedAtMilliseconds === null) return true;
  if (!Number.isSafeInteger(windowStartedAtMilliseconds) || nowMilliseconds < windowStartedAtMilliseconds) return false;
  return nowMilliseconds - windowStartedAtMilliseconds >= newSubmissionWindowMilliseconds ||
    acceptedCount < maximumNewSubmissionsPerWindow;
}

function getRateLimitResponse(isAllowed) {
  return isAllowed ? "Allowed" : "TooManyRequests";
}

function shouldAutoRetryAfterRateLimit() { return false; }

module.exports = {
  structuredLogRetentionMilliseconds,
  queryMinimumIntervalMilliseconds,
  repeatSubmissionMinimumIntervalMilliseconds,
  newSubmissionWindowMilliseconds,
  maximumNewSubmissionsPerWindow,
  getThirtyDayLogStorageDisposition,
  isSafeStructuredLog,
  isMinimumIntervalAllowed,
  isNewSubmissionAllowed,
  getRateLimitResponse,
  shouldAutoRetryAfterRateLimit
};
