# 작업 정보

## 작업명

Prototype 6 Phase 2 — 게임 진입 생산 연결 및 사용자 수동 작업 계획

## 작업 일자

20260920

## 작업 담당자

AI, 사용자

## 작업 상태

완료 (20260921). Step 1~9를 모두 수행했고 Roadmap 006 Phase 2 완료 및 Phase 3 인계를 기록했다.

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

착수 당시 Roadmap의 Phase 1 상태는 완료였지만 하단 다음 작업은 Phase 1을 가리키고 있었다. Step 1에서 다음 작업을 Phase 2로 정리했고, Step 9에서 Phase 2 완료와 Phase 3 인계를 반영했다. 사용자가 GamePad 미보유 시 검증 제외를 명시적으로 승인했으므로, Phase 2의 입력 장치 확인은 사용 가능한 Keyboard/Mouse 결과와 GamePad 미확인 사유를 기록하는 방식으로 판정했다.

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

- [x] 상태 소유자·입력 처리 경로·Run 없는 초기화 경계와 회귀 영향 목록이 확정됐다.

### 수행 결과 (20260920)

#### 상태 소유자와 생산 연결 경계

| 책임 | 기존 확인 결과 | Phase 2 연결 기준 |
|---|---|---|
| Navigation 계약·선택·Run 요청 | 순수 `GameNavigationState`가 Boot부터 Main Menu, Mode Select, Pause, Result까지의 허용 전이와 선택을 소유한다. | `GameSystem`이 이 상태 모델의 요청을 생산 System 호출로 해석한다. Navigation 규칙을 `GameSystem`이나 UI Component에 중복 구현하지 않는다. |
| Run 생명주기와 실행 순서 | `GameSystem`이 Runtime Data 생성, System 초기화, Stage 시작, Pause/Resume, Result 및 정리를 직접 조정한다. 현재 `Start()`가 즉시 `StartGame()`을 호출한다. | Boot 완료는 Run을 만들지 않고 Main Menu만 표시한다. Mode Submit 또는 Retry 뒤의 Run 요청에서만 기존 `StartGame()` 흐름을 사용한다. |
| Runtime Data | `RuntimeDataSystem.CreateRuntimeData(E_GameMode)`와 `ClearRuntimeData()`가 생성·제거를 소유한다. | 메뉴에는 Runtime Data를 만들지 않는다. 초기화 실패·Pause Main Menu 확정·Result Main Menu에서는 기존 중단·정리 API를 사용해 부분 Run을 제거한다. |
| UI 표시·Focus | `UIManagementSystem.Initialize(GameRuntimeData)`는 Runtime Data가 없으면 Error를 기록하며, 현재 HUD·Pause·Result만 표시한다. | 메뉴 전용 초기화/표시 경계를 추가해 Run 없는 화면을 지원한다. 신규 화면의 Root 표시와 Button 선택은 UIManagementSystem이 적용하며, 결과/HUD 기존 참조는 보존한다. |
| UI와 Player Action Map | `PlayerInputSystem`과 `UIInputSystem`이 각 Action Map을 직접 관리하고, `GameSystem`이 활성 상태를 요청한다. | Menu·Pause·Result에서는 Player Action Map을 비활성화한다. Playing에서만 Player Action Map을 활성화하고 UI Cancel만 Pause 요청으로 해석한다. |

`GameState`는 `None → Initializing → Ready → Playing` 전이만 허용하므로, Main Menu는 기존 게임 상태를 임의로 Playing/Ready로 바꾸지 않고 Navigation 화면으로 표현한다. Run 없는 UI 초기화는 `UIVisibilityState`가 `None` 상태에서 HUD·Pause·Result를 모두 숨기는 기존 동작을 재사용하되, 메뉴 Root의 표시 책임을 별도로 둔다.

#### 입력 단일 처리 경로

- `Assets/InputSystem_Actions.inputactions`의 UI Map에는 Keyboard WASD/Arrow, Gamepad D-pad·양쪽 Stick Navigate와 Submit·Cancel·Point·Click이 이미 정의되어 있다.
- Scene의 `EventSystem`에는 `InputSystemUIInputModule`이 있으며, 기존 UI Action Asset 참조와 Move/Submit/Cancel/Point/Left Click 참조는 유효하다. 반복 값도 계약값인 Delay `0.5`, Rate `0.1`이다. 다만 Module은 비활성이고 First Selected가 비어 있다.
- Phase 2의 메뉴 Navigate·Submit·Click 및 Button 선택은 활성화한 `InputSystemUIInputModule`과 Button의 Unity UI 이벤트 경로 하나로 처리한다. `GameSystem`은 메뉴에서 `UIInputSystem`의 Navigate/Submit/Click을 다시 해석하지 않는다. 따라서 현재 Pause/Result의 수동 좌표 hit-test·선택 실행 경로는 신규 Button 이벤트와 병존하지 않게 교체한다.
- `UIInputSystem`은 Playing 중 Cancel을 Pause 요청으로 해석하는 경로와 transient 입력 소비를 유지한다. 화면 또는 Run 상태를 전환한 프레임에는 그 입력을 소비하며, EventSystem의 Button 콜백도 한 번만 `GameSystem`의 Navigation 요청으로 전달한다.

