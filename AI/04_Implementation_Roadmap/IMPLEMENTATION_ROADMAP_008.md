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

기술 검증·후속 Phase 인계 통과, 임시 관리자 Key/역할 회수 확인 대기 — 2026-10-06. Step1~11·사용자Unity909/257·실제Module/서비스/UI에 더해 실제CloudSave batch/CAS·저장 경계20복구·경합2사례를 두 실행으로 확인했다. 단일 실행 전체PASS나 전체transaction/무제한 서비스 장애 보장으로 확대하지 않는다. Step12/Phase2 최종 완료는 이번 도구 임시Service Account Key 폐기·Cloud Save Editor 역할 회수 보고 후 처리한다. 기존legacy N/A·시작지연 수용 제한 및 Phase3~5 인계는 유지하며 Production 승인은 별도다.

## Phase 3. 제출·조회 제한 해소

실행 계획: [20261006_08_Phase3ManualSteps.md](../90_Tasks/Prototype_8/20261006_08_Phase3ManualSteps.md). Step 1~6은 AI 구현·정적 검사·Unit Test 준비, Step 7~9는 사용자 Unity 검증·verification Module 게시·실제 서비스/UI 확인, Step 10은 완료 판정이다. 2026-10-06 계획 작성 완료이며 Phase 3 구현·실행 검증은 대기 상태다.

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

Prototype 8 Phase 1 및 Phase 2 Step 1~5의 로컬 서버 구현/검증, Step 6-1~6-5의 Client 코드/Test 작성·정적 검증은 완료했다. 2026-10-04 최신 사용자 Unity 컴파일 성공·Edit Mode 850/850·Play Mode 242/242 통과 및 각 단계의 예상 밖 Error/Warning 없음과 Scene 연결·영어 label 반영/AI 읽기 전용 대조를 근거로 Step 7·8도 완료했다. 신규 작성 사례 Edit 121/Play 8은 사용자 전체 실행 수와 구분한다. Step 9~12의 verification 적용·실서비스·화면·최종 승인 및 Phase 2 전체는 미완료이며 상세 근거는 Phase 2 Task에서 관리한다.

## 다음 작업

2026-10-06 Remaining 실제PASS로 AFTER10·경합2사례까지 확보해 기술 검증 범위는 통과했다. 다음은 **사용자 임시 Key 폐기 / Unity_Flow_State의 Cloud Save Editor 역할 회수 완료 보고**뿐이다. 이전 검사·UnityTest/Build/Scene/Module 적용을 반복하지 않는다. 보고 후 Step12/Phase2 최종 완료 상태를 갱신한다. 실제두실행의20복구/경합2와 기존Module/UI/Unity909/257를 함께 사용하며 관리자 저장 도구를 Module/Jint 실행 근거로 혼동하지 않는다.

2026-10-06 실제 저장 검사에서 batch/CAS·정상 이전·장애 복구19사례 PASS를 확보했고 응답 미확인/HTTP0으로 전체 실행은 중단됐다. AFTER10·두 대상 경합·제출 예약/이전 경합은 미확인으로 유지한다. 다음은 LIVE_TRANSFER_STORAGE_PROBE의 **`-Run -Remaining` 사용자 실행·안전한 결과 전달**이다. 새 전용 공간으로 미확인3사례만 검사하고 기존19PASS/자료를 보존한다. 도구9로컬검사·상세 안전 진단을 준비했으며 Unity/Module/Scene 변경은 없고 Step12/Phase2전체는 미완료다.

2026-10-06 Step 12 후속 도구 준비 완료. 다음은 `UGS/Verification/LIVE_TRANSFER_STORAGE_PROBE.md`에 따른 사용자 임시 Service Account/Cloud Save Editor·PowerShell 숨김 입력·verification 전용 p2v 데이터 검사·키 회수와 안전한 결과 보고다. Node5도구검사/완료20장애사례/동시 이전 로컬 통과와 원격 결과를 구분한다. 기존 A/B/발급기/Leaderboard·C#/Module/Scene 변경은 없으며 추가 Unity Build/Test/게시가 필요하지 않다. Module 인증/Jint/전체5초 경계까지 증명하는 도구는 아니므로 실제 결과 후 Step12를 재판정하며 현재 Phase2전체는 미완료다.

