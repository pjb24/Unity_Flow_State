# 작업 정보

## 작업명

Prototype 7 Phase 2 — 로컬 기록·설정 저장 계층 수동 작업 계획

## 작업 일자

20260926

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. Phase 1은 완료됐으며 Phase 2 구현과 아래 Step의 완료 판정은 대기 상태다.

# 작업 목적

Roadmap 007 Phase 2에서 게임 로직과 저장 수단을 분리한 로컬 기록·설정 저장 계층을 구축한다. 수치 계산, 상태 전이, 직렬화, Migration, 손상 복구 및 중복 방지는 정적 검사와 Unity Edit Mode Unit Test로 우선 검증한다. 재실행과 실제 사용자 저장 경로처럼 실행 환경이 필요한 사항만 사용자 수동 작업으로 남긴다.

# 작업 대상

- Record Submission Service와 Memory·Local·Online Record Repository 경계
- 개인 최고 기록과 Offline 제출 대기열
- Tutorial 완료, Settings 및 Input Binding 영구 저장
- Save Version, Migration, 누락·손상·지원하지 않는 Save의 안전한 기본값 복구
- 재실행 후에도 유지되는 Run ID와 중복 제출 방지
- 저장 계층과 기존 Result·Settings·Navigation Runtime 상태의 연결
- 관련 계약 문서, 생산 코드와 Edit Mode·Play Mode Test

Unity Gaming Services Authentication·실제 온라인 제출·조회는 Phase 3 대상이며, Leaderboard 화면은 Phase 4 대상이다. Phase 1에서 확정하지 않은 Save 형식·Migration 세부 규칙은 구현 전 조사와 필요한 결정으로 처리한다.

# 작업 전 상태

- Roadmap 007의 Phase 2는 대기 상태다.
- Phase 1 Step 1~8은 완료됐다. 저장 위치·계정·기록·Version·Offline 운영 정책과 순수 정책 모델의 Unity Script Compilation 및 Edit Mode Test 716개 성공이 기록됐다.
- 현재 Run의 `ResultData`는 Runtime 전용이다. Settings, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록과 제출 대기열은 로컬 영구 저장 대상으로 확정됐으나 실제 저장 연결은 아직 없다.
- 현재 `GameNavigationState.CurrentRunId`는 Navigation 인스턴스 내에서만 증가하므로 영구 중복 제출 식별자가 아니다.
- 현재 프로젝트에는 PlayerPrefs, 파일 기반 저장, Authentication 및 Leaderboard SDK 구현이 없다.

# 조사 내용

`IMPLEMENTATION_ROADMAP_007.md`는 Phase 2에서 로컬 저장 계층과 Repository 경계를 구현하고, 직렬화·Migration·손상 복구·중복 방지는 Edit Mode Test로, 재실행 복구는 Play Mode Test로 검증하도록 정의한다. `20260924_01_Phase1ManualSteps.md`는 Phase 1 정책 확정과 Unity 검증을 완료했고, 실제 로컬 저장·Run 식별자 연결을 Phase 2로 넘겼다.

# 작업 내용

AI는 기존 코드·문서의 정적 조사, 계약·생산 코드·자동 Test 작성, 정적 대조와 실패 분석을 담당한다. 사용자는 아래에 명시된 Unity Editor의 Compile·Test Runner 실행 및 대상 플랫폼 Player 확인만 수행한다. 필요한 Scene·Inspector 변경은 AI가 대상과 설정 방법을 안내하고 사용자가 수행한다. AI는 Unity Editor, Test Runner, Player Build, Scene 편집 또는 실제 사용자 저장 파일 조작을 수행하지 않는다.

## Step 1. Phase 1 선행 정책과 구현 경계를 정적으로 확인한다

### AI 작업

1. Phase 1에서 승인된 저장 위치, 계정, Record Key, Version, Offline·삭제 정책을 계약 문서에서 확인한다.
2. `ResultData`, Settings, Input Binding, Tutorial, Navigation과 Runtime Data의 현재 생산·초기화·폐기 경로를 다시 대조한다.
3. 로컬 저장 구현만 Phase 2에 포함하고, 실제 SDK·온라인 전송·Leaderboard UI가 Phase 3~4에 남는지 확인한다.

### 사용자 수동 작업

없음. 이미 확정된 Phase 1 정책은 재결정하지 않는다. Save 형식·Migration처럼 아직 계약이 없는 선택이 실제 구현에 필요하면 AI가 대안과 영향을 정리해 해당 항목만 요청한다.

### 완료 조건

- [ ] 구현할 저장 항목, 금지된 외부 서비스 범위 및 Phase 1 정책 근거가 목록화됐다.

## Step 2. 저장소 경계와 Save 계약을 구현한다

### AI 작업

