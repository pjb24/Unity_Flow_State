// Source contracts, not .NET compilation or timing tests. No network calls.
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const { duplicateMembers } = require("./csharp-source-signatures.cjs");
const read = name => fs.readFileSync(path.resolve(__dirname, "../Modules/FlowStateVerification", name + ".cs"), "utf8");
const services = read("ServerServices"), runtime = read("ServerScriptRuntime"), entry = read("FlowStateVerification");
assert.match(services, /RequestBudgetMilliseconds = 5000;/);
assert.equal((services.match(/new CancellationTokenSource\(/g) || []).length, 1, "one invocation deadline, never one per HTTP call");
assert.ok(services.includes("private static readonly HttpClient SharedHttp"));
assert.equal((services.match(/new HttpClient\(/g) || []).length, 1);
assert.ok(services.includes("PooledConnectionLifetime = TimeSpan.FromMinutes(5)"));
assert.ok(services.includes('request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceToken)'));
assert.ok(!services.includes("DefaultRequestHeaders.Authorization"));
assert.ok(!services.includes("SharedHttp.Dispose()"));
assert.ok(runtime.includes('accountResponse["serverElapsedMilliseconds"] = services.ElapsedMilliseconds'));
assert.ok(runtime.includes('accountResponse["serverServiceCalls"] = services.ServiceCalls'));
assert.ok(services.includes("RequestBudgetMilliseconds - (int)_clock.ElapsedMilliseconds"));
const call = services.slice(services.indexOf("internal string Call("), services.indexOf("public void Dispose()"));
assert.ok(call.indexOf("ThrowIfExpired();") < call.indexOf("JArray.Parse"));
assert.ok(call.includes("if (remaining <= 0) return Failure(503)"));
assert.match(call, /SendAsync\([\s\S]*?DeadlineToken\)/);
assert.ok(call.includes("ReadAsStreamAsync(DeadlineToken)"));
assert.ok(call.includes("ReadAsync(buffer.AsMemory(), DeadlineToken)"));
assert.ok(call.lastIndexOf("ThrowIfExpired();") > call.indexOf("JToken.Parse(json)"));
assert.ok(services.includes("_deadline.Dispose()"));
assert.ok(runtime.includes(".CancellationToken(services.DeadlineToken)"));
assert.ok(runtime.includes(".TimeoutInterval(TimeSpan.FromMilliseconds(ServerServices.RequestBudgetMilliseconds))"));
assert.ok(runtime.includes("getSecret(name, services.RemainingMilliseconds)"));
assert.match(entry, /\(name, remainingMilliseconds\)[\s\S]*?WaitAsync\(TimeSpan.FromMilliseconds\(remainingMilliseconds\)\)/);
assert.equal(/FromSeconds\((5|15)\)|12000|15000/.test(runtime + services + entry), false, "no independent or longer server budgets");
assert.ok(runtime.lastIndexOf("services.ThrowIfExpired();", runtime.indexOf("return response;")) > runtime.indexOf("DeserializeObject<object>(result)"));
assert.equal((entry.match(/CloudCodeFunction\(/g) || []).length, 9);
for (const source of [services, runtime, entry]) {
  assert.deepEqual(duplicateMembers(source), []);
  const plain = source.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").replace(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/g, '""');
  const stack = [], pairs = { ")": "(", "}": "{", "]": "[" };
  for (const char of plain) { if ("({[".includes(char)) stack.push(char); if (pairs[char]) assert.equal(stack.pop(), pairs[char]); }
  assert.deepEqual(stack, []);
}
console.log("PASS server source budget: one 5000ms invocation deadline, HTTP/body/Secret/JS share remaining time, expired result rejected; .NET/Jint timing NOT EXECUTED");
