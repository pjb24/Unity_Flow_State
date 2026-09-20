# 작업 정보

## 작업명

Prototype 6 Phase 2 — 게임 진입 생산 연결 및 사용자 수동 작업 계획

## 작업 일자

20260920

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. 아래 Step 1~9는 미수행이며 Phase 2는 대기 상태다.

# 작업 목적

Boot, Main Menu, Mode Select와 기존 Pause/Result를 실제 게임 흐름에 연결한다. 사용자가 수행할 Scene 편집과 Unity 검증을 순서대로 안내하고, 정적으로 판정 가능한 설정과 자동 Test로 판정할 상태를 수동 관찰에 맡기지 않는다.

# 작업 대상

- Roadmap 6 Phase 2 및 Phase 1에서 확정한 Navigation 계약
- GameSystem, UIManagementSystem, UIInputSystem 및 Run 생명주기에 연결되는 System
- 기존 순수 Navigation 모델, Edit Mode Unit Test 및 생산 Scene 기반 Play Mode Test
- 사용자가 편집할 `Assets/Scenes/SampleScene.unity`의 메뉴 UI·EventSystem·직렬화 참조

Settings는 진입 출처를 기억하는 빈 기능 화면과 Back까지만 연결한다. 세부 설정·Rebinding은 Phase 3, How To Play 내용·첫 Run 자동 안내는 Phase 4, 단말기별 표시 이력 영구 저장과 실제 Leaderboard 통신은 Roadmap 7 범위다. 기존 확정 정책은 변경하지 않는다.

# 작업 전 상태

- Phase 1 완료: 사용자 보고 기준 Script Compilation 성공, Edit Mode 672개 전체 성공, 예상하지 않은 Error/Warning 없음. 이 결과는 Phase 2 변경 후의 검증을 대신하지 않는다.
- 현재 `GameSystem.Start()`는 `StartGame()`을 호출한다. `UIManagementSystem.Initialize(GameRuntimeData)`는 Runtime Data가 없으면 Error를 기록하므로 Run 없는 메뉴용 초기화 경계가 필요하다.
- 기존 UI에는 HUD, PausePanel, ResultPanel 및 Mode별 Result Content가 있고 Pause/Result에 Quit 참조가 남아 있다.
- Scene의 InputSystemUIInputModule은 비활성 상태이며 반복 설정은 이미 0.5초/0.1초다. EventSystem First Selected는 비어 있다.
- 기존 `ProductionSceneGameModeTestUtility`는 private Mode 필드 설정과 직접 StartGame 호출을 사용한다. 생산 Boot 변경의 영향을 받는 테스트 준비 경로를 함께 검토해야 한다.

# 조사 내용

AI 진입 문서, Project·Rules, General Task Template, Roadmap 6, Phase 1 인계, 관련 System/Feature 문서와 위 코드·Scene YAML·Play Mode Test 준비 경로를 읽었다. 여러 System에 걸친 작업 순서를 정리하는 문서이므로 GENERAL_TASK_TEMPLATE을 사용했다.

Roadmap의 Phase 1 상태는 완료지만 하단 다음 작업은 아직 Phase 1을 가리킨다. Phase 2 착수 시 Step 1에서 진행 상태 문구를 정리한다. ResultMenu 문서의 과거 Phase 5 Gamepad 제외 문구는 이번 Roadmap 6 Phase 2의 Keyboard/Gamepad 필수 검증을 면제하지 않는다.

# 작업 내용

각 Step의 완료 조건은 실제 수행 후 체크한다. AI는 코드·Test 작성과 정적 검사를 수행한다. 사용자는 Scene 편집, Unity Script Compilation 및 Test Runner 실행, 화면 확인을 수행한다. AI는 Unity Editor·Build·Test Runner를 실행하거나 Scene/Prefab을 수정하지 않는다.

## Step 1. 생산 연결 경계와 영향 범위를 정적으로 확정한다

### AI 작업

- 기존 사용자 변경을 보존하면서 GameSystem 시작·종료·Retry·실패 정리 경로, UI 초기화와 입력 소비, Stage/Player/Timer/Camera 초기화 순서를 추적한다.
- Boot 메뉴 표시에는 Run Data가 필요하지 않도록 초기화 책임을 구분한다. 메뉴에서 Player 물리·자동 이동·Stage·Timer가 진행되지 않도록 기존 중단 API를 재사용할 방법을 정한다.
- UIInputSystem과 EventSystem이 같은 Navigate/Submit/Click을 중복 실행하지 않도록 단일 처리 경로를 확정한다. Unity 기본 반복을 우선 사용하고 모델 반복과 이중 적용하지 않는다.
- Input Actions, asmdef, Scene YAML·GUID·fileID와 기존 Test 본문을 검사한다. 동일 내용을 사용자에게 Inspector로 재확인하도록 요청하지 않는다.
- Phase 1 계약과 생산 연결 차이 및 관련 문서의 오래된 설명을 정리하고 Roadmap 다음 작업을 Phase 2로 갱신한다.

