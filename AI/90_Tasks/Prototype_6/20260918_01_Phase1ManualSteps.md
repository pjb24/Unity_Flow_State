# 작업 정보

## 작업명

Prototype 6 Phase 1 게임 진입·UI Navigation 계약 및 검증 계획

## 작업 일자

20260918

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. 아래 Step은 아직 수행하지 않았다.

---

# 작업 목적

Roadmap 6 Phase 1의 게임 진입 상태, Run 생명주기와 UI Navigation 계약을 확정한다. 정적 검사와 Edit Mode Unit Test로 판정할 수 있는 내용은 자동 검증하고, 사용자 수동 작업은 제품 정책 선택과 Unity Editor에서의 컴파일·Test Runner 실행에 한정한다.

# 작업 대상

- Boot·Main Menu·Mode Select·Playing·Pause·Result 흐름
- Run Runtime Data 생성·보존·종료·제거와 Retry·Main Menu 복귀
- 화면별 기본 선택, Navigate·Submit·Cancel 및 입력 소비 규칙
- Stage·Infinite·Leaderboard·How To Play·Settings·Quit 진입과 복귀
- Keyboard·Gamepad 입력 계약과 기존 Mouse 입력 호환
- 일반 UI·개발자 UI 표시 경계

Phase 1은 계약과 순수 상태 모델 검증 단계다. 실제 Boot 흐름 교체, Scene UI 제작·배치·직렬화 연결, EventSystem 변경, 물리·Camera 제어와 Action Map 생산 연결은 Phase 2 이후에 수행한다. Settings 세부 기능·Rebinding은 Phase 3, 안내 화면은 Phase 4, 영구 저장·실제 Leaderboard 통신은 Roadmap 7 범위다.

# 작업 전 상태

- Roadmap 5는 사용자 승인 대체 검증 기준을 포함해 완료로 기록되어 있다. 최종 보고 수치는 Edit Mode `649/649`, Play Mode `224/224`이며 Build와 핵심 플레이 확인이 통과했다. 이는 이전 기준선이며 이번 Phase의 테스트 성공 수로 재사용하지 않는다.
- Roadmap 6의 모든 Phase는 대기 상태다.
- `GameSystem.Start()`는 현재 `StartGame()`을 호출한다.
- `E_GameState`는 None·Initializing·Ready·Playing·Paused·Ending·Ended, `E_UIState`는 None·StageHud·Pause·Result를 정의한다.
- 기존 GamePause 문서는 Resume·Retry·Quit, ResultMenu 문서는 Retry·Quit을 정의한다. Main Menu 복귀와 새 화면 Navigation은 새로 계약을 확정해야 한다.
- ResultMenu의 기존 검증 범위는 Keyboard·Mouse이며 Roadmap 6은 Gamepad까지 요구한다.

# 조사 내용

계획 작성 시 Roadmap 6, GameSystem·UIManagementSystem·UIInputSystem, StagePlay·GamePause·ResultMenu 문서와 현재 진입 코드·상태 enum·테스트 파일 목록을 확인했다. 실제 전체 호출 경로와 테스트 본문 감사는 Step 1에서 수행한다.

# 작업 내용

아래 순서로 수행한다. 각 Step 완료 여부는 해당 산출물과 실제 검증 근거로 판정하며, 이 계획 작성만으로 Phase 1을 시작하거나 완료 처리하지 않는다.

## Step 1. 기존 생산 경로와 기준선을 정적으로 조사한다

### AI 작업

