"use strict";
// Read-only source inspection. Never launches Unity, a compiler, or Test Runner.
const assert = require("node:assert/strict"), fs = require("node:fs"), path = require("node:path");
const { execFileSync } = require("node:child_process");
const { duplicateMembers } = require("./csharp-source-signatures.cjs");
const root = path.resolve(__dirname, "../..");
const read = file => fs.readFileSync(path.join(root, file), "utf8");
const git = args => execFileSync("git", args, { cwd: root, encoding: "utf8" }).split("\0").filter(Boolean);
const changed = [...new Set([...git(["diff", "--diff-filter=ACMRT", "--name-only", "-z", "HEAD", "--", "*.cs"]), ...git(["ls-files", "--others", "--exclude-standard", "-z", "--", "*.cs"])])];
const guids = new Map();
for (const file of changed) {
  const source = read(file);
  assert.deepEqual(duplicateMembers(source), [], file + " duplicate C# member declarations");
  assert.ok(!source.includes("<<<<<<<") && !source.includes(">>>>>>>"), file);
  const plain = source.replace(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g, "");
  const stack = [], pairs = { ")": "(", "}": "{", "]": "[" };
  for (const char of plain) { if ("({[".includes(char)) stack.push(char); if (pairs[char]) assert.equal(stack.pop(), pairs[char], file); }
  assert.equal(stack.length, 0, file);
  // Standalone Cloud Code Module source outside Assets is not a Unity asset.
  if (file.replace(/\\/g, "/").startsWith("Assets/")) {
    const match = read(`${file}.meta`).match(/^guid: ([a-f0-9]{32})\r?$/m); assert.ok(match, file);
    assert.ok(!guids.has(match[1]), `${file} duplicate C# GUID`); guids.set(match[1], file);
  }
}
const features = JSON.parse(read("Assets/Scripts/Runtime/Features/FlowState.Runtime.Features.asmdef"));
const edit = JSON.parse(read("Assets/Tests/EditMode/FlowState.EditModeTests.asmdef"));
const play = JSON.parse(read("Assets/Tests/PlayMode/FlowState.PlayModeTests.asmdef"));
assert.equal(features.name, "FlowState.Runtime.Features");
for (const name of ["Unity.Services.Core", "Unity.Services.Core.Environments", "Unity.Services.Authentication", "Unity.Services.CloudCode"])
  assert.ok(features.references.includes(name), name);
