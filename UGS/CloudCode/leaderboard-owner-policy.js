// Pure policy only. Owner allocation and persistence require a server-side store.
const uuidV4 = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

function tryNormalizeGeneratedLeaderboardOwnerId(value) {
  if (typeof value !== "string" || !uuidV4.test(value)) return null;
  return value.toLowerCase();
}

// A legacy leaderboard row is preserved only when its exact existing owner ID
// comes from the authenticated server context, never from a client parameter.
function tryUseLegacyLeaderboardOwnerId(existingOwnerId, authenticatedPlayerId) {
  if (typeof existingOwnerId !== "string" || !existingOwnerId ||
      typeof authenticatedPlayerId !== "string" || !authenticatedPlayerId ||
      existingOwnerId !== authenticatedPlayerId) return null;
  return existingOwnerId;
}

module.exports = {
  tryNormalizeGeneratedLeaderboardOwnerId,
  tryUseLegacyLeaderboardOwnerId
};
