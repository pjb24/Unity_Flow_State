// Pure display policy only. It never receives or exposes an internal Player ID.
const number = require("./public-player-number-policy");

function formatLeaderboardAccount(numberValue, isCurrentAccount) {
  const value = number.tryNormalizePublicPlayerNumber(numberValue);
  if (!value) return null;
  return isCurrentAccount === true ? `${value} (You)` : value;
}

function getPublicNumberPanelState({ isLoading, numberValue, failed }) {
  if (failed === true) return { state: "Error", text: "Could not load public number.", retry: true };
  if (isLoading === true) return { state: "Loading", text: "Loading public number...", retry: false };
  const text = formatLeaderboardAccount(numberValue, true);
  return text ? { state: "Ready", text, retry: false } :
    { state: "Error", text: "Could not load public number.", retry: true };
}

function getLeaderboardFailureState() {
  return { state: "Error", text: "Could not load leaderboard.", retry: true };
}

function getPublicNumberRecoveryNotice() {
  return "A public number cannot recover an account. Use a transfer code and verification value.";
}

function isOfflinePlayAllowed() { return true; }

module.exports = { formatLeaderboardAccount, getPublicNumberPanelState, getLeaderboardFailureState,
  getPublicNumberRecoveryNotice, isOfflinePlayAllowed };