#### 초기화·정리 순서

1. Boot: UI 입력 경로와 메뉴 UI만 준비하고 Player Action Map, Runtime Data, Stage, Timer, Camera Follow, Player 이동·물리는 시작하지 않는다.
2. Mode Submit/Retry: Navigation 상태가 Run 시작을 요청한 뒤에만 `RuntimeDataSystem` 생성과 기존 Player·Collision·Stage·Movement·Infinite·Camera 초기화, Stage 시작, Timer 시작, Player 입력 활성화를 수행한다.
3. 실패/Main Menu: `StageSystem.StopStage`, Player 입력 비활성화, Camera Follow 중지, Infinite·Movement 중지, Timer 제거, Runtime Data 제거를 기존 `AbortGameStart()`/종료 경로의 책임으로 재사용한다. 실패 후에는 Main Menu Navigation으로 복귀한다.
4. Pause/Result: Pause는 기존 중단 API로 Run을 유지한다. Retry는 같은 선택 Mode의 새 Run을 시작하고, Main Menu 확정은 Result를 만들지 않고 Run을 정리한다.

#### 회귀 영향 목록

- `GameSystem`: 자동 시작, Retry, End/Abort, Pause/Result의 Quit 처리, UI State와 Action Map 전환을 Navigation 생산 연결에 맞게 변경한다.
- `UIManagementSystem`과 `UIVisibilityState`: Runtime Data가 없는 메뉴 초기화, Main Menu·Mode Select·기능 화면·Pause 확인 표시 및 Focus, 기존 HUD/Pause/Result 표시 보존을 검증한다.
- `UIInputSystem`과 `SampleScene` EventSystem: UI Action Map과 EventSystem Module의 중복 소비 방지, 기본 선택 및 배경 Click 유지 정책을 검증한다.
- `RuntimeDataSystem`, `StageSystem`, `TimerSystem`, `PlayerInputSystem`, `PlayerMovementSystem`, `PlayerControllerSystem`, `InfiniteModeSystem`, `CameraFollow`: Boot에서 Run이 없고 Mode Submit 뒤에만 초기화·진행하며, Retry/Main Menu/실패에서 정리되는지를 Play Mode로 검증한다.
- 기존 생산 Scene 자동 시작을 전제하는 `ProductionSceneGameModeTestUtility`, `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `PauseMenuIntegrationTests`, `ResultMenuIntegrationTests`, `ModeResultDisplayIntegrationTests`, Stage/Infinite Mode 통합 Test 및 SampleScene을 직접 로드하는 이동·카메라·Collectible·Goal 관련 Play Mode Test는 새 Boot 경로에 맞춰 준비/기대값을 검토한다.

`InputSystem_Actions`, asmdef, Scene YAML의 기존 UI Action Asset GUID/fileID와 GameSystem·UIManagementSystem·UIInputSystem 직렬화 참조는 읽기 전용으로 확인했다. 이 Step에서는 Scene, Input Actions, Prefab, ProjectSettings를 수정하지 않았고 Unity Editor, Test Runner, Build도 실행하지 않았다. `git diff --check`도 공백 오류 없이 통과했다.

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

- [x] 생산 연결과 Test가 작성됐고 실제 Inspector 연결표가 준비됐다.

### 수행 결과 (20260920)

- `GameSystem.Start()`는 더 이상 Run을 시작하지 않는다. Boot에서 `UIManagementSystem.InitializeMenu()`과 UI Action Map만 준비한 뒤 `GameNavigationState.CompleteBoot()`으로 Main Menu를 표시한다.
- `GameSystem.RequestNavigationSelection(E_NavigationItem)`은 코드와 Test의 타입 안전한 단일 진입점이다. Inspector용으로는 UnityEvent가 enum 인자를 표시하지 않는 제약을 피하기 위해 인자 없는 `Select*` 공개 래퍼를 제공한다. Mode Select의 Stage/Infinite, Pause/Result Retry, Pause Main Menu 확인, 기능 화면 Back 및 Main Menu Quit을 Navigation 상태 모델과 기존 Run 시작·중단 API에 연결했다.
- Mode Submit과 Retry에서만 `StartRequestedRun()`이 Runtime Data 생성, System 초기화, Stage/Timer 시작, Player 입력 활성화를 실행한다. 실패는 `AbortGameStart()`에서 부분 Run을 정리하고 Navigation 상태를 Main Menu로 복귀시킨다.
- `EndRun(bool)`으로 Result 생성 종료와 Main Menu용 정리를 구분했다. Pause Main Menu 확정은 Result를 만들지 않고 Runtime Data·Timer·Stage·입력·Camera Follow·Movement를 정리한다.
- `UIManagementSystem`에 메뉴 전용 초기화, Navigation Root 표시, 선택 Button Focus와 신규 화면·Button 직렬화 필드를 추가했다. 기존 HUD, Pause, Result 필드와 표시 경로는 유지했다.
- `HasNavigationUIConfiguration`이 모든 신규 Root/Button 참조가 연결됐을 때만 EventSystem Button 기반 경로를 사용하게 한다. 연결 전에는 기존 Pause/Result 입력 경로를 유지하므로 Scene 작업 전 직렬화 누락으로 새 입력과 기존 입력이 섞이지 않는다.
- `ProductionSceneGameModeTestUtility`는 private Mode 설정과 직접 `StartGame()` 호출을 제거하고 Main Menu Play → Mode Select 요청을 사용한다. `GameEntryBootIntegrationTests`를 추가해 실제 Scene Boot의 Run 부재, Infinite Mode Submit, Pause Settings Back 및 Pause Main Menu 확인의 Run 보존·정리를 Play Mode에서 판정하도록 작성했다.
- Unity Script Compilation, Unity Test Runner, Build, Scene/Prefab 수정은 수행하지 않았다.

### Inspector 연결표

| Scene 경로 | Component 실제 타입 | Inspector 실제 필드 | 연결 객체 | 초기 활성 상태 | 이벤트 등록 주체 |
|---|---|---|---|---|---|
| `GameSystem` | `FlowState.Runtime.Systems.GameSystem` | 없음 | 기존 System 참조 유지 | 활성 | 각 메뉴 Button의 On Click이 해당 인자 없는 `Select*` 공개 메서드를 직접 호출 |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | `_mainMenuPanel`, `_modeSelectPanel`, `_pauseMainMenuConfirmPanel`, `_leaderboardPanel`, `_howToPlayPanel`, `_settingsPanel` | Step 4에서 만드는 각 화면 Root | Main Menu만 Boot 뒤 활성, 나머지는 비활성 | `GameSystem`이 Navigation 상태 변화 뒤 `SetNavigationScreen`을 직접 호출 |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | `_mainMenuPlayButton`, `_mainMenuHowToPlayButton`, `_mainMenuLeaderboardButton`, `_mainMenuSettingsButton`, `_mainMenuQuitButton` | MainMenuPanel의 해당 Button | Root에 따름 | On Click → 각각 `GameSystem.SelectPlay`, `SelectHowToPlay`, `SelectLeaderboard`, `SelectSettings`, `SelectQuit` |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | `_modeSelectStageButton`, `_modeSelectInfiniteButton`, `_modeSelectBackButton` | ModeSelectPanel의 해당 Button | 비활성 | On Click → 각각 `GameSystem.SelectStage`, `SelectInfinite`, `SelectBack` |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | 기존 `_pauseResumeButton`, `_pauseRetryButton`, 신규 `_pauseSettingsButton`, `_pauseMainMenuButton` | PausePanel의 Resume, Retry, Settings, Main Menu Button | 비활성 | On Click → 각각 `GameSystem.SelectResume`, `SelectRetry`, `SelectSettings`, `SelectMainMenu`. 기존 `_pauseQuitButton`에는 새 연결을 남기지 않는다. |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | `_pauseMainMenuConfirmButton`, `_pauseMainMenuCancelButton` | PauseMainMenuConfirmPanel의 Main Menu, Cancel Button | 비활성 | On Click → 각각 `GameSystem.SelectMainMenu`, `SelectCancel` |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | 기존 `_retryButton`, 신규 `_resultMainMenuButton` | ResultPanel의 Retry, Main Menu Button | 비활성 | On Click → 각각 `GameSystem.SelectRetry`, `SelectMainMenu`. 기존 `_quitButton`에는 새 연결을 남기지 않는다. |
| `UIManagementSystem` | `FlowState.Runtime.Systems.UIManagementSystem` | `_leaderboardBackButton`, `_howToPlayBackButton`, `_settingsBackButton` | 각 기능 화면의 Back Button | 비활성 | On Click → 모두 `GameSystem.SelectBack` |
| `EventSystem` | `UnityEngine.InputSystem.UI.InputSystemUIInputModule` | 기존 Move/Submit/Cancel/Point/Left Click 참조 | 기존 `InputSystem_Actions` UI Map | Step 5에서 활성 완료 | Module이 Navigate·Submit·Click을 Button UI에 전달. GameSystem은 메뉴 Navigate·Submit·Click을 별도 처리하지 않음. |

각 Button On Click에는 위 표의 `GameSystem.Select*` 메서드 하나만 등록한다. `RequestNavigationSelection(E_NavigationItem)`은 Unity Button 드롭다운에 표시되지 않을 수 있으므로 연결 대상으로 사용하지 않는다. 기존 GameSystem Quit·Retry 메서드 또는 중복 리스너를 함께 등록하지 않는다.

## Step 3. 코드 정적 검사 후 Unity 컴파일과 Edit Mode Test를 확인한다

### AI 작업

- API·namespace·asmdef 의존성, 직렬화 필드 변경, 이벤트 등록/해제, null 처리, 중복 입력 경로와 문서 계약을 검사한다.
- `git diff --check`와 변경 범위를 검사하고 실행할 실제 Test 클래스 목록을 제공한다.

### 사용자 수동 작업

1. Unity Editor가 Script Compilation을 완료할 때까지 기다린다. 아직 Play를 누르지 않는다.
2. Console의 Compile Error 및 예상하지 않은 Warning을 확인한다. 발생하면 메시지·파일·행을 전달한다.
3. Test Runner의 Edit Mode에서 AI가 지정한 관련 Test를 실행하고 통과 후 전체 Edit Mode를 실행한다.
4. 실행 수, Passed/Failed 및 예상하지 않은 Error/Warning 유무를 전달한다. 실패 시 이름·메시지·Stack Trace를 전달한다.

### Step 3 정적 검사 결과 (20260920)

- `GameSystem`, `UIManagementSystem`, `ProductionSceneGameModeTestUtility`, `GameEntryBootIntegrationTests`의 새 공개 API·호출 지점을 대조했다. `RequestNavigationSelection(E_NavigationItem)`, `InitializeMenu()`, `SetNavigationScreen(E_NavigationScreen, E_NavigationItem)`의 정의와 호출이 일치한다.
- 신규 Play Mode Test는 `FlowState.PlayModeTests`가 이미 참조하는 `FlowState.Runtime.Core`의 Navigation enum만 정적으로 참조한다. 생산 `GameSystem`은 기존 Test와 같은 reflection 호출로 찾아 별도 asmdef 의존성을 만들지 않는다.
- 새 코드에 `System.Linq`, 새 nullable 연산자, `public event` 또는 Scene/Prefab·Input Actions·Package·ProjectSettings 변경이 없다. `git diff --check`는 공백 오류 없이 통과했다.
- 사용자 실행 대상 관련 Edit Mode Test는 `GameNavigationStateTests`, `GameStateTests`, `GameRuntimeDataTests`, `PauseMenuStateTests`, `UIVisibilityStateTests`다. 이들은 모두 통과한 뒤 전체 Edit Mode를 실행한다.
- 신규·영향 Play Mode Test는 Step 7에서 실행한다. 우선 목록은 `GameEntryBootIntegrationTests`, `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `UIInputSystemTests`, `PauseMenuIntegrationTests`, `ResultMenuIntegrationTests`, `PausePanelSceneConfigurationTests`, `ModeUISceneConfigurationTests`, `ModeResultDisplayIntegrationTests` 및 Stage/Infinite Mode 통합 Test다. Scene 연결 전에는 새 Scene 구성 Test를 실행 대상으로 지정하지 않는다.
- Unity Script Compilation, Unity Test Runner, Build, Scene/Prefab 수정은 수행하지 않았다. 따라서 아래 완료 조건은 사용자 실행 결과를 받은 뒤에만 완료 처리한다.

