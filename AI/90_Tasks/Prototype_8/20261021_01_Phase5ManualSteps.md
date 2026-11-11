# Prototype 8 - Phase 5 통합 검증과 공개 운영 후보 판정

## 작업 정보

- 작업일: 2026-10-21
- 범위: Flow State / Prototype 8 / Phase 5
- 담당: AI(정적 대조·Unit Test·도구/절차 준비), 사용자(Unity 컴파일/Test Runner/Build·실제 서비스 요청·화면 조작)
- 선행: [Phase 4 Manual Steps](20261007_09_Phase4ManualSteps.md) Step 10 완료.
- 기준 source: Git `8774b2db2defa76c7e6a4ddd73f0f37f187c7cee`.
- 상태: **Step 1~13·Phase 5 완료(승인 예외/잔여 한계 포함) / Candidate01 공개 운영 후보 판정, 일반 사용자 공개 미승인 (2026-11-11)**.

## 작업 목적과 작업 전 상태

실제로 빌드한 Windows Player에서 계정·기록·Leaderboard·Offline 복구·UI를 확인하고,
Production 서비스와 연결한 공개 운영 후보의 근거를 모은다. 테스트로 판정 가능한 로직을 사람이 반복 확인하지 않는다.

Phase 4에서 환경 중립 Module, 별도 Production Secret/보드/Cloud Run LogSink, 30일 보관 정책과
운영 조회 권한을 준비했고 사용자가 production의 `FlowStateVerification` Module 하나를 배포했다.
verification 계정/순위/합성 로그 수집·Unity 컴파일/전체 테스트 성공은 기존 근거로 보존한다.
최초 계획 작성 시점에는 다음 항목을 아직 확인하지 않았다. 현재 결과는 아래 Step별 최신 기록·Step 13을 따른다.

- Production Module의 실제 호출·Secret 읽기·서명 로그 수집 성공.
- Production Windows Player Build 및 그 Player의 계정·제출·조회·Offline 복구.
- 실제 배포 복구 후 데이터 보존. 첫 배포 이전 Production Module은 존재하지 않는다.
- 격리 Editor 실행에서 Continue 후 Online Record Notice가 반복되는 UI의 개선 또는 수용 여부.

문서 작성은 위 작업의 실행이나 일반 사용자 공개 승인이 아니다.

## 조사 내용과 적용 경계

- Module reference는 `Assets/CloudCode/FlowStateVerification.ccmr`이며 C# host와 embedded JS를 함께 배포한다.
- `OnlineRecordConfiguration`의 `FLOW_STATE_PRODUCTION` 분기가 Player의 환경을 정한다.
  Deployment 창의 환경·GCP 프로젝트 선택은 Player의 환경 선택을 대신하지 않는다.
- Production 기본 인증 프로필은 승인된 Production 설정을 따른다. `--fs-verification-session=A/B/Legacy/Phase3New`는
  verification 전용이며 Production 새 계정 생성용 인수로 사용하지 않는다.
- 기존 `Invoke-VerificationSafeLogProbe.ps1`, Online Verification A/B 창, live storage probe는 verification 전용이다.
  이 도구를 수정해 Production 대상에 바로 실행하는 절차는 제공하지 않는다.
- Windows에서는 실행 파일 폴더만 복사해도 Local Save·Authentication 저장이 분리된다고 보장할 수 없다.
  새 계정/두 기기 검증은 전용 테스트 기기·OS 사용자 등 실제로 격리된 실행 공간을 확보한다.
- 실제 식별자·Secret/인증 키·token·Player ID·이전 코드/인증값·원시 저장/로그는 Task·Git·채팅에 기록하지 않는다.
  번호·기준 자료는 필요한 경우 로컬 비공개 비교에만 사용하고 결과만 보고한다.

## 검증 책임 분리

| 대상 | AI가 먼저 수행할 정적 검사·Unit Test | 사용자에게 남기는 실제 확인 |
| --- | --- | --- |
| 환경/계정 격리 | Build Profile define·승인 설정·프로필 allowlist·Local Save/Pending scope·오환경 거부 | 승인한 Player와 QA 계정으로 실행 |
| 계정 준비/이전 | 동의 저장 실패·인증/서버 timeout·이전 만료/취소/재발급·CAS·A/B 연결·stale 응답·최고 기록 보존 | 실제 두 실행 공간에서 번호 표시·한 번의 승인된 이전·재시작 |
| 기록 제출/조회 | 점수 검증·멱등성·응답 유실·동일 제출 ID·TOP 10/AROUND 7·동점·100행 초과·조회 오류 상태 | 실제 플레이로 두 Mode 제출 및 TOP/AROUND YOU 표시 |
| Offline/Pending | v7 migration·180일 경계·가짜 시계·저장 실패·재시작·중복 제거 | 네트워크 단절 상태의 Player 재시작과 복구 |
| 로그 | 7필드·HMAC/시각·오류 비노출·고정 대상·timeout·키 분리 | 승인된 합성 호출 1건과 실제 Cloud Logging 저장 |
| UI | 실제 Scene 참조 정적 대조·상태 전환·입력 잔류·중복 클릭·모달/해제·해상도별 레이아웃 검증 가능 부분 | 실제 Player의 가독성·겹침·Keyboard/Mouse 동작 |
| 복구 | 이전 reference·호환성·migration 금지 조합·자료 전후 비교기 | 기존 배포가 있는 경우에만 승인된 실제 복구 배포 |

시간 경계에 맞춘 빠른 클릭, 180일 대기/PC 시계 변경, 수백 번 수동 제출, 직접 저장 JSON 변조는 수동 확인 항목이 아니다.
Unit Test는 생산 코드/서버 대역·합성 저장을 사용하며 실제 서비스 계정·Production 데이터를 호출하지 않는다.

## 진행 순서

아래 번호로 한 항목씩 요청한다. 예: `Phase 5 Step 1 수행` 또는 `Step 7-1 수행 방법을 알려줘`.
체크는 실제 결과를 받은 뒤 갱신한다. 도구가 아직 없거나 승인 범위가 불명확하면 해당 작업의 착수 조건부터 해결한다.

| Step | 작업 | 담당 | 실제 서비스 변경 |
| --- | --- | --- | --- |
| 1 | 대상·QA 자료·중단 기준·실행 승인 확정 | 사용자 + AI | 없음 |
| 2 | 정적 검사·Unit Test·점검 도구·알려진 UI 문제 준비 | AI, 필요 시 사용자 Scene 작업 | 아직 없음 |
| 3 | Unity 컴파일·전체 EditMode/PlayMode 결과 확정 | 사용자 | 없음 |
| 4 | 승인된 Windows Player 빌드 | 사용자 | 없음 |
| 5 | Production 합성 로그 실제 연결 확인 | 사용자 | 합성 로그 1건 |
| 6 | 신규 Production QA 계정 준비 | 사용자 | 승인된 QA 계정/서버 자료 생성 |
| 7 | Stage/Infinite 실제 제출·순위 조회 | 사용자 | 승인된 QA 기록 제출 |
| 8 | Offline Pending·종료/재시작·복구 | 사용자 | QA Pending 재전송 |
| 9 | 기존 QA 계정 재실행·자료 보존 | 사용자 + AI 비교 | 상태 조회에 따른 정상 서버 갱신 가능 |
| 10 | QA A→B 계정 이전·이전 후 보존 | 사용자 + AI 비교 | 두 QA 계정의 연결 변경 |
| 11 | Player Keyboard/Mouse·화면 최종 확인 | 사용자 | 이미 승인된 확인 범위만 |
| 12 | 배포 복구와 자료 보존 검증 | 사용자 + AI | 별도 승인된 Module 재배포만 |
| 13 | 완료 근거·남은 위험·공개 여부 판정 | AI + 사용자 | 별도 공개 승인 전 없음 |

## Step 1. 검증 대상·실행 공간과 승인을 확정한다

### 1-1 AI: 기존 근거·로컬 대상 대조

- Phase 4의 배포 SHA·source 이후 변경·API 계약·Production 설정과 Build Profile을 읽기 전용으로 대조한다.
- 기존 설정·보관 기간·보드·검증 결과를 다시 만들거나 전부 재시험시키지 않는다. 변경된 항목만 영향 범위를 정한다.
- 대상이 바뀌었으면 사용자에게 해당 차이만 보고한다. 비밀값은 읽거나 출력하지 않는다.

#### 1-1 수행 결과 — 2026-10-21, AI 완료

- 기준 배포 SHA `8774b2db2defa76c7e6a4ddd73f0f37f187c7cee`와 현재 HEAD `46f4c20182f68aa31b76ff927e7a18401450344a`를 읽기 전용으로 대조했다.
  기준 이후 `Assets`의 Client/Scene/Module manifest와 `UGS`의 실행 source에는 차이가 없고, 변경은 Unity Services Editor 배포 환경 설정 1개와
  Production inventory·환경 구성·이전 운영·배포 기록 문서다. 따라서 이 차이만으로 Unity 컴파일·Test Runner·Player Build·원격 재배포를 반복하지 않는다.
- `FlowStateVerification`의 C# host는 10개 Cloud Code 함수 선언을 유지하고, 제공 OpenAPI의 10개 POST 경로와 이름을 대조했다.
  원격 artifact hash·게시 reference는 로컬에서 확인할 수 없으므로 동일하다고 추정하지 않는다.
- `Windows-production` Build Profile에 Windows 대상과 `FLOW_STATE_PRODUCTION` define이 있으며 Development build가 꺼져 있음을 정적으로 확인했다.
  Git 미추적 Production 설정 파일은 `EnvironmentName`·Project/Environment ID·Authentication Profile의 비어 있지 않은 값을 가진다.
  값·식별자는 읽거나 기록하지 않았다. 실제 Windows x64/1920×1080 Player Build 적합성은 Step 4의 사용자 Build에서 확인한다.
- `ProjectSettings/Packages/com.unity.services.core/Settings.json`의 환경 선택은 기준 SHA 이후 production으로 바뀌었다.
  이는 Editor/배포 환경 설정이며 Player의 환경 선택을 대신하지 않는다. Player는 Build Profile define과 비공개 Production 설정으로 고정된다는
  Phase 4 계약을 유지한다.
- Unity Editor·Unity Test Runner·Scene·원격 UGS/GCP 요청·Secret 열람은 수행하지 않았다. 작업 기록을 반영하기 전 작업 트리는 깨끗했으며,
  현재 변경은 이 Task 기록뿐이다.

판정: **Step 1-1 완료.** Step 1 전체의 완료 여부는 1-2의 QA 실행 공간 지정과 1-3의 실제 쓰기 범위·단일 운영자·중단 기준에 대한
사용자 승인으로 판정한다.

### 1-2 사용자: QA A/B 실행 공간 지정

1. 일반 사용자 자료가 없는 전용 테스트 실행 공간 A를 지정한다.
2. 계정 이전에는 별도 인증 상태를 가진 실행 공간 B도 지정한다. B는 Step 10까지 실행을 미뤄도 된다.
3. 기존 사용자 Local Save/인증을 지워 새 계정을 만드는 대신 격리된 공간을 사용한다.
4. 사용할 환경이 production인지, 계정 별칭 A/B와 실행할 빌드가 무엇인지만 보고한다. 실제 번호·경로는 공유하지 않는다.

#### 1-2 수행 결과 — 2026-10-22, 사용자 완료

- production QA 실행 공간을 서로 다른 전용 로컬 Windows **표준 사용자** 계정으로 지정했다.
- 이전 전 기준 계정의 실행 공간 별칭은 **Production QA 출발**, 이전 후 수신·보존 확인용 실행 공간 별칭은
  **Production QA 수신**이다. 이후 문서·결과 보고는 A/B 대신 이 역할 별칭을 사용한다.
- 두 실행 공간은 인증 상태와 Local Save를 공유하지 않는다. 기존 일반 사용자 자료를 삭제하거나 재사용하지 않았다.
- 실제 Windows 계정 이름·프로필 경로·공개 번호·인증값은 기록하지 않는다. Production Player 실행과 계정 생성은
  Step 1-3 승인 및 후속 착수 조건이 충족될 때까지 수행하지 않는다.

판정: **Step 1-2 완료.**

### 1-3 사용자: 실제 변경 범위 승인

다음 중 승인한 범위를 명시한다. Phase 4의 Module 배포 승인이 아래 실행을 자동 승인하지 않는다.

- Production 합성 로그 점검 1건과 필요한 최소 권한의 점검용 인증 사용.
- QA A/B 최대 두 실행 공간의 익명 계정·서버 자료 생성, 각 Step의 정상 플레이 제출/조회.
- Offline QA Pending 재전송과 QA A→B 이전.
- 기존 자료 보존, 실패 시 추가 제출/이전 중단, 운영자 1인 담당. 실행 시간대는 별도 관리하지 않는다.
- 실제 Module 복구 배포는 Step 12에서 후보·변경 창을 별도로 승인한다.

QA 자료는 정리 승인이 있기 전까지 남겨 둔다. 확인 종료를 이유로 서버 자료나 계정을 자동 삭제하지 않는다.

#### 1-3 수행 결과 — 2026-10-22, 사용자 승인

- 사용자는 다음 실제 변경 범위를 모두 승인했다: Production 합성 로그 점검 1건, **Production QA 출발** 및
  **Production QA 수신**의 익명 계정·QA 서버 자료 생성, 정상 플레이 제출·조회, Offline Pending 재전송,
  출발→수신 계정 이전.
- 기존 자료와 QA 자료는 보존하며, 실패·대상 불일치·timeout 또는 응답 미확인 시 추가 제출·이전을 중단한다.
  QA 자료·계정의 삭제는 별도 사용자 승인 전까지 수행하지 않는다.
- 운영자는 사용자 1인이다. 사용자 요청으로 실행 시간대는 별도 승인·기록·관리하지 않도록 Step 1 완료 기준을 변경했다.
- 실제 Module 복구 배포는 이 승인의 범위 밖이며 Step 12에서 후보·변경 창을 별도로 승인한다.

판정: **Step 1-3 완료.**

### Step 1 완료 판정 — 2026-10-22

- [x] 1-1 배포 기준·source 차이·API 계약·Production 설정과 Build Profile의 읽기 전용 대조 완료.
- [x] 1-2 Production QA 출발/수신의 분리된 실행 공간·인증·Local Save 비공유 확인 완료.
- [x] 1-3 허용된 실제 쓰기 범위·단일 운영자·중단 기준 기록 완료.

**Step 1 완료.** 다음 작업은 Step 2의 정적 검사·자동 검증 대조·Production 점검 도구 준비이며,
이 단계에서도 Unity Test Runner·Player Build·Scene 작업·원격 Production 요청은 수행하지 않는다.

완료 조건: [x] 대상·QA 격리 방법·승인된 쓰기 범위·단일 운영자·중단 기준 기록. 실행 시간대는 사용자 승인으로 별도 관리하지 않는다.

## Step 2. 수동 확인 전에 정적 검사·Unit Test·도구를 준비한다

### 2-1 AI: 기존 자동 검사를 대조하고 부족한 사례만 보강

- Node 검사와 로컬 LogSink 검사를 재사용한다. Phase 4 최종 기준은 Node 52파일 통과이며 이후 수치는 실제 실행 결과로 갱신한다.
- `OnlineRecordConfigurationTests`, `OnlineLocalSaveScopeTests`, `OnlineRecordRepositoryTests`,
  `OnlineAccountCoordinatorTests`, `AccountTransferControllerTests`, `AccountTransferCompletionTests`,
  `LeaderboardViewStateTests` 및 격리 PlayMode 테스트를 책임 표와 대조한다.
- 테스트 자료만 정상인 별도 로직을 만들지 않고 실제 coordinator/store/codec/transport·Scene 참조를 검증한다.
- 결과 미확인·저장 실패·취소·중복·동시 요청은 대역/가짜 시계로 판정한다. 실제 사용자에게 정밀한 버튼 타이밍을 요구하지 않는다.

### 2-2 AI: Production 점검 도구와 비공개 자료 비교 준비

- 계획 작성 당시 Production 전용 수동 LogSink 점검 도구는 준비되지 않았다. 2026-10-22 아래 수행 결과의 별도 도구를 준비했다.
  승인한 Project/Environment·허용 endpoint·인증 방식·합성 7필드를 확정하고 별도 도구를 준비한 뒤 로컬 대역으로 검사한다.
- verification 전용 도구는 고정 대상을 유지한다. Production 도구는 대상 불일치 거부, 명시적 실행 옵션,
  숨김 credential 입력·정리, 자동 재시도 없음, 오류 원문/Secret 비출력을 Unit Test로 확인한다.
- 계정/최고 기록·Pending 비교는 기존 도구의 적용 범위를 확인하고, 지원되지 않으면 Production 자료를
  기존 verification 도구에 넣지 않는다. 필요한 비교기를 AI가 준비해 로컬 비공개 기준 자료를 자동 비교한다.
- 공개 번호·점수·수락 시각·제출 ID·귀속 등은 원문을 채팅으로 받아 대조하지 않고 동일 여부·차이 분류만 출력한다.
- 도구가 없거나 안전한 인증/대상 대조가 안 되면 Step 5 및 해당 비교 Step은 **준비 대기**다.

### 2-3 AI + 사용자: 격리 Notice UI의 개선 또는 수용 결정

- Phase 4의 `IsolationArmed=True` Notice 반복을 Unit Test로 재현하고 일반 Play 흐름과 분리한다.
- AI는 수정안/회귀 검사를 준비하고 필요 시 사용자에게 개선 범위 승인을 받는다. Scene을 직접 수정하지 않는다.
- Scene 변경이 실제 필요한 경우에만 연결 대상·Inspector 항목·확인법을 작성하고 사용자가 적용한다.
  참조 여부는 AI 정적/자동 검사로 확인한다.
- 개선을 보류한다면 Editor 격리 도구에 한정된 제한이라는 근거와 사용자의 수용 결정을 기록한다.
  일반 Player에도 같은 문제가 나타나면 필수 실패로 분류한다.

완료 조건: [x] 필요한 정적/Node 검사 통과·Unity 테스트 준비·Production 도구 준비·알려진 문제 처리 기준 기록.
이 Step은 준비이며 AI가 Unity Test Runner·원격 요청을 실행하지 않는다.

### Step 2 세부 수행 결과 — 2026-10-22

#### 2-1-1 자동 검증 책임 대조 — 완료

| 책임 | 기존 실제 생산 코드 기반 자동 검증 |
| --- | --- |
| 환경/저장 scope·오환경 거부 | OnlineRecordConfigurationTests, OnlineLocalSaveScopeTests, phase4-step2-policy.test.cjs |
| 동의 저장 실패·인증/서버 timeout·늦은 응답 | OnlineRecordRepositoryTests, OnlineAccountCoordinatorTests |
| 이전 취소/만료·중복·CAS/응답 유실·최고 보존 | AccountTransferControllerTests, AccountTransferCompletionTests, transfer-lifecycle/transfer-races/transfer-completion Node 검사 |
| TOP 10/AROUND 7·동점/전역 rank·100행 초과 | LeaderboardViewStateTests, leaderboard-query-policy 및 record-account-integration Node 검사 |
| v7 migration·180일·Pending ID/귀속·저장 실패 | OnlineLocalSaveScopeTests, OnlineRecordRepositoryTests, submission-retention-policy 및 receipt-journal Node 검사 |
| Scene 참조·입력/모달·검증 격리 | ModeUISceneConfigurationTests, AccountTransferUIIsolationTests, VerificationToolIsolationTests, PlayModeRecordIsolationTests |

위 C# 테스트는 존재·source 책임을 대조했으며 이번 AI 실행의 통과 개수로 사용하지 않는다.

#### 2-1-2 로컬 Node·LogSink·정적 검사 — 완료

- `UGS/Tests` 독립 실행 Node **53파일 통과, 실패 0**(기존 52 + production-tools.test.cjs 1, helper 2개 제외).
- `node --test UGS/LogSink/test.cjs` 통과: 합성 HMAC/시각·7필드·크기·route·로컬 HTTP 경계.
- Production 전용 Node 테스트 **9개 통과**: 대상 불일치/확인 옵션/고정 endpoint/무재시도/오류 비출력/로컬 자료 보존·차이·export 조립.
- PowerShell wrapper 대역 검사 통과: 숨김 credential 입력과 성공·실패 뒤 이전 환경 복원 확인.
- `client-unity-preflight.cjs` 통과: 변경 C# 2파일 source/메타/asmdef/격리 경계.
  출력 Edit 183/Play 23은 해당 도구가 집계하는 기존 일부 fixture의 준비 사례이며 전체 Unity 실행 수가 아니다.
- Phase 3 technical contract 정적 검사 통과. Unity compile/Test Runner·.NET Module 실행·원격 서비스 결과와 구분한다.

#### 2-2-1 Production 합성 로그 도구 — 준비 완료

- `production-scope.cjs`, `production-safe-log-probe.cjs`, `Invoke-ProductionSafeLogProbe.ps1`를 별도로 추가했다.
  verification 도구의 고정 대상은 변경하지 않았다. 공통 요청/안전한 응답 해석을 재사용하고 두 고정 URL만 Production scope로 매핑한다.
- 로컬 비공개 Production 설정·Editor 환경·Cloud Project를 대조하며 값은 출력하지 않는다.
  `--plan`을 실제 로컬 실행하여 `ScopeMatched / NoRemoteRequests`를 확인했다.
