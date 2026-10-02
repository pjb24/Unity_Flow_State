// Pure client-state policy only. It does not access Local Save, Authentication, UI, or network services.
function getTransferStartDisposition(pendingCount, localSaveReady) {
  if (localSaveReady !== true) return "LocalSaveUnavailable";
  if (!Number.isSafeInteger(pendingCount) || pendingCount < 0) return "Invalid";
  return pendingCount === 0 ? "Ready" : "PendingMustBeCleared";
}

function getSourceCompletionActions(serverCompleted) {
  if (serverCompleted !== true) return [];
  return ["ClearAccountScopedCache", "ClearPersonalBests", "ClearAuthenticationSession",
    "AuthenticateNewAnonymous"];
}

function getTargetCompletionActions(localPendingCount, serverCompleted) {
  if (serverCompleted !== true || localPendingCount !== 0) return [];
  return ["ClearAccountScopedCache", "ReplacePersonalBestsFromConnectedAccount",
    "RefreshConnectedAccount"];
}

function preservesLocalOnlyData(action) {
  return action === "Settings" || action === "Tutorial";
}

module.exports = {
  getTransferStartDisposition,
  getSourceCompletionActions,
  getTargetCompletionActions,
  preservesLocalOnlyData
};