### 완료 조건

- [x] 컴파일과 관련/전체 Edit Mode가 통과했고 예상하지 않은 Error/Warning이 없다.

### 수행 결과 (20260920)

- 사용자가 Unity Script Compilation 성공을 확인했고 예상하지 않은 Error/Warning이 없다고 전달했다.
- 사용자가 전체 Edit Mode Test 672개를 실행해 모두 성공했고, 관련 예상하지 않은 Error/Warning이 없다고 전달했다.
- 새 `GameEntryBootIntegrationTests`를 포함한 신규·영향 Play Mode Test는 Step 7의 실행 대상이다. 이 Test들은 실제 생산 Scene의 Boot와 메뉴/입력 구성까지 검증하므로 Step 4~6의 Scene UI·EventSystem·직렬화 참조 작업과 정적 Scene 검사가 끝난 뒤 관련 Test와 전체 Play Mode 회귀로 실행한다.
- Unity Test Runner와 Build는 AI가 실행하지 않았다. Scene/Prefab 수정도 수행하지 않았다.

## Step 4. Scene 메뉴 UI를 제작·배치한다

선행 조건: Step 3 통과 및 Step 2의 실제 연결표 제공. 아래 이름은 신규 객체의 권장 이름이며 기존 객체를 대체하거나 재배치할 때는 확정 연결표를 따른다.

