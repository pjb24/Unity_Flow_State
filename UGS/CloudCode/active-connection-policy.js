// Pure policy only. Account/ledger CAS and service calls require server-side storage.
const active = "Active";

function authorizeActiveConnection(account, binding, contextPlayerId) {
  if (!account || !binding || typeof contextPlayerId !== "string" || !contextPlayerId)
    return "AccountUnavailable";
  if (account.currentPlayerId !== contextPlayerId || binding.playerId !== contextPlayerId ||
      binding.accountId !== account.accountId || binding.connectionRevision !== account.connectionRevision)
    return "ActiveDeviceRequired";
  return account.status === active ? "Authorized" : "TransferPending";
}

function reserveMutation(account, binding, contextPlayerId, operation) {
  if (authorizeActiveConnection(account, binding, contextPlayerId) !== "Authorized")
    return "Rejected";
  if (!operation || typeof operation.id !== "string" || !operation.id) return "Invalid";
  if (!account.onlineOperation) return "Reserved";
  return account.onlineOperation.id === operation.id &&
    account.onlineOperation.playerId === contextPlayerId &&
    account.onlineOperation.connectionRevision === account.connectionRevision ? "Resume" : "Busy";
}

function canBeginTransfer(account, binding, contextPlayerId, hasLedgerActive) {
  return authorizeActiveConnection(account, binding, contextPlayerId) === "Authorized" &&
    !account.onlineOperation && hasLedgerActive !== true;
}

function isQuerySnapshotCurrent(account, binding, snapshot) {
  return !!snapshot && authorizeActiveConnection(account, binding, snapshot.playerId) === "Authorized" &&
    account.accountId === snapshot.accountId && account.connectionRevision === snapshot.connectionRevision;
}

module.exports = { active, authorizeActiveConnection, reserveMutation, canBeginTransfer, isQuerySnapshotCurrent };