2026-10-06 Step 12 검토 결과: 근거 대조·로컬 Node38파일 재검사/50C# 정적 검사·Phase 3~5 인계를 완료했다. 실제 서비스의 장애/경합 이전 복구 근거가 없어 **Step 12·Phase 2 전체는 미완료**다. 다음은 해당 미확인 항목을 위한 별도 verification 자동 검증 범위/원격 변경·실행 승인 또는 명시적인 위험 수용/완료 기준 결정이다. 지금 사용자에게 기존 Unity Test/Scene/Build/빠른 버튼 조작을 반복하도록 요구하지 않는다. Phase 3은 receipt180일/128건·100명 초과/전역 순위, Phase 4는 Production/30일 로그/롤백, Phase 5는 사용자 Player Build/통합 검증을 인계받는다. 아래 Step 12 착수 전 안내는 이전 이력이다.

2026-10-06 최신 사용자 Unity 컴파일 성공·EditMode 909/909·PlayMode 257/257 및 각 단계 예상 밖 Error/Warning 없음으로 마지막 Client 수정의 회귀 대기를 해소했다. 앞선 게임 화면/이전/복사/Offline/Pending/조작 확인 및 사용자 승인한 시작 지연 추가 확인 종료를 근거로 Step 11 완료. **다음 작업은 Step 12의 Phase 2 완료 근거·미확인 사항 대조와 Phase 3~5 인계 검토**다. 지연 원인/실측 개선과 원격 원자성을 확인한 것으로 기록하지 않으며 Step 12·Phase 2 전체·Production 승인은 미완료다. 아래 다음 작업/대기 설명은 이전 이력이다.

2026-10-06 최신 사용자 결정: 계정 창 시작 지연 추가 확인은 수용한 제한으로 종료했다. 다음은 마지막 Client 수정 후 사용자 Unity 컴파일·전체 EditMode/PlayMode 회귀 결과 확인이다. 기존 A 재시작/시간 측정/안내 문구 보고 요청은 종료했다. UGS 원인과 실제 5초 응답은 미확인으로 구분하며 Step 11은 최신 회귀 결과 대기다.

2026-10-06 사용자904/257 회귀·이전된 B 번호/본인 행·화면 조작 확인 뒤 A 시작 번호 지연을 개선하는 Client 코드를 준비했다. 다음은 Task11-F의 **최신 사용자 컴파일/전체 Test → 같은 A 재시작 직후 계정 화면/단일 Refresh 결과 확인**이다. 새 이전·Scene·Module 작업은 필요하지 않다. Node38파일·50 C# 정적 검사/Edit5사례 준비와 실제 Unity/지연 개선을 구분하고 Step11/12·Phase2전체는 미완료다.

2026-10-06 복사 UI와 게임 Complete Transfer의 공개 번호 변경/완료 안내를 사용자 확인했다. 설정·키 바인딩 보존은 정적/자동 Test 책임으로 정리했다. 다음은 A의 새 Anonymous 안내·B의 이전 후 본인 행·Keyboard 화면 선택/Submit/Close 확인과 최신 사용자 전체 Unity Test 결과다. 설정 값을 인위적으로 변경해 재비교하는 작업은 제외하며 Step 11/12·Phase 2 전체는 미완료다.

2026-10-06 최신 범위는 **코드/인증값 개별 복사 버튼 두 개 + 일반 Ctrl+V 입력**이다. 사용자 요청으로 전용 Paste 버튼/파서를 제거했다. 다음은 Task의 수정된 Scene 연결→사용자 컴파일/전체 Test→11-F 입력/이전 확인이다. 아래 Copy/Paste Transfer Details 안내는 변경 전 이력이며 Step 11 전체는 미완료다.

2026-10-06 Step 11-E 발급/재발급/취소 UI 보고와 복사 요청을 반영해 Client Copy/Paste Transfer Details를 준비했다. 다음은 Phase 2 Task의 사용자 Scene 버튼 두 개 생성·View 참조 연결 → Unity 컴파일/전체 Test → 11-F 복사·붙여넣기로 실제 A→B 이전이다. Node 38파일·50 C# 정적 검사와 실제 Unity 실행을 구분하며 Step 11/12·Phase 2 전체는 미완료다. 복사 자격 증명의 OS clipboard 잔존은 AccountTransfer 정책에 별도로 기록했다.