1. 게임 로직이 특정 파일 형식·경로·서버 SDK에 직접 의존하지 않도록 Submission Service와 Memory·Local Repository 및 후속 Online Repository 계약을 정의한다. 실제 온라인 구현은 추가하지 않는다.
2. Save 모델에 Save Version, 후보의 계정 귀속 식별자, 개인 최고 기록, 제출 대기열, Tutorial 완료, Settings와 Binding을 정책에 맞게 포함한다. 인증 토큰·서비스 Secret·개인정보는 제외한다.
3. 영구 제출 ID와 현재 Runtime Run ID의 역할을 구분하고, 같은 Run에서 후보가 중복 생성되지 않도록 보존 범위와 거부 규칙을 구현한다.
4. 실제 파일 경로와 플랫폼 API는 Local Repository 구현 내부에만 둔다. Save 쓰기 중단 시 기존의 유효한 데이터가 손상되지 않도록 저장 절차를 정의한다.

### 사용자 수동 작업

없음. 계약과 코드의 책임 분리는 AI가 정적 검토로 처리한다.

### 완료 조건

- [ ] 저장소 경계가 게임 로직과 분리되고 Save 계약이 정책과 일치한다.

## Step 3. 직렬화·복구·제출 대기 규칙을 Unit Test로 구현한다

### AI 작업

1. 정상 Save의 직렬화·역직렬화, 지원하는 구버전의 Migration, 누락 필드, 손상 데이터 및 지원하지 않는 Version의 안전한 복구를 생산 코드와 Edit Mode Unit Test로 구현한다. 복구 시 보존하거나 초기화하는 항목은 계약에 명시한다.
2. 개인 최고 기록의 갱신, Record Key 분리, 같은 Run 중복 후보 거부, 영구 제출 ID 유지, Offline 대기열 추가·제거·재시도 상태를 생산 코드 호출로 Test한다.
3. 저장 중단·부분 파일·반복 로드/저장과 이전 유효 Save 보존을 격리된 임시 저장소 또는 대역으로 Test한다. 실제 사용자 저장 파일은 Test 대상에 넣지 않는다.
4. Test에 독립적인 직렬화·정렬·중복 판정 로직을 복제하지 않고, 외부 Scene·프레임·네트워크·실제 사용자 파일에 의존하지 않는지 정적으로 확인한다.

### 사용자 수동 작업

없음. 자동 판정 가능한 데이터·상태 규칙은 수동 조작으로 검증하지 않는다.

### 완료 조건

- [ ] Save 복구, Migration, 쓰기 중단, 최고 기록, 대기열과 중복 방지의 정상·거부·경계 사례가 Edit Mode Unit Test에 연결됐다.

## Step 4. Settings·Binding·Tutorial 저장 연결과 Play Mode Test를 구현한다

### AI 작업

1. 시작 시 저장 상태를 적용하고 변경 시 정책에 맞게 저장 요청하는 연결을 구현한다.
2. Settings, Input Binding 및 Tutorial 완료 상태가 기존 Runtime 책임과 충돌하지 않는지 정적으로 확인한다.
3. 실제 생산 구성의 초기화·저장·재생성 뒤 상태 복구를 검증할 Play Mode Test를 작성한다. 프로세스 종료 후 재시작 자체와 플랫폼 저장 경로는 Step 7에서 확인한다.

### 사용자 수동 작업

없음. 실행 흐름의 자동 판정은 Play Mode Test로 처리한다.

### 완료 조건

- [ ] Settings·Binding·Tutorial·개인 최고 기록의 저장·재생성 후 복구를 검증하는 생산 경로 기반 Play Mode Test가 작성됐다.

## Step 5. 문서·코드·Test 정적 대조를 수행한다

### AI 작업

1. Project 문서의 저장 범위, 관련 Feature·System 문서의 책임, 코드와 Test 이름을 대조한다.
2. 저장 계층에서 SDK 직접 참조, 비밀값 저장, Runtime 결과의 중복 소유, 정책과 다른 Record Key가 없는지 정적 검색한다.
3. 변경된 asmdef 참조와 Test의 생산 코드 참조를 정적으로 검사하고, 집중 및 회귀 Test 범위를 지정한다.

### 사용자 수동 작업

없음. 문서·코드·참조 관계는 정적 검증으로 처리한다.

### 완료 조건

- [ ] 문서·구현·Test의 책임과 참조 방향이 일치하고, 정적 검사에서 범위 외 SDK·Scene 의존이 발견되지 않았다.

## Step 6. Unity Script Compilation과 자동 Test를 확인한다

### AI 작업

1. 사용자에게 실행할 집중 Edit Mode Fixture, 관련 전체 Edit Mode 및 필요한 Play Mode 회귀 범위를 지정한다.
2. 전달받은 Test 실패 이름·메시지·Stack Trace와 예상하지 않은 Console Error/Warning을 분석해 수정 범위를 제시한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 결과와 예상하지 않은 Error/Warning 유무를 확인한다.
2. AI가 지정한 집중 Edit Mode Fixture를 Unity Test Runner에서 실행한다.
3. AI가 변경 영향에 따라 지정한 전체 Edit Mode 및 관련 Play Mode 회귀를 실행한다.
4. 각 실행의 시도·성공·실패 개수와 예상하지 않은 Error/Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 함께 전달한다.

### 완료 조건

