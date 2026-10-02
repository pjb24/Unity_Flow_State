# 목적

Prototype 7의 verification 후보를 기반으로 서버 발급 십진 Public Player Number를 도입하고, 온라인 기록 서비스를 공개 운영할 수 있도록 준비한다.

# 프로젝트 정보

## 프로젝트명

Flow State

## 목표

8차 프로토타입 (Prototype_8)

- 내부 UGS Player ID와 별개의 공개 번호로 계정과 Leaderboard 행을 식별할 수 있게 한다.
- 신규 계정의 수동 ledger 생성과 계정별 128 제출 ID·보드 100명 조회 제한을 해소한다.
- verification과 Production을 분리하고 배포·롤백·장애 대응 및 데이터 보존을 검증한다.

문서 작성 완료와 구현 완료는 구분한다. 실제 Production 적용은 대상과 변경 내용을 확정한 별도 배포 승인 절차에서 수행한다.

# 범위와 기존 결정의 승계

2026-09-29 결정 중 Prototype 7 Phase 4에서 완료한 UI·순위 동작은 회귀 검증 기준으로 유지한다. 세부 계약의 기준 문서는 `Leaderboard.md`, `ResultMenu.md`, `RecordSubmission.md`다.

| 기존 결정 | Prototype 8 적용 |
| --- | --- |
| 확정 기록 없는 사용자의 조회 제한, 성공한 0건 목록만 Empty | 유지 |
| 상위·내 주변 독립 조회, 원인을 표시하는 Error, 단일 고정 Stage 선택 | 유지 |
| Main Menu 내 최고 전용 조회 제외, Result에서 명시적으로 순위 보기 | 유지 |
| 로컬 최고·Online best·이번 Run 제출 상태 구분 | 유지 |
| 같은 Score는 공동 순위, 같은 순위 안에서는 서버 수락 시각 오름차순 | 유지. 조회 규모 확대와 페이지 경계에서도 검증 |
| 현재 마스킹된 계정·행 표시와 `(You)` | 공개 번호 도입 전까지 유지. 도입 후 전체 십진 공개 번호와 본인 행의 `<공개 번호> (You)` 표시로 전환 |
| Rejected 사유·재시도 불가, Pending에만 제출 Retry | 유지. 조회 Retry와 구분 |
| 동의 취소 시 요청 없이 원래 화면 복귀, Anonymous 복구 제한 | 유지. 공개 번호 발급을 계정 복구 기능으로 취급하지 않음 |
| Windows x64·1920×1080 Windowed·Keyboard + Mouse | 검증 기준 유지. Gamepad는 지원 선언 시 포함 |
| 성능·응답 시간 측정 제외 | 현재 계획에서도 유지. Timeout·Offline·오류·로딩 대기 제한은 기능 검증 대상으로 유지 |
| verification 한도 내 후보 | 공개 운영 준비로 확장. 운영 배포·데이터 전환은 별도 절차로 검증 |

Pending의 Local Save 보존, 재시작 후 동일 제출 ID 복원, Submitted/Rejected 응답 뒤 해당 Pending 삭제 정책도 유지한다. 완료 제출 ID와 거절 사유를 로컬에 영구 저장하는 기능은 추가하지 않는다.

# 개발 단계

## Phase 1. 공개 운영과 공개 번호 계약 확정

### 목표

이관된 작업의 정책과 검증 범위를 구현 전에 확정한다.

### 구현 대상

- 공개 번호의 발급 시점·유일성 범위·자릿수·재발급/재사용 정책 및 기존 계정 전환 계획.
- 내부 계정과 공개 번호의 영속 매핑, 번호 미발급·조회 실패 시 UI 처리와 Anonymous 복구 제한 안내.
- 제출 중복 판정 정보의 보관·정리 기간과 오래된 Pending 재전송 처리 정책.
- 보드 조회 페이지·상위/내 주변 한도, 동점 경계 및 수락 시각 동률 처리 기준.
- Production 테스트 계정·기록의 분리와 verification 데이터 이전 여부. 기존 데이터의 자동 이전·초기화를 가정하지 않는다.
- 서비스 오류 관측, rate limit·장애 대응, 공개 경쟁 수준에 필요한 부정행위 방지 범위.

### 완료 조건

- 위 정책의 미정 항목이 결정되고 관련 Feature·System 문서에 반영되어 있다.
- 기존 UGS 구조로 충족할 수 있는 범위와 추가 검토가 필요한 제약이 구분되어 있다.
- 강화된 서버 검증이 필요하다면 별도 구현 범위와 완료 조건이 확정되어 있다.

### 상태

완료 — 2026-10-03. Step 1~8에서 공개 번호·논리 계정·기기 이전·receipt·조회·환경·운영 정책과 정책 Test를 확정하고 Phase 2~5 인계를 기록했다. 이는 구현·Unity·원격 서비스 검증 완료가 아니며, 실제 제약 해소는 후속 Phase에서 수행한다.