2026-10-05 사용자 폐기 완료 안내 확인으로 11-D 수동 화면 확인을 완료했다. 다음은 Phase 2 Task **11-E 코드 발급·창 닫기·재발급·취소 UI 확인**이다. 최신 코드 수정 이후 전체 Unity Test와 이후 화면 항목은 대기이며 Step 11/12·Phase 2 전체는 미완료다. 아래 11-D 화면 재확인 대기 안내는 해결 전 이력이다.

2026-10-05 Step 11-D의 Offline/온라인 복귀·Submitted·입력 UI를 사용자 확인했고 폐기 완료 안내 가독성을 개선했다. 다음은 Phase 2 Task 11-D 재확인(사용자 최신 컴파일/전체 Test → B에서 완료 문구가 유지되는지 확인)이다. 그 뒤 11-E~G 발급/취소·게임 이전·조작 확인을 진행한다. Node 38파일·50 C# source preflight 및 Test 작성과 실제 Unity 실행 근거를 구분하며 Step 11/12·Phase 2 전체는 미완료다.

2026-10-05 Step 11-C의 사용자 전체 공개 번호/TOP `(You)` 확인 뒤 AROUND YOU 인증 실패와 한글 표시 문제에 대응했다. Client 동시 인증 결과 공유·영어 안내 및 회귀 Test를 수정했고 Node 38파일·50 C# 정적 검사를 통과했다. 다음은 **사용자 컴파일·전체 Edit/Play Mode Test → A의 게임 Leaderboard Retry 및 Settings의 Online Record Notice 확인**이다. Step 11/12·Phase 2 전체는 미완료이고 901/255는 이번 수정 전 근거다. Scene/서버 변경 없이 기존 A/B를 유지한다.

2026-10-05 Step 11 정적 준비 완료 / 사용자 실제 Game 화면 확인 대기. Phase 2 Task 11-A~H에 격리 예약 취소·A/B 실행 인수, 사용자 Scene 문구 정리, Offline/Pending·발급/취소·A→B 완료·Keyboard/Mouse 절차를 작성했다. 실행 코드·Scene·서비스 변경 없이 정적 계약/preflight를 통과했고 최신 사용자 901/255 회귀 근거를 유지한다. 다음은 사용자 화면 작업이며, 결과 미보고로 Step 11/12·Phase 2 전체·Production 승인은 미완료다. 아래 Step 10-3 완료 근거는 유지한다.

2026-10-05 최신사용자 Unity컴파일성공·EditMode901/901·PlayMode255/255/모든예상밖ErrorWarning없음과verification 실제연결교체/source거부·복구/사용credential타호출자거부·번호/고정행metadata·동일ID후속제출·재시작보존을대조해 **Step10-3완료**했다. Step10 실행범위는10-1/10-3완료·10-2대상없음사용자승인N/A이며legacy전환PASS아니다. **다음은Step11 생산화면/Offline흐름확인**이다. 현재C활성A/B새Anonymous역할을유지하고추가원격발급/제출을지금요구하지않는다. Step12/Phase2전체/Production운영승인은미완료이며latecommit/기동통신원인·실서비스경합/다중원자성·정밀5초미확인한계는별도유지한다. 아래미완료/회귀대기안내는이전이력이다.

2026-10-05 Step10-3 최종활성A 재시작공개보존PASS(1192ms)까지 실제시나리오근거를확보했다. sourceInactive/ActiveDeviceRequired·정상새Anonymous복구·사용credential다른호출자거부·고정행metadata·동일ID후속확정/재시작보존확인. 다음은추가원격실행이아닌 **최신수정후사용자Unity컴파일/전체EditMode·PlayMode/예상밖ErrorWarning결과확인**이다. 미보고회귀로Step10-3최종완료는대기한다. 기존timeout/latecommit·플랫폼/통신원인/실서비스원자성미확인은Step12최종판정에명시하며Phase2전체완료/운영승인으로확대하지않는다. AI빌드/TestRunner/Scene/원격호출없음.