- 명시 실행 옵션·숨김 credential 입력/정리·고정 함수 1회·자동 재시도 금지·Accepted/Completed/None/202 판정을 준비했다.
  Production 실제 권한·Secret 읽기·수집 성공은 Step 5에서 사용자 확인한다.
- 구체적인 인증 준비·실행 명령·실패 보고 방법은 [Production QA 도구](../../../UGS/Verification/PRODUCTION_QA_TOOLS.md)에 기록했다.

#### 2-2-2 비공개 저장·서버 자료 비교기 — 준비 완료

- `capture-production-snapshot.cjs`와 `compare-production-snapshots.cjs`를 추가했다.
  실제 Local Save v7 복제와 Dashboard 단일 계정/행 export를 로컬에서 검증·조립하며 기존 기준은 덮어쓰지 않는다.
- Production QA 출발/수신의 Pending 보존/제거, 재시작, 서버 행/metadata·연결 이전 보존을 고정 결과 분류로만 출력한다.
  캡처 파일은 Git 제외 `Ignore/Phase5Private`에 보관하며 인증 저장소·실제 저장 파일을 수정하지 않는다.
- `REMOVED_ONLY`는 제출 성공 판정이 아니다. 실제 Submitted/Rejected와 서버 행 확인을 함께 사용한다.
  실제 legacy/export가 없거나 미지원이면 미검증/N/A 근거를 유지한다.

#### 2-3-1 격리 Notice 원인·수정안 — 준비 완료

- `GameSystem.InitializeLocalState`의 격리 경로는 메모리 저장소만 만들고 온라인 coordinator 생성을 건너뛴다.
  기존 `ConfirmOnlineRecoveryNoticeAsync`는 coordinator가 null이면 false라서 Continue 뒤 동의 상태가 남지 않았다.
- 격리 초기화 여부를 instance에 보관하고, 해당 경로에서만 기존 메모리 repository에 확인 상태를 저장하도록 변경했다.
  Player ID·공개 번호·인증 요청은 만들지 않는다. 일반 Player의 coordinator 경로는 유지한다.
- `PlayModeRecordIsolationTests`를 보강했다: 자동 테스트 격리와 Editor 예약 격리 두 경우의 동의 저장·중복 확인·메모리 저장·온라인 객체 부재,
  일반 미초기화 경로의 동의 실패. 이 테스트는 기존 코드에서 실패하는 확인 조건을 포함한다.
- Scene/Inspector 참조 변경은 필요 없으며 기존 Continue 버튼 연결을 사용한다.
  일반 Player의 실제 Notice/UI 확인은 Step 11, Unity 컴파일·전체 테스트는 Step 3에서 사용자가 수행한다.

### Step 2 완료 판정·다음 수동 작업

**Step 2 준비 완료.** 로컬 검사와 회귀 테스트 준비를 완료했으며 Unity 컴파일·Test Runner 성공을 의미하지 않는다.
실제 Production 요청·credential 발급·QA 계정 생성·Player Build는 아직 수행하지 않았다.

다음은 **Step 3**이다. 사용자는 Unity 컴파일 완료 후 예상 밖 Error/Warning을 확인하고, 실서비스 호출이 없는
격리된 전체 EditMode·PlayMode를 실행하여 각 실행 수/실패 수/예상 밖 Error·Warning 여부를 보고한다.
기존 자동 테스트 기준인 기본 Windows(verification) profile에서 수행하고 Production Player profile 선택은 Step 4에서 한다.
실패 시 이름·안전한 메시지만 먼저 전달한다. Scene 수동 수정이나 Module 재배포는 이번 변경에 필요하지 않다.

## Step 3. Unity 컴파일·전체 EditMode/PlayMode 결과를 확정한다

사용자 수행:

1. Step 2에서 승인된 Client/Scene 변경을 적용했다면 Unity 컴파일이 끝날 때까지 기다린다.
2. Console의 예상 밖 Error/Warning을 확인한다.
3. Test Runner에서 전체 EditMode와 전체 PlayMode를 각각 실행한다. 실서비스 호출이 없는 격리 테스트 구성을 사용한다.
4. 실행 개수·실패 수·예상 밖 오류/경고 여부를 보고한다. 실패 시 이름·안전한 메시지를 먼저 전달한다.

변경이 없고 Phase 4 이후 같은 source의 결과를 재사용할 수 있으면 AI가 그 근거를 연결하며,
단순 문서 변경·Git commit 때문에 다시 실행시키지 않는다. 기존 개수 미보고를 이유로 숫자를 추정하지 않는다.

### Step 3 수행 결과 — 2026-10-22, 사용자 확인 완료

- Unity Script Compilation 성공, 예상 밖 Error/Warning 없음.
- 전체 EditMode **890/890 성공**, 예상 밖 Test Error/Warning 없음.
- 전체 PlayMode **256/256 성공**, 예상 밖 Test Error/Warning 없음.
- 결과는 Step 2 이후 현행 작업 트리에 연결한다. HEAD는 `46f4c20182f68aa31b76ff927e7a18401450344a`이며,
  변경된 C#의 SHA-256은 다음과 같다. 단순 문서 갱신으로 재실행이나 추가 commit을 요구하지 않는다.
  - GameSystem.cs: `D6510EDADB8EE71DB82CEBA16391289C3FD5C79D03DC339BF72DD8C0D2D69444`
  - PlayModeRecordIsolationTests.cs: `9B29E663010EF4E635B6E54899C0BD3B8CCCBEC756373B1D4D81E90E2F042168`
- `PlayModeRecordIsolationTests`의 두 격리 경로·중복 확인·온라인 객체 부재 검증이 포함된 전체 PlayMode 성공 보고로
  **Step 2-3의 격리 Notice 수정 자동 회귀 검증을 완료**한다. 실제 Player 화면 확인은 Step 11에 남긴다.
- 배포 기준 `8774b2db2defa76c7e6a4ddd73f0f37f187c7cee`부터 현재 작업 트리까지 `UGS/Modules`,
  `UGS/CloudCode`, `Assets/CloudCode`의 tracked 차이와 미추적 신규 파일이 모두 없다.
  배포 Module 변경이 없으므로 재배포·추가 Git commit은 요구하지 않는다. 원격 artifact hash 동일성은 추정하지 않는다.
- Unity Services Editor 환경 설정은 사용자 변경으로 보존한다. Player 환경 선택은 별도의 Production Build Profile로 확인한다.

**Step 3 완료.** 다음은 Step 4의 AI 읽기 전용 Build 사전 검사와 사용자 Windows Player Build다.

완료 조건: [x] 빌드 후보에 필요한 컴파일/전체 Unity 테스트 결과가 source와 연결됨.

## Step 4. Windows Player를 빌드하고 실행 정보를 기록한다

착수 조건: Step 1~3 완료. 검증된 source를 HEAD와 작업 트리 변경 파일의 hash로 연결한다.
추가 코드 변경이 생기면 영향 범위와 필요한 검증을 갱신하고, Module source가 바뀐 경우에만 필요한 배포 절차를 별도로 다룬다.
Module 변경이 없는 현재 후보는 추가 Git commit·Module 재배포를 선행 조건으로 요구하지 않는다.

### 4-1 AI 사전 검사

Production Build Profile의 `FLOW_STATE_PRODUCTION`, 승인된 Production 설정, Scene 목록/참조,
온라인 Local Save scope, 로그의 비밀값 비노출 및 test-only 경로 제외를 정적으로 확인한다.
식별자를 보여 주기 위한 Scene·UI를 사용자에게 새로 만들도록 요구하지 않는다.

#### 4-1-1 Profile·Production 설정·화면 기본값 — 완료 (2026-10-22)

- Unity `6000.3.5f2`와 `Windows-production.asset`를 읽기 전용 대조했다.
  BuildTarget 19=StandaloneWindows64, Subtarget 2=Player, Architecture 0=x64임을 설치된 Unity DLL의
  enum 메타데이터로 확인했다. 메타데이터만 읽었으며 Unity Editor·빌드·컴파일러를 실행하지 않았다.
- `FLOW_STATE_PRODUCTION`은 Production profile에만 있으며 전역 Standalone define에는 없다.
  Development/Profiler/Script Debugging은 꺼져 있다. Profile은 전역 Player Settings와 Scene 목록을 상속한다.
- 전역 기본 화면은 1920×1080, fullscreenMode 3=Windowed다. 현재 profile에는 별도 Player Settings override가 없다.
  Settings의 Fullscreen 선택은 runtime에서 화면 모드를 바꿀 수 있으므로 실제 창 상태는 후속 Player 실행 때 확인한다.
- Git 제외 Production 설정의 환경 이름·인증 profile·Project 연결·verification과 다른 환경 조합을 비노출 대조해 통과했다.
  현재 Editor 환경 selector는 verification이다. Player는 profile define과 명시적 `SetEnvironmentName`을 사용하므로
  이 selector를 Production Player 빌드 전환용으로 변경할 필요는 없다.

#### 4-1-2 Scene·저장 scope·테스트 경로·로그 — 완료 (2026-10-22)

- 활성 Build Scene은 `Assets/Scenes/SampleScene.unity` 하나이며 meta GUID가 일치한다.
  GameSystem 필수 참조 14개와 Notice 참조 4개가 Scene 내 실제 fileID를 가리킴을 정적으로 확인했다.
  Scene/Inspector 변경은 필요 없다. 참조의 실행 동작은 Step 3 전체 PlayMode 성공 근거를 연결한다.
- 테스트 asmdef는 TestAssemblies로 표시되고 EditMode는 Editor로 제한된다. Scene에 Editor/Test script 참조가 없고
  별도 `Assets/StreamingAssets` payload도 없다. 일반 Build를 사용하며 Test Runner의 테스트용 Player 빌드는 사용하지 않는다.
- GameSystem의 Editor 격리 메모리 저장·확인 상태 분기는 `UNITY_EDITOR`에 한정된다.
  Player는 사용자별 persistent 저장과 `(Project ID, Environment ID)` scope, 승인 Production 인증 profile을 사용한다.
  기존 `--fs-verification-session=...` 인수 파서는 존재하지만 Production QA 실행에 사용하지 않으며
  인증 profile allowlist는 verification profile의 원격 인증을 거부한다.
- 인증/transport 진단은 고정 분류·숫자 코드이며 raw 예외 메시지·token을 출력하지 않는다.
  기존 파일 I/O 실패 경고는 예외 메시지에 로컬 파일 경로가 포함될 수 있으므로 Player.log 전체를 채팅에 공유하지 않는다.

#### 4-1-3 검증 source·사전 검사 결과 — 완료 (2026-10-22)

- `node UGS/Verification/check-phase5-build.cjs` 통과. 위 설정·참조·저장/로그/격리 경계를 읽기 전용으로 확인하며
  식별자·Secret·설정값 원문을 출력하지 않는다.
- GameSystem SHA-256이 Step 3의 `D6510EDA...D69444`와 같음을 확인했다.
  사용자 컴파일 성공·EditMode 890/890·PlayMode 256/256·예상 밖 Error/Warning 없음 근거를 유지한다.
- 배포 기준 이후 Module host·embedded JS·manifest 변경이 없다. 이번 작업은 정적 검사 도구와 문서만 추가/갱신했다.
  추가 Git commit·Unity 전체 테스트 반복·Module 재배포를 요구하지 않는다.

**Step 4-1 완료.** Step 4 전체는 다음 사용자 실제 Build 결과를 받은 뒤 판정한다.

### 4-2 사용자 실제 빌드

1. Unity **File → Build Profiles**에서 기존 **Windows-production**을 선택하고 활성화한다.
   profile 전환에 따른 컴파일이 끝나기를 기다리고 예상 밖 Error/Warning이 없는지 확인한다.
2. Windows x64 Player·1920×1080 Windowed·Development Off·SampleScene은 4-1에서 이미 확인했다.
   같은 설정을 다시 만들거나 Scene을 수정할 필요 없다. 다른 값을 발견했다면 Build 전에 차이만 보고한다.
3. 기존 배포물을 덮어쓰지 않는 빈 전용 출력 폴더를 선택한다. 예: 로컬 `Builds/ProductionQA/Candidate01`.
   폴더의 실제 절대 경로는 채팅/Task에 남기지 않고 출력물 별칭 **Production-QA-Build-01**을 사용한다.
4. **Build**를 실행한다. **Build And Run**은 사용하지 않는다. 완료 후 Player를 실행하지 않고 파일을 보관한다.
   실제 QA 실행은 Step 5의 Production 합성 로그 연결 확인 후 Step 6에서 Production QA 출발로 진행한다.
5. Build 성공/실패, 사용 profile `Windows-production`, 출력물 별칭, 예상 밖 Error/Warning 여부만 보고한다.
   source는 Step 3의 HEAD+변경 파일 hash와 연결하며 별도 commit을 요구하지 않는다.

4-2 상태: **사용자 실제 Build 완료 보고 수신(2026-10-22)**. 시간대 별도 관리는 Step 1 승인대로 요구하지 않는다.
같은 저장 source로 빌드했다면 추가 Test Runner 재실행은 필요 없다.

### Step 4 수행 결과 — 2026-10-22, 완료

- 사용자가 Step 4의 Build 완료를 보고했다. 앞선 `Windows-production`의 `FLOW_STATE_PRODUCTION` 설정 확인과
  4-1 사전 검사·Step 3 검증 source에 연결하여 Production Windows Player 빌드 완료로 기록한다.
- 출력물은 이 절차의 관리 별칭 **Production-QA-Build-01**로 지칭한다. 실제 출력 경로와 artifact hash는 제공받지 않았으며 추정하지 않는다.
- Build 단계의 예상 밖 Error/Warning 유무는 별도 보고되지 않았다. Step 3의 컴파일/테스트 무오류 보고와 혼동하지 않는다.
- 1920×1080 Windowed는 정적 기본값이며 실제 실행·UI·Production 연결 성공은 아직 확인하지 않았다.
  다음은 Step 5 합성 로그 연결 확인이며 QA Player 실행은 Step 6에서 진행한다.
- Module 변경이 없으므로 추가 commit·재배포·기존 테스트 재실행은 요구하지 않는다.

**Step 4 완료.** 사용자 Build 완료 보고와 정적 실행 후보 설정을 근거로 판정하며 실제 Player 동작은 후속 Step에서 검증한다.

