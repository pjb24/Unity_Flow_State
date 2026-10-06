"use strict";
// Read-only inventory. This is not C# compilation, coverage or Unity execution.
const fs = require("node:fs"), path = require("node:path"), crypto = require("node:crypto");
const root = path.resolve(__dirname, "../..");
function mask(source) {
  return source.replace(/@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g,
    text => text.replace(/[^\r\n]/g, " "));
}
function inventory(folder = "EditMode") {
  if (folder !== "EditMode" && folder !== "PlayMode") throw Error("Invalid test folder");
  const result = [];
  for (const filename of fs.readdirSync(path.join(root, "Assets/Tests", folder)).filter(name => name.endsWith("Tests.cs")).sort()) {
    const file = "Assets/Tests/" + folder + "/" + filename, source = fs.readFileSync(path.join(root, file), "utf8"), plain = mask(source);
    const attrs = [...plain.matchAll(/\[(TestCase|Test|UnityTest)\b[^\]]*\]/g)];
    for (const match of plain.matchAll(/\bpublic\s+(?:async\s+)?(?:void|Task(?:<[^>]+>)?|IEnumerator)\s+(\w+)\s*\([^{}]*?\)\s*\{/g)) {
      const prior = attrs.filter(attr => attr.index < match.index);
      if (!prior.length) continue;
      const last = prior.at(-1);
      if (plain.slice(last.index + last[0].length, match.index).trim()) continue;
      const own = [last];
      for (let i = prior.length - 2; i >= 0; i--) {
        const attr = prior[i];
        if (plain.slice(attr.index + attr[0].length, own[0].index).trim()) break;
        own.unshift(attr);
      }
      let end = match.index + match[0].length, depth = 1;
      for (; end < plain.length && depth; end++) { if (plain[end] === "{") depth++; if (plain[end] === "}") depth--; }
      if (depth) throw Error(file + " unbalanced test method");
      const body = source.slice(match.index + match[0].length, end - 1);
      const cases = own.filter(attr => attr[1] === "TestCase").map(attr => source.slice(attr.index, attr.index + attr[0].length));
      const normalized = body.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").replace(/\s+/g, " ").trim();
      result.push({ file, fixture: filename.slice(0, -3), method: match[1], line: source.slice(0, match.index).split("\n").length,
        cases: cases.length ? cases : ["[" + last[1] + "]"], body, normalized,
        hash: crypto.createHash("sha256").update(normalized).digest("hex"),
        assertions: (body.match(/\b(?:Assert|CollectionAssert|StringAssert)\s*\./g) || []).length,
        calls: [...new Set([...body.matchAll(/\b([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)+)\s*\(/g)].map(call => call[1]))]
      });
    }
    const expected = (source.match(/\[TestCase\(/g) || []).length + (source.match(/\[(?:Test|UnityTest)\]/g) || []).length;
    const actual = result.filter(test => test.file === file).reduce((sum, test) => sum + test.cases.length, 0);
    if (expected !== actual) throw Error(`${file}: expected ${expected}, parsed ${actual}`);
  }
  return result;
}
if (require.main === module) {
  const args = process.argv.slice(2), folder = args[0] === "--playmode" ? (args.shift(), "PlayMode") : "EditMode";
  const tests = inventory(folder);
  if (args[0] === "--bodies") {
    for (const test of tests.filter(test => args.slice(1).some(name => test.fixture === name)))
      console.log(`${test.fixture}:${test.line} ${test.method} ${test.cases.join(" ")}\n${test.normalized}`);
  } else if (args[0] === "--names") {
    for (const test of tests) console.log(`${test.fixture}:${test.line} ${test.method} cases=${test.cases.length} asserts=${test.assertions}`);
  } else if (args[0] === "--json") console.log(JSON.stringify(tests));
  else {
    for (const fixture of [...new Set(tests.map(test => test.fixture))]) {
      const subset = tests.filter(test => test.fixture === fixture);
      console.log(`${fixture}: methods=${subset.length} cases=${subset.reduce((sum, test) => sum + test.cases.length, 0)}`);
    }
    const groups = new Map();
    for (const test of tests) { if (!groups.has(test.hash)) groups.set(test.hash, []); groups.get(test.hash).push(test); }
    console.log("Exact body duplicates (attributes/setup may differ):");
    for (const group of groups.values()) if (group.length > 1) console.log(group.map(test => test.fixture + "." + test.method).join(" / "));
    console.log(`TOTAL fixtures=${new Set(tests.map(test => test.fixture)).size} methods=${tests.length} cases=${tests.reduce((sum, test) => sum + test.cases.length, 0)}`);
  }
}
module.exports = { inventory };