2026-10-05 B 공개 보존 실제PASS(1149ms) 뒤 후속60초 도구는 status반복/전체timeout·Pending1로 미확정이다. 동일 저장ID 단1회 처리/서버Submitted·로컬저장 확인만Pending제거하는 Client-only 검증 버튼을 준비했고 새기록/ID/자동retry는없다. EditMode5사례준비/Node38파일/preflight49C# 통과와 실제Unity/원격대기를구분한다. 다음은 Task10-3-Q 사용자컴파일/전체Test→기존B Pending1재전송→확정뒤별도보존조회다. 이번 Module게시/Scene/초기화/AI빌드·TestRunner·원격없음, Step10-3미완료유지.

2026-10-05 Step10-3 완료 필요 작업 요청에 따라 완료 항목/미확인 직접 source 거부·B 후속 제출/재시작·자격 재사용/첫 요청 timeout·최근 회귀를 Task10-3-O로 정리했다. B 공개 기준 비교의 반복 인증/완료 복구를 제거하는 Client-only 변경과 mismatch 안전 분류/4 EditMode 사례 준비, Node38파일/정적 계약/preflight 통과. 실제 Unity/원격 성공은 대기이며 다음은 사용자 컴파일/전체Test→기존 B에서 원래 A baseline의 서버 본인 행 보존 비교다. 이번 변경에는 Module 재게시/Scene/초기화가 없고 새 이전은 아직 하지 않는다. 기존 Dashboard 보존 PASS와 UI 실패를 구분하고 Step10-3/Phase2 미완료 유지.

2026-10-05 이전 Module 게시 성공과 Editor 재시작 첫 요청 실패 관찰을 받아 shared HTTP 연결 풀(요청별 token/CTS/5초 유지) 및 안전한 auth/client/server 시간·요청 순번/호출 횟수 진단을 추가했다. 자동 예열/자동 retry/늦은 상태 적용은 없다. Node38파일/정적 계약/Client preflight 및 DTO EditMode1사례 준비와 실제 컴파일/.NET/Jint/원격 미검증을 구분한다. 다음은 Task 10-3-N 사용자 컴파일/전체 Test·이번 Module 새 빌드/verification 게시 후 기존 A 최초/후속 읽기 요청 시간 비교다. HTTP 반복 초기화 제거는 cold-start 원인 확정/해소 보장이 아니며 Scene/계정·기록 초기화/AI 빌드·Test Runner·원격 없이 Step 10-3 미완료 유지.

2026-10-05 A 원래 세션 관찰 두 번 timeout 후 사용자 승인으로 서버 Inactive source의 중복 조회/불필요 recovery를 제거하고 창 상태/기록 조회 버튼을 분리했다. 각 클릭 전체 5초/원래 인증 대조·권한/CAS·늦은 결과 차단은 유지한다. 새 endpoint 대역 4사례 포함 Node 38파일/정적 계약/Client preflight 통과와 실제 Unity/.NET/Jint/원격 미확인을 구분한다. 다음은 Task 10-3-M의 사용자 컴파일/전체 Test·verification Module 재빌드/게시 후 A 상태/조회 각각 확인이다. AI 빌드/Test Runner/Scene/원격 호출은 없고 Step 10-3/Phase 2 전체는 미완료다. 아래 기존 단일 버튼 관찰 안내는 이번 분리 이전 이력이다.

2026-10-05 Step 10-3-K Dashboard 이전 후 자료의 오프라인 비교가 **PASS / TRANSFER_FIXED_ROW_BINDING_PRESERVED**다. 논리 계정/공개 번호/고정 owner 및 Stage score·acceptedAt·submissionId 유지, 활성 연결 교체·revision 1 증가·A Inactive/B Active와 Infinite 행 전후 없음을 확인했다. 기존 UI 공개 비교 FAIL/캡처 timeout·422는 미해결이며 실제 원자성·재사용 거부를 이 자료로 입증하지 않는다. 다음은 Task 10-3-L의 원래 A 전용 거부 관찰이다. 일반 계정 패널/Refresh·후속 제출은 그 전에 하지 않는다. 코드/Module/Scene/빌드/Test Runner 변경 없이 Step 10-3/Phase 2는 미완료로 유지한다. 아래 준비 단계 안내는 이전 이력이다.

