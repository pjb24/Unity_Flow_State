"use strict";
// Local deployment configuration only. Never print identifier values.
const fs = require("node:fs"), path = require("node:path");
function validateScope(scope) {
  const uuid = /^[a-f0-9]{8}-(?:[a-f0-9]{4}-){3}[a-f0-9]{12}$/i;
  if (!scope || scope.environmentName !== "production" || !uuid.test(scope.projectId) || !uuid.test(scope.environmentId) ||
      scope.projectId === scope.environmentId) throw Error("PRODUCTION_SCOPE_INVALID");
  return Object.freeze({ projectId: scope.projectId, environmentId: scope.environmentId, environmentName: "production" });
}
function parseScope(config, project, verification) {
  const value = name => {
    const matches = [...config.replace(/\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "").matchAll(
      new RegExp('public\\s+const\\s+string\\s+' + name + '\\s*=\\s*"([^"\\r\\n]*)"\\s*;', 'g'))];
    if (matches.length !== 1) throw Error("PRODUCTION_SCOPE_INVALID");
    return matches[0][1];
  };
  const scope = validateScope({ projectId: value("ProjectId"), environmentId: value("EnvironmentId"), environmentName: value("EnvironmentName") });
  if (value("AuthenticationProfile") !== "flow-state-production" ||
      project.match(/^\s*cloudProjectId:\s*(\S+)\s*$/m)?.[1] !== scope.projectId ||
      verification.environmentId === scope.environmentId) throw Error("PRODUCTION_SCOPE_MISMATCH");
  return scope;
}
function loadScope() {
  const root = path.resolve(__dirname, "../..");
  const read = file => fs.readFileSync(path.join(root, file), "utf8").replace(/^\uFEFF/, "");
  return parseScope(read("Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs"),
    read("ProjectSettings/ProjectSettings.asset"), require("./verification-scope.cjs"));
}
module.exports = { validateScope, parseScope, loadScope };