### 사용자 수동 작업

`SampleScene`의 `UIRoot`는 Canvas가 아니며, 그 아래 StageHUD·InfiniteHUD·Momentum HUD·ResultPanel·PausePanel이 각각 별도 Canvas를 소유한다. 신규 메뉴를 기존 HUD 또는 Pause/Result의 Canvas 아래에 넣지 않는다. `UIRoot`의 직접 자식으로 Screen Space - Overlay `MenuCanvas`를 하나 만들고, Canvas Scaler와 Graphic Raycaster를 함께 둔다. `MenuCanvas`는 기존 PausePanel보다 Hierarchy에서 뒤에 두고 Override Sorting을 사용한다면 Sorting Order를 기존 HUD/Result보다 높게 둔다. 아래 신규 Panel은 모두 이 `MenuCanvas`의 자식으로 만든다.

1. Play Mode를 종료하고 Project 창에서 `Assets/Scenes/SampleScene.unity`를 연다.
2. `UIRoot/MenuCanvas` 아래에 Panel과 `Button - TextMeshPro`를 만든다. 기존 HUD·Result 내용·Momentum HUD 참조는 보존한다.
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

- [x] 필요한 화면과 버튼이 저장됐고 Scene 구조 정적 검사가 통과했다.

### 수행 결과 (20260920)