- [ ] 지정한 Compile·Edit Mode·Play Mode Test가 모두 성공하고 예상하지 않은 Error/Warning이 없다.

## Step 7. 대상 플랫폼 Player의 저장 경로와 파일 수명을 확인한다

### AI 작업

1. 구현된 저장 위치와 예상되는 생성·갱신·보존 조건, 생산 화면에서 확인할 수 있는 데이터 항목을 사용자에게 제공한다.
2. 사용자가 보고한 결과가 Save 계약 및 자동 Test 결과와 일치하는지 분석한다.

### 사용자 수동 작업

1. 대상 플랫폼을 정해 AI에게 알리고, 해당 플랫폼 Player를 Unity Editor에서 직접 Build해 실행한다.
2. AI가 안내한 실제 저장 경로를 확인한다. 생산 화면에서 접근 가능한 Settings 변경, Binding 변경, Tutorial 완료 및 기록 생성 후 앱을 종료하고 다시 실행해 복구 여부를 확인한다.
3. 사용자 저장 위치에서 파일 생성·갱신과 앱 종료 후 보존 여부만 확인한다. 파일 내용을 열거나 직접 손상·삭제할 필요는 없다.
4. 수행한 플랫폼, Build 결과, 재실행·저장 경로 확인 결과와 예상하지 않은 Error/Warning을 전달한다. 생산 화면에서 확인할 수 없는 항목은 그 항목과 이유를 알린다.

수치·상태 전이·중복 방지·손상 복구·삭제 규칙은 자동 Test로 판정한다. 대상 플랫폼에서만 드러나는 저장 경로, 파일 생성·갱신·보존 및 실제 프로세스 재시작 후 복구를 수동 확인한다. Scene·Inspector 연결이 추가로 필요하면 AI가 정확한 대상과 설정값을 제시한 뒤 사용자가 적용한다.

### 완료 조건

- [ ] 대상 플랫폼 Player에서 저장 파일의 생성·갱신·보존과 프로세스 재시작 후 접근 가능한 상태의 복구를 확인했다.

## Step 8. Phase 2 완료 근거와 후속 범위를 정리한다

### AI 작업

1. Roadmap 007 Phase 2의 완료 조건을 정책 근거, 정적 검사, Edit Mode·Play Mode Test 및 대상 플랫폼 확인 결과에 연결한다.
2. 미결 정책, 실패한 Test 또는 미확인 대상 플랫폼 항목이 있으면 Phase 2를 완료 처리하지 않는다.
3. 완료 조건 충족 시 Roadmap 상태와 Task 기록을 갱신하고, Phase 3의 Authentication·온라인 제출 범위를 분리한다.

### 사용자 수동 작업

없음. Step 6~7의 근거가 부족한 경우에만 해당 미충족 작업을 특정해 요청한다.

### 완료 조건

- [ ] Phase 2의 저장·복구·대기열·중복 방지·자동 Test·대상 플랫폼 근거가 모두 기록됐다.

# 영향 범위

이번 작업은 `AI/90_Tasks/Prototype_7`의 Phase 2 실행 계획을 현재 Phase 1 완료 상태에 맞게 갱신한다. 후속 구현에서는 Project·System·Feature 문서, Runtime 코드, asmdef와 Edit Mode·Play Mode Test가 변경될 수 있다. 실제 온라인 서비스와 Leaderboard UI는 Phase 3~4 범위다.

# 검증 내용

- 각 Phase 2 구현 대상과 완료 조건을 Step 1~8에 대응시킨다.
- 직렬화, Migration, 손상 복구, 최고 기록, 대기열과 중복 방지는 Edit Mode Unit Test를 우선한다.
- 생산 구성의 저장·재생성 후 복구는 Play Mode Test를 우선하고, 실제 프로세스 재시작과 대상 플랫폼의 경로·파일 수명만 수동으로 확인한다.
- 사용자 수동 작업은 Unity Editor의 Compile·Test Runner와 대상 플랫폼 Player 확인으로 한정한다.

# 검증 결과

이 문서는 Phase 2 실행 계획이다. Phase 1 정책 확정과 Unity 검증은 완료됐다. Phase 2 생산 코드·Test 구현, Unity 검증 및 대상 플랫폼 확인은 아직 수행되지 않았으므로 Phase 2를 완료로 기록하지 않는다.

# 후속 작업

Phase 2 Step 1의 정적 조사부터 수행한다.

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
- `AI/02_Systems/SettingsSystem.md`
- `AI/03_Features/TimeRecord.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/Leaderboard.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_7/20260924_01_Phase1ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 수동 작업을 Compile·Test Runner·대상 플랫폼 Player 확인으로만 구분했다.
- [x] 정적 검증과 Edit Mode·Play Mode Unit Test 우선 원칙을 명시했다.
- [x] 자동 판정 가능한 수치·상태 규칙을 수동 작업에서 제외했다.
- [x] Phase 2와 Phase 3~4의 책임 경계를 분리했다.
- [x] 구현·Test·플랫폼 검증 미실행 상태를 Phase 2 완료로 기록하지 않았다.