절차 근거: [Unity 6.3 Windows Build 설정](https://docs.unity3d.com/6000.3/Documentation/Manual/WindowsStandaloneBinaries.html).

빌드 설정으로 해상도를 고정할 수 없는 경우 사용자에게 Player의 실제 지원 설정/창 크기 조정 방법만 안내한다.
지원하지 않는 command-line 옵션이나 아직 없는 진단 기능을 있다고 가정하지 않는다.

완료 조건: [x] 사용자 Player Build 성공·1920×1080 Windowed 실행 후보·source/출력물 연결 기록.

## Step 5. Production 합성 로그의 실제 연결을 확인한다

착수 조건: Step 1의 합성 호출 승인 및 Step 2-2 전용 절차/도구 로컬 검사 완료.

### 5-1 AI: 도구·대상 사전 확인 — 완료 (2026-10-22)

- 기존 승인을 유지하며 Production 설정의 비공개 Project/Environment·고정 Module/함수·자격 증명 숨김 입력·무재시도 경계를 대조했다.
- 최초 로컬 `--plan`은 Editor selector가 verification이라는 이유로 `PRODUCTION_SCOPE_MISMATCH`를 반환했다.
  실제 Production 설정 오류가 아니라 점검 도구가 Editor 배포 선택에 의존한 조건 때문이며 원격 요청은 없었다.
- `production-scope.cjs`를 보정해 대상은 비공개 Production 설정과 Cloud Project 연결로만 고정한다.
  production 이름·인증 profile·유효 ID·verification과 다른 환경·Project 일치 검사는 유지한다.
  Editor selector는 요청 대상으로 사용하지 않으며 사용자가 selector를 바꿀 필요 없다.
- Production 도구 테스트 9개·PowerShell credential 정리 대역 검사·읽기 전용 Build 사전 검사가 통과했다.
  실제 `--plan`도 `ScopeMatched / NoRemoteRequests`로 통과했다.
- 변경은 로컬 점검 도구/테스트/문서다. 배포 Module host·embedded JS·manifest 및 Player source 변경은 없다.
  추가 Git commit·Unity Build/Test Runner 반복·Module 재배포는 필요 없다.

### 5-2 사용자: 점검 인증 준비·합성 호출 1회 — 완료

프로젝트 루트에서 아래 명령을 사용자 본인이 실행한다. 기존 점검용 UGS Service Account credential이
승인 Project에서 호출 권한을 가지고 있으면 사용할 수 있다. 없으면 UGS의 Manage organization → Service Accounts에서
점검용 credential을 준비하고 승인 Project의 필요한 호출 권한만 부여한다. 키 원문을 채팅으로 보내지 않는다.
GCP 런타임 service account key나 게임의 LogSink/이전 HMAC은 이 도구의 입력값이 아니다.

1. Step 2에서 확정한 Production 전용 도구와 인증으로 합성 이벤트를 한 번 보낸다.
   [Production QA 도구](../../../UGS/Verification/PRODUCTION_QA_TOOLS.md)의 인증 준비 절차를 확인하고 아래를 사용자 본인이 실행한다.

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\UGS\Verification\Invoke-ProductionSafeLogProbe.ps1 -Run -ConfirmProduction
   ```
2. 안전한 결과가 Accepted/HTTP 202인지 확인한다. 실패하면 분류만 보고하고 키를 임의 회전하지 않는다.
   기대 결과는 `PASS / PRODUCTION_SAFE_LOG_PROBE / Status=Accepted / Phase=Completed / Reason=None / HTTP=202`다.
   키는 스크립트의 숨김 입력창에만 넣으며 FAIL·timeout·응답 미확인은 추가 호출 없이 보고한다.

### 5-3 사용자: 실제 Cloud Logging 저장 확인 — 완료

1. GCP **Production 프로젝트**의 Logging → Logs Explorer에서 이번 호출을 포함하는 검색 범위로 아래 조건을 사용한다.

   ```text
   resource.type="cloud_run_revision"
   jsonPayload.environment="production"
   jsonPayload.reason="SyntheticProbe"
   ```

2. jsonPayload가 environment/moduleReference/outcome/reason/durationBucket/correlationId/rateLimitResult의
   정확히 7필드인지, production·승인 Module·Accepted·SyntheticProbe·Allowed인지 확인한다.
   `moduleReference`는 `FlowStateVerification`이며 `durationBucket`은 숫자, `correlationId`는 서버 생성 값인지 확인한다.
   일반 로그/health 요청이 아니라 `jsonPayload` 합성 이벤트 본문을 확인한다. 원문/스크린샷/식별자는 보내지 않는다.
3. 자동 재시도하지 않는다. RateLimited면 최소 60초 후 승인된 수동 재실행 한 번만 수행한다.

### 5-4 결과 대조·완료 판정 — 완료 (2026-10-22)

아래의 안전한 결과만 보고받아 판정한다.

```text
합성 호출: PASS/FAIL (도구의 안전한 결과 행)
Production Cloud Logging 이벤트: 확인/미확인
jsonPayload 정확히 7필드: 일치/불일치
production / FlowStateVerification / Accepted / SyntheticProbe / Allowed: 일치/불일치
점검 credential 새 발급·임시 권한 추가: 있음/없음
```

사용자 실행 결과와 제공된 로그를 대조했다.

- 합성 호출: **PASS / Status=Accepted / Phase=Completed / Reason=None / HTTP=202**.
- 실제 저장: Production Cloud Run의 `cloud_run_revision` 이벤트 확인.
- jsonPayload: **정확히 7필드**, 필수 이름 전체 일치. environment=production, moduleReference=FlowStateVerification,
  outcome=Accepted, reason=SyntheticProbe, rateLimitResult=Allowed이며 durationBucket은 숫자 250, correlationId는 서버 생성 형식이다.
- Cloud Logging envelope의 resource/labels/insertId/logName/timestamp 등은 jsonPayload 7필드 제한에 포함되지 않는다.
- 사용자 제공 로그 원문·프로젝트/리비전/인스턴스 식별자·correlationId는 문서에 복사하지 않았다.
- 점검 credential 신규 발급·추가 임시 권한 여부는 아직 미보고이며 Step 13에서 확인한다. Step 5 완료를 막는 항목은 아니다.

**Step 5 완료.** Production 합성 호출·Module의 LogSink 서명 전송 수락·실제 7필드 저장을 확인했다.
일반 요청 전체의 무손실 수집·게임 계정/제출/조회 성공으로 확대하지 않는다. 다음은 Step 6 Production QA 출발 계정 준비다.
새 점검 credential/임시 권한이 있으면 Step 13 회수 대상으로 기록하고 런타임 LogSink 권한은 유지한다.

Phase 4의 health 204나 verification Accepted는 이 Step의 성공 근거를 대신하지 않는다.
Production 일반 요청의 250 ms 로그 전송은 최선 시도이며 합성 Probe 성공이 무손실 수집 보장은 아니다.

완료 조건: [x] Production 합성 호출·서명 수락 및 실제 7필드 저장 확인. 원문 이벤트는 기록하지 않음.

## Step 6. 신규 Production QA 계정을 준비한다

착수 조건: Step 1에서 지정한 Production QA 출발 실행 공간·계정 생성 승인, Step 4 Player 준비, Step 5 연결 점검 완료.

### 6-1 AI: 선행 조건·생산 UI/계정 경로 대조 — 완료 (2026-10-22)

- Step 1의 분리된 전용 Windows 표준 사용자·QA 생성 승인, Step 4 사용자 Build 완료,
  Step 5 Production Accepted/202·7필드 저장 확인을 연결했다.
- `check-phase5-build.cjs`가 다시 통과하여 Profile·Production 설정·Scene 참조·기존 검증 source 일치를 확인했다.
- 실제 Scene label은 Settings, Online Record Notice, Continue, Account / Device Transfer, Refresh Status다.
- `AccountTransferView`의 공개 번호 표시는 `OnlineAccountViewState.NumberText`를 사용하며 내부 Ready 상태에서만 유효 번호를 반환한다.
  화면에 Ready라는 단어를 찾을 필요 없으며 공개 번호 표시와 오류/처리 중 안내를 확인한다.
- 기존 `NoConsentAndOffline_MakeZeroAuthenticationOrTransportCalls`와
  `AuthenticationImmediatelyIssuesNumberAndPersistsExactScopedCache`를 포함한 Step 3 성공 근거를 유지한다.
- Module/Player source 변경이 없다. Unity Build/Test Runner·Scene 변경·실제 인증값 열람·원격 요청은 수행하지 않았다.

### 6-2 사용자: Production QA 출발에서 Player 실행·동의 — 완료

1. 전용 Windows **표준 사용자** 계정인 **Production QA 출발**로 로그인한다. 기존 관리자/개발 계정에서 실행하거나
   다른 사용자 자격 증명으로 관리자 실행하지 않는다. Production QA 수신은 아직 실행하지 않아도 된다.
2. Step 4의 **Production-QA-Build-01**을 사용한다. 다른 Windows 사용자에서 빌드 폴더 접근이 안 되면
   실행 파일·`*_Data`·DLL 등 **빌드 출력 폴더 전체**를 출발 사용자가 읽을 수 있는 전용 위치로 복사한다.
   기존 Local Save·AppData·Authentication 저장은 복사하지 않는다. 실제 경로는 공유하지 않는다.
3. `.exe`를 기본 실행한다. verification 세션 인수나 Unity Editor 격리 도구를 사용하지 않는다.
   최초 조작 안내가 나오면 Main Menu로 이동하고 **Settings → Online Record Notice**를 연다.
4. Anonymous 기록의 복구 제한 안내를 읽고 **Continue**를 한 번 누른다.
   **공개 번호만으로 계정을 복구할 수 없고 이전 코드와 인증값이 필요하다**는 점을 확인한다.

### 6-3 사용자: 공개 번호·오류 확인 — 완료(오류 예외 사용자 승인)

1. Settings에서 **Account / Device Transfer**를 연다. 처리가 끝날 때까지 기다린다.
2. **Public number:** 뒤에 선행 0을 포함한 **10자리 십진 공개 번호**가 표시되고,
   `Public number: unavailable`, 처리 중 유지 또는 오류/재시도 안내가 없는지 확인한다.
   실제 번호는 로컬 비공개 기준으로만 보관하며 채팅/스크린샷에 붙이지 않는다.
3. 번호 확인이 되면 추가 Refresh는 필요 없다. 첫 요청이 실패하면 **Refresh Status**를 한 번만 사용할 수 있다.
   timeout·응답 미확인·같은 실패가 계속되면 플레이·제출·이전·신규 계정 생성 반복을 멈추고 안전한 상태 문구만 보고한다.
4. 이 Step에서는 Stage/Infinite 플레이·Start Transfer·Complete Transfer를 수행하지 않는다.
   정상 호출로 계정이 준비되는지 확인하며 Dashboard에서 Cloud Save container/key/ledger를 수동 생성하지 않는다.

### 6-4 결과 대조·완료 판정 — 완료(2026-10-23 사용자 승인)

```text
실행 공간: Production QA 출발
실행 빌드: Production-QA-Build-01
복구 제한 안내·Continue: 확인/실패
10자리 공개 번호 표시: 정상/없음
오류·timeout 안내: 없음/안전한 문구만
Refresh Status 사용: 안 함/1회
```

Player가 진단 상태를 표시하지 않으면 존재하지 않는 버튼을 찾게 하지 않고, 실제 계정 화면과
생산 코드의 표시 조건으로 판정한다. 후속 6-5의 실제 Notice·번호/조회·수정 검증 근거와 6-5-9의 명시적 예외 승인으로 Step 6을 완료했다.

완료 조건: [x] Production QA 출발의 동의·인증·번호/조회 준비와 안전한 결과 기록.
최초 Unavailable·Retry 4회 문제의 해결은 6-5-9 사용자 승인으로 완료 조건에서 제외한다. 오류 없는 최초 준비를 확인한 것으로 해석하지 않는다.

### 6-5 실제 Player 문제·수정 인계 — 2026-10-22

#### 6-5-1 사용자 관찰·원인 대조 — 완료

- 최초 Leaderboard 진입과 Settings 양쪽에서 Continue 후 Notice가 닫히지 않는 현상을 보고받았다.
  공개 번호 표시는 오래 걸린 뒤 확인됐고, 최초 TOP은 No records, AROUND YOU는 Read leaderboard/HTTP 404 오류였다.
  이후 사용자의 정상 Stage 플레이 뒤 AROUND YOU에 본인 기록이 나타났다. 실제 번호·원문 식별자는 기록하지 않는다.
- 공개 번호·Stage 행 확인은 기존 QA 자료의 성공 근거로 보존한다. 새 계정 생성/Stage 재제출로 반복하지 않는다.
  Notice 필수 UI 문제가 남아 Step 6 전체와 Step 11은 완료 처리하지 않는다. Step 7 전체 완료로도 확대하지 않는다.
- Continue의 기존 adapter는 인증·서버 계정/번호 준비와 Pending 재시도를 모두 await한 뒤 창을 닫았다.
  이는 네트워크 대기 동안 Notice가 남는 직접 코드 경로다. 장시간 지연이 정확히 어느 서비스에서 발생했는지는 아직 미확인이다.
- query-records는 me 404만 빈 성공으로 처리하고 around 404는 ServiceUnavailable로 전달했다.
  404는 보드 누락일 수도 있어 단순히 Error 표시를 없애지 않고 실제 부재 판정 경계를 수정했다.

#### 6-5-2 AI 로컬 수정·회귀 준비 — 완료

- `OnlineRecordCoordinator.TryConfirmRecoveryNotice`로 동의 저장과 원격 인증을 분리했다.
  `GameSystem`은 저장 성공 직후 Notice를 닫고 준비 작업을 진행한다. Leaderboard는 Loading 상태에서 같은 준비 Task를 기다린다.
  반복 Continue는 진행 중 Task에 합류하며 저장 실패에서는 창을 유지하고 인증을 시작하지 않는다.
  Pending 재전송은 번호 준비 완료 뒤 처리하여 창 닫힘·목록 준비를 붙잡지 않게 했다.
- `query-records.js`는 around 404에서 본인 score 404와 보드 정상 조회를 확인한 뒤 `Success/NoPersonalRecord`를 반환한다.
  me 404에도 보드 존재 확인과 최종 활성 계정 재검사를 적용했다. 본인 행이 있거나 보드 누락/권한/서비스 실패면 Error를 유지한다.
- UI는 해당 성공 상태를 `No online record yet. Submit a record to see your position.`으로 표시하고 TOP의 No records와 구분한다.
- 기존 생산 endpoint 대역에 12개 사례를 추가하여 `record-account-integration` **46/46** 통과,
  전체 Node 독립 **53파일 실패 0**, 변경/신규 C# **5파일 정적 preflight** 통과.
- `OnlineRecoveryNoticeFlowTests`의 PlayMode 6사례(느린 인증 성공/실패·동의 저장 실패·두 빈 목록 문구·실제 Scene 최초 Leaderboard Loading 진입)를 준비했다.
  Unity compiler/Test Runner를 실행한 결과는 아니다. Scene 참조 변경은 필요 없다.
- 서버 변경은 `UGS/CloudCode/query-records.js` 한 파일이며 현행 C# Module에 embedded된다.
  이전의 "Module 변경 없음" 판단은 그 시점의 근거로 보존하고 **이번 수정에는 적용하지 않는다**.
  `check-phase5-build.cjs`는 기존 Step 3 hash와 다른 후보를 회귀/재빌드 대기(exit 2)로 표시한다.

#### 6-5-3 사용자 Unity 회귀·Module 게시·Player 재빌드 — 회귀 완료, 게시·재빌드 대기

1. 기본 Windows(verification) profile에서 Unity 컴파일·전체 EditMode/PlayMode를 실행하고
   실제 수량·실패·예상 밖 Error/Warning을 보고한다. Scene 수정은 없다.
2. 회귀 통과 후 변경된 Module의 verification 적용/대역과 계약 근거를 연결하고 Production 새 게시 대상·변경 범위를 확정한다.
   게시 단위는 기존 `FlowStateVerification.ccmr` 하나다. AI가 원격 배포나 Unity 빌드를 수행하지 않는다.
   실제 Production 게시 결과 전에는 수정된 서버 동작을 검증 완료로 기록하지 않는다.
3. 사용자 Production Player를 기존 출력물을 덮어쓰지 않는 새 출력물 별칭으로 Build한다.
   기존 QA 인증/Local Save·서버 Stage 기록은 유지한다. 새 후보 source/검증/출력물의 연결을 갱신한다.
4. 같은 Production QA 출발에서 Settings의 Notice Continue가 원격 응답 전에 닫히는지 확인한다.
   최초 동의 Leaderboard 경로는 새 자동 테스트로 검증하며 이를 수동 재현하려고 동의/계정을 초기화하지 않는다.
5. Infinite 기록이 아직 없다면 Infinite의 AROUND YOU를 열어 미기록 안내를 확인한다.
   Stage는 기존 본인 행을 재조회하며 시험을 위해 추가 Stage 제출이나 새 QA 계정을 만들지 않는다.

6-5-3-1은 아래 6-5-5, 게시·재빌드는 6-5-6의 사용자 보고로 완료했다. 현재 다음 작업은 **6-5-3-4~5 같은 QA 계정 UI 확인**이다.
Step 4/5의 기존 후보 Build·합성 로그 성공은 유지하되 이번 수정 후보의 Build·서버 동작 성공을 대신하지 않는다.

#### 6-5-4 사용자 EditMode 4실패·fixture 보정 — 2026-10-22

- 사용자 컴파일 성공·Error 없음 보고. Warning 여부와 이번 EditMode 전체 실행 수는 미보고이며 추정하지 않는다.
- EditMode 실패 4개를 보고받았다. PlayMode 결과는 아직 없다.
  - AuthenticationImmediatelyIssuesNumberAndPersistsExactScopedCache: 온라인 준비는 성공했고 마지막 저장 scope의 verification 고정 assertion에서 실패.
  - ScopedAreas_RoundTripAndPreserveSettingsTutorialAndBindings: 기본 scope로 저장한 자료를 verification으로 고정 복귀해 빈 영역을 참조.
  - MigrationWriteFailure_KeepsOriginalFilePendingAndBlocksRequests: v5 자료는 verification 귀속인데 활성 scope에 Pending이 있다고 기대.
  - ProductionQueryAndSubmissionGate_BlocksUnpersistedMigrationBeforeAuthentication: v5를 활성 환경 자료로 가정하여 checkpoint 후 온라인 성공을 기대.
- `Library/BuildProfileContext.asset`에 FLOW_STATE_PRODUCTION이 캐시되어 있음을 읽기 전용 확인했다.
  위 네 실패 위치는 Production 활성 scope와 verification 고정 fixture/기대값의 불일치로 설명된다.
  테스트 실패를 이유로 환경 분리나 v1~v5의 verification 귀속 정책을 바꾸지 않는다.
- 테스트 2파일만 수정했다. 계정 캐시는 CreateConfigured 기대값으로, 영역 왕복은 실제 source.OnlineScope로 복귀한다.
  migration 저장 실패/요청 gate의 입력은 활성 scope를 명시적으로 보유하는 v6→v7 자료로 바꿨다.
  v1~v5의 verification 귀속·다른 환경에서 비활성 보존 검증은 기존 legacy 테스트에서 계속 유지한다.
- C# source preflight(변경/신규 7파일), Local Save·account 정적 계약, Phase 4 환경 분리 정책 검사는 통과했다.
  Unity Test Runner를 실행하지 않았으므로 수정 후 실제 통과는 사용자 결과 대기다.
- 현재 Windows-production profile을 유지해도 이 네 테스트의 의미가 맞도록 보정했다.
  다음은 사용자 재컴파일 후 **전체 EditMode와 PlayMode 결과 보고**다. 추가 서버/Player source 변경이나 Scene 수정은 없다.
  실제 QA 저장·계정·기록을 초기화하거나 Module 게시를 반복하지 않는다.

#### 6-5-5 두 profile의 사용자 Unity 회귀 — 완료 (2026-10-22)

- 사용자가 Production·verification profile 각각에서 컴파일 성공·Error 없음과 전체 Edit Mode·Play Mode 테스트 성공을 보고했다.
- 6-5-4의 네 실패 보정 후 두 profile 회귀를 완료했다. 실행 수와 Warning 여부는 이번 보고에 없으므로 추정하지 않는다.
- 검증 대상 GameSystem.cs SHA-256: `E8CD3349BC93AAA9B0A0E7977D71E172B33D2AE3A7123096134AAD8B03B56199`.
  정적 Build 검사 기준을 이 사용자 회귀 완료 source로 갱신한다. 새 Player Build 성공을 뜻하지 않는다.
- 배포 기준 `8774b2db2defa76c7e6a4ddd73f0f37f187c7cee` 대비 Module embedded source 변경은
  `UGS/CloudCode/query-records.js` 한 파일이다. 수정 Module 게시·새 Production Player Build·같은 QA 계정의 실제 UI 확인은 대기다.
- AI는 Unity 컴파일·Test Runner·Build·원격 게시·Scene 변경을 수행하지 않았다. Step 6 전체는 아직 미완료다.

#### 6-5-6 수정 Module·새 Player 사용자 결과 — 2026-10-23

- 사용자가 수정 Module 게시와 새 Player Build 성공을 보고했다. 게시 환경별 상세/version·새 출력물 식별자는 미보고이며 추정하지 않는다.
- Production QA 출발에서 새 Player를 실행했고 기존 로컬 기록이 유지됐다. Leaderboard 직행 시 Notice가 표시되지 않았다.
  생산 코드의 자동 Notice 조건은 로컬 기록 유무가 아니라 저장된 `HasConfirmedRecoveryNotice`다.
  기존 동의를 유지한 경우 자동 Notice 생략은 정상이며 최초 동의 재현을 위해 저장/인증을 삭제하지 않는다.
- 첫 조회에서 Unavailable이 나타났고 잠시 대기 후 Retry하자 공개 번호와 기록이 정상 조회됐다.
  Retry 복구 성공으로 기록하되 첫 요청 오류가 없었다거나 원인이 계정 생성/콜드 스타트였다고 판정하지 않는다.
  최초 실패의 정확한 안전한 상태 문구/진단은 미보고다. 반복될 경우 별도 조사할 관찰 사항으로 유지한다.
- 게시·Build 완료 항목은 다시 요구하지 않는다. 남은 수동 확인은 Settings → Online Record Notice → Continue 닫힘과,
  Infinite 미기록 상태의 AROUND YOU 안내다. 기존 기록 조회 성공만으로 미기록 분기를 검증한 것으로 확대하지 않는다.
- Step 6 전체는 아직 미완료다. AI는 이번 결과 기록 과정에서 runtime/Module/Scene을 수정하거나 Build/Test Runner를 실행하지 않았다.

#### 6-5-7 재확인 결과·근거 범위 구분 — 2026-10-23

- Settings → Online Record Notice → Continue에서 창이 닫힘을 사용자 확인했다.
  이는 기존 동의·계정 준비 상태의 경로이며 최초 동의/미준비 계정의 실서비스 문제 해결 근거로 대체하지 않는다.
- 기록 없는 Infinite의 AROUND YOU에서 오류 대신 미기록 안내 표시를 사용자 확인했다. 해당 수정의 실제 Player 확인은 완료다.
- 이번 재실행에서는 즉시 Leaderboard에 접근해 Retry 없이 번호/기록을 정상 조회했다.
  앞선 Unavailable 관찰은 보존한다. 서버 환경이 원인이라는 사용자 추정은 미확인으로 기록하며 클라이언트 원인도 배제하지 않는다.
- 최초 동의·미완료 인증 경로의 근거는 통과 보고된 PlayMode 자동 테스트다.
  `ContinueClosesBeforeAuthentication_QueriesWaitAndRepeatedContinueJoins`는 메모리 저장소/지연 인증 대역으로
  네트워크 응답 전 창 닫힘·동의 저장·조회 대기·중복 합류를 확인한다.
  `FirstLeaderboardContinueClosesNoticeAndEntersLoadingBeforeAuthentication`는 실제 SampleScene에서
  최초 Leaderboard Notice → Continue → 창 닫힘/Loading을 확인하되 인증과 기록 저장소는 대역이다.
  두 profile의 Test 통과와 실제 Production 최초 계정 생성/서비스 지연 조건의 재현을 혼동하지 않는다.
- Settings·미기록 안내의 수동 확인은 더 요구하지 않는다. 최초 동의 실서비스 재현은 미수행으로 유지한다.
  기존 QA 저장/인증 초기화나 신규 Production 계정 생성은 수행하지 않는다.
  Step 6 전체는 이 검증 범위 판정 전까지 미완료로 유지한다. 이번 작업은 문서 기록만이며 source 변경/재게시/재빌드는 없다.

#### 6-5-8 최초 동의 실서비스 사용자 확인 — 2026-10-23

- 앞선 초기화 시도는 개발 Windows 사용자와 QA 출발 사용자를 혼동한 것으로 사용자가 설명했다.
  이후 Production QA 출발에서 초기 조건 준비 후 새 Player → 즉시 Leaderboard → Notice 표시 → Continue → Leaderboard UI 진입을 보고했다.
  최초 동의 조건에서 Notice 닫힘·화면 전환의 실서비스 확인은 완료다. 기존 준비 상태의 Settings 확인과 구분한다.
- 이어 Unavailable이 표시됐고 Retry를 4회 시도한 뒤 Leaderboard가 표시됐다.
  이는 최초 준비/조회 실패 후 수동 재시도로 복구된 결과이며 정상 Loading만으로 최초 준비가 끝난 결과는 아니다.
  "계정이 아직 없었음/Retry 동안 임시 계정 생성"은 사용자 추정으로 보존한다. 실제 UGS 인증/서버 공개 계정 준비의
  어느 단계가 실패했는지, 동일 계정 준비를 이어갔는지 여부는 현재 보고만으로 확정하지 않는다.
- Notice 수정의 최초 조건 확인은 더 요구하지 않는다. 새 계정 생성/저장 초기화/추가 Retry로 재현을 반복하지 않는다.
  현 신규 확인 세션과 기존 출발 보관 자료를 별도 보존한다. 원래 출발로 복원할 경우 Local Save와 인증 캐시를 같은 쌍으로 복원한다.
- 남은 문제는 최초 계정 준비/조회의 Unavailable 및 Retry 4회 필요 원인이다. 안전한 오류 reason/phase/HTTP가 있으면
  그 근거로 조사하며 원문 저장/인증/공개 번호는 요구하지 않는다. Step 6 전체는 이 문제 미해결로 미완료다.
- 이번 처리는 읽기 전용 코드 대조·문서 갱신이며 runtime/Module/Scene 변경·Build/Test Runner·원격 요청은 수행하지 않았다.

#### 6-5-9 완료 기준 변경·Step 6 완료 — 2026-10-23 사용자 승인

- 사용자가 "Unavailable 이후 Retry 4회" 문제를 보류하고 처리할 예정이 없음을 명시하며 Step 6 완료를 지시했다.
- 해당 문제의 원인 조사·수정·재현 검증을 Step 6 완료 조건에서 제외한다. 후속 수정 일정이나 처리 계획을 자동으로 만들지 않는다.
  최초 준비/조회 실패와 수동 재시도 필요는 원인 미확인·미해결인 잔여 위험으로 유지한다. 해결/서버 원인 확정으로 기록하지 않는다.
- 최초 실서비스 Notice 표시 → Continue → Leaderboard UI 진입, Retry 후 조회 복구,
  기존 상태 Settings Notice 닫힘·Infinite 미기록 안내, 두 profile Unity 회귀·수정 Module 게시·새 Build 결과를 근거로 Step 6을 완료한다.
- 앞선 6-5의 미완료/조사 대기 문구는 당시 이력이며 현재 판정은 본 항목이 우선한다.
  다음은 Step 7이다. Step 7·11·Phase 5 전체 또는 일반 사용자 공개를 완료/승인한 것은 아니다.
- 기존 출발 보관 자료와 최초 실행 확인 세션의 저장/인증은 혼합하지 않는다. 추가 계정 생성·초기화·재게시·재빌드는 요구하지 않는다.

## Step 7. 실제 Stage·Infinite 기록을 제출하고 순위를 확인한다

### 7-1 AI: 선행 조건·정적/로컬 대역 대조 — 완료 (2026-10-23)

- 사용자가 최초 Notice 확인에 사용한 신규 게임 계정 상태를 유지하기로 선택했다.
  Windows 실행 공간 별칭은 계속 **Production QA 출발**이며, 이 신규 게임 계정을 Step 7~10의 동일 출발 기준으로 사용한다.
  원래 Stage 기록이 있는 출발 상태는 별도 보관한다. 저장·인증을 복원/혼합하거나 옛 행을 현 계정의 본인 기록 근거로 사용하지 않는다.
- 수정 Module 게시·새 Player Build·두 profile Unity 전체 회귀의 기존 사용자 성공 근거를 연결했다.
  이번 작업은 source 변경이 없어 재컴파일/Runner·재게시·재빌드·Scene 작업을 요구하지 않는다.
- `record-account-integration.test.cjs`의 실제 bundled 제출/조회 endpoint 로컬 대역 **46/46**,
  leaderboard query/owner 정책 2파일, `static-contracts.cjs`, `client-unity-preflight.cjs`(변경/신규 C# 7파일),
  `check-phase5-build.cjs`를 재실행해 통과했다. 원격 UGS 호출·Unity 실행은 하지 않았다.
  Build 검사기의 `PlayerRebuildStillRequired` 고정 출력은 이전 인계 문구이며 6-5-6의 사용자 새 Build 완료 보고를 취소하지 않는다.
- TOP 10/AROUND 7·전역 순위·동점·고정 소유 행·계정 fence·공개 번호 응답/미기록 판정은 자동/정적 근거를 사용한다.
  소수 실제 행만으로 전체 순위 정책이나 서버 원자성을 수동 입증하도록 요구하지 않는다.
- 생산 UI를 대조했다. 제출 성공 표시는 `Submission: Submitted`다. Pending 0건이나 로컬 최고만으로 성공 판정하지 않는다.
  Stage는 Cleared만 제출 가능하고 Infinite는 정상 확정 Result가 필요하다. Submitted는 최고 기록 갱신을 뜻하지 않는다.
- Step 6의 최초 Unavailable·Retry 4회 문제는 사용자 승인대로 처리 계획 없이 보류하며 재조사 조건으로 사용하지 않는다.

### 7-2 Stage — 완료

1. **Production QA 출발**에서 현재 신규 게임 계정 상태와 이미 완료한 새 Production Player를 유지한다.
   Main Menu → Play → Stage에서 `stage-001`을 한 번 정상 클리어한다. 기존 보관 상태의 과거 Stage 성공을 대신 사용하지 않는다.
2. 제출 완료를 확인한다. Pending/오류이면 새 Run으로 덮지 말고 상태를 보고한다.
3. Leaderboard에서 **Stage / TOP**, **Stage / AROUND YOU**를 각각 연다.
4. AROUND YOU의 본인 행에 `(You)`가 표시되고 화면 계정의 공개 번호와 일치하는지 로컬에서 확인한다.
   Stage 시간 표시를 확인한다. TOP은 상위 10개이므로 본인 행이 TOP 밖에 있어도 실패가 아니다.
5. 화면의 조회 Retry를 한 번 사용해 재조회와 Back 복귀를 확인한다. `Retry Pending (N)`과 혼동하지 않는다.

### 7-3 Infinite — 완료(후속 Run 성공, 첫 실패 이력 보존)

1. 정상 Infinite 플레이를 종료해 기록을 제출한다.
2. 제출 완료 후 **Infinite / TOP**, **Infinite / AROUND YOU**를 각각 연다.
3. AROUND YOU 본인 행의 `(You)`·공개 번호 일치와 점수 표시를 확인하고 Back으로 복귀한다.
   TOP 밖에 본인 행이 있는 것은 실패가 아니다. 높은 점수 우선/동점의 정확한 정책은 자동 근거로 판정한다.

### 7-4 결과 대조·완료 판정 — 완료 (2026-11-09)

```text
실행 공간/상태: Production QA 출발 / 최초 Notice 확인에 사용한 신규 게임 계정 유지
Stage: Cleared / Submission: Submitted 확인 또는 안전한 실패 문구
Stage TOP: 정상/실패, AROUND YOU: 본인 행·(You)·공개 번호 일치 확인/실패
Stage 조회 Retry·Back: 정상/실패
Infinite: 정상 Result / Submission: Submitted 확인 또는 안전한 실패 문구
Infinite TOP: 정상/실패, AROUND YOU: 본인 행·(You)·공개 번호 일치 확인/실패
Infinite Back: 정상/실패
```

실제 공개 번호·점수·인증값은 공유하지 않는다. Pending/Rejected/조회 실패면 같은 Mode를 추가 플레이해 덮지 말고 결과를 보고한다.
제출 제한을 시험하려고 연속 Run을 만들거나 최고 갱신/상위권 진입을 강제하지 않는다. 초기 조회 지연 문제는 보류 결정을 유지한다.

AI는 TOP 10/AROUND 7 요청·전역 순위·동점·대규모 보드를 Unit Test로 판정한다.
Production에 100명 이상의 QA 계정이나 128건 이상의 기록을 만들도록 요구하지 않는다.
빈 보드라도 내 최초 행이 생기면 충분하며 최대 행 수 채우기는 수동 조건이 아니다.

완료 조건: [x] Stage 제출/두 목록, [x] Infinite 후속 Run 제출/두 목록 정상.
아래 7-6의 추가 사용자 보고로 현재 Pending 표시 없음·Back 정상 복귀를 확인하여 Step 7을 완료했다.

### 7-5 사용자 실제 제출·조회 보고 — 2026-11-09

- Production QA 출발의 신규 게임 계정에서 공개 번호 확인을 보고받았다. 실제 번호는 문서에 복사하지 않는다.
  번호의 전 실행 대비 변경 여부/의도하지 않은 재인증 여부는 이번 보고만으로 추정하지 않는다.
- Stage 정상 클리어·`Submission: Submitted`·TOP/AROUND YOU의 `(You)`/공개 번호 일치·조회 Retry 동일 결과를 확인했다.
- Infinite 첫 정상 종료 뒤 제출 실패를 보고했다. 이후 Result의 Retry로 Infinite를 다시 플레이하여 종료했고
  그 후속 Run의 제출 성공·TOP/AROUND YOU `(You)`/공개 번호 일치를 보고했다. 게임 종료도 보고했다.
- 새 플레이는 새 제출 후보가 될 수 있으므로 후속 Run 성공을 첫 실패 제출 ID의 재전송 성공으로 판정하지 않는다.
  첫 실패의 Pending/Rejected/안전한 reason, 해당 후보 terminal/잔류 여부는 미보고다. 자동 재시도로 이미 처리됐을 가능성도 미확인이다.
- Stage·Infinite의 Back 조작 결과는 미보고이며 게임 종료를 Back 성공으로 대신하지 않는다.
  추가 플레이/초기화는 요구하지 않는다. 같은 계정 재실행 후 현재 Pending 유무를 확인하고,
  남아 있는 대기가 있으면 Step 8 신규 Offline 기록 생성 전에 상태를 먼저 기록한다. 첫 실패 원인 추정이나 수정은 수행하지 않는다.
- Step 6 최초 조회 지연 보류는 유지하되 이번 제출 실패를 그 승인 범위에 자동 포함하지 않는다.
  Step 7 주요 제출/조회 조건은 충족했으나 전체 완료는 위 잔여 결과 확인 전까지 보류한다.

### 7-6 추가 사용자 결과·Step 7 완료 — 2026-11-09

- 사용자가 Pending 표시 없음과 Leaderboard에서 Back 정상 복귀를 확인했다. 7-5의 현재 Pending/Back 확인 대기를 해소한다.
- Stage Submitted·TOP/AROUND 본인 표시·조회 Retry, Infinite 후속 Run 제출·두 목록 본인 표시,
  현재 Pending 표시 없음·Back 정상 복귀 및 7-1 자동/정적 근거를 연결해 Step 7을 완료한다.
- 첫 Infinite 실패의 정확한 사유와 최초 제출 ID의 terminal 결과는 미확인 이력으로 보존한다.
  Pending 표시 없음만으로 첫 기록의 서버 수락/자동 재전송 성공을 판정하지 않는다. Step 7의 성공 근거는 확인된 후속 Run이다.
- 다음은 Step 8이다. 실제 Offline 생성·재시작·재전송 검증을 완료한 것은 아니다.
  같은 출발 게임 계정을 유지하며 Step 8 착수 직전에도 Pending 없는 기준 상태를 유지한다.
- 이번 처리는 문서 갱신만이며 추가 Run·source/Scene 변경·Build/Test Runner·Module 재게시를 수행하거나 요구하지 않았다.

## Step 8. Offline Pending의 보존·재시작·재전송을 확인한다

착수 조건: Step 7의 같은 **Production QA 출발 게임 계정**에서 제출/조회 성공·현재 Pending 표시 없음을 확인했다.
원래 출발 보관 상태를 복원하거나 다른 게임 계정/verification 환경으로 변경하지 않는다.

사용자 수행:

1. 아래 8-2에서 Offline Stage Pending을 1건 생성하고 Player 정상 종료 후 저장 복사본을 확보한다.
2. 8-3에서 Offline 재실행/정상 종료 후 두 번째 복사본을 확보하고 AI의 보존 비교 결과를 기다린다.
3. 비교 통과 후에만 8-4 온라인 복구·원래 후보 수락/제거 확인을 진행한다.

원래 제출 ID·생성 시각·귀속의 보존과 재전송 멱등성·정확한 제거는 AI가 Unit Test와 준비한 로컬 비교기로 대조한다.
재연결 시 실제 구현에 의해 먼저 처리됐으면 남은 대기 없음 결과를 기록하며 시험을 맞추려고 추가 기록을 만들지 않는다.
timeout/응답 미확인이면 Pending을 남기고 결과를 보고한다. 수동 저장 파일 편집·강제 중복 제출은 필요 없다.

### 8-1 AI: 정적·로컬 대역 대조 — 완료 (2026-11-09)

- Step 7 사용자 결과·기존 두 profile Unity 회귀를 연결했다. runtime/Module/Scene source 변경은 없다.
  재컴파일·Unity Test Runner·Build·Module 재게시를 요구하거나 수행하지 않는다.
- `production-tools.test.cjs` **9/9**, retention 정책·Local Save 정적 계약,
  실제 bundled endpoint 대역 **46/46**, `static-contracts.cjs`, C# preflight **7파일**을 재실행해 통과했다.
  합성 자료/로컬 대역만 사용했으며 실제 UGS 요청이나 QA 저장 열람은 아직 하지 않았다.
- `OnlineRecordRepositoryTests`의 Pending 원래 ID 재시작 복구·Timeout 보존·제거 저장 실패·3회 제한/백오프 사례를 정적으로 대조했다.
  이는 이번 C# Test 실행이 아니다. 기존 사용자 전체 Test 통과 근거와 구분한다.
- 앱 시작/연결 복구 시 자동 재전송될 수 있다. 실제 Leaderboard 버튼은 **Retry Pending (N)**이며 조회 **Retry**와 다르다.
  수동 전송 결과 문구는 `Last retry: N submitted, N rejected, N pending`이다.
  과거 안내의 Retry Pending Uploads 대신 이 실제 명칭을 사용한다.
- `pending-restored`는 Production scope·동일 계정·전체 저장 내용과 ID/시각/귀속/payload 보존을 검사한다.
  `pending-removed`의 `REMOVED_ONLY`는 제거 근거이지 Submitted 근거가 아니다. 기존 최고 행 조회만으로 이번 수락을 추정하지 않는다.

### 8-2 사용자: Offline Pending 생성·첫 자료 보관 — 완료 (2026-11-09)

1. **Production QA 출발**에서 Player를 종료한 상태로 시작한다. 직전 Pending 없음 기준을 유지한다.
2. Wi-Fi·유선 등 PC의 실제 네트워크 연결 경로를 모두 끊는다. 방화벽/서버 정책이나 인증 상태는 수정하지 않는다.
3. 같은 Production Player를 실행해 Stage를 **한 번** 정상 클리어하고 Result의 Pending 표시를 확인한다.
   추가 Stage/Infinite 플레이나 Result의 게임 Retry는 하지 않는다.
4. 네트워크를 끊은 채 Player를 정상 종료한다.
5. QA 출발의 아래 저장 파일을 비공개 위치에 **복사**하고 이름을 바꾼다. 원본은 이동/편집하지 않는다.

```text
원본: %USERPROFILE%\AppData\LocalLow\DefaultCompany\Unity_Flow_State\flow-state-save.json
복사 위치: C:\Unity\Unity_Flow_State\Ignore\Phase5Private
첫 복사본 이름: step8-offline-before-raw.json
```

접근이 안 되면 관리자 자격으로 Player를 실행하지 말고 알려준다. 복사본 이름이 이미 있으면 덮어쓰지 않는다.
원문/공개 번호/인증 캐시는 채팅이나 Git에 올리지 않는다. 사용자 역할은 실제 QA 실행 자료의 복사뿐이며 정적 내용 검사는 AI가 한다.

### 8-3 사용자: Offline 재실행·자료 보관 / AI: 비교 — 완료 (2026-11-09)

1. 네트워크가 계속 끊긴 상태에서 같은 Player를 재실행한다. 새 플레이는 하지 않는다.
2. Leaderboard에서 **Retry Pending (1)** 등 대기 개수가 유지되는지 확인한다.
   목록 Offline/Error와 Pending 개수는 구분한다. Offline에서 Refresh/Retry를 반복하지 않는다.
3. Player를 정상 종료한 뒤 같은 원본 저장을 **step8-offline-restarted-raw.json** 이름으로 위 비공개 위치에 복사한다.
4. 생성 시 Pending 표시·재실행 후 개수·복사본 준비 여부만 보고한다.
   종료 후 보고를 위해 네트워크를 복구해도 되지만 AI 비교 전 게임을 다시 실행하지 않는다.
5. AI가 두 파일의 Production scope·동일 계정·Pending 1건과 원래 ID/시각/귀속/payload 보존을 검사하고
   capture 도구/`pending-restored` 비교를 수행한다. 사용자는 JSON을 열거나 비교 명령을 실행할 필요가 없다.

8-3 결과: 사용자가 최초 Pending 표시·Offline 재실행 후 1건·두 복사본 준비를 보고했다.
AI가 두 raw 저장을 capture 도구로 검증해 별도 `step8-offline-before-validated.json` /
`step8-offline-restarted-validated.json`을 비공개 위치에 생성했다. raw/원본을 덮어쓰지 않았다.
Production scope·유효 v7·동일 계정·양쪽 Pending 정확히 1건을 확인했고 `pending-restored`는 **PRESERVED**로 통과했다.
이는 원래 제출 ID·생성 시각·귀속·payload 및 전체 저장 내용의 재시작 보존 근거다. 원문/식별자는 출력하지 않았다.
실제 온라인 수락/제거 근거는 아니며 8-4는 아직 미수행이다.

### 8-4 사용자: 온라인 복구 / AI: 제거·수락 근거 대조 — 완료

1. 8-3 AI 비교 통과 후 같은 계정/Player에서 네트워크를 복구한다. 새 Run을 만들지 않는다.
2. 계정 준비가 필요하면 Settings → Account / Device Transfer → Refresh Status를 한 번 사용한다.
3. 자동 처리 뒤에도 Pending이 남아 있으면 Leaderboard의 **Retry Pending (N)**을 한 번 사용한다.
   submitted/rejected/pending 개수와 안전한 상태를 보고한다. 조회 Retry/게임 Retry와 혼동하지 않는다.
4. 자동 전송으로 이미 대기가 없으면 그대로 보고한다. 추가 대기 생성이나 정밀한 클릭 타이밍을 요구하지 않는다.
5. Leaderboard 조회·Pending 표시 없음 여부를 확인하고 Player를 정상 종료한 뒤,
   같은 원본 저장을 **step8-online-after-raw.json**으로 복사한다. AI가 같은 계정·기준 후보 제거를 대조한다.
6. Submitted/Rejected를 구분한다. 자동 처리로 수락 근거를 못 얻었으면 AI가 기준 제출 ID와 읽기 전용 서버 receipt 등
   수락 근거를 연결할 추가 절차를 안내한다. Pending 제거/기존 최고 행 조회만으로 성공을 추정하지 않는다.

### 8-5 결과 대조·완료 판정 — 완료 (2026-11-09)

8-1~3 보존 비교·8-4 원래 후보 수락/제거와 제출 후 Stage 조회 성공을 8-6~7로 연결해 Step 8을 완료했다.
오류/timeout/Rejected/잔류 시 추가 Run·초기화·반복 수동 재시도 없이 상태를 보고한다. 최초 조회 지연 문제의 기존 보류 결정은 유지한다.

완료 조건: [x] Offline 생성·재시작 보존·온라인 복구의 실제 Player 경로와 자동 비교 결과 연결.

### 8-6 사용자 재전송 결과·AI 저장 대조 — 2026-11-09

- 사용자가 Leaderboard 진입 시 자동 전송은 관찰하지 못했고 **Retry Pending (1)**을 실행했다고 보고했다.
  `Last retry: 1 submitted, 0 rejected, 0 pending`을 확인했다. 조회 진입 자체가 자동 제출 계기라고 설명하지 않는다.
  생산 코드의 앱 시작/연결 복구 계기와 이번 수동 성공을 구분하며 자동 처리 미관찰의 원인은 추정하지 않는다.
- AI가 `step8-online-after-raw.json`의 Production scope·유효 v7 저장을 검증하고 별도 validated 복사본을 생성했다.
  재시작 Offline 기준과 `pending-removed` 비교는 **REMOVED_ONLY** 통과, 같은 계정·전 Pending 정확히 1건/후 0건을 확인했다.
- 수락 근거는 사용자 수동 처리 결과의 **1 submitted / 0 rejected**이며 저장 비교는 제거 근거다.
  이번 생성 후보가 1건뿐이라는 전후 자료와 함께 원래 Pending의 수동 재전송 수락/제거를 확인한다.
  원본/raw를 수정·삭제하지 않았으며 원문/식별자는 출력하지 않았다.
- Offline 생성/재시작 보존·온라인 수동 제출 수락·저장 제거는 완료했다.
  제출 후 Stage Leaderboard 정상 조회 여부는 이번 보고에 명시되지 않아 마지막 확인으로 남긴다.
  추가 플레이·Pending 재생성·수동 제출 재시도·재빌드/재게시나 자동 전송 재현은 요구하지 않는다.

### 8-7 제출 후 조회 확인·Step 8 완료 — 2026-11-09

- 사용자가 Pending 제출 후 Stage Leaderboard 조회 성공을 확인해 마지막 조회 결과 대기를 해소했다.
- 실제 Offline 생성/재시작 1건 보존과 AI `PRESERVED`, 수동 `1 submitted, 0 rejected, 0 pending`,
  동일 계정의 저장 제거 `REMOVED_ONLY` 및 제출 후 Stage 조회를 근거로 Step 8을 완료한다.
- 자동 전송은 이번 실행에서 관찰되지 않았으며 검증 완료로 확대하지 않는다. 앞선 조회 지연 보류 결정도 유지한다.
- 앞선 대기 문구는 당시 이력이다. 다음은 같은 출발 게임 계정의 Step 9 재실행·자료 보존 확인이다.
  이번 보고 반영은 문서만 변경했으며 추가 플레이·원격 요청·Unity Build/Test Runner·Scene/Module 변경은 없다.

## Step 9. 기존 Production QA 계정의 재실행·자료 보존을 확인한다

기존 계정은 Step 6~8의 **같은 Production QA 출발 게임 계정**이다. 최초 Notice 확인 후 유지하기로 선택한 신규 상태이며,
그보다 이전의 보관된 원래 출발 계정을 복원하거나 verification 자료를 Production으로 복사하는 뜻이 아니다.

사용자 수행:

1. 9-2에서 종료된 Player의 로컬 저장과 읽기 전용 서버 기준 자료를 확보하고 AI 검증을 기다린다.
2. 9-3에서 같은 계정·Player를 재실행해 계정/두 목록·기존 설정 적용만 확인하고 정상 종료한다. 새 기록 제출은 하지 않는다.
3. 전후 자료의 정확한 값·서버 시각·귀속·Settings/Binding/Tutorial은 9-4에서 AI가 비교한다.

Local Save 구버전 migration은 복제 자료·가짜 시계 Unit Test로 확인한다. 실제 legacy 자료가 없는 경우
서비스 migration 전후 비교는 N/A로 기록하며 존재하지 않는 오래된 계정을 만들거나 운영 저장을 변조하지 않는다.

### 9-1 AI: 선행 조건·정적/비교 도구 준비 — 완료 (2026-11-09)

- Step 8 보존/수동 수락/저장 제거/Stage 조회 성공과 Pending 0건 기준을 연결했다.
- `production-tools.test.cjs` **9/9**, Local Save·Cloud Save 데이터 참조 정적 계약과 C# preflight **7파일**을 재실행해 통과했다.
  기존 `step8-online-after-raw.json`을 restart 자기 비교로 검증해 유효 Production 기준임을 재확인했다.
  자기 비교는 새 실제 재실행 검증이 아니다. current baseline은 9-2에서 다시 확보한다.
- restart 도구는 동일 계정/공개 번호·전체 저장·개인 최고/Pending·Settings/Binding/Tutorial을 비교한다.
  server-restart는 C·고정 owner·활성 Player/revision·두 행의 score와 전체 metadata(수락 시각/제출 ID 포함)를 비교한다.
  두 비교를 혼동하지 않으며 화면의 같은 점수만으로 서버 시각 보존을 판정하지 않는다.
- 기존 scope/codec/migration 자동 테스트를 정적으로 대조하고 사용자 두 profile 전체 Test 성공 근거를 유지했다.
  현 QA 상태는 처음부터 준비한 v7이며 이 계정의 실제 legacy service migration 비교는 **N/A**다.
  오래된 운영 자료가 없는데 구버전 저장/기록을 만들어 재현하지 않는다. 구버전 정책의 자동 검증 근거와 구분한다.
- raw 자료는 `Ignore/Phase5Private`에만 보관한다. before/after manifest를 비공개 위치에 준비했고 값 비교/manifest 편집을 사용자에게 맡기지 않는다.
  runtime/Module/Scene 변경·Unity Build/Runner·실제 UGS 요청은 하지 않았다. 재게시·재빌드는 필요 없다.

### 9-2 사용자: 재실행 전 기준 확보 / AI: 검증 — 완료 (2026-11-09)

1. Production QA 출발의 같은 Player를 종료 상태로 유지한다. Step 8 뒤 추가 플레이/설정 변경/이전은 하지 않는다.
2. 해당 사용자 `%USERPROFILE%\AppData\LocalLow\DefaultCompany\Unity_Flow_State\flow-state-save.json`을
   `C:\Unity\Unity_Flow_State\Ignore\Phase5Private\step9-before-raw.json`으로 복사한다. 원본은 수정/이동하지 않는다.
3. 승인된 UGS Project의 **production** Dashboard에서 읽기 전용으로 본인 계정을 찾는다.
   Cloud Save Game Data / Private Custom의 `fs8-number-<본인 공개 번호>` → `fs_account_v1` value에서 accountId를 로컬에서 확인하고,
   `fs8-account-<그 accountId>`의 `fs_account_v1` **value JSON만** `step9-account-before.json`으로 보관한다.
   이 값의 leaderboardOwnerId가 두 보드의 고정 본인 행 소유 ID다. 로컬 저장의 accountId와 서버 논리 C를 혼동하지 않는다.
4. Leaderboards에서 **fs-stage-stage-001-r1**, **fs-infinite-v2**의 위 고정 owner 행을 각각 읽기 전용으로 보관한다.
   파일명은 `step9-stage-before.json`, `step9-infinite-before.json`이다.
   각 파일은 단일 행 `playerId`, `score`, `metadata`(acceptedAt/submissionId 포함) JSON이어야 한다.
   목록 전체나 인증값·write lock·이전 자격 증명을 복사하지 않는다. 자료는 모두 위 비공개 폴더에 보관한다.
5. 현재 두 Mode 행은 이미 확인됐으므로 null로 대체하지 않는다. 대상/형식이 불명확하면 임의 편집 없이 알려준다.
   Dashboard 직접 자료 편집/서버 호출/권한 추가·Secret 입력은 하지 않는다. 파일 내용/식별자를 채팅에 붙이지 않는다.
6. 4파일 준비 여부만 보고한다. AI가 로컬/서버 기준의 Production scope·동일 계정·두 행/시각/ID 유효성을 검증한 뒤 재실행을 안내한다.
   파일명 충돌 시 덮어쓰지 않는다. 게임 재실행보다 이 기준 확보가 먼저다.

9-2 자료 검사 결과(2026-11-09): 사용자 4파일 준비 보고 후 로컬 raw의 유효 Production v7 capture를 통과해
`step9-before-validated.json`을 별도 생성했다. 서버 3파일은 실제 `.json.txt` 확장자였으며 이름/원문은 변경하지 않았다.
계정 파일은 JSON·Production scope·Active·필수 필드 형식이 유효하다. 두 행 파일은 JSON 해석 실패이며
playerId/score/metadata 필드명을 찾을 수 없어 고정 본인 행/점수/수락 metadata를 검증할 수 없다.
원문을 출력하지 않았고 잘못된 자료를 서버 baseline 성공으로 처리하지 않았다.
두 행은 기존 원문을 보존하고 단일 완전한 행 JSON으로 `step9-stage-row-before.json`,
`step9-infinite-row-before.json`을 추가 확보한다. AI가 실제 준비된 파일명에 맞춰 manifest를 연결한다.
9-3 재실행은 아직 대기다. 점수/행 ID를 추정하거나 metadata만으로 누락된 필드를 만들어 채우지 않는다.

추가 자료 검증 결과(2026-11-09): 두 `*-row-before.json`은 유효 단일 행이며 score와 acceptedAt/submissionId가 유효하다.
실제 파일을 참조하는 before manifest로 capture를 통과해 `step9-server-before-validated.json`을 생성했다.
기존 `step9-before-validated.json`과 서버 자료의 Production project/environment·활성 Player·공개 번호가 일치하고,
두 행이 같은 논리 계정의 고정 owner에 귀속되며 로컬 Pending 0건임을 확인했다. 원문/식별자를 출력하지 않았다.
9-2 기준 검증은 완료이며 이전 형식 실패 자료/검증 복사본은 덮어쓰거나 삭제하지 않았다.

### 9-3 사용자: 같은 계정 재실행·적용 상태 확인 — 완료 (2026-11-09)

1. 네트워크 연결 상태에서 같은 출발 사용자·Player를 기본 실행한다. 인증/저장을 초기화하거나 명령행 profile을 변경하지 않는다.
2. Settings → Account / Device Transfer와 Stage/Infinite의 TOP·AROUND YOU를 조회한다.
   기존 공개 번호·본인 표시·기록이 유지되고 Pending이 없는지 로컬에서 확인한다. 필요시 조회 Retry만 사용한다.
3. Settings·기존 입력 조작이 이전 상태로 적용되는지 확인하고 튜토리얼이 이전 완료 상태와 다르게 자동 반복되는지 확인한다.
   시험을 위해 설정/Binding을 바꾸거나 새 플레이·제출/이전/Reset을 하지 않는다.
4. Player 정상 종료 후 저장을 `step9-after-raw.json`으로 복사한다.
5. 같은 Dashboard 대상의 account value/두 단일 행을 다시 읽어
   `step9-account-after.json`, `step9-stage-after.json`, `step9-infinite-after.json`으로 보관한다.
6. 화면 확인 성공/실패와 4파일 준비 여부만 보고한다. 실제 번호·점수·원문은 공유하지 않는다.

### 9-4 AI: 재실행 전후 정확한 보존 비교 — 완료 (2026-11-09)

- capture 도구와 `restart`/`server-restart` 비교로 같은 계정·전체 저장·두 서버 행/metadata·연결 revision 보존을 판정한다.
  차이가 나면 실패 분류만 보고하고 원본 복원/덮어쓰기·새 제출로 수정하지 않는다.
- 사용자 화면 적용 결과와 실제 legacy N/A를 연결한다. 준비된 서버 baseline은 Step 10 이전 전 자료로 승계하되
  그 사이 쓰기가 발생하면 새 기준이 필요하다. Step 9 통과를 이전 성공/전체 공개 승인으로 확대하지 않는다.
- 사용자 after 4파일 준비 보고를 받아 두 capture를 통과하고 `step9-after-validated.json` /
  `step9-server-after-validated.json`을 별도 생성했다. 기존 baseline/raw를 덮어쓰거나 편집하지 않았다.
- 실제 before/after의 `restart`는 **PRESERVED**다. 같은 계정·공개 번호·전체 저장(개인 최고/Pending·Settings/Binding/Tutorial)이 일치한다.
- 실제 before/after의 `server-restart`도 **PRESERVED**다. 같은 C·고정 owner·활성 Player/연결 revision과
  Stage/Infinite 두 행의 score·전체 metadata(acceptedAt/submissionId 포함)가 보존됐다. 원문/식별자는 출력하지 않았다.
- 9-1~2·9-4는 완료다. 재실행 후 계정/두 목록 정상 조회·설정/입력의 실제 적용·튜토리얼 자동 반복 없음의
  사용자 화면 결과는 파일 준비 보고에 명시되지 않아 확인 대기로 남긴다. 저장 동일성과 실제 화면 적용을 혼동하지 않는다.
  이미 확인한 결과 보고만 요청하며 추가 플레이·설정 변경·재시작/자료 재수집은 요구하지 않는다.

### 9-5 사용자 화면 결과·Step 9 완료 — 2026-11-09

- 사용자가 재실행 후 계정 화면/Stage·Infinite 정상 조회, 기존 설정·입력 정상 적용,
  완료한 튜토리얼 자동 반복 없음의 세 조건 모두 정상을 확인했다. 9-3 결과 대기를 해소한다.
- 실제 before/after의 로컬 `restart`·서버 `server-restart` **PRESERVED**와 사용자 화면 결과를 연결해 Step 9을 완료한다.
  개인 최고/Pending·동일 계정/공개 번호·Settings/Binding/Tutorial·두 서버 행/수락 metadata·활성 연결 revision 보존이 근거다.
- 현 QA 계정은 새 v7 상태여서 실제 legacy service migration 비교는 N/A이며 구버전 자료를 조작하지 않았다.
  이전의 미완료/화면 대기 문구는 당시 이력이다. 다음은 Step 10이며 이전 성공/일반 공개 승인을 뜻하지 않는다.
- Step 9 after 검증 기준은 같은 출발의 이전 전 자료로 보관한다. 그 사이 새 제출/연결 변경이 있으면 비교 기준을 다시 확정한다.
  이번 결과 반영은 문서만 수정했으며 추가 플레이·원격 요청·Build/Test Runner·Scene/Module 변경은 하지 않았다.

완료 조건: [x] 동일 Production QA 계정 재연결·기록/Pending 보존. 실제 legacy 자료 여부와 N/A 근거 구분.

## Step 10. QA A→B 이전과 기록 보존을 확인한다

착수 조건: Step 1에서 승인한 두 QA 계정 생성·연결 변경·한 번의 정상 이전, Windows 사용자별 인증/저장 격리,
Step 9 출발 before/after 보존 확인·Pending 없는 기준과 비교 도구 준비. 기존 사용자 승인을 반복 요구하지 않는다.

### 10-1 AI: 승인·기준·UI·로컬 이전 검증 — 완료 (2026-11-09)

- 출발은 Step 6 최초 Notice 확인 이후 Step 7~9에서 유지한 같은 신규 게임 계정이다.
  Step 9 after 검증 저장/서버 snapshot을 출발 이전 전 기준으로 승계한다. 그 후 쓰기가 있었다면 새 기준부터 확인한다.
  이전에 보관했던 원래 출발 상태를 복원/혼합하지 않는다.
- 이전 정책/실제 endpoint 대역·클라이언트 계약·도구 **12파일 실패 0**과 C# preflight **7파일**을 확인했다.
  start 20/20·reissue 24/24·lifecycle 22/22·completion 22/22·races 22/22 대역을 포함한다.
  기존 Step 9 서버 before/after 재비교도 PRESERVED다. 새 Unity 컴파일/Test Runner·실제 UGS 요청은 수행하지 않았다.
- Scene은 읽기 전용으로 확인했다. 실제 문구는 Settings → **Account / Device Transfer**,
  **Start Transfer**, **Enter Transfer Code**, **Complete Transfer**, **Refresh Status**,
  **Copy Transfer Code**, **Copy Verification Value**다. Scene 변경은 필요 없다.
- 성공 후 수신은 출발의 C/공개 번호/서버 최고 전체를 적용한다. 수신의 이전 로컬 최고를 합산하지 않는다.
  출발의 기존 Player 연결은 Inactive가 되며 출발 UI가 새 Anonymous로 전환될 수 있다. 그 새 번호를 이전 실패로 오판하지 않는다.
  수신이 원래 사용하던 C는 Detached로 보존되고 그 서버 행을 삭제/병합하지 않는다.
- 기존 비교 도구는 Active server-restart만 지원해 원래 수신의 정상 Detached 상태를 검사할 수 없었다.
  로컬 도구에 `server-detached` capture·`recipient-detached` 비교와 회귀 사례를 추가했다.
  Production 도구 **10/10** 통과. 원래 C/번호/owner·두 행 전체 metadata 보존 및 Detached/revision+1을 검사하고
  잘못된 상태·번호/귀속/점수/시각/제출 ID 변경·행 소실을 거부한다. 런타임/Module 변경이 아니므로 재빌드·재게시하지 않는다.
- 이전 code/9자리 검증값은 표시/입력에만 사용하고 채팅·Git·비공개 snapshot·스크린샷·로그에도 보관하지 않는다.
  인증 캐시/Local Save를 Windows 사용자 간 복사하지 않는다. 비교용 원문은 Ignore/Phase5Private에만 보관한다.

### 10-2 사용자: Production QA 수신 준비 — 공개 번호 준비 대기

1. 별도 Windows 표준 사용자 **Production QA 수신**으로 로그인한다. 다른 사용자 자격 증명의 관리자 실행은 하지 않는다.
2. 같은 승인된 Production Player 빌드 출력 **전체 폴더**를 수신이 읽을 수 있는 위치에서 실행한다.
   접근이 안 되면 실행 파일만이 아니라 Data/DLL을 포함한 출력 폴더만 복사한다. 출발의 AppData/저장/인증 캐시는 복사하지 않는다.
3. 최초 안내를 거쳐 Main Menu → Settings → Online Record Notice → Continue로 동의한다.
   기존 동의가 있으면 초기화하지 않고 그대로 사용한다.
4. Settings → Account / Device Transfer에서 공개 번호 준비를 확인한다. 필요하면 Refresh Status를 한 번 사용한다.
   출발과 다른 번호이며 오류/처리 중이 아닌지 로컬에서만 대조한다. 실제 번호/ID는 보고하지 않는다.
5. Pending이 없음을 확인한다. Stage/Infinite를 플레이하지 않는다. 기존 승인 QA 기록이 있다면 삭제하지 말고 존재 여부만 보고한다.
   Pending이 있으면 폐기/추가 제출/이전하지 말고 상태를 먼저 알려준다.
6. Player 정상 종료 후 수신 사용자의 Local Save를 **step10-recipient-before-raw.json**으로 비공개 폴더에 복사한다.
   위치는 `C:\Unity\Unity_Flow_State\Ignore\Phase5Private`이며 원본은 각 사용자의
   `%USERPROFILE%\AppData\LocalLow\DefaultCompany\Unity_Flow_State\flow-state-save.json`이다.
   접근이 안 되면 관리자 자격으로 Player를 바꾸지 말고 알려준다. 파일 충돌 시 덮어쓰지 않는다.
7. 번호 준비/출발과 다름·Pending 유무·기존 기록 유무·복사본 준비 여부만 보고한다.
   **Start Transfer/Enter Transfer Code/Complete Transfer는 아직 실행하지 않는다.**

10-2 사용자 관찰·raw 검사(2026-11-09): 수신에서 Candidate01 실행·Leaderboard 공개 번호 확인 불가·파일 복사를 보고했다.
AI가 `step10-recipient-before-raw.json`을 읽기 전용으로 확인했다. v7·Production project/environment·동의 true·온라인 Player 값 존재,
출발과 다른 로컬 owner/Player·Pending 0건·로컬 개인 최고 0건이다. 유효한 10자리 공개 번호 캐시는 없다.
이는 번호 준비 완료나 서버 계정/행 부재를 확인한 자료가 아니므로 valid baseline capture/10-2 완료 처리를 하지 않는다.
Candidate01은 Step 4에서 예시로 사용한 이름이지만 폴더 이름만으로 구버전이라고 확정할 수 없다.
Step 6 수정 후 출발의 Step 7~9에 사용한 동일 출력물인지 사용자 확인이 필요하며 재빌드를 요구하지 않는다.
다음은 수신 Settings → Account / Device Transfer에서 준비 결과를 확인하고 필요시 Refresh Status 한 번만 사용하는 것이다.
실패가 유지되면 안전한 상태/오류만 보고하며 Stage 플레이·초기화·새 계정 생성/이전·반복 재시도는 하지 않는다.
준비가 성공하면 기존 실패 상태 raw를 덮어쓰지 않고 **step10-recipient-ready-raw.json**으로 새 복사본을 확보한다.
최초 조회 지연의 기존 보류 결정을 변경하거나 원인을 단정하지 않되 실제 번호 준비 전에는 이전을 시작하지 않는다.

10-2 후속 사용자 결과(2026-11-09): Candidate01은 출발 Step 7~9의 같은 빌드임을 확인했다.
수신 공개 번호는 계속 unavailable이며 `Could not verify the request. Select Refresh Status. Offline play is still available.`이 표시됐다.
빌드 차이를 원인으로 추정하지 않는다. 이 문구는 Controller의 예외 catch/여러 실패 응답 기본 분기에서 공통 사용하므로
번호 미발급·계정 부재·서버 지연·클라이언트 오류 중 어느 하나로 확정하지 않는다.
코드상 계정 패널은 인증+상태+번호 전체에 5초 budget을 사용하며 transport도 별도 5초 제한이 있다.
이는 가능한 실패 경로일 뿐 이번 Timeout의 실제 증거는 아니다. LastDiagnostic의 모든 값이 Player.log에 남는 구조도 아니다.
추가 Refresh/재실행·계정 초기화·Stage 플레이·이전 발급/완료는 중단하고 현재 실행의 Player.log를
비공개 **step10-recipient-unavailable-Player.log**로 복사해 기존 안전한 인증/SDK/late failure 진단 유무부터 AI가 조사한다.
로그 원문은 채팅/Git에 올리지 않는다. Player 종료 후 새 실행 전에 복사하며 파일이 없으면 추가 실행 대신 알려준다.
수신 번호 준비를 확인하기 전 Step 10-2/전체 Step은 미완료다. 이번 결과 반영에서 runtime/Module/Scene은 수정하지 않았다.

10-2 Player.log 확인(2026-11-09): 비공개 로그 42행에서 인증 실패/late request 실패·Coordinator 경고·Timeout/예외·네트워크 실패 흔적을 찾지 못했다.
원문/경로/식별자는 출력하지 않았다. 실패를 정상으로 판정하거나 로그 부재로 SDK/서버 원인을 배제하지 않는다.
Client LastDiagnostic/일부 실패 reason은 로그로 출력하지 않는 코드이므로 이번 로그는 원인 판별에 충분하지 않다.
추가 요청 없이 서버 준비 상태를 구분하도록 raw의 수신 Player를 사용한 비공개 **step10-recipient-dashboard-lookup.json**을 생성했다.
파일은 production·playerBindingContainer·key·진단 export 파일명만 포함하며 값은 채팅/Git에 올리지 않는다.
사용자는 그 파일의 대상 container를 Dashboard Private Custom에서 읽기 전용으로 찾아 `fs_account_v1` value를
**step10-recipient-binding.json**으로 보관한다. 연결표가 있으면 accountId가 가리키는 `fs8-account-<accountId>`의 같은 key value를
**step10-recipient-account-diagnostic.json**으로 보관한다. 값 비교/상태 판정은 AI가 한다.
연결표/계정이 없으면 부재만 보고하며 수동 생성/수정·새 계정/재시도·권한 추가는 하지 않는다.
이 자료는 원인 구분용이며 번호 준비 성공 baseline/10-2 완료로 취급하지 않는다. 출발 자료/인증은 변경하지 않는다.

10-2 연결표/누락 계정 진단(2026-11-09): 사용자 binding 파일 준비·그 accountId가 가리키는 account container 미발견을 보고했다.
AI가 파일에서 Production project/environment·수신 Player 일치·schemaVersion 1·revision 유효·generatedOwner 존재를 확인했다.
status 필드와 transferOperation은 없다. 이는 provisioning의 최초 binding 생성 형식과 일치하며 Inactive 연결표로 분류하지 않는다.
실제 account 부재는 사용자 Dashboard 관찰이며 AI가 원격 조회해 확정한 것은 아니다. 원문/내부 ID는 기록하지 않는다.

현 `get-account-transfer-status.js`는 binding이 **없을 때만** provisioning.ensure를 호출한다.
초기 binding은 있고 account가 없으면 ensure를 건너뛰고 completion.recover가 AccountUnavailable로 실패한다.
endpoint는 이를 TransientFailure/TransferUnavailable로 반환한다. Client는 상태 성공 전 번호 요청을 하지 않아 같은 경로에 머물 수 있다.
실제 endpoint source와 합성 SDK 대역으로 같은 초기 binding/account 부재를 3회 호출해
**모두 TransferUnavailable·provisioning 호출 0·쓰기 0**을 재현했다. 실제 Production 요청/변경은 없었다.
따라서 이 초기 상태의 자동 복구 경로 누락은 코드상 확인했으나 최초 provisioning 중단의 원인/시점은 미확인이다.
앞서 보류한 조회 속도 문제의 원인을 확정하거나 그 승인을 이번 계정 준비 실패로 확대하지 않는다.
수정 방향은 검증된 초기 binding의 누락/Preparing 계정만 기존 provisioning 경로로 안전하게 재개하고
Inactive·이전 진행 중·다른 계정/범위의 연결은 재생성하지 않는 것이다. 아직 서버/runtime source는 수정하지 않았다.
다음은 사용자 서버 코드 수정 지시 후 회귀 사례/정적·대역 검증 및 사용자 변경 Module 게시 절차다.
Dashboard 수동 account 생성/삭제·연결표 편집·초기화·재시도 반복·이전 진행은 하지 않는다. Step 10-2/전체는 미완료다.

#### 10-2-1 초기 계정 준비 재개 수정·검증 — 완료 (2026-11-09)

- 사용자 "문제를 해결하기 위한 작업" 지시에 따라 `UGS/CloudCode/get-account-transfer-status.js`를 수정했다.
  초기 binding이 남아 있으면 Player/UUID v4 계정·generatedOwner/최초 revision/상태·작업 경계를 검사하고,
  account가 없거나 같은 초기 소유권의 Preparing일 때 기존 provisioning.ensure를 재개한다.
  새 binding/계정 ID를 생성해 기존 연결을 대체하지 않으며 이미 발급된 번호·legacy 행과 metadata는 기존 계약대로 보존한다.
- Inactive의 기존 빠른 조회·이전 완료 복구는 유지한다. Active/교체 binding·다른 revision/소유권/환경·진행 중 이전/제출,
  읽기 예외 403/404/503은 초기 부재로 오인해 provisioning을 시작하지 않는다. 5초 제한/Secret/정책은 변경하지 않았다.
- 새 `account-status-provisioning-recovery.test.cjs` **8/8** 통과. 실제 status endpoint와 provisioning을 SDK 대역으로 실행하여
  최초 binding 복구·Preparing/할당 번호 보존·legacy 행 보존·동시 재개·경계 거부를 검증했다.
  **17개 쓰기 경계 × before/after commit = 34개 중단 지점**을 모두 status 재요청으로 복구하고 반복 완료 후 쓰기 없음을 확인했다.
- 기존 `transfer-completion.test.cjs`의 endpoint 대역 require 경계만 새 정책 참조에 맞췄고 **22/22** 통과했다.
  전체 Node `*.test.cjs` **47파일 실패 0**(Node runner 71 항목 통과), static contracts·Cloud Save 참조·Production inventory·
  Access Control/Secret 경계·C# preflight **7파일** 및 변경 JS 문법을 통과했다. 원격/Unity compiler/Runner를 실행한 결과는 아니다.
- 실제 수신 binding의 형태가 새 초기 재개 조건에 맞는지도 읽기 전용으로 확인했다. 실제 account 생성/번호 준비 성공은 아직 미확인이다.
  최초 provisioning 중단 원인은 미확인으로 유지하며 이 수정이 최초 서비스 지연 원인을 해결했다고 주장하지 않는다.
- 이번 추가 서버 변경은 status endpoint **1파일**이다. 기존 query-records 수정은 앞서 게시/확인한 이력이며 이번 수정과 구분한다.
  status source SHA-256: `FD1E39DF5CEF0F1BFF425BB7852D6D904B7EA3C50B556195A3911FA9712F66EC`.
  JS가 Module에 embedded되므로 변경 Module 게시가 필요하다. Unity Client/Scene 변경은 없어 **새 Player Build·Unity 전체 Test 재실행은 요구하지 않는다**.
  앞선 source 미수정/수정 지시 대기 문구는 당시 이력이다.

#### 10-2-2 사용자: 변경 Module 게시·같은 수신 재확인 — 대기

1. 기존 검증/게시 절차로 최신 status source가 포함되도록 같은 **FlowStateVerification.ccmr** Module을 빌드·게시한다.
   명시적으로 production 대상을 확인한다. AI가 Module/Unity 빌드·원격 게시를 수행하지 않는다.
   Secret 재발급·서버 account/binding 수동 생성/수정/삭제·인증/Local Save 초기화는 하지 않는다.
2. 같은 Candidate01 Player·Production QA 수신 사용자·기존 인증/Local Save를 유지한다. 새 빌드를 만들거나 계정을 새로 만들지 않는다.
3. Settings → Account / Device Transfer를 열어 번호 준비를 확인한다. 아직 준비되지 않았으면 **Refresh Status 한 번**만 사용한다.
   계속 실패하면 안전한 문구만 보고하고 반복/플레이/이전으로 우회하지 않는다.
4. 준비 성공 시 공개 번호가 출발과 다른지 로컬에서만 확인하고 게임을 정상 종료한 뒤 저장을
   **step10-recipient-ready-raw.json**으로 새 복사한다. 기존 before raw/binding/log 원문은 보존한다.
5. Module 게시 결과·번호 준비/출발과 다름·안전한 오류·ready raw 준비 여부만 보고한다.
   실제 번호/인증값은 공유하지 않는다. 준비 완료와 양쪽 기준 검증 전 Start Transfer/Complete Transfer는 하지 않는다.
6. 실제 수신 번호 준비/기준 확인 전 10-2 전체와 Step 10은 미완료다.

#### 10-2-3 수정 Module 게시 후 실패 지속·추가 진단 — 2026-11-09

- 사용자가 변경 Module 빌드/production 게시 후 같은 수신 Player 실행에도 번호 unavailable·기대 account 미생성을 보고했다.
  실제 서비스 문제가 해결됐다고 판정하지 않는다. 이전 코드 발급/완료는 아직 진행하지 않는다.
- AI가 로컬 `bin/Release/net9.0/linux-x64/FlowStateVerification.dll`을 바이너리 읽기 전용 검사해
  현재 status endpoint source 전체와 새 재개 marker의 embedded 포함을 확인했다.
  이는 로컬 artifact 확인이며 원격 활성 DLL과 동일함을 직접 검증한 것은 아니다. 같은 source를 무근거로 다시 게시하도록 요구하지 않는다.
- provisioning은 account 생성 전에 `legacy.freeze`에서 Protected ledger/guard와 두 보드의 기존 score/보드 존재를 조회한다.
  binding 재개를 수정해도 이 단계의 API/권한/입력/시간 실패면 account는 생성되지 않을 수 있다.
  5초 budget·이전 Protected 자료의 어떤 항목이 실제 원인인지는 아직 미확인이다. 관측 없이 추가 서버 수정을 하지 않는다.
- 추가 원격 요청/Refresh·초기화 없이 읽기 전용 상태를 확보하도록 비공개 **step10-recipient-protected-lookup.json**을 준비했다.
  사용자 Dashboard production → Cloud Save **Player Data / Protected**에서 해당 Player의
  `fs_submission_ledger_v1` value와 `fs_creation_guard_v1` value를 각각
  **step10-recipient-protected-ledger.json**, **step10-recipient-protected-guard.json**으로 보관한다.
  값은 Ignore/Phase5Private에만 두고 채팅/Git/로그에 올리지 않는다. 키가 없거나 Player를 찾지 못하면 부재만 보고한다.
- AI가 두 자료의 schema/migration 대상·scope·진행 상태를 비교한다. 사용자는 값을 수정하거나 서버 key/container를 만들지 않는다.
  기존 source/raw/binding/진단 로그는 보존하며 이번 진단 인계에서는 runtime/Module/Scene을 추가 수정하지 않았다.
  현재 10-2/Step 10 전체는 실제 수신 계정 준비 실패로 미완료다.

#### 10-2-4 Protected 자료 결과·일반 상태 경계 검증 — 2026-11-09

- 사용자가 Protected ledger/guard 파일 준비를 보고했고 특정 ID/번호만 처리하지 않는 일반적 수정을 명시했다.
  실제 Player/계정/번호를 production 조건문/테스트에 하드코딩하지 않는다. 최초 revision 등 상태 계약은 실제 식별값 예외와 구분한다.
- guard의 Production scope/schema는 유효하다. ledger는 v1·active 빈 문자열·entries/best 0건·migration 존재·hadLegacyLedger=false다.
  v1 ledger scope는 top-level이 아니라 migration에 있으며 해당 project/environment는 유효하다.
- **migration.accountId가 기존 수신 binding.accountId와 다르다.** 양쪽은 유효 UUID 형태지만 출발 C나 수신 로컬 owner와도 혼동하지 않는다.
  두 export가 같은 실제 수신 Player의 최신 자료라면 legacy.freeze의 기존 LedgerConflict 거부 조건과 직접 일치한다.
  빈 ledger라도 권한 근거인 migration 대상을 임의 변경/무시하거나 새 binding으로 재귀속하지 않는다.
  이 불일치가 생긴 시점/원인은 아직 미확인이다. 자료 대상 혼동/오래된 export 가능성도 최신 연결표로 구분해야 한다.
- 회귀에 **서로 다른 합성 Player·계정/owner·Project/Environment 6조합 × 정상/불일치 2경우**를 추가했다.
  같은 계정 marker는 정상 준비하고 다른 계정 marker는 반복 요청에도 쓰기 0/자료 불변으로 거부함을 검증했다.
  status 준비 회귀는 최신 **9/9**, 전체 Node **47파일 실패 0**(runner 72항목)이다. 실제 식별자/자료를 테스트에 사용하지 않았다.
- 이번 추가 변경은 테스트뿐이며 production source/Module/Client/Scene은 더 수정하지 않았다. 재게시/Player Build/Unity Test Runner는 요구하지 않는다.
- 비공개 **step10-recipient-migration-lookup.json**에 같은 수신의 연결표 container와 marker가 가리키는 account container를 준비했다.
  사용자 read-only로 현재 연결표 value를 **step10-recipient-binding-current.json**,
  migration 대상 account의 같은 key value를 **step10-recipient-migration-account.json**으로 보관한다.
  없으면 부재만 보고하며 생성/삭제/편집/초기화·재시도·이전으로 우회하지 않는다. 원문은 채팅/Git에 올리지 않는다.
- 최신 원본 연결과 migration 대상 계정의 상태/소유권을 확인하기 전 수동 자료 복구나 추가 서버 복구 정책을 임의 결정하지 않는다.
  10-2/Step 10 전체는 아직 미완료다.

#### 10-2-5 최신 자료 관계 확인·복구 범위 구분 — 2026-11-09

- 사용자가 최신 binding/migration account 2파일 준비를 보고했다. AI가 비공개로 범위·소유권·참조 관계를 비교했다.
  최신 binding은 이전 export와 같은 수신 Player/계정/generatedOwner·초기 revision이며 migration 대상과 여전히 다르다.
- migration 대상 account는 같은 production·같은 수신 Player를 initial/current Player로 가진 **Preparing/revision 1**이다.
  유효 공개 번호가 이미 발급돼 있고 UUID 계정/고정 owner가 유효하며 transfer/onlineOperation은 없다.
  account.migration.ledger는 이번 Protected ledger와 **전체 일치**하고 account.migration.scores는 비어 있다.
  따라서 오래된 binding export만의 문제가 아니며 확보한 자료들에서 같은 수신의 초기 연결 참조 불일치가 유지됨을 확인했다.
  이 불일치가 발생한 원인/시점·원격 저장 동시성 문제 여부는 아직 미확인이다.
- 기존 초기 재개 수정은 binding이 가리키는 계정만 재개한다. 현재는 그 경로와 Protected marker가 다른 C를 가리켜
  LedgerConflict로 중단된다. 단순 지연으로 분류하지 않으며 반복 요청/새 계정/번호 재발급으로 우회하지 않는다.
- 추가 복구는 이미 존재하는 같은 수신의 Preparing 계정·기발급 번호·고정 owner·원본 snapshot을 보존하고,
  서버가 같은 Player/범위·초기 상태·경합/이전/제출 부재·원본 일치를 재검증한 경우에만 초기 binding 참조를 CAS로 복구하는 정책이다.
  Active/Detached/이전 진행 계정·다른 Player/환경·불일치 snapshot을 자동 연결하지 않는 범용 조건과 회귀가 필요하다.
  이는 단순 account 생성 재개를 넘어 저장된 연결 참조를 바꾸는 정책이므로 사용자 명시 지시 전 추가 구현/적용하지 않는다.
- 이번 작업은 읽기 전용 관계 확인·문서 갱신뿐이며 production/runtime/Module source 추가 변경·재게시/Build/원격 호출은 없다.
  Dashboard 직접 편집/삭제·migration marker 무시·저장/인증 초기화도 하지 않는다. Step 10 전체는 미완료다.

#### 10-2-6 승인된 범용 초기 연결 참조 복구 구현·검증 — 완료 (2026-11-09)

- 사용자 "작업을 수행한다"로 10-2-5 복구 범위 승인을 받고 구현했다. 특정 실제 계정/Player/번호를 코드 조건이나 테스트에 넣지 않았다.
- `legacy-account-store.js`에 현재 인증 Player의 frozen ledger를 읽기 전용으로 반환하는 내부 메서드를 추가했다.
  `account-provisioning-service.js`는 현재 초기 binding의 계정이 없고 freeze에서 LedgerConflict가 발생할 때만 복구 후보를 검사한다.
  서버 marker가 가리키는 같은 Player/환경의 Preparing·revision 1, 이전/제출 작업/연결 이력 부재를 확인한다.
- 원본 Protected ledger/보드 snapshot 전체 일치, destination ledger·기발급 번호/owner 매핑·현재 임시 owner 매핑을 검증한다.
  존재하는 현재 계정/다른 소유권/읽기 실패는 거부한다. CAS 직전 source/account/binding 내용과 write lock·현재 대상 부재를 재확인한다.
  **binding만 CAS로 기존 Preparing 계정에 맞추고 marker는 수정하지 않는다.** 기존 provisioning이 활성화를 이어간다.
- 기존 번호·고정 owner·private ledger·원본 Protected 자료·서버 행/전체 metadata를 보존한다.
  Active/Detached/이전 중/다른 Player/범위·snapshot 불일치·관측 중 변경된 자료를 자동 연결하지 않는다.
  모든 container의 원자적 트랜잭션이나 실제 서비스 동시성까지 검증한 것으로 주장하지 않는다.
- 새 `account-initial-binding-recovery.test.cjs` **10/10** 통과:
  서로 다른 합성 Player/계정/Project/Environment 6조합의 신규/legacy 기록 보존,
  기발급 번호 유지·새 ID/번호 미발급·양쪽 쓰기 전후 중단 복구·동시 복구 수렴·반복 완료 무쓰기,
  소유권/상태/매핑/원본 충돌 및 403/404/503 거부, Inactive로의 CAS 경합·source 변경 거부,
  번호 발급 전/ledger 생성 전 중단·JSON key 순서·실제 embedded JS 로더의 Node VM 실행을 포함한다.
- 기존 status 준비 회귀 **9/9** 유지, 전체 Node **48파일 실패 0**(runner 82항목),
  static/Cloud Save/Access Control 계약·C# source preflight **7파일**·JS 문법·문서 diff 검사를 통과했다.
  실제 Production 호출·지연/권한·.NET/Jint 실행·Unity 컴파일/Test Runner/빌드의 성공 근거는 아니다.
- 이번 추가 Module source 변경은 **account-provisioning-service.js / legacy-account-store.js 두 파일**이다.
  SHA-256은 각각 `0BCF0A2362285F9A2247A22D1DC9FDE6EC33E5CAA2D0F470627E622427F9E170` /
  `AFF40BE3016C8313BC705CE0B6A78E5CB2311BD46A01E78C98B8BFD370A2575E`다.
  Client/C# Module host/Scene·시간 제한·Secret/Access Control 변경은 없어 같은 Player를 유지한다.
  앞선 승인 대기/source 추가 수정 없음은 당시 이력이다. 실제 수신 번호 준비와 Step 10 전체는 아직 미완료다.

#### 10-2-7 사용자: 변경 Module 게시·같은 수신 재확인 — 번호 준비 확인 완료 (2026-11-09)

1. 최신 두 JS source가 포함되도록 기존 **FlowStateVerification.ccmr** Module을 사용자 빌드·production 게시한다.
   AI는 Module/Unity 빌드·원격 배포를 하지 않는다. **새 Player Build·Unity Test Runner 재실행은 요구하지 않는다.**
2. 같은 Production QA 수신 Windows 사용자·Candidate01·기존 인증/저장으로 실행한다.
   Settings → Account / Device Transfer에서 번호 준비를 확인하고 필요하면 Refresh Status **한 번**만 사용한다.
   실패하면 안전한 상태만 보고하고 초기화·새 계정 생성·Stage 플레이·반복 재시도·이전으로 우회하지 않는다.
3. 이번 성공 조건은 처음 binding이 가리키던 미생성 계정을 새로 만드는 것이 아니라,
   **이미 번호가 발급된 migration 대상 Preparing 계정으로 초기 연결을 복구**하는 것이다.
   따라서 처음 찾지 못했던 임시 account container가 계속 없는 것은 실패 근거가 아니다.
4. 번호 준비가 되면 출발과 다른 번호인지 로컬에서 확인하고 Player를 정상 종료한 뒤,
   같은 저장 파일을 **step10-recipient-recovered-raw.json**으로 새 복사한다. 이전 raw/로그/서버 자료를 덮어쓰지 않는다.
5. 게시 결과·수신 번호 준비·출발과 다름/안전한 실패·복사본 준비만 보고한다. 실제 번호/이전 코드는 공유하지 않는다.
   AI가 기존 migration 계정/번호와 일치하는지 검증한 뒤 10-3의 이전 전 서버 기준을 다시 안내한다.
   수신 준비/양쪽 기준 검증 전 Start Transfer/Complete Transfer는 하지 않는다.

사용자 수신 공개 번호 확인·`step10-recipient-recovered-raw.json` 복사를 보고받았다.
AI 오프라인 대조에서 최초 수신 raw 대비 변경 필드는 publicPlayerNumber뿐이며, 인증 Player/로컬 owner와 나머지 저장은 유지됐다.
기존 migration 계정의 기발급 번호와 일치하고 출발 Player/번호와 다르며 Pending 0·개인 최고 0이다.
Production v7 capture를 통과해 `step10-recipient-before-validated.json`을 별도로 생성했다. 원본/기존 자료는 덮어쓰지 않았다.
이는 번호 준비 근거이며 현재 서버 Active 상태·연결 복구의 전체 쓰기 결과는 다음 서버 export로 확인한다.
실제 Module 게시 버전/시각이나 원격 DLL 동일성을 이 보고로 추정하지 않는다.

### 10-3 사용자 자료 확보 / AI: 양쪽 이전 전 기준 검증 — 완료 (2026-11-09)

- AI가 수신 raw의 Production scope·출발과 다른 Player/번호·Pending 0을 검사한다.
  출발 Step 9 after 기준은 그대로 유지한다. 수신 기존 서버 C/두 행 유무를 읽기 전용으로 캡처한다.
  행이 없으면 확인 근거와 함께 null로 기록하고 Stage/Infinite 제출로 채우지 않는다.
- 수신 기존 account value와 존재하는 본인 행 전체를 비공개로 확보하며 파일명/manifest는 준비 결과에 맞춰 AI가 안내/작성한다.
  기존 행 metadata/서버 수락 시각·원래 연결 revision을 포함한 기준이 유효해진 뒤에만 실제 이전을 시작한다.

현재 필요한 사용자 자료:

1. production Cloud Save의 Private Custom Data에서 비공개 `step10-recipient-migration-lookup.json`의
   `migrationAccountContainer`를 열어 **현재** `fs_account_v1` value를 `step10-recipient-account-before.json`으로 저장한다.
   이전 Preparing 진단 파일을 재사용하거나 처음 binding이 가리키던 미생성 container를 찾지 않는다.
2. production의 `fs-stage-stage-001-r1`과 `fs-infinite-v2`에서 위 계정의 leaderboardOwnerId에 해당하는 본인 행 유무를 확인한다.
   행이 있으면 playerId/score/metadata를 포함한 전체 행을 각각 `step10-recipient-stage-before.json`,
   `step10-recipient-infinite-before.json`으로 저장한다. 없으면 보드별로 행 없음을 보고하며 파일을 만들려고 플레이하지 않는다.
3. 모두 `Ignore/Phase5Private`에 새 파일로 보관한다. 개인 최고 0이나 TOP에 안 보인다는 사실을 서버 행 부재로 대신하지 않는다.
   AI가 현재 Active/수신 연결/기존 계정·번호·owner와 두 행을 검증하고 실제 확인된 부재만 manifest의 null로 기록한다.
   양쪽 기준 검증 통과 전 Start Transfer/Complete Transfer·새 플레이는 하지 않는다.

10-3 결과:

- 사용자 현재 수신 account value 복사와 미플레이·두 보드 기록 없음 보고를 받았다. 실제 번호는 문서에 기록하지 않는다.
- AI가 production 범위·Active·원래 migration 계정/번호/고정 owner 유지·같은 수신 인증 Player·revision 1·이전 없음,
  출발과 다른 계정/Player/owner 및 양쪽 Pending 0을 오프라인 대조했다.
- 두 보드 부재는 사용자 보고를 근거로 manifest의 null로 기록했다. AI가 원격 보드를 직접 조회했거나 export로 부재를 독립 입증한 것은 아니다.
  `step10-recipient-server-before-manifest.json`과 별도 `step10-recipient-server-before-validated.json`을 비공개로 생성했고 capture를 통과했다.
- 출발은 Step 9 after 기준을 승계한다. 기존 서버 before/after 비교 PRESERVED를 재확인했고 도구 회귀 10/10을 통과했다.
  이후 추가 플레이/제출/이전이 없다는 기존 진행 조건을 유지한다. 새 원격 export로 출발 현재 상태를 다시 조회한 것은 아니다.
- 10-3을 완료하고 정상 이전 1회의 사용자 절차로 진행한다. 실제 이전·이전 후 연결/행 보존·재시작은 미완료다.

### 10-4 사용자: 승인된 정상 이전 한 번 — 화면 전환 확인 (2026-11-11)

1. 출발의 Settings → Account / Device Transfer → **Start Transfer**로 원문을 발급한다.
   코드와 검증값은 사용자 본인이 수신에서만 입력하며 공개 번호를 이전 코드로 사용하지 않는다.
2. 수신의 같은 화면 → **Enter Transfer Code**에서 코드와 9자리 검증값을 입력하고 **Complete Transfer**를 한 번 실행한다.
   동일 PC에서는 Windows 사용자를 전환해 진행하되 출발 표시 원문을 채팅/파일/스크린샷에 저장하지 않는다.
   출발 창을 닫으면 원문을 지우며 재발급은 이전 값을 무효화하므로 임의 반복하지 않는다.
3. 성공/안전한 실패 문구만 보고한다. 응답 미확인이면 새 이전을 발급하거나 Complete를 반복하지 말고
   같은 수신에서 **Refresh Status**로 명시 확인한다. TooManyRequests 안내 시 표시된 제한을 따르고 빠른 반복을 하지 않는다.
4. 수신에서 출발의 기존 번호·Stage/Infinite 본인 행을 확인한다. 원래 수신 최고와 병합되지 않아야 한다.
   출발에서는 Refresh Status로 기존 연결 종료/새 Anonymous 전환 안내를 확인한다. 권한 거부 시험용 제출은 하지 않는다.

10-4 사용자 결과: 수신에서 출발의 기존 공개 번호로 전환됐고 해당 번호의 리더보드 행에 `(You)`가 표시됨을 확인했다.
출발 Refresh Status 후 기존 연결 종료와 새 공개 번호의 새 계정 전환도 확인했다. 실제 번호는 문서에 복사하지 않는다.
이전 결과 화면의 정확한 문구·Stage/Infinite 각각의 확인 여부는 미보고이며 통합 리더보드 보고를 양쪽 모드 완료로 확대하지 않는다.
이전 UI의 주요 연결 전환 근거를 확보했으나 서버 고정 행/metadata·binding 및 재시작 비교 전 Step 10 전체는 미완료다.
새 이전/추가 플레이 없이 아래 자료를 확보한다.

### 10-5 사용자 자료 확보 / AI: 이전 후 보존·연결 대조 — 완료 (2026-11-11)

- 출발 C의 account·두 고정 owner 행 및 **이전 전 출발 Player의 binding**, **수신 Player의 binding**을 읽기 전용 캡처한다.
  `transfer` 비교로 C/번호/고정 행·metadata 불변, 활성 Player 변경·revision+1, 원 출발 Inactive/수신 Active를 확인한다.
  출발 UI가 새 Anonymous로 전환해도 Inactive 비교 대상은 보관한 **이전 전 Player**이며 새 Player와 혼동하지 않는다.
- 원래 수신 C의 Detached account·원래 두 행은 `server-detached` capture와 `recipient-detached` 비교로 확인한다.
  Detached marker가 이번 이전의 transfer ID와 같은지도 AI가 snapshot 안에서 비공개로 대조한다.
  원래 수신의 기존 행은 남아 있어도 현재 수신의 본인 행으로 표시되면 안 된다.
- 수신 Local Save의 서버 최고 적용/계정 cache·Pending 없음과 기기 Settings/Binding/Tutorial 보존은 AI가 비교한다.
  사용자에게 ID/시각/JSON 직접 비교를 시키지 않는다. 도구의 통과만으로 실제 화면 적용을 추정하지 않는다.

10-5-1 서버 자료 확보:

AI가 이전 전 기준에서 정확한 대상만 추출해 비공개 `Ignore/Phase5Private/step10-after-dashboard-lookup.json`을 준비했다.
production Cloud Save → Private Custom Data에서 아래 lookup 필드의 container를 열고 `fs_account_v1`의 현재 value를 새 파일로 저장한다.

| lookup 필드 | 저장할 파일 |
|---|---|
| transferredAccountContainer | step10-account-after.json |
| originalSourceBindingContainer | step10-source-binding-after.json |
| recipientBindingContainer | step10-recipient-binding-after.json |
| detachedRecipientAccountContainer | step10-recipient-detached-account.json |

새 Anonymous 출발 Player의 binding이 아니라 위 originalSourceBindingContainer를 사용한다.
서버 값은 읽기/복사만 하며 수정하지 않는다. 모든 파일은 `Ignore/Phase5Private`에 보관하고 기존 파일을 덮어쓰지 않는다.

10-5-2 리더보드·Local Save 확보:

1. production Stage/Infinite 보드에서 lookup의 transferredLeaderboardOwnerId 행 전체(playerId/score/metadata)를 각각
   `step10-stage-after.json`, `step10-infinite-after.json`으로 저장한다. 이전 파일 재사용 없이 현재 서버 행을 확보한다.
2. lookup의 detachedRecipientLeaderboardOwnerId는 원래 수신 계정이다. 두 보드에서 계속 행이 없는지 확인해 보고한다.
   행이 생겼다면 숨기거나 null로 간주하지 말고 전체 행을 별도 복사해 알려준다. 이 확인을 위해 새 플레이하지 않는다.
3. 수신 Player에서 Stage와 Infinite 각각 출발의 번호/본인 표시·기록 조회를 확인하고 정상 종료한다.
   수신 Windows 사용자의 기존 원본 `flow-state-save.json`을 `step10-recipient-after-raw.json`으로 복사한다.
   출발의 새 계정 저장과 혼동하지 않는다. 현재 Pending 유무와 두 모드 화면 확인 여부도 보고한다.
4. AI가 위 7개 파일로 이전/Detached 서버 snapshot 및 수신 저장을 검증한 뒤 10-6 재실행을 안내한다.
   서버/인증 ID·번호·metadata의 수동 비교나 새 빌드·Module 게시·Unity Test Runner는 요구하지 않는다.

10-5 중간 결과 (2026-11-11):

- 사용자 원래 수신 owner의 두 보드 기록 없음·이전된 번호의 `(You)`·Pending 없음 보고를 받았다.
- AI가 현재 C/번호/고정 owner 보존·Active/revision+1·같은 수신 Player 연결·원 출발 Inactive binding·수신 Active binding을 확인했다.
  Completed transfer/출발 inactive/원래 수신 detached marker도 동일 이전에 귀속된다.
- 원래 수신 account는 server-detached capture 및 recipient-detached **PRESERVED**를 통과했다. 두 보드 부재는 사용자 보고 근거다.
- 수신 raw capture를 통과했다. 기기 owner·인증과 공개 번호/개인 최고 외 전체 저장(설정/입력/튜토리얼 포함)이 이전 전과 같다.
  번호 cache는 출발 기존 번호이며 Pending 0·개인 최고 2건이다.
- 두 보드 파일은 acceptedAt/submissionId의 **metadata만** 담고 있다. metadata는 기존과 일치하나 playerId/score가 없어
  전체 서버 capture/transfer 비교는 입력 불완전으로 통과하지 못했다. 이전 값으로 현재 행을 재구성하지 않으며 Step 10-5는 미완료다.
- 기존 파일을 보존하고 현재 보드의 playerId·score·metadata 전체를 `step10-stage-after-full.json`,
  `step10-infinite-after-full.json`으로 새 확보한다. manifest는 새 파일명을 참조한다.
  Dashboard가 metadata만 복사한다면 같은 현재 행의 Player ID·Score와 metadata를 한 JSON 객체로 옮긴다.
  두 파일만 보완하며 나머지 5파일 재수집·새 이전/플레이·재시작 검증은 아직 요구하지 않는다.

10-5 최종 결과 (2026-11-11):

- 사용자 보완한 두 full 행으로 server capture 및 `transfer` **PRESERVED**를 통과했다.
  출발 기존 C/번호/고정 owner와 Stage/Infinite 점수·metadata 전체가 보존됐고 revision+1·활성 Player 변경·원 출발 Inactive/수신 Active를 확인했다.
- 원래 수신 계정의 `recipient-detached` **PRESERVED**도 재확인했다. 앞선 동일 이전 marker/수신 저장 보존·Pending 0 및 사용자 화면 결과와 연결해 10-5를 완료한다.
- metadata만 있던 과거 입력 실패는 보완 이력으로 유지한다. 새 파일로 비교했으며 과거 파일을 덮어쓰거나 이전 score를 현재 값으로 대체하지 않았다.
  Step 10 전체는 재시작 근거 전까지 미완료다.

### 10-6 사용자: 수신 재실행 / AI: 최종 보존 대조 — 완료 (2026-11-11)

- Production QA 수신의 같은 Candidate01을 재실행한다. Stage·Infinite 각각 이전받은 출발 번호의 `(You)`와 기록 조회·Pending 없음을 확인한다.
  새 플레이/이전·출발 기존 상태 복원은 하지 않는다. 확인 후 Player를 정상 종료한다.
- 수신 Local Save를 `step10-recipient-restarted-raw.json`으로 새 복사한다.
  production의 같은 transferredAccountContainer `fs_account_v1` 현재 value를 `step10-account-restarted.json`으로 확보하고,
  같은 transferredLeaderboardOwnerId의 Stage/Infinite 전체 행(playerId/score/metadata)을
  `step10-stage-restarted-full.json`, `step10-infinite-restarted-full.json`으로 확보한다.
  기존 비공개 step10-after-dashboard-lookup.json을 재사용하며 다른 binding/원래 수신 Detached 자료를 반복 수집하지 않는다.
  4파일은 Ignore/Phase5Private에 보관한다. AI가 재시작 manifest를 준비했으며 파일명만으로 통과를 추정하지 않는다.
- AI가 이전 직후/재시작 후 restart/server-restart와 사용자 화면 결과를 연결한다.
  현재 실제 이전과 10-5 서버/저장 대조는 완료이며 수신 재시작 근거 전 Step 10 전체는 미완료다.

10-6 자료 검증 결과 (2026-11-11): 사용자가 재시작 후 4개 파일 준비를 보고했다.
AI가 새 raw/server capture를 통과해 별도 validated 자료를 생성했으며 `restart`와 `server-restart` 모두 **PRESERVED**다.
이는 이전 직후 대비 전체 Local Save(인증/번호 cache·개인 최고·Pending 0·설정/입력/튜토리얼)와 서버 계정/현재 연결/revision·두 고정 행 점수/metadata 보존 근거다.
실제 재실행 후 Stage/Infinite 각각 본인 표시·기록 조회와 Pending 없음의 화면 결과는 이번 파일 준비 보고만으로 추정하지 않는다.
추가 파일/플레이/새 이전 없이 해당 화면 결과만 확인하면 Step 10 최종 완료 판정을 할 수 있다. 현재 전체는 그 보고 전까지 미완료다.

취소/만료·실패 누적·정밀 경합·응답 유실의 모든 분기를 운영 계정에서 수동 재현하지 않는다. 해당 정확성은 Unit Test로 판정한다.
이전 성공 뒤 후속 새 기록 제출은 승인 범위와 기준 비교 완료 여부를 확인한 후 필요한 경우에만 수행한다.

10-6 최종 사용자 결과 (2026-11-11): 재실행 후 Stage/Infinite 각각 이전받은 번호의 `(You)`·정상 기록 조회와 Pending 없음 두 확인사항을 모두 정상으로 보고했다.
이전 실제 화면·출발 연결 종료/새 계정 전환, transfer/recipient-detached/restart/server-restart PRESERVED 및 수신 기기 설정 보존 근거를 연결해 **Step 10을 완료**한다.
위 화면 결과 확인 대기는 이 보고 전 이력이다. 다음은 Step 11이며 추가 이전/플레이·빌드/게시/Unity Test Runner는 요구하지 않는다.
정상 이전 시나리오 완료를 원격 모든 장애/경합 분기·cross-container 원자성 보장이나 Phase 5 전체 완료로 확대하지 않는다.

완료 조건: [x] A→B 실제 이전·기록 귀속/보존·A 비활성 안내·B 재시작 결과와 자동 비교 근거 연결.

## Step 11. Player 화면과 Keyboard/Mouse 조작을 확인한다

상태: **완료(사용자 기준 변경 승인, 2026-11-11)**.

사용자가 기존 Editor PlayMode 정상 근거를 수용하고 Step 11을 통과시키도록 명시 승인했다.
이에 이 Step만을 위한 별도 Player 가독성/Keyboard·Mouse/포커스/해상도·화면 배치 수동 검증 의무를 면제한다.
Step 6~10에서 이미 확보한 실제 Player 결과는 유지하며, 추가로 확인하지 않은 항목을 실제 실행 PASS로 바꾸지는 않는다.
Editor PlayMode 통과가 Player의 모든 동작 동일성을 보장한다는 기술적 결론은 내리지 않는다.
빌드 전용 조건·입력/포커스·디스플레이 차이 등에 의한 미확인 위험은 사용자 수용으로 남기며 새 실패가 실제 발견되면 별도 분류한다.
아래 체크리스트/절차는 승인 전 원래 검증 범위이며 미체크는 해당 개별 실행 결과가 없다는 뜻이다.
추가 수동 확인/빌드·Unity Test Runner·Scene 수정을 요구하지 않는다. 다음은 Step 12다.

Step 6~10에서 이미 연 화면을 재사용해 불필요한 제출·이전을 추가하지 않는다.

사용자 확인:

- [ ] 1920×1080 Windowed에서 Main Menu·Result·Settings·계정/이전 화면·Leaderboard 글자/버튼이 읽힘.
- [ ] Keyboard 선택·Submit/Cancel·Back과 Mouse 클릭이 정상 동작하고 포커스·모달 닫기가 자연스러움.
- [ ] Stage/Infinite·TOP/AROUND YOU 전환과 Loading/정상/Empty/Error/Pending 안내가 구분됨.
- [ ] Retry·Pending Retry·Back이 실제 표시 행과 겹치지 않음. 최대 표시 행의 재현은 자동 테스트 또는 로컬 대역을 우선함.
- [ ] 일반 Player에서 Continue 후 Notice가 반복되지 않음. 격리 Editor UI 제한은 Step 2-3 결정과 별도 구분함.

Notice 확인은 Step 2-3의 수정 → Step 3의 자동 회귀 검증(완료) → 이 Step의 실제 Player 화면 확인 순서다.
실제 확인은 Step 6~10에서 이미 사용하는 Production Player와 QA 계정으로 수행한다.
최초 미동의 상태에서 Leaderboard 진입 또는 Settings의 Online Record Notice를 열어 Continue를 누른 뒤,
같은 실행에서 Leaderboard를 나갔다 다시 열었을 때 Notice가 자동 반복되지 않는지 확인한다.
Settings에서 Notice 버튼을 다시 누르면 안내가 열리는 것은 명시적 재열람이며 정상이다.
격리 Editor의 확인 상태는 실행 중 메모리에만 남으므로 Play 종료 뒤 새 격리 실행에서 다시 뜨는 것도 정상이다.
자동 검증된 격리 동의 상태를 다시 수동으로 검사하거나 새 QA 계정·제출을 추가할 필요는 없다.

임의 장애를 Production에 유발해 모든 오류 화면을 수동으로 만들지 않는다. 자동/로컬 대역으로 이미 검증한 화면은 그 근거를 연결한다.
Scene 문제가 발견되면 AI는 방법·자동 검사만 준비하고 사용자가 Scene을 수정한다. 수정 후 필요한 컴파일/Test/재빌드 범위를 다시 정한다.

변경 전 완료 조건: [ ] 실제 Player 가독성·조작 수동 결과 및 자동 검증 근거 연결. 필수 UI 실패 없음.

적용 완료 조건: [x] 기존 Editor/Player 근거 수용 및 별도 Player 수동 검증 면제에 대한 사용자 명시 승인·잔여 위험 기록.

## Step 12. 배포 복구와 자료 보존을 검증한다

상태: **완료(다음 실제 업데이트로 복구 시험 연기·사용자 기준 변경 승인, 2026-11-11)**.

### 12-1 AI: 복구 후보와 호환성 대조 — 완료 (2026-11-11)

- 최초 Production 배포 전 원격 Module이 없었음을 유지한다. 최초 배포 실패는 수정 후 재배포이지 이전 버전 rollback이 아니다.
- 실제 다음 변경이 있다면 직전 성공 배포의 source/reference·Local Save/Cloud Save writer·migration marker·Secret 계약을 대조한다.
- 승인된 QA 자료 기준·복구 후 비교 항목을 준비한다. source만 같다는 이유로 rollback 동작을 실행 검증했다고 기록하지 않는다.

#### 12-1-1 Source/reference 대조 — 완료

- Phase 4 최초 게시 보고의 source는 Git `8774b2db2defa76c7e6a4ddd73f0f37f187c7cee`다.
  최초 게시 전 Module 없음은 과거 baseline이다. 현재는 이후 Module 수정 게시와 실제 서비스 검증이 있어 "이전 배포가 전혀 없음"으로 판정하지 않는다.
- 해당 source와 현재 작업 트리의 Module 범위 차이는 `account-provisioning-service.js`, `get-account-transfer-status.js`,
  `legacy-account-store.js`, `query-records.js` 4개다. 과거 source 복구는 신규 계정 초기 연결 복구/미기록 안내 수정을 제거하므로 자동 승인 후보가 아니다.
- 현재 서버 JS 39개·Module host/project/runtime/ccmr를 합친 source 50파일, 로컬 DLL 및 QA 기준 3개의 SHA256을
  비공개 `Ignore/Phase5Private/step12-static-recovery-reference.json`에 기록했다.
  로컬 DLL은 현재 JS 39개 bytes 포함을 확인했다. 이 확인은 원격 활성 DLL 동일성·host 빌드 source 동일성·게시 revision 증명이 아니다.
  hash manifest는 source archive 자체도 아니므로 이것만으로 재현 가능한 복구 백업 확보를 선언하지 않는다.

#### 12-1-2 호환성·회귀 대조 — 완료

- 위 과거 source 대비 Module host/ccmr·Local Save codec/data·Access Control·서버 schema/Secret 암호 계약 파일 변경이 없음을 확인했다.
  Local Save v7·서버 schema v1·HMAC Secret 계약을 유지하지만 schema 같음만으로 구버전 동작 안전성을 보장하지 않는다.
- migration marker/receipt를 무시하는 구 writer·v7 미지원 Client는 재도입하지 않는다. 현재 복구 수정은 같은 기존 계정/번호/원본 보존 조건의 제한적 CAS이며
  과거 source는 해당 중단 복구를 지원하지 않는다. 이미 복구된 계정에서 다시 실패한다고 추정하지 않으나 같은 문제의 재발 보호가 제거되는 위험을 기록한다.
- 로컬 Node **48개 test 파일 / runner 82항목** 실패 0, Phase 4 Step 5 권한/Secret 정적 계약·Step 6 안전 로그 계약·diff 공백 검사를 통과했다.
  policy 회귀는 같은 환경/reference·권한/호환성 gate, 데이터 삭제/환경 간 복사 금지·legacy writer 거부를 포함한다.
  로컬 대역/정적 검사이며 Unity Test Runner·빌드·원격 복구 실행이 아니다.

#### 12-1-3 자료 보존 기준·중단 조건 — 완료

- Step 10 재시작 후 수신 Local Save/활성 C와 두 전체 행·원래 수신 Detached를 현재 검증 기준으로 승계한다.
  transfer/recipient-detached/restart/server-restart PRESERVED는 현재 버전 검증이며 복구 후 보존 PASS가 아니다.
- 향후 실제 시험 직전에 새 기준을 확보해 전체 Local Save, Pending ID/payload/개수, C/번호/고정 owner·두 점수/전체 metadata,
  현재 인증 연결/revision·Inactive/Detached 상태를 대조한다. 시험 중 새 제출/이전이 없다면 모두 불변이어야 한다.
- 계정/번호/owner/행·metadata·Pending 변경, 소유권 오류·필수 기능 실패면 시험을 중단한다.
  최신 검증된 source/artifact를 다시 게시하는 복구는 사전 승인 범위에 포함해야 하며 서버 자료/Local Save/인증 삭제·수동 값 편집으로 수습하지 않는다.

### 12-2 사용자: 실제 복구 시험 여부 결정 — 승인 대기

#### 12-2-1 다음 실제 업데이트로 복구 시험 연기 — 승인 완료 (2026-11-11)

현재 정상 후보에 시험용 변경/장애나 가짜 이전 버전을 만들지 않는다. 사용자가 다음 조건을 명시 승인하면 Step 12를 기준 변경 승인으로 완료할 수 있다.

- 이번 후보의 실제 복구 배포/복구 후 자료 보존은 **미검증**으로 남긴다. 현재 Step 10 보존 PASS로 대체하지 않는다.
- 다음 실제 Module 업데이트 전에 성공한 source 전체·artifact/hash·환경/게시 reference와 직전 QA 기준을 보존하고 호환성을 다시 판정한다.
  시험 대상/유지할 최종 버전·중단/재복구 범위를 별도 승인한 후 실제 복구/자료 보존 시험을 수행한다.
- 검증 전에는 현재 정상 버전의 실서비스 결과만 근거로 삼으며 과거 수정이 제거되는 구버전을 승인 없이 게시하지 않는다.

사용자가 "다음 실제 업데이트로 복구 시험 연기 승인"을 명시했다. 이는 Step 11과 별도의 기준 변경 승인이다.
12-1 정적 준비와 위 미검증 위험/후속 조건 기록을 연결해 Step 12를 완료 처리한다.
실제 복구 배포·복구 후 데이터 보존은 여전히 미실시/미검증이며 실행 PASS가 아니다.
다음 실제 Module 업데이트 전에 성공 source 전체·artifact/hash·환경/게시 reference·새 QA 기준을 보존하고,
호환성/시험·중단·재복구 범위·최종 유지 버전을 별도 승인한 후 시험한다. 이번 승인은 향후 원격 배포 자체의 사전 승인이 아니다.
현재 추가 배포/초기화·빌드/Unity Runner·Scene 작업은 없다. 다음은 Step 13 최종 근거 대조이며 Phase 5 전체는 아직 미완료다.

#### 12-2-2 실제 시험을 지금 선택한 경우: 배포 전 자료/범위 승인

현재/이전의 정확한 source 전체·artifact/reference 대응과 되돌아올 최종 버전을 먼저 확정한다.
과거 4개 수정 제거 위험과 12-1-3의 새 기준/중단·재복구 범위를 승인받은 후에만 사용자가 수행한다.
운영자는 1인이며 기존 Step 1 결정대로 별도 수행 시간대 관리 조건은 요구하지 않는다.

#### 12-2-3 승인된 시험의 사용자 실행·AI 비교 — 미착수

1. 현재/이전 reference·중단/재복구 기준·검증 자료·최종 유지 버전을 승인한다.
2. 같은 Production 환경에 승인된 Module 하나만 복구 배포한다.
3. QA 계정/기록/Pending의 비공개 기준과 복구 후 상태를 비교하고 손실·병합·삭제가 없는지 확인한다.
4. 복구 뒤 어떤 버전을 유지할지 결정하고 배포 결과를 기록한다. 데이터 삭제나 환경 간 덮어쓰기는 하지 않는다.

실제 이전 성공 배포가 없거나 시험 변경을 승인하지 않은 경우 **실제 복구 후 보존은 미검증**으로 남긴다.
이 시험만을 위해 의미 없는 코드 변경·Production 장애·가짜 이전 버전을 만들지 않는다.
Phase 4에서 제외한 rollback 시험을 재도입하는 것이 아니라, 로드맵 Phase 5의 실제 복구/보존 요구를 별도로 다루는 단계다.
현재 후보에 적용할 수 없어 완료 기준을 면제/연기하려면 사용자의 명시적인 기준 변경 승인과 로드맵 갱신이 필요하다.
미실시를 성공으로 표시하거나 문서 작성만으로 Phase 5 전체를 완료하지 않는다.

완료 조건: [ ] 실제 복구·보존 근거 확보, 또는 [x] 명시적 완료 기준 변경 승인·미검증 위험/후속 조건 기록.

## Step 13. Phase 5 완료 여부와 공개 운영 후보를 판정한다

상태: **완료 / Phase 5 완료·공개 운영 후보 판정 (2026-11-11)**.

AI가 아래 근거를 source·빌드·대상별로 대조하고 사용자가 최종 검토한다.

- [x] Step 1 승인과 Step 2의 자동 검증/안전 도구가 실제 수행 결과와 연결됨.
- [x] 최신 후보의 Unity 컴파일·전체 테스트·Windows Player Build 성공 및 예상 밖 Warning 없음 사용자 보고가 연결됨.
- [x] 실제 Production 합성 로그 수집·신규/기존 QA 계정·두 Mode 제출/조회·Offline·이전/보존 확인.
- [x] 실제 Player UI/Keyboard/Mouse와 격리 Notice 문제의 처리/수용 결과 기록.
- [x] 복구 후 데이터 보존의 실제 근거 또는 명시 승인된 기준 변경 및 잔여 위험 기록.
- [x] 보고된 검증 결과의 예상 밖 필수 실패 없음 확인(승인 예외/과거 실패 이력은 유지). timeout/로그·원격 artifact 한계는 13-2에 기록함.
- [x] 점검 credential 처리 판정 기록: 기존 verification 합성 점검 계정은 유지하며, 실제 식별된 신규 임시 항목만 회수 대상으로 삼는다. 신규 발급 없음/회수 완료를 입증한 것은 아니며 소급 확인 한계는 13-3~4에 유지한다.
  런타임 LogSink 서비스 계정·필수 Secret 권한은 유지하며 QA 자료 삭제는 별도 승인 대상으로 남김.

필수 근거가 없으면 미완료 사유와 다음 작업을 기록한다. verification 결과를 Production 성공으로 대체하지 않는다.
모두 충족하면 Phase 5 완료/공개 운영 **후보**를 판정한다. Steam 배포·일반 사용자 공개 개시는 별도 사용자 승인이다.

### 13-1 AI: 후보·완료 근거 대조 — 완료 (2026-11-11)

| 근거 | 최종 대조 결과 |
|---|---|
| 승인/격리 | Step 1의 두 Windows 표준 사용자 격리·범위 승인·운영자 1인/별도 시간대 제외 유지 |
| Unity/Player | Step 6-5-5의 두 profile 컴파일/전체 테스트 성공과 6-5-6 새 Player Build 성공, Step 7~10 동일 Candidate01 실행 보고 연결. 최신 횟수/Warning 미보고는 과거 890/256으로 대체하지 않음 |
| 환경·Scene 정적 | check-phase5-build 통과: Production define/Release·명시 환경·저장 scope·Scene 참조/Editor 격리 제외 확인. Scene 읽기만 수행 |
| 서버/QA 동일성 | Step 12 source50파일·로컬 DLL·QA 기준3개 hash 불변. 현재 JS39개 로컬 embedded 일치 근거 유지. 원격 artifact 동일성 증명 아님 |
| 실제 Production | Step 5의 Accepted/Completed/None/202와 실제 안전 로그 7필드, Step 6~10 실제 계정·제출/조회·Pending/이전/보존 근거 연결 |
| 비교 재검증 | transfer/recipient-detached/restart/server-restart 네 비교 모두 PRESERVED 재확인 |
| 로컬 회귀 | Node48개 test 파일/runner82항목 실패0·static-contracts·C# preflight 7파일 통과. C# 사례 수 출력은 준비 수이며 이번 실행 수 아님 |
| 승인 예외 | Step 6 최초 조회 지연 처리 제외, Step 11 별도 Player UI/입력 수동 검증 면제, Step 12 복구 시험 차기 실제 업데이트 연기 승인 유지 |

빌드 정적 도구의 PlayerRebuildStillRequired는 Step 6 source 변경 시점의 안내다. 새 빌드 성공·동일 Candidate01 실제 실행 보고가 이미 있으므로
그 고정 문구만으로 재빌드를 반복 요구하지 않는다. 이번에는 runtime/server/Scene source를 변경하지 않았고 새 commit/Module 재게시도 요구하지 않는다.

### 13-2 AI: 잔여 위험·후속 의무 — 기록 완료

- 최초 Unavailable/Retry 지연은 원인 미확인·수정 계획 없이 사용자 승인으로 제외했다. 해결/원인 확정으로 표시하지 않는다.
- 첫 Infinite 실패 사유/원래 제출 terminal은 미확인이다. 후속 새 Run 성공·현재 Pending 없음과 구분하고 과거 실패를 삭제하지 않는다.
- 실제 legacy 전환은 대상 없음/N/A이며 원격 legacy migration PASS가 아니다.
- 별도 Player UI/Keyboard·Mouse 수동 검증은 사용자 승인으로 면제됐고 Editor/Player 동일성 보장은 아니다.
- 복구 배포 후 보존은 미검증이다. 다음 실제 업데이트 전에 성공 source 전체/artifact·환경/게시 reference/새 QA 기준 보존,
  호환성·시험/중단/재복구 범위·최종 유지 버전 별도 승인 후 실제 시험한다. hash manifest만으로 복구 source archive 확보를 선언하지 않는다.
- 로그 1건 도착은 정상 경로 근거이며 모든 실패/전송 유실·서비스 장애/시간 경계·원격 다중 container 원자성/모든 경합을 보장하지 않는다.
- 원격 활성 Module source/artifact의 정확한 동일성 및 게시 version/time 미보고 한계를 유지한다. 현재 로컬 DLL hash/embedded 대조를 원격 증명으로 대체하지 않는다.
- QA 계정/파일/서버 자료는 보존한다. 삭제·인증 복원/환경 복사/계정 병합·추가 원격 쓰기·Steam/일반 공개는 승인하지 않았다.

### 13-3 사용자: 정적 판정 불가 보고 보충 — 처리 결론 기록 완료

1. Step 5 점검을 위해 UGS Service Account credential 신규 발급/임시 권한 추가가 있었는지 보고한다.
   없으면 해당 없음으로 기록한다. 있었다면 그 점검 전용 credential/추가 임시 권한만 회수한 결과를 보고한다.
   기존 공용 운영 credential·런타임 LogSink 서비스 계정·필수 Secret 권한을 무차별 삭제하지 않으며 키 원문은 수집하지 않는다.
2. 최신 Candidate01의 두 profile 컴파일/테스트·Player Build에 예상 밖 Warning도 없었는지 기존 결과를 보충한다.
   이미 보고된 성공을 다시 실행하라는 요구가 아니다. 확인하지 못했으면 미확인으로 보고하고, 실제 Warning이 있었다면 안전한 경고 내용으로 필수 실패 여부를 판정한다.

### 13-4 최종 판정 — 완료 (2026-11-11)

13-3 보충 보고: 사용자가 최신 두 profile 컴파일/테스트·Candidate01 Build의 예상 밖 Warning 없음을 확인했다.
credential 신규 발급/임시 권한 추가 여부는 기억하지 못해 미확인이다. 현재 남은 항목은 credential 식별/필요한 점검 전용 권한 정리다.
공식 안내의 Account → Manage organization → Administration → Service Accounts에서 계정별 Keys·Organization roles/Project roles를 읽기 전용으로 확인한다.
사용했던 Key ID를 로컬 보관 기록과 대응할 수 있으면 같은 키를 식별한다. Secret 재조회·새 키 생성·probe 재실행으로 확인하지 않는다.
계정 이름/생성 시각(표시될 때)만으로 점검 전용이라고 단정하지 않는다. 기존 배포/자동화/운영 사용 여부와 비교하고 불명확하면 삭제하지 않는다.
wrapper는 숨김 입력을 사용하고 종료 시 process 환경을 복원하므로 console의 마스킹이나 안전 로그로 사용 키의 정체/신규 여부를 복원할 수 없다.
식별이 안 되면 비밀값을 가린 계정 이름·역할·Keys 개수/생성일(표시될 때) 같은 비민감 요약으로 추가 검토한다.
런타임 GCP LogSink 서비스 계정/Secret와 UGS 점검 키를 혼동하지 않는다. 회수 대상을 확정하기 전 원격 삭제/권한 제거를 수행하지 않는다.
사용자 후속 보고에서 verification synthetic probe 전용 이름/설명, 조직 역할 없음·해당 Project Cloud Code Viewer,
키 1개/베어러 토큰 1개(상대 생성일 표시), 배포/자동화 사용 여부 모름을 확인했다. 실제 principal 이름/키 값은 기록하지 않는다.
저장소 AI/UGS에는 보고된 계정 이름의 명시적 참조가 없었다. 이는 외부 CI/자동화의 미사용 증명이 아니다.
점검 전용 후보로 분류하되 Step 5가 사용한 키/신규 권한과의 동일성은 미확인이다. 해당 계정 권한은 production 한정이라고 추정하지 않는다.
외부 사용 확인 전 계정 전체/키/토큰/역할의 자동 삭제를 하지 않으며 기존 런타임 권한은 유지한다.

13-3 Phase 4 기록 재조사 (2026-11-11):

- `20261007_09_Phase4ManualSteps.md` Step 6-3.6.3은 2026-10-20 verification 전용 UGS Service Account/Stateless Token 호출과 최종 Accepted/202를 기록한다.
  Step 8-E-5도 동일 용도의 wrapper 인증·호출·202/7필드 수집을 기록한다. 따라서 합성 점검 계정이 Phase 5에 처음 필요해진 것은 아니다.
- `UGS/ENVIRONMENT_NEUTRAL_MODULE_CONFIGURATION.md` 8-E 실행 안내는 이전 합성 Probe에 사용한 UGS credential 재사용을 명시한다.
  Phase 5 Step 5 역시 기존 승인 credential 재사용을 허용한다. 모든 probe 계정/키가 작업 종료 후 자동 회수 대상이라는 이전 해석은 적용하지 않는다.
- 사용자 보고된 verification synthetic 전용 이름/설명은 이 기존 목적과 부합하므로 **기존 점검 계정 유지 후보**로 분류한다.
  이를 현재 자료만으로 신규 임시 계정/불필요 운영 키라고 판정하지 않으며 외부 자동화 미사용 증명을 선행 요구해 삭제를 유도하지 않는다.
- 실제 계정명/Key ID는 과거 기록에 없으므로 현재 키와 당시 키의 정확한 동일성, Phase 5 추가 발급/권한 변경 여부의 완전한 소급 확인은 불가능하다.
  신규 추가 없음이나 회수 완료로 바꾸지 않는다. Phase 4 계정은 기존 운용 목적에 따라 유지할 수 있고, 실제 확인된 신규 임시 항목만 별도 회수 대상이다.
- 현재 원격 삭제/권한 제거·재발급/재호출은 하지 않는다. 기존 계정을 유지하고 소급 확인 불가를 잔여 한계로 수용할지 사용자 확인 후 Step 13 최종 판정한다.

기술 근거/예외 대조는 준비됐으나 위 보고 없이 credential 정리 완료·예상 밖 필수 실패 없음 또는 Phase 5 완료/공개 후보를 확정하지 않는다.
사용자 보고를 반영해 해당 없음/정리 완료와 Warning 결과를 판정하고 최종 결론을 문서화한다. 공개 개시는 별도 사용자 승인이다.

#### 최종 처리 결론

- 계정 역할 설명 후 사용자 "Step 13 처리 수행" 지시에 따라 기존 verification 합성 로그 점검 계정을 재사용 목적의 유지 대상으로 처리한다.
  계정/키/토큰/역할을 원격 삭제하거나 변경하지 않았다. 과거 키와 정확한 동일성/Phase 5 추가 변경 이력은 미확인 한계로 남기며
  신규 발급 없음·키 회수 완료·해당 계정의 함수 단위 권한 제한을 증명했다고 기록하지 않는다.
- 기술 검증·최신 Warning 없음 보고, Phase 4 기존 credential 재사용 근거와 현행 역할 검토를 연결해 13-3 처리 결론을 확정한다.
  추가 임시 credential이 실제 식별되면 별도 회수 대상으로 처리한다. 기존 운용 계정까지 삭제하는 것은 완료 조건이 아니다.
- 최종 Node48파일/runner82항목 실패0 및 transfer/recipient-detached/restart/server-restart 네 비교 PRESERVED를 재확인했다.
  Step 1~12의 실제 Production/Player 근거와 승인 예외·13-2 잔여 위험을 포함해 **Step 13 및 Phase 5 완료, Candidate01 공개 운영 후보**로 판정한다.
- 이는 모든 항목의 무예외 실행 PASS가 아니다. 최초 조회 지연 제외·별도 Player UI 수동 검증 면제·실제 복구 시험 차기 업데이트 연기,
  legacy N/A·첫 Infinite 실패 이력·credential 소급 확인/원격 artifact/모든 장애·경합·원자성 미확인 한계는 계속 유효하다.
- 다음 실제 Module 업데이트 전 Step 12의 source 전체/artifact/reference/새 QA 기준 보존·호환성/시험·중단/재복구 범위 별도 승인 후 실제 복구 시험을 수행한다.
  QA 자료/기존 계정·런타임 LogSink 권한/Secret은 유지한다. Steam 배포·일반 사용자 공개·자료 삭제는 별도 승인이다.
- 이번 최종 처리는 로컬 재검증/문서 갱신뿐이다. runtime/Module/Scene source 변경·Unity Build/Test Runner·원격 배포·새 credential/commit은 없다.
  위 승인/보고 대기 서술은 최종 처리 전 이력이다.

## 결과 보고 원칙과 기록표

| Step | 결과 | 기록할 내용 |
| --- | --- | --- |
| 1 | 완료 | Production QA 출발/수신 격리·전체 쓰기 범위 승인·운영자 1인, 시간대 별도 관리 제외 |
| 2 | 완료 | Node 53파일/Production 9검사·로컬 LogSink·PowerShell 대역·C# 정적 검사 및 Step 3 회귀 연결 |
| 3 | 완료 | 컴파일 성공·EditMode 890/890·PlayMode 256/256·각 예상 밖 Error/Warning 없음, Module source 변경 없음 |
| 4 | 완료 | 정적 사전 검사·사용자 Build 성공 및 후속 동일 Candidate01 실제 실행 연결. 최신 예상 밖 Warning 없음은 Step 13에서 보충 확인 |
| 5 | 완료 | 사용자 Accepted/Completed/None/202 및 Production 실제 jsonPayload 7필드 저장 대조 완료 |
| 6 | 완료(사용자 예외 승인) | 최초 Notice 전환·Retry 후 조회 복구·미기록 안내 확인. 최초 Unavailable/Retry 4회는 원인 미확인·처리 계획 없이 보류하며 완료 조건에서 제외 |
| 7 | 완료 | Stage Submitted/두 목록/Retry·Infinite 후속 Run 제출/두 목록·현재 Pending 표시 없음·Back 정상 확인. 첫 Infinite 실패 원인/최초 ID 결과는 미확인 이력 보존 |
| 8 | 완료 | Offline 1건/재시작 PRESERVED, 수동 1 submitted·0 rejected·0 pending, 저장 REMOVED_ONLY·후 0건 및 제출 후 Stage 조회 확인 |
| 9 | 완료 | 실제 로컬 restart·server-restart PRESERVED, 계정/두 목록 조회·기존 설정/입력 적용·튜토리얼 비반복 정상 사용자 확인. 실제 legacy N/A |
| 10 | 완료 | 정상 이전·출발 연결 종료/새 계정 전환, transfer/recipient-detached/restart/server-restart PRESERVED 및 재실행 후 두 모드 본인 표시·조회/Pending 없음 사용자 확인 |
| 11 | 완료(사용자 기준 변경 승인) | 기존 검증 근거 수용·별도 Player UI/입력 수동 검증 면제. 미확인 항목의 실제 실행 PASS/Editor-Player 동일성 보장은 아님 |
| 12 | 완료(사용자 기준 변경 승인) | 정적 준비 완료·다음 실제 업데이트로 복구 시험 연기 승인. 실제 복구 후 보존 미검증·차기 후보/자료 보존·호환성/시험 범위 별도 승인 후 수행 |
| 13 | 완료 | 기존 점검 계정 유지/credential 소급 확인 한계 기록, 최신 Warning 없음·최종 Node/4비교 통과. Phase 5 완료·Candidate01 공개 운영 후보, 실제 공개 미승인 |

최초 계획 작성 시점에는 실행 미착수였다. 현재 Step 1~13은 승인 예외/잔여 한계를 포함해 완료했으며 Player Build·실제 Production 서비스/이전·보존 근거를 확보했다.
Phase 5 완료·Candidate01 공개 운영 후보로 판정했고 일반 사용자 공개 개시는 별도 승인 대기다.
실행 파일/도구 경로와 버튼이 확정되지 않은 항목은 담당 Step 수행 때 코드와 대조해 구체화한다.

## 영향 범위·검증 결과·후속 작업

- 최초 계획 작성의 영향 범위는 Tasks·Roadmap·프로젝트 진행 기록이었다.
  2026-10-22 Step 2에서 Production 로컬 도구·테스트·GameSystem의 Editor 격리 동의 경로를 보강했다.
  Scene·서버 Module·Secret/환경 정책은 변경하지 않았다.
- 문서 검증: Phase 4 인계·로드맵·현행 source를 대조했다. 13개 Step 연속 번호·실행 미착수 상태·승인 경계·
  참조한 기존 EditMode 테스트 파일 존재·관련 로컬 문서 링크·식별자 비기록·문서 공백/줄바꿈 정적 검사를 통과했다.
- 도구/새 Unit Test 구현과 실제 실행은 각 Step의 후속 작업이다. 계획만으로 테스트 통과를 주장하지 않는다.
- 후속 작업: **공개 개시 여부는 사용자 별도 결정**. 다음 실제 Module 업데이트 전 승인된 복구 시험을 수행한다. 현재 후보/QA 자료/점검 계정을 유지하고 추가 빌드/Runner·재게시/probe·새 키 발급은 요구하지 않는다.

## 관련 문서

- [Prototype 8 Roadmap](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md)
- [Phase 4 완료·인계](20261007_09_Phase4ManualSteps.md)
- [Phase 3 실제 검증 이력](20261006_08_Phase3ManualSteps.md)
- [Production Inventory](../../../UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md)
- [환경 중립 Module 구성](../../../UGS/ENVIRONMENT_NEUTRAL_MODULE_CONFIGURATION.md)
- [Safe LogSink 계약](../../../UGS/SAFE_LOG_SINK_CONTRACT.md)
- [계정 이전 운영 안내](../../../UGS/TRANSFER_OPERATIONS_RUNBOOK.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
