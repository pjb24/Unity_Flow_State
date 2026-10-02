// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/leaderboard-owner-policy");

assert.equal(policy.tryNormalizeGeneratedLeaderboardOwnerId("8A1C2E30-4F8B-4C5A-9D72-3BA41C2E6F90"),
  "8a1c2e30-4f8b-4c5a-9d72-3ba41c2e6f90");
for (const value of ["8a1c2e30-4f8b-3c5a-9d72-3ba41c2e6f90",
  "8a1c2e30-4f8b-4c5a-7d72-3ba41c2e6f90", "C", "0000000001", "", null])
  assert.equal(policy.tryNormalizeGeneratedLeaderboardOwnerId(value), null, String(value));

assert.equal(policy.tryUseLegacyLeaderboardOwnerId("legacy-ugs-player", "legacy-ugs-player"),
  "legacy-ugs-player");
for (const [owner, authenticated] of [["legacy-ugs-player", "other-player"], ["", ""],
  ["0000000001", "other-player"], [null, "legacy-ugs-player"]])
  assert.equal(policy.tryUseLegacyLeaderboardOwnerId(owner, authenticated), null);

console.log("PASS leaderboard owner policy: generated UUID v4 and authenticated legacy owner only");
