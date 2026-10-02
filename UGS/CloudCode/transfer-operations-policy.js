// Pure operational policy. It neither writes audit records nor reads secrets.
const auditEvents = new Set(["Requested", "CredentialReissued", "Completed", "Cancelled", "Expired", "VerificationFailed"]);
const auditOutcomes = new Set(["Accepted", "Rejected", "Failed"]);

function isSameEnvironment(request, account, transfer) {
  if (!request || !account || !transfer) return false;
  return typeof request.projectId === "string" && request.projectId.length > 0 &&
    typeof request.environmentId === "string" && request.environmentId.length > 0 &&
    request.projectId === account.projectId && request.projectId === transfer.projectId &&
    request.environmentId === account.environmentId && request.environmentId === transfer.environmentId;
}

function createSafeAuditEvent(input) {
  if (!input || !auditEvents.has(input.event) || !auditOutcomes.has(input.outcome) ||
      typeof input.transferId !== "string" || input.transferId.length === 0 ||
      typeof input.environmentId !== "string" || input.environmentId.length === 0 ||
      !Number.isSafeInteger(input.failureCount) || input.failureCount < 0 ||
      !Number.isSafeInteger(input.occurredAtUnixMilliseconds) || input.occurredAtUnixMilliseconds < 0) return null;
  return {
    event: input.event,
    outcome: input.outcome,
    transferId: input.transferId,
    environmentId: input.environmentId,
    occurredAtUnixMilliseconds: input.occurredAtUnixMilliseconds,
    failureCount: input.failureCount,
    reasonCategory: typeof input.reasonCategory === "string" ? input.reasonCategory : ""
  };
}

function isSafeForGeneralError(value) {
  return typeof value === "string" &&
    !/transfer.?code|verification.?value|authorization|access.?token|service.?token|hmac|digest|player.?id/i.test(value);
}

function getDeploymentDisposition(input) {
  if (!input || typeof input.sourceEnvironment !== "string" || input.sourceEnvironment.length === 0 ||
      typeof input.targetEnvironment !== "string" || input.targetEnvironment.length === 0) return "Blocked";
  if (input.sourceEnvironment === input.targetEnvironment) return "RejectSameEnvironment";
  if (!input.localTestsPassed || !input.remoteBackupCaptured || !input.accessPolicyReviewed ||
      !input.requiredSecretsAvailable || !input.targetEnvironmentExplicit) return "Blocked";
  return "Ready";
}

function getRollbackDisposition(input) {
  if (!input || !input.previousVersionCaptured || !input.targetEnvironmentExplicit) return "Blocked";
  return "RepublishPreviousVersion";
}

module.exports = {
  isSameEnvironment,
  createSafeAuditEvent,
  isSafeForGeneralError,
  getDeploymentDisposition,
  getRollbackDisposition
};
