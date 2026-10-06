"use strict";
// Admin storage probe, NOT an authenticated player/Cloud Code invocation.
// Default is offline plan. No existing fs8 entities or Leaderboards are touched.
const crypto = require("node:crypto");
const { isDeepStrictEqual } = require("node:util");
const { createAccountStore, projectId, environmentId } = require("../CloudCode/account-store");
const { createAccountService } = require("../CloudCode/account-service");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const { createTransferCompletionService } = require("../CloudCode/transfer-completion-service");
const scope = { schemaVersion: 1, projectId, environmentId };
function fail(code, status = 0) { return Object.assign(new Error(code), { code, response: { status } }); }
function check(value, code) { if (!value) throw fail(code); }

async function request(fetcher, url, options) {
  let receivedStatus = 0;
  try {
    const response = await fetcher(url, { ...options, redirect: "error", signal: AbortSignal.timeout(5000) });
    receivedStatus = response.status || 200;
    if (!response.ok) throw fail("HTTP_FAILURE", response.status);
    // Do not return/log remote error bodies or exceptions containing credentials.
    return await response.json();
  } catch (error) {
    const safe = fail(error.code === "HTTP_FAILURE" ? error.code : "REQUEST_UNCONFIRMED", error.response?.status || receivedStatus);
    safe.networkFault = error.name === "TimeoutError" ? "TIMEOUT" : error.name === "AbortError" ? "ABORTED" :
      error.name === "SyntaxError" ? "INVALID_JSON" : error.code === "HTTP_FAILURE" ? "HTTP_STATUS" :
      ["UND_ERR_CONNECT_TIMEOUT", "ENOTFOUND", "ECONNRESET", "ECONNREFUSED"].includes(error.cause?.code) ? error.cause.code : "TRANSPORT_UNKNOWN";
    throw safe;
  }
}
async function step(caseId, phase, action) {
  try { return await action(); }
  catch (error) { error.probeCase = caseId; error.probePhase = phase; throw error; }
}
function failureLine(error) {
  const safeWord = (value, fallback) => typeof value === "string" && /^[A-Za-z0-9_-]{1,48}$/.test(value) ? value : fallback;
  return `FAIL / ${safeWord(error.code, "PROBE_UNCONFIRMED")} / HTTP=${Number.isInteger(error.response?.status) ? error.response.status : 0}` +
    ` / Case=${safeWord(error.probeCase, "NOT_STARTED")} / Phase=${safeWord(error.probePhase, "TOKEN_EXCHANGE")}` +
    ` / Operation=${safeWord(error.probeOperation, "NONE")} / RequestOrdinal=${Number.isInteger(error.requestOrdinal) ? error.requestOrdinal : 0}` +
    ` / NetworkFault=${safeWord(error.networkFault, "NONE")} / DataRetained=True / NoAutomaticRetry`;
}
async function exchange(keyId, keySecret, fetcher = fetch) {
  check(typeof keyId === "string" && /^[A-Za-z0-9_-]+$/.test(keyId) &&
    typeof keySecret === "string" && keySecret.length > 0 && !/[\r\n]/.test(keySecret), "SERVICE_KEY_REQUIRED");
  const result = await request(fetcher,
    `https://services.api.unity.com/auth/v2/token-exchange?projectId=${projectId}&environmentId=${environmentId}`,
    { method: "POST", headers: { Authorization: "Basic " + Buffer.from(`${keyId}:${keySecret}`).toString("base64") } });
  check(typeof result.accessToken === "string" && result.accessToken.length > 0, "TOKEN_EXCHANGE_FAILED");
  return result.accessToken;
}
function adapter(token, runId, caseId, fetcher = fetch) {
  check(typeof token === "string" && token.length > 0 && !/[\r\n]/.test(token), "TOKEN_REQUIRED");
  check(/^[a-f0-9]{32}$/.test(runId) && /^[a-z0-9-]{1,40}$/.test(caseId), "INVALID_RUN_SCOPE");
  const names = new Map(), reverse = new Map(), verified = new Set();
  const state = { writes: 0, requests: 0, faultAt: 0, faultMode: "", injected: false, operations: [] };
  async function send(url, options, operation) {
    const ordinal = ++state.requests;
    try { return await request(fetcher, url, options); }
    catch (error) { error.probeOperation = operation; error.requestOrdinal = ordinal; throw error; }
  }
  function target(project, original) {
    check(project === projectId && /^fs8-[A-Za-z0-9_-]+$/.test(original), "SCOPE_DENIED");
    if (!names.has(original)) {
      const mapped = `p2v-${runId}-${crypto.createHash("sha256").update(`${caseId}/${original}`).digest("hex").slice(0, 12)}`;
      check(!reverse.has(mapped) || reverse.get(mapped) === original, "NAMESPACE_COLLISION");
      names.set(original, mapped); reverse.set(mapped, original);
    }
    return names.get(original);
  }
  async function call(project, original, method, body, keys) {
    const mapped = target(project, original);
    const root = `https://cloud-save.services.api.unity.com/v1/data/projects/${projectId}/custom/${mapped}/private/`;
    if (!verified.has(mapped)) {
      // Refuse to adopt or overwrite a pre-existing namespace, even on collision.
      const initial = await send(root + "items", { method: "GET", headers: { Authorization: `Bearer ${token}` } }, "NAMESPACE_READ");
      check(Array.isArray(initial.results) && initial.results.length === 0, "NAMESPACE_ALREADY_EXISTS");
      verified.add(mapped);
    }
    const writing = method !== "GET";
    let ordinal = 0;
    if (writing) {
      ordinal = ++state.writes;
      state.operations.push(body);
      if (ordinal === state.faultAt && state.faultMode === "before") {
        state.injected = true; throw fail("INJECTED_BEFORE_WRITE");
      }
    }
    const query = new URLSearchParams();
    for (const key of keys || []) query.append("keys", key);
    const url = root + (writing ? body.data ? "item-batch" : "items" : "items") + (query.size ? `?${query}` : "");
    const data = await send(url, { method,
      headers: { Authorization: `Bearer ${token}`, ...(writing ? { "Content-Type": "application/json" } : {}) },
      ...(writing ? { body: JSON.stringify(body) } : {}) }, writing ? body.data ? "WRITE_BATCH" : "WRITE_SINGLE" : "READ_ITEMS");
    if (writing && ordinal === state.faultAt && state.faultMode === "after") {
      state.injected = true; throw fail("INJECTED_RESPONSE_LOSS");
    }
    return { data };
  }
  return { state, target, api: {
    getPrivateCustomItems: (p, id, keys) => call(p, id, "GET", null, keys),
    setPrivateCustomItem: (p, id, value) => call(p, id, "POST", value),
    setPrivateCustomItemBatch: (p, id, value) => call(p, id, "POST", value)
  } };
}
async function seed(store) {
  const players = ["probeA", "probeB", "probeC"];
  const accounts = [];
  for (let i = 0; i < players.length; i++) {
    const accountId = crypto.randomUUID(), owner = crypto.randomUUID(), number = String(i + 1).padStart(10, "0");
    const account = { ...scope, accountId, currentPlayerId: players[i], initialPlayerId: players[i],
      connectionRevision: 1, status: "Active", publicPlayerNumber: number, leaderboardOwnerId: owner };
    await store.create("account", accountId, account);
    await store.create("player", players[i], { ...scope, playerId: players[i], accountId, status: "Active", connectionRevision: 1 });
    await store.create("ledger", accountId, { ...scope, accountId, active: "", entries: [], best: {} });
    await store.create("number", number, { ...scope, accountId });
    await store.create("owner", owner, { ...scope, accountId });
    accounts.push(account);
  }
  return accounts;
}
async function setup(token, runId, caseId, fetcher, clock = Date.now, issue = true) {
  const live = adapter(token, runId, caseId, fetcher);
  const context = playerId => ({ ...scope, playerId, serviceToken: "admin-probe-not-player-auth" });
  const store = createAccountStore(context("probeA"), live.api);
  const accounts = await step(caseId, "SETUP", () => seed(store));
  const secret = crypto.randomBytes(32).toString("base64"); // Ephemeral probe-only key, not the deployed Secret.
  const provider = async () => secret;
  const credential = issue ? await step(caseId, "START_TRANSFER", () => createTransferStartService(store, provider, { clock }).start(context("probeA"))) : null;
  check(!issue || credential.status === "Success", "PROBE_START_FAILED");
  live.state.writes = 0; live.state.operations = [];
  const complete = player => createTransferCompletionService(store, provider, clock).complete(context(player), credential.code, credential.verificationValue);
  return { live, store, context, accounts, complete,
    start: () => createTransferStartService(store, provider, { clock }).start(context("probeA")) };
}
async function verify(f, winner) {
  const account = (await f.store.read("account", f.accounts[0].accountId)).value;
  check(account.status === "Active" && account.currentPlayerId === winner && account.connectionRevision === 2 &&
    account.transfer.status === "Completed" && account.transfer.credentialActive === false, "TERMINAL_STATE_INVALID");
  check(account.publicPlayerNumber === f.accounts[0].publicPlayerNumber &&
    account.leaderboardOwnerId === f.accounts[0].leaderboardOwnerId, "IDENTITY_CHANGED");
  const resolved = await createAccountService(f.store).resolve(f.context(winner));
  check(resolved.account.value.accountId === account.accountId, "WINNER_AUTHORITY_INVALID");
  let denied = false;
  try { await createAccountService(f.store).resolve(f.context("probeA")); }
  catch (error) { denied = error.reason === "ActiveDeviceRequired"; }
  check(denied, "SOURCE_NOT_DENIED");
  for (const before of f.accounts) {
    const ledger = (await f.store.read("ledger", before.accountId)).value;
    check(isDeepStrictEqual(ledger.best, {}) && ledger.entries.length === 0 && ledger.active === "", "LEDGER_CHANGED");
  }
  for (const player of ["probeB", "probeC"]) if (player !== winner) {
    const other = await createAccountService(f.store).resolve(f.context(player));
    check(other.account.value.accountId !== account.accountId, "MULTIPLE_ACTIVE_TARGETS");
  }
}
async function run(token, runId, report = console.log, fetcher = fetch, options = {}) {
  const clock = options.clock || Date.now;
  const wait = options.wait || (ms => new Promise(resolve => setTimeout(resolve, ms)));
  const remaining = options.selection === "remaining";
  check(!options.selection || ["all", "remaining"].includes(options.selection), "INVALID_CASE_SELECTION");
  let count = 10;
  if (!remaining) {
  await step("storage-contract", "CHECK_CONTRACT", async () => {
  const contract = adapter(token, runId, "storage-contract", fetcher);
  const contractContext = { ...scope, playerId: "probe", serviceToken: "admin-probe-not-player-auth" };
  const contractStore = createAccountStore(contractContext, contract.api);
  const original = await contractStore.create("account", "atomic", { ...scope, probe: 0 });
  await contractStore.compareExchange(original, { ...scope, probe: 1 });
  let rejected = false;
  try {
    await contract.api.setPrivateCustomItemBatch(projectId, "fs8-account-atomic", { data: [
      { key: "fs_account_v1", value: { ...scope, probe: 2 }, writeLock: original.writeLock },
      { key: "probe_uncommitted", value: true }
    ] });
  } catch (error) { rejected = error.response?.status === 409; }
  const after = await contract.api.getPrivateCustomItems(projectId, "fs8-account-atomic", ["fs_account_v1", "probe_uncommitted"]);
  check(rejected && after.data.results.length === 1 && after.data.results[0].value.probe === 1, "BATCH_ATOMICITY_NOT_CONFIRMED");
  const shared = await contractStore.read("account", "atomic");
  const competing = await Promise.allSettled([2, 3].map(probe => contractStore.compareExchange(shared, { ...scope, probe })));
  check(competing.filter(result => result.status === "fulfilled").length === 1 &&
    competing.filter(result => result.status === "rejected" && result.reason.response?.status === 409).length === 1, "CAS_SINGLE_WINNER_NOT_CONFIRMED");
  report("PASS / LIVE_BATCH_ATOMICITY_AND_CAS_SINGLE_WINNER");
  });
  const baseline = await setup(token, runId, "baseline", fetcher, clock);
  check((await step("baseline", "COMPLETE_TRANSFER", () => baseline.complete("probeB"))).status === "Success", "BASELINE_FAILED");
  count = baseline.live.state.writes;
  check(count > 0 && count <= 24, "UNEXPECTED_WRITE_COUNT");
  await step("baseline", "VERIFY", () => verify(baseline, "probeB"));
  report("PASS / BASELINE_STORAGE_TRANSFER");
  }
  for (const mode of ["before", "after"]) for (let at = 1; at <= count; at++) {
    if (remaining && (mode !== "after" || at !== 10)) continue;
    const caseId = `${mode}-${at}`;
    const f = await setup(token, runId, caseId, fetcher, clock);
    f.live.state.faultAt = at; f.live.state.faultMode = mode;
    try { await step(caseId, "INJECT_FAULT", () => f.complete("probeB")); }
    catch (error) {
      // Unexpected network/HTTP errors retain their original safe diagnostics.
      if (!["INJECTED_BEFORE_WRITE", "INJECTED_RESPONSE_LOSS"].includes(error.code)) throw error;
    }
    check(f.live.state.injected, "FAULT_NOT_REACHED");
    f.live.state.faultAt = 0;
    // Deliberate next probe step, not an SDK/network retry. Respect the real 5s throttle.
    await wait(5100);
    const reply = await step(caseId, "RECOVER", () => f.complete("probeB"));
    check(["Success", "AlreadyCompleted"].includes(reply.status), "RECOVERY_NOT_CONFIRMED");
    await step(caseId, "VERIFY", () => verify(f, "probeB"));
    report(`PASS / STORAGE_RECOVERY_${mode.toUpperCase()}_${at}`);
  }
  const race = await setup(token, runId, "race", fetcher, clock);
  const raceResults = await Promise.allSettled([step("race", "COMPLETE_RACE", () => race.complete("probeB")), step("race", "COMPLETE_RACE", () => race.complete("probeC"))]);
  for (const result of raceResults) if (result.status === "rejected" && ["REQUEST_UNCONFIRMED", "HTTP_FAILURE"].includes(result.reason.code) && result.reason.response?.status !== 409) throw result.reason;
  const account = (await step("race", "READ_TERMINAL", () => race.store.read("account", race.accounts[0].accountId))).value;
  check(account.transfer.status === "Completed" && ["probeB", "probeC"].includes(account.currentPlayerId), "RACE_TERMINAL_UNCONFIRMED");
  await wait(5100);
  await step("race", "RECOVER_WINNER", () => race.complete(account.currentPlayerId));
  const loser = account.currentPlayerId === "probeB" ? "probeC" : "probeB";
  check((await step("race", "REJECT_LOSER", () => race.complete(loser))).status === "InvalidCredential", "LOSER_CREDENTIAL_ACCEPTED");
  await step("race", "VERIFY", () => verify(race, account.currentPlayerId));
  report("PASS / STORAGE_SINGLE_WINNER_RACE");
  const submissionRace = await setup(token, runId, "submission-race", fetcher, clock, false);
  const submissionId = crypto.randomUUID();
  const submissionResults = await Promise.allSettled([step("submission-race", "START_RACE", () => submissionRace.start()),
    step("submission-race", "RESERVE_RACE", () => createAccountService(submissionRace.store, clock).reserveSubmission(submissionRace.context("probeA"), submissionId))]);
  for (const result of submissionResults) if (result.status === "rejected" && ["REQUEST_UNCONFIRMED", "HTTP_FAILURE"].includes(result.reason.code) && result.reason.response?.status !== 409) throw result.reason;
  const racedAccount = (await step("submission-race", "VERIFY", () => submissionRace.store.read("account", submissionRace.accounts[0].accountId))).value;
  check(racedAccount.status === "TransferPending" && !racedAccount.onlineOperation ||
    racedAccount.status === "Active" && racedAccount.onlineOperation?.id === submissionId,
    "SUBMISSION_TRANSFER_FENCE_UNCONFIRMED");
  report("PASS / STORAGE_SUBMISSION_TRANSFER_FENCE / NoLeaderboardWrite=True");
  report(remaining ? "PASS / LIVE_STORAGE_REMAINDER / FaultCases=1 / RaceCases=2 / FullRun=False / ModuleAuthAndRuntime=NOT_TESTED / ExistingAccountsUntouched=True" :
    `PASS / LIVE_STORAGE_PROBE / FaultCases=${count * 2} / ModuleAuthAndRuntime=NOT_TESTED / ExistingAccountsUntouched=True`);
}
async function main(args = process.argv.slice(2)) {
  if (args.length === 0 || args.length === 1 && args[0] === "--plan") {
    console.log("PLAN / verification only / isolated p2v-* Private Game Data / no existing accounts or Leaderboards / no remote calls"); return;
  }
  const remaining = args.includes("--remaining");
  check(args.length === (remaining ? 3 : 2) && new Set(args).size === args.length &&
    args.includes("--run") && args.includes("--confirm-verification-writes"), "EXPLICIT_WRITE_CONFIRMATION_REQUIRED");
  const keyId = process.env.FS_PHASE2_PROBE_KEY_ID, keySecret = process.env.FS_PHASE2_PROBE_KEY_SECRET;
  delete process.env.FS_PHASE2_PROBE_KEY_ID; delete process.env.FS_PHASE2_PROBE_KEY_SECRET;
  const token = await exchange(keyId, keySecret);
  const runId = crypto.randomBytes(16).toString("hex");
  console.log(`RUN / verification / ProbeNamespace=p2v-${runId}-* / DataRetained=True`);
  await run(token, runId, console.log, fetch, { selection: remaining ? "remaining" : "all" });
}
if (require.main === module) main().catch(error => {
  console.error(failureLine(error));
  process.exitCode = 1;
});
module.exports = { adapter, exchange, setup, verify, run, main, failureLine };
