"use strict";
// Separate Production entry point. The verification probe and its fixed scope remain unchanged.
const { loadScope, validateScope } = require("./production-scope.cjs");
const { run: runVerification, failureLine: verificationFailureLine } = require("./verification-safe-log-probe.cjs");
const names = ["FS_PRODUCTION_PROBE_KEY_ID", "FS_PRODUCTION_PROBE_KEY_SECRET"];
function failureLine(error) {
  const inputReasons = ["PRODUCTION_SCOPE_INVALID", "PRODUCTION_SCOPE_MISMATCH", "EXPLICIT_PRODUCTION_CONFIRMATION_REQUIRED"];
  if (error && inputReasons.includes(error.message))
    return `FAIL / PRODUCTION_SAFE_LOG_PROBE / Phase=Input / Reason=${error.message} / HTTP=0`;
  return verificationFailureLine(error).replace("VERIFICATION_SAFE_LOG_PROBE", "PRODUCTION_SAFE_LOG_PROBE");
}
async function run(scope, keyId, keySecret, fetcher = fetch, diagnostic = () => {}) {
  validateScope(scope);
  // Reuse the audited request/response contract, with a closed URL mapping only.
  // No arbitrary URL or endpoint can be supplied by arguments or credentials.
  const verification = require("./verification-scope.cjs");
  const tokenFrom = `https://services.api.unity.com/auth/v2/token-exchange?projectId=${verification.projectId}&environmentId=${verification.environmentId}`;
  const moduleFrom = `https://cloud-code.services.api.unity.com/v1/projects/${verification.projectId}/modules/FlowStateVerification/EmitVerificationSafeLogProbe`;
  const tokenTo = `https://services.api.unity.com/auth/v2/token-exchange?projectId=${scope.projectId}&environmentId=${scope.environmentId}`;
  const moduleTo = `https://cloud-code.services.api.unity.com/v1/projects/${scope.projectId}/modules/FlowStateVerification/EmitVerificationSafeLogProbe`;
  let stage = 0;
  return runVerification(keyId, keySecret, (url, options) => {
    if (stage === 0 && url === tokenFrom) { stage++; return fetcher(tokenTo, options); }
    if (stage === 1 && url === moduleFrom) { stage++; return fetcher(moduleTo, options); }
    throw Error("PRODUCTION_ENDPOINT_REJECTED");
  }, diagnostic);
}
async function main(args = process.argv.slice(2), fetcher = fetch, report = console.log, scopeLoader = loadScope) {
  const keyId = process.env[names[0]], keySecret = process.env[names[1]];
  delete process.env[names[0]]; delete process.env[names[1]];
  if (!args.length || args.length === 1 && args[0] === "--plan") {
    scopeLoader();
    report("PLAN / production / ScopeMatched / FlowStateVerification.EmitVerificationSafeLogProbe / OneSyntheticEvent / NoRemoteRequests");
    return true;
  }
  if (args.length !== 2 || args[0] !== "--run" || args[1] !== "--confirm-production") throw Error("EXPLICIT_PRODUCTION_CONFIRMATION_REQUIRED");
  const scope = scopeLoader();
  let diagnostic;
  const status = await run(scope, keyId, keySecret, fetcher, value => { diagnostic = value; });
  const accepted = status === "Accepted" && diagnostic.phase === "Completed" && diagnostic.reason === "None" && diagnostic.httpStatus === 202;
  report(`${accepted ? "PASS" : "FAIL"} / PRODUCTION_SAFE_LOG_PROBE / Status=${status} / Phase=${diagnostic.phase} / Reason=${diagnostic.reason} / HTTP=${diagnostic.httpStatus}`);
  return accepted;
}
if (require.main === module) main().then(ok => { if (!ok) process.exitCode = 1; }).catch(error => {
  console.error(failureLine(error)); process.exitCode = 1;
});
module.exports = { main, run, failureLine };
