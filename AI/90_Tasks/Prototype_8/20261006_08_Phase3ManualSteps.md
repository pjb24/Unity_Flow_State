# 작업 정보

## 작업명

Prototype 8 Phase 3 — 제출·조회 제한 해소 및 수동 적용 절차

## 작업 일자

20261006

## 작업 담당자

- AI: 구현, 정적 검사, Unit Test 작성, 로컬 대역 Test 실행, 사용자 실행 절차 준비.
- 사용자: Unity 컴파일·Test Runner 실행, verification Module 빌드·게시, 실제 서비스·화면 확인, 필요한 Scene 수정.

## 작업 상태

**계획 작성 완료 / Phase 3 구현·실행 검증 대기.** 아래 체크박스는 해당 Step의 실제 결과를 확인한 뒤 완료 처리한다. 이번 작업은 실행 계획 작성이며 서버·Client·Scene·서비스 설정을 변경하지 않았다.

# 작업 목적

제출 이력이 128건에 도달하면 막히는 문제와 보드에 100명보다 많은 기록이 있으면 조회가 실패하는 문제를 해결한다. 오래된 제출 이력을 정리하면서 기존 최고 기록·공개 번호·기기 이전·중복 제출 방지를 유지한다.

사용자는 수백 번 플레이하거나 180일을 기다리는 대신, 자동 Test 결과를 확인하고 실제 계정으로 제출·조회·화면을 확인한다.

# 작업 대상

- 서버: `UGS/CloudCode`, `UGS/Modules/FlowStateVerification`.
- Client: 제출 후보·Local Save·Pending 재시도·Leaderboard 조회/표시.
- 검증: `UGS/Tests`, `Assets/Tests/EditMode`, `Assets/Tests/PlayMode`, 필요한 격리 검증 도구.
- 게시 대상: `Assets/CloudCode/FlowStateVerification.ccmr`의 C# Module.

# 작업 전 상태

- `submit-record.js`는 논리 계정 ledger의 entries가 128건이면 `LedgerCapacity`로 실패한다.
- `query-records.js`는 offset 0, limit 100 단일 페이지를 읽고 total이 100보다 크면 `VerificationBoardCapacity`로 실패한다.
- 계정 자동 준비와 논리 계정 ledger 생성은 Phase 2의 `account-provisioning-service.js`에 이미 있다. Phase 3에서는 이를 재사용하고 직접 첫 제출 경로·부분 실패 복구를 보강한다.
- `submission-retention-policy.js`와 `leaderboard-query-policy.js`는 정책 계산이다. 실제 receipt 분할·정리와 다중 페이지 조회의 구현 완료 근거가 아니다.
- Local Save 현재 version은 6이다. Pending 생성 시각 저장·이전 파일 migration은 구현 시 실제 모델과 대조한다.
- 최신 사용자 보고: Unity 컴파일 및 예상 밖 Error/Warning 없음, EditMode 887개·PlayMode 254개 성공. 이는 Phase 3 변경 전 기준이다.
- 사용자 임시 관리자 Key 폐기·역할 회수 보고는 확보돼 있다. Phase 2 문서 일부에는 이전 대기 상태가 남아 있으므로 Step 1에서 최신 보고와 상태를 맞춘다.

# 조사 내용

로드맵 Phase 3, Phase 1 Step 3·4 및 후속 인계, Phase 2 Task, RecordSubmission·Leaderboard 계약, 실제 제출·조회·계정 준비 소스를 대조했다.

계약의 핵심은 다음과 같다.

- 서버 terminal receipt: Submitted/Rejected 확정 서버 시각부터 정확히 180일 보관. 논리 계정별 시간 분할 저장.
- Local Pending: 생성 시각부터 180일에 `SubmissionExpired`로 정리. 생성 시각 없는 이전 파일은 최초 업그레이드 로드 시각을 한 번 저장.
- 보관 중 같은 계정·ID·payload는 기존 결과, 다른 payload는 `SubmissionIdConflict`. receipt가 실제 정리된 뒤 같은 ID는 새 제출로 처리.
- 진행 중 제출·최고 기록·계정·공개 번호·Leaderboard 행과 metadata는 terminal receipt 정리와 분리해 보존.
- TOP 최대 10행, AROUND YOU 최대 7행. 전체 순위는 공동 순위이며 동점 표시 순서는 score → acceptedAt → 정확한 공개 번호 순서.
- 자동 타이머 갱신 대신 최초 진입·명시적 Retry/새로고침·재진입에서 조회.
- 기존 5초 작업 제한과 기기 이전의 활성 연결 검사를 유지. 대량 조회를 위해 대기 제한을 늘리는 방식으로 완료 처리하지 않는다.

# 작업 내용

## 진행 순서

Step 1~6은 AI 구현·검증 작업이다. **사용자는 Step 7부터 직접 실행한다.** Step 7~9는 앞 단계 준비가 완료된 후 진행한다.

| Step | 목적 | 직접 수행자 |
| --- | --- | --- |
| 1 | 기존 구현·서비스 제약·변경 목록 확정 | AI |
| 2 | 신규 계정 첫 제출 자동 준비 확인·보강 | AI |
| 3 | 128건 제한을 receipt 분할·180일 정리로 대체 | AI |
| 4 | Local Pending 생성 시각·만료·migration 구현 | AI |
| 5 | 100명 초과 조회·전역 순위 구현 | AI |
| 6 | 자동 Test·정적 검사·수동 실행 도구 준비 | AI |
| 7 | Unity 컴파일·전체 Edit/PlayMode Test 실행 | 사용자 |
| 8 | 게시 전 백업·verification Module 게시 | 사용자 |
| 9 | 실제 제출·조회·Offline 복구·화면 확인 | 사용자 |
| 10 | 근거 대조·Phase 3 완료 판정 | AI + 사용자 결과 |

## Step 1. 구현 시작 전에 변경 범위를 확정한다

**목적:** 기존 기능을 재사용하고 저장·조회 변경에 필요한 제약을 먼저 확인한다.

AI 작업:

1. Phase 2 최신 사용자 성공 보고·Key 폐기·역할 회수·검사용 계정 삭제 보고와 Task/Roadmap의 완료 상태를 대조한다.
2. 신규 계정 준비, 논리 계정 ledger, 기기 이전의 제출 예약, Client Pending, Module 서비스 adapter의 실제 호출 경로를 정리한다.
3. 실제 사용하는 Unity 공식 API 문서와 설치 SDK에서 페이지 조회·저장 item 크기/개수·CAS·삭제·시간 제한을 확인하고 출처·확인일을 기록한다.
4. 시간 분할 단위·receipt 검색 방법·정리 실행 계기·기존 ledger 전환 절차·조회 snapshot 일관성 방법을 확정한다. 단일 객체 무한 누적이나 전체 행을 무제한 순회하는 임시 해법과 구분한다.
5. 삭제 API/페이지 API가 Module adapter에 필요하면 변경 목록에 포함한다. 필요한 실제 서비스 검증이 대역만으로 충분하지 않다면 Step 9에 안전한 격리 검사를 별도로 준비한다.

### Step 1-1. 계약·설계 확정 — 완료 (2026-10-06)

- 선택 규칙, Cloud Save/Leaderboard 공식 계약, receipt 용량 예산, lazy migration 및 provider-neutral port 경계를 확정했다.
- `check-phase3-technical-contracts.cjs`는 로컬 코드·문서·용량 예산을 읽기 전용으로 검사한다. 원격 UGS 권한·게시 동작은 Step 9에서 별도로 확인한다.

### Step 1-2. 어댑터·저장소 기반 구현 — 완료 (2026-10-06)

- 완료: Module adapter의 Private Custom 조건부 delete 및 Leaderboard player-range 경로와 대역 구현, receipt store의 bounded ID/create-if-absent/CAS/conditional delete·manifest·locator, v1→v2 lazy-migration planner 및 receipt-first/ledger-CAS 전환 helper와 단위 Test.
- shard에 receipt를 append하는 원자적 갱신, locator를 이용한 replay, 정리 재개, v2 ledger 제출 경로 연결은 Step 2·3의 실제 기능 구현·완료 조건에 속한다.
- 이 하위 Step은 설계만 다루던 Step 1의 범위를 넘은 기반 구현을 명시적으로 분리했다. 실제 제출 경로 전환과 128건 제한 제거를 완료했다고 주장하지 않는다.

### Step 1에서 확정한 규칙