- `UIRoot`의 직접 자식 `MenuCanvas`를 확인했다. Screen Space - Overlay Canvas, Canvas Scaler(Reference Resolution `1920 x 1080`), Graphic Raycaster가 있으며, 신규 메뉴 Root의 부모로 사용된다.
- `MenuCanvas` 아래에 `MainMenuPanel`, `ModeSelectPanel`, `PauseMainMenuConfirmPanel`, `LeaderboardPanel`, `HowToPlayPanel`, `SettingsPanel`이 모두 있다. MainMenuPanel만 활성이고 나머지 신규 Root는 비활성으로 저장됐다.
- Main Menu의 Play/How To Play/Leaderboard/Settings/Quit, Mode Select의 Stage/Infinite/Back, 확인 화면의 Main Menu/Cancel, 기능 화면의 Back Button을 확인했다.
- 기존 PausePanel은 비활성 상태로 저장됐으며 Resume/Retry/Settings/Main Menu Button 구성을 확인했다. 기존 ResultPanel은 비활성 상태로 저장됐으며 Retry/Main Menu Button 구성을 확인했다. 기존 StageHUD, InfiniteHUD, Result Content 및 UIManagementSystem의 기존 HUD·Result 참조는 보존됐다.
- 신규 UIManagementSystem 직렬화 필드와 Button On Click, EventSystem Navigation은 의도적으로 아직 비어 있으며 Step 5에서 연결한다. Unity Editor Play Mode, Test Runner, Build는 실행하지 않았다.

## Step 5. Navigation·EventSystem·System 참조를 연결한다

### 사용자 수동 작업

1. 각 메뉴 Button의 Navigation을 `Explicit`으로 설정한다. Select On Up/Down에는 같은 화면의 이전/다음 Button을 연결한다. 첫 항목 Up과 마지막 항목 Down, 세로 목록 Left/Right는 None으로 두어 다른 화면이나 반대 끝으로 이동하지 않게 한다. Back만 있는 화면은 네 방향 모두 None으로 둔다.
2. 기존 EventSystem을 선택하고 Step 2에서 확정한 입력 연결표를 적용한다. 표준 InputSystemUIInputModule을 사용하는 경우 해당 Component를 활성화하고 기존 UI Action Asset의 Navigate/Submit/Cancel/Point/Click을 각각 Move/Submit/Cancel/Point/Left Click에 연결한다. 반복 지연 0.5초·간격 0.1초를 사용한다. 정확한 Asset 경로·Action 참조는 AI가 정적으로 찾아 연결표로 제공한다.
3. First Selected는 연결표에 따라 Main Menu의 Play를 지정한다. 이후 화면별 선택 및 복귀 선택은 생산 코드가 갱신한다. 배경 Click이 선택을 지우지 않도록 Deselect On Background Click 설정을 연결표대로 적용한다.
4. UIManagementSystem과 신규 UI Component의 Root·Button·Text 필드에 Step 4에서 만든 객체를 드래그한다. Pause/Result의 기존 Quit 연결은 Main Menu 동작으로 변경한다. 이름/문구만 바꾸고 Quit 호출을 남기지 않는다.
5. Button On Click은 Step 2의 등록 주체 표를 따른다. 코드가 등록하는 버튼에는 Inspector 이벤트를 중복 추가하지 않는다. Inspector 연결 방식이면 표에 적힌 실제 Component·메서드·인자를 지정한다.
6. Scene을 저장하고 완료를 전달한다. 참조의 정확성·클릭 실행 횟수는 다음 단계의 정적 검사와 Test로 판정한다.

### 완료 조건

- [x] 연결표의 모든 참조·Navigation·이벤트 연결을 Scene에 저장했다.

### 수행 결과 (20260921)

- `UIManagementSystem`의 신규 Panel 6개와 Button 19개 참조가 모두 저장됐다. 기존 `_quitButton`, `_pauseQuitButton`은 비어 있어 기존 Quit 경로를 중복 호출하지 않는다.
- 19개 Button On Click은 모두 `GameSystem`의 인자 없는 `Select*` 메서드 하나를 대상으로 저장됐다. Main Menu 5개, Mode Select 3개, Pause 4개, Main Menu 확인 2개, Result 2개, 기능 화면 Back 3개의 연결이 표와 일치한다.
- 다중 항목 화면은 Explicit Navigation(상·하 인접 Button, 경계와 좌·우 None)으로 저장됐고, 단일 Back Button 화면 3개는 모든 방향 None으로 저장됐다.
- EventSystem의 `InputSystemUIInputModule`은 활성화돼 있으며 UI Action Asset 참조, Move/Submit/Cancel/Point/Left Click 참조, Repeat Delay `0.5`, Repeat Rate `0.1`, Deselect On Background Click 해제 및 Main Menu Play First Selected가 저장됐다.
- Scene은 읽기 전용 정적 검사만 수행했다. Unity Editor Play Mode, Test Runner, Build는 실행하지 않았다.

