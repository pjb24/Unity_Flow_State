// Offline contract checks. Node VM tests the actual embedded shim, NOT the .NET/Jint host.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const vm = require("node:vm");
const crypto = require("node:crypto");
const { fixture } = require("./account-sdk-double.cjs");
const root = path.resolve(__dirname, "../Modules/FlowStateVerification");
const secret = Buffer.alloc(32, 7).toString("base64"); // Synthetic test key only.
function runtime(f, options = {}) {
  const sandbox = vm.createContext({
    __source(name) {
      assert.match(name, /^[a-z0-9-]+\.js$/);
      return fs.readFileSync(path.resolve(__dirname, "../CloudCode", name), "utf8");
    },
    __random: count => crypto.randomBytes(count).toString("hex"),
    __hmac: (key, text) => crypto.createHmac("sha256", Buffer.from(key, "hex")).update(text).digest("hex"),
    __equals: (left, right) => crypto.timingSafeEqual(Buffer.from(left, "hex"), Buffer.from(right, "hex")),
    __encoding: (value, from, to) => Buffer.from(value, from).toString(to),
    __secret: name => {
      assert.equal(name, "FS_TRANSFER_HMAC_VERIFICATION_V1");
      return options.noSecret ? JSON.stringify({ ok: false, status: 503 }) : JSON.stringify({ ok: true, value: { value: secret } });
    },
    async __service(method, argsJson) {
      const api = Object.hasOwn(f.save, method) ? f.save : f.leaderboard;
      try { return JSON.stringify({ ok: true, value: await api[method](...JSON.parse(argsJson)) }); }
      catch (error) { return JSON.stringify({ ok: false, status: error.response ? error.response.status : 503 }); }
    }
  });
  vm.runInContext(fs.readFileSync(path.join(root, "runtime.js"), "utf8"), sandbox);
  return {
    evaluate: code => vm.runInContext(code, sandbox),
    async call(endpoint, params = {}, context = f.context) {
      return JSON.parse(await sandbox.__invoke(endpoint, JSON.stringify(context), JSON.stringify(params)));
    }
  };
}
async function main() {
  const f = fixture(); const r = runtime(f);
  const native = require("../CloudCode/transfer-cryptography").createTransferCryptography(secret);
  const adapted = r.evaluate(`__load("transfer-cryptography").createTransferCryptography(${JSON.stringify(secret)})`);
  assert.equal(adapted.codeDigest("ABCD-EFGH"), native.codeDigest("ABCD-EFGH"));
  assert.equal(adapted.credentialHmac("test", 1, "ABCDEFGH", "000000007"), native.credentialHmac("test", 1, "ABCDEFGH", "000000007"));
  assert.equal(r.evaluate('Buffer.from("ffffffff", "hex").readUInt32BE(0)'), 4294967295);
  assert.throws(() => r.evaluate('__load("../../outside")'), /Unsupported module/);
  assert.throws(() => r.evaluate('__crypto.timingSafeEqual(Buffer.from("ff", "hex"), Buffer.from("ffff", "hex"))'), /Invalid digest length/);
  assert.equal((await r.call("get-public-player-number")).status, "Success");
  assert.equal((await r.call("get-account-personal-bests")).status, "Success");
  const stage = "fs-stage-stage-001-r1";
  const candidate = { boardId: stage, rulesVersion: 1, submissionId: "11111111-1111-4111-8111-111111111111",
    score: 60000, runDurationMilliseconds: 0, baseDistanceScore: 0, momentumBonus: 0,
    distanceScore: 0, collectibleScore: 0, totalScore: 0, maximumMomentumMultiplier: 1 };
  const submit = (value, context = f.context) => r.call("submit-record", { request: JSON.stringify(value) }, context);
  const me = (context = f.context) => r.call("query-records", { request: JSON.stringify({ boardId: stage, kind: "me", limit: 1 }) }, context);
  assert.equal((await me()).status, "Success");
  assert.equal((await submit(candidate)).status, "Submitted");
  const stageBaseline = await me();
  assert.equal(stageBaseline.status, "Success");
  assert.equal(stageBaseline.entries.length, 1);
  assert.equal(stageBaseline.entries[0].isMe, true);
  assert.equal(stageBaseline.entries[0].score, 60000);
  const savedScores = JSON.stringify([...f.scores]);
  assert.equal((await submit(candidate)).status, "Submitted");
  assert.equal((await submit({ ...candidate, submissionId: "22222222-2222-4222-8222-222222222222", score: 70000 })).status, "Submitted");
  assert.equal(JSON.stringify([...f.scores]), savedScores, "duplicate and worse score retain original row/metadata");
  assert.deepEqual(await me(), stageBaseline);
  const started = await r.call("start-account-transfer");
  assert.equal(started.status, "Success");
  assert.match(started.code, /^[A-Z0-9]{4}-[A-Z0-9]{4}$/);
  assert.match(started.verificationValue, /^\d{9}$/);
  assert.equal((await r.call("get-account-transfer-status")).status, "TransferPending");
  const accountId = f.value("fs8-player-A").accountId;
  const transferId = f.value(`fs8-account-${accountId}`).transfer.transferId;
  assert.equal((await r.call("cancel-account-transfer", { transferId })).status, "Active");
  const next = await r.call("start-account-transfer");
  assert.equal(next.status, "Success");
  const reissued = await r.call("reissue-account-transfer");
  assert.equal(reissued.status, "Success");
  assert.equal((await r.call("get-public-player-number", {}, { ...f.context, playerId: "B" })).status, "Success");
  const completed = await r.call("complete-account-transfer", { code: reissued.code, verificationValue: reissued.verificationValue }, { ...f.context, playerId: "B" });
  assert.equal(completed.status, "Success");
  const b = { ...f.context, playerId: "B" };
  assert.equal((await r.call("get-public-player-number", {}, b)).status, "Success");
  assert.deepEqual(await me(b), stageBaseline, "transfer retains public number, score, acceptedAt, rank and isMe");
  assert.equal((await me()).status, "TransientFailure", "old device loses account authority");
  assert.equal(JSON.stringify([...f.scores]), savedScores);
  assert.equal((await r.call("query-records", { request: JSON.stringify({ boardId: "fs-infinite-v2", kind: "top", limit: 10 }) }, b)).status, "Success");
  assert.equal((await r.call("submit-record", { request: "invalid" }, b)).status, "Rejected");
  const legacy = fixture();
  const reorder = value => {
    if (Array.isArray(value)) return value.map(reorder);
    if (value === null || typeof value !== "object") return value;
    return Object.fromEntries(Object.keys(value).sort().map(key => [key, reorder(value[key])]));
  };
  for (const method of ["getPrivateCustomItems", "getProtectedItems"]) {
    const original = legacy.save[method];
    legacy.save[method] = async (...args) => reorder(await original(...args));
  }
  for (const [board, score] of [[stage, 60000], ["fs-infinite-v2", 10]])
    legacy.scores.set(board + "/old-player", { playerId: "old-player", score,
      metadata: { acceptedAt: 100, submissionId: candidate.submissionId } });
  const legacyRuntime = runtime(legacy);
  assert.equal((await legacyRuntime.call("get-public-player-number")).status, "Success");
  const originalLegacyScores = JSON.stringify([...legacy.scores]);
  for (const board of [stage, "fs-infinite-v2"]) {
    const result = await legacyRuntime.call("query-records", { request: JSON.stringify({ boardId: board, kind: "top", limit: 10 }) });
    assert.equal(result.status, "Success", JSON.stringify(result));
    assert.equal(result.entries.length, 1);
    assert.equal(result.entries[0].isMe, false, "legacy projection does not grant caller authority");
  }
  assert.equal(JSON.stringify([...legacy.scores]), originalLegacyScores);
  const missing = fixture();
  const unavailable = runtime(missing, { noSecret: true });
  assert.equal((await unavailable.call("get-public-player-number")).status, "Success");
  const beforeMissing = JSON.stringify([...missing.privateItems]);
  assert.equal((await unavailable.call("start-account-transfer")).status, "TransientFailure");
  assert.equal(JSON.stringify([...missing.privateItems]), beforeMissing);
  // Client and server must agree on all nine function names. Crypto helpers are never endpoints.
  const transport = fs.readFileSync(path.resolve(__dirname, "../../Assets/Scripts/Runtime/Features/UgsOnlineRecordTransport.cs"), "utf8");
  const entry = fs.readFileSync(path.join(root, "FlowStateVerification.cs"), "utf8");
  const functions = [...entry.matchAll(/CloudCodeFunction\("([^"]+)"\)/g)].map(match => match[1]);
  assert.equal(functions.length, 9);
  for (const fn of functions) assert.ok(transport.includes(`return "${fn}"`));
  assert.ok(transport.includes("CallModuleEndpointAsync<T>"));
  assert.equal(/Instance\.CallEndpointAsync/.test(transport), false);
  const ccmr = path.resolve(__dirname, "../../Assets/CloudCode/FlowStateVerification.ccmr");
  const modulePath = JSON.parse(fs.readFileSync(ccmr, "utf8")).modulePath;
  assert.equal(path.resolve(path.dirname(ccmr), modulePath), path.join(root, "FlowStateVerification.sln"));
  assert.ok(fs.existsSync(path.resolve(path.dirname(ccmr), modulePath)));
  assert.deepEqual(fs.readdirSync(path.dirname(ccmr)).sort(),
    ["FlowStateVerification.ccmr", "FlowStateVerification.ccmr.meta"],
    "only the current Module authoring entry belongs in Assets/CloudCode");
  for (const meta of [ccmr + ".meta", path.dirname(ccmr) + ".meta"])
    assert.match(fs.readFileSync(meta, "utf8"), /^guid: [a-f0-9]{32}$/m);
  const host = fs.readFileSync(path.join(root, "ServerScriptRuntime.cs"), "utf8");
  assert.equal(host.includes(".AllowClr("), false);
  assert.ok(host.includes("context.PlayerId"));
  assert.ok(host.includes("context.ServiceToken"));
  const standardCrypto = fs.readFileSync(path.join(root, "ServerCryptography.cs"), "utf8");
  for (const name of ["RandomNumberGenerator.GetBytes", "HMACSHA256.HashData", "CryptographicOperations.FixedTimeEquals"])
    assert.ok(standardCrypto.includes(name));
  const services = fs.readFileSync(path.join(root, "ServerServices.cs"), "utf8");
  for (const route of ["/private/item-batch", "/protected/item-batch", "/scores/players/", "context.ServiceToken", "AllowAutoRedirect = false"])
    assert.ok(services.includes(route));
  const project = fs.readFileSync(path.join(root, "FlowStateVerification.csproj"), "utf8");
  assert.ok(project.includes('Include="../../CloudCode/*.js"'));
  assert.equal(project.includes("Phase2Verification"), false);
  for (const endpoint of entry.matchAll(/return Run\("([a-z-]+)"/g))
    assert.ok(fs.existsSync(path.resolve(__dirname, "../CloudCode", endpoint[1] + ".js")));
  assert.ok(project.includes('<TargetFramework>net9.0</TargetFramework>'));
  assert.ok(project.includes('Include="Jint" Version="4.16.4"'));
  console.log("PASS module shim: crypto parity, nine endpoint contracts, Stage submit/query/duplicate/worse score, transfer preserves row and denies old device, missing Secret, fixed client routes (offline Node VM; .NET build/runtime unverified)");
}
main().catch(error => { console.error(error); process.exitCode = 1; });