for (const test of [edit, play]) { assert.ok(test.references.includes(features.name)); assert.ok(test.optionalUnityReferences.includes("TestAssemblies")); }
assert.ok(edit.includePlatforms.includes("Editor")); assert.ok(play.references.includes("Unity.TextMeshPro"));
const setup = read("Assets/Tests/PlayMode/PlayModeRecordIsolationTests.cs"), game = read("Assets/Scripts/Runtime/Systems/GameSystem.cs"), records = read("Assets/Scripts/Runtime/Features/IOnlineRecordRepository.cs");
assert.ok(setup.includes("[SetUpFixture]")); assert.ok(setup.includes("IsolationField.SetValue(null, true)")); assert.ok(setup.includes("IsolationField.SetValue(null, false)"));
assert.ok(game.includes("? new TestMemorySaveFileStore()"));
assert.ok(game.indexOf("if (UseIsolatedRecordsForPlayModeTests || VerificationSessionConfiguration.IsEditorIsolationArmed)") >= 0);
assert.ok(game.indexOf("if (UseIsolatedRecordsForPlayModeTests || VerificationSessionConfiguration.IsEditorIsolationArmed)") < game.indexOf("IOnlineAuthenticationGateway authentication ="));
assert.ok(records.includes("GetTopAsync(RecordBoardKey boardKey, int limit = 10)"));
assert.ok(records.includes("GetAroundAsync(RecordBoardKey boardKey, int limit = 7)"));
assert.ok(game.includes("_onlineRepository.GetTopAsync(key, 10)"));
assert.ok(game.includes("_onlineRepository.GetAroundAsync(key, 7)"));
const leaderboardUi = read("Assets/Scripts/Runtime/Systems/UIManagementSystem.cs");
assert.ok(leaderboardUi.includes("private static string SafeQueryDiagnostic(OnlineLeaderboardResult result)"));
assert.ok(leaderboardUi.includes("case \"ReadLeaderboard\": phase = \"Read leaderboard\""));
assert.ok(leaderboardUi.includes("case \"MissingServerMetadata\": return \"Leaderboard metadata unavailable\""));
const newEdit = ["OnlineLocalSaveScopeTests", "OnlineAccountCoordinatorTests", "AccountTransferCompletionTests", "AccountTransferControllerTests", "VerificationSessionTests", "CloudCodeModuleRoutingTests"];
const newPlay = ["AccountTransferUIIsolationTests", "VerificationToolIsolationTests"];
let editCount = 0, playCount = 0;
for (const [folder, names] of [["EditMode", newEdit], ["PlayMode", newPlay]])
  for (const name of names) {
    const source = read(`Assets/Tests/${folder}/${name}.cs`);
    assert.ok(!/new UgsOnlineAuthenticationGateway|new UgsOnlineRecordTransport|new PersistentLocalSaveFileStore|AuthenticationService\.Instance|UnityServices\.InitializeAsync/.test(source), name);
    const count = [...source.matchAll(/\[Test\]|\[TestCase\(|\[UnityTest\]/g)].length;
    if (folder === "EditMode") editCount += count; else playCount += count;
    console.log(`PREPARED / ${folder} / ${name} / ${count} source cases (NOT EXECUTED)`);
  }
assert.equal(editCount, 254); assert.equal(playCount, 38);
const integrationSuites = {
  EditMode: ["LeaderboardViewStateTests", "GameNavigationStateTests", "ResultTextFormatterTests"],
  PlayMode: ["OnlineRecoveryNoticeFlowTests", "ResultMenuIntegrationTests", "ModeResultDisplayIntegrationTests",
    "ModeUISceneConfigurationTests", "PlayModeRecordIsolationTests"]
};
// Inspect serialized production references without launching or editing the Scene.
const sceneBlocks = read("Assets/Scenes/SampleScene.unity").split(/^--- /m);
const uiGuid = read("Assets/Scripts/Runtime/Systems/UIManagementSystem.cs.meta").match(/^guid: (\w+)/m)[1];
const gameGuid = read("Assets/Scripts/Runtime/Systems/GameSystem.cs.meta").match(/^guid: (\w+)/m)[1];
const uiBlock = sceneBlocks.find(block => block.includes("guid: " + uiGuid));
const gameBlock = sceneBlocks.find(block => block.includes("guid: " + gameGuid));
assert.ok(uiBlock); assert.ok(gameBlock);
const gameId = gameBlock.split("\n")[0].match(/&(\d+)/)[1];
for (const field of ["_resultRecordStatusText", "_resultSubmissionRetryButton", "_resultLeaderboardButton",
  "_leaderboardTopText", "_leaderboardAroundText", "_leaderboardAccountText", "_leaderboardInfiniteButton",
  "_leaderboardBackButton", "_leaderboardPendingRetryButton"]) {
  const reference = uiBlock.match(new RegExp("  " + field + ": \\{fileID: (\\d+)\\}"));
  assert.ok(reference, field); assert.notEqual(reference[1], "0", field);
  assert.ok(sceneBlocks.some(block => new RegExp("^!u!\\d+ &" + reference[1] + "\\r?\\n").test(block)), field);
}
for (const method of ["SelectResultLeaderboard", "SelectLeaderboardRetry", "SelectLeaderboardPendingRetry",
  "RetryPendingSubmission", "SelectMainMenu", "ConfirmOnlineRecoveryNotice", "CancelOnlineRecoveryNotice"])
  assert.ok(sceneBlocks.some(block => block.includes("m_MethodName: " + method) && block.includes("m_Target: {fileID: " + gameId + "}")), method);
console.log("PASS read-only SampleScene references: 9 UI targets and 7 GameSystem callbacks; no Scene edits.");
for (const [folder, names] of Object.entries(integrationSuites))
  for (const name of names) {
    const source = read(`Assets/Tests/${folder}/${name}.cs`);
    assert.deepEqual(duplicateMembers(source), [], name);
    console.log(`PREPARED / integration / ${folder} / ${name} / ${[...source.matchAll(/\[Test\]|\[TestCase\(|\[UnityTest\]/g)].length} source cases (NOT EXECUTED)`);
  }
console.log(`PASS Unity preflight: ${changed.length} changed/new C# files, meta GUIDs, source delimiters, asmdefs, isolated setup; Edit ${editCount}/Play ${playCount} prepared. NO compile/Test Runner/Scene/build/remote execution.`);
