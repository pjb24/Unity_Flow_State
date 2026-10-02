// Pure policy test. No credentials, network, Unity Editor, Test Runner, or UGS service calls.
const assert = require("assert").strict;
const policy = require("../CloudCode/public-player-number-policy");

assert.equal(policy.width, 10);
assert.equal(policy.minimum, 1);
assert.equal(policy.maximum, 9999999999);
assert.equal(policy.tryFormatIssuedNumber(1), "0000000001");
assert.equal(policy.tryFormatIssuedNumber(42), "0000000042");
assert.equal(policy.tryFormatIssuedNumber(9999999999), "9999999999");

for (const value of [0, -1, 1.5, 10000000000, Number.MAX_SAFE_INTEGER + 1, NaN, Infinity])
  assert.equal(policy.tryFormatIssuedNumber(value), null, String(value));

assert.equal(policy.tryNormalizePublicPlayerNumber("0000000001"), "0000000001");
assert.equal(policy.tryNormalizePublicPlayerNumber("9999999999"), "9999999999");
for (const value of ["0000000000", "10000000000", "1", "000000001", "000000000a", 1, null])
  assert.equal(policy.tryNormalizePublicPlayerNumber(value), null, String(value));

console.log("PASS public player number policy: fixed width, leading zero, safe range, exact string");
