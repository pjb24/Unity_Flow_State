"use strict";
// Read-only metadata audit. Only fixed Git read commands may spawn a process.
// No Unity/compiler/service invocation, network, extraction, or filesystem writes.
const fs = require("node:fs");
const path = require("node:path");
const crypto = require("node:crypto");
const childProcess = require("node:child_process");

class AuditError extends Error {
  constructor(code) { super(code); this.code = code; }
}
const hash = bytes => crypto.createHash("sha256").update(bytes).digest("hex");
const compare = (a, b) => a < b ? -1 : a > b ? 1 : 0;
function relative(input) {
  if (typeof input !== "string" || !input || input.includes("\0") || /^[\\/]|^[A-Za-z]:/.test(input)) throw new AuditError("PATH_OUTSIDE_SCOPE");
  const parts = input.replace(/\\/g, "/").split("/");
  if (parts.includes("..") || parts.some(part => part.includes(":"))) throw new AuditError("PATH_OUTSIDE_SCOPE");
  const normalized = parts.filter(part => part && part !== ".").join("/");
  if (!normalized) throw new AuditError("PATH_OUTSIDE_SCOPE");
  return normalized;
}
function paths(values) { return [...new Set(values.map(relative))].sort(compare); }
function nul(text) {
  if (typeof text !== "string" || (text && !text.endsWith("\0"))) throw new AuditError("GIT_TRUNCATED_NUL");
  return text ? text.slice(0, -1).split("\0") : [];
}
function stagedChanges(text) {
  const fields = nul(text), rows = [];
  for (let i = 0; i < fields.length;) {
    const status = fields[i++];
    if (!/^(?:[ACDMRTUXB]|[RC]\d+)$/.test(status) || i >= fields.length) throw new AuditError("GIT_STATUS_FORMAT");
    const first = relative(fields[i++]);
    if (/^[RC]/.test(status)) {
      if (i >= fields.length) throw new AuditError("GIT_STATUS_FORMAT");
      rows.push({ status, from: first, path: relative(fields[i++]) });
    } else rows.push({ status, path: first });
  }
  return rows;
}
function ignoreResult(result, queries) {
  if (result.error || ![0, 1].includes(result.status)) throw new AuditError("GIT_IGNORE_FAILED");
  const f = nul(result.stdout);
  if (f.length !== queries.length * 4) throw new AuditError("GIT_IGNORE_FORMAT");
  return queries.map((query, i) => {
    const [source, line, pattern, echoed] = f.slice(i * 4, i * 4 + 4);
    if (query !== echoed || (line && !/^\d+$/.test(line))) throw new AuditError("GIT_IGNORE_FORMAT");
    return { ignored: !!pattern && !pattern.startsWith("!"),
      source: source && !path.isAbsolute(source) && !/^[A-Za-z]:/.test(source) ? source : source ? "<external-ignore>" : null,
      line: line ? Number(line) : null, pattern: pattern || null };
  });
}
function parseJson(text) {
  try { return JSON.parse(text.replace(/^\uFEFF/, "")); }
  catch (_) { throw new AuditError("JSON_INVALID"); }
}
function object(value) { return value !== null && typeof value === "object" && !Array.isArray(value); }
function publicUrl(value) {
  try {
    const u = new URL(value);
    if (!/^https?:$/.test(u.protocol) || u.username || u.password || u.search || u.hash) return "<redacted-url>";
    return u.href;
  } catch (_) { return "<redacted-url>"; }
}
function spec(value) {
  if (typeof value !== "string") return "<invalid-spec>";
  if (/^\d+\.\d+\.\d+(?:[-+][A-Za-z0-9.-]+)?$/.test(value)) return value;
  if (value.startsWith("file:")) {
    try { return "file:" + relative(value.slice(5)); } catch (_) { return "<redacted-local-path>"; }
  }
  return publicUrl(value);
}
function issue(code, target, severity = "error") { return { code, path: target, severity }; }
function graph(manifest, lock) {
  if (!object(manifest) || !object(manifest.dependencies) || !object(lock) || !object(lock.dependencies)) throw new AuditError("PACKAGE_SCHEMA");
  const diagnostics = [], edges = [], nodes = [], deps = lock.dependencies;
  const direct = manifest.dependencies;
  for (const id of Object.keys(direct).sort(compare)) {
    if (!/^[a-z0-9][a-z0-9._-]+$/.test(id) || typeof direct[id] !== "string") throw new AuditError("PACKAGE_SCHEMA");
    if (!Object.hasOwn(deps, id)) diagnostics.push(issue("PACKAGE_DIRECT_MISSING", id));
  }
  for (const id of Object.keys(deps).sort(compare)) {
    const p = deps[id];
    if (!/^[a-z0-9][a-z0-9._-]+$/.test(id) || !object(p) || typeof p.version !== "string" || !object(p.dependencies)) throw new AuditError("PACKAGE_SCHEMA");
    const source = p.source;
    if (!["registry", "builtin", "local", "git", "embedded"].includes(source)) diagnostics.push(issue("PACKAGE_SOURCE_UNKNOWN", id));
    if (["registry", "builtin"].includes(source) && !/^\d+\.\d+\.\d+(?:[-+][A-Za-z0-9.-]+)?$/.test(p.version)) diagnostics.push(issue("PACKAGE_VERSION_UNRESOLVED", id));
    if (source === "registry" && (!p.url || publicUrl(p.url) === "<redacted-url>")) diagnostics.push(issue("PACKAGE_REGISTRY_UNVERIFIED", id));
    if (Object.hasOwn(direct, id)) {
      const requested = direct[id];
      if (requested.startsWith("file:") && !["local", "embedded"].includes(source)) diagnostics.push(issue("PACKAGE_SOURCE_MISMATCH", id));
      else if (/^(?:https?:|git\+|ssh:)/.test(requested) && source !== "git") diagnostics.push(issue("PACKAGE_SOURCE_MISMATCH", id));
      else if (/^\d+\./.test(requested) && !["registry", "builtin"].includes(source)) diagnostics.push(issue("PACKAGE_SOURCE_MISMATCH", id));
      if (requested.replace(/\\/g, "/") !== p.version.replace(/\\/g, "/")) diagnostics.push(issue("PACKAGE_DIRECT_VERSION_DIFFERENCE", id, "finding"));
    }
    nodes.push({ id, direct: Object.hasOwn(direct, id), source, version: spec(p.version),
      registry: source === "registry" ? publicUrl(p.url) : null });
    for (const to of Object.keys(p.dependencies).sort(compare)) {
      if (!/^[a-z0-9][a-z0-9._-]+$/.test(to) || typeof p.dependencies[to] !== "string") throw new AuditError("PACKAGE_SCHEMA");
      const target = deps[to];
      if (!target) diagnostics.push(issue("PACKAGE_TRANSITIVE_MISSING", id + " -> " + to));
      if (target && target.source === "registry" && /^\d+\.\d+\.\d+$/.test(p.dependencies[to]) && /^\d+\.\d+\.\d+$/.test(target.version)) {
        const requestedParts = p.dependencies[to].split(".").map(Number), actualParts = target.version.split(".").map(Number);
        for (let i = 0; i < 3; i++) {
          if (actualParts[i] === requestedParts[i]) continue;
          if (actualParts[i] < requestedParts[i]) diagnostics.push(issue("PACKAGE_TRANSITIVE_BELOW_REQUEST", id + " -> " + to, "finding"));
          break;
        }
      }
      edges.push({ from: id, to, requested: spec(p.dependencies[to]), resolved: target ? spec(target.version) : null,
        versionDifference: !!target && p.dependencies[to] !== target.version });
    }
  }
  const visiting = new Set(), done = new Set();
  function visit(id) {
    if (visiting.has(id)) { diagnostics.push(issue("PACKAGE_CYCLE", id)); return; }
    if (done.has(id) || !deps[id]) return;
    visiting.add(id);
    for (const to of Object.keys(deps[id].dependencies).sort(compare)) visit(to);
    visiting.delete(id); done.add(id);
  }
  Object.keys(deps).sort(compare).forEach(visit);
  return { nodes, edges, diagnostics };
}
const excludedDirectories = new Set(["node_modules", "obj", "bin", "build", "builds", "library", "temp", "logs", ".git", ".vs", ".vscode", "usersettings", "ignore", ".agents", ".codex", ".aws"]);
function sensitive(p) {
  if (p.endsWith(".meta")) return false;
  return /(?:^|\/)(?:\.env(?:\..*)?|.*(?:credentials|secret|receipt|account[-_]data).*)$/i.test(p) || /\/OnlineRecordProductionConfiguration\.cs$/.test(p);
}
function reader(root, io = fs) {
  const fullRoot = path.resolve(root);
  if (io.lstatSync(fullRoot).isSymbolicLink()) throw new AuditError("ROOT_LINK");
  function resolve(p) {
    const clean = relative(p), pieces = clean.split("/");
    for (let i = 1; i <= pieces.length; i++) {
      if (io.lstatSync(path.join(fullRoot, ...pieces.slice(0, i))).isSymbolicLink()) throw new AuditError("SCOPE_LINK");
    }
    return path.join(fullRoot, ...pieces);
  }
  return {
    root: fullRoot, io,
    read(p) {
      if (sensitive(relative(p))) throw new AuditError("SENSITIVE_READ_BLOCKED");
      try { return io.readFileSync(resolve(p)); }
      catch (e) { throw e instanceof AuditError ? e : new AuditError("IO_READ_FAILED"); }
    },
    stat(p) { return io.lstatSync(path.join(fullRoot, relative(p))); },
    walk(start, diagnostics, includeCache = false) {
      const rows = [];
      function visit(p) {
        let s;
        try { s = io.lstatSync(path.join(fullRoot, p)); }
        catch (_) { diagnostics.push(issue("IO_ENUMERATION_FAILED", p)); return; }
        if (s.isSymbolicLink()) { diagnostics.push(issue("SCOPE_LINK", p)); return; }
        if (s.isDirectory()) {
          if (!includeCache && excludedDirectories.has(path.posix.basename(p).toLowerCase())) return;
          rows.push({ path: p, directory: true });
          let names;
          try { names = io.readdirSync(path.join(fullRoot, p)); }
          catch (_) { diagnostics.push(issue("IO_ENUMERATION_FAILED", p)); return; }
          for (const name of names.sort(compare)) visit(p + "/" + name);
        } else if (s.isFile()) rows.push({ path: p, directory: false });
        else diagnostics.push(issue("UNSUPPORTED_FILE_KIND", p));
      }
      const clean = relative(start);
      // Validate parents too; a scoped directory must not hide a parent junction.
      try { resolve(clean); } catch (e) { diagnostics.push(issue(e.code || "IO_ENUMERATION_FAILED", clean)); return rows; }
      visit(clean);
      return rows;
    }
  };
}
function gitReader(root) {
  return (args, input) => {
    const allowed = [
      ["rev-parse", "HEAD"], ["ls-files", "-z"], ["ls-tree", "-r", "--name-only", "-z", "HEAD"],
      ["ls-files", "--others", "--exclude-standard", "-z"],
      ["diff", "--no-ext-diff", "--no-textconv", "--cached", "--name-status", "-z"],
      ["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"]
    ];
    if (!allowed.some(command => JSON.stringify(command) === JSON.stringify(args))) throw new AuditError("GIT_COMMAND_BLOCKED");
    return childProcess.spawnSync("git", ["--no-optional-locks", "-c", "core.fsmonitor=false", "-C", root, ...args],
      { input, encoding: "utf8", windowsHide: true, maxBuffer: 64 * 1024 * 1024 });
  };
}
function requiredGit(git, args) {
  const r = git(args);
  if (r.error || r.status !== 0) throw new AuditError("GIT_READ_FAILED");
  return r.stdout;
}
function gitSnapshot(git) {
  return { head: requiredGit(git, ["rev-parse", "HEAD"]).trim(),
    tracked: paths(nul(requiredGit(git, ["ls-files", "-z"]))),
    headPaths: paths(nul(requiredGit(git, ["ls-tree", "-r", "--name-only", "-z", "HEAD"]))),
    untracked: paths(nul(requiredGit(git, ["ls-files", "--others", "--exclude-standard", "-z"]))),
    staged: stagedChanges(requiredGit(git, ["diff", "--no-ext-diff", "--no-textconv", "--cached", "--name-status", "-z"])) };
}
function caseCollisions(values) {
  const groups = new Map(), collisions = [];
  for (const p of paths(values)) {
    const key = p.toLowerCase();
    if (groups.has(key)) collisions.push({ first: groups.get(key), second: p });
    else groups.set(key, p);
  }
  return collisions;
}
function metaIndex(files, read) {
  const map = new Map(), diagnostics = [], entries = [];
  for (const p of paths(files).filter(p => p.endsWith(".meta"))) {
    const text = read(p).toString("utf8"), m = text.match(/^guid:\s*(\S+)\s*$/m);
    if (!m || !/^[a-fA-F0-9]{32}$/.test(m[1])) { diagnostics.push(issue("META_GUID_INVALID", p)); continue; }
    const entry = { path: p.slice(0, -5), meta: p, guid: m[1].toLowerCase(), metaSha256: hash(Buffer.from(text)), folder: /^folderAsset:\s*yes/m.test(text) };
    if (map.has(entry.guid)) diagnostics.push(issue("META_GUID_DUPLICATE", p));
    else map.set(entry.guid, entry);
    entries.push(entry);
  }
  return { map, entries, diagnostics };
}
function references(text, from, index, jsonMode = false) {
  const values = [], normalized = jsonMode ? text.replace(/\\"/g, '"') : text;
  const pattern = jsonMode ? /"guid"\s*:\s*"([^"\s]+)"/g : /\bguid:\s*([^\s,}]+)/g;
  for (const m of normalized.matchAll(pattern)) {
    const guid = m[1].toLowerCase();
    const builtin = /^0{16}[def]0{15}$/.test(guid);
    let status;
    if (!/^[a-f0-9]{32}$/.test(guid)) status = "invalid";
    else if (/^0{32}$/.test(guid)) status = "null";
    else if (builtin) status = "builtin";
    else status = index.has(guid) ? "resolved" : "unresolved";
    values.push({ from, guid: /^[a-f0-9]{32}$/.test(guid) ? guid : "<invalid-guid>", status,
      target: index.has(guid) ? index.get(guid).path : null });
  }
  return values;
}
function referenceLimits(file, bytes) {
  const reasons = [];
  if (bytes.includes(0) || /\.(ttf|otf|png|jpg|dll|fbx|wav|mp3|ogg)$/i.test(file)) reasons.push("BINARY_INTERPRETATION_REQUIRED");
  if (/\.(cs|shader|hlsl|cginc)$/i.test(file) && /Resources\s*\.\s*(?:Load|LoadAll)|Shader\s*\.\s*Find|Type\s*\.\s*GetType|Assembly\s*\.\s*Load|Instantiate\s*[<(]/.test(bytes.toString("utf8"))) reasons.push("NAME_OR_DYNAMIC_REFERENCE");
  return { path: file, reasons, unused: false, independentRestore: false };
}
function classification(input) {
  const known = ["original", "modified", "derived", "project", "mixed", "unknown"];
  if (!object(input) || !known.includes(input.kind)) throw new AuditError("POLICY_SCHEMA");
  const reasons = [];
  for (const key of ["sourceVerified", "rightsVerified", "metaReproducible", "importReproducible"])
    if (input[key] !== true) reasons.push(key);
  if (input.kind !== "original") reasons.push("not-unmodified-original");
  if (input.projectSettings === true) reasons.push("project-settings");
  if (input.requiresUserDecision === true && input.userDecision !== "exclude-candidate") reasons.push("user-decision-pending");
  return { direction: reasons.length ? "preserve-pending" : "untracked-candidate", reasons, applied: false, rightsGranted: false };
}
const requiredFields = ["asset_id", "record_kind", "product", "version", "official_source", "acquisition", "license", "usage_conditions", "notices", "evidence", "original_storage", "import_path", "modifications", "purpose", "game_usage", "references", "meta_import", "git_boundary", "restore", "lifecycle", "verification", "status", "owner_review"];
function registrationTemplate(text) {
  const found = [...text.matchAll(/^\| ([a-z_]+) \|/gm)].map(m => m[1]);
  return { missing: requiredFields.filter(field => !found.includes(field)), duplicates: found.filter((v, i) => found.indexOf(v) !== i) };
}
function collect(root, adapters = {}) {
  const io = adapters.fs || fs, git = adapters.git || gitReader(path.resolve(root)), disk = reader(root, io);
  const diagnostics = [], snapshot = gitSnapshot(git), initialIndex = hash(disk.read(".git/index"));
  const roots = ["Assets", "Packages", "ProjectSettings", "Tools", "UGS"], rows = [];
  for (const p of roots) rows.push(...disk.walk(p, diagnostics));
  for (const p of [".gitignore", "package.json", "package-lock.json"]) {
    try { disk.stat(p); rows.push({ path: p, directory: false }); }
    catch (e) {
      // Absence is optional; permissions or partial reads are not a clean absence.
      if (e.code !== "ENOENT") diagnostics.push(issue("IO_STAT_FAILED", p));
    }
  }
  const rowPaths = new Set(rows.map(r => r.path));
  const scoped = p => roots.some(root => p === root || p.startsWith(root + "/")) || [".gitignore", "package.json", "package-lock.json"].includes(p);
  for (const p of snapshot.tracked.filter(scoped)) if (!rowPaths.has(p)) {
    const excluded = p.split("/").some(part => excludedDirectories.has(part.toLowerCase()));
    diagnostics.push(issue(excluded ? "TRACKED_EXCLUDED_SCOPE" : "TRACKED_PATH_MISSING", p, excluded ? "finding" : "error"));
    rows.push({ path: p, directory: false, missingOrExcluded: true });
  }
  rows.sort((a, b) => compare(a.path, b.path));
  for (const pair of caseCollisions([...rows.map(r => r.path), ...snapshot.tracked])) diagnostics.push(issue("PATH_CASE_COLLISION", pair.first + " / " + pair.second));
  const queries = rows.map(r => r.path + (r.directory ? "/" : ""));
  const ignores = ignoreResult(git(["check-ignore", "--no-index", "-v", "-z", "--non-matching", "--stdin"], queries.join("\0") + "\0"), queries);
  for (let i = 0; i < rows.length; i++) Object.assign(rows[i], { indexTracked: snapshot.tracked.includes(rows[i].path), headTracked: snapshot.headPaths.includes(rows[i].path), ignore: ignores[i] });
  const manifest = parseJson(disk.read("Packages/manifest.json").toString("utf8")), lock = parseJson(disk.read("Packages/packages-lock.json").toString("utf8")), packages = graph(manifest, lock);
  diagnostics.push(...packages.diagnostics);
  const cache = new Map(), cacheRoots = [], packageMeta = [];
  try {
    // Inspect only metadata-discovered package roots, never the rest of Library.
    disk.read("Packages/manifest.json");
    const cachePath = "Library/PackageCache";
    if (disk.stat("Library").isSymbolicLink() || disk.stat(cachePath).isSymbolicLink()) throw new AuditError("SCOPE_LINK");
    for (const name of io.readdirSync(path.join(disk.root, cachePath)).sort(compare)) {
      const p = relative(cachePath + "/" + name);
      if (disk.stat(p).isSymbolicLink()) throw new AuditError("SCOPE_LINK");
      const j = parseJson(disk.read(p + "/package.json").toString("utf8"));
      if (!object(j) || typeof j.name !== "string" || !/^[a-z0-9][a-z0-9._-]+$/.test(j.name) || typeof j.version !== "string") throw new AuditError("PACKAGE_CACHE_SCHEMA");
      if (cache.has(j.name)) diagnostics.push(issue("PACKAGE_CACHE_DUPLICATE", j.name));
      cache.set(j.name, { root: p, version: j.version, dependencies: j.dependencies || {} });
    }
    for (const node of packages.nodes) {
      const c = cache.get(node.id);
      if (!c) { diagnostics.push(issue("PACKAGE_CACHE_MISSING", node.id, "finding")); continue; }
      if (c.version !== lock.dependencies[node.id].version) diagnostics.push(issue("PACKAGE_CACHE_VERSION", node.id));
      const normalized = v => JSON.stringify(Object.entries(v).sort(([a], [b]) => compare(a, b)));
      if (normalized(c.dependencies) !== normalized(lock.dependencies[node.id].dependencies)) diagnostics.push(issue("PACKAGE_CACHE_DEPENDENCIES", node.id));
      cacheRoots.push(c.root);
      packageMeta.push(...disk.walk(c.root, diagnostics, true).filter(r => r.path.endsWith(".meta")).map(r => r.path));
    }
  } catch (e) {
    diagnostics.push(issue(e.code || "PACKAGE_CACHE_READ_FAILED", "Library/PackageCache"));
  }
  const actualFiles = rows.filter(r => !r.directory && !r.missingOrExcluded).map(r => r.path);
  const allFiles = paths([...actualFiles, ...packageMeta]);
  const metas = metaIndex(allFiles, p => disk.read(p)); diagnostics.push(...metas.diagnostics);
  const assetPaths = new Set(rows.filter(r => !r.missingOrExcluded).map(r => r.path));
  for (const entry of metas.entries) {
    if (entry.path.startsWith("Assets/")) {
      if (!assetPaths.has(entry.path)) diagnostics.push(issue("ORPHAN_META", entry.meta, "finding"));
    } else {
      try { disk.stat(entry.path); } catch (_) { diagnostics.push(issue("ORPHAN_META", entry.meta, "finding")); }
    }
  }
  for (const r of rows.filter(r => r.path.startsWith("Assets/") && !r.path.endsWith(".meta") && !r.missingOrExcluded)) {
    if (!assetPaths.has(r.path + ".meta")) diagnostics.push(issue("ASSET_META_MISSING", r.path, "finding"));
  }
  const serialized = /\.(unity|prefab|mat|asset|controller|overrideController|anim|playable|renderTexture|lighting|physicMaterial)$/i;
  const queue = actualFiles.filter(p => /^(Assets|ProjectSettings)\//.test(p) && serialized.test(p));
  const seen = new Set(), refs = [], limits = [], sourceHashes = [];
  while (queue.length) {
    const p = queue.shift(); if (seen.has(p)) continue; seen.add(p);
    const bytes = disk.read(p); sourceHashes.push({ path: p, sha256: hash(bytes) });
    if (bytes.includes(0)) { limits.push(referenceLimits(p, bytes)); continue; }
    const found = references(bytes.toString("utf8"), p, metas.map); refs.push(...found);
    for (const ref of found) if (ref.status === "resolved" && serialized.test(ref.target)) queue.push(ref.target);
  }
  for (const p of actualFiles.filter(p => p.startsWith("Assets/") && !sensitive(p))) {
    if (/\.(cs|ttf|otf|png|jpg|dll|fbx|wav|mp3|ogg)$/i.test(p)) {
      const bytes = disk.read(p), limit = referenceLimits(p, bytes);
      if (limit.reasons.length) limits.push(limit);
    }
    if (/\.shadergraph$/.test(p)) refs.push(...references(disk.read(p).toString("utf8"), p, metas.map, true));
    if (p.endsWith(".meta")) {
      // The top-level guid declares identity, not an importer reference.
      const text = disk.read(p).toString("utf8").replace(/^guid:.*$/m, "");
      refs.push(...references(text, p, metas.map));
    }
  }
  for (const ref of refs) if (["unresolved", "invalid"].includes(ref.status)) diagnostics.push(issue(ref.status === "invalid" ? "REFERENCE_GUID_INVALID" : "REFERENCE_UNRESOLVED", ref.from + " -> " + ref.guid, "finding"));
  const templatePath = "AI/99_Templates/ASSET_DEPENDENCY_TEMPLATE.md";
  let templateStatus = { missing: requiredFields.slice(), duplicates: [] };
  try { templateStatus = registrationTemplate(disk.read(templatePath).toString("utf8")); }
  catch (_) { diagnostics.push(issue("REGISTRATION_TEMPLATE_MISSING", templatePath)); }
  if (templateStatus.missing.length || templateStatus.duplicates.length) diagnostics.push(issue("REGISTRATION_TEMPLATE_FIELDS", templatePath));
  const baselinePath = adapters.baselinePath || null;
  let baselineCount = 0;
  if (baselinePath) try {
    const baseline = parseJson(disk.read(baselinePath).toString("utf8"));
    for (const f of baseline.files) {
      if (hash(disk.read(f.path)) !== f.sha256) diagnostics.push(issue("BASELINE_HASH_CHANGED", relative(f.path)));
      baselineCount++;
    }
  } catch (e) { diagnostics.push(issue(e.code || "BASELINE_MISSING", baselinePath)); }
  const finalIndex = hash(disk.read(".git/index"));
  if (initialIndex !== finalIndex) diagnostics.push(issue("INDEX_CHANGED_DURING_AUDIT", ".git/index"));
  diagnostics.sort((a, b) => compare(a.code + a.path, b.code + b.path));
  refs.sort((a, b) => compare(a.from + a.guid, b.from + b.guid));
  const collectionComplete = !diagnostics.some(d => d.severity === "error");
  const ok = diagnostics.length === 0;
  return { schemaVersion: 1, ok, collectionComplete, boundaryComplete: false, independentRestore: false,
    scope: { roots, cacheRoots, exclusions: [...excludedDirectories].sort(compare), sensitiveBodiesRead: false,
      referenceAnalysis: "project serialized closure + ShaderGraph/importer GUIDs + project source/name/binary indicators; no Unity load/type/fileID interpretation or exhaustive package dynamic analysis",
      baselinePath },
    git: { head: snapshot.head, trackedCount: snapshot.tracked.length, staged: snapshot.staged, untracked: snapshot.untracked,
      indexUnchanged: initialIndex === finalIndex },
    summary: { actualFiles: actualFiles.length, directories: rows.filter(r => r.directory).length,
      ignoredFiles: rows.filter(r => !r.directory && r.ignore.ignored).length,
      packages: packages.nodes.length, packageEdges: packages.edges.length, metaCount: metas.entries.length,
      serializedFiles: seen.size, referenceOccurrences: refs.length, unresolvedOccurrences: refs.filter(r => r.status === "unresolved").length,
      baselineHashes: baselineCount },
    inventory: rows, packages, references: refs, limits, sourceHashes, templateStatus,
    // No owner/rights/meta decision can be inferred from a folder or ignore rule.
    ownership: rows.filter(r => r.path.startsWith("Assets/") && !r.directory && !r.path.endsWith(".meta"))
      .map(r => ({ path: r.path, ...classification({ kind: "unknown" }) })), diagnostics };
}
function cli(args, adapters = {}) {
  let root = path.resolve(__dirname, "../..");
  const options = { ...adapters };
  try {
    for (let i = 0; i < args.length; i++) {
      if (args[i] === "--root" && i + 1 < args.length) root = args[++i];
      else if (args[i] === "--baseline" && i + 1 < args.length) options.baselinePath = relative(args[++i]);
      else if (args[i] !== "--check") throw new AuditError("CLI_ARGUMENT");
    }
    const report = collect(root, options);
    return { exitCode: report.ok ? 0 : report.collectionComplete ? 1 : 2, report };
  } catch (e) {
    return { exitCode: 2, report: { schemaVersion: 1, ok: false, collectionComplete: false, boundaryComplete: false,
      independentRestore: false, diagnostics: [issue(e instanceof AuditError ? e.code : "COLLECTION_FAILED", "<collection>")] } };
  }
}
if (require.main === module) {
  const result = cli(process.argv.slice(2));
  process.stdout.write(JSON.stringify(result.report, null, 2) + "\n");
  process.exitCode = result.exitCode;
}
module.exports = { AuditError, relative, paths, nul, stagedChanges, ignoreResult, parseJson, publicUrl, spec, graph,
  reader, gitReader, gitSnapshot, caseCollisions, metaIndex, references, referenceLimits, classification, registrationTemplate, requiredFields, collect, cli };
