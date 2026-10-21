// Local-only scope contract. No Unity, UGS, secret, or network access.
const assert = require("assert").strict;
const scope = require("../CloudCode/execution-scope");

const verification = scope.createExecutionScope({ projectId: "verification-project", environmentId: "verification-environment" });
const production = scope.createExecutionScope({ projectId: "production-project", environmentId: "production-environment" });

assert.deepEqual(verification, { projectId: "verification-project", environmentId: "verification-environment" });
assert.equal(Object.isFrozen(verification), true);
assert.equal(scope.createExecutionScope(null), null);
assert.equal(scope.createExecutionScope({ projectId: "", environmentId: "environment" }), null);
assert.equal(scope.createExecutionScope({ projectId: "project", environmentId: "" }), null);

const stored = scope.createScopedValue(verification, { schemaVersion: 1, accountId: "opaque-account" });
assert.equal(stored.projectId, verification.projectId);
assert.equal(stored.environmentId, verification.environmentId);
assert.equal(scope.matchesExecutionScope(stored, verification), true);
assert.equal(scope.matchesExecutionScope(stored, production), false);
assert.equal(scope.createScopedValue(null, { schemaVersion: 1 }), null);

console.log("PASS execution scope contract: immutable per-invocation scope, explicit scoped values, and environment mismatch rejection.");
