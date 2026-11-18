# 작업 정보

## 작업명

Prototype 9 Phase 3 — Stage 기록 접근 차단과 Infinite 기록 보존 실행 절차

## 작업 일자

20261117

## 작업 담당자

- AI: 경로 조사·생산 코드/Unit Test 수정·정적 검사·로컬 대역 Test·문서/사용자 인계.
- 사용자: Unity Script Compile·필요한 Scene 적용·Unity Test Runner 실행. Player 통합 후보 검증은 Phase 4에서 수행한다.

## 작업 상태

**Phase 3 / Step 1~11 완료(2026-11-18).** 기록/Editor/UI 경계와 회귀 Test, 최종 정적/로컬 16스크립트 통과에 이어 사용자 최신 Script Compile 성공·EditMode **1031/1031**·PlayMode **316/316** 성공 및 각 단계의 예상 밖 Error/Warning 없음을 보고받았다.

실제 Unity 실행의 근거는 사용자의 이번 보고다. AI가 Compile/Runner를 실행하거나 제공되지 않은 XML/Console 로그를 직접 검토했다고 기록하지 않는다. 추가 Scene 적용·서버/Module 변경은 없다. Phase 4의 최신 Player 후보/화면 통합 검증과 Roadmap 9 전체는 미완료다.

기준선은 Phase 2 완료 시점의 사용자 EditMode **919개**, PlayMode **288개** 전체 통과와 Windows-production의 최신 `Build/ProductionQA/Candidate02/` 빌드/플레이 보고다. 이 후보는 사용자 결정으로 같은 경로에 덮어쓴 결과다. 이전 실물 보존이나 다른 후보 폴더 생성을 다시 요구하지 않는다. 이후 Phase 3 변경의 검증은 새 결과로 기록한다.

# 작업 목적

새 Client의 지원되는 사용자/직접 호출/Editor 도구 경로에서 Stage 기록의 생성·저장·조회·표시·제출·조작을 차단한다.
Infinite의 점수/Board·저장·Pending·재시도·계정 이전·개인 최고 snapshot 계약을 유지한다.
기존 구조의 경계에 guard를 적용하고, 부작용 없음과 Infinite 회귀를 Unit Test로 판정한다.

# 작업 대상

- 생산 경계: GameSystem, RecordSubmissionPolicy/Service, LocalRecordRepository, CloudCodeRecordRepository, OnlineRecordCoordinator와 실제 전송 경계.
- 공유 저장/자료형: MemoryRecordRepository, RecordSubmissionQueue, LocalSaveData/JsonCodec, RecordBoardKey/Configuration, ResultRecordPresentation.
- 계정: OnlineAccountCoordinator, AccountTransferPendingGate/Controller, 계정별 최고 snapshot/환경 격리.
- 도구: OnlineRecordVerificationWindow, VerificationPendingRetry/RemoteGuard, UgsOnlineRecordTransport와 지원되는 Editor 호출 경로.
- 자동 검증: 기존 EditMode/PlayMode Test 및 UGS/Tests의 읽기 전용 계약·로컬 SDK 대역 Test.
- 문서: Roadmap 9, 관련 Feature/System, Project Memory와 이 Task.

# 작업 전 상태

## 확정 범위와 선행 완료

1. Phase 1의 정책과 Phase 2의 완료 판정을 유지한다. Main Menu/Result는 Infinite Leaderboard만 열고 Stage 탭/Navigation/직접 선택은 이미 차단했다. 이를 다시 구현하거나 컴포넌트를 재부착하지 않는다.
2. Stage 플레이/Goal/시간 결과는 거절하고 Deprecated 소스·enum 숫자·직렬화 참조는 보존했다. Infinite/Result Retry와 Cancel 기본 선택의 Pause Retry 확인창도 완료했다.
3. 사용자는 기존 Stage 기록이 없고 앞으로도 제출되지 않는다고 전제했다. Stage 자료 조사·보관/이관·혼합 Pending 호환·전용 만료/폐기·이전 gate 예외 개발은 제외한다. 이미 확정한 정책을 재질문하지 않는다.
4. 구 Client는 미지원이다. 현재 전환에 서버 계약 변경은 불필요하다는 판정을 유지한다. 구 버전 원격 강제 차단·서버/보드 삭제·Module 배포를 이 Task에서 수행하지 않는다.
5. `fs-infinite-v2`, ScoringVersion.Current, 기존 v7 저장/계정·endpoint 계약을 유지한다. 계정 전체 snapshot을 임의 필터링하거나 이전 recipient의 다른 계정 최고와 합산하지 않는다.

## 작업 방식

- 아래 Step은 실제 수행 순서다. 각 Step의 코드/Test 준비와 검증 근거를 기록한 뒤 체크한다. 작성 시점에는 실행 체크를 하지 않는다.
- 작은 guard/기존 메서드 수정으로 해결한다. 새 구조/정책 클래스는 기존 책임으로 표현할 수 없을 때만 근거를 남겨 도입한다.
- 정적 판정은 AI가 처리한다. 파일/GUID·직렬화·이벤트/Navigation·Scene 초기 활성·프로필·소스 경로 조사를 사용자에게 넘기지 않는다.
- AI는 Unity Compile/Build/Test Runner를 실행하지 않는다. Scene/Prefab은 읽기 전용으로 확인하고 변경이 필요하면 Step 9에서 정확한 Editor 적용표를 제공한다.
- UI GameObject/컴포넌트 동적 생성, 누락 자동 보충, Runtime 고정 레이아웃 덮어쓰기는 금지한다. Text/활성/포커스/스크롤 상태 갱신은 유지한다.
- Unit Test는 가짜 인증/전송/시계/지연/메모리 저장으로 격리한다. 실제 저장·기기 계정·UGS 인증·원격 요청/제출/이전은 실행하지 않는다. 로컬 Node Test도 SDK 대역 사용을 먼저 확인한다.
- Phase 3의 사용자 실행은 Step 9 Compile/필요한 Scene 적용, Step 10 Test Runner에 모은다. 화면/Player 통합 후보의 최종 확인은 Phase 4에 인계한다.

# 조사 내용

문서 작성 시 읽기 전용으로 확인한 경로다. 상세 호출자와 반환 사유·부작용 순서는 Step 1에서 전수 확정한다.

| 현재 소스/메서드 | 확인한 현재 상태 | 실행 Step |
| --- | --- | --- |
| `Assets/Scripts/Runtime/Features/RecordSubmissionPolicy.cs` / TryCreateStageCandidate | Stage Cleared 결과로 후보를 생성하는 구현이 남아 있다. Infinite 후보는 점수 구성/상한/Scoring Version을 검증한다. | 2 |
| `Assets/Scripts/Runtime/Systems/GameSystem.cs` / TryStoreCurrentResultCandidate | 생산 Stage 종료가 차단돼도 Stage 후보를 만드는 보존 분기가 남아 있다. 진입 직후 거절이 필요하다. | 2, 7 |
| `RecordSubmissionService.cs`, `LocalRecordRepository.cs` / TryStoreCandidate, TryEnqueuePending, TryUpdatePersonalBest | 서비스→Pending/최고 저장 경계가 있다. Local은 후보 시각을 부여하고 Memory 저장소로 위임한다. | 2 |
| `MemoryRecordRepository.cs`, `RecordSubmissionQueue.cs`, `LocalRecordRepository.cs` / RestoreMemoryRecords | 저장 복원도 Memory의 동일 갱신/큐 메서드를 사용한다. 이 내부 메서드를 일괄 거절로 바꾸면 복원 후 checkpoint가 자료를 누락시킬 수 있다. | 1, 2, 5 |
| `CloudCodeRecordRepository.cs` / SubmitAsync, GetTopAsync, GetAroundAsync, GetPersonalBestAsync→QueryAsync | Board 매핑 뒤 인증/전송하는 공통 경로이며 Stage 매핑도 존재한다. UI 숨김만으로 직접 호출을 막지 못한다. | 3 |
| `OnlineRecordConfiguration.cs`, `RecordBoardKey.cs`, `LocalSaveJsonCodec.cs` | Stage Board ID/자료형/역직렬화 경로가 남아 있다. 내부 형식 보존과 실행 허용을 분리해야 한다. | 1, 3, 5 |
| `OnlineRecordCoordinator.cs` / ProcessPendingAsync | 만료/checkpoint/인증 뒤 Pending을 순회하고 Submitted/Rejected 뒤 로컬 정리를 수행한다. 거절을 원격 terminal 응답과 혼동하면 안 된다. | 4 |
| `LocalRecordRepository.cs`, `AccountTransferPendingGate.cs` | Pending 보존 180일, 이전 gate/계정 전환/내구 저장 경계가 있다. | 4, 5 |
| `OnlineAccountCoordinator.cs` / TryReadSnapshot | 계정 전체 서버 최고를 읽으며 Stage/Infinite Board를 해석한다. 요청 차단과 내부 응답 해석은 같은 책임이 아니다. | 5 |
| `Assets/Editor/OnlineRecordVerificationWindow.cs` | Stage 조회·제출·direct-write 검증과 숫자 probe 2/5/6 등의 직접 진입이 남아 있다. 일부 경로는 인증을 먼저 수행한다. | 6 |
| `VerificationPendingRetry.cs`, `VerificationRemoteGuard.cs`, `UgsOnlineRecordTransport.cs` | 검증 재전송/권한·시간 제한 wrapper와 Module SDK 전송 경계가 있다. Client UI 외의 직접 경로도 조사 대상이다. | 3, 4, 6 |

위 표의 파일명만 표시한 항목은 모두 `Assets/Scripts/Runtime/Features/` 아래다. Phase 1 계약 P9-C04~C08/C10과 Phase 2 Step 10 인계를 따른다.

# 작업 내용

## 실행 순서

| Step | 실제 수행 결과 | 담당 |
| --- | --- | --- |
| 1 | 완료 — 진입/부작용/내부 자료 보존 표·fixture 분류·정적/로컬 기준선 및 기존 실패 확정 | AI |
| 2 | 완료 — 신규 Stage 후보 생성·저장 차단 코드와 Infinite/내부 형식 Test 준비·정적 검증 | AI |
| 3 | 완료 — Stage 직접 조회/제출·전송 우회 차단 코드와 호출 0회/Infinite 회귀 Test 준비 | AI |
| 4 | 완료 — 미지원 재전송의 부작용 전 거절과 Infinite 재시도/만료/내구 저장 Test 준비·정적 검증 | AI |
| 5 | 완료 — 기존 이전/snapshot/환경 격리 계약 대조·생산 코드 보존·회귀 Test 보강/정적·로컬 검증 | AI |
| 6 | 완료 — Editor Stage UI/직접 메서드/과거 probe의 부작용 전 차단과 Infinite/격리 보호 Test 준비·정적/로컬 검증 | AI |
| 7 | 완료 — 기존 UI/Result 경계 대조·과거 후보/선택 및 지연 응답 차단·fixture 전환 감사와 회귀 Test 준비 | AI |
| 8 | 완료 — 최종 경로 폐쇄/선언·반사·Scene·저장 보존 검토와 16스크립트 통과·사용자 실행 인계 | AI |
| 9 | 완료 — 사용자 최신 Compile 성공·예상 밖 Error/Warning 없음·추가 Scene 작업 없음/AI 연결 재검사 | 사용자 + AI |
| 10 | 완료 — 사용자 EditMode 1031/1031·PlayMode 316/316 성공·각 예상 밖 Error/Warning 없음 | 사용자 + AI |
| 11 | 완료 — Phase 3 세 조건의 근거 연결과 최신 Player 후보/화면 검증의 Phase 4 인계 | AI |

## Step 1. 기록 진입 경계와 변경 전 기준선을 확정한다

### AI 작업

1-1. Phase 1 P9-C04~C08/C10·Phase 2 최종 인계를 대조한다. 신규 사용자 기능/구 Client 지원/Stage 자료 처리 정책을 추가하지 않는다.

1-2. 위 조사 표의 각 진입과 모든 호출자를 `사용자 생산 동작 / 직접 공개 실행 API / Editor 도구 / 내부 자료·복원 / 순수 모델·합성 Test`로 분류한다. 메서드별 거절값/사유, 첫 부작용(후보 ID·시각/메모리/파일/인증/전송/화면 변경), guard 위치와 Test를 한 표에 연결한다. public 키워드나 Stage 문자열만으로 내부 저장 형식을 제거하지 않는다.

1-3. `TryCreateStageCandidate`, Stage Board ID/enum, Submit/GetTop/GetAround/GetPersonalBest, Retry/Pending/snapshot, CloudCodeService 직접 호출과 Editor HTTP/direct-write 호출을 검색한다. 기존 UI guard를 우회하는 실제 경로를 빠짐없이 표에 추가한다. 없는 API를 가정하지 않는다.

1-4. 기존 Node 소스 검사와 로컬 대역 Test를 실행할 준비를 하고 현재 결과를 기록한다. 예: `node UGS/Tests/client-unity-preflight.cjs`, `node UGS/Tests/record-account-integration.test.cjs`, `node UGS/Tests/account-personal-bests.test.cjs`. 원격 호출/외부 프로세스 여부를 먼저 확인하고 오프라인 대역만 실행한다. 과거 PASS 수를 새 실행 결과로 복사하지 않는다.

### 정적 검증·Unit Test

- 반환 전/후 부작용과 지원 경로의 정적 호출망을 확인한다. Stage 참조는 `보존 사유/차단 Test` 또는 `수정 Step` 중 하나로 귀속한다.
- 기존 Stage 성공 fixture를 쓰는 Test를 전수 분류한다. 정책/생산 성공 기대는 Infinite로 전환하고, 명시적 거절 Test·순수 자료형/formatter의 합성 Test는 목적을 유지한다.
- Stage 기록이 없다는 사용자 전제와 무관한 보관/혼합 큐 호환 Test를 신규 요구사항으로 확대하지 않는다.

### 사용자 수동 작업

없음. 아직 Compile/Scene/Runner/빌드를 요청하지 않는다.

### 완료 조건

- [x] 모든 지원 진입에 소유 Step/guard/Test가 연결되고 내부 형식 보존과 생산 실행 차단이 구분됐다. 변경 전 정적/로컬 결과와 실패·한계를 기록했다.

### Step 1 수행 결과 — 2026-11-17

#### 1-1. 정책·선행 완료·기준선 대조 — 완료

