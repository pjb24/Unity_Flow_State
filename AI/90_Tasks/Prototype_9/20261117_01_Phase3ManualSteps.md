# 작업 정보

## 작업명

Prototype 9 Phase 3 — Stage 기록 접근 차단과 Infinite 기록 보존 실행 절차

## 작업 일자

20261117

## 작업 담당자

- AI: 경로 조사·생산 코드/Unit Test 수정·정적 검사·로컬 대역 Test·문서/사용자 인계.
- 사용자: Unity Script Compile·필요한 Scene 적용·Unity Test Runner 실행. Player 통합 후보 검증은 Phase 4에서 수행한다.

## 작업 상태

**Task 작성·사전 조사 완료. Phase 3 잔여 구현 Step 1~11은 미착수다.**
이 문서 작성은 Phase 3 구현/실행 완료를 의미하지 않는다. 이번 작업에서는 생산 코드·Test·Scene·프로필·서버를 변경하지 않는다.

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
| 1 | 진입/부작용/내부 자료 보존 표와 정적·로컬 기준선 확정 | AI |
| 2 | 신규 Stage 후보 생성·저장 차단과 Infinite 후보 Test | AI |
| 3 | Stage 직접 조회/제출·전송 우회 차단과 호출 0회 Test | AI |
| 4 | Infinite Pending/재시도/만료/내구 저장 회귀와 우회 재전송 차단 | AI |
| 5 | Infinite 계정 이전/snapshot/환경 격리 보존 검증 | AI |
| 6 | Editor Stage 버튼/과거 probe 직접 진입 차단 | AI |
| 7 | 선행 UI/Result/호출자 통합과 기존 fixture 전환 | AI |
| 8 | 최종 정적 검사·로컬 대역 Test·실행 목록/문서 인계 | AI |
| 9 | 최신 Compile·필요한 Scene 적용 및 AI YAML 재검사 | 사용자 + AI |
| 10 | EditMode/PlayMode 전체 실행·실패 수정·재검증 | 사용자 + AI |
| 11 | Phase 3 완료 판정과 Phase 4 인계 | AI |

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

- [ ] 모든 지원 진입에 소유 Step/guard/Test가 연결되고 내부 형식 보존과 생산 실행 차단이 구분됐다. 변경 전 정적/로컬 결과와 실패·한계를 기록했다.

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

- [ ] 신규 Stage 후보/저장 진입을 직접 호출해도 부작용 전에 거절하도록 구현했고 Infinite/내부 자료 보존 Test를 준비했다.

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

- [ ] 지원되는 Stage 직접 조회/제출 경로가 인증/전송 전에 거절되고 Infinite 요청 계약의 회귀 Test를 준비했다.

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

- [ ] Stage 재전송 우회가 차단되고 Infinite 재시도/만료/내구 저장의 기존 계약을 검증할 Test를 준비했다.

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

- [ ] Infinite 이전/snapshot/저장 계약이 유지되고 Stage 접근 차단이 내부 읽기·계정/환경 데이터를 파괴하지 않는 Test를 준비했다.

## Step 6. Editor Stage 도구와 과거 probe 번호를 차단한다

### AI 작업

6-1. OnlineRecordVerificationWindow의 Stage 조회/제출/검증 버튼·Stage 선택 toggle·legacy 검증·direct-write/account mismatch 경로를 모두 조사한다. 사용자 조작은 숨기거나 비활성화하고 지원 종료 이유/Infinite 대체 경로를 표시한다.

6-2. 버튼 숨김과 별도로 기존 메서드/숫자 probe 직접 호출을 인증·기준선 조회·격리 저장 생성·SDK/HTTP 요청 전에 거절한다. 특히 probe 2/5/6과 GetSelectedBoard 등 과거 Stage 선택 진입을 표에 명시하고 다른 번호도 전수 대조한다.

6-3. Stage 전용 코드는 삭제 대신 적용 가능한 Obsolete/Deprecated 표기와 guard/이유를 보존한다. Infinite 검증/안전 로그/권한·timeout·Late 응답 처리와 자동 실행 금지·PlayMode 격리의 기존 보호는 약화하지 않는다.