2026-10-05 사용자 승인에 따라 Step 10-2는 **적용 대상 없음(N/A)·실제 전환 검증 미실시로 종료**한다. 기존 자료 최초 전환이 원격 PASS한 것은 아니며 향후 legacy 환경 검증은 별도로 필요하다. 다음은 Step 10-3이며 기존 A/B/기준을 유지하고 우선 A Ready/Pending=0을 사용자 확인한다. 내부 기준 확보 전 Start/완료/후속 제출을 하지 않는다. 상세 단계별 안내는 Phase 2 Task 10-3-A에 준비했다. 새 코드/Unity Test/Module/Scene/원격 seed·초기화는 없고 Step 10-3/Phase 2 전체는 미완료다. 아래 10-2 미완료/사례 확인 대기 안내는 승인 전 이력이다.

2026-10-05 Step 10-2 정적/오프라인 준비를 완료했다. 최초 미전환 사례 판정·원본 행/metadata/ledger receipt/best/source snapshot·재실행 유지의 compare-legacy-cutover 도구와 로컬 template, Dashboard 읽기→동일 Legacy 인증 전환→재시작 비교의 상세 지침을 추가했다. production SDK 대역 80검사/전체 Node 38파일/정적 계약/Client preflight 통과, Unity/Module/Scene 코드는 변경하지 않았다. 다음은 사용자 미전환 기존 기록 및 원래 인증 프로필 존재 확인이다. 기존 자료 미보존 결정을 유지하며 A/B 재사용/초기화/구 Script 재게시/승인 없는 합성 원격 사례는 하지 않는다. 실제 서비스 기준이 없어 Step 10-2와 Phase 2 전체는 미완료다. 상세 절차는 Phase 2 Task 최신 10-2-A~D를 따른다.

2026-10-05 최신 사용자 컴파일 성공·EditMode 891/891·PlayMode 255/255·각각 예상 밖 Error/Warning 없음, 기존 A/B 서비스/오프라인 distinct·A 재시작/Player 403·서버 5초 Module 게시·B 최초 사용 근거로 Step 10-1을 완료하고 Step 7 최신 회귀 근거를 갱신했다. **다음 작업은 Step 10-2**이며 Step 10-3·11/12·Phase 2 전체는 미완료다. 창 실제 5초 종료 관찰/서버 시간 경계는 미보고로 Test 성공과 구분한다. 기존 기준/계정을 유지하고 신규 제출/초기화/재게시/Scene 변경은 반복하지 않는다. 아래 미완료·회귀 대기 안내는 과거 이력이며 최신 절차는 Phase 2 Task 완료 항목을 따른다.

2026-10-05 최신 사용자가 B 최초 온라인/서버 5초 Module 게시·Client 컴파일 성공/Unity Test 미실행을 확인했다. A/B Step 10-1 원격 결과 기록은 완료했으나 회귀 확인이 남아 전체 Step은 미완료다. 창 10초 이상 대기에 대응해 현행 Phase 2 창의 버튼 한 번에 auth/transport/clear/retry-delay/direct Write가 공통 5초 deadline을 공유하도록 수정하고 client Module timeout도 5초로 낮췄다. timeout은 WINDOW_TIMEOUT/late 결과 미적용/Pending 보존이며 정확한 메인 스레드 강제 선점은 보장하지 않는다. Node 37파일/정적 계약/Client source preflight 통과, EditMode 7사례 추가. 다음은 사용자 컴파일·전체 Unity Test 및 조회 전용 시간 제한 확인이고 이번 Client-only 수정에는 Module 재게시/Scene/저장 초기화가 필요 없다. 상세 최신 절차는 Phase 2 Task에 있다.

2026-10-05 후속 서버 제한 요청 반영: Module 9함수의 HTTP/body/Jint/Secret에 invocation 공통 5초 budget을 적용했다. 만료 후 새 호출/성공 반환을 거부하지만 이미 반영된 쓰기는 삭제/rollback하지 않는다. 기동·통신을 포함한 5초 클라이언트 수신 보장은 별개다. Node 37파일/정적 계약/Client preflight 및 서버 3파일 정적 검사는 통과했고 .NET/Jint 실시간·빌드·원격은 미확인이다. 다음은 사용자의 verification FlowStateVerification Module 하나 Deploy Selected와 계정 확인/B 검증이다. 바로 아래 Client-only 재게시 불필요 안내는 이번 서버 수정에 적용하지 않는다. Scene/Secret/정책/데이터 초기화 없이 진행하고 Step 10-1은 미완료다.

