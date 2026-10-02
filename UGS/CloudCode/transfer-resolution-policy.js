// Pure policy only. Server CAS, credential verification, storage, and network calls are out of scope.
const pending = "TransferPending";
const completed = "Completed";
const cancelled = "Cancelled";
const expired = "Expired";

function canUseTransferTarget(target) {
  return !!target && typeof target.playerId === "string" && !!target.playerId &&
    target.hasLocalPending === false;
}

function resolveTransfer(transfer, request) {
  if (!transfer || !request || typeof transfer.status !== "string") return { outcome: "Invalid" };
  if (request.kind === "complete" && transfer.status === completed)
    return transfer.completedPlayerId === request.playerId ? { outcome: "AlreadyCompleted", transfer } :
      { outcome: "Consumed", transfer };
  if (transfer.status !== pending) return { outcome: "Unavailable", transfer };
  if (request.kind === "expire") {
    if (!Number.isSafeInteger(request.nowMilliseconds) ||
        !Number.isSafeInteger(transfer.expiresAtMilliseconds)) return { outcome: "Invalid", transfer };
    if (request.nowMilliseconds < transfer.expiresAtMilliseconds) return { outcome: "NotExpired", transfer };
    return { outcome: "Expired", transfer: { ...transfer, status: expired, credentialActive: false } };
  }
  if (request.kind === "cancel") {
    if (request.playerId !== transfer.sourcePlayerId) return { outcome: "NotSource", transfer };
    return { outcome: "Cancelled", transfer: { ...transfer, status: cancelled, credentialActive: false } };
  }
  if (request.kind === "complete" && (!Number.isSafeInteger(request.nowMilliseconds) ||
      !Number.isSafeInteger(transfer.expiresAtMilliseconds))) return { outcome: "Invalid", transfer };
  if (request.kind === "complete" && request.nowMilliseconds >= transfer.expiresAtMilliseconds)
    return { outcome: "Expired", transfer: { ...transfer, status: expired, credentialActive: false } };
  if (request.kind !== "complete" || request.playerId === transfer.sourcePlayerId ||
      transfer.credentialActive !== true || request.credentialsValid !== true || request.retryAllowed !== true ||
      !canUseTransferTarget(request.target))
    return { outcome: "Rejected", transfer };
  return {
    outcome: "Completed",
    transfer: { ...transfer, status: completed, credentialActive: false,
      completedPlayerId: request.playerId, completedConnectionRevision: transfer.connectionRevision + 1 }
  };
}

module.exports = { pending, completed, cancelled, expired, canUseTransferTarget, resolveTransfer };