### 정적 검증·Unit Test

- `VerificationSessionTests`, `CloudCodeModuleRoutingTests`, PlayMode `VerificationToolIsolationTests`에서 기존 Stage 메서드/과거 probe 직접 진입도 상태 불변·인증/전송 0회인지 검사한다. 실제 검증창 버튼이나 외부 서비스를 실행하지 않는다.
- `UGS/Tests/client-verification-tools-contracts.test.cjs`를 새 정책에 맞게 보완한다. Stage 버튼 존재를 요구하는 옛 소스 assertion은 새 거절 계약으로 대체하되 격리/민감 로그/권한 검사는 유지한다.
- CloudCodeService/HTTP/direct-write 호출 검색과 각 guard의 선후 관계를 정적으로 대조한다. 문자열이 남았다는 이유만으로 미완료 판정하거나, Obsolete만 붙였다고 접근 차단 완료 판정하지 않는다.

### 사용자 수동 작업

없음. Stage probe/direct-write 거절을 실제 서버에 요청해 확인하지 않는다.

### 완료 조건

- [ ] Stage 도구 UI와 직접/과거 번호 진입이 차단되고 Infinite 도구·자동 격리/권한 보호 Test를 준비했다.

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

- [ ] 새 기록 경계가 기존 Infinite 화면/복귀/상태 계약과 연결되고 Stage 성공 fixture의 용도별 전환·회귀 Test 준비를 마쳤다.

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

- [ ] 경로 폐쇄 표·소스/저장/계정/도구 검사와 오프라인 대역 Test 결과가 연결됐고 사용자 Compile/Scene/Runner 인계를 준비했다.

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

- [ ] 사용자 Compile 결과와 필요한 Scene 적용/정적 재검사를 통과했다. Scene 변경 불필요 시 그 근거를 기록했다.

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

- [ ] 최신 필수 EditMode/PlayMode 실행 결과가 모두 통과했고 실패·미실행/예상 밖 로그가 해결됐다.

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

- [ ] Phase 3 세 조건의 실제 근거와 Phase 4 잔여 범위를 기록하고 Phase 3 완료 여부를 정확히 반영했다.

# 영향 범위

이번 작성은 새 Task와 진행 안내/Project Memory 링크만 변경한다. 생산 C#/Test·Scene/Prefab·Build Profile·저장·서버/Module 변경은 없다.
후속 실제 구현은 각 Step 완료 시 변경 파일/이유/계약·검증 결과를 이 문서에 기록한다. 기존 사용자의 미커밋 변경을 보존한다.

# 검증 내용

- Step 순서·AI/사용자 책임·정적 검증/Unit Test·수동 작업/완료 조건을 대조한다.
- 조사에 인용한 소스/기존 Test/Node 파일·관련 문서와 날짜/작업 번호·상대 링크를 확인한다.
- 선행 완료 항목 재작업, 제외된 Stage 보관/이관·구 Client 지원, 원격/Unity 실행·Scene 편집 요구가 섞이지 않았는지 확인한다.
- 문서 작성 완료와 구현/Runner 완료를 구분하고 실행 체크는 미체크로 유지한다.

# 검증 결과

문서 작성 후 Node 읽기 전용 검사로 Step 1~11 순서·미착수 완료 체크 11개·상대 링크·조사 대상 소스 18개·명시한 기존 Unity suite·로컬 스크립트 14개·Roadmap/Memory 역링크와 Task 공백 검사를 통과했다. 변경한 추적 문서의 `git diff --check`도 통과했다.
생산 코드/Unit Test/Scene·프로필·서버는 수정하지 않았으며 Unity Compile/Runner/빌드/Player나 서비스 대역/원격 Test를 실행하지 않았다. 완료한 것은 Task 문서 준비와 문서 자체의 정적 확인이다.

# 후속 작업

다음은 **Step 1의 진입 경계/기준선 확정**이다. Step 9 이전에 사용자 Compile/Scene/Runner/빌드나 실제 인증/제출/계정 이전을 시작할 필요가 없다.

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
- 미착수인 구현/사용자 실행을 완료로 표시하지 않으며 현재 작업은 문서 준비 완료로만 기록한다.