2026-10-05 사용자 A 제출·기준 캡처·재시작 번호/점수/수락 시각 보존·Player 직접 Write 두 서비스 403 결과가 성공했다. 요청에 따라 계정 패널 확인 전체 SDK 대기에 공통 5초 제한을 적용했고 늦은 응답/자동 재시도는 차단한다. Client-only 변경이므로 이번에는 Module 재게시/Scene 작업이 필요 없다. Node 36파일/정적 계약/C# 48파일 source preflight 통과, 새 EditMode 5개 포함 실제 Unity 컴파일/Test는 사용자 대기다. 다음은 사용자 컴파일/Test 확인 후 Step 10-1-D의 별도 B 검증 및 기존 로컬 기준 파일 distinct 비교다. A 신규 제출을 반복하지 않는다. Step 10-1은 아직 미완료이며 상세 절차는 Phase 2 Task 최신 항목을 따른다.

2026-10-04 최신 일괄 수정: provisioning의 JSON 키 순서 오판과 기존 행 Preparing 계정 재조회 복구 누락을 오프라인에서 재현·수정했다. 값/소유권/CAS 검증과 기존 점수/시각은 보존한다. Node 36파일·JavaScript 문법 74파일·정적 계약·C# 48파일 source preflight/중복 선언 검사를 통과했고 저장 전 실패/응답 유실 30개 지점 및 embedded JS shim의 제출/조회/이전도 확인했다. 실제 컴파일/.NET/Jint/원격 원인은 사용자 확인 대기다. 다음 작업은 사용자 컴파일/Test → verification FlowStateVerification Module 하나 Deploy Selected → 같은 A 조회 전용 Probe다. 이전 Client-only 재게시 불필요 안내는 이번 embedded 서버 수정에 적용하지 않는다. Scene/Secret/정책/데이터 초기화/Player Build 변경은 없으며 Step 10-1은 미완료다. 상세 절차는 Phase 2 Task 최신 일괄 수정 항목을 따른다.

2026-10-04 설치 SDK의 Edit Mode 초기화 거부와 검증 창의 Play 차단 충돌을 확인해 격리 예약→사용자 수동 Play 실행 경로로 수정했다. 예약된 게임은 메모리 저장·온라인 비활성, 창만 A/B 격리 세션을 사용한다. 자동 Test/Batch/비예약/비Play 요청 차단과 종료 정리는 유지한다. Scene/UI 생성·수정은 없으며 기존 Edit Mode 원격 안내는 폐기한다. Node 34개/정적 계약은 통과했고 실제 Unity 컴파일/Test 및 A 계정 Ready 결과를 기다린다. 현행 사용자 절차는 Phase 2 Task 10-1-A/C를 따른다. Step 10-1은 미완료이며 Module 재게시/Secret/빌드는 필요 없다.

2026-10-04 최신 A/B 확인에서 동의/로컬 저장 준비 후 SDK 인증이 `AuthenticationUnavailable`로 실패했다. 창의 개별/전체 메시지 복사와 민감 정보 제외·원문 확인, 안전한 인증 단계/실패 분류/SDK 코드 진단 및 PlayMode 복사 Test 2개를 준비했다. Node 34개/정적 계약은 통과했으나 Unity 실행과 실제 인증 원인은 확인 대기다. 같은 A 계정 패널 열기의 안전한 전체 복사 결과부터 확인하고 Step 10-1은 미완료로 유지한다. 세부 절차는 Phase 2 Task를 따른다. Module/Scene/Secret/Build 작업은 필요 없다.

2026-10-04 사용자 10-1-B의 활성 계정 준비 실패를 기록하고 Editor 검증 창의 잘림/불충분한 진단을 수정했다. 신규 제출·기준 캡처 전에 같은 A의 안전한 계정 State/Reason 결과를 확인한다. PlayMode 진단 Test 4개 추가와 레이아웃 정적 검사 준비를 실제 Unity 컴파일/Test/화면 확인과 구분한다. 원격 근본 원인과 Step 10-1/Phase 2는 아직 미확인/미완료이며 Module 재게시/Scene/Secret/빌드는 요구하지 않는다.

