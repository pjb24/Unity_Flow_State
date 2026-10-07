# 작업 정보

## 작업명

Prototype 8 Phase 2 — 공개 번호·논리 계정·기기 이전 구현 및 수동 적용 절차

## 작업 일자

20261003

## 작업 담당자

AI: 구현·정적 검증·Unit Test 작성·Node 실행·정확한 수동 적용 지침 작성.
사용자: Unity 컴파일·Test Runner 실행, Scene/Inspector 적용, verification 서비스 적용 및 화면 확인.

## 작업 상태

2026-10-06 게시용 폐기 파일 정리 검증 완료: 로컬 Node `UGS/Tests/*.test.cjs` 45파일 통과, `Tools/Tests/*.test.cjs` 2파일 통과(EditMode887/PlayMode254 소스 감사), `static-contracts.cjs`, `client-unity-preflight.cjs` 통과. 기존 Module 검사에 ccMR→sln 경로·현재 게시 entry만 존재·meta·9함수 원본 존재를 보강했다. 이번 문서 변경의 scoped `git diff --check`는 통과했다. 전체 작업 트리 검사에서 기존 `SampleScene.unity`의 trailing whitespace가 확인됐으나 사용자 Scene은 변경하지 않았다. Unity 컴파일·Build·Test Runner·원격 호출·게시를 실행하지 않았으며 이번 정리는 완료다.

2026-10-06 게시용 폐기 파일 정리: 사용자 승인으로 `Assets/CloudCode/Phase2Verification` JS 래퍼 9개·meta 10개, `UGS/Tests/phase2-editor-staging.test.cjs`, `UGS/Tests/phase2-bundle-diagnostics.test.cjs`, `UGS/Verification/check-phase2-bundles.cjs` 총 22파일을 제거했다. 제거 전 파일별 SHA256 일치를 확인한 백업은 `C:\Unity\Unity_Flow_State\Ignore\ObsoleteCloudCodeStaging-20261006`에 원래 상대 경로로 보관한다. 본문의 해당 파일·bundle 진단 언급은 당시 작업 이력이다. 현재 게시 대상은 `FlowStateVerification.ccmr` Module 하나이며 실제 서버 원본·Module·migration 회귀를 유지했다. `UGS/VERIFICATION_DEPLOYMENT.md`에서 현행 Module 지침과 과거 JS 지침을 분리했다. 이번 정리에는 Module 재빌드·원격 게시·Scene 작업이 필요하지 않다. 정적 검사 결과는 아래 후속 기록을 따른다.

2026-10-06 Step 12 후속 실제 저장 검사 성공: 사용자 Remaining 실행에서 AFTER10·두 대상 동시 이전·제출 예약/이전 시작 경합·LIVE_STORAGE_REMAINDER PASS를 확인했다. 앞선 실제 batch/CAS·정상 이전·before10/after9와 합쳐 장애 복구20사례 및 경합2사례를 두 실행의 근거로 확보했다. 단일 실행 전체 PASS로 바꾸거나 관리자 저장 검사를 Module/Anonymous/Jint 검증으로 확대하지 않는다. 기존 Step10~11 Module/서비스/UI·최신 Unity909/257와 대조해 준비한 기술 검증 범위는 통과했다. **남은 사용자 작업은 임시 Service Account Key 폐기·Cloud Save Editor 역할 회수 완료 보고**이며, 이 종료 확인까지 Step12/Phase2 전체 완료 처리는 대기한다. 추가 도구 실행/Unity Test/Build/Scene 작업은 없다.

2026-10-06 실제 저장 도구 사용자 결과: verification 실행에서 LIVE_BATCH_ATOMICITY_AND_CAS_SINGLE_WINNER·BASELINE_STORAGE_TRANSFER·BEFORE1~10·AFTER1~9가 PASS했고 다음 요청이 REQUEST_UNCONFIRMED/HTTP0으로 중단됐다. 위치/원인은 이전 출력으로 확정할 수 없고 전체 PASS로 기록하지 않는다. 19장애 복구의 실제 근거는 유지한다. 도구에 안전한 Case/Phase/Operation/RequestOrdinal/NetworkFault 진단 및 새 전용 공간의 남은 AFTER10/경합2사례만 검사하는 `-Run -Remaining`을 추가했다. 5초 제한·자동 retry 없음·기존 자료 보존 유지. 다음은 LIVE_TRANSFER_STORAGE_PROBE의 후속 실행 결과이며 Step12/Phase2는 미완료다.

2026-10-06 실제 저장 검사 안내 상세화: 사용자 요청으로 LIVE_TRANSFER_STORAGE_PROBE에 서비스 계정 `fs-phase2-storage-probe`·영어 설명·Key 표시 이름·조직/프로젝트/환경 ID·Cloud Save Editor 프로젝트 역할·터미널 질문별 입력·결과 파일 경로·키/권한 회수 대상을 지정했다. Unity 생성 Key ID/Secret Key와 무작위 실행/계정 ID는 고정 이름과 구분했다. 로컬 ProjectSettings/도구 소스와 공식 관리자 인증 안내를 대조했으며 문서만 변경했다. 원격 결과는 여전히 미보고이므로 Step12/Phase2는 미완료다.

2026-10-06 사용자 “할 수 있으면 해봐”에 따라 Step 12 실제 저장 검증 도구를 준비했다. `UGS/Verification/LIVE_TRANSFER_STORAGE_PROBE.md`의 사용자 절차로 임시 Service Account/Cloud Save Editor → PowerShell 숨김 입력 → 전용 verification 자료 검사 → 키 회수를 진행한다. 전용 p2v namespace에서 기존 서버 저장/이전 코드를 사용하며 기존 A/B·번호 발급기·Leaderboard·Module·Scene은 변경하지 않는다. 로컬 도구 5검사(20장애사례/동시 이전 포함) 통과, 원격 미실행으로 Step 12/Phase 2는 미완료다. 게시된 Module/Anonymous/Jint/전체 시간 budget 검증과 관리자 저장 검사를 구분한다.

2026-10-06 Step 12 판정: Step 1~11 근거 대조·최신 사용자 컴파일/Edit909/Play257·로컬 Node38파일 재검사·후속 Phase 인계 정리를 수행했다. 번호/표시/Offline/이전 정상 사례는 확인됐지만 실제 서비스의 강제 장애·동시 연결 교체 복구는 로컬 대역 근거뿐이다. 공식 Private Custom Item Batch 원자성/WriteLock 계약은 확인했고 전체 이전의 여러 요청을 한 transaction으로 보장하는 근거와 구분했다. **Step 12와 Phase 2 전체는 미완료**로 유지한다. 현재 추가 Scene/빌드/Test Runner/수동 타이밍 작업은 없으며, 남은 서비스 자동 검증 도구·범위는 별도 승인 후 준비한다. 아래 Step 11 완료 근거는 유효하다.

2026-10-06 최종 사용자 회귀 확인: Unity Script Compilation 성공, EditMode 909/909·PlayMode 257/257 성공 및 각 단계의 예상치 못한 Error/Warning 없음을 확인했다. 마지막 Client 수정 후 결과로 기존 904/257 대기를 해소했다. 앞선 게임 화면·복사·이전·Leaderboard 본인 표시·Offline/Pending 처리·조작/가독성 확인과 사용자 승인한 시작 지연 추가 확인 종료를 합쳐 **Step 11을 완료 처리했다. 다음 작업은 Step 12**다. 지연 개선 실측/UGS 원인 확정·원격 원자성 검증으로 확대하지 않으며 Step 12·Phase 2 전체·Production 승인은 미완료다. 아래 대기 안내는 이전 이력이다.

2026-10-06 사용자 결정: 시작 직후 계정 창의 지연 추가 확인은 수용한 제한으로 넘긴다. 아래 지연 재확인 요청은 종료했으며, 다음 작업은 마지막 Client 수정 후 사용자 Unity 컴파일·전체 EditMode/PlayMode 회귀 결과 확인이다. 904/257는 마지막 수정 전 결과다. 지연 원인을 UGS로 확정하거나 실제 5초 응답을 확인한 것으로 기록하지 않는다. Step 11 전체는 최신 회귀 결과 대기다.

2026-10-06 최신 사용자 결과: 컴파일 성공·EditMode 904/904·PlayMode 257/257와 예상 밖 Error/Warning 없음을 확인했다. 이전 후 B의 공개 번호와 Leaderboard `(You)` 일치 및 화면 조작 양호를 확인했다. A의 새 번호가 약 30초 뒤 나타난다는 체감 지연은 미해결 UI 문제로 남아, 백그라운드 인증/복구 전체 5초 budget·Refresh의 진행 중 인증 합류·상태/번호 중복 조회 제거를 Client에서 수정했다. EditMode 5사례 추가, Node 38파일·50 C# source preflight(Edit 180/Play 23 준비) 통과. 다음은 사용자 최신 컴파일/전체 Test와 같은 A의 시작 직후 계정 화면 재확인이다. 새 이전/Scene/Module 적용은 필요하지 않으며 904/257는 이번 수정 전 실행 근거다. Step 11 전체는 미완료다.

2026-10-06 Step 11-F 사용자 결과: 코드/인증값 개별 복사와 복사 안내 변경, 두 입력값으로 Complete Transfer, 공개 번호 변경 및 Transfer confirmed 표시를 확인했다. 설정/키 바인딩은 별도로 변경해 관찰하지 않았으며 이를 수동 PASS로 기록하지 않는다. 계정 전환 저장 코드가 같은 로컬 Settings(BindingOverrides 포함)와 튜토리얼 상태를 그대로 사용하고, 기존 자동 Test가 기기 자료 보존을 검사함을 정적으로 대조했다. 설정 보존을 인위적인 사용자 변경/수동 비교로 다시 요구하지 않는다. 남은 화면 항목은 B의 이전 후 본인 행, A 계정 창의 새 Anonymous 안내와 키보드 화면 선택/Submit/Close 가독성이다. 최신 전체 Unity Test 결과는 대기이며 Step 11 전체는 미완료다.

2026-10-06 최신 요청 반영: 붙여넣기 전용 버튼/파서를 제거하고 **Copy Transfer Code / Copy Verification Value** 두 버튼으로 값을 각각 복사하도록 수정했다. 입력은 기존 TMP 입력칸의 일반 Ctrl+V를 사용한다. 새 복사 테스트와 Scene 연결 안내를 갱신했다. Node 38파일·50 C# 정적 검사 통과와 사용자 Scene/Unity 실행 대기를 구분한다. 아래 Copy/Paste Transfer Details 안내는 변경 전 이력이다.

2026-10-06 Step 11-E 부분 확인: 사용자가 Start Transfer의 코드·인증값·만료 시각과 발급/재발급 안내, Cancel Transfer 취소 안내를 확인했다. 요청에 따라 게임 View에 Copy Transfer Details/Paste Transfer Details 기능을 준비했다. 복사/붙여넣기는 로컬 사용자 동작이며 Complete Transfer가 별도로 서버 요청을 수행한다. 사용자 Scene 버튼 생성·참조 연결과 최신 Unity 컴파일/전체 Test/화면 확인이 다음 작업이다. Node 38파일·50 C# source preflight(Edit 175/Play 23 준비) 통과와 실제 Unity 실행을 구분하며 Step 11 전체는 미완료다.

2026-10-05 최신 사용자 화면 결과: Confirm Discard 후 `Pending records discarded. Online records were kept.`가 표시됨을 확인받았다. 앞선 Offline 플레이·Pending 처리·온라인 복귀·Submitted·이전 입력 결과와 함께 **11-D 수동 화면 확인을 완료 처리**했다. 다음은 11-E A의 코드 발급·재발급·취소 확인이다. 최근 코드 수정 후 전체 Unity Test 결과 및 11-E~G는 대기이므로 Step 11 전체는 미완료다. 아래 폐기 완료 안내 대기 기록은 해결 전 이력이다.

2026-10-05 Step 11-D 사용자 결과: Offline 계정 오류/플레이·Result 제출 Retry·Pending 버튼·Keep Records 복귀·폐기 후 Pending 버튼 제거·온라인 Refresh 정상화·Stage Submitted·이전 입력 화면을 확인했다. 폐기 완료 문구의 가독성은 문제로 남아, 성공 후 기존 Result 페이지에 완료 안내를 유지하도록 Controller를 수정했다. 저장 실패는 DiscardConfirmation 페이지/기록을 유지한다. 기존 EditMode assertion 보강·새 PlayMode 1사례 준비, Node 38파일·50 C# source preflight(Edit 175/Play 22 준비) 통과. 다음은 사용자 최신 컴파일/전체 Test와 B의 폐기 완료 안내 재확인이다. Step 11-D 최종 판정과 Step 11 전체는 대기다.

2026-10-05 최신 화면 확인: 사용자가 Online Record Notice의 글자가 잘리지 않음을 확인했다. 11-B의 복구 안내 가독성 문제를 해결 완료로 기록한다. 계정 10자리 번호·TOP 본인 `(You)`·TOP/AROUND YOU 표시도 사용자 확인 완료다. 나머지 11-C의 Settings/Back 확인과 11-D~G 화면 작업 및 최근 코드 수정 후 전체 Unity Test 결과는 별도 보고 대기이며 Step 11 전체는 미완료다. 아래 글자 잘림 대기 기록은 해결 전 이력이다.

2026-10-05 Step 11-C 후속 결과: 사용자에게 TOP과 AROUND YOU가 모두 표시됨을 확인받았다. Online Record Notice의 글자 잘림은 남아 있다. Scene 읽기에서 RecoveryWindow 520×300, 본문 456×104를 확인했고, 사용자 적용용 창/본문/버튼 RectTransform 값을 11-B에 추가했다. 다음은 사용자 레이아웃 적용·실제 가독성 확인이다. 최근 코드 수정 후 전체 Unity Test 결과는 별도 사용자 보고 대기이며 Step 11은 미완료다.

2026-10-05 Step 11-C 사용자 결과: 계정 화면의 전체 10자리 공개 번호와 TOP 본인 행 `(You)` 표시를 확인했다. AROUND YOU의 Authentication unavailable 및 한글 표시 문제를 발견해 Client 코드를 수정했다. 동시 기록 조회의 인증 결과 공유, 런타임 안내 영어화, EditMode 3사례 추가와 기존 PlayMode 영어 표시 assertion 보강을 수행했다. Node 38개 검사 파일과 50개 C# source preflight가 통과했다. **다음은 사용자 Unity 컴파일·전체 Edit/Play Mode Test 및 게임 Leaderboard Retry 화면 재확인**이다. Step 11은 미완료이며 아래 901/255는 이번 수정 전 근거다.

2026-10-05 Step 11 착수: 실제 Game 화면용 A/B 실행 인수·예약 취소, 사용자 전용 Scene 문구 정리, Offline/Pending·발급/취소·A→B 완료·Keyboard/Mouse 확인 절차를 Step 11-A~H에 준비했다. 정적 점검은 통과했고 실행 코드·Scene·서비스는 변경하지 않았다. 최신 사용자 901/255 회귀 근거를 유지하며 **다음은 사용자 Step 11-A~H의 화면 작업**이다. 화면 결과 미보고로 Step 11/12·Phase 2 전체는 미완료다. 아래 Step 10-3 완료 기록은 유효한 이전 단계 근거다.

2026-10-05 최종 확인: 사용자 보고로 Unity Script Compilation 성공, EditMode 901/901·PlayMode 255/255 성공 및 각 단계의 예상치 못한 Error/Warning 없음을 확인했다. verification의 이전·권한 거부·사용된 인증값 재사용 거부·기록 보존 근거와 함께 **Step 10-3을 완료 처리했다. 다음 작업은 Step 11**이다. Step 10-2는 사용자 승인에 따른 적용 대상 없음(N/A)이며 실제 기존 계정 전환 PASS로 간주하지 않는다. 원격 지연 원인과 장애·경합 원자성의 미확인 사항은 Step 12에서 별도 검토하며, Step 11/12·Phase 2 전체·Production 승인은 아직 완료되지 않았다. 아래 최신 상태 표기는 이전 이력이다.

2026-10-05 최신 상태: 사용자 확인으로 미전환 기존 계정이 없고, 사용자 “진행한다” 승인에 따라 **Step 10-2는 적용 대상 없음(N/A)·실제 전환 검증 미실시로 종료**했다. 실제 서비스 PASS와 구분하며 합성 원격 자료를 만들지 않는다. **다음 작업은 Step 10-3**이다. 이전/고정 행·서비스 제약과 Step 11/12·Phase 2 전체는 미완료다. 아래 Step 10-2 대기 안내는 이전 이력이다.

2026-10-05 최신 사용자 결과: Unity 컴파일 성공, EditMode 891/891·PlayMode 255/255 성공, 각 단계 예상 밖 Error/Warning 없음. 앞선 A/B 실제 서비스 결과·로컬 distinct 비교·서버 5초 수정 Module 게시 성공·B 최초 사용 확인과 합쳐 **Step 10-1을 완료 처리**하고 Step 7의 최신 Client 회귀 검증을 갱신했다. 다음 작업은 Step 10-2다. Step 10-2/10-3·11/12 및 Phase 2 전체는 미완료다. 창의 수정 후 실제 wall-clock 5초 종료 관찰과 서버 시간 경계/기동·통신 시간은 별도 미보고이며 이번 Test 성공으로 실측했다고 기록하지 않는다.

아래는 초기 완료 이력이며 현행 다음 작업은 위 최신 상태를 따른다.

Step 1~5의 서버 코드·로컬 Node 검증과 Step 6-1~6-5의 Client 코드/Test 작성·정적 대조를 2026-10-03 완료했다. 2026-10-04 사용자가 최신 Unity Script Compilation 성공, Edit Mode 850개 실행·850개 통과, Play Mode 242개 실행·242개 통과 및 각 단계의 예상 밖 Error/Warning 없음을 확인했다. AI의 선행 정적/Node 검증 및 사용자 Scene 연결·저장·영어 label 적용/읽기 전용 대조 근거와 합쳐 Step 7·8을 완료 처리했다. 실제 배치에 맞춘 사용자 Navigation 변경을 유지한다. 다음은 Step 9 verification 적용이며 Step 9~12와 Phase 2 전체는 미완료다.

# 작업 목적

Roadmap 008 Phase 2를 실제로 수행할 순서로 나누고, 코드로 검증할 항목과 사람이 직접 적용·확인할 항목을 구분한다. Phase 1의 확정 정책을 다시 선택하지 않고 실제 저장·서비스·Client·UI에 연결한다.

# 작업 대상

- 공개 번호 발급·조회·기존 계정 전환, 논리 계정 C와 단일 활성 연결, 고정 Leaderboard 소유 행.
- 서버 발급 이전 코드·인증값, 잠금·재발급·취소·만료·B 연결, 제출 예약과 연결 교체 경합.
- 계정 이전 전후 Local Save·개인 최고·인증 상태, 공개 번호·오류·이전 UI.
- verification에서의 제한된 서비스 적용과 실제 서비스·UI 확인.

Phase 3의 180일 receipt 분할·정리, 128건/100명 제한 해소, Phase 4의 Production 구성·배포와 30일 로그 sink, Phase 5의 최종 Windows Player Build·운영 후보 판정은 후속 작업이다. Phase 2에서는 기존 제한을 숨기거나 해소했다고 기록하지 않는다.

# 작업 전 상태

- Phase 1 Task Step 1~8 완료. 공개 번호는 인증 직후 서버 발급하는 10자리 십진 문자열이다.
- 이전 코드는 Crockford Base32 8자리, 인증값은 십진 9자리, 만료는 90일, 반복 검증 간격은 5초다. 활성 코드 공간은 환경별 천만 개 동시 활성 목표를 가진다. 정책의 코드 공간 계산과 실제 서비스 용량 입증은 구분한다.
- B의 기존 온라인 기록은 이전을 막지 않는다. B의 Local Save Pending은 이전을 막으며 전송 완료 또는 명시적 폐기 후 다시 시도하도록 안내한다.
- 이전은 C의 활성 연결만 교체한다. 고정 Leaderboard 행·공개 번호·최고 기록·수락 시각은 유지한다. B의 로컬 개인 최고는 C의 서버 개인 최고로 덮어쓰며, C 기록이 없으면 비운다.
- 현재 서비스 코드와 Client는 UGS Player ID 귀속 경로를 사용하며 C 해석·공개 번호·이전 서비스는 미구현이다. Phase 1 정책 Test 통과는 이 통합의 검증 근거가 아니다.

# 조사 내용

- 설계 근거는 Phase 1 Task의 2-1~2-10과 Step 7 대응표·Step 8 인계다.
- 기존 서버 파일: `UGS/CloudCode/submit-record.js`, `UGS/CloudCode/query-records.js`.
- 기존 Client 파일은 `Assets/Scripts/Runtime/Features/`의 `OnlineAccountState.cs`, `UgsOnlineAuthenticationGateway.cs`, `CloudCodeRecordRepository.cs`, `OnlineRecordCoordinator.cs`, `LocalSaveJsonCodec.cs` 등에 있다.
- 기존 Scene은 `Assets/Scenes/SampleScene.unity`다. `Canvas`, `OnlineRecoveryNoticePanel`, Result 제출 상태·Retry UI가 존재한다. 새로운 이전 UI의 위치·Component·Inspector 필드는 구현 후 Step 8에서 정확히 지정한다.
- 기존 검증 Test는 `Assets/Tests/EditMode/OnlineRecordRepositoryTests.cs`, `LocalSaveJsonCodecTests.cs`, `LeaderboardViewStateTests.cs`, `RecordLeaderboardPolicyTests.cs` 및 `Assets/Tests/PlayMode/PlayModeRecordIsolationTests.cs` 등에 있다.
- 원격 적용 기준은 `UGS/VERIFICATION_DEPLOYMENT.md`와 `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`다. Prototype 7의 제한된 재게시 안내를 Prototype 8의 신규 배포 목록으로 그대로 사용하지 않는다.

# 작업 내용

## 수행 원칙과 순서

각 Step은 AI 구현·정적 검증·Unit Test 준비 후, 필요한 사용자 적용·실행을 수행한다. 사용자 작업이 없는 Step은 AI가 검증 후 완료 처리한다. 사용자 작업이 있는 Step은 실제 결과를 받은 뒤 완료 처리한다.

Step 1~12는 전체 흐름이며, 큰 작업은 `2-1`, `2-2`처럼 독립적인 하위 Step으로 수행한다. 각 하위 Step의 산출물·검증·완료 조건을 기록하고 모두 완료된 경우에만 상위 Step을 완료한다. 사용자 수동 작업과 공통 조건은 상위 Step의 안내를 따른다. 하위 Step 완료 시 Node 실행 결과와 C# Test 작성·정적 확인 결과를 구분하며, C# 실행 검증은 Step 7에서 확인한다.

### Step 분할 검토 결과

- 서버 저장 기반·번호 발급·기존 자료 전환, 이전 발급·재발급·취소/만료, 제출·조회, Client 저장·인증·UI는 각각 별도 하위 Step으로 나눈다. 서로 다른 실패 복구와 Test를 독립적으로 검토할 수 있어야 한다.
- verification 적용은 백업·설정·게시, 실제 서비스 확인은 신규 계정·기존 계정·이전으로 나눈다. 한 부분의 실패로 다른 부분의 완료 근거가 섞이지 않게 한다.
- verification 실행 도구는 Step 6에서 준비하고 Step 7에서 컴파일·격리 Test를 확인한다. Step 10에서는 준비된 도구를 사용한다.
- Step 7의 Play Mode Test는 저장·인증·통신을 대역으로 격리한다. UI/참조는 코드로 생성하지 않고 사용자가 Step 8에서 연결한 실제 Scene 구성을 사용한다. 따라서 Scene 기반 UI 검증은 사용자 Step 8 적용 뒤 실행하며 완료 결과는 Step 7·8 양쪽에 기록한다.
- Phase 3~5 작업을 새 Step으로 추가하지 않는다. 현재 계획은 12개 상위 Step과 21개 하위 Step으로 구분한다.

- AI는 Unity Editor Build와 Unity Test Runner를 실행하지 않는다. Scene·Prefab을 직접 수정하지 않는다.
- 정적 확인으로 알 수 있는 경로·참조·직렬화·입력 정의·환경 상수·권한 정책을 사용자에게 재확인시키지 않는다. 원격 설정과 실행 결과만 수동으로 확인한다.
- Unit Test는 생산 코드와 메모리 저장소·서비스 대역·가짜 시계를 사용한다. Test 내부에 구현을 복제하지 않는다. 실제 계정·사용자 Local Save·원격 서비스와 분리한다.
- 동시 발급, 두 B 경합, 제출과 이전 충돌, 응답 유실, 90일·5초 경계는 자동 검증한다. 빠른 클릭이나 90일 대기를 요구하지 않는다.
- 원격 수동 작업은 verification에 한정한다. 기존 번호·계정·행·ledger를 삭제하거나 초기화하지 않는다. 자격 증명·Secret 값·토큰은 채팅·저장소·로그로 전달하지 않는다.
- 신규 서버 Script/키/Component/Test 이름은 해당 Step 구현 후 실제 파일과 일치하는 적용표로 제공한다. 아래 예정 Test 책임은 아직 존재하는 fixture 이름을 의미하지 않는다.
- 코드 변경 뒤에는 Step 7을 수행한다. 이후 Step에서 코드가 바뀌면 영향받는 컴파일·Test를 다시 확인한다.

| Step | 작업 | 사용자가 직접 할 일 |
| --- | --- | --- |
| 1 | 기존 코드·SDK·저장 원자성 구현 계획 | 기본 없음 |
| 2 (2-1~2-3) | 서버 저장 기반·공개 번호·기존 자료 전환 | 없음 |
| 3 (3-1~3-3) | 이전 발급·재발급·취소·만료 | 없음 |
| 4 (4-1~4-2) | B 연결·경합·부분 실패 복구 | 없음 |
| 5 (5-1~5-2) | 제출·조회 경로 C 귀속 전환 | 없음 |
| 6 (6-1~6-5) | Local Save·인증·완료 상태·UI·검증 도구 | 없음 |
| 7 | Unity 컴파일·자동 Test 실행 | Editor Console, Edit/Play Mode Test Runner |
| 8 | Scene/Inspector 적용 | UI 생성·Component 참조·Navigation 연결 |
| 9 (9-1~9-3) | verification 서비스 적용 | 대상 확인·백업·Secret 권한·변경 목록 게시 |
| 10 (10-1~10-3) | verification 실제 서비스 검증 | 격리된 인증 세션으로 실제 검증 도구 실행 |
| 11 | 두 기기 UI 흐름 확인 | 발급·Pending 안내·이전·A 새 시작·Offline 화면 |
| 12 | Phase 2 완료 판정·후속 인계 | 미보고 실행 결과 제출만 |

## Step 1. 기존 구현과 서버 저장 원자성 계획을 확정한다

### AI 수행

1. Phase 1 계약과 Client·서버·SDK·기존 Test를 대조하고 변경 파일·API·저장 단위·호환 순서를 정리한다.
2. Cloud Save 최초 생성, write lock, 다중 저장 단위의 부분 실패, 고정 소유 ID의 Leaderboards 쓰기, Script 의존성 배포 방법을 현재 SDK 및 공식 문서로 확인한다. 동일 항목 CAS를 다중 항목 transaction으로 간주하지 않는다.
3. Account를 권한·terminal 판정의 기준으로 삼고 PlayerBinding·번호 매핑·TransferLookup 불일치의 복구 순서와 fail-closed 조건을 구체화한다.
4. 최초 번호 예약·중단 뒤 재개·번호 비재사용, 환경별 코드 유일 예약, 제출 예약 복구의 Test 사례를 정한다. UGS에서 입증하지 못한 제약은 미확인으로 기록한다.

### 사용자 수동 작업

기본 없음. 공식 문서·로컬 자료로 확인할 수 없는 원격 설정이 구현을 막을 때만 AI가 대상 Project/Environment, 확인할 설정 화면과 필요한 값 목록을 제공한다. 사용자는 해당 값을 읽어 요약한다. 이 단계에서는 게시·Secret 생성·자료 수정하지 않는다.

### 완료 조건

- [x] 변경 대상·물리 저장/API·실패 복구·자동 검증·원격 검증 지점을 정했다.
- [x] 기존 자료 보존과 cutover/호환 롤백 조건, 미확인 서비스 제약을 기록했다.

### Step 1 수행 결과 — 2026-10-03

- 현재 `submit-record`·`query-records`와 Client는 인증 `context.playerId`를 Protected ledger와 Leaderboard 행 소유자로 직접 사용한다. 논리 계정 C·공개 번호·고정 소유 행·기기 이전의 생산 경로는 아직 없다.
- Phase 2 서버 저장은 Cloud Save Private Game Data/Custom Item을 후보로 하되, Account 본문을 권한·terminal 판정의 기준으로 둔다. PlayerBinding·PublicNumberBinding·LeaderboardOwnerBinding·TransferLookup은 조회/예약 보조 자료이며 Account 본문과 불일치하면 권한을 주지 않는다. C별 ledger/작업 예약은 Account의 연결 revision 및 상태를 재검증한 뒤에만 진행한다.
- Cloud Save write lock은 이미 존재하는 단일 항목의 충돌 검출만 제공한다. 신규 항목 생성은 lock을 생략하며, 여러 저장 항목·Leaderboard 쓰기를 하나의 transaction으로 보장하지 않는다. 그러므로 최초 발급·cutover·연결 교체는 단계별 상태, idempotent 재시도, 원본 보존 및 재조회 기반 복구로 구현하고, 어느 단계도 부분 매핑만으로 권한을 부여하지 않는다.
- 기존 자료 cutover는 원본 Protected ledger와 기존 Leaderboard 행/metadata를 보존한 snapshot을 먼저 기록하고, Account·모든 Binding·대상 ledger가 확인된 경우에만 활성화한다. 활성화 전 중단은 원본이 변하지 않은 경우에만 기존 경로로 복구하며, 활성화 후에는 구버전을 무조건 재게시하지 않는다.
- 자동 검증은 Node 저장소 대역으로 동시 발급, 최초 생성/각 쓰기 실패, 응답 유실, 재시작, cutover 재개, 두 B 경합, 제출 예약과 이전의 양방향 차단을 다룬다. 실제 Cloud Save 최초 생성·다중 항목 원자성·서비스 토큰 Leaderboards 쓰기·권한 강제는 Step 10 verification 검증까지 미확인으로 유지한다.
- 확인 근거(2026-10-03): Unity Cloud Save [write locks](https://docs.unity.com/en-us/cloud-save/concepts/write-locks), [Cloud Code에서 Cloud Save 사용](https://docs.unity.com/en-us/cloud-save/tutorials/cloud-code), [Cloud Code token 지원](https://docs.unity.com/en-us/cloud-code/scripts/how-to-guides/token-support), [Cloud Code JavaScript의 Leaderboards 접근](https://docs.unity.com/en-us/leaderboards/access-cloud-code). 로컬 `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`(21/21), Phase 1 정책 Test 11개와 `git diff --check`가 통과했다. 이 검사는 원격 UGS, Unity Editor, Unity Test Runner, Build, Scene을 사용하지 않는다.

### Step 1 사용자 수동 작업 결과

없음. 현재 저장소와 Unity 공식 문서만으로 Step 1의 계획·제약을 확정할 수 있었다. 원격 Project/Environment 설정값, 최초 생성 동시성, 실제 권한과 서비스 토큰 쓰기 강제력은 게시하지 않는 이 Step에서 확인하지 않으며 Step 9~10의 verification 작업으로 남긴다.

## Step 2. 논리 계정·공개 번호·고정 소유 행과 기존 자료 전환을 구현한다

### Step 2-1. 서버 저장 기반과 권한·작업 예약을 구현한다

Step 1의 물리 저장 계획에 따라 Account·PlayerBinding·고정 소유 ID 저장소, 활성 연결/revision 대조, 제출 작업 예약과 이전 시작 차단을 구현한다. 저장소·시계·ID 생성기를 주입하고 환경 불일치·부분 매핑·예약 복구를 Unit Test로 확인한다. 이후 Step 3의 잠금은 이 기반을 사용한다.

- [x] 저장 기반·권한·예약의 Node Unit Test와 정적 검증이 통과했다.

### Step 2-2. 공개 번호 발급·재조회·유일 예약을 구현한다

2-1의 저장소로 인증 계정의 신규 번호 발급·기존 번호 반환·중단 후 재개를 구현한다. 번호 유일 예약·비재사용·문자열 정밀도·동시 발급을 자동 검증한다.

- [x] 발급·재조회·충돌·부분 실패 Test가 통과했다.

### Step 2-3. 기존 계정·고정 행·ledger의 전환을 구현한다

2-1~2-2 완료 후 기존 자료의 승계·재개 가능한 cutover·활성화 전 대조·호환 복구를 구현한다. 기존 행·최고 기록·수락 시각·Pending 귀속 보존과 전환 재실행을 검증한다.

- [x] 기존 자료 전환·보존·부분 실패 복구 Test가 통과했다.

### AI 수행

인증 context로 C를 찾는 저장소·발급 서비스, 번호 유일 예약과 기존 번호 반환, 기존 UGS 행/ledger를 승계하는 재개 가능한 cutover를 구현한다. 공개 번호는 문자열로 직렬화한다. 새로운 계정의 고정 소유 ID는 서버 생성 UUID v4를 사용한다. 전환 활성화 전에 매핑·원래 기록·Pending 귀속을 대조한다.

Unit Test: 같은 계정 재요청·재시작, 다른 계정 동시 발급, 각 저장 단계 실패·응답 유실·재개, 선행 0·최대값, 번호 미재사용, 기존 기록 metadata 보존, 기존 행의 지연 번호 발급, Client 임의 소유 ID 거부. 기존 정책 단위를 재사용하고 실제 서비스 계층을 저장소 대역으로 검사한다.

### 사용자 수동 작업

없음. 번호·매핑·ledger를 Dashboard에서 수동으로 만들어 구현을 대신하지 않는다.

### 완료 조건

- [x] 서버 저장·발급·cutover 구현과 해당 Node Unit Test가 통과했다.
- [x] 실제 서비스의 최초 생성·유일성 보장은 Step 10 검증 대상으로 남겼다.

### Step 2 수행 결과 — 2026-10-03

완료 범위는 아래 서버 소스와 실제 서비스 코드에 SDK 대역을 주입한 Node 검증이다. 원격 Script 게시·원격 저장 보장 검증은 Step 9~10에 남긴다. Client·C 귀속 제출/조회 통합은 Step 5~6이며 현재 이 소스를 게시하지 않는다.

| 산출물 | 책임 |
| --- | --- |
| `UGS/CloudCode/account-store.js` | Private Custom Item 읽기·기존 항목 CAS·guard 배치 최초 생성, context/scope 및 50자 Custom ID 검증 |
| `UGS/CloudCode/account-service.js` | PlayerBinding과 Account 권한·revision 대조, 번호/소유 매핑 재검증, 제출 예약·terminal 확인 후 해제·이전 시작 gate |
| `UGS/CloudCode/account-provisioning-service.js` | 서버 UUID v4 C/소유 ID, 번호 allocator·영구 번호 예약, Preparing→Active 전환과 재개 |
| `UGS/CloudCode/legacy-account-store.js` | Protected source ledger 동결·기존 Board 기록 조회·보존 snapshot |
| `UGS/CloudCode/get-public-player-number.js` | 인증 context 전용 발급/조회 endpoint. 입력 없음, 표시 번호·안전한 상태만 응답, 로컬 의존성 bundling 필요 |
| `UGS/CloudCode/submit-record.js` | Step 2 당시 legacy marker 쓰기 차단. Step 5에서 C 귀속으로 교체했으며 이전 bridge는 `UGS/Migration/legacy-barrier-submit.js`에 보존 |
| `UGS/Tests/account-sdk-double.cjs` | 단일 Custom Item/Player batch의 원자성과 lock 충돌을 모델링한 SDK 대역 |
| `UGS/Tests/account-service.test.cjs` | 권한·환경·stale 응답·예약/해제·제출/이전 경합 20개 |
| `UGS/Tests/account-provisioning.test.cjs` | 신규/기존 발급·동시 생성·재시작·번호 소진·각 쓰기 실패/응답 유실·행/metadata 보존·기존 제출/cutover 양방향 경합 19개 |

#### 물리 저장과 최초 생성

- Private Custom ID는 `fs8-<kind>-<id>`이며 kind는 `account`, `player`, `owner`, `number`, `ledger`, `allocator`다. payload key는 `fs_account_v1`이다. allocator ID는 `public`, 나머지 ID는 서버 생성 C, context Player ID, 고정 소유 ID 또는 10자리 번호다. Project/Environment는 서비스 토큰 환경과 payload 양쪽에서 대조한다.
- 각 Custom Item의 `fs_creation_guard_v1`에는 불변 schemaVersion/Project/Environment만 둔다. 이 guard의 최초 저장만 lock 없이 수행하며, 늦은 초기화가 계정·번호·counter를 덮어쓰지 않는다.
- guard의 writeLock을 **먼저** 읽고 대상 payload 부재를 확인한 뒤 `setPrivateCustomItemBatch`로 guard CAS와 신규 payload를 함께 저장한다. 같은 Custom Item 내부 배치의 원자성을 이용하며, 서로 다른 Custom Item 간 transaction을 가정하지 않는다. 존재하는 payload는 create에서 덮어쓰지 않는다. 실제 배치 강제력은 Step 10 검증 대상이다.
- Protected ledger가 없는 경우에도 같은 Player의 불변 guard와 `setProtectedItemBatch`로 marker를 포함한 source barrier를 만든다. 개별 계정·번호·매핑을 사용자가 수동 생성하지 않는다.
- 설치된 Cloud Code 2.10.4의 `UnityServicesNpmLocal~/cloud-save-1.4.0.tgz` 안 `package/dist/api.d.ts`에서 Private get/set/batch, Protected get/set/batch의 입력·동일 Custom Item 배치 원자성·50자 Custom ID 제약을 직접 대조했다. 이 근거는 Step 1의 단일 항목 CAS 설명을 보완한다. [Unity SDK 문서](https://cloud-code-sdk-documentation.cloud.unity3d.com/cloud-save/), [Game Data](https://docs.unity.com/en-us/cloud-save/concepts/game-data), [write locks](https://docs.unity.com/en-us/cloud-save/concepts/write-locks)를 2026-10-03 확인했다.

#### 실패 복구·권한·번호

- 최초 PlayerBinding이 서버 생성 C/신규 고정 소유 후보를 확정한다. Account가 준비 중이면 일반 권한은 부여하지 않는다. 부분 매핑·다른 환경·다른 활성 Player/revision·번호/소유 매핑 불일치는 차단한다.
- allocator의 next를 CAS로 증가시킨 뒤 번호 매핑을 예약하고 Account에 10자리 문자열을 기록한다. 같은 C 경합은 Account CAS로 하나의 확정 번호에 수렴한다. 중단/응답 유실로 소모된 번호는 다시 쓰지 않으며, 미사용 번호 예약은 활성 Account 번호를 대신하지 않는다. 최대 번호 다음에는 `PublicNumberExhausted`로 실패한다.
- C와 신규 owner는 별도 CSPRNG UUID v4다. 서버 `crypto.randomBytes` 사용 가능 여부는 Step 10의 원격 실행으로 확인하고, unavailable일 때 일반 실패를 반환한다. Client의 C·번호·소유 ID 입력은 권한이나 발급 값으로 사용하지 않는다.
- 제출 예약은 Account CAS로 저장하고 동일 ID/revision만 Resume한다. matching terminal ledger를 확인한 뒤에만 해제한다. 응답 유실 뒤 재조회로 복구하며 시간 경과만으로 예약을 탈취/해제하지 않는다. 이전 시작은 같은 Account token을 이후 Step 3에서 CAS해야 한다.

#### cutover·배포·호환 복구

- 기존 ledger의 active/Pending 처리가 끝나기 전에는 source를 동결하지 않는다. 동결 CAS와 기존 제출 예약 CAS 중 한 쪽만 통과한다. 원본 entries/payload/terminal 상태/거절 사유/acceptedAt/best를 보존하고 marker만 추가한다.
- 기존 기록이 있으면 context에서 확인한 기존 Player ID 행을 고정 owner로 승계한다. row score와 metadata를 읽기만 하고 점수 쓰기·행 복사·삭제는 하지 않는다. 기존 행만 있고 ledger가 없으면 서버 metadata로 C best를 구성한다. score 조회 404 뒤에는 Board 존재를 별도 조회해 구성 오류를 0건으로 오인하지 않는다.
- source snapshot·대상 ledger·Account·Player/번호/owner 매핑을 대조한 뒤 Account를 Active로 CAS한다. 원본 source와 snapshot은 남긴다. Local Save/Pending 귀속·ID는 변경하지 않았다. 다른 사용자가 제출해 변하는 rank는 snapshot에서 제외한다.
- Step 9에서는 먼저 barrier-aware 제출 버전(`UGS/Migration/legacy-barrier-submit.js`)을 기존 `submit-record` 이름에 게시하고 구버전 실행을 차단/배출한 뒤에만 번호 endpoint 및 cutover를 허용한다. 이후 같은 이름을 C 귀속 endpoint로 교체하며 별도의 legacy 호출 endpoint를 만들지 않는다. Step 6 Client 준비 전 단독 게시하지 않는다. 공용 라이브러리를 원격 Script로 각각 게시하거나 raw entry만 Dashboard에 붙여 넣지 않는다. 설치된 패키지의 `Documentation~/Authoring/javascript_project.md`에서 `module.exports.bundling = true`를 확인했으며, 정확한 사용자 게시 적용표는 Step 9에서 제공한다.
- marker 기록 이후에는 구버전 submit-record를 단순 재게시하지 않는다. 현재 구현은 자동 역전환/marker 삭제를 제공하지 않는다. source가 동결된 상태에서 호환 버전으로 전진 복구한다. cutover 전 원본 미변경 상태의 rollback과 cutover 후 호환 복구를 구분한다.

#### 검증과 현재 수동 작업

- Node: Account service 20/20, provisioning 19/19, 기존 submit/query 대역 22/22 통과. 신규/기존 전환 각각 모든 쓰기 지점의 commit 전 실패와 commit 후 응답 유실을 반복 주입했다. 실제 기존 submit-record와 cutover의 source CAS 양방향 경합도 SDK 대역으로 검사했다. Phase 1 인계 계약 검사와 정적 계약 검사도 통과했다.
- 실제 UGS의 동일 Item batch 원자성, 서비스 토큰 Private/Protected 접근, 최초 생성·CAS의 read-after-write 관찰, remote crypto 및 의존성 bundling, 기존 행/metadata 보존은 Step 10에서 확인한다. 현재 대역 성공을 원격 성공으로 처리하지 않는다. 128 entries/100명 제한은 그대로다.
- 현재 수동 작업은 없다. Unity 코드/Scene/Prefab은 변경하지 않았으며 Unity 빌드·Test Runner·Editor·원격 요청/게시를 실행하지 않았다. Step 3부터 계속 진행한다.

## Step 3. 이전 시작·자격 증명·재발급·취소·만료를 구현한다

### Step 3-1. 이전 시작·자격 증명 생성·잠금을 구현한다

2-1의 권한·예약을 사용해 `TransferPending` 전이와 CSPRNG·digest/HMAC·코드 유일 예약을 구현한다. 생성 충돌·Secret 누락·저장 중단·응답 유실에서 재개 또는 안전한 실패를 검증한다.

- [x] 시작·잠금·원문 비보관·생성 충돌 Test가 통과했다.

### Step 3-1 수행 결과 — 2026-10-03

완료 범위는 서버 이전 시작 소스와 실제 서비스에 SDK 대역·가짜 시각·난수 공급자를 주입한 로컬 Node 검증이다. 원격 게시·Secret 설정·실서비스 검증은 수행하지 않았다.

#### 구현과 실패 복구

- `UGS/CloudCode/transfer-cryptography.js`: 서버 `crypto.randomBytes`로 Crockford Base32 8자리 코드와 선행 0을 보존하는 십진 9자리 인증값을 생성한다. 코드 표시는 4-4이며 인증값은 rejection sampling으로 편향을 없앤다. 난수/Secret 오류에 기본값을 쓰지 않고 실패한다. 난수 rejection은 최대 32회다.
- 코드 조회는 환경/프로젝트 범위를 포함하는 HMAC-SHA256 digest이며, 자격 증명 HMAC은 transferId·credentialRevision·정규화 코드·인증값·환경을 결합한다. 코드·인증값·Secret 원문은 저장 요청과 로그에 넣지 않는다. 서버 Secret은 canonical base64 32바이트 키만 허용한다.
- `UGS/CloudCode/transfer-start-service.js`: 인증 활성 A의 PlayerBinding·Account·revision을 확인하고 Step 2의 제출 예약 및 C ledger 검사를 재사용한다. `account-service.js`는 active 표시뿐 아니라 entries의 Pending도 차단한다. 제출 예약과 시작은 동일 Account write lock CAS로 경쟁하므로 선행 검사가 오래되어도 둘 다 성공할 수 없다.
- Account CAS로 `TransferPending` 잠금과 비활성 HMAC 자격 증명을 기록한 뒤, Step 2의 immutable guard batch를 사용해 `fs8-t-<digest>` 조회를 유일 예약한다. 43자리 base64url digest의 Custom ID는 49자로 제한 안에 든다. 조회 예약 및 Account 자격 증명 활성화의 재확인이 끝난 최초 요청만 원문을 반환한다. 공개 번호·고정 행 소유자·기록·연결은 변경하지 않는다.
- 다른 Account의 코드 예약은 덮어쓰지 않고 난수를 다시 생성한다(최대 8개 후보). 최초 생성 guard의 409 경합은 조회를 다시 읽고 최대 3회 예약을 시도한다. 다른 오류에 write lock을 생략하지 않는다. 후보 소진/잠금 이후 저장 실패는 안전하게 잠금을 유지한다.
- 이미 Pending인 요청은 저장된 digest 예약·활성화를 재개할 수 있지만 원문은 반환하지 않고 `CredentialReissueRequired`를 반환한다. 응답 유실·재시작으로 원문을 잃으면 Step 3-2의 재발급을 사용한다. 시작 도중 90일 만료/서버 시각 역전도 원문 반환을 막으며, 실제 만료 처리·잠금 해제·상태 조회는 Step 3-3에 남긴다.
- `UGS/CloudCode/start-account-transfer.js`: Client 입력 없는 bundled endpoint다. Secret/SDK 오류 내용·토큰·ID·digest/HMAC을 로그/오류 응답에 넣지 않고 고정 일반 오류만 반환한다. 공용 의존성을 포함해 Step 9에서 게시하며 지금 단독 게시하지 않는다.

#### Secret 적용 경계

`FS_TRANSFER_HMAC_VERIFICATION_V1`을 주입된 `secretManager.getSecret(name)`의 `.value`에서 읽는다. Step 9에서 verification 환경 수준의 32바이트 CSPRNG 키/base64 값, 접근 권한 및 환경 범위를 적용·확인한다. 값은 채팅·저장소·로그에 전달하지 않는다. 공식 API는 환경→프로젝트→조직 fallback과 최대 5분 캐시를 사용하므로 환경 Secret의 실제 존재를 확인해야 하며, 회전을 즉시 반영한다고 가정하지 않는다. 회전 및 기존 자격 증명 무효화 절차는 후속 운영 적용에서 검증한다. [Unity Secret Manager Cloud Code Scripts 통합 문서](https://docs.unity.com/en-us/services/secret-manager/tutorials/integrations/cloud-code/scripts)를 2026-10-03 확인했다.

#### 검증과 현재 수동 작업

- `UGS/Tests/transfer-start.test.cjs` 20/20 통과: 실제 CSPRNG 형식, 정규화/선행 0/HMAC 결합, Secret·난수 오류, 같은 Account 시작 경합, 서로 다른 Account 코드 충돌/소진, 제출과 시작의 양방향 CAS 경합, 모든 시작 쓰기 경계의 commit 전/후 실패, 응답 유실·재시작·원문 비보관, 만료/시각 역전 및 endpoint 일반 오류를 검사했다.
- 기존 Account service 20/20·provisioning 19/19·Cloud Code 22/22와 Phase 1 정책/인계 검사가 통과했다. `node --test`로 전체 18개 `.test.cjs` 파일이 통과했으며 신규 정적 계약, JS 구문 검사와 `git diff --check`도 통과했다. SDK 대역 성공은 실제 Cloud Save 원자성/권한/관찰 보장의 증거가 아니므로 Step 10에서 확인한다.
- 현재 사용자 수동 작업은 없다. Unity 코드·Scene·Prefab 변경, Unity 빌드·Editor·Test Runner, 원격 요청·게시·Secret 생성은 수행하지 않았다. Local Save Pending 시작 차단은 Step 6, C 귀속 제출/조회 연결은 Step 5, 재발급/5초 제한은 Step 3-2에 남긴다. Step 3-1만 완료 처리하며 Step 3 전체는 3-2·3-3 완료 전까지 미완료다.

### Step 3-2. 재발급과 인증 시도 제한을 구현한다

동일 이전 요청의 재발급·기존 코드 즉시 무효화·5초 검증 제한을 구현한다. 재발급과 검증 경합·실패 횟수 무제한·선행 0·입력 정규화를 검사한다.

- [x] 재발급·즉시 무효화·검증 제한 Test가 통과했다.

### Step 3-2 수행 결과 — 2026-10-03

완료 범위는 활성 A의 서버 재발급과 Step 4에서 사용할 내부 자격 증명 검증/반복 제한 경계 및 로컬 Node 검증이다. B 연결 교체·완료 endpoint·취소/만료 처리는 구현하지 않았다.

#### 구현과 권한 경계

- `UGS/CloudCode/transfer-start-service.js`의 `reissue(context)`와 공통 발급 경계를 구현했다. 인증된 활성 A·PlayerBinding·Account revision·같은 미만료 TransferPending을 확인하며 `transferId`, 최초 발급/만료 시각, 연결 및 공개 번호/고정 소유 행을 유지한다. 자격 증명 revision만 증가시키며 검증 시각·실패 누적 수를 초기화하지 않는다.
- Secret·난수 검증 뒤 Account CAS 한 번으로 기존 digest/HMAC을 새 revision의 비활성 자격 증명으로 교체한다. 이 CAS부터 기존 lookup은 권한이 없으며 새 lookup 유일 예약·Account 활성화 재확인 전에는 새 원문도 반환하지 않는다. 발급 공통 경계의 충돌 재생성·8회 제한·guard 409 복구를 재사용한다. 이전 코드로 재생성되어도 이전 revision의 예약을 재사용하지 않는다.
- 오래된 lookup은 삭제하거나 다른 이전에 덮어쓰지 않는다. 조회는 후보 Account를 찾는 수단일 뿐, Account 본문의 활성 상태·현재 digest·transferId·credentialRevision·연결 revision·만료를 모두 대조해야 한다. lookup 정리/재활용 용량은 이 Step에서 입증하지 않으며 천만 동시 활성 서비스 용량을 달성했다고 판정하지 않는다.
- `UGS/CloudCode/reissue-account-transfer.js`는 Client 입력 없는 bundled endpoint다. Step 9 전에는 게시하지 않으며 오류/로그는 고정 일반 메시지만 사용한다. Secret 설정은 기존 Step 3-1 이름/범위와 Step 9 절차를 따른다.
- `UGS/CloudCode/transfer-verification-service.js`는 공개 Script가 아닌 내부 의존성이다. 인증 context·환경, 코드 정규화·lookup·Account의 현재 자격 증명과 서버 시각을 검증한다. HMAC은 `crypto.timingSafeEqual`로 비교하고 인증값을 숫자로 변환하지 않는다. 알려진 활성 코드의 잘못된 인증값 형식도 실패 시도로 기록한다. 형식 오류/없는 코드/구버전 lookup은 어떤 유효 이전 요청인지 확정할 수 없거나 비활성이므로 해당 Account를 변경하지 않는다.
- 같은 transferId의 모든 B 시도는 Account의 마지막 시도부터 5,000ms 이상일 때만 허용하며 성공·실패 모두 제한을 갱신한다. 4,999ms는 거부, 5,000ms부터 허용, 서버 시각 역전은 거부한다. 제한은 `TooManyRequests`와 안전한 재시도 시각을 반환한다. 저장된 시각/실패 누적 수가 손상되면 실패하며, 최초 시도 전의 명시적 null만 시도 이력 없음으로 인정한다.
- 시도 시각과 실패 누적 수를 같은 Account CAS로 기록한다. 실패 누적 수는 영구 차단 조건이 아니며 안전 정수 상한에서는 포화시켜 카운터 범위 때문에 인증을 차단하지 않는다. 재발급·다른 B 검증이 먼저 CAS를 확정하면 오래된 비교 결과를 재시도하지 않는다. 저장 후 현재 자격 증명과 만료를 다시 확인한다.
- 내부 `Verified` 결과의 Account snapshot/write lock은 **연결 교체 권한이나 Client 응답이 아니다**. Step 4 완료 경계는 이 정확한 Account token의 CAS와 commit 직전 만료·연결·자격 증명 재확인을 사용해야 한다. 확인 snapshot 직후 재발급이 확정되어도 오래된 token의 연결 교체 CAS는 실패한다. B 적격성·Local Save Pending·연결 교체 및 응답 축소는 Step 4~6에 남긴다.

#### 저장 중단·응답 유실

재발급 첫 CAS 전 오류는 기존 자격 증명을 유지한다. CAS 이후 오류는 기존 자격 증명이 이미 무효화된 잠금 상태를 유지하며 Step 3-1의 시작 재조회 경계로 hash 예약/활성화를 재개하거나 다시 재발급한다. 원문은 저장·복원하지 않고 `CredentialReissueRequired`를 안내한다. 검증 시도 commit 전 오류는 다시 시도할 수 있으나 commit 후 응답 유실은 시각·실패 누적 수와 5초 제한을 보존하여 이중 집계를 막는다. 취소/만료 복구·조회 endpoint는 Step 3-3에서 구현한다.

#### 검증과 현재 수동 작업

- `UGS/Tests/transfer-reissue.test.cjs` 24/24 통과: 동일 요청/최초 만료 유지, 구 코드 즉시 무효화, 충돌 재생성/소진, 5초 직전/동일/직후·B 간 공유·성공 제한·시각 오류, 무제한 실패/카운터 포화, 입력 aliases/선행 0, 두 재발급/두 검증 CAS 경합, 재발급↔검증 양방향 경합, stale verified token 차단, confirmation 전/후 재발급·만료, 모든 재발급 쓰기의 commit 전/후 실패·응답 유실·원문 비보관과 endpoint 오류 비노출을 검사했다.
- Step 3-1 이전 시작 20/20, Account service 20/20·provisioning 19/19·기존 Cloud Code 22/22도 통과했다. `node --test` 전체 19개 `.test.cjs` 파일, Phase 1 정책/인계, 정적 계약·JS 구문 검사와 `git diff --check`가 통과했다. SDK 대역 검증은 실제 Cloud Save의 원자성·권한 보장을 대신하지 않는다.
- 현재 사용자 수동 작업은 없다. Unity 코드·Scene·Prefab 변경, Unity 빌드·Editor·Test Runner, 원격 요청·게시·Secret 생성은 수행하지 않았다. Secret 설정은 Step 9, 실제 서비스 확인은 Step 10에서 수행한다. Step 3-2만 완료 처리하며 Step 3 상위 항목은 Step 3-3 완료 전까지 미완료다.

### Step 3-3. 취소·만료·상태 재조회를 구현한다

90일 서버 시각 기준 만료, A 취소, 잠금 해제·자격 증명 비활성화, 재시작 뒤 상태 조회를 구현한다. 90일 경계와 취소/만료 경합·부분 실패를 검증한다. B 완료와의 경합은 4-2에서 합쳐 검사한다.

- [x] 취소·만료·재조회·복구 Test가 통과했다.

### Step 3-3 수행 결과 — 2026-10-03

완료 범위는 활성 A의 취소·서버 시각 기반 만료·재시작 상태 조회와 실제 서버 서비스/SDK 대역 Node 검증이다. B 연결 완료와의 실제 경합은 예정된 Step 4-2에서 합쳐 검사하며 이 Step에서 B 연결을 구현하지 않았다.

#### 구현·상태 전이

- `UGS/CloudCode/transfer-state.js`로 활성 A의 인증 context·PlayerBinding·Account·revision 대조와 Pending 저장 상태 검증을 공유한다. Step 3-1의 시작/재발급도 이 검증을 사용하며 시간·자격 증명·시도 자료가 손상되면 임의 복구하지 않는다.
- `UGS/CloudCode/transfer-lifecycle-service.js`는 `cancel`·`expire`·`status` 경계를 제공한다. 취소/만료는 정확한 Account write lock CAS 한 번으로 Account를 Active로 복원하고 transfer를 Cancelled/Expired, credentialActive=false로 확정하며 digest/HMAC 필드를 제거한다. A 연결·connectionRevision·공개 번호·고정 행 소유 ID·ledger·PlayerBinding은 유지한다. 서버 terminal 시각과 실패 누적 자료는 보존한다.
- 90일 직전에는 Pending이며 정확한 expiresAt 이상에서 Expired를 확정한다. 상태 조회가 필요한 만료를 지연 처리하므로 Secret·Client timer·예약 작업 없이 재시작 이후에도 복구할 수 있다. 이미 Expired/Cancelled면 다시 쓰지 않고 같은 결과를 반환한다. 자동 만료 작업을 별도로 등록하지 않았으며, 요청이 없으면 저장된 Pending은 조회/후속 서버 처리 때까지 남을 수 있다. 자격 증명의 시간 검증은 그동안에도 만료된 값을 거부한다.
- 기존 Phase 1 resolution 정책대로 A 취소와 만료가 경합하면 먼저 terminal CAS를 확정한 결과를 유지한다. 만료 시각 이후 A 취소가 만료 처리보다 먼저 확정되면 Cancelled이며 두 결과 모두 A 연결 유지·자격 증명 폐기로 끝난다. 다른 terminal 상태를 임의로 Active로 되돌리지 않는다. B 완료/취소/만료 조합은 Step 4-2에 남긴다.
- 취소는 상태 조회에서 받은 같은 `transferId`를 필수 상관 ID로 사용한다. ID만으로 권한을 부여하지 않으며 인증된 활성 A와 현재 Account를 대조한다. 이전 transferId의 늦은 취소가 새 요청을 취소하지 않는다. 종료 후 새 이전은 새로운 요청 ID·새 자격 증명·새 90일 기산점을 사용한다.
- 상태 응답은 Active/TransferPending, 이전 상태, 이전 상관 ID, Pending의 만료 시각과 `credentialReissueRequired`로 제한한다. 재시작 조회에서 원문을 반환/복원하거나 비활성 발급을 자동 활성화하지 않는다. 코드·인증값·HMAC/digest·내부 Player/Account ID·공개 번호·실패 누적 수는 이 응답에 넣지 않는다. 반환된 상태는 Client 권한이 아니며 Step 5의 서버 기록 경계가 연결을 다시 확인한다.
- `UGS/CloudCode/cancel-account-transfer.js`는 필수 String `transferId`, `UGS/CloudCode/get-account-transfer-status.js`는 입력 없는 bundled endpoint다. 둘 다 Secret 없이 동작하고 오류/로그는 고정 일반 메시지만 반환한다. Step 6 Client에서는 취소 전 상태 조회의 현재 상관 ID를 사용한다. Step 9 전에는 Script를 게시하지 않는다.

#### 저장 중단·재개·확인 경계

terminal CAS commit 전 실패는 Pending과 A 연결을 유지하며 재호출로 확정한다. commit 후 응답 유실은 이미 Active/terminal/자격 증명 폐기가 함께 기록되므로 재호출·재시작 조회로 같은 결과를 복원한다. lookup은 tombstone으로 남아도 Account의 현재 상태/자격 증명 대조가 권한을 막으므로 별도 lookup 쓰기 실패에 의존하지 않는다. 늦은 lookup 생성·재발급·검증·취소는 오래된 Account CAS로 terminal 결과를 되돌릴 수 없다. terminal 저장 후 새 이전이 확인 전에 시작되면 오래된 완료 응답을 성공 처리하지 않고 다시 조회하도록 실패한다.

#### 검증과 현재 수동 작업

- `UGS/Tests/transfer-lifecycle.test.cjs` 22/22 통과: A 자료 보존/잠금 해제/검증 자료 제거, Secret 없는 조회, 원문/내부 필드 비노출, 90일 직전/동일/직후, 취소↔만료 단일 winner·동시 조회, 각 terminal CAS commit 전/후 실패·응답 유실·재시작, 지연 취소 generation fence, 새 이전/새 기산점, 비활성 발급 취소, 환경·활성 연결·시각·저장 상태 오류, 취소↔재발급/검증 경합, 늦은 lookup과 만료 후 stale verified token 차단, 확인 전 새 이전 및 endpoint 오류 비노출을 검사했다.
- 기존 시작 20/20·재발급/검증 제한 24/24·Account service 20/20·provisioning 19/19·기존 Cloud Code 22/22와 정책/인계 검사가 통과했다. `node --test` 전체 20개 `.test.cjs` 파일, 정적 계약·JS 구문 검사와 `git diff --check`가 통과했다. 공유 검증 분리 중 발견한 시각 역전 오류 분류 회귀를 기존 TransferExpired 계약으로 수정하고 전체 검사를 다시 통과시켰다. Test의 공개 번호 메서드 참조 오류도 실제 `getPublicNumber`로 수정했다.
- 현재 사용자 수동 작업은 없다. Unity 코드·Scene·Prefab 변경, Unity 빌드·Editor·Test Runner, 원격 요청·게시·Secret 생성은 수행하지 않았다. 실제 Cloud Save 원자성/권한·관찰 및 서버 runtime 보장은 Step 9~10에서 확인하며 로컬 성공을 원격 성공으로 기록하지 않는다.
- Step 3-1~3-3 및 Step 3 상위 항목을 로컬 서버 구현 범위에서 완료 처리한다. 다음은 Step 4-1 B 연결 완료·기존 B 계정 분리다. B 완료 후 A 상태 안내, 실제 완료/취소/만료 경합, Client/Scene 및 Phase 2 전체는 후속 Step으로 남긴다.

### AI 수행

활성 A와 제출 예약/Pending 조건, `TransferPending` 잠금, CSPRNG 코드·인증값 생성, 환경별 활성 코드 유일 예약·충돌 재생성, digest/HMAC·Secret 주입, 서버 시간·5초 제한을 구현한다. 재발급은 기존 값을 즉시 무효화하며 취소·만료는 A 연결을 유지·복원한다. 원문을 저장·로그에 남기지 않고 발급 응답에서만 보여준다.

Unit Test: 코드 정규화·선행 0, 생성 충돌 재시도·실패, 재발급/취소/만료의 저장 중단·재개, 90일·5초 직전/동일/직후, 인증 실패 누적으로 영구 차단하지 않음, 이전 화면 종료·앱 재시작 후 원문 복원 없음. 천만 개 목표는 공간·충돌 처리 검증과 서비스 제약을 별도로 기록하며 천만 계정을 실제 생성하지 않는다.

### 사용자 수동 작업

없음. Secret 설정은 Step 9에서만 사용자에게 정확한 이름·범위·권한을 제공한다.

### 완료 조건

- [x] 시간·잠금·자격 증명·원문 비보관의 생산 서비스 Unit Test가 통과했다.
- [x] Secret 누락·저장 실패에서 안전하게 실패하고 연결을 잘못 바꾸지 않는다.

## Step 4. B 연결과 단일 활성 연결 검증을 구현한다

### Step 4-1. B 연결 완료와 기존 B 계정의 분리를 구현한다

Step 2~3의 기반으로 자격 증명 검증·C 연결·코드 소진·B의 기존 계정 연결 해제를 구현한다. B의 기존 온라인 기록·번호·행을 보존하고 동일 B 완료 재호출의 결과를 고정한다.

- [x] B 연결·원래 계정 자료 보존·동일 B 재호출 Test가 통과했다.

### Step 4-1 수행 결과 — 2026-10-03

완료 범위는 서버 B 연결 완료·기존 B 계정 연결 분리·응답 유실 복구 및 실제 서비스/SDK 대역 Node 검증이다. 모든 종류의 연결 경합 조합과 원격 보장은 Step 4-2 및 Step 10에 남긴다. Client Local Save Pending·UI는 Step 6에서 연결한다.

#### 서버 완료 경계와 자료 보존

- `UGS/CloudCode/transfer-completion-service.js`와 bundled endpoint `UGS/CloudCode/complete-account-transfer.js`를 구현했다. 입력은 String `code`·`verificationValue`뿐이며 인증 context의 B를 사용한다. Client Account ID·공개 번호·소유 행 ID로 권한을 선택하지 않는다. Step 3의 코드 정규화·선행 0·Secret HMAC·5초 검증 제한과 검증 후 Account write lock을 재사용한다. 알려진 코드의 잘못된 인증값 형식도 검증 시도로 처리한다.
- B는 Step 2에서 발급된 유효 PlayerBinding과 Active 논리 계정을 사용한다. B 기존 기록 보유 자체는 차단 조건이 아니다. 다만 이미 시작한 서버 제출 예약/ledger Pending은 버리지 않고 확정·복구 후 완료를 다시 시도하도록 한다. Local Save Pending 없음은 이 서버에서 확인할 수 없으며 Client가 Step 6에서 요청 전에 실제 저장소로 차단해야 한다. 내부 resolution 정책의 `hasLocalPending: false`는 서버가 Local Save를 검증했다는 근거가 아니다.
- 여러 Item을 단일 transaction으로 취급하지 않는다. B PlayerBinding의 `transferOperation` CAS 예약 → 기존 B Account의 `TransferJoining` CAS fence → 정확한 검증 token의 C terminal CAS → 기존 B Detached → B/A 매핑 복구 → C Active 순서의 재개 가능한 saga다. C terminal CAS는 currentPlayerId=B, connectionRevision+1, Completed/credentialActive=false 및 `TransferCompleting`을 함께 확정한다. commit 직전 원래 만료·자격 증명·revision을 다시 검사한다.
- `TransferJoining`/`TransferCompleting`과 PlayerBinding 예약은 일반 온라인 권한을 주지 않는다. `account-service.js`·`transfer-state.js`에서 예약/Inactive binding을 차단한다. 예약 직전 B 제출이 먼저 확정된 경우에는 그 동일 submission ID의 Resume/terminal release만 허용하여 기존 제출을 마칠 수 있게 하며 새 제출·조회·이전 시작은 차단한다. 기존 B CAS가 먼저 잠기면 오래된 제출 예약은 확정될 수 없다.
- 기존 B Account는 currentPlayerId=null·Detached 및 기존 revision+1로 분리한다. B의 기존 번호·고정 소유 ID·ledger·receipt·최고 기록·Leaderboard 행/metadata는 그대로 남고 C로 복사/병합/삭제하지 않는다. C의 번호·고정 행·ledger도 유지한다. B PlayerBinding은 C와 새 revision으로 바꾸고 A PlayerBinding은 Inactive로 표시한다. Account 본문과 매핑 대조가 완료되기 전에는 C를 Active로 열지 않는다.
- C 완료 확정 이후 취소/만료 요청이 A 연결을 복원하지 않는다. A의 완료 상태 조회는 Inactive/Completed와 이전 상관 ID만 반환하며, A의 기록·취소·새 이전 요청은 활성 연결 검사에서 거부한다. B의 완료 상태는 Active/Completed다. 상태 응답은 원문·HMAC/digest·내부 Account/Player ID를 노출하지 않는다.

#### 재호출·receipt·실패 복구

- 완료가 확정된 같은 B·같은 자격 증명은 시간 제한을 다시 소비하거나 revision을 증가시키지 않고 `AlreadyCompleted`와 C 번호만 재확인한다. 다른 B 또는 잘못된 인증값은 일반 `InvalidCredential`만 받는다. 완료 이후 만료 시각이 지나도 같은 B의 확정 결과 확인은 허용하되 새 연결을 만들지 않는다.
- 서버 Private `fs8-receipt-<transferId>`의 immutable completion receipt를 추가했다(Custom ID 48자). 원문 없이 digest/HMAC·완료 B/revision·C 번호/상관 ID만 보존하고 guarded 최초 생성으로 덮어쓰지 않는다. 이후 B가 같은 C에서 새 이전을 시작해 현재 transfer가 바뀌어도 이전 요청 재호출은 receipt와 현재 C/B 권한을 대조하여 기존 완료 결과만 반환하며 새 Pending을 바꾸지 않는다. B가 더 이상 C의 현재 연결이 아니면 과거 receipt만으로 다시 연결하지 않는다. receipt 보관/정리 용량은 이 Step에서 운영 검증했다고 기록하지 않는다.
- C terminal CAS 전 저장 실패/경합은 B 예약을 유지해 재시도할 수 있다. Pending 상태에서 임의 rollback으로 다른 완료 실행과 경쟁하지 않는다. 취소/만료·다른 B terminal 결과가 이미 확정되었으면 해당 B의 자기 예약만 되돌린다. C가 같은 B Completed로 확정된 경우에는 rollback하지 않고 receipt·기존 B 분리·매핑·C 활성화를 앞으로 복구한다. 각 단계는 동일 ID/revision/예약을 대조해 멱등 재개하며 오류를 write lock 생략으로 우회하지 않는다.
- 초기 verification commit 후 응답 유실이고 아직 terminal이 아니면 기존 5초 제한을 보존한다. Client 자동 무한 재시도는 하지 않는다. C terminal 이후 응답 유실은 같은 B의 유효 자격 증명 재호출로 `AlreadyCompleted` 복구한다. Script 일반 오류는 SDK/Secret 메시지·원문·토큰·ID를 반환하거나 로그에 넣지 않는다. Secret/runtime 장애에서는 잠금을 임의로 해제하지 않는다.

#### 검증과 현재 수동 작업

- `UGS/Tests/transfer-completion.test.cjs` 18/18 통과: C 연결/revision·양쪽 번호/행 귀속 보존, 기존 B receipt/best/metadata 보존, 같은 B 재호출/만료 뒤 확인·다른 B/오인증 거부, A/B 완료 상태·A 차단, context/Secret/입력 오류, 자체 A 연결 거부, 5초 제한, B 서버 Pending/예약 차단, 만료 terminal 처리, 각 부분 상태 권한 차단, 모든 완료 쓰기의 commit 전/후 실패·재시작 재개·원문 비보관, 취소 선점, B 제출 선점 후 동일 제출 복구, 새 이전 이후 과거 receipt 재호출, A 취소 후 B 예약 복구 및 endpoint 오류 비노출을 검사했다.
- 기존 취소/만료/조회 22/22·재발급/제한 24/24·시작 20/20·Account service 20/20·provisioning 19/19·Cloud Code 22/22와 정책/인계 검사도 통과했다. `node --test` 전체 21개 `.test.cjs` 파일, 정적 계약·JS 구문 검사와 `git diff --check`가 통과했다. 테스트 작성 중 B 사전 부적격 상태의 예약 잔존을 발견해 예약 전 사전 검사와 안전한 재개 정책을 보완했고, 원문 비보관 검사에서 테스트 인증값이 공개 번호 부분 문자열과 겹치는 오류는 가짜 난수 fixture를 변경해 해소했다.
- 현재 사용자 수동 작업은 없다. Unity 코드·Scene·Prefab 변경, Unity 빌드·Editor·Test Runner, 원격 요청·게시·Secret 생성은 수행하지 않았다. endpoint 단독 게시는 하지 않는다. Step 5의 실제 제출/조회 C 귀속, Step 6 Client Pending·UI, Step 9 배포/Secret, Step 10 실제 저장/권한 보장은 아직 후속 작업이다.
- Step 4-1만 완료 처리한다. Step 4-2에서는 두 B·완료/취소/만료·재발급·지연 A 및 복수 계정/예약의 종합 경합, rollback/복구 자체 실패와 모든 read/write 중단 조합을 확장한다. 이번의 순차 쓰기 경계 검증만으로 종합 경합/원격 보장이 모두 검증되었다고 판정하지 않는다. Step 4 상위 항목과 Phase 2 전체는 미완료다.

### Step 4-2. 연결 교체 경합과 부분 실패 복구를 검증한다

두 B, 완료/취소/만료, 제출 예약/이전 시작의 경합을 실제 서비스 코드와 제어 가능한 저장소 대역으로 검증한다. 각 쓰기 실패·응답 유실·재시작·A 지연 요청을 주입해 단일 winner와 매핑 복구를 확인한다.

- [x] 경합·중단·재개 Test가 통과했고 미확인 원격 보장을 Step 10에 연결했다.

### Step 4-2 수행 결과 — 2026-10-03

완료 범위는 Step 4 생산 서비스의 제어 가능한 경합/장애 주입과 복구 수정 및 Node 검증이다. 실제 Unity/Client·기존 제출/조회 endpoint 통합·UGS 저장 보장은 완료 범위가 아니다. 가능한 모든 실행 순서를 수학적으로 입증했다고 주장하지 않고 아래 재현한 실행 경계와 trace를 근거로 판정한다.

#### 자동 경합·장애 주입

- `UGS/Tests/transfer-races.test.cjs` 22/22 통과. Step 4-1의 실제 서비스 fixture를 재사용하며 저장소/Clock/Secret 대역으로만 제어한다. 동시 두 B와 5초 뒤 재개한 두 번째 B, 같은 B 중복 요청, 한 B의 서로 다른 두 C 연결 경합에서 terminal 연결·revision·기존 계정 분리·현재 매핑의 단일 winner를 확인했다.
- 완료 선점 뒤 취소/만료 거부, 만료 선점 뒤 B 예약 복원, 재발급 선점 뒤 오래된 완료 token 거부/새 코드 재개, A 지연 제출/조회·오래된 Account token 거부를 확인했다. 시작↔제출 예약의 양방향 CAS 경합은 기존 시작/Account service Test와 함께 재실행했다. B 기존 기록·번호·행·metadata 보존은 Step 4-1 회귀 Test를 그대로 통과했다.
- 성공 완료 trace의 SDK 읽기 31곳, Secret 없는 terminal 복구 trace의 읽기 18곳, 취소/rollback trace의 읽기 22곳에 각각 한 번씩 오류를 주입하고 재시작 재호출로 확정 결과와 매핑이 수렴함을 확인했다. terminal 복구의 쓰기 6곳은 commit 전/후 각각 실패시켰다. rollback Account/binding 쓰기와 B 자체 이전의 예약 해제에도 commit 전/후 오류를 주입했다. 모든 성공 완료 쓰기의 commit 전/후 검증은 기존 Step 4-1 Test를 재실행했다. 이 숫자는 현재 자동 Test trace의 경계 수이지 UGS의 장애 보장/가능한 모든 조합 수가 아니다.
- 동시 복구에서도 Detached/C connectionRevision을 두 번 증가시키지 않는지, 원문 없는 미확정 예약은 C 연결 권한을 얻지 못하는지, 다른 B/다른 환경은 terminal 복구를 하지 못하는지, 과거 receipt 확인 중 C 연결이 바뀌면 오래된 성공 응답을 버리는지 검사했다.

#### 발견한 문제와 수정

- B 자신의 이전 시작이 B Account fence를 먼저 선점하면 기존 완료 예약 때문에 자기 취소까지 막히는 경합을 재현했다. `transfer-completion-service.js`의 `releaseOwnPendingFence`는 B 자체 TransferPending이 이미 오래된 Joining CAS를 차단하는 경우에만, 그 자체 이전을 취소하지 않고 아직 연결되지 않은 binding 예약만 해제한다. Active/Joining 예약을 임의 시간으로 해제하지 않는다.
- 취소 후 새 이전과 B 복구가 끝난 뒤 늦은 Joining 쓰기가 도착하면 B만 다시 잠기는 경합을 재현했다. rollback은 Account에 `revokedJoiningTransfer`를 먼저 기록해 이전 write lock을 fence하고, 새 token을 본 worker도 같은 철회 작업을 거부하게 한다. 기존 B Account를 읽은 뒤 binding 예약/old revision도 재확인한다. 단순 token 회전 뒤 binding 해제 사이에 생기는 gap까지 철회 marker로 차단한다. marker는 특정 예약의 무효화 근거이며 Client 권한이나 다른 요청의 차단 조건이 아니다.
- 완료가 commit된 뒤 앱 재시작으로 원문을 잃으면 partial 매핑을 복구할 방법이 부족했다. `transfer-completion-service.js`에 인증된 `recover(context)`를 추가하고 `get-account-transfer-status.js`가 이를 먼저 호출하도록 연결했다. Account ID 입력·Secret·코드/인증값 없이도 binding에서 도출한 같은 B와 서버 Completed terminal/예약이 일치할 때만 앞으로 복구한다. 이미 확정된 연결의 재개이지 새 연결 승인이 아니다.
- 아직 terminal이 아닌 예약은 `TransferPending`/`TransferRecoveryPending`만 반환하고 자격 증명 없이 연결하지 않는다. A 취소/서버 만료/다른 B terminal이 확정되면 자기 예약을 되돌리며, 만료 시각에는 Secret 없이 Expired와 A 유지/기존 B 복원을 처리한다. 정상 Pending의 자격 증명 재입력·원본 A 재발급/취소 UI는 Step 6에 연결한다.
- completion receipt에 원본 A를 기록하고 Inactive binding에 `inactiveByTransferId`를 보존했다. B가 새 이전을 시작해 현재 transfer가 바뀌어도 A 재시작 상태 조회는 자기 완료 receipt로 Inactive/Completed를 확인한다. 과거 receipt의 B 성공 응답 직전에는 현재 Account와 binding을 재확인하여 그 사이 다른 기기로 연결된 B에게 이전 번호를 성공 표시하지 않는다.

#### 로컬 근거와 Step 10 미확인 보장

| 로컬 확인 | 아직 확인하지 않은 원격 보장 | 후속 확인 |
| --- | --- | --- |
| SDK 대역의 Account/binding CAS 및 receipt guard 경합 | 실제 Cloud Save write lock 강제·동일 Item guard batch 원자성·서비스 토큰 권한 | Step 10-1 및 10-3에서 실제 CAS 충돌/Private 접근과 guard 동시 생성 확인 |
| 부분 매핑은 신규 Account 권한 경계에서 거부·terminal만 복구 | 원격 read-after-write 관찰·동시 Cloud Code 실행·응답 유실 이후 저장 관찰 | Step 10-3에서 두 격리 세션과 서버 재조회로 확인; 주입 불가능한 경계는 미입증으로 유지 |
| Secret 없는 상태/복구 및 Node crypto·오류 비노출 | 원격 bundling/runtime·Secret 환경 범위/cache·endpoint 배포와 Player 직접 Write 차단 | Step 9 배포표와 Step 10-1/10-3에서 확인 |
| 현재 trace 읽기/쓰기 중단·재시작·기존 자료 보존 | 실제 기존 행 metadata 보존과 Client Pending/UI·실제 제출/조회 C 귀속 | Step 5~6 구현 후 Step 10-2/10-3·Step 11에서 확인 |

천만 동시 활성 코드·대규모 tombstone/receipt 용량과 Production/로그 sink 운영 준비를 이 로컬 검증으로 입증하지 않는다. 원격 원자성이나 필수 권한이 확인되지 않으면 Step 12/Phase 2 전체를 완료 처리하지 않는다. 원격 확인을 위해 기존 자료를 삭제하거나 임의 계정을 대량 생성하지 않는다.

#### 검증과 현재 수동 작업

- 신규 경합/복구 22/22·기존 완료 18/18·취소/만료/조회 22/22·재발급/제한 24/24·시작 20/20·Account service 20/20·provisioning 19/19·Cloud Code 22/22와 정책/인계 검사가 통과했다. `node --test` 전체 22개 `.test.cjs` 파일, 정적 계약·JS 구문 검사와 `git diff --check`가 통과했다. 공유 fixture를 import할 때 기존 Test가 중복 실행되지 않도록 기존 완료 Test의 실행 진입점만 분리했다.
- 현재 사용자 수동 작업은 없다. Unity 코드·Scene·Prefab 변경, Unity 빌드·Editor·Test Runner, 원격 요청·게시·Secret 생성은 수행하지 않았다. 빠른 동시 클릭·90일 대기·정밀 5초 경계를 수동 작업으로 요구하지 않는다.
- Step 4-2와 Step 4 상위 항목은 **신규 서버 Account/이전 경계의 로컬 구현·검증 범위**에서 완료 처리한다. 기존 `submit-record`/`query-records`의 실제 C 귀속 전환은 다음 Step 5이며, 지금 endpoint를 단독 게시하거나 전체 기록 경로가 통합되었다고 판정하지 않는다. Phase 2 전체·Client/Scene·원격 검증은 미완료다.

### AI 수행

B의 Anonymous 인증과 코드·인증값 검증, terminal winner, 코드 소진, Account와 역방향 매핑의 복구를 구현한다. B의 기존 온라인 자료는 원래 계정에 남기고 C와 합치지 않는다. 동일 B의 완료 재호출은 같은 결과를 반환한다. 활성 연결·revision·제출 예약·조회 snapshot 재검증을 기록 경계에 연결한다.

Unit Test: 두 B 완료 경합, 완료/취소/만료 경합, A 지연 요청, 제출 예약과 이전 시작의 양방향 차단, 각 CAS 실패·응답 유실·재시작, 같은 B 재호출, 다른 B 거부, 다른 환경 코드, B 기존 온라인 자료 보존. Client Pending 판정은 Step 6의 저장소 대역 Test로 연결한다.

### 사용자 수동 작업

없음. 빠른 동시 조작으로 단일 활성 연결을 판정하지 않는다.

### 완료 조건

- [x] 경합·복구·권한 검증의 Node Unit Test가 통과했다.
- [x] 연결 교체 중 부분 매핑을 권한으로 사용하는 경로가 없다. (Step 4의 신규 서버 Account/이전 경계; 기존 제출/조회 통합은 Step 5.)

## Step 5. 실제 제출·조회 경로를 C와 고정 행에 연결한다

### Step 5-1. 제출·최고 기록 갱신을 고정 행에 연결한다

`submit-record`를 C·활성 연결·제출 예약에 연결한다. B 후속 제출이 동일 행을 사용하고 A 지연 제출·이전 중 제출을 거부하는지, 점수와 수락 시각 보존·응답 유실 복구가 유지되는지 검증한다.

- [x] 생산 제출 경로의 변경·회귀 Node Test가 통과했다.

### Step 5-2. 조회·공개 번호·본인 행 판정을 연결한다

`query-records`에 활성 C·조회 후 revision 재검증·기존 행 번호 조회/발급·응답 축소를 연결한다. 현재 100명 한도 내에서 고정 행·공개 번호·본인 판정과 오류 비노출을 검증한다.

- [x] 조회·행 표시 계약·stale 응답·번호 발급 실패 Test가 통과했다.

### AI 수행

`submit-record`·`query-records`가 인증 호출자의 활성 C를 해석하고 고정 행을 사용하도록 전환한다. 이전 중 제출·조회는 차단한다. 기존 최고 기록·수락 시각을 유지하고 행의 공개 번호와 `(You)` 판정을 반환한다. 응답은 UI에 필요한 정보로 제한한다. 기존 입력 검증·직접 Write 차단·Pending 재시도 계약을 유지한다.

Unit Test: 이전 전후 동일 행·번호·metadata, B 후속 제출도 같은 행, A 거부, 조회된 기존 행의 번호 발급 실패, 서로 다른 계정 혼합 방지, Error에서 내부 ID 비노출. 기존 Cloud Code Test 21개를 실제 변경 계약에 맞게 확장·갱신하고 정적 계약 검사도 변경 파일과 함께 갱신한다.

### 사용자 수동 작업

없음. 실제 서비스 호출은 Step 10에서 수행한다.

### 완료 조건

- [x] 생산 제출·조회 경로의 변경·회귀 Node Test가 통과했다.
- [x] 128/100 및 receipt 정리는 Phase 3로 인계하며 기존 제한 상태를 기록했다.

### Step 5 수행 결과 — 2026-10-03

완료 범위는 5-1·5-2의 로컬 서버 코드와 SDK 대역 Node 검증이다. 실제 서비스 보장·게시·Client 표시·Unity 실행은 완료 근거에 포함하지 않는다.

- `submit-record.js`: 인증 호출자의 C·활성 연결/revision을 해석하고 C private ledger에 CAS로 기록한다. Account 제출 예약 뒤 ledger를 다시 읽으며 동일 ID는 같은 payload/수락 시각으로 복구한다. terminal 저장 뒤에만 예약을 해제한다. 이전 중·Inactive A·잘못된 연결은 기록을 쓸 수 없다. B 후속 제출은 C의 영구 `leaderboardOwnerId`를 사용한다. Stage/Infinite 검증·개선 기록만 갱신·동일/열등 기록 metadata 보존 정책은 유지한다.
- `query-records.js`: 활성 C 확인 후 전체 100명 이하 snapshot을 조회한다. 서비스가 반환한 행 소유 ID만 역매핑하며, 미전환 legacy 행은 기존 provisioning을 통해 번호를 발급한다. 모든 행 번호 매핑을 검사하고 응답 직전 C/revision/번호/소유 행을 재검증한다. 실패 시 부분 목록 없이 안전한 Error만 반환한다.
- 행 응답은 `publicPlayerNumber`(정확한 10자리 문자열), `isMe`, `score`, `acceptedAt`, `rank`뿐이다. 본인 여부는 C 일치로 판정하며 분리된 기존 B 행은 본인으로 표시하지 않는다. 같은 점수는 공동 순위, 같은 점수/시각은 공개 번호 오름차순이다. 내부 Player/Account/소유 행 ID는 응답에 넣지 않는다.
- 두 endpoint는 기존 `request: String / Required` 입력을 유지하지만 로컬 의존성 bundling이 필요하다. 현재 C# 응답 모델/표시 및 인증 직후 발급은 Step 6에서 연결한다. 기존 Client와 바로 호환된다고 보지 않으며 지금 서버만 게시하지 않는다. 확정 기록 없는 사용자의 무요청 Client gate도 유지/검증한다.
- 기존 source barrier-aware 제출 구현을 `UGS/Migration/legacy-barrier-submit.js`에 보존했다. 이는 Step 9의 동일 Script 이름에서 일시적으로 사용하는 cutover bridge이며 새 endpoint가 아니다. migration 후 legacy 버전으로 rollback하거나 source marker를 삭제하지 않는다. 기존 source CAS 양방향 경합 Test는 이 보존 버전으로 계속 실행한다.
- 검증: 기존 Cloud Code 회귀 22/22, 신규 실제 endpoint/C 통합 19/19, provisioning 19/19 및 전체 23개 `.test.cjs` 파일 통과. Account/이전 시작/재발급/취소/완료/경합 Test도 재실행했다. 정적 계약·모든 서버/bridge JS 구문·`git diff --check` 통과. 제출 4개 저장 경계의 commit 전 실패/후 응답 유실, Rejected 3개 경계, 모든 제출 SDK read 실패, 고정 행/번호/시각·A 거부·stale 조회·번호 발급 실패·응답 비노출을 자동 검증했다.
- C별 128 entries/보드 100명 단일 snapshot 제한과 terminal receipt 미정리 상태는 유지한다. 180일 분할/정리·한도 해소는 Phase 3다. 서비스 토큰의 UUID 소유 행 쓰기, 실제 keepBest metadata 보존, batch/CAS 및 read-after-write 보장은 Step 10에서 확인한다.
- 현재 사용자 수동 작업은 없다. Unity 빌드·Editor·Test Runner·Scene/Prefab 수정·원격 요청/게시를 수행하지 않았다. 다음은 Step 6-1이며 Phase 2 전체·Step 7~12는 미완료다.

## Step 6. Client·Local Save·인증·표시·이전 UI 코드를 구현한다

### Step 6-1. Local Save migration과 Pending gate를 구현한다

환경별 온라인 저장 영역·기존 자료 migration·Pending 귀속·명시적 폐기 API를 구현한다. 저장 실패 시 차단, 다른 환경 자료 재귀속 금지, A/B Pending 존재 시 요청 0회를 생산 코드 Edit Mode Test로 작성한다.

- [x] Codec·저장소·Pending gate 코드와 Test를 작성하고 정적으로 대조했다.

### Step 6-1 수행 결과 — 2026-10-03

완료 범위는 C# 코드·Edit Mode Test 작성 및 정적 대조다. Unity 컴파일·Test Runner·Player 실행 결과를 통과로 기록하지 않는다. Step 6 상위 항목은 6-2~6-5가 남아 있어 미완료다.

- `OnlineDataScope.cs`, `OnlineLocalSaveData.cs`, `LocalSaveData.cs`, `LocalSaveJsonCodec.cs`: 저장 형식을 v6로 확장했다. 현재 영역과 비활성 영역을 `(Project ID, Environment ID)`로 구분하고 인증 Player ID·동의·개인 최고·Pending을 각 영역에 보존한다. 다른 환경에서는 해당 자료를 선택/전송/병합하지 않는다. 기존 v1~v5 자료는 현재 빌드 환경과 무관하게 원래 verification ID에만 귀속하고 동일 Pending ID/소유자/값을 보존한다. Settings·튜토리얼·입력 설정과 기기 로컬 UUID는 공용이다. 번호 캐시·논리 C 연결 모델은 Step 6-2에서 이 영역에 추가한다.
- `LocalRecordRepository.cs`: 로드 때 현재 환경만 메모리에 복원하며, migration/환경 선택 결과를 원자 저장한다. 저장 실패는 `IsLocalSaveReady=false`로 온라인·이전을 차단하되 원본 파일과 Pending을 유지한다. 성공한 checkpoint로 재개할 수 있다. 손상된 scope/중복 영역/읽을 수 없는 Pending은 정상적인 빈 대기열로 취급하지 않고 fail closed한다. 손상/읽기 실패 파일을 기본값으로 덮어쓰지 않으며, 유효 파일의 재로드 전 온라인 요청을 하지 않는다. 재로드 때 메모리를 새로 만들어 다른 영역/이전 로드의 대기열이 혼합되지 않게 했다.
- `TryDiscardPending()`: 사용자 명시적 폐기용 API다. 현재 영역 Pending을 비운 자료를 저장한 뒤에만 메모리에서 제거한다. 저장 실패 시 같은 ID가 남고 이전은 차단된다. 비활성 환경 Pending·개인 최고·동의/Player ID·Settings/튜토리얼은 삭제하지 않는다. 일반 설정 checkpoint로 비활성 영역을 없애거나 교체하는 것도 거부한다.
- `AccountTransferPendingGate.cs`: A 시작과 B 완료가 사용할 공통 요청 경계를 준비했다. 모든 소유자의 현재 영역 Pending이 0건이고 configured scope가 일치하며 checkpoint가 성공한 경우에만 callback을 호출한다. 같은 저장소를 쓰는 여러 gate의 중복 요청과 요청 중 새 Pending 등록/폐기를 차단하고 finally에서 예약을 해제한다. 기존 서버 기록의 유무는 이 gate 조건이 아니다. 이 로컬 판단은 서버 권한/성공 확인이 아니며 Step 6-2의 실제 start/complete transport는 반드시 이 gate를 거쳐야 한다.
- `OnlineRecordCoordinator.cs`, `CloudCodeRecordRepository.cs`, `GameSystem.cs`: 생산 제출·조회와 인증 전에 저장 준비/환경 조건을 대조한다. `GameSystem`은 저장소의 `CanUseOnlineData`를 repository에 주입한다. 기존 저장 재생성 경로도 현재/비활성 영역·계정·Pending을 보존한다. 기존 생성자 호출은 유지하며 별도 검증 경로가 새 선택적 gate를 사용하지 않는 경우는 Step 6-5에서 다시 대조한다.
- 신규 `OnlineLocalSaveScopeTests.cs` Edit Mode **25개 사례**를 작성했다. v1~v5 migration, project/environment 불일치의 인증/제출/이전 0회, 영역 왕복·Settings/입력 보존, 저장 실패/재시작, A/B/다른 소유자 Pending 0회, 명시적 폐기 실패/성공, 타 환경 Pending 보존, checkpoint 실패, 중복 요청/새 Pending fence, callback 실패 해제, 손상 Pending/중복 scope, reload 혼합 방지 및 생산 query/submit gate를 포함한다. 실제 C# 실행은 미실행이다.
- `UGS/Tests/client-local-save-contracts.test.cjs`의 읽기 전용 C# 계약·직렬화 필드·gate 순서·asmdef 경계 참조·신규 `.meta` 형식/중복·스타일/중괄호 검사를 통과했다. 전체 Node `.test.cjs` 파일 **24/24**와 기존 정적 계약·`git diff --check` 통과. 이는 C# 컴파일/테스트 실행을 대신하지 않는다. 신규 package/asmdef 변경은 없다.
- 지금 필요한 Scene/Inspector·서비스·Local Save 수동 초기화 작업은 없다. 사용자 저장 파일을 삭제하거나 이관 도구로 변경하지 않았다. Unity Editor/Build/Test Runner·Scene/Prefab 수정·원격 요청/게시를 수행하지 않았다. v6 파일은 v5 이하 Client codec에서 읽을 수 없으므로 v6 저장 후 구 Client로 rollback하지 않는다.
- Step 7 사용자 실행 인계: `OnlineLocalSaveScopeTests` 25개와 영향받는 기존 `LocalSaveJsonCodecTests`, `OnlineRecordRepositoryTests`, Local/Memory repository·RecordSubmission·GameSystem 저장/복구 관련 Edit Mode 및 격리 `PlayModeRecordIsolationTests`를 포함한다. 정상 저장 경로에서는 신규 Warning/Error가 없어야 한다. 손상 입력 사례의 기존 고정 recovery Warning만 기대 로그로 지정했다. 실행 수·실패 이름/메시지/Stack Trace는 Step 7에 기록한다. 정적 확인 가능한 필드·환경·참조를 별도 수동 검증으로 요구하지 않는다.

### Step 6-2. 인증·서버 요청·공개 번호 모델을 구현한다

인증 직후 발급/조회, 이전 transport·서버 상태 조회, 번호 문자열·오류·Retry 모델을 구현한다. Offline Run 허용·계정 불일치·stale 응답·안전한 응답만 표시하는 Edit Mode Test를 준비한다.

- [x] 인증·transport·표시 모델 코드와 Test를 작성하고 정적으로 대조했다.

### Step 6-2 수행 결과 — 2026-10-03

완료 범위는 생산 C# 인증/요청·표시 모델·Edit Mode Test 작성과 정적 대조다. Unity 컴파일·C# Test 실행·원격 호출 결과는 미실행이며 완료 근거에 포함하지 않는다. Step 6 상위 항목은 6-3~6-5가 남아 있어 미완료다.

- `OnlineAccountCoordinator.cs`는 `IOnlineAuthenticationGateway`를 구현하고 기존 UGS 인증 gateway를 감싼다. 동의·configured scope·저장 준비·네트워크 조건을 확인하고, 인증 Player ID가 저장된 연결과 다르면 번호 요청 없이 거부한다. 처음 연결한 Player ID를 저장한 뒤 `get-public-player-number`를 즉시 요청한다. 인증 재호출에서도 서버 번호를 확인하며, 성공한 정확한 10자리 번호를 저장한 뒤에만 Ready/인증 성공을 반환한다. 실패한 번호·다른 계정의 번호·unknown reason/원본 예외는 화면에 전달하지 않는다.
- `OnlineAccountState.cs`와 `LocalSaveJsonCodec.cs`에 환경별 `PublicNumberCache` 문자열을 추가했다. 이전 v6 파일에 필드가 없어도 빈 캐시로 읽는다. cache는 표시 권한/복구 근거가 아니며, 재시작 초기와 번호 조회 실패 때 화면에 표시하지 않는다. scope·바인딩 객체·request generation이 바뀐 응답은 무시하고 cache를 덮어쓰지 않는다. C 내부 ID·이전 상태·코드/인증값은 Local Save에 추가하지 않았다.
- `IOnlineAccountTransport.cs`, `OnlineAccountResponse.cs`, `UgsOnlineRecordTransport.cs`: 기존 UGS transport의 동일 15초 timeout/늦은 결과 관찰 경로를 재사용한다. 번호·상태·시작·재발급은 빈 입력 Dictionary, 취소는 String `transferId` 하나, 완료는 String `code`/`verificationValue` 두 개로 SDK를 호출한다. 서버 입력 정의와 정적으로 대조했다. request JSON을 account endpoint의 실제 개별 입력 대신 보내지 않는다. 인증 context 외에 Player/C/공개 번호/소유 행 ID를 요청 입력으로 사용하지 않는다.
- A 시작과 B 완료는 Step 6-1 Pending gate를 먼저 통과하며 Pending 존재 시 인증/transport 0회다. 완료 입력의 Crockford 별칭·대소문자·하이픈을 정규화하고 9자리 인증값의 선행 0을 유지한다. 재발급은 Pending 상태에서만, 취소는 현재 status 응답에서 받은 canonical UUID v4 상관 ID가 있을 때만 요청한다. 시작 응답의 원문은 해당 호출자에게만 반환하고 ViewState/Local Save에는 저장하지 않는다. 화면 수명/원문 폐기는 Step 6-4에 연결한다.
- `OnlineAccountViewState.cs`, `PublicPlayerNumber.cs`: Ready·Loading·Offline·Error/Retry·TransferPending·Inactive·Completed 모델과 정확한 ASCII 10자리 검증/표시를 구현했다. `0000000000`·숫자 변환·내부 ID/마스킹 폴백은 허용하지 않는다. 모든 상태에서 Offline Run 허용을 유지하며 게임 시작 조건에 번호 상태를 추가하지 않았다. 명시적 `RetryPublicNumberAsync`, `RefreshStatusAsync`, `Invalidate`를 준비했다.
- `GameSystem.cs`: 실제 생산 인증·제출·조회에서 새 account coordinator를 사용하도록 연결했다. 기존 `OnlineLeaderboardEntry`는 `publicPlayerNumber`/`isMe` 계약으로 전환했다. `CloudCodeRecordRepository`는 행 번호가 잘못되면 안전한 Error로 처리하고, `UIManagementSystem`은 전체 공개 번호 및 서버 `isMe`로 `(You)`를 표시한다. 기존 ID 마스킹 경로를 제거했으며 Scene/직렬화 Component 필드는 수정하지 않았다. 새 계정 패널/Retry 버튼/이전 화면 Controller 연결은 Step 6-4 및 사용자 Step 8에 남긴다.
- Timeout/서비스 실패는 저장된 바인딩·Pending·개인 최고를 지우거나 새 계정으로 전환하지 않는다. Completed/Inactive 확인 뒤에는 기존 캐시로 온라인 요청을 재개하지 않도록 handoff 잠금을 유지한다. B의 번호 응답은 이 Step에서 성공 확인 자료로만 반환하고 기존 개인 최고/저장 캐시는 교체하지 않았다. Step 6-3에서 서버 개인 최고 교체/비움 또는 A의 새 Anonymous 전환을 저장/확인한 뒤 runtime 기록·표시 세션을 초기화하도록 확장한다. 번호 Retry만으로 이 잠금을 해제하지 않는다. restart recovery의 최종 적용 역시 Step 6-3 범위다.
- 신규 `OnlineAccountCoordinatorTests.cs` **23개 Edit Mode 사례**를 작성했다. 번호 범위/선행 0·Unicode 거부, 인증 직후 발급/저장, 무동의/Offline/계정 불일치 0회, 안전한 Error/명시 Retry, 재시작 캐시 비표시, generation/바인딩 변경 stale 응답, A/B Pending gate, 원문 미저장, 정확한 상태/취소/완료 입력, timeout 보존/조회 복구, handoff 잠금, 환경 캐시 보존/저장 실패, 중복 요청과 Leaderboard 본인 표시 계약을 포함한다. 실제 C# 실행은 미실행이다.
- 신규 `UGS/Tests/client-account-contracts.test.cjs`에서 C#·서버 endpoint 입력·DTO·scoped cache·gate/epoch·안전한 표시·meta/스타일/구분자 균형을 정적으로 확인했다. 전체 Node Test 파일 **25/25**, 기존 Client Local Save 정적 계약과 서버 계약 검사 및 `git diff --check` 통과. 신규 package/asmdef 변경은 없으며 C# 컴파일을 수행하지 않았다.
- 현재 수동 적용은 없다. 사용자의 Local Save·Scene/Prefab·원격 서비스를 변경하지 않았고 Unity Editor/Build/Test Runner를 실행하지 않았다. Step 7에서는 신규 `OnlineAccountCoordinatorTests` 23개, 앞선 `OnlineLocalSaveScopeTests` 25개와 영향받는 `OnlineRecordRepositoryTests`, `LocalSaveJsonCodecTests`, `LeaderboardViewStateTests` 및 저장·계정/UI 격리 Play Mode Test를 실행한다. 정상 경로의 신규 Warning/Error는 없다. 서버/Unity 패키지 동작·실제 입력 직렬화·UI 표시 판정은 Step 7/10/11에서 확인한다.

### Step 6-3. 이전 완료 뒤 A/B 상태와 개인 최고를 반영한다

서버 완료 확인 뒤 A 인증 세션·C 캐시 초기화 및 새 Anonymous 시작, B의 C 재조회·개인 최고 전체 교체/비움을 구현한다. 확인 전 Timeout의 상태 보존과 재시작 복구도 검사한다.

- [x] A/B 완료 처리·개인 최고·새 시작·Timeout 복구의 코드와 Test를 준비했다.

### Step 6-3 수행 결과 — 2026-10-03

완료 범위는 생산 Client/서버 조회 코드·Edit Mode Test 작성·정적 대조와 로컬 Node 실행이다. C# 컴파일·Test Runner·실제 Authentication/원격 호출 결과는 미실행이다. Step 6 상위 항목은 6-4~6-5가 남아 있어 미완료다.

- `get-account-personal-bests.js`를 추가했다. 입력 없는 bundled endpoint이며 인증 context의 활성 C·revision·번호/소유 매핑을 대조하고 C ledger의 Stage/Infinite 개인 최고 전체를 한 번에 반환한다. 본문은 `status`, `publicPlayerNumber`, `personalBests[{boardId,score,submissionId}]`뿐이다. 누락/손상 ledger·진행 중 제출·연결/ledger token 변화·서비스 오류는 실패이며 빈 목록으로 대체하지 않는다. 명시적 성공의 빈 배열만 기록 없음으로 인정한다. 조회는 ledger/Leaderboard/기존 B 서버 자료를 쓰거나 삭제하지 않는다. 128/100 제한 해소와 receipt 정리는 구현하지 않았다.
- `get-account-transfer-status.js`는 binding이 없는 최초 인증/번호 발급 전 중단 사례만 기존 provisioning을 사용한 뒤 상태를 반환한다. 기존 Inactive 또는 이전 예약 binding을 새 계정으로 덮어쓰지 않는다. 인증 직후 Client는 저장 번호를 표시하기 전에 이 상태 경계를 확인한다. Completed/Inactive와 이후 바뀐 terminal 상태에서도 현재 C 번호가 저장 번호와 달라지면 개인 최고 교체 경계를 통과해야 한다.
- B 완료/AlreadyCompleted 확인 뒤 현재 인증 Player로 번호와 전체 최고 스냅샷을 재조회한다. 새 번호와 C의 최고 목록을 현재 환경 Local Save에 원자적으로 함께 저장한 뒤 메모리를 전체 교체한다. 더 좋았던 B 로컬 최고도 유지/병합하지 않으며 C에 없는 Board는 비운다. `OnlinePersonalBestSnapshot.cs`/`OnlinePersonalBestEntry.cs`는 이 조회 전용 DTO다. 기존 `RecordSubmissionCandidate`를 로컬 순위값 캐시로 재사용하되 서버에 없는 Run 구성 요소를 복원했다고 판정하지 않고 Pending에 넣거나 재제출하지 않는다. 이후 확정된 신규 Run만 Pending 후보가 된다.
- `LocalRecordRepository.cs`의 account transition 예약은 모든 소유자의 현재 환경 Pending 0건·configured scope·저장 준비를 요구한다. async 교체 중 새 Pending/개인 최고 갱신/폐기를 차단하고 finally로 해제한다. 실패한 쓰기는 기존 파일·메모리·번호·Pending을 보존한다. Settings·Input Binding·튜토리얼·기기 UUID와 다른 환경 영역은 그대로 보존한다. 이전 시작 뒤 Offline Run 등으로 새 Pending이 생긴 완료 복구도 자동 삭제/재귀속하지 않고 `PendingMustBeCleared`로 남긴다. 해결은 기존 명시적 Pending 폐기 또는 해당 연결에서 가능한 확정 처리 경계이며 UI는 6-4에서 연결한다.
- A의 인증된 Inactive/Completed 확인 뒤 빈 온라인 연결·번호/최고 제거를 먼저 원자 저장하고, `IOnlineAuthenticationSession.TryClearSessionAsync(expectedPlayerId)`로 SDK의 `SignOut(true)` 및 `ClearSessionToken()`을 호출한 뒤 다른 Anonymous Player로 인증/번호 발급한다. 설치된 Authentication 패키지의 API/세션 제거 구현을 정적으로 확인했다. 저장 실패 전에는 토큰을 제거하지 않는다. 저장 이후 토큰 제거/새 인증 실패는 안전한 Error와 상태 Retry로 남긴다. 빈 연결 저장 뒤 재시작 시 옛 SDK 세션이면 다시 인증된 Inactive를 확인해 제거하고, 이미 제거됐다면 새 Player를 귀속할 수 있다. 서버 완료가 불명확한 Timeout만으로 이 초기화를 실행하지 않는다.
- handoff 이후에는 기존 coordinator를 재사용하면서 terminal 잠금을 성공 확인 뒤 해제하고, `GameSystem.ClearAccountPresentation`이 Leaderboard 요청 version·번호/Result 표시·`OnlineRecordCoordinator`의 runtime terminal receipt/건수를 초기화한다. 오래된 조회·제출 응답은 계정/번호 경계를 다시 확인하며 새 연결에 적용하지 않는다. 이 방식은 6-2에서 준비한 handoff를 실제 저장/인증 경계에 연결하며 임의 새 저장소/프로필로 우회하지 않는다.
- 새 번호와 최고를 함께 저장한 것이 완료 적용 경계다. 서버가 현재 번호를 재확인했고 저장 번호와 같으면 과거 Completed 상태를 재조회하더라도 기존 Offline 최고/Pending을 다시 비우지 않는다. 캐시는 서버 권한·번호 조회 실패 폴백으로 쓰지 않는다. raw 이전 자격 증명·내부 C·이전 상태/ID·token은 Local Save에 추가하지 않았다. 앱 재시작 후 원문 없는 미확정 B 예약은 성공/새 연결로 인정하지 않는다.
- `AccountTransferCompletionTests.cs` **29개 Edit Mode 사례를 작성했으며 실행하지 않았다.** 전체 교체/비움·Infinite/누락 Stage·저장 실패 재시도·완료 응답 Timeout/재시작·A 토큰 제거 순서와 신규 인증 실패·옛/새 SDK 세션 재시작·모든 owner Pending·stale generation/binding·교체 중 새 기록 차단·손상 스냅샷·반복 Completed의 새 Offline 기록 보존·표시/terminal receipt 초기화·지연 기록 응답을 준비했다. 기존 `OnlineAccountCoordinatorTests` 23개도 상태 조회 선행과 완료 적용 계약에 맞게 갱신했다.
- `account-personal-bests.test.cjs` **14/14 통과**: 전체/빈 스냅샷, 환경/인증/상태/진행 중 제출 거부, 손상/누락 실패, ledger/연결 경합, B는 C만 조회하고 A는 거부, 실제 bundled endpoint 입력·최초 status provisioning·Inactive 덮어쓰기 금지를 SDK 대역으로 실행했다. Client 저장/계정/완료 정적 계약·기존 서버 정적 계약과 **전체 27개 Node Test 파일**이 통과했다. 기존 gate의 문자열 정적 검사를 강화된 account transition 조건에 맞게 갱신하고 신규 Node 대역의 없는 Item 검사를 store read 경계로 바로잡은 뒤 전체 재검증했다. 이 결과는 C# 실행/UGS 원격 보장의 대체 근거가 아니다.
- **현재 사용자 수동 적용은 없다.** Scene/Prefab·사용자 Local Save·원격 서비스를 수정하거나 Unity Editor/Build/Test Runner를 실행하지 않았다. Step 7에서 신규 29개와 기존 48개 사례 및 영향받는 `OnlineRecordRepositoryTests`, `LocalSaveJsonCodecTests`, `LeaderboardViewStateTests`, GameSystem 저장/Result·격리 Play Mode Test를 사용자 Test Runner로 실행한다. 정상 경로에 신규 Warning/Error는 추가하지 않았다. 실행 수·실패 메시지/Stack Trace·컴파일 결과를 기록한다.
- Step 9 적용표에 **신규 `get-account-personal-bests` / 입력 없음 / bundling / `account-store`, `account-service` 및 정책 의존성 / 서비스 토큰의 Private Game Data read 권한**을 추가한다. 변경된 `get-account-transfer-status`의 최초 provisioning 의존성·기존 서비스 read/write 권한도 같은 배포 묶음에서 확인한다. 기존 Player 직접 Write Deny와 Secret 이름/권한은 유지하고 현재는 게시하지 않는다. 실제 SDK 토큰 제거·신규 Player·최고/행 보존·저장/인증 실패 복구는 Step 7/10, 화면 문구·조작은 Step 11에 남긴다. 다음 작업은 Step 6-4다.

### Step 6-4. 이전 화면 Controller/View와 격리 UI Test를 준비한다

시작·복구 입력·재발급·취소·결과 화면의 코드, 단일 이벤트 등록·화면 닫힘·원문 수명·중복 클릭 방지를 구현한다. Scene 없이 생성 가능한 UI 구성으로 Edit/Play Mode Test를 준비하고 Step 8 적용표에 사용할 실제 필드를 확정한다.

- [x] UI 코드·격리 Test·Scene 적용표에 필요한 Component/필드를 준비했다.

### Step 6-4 수행 결과 — 2026-10-03

완료 범위는 코드·격리 C# Test 작성·정적 검증·수동 적용 지침 준비다. Unity 컴파일/Test 실행·Scene 적용·실서비스 화면 확인을 완료한 것은 아니다. Step 6 상위 항목은 6-5가 남아 미완료다.

- `AccountTransferController`, `AccountTransferScreenState`, `IAccountTransferView`는 Unity UI와 분리된 시작·입력·발급·재발급·취소·결과·Pending 폐기 확인 흐름이다. 실제 `OnlineAccountCoordinator`를 재사용하고 공개 번호는 문자열 그대로 표시한다. 동의 저장 성공 전 인증하지 않으며 모든 소유자의 현재 환경 Pending이 남아 있으면 시작/완료 요청을 막는다. Pending Retry는 기존 `OnlineRecordCoordinator.RetryAllPendingAsync`, 명시적 폐기는 저장 성공 경계를 사용하고 자동으로 이전을 시작하지 않는다.
- `AccountTransferView`와 `AccountTransferUIController`는 Settings용 MonoBehaviour다. UI 코드가 기존 GameSystem의 저장소/인증/계정 세션에만 연결되며 독립 SDK/저장소를 만들지 않는다. 초기화만으로 온라인 요청하지 않는다. Settings·입력 재바인딩 조건을 확인한 뒤 사용자 버튼으로 진입한다. 버튼별 named handler를 한 번만 등록하고 disable/재초기화 때 해제한다. Inspector persistent OnClick·중복 참조·잘못된 배열/부모 관계는 초기화 실패로 차단한다.
- 요청 중 중복 실행/재진입을 막고 공개 번호를 숨긴다. 닫기/disable은 입력·발급 원문·화면 모델을 비우며 세대 검사로 늦은 응답이 다시 표시되지 않는다. 서버 변경/완료 handoff 자체는 화면 닫기로 취소하지 않는다. coordinator가 source transport DTO 원문을 finally에서 비우고 controller가 반환 DTO도 비운다. 원문은 Local Save/로그에 넣지 않는다. 요청 중 전송용 immutable string의 수명이나 GC 메모리 완전 삭제를 보장했다고 주장하지 않는다.
- Settings 배경 CanvasGroup의 interactable/raycast만 차단·복원하고 모달 내부 Navigation은 Explicit 상하 이동을 양 끝에서 고정한다. 처리 중 포커스는 닫기 버튼으로 이동한다. GameSystem Cancel은 열린 이전 패널만 닫고 배경 Settings 복귀/게임 재개로 전달하지 않는다.
- 신규 `AccountTransferControllerTests` **Edit Mode 17개**, `AccountTransferUIIsolationTests` **Play Mode 7개**를 작성했다. 동의/Offline 무요청·Pending gate·폐기 실패/확인·완료/취소/Retry·stale 원문·중복 요청·선행 0·OnDisable·단일 listener·포커스를 저장소/인증/transport 대역과 런타임 생성 UI로 검사한다. Play Test는 Scene 로드/원격 요청 없이 Assembly-CSharp View를 reflection으로 연결한다. 실제 폰트/레이아웃은 검사하지 않는다. 모두 **미실행**이며 신규 누계는 Edit 94개/Play 7개다.
- 신규 `UGS/Tests/client-account-transfer-ui-contracts.test.cjs`, 기존 Client/서버 정적 계약과 **전체 28개 Node Test 파일**, `git diff --check`가 통과했다. 신규 source 계약의 GameSystem 저장소 필드명을 실제 `_localRecordRepository`에 맞게 바로잡은 뒤 재검증했다. 이 결과는 C# 컴파일/Test 실행이나 UGS 실제 동작의 대체 근거가 아니다.

#### 사용자 수동 작업 및 Step 8 적용표 초안

**Step 6-4 수행 당시에는 즉시 수동 적용이 필요하지 않았다.** 이후 UI Test를 사용자 Scene 기반으로 전환했으므로 현재 남은 사용자 적용은 아래 Step 8-1~8-8 상세 지침을 따른다. 신규 Edit 17개/Play 7개와 앞선 Edit 77개는 이 Step의 작성 사례 수이며 최신 실행 결과와 구분한다. AI는 Unity 빌드/Editor/Test Runner를 실행하지 않았다.

Step 6-4 수행 당시 Scene/Prefab은 읽거나 변경하지 않았다. 아래는 당시의 **제안명/초안**이다. 이후 Step 8에서 기존 `UIManagementSystem._settingsPanel`과 저장된 Hierarchy를 읽기 전용으로 확인했으며, 기존 SettingsWindow에 CanvasGroup을 추가하고 그 형제로 Host를 배치하는 상세 지침으로 구체화했다. 실제 적용은 Step 8 상세 지침이 우선이며 사용자만 수행한다.

1. 기존 Settings 패널 안에서 배경의 기존 조작들을 `SettingsContent` CanvasGroup으로 묶고 그 안에 `OpenAccountTransferButton`을 추가한다. 기존 Settings 포커스/Navigation 순서에 이 진입 버튼을 연결한다. SettingsContent 밖의 **형제** `AccountTransferHost`에 `AccountTransferView`와 `AccountTransferUIController`를 추가한다. Host를 Root/SettingsContent 안에 넣지 않는다. Host는 Settings가 활성일 때 활성이고 자식 `AccountTransferRoot`만 초기 비활성으로 둔다.
2. Root에 Overview/Issued/Input/Result/DiscardConfirmation의 5개 Page를 추가한다. AccountText/StatusText는 Root 공통, CredentialText는 Issued Page, CodeInput/VerificationInput은 Input Page에 둔다. 12개 동작 버튼은 Root의 공통 Actions에 두며 **어느 Page 안에도 넣지 않는다**. Root를 배경보다 앞에 두고 전체 영역 raycast 가능한 배경 Image를 배치한다. 기존 Canvas·GraphicRaycaster·EventSystem을 재사용하며 중복 EventSystem은 만들지 않는다.
3. 모든 버튼의 Inspector OnClick persistent 항목은 **0개**로 둔다. 코드가 listener를 등록한다. TMP Font는 기존 프로젝트의 한국어 지원 자산을 사용하고 입력의 TMP Text/Viewport/placeholder도 정상 연결한다. 코드가 입력 길이 9와 인증값 Password/Digit, richText=false를 설정한다. 9자리 인증값은 숫자로 변환하지 않으며 `000000007` 같은 선행 0을 유지한다.

| Component 실제 필드 | 사용자 Inspector 연결 |
|---|---|
| `AccountTransferUIController._gameSystem` / `_view` | 기존 GameSystem / 같은 Host의 AccountTransferView |
| `AccountTransferView._root` / `_settingsContent` | 초기 비활성 AccountTransferRoot / 배경 전용 SettingsContent CanvasGroup |
| `_pages` | Size 5: 0 Overview, 1 Issued, 2 Input, 3 Result, 4 DiscardConfirmation |
| `_accountText` / `_statusText` / `_credentialText` | AccountText / StatusText / Issued Page의 CredentialText |
| `_codeInput` / `_verificationInput` | Input Page의 TMP_InputField 두 개 |
| `_openButton` | SettingsContent의 OpenAccountTransferButton |
| `_actionButtons` | Size 12: 아래 enum 순서, 모두 Root 공통 Actions 아래 |

| 배열 index | 실제 action / 제안 버튼 문구 |
|---|---|
| 0 | Start / 이전 시작 |
| 1 | OpenInput / 이전 코드 입력 |
| 2 | Complete / 이전 완료 요청 |
| 3 | Reissue / 코드 재발급 |
| 4 | CancelTransfer / 이전 취소 |
| 5 | Refresh / 상태 다시 확인 |
| 6 | ConfirmConsent / 복구 안내 확인·온라인 사용 동의 |
| 7 | RetryPending / 전송 대기 기록 다시 전송 |
| 8 | RequestDiscard / 전송 대기 기록 폐기 |
| 9 | ConfirmDiscard / 폐기 확인 |
| 10 | CancelDiscard / 폐기하지 않음 |
| 11 | Back / 닫기 |

Step 8은 기존 부모 경로 확인/사용자 연결 전까지 미완료다. 실제 조작·문구·한글 표시·레이아웃·SDK/실서비스 결과는 Step 7/10/11에서 확인한다. 다음은 Step 6-5다.

### Step 6-5. verification 검증 도구와 격리 세션을 준비한다

Step 10의 실서비스 실행 도구를 준비하고 기기 A/B의 테스트 세션·인증 프로필·Local Save 경로·테스트 Board·결과 비교 항목을 지정한다. 자동 Test에서는 원격 호출을 끄고, 명시적인 사용자 실행에서만 verification에 요청한다. 기존 도구 확장 여부·실제 메뉴·버튼·기대 결과를 문서화한다.

- [x] 도구·세션 분리·원격 호출 차단 Test와 실행 지침을 준비했다.

### Step 6-5 수행 결과 — 2026-10-03

완료 범위는 도구·격리 세션·실행 지침·C# Test 작성과 정적 검증이다. 원격 실행/Unity 컴파일/실제 Test Runner/Build/Scene 작업은 수행하지 않았다. Step 6-1~6-5의 코드 준비를 마쳤으며 다음은 사용자 Step 7이다. Phase 2 전체 완료를 의미하지 않는다.

- 기존 `Assets/Editor/OnlineRecordVerificationWindow.cs`의 **`Flow State > Online Record Verification`** 메뉴를 확장했다. 기본 `Prototype 8 Phase 2 (Scene 없음)` 모드는 Play Mode/GameSystem/Scene 없이 격리 저장소와 실제 계정·기록 coordinator 및 이전 Controller를 사용한다. 구 Prototype 7 모드는 남기되 자동 Test를 차단하고 수동 ledger 초기화 안내를 제거했다. 새 창의 원격 허용은 기본 false이며 창 닫힘 때 해제·원문 제거한다. 준비 버튼은 격리 로컬 UUID/저장만 준비하고 SDK 인증/온라인 요청을 하지 않는다.
- `VerificationSessionConfiguration`은 A/B의 프로필과 저장 디렉터리, Legacy의 기존 인증 프로필과 별도 저장 경로를 고정한다. `UgsOnlineAuthenticationGateway`는 같은 프로세스에서 다른 프로필·외부 초기화·실제 SDK Profile 불일치를 거부한다. 정상 기본 프로필/기존 Local Save 위치는 그대로다. `PersistentLocalSaveFileStore`의 directory 생성자와 GameSystem의 명시적 launch flag를 추가해 도구/게임의 격리 경로 선택을 일치시켰다. 기존 Play Test 메모리 저장·UGS 생성 전 return 경계는 유지했다.
- `VerificationRemoteGuard`가 동의/자동 실행 여부를 인증·token 제거·기록/계정 transport 직전에 검사하고 인증 뒤에도 재확인한다. 창은 Test Framework의 설치된 `IsRunActive` API를 read-only reflection으로 확인한다. API 없음/확인 실패·`-runTests`·Batch·Play 진입은 fail closed한다. 자동 Test는 대역만 사용하며 실 SDK 준비/호출을 차단한다. 허용 해제는 이미 전송한 서버 변경을 취소하지 않는다.
- `VerificationRecordBaseline`/`VerificationRecordComparison`은 Project/Environment/Board/확정 공개 번호와 성공한 본인 행의 점수·수락 시각을 자동 비교한다. 빈 계정도 같은 번호를 요구한다. 다른 이용자 때문에 변할 수 있는 rank는 보존 비교에서 제외한다. 오류/누락은 빈 성공으로 처리하지 않는다. 기준 자료는 사용자 로컬 전용이며 원문 코드·인증값·token·Player ID는 포함하지 않는다. 내부 고정 owner·binding·원자성의 증명으로 오인하지 않는다.
- `UGS/Verification/compare-service-snapshots.cjs`는 사용자 Dashboard 읽기 자료를 오프라인 비교한다. 기존 행 owner/점수/submissionId/수락 시각 보존, 같은 C·번호·고정 owner와 revision +1·A Inactive/B Active를 검사한다. `verification-service-comparison.test.cjs` **19개 로컬 사례 통과**. 실제 자료를 읽거나 원격 요청을 실행하지 않았다. snapshot의 신뢰성/서비스 원자성까지 입증하지 않는다.
- `VerificationSessionTests` **Edit Mode 27개**와 `VerificationToolIsolationTests` **Play Mode 1개**를 작성했다. 기본 비허용 경계의 인증/token/transport 0회, 인증 중 허용 철회, A/B/Legacy 분리, launch flag의 잘못된 값/중복 거부, 같은 번호/빈 성공·metadata 보존/오류/환경 차단, 자동 Test의 창 준비 차단을 검사한다. **C# 미실행**, 신규 누계 Edit **121개**/Play **8개**다.
- 신규 도구 정적 계약·기존 Client/서버 정적 계약, **전체 30개 Node Test 파일** 및 `git diff --check`가 통과했다. 정적 검토에서 EditorApplication의 Play 진입 API·제출 결과의 실제 `Result` 속성·fresh 저장의 기기 UUID 초기화·중복 삽입된 launch parser를 바로잡았다. 신규 괄호 검사에서 URL의 `//`를 주석으로 오인하는 scanner와 EditorApplication substring 오탐을 수정해 재검증했다. C# 실행 성공으로 기록하지 않는다.

#### 지금 할 수동 작업 / 실행 전제

현재 원격 실행/Scene 적용을 할 필요는 없다. 다음 **Step 7**에서 사용자가 Unity 컴파일·신규 27 Edit/1 Play 및 앞선 94 Edit/7 Play, 영향받는 `OnlineRecordConfigurationTests`, `OnlineAccountCoordinatorTests`, 저장/Codec·GameSystem 격리·Navigation/UI Test를 실행한다. 정상 격리 fixture의 신규 Warning/Error는 기대하지 않는다. 거부 시나리오의 실제 gateway/기록 Warning은 실서비스 실행 분류와 함께 기록하며 예상 밖 Warning/Error를 숨기지 않는다.

실서비스 버튼은 **Step 7 통과·Step 9 배포/Secret/권한 준비 완료 뒤 Step 10에서만** 실행한다. AI는 원격 허용을 켜거나 테스트 계정을 생성/전환/삭제하지 않았다. Production 사용·기존 ledger/Board 초기화·사용자 인증 token 복사/삭제는 하지 않는다.

#### A/B 및 기존 계정 세션 지정

| 세션 | Authentication Profile | 격리 Local Save (`persistentDataPath` 기준) | 용도 |
|---|---|---|---|
| A | `flow-state-phase2-a` | `Prototype8Verification/A/flow-state-save.json` | 신규 발급·원래 활성 기기 |
| B | `flow-state-phase2-b` | `Prototype8Verification/B/flow-state-save.json` | 별도 신규 계정·이전 수신 기기 |
| Legacy | 기존 `flow-state-verification` | `Prototype8Verification/Legacy/flow-state-save.json` | 기존 verification 자료의 최초 cutover 검증 |

- 실제 절대 경로는 창의 `격리 세션 준비 (로컬만)` 후 표시한다. 준비한 창에서는 세션을 바꿀 수 없다. 한 Editor 프로세스의 SDK는 한 프로필만 사용하므로 A↔B↔Legacy 전환 전 **Editor 종료·재시작**한다. 동시에 두 Editor를 사용하려면 별도 프로젝트 사본/동일 코드·동일 verification Project·Environment를 사용한다. 같은 저장 파일을 두 프로세스에서 동시에 열지 않는다.
- 인증 프로필은 기존 token을 그대로 사용한다. A/B가 이미 사용된 프로필이면 신규 계정 최초 발급 사례가 아니다. 자동으로 token을 지우지 말고 기존 사용자 자료와 구별된 OS 사용자/새 verification 테스트 환경을 사용자 승인하에 준비한다. 동일 프로필 이름만으로 Editor와 Player가 같은 인증 저장 매체/계정이라고 판정하지 않는다.
- Legacy는 새 Anonymous로 대체하지 않는다. 사용자가 기존 테스트 기록/계정 사본을 보존하지 않기로 한 Step 9-1 결정을 우선하며, 없는 사본의 복구/재백업을 필수로 요구하지 않는다. Step 10-2에서 아직 전환되지 않은 기록과 그 원래 인증 프로필이 실제로 남아 있는지 먼저 확인한다. 없다면 중단하고 별도 검증 자료 준비의 범위/승인을 논의한다. 원본 인증 token/Local Save/원격 ledger는 지우거나 덮어쓰지 않는다. 이미 전환된 계정으로 최초 migration을 검증하지 않으며 실제 Dashboard 기준 확보는 수동, 수치/보존 판정은 오프라인 도구가 한다.
- 실제 테스트 Board는 `fs-stage-stage-001-r1` (Stage / `stage-001` / rules 1), `fs-infinite-v2` (Infinite / rules 2)다. 임의 Board·원격 ID를 입력하지 않는다. 검증용 Stage 제출 버튼은 **60초 / 60000ms** 후보를 실제 production policy로 생성·Pending 저장 뒤 서버에 전송한다. 기존 최고보다 나쁘면 행은 바뀌지 않을 수 있다. 현재 100명/128건 한도는 해소하지 않았다.

#### Step 10 실제 메뉴·버튼·기대 결과

1. Play Mode를 끈 새 Editor에서 위 메뉴를 열고 기본 Phase 2 모드·A를 선택한다. `격리 세션 준비 (로컬만)`은 `LOCAL_READY`이며 원격 호출 0회다. `verification 원격 요청·테스트 기록 변경 허용`을 사용자가 체크한 뒤 `계정 패널 열기 / 상태 확인` → `ConfirmConsent`를 실행한다. 동의 저장 후 인증/상태/번호 요청을 시작한다. 초기 실패에서 공개 번호 캐시를 성공으로 취급하지 않는다.
2. `격리 Stage 기록 제출 (60초)`의 성공은 `PASS / SERVER_SUBMITTED`다. 필요한 Infinite 기록은 사용자 게임 Run으로 생성한다. `Infinite (해제: Stage)`로 Board를 고른 후 `내 최고 기준 저장 / 재조회 번호 비교`를 각각 실행한다. 기대는 `PASS / NUMBER_STABLE_AND_BASELINE_CAPTURED`다. `기준 자료를 사용자 파일로 저장`으로 Board별 로컬 기준을 보관한다. 기본 파일은 로컬 비교 자료일 뿐 결과 보고가 아니다. 재시작 뒤 해당 JSON을 `비교할 기준 JSON (로컬 전용)`에 넣고 `기준 자료와 번호·점수·수락 시각 비교`를 실행한다. 기대는 `PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED`다.
3. B도 별도 Editor/재시작에서 같은 절차로 동의·인증한다. A/B 사전 번호 구분은 `node UGS/Verification/compare-service-snapshots.cjs distinct "A의 실제 기준 파일 경로" "B의 실제 기준 파일 경로"`로 자동 판정한다. 같은 Board의 로컬 기준 파일을 사용하고 `PASS / AB_NUMBERS_DISTINCT`가 아니면 중단한다. 창에 표시된 번호/Player ID를 보고에 붙이지 않는다. B의 기존 서버 기록은 허용하지만 양쪽 현재 환경의 모든 Local Save Pending은 이전 시작/완료 전에 0건이어야 한다.
4. A 패널 `Start` → 발급 원문을 **사용자가 B 입력으로만 전달**한다. 원문을 보고/로그/기준 파일에 저장하지 않는다. `Reissue`는 기존 원문을 무효화하고 만료 시각은 유지한다. `CancelTransfer`는 상태 조회의 상관 ID를 사용하며 A 연결을 유지한다. 닫힘/재시작 후 원문을 복원하지 않는다. Pending이 남으면 `RetryPending` 또는 `RequestDiscard` → `ConfirmDiscard`를 사용한다. `CancelDiscard`는 폐기하지 않는다. 저장 실패 시 Pending을 유지한다.
5. B 패널 `OpenInput` → 코드/9자리 인증값 입력 → `Complete`를 실행한다. 완료 적용 뒤 A 기준 파일을 Board별로 B에 입력해 보존 비교한다. 비교 전 B 후속 새 최고를 제출하지 않는다. 이후 B의 새 Stage 제출은 `SERVER_SUBMITTED`여야 한다. 원문 재사용은 같은 B의 idempotent 완료 복구와 다른 인증 계정의 무효 코드 거부를 구분한다. 5초/90일 정밀 경계와 중복 클릭은 자동 Test로 검사하며 사람에게 빠른 입력/장시간 대기를 요구하지 않는다.
6. A는 완료 후 `계정 패널 열기`나 `Refresh` **전에** `원래 인증 세션의 Inactive·조회 실패 관찰 (복구 전에)`를 실행한다. 기대는 `OBSERVED / ORIGINAL_INACTIVE_AND_QUERY_FAILED`다. 현재 서버의 일반 오류 응답만으로 실패 원인이 권한 거부임을 증명하지 않는다. 이후 패널 재조회는 production coordinator의 Inactive 저장·token 제거·새 Anonymous 복구를 사용한다. 같은 C로 돌아오지 않아야 한다. 별도 새 인증 A에서 사용된 코드 완료 요청은 유효한 새 연결로 허용되면 안 된다.
7. 활성 계정의 `Player 직접 Write 403 검증`은 실제 Player token으로 Leaderboard와 Cloud Save Default probe 두 요청을 실행한다. 기대는 `PASS / PLAYER_WRITE_DENIED` (둘 다 HTTP 403)다. 2xx면 검증 실패·해당 테스트 계정의 probe 오염만 사용자 Dashboard에서 확인/복구한다. 전체 데이터 초기화는 하지 않는다. Protected/Private 권한의 모든 API와 서비스 CAS 강제력은 이 두 요청만으로 입증하지 않는다.
8. `안전한 결과 복사`의 PASS/FAIL/OBSERVED와 실행 시각·대상 환경·사용자가 Step 9에서 확인한 Script version만 보고한다. 공개 번호·Player ID·내부 C·원문·token·전체 오류/기준 JSON을 보고에 붙이지 않는다. `원문 제거 / 계정 패널 닫기` 또는 창 닫기는 원문을 제거하고 원격 허용을 끈다. 허용을 끄면 추가 호출을 차단하지만 이미 시작한 서버 commit은 취소하지 않는다.

#### 내부 고정 행·기존 전환의 서비스 자료 자동 비교

공개 API가 숨기는 owner/C/binding의 실제 보존은 사용자 Dashboard **읽기**로 확보한 자료와 오프라인 도구를 함께 사용한다. 수치를 눈으로 판정하지 않는다. 실제 읽기와 실행은 Step 9/10의 사용자 작업이며 지금 자료 확보/서비스 실행을 하지 않았다.

- 로컬 JSON root는 `projectId`, `environmentId`, `rows`를 갖는다. `rows`는 위 두 Board ID를 key로 갖고, 각 값은 해당 고정 owner의 `{playerId, score, metadata:{acceptedAt, submissionId}}` 또는 실제 **행 없음이 확인된 `null`**이다. 누락/읽기 오류를 null로 기록하지 않는다.
- `legacy-before.json`은 추가로 기존 테스트 계정의 `legacyPlayerId`를 기록한다. `legacy-after.json`은 Private Custom Data `fs8-account-<C>` / Item `fs_account_v1`의 value에서 `projectId`, `environmentId`, `accountId`, `publicPlayerNumber`, `leaderboardOwnerId`, `currentPlayerId`, `connectionRevision`, `status`만 골라 `account`에 둔다. cutover 전후 row owner·점수·submissionId·수락 시각이 같고 최초 owner/currentPlayer가 기존 계정인지 자동 비교한다. 구자료의 metadata가 없으면 통과시킬 수 없으며 초기화로 해결하지 않는다.
- `transfer-before.json`의 `account`/`rows`는 A 시작 전 **Active** 기준이다. `transfer-after.json`은 B 완료 적용 뒤 새 제출 전 같은 C의 `account`/`rows`와 `sourceBinding`, `targetBinding`을 기록한다. binding은 Private `fs8-player-<A/B>` / Item `fs_account_v1`의 value에서 `projectId`, `environmentId`, `playerId`, `accountId`, `connectionRevision`, `status`만 고른다. 자격 증명·digest/HMAC·receipt 전체·Secret을 복사하지 않는다.
- 사용자 로컬 파일의 내부 ID는 비교에만 사용한다. 도구는 파일에 입력된 임의 Player ID로 인증/요청하지 않으며 원격 권한을 부여하지 않는다. Dashboard read 자료의 출처·시각·Script version을 사용자가 확인한다. 이 자료를 채팅/저장소에 넣지 않는다.

```powershell
node UGS/Verification/compare-service-snapshots.cjs legacy "C:\UserVerification\legacy-before.json" "C:\UserVerification\legacy-after.json"
node UGS/Verification/compare-service-snapshots.cjs transfer "C:\UserVerification\transfer-before.json" "C:\UserVerification\transfer-after.json"
```

경로는 **사용자가 실제 확보한 로컬 파일 경로**로 바꾼다. 도구는 읽기 전용·원격 호출 없음이며 성공은 exit 0과 `LEGACY_FIXED_ROW_PRESERVED` / `TRANSFER_FIXED_ROW_BINDING_PRESERVED`, 실패/손상은 exit 1이다. private 값을 출력하지 않는다. 전후 owner/번호/기록 보존·C의 revision +1·A Inactive/B Active를 비교하지만 동시 최초 생성 batch 원자성·모든 중단 지점/관찰 시점 사이 상태·장애 주입 가능성·감사 로그 강제력은 증명하지 않는다. 이 필수 서비스 보장은 Step 10/12에서 별도 확인하며 미확인은 미확인으로 남긴다.

#### Step 11 게임 UI 세션 연결 (Scene 변경 없음)

사용자가 이후 Editor에서 만든 검증 Player는 `FlowState.exe --fs-verification-session=A` / `FlowState.exe --fs-verification-session=B`처럼 실행할 수 있다. Editor 프로젝트 실행도 같은 flag를 사용하면 GameSystem이 같은 세션 설정을 선택한다. flag 없음은 기존 기본 프로필/저장 경로이며, 잘못된 값/중복 flag는 fail closed한다. 기존 Scene/Inspector 변경은 필요하지 않으며 이전 패널의 사용자 연결 자체는 Step 8을 따른다.

Editor와 Player의 실제 인증 저장 매체가 다르면 같은 flag/Profile 이름이어도 다른 Anonymous가 될 수 있다. token을 복사해 맞추지 않는다. 해당 Player A/B를 별도 검증 쌍으로 취급해 공개 번호·서비스 기준 자료와 Step 10의 이전 검증을 다시 확보한다. 같은 저장 매체에서 같은 인증으로 확인된 쌍만 재사용한다. Build·Player 실행·화면 확인은 사용자 Step 11/후속 Phase이며 AI는 시도하지 않았다.

### AI 수행

1. 인증 직후 번호 발급/조회, 오류·Retry·Offline 허용, C 기준 본인 행 표시와 연결 상태 모델을 구현한다.
2. 온라인 자료를 `(Project ID, Environment ID)`로 구분하는 Local Save migration, Pending 귀속 유지, Pending 폐기 저장 성공 뒤에만 이전 허용을 구현한다.
3. 이전 시작/입력/재발급/취소/결과 UI의 Controller/View를 구현한다. 코드·인증값은 Local Save에 저장하지 않는다.
4. B 완료 확인 뒤 서버 개인 최고로 로컬 최고를 완전히 덮어쓰고 기록이 없으면 비운다. A 완료 확인 뒤 C 캐시·로컬 최고와 이전 인증 세션을 지우고 새 Anonymous로 시작한다. Settings·튜토리얼·입력 설정은 보존한다.
5. 서버 확정 여부가 불명확한 Timeout에서는 임의 연결 전환·기록 초기화를 하지 않고 서버 상태 재조회로 복구한다.

Unit Test: Codec migration·다른 환경 자료 차단, Pending 있는 A/B의 요청 0회, 폐기 저장 실패, B 기존 온라인 기록 허용, 개인 최고 교체/비움, A 새 인증·캐시 제거, 이전 원문 미저장, stale 응답 무시, Offline Run 허용. 실제 Controller·저장소·인증 gateway를 대역으로 검사하는 Edit Mode Test를 작성한다.

### 사용자 수동 작업

없음. Scene 수정 없이 코드·Test를 먼저 준비한다.

### 완료 조건

- [x] Client·직렬화·UI 코드와 Edit Mode Test를 작성하고 정적으로 대조했다.
- [ ] 실행 결과는 Step 7에서 사용자 Test Runner로 확인한다.

## Step 7. Unity 컴파일과 Edit/Play Mode Test를 실행한다

2026-10-06 사용자 회귀 확인: Unity 컴파일 성공·EditMode 904/904·PlayMode 257/257 및 각 단계의 예상 밖 Error/Warning 없음을 확인받았다. 복사 버튼·완료 안내·동시 조회 수정까지의 실행 근거다. 이후 A 번호 지연 대응 코드/새 EditMode 5사례는 별도 최신 실행 결과를 기다린다.

**최신 회귀 확인 — 2026-10-05:** Step10-3 중 변경한 Client/검증 도구/Test 준비 오류 수정 뒤 사용자가 Unity Script Compilation 성공·EditMode901/901·PlayMode255/255 및 컴파일/각Test의 예상 밖 Error/Warning 없음을 확인했다. 기존 Step7 완료는 유지하고 이 결과를 최신 회귀 근거로 사용한다. AI는 Unity 컴파일/TestRunner/빌드를 실행하지 않았다. 아래 실행 수는 이전 시점 근거다.

### AI 선행 검증

모든 변경 C#·asmdef·직렬화 경로·정책 참조를 검사하고 Node Unit Test·정적 계약 검사를 실행한다. 사용자에게 실제 추가/변경 fixture 이름, 영향받는 기존 Test, 예상 Warning/Error를 파일 기준으로 제공한다. Play Mode Test는 격리된 저장소·인증·서비스 대역을 사용한다.

- [x] AI 선행 정적 검증·Node 실행·실제 fixture/회귀 범위·사용자 실행 지침을 준비했다 (2026-10-03).

### Step 7 선행 검증 결과 — 2026-10-03

**Step 7은 2026-10-04 최신 사용자 컴파일·Edit Mode 850/850·Play Mode 242/242 통과 및 예상 밖 Error/Warning 없음으로 완료했다.** 아래는 2026-10-03 AI 선행 검증 기록이다. AI는 Unity Editor/컴파일러/Build/Test Runner/Player를 실행하지 않았고 Scene/Prefab을 수정하지 않았다. Scene 읽기 전용 대조는 이후 Step 8에서 수행했다. 원격 호출·사용자 Local Save/인증 세션 접근도 하지 않았다.

- `UGS/Tests/client-unity-preflight.cjs`를 추가·실행했다. Git의 변경/신규 목록에 있는 **C# 41개 파일**의 conflict marker·기본 괄호 대응·메타 GUID 존재/중복, Features/Edit/Play asmdef JSON/참조와 TestAssemblies, TMP 참조를 정적으로 대조했다. C# 컴파일이나 모든 타입/오버로드 유효성의 증명이 아니다.
- 기존 Client 저장/계정/완료/UI/검증 도구 정적 계약과 서버 정적 계약, **전체 30개 Node Test 파일**을 재실행해 통과했다. `git diff --check`도 통과했다. LF→CRLF 안내는 Git 경고이며 Unity Warning과 다르다. asmdef 변경은 없으며 신규 MonoBehaviour는 Systems/Assembly-CSharp, EditorWindow는 Assets/Editor/Assembly-CSharp-Editor에 두고 Test는 reflection을 사용한다. Features에 UI/TMP 의존성을 추가하지 않았다.
- 설치된 Authentication API의 `Profile`, `SignOut(bool)`, `ClearSessionToken`과 TMP의 `SetTextWithoutNotify`, Test Framework active-run API를 로컬 패키지 소스로 대조했다. package/정책·scope·endpoint params·raw 원문 미직렬화 경계는 기존 정적 계약 검사로 확인했다. 이 선행 검사 당시 Unity 실제 Import·컴파일·동작은 미확인이었다. 이후 사용자 컴파일·Edit Mode 결과는 아래에 별도로 기록한다.
- 신규 fixture는 실 SDK/파일 저장소를 직접 만들지 않으며 아래 source attribute 수는 **작성된 사례 수이지 Runner 통과 수가 아니다**. 공용 `PlayModeRecordIsolationSetup`은 namespace의 테스트 시작/종료 때 GameSystem의 `UseIsolatedRecordsForPlayModeTests`를 설정/복원한다. GameSystem은 메모리 저장소 선택 뒤 UGS 생성 전 return한다. UI Test는 현재 SampleScene의 실제 UI/Inspector 참조를 읽고 메모리/인증/transport 대역만 사용한다. 종료 때 View를 닫고 입력·선택·미완료 대역 요청을 정리하며 production binding Component의 enabled 상태를 복원한다. 다음 Scene 로드는 이전 테스트의 Scene 인스턴스를 교체한다. 검증 창 Test도 생성한 창을 제거한다.

| 탭 / 실제 fixture (Assets/Tests 아래) | 신규 작성 사례 |
|---|---:|
| EditMode / `OnlineLocalSaveScopeTests` | 25 |
| EditMode / `OnlineAccountCoordinatorTests` | 23 |
| EditMode / `AccountTransferCompletionTests` | 29 |
| EditMode / `AccountTransferControllerTests` | 17 |
| EditMode / `VerificationSessionTests` | 27 |
| PlayMode / `AccountTransferUIIsolationTests` | 7 |
| PlayMode / `VerificationToolIsolationTests` | 1 |
| 신규 합계 | Edit 121 / Play 8 |

#### 반드시 포함할 기존 회귀 범위

- EditMode: `OnlineRecordRepositoryTests`, `LocalSaveJsonCodecTests`, `LeaderboardViewStateTests`, `OnlineRecordConfigurationTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`, `GameNavigationStateTests`, `SettingsStateTests`, `ResultSystemTests`, `ResultTextFormatterTests`, `TimeRecordTests`, `ScoreRecordTests`. 공용 저장/계정/기록·navigation 변경이므로 **EditMode Run All**을 권장한다. 실제 전체 발견 수는 사용자 Runner 결과로 기록한다.
- PlayMode: `PlayModeRecordIsolationTests`, `LocalPersistenceIntegrationTests`, `SettingsPanelIntegrationTests`, `SettingsInteractiveRebindTests`, `UIInputSystemTests`, `ResultMenuIntegrationTests`, `ModeResultDisplayIntegrationTests`, `GameEntryBootIntegrationTests`, `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `PauseMenuIntegrationTests`. 공용 GameSystem/UI 변경이므로 **PlayMode Run All**을 권장한다. 기존 Scene 기반 fixture의 Scene 로드는 사용자가 실행한 Test Runner가 처리하며 AI는 실행하지 않는다. 새 이전 패널의 Scene 연결은 아직 Step 8 대상이므로 그 통합 완료를 이 결과에 포함하지 않는다.

#### 예상 로그

- 신규 정상/격리 fixture에는 새 Error/Warning이 기대되지 않는다. `OnlineLocalSaveScopeTests.CorruptPending_IsNotSilentlyDroppedToUnlockTransfer`는 손상 자료를 정상처럼 덮어쓰지 않는 경로의 **`[LocalRecordRepository] Save recovery used default values.`** Warning 1회를 기존 `LogAssert.Expect`로 검증한다. 요구된 복구 경로 자체의 로그이며 다른 Warning을 허용하지 않는다.
- 기존 `OnlineRecordRepositoryTests`의 `Timeout_RemainsPending`, `QueryTimeout_HasSpecificReason_AndNextQueryCanSucceed`, `QueryOfflineFailure_HasClientFailureReason` 등은 이미 고정된 Submission unavailable/Query timed out/Query unavailable Warning을 해당 실패 주입 사례에서만 검증한다. 일반 Warning/Error를 무시하거나 `LogAssert.ignoreFailingMessages`로 통과시키지 않는다.
- 실제 UGS authentication/configuration/duplicate UI binding Warning, 예상하지 않은 TMP 폰트/참조 Warning·NullReference·MissingReference는 정상 격리 Test의 기대 로그가 아니다. 발생하면 Test 이름·메시지·Stack Trace와 함께 실패로 전달한다. 숨기기 위한 로그 기대/설정 변경은 하지 않는다.

### 사용자 실행 방법 (이 Step에 필요한 수동 작업)

아래 컴파일/Edit Mode 절차는 Scene 연결 전에도 수행할 수 있다. UI Play Mode 7개는 Scene 기반으로 코드 전환했으므로 Step 8 사용자 적용을 먼저 마친 뒤 실행한다. 최신 코드 전환 결과와 사용자 연결 조건은 아래 별도 기록을 따른다.

1. Unity **6000.3.5f2**로 현재 프로젝트를 연다 (`ProjectSettings/ProjectVersion.txt` 기준). 별도 Build·Player 실행·Scene/Inspector 수정·계정/파일 초기화는 필요 없다. verification 도구의 원격 실행 버튼은 누르지 않는다. 소스 Import/컴파일이 끝난 뒤 Console의 Compile Error와 예상 밖 Warning/Error를 확인한다. 컴파일 오류가 있으면 아래 Test를 진행하지 않고 메시지/파일·행 번호를 전달한다.
2. `Window > General > Test Runner` → **EditMode**에서 필터를 지우고 **Run All**을 실행한다. 위 신규 5개 fixture가 발견되었는지 확인한다. 발견되지 않으면 성공으로 판정하지 않는다. source 작성 수 121은 신규 부분집합이며 전체 실행 수와 혼동하지 않는다.
3. **PlayMode**에서 위 신규 2개와 기존 격리 fixture/회귀 범위를 검증한다. 자동 Test에 필요한 동의·실계정 발급·Secret·게시·원격 호출·정밀 클릭은 없다. 단, `AccountTransferUIIsolationTests` 7개는 사용자 Scene 작업 방식에 맞게 코드를 전환했다. Step 8의 사용자 Scene/Inspector 연결과 최신 수정본 컴파일 후 재검증하며, 이 7개를 제외한 실행으로 전체 통과를 판정하지 않는다.
4. 두 탭의 실제 발견/실행·Passed/Failed/Skipped 수와 컴파일/예상 밖 Console 결과를 전달한다. 가능하면 Runner의 결과 XML을 사용자 로컬에 보관한다. 실패는 Test 이름·실패 메시지·Stack Trace를 전달하되 실제 token/원문/Player ID는 제외한다. Skip/미발견을 통과로 계산하지 않는다.
5. 코드 수정이 필요하면 AI 정적 검사 후 수정본을 Import하고 영향받는 Test 전체를 다시 실행한다. 최신 수정 뒤 결과가 아닌 이전 통과로 완료 처리하지 않는다.

전달 형식: `Unity 버전 / 컴파일 오류·예상 밖 Warning / Edit 발견·실행·Passed·Failed·Skipped / Play 발견·실행·Passed·Failed·Skipped / 실패 이름·메시지·Stack Trace`.

사용자 검증 결과(2026-10-03): Unity Script Compilation 성공, 컴파일 관련 예상 밖 Error/Warning 없음, Edit Mode 850개 실행·850개 통과, Edit Mode 관련 예상 밖 Error/Warning 없음을 확인했다. 이전 `InvalidNumberOrUnknownReason_HidesCacheAndSupportsExplicitRetry` 실패 수정 뒤 전달받은 최신 Edit Mode 결과로 기록한다. 신규 작성 사례 121개와 전체 실행 수 850개는 구분한다. 당시 PlayMode `AccountTransferUIIsolationTests` 7개 전체 실패(0.161s)는 통과 결과를 받지 못했고 다른 Play Mode 결과도 미확인이어서 완료 조건을 체크하지 않았다. 이후 Scene 기반 전환/사용자 연결/영어화 뒤 2026-10-04 최신 결과로 완료했다. 기존 방식의 상세 실패 원인이 소급 확정된 것은 아니다.

Play 실패 및 검증 방식 변경(2026-10-03): 이전 7개 fixture는 SetUp에서 UI 객체와 필드 참조를 코드로 생성했으며 Scene 미연결이 기존 실패 원인이었다고 단정하지 않는다. 실패 메시지/Stack Trace 미수신으로 원인은 미확정이다. 사용자는 코드로 UI·참조를 생성하는 방식을 원하지 않으며 모든 Scene 관련 수정은 직접 수행한다. UI 검증은 실제 Scene/Inspector 참조를 사용하는 방식으로 전환하고 Scene 적용·연결과 통합 재검증은 Step 8에서 진행한다. 이후 사용자 요청에 따라 Test 코드를 Scene 기반으로 전환했다. 상세 변경과 최신 검증 상태는 아래 기록을 따른다. 기존 실패 7개를 통과·Skip으로 처리하지 않으며 Controller의 독립 Edit Mode 검증은 유지한다. AI는 Scene 수정·Unity Test Runner 실행을 하지 않는다.

실패 수정 기록(2026-10-03): `OnlineAccountCoordinatorTests.Transport`의 개인 최고 스냅샷 번호가 이전 완료용 `9999999999`로 고정되어 정상 Retry 번호 `0000000007`과 달랐다. 저장 캐시와 서버 번호가 다르면 production coordinator는 번호/전체 최고 교체를 요구하며 서로 다른 스냅샷을 안전하게 거부한다. 대역 스냅샷을 `HasCompleted ? "9999999999" : Number`로 바꾸어 번호 조회와 일치시켰다. 실패 Test에 스냅샷 요청 및 최종 번호 캐시 저장 assertion을 추가했다. production 안전 경계는 수정/완화하지 않았다. 해당 fixture 정적 계약을 보강하고 C# 41개 preflight·전체 Node 30개·`git diff --check`를 재검증해 통과했다. Unity 재실행은 하지 않았으며 사용자는 수정본 Import 뒤 `OnlineAccountCoordinatorTests` 전체와 EditMode 회귀/PlayMode Run All 결과를 확인해야 한다.

Step 7 재요청에 따른 정적 재검증(2026-10-03): C# 41개 파일 preflight·신규 Edit 121/Play 8 작성 수·전체 30개 Node Test 파일·서버 정적 계약·`git diff --check`를 다시 실행해 통과했다. 이 정적 재검증 당시 Unity 실행 결과는 미수신이었으며 빌드/Test Runner/Scene/원격 작업은 수행하지 않았다. 당시에는 사용자 Play Mode 재검증이 남아 Step 7 전체가 미완료였다. 현재 완료 여부는 아래 2026-10-04 최신 결과를 따른다.

### 사용자 수동 작업

1. Unity Editor에서 프로젝트를 열고 소스 Import·컴파일 완료를 기다린다. Console의 Compile Error 및 예상하지 않은 Warning/Error 유무를 확인한다.
2. `Window > General > Test Runner`에서 EditMode를 선택하고 변경된 Test와 영향받는 기존 Test를 실행한다. 공통 Local Save/인증/제출 구조가 바뀌었다면 해당 기존 Test 전체를 포함한다.
3. PlayMode 탭에서 AI가 제공한 격리된 UI 상태·수명·중복 클릭·원격 요청 차단 Test를 실행한다. 실제 서비스 검증 도구와 혼동하지 않는다.
4. 실행 수·통과/실패 수를 전달한다. 실패가 있으면 Test 이름·메시지·Stack Trace를 전달한다. 토큰이나 실제 요청 자격 증명은 제외한다.

### 완료 조건

- [x] 사용자 확인: Unity Script Compilation 성공 및 예상 밖 Error/Warning 없음 (2026-10-03).
- [x] 사용자 확인: Edit Mode 850개 실행·850개 통과 및 예상 밖 Error/Warning 없음 (2026-10-03).
- [x] 최신 변경으로 컴파일과 관련 Edit/Play Mode Test가 통과했다 (2026-10-04, 사용자 확인).
- [x] 실패 수정·Scene 기반 전환·영어화 뒤 Test를 재실행했다. 미실행 결과를 통과로 처리하지 않았다 (2026-10-04, 사용자 확인).

### Step 7 최신 사용자 검증 결과 및 완료 — 2026-10-04

| 검증 대상 | 사용자 실행/확인 결과 | 예상 밖 Error/Warning |
|---|---|---|
| Unity Script Compilation | 성공 | 없음 |
| Edit Mode | 850개 실행·850개 통과 | 없음 |
| Play Mode | 242개 실행·242개 통과 | 없음 |

영어화 및 Scene 기반 테스트 전환 이후 전달받은 최신 결과다. 사용자 실행 결과이며 AI가 Unity를 실행하거나 결과 XML을 별도로 확보해 검증한 것으로 기록하지 않는다. 신규 작성 사례 수와 전체 실행 수를 혼동하지 않으며 발견/Skipped 수는 별도 보고가 없어 임의 수치를 기록하지 않는다. 이전 UI fixture 7개 실패 기록은 보존하고 최신 관련 테스트 검증 완료와 구분한다. 기존 AI 정적/Node 근거와 함께 Step 7을 완료 처리한다. 실제 SDK/원격 요청·서비스 원자성·최종 화면 가독성을 통과했다고 판단하지 않는다.

### UI Test Scene 기반 코드 전환 결과 — 2026-10-03

- [x] 사용자가 구성한 Scene/Inspector UI를 사용하는 테스트 코드 전환 및 AI 정적 검증을 완료했다.
- `AccountTransferUIIsolationTests`의 기존 7개 사례를 유지했다. `UnitySetUp`에서 격리 저장 guard가 true인지 먼저 확인한 뒤 `SampleScene`을 로드하고, 해당 Scene의 유일한 `AccountTransferView`와 같은 Host의 `AccountTransferUIController`를 찾는다. 실제 GameSystem boot를 기다려 `SelectSettings`로 Settings를 열며, Inspector 참조를 읽기만 한다. UI·EventSystem 생성, AddComponent, 필드 SetValue, 비활성 TMP로 렌더링을 우회하는 처리는 제거했다.
- 계정 backend만 FileStore/Authentication/Transport 대역으로 교체한다. 자동 production binding Component는 테스트 중 비활성화하고 종료 때 enabled 상태를 복원한다. 이 7개는 실제 UI·참조·수명·중복 클릭을 격리 검증하며 실제 UGS 자동 binding이나 서비스 통합 성공을 증명하지 않는다. 해당 통합은 Step 10/11 검증에 남긴다.
- Inspector 연결 누락, View 중복, 초기 Root 활성, 잘못된 Page/Action 배열·persistent OnClick, TMP Viewport/Text/Font 누락은 Step 8 안내와 함께 실패한다. 자동 생성/보완·Skip·로그 무시는 하지 않는다. 버튼 클릭·연속 Submit 등 자동 판정 가능한 동작을 사용자 수동 조작으로 넘기지 않는다.
- 변경한 source 계약, C# 41개 preflight, 전체 Node Test 30개 파일 및 서버 정적 계약을 통과했다. Scene 파일·Prefab·Build Profile/Scene List는 읽거나 수정하지 않았고 Unity 컴파일/Build/Test Runner/원격 요청은 실행하지 않았다. 이전 사용자 컴파일·Edit Mode 850/850 결과는 이 Play Mode 테스트 코드 변경 전 확인 결과이며, 이 전환 당시에는 최신 수정본 컴파일·Play Mode 실행이 미확인이어서 Step 7·8을 미완료로 유지했다. 이후 2026-10-04 최신 사용자 결과로 완료했다.

남은 사용자 작업:

1. Step 8 적용표대로 `Assets/Scenes/SampleScene.unity`의 UI와 Inspector를 구성하고 저장한다. Host의 View와 UIController는 활성, Root만 초기 비활성으로 둔다. 5개 Page·12개 공통 Action 버튼과 TMP Font/Text/Viewport 참조를 연결하고 버튼 Inspector OnClick은 비워 둔다. 별도 테스트 전용 Canvas/Scene/fixture Component는 만들지 않는다.
2. 기존 `SampleScene`이 활성 Scene List에 포함되어 있는지 확인한다. 이미 포함된 항목을 중복 추가하지 않는다. 빌드는 필요 없다.
3. 최신 소스 Import/컴파일과 예상 밖 Error/Warning 없음을 확인한 뒤 사용자 Test Runner에서 `AccountTransferUIIsolationTests` 7개 및 영향받는 기존 Settings/UIInput/navigation·기록 격리 Play Mode 회귀 범위를 실행한다. Step 7 완료 전에는 Play Mode 전체 결과도 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 전달한다.

## Step 8. 사용자가 계정·기기 이전 화면을 만든다

2026-10-04에 초보자용으로 다시 작성하고 UI 문구를 영어로 변경했다. 아래 순서대로 **사용자가 Unity Editor에서 작업한다**. AI는 Scene을 수정하지 않는다. 지정한 크기·색은 이번 화면의 작업 기준이며 화면 표시를 Unity에서 확인한 결과는 아니다.

### 사용자 완료 보고 및 추가 작업 — 2026-10-04

사용자가 Step 8 Scene 작업 완료를 보고했다. 저장된 Scene을 읽기 전용으로 대조해 유일한 View/UIController, 필수 참조, 5개 Page/12개 공통 Action 버튼, 초기 비활성 Root, Host/TMP 활성, 빈 persistent OnClick, 배경 CanvasGroup 플래그를 확인했다. Navigation은 사용자의 실제 배치 기준을 존중하며 예시 방향을 강제하지 않는다. Scene은 AI가 수정하지 않았다.

영어 Text/Placeholder의 사용자 Scene 반영을 읽기 전용으로 확인했다. 크기/색/부모/배열/이벤트 연결은 영어화 때문에 다시 구성하지 않는다. 코드가 표시하는 계정/이전 안내·상태 문구 및 관련 Edit Mode 메시지 assertion도 영어로 맞췄다. 영어화 뒤 사용자가 최신 컴파일과 Edit Mode 850/850·Play Mode 242/242 통과 및 예상 밖 Error/Warning 없음을 확인했다. 이전 통과를 재사용하지 않고 이 최신 결과로 Step 7·8 검증 조건을 완료 처리했다. 실제 화면의 배치·가독성은 Step 11에 남긴다.

영어화 검증 결과: UI 문자열의 ASCII 전용 source 계약, C# 41개 preflight, 전체 Node 30개 Test 파일 및 서버 정적 계약이 통과했다. 최신 수정 뒤 사용자 `AccountTransferControllerTests` 전체와 영향받는 계정 Edit Mode 회귀, `AccountTransferUIIsolationTests` 7개 및 Play Mode 전체 결과를 확인한다. 빌드/Test Runner는 AI가 실행하지 않았다. 전체 `git diff --check`는 사용자 변경 Scene의 Unity 직렬화 빈 필드 행 공백을 보고했으며 Scene을 자동 수정하거나 이를 Unity 컴파일/기능 오류로 판정하지 않았다. 이번 AI 수정 파일만 대상으로 공백 검사를 별도 수행한다.

### Step 8-1. Scene을 열고 작업 준비하기

1. Unity 위쪽 Play 버튼이 꺼져 있는지 확인한다.
2. Project 창에서 `Assets/Scenes/SampleScene.unity`를 더블클릭한다.
3. Hierarchy 창에서 `UIRoot → MenuCanvas → SettingsPanel → SettingsWindow`를 펼친다.
4. 작업 중 화면을 보기 위해 **SettingsPanel의 Inspector 맨 위 체크박스만 잠시 켠다**. 작업 마지막에 다시 끈다.
5. 기존 SettingsWindow와 그 안의 설정 버튼은 삭제하거나 옮기지 않는다. 새 Canvas나 EventSystem도 만들지 않는다.

이 문서에서 사용하는 방법:

- **자식 만들기:** Hierarchy에서 부모를 선택하고 마우스 오른쪽 버튼을 눌러 UI 메뉴에서 만든다. 만든 오브젝트가 선택한 부모 아래에 있는지 확인한다.
- **빈 UI 만들기:** `UI > Panel`로 만든 뒤 Inspector의 **Image Component만 Remove Component**로 제거한다. RectTransform은 남긴다. 아래 표에서 '빈 UI'라고 쓴 곳은 이 방법을 쓴다.
- **이름 바꾸기:** Hierarchy에서 선택하고 F2를 누른다.
- **중앙 배치:** RectTransform의 작은 사각형인 Anchor Presets에서 가운데 칸을 선택한다. Anchors Min/Max와 Pivot은 모두 `X=0.5, Y=0.5`로 맞춘 뒤 표의 Pos X/Pos Y/Width/Height를 입력한다. Rotation은 모두 0, Scale은 모두 1이다.
- **부모 크기 채우기:** Anchors Min=`0,0`, Max=`1,1`, Pivot=`0.5,0.5`로 맞춘다. Left/Right/Top/Bottom을 모두 0으로 입력한다. 표의 '전체 채우기'는 이 뜻이다.
- **색 입력:** Image나 TMP Text의 Color를 클릭하고 Hexadecimal 칸에 표의 8자리 값을 넣는다. 마지막 두 자리는 투명도다. `FF`는 불투명, `99`는 약 60% 불투명이다.
- **참조 연결:** Hierarchy의 오브젝트를 Inspector의 해당 칸으로 마우스로 끌어다 놓는다.

수치는 기존 MenuCanvas의 **1920×1080 기준**이다. 기존 CanvasScaler와 기존 설정 화면의 크기는 바꾸지 않는다.

**폰트:** 사용자 결정에 따라 이 화면은 영어로 표시한다. 기존 영어 TMP 글자의 Font Asset을 새 글자에도 연결한다. 한글 Font Asset 추가는 요구하지 않는다. Font Asset을 None으로 두거나 TMP Component를 끄지 않는다.

### Step 8-2. 기존 설정 창에 진입 버튼 추가하기

1. **SettingsWindow**를 선택한다.
2. Inspector의 Add Component에서 `Canvas Group`을 추가한다. 이미 있으면 추가하지 않는다.
3. Canvas Group 값을 다음처럼 맞춘다.

| 항목 | 값 |
|---|---|
| Alpha | 1 |
| Interactable | 켬 |
| Blocks Raycasts | 켬 |
| Ignore Parent Groups | 끔 |

4. SettingsWindow 아래에 `UI > Button - TextMeshPro`로 버튼을 만들고 **OpenAccountTransferButton**으로 이름을 바꾼다.
5. 버튼을 중앙 배치하고 다음 값을 넣는다. 기존 버튼 3개는 그대로 두고 그 아래에 새 버튼을 놓는 위치다.

| 대상 | Pos X | Pos Y | Width | Height | 색 |
|---|---:|---:|---:|---:|---|
| OpenAccountTransferButton의 Image | 0 | -405 | 360 | 48 | `2563EBFF` |
| 버튼 안의 TMP 글자 | 전체 채우기 | — | — | — | `FFFFFFFF` |

6. 버튼 안의 TMP 글자는 `Account / Device Transfer`, Font Size=`22`, Alignment=`Center / Middle`로 한다. Auto Size는 끈다. 글자의 Raycast Target도 끈다.
7. 버튼의 **On Click() 목록은 비워 둔다**. 기존 버튼을 복제했다면 복제본의 On Click() 항목만 모두 제거한다. 새 버튼에 기존 GameSystem 함수를 연결하지 않는다.

기존 `SettingsRecoveryNoticeButton`은 다른 기능의 버튼이다. 삭제하거나 새 버튼으로 바꾸지 않는다.

### Step 8-3. 새 창의 바탕 만들기

아래 순서대로 만든다. **AccountTransferHost와 SettingsWindow는 나란히 있는 형제 오브젝트**다.

```text
SettingsPanel
├─ SettingsWindow                  기존 설정 창
│  └─ OpenAccountTransferButton     방금 만든 버튼
└─ AccountTransferHost              새로 만들 빈 UI
   └─ AccountTransferRoot           빈 UI
      ├─ DimBackground             Image
      └─ TransferWindow            Image
         ├─ TitleText              TMP 글자
         ├─ AccountText            TMP 글자
         ├─ StatusText             TMP 글자
         ├─ Pages                  빈 UI
         └─ Actions                빈 UI
```

1. **SettingsPanel을 선택**하고 빈 UI를 만든다. 이름은 `AccountTransferHost`로 한다. Hierarchy에서 SettingsWindow보다 아래쪽에 둔다.
2. Host를 선택하고 Add Component로 `AccountTransferView`, `AccountTransferUIController`를 각각 1개 추가한다. 두 Component의 체크박스는 켠다. **Host에 Image는 남기지 않는다.**
3. Host 아래에 빈 UI인 `AccountTransferRoot`를 만든다. 작업 중에는 켜 둔다.
4. Root 아래에 `UI > Image`로 `DimBackground`를 만든다.
5. Root 아래에 또 `UI > Image`로 `TransferWindow`를 만든다. DimBackground보다 아래쪽에 놓는다.
6. TransferWindow 아래에 `UI > Text - TextMeshPro`로 TitleText, AccountText, StatusText를 만든다.
7. TransferWindow 아래에 빈 UI인 Pages와 Actions를 만든다.

| 대상 | 배치 | Pos X | Pos Y | Width | Height | Image 색 |
|---|---|---:|---:|---:|---:|---|
| AccountTransferHost | 전체 채우기 | — | — | — | — | Image 없음 |
| AccountTransferRoot | 전체 채우기 | — | — | — | — | Image 없음 |
| DimBackground | 전체 채우기 | — | — | — | — | `00000099` |
| TransferWindow | 중앙 | 0 | 0 | 1000 | 900 | `172338FF` |
| TitleText | 중앙 | 0 | 400 | 900 | 48 | 아래 글자 표 참고 |
| AccountText | 중앙 | 0 | 310 | 900 | 110 | 아래 글자 표 참고 |
| StatusText | 중앙 | 0 | 190 | 900 | 100 | 아래 글자 표 참고 |
| Pages | 중앙 | 0 | -10 | 900 | 280 | Image 없음 |
| Actions | 중앙 | 0 | -300 | 900 | 272 | Image 없음 |

DimBackground와 TransferWindow의 Image는 **Source Image=None / Image Type=Simple / Raycast Target=켬**으로 한다. 별도 Canvas는 추가하지 않는다.

글자는 다음처럼 설정한다. 모든 TMP Component는 체크박스를 켜고, **Auto Size=끔 / Rich Text=끔 / Raycast Target=끔**으로 한다.

| TMP 대상 | 처음 넣을 Text | Font Size | 색 | 정렬 | 줄바꿈 |
|---|---|---:|---|---|---|
| TitleText | Account / Device Transfer | 34, Bold | `F8FAFCFF` | Center / Middle | 끔 |
| AccountText | 빈칸 | 26 | `F1F5F9FF` | Left / Top | 켬 |
| StatusText | 빈칸 | 24 | `FBBF24FF` | Left / Top | 켬 |

AccountText와 StatusText는 실행할 때 코드가 내용을 넣는다. 공개 번호나 실제 이전 코드를 미리 입력하지 않는다.

### Step 8-4. 화면 5개와 입력란 만들기

1. **Pages 아래에 빈 UI 5개**를 만든다. 이름은 `Overview`, `Issued`, `Input`, `Result`, `DiscardConfirmation`으로 한다.
2. 5개 모두 **전체 채우기**로 한다. Image는 없다. 한 화면을 다른 화면 아래에 만들지 않는다.
3. 아래 표대로 각 화면 안에 글자와 입력란을 만든다.

| 부모 | 만들 대상 | 종류 | Pos X / Pos Y | Width / Height |
|---|---|---|---|---|
| Overview | OverviewText | TMP 글자 | 0 / 0 | 840 / 220 |
| Issued | CredentialText | TMP 글자 | 0 / 0 | 840 / 220 |
| Input | CodeLabel | TMP 글자 | 0 / 105 | 760 / 32 |
| Input | CodeInput | TMP 입력란 | 0 / 45 | 760 / 60 |
| Input | VerificationLabel | TMP 글자 | 0 / -25 | 760 / 32 |
| Input | VerificationInput | TMP 입력란 | 0 / -85 | 760 / 60 |
| Result | ResultText | TMP 글자 | 0 / 0 | 840 / 220 |
| DiscardConfirmation | DiscardText | TMP 글자 | 0 / 0 | 840 / 220 |

표의 글자/입력란은 모두 **중앙 배치**다. 화면 5개 자체만 전체 채우기다.

| TMP 대상 | Text | Font Size | 색 | 정렬 |
|---|---|---:|---|---|
| OverviewText | A public number cannot recover your account. Use a transfer code and verification value to move to another device. | 24 | `CBD5E1FF` | Left / Top |
| CredentialText | 빈칸 | 28 | `F8FAFCFF` | Left / Top |
| CodeLabel | Transfer code | 24 | `CBD5E1FF` | Left / Middle |
| VerificationLabel | Verification value (9 digits) | 24 | `CBD5E1FF` | Left / Middle |
| ResultText | See the status message above for the result. | 24 | `CBD5E1FF` | Left / Top |
| DiscardText | Discarded pending records cannot be uploaded again. Select Confirm Discard to continue. | 24 | `FCA5A5FF` | Left / Top |

이 글자들도 **Auto Size=끔 / Rich Text=끔 / Raycast Target=끔 / 줄바꿈=켬**으로 한다. Font Style은 Normal이다. CredentialText에는 실제 값을 입력하지 않는다.

입력란 만들기:

1. **Input을 선택**하고 `UI > Input Field - TextMeshPro`로 만든다. 이름을 CodeInput으로 바꾼다. 같은 방법으로 VerificationInput을 만든다.
2. 입력란 바탕 Image는 `0B1220FF`, Raycast Target은 켬이다.
3. 각 입력란 안의 **Text Area**는 전체 채우기로 맞춘 뒤 Left/Right=`16`, Top/Bottom=`8`로 한다.
4. Text Area 안의 **Text와 Placeholder**는 전체 채우기, Font Size=`28`, Alignment=`Left / Middle`, Font Style=`Normal`, Auto Size=끔으로 한다. Text 색은 `F1F5F9FF`, Placeholder 색은 `94A3B8FF`이다. 기존 영어 Font Asset도 연결한다.
5. TMP_InputField Component에서 아래 값을 넣는다.

| 항목 | CodeInput | VerificationInput |
|---|---|---|
| Text | 빈칸 | 빈칸 |
| Line Type | Single Line | Password 선택 시 한 줄 입력으로 설정됨 |
| Content Type | Standard | Password |
| Character Limit | 9 | 9 |
| Placeholder의 글자 | ABCD-2345 | 9-digit verification value |
| Interactable | 켬 | 켬 |

6. 각 TMP_InputField의 **Text Viewport**에는 자기 Text Area, **Text Component**에는 자기 Text, **Placeholder**에는 자기 Placeholder를 끌어 넣는다. UI 메뉴로 만들었다면 보통 이미 연결되어 있다. None일 때만 해당 자식을 연결한다.
   Password를 선택하면 일부 입력 설정이 Inspector에서 숨겨진다. 숨겨진 항목을 찾으려고 Content Type을 Custom으로 바꾸지 않는다. 인증값의 숫자 입력 제한은 실행할 때 기존 View 코드가 설정한다.
7. 입력 글자와 Placeholder의 Raycast Target은 끈다. 입력란의 Transition은 Color Tint로 하고 Colors는 Step 8-5의 Button Colors 표와 같게 설정한다. Custom Caret Color를 켜고 Caret Color는 `FFFFFFFF`, Selection Color는 `3B82F666`으로 한다. 입력란의 On Value Changed / On End Edit / On Submit 목록에는 아무 함수도 추가하지 않는다. 입력값을 숫자로 변환하는 기능도 추가하지 않는다.

### Step 8-5. 동작 버튼 12개 만들기

1. **Actions**를 선택하고 Add Component에서 `Grid Layout Group`을 추가한다. 버튼 위치를 자동으로 정렬하는 기능이다.
2. 다음 값을 넣는다. Actions에 다른 Layout Group이나 Content Size Fitter는 추가하지 않는다.

| Grid Layout Group 항목 | 값 |
|---|---|
| Padding Left / Right | 각각 10 |
| Padding Top / Bottom | 각각 6 |
| Cell Size X / Y | 280 / 56 |
| Spacing X / Y | 20 / 12 |
| Start Corner | Upper Left |
| Start Axis | Horizontal |
| Child Alignment | Upper Center |
| Constraint | Fixed Column Count |
| Constraint Count | 3 |

3. Actions 아래에 `UI > Button - TextMeshPro` 버튼을 1개 만든다.
4. 버튼의 Image는 Source Image=None, Image Type=Simple, Raycast Target=켬으로 한다. Button의 Interactable은 켠다. Transition은 Color Tint로 한다.
5. Button의 Colors는 아래처럼 설정한다. 실제 바탕 색은 다음 버튼 표의 Image Color로 지정한다.

| Button Colors 항목 | 값 |
|---|---|
| Normal Color | `FFFFFFFF` |
| Highlighted Color | `DDEBFFFF` |
| Pressed Color | `AFCBEEFF` |
| Selected Color | `DDEBFFFF` |
| Disabled Color | `80808080` |
| Color Multiplier / Fade Duration | 1 / 0.1 |

6. 버튼 안의 TMP 글자는 전체 채우기, Left/Right=`8`, Top/Bottom=`4`, Font Size=`20`, Color=`FFFFFFFF`, Alignment=`Center / Middle`, Font Style=Normal로 한다. Auto Size/줄바꿈/Raycast Target은 끈다.
7. **On Click() 목록을 비워 둔다.** 이 버튼을 Ctrl+D로 11번 복제한다. 각 버튼의 이름·글자·Image 색을 다음처럼 바꾼다. Hierarchy의 위에서 아래 순서도 표와 같게 한다.

| 순서 | 버튼 이름 | 버튼 글자 | Image Color |
|---:|---|---|---|
| 0 | StartTransferButton | Start Transfer | `2563EBFF` |
| 1 | OpenTransferInputButton | Enter Transfer Code | `334155FF` |
| 2 | CompleteTransferButton | Complete Transfer | `2563EBFF` |
| 3 | ReissueTransferButton | Reissue Code | `334155FF` |
| 4 | CancelTransferButton | Cancel Transfer | `B45309FF` |
| 5 | RefreshTransferButton | Refresh Status | `334155FF` |
| 6 | ConfirmConsentButton | Enable Online Features | `2563EBFF` |
| 7 | RetryPendingButton | Retry Pending Uploads | `334155FF` |
| 8 | RequestDiscardButton | Discard Pending Records | `B91C1CFF` |
| 9 | ConfirmDiscardButton | Confirm Discard | `B91C1CFF` |
| 10 | CancelDiscardButton | Keep Records | `334155FF` |
| 11 | TransferBackButton | Close | `475569FF` |

8. 버튼 크기와 위치는 Grid Layout Group이 정한다. 직접 Pos 값을 넣지 않는다. 모두 켜져 있을 때 3열×4행, 각 버튼 280×56으로 놓인다. 실행 중 필요 없는 버튼은 코드가 숨기고 나머지를 다시 정렬한다.
9. 버튼은 반드시 **Actions 아래**에 둔다. Overview나 Issued 같은 화면 안으로 옮기지 않는다. 새 TransferBackButton은 계정 창을 닫는 버튼이며 기존 설정 창의 BackButton과 다르다.
10. 처음 만든 진입 버튼 OpenAccountTransferButton에도 위의 Button Colors 값을 적용한다. 진입 버튼의 크기 360×48과 글자 크기 22는 유지한다.

### Step 8-6. Inspector의 빈칸 연결하기

**AccountTransferHost**를 선택한다. 아래 표의 오른쪽 오브젝트를 왼쪽 Inspector 칸으로 끌어 넣는다. '같은 Host'는 지금 선택한 AccountTransferHost를 뜻한다.

AccountTransferUIController:

| Inspector 칸 | 끌어 넣을 대상 |
|---|---|
| Game System (`_gameSystem`) | `GameRoot/Systems/GameSystem` |
| View (`_view`) | 같은 Host의 AccountTransferView |

AccountTransferView:

| Inspector 칸 | 끌어 넣을 대상 |
|---|---|
| Root (`_root`) | AccountTransferRoot |
| Settings Content (`_settingsContent`) | 기존 SettingsWindow. 추가한 CanvasGroup이 연결된다 |
| Account Text (`_accountText`) | TransferWindow/AccountText |
| Status Text (`_statusText`) | TransferWindow/StatusText |
| Credential Text (`_credentialText`) | Pages/Issued/CredentialText |
| Code Input (`_codeInput`) | Pages/Input/CodeInput |
| Verification Input (`_verificationInput`) | Pages/Input/VerificationInput |
| Open Button (`_openButton`) | SettingsWindow/OpenAccountTransferButton |

**Pages (`_pages`)**를 펼치고 Size를 **5**로 넣는다. Element는 아래 순서대로 연결한다.

| Inspector 칸 | 끌어 넣을 대상 |
|---|---|
| Element 0 | Overview |
| Element 1 | Issued |
| Element 2 | Input |
| Element 3 | Result |
| Element 4 | DiscardConfirmation |

**Action Buttons (`_actionButtons`)**를 펼치고 Size를 **12**로 넣는다. Step 8-5 버튼 표의 순서 0~11대로 각 Button을 연결한다. 예를 들어 Element 0은 StartTransferButton, Element 11은 TransferBackButton이다. 같은 버튼을 여러 칸에 넣지 않는다.

기존 UIManagementSystem과 SettingsUIController의 Inspector는 새 Host로 바꾸지 않는다. AccountTransferView/UIController를 다른 오브젝트에 더 추가하지 않는다.

### Step 8-7. 키보드 이동과 저장 상태 맞추기

사용자는 버튼의 실제 위치에 맞춰 Up/Down/Left/Right 연결을 변경했다고 보고했다 (2026-10-04). 그 연결을 그대로 유지한다. 아래 표는 세 버튼을 세로로 배치했을 때의 예시이며, 현재 배치에 맞지 않으면 되돌리지 않는다. 방향키를 눌렀을 때 해당 방향의 가까운 버튼으로 이동하도록 연결한다.

| 선택할 버튼 | Button의 Navigation | 바꿀 칸 → 끌어 넣을 버튼 |
|---|---|---|
| 기존 RestoreDefaultsButton | Explicit | Select On Down → OpenAccountTransferButton |
| 새 OpenAccountTransferButton | Explicit | Select On Up → RestoreDefaultsButton, Select On Down → 기존 BackButton |
| 기존 BackButton | Explicit | Select On Up → OpenAccountTransferButton |

기존 버튼의 다른 방향 칸은 그대로 둔다. 새 계정 창 안의 버튼/입력란은 실행할 때 코드가 키보드 순서를 정하므로 따로 연결하지 않는다.

편집 중에는 여러 화면을 켜서 배치를 볼 수 있다. **저장하기 직전에** Inspector 맨 위 오브젝트 체크박스를 다음처럼 맞춘다.

| 대상 | 체크박스 |
|---|---|
| SettingsPanel | 끔 |
| SettingsWindow / OpenAccountTransferButton | 켬 |
| AccountTransferHost | 켬 |
| **AccountTransferRoot** | **끔** |
| DimBackground / TransferWindow / Pages / Actions | 켬 |
| Overview | 켬 |
| Issued / Input / Result / DiscardConfirmation | 끔 |
| 12개 동작 버튼 | 켬 |

- Host의 View/UIController와 TMP/Text/InputField **Component 체크박스는 켜 둔다**.
- AccountText/StatusText/CredentialText와 두 입력란의 Text는 빈칸으로 둔다.
- 신규 버튼 13개의 On Click()에는 항목이 없어야 한다.
- Ctrl+S로 **SampleScene을 저장**한다.
- SettingsPanel을 끄면 자식이 회색으로 보이는 것은 정상이다. Host 자체의 체크박스를 끄라는 뜻이 아니다.

### Step 8-8. 작업 완료를 알리고 테스트하기

1. Scene 저장을 마치면 AI에 **'Step 8 Scene 연결·저장 완료'**라고 알린다. AI가 저장된 파일에서 참조·배열·부모 관계·초기 상태·이벤트 중복을 정적으로 검사한다. 이 검사를 대신하려고 모든 버튼을 수동으로 눌러 볼 필요는 없다.
2. 사용자가 Unity의 소스 Import/컴파일 완료를 기다리고 예상 밖 Error/Warning이 없는지 확인한다. 빌드는 하지 않아도 된다.
3. 사용자가 `Window > General > Test Runner`를 열고 PlayMode에서 `AccountTransferUIIsolationTests` 7개를 실행한다. 테스트용 Scene/Component를 추가하거나 UIController를 미리 끄지 않는다.
4. 7개가 통과하면 Step 7에 적은 기존 회귀 범위를 포함해 **PlayMode Run All**을 실행하고 실행 수·통과/실패 수·예상 밖 Error/Warning 유무를 알려준다. 실패하면 테스트 이름·첫 실패 메시지·Stack Trace를 전달한다.
5. 코드 전환 전의 컴파일·Edit Mode 850/850 통과 기록과 이번 결과는 구분한다. 최신 컴파일/Play Mode 결과가 확인되기 전까지 Step 7 전체를 완료하지 않는다.

테스트가 버튼 연속 입력·늦은 응답·창 닫힘·입력값 삭제를 자동으로 확인한다. 사용자가 빠르게 클릭해서 재현할 필요는 없다. 일반 Play 버튼으로 실제 이전 요청을 시험하는 작업은 여기서 하지 않는다. 실제 서비스는 Step 10, 최종 화면 가독성은 Step 11에서 확인한다.

### 완료 조건

- [x] 사용자 Scene 연결·저장 완료 보고 및 영어 label 반영의 읽기 전용 대조를 확인했다 (2026-10-04). 실제 배치에 맞춘 Navigation 변경을 유지한다.
- [x] AI의 Scene 정적 검사와 사용자 최신 Play Mode 242/242 통과·예상 밖 Error/Warning 없음을 확인했다 (2026-10-04).

Step 8은 Scene 연결/초기 구성·검증 범위에서 완료했다. 동적 AccountText/StatusText/CredentialText의 저장 초기 Text에 오브젝트 이름이 남아 있으나 Initialize/Render가 닫힌 상태에서 비우고 실행 중 내용을 대체하므로 이번 통과를 막는 기능 오류로 판단하지 않는다. 초기 Text를 빈칸으로 정리하는 표시 관리 사항과 실제 화면 가독성은 Step 11에서 함께 확인한다. AI는 Scene을 수정하지 않았으며 테스트 결과만으로 서비스 통합/최종 화면 승인을 완료하지 않는다. 다음은 Step 9다.

## Step 9. verification에 필요한 서비스 변경만 적용한다

### Step 9-1. 대상 확인·백업·적용표를 확정한다

사용자는 아래 수동 절차 1~2로 verification 대상과 기존 버전을 확인·백업한다. AI는 로컬 파일·입력 정의·의존성·호환 복구 순서를 정적으로 대조한다.

- [x] 적용 대상·백업·실제 변경표·복구 기준을 기록했다 (2026-10-04 사용자 대상/설정 보고 및 기존 두 Script 소스 백업·Draft 없음 확인).

#### 2026-10-04 AI 준비 결과

- [x] 로컬 대상 9개 endpoint와 같은 이름의 임시 bridge, 입력 정의, 의존성, Client 호출 이름, 환경 상수, Secret 소비 지점과 복구 기준을 대조했다.
- [x] 읽기 전용 검사 `UGS/Verification/check-phase2-deployment.cjs`와 회귀 Test `UGS/Tests/phase2-deployment-plan.test.cjs`를 추가했다. 전체 Node Test 파일 31개와 `UGS/Tests/static-contracts.cjs`가 통과했다.
- [x] 사용자가 실제 verification 대상·원격 Script 목록/활성 version·입력·정책·Board 설정·Secret 부재를 확인했다 (2026-10-04 13:40 KST).
- [x] 사용자 보고로 `query-records` v2와 `submit-record` v1의 로컬 source 백업 완료 및 Draft 없음을 확인했다 (2026-10-04). 입력·정책·Board 설정은 아래 보고 내용으로 기록했다.

AI는 로컬 준비와 사용자 원격 확인 보고의 기록을 완료했다. 사용자 대상/설정 보고와 기존 두 Script source 백업 완료·Draft 없음 확인을 근거로 Step 9-1을 완료했다. 백업 파일 자체를 AI가 열어 대조한 것은 아니다. AI는 원격 접속·Script 게시·Secret 조회/생성·권한 변경·Unity 실행·Scene 수정을 하지 않았다. 다음은 Step 9-2이며 Step 9 전체 및 원격 적용/검증은 아직 미완료다.

#### 적용 대상과 변경표

| 항목 | 로컬 기준 | 원격 확인 상태 |
| --- | --- | --- |
| Project ID | `c76d55cf-7846-494b-9dce-a0797b179b36` | 사용자 보고 일치 (13:40 KST) |
| Environment | `verification` / `a20a46fa-1edb-4d79-9c35-02f2fed31896` | 사용자 보고 일치 (13:40 KST) |
| Stage Board | `fs-stage-stage-001-r1`, ascending / keepBest / rules 1 | Lowest to highest / Best score 확인, 이름에 Rules v1 표시 |
| Infinite Board | `fs-infinite-v2`, descending / keepBest / scoring 2 | Highest to lowest / Best score 확인, 이름에 Scoring v2 표시 |
| Secret 이름 | `FS_TRANSFER_HMAC_VERIFICATION_V1` | 없음. 기존 값/권한 백업 대상 없음. Step 9-2에서 신규 준비 |
| Player 직접 Write 정책 | Cloud Save·Leaderboard의 기존 Deny 유지 | 사용자 보고와 로컬 두 Deny statement 일치 |

아래 9개가 최종 원격 Script다. 로컬 source는 모두 `UGS/CloudCode/<원격 이름>.js`다. 사용자 보고로 기존 2개 업데이트·신규 7개 생성을 확정했다. 지금 생성/게시하지 않는다.

#### 사용자 원격 확인 기록 (2026-10-04 13:40 KST)

| 원격 Script | 기존 활성 version | 실제 변경 대상 | 게시 source 백업 |
| --- | --- | --- | --- |
| `query-records` | 2 | 같은 이름 업데이트, `request`: String / Required / 기본값 없음 유지 | 로컬 백업 완료, Draft 없음 (사용자 확인) |
| `submit-record` | 1 | 같은 이름 업데이트, `request`: String / Required / 기본값 없음 유지 | 로컬 백업 완료, Draft 없음 (사용자 확인) |
| `get-public-player-number` | 없음 | 신규 생성 | 기존 source 없음 |
| `get-account-transfer-status` | 없음 | 신규 생성 | 기존 source 없음 |
| `get-account-personal-bests` | 없음 | 신규 생성 | 기존 source 없음 |
| `start-account-transfer` | 없음 | 신규 생성 | 기존 source 없음 |
| `reissue-account-transfer` | 없음 | 신규 생성 | 기존 source 없음 |
| `cancel-account-transfer` | 없음 | 신규 생성 | 기존 source 없음 |
| `complete-account-transfer` | 없음 | 신규 생성 | 기존 source 없음 |

권한 기록: `DenyPlayerCloudSaveWrites` / `DenyPlayerLeaderboardWrites` 모두 Effect `Deny`, Action `Write`, Principal `Player`, Expires `Never`. Resource는 각각 `urn:ugs:cloud-save:/**`, `urn:ugs:leaderboards:/**`다. 보고한 두 statement는 로컬 정책과 일치한다. 전체 정책의 적용 범위/추가 statement는 보고되지 않았으며 실제 거부 동작의 검증은 Step 10에 남긴다.

Board 기록: Infinite의 Name은 `Flow State Infinite Scoring v2`, 생성 시각은 2026-09-27 06:20 UTC. Stage의 Name은 `Flow State Stage stage-001 Rules v1`, 생성 시각은 2026-09-27 06:19 UTC. 둘 다 Date modified / Last reset / Live version ID는 `-`, Archived versions는 0, Reset schedule / Buckets / Tiers는 `None`이다. 이름의 v1/v2 표시를 별도의 서버 metadata 검증으로 확대하지 않는다.

사용자는 아직 운영 배포 전이며 기존 테스트 기록을 보존하지 않기로 했다. 따라서 **기존 인증 프로필·Local Save 비공개 사본 보관은 이번 준비의 필수 작업에서 제외**한다. 이 결정은 Script source/설정 백업과 별개이며 AI의 원격 자료 삭제·초기화 허가는 아니다. 기존 migration 검증을 통과한 것으로 처리하지 않는다. Step 10에서 필요한 legacy 사례는 별도 통제된 검증 자료로 준비하거나 미확인으로 기록하며, 기존 계정 사본을 다시 요구하지 않는다.

| 원격 Script 이름 (= 로컬 파일명에서 `.js` 제외) | 입력 정의 | 함께 묶는 로컬 의존 파일 수 | Secret 읽기 |
| --- | --- | --- | --- |
| `get-public-player-number` | 없음 (`{}`) | 7 | 없음 |
| `get-account-transfer-status` | 없음 (`{}`) | 14 | 없음 |
| `get-account-personal-bests` | 없음 (`{}`) | 4 | 없음 |
| `start-account-transfer` | 없음 (`{}`) | 10 | 위 Secret |
| `reissue-account-transfer` | 없음 (`{}`) | 10 | 위 Secret |
| `cancel-account-transfer` | `transferId`: String / Required / 기본값 없음 | 6 | 없음 |
| `complete-account-transfer` | `code`, `verificationValue`: 각각 String / Required / 기본값 없음 | 11 | 위 Secret |
| `submit-record` | `request`: String / Required / 기본값 없음 | 4 | 없음 |
| `query-records` | `request`: String / Required / 기본값 없음 | 7 | 없음 |

`request`는 JSON 객체를 문자열로 전달하는 기존 계약이다. 입력이 없는 Script에 임의의 Player ID/Account ID 입력을 추가하지 않는다. 원격에 남은 불필요한 입력도 백업한 뒤 Step 9-3에서 이 정의와 맞춘다.

`UGS/Migration/legacy-barrier-submit.js`는 운영 legacy Client 전환용 참고 구현이다. 2026-10-04 사용자 확인에 따라 현재 운영 배포 전 개발 프로젝트에서는 사용/게시하지 않는다. 기존 `submit-record`를 최종 버전으로 직접 교체한다.

배포 방식은 설치된 Cloud Code 2.10.4 패키지의 Editor authoring/bundling으로 정한다. 최종 9개 파일은 `module.exports.bundling = true`이며 내부 `require` 대상들을 합친 결과로 게시해야 한다. 원본 한 파일만 Dashboard에 붙여넣거나 `account-store`/`transfer-*` 내부 라이브러리를 별도 원격 Script로 만들지 않는다. 외부 SDK는 `@unity-services/cloud-save-1.4`, `@unity-services/leaderboards-1.1`이고 서버 기본 모듈 `crypto`를 사용한다. 정확한 endpoint별 의존 파일 목록은 AI 검사 결과에 포함된다.

원본 source는 `Assets` 밖의 `UGS/CloudCode`에 유지한다. Step 9-3에서 `Assets/CloudCode/Phase2Verification`에 최종 9개 게시 entry를 준비했다. 내부 라이브러리/원본을 복사하지 않고 상대 경로로 원본을 참조하는 wrapper를 사용해 중복 deployable Script와 복사본 불일치를 피한다. 원본 참조·입력·bundling 및 설치 패키지 parser를 AI가 대조한 다음 사용자가 Editor Deployment로 게시한다. 이는 Player Build나 Scene 작업이 아니다. bundling 설명은 설치 패키지 `Documentation~/Authoring/javascript_project.md` 및 [Unity 공식 Editor authoring 안내](https://docs.unity.com/en-us/cloud-code/scripts/how-to-guides/write-scripts/unity-editor)를 기준으로 한다. AI는 bundle 생성/게시를 하지 않았다.

Secret은 start/reissue/complete 3개 endpoint가 읽는다. 32바이트 무작위 값을 정규 Base64로 인코딩한 44글자(`=`로 끝남) 계약과 Secret 권한 준비는 Step 9-2에서 다룬다. 여기서는 기존 Secret 이름·존재 여부·권한 정보만 기록한다. Secret 값을 보거나 백업 파일/채팅/저장소에 복사하지 않는다.

저장소는 Private Custom Item의 `fs8-<kind>-<id>` (`account`, `player`, `owner`, `number`, `ledger`, `allocator`, `t`, `receipt`) 및 `fs_account_v1`/`fs_creation_guard_v1` 키를 코드가 관리한다. 기존 Protected Player ledger `fs_submission_ledger_v1`과 guard, Leaderboard 행/metadata를 유지한다. 사용자가 계정·공개 번호·binding·ledger를 미리 만들거나 초기화하지 않는다. 기존 정책 파일 전체 덮어쓰기도 하지 않는다. 실제 Player Write 거부와 저장 원자성은 Step 10의 확인 사항이다.

#### 사용자 작업: 지금은 확인·백업만 한다

1. Unity Dashboard에서 해당 프로젝트를 열고 Environment를 `verification`으로 선택한다. 화면에서 Project ID와 Environment ID가 위 표와 같은지 확인한다. 다른 환경이라면 작업을 멈춘다. 게임의 Play/온라인 검증 버튼이나 Cloud Code 실행 버튼은 누르지 않는다.
2. 사용자 PC의 비공개 위치에 백업 폴더를 만든다. 예: `20261004_Phase2_verification_predeploy`. Git 저장소 밖에 보관한다. 폴더 안에 대상 Project/Environment ID와 확인 시각을 적은 `inventory.txt`를 만든다.
3. Dashboard의 Cloud Code Scripts 목록에서 위 9개 이름을 찾아 `inventory.txt`에 각각 **있음/없음**, 있으면 **게시된 활성 version**, **게시된 입력 이름/type/Required/기본값**을 기록한다. 목록에 없으면 `없음`이라고만 적고 새로 만들지 않는다. Script가 있는데 게시된 버전이 없으면 `있음, 게시 버전 없음`으로 기록한다.
4. 존재하는 Script마다 **게시된 버전의 전체 source와 입력 정의**를 백업한다. 예: `submit-record.published.js`, `submit-record.params.txt`에 저장하고 version도 기록한다. Dashboard에서 source 내보내기가 제공되면 사용하고, 제공되지 않으면 게시된 source 전체를 복사해 로컬 파일로 저장한다. 편집 중인 Draft와 게시 버전이 다르면 둘을 구분해 각각 저장한다. 화면에서 게시 버전 source를 읽을 수 없거나 권한이 부족하면 Draft만으로 백업 완료 처리하지 말고 해당 제한을 알려준다. **Save/Publish/Deploy는 누르지 않는다.**
5. 현재 Access Control 정책 원문과 적용 범위(Project/Environment), 표시되는 활성 version/갱신 시각을 백업한다. 두 Board의 이름·ID·정렬 방향·keepBest 설정 및 현재 확인 가능한 rules/scoring metadata도 기록한다. Secret은 위 이름의 존재 여부와 화면에 표시되는 접근 권한/적용 범위만 기록한다. 조회가 불가능하면 `확인 불가`라고 기록하고 비밀값은 열지 않는다. 설정을 저장하거나 바꾸지 않는다.
6. 기존 인증 프로필·Local Save 사본 보관은 2026-10-04 사용자 결정으로 제외한다. 별도의 기존 테스트 기록 보존 작업을 요구하지 않는다. 이는 서비스 자료 삭제나 migration 검증 완료를 의미하지 않는다.
7. 아래 보고 양식을 채워 전달한다. 존재하는 Script의 백업이 빠졌거나 활성 version을 모르면 완료가 아니다. 백업 source/정책의 AI 정적 비교가 필요하면 비밀·인증 정보가 없는 파일만 별도로 제공한다. AI가 로컬 입력/의존성을 다시 사람에게 검수시키지는 않는다.

```text
대상 Project/verification Environment ID 일치: 예 / 아니오
확인 시각:
9개 Script 각각: 이름 / 없음 또는 활성 version / source·입력 백업 완료 여부
Draft와 게시 버전 차이: 없음 / 있음(별도 백업 여부)
Access Control 정책·두 Board 설정 백업: 완료 / 미완료(이유)
Secret FS_TRANSFER_HMAC_VERIFICATION_V1: 있음 / 없음 / 확인 불가
Secret 권한/범위 메타데이터: 확인 및 기록 / 확인 불가
기존 계정 인증 프로필·Local Save 비공개 사본: 사용자 결정으로 보관 제외 (확인됨)
원격 수정·게시·실행: 하지 않음
```

Script가 `없음`인 것은 신규 생성 대상으로 기록할 수 있다. Secret이 `없음`이면 Step 9-2에서 준비한다. 원격 Script/권한/대상 정보를 조회할 수 없는 상태는 미확인으로 남기고 적용을 진행하지 않는다. 원격 버전/입력을 받으면 AI가 변경표의 각 행에 신규/변경 여부와 이전 version·백업 근거를 반영한다.

#### 게시 순서와 복구 기준 (최초 운영 legacy 전환 계획, 현재 개발 적용은 Step 9-3 우선)

2026-10-04 사용자 확인으로 현재 개발 환경의 bridge 게시/구버전 로그 배출 보고는 제외했다. 아래 legacy 전환 계획은 운영 중인 구 Client를 위한 참고 기준이며 현재 수행 절차가 아니다. 실제 작업은 수정된 Step 9-3의 호출 중지·최종 9개 직접 게시를 따른다. migration marker 이후 호환 복구 기준과 부분 게시 상태 호출 금지는 유지한다.

1. verification에 요청하는 모든 테스트 Client/검증 도구를 중지하고 신규 요청을 차단한다. 기존 `submit-record`가 legacy 버전이면 같은 이름에 barrier-aware bridge를 적용하는 호환 전환을 준비한다. 이미 C 기반이면 무조건 bridge로 되돌리지 않고 원격 백업 source를 대조해 필요한 변경만 정한다.
2. 이전 source로 시작된 실행이 끝났다는 근거를 원격 로그/운영 상태에서 확인한다. SDK Timeout이나 일정 시간 기다렸다는 사실만으로 종료 판정하지 않는다. 배출을 확인할 수 없으면 migration을 시작하지 않는다.
3. Secret/권한을 준비하고 최종 C 기반 `submit-record`/`query-records` 및 계정·이전 endpoint 묶음을 적용한다. 게시가 원자적이라고 가정하지 않는다. 여러 Script의 version/입력/의존성이 모두 맞을 때까지 호출을 재개하지 않는다. 번호/상태/조회도 최초 provisioning/cutover를 일으킬 수 있으므로 전환 중 시험 호출하지 않는다.
4. cutover 전 복구는 아직 migration marker가 기록되지 않고 자료가 변경되지 않았으며 실행이 배출됐다는 근거가 있을 때에만, 백업한 호환 source·입력·version으로 판단한다. 모르는 상태에서 구버전을 재게시하지 않는다.
5. source migration marker 기록 후에는 marker를 무시하는 구버전 제출로 rollback하지 않는다. marker 삭제·legacy endpoint 추가·Account/번호/행/ledger 초기화를 금지한다. barrier·CAS·현재 binding/receipt를 보존하는 수정으로 복구한다. Client Local Save v6도 구 v5 codec으로 되돌리거나 삭제하지 않는다.

원격 존재 여부와 기존 version은 사용자 보고로 확정했다. 현재 운영 배포 전 개발 상태임을 확인받아 bridge 단계는 제외한다. 실제 migration marker가 기록된 자료의 호환 복구 기준은 유지하며 원격 자료 삭제/초기화는 수행하지 않는다.

#### AI 정적 검증 재실행

```powershell
node UGS/Verification/check-phase2-deployment.cjs
node UGS/Verification/check-phase2-deployment.cjs --json
node UGS/Tests/phase2-deployment-plan.test.cjs
```

AI가 수행하는 읽기 전용 검사다. JSON 결과는 endpoint별 params·전체 로컬 의존 경로·외부 SDK·source 묶음 SHA-256을 포함한다. SHA-256은 LF 정규화한 로컬 source 묶음의 식별값이지 게시 bundle 해시나 원격 version이 아니다. 원격 inventory/backup 확인은 항상 별도 상태다. 사용자에게 이 명령 실행이나 Unity 컴파일/Test/Build 재실행을 추가로 요구하지 않는다.

### Step 9-2. Secret·권한·필요 설정을 준비한다

사용자는 아래 수동 절차 3 및 5의 필요한 설정을 수행한다. 신규 Script 권한을 게시 후 연결해야 하는 경우에는 적용표에 의존 순서를 명시한다. 기존 Player 직접 Write Deny는 유지한다.

- [x] 대상 환경의 Secret·권한·설정이 적용표와 일치한다 (2026-10-04 사용자 Secret 생성·verification 환경 범위·Cloud Code만 접근 확인).

#### AI 준비와 현재 상태 (2026-10-04)

- [x] 서버의 정확한 Secret 이름·32바이트 정규 Base64 계약·`secretManager.getSecret` 주입·3개 소비 endpoint를 정적으로 대조했다. 하드코딩된 운영 키나 기본 키를 추가하지 않았다.
- [x] 사용자 보고한 Player Cloud Save/Leaderboard Write Deny 및 Board 설정을 로컬과 대조했다. 해당 설정은 유지 대상이며 재생성/덮어쓰기는 필요하지 않다.
- [x] 사용자 전용 Secret 생성 도구 `UGS/Verification/new-transfer-hmac-secret.cjs`를 준비했다. AI는 `--self-test`의 가상 값 검사만 실행하며 실제 배포용 Secret을 생성하거나 클립보드에 접근하지 않는다. 최초 `.ps1` 자체 검사 시도는 실행 정책으로 차단돼 실제 실행되지 않았다. 정책 변경 없이 Node 도구로 전환했고 `.ps1`은 Node를 호출하는 호환 wrapper만 남겼다.
- [x] 사용자가 Secret 값 작업 완료를 보고했다 (2026-10-04). 실제 값은 전달받거나 조회하지 않았다.
- [x] 저장 범위 `verification`과 Service access `Cloud Code만`의 최종 확인을 받았다 (2026-10-04 사용자 보고).

사용자 Secret 값 작업 완료·verification 환경에만 유효·Cloud Code만 접근 가능 확인을 근거로 Step 9-2를 완료했다. Secret 실제 값은 AI가 조회/수신하지 않았다. 기존 정책/Board의 대조와 로컬 형식 검증을 사용자 설정 보고와 구분한다. 다음은 Step 9-3이며 Script 게시와 실제 Secret 접근·권한 강제력 검증은 아직 미완료다. 이 Step의 변경은 Secret 1개와 그 Service access뿐이다. Player Build·Scene·Unity Test Runner 작업은 없다.

#### 확정 설정표

| 항목 | 적용 값 / 조치 |
| --- | --- |
| 저장 범위 | Project `c76d55cf-7846-494b-9dce-a0797b179b36`의 Environment `verification` (`a20a46fa-1edb-4d79-9c35-02f2fed31896`)에만 생성 |
| Secret Key | `FS_TRANSFER_HMAC_VERIFICATION_V1` (대소문자/밑줄 그대로) |
| Value | 사용자 PC에서 CSPRNG로 생성한 32바이트를 정규 Base64로 인코딩한 44글자. 끝은 `=`, 따옴표·공백·줄바꿈 없음 |
| Description (선택) | `Verification account transfer HMAC v1` |
| Service access | `Cloud Code`만 선택. Player나 다른 서비스에 접근 권한을 추가하지 않음 |
| Secret 소비 코드 | `start-account-transfer`, `reissue-account-transfer`, `complete-account-transfer` |
| 기존 정책 | `DenyPlayerCloudSaveWrites`, `DenyPlayerLeaderboardWrites` 유지. Player Write Allow 예외 추가 금지 |
| Board / Cloud Save | 기존 Board 유지. Account/번호/ledger/guard 수동 생성·seed·reset 없이 코드가 관리 |

Secret Manager의 서비스 접근은 **Cloud Code 서비스 단위**다. 위 3개는 현재 코드에서 Secret을 읽는 endpoint 목록이지 Dashboard에서 세 Script만 개별 허가하는 메뉴를 뜻하지 않는다. 공식 문서에 따른 신규 Secret의 Service access는 생성할 때 `Cloud Code`를 선택한다. 따라서 Script가 아직 없어도 Secret을 먼저 준비할 수 있으며 개별 Script 게시 후 Secret 권한을 연결하는 작업은 필요하지 않다. ([Unity Service access 안내](https://docs.unity.com/en-us/services/secret-manager/concepts/service-access))

Cloud Save Private/Protected와 Leaderboard 서버 호출은 SDK의 Cloud Code context/serviceToken을 사용한다. 별도 Service Account 키, Client용 Secret, Player Write Allow 정책을 만들지 않는다. 실제 서비스 접근 가능 여부·Player 거부·CAS 원자성은 Step 10에서 검증하며 여기서 성공으로 기록하지 않는다. ([Unity token 지원](https://docs.unity.com/en-us/cloud-code/scripts/how-to-guides/token-support), [서버 접근 제어](https://docs.unity.com/en-us/cloud-code/server-access-control))

#### 사용자 수동 작업: Secret 1개 만들기

1. Unity Dashboard에서 대상 프로젝트를 연다. **Development → Environments → verification → Secrets**로 들어간다. 프로젝트 전체의 **Development → Secrets**나 조직의 Secrets 화면에는 만들지 않는다. Environment ID를 Step 9-1 기록과 맞춘다. ([Unity 환경별 Secret 생성 안내](https://docs.unity.com/en-us/services/secret-manager/tutorials/store-secrets))
2. **Add secret**을 누른다. Key에 `FS_TRANSFER_HMAC_VERIFICATION_V1`, Description에 `Verification account transfer HMAC v1`을 입력한다. 접근 서비스를 선택하는 항목에서 **Cloud Code만 선택**한다. 메뉴가 없거나 권한 오류가 나오면 화면의 안전한 오류 문구만 알려준다. 임의로 조직 전체 권한이나 다른 서비스를 추가하지 않는다.
3. Windows 클립보드 기록/장치 간 동기화를 쓰는 경우 먼저 비활성화한다. 일반 PowerShell을 사용자 PC에서 열고 프로젝트 폴더로 이동해 아래를 실행한다. AI가 실행하는 명령이 아니다. Node와 기본 PowerShell을 사용하며 `.ps1` 실행 정책을 변경하지 않는다. 실행 오류가 나오면 비밀값 없는 오류 안내만 전달한다.

```powershell
Set-Location -LiteralPath 'C:\Unity\Unity_Flow_State'
node UGS/Verification/new-transfer-hmac-secret.cjs
```

4. `Ready:` 안내가 나오면 생성된 키가 **클립보드에만** 들어 있다. Secret의 **Value** 칸에 `Ctrl+V`로 붙여넣는다. 다른 텍스트 입력칸·메모장·채팅에는 붙여넣지 않는다. 이름과 대상 환경, Cloud Code 접근 선택을 확인한 뒤 **Add**를 누른다. 실제 키의 형식은 도구가 확인하므로 사용자가 값을 세거나 AI에 전달할 필요가 없다.
5. 목록에 정확한 Key가 나타나고 저장 범위가 `verification`, Service access가 `Cloud Code`인지 확인한다. 생성 후 값은 다시 표시할 수 없으므로 값 확인을 위해 수정/재생성하지 않는다. 동일 이름의 상위 범위 Secret/override가 뜻밖에 보이면 해당 사실만 알려주고 중복 생성하지 않는다.
6. PowerShell에서 `Set-Clipboard -Value ''`를 실행해 현재 클립보드를 비운다. Windows 클립보드 기록/장치 간 동기화를 쓰는 경우 생성 전에 비활성화하고, 값이 기록됐다면 `Win+V`에서도 해당 항목을 지운다. 현재 클립보드 비우기가 기록/동기화 사본까지 삭제한다고 가정하지 않는다. Secret 값의 화면 캡처나 파일 저장은 하지 않는다.
7. 아래 결과만 전달한다. 이미 확인한 정책/Board 설정을 다시 만들거나 Script를 게시/실행하지 않는다.

```text
Secret 생성: 완료 / 실패(비밀값 없는 오류 문구)
Project/verification Environment ID 일치: 예 / 아니오
Key: FS_TRANSFER_HMAC_VERIFICATION_V1
저장 범위: verification 환경
Service access: Cloud Code만
Value 준비: 제공 도구 Ready 안내 확인 (값 전달 금지)
클립보드 및 사용 중인 기록 정리: 완료
기존 두 Player Write Deny·Board 설정: 변경하지 않음
```

#### 검증·오류·후속 순서

AI는 도구의 `--self-test`로 실제 서버 검증 함수를 사용해 가상 32바이트 값의 형식과 잘못된 길이/패딩/줄바꿈 거부를 확인했다. 생성 도구는 값을 PowerShell의 표준 입력으로만 전달하며 명령행 인자/파일/터미널에는 쓰지 않는다. 자체 검사는 클립보드에 접근하지 않는다. 전체 Node Test 파일 31개 및 정적 계약 검사가 통과했다. 기존 Node 회귀는 Secret 없음/SDK 실패를 `TransferSecretUnavailable`로 안전하게 분류하고 원문/비밀이 응답·저장·로그로 나가지 않는 계약을 검사한다. 실제 원격 Secret 값을 AI가 읽어 검증하지 않는다.

Step 9-3에서 bundled Script를 게시한 뒤 Step 10에서 인증된 요청으로 실제 Secret 접근을 검증한다. Cloud Code는 Secret을 최대 5분 캐시할 수 있어 변경 직후 결과를 근거로 키를 반복 교체하지 않는다. 이것은 기존 Script 실행 배출을 확인하는 대기 시간과 별개다. 이전이 진행 중인 키를 임의로 변경/삭제하면 credential 검증이 끊길 수 있으므로 사용 이후 키 변경은 별도 복구 계획 없이 하지 않는다. ([Unity Script Secret 통합·캐시 안내](https://docs.unity.com/en-us/services/secret-manager/tutorials/integrations/cloud-code/scripts))

### Step 9-3. 호환 순서로 게시하고 적용 결과를 기록한다

사용자는 아래 **현재 수행 절차: C# Module 전환**을 수행한다. 원래 Script 적용표는 진단 이력이며 현재 게시 대상은 Module 하나다. 저장 자료의 cutover 뒤에는 무조건 구버전을 재게시하지 않고 Step 1의 호환 복구 기준으로 판정한다.

- [x] Module·함수 입력 정의·의존성·게시 결과를 기록했다. Module 게시 version/hash는 미보고이며 API 스펙 version과 구분한다 (2026-10-04).

#### 2026-10-04 첫 게시 결과와 로컬 진단

사용자 첨부 보고로 대상 Project/verification ID 일치·신규 요청 중지·게시 중 호출 없음·bridge 미사용·Editor Deployment/JS 초기화 성공·Secret/기존 Deny/Board 유지·실제 검증 호출 미실행을 확인했다. **게시 3개 성공, 6개 실패로 Step 9-3은 미완료**다. 전체 재게시나 원격 실행 전에 원인을 확인한다.

| Script | 사용자 게시 결과 | 활성 version / 입력 | 로컬 bundle의 crypto import |
| --- | --- | --- | --- |
| `cancel-account-transfer` | 성공 | 1 / `transferId`, STRING, Required True | 없음 |
| `get-account-personal-bests` | 성공 | 1 / 없음 | 없음 |
| `submit-record` | 성공 | 2 / `request`, STRING, Required True | 없음 |
| `complete-account-transfer` | Failed to Publish / Bad Request. Compilation Error | 활성 version 미보고 | 있음 |
| `get-account-transfer-status` | 같은 컴파일 실패 | 활성 version 미보고 | 있음 |
| `get-public-player-number` | 같은 컴파일 실패 | 활성 version 미보고 | 있음 |
| `query-records` | 같은 컴파일 실패 | 게시 전 v2 기록, 실패 후 활성 version은 미확인 | 있음 |
| `reissue-account-transfer` | 같은 컴파일 실패 | 활성 version 미보고 | 있음 |
| `start-account-transfer` | 같은 컴파일 실패 | 활성 version 미보고 | 있음 |

성공 보고의 필수 입력 type/Required는 로컬과 일치한다. 기본값 없음은 재게시 최종 입력 결과에서 확인한다. 실패한 Script의 Draft와 활성 source가 같다고 가정하지 않는다. 게시 성공은 실제 endpoint 동작이나 Secret 접근 성공이 아니다.

AI는 `UGS/Verification/check-phase2-bundles.cjs`로 설치된 Editor JavaScript bundler의 출력만 메모리에서 생성해 검사했다. Unity/Player Build·Test Runner·npm 설치·원격 호출·파일 저장·Secret 조회는 하지 않았다. 9개 bundle 모두 로컬 Node 문법을 통과하고 상대 경로 require가 외부에 남지 않았다. **crypto import가 있는 정확한 6개와 실패한 6개가 일치**한다. Unity의 [지원 라이브러리 전체 목록](https://docs.unity.com/en-us/cloud-code/scripts/reference/available-libraries)에 `crypto`는 없으므로 원격 import 지원 문제가 가장 유력하다. 단, 첨부 로그는 Publish 단계의 일반 Compilation Error만 있어 서버가 실패시킨 정확한 원인 메시지는 아직 미확인이다. 로컬 Node의 crypto 지원을 Cloud Code 지원으로 간주하지 않는다.

추가로 설치 bundler는 9개 모두에 named/default 혼용 경고를 내고 `module.exports`를 `{ default, bundling, params }` 객체 형태로 출력했다. default는 함수다. 이 현상은 성공/실패 Script에 모두 있어 6개 실패만의 원인으로 단정하지 않는다. 실제 서비스의 호출 함수 해석과 호환 여부는 아직 확인하지 않았으며 최종 export 형식도 후속 대조 대상이다.

읽기 전용 진단 회귀 `UGS/Tests/phase2-bundle-diagnostics.test.cjs`를 추가했다. 진단은 외부 SDK를 대역으로 바꿔 정의만 로드하고 endpoint는 호출하지 않는다. 전체 Node Test 파일 33개/정적 계약 검사 통과는 로컬 근거이며 원격 컴파일 성공으로 기록하지 않는다. 원래 32개 회귀는 실제 bundler 결과/원격 import 허용 여부까지 입증하지 못했음을 구분한다.

2026-10-04 사용자 `start-account-transfer` 상세 보고로 원인을 확정했다:

```text
CompilationError: Cannot find module 'crypto'
at start-account-transfer.js:6:20
```

이는 Cloud Code JavaScript 서버가 `crypto`를 import하지 못하는 문제다. Secret 값/범위/권한 오류나 Unity C# 컴파일 문제가 아니다. 단순 재게시·키 교체·권한 변경으로 해결하지 않는다. 다른 5개 실패도 같은 crypto 의존성이라 공통 원인으로 판단하되 각 서버 오류 상세를 직접 확인한 것으로 기록하지 않는다. 앞선 원인 미확인 기록은 최초 로그 분석 시점의 상태이며 이번 상세 보고로 crypto 원인은 확정됐다.

추가 Dashboard 오류 수집은 현재 필요 없다. 기존 공개 번호/Account ID 발급의 CSPRNG, 이전 코드/인증값의 CSPRNG와 HMAC-SHA256을 유지할 수정 경로를 정해야 한다. 권장 검토 방향은 Cloud Code C# Module로 관련 서버 기능을 옮겨 .NET 표준 `RandomNumberGenerator`/`HMACSHA256`을 사용하는 것이다. ([Unity Module의 .NET 런타임](https://docs.unity.com/en-us/cloud-code/modules/overview), [표준 CSPRNG](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator.getbytes), [HMACSHA256](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha256))

Module 전환은 import 한 줄 변경이 아니라 서버 프로젝트/SDK·endpoint 배포 방식·Client transport·Test와 적용표 변경을 포함한다. 실제 전환 범위/Secret 연동은 구현 전에 확인하고 사용자 방향을 받은 뒤 작업한다. 이번 보고 반영에서는 생산 로직·Client·Scene을 바꾸거나 Module 생성/빌드/게시를 수행하지 않았다. 사용자의 Unity 빌드/Test Runner/Scene 작업 경계도 유지한다.

수정 경로 확정 전에는 새 키 생성/교체, Access Control 변경, 전체 재게시, 자료 reset, bridge 게시를 하지 않는다. crypto를 없애기 위해 Math.random/고정 번호/직접 작성한 약한 암호로 대체하지 않는다. Cloud Code Module/별도 보안 서비스 같은 구조 확장은 사용자 방향을 받은 뒤 수행한다. 현재 앱/검증 호출 중지 상태와 Step 9-3 미완료 상태를 유지한다.

#### 현재 수행 절차: C# Module 전환 (2026-10-04)

사용자가 “어떻게든 가능한 방향으로 작업을 진행한다”를 요청하여 아래 경로의 소스를 준비했다. **앞의 ‘수정 경로 승인 전’ 기록은 진단 이력이다. 이전 JS 9개 재게시 절차는 이제 수행하지 않는다.**

- [x] `UGS/Modules/FlowStateVerification`에 독립 .NET 9 서버 프로젝트/solution/게시 profile을 추가했다. Unity Assets 밖에 있어 Unity Player에 서버 코드/Secret이 포함되지 않는다.
- [x] `Assets/CloudCode/FlowStateVerification.ccmr`이 서버 solution을 참조한다. 이 파일 하나만 게시한다.
- [x] 기존 `UGS/CloudCode` 계정·CAS·이전 로직을 embedded resource로 재사용한다. Jint 4.16.4는 서버 내 JavaScript 실행기이며 Unity JavaScript Script 서비스에 crypto를 import하지 않는다. 암호/encoding만 .NET 표준 구현으로 연결한다. Player 제공 source 실행·CLR 접근·공개 HMAC/Secret API는 제공하지 않는다.
- [x] Secret은 공식 Cloud Code .NET SDK의 `SecretManager.GetSecret`으로 기존 이름만 읽는다. Cloud Save/Leaderboards는 같은 service token·Project와 REST 경로를 사용하고 writeLock/batch/404와 불확실한 쓰기 재시도 정책을 유지한다.
- [x] `UgsOnlineRecordTransport`는 기존 Script 식별자/요청·응답 형식을 유지하면서 아래 Module 함수로 연결한다. 알 수 없는 이름은 호출 전에 거부한다. 구 Script fallback은 없다.
- [x] 오프라인 Node 테스트 파일 34개와 정적 계약 통과. 실제 runtime shim의 암호 데이터 일치·9개 endpoint·발급/재발급/취소/기기 B 이전 완료·Secret 불가를 테스트 모형에서 확인했다. **Node VM 결과는 .NET/Jint 실행 성공이 아니다.**
- [x] 사용자 변경 후 Unity Editor 컴파일 성공 확인 (2026-10-04 보고).
- [x] 이번 변경 후 사용자 Unity 컴파일 성공·EditMode 863/863·PlayMode 242/242 및 각각 예상 밖 Error/Warning 없음 확인 (2026-10-04).
- [x] 사용자 `FlowStateVerification.ccmr` 게시 성공 확인 (2026-10-04 보고). 사용자 Editor 게시 결과이며 AI가 빌드/게시하지 않았다.
- [x] 사용자 다운로드 `UGS/FlowStateVerification.yaml`의 Project/Module 이름·POST 함수 9개·입력 이름/String type·기본값 항목 없음이 서버/Client 코드와 일치함을 정적으로 확인했다.
- [x] 실제 게시 대상 verification 환경 확인 (2026-10-04 사용자 보고). YAML에는 Environment ID가 없어 사용자 보고를 별도 근거로 기록한다. Module 게시 version/hash/시각은 미보고이며 추정하지 않는다.
- [x] Step 9-3 완료 (2026-10-04). 실제 Module 실행·Secret 접근·권한 강제력·필수 입력 누락 거부는 Step 10에서 확인한다.

AI는 빌드/Unity Test Runner/원격 게시/Scene 수정을 수행하지 않았다. 로컬 SDK 설치 목록 조회만 했으며 .NET SDK 9.0.315가 있다. SDK·패키지 설치를 새로 수행하지 않았다. 기존 850/242 Test 성공은 이번 transport 변경 전 결과다. EditMode에 Module routing Test 13개를 추가했고 실행하지 않았다.

##### 2026-10-04 사용자 컴파일·Module 게시 성공 보고

**최신 완료 판정:** 사용자가 게시 환경 verification과 Unity Script Compilation 성공·예상 밖 Error/Warning 없음, EditMode **863/863 성공**·예상 밖 Error/Warning 없음, PlayMode **242/242 성공**·예상 밖 Error/Warning 없음을 보고했다. 앞선 `FlowStateVerification.ccmr` 게시 성공 및 다운로드 YAML의 Project/Module·9개 함수/입력 정적 일치 근거와 합쳐 **Step 9-3과 Step 9 적용 범위를 완료**했다. EditMode 863개는 이전 850개에 새 routing Test 13개를 더한 수와 일치하며 실제 성공 판정의 근거는 사용자 실행 보고다. Module 게시 version/hash/시각은 미보고이며 `info.version: 1.0.0`으로 대신하지 않는다. API 스펙에 `required`/환경/인증 강제력 근거가 없는 점은 유지하며 실제 서비스 검증은 Step 10으로 인계한다. 재게시/Secret 변경/Scene 작업은 필요 없다. 아래 확인 대기 기록은 이전 보고 시점의 이력이다. AI는 Unity 빌드/Test Runner/원격 호출을 수행하지 않았고 Phase 2 전체/Step 10~12는 미완료다.

사용자가 Unity Editor 컴파일 성공과 `FlowStateVerification.ccmr` 게시 성공을 보고했다. 앞선 Module 게시 미확인 상태를 이 보고로 갱신한다. 재빌드/재게시나 Secret 재생성을 요구하지 않는다. 이번 변경 후 예상 밖 Error/Warning 여부·EditMode/PlayMode 결과, 실제 게시 대상 Project/verification 환경 및 Dashboard 9개 함수/입력은 아직 보고되지 않았다. 이전 850/242 통과를 새 변경의 결과로 재사용하지 않는다. 따라서 Step 9-3은 남은 확인 대기이며 실제 Module 실행·Secret 접근·서비스 권한 검증은 여전히 Step 10 범위다. AI는 Unity 실행/빌드/Test Runner·원격 호출·Scene 변경 없이 보고만 기록했다.

##### 9-3-A. 현재 상태를 유지한다

**API 스펙 대조 결과 (2026-10-04):** 사용자가 다운로드한 `UGS/FlowStateVerification.yaml`을 읽기 전용으로 서버 entry/Client 함수 매핑과 대조했다. Project ID·Module 이름·POST 함수 9개·입력 이름/문자열 type이 모두 일치하며 `context`/`client` 입력이나 기본값 항목은 없다. 이 항목들을 사용자가 Dashboard에서 다시 대조할 필요는 없다. YAML에는 Environment ID와 입력 `required` 선언이 없으므로 verification 대상과 서비스의 필수 입력 강제 여부는 증명하지 못한다. `info.version: 1.0.0`은 API 스펙 version이며 Module 게시 version으로 기록하지 않는다. `security: - { }`도 실제 인증/권한 보장의 증거가 아니다. 서버의 Project/Environment/Player/serviceToken 검사는 유지되며 실제 인증·Secret·서비스 권한은 Step 10에서 확인한다. 이번 변경 후 Test/Error/Warning 및 verification 환경 확인 대기가 남아 Step 9-3은 미완료다. AI는 빌드/Test Runner/원격 호출/Scene 수정 없이 정적 대조와 기록만 수행했다.

1. Editor Play·테스트 앱·온라인 검증 요청은 계속 중지한다. 게시만 확인하는 동안 Dashboard Run/Test도 누르지 않는다.
2. Secret `FS_TRANSFER_HMAC_VERIFICATION_V1` 값/verification 범위/Cloud Code 전용 접근은 그대로 둔다. Secret을 다시 만들 필요가 없다.
3. 기존 Player Write Deny 2개와 Leaderboard 2개도 그대로 둔다. bridge 게시·데이터 reset·기존 Script 삭제·구버전 복원은 하지 않는다.

##### 9-3-B. Unity 컴파일과 Module 참조를 확인한다 (Scene 작업 없음)

1. Unity Editor로 돌아가 새 파일 import와 C# 컴파일이 끝날 때까지 기다린다. Error/예상 밖 Warning이 있으면 첫 오류를 전달한다.
2. 사용자 Unity Test Runner에서 EditMode/PlayMode 테스트를 실행한다. AI는 실행하지 않는다. Test는 원격 호출 없이 실행되며 새 routing Test도 포함된다.
3. Project 창에서 `Assets → CloudCode → FlowStateVerification.ccmr`을 선택한다. Inspector의 Module Project/solution 경로는 `UGS/Modules/FlowStateVerification/FlowStateVerification.sln`이어야 한다. 참조 자체는 AI가 정적으로 확인했다. 경로가 비거나 다른 값을 가리키면 알려준다.
4. **Generate Module / Generate Bindings는 누르지 않는다.** 서버 프로젝트와 Client 연결을 이미 준비했다. Scene에 추가할 Component/UI/참조는 없다.

##### 9-3-C. Module 하나만 게시한다

1. 기존 **Services → Deployment** 창을 연다. 환경은 **verification**, Project ID는 `c76d55cf-7846-494b-9dce-a0797b179b36`, Environment ID는 `a20a46fa-1edb-4d79-9c35-02f2fed31896`이다. 이미 설치한 Deployment 패키지/JS 초기화는 반복하지 않는다.
2. 목록에서 **FlowStateVerification (C# Module)** 한 개를 선택하고 **Deploy Selected**로 게시한다. 예전 `Phase2Verification` JS 게시 래퍼는 2026-10-06 로컬에서 제거했다.
3. **Deploy Selected**를 누른다. 이 작업은 사용자 Editor가 .NET Module을 빌드한 다음 verification에 게시하는 작업이다. AI는 빌드를 시도하지 않았다. 처음에는 NuGet 의존성 복원이 필요하여 시간이 걸릴 수 있다.
4. Module 항목을 찾지 못하면 `FlowStateVerification.ccmr`만 Reimport 후 목록을 확인한다. 그래도 없거나 .NET SDK 경로를 찾지 못하면 창의 첫 오류를 알려준다. `Library`/패키지 파일을 직접 수정하거나 임의 SDK downgrade를 하지 않는다.
5. 실패하면 비밀값 없는 첫 build/publish Error와 단계(restore/build/upload)를 알려준다. 실패 시 호출 중지를 유지한다. 서버 프로젝트를 JS Script로 붙여넣는 방식은 사용하지 않는다.
6. 성공하면 Dashboard **Cloud Code → Modules**에서 같은 verification 환경의 `FlowStateVerification`이 게시되어 있는지 확인한다. 화면에 version/hash/게시 시각이 표시되면 그 실제 값을 보고한다. 예상 version 번호는 강제하지 않는다.

Module은 9개 함수를 한 단위로 게시한다. 원격의 기존 Script 3개 성공/6개 실패 상태를 이 단계에서 삭제하거나 복구할 필요는 없다. 새 Client는 그것들을 호출하지 않는다. **게시되지 않은 Module을 새 Client로 호출하면 실패하므로 Step 10까지 호출 중지를 유지한다.**

##### 9-3-D. 함수와 입력을 확인하고 보고한다

Dashboard Module의 함수 목록에서 아래 9개 이름을 확인한다. params의 이름/대소문자는 정확해야 한다. 모든 문자열 입력은 필수이며 기본값이 없다. `context`/`client`는 서버가 주입하며 사용자가 입력을 만들지 않는다. Client binding 생성은 필요 없다.

2026-10-04 다운로드 YAML로 함수 이름/입력 이름/String type/기본값 항목 없음은 AI가 대조 완료했다. 아래 표는 호출 시 반드시 전달할 입력 계약이다. 생성된 YAML에 `required` 목록이 없으므로 서비스가 누락 입력을 어떤 방식으로 거부하는지는 아직 확인하지 않았다. YAML을 수정하거나 재게시해서 확인을 대체하지 않는다.

| 기존 식별자 | Module 함수 | 사용자 입력 |
| --- | --- | --- |
| get-public-player-number | GetPublicPlayerNumber | 없음 |
| get-account-transfer-status | GetAccountTransferStatus | 없음 |
| get-account-personal-bests | GetAccountPersonalBests | 없음 |
| start-account-transfer | StartAccountTransfer | 없음 |
| reissue-account-transfer | ReissueAccountTransfer | 없음 |
| cancel-account-transfer | CancelAccountTransfer | transferId: String, 필수 |
| complete-account-transfer | CompleteAccountTransfer | code, verificationValue: 각각 String, 필수 |
| submit-record | SubmitRecord | request: String, 필수 |
| query-records | QueryRecords | request: String, 필수 |

`request`는 기존처럼 JSON 문자열이다. 기존 Script 활성 version과 새 Module version은 별개의 기록이다. 함수 입력에 예상 밖 항목이 있으면 직접 변경하지 말고 알려준다. 이 단계에서는 실제 함수 호출을 하지 않는다.

```text
대상 Project/verification ID 일치: 예 / 아니오
앱/온라인 검증 요청 중지: 유지
Unity 컴파일: 성공 / 첫 오류
예상 밖 Compile Error/Warning: 없음 / 내용
EditMode / PlayMode: 성공 수 / 실패 수 (이번 변경 후)
선택한 게시 대상: FlowStateVerification C# Module 한 개
Module 빌드/게시: 성공 / 실패(첫 오류)
Dashboard 게시 version/hash/시각: 표시된 값 / 표시되지 않음
Module 함수 9개 및 입력: 위 표와 일치 / 다른 항목
Secret·Deny 정책·Leaderboard: 변경하지 않음
Scene 작업: 없음
실제 원격 검증 호출: 아직 하지 않음
```

사용자 보고를 받기 전에는 Step 9-3/9 전체를 완료하지 않는다. Module 준비·정적 통과만으로 게시 성공이나 Secret 접근 성공을 선언하지 않는다. 게시/입력 확인 후 Step 10에서 실제 인증된 요청으로 검증한다. production 배포는 하지 않는다.

근거: [Unity Module Editor 게시](https://docs.unity.com/en-us/cloud-code/modules/getting-started), [Module Secret SDK](https://docs.unity.com/en-us/services/secret-manager/tutorials/integrations/cloud-code/modules), [Jint 공식 프로젝트](https://github.com/sebastienros/jint), [Unity Leaderboards REST](https://docs.unity.com/en-us/oas-leaderboards/1.0.0).

### AI 선행 작업

Step 1에서 확정한 실제 SDK/Script 배포 방식을 기준으로 파일→원격 Script 이름→입력 정의→의존성→Secret 이름/권한→필요 저장소 설정→버전 호환→복구 순서의 변경표를 제공한다. 원격 검증 도구와 실행 방법도 이때 제공한다. production 배포·30일 sink 구축은 Phase 4에 남긴다.

### 사용자 수동 작업

1. Unity Dashboard에서 Project `c76d55cf-7846-494b-9dce-a0797b179b36`, `verification` Environment `a20a46fa-1edb-4d79-9c35-02f2fed31896`을 선택한다.
2. 변경표 대상의 기존 Script source·활성 version·입력 정의·권한 정책·Secret 이름/권한을 사용자 로컬에 백업한다. 기존 계정·번호·행·ledger를 초기화하지 않는다.
3. 변경표의 정확한 Secret 이름·대상 환경·접근 권한으로 HMAC 비밀을 준비한다. 비밀값은 사용자 환경에서만 입력하며 채팅이나 저장소에 넣지 않는다.
4. 변경표의 신규/변경 Script와 의존성을 실제 배포 방식으로 적용한다. 입력 정의도 로컬 `params` 및 transport와 일치시킨다. 이름이나 입력을 임의로 만들어 게시하지 않는다.
5. 필요한 권한 변경만 기존 정책과 비교해 적용한다. 기존 Player Cloud Save·Leaderboard Write Deny를 유지한다. CLI/API가 필요한 설정은 AI가 공식 문서로 확인한 정확한 명령을 제공한 후 실행한다.
6. 적용한 version·입력 정의 일치 여부·안전한 오류 분류를 전달한다. 예상치 못한 실패에서는 변경표의 호환 가능한 복구 절차를 수행하고 데이터 삭제로 해결하지 않는다.

### 완료 조건

- [x] verification에 최신 C# Module 적용표대로 게시했고 대상·기존 Script 백업·게시 성공·호환 복구 근거를 기록했다. Module 게시 version/hash는 미보고이며 추정하지 않는다 (2026-10-04).
- [x] 사용자 Secret의 verification 범위/Cloud Code 전용 접근과 기존 권한 설정을 확인했다. 실제 Secret 접근·권한 강제력·저장 원자성·Module 실행은 Step 10에서 검증한다 (2026-10-04).

## Step 10. 실제 서비스의 인증·저장·연결·기록 경계를 검증한다

### Step 10-1. 신규 계정·발급·실제 권한을 확인한다

**작업 상태: 완료 (2026-10-05).** 아래 과거 미완료/확인 대기 기록은 당시 이력이며 최신 완료 근거를 우선한다.

사용자는 준비된 검증 도구로 격리된 신규 계정의 인증·발급·재조회·고정 행 쓰기·Player 직접 Write 거부를 실행한다. 번호·저장 비교는 도구가 자동 판정한다.

- [x] 신규 B 발급·A/B 제출/번호 구분·A 재시작 유지·서비스 토큰 쓰기/Player 두 서비스 403의 실제 결과를 기록했다 (2026-10-05). 최신 Client 변경 후 사용자 컴파일·EditMode 891/891·PlayMode 255/255 및 예상 밖 Error/Warning 없음도 확인했다.

#### AI 준비/정적 검증 결과 (2026-10-04)

**2026-10-05 최신 전체 Unity 회귀 성공·Step 10-1 완료:** 사용자가 최신 Script Compilation 성공 및 예상 밖 Error/Warning 없음, EditMode 891개 시도/891개 성공 및 예상 밖 Error/Warning 없음, PlayMode 255개 시도/255개 성공 및 예상 밖 Error/Warning 없음을 보고했다. 실행 수는 사용자 실제 보고 근거이며 AI가 Unity를 실행하거나 XML을 확보한 결과가 아니다. 발견/Skipped 수는 미보고로 추정하지 않는다. 앞선 사용자 A/B 제출·번호 재조회/기준 캡처·A 재시작 보존·두 서비스 Player 403, AI의 원본 변경 없는 실제 기준 파일 AB_NUMBERS_DISTINCT, 사용자 서버 5초 Module 게시 성공·B 최초 온라인 사용 확인과 합쳐 Step 10-1을 완료한다. AI 정적/Node 및 Step 7 최신 Client 회귀 근거도 갱신한다. A/B 기준 파일은 Step 10-3에서 재사용하며 신규 제출/초기화를 반복하지 않는다.

창 deadline의 상태/호출/late-response/Pending 회귀 Test 통과와 실제 서비스의 wall-clock 시간 측정은 구분한다. 수정 후 창의 실제 5초 종료 관찰, .NET/Jint/서버 정확한 시간 경계, 기동/통신 지연을 포함한 5초 수신은 미보고·미입증이다. 이 성능 관찰은 Step 10-1의 발급·행·권한 결과를 대체하거나 Phase 2 전체 완료 근거로 확대하지 않는다. 다음은 별도 기존 자료 사례의 **Step 10-2**이며 이전·고정 행/metadata·HMAC Secret/단일 활성 연결·원격 경합/원자성은 Step 10-3에 남긴다. 새로운 코드/Scene/Module 변경을 하지 않았으며 AI는 빌드/Test Runner/원격 호출/게시를 수행하지 않았다.

**2026-10-05 최신 보고·창 전체 작업 5초 보강:** 사용자는 앞선 Client 수정의 컴파일 성공, EditMode/PlayMode 미실행, 서버 5초 수정의 Module 게시 성공, B 이번 최초 온라인 사용을 확인했다. A/B 원격 보고·distinct 비교를 합쳐 Step 10-1의 서비스 결과 기록 항목은 완료했다. 이 이후 Client 변경의 컴파일/회귀 Test와 실제 시간 제한은 아직 미확인으로 Step 10-1 전체 및 Step 7 최신 회귀 확인은 완료로 판정하지 않는다.

사용자는 서버 제한 적용 후에도 창의 작업이 10초 이상 대기한다고 보고했다(버튼/정확한 구간 미지정). 코드를 점검하니 계정 패널만 5초 제한이고 일반 Module 호출/직접 Write Probe가 15초, Probe 인증·조회·제출 등이 연속 호출되며 제출 재시도에는 1초/2초 지연이 더해졌다. 이번 변경으로 **현행 Prototype 8 Phase 2 창의 ExecutePhase2/ProbePhase2 버튼 한 번마다 VerificationRemoteGuard.BeginOperation의 단일 5000ms deadline**을 공유한다. 인증/세션 정리/두 transport overload/재시도 지연에 같은 deadline을 적용하고 만료 후 후속 호출을 시작하지 않는다. 직접 Write의 UnityWebRequest도 같은 deadline으로 race하고 timeout 시 Abort하며 per-request timeout도 5초다. Module transport의 기본 15000ms도 5000ms로 낮췄다. 이제 **WindowOperationTimeoutMs=5000 / AccountPanelTimeoutMs=5000 / TimeoutMs=5000**으로 표시한다. 이전 “Module TimeoutMs=15000 유지”는 과거 변경 이력이다.

전체 budget 만료 후 summary는 **FAIL / WINDOW_TIMEOUT / TimeoutMs=5000**으로 종료하고 busy를 해제한다. 늦은 SDK 결과는 관찰하되 표시/계정/다음 요청에 적용하지 않는다. 미확정 Pending은 보존하고 terminal receipt 확인 없이 제출 성공으로 처리하지 않는다. OnlineRecordCoordinator는 deadline이 적용된 retry delay의 TimeoutException에서 추가 시도 없이 종료한다. 이 deadline은 Unity 정상 실행 중 비동기 대기를 제한하며 메인 스레드 정지/긴 동기 callback의 강제 선점은 보장하지 않는다. 이미 서버에 전송된 쓰기의 취소/rollback 보장도 없다. 구 Prototype 7 모드는 현행 검증 대상이 아니다.

EditMode 7사례(auth/account/records/session-clear/retry-delay timeout, 다중 호출 deadline 공유·명시적 다음 동작 재설정, 실제 Pending retry timeout 보존)를 작성했다. Node 37파일/정적 계약/C# 48파일 preflight/중복 선언 검사는 통과했고 source 준비는 Edit 162/Play 21이다. 실제 Unity 실행은 아직 하지 않았다. **이번에는 Client-only 수정이므로 이미 성공한 서버 Module을 다시 게시하지 않는다.** 사용자 작업은 Unity 컴파일 및 영향받는 전체 EditMode/PlayMode 실행 결과(수/성공/예상 밖 Error·Warning)를 보고하는 것이다. A/B 신규 제출을 반복하거나 저장을 초기화하지 않는다. 기존 계정 상태 확인/조회 전용으로 수정된 시간 제한을 확인하며 지연이 남으면 버튼명과 민감 정보 제외 전체 결과를 보고한다. Scene/빌드/게시/Test Runner/원격 호출은 AI가 수행하지 않았다.

**2026-10-05 최신 B 실제 결과와 A/B 파일 비교:** 사용자 B 보고는 격리 예약/Play, B 프로필·저장 분리, Consent=False/원격 비허용의 준비 상태를 확인했다. 원격 허용 뒤 패널 열기는 인증하지 않았고 ConfirmConsent에서 Consent=True/Auth Complete 후 panel Timeout, 명시적 Refresh 뒤 Ready가 됐다. 이어 신규 Stage 제출과 별도 일반 Stage 제출 모두 SERVER_SUBMITTED_AND_ME_VERIFIED, 기준 캡처 NUMBER_STABLE_AND_BASELINE_CAPTURED를 보고했다. 일반 제출은 신규 제출과 별도 후속 실행으로 기록하며 최초 사례를 두 번 완료한 것으로 세지 않는다. Timeout은 최종 Ready/제출 성공과 구분하고, 이 결과로 서버 5초 수정의 게시 여부/정확한 시간을 추정하지 않는다.

사용자 제공 A/B 기준 파일을 읽기 전용으로 비교했다. 처음 SNAPSHOT_INPUT_INVALID였으나 두 파일 모두 UTF-8 BOM을 가진 정상 JSON임을 비공개 내용 출력 없이 확인했다. 비교 도구 parseSnapshot에 선두 BOM만 제거하는 처리를 추가했고 BOM 유무/잘못된 JSON/후행 BOM 거부 Test 4개를 추가했다. 원본 기준·Local Save 파일은 변경하지 않았다. 비교 23개 로컬 사례 통과 후 실제 파일 **PASS / AB_NUMBERS_DISTINCT**: 대상 Project/environment/Stage Board 일치와 서로 다른 유효한 10자리 번호를 확인했다. 번호/본문은 기록하지 않는다. 비교 도구의 출력 UTC는 2026-10-04T16:49:08.847Z이며 작업 일자는 사용자 시간대 기준 2026-10-05다.

A 제출·기준 캡처·재시작 유지·두 서비스 403, B 제출·캡처, A/B 번호 구분의 보고 근거가 모였다. **Step 10-1 전체 완료 처리는 최신 Client 변경 후 Unity 컴파일/전체 EditMode·PlayMode 수/예상 밖 Error·Warning 없음, 서버 5초 수정의 verification Module 게시 성공 및 B 최초 온라인 사용 여부 확인까지 보류한다.** 이번 BOM 읽기 수정에는 Unity/Module 재게시나 Scene 작업이 필요 없다. A/B 신규 제출/프로필·저장 초기화를 반복하지 않고 기준 파일을 이후 Step 10-3 비교용으로 보관한다. Step 10-2/10-3 전체 서비스/이전 검증은 별도 미완료다.

**2026-10-05 서버 응답 작업도 공통 5초로 제한 — 이번에는 Module 재게시 필요:** 사용자 후속 요청으로 FlowStateVerification의 9개 함수가 사용하는 서버 budget을 12000ms에서 **5000ms**로 변경했다. 하나의 invocation clock과 cancellation token을 HTTP 전송·응답 본문 읽기·Jint 실행에 공유한다. Secret 조회는 별도 5초를 더 기다리지 않고 같은 invocation의 남은 시간만 WaitAsync한다. Jint의 15초 실행 제한도 5초로 낮추고 공통 취소 token을 연결해 Execute/Invoke 전환 때 전체 budget이 재시작되지 않도록 했다. 만료 후 새 서비스 호출과 성공 결과 반환을 거부하고 기존 안전한 503/ServiceUnavailable 경로로 처리한다. timeout된 CAS/점수 쓰기는 이미 반영됐을 수 있으므로 rollback/삭제/자동 재시도하지 않는다. Pending/receipt/Preparing 복구 규칙은 그대로 유지한다.

Jint 4.16.4의 [CancellationToken API](https://raw.githubusercontent.com/sebastienros/jint/v4.16.4/Jint/Constraints/ConstraintsOptionsExtensions.cs)를 공식 source와 대조했다. 제한은 협력적 취소이며 임의 CLR callback·프로세스 스케줄링·Cloud Code 기동/큐·왕복 통신까지 포함해 클라이언트 수신 5초를 보장하지 않는다. 서버가 제어하는 작업 대기 budget 5초와 앞선 계정 패널 전체 대기 budget 5초를 각각 적용한다. Client의 일반 Module transport 15초와 Player 직접 Write Probe는 이번 서버 요청 범위에서 변경하지 않았다.

새 Node source contract는 한 invocation deadline, HTTP/body의 token 공유, Secret의 남은 시간, Jint 취소 token, 만료 결과 거부, 서버 C# 3파일 중복 선언/괄호를 검사한다. Node **37파일**, 기존 정적 계약, Client C# 48파일 preflight(Edit 155/Play 21 준비) 모두 통과했다. .NET/Jint의 실제 시간 경계·컴파일/원격 동작은 검증하지 않았고 AI는 빌드/게시/Test Runner/Scene/원격 호출을 하지 않았다. **사용자 작업:** 요청 종료 후 Play 중지 → Services > Deployment에서 verification / FlowStateVerification C# Module 하나만 **Deploy Selected**로 사용자가 빌드·재게시 → 게시 성공 후 Editor 재시작·격리 예약/Play·대상 세션 준비/원격 허용·계정 확인. A 신규 Stage 제출은 반복하지 않고 다음 B 검증을 진행한다. Secret/정책/Board/Scene/저장 초기화는 불필요하다. 바로 아래 Client-only 재게시 불필요 안내는 이전 변경만의 이력이며 이번 서버 수정에는 적용하지 않는다. Step 10-1 전체는 B/번호 구분이 남아 미완료다.

**2026-10-05 최신 사용자 결과·계정 패널 5초 제한:** A의 SERVER_SUBMITTED_AND_ME_VERIFIED / NUMBER_STABLE_AND_BASELINE_CAPTURED에 이어 사용자가 재시작 비교 PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED와 PLAYER_WRITE_DENIED / Leaderboards=403 / CloudSave=403을 보고했다. 전체 복사 시각은 2026-10-04 15:15:08Z이며 계정 Ready/인증 Complete/Pending=0이다. A의 제출·기준 캡처·재시작 보존·직접 Write 거부는 보고된 범위에서 성공했다. 계정 최초 생성 여부/게시 version·hash/최신 전체 Unity Test 수는 이 결과만으로 추정하지 않는다. B 및 A/B 번호 구분 검증이 남아 Step 10-1 전체는 미완료다. 기준 JSON/공개 번호는 문서에 복사하지 않는다.

사용자 요청에 따라 AccountTransferController의 Open/Refresh/ConfirmConsent 상태 확인 경로를 OnlineAccountCoordinator.RefreshPanelAsync에 연결했다. 인증·상태 확인·번호 조회와 완료 복구의 SDK 대기를 **하나의 5000ms deadline**으로 제한한다. 호출별 5초를 누적하지 않고 이미 만료된 budget이면 새 SDK 호출을 시작하지 않는다. timeout은 Error/Timeout으로 대기를 종료하고 busy를 해제하며, 늦은 Task의 성공/실패는 관찰하되 계정/번호/저장을 갱신하거나 자동 재시도하지 않는다. SDK 인증 또는 서버 반영을 강제 취소한 것은 아니며 Unity 메인 스레드가 멈춘 상황의 정확한 wall-clock 종료까지 보장하지 않는다. 기존 record/transfer mutation transport의 15000ms와 서버 budget은 변경하지 않는다. 창에는 별도 **AccountPanelTimeoutMs=5000**을 표시하고 기존 **TimeoutMs=15000**은 Module transport 자체의 제한으로 유지한다. 계정 패널 상태 확인을 기다리는 제한은 5초다.

EditMode에 auth/status/number 지연, 하나의 deadline 공유, 늦은 응답 미적용, explicit Refresh 복구, 빠른 성공, 이미 만료된 budget의 호출 차단을 다루는 Test 5개를 추가했다. Node 36파일·정적 계약·C# 48파일 preflight/중복 선언 검사 통과, source 준비 수 Edit 155/Play 21. Unity 컴파일/실제 Test는 사용자 확인 대기이며 AI는 빌드/Test Runner/Scene/원격 작업을 하지 않았다. **이번 Client 변경에는 Module 재게시/Scene 작업이 필요 없다.** 다음 사용자 작업은 컴파일/관련 Test 확인 후 10-1-D의 별도 B 세션·신규 Stage 제출·기준 파일 저장이다. A의 신규 Stage 제출을 반복하지 않는다. A/B 기준 파일 전체 로컬 경로를 알려주면 AI가 기존 오프라인 distinct 비교를 수행한다(본문/번호를 채팅에 붙이지 않는다).

**최신 일괄 수정·회귀 검증 — 아래 이전 진단 기록보다 이 절차를 우선한다:** 실제 서버 코드를 사용하는 오프라인 대역에서 공개 행 조회 실패를 재현하고 두 결함을 수정했다. (1) Cloud Save가 JSON 객체 키 순서를 바꾸면 account provisioning의 JSON.stringify 비교가 같은 데이터를 AccountConflict/LedgerConflict로 오판했다. 다섯 비교를 JSON 값 비교로 교체했다. 객체 키 순서만 무시하며 값·자료형·배열 순서·키 누락/추가는 계속 검사한다. (2) 기존 행 소유자 매핑 저장 뒤 계정 활성화 전에 실패하면 재조회가 Preparing 계정을 복구하지 못했다. 서버가 조회한 기존 행의 동일 원래 소유자·계정에 한해 provisioning을 재개하고 매핑/소유권을 다시 확인한다. 다른 계정/소유자로 바뀌면 빈 실패 응답을 유지한다. 기존 행·점수·시각·submissionId는 변경하지 않았다. 이는 재현된 로컬 결함이며 사용자 원격 실패의 확정 원인으로 단정하지 않는다.

**정적/오프라인 결과:** Node 테스트 36파일 전부 통과, JavaScript 문법 74파일 통과, 서버/Client 정적 계약 통과, C# 48파일 source preflight 통과. C# 검사에 중복 메서드 선언 감지와 자체 회귀 사례를 추가해 앞선 CS0111 재발을 점검했다(컴파일러를 대신하는 완전한 C# 검사는 아님). 키 순서 변경·두 Board 기존 행·저장 전 실패/저장 후 응답 유실 30개 중단 지점·재개 중 계정/소유권 불일치 거부를 검증했다. 실제 embedded JavaScript shim의 Node VM 대역에서 Stage 제출/조회·중복 제출·더 나쁜 점수의 기존 최고/시각 보존·이전 후 동일 본인 행·이전 장치 권한 거부·기존 두 Board의 공개 행 투영을 확인했다. .NET/Jint 실행 및 원격 호출은 하지 않았다. Unity source 준비 수는 Edit 150/Play 21이며 실행 성공 수가 아니다.

**사용자에게 남은 작업(한 번의 재게시):** 요청이 끝난 뒤 Play 중지 → Unity 컴파일/사용자 Test Runner 확인 → Services > Deployment에서 verification 환경과 Project를 확인하고 **FlowStateVerification (C# Module) 한 개만 Deploy Selected** → 게시 성공 뒤 Editor 재시작 → 같은 A에서 격리 예약·수동 Play·격리 세션 준비·원격 허용·계정 패널/상태 확인 → **Stage 제출 전 조회만 확인 (기록 제출 없음)**을 한 번 실행한다. STAGE_ME_EMPTY이면 기존 Step 10-1 신규 제출 절차를 계속한다. 기존 본인 행이면 삭제/초기화하지 않고 신규 사례 조건과 구분한다. 실패하면 민감 정보 제외 전체 복사 결과를 보낸다. 이번 수정은 서버 JS가 Module에 embedded되므로 Module 재게시가 필요하다. Dashboard의 예전 JS/Deploy All은 사용하지 않는다. Secret/정책/Scene/Board/저장/인증 프로필/Player Build 변경은 불필요하다. AI는 빌드/Test Runner/Scene 수정/원격 호출/게시를 하지 않았다. 실제 결과가 없으므로 **Step 10-1은 미완료**다.

**최신 13:48:51Z 결과와 공개 행 세부 진단:** 사용자는 QueryRecords 응답 수신 뒤 `ResolvePublicRows / ServiceStatus=0 / ServiceUnavailable`을 보고했다. 이는 Leaderboard 목록을 받은 뒤 공개 행을 구성하는 단계의 실패이며, HTTP 0번 오류가 아니라 확보된 HTTP 상태가 없다는 뜻이다. 본인 행 유무/기존 데이터 충돌/실패한 다른 행 소유자는 아직 미확정이다. 임의 행을 제거하거나 무시해 조회를 성공으로 만들지 않는다. 사전 조회에서 Stage 제출은 하지 않았다.

서버 단계에 ReadOwnerMapping/ProvisionLegacyRow/ReadProvisionedOwner/ReadRowAccount/ValidateRowAccount/ValidatePublicNumber/ReadNumberMapping/ValidateNumberMapping/SortPublicRows를 추가하고 내부 `fault.reason`은 whitelist의 `queryFault`로만 전달한다. Client도 whitelist로 걸러 `QueryFault`를 표시한다. 범위는 저장 scope/lock·계정/번호·ledger/legacy/metadata·활성 연결 등 고정 오류 분류이며 임의 Error.message·ID·번호·응답 원문은 제외한다. 실패는 계속 TransientFailure로 처리하고 기존 안전 조건을 완화하지 않는다. Node 대역에서 owner/account/number read의 scope 오류 3개와 legacy 발급의 알려진/임의 오류 2개, PlayMode fault 필터 Test 3개를 추가했다. Node 34파일·정적 계약/preflight(Edit 150/Play 21 준비)는 통과했다. 예상 전체 EditMode 879/PlayMode 255와 실제 실행 수를 구분한다.

**다음 사용자 작업:** Play 중 요청 종료 후 중지 → Unity 컴파일/관련 Test 확인 → Services > Deployment에서 **verification / FlowStateVerification Module 한 개 / Deploy Selected**로 사용자가 Module 빌드·재게시 → Editor 재시작 → 같은 A 격리 예약·수동 Play·준비·원격 허용·계정 확인 → **Stage 제출 전 조회만 확인 (기록 제출 없음)**을 한 번 실행 → 민감 정보 제외 전체 복사를 보낸다. 이번 서버 진단 변경에도 Module 재게시가 필요하며 이전 게시 결과를 새 변경 성공으로 재사용하지 않는다. Secret/정책/Scene/Board/계정·저장 초기화/Player Build 변경은 필요 없다. .NET/Unity 컴파일·게시·원격 근본 원인은 사용자 확인 대기다. AI는 빌드/Test Runner/원격 호출/게시를 하지 않았고 Step 10-1은 미완료다.

**최신 서버 조회 진단 보강 — 이번에는 Module 재게시가 필요하다:** 사용자 13:31:03Z의 조회 전용 결과는 계정 Ready/QueryRecords 응답 수신 후 `QueryStatus=TransientFailure / QueryReason=ServiceUnavailable / RowCount=0`이었다. 이는 성공한 빈 조회가 아니므로 본인 Stage 행 유무는 미확정이다. 해당 Probe는 Stage 기록을 제출하지 않았다. `query-records.js`가 계정/번호/Leaderboard/행 매핑/최종 권한 확인 예외를 공통 ServiceUnavailable로 바꿔 세부 원인을 숨기는 점을 확인했다. REST Leaderboards 조회 경로는 [공식 API](https://docs.unity.com/en-us/oas-leaderboards/1.0.0)와 대조했으나 실패 원인은 아직 미확정이다. 설정/Secret/정책을 임의 변경하지 않는다.

서버 query 응답에 신뢰된 `queryPhase`와 100~599 범위의 `serviceStatus`(미확인 0)를 추가했다. 단계는 ResolveAccount/ReadPublicNumber/ReadLeaderboard/ResolvePublicRows/RecheckAccount/EndpointSetup으로만 구성하고 예외 원문/Player ID/번호/토큰/응답 본문은 담지 않는다. Client는 단계 whitelist와 숫자 범위를 다시 제한해 결과 복사에 `QueryPhase / ServiceStatus`를 표시한다. 성공/권한/기록 제출 조건은 변경하지 않았다. Node SDK 대역으로 403/404/503·잘못된 상태와 비밀 원문 제외를 검증하는 사례 4개, PlayMode 안전한 단계 표시 Test 3개를 추가했다. Node 34파일/서버 정적 계약/source preflight(Edit 150/Play 18 준비)는 통과했지만 .NET/Unity 컴파일·게시·실서비스는 사용자 확인 대기다. 예상 전체 EditMode 879/PlayMode 252는 실제 실행 수와 구분한다.

**현재 사용자 작업:** Play 중 요청 종료를 확인하고 Play를 중지한다. Unity 컴파일·관련 Test 확인 후 **Services → Deployment**에서 verification 환경/기존 Project ID를 확인하고 **FlowStateVerification (C# Module)** 한 개만 선택해 **Deploy Selected**한다. 이 작업의 Module 빌드/게시도 사용자가 Editor에서 수행한다. 예전 JS/다른 서비스/Deploy All은 선택하지 않는다. `UGS/CloudCode/query-records.js`는 Module에 embedded되므로 Dashboard JS Script만 수정해선 반영되지 않는다. 이전 “Module 재게시 불필요”는 Client-only 변경 당시 이력이며 이번 서버 진단 변경에는 적용하지 않는다. 게시 성공 뒤 Editor 재시작 → 격리 예약 → 수동 Play → 같은 A 준비·원격 허용·계정 상태 확인 → **Stage 제출 전 조회만 확인 (기록 제출 없음)**을 한 번 실행하고 민감 정보 제외 전체 복사 결과를 보낸다. `QueryPhase=None / ServiceStatus=0`만 나오면 진단 코드 반영 여부도 미확인이다. Ready만으로 조회 성공으로 판정하지 않고 `STAGE_ME_EMPTY` 전에는 제출하지 않는다. AI는 빌드/Test Runner/Scene 수정/원격 호출/게시를 하지 않았다. Secret/Access Control/Board/Scene/Player Build 변경은 불필요하며 **Step 10-1은 미완료**다.

**최신 A Ready/Stage 사전 검사 실패:** 사용자 12:52:38Z에는 A 계정 Ready·GetPublicPlayerNumber 응답 수신·Pending=0을 확인했고 12:59:46Z에는 QueryRecords 응답 수신 뒤 `STAGE_PROBE_REQUIRES_EMPTY_ME`를 보고했다. 이전 도구는 기존 행/실패 응답/유효하지 않은 본인 행을 같은 문구로 처리하므로 이 보고만으로 기존 기록이 있다고 단정하지 않는다. 해당 실행은 사전 검사에서 반환했으므로 Stage 후보 enqueue/제출을 하지 않았다. 서버 조회 중 legacy 매핑 생성 여부는 미확인이다. 사용자 CS0111 보고 후 `GetStageProbeBlockReason` 중복 선언 두 개를 확인해 단일 선언으로 정리하고 Node 정적 계약에 선언 수 1 검사를 추가했다. 수정 후 파일을 다시 읽어 해당 메서드와 `IsSubmittedMe`가 각각 한 개임을 확인했으며 Node 34개/정적 계약은 통과했다. Unity 재컴파일/Test는 사용자 확인 대기다.

사전 실패를 `STAGE_PROBE_QUERY_FAILED`, `STAGE_PROBE_INVALID_ME`, `STAGE_PROBE_REQUIRES_EMPTY_ME`로 나누고 안전한 `QueryStatus / QueryReason / RowCount`를 출력한다. 임의 응답/번호/행 원문을 출력하지 않는다. **Stage 제출 전 조회만 확인 (기록 제출 없음)** 버튼을 추가했으며 정상 빈 조회도 제출 코드 전에 반환한다. 기존 일반 제출은 유효한 기존 행의 최고/시각 보존 규칙을 유지한다. EditMode 분류 Test 5개 추가, Node 34개/정적 계약/preflight(Edit 150/Play 15 준비) 통과. 실제 Unity 컴파일/Test는 사용자 확인 대기이고 전체 예상 EditMode 879/PlayMode 249는 실제 실행 수와 구분한다.

**다음 사용자 작업:** 변경 후 Unity 컴파일/관련 Test 확인 → Editor 재시작 → 격리 예약·수동 Play → 같은 A 준비·원격 허용·계정 상태 확인 → **Stage 제출 전 조회만 확인 (기록 제출 없음)**을 한 번 실행 → **전체 메시지 복사 (민감 정보 제외)** 결과를 보낸다. 신규 Stage 제출/기존 일반 제출/기준 캡처·저장은 원인이 구분되기 전에 하지 않는다. 기존 행이 확인되면 삭제·새 프로필 초기화로 신규 사례처럼 만들지 않는다. Module 재게시/Secret/Scene/빌드는 불필요하고 Step 10-1은 미완료다. 아래 기존 복합 실패 안내는 당시 이력이다.

**최신 인증 성공·Module Timeout/로그 없음:** 사용자 2026-10-04 12:28:45Z 결과는 `AuthPhase=Complete / AuthFailure=None / SDKState=Initialized`, 계정 결과 `State=Error / Reason=Timeout / Pending=0`이었다. Edit Mode 초기화 문제는 해당 실행에서 해소됐고 `GetAccountTransferStatus` 응답은 시간 초과로 미확정이다. 사용자는 verification Cloud Code 로그가 조회되지 않는다고 보고했다. 로컬 C# Module은 명시적 ILogger를 사용하지 않고 embedded JS `__invoke`에 logger를 전달하지 않으므로 로그가 없다는 사실만으로 호출 미도달/함수 미실행을 판정하지 않는다. SDK의 로컬 호출 시작도 서버 수신을 증명하지 않는다. 계정 provisioning은 원격에 반영됐을 수 있으므로 인증/저장 초기화·Stage 반복 제출은 하지 않는다.

Client transport에 안전한 `Module / Function / ModulePhase / SDKErrorCode / TimeoutMs` 진단을 추가해 현재 계정 상태와 전체 메시지 복사에 포함한다. `RequestStarted`는 SDK 호출 시작, `ClientTimeout`은 15000ms 로컬 제한, `SdkTimeout`은 SDK Task의 TimeoutException, `ResponseReceived/RequestFailed`는 SDK Task 종료, `LateResponseReceived/LateRequestFailed`는 로컬 시간 초과 뒤 종료다. 응답 수신은 업무 성공을 뜻하지 않으며 늦은 응답으로 Timeout 계정 상태를 자동 Ready로 바꾸지 않는다. 이전 요청의 늦은 완료는 새 요청 진단을 덮어쓰지 않는다. 임의 예외 원문·토큰·Player ID·응답 본문은 출력하지 않는다. 제한 시간 연장/자동 재시도/서버·Module 재게시·Scene 변경은 하지 않았다.

**다음 확인:** 사용자 컴파일/Test 확인 후 Editor 재시작 → 격리 예약 → 수동 Play → 같은 A 준비 → 원격 허용 → 계정 패널 열기를 한 번 실행한다. 완료 시 **전체 메시지 복사 (민감 정보 제외)**를 보내고, `ClientTimeout`이면 추가 요청 없이 약 30초 뒤 같은 전체 복사 결과를 한 번 더 보낸다. 결과가 계속 ClientTimeout이면 그대로 보고한다. Console의 새 경고가 있으면 안전한 첫 줄/SDK 코드만 함께 보고한다. Dashboard 로그 검색/Run을 반복 요구하지 않는다. `Ready` 전에는 Stage 제출/기준 캡처를 진행하지 않는다. 이번 신규 초기 transport 진단 Test 1개를 포함한 예상 전체 EditMode 874/PlayMode 249와 실제 실행 수는 구분한다. Node 34개/정적 계약/preflight(Edit 145/Play 15 준비) 통과, 실제 Unity 실행·원격 근본 원인은 미확인으로 **Step 10-1은 미완료**다. AI는 빌드/Test Runner/원격 요청을 수행하지 않았다.

**최신 수정 — 아래 Edit Mode 원격 실행 안내는 폐기한다:** 사용자 11:56:14Z 보고는 `AuthPhase=ServicesInitialize / SDKState=Uninitialized / SDKErrorCode=0`이었다. 설치된 Core SDK `UnityServices.cs:128`은 Edit Mode 초기화를 명시적으로 거부한다. 검증 창이 반대로 Play를 차단한 것이 이번 실패의 원인이다. Secret/Module/동의 설정 문제로 처리하거나 같은 Edit Mode 요청을 반복하지 않는다. **현재 실행 방법은 아래 10-1-A/C의 격리 예약 후 Play 절차**다. 이전 준비/실패 기록의 “Play하지 않는다”는 당시 이력이며 현행 실행 지침이 아니다.

창에 Play 전 격리 실행 예약/취소를 추가했다. 예약은 SessionState에 Editor 세션 한정 표식만 보관하며 인증·원격·디스크 작업을 하지 않는다. 예약 후 사용자가 Play하면 GameSystem은 기존 메모리 Save Store를 사용하고 온라인 초기화를 건너뛴다. 창의 A/B 저장·프로필과 충돌하지 않으며 게임 UI/참조/Scene은 생성하거나 수정하지 않는다. 실제 요청은 수동 Play 실행 중·예약됨·명시적 허용·정확한 Project·비자동 Test/비Batch일 때만 가능하다. Reload Domain/Reload Scene이 꺼진 실행은 예약을 거부한다. 현재 ProjectSettings 정적 값은 options Enabled=1/options=0으로 둘 다 reload되어 별도 설정 작업은 필요 없다. 설정은 AI가 변경하지 않았다. Play 종료 시 허용/표식/민감 정보/도구 상태를 정리하고 준비 세대가 다른 요청은 가드에서 차단한다. 이미 시작한 서버 요청을 강제 취소하는 기능은 아니다.

EditMode 실행 조건 Test를 Play/격리/Project 조건으로 수정하고 차단 사례 1개, PlayMode 자동 예약 차단·종료 정리 Test 1개를 추가했다. Node 파일 34개/서버·Client 정적 계약과 source preflight(Edit 144/Play 15 작성 사례)는 통과했다. 실제 Unity 컴파일/Test/수동 원격 성공은 사용자 확인 대기다. 예상 전체 실행 수는 기존 863/242 이후 추가분 기준 EditMode 873/PlayMode 249이며 실제 발견/실행 수를 보고한다. **Step 10-1은 실제 서비스 결과가 남아 미완료다.** Module 재게시/Secret 변경/Scene/빌드는 불필요하다.

**최신 10-1-A/B 보고와 복사 기능:** A 격리 세션은 `LOCAL_READY`, `Consent=True / Pending=0 / LocalSave=Ready / RemoteAllowed=True`였으나 신규 Stage Probe는 `State=Error / Reason=AuthenticationUnavailable`로 중단했다. gateway의 기존 일반 경고만으로 정확한 실패 원인을 확정할 수 없다. 현재 시도의 인증 실패는 Module 요청/Stage enqueue·제출/기준 캡처 전에 발생했다. 이전 계정 요청에서 provisioning이 수행되었는지는 여전히 미확인이다. SDK 초기화·프로필/Project 검사·익명 로그인 중 어디서 실패했는지 `AuthPhase / AuthFailure / SDKState / SDKErrorCode`로 구분하도록 보강했다. 예외 원문·토큰·Player ID는 출력하지 않으며 기존 환경/프로필 경계는 완화하지 않았다.

창의 각 안내/상태/결과에는 **이 메시지 복사**, 프로필/경로/번호에는 **이 값 복사**를 제공한다. 창 맨 아래의 **전체 메시지 복사 (민감 정보 제외)**는 현재 창에 표시되는 메시지를 한 번에 복사한다(과거 결과 이력은 아님). 공개 번호·경로·이전 코드·인증값·입력 JSON은 `[민감 정보 제외]`로 대체하므로 보고할 때는 이 버튼을 사용한다. **전체 메시지 복사 (민감 정보 포함 / 로컬 전용)**는 확인 대화상자 후 원문을 복사하며 채팅/로그에 공유하지 않는다. 민감한 개별 값 복사도 확인을 거친다. 창을 닫으면 복사용 원문 목록도 정리한다. 자동 Test 중에는 클립보드 변경/확인 대화상자를 실행하지 않는다. PlayMode에 순수 보고서 민감 정보 포함/제외 및 닫기 정리 Test 2개를 추가했으며 실제 Unity 실행은 사용자 확인 대기다.

**이번에 필요한 사용자 확인:** 변경 후 Unity 컴파일과 관련 EditMode/PlayMode Test의 Error/Warning을 확인한다. 기존 863/242 성공 이후 추가한 판정 9개·진단 4개·복사 2개를 포함하면 전체 EditMode 872개/PlayMode 248개가 예상되지만 실제 발견/실행 수를 보고한다. 이어 Editor를 종료·재시작하고 같은 A 선택 → 격리 세션 준비 → 원격 요청 허용 → **계정 패널 열기 / 상태 확인**을 한 번 실행한다. 종료 후 창 아래 **전체 메시지 복사 (민감 정보 제외)**로 결과를 전달한다. `ConfirmConsent`는 계정 패널이 열린 동안에만 표시하며 `Consent=True`이면 비활성이 정상이다. 패널을 열지 않고 Probe만 누르면 해당 버튼은 표시되지 않는다. 동의를 다시 저장하거나 인증/저장 파일을 지우지 않는다. `State=Ready` 전에는 신규 Stage 제출·기준 캡처/저장을 반복하지 않는다. Module 재게시·Secret 변경·Scene·빌드는 필요 없다. **Step 10-1은 인증 실패 원인/실제 서비스 성공 결과를 기다리는 미완료 상태다.**

이번 보강의 로컬 Node 테스트 파일 34개·검증 도구 정적 계약·Unity source preflight(Edit 143/Play 14 작성 사례)는 통과했다. 실제 Unity 컴파일/Test 및 서비스 성공 결과와는 구분하며 AI는 빌드/Test Runner/Scene 수정/원격 요청을 수행하지 않았다.

**10-1-B 사용자 실패 보고와 Editor 창 수정:** 사용자는 `LOCAL_READY`, 2026-10-04 11:04:05Z의 `ACCOUNT_ACTION_FINISHED`를 확인했으나 신규 제출/기준 캡처는 모두 활성 계정 준비 실패했고 기준 파일 저장 버튼도 비활성이었다. 동의는 이미 저장되어 있었다. `ACCOUNT_ACTION_FINISHED`는 호출 종료이지 인증/번호 발급 성공이 아니며, 보고만으로 SDK/Module/저장 실패 중 원인을 확정할 수 없다. 해당 Probe는 활성 계정 확인에서 반환해 Stage 후보 enqueue/제출·기준 캡처 단계로 진행하지 않았다. 계정 확인 자체가 서버 provisioning을 일부 수행했는지는 미확인이다.

창에는 전체 ScrollView·최소 크기 520×420·버튼/체크박스 줄바꿈 및 CalcHeight 기반 높이·저장 경로의 자동 줄바꿈·여러 줄 JSON 입력을 적용했다. 화면이 짧으면 아래로 스크롤한다. 계정 요청 종료/Probe 준비 실패에는 안전한 `State/Reason/Consent/Pending/LocalSave/RemoteAllowed` 분류를 출력하며, 임의 원인 텍스트·토큰·번호·코드는 결과 복사에 넣지 않는다. 레이아웃/진단 정적 계약과 PlayMode 진단 필터 Test 4개·기존 격리 Test의 최소 창 크기 검사를 추가했다. 실제 화면/Unity 컴파일/Test는 사용자 확인 대기다. [Unity 자동 배치 Button](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/guilayout/button), [ToggleLeft](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityeditor/editorguilayout/toggleleft)의 스타일/높이 옵션을 사용했다.

**다음 확인은 전체 10-1-B 반복이 아니다:** 변경 후 컴파일/Test 확인 → Editor 종료/재시작 → 같은 A 격리 세션 준비 → 원격 요청 허용 → 계정 패널 열기/상태 확인 순서로 진행한다. 이미 동의했다면 ConfirmConsent가 비활성이어도 정상이다. 이때 **안전한 결과 복사**로 `State/Reason/Consent/Pending/LocalSave/RemoteAllowed`가 포함된 새 종료 결과를 보고한다. `State=Ready`가 아니면 신규 제출·기준 캡처/내보내기를 반복하지 않는다. 아직 기준 JSON이 생성되지 않았으므로 저장 버튼 비활성은 정상이며 버튼을 강제로 활성화하지 않는다. 같은 A를 유지하고 인증/저장 파일을 지우지 않는다. Module 재게시/Secret 변경/Scene/빌드는 필요 없다. PlayMode는 기존 242 기준 신규 진단 Test 4개로 246개가 예상되나 실제 수를 보고한다. Step 10-1은 원격 실패/진단 대기로 미완료다.

**컴파일 실패 수정:** 사용자 `VerificationRecordComparison.cs(36,28)` CS0111 보고를 확인하니 동일한 `IsSubmittedMe` 선언/본문이 두 번 있었다. 현재 파일을 단일 선언으로 정리하고 정적 계약에 선언 수 1개 검사를 추가했다. 실제 Unity 재컴파일/Test 결과는 사용자 확인 대기다. 앞선 정적 통과를 Unity 컴파일 성공으로 간주하지 않으며 Step 10-1은 미완료다. Module/Scene/빌드/원격 설정은 변경하지 않았다.

- [x] 현재 게시된 `FlowStateVerification` Module 함수와 Client transport 연결, verification 고정 환경·A/B 프로필/Local Save 분리·원격 동의 guard·Test/Batch/Play 중 실행 차단을 정적으로 대조했다.
- [x] 기존 Editor 검증 창에 **Step 10-1 신규 Stage 제출·서버 행 확인 (60초)** 버튼을 추가했다. 제출 전 본인 Stage 행이 없는지 확인하며, 기존 행/조회 실패 시 쓰기 전에 중단한다. 제출 후 Pending 처리 성공과 실제 서버 본인 행 1개·같은 공개 번호·점수 60000ms·양수 서버 수락 시각을 자동 대조한다. 제출 실패 시 Pending을 보존하고 강제 완료 처리하지 않는다.
- [x] 기존 일반 Stage 제출 버튼은 기존 최고를 허용하는 후속 제출 용도로 유지한다. 더 나쁘거나 같은 기록 제출에서는 기존 점수/수락 시각 유지도 대조한다. Step 10-3의 B 후속 제출을 신규 행 전용 조건으로 막지 않는다.
- [x] Player 직접 Write 결과는 토큰/응답 본문 없이 두 서비스 HTTP 코드만 표시한다. 둘 다 403일 때만 PASS다. 401/네트워크 실패/일반 오류를 권한 거부 성공으로 처리하지 않는다.
- [x] 제출 행 판정의 EditMode Test 9개와 정적 계약 검사를 추가했다. 전체 Node Test 파일 34개·정적 계약·Unity 소스/메타 preflight·변경 파일 diff 공백 검사가 통과했다. Node는 실제 UGS에 연결하지 않았다.
- [x] 최신 변경 후 사용자 Unity 컴파일·EditMode 891/891·PlayMode 255/255와 각 단계 예상 밖 Error/Warning 없음 및 아래 적용 범위의 A/B 원격 검증 보고를 받았다 (2026-10-05).

**Step 10-1은 아직 미완료다.** 실제 서비스 호출은 AI가 하지 않았다. 기존 863/242 Test 성공은 이번 보강 전 결과이며 새 Test 9개도 AI가 실행하지 않았다. 서버 Module/Scene/Player Build는 변경하지 않았으므로 Module 재게시나 Scene 작업은 필요 없다.

#### 10-1-A. 새 Editor 세션과 검증 창을 준비한다

1. Unity에서 컴파일이 끝나고 예상 밖 Error/Warning이 없는지 확인한다. 사용자가 EditMode/PlayMode Test를 실행한다. Test 종료 뒤에만 아래 원격 작업을 진행한다. 최신 추가분을 포함하면 전체 EditMode 873개/PlayMode 249개가 예상되지만 실제 발견/실행 수를 보고한다. AI는 Test Runner/빌드를 실행하지 않는다.
2. Unity Editor를 완전히 종료한 뒤 프로젝트를 다시 연다. 다른 테스트 앱도 종료한다. **아직 Play를 누르지 않는다. 먼저 아래 격리 실행을 예약해야 한다.**
3. 상단 메뉴 **Flow State → Online Record Verification**을 연다. 창 맨 위 **Prototype 8 Phase 2 (Scene 없음)** 체크를 켠 상태로 둔다. 구 Prototype 7 모드는 사용하지 않는다.
4. 창에서 **Play 전 격리 실행 예약 (원격 호출 없음)**을 누른다. `ISOLATION_ARMED`와 `IsolationArmed=True`를 확인한 뒤 Unity 상단 **Play**를 직접 누른다. Play 진입 후 검증 창으로 돌아와 `IsolationArmed=True / Playing=True`를 확인한다. **격리 세션 A → 격리 세션 준비 (로컬만)**를 누른다. `LOCAL_READY`, 프로필 `flow-state-phase2-a`, 경로 끝 `Prototype8Verification/A/flow-state-save.json`을 확인한다. 게임 조작은 하지 않는다. `ARM_BLOCKED`이면 Project Settings > Editor > Enter Play Mode Settings에서 Reload Domain/Reload Scene을 켜고 예약부터 다시 수행한다. 창을 닫거나 Play를 중지했으면 원격 작업을 이어 누르지 않는다.
5. A/B는 아직 온라인에 사용하지 않은 프로필이어야 신규 사례다. 이미 해당 프로필로 온라인 검증을 했거나 기존 Local Save/공개 번호가 있으면 알려준다. 계정/인증 토큰/저장 파일을 지워 새 사례처럼 만들지 않는다. Stage 행이 없다는 사실만으로 계정 자체가 처음 생성됐다고 단정하지 않는다.
6. Play 중 창의 **verification 원격 요청·테스트 기록 변경 허용**을 체크한다. 이 뒤의 버튼은 verification에 테스트 계정/기록을 생성한다. Step 9의 요청 중지는 이 통제된 격리 검증에 한해서 해제한다. 게임은 메모리 저장/온라인 비활성 상태이며 다른 앱/Dashboard Run은 계속 중지한다. 현재는 **계정 패널 열기 / 상태 확인**을 한 번 실행하고 **전체 메시지 복사 (민감 정보 제외)** 결과를 보고한다. `State=Ready` 전에는 Stage 제출·기준 캡처를 반복하지 않는다.

#### 10-1-B. A의 인증·번호 발급·서버 행을 확인한다

1. **계정 패널 열기 / 상태 확인**을 누른다.
2. 복구 제한 안내를 읽고 동의하는 경우 **ConfirmConsent** 버튼을 누른다. SDK가 A 프로필로 익명 인증하고 Module이 계정/번호를 발급한다. 이미 동의한 재시작 사례에서는 이 버튼이 비활성인 것이 정상이다.
3. 요청이 끝나면 공개 번호가 표시돼야 한다. 번호 자체를 채팅/보고에 쓰지 않는다. 오류/Retry 상태이면 뒤의 제출 버튼을 누르지 말고 아래 안전한 결과와 예상 밖 오류 유무를 보고한다. `ACCOUNT_ACTION_FINISHED` 문구만으로 성공으로 판단하지 않는다.
4. **Infinite (해제: Stage)** 체크를 **끈다**.
5. **Step 10-1 신규 Stage 제출·서버 행 확인 (60초)**를 **한 번** 누른다. 기대 결과는 `PASS / SERVER_SUBMITTED_AND_ME_VERIFIED`다. 사람이 점수·수락 시각·행 개수를 직접 계산/비교할 필요는 없다.
6. 기존 일반 **격리 Stage 기록 제출 (60초)** 버튼은 이 단계에서 사용하지 않는다. 신규 버튼의 `STAGE_PROBE_REQUIRES_EMPTY_ME`이면 새 Stage 사례가 아니거나 조회 실패이며 실제 신규 검증으로 완료하지 않는다. Pending/행 검증 실패에서는 재클릭·Pending 삭제·수동 ledger 생성으로 해결하지 말고 결과를 보고한다.
7. **내 최고 기준 저장 / 재조회 번호 비교**를 누른다. 기대 결과는 `PASS / NUMBER_STABLE_AND_BASELINE_CAPTURED`다.
8. **기준 자료를 사용자 파일로 저장**을 누르고 파일명을 `step10-1-a-baseline.json`으로 지정한다. 기본으로 열린 A 격리 저장 폴더에 두어도 된다. `flow-state-save.json`을 덮어쓰지 않는다. JSON은 로컬 비교 자료이며 채팅에 붙이지 않는다. 저장한 전체 경로만 기억한다.

#### 10-1-C. Editor 재시작 뒤 A의 번호·기록 유지와 직접 Write 거부를 확인한다

1. 실행 중인 요청이 끝나면 Play를 중지하고 Unity Editor를 완전히 종료·재실행한다. 같은 검증 창을 연다.
2. 다시 **Play 전 격리 실행 예약 → 상단 Play 직접 실행 → A → 격리 세션 준비 (로컬만) → 원격 요청 허용 → 계정 패널 열기 / 상태 확인** 순서로 진행한다. 같은 A 프로필/격리 저장 파일을 재사용한다. SDK 인증 정보를 삭제하지 않는다.
3. **Infinite 체크는 끈다.** 앞에서 저장한 `step10-1-a-baseline.json`을 텍스트 편집기로 열어 전체 내용을 복사하고 검증 창의 **비교할 기준 JSON (로컬 전용)**에 붙여넣는다. Local Save나 다른 파일 내용을 붙이지 않는다.
4. **기준 자료와 번호·점수·수락 시각 비교**를 누른다. 기대 결과는 `PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED`다. 자동 비교가 재시작 전후 공개 번호·본인 행·점수·수락 시각을 확인한다. 순위는 다른 계정의 제출로 달라질 수 있어 비교 대상이 아니다.
5. **Player 직접 Write 403 검증**을 **한 번** 누른다. 기대 결과는 `PASS / PLAYER_WRITE_DENIED / Leaderboards=403 / CloudSave=403`이다. 이는 실제 A의 Player Access Token으로 두 서비스에 시험 쓰기를 보내는 작업이다. 정상 설정에서는 둘 다 거부된다.
6. 403 이외의 결과이면 중단하고 두 HTTP 코드만 보고한다. 2xx이면 실제 테스트 행/probe 키가 생성됐을 수 있다. 삭제·정책 수정·초기화는 별도 확인 전 하지 않는다. 전체 응답/Access Token/Player ID를 공유하지 않는다.

#### 10-1-D. B를 별도 신규 사례로 준비하고 번호 구분을 비교한다

1. Editor를 다시 완전히 종료·재실행한다. **격리 세션 B**를 선택하고 A와 같은 준비·동의·신규 Stage 제출·기준 캡처 순서(10-1-A/B)를 수행한다. 프로필은 `flow-state-phase2-b`, 저장 경로 끝은 `Prototype8Verification/B/flow-state-save.json`이다. 같은 Editor 프로세스에서 A/B를 전환하지 않는다.
2. B 기준 파일은 `step10-1-b-baseline.json`으로 저장한다. A 파일을 덮어쓰지 않는다. 아직 **Start/Reissue/Complete/CancelTransfer/OpenInput**은 누르지 않는다. 이전/Secret 검증은 Step 10-3 작업이다.
3. A/B 기준 파일의 **전체 로컬 경로만** AI에게 알려준다. AI가 기존 `UGS/Verification/compare-service-snapshots.cjs`의 `distinct` 비교를 오프라인으로 실행해 같은 대상/Board이며 서로 다른 10자리 번호인지 판정한다. 사용자가 번호를 눈으로 비교하거나 Node 명령을 직접 실행할 필요는 없다. 두 파일이 AI가 읽을 수 없는 위치라면 접근 가능한 로컬 경로를 별도로 정한다. JSON 본문/번호를 채팅으로 보내지 않는다.

#### 보고 양식과 확인 범위

각 작업 뒤 검증 창의 **안전한 결과 복사**를 사용해 분류/시각을 기록한다. 현재 일부 Probe PASS에는 실행 시각이 없으므로 사용자 확인 시각도 함께 적는다. Error/Warning은 안전한 첫 메시지만 보고하며 민감한 전체 응답을 복사하지 않는다.

```text
이번 변경 후 Unity 컴파일: 성공 / 실패
Compile / EditMode / PlayMode 예상 밖 Error·Warning: 없음 / 안전한 내용
EditMode / PlayMode: 시도 수 / 성공 수 / 실패 수
확인 환경: verification
확인 시각: YYYY-MM-DD HH:mm (KST)
A/B는 이전에 온라인 사용하지 않은 격리 프로필: 예 / 아니오
A 신규 제출: PASS / 분류
A 재조회/기준 캡처: PASS / 분류
A Editor 재시작 비교: PASS / 분류
Player 직접 Write: Leaderboards HTTP 코드 / CloudSave HTTP 코드
B 신규 제출·재조회/기준 캡처: PASS / 분류
A 기준 JSON 전체 로컬 경로:
B 기준 JSON 전체 로컬 경로:
원격 작업 중 예상 밖 Error/Warning: 없음 / 안전한 내용
```

이 Step의 서비스 제출·공개 본인 행 관찰은 서비스 토큰 쓰기 경로의 실제 성공 근거다. private `leaderboardOwnerId` UUID와 실제 행 Player ID의 일치·기기 교체 뒤 고정 행 유지·CAS 원자성은 이 공개 비교만으로 증명하지 않는다. 내부 snapshot의 고정 소유 행 대조는 Step 10-3, 기존 자료 cutover는 Step 10-2로 남긴다. 실제 HMAC Secret 접근도 이전 API를 호출하지 않은 이 Step에서는 미확인이다. `required` 없는 OpenAPI 입력 누락의 서비스 거부 방식 역시 미검증으로 유지한다. Dashboard 초기화/수동 ledger 생성/Module 재게시/빌드/Scene 작업은 이 절차에 필요 없다.

사용자 실제 결과와 A/B 오프라인 비교 근거를 받은 뒤 이 Step의 완료를 판정한다. 필수 실패나 신규 사례 여부가 확인되지 않으면 미완료를 유지하고, 로컬 대역/정적 통과만으로 실제 서비스를 완료 처리하지 않는다.

### Step 10-2. 기존 자료 전환과 기록 보존을 확인한다

**작업 상태: 적용 대상 없음(N/A), 2026-10-05 사용자 승인으로 종료.** 기존 기록 미보존·운영 배포 전 개발 상태에서 실제 미전환 계정이 없음을 확인하고 사용자 “진행한다” 승인으로 이 적용 범위를 제외한다. **실제 최초 전환 검증 PASS/성공은 아니다.** 정적/대역 회귀 근거는 유지하며 실제 이전 데이터가 있는 향후 환경에서는 별도 검증이 필요하다. 10-2-A~D는 이번 프로젝트에서 실행하지 않는 참고 절차다. 이전 “선택 대기/미완료” 표현은 당시 이력이고 아래 Step 10-3으로 진행한다.

**2026-10-05 사용자 확인: 실제 검사 대상 없음.** 사용자는 기존 Leaderboard 기록은 있지만 공개 번호/C 연결이 아직 없는 계정이 없다고 확인했다. A/B 로컬 기준 대조 결과 사용자가 축약해 적은 첫 번호는 A, 세 번째는 B이며 나머지도 이미 공개 번호/C가 존재해 최초 전환 사례로 사용할 수 없다. 실제 번호/accountId는 여기에 기록하지 않는다. 따라서 10-2-B~D는 실행하지 않는다. marker/binding/계정·기록 삭제나 구 Script 재게시로 대상을 만들지 않는다.

정적/오프라인 준비는 완료했으나 **실제 최초 전환·보존 검증은 미완료**다. 운영 배포 전 개발 프로젝트에서 이 항목을 적용 대상 없음으로 제외하고 Step 10-3으로 진행할지, 별도 통제된 합성 원격 사례를 승인받아 준비할지는 사용자 선택 대기다. “대상 없음”을 실제 서비스 검증 PASS로 처리하지 않으며 이번 보고만으로 필수 조건을 임의 제외하거나 Step 10-2/Phase 2 전체를 완료하지 않는다.

사용자는 신규 계정과 구분된 기존 verification 사례로 cutover·번호 부여·행/최고 기록/수락 시각 보존을 확인한다. 이미 전환된 계정으로 최초 전환 검증을 대체하지 않는다.

- [x] 실제 미전환 계정 없음·적용 대상 제외 승인·검증 미실시(N/A)를 기록했다 (2026-10-05). 기존 자료 최초 전환·재실행·보존의 실제 성공은 주장하지 않는다.

#### AI 수행 결과 — 2026-10-05

- [x] 실제 provisioning/legacy store의 원래 Player 소유 행 유지, Protected ledger CAS barrier, Submitted/Rejected receipt·best 복사, source 재확인, 404와 서비스 실패 구분, Preparing 재개 경로를 정적으로 대조했다.
- [x] 기존 `legacy` 비교는 행/핵심 metadata만 판정하므로 이 Step의 최초 전환·ledger 보존 증거로 단독 사용하지 않는다. 새 **UGS/Verification/compare-legacy-cutover.cjs**는 전환 전 binding 없음·미전환 ledger·실제 기록 존재, 전환 후 동일 원래 owner/currentPlayer·revision 1·공개 번호 형식·행 전체 metadata·원본 frozen ledger·Account migration snapshot·C ledger receipt/best, 재실행 후 계정/번호/ledger 보존을 오프라인으로 판정한다. JSON 키 순서와 다른 사람 제출로 바뀐 rank만 비교에서 제외한다.
- [x] 실제 서버 production service와 SDK 대역으로 새 비교 Test **80개 검사**(ledger 있는 기존 자료/ledger 없는 기존 행, 두 Board, 순서/순위 변화, 이력/metadata 변경·부분 자료·잘못된 최초 사례 거부)를 통과했다. provisioning 19/19, JSON 순서/중단 30지점 회귀, 전체 Node **38파일**, 정적 계약, C# 48파일 preflight(Edit 162/Play 21 준비), 두 새 CJS 문법 검사를 통과했다. 실제 원격 최초 전환 성공으로 기록하지 않는다.
- [x] **UGS/Verification/Templates/legacy-before.example.json / legacy-after.example.json**을 준비했다. 빈 template는 PASS가 될 수 없다. 사용자는 아래 절차로 Dashboard 원문을 로컬 파일에 보관하고, AI가 이 형식에 맞게 조립·검사한다. 사용자에게 JSON template 직접 작성을 요구하지 않는다.

이번 변경은 오프라인 도구/Node Test/문서뿐이다. Unity C#·Scene·Module·SDK/Secret/정책은 변경하지 않았다. 기존 사용자 컴파일/EditMode 891/891·PlayMode 255/255 결과는 유지하며 이번 준비 때문에 Unity Test/빌드/게시를 다시 요구하지 않는다. AI는 빌드/Test Runner/Scene 수정/원격 호출/게시/자료 초기화를 하지 않았다. **아직 최초 전환 가능한 실제 사례와 before/after/restart 자료가 없으므로 Step 10-2는 미완료**다.

#### 10-2-A. 먼저, 검사할 옛 계정이 남아 있는지 확인한다

**지금은 이 A 단계만 확인하고 결과를 알려준다. B~D는 AI의 확인을 받은 뒤 진행한다.**

이 Step은 “예전에 저장한 온라인 기록이 새 계정 방식으로 바뀌어도 그대로 남는지” 검사하는 작업이다. Step 10-1에서 만든 A/B 계정은 이미 새 방식이므로 이 검사에 사용할 수 없다.

1. Unity의 Play를 끈다. 다른 게임 앱도 종료한다. Online Verification의 계정 확인·조회 버튼은 아직 누르지 않는다. 버튼을 누르면 검사 전에 옛 자료가 바뀔 수 있다.
2. 다음 두 가지가 남아 있는지 확인한다.
   - **옛 온라인 기록:** Step 10-1의 A/B가 아닌, 그 전에 테스트하던 계정의 Stage 또는 Infinite 기록.
   - **옛 로그인 정보:** 그 계정에 사용했던 `flow-state-verification` 프로필. 이 프로필의 로그인 정보를 지우거나 새 계정으로 바꾸지 않았어야 한다. 토큰을 찾아보거나 복사할 필요는 없다.
3. 다음 양식으로 답한다. ID나 기록 값은 보내지 않는다.

```text
옛 온라인 기록: 있음 / 없음 / 잘 모르겠음
옛 flow-state-verification 로그인 정보: 유지함 / 삭제·초기화함 / 잘 모르겠음
```

**없거나 잘 모르겠으면 여기서 멈춘다.** 이전에 보관하지 않기로 한 자료를 다시 복구하라는 뜻이 아니다. 새 계정을 만들거나 A/B·저장 파일을 지워 대체하지 않는다. AI가 먼저 검사 가능한 상황인지 확인한다.

#### 10-2-B. 바꾸기 전 자료를 파일로 복사한다

**AI가 옛 계정을 사용할 수 있다고 확인한 경우에만 진행한다.**

사용자는 Dashboard의 실제 값을 복사해서 로컬 파일에 보관한다. **JSON 템플릿을 직접 완성하거나, 점수·시간·이력을 계산할 필요는 없다. AI가 파일을 읽어 조립하고 비교한다.**

1. 파일 탐색기에서 문서 폴더 등에 `Step10-2` 폴더를 하나 만든다. 아래 파일들은 모두 이 폴더에 둔다. 기존 Local Save나 A/B 기준 파일은 건드리지 않는다.
2. Unity Dashboard에서 기존 프로젝트와 **verification** 환경을 선택한다. 이후 Dashboard에서는 **조회와 복사만** 한다. Edit/Delete/Create/Save 같은 데이터 변경 버튼은 누르지 않는다.
3. **Leaderboards**에서 다음 두 Board를 연다.
   - Stage: `fs-stage-stage-001-r1`
   - Infinite: `fs-infinite-v2`
4. 옛 계정의 기록 행을 찾는다. 그 행의 **Player ID**를 복사해 메모장에 붙여넣고 `old-player-id.txt`로 저장한다. 이 ID는 이후 Dashboard 검색에만 사용하며 채팅에는 붙이지 않는다.
5. 같은 Player ID의 행에서 **Player ID, score, metadata**를 복사해 각각 `before-stage.txt`, `before-infinite.txt`로 저장한다. JSON으로 볼 수 있으면 원문 그대로 복사한다. 표와 metadata가 따로 보이면 항목 이름과 실제 값을 그대로 붙여넣어도 된다. metadata 안의 `acceptedAt`, `submissionId`와 다른 값도 빼지 않는다.
   - 행이 실제로 없으면 해당 파일에는 `행 없음 확인`이라고만 적는다.
   - 화면을 읽을 수 없거나 metadata를 찾지 못하면 “행 없음”이라고 적지 말고 멈춘 뒤 상황을 알려준다.
6. **Cloud Save → Game Data**에서 **Private** 영역을 선택하고 다음 Custom ID를 검색한다.
   - 검색어: `fs8-player-` 뒤에 4번에서 복사한 Player ID를 붙인 값.
   - 예를 들어 Player ID가 `abc123`이면 검색어는 `fs8-player-abc123`이다. 이 예시를 실제 값으로 사용하지 않는다.
   - 그 안에 `fs_account_v1` 항목이 **있으면 이미 전환됐거나 전환을 시작한 계정**이다. 더 진행하지 말고 “계정 항목 있음”이라고 알려준다.
   - 실제 검색 결과에 해당 항목이 없으면 `before-account-check.txt`에 `Private 영역 확인 / fs_account_v1 항목 없음`이라고 적는다.
   - 메뉴가 다르거나 검색이 실패하면 “없음”으로 처리하지 말고 메뉴 이름·안전한 오류 분류만 알려준다.
7. **Cloud Save → Player Data → Manage Players**에서 같은 Player ID를 검색하고 **Protected** 영역을 선택한다. `fs_submission_ledger_v1` 항목을 찾는다. 전체 값을 보는 방법은 [Unity 공식 Dashboard 안내](https://docs.unity.com/en-us/cloud-save/tutorials/dashboard)를 참고한다.
   - 항목이 있으면 **value 전체**를 복사해 `before-old-ledger.txt`로 저장한다. 값이 JSON이면 그대로 붙여넣는다. key/writeLock이 포함된 바깥 정보 대신 value만 복사하며 내용을 고치지 않는다.
   - 항목이 실제로 없으면 파일에 `항목 없음 확인`이라고 적는다.
8. 여기서 멈추고 **Step10-2 폴더의 전체 경로와 파일 이름들만** 알려준다. 파일 본문은 보내지 않는다.

AI가 파일을 읽어 이전 계정이 맞는지, 전환 전 상태인지, 진행 중인 기록 제출이 없는지 검사한다. 내부 ID·점수·시간·번호는 출력하지 않는다. 파일 내용을 읽을 수 없거나 필요한 값이 빠졌으면 AI가 그 부분만 안내한다.

**AI가 `PASS / LEGACY_BEFORE_READY`를 확인하기 전에는 C를 진행하지 않는다.** 문서의 example JSON은 AI가 기준 자료를 조립할 때 쓰는 형식 참고용이다. 사용자가 직접 채우지 않는다.

#### 10-2-C. 옛 계정으로 접속해 전환한다

**B의 검사에서 PASS를 받은 뒤에만 진행한다.**

1. Unity Editor를 완전히 종료한 뒤 다시 연다. 다른 게임 앱은 계속 꺼 둔다.
2. **Flow State → Online Record Verification**을 연다.
3. **Prototype 8 Phase 2**를 켜고, 격리 세션을 **Legacy**로 선택한다. A나 B를 선택하지 않는다.
4. **Play 전 격리 실행 예약 (원격 호출 없음)**을 누른다.
5. Unity 상단 **Play**를 직접 누른다.
6. 검증 창에서 **격리 세션 준비 (로컬만)**를 누른다. 다음 두 값을 확인한다.
   - 인증 프로필: `flow-state-verification`
   - 저장 경로의 끝: `Prototype8Verification/Legacy/flow-state-save.json`
7. **verification 원격 요청·테스트 기록 변경 허용**을 켠다.
8. **계정 패널 열기 / 상태 확인**을 누른다. 동의하지 않은 상태라면 안내를 읽고 **ConfirmConsent**를 누른다.
9. 결과가 **State=Ready / Reason=None**이면 **전체 메시지 복사 (민감 정보 제외)**로 결과를 알려준다. 그 뒤 D의 전환 후 자료를 보관한다.
   - Timeout/Error이면 **여기서 멈추고 같은 복사 결과를 알려준다.** 이미 서버 저장 일부가 바뀌었을 수 있으므로 AI의 확인 없이 요청을 반복하지 않는다.
   - AI가 Refresh로 재개하도록 안내하면 같은 Legacy 세션과 B에서 보관한 원래 자료를 유지한 채 진행한다. 최초 기준 파일을 새 값으로 덮어쓰지 않는다.

**누르면 안 되는 버튼:** 신규 Stage 제출, 일반 Stage 제출, Infinite 기록 제출, Start, Reissue, Complete, CancelTransfer. 비교가 끝나기 전에 기록을 새로 만들거나 계정을 이전하지 않는다.

Local Save나 로그인 정보를 복사·삭제·초기화할 필요는 없다. 이 Step은 서버 기록 보존 검사이며 Settings 같은 원본 Local Save 전체 보존 검사는 아니다.

#### 10-2-D. 바뀐 뒤와 재시작 뒤 자료를 보관한다

**첫 번째: 전환 직후 자료 보관**

1. C에서 Ready를 확인하면 Dashboard의 **verification** 환경으로 돌아간다. B에서 사용한 같은 옛 Player ID를 사용한다.
2. 다음 표대로 실제 값을 복사해 `Step10-2` 폴더에 저장한다. JSON 문법을 새로 작성하지 말고 **value 전체**를 그대로 복사한다.

| 어디에서 찾는가 | 무엇을 복사하는가 | 저장할 파일 |
| --- | --- | --- |
| Cloud Save / Game Data / Private / `fs8-player-<옛 Player ID>` | `fs_account_v1`의 value 전체 | `after-player.txt` |
| Cloud Save / Game Data / Private / `fs8-account-<accountId>` | `fs_account_v1`의 value 전체 | `after-account.txt` |
| Cloud Save / Game Data / Private / `fs8-ledger-<accountId>` | `fs_account_v1`의 value 전체 | `after-ledger.txt` |
| Cloud Save / Player Data / 같은 옛 Player / Protected | `fs_submission_ledger_v1`의 value 전체 | `after-old-ledger.txt` |
| Stage Board / 같은 옛 Player의 행 | Player ID·score·metadata 원문 | `after-stage.txt` |
| Infinite Board / 같은 옛 Player의 행 | Player ID·score·metadata 원문 | `after-infinite.txt` |

3. 표의 `accountId`는 **after-player.txt에 복사한 value 안의 accountId 값**이다. 그 문자열을 복사해 검색어 뒤에 붙이면 된다. `<accountId>`라는 글자 자체를 입력하지 않는다.
4. 각 항목이 없거나 읽기 오류가 나면 빈 JSON을 만들어 넣지 말고 중단해 알려준다. Board 행이 실제로 없는 경우만 B와 같이 `행 없음 확인`이라고 적는다.
5. 폴더 경로와 새 파일 이름들만 알려준다. **AI가 전환 전 자료와 비교한다.** 첫 전환의 기대 결과는 `PASS / LEGACY_CUTOVER_ROWS_AND_LEDGER_PRESERVED`다. AI가 확인하기 전에는 다음 재시작 단계로 넘어가지 않는다.

**두 번째: 재시작 뒤 같은 자료 보관**

6. 첫 비교가 PASS이면 Play를 중지하고 Unity Editor를 완전히 종료·재실행한다.
7. C의 **2~8번**을 같은 Legacy로 다시 수행한다. 결과는 다시 Ready여야 한다. 새 기록 제출과 이전 버튼은 누르지 않는다.
8. 위 표의 자료를 다시 복사하되, 파일 이름 앞의 `after-`를 **`restarted-`**로 바꿔 저장한다. 예: `restarted-account.txt`. 기존 before/after 파일은 덮어쓰지 않는다.
9. 민감 정보 제외 Ready 결과와 새 파일 이름들을 알려준다. AI가 **전환 전→재시작 후**, **전환 직후→재시작 후**를 비교한다.

기대 결과는 `PASS / LEGACY_CUTOVER_ROWS_AND_LEDGER_PRESERVED`와 `PASS / LEGACY_RESTART_PRESERVED`다. 점수·번호·시간·제출 이력은 AI가 비교하므로 사람이 계산하지 않는다. 다른 계정 때문에 순위만 바뀌는 것은 실패가 아니다.

#### 완료 판정과 보고

사용자가 할 일은 **옛 계정 확인 → Dashboard 원문을 파일로 보관 → Legacy 계정 확인 버튼 실행 → 전환 후/재시작 후 원문 보관**이다. 파일 조립·자료 유효성 검사·숫자 비교·Node 실행은 AI가 수행한다.

최초 사례 조건, 전환 전 기준, 전환·재시작 Ready, 세 번의 보존 비교가 모두 확인되면 Step 10-2를 완료 처리한다. 없어진 옛 계정이나 이미 전환된 A/B를 새 계정으로 대체하지 않는다. 실제 서버 CAS/경합·이전 Secret·계정 이전 검증은 Step 10-3/12에 남긴다.

보고에는 **A의 두 답변**, **파일 폴더/이름**, **민감 정보 제외 결과**, **확인 시각(KST)**, **예상 밖 Error/Warning 유무**만 넣는다. 파일 본문·Player ID·accountId·공개 번호·토큰은 붙이지 않는다. **지금은 A의 두 답변만 보내면 된다.**

### Step 10-3. A/B 이전·고정 행·서비스 복구 경계를 확인한다

**작업 상태: 준비 중 / 실제 이전 미실행.** Step 10-2는 사용자 승인 N/A이며, Step 10-1에서 만든 기존 A/B와 저장된 두 기준 파일을 그대로 사용한다. 계정/Save를 초기화하거나 신규 Stage 제출을 다시 수행하지 않는다.

#### 10-3-A. 먼저 A를 준비하고, 아직 이전은 시작하지 않는다

**확인 완료 — 2026-10-05:** 사용자 A 격리 예약/준비/Profile/저장 분리와 최초 WINDOW_TIMEOUT을 보고했다. 명시적 Refresh 결과(2026-10-05 05:29:23Z)는 Ready/Reason=None/Consent=True/Pending=0/LocalSave=Ready/원격 허용/인증 Complete이며 panel/window/Module timeout은 모두 5000ms였다. 기존 A/B 기준 파일이 그대로 있음도 확인했다. 최초 Timeout은 성공으로 처리하지 않고 Refresh 성공과 구분한다. 문자열의 TimeoutMs는 설정값이며 실제 경과 시간 실측 보고로 간주하지 않는다. 아래 A 준비를 반복하지 않고 10-3-B로 진행한다.

**지금은 아래 1~6번만 수행하고 결과를 알려준다.** 이전을 시작하면 A/B 연결이 실제로 바뀌므로 기준 자료를 확인한 다음 안내에 따라 진행한다.

1. 현재 요청이 끝났으면 Play를 중지하고 Unity Editor를 완전히 종료·재실행한다. 다른 게임 앱은 종료한다. 계정/저장/기준 파일은 지우지 않는다.
2. **Flow State > Online Record Verification**을 연다. **Prototype 8 Phase 2** 켬, **격리 세션 A**를 선택한다.
3. **Play 전 격리 실행 예약 (원격 호출 없음)**을 누른 뒤 Unity 상단 **Play**를 직접 누른다.
4. **격리 세션 준비 (로컬만)**를 누른다. Profile은 `flow-state-phase2-a`, 저장 경로 끝은 `Prototype8Verification/A/flow-state-save.json`이어야 한다.
5. **verification 원격 요청·테스트 기록 변경 허용 > 계정 패널 열기 / 상태 확인**을 실행한다. 기대는 **State=Ready / Reason=None / Pending=0**이다. `WINDOW_TIMEOUT`/Error이면 중단하고 **전체 메시지 복사 (민감 정보 제외)**를 보낸다. Timeout을 서버 취소로 간주하지 않는다.
6. Ready이면 **전체 메시지 복사 (민감 정보 제외)**로 결과를 보내고, 기존 `step10-1-a-baseline.json`과 `step10-1-b-baseline.json` 파일이 그대로 있는지도 알려준다. 파일 내용·번호·ID는 보내지 않는다.

**아직 누르지 않을 버튼:** Start, Reissue, CancelTransfer, OpenInput, Complete, 신규/일반 Stage 제출, Pending 폐기. Pending이 0이 아니면 임의 삭제하지 말고 그 상태를 알린다.

#### 10-3-B. 이전 전 A의 실제 계정·기록을 파일에 보관한다

**확인 완료 — 2026-10-05:** 사용자가 Ignore/Step10-3의 before-account-A.txt / before-stage-A.txt / before-infinite-A.txt를 제공했다. 비공개 내용 출력 없이 실제 Account scope/Active/revision·onlineOperation 없음·원래 활성 Player·고정 Stage owner·안전한 점수/metadata·기존 A 공개 번호/본인 점수/수락 시각 일치·Infinite 행 없음의 10개 검사를 통과했다. **PASS / TRANSFER_BEFORE_BASELINE_VERIFIED**이며 원본 txt는 수정하지 않았다. Git ignore 적용을 확인한 같은 폴더에 AI가 **transfer-before.json**을 조립했고 실제 내부 번호/ID/원문은 이 문서에 기록하지 않는다. 이후 이전 후 같은 기준 파일로 compare-service-snapshots transfer를 실행한다. 실제 이전 성공/Secret 접근 근거는 아직 없다.

**A 세션을 그대로 유지한다. 이 제목의 B는 절차 번호이며, 격리 세션 B로 바꾸라는 뜻이 아니다.** 아직 Start를 누르지 않는다. Dashboard 조회만 하므로 새 빌드/게시/Scene 작업은 필요 없다.

1. 파일 탐색기에서 문서 폴더 등에 **Step10-3** 폴더를 만든다. 기존 A/B 기준 파일은 그대로 둔다.
2. Unity Dashboard에서 기존 Project와 **verification** 환경을 선택한다. **Cloud Save > Game Data > Private**에서 `fs8-account-` 뒤에 **A의 accountId**를 붙여 검색한다. A는 앞서 기준 파일로 확인한 첫 공개 번호의 계정이며, 세 번째는 B다. 숫자/ID를 새로 추측하지 않는다.
3. 검색한 Custom ID의 **fs_account_v1** 항목에서 **value 전체**를 복사한다. 메모장에 붙여넣고 Step10-3 폴더에 **before-account-A.txt**로 저장한다. Dashboard의 Edit/Save/Delete/Create는 누르지 않는다. 항목을 찾지 못하면 빈 값으로 대체하지 말고 멈춰 알려준다.
4. 복사한 value에서 **leaderboardOwnerId** 값을 찾는다. 이 문자열을 복사한다. **Leaderboard의 Player ID 검색에는 이 값을 사용한다. currentPlayerId나 accountId가 아니다.** 원래 로그인 Player ID와 기록 소유자 ID가 다른 것이 정상일 수 있으므로 같게 고치지 않는다.
5. **Leaderboards > fs-stage-stage-001-r1**에서 4번의 문자열과 같은 Player ID의 행을 찾는다. **Player ID·score·metadata 전체**를 원문으로 복사해 **before-stage-A.txt**로 저장한다. JSON 보기와 표가 나뉘어 있으면 항목 이름과 값을 그대로 붙여넣어도 된다. 사람이 JSON 문법을 새로 작성하거나 시간 값을 계산하지 않는다.
6. **Leaderboards > fs-infinite-v2**에서도 같은 문자열로 찾아 **before-infinite-A.txt**로 저장한다. 해당 owner의 행이 실제로 없으면 파일에 **행 없음 확인**이라고만 적는다. 검색 오류/metadata를 못 찾음은 행 없음이 아니므로 중단해 알려준다. Stage는 Step 10-1의 기록이 있어야 하므로 없으면 진행하지 않는다.
7. 여기서 멈추고 **폴더 전체 경로와 다음 세 파일 이름만** 알려준다. 본문·Player ID·accountId·번호는 채팅에 붙이지 않는다.

| 파일 | 내용 |
| --- | --- |
| before-account-A.txt | A의 Private Account value 전체 |
| before-stage-A.txt | A 고정 owner의 Stage 행 원문·metadata |
| before-infinite-A.txt | 같은 owner의 Infinite 행 원문·metadata 또는 확인된 행 없음 |

AI가 파일을 읽어 scope/Active 상태/계정·owner/기준 번호·Stage 최고와 수락 시각/metadata를 검사하고 이후 이전 비교용 기준을 조립한다. 기존 A 공개 본인 기준과 불일치하면 Start 전에 원인을 확인한다. **AI의 확인을 받은 뒤에만 Start/재발급/취소/완료 단계로 넘어간다.** 점수·ID·시간 보존 판정과 Node 실행은 AI가 수행한다.

#### 10-3-C. A에서 Start를 한 번 실행한다

**확인 완료 — 2026-10-05:** 사용자 Start 결과 UTC 2026-10-05 05:53:04Z는 TransferPending/Reason=None/Pending=0, StartAccountTransfer/ResponseReceived/SDKErrorCode=0이며 timeout 설정은 모두 5000ms다. 발급 화면·코드 형식·9자리 인증값 표시와 만료 UTC 2027-01-03 05:53:04Z도 보고했다. 원문은 문서/파일에 복사하지 않는다. 이 결과는 Start 측 Secret 접근·발급 경로의 실제 실행 근거지만 B 완료/HMAC 검증·A 조회/제출 잠금·코드 무효화/CAS 원자성까지 입증하지 않는다. 코드/인증값이 채팅에 포함돼 **노출된 값은 B에 사용하지 않고 10-3-D Reissue로 교체**한다. 아직 실제 재발급/취소/이전 완료는 미확인이다.

**이전 전 기준 검사가 PASS일 때만 진행한다. 지금은 Start 한 번의 결과까지만 확인한다.**

1. 현재 검증 창의 **세션 A / State=Ready / Pending=0**을 확인한다. Play/창을 닫았다면 10-3-A와 같은 A/같은 저장 파일로 예약·Play·준비·원격 허용·계정 확인을 다시 진행한다. 새 프로필/새 Stage 기록을 만들지 않는다.
2. 열린 계정 패널의 **Start** 버튼을 **한 번** 누른다.
3. 성공 시 **State=TransferPending**과 발급 화면이 나타나야 한다. 코드 형식은 영문/숫자 `XXXX-XXXX`, 인증값은 9자리 숫자다. **값 자체는 채팅·로그·기준 파일에 보관하지 않는다.** 값 형식/발급 화면 유무만 확인하고 **전체 메시지 복사 (민감 정보 제외)**를 보낸다.
4. 성공/실패와 관계없이 여기서 멈춘다. **Reissue/CancelTransfer/Complete/새 기록 제출/B 전환은 아직 하지 않는다.** 다음 재발급/취소·연결 검증 순서는 결과 확인 뒤 안내한다.
5. WINDOW_TIMEOUT/Error는 서버 이전 요청이 실패/취소됐다는 뜻이 아니다. 새 Start를 반복하거나 transfer/계정/저장을 삭제하지 말고 안전한 결과를 보낸다. 서버 상태 확인/필요한 명시적 Refresh·원문을 잃었을 때 재발급은 그 결과를 바탕으로 따로 안내한다.

#### 10-3-D. 노출된 이전 원문을 Reissue로 교체한다

**확인 완료 — 2026-10-05:** 사용자는 ReissueAccountTransfer/ResponseReceived/SDKErrorCode=0, TransferPending/Reason=None/Pending=0을 보고했다(요청 종료 UTC 05:59:38Z, 전체 복사 05:59:50Z). 새 원문은 보고에 포함하지 않았다. 보고된 만료 UTC 2027-01-03 05:53:04Z는 최초 발급과 같아 표시된 만료 유지 검증을 확인했다. Reissue 응답 성공을 기록하지만 옛 원문을 실제 B에 넣어 거부를 관찰한 것으로 기록하지 않는다. 원문/9자리 인증값은 저장하지 않는다. 다음은 동일 A의 취소→명시적 Refresh이며 아직 B 완료는 미실행이다.

1. **같은 A / State=TransferPending / Pending=0**을 유지한다. B로 전환하지 않는다. Play/창을 닫았다면 같은 A를 다시 준비하고 Refresh로 TransferPending을 확인한다. 새 Start를 실행하지 않는다.
2. 계정 패널의 **Reissue**를 **한 번** 누른다. 실제 server는 credentialRevision을 올리고 원래 code/HMAC를 무효화한 뒤 새 값을 활성화하며 최초 만료 시각을 유지한다. 기존 값 삭제나 Secret/계정 초기화는 하지 않는다.
3. 성공하면 새 발급 화면과 **State=TransferPending**이어야 한다. **전체 메시지 복사 (민감 정보 제외)** 결과와 **만료(UTC) 날짜·시각만 별도**로 보고한다. 기대 만료는 최초 요청과 같은 2027-01-03 05:53:04Z다. 정확한 ms/90일 경계는 자동 Test와 구분하며 사용자가 계산하지 않는다.
4. **새 코드/9자리 인증값은 보내지 않는다.** 새 값은 사용자 화면에서만 확인하고 이전 입력 외 로그/기준 파일/채팅에 남기지 않는다. 이미 채팅에 나온 옛 값으로 Complete를 시험하지 않는다. 실제 서버의 원문 무효화 관찰·별도 취소/잠금·B 연결 검증은 다음 안내로 분리한다.
5. Timeout/Error이면 Reissue/Start를 반복하지 않고 민감 정보 제외 결과를 보낸다. 원래 값이 이미 무효화됐거나 새 값만 발급됐을 수 있으므로 어느 쪽이 유효하다고 추정하지 않는다. B 연결/Complete/새 제출/취소는 아직 하지 않는다.

#### 10-3-E. 같은 A에서 취소한 뒤 활성 상태를 재확인한다

**확인 완료 — 2026-10-05:** CancelAccountTransfer/ResponseReceived/SDKErrorCode=0(종료 UTC 06:06:00Z) 뒤 NotRequested/Reason=None/Pending=0과 `Transfer cancelled. The existing connection is unchanged.`를 보고했다. 이어 명시적 Refresh(종료 UTC 06:06:46Z)의 GetPublicPlayerNumber/ResponseReceived에서 Ready/Reason=None/Pending=0·로컬 저장/인증 정상과 timeout 설정 5000ms를 확인했다. 취소와 A 준비 복구는 성공했지만 이 메시지만으로 실제 고정 행/metadata 보존·옛 코드 거부 관찰·원자성까지 입증하지 않는다. 취소된 원문은 B에 사용하지 않는다. 다음은 10-3-F의 기존 A 공개 기준 비교이고 새 Start/B 연결은 아직 미실행이다.

1. **같은 A / State=TransferPending / Pending=0**에서 **CancelTransfer**를 **한 번** 누른다. 다른 세션으로 바꾸거나 새 Start를 누르지 않는다.
2. 종료 후 **전체 메시지 복사 (민감 정보 제외)** 결과를 보관한다. 정상 완료라면 CancelAccountTransfer/ResponseReceived이며 TransferPending이 해제되고 코드/인증값 화면이 사라진다. 현재 coordinator는 취소의 Active 응답을 **State=NotRequested / Reason=None**으로 표시할 수 있다. 번호 재조회 전 상태이므로 그것만으로 실패로 처리하지 않는다.
3. 정상 취소 뒤 **Refresh**를 **한 번** 누른다. 기대는 **State=Ready / Reason=None / Pending=0**이다. 새 기록을 제출하거나 계정을 초기화하지 않는다.
4. CancelTransfer와 Refresh의 **민감 정보 제외 결과 두 개**를 보고한다. 첫 CancelTransfer가 WINDOW_TIMEOUT/Error이면 반복 취소나 새 Start를 하지 않고 그 결과부터 보고한다. timeout이 서버 취소를 되돌리는 것은 아니며 처리가 일부 반영됐을 수 있다.
5. 취소한 요청의 발급 원문은 이제 B에 사용하지 않는다. 성공/원래 A 기록 보존 확인 뒤 새로운 Start로 별도의 실제 연결 사례를 준비한다. **지금은 B 전환/Complete/새 제출을 하지 않는다.**

#### 10-3-F. 취소 뒤 A의 기존 번호·Stage 기록을 비교한다

**확인 완료 — 2026-10-05:** 사용자 UTC 06:17:42Z 보고에서 같은 A Ready/Reason=None/Pending=0·QueryRecords ResponseReceived와 **PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**를 확인했다. 취소 뒤 A 공개 번호·본인 Stage 점수·수락 시각 보존의 실제 결과다. 순위/내부 owner·binding/CAS/실제 코드 무효화 관찰까지 입증한 것으로 확대하지 않는다. 기존 transfer-before.json은 유지하며 다음은 별도 새 요청의 10-3-G다.

1. 현재 **같은 A / State=Ready / Pending=0**을 유지한다. **Infinite (해제: Stage)** 체크를 끈다.
2. 로컬 **step10-1-a-baseline.json**을 메모장으로 연다. 전체 내용을 복사해 검증 창의 **비교할 기준 JSON (로컬 전용)** 칸에 붙여넣는다. **채팅에 붙이지 않는다.** B 기준이나 transfer-before.json/Local Save를 넣지 않는다.
3. **기준 자료와 번호·점수·수락 시각 비교**를 한 번 누른다. 기대 결과는 **PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 기존 번호·본인 최고 점수·수락 시각은 도구가 검사하므로 사람이 값을 계산하지 않는다.
4. **전체 메시지 복사 (민감 정보 제외)** 결과를 보내고 멈춘다. Timeout/FAIL이면 새 Start/재제출/기준 덮어쓰기를 하지 않는다. PASS 확인 뒤 새로운 Start로 B 연결용 요청을 따로 만든다.
5. **아직 누르지 않을 버튼:** 신규/일반 Stage 제출, Start/Reissue/CancelTransfer/OpenInput/Complete, Pending 폐기. 이번 공개 비교는 취소 뒤 공개 기준 보존을 확인하는 것이며, 내부 고정 owner와 source/target binding의 이전 전후 비교는 이후 B 완료 뒤 기존 transfer-before 기준으로 수행한다.

#### 10-3-G. B 연결용 새 이전 요청을 만든다

**확인 완료 — 2026-10-05:** 사용자 새 StartAccountTransfer/ResponseReceived/SDKErrorCode=0(종료 UTC 06:27:22Z, 전체 복사 06:27:24Z), A TransferPending/Reason=None/Pending=0과 새 요청 만료 UTC 2027-01-03 06:27:22Z를 확인했다. 앞선 취소 사례와 다른 새 요청이며 새 원문은 보고에 없었다. 만료가 새 발급 기준으로 바뀌는 것은 정상이고 이전 Reissue의 만료 유지 조건과 구분한다. 원문은 AI가 저장하지 않으며 실제 B 연결/HMAC 검증은 아직 미실행이다. 다음은 10-3-H B 준비만 진행한다.

1. 현재 같은 **A / Ready / Pending=0**에서 **Start**를 **한 번** 누른다. 앞선 취소 사례와 별개의 새 요청이다. 새 Stage 제출/새 계정/기준 덮어쓰기를 하지 않는다.
2. 성공 시 **TransferPending**과 새 발급 화면이 나타나야 한다. **전체 메시지 복사 (민감 정보 제외)** 결과와 **만료(UTC) 값만** 보고한다. 새 요청이므로 만료는 앞선 취소 사례의 만료보다 뒤로 바뀔 수 있다. 재발급의 만료 유지 조건과 혼동하지 않는다.
3. 이 새 코드/9자리 인증값은 이후 B 입력에 필요하다. **Editor 종료·재시작으로 발급 원문 화면이 사라지기 전에**, 사용자만 볼 수 있는 종이에 잠시 적어둔다. 채팅/스크린샷 공유/로그/repo/기준 JSON에 넣지 않는다. 직접 B 입력에만 사용하고 완료 후 임시 메모는 폐기한다. AI는 이 원문을 받거나 저장하지 않는다.
4. 보고 후 여기서 멈춘다. **아직 B로 바꾸거나 OpenInput/Complete/Reissue/CancelTransfer/새 제출을 누르지 않는다.** 새 Start 결과를 확인한 뒤 B 준비/완료를 단계별로 안내한다.
5. Timeout/Error이면 새 Start를 반복하거나 취소/원문 복원을 시도하지 않는다. 아직 원문이 표시되지 않았더라도 서버 요청이 만들어졌을 수 있으므로 민감 정보 제외 결과부터 보고한다. 이미 취소된 첫 사례의 원문은 절대 B에 사용하지 않는다.

#### 10-3-H. Editor를 재시작해 기존 B를 준비한다

**확인 완료 — 2026-10-05:** 사용자 B 계정 확인(종료 UTC 06:30:12Z, 전체 복사 06:30:42Z)에서 `flow-state-phase2-b`, Ready/Reason=None/Consent=True/Pending=0/LocalSave=Ready/원격 허용·인증 Complete·GetPublicPlayerNumber/ResponseReceived/SDKErrorCode=0을 확인했다. 아직 이전 완료 응답은 없으며 이 결과는 기존 B 준비 확인이다. 다음은 10-3-I의 마지막 새 원문 입력/Complete 한 번이고 새 기록 제출은 하지 않는다.

**지금은 B의 계정 상태 확인까지만 수행한다. 아직 이전 코드를 입력하거나 Complete를 누르지 않는다.**

1. 사용자만 볼 수 있는 임시 종이 메모에 **10-3-G의 마지막 새 코드/9자리 인증값**을 보관했는지 확인한다. 최초 노출/재발급/취소 사례의 값과 혼동하지 않는다. 새 값을 잃었다면 계정/코드 원문 복원을 시도하지 말고 그 상태만 알려준다.
2. A의 요청이 끝나면 **Play 중지 > Unity Editor 완전 종료 > 프로젝트 다시 열기**를 수행한다. 같은 Editor 프로세스에서 A/B를 바로 전환하지 않는다. A/B Save·기준 파일을 지우거나 덮어쓰지 않는다.
3. **Flow State > Online Record Verification > Prototype 8 Phase 2 켬 > 격리 세션 B**를 선택한다.
4. **Play 전 격리 실행 예약 > Unity 상단 Play > 격리 세션 준비 (로컬만)**를 수행한다. Profile은 `flow-state-phase2-b`, 경로 끝은 `Prototype8Verification/B/flow-state-save.json`이어야 한다.
5. **verification 원격 요청·테스트 기록 변경 허용 > 계정 패널 열기 / 상태 확인**을 수행한다. 기대는 **State=Ready / Reason=None / Consent=True / Pending=0**이다. 기존 B의 동의/로그인/저장을 유지하며 새 Anonymous로 대체하지 않는다.
6. **전체 메시지 복사 (민감 정보 제외)** 결과를 보내고 멈춘다. WINDOW_TIMEOUT/Error이면 Complete/새 Start/계정 초기화를 하지 말고 그 결과부터 보고한다. Pending이 남아 있으면 임의 폐기하지 않는다.

**아직 누르지 않을 버튼:** OpenInput, Complete, Start, Reissue, CancelTransfer, 신규/일반 Stage 제출, Pending 폐기. 이후 AI가 B 준비 결과를 확인한 뒤 마지막 유효 원문 입력/완료를 안내한다. 코드/9자리 인증값 자체는 어떤 결과 보고에도 넣지 않는다.

#### 10-3-I. B에서 마지막 새 원문을 입력해 Complete를 한 번 실행한다

**2026-10-05 사용자 완료 실행·B Refresh 결과:** 사용자는 Complete를 실행했으나 직후 결과를 복사하지 않았다고 보고했다. Complete의 status/시각/endpoint 최종 결과는 추정하지 않는다. 이후 같은 B Refresh(종료 UTC 06:48:19Z, 전체 복사 06:48:41Z)는 Ready/Reason=None/Pending=0·인증/저장 정상·GetPublicPlayerNumber ResponseReceived와 `Transfer confirmed. Server personal bests have been applied.`를 보고했다. B 공개 번호가 기존 A 번호로 바뀌었다는 사용자 관찰도 확인했다. 이는 B의 이후 인증 상태/완료 반영 관찰 근거이며 Complete 직후 응답을 확보한 것으로 기록하지 않는다. **응답 확보를 위해 Complete를 재실행하지 않는다.** 다음은 B에서 기존 A 공개 본인 기준과 비교(10-3-J), 이어 내부 source/target binding·고정 행 비교이며 실제 비교 전 Step 10-3은 미완료다. A 일반 패널/Refresh나 후속 새 제출은 아직 하지 않는다.

1. 현재 **같은 B / Ready / Pending=0**을 유지하고 계정 패널의 **OpenInput**을 누른다.
2. **이전 코드** 칸에 10-3-G의 **UTC 2026-10-05 06:27:22Z 새 Start에서 받은 마지막 코드**를 입력한다. 최초 노출/재발급/취소 사례의 코드를 사용하지 않는다.
3. **9자리 인증값** 칸에 그 새 코드와 함께 발급된 값을 입력한다. 공개 번호가 아니며 앞의 0도 빼지 않는다. 코드/인증값을 채팅·스크린샷 공유·로그·기준 JSON에 넣지 않는다.
4. **Complete**를 **한 번** 누른다. 이 요청은 A의 논리 계정 연결을 B로 실제 이전하고, B의 로컬 온라인 최고 기록을 A 계정의 서버 최고로 반영한다. 원래 B의 최고와 합치거나 수동으로 삭제하지 않는다.
5. 종료 뒤 **전체 메시지 복사 (민감 정보 제외)** 결과를 보내고 멈춘다. 정상 완료에서는 완료 안내와 Ready/Reason=None/Pending=0이 기대되며, 내부 후속 번호/최고 조회 때문에 마지막 Function이 GetPublicPlayerNumber로 표시될 수 있다. 숫자 자체는 보내지 않는다. 최종 성공 판정은 다음 A 기준 비교/내부 binding·고정 행 비교까지 구분한다.
6. WINDOW_TIMEOUT/Error이면 성공/실패나 서버 rollback을 추정하지 않는다. Complete/Start/Reissue/CancelTransfer를 반복하지 말고 안전한 결과부터 보낸다. 서버는 이미 B 연결까지 반영됐을 수 있으며 필요한 Refresh/완료 복구는 결과에 따라 별도 안내한다. 임시 원문 메모는 복구가 필요할 경우 사용자만 보관하고, 성공 확인 뒤 폐기한다.
7. **아직 하지 않을 작업:** 신규/일반 Stage·Infinite 기록 제출, B 기준 캡처로 기존 A 기준 덮어쓰기, A 일반 패널/Refresh 실행, 임의 token/Save/계정 초기화. A의 원래 세션 거부 관찰은 복구 전에 수행해야 하므로 순서를 따로 안내한다.

#### 10-3-J. B에서 A의 이전 전 공개 기준과 비교한다

**2026-10-05 후속 캡처 실패·대체 자료 확보:** 첨부 보고에서 현재 캡처는 UTC 07:06:33Z WINDOW_TIMEOUT, 이어 ACCOUNT_NOT_READY, B Refresh 종료 07:06:57Z Ready 복구, 다시 07:07:25Z WINDOW_TIMEOUT이었다. Query timed out 경고 2개, SDK Invocation 422/InvalidOperationException ServiceUnavailable·늦은 SDKErrorCode=9009도 보고했다. 요청한 현재 공개 baseline 파일은 로컬에서 확인되지 않았다. 이전 06:53:23Z의 공개 비교 불일치와 이후 캡처 timeout은 별개로 기록한다. SDKErrorCode와 서버의 공통 예외 변환만으로 원인을 확정하지 않는다.

코드 검사에서 캡처는 `_account.TryAuthenticateAsync`→`_records.GetPersonalBestAsync` 내부의 동일 account 인증→캡처의 RetryPublicNumberAsync를 거치며, Completed 상태에는 서버 최고/번호 복구 조회도 반복될 수 있다. 전체 5초를 공유하는 동안 여러 서버 요청이 누적되는 구조는 확인했으나 각 원격 요청의 실측 시간/실패 내부 단계는 미확인이다. **캡처 반복/5초 연장/새 제출/이전 mutation/기준 덮어쓰기/A 일반 Refresh는 하지 않는다.** 사용자에게 추가 컴파일·게시 반복을 요구하는 대신 원래 계획한 이전 후 내부 자료를 10-3-K로 확보해 실제 계정/고정 행 보존부터 비교한다. 이번 턴은 원인 검사와 문서 갱신뿐이며 C#/Module/Scene 변경·빌드/Test Runner/AI 원격 호출은 하지 않았다. 공개 비교 실패·캡처 시간 제한/422 문제는 해결됐다고 기록하지 않고 Step 10-3을 미완료로 유지한다.

**2026-10-05 실패 보고·이후 작업 중지:** 사용자 UTC 06:53:23Z는 B Ready/Reason=None/Pending=0·QueryRecords ResponseReceived 뒤 **FAIL / 기준·현재 공개 행 불일치**였다. 이 문구는 current IsValidMe 검사 이후 ArePreserved 실패 분기이며 scope/Board/번호/행 개수·score/acceptedAt 중 어느 차이인지는 미확정이다. Complete/새 제출/기준 덮어쓰기/A 일반 Refresh를 반복하지 않는다. AI가 기존 로컬 transfer-before와 실제 A baseline 파일을 다시 읽어 scope·Stage/본인 행 1개·번호·점수·수락 시각 일치를 확인했다. 실제 창에 붙인 입력 JSON과 현재 B 원격 me 행은 이 로컬 검사로 확인할 수 없으며 서버 기록 손상/이전 실패로 단정하지 않는다.

**다음 사용자 작업은 읽기/기준 내보내기만:** 같은 B Ready/Pending=0에서 Infinite 체크 끔 → **내 최고 기준 저장 / 재조회 번호 비교** 한 번 → PASS일 때 **기준 자료를 사용자 파일로 저장**으로 `Ignore/Step10-3/step10-3-b-current-public.json`에 새 파일로 저장 → 전체 로컬 경로와 민감 정보 제외 결과만 전달한다. 원래 A/B baseline·transfer-before·Local Save는 덮어쓰지 않는다. 이 버튼은 현재 조회를 캡처하는 용도이며 B→A 보존 검증 성공을 의미하지 않는다. Timeout/FAIL이면 반복하거나 빈 파일을 만들지 말고 안전한 결과부터 보고한다. AI가 현재 파일의 실제 Board/scope/번호/행·점수/수락 시각을 원래 A 기준과 비교해 값 자체를 출력하지 않고 mismatch 종류만 판정한다. 이번 조사에 C#/Module/Scene 변경·빌드·Test Runner·게시·AI 원격 호출은 없다. Step 10-3-J/전체 Step 10-3은 미완료다.

**위 캡처 안내와 아래 1~6은 이전 안내의 실행 이력이다. 현재는 실패가 보고되었으므로 재실행하지 않고 10-3-K만 수행한다.**

1. 현재 **같은 B / Ready / Pending=0**을 유지한다. **Infinite (해제: Stage)** 체크를 끈다. A 세션으로 바꾸지 않는다.
2. **step10-1-a-baseline.json**을 메모장으로 연다. **A 파일**이다. B 파일이나 transfer-before.json/Local Save를 넣지 않는다.
3. 파일 전체를 검증 창 **비교할 기준 JSON (로컬 전용)**에 붙여넣는다. 채팅/결과 보고에는 본문을 붙이지 않는다.
4. **기준 자료와 번호·점수·수락 시각 비교**를 **한 번** 누른다. 기대는 **PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. B의 인증으로 A 계정의 공개 번호·본인 Stage 행·점수·수락 시각이 이어졌는지 도구가 비교한다. 순위는 비교 대상이 아니다.
5. **전체 메시지 복사 (민감 정보 제외)** 결과를 보내고 멈춘다. Timeout/FAIL이면 새 기록 제출/기준 덮어쓰기/완료 반복으로 해결하지 않는다.
6. **아직 하지 않을 작업:** Start/Reissue/CancelTransfer/Complete, 신규/일반 Stage·Infinite 제출, 기준 파일 덮어쓰기, A 일반 패널/Refresh. 내부 고정 owner/계정 연결 비교와 원래 A 거부 관찰을 먼저 안내한다. B가 정상 완료 반영 상태이고 새 원문 복구가 필요 없으면 사용자 임시 코드·인증값 메모는 폐기하며 AI에게 보내지 않는다.

#### 10-3-K. 캡처를 반복하지 않고 Dashboard의 이전 후 자료를 보관한다

**2026-10-05 저장 자료 정적 비교 완료:** 사용자가 저장한 after 원문 5개를 읽어 기존 transfer-before.json과 대조하고, 원본을 수정하지 않은 채 Git 제외 `Ignore/Step10-3/transfer-after.json`을 조립했다. 파일 읽기 재검증과 기존 `compare-service-snapshots.cjs transfer` 실행 결과는 **PASS / TRANSFER_FIXED_ROW_BINDING_PRESERVED**다. 동일 Project/verification·Active 논리 계정/공개 번호/고정 owner 유지·활성 Player 교체·connectionRevision 정확히 1 증가·원래 A binding Inactive/현재 B binding Active·같은 고정 Stage 행의 score/acceptedAt/submissionId 보존·Infinite 행 전후 없음·onlineOperation 없음을 확인했다. 추가로 transfer Completed/credentialActive=false·완료 Player/버전 일치·source binding의 같은 transfer 표식을 확인했다. 이는 저장된 Dashboard 자료의 비교 근거이며 서버 다중 항목 원자성, 실제 재사용 요청 거부 또는 UI 공개 조회 성공까지 입증하지 않는다. 기존 공개 비교 FAIL·캡처 timeout/422는 미해결로 유지한다. 다음은 10-3-L만 수행하며 후속 제출은 아직 하지 않는다.

**이 단계는 Dashboard 조회·원문 복사만 한다. B 창의 캡처/비교·Complete/새 기록 제출은 반복하지 않는다. A로 바꾸거나 일반 계정 패널/Refresh도 누르지 않는다.**

1. Dashboard에서 같은 Project와 **verification**을 선택한다. 이전에 사용한 **Ignore/Step10-3** 폴더에 아래 **새 파일 5개**를 저장한다. before 파일과 transfer-before.json은 덮어쓰지 않는다.
2. **Cloud Save > Game Data > Private**에서 이전에 읽었던 **A의 같은 accountId**에 대한 `fs8-account-<A accountId>`를 찾는다. B의 옛 accountId로 바꾸지 않는다. `fs_account_v1`의 **value 전체**를 복사해 **after-account.txt**로 저장한다.
3. **원래 A의 로그인 Player ID**는 기존 **before-account-A.txt의 currentPlayerId**다. 그 값을 복사해 Private Game Data에서 `fs8-player-<그 값>`을 찾는다. `fs_account_v1`의 value 전체를 **after-source-binding.txt**로 저장한다. 기대 Inactive 등의 판정은 AI가 한다.
4. **이전 후 활성 Player ID**는 방금 저장한 **after-account.txt의 currentPlayerId**다. 그 값을 복사해 `fs8-player-<그 값>`을 찾는다. `fs_account_v1`의 value 전체를 **after-target-binding.txt**로 저장한다. 번호나 accountId를 Player ID 대신 넣지 않는다.
5. **Leaderboard 검색에 사용할 기록 소유자**는 원래 **before-account-A.txt의 leaderboardOwnerId**다. 이전 전과 같은 문자열을 사용한다. 두 Board에서 같은 owner의 **Player ID·score·metadata 전체**를 원문으로 복사한다.

| 실제 자료 | 새 파일 이름 |
| --- | --- |
| A의 동일 논리 Account value 전체 | after-account.txt |
| 원래 A 로그인 Player의 binding value 전체 | after-source-binding.txt |
| 현재 활성 Player의 binding value 전체 | after-target-binding.txt |
| fs-stage-stage-001-r1의 원래 고정 owner 행 | after-stage.txt |
| fs-infinite-v2의 같은 원래 고정 owner 행 | after-infinite.txt |

6. Infinite 행이 실제로 없으면 **행 없음 확인**이라고 적는다. 항목/Stage를 찾지 못했거나 읽기 오류가 나면 없다고 대체하지 말고 멈춰 알려준다. Dashboard Edit/Save/Delete/Create를 누르지 않는다.
7. 같은 폴더에 저장했으면 **저장 완료**만 알려준다. 다른 위치면 폴더 경로만 알려준다. 파일 본문/번호/Player ID/계정 ID는 채팅에 붙이지 않는다.

AI가 원문을 읽어 transfer-after.json을 조립하고 기존 transfer-before와 **계정/번호/고정 owner 동일·활성 Player 교체·revision 1 증가·원래 binding Inactive/새 binding Active·Stage/Infinite 점수/submissionId/수락 시각 보존**을 비교한다. 이 실제 서비스 자료 비교가 PASS해도 앞선 창 공개 비교 실패/422가 자동 해결됐다고 간주하지 않는다. 원래 A 거부·코드 비활성·B 후속 제출/재시작·UI 시간 제한 문제는 이후 순서로 남긴다.

#### 10-3-L. 원래 A에서 온라인 접근이 막혔는지 확인한다

**2026-10-05 같은 세션 재시도도 timeout:** 사용자 UTC 08:38:58Z 결과는 동일 A/Auth Complete/SDK Initialized·GetAccountTransferStatus LateResponseReceived·FAIL/WINDOW_TIMEOUT(5000ms)이었다. 첫 인증 지연만으로 설명할 수 없고 Inactive 응답 본문/후속 QueryRecords 성공적 실행 결과는 여전히 미확보다. **아래 한 번 더 재시도 안내는 이미 수행했으므로 종료한다. 더 누르거나 일반 Refresh/새 계정·새 제출·timeout 연장으로 우회하지 않는다.** 서버 endpoint가 provisioning 유무 확인→completion recovery→lifecycle status 순서로 호출하며 같은 binding을 반복 조회하는 구조를 확인했다. 이는 누적 원격 조회 경로 근거이지 실제 지연의 원인/실측 시간 증명은 아니다. 기존 Dashboard 보존 비교는 PASS로 유지한다. 다음은 사용자 승인 후 중복 조회 축소·원래 A 관찰 결과 분리/안전한 단계 진단 등 도구 개선을 별도 수행하는 것이며 이번 보고 처리에서는 코드/서버/Scene 변경·빌드/Test Runner/AI 원격 호출 없이 Step 10-3-L/전체 Step 미완료로 기록한다.

**2026-10-05 최초 관찰 timeout 보고:** 사용자 UTC 08:12:49Z 전체 복사 결과는 기존 A 프로필·NotRequested/Consent=True/Pending=0·인증 Complete와 GetAccountTransferStatus LateResponseReceived/SDKErrorCode=0 뒤 FAIL/WINDOW_TIMEOUT(5000ms)이었다. Inactive 응답 본문과 QueryRecords 결과는 확보하지 못했으므로 원래 A 요청 거부 관찰은 미완료다. 전용 분기는 account coordinator 상태를 갱신하지 않아 NotRequested만으로 서버 활성 여부를 판단하지 않는다. 상태 응답이 늦었다는 진단은 확인했지만 SDK 초기화/인증·서버 기동/통신별 실측 원인은 미확인이다. 기존 Dashboard binding/행 비교 PASS는 유지하며 UI 시간 제한 문제가 해결됐다고 기록하지 않는다.

**이 보고 이후 다음 작업:** Editor를 재시작하거나 Play를 종료하지 않고 현재 같은 A 세션을 유지한다. 기존 인증이 완료된 상태에서 전용 **원래 인증 세션의 Inactive·조회 실패 관찰 (복구 전에)** 버튼만 **명시적으로 한 번 더** 누르고 민감 정보 제외 전체 복사를 보낸다. 이는 자동 재시도나 timeout 연장이 아니며 전용 인증 경로는 이미 SignedIn이면 다시 Anonymous SignIn을 호출하지 않는다. 일반 패널/Refresh·ConfirmConsent·새 제출·세션 삭제는 하지 않는다. 다시 timeout/FAIL이면 추가 반복하지 않고 보고한다. 읽기 관찰 결과에 따라 후속 도구 개선 필요성을 별도로 판단한다. 이번 턴은 코드 검사/결과 기록뿐이며 C#/Module/Scene 변경·AI 빌드/Test Runner/원격 호출은 없다.

**계정 패널 열기 / 상태 확인·Refresh를 먼저 누르지 않는다.** 이 버튼들은 원래 A의 비활성 연결을 확인한 뒤 새 Anonymous로 복구할 수 있어, 원래 세션의 거부 관찰과 구분해야 한다. 아래 전용 버튼은 coordinator 복구를 거치지 않고 원래 SDK 인증 세션의 상태와 조회 실패를 관찰한다. 새 계정/새 이전/기록 제출·저장 파일 삭제는 필요 없다.

1. 현재 Play를 종료하고 Unity Editor를 완전히 닫은 뒤 같은 프로젝트를 다시 연다. A/B 전환은 기존 안내대로 Editor 재시작 후 수행한다.
2. **Flow State → Online Record Verification**을 열고 **Play 전 격리 실행 예약 (원격 호출 없음)**을 누른다. Unity 상단 **Play**를 직접 누른다.
3. 창의 **격리 세션**을 **A**로 선택하고 **격리 세션 준비 (로컬만)**를 누른다. 인증 프로필이 **flow-state-phase2-a**인지 확인한다. 로컬 준비 실패면 중지하고 결과만 보고한다.
4. **verification 원격 요청·테스트 기록 변경 허용**을 체크한다. **Infinite (해제: Stage)**는 끈다. **계정 패널 열기 / 상태 확인·Refresh·ConfirmConsent는 누르지 않는다.** A는 기존 동의/기준/인증 프로필을 그대로 사용한다.
5. **원래 인증 세션의 Inactive·조회 실패 관찰 (복구 전에)**를 한 번 누른다. 기대 결과는 **OBSERVED / ORIGINAL_INACTIVE_AND_QUERY_FAILED**다. 이 분류는 원래 A 상태가 Inactive이고 본인 기록 조회가 실패했다는 관찰이며 일반 오류의 상세 거부 원인·제출 거부·원자성까지 PASS한 것은 아니다.
6. **전체 메시지 복사 (민감 정보 제외)** 결과를 보내고 멈춘다. WINDOW_TIMEOUT/FAIL이면 반복하거나 일반 Refresh/복구·새 제출로 해결하지 않는다. 다음 작업은 이 결과를 확인한 뒤 안내한다.

이 단계는 사용자 수동 인증/읽기 요청이며 AI가 원격 호출하지 않는다. 기존 도구를 그대로 사용하므로 추가 컴파일·Test Runner·Module 재게시·빌드·Scene 작업이 필요 없다. Step 10-3은 B 후속 제출/재시작·UI 오류·미확인 서비스 경계가 남아 미완료다.

#### 10-3-M. 5초 제한을 유지한 상태/조회 분리 수정 적용

**2026-10-05 분리 상태 버튼도 timeout 보고:** 사용자 UTC 09:07:02Z는 A/Auth Complete/SDK Initialized·GetAccountTransferStatus **ClientTimeout**(5000ms) 및 WINDOW_TIMEOUT이었다. 새 상태 전용 버튼이 실행됐지만 이번 embedded 서버 수정의 Module 재빌드/verification 게시 성공·버전/시각과 Unity 컴파일/Test 결과는 별도로 미보고다. 새 Client UI가 있다는 것만으로 서버 수정 게시까지 확인하지 않는다. ClientTimeout은 transport의 해당 SDK 호출 시작 후 5초 응답 경쟁이 만료된 근거이며 Inactive 응답 본문/기록 조회 거부는 여전히 미확인이다. 상태·조회 연속 실행만으로 현재 timeout을 설명하지 않는다. 더 반복하거나 조회 버튼/일반 Refresh/새 계정·제출/timeout 연장으로 우회하지 않는다. 다음은 사용자에게 이번 수정 이후 verification Module 재빌드·게시 여부/게시 시각(가능하면 version)을 확인하는 것이며 확인 없이 재게시 반복이나 추가 코드 변경은 하지 않는다. 기존 Dashboard 보존 PASS/5초 제한은 유지하고 Step 10-3은 미완료다.

**2026-10-05 사용자 승인 후 로컬 수정:** 검증 창의 기존 합쳐진 관찰 버튼을 **원래 A 상태만 확인 (복구 전에 / 제출 없음)**과 **원래 A 기록 조회 거부만 확인 (복구 전에 / 제출 없음)**으로 분리했다. 각 클릭은 기존 공통 5000ms budget 안에서 SDK 인증의 원래 Player 대조 후 해당 서버 endpoint 하나만 호출한다. 상태 버튼은 Inactive 관찰 뒤 즉시 반환하고 기록 조회를 자동 호출하지 않는다. 조회 버튼도 상태 확인/번호 조회/coordinator 복구를 호출하지 않는다. 늦은 결과 미적용/Pending 보존/원격 동의·Play 격리/자동 실행 차단은 유지한다. NotRequested는 coordinator를 사용하지 않는 전용 경로의 로컬 상태여서 서버 권한 판정이 아니다.

서버 GetAccountTransferStatus는 같은 요청에서 읽은 scoped Player binding이 Inactive이고 transferOperation이 없으면 completion recovery를 건너뛰고 lifecycle의 내부 inactiveStatus 경로로 전달한다. 다른 Player/kind/scope/Active/진행 중 operation을 전달하면 거부하며, 이 내부 함수는 게시 endpoint/Client 입력이 아니다. 완료 receipt를 대조할 때 player/receipt **2회 조회**, receipt가 없으면 authoritative Account 추가 **3회 조회**다. Active/B/부분 완료는 기존 recovery 경로를 유지하고 status(context)의 기존 호출 계약은 유지했다. 권한 확인/CAS/기록 값·5초 상수/Secret/환경/Scene/Local Save는 변경하지 않았다.

**로컬 검증:** 새 production endpoint 대역 4사례(조회 횟수/원본 불변·receipt 없는 fallback·변조/실패 폐쇄·B recovery), 기존 장애 전후 복구/경합 포함 전체 Node Test **38파일 통과**, 정적 계약/Client preflight(48 C# 파일·중복 선언/asmdef/meta/구문 경계)/변경 JS 문법·diff 공백 검사를 통과했다. C# 실행/컴파일/Test Runner 또는 .NET/Jint 빌드/원격 실측을 실행한 결과가 아니다. 원격 timeout·기존 공개 비교 FAIL/422와 Step 10-3 전체는 실제 적용 확인 전 미완료다. 아래는 **이번 수정 이후** 사용자 작업이다.

1. 현재 Play를 종료한다. Unity Script Compilation과 예상 밖 Error/Warning을 확인하고 EditMode/PlayMode 전체를 사용자가 실행한다. 결과가 실패면 게시/원격 확인으로 넘어가지 말고 실패 내용을 보낸다. Scene 작업은 없다.
2. 서버 JS가 Module에 embedded되므로 이전 Module 게시에는 이번 수정이 없다. 기존 방식으로 **verification**의 **FlowStateVerification Module 하나**를 사용자가 빌드/Deploy Selected한다. standalone JS Script 게시/bridge/Secret/정책/기록/계정 초기화는 하지 않는다. AI는 빌드·게시에 관여하지 않는다.
3. Editor를 완전히 종료·재시작한다. Online Record Verification → **Play 전 격리 실행 예약** → 상단 **Play** → **A** 선택 → **격리 세션 준비 (로컬만)** 순서로 준비한다. 인증 프로필 flow-state-phase2-a와 기존 A 저장 경로를 유지하고 **원격 요청 허용** 체크, Infinite는 끈다. 일반 계정 패널/Refresh·ConfirmConsent는 누르지 않는다.
4. **원래 A 상태만 확인 (복구 전에 / 제출 없음)**을 한 번 누른다. 기대는 **OBSERVED / ORIGINAL_INACTIVE_STATUS**다. 이 결과만으로 기록 조회까지 성공 판정하지 않는다. Timeout/FAIL이면 다음 버튼/자동 또는 수동 반복 없이 전체 안전한 메시지를 보낸다.
5. 4가 기대 결과면 **원래 A 기록 조회 거부만 확인 (복구 전에 / 제출 없음)**을 한 번 누른다. 기대 관찰은 **OBSERVED / ORIGINAL_QUERY_FAILED**이며 QueryStatus/Reason/Phase/Fault도 함께 복사된다. 일반 서비스 오류만으로 정확한 권한 거부 원인/제출 거부·원자성을 PASS하지 않는다.
6. 두 버튼 각각의 **전체 메시지 복사 (민감 정보 제외)**와 컴파일/Test/Module 게시 결과를 보낸 뒤 멈춘다. 새 기록 제출·기준 덮어쓰기·Complete 반복·A 일반 복구는 아직 하지 않는다. 이전 상태/고정 행 보존의 Dashboard 비교 PASS는 유지한다.

#### 10-3-N. Editor 재시작 후 첫 요청 지연을 분리해서 확인한다

**2026-10-05 일반 패널 실행 후 A 세션 변경 확인:** 사용자는 계정 패널 열기/상태 확인을 먼저 실행해 Ready/Reason=None/Pending=0·GetPublicPlayerNumber(RequestOrdinal3/ClientElapsedMs481)를 보고했고, 뒤의 A 상태 전용 버튼은 RequestOrdinal4/ClientElapsedMs473/ServerElapsedMs245/ServerServiceCalls7·ORIGINAL_STATUS_NOT_INACTIVE(전체 복사 UTC09:46:56Z)였다. Play는 종료됐다. AI가 현재 A Local Save를 비공개 읽기 전용으로 기존 transfer-before와 비교한 결과 scope는 같지만 onlinePlayerId와 공개 번호 모두 원래 A와 달랐다. 생산 coordinator의 Inactive source 복구는 blank binding 저장→옛 SDK 세션 정리→새 Anonymous→새 Player 저장 경로이므로 현재 A는 원래 이전 source 관찰용 인증과 구분해야 한다. 현재 실패 분류로 원래 A가 재활성화됐거나 이전이 취소됐다고 판단하지 않는다. 원래 A 직접 상태/조회 거부 근거는 확보되지 않았으며 옛 인증 복원/저장 초기화·새 이전 생성으로 재현하지 않는다. 후속 요청 473ms(서버 내부245ms)는 실제 빠른 응답 근거이나 직전 최초 timeout과 플랫폼/통신 원인 확정 비교는 아니다. GetPublicPlayerNumber의 서버 시간 -1은 그 endpoint에 timing 계측을 넣지 않은 정상 미측정이다. 다음 조사에서는 현재 A 프로필은 **복구 후 새 계정**으로 취급하고 원래 A 관찰과 첫 요청 시간 문제를 분리한다. 새 SDK 자격 증명은 읽거나 로그화하지 않았으며 코드/Scene/서버 변경·AI 빌드/Test Runner/원격 호출 없이 기존 Dashboard 고정 행/연결 보존 PASS 유지·Step 10-3 미완료다.

**2026-10-05 최초 시간 진단 보고(UTC 09:40:40Z):** 기존 A/Auth Complete·AuthElapsedMs=367, RequestOrdinal=1·GetAccountTransferStatus ClientTimeout·ClientElapsedMs=5054·ServerElapsedMs/ServerServiceCalls=-1·WINDOW_TIMEOUT이었다. 인증은 완료됐고 Module 요청 자체가 응답 제한에 걸린 근거이며, 현재 -1은 응답의 서버 측정값 미확보이지 서버 미실행/처리 시간 0/구 Module 게시의 증거가 아니다. 인증·초기화만으로 현재 지연을 설명하지 않고 late 응답의 서버 시간 확보를 다음 근거로 삼는다. 콘솔 UI State None/verification InitializationOptions 로그는 정상 정보이며 인증 Error/Warning 근거가 아니다. 설치 Cloud Code SDK는 초기화에서 retry provider 없는 HttpClient를 생성하고 request timeout 30초를 설정한다. 현행 앱의 5초 응답 경합은 SDK 전송 취소가 아니므로 이후 late 응답이 올 수 있으며, SDK의 retry 관련 코드 존재만으로 이번 요청이 자동 retry됐다고 기록하지 않는다. 다음 사용자 작업은 **현재 A/Play 유지·원격 버튼/Refresh 재실행 없이 전체 메시지만 다시 복사**하여 LateResponseReceived/LateRequestFailed 및 서버 시간 진단 변화 여부를 전달하는 것이다. 응답 종료가 확인되기 전 후속 요청을 실행하지 않는다. 이번 보고 처리에는 코드 변경/빌드/Test Runner/재게시/AI 원격 호출이 없고 Step 10-3은 미완료다.

**2026-10-05 컴파일 오류 후속 수정:** 사용자 CS0102(OnlineAccountResponse.serverTimingPresent 중복)를 확인해 동일 bool field 두 선언 중 하나를 제거했다. 기존 source audit는 메서드만 검사해 이를 놓쳤으므로 단일 field 선언의 중복 검사와 DTO 중복/초기값/중첩 타입·expression-bodied property 오탐 방지 회귀를 추가했다. Node38파일·Client preflight(C#48파일)/server source budget/중복 검사·diff 통과. Unity 실제 재컴파일은 사용자 확인 대기이며 이번 후속 수정에는 서버/Scene/계정·기록 변경이 없다. 원래 N의 서버 변경 게시 필요와 이번 client 중복 제거를 구분하고 Step 10-3은 미완료다.

**2026-10-05 사용자 확인/후속 수정:** 사용자는 이전 10-3-M의 FlowStateVerification Module 빌드·배포 성공을 확인했고, Editor 재시작 뒤 첫 서버 요청이 항상 실패하는 것 같다고 보고했다. 게시 누락을 현재 원인으로 단정하지 않는다. 첫 요청/후속 요청의 실제 시간 대조는 아직 없다. 서버가 매 invocation마다 HttpClient/연결 풀을 만들고 Dispose하는 구조를 확인해 **공유 HttpClient/5분 pooled connection lifetime**으로 수정했다. 인증 토큰은 shared DefaultRequestHeaders에 넣지 않고 invocation별 request.Headers에만 설정한다. 각 invocation의 서비스 context/CTS·공통 5초 budget·deadline HTTP/body read·권한/CAS·No explicit retry는 유지하며 요청/응답 데이터·계정·토큰 캐시는 만들지 않는다. HTTP 연결 재사용은 확인된 반복 초기화 비용을 줄이는 수정이며 실제 첫 요청 timeout 원인 확정/해소 보장은 아니다.

Client 인증 진단에는 **AuthElapsedMs**, Module 진단에는 **RequestOrdinal/ClientElapsedMs/ServerElapsedMs/ServerServiceCalls**를 추가했다. RequestOrdinal은 현재 transport instance의 순번으로 Editor 전체/서버 process의 첫 요청을 뜻하지 않는다. ServerElapsedMs는 ServerServices 시작부터 응답 생성까지라 Module 플랫폼 기동/스케줄링·왕복 통신은 포함하지 않는다. GetAccountTransferStatus 응답에 안전한 서버 시간/실제 HTTP 호출 횟수만 추가하며 serverTimingPresent가 없으면 **-1(미측정)**이다. 늦은 응답은 timing 진단만 반영하고 계정/기록/완료 상태에는 적용하지 않는다. 초기 SDK 비용·Module 초기 기동·서비스 통신 중 어느 부분인지 실측 없이 단정하지 않는다. 자동 warm-up·숨겨진 첫 요청·자동 retry·5초 연장·Scene 변경은 없다.

**로컬 검증:** 시간 DTO와 미측정 marker EditMode 1사례 추가(사용자 실행 대기), HTTP 공유/요청별 token·deadline/안전한 timing/sequence fence의 정적 계약 추가. Node Test **38파일**, server source budget/정적 계약·Client preflight(C#48파일, 준비 Edit163/Play21)·diff 검사 통과. Node/source 검사로 .NET/Jint/Unity 실행이나 실제 cold-start/동시 요청 token 분리 시간을 실증했다고 기록하지 않는다. Step 10-3 전체와 원격 timeout 해결은 미완료다. 이전 Module 게시 성공은 이번 N의 새 C# 서버 수정까지 포함하지 않는다.

1. 사용자가 Play 종료 후 이번 Client 수정의 Unity 컴파일·예상 밖 Error/Warning과 전체 EditMode/PlayMode를 확인한다. 실패면 중지하고 결과를 보낸다. AI는 빌드/Test Runner를 하지 않는다.
2. 사용자가 **이번 N 수정 이후** FlowStateVerification Module을 다시 빌드해 **verification**에 게시한다. 이전 Script/Secret/권한/Scene/계정·기록·기준 파일은 변경하지 않는다.
3. Editor 완전 종료·재시작 → 검증 창 격리 예약 → Play → 기존 A 선택/로컬 준비/원격 허용, Infinite 해제. 일반 계정 패널/Refresh·ConfirmConsent는 누르지 않는다.
4. 첫 원격 작업으로 **원래 A 상태만 확인**을 한 번 누르고, 끝난 즉시 **전체 메시지 복사 (민감 정보 제외)**를 보낸다. RequestOrdinal/AuthElapsedMs/ClientElapsedMs/ServerElapsedMs/ServerServiceCalls가 포함된다. A가 Inactive인지와 최초 요청 시간은 별도 판정한다.
5. timeout이면 버튼을 반복하지 않는다. 기존 SDK 호출이 나중에 끝나 **LateResponseReceived/LateRequestFailed**로 표시가 바뀌면 새 요청 없이 같은 전체 메시지를 다시 복사한다. 늦은 성공은 첫 요청 PASS나 계정 상태 적용으로 처리하지 않는다. 늦은 진단이 없으면 없다고 보고하고 중지한다.
6. 첫 호출이 ResponseReceived 또는 LateResponseReceived/LateRequestFailed로 종료된 뒤에만, 같은 Play/A 세션의 **원래 A 상태만 확인**을 명시적으로 **한 번 더** 실행하여 후속 요청 결과를 별도로 보낸다. 두 결과를 비교하는 제한된 읽기 사례이며 자동 retry나 성공할 때까지 반복하는 지침은 아니다. 두 번째도 실패하면 더 반복하지 않는다. 이 시간 비교 단계에서는 기록 조회·새 제출/완료·일반 복구는 하지 않는다.

AI는 안전한 timing 차이와 서버 내부 시간/호출 횟수를 대조한다. ServerElapsedMs가 짧고 ClientElapsedMs가 길어도 그 차이 전체를 플랫폼 cold-start로 단정하지 않으며 통신/SDK/스케줄링 등을 구분할 추가 근거가 필요하다. 서버 내부 시간 자체가 길면 서비스 호출/JS host 경로를 다음 조사 대상으로 좁힌다. 실제 결과를 받기 전 timeout 해결/Step 완료로 기록하지 않는다.

#### 10-3-O. 완료까지 남은 작업을 기존 B부터 확인한다

**2026-10-05 B 공개 보존 실제 PASS:** 사용자는 같은 B/Play의 명시적 두 번째 QueryRecords에서 RequestOrdinal2/ClientElapsedMs1149/AuthElapsedMs0·ResponseReceived/SDKErrorCode0와 **PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**(전체 복사 UTC10:28:52Z)를 보고했다. 현재 B 인증으로 원래 A 공개 번호/본인 Stage 점수/acceptedAt 유지의 실제 결과다. NotRequested는 coordinator를 사용하지 않는 읽기 비교 경로의 로컬 표시이며 인증 실패를 뜻하지 않는다. 이전 공개 비교 FAIL의 당시 원인은 확정하지 않되 수정된 현재 비교 경로의 보존 확인은 완료했다. 같은 SDK 첫 요청17922ms/후속1149ms 차이는 관찰됐으나 플랫폼 cold-start/통신/서버 내부 원인 및 첫 요청 문제 해소는 미확인이다. 다음은 **10-3-P의 B 후속 일반 Stage 제출 한 번**이며 신규 Step10-1 제출/새 이전/원래A 복구/초기화는 하지 않는다. 전체 Step10-3은 아직 미완료다.

**2026-10-05 B 첫 비교 요청 종료 확인:** 사용자 UTC10:25:39Z는 B/AuthElapsedMs394·첫 QueryRecords ClientTimeout/ClientElapsedMs5087·WINDOW_TIMEOUT이며, Play를 유지한 UTC10:27:01Z 복사에서는 같은 RequestOrdinal1의 **LateResponseReceived/ClientElapsedMs17922**로 변경됐다. 최초 Module SDK 응답이 약17.9초 뒤 도착한 근거이고 timeout된 비교 작업에는 적용하지 않았으므로 공개 기록 보존 PASS로 처리하지 않는다. QueryRecords에는 현재 서버 timing 계측을 넣지 않아 ServerElapsedMs/ServiceCalls=-1은 정상 미측정이며 기동/왕복/서버 내부 병목을 이 값만으로 구분하지 못한다. 상태 endpoint만의 지연이 아니라 B 첫 QueryRecords도 지연됐음을 기록한다. 다음은 **현재 같은 B/Play/기준 입력을 유지**하고 원래 요청의 종료 확인 후 기준 비교를 **명시적으로 한 번만** 재실행하여 후속 요청 시간/보존 결과를 확보한다. 이는 자동 retry/제출·이전 변경이 아니며 일반 패널/Refresh·새 제출·초기화/Editor 재시작은 하지 않는다. 다시 timeout/FAIL이면 추가 반복하지 않고 보고한다. 이번 보고 처리에는 코드 변경/서버 게시/AI 빌드·Test Runner/원격 호출이 없고 첫 요청 지연/Step10-3은 미완료다.

**2026-10-05 CS0136 후속 수정:** 사용자가 보고한 비교 분기 current 지역 변수와 바깥 current의 이름 충돌을 comparedMe로 분리했고 관련 참조도 모두 변경했다. 분기에서 바깥 current 선언/참조가 재도입되지 않도록 source 회귀 검사를 추가했다. 주변 baseline/boardId/number는 서로 형제 scope이며 바깥 current와 같은 충돌은 없다. Node38파일·Client preflight C#48파일·diff 검사 통과, 실제 Unity 재컴파일은 사용자 대기다. source 검사로 C# 전체 lexical 규칙/실제 compilation 통과를 주장하지 않는다. 이번 후속 수정은 Client-only라 Module 재게시/Scene 변경 없이 아래 컴파일/회귀→B 보존 비교 절차를 유지한다.

**2026-10-05 완료 필요 작업 요청 후 Client-only 수정:** B Local Save를 원본과 비공개 대조해 configured scope와 원래 A 공개 번호 일치를 다시 확인했다. 기준 비교 버튼(probe1)은 account coordinator의 인증/완료 복구→repository의 재인증→개인 최고 재조회 반복을 제거했다. 기존 SDK 인증이 Local Save Player와 같은지 확인한 뒤 **서버 QueryRecords me 한 번**으로 본인 행을 받아 비교한다. 1개 본인 행은 그 서버 응답의 공개 번호를 사용하고, 빈 행일 때만 추가 서버 번호 조회가 필요하다. 캐시/Client가 정한 계정 ID로 서버 권한을 부여하지 않으며 서버 query의 활성 연결/끝단 대조는 유지한다. 계정·기록·Local Save/SDK token을 변경하지 않고 일반 패널/완료 복구를 호출하지 않는다. 불일치는 고정 안전 분류(SCOPE/BOARD/PUBLIC_NUMBER/ROW_COUNT/ROW_NUMBER/SCORE/ACCEPTED_AT 등)만 출력한다. 비교 대상/성공 기준을 완화하지 않으며 기존 ArePreserved 판정은 그대로 사용한다. 5초 전체 budget/늦은 결과 미적용/Play 격리·동의·자동 실행 차단은 유지한다.

**AI 확인:** 원래 transfer-before→after 오프라인 비교 PASS 재확인, Node38파일/정적 계약/Client preflight C#48파일·중복 member/메타/asmdef/diff 통과. 불일치 분류 EditMode4사례 추가 및 rank 변경 PASS와 진단 일치 검증을 준비했다(준비 누계 Edit167/Play21; 사용자 전체 실행 수가 아님). Unity 실제 컴파일/Test Runner는 수행하지 않았다. 이번 변경은 Client-only라 **이번 변경 때문에 Module 재빌드/게시할 필요가 없다.** N 서버 시간 계측은 사용자의 server245ms/HTTP7 실제 보고로 계측 적용 근거가 있다. 기존 첫 요청 timeout/공개 비교 FAIL은 새 실제 결과 전 해결로 표시하지 않는다.

| 완료 판정 항목 | 현재 확보한 근거/남은 일 |
| --- | --- |
| A Start/Reissue/Cancel·만료 유지·취소 후 기록 보존 | 사용자 실제 결과 확인 완료 |
| 기존 기록 있는 B의 C 연결 완료·공개 번호 유지 | B Refresh/서버 완료 Account·binding/로컬 번호 확인 완료 |
| 고정 owner·Stage score/acceptedAt/submissionId 보존 | before/after Dashboard 원문 오프라인 PASS; Infinite 행 전후 없음 |
| 이전 자격 증명 비활성 | Dashboard Completed/credentialActive=false 확인; 다른 호출자 실제 재사용 거부는 미관찰 |
| B 공개 본인 행 보존·조회/후속 제출/재시작 | 기존 공개 비교 FAIL 원인 미확정. 아래 B 비교부터 다시 확인 필요 |
| 이전 source의 실제 조회/제출 거부 | 원래 A 인증이 일반 패널에서 새 Anonymous로 복구됨. 원래 A 직접 조회는 미확인; 옛 token 복원/초기화는 하지 않음 |
| 최근 변경의 Unity 회귀·첫 요청 5초/late 시간 | 컴파일 오류 수정 후 전체 Test 결과 미보고; 첫 요청 timeout 미해결 |
| 다중 항목 CAS/제어 불가능한 원격 장애 | 로컬 장애 대역 통과와 실제 서비스 보장을 구분. 미확인을 PASS하지 않고 Step12 인계 필요 |

**지금 사용자가 할 일 — 아래만 수행한다**

1. 사용자가 이번 수정의 Unity 컴파일과 예상 밖 Error/Warning, 전체 EditMode/PlayMode를 확인한다. 실패면 실패 내용만 전달하고 다음으로 넘어가지 않는다. 빌드/게시·Scene/설정/계정 초기화는 없다.
2. Editor를 완전히 닫고 다시 연다. Online Record Verification → **Play 전 격리 실행 예약** → 상단 **Play** → 격리 세션 **B** → **격리 세션 준비 (로컬만)**. 인증 프로필 **flow-state-phase2-b**를 확인하고 원격 요청 허용 체크/Infinite 해제한다. 일반 계정 패널/Refresh·Start·Complete·새 제출은 누르지 않는다.
3. 기존 **A/step10-1-a-baseline.json**을 메모장으로 열어 전체를 **비교할 기준 JSON (로컬 전용)**에 붙인다. B baseline이나 transfer-before.json을 붙이지 않는다. 기준 파일은 덮어쓰지 않고 내용은 채팅에 보내지 않는다.
4. **기준 자료와 번호·점수·수락 시각 비교**를 한 번 누른다. 기대는 **PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 이 작업은 이전의 비공개 Dashboard 비교와 별도로 현재 B 인증으로 서버가 C 본인 행을 반환하는지 확인한다.
5. **전체 메시지 복사 (민감 정보 제외)**와 컴파일/Test 결과를 보낸다. **Play를 유지한다.** timeout이면 버튼/일반 Refresh/재제출을 누르지 않는다. 기존 요청의 late 진단으로 표시가 바뀌면 새 요청 없이 복사만 추가한다. 새로운 mismatch 분류면 어떤 값 차이인지 AI가 비교하며 민감 원문은 필요할 때 로컬 파일로만 확보한다.

**B 비교 PASS 이후 진행할 일(지금은 실행하지 않는다):** 기존 B의 일반 격리 Stage 제출60초 사례는 기존 최고보다 같거나 나쁜 제출을 받아도 원래 최고/수락 시각을 유지해야 한다. 결과 확인 뒤 같은 A 기준 비교·B Editor 재시작 뒤 보존 비교를 진행한다. 원래 A가 새 인증으로 바뀐 현재 상태에서는 전용 source 버튼을 눌러도 원래 source 거부를 입증할 수 없다. 이 필수 실제 사례를 채우려면 같은 C를 유지한 통제된 별도 연결 교체(예: 현재 C 활성 B→복구 후 A)와 source 복구 전에 거부 관찰이 필요하며, 보존/제출 검증 후 정확한 새 before/after 기준과 순서를 별도로 안내한다. 지금 새 이전을 발급/완료하거나 옛 token/데이터를 복원하지 않는다. 첫 요청 지연과 실제 거부/재사용·회귀 미확인까지 해결/기록하기 전 **Step10-3은 미완료**다. 원자성/서비스 제약의 확인 불가능성을 해당 Step의 승인 없이 제외하지 않는다.

#### 10-3-P. 같은 B에서 기존 기록을 유지하는 후속 제출을 확인한다

**2026-10-05 제출 도구 timeout/Pending 보존 보고:** 사용자 UTC10:39:54Z는 B State Error/ReasonTimeout·Pending1·GetAccountTransferStatus ordinal10/ResponseReceived/client690ms/server444ms/HTTP18·WINDOW_TIMEOUT이다. 마지막 응답은 상태 endpoint라 실제 SubmitRecord 서버 확정 여부는 이 보고만으로 판단하지 않는다. 추가 candidate/new ID/제출 버튼 반복·Pending 폐기·계정/저장 초기화는 금지한다. 코드 검사에서 probe2의 account 인증→조회 Repository의 account 재인증→Pending coordinator/Submit Repository의 재인증이 Completed recovery/번호 조회를 반복하는 구조를 확인했다. 이는 실제 중복 경로이며 각 요청 누계 시간의 실측/원격 SubmitRecord 실행 여부는 미확인이다. 아래 이전 신규60초 버튼 안내는 이미 수행했으므로 더 실행하지 않고 10-3-Q로 기존 Pending만 처리한다.

**진입 근거:** 10-3-O B 공개 보존 비교 PASS와 Pending=0을 확인했다. 현재 같은 B/Play/원래 A 기준 JSON 입력을 유지한다. 이 다음부터는 verification 기록 제출(원격 쓰기)을 사용자가 명시적으로 실행한다. 서버는 B의 실제 활성 연결을 다시 검사한다. 새 계정/새 이전/기준 캡처·기준 덮어쓰기/Scene·Module 변경은 없다.

1. **격리 Stage 기록 제출 (60초)**를 **한 번** 누른다. **Step 10-1 신규 Stage 제출·서버 행 확인 (60초)**은 빈 행을 요구하므로 누르지 않는다. 일반 계정 패널/Refresh도 먼저 누를 필요 없다. 제출 도구가 자체 account 확인/Pending 저장·확정 처리를 수행한다.
2. 이 버튼은 기존 Stage 최고보다 같거나 나쁜 60초 기록을 새 submissionId로 제출하고, 확정 후 서버 본인 행을 다시 조회한다. 기존 최고가 유지되는 경우 이전 최고 점수·acceptedAt이 같아야 PASS다. 이미 확보한 원래 Stage 점수/수락 시각과 비교 기준을 변경하지 않는다.
3. 기대 결과는 **PASS / SERVER_SUBMITTED_AND_ME_VERIFIED**이고 Pending=0이다. **전체 메시지 복사 (민감 정보 제외)**를 보내고 Play를 유지한다. 제출 timeout/FAIL 또는 Pending이 남으면 성공/실패 미확정이므로 제출 버튼/신규 ID·새 계정·저장 초기화로 반복하지 않는다. 안전한 결과부터 보고한다.
4. 제출 PASS와 Pending=0을 AI가 확인한 뒤 같은 기준의 보존 재비교와 B Editor 재시작 후 보존 확인을 다음 순서로 안내한다. 해당 결과 전 원래 C의 연결을 바꾸는 새 이전은 하지 않는다.

이 단계의 기존 제출 경로와 Stage keepBest/acceptedAt 판정은 코드/오프라인에서 준비됐으나 B 이전 완료 뒤 실제 후속 제출은 아직 사용자 결과 대기다. AI는 원격 쓰기/Unity 실행을 하지 않으며 전체 Step10-3은 미완료로 유지한다.

#### 10-3-Q. 새 기록 없이 현재 Pending 1건만 명시적으로 처리한다

**2026-10-05 기존 Pending 확정 PASS:** 사용자 UTC10:49:18Z는 B/ConsentTrue/LocalSaveReady·Pending0/AuthElapsedMs350·SubmitRecord RequestOrdinal1/ResponseReceived/client1477ms/SDKErrorCode0·**PASS/PENDING_SUBMISSION_CONFIRMED**다. 저장된 동일 ID의 서버 Submitted 확인과 로컬 제거 저장 성공을 검증 경로로 확인했다. 이전 timeout 전에 이미 서버 반영됐던 동일ID 재확정인지 이번 첫 반영인지는 응답 분류만으로 구분하지 않으며 새ID를 만든 것으로 기록하지 않는다. 이번 세션 첫 Module 요청이 성공했으므로 모든 첫 요청이 항상 실패한다는 주장은 성립하지 않는다. 기존 최초Query17.922초 사례의 원인/해결까지 확정하지 않는다. 다음은 10-3-R의 제출 후 보존 재비교다. 최신 전체 Unity Test/Error·Warning 결과는 별도 미보고이며 실제 실행 버튼 성공만으로 전체 회귀 통과를 추정하지 않는다. 전체 Step10-3은 미완료다.

**2026-10-05 Client-only 도구 준비:** **기존 Pending 1건 재전송 (새 기록 생성 없음)** 버튼을 추가했다. 기존 생산 CloudCodeRecordRepository에 검증 guard의 SDK 인증을 직접 연결해 consent/Local owner/scope/SDK Player·응답 시 동일 binding과 서버 endpoint의 활성 연결/ledger/idempotency 검사를 유지한다. 계정 coordinator의 반복 완료 복구·번호 조회는 거치지 않는다. VerificationPendingRetry는 저장된 candidate/동일 submissionId를 **1회만** SubmitAsync하며 새로운 ID/기록을 만들거나 retry loop/delay를 실행하지 않는다. 정확히 Pending1건·로컬 저장 준비/동의가 있어야 실행한다. Submitted 응답·동일 local binding·원자적 로컬 제거 저장이 성공해야 **PASS/PENDING_SUBMISSION_CONFIRMED**다. 미확정/Rejected/timeout/저장 실패는 Pending을 남겨 검토하며 자동 폐기하지 않는다. 서버에서 이미 반영됐어도 같은 ID의 재전송은 기존 ledger 확정 결과를 확인하는 것이지 새 score 쓰기를 요구하는 것이 아니다. 제출/행 재조회는 같은5초에 연속 수행하지 않는다.

**검증:** Submitted/Transient/Rejected의 단1회 호출·Pending 제거/보존, 명시적 두 번째 동일 request/ID, 동의 없음 호출0의 EditMode5사례 추가(실제 사용자 실행 대기). Node38파일/Client source contracts·preflight C#49파일(준비Edit172/Play21)·중복/메타/diff 통과. 서버/Module/Scene/기존 Local Save는 AI가 변경하지 않으며 이번 Client-only 수정에 Module 게시/빌드는 필요 없다.

1. **기존 Pending은 그대로 둔다.** Play를 종료해 이번 수정의 컴파일/전체 EditMode/PlayMode를 사용자가 확인한다. 기존 B flow-state-save.json/baseline/인증 프로필을 삭제하거나 새로 만들지 않는다.
2. Editor 종료·재시작 → 격리 예약 → Play → **기존 B** 선택 → 로컬 준비 → 원격 허용/Infinite 해제. Pending=1인지 확인한다. Pending 수가 다르면 다른 버튼을 누르지 말고 보고한다. 일반 패널/Refresh·신규/일반 Stage 제출·이전/폐기는 하지 않는다.
3. **기존 Pending 1건 재전송 (새 기록 생성 없음)**을 한 번 누른다. 기대는 **PASS / PENDING_SUBMISSION_CONFIRMED**, Pending0이다. 본인 행/score/acceptedAt 재조회까지 검증한 결과는 아니다.
4. 전체 안전한 메시지를 보내고 **Play를 유지**한다. timeout/FAIL이면 새 제출/ID/폐기를 하지 않는다. 같은 SDK 요청이 late 종료하면 재실행 없이 진단을 복사한다. 다음 동일ID 재전송의 안전성을 AI가 판단한 뒤에만 명시적으로 재실행하며 성공할 때까지 반복하지 않는다.
5. PASS/Pending0을 AI가 확인한 뒤 별도 **기준 자료와 번호·점수·수락 시각 비교**로 기존 A 공개 baseline을 다시 비교하고 B Editor 재시작 후 보존을 확인하는 순서로 진행한다. 먼저 제출 확정/로컬 제거까지 해결하며 새 이전은 하지 않는다.

전체 Step10-3/첫 요청 지연·직접 source 거부/최근 회귀는 아직 미완료다. Pending 제거를 위해 성공 판정을 완화하거나 timeout을 늘리지 않는다.

#### 10-3-R. 제출 확정 뒤 같은 B에서 원래 최고 보존을 다시 확인한다

**2026-10-05 제출 후 실제 보존 PASS:** 사용자 UTC10:51:47Z는 같은 B/Pending0·QueryRecords ordinal2/ResponseReceived/client1058ms/Auth0·**PASS/PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 기존ID 제출 확정/로컬제거 이후에도 원래 A 공개 번호/본인 Stage 최고 score/acceptedAt이 유지됨을 확인했다. 서버 원본 submissionId는 이전 before/after 내부 비교 근거와 구분하며 이번 공개 응답에 없는 값을 추가 확인한 것으로 주장하지 않는다. 다음은 10-3-S의 기존 B Editor 재시작 후 같은 기준 비교이며 신규 제출/이전은 아직 하지 않는다. 최신 전체 Unity 회귀·원래 source 직접 거부/실제 재사용·최초 지연 원인은 미확인으로 남고 전체 Step10-3은 미완료다.

1. **현재 B/Play/Pending0**을 유지한다. 새 제출·Pending 재전송·이전·일반 패널/Refresh는 누르지 않는다.
2. Infinite는 끈다. **기존 A/step10-1-a-baseline.json** 전체를 **비교할 기준 JSON (로컬 전용)**에 붙인다. 검증 창 입력이 비어 있으면 다시 붙이며 B baseline/새 캡처로 대체하거나 기존 파일을 덮어쓰지 않는다.
3. **기준 자료와 번호·점수·수락 시각 비교**를 한 번 누른다. 기대는 **PASS/PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 같은 점수의 후속 제출 확정 이후에도 원래 최고 score/acceptedAt과 공개 본인 번호가 유지됐는지 실제로 조회한다.
4. 안전한 전체 메시지를 보내고 Play를 유지한다. 실패/timeout이면 반복/신규 ID/더 나은 점수로 기준을 바꾸지 않는다. 아직 본인 행 비교가 끝나지 않았으므로 제출 확정 PASS만으로 이 단계까지 완료하지 않는다.

**다음 재시작 확인(위 PASS 확인 후):** Editor 종료·재시작 후 같은 B/기존 Local Save로 격리 실행 준비하고 동일 원래 A baseline의 읽기 비교를 수행한다. 일반 패널이 필수는 아니며 서버 query가 B 활성 연결을 검증한다. 이 결과를 받은 뒤 source 거부/재사용/최신 Unity 회귀·최초 지연 미확인 사항을 완료 조건과 대조한다. 지금은 위1~4만 수행한다.

이미 컴파일/전체 EditMode·PlayMode를 실행했다면 성공 수와 예상 밖 Error/Warning 유무를 함께 보고한다. 실행하지 않았다면 미실행으로 알려주며 실행했다고 추정하지 않는다. 이번 결과 기록에는 Client/Module/Scene 변경·AI 빌드/Test Runner/원격 호출이 없다.

#### 10-3-S. Editor 재시작 뒤 같은 B의 보존을 확인한다

**2026-10-05 B 재시작 실제 PASS:** 사용자 UTC10:55:06Z는 Editor 재시작 후 기존 B/Pending0/LocalSaveReady/Auth373ms·QueryRecords ordinal1/ResponseReceived/client1646ms·**PASS/PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. B 재시작 후 원래 A 공개번호/본인 최고score/acceptedAt 유지 확인 완료다. 최초요청의 실제 정상 사례이며 이전17.922초 timeout 원인이 해소됐거나 모든 첫요청이 동일하다고 단정하지 않는다. B 연결→동일ID 후속제출 확정→제출후 비교→재시작후 비교 근거를 확보했으며 이 검증을 추가 반복하지 않는다. 남은 직접 source 기록거부/다른 호출자의 사용된 credential 거부와 최신 전체Unity 회귀는 별도다. 원래A SDK인증은 일반패널복구로 정리되어 당시 직접거부를 재실행할 수 없다. 같은 C의 고정owner/번호/기록을 유지하며 현재 활성B→복구후A로 추가 통제된 연결교체를 하면 새 source B에서 복구 전에 관찰하고 실제사용credential의 다른호출자 거부를 확인할 수 있으나 **활성 기기가 B에서 A로 바뀌는 새 원격 mutation**이므로 사용자 동의 없이 실행/발급을 지시하지 않는다. 다음은 최신 전체컴파일/Test/Error·Warning 결과와 이 추가검증 방식 승인 확인이다. 기존자료/계정초기화/oldtoken복원/AI 원격은 없고 Step10-3은 미완료다.

1. 현재 Play를 종료하고 Unity Editor를 완전히 닫았다가 같은 프로젝트를 다시 연다. B Local Save/인증 프로필/기준 파일을 삭제하거나 새로 만들지 않는다.
2. 검증 창 → **Play 전 격리 실행 예약** → 상단 **Play** → 격리 세션 **B** → **격리 세션 준비 (로컬만)**. **flow-state-phase2-b**, Pending0/LocalSaveReady를 확인한다. 원격 요청 허용 체크/Infinite 해제한다. 일반 패널/Refresh는 필요 없다.
3. 동일한 **A/step10-1-a-baseline.json**을 비교 입력에 붙이고 **기준 자료와 번호·점수·수락 시각 비교**를 한 번 누른다. 기대는 **PASS/PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 신규/기존 Stage 제출·Pending 재전송·이전 발급/완료는 누르지 않는다.
4. 전체 안전한 메시지를 보내고 **Play를 유지**한다. 첫 요청이 timeout이면 성공/실패를 추정하거나 새 계정/기준을 만들지 않는다. 기존 요청의 late 종료 표시가 나타나면 버튼 없이 추가 복사한다. 종료 확인 후 같은 세션의 명시적 추가 읽기1회 여부는 결과를 보고 안내한다.
5. 최신 Unity 컴파일·전체 EditMode/PlayMode 성공 수·예상 밖 Error/Warning 유무를 실행한 경우에만 함께 보고한다. 아직 실행하지 않았으면 미실행으로 알려준다. 이미 보고한 과거891/255를 새 수정 후 결과로 대체하지 않는다.

이번 단계는 읽기와 사용자 Editor 재시작이며 코드/Module/Scene 변경·AI 빌드/TestRunner·원격 호출이 없다. 통과해도 직접 source 거부·실제 credential 재사용 거부·최근 회귀/서비스 제약 미확인을 함께 최종 판정하기 전 Step10-3 전체를 완료하지 않는다.

#### 10-3-T. 승인된 B→A 검증의 이전 직전 기준을 보관한다

**2026-10-05 원문 저장/정적 비교 완료:** 사용자가 저장 완료를 보고했다. 파일은 안내한 하위 폴더가 아닌 `Ignore/Step10-3/before-account.txt`, `before-stage.txt`, `before-infinite.txt`에 있어 해당 원문을 읽었다. 같은scope/논리C/번호/고정owner·Active의 현재B/같은revision·onlineOperation없음·이전transferCompleted/credentialInactive·Stageowner/안전한score·acceptedAt/submissionId와 원래두Board/metadata값 보존을 확인해 **PASS/B_TO_A_BEFORE_BASELINE_VERIFIED**다. 원문/기존A자료를 수정하지 않고 Git제외 `B-to-A/transfer-before.json`을 조립·읽기 재검증했다. 현재서버원문을 새로 확보한 근거이며 과거자료 복사로 대체하지 않았다. 다음은10-3-U의 같은B Ready/Pending0→Start1회, 실제새발급/활성교체는 아직미실행이다.

**2026-10-05 사용자 승인:** 사용자가 현재 C의 활성 연결을 B→복구 후 A로 교체하는 추가 통제 검증 진행을 승인했다. 원래 C의 공개 번호/고정 owner/Stage 기록은 유지하고 활성 기기만 바꾸며 A의 새 임시 계정 기록은 합치거나 삭제하지 않는다. AI는 원격 호출/발급/완료를 수행하지 않는다. 이번 승인은 최신 전체 Unity 회귀 성공 보고가 아니므로 별도 결과는 계속 대기다.

**지금은 Dashboard 읽기/저장만 한다. Start/Complete는 아직 누르지 않는다.**

1. 기존 `Ignore/Step10-3` 안에 **B-to-A** 폴더를 만든다. 기존 before/after/원래 A·B baseline/Local Save는 덮어쓰거나 삭제하지 않는다.
2. 기존 **Ignore/Step10-3/after-account.txt**를 메모장으로 연다. 이 파일의 **accountId**가 이번에도 유지할 C 계정이다. 새 A 계정 ID나 옛 B 임시 계정 ID를 사용하지 않는다. ID는 채팅에 붙이지 않는다.
3. Unity Dashboard에서 같은 Project의 **verification**을 선택한다. **Cloud Save → Game Data → Private**에서 `fs8-account-<위 accountId>`를 찾는다. `fs_account_v1`의 **현재 value 전체**를 복사해 **B-to-A/before-account.txt**로 저장한다. 이전 after-account 파일을 그대로 복사하는 대신 현재 서버 값을 새로 읽는다.
4. 방금 저장한 **before-account.txt의 leaderboardOwnerId**를 사용해 **Leaderboards → fs-stage-stage-001-r1**의 해당 owner 행을 찾는다. **Player ID·Score·Metadata 전체**를 복사해 **B-to-A/before-stage.txt**로 저장한다. 공개 번호/currentPlayerId/accountId로 Leaderboard owner를 대신하지 않는다.
5. 같은 owner로 **fs-infinite-v2**를 조회해 행을 **B-to-A/before-infinite.txt**로 저장한다. 실제 행이 없으면 **행 없음 확인**이라고 적는다. 조회 오류나 항목을 못 찾은 경우를 없다고 대체하지 않는다.
6. Dashboard의 Edit/Save/Delete/Create는 누르지 않는다. 세 파일을 저장하면 **저장 완료**만 알려준다. 다른 경로면 폴더 경로만 전달하고 ID/파일 본문은 채팅에 보내지 않는다.

AI가 원문을 읽어 이번 before JSON을 조립하고 기존 after/원래 A 공개 baseline과 scope·같은 논리 C/번호/고정 owner·현재 B 활성 연결/예상 revision·onlineOperation 없음·Stage score/metadata·Infinite 없음/있음 일치를 정적으로 검사한다. 과거 receipt나 timestamp를 현재 자료로 추정하지 않는다. 검사 PASS 뒤에만 B Start/새 원문 임시 보관→A 입력/완료→B 복구 전 상태/조회 거부→사용된 credential 다른 호출자 거부를 단계별로 안내한다. 현재 Play를 유지할 수 있으면 B를 유지하고 원격 버튼은 누르지 않는다. 종료했다면 재시작해 준비할 수 있으나 초기화하지 않는다. 새 이전/기록 제출은 지금 하지 않는다.

이미 최신 전체 Unity 컴파일/EditMode/PlayMode를 수행했다면 실제 성공 수/예상 밖 Error·Warning 유무도 함께 전달한다. 미실행이면 미실행으로 알려주고 실제 서비스 자료 비교와 회귀 결과를 구분한다. Step10-3 전체는 아직 미완료다.

#### 10-3-U. 같은 B에서 A로 넘길 새 이전 요청을 시작한다

**2026-10-05 기존 Start 종료확인:** 사용자 UTC11:11:41Z는 같은B/ordinal3 **LateRequestFailed/SDKErrorCode2/ClientElapsedMs25297**, ErrorTimeout/Pending0다. SDK호출이약25.3초뒤실패종료한근거이며 서버CAS/잠금 미반영을보장하지않는다. Secret/플랫폼/통신의실측원인은미확인이고 code2를상세원인으로추정하지않는다. 다음은 현재B/Play/기존인증을유지하고 **Refresh한번**으로서버실제상태를읽어안전한전체메시지를보내는것이다. TransferPending이면기존요청을유지한재발급필요여부를판정하며, Active/Ready여도새Start를자동실행하지않고결과를보고판단한다. 코드원문은미수신으로기록하고Start/Reissue/Cancel/Complete/제출을지금추가실행하지않는다. 이번보고처리코드/서버/Scene/AI빌드·TestRunner/원격변경없음·Step10-3미완료유지.

**2026-10-05 B 준비成功/Start 미확정:** 사용자는 Play 종료 뒤 기존B 재준비·계정패널을 실행해 Ready/ReasonNone/Pending0·GetPublicPlayerNumber ordinal2/client411ms(종료UTC11:08:55Z)를 보고했다. 이어 Start는 ordinal3/Auth0/StartAccountTransfer ClientTimeout5052ms·ErrorTimeout/Pending0·WINDOW_TIMEOUT(전체복사UTC11:09:43Z)이다. **Start는 서버 mutation이므로 미확정이며 로컬 Pending0이 서버 이전 잠금 미반영을 뜻하지 않는다.** 코드상 Start는 read-only 상태/번호와 달리 Secret/HMAC·발급/activation CAS를 거치며 순번3 실패를 세션 최초 요청 지연만으로 설명하지 않는다. 실제 Secret/HTTP/기동 병목 및 커밋 여부는 미확인이다. 코드 원문/만료는 이번 보고에 없으며 생성/수신된 것으로 추정하지 않는다. 다음은 현재B/Play/원래SDK세션 유지·Start/Reissue/Cancel/Complete/신규제출/일반Refresh를 추가실행하지 않고 기존 요청의 **LateResponseReceived/LateRequestFailed** 종료 표시를 전체 안전한 메시지로 확보한다. 종료확인 후 명시적Refresh로 서버 상태를 읽어 TransferPending이면 Reissue(새Start아님) 필요 여부를 안내하고, Active이면 새 요청 여부를 별도판정한다. 늦은 응답에서도 credential 원문을 적용/복원/출력하지 않는다. 이번보고처리코드/서버/Scene변경·AI빌드/TestRunner/원격없음, Step10-3미완료다.

1. 현재 **B** Play 세션이 유지되면 그대로 사용한다. 종료했다면 Editor 종료·재시작→격리예약→Play→기존B 로컬준비/원격허용으로 준비한다. B LocalSave/인증프로필/기준파일을 초기화하지 않는다.
2. **계정 패널 열기 / 상태 확인**을 한 번 누른다. 이번 B는 현재 C의 활성 기기라 Ready/ReasonNone/Pending0을 확인한 후 Start 화면을 여는 목적이다. 이후 새 이전 완료 뒤 source B 관찰에서는 이 일반 패널/Refresh를 먼저 누르면 안 된다는 점과 구분한다. timeout/FAIL/Pending잔여면 Start를 누르지 않고 안전한 결과만 보내고 Play를 유지한다.
3. Ready/Pending0이면 **Start**를 한 번 누른다. 기대는 **TransferPending/ReasonNone/Pending0**, StartAccountTransfer ResponseReceived다. 이전 취소/완료 때 사용했던 코드가 아닌 새 이전의 코드/9자리 인증값이 발급된다.
4. 새 **코드·인증값·만료시각**을 사용자만 보는 임시 종이에 기록한다. 화면을 닫거나 Editor를 종료하면 원문이 지워지므로 A 입력까지 보관한다. 코드/인증값을 채팅/공개로그/기준파일에 보내지 않는다. 이번 원문을 잃으면 이전 값으로 대신하거나 자동복원하지 말고 상태만 보고한다.
5. **전체 메시지 복사 (민감 정보 제외)** 결과와 **만료 UTC만** 보낸다. 아직 A OpenInput/Complete·B Reissue/CancelTransfer·추가Start/기록제출을 하지 않는다. Start timeout은 서버반영 미확정이므로 재발급/새Start로 우회하지 않고 먼저 보고한다.

이번 새 이전의 source는 **현재 활성B**, target은 **복구후A**이고 같은C/번호/고정owner/기록을 계속 쓴다. AI는 원문을 읽거나 저장하지 않고 원격발급/완료도 하지 않는다. 발급확인 뒤 Editor재시작/기존A Pending0준비→새원문입력/Complete를 별도 안내한다. 그후 sourceB는 일반패널복구보다 전용상태/조회관찰을 먼저 수행해야 한다. 최신전체Unity회귀결과는 계속 별도대기·Step10-3미완료다.

#### 10-3-V. 응답을 잃은 B 이전 요청의 자격 증명을 재발급한다

**2026-10-05 재발급 실제 성공:** 사용자 ReissueAccountTransfer ordinal5/ResponseReceived/SDK0/client3356ms(종료UTC11:16:12Z·전체복사11:16:22Z)는 B/TransferPending/ReasonNone/Pending0와 새기기입력안내다. 새만료는 **2027-01-03 11:09:55Z**로보고됐다. 이새transfer의최초만료는이전미보고라직접전후유지검증으로확대하지않는다. 원문은채팅에없으며AI가받거나저장하지않았다. 다음10-3-W에서사용자가최신재발급원문보관을확인한후기존A 준비/완료를1회수행한다. 아직실제B→A활성연결교체는미실행·Step10-3미완료다.

**2026-10-05 명시적Refresh확인:** 사용자 GetAccountTransferStatus ordinal4/ResponseReceived/client569ms/server271ms/HTTP7(종료UTC11:14:03Z·전체복사11:14:13Z)는 **TransferPending/ReasonNone/Pending0**와 `The original details cannot be restored. Reissue or cancel the transfer.`다. Start의 SDK늦은실패에도 서버 이전잠금이반영됐음을 확인했다. credentialActive/조회lookup전체완료나당시원문수신은이상태만으로추정하지않는다. 새Start로중복이전요청을만들지않고원래요청의Reissue로새원문을받는다.

1. 현재 **B/Play/TransferPending/Pending0**을 유지한다. **Reissue**를 한 번 누른다. Start/CancelTransfer/Complete·기록 제출·계정/저장 초기화는 누르지 않는다.
2. 기대는 ReissueAccountTransfer **ResponseReceived/TransferPending/ReasonNone/Pending0**와 새 코드/9자리 인증값 표시다. 이번 새 원문만 사용하며 응답을 잃은 Start의 원문을 복원하려 하지 않는다.
3. 새 코드·인증값·만료시각을 사용자만 보는 임시 종이에 기록한다. 채팅/공개로그/기준파일에는 원문을 넣지 않는다. 화면닫기/Editor종료 전에 A 입력에 필요한 새 원문을 보관한다.
4. **전체 메시지 복사(민감 정보 제외)**와 **만료 UTC만** 보고한다. timeout/FAIL이면 재발급을 반복하거나 원문을 받았다고 추정하지 않는다. Play를 유지하고 기존요청의late종료/서버상태부터확인한다.
5. 재발급성공/원문보관을확인한뒤 기존A 준비→입력/완료를별도안내한다. 재발급은동일transfer의자격회전이어서원래만료를연장하지않으며 서버CAS와서버값을권한근거로쓴다. 이번 새요청의 최초만료는 사용자에게보고되지않았으므로 이전옛사례만료와동일하다고추정하지않는다.

현재상태확인은실제서버근거이고거래다중항목원자성/늦은실패내부원인을입증하지않는다. 기존before·C번호/고정owner·기록은보존하고코드/Module/Scene변경·AI빌드/TestRunner/원격호출은없다. Step10-3전체미완료다.

#### 10-3-W. 기존 A에서 최신 재발급 원문으로 연결을 완료한다

**2026-10-05 A 완료/로컬반영 실제 확인:** 사용자 A로컬준비LOCAL_READY·Pending0(전체UTC11:20:24Z), 패널Ready/번호응답ordinal3/client598ms(종료11:20:33Z), Complete실행후Ready/ReasonNone/Pending0·Transfer confirmed/서버최고적용안내·후속번호응답ordinal7/client413ms(종료11:21:14Z·전체11:21:24Z)를확인했다. client413ms는마지막번호요청시간이지Complete전체시간아니다. AI의비공개LocalSave읽기검사에서A scope/번호가이번C와일치·A Player는sourceB와다름·B LocalPlayer는여전히이번before source와같음·A/B Pending0을확인했다. 서버고정행/연결revision·sourceinactive는아직별도after/dedicated관찰대기다. 동일Complete를반복하지않고다음10-3-X에서sourceB 일반복구전에상태/조회거부를확인한다. 원문은다른호출자재사용거부확인까지사용자만임시보관하고아직폐기하지않는다.

1. 사용자만 보는 종이에 **방금 B Reissue에서 표시된 최신 코드/9자리 인증값**을 보관했는지 확인한다. 새만료 **2027-01-03 11:09:55Z**인요청이다. 옛A→B코드나응답을잃은새Start의추정값을사용하지않는다. 원문을보관하지못했으면임의복원/재발급반복없이그상태만알려준다.
2. Play 종료→Editor 완전종료·재실행→검증창 격리예약→Play→**기존 A** 선택→**격리 세션 준비(로컬만)**. **flow-state-phase2-a**와기존A저장경로/Pending0/LocalSaveReady를확인한다. A는이전일반패널복구로생성된기존새Anonymous를쓰며새프로필/새저장/계정초기화는하지않는다. 원격요청허용체크, Infinite해제.
3. A에서 **계정 패널 열기 / 상태 확인**을한번누른다. 현재A는이번이전의target이며아직원래C의source가아니라입력화면준비에일반패널을쓴다. **Ready/ReasonNone/Pending0**일때만다음으로간다. timeout/오류/Pending잔여면OpenInput/Complete/재발급으로우회하지않고전체안전메시지를보내며Play유지한다.
4. **OpenInput**을누르고1의최신코드를**이전 코드**, 9자리인증값을**9자리 인증값**입력칸에직접넣는다. 공개번호를인증값으로쓰지않는다. 원문은채팅/기준/일반로그로보내지않는다.
5. **Complete**를한번누른다. 즉시전체메시지복사(민감정보제외)를보낸다. 성공이면원래C의공개번호/서버개인최고가A에반영돼Ready/Pending0·Transfer confirmed 안내가기대된다. timeout/오류는연결변경미확정이므로다시Complete/새이전/신규제출·초기화하지않고Play를유지해late종료를보고한다.
6. 완료후원래C의번호표시유지여부를사용자화면에서확인하되번호자체는보내지않는다. 완료응답과번호관찰을기록하며값/서버고정행보존은별도비교한다. A/Play유지, 제출/새이전은하지않는다.

**중요:** 이후source인B에서는일반계정패널/Refresh보다먼저전용상태/조회거부를확인해야한다. 일반패널을열면B도새Anonymous로복구되어직접거부검증기회를잃을수있다. 다음안내까지B에접속하지않는다. 최신원문종이는**사용된자격의다른호출자재사용거부확인까지**사용자만임시보관하며AI에게보내지않고해당검증종료뒤폐기한다. 같은target A의동일요청재호출은멱등완료일수있으므로다른호출자거부와혼동하지않는다.

코드/Module/Scene변경·AI빌드/TestRunner/원격없이사용자가직접완료를수행한다. 전체Step10-3/최근회귀는미완료로유지한다.

#### 10-3-X. 이전 기기 B를 복구하지 않고 비활성/기록 거부를 확인한다

**2026-10-05 sourceB 직접 비활성/기록거부 확인:** 사용자 B상태는ordinal1/client494ms/server174ms/HTTP2·ResponseReceived·**OBSERVED/ORIGINAL_INACTIVE_STATUS**(전체UTC11:33:36Z), 이어조회는ordinal2/client369ms·ResponseReceived·QueryStatusTransientFailure/QueryReasonServiceUnavailable/QueryPhaseResolveAccount/**QueryFaultActiveDeviceRequired**/ServiceStatus0·ORIGINAL_QUERY_FAILED(11:34:00Z)였다. 코드상Inactive binding에서account-service.resolve가ActiveDeviceRequired를던지고query의허용된fault분류가유지된다. 이번결과는generic오류만이아니라현재B인증의활성기기권한거부로확인할수있다. 정확한HTTP403/서버제출거부/다중항목원자성까지확대하지않는다. 기존sourceB인증복구전에실행됐으며NotRequested는coordinator미사용표시다. 다음10-3-Y의Dashboardafter계정/binding/고정행보존비교후사용된credential의다른호출자거부를진행한다. 일반B패널/Refresh·새이전/제출은아직하지않는다.

**지금부터 B의 일반 계정 패널/Refresh/ConfirmConsent를 먼저 누르지 않는다.** A완료가확인됐고BLocalSave에원래sourcePlayer가유지됨을AI가비공개대조했다. UI의전용버튼이름에 **원래 A**가있어도구현은현재선택된세션의SDK Player와저장Player를대조하므로이번에는**B**에서사용한다. 이름의A를보고격리세션A를선택하지않는다. 이번진행중에는컴파일/프로필초기화/이전추가생성을하지않는다.

1. A Play 종료→Editor 완전종료·재시작→격리예약→Play→**기존 B**선택→격리세션준비(로컬만). 인증프로필 **flow-state-phase2-b**, Pending0/LocalSaveReady확인. 원격허용체크, Infinite해제.
2. **계정 패널 열기/상태 확인·Refresh·ConfirmConsent는 누르지 않는다.**
3. **원래 A 상태만 확인 (복구 전에 / 제출 없음)**을한번누른다. 이번검사대상은선택된**B**다. 기대는 **OBSERVED/ORIGINAL_INACTIVE_STATUS**다. NotRequested는coordinator미사용의로컬표시여서서버Inactive응답과혼동하지않는다.
4. 3이기대결과면 **원래 A 기록 조회 거부만 확인 (복구 전에 / 제출 없음)**을한번누른다. 현재B가원래C의me기록에접근할수없어야하며QueryFault/Phase를함께수집한다. 기대관찰은 **OBSERVED/ORIGINAL_QUERY_FAILED**이며genericServiceUnavailable만으로정확한권한거부를PASS하지않는다. **QueryFault=AccountInactive/Phase=ResolveAccount**등생산서버분류를AI가코드와대조해판정한다.
5. 각버튼별 **전체메시지복사(민감정보제외)**를보내고**Play유지**한다. timeout/FAIL이면다음버튼/일반패널/추가이전/반복을하지않는다. 기존요청late종료가나타나면원격실행없이추가복사만한다.

다음은실제source거부관찰확인뒤Dashboard after계정/고정행·source/target binding 비교와사용된원문의다른호출자거부다. 현재sourceB를일반패널로복구하는것은그전용관찰확인후에만안내한다. 최신종이원문은여전히사용자만보관한다. 코드/Module/Scene변경·AI빌드/TestRunner/원격없음·Step10-3전체미완료유지.

#### 10-3-Y. B→A 완료 후 계정/연결/고정 행 자료를 보관한다

**2026-10-05 이전 후 내부 비교 PASS:** 지정 B-to-A 폴더의 원문 5파일을 읽었다. binding에는 Key/fs_account_v1과 fs_creation_guard_v1의 여러 JSON 값이 있어 fs_account_v1의 유일한 블록만 정적으로 추출했다. 원문은 변경하지 않고 같은 scope/C/공개 번호/고정 owner·Active target A·revision+1·source B Inactive/target A Active·transfer Completed/credentialActive=false·onlineOperation 없음·두 Board 행/score/acceptedAt/submissionId/metadata 전체 값 보존을 확인했다. Git 제외 transfer-after.json 조립/읽기 재검증과 기존 비교 도구 결과는 **PASS/TRANSFER_FIXED_ROW_BINDING_PRESERVED**다. 읽기 실패를 null/성공으로 대체하지 않았다. 다음 10-3-Z에서 source 관찰을 끝낸 B를 명시적으로 일반 패널에서 복구하고, 다른 호출자가 사용된 원문을 Complete했을 때 거부되는지 1회 확인한다. 새 Start/제출/데이터 초기화는 하지 않는다. 최신 Unity 전체 회귀/서비스 경계는 별도 미확인이며 전체 Step10-3은 미완료다.

같은Project/verification Dashboard에서아래5파일을 **Ignore/Step10-3/B-to-A**에새저장한다. before/원래A·B자료를덮어쓰지않고조회·복사만한다. B 일반패널/Refresh·새제출/이전은아직하지않는다. 파일본문/ID/원문credentials는채팅에보내지않는다.

1. **B-to-A/transfer-before.json의 account.accountId** 또는방금확인한루트before-account.txt의accountId를사용해 **Cloud Save→Game Data→Private→fs8-account-<accountId>**를찾는다. 현재`fs_account_v1` value전체를 **after-account.txt**에저장한다. 이전전과같은논리C이며임시A계정을선택하지않는다.
2. **이번before-account.txt의 currentPlayerId**는sourceB다. **fs8-player-<그값>**의value전체를 **after-source-binding.txt**에저장한다.
3. **방금새저장한 after-account.txt의 currentPlayerId**는targetA다. **fs8-player-<그값>**의value전체를 **after-target-binding.txt**에저장한다.
4. **이번before-account.txt의 leaderboardOwnerId**로 **fs-stage-stage-001-r1**에서같은고정owner행을찾아PlayerID·Score·Metadata전체를 **after-stage.txt**에저장한다.
5. 같은owner로 **fs-infinite-v2**의행을 **after-infinite.txt**에저장한다. 실제없으면 **행 없음 확인**이라고적는다. 조회실패/찾기오류를null/없음으로대체하지않는다.
6. DashboardEdit/Save/Delete/Create는누르지않는다. 저장후 **저장 완료**만알려준다. 다른폴더에저장하면경로만알려준다. AI가원문으로afterJSON조립·sameC/번호/고정owner·revision+1·sourceInactive/targetActive·행score/acceptedAt/submissionId/metadata보존을기존오프라인도구로판정한다.

실제source조회거부는10-3-X에서확인됐고이단계는내부계정/행보존확인이다. 해당PASS를받은뒤에만B복구/사용된code의다른호출자거부를안내한다. 종이최신credentials는해당검증끝까지사용자만보관한다. 코드/Module/Scene변경·AI빌드/TestRunner/원격없음, 전체Step10-3/최근Unity회귀/미확인서비스경계는미완료로유지한다.

#### 10-3-Z. 사용된 자격 증명을 다른 호출자가 재사용하면 거부되는지 확인한다

**2026-10-05 다른호출자재사용거부확인:** 사용자 새B Complete(종료UTC12:03:05Z/전체12:03:14Z)는 **Error/ReasonInvalidCredential/Pending0**·CompleteAccountTransfer ordinal6/ResponseReceived/SDK0/client755ms와`Invalid code or verification value. Enter them again.`이다. 앞선다른Player비교와사용자최신동일원문입력지침을근거로같은target멱등호출이아닌다른호출자의사용된자격재사용거부를확인했다. AI후속LocalSave비공개검사에서도B는완료targetA와다른Player/다른번호/Pending0을유지한다. Error는이사례에서예상한거부표시며통신timeout이나unexpectedError로숨기지않는다. 임시종이원문은이검증이끝났으므로사용자폐기를안내한다. 다음10-3-AA에서활성A의공개보존최종확인과최근전체Unity회귀결과/미확인서비스경계를대조한다. 새이전/Complete반복·제출·데이터초기화는없으며전체Step10-3은최종근거대조전미완료다.

**2026-10-05 새 호출자 B 복구 확인:** 사용자 Refresh(종료UTC11:57:08Z/전체11:57:24Z)는 Ready/ReasonNone/Pending0·`This device now uses a new anonymous account.`와최종번호응답ordinal5/client3208ms/Auth303ms다. 3208ms는번호요청시간이며전체복구시간으로주장하지않는다. AI의비공개LocalSave비교로동일scope·B새Player는원래sourceB/완료targetA와모두다름·B새번호는C번호와다름·Pending0을확인했다. 따라서같은target멱등완료가아닌다른호출자의사용자격재사용거부를검증할준비가됐다. 다음은현재B OpenInput→방금A완료에사용한최신동일원문직접입력→Complete1회→안전한전체메시지이며기대ReasonInvalidCredential다. 원문을AI에보내거나새Start/잘못된임의값으로대체하지않는다. 결과전종이는임시보관·Step10-3미완료·코드/서버/Scene/AI빌드·TestRunner/원격변경없음.

**2026-10-05 B 패널 복구 timeout/late 종료:** 사용자 UTC11:54:07Z는 B계정패널 실행의 ErrorTimeout/Pending0·GetAccountTransferStatus ordinal3/LateResponseReceived·client15627ms/server2669ms/HTTP2·WINDOW_TIMEOUT이다. 해당서버측정은함수내부일부경계이며client와약12958ms차이가있어지연전체를JS/CloudSave처리로설명하지않는다. 그차이는플랫폼스케줄링/통신/SDK/직렬화등구분미확인이라cold-start로확정하지않는다. 늦은응답은coordinator복구권한/상태에미적용이며Ready/새SDKPlayer확정전OpenInput/Complete를실행하지않는다. 기존요청이종료됐으므로다음현재B/Play에서**Refresh1회**로서버상태/복구를명시적으로확인하고안전한메시지를보낸다. Ready여도이번단계에서는입력/완료를아직하지않으며새호출자확인뒤다음으로진행한다. 기존source직접거부/내부보존PASS는유지하며source일반복구를이제허용한상태를구분한다. 코드/서버/Scene/AI빌드·TestRunner/원격변경없음, Step10-3미완료다.

**이제 B 일반 패널 사용을 허용한다.** source B의 직접 비활성/조회 거부와 B→A 완료 후 내부 비교가 끝났다. 이번에는 B를 생산 복구 경로에서 **새 Anonymous**로 시작해, 완료 target A와 다른 호출자의 재사용 거부를 확인한다. 같은 target A의 동일 요청 재호출은 멱등 완료일 수 있으므로 이 검증에 A를 사용하지 않는다. 원래 C의 활성 A/번호/기록은 유지해야 한다.

1. 현재 **B/Play/Pending0**를 유지한다. Play 종료했다면 Editor 완전 재시작→격리 예약→Play→기존B 로컬준비/원격허용으로 준비한다. 프로필·저장·서버 데이터는 초기화하지 않는다.
2. B에서 **계정 패널 열기 / 상태 확인**을 **한 번** 누른다. 서버 Inactive를 확인한 생산 coordinator가 옛 B 인증·C 캐시를 정리하고 새 Anonymous/새 논리 계정으로 복구한다. **Ready/Pending0**일 때만 다음으로 진행한다. timeout/오류면 입력/완료/발급 반복 없이 안전한 메시지를 보내고 Play를 유지한다.
3. **OpenInput**을 누른다. 임시 종이에 보관한 **방금 A Complete 성공에 사용했던 최신 B Reissue 코드/9자리 인증값**을 **수정 없이 그대로** 다시 입력한다. 새 코드를 발급하거나 다른 틀린 값으로 대신하지 않는다. 원문은 채팅/기준/공개로그에 넣지 않는다.
4. **Complete**를 **한 번** 누른다. 기대는 CompleteAccountTransfer **ResponseReceived**와 **Reason=InvalidCredential** 및 `Invalid code or verification value. Enter them again.` 안내다. 이 검증의 InvalidCredential은 예상한 거부 결과이며 성공 이전/HTTP 오류/통신 timeout과 구분한다. Error 상태 표시가 있더라도 Reason/응답으로 판정한다.
5. **전체 메시지 복사(민감 정보 제외)**를 보내고 **Play를 유지**한다. `Transfer confirmed`나 원래 C 번호로 바뀌면 성공으로 처리하지 말고 그대로 보고한다. timeout/FAIL일 때도 성공/거부를 추정하거나 Complete를 반복하지 않는다.
6. AI가 다른 SDK 호출자/Local Save 연결 유지와 응답을 대조해 실제 재사용 거부를 판정한 뒤, A의 C 공개 보존 재확인/최근 전체 Unity 회귀 결과/미확인 서비스 경계를 최종 대조한다. 종이 원문은 이번 확인 종료를 안내받은 후 폐기한다.

이 과정에서 생성되는 새 B 계정은 생산 inactive 복구의 정상 결과이며 C 자료를 병합·삭제하거나 임의 서버 owner로 쓰지 않는다. 같은 target의 멱등 완료와 다른 호출자의 거부를 혼동하지 않는다. AI는 인증 토큰/원문을 읽거나 원격 호출하지 않고 코드/Module/Scene/빌드/TestRunner 변경도 없다. 전체 Step10-3은 결과·회귀 대조 전 미완료다.

#### 10-3-AA. 재사용 거부 뒤 활성 A의 공개 보존과 최신 회귀를 대조한다

**최종 완료 — 2026-10-05:** 사용자가 최신 Unity Script Compilation 성공·예상 밖 Error/Warning 없음, **EditMode901/901·PlayMode255/255** 성공 및 각 Test의 예상 밖 Error/Warning 없음을 확인했다. PendingLocal 준비오류 수정 후 전체회귀로판정하며 과거891/255나정적검사를실행결과로대체하지않는다. verification 실제번호·C고정owner/행metadata·A→B/B→A활성교체/revision/sourceInactive/targetActive·sourceActiveDeviceRequired 조회거부/새Anonymous복구·사용credential다른호출자InvalidCredential·동일IDPending확정/후속제출/제출후·재시작후보존의근거와대조해 **Step10-3 수행범위를완료처리**했다. 기존미전환legacy사례는사용자승인10-2 N/A이며실제전환PASS가아니다. 응답timeout후원격commit가능성/플랫폼·통신대기원인/실서비스제어불가능경합·다중항목원자성/정밀5초미확인을보장으로기록하지않고Step12최종판정한계로유지한다. **Phase2전체/Step11·12/Production운영은완료나승인처리하지않는다.** 다음은Step11 실제생산화면흐름/Offline안내확인이며지금추가이전/조회/원격쓰기를요구하지않는다. 최신정상활성C는A, B는복구후새Anonymous이므로후속절차에서기기역할을혼동하지않는다. AI빌드/TestRunner/Scene/원격호출은없었다. 아래미완료/회귀대기문구는완료전실행이력이다.

**2026-10-05 최신 EditMode 실패/테스트 준비 수정:** 사용자 VerificationRetry 5사례가 모두 PendingLocal의후보생성준비(194줄)에서ExpectedTrue/False로실패했다고보고했다. 빈LocalSave의AccountId는string.Empty이며fixture가이를설정하지않고후보를생성해생산HasIdentity검사에서거부된것을코드대조로확인했다. 테스트준비에명시적로컬owner저장/비어있지않음확인후동의/후보생성순서를추가했다. 동의없음사례도기존Player를ordinarySave로비우려는보호정책위반을제거하고PendingLocal(false)로처음부터미동의/미귀속상태를구성했다. 생산코드/권한/서버/Scene/실제LocalSave는변경하지않으며Test삭제/성공조건완화는없다. 초기화순서source회귀추가·Node38파일/Client preflight C#49파일(기존준비Edit172/Play21)/diff통과, 실제수정후UnityTest재실행은사용자대기다. 실서비스PASS근거는유지하되이실패/수정후회귀대기로Step10-3전체완료는보류한다. 다음은사용자컴파일/전체EditMode·PlayMode/ErrorWarning결과이며새원격실행/Module게시/계정초기화는필요없다.

**2026-10-05 최종 공개 보존 PASS·추가 원격 실행 종료:** 사용자 A Editor재시작후비교(전체UTC12:09:18Z)는 Pending0/LocalSaveReady/Auth367ms·QueryRecords ordinal1/ResponseReceived/client1192ms·**PASS/PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 다른호출자재사용거부이후에도실제활성A인증으로같은C의공개번호/본인Stage최고score/acceptedAt이유지됨을확인했다. A→B/B→A연결·source직접Inactive/ActiveDeviceRequired거부·생산새Anonymous복구·같은C고정행/metadata보존·동일IDPending확정/후속제출/재시작·다른호출자사용credential거부의계획된실서비스시나리오근거를확보했다. **추가Start/재발급/완료/제출·Dashboard내보내기·공개비교를반복하지않는다.** 최신수정후전체Unity컴파일/EditMode/PlayMode·예상밖ErrorWarning결과만사용자미보고로남았으며과거891/255를새수정통과로쓰지않는다. 이회귀결과확인전Step10-3완료체크는유지하지않고미완료로둔다. 서버latecommit/플랫폼·통신대기원인·제어불가능한경합/다중항목원자성·5초정밀시각의미확인은운영상/Step12Phase2최종판정범위로계속명시하며이번정상시나리오PASS로입증됐다고하지않는다. 이번턴코드/Scene/서버/AI빌드·TestRunner/원격변경없음.

1. 이번검증에만사용한최신코드/인증값임시종이를폐기한다. 채팅/공개로그/기준에원문은보관하지않으며원문복원/재입력/새발급이필요한검증은더없다.
2. B Play종료→Editor 완전종료·재시작→격리예약→Play→**기존 A**선택→격리준비/원격허용/Infinite해제. A LocalSave/인증/원래baseline은초기화하지않고Pending0/LocalSaveReady를확인한다.
3. 원래 **A/step10-1-a-baseline.json**을비교입력에붙여 **기준 자료와 번호·점수·수락 시각 비교**를한번누른다. 일반패널/Refresh/Start·Complete/신규제출은필요없다. 기대는 **PASS/PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED**다. 실제A인증의서버me조회는A가여전히C의활성연결인지검사한다.
4. 안전한전체메시지를보내고Play를유지한다. timeout/FAIL이면재사용거부성공만으로현재A상태까지PASS하지않고그대로보고한다. 기존요청이late종료되면추가실행없이진단만복사한다.
5. 최근수정후 **Unity Script Compilation 성공 여부·예상 밖 Error/Warning 유무·전체 EditMode/PlayMode 실제 성공 수/실패 수·각각 예상 밖 Error/Warning 유무**를함께보고한다. 미실행이면미실행으로알려준다. AI가Unit실행/빌드를대신하지않으며과거891/255를최신수정의결과로대체하지않는다. 이단계에서빌드/Module재게시/Scene작업은없다.

AI는직접source거부/정상Anonymous복구·같은C/번호/고정owner/metadata유지·동일ID후속확정/재시작·사용된원문다른호출자거부를실제보고와대조한다. 이전timeout/latecommit의운영상한계, 플랫폼/통신원인미확인, 제어불가능한원격경합/장애원자성·90일정밀경계의로컬Test와실서비스범위를별도로기록한다. 미확인필수조건을승인없이N/A/PASS로바꾸거나Step12의Phase2완료로확대하지않는다. 최종결과전Step10-3전체미완료다.

#### 이후 진행 순서 — 결과 확인 후 단계별로 안내한다

1. **이전 전 내부 기준 확보:** AI 확인 뒤 사용자는 verification Dashboard의 A Private Account value와 A 고정 owner의 두 Board 기록 원문을 로컬 파일에 복사한다. AI가 비교용 JSON 조립/현재 계정·scope·owner·metadata 검사를 한다. 공개 본인 기준 파일만으로 내부 UUID owner 보존까지 입증하지 않는다. Dashboard 데이터 편집/삭제는 하지 않는다.
2. **A의 이전 요청/잠금·재발급·취소:** 기준 확보 후 A의 Start 등을 안내된 순서로 실행하고 안전한 상태만 보고한다. 코드·9자리 인증값은 사용자 화면에서만 다루며 채팅/로그/기준 파일에 넣지 않는다. 취소·재발급은 각각 별도 결과로 기록한다.
3. **B의 연결 완료:** Editor 종료·재시작 후 같은 B로 준비하고 Pending=0을 확인한다. 안내받은 마지막 유효 코드/인증값을 B 입력 화면에 사용자가 직접 넣는다. 공개 번호를 인증값으로 쓰지 않는다. Complete timeout은 성공/실패가 미확정일 수 있어 새 코드/계정을 만들거나 반복 완료하지 않고 상태부터 보고한다.
4. **후속 제출 전에 보존 비교:** B가 A의 기존 공개 본인 기준을 그대로 유지하는지 창이 비교하고, A/B binding과 같은 고정 owner의 행을 Dashboard 읽기로 확보해 AI가 기존 `compare-service-snapshots.cjs transfer`로 비교한다. 원래 B 계정의 기록을 A 기록과 합치거나 지우지 않는다.
5. **A 원래 세션 거부:** 완료 뒤 A에서는 일반 계정 패널/Refresh보다 먼저 기존 **원래 인증 세션의 Inactive·조회 실패 관찰 (복구 전에)** 버튼을 사용한다. 일반 오류만으로 정확한 거부 원인/CAS 원자성까지 입증하지 않는다. 이후 정상 복구는 기존 coordinator 경로로 따로 확인한다.
6. **코드 무효화·B 후속 제출/재시작 복구:** 앞선 비교 결과 확인 뒤 별도 안내로 진행한다. C의 새 최고가 생기기 전에 이전 전후 비교를 끝내며, 이전 코드 재사용의 같은 B 멱등 복구와 다른 계정의 거부를 혼동하지 않는다.

기존 production service/Module 함수/Guard·자동 Test/오프라인 비교 도구를 재사용한다. 이번 안내는 코드 변경이 아니므로 Unity 빌드/Test Runner/Module 재게시/Scene 작업은 필요 없다. 최신 사용자 컴파일·EditMode 891/891·PlayMode 255/255 및 Error/Warning 없음은 유지한다. 정밀 5초/90일·장애/경합 대역 검증과 실제 서비스 결과는 구분하고, 제어할 수 없는 원격 원자성/장애 보장은 Step 12에서 미확인으로 남긴다. **Step 10-3은 실제 결과가 없어 미완료**다.

사용자는 준비된 A/B 세션으로 잠금·연결 교체·A 거부·B 후속 제출·코드 비활성화를 확인한다. 자동 비교로 행·metadata 보존을 판정하고, 제어 가능한 원격 실패와 미확인 서비스 보장을 구분한다.

- [x] 이전 뒤 단일 활성 연결·기록 보존·실서비스 확인 범위를 기록했다. (Step10-3 완료: 최신 Edit901/Play255 및 verification 실제 사례; 미확인 서비스 보장은 Step12에 유지)

### AI 선행 작업

6-5에서 준비하고 Step 7에서 컴파일·격리 Test를 확인한 verification 실행 도구의 지침을 제공한다. 이 Step에서 코드 수정이 필요하면 게시/실행 전에 영향받는 Step 7 검증을 다시 수행한다. Player context 없는 Dashboard 실행이나 Client가 지정한 임의 Player ID를 인증 성공 근거로 사용하지 않는다.

### 사용자 수동 작업

1. 도구에 지정된 별도 Anonymous 프로필 A/B와 격리된 Local Save로 인증한다. 같은 로컬 프로필을 공유하는 Editor/Player를 두 기기로 간주하지 않는다.
2. 도구의 발급·재조회 검증을 실행하고 재시작 뒤 같은 번호가 돌아오는지 확인한다. 신규·기존 계정 전환은 서로 별도 사례로 실행한다.
3. 검증 도구의 실제 서비스 시나리오를 실행한다: 기존 기록 승계, A 이전 요청·잠금, B 연결, B 후속 제출·조회, A 온라인 거부, 코드 비활성화, 기존 최고·수락 시각 보존.
4. 서비스 토큰 경로의 고정 소유 행 쓰기와 실제 Player 직접 Write 거부도 도구로 확인한다. 새 행 생성/유지 및 CAS·최초 생성·부분 실패 복구의 서비스 관찰 결과는 도구가 비교하고 안전한 통과/실패 분류만 출력한다.
5. 결과 분류·실행 시각·대상 환경·Script version을 기록한다. 공개 번호·Player ID·코드·인증값을 결과 보고에 붙이지 않는다.

실제 서비스가 제어된 장애/경합 주입을 지원하지 않으면 해당 보장을 입증했다고 기록하지 않는다. 로컬 대역 결과와 원격 확인 결과를 구분하고, 미확인 원자성이 Phase 2 필수 조건을 막으면 Step 12를 미완료로 유지한다. 90일·5초 정밀 경계는 Unit Test로 확인한다.

### 완료 조건

- [x] 적용 대상의 실제 발급·고정 행·서비스 권한·A/B 연결 경계를 확인했다. (10-1/10-3 완료, 미전환 기존 계정 사례는 사용자 승인 10-2 N/A·실제 전환 검증 미실시)
- [x] 필수 서비스 제약의 확인 결과와 미확인 사항을 기록했다. (실서비스 장애/경합 원자성·지연 원인은 보장하지 않으며 Step12의 Phase2 완료 판정에서 별도 검토)

## Step 11. 게임 화면에서 계정 이전과 Offline 플레이를 확인한다

**완료 (2026-10-06):** 사용자 화면 결과와 최신 컴파일·EditMode 909/909·PlayMode 257/257 및 예상 밖 Error/Warning 없음으로 완료했다. 설정/키 바인딩 보존은 정적·자동 검증 근거이며 인위적인 수동 값 변경 확인으로 기록하지 않는다. 시작 직후 지연은 사용자 승인한 수용 제한으로 남기고 응답 시간 개선 PASS로 간주하지 않는다.

### 이 작업을 하는 이유

Step 10에서는 검증 창으로 서버 기능이 동작하는지 확인했다.
Step 11에서는 **플레이어가 실제로 사용하는 게임 화면**으로 같은 기능을 사용할 수 있는지 확인한다.

확인할 내용은 네 가지다.

- 공개 번호와 안내 글자가 화면에서 끝까지 읽힌다.
- 인터넷이 끊겨도 게임을 시작하고, 제출 대기 기록을 처리할 수 있다.
- 게임의 버튼으로 이전 코드 발급·취소·계정 이전을 할 수 있다.
- 마우스와 키보드로 원하는 버튼을 선택하고 원래 화면으로 돌아갈 수 있다.

**현재 A는 기록을 가진 계정이고, B는 새 계정이다. 이번 작업에서는 A의 계정을 B로 이전한다. 완료 후에는 B가 그 계정을 사용한다.**

아래를 **11-A → 11-B → 11-C → 11-D → 11-E → 11-F → 11-G** 순서로 수행한다. 결과는 11-H에 적는다.

### 11-A. A용·B용 Unity 실행 바로가기 만들기

**목적:** 한 PC에서 A와 B를 번갈아 실행하며 각자의 계정과 저장 파일을 사용한다.

1. Unity의 상단 Play 버튼을 눌러 Play를 종료한다.
2. `Flow State → Online Record Verification` 창을 연다. `격리 실행 예약 취소` 버튼을 누른다. 창을 닫는다.
3. Unity Editor를 종료한다.
4. Unity Hub에서 `6000.3.5f2`의 설치 폴더를 연다. 그 안의 `Editor/Unity.exe`를 찾는다.
5. Unity.exe의 바로가기를 두 개 만든다. 이름을 `Flow State A`, `Flow State B`로 정한다.
6. `Flow State A` 바로가기를 우클릭하고 **속성 → 대상**을 연다. 기존 Unity.exe 경로 뒤에 공백 한 칸과 다음 내용을 붙인다.

   ```text
   -projectPath "C:\Unity\Unity_Flow_State" --fs-verification-session=A
   ```

7. `Flow State B`의 **속성 → 대상**에도 다음 내용을 붙인다.

   ```text
   -projectPath "C:\Unity\Unity_Flow_State" --fs-verification-session=B
   ```

8. 각 바로가기에서 **적용 → 확인**을 누른다.
9. `Flow State A`로 Unity를 연다. `Assets/Scenes/SampleScene.unity`를 연다.

**결과:** A/B 바로가기 두 개가 만들어지고 A로 프로젝트가 열린다.

이후 문서의 **“A로 실행”**은 A 바로가기로 Unity를 열고 Play를 누르라는 뜻이다.
**“B로 전환”**은 Play 종료 → Unity 종료 → B 바로가기로 Unity 실행 → Play 순서로 하라는 뜻이다. A로 전환할 때도 같은 순서를 사용한다.

### 11-B. 계정 복구 안내 문구 고치기

**화면 확인 완료 — 2026-10-05:** 사용자 레이아웃 적용 후 Online Record Notice의 글자 잘림이 해결됐음을 확인받았다.

**목적:** 계정 복구 안내 한 곳을 현재 계정 이전 기능에 맞게 바꾼다.

`AccountText`, `StatusText`, `CredentialText`의 임시 글자는 Scene에서 위치를 확인하는 용도로 유지한다. AccountTransferView는 초기화와 화면 표시 시 해당 내용을 갱신하므로, 임시 글자 비우기를 완료 조건에서 제외했다.

#### 준비

1. Unity에서 Play가 켜져 있으면 상단 Play 버튼을 눌러 종료한다.
2. Project 창에서 `Assets → Scenes → SampleScene`을 더블클릭한다.

#### 복구 안내 문구 바꾸기

1. Hierarchy 검색창에 `RecoveryNoticeBodyText`를 입력한다.
2. 검색 결과에서 해당 오브젝트를 클릭한다. 위치는 `UIRoot → OnlineRecoveryNoticeCanvas → OnlineRecoveryNoticePanel → RecoveryWindow → RecoveryNoticeBodyText`다.
3. Inspector의 **TextMeshPro - Text (UI) → Text Input**을 클릭한다.
4. 기존 글자를 모두 선택하고 아래 영어 전체를 붙여 넣는다. 다섯 줄의 줄바꿈을 유지한다.

   ```text
   Online records belong to your anonymous account.
   Use a transfer code and verification value to move to another device.
   Create them on your active device before switching devices.
   A public number alone cannot recover your account.
   Offline play is always available.
   ```

#### Online Record Notice의 글자 잘림 수정

**목적:** 다섯 줄 안내가 자동 줄바꿈되어도 본문과 버튼이 겹치지 않도록 창을 넓힌다.

사용자 보고: TOP과 AROUND YOU는 모두 표시됐고 Online Record Notice의 글자가 잘린다. 현재 Scene에서 창은 520×300, 본문은 456×104로 확인됐다. 아래 값으로 사용자가 수정한다.

1. Play를 종료한다.
2. Hierarchy에서 아래 표의 오브젝트를 하나씩 검색해 선택한다.
3. Inspector의 **Rect Transform**에서 표의 값을 입력한다. Anchor와 Pivot을 먼저 입력하고 위치·크기를 입력한다.

| 오브젝트 | Anchor Min / Max | Pivot | Pos X / Y | Width / Height |
| --- | --- | --- | --- | --- |
| `RecoveryWindow` | 둘 다 X=0.5, Y=0.5 | X=0.5, Y=0.5 | 0 / 0 | 720 / 440 |
| `RecoveryNoticeTitleText` | 둘 다 X=0.5, Y=1 | X=0.5, Y=1 | 0 / -30 | 656 / 42 |
| `RecoveryNoticeBodyText` | 둘 다 X=0.5, Y=1 | X=0.5, Y=1 | 0 / -88 | 656 / 248 |
| `OnlineRecoveryNoticeConfirmButton` | 둘 다 X=0.5, Y=1 | X=0.5, Y=1 | -120 / -360 | 220 / 48 |
| `OnlineRecoveryNoticeCancelButton` | 둘 다 X=0.5, Y=1 | X=0.5, Y=1 | 120 / -360 | 220 / 48 |

4. `RecoveryNoticeBodyText`의 **TextMeshPro - Text (UI)**에서 아래 값을 지정한다.
   - Font Size: `20`
   - Auto Size: 체크 해제
   - Wrapping / Text Wrapping Mode: `Normal` (자동 줄바꿈)
   - Alignment: 가로 `Left`, 세로 `Top`
   - Overflow: `Overflow`
   - Margin: Left/Top/Right/Bottom 모두 `0`
5. `RecoveryNoticeTitleText`의 Font Size를 `32`로 지정한다.
6. Ctrl+S로 SampleScene을 저장한다.
7. Game 탭을 `1920 × 1080`으로 맞추고 Play를 누른다.
8. 게임의 **Settings → Online Record Notice**를 누른다. 제목·본문 다섯 줄·Continue/Cancel 버튼이 모두 읽히고 서로 겹치지 않는지 확인한다.

제안한 배치에서 제목은 창 위에서 30~72, 본문은 88~336, 버튼은 360~408 영역을 사용한다. 본문과 버튼 사이 간격은 24, 버튼 아래 간격은 32다. 이 간격은 정적으로 대조했으며 실제 글자 가독성은 사용자 화면 결과로 확인한다.

#### 저장하고 11-C로 이동

1. Hierarchy 검색창을 비운다.
2. Ctrl+S로 SampleScene을 저장한다.
3. Game 탭의 해상도를 `1920 × 1080`으로 선택한다.
4. 상단 Play를 누른다.
5. 11-C의 **Settings → Account / Device Transfer** 확인을 진행한다.

**11-B 완료 기준:** 영어 다섯 줄과 위의 레이아웃이 저장되고, Online Record Notice 창에서 제목·본문·버튼이 끝까지 읽힌다.

### 11-C. A의 공개 번호와 돌아가기 확인하기

**목적:** 플레이어가 자신의 계정을 알아볼 수 있고, 계정 창을 열었다 닫아도 메뉴를 계속 사용할 수 있는지 확인한다.

1. 게임 화면에서 **Settings → Account / Device Transfer**를 누른다.
2. `Enable Online Features` 버튼이 보이면 안내를 읽고 누른다.
3. `Public number:` 뒤의 번호를 본다. **10자리와 앞쪽 0까지 모두 읽히는지** 확인한다.
4. **Back**을 누른다. Settings 화면으로 돌아왔는지 확인한다.
5. Settings에서 **Online Record Notice** 버튼을 누른다. 11-B에서 입력한 안내가 끝까지 읽히는지 확인한다. Hierarchy의 버튼 이름은 `SettingsRecoveryNoticeButton`이다.
6. 안내를 닫고 Settings 화면으로 돌아온다. 설정·키 바인딩 보존은 저장 코드와 자동 Test로 판정한다.
7. Main Menu로 돌아간다. **Leaderboard**를 열고 Stage 보드를 선택한다.
8. 본인 행의 공개 번호가 끝까지 보이고 `(You)`가 붙는지 확인한다. 다른 사람의 행도 보이면 본인 행과 구분되는지 확인한다.
9. **Back**을 누른다. Main Menu로 돌아왔는지 확인한다.

**결과:** 전체 번호·계정 안내·본인 행 표시를 읽을 수 있고, Back으로 원래 화면에 돌아간다.

### 11-D. B에서 인터넷 끊기·제출 대기·기록 폐기 확인하기

**수동 화면 확인 완료 — 2026-10-05:** 사용자가 Confirm Discard 후 `Pending records discarded. Online records were kept.` 안내를 확인했다. 아래 재확인 절차의 화면 작업은 완료된 이력이다. 최신 전체 Unity Test 결과는 별도 보고 대기다. 다음 화면 작업은 11-E다.

**2026-10-05 진행 결과:** 아래 기능 흐름은 사용자 성공 확인을 받았다. 남은 항목은 폐기 완료 안내를 읽을 수 있는지다. 성공 후 Result 화면에 `Pending records discarded. Online records were kept.`를 표시하도록 수정했고, 해당 문구는 다음 사용자 동작까지 유지한다. 전체 순서를 반복하는 대신 아래 재확인 항목을 수행한다.

#### 폐기 완료 안내 재확인

1. Unity의 최신 스크립트 컴파일과 전체 EditMode/PlayMode Test 결과를 확인한다.
2. B로 실행하고 인터넷을 끊는다.
3. Stage 1을 클리어하여 제출 대기 기록 한 건을 만든다.
4. **Settings → Account / Device Transfer → Discard Pending Records → Confirm Discard**를 누른다.
5. 결과 화면에서 **Pending records discarded. Online records were kept.**가 읽히고 잠시 기다려도 유지되는지 확인한다.
6. **Back**으로 돌아간 뒤 인터넷을 연결한다. **Refresh Status**로 정상 계정 표시를 확인한다.
7. 컴파일/Test 결과와 폐기 완료 안내 확인 결과를 알려 준다.

**목적:** 인터넷이 끊겨도 플레이할 수 있고, 나중에 제출할 기록을 사용자가 이해하고 처리할 수 있는지 확인한다.
여기서 **Pending은 “서버 제출을 기다리는 기록”**을 뜻한다.

1. B로 전환한다.
2. **Settings → Account / Device Transfer**를 연다. `Enable Online Features`가 보이면 누른다. 공개 번호 표시를 확인한다.
3. Back으로 닫는다. B의 설정·키 바인딩 보존은 저장 코드와 자동 Test로 판정한다.
4. PC의 Wi-Fi를 끄거나 유선 케이블을 뽑는다.
5. **Settings → Account / Device Transfer → Refresh Status**를 누른다. Offline 또는 통신 실패와 재확인 안내가 읽히는지 확인한다.
6. Back으로 Main Menu에 돌아간다. **Play → Stage 1**로 게임을 시작한다.
7. Stage를 끝까지 클리어한다.
8. Result 화면에서 제출 대기 안내와 **Retry Submission** 버튼이 읽히는지 확인한다.
9. Main Menu로 돌아와 **Settings → Account / Device Transfer**를 연다.
10. **Retry Pending Uploads**와 **Discard Pending Records** 버튼을 확인한다. 이전 전에 대기 기록을 처리하라는 화면 안내가 이해되는지 확인한다.
11. **Discard Pending Records → Keep Records**를 누른다. 폐기 확인 화면에서 돌아오는지 확인한다.
12. 다시 **Discard Pending Records → Confirm Discard**를 누른다. 방금 만든 B의 대기 기록을 폐기한다. 폐기 완료 안내가 읽히는지 확인한다.
13. Wi-Fi를 켜거나 유선 케이블을 연결한다.
14. **Refresh Status**를 누른다. 정상 계정 표시로 돌아오는지 확인한다.
15. Main Menu → **Play → Stage 1**에서 온라인 상태로 한 번 클리어한다. Result에서 `Submitted`를 확인한다. 제출 대기 상태면 **Retry Submission** 또는 계정 화면의 **Retry Pending Uploads**로 처리한다.
16. **Settings → Account / Device Transfer → Enter Transfer Code**를 누른다. 코드 입력 화면이 열리는지 확인하고 Back으로 돌아온다.

**결과:** 인터넷 없이 플레이할 수 있고, 제출 대기·폐기 확인·온라인 복귀 안내가 읽힌다. 온라인 기록이 있는 B도 계정 이전 입력 화면을 열 수 있다.

`Refresh Status`는 계정 상태를 다시 조회하는 버튼이다.
`Retry Pending Uploads`는 저장된 제출 대기 기록을 다시 보내는 버튼이다.
이 두 버튼의 용도를 화면에서 구분할 수 있는지 확인한다.

### 11-E. A에서 코드 발급·창 닫기·재발급·취소 확인하기

**2026-10-06 확인 결과:** Start Transfer에서 코드·인증값·Expires (UTC) 표시, Start/Reissue의 발급 안내, Cancel Transfer의 취소 안내를 사용자 확인했다. 코드와 인증값을 함께 복사하고 두 입력칸에 한 번에 붙여넣는 버튼을 준비했다.

**목적:** 이전 코드가 읽기 쉽게 표시되고, 발급 화면을 닫은 사용자가 재발급이나 취소로 다음 행동을 할 수 있는지 확인한다.

1. A로 전환한다.
2. **Settings → Account / Device Transfer → Start Transfer**를 누른다.
3. 발급 화면에서 다음 세 항목이 끝까지 읽히는지 확인한다.
   - `Transfer code:` — 가운데 하이픈이 있는 코드
   - `Verification value:` — 9자리 인증값
   - `Expires (UTC):` — 만료 날짜와 시각
4. **Back**을 누른다.
5. **Account / Device Transfer**를 다시 연다.
6. 새 화면에 재발급·취소 안내와 **Reissue Code / Cancel Transfer** 버튼이 보이는지 확인한다.
7. **Reissue Code**를 누른다. 새 코드와 인증값이 읽히는지 확인한다.
8. **Cancel Transfer**를 누른다. 취소 완료 안내가 읽히는지 확인한다.
9. **Refresh Status**를 누른다. 정상 계정 표시로 돌아오는지 확인한다.

**결과:** 발급 내용을 읽을 수 있고, 창을 다시 연 뒤 재발급·취소를 사용할 수 있다.

#### 코드·인증값 복사 버튼 추가 — 사용자 Scene 작업

**목적:** 값만 복사해서 일반 Ctrl+V로 각각의 입력칸에 붙여넣는다.

1. Play를 종료하고 SampleScene을 연다.
2. Hierarchy에서 `UIRoot → MenuCanvas → SettingsPanel → AccountTransferHost → AccountTransferRoot → TransferWindow → Pages → Issued`를 선택한다.
3. Issued 아래에 **UI → Button - TextMeshPro**로 버튼 두 개를 만든다. 이전 안내대로 만든 버튼이 있으면 아래 두 버튼으로 이름을 바꾸고 모두 Issued 아래로 옮겨 재사용한다.

| 오브젝트 이름 | Text (TMP)의 Text Input | Pos X | Pos Y |
| --- | --- | --- | --- |
| `CopyTransferCodeButton` | `Copy Transfer Code` | -190 | -140 |
| `CopyVerificationValueButton` | `Copy Verification Value` | 190 | -140 |

4. 아래의 **전체 경로·Rect Transform·Image/Button·Text (TMP) 설정표**를 적용한다.
5. 각 버튼 안의 Text (TMP) 오브젝트 이름을 `Label`로 정하고 아래 텍스트 설정을 적용한다.
6. `AccountTransferHost`의 **Account Transfer View**에 아래 버튼을 연결한다.
   - **Copy Code Button**: `CopyTransferCodeButton`
   - **Copy Verification Button**: `CopyVerificationValueButton`
7. 새 버튼의 **On Click()**을 빈 목록으로 설정한다. 기존 Action Buttons 배열은 12개 구성을 유지한다.
8. Ctrl+S로 Scene을 저장한다. Unity 컴파일과 전체 EditMode/PlayMode Test를 실행하고 결과를 알려 준다.

##### 전체 Hierarchy 경로

코드 복사 버튼:

```text
UIRoot/MenuCanvas/SettingsPanel/AccountTransferHost/AccountTransferRoot/TransferWindow/Pages/Issued/CopyTransferCodeButton
```

인증값 복사 버튼:

```text
UIRoot/MenuCanvas/SettingsPanel/AccountTransferHost/AccountTransferRoot/TransferWindow/Pages/Issued/CopyVerificationValueButton
```

각 버튼의 글자는 해당 버튼 아래 `Label`에 둔다. 예를 들어 코드 복사 글자의 전체 경로는 위 코드 버튼 경로 뒤에 `/Label`을 붙인 것이다.

##### Rect Transform — 버튼 오브젝트를 선택해서 입력

| Inspector 항목 | CopyTransferCodeButton | CopyVerificationValueButton |
| --- | --- | --- |
| Anchor Min | X=0.5, Y=0.5 | X=0.5, Y=0.5 |
| Anchor Max | X=0.5, Y=0.5 | X=0.5, Y=0.5 |
| Pivot | X=0.5, Y=0.5 | X=0.5, Y=0.5 |
| Pos X | -190 | 190 |
| Pos Y | -140 | -140 |
| Pos Z | 0 | 0 |
| Width | 360 | 360 |
| Height | 40 | 40 |
| Rotation X/Y/Z | 0 / 0 / 0 | 0 / 0 / 0 |
| Scale X/Y/Z | 1 / 1 / 1 | 1 / 1 / 1 |

코드 버튼은 왼쪽, 인증값 버튼은 오른쪽에 놓인다. 두 버튼 사이 간격은 20이다. 현재 CredentialText 아래와 기존 Actions 위 사이에 놓이는 배치다.

##### Image와 Button — 두 버튼에 같은 값 적용

**Image 컴포넌트**:

- Source Image: 기존 계정 버튼과 같은 배경 Sprite
- Color: `#FFFFFF`, Alpha=`255`
- Raycast Target: 체크

**Button 컴포넌트**:

- Interactable: 체크
- Transition: `Color Tint`
- Target Graphic: 해당 버튼 자신의 **Image** 컴포넌트
- Color Multiplier: `1`
- Fade Duration: `0.1`
- On Click(): 빈 목록

Color Tint의 각 색상 입력창에서 다음 HEX와 Alpha를 입력한다.

| Button의 색상 항목 | HEX | Alpha | 표시 의미 |
| --- | --- | --- | --- |
| Normal Color | `#243B5A` | 255 | 기본 진한 파랑 |
| Highlighted Color | `#35547A` | 255 | 마우스를 올렸을 때 |
| Pressed Color | `#172B43` | 255 | 버튼을 누르는 동안 |
| Selected Color | `#496F9C` | 255 | 키보드로 선택했을 때 |
| Disabled Color | `#293442` | 255 | 비활성 상태 |

Image Color는 흰색으로 두고 실제 버튼 색은 위의 Color Tint에서 지정한다. Image Color와 Color Tint는 곱해져 표시되므로 흰색 Image를 사용하면 지정한 Tint 색이 그대로 적용된다. 색상창 HEX가 RGBA 8자리를 사용하면 위 6자리 뒤에 `FF`를 붙인다. 예: Normal=`243B5AFF`.

##### Label — 각 버튼의 자식 오브젝트를 선택해서 입력

Label의 **Rect Transform**:

- Anchor Min: X=`0`, Y=`0`
- Anchor Max: X=`1`, Y=`1`
- Pivot: X=`0.5`, Y=`0.5`
- Left=`12`, Right=`12`, Top=`0`, Bottom=`0`
- Rotation X/Y/Z=`0/0/0`, Scale X/Y/Z=`1/1/1`

Label의 **TextMeshPro - Text (UI)**:

| 항목 | 설정 |
| --- | --- |
| 코드 버튼 Text Input | `Copy Transfer Code` |
| 인증값 버튼 Text Input | `Copy Verification Value` |
| Font Asset | 기존 CredentialText와 같은 영어 폰트 |
| Font Size | 22 |
| Auto Size | 체크 해제 |
| Font Style | Normal |
| Color | `#FFFFFF`, Alpha 255 |
| Alignment | 가로 Center / 세로 Middle |
| Wrapping | No Wrap |
| Overflow | Overflow |
| Margin | Left/Top/Right/Bottom 모두 0 |
| Raycast Target | 체크 해제 |

##### Inspector 참조 연결과 확인

1. `UIRoot/MenuCanvas/SettingsPanel/AccountTransferHost`를 선택한다.
2. **Account Transfer View → Copy Code Button**에 코드 복사 버튼을 드래그한다.
3. **Account Transfer View → Copy Verification Button**에 인증값 복사 버튼을 드래그한다.
4. Ctrl+S로 저장한다.
5. A에서 Play → **Settings → Account / Device Transfer → Start Transfer**를 실행한다. 발급 화면에서 두 복사 버튼이 보이는지 확인한다.
6. 마우스를 올렸을 때와 눌렀을 때 색 변화, 방향키로 선택했을 때 선택 색이 보이는지 확인한다. 실행 중 방향키 선택 순서는 코드 복사 → 인증값 복사 → 보이는 기존 계정 버튼 순서로 구성된다.
7. 각각 클릭하여 `Transfer code copied.`와 `Verification value copied.` 안내가 보이는지 확인한다. 실제 클립보드에는 해당 값만 들어간다.

사용 방법:
- **Copy Transfer Code** → 코드 입력칸 선택 → **Ctrl+V**
- **Copy Verification Value** → 인증값 입력칸 선택 → **Ctrl+V**
- 두 입력칸을 확인한 뒤 **Complete Transfer**를 누른다.

클립보드에는 마지막으로 복사한 값 하나가 들어간다. 한 PC에서 A/B 실행을 종료·전환하며 두 값을 전달할 때는 두 값을 본인이 선택한 비공개 전달 수단에 옮겨 두고, B에서 각각 복사해 Ctrl+V로 입력한다. 복사한 원문은 OS 클립보드 기록·동기화에 남을 수 있으므로 비공개 자격 증명으로 취급한다.

새 Clipboard PlayMode Test는 위 두 복사 버튼이 연결된 뒤 실행한다. 정확한 값 복사·선행 0·busy/닫힘 차단·원래 클립보드 복원을 검사한다. Scene 작업과 실제 Unity 실행은 사용자가 수행한다.

### 11-F. A의 계정을 B로 이전하기

**2026-10-06 후속 확인:** 사용자에게 이전 후 B의 공개 번호·Leaderboard 본인 `(You)` 일치와 화면 조작 양호를 확인받았다. A의 시작 직후 번호 표시 지연은 사용자 결정으로 추가 확인을 종료했다.

**2026-10-06 사용자 확인:** 코드와 인증값 복사·안내 변경, Complete Transfer 뒤 공개 번호 변경과 Transfer confirmed 표시를 확인했다. 설정/키 바인딩을 별도로 변경하지 않은 것은 화면 검증 실패가 아니다. 아래의 보존 항목은 정적/자동 Test 판정으로 정리했다.

**목적:** 검증 창이 아닌 게임 버튼으로 계정 이전을 완료하고, 받는 쪽과 보내는 쪽의 안내가 이해되는지 확인한다.

1. A의 계정 화면에서 **Start Transfer**를 누른다.
2. 발급 화면의 **Copy Transfer Code**, **Copy Verification Value**로 두 값을 각각 복사하여 본인이 선택한 비공개 전달 수단에 옮겨 둔다. 각 복사 완료 안내를 확인한다.
3. B로 전환한다.
4. **Settings → Account / Device Transfer**를 연다.
5. **Enter Transfer Code**를 누른다.
6. 전달받은 코드만 클립보드에 복사하고 Transfer code 입력칸을 선택해 **Ctrl+V**를 누른다.
7. 전달받은 인증값만 클립보드에 복사하고 Verification value 입력칸을 선택해 **Ctrl+V**를 누른다. 두 입력칸이 채워졌는지 확인한다.
8. **Complete Transfer**를 누른다.
9. 이전 완료 안내와 공개 번호를 확인한다. B에 A가 사용하던 번호가 표시되는지 본다.
10. Back으로 Main Menu에 돌아간다. **Leaderboard → Stage 보드**를 연다. **AROUND YOU**의 본인 행에서 이전받은 계정의 번호와 Stage 기록이 읽히는지 확인한다.
11. B의 설정·키 바인딩·튜토리얼 보존은 계정 전환 저장 코드와 기존 자동 Test로 판정한다.
12. A로 전환한다. **Settings → Account / Device Transfer**를 연다.
13. 이전 완료 후 새 계정으로 시작한다는 안내를 읽는다. `Transfer confirmed. This device now uses a new anonymous account.`라는 안내가 나올 수 있다.
14. A의 설정·키 바인딩·튜토리얼 보존도 같은 정적/자동 Test 판정으로 처리한다.
15. 일반 문구를 클립보드에 복사해 이전 자격 증명을 덮어쓴다. OS 클립보드 기록과 비공개 전달 수단에 보관한 이전 원문을 정리한다.

**결과:** B가 이전받은 계정을 사용하고, A에는 새 계정 시작 안내가 표시된다. 각 실행의 설정은 유지된다.

#### A의 시작 직후 계정 화면 지연 — 추가 확인 종료

**목적:** 게임 시작 직후 백그라운드 복구와 계정 창의 Refresh가 겹쳐도 같은 결과를 기다리고 대기 상태를 종료하는지 확인한다.

사용자가 A에서 번호가 약 30초 뒤 생성되는 것처럼 느껴진다고 보고했다. 실제 원격 소요 시간은 측정되지 않았다. 코드상 백그라운드 인증에는 패널 전체 budget이 없었고, 진행 중 Refresh는 RequestInProgress로 즉시 반환했다. 수정 후 백그라운드 인증/복구도 하나의 5초 deadline을 사용하고, Refresh는 진행 중 인증의 검증된 결과를 같은 deadline 경계/현재 generation·Player 대조로 사용한다. 신선한 Active 조회는 상태를 반복 조회하지 않고 번호만 한 번 확인하며, 복구에서 이미 번호 확인을 끝냈으면 추가 번호 조회를 생략한다. timeout 뒤 늦은 응답 적용·자동 retry는 기존 경계로 차단한다. 독립된 시작 작업은 Refresh의 대기 timeout만으로 취소하지 않는다.

**사용자 결정:** 시작 직후 지연은 수용한 제한으로 넘긴다. 위 수정의 실제 지연 개선 여부와 원인은 미확인으로 남긴다.

기존 안내의 “대기 시간”은 클릭부터 결과가 표시될 때까지의 대략적인 경과 시간을 뜻했으며, 화면에 별도의 시간 표시 UI가 있다는 뜻이 아니었다. “안내 문구”는 계정 창에 실제 표시되는 상태/오류 문구를 뜻했다. 해당 시간 측정·문구 보고 요청은 종료했다.

**회귀 확인 완료:** 마지막 Client 수정 후 사용자 Unity 컴파일 성공·EditMode 909/909·PlayMode 257/257와 각 단계 예상 밖 Error/Warning 없음을 확인했다. 이 항목의 추가 사용자 작업은 없다.

### 11-G. 키보드와 마우스로 조작하기

**2026-10-06 사용자 결과:** 화면 조작이 괜찮음을 확인받았다.

**목적:** 버튼을 눈으로 찾고 원하는 화면으로 이동할 수 있는지 확인한다.

1. 계정 창을 열고 마우스로 버튼을 클릭한다. 선택한 버튼에 맞는 화면이 열리는지 확인한다.
2. 같은 화면에서 키보드 방향키를 누른다. 현재 선택된 버튼이 눈에 보이는지 확인한다.
3. 선택 표시가 실제 버튼 위치에 맞게 이동하는지 확인한다.
4. 현재 설정의 Submit 키를 누른다. 선택한 버튼이 실행되는지 확인한다.
5. Back을 선택해 실행한다. Settings로 돌아오는지 확인한다.
6. 번호·안내·입력칸·발급 화면을 보며 글자 잘림이나 겹침을 확인한다.
7. 문제가 있으면 **화면 이름 / 선택한 버튼 / 누른 방향 / 실제 이동한 버튼**을 적는다.

**결과:** Mouse와 Keyboard 모두 화면 이동이 자연스럽고, 글자와 선택 표시가 잘 보인다.

### 작업 중 오류가 표시되면

1. 화면의 **Refresh Status**를 눌러 현재 상태를 확인한다.
2. 이전 중 안내가 나오고 코드가 필요하면 **Reissue Code**를 누른다.
3. 이전을 취소하려면 **Cancel Transfer**를 누른다.
4. 상태가 계속 불명확하면 **A 또는 B / 누른 버튼 / 영어 오류 안내**를 적어 전달한다.
5. 이미 다른 Pending이 보이거나 폐기할 기록의 출처가 불명확하면 해당 화면을 닫고 **B / 대기 기록 상황**을 전달한다.

### 11-H. 결과 알려 주기

각 줄에 `성공 / 실패 / 미실시`를 적는다. 실패한 줄에는 화면 이름과 영어 안내를 함께 적는다. 번호와 코드 대신 확인 결과만 전달한다.

```text
11-A A/B 바로가기 실행:
11-B Scene 텍스트 수정·저장:
11-C A 공개 번호·계정 안내·본인 행·Back:
11-D B Offline 플레이·제출 대기·폐기·온라인 복귀:
11-E A 코드 발급·재발급·취소:
11-F A→B 완료·A 새 계정 안내·설정 유지:
11-G Mouse/Keyboard 조작·글자 가독성:
예상치 못한 Error/Warning:
현재 계정을 사용하는 세션:
```

### 작업 기록

2026-10-06 마지막 Client 수정 후 사용자 Unity Script Compilation 성공·EditMode 909/909·PlayMode 257/257 및 각 단계 예상 밖 Error/Warning 없음을 확인했다. 앞선 Step 11 화면 결과와 시작 지연 추가 확인 종료 결정을 합쳐 Step 11을 완료했다. 현재 이전받은 계정은 B가 사용한다는 앞선 관찰을 유지하며 새 서비스 상태를 추정하지 않는다. 이번 턴은 문서 갱신만 수행했고 코드·Scene·원격 상태를 변경하거나 AI가 Unity 컴파일/빌드/Test Runner를 실행하지 않았다. 다음은 Step 12의 완료 근거·미확인 사항·후속 Phase 인계 검토다.

2026-10-06 사용자 최신 컴파일·Edit904/Play257·예상 밖 로그 없음 및 B의 이전된 번호/본인 `(You)` 표시·화면 조작 결과를 기록했다. A 시작 직후 약 30초 체감 지연은 실제 시간/원인 확정이 아니며 미해결로 유지했다. 코드의 시작 인증 누적 대기·진행 중 Refresh fail-fast·중복 상태/번호 조회를 개선했고 시작 auth/복구 5초 budget, 진행 중 결과 합류, timeout 후 늦은 status 차단/명시적 retry, UI 대기와 독립 시작 작업의 분리, A Inactive 복구의 번호 단일 조회에 대한 EditMode 5사례를 추가했다. Node 38파일·50 C# source preflight(Edit180/Play23 준비)와 diff check 통과, 실제 Unity/원격 개선 결과는 사용자 대기다. Scene/Module/계정/기록을 변경하거나 Unity 빌드/Test Runner를 실행하지 않았다.

2026-10-06 사용자 복사/Complete 결과: 두 값 복사·복사 문구 변경·Complete Transfer의 공개 번호 변경/Transfer confirmed를 확인했다. 설정 변경을 하지 않아 사용자 값 비교 근거는 없으며 수동 보존 PASS로 확대하지 않는다. LocalRecordRepository.TryApplyAccountTransition은 새 저장 자료를 만들 때 `_lastSave.Settings`와 `_lastSave.HasCompletedTutorial`을 그대로 전달하고, LocalSettingsData의 BindingOverrides도 그 Settings에 포함된다. A/B 전환 분기 모두 해당 저장 경계를 사용하며 GameSystem.ClearAccountPresentation은 온라인/Result 표시만 초기화한다. 기존 AccountTransferCompletionTests의 기기 자료 보존 및 OnlineLocalSaveScopeTests의 키 바인딩 보존 assertion을 대조했다. 이 범위는 정적/자동 검증 책임으로 처리하며 사용자의 인위적인 설정 변경/값 비교를 제외했다. 실제 최신 자동 Test 실행 결과와 A 새 Anonymous/이전 후 B 본인 행/Keyboard 화면 이동 가독성은 별도 사용자 확인 대기다. 이번 작업은 문서 갱신과 읽기 전용 점검이며 실행 코드·Scene은 그대로다.

2026-10-06 Start/Reissue/Cancel의 사용자 발급·취소 표시 결과와 복사 요청을 기록했다. 실제 발급 UI의 사용자 클릭으로 코드/인증값 두 줄을 OS 클립보드에 복사하고 입력 UI에서 함께 붙여넣는 View 로직을 준비했다. 붙여넣기는 정확한 형식/길이/ASCII를 확인한 뒤 두 입력을 함께 갱신하며 잘못된 입력은 보존/안전 안내한다. 닫힘·busy·잘못된 페이지의 clipboard 동작을 차단하고 명시적 Complete 이전에는 서버 요청하지 않는다. OS clipboard 잔존은 사용자 복사 정책으로 Feature에 명시했으며 Local Save/로그/원격 전송은 변경하지 않았다. 새 PlayMode 1사례는 실제 Scene 두 버튼·복사/붙여넣기·잘못된 clipboard·선행 0·닫힘·명시적 Complete·이전 clipboard 복원을 검사한다. Node 38파일·50 C# source preflight(Edit 175/Play 23 준비) 통과, Scene 적용/Unity 실행은 사용자 대기다.

2026-10-05 11-D 보고: Offline Refresh의 안전한 실패/Offline 허용 문구, Offline Stage 클리어와 Retry Submission, Retry Pending Uploads/Discard Pending Records, Keep Records 복귀, Confirm Discard 뒤 Pending 버튼 제거, 온라인 복구 Refresh, 온라인 Stage Submitted, Enter Transfer Code의 두 입력칸을 사용자 확인했다. 폐기 완료 안내를 놓치는 문제는 미해결로 남겼다. 코드에 타이머 제거 처리는 없고 기존 흐름은 폐기 후 Overview로 즉시 돌아갔으므로, 성공 시 기존 Result 페이지/영어 완료 안내를 다음 사용자 동작까지 유지하도록 변경했다. 실패 시 기존 확인 화면과 Pending을 유지한다. 기존 EditMode 성공/실패 assertion을 보강하고 실제 SampleScene의 버튼/StatusText/Result를 사용해 여러 프레임 유지·Back 정리를 확인하는 PlayMode 1사례를 추가했다. Node 38파일 및 50 C# source preflight 통과, Edit 175/Play 22 준비이며 Unity 실행은 사용자 대기다. Scene/서버/저장 형식 변경은 없다.

2026-10-05 사용자 후속 화면 결과: TOP과 AROUND YOU 표시 성공을 확인했다. Online Record Notice의 글자 잘림을 남은 UI 문제로 기록하고 Scene 읽기 전용으로 현재 창/본문/버튼 치수를 조사했다. 11-B에 사용자 전용 레이아웃 수정 값을 작성했다. 이번 후속 작업은 문서 변경이며 Scene·실행 코드는 그대로 유지했다. 최근 코드 수정 이후의 컴파일/전체 Unity Test와 창 실제 가독성은 사용자 결과 대기다.

2026-10-05 11-C 진행 결과와 수정: 사용자에게 전체 공개 번호 표시·TOP의 `(You)` 성공을 확인받았다. Settings 복구 안내의 실제 버튼 이름을 **Online Record Notice**로 명시했다. 런타임 Leaderboard 계정 안내에 남은 한글 한 문장을 영어로 교체했다. TOP/AROUND YOU는 함께 조회하는데 OnlineAccountCoordinator가 진행 중인 두 번째 인증을 즉시 실패시키던 경로를 수정하여 동일한 진행 중 인증 Task를 공유한다. 인증 성공/실패 후 다음 명시적 조회에서는 다시 인증하고, Invalidate에 의한 stale 차단 및 이전 쓰기의 exclusive gate는 유지한다. 성공·실패/다음 조회·Invalidate의 EditMode 3사례를 추가하고 기존 동시성 Test를 결과 공유 계약에 맞췄다. 기존 PlayMode UI Test에 영어 계정 표시 assertion을 보강했다. Node 38개 검사 파일·Client 50 C# source preflight(Edit 175/Play 21 준비)·정적 계약·diff check 통과. 실제 Unity 실행 결과는 사용자 확인 대기다. Scene 원본의 텍스트에서 한글이 검출되지 않았고 Scene 변경 없이 런타임 문구만 수정했다.

#### 이번 수정 후 사용자 확인

1. Play를 종료하고 Unity의 스크립트 컴파일 결과를 확인한다.
2. 전체 EditMode와 PlayMode Test를 실행하고 통과 수 및 예상 밖 Error/Warning 유무를 알려 준다.
3. A 바로가기로 실행한 게임에서 **Main Menu → Leaderboard → Stage → Retry**를 누른다.
4. TOP과 AROUND YOU의 조회 결과가 모두 표시되고 계정 안내가 영어로 읽히는지 확인한다. 오류가 있으면 영역 이름과 영어 안내를 알려 준다.
5. Main Menu → **Settings → Online Record Notice**를 눌러 복구 안내를 확인한다.

2026-10-05: 사용자 작업을 목적·버튼·확인 결과 중심으로 다시 작성했다. Step 11의 정적 준비는 완료되었고 실제 화면 확인은 사용자 결과 대기다. 최신 사용자 컴파일 성공·EditMode 901/901·PlayMode 255/255 및 예상 밖 Error/Warning 없음은 기존 회귀 근거로 유지한다.

### 완료 조건

- [x] 11-A~G의 화면 결과가 보고됐다. 설정/키 바인딩은 정적·자동 검증, 시작 지연은 사용자 승인한 추가 확인 종료로 구분했다.
- [x] 발견한 UI 문제가 해결됐고 변경 영향에 맞는 재검증이 확인됐다. 최신 사용자 컴파일·EditMode 909/909·PlayMode 257/257, 예상 밖 Error/Warning 없음. 시작 지연의 실측 개선은 미확인·수용 제한이다.

## Step 12. Phase 2 완료를 판정하고 Phase 3~5에 인계한다

### 수행 결과 — 2026-10-06

**최종 판정: 완료.** 첫 실행 `p2v-8ff93ebaaed15bf42f7043915b7365b4-*`에서 batch/CAS·정상 이전·BEFORE1~10·AFTER1~9가 PASS였다. 두 번째 실행 `p2v-05d31f106c6b3165ef553bbe7d3f3856-*`에서 AFTER10·STORAGE_SINGLE_WINNER_RACE·STORAGE_SUBMISSION_TRANSFER_FENCE·LIVE_STORAGE_REMAINDER(FaultCases1/RaceCases2/FullRunFalse)이 PASS다. 두 실행을 합쳐 실제 Cloud Save의 저장 경계20복구·경합2사례를 확인했다. 최초 REQUEST_UNCONFIRMED/HTTP0은 실제 원인 미확인인 실패 이력으로 보존하고 후속 성공으로 없었던 일이 되었다고 기록하지 않는다. 보완 후 로컬 Node39파일/47검사·정적 계약·JavaScript/PowerShell 구문 통과도 구분한다. 이후 사용자 보고로 이번 검사에 사용한 임시 Service Account Key 폐기, `Unity_Flow_State / Cloud Save Editor` 역할 회수, 검사용 계정 삭제를 확인했다.

기존 Step10~11의 실제 Module/Anonymous/권한 거부/고정 행 보존/사용자 UI·Unity909/257와 위 실제 저장 검사 근거를 함께 대조했다. 이전 저장은 여러 요청을 복구하는 saga이며 단일 전체 transaction 보장을 요구하거나 입증한 것이 아니다. 검증한 저장 경계 실패·응답 유실·연결 경합 복구는 충족됐다. Unity 서버 내부 장애 자체·모든 플랫폼/통신 장애 조합·관리자 도구의 Jint 실행·전체 응답5초 실측은 추가 보장으로 선언하지 않는다. 기존 legacy N/A, 시작 지연 수용 제한과 Phase3~5 인계는 유지한다.

**완료 보류 이력:** 검사 실패가 아니라 이번 검사에만 허용한 임시 Key/프로젝트 역할 회수 보고를 기다렸었다. 사용자 보고로 Key 폐기·역할 회수·검사용 계정 삭제가 완료되어 이 보류를 해제했다. 아래 초기 미확인 판정은 도구 실행 전 이력이다.

**후속 자동 도구 최종 검사:** 전체 Node 39파일/43검사 통과(fail/skipped0), 신규 도구5검사에 실제 서버 코드 기반 로컬 저장 before/after20사례·batch409/동시CAS·두 대상 이전·제출 예약/이전 시작 경합을 포함했다. JavaScript/PowerShell 구문·기존 정적 계약도 통과했다. 원격 Service Account가 준비되지 않아 AI는 원격 검사를 실행하지 않았으며 사용자가 전용 도구 실행 후 안전한 결과를 전달해야 한다. 최신 Unity909/257를 추가 Unity 실행으로 대체하지 않는다.

**판정: Step 12 및 Phase 2 완료.** Step 1~11 완료와 실제 저장 경계 20복구·경합 2사례, 임시 관리자 접근 종료 보고를 함께 근거로 한다. 실제 서비스 장애/경합 복구의 범위는 확인한 사례로 한정하며, 무제한 장애 보장으로 확대하지 않는다. Step 10-2는 적용 대상 없음에 대한 사용자 승인 N/A이지 실제 기존 계정 전환 PASS가 아니다.

| Phase 2 조건 | 확보한 근거 | 판정 범위 |
| --- | --- | --- |
| 서로 다른 계정의 번호 분리·같은 계정 재요청/재시작 유지 | Step 2 로컬 동시 발급/재시도 검사, Step 10-1 A/B 실제 발급·재조회·재시작 | 정상 서비스 사례 확인. 강제 장애/경합은 로컬 검사 |
| 전체 십진 번호·내부 ID와 구분 | 서버 번호 문자열/독립 owner 계약, Step 11 전체 10자리·Leaderboard `(You)` 사용자 확인 | 완료 |
| Offline 플레이·계정 혼합 방지 | scope/Player/generation 경계 자동 Test, Step 11 Offline 플레이·Pending·폐기·온라인 복귀 | 완료 범위 기록 |
| 단일 활성 연결·원래 기기 거부 | Step 10-3 A→B/B→A 실제 교체·source Inactive/ActiveDeviceRequired·새 Anonymous 복구·사용된 credential 거부 및 실제 Cloud Save20저장 복구/경합2사례 | 검증한 정상·저장 경계 복구·경합 사례 완료. 모든 서비스 장애에 대한 무제한 보장과 구분 |
| 고정 행·기존 수락 시각 보존 | Step 10-3 Dashboard before/after 정적 비교·같은 owner/metadata, 후속 제출/재시작 공개 비교 | 완료. Infinite 행 없음 관찰을 실제 Infinite 기록 이전 검증으로 확대하지 않음 |
| 자동 Test·사용자 UI | 최신 사용자 컴파일 성공·EditMode 909/909·PlayMode 257/257·예상 밖 Error/Warning 없음, Step 11 화면 결과 | 완료 |

**이번 AI 검증:** 38개 Node Test 파일 38/38 통과(fail/skipped 0), 50개 C# source preflight·정적 계약 통과. C# source 준비 사례 Edit180/Play23은 전체 Unity 실행 수 909/257와 구분한다. 실행 코드와 Scene은 이번 Step에서 변경하지 않았다.

### 서비스 계약에서 확인한 범위와 제한

**후속 실제 확인:** 공식 계약 대조에 더해 실제 Private Custom batch의 stale lock 전체 거부 및 두 쓰기의 CAS 단일 승자를 확인했다. 여러 저장 요청의 복구는 사용자 실제20경계/경합2사례로 확인했다. 임시 Key 폐기·역할 회수·검사용 계정 삭제도 사용자 보고로 완료했다. 아래 미확인 기술 범위 설명은 원격 검사 전 이력이다.

- [Unity Cloud Save API](https://docs.unity.com/en-us/oas-cloud-save/1.0.0)의 **Set Private Custom Item Batch**는 같은 custom ID의 batch에서 한 항목이 실패하면 전체 batch가 실패하는 원자성을 명시한다. `account-store.create`의 guard와 대상 키는 같은 custom ID의 해당 batch를 사용한다. 공식 계약과 코드 대조 근거이며 실제 충돌 주입을 실행한 근거는 아니다.
- [Write locks](https://docs.unity.com/en-us/cloud-save/concepts/write-locks)의 기존 항목 충돌 검사는 `compareExchange`의 필수 writeLock 경계와 대조했다. 새 항목 생성 자체를 writeLock만으로 보호한다고 주장하지 않는다.
- 이전은 Account·binding·credential 등의 여러 저장 요청을 복구 가능한 단계로 수행한다. 위 단일 batch 계약은 전체 요청 묶음의 원자성을 증명하지 않는다. 로컬 장애/응답 유실/경합 검사와 실제 저장 경계 20복구·경합 2사례, 실제 정상 교체·조회 거부를 확인했다. Unity 서버 내부 장애와 모든 플랫폼/통신 조합의 무제한 보장은 Phase 2 완료 범위에 포함하지 않는 제한으로 기록한다.
- 시작 직후 지연은 사용자 승인한 수용 제한이다. 원인이 UGS라는 확정이나 모든 응답의 실측 5초 PASS는 없다. Timeout 후 원격 변경이 반영될 수 있으므로 명시적 상태 확인 경계를 유지한다.

### 사용자 종료 작업 — 완료

**검사 추가 실행 없이 수행한 종료 작업:** 목적은 일회성 검사에 부여한 관리자 접근 권한을 종료하는 것이었다.

1. Unity Dashboard → Administration → Service Accounts → `fs-phase2-storage-probe`에서 이번 검사에 사용한 임시 Key를 Delete/Revoke로 폐기했다.
2. Project roles에서 `Unity_Flow_State / Cloud Save Editor`를 제거했다.
3. 검사용 계정을 삭제했다.
4. 사용자 완료 보고를 Step 12 및 Roadmap에 반영했다. Key ID/Secret Key 값은 문서에 기록하지 않는다.

Key·역할 회수·계정 삭제는 로컬 소스로 확인할 수 없는 실제 Dashboard 상태이므로 사용자 보고를 근거로 기록한다. Access Token의 즉시 무효화까지 검증한 것으로 쓰지 않는다. 아래 실행 절차는 이미 성공한 이전 안내다.

**도구 실행 이력:** 사용자 후속 요청에 따라 [실제 저장 검사 실행 안내](../../../UGS/Verification/LIVE_TRANSFER_STORAGE_PROBE.md)를 작성했고, 실제 결과와 임시 Key·역할 회수·검사용 계정 삭제의 사용자 완료 보고를 받아 전체 완료 판정에 반영했다. Module 게시·Unity Build/Test·Scene 변경은 이 종료 처리에 포함하지 않는다. 아래 “현재 추가 작업 없음”은 도구 준비 전 판정 이력이다.

**현재 추가로 수행할 화면·Scene·빌드·Unity Test 작업은 없다.** 마지막 수정의 컴파일·전체 Test 결과는 이미 받았다.

남은 목적은 **서버가 이전 도중 실패하거나 두 요청이 겹쳐도 계정/번호/활성 연결을 안전하게 복구하는지 실제 서비스에서 확인하는 것**이다. 이를 사람이 빠르게 버튼을 눌러 확인하는 작업으로 대체하지 않는다. 별도 verification 자동 검증 도구와 변경/실행 대상을 먼저 정하고 승인받은 후 준비해야 한다. 현재 Step 12 요청만으로 서버에 장애 주입 기능을 게시하거나 새 계정·원격 기록을 생성하지 않는다. 미확인 위험을 수용해 완료 기준을 변경하는 선택도 별도 사용자 결정이 필요하다.

### Phase 3~5 인계

**후속 도구 검증 기록:** `verify-live-transfer-storage.cjs`와 사용자 숨김 입력 wrapper를 추가했다. production Account store/Start/Complete/resolve를 재사용하고 실제 Private Custom API 경로에 전용 주소를 대응시킨다. 기존 namespace가 발견되면 쓰기 전 중단하며 프로젝트/환경 고정 토큰 교환·redirect 차단·요청별5초·자동 HTTP retry 없음·오류 원문 숨김·무삭제 경계를 검사했다. 오래된 lock batch 전체 거부/동일 lock 동시 쓰기·완료10저장 before/after20사례·두 대상 동시 완료를 준비했다. 로컬 모의 HTTP 5개 Node 검사와 PowerShell 구문 검사는 통과했고 원격/Unity/Module 실행은 없다. 실제 Module의 인증·Jint/전체5초 경계/서버 내부 장애는 이 도구가 입증하지 않는다. 임시 관리자 키는 기존 HMAC Secret과 별개이며 사용자 로컬 입력에서만 처리한다.

| 인계 대상 | 수행 목적과 남은 작업 |
| --- | --- |
| Phase 3 | 수동 ledger·128 ID·100명 한도 해소. C별 terminal receipt 180일 보관/정리와 동일 ID 재전송·payload 충돌·180일 Pending 만료, 128건 초과/100명 초과 자료, TOP10/AROUND7 전역 공동 순위·수락 시각/번호 동점 경계를 자동 검사한다. Phase 2의 기존 제한을 해소됐다고 기록하지 않는다. |
| Phase 4 | Production/verification 분리·권한·Secret·배포 버전/롤백/호환성, 민감 정보 제외 30일 로그 보관과 삭제·장애 대응을 준비한다. 현재 verification만 게시됐으며 Production 적용은 별도 승인이다. 미확인 서비스 장애/경합 복구 근거도 함께 인계한다. |
| Phase 5 | 최신 전체 Test와 Windows x64·1920×1080 Windowed Player Build·Keyboard/Mouse 실제 Player 통합 확인을 수행한다. Build와 Unity 실행은 사용자 담당이며 Editor의 909/257를 Player Build 성공으로 대신하지 않는다. 실제 Infinite 기록·운영 대상·롤백 근거는 해당 Phase의 승인된 범위에서 확인한다. |

### AI 수행

Step 1~11 근거를 Roadmap Phase 2의 번호 유일성/유지·전체 표시·내부 ID와 구분·Offline/계정 혼합 방지·단일 활성 연결·고정 행 metadata·자동 Test/UI 완료 조건과 대조한다. Task·Roadmap·Project Memory를 실제 결과로 갱신한다. 구현 완료·대역 통과·Unity 실행·verification 원격 확인을 별도로 기록한다.

### 사용자 수동 작업

미보고된 컴파일·Test Runner·Scene 적용·서비스·화면 결과가 있을 때만 해당 결과를 전달한다. 이미 확정한 정책을 다시 승인하지 않는다.

### 완료 조건

- [x] Step 1~11의 필수 구현·검증·사용자 결과가 모두 기록됐다. Step 10-2 N/A 및 시작 지연 수용 제한을 실제 PASS와 구분했다.
- [x] 필수 완료 범위에 미확인 원자성·권한·서비스 제약 또는 필수 실패가 없다.

  기술 범위는 실제 저장20복구/경합2사례·기존 Module/권한/UI/Unity 근거로 충족했고, 일회성 검사 관리자 Key 폐기·역할 회수·검사용 계정 삭제의 사용자 완료 보고를 받았다. 무제한 장애 보장·Production 승인과 구분한다.
- [x] Phase 3의 receipt·조회 한도, Phase 4의 Production/30일 로그·복구, Phase 5의 전체 Test/Build 인계를 기록했다.

# 영향 범위

Step 1~5 서버 기반에 Step 6-1~6-5의 환경별 Local Save v6·Pending gate·인증/번호/이전 transport·생산 Leaderboard 계약·A/B 완료 반영/재시작 복구·Controller/View·격리 UI Test·verification 도구·A/B/Legacy 세션을 준비했다. 신규 누계 Edit Mode 121개·Play Mode 8개는 작성/정적 대조만 완료했고 Unity 실행·Scene/Prefab·원격 서비스는 변경하지 않았다.

# 검증 내용

문서의 Step 순서·담당·기존 경로·Phase 1 정책·Roadmap 범위를 정적으로 대조한다. 문서 작성 시 Unity 실행이나 서비스 적용을 요구하지 않는다. 향후 구현 Test는 해당 Step 수행 때 작성·실행한다.

# 검증 결과

계획 작성 당시 Step 순서·담당·참조 경로를 정적으로 대조하고 21개 하위 Step의 산출물·검증·완료 조건을 작성했다. 현재 Step 1~5는 로컬 서버 구현, Step 6-1~6-5는 C# 코드/Test 작성·정적 대조 범위에서 완료했다. Node 개인 최고 스냅샷/실제 endpoint 14/14·C/기록 통합 19/19·Cloud Code 회귀 22/22·경합/복구 22/22·완료/분리 18/18·취소/만료/조회 22/22·재발급/검증 제한 24/24·이전 시작 20/20·Account service 20/20·provisioning 19/19, 전체 30개 Node Test 파일, Client 저장/계정/완료/UI/검증 도구 정적 계약과 기존 정적 계약 및 `git diff --check`가 통과했다. 신규 Edit Mode 총 121개·Play Mode 8개 사례는 미실행이며 Unity 컴파일·원격 적용 결과는 없다.

# 후속 작업

Step 7 최신 사용자 컴파일·Edit Mode 850/850·Play Mode 242/242 및 예상 밖 Error/Warning 없음과 Step 8 사용자 Scene 연결·저장·영어 label 반영/AI 읽기 전용 정적 대조를 근거로 2026-10-04 완료 처리했다. 다음은 Step 9 verification 적용이다. AI는 빌드·Test Runner·Scene 수정·원격 요청을 수행하지 않는다. 실제 서비스 실행은 Step 10, 화면 확인은 Step 11이며 Step 9~12 및 Phase 2 전체는 미완료다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md`
- `AI/03_Features/AccountTransfer.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/02_Systems/AccountConnectionSystem.md`
- `AI/02_Systems/RecordSubmissionSystem.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`
- `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_8/20260930_01_Phase1ManualSteps.md`

# 작성 완료 기준

- [x] Phase 1 확정 계약과 Phase 2 수행 범위를 대응시켰다.
- [x] 사용자 수동 작업을 실제 적용·실행·화면 확인 Step으로 나눴다.
- [x] 정적 검증·생산 코드 Unit Test와 사용자 Unity Test Runner의 책임을 지정했다.
- [x] 계획 작성 완료와 Phase 2 수행 완료를 구분했다.