- GameSystem의 Start·StartGame·PauseGame·ResumeGame·RetryGame·EndGame, RuntimeDataSystem·StageSystem·ResultSystem 호출 순서를 읽고 실제 Run 생성·제거 시점을 기록한다.
- 게임 상태와 화면 상태의 소유자를 구분하고 기존 enum·상태 모델·Action Map 전환·transient 입력 소비 경로를 조사한다.
- `GameStateTests`, `GameRuntimeDataTests`, `PauseMenuStateTests`, `UIVisibilityStateTests`, `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `UIInputSystemTests`, Pause/Result 통합 Test 본문을 대조한다.
- Input Actions, asmdef, Scene YAML·GUID·직렬화 참조를 읽기 전용으로 검사한다. 사용자에게 Inspector로 같은 항목을 재확인하도록 요구하지 않는다.
- `git status`와 변경 파일을 확인해 기존 사용자 변경을 구분한다. 생산 연결에 영향을 주는 문서 불일치와 재사용 가능한 구조를 기록한다.

### 사용자 수동 작업

없음. 파일로 확인할 수 없는 제품 결정은 다음 Step의 선택 항목으로 모은다.

### 완료 조건

- [ ] 현행 자동 시작·Run 생명주기·UI 입력 경로의 근거가 기록됐다.
- [ ] Phase 1의 계약/모델 범위와 Phase 2의 생산 연결 범위가 구분됐다.
- [ ] 기존 테스트 재사용 범위와 신규 계약 검증 공백이 식별됐다.

## Step 2. 미확정 제품 정책과 상태 전이 계약을 확정한다

### AI 작업

- Roadmap의 확정 요구사항을 우선 적용한다. Boot 이후 Main Menu 진입, Mode 선택과 시작 요청 이후에만 Run 생성, 같은 Mode Retry, Main Menu 복귀 시 이전 Run 정리를 표에 반영한다.
- 상태 전이 표에 현재 상태·요청·허용 조건·다음 상태·Mode·Run 생성/보존/제거·실패 결과를 적는다.
- Boot/화면 상태를 기존 게임 상태와 어떻게 구분할지 책임을 정한다. 화면마다 게임 상태 enum을 무조건 추가하지 않는다.
- Main Menu 복귀를 정상 Result 생성과 구분하고, Pause와 종료가 겹칠 때의 우선순위 및 초기화 실패 시 복귀 상태를 정의한다.
- 명시되지 않은 정책만 선택안과 권고 이유를 제시한다. 예: Mode 항목 Submit으로 즉시 시작할지 별도 Start를 둘지, Pause/Result에 Quit을 유지할지, 선택 복귀 시 이전 항목을 기억할지.
- Quit 외에 불필요한 확인 단계를 추가하지 않는다. Quit 확인창도 필수라고 추정하지 않는다.

### 사용자 수동 작업

1. AI가 제공한 전이 표와 미확정 선택 항목을 문서로 검토한다.
2. 제품 동작 선택이 필요한 항목에만 원하는 규칙을 답한다. 이미 확정된 요구사항을 재승인할 필요는 없다.

Unity Editor 조작은 없다. 미확정 선택이 없으면 사용자 수동 작업도 없다.

### 완료 조건

- [ ] 실행·Mode 선택·시작·Pause·Resume·Result·Retry·Main Menu 복귀 흐름이 확정됐다.
- [ ] 중복/잘못된 요청과 초기화 실패에서 Run 상태가 누출되지 않는 계약이 있다.
- [ ] 확정 규칙과 아직 결정하지 않은 항목이 혼재하지 않는다.

## Step 3. 화면별 Navigation·입력·표시 계약을 확정한다

### AI 작업

- 화면 계약 표를 작성한다. 열은 화면, 진입 위치, 항목 순서, 기본 선택, Navigate 경계 처리, Submit 결과, Cancel 목적지, 복귀 선택, Run 유지 여부, Player/UI 입력 허용 여부로 구성한다.
- Main Menu의 Play·Leaderboard·How To Play·Settings·Quit, Mode Select의 Stage·Infinite, Pause·Result와 각 기능 화면을 빠짐없이 포함한다.
- Main Menu 자체의 Cancel, Mode Select 취소, Pause에서 Settings 진입 후 복귀, Result에서 Main Menu 복귀를 명시한다. Phase 3/4 화면은 목적지와 복귀 계약만 정한다.
- 미구현 Leaderboard의 항목 활성 여부, 선택 가능 여부, 안내 방식과 돌아가기 규칙을 확정한다. 실제 조회·제출·Offline 처리 구현은 제외한다.
- Keyboard/Gamepad의 Navigate·Submit·Cancel 의미, 유지 입력 반복 기준, 중립 복귀, 상태 전환 프레임 입력 소비, 동시 Submit/Click 및 장치 전환 시 중복 실행 방지 계약을 정의한다. 수치가 필요하면 확정값 또는 기존 재사용 근거를 기록한다.
- 기존 Mouse 경로를 유지할 범위를 명시하고, 입력 장치별 실제 Binding은 Input Actions와 대조한다. Rebinding 구현을 앞당기지 않는다.
- 일반 빌드에서 개발자 Difficulty 정보가 숨겨지는 표시 규칙을 포함한다.
- 규칙은 해당 Feature 문서, 책임은 System 문서에 정리한다. Phase 1 확정 계약과 아직 미연결인 생산 동작을 구분하여 구현 완료로 오해하지 않게 한다. 새 문서가 필요하면 해당 템플릿을 적용한다.

### 사용자 수동 작업

AI가 정적으로 결정할 수 없는 기본 선택·Cancel·미구현 기능 안내 방식 등이 남은 경우에만 제시된 정책안을 검토하고 선택한다. Gamepad 연결, Scene 편집이나 화면 가독성 평가는 이 Step에서 요구하지 않는다.

### 완료 조건

- [ ] 모든 화면에 기본 선택·Cancel 목적지·복귀 경로가 있다.
- [ ] Keyboard/Gamepad 의미와 중복 입력 소비 규칙이 확정됐다.
- [ ] 일반/개발자 UI 경계와 Leaderboard 미구현 표시 정책이 있다.

## Step 4. 순수 상태 모델과 Edit Mode Unit Test를 작성한다

### AI 작업

- Step 2–3의 확정 계약을 기존 구조에 맞는 순수 상태 모델/API로 표현한다. 테스트 안에 별도 Navigation 구현이나 생산 계산식을 복제하지 않는다.
- 모델은 Scene·프레임·실제 입력 장치 없이 상태와 실행 요청을 판정하게 한다. Runtime Data 실제 생성/제거 및 System 호출의 생산 연결은 Phase 2에서 구현·Play Mode 검증한다.
- 계약 표의 허용/거부 조합을 매개변수 Test로 다룬다. 잘못된 요청은 상태·선택·Run 식별 정보를 변경하지 않는지도 검사한다.
- 아래 Unit Test 범위를 충족하고 기존 테스트와 겹치는 부분은 재사용한다.

| 영역 | 자동 검증 내용 |
|---|---|
| Boot·Main Menu | 시작 요청 전 Run 생성 명령 없음, 기본 선택, 중복 Boot/진입 요청 |
| Mode Select | Stage/Infinite 선택, 미선택/유효하지 않은 Mode 거부, Cancel 후 Run 없음 |
| Run 생명주기 | 시작 1회, Resume은 같은 Run, Retry는 같은 Mode의 새 Run 요청, 복귀 시 정리 요청 |
| 실패·충돌 | 초기화 실패의 복귀 상태, 중복 시작/종료/Retry, Pause와 종료 우선순위 |
| Navigation | 모든 화면 기본 선택, 목록 양 끝·선택 불가 항목, Submit/Cancel, 이전 화면 복귀 |
| 입력 경계 | 같은 입력 중복 소비, 전환 직후 잔류 Submit/Cancel, 반복 간격 경계, 비활성 Player 입력 정책 |
| 기능 화면 | 미구현 Leaderboard 요청 결과, Settings/How To Play 진입·복귀 중 Run 정책 |
| 표시 정책 | Mode별 HUD·Pause·Result, 일반/개발 빌드의 개발자 UI 표시 정책 |

- 생산 UI가 아직 없으므로 실제 Button 선택·장치 입력·물리 정지는 이 Test로 검증됐다고 기록하지 않는다.

### 사용자 수동 작업

없음. AI가 코드와 Test를 작성한다. Unity Test Runner는 아직 실행하지 않는다.

### 완료 조건

- [ ] 새 계약의 허용·거부·기본 선택·초기화·중복 요청을 Edit Mode Test로 판정할 수 있다.
- [ ] Test가 Phase 2에서 사용할 실제 상태 모델을 호출한다.
- [ ] Scene·Prefab·생산 자동 시작 흐름을 변경하지 않았다.

## Step 5. 컴파일 요청 전 최종 정적 검사를 수행한다

### AI 작업

- 계약 표의 각 규칙을 상태 모델·Test와 연결해 미검증 항목을 확인한다.
- 모델 API, namespace, asmdef 참조, Editor/Test API의 Runtime 유입, 기존 enum의 직렬화 값 영향과 기존 호출부 호환성을 검사한다.
- 문서 간 중복·충돌, 미확정 정책, 테스트 전용 가짜 구현 여부를 검사한다.
- `git diff --check`와 변경 범위를 확인한다. 관련 없는 Scene/Prefab/Package/ProjectSettings 변경은 사용자 변경과 구분하고 임의로 되돌리지 않는다.
- 실제 추가·변경된 테스트 클래스와 실행할 Edit Mode 목록을 제공한다. 신규 클래스 이름과 실행 개수는 구현 후 확인하고 기록한다.

### 사용자 수동 작업

없음. 정적 오류가 있으면 AI가 먼저 수정한다.

### 완료 조건

- [ ] 문서·상태 모델·Test가 확정 계약과 일치한다.
- [ ] 컴파일 전에 정적으로 해결 가능한 문제가 정리됐다.
- [ ] 사용자에게 전달할 실제 Test 목록과 회귀 범위가 준비됐다.

## Step 6. Unity 컴파일과 Edit Mode Test를 실행한다

### AI 작업

- 사용자 결과를 분석하고 실패 시 생산 모델·Test·계약 중 원인을 확인한다.
- 수정이 필요하면 영향 범위 내에서 처리하고 재실행 목록을 제공한다. 실패를 숨기기 위해 assertion을 제거하거나 예상하지 않은 로그를 허용하지 않는다.
- 순수 신규 모델에 한정되면 Phase 1 필수 실행은 관련 및 전체 Edit Mode Test다. 기존 생산 흐름이나 공통 API에 영향이 생긴 경우 관련 Play Mode 회귀 목록도 별도로 제공하고 사용자 실행 결과를 받는다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료를 기다린다.
2. Console의 Compile Error와 예상하지 않은 Warning 유무를 확인한다. 문제가 있으면 메시지와 발생 위치를 전달한다.
3. Test Runner의 Edit Mode 탭에서 Step 5가 지정한 관련 테스트를 실행한다.
4. 관련 테스트 통과 후 검색 필터를 해제하고 전체 Edit Mode Test를 실행한다.
5. 전체 실행 수, Passed/Failed 수, 예상하지 않은 Error/Warning 유무를 전달한다. 실패가 있으면 Test 이름·메시지·Stack Trace를 함께 전달한다.
6. AI가 생산 영향 때문에 추가로 지정한 Play Mode 회귀가 있을 때만 해당 테스트를 실행하고 같은 형식으로 결과를 전달한다.

Build, Scene 편집, 화면 조작 검증은 필요 없다. 실패나 예상하지 않은 Error/Warning이 있으면 다음 완료 단계로 넘어가지 않는다.

### 완료 조건

- [ ] Unity Script Compilation 성공과 예상하지 않은 Error/Warning 부재가 확인됐다.
- [ ] 관련 및 전체 Edit Mode Test가 통과했다.
- [ ] 영향 때문에 추가로 지정한 회귀가 있다면 그 결과도 통과했다.

## Step 7. Phase 1 결과를 기록하고 Phase 2로 인계한다

### AI 작업

- 확정된 상태 전이·화면 Navigation·입력 표와 모델·Test를 최종 대조한다.
- 실제 Compile/Test 결과와 변경 범위를 Task 결과에 기록한다. 테스트 미실행 또는 미확정 정책이 있으면 Phase 1을 완료 처리하지 않는다.
- Roadmap 6 Phase 1만 완료로 갱신하고 다음 작업을 Phase 2 생산 연결로 지정한다.
- Phase 2에서 필요한 Scene UI 제작, GameSystem Boot 연결, Action Map·Runtime Data 연결, 실제 Keyboard/Gamepad 및 Focus 확인 항목을 인계한다. Scene 편집은 이후 사용자가 수행할 수 있도록 별도 절차로 작성한다.
- Settings 기능·Rebinding·안내·영구 저장이 Phase 1에 구현됐다고 기록하지 않는다.

### 사용자 수동 작업

없음. 이미 전달된 정책과 테스트 결과를 AI가 문서화한다.

### 완료 조건

- [ ] Roadmap Phase 1의 모든 계약 완료 조건에 근거가 있다.
- [ ] 실제 Test 결과와 미구현 생산 연결 범위가 명확히 기록됐다.
- [ ] Phase 2 인계 항목이 정리됐다.

# 사용자 수동 작업 요약

1. Step 2–3에서 문서와 코드로 결정할 수 없는 제품 정책이 제시될 경우에만 선택한다.
2. Step 6에서 Unity 컴파일과 관련/전체 Edit Mode Test를 실행하고 실제 결과를 전달한다.
3. 기존 생산 흐름 영향으로 추가 회귀가 지정된 경우에만 해당 Play Mode Test를 실행한다.

상태값·전이 횟수·중복 입력·기본 선택은 수동 플레이로 판정하지 않는다. Phase 1에는 Scene/Prefab 편집, Build, 장시간 플레이, 실제 Gamepad 탐색·화면 배치 확인을 요구하지 않는다. AI는 Unity Editor·Build·Test Runner를 실행하지 않는다.

# 영향 범위

이번 계획 작성은 Tasks 문서만 변경한다. 향후 Step 실행 범위는 관련 Feature/System 계약 문서, 순수 상태 모델·Edit Mode Test, Roadmap 진행 상태다. 실제 생산 Scene 연결은 Phase 2 범위다.

# 검증 내용

계획을 Roadmap 6 Phase 1의 구현 대상·완료 조건·검증 책임 및 기존 문서·코드와 대조했다. 문서 작성에 GENERAL_TASK_TEMPLATE을 적용했다. 실행 Step별 결과는 실제 수행 후 기록한다.

# 검증 결과

계획 작성 완료. Phase 1 실행·컴파일·테스트는 미수행이며 Step 1–7의 완료 조건은 모두 미확인 상태다.

# 후속 작업

Step 1부터 수행한다. Phase 1 완료 후 Phase 2 생산 연결과 사용자 Scene 작업 계획을 작성한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_006.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260917_03_Phase4ManualSteps.md`
- `AI/90_Tasks/Prototype_5/20260917_04_Phase4VerificationResult.md`

# 작성 완료 기준

- AI 작업과 사용자 수동 작업, 완료 조건을 Step별로 구분했다.
- 정적 검사와 순수 상태 Unit Test를 수동 플레이보다 우선했다.
- 미확정 제품 정책과 이미 확정된 Roadmap 요구사항을 구분했다.
- Phase 1 계약 검증과 Phase 2 이후 생산 연결 검증을 구분했다.
- 수행하지 않은 Step과 테스트를 완료로 기록하지 않았다.
