"use strict";
// Source/ledger consistency checks only. No Unity, compiler or network calls.
const assert=require("node:assert/strict"), fs=require("node:fs"), path=require("node:path");
const {methods,rows,groups}=require("./playmode-test-review.cjs");
assert.equal(methods.length,231); assert.equal(rows.length,254);
assert.equal(new Set(methods.map(t=>t.fixture)).size,37);
assert.equal(new Set(rows.map(t=>t.category)).size,8);
assert.equal(groups.reduce((sum,g)=>sum+rows.filter(r=>r.category===g[0]).length,0),254);
const csv=name=>fs.readFileSync(path.resolve(__dirname,"../../AI/90_Tasks/Prototype_8/"+name),"utf8").trimEnd().split(/\r?\n/).slice(1)
 .map(line=>[...line.matchAll(/"((?:""|[^"])*)"(?:,|$)/g)].map(m=>m[1].replace(/""/g,'"')));
const before=csv("20261006_06_PlayModeTestAudit_Cases.csv"), after=csv("20261006_07_PlayModeTestActions.csv");
assert.equal(before.length,257); assert.equal(after.length,18);
const original=new Map(before.map(f=>[f[2]+"."+f[3]+":"+f[4],f]));
const retained=rows.filter(r=>r.verdict==="유지 권고"); assert.equal(retained.length,239);
for(const r of retained) {
 const previous=original.get(r.fixture+"."+r.method+":"+r.input.replace(/[\r\n]+/g," "));
 assert.ok(previous,r.method); assert.equal(r.hash,previous[8],r.method+" retained body");
}
for(const f of after.filter(f=>f[6]==="조치 적용")) {
 const r=rows.find(r=>r.fixture===f[2]&&r.method===f[3]); assert.ok(r,f[3]);
 assert.equal(r.hash,f[8]); assert.equal(r.file+":"+r.line,f[5]); assert.equal(r.assertions,Number(f[9]));
}
for(const f of after.filter(f=>f[6]==="통합 제거 적용"))assert.ok(!methods.some(t=>t.fixture===f[2]&&t.method===f[3]));
function find(fixture,method) {const t=methods.find(t=>t.fixture===fixture&&t.method===method);assert.ok(t,method);return t;}
for(const [fixture,method] of [
 ["PauseMenuIntegrationTests","PauseClickAndSubmit_RetryExecutesOnlyOnce"],
 ["ResultMenuIntegrationTests","ResultClickAndSubmit_RetryExecutesOnlyOnce"],
 ["ResultMenuIntegrationTests","ResultMenu_SubmitRetry_StartsNewStageOnce"],
 ["ResultMenuIntegrationTests","InfiniteResult_UISubmitRetry_StartsInfiniteRun"]]) {
 const t=find(fixture,method); assert.ok(t.body.includes('SetPrivateField(_uiInputSystem, "_isSubmitPressed", true)'));
 assert.ok(t.body.includes('"IsSubmitPressed"')); assert.ok(!t.body.includes('InvokePublicMethod(_gameSystem, "SelectRetry")'));
 assert.ok(t.body.includes("ExecuteEvents.submitHandler") || t.body.includes("SubmitSelectedRetryButton()"), "Submit must reach the EventSystem, not only a flag");
 if(method.includes("ClickAndSubmit")) {
  assert.ok(t.body.indexOf('"_isSubmitPressed", true')<t.body.indexOf(".onClick.Invoke()"));
  assert.ok(t.body.indexOf("object restartedRuntimeData")<t.body.indexOf("yield return null;",t.body.indexOf(".onClick.Invoke()")));
  assert.ok(t.body.includes("Is.SameAs(restartedRuntimeData)"));
 }
}
const resultSource=fs.readFileSync(path.resolve(__dirname,"../../Assets/Tests/PlayMode/ResultMenuIntegrationTests.cs"),"utf8");
assert.ok(resultSource.includes("private void SubmitSelectedRetryButton()"));
assert.ok(resultSource.includes("EventSystem.current.currentSelectedGameObject,"));
assert.ok(resultSource.includes("new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler), Is.True"));
const rebase=find("InfiniteModeIntegrationTests","RepeatedWorldRebase_ProductionScenePreservesRunState");
assert.ok(rebase.body.includes('InvokePrivateBoolean(_infiniteModeSystem, "ProcessRunMetrics")'));
assert.ok(rebase.body.includes("Is.EqualTo(playerVelocity)"));
const seed=find("InfiniteModeIntegrationTests","ControlledSeed_ProductionSelection_ActivatesDifferentPattern");
assert.ok(!seed.body.includes("WaitForSecondsRealtime(5.0f)"));
const local=find("LocalPersistenceIntegrationTests","RestoredSettingsAndTutorial_AreAppliedToNewRuntimeState");
assert.ok(local.body.includes("finally"));assert.ok(local.body.includes("AudioListener.volume = previousVolume"));
for(const name of ["Initialize_AfterResult_ClearsPreviousResultText","ConsecutiveInfiniteRuns_DisplayIndependentResults","StageRunAfterInfiniteRun_DoesNotKeepInfiniteResultText"])
 assert.ok(find("ModeResultDisplayIntegrationTests",name).body.includes('Is.EqualTo("Distance Score: 129")'));
assert.ok(!methods.some(t=>t.body.includes("Is.AnyOf(")),"unsupported NUnit constraint");
console.log("PASS / PLAYMODE_STATIC_REVIEW / Original=257 / Current=254 / Unchanged=239 / Actions=18 / UnityNotRun=True");
