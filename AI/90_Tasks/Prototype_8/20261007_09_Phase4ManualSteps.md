# Prototype 8 - Phase 4 운영 환경·배포·복구 준비

## 작업 정보

- 작업일: 2026-10-07
- 범위: Prototype 8 Phase 4
- 담당: AI(정적 검사·Unit Test·문서·절차), 사용자(Unity/UGS Dashboard·CLI·외부 로그 sink·실제 배포·복구 확인)
- 선행 조건: Phase 1~3 완료. Phase 3의 verification 환경 결과를 Production 완료 근거로 사용하지 않는다.

## 목적

verification과 Production을 환경, 데이터, 권한, 배포 이력, 로그 보존 측면에서 분리한다. Production 변경은 대상 확인·복구 자료·권한·로그 보존·호환성 판정이 모두 준비된 경우에만 사용자가 수행한다.

이 문서는 Production 공개 또는 Windows Player Build 검증을 완료 처리하지 않는다. 이는 Phase 5 범위다.

## 현재 확인된 사실과 미확정 값

- verification 대상은 Project `Unity_Flow_State`, Environment `verification`이며 C# Module `FlowStateVerification` 하나가 현재 게시 단위다.
- Phase 3에서 verification의 신규 계정 제출, Offline Pending 복구, Stage/Infinite TOP·AROUND YOU를 확인했다.
- Dashboard는 활성 C# Module source를 내려받아 로컬 Git source와 정확히 대조하는 기능을 제공하지 않았다. Git `b2953d9`는 과거 rollback 후보일 뿐 활성 원격 source의 동일성 증명이 아니다.
- Production Project ID, Environment ID, 조직의 Secret/권한 담당자, 로그 sink 제품·접근 방식·30일 보존/삭제 설정은 아직 확인되지 않았다. 이 값들은 추정하거나 문서에 임의 입력하지 않는다.
- verification의 계정·Local Save·Cloud Save·Leaderboard 행·Secret 값은 Production으로 복사하지 않는다.

## 공통 안전 규칙

1. 비밀값, Access Token, Service Account Key, Player ID, 이전 코드·인증값, 요청 전문, 원격 source 전문을 채팅·Task·커밋·명령 이력에 기록하지 않는다.
2. 한 번에 하나의 Project/Environment와 하나의 배포 단위만 다룬다. `Deploy All`, Board/Cloud Save 초기화, 기존 데이터 덮어쓰기, 환경 간 자료 복사는 금지한다.
3. 자동 검사는 실제 UGS 인증·Secret 읽기·원격 게시를 수행하지 않는다. 실제 원격 작업은 사용자가 직접 수행하고, 성공 여부·대상 식별자·버전/게시 시각·안전한 오류 분류만 공유한다.
4. 설정 또는 코드가 바뀌면 먼저 정적 검사와 Unit Test를 갱신한다. Unity Script Compilation·Test Runner·Player Build는 AI가 실행하지 않는다.
5. Production의 운영 테스트 계정·기록 생성 여부는 별도 승인 없이는 가정하지 않는다. 실제 Player 쓰기 거부 검증은 verification 격리 계정에서 수행하며, Production에서는 정책의 읽기 전용 대조와 배포 절차 재현성으로 분리한다.

## 진행 순서

| Step | 목적 | 주 담당 | 원격 변경 |
| --- | --- | --- | --- |
| 1 | 범위·현행 배포 모델·정적 기준 확정 | AI | 없음 |
| 2 | 환경 분리·호환성·정책 Unit Test 준비 | AI | 없음 |
| 3 | 대상 inventory와 복구 자료를 읽기 전용으로 확보 | 사용자 | 없음 |
| 4 | Production 환경 경계와 Client 선택 값을 구성 | 사용자 + AI | 승인된 설정만 |
| 5 | 최소 권한·Secret 이름/권한·직접 쓰기 차단 확인 | 사용자 + AI | 승인된 설정만 |
| 6 | 30일 안전 로그 sink·삭제 절차를 준비 | 사용자 + AI | 외부 운영 설정 |
| 7 | verification에서 배포·rollback 절차를 연습 | 사용자 + AI | verification만 |
| 8 | Production 배포 준비 상태를 대조하고 승인 게이트를 판정 | AI + 사용자 | 없음 |
| 9 | 승인된 Production 적용 및 복구 가능성 기록 | 사용자 | Production, 명시 승인 뒤만 |
| 10 | Phase 4 완료 판정과 Phase 5 인계 | AI + 사용자 | 없음 |

## Step 1. 범위와 현행 모델을 정적으로 확정한다

### AI 작업

1. `Assets/CloudCode/FlowStateVerification.ccmr`, `UGS/Modules/FlowStateVerification`, `UGS/CloudCode`, `UGS/AccessControl/project-policy.json`의 배포 단위·함수 계약·직접 쓰기 거부 정책을 읽기 전용으로 대조한다.
2. `UGS/VERIFICATION_DEPLOYMENT.md`의 현재 C# Module 절차와 과거 raw JavaScript/개별 Script 게시 이력을 분리한다. 해당 문서의 과거 “Phase 4의 제한된 원격 변경” 절은 현재 Production 절차의 권한이 아니다.
3. Production 값이 코드·테스트 fixture·문서 예시에 하드코딩되거나 verification 값이 Production profile에 재사용되지 않도록 검사할 정적 검사 항목을 정한다.
4. Task의 수동 입력 표를 만들되 Secret 값·Player ID·원격 source 본문 입력란은 만들지 않는다.

### 사용자 작업

없음.

### 완료 조건

- [x] 현재 C# Module 배포 모델과 과거 게시 이력이 구분된다.
- [x] 환경/비밀값/직접 쓰기 정책의 정적 검사 항목이 확정된다.

### Step 1 수동 확인 기록표

Step 1은 AI의 읽기 전용 대조와 정적 검사만으로 완료한다. 이후 사용자가 Step 3,
5, 6, 7에서 사실을 기록할 때는 아래 표를 사용한다. 비밀값, Player ID, 요청 본문,
원격 source 본문은 이 표와 Task에 기록하지 않는다.

| 확인 항목 | 기록 가능한 내용 | 기록 금지 내용 | 사용 Step |
| --- | --- | --- | --- |
| Production 대상 분리 | 내부 inventory 보관 여부, verification과 구분 확인 결과 | Project/Environment ID를 포함한 민감 inventory 전문 | 3 |
| 활성 Module·복구 후보 | Module 표시 이름, 안전한 reference/게시 시각의 내부 보관 여부, source 내려받기 가능 여부 | 원격 source 본문 | 3, 7, 9 |
| Access Control·Secret | 정책 대조 결과, Secret 이름, Cloud Code 접근 권한 대조 결과 | Secret 값, token, key, HMAC | 3, 5 |
| 로그 sink | 제품/담당자/30일 보존·삭제 절차의 내부 보관 여부 | 요청·응답 원문, Player ID, 공개 번호 | 6 |
| 배포·rollback rehearsal | 대상 환경, Module 하나, Deploy Selected 여부, 안전한 결과 분류 | Deploy All 결과, 데이터 초기화/환경 간 복사 자료 | 7 |
| Production gate | `READY_FOR_APPROVAL` 또는 `PRODUCTION_DEPLOY_BLOCKED`와 누락 근거 | 승인 전 Production 변경 결과 | 8 |

### Step 1 수행 결과 — 2026-10-07

- [x] 1-1: `.ccmr` manifest, C# Module, embedded `UGS/CloudCode` 함수 9개와 Player Write Deny 정책을 읽기 전용으로 대조했다.
- [x] 1-2: 현재 배포 단위가 `FlowStateVerification` C# Module 하나임을 확인하고, 과거 JavaScript `query-records` 단독 게시 절을 참고 이력으로 명시했다.
- [x] 1-3: Production 식별자 미하드코딩, verification 전용 runtime scope, Module 단일 manifest, Secret 이름만의 allowlist, Player 직접 Write Deny를 정적 검사 항목으로 확정했다.
- [x] 1-4: 위 수동 확인 기록표를 추가했다. 값 자체가 아닌 내부 보관 여부와 안전한 결과만 기록한다.
- [x] 완료: Step 1의 두 완료 조건을 충족했다. 이 결과는 Production inventory·Secret 권한·원격 Module 활성 source 동일성·배포 승인 확인이 아니며, 해당 사실은 후속 Step에서만 확인한다.