- [Phase 1](20261111_02_Phase1ManualSteps.md)의 P9-C04~C08/C10 및 Step 3~4의 최신 확정 범위를 [Phase 2 Step 10](20261112_01_Phase2ManualSteps.md#step-10-phase-2-완료를-판정하고-phase-3에-인계한다), Roadmap/Memory, RecordSubmissionSystem/AccountConnectionSystem, RecordSubmission/Leaderboard/AccountTransfer와 대조했다.
- Phase 1 초기에 검토한 Stage 보관/혼합 큐/이전 예외보다 이후 확정한 **Stage 기록 없음·추가 제출 없음의 사용자 전제와 별도 처리 제외**가 우선한다. 구 Client 미지원, v7·Infinite Scoring v2/`fs-infinite-v2`·endpoint 유지, 서버/보드 삭제·원격 배포 제외를 유지한다.
- Phase 2 완료 근거인 사용자 919/288 전체 통과와 최신 Candidate02는 변경 전 기준선이다. 사용자 Leaderboard Stage 선택/탭·Navigation 거절과 Top/Around/Result Infinite 조회 고정은 재구현하지 않는다.
- 조사 시작 시 `git status --short`는 출력 없음, HEAD는 `a05ae1ae362888358bea659bb14ba293c309826c`다. 아래 줄 번호는 이 변경 전 소스 기준이다. Step 1에서 생산 C#/Test 수정은 없다.

#### 1-2. 호출망·첫 부작용·거절 경계 확정 — 완료

검색 범위는 `Assets/Scripts`, `Assets/Editor`, `Assets/Tests`의 C# 전체다. `TryCreateStageCandidate`, `TryCreateStage`, Stage Board ID/Mode, Submit/Top/Around/PersonalBest, Pending/Retry/snapshot 및 `CallAsync`, `CallModuleEndpointAsync`, `UnityWebRequest`/HTTP/direct-write를 교차 검색하고 호출 대상 본문을 읽었다. 이 범위에서 Module SDK 호출은 `UgsOnlineRecordTransport`, 직접 HTTP 전송은 `OnlineRecordVerificationWindow.PostJsonAsync`로 모인다. 서버 JS는 기존 계약/로컬 대역 조사 대상으로만 읽었다.

분류: **생산**=사용자 게임/UI 동작, **직접**=지원되는 실행 API, **도구**=Editor 조작/직접 메서드, **내부**=복원/저장/snapshot, **모델**=순수 자료형/합성 계산. 아래 guard/반환은 후속 구현 기준이며 현재 적용됐다는 뜻이 아니다. 파일명만 있는 Runtime Feature는 `Assets/Scripts/Runtime/Features/` 아래, GameSystem/UIManagementSystem은 `Assets/Scripts/Runtime/Systems/` 아래다.

| 진입·실제 호출자 / 분류 | 현재 반환·첫 부작용 | 후속 guard·거절/보존 기준 | 소유 Step / 검증 연결 |
| --- | --- | --- | --- |
| `GameSystem.TryStoreCurrentResultCandidate:1502` ← 결과 확정·갱신 처리의 두 호출(:1381/:1498) / 생산 | 기본 null/중복 검사 뒤 `Guid.NewGuid`(:1512), Stage factory, 최고 조회→service→Result 표시→SaveLocalState | Stage/무효 Mode를 ID 생성 전에 `return`; 기존 Result/최고/Pending/파일 불변 | 2·7 / RecordSubmissionPolicyTests, OnlineLocalSaveScopeTests, ResultMenuIntegrationTests |
| `RecordSubmissionPolicy.TryCreateStageCandidate:8` ← GameSystem, Editor ProbePhase2, 10개 Test 파일 / 직접 | 정상 Cleared는 `true`와 새 후보; 실패는 `false/out null` | 선언·Deprecated 사유 보존, 모든 Stage 입력은 `false/out null`; Infinite factory 검증 유지 | 2 / RecordSubmissionPolicyTests |
| `RecordSubmissionService.TryStoreCandidate:12` ← GameSystem, LocalSaveJsonCodecTests / 직접 | repository enqueue 후 최고 갱신; 성공 true | 합성 Stage/무효 Board는 repository 호출 전 false; ID·귀속/중복 기존 실패 보존 | 2 / LocalSaveJsonCodecTests, OnlineLocalSaveScopeTests |
| `LocalRecordRepository.TryEnqueuePending:107`, `TryUpdatePersonalBest:120` ← service, Editor probe, Test / 직접 | enqueue는 `_now()`로 시각 부여 후 Memory queue; 최고는 Memory 변경 | Stage/무효 후보는 시각·Memory 변경 전 false; 파일 저장은 별도 checkpoint | 2 / OnlineLocalSaveScopeTests, LocalSaveJsonCodecTests |
| `LocalRecordRepository.TryGetPersonalBest:171` ← GameSystem(:1556), Test / 직접 | Memory의 Stage 최고도 true/out 후보로 읽음; 파일/인증 없음 | 사용자용 Stage/무효 조회는 false/out null; 내부 전체 snapshot은 필터링하지 않음 | 3·5 / LocalSaveJsonCodecTests, OnlineLocalSaveScopeTests, AccountTransferCompletionTests |
| `CloudCodeRecordRepository.SubmitAsync:28` ← OnlineRecordCoordinator, VerificationPendingRetry, Test; `GameSystem.OnlineRecords:84`로 노출 / 직접 | null/타 소유자는 TransientFailure, 매핑 실패는 Rejected/InvalidBoard; 유효 Stage는 인증→submit-record 전송 | Stage를 인증 전 `TransientFailure/StageModeUnsupported`로 거절; 서버 terminal Rejected로 가장하지 않음. 무효 Board의 기존 InvalidBoard 반환과 coordinator 조기 차단을 구분 | 3·4 / OnlineRecordRepositoryTests, CloudCodeModuleRoutingTests |
| 같은 repository의 `GetTopAsync:64`, `GetAroundAsync:69`, `GetPersonalBestAsync:74`→`QueryAsync:96` ← GameSystem, Editor Query/Probe, Test / 직접 | 유효 Stage는 인증→query-records; 무효 query는 TransientFailure/InvalidQuery | Stage는 인증 전 TransientFailure/StageModeUnsupported; 무효 Board는 InvalidQuery 유지. Infinite로 치환하지 않음 | 3 / OnlineRecordRepositoryTests, OnlineAccountCoordinatorTests |
| `UgsOnlineRecordTransport.CallAsync` 두 overload(:16/:21)→`CallEndpointAsync:29` ← repository, OnlineAccountCoordinator, VerificationRemoteGuard, Editor CallSubmitAsync / 직접 | endpoint 매핑→요청 순번/진단 변경→CloudCodeService SDK; raw JSON/dictionary의 Stage 요청을 검사하지 않음 | submit-record/query-records의 Stage payload는 순번/진단·SDK 전 `NotSupportedException("StageModeUnsupported")`; 무효 record payload도 SDK 전 거절. dictionary의 request 경로도 포함. 계정 endpoint/응답 snapshot은 허용 유지 | 3 / CloudCodeModuleRoutingTests, OnlineRecordRepositoryTests; SDK 없는 직접 호출 Test |
| `VerificationRemoteGuard.CallAsync` 두 overload(:65/:70) ← Editor raw 요청, repository/account / 도구 | 권한·deadline 후 위임; Board guard 없음 | 기존 권한/timeout/late 보호 유지; raw record의 Stage는 위임 전 같은 미지원 사유로 거절. 계정 요청 일괄 차단 금지 | 3·6 / VerificationSessionTests, CloudCodeModuleRoutingTests |
| `OnlineRecordCoordinator.RetryPendingAsync:88`, `RetrySubmissionAsync:100`, `RetryAllPendingAsync:114`→`ProcessPendingAsync:125` ← GameSystem 부팅/동의/복구/현재 Result/Leaderboard, AccountTransferController retry callback, Editor / 생산·직접 | semaphore 뒤 만료 저장→checkpoint→Bind 인증→후보 순회→최대 3회 제출→terminal 제거/집계 | 대상 후보의 지원 Mode/Board를 만료·checkpoint·Bind 전에 검사, 미지원 요청은 Task 완료/no-op; 저장/인증/전송/terminal 집계 없음. 대상 ID 선택·기존 Infinite 재시도/잠금 유지 | 4 / OnlineRecordRepositoryTests, OnlineLocalSaveScopeTests |
| `VerificationPendingRetry.RetryOnceAsync:8` ← Editor probe 8, VerificationSessionTests / 도구·직접 | checkpoint가 후보 조회보다 먼저 실행; 1건이면 Submit 후 Submitted만 삭제 | 후보 지원 검사와 정확히 1건/소유 확인을 checkpoint·Submit보다 먼저 완료; Stage는 `PENDING_STAGE_UNSUPPORTED`, 원본/terminal 상태 불변 | 4·6 / VerificationSessionTests |
| `GameSystem.RefreshResultOnlineBestAsync:1004`, `QueryLeaderboardAsync:1032`, LeaderboardViewState.SelectGameMode / 생산 | Result non-Infinite는 이미 return; 사용자 Top/Around는 Infinite Board 고정; Stage 선택은 상태 불변 | 기존 guard 유지. Top=10/Around=7/Me·독립 요청·stale 보호 대조만 수행 | 7 / LeaderboardViewStateTests, GameNavigationStateTests, OnlineRecoveryNoticeFlowTests |
| `GameSystem.RetryPendingSubmission`→`RetryCurrentResultSubmission:510`, `CompleteCurrentResultOnlineAsync:1457`, `RefreshResultRecordPresentation:1570` / 생산 | 합성 Stage presentation의 후보로 Retry/상태 표시 가능; 온라인 최고 조회만 선행 차단 | Stage/무효 candidate를 Retry/표시 변경 전에 거절; Result/기존 Infinite 표시 불변 | 7 / ResultMenuIntegrationTests, OnlineRecoveryNoticeFlowTests |
| `UIManagementSystem.SetResultRecordPresentation:611` ← GameSystem.ApplyResultRecordPresentation / 생산·직접 | null 검사만 있고 text/Retry 활성 변경; presentation 모델 자체에는 Mode guard 없음 | Stage/무효 presentation의 사용자 표시 경계에서 차단; 순수 모델 constructor는 유지 | 7 / ModeResultDisplayIntegrationTests, ResultMenuIntegrationTests |
| `LocalRecordRepository.TryLoad/RestoreMemoryRecords:258` 및 `MemoryRecordRepository.TryEnqueuePending/TryUpdatePersonalBest/Create*Snapshot`, `RecordSubmissionQueue` / 내부·모델 | 복원과 신규 저장이 같은 Memory 갱신 메서드 사용; 불변 스냅샷 복사/큐 상태 관리 | Memory/queue 일괄 Stage 거절 금지. 신규 저장 guard는 위 service/Local 진입에 둠; 복원 후 checkpoint 자료 누락 방지 | 2·5 / LocalSaveJsonCodecTests, RecordSubmissionPolicyTests, OnlineLocalSaveScopeTests |
| `LocalRecordRepository.TrySave/TrySaveOnlineAccount/TryCheckpoint/CreateSaveData` ← GameSystem 저장, coordinator/account, 도구 / 내부 | 전체 최고/Pending·계정·비활성 환경을 직렬화/원자 쓰기 | 전체 자료 형식 보존; public 선언만 보고 사용자 Stage API로 취급하지 않음. 신규 자료 생성 경계와 분리, snapshot 필터 추가 없음 | 5 / LocalSaveJsonCodecTests, OnlineLocalSaveScopeTests |
| `TryRemovePending/TryExpirePending` ← coordinator/VerificationPendingRetry, `TryDiscardPending` ← AccountTransferController ConfirmDiscard / 내부·생산 공통 | 만료/terminal/명시 확인 시 저장 성공 후 Memory 정리 | 기존 Infinite 수명/commit 순서·사용자 확인 유지. Stage 전용 폐기/수명·혼합 큐 정책 추가 없음. 미지원 재전송은 위 조기 guard로 여기까지 진입하지 않음 | 4·5·7 / OnlineRecordRepositoryTests, AccountTransferControllerTests, AccountTransferUIIsolationTests |
| `AccountTransferPendingGate.TryRequestAsync`→Local.TryBeginTransferRequest, OnlineAccountCoordinator.RequestAsync/FinishCompletionAsync→TryApplyAccountTransition / 생산·내부 | Pending 0·checkpoint/잠금 후 인증/요청; 완료 후 전체 최고·계정 교체의 원자 쓰기 | 기존 gate/설정·환경 보존/전체 교체 유지. recipient 최고 합산·Stage 보관 예외 없음 | 5 / AccountTransferCompletionTests, AccountTransferControllerTests, OnlineAccountCoordinatorTests |
| `OnlineAccountCoordinator.TryReadSnapshot:469`, LocalSaveJsonCodec.ReadCandidates/TryCreateBoardKey:302/:349, LocalSaveData/OnlineArea / 내부 | Stage Board 해석→캐시 후보; codec은 factory 없이 constructor로 복원 | 사용자 Stage 조회 거절과 분리; 전체 snapshot parser·중복/불완전 응답 거절 유지. 자료 삭제/변환/신규 Pending 생성 없음 | 5 / AccountTransferCompletionTests, LocalSaveJsonCodecTests, account-personal-bests.test.cjs |
| `RecordBoardKey.TryCreateStage`, `OnlineRecordConfiguration.TryGetLeaderboardId`, StageRecordIdentity, enum / 모델·내부 | Stage 식별자/매핑 생성; 인증/파일 없음 | 값·서명·직렬화 계약 보존, 매핑 성공을 실행 허용으로 사용하지 않도록 위 실행 경계에 guard | 3·5 / OnlineRecordConfigurationTests, RecordLeaderboardPolicyTests |
| `RecordLeaderboardPolicy`, RecordSubmissionCandidate/Entry, ResultRecordPresentation, ResultTextFormatter, VerificationRecordBaseline/Comparison / 모델 | 정렬/반올림/합성 표시·비교; 자체 원격 호출 없음 | 합성 계약/Deprecated 자료 해석 보존; 생산 factory 성공 기대만 분리. Stage 문자열만으로 삭제하지 않음 | 2·6·7 / 아래 fixture 표, ResultTextFormatterTests |
| `Assets/Editor/OnlineRecordVerificationWindow.cs` Query/검증/probe/SelectedBoard/PostJsonAsync / 도구 | UI·직접 메서드·숫자 진입으로 인증/저장/SDK/HTTP 우회 가능 | 아래 1-3 표의 직접 진입 guard를 UI 숨김과 별도 구현 | 6 / VerificationSessionTests, VerificationToolIsolationTests, client-verification-tools-contracts.test.cjs |

반환 규약은 기존 bool/out·Task·OnlineSubmissionResult/OnlineLeaderboardResult를 사용한다. `StageModeUnsupported`는 로컬 지원 종료 사유이며 서버 Submitted/Rejected 영수증이 아니다. 예상 외 Mode/잘못된 record payload도 원격 요청 전에 거절하며, Step 3에서 raw JSON 해석/예외 경로를 기존 Test와 함께 구현한다. 새 정책 클래스·공개 Stage 우회 factory는 만들지 않는다.

공통 부작용 검증은 거절 전후의 후보/Result/최고/Pending snapshot 동일성, fake clock/저장/인증/transport 카운터 0, terminal 집계 불변으로 연결한다. SDK/HTTP를 호출해서 거절을 확인하지 않는다. 내부 형식 보존 행은 사용자 Stage API 허용이 아니라 자료 손실 없는 읽기/쓰기의 경계다.

#### 1-3. Editor 우회 진입 및 Stage fixture 전수 분류 — 완료

문서 초안의 `GetSelectedBoard`라는 표현에 대응하는 실제 메서드는 **SelectedBoard(:560)**다. 이름이 다른 API를 가정하지 않았다. Editor 구 도구의 Query/검증 버튼과 격리 모드의 Stage toggle/버튼은 현재 살아 있다. `CanRunLegacy/CanRunPhase2`는 실행 권한/자동 Test 격리 보호이고 Stage 지원 종료 guard는 아니다.

| Editor 진입/호출 | 현재 첫 부작용·Stage 우회 | Step 6 차단/보존·Test |
| --- | --- | --- |
| `Query(game, false):219` ← legacy Stage 조회 버튼 | `_busy` 변경 후 repository Top/Around/Me | busy/인증/전송 전 종료; Infinite=true 경로 유지; VerificationToolIsolationTests |
| `VerifyServerValidation:281`→CreateStageProbe→CallSubmitAsync | busy→SDK 인증 상태 읽기→UUID→raw submit-record; 중복/변조/버전99 모두 Stage | 메서드 첫 진입에서 지원 종료, 새 ID/SDK 요청 없음; VerificationToolIsolationTests |
| `VerifyDirectWriteDenied:316`, `VerifyAccountMismatch:352` | access token 읽기/Stage HTTP+CloudSave 쓰기 또는 SignOut/SwitchProfile/SignIn 후 Stage 조회 | token/세션 변경 전에 종료; 버튼 숨김과 직접 호출 거절을 각각 검증 |
| `SelectedBoard:560` ← probe 0/1/7 및 fallback | `_infinite=false`이면 Stage key 반환, 일부 호출자는 이미 인증 수행 | 오래된 Stage 선택 거절을 각 probe의 가장 앞에서 검사. helper에만 guard를 두거나 Infinite 치환으로 요청을 계속하지 않음 |
| `ProbePhase2(0)` | 계정 인증→선택 Board Me→공개 번호 재조회→baseline export 메모리 변경 | Infinite 기준 캡처 유지, Stage 선택 시 busy/BeginOperation/인증 전 `STAGE_RECORDS_UNSUPPORTED` |
| `ProbePhase2(1)` | baseline 해석→guard 인증→raw query-records(me), 필요시 번호 조회 | Infinite 비교 유지; Stage 선택은 baseline/인증 전 같은 거절 |
| `ProbePhase2(2/5/6)` | busy/BeginOperation→계정 인증→Stage 기준선 Me; 2/5는 UUID/후보/Pending/checkpoint/재시도/Me | 세 번호 모두 첫 진입에서 `STAGE_RECORDS_UNSUPPORTED`; 번호를 제거/재사용하지 않음; 상태·저장·인증/전송 0회 Test |
| `ProbePhase2(3)`→`ProbePhase2DirectWrites:807` | 계정 인증→access token→Stage Leaderboard/CloudSave HTTP | probe 3 및 helper 각각 인증/token/HTTP 전 종료 |
| `ProbePhase2(4)` | guard 인증→get-account-transfer-status | 계정 상태 전용이므로 기존 권한/격리 아래 유지, Stage Board 선택과 무관 |
| `ProbePhase2(7)` | guard 인증→선택 Board의 raw query-records | Infinite 권한 거절 비교 유지; Stage 선택은 인증 전 종료 |
| `ProbePhase2(8)` | production repository 구성→VerificationPendingRetry의 checkpoint/Submit | Infinite 기존 Pending 1건 재전송 유지; Stage 후보는 Step 4 guard로 저장/인증/전송 전 거절 |
| 그 외 probe 값의 fallback | 계정 인증→SelectedBoard Me→baseline 비교 | 지원 번호 0~8 외 값은 busy/인증 전 무효 요청 거절; Stage/무효 선택을 이용한 fallback 우회도 거절 |
| `CreateStageProbe:835`, `CallSubmitAsync:853`, `PostJsonAsync:861` | 순수 payload 생성→transport 직접 호출 또는 UnityWebRequest.SendWebRequest | payload 모델은 Deprecated로 보존, raw transport/Stage HTTP helper도 직접 호출 검사; Infinite/계정 권한·timeout·late 보호 유지 |
| `PreparePhase2/ExecutePhase2`, Authenticate/RetryPending | 격리 저장 구성/계정 인증/Pending 공통 경로 | Stage probe 실행과 분리, 기존 자동 실행 금지·PlayMode 격리·민감 로그 보호 유지. 실제 실행하지 않음 |

Stage factory를 호출하는 Test는 `rg`와 파일 순회 읽기 전용 Node 집계로 **10파일/16호출**을 확인했다. 아래 분류는 기존 성공 helper가 어떤 용도인지에 따른 전환 계획이다. Step 1에서 Test를 수정/실행하지 않았다.

| 기존 Test 파일·factory 호출 수 | 용도/후속 처리 | 소유 Step |
| --- | --- | --- |
| EditMode `RecordSubmissionPolicyTests` / 6 | Stage 성공·실패 입력은 지원 종료 false/out null Test로 전환; queue ID/3회/소유/terminal Test는 유효 Infinite fixture로 전환 | 2·4 |
| EditMode `RecordLeaderboardPolicyTests` / 1 | Stage 정렬/동점/독립 Board 순수 모델은 합성 Stage 자료로 보존; 생산 factory 성공 의존 제거 | 2·7 |
| EditMode `LocalSaveJsonCodecTests` / 1 | serialize/legacy 해석은 합성 Stage 형식 보존; 저장/재시작·공개 최고 조회 성공 기대는 Infinite로 전환, Stage 조회 거절 분리 | 2·3·5 |
| EditMode `OnlineLocalSaveScopeTests` / 1 | 저장/수명/환경/귀속/동의 공통 성공 helper는 Infinite 전환; Stage 신규 저장/조회/Retry 거절은 별도 합성 입력으로 실제 guard 도달 | 2·4·5 |
| EditMode `OnlineRecordRepositoryTests` / 1 | Stage Board 직접 생성도 5곳 존재. 정상 제출/조회/timeout/인증/재시도는 Infinite; Stage 직접 호출은 인증/전송 0회와 새 거절값 검증 | 3·4·7 |
| EditMode `OnlineAccountCoordinatorTests` / 1 | 정상 인증/귀속/조회 및 직접 Stage key 2곳은 Infinite 성공/지연 fixture로 전환; snapshot 내부 형식 검증과 분리 | 3·5·7 |
| EditMode `AccountTransferCompletionTests` / 1 | 이전 성공·재시작·terminal/저장 실패·stale 테스트의 일반 Stage 최고/Pending는 Infinite 전환; 전체 snapshot parser의 합성 형식 검증만 별도 보존 | 5 |
| EditMode `AccountTransferControllerTests` / 1 | Pending gate/명시 폐기/잠금·retry의 공통 후보를 Infinite 전환, 실제 파일/계정 호출 없음 | 4·5 |
| EditMode `VerificationSessionTests` / 2 | retry 성공/미확정/저장실패 fixture는 Infinite; Stage retry/probe 거절은 합성 Stage. 비교/StageProbeBlockReason 순수 모델은 보존 | 4·6 |
| PlayMode `AccountTransferUIIsolationTests` / 1 | UI Pending/gate 공통 성공 fixture를 Infinite로 전환; 기존 생산 Scene과 메모리 저장 사용 | 5·7 |

factory 호출이 없는 `OnlineRecordConfigurationTests`의 Stage 매핑, `ResultTextFormatterTests`·ResultData/TimeRecord 등 순수 자료형, VerificationRecordComparison의 합성 Stage baseline은 내부 계약으로 유지한다. `ModeResultDisplayIntegrationTests` 등 이미 Stage 거절을 검사하는 Test는 유지/보완한다. 서버 Node Test의 Stage 성공은 **기존 서버 계약의 로컬 대역**이며 새 Client의 Stage 허용 Test로 해석하거나 일괄 삭제하지 않는다. fixture 전환 시 유효 점수 상한/ID/귀속을 사용하고 공용 helper의 Assert를 제거하지 않는다.

#### 1-4. 변경 전 정적/오프라인 기준선 — 완료

실행 전 `require`/VM loader/SDK 주입/파일·프로세스 호출을 확인했다. 소스 계약 5개는 fs 읽기·assert, preflight의 외부 프로세스는 읽기 전용 `git diff --name-only`/`git ls-files`뿐이다. `account-sdk-double.cjs`는 Map 저장·점수 대역이며 account/legacy/receipt store는 주입된 API를 사용한다. record-account-integration의 실제 endpoint VM loader는 Unity SDK import를 f.save/f.leaderboard로 치환하고 허용하지 않은 dependency는 throw한다. snapshot/transfer endpoint도 store/서비스를 주입하고 Secret은 테스트 상수/콜백을 사용한다. 선택한 서비스 Test에서 SDK 실물·환경 자격 증명·네트워크/배포/Unity 프로세스·실제 사용자 저장을 사용하지 않는 경로를 확인한 후 실행했다.

아래 실행 명령은 모두 `node UGS/Tests/<파일>`이며 Node 버전은 `v24.21.0`이다. **14스크립트 중 13통과/1실패**다. 숫자를 제공하지 않는 assertion 스크립트에 임의 Test 건수를 붙이지 않았다.

| 파일 | 이번 결과 | 판정 범위/한계 |
| --- | --- | --- |
| `client-unity-preflight.cjs` | PASS / 변경·신규 C# 0파일; 기존 선택 suite Edit 183/Play 23 source cases PREPARED | asmdef/격리 setup·소스 준비 검사; 변경 C# loop는 0회, C# Compile/전체 Test 실행 아님 |
| `static-contracts.cjs` | PASS | 서버/Module/Client 소스 assertion, 동작 실행 아님 |
| `client-local-save-contracts.test.cjs` | PASS | v7/격리/수명/gate 소스, Edit 27 prepared 표시는 실행 수 아님 |
| `client-account-contracts.test.cjs` | PASS | 계정/귀속/stale 소스, Edit 30 prepared |
| `client-transfer-completion-contracts.test.cjs` | PASS | 전체 snapshot 교체/세션/격리 소스, Edit 30 prepared |
| `client-verification-tools-contracts.test.cjs` | **FAIL / exit 1** | line 174의 기존 GameSystem null 문법 검사에서 중단; 뒤 assertion 미실행 |
| `record-account-integration.test.cjs` | PASS **46/46** | 실제 submit/query 함수·endpoint VM와 로컬 SDK 대역 |
| `account-personal-bests.test.cjs` | PASS **18/18** | Infinite-only/전체 snapshot/이전·동일 계정/빈 결과/잘못된 응답 대역 |
| `cloud-code.test.cjs` | PASS **28/28** | Infinite 점수/상한·동일 ID·조회/기존 Stage 서버 계약 대역 |
| `leaderboard-query-policy.test.cjs` | PASS | 순수 조회 정책 assertion |
| `submission-retention-policy.test.cjs` | PASS | 순수 180일/terminal replay 정책 assertion |
| `transfer-completion.test.cjs` | PASS **22/22** | 계정 이전 서비스의 메모리 SDK 대역 |
| `transfer-races.test.cjs` | PASS **22/22** | 이전 경합/늦은 응답 서비스 대역 |
| `json-snapshot-consistency.test.cjs` | PASS / **중단 쓰기 경계 30개** | JSON roundtrip·두 legacy Board 서비스 대역, score 쓰기 없음; 30은 Test 개수가 아님 |

실패 근거: `client-verification-tools-contracts.test.cjs:174`의 금지 문법(`System.Linq`, `?.`, `??`) assertion이 `Assets/Scripts/Runtime/Systems/GameSystem.cs`에서 actual=false/expected=true로 실패했다. 해당 소스 검색 결과는 **GameSystem.cs:1143의 `_uiInputSystem?.ConsumeTransientInput();` 1곳**이다. 소스 규칙과 실제 기존 코드의 불일치이며 원격 실패/Unity Compile 실패로 분류하지 않는다. Step 1에서 baseline을 고치거나 assertion을 약화하지 않았다. **Step 8에서 명시적 null 검사로 수정하고 이 스크립트 및 영향 검사 재실행**을 필수 인계한다. Step 6에서 Stage 버튼/probe 성공을 요구하는 옛 소스 assertion도 새 거절 계약으로 전환하되 격리/민감 로그/권한 검사는 유지한다.

P9-C04/05는 위 경로/fixture 표와 후속 guard·Deprecated 확인, C06은 Infinite 대역/후속 Client Test, C07은 ID/3회/180일/commit·실패 보호, C08은 snapshot/이전/환경 격리, C10은 무배포·기존 계약 보존에 연결했다. 로컬 PASS는 새 Client의 Stage 차단 완료·실제 C# 동작·원격 서비스/Player 후보 검증이 아니다. Unity Compile/Runner/빌드/Player·Scene 편집은 실행하지 않았다.

#### Step 1 완료 판정과 다음 작업

호출망·첫 부작용·후속 guard/반환·Test/소유 Step, 내부 형식/복원 보존, 10파일 fixture 전환과 현재 기준선 실패/한계를 기록했으므로 **Step 1을 완료 처리**한다. 기존 실패 기록까지 요구하는 조사 Step의 완료이며 전체 검사 PASS나 Phase 3 접근 차단 완료를 의미하지 않는다. Step 2~11은 미착수이고 다음은 **Step 2 신규 Stage 후보 생성/저장 차단**이다.

**사용자 수동 작업: 없음.** Step 1에 Scene 변경도 없다. 사용자 Compile/필요한 Scene 적용은 Step 9, EditMode/PlayMode 실행은 Step 10, Player 통합 후보 판정은 Phase 4에 유지한다.

## Step 2. Stage 후보 생성과 신규 저장을 차단한다

### AI 작업

2-1. RecordSubmissionPolicy.TryCreateStageCandidate를 지원 종료 경계로 만든다. 정상 값/Stage Cleared도 성공 후보를 반환하지 않도록 하고 선언·이유/Infinite 대체 경로를 보존한다. Infinite 후보의 점수/Scoring Version/상한 검증은 유지한다.

2-2. GameSystem.TryStoreCurrentResultCandidate는 Stage/무효 Mode를 ID 생성·개인 최고·Pending·저장/온라인 처리 전에 거절한다. Scene/런타임의 선행 차단에만 의존하지 않는다.

2-3. RecordSubmissionService와 LocalRecordRepository의 지원되는 신규 저장 API에 합성 Stage 후보를 직접 주입해도 거절되도록 한다. 내부 복원과 공유 메모리 저장소에 같은 API가 쓰이는지 먼저 확인한다. 필요하면 기존 구조 안에서 신규 동작 경계와 내부 복원 책임을 구분하고, Stage 자료 누락/자동 삭제를 만드는 일괄 변경은 하지 않는다.

### 정적 검증·Unit Test

- `RecordSubmissionPolicyTests`, `OnlineLocalSaveScopeTests`, `OnlineRecordRepositoryTests`에 Stage/무효 Mode 거절과 기존 Infinite 후보의 유효/무효 점수·음수/상한·ID/귀속 검증을 연결한다.
- 거절 시 out 후보=null, Pending/최고/제출 상태 불변, 저장 시도/인증/전송 0회를 검사한다. null·다른 소유자·중복 ID의 기존 거절도 유지한다.
- 우회 거절 Test의 합성 Stage 후보는 기존 codec/자료형 또는 Test 전용 reflection으로 준비한다. Test 편의를 위해 생산 Stage 생성 API를 다시 허용하거나 새 공개 우회 factory를 만들지 않는다.
- `LocalSaveJsonCodecTests`의 형식 검증은 생산 Stage 후보 성공 기대와 분리한다. 실제 파일은 사용하지 않는다.

### 사용자 수동 작업

없음.

### 완료 조건

- [x] 신규 Stage 후보/저장 진입을 직접 호출해도 부작용 전에 거절하도록 구현했고 Infinite/내부 자료 보존 Test를 준비했다.

### Step 2 수행 결과 — 2026-11-17

#### 2-1. Stage 후보 factory의 지원 종료 — 완료

`RecordSubmissionPolicy.TryCreateStageCandidate`는 선언/서명을 유지하고 정상 Cleared를 포함해 항상 **false/out null**로 반환한다. Deprecated 사유와 `TryCreateInfiniteCandidate` 대체 경로를 주석으로 표시했다. 기존 호출자가 남아 있어 일괄 Obsolete 경고를 추가하는 대신 사유 주석을 사용한다. Stage 변환 helper/Board/enum과 내부 형식을 보존했고 새로운 공개 우회 factory는 만들지 않았다. Infinite factory의 점수 구성·상한·ScoringVersion·UUID v4/귀속 검증 본문은 변경하지 않았다.

#### 2-2. GameSystem의 ID 생성 전 거절 — 완료

`GameSystem.TryStoreCurrentResultCandidate` 첫 진입에서 null/Stage/무효 Mode Result를 return한다. guard는 `Guid.NewGuid`·최고 조회·service 저장·Result presentation·SaveLocalState보다 앞이다. 기존 Stage 분기는 보존하지만 이 진입에서는 도달하지 못한다. 선행 Scene/Stage 플레이 차단에만 의존하지 않는다.

PlayMode `ResultMenuIntegrationTests.DirectResultCandidate_UnsupportedMode_KeepsRecordsAndPresentation`의 Stage/무효 Mode 2개 case는 기존 생산 Scene에서 실제 GameSystem 메서드를 reflection으로 호출하도록 준비했다. Infinite Run/로컬 저장 자료가 존재하고 중복 후보 guard가 우회된 상태에서 후보 저장소·Result presentation·후보 marker·Runtime이 불변인지 확인한다. ID 발급 전 순서는 별도 소스 계약으로 확인한다. Test Runner는 실행하지 않았다.

#### 2-3. 신규 저장과 내부 복원의 분리 — 완료

- `RecordSubmissionService.TryStoreCandidate`와 Local의 `TryEnqueuePending/TryUpdatePersonalBest`는 null 또는 현재 Infinite Board가 아닌 후보를 false로 거절한다. Mode=Infinite, RulesVersion=ScoringVersion.Current, StageId=빈 문자열을 모두 확인한다. Stage/무효 Mode·구 Version/오염된 Board 후보의 직접 주입도 거절한다.
- service는 repository 호출 전에, Local enqueue는 시각 부여/Memory 변경 전에 거절한다. 이 경계는 인증/transport를 호출하지 않으며 거절을 서버 terminal 영수증으로 만들지 않는다.
- 공유 `MemoryRecordRepository`, `RecordSubmissionQueue`, `RestoreMemoryRecords`, LocalSaveData/JsonCodec와 계정 snapshot parser는 변경하지 않았다. 복원은 기존 Memory 메서드로 직접 수행하며 전체 snapshot의 필터/자료 자동 삭제·Infinite 변환은 추가하지 않았다. 공개 Local 최고 조회·온라인 조회/제출 guard는 Step 3에 남는다.
- `RecordSubmissionPolicyTests`에 service repository 호출 0회, Local fake clock/저장 시도 0회와 기존 최고/Pending/파일 불변 Test를 추가했다. 테스트용 `OnlineTestFileStore.WriteAttempts`로 실제 쓰기 시도를 센다. null·Stage·무효 Mode·구 Infinite Version·StageId 오염 입력을 포함한다.
- `LocalSaveJsonCodecTests.InternalStageFormat_LoadAndCheckpoint_PreserveWholeRecordsWithoutNewStageWrites`는 기존 합성 Stage 자료를 메모리 파일에서 복원하고, 신규 쓰기는 거절하되 checkpoint/재시작 후 기존 최고/Pending/설정이 남는지 검사한다. Stage 보관 기능을 새로 개발한 것이 아니라 공유 복원에 일괄 guard를 넣지 않았는지 확인하는 회귀 Test다.

#### 2-4. 영향 fixture·정적/로컬 검증 및 인계 — 완료

factory 차단이 기존 성공 helper를 깨뜨리므로 Step 1의 10파일을 모두 처리했다. `RecordSubmissionPolicyTests`의 Stage 성공 기대는 거절로 전환하고 queue 공통 fixture는 Infinite로 바꿨다. `OnlineLocalSaveScopeTests`, `OnlineRecordRepositoryTests`, `OnlineAccountCoordinatorTests`, `AccountTransferControllerTests`, `VerificationSessionTests`, PlayMode `AccountTransferUIIsolationTests`의 일반 후보도 유효 Infinite로 전환했다. ID·귀속·중복/실패 assertion은 유지했다.

`AccountTransferCompletionTests`는 정상 snapshot/캐시/Pending를 Infinite로 맞추고 서버 최고 500·recipient 최고 1000을 사용해 **recipient가 더 높은 점수를 갖고 있어도 연결 계정 최고로 교체**되는 의미를 유지했다. Stage가 사라지는 전체 snapshot Test는 내부 합성 Stage 캐시를 먼저 seed하여 기존 검증을 유지한다. `RecordLeaderboardPolicyTests`의 순수 Stage 정렬 및 `LocalSaveJsonCodecTests`의 내부 형식 fixture는 비공개 constructor reflection으로 준비한다. 성공을 위해 생산 factory를 열거나 assertion을 제거하지 않았다.

추가 준비 case는 **EditMode 17개 / PlayMode 2개**다(소스 attribute 기준, 실제 실행/통과 수 아님). EditMode는 Infinite 음수/구성 합계·ID/소유자 입력 6개, service 거절 5개, Local 거절 5개, 내부 Stage 복원 1개다. 기존 Infinite 유효/상한 초과·Version·누락 limit·중복 ID/다른 소유자·재시도 Test는 유지한다. `ScoringVersion` 사용 파일의 Core using, reflection의 실제 비공개 constructor 12인자/ResultData backing field·GameSystem 필드/메서드, asmdef/meta/GUID·선언/최초 사용 순서를 정적으로 대조했다.

| 이번 변경 범위 | 파일 |
| --- | --- |
| 생산 C# 4개 | RecordSubmissionPolicy, RecordSubmissionService, LocalRecordRepository, GameSystem |
| EditMode C# 10개 | RecordSubmissionPolicyTests, RecordLeaderboardPolicyTests, LocalSaveJsonCodecTests, OnlineLocalSaveScopeTests, OnlineRecordRepositoryTests, OnlineAccountCoordinatorTests, AccountTransferCompletionTests, AccountTransferControllerTests, VerificationSessionTests, OnlineTestFileStore |
| PlayMode C# 2개 | AccountTransferUIIsolationTests, ResultMenuIntegrationTests |
| Node 소스 계약 2개 | client-local-save-contracts.test.cjs, client-verification-tools-contracts.test.cjs |
| 문서 | 이 Task, Roadmap 9, Project Memory, RecordSubmissionSystem, RecordSubmission |

Node 실행 전의 SDK 대역/읽기 전용 감사는 1-4와 동일한 스크립트 본문/의존성을 대조했다. 수정한 두 Node 파일도 fs 읽기/assert만 수행한다. 실행 명령은 `node UGS/Tests/<파일>`이며 최종 결과는 아래와 같다.

| 스크립트 | 결과/범위 |
| --- | --- |
| client-unity-preflight.cjs | PASS / 변경 C# 16파일의 중복 선언·소스 괄호·meta/GUID, asmdef/격리 setup. 선택된 기존 suite Edit 183/Play 23 PREPARED 표시는 전체 실행 수가 아님 |
| static-contracts.cjs | PASS / 기존 서버/Client/Module 소스 계약 |
| client-local-save-contracts.test.cjs | PASS / v7/격리·복원 및 Step 2 factory·신규 쓰기/ID 발급 전 guard 순서·준비 Test 소스 계약 |
| client-account-contracts.test.cjs | PASS / 기존 계정·귀속/stale 소스 계약 |
| client-transfer-completion-contracts.test.cjs | PASS / 전체 snapshot 교체/내구 저장 소스 계약 |
| client-verification-tools-contracts.test.cjs | PASS / Infinite fixture의 소유 ID 초기화 순서, 권한/격리/로그/도구 소스 계약 |
| record-account-integration.test.cjs | PASS 46/46 / 기존 실제 서비스 함수·endpoint의 로컬 SDK 대역 |
| account-personal-bests.test.cjs | PASS 18/18 / Infinite-only/전체 snapshot·이전·격리 로컬 SDK 대역 |

검증 중 실패와 조치: 최초 관련 8스크립트 실행은 7통과/1실패로, 1-4의 기존 `GameSystem` null 조건부 문법 위반이었다. 이번 변경 파일의 소스 검사를 통과시키도록 해당 한 줄을 동작이 같은 명시적 null 검사로 바꾸고 검증 도구 계약/preflight를 재실행해 통과했다. 소스 계약을 추가하는 과정에서 `client-local-save-contracts`의 `const game` 중복 선언도 발견해 기존 변수를 재사용하도록 수정했고 해당 파일 재실행은 통과했다. 검사 조건을 삭제/약화하지 않았다. Step 1에서 Step 8로 인계했던 기존 문법 실패는 **Step 2에서 해결**했다.

소스 검사는 C# 이름 해석/Compile이나 Unit Test 동작 PASS를 증명하지 않는다. 46/18은 서비스 JS의 오프라인 대역 결과이며 C# 실행 건수가 아니다. Unity Compile/Test Runner/빌드/Player·실제 저장/인증/원격 요청·Scene 편집은 수행하지 않았다.

**완료 판정:** Step 2의 기준인 부작용 전 신규 Stage 후보/저장 거절 구현과 Infinite/내부 자료 보존 Test 준비·정적 검증을 마쳤으므로 Step 2를 완료 처리한다. 다음은 **Step 3 직접 Repository 조회/제출·전송 우회 차단**이다. Step 3~11 및 Phase 3 전체는 미완료다.

**사용자 수동 작업: 없음. 추가 Scene 작업도 없음.** Compile/필요한 Scene 적용은 Step 9, 최신 EditMode/PlayMode 실행은 Step 10에 유지한다. 현재 919/288 결과나 Candidate02를 이번 변경의 실행 근거로 승계하지 않는다.

## Step 3. 직접 Repository 조회·제출과 전송 우회를 차단한다

### AI 작업

3-1. CloudCodeRecordRepository의 SubmitAsync·Top/Around/PersonalBest 공통 조회 진입에 Stage/무효 Board 거절을 적용한다. 기존 반환 자료형으로 구분 가능한 안전한 미지원 사유를 사용하고, 정확한 사유/로그 계약을 이 Task에 확정한다. Stage를 Infinite Board로 바꿔 대신 실행하지 않는다.

3-2. 지원되는 로컬/메모리 조회의 실제 사용자 노출 경계도 대조한다. 내부 codec/보관 snapshot의 형식 보존과 사용자 개인 최고 조회를 구분한다. 공유 snapshot을 무조건 필터링해 다음 저장에서 원본 항목을 지우지 않는다.

3-3. raw record endpoint 요청과 Editor의 Module/HTTP 호출이 Repository를 우회할 수 있는지 확인한다. 도달 가능한 경로는 기존 transport/도구 진입의 가장 이른 적절한 경계에서 차단한다. 계정 endpoint나 서버 snapshot 응답까지 일괄 금지하는 범용 필터를 추가하지 않는다.

### 정적 검증·Unit Test

- `OnlineRecordRepositoryTests`에서 Stage Top/Around/Me/Submit·무효 Board를 직접 호출하고 가짜 인증 횟수=0, 가짜 전송 횟수=0, 로컬 변경=0과 안전한 반환값을 검사한다. 이미 인증/동의가 있는 경우에도 거절돼야 한다.
- Infinite는 기존 `submit-record`/`query-records`, Board/Version, Top=10/Around=7/Me, 요청 payload와 응답/공개 번호 검증을 유지한다. timeout·인증 실패·stale 계정 응답도 회귀한다.
- `OnlineRecordConfigurationTests`에서 내부 Stage 식별자 보존을 실행 허용과 혼동하지 않는다. 전송 guard를 추가하면 raw 요청의 거절/정상 Infinite 통과도 SDK 없이 검증한다.
- 남은 모든 Stage 원격 요청 호출자가 guard/Test 표에 연결되는지 정적으로 다시 검색한다.

### 사용자 수동 작업

없음. 실제 Stage/Infinite 서버 요청으로 차단 여부를 시험하지 않는다.

### 완료 조건

- [x] 지원되는 Stage 직접 조회/제출 경로가 인증/전송 전에 거절되고 Infinite 요청 계약의 회귀 Test를 준비했다.

### Step 3 수행 결과 — 2026-11-17

#### 3-1. Repository의 직접 Stage 조회/제출 거절 — 완료

`CloudCodeRecordRepository.SubmitAsync`와 Top/Around/PersonalBest 공통 `QueryAsync`에 Stage guard를 추가했다. guard는 Board 매핑·계정 callback/동의/인증·transport 호출보다 앞이다. 이미 인증된 경로에서도 Stage 요청을 Infinite로 바꾸지 않고 거절한다. 기존 자료형을 사용하며 새 서버 상태/endpoint는 추가하지 않았다.

| 호출 | 반환/예외·사유 | 부작용/로그 계약 |
| --- | --- | --- |
| Stage SubmitAsync | OnlineSubmissionResult(TransientFailure, StageModeUnsupported) | 인증/전송 없음; 서버 Rejected/Submitted 영수증으로 취급하지 않음 |
| Stage Top/Around/PersonalBest | status=TransientFailure, reason=StageModeUnsupported, 빈 entries | 인증/전송 없음; 성공/Empty 조회로 표시하지 않음 |
| 기타 무효 Board 제출/조회 | 기존 Rejected/InvalidBoard 또는 TransientFailure/InvalidQuery | 기존 반환 계약 유지, 인증/전송 없음. coordinator의 사전 검사는 Step 4 소유 |
| Local Stage/무효 최고 조회 | false/out null | Memory 조회/저장·전체 snapshot 변경 없음 |
| raw submit-record/query-records의 Stage Board | NotSupportedException(StageModeUnsupported) | SDK/진단 순번 변경·위임 전 거절 |
| raw record의 잘못된 JSON·Board·제출 Version·조회 kind/limit | ArgumentException(InvalidRecordRequest) | SDK/진단 순번 변경·위임 전 거절, 요청/자격 증명 원문 미출력 |
| 보존된 Editor Stage HTTP helper 직접 호출 | NotSupportedException(StageModeUnsupported) | 자동 실행 판정·deadline/UnityWebRequest 생성·SendWebRequest 전에 거절 |

지원 종료 거절 자체는 Warning/Error를 새로 출력하지 않는다. 기존 Infinite timeout/서비스 실패의 안전 로그·권한 거절 계약은 유지한다. Stage Mode 판정은 BoardKey.GameMode 기준이며 null/다른 소유자의 기존 TransientFailure도 유지한다. Stage 후보가 들어오면 소유자와 무관하게 지원 종료 거절이 우선한다.

#### 3-2. 사용자 로컬 최고 조회와 내부 형식 보존 — 완료

`LocalRecordRepository.TryGetPersonalBest`는 현재 Infinite Board만 허용한다. Stage/무효 Mode·구 Version/오염된 StageId는 out null/false로 Memory 조회 전에 거절한다. 생산 호출자 검색 결과 해당 조회는 GameSystem의 현재 후보 최고 비교에 사용되며, 공유 Memory의 동일 메서드는 Local의 위임과 모델 Test에서 사용한다. public 선언만 보고 공유 모델/복원까지 일괄 차단하지 않았다.

Memory/queue, `CreateSaveData/Create*Snapshot`, LocalSaveData/JsonCodec와 `OnlineAccountCoordinator.TryReadSnapshot`은 변경하지 않았다. `LocalSaveJsonCodecTests`의 사용자 최고 조회 성공 fixture만 Infinite로 바꾸고, 합성 Stage 내부 복원 Test에는 **사용자 조회는 거절하되 checkpoint/재시작 후 Stage 최고/Pending는 보존**되는 assertion을 연결했다. `OnlineRecordConfigurationTests`의 Stage Board 매핑 성공은 유지하며, 그 ID의 실행 요청은 별도로 거절함을 검사한다. 서버 Stage/Infinite 전체 snapshot도 임의 필터링하지 않는다.

#### 3-3. raw Module SDK/Editor HTTP 전송 우회 거절 — 완료

기존 `OnlineRecordConfiguration`에 `ValidateRecordRequest(endpoint, request)`를 추가했다. 식별자/endpoint 계약 책임 안에서 작은 공통 검사로 재사용하며 새 정책 클래스는 만들지 않았다. 검사 대상은 **submit-record/query-records 두 endpoint뿐**이다. JsonUtility로 요청을 해석해 Stage Board 접두사를 거절하고 현재 Infinite Board/제출 Version·조회 kind/limit를 확인한다. 제출 점수·상한·귀속·ID의 최종 서버 검증을 대체하거나 응답 snapshot을 필터링하지 않는다. 허용한 Infinite 요청 문자열/계정 파라미터는 재작성하지 않는다.

- `UgsOnlineRecordTransport.CallEndpointAsync`에서 공통 검사를 Module 함수 매핑·요청 순번 증가·진단 변경·SDK 호출보다 앞에 둔다. string overload와 dictionary의 `request` 경로가 모두 같은 guard를 통과한다. request 누락도 record endpoint라면 거절한다.
- `VerificationRemoteGuard` 두 overload도 기존 권한 검사 다음, delegate/timeout wrapper 실행 전에 같은 검사를 수행한다. 권한이 없는 기존 호출은 기존 InvalidOperationException 계약을 유지한다. get-account-personal-bests 등 계정 endpoint는 record 검사를 건너뛰며, 파라미터/계정 전체 응답 계약은 그대로다.
- `Assets/Editor/OnlineRecordVerificationWindow.PostJsonAsync`는 실제 보존된 `/leaderboards/fs-stage-` URL 경로를 가장 앞에서 거절한다. Stage Leaderboard 요청을 먼저 하던 legacy/direct-write 경로는 이 예외로 다음 CloudSave 쓰기도 진행하지 못한다. 일반 HTTP helper/계정 endpoint를 전부 막는 범용 필터는 넣지 않았다.

최종 소스 검색에서 실제 Module SDK 호출은 Ugs transport의 1곳, HTTP 전송은 위 PostJsonAsync의 1곳에 모이며 각각 guard/소스 계약에 연결했다. 남아 있는 Stage 참조는 다음 소유 범위로 구분한다.

| 남은 호출/참조 | 현재 차단 위치·남은 작업 |
| --- | --- |
| GameSystem의 보존된 Stage 후보 분기 | Step 2 ID 생성 전 guard 및 factory false/out null로 도달 차단 |
| Editor Query(false), VerifyServerValidation→CallSubmitAsync, ProbePhase2 2/5/6 | Repository/raw SDK 전송은 이번에 거절. busy/인증/기준선/격리 저장 전의 진입 guard와 버튼/번호 처리는 **Step 6** |
| Probe 0/1/7·SelectedBoard의 Stage 선택, VerifyAccountMismatch | raw/Repository는 거절. 인증/프로필 변경 이전의 Stage 선택 거절은 **Step 6** |
| VerifyDirectWriteDenied, ProbePhase2DirectWrites/probe 3 | 실제 Stage HTTP helper는 거절. token 조회/인증 전 직접 메서드·probe guard는 **Step 6** |
| OnlineRecordCoordinator 및 VerificationPendingRetry | Repository는 Stage를 terminal로 반환하지 않으므로 전송/terminal 정리는 차단됨. 선행 checkpoint/만료/Bind/Retry 부작용 전 거절은 **Step 4** |
| Stage Board/enum·codec·계정 전체 snapshot·합성 formatter/정렬 | 내부 형식 보존. snapshot/이전 회귀는 **Step 5**, 사용자 Result/표시 통합은 **Step 7** |

따라서 이번 완료는 **직접 Repository/로컬 조회 및 공통 전송 경계**의 완료다. Editor에서 인증이 먼저 발생하는 현행 호출자나 coordinator가 이미 수행한 로컬 저장까지 모두 차단했다고 기록하지 않는다. 후속 Step의 수명/gate/UI 정책을 앞당겨 추가하지 않았다.

#### 3-4. Test 준비·정적/오프라인 검증 — 완료

추가 소스 case는 **EditMode 30개 / PlayMode 1개**이며 실제 실행 수가 아니다.

| suite | 이번 준비/조정 |
| --- | --- |
| OnlineRecordRepositoryTests | 6개 추가: 동의 전/기존 인증 후 Stage Top/Around/Me/Submit의 가짜 인증·전송 0회/로컬 파일·저장 시도·최고/Pending 불변, 무효 Board 3종의 거절/로컬 조회 불변, Infinite 제출 payload 전 필드·Top=10/Around=7/Me=1 요청 계약 |
| CloudCodeModuleRoutingTests | 17개 추가: Stage raw string/dictionary×2 endpoint, null/빈 값/잘못된 JSON/Board/조회 조건·request 누락·구 제출 Version의 SDK 전 거절/LastDiagnostic·순번 보존, 정상 Infinite raw 검사/계정 endpoint 제외 |
| VerificationSessionTests | 7개 추가: 권한이 있는 guard에서 Stage 두 overload 위임 0회, Infinite 두 overload의 요청/파라미터 보존, 계정 snapshot endpoint 파라미터 보존. 기존 권한·timeout·late Test 유지 |
| VerificationToolIsolationTests | 1개 추가: 실제 Editor static HTTP helper를 합성 URL/빈 token으로 직접 호출하면 자동 Test 격리 검사보다 먼저 Stage 미지원 예외를 반환하도록 준비 |
| OnlineAccountCoordinatorTests / 기존 OnlineRecordRepositoryTests | Stage query 성공/timeout/authentication/stale fixture를 Infinite Board로 전환(각 2곳/5곳). 원래 성공·실패 assertion을 제거하지 않음 |
| LocalSaveJsonCodecTests / OnlineRecordConfigurationTests | 공개 최고 조회 성공 fixture만 Infinite로 전환하고 내부 Stage 복원은 보존. Stage ID 매핑 성공과 raw 실행 거절을 분리 |

추가한 SDK 전 거절 Test는 초기화/인증 없는 transport 인스턴스에서 잘못된 요청만 호출하며 guard 이전에 SDK를 실행하지 않는다. 정상 Infinite raw 통과는 공통 검사와 가짜 전송 wrapper로 검증 준비했고 실제 SDK 정상 요청은 실행하지 않는다. PlayMode Test는 Scene/UI를 생성·편집하지 않으며 Editor helper의 지원 종료 경계만 reflection으로 호출한다. 반사 메서드/필드·constructor 인자·Core using·async 반환형·asmdef/meta/GUID·변수 선언 순서를 정적으로 대조했다.

실행 전 기존 읽기 전용/SDK 대역 의존성을 1-4와 다시 대조했다. 수정한 Node 파일은 소스 읽기/assert와 읽기 전용 git만 사용한다. 명령은 모두 `node UGS/Tests/<파일>`이며 아래 **10스크립트가 모두 통과**했다.

| 스크립트 | 이번 결과·한계 |
| --- | --- |
| client-unity-preflight.cjs | PASS / 누적 변경 C# 24파일 소스/중복 선언/meta/GUID·asmdef/격리. 선택 suite Edit 207/Play 24 PREPARED 표시는 실행 수가 아님 |
| static-contracts.cjs | PASS / 기존 서버/Client/Module 소스 계약 |
| client-local-save-contracts.test.cjs | PASS / v7·복원과 Step 2 생성/저장 경계 소스 계약 |
| client-account-contracts.test.cjs | PASS / 기존 계정/귀속/stale 소스 계약 |
| client-transfer-completion-contracts.test.cjs | PASS / 전체 snapshot 교체/내구 저장 소스 계약 |
| client-verification-tools-contracts.test.cjs | PASS / 기존 권한/격리/민감 로그와 Step 3 Repository/SDK/양쪽 wrapper/HTTP guard의 선후 관계. 선택 Test 소스 Edit 67/Play 15 PREPARED |
| record-account-integration.test.cjs | PASS 46/46 / 기존 실제 서비스 함수/endpoint의 로컬 SDK 대역 |
| account-personal-bests.test.cjs | PASS 18/18 / Infinite-only/전체 snapshot·계정 이전 로컬 SDK 대역 |
| cloud-code.test.cjs | PASS 28/28 / Infinite 점수·ID·조회 및 기존 서버 계약 로컬 대역 |
| leaderboard-query-policy.test.cjs | PASS / 순수 조회 정책 assertion |

이 단계의 실행 검사 실패는 없다. preflight의 선택 suite 예상 수는 추가된 Test attribute 수(기존 183/23→207/24)에 맞춰 갱신했으며 검사 조건을 삭제하지 않았다. Node 결과는 C# 이름 해석/Compile/Unit Test나 원격 서비스 성공을 증명하지 않는다. 최신 Unity Compile/Runner/빌드/Player·실제 저장/인증/원격 제출/계정 이전은 수행하지 않았다.

**완료 판정:** Stage 직접 조회/제출 및 raw 전송 우회를 해당 부작용 전에 거절하는 코드와 Infinite/내부 형식 보존 Test 준비·정적/로컬 검증을 마쳤으므로 Step 3을 완료 처리한다. 다음은 **Step 4 Infinite Pending/재시도/만료·저장 보호**다. Step 4~11과 Phase 3 전체는 미완료다.

**사용자 수동 작업: 없음. 추가 Scene 작업도 없음.** 최신 Compile/필요한 Scene 적용은 Step 9, EditMode/PlayMode 실행은 Step 10에 유지한다. 기존 919/288·Candidate02를 이번 소스의 실행 근거로 승계하지 않는다.

## Step 4. Infinite Pending·재시도·만료·저장 보호를 유지한다

### AI 작업

4-1. OnlineRecordCoordinator의 개별/전체 Retry, 부팅/온라인 복구/인증 후 재전송과 VerificationPendingRetry의 호출자를 대조한다. 합성 미지원 후보의 직접 재전송도 Stage 원격 호출을 만들지 않도록 한다.

4-2. 미지원 로컬 요청을 서버 Submitted/Rejected 응답으로 가장하지 않는다. 이를 근거로 기존 자료를 삭제하거나 전송 성공·제출 건수 증가를 표시하지 않는다. 예상 밖 자료 처리의 변경이 필요하면 부작용 없는 거절로 제한하고 Stage 큐 이관/전용 수명/사용자 폐기 정책을 추가하지 않는다.

4-3. Infinite의 동일 Submission ID·최대 3회 재시도·중복 실행 잠금·terminal 응답과 로컬 commit 순서·180일 만료를 유지한다. 저장 실패/timeout/귀속 또는 환경 불일치 시 보존되는지 확인한다.

### 정적 검증·Unit Test

- `OnlineRecordRepositoryTests`, `OnlineLocalSaveScopeTests`, `VerificationSessionTests`에 fake clock/delay/file store/transport를 사용한다. 실제 대기 1/2/4초나 180일을 기다리지 않는다.
- transient→성공, 연속 transient 3회, Rejected, terminal 뒤 저장 실패, 중복 Retry 요청, 단일 ID만 Retry, 180일 직전/정확한 경계/이후, 다른 소유자/환경을 검증한다.
- 제출/조회 Retry를 구분하고 Pending 수 0을 제출 성공으로 간주하지 않는다. Stage 거절 시 저장/인증/전송/terminal 정리·UI 집계에 새 부작용이 없는지 검사한다.
- 기존 영구 저장 실패·재시작 복원 Test를 유지하고 테스트용 저장소에서만 실행한다.

### 사용자 수동 작업

없음. 실제 Pending 생성/삭제·오프라인 반복 조작·시간 변경을 요구하지 않는다.

### 완료 조건

- [x] Stage 재전송 우회가 차단되고 Infinite 재시도/만료/내구 저장의 기존 계약을 검증할 Test를 준비했다.

### Step 4 수행 결과 — 2026-11-17

#### 4-1. 호출자 대조와 재전송 전 검사 — 완료

GameSystem의 부팅/온라인 복구·동의 후 `RetryPendingAsync`, 현재 Result의 `RetrySubmissionAsync`, Leaderboard/계정 패널의 `RetryAllPendingAsync`와 Editor의 동일 경로를 대조했다. 세 coordinator API는 모두 semaphore를 거쳐 `ProcessPendingAsync`로 합류한다. Editor probe 8은 `VerificationPendingRetry.RetryOnceAsync`를 호출한다. 독립적인 조회 Retry와 제출 Retry는 합치지 않았다.

`OnlineRecordCoordinator.CanProcessPending`는 파일/인증/전송 없이 로컬 계정·현재 환경과 전체 Pending의 소유자/현재 Infinite Board를 확인한다. 검사 위치는 **시계 읽기→TryExpirePending→TryCheckpoint→Bind 인증보다 앞**이다. 단일 ID Retry도 만료/checkpoint가 전체 큐를 다루므로 전체 snapshot을 먼저 확인한다. 미지원·다른 소유자/환경이 있으면 해당 trigger를 그대로 종료하며, 지원 항목만 골라 저장하거나 Stage 항목을 삭제/변환하지 않는다. 예상 밖 자료에 대한 단순 거절이며 Stage 큐 분리/혼합 호환/이관·전용 수명 정책은 추가하지 않았다.

Bind의 await 뒤와 각 제출 시도 전에도 같은 지원/귀속 경계를 재검사한다. 각 시도는 로컬 저장 준비 상태도 확인한다. 기존 semaphore의 자동 trigger 병합·명시 Retry 대기/순차 실행은 유지한다. 명시적 동의 저장/계정 연결 API 자체는 후보 재전송과 구분되는 작업이므로 그 기능을 제거하지 않았다. Editor가 재시도 API 호출 이전에 수행하는 계정/도구 준비의 차단은 Step 6 범위다.

#### 4-2. 미지원 결과와 원본/집계 보존 — 완료

| 경계/입력 | 결과 | 보존/호출 계약 |
| --- | --- | --- |
| coordinator의 Stage/무효 Board·다른 소유자/환경 | Task 정상 완료/no-op | 시계/만료/checkpoint/인증/전송/지연 호출 없음, Pending/최고/파일·Submitted/Rejected/terminal 영수증 불변 |
| VerificationPendingRetry의 Stage | PENDING_STAGE_UNSUPPORTED | checkpoint/인증/전송·Pending 삭제 없음 |
| 같은 helper의 다른 미지원 Board | PENDING_RECORD_UNSUPPORTED | 동일하게 부작용 전 거절 |
| 같은 helper의 다른 소유자 | 기존 PENDING_OWNER_MISMATCH | checkpoint 전에 거절 |
| 같은 helper의 다른 환경/동의 없음 | 기존 PENDING_LOCAL_NOT_READY | checkpoint 전에 거절 |
| 같은 helper의 Pending 0건/복수 건 | 기존 PENDING_REQUIRES_EXACTLY_ONE | checkpoint 전 종료; 건수 0을 제출 성공으로 표시하지 않음 |

VerificationPendingRetry는 local/online·동의/환경→건수/소유/지원 Board→checkpoint/CanUseOnlineData→제출 순서로 변경했다. 지원되는 Infinite만 기존 단일 전송을 진행한다. Submitted 확인 뒤의 binding/stale 검사와 저장 성공 후 Pending 정리는 유지한다. Rejected/Transient를 제출 성공으로 간주하지 않는다. 지원 종료 사유는 서버 Rejected/Submitted 응답으로 가장하지 않으며 새 Warning/Error·민감 자료 로그도 추가하지 않았다.

#### 4-3. Infinite 재시도·수명·내구 저장 Test — 완료

coordinator 생성자에 마지막 선택 인자 `Func<long> now`를 추가해 시계를 주입할 수 있게 했다. 생산 기본값은 기존과 같은 `DateTimeOffset.UtcNow.ToUnixTimeMilliseconds`이며 기존 호출의 인자/생명주기를 유지한다. Stage 거절 Test에서는 이 시계조차 읽지 않는지 확인한다. `CanUseOnlineData`를 checkpoint 앞에 일괄 배치하지 않아 저장 실패 뒤 정상 checkpoint로 복구하는 기존 경로도 유지했다.

동일 Submission ID, trigger당 최대 3회, 두 추가 시도 전 지연 **1000/2000ms**, 중복 실행 잠금, terminal 응답 후 **로컬 commit 성공→영수증/집계 반영**, 180일 만료와 저장 실패 보호를 보존했다. 4초 대기나 네 번째 시도를 새로 넣지 않았다. 계산·파일 정책·서버 receipt 수명 코드는 변경하지 않았다.

| suite | 추가 준비 case | 내용 |
| --- | --- | --- |
| OnlineRecordRepositoryTests | 13 | Stage/구 Infinite Version×background/single/all 6개에서 시계·지연·저장·인증·전송 0회/terminal 집계·원본/재시작 불변; 다른 소유자/환경의 만료 대상 자료 보호 2개; transient→성공의 동일 ID/백오프 1개; 진행 중 중복 background 병합 1개; Submitted/Rejected 뒤 저장 실패·동일 ID 재시도 후만 집계 2개; 만료 후 Pending 0이 제출 성공이 아님 1개 |
| OnlineLocalSaveScopeTests | 3 | 가짜 시계로 180일 직전/정확한 경계/직후를 검사하고 최고 기록·설정/튜토리얼·재시작 보존 확인 |
| VerificationSessionTests | 6 | Stage/구 Version의 checkpoint·인증/전송 0회 2개; 다른 소유자/환경의 쓰기 전 거절 2개; Submitted 후 저장 실패→같은 ID로 복구 1개; Pending 0은 성공이 아니며 checkpoint도 없음 1개 |

합계 **EditMode 22개 case 추가 준비**이며 실제 Runner 실행/통과 수가 아니다. `OnlineTestTransport.BeforeResponse` 콜백과 기존 메모리 file store/카운터·TaskCompletionSource·즉시 완료 delay를 사용한다. 테스트용 합성 Stage/구 Board는 기존 codec/비공개 constructor로 복원하며 생산 factory를 다시 허용하지 않는다. 실파일·기기 저장·네트워크·실제 시간 대기는 사용하지 않는다. 만료 Warning은 해당 정상 Infinite 만료 Test에만 정확한 LogAssert.Expect를 사용한다.

기존 `RetryHasThreeAttemptsWithBackoff_AndNextTriggerResetsBudget`, 단일 ID Retry, recovery와 명시 Retry의 순차 실행, RetryAll의 transient 후 다음 후보 진행, Submitted/Rejected terminal, timeout/재시작 ID 보존·checkpoint/제거 저장 실패 Test를 유지했다. Local의 기존 만료 저장 실패 Test와 환경/귀속·계정 gate 회귀도 유지했다. 새 Test에서 실제 coordinator/Local repository/codec을 사용하며 정책을 테스트 코드에 다시 구현하지 않았다.

#### 4-4. 정적/로컬 검증과 다음 인계 — 완료

생산 수정은 `OnlineRecordCoordinator`, `VerificationPendingRetry` 2파일이다. Test는 위 3개 suite와 `OnlineTestTransport` 4파일, Node는 기존 local-save/verification 소스 계약과 preflight 3파일을 수정했다. 기본 시계·선택 인자 호출자, 반사 constructor 12인자와 실제 필드/메서드 이름·Core using·변수 최초 선언/async 반환형·asmdef/meta/GUID를 정적으로 확인했다. Scene/Prefab·enum/저장 형식/Board·서버/Module은 수정하지 않았다.

수정한 Node 소스 계약은 CanProcessPending의 순수 검사와 시계/만료/checkpoint/Bind·전송·terminal commit 순서, 단일 검증 재전송의 Stage/소유/환경/건수 검사 순서를 확인한다. 기존 권한/격리·민감 로그/timeout·복원 계약을 제거하지 않았다. preflight 선택 suite의 소스 case 수는 207/24→**216/24**로 갱신했다.

실행 전 소스 읽기/읽기 전용 git·SDK 주입/VM loader·메모리 저장 대역을 1-4 기준으로 대조했다. `submission-retention-policy`는 순수 assertion이며 실제 시간/원격 호출이 없다. 명령은 `node UGS/Tests/<파일>`이고 **아래 10스크립트가 모두 통과**했다. 이번 실행 검사 실패는 없다.

| 파일 | 이번 결과/범위 |
| --- | --- |
| client-unity-preflight.cjs | PASS / 누적 변경 C# 27파일 소스/중복 선언/meta/GUID·asmdef/격리; 선택 Edit 216/Play 24 PREPARED는 실제 실행 수가 아님 |
| static-contracts.cjs | PASS / 기존 서버/Client/Module 소스 계약 |
| client-local-save-contracts.test.cjs | PASS / 기존 저장/Step 2 및 Step 4의 순수 gate·만료/인증 전 순서·시계 주입·terminal 내구 저장 계약 |
| client-account-contracts.test.cjs | PASS / 기존 계정/귀속/stale 소스 계약 |
| client-transfer-completion-contracts.test.cjs | PASS / 전체 snapshot 교체/내구 저장 소스 계약 |
| client-verification-tools-contracts.test.cjs | PASS / 기존 격리/권한/로그·Step 3/4의 전송/단일 재시도 guard; 선택 Edit 73/Play 15는 준비 소스 수 |
| submission-retention-policy.test.cjs | PASS / 180일 terminal replay·Pending 수명 순수 정책 assertion |
| record-account-integration.test.cjs | PASS 46/46 / 기존 실제 서비스 함수/endpoint의 로컬 SDK 대역 |
| account-personal-bests.test.cjs | PASS 18/18 / Infinite-only/전체 snapshot·이전 로컬 SDK 대역 |
| cloud-code.test.cjs | PASS 28/28 / Infinite 점수/ID·조회 및 기존 서비스 계약 로컬 대역 |

Node PASS는 C# 이름 해석/Compile/Unit Test 실행이나 원격 서비스 성공이 아니다. Unity Compile/Runner/빌드/Player·실제 저장/인증/제출/계정 이전은 수행하지 않았다.

**완료 판정:** Stage 재전송을 저장/인증 전에 거절하는 코드와 Infinite 재시도/만료/내구 저장 Test 준비·정적/로컬 검증을 마쳤으므로 Step 4를 완료 처리한다. 다음은 **Step 5 Infinite 계정 이전/개인 최고 snapshot 보존**이다. Editor probe의 인증 전 진입 차단은 Step 6에 남아 있고 Step 5~11·Phase 3 전체는 미완료다.

**사용자 수동 작업: 없음. 추가 Scene 작업도 없음.** 실제 Pending 생성/삭제·기기 시간 변경·빠른 중복 입력 재현은 요구하지 않는다. 최신 Compile/필요한 Scene 적용은 Step 9, EditMode/PlayMode 실행은 Step 10에 유지한다.

## Step 5. Infinite 계정 이전과 개인 최고 snapshot을 보존한다

### AI 작업

5-1. AccountTransferPendingGate·LocalRecordRepository의 이전 시작/완료·진행 중 저장 잠금과 OnlineAccountCoordinator의 응답 반영 순서를 대조한다. Infinite Pending 0건·활성 연결·내구 저장 조건을 유지한다.

5-2. 계정 전체 최고 snapshot의 내부 해석과 사용자 Stage 조회 API 거절을 분리한다. Infinite-only/빈 snapshot의 정상 처리, 불완전/잘못된 응답·중복 보드·공개 번호·오래된 응답 거절을 유지한다. Stage 형식이 남았다는 이유만으로 공통 parser를 제거하지 않는다.

5-3. 이전 recipient의 기존 다른 계정 최고/Pending과 합산하지 않고 새 연결 계정의 전체 최고로 교체하는 계약을 유지한다. Settings/Tutorial·비활성 환경 영역·공개 번호/로컬 소유/서버 활성 ID의 역할을 보존한다.

### 정적 검증·Unit Test

- `AccountTransferCompletionTests`, `AccountTransferControllerTests`, `OnlineAccountCoordinatorTests`, `OnlineLocalSaveScopeTests`, `LocalSaveJsonCodecTests`를 실제 생산 coordinator/codec과 메모리 저장소로 검증한다.
- Pending>0 이전 거절/인증·전송 0회, Pending=0 성공, 저장 실패 시 원래 계정/최고 보존, stale 응답/진행 중 중복 거절, snapshot 교체·재시작·환경 전환을 확인한다.
- `UGS/Tests/account-personal-bests.test.cjs`, `record-account-integration.test.cjs` 등의 Infinite 대역 계약과 연결한다. 원격 두 기기 이전이나 Stage 자료 보관 기능을 새로 요구하지 않는다.

### 사용자 수동 작업

없음. 실제 계정 이전/복구 코드 발급·기기 저장 삭제를 하지 않는다.

### 완료 조건

- [x] Infinite 이전/snapshot/저장 계약이 유지되고 Stage 접근 차단이 내부 읽기·계정/환경 데이터를 파괴하지 않는 Test를 준비했다.

### Step 5 수행 결과 — 2026-11-17

#### 5-1. Pending gate·활성 연결·내구 저장 순서 대조 — 완료

생산 코드를 읽고 다음 순서를 확인했다. 기존 구조가 Step 5 계약을 충족하므로 생산 변경은 추가하지 않았다.

| 경계 | 확인한 순서/보존 계약 | 회귀 근거 |
| --- | --- | --- |
| AccountTransferPendingGate.TryRequestAsync→Local.TryBeginTransferRequest | 진행 중 요청/transition·신뢰 가능한 로드·현재 환경·Pending 0건 확인→checkpoint/CanUseOnlineData·Pending 재확인→잠금→callback, finally 잠금 해제 | OnlineAccountCoordinatorTests의 Pending>0 Start/Complete 인증·전송 0회와 새 checkpoint 실패 Test |
| OnlineAccountCoordinator.RequestAsync | RequestInProgress 거절, 인증/현재 binding 및 입력 검증 뒤 요청; Start/Complete는 위 gate를 통과, 응답은 generation/binding 대조 뒤 반영 | 기존 계정/Controller 중복·stale/timeout Test와 보강한 잠금 Test |
| FinishCompletionAsync→Local.TryBeginAccountTransition | 서버 응답/공개 번호·활성 현재 연결 대조, Pending 0건·환경·소유 transfer gate·진행 중 transition 확인 후 전체 snapshot 대기 | AccountTransferCompletionTests의 이전 성공/복구/Pending·동시 쓰기 보호 |
| Local.TryApplyAccountTransition | 기대 binding 동일성·잠금·Pending 0건 확인→설정/튜토리얼·비활성 환경과 함께 현재 계정/전체 최고를 한 번의 TrySave로 교체→성공 후 Memory 초기화/복원 | 기존 저장 실패/재시작 Test, 새 정상/실패별 잠금 해제 Test 및 전체 snapshot Test |

새 `EmptyPending_TransferCheckpointFailure_BlocksAuthenticationAndReleasesGate`는 Pending 0건이어도 저장이 실패하면 Start/Complete 둘 다 인증/전송을 시작하지 않고 원래 binding/파일을 보존하는지 검사하도록 준비했다. 실패 후 정상 checkpoint/gate callback·신규 Pending 저장이 다시 가능한지도 확인한다. 기존 Pending>0 계정/Controller Test에는 저장 시도·파일·소유/ID 불변 assertion을 보강했다.

활성 연결의 최종 권한은 서버 책임이다. Client의 현재 Player/binding·generation 검사와 로컬 SDK 대역의 서버 활성 권한 검증을 연결했으며, 가짜 응답 통과를 원격 활성 연결 전수 검증으로 기록하지 않는다. Stage 자료 보관 조건에 따른 gate 예외는 추가하지 않았다.

#### 5-2. 전체 snapshot 해석과 사용자 Stage 조회 분리 — 완료

`OnlineAccountCoordinator.TryReadSnapshot`은 Success/정확한 공개 번호·배열/최대 보드 수·행/점수·UUID v4 ID·중복 Board를 검증한 뒤 **계정 전체 최고**를 해석한다. Stage/Infinite 내부 Board 해석을 유지한다. Stage가 포함됐다는 이유로 전체 응답을 필터링/실패 처리하지 않으며, 사용자 Stage 조회의 false/out null과 구분한다. 파싱 결과는 로컬 소유 ID에 귀속된 순위 캐시이며 Run 시간/점수 구성 요소나 Pending을 새로 만들지 않는다.

`FinishCompletionAsync`는 snapshot 응답 이후에도 현재 generation/Player·동일 binding·서버 확인 공개 번호를 검사한 다음 적용한다. 불완전/잘못된 응답을 빈 계정의 성공으로 치환하지 않는다. 이미 같은 연결을 적용한 뒤 반복된 Completed 응답은 기존 오프라인 기록/Pending를 지우지 않는 기존 경로를 유지한다.

- `InvalidSnapshot_PreservesWholeLocalArea`에 null snapshot/행·잘못된 공개 번호/ID·v4 아닌 ID·Infinite int 상한 초과·최대 보드 수 초과 **7개 case**를 추가했다. 기존 실패/null 배열/중복/모르는 Board/음수/번호 불일치와 합쳐 13개 입력에서 파일/쓰기 시도·원래 최고/번호·Pending·비활성 환경 불변을 검사한다.
- 기존 snapshot timeout·binding 교체·Invalidate 이후 늦은 응답 거절 Test를 유지했다. snapshot 대기 중 중복 Refresh는 RequestInProgress, 추가 Complete는 CompletionHandoffRequired라는 실제 기존 거절을 대조해 assertion에 반영했다.
- 순수 내부 codec의 형식 보존 Test와 사용자 Local Stage 조회 거절은 계속 분리한다. Snapshot 서버 endpoint는 계정 입력/인증 환경에서 전체 결과를 반환하며 record 요청 필터의 대상이 아니다.

#### 5-3. 전체 교체·계정 역할·환경/재시작 보존 Test — 완료

`WholeSnapshot_KeepsInternalBoardsAndDeviceDataAcrossRestartAndScopeSwitch` 2개 case는 실제 account coordinator/Local repository/codec과 메모리 저장소를 사용한다. Infinite-only 또는 합성 Stage+Infinite snapshot을 받고, recipient의 기존 Infinite 최고 1000보다 낮은 연결 계정 최고 **500으로 전체 교체**함을 확인한다. 높은 값만 선택하거나 두 계정 기록을 합산하지 않는다.

Test는 로컬 소유 ID(owner), 인증 Player(B), 10자리 공개 번호를 서로 다른 역할로 유지하고 snapshot 캐시는 로컬 소유자에 귀속되는지 검사한다. snapshot에서 읽은 ranking 값은 Run 점수/시간으로 꾸며 제출하지 않으며 Pending과 submit-record/query-records 호출을 생성하지 않는다. Stage 항목이 있는 경우 사용자 Local 조회는 거절하되 내부 전체 저장·checkpoint·재시작·다른 환경 로드 후 복귀에서도 항목이 남는지 확인한다.

입력 binding override·Settings/튜토리얼과 비활성 환경의 계정/공개 번호·개인 최고/다른 소유자의 Pending ID를 보존하는 assertion을 추가했다. 비활성 환경은 이전 대상과 합치지 않는다. 이는 합성 자료를 통한 공유 형식/계정 보존 Test이며 Stage 보관/이관 기능이나 실제 Stage 자료 조사 요구의 추가가 아니다.

기존 빈 snapshot 성공 Test는 재시작 후 현재 최고/Pending가 빈 상태이고 새 공개 번호 및 비활성 환경이 남는지 보강했다. transition 잠금 Test는 정상/저장 실패 2개 case로 확장해 진행 중 신규 최고/Pending/폐기·중복 요청은 거절하고 finally 이후에는 잠금이 해제되는지 확인한다. 저장 실패 시 원래 계정/최고가 남는 기존 Test도 유지한다.

이번 추가는 **EditMode 12개 case**다. AccountTransferCompletionTests에 정상 snapshot 2개·잘못된 응답 7개·잠금 실패 경계 1개(기존 1개→2개로 확장)로 10개, OnlineAccountCoordinatorTests에 저장 실패 gate 2개를 추가했다. AccountTransferControllerTests는 기존 assertion만 보강했다. 기존 OnlineLocalSaveScopeTests의 환경 왕복/격리와 LocalSaveJsonCodecTests의 형식/재시작 Test도 대조했으며 무관한 새 구조/중복 fixture를 추가하지 않았다. 실제 Runner 실행 수가 아니다.

#### 5-4. 정적/로컬 검증과 후속 인계 — 완료

이번 Step의 C# 수정은 위 Test 3파일뿐이다. 작업 전후 `Assets/Scripts`와 `Assets/Editor`의 C# 전체 경로/내용 SHA-256 집계가 같음을 확인해 생산 코드 보존을 확인했다. AccountTransferPendingGate·LocalRecordRepository·OnlineAccountCoordinator·codec/서버 계약은 그대로다. 실제 필드/메서드·Core/Feature using·async 반환형·변수 선언/인자·asmdef/meta/GUID와 기존 거절 사유의 우선순위를 정적으로 대조했다.

`client-transfer-completion-contracts.test.cjs`에 Pending/checkpoint/잠금 순서, Stage/Infinite 전체 parser·로컬 소유 귀속, snapshot 이후 stale/번호 검사와 원자 적용 순서·필터 없는 내부 복원을 검사하는 소스 계약을 추가했다. preflight의 선택 suite 수는 216/24→228/24로 갱신했다. Node 스크립트는 소스 읽기/assert/읽기 전용 git이며, 아래 서버 Test는 기존 VM loader/가짜 SDK/메모리 저장·가짜 Secret을 사용하는 경로임을 확인한 뒤 실행했다. 실제 계정 이전/자격 증명 발급·기기 저장·원격 SDK 요청은 수행하지 않았다.

실행 명령은 `node UGS/Tests/<파일>`이고 **12스크립트 모두 통과**했다. 이번 실행 검사 실패는 없다.

| 파일 | 이번 결과/범위 |
| --- | --- |
| client-unity-preflight.cjs | PASS / 누적 변경 C# 27파일 소스/중복 선언/meta/GUID·asmdef/격리; 선택 Edit 228/Play 24 PREPARED는 실행 수가 아님 |
| static-contracts.cjs | PASS / 기존 서버/Client/Module 소스 계약 |
| client-local-save-contracts.test.cjs | PASS / 저장/v7/복원·신규 쓰기/재시도 거절 순서 소스 계약 |
| client-account-contracts.test.cjs | PASS / 계정/귀속/일반 입력·stale 계약; Edit 32 source cases prepared |
| client-transfer-completion-contracts.test.cjs | PASS / 기존 전체 교체/내구 저장·SDK API 및 Step 5 gate/parser/응답 fence/복원 소스 계약; Edit 40 prepared |
| client-verification-tools-contracts.test.cjs | PASS / 기존 권한/격리·로그·record/raw/재시도 소스 계약 |
| account-personal-bests.test.cjs | PASS 18/18 / Infinite-only·계정 전체/빈 snapshot·잘못된 응답·이전 후 recipient 미병합 로컬 SDK 대역 |
| record-account-integration.test.cjs | PASS 46/46 / 실제 서비스 함수/endpoint의 계정/기록 통합 SDK 대역 |
| transfer-completion.test.cjs | PASS 22/22 / 계정 이전 서비스·복구/영수증 로컬 SDK 대역 |
| transfer-races.test.cjs | PASS 22/22 / 이전 경합/늦은 응답 로컬 SDK 대역 |
| json-snapshot-consistency.test.cjs | PASS / JSON roundtrip·중단 쓰기 경계 30개·score 쓰기 없음. 30은 Test 개수가 아님 |
| cloud-code.test.cjs | PASS 28/28 / Infinite 점수/ID·조회와 기존 서버 계약 로컬 대역 |

이 결과는 C# 이름 해석/Compile/Unit Test 실행·원격 서비스 성공을 증명하지 않는다. Unity Compile/Runner/빌드/Player는 실행하지 않았다. Scene/Prefab·프로필·생산 저장 형식/Board/서버/Module은 변경하지 않았다.

**완료 판정:** 기존 계정 계약 대조, 생산 코드 보존과 Infinite 이전/snapshot/저장·내부 읽기/환경 보존 Test 준비 및 정적/로컬 검증을 마쳤으므로 Step 5를 완료 처리한다. 다음은 **Step 6 Editor Stage 도구와 과거 probe의 인증 전 진입 차단**이다. Step 6~11·Phase 3 전체는 미완료다.

**사용자 수동 작업: 없음. 추가 Scene 작업도 없음.** 실제 두 기기 이전/복구 코드 발급·기기 저장 삭제·원격 Stage 보관을 요구하지 않는다. 최신 Compile/필요한 Scene 적용은 Step 9, EditMode/PlayMode 실행은 Step 10에 유지한다.

## Step 6. Editor Stage 도구와 과거 probe 번호를 차단한다

### AI 작업

6-1. OnlineRecordVerificationWindow의 Stage 조회/제출/검증 버튼·Stage 선택 toggle·legacy 검증·direct-write/account mismatch 경로를 모두 조사한다. 사용자 조작은 숨기거나 비활성화하고 지원 종료 이유/Infinite 대체 경로를 표시한다.

6-2. 버튼 숨김과 별도로 기존 메서드/숫자 probe 직접 호출을 인증·기준선 조회·격리 저장 생성·SDK/HTTP 요청 전에 거절한다. 특히 probe 2/5/6과 SelectedBoard(조사 전 명칭 GetSelectedBoard) 등 과거 Stage 선택 진입을 표에 명시하고 다른 번호도 전수 대조한다.

6-3. Stage 전용 코드는 삭제 대신 적용 가능한 Obsolete/Deprecated 표기와 guard/이유를 보존한다. Infinite 검증/안전 로그/권한·timeout·Late 응답 처리와 자동 실행 금지·PlayMode 격리의 기존 보호는 약화하지 않는다.

### 정적 검증·Unit Test

- `VerificationSessionTests`, `CloudCodeModuleRoutingTests`, PlayMode `VerificationToolIsolationTests`에서 기존 Stage 메서드/과거 probe 직접 진입도 상태 불변·인증/전송 0회인지 검사한다. 실제 검증창 버튼이나 외부 서비스를 실행하지 않는다.
- `UGS/Tests/client-verification-tools-contracts.test.cjs`를 새 정책에 맞게 보완한다. Stage 버튼 존재를 요구하는 옛 소스 assertion은 새 거절 계약으로 대체하되 격리/민감 로그/권한 검사는 유지한다.
- CloudCodeService/HTTP/direct-write 호출 검색과 각 guard의 선후 관계를 정적으로 대조한다. 문자열이 남았다는 이유만으로 미완료 판정하거나, Obsolete만 붙였다고 접근 차단 완료 판정하지 않는다.

### 사용자 수동 작업

없음. Stage probe/direct-write 거절을 실제 서버에 요청해 확인하지 않는다.

### 완료 조건

- [x] Stage 도구 UI와 직접/과거 번호 진입이 차단되고 Infinite 도구·자동 격리/권한 보호 Test를 준비했다.

### Step 6 수행 결과 — 2026-11-17

#### 6-1. Stage 도구 UI와 지원 종료 이유

`Assets/Editor/OnlineRecordVerificationWindow.cs`의 legacy Stage 조회·서버 중복/변조/버전 검증·Player direct-write·계정 불일치 버튼과 Phase 2 Stage 선택 toggle·probe 2/3/5/6 버튼을 숨겼다. 두 화면에 `STAGE_RECORDS_UNSUPPORTED`와 Infinite 대체 경로를 표시한다. 계정 복구 제한 안내는 유지하며 이전 검증은 격리 계정 패널을 안내한다.

기본 선택은 Infinite이며 Board는 `ScoringVersion.Current`의 `fs-infinite-v2`다. 과거 `_infinite=false` 상태를 요청 중 자동 변환하지 않는다. 필요하면 사용자가 `Use Infinite board`를 명시적으로 눌러 지원 Board를 선택한다. 기존 Infinite 조회·Pending 재시도·계정 패널과 공개 번호/점수/수락 시각 비교는 보존했다.

#### 6-2. 직접 메서드와 모든 probe 번호 대조

| 진입 | 현재 판정과 부작용 경계 |
| --- | --- |
| `Query(game, false)` | `CanRunLegacy`·busy·계정/조회 전에 반환한다. true인 Infinite 경로는 유지한다. |
| `VerifyServerValidation`, `VerifyDirectWriteDenied`, `VerifyAccountMismatch` | 영구 지원 종료 guard로 권한 검사·busy·인증/프로필 전환·token/전송 전에 반환한다. |
| `SelectedBoard()` | 실제 메서드명이다. 과거 Stage 선택은 `NotSupportedException(STAGE_RECORDS_UNSUPPORTED)`이며 선택/상태를 바꾸지 않는다. Infinite는 현재 Scoring Version으로 생성한다. |
| probe 0 | Infinite 내 최고/공개 번호 기준 캡처 유지. Stage 선택은 진입 첫 줄에서 거절한다. |
| probe 1 | Infinite 기준 JSON 비교 유지. Stage 선택은 인증·기준선 해석 전에 거절한다. |
| probe 2 | 과거 Stage 제출. 선택값과 관계없이 영구 거절한다. |
| probe 3 / `ProbePhase2DirectWrites()` | 과거 Player direct-write. 번호 진입과 helper 직접 진입 모두 token 접근 전에 영구 거절한다. |
| probe 4 | 원래 A 계정 Inactive 상태 확인 유지. 기록 Board 선택을 사용하지 않는 계정 endpoint다. |
| probe 5 | 과거 신규 Stage 제출/서버 행 확인. 선택값과 관계없이 영구 거절한다. |
| probe 6 | 과거 Stage 제출 전 조회. 선택값과 관계없이 영구 거절한다. |
| probe 7 | 원래 A 기록 조회 거절 확인 유지. Stage 선택은 원래 A 인증 전에 거절한다. |
| probe 8 | 기존 Infinite Pending 단일 재전송 유지. 저장 snapshot의 Stage는 지원 종료 사유, 그 외 미지원 Mode/Version/StageId는 `UNSUPPORTED_PENDING_RECORD`로 deadline·checkpoint·인증 전에 거절한다. 원본을 삭제/변환하지 않는다. |
| 범위 밖 번호 | `INVALID_PROBE`로 첫 진입에서 거절하며 다른 작업으로 해석하지 않는다. |
| `CallSubmitAsync` / `PostJsonAsync` | raw 제출 helper는 record payload 검증 후 자동 실행/SDK 경계로 진행한다. 기존 Stage URL helper 거절은 HTTP 생성 이전에 유지한다. |

`GetProbeBlockReason`은 번호/선택/메모리 Pending snapshot만 읽는다. `ProbePhase2`는 이 판정을 `CanRunPhase2`, busy 변경, deadline 생성, 계정 인증·기준선 조회·격리 저장/SDK/HTTP보다 먼저 수행한다. 거절 시 summary·입력/내보내기·세션 세대·선택·저장 내용은 바꾸지 않는다. 별도 세션 준비는 기존 명시적 로컬 준비 작업이며 Stage probe가 이를 호출하지 않는다.

#### 6-3. Deprecated 소스와 Infinite/자동 격리 보호 Test

Stage 전용 메서드/DTO·기존 probe 본문은 Deprecated 이유와 함께 보존했다. `IsStageRecordToolRetired()`는 항상 true인 비설정식 경계다. 메서드 guard는 보존된 async 본문에 상수 조건의 도달 불가 경고를 추가하지 않는다. 순수 내부 Stage 비교/직렬화 계약과 서버/계정 전체 snapshot은 변경하지 않았다.

| Test 소스 | 추가 case와 준비한 판정 |
| --- | --- |
| EditMode `VerificationSessionTests` | 24개: 모든 번호/선택의 순수 분류 20개, `SelectedBoard` 2개, 합성 Stage/구 Infinite Pending의 deadline·쓰기·인증 전 거절/원본 보존 2개. 현재 suite 97개. |
| EditMode `CloudCodeModuleRoutingTests` | 2개: 정상/잘못된 Stage Version payload를 raw helper에 직접 주입하고 자동 실행 guard·null 전송 이전의 지원 종료 예외 확인. 현재 suite 33개. |
| PlayMode `VerificationToolIsolationTests` | 14개: legacy/숫자/범위 밖/direct-write helper 직접 진입의 상태 불변·가짜 인증/전송/시계/메모리 쓰기 0회. 현재 suite 29개. |

총 추가 Test 소스는 **EditMode 26개/PlayMode 14개**다. fake/메모리 저장을 사용하고 window는 해제 후 기존 SessionState 격리 값을 복구한다. 실제 UGS·검증창 버튼·영구 저장은 실행하지 않는다. 자동 Test의 `CanRunPhase2`를 우회하지 않으며, 순수 분류 Test·직접 진입 Test·정적 guard 순서 검사를 함께 사용한다. 기존 원격 동의/프로필/PlayMode 격리·민감 로그 제한·5초 timeout/지연 응답 폐기·자동 Test/Batch 거절 검사는 유지했다. **C# Test는 준비만 했으며 Unity에서 실행하지 않았다.**

#### 6-4. 정적·로컬 검증과 완료 판정

기존 Stage 버튼/성공 경로를 요구하던 Node 소스 assertion을 UI 부재·전수 번호 분류·부작용 전 guard·Infinite/계정 probe 보존 계약으로 대체했다. 자동 격리/권한·안전 로그/timeout assertion은 유지했다. 실제 SDK 대신 로컬 대역을 사용하는 기존 스크립트를 확인한 뒤 아래 **10개를 모두 통과**했다.

| 실행한 `UGS/Tests/` 스크립트 | 결과 |
| --- | --- |
| `client-unity-preflight.cjs` | 누적 변경 C# 27개·GUID/asmdef·소스 구분자·격리 setup 확인. 선택 suite Edit 254/Play 38 소스 case 준비. Compile/Runner 결과 아님. |
| `static-contracts.cjs` | 기존 서버/계정 계약 소스 검사 통과. 배포/원격 실행 없음. |
| `client-local-save-contracts.test.cjs` | v7/내부 복원/신규 쓰기·재시도 guard 소스 계약 통과. |
| `client-account-contracts.test.cjs` | 계정 권한·인자·scope/stale 계약 소스 검사 통과. |
| `client-transfer-completion-contracts.test.cjs` | 전체 snapshot 교체/이전·내부 보존 계약 소스 검사 통과. |
| `client-verification-tools-contracts.test.cjs` | 신규 Step 6 UI·직접 진입/번호/pending/raw guard 순서 및 기존 격리 보호 계약 통과. |
| `record-account-integration.test.cjs` | 로컬 SDK 대역 46/46 통과. |
| `account-personal-bests.test.cjs` | 로컬 SDK 대역 18/18 통과. |
| `cloud-code.test.cjs` | 로컬 SDK 대역 28/28 통과. |
| `leaderboard-query-policy.test.cjs` | 기존 조회/순위 정책 로컬 대역 통과. |

이번 Step 변경은 Editor 생산 C# 1개·Test C# 3개·Node 2개와 관련 문서다. 누적 C# 27개·Node 4개·문서 7개이며 Scene/Prefab·Build Profile·서버/Module·실제 사용자 저장은 변경하지 않았다. 누적 추가 Test 소스 case는 Edit 107/Play 17이다. 완료 체크/문서 상대 링크·진행 상태·변경 범위·`git diff --check`도 정적으로 확인한다.

**사용자 수동 작업: 없음.** 실제 Stage 요청으로 거절을 확인할 필요가 없다. Unity Compile/필요한 Scene 적용은 Step 9, Test Runner는 Step 10이며 AI가 실행하지 않았다. Phase 2의 919/288 통과나 Candidate02를 이번 변경의 실행 근거로 승계하지 않는다.

**완료 판정:** Stage 도구 UI·직접 메서드/과거 probe의 부작용 전 차단 코드와 Infinite/격리 보호 Test 준비·정적/로컬 검증을 마쳤으므로 Step 6을 완료 처리한다. 다음은 **Step 7 기존 UI·Result·호출자와 Test fixture 통합**이다. Step 7~11과 Phase 3 전체는 미완료다.

## Step 7. 기존 UI·Result·호출자와 Test fixture를 통합한다

### AI 작업

7-1. Phase 2에서 완료한 Leaderboard Stage 비활성·Infinite 기본값/탐색·Top/Around·Result Me·Back/Cancel 출처 보존을 새 Repository 경계와 대조한다. 이미 일치하면 변경하지 않는다.

7-2. Result의 개인 최고/제출·조회 Retry·Pending 상태와 오래된 Stage/미정의 선택의 직접 진입을 점검한다. 기존 Infinite 정상 표시/계정 공개 번호/독립적인 Top·Around Loading·Empty·Offline·Error/late 응답 무효화를 유지한다. 미지원 사유나 오래된 값이 Stage 기록을 다시 표시하지 않게 한다.

7-3. 기존 `OnlineRecordRepositoryTests`, `OnlineLocalSaveScopeTests`, `VerificationSessionTests` 등 Stage 성공 factory helper를 전수 전환한다. Infinite 테스트에 합성 점수 제한/ID/귀속이 유효해야 하고 Stage 거절 Test는 거절 지점까지 실제로 도달해야 한다. 공용 helper에서 Assert를 제거하거나 guard를 우회해 통과시키지 않는다.

### 정적 검증·Unit Test

- EditMode `LeaderboardViewStateTests`, `GameNavigationStateTests`, `ResultTextFormatterTests`와 PlayMode `OnlineRecoveryNoticeFlowTests`, `ResultMenuIntegrationTests`, `ModeResultDisplayIntegrationTests`, `ModeUISceneConfigurationTests`, `AccountTransferUIIsolationTests`, `PlayModeRecordIsolationTests`를 대조/보완한다.
- 생산 SampleScene/실제 직렬화 참조를 사용한다. UI 생성 fixture·누락 런타임 보충은 추가하지 않는다. 정책과 무관한 Stage 순수 모델/자료형 Test를 무조건 삭제하지 않는다.
- 같은/인접 프레임·중복 클릭/Submit·창 취소·비동기 응답 후 화면 이동은 자동 Test로 확인한다. 사람에게 빠른 입력 재현을 요구하지 않는다.

### 사용자 수동 작업

없음. 이 시점 Scene 변경은 적용표만 준비하고 Step 9에서 전달한다.

### 완료 조건

- [x] 새 기록 경계가 기존 Infinite 화면/복귀/상태 계약과 연결되고 Stage 성공 fixture의 용도별 전환·회귀 Test 준비를 마쳤다.

### Step 7 수행 결과 — 2026-11-18

#### 7-1. 기존 Infinite 화면/탐색과 생산 Scene 연결 대조

Phase 2의 `LeaderboardViewState`/`GameNavigationState`는 Infinite 기본값·Stage/미정의 선택 거절·Main Menu/Result 진입·출처별 Back/Cancel을 이미 구현했다. `GameSystem.QueryLeaderboardAsync`의 현재 Infinite Board·Top 10/Around 7과 Repository 거절 경계가 일치하므로 해당 정책을 재구현하지 않았다. `UIManagementSystem.SetLeaderboardViewState`에는 과거/미정의 Mode의 상태가 직접 전달돼도 표시하지 않는 진입 검사를 보강했다.

`SampleScene.unity`의 실제 직렬화 자료를 읽어 Result 상태 Text/제출 Retry/Leaderboard 버튼, Top/Around/공개 번호 Text, Infinite/Back/Pending Retry의 **9개 UI 참조**가 존재하는 로컬 컴포넌트를 가리키는지 확인했다. Result Leaderboard·공통 조회 Retry·Pending Retry·현재 Result 재전송·Main Menu·동의 Continue/Cancel의 **7개 콜백**이 실제 GameSystem을 대상으로 하는지도 확인했다. Top/Around는 독립 요청 API이며 Scene의 조회 Retry는 공통 버튼이다. 개별 Top/Around onClick이 있어야 한다는 최초 검사 가정은 실제 연결에 맞춰 수정했다. Scene 누락으로 판정하거나 편집하지 않았다.

**Scene 적용표: 변경 대상 없음.** 기존 생산 UI의 참조·Navigation/초기 비활성과 격리 설정을 유지한다. Step 9에서 이 변경에 대해 새 GameObject/컴포넌트·Inspector 연결을 추가할 필요가 없다.

#### 7-2. Result 표시/재시도와 비동기 응답 경계

| 경계 | 반영/보존 결과 |
| --- | --- |
| `UIManagementSystem.SetResultRecordPresentation` | 현재 Mode가 Infinite인지 먼저 확인한다. 후보가 있으면 현재 Infinite Mode/Version·빈 StageId인지 검사한 뒤 Text/Retry UI를 갱신한다. Stage/구 Version/미정의 후보는 기존 표시를 바꾸지 않는다. |
| `GameSystem.RetryCurrentResultSubmission` | 같은 후보 지원 검사를 busy·coordinator 재시도 전에 수행한다. 기존 중복 실행 방지와 후보 ID별 재시도는 유지한다. |
| `RefreshResultRecordPresentation`, `ApplyResultRecordPresentation` | 미지원 후보는 Pending/terminal 조회·표시/Navigation Retry 가용성 변경 전에 반환한다. 내부 snapshot/저장 자료를 삭제하거나 후보를 Infinite로 바꾸지 않는다. |
| `RefreshResultOnlineBestAsync` | 오래된 expectedCandidate는 개인 최고 요청 세대 변경 전에 거절한다. 현재 후보의 Mode/Version/StageId와 결과 Mode를 검사하며 Stage/미정의 선택을 조회하지 않는다. Board는 `ScoringVersion.Current`를 사용한다. |
| 개인 최고 응답 적용 | await 이후 현재 runtime·화면·후보 identity·결과 Mode가 요청 시작과 유효하게 연결되는지 확인한다. 다른 Result/화면의 응답은 적용하지 않는다. 기존 개인 최고 요청 세대 검사도 유지한다. |
| Top/Around 응답 적용 | 각각 await 이후 runtime·화면이 요청 시작과 같은지 확인하고, 기존 독립 요청 세대로 최신 응답만 적용한다. 한쪽 실패가 다른 쪽 성공/개인 최고를 덮어쓰지 않는다. |
| Infinite 표시/상태 | Local best, Pending/Submitted/Rejected, 제출 성공과 별도 Online best, 공개 번호, Loading/Empty/Offline/Error 및 출처별 복귀를 보존한다. |

새 계정/온라인 서비스·공통 정책 클래스나 런타임 UI 보충을 추가하지 않았다. 미지원 사유나 과거 자료를 Stage 행으로 다시 표시하지 않으며 보존된 순수 Stage formatter는 변경하지 않았다.

#### 7-3. 기존 fixture 감사와 회귀 Test 준비

Step 1의 Stage factory 사용 10파일/16호출 목록을 다시 대조했다. 현재 `TryCreateStageCandidate` 호출은 **`RecordSubmissionPolicyTests`의 거절 검사 6곳뿐**이다. `OnlineRecordRepositoryTests`, `OnlineLocalSaveScopeTests`, `OnlineAccountCoordinatorTests`, 이전/검증/PlayMode 계정 fixture의 공통 성공 후보는 앞선 Step에서 유효 Infinite 점수·UUID v4·귀속으로 전환됐다. 공용 helper의 Assert는 유지하며 합성 Stage 입력은 실제 거절 경계, 내부 codec/Memory/전체 snapshot·순수 정렬/formatter 검증으로 분리했다. 서버 로컬 대역의 기존 Stage 계약은 유지했다.

| Suite | 이번 변경과 기존 보존 범위 |
| --- | --- |
| EditMode `LeaderboardViewStateTests` | **5개 추가**, 현재 12개. Around의 Success/Empty/Offline/Timeout/StageModeUnsupported 상태가 Top 성공·개인 최고와 독립적인지 검사한다. 기존 오래된 요청/Invalidate/Stage·미정의 선택 검사 유지. |
| EditMode `GameNavigationStateTests`, `ResultTextFormatterTests` | 각각 60/43개 소스 case 대조. 출처별 복귀·Retry·같은/인접 입력·무효 선택과 현재 Infinite/내부 Stage 순수 formatter 검사 유지. |
| PlayMode `ResultMenuIntegrationTests` | **11개 추가**, 현재 20개. 과거 Stage/구 Infinite/미정의 후보의 표시·재시도·조회 거절 3개, 정상 Pending/Submitted와 독립 Online best 1개, Stage/미정의 현재 선택 거절 2개, 오래된 expectedCandidate 거절 1개, Result 변경/화면 이동 뒤 늦은 개인 최고 2개, Top/Around 화면 이동 뒤 늦은 응답 2개. |
| PlayMode `OnlineRecoveryNoticeFlowTests` | 7개 유지. 동의 Continue 중복/인증 대기·로컬 저장 실패·Stage Result 조회 거절과 Empty 구분. |
| PlayMode `ModeResultDisplayIntegrationTests`, `ModeUISceneConfigurationTests` | 8/5개 유지. Stage/오래된 UI 표시 거절과 생산 Scene 직렬화/콜백·포커스·Infinite Result 표시. |
| PlayMode `AccountTransferUIIsolationTests`, `PlayModeRecordIsolationTests` | 9/3개 유지. 생산 Scene 계정 UI 격리·복사/취소·실제 저장/UGS 차단. |

추가 소스 case는 **Edit 5/Play 11**이며 누적 추가는 **Edit 112/Play 28**이다. 새 통합 Test는 기존 SampleScene setup·실제 직렬화 UI·메모리/fake repository와 지연 Task를 사용하고 GameObject/UI를 생성하지 않는다. 정상 Infinite 후보는 실제 factory 성공을 Assert한다. 합성 과거 자료만 내부 생성자/반사로 구성한다. 네트워크가 NotReachable이면 지연 Test는 SDK 대신 Offline 결과/가짜 요청 0회를 검사하고 종료한다. 연결 가능한 환경에서도 실제 요청은 fake로만 처리한다.

기존 같은/인접 프레임·중복 click/Submit의 단일 Retry 실행, 동의 반복/취소와 계정 창 보호 Test는 유지했다. 빠른 사람 입력으로 재현하도록 요청하지 않는다. **모든 C# Test는 준비 상태이며 Unity에서 실행하지 않았다.**

#### 7-4. 정적·로컬 검증과 완료 판정

`client-unity-preflight.cjs`에 위 9개 Scene 참조/7개 콜백의 읽기 전용 검사를 추가하고 관련 integration suite 소스 집계를 보완했다. 기존 선택 suite Edit 254/Play 38과 별도로 integration suite Edit 115/Play 43개를 표시하며 전체 Unity 실행 건수로 합산/주장하지 않는다. `client-verification-tools-contracts.test.cjs`에 Result 표시/재시도/개인 최고 지원 guard와 화면/runtime/후보 응답 적용 순서·생산 Scene fixture 계약을 보강했다.

관련 **12스크립트 모두 통과**: `client-unity-preflight.cjs`, `static-contracts.cjs`, `client-local-save-contracts.test.cjs`, `client-account-contracts.test.cjs`, `client-transfer-completion-contracts.test.cjs`, `client-verification-tools-contracts.test.cjs`, `client-account-transfer-ui-contracts.test.cjs`, `phase1-policy-contracts.test.cjs`, `record-account-integration.test.cjs`(46/46), `account-personal-bests.test.cjs`(18/18), `cloud-code.test.cjs`(28/28), `leaderboard-query-policy.test.cjs`. 모두 `UGS/Tests/` 아래의 읽기 전용 소스 검사 또는 기존 로컬 SDK 대역이다. 원격·Unity 실행 결과가 아니다.

이번 변경은 생산 C# 2개(GameSystem/UIManagementSystem)·Test C# 2개·Node 2개와 관련 문서다. 누적 변경은 C# 29개·Node 4개·문서 7개다. Scene/Prefab·프로필·서버/Module·실제 저장 변경은 없다. 반사 대상 메서드/필드와 인자를 소스에 대조했으며 문서 링크/완료 체크 7개·미완료 4개/변경 범위 및 `git diff --check`를 정적으로 확인한다.

**사용자 수동 작업: 없음.** Scene 적용은 필요하지 않다. 최신 Compile은 Step 9, EditMode/PlayMode 전체 실행은 Step 10에 모으며 빌드·Runner를 AI가 수행하지 않았다.

**완료 판정:** 기존 Infinite 화면/복귀 계약과 새 경계를 대조하고 Result 직접 진입/지연 응답 보강·fixture 감사·회귀 Test 준비 및 정적/로컬 검증을 마쳤으므로 Step 7을 완료 처리한다. 다음은 **Step 8 최종 경로/정적 검사·로컬 대역 Test·사용자 실행 인계**다. Step 8~11과 Phase 3 전체는 미완료다.

## Step 8. 최종 정적 검사·로컬 대역 Test와 실행 인계를 완료한다

### AI 작업

8-1. Step 1 경로 표의 모든 행을 최종 코드/guard/거절 사유/Test에 연결한다. 남은 Stage 참조는 내부 자료·Deprecated 보존·합성 Test·차단된 경로로 분류하고 지원되는 미차단 진입이 없는지 확인한다.

8-2. 변경 C#의 선언/지역 변수 최초 사용 순서·using·반사 메서드/필드 이름/인자·async 반환형·asmdef·meta/GUID·enum 숫자/직렬화 참조를 검토한다. 과거 CS0103/CS0841 재발을 별도 점검한다. preflight는 C# 이름 해석/컴파일 검사라는 주장을 하지 않는다.

8-3. `node UGS/Tests/client-unity-preflight.cjs`, 관련 소스 계약과 아래 로컬 SDK 대역 Test를 감사 후 실행한다. 실행 파일/결과/건수/한계를 기록하고 실패 시 원인을 수정한 뒤 영향 검사만 재실행한다.

| 종류 | 기존 실행 파일 예시 | 판정 범위 |
| --- | --- | --- |
| C# 소스 계약 | `UGS/Tests/static-contracts.cjs`, `client-local-save-contracts.test.cjs`, `client-account-contracts.test.cjs`, `client-transfer-completion-contracts.test.cjs`, `client-verification-tools-contracts.test.cjs` | 참조/분기·기존 저장/이전·검증 격리 계약. C# 실행 성공은 아님 |
| 실제 서비스의 로컬 SDK 대역 | `UGS/Tests/record-account-integration.test.cjs`, `account-personal-bests.test.cjs`, `cloud-code.test.cjs`, `leaderboard-query-policy.test.cjs`, `submission-retention-policy.test.cjs`, `transfer-completion.test.cjs`, `transfer-races.test.cjs`, `json-snapshot-consistency.test.cjs` | Infinite 제출/조회/동일 ID·snapshot/이전/수명 회귀. 원격 서비스 성공은 아님 |

8-4. `git diff --check`는 변경 코드/문서 범위로 수행하고 Unity 저장 YAML의 관례적인 공백을 이유로 Scene을 AI가 편집하지 않는다. 새 Task/링크·누락된 참조·소스 파일 변경 범위도 확인한다.

8-5. Feature/System은 실제 반영한 현행 계약만 갱신한다. Step 9의 Scene 변경 유무·정확한 대상/필드/최종값·Compile 요청, Step 10의 실행 assembly/suite·예상 로그를 확정해 한 곳에 보관한다.

### 정적 검증·Unit Test

- fake 인증/전송/저장 카운터·ID/요청/결과·상태 snapshot을 assertion으로 판정하는지 검토한다. 소스 문자열 확인만으로 동작 Test를 대신하지 않는다.
- 기존 PlayMode 격리 setup이 실제 저장/UGS를 차단하는지 확인한다. 생산 guard를 Test 전용 bypass로 무효화하지 않는다.
- 예상 Warning/Error는 거절 Test별로 정확한 LogAssert.Expect를 사용한다. 광범위한 로그 무시/실패 Test 비활성화/전체 stage 검색 결과 삭제는 금지한다.

### 사용자 수동 작업

없음. AI의 준비가 끝난 뒤 Step 9를 안내한다.

### 완료 조건

- [x] 경로 폐쇄 표·소스/저장/계정/도구 검사와 오프라인 대역 Test 결과가 연결됐고 사용자 Compile/Scene/Runner 인계를 준비했다.

### Step 8 수행 결과 — 2026-11-18

#### 8-1. 최종 경로 폐쇄와 남은 Stage 참조 분류

Step 1의 생산/직접/내부 자료 경로와 Editor 표의 모든 번호를 아래 최종 경계에 연결했다. 메서드명은 현재 소스를 기준으로 하며 Step 1의 과거 줄 번호를 최신 위치로 해석하지 않는다. 아래의 Test는 준비한 C# 소스이며 실행 통과 근거가 아니다.

| Step 1 진입·호출망 | 최종 차단/보존 경계·사유 | 연결한 Test/정적 근거 |
| --- | --- | --- |
| GameSystem Result 확정/갱신 → TryStoreCurrentResultCandidate | Infinite 이외 Result는 ID 생성·최고 조회/저장 전에 반환 | ResultMenuIntegrationTests, RecordSubmissionPolicyTests |
| TryCreateStageCandidate | 모든 입력 false/out null. 선언/Deprecated 사유와 내부 변환 소스 유지 | RecordSubmissionPolicyTests |
| RecordSubmissionService → Local enqueue/최고 갱신 | 현재 Infinite Mode/Version·빈 StageId만 신규 저장. 미지원은 repository/clock/Memory 변경 전 false | RecordSubmissionPolicyTests, OnlineLocalSaveScopeTests |
| Local TryGetPersonalBest | Stage/무효 Board false/out null. 전체 내부 snapshot과 구별 | OnlineRecordRepositoryTests, LocalSaveJsonCodecTests |
| CloudCodeRecordRepository Submit | Stage는 인증 전 TransientFailure/StageModeUnsupported. 서버 terminal receipt로 취급하지 않음 | OnlineRecordRepositoryTests |
| CloudCodeRecordRepository Top/Around/Me → QueryAsync | Stage는 인증/전송 전 같은 로컬 사유. 잘못된 Board/limit은 InvalidQuery | OnlineRecordRepositoryTests, OnlineRecordConfigurationTests |
| UgsOnlineRecordTransport의 두 overload → CallEndpointAsync | record JSON 검증이 Module/진단/SDK 호출보다 먼저 실행. Stage prefix는 NotSupportedException | CloudCodeModuleRoutingTests, client-verification-tools-contracts |
| VerificationRemoteGuard의 두 overload | 기존 권한 거절 유지. 허용된 fake 호출도 record 검증 후 deadline/전송 진행. 계정 endpoint는 필터 대상 아님 | VerificationSessionTests |
| OnlineRecordCoordinator background/single/all Retry | 전체 큐 지원/귀속/환경을 clock·만료/checkpoint·Bind 전에 검사. 미지원 자료/terminal 집계 불변 | OnlineRecordRepositoryTests, OnlineLocalSaveScopeTests |
| VerificationPendingRetry | scope/동의·단일 후보/소유·지원 Board를 checkpoint/전송 전에 검사. Stage는 PENDING_STAGE_UNSUPPORTED | VerificationSessionTests |
| 계정 Pending gate/이전 완료 → 전체 최고 snapshot | 기존 인증 전 gate·원자적 전체 교체/저장·재시작·환경 격리 유지. record 필터로 내부 Stage 자료를 누락시키지 않음 | OnlineAccountCoordinatorTests, AccountTransferCompletionTests, AccountTransferControllerTests |
| Memory repository/queue·codec·Local RestoreMemoryRecords | 내부 복원은 직접 Memory 갱신으로 보존. 신규 쓰기 guard로 기존 자료를 필터링/자동 삭제하지 않음 | LocalSaveJsonCodecTests, AccountTransferCompletionTests, client-local-save-contracts |
| RecordBoardKey/Configuration 매핑·enum·후보/entry/formatter/비교 | Stage 식별/직렬화·순수 합성 모델 보존. 매핑 성공을 실행 허용으로 사용하지 않음 | OnlineRecordConfigurationTests, RecordLeaderboardPolicyTests, ResultTextFormatterTests |
| 사용자 Leaderboard 선택/직접 표시 | Infinite 기본/Top 10·Around 7·공개 번호/출처별 복귀 유지. Stage/미정의 선택과 과거 Mode UI는 거절 | LeaderboardViewStateTests, GameNavigationStateTests, ModeUISceneConfigurationTests |
| Result 표시/Retry/Me·응답 적용 | 미지원 후보/선택은 UI/Navigation·재시도·조회 이전 거절. runtime/화면/후보/요청 세대로 오래된 응답 적용 방지 | ResultMenuIntegrationTests, OnlineRecoveryNoticeFlowTests, ModeResultDisplayIntegrationTests |
| Editor Query(false)·legacy 검증 3종 | UI 숨김과 별도로 busy·프로필/인증/token/전송 전 영구 거절. Query(true) 유지 | VerificationToolIsolationTests |
| Editor SelectedBoard·probe 전 번호 | Stage 선택 0/1/7, 과거 2/3/5/6, 미지원 Pending 8, 범위 밖 번호는 첫 진입 거절. 0/1/4/7/8의 지원 경로 유지 | VerificationSessionTests, VerificationToolIsolationTests |
| Editor direct-write/raw helper·Prepare/Execute/Authenticate/Retry | direct-write helper 영구 거절·Stage 제출/URL 검증이 SDK/HTTP 이전. 별도 Infinite/계정 준비·권한·자동 실행 금지/격리·timeout/late 보호 유지 | CloudCodeModuleRoutingTests, VerificationSessionTests, VerificationToolIsolationTests |

남은 Stage 문자열은 **지원 종료 guard/숨긴 실행 본문**, **내부 저장·직렬화/전체 snapshot**, **Deprecated 순수 모델**, **합성 거절/보존 Test와 기존 서버 대역 계약**으로 분류했다. 내부 Memory/계정 snapshot을 Stage 사용자 실행 API로 오인해 차단하거나 자료 삭제/구 Client 호환 기능을 추가하지 않았다. 조사한 지원 경로에서 미차단 Stage 실행 진입은 찾지 못했다. 이 결론은 소스 검토 범위이며 실제 Unity/원격 검증으로 확대하지 않는다.

#### 8-2. 선언·반사·격리·직렬화 검토

- 누적 변경 C# 29개에 대해 기존 preflight의 중복 선언/소스 구분자·GUID·asmdef 검사를 통과했다. `_now`, 전송 request, Result의 currentCandidate/currentRuntime/currentScreen과 probe 분류 변수가 선언 뒤 사용되는지 소스로 검토했다. using/namespace·named argument·await/Task 반환과 generic fake transport도 대조했다. CS0103/CS0841의 재발 징후를 찾지 못했지만 이름 해석/컴파일 성공을 보장하는 검사는 아니다.
- Editor 반사 진입 8종의 실제 선언/반환/인자와 주요 필드 14개, GameSystem 호출 메서드 7종을 대조했다. 합성 자료는 RecordBoardKey private 생성자 3인자와 RecordSubmissionCandidate internal 생성자 12인자이며 실제 선언과 일치한다. optional 개인 최고 인자는 `new object[] { null }`, parameterless 호출은 null 인자 배열로 구분한다. Editor 창은 Assembly-CSharp-Editor, GameSystem/UIManagementSystem은 Assembly-CSharp의 실제 경로다.
- enum Stage=0/Infinite=1, RecordBoardKey/Candidate·Memory/queue·LocalSaveJsonCodec/LocalSaveData·OnlineAccountCoordinator·LeaderboardViewState·Core/GameNavigationState·ResultTextFormatter의 **11개 소스가 HEAD와 동일**함을 확인했다. 저장 v7/전체 snapshot·지원 Scoring Version 계약을 임의 변경하지 않았다. 최초 보존 목록에서 GameNavigationState를 Features로 적은 검사 경로는 실제 Core 경로로 바로잡았다.
- 변경 Test C# 17개에는 실제 UGS/영구 저장 생성, 광범위한 LogAssert 무시·Assert.Ignore/Ignore/Explicit 추가가 없다. PlayMode SetUpFixture의 메모리 저장/온라인 비활성화와 Editor의 자동 Test/Batch 거절을 유지한다. 동작 Test는 fake clock/저장·인증/전송 횟수, ID/payload·전체 상태/파일 내용·지연 Task를 assertion으로 검사한다. 소스 assertion만으로 C# 동작 검증을 대신하지 않는다.
- SampleScene 전체 **1,272 block·로컬 참조 2,931개·콜백 37개·Navigation 방향 232개**의 대상과 콜백 메서드를 읽기 전용으로 대조했다. 외부 GUID 69개는 Assets/Packages/PackageCache의 metadata와 Unity builtin 분류에서 해결했다. 초기 GUID 열거가 무시된 TMP 기본 폰트를 누락해 `--hidden --no-ignore`로 수정한 뒤 누락 0개를 확인했다. 기존 UI 참조 9개/GameSystem 콜백 7개 preflight도 통과했다. Scene/Prefab 편집은 없다.

#### 8-3. 최종 정적·로컬 대역 실행 결과

실행 전 기존 fs/VM loader·SDK 주입을 다시 확인했다. record-account endpoint VM의 Unity SDK import는 Map 기반 save/leaderboard 대역으로 치환되며, transfer/JSON 서비스는 store·clock/Secret provider를 fake로 주입한다. transfer-races는 completion fixture를 import하며 completion의 실행부는 require.main으로 보호된다. 실제 환경 자격 증명/네트워크·배포·실제 저장·Unity/컴파일러를 호출하지 않는다.

실행 명령은 `node UGS/Tests/<파일>`이며 아래 **16개 모두 exit 0**이다. 별도 숫자가 없는 스크립트에 임의 통과 건수를 붙이지 않았다.

| 스크립트 | 최종 결과/판정 범위 |
| --- | --- |
| client-unity-preflight.cjs | PASS. C# 29개·GUID/asmdef·격리·Scene 연결. 기존 선택 suite Edit 254/Play 38, 별도 integration Edit 115/Play 43은 소스 준비 집계이며 Runner 실행 수 아님. |
| static-contracts.cjs | PASS. 서버/Module·계정 기존 소스 계약. |
| client-local-save-contracts.test.cjs | PASS. v7/복원·신규 쓰기·만료/재시도 경계. |
| client-account-contracts.test.cjs | PASS. scope/권한·정확한 인자·stale 응답 보호. |
| client-transfer-completion-contracts.test.cjs | PASS. 이전·전체 snapshot 교체·내부 복원/저장 경계. |
| client-verification-tools-contracts.test.cjs | PASS. Stage Repository/raw/Editor/UI guard·자동 격리·권한/민감 로그·응답 적용 순서. |
| client-account-transfer-ui-contracts.test.cjs | PASS. 생산 계정 UI 연결/격리·이전 상태 계약. |
| phase1-policy-contracts.test.cjs | PASS. 확정 정책/Phase 인계 정합성. |
| record-account-integration.test.cjs | 로컬 SDK 대역 46/46 PASS. |
| account-personal-bests.test.cjs | 로컬 SDK 대역 18/18 PASS. |
| cloud-code.test.cjs | 로컬 SDK 대역 28/28 PASS. |
| leaderboard-query-policy.test.cjs | PASS. 조회 창/공개 번호 최종 동점 순서. |
| submission-retention-policy.test.cjs | PASS. 180일 Pending/terminal replay·삭제 후 신규 판정. |
| transfer-completion.test.cjs | 로컬 SDK 대역 22/22 PASS. |
| transfer-races.test.cjs | 결정적 이전 동시성/권한 fence 대역 22/22 PASS. |
| json-snapshot-consistency.test.cjs | PASS. JSON key 순서·두 legacy Board·30개 중단 쓰기 경계와 점수 쓰기 없음. 기존 서버 계약이며 Client Stage 허용 근거 아님. |

최신 C# Test는 여전히 **준비/미실행**이다. 누적 추가 소스 case Edit 112/Play 28을 과거 사용자 919/288에 더해 예상 Runner 수를 확정하지 않는다. 실제 통과/실패/미실행 수는 Step 10의 실행 결과를 따른다. 이번 Step은 생산 코드/Test를 수정하지 않았다.

#### 8-4. 변경 범위·문서·잔여 판정

Step 8에서는 이 Task·Project Memory·Roadmap의 상태/인계만 갱신했다. Feature/System의 Step 2~7 현행 계약이 최종 소스와 일치하는지 확인했고 별도 작업 이력을 중복 추가하지 않았다. 누적 변경 C# 29개·Node 4개·문서 7개이며 Scene/Prefab·meta/asmdef·프로필·서버/Module·실제 사용자 저장 변경은 없다. C#·Scene/Prefab 추적 자산 247개의 Step 8 전후 해시 동일성, 문서 상대 링크 71개·체크 8개/미체크 3개·공백 검사를 최종 확인했다.

**완료 판정:** 최종 경로 폐쇄·선언/반사·저장 보존·Scene 연결 검토와 16스크립트 통과 및 아래 실행 인계가 준비됐으므로 Step 8을 완료 처리한다. Step 9~11과 Phase 3 전체는 미완료다. 기존 Candidate02도 이번 소스 변경의 검증 근거로 승계하지 않는다.

#### 8-5. 사용자 실행 인계 — Step 9~10의 단일 기준

**다음은 Step 9: 최신 Script Compile 확인이다.** Unity Editor로 돌아가 자동 Script Compile이 끝난 뒤 아래 형태로 알려준다. Scene 편집/재연결·UI 생성·정적 GUID 조사나 Stage 원격 probe 실행은 필요하지 않다.

`Compile=성공/실패, Scene=변경 없음, 예상 밖 Error/Warning=없음/로그`

**Scene 적용표: 대상/컴포넌트/필드/최종값 모두 변경 없음.** Step 7~8에서 기존 연결과 참조를 확인했다. AI가 Scene/Prefab을 편집하지 않았으며 기존 Settings/Outline/Stage Root 구성을 다시 만들지 않는다. Compile 실패/예상 밖 로그가 있으면 이름·메시지·Stack Trace를 전달하고 수정 후 해당 Compile을 확인한다.

Compile 확인 후 **Step 10**에서 `Window → General → Test Runner`의 필터를 해제하고 아래 두 assembly를 각각 Run All한다. AI는 Test Runner를 실행하지 않는다.

| assembly | 전체 실행에 포함할 주요 회귀 suite |
| --- | --- |
| FlowState.EditModeTests | RecordSubmissionPolicyTests, RecordLeaderboardPolicyTests, LocalSaveJsonCodecTests, OnlineRecordConfigurationTests, OnlineRecordRepositoryTests, OnlineLocalSaveScopeTests, OnlineAccountCoordinatorTests, AccountTransferCompletionTests, AccountTransferControllerTests, VerificationSessionTests, CloudCodeModuleRoutingTests, LeaderboardViewStateTests, GameNavigationStateTests, ResultTextFormatterTests 및 나머지 전체 suite |
| FlowState.PlayModeTests | AccountTransferUIIsolationTests, VerificationToolIsolationTests, ResultMenuIntegrationTests, OnlineRecoveryNoticeFlowTests, ModeResultDisplayIntegrationTests, ModeUISceneConfigurationTests, PlayModeRecordIsolationTests 및 기존 게임 진입/Infinite/Stage 거절/입력·물리 등 나머지 전체 suite |

각 실행의 **통과/실패/미실행 수, 실패 Test 이름·메시지·Stack Trace, 예상 밖 Error/Warning**을 전달한다. 가능하면 결과 파일을 보관한다. Test별 예상 로그는 기존 `LogAssert.Expect`가 소비하며 전체 로그 무시로 처리하지 않는다. 아래는 이번 영향 범위의 정확한 예상 Warning이다. 전체 assembly의 다른 기존 예상 로그도 해당 Test 소스의 Expect를 따른다.

| Test/상황 | 정확한 예상 Warning |
| --- | --- |
| OnlineRecordRepositoryTests의 만료 Test | `[OnlineRecordCoordinator] SubmissionExpired: old Pending records were removed.` |
| 제출 timeout | `[CloudCodeRecordRepository] Submission unavailable; pending retained.` (최대 시도 수만큼 각 Expect) |
| 조회 timeout | `[CloudCodeRecordRepository] Query timed out.` |
| 조회 fake 실패 | `[CloudCodeRecordRepository] Query unavailable.` |
| OnlineLocalSaveScopeTests의 손상 저장 | `[LocalRecordRepository] Save recovery used default values.` |
| ModeResultDisplayIntegrationTests의 Stage Result | `[UIManagementSystem] Only Infinite Result Data can be displayed.` |
| 같은 suite의 Stage runtime | `[UIManagementSystem] Only created Infinite runtime data can be displayed.` |

지원 종료 반환/예외는 assertion 대상이며 별도의 원격 호출/Console Warning 성공 로그를 요구하지 않는다. 실패 시 AI가 원인과 영향 범위를 수정·정적으로 확인한 뒤 필요한 suite 재실행을 안내한다. 빌드/Player·화면 통합 후보 검증은 이 Step의 필수 작업이 아니며 Phase 4에서 다룬다.

## Step 9. 사용자가 Compile하고 필요한 Scene 변경만 적용한다

### AI 선행 작업

Step 8 결과를 바탕으로 변경 전/후 Scene YAML을 읽기 전용으로 대조한다. 현재 Phase 2 구성은 필수 UI 참조 27개·GameSystem 이벤트 31개·Navigation 방향 232개가 유효하다. 수치는 기준선이며 필수 개수를 하드코딩해 실제 새 연결을 누락시키지 않는다.

Phase 3에 Scene 변경이 없으면 **추가 Scene 작업 없음**으로 명시한다. 있으면 이 Step에 실제 Hierarchy 경로/컴포넌트/필드/기존값→최종값·연결/OnClick/Navigation·문구/배치·미리보기 원복값을 기입한 후 사용자에게 인계한다. AI가 Scene/Prefab을 변경하지 않는다.

### 사용자 수동 작업

1. Unity 최신 Script Compile 완료와 Error/예상 밖 Warning 유무를 보고한다. 실패 시 로그를 전달하고 AI 수정 후 재Compile한다.
2. AI가 확정한 Scene 변경이 있는 경우에만 해당 목록을 Editor에서 적용한다. 기존 Settings handler/Outline/Stage Root 설정을 다시 만들지 않는다. 작업/미리보기 후 저장한다.
3. `Compile=성공/실패, Scene=변경 없음/적용·저장 완료, 예상 밖 Error/Warning=없음/로그`를 전달한다. fileID/GUID/YAML 조사를 직접 수행할 필요는 없다.

### AI 반영·검증

보고 후 저장된 YAML에서 필수 참조 타입·이벤트/메서드·Navigation 대상·비활성/포커스 차단·문구/배치를 다시 확인한다. 정적 판정 누락을 수동 플레이로 대신하지 않는다. 빠진 항목만 안내하고 재검사한다.

### 완료 조건

- [x] 사용자 Compile 결과와 필요한 Scene 적용/정적 재검사를 통과했다. Scene 변경 불필요 시 그 근거를 기록했다.

### Step 9 수행 결과 — 2026-11-18

#### 9-1. AI 정적 재검사 — 완료

`Assets/Scenes/SampleScene.unity`가 HEAD 기준본과 줄바꿈 정규화 후 동일하며 누적 변경 목록에도 Scene/Prefab·meta/asmdef 변경이 없음을 확인했다. 현재 Scene의 1,272 block·로컬 참조 2,931개·직렬화 콜백 37개·Navigation 방향 232개를 읽기 전용으로 다시 검사했다. 콜백 대상/스크립트/메서드와 Navigation 대상이 존재한다. Step 8의 추가 Scene 적용 없음 판정이 유효하다. 코드/Test/Scene은 수정하지 않았으며 Unity Compile·빌드·Runner·원격 호출을 실행하지 않았다.

#### 9-2. 사용자 최신 Script Compile 확인 — 완료

1. Unity Editor로 돌아가 최신 스크립트의 자동 Compile이 끝날 때까지 기다린다.
2. Console에서 Compile Error 및 예상 밖 Warning 유무를 확인한다. Scene 편집/재연결·빌드·Play/Test Runner 실행은 이번 확인에 필요하지 않다.
3. `Compile=성공/실패, Scene=변경 없음, 예상 밖 Error/Warning=없음/로그` 형식으로 결과를 전달한다. 문제가 있으면 정확한 메시지·파일/줄·Stack Trace도 전달한다.

이 확인은 정적 검사로 대체할 수 없는 Unity의 실제 컴파일 결과다. 과거 Phase 2 Compile/Test 결과나 Step 8의 Node PASS를 최신 Compile 성공으로 간주하지 않는다.

2026-11-18 사용자 보고: **Unity Script Compilation 성공**, 해당 Compilation에 **예상 밖 Error/Warning 없음**. 이번 Phase 3 최신 소스의 실행 근거로 반영한다. 보고 이후 AI는 코드/Test/Scene을 추가 수정하지 않았다.

#### 9-3. 결과 반영과 완료 판정 — 완료

사용자 최신 Compile 성공/예상 밖 로그 없음 보고와 9-1의 Scene 기준본 동일성·연결 정적 재검사를 연결해 **Step 9를 완료 처리**했다. Scene 변경 불필요 판정은 유지한다. 실제 로그 파일은 제공받지 않았으며 AI가 Unity Compile을 실행하거나 로그 원문을 검토했다고 기록하지 않는다.

## Step 10. 사용자가 EditMode·PlayMode Test를 실행한다

### AI 선행 작업

계산/정책/직접 API의 EditMode Test와 생산 Scene/입력/상태 연동의 PlayMode Test를 준비한다. Step 2~7의 suite 외에도 기록 서비스/저장/계정 공통 의존성이 있으므로 최종 실행은 assembly 전체로 한다. 과거 919/288은 기준선이며 최신 성공 수가 아니다.

### 사용자 수동 작업

1. `Window → General → Test Runner`에서 `FlowState.EditModeTests`를 **필터 없이 Run All**한다.
2. `FlowState.PlayModeTests`도 **필터 없이 Run All**한다. AI는 Test Runner를 실행하지 않는다.
3. 두 Run의 통과/실패/미실행 수·실패 이름/메시지/Stack Trace·예상 밖 Error/Warning을 전달한다. 가능하면 결과 파일도 보관한다. 로그/파일은 실제 제공받은 범위만 확인했다고 기록한다.
4. 실패 시 AI가 코드/fixture·정적 검사 후 재실행 범위를 지정한다. 해당 원인으로 영향이 있는 suite를 다시 실행하며 새 변경/미해결 우려가 없으면 무조건 전체를 반복하지 않는다.

### AI 반영·검증

Stage 요청의 인증/전송/저장 호출 0회, Infinite 제출/조회/Pending/이전/snapshot 보존과 UI/도구 격리의 실제 결과를 계약 표에 반영한다. 로컬 Node PASS를 C# Compile/Runner PASS로 대체하지 않는다. 실패/미실행을 통과로 기입하지 않는다.

### 완료 조건

- [x] 최신 필수 EditMode/PlayMode 실행 결과가 모두 통과했고 실패·미실행/예상 밖 로그가 해결됐다.

### Step 10 수행 결과 — 2026-11-18

| 사용자 실행 보고 | 최신 결과 |
| --- | --- |
| EditMode | **1031개 시도·1031개 성공**. 관련 예상 밖 Error/Warning 없음. |
| PlayMode | **316개 시도·316개 성공**. 관련 예상 밖 Error/Warning 없음. |

8-5의 두 assembly 전체 실행 인계에 대한 사용자 결과로 기록한다. 실패 또는 별도 미실행 항목은 보고되지 않았으며 시도한 항목은 모두 성공했다. 과거 Phase 2의 919/288과 구분되는 최신 Phase 3 검증이다. C# guard·부작용 0회/원본 보존·Infinite 수명/계정/도구·생산 Scene 통합 Test를 포함한 결과로 연결한다.

실행 주체는 사용자다. AI가 Runner를 실행하지 않았으며 XML·Console 원문/Stack Trace는 제공되지 않아 직접 검토하지 않았다. 실패/예상 밖 로그가 없고 보고 뒤 코드/Test 변경도 없으므로 추가 재실행은 요구하지 않는다. **Step 10 완료**다.

## Step 11. Phase 3 완료를 판정하고 Phase 4에 인계한다

### AI 작업

11-1. 아래 완료 표와 Roadmap 9 Phase 3의 세 조건을 실제 코드·정적/로컬 결과·Step 9~10 사용자 근거에 연결한다. Stage 자료가 없다는 사용자 전제를 원격 전수조사 결과로 바꾸지 않는다.

| 완료 조건 | 필요한 근거 |
| --- | --- |
| 지원되는 Stage 기록 생성/조회/표시/조작/제출 불가 | 전체 호출망/guard·직접 Stage/과거 probe 거절 Test·인증/전송/저장 부작용 0회·생산 UI/도구 연결 확인 |
| Infinite 조회/제출/계정 이전·재시작 보존 | 실제 정책/coordinator/codec Unit Test·서버 함수의 오프라인 SDK 대역·최신 EditMode/PlayMode 결과·Phase 2 Infinite 회귀 |
| 기존 자료 자동 삭제/Infinite 변환·병합 없음 | 내부 저장/복원/snapshot 경계 정적 검사·저장 실패/환경 격리/계정 교체 Test·변경 범위 확인 |

11-2. Step 1~10 완료 근거가 있으면 Phase 3을 완료 처리하고 Roadmap/Feature/System/Project Memory를 갱신한다. 준비만 끝났거나 실제 Compile/Runner 실패가 남으면 완료하지 않는다.

11-3. Phase 4에 최신 소스/Scene/Test·Phase 2 Candidate02와의 차이·Player/시각 확인 필요 범위를 인계한다. Phase 3 코드 변경 뒤에는 기존 Candidate02가 최신 소스를 대표하지 못한다는 점을 기록한다. 최종 후보 경로·덮어쓰기/보존은 사용자의 최신 지시를 따르며 확정된 과거 보존 요구를 다시 만들지 않는다.

11-4. Phase 4의 Player 통합 후보 판정·필요한 제한적 실서비스 검증은 별도 단계다. 이 Task의 fake 호출 0회/서비스 대역 PASS를 원격 검증·전체 Roadmap 9 완료로 확대하지 않는다. 추가 Module 변경이 실제 필요해지면 변경 필요성/복구 기준과 기존 승인 경계를 먼저 검토하고 독단 배포하지 않는다.

### 사용자 수동 작업

Step 9~10 결과가 모두 있으면 없음. 정적 검사로 처리할 수 있는 추가 항목을 요청하지 않는다.

### 완료 조건

- [x] Phase 3 세 조건의 실제 근거와 Phase 4 잔여 범위를 기록하고 Phase 3 완료 여부를 정확히 반영했다.

### Step 11 수행 결과 — 2026-11-18

#### 11-1. Phase 3 최종 완료 판정

| Phase 3 완료 조건 | 연결한 실제 근거 | 판정 |
| --- | --- | --- |
| 지원되는 Stage 기록 생성/조회/표시/조작/제출 불가 | Step 2~4 생산/직접/전송/재시도 guard, Step 6 Editor/과거 probe 거절, Step 7 UI/Result 통합, 8-1 경로 폐쇄 표와 Step 9~10 최신 Compile/1031·316 성공 보고 | 충족 |
| Infinite 조회/제출/이전·재시작 보존 | 정책/coordinator/codec·이전/전체 snapshot·Scene 격리 회귀 Test의 최신 성공 보고, Step 8 로컬 SDK 대역의 제출/조회/수명/이전·동시성/JSON snapshot 성공 | 충족 |
| 기존 자료 자동 삭제/Infinite 변환·병합 없음 | 내부 Memory/codec/계정 snapshot 보존·정적 변경 범위, 내구 저장 실패/환경 격리·전체 snapshot 교체/재시작 Test의 최신 성공 보고 | 충족 |

**Step 1~11 및 Phase 3을 완료 처리한다.** 사용자의 Stage 자료 없음 전제는 유지하며 이를 원격 전수조사 결과로 바꾸지 않는다. 서버 계약/Module 배포·자료 삭제/보관·구 Client 지원을 추가하지 않았다. Feature/System·Roadmap/Project Memory의 최신 검증/다음 작업 상태도 갱신한다.

#### 11-2. Phase 4 인계

- 최신 기준은 이번 Phase 3 소스와 사용자 Compile 성공·EditMode 1031/1031·PlayMode 316/316 성공 및 예상 밖 Error/Warning 없음이다. Scene/Prefab은 Phase 2 기준본을 유지한다.
- Phase 2 `Build/ProductionQA/Candidate02/`는 당시 사용자 검증 기준선이며 Phase 3의 기록/Editor/UI 변경을 포함한 최신 Player로 간주하지 않는다. Phase 4에서 최신 소스로 사용자가 빌드한 후보와 Player 결과를 연결한다. 후보 경로/덮어쓰기·보존은 사용자의 최신 지시를 따르며 과거 Candidate 실물 보존 의무를 새로 만들지 않는다.
- Phase 4의 잔여 범위는 Infinite 단일 시작/재시작·Pause/Result/복귀·화면/포커스·온라인/저장/계정 경계의 Player 통합 확인과 최종 전환 후보 판정이다. 정적/자동 검증으로 처리한 항목을 사람의 빠른 입력 재현이나 Stage 원격 probe로 다시 요구하지 않는다.
- 로컬 SDK 대역/Unity 자동 Test 성공은 실제 원격 서비스 성공·복구 시험/공개 승인과 구분한다. 필요한 제한적 실서비스 확인이나 Module 변경이 실제 발생하면 기존 승인·환경/자료 보존·복구 경계를 따른다. 이번에 원격 배포·삭제·빌드·Player 실행은 수행하지 않았다.

이 인계를 마쳤으므로 **Step 11 완료**다. 다음은 **Roadmap 9 Phase 4**이며 Phase 4와 Roadmap 9 전체는 아직 미완료다. 이번 사용자 보고 반영을 위해 코드/Test/Scene을 추가 수정하거나 Unity 재실행을 요구하지 않는다.

# 영향 범위

Step 1~8의 코드/검증과 Step 9~10의 사용자 최신 실행 결과를 연결하고 Step 11의 최종 판정/Phase 4 인계를 반영했다. 이번 사용자 보고 반영의 코드/Test/Scene 변경은 없다. 현재 누적 변경은 C# 29개·Node 4개·문서 7개다. Scene/Prefab·Build Profile·실제 사용자 저장·서버/Module 변경은 없다.
후속 실제 구현은 각 Step 완료 시 변경 파일/이유/계약·검증 결과를 이 문서에 기록한다. 기존 사용자의 미커밋 변경을 보존한다.

# 검증 내용

- Step 순서·AI/사용자 책임·정적 검증/Unit Test·수동 작업/완료 조건을 대조한다.
- 조사에 인용한 소스/기존 Test/Node 파일·관련 문서와 날짜/작업 번호·상대 링크를 확인한다.
- 선행 완료 항목 재작업, 제외된 Stage 보관/이관·구 Client 지원, 원격/Unity 실행·Scene 편집 요구가 섞이지 않았는지 확인한다.
- 문서/코드 준비와 실제 사용자 Compile/Runner 결과를 구분한다. Step 9~10의 사용자 성공 보고와 Step 11의 최종 근거를 확인한 항목만 완료 체크한다.

# 검증 결과

최초 문서 작성 시에는 Step 1~11 순서·미착수 완료 체크 11개·상대 링크·조사 대상 소스 18개·기존 Unity suite·로컬 스크립트 14개·Roadmap/Memory 역링크와 Task 공백 검사만 통과했다. 당시에는 서비스 대역을 실행하지 않았다.

이후 Step 1에서 소스/호출자·fixture를 대조하고 위 14스크립트를 실제 실행했다. 결과는 **13통과/1실패**, 상세 실행 건수·한계·기존 실패와 Step 8 인계는 **1-4**를 따른다. Step 1 기록 후 변경 문서 상대 링크 63개·완료 체크 1개/미완료 10개·하위 결과 1-1~1-4·fixture 10파일/16호출·기준선 스크립트 14개·Feature 소스 18개 존재·변경 문서 3개 범위 검사를 통과했다. `git diff --check`도 통과했다. 생산 코드/Unit Test/Scene·프로필·서버는 수정하지 않았으며 Unity Compile/Runner/빌드/Player·원격 Test는 실행하지 않았다.

위 결과는 Step 1 시점의 기록이다. Step 2는 **2-4의 최종 관련 8스크립트 통과** 및 신규 거절/복원 Test 준비로 판정했다. Step 1의 기존 문법 실패는 해결됐다. 최종 변경 범위(생산 C# 4/Test C# 12/Node 2/문서 5)·추가 소스 case Edit 17/Play 2·문서 상대 링크 67개·완료 2개/미완료 9개 검사와 `git diff --check`도 통과했다. 최신 C# Compile/Unit Test 실행은 사용자 Step 9~10 대기다.

최신 Step 3은 **3-4의 관련 10스크립트 통과**와 추가 Test 소스 Edit 30/Play 1 준비로 판정했다. 최신 문서 상태/링크·완료 체크 3개/미완료 8개·변경 범위 및 공백을 정적으로 확인했다. 최신 Unity Compile/Runner·Phase 3 전체는 여전히 미완료다.

Step 4는 **4-4의 관련 10스크립트 통과**와 추가 EditMode 소스 case 22개 준비로 판정했다. 완료 체크 4개/미완료 7개·현재 진행 상태/상대 링크·누적 변경 범위와 공백을 정적으로 확인했다. 최신 C# Compile/Runner·Phase 3 전체는 미완료다.

Step 5는 **5-4의 관련 12스크립트 통과**와 추가 EditMode 소스 case 12개 준비로 판정했다. 생산 C# 전체 해시 동일성, 완료 체크 5개/미완료 6개·현재 진행 상태/상대 링크·변경 범위와 공백을 정적으로 확인했다. 최신 Unity Compile/Runner·Phase 3 전체는 미완료다.

Step 6은 **6-4의 관련 10스크립트 통과**와 추가 EditMode 26개/PlayMode 14개 소스 case 준비로 판정했다. 완료 체크 6개/미완료 5개·문서 상태/상대 링크·변경 범위와 공백을 정적으로 확인했다. 최신 Unity Compile/Runner·Phase 3 전체는 미완료다.

Step 7은 **7-4의 관련 12스크립트 통과**와 추가 EditMode 5개/PlayMode 11개 소스 case 준비로 판정했다. Scene 참조 9개/콜백 7개·완료 체크 7개/미완료 4개·문서 링크/현재 상태·변경 범위/공백을 정적으로 확인했다. 최신 Unity Compile/Runner·Phase 3 전체는 미완료다.

Step 8은 **8-1~8-5의 경로/선언·반사/격리/Scene 검토와 16스크립트 통과·실행 인계 준비**로 판정했다. 최신 C# Compile/Runner는 실행하지 않았으며 사용자 Step 9~10 대기다. 완료 체크 8개/미완료 3개와 문서·변경 범위/공백을 최종 대조했다.

최신 Step 9~11은 사용자 Compile 성공·EditMode 1031/1031·PlayMode 316/316 성공과 각 예상 밖 Error/Warning 없음 보고를 Scene 정적 재검사·16스크립트 통과·최종 조건 표에 연결해 완료 처리했다. XML/Console 원문은 제공되지 않았으며 AI 직접 실행/원문 검토로 기록하지 않는다. Phase 3 완료, Phase 4·Roadmap 9 전체 미완료다.

# 후속 작업

**Step 1~11 / Phase 3 완료.** 사용자 최신 Compile 성공·EditMode 1031/1031·PlayMode 316/316 성공과 각 예상 밖 Error/Warning 없음을 반영했다. 다음은 **Roadmap 9 Phase 4의 최신 Player 통합 후보 검증/판정**이다. 인계 범위는 **11-2**를 따른다. AI가 Unity Compile/빌드/Runner·원격 검증을 수행하지 않는다.

# 관련 문서

- [Roadmap 9](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md)
- [공통 의존성과 검증 경계](../../04_Implementation_Roadmap/README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [RecordSubmissionSystem](../../02_Systems/RecordSubmissionSystem.md)
- [AccountConnectionSystem](../../02_Systems/AccountConnectionSystem.md)
- [RecordSubmission](../../03_Features/RecordSubmission.md)
- [Leaderboard](../../03_Features/Leaderboard.md)
- [AccountTransfer](../../03_Features/AccountTransfer.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)

# 관련 작업 기록

- [Systems/Features 문서 정합성 정리 — Phase 3 구현 착수와 구분](../20261117_03_SystemFeatureDocumentAlignment.md)
- [Phase 1 확정 정책·조사·완료 인계](20261111_02_Phase1ManualSteps.md)
- [Phase 2 완료·최신 Candidate02/919·288 검증·Phase 3 인계](20261112_01_Phase2ManualSteps.md)

# 작성 완료 기준

- 실제 변경/검증을 Step 1~11에 배치하고 세부 작업·담당·사용자 작업·완료 조건을 명시했다.
- 정적으로 판정할 대상과 Unit Test로 판정할 동작을 구분하고 기존 실제 생산 코드/fixture에 연결했다.
- Step 1~11의 근거를 연결해 Phase 3 완료를 기록하며 Phase 4/Player 검증·Roadmap 9 전체 완료와 구분한다. 사용자 보고를 AI 직접 실행/XML·로그 원문 검토로 기록하지 않는다.
