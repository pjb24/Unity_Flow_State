// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/transfer-credential-policy");
const code = "ABCD2345";

assert.equal(policy.transferCodeLength, 8);
assert.equal(policy.verificationValueLength, 9);
assert.equal(policy.transferCodeCapacity, 1099511627776);
assert.equal(policy.tryNormalizeTransferCode("abcd-2345"), code);
assert.equal(policy.formatTransferCode(code), "ABCD-2345");
assert.equal(policy.tryNormalizeTransferCode("ABCD-234O"), "ABCD2340");
assert.equal(policy.tryNormalizeTransferCode("ABCD-234I"), "ABCD2341");
for (const value of ["ABCD-234@", "ABCD-234", "ABCD 2345", 1, null])
  assert.equal(policy.tryNormalizeTransferCode(value), null, String(value));
assert.ok(Math.abs(policy.expectedTransferCodeCollisionPairs(10000000) - 45.4747) < 0.001);
assert.equal(policy.expectedTransferCodeCollisionPairs(policy.transferCodeCapacity + 1), null);
assert.equal(policy.tryNormalizeVerificationValue("000000000"), "000000000");
assert.equal(policy.tryNormalizeVerificationValue("123456789"), "123456789");
for (const value of ["12345678", "1234567890", "12345A789", 123456789, null])
  assert.equal(policy.tryNormalizeVerificationValue(value), null, String(value));

const issued = 1000000;
const expires = issued + policy.transferLifetimeMilliseconds;
assert.equal(policy.isTransferExpired(issued, expires - 1), false);
assert.equal(policy.isTransferExpired(issued, expires), true);
assert.equal(policy.isTransferExpired(issued, issued - 1), true);
assert.equal(policy.isVerificationRetryAllowed(null, issued), true);
assert.equal(policy.isVerificationRetryAllowed(issued, issued + 4999), false);
assert.equal(policy.isVerificationRetryAllowed(issued, issued + 5000), true);
assert.equal(policy.canStartTransfer({ isActiveConnection: true, hasPendingSubmissions: false, isTransferPending: false }), true);
assert.equal(policy.canStartTransfer({ isActiveConnection: false, hasPendingSubmissions: false, isTransferPending: false }), false);
assert.equal(policy.canStartTransfer({ isActiveConnection: true, hasPendingSubmissions: true, isTransferPending: false }), false);
assert.equal(policy.canReissueTransferCredential({ isActiveConnection: true, isTransferPending: true, isExpired: false }), true);
assert.equal(policy.canReissueTransferCredential({ isActiveConnection: true, isTransferPending: true, isExpired: true }), false);

console.log("PASS transfer credential policy: format, expiry, throttle, lock and reissue boundaries");