## Step 2. 자동 검증을 먼저 준비한다

### AI 작업

다음 Unit Test와 정적 검사를 구현하고 실행한다. 실제 UGS 호출, Secret 조회, CLI 인증, Dashboard 변경은 포함하지 않는다.

1. 환경 쌍 검사: verification과 Production은 서로 다른 non-empty Project/Environment 조합이어야 하며, 동일 조합·누락·verification ID 재사용을 거부한다.
2. Client 환경 선택 검사: 빌드에 선택된 환경만 사용하고, 런타임 입력·Remote Config·사용자 Local Save가 대상 환경을 바꾸지 못하게 한다. 온라인 Local Save/Pending 경로는 `(Project ID, Environment ID)`로 분리됨을 검증한다.
3. 배포 manifest 검사: Production manifest가 C# Module 하나만 가리키며, `Deploy All`·개별 raw JavaScript 게시·입력 정의 변경을 계획에 포함하지 않음을 검증한다.
4. Access Control 정책 검사: Player의 Cloud Save Write와 Leaderboards Write Deny 문장이 유지되고, 광범위 Player Allow 또는 정책 Sid 충돌 후보를 실패로 처리한다.
5. 구조화 로그 검사: 허용 필드(environment, module/version reference, safe outcome/reason, duration bucket, correlation ID, rate-limit result)만 사용하고, Secret·token·Player ID·공개 번호·이전 자격 증명·요청 본문이 포함되면 실패한다.
6. 배포/rollback 상태 전이 검사: backup 없음, 대상 불일치, 로그 sink 미준비, 권한 불일치, 호환성 미판정에서는 Production deploy gate가 열리지 않음을 검증한다. rollback은 같은 환경의 직전 확인 버전만 선택하며 데이터 삭제/환경 간 복사를 하지 않음을 검증한다.
7. Client/Module 호환성 검사: 지원 Client 버전 범위, API 입력/응답의 backward-compatible 조건, Local Save migration marker가 있는 자료에 구버전 writer를 재도입하지 않는 조건을 표 기반 Unit Test로 검증한다.

### 사용자 작업

없음. AI가 준비한 Unity Test는 후속 Step 8 또는 10에서 사용자가 실행한다.

### 완료 조건

- [x] 환경 분리·정책·로그 필드·배포 gate·rollback·호환성 Unit Test와 정적 검사가 통과한다.
- [x] 자동 검사 결과를 실제 Production 설정 또는 실제 Secret 검증으로 과장하지 않는다.

### Step 2 수행 결과 — 2026-10-08

- [x] 2-1: non-empty verification/Production 환경 쌍과 `(Project ID, Environment ID)` Local Save scope 격리 정책을 단위 테스트했다. 기존 Unity EditMode `OnlineLocalSaveScopeTests`는 실행 대기 상태로 유지한다.
- [x] 2-2: build scope와 런타임 요청 scope가 같을 때만 허용하는 정책을 추가했고, Local Save가 다른 환경이면 온라인 사용을 차단하는 기존 C# 정적 계약을 재사용했다.
- [x] 2-3: `.ccmr`의 단일 C# Module manifest만 허용하고 raw JavaScript 경로·추가 배포 항목을 거부하는 단위 테스트를 추가했다.
- [x] 2-4: 두 Player Write Deny, Player Allow 금지, 중복 Sid 거부를 실제 `project-policy.json`에 대해 단위 테스트했다.
- [x] 2-5: 허용된 7개 필드만 갖는 구조화 로그 정책과 Player ID·payload 등 추가 필드 거부를 단위 테스트했다.
- [x] 2-6: inventory·rollback reference·권한·로그·호환성·verification rehearsal 중 하나라도 빠지면 Production gate를 닫고, 같은 환경의 직전 Module reference만 데이터 삭제·환경 복사 없이 rollback하도록 단위 테스트했다.
- [x] 2-7: 요청/응답 호환성과 Local Save migration marker 뒤 legacy writer 재도입 금지를 표 기반 단위 테스트했다.
- [x] 완료: 로컬 Node 단위 테스트와 정적 검사를 통과했다. Unity EditMode/PlayMode Test의 실제 실행, Production inventory/Secret/원격 설정 확인은 이 결과에 포함하지 않는다.

## Step 3. 대상 inventory와 복구 자료를 읽기 전용으로 확보한다 — 사용자

변경 전에 Dashboard 또는 조직의 승인된 관리 도구에서 아래 항목을 **읽기 전용**으로 확인한다. 값 자체가 민감하거나 식별 가능한 경우에는 안전한 내부 보관소에만 저장하고 채팅에는 성공/실패만 보고한다.

1. Production Project와 Production Environment의 표시 이름 및 ID가 verification과 다른지 확인한다.
2. Cloud Code, Cloud Save, Leaderboards, Access Control의 대상 환경이 모두 Production인지 확인한다. Player Authentication의 Identity Provider는 프로젝트 범위 설정으로 별도 읽기 전용 대조하며, Client 초기화의 Production 환경 선택과 혼동하지 않는다.
3. 현재 활성 Module의 표시 이름, 배포/게시 시각, Dashboard가 제공하는 version/reference, 입력 정의를 보관한다. source 다운로드가 안 되면 그 사실과 Git commit/archive hash를 별도로 기록한다.
4. Access Control 정책 전문, Secret의 **이름과 Cloud Code 접근 권한만** 보관한다. Secret 값은 열람·내보내기·공유하지 않는다.
5. Stage/Infinite Leaderboard 설정과 Cloud Save schema/key namespace를 읽기 전용으로 기록한다. 계정, ledger, Leaderboard 행, Board를 초기화하거나 verification 데이터를 복사하지 않는다.
6. 현재 Client release candidate의 Git commit, Module source commit, 호환성 표의 버전을 같은 내부 변경 기록에 연결한다.

중단 조건: Project/Environment를 확정할 수 없거나, 원격 대상이 verification과 구분되지 않거나, 복구 자료의 저장 위치가 없으면 이후 Step을 진행하지 않는다.

### Step 3 로컬 구성 계약 기록 — 2026-10-08

아래는 저장소의 문서·정책 artifact에서 확인한 **대조 기준**이다. Production 원격 설정이나
Secret 값의 존재를 뜻하지 않으며, 사용자는 Step 3-4~3-6에서 각 항목을 읽기 전용으로
대조해야 한다.

| 영역 | 로컬 구성 계약 | Production 원격 대조 상태 |
| --- | --- | --- |
| Access Control | `Player`의 `urn:ugs:leaderboards:/**` 및 `urn:ugs:cloud-save:/**` `Write`는 각각 `Deny`다. 추가 `Player Allow`나 동일 Sid 충돌은 허용하지 않는다. | 미확인 |
| Secret | Cloud Code가 읽을 수 있는 Secret **이름**은 `FS_TRANSFER_HMAC_VERIFICATION_V1` 하나다. 값은 기록·열람·내보내기 금지이며, 필요한 형식은 canonical Base64 32-byte key다. | 이름·Cloud Code 읽기 권한 미확인; 값은 대조 대상 아님 |
| Leaderboard | Stage: `fs-stage-stage-001-r1`, ascending, keepBest, rules 1. Infinite: `fs-infinite-v2`, descending, keepBest, scoring 2. | 기존 Production 보드/행 없음 보고; 보드 설정값 자체는 미확인 |
| Cloud Save — 계정·receipt | Private Custom data에서 `fs8-<kind>-<id>` container와 `fs_account_v1` key를 사용한다. receipt는 `r-<account>-<bucket>-<serial>`/`l-<account>-<prefix>` container, `manifest`·`fs_receipt_guard_v1`·`r-<submission>` key를 사용한다. 모든 값은 schemaVersion 1 및 환경 scope와 일치해야 한다. | namespace·access class·schema 존재 여부 미확인 |
| Cloud Save — legacy ledger | 기존 Player Protected key는 `fs_submission_ledger_v1`이며 초기 seed는 `version: 1`, 빈 `active`/`entries`/`best` object다. 기존 값을 덮어쓰거나 초기화하지 않는다. | Production에 기존 ledger/seed 없음 보고; 생성·복사 금지 |

