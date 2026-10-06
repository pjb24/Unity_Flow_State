"use strict";
// Static review ledger. Generates patches only; never writes files or runs Unity.
const { inventory } = require("./editmode-test-inventory.cjs");
const fs = require("node:fs"), path = require("node:path");
const groups = [
  ["이동·착지", "JumpFeature NormalLandingFeature MomentumLandingFeature PlayerMovementMath PlayerSurfaceMath", "이동·점프·착지의 정상 입력, 거부 입력 및 수학 경계 계약"],
  ["모멘텀·점수", "MomentumScoreState MomentumHudPresentation MomentumProductionState InfiniteScoreState ScoringVersion", "점수 계산·누적·버전 구분 및 런타임 모멘텀 전달 계약"],
  ["월드 재배치·카메라", "WorldRebaseState PlayerWorldRebase CameraWorldRebase CameraFollow", "좌표 재배치 후 거리·속도·카메라 연속성 계약"],
  ["Infinite 진행 상태", "InfiniteModeState InfiniteDistanceState InfiniteDifficultyState InfiniteModeRuntimeData", "Infinite 시작·진행·중단·재시작 및 거리·난이도 계약"],
  ["Infinite 패턴·수집물", "InfinitePatternAuthoring InfinitePatternCatalogFactory InfinitePatternCatalog InfinitePatternDefinition InfinitePatternGeometry InfinitePatternSelectionState InfinitePatternSlotProgression InfinitePatternTraversalMath InfiniteCollectibleLayout CollectibleRuntimeData", "패턴 데이터·연결·선택·배치·통과 가능성 및 수집물 중복 방지 계약"],
  ["게임·UI·설정", "GameState GameRuntimeData GameNavigationState UIVisibilityState SettingsState HowToPlayBindingFormatter", "게임 상태·메뉴 이동·화면 표시·설정·키 바인딩 계약"],
  ["결과·시간·기록", "TimeRecord TimerRuntimeData ScoreRecord ResultData ResultSystem ResultTextFormatter", "결과 확정·기록 비교·시간 및 점수 표시 계약"],
  ["저장·온라인 기록", "LocalSaveJsonCodec OnlineLocalSaveScope OnlineRecordRepository OnlineRecordConfiguration RecordSubmissionPolicy RecordLeaderboardPolicy LeaderboardViewState", "저장 무결성·격리·재전송·제출/조회 정책 및 Leaderboard 상태 계약"],
  ["계정·이전", "AccountTransferCompletion AccountTransferController OnlineAccountCoordinator", "인증·계정 이전·동의·실패 복구·개인 최고 기록 적용 계약"],
  ["검증 도구·모듈 연결", "VerificationSession CloudCodeModuleRouting", "검증 환경 격리·원격 요청 제한·기준 비교 및 모듈 함수 연결 계약"]
];
const exceptions = new Map();
function mark(fixture, method, verdict, reason) { exceptions.set(fixture + "Tests." + method, { verdict, reason }); }
mark("WorldRebaseState", "RepeatedRebase_PreservesScoreAndDifficultyContinuity", "통합 보강 적용", "단일 사례 통합. 첫째·둘째 재배치 전후 거리·난이도 동일 검증 추가, 점수 불변 검사 유지.");
mark("InfinitePatternSelectionState", "SameCatalogSeedAndRequests_ProducesSameSequence", "검토 조치 적용", "양쪽 시작과 매 선택 성공 및 유효 ID 확인 후 시퀀스 비교.");
mark("InfinitePatternSelectionState", "TrySelectNext_WhenAlternativeExists_RespectsRepeatLimit", "검토 조치 적용", "시작과 매 선택 성공, Flat/Other 허용 ID 확인 후 반복 제한 검사.");
mark("GameNavigationState", "ShouldShowDifficulty_RequiresDevelopmentContextAndSetting", "검토 조치 적용", "전체 8조합을 고정 기대값으로 검사. 기존 4사례에서 4사례 추가.");
mark("MomentumProductionState", "ProcessRunMetrics_AfterFinalize_IsRejectedWithoutChangingRuntimeValues", "검토 조치 적용", "실제 검사하는 종료 후 진행 거부 및 값 보존으로 이름 정정.");
mark("AccountTransferCompletion", "ClearSessionHistory_RemovesRuntimeTerminalReceiptsAndCounters", "검토 조치 적용", "초기화 단위 테스트로 이름 정정. enqueue 성공 및 초기 Submitted receipt 존재 확인 후 초기화 검증.");
mark("OnlineRecordRepository", "SubmissionTimeout_RetryRetainsOriginalPendingAcrossRestart", "검토 조치 적용", "실제 저장소·Coordinator로 동일 ID Pending 1건과 receipt 없음, 저장 재로드 후 동일 ID 보존 검사. fake 대기로 실시간 지연 없음.");
mark("CloudCodeModuleRouting", "NewTransport_StartsWithNotRequestedDiagnostic", "검토 조치 적용", "생성 직후 진단 상태로 이름 정정. SDK 미초기화를 증명했다고 주장하지 않음.");
mark("InfinitePatternGeometry", "EveryTransition_ProvidesMinimumWindowAtFixedSpeed", "이름 정정 적용", "고정 BaseHorizontalSpeed 검사에 맞게 이름 정정.");
mark("InfinitePatternTraversalMath", "CanTraverseJump_SelectedBoundaryGap_IsAcceptedAtFixedSpeed", "이름 정정 적용", "고정 속도 검사에 맞게 이름 정정.");
const methods = inventory().map(test => {
  const group = groups.find(g => g[1].split(" ").some(name => name + "Tests" === test.fixture));
  if (!group) throw Error("Unclassified " + test.fixture);
  const exception = exceptions.get(test.fixture + "." + test.method);
  const assessment = exception || { verdict: "유지 권고", reason: group[2] + "을 검증. 현재 정적 검토에서 같은 입력·같은 계층의 제거 근거를 찾지 못함. 최소 테스트 집합임을 증명한 판정은 아님." };
  return { ...test, category: group[0], ...assessment };
});
const rows = methods.flatMap(test => test.cases.map((input, index) => ({ ...test, input, index: index + 1 })));
if (methods.length !== 605 || rows.length !== 887 || new Set(methods.map(t => t.fixture)).size !== 52) throw Error("Inventory changed: redo review");
for (const key of exceptions.keys()) if (!methods.some(t => t.fixture + "." + t.method === key)) throw Error("Missing review " + key);
const removed = [
  ["InfinitePatternGeometry", "TraversalContract_HeightAboveJumpApex_IsRejected", 209, "중복 제거 적용", "동일 TraversalMath 검사 유지."],
  ["InfinitePatternGeometry", "FixedSpeedTraversalContract_ShortLanding_IsAccepted", 218, "중복 제거 적용", "동일 TraversalMath 검사 유지."],
  ["WorldRebaseState", "RebasePreservedDistance_ProducesSameScoreAndDifficultyInput", 62, "통합 적용", "RepeatedRebase에 거리·난이도 검증을 옮긴 뒤 제거."]
].map(([name, method, line, verdict, reason]) => ({fixture:name+"Tests", file:"Assets/Tests/EditMode/"+name+"Tests.cs", method, line, verdict, reason, category:groups.find(g=>g[1].split(" ").includes(name))[0], input:"[Test] (제거 전 사례; 소스 위치도 제거 전)", calls:[], assertions:0}));
const baselinePath = path.resolve(__dirname, "../../AI/90_Tasks/Prototype_8/20261006_02_EditModeTestAudit_Cases.csv");
const legacyRemoved = fs.readFileSync(baselinePath, "utf8").trimEnd().split(/\r?\n/).slice(1)
  .map(line => [...line.matchAll(/"((?:""|[^"])*)"(?:,|$)/g)].map(match => match[1].replace(/""/g, '"')))
  .filter(fields => fields[2] === "ScoreCalculatorTests")
  .map(fields => ({ category:fields[1], fixture:fields[2], method:fields[3],
    input:fields[4] + " (제거 전 사례; 소스 위치도 제거 전)",
    file:fields[5].split(":")[0], line:Number(fields[5].split(":")[1]),
    verdict:"승인된 레거시 제거 적용", reason:"사용자 명시적 승인으로 미사용 ScoreCalculator 클래스와 해당 테스트 제거. 기존 v1 저장·기록 호환 API 유지.",
    calls:[], assertions:Number(fields[9]) }));
