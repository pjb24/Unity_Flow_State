"use strict";
// User-run synthetic probe. No player authentication, storage writes or secret reads.
const { projectId, environmentId } = require("./verification-scope.cjs");
const credentialIdVariable = "FS_SAFE_LOG_PROBE_KEY_ID";
const credentialSecretVariable = "FS_SAFE_LOG_PROBE_KEY_SECRET";

function failure(code, phase, status = 0) {
  return Object.assign(new Error(code), { code, phase, status });
}
async function requestJson(fetcher, url, headers, body, phase) {
  let status = 0;
  try {
    const response = await fetcher(url, {
      method: "POST", headers, body, redirect: "error", signal: AbortSignal.timeout(20000)
    });
    status = response.status;
    if (!response.ok) throw failure("HTTP_FAILURE", phase, status);
    try { return await response.json(); }
    catch (_) { throw failure("INVALID_JSON", phase, status); }
  } catch (error) {
    if (error.code === "HTTP_FAILURE" || error.code === "INVALID_JSON") throw failure(error.code, phase, status);
    throw failure("REQUEST_UNCONFIRMED", phase, status);
  }
}
async function run(keyId, keySecret, fetcher = fetch, onDiagnostic = () => {}) {
  if (typeof keyId !== "string" || !/^[A-Za-z0-9_-]+$/.test(keyId) ||
      typeof keySecret !== "string" || !keySecret || /[\r\n]/.test(keySecret))
    throw failure("SERVICE_ACCOUNT_KEY_REQUIRED", "Input");
  const token = await requestJson(fetcher,
    `https://services.api.unity.com/auth/v2/token-exchange?projectId=${projectId}&environmentId=${environmentId}`,
    { Authorization: "Basic " + Buffer.from(`${keyId}:${keySecret}`).toString("base64"), "Content-Type": "application/json" },
    "{}", "TokenExchange");
  if (typeof token.accessToken !== "string" || !token.accessToken || /[\r\n]/.test(token.accessToken))
    throw failure("TOKEN_UNAVAILABLE", "TokenExchange");
  const response = await requestJson(fetcher,
    `https://cloud-code.services.api.unity.com/v1/projects/${projectId}/modules/FlowStateVerification/EmitVerificationSafeLogProbe`,
    { Authorization: `Bearer ${token.accessToken}`, "Content-Type": "application/json" },
    JSON.stringify({ params: {} }), "SyntheticProbe");
  const status = response && response.output && response.output.status;
  if (!["Accepted", "Unavailable", "RateLimited"].includes(status))
    throw failure("UNEXPECTED_RESPONSE", "SyntheticProbe");
  const phases = ["EnvironmentCheck", "UrlSecretRead", "HmacSecretRead", "UrlValidation", "HmacValidation", "EventPreparation", "HttpRequest", "Completed"];
  const reasons = ["Failed", "Timeout", "HttpRejected", "None", "EnvironmentConfigurationUnavailable"];
  onDiagnostic({
    phase: phases.includes(response.output.phase) ? response.output.phase : "Unknown",
    reason: reasons.includes(response.output.reason) ? response.output.reason : "Unknown",
    httpStatus: Number.isInteger(response.output.httpStatus) && response.output.httpStatus >= 100 && response.output.httpStatus <= 599 ? response.output.httpStatus : 0
  });
  return status;
}
function failureLine(error) {
  const code = ["HTTP_FAILURE", "INVALID_JSON", "REQUEST_UNCONFIRMED", "SERVICE_ACCOUNT_KEY_REQUIRED",
    "TOKEN_UNAVAILABLE", "UNEXPECTED_RESPONSE"].includes(error.code) ? error.code : "PROBE_UNCONFIRMED";
  const phase = ["Input", "TokenExchange", "SyntheticProbe"].includes(error.phase) ? error.phase : "Unknown";
  return `FAIL / VERIFICATION_SAFE_LOG_PROBE / Phase=${phase} / Reason=${code} / HTTP=${Number.isInteger(error.status) ? error.status : 0}`;
}
async function main(args = process.argv.slice(2), fetcher = fetch, report = console.log) {
  if (!args.length || args.length === 1 && args[0] === "--plan") {
    report("PLAN / verification / FlowStateVerification.EmitVerificationSafeLogProbe / OneSyntheticEvent / NoRemoteRequests");
    return true;
  }
  if (args.length !== 1 || args[0] !== "--run") throw failure("PROBE_UNCONFIRMED", "Input");
  const keyId = process.env[credentialIdVariable], keySecret = process.env[credentialSecretVariable];
  delete process.env[credentialIdVariable]; delete process.env[credentialSecretVariable];
  let diagnostic;
  const status = await run(keyId, keySecret, fetcher, value => { diagnostic = value; });
  report(`${status === "Accepted" ? "PASS" : "FAIL"} / VERIFICATION_SAFE_LOG_PROBE / Status=${status}` +
    (status === "Unavailable" ? ` / Phase=${diagnostic.phase} / Reason=${diagnostic.reason} / HTTP=${diagnostic.httpStatus}` : ""));
  return status === "Accepted";
}
if (require.main === module) main().then(ok => { if (!ok) process.exitCode = 1; }).catch(error => {
  console.error(failureLine(error)); process.exitCode = 1;
});
module.exports = { run, main, failureLine };