## Phase 2. Public Player Number 발급·표시

### 목표

서버가 발급한 전체 십진 공개 번호로 계정과 기록을 식별한다.

### 구현 대상

- 공개 번호 발급·저장·조회와 기존 계정의 번호 부여.
- 중복 요청·동시 발급·실패 후 재시도에서 번호와 계정 매핑의 일관성.
- 계정 안내와 Leaderboard 행의 공개 번호 표시, 본인 행 `(You)` 표시.
- 번호 도입 과정에서 기존 순위·개인 최고·계정 귀속 Pending 보존.
- 외부 ID 없이 Anonymous 계정의 유일한 활성 기기를 새 기기로 이전하는 서버 발급 코드·인증값과 연결 교체.
- 논리 계정의 고정 Leaderboard 소유 ID를 사용하고, 기기 이전 뒤에도 동일 행·순위 metadata를 유지.

### 완료 조건

- 서로 다른 계정에 같은 번호를 발급하지 않고, 같은 계정의 재요청·재시작에서 확정된 번호를 유지한다.
- 전체 십진 공개 번호가 정밀도 손실·생략 없이 표시된다.
- 내부 UGS Player ID의 십진 변환을 공개 번호 발급으로 대체하지 않는다.
- 번호 발급·조회 실패가 Offline 플레이를 차단하지 않으며 다른 계정의 번호나 기록을 표시하지 않는다.
- 이전 성공 뒤 새 기기만 활성 연결이 되고, 이전 기기는 같은 논리 계정의 온라인 기능을 사용할 수 없다.
- 기기 이전이 Leaderboard 기록을 새 Player ID 행으로 복사·이동하지 않고 기존 서버 수락 시각과 동점 표시 순서를 보존한다.
- 자동 Test와 사용자 UI 확인이 통과한다.

### 상태

대기

## Phase 3. 제출·조회 제한 해소

### 목표

검증용 수동 준비와 고정 용량 제한을 해소하면서 기록의 정확성을 유지한다.

### 구현 대상

- 신규 계정의 Protected ledger 자동 초기화와 동시 초기화·부분 실패 복구.
- 128 submission ID 제한을 C별 시간 분할 terminal receipt로 대체한다. receipt는 180일 동안 중복·payload 충돌 판정에 사용하고, 이후 삭제한다. Local Save Pending도 생성 뒤 180일에 `SubmissionExpired`로 정리한다.
- 100명 조회 제한을 대체하는 상위 10·내 주변 7 조회와 서버 전역 순위 처리. 사용자 페이지 UI는 제공하지 않는다.
- 보관 정보 정리, 재전송, 동점 경계 분리에도 전역 공동 순위·수락 시각·공개 번호 최종 순서 보존. 최초 진입·명시적 새로고침·재진입 외 자동 조회 갱신을 하지 않는다.

### 완료 조건

- 신규 계정이 Dashboard 수동 ledger 생성 없이 기록을 제출할 수 있다.
- 128건을 넘는 제출과 100명을 넘는 보드 자료를 자동 Test로 검증한다.
- 동시 요청·삭제 저장 실패·180일 전 Pending 재전송이 중복 반영이나 계정 간 기록 혼합을 만들지 않는다. 180일 뒤 receipt가 삭제된 동일 ID는 신규 제출로 허용한다.
- 페이지 경계의 동점과 내 주변 순위가 전체 순위 계약과 일치한다.
- 용량 경계 검증은 정확성 검증이며 성능 측정 통과로 기록하지 않는다.

### 상태

대기

## Phase 4. 운영 환경·배포·복구 준비

### 목표

운영 대상과 검증 대상을 분리하고 서비스 변경을 적용·복구할 수 있게 한다.

### 구현 대상

- 환경별 Authentication·Cloud Code·Cloud Save·Leaderboard·Access Control 설정과 Client의 명시적 환경 선택.
- verification 자료를 Production으로 이전하지 않고 운영 테스트는 verification에만 둔다. 빌드 고정 환경과 `(Project ID, Environment ID)`별 온라인 Local Save·Pending 분리로 잘못된 환경 전송을 차단한다.
- 배포 대상·버전·권한·비밀값 관리, 변경 전 보존과 롤백 절차.
- PII·비밀값을 제외한 최소 구조화 로그 30일 보관, C 귀속 요청 제한, Timeout·1인 수동 장애 대응 및 테스트 계정·데이터 관리. 30일 로그 sink·삭제 절차가 없으면 Production 배포를 차단한다.

### 완료 조건

- verification과 Production의 계정·제출·조회 경로가 섞이지 않음을 검증한다.
- Client 직접 쓰기 차단과 서버 검증 경계가 운영 설정에서도 유지된다.
- 비밀값을 노출하지 않고 장애 원인을 파악할 수 있으며 배포·롤백 절차가 재현 가능하다.
- 서버 상태와 Client 버전이 달라질 때의 호환성·데이터 보존 기준이 충족된다.