### 사용자 수동 작업

없음. 확정 정책으로 판단할 수 없는 제품 선택이 실제로 발견되면 해당 선택만 별도로 제시한다.

### 완료 조건

- [ ] 상태 소유자·입력 처리 경로·Run 없는 초기화 경계와 회귀 영향 목록이 확정됐다.

## Step 2. 생산 연결과 Unit/통합 Test를 구현한다

### AI 작업

- 기존 `GameNavigationState`를 재사용하여 Boot 완료 후 Main Menu, Mode Submit 후 Run 생성, 초기화 실패 시 부분 Run 정리와 메뉴 복귀를 연결한다.
- Pause는 Resume → Retry → Settings → Main Menu, Result는 Retry → Main Menu로 연결한다. Pause Main Menu 확인은 Main Menu → Cancel, 기본 Main Menu다. Quit은 Main Menu에만 연결한다.
- Main Menu와 Pause의 Settings 진입·Back 복귀, 미구현 기능 화면, 선택 복원, clamp, transient 입력 소비, Playing에서의 Pause Cancel을 연결한다. 선택은 Application Runtime에서만 기억한다.
- 순수 계약은 기존 Edit Mode Unit Test를 재사용·보강한다. 생산 전이를 테스트 내부에 복제하지 않는다.
- 아래 검증 표의 Play Mode Test를 작성하고 기존 자동 시작 전제와 Quit 전제를 새 계약에 맞게 수정한다. Boot 검증을 테스트 전용 자동 시작으로 우회하지 않는다.
- 새 Scene 참조가 필요한 코드를 작성하되 Scene을 자동 생성·수정하는 Editor/Runtime 스크립트로 사용자 Scene 작업을 대신하지 않는다.
- 구현이 확정되면 이 문서에 **Scene 경로 → Component 실제 타입 → Inspector 실제 필드 → 연결 객체 → 초기 활성 상태 → 이벤트 등록 주체** 표를 추가한다. 현재 없는 필드명이나 메서드를 사용자에게 미리 입력하도록 요구하지 않는다.

### 자동 검증 설계

| 대상 | 검증 방법과 핵심 assertion |
|---|---|
| 화면 선택·허용/거부 전이 | Edit Mode: 기본 선택, clamp 양 끝, Back/Cancel 목적지, 앱 실행 범위 선택 기억, 잘못된 요청의 상태 불변 |
| Boot → Main Menu | Play Mode: 실제 Scene을 로드하고 여러 프레임/물리 Tick 동안 Run 부재, Player·Stage·Timer 미진행, UI 활성·Player 입력 차단 |
| Stage/Infinite Submit | Play Mode 매개변수 사례: 실제 생산 메뉴 요청으로 정확한 Mode의 Run 하나 생성, HUD·Action Map 전환 |
| Pause·Settings·Resume | Play Mode: 같은 Run과 위치·속도 보존, Timer/거리/Score 정지, Settings Back은 Pause Settings 선택으로 복귀 |
| Retry·Main Menu·실패 | Play Mode: Retry는 같은 Mode의 새 Run, 이전 자원·Result·입력 잔류 제거, Pause 확인 취소는 Run 보존, 확정은 Result 생성 없이 정리, 초기화 실패 후 재시도 가능 |
| 실제 UI 입력 연결 | Play Mode: Keyboard/Gamepad Navigate·Submit·Cancel, Mouse Point·Click/Back, 배경 Click, 기본 Focus 및 화면 전환 뒤 선택 복원 |
| 중복·경계 입력 | Unit 및 Play Mode: 같은 프레임 Submit+Click, 누른 채 화면 전환, 빠른 Pause/Resume, 종료와 Pause 경합에서 실행 횟수·상태 검증 |
| Scene 구성 | 정적 YAML/GUID 검사 및 구성 Test: 필수 참조, Button 순서·Navigation, Input Actions 매핑, 화면 차단·표시 조합 |
| Quit | 종료 요청 경계 Test로 Main Menu에서만 한 번 요청됨을 판정. Test Runner 프로세스를 실제 종료하지 않음 |

### 사용자 수동 작업

