"use strict";
// Read-only review inventory. Emits patches, not Unity execution or file writes.
const { inventory } = require("./editmode-test-inventory.cjs");
const groups = [
 ["이동·물리·카메라", "AutoMovementIntegration CameraFollowIntegration CollisionSystemContact PlayerJumpIntegration MomentumLandingIntegration WallFallPhysics WallLandingRecoveryIntegration", "생산 이동 연결, 실제 물리 접촉·점프·착지 및 카메라 추적"],
 ["게임·입력·메뉴", "GameEntryBootIntegration GameLifecycleIntegration GamePauseOrchestration HowToPlayIntegration PauseMenuIntegration ResultMenuIntegration UIInputSystem", "시작/종료·Pause/Retry, 입력 소비, 메뉴 및 안내 연결"],
 ["Infinite 진행·패턴", "InfiniteMapPattern InfiniteModeIntegration InfiniteModeSystem InfinitePatternConnectionIntegration InfinitePatternTraversalIntegration", "패턴 재사용·연결·물리 통과, 거리/점수·타이머·재배치 및 종료"],
 ["수집물", "CollectibleLifecycleIntegration ScoreCollectible InfiniteCollectibleLayoutIntegration StageCollectibleLayoutIntegration", "실제 Trigger·스코프·중복 획득 방지 및 생산 배치"],
 ["화면·Scene 참조", "InfiniteHudIntegration ModeResultDisplayIntegration ModeUISceneConfiguration PausePanelSceneConfiguration StageCollisionConfiguration", "표시 갱신/보존, 생산 Scene의 Inspector 참조·구성 및 충돌 설정"],
 ["설정·저장", "LocalPersistenceIntegration SettingsInteractiveRebind SettingsPanelIntegration", "설정 적용·실제 키 재지정·확인 창 연결"],
 ["Stage·타이머", "StageGoalIntegration StageSystem TimerSystem", "Goal·낙하·Stage 종료 중복 방지 및 프레임 간 시간 측정"],
 ["계정 UI·검증 격리", "AccountTransferUIIsolation PlayModeRecordIsolation VerificationToolIsolation", "사용자 구성 UI의 계정 동작, 민감 정보 정리 및 온라인/저장 격리"]
];
const actions = new Map();
function mark(fixture, method, reason, alias) {
 const entry = {reason, verdict:"조치 적용"};
 actions.set(fixture+"Tests."+method, entry);
 if(alias) actions.set(fixture+"Tests."+alias, entry);
}
mark("PauseMenuIntegration", "RetryButtonClick_MatchesDirectRetry", "동일 Pause Retry 버튼/독립 Run 검증을 RetryButton_StartsOneIndependentRun의 즉시 및 2프레임 보존 검사로 통합.");
mark("PauseMenuIntegration", "RetryButton_StartsOneIndependentRun", "즉시 Playing/독립 Run 확인과 이후 같은 Run 보존 assertion으로 통합 검증 보강.");
mark("ResultMenuIntegration", "ResultMenu_RetryButtonStartsNewStageRun", "동일 Result Retry 버튼의 Playing 검증을 MouseClickRetry의 즉시·다음 프레임 보존 검사로 통합.");
mark("ResultMenuIntegration", "ResultMenu_MouseClickRetry_StartsNewStageOnce", "통합 대상. 즉시 Playing 및 Stage, 이후 같은 Run을 유지하는지 확인.");
mark("InfiniteModeIntegration", "WorldRebase_ProductionScenePreservesRunStateAndRelativePositions", "단일 재배치의 속도 보존 assertion을 반복 재배치에 옮긴 뒤 통합.");
mark("InfiniteModeIntegration", "RepeatedWorldRebase_ProductionScenePreservesRunState", "두 재배치 각각의 속도 보존 및 metrics 갱신 성공 assertion 추가. 100회 스코프 스트레스 검사는 별도 유지.");
mark("PauseMenuIntegration", "PauseClickAndSubmit_RetryExecutesOnlyOnce", "같은 프레임에 Click과 실제 Unity Submit 이벤트를 보내고, 입력 소비와 이후 Run 동일성 확인. 플래그만으로 Submit을 대신하지 않음.");
mark("ResultMenuIntegration", "ResultClickAndSubmit_RetryExecutesOnlyOnce", "Click과 Unity Submit 이벤트를 같은 프레임에 전달. 클릭 직후 Run을 캡처해 즉시/다음 프레임 중복 Retry 감지.");
mark("ResultMenuIntegration", "ResultMenu_SubmitRetry_StartsNewStageOnce", "직접 SelectRetry나 플래그만 설정하는 방식 대신 EventSystem의 선택된 Retry 버튼 Submit 이벤트 전달. 입력 소비 및 다음 프레임 Run 동일성 확인.");
mark("ResultMenuIntegration", "InfiniteResult_KeyboardSubmitRetry_StartsInfiniteRun", "EventSystem의 선택된 Retry 버튼 Submit 이벤트로 Infinite 시작/입력 소비 확인. 실제 OS 키 이벤트 검사는 아니므로 UISubmit으로 이름 정정.", "InfiniteResult_UISubmitRetry_StartsInfiniteRun");
mark("InfiniteModeIntegration", "ControlledSeed_ProductionSelection_ActivatesDifferentPattern", "최종 assertion 뒤의 FreezeAll 및 5초 realtime 대기는 검사 결과를 추가하지 않으므로 제거.");
mark("InfinitePatternTraversalIntegration", "Flat_StationaryStart_AcceleratesAndRemainsGrounded", "현재 고정 속도 규칙에 맞게 Accelerates를 UsesFixedSpeed로 이름 정정.", "Flat_StationaryStart_UsesFixedSpeedAndRemainsGrounded");
mark("LocalPersistenceIntegration", "RestoredSettingsAndTutorial_AreAppliedToNewRuntimeState", "설정 변경 성공 확인. 실패 시에도 생성 객체를 finally로 정리하고 원래 AudioListener 볼륨 복구.");
mark("ModeResultDisplayIntegration", "Initialize_AfterResult_ClearsPreviousResultText", "이전 결과 설정 성공과 표시값을 먼저 확인해 빈 화면을 지워도 통과하는 경우 방지.");
mark("ModeResultDisplayIntegration", "ConsecutiveInfiniteRuns_DisplayIndependentResults", "각 결과 설정 성공과 첫 결과 표시를 확인해 첫 Run 준비 실패가 가려지는 경우 방지.");
mark("ModeResultDisplayIntegration", "StageRunAfterInfiniteRun_DoesNotKeepInfiniteResultText", "Infinite 결과 설정 성공/표시를 확인한 뒤 Stage 결과 설정 성공과 이전 표시 정리 검사.");
mark("InfiniteHudIntegration", "Retry_NewRuntimeDataResetsHudToZero", "Retry 전 0이 아닌 distance/score 표시를 먼저 검증해 초기화하지 않아도 통과하는 경우 방지.");
mark("InfiniteMapPattern", "ResetPatterns_AfterAdvance_RestoresInitialTransforms", "Initialize 및 두 Advance 성공/횟수 2를 확인한 뒤 Reset 검사.");
const methods = inventory("PlayMode").map(test=>{
 const group = groups.find(g=>g[1].split(" ").some(name=>name+"Tests"===test.fixture));
 if(!group) throw Error("Unclassified "+test.fixture);
 const review = actions.get(test.fixture+"."+test.method);
 return {...test, category:group[0], verdict:review ? review.verdict : "유지 권고", reason:review ? review.reason : group[2]+" 계약 확인. 현재 정적 검토에서 같은 입력·같은 계층의 제거 근거를 찾지 못함. 실제 실행/최소 집합 증명은 아님."};
});
const rows = methods.flatMap(test=>test.cases.map(input=>({...test,input})));
const header = ["번호","카테고리","Fixture","메서드","입력/실행 방식","소스 위치","판정","근거/조치","본문 SHA256","직접 assertion 수(helper 제외)"].map(quote).join(",");
function quote(value) {return '"'+String(value).replace(/"/g,'""').replace(/[\r\n]+/g," ")+'"';}
function rowText(row,index) {return [index+1,row.category,row.fixture,row.method,row.input,row.file+":"+row.line,row.verdict,row.reason,row.hash,row.assertions].map(quote).join(",");}
if(require.main===module) {
 if(process.argv[2]==="--actions-patch") {
  const fs=require("node:fs"), path=require("node:path");
  const removedNames=["RetryButtonClick_MatchesDirectRetry","ResultMenu_RetryButtonStartsNewStageRun","WorldRebase_ProductionScenePreservesRunStateAndRelativePositions"];
  const source=fs.readFileSync(path.resolve(__dirname,"../../AI/90_Tasks/Prototype_8/20261006_06_PlayModeTestAudit_Cases.csv"),"utf8");
  const deleted=source.trimEnd().split(/\r?\n/).slice(1).map(line=>[...line.matchAll(/"((?:""|[^"])*)"(?:,|$)/g)].map(m=>m[1].replace(/""/g,'"')))
   .filter(f=>removedNames.includes(f[3])).map(f=>({category:f[1],fixture:f[2],method:f[3],input:f[4]+" (제거 전 사례/소스 위치)",file:f[5].split(":")[0],line:Number(f[5].split(":")[1]),verdict:"통합 제거 적용",reason:f[7],hash:f[8],assertions:Number(f[9])}));
  if(deleted.length!==3)throw Error("Historical removal list changed");
  const entries=[...rows.filter(r=>r.verdict!=="유지 권고"),...deleted];
  console.log("*** Begin Patch\n*** Add File: AI/90_Tasks/Prototype_8/20261006_07_PlayModeTestActions.csv\n+"+header+"\n"+entries.map((r,i)=>"+"+rowText(r,i)).join("\n")+"\n*** End Patch");
 } else if(process.argv[2]==="--patch") {
  const start=Number(process.argv[3]), count=Number(process.argv[4]);
  if(!Number.isInteger(start)||!Number.isInteger(count)||start<0||count<1||start>=rows.length)throw Error("Invalid range");
  const file="AI/90_Tasks/Prototype_8/20261006_06_PlayModeTestAudit_Cases.csv";
  console.log("*** Begin Patch\n"+(start===0 ? "*** Add File: "+file+"\n+"+header : "*** Update File: "+file+"\n@@\n "+rowText(rows[start-1],start-1))+"\n"+rows.slice(start,start+count).map((r,i)=>"+"+rowText(r,start+i)).join("\n")+"\n*** End Patch");
 } else {
  console.log(JSON.stringify({fixtures:new Set(methods.map(t=>t.fixture)).size,methods:methods.length,cases:rows.length,
   categories:Object.fromEntries(groups.map(g=>[g[0],rows.filter(t=>t.category===g[0]).length])),
   reviewCases:rows.filter(t=>t.verdict!=="유지 권고").length},null,2));
 }
}
module.exports={methods,rows,groups};
