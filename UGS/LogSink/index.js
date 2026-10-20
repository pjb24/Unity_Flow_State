"use strict";

const crypto = require("crypto");
const http = require("http");

const maxBodyBytes = 2048;
const allowedFields = new Set([
  "environment", "moduleReference", "outcome", "reason", "durationBucket", "correlationId", "rateLimitResult"
]);

function isSafeEvent(value) {
  if (!value || typeof value !== "object" || Array.isArray(value)) return false;
  if (Object.keys(value).some(key => !allowedFields.has(key))) return false;
  return typeof value.environment === "string" && value.environment.length > 0 &&
    typeof value.moduleReference === "string" && value.moduleReference.length > 0 &&
    typeof value.outcome === "string" && value.outcome.length > 0 &&
    typeof value.reason === "string" && value.reason.length > 0 &&
    Number.isSafeInteger(value.durationBucket) && value.durationBucket >= 0 &&
    typeof value.correlationId === "string" && value.correlationId.length > 0 &&
    typeof value.rateLimitResult === "string" && value.rateLimitResult.length > 0;
}

function isFreshTimestamp(value, nowMilliseconds) {
  if (!/^\d{13}$/.test(value || "")) return false;
  const timestamp = Number(value);
  return Number.isSafeInteger(timestamp) && Math.abs(nowMilliseconds - timestamp) <= 5 * 60 * 1000;
}

function computeSignature(secret, timestamp, body) {
  return crypto.createHmac("sha256", secret).update(`${timestamp}.`).update(body).digest("base64");
}

function hasValidSignature(secret, timestamp, signature, body) {
  if (typeof secret !== "string" || secret.length < 32 || typeof signature !== "string") return false;
  const expected = Buffer.from(computeSignature(secret, timestamp, body), "utf8");
  const supplied = Buffer.from(signature, "utf8");
  return expected.length === supplied.length && crypto.timingSafeEqual(expected, supplied);
}

function createServer({ secret, now = () => Date.now(), emit = event => process.stdout.write(`${JSON.stringify(event)}\n`) }) {
  if (typeof secret !== "string" || secret.length < 32) throw new Error("FLOW_STATE_LOG_SINK_HMAC must be a non-empty secret of at least 32 characters.");
  const server = http.createServer((request, response) => {
    if (request.method === "GET" && request.url === "/health") {
      response.writeHead(204).end();
      return;
    }
    if (request.method !== "POST" || request.url !== "/v1/events") {
      response.writeHead(404).end();
      return;
    }
    const timestamp = request.headers["x-flow-state-timestamp"];
    const signature = request.headers["x-flow-state-signature"];
    if (typeof timestamp !== "string" || typeof signature !== "string" || !isFreshTimestamp(timestamp, now())) {
      response.writeHead(401).end();
      return;
    }
    if (!/^application\/json(?:;|$)/i.test(request.headers["content-type"] || "")) {
      response.writeHead(415).end();
      return;
    }
    const chunks = [];
    let size = 0;
    let tooLarge = false;
    request.on("data", chunk => {
      size += chunk.length;
      if (size > maxBodyBytes) { tooLarge = true; return; }
      chunks.push(chunk);
    });
    request.on("end", () => {
      if (tooLarge) { response.writeHead(413).end(); return; }
      const body = Buffer.concat(chunks);
      if (!hasValidSignature(secret, timestamp, signature, body)) {
        response.writeHead(401).end();
        return;
      }
      let event;
      try { event = JSON.parse(body.toString("utf8")); } catch { response.writeHead(400).end(); return; }
      if (!isSafeEvent(event)) { response.writeHead(400).end(); return; }
      emit(event);
      response.writeHead(202).end();
    });
  });
  server.requestTimeout = 5 * 1000;
  server.headersTimeout = 6 * 1000;
  return server;
}

if (require.main === module) {
  const port = Number(process.env.PORT || "8080");
  createServer({ secret: process.env.FLOW_STATE_LOG_SINK_HMAC }).listen(port, "0.0.0.0");
}

module.exports = { allowedFields, isSafeEvent, isFreshTimestamp, computeSignature, hasValidSignature, createServer };
