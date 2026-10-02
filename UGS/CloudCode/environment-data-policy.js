// Pure environment-data policy. Build configuration, Local Save I/O and UGS calls are out of scope.
function tryCreateOnlineScope(projectId, environmentId) {
  if (typeof projectId !== "string" || projectId.length === 0 ||
      typeof environmentId !== "string" || environmentId.length === 0) return null;
  return { projectId, environmentId };
}

function isSameOnlineScope(left, right) {
  return !!left && !!right && left.projectId === right.projectId &&
    left.environmentId === right.environmentId;
}

function canUseOnlineLocalData(savedScope, activeScope) {
  return isSameOnlineScope(savedScope, activeScope);
}

function getLegacyOnlineDataMigration(savedScope, verificationScope) {
  if (savedScope) return "KeepExistingScope";
  return tryCreateOnlineScope(verificationScope && verificationScope.projectId,
    verificationScope && verificationScope.environmentId) ? "AssignVerificationScope" : "Reject";
}

function canCopyEnvironmentData(sourceScope, targetScope) {
  return false;
}

function isProductionTestDataAllowed() {
  return false;
}

module.exports = { tryCreateOnlineScope, isSameOnlineScope, canUseOnlineLocalData,
  getLegacyOnlineDataMigration, canCopyEnvironmentData, isProductionTestDataAllowed };
