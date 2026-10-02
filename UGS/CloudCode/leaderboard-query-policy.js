// Pure query-display policy. It does not fetch pages or calculate global ranks.
const topEntryCount = 10;
const aroundEntryCount = 7;

function getRequestedEntryCount(kind) {
  if (kind === "top") return topEntryCount;
  if (kind === "around") return aroundEntryCount;
  return 0;
}

function hasPlayerPageNavigation() { return false; }

function getTiePageBoundaryDisposition() { return "SplitTieGroupKeepGlobalRank"; }

function canRefreshSnapshot(trigger) {
  return trigger === "InitialEntry" || trigger === "ExplicitRetry" ||
    trigger === "ExplicitRefresh" || trigger === "Reentry";
}

function compareExactPublicNumbers(left, right) {
  if (typeof left !== "string" || typeof right !== "string" ||
      !/^\d{10}$/.test(left) || !/^\d{10}$/.test(right)) return null;
  return left.localeCompare(right);
}

module.exports = { topEntryCount, aroundEntryCount, getRequestedEntryCount, hasPlayerPageNavigation,
  getTiePageBoundaryDisposition, canRefreshSnapshot, compareExactPublicNumbers };
