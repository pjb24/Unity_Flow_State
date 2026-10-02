const assert = require("assert").strict;
const policy = require("../CloudCode/environment-data-policy");

const verification = policy.tryCreateOnlineScope("project", "verification");
const production = policy.tryCreateOnlineScope("project", "production");
assert.deepEqual(verification, { projectId: "project", environmentId: "verification" });
assert.equal(policy.tryCreateOnlineScope("", "verification"), null);
assert.equal(policy.isSameOnlineScope(verification, verification), true);
assert.equal(policy.isSameOnlineScope(verification, production), false);
assert.equal(policy.canUseOnlineLocalData(verification, production), false);
assert.equal(policy.canUseOnlineLocalData(production, production), true);
assert.equal(policy.getLegacyOnlineDataMigration(null, verification), "AssignVerificationScope");
assert.equal(policy.getLegacyOnlineDataMigration(verification, verification), "KeepExistingScope");
assert.equal(policy.getLegacyOnlineDataMigration(null, null), "Reject");
assert.equal(policy.canCopyEnvironmentData(verification, production), false);
assert.equal(policy.isProductionTestDataAllowed(), false);

console.log("PASS environment data policy: explicit build scope, isolated online Local Save and verification-only tests");