로컬 근거: `UGS/AccessControl/project-policy.json`, `UGS/CloudCode/transfer-cryptography.js`,
`UGS/CloudCode/account-store.js`, `UGS/CloudCode/receipt-store.js`,
`UGS/CloudCode/legacy-account-store.js`, `UGS/VERIFICATION_DEPLOYMENT.md`.
Cloud Save container·key의 사람이 읽을 수 있는 설명은
[`UGS/CLOUD_SAVE_DATA_REFERENCE.md`](../../../UGS/CLOUD_SAVE_DATA_REFERENCE.md)에 별도로 관리한다.
Production의 비밀값 없는 inventory와 deploy gate 상태는
[`UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md`](../../../UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md)에 관리한다.

### Step 3 현재 진행 기록 — 2026-10-09

- [x] 3-1: 사용자가 Production 대상이 verification과 구분됨을 읽기 전용으로 확인했다. 식별값은 Task에 기록하지 않는다.
- [x] 3-2: Production의 Cloud Code·Cloud Save·Leaderboards·Access Control에 등록 자료나 설정이 없음을 읽기 전용으로 확인했다. 이는 첫 배포 전 비구성 baseline이며 Production 적용 준비 완료를 뜻하지 않는다.
- [x] 3-3: Production에 C# Module이 없으므로 활성 Module·게시 시각·version/reference·rollback reference가 없음을 기록했다. verification `FlowStateVerification`은 Dashboard version/reference와 source 다운로드를 제공하지 않으며, Git commit은 원격 source 동일성 증명이 아니다.
- [x] 3-4: verification에 `FS_TRANSFER_HMAC_VERIFICATION_V1` Secret 이름이 존재하고 서비스 액세스가 Cloud Code만임을 읽기 전용으로 확인했다. 값은 열람·기록하지 않았다.
- [x] 3-5: Production에 Leaderboard 보드와 Cloud Save 데이터가 없음을 읽기 전용으로 확인했다. namespace·schema·key를 새로 만들거나 verification 자료를 복사하지 않는다.
- [x] 3-6: Client release candidate와 Module source candidate를 같은 Git `a6258ef5a6e1c22126fa3bd6ec98caeb74e8db6c`으로 연결하고, Local Save v7·Cloud Code schema v1·Phase 4 호환성 표 v1을 `UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md`에 기록했다. 이는 원격 Module 동일성·Production 승인·rollback reference의 증명이 아니다.

### Step 3 Dashboard 관찰 기록 — 2026-10-08

| 환경 | 영역 | 읽기 전용 관찰 결과 | 판정 |
| --- | --- | --- | --- |
| verification | Access Control | `DenyPlayerCloudSaveWrites`와 `DenyPlayerLeaderboardWrites`가 각각 Player의 전체 Cloud Save/Leaderboards `Write`를 `Deny`하며 만료되지 않는다. | 로컬 정책 artifact와 일치 |
| verification | Secret | `FS_TRANSFER_HMAC_VERIFICATION_V1` 이름의 개발 환경 비밀이 존재하며 서비스 액세스는 Cloud Code만이다. 값은 열람·기록하지 않았다. | Secret 이름·Cloud Code 최소 접근이 로컬 계약과 일치 |
| project 범위 | Player Authentication | Identity Providers 목록이 비어 있다. 현재 Client의 Anonymous sign-in은 외부 Identity Provider를 요구하지 않는다. | 환경별 설정으로 취급하지 않음; 프로젝트 범위의 읽기 전용 관찰로 기록 |
| verification | Leaderboards | `fs-infinite-v2`는 Highest to lowest·Best score, `fs-stage-stage-001-r1`은 Lowest to highest·Best score다. 두 보드 모두 reset schedule, buckets, tiers, live/archived version이 없다. | 로컬 보드 계약과 일치 |
| verification | Cloud Save / Game Data Private | `fs8-account-*`, `fs8-allocator-public`, `fs8-ledger-*`, `fs8-number-*`, `fs8-owner-*`, `fs8-player-*`, `fs8-receipt-*`, `fs8-t-*`, receipt journal `r-*` 및 locator `l-*` container가 관찰됐다. `fs8-*`에는 `fs_account_v1`·`fs_creation_guard_v1`, journal에는 `manifest`·`s-001`·`fs_receipt_guard_v1`, locator에는 `r-<submission>`·`fs_receipt_guard_v1`가 관찰됐다. | `CLOUD_SAVE_DATA_REFERENCE.md`의 namespace·key 계약과 일치 |
| verification | Cloud Save / Probe | `p2v-*` Private Game Data가 관찰됐다. | `LIVE_TRANSFER_STORAGE_PROBE`의 격리 verification 검사 자료이며 자동 삭제 대상이 아님 |
| Production | Cloud Code / C# Modules | C# Module 없음. | 첫 배포 전 baseline; 활성 Module·rollback reference 없음 |
| Production | Access Control | 등록 정책 없음. | 첫 배포 전 baseline; 정책 revision/reference 없음 |
| Production | Leaderboards | 등록 보드 없음. | 첫 배포 전 baseline; Board 생성·verification 자료 복사 금지 |
| Production | Cloud Save | 데이터 없음. | 첫 배포 전 baseline; container·key·schema 생성 금지 |
| Production | Secret | 개발 환경 Secret 없음. | 첫 배포 전 baseline; Secret 생성·값 열람 금지 |

위 관찰은 실제 Secret 값, Player ID, Cloud Save 값 또는 원격 source를 포함하지 않는다. Production에
직전 Module reference·정책·보드/Cloud Save baseline이 **없음**을 첫 배포 전 baseline으로 기록했으므로
Step 3 inventory는 완료했다. 다만 rollback reference가 없고 이후 Step 4~7의 증거도 없으므로 Step 8의
Production deploy gate는 열리지 않는다.

### 완료 조건

- [x] Production 대상과 verification 대상의 분리가 증명 가능한 내부 inventory로 보관된다.
- [x] 배포 전 복구 후보와 현재 설정의 읽기 전용 기준이 있다. Production은 첫 배포 전 상태이므로 원격 복구 후보가 없음을 명시적으로 기록했다.

### Step 3 수행 결과 — 2026-10-09

- [x] 완료: Production의 비구성 baseline, verification의 현재 구성, Secret 최소 접근, Cloud Save/Leaderboard 계약 및 Client·Module·Local Save 호환성 연결을 읽기 전용으로 기록했다.
- [x] 완료 한계: Production의 원격 Module/정책/Board/Cloud Save/Secret은 모두 없고 rollback reference도 없다. 이는 Step 3의 inventory 결과이며 Production 변경 또는 Step 8 승인 결과가 아니다.

## Step 4. Production 환경 경계와 Client 선택 값을 구성한다 — 사용자

Step 2의 자동 검사와 Step 3 inventory가 완료된 뒤에만 수행한다.