if (legacyRemoved.length !== 23) throw Error("Historical legacy inventory changed");
removed.push(...legacyRemoved);
const ledgerRows = [...rows.filter(row=>row.verdict !== "유지 권고"), ...removed];
const file = "AI/90_Tasks/Prototype_8/20261006_04_EditModeTestActions.csv";
const quote = value => '"' + String(value).replace(/"/g, '""').replace(/[\r\n]+/g, " ") + '"';
const header = ["번호", "카테고리", "Fixture", "테스트 메서드", "사례 입력", "소스 위치", "판정", "판정 근거", "호출 단서(일부; helper 포함)", "직접 assertion 수(helper 제외)"].map(quote).join(",");
function rowText(row, i) { return [i+1, row.category, row.fixture, row.method, row.input, row.file+":"+row.line, row.verdict, row.reason, row.calls.filter(call => !/^(Assert|CollectionAssert|StringAssert|LogAssert)\./.test(call)).slice(0, 12).join("; "), row.assertions].map(quote).join(","); }
if (require.main === module && process.argv[2] === "--patch") {
  const start = Number(process.argv[3]), count = Number(process.argv[4]);
  if (!Number.isInteger(start) || !Number.isInteger(count) || start < 0 || count < 1 || start >= ledgerRows.length) throw Error("Invalid range");
  const lines = ledgerRows.slice(start, start + count).map((row, i) => rowText(row, start + i));
  console.log("*** Begin Patch\n" + (start === 0 ? "*** Add File: " + file + "\n+" + header : "*** Update File: " + file + "\n@@\n " + rowText(ledgerRows[start-1], start-1)) + "\n" + lines.map(line => "+" + line).join("\n") + "\n*** End Patch");
} else if (require.main === module) {
  const tally = values => Object.fromEntries([...new Set(values)].map(value => [value, values.filter(item => item === value).length]));
  console.log(JSON.stringify({ methods: methods.length, cases: rows.length, categories: tally(rows.map(row => row.category)), verdicts: tally(rows.map(row => row.verdict)), fixtures: groups.map(g => ({category:g[0], fixtures: [...new Set(methods.filter(t=>t.category===g[0]).map(t=>t.fixture))]})) }, null, 2));
}
module.exports = { methods, rows, groups, header, rowText, ledgerRows, removed };
