// Pure retention policy. Storage cleanup and server timestamps are implemented later.
const receiptRetentionMilliseconds = 180 * 24 * 60 * 60 * 1000;

function isValidServerTimestamp(value) {
  return Number.isSafeInteger(value) && value >= 0;
}

function isTerminalReceiptExpired(terminalAtMilliseconds, nowMilliseconds) {
  if (!isValidServerTimestamp(terminalAtMilliseconds) || !isValidServerTimestamp(nowMilliseconds) ||
      nowMilliseconds < terminalAtMilliseconds) return false;
  return nowMilliseconds >= terminalAtMilliseconds + receiptRetentionMilliseconds;
}

function getPendingDisposition(localCreatedAtMilliseconds, nowMilliseconds) {
  if (!isValidServerTimestamp(localCreatedAtMilliseconds) || !isValidServerTimestamp(nowMilliseconds) ||
      nowMilliseconds < localCreatedAtMilliseconds ||
      nowMilliseconds >= localCreatedAtMilliseconds + receiptRetentionMilliseconds) return "SubmissionExpired";
  return "RetryAllowed";
}

function getReplayDisposition(receipt, payloadMatches, nowMilliseconds) {
  if (!receipt || isTerminalReceiptExpired(receipt.terminalAtMilliseconds, nowMilliseconds)) return "NewSubmission";
  return payloadMatches === true ? "ExistingResult" : "SubmissionIdConflict";
}

module.exports = {
  receiptRetentionMilliseconds,
  isTerminalReceiptExpired,
  getPendingDisposition,
  getReplayDisposition
};
