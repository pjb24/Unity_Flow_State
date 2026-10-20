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

function isDistinctEnvironmentPair(verification, production) {
  return isNonEmptyScope(verification) && isNonEmptyScope(production) &&
    (verification.projectId !== production.projectId || verification.environmentId !== production.environmentId);
}

function isFixedBuildScope(buildScope, requestedScope) {
  return isNonEmptyScope(buildScope) && (requestedScope === null ||
    (isNonEmptyScope(requestedScope) && buildScope.projectId === requestedScope.projectId &&
      buildScope.environmentId === requestedScope.environmentId));
}

function hasIsolatedOnlineLocalScope(savedScope, buildScope) {
  return isFixedBuildScope(buildScope, savedScope);
}

function isSingleCSharpModuleManifest(manifest) {
  return !!manifest && Object.keys(manifest).length === 1 && typeof manifest.modulePath === "string" &&
    /^\.\.\/\.\.\/UGS\/Modules\/FlowStateVerification\/FlowStateVerification\.sln$/.test(manifest.modulePath);
}

function hasSafePlayerWritePolicy(policy) {
  if (!policy || !Array.isArray(policy.statements)) return false;
  const expected = new Map([
    ["DenyPlayerLeaderboardWrites", "urn:ugs:leaderboards:/**"],
    ["DenyPlayerCloudSaveWrites", "urn:ugs:cloud-save:/**"]
  ]);
  const sids = new Set();
  for (const statement of policy.statements) {
    if (!statement || typeof statement.Sid !== "string" || sids.has(statement.Sid)) return false;
    sids.add(statement.Sid);
    if (statement.Principal === "Player" && statement.Effect === "Allow") return false;
  }
  return [...expected].every(([sid, resource]) => policy.statements.some(statement =>
    statement.Sid === sid && statement.Principal === "Player" && statement.Effect === "Deny" &&
    statement.Resource === resource && Array.isArray(statement.Action) && statement.Action.length === 1 &&
    statement.Action[0] === "Write"));
}

const phase4LogFields = new Set([
  "environment", "moduleReference", "outcome", "reason", "durationBucket", "correlationId", "rateLimitResult"
]);

function isSafePhase4StructuredLog(input) {
  if (!input || typeof input !== "object" || Object.keys(input).some(key => !phase4LogFields.has(key))) return false;
  return typeof input.environment === "string" && input.environment.length > 0 &&
    typeof input.moduleReference === "string" && input.moduleReference.length > 0 &&
    typeof input.outcome === "string" && input.outcome.length > 0 &&
    typeof input.reason === "string" && input.reason.length > 0 &&
    Number.isSafeInteger(input.durationBucket) && input.durationBucket >= 0 &&
    typeof input.correlationId === "string" && input.correlationId.length > 0 &&
    typeof input.rateLimitResult === "string" && input.rateLimitResult.length > 0;
}

function getProductionDeployGate(input) {
  if (!input || !isDistinctEnvironmentPair(input.verification, input.production)) return "BlockedEnvironment";
  for (const requirement of ["inventoryCaptured", "rollbackReferenceCaptured", "accessPolicyReviewed",
    "secretAccessReviewed", "logRetentionReady", "logDeletionReady", "compatibilityApproved", "verificationRehearsed"])
    if (input[requirement] !== true) return "Blocked";
  return "ReadyForApproval";
}

function getPhase4RollbackDisposition(input) {
  if (!input || !isNonEmptyScope(input.target) || !isNonEmptyScope(input.previous) ||
      input.target.projectId !== input.previous.projectId || input.target.environmentId !== input.previous.environmentId ||
      typeof input.previous.moduleReference !== "string" || input.previous.moduleReference.length === 0 ||
      input.deletesData === true || input.copiesEnvironmentData === true) return "Blocked";
  return "RepublishPreviousModule";
}

function isBackwardCompatibleContract(contract) {
  return !!contract && contract.requestBackwardCompatible === true && contract.responseBackwardCompatible === true &&
    !(contract.hasLocalSaveMigrationMarker === true && contract.reintroducesLegacyWriter === true);
}

function isNonEmptyScope(scope) {
  return !!scope && typeof scope.projectId === "string" && scope.projectId.length > 0 &&
    typeof scope.environmentId === "string" && scope.environmentId.length > 0;
}

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
  shouldAutoRetryAfterRateLimit,
  isDistinctEnvironmentPair,
  isFixedBuildScope,
  hasIsolatedOnlineLocalScope,
  isSingleCSharpModuleManifest,
  hasSafePlayerWritePolicy,
  isSafePhase4StructuredLog,
  getProductionDeployGate,
  getPhase4RollbackDisposition,
  isBackwardCompatibleContract
};