1. Unity Dashboard에서 Production Environment를 선택한 뒤 Cloud Code·Cloud Save·Leaderboards가 모두 같은 Production Environment를 가리키는지 확인한다. Player Authentication의 Identity Provider는 프로젝트 범위 설정이므로 별도로 읽기 전용 대조한다.
2. Client의 Production build profile에는 승인된 Production Project/Environment 쌍만 넣고, verification profile과 별도 build profile/Local Save namespace를 사용한다. 한 build가 사용자 입력으로 두 환경을 전환하지 않게 한다.
3. Production에 verification 계정, Local Save, Cloud Save 행, Leaderboard 기록, transfer code, Secret을 복사하지 않는다.
4. 환경값 또는 build profile을 새로 추가/수정했다면 그 값은 안전한 내부 배포 설정에만 저장한다. 값은 Task·소스·테스트 fixture에 복사하지 않는다.
5. 코드 변경이 생긴 경우 AI가 정적 검사와 Unit Test를 갱신한 뒤, 사용자는 Unity Script Compilation과 전체 EditMode/PlayMode Test를 실행한다.

### Step 4 세부 작업

#### 4-1. 빌드별 Client 환경 경계 — AI 완료

- [x] `FLOW_STATE_PRODUCTION`이 **활성 Build Profile에만** 있을 때 Production 설정을 선택하고, 그 외에는 verification 설정만 선택하도록 했다.
- [x] Production Project ID·Environment ID는 Git 미추적 `OnlineRecordProductionConfiguration.cs`에서만 읽는다. 템플릿은 `Assets/Scripts/Runtime/Features/OnlineRecordProductionConfiguration.cs.example`이며 실제 파일과 `.meta`는 `.gitignore`에 등록했다.
- [x] Production 설정이 비어 있거나 Cloud Project ID·Authentication profile·환경 이름이 맞지 않으면 UGS 초기화 전에 fail-closed한다. Runtime 입력·Remote Config·Local Save는 환경을 바꿀 수 없다.
- [x] Local Save/Pending는 기존 `(Project ID, Environment ID)` scope로 계속 분리하며, 이전 verification 자료는 Production scope로 재할당하지 않는다.

#### 4-2. Production Build Profile 연결 — 사용자 수동 작업

1. 안전한 로컬 배포 workspace에서 `OnlineRecordProductionConfiguration.cs.example`을 같은 폴더의 `OnlineRecordProductionConfiguration.cs`로 복사한다. Production의 승인된 Project ID·Environment ID만 입력하고, 이 파일·`.meta`·값을 Git/Task/채팅에 넣지 않는다.
2. Unity **Build Profiles**에서 Production 전용 profile을 만든다. Production profile의 **Build Data > Scripting Defines**에만 `FLOW_STATE_PRODUCTION`을 추가한다. verification profile에는 이 symbol이 없어야 한다.
3. Unity의 Environment Selector는 profile별 설정이 아니므로 Production/verification build 전환 수단으로 사용하지 않는다. 이 Client는 profile symbol이 선택한 `OnlineRecordProductionConfiguration.cs`의 `EnvironmentName`을 `InitializationOptions.SetEnvironmentName(...)`에 전달하여 UGS 환경을 고정한다.
4. Cloud Project 연결은 profile별 값이 아니라 Unity Project 전체 값이다. Project의 Cloud Project 연결이 1번의 승인된 Project ID와 일치하는지 읽기 전용으로 대조한다. profile별 UGS 환경 분리는 `FLOW_STATE_PRODUCTION`과 `InitializationOptions.SetEnvironmentName(...)`로만 수행한다.
5. verification profile을 다시 활성화하여 `FLOW_STATE_PRODUCTION`이 없고, `OnlineRecordProductionConfiguration.cs`가 없거나 비어 있어도 verification 경로가 유지되는지 확인한다. 두 profile을 동시에 활성화하거나 하나의 build에서 런타임으로 전환하지 않는다.
6. Unity compilation과 build는 사용자가 별도 시점에 실행한다. 이 Step에서 Scene 수정, Production UGS 데이터 생성, 로그인, Cloud Code 호출, Player/Cloud Save/Leaderboard 쓰기는 하지 않는다.

수동 작업 후에는 값 대신 다음 결과만 보고한다: Production profile에서 symbol 존재 여부, verification profile에서 symbol 부재 여부, Production 설정 파일의 Git 미추적 여부, Cloud Project/Environment display name 대조 성공·실패. 설정값·Secret·Player ID는 보고하지 않는다.

### Step 4 수행 기록 — 2026-10-09

- [x] 4-1: build profile 전용 compile symbol과 Git 미추적 Production 설정 파일을 사용해 Client 환경 선택을 고정하고, 잘못되거나 누락된 설정을 UGS 초기화 전에 차단했다.
- [x] 4-2-1: 사용자가 Production Build Profile의 Scripting Defines에만 `FLOW_STATE_PRODUCTION`을 등록하고, Git 미추적 `OnlineRecordProductionConfiguration.cs`에 승인된 Project ID·Environment ID를 입력했다고 보고했다. 값은 AI가 열람·기록하지 않았다.
- [x] 4-2-2: verification은 별도 Build Profile이 아닌 기본 Windows profile을 사용하며, 이 profile은 Build Profile 전용 Scripting Defines를 지원하지 않아 `FLOW_STATE_PRODUCTION`이 없다. Cloud Project 연결은 profile별로 분리하지 않고 Project 전체가 승인된 Cloud Project에 연결되어 있음을 사용자가 읽기 전용으로 확인했다.
- [x] 4-2-3: 사용자가 Unity Script Compilation 성공을 확인했다. Player Build와 Test Runner는 이 Step의 AI 작업에 포함하지 않는다.

### 완료 조건

- [x] Production Build Profile은 `FLOW_STATE_PRODUCTION`과 Git 미추적 승인 설정으로 Production 환경 조합을 선택하고, 기본 Windows verification profile은 symbol 없이 verification 조합을 선택한다. Local Save/Pending는 `(Project ID, Environment ID)` namespace로 분리된다.
- [x] 사용자 조작이나 Local Save로 환경을 교차 선택할 수 없다.

### Step 4 수행 결과 — 2026-10-09

- [x] 완료: Production Build Profile과 기본 Windows verification profile의 compile symbol·UGS 초기화 환경·Local Save/Pending scope 분리를 구성하고, 사용자가 Unity Script Compilation 성공을 확인했다.
- [x] 완료 한계: Unity Cloud Project 연결은 Project 전체 설정이며 profile별로 분리되지 않는다. Production/verification 서비스 환경은 Client 초기화의 명시적 환경 이름으로 분리하며, Production UGS 데이터 생성·원격 호출·Player Build·Test Runner는 수행하지 않았다.

## Step 5. 최소 권한·Secret 접근·직접 쓰기 차단을 확인한다 — 사용자

1. Step 3에서 백업한 정책과 `UGS/AccessControl/project-policy.json`을 대조한다. Player의 Leaderboards Write와 Cloud Save Write Deny가 유지되는지 확인한다.
2. Player Write를 다시 Allow하는 넓은 정책, 같은 Sid의 충돌 정책, 대상 환경 불일치를 발견하면 정책을 삭제/덮어쓰지 말고 중단한다.
3. Cloud Code가 필요한 Secret의 **이름과 읽기 권한**만 확인한다. 값, key, token, HMAC은 표시·복사·로그 기록하지 않는다.
4. 정책을 수정해야 한다면 Step 3 backup 뒤 조직의 승인된 CLI/관리 절차로 Production에만 적용한다. 적용 직후 대상 Environment와 정책 revision/reference를 다시 읽어 대조한다.
5. Player 직접 쓰기 거부의 실제 403 검증은 Production 데이터에 영향을 주지 않는 verification 격리 계정에서 수행한다. Production에서는 읽기 전용 정책 대조와 동일 policy artifact의 정적/Unit Test로 제한한다.

### 완료 조건

- [x] Player 직접 쓰기를 차단하는 정책과 Cloud Code의 필요한 Secret 접근이 대상 환경에서 대조된다.
- [x] Secret 값은 어떤 확인 자료에도 포함되지 않는다.

### Step 5 세부 작업

#### 5-1. 로컬 최소 권한 계약 — AI 완료