2026-10-04 Step 10-1의 Editor 검증 버튼/행 자동 판정·안전한 HTTP 결과와 사용자 A/B 신규 인증·제출·재시작 비교·403 지침을 준비했다. EditMode 판정 Test 9개 추가와 Node 테스트 파일 34개/정적 계약은 통과했지만 이번 변경 후 Unity 컴파일/Test 및 실제 verification 결과는 대기다. Step 9 적용 완료는 유지하고 Step 10-1/Phase 2는 미완료다. Module 재게시/빌드/Scene 작업은 요구하지 않는다. 세부 절차/결과 분류는 Phase 2 Task Step 10-1에서 관리한다.

2026-10-04 최신 사용자 verification 환경 게시 확인·Unity 컴파일 성공·EditMode 863/863·PlayMode 242/242 성공 및 모든 단계의 예상 밖 Error/Warning 없음, 앞선 Module 게시 성공/API 스펙 대조를 근거로 Step 9-3과 Step 9 적용 범위를 완료했다. **다음 작업은 Step 10-1 실제 서비스 검증**이다. Module 게시 version/hash는 미보고이며 추정하지 않는다. 실제 Module 실행·Secret 접근·서비스 권한/저장 원자성·필수 입력 누락 거부는 아직 검증하지 않았고 Step 10~12/Phase 2 전체는 미완료다. 아래 초기 Step 9 준비/확인 대기 설명은 과거 이력이다.

2026-10-04 Step 8 사용자 Scene 작업·영어 label 반영 및 읽기 전용 정적 대조, 최신 사용자 컴파일·Edit Mode 850/850·Play Mode 242/242 통과와 예상 밖 Error/Warning 없음으로 Step 7·8을 완료했다. 실제 위치에 맞춘 사용자 Navigation 변경과 계정/이전 UI의 영어 표시 결정을 유지한다.

Phase 2에서 공개 번호 발급·기존 자료 cutover·논리 계정의 단일 활성 연결·기기 이전·고정 Leaderboard 소유 행을 Cloud Code와 Client에 구현하고, 원자성·경합·응답 유실을 자동 검증한다.

실행 순서와 사용자 수동 적용·검증 근거는 `AI/90_Tasks/Prototype_8/20261003_01_Phase2ManualSteps.md`에서 관리한다. 2026-10-04 Step 9-1의 로컬 적용표(9개 bundled endpoint·같은 이름의 standalone bridge), 입력/의존성/호환 복구 대조와 읽기 전용 검사 준비를 완료했고 전체 Node Test 파일 31개가 통과했다. 사용자 실제 대상/설정 보고와 기존 query-records v2·submit-record v1의 로컬 source 백업 완료·Draft 없음 확인을 근거로 Step 9-1을 완료했다. 다음은 Step 9-2 Secret·권한·필요 설정 준비다. 원격 게시와 설정 변경은 수행하지 않았고 Step 9 전체 및 Step 10~12는 미완료다. Unity 자동 Test의 통과를 원격 서비스 검증이나 화면 가독성 승인으로 확대하지 않는다. 128/100 제한과 receipt 정리는 Phase 3로 유지한다.

## 보류된 작업

최신 상태 (2026-10-04): Step 9-3/Step 9의 게시 적용은 완료했고, 아래 게시/환경/Test 확인 대기 기록은 이전 상태다. 실제 서비스 검증을 수행하지 않았으므로 Step 10~12와 Phase 2 전체는 미완료로 유지한다. AI는 Unity 빌드/Test Runner/Scene 수정/원격 호출을 수행하지 않았다.

2026-10-04 다운로드된 `UGS/FlowStateVerification.yaml`의 Project/Module·함수 9개·입력 이름/String type은 서버/Client와 정적 일치 확인했다. YAML은 Environment ID/required 선언을 포함하지 않는다. Step 9-3은 이번 변경 후 Test/Error/Warning 및 실제 verification 대상 확인 대기이며, 실제 서비스 인증/Secret/권한 검증은 Step 10에 남긴다. 같은 함수/입력 대조를 사용자 수동 작업으로 반복하지 않는다.

2026-10-04 사용자 Unity Editor 컴파일 성공·`FlowStateVerification.ccmr` 게시 성공 보고를 기록했다. 아래 Module 준비 시점의 빌드/게시 미검증 기록은 과거 상태다. 이번 변경 후 예상 밖 Error/Warning·EditMode/PlayMode 결과 및 실제 Project/verification 대상·원격 함수 9개/입력 확인을 기다린다. 재게시 없이 남은 확인을 받으며 Step 9-3/9 전체/Phase 2는 아직 미완료다. 실제 Module 서비스 호출과 Secret/권한 검증은 Step 10에 남긴다.

