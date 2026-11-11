"use strict";
// User-run offline capture. Reads existing local save / Dashboard exports only.
const fs = require("node:fs"), path = require("node:path");
const { loadScope } = require("./production-scope.cjs");
const { compare, validServer } = require("./compare-production-snapshots.cjs");
const privateRoot = path.resolve(__dirname, "../../Ignore/Phase5Private");
function assemble(mode, input, scope, readJson) {
  if (mode === "save") {
    if (compare("restart", input, input, scope) !== "PRESERVED") throw Error();
    return input;
  }
  if (!["server", "server-detached"].includes(mode) || !input || typeof input.accountFile !== "string" || !input.rowFiles) throw Error();
  const readValue = p => {
    const value = readJson(p);
    return value && Object.hasOwn(value, "value") ? value.value : value;
  };
  const snapshot = { ...scope, account: readValue(input.accountFile), rows: {} };
  for (const board of ["fs-stage-stage-001-r1", "fs-infinite-v2"]) {
    if (!Object.hasOwn(input.rowFiles, board)) throw Error();
    snapshot.rows[board] = input.rowFiles[board] === null ? null : readValue(input.rowFiles[board]);
  }
  for (const [field, key] of [["sourceBinding", "sourceBindingFile"], ["targetBinding", "targetBindingFile"]])
    if (input[key]) snapshot[field] = readValue(input[key]);
  if (mode === "server-detached" ? !validServer(snapshot, scope, true) :
    compare("server-restart", snapshot, snapshot, scope) !== "PRESERVED") throw Error();
  return snapshot;
}
function main(args = process.argv.slice(2), report = console.log) {
  try {
    if (args.length !== 3) throw Error();
    const [mode, inputPath, outputName] = args;
    // A basename, never an arbitrary output path. Never overwrites a prior baseline.
    if (!/^[a-zA-Z0-9_-]+\.json$/.test(outputName)) throw Error();
    const readJson = p => JSON.parse(fs.readFileSync(p, "utf8").replace(/^\uFEFF/, ""));
    const manifest = readJson(inputPath), base = path.dirname(path.resolve(inputPath));
    const snapshot = assemble(mode, manifest, loadScope(), p => readJson(path.resolve(base, p)));
    fs.mkdirSync(privateRoot, { recursive: true });
    fs.writeFileSync(path.join(privateRoot, outputName), JSON.stringify(snapshot), { flag: "wx", mode: 0o600 });
    report("PASS / PRODUCTION_CAPTURE / PRIVATE_BASELINE_CREATED");
    return true;
  } catch (_) { report("FAIL / PRODUCTION_CAPTURE / INPUT_OR_OUTPUT_INVALID"); return false; }
}
if (require.main === module && !main()) process.exitCode = 1;
module.exports = { assemble };