- [x] `UGS/AccessControl/project-policy.json`은 Player의 `urn:ugs:leaderboards:/**` 및 `urn:ugs:cloud-save:/**` `Write`를 각각 하나의 `Deny` statement로 제한한다. Player `Allow`와 중복 Sid는 허용하지 않는다.
- [x] C# Module runtime과 embedded Cloud Code는 `FS_TRANSFER_HMAC_VERIFICATION_V1` 이름만 Secret Manager에 요청한다. Secret 값은 로그·응답·Local Save에 넣지 않는다.
- [x] verification 전용 Editor 검증 창에 Player token으로 Leaderboards와 Cloud Save 직접 Write를 각각 한 번 시도하고 둘 다 403일 때만 통과시키는 경로가 있다.
- [x] 한계 기록: 현재 Module과 embedded Cloud Code의 Project/Environment 계약은 verification에 고정되어 있다. Production Access Control·Secret 설정은 최소 권한 준비일 뿐 Production Module 배포 가능 여부나 Production 호출 성공을 뜻하지 않는다.

#### 5-2. Production 최소 권한 설정 — 사용자 수동 작업

Production은 현재 정책과 Secret이 없는 첫 배포 전 baseline이다. 승인된 설정을 만들려면 Dashboard에서 `production` 환경을 다시 확인한 뒤 다음을 수행한다.

1. **Access Control**에 로컬 `UGS/AccessControl/project-policy.json`과 동일한 두 statement를 만든다. 각각 `DenyPlayerLeaderboardWrites` / `DenyPlayerCloudSaveWrites`, Effect `Deny`, Action `Write`, Principal `Player`, Resource `urn:ugs:leaderboards:/**` / `urn:ugs:cloud-save:/**`, Expires `Never`이어야 한다. 추가 Player `Allow`는 만들지 않는다.
2. **Secret**에 이름 `FS_TRANSFER_HMAC_VERIFICATION_V1`을 만든다. 값은 verification Secret을 복사하지 않은 독립적인 canonical Base64 32-byte key여야 하며, 화면·채팅·문서·Git에 표시하거나 기록하지 않는다. 서비스 액세스는 **Cloud Code만** 부여한다. 설명은 `Production account transfer HMAC v1`을 사용한다.
3. verification Secret을 만들 때 사용한 동일한 사용자 전용 도구를 Production에서도 실행한다. 클립보드 기록/장치 간 동기화를 끈 뒤 프로젝트 루트에서 `node UGS/Verification/new-transfer-hmac-secret.cjs`를 인수 없이 실행한다. `Ready:`만 확인하고, 생성 값은 target environment의 Secret Value 칸에 한 번만 붙여넣은 뒤 `Set-Clipboard -Value ""`로 클립보드를 비운다. 이 도구는 32 random bytes를 44-character canonical Base64로 만들고 값을 출력·파일 저장·명령 인수 전달하지 않는다.
4. 저장 뒤 읽기 전용으로 두 정책의 Sid/Effect/Action/Principal/Resource/Expires와 Secret의 이름·설명·서비스 액세스만 재확인한다. 값, token, key, HMAC, Player ID는 공유하지 않는다.
5. 하나라도 다르거나 Player `Allow`/Sid 충돌이 보이면 덮어쓰기·삭제·재시도하지 말고 중단한다.

#### 5-3. verification 실제 직접 Write 거부 — 사용자 수동 작업

1. Production profile이 아닌 기본 Windows verification profile을 활성화한다.
2. Scene을 수정하지 말고 Unity 메뉴 **Flow State > Online Record Verification**을 연다. 격리 verification session을 준비한 뒤 **Player 직접 Write 403 검증**을 한 번만 실행한다.
3. 기대 결과는 `PASS / PLAYER_WRITE_DENIED / Leaderboards=403 / CloudSave=403`이다. 401·네트워크 오류·일반 오류는 통과가 아니다. 2xx이면 중단하고 두 HTTP 상태 코드만 보고한다. probe key가 생겼을 가능성은 verification Dashboard에서만 확인하며, 전체 데이터 초기화는 금지한다.

### Step 5 수행 기록 — 2026-10-09

- [x] 5-1: 로컬 Access Control·Secret allowlist·verification 403 probe 경로를 정적 대조했다.
- [x] 5-2-1: 사용자가 Production에 Leaderboards/Cloud Save Player Write Deny 정책을 만들고, 독립 Secret 값을 생성해 서비스 액세스를 Cloud Code만으로 제한했다고 보고했다. Secret 값은 열람·기록하지 않았다.
- [x] 5-2-2: Production에서 `DenyPlayerCloudSaveWrites`와 `DenyPlayerLeaderboardWrites`가 각각 Player의 전체 Cloud Save/Leaderboards `Write`를 `Deny`, `Expires=Never`로 설정했음을 읽기 전용으로 대조했다. Secret `FS_TRANSFER_HMAC_VERIFICATION_V1`의 설명은 `Production account transfer HMAC v1`이고 서비스 액세스는 Cloud Code만이다. 값은 열람·기록하지 않았다.
- [x] 5-3: 2026-10-09 verification 격리 `flow-state-phase2-a`에서 처음 계정 상태 조회는 `WINDOW_TIMEOUT`이었으나, 명시적 상태 확인 뒤 `State=Ready`, `Pending=0`, `RemoteAllowed=True`가 됐다. 이어 Player 직접 Write probe를 한 번 실행해 `PASS / PLAYER_WRITE_DENIED / Leaderboards=403 / CloudSave=403`을 확인했다. Secret·token·Player ID·응답 본문은 기록하지 않았다.

### Step 5 수행 결과 — 2026-10-09

- [x] 완료: Production의 두 Player Write Deny와 Cloud Code 전용 Secret 최소 접근을 읽기 전용으로 대조하고, verification 격리 Player의 Leaderboards/Cloud Save 직접 Write가 각각 HTTP 403으로 거부됨을 확인했다.
- [x] 완료 한계: Production Module은 아직 배포하지 않았으며 현재 Module source는 verification scope에 고정되어 있다. 이 Step은 Production Module 호출·Secret 실제 읽기·운영 Player 호출 또는 deploy gate 승인을 뜻하지 않는다.

## Step 6. 30일 안전 로그 sink와 삭제 절차를 준비한다 — 사용자

1. 조직이 승인한 로그 sink를 선정하고 접근 담당자, 보관 기간 30일, 자동 삭제 또는 삭제 runbook, 접근 제어 방식을 내부 운영 기록에 명시한다.
2. UGS Cloud Code 기본 로그만으로 30일 요건을 충족하는지 공식 보존 설정/계약으로 확인한다. 충족하지 못하면 PII·비밀값을 받지 않는 최소 외부 sink를 먼저 준비한다.
3. Step 2의 허용 필드만 sink schema로 등록한다. raw request/response, Player ID, 공개 번호, Secret, token, transfer credential은 수집 금지로 설정한다.
4. verification에서 합성한 안전 이벤트로 수집·조회·권한 제한·삭제 절차를 점검한다. 실제 계정/Secret/요청 본문을 사용하지 않는다.
5. 30일 보존 또는 삭제 절차를 입증할 수 없으면 Production 배포 gate를 닫아 둔다.

### 완료 조건

- [ ] 최소 구조화 로그의 허용 필드·접근 권한·30일 보존·삭제 절차가 문서와 설정으로 대조된다.
- [ ] sink가 준비되지 않았으면 Production 배포가 명시적으로 차단된다.

### Step 6 진행 기록 — 2026-10-09

- [x] 6-1: `UGS/SAFE_LOG_SINK_CONTRACT.md`에 Step 2의 closed schema(7개 허용 필드), 금지 데이터,
  최소 접근 제어, 30일 보존 및 삭제 계약을 기록하고 정적 계약 검사를 추가했다.
- [x] 6-2 판정: Unity Cloud Code 기본 로그는 공식 문서상 7일 보존이므로 30일 요건을 충족하지
  못한다. 또한 현재 C# Module source에는 external sink emitter가 없다. 외부 sink가 설정되지 않은
  현재 상태에서는 `PRODUCTION_DEPLOY_BLOCKED`를 유지한다.