1. 신규 계정 ledger는 기존 논리 계정 준비 흐름에서 create-if-absent로 생성한다. 동시 요청·timeout·응답 유실 뒤에는 최신 상태를 재조회하고 CAS로 복구하며, 무조건 덮어쓰지 않는다.
2. terminal receipt는 계정별 UTC 시간 버킷의 다중 shard에 저장한다. Cloud Save Private Custom container는 custom ID별 총 key 수·총 크기 한도를 넘지 않게 관리하고, 시간 버킷 container가 목표 바이트 또는 key 예산에 도달하면 같은 시간 버킷의 overflow container로 rollover한다. shard는 128건 고정 제한이 아니다. 127/128/129/256은 기존 제한을 넘는 처리와 rollover를 검증하는 Test 값이다.
3. terminal receipt는 180일 동안 같은 제출 ID의 중복·payload 충돌 판정에 사용한다. 180일 뒤에는 논리적으로 만료하며, 실제 삭제 성공 뒤에만 같은 ID를 신규 제출로 허용한다.
4. 만료 정리는 제출·재전송·계정 상태 요청에서 제한된 수의 만료 shard만 처리하는 opportunistic cleanup으로 수행한다. 삭제 실패·응답 유실은 삭제 성공으로 간주하지 않으며, 다음 안전한 요청에서 재대조한다.
5. 현재 테스트 자료와 출시 후 legacy 자료는 모두 lazy migration을 기본 경로로 사용한다. 계정의 제출·조회 시 v1과 새 receipt 구조를 대조·전환하고, 성공 뒤 migration marker를 CAS로 확정한다.
6. 관리자 일괄 migration은 선택 계정을 미리 전환하는 보조 도구다. lazy migration을 대체하지 않으며, 누락·중단된 계정은 다음 사용에서 lazy migration으로 전환한다.
7. TOP 10과 AROUND YOU 최대 7행은 API 기반으로 독립 조회한다. Client에 페이지 이동 UI를 제공하지 않고 전체 보드를 무제한 순회해 순위를 계산하지 않는다.
8. 순위는 score 기준 competition rank를 사용한다. 같은 score 내 표시는 acceptedAt 오름차순, 정확한 10자리 공개 번호 오름차순으로 안정화한다.
9. 조회 중 페이지 누락·중복·rank/total 불일치·공개 번호 매핑 실패가 있으면 부분 목록을 Success 또는 Empty로 표시하지 않고 Error로 처리한다. 자동 새로고침·자동 재시도는 하지 않는다.
10. 제출 정책·receipt·순위 규칙은 provider-neutral port 뒤에 둔다. 현재 UGS 구현체는 계정 저장·receipt 저장·Leaderboard 읽기/쓰기를 구현하며, 포트는 서비스의 전체 transaction을 보장한다고 가정하지 않는다.

### Step 1에서 확인한 기술 계약 — 2026-10-06

