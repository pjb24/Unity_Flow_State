# 작업 정보

## 작업명

Prototype 7 Phase 1 — 저장 범위와 Leaderboard 운영 정책 수동 작업 계획

## 작업 일자

20260924

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. Phase 1 실행과 아래 Step의 완료 판정은 대기 상태다.

# 작업 목적

Roadmap 007 Phase 1에서 영구 저장 대상, 기록 구조, 순위와 계정·Offline 운영 정책을 확정한다. 사용자는 제품·운영 선택만 수행하고, 기존 계약 조사와 결정 가능한 규칙의 판정은 정적 검사 및 Edit Mode Unit Test로 처리한다.

# 작업 대상

- 로컬·서버·Runtime 전용 데이터의 경계
- Player 식별, Anonymous 계정과 기기 변경·복구 정책
- Stage별 Clear Time 및 InfiniteMode Total Score 기록과 순위 정책
- 동점, Player별 최고 기록, Scoring Version·Game Version 및 기록 초기화
- Offline 제출 대기·중복 방지·재시도·서비스 장애·삭제 정책
- Client Score 신뢰 수준과 서버 검증 범위
- 관련 Project·Feature·System 문서, 순수 정책 모델과 Edit Mode Unit Test

로컬 저장소 구현, Unity Gaming Services 연결, 실제 인증·전송, Leaderboard 화면, Scene 편집과 Player Build는 Phase 1 대상이 아니다.

# 작업 전 상태

- Roadmap 005·006은 완료됐다. Roadmap 007 Phase 1~4는 대기 중이다.
- `PROJECT_OVERVIEW.md`, `ARCHITECTURE.md`, `PROJECT_MEMORY.md`에는 현재 버전의 로컬·서버 저장 제외와 Runtime 전용 데이터 규칙이 남아 있다. Phase 1 결정 후 해당 범위를 갱신해야 한다.
- `TimeRecord`는 Stage의 `Cleared` 또는 `Fell` 결과를 확정한다. Stage Leaderboard에 제출할 결과 원인은 별도 정책으로 확정해야 한다.
- `ScoreRecord`는 InfiniteMode의 Total Score와 Scoring Version을 확정한다. 서로 다른 Scoring Version의 기록을 비교하지 않는 규칙이 이미 있다.
- 현재 `ResultData`에는 Stage ID가 없다. `GameNavigationState.CurrentRunId`는 현재 Navigation 인스턴스의 Run 시작마다 증가하며 영구 제출 ID 계약은 없다. Stage 구분 Key와 재시도 간 유지되는 제출 식별자는 Phase 1에서 정의하고 구현은 후속 Phase에 배치한다.
- Prototype 6 Phase 4는 자동 안내·재열람·Binding 표기와 전체 회귀를 완료했다. Tutorial 완료 상태, Settings 및 Binding의 영구 저장과 실제 Leaderboard는 Roadmap 007에 남겨 두었다.

# 조사 내용

`AI/README.md`의 Project·Rules 확인 순서, Roadmap 007, Prototype 6 Phase 4 완료 기록, `TimeRecord.md`, `ScoreRecord.md`, `Leaderboard.md`, `ResultSystem.md`와 현재 `ResultData`·Navigation 상태를 확인했다. `INVESTIGATION_RULE.md`와 `VERIFICATION_RULE.md`에 따라 확정 정책과 미정 정책을 구분하고, 자동 판정 가능한 상태·정렬·중복 규칙은 Unit Test 대상으로 배치한다.

# 작업 내용

AI는 제안·장단점, 정적 조사, 계약 문서, 순수 정책 코드와 Unit Test를 담당한다. 사용자는 아래 Step에서 명시한 정책 선택과 Unity Editor의 Compile·Test Runner 실행만 담당한다. AI는 Unity Editor, Test Runner 또는 Build를 실행하지 않는다.

## Step 1. 저장·기록 경계를 정적으로 조사한다

### AI 작업

1. Runtime Data, `ResultData`, Settings·Input Binding·Tutorial 상태와 기록 생성 경로를 코드·문서에서 목록화한다.
2. 현재 Stage ID, Player ID, Game Version, 제출 ID와 영구 저장 수단의 존재 여부를 검색하고, 확인된 것과 미구현 항목을 구분한다.
3. 기존 저장 제외 문구와 Roadmap 007의 변경 대상, Phase 2~4로 넘길 구현 범위를 대조한다.

### 사용자 수동 작업

없음. 파일·코드·문서로 판정할 수 있는 사항은 AI가 조사한다.

### 완료 조건

- [ ] 저장 후보와 기존 생산 데이터 원천, 누락된 식별자 및 문서 충돌 목록이 작성됐다.

## Step 2. 저장 대상과 계정 정책을 결정한다

### AI 작업

1. 데이터 항목별 `Runtime 전용 / 로컬 영구 / 서버` 배치안을 제시한다. Settings 값·Input Binding·Tutorial 완료·개인 최고 기록·제출 대기열·계정 식별 정보·Run 결과를 빠짐없이 포함한다.
2. Anonymous Authentication의 최초 생성, 재설치·기기 변경 시 기록 접근, 계정 연결·복구 지원 여부와 사용자에게 표시할 제한을 선택지와 장단점으로 제시한다.
3. 불필요한 개인정보·Secret 저장을 요구하는 안이 없는지 계약과 대조한다.