없음. Scene 준비가 끝나기 전 생산 Scene 통합 Test를 실행하도록 요구하지 않는다.

### 완료 조건

- [ ] 생산 연결과 Test가 작성됐고 실제 Inspector 연결표가 준비됐다.

## Step 3. 코드 정적 검사 후 Unity 컴파일과 Edit Mode Test를 확인한다

### AI 작업

- API·namespace·asmdef 의존성, 직렬화 필드 변경, 이벤트 등록/해제, null 처리, 중복 입력 경로와 문서 계약을 검사한다.
- `git diff --check`와 변경 범위를 검사하고 실행할 실제 Test 클래스 목록을 제공한다.

### 사용자 수동 작업

1. Unity Editor가 Script Compilation을 완료할 때까지 기다린다. 아직 Play를 누르지 않는다.
2. Console의 Compile Error 및 예상하지 않은 Warning을 확인한다. 발생하면 메시지·파일·행을 전달한다.
3. Test Runner의 Edit Mode에서 AI가 지정한 관련 Test를 실행하고 통과 후 전체 Edit Mode를 실행한다.
4. 실행 수, Passed/Failed 및 예상하지 않은 Error/Warning 유무를 전달한다. 실패 시 이름·메시지·Stack Trace를 전달한다.

### 완료 조건

- [ ] 컴파일과 관련/전체 Edit Mode가 통과했고 예상하지 않은 Error/Warning이 없다.

## Step 4. Scene 메뉴 UI를 제작·배치한다

선행 조건: Step 3 통과 및 Step 2의 실제 연결표 제공. 아래 이름은 신규 객체의 권장 이름이며 기존 객체를 대체하거나 재배치할 때는 확정 연결표를 따른다.

### 사용자 수동 작업

1. Play Mode를 종료하고 Project 창에서 `Assets/Scenes/SampleScene.unity`를 연다.
2. AI 연결표가 지정한 기존 UI Canvas 아래에 Panel과 `Button - TextMeshPro`를 만든다. 기존 HUD·Result 내용·Momentum HUD 참조는 보존한다.
3. 아래 표대로 제목과 Button을 위에서 아래로 배치한다. 기존 PausePanel/ResultPanel은 메뉴 버튼을 수정하고 기존 결과 Text는 유지한다.

| 화면/권장 Root | 항목 순서 | 기본 선택 |
|---|---|---|
| MainMenuPanel | Play, How To Play, Leaderboard, Settings, Quit | Play |
| ModeSelectPanel | Stage, Infinite, Back | 최초 Stage; 이후 Runtime 선택 복원 |
| 기존 PausePanel | Resume, Retry, Settings, Main Menu | Resume |
| PauseMainMenuConfirmPanel | Main Menu, Cancel | Main Menu |
| 기존 ResultPanel | Retry, Main Menu | Retry |
| LeaderboardPanel | 미구현 안내, Back | Back |
| HowToPlayPanel | 안내 준비 중 표시, Back | Back |
| SettingsPanel | 설정 준비 중 표시, Back | Back |

4. 각 화면의 RectTransform을 지정된 Canvas 범위 안에 맞추고 버튼 간격·텍스트 크기·Selected/Highlighted 색상을 조정한다. 선택된 버튼을 육안으로 구분할 수 있게 한다.
5. Pause/Result/확인 화면은 해당 Mode HUD보다 앞에 표시되도록 연결표에 지정한 Canvas 정렬 또는 Hierarchy 순서를 적용한다. 숨긴 화면이 Click을 가로채지 않도록 초기 활성 상태와 Raycast 설정도 연결표대로 적용한다.
6. Scene을 저장하고 저장 완료를 AI에게 전달한다. 이 단계에서는 Play로 동작을 판정하지 않는다.

### AI 작업 및 완료 조건

저장된 Scene을 읽기 전용으로 검사하여 객체·Component·Root·버튼 누락만 구체적으로 안내한다.

- [ ] 필요한 화면과 버튼이 저장됐고 Scene 구조 정적 검사가 통과했다.

## Step 5. Navigation·EventSystem·System 참조를 연결한다

### 사용자 수동 작업