## Step 6. Scene·입력 연결을 정적으로 검사한다

### AI 작업

- Scene YAML의 fileID/GUID를 실제 Script·Input Actions·객체로 해석하여 누락/잘못된 타입/중복 EventSystem·입력 처리·On Click 연결을 검사한다.
- 같은 화면 내 Explicit Navigation 연결, clamp 경계, Back 접근, 초기 Root 활성 상태, Canvas 정렬·Raycast 및 기존 HUD/Result 참조 보존을 검사한다.
- enum 직렬화 값과 기존 Quit 참조의 전환, UI Action Asset의 Keyboard/Gamepad/Mouse Binding 및 반복 설정을 대조한다.
- 수정 필요 시 사용자에게 정확한 객체 경로·Inspector 필드·현재 값·목표 값만 제공한다. 사용자 저장 후 변경 부분을 다시 정적 검사한다.

### 사용자 수동 작업

AI가 지적한 Scene 항목만 Inspector에서 수정하고 저장한다. 정적으로 통과한 항목을 다시 육안으로 확인할 필요는 없다.

### 완료 조건

- [x] Scene·입력 연결과 변경 범위 정적 검사가 통과했다.

### 수행 결과 (20260921)

- `SampleScene`에는 `EventSystem`과 `InputSystemUIInputModule`이 각각 하나만 존재한다. Module은 `InputSystem_Actions`의 UI Action Asset을 참조하며 Move/Submit/Cancel/Point/Left Click 참조, 반복 지연 `0.5`초·간격 `0.1`초, Background Click 선택 해제 비활성, First Selected=Main Menu Play 설정이 모두 일치한다.
- UI Map에는 Navigate의 Keyboard W/화살표 및 Gamepad Stick/D-pad, Submit/Cancel의 공통 Submit/Cancel, Point의 Mouse/Pen/Touch, Click의 Mouse/Pen/Touch/XR 바인딩이 존재한다.
- `UIManagementSystem`의 신규 Root 6개와 Button 19개 참조는 모두 유효한 Scene 객체를 가리킨다. 기존 `_quitButton`, `_pauseQuitButton`은 비어 있어 이전 Quit 이벤트가 남아 있지 않다.
- 19개 Button은 모두 GameSystem의 인자 없는 `Select*` 메서드 하나만 호출하며, 대상 fileID는 동일한 `GameSystem` Component다. 메뉴·모드·Pause·확인·Result·Back의 메서드 매핑이 연결표와 일치한다.
- 다중 Button 화면의 Navigation은 Explicit 상·하 인접 연결과 경계/좌우 None으로, 단일 Back 화면은 전체 None으로 저장됐다. Main Menu만 활성이고 신규 나머지 Root, PausePanel 및 ResultPanel은 비활성이다. MenuCanvas는 Screen Space Overlay, GraphicRaycaster 활성, Sorting Order `10`으로 저장돼 기존 HUD Canvas보다 앞선다.
- Unity가 생성한 빈 YAML 값의 후행 공백은 `git diff --check`에서 보고되지만, 새 Event/참조의 기능 오류나 사용자 수정 대상으로 판정하지 않았다. Scene 수정, Unity Editor Play Mode, Test Runner, Build는 수행하지 않았다.

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

- [x] 최종 변경 기준 컴파일, Edit Mode 및 Play Mode 회귀가 통과했고 예상하지 않은 Error/Warning이 없다.

### 실행 대상 정적 확정 (20260921)

- Step 2 이후 Inspector용 `GameSystem.Select*` 메서드가 추가됐으므로, 이전 Step 3의 컴파일/Edit Mode 성공 결과만으로는 최종 변경 기준을 충족하지 않는다. Unity Script Compilation과 전체 Edit Mode를 다시 실행한다.
- 우선 Play Mode Test Runner에서 다음 9개 Fixture를 실행한다: `GameEntryBootIntegrationTests`(4), `GameLifecycleIntegrationTests`(7), `GamePauseOrchestrationTests`(5), `UIInputSystemTests`(3), `PauseMenuIntegrationTests`(9), `ResultMenuIntegrationTests`(8), `PausePanelSceneConfigurationTests`(1), `ModeUISceneConfigurationTests`(2), `ModeResultDisplayIntegrationTests`(7). 합계 46개다.
- 위 우선 Fixture가 통과하면 전체 Play Mode 회귀를 실행한다. 이 목록에는 `ProductionSceneGameModeTestUtility`로 Stage/Infinite 시작을 준비하는 기존 통합 Test가 포함되므로, Boot 변경 이후의 기존 게임 흐름 회귀도 함께 판정한다.
- Test 대상 Fixture, 새 `GameEntryBootIntegrationTests`의 4개 UnityTest, `RequestNavigationSelection(E_NavigationItem)` reflection 호출과 공개 API의 이름·인자 형식을 정적으로 대조했다. 불일치는 발견되지 않았다.
- Unity Editor Play Mode, Test Runner, Build는 AI가 실행하지 않았다. 사용자 실행 결과를 받은 뒤에만 이 Step을 완료 처리한다.