- [x] 6-2 준비: 로컬 Docker 개발과 Cloud Run 운영에 공통인 `UGS/LogSink` closed-schema 수집기와
  HMAC 경계, 무비용 한계 및 수동 배포 절차를 준비했다. Cloud Run/Logging/Secret/IAM 실제 설정과
  Module 전송 연결은 아직 수행하지 않았다.
- [x] 6-2 로컬 확인: 사용자가 로컬 `.env`에 로그 수집 전용 HMAC을 비공개로 입력한 뒤
  `docker compose up --build`를 실행했고, `http://127.0.0.1:8080/healthz`가 HTTP 204를
  반환함을 확인했다. 이후 `docker compose down`으로 로컬 컨테이너를 중지했다. Secret 값·요청 본문은
  기록하지 않았다.
- [x] 6-2 로컬 완성도 확인: health check 외에 합성 안전 이벤트 수집, schema 밖 필드 거부, 잘못된
  HMAC·만료 timestamp 거부, malformed/oversized body 거부를 확인했다. `docker compose down` 뒤 재기동해
  probe 전체를 다시 통과했고, Docker 로그에는 허용된 7개 합성 필드만 있었으며 HMAC, request body,
  Player 식별자, 원시 오류 전문은 없었다. Cloud Run staging 준비 단계로 진행할 수 있다.
- [x] 6-2 로컬 HTTP 계약 확인: 실행 중인 Docker 수집기에 사용자 전용 `probe.ps1`을 실행해 health
  `204`, 합성 안전 이벤트 `202`, 추가 필드 `400`, 잘못된 HMAC·만료 timestamp `401`, 잘못된 JSON
  `400`, oversized body `413`, 잘못된 content type `415`, 알 수 없는 경로 `404`를 확인했다. Secret·
  signature·request body는 출력하거나 기록하지 않았다.
- [x] 6-2 로컬 재기동/출력 확인: `docker compose down` 뒤 재기동한 probe가 전체 통과했고,
  `docker compose logs`에는 허용된 7개 합성 필드만 있음을 사용자가 대조했다.
- [x] 6-2 배포 전 코드 준비: 수집기가 잘못된 JSON/schema/HMAC/timestamp/content type/경로를
  명시 상태 코드로 거부하고, oversized body는 연결 종료 대신 `413`으로 거부하도록 보강했다. Node
  로컬 HTTP 계약 검사에는 정상 `202`과 모든 거부 경로가 포함된다. `UGS/LogSink/probe.ps1`은
  실행 중인 Docker 수집기에 같은 합성 HTTP 경계를 확인하는 사용자 전용 probe이며, Docker 실행
  확인은 별도 사용자 작업이다.
- [x] 6-3.1-AI 준비: `UGS/LogSink/CLOUD_RUN_STAGING_SETUP.md`에 verification 전용 GCP project,
  region, 필요한 API, 비용 알림 경계, 안전한 완료 보고 형식과 중단 기준을 작성했다. 실제 외부 자원은
  만들거나 조회하지 않았다.
- [x] 6-3.1 사용자 구성 — 2026-10-10: verification 전용 GCP project 격리, Cloud Run region 결정,
  Cloud Run/Logging/Secret Manager/Artifact Registry API 활성화를 사용자가 대조했다. GCP project에는
  전역 region 등록이 없으므로, 결정한 region은 아직 리소스에 적용하지 않았고 6-3.4에서 Cloud Run
  service와 Artifact Registry repository를 만들 때 적용한다. service/repository/Secret/IAM/UGS Module/
  Production 변경은 수행하지 않았다. Project ID, endpoint, credential은 기록하지 않았다.
- [x] 6-3.2-AI 준비: `UGS/LogSink/CLOUD_RUN_SECRET_AND_IAM.md`에 GCP/UGS verification LogSink
  HMAC은 같은 값, local/Production/account-transfer HMAC은 서로 다른 값이라는 관계와 secret-level
  최소 IAM runbook을 기록했다. Cloud Run stdout은 자동 수집되므로 Logging Writer를 service identity에
  부여하지 않도록 정정했다.
- [x] 6-3.2 사용자 구성 완료 (2026-10-10): verification LogSink HMAC은 local/Production/account-transfer
  HMAC과 분리해 생성했고, GCP Secret Manager와 UGS verification Environment Secret에 같은 값으로 비공개
  저장했다. LogSink service account에는 해당 GCP secret 하나의 Secret Accessor만 부여했고, UGS service
  access는 Cloud Code만 선택했다. JSON key, Cloud Run service, repository, endpoint, Module 연결, Production
  변경은 만들지 않았다. 값·principal 식별자는 기록하지 않는다.
- [x] 6-3.3 30일 보존·삭제 설정 완료 (2026-10-10): verification GCP project의 `_Default` 로그 버킷
  보관 기간이 30일임을 대조했다. LogSink를 Cloud Storage·BigQuery·Pub/Sub·다른 버킷으로 내보내거나
  보관하는 경로가 없고, `_Required` 감사 로그 버킷은 변경하지 않았음을 확인했다. Cloud Run service,
  endpoint, Cloud Code 연결, Production 변경은 수행하지 않았다. 실제 로그 내용은 기록하지 않는다.
- [x] 6-3.4 Cloud Run 수집기 verification 배포 완료 (2026-10-11): 사용자가 `verification-v3` 이미지를
  업로드·배포한 뒤 CLI로 얻은 서비스 URL의 `GET /health`에서 HTTP 204를 확인했다. 앞서 포트 8080,
  최소 인스턴스 0·최대 1, LogSink 전용 서비스 계정과 고정 Secret 버전 주입을 대조했다. Cloud Code
  연결 및 Production 변경은 아직 수행하지 않았다. 실제 URL과 Secret 값은 기록하지 않는다.
  - 2026-10-10 오류 이력 (해결됨): 새 V2 이미지의 로컬 `/healthz`는 HTTP 204이나, CLI가 반환한 정확한
    Cloud Run URL과 Cloud Run proxy 모두 `/healthz`에서 Google Frontend HTTP 404를 반환했다. 두
    verification service에서 같은 현상이 재현됐다. 최신 revision/100% traffic, ingress `all`, 기본 HTTPS
    URL 활성화, 고정 Secret version 주입은 대조됐고, VPC Service Controls `HttpIngress` policy 감사 로그도
    조회되지 않았다. 위 증거만으로 GCP routing/project-level 장애 또는 VPC 차단을 확정할 수 없으며,
    이전의 해당 진단은 철회한다. `/healthz`가 Google Frontend에서 가로채지는 동일 증상의 해결 사례를
    확인해 소스·probe·단위 테스트를 `/health`로 수정했다. 수정 이미지 배포 후 `/health`가 HTTP 204를
    반환해 경로 변경으로 해결됐음을 확인했다. GCP 장애 및 사용자 설정 오류라는 이전 단정은 근거가 없었다.
- [x] 6-3.5.1 Cloud Code 안전 전송 코드·정적 검사 — AI (2026-10-20):
  `FS_LOG_SINK_URL_VERIFICATION_V1`과 `FS_LOG_SINK_HMAC_VERIFICATION_V1`을 C# host에서만 읽고,
  HTTPS Cloud Run의 고정 `/v1/events`로 HMAC 서명한 closed-schema 7개 필드만 전송하는 경계를
  추가했다. 기존 9개 Module 함수의 성공/실패 결과 뒤에만 250 ms 이내로 비차단 전송을 시도하며,
  Player·계정·요청/응답·Secret은 전송하지 않는다. Secret 값·endpoint는 저장소에 넣지 않았고,
  Node 정적 계약 및 LogSink HTTP 계약을 통과했다.
- [x] 6-3.5.2 verification Module 배포·송신 활성화 — 사용자 (2026-10-20): verification 환경의
  `FlowStateVerification` C# Module 하나를 배포하는 데 성공했다. Secret 값·endpoint·원격 응답
  본문은 기록하지 않았다. 실제 합성 이벤트 수집 및 조회 권한 확인은 6-3.6에서 수행한다.