1. 각 메뉴 Button의 Navigation을 `Explicit`으로 설정한다. Select On Up/Down에는 같은 화면의 이전/다음 Button을 연결한다. 첫 항목 Up과 마지막 항목 Down, 세로 목록 Left/Right는 None으로 두어 다른 화면이나 반대 끝으로 이동하지 않게 한다. Back만 있는 화면은 네 방향 모두 None으로 둔다.
2. 기존 EventSystem을 선택하고 Step 2에서 확정한 입력 연결표를 적용한다. 표준 InputSystemUIInputModule을 사용하는 경우 해당 Component를 활성화하고 기존 UI Action Asset의 Navigate/Submit/Cancel/Point/Click을 각각 Move/Submit/Cancel/Point/Left Click에 연결한다. 반복 지연 0.5초·간격 0.1초를 사용한다. 정확한 Asset 경로·Action 참조는 AI가 정적으로 찾아 연결표로 제공한다.
3. First Selected는 연결표에 따라 Main Menu의 Play를 지정한다. 이후 화면별 선택 및 복귀 선택은 생산 코드가 갱신한다. 배경 Click이 선택을 지우지 않도록 Deselect On Background Click 설정을 연결표대로 적용한다.
4. UIManagementSystem과 신규 UI Component의 Root·Button·Text 필드에 Step 4에서 만든 객체를 드래그한다. Pause/Result의 기존 Quit 연결은 Main Menu 동작으로 변경한다. 이름/문구만 바꾸고 Quit 호출을 남기지 않는다.
5. Button On Click은 Step 2의 등록 주체 표를 따른다. 코드가 등록하는 버튼에는 Inspector 이벤트를 중복 추가하지 않는다. Inspector 연결 방식이면 표에 적힌 실제 Component·메서드·인자를 지정한다.
6. Scene을 저장하고 완료를 전달한다. 참조의 정확성·클릭 실행 횟수는 다음 단계의 정적 검사와 Test로 판정한다.

### 완료 조건

- [ ] 연결표의 모든 참조·Navigation·이벤트 연결을 Scene에 저장했다.

## Step 6. Scene·입력 연결을 정적으로 검사한다

### AI 작업

- Scene YAML의 fileID/GUID를 실제 Script·Input Actions·객체로 해석하여 누락/잘못된 타입/중복 EventSystem·입력 처리·On Click 연결을 검사한다.
- 같은 화면 내 Explicit Navigation 연결, clamp 경계, Back 접근, 초기 Root 활성 상태, Canvas 정렬·Raycast 및 기존 HUD/Result 참조 보존을 검사한다.
- enum 직렬화 값과 기존 Quit 참조의 전환, UI Action Asset의 Keyboard/Gamepad/Mouse Binding 및 반복 설정을 대조한다.
- 수정 필요 시 사용자에게 정확한 객체 경로·Inspector 필드·현재 값·목표 값만 제공한다. 사용자 저장 후 변경 부분을 다시 정적 검사한다.

### 사용자 수동 작업

AI가 지적한 Scene 항목만 Inspector에서 수정하고 저장한다. 정적으로 통과한 항목을 다시 육안으로 확인할 필요는 없다.

### 완료 조건

- [ ] Scene·입력 연결과 변경 범위 정적 검사가 통과했다.

## Step 7. Unity 통합 Test와 전체 회귀를 실행한다

### AI 작업

실제 작성된 신규 Test 이름과 기존 영향 범위를 대조하여 실행 목록을 제공한다. 우선 검토 대상은 GameLifecycleIntegrationTests, GamePauseOrchestrationTests, UIInputSystemTests, PauseMenuIntegrationTests, ResultMenuIntegrationTests, PausePanelSceneConfigurationTests, ModeUISceneConfigurationTests, ModeResultDisplayIntegrationTests 및 Stage/Infinite Mode 통합 Test다.

### 사용자 수동 작업

1. Script Compilation 성공과 예상하지 않은 Error/Warning 부재를 확인한다.
2. Step 3 이후 코드가 변경됐다면 관련/전체 Edit Mode Test를 다시 실행한다. Scene 설정을 읽는 Edit Mode Test가 있다면 해당 Test도 실행한다.
3. Test Runner의 Play Mode에서 AI가 지정한 신규·관련 Test를 실행한다.
4. 관련 Test 통과 후 전체 Play Mode Test를 실행한다. Boot 변경은 기존 생산 Scene을 사용하는 공통 시작 경로에 영향을 주므로 전체 회귀 대상으로 한다.
5. Edit/Play Mode별 실행 수·Passed/Failed·예상하지 않은 Error/Warning 유무를 전달한다. 실패는 이름·메시지·Stack Trace를 함께 전달한다.

### 실패 처리

AI는 원인과 영향 범위를 조사하여 코드/Test 수정 또는 정확한 Scene 수정 절차를 제공한다. 예상값 완화나 무관한 LogAssert.Expect로 실패를 숨기지 않는다. 수정 후 관련 Test와 영향받는 전체 회귀를 재실행한다.

### 완료 조건