### 실패 분석 및 Test 갱신 (20260921)

- 사용자 실행에서 Play Mode 13건 실패가 보고됐다. 확인된 `ModeUI_HasRequiredHierarchyComponentsAndReferences`, `PausePanel_HasRequiredHierarchyReferencesAndStateMapping`은 이전 `QuitButton`과 `_quitButton`/`_pauseQuitButton` 직렬화 계약을 요구한 Test가 원인이었다. 현재 계약인 Result/Pause `MainMenuButton`, `_resultMainMenuButton`, `_pauseMainMenuButton`으로 갱신했다.
- 확인된 Infinite Retry와 Result Retry 실패는 새 EventSystem Button 경로가 활성화된 뒤에도 Test가 더 이상 메뉴를 처리하지 않는 `UIInputSystem` private Submit/Click/Navigate 플래그를 주입한 것이 원인이었다. `PauseMenuIntegrationTests`와 `ResultMenuIntegrationTests`는 실제 Button `onClick` 및 `GameSystem.SelectRetry` 공개 진입점을 사용하도록 갱신했다.
- Pause Main Menu 및 Result Main Menu Test는 Application Quit을 기대하지 않고, 확인 화면 전환 또는 Main Menu 복귀와 Quit Service 미호출을 판정하도록 변경했다. 변경은 Scene을 수정하지 않는다.
- 변경된 Test 4개 파일의 정적 API·필드명·Button 이름 대조와 `git diff --check`를 통과했다. Unity Script Compilation과 Test Runner는 사용자 재실행이 필요하다.
- 재실행에서 남은 `ModeUI_HasRequiredHierarchyComponentsAndReferences` 실패는 Boot에서 Run Data를 만들지 않는 새 계약상 `MomentumGradientEffect.HasGradient`가 false인 점을 예전 Test가 true로 기대한 것이 원인이었다. Boot 기대값을 false로 갱신했다.
- 재실행에서 남은 `InfiniteResult_KeyboardSubmitRetry_StartsInfiniteRun` SetUp의 NullReference는 Run 정리/재시작 전환 프레임에 `UIManagementSystem.Update()`가 null Runtime Data를 읽은 것이 원인이었다. Runtime Data가 없는 경우 HUD 갱신을 건너뛰도록 방어했고, UI 상태 전환이나 Scene을 변경하지 않았다.
- 사용자가 최종 Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 없음을 확인했다. Play Mode Test 228개가 모두 성공했고 예상하지 않은 Error/Warning이 없음을 확인했다.
- 마지막 Edit Mode 성공 이후 `UIManagementSystem` Runtime 코드와 Play Mode Test가 갱신됐으므로, 최종 변경 기준 전체 Edit Mode 재실행 결과가 아직 필요하다. 해당 결과를 받기 전에는 이 Step을 완료 처리하지 않는다.
- 사용자가 최종 변경 기준 전체 Edit Mode Test 672개 성공 및 예상하지 않은 Error/Warning 없음을 확인했다. 이로써 Unity Script Compilation, Edit Mode 672개, Play Mode 228개가 모두 성공했고 Step 7을 완료 처리한다.

## Step 8. 실제 입력 장치와 화면 표현을 확인한다

### 사용자 수동 작업

1. Gamepad를 보유한 경우 연결하고 Game View에서 Play를 시작한다. Main Menu 버튼의 글자와 선택 표시가 구분되는지 본다.
2. Keyboard 방향키/WASD와 Submit으로 Main Menu → Mode Select → Back을 천천히 이동한다. Gamepad D-pad/Stick·Submit·Cancel로 같은 경로를 이동하며 Focus 가시성과 조작감을 확인한다.
3. Mouse로 각 기능 화면을 열고 화면의 Back 버튼을 클릭한다. Keyboard로 Back 버튼을 선택·Submit했을 때도 화면에서 선택과 복귀를 이해할 수 있는지 확인한다.
4. Stage와 Infinite를 각각 시작하여 Pause → Settings → Back, Pause → Main Menu 확인 → Cancel 및 Main Menu 확정, Result 메뉴의 버튼·HUD가 읽히고 겹치지 않는지 확인한다. Result 도달에 장시간 플레이가 필요하면 AI가 기존 생산 종료 경로를 사용하는 재현 절차를 먼저 제공한다.
5. Keyboard → Mouse → Gamepad 순서로 천천히 장치를 바꾸며 선택 표시가 보이지 않거나 조작이 끊기는지 확인한다. 수행 중인 Game View 해상도·장치와 문제 화면·재현 순서를 기록한다.
6. Play Mode를 종료하고 결과를 전달한다. 화면 변경이 필요하면 Edit Mode에서 수정·저장하고 AI의 정적 재검사를 받는다.