- [x] 6-3.5 완료 — verification Cloud Code 안전 이벤트 전송 경계의 코드·정적 계약·단일 Module
  배포를 모두 대조했다. Cloud Run 수집·권한·retention의 실제 확인은 6-3.6 전까지 완료로 간주하지 않는다.
- [x] 6-3.6.1 합성 이벤트 함수·정적 검사 — AI (2026-10-20): 입력과 Player·계정 자료를 사용하지 않는
  `EmitVerificationSafeLogProbe`를 추가했다. 고정 7개 필드의 `Accepted`/`SyntheticProbe` 이벤트만
  전송하고, Module process별 60초 간격 guard가 거부하면 `RateLimited`만 반환한다. 실제 수집 점검 전에
  이 변경이 포함된 Module 재배포가 필요하다.
- [x] 6-3.6.2 verification Module 재배포·endpoint 확인 — 사용자 (2026-10-20):
  `FlowStateVerification` Module 배포에 성공했고, Module endpoint 목록에서
  `EmitVerificationSafeLogProbe`가 표시됨을 확인했다. 아직 합성 호출·Cloud Run 수집·권한 점검은
  수행하지 않았다.
- [x] 6-3.6.3 verification 합성 Probe 호출·수집 확인 — 사용자: verification 전용 Unity Service Account의
  Stateless Token으로 `EmitVerificationSafeLogProbe`를 한 번 호출한다. HTTP body는 endpoint의 JSON 계약을
  만족하는 빈 객체 `{}`만 보내며, Player·계정 이전·Secret·게임 요청 데이터는 넣지 않는다. 예상 응답은 `Accepted`이며, 바로 뒤 Cloud
  Logging에서 같은 시각의 합성 이벤트 한 건을 확인한다. `RateLimited`면 60초 뒤 한 번만 다시 호출하고,
  `Unavailable` 또는 HTTP 오류면 상태 코드와 안전한 실패 분류만 기록한다.
  - 2026-10-20 첫 호출 결과: Cloud Code endpoint 인증·호출은 완료됐고 반환 상태는 `Unavailable`이었다.
    Secret 값·endpoint·token·실제 요청/응답 본문은 기록하지 않았다. Cloud Run request 로그 유무와 HTTP
    상태를 읽기 전용으로 대조하기 전에는 재시도·Secret 회전·설정 변경을 하지 않는다.
  - 2026-10-20 보정: 첫 호출 시 Cloud Run request 로그가 없었다. 일반 게임 요청의 250 ms 송신 예산은
    유지하고, Player·계정 자료를 사용하지 않는 합성 Probe만 Secret 조회와 Cloud Run cold start를 위해
    최대 4000 ms를 쓰도록 AI가 수정했다. 이 수정은 Module 재배포 뒤에만 적용된다.
- 2026-10-20 완료 결과: 보정 Module 배포 뒤 Stateless Token으로 합성 Probe를 한 번 호출해 `Accepted`를 받았다. Cloud Run request 로그의
  `POST /v1/events`는 HTTP 202이었고, stdout에는 closed schema의 7개 필드만 기록됐다. 최소 인스턴스
  0에서 새 instance가 시작된 뒤 startup probe가 성공한 사실도 확인했다. URL·Project/환경 식별자·token·
  상관관계 값·event body는 기록하지 않았다.
- [x] 6-3.6.4-A closed schema·운영 담당자 조회 확인 — 사용자 (2026-10-20): 운영 담당자 계정이
  합성 이벤트를 조회했고, `environment`, `moduleReference`, `outcome`, `reason`, `durationBucket`,
  `correlationId`, `rateLimitResult` 외 필드가 없음을 대조했다. 비운영 담당자 조회 거부 확인은 남아 있다.
- [x] 6-3.6.4-A closed schema·운영 담당자 조회 확인 — 사용자 (2026-10-20): 운영 담당자 계정에서 Cloud Run
  request/stdout 로그를 조회해 허용된 7개 필드와 `Accepted`/`SyntheticProbe`/`Allowed`만 있음을 확인했다.
- [x] 6-3.6.4 closed schema·조회 권한 점검 완료 — 6-3.6.4-A의 운영 담당자 조회와 closed schema 대조로
  완료한다. 로그 조회 권한이 없는 별도 계정의 접근 거부 시험은 GCP IAM 기본 동작을 중복 확인하는 작업이므로
  별도 완료 조건으로 요구하지 않는다. 향후 IAM 역할 또는 그룹 구성을 변경할 때에는 해당 변경의 검토 절차에서
  최소 권한을 대조한다.
- [x] 6-3.6.5 보관·삭제 설정 재대조 — 사용자 (2026-10-20): `_Default` 로그 버킷의 보관 기간이
  30일임을 재확인했다. Logs Router에는 GCP 기본 내부 라우팅 항목인 `_Default`와 `_Required`만 있었고,
  LogSink 로그를 Cloud Storage·BigQuery·Pub/Sub·다른 로그 버킷으로 내보내거나 별도 보관하는 경로는
  없었다. 30일 경과를 기다리지 않고 retention 설정을 근거로 자동 삭제를 대조했다.
- [x] 6-3.6 완료 — 6-3.6.3~6-3.6.5가 모두 안전한 메타데이터로 통과했다.
- [x] 완료: 6-3.1~6-3.6의 설정 및 점검 결과를 안전한 메타데이터로 대조했다. Step 6을 완료 처리한다.

## Step 7. verification에서 배포·rollback 절차를 연습한다 — 사용자

### 7-1. 정적 사전점검 — AI 완료 (2026-10-20)

- `phase4-step1-static-contracts`, `phase4-step2-policy`, `phase4-step5-static-contracts`,
  `phase4-step6-log-contracts`를 통과했다. 단일 C# Module, 환경 분리, Player Write Deny,
  Secret 경계, 안전 로그 및 rollback gate의 로컬 계약은 유지된다.
- 현재 로컬 Module 후보에는 Git `a6258ef5` 뒤에 추가된 안전 로그 송신 source가 포함된다.
  `a6258ef5`와 그 이전 commit은 현재 원격 Module과의 동일성도, 이번 rehearsal의 안전한
  rollback 후보도 증명하지 않는다.
- 저장소의 `UGS/FlowStateVerification.yaml`은 내려받은 API spec artifact이며 현재
  `EmitVerificationSafeLogProbe` endpoint를 포함한다. C# 공개 함수 목록과 API spec endpoint 목록이
  일치하는지 정적 검사한다. 다음 Module 배포 뒤에도 Dashboard의 최신 API spec을 다시 내려받아
  같은 대조를 수행한다. 이 파일을 remote source 또는 rollback source로 쓰지 않는다.

### 7-1 수행 결과 — 2026-10-20

- 사용자가 현재 verification 배포 source를 Git으로 관리하고, 최종 commit SHA를 rollback 후보
  식별자로 사용하기로 했다. commit 전에는 SHA를 추정·기록하지 않는다.
- 마지막 verification 배포 뒤 Module source 변경은 없으므로, 배포 조건을 충족하는 새 배포와 실제
  rollback은 아직 수행하지 않는다.
- Online Verification에서 Leaderboard 조회 전에 격리 예약, Play, 격리 세션 준비, 원격 동의 및
  계정 `Ready` 상태를 요구하는 것은 의도된 보호 흐름이다. 이 전제 조건을 충족한 뒤 읽기 전용
  Stage/Infinite TOP·AROUND YOU 조회를 수행할 수 있다.
- 사용자가 최신 API spec을 내려받아 교체했고, `EmitVerificationSafeLogProbe`를 포함한 C# 공개
  함수와 endpoint 목록의 정적 대조를 통과했다.

### 7-2. rollback 후보 고정 — 사용자