- [ ] 최종 변경 기준 컴파일, Edit Mode 및 Play Mode 회귀가 통과했고 예상하지 않은 Error/Warning이 없다.

## Step 8. 실제 입력 장치와 화면 표현을 확인한다

### 사용자 수동 작업

1. Gamepad를 연결하고 Game View에서 Play를 시작한다. Main Menu 버튼의 글자와 선택 표시가 구분되는지 본다.
2. Keyboard 방향키/WASD와 Submit으로 Main Menu → Mode Select → Back을 천천히 이동한다. Gamepad D-pad/Stick·Submit·Cancel로 같은 경로를 이동하며 Focus 가시성과 조작감을 확인한다.
3. Mouse로 각 기능 화면을 열고 화면의 Back 버튼을 클릭한다. Keyboard로 Back 버튼을 선택·Submit했을 때도 화면에서 선택과 복귀를 이해할 수 있는지 확인한다.
4. Stage와 Infinite를 각각 시작하여 Pause → Settings → Back, Pause → Main Menu 확인 → Cancel 및 Main Menu 확정, Result 메뉴의 버튼·HUD가 읽히고 겹치지 않는지 확인한다. Result 도달에 장시간 플레이가 필요하면 AI가 기존 생산 종료 경로를 사용하는 재현 절차를 먼저 제공한다.
5. Keyboard → Mouse → Gamepad 순서로 천천히 장치를 바꾸며 선택 표시가 보이지 않거나 조작이 끊기는지 확인한다. 수행 중인 Game View 해상도·장치와 문제 화면·재현 순서를 기록한다.
6. Play Mode를 종료하고 결과를 전달한다. 화면 변경이 필요하면 Edit Mode에서 수정·저장하고 AI의 정적 재검사를 받는다.

Run 개수·물리 정지·Timer 값·이벤트 횟수·중복 입력·짧은 타이밍 경합은 Step 7 Test로 판정한다. IDE 디버거 사용, 빠른 연타나 정밀 타이밍 조작을 요구하지 않는다. Gamepad 확인이 불가능하면 해당 항목을 미확인으로 남기며 Phase 2를 완료 처리하지 않는다.

### 완료 조건

- [ ] 실제 Keyboard/Gamepad/Mouse의 Focus·가독성·조작 확인 결과가 기록됐다.

## Step 9. Phase 2 결과를 기록하고 다음 Phase로 인계한다

### AI 작업

- Roadmap Phase 2 완료 조건 각각에 정적 검사·Test·화면 확인 근거를 연결하고 최종 변경 이후 결과인지 확인한다.
- 실제 컴파일·Test 결과, 사용자 Scene 변경 범위, 화면 확인 결과를 기록한다. 미확인 또는 실패 항목이 있으면 완료 처리하지 않는다.
- 완료 근거가 충족되면 Roadmap Phase 2만 완료로 바꾸고 다음 작업을 Phase 3으로 갱신한다. Settings 세부 기능·Rebinding·How To Play 자동 안내·영구 저장을 구현 완료로 기록하지 않는다.

### 사용자 수동 작업

없음. Phase 2의 필수 완료 조건에 별도 Build는 없다. 사용자가 Build를 수행한다면 결과는 별도로 기록하고, 전체 UI 흐름의 필수 Build 검증은 Roadmap Phase 4에 유지한다.

### 완료 조건

- [ ] Phase 2 완료 근거와 Phase 3 인계가 기록됐다.

# 영향 범위

이번 계획 작성은 이 Task 문서만 추가한다. 실행 시 코드·Test·관련 계약·Roadmap 변경은 AI, Scene/Prefab 편집은 사용자가 담당한다.

# 검증 내용

Roadmap Phase 2 완료 조건과 Phase 1 인계를 Step 및 검증 표에 대응시켰다. 기존 파일 경로·초기화 전제·Scene 입력 설정을 읽기 전용으로 조사했다. 미래 Inspector 필드·신규 Test 이름은 구현 후 확정하도록 구분했다.

# 검증 결과

계획 작성 완료. Phase 2 구현·Scene 편집·컴파일·Test Runner·Build는 수행하지 않았다. Step 1~9의 완료 조건은 모두 미확인이다.

# 후속 작업

Step 1부터 순서대로 수행한다. Scene 편집은 Step 2의 실제 연결표와 Step 3 컴파일 성공 후 시작한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_006.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/03_Features/GameEntryNavigation.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/03_Features/Settings.md`
- `AI/03_Features/HowToPlay.md`
- `AI/03_Features/Leaderboard.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_6/20260918_01_Phase1ManualSteps.md`
