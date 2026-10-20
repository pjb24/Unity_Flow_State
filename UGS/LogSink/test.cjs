"use strict";

const assert = require("assert").strict;
const http = require("http");
const sink = require("./index");

const safe = {
  environment: "verification", moduleReference: "synthetic-reference", outcome: "Accepted", reason: "SyntheticProbe",
  durationBucket: 0, correlationId: "opaque-synthetic-id", rateLimitResult: "Allowed"
};
assert.equal(sink.isSafeEvent(safe), true);
for (const forbidden of ["playerId", "payload", "token", "secret", "requestBody", "publicNumber"])
  assert.equal(sink.isSafeEvent({ ...safe, [forbidden]: "forbidden" }), false, forbidden);
assert.equal(sink.isFreshTimestamp("1700000000000", 1700000000000), true);
assert.equal(sink.isFreshTimestamp("1699999699999", 1700000000000), false);
const secret = "synthetic-test-secret-with-at-least-thirty-two-characters";
const body = Buffer.from(JSON.stringify(safe));
const signature = sink.computeSignature(secret, "1700000000000", body);
assert.equal(sink.hasValidSignature(secret, "1700000000000", signature, body), true);
assert.equal(sink.hasValidSignature(secret, "1700000000000", signature, Buffer.from("{}")), false);

function request(port, method, requestPath, body, headers = {}) {
  return new Promise((resolve, reject) => {
    const request = http.request({ host: "127.0.0.1", port, method, path: requestPath, headers }, response => {
      response.resume();
      response.on("end", () => resolve(response.statusCode));
    });
    request.on("error", reject);
    request.end(body);
  });
}

(async () => {
  const emitted = [];
  const now = 1700000000000;
  const server = sink.createServer({ secret, now: () => now, emit: event => emitted.push(event) });
  await new Promise(resolve => server.listen(0, "127.0.0.1", resolve));
  const port = server.address().port;
  const headersFor = value => ({
    "content-type": "application/json", "x-flow-state-timestamp": String(now),
    "x-flow-state-signature": sink.computeSignature(secret, String(now), value)
  });
  try {
    assert.equal(await request(port, "GET", "/health"), 204);
    assert.equal(await request(port, "POST", "/v1/events", body, headersFor(body)), 202);
    assert.deepEqual(emitted, [safe]);
    const extra = Buffer.from(JSON.stringify({ ...safe, playerId: "forbidden" }));
    assert.equal(await request(port, "POST", "/v1/events", extra, headersFor(extra)), 400);
    assert.equal(await request(port, "POST", "/v1/events", body, { ...headersFor(body), "x-flow-state-signature": "invalid" }), 401);
    assert.equal(await request(port, "POST", "/v1/events", body, { ...headersFor(body), "x-flow-state-timestamp": String(now - 300001) }), 401);
    const invalidJson = Buffer.from("{");
    assert.equal(await request(port, "POST", "/v1/events", invalidJson, headersFor(invalidJson)), 400);
    const tooLarge = Buffer.alloc(2049, 0x20);
    assert.equal(await request(port, "POST", "/v1/events", tooLarge, headersFor(tooLarge)), 413);
    assert.equal(await request(port, "POST", "/v1/events", body, { ...headersFor(body), "content-type": "text/plain" }), 415);
    assert.equal(await request(port, "POST", "/unknown", body, headersFor(body)), 404);
  } finally {
    await new Promise(resolve => server.close(resolve));
  }
  console.log("PASS safe log sink: health, closed schema, HMAC/timestamp, malformed input, size limit, content type, and route boundary.");
})().catch(error => { console.error(error); process.exitCode = 1; });