### 사용자 수동 작업

1. AI의 데이터 항목별 표를 보고 각 항목의 저장 위치와 보존 여부를 결정한다.
2. Anonymous 계정만 사용할지, 계정 연결·복구를 제공할지, 기기 변경·재설치 시 기존 기록을 복구할 수 없는 경우 어떤 안내를 할지 결정한다.
3. 결정 내용과 남겨 둘 예외를 채팅으로 전달한다. Unity Editor나 서비스 Console 작업은 하지 않는다.

### 완료 조건

- [ ] 로컬·서버·Runtime 전용 범위와 Anonymous 계정의 복구 제한이 확정됐다.

## Step 3. 제출 기록과 순위 정책을 결정한다

### AI 작업

1. Stage 제출 가능 원인(`Cleared`/`Fell`), Stage 구분 Key, Clear Time 저장 단위·반올림 기준, InfiniteMode 제출 가능한 최종 결과와 Total Score 필드를 확인해 선택지로 제시한다.
2. Stage별 Clear Time 오름차순, InfiniteMode Total Score 내림차순의 확정 기준 위에서 동점의 순위·표시 순서, Player별 최고 기록 교체 조건, 동일 점수의 재제출 처리안을 장단점과 함께 제시한다.
3. 기록 Key에 들어갈 Mode·Stage·Scoring Version·Game Version의 역할과 규칙 변경 시 새 Leaderboard 분리 또는 초기화 정책을 제시한다. 서로 다른 Scoring Version을 비교하지 않는 기존 계약은 유지한다.

### 사용자 수동 작업

1. Stage 제출 원인, Stage ID 형식, 시간 정밀도와 반올림, InfiniteMode 제출 결과 범위를 결정한다.
2. 동점 순위와 표시 순서, Player 최고 기록 교체·동점 재제출 규칙을 결정한다.
3. Score 규칙 및 Game Version 변경 시 기존 기록 보존·분리·초기화 정책을 결정하고 채팅으로 전달한다. 실제 기록이나 서버 데이터를 만들거나 삭제하지 않는다.

### 완료 조건

- [ ] 두 Mode의 Record 구조·순위·동점·최고 기록·Version 정책이 모순 없이 확정됐다.

## Step 4. Offline·운영·신뢰 정책을 결정한다

### AI 작업

1. Offline 또는 Authentication·서비스 실패 시 로컬 Result 유지, 제출 대기, 재시도 트리거·횟수·영구 실패 처리와 같은 Run의 중복 전송 방지안을 제시한다.
2. 제출 ID의 생성 시점·재실행 후 유지 범위, 대기열 보존·삭제·계정 전환 시 귀속 규칙을 제시한다. 현재 `CurrentRunId`만으로 영구 중복 방지가 가능한지 정적으로 판정한다.
3. 로컬·서버 기록 삭제, 계정 초기화·복구, Client Score 신뢰 수준과 서버에서 검증할 최소 필드를 제시한다. 강화된 부정행위 방지의 후속 범위도 분리한다.

### 사용자 수동 작업

1. Offline 기록의 보존·자동/수동 재시도·포기 조건과 서비스 장애가 Run 결과 또는 Menu를 막지 않는 정책을 결정한다.
2. 중복 제출 방지, 계정 변경 시 대기 기록 처리, 기록 삭제 요청과 계정 복구 범위를 결정한다.
3. Client가 계산한 기록을 어느 수준까지 신뢰할지와 Phase 3 서비스 연결 시 필요한 검증·운영 범위를 결정해 채팅으로 전달한다. 네트워크 차단 실험이나 실제 서비스 설정은 하지 않는다.

### 완료 조건

- [ ] Offline·재시도·중복 방지·삭제·복구·신뢰 정책이 확정됐고 플레이 차단 금지 조건이 명시됐다.

## Step 5. 확정 정책을 문서 계약과 Test 명세로 반영한다

### AI 작업

1. 확정된 저장 범위만 Project 문서에 반영하고, 기록·Leaderboard 동작 규칙은 해당 Feature 문서에 반영한다. Result 생성과 저장·제출의 System 책임을 분리한다.
2. 결정마다 정상·거부·경계 사례를 작성한다. 기록 Key 분리, 정렬·동점, 최고 기록 갱신, Version 불일치, 동일 제출 ID, Offline 대기·재시도를 Edit Mode Unit Test 항목에 연결한다.
3. Stage ID·영구 제출 ID·실제 저장소·SDK·UI 연결 가운데 Phase 2~4 구현 대상과 현재 Phase 1 순수 정책 대상의 경계를 명시한다.

### 사용자 수동 작업

없음. 사용자가 Step 2~4에서 승인한 결정은 AI가 문서와 Test 명세로 반영한다.

### 완료 조건

- [ ] Project·Feature·System 문서가 확정 정책과 일치하고, 모든 결정 가능한 규칙에 Test 사례가 연결됐다.