- [Cloud Save OpenAPI](https://docs.unity.com/en-us/oas-cloud-save/1.0.0)에 따라 Private Custom Data는 custom ID별 Private access class에 최대 2,000 key와 총 5 MiB를 저장할 수 있다. key 하나 또는 shard 하나의 크기만이 아니라 같은 custom ID의 총량을 관리한다.
- 같은 OpenAPI에 따라 Private Custom batch는 최대 20 item이며 원자적으로 성공하거나 실패한다. 기존 항목의 set/delete에는 writeLock을 포함할 수 있고, 충돌은 HTTP 409이다. 생성 시 writeLock은 생략한다.
- 같은 OpenAPI에 따라 Private Custom item delete는 key별 DELETE와 선택적 writeLock을 제공한다. 현재 Module adapter에는 이 호출이 없으므로 Phase 3 변경 목록에 포함한다.
- [Leaderboard Client API](https://docs.unity.com/en-us/oas-leaderboards/1.0.0)는 player-centered range 응답에 rank·score·metadata를 제공한다. 현재 Module adapter에는 range 호출이 없으므로 Phase 3 변경 목록에 포함한다.

### Step 1 저장·조회 설계 결과

#### receipt 저장 layout

- terminal receipt의 현재 최대 payload 직렬화 예시는 157 bytes, `{ id, payload, status, reason, acceptedAt }` envelope 예시는 약 290 bytes다. 새 receipt envelope의 상한은 1 KiB로 제한하고, 이를 넘는 값은 저장 전에 안전하게 거부한다.
- receipt shard의 목표 크기는 256 KiB다. 따라서 1 KiB 상한 receipt만 저장되어도 shard당 최소 256건을 수용하며, 실제 더 작은 receipt는 byte 예산까지 더 많이 저장할 수 있다. 128건은 저장 제한이 아니라 rollover 회귀 Test 경계값이다.
- 한 receipt container의 목표 예산은 4 MiB와 1,600 key다. Cloud Save custom ID의 5 MiB·2,000 key 한도에서 manifest·CAS 재시도·향후 메타데이터에 1 MiB·400 key를 남긴다. 256 KiB shard 16개면 container당 최소 4,096건을 수용한다.
- receipt container ID는 `r-<accountId>-<yyyymm>-<nnn>`이다. accountId가 UUID v4일 때 최대 49자로 Custom ID의 50자 제한 안에 들어간다. `nnn`은 `001`부터 시작하는 같은 월의 overflow serial이며, 계정 ledger의 receipt bucket directory가 활성 serial과 정리 대상을 보관한다.
- 각 receipt container는 `manifest` key와 shard key를 가진다. manifest는 shard별 byte/count/write 상태를 보관하고 CAS로 다음 shard 또는 overflow container를 연다. receipt는 active submission reservation이 가리키는 위치에 먼저 기록하고, 완료 전 재조회로 같은 ID/payload/terminal 결과를 확인한다.
- 중복 조회는 submission ID의 앞 세 hex 문자로 결정하는 locator container `l-<accountId>-<hhh>`의 `r-<submissionId>` key를 먼저 읽는다. locator는 receipt 위치와 terminal 시각을 가리킨다. 같은 ID 재전송은 전체 시간 버킷을 순회하지 않는다.
- receipt 작성, locator 작성, account terminal 확정은 하나의 전체 transaction이 아니다. account의 active submission reservation에 receipt 위치·단계를 기록하고, timeout/응답 유실 뒤 같은 ID가 이 단계부터 재개한다. locator는 receipt 삭제가 성공한 뒤에만 삭제한다.
- 만료 정리는 bucket directory가 가리키는 가장 오래된 shard 하나만 대상으로 한다. receipt 삭제 성공 뒤 locator를 삭제하며, 중간 실패는 active cleanup 단계로 남겨 같은 항목을 재개한다.

#### provider-neutral port와 UGS adapter

| Port 책임 | 필요한 결과 | 현재 UGS adapter 변경 |
| --- | --- | --- |
| Account/operation store | create-if-absent, CAS read/write, active reservation 복구 | 기존 account store 재사용, reservation에 receipt location/phase 추가 |
| Receipt store | manifest/shard/locator read·CAS write·조건부 delete | Private Custom key별 delete와 read/write adapter 추가 |
| Leaderboard store | top page, player range, player score, score write | player-range REST route와 metadata/rank DTO 추가 |
| 오류 모델 | `Conflict`, `UnknownCommit`, `Unavailable` | HTTP 409·timeout/응답 유실·5xx를 provider-neutral 결과로 변환 |

Leaderboard player-range의 공식 REST 경로는 `/v1/projects/{projectId}/leaderboards/{leaderboardId}/scores/players/{playerId}/range`이며 ServiceAccount bearer 인증과 `rangeLimit`, `includeMetadata`를 지원한다. TOP은 기존 offset/limit 조회를 10으로 제한하고, AROUND YOU는 `rangeLimit=3`으로 최대 7행을 얻는다. rank/metadata 누락 또는 응답 불일치는 Error로 처리한다.

### Step 1에서 추가로 대조할 기술값

- 삭제 API의 writeLock·409·응답 유실 처리와 현재 Module adapter의 구현·테스트 대역을 대조한다.
- Leaderboard player-range·rank·metadata·페이지/버전 응답을 설치 SDK와 OpenAPI에서 대조해 snapshot 검증에 쓸 필드를 확정하고, Module의 service token 호출과 같은 계약인지 verification 격리 검사로 확인한다.
- provider-neutral port의 오류 모델(Conflict, UnknownCommit, Unavailable)과 UGS adapter의 매핑을 확정한다.

### 기술값 확인 방법과 자동 검사

1. **공식 계약 확인:** Cloud Save 및 Leaderboard OpenAPI의 갱신일·제한·HTTP 상태·request/response field를 위 링크에서 확인한다. 문서 계약이 바뀌면 확인일·값·출처를 이 Step에 갱신하고, 구현 전에 영향 범위를 다시 판정한다.
2. **설치 Module SDK 확인:** `UGS/Modules/FlowStateVerification/FlowStateVerification.csproj`의 `Com.Unity.Services.CloudCode.Core`·`Com.Unity.Services.CloudCode.Apis` PackageReference와 IDE 정의 이동 또는 `dotnet list UGS/Modules/FlowStateVerification/FlowStateVerification.csproj package --include-transitive` 결과를 대조한다. 설치 SDK의 메서드/DTO가 OpenAPI와 다르면 adapter는 실제 설치 SDK 또는 명시적인 REST 계약 중 하나만 사용한다.
3. **현재 adapter 확인:** `UGS/Modules/FlowStateVerification/ServerServices.cs`와 `runtime.js`에서 Cloud Save/Leaderboard 호출 이름이 양쪽에 모두 있는지 확인한다. key별 delete와 player-centered range는 Phase 3 구현 전에는 PENDING이며, 구현 후 두 파일과 Test 대역에서 동시에 지원해야 한다.
4. **receipt 예산 산정:** 최대 길이의 receipt를 실제 직렬화 형식으로 만들어 byte 수를 측정한다. 예상 180일 제출 수와 2,000 key·5 MiB 총량을 대조해 custom ID당 byte/key 예산과 overflow container rollover 기준을 정한다. 경계 직전·동일·직후는 가짜 저장소 Test에서 자동 검증한다.
5. **원격 범위 확인:** 실제 service token의 player-range/metadata·version/snapshot 동작과 delete의 writeLock/409/응답 유실은 verification 전용 격리 자료로만 확인한다. 이 검사는 Step 9의 별도 승인·백업 뒤에 수행하며 자동 로컬 검사의 성공으로 대체하지 않는다.

로컬 읽기 전용 자동 검사는 다음과 같다.

```powershell
node UGS/Verification/check-phase3-technical-contracts.cjs
node UGS/Verification/check-phase3-technical-contracts.cjs --json
```

기본 실행은 공식 계약의 기록값, 설치 Module 패키지, 현 adapter capability를 보고한다. 현재 없는 delete/player-range는 `PENDING`으로 보고하지만 실패하지 않는다. Phase 3 adapter 구현 뒤에는 아래 엄격 검사를 실행해 두 호출이 빠지면 실패하게 한다.

```powershell
node UGS/Verification/check-phase3-technical-contracts.cjs --require-adapter
```

이 도구는 네트워크·Unity·원격 UGS를 실행하거나 변경하지 않는다. OpenAPI 최신성, 배포된 Module, service token 권한, 실제 서비스 한도와 snapshot 의미는 위 공식 계약 대조 및 Step 9 격리 검증으로 별도 확인한다.

사용자 작업: 현재 없음. 추가 권한·데이터 변경 승인이 필요해지면 정확한 대상과 이유를 제시한다.

완료 조건: [x] 구현 방식·변경 파일·기존 자료 보존·API 근거와 미확인 사항이 정리됐다. Step 1-1·1-2 완료. Unity/원격 서비스/Scene 수동 작업은 필요하지 않으며, 후속 Step 2·3 구현 및 Step 7~9 검증과 구분한다.

## Step 2. 새 계정의 첫 제출을 자동으로 준비한다

**목적:** Dashboard에서 ledger를 만들어 주지 않아도 첫 기록을 제출할 수 있게 한다.

AI 작업:

1. 기존 계정 준비 코드를 재사용해 첫 온라인 제출에 필요한 저장 자료를 준비한다.
2. 번호 조회가 선행되지 않은 첫 제출, 동시 첫 요청, 각 저장 경계의 실패·응답 유실 뒤 복구를 테스트한다.
3. 기존 계정과 이전 후 새 활성 기기의 기록·번호·소유 행을 보존한다.

### Step 2 수행 결과 — 완료 (2026-10-06)

- `submit-record` 실제 endpoint는 유효한 submission ID가 있는 요청에서만 account provisioning을 먼저 수행한 뒤 제출 서비스를 호출한다. 따라서 새 인증 계정은 Dashboard/수동 ledger 생성 없이 첫 제출을 시작한다.
- JSON이 깨졌거나 submission ID가 잘못된 요청은 provisioning 전에 기존 거부 응답으로 끝나므로 새 계정·ledger·번호를 만들지 않는다.
- 로컬 SDK 대역에서 새 계정 첫 제출, 동일·서로 다른 계정 동시 초기화, 각 저장 경계의 실패/응답 유실 복구, 기존 계정·legacy row·공개 번호·소유권 보존을 검증했다. Unity와 원격 UGS 동작은 Step 7~9에서 별도로 검증한다.

사용자 작업: 현재 없음. 실제 새 계정의 첫 제출은 Step 9-A에서 확인한다.

완료 조건: [x] 첫 제출·동시 초기화·부분 실패·기존 계정 보존 Test 통과. 수동 작업 없음. Unity Editor 빌드·Unity Test Runner·Scene 변경·원격 호출은 이 Step에서 수행하지 않는다.

## Step 3. 서버 제출 이력을 분할하고 오래된 receipt를 정리한다

**목적:** 129번째 이후에도 제출을 처리하고, 완료 이력을 영구 누적하지 않게 한다.

AI 작업:

1. terminal receipt를 계정별 시간 버킷·바이트 기반 다중 shard 저장으로 전환하고, 진행 중 요청·Board best와 분리한다.
2. 기존 entries를 전환하는 중단·재시도에서도 ID·payload·결과·서버 시각을 보존한다.
3. 180일 경계에서 만료 receipt의 저장 삭제 성공을 확인한 후 신규 제출처럼 처리한다. 삭제 실패·응답 미확인은 복구 경로로 처리한다.
4. 동시 같은 ID·payload 충돌·응답 유실·제출/정리/이전 경합에 CAS와 활성 연결 검사를 적용한다.
5. `GetAccountPersonalBests` 및 이전 복구가 새 저장 형식을 읽도록 연동한다. 이전 완료 receipt와 제출 terminal receipt를 구분한다.

### Step 3-1. receipt journal·shard·locator 구현

manifest/shard의 바이트·건수 예산, shard rollover, submission ID locator, CAS 재시도 안전성을 구현하고 대역 Test로 검증한다.

### Step 3-2. 제출·lazy migration 연결

v1 ledger와 현재 테스트 데이터를 최초 제출/조회에서 v2 journal로 lazy migration하고, 제출·재전송·개인 최고 기록·기기 이전이 새 상태를 읽게 한다.

**완료 — 2026-10-06.** `submit-record`는 v1 ledger를 receipt-first/ledger-CAS 순서로 v2 journal로 전환한 뒤 v2 pending·best·terminal receipt 경로를 사용한다. v2 snapshot, 계정 예약 해제와 이전 precheck도 v2 상태를 읽는다. endpoint 대역에서 v1→v2 전환 뒤 130개 terminal 제출을 처리했고, 기존 제출/이전·개인 최고 기록·embedded Module shim 회귀를 통과했다.

사용자 작업: 없음. Unity Editor 빌드·Unity Test Runner·Scene 변경·원격 UGS 호출은 Step 3-2에서 수행하지 않는다.

### Step 3-3. retention cleanup·경합 회귀

180일 opportunistic cleanup, 삭제 응답 유실 재개, 동일 ID 충돌, 제출/정리/이전 경합과 129건 이상 회귀 Test를 구현한다.

**완료 — 2026-10-06.** 제출 요청은 receipt directory의 가장 오래된 버킷에서 shard 하나만 정리한다. shard/manifest CAS가 성공한 뒤 locator를 조건부 삭제하고, 응답 유실 뒤에는 manifest의 `expiredIds` cleanup 상태로 locator 정리를 재개한다. 로컬 대역에서 180일 만료·locator 삭제 응답 유실·cleanup/append CAS 경합 재시도·payload 충돌·v2 pending의 이전 차단과 terminal 해제를 검증했다.

사용자 작업: 없음. Unity Editor 빌드·Unity Test Runner·Scene 변경·원격 UGS 호출은 Step 3-3에서 수행하지 않는다.

사용자 작업: 현재 없음. 128건 초과와 시간 경계는 Step 6의 자동 Test로 판정한다.

완료 조건: [x] 128건 초과·정리·migration·복구·기기 이전 회귀 Test 통과. Step 3-1·3-2·3-3 완료. Unity·게시·실제 서비스 검증은 Step 7~9에서 별도로 수행한다.

## Step 4. Local Pending의 생성 시각과 만료를 구현한다

**목적:** 오래된 대기 기록을 명확히 정리하고 정상 Pending의 재시도 정보를 유지한다.

AI 작업:

1. 후보 생성 때 생성 시각을 저장하고 재시도·재시작에서 동일 시각·제출 ID·계정 귀속을 유지한다.
2. 생성 시각 없는 이전 Local Save를 전환한다. 다시 로드해도 시작 시각이 바뀌지 않도록 한다.
3. 180일 경계에서 `SubmissionExpired`를 표시하고 해당 항목만 저장 제거한다. 제거 저장 실패를 검증한다.
4. 다른 계정·다른 환경의 Pending과 기기 공용 Settings·입력 설정·Tutorial을 보존한다.
5. 만료 안내는 영어 문구로 작성하고 표시/선택 상태를 Test로 검증한다.

### Step 4-1. Pending 생성 시각·v7 Local Save migration — 코드 완료

Pending 후보에 `createdAtMilliseconds`를 저장하고 Local Save 형식을 v7로 올렸다. v6 이하의 생성 시각 없는 Pending은 최초 deserialize에서 한 번만 UTC 시각을 부여하며, 다음 checkpoint로 저장한다. Settings·Tutorial·inactive online scope는 기존 LocalSaveData 경로로 보존한다.

### Step 4-2. 만료 저장·재시도 경로 — 코드 완료

LocalRecordRepository는 180일을 넘은 Pending만 save-first 방식으로 제거한다. 저장 실패면 메모리와 파일을 유지한다. OnlineRecordCoordinator는 retry 전에 만료 정리를 수행하고, 정리된 경우 영어 `SubmissionExpired` 진단을 남긴다.

### Step 4-3. Unity 자동 검증·수동 확인 준비 — 대기

EditMode에서 migration 시각 고정, 정확한 180일 경계, 저장 실패, 재시작 및 scope 격리를 검증하고, Step 9-C의 복제 저장+가짜 시계 수동 확인을 준비한다. AI는 Unity Test Runner·Editor 빌드·Scene을 실행하거나 수정하지 않는다.

**완료 — 2026-10-06.** 사용자가 Unity Script Compilation 성공, 예상 밖 Error/Warning 없음, EditMode 889/889 성공 및 예상 밖 Test Error/Warning 없음을 확인했다. 실제 복제 저장+가짜 시계의 화면 확인은 Step 9-C에서 수행한다.

사용자 작업: 현재 없음. 만료 화면은 Step 6에서 준비하는 **복제 저장 + 가짜 시계의 로컬 전용 검증**으로 Step 9-C에서 확인한다.

완료 조건: [x] Local Save migration·만료·저장 실패·재시작·환경/계정 격리 Test 통과. Step 4-1·4-2·4-3 완료. Scene 작업 없음. Step 9-C의 실제 화면 확인은 별도다.

## Step 5. 대규모 보드에서 TOP과 AROUND YOU를 조회한다

**목적:** 100명 초과 보드와 페이지 경계에서도 정확한 목록·순위를 표시한다.

AI 작업:

1. 상위 10·주변 7을 반환하도록 실제 조회 경로와 Module adapter를 구현한다. 검증용 `me` 조회·개인 최고 조회와의 호환성을 유지한다.
2. Stage 오름차순·Infinite 내림차순에서 전역 competition rank를 계산한다. API rank를 사용한다면 계약과 일치함을 증명한다.
3. 페이지를 가로지르는 동점과 acceptedAt 동률에서 공개 번호 순서를 보존한다. 부분 페이지의 첫 행을 무조건 1위로 처리하지 않는다.
4. 읽는 동안 행이 변경되거나 페이지 누락·중복·번호 연결 실패가 있으면 불완전 결과를 성공/Empty로 표시하지 않도록 한다.
5. TOP/AROUND YOU 독립 상태, 명시적 갱신, 안전한 오류·내 행 표시·계정 이전 회귀를 유지한다.

### Step 5-1. API·Module adapter — 완료 (2026-10-07)

TOP은 offset 0/limit 10, AROUND YOU는 player-centered rangeLimit 3(최대 7), ME는 player score API를 사용하도록 `query-records`를 전환했다. 100건 전체 읽기와 `VerificationBoardCapacity` 제한을 제거했다.

### Step 5-2. 전역 rank·일관성 — 완료 (2026-10-07)

서비스 응답의 rank를 필수로 검증하고, 로컬에서 전체 목록을 정렬해 전역 rank를 재계산하지 않는다. rank/metadata/행 식별자 누락, AROUND YOU에서 본인 행 누락, 매핑·권한·서비스 오류는 부분 Success가 아닌 Error로 처리한다.

### Step 5-3. Client 요청·로컬 검증 — 완료 (2026-10-07)

Client 기본 요청을 TOP 10·AROUND YOU 7로 맞췄다. 로컬 대역에서 AROUND YOU range API 경로와 global rank 보존, 기존 제출/이전/embedded Module shim 회귀를 검증했다. Unity/실제 서비스/UI 확인은 Step 7~9에서 별도로 수행한다.

사용자 작업: 현재 없음. 순위 계산과 페이지 경계는 자동 Test로 판정한다.

완료 조건: [x] 100명 초과 제한 제거·동점 rank 보존·내 주변 range·부분 실패 Error·Client 요청 제한 Test 통과. Step 5-1·5-2·5-3 완료. 수동 작업·Scene 작업 없음.

## Step 6. 자동 Test와 수동 확인 준비를 끝낸다

**목적:** 사람의 반복 플레이·정밀 클릭·장기간 대기 없이 경계를 검증한다.

AI는 아래 사례를 기존 Test에 보강한다. 독립된 새 책임만 별도 fixture로 나누고 단순 중복 Test를 추가하지 않는다.

| 검증 대상 | 자동 검증할 사례 | 실행 수단 |
| --- | --- | --- |
| 신규 계정 준비 | 첫 직접 제출, 동시 준비, 저장 전/후 실패·복구 | 서버 대역 Unit Test |
| 제출 수량 | 127·128·129·256건, 여러 분할 구간 | 서버 대역 Unit Test |
| 중복 판정 | 같은 ID/payload, 다른 payload, 응답 유실 뒤 동일 ID | 서버 대역 + EditMode |
| receipt 만료 | 180일 직전·동일·직후, 삭제 실패/응답 유실·동시 정리 | 가짜 시계·저장소 대역 |
| 기존 자료 전환 | 일부 전환 후 중단/재시도, best/metadata 유지 | 서버 대역 Unit Test |
| Local Pending | 생성 시각 보존, 이전 version 전환 1회, 만료 저장 실패 | EditMode |
| 격리 | 계정/환경 혼합 방지, 만료로 Settings/입력 설정 유지 | EditMode + 기존 PlayMode |
| 조회 수량 | 0·1·7·10·99·100·101·201행, 서비스 페이지 크기 경계 | 서버 대역 Unit Test |
| 전역 순위 | 1,1,3, 페이지 경계 동점, acceptedAt 동률·공개 번호 정렬 | 서버 대역 Unit Test |
| 내 주변 | 첫 행·중간·마지막·내 행 없음, 최대 7행 | 서버 대역 + EditMode |
| 조회 실패 | 페이지 누락/중복/변동·번호 연결 실패·Timeout·불완전 성공 차단 | 서버 대역 Unit Test |
| UI 연동 | TOP/AROUND YOU 독립 상태·영어 안내·명시적 갱신·포커스 | EditMode + PlayMode |
| 기기 이전 | 분할 receipt/best 유지·이전과 제출/정리 경합·옛 기기 거부 | 기존 이전 회귀 확장 |

추가 AI 준비:

1. 소스 문법·중복 C# 멤버·asmdef/meta·저장 version·고정 Project/Environment·9함수 연결·비밀값 노출·문서 링크를 정적으로 검사한다.
2. 실제 서버 원본/Module shim을 사용하는 로컬 Node Test를 실행한다. 대역 결과와 실제 .NET/서비스 결과를 구분해 기록한다.
3. Unit Test는 가짜 시간·가짜 서비스·메모리/임시 저장을 사용한다. 실제 인증·원격 게시와 자동 Test를 분리한다.
4. 수동 확인용 새 계정 프로필 `flow-state-phase3-new`와 기존 계정 회귀 경로를 격리 도구에 준비한다. 새 프로필은 최초 번호 조회보다 **첫 직접 제출**을 먼저 시험할 수 있게 한다.
5. 실제 사용 가능한 버튼 이름·저장 전체 경로·안전한 PASS/FAIL 메시지를 이 문서 Step 9에 갱신한다. **현재 Phase 3 전용 프로필/버튼/만료 미리보기는 아직 준비 전이다.** 사용자에게 없는 버튼을 찾게 하지 않는다.
6. Scene 참조 변경이 필요한 경우 사용자용 별도 하위 Step에 Hierarchy 전체 경로·컴포넌트·참조 슬롯·영어 Text·RectTransform·색·Navigation 값을 지정한다. 기존 Scene을 읽기 전용 대조하고 실제 수정은 사용자에게 맡긴다.

### Step 6-1. 경계 자동 Test와 정적 검사 — 완료 (2026-10-07)

- 실제 endpoint/Module shim을 사용하는 로컬 대역 Test에서 신규 계정 첫 제출, 127·128·129·256 terminal 제출, 0·1·7·10·99·100·101·201행의 서비스 total과 TOP 10 고정 요청, range API·전역 rank·이전/복구 경계를 검증했다. 이는 메모리 SDK double 결과이며 실제 UGS 호출이나 성능 측정이 아니다.
- `check-phase3-technical-contracts.cjs --require-adapter`, Local Save/verification-tool 계약 검사, Unity source preflight를 통과했다. preflight는 변경 C# 멤버 중복·괄호·meta GUID·asmdef·격리 경계를 검사하며 Unity 컴파일/Test Runner를 실행하지 않는다.

### Step 6-2. 새 계정 격리 실행 경로 — 완료 (2026-10-07)

- `E_VerificationSession.Phase3New`를 추가했다. 인증 Profile은 `flow-state-phase3-new`, Local Save는 기존 A/B/Legacy와 분리된 `<persistentDataPath>/Prototype8Verification/Phase3New/flow-state-save.json`이다.
- 새 EditMode 사례는 profile·저장 경로·명시 launch argument `--fs-verification-session=Phase3New`의 분리를 검증하도록 작성했다. 실행은 Step 7의 Unity Test Runner에서 사용자가 한다.

### Step 6-3. Step 9 실행 절차 확정 — 완료 (2026-10-07)

1. **Step 7~8을 통과한 뒤에만** Unity에서 `Flow State > Online Record Verification`을 연다. `Prototype 8 Phase 2 (Scene 없음)` 토글을 유지하고, Play 전 `격리 세션`에 **Phase3New**를 고른다.
2. **Play 전 격리 실행 예약 (원격 호출 없음)** → 상단 Play → `verification 원격 요청·테스트 기록 변경 허용` → **격리 세션 준비 (로컬만)** 순서로 실행한다. 표시된 Profile이 `flow-state-phase3-new`이고 Local Save 경로가 `.../Prototype8Verification/Phase3New/flow-state-save.json`인지 확인한다. 이 경로는 민감한 로컬 정보일 수 있으므로 채팅에 복사하지 않는다.
3. **계정 패널 열기 / 상태 확인**으로 동의·인증·공개 번호를 준비한 뒤, **Step 10-1 신규 Stage 제출·서버 행 확인 (60초)**을 정확히 한 번 실행한다. 이 도구는 빈 내 행을 먼저 읽기 전용으로 확인한 다음 유효 후보 하나만 생산 제출 경로로 전송한다. `PASS / SERVER_SUBMITTED_AND_ME_VERIFIED`가 기대 결과다. 이미 행이 있으면 새 프로필을 재사용하거나 초기화하지 말고 중단해 보고한다.
4. 기존 계정은 A/Legacy 중 실제 보존된 세션을 선택해 `내 최고 기준 저장 / 재조회 번호 비교`와 `기준 자료와 번호·점수·수락 시각 비교`를 사용한다. Offline Pending은 일반 게임에서 만들고 **기존 Pending 1건 재전송 (새 기록 생성 없음)**으로 정확히 한 건만 재전송한다.
5. 만료 안내의 화면 가독성은 아직 전용 fake-clock UI를 만들지 않는다. Step 4의 EditMode 경계·저장 실패 Test가 정확성을 판정하며, Step 9-C에서는 실제 Offline 복구와 `SubmissionExpired` 진단이 생긴 경우에만 화면을 관찰한다. Scene 변경은 필요 없고, 이 Step에서 Scene을 수정하지 않는다.

사용자 작업: Step 6 자체에는 없음. 위 절차는 Step 7~9에서 사용한다. Unity Editor 빌드·Unity Test Runner·Scene 변경·원격 UGS 호출은 AI가 수행하지 않았다.

완료 조건: [x] 로컬 서버 Test·정적 검사 통과, Unity Test 작성 완료, Step 9 실제 실행 경로 확정. Unity 실행·게시·실제 서비스/UI 판정은 Step 7~9에 남는다.

## Step 7. Unity 컴파일과 전체 Test를 실행한다 — 사용자

**목적:** 실제 Unity에서 변경 코드와 기존 게임/UI가 함께 동작하는지 확인한다.

### Step 7-1. Unity 실행 전 정적 준비 — 완료 (2026-10-07)

- `client-unity-preflight.cjs`가 변경/신규 C# 10개, 중복 멤버·구문 구분자·Assets meta GUID·asmdef·격리 경계를 확인했다. Phase 3 관련으로 준비된 사례는 EditMode 183개, PlayMode 23개다.
- verification 도구 source 계약도 통과했다. 이 결과는 C# 컴파일이나 Unity Test Runner 실행 결과가 아니며, Scene을 읽거나 수정하지 않는다.

### Step 7-2. Unity 컴파일·전체 Test 실행 — 완료 (2026-10-07)

1. Unity에서 프로젝트를 열고 Script Compilation이 끝날 때까지 기다린다.
2. Console에서 새 Error/Warning을 확인한다. 오류가 있으면 메시지 전체를 전달하고, 오류를 해결하기 전에는 Test를 실행하지 않는다.
3. **Window → General → Test Runner → EditMode → Run All**을 실행한다.
4. 이어서 **PlayMode → Run All**을 실행한다.
5. Scene 변경은 필요 없다. Test가 Scene 관련 오류를 낼 경우에만 Play를 종료한 뒤 오류 메시지와 대상 Scene/개체 경로를 전달한다. AI는 Scene을 수정하지 않고 적용 방법만 제공한다.
6. 아래 형식으로 실제 화면에 표시된 결과를 알려 준다.

```text
Script Compilation: 성공 / 실패
예상 밖 컴파일 Error·Warning: 없음 / 메시지
EditMode: 성공 수 / 전체 수
PlayMode: 성공 수 / 전체 수
예상 밖 테스트 Error·Warning: 없음 / 메시지
```

**완료 — 2026-10-07.** 사용자가 Unity Script Compilation 성공과 예상 밖 Error/Warning 없음을 확인했고, EditMode 890/890 및 PlayMode 254/254 성공과 예상 밖 Test Error/Warning 없음을 보고했다. Step 7-1의 EditMode 183·PlayMode 23은 Phase 3 관련 소스 사례를 정적으로 집계한 수이며, 실제 전체 Test Runner 수량과 다르다.

완료 조건: [x] Step 7-1·7-2 완료. 최신 변경 컴파일·전체 Edit/PlayMode 성공 및 예상 밖 Error/Warning 없음이 사용자 보고로 확인됐다.

### Step 7-3. Phase3New 인증 허용 목록 회귀 — 완료 (2026-10-07)

Step 9-1에서 발견한 `UnsupportedProfile`을 수정해 `flow-state-phase3-new`을 fail-closed 허용 목록에 추가했다. Unity Editor에서 Script Compilation 완료·예상 밖 Error/Warning 없음 확인 후, EditMode와 PlayMode **Run All**을 다시 실행한다. Scene 변경·Module 빌드·배포는 필요 없고 AI는 Unity 컴파일/Test Runner를 실행하지 않는다.

**완료 — 2026-10-07.** 사용자가 최신 Client 인증 변경 뒤 Unity Script Compilation 성공·예상 밖 Error/Warning 없음, EditMode 890/890 성공·예상 밖 Test Error/Warning 없음, PlayMode 254/254 성공·예상 밖 Test Error/Warning 없음을 확인했다.

완료 조건: [x] 최신 Client 인증 변경의 Script Compilation 및 전체 Edit/PlayMode 성공, 예상 밖 Error/Warning 없음이 사용자 보고로 확인됐다.

## Step 8. 백업하고 verification Module을 게시한다 — 사용자

**목적:** 새 저장·조회 코드를 실제 verification 서비스에서 사용한다.

### Step 8-1. 게시 대상·정적 계약 확인 — 완료 (2026-10-07)

- 게시 manifest는 `Assets/CloudCode/FlowStateVerification.ccmr`의 `FlowStateVerification.sln` 하나를 가리킨다. 현재 Module shim의 9개 endpoint 계약과 `submit-record`/`query-records`의 `request: String, Required` 계약을 로컬에서 검사했다.
- Phase 3 adapter 기술 계약과 Module shim 대역 Test를 통과했다. 이 검증은 Module 빌드·Dashboard 대상 확인·원격 소스 백업·게시·실제 호출을 수행하거나 증명하지 않는다.

### Step 8-2. 게시 전 백업 — 사용자 대기

1. Play를 종료한다. 테스트 앱을 닫는다.
2. `C:\Unity\Unity_Flow_State\Ignore\StepPhase3\Before` 폴더를 만든다.
3. 현재 활성 `FlowStateVerification` Module의 **실행 가능한 source/산출물**과 활성 버전 번호·게시 시각을 `Before\published-module-before`에 보관한다. Dashboard가 source 다운로드를 제공하지 않으면, 게시에 사용했던 정확한 source/산출물 사본과 활성 버전 화면 캡처를 함께 보관한다. endpoint 목록이나 API 스펙만 저장한 것은 실행 코드 백업이 아니다.
4. 기존 검증 세션 A/Legacy 각각에 대해 `Flow State > Online Record Verification` 창의 **격리 세션 준비 (로컬만)** 후 표시되는 실제 `Local Save` 경로를 기록하고, 해당 `flow-state-save.json`을 `Before\local-save`에 원본 이름을 보존해 복사한다. 준비한 세션을 바꾸려면 Editor를 종료·재시작한다. 새 `Phase3New` 프로필은 아직 생성하거나 백업하지 않는다.
5. Dashboard에서 기존 검증 계정의 account/ledger 및 Stage·Infinite 최고 행을 읽어 `Before\service-baseline`에 보관한다. 최소 항목은 기록 시각, 대상 환경, 활성 Module 버전, 각 board의 공개 행(score/acceptedAt/public number)이며, Player ID·account ID·token·receipt payload·Secret은 보관 파일이나 채팅에 넣지 않는다.
6. 백업이 하나라도 불완전하면 게시하지 않는다. 기존 ledger/board/계정/정책/Secret을 삭제·초기화·재생성하지 않는다.

### Step 8-3. verification Module 게시 — 완료 (2026-10-07)

1. Unity Dashboard에서 프로젝트 **Unity_Flow_State**, 환경 **verification**을 선택하고 Project/Environment ID를 아래 값과 비교한다.

```text
Project ID: c76d55cf-7846-494b-9dce-a0797b179b36
Environment ID: a20a46fa-1edb-4d79-9c35-02f2fed31896
```

2. Unity Editor에서 `FlowStateVerification` Module을 빌드한다. AI는 이 빌드를 실행하지 않는다.
3. **Window → Services → Deployment**를 열고 대상 환경이 **verification**인지 다시 확인한다.
4. **FlowStateVerification** Module 하나만 선택하고 **Deploy Selected**를 실행한다. **Deploy All**, 구 JavaScript Script 게시, Board/Cloud Save/Access Control 초기화는 수행하지 않는다.
5. 성공 여부·활성 버전·게시 시각 및 대상 Project/Environment 일치 여부만 알려 준다. Secret·token·내부 ID·전체 로그는 공유하지 않는다.

저장 형식 변경은 Step 1에서 확정한 전환 방식으로 적용한다. 코드와 데이터 형식이 호환되지 않는 경우의 복구 절차를 게시 전에 확정한다. 이전 Module 재게시만으로 새 저장 자료와 호환된다고 가정하지 않는다.

사용자 작업: Step 8-2와 8-3을 수행해야 한다. Scene 변경과 Unity Test Runner 실행은 필요 없다.

### Step 8-2. 게시 전 백업 — 완료 (2026-10-07)

- 사용자는 Dashboard C# Module에서 source/실행 산출물을 다운로드할 수 없고 API Spec만 다운로드할 수 있음을 확인했다. Unity 공식 문서도 Dashboard C# Module은 preview와 OpenAPI 다운로드만 지원한다고 설명한다. 따라서 API Spec은 계약 자료로만 `Before`에 보관할 수 있으며, 이전 실행 Module을 재게시할 수 있는 백업이 아니다.
- 기존 A Local Save는 보관했고 Legacy Local Save는 존재하지 않는다. 기존 verification 계정도 없으므로 기존 account/ledger 및 Stage/Infinite 행 기준 자료는 확보할 대상이 없다. 이 부재를 `Before\service-baseline`에 기록한다.
- Git `b2953d9ff0d6685898477e3ae2b1f20d11e131f0`에서 `.ccmr`, Module source와 embedded `UGS/CloudCode` 전체 44파일을 `Before\FlowStateVerification-pre-phase3-b2953d9.zip`으로 archive했다. SHA-256과 분리 workspace 복구 방법은 같은 이름의 `.manifest.txt`에 기록했다.
- 이 archive는 Phase 3 이전 Git rollback 후보이며 현재 원격 활성 Module과 정확히 같다는 증명은 아니다. Dashboard C# Module은 source/revision-to-source 대응을 제공하지 않아 사용자도 이 후보와의 정확한 일치를 확인할 수 없다. 사용자는 이 한계를 인지하고 현재 archive를 verification 게시의 최선의 복구 자료로 수용했다. API Spec/last uploaded date만으로 일치를 주장하지 않는다.

다음 작업은 위 Step 8-3의 빌드·Deploy Selected다. 성공 여부·활성 버전·게시 시각·Project/Environment 일치 여부만 보고한다. AI는 Unity Module 빌드·게시·원격 UGS 호출을 수행하지 않는다.

**완료 — 2026-10-07.** 사용자가 `FlowStateVerification` Module 빌드와 Deploy Selected 성공, 게시 시각 **Oct 7, 2026, 3:01 PM**을 보고했고, 대상이 Project `Unity_Flow_State` / Environment `verification`임을 확인했다. Dashboard에는 별도 version 정보가 표시되지 않았다. Git `b2953d9` rollback 후보는 원격 source와의 동일성을 증명하지 않는 최선의 복구 자료라는 한계를 유지한다.

### Step 8-4. Leaderboard query rank 보정 재게시 — 부분 완료 (2026-10-07)

Step 9-3의 실제 TOP 조회는 `Unavailable`, AROUND YOU는 `Service unavailable`로 실패했다. Client의 20건 기본값은 TOP 10/AROUND 7로 고정했으나, 서비스가 반환하는 0-based rank를 Module이 metadata 오류로 거부하는 문제가 남아 있었다. Module은 이제 0-based service rank를 1-based 공개 competition rank로 정규화한다. AROUND YOU 실패는 허용된 phase/HTTP 상태만 화면에 표시하도록 Client 진단을 보강했다.

1. Play를 종료한다.
2. Unity Editor에서 `FlowStateVerification` Module을 다시 빌드한다. AI는 빌드를 실행하지 않는다.
3. Deployment 창에서 Project `Unity_Flow_State`, Environment `verification`을 다시 확인하고 **FlowStateVerification 하나만** 선택해 **Deploy Selected**를 실행한다. Board·Cloud Save·Access Control·Secret은 변경하지 않는다.
4. Module 빌드/배포 성공, 대상 Project/Environment, 게시 시각만 보고한다. Dashboard가 version을 표시하지 않으면 `version 없음`으로 기록한다.

이 재게시에는 Scene 변경이나 새 계정/새 기록/Pending 폐기가 필요 없다. Dashboard source 다운로드 불가라는 기존 한계와 Git `b2953d9` rollback 후보의 한계는 그대로다.

**결과:** 사용자가 Module 빌드·배포 및 최신 Client 컴파일을 완료했다. Stage/Infinite TOP은 공개 one-based rank로 정상 표시됐다. AROUND YOU는 안전 진단 `Read leaderboard, HTTP 503`으로 실패했으며, 그 결과 C# Module adapter가 JavaScript SDK의 options 객체를 정수로 캐스팅하는 결함을 확인했다. 다음 Step 8-5에서 이 adapter만 재게시한다.

### Step 8-5. Leaderboard player-range adapter 보정 재게시 — 완료 (2026-10-07)

`getLeaderboardPlayerRange`의 JavaScript 호출 네 번째 인수는 `{ params: { rangeLimit: 3, includeMetadata: true } }`다. C# Module adapter는 이를 숫자로 잘못 캐스팅해 내부 예외를 503으로 표시했다. 이제 `params.rangeLimit`을 정수로 검증해 읽고, 0~3 범위만 허용한다. Client·Scene·저장 자료는 변경하지 않는다.

1. Play를 종료한다.
2. Unity Editor에서 `FlowStateVerification` Module을 빌드한다.
3. Deployment에서 Project `Unity_Flow_State`, Environment `verification`과 Module 하나를 확인한 뒤 **Deploy Selected**를 실행한다.
4. 성공 후 인터넷 연결 상태의 일반 게임에서 Leaderboard를 열고 Stage와 Infinite 각각의 TOP·AROUND YOU·Retry를 한 번 확인한다. 새 기록 제출, Pending 폐기, 계정 이전, Scene 변경은 하지 않는다.

이 변경은 C# Module-only이므로 최신 Client Unity Script Compilation 및 전체 EditMode/PlayMode 성공 근거를 대체하거나 무효화하지 않는다. 다만 Module 빌드·배포와 실제 AROUND YOU 결과는 별도로 필요하다.

**완료 — 2026-10-07.** 사용자 재게시 뒤 Stage TOP/AROUND YOU와 Infinite TOP/AROUND YOU가 모두 성공적으로 표시됐고, `Retry` 한 번도 같은 정상 결과로 재조회됐다. Stage는 본인 포함 4행, Infinite는 본인 1행으로 현재 존재 행만 표시했고, 공개 번호·점수 외 식별 정보는 노출되지 않았다.

완료 조건: [x] Step 8-1·8-2·8-3 완료. 이전 실행 코드/검증 자료의 가능한 범위 백업, 대상 일치 및 Module 게시 성공을 사용자 보고로 확인했다. 실제 endpoint/UI 결과는 Step 9에서 확인한다.

## Step 9. 실제 제출·조회와 화면을 확인한다 — 사용자

**진행 조건:** Step 6의 도구·버튼·경로 확정과 Step 7~8 성공 후 실행한다. Step 6-3의 절차와 정확한 버튼 이름을 사용한다.

### Step 9-1. 새 계정 첫 제출·재시작 보존 — 완료 (2026-10-07)

**목적:** 수동 ledger 생성 없이 신규 계정이 실제 서비스를 이용하는지 확인한다.

**인증 수정 — 2026-10-07.** 최초 `ConfirmConsent`는 `AuthFailure=UnsupportedProfile`로 실패했다. `Phase3New` 세션 설정만 추가하고 `UgsOnlineAuthenticationGateway`의 fail-closed 허용 Profile 목록에는 `flow-state-phase3-new`을 추가하지 않은 Client 누락이었다. 허용 목록과 정적 계약을 수정했으며, 이 시도는 SDK 초기화 전 실패해 Module 호출·새 계정·ledger·제출을 만들지 않았다. Module 재게시나 Scene 변경은 필요 없지만 C# 변경 뒤의 Unity 컴파일·전체 Test Runner 회귀가 필요하다.

**상태 조회 timeout — 2026-10-07.** 수정 뒤 `GetAccountTransferStatus`는 인증 완료(`AuthPhase=Complete`, 388ms) 후 ClientTimeout 5,052ms로 종료됐다. server elapsed/service-call 정보가 없으므로 endpoint가 계정을 생성·변경했는지 판단하지 않으며, 제출·프로필 초기화·재인증·자동 재시도를 하지 않는다. 같은 열린 계정 패널에서 **RefreshStatus**를 정확히 한 번 명시적으로 실행한다. `Ready`/공개 번호가 확인되면 다음 단계로 진행하고, 다시 Timeout/오류면 결과만 보고하고 중단한다.

**제출 검증 전체 제한 수정 — 2026-10-07.** 명시적 상태 새로고침 뒤 `Ready` 및 공개 번호 조회가 성공했다. 그러나 **Step 10-1 신규 Stage 제출·서버 행 확인 (60초)**은 사전 조회·최대 3회 제출 시도(1초/2초 bounded backoff 포함)·사후 조회를 수행하면서도 창 전체 제한이 5초여서, 마지막 상태 조회는 360ms/서버 160ms로 성공했는데도 전체 절차가 `WINDOW_TIMEOUT`으로 끝났다. 이는 원격 제출 성공/실패를 판정할 근거가 아니므로 같은 버튼을 재실행하지 않는다. Client 검증 도구를 수정하여 각 SDK 요청의 5초 상한은 유지하고, 제출 probe 2/5의 전체 제한만 표기와 같은 60초로 제한했다. Module 재게시·Scene 변경은 필요 없으며, 최신 Client 변경의 Unity 컴파일과 전체 EditMode/PlayMode 회귀 성공 뒤 새 Play 격리 세션에서 다시 진행한다.

**실제 제출·재시작 결과 — 2026-10-07.** 새 `flow-state-phase3-new` 격리 Local Save에서 계정이 `Ready`가 되고 공개 번호 조회가 성공했다. 최초 신규 제출 시도 뒤의 응답은 위 전체 제한으로 유실됐으나, 재시작한 같은 격리 세션의 읽기 전용 Stage 조회가 `Success / RowCount=1`을 확인했다. `Pending=0`이므로 새 후보 또는 재시도를 만들지 않았고, 이어 로컬 기준 자료와 재시작 뒤 조회의 공개 번호·점수·수락 시각이 `PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED`로 일치했다. 따라서 실제 신규 계정의 서버 행 및 재시작 보존은 확인했다. 유실 응답을 `SERVER_SUBMITTED_AND_ME_VERIFIED`로 소급 표기하지 않으며, 60초 도구의 최신 Client 변경에 대한 Unity 컴파일·전체 EditMode/PlayMode 회귀가 남아 있다.

1. 먼저 Step 7-3의 Unity 회귀를 완료한다. 그 뒤 Unity에서 `Flow State > Online Record Verification`을 열고, `Prototype 8 Phase 2 (Scene 없음)`을 유지한 채 `격리 세션`을 **Phase3New**로 고른다.
2. **Play 전 격리 실행 예약 (원격 호출 없음)** → 상단 Play → `verification 원격 요청·테스트 기록 변경 허용` → **격리 세션 준비 (로컬만)** 순서로 실행한다. 표시된 Profile이 `flow-state-phase3-new`, Local Save가 `Prototype8Verification/Phase3New/flow-state-save.json`인지 확인한다.
3. **계정 패널 열기 / 상태 확인** 후, `ConfirmConsent`가 표시·활성화된 경우에만 실행해 복구 안내 동의·인증·공개 번호 준비를 완료한다. 이미 `Consent=True`이고 `Ready`/공개 번호가 표시되어 버튼이 비활성화된 경우는 동의가 저장된 정상 상태이므로 누르지 않는다.
4. 이전 5초 종료의 제출 도달 여부를 먼저 구분한다. **Stage 제출 후 조회만 확인 (기록 제출 없음)**을 정확히 한 번 실행한다. `PASS / STAGE_ME_EMPTY`일 때만 다음 새 제출 단계로 간다. 기존 행·Pending·오류가 보이면 **Step 10-1**을 누르지 말고 결과를 보고한다.
5. **Step 10-1 신규 Stage 제출·서버 행 확인 (60초)**을 정확히 한 번 실행한다. 이 도구는 빈 내 행을 읽기 전용으로 확인한 뒤 유효 후보 하나만 제출한다. 기대 결과는 `PASS / SERVER_SUBMITTED_AND_ME_VERIFIED`다. `STAGE_ME_EMPTY`가 아니거나 실패하면 재제출·프로필 초기화 없이 결과를 보고한다.
6. **내 최고 기준 저장 / 재조회 번호 비교**로 `PASS / NUMBER_STABLE_AND_BASELINE_CAPTURED`를 확인하고, 필요하면 **기준 자료를 사용자 파일로 저장**으로 로컬 기준 파일만 보관한다. 공개 번호·Player ID·기준 JSON·token은 채팅에 붙이지 않는다.
7. Play를 종료한 뒤 같은 순서로 Phase3New 세션을 다시 준비하고, 저장한 기준 JSON으로 **기준 자료와 번호·점수·수락 시각 비교**를 실행한다. 기대 결과는 `PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED`다.

완료 조건: [x] 새 계정 첫 제출 시도 뒤 서버 행 1건·재시작 공개 번호/점수/수락 시각 보존, 최신 Client Unity 회귀 성공을 확인했다. 최초 제출 응답 유실은 중복 제출하지 않고 읽기 전용 서버 행·보존 비교로 판정했으며, `SERVER_SUBMITTED_AND_ME_VERIFIED` 응답을 소급 주장하지 않는다.

### Step 9-2. 기존 계정 자료 보존 — N/A (2026-10-07)

**판정:** 기존 verification 계정과 Legacy Local Save가 존재하지 않아 migration 전/후의 최고 기록 보존을 실제로 비교할 대상이 없다. 기존 계정을 새로 만들거나 데이터를 합성해 이 항목을 PASS로 만들지 않는다. Step 9-1에서 생성하는 새 계정은 신규 제출 검증용이며 기존 자료 보존의 대체 근거가 아니다.

### Step 9-3. Offline Pending 복구와 조회/UI — 완료 (2026-10-07)

**Offline 복구 완료·Leaderboard 호출 수정 — 2026-10-07.** 사용자가 오프라인 Stage clear에서 `Submission: Pending`을 확인하고, Play 재시작 뒤 Account 화면에서 Pending 보존을 확인했다. 명시적 `Retry Pending Uploads`는 `Pending records processed`로 종료되어 Pending을 제거했다. 이는 동일 저장 후보의 실제 재전송·제거 근거다. 이어 Stage/Infinite의 TOP과 AROUND YOU가 모두 `Invalid query`로 표시됐다. 원인은 Client `IOnlineRecordRepository`가 TOP/AROUND의 선택적 기본 인수를 이전값 20으로 계속 선언한 데 있었다. 인터페이스 기본값을 서버 계약과 같은 TOP 10/AROUND 7로 바꾸고 GameSystem 호출에도 10/7을 명시했다. 최신 Client의 Unity 컴파일·EditMode 890/890·PlayMode 254/254 성공도 확인했다.

**rank 보정·안전 진단 재게시 대기 — 2026-10-07.** 제한 보정 뒤 실제 화면은 TOP `Unavailable`, AROUND YOU `Service unavailable`을 표시했다. TOP은 Module이 service rank 0을 metadata 오류로 판정한 경로를 1-based 공개 rank로 보정해야 하며, AROUND YOU는 실제 HTTP 상태를 추측하지 않도록 `Read leaderboard`와 allow-list HTTP 상태만 표시한다. Module 대역 통합 Test 34/34 및 Client 정적 검사 통과는 실제 verification 호출의 대체 근거가 아니다. Step 8-4의 Module 재게시와 최신 Client 컴파일/전체 Test 뒤 Stage·Infinite를 재확인한다. 신규 제출·Pending 폐기·Scene 변경은 금지한다.

**player-range adapter 보정 대기 — 2026-10-07.** Step 8-4 재게시 뒤 TOP은 정상 표시됐으나 AROUND YOU는 Stage·Infinite 모두 `Service unavailable (Read leaderboard, HTTP 503)`이었다. C# adapter의 player-range fourth argument는 options object인데 `(int)args[3]`으로 캐스팅해 503을 만든 것이 확인됐다. `params.rangeLimit`만 읽고 0~3을 허용하도록 수정했으며, Node server source budget·Module shim·record/account integration 34/34·Client 정적 검사 통과는 확인했다. Step 8-5 Module 재게시 뒤 실제 Stage/Infinite AROUND YOU를 다시 확인한다.

**Stage·Infinite 조회·Retry 성공 — 2026-10-07.** Step 8-5 재게시 후 Stage TOP 5행·AROUND YOU 4행과 Infinite TOP/AROUND YOU 각 1행이 정상 표시됐고, `Retry` 재조회도 같은 정상 결과를 유지했다. 이는 실제 service rank 정규화와 player-range adapter 경로가 동작한 근거다. 기존 Back 확인과 Offline Pending 재전송 결과도 유지한다. 다만 안전 진단 UI 변경 뒤에는 Script Compilation 성공만 보고됐으므로, Step 9-3과 Step 9 전체 완료 전 최신 전체 EditMode/PlayMode 회귀 결과가 필요하다.

**최신 Unity 회귀 완료 — 2026-10-07.** 안전 진단 UI 및 최신 Module 경로 반영 뒤 사용자가 Unity Script Compilation 성공·예상 밖 Error/Warning 없음, EditMode 890/890 성공·예상 밖 Test Error/Warning 없음, PlayMode 254/254 성공·예상 밖 Test Error/Warning 없음을 확인했다. 따라서 Offline Pending 생성·재시작 보존·명시적 재전송, Stage/Infinite TOP·AROUND YOU·Retry 및 Back 화면 경로를 완료 처리한다.

1. Step 9-1 성공 뒤 일반 게임을 실행한다. 인터넷 연결을 끊고 Stage를 클리어해 Pending 하나를 만든다. 기존 account/ledger/board를 초기화하지 않는다.
2. Result의 Pending 안내와 제출 재시도 동작을 확인한 뒤 Play를 종료·재시작해 같은 Pending이 남는지 확인한다.
3. 인터넷을 연결하고 **Refresh Status** 후 **Retry Pending Uploads**를 한 번 실행한다. `Submitted` 확정과 해당 Pending만 제거되는 결과를 확인한다. Timeout/실패면 Pending을 보존하고 결과를 보고한다.
4. Main Menu의 **Leaderboard → Stage**에서 **TOP**, **AROUND YOU**, **Retry**를 확인한다. 이어 Infinite에서도 같은 항목을 확인한다. 각 목록은 존재하는 행만 표시하며 TOP 10·AROUND YOU 7 초과 여부나 100명 초과/동점 전역 순위는 Step 6 자동 Test 근거로 판정한다.
5. Keyboard와 Mouse로 Retry·Back을 각각 한 번 확인한다. 영어 문구의 잘림·깨짐이나 Scene 참조 문제를 발견하면 Play를 종료하고 오류 화면·대상 Scene/개체 경로를 보고한다. AI는 Scene을 변경하지 않는다.

180일 만료 계산·동일 ID·삭제 저장 실패는 자동 Test가 판정한다. 현재 별도 fake-clock Scene/UI 미리보기는 제공하지 않으므로 존재하지 않는 버튼이나 저장 파일 편집을 요구하지 않는다.

### Step 9-4. Leaderboard 최대 행 레이아웃 — 완료 (2026-10-07)

실서비스 보드에는 당시 Stage 5행·AROUND YOU 4행만 존재했으므로, 최대 TOP 10행과 AROUND YOU 7행이 화면 하단의 Retry/Pending Retry/Back을 가리지 않는지는 별도로 확인했다. 사용자가 `SampleScene`의 `LeaderboardPanel > MenuList`에서 TOP·AROUND YOU Text 영역과 하단 버튼 영역을 분리하도록 RectTransform을 조정하고, 임시 TOP 10건·AROUND YOU 7건 표시에서 겹침 없이 적절한 위치에 표시됨을 확인했다. 그 Scene 변경 뒤 Unity Script Compilation 성공·예상 밖 Error/Warning 없음, EditMode 890/890 및 PlayMode 254/254 성공·예상 밖 Test Error/Warning 없음을 다시 확인했다.

완료 조건: [x] Step 9-1·9-3의 실제 서비스/UI 결과, Step 9-4의 최대 행 레이아웃, 그리고 최신 Unity 회귀가 사용자 보고로 확인됐다. Step 9-2는 기존 verification 계정·Legacy Local Save 부재로 N/A다.

## Step 10. Phase 3 완료를 판정한다

**목적:** 구현·대역·Unity·실제 서비스·화면 근거를 분리해 완료를 결정한다.

AI 작업:

1. Step 1~9 결과를 로드맵 Phase 3 완료 조건과 대조한다.
2. 128건 초과, 100명 초과, 중복/정리/180일 Pending, 페이지 경계 순위의 자동 Test 근거를 기록한다.
3. 실제 신규 제출·기존 자료 보존·서비스 조회·Offline 복구·UI 결과를 별도로 기록한다.
4. 필수 실패와 미확인 항목을 정리하고 Task·Roadmap·Project Memory 상태를 갱신한다.

### Step 10 수행 결과 — 완료 (2026-10-07)

- 신규 계정은 수동 ledger 생성 없이 실제 verification에 제출을 시작했고, 응답 유실 뒤에는 중복 제출하지 않고 서버의 내 행 1건과 재시작 뒤 공개 번호·점수·수락 시각 보존을 확인했다.
- receipt journal/lazy migration/retention/CAS 및 127·128·129·256 제출, 0~201 service-total TOP 10, 동점/전역 rank와 AROUND YOU 최대 7은 자동 Test·대역 근거로 검증했다. 이는 성능 측정이나 전체 transaction 보장이 아니다.
- Offline Pending 1건의 생성·재시작 보존·명시적 재전송·제거, Stage/Infinite TOP·AROUND YOU·Retry·Back을 verification에서 확인했다. player-range adapter와 service rank 정규화는 실제 Module 재게시 뒤 정상 표시로 확인했다.
- 최신 Unity Script Compilation, EditMode 890/890, PlayMode 254/254가 성공했고 예상 밖 Error/Warning은 없었다. 마지막 Scene 레이아웃 변경 뒤에도 같은 Unity 회귀 결과를 다시 확인했다.
- 기존 verification 계정·Legacy Local Save가 없으므로 기존 자료 migration 보존의 실제 A/B 비교는 N/A다. 데이터를 합성하거나 초기화하지 않았다. Dashboard C# Module source 다운로드 불가에 따른 Git `b2953d9` rollback 후보의 동일성 미증명 한계는 유지한다.

완료 조건: [x] Phase 3의 구현·자동 Test·최신 Unity 회귀·verification Module/서비스/UI 확인을 완료했고, 미해결 필수 실패는 없다. Production 배포/운영·정확한 원격 source rollback 보증·Windows Player Build는 Phase 4~5 범위다.

# 영향 범위

이번 작업은 Phase 3 실행 계획 문서와 로드맵의 계획 연결에 한정한다. 실제 구현 영향 범위는 각 Step에서 확정한다. Production 배포·로그 보존/운영 복구는 Phase 4, Windows Player Build 통합 판정은 Phase 5에 연결한다.

# 검증 내용

- Phase 3 로드맵의 구현 대상·완료 조건과 Step 1~10 및 Test 대응표를 대조했다.
- 기존 128/100 제한, 자동 계정 준비, Module 게시 방식, 현재 검증 프로필을 소스에서 확인했다.
- 수동 작업과 자동 경계 검증을 분리하고 아직 없는 실행 도구는 준비 전으로 표시했다.
- 문서 정적 검사에서 Step 1~10 순서, 모든 로컬 링크 대상 존재, 기간/용량/Test/준비 조건, 문서 공백 검사를 통과했다. 로드맵 변경의 `git diff --check`도 통과했다.

# 검증 결과

계획 문서 작성 완료. Phase 3 구현·Unit Test 작성/실행·Unity 실행·실서비스 검증은 미수행이다. 기존 Phase 2의 결과를 Phase 3 완료 근거로 사용하지 않는다.

# 후속 작업

Step 1부터 수행한다. Step 6에서 실제 수동 버튼·자료 경로를 확정한 뒤 사용자가 Step 7~9를 진행한다.

# 관련 문서

- [Roadmap 008](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [RecordSubmission](../../03_Features/RecordSubmission.md)
- [Leaderboard](../../03_Features/Leaderboard.md)
- [RecordSubmissionSystem](../../02_Systems/RecordSubmissionSystem.md)
- [검증 규칙](../../01_Rules/VERIFICATION_RULE.md)
- [구현 규칙](../../01_Rules/IMPLEMENTATION_RULE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
- [Module 게시 안내](../../../UGS/VERIFICATION_DEPLOYMENT.md)

# 관련 작업 기록

- [Phase 1 정책·인계](20260930_01_Phase1ManualSteps.md)
- [Phase 2 실행·검증](20261003_01_Phase2ManualSteps.md)

# 작성 완료 기준

- [x] Phase 3 수행 순서와 담당자를 Step으로 표현했다.
- [x] 사용자가 직접 해야 하는 작업에 목적·동작·기대 결과를 명시했다.
- [x] 수량·시간·경합·migration을 정적 검사/Unit Test 중심으로 배치했다.
- [x] 계획 완료와 구현·Unity·원격 검증 완료를 구분했다.
