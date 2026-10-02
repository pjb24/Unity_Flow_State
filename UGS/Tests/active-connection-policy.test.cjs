// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/active-connection-policy");
const account = { accountId: "C", status: policy.active, currentPlayerId: "A", connectionRevision: 7 };
const bindingA = { playerId: "A", accountId: "C", connectionRevision: 7 };
const snapshotA = { accountId: "C", playerId: "A", connectionRevision: 7 };
const operation = { id: "submission-1", playerId: "A", connectionRevision: 7 };

assert.equal(policy.authorizeActiveConnection(account, bindingA, "A"), "Authorized");
assert.equal(policy.reserveMutation(account, bindingA, "A", operation), "Reserved");
assert.equal(policy.canBeginTransfer(account, bindingA, "A", false), true);
const reserved = { ...account, onlineOperation: operation };
assert.equal(policy.reserveMutation(reserved, bindingA, "A", operation), "Resume");
assert.equal(policy.reserveMutation(reserved, bindingA, "A", { ...operation, id: "submission-2" }), "Busy");
assert.equal(policy.canBeginTransfer(reserved, bindingA, "A", false), false);
assert.equal(policy.canBeginTransfer(account, bindingA, "A", true), false);
const afterTransfer = { accountId: "C", status: policy.active, currentPlayerId: "B", connectionRevision: 8 };
const bindingB = { playerId: "B", accountId: "C", connectionRevision: 8 };
assert.equal(policy.authorizeActiveConnection(afterTransfer, bindingA, "A"), "ActiveDeviceRequired");
assert.equal(policy.isQuerySnapshotCurrent(afterTransfer, bindingA, snapshotA), false);
assert.equal(policy.isQuerySnapshotCurrent(afterTransfer, bindingB,
  { accountId: "C", playerId: "B", connectionRevision: 8 }), true);
assert.equal(policy.authorizeActiveConnection({ ...account, status: "TransferPending" }, bindingA, "A"),
  "TransferPending");

console.log("PASS active connection policy: reservation, transfer gate and stale request rejection");