## Step 6. 순수 정책 모델과 Edit Mode Unit Test를 작성한다

### AI 작업

1. 확정 정책 중 순수하게 계산·판정 가능한 Record 유효성, Board Key, 정렬·동점, 최고 기록 선택, Version 분리, 동일 제출 ID 거부와 Offline 제출 상태 전이를 필요한 최소 모델로 구현한다.
2. 실제 생산 모델을 호출하는 Edit Mode Unit Test로 정상·거부·경계값을 검증한다. Test 안에 생산 정렬·계산 로직을 복제하지 않는다.
3. Scene·프레임·외부 SDK·네트워크·실제 파일에 의존하지 않는지, 기존 기록 생성과의 책임 중복이 없는지 정적으로 검사한다.

### 사용자 수동 작업

없음. 코드·Test 작성과 정적 검사는 AI가 수행한다.

### 완료 조건

- [ ] 확정된 순수 정책의 생산 코드와 대응 Unit Test가 작성되고 정적 대조를 통과했다.

## Step 7. Unity Script Compilation과 자동 Test를 확인한다

### AI 작업

1. 코드·asmdef·문서·Test 이름과 변경 범위를 정적으로 검사하고, 집중 Edit Mode Fixture와 영향받는 전체 회귀 범위를 지정한다.
2. 사용자가 전달한 실패 Test 이름·메시지·Stack Trace를 분석해 수정한 뒤 해당 범위를 다시 지정한다.
3. 실제 실행 결과와 예상하지 않은 Error/Warning 유무만 완료 근거로 기록한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error/Warning 유무를 확인한다.
2. AI가 지정한 집중 Edit Mode Fixture를 Unity Test Runner에서 실행한다.
3. 공통 Core·Record 계약 변경의 영향 범위에 따라 AI가 지정한 전체 Edit Mode 및 필요한 Play Mode 회귀를 실행한다.
4. 각 실행의 시도·성공·실패 개수와 예상하지 않은 Error/Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 전달한다.

Phase 1에서 Scene과 생산 플레이 경로를 변경하지 않은 경우 화면 확인과 Player Build는 요구하지 않는다. 정적 검사나 Unit Test로 판정할 수 있는 수치·상태를 수동 조작으로 중복 검증하지 않는다.

### 완료 조건

- [ ] 최신 변경 기준 Script Compilation과 지정된 자동 Test가 모두 성공하고 예상하지 않은 Error/Warning이 없다.

## Step 8. Phase 1 완료 근거와 후속 범위를 정리한다

### AI 작업

1. Roadmap 007 Phase 1의 각 완료 조건을 확정 정책, 정적 대조와 실제 Test 결과에 연결한다.
2. 결정하지 않은 항목이나 실패한 검증이 있으면 완료 처리하지 않는다. Phase 2의 로컬 저장 구현, Phase 3의 실제 서비스, Phase 4의 UI·Build 작업을 구분한다.
3. 필수 근거가 모두 충족되면 Phase 1과 Roadmap 진행 상태를 갱신하고 검증 결과를 Task 영역에 기록한다.

### 사용자 수동 작업

없음. 추가 Unity 작업이 필요한 경우에만 Step 7의 미충족 항목을 특정해 요청한다.

### 완료 조건

- [ ] 저장·계정·순위·Version·Offline·운영 정책과 Unit Test 근거가 모두 기록되고 Phase 1 상태가 갱신됐다.

# 영향 범위

이번 작업은 `AI/90_Tasks/Prototype_7`에 Phase 1 실행 계획을 추가한다. 정책 승인 후 Project·Feature·System 문서와 순수 정책 코드·Test가 변경될 수 있다. Scene, 외부 서비스 설정과 실제 사용자 데이터는 이 계획에서 변경하지 않는다.

# 검증 내용

- Roadmap 007 Phase 1의 구현 대상·완료 조건을 Step 1~8에 대응시킨다.
- 정책 선택만 사용자 수동 작업으로 남기고, 기존 데이터·코드 조사와 결정적인 규칙 판정은 정적 검사·Unit Test로 배치한다.
- 실제 Unity 실행이 필요한 Compilation과 Test Runner만 Step 7에 배치한다.
- Phase 2~4의 저장소·서비스·UI 구현을 Phase 1 완료 조건에 포함하지 않는다.

# 검증 결과

이 문서는 실행 계획이다. 정책 결정, 생산 코드·Unit Test 구현과 Unity 검증은 아직 수행되지 않았으며 Phase 1 완료로 기록하지 않는다.

# 후속 작업

Step 1의 정적 조사부터 시작하고, Step 2~4에서 사용자 정책 결정을 받은 뒤 계약과 순수 Unit Test를 작성한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/03_Features/TimeRecord.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/Leaderboard.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_6/20260923_01_Phase4ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 선택과 Unity 실행을 Step별 수동 작업으로 구분했다.
- [x] 정적 검사와 Edit Mode Unit Test 우선 원칙을 명시했다.
- [x] Phase 1과 후속 Phase의 책임 경계를 구분했다.
- [x] 정책·Test 미실행 상태를 완료로 기록하지 않았다.