Run 개수·물리 정지·Timer 값·이벤트 횟수·중복 입력·짧은 타이밍 경합은 Step 7 Test로 판정한다. IDE 디버거 사용, 빠른 연타나 정밀 타이밍 조작을 요구하지 않는다. GamePad를 보유하지 않은 경우 해당 항목은 미확인 사유를 기록하고, 사용자가 승인한 검증 제외로 처리한다.

### 완료 조건

- [x] 사용 가능한 실제 입력 장치의 Focus·가독성·조작 확인 결과와 GamePad 미확인 사유가 기록됐다.

### 수행 결과 (20260921)

- 사용자가 Mouse와 Keyboard로 메뉴 Focus·Button 조작을 확인했고 정상 동작을 확인했다.
- GamePad는 보유하지 않아 확인하지 못했다. 사용자의 명시적 승인에 따라 미확인 사유를 기록한 검증 제외로 처리하며 Step 8을 완료 처리한다.

## Step 9. Phase 2 결과를 기록하고 다음 Phase로 인계한다

### AI 작업

- Roadmap Phase 2 완료 조건 각각에 정적 검사·Test·화면 확인 근거를 연결하고 최종 변경 이후 결과인지 확인한다.
- 실제 컴파일·Test 결과, 사용자 Scene 변경 범위, 화면 확인 결과를 기록한다. 미확인 또는 실패 항목이 있으면 완료 처리하지 않는다.
- 완료 근거가 충족되면 Roadmap Phase 2만 완료로 바꾸고 다음 작업을 Phase 3으로 갱신한다. Settings 세부 기능·Rebinding·How To Play 자동 안내·영구 저장을 구현 완료로 기록하지 않는다.

### 사용자 수동 작업

없음. Phase 2의 필수 완료 조건에 별도 Build는 없다. 사용자가 Build를 수행한다면 결과는 별도로 기록하고, 전체 UI 흐름의 필수 Build 검증은 Roadmap Phase 4에 유지한다.

### 완료 조건

- [x] Phase 2 완료 근거와 Phase 3 인계가 기록됐다.

### 수행 결과 (20260921)

- Boot에서 Run·Player·Stage·Timer가 시작하지 않는 조건은 `GameEntryBootIntegrationTests`를 포함한 최종 Play Mode 228개 성공으로 확인했다. Mode Select 후에만 Stage/Infinite Run이 시작되고, Retry가 같은 Mode의 새 Run을 시작하며, Main Menu 복귀가 Runtime Data를 정리하는 조건도 해당 회귀와 Step 6 정적 연결 검사로 확인했다.
- 메뉴 Root·Button 19개·EventSystem·Input Action·Navigation·On Click 연결은 Step 4~6의 Scene 정적 검사와 구성 Test로 확인했다. Keyboard와 Mouse의 Focus·조작은 사용자 확인으로 정상이며, GamePad 미보유는 사용자가 승인한 검증 제외 사유로 기록했다.
- 최종 Unity Script Compilation 성공, Edit Mode 672개 성공, Play Mode 228개 성공 및 각 단계의 예상하지 않은 Error/Warning 없음을 사용자 확인으로 기록했다. Phase 2의 필수 Build는 없으며 AI는 Build를 실행하지 않았다.
- Roadmap 006의 Phase 2를 완료로 바꾸고 다음 작업을 Phase 3 기능 접근 UI·Settings 범위 확정·Input Rebinding 작업 준비로 갱신했다. Settings 세부 기능, Input Rebinding, How To Play 자동 안내, 영구 저장과 Build 검증은 구현 완료로 기록하지 않았으며 이후 Phase 범위로 인계한다.

# 영향 범위

Phase 2 실행으로 GameSystem·UIManagementSystem 생산 연결, 관련 Play Mode Test와 공통 Test 준비 경로, Roadmap 및 이 Task 문서가 변경됐다. SampleScene의 메뉴 UI·EventSystem·직렬화 참조는 사용자가 편집했다. AI는 Scene/Prefab을 수정하지 않았다.

# 검증 내용

Roadmap Phase 2 완료 조건을 Step 1~9의 정적 검사, 생산 Scene 구성, 최종 컴파일·Edit/Play Mode 회귀 및 사용자 화면 확인 결과에 대응시켰다. Inspector 필드와 Test 이름은 실제 구현 결과를 기준으로 확정했다.

# 검증 결과

Phase 2 구현과 사용자 Scene 편집을 완료했다. 최종 Unity Script Compilation, Edit Mode 672개, Play Mode 228개 성공 및 예상하지 않은 Error/Warning 없음을 기록했다. AI는 Unity Editor, Test Runner, Build 및 Scene/Prefab 수정을 수행하지 않았다. Step 1~9 완료 조건을 충족했다.

# 후속 작업

Phase 3 기능 접근 UI·Settings 범위 확정·Input Rebinding 작업을 준비한다.

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