현재 `Assets/CloudCode/FlowStateVerification.ccmr`, `UGS/Modules/FlowStateVerification`,
`UGS/CloudCode`를 포함하는 현재 source 후보를 로컬 Git commit 또는 접근 제한된 내부 archive로
고정한다. archive/commit 식별자와 보관 성공 여부만 기록한다. 이 후보는 이후 rollback 때 다시
빌드·Deploy Selected할 유일한 source가 된다.

### 7-3. 단일 Module 배포와 읽기 전용 확인 — 사용자

현재 source가 마지막 verification 배포 뒤 변경된 경우에만 `FlowStateVerification` 하나를 빌드하고
verification을 재확인한 뒤 **Deploy Selected**를 실행한다. 배포 뒤에는 안전한 격리 계정으로 계정 상태,
Stage TOP·AROUND YOU, Infinite TOP·AROUND YOU를 각각 한 번 읽기 전용으로 확인한다. 최신 API spec을
내려받아 함수 목록만 대조한다.

### 7-4. rollback 배포와 재확인 — 사용자

7-2에서 고정한 후보와 다른 source를 7-3에서 배포한 경우에만, 7-2 후보를 다시 열어
`FlowStateVerification` 하나를 빌드하고 **Deploy Selected**한다. 이어서 7-3과 같은 읽기 전용 조회를
한 번씩 수행한다. 배포/rollback 시각, 대상 환경, Module 이름, archive/commit 식별자, 결과 분류만 기록한다.

현재 source와 7-2 후보가 같다면 실제 rollback은 발생하지 않으므로 rehearsal 완료로 처리하지 않는다.

1. Step 3 방식으로 verification의 현행 대상·배포 reference·입력 정의·정책 reference를 읽기 전용으로 보관한다.
2. 변경이 있는 경우에만 `FlowStateVerification` C# Module 하나를 빌드하고, verification을 재확인한 뒤 **Deploy Selected**를 실행한다. `Deploy All`, raw JavaScript 개별 게시, Board/ledger 초기화는 하지 않는다.
3. 배포 후에는 안전한 verification 격리 계정으로 계정 상태 조회와 Stage/Infinite TOP·AROUND YOU 조회를 한 번씩 확인한다. 새 기록 제출·계정 이전·자료 삭제는 이 rehearsal에 포함하지 않는다.
4. 실패, 대상 불일치, 예상 밖 경고, 비밀 노출 우려가 있으면 새 배포를 중단한다. 같은 verification 환경의 Step 1에서 보관한 직전 확인 Module reference만 재게시한다. Cloud Save·Leaderboard·계정 데이터를 삭제하거나 다른 환경 자료로 덮어쓰지 않는다.
5. rollback 뒤 같은 읽기 전용 조회로 정상 복귀 여부를 확인하고, 배포/rollback 시각·대상·reference·안전한 결과만 기록한다.

### 완료 조건

- [ ] verification에서 한 환경·한 Module 단위의 배포와 rollback 절차가 재현된다.
- [ ] rollback이 데이터 삭제나 환경 간 복사 없이 수행됨을 확인한다.

## Step 8. Production 적용 가능 여부를 대조한다

### AI 작업

1. Step 1~7의 확인 근거가 모두 있고, Step 2의 정적 검사/Unit Test가 통과하며, 최신 코드 변경이 있으면 Unity 회귀가 통과했는지 대조한다.
2. 다음 중 하나라도 없으면 `PRODUCTION_DEPLOY_BLOCKED`로 판정한다: 대상 inventory, 복구 후보, 최소 권한 대조, Secret 권한 대조, 30일 로그/삭제 절차, 호환성 표, verification rehearsal 결과.
3. 허용된 Production 변경 단위·직전 rollback reference·담당자·중단 기준만 정리한다. Secret 값, Player 식별자, 원격 source 본문은 기록하지 않는다.

### 사용자 작업

AI의 대조 결과가 `READY_FOR_APPROVAL`일 때에만 Production 적용 권한과 변경 창을 승인한다. 승인 전에는 Step 9를 수행하지 않는다.

### 완료 조건

- [ ] Production 적용 여부가 확인 근거에 따라 `READY_FOR_APPROVAL` 또는 `PRODUCTION_DEPLOY_BLOCKED` 중 하나로 명확히 판정된다.

## Step 9. 승인된 Production 적용과 복구 가능성을 기록한다 — 사용자

이 Step은 Step 8의 `READY_FOR_APPROVAL`과 사용자의 명시적 Production 변경 승인이 모두 있을 때만 수행한다.

1. 변경 직전에 Project/Environment·Module 이름·직전 rollback reference·로그 sink 상태를 다시 확인한다.
2. 승인된 단일 배포 단위만 적용한다. 대상 불일치·버전 불명·입력 정의 차이·예상 밖 경고가 있으면 즉시 중단한다.
3. 배포 뒤에는 Dashboard가 제공하는 게시 시각/reference와 대상 Environment만 기록한다. 운영 데이터 생성, verification 자료 복사, 수동 ledger 생성, Board 초기화는 하지 않는다.
4. 오류 또는 호환성 문제가 생기면 같은 Production 환경의 직전 확인 reference로 rollback하고, 데이터 삭제·환경 간 덮어쓰기·Secret 노출 없이 Step 7과 같은 읽기 전용 확인을 수행한다.
5. Production에서 Player 기능을 실제로 실행하는 통합 검증과 Windows Player Build는 Phase 5에서 별도 승인 뒤 수행한다.

### 완료 조건

- [ ] 승인된 단일 Production 변경이 대상·reference·복구 절차와 함께 기록되거나, 중단/rollback 결과가 안전하게 기록된다.

## Step 10. Phase 4 완료를 판정하고 Phase 5로 인계한다

다음을 모두 만족할 때만 완료 처리한다.

- [ ] verification과 Production의 환경·계정·제출·조회·Local Save/Pending 경로가 섞이지 않음을 정적 검사·설정 inventory·Unit Test로 확인했다.
- [ ] Player 직접 쓰기 차단, Cloud Code Secret 최소 접근, Server 검증 경계가 대상 설정과 verification 실제 거부 검사로 대조됐다.
- [ ] 비밀값 없는 구조화 로그, 30일 보존, 삭제 절차, 1인 수동 장애 대응 및 테스트 자료 관리가 준비됐다.
- [ ] 배포 전 backup, 단일 배포 단위, rollback reference, verification rehearsal, Production 승인 gate가 재현 가능하다.
- [ ] Client/Module/Local Save 호환성 표와 데이터 보존·rollback 금지 조합이 문서·Unit Test로 대조됐다.
- [ ] 최신 변경이 있으면 Unity Script Compilation, 전체 EditMode/PlayMode Test를 사용자가 통과시켰다.

Phase 5 인계 항목은 Windows x64 1920×1080 Player Build, Keyboard/Mouse UI, 승인된 운영 대상의 신규/기존 계정 및 Offline 복구, 운영 rollback 후 데이터 보존이다. verification 결과만으로 Production 검증 완료 또는 일반 사용자 공개를 선언하지 않는다.

## AI와 사용자의 역할 분리

| 영역 | AI | 사용자 |
| --- | --- | --- |
| 코드·문서·정책 artifact | 정적 검사와 Unit Test 작성/실행, 절차 작성 | 결과 검토 |
| Unity | 실행하지 않음 | Compilation, Test Runner, Build 실행 |
| Scene | 수정하지 않음 | 필요할 때만 직접 적용 |
| UGS/외부 로그 sink | 실제 인증·조회·게시·Secret 열람을 하지 않음 | Dashboard/CLI/sink 설정·배포·rollback 수행 |
| 민감 정보 | 노출·저장·요청하지 않음 | 승인된 내부 보관소에서만 관리 |

## 관련 문서

- [Prototype 8 Roadmap](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md)
- [Phase 3 Manual Steps](20261006_08_Phase3ManualSteps.md)
- [Verification Deployment](../../../UGS/VERIFICATION_DEPLOYMENT.md)
- [Transfer Operations Runbook](../../../UGS/TRANSFER_OPERATIONS_RUNBOOK.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