### 상태

대기

## Phase 5. 통합 검증과 공개 운영 후보 판정

### 목표

변경된 공개 번호·서비스·저장·UI를 Windows Player에서 검증하고 운영 후보의 완료 근거를 정리한다.

### 구현 대상

- 전체 자동 Test와 실제 서비스 인증·번호 발급·제출·조회 검증.
- Offline·재시작 Pending 복구, 기존 계정 전환, 롤백 후 데이터 보존.
- Windows x64·1920×1080 Windowed Build와 Keyboard/Mouse·UI 가독성 확인.

### 완료 조건

- 최신 변경으로 컴파일·전체 Edit/Play Mode Test와 대상 Player Build가 통과한다.
- 승인된 운영 대상에서 신규/기존 계정, 두 Mode, Offline 복구와 데이터 보존을 확인한다.
- 해결하지 않은 필수 실패가 없고 운영 한계·배포 상태·롤백 근거가 기록되어 있다.
- verification만 확인했다면 운영 검증 완료로 처리하지 않는다. 일반 사용자 대상 공개 개시는 별도 결정으로 기록한다.

### 상태

대기

# 검증 책임

- AI: 문서·코드·설정 정적 검사, 자동 Test 작성, 로컬 서버 대역 검사, 수동 절차 작성.
- 사용자: Unity 컴파일·Test Runner·Build 실행, Scene 적용, 실제 서비스 적용과 Player·화면 확인.
- Play Mode Test는 실제 계정·Local Save·verification/Production 서버에 제출하지 않도록 격리한다.
- 동시 요청·중복 처리·경계값은 자동 검증으로 판정하고 사람에게 정밀한 클릭 타이밍을 요구하지 않는다.
- Prototype 7의 성공 결과는 기준점이며 Prototype 8 변경 후 검증 결과를 대신하지 않는다.

# 현재 개발 진행 상태

## 진행 중인 작업

Prototype 8 Phase 1 완료 — 정책·정적 계약 검증·Phase 2~5 인계를 완료했다. 구현은 미착수다.

## 다음 작업

Phase 2에서 공개 번호 발급·기존 자료 cutover·논리 계정의 단일 활성 연결·기기 이전·고정 Leaderboard 소유 행을 Cloud Code와 Client에 구현하고, 원자성·경합·응답 유실을 자동 검증한다.

실행 순서와 사용자 수동 적용·Unity Test Runner·verification 확인 절차는 `AI/90_Tasks/Prototype_8/20261003_01_Phase2ManualSteps.md`에서 관리한다. 계획 작성 완료이며 Step 1~12 구현·적용·검증은 대기다.

## 보류된 작업

- 자체 Backend 전환: 기존 UGS로 목표를 충족할 수 없는 제약이 확인될 때 검토한다.
- 외부 ID 계정 연결, 이전 자격 증명 없는 분실 기기 복구, 친구·시즌·지역별 순위, 보상과 추가 Stage: 이번 목표에 포함하지 않는다. 서버 발급 코드·인증값을 사용하는 Anonymous 계정의 기기 이전은 Phase 2 구현 대상이다.
- 강화된 부정행위 방지·Server Authoritative Run: Phase 1에서 현재 서버 입력 검증 유지로 범위를 확정했다. Server Authoritative Run과 자동 부정행위 판정은 이번 Prototype 범위 밖이며, 필요성이 생길 때 별도 목표·완료 조건으로 검토한다.
- 추가 플랫폼·해상도·Gamepad 지원 및 성능·응답 시간 측정: 별도 범위 결정 전까지 제외한다.

## 완료된 단계

- Phase 1. 공개 운영과 공개 번호 계약 확정 — 2026-10-03 완료. 정책·정적 계약 검증·후속 인계를 완료했으며 구현·운영 검증은 포함하지 않는다.

# 구현 우선순위

1. 운영·공개 번호·데이터 정책 확정.
2. 공개 번호 발급과 UI 전환.
3. 제출·조회 제한 해소.
4. 운영 환경 분리와 배포·롤백 준비.
5. 통합 검증 및 운영 후보 판정.

# 완료 기준

전체 십진 공개 번호가 안정적으로 발급·표시되고, 기존 기록·Pending을 보존하면서 검증용 수동 준비와 고정 용량 제한을 해소한다. 환경 분리·권한·배포·롤백 및 Windows Player 검증 근거를 갖춘 공개 운영 후보를 완성한다.

# 관련 문서

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/ResultMenu.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/90_Tasks/Prototype_7/20260928_01_Phase4ManualSteps.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`

# 작성 완료 기준

- Prototype 7에서 승계할 계약과 Prototype 8의 신규 목표를 구분했다.
- 단계별 목표·완료 조건·미정 정책·검증 책임을 명시했다.
- 계획 작성, 구현 완료, 운영 검증과 공개 개시를 구분했다.
