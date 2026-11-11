"use strict";
// Read-only build preparation. Does not load Unity, compile, run tests or access services.
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path"), crypto = require("node:crypto");
const { execFileSync } = require("node:child_process");
const { parseScope } = require("./production-scope.cjs");
const root = path.resolve(__dirname, "../..");
const read = p => fs.readFileSync(path.join(root, p), "utf8").replace(/^\uFEFF/, "");
const integer = (s, name) => Number(s.match(new RegExp("^\\s*" + name + ": (\\d+)\\s*$", "m"))?.[1]);
const requireFact = (condition, label) => assert.ok(condition, label);
const report = label => console.log("PASS / PHASE5_BUILD_STATIC / " + label);
function main() {
  const profilePath = "Assets/Settings/Build Profiles/Windows-production.asset";
  const profile = read(profilePath), settings = read("ProjectSettings/ProjectSettings.asset");
  requireFact(integer(profile, "m_BuildTarget") === 19 && integer(profile, "m_Subtarget") === 2 && integer(profile, "m_Architecture") === 0, "Windows x64 Player profile");
  requireFact(integer(profile, "m_HasScriptingDefines") === 1 && /- FLOW_STATE_PRODUCTION\s/.test(profile), "Production define");
  for (const option of ["m_Development", "m_AllowDebugging", "m_ConnectProfiler", "m_BuildWithDeepProfilingSupport"])
    requireFact(integer(profile, option) === 0, "Release options");
  requireFact(integer(profile, "m_OverrideGlobalSceneList") === 0 && /m_Settings: \[\]/.test(profile), "Inherited scenes and Player settings");
  requireFact(integer(settings, "defaultScreenWidth") === 1920 && integer(settings, "defaultScreenHeight") === 1080 && integer(settings, "fullscreenMode") === 3, "1920x1080 Windowed default");
  requireFact(!/Standalone:[^\r\n]*FLOW_STATE_PRODUCTION/.test(settings), "No global Production define");
  report("WindowsX64Player / ProductionDefine / Release / Default1920x1080Windowed");

  const configuration = read("Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs");
  // Editor selector is not a Player environment switch. Validate private build scope independently.
  const scope = parseScope(configuration, settings,
    require("./verification-scope.cjs"));
  const auth = read("Assets/Scripts/Runtime/Features/UgsOnlineAuthenticationGateway.cs");
  requireFact(auth.includes("options.SetEnvironmentName(OnlineRecordConfiguration.ActiveEnvironmentName)") && auth.includes("Application.cloudProjectId != OnlineRecordConfiguration.ActiveProjectId") && auth.includes("IsAllowedAuthenticationProfile(_profile)"), "Explicit initialization and profile guards");
  requireFact(execFileSync("git", ["check-ignore", "Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs"], { cwd: root, encoding: "utf8" }).trim().length > 0, "Private configuration ignored");
  requireFact(read("Assets/Scripts/Runtime/Features/LocalRecordRepository.cs").includes("_activeScope.Matches(OnlineDataScope.CreateConfigured())"), "Online save scope");
  report("PrivateProductionScopeMatched / ExplicitEnvironment / ScopedSave / NoIdentifierOutput");
  const editor = JSON.parse(read("ProjectSettings/Packages/com.unity.services.core/Settings.json"));
  console.log("INFO / PHASE5_BUILD_STATIC / EditorSelectorMatchesProduction=" + (editor.EnvironmentName === "production" && editor.EnvironmentId === scope.environmentId) + " / PlayerUsesProfileDefine");

  const buildScenes = read("ProjectSettings/EditorBuildSettings.asset");
  const scenes = [...buildScenes.matchAll(/- enabled: 1\r?\n\s+path: ([^\r\n]+)\r?\n\s+guid: ([a-f0-9]{32})/g)];
  requireFact(scenes.length === 1 && scenes[0][1] === "Assets/Scenes/SampleScene.unity", "Production scene list");
  const scene = read(scenes[0][1]);
  requireFact(read(scenes[0][1] + ".meta").includes("guid: " + scenes[0][2]), "Scene GUID");
  const documents = scene.split(/^--- !u!/m).slice(1);
  const ids = new Set(documents.map(d => d.match(/^\d+ &(-?\d+)/)?.[1]));
  function component(file) {
    const guid = read(file + ".meta").match(/^guid: ([a-f0-9]{32})/m)[1];
    const matches = documents.filter(d => d.includes("guid: " + guid + ","));
    requireFact(matches.length === 1, "Unique scene component");
    return matches[0];
  }
  const game = component("Assets/Scripts/Runtime/Systems/GameSystem.cs");
  for (const field of ["_runtimeDataSystem", "_uiManagementSystem", "_playerInputSystem", "_uiInputSystem", "_settingsSystem", "_playerMovementSystem", "_playerControllerSystem", "_collisionSystem", "_stageSystem", "_infiniteModeSystem", "_timerSystem", "_resultSystem", "_cameraSystem", "_cameraFollow"])
    requireFact(ids.has(game.match(new RegExp("\\s" + field + ": \\{fileID: (-?\\d+)\\}"))?.[1]), "GameSystem reference " + field);
  const ui = component("Assets/Scripts/Runtime/Systems/UIManagementSystem.cs");
  for (const field of ["_onlineRecoveryNoticePanel", "_onlineRecoveryNoticeConfirmButton", "_onlineRecoveryNoticeCancelButton", "_settingsRecoveryNoticeButton"])
    requireFact(ids.has(ui.match(new RegExp("\\s" + field + ": \\{fileID: (-?\\d+)\\}"))?.[1]), "Notice reference " + field);
  report("SingleSampleScene / GUID / GameSystemReferences / NoticeReferences");

  for (const file of ["Assets/Tests/EditMode/FlowState.EditModeTests.asmdef", "Assets/Tests/PlayMode/FlowState.PlayModeTests.asmdef"])
    requireFact(JSON.parse(read(file)).optionalUnityReferences.includes("TestAssemblies"), "Test assembly marking");
  const editorFiles = execFileSync("git", ["ls-files", "Assets/Editor/*.cs", "Assets/Tests/**/*.cs"], { cwd: root, encoding: "utf8" }).trim().split(/\r?\n/);
  for (const file of editorFiles) {
    if (!fs.existsSync(path.join(root, file + ".meta"))) continue;
    const guid = read(file + ".meta").match(/^guid: ([a-f0-9]{32})/m)?.[1];
    requireFact(!guid || !scene.includes("guid: " + guid + ","), "No Editor/Test scene script");
  }
  requireFact(!fs.existsSync(path.join(root, "Assets/StreamingAssets")), "No arbitrary StreamingAssets payload");
  const source = read("Assets/Scripts/Runtime/Systems/GameSystem.cs");
  const playerSource = source.replace(/^#if UNITY_EDITOR\r?\n[\s\S]*?^#endif\r?\n/gm,
    block => block.includes("#else") ? block.split(/#else\r?\n/)[1].replace(/#endif\r?\n$/, "") : "");
  requireFact(!/TestMemorySaveFileStore|_hasIsolatedLocalState|UseIsolatedRecordsForPlayModeTests|UnityEditor\./.test(playerSource), "No Editor isolation branch in Player");
  for (const file of ["UgsOnlineAuthenticationGateway.cs", "UgsOnlineRecordTransport.cs"])
    requireFact(!/Debug\.Log(?:Warning|Error)?\([^;]*(?:error\.Message|error\.ToString\(|AccessToken|SessionToken)/s.test(read("Assets/Scripts/Runtime/Features/" + file)), "No raw authentication/transport secret log");
  report("TestAssembliesMarked / NoEditorTestSceneScript / EditorIsolationExcluded / AuthenticationLogsSanitized");
  const hash = crypto.createHash("sha256").update(fs.readFileSync(path.join(root, "Assets/Scripts/Runtime/Systems/GameSystem.cs"))).digest("hex").toUpperCase();
  if (hash !== "E8CD3349BC93AAA9B0A0E7977D71E172B33D2AE3A7123096134AAD8B03B56199") {
    console.log("PENDING / PHASE5_BUILD_STATIC / SourceChangedSinceLatestUserRegression / UserUnityRegressionAndPlayerRebuildRequired");
    process.exitCode = 2;
  } else report("Step6DualProfileRegressionGameSystemHashMatched / PlayerRebuildStillRequired / NoUnityBuildOrTestRunnerOrRemoteRequests");
}
try { main(); } catch (_) { console.error("FAIL / PHASE5_BUILD_STATIC / SourceOrConfigurationMismatch / NoPrivateValuesPrinted"); process.exitCode = 1; }