2026-10-04 사용자 진행 승인 후 `FlowStateVerification` C# Module과 고정 9개 Client 함수 연결을 준비했다. 기존 JS 계정/이전 로직은 Jint embedded source로 재사용하고 암호는 .NET 표준 구현에 연결한다. Node 테스트 34개/정적 계약 통과와 실제 .NET Module 빌드·게시·원격 실행 미검증을 구분한다. Step 9-3은 `.ccmr` 하나의 사용자 게시/함수 입력 확인 대기이며 기존 9개 JS 직접 게시 지침은 더 이상 적용하지 않는다. Secret/정책/Board를 유지하며 실제 서비스 동작은 Step 10에서 검증한다. Unity 빌드/Test Runner/Scene 작업은 AI가 수행하지 않았다. 아래 방향 승인 전 기록은 과거 진단 이력이다.

2026-10-04 사용자 start-account-transfer 서버 상세 `Cannot find module 'crypto'`로 Step 9-3의 crypto import 부재를 확정했다. 현재 JavaScript 적용표로는 6개 게시가 완료되지 않는다. CSPRNG/HMAC 보장을 유지할 수정 경로가 필요하며 Cloud Code C# Module 전환은 서버/Client transport/Test/배포 구조 변경을 포함하므로 사용자 방향 확인 후 준비한다. Secret/정책 변경이나 재게시 반복은 요구하지 않는다. Step 9-3/9 전체/Phase 2는 미완료다.

2026-10-04 Step 9-3 첫 사용자 게시는 3개 성공/6개 Compilation Error로 미완료다. 설치 JS bundler의 로컬 출력 검사에서 crypto import 6개와 실패 6개가 일치했으며 원격 지원 문제를 우선 의심한다. 서버 컴파일 상세 메시지는 미확인이다. 로컬 진단 회귀 포함 Node Test 파일 33개 통과와 원격 실패를 분리 기록하고, 호출 중지·오류 상세 확인 후 안전한 수정 방향을 정한다. 기존 Script 성공만으로 Step 10을 시작하지 않는다.

2026-10-04 사용자가 현재 운영 배포 전 개발 상태임을 확인해 Step 9-3의 bridge 게시/구버전 실행 로그 배출 보고를 제외했다. 현재 적용은 앱/검증 요청 중지 후 최종 9개 직접 게시다. 앞선 bridge 포함 계획을 대체하며 실제 게시 결과 확인 전까지 Step 9-3은 미완료다.

2026-10-04 Step 9-3 AI 준비에서 최종 9개 Editor 게시 entry/meta와 원본 참조/설치 패키지 parser 대조를 완료했고 전체 Node Test 파일 32개/정적 계약이 통과했다. 사용자 작업은 bridge 게시·이전 실행 종료 근거·Deployment/JS 초기화·verification Deploy Selected·실제 version/입력 보고다. 원격 게시 결과가 없으므로 Step 9-3 및 Step 9 전체는 미완료이며 세부 순서는 Phase 2 Task에 둔다.

2026-10-04 Step 9-2 AI 준비에서 verification 환경 Secret 이름/형식·Cloud Code Service access·기존 Deny 유지와 사용자 전용 생성 도구를 확정하고 가상 값 자체 검사·전체 Node Test 파일 31개·정적 계약을 통과했다. 이후 사용자 Secret 값 작업 완료·verification 환경에만 유효·Cloud Code만 접근 가능 확인을 근거로 Step 9-2를 완료했다. 다음은 Step 9-3이며 Script 게시·실제 권한/Secret 접근 검증은 Step 9-3/10에 남긴다. Step 9 전체와 Phase 2 전체는 아직 미완료다.

2026-10-04 13:40 KST 사용자 보고로 Step 9-1 실제 대상·기존 Script 2개(version 2/1)·신규 대상 7개·권한/Board 설정·Secret 부재를 기록하고, 후속 source 백업 완료·Draft 없음 확인으로 Step 9-1을 완료했다. 기존 테스트 계정의 인증 프로필/Local Save 보관은 사용자 결정으로 제외한다. 이 결정은 원격 삭제나 migration 검증 생략을 승인한 것으로 해석하지 않는다.

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
