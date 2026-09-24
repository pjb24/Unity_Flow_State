# 작업 정보

## 작업명

Prototype 6 Phase 4 — How To Play, 첫 플레이 안내 및 전체 UI 흐름 수동 작업 계획

## 작업 일자

20260923

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. Phase 4 구현과 아래 Step 수행은 대기 상태다. 각 Step은 실제 수행 근거가 확보된 뒤 완료 처리한다.

# 작업 목적

Roadmap 006 Phase 4를 실행할 순서와 사용자 수동 작업을 정의한다. 상태·수치·연결은 정적 검사와 Unit/통합 Test로 검증하고, 사용자는 제품 규칙 결정, Scene 편집, Unity 검증 실행 및 실제 화면·입력 확인을 수행한다.

# 작업 대상

- 기존 How To Play 화면과 Main Menu 진입·복귀
- 첫 Run 전 안내, 선택 Mode 보존 및 Run 시작 연결
- 자동 이동, Jump, Momentum Landing, Collectible, Pause와 Mode별 목표 안내
- 실제 Player/UI Input Binding을 반영하는 Keyboard·Gamepad 조작 표시
- Navigation 상태, GameSystem, UIManagementSystem, PlayerInputSystem, UIInputSystem 및 Settings 연계
- Edit Mode Unit Test, 생산 Scene 기반 Play Mode Test, Scene 정적 검사
- 사용자가 편집할 `Assets/Scenes/SampleScene.unity`

별도 Tutorial Stage, 영구 저장, 실제 Leaderboard 통신은 포함하지 않는다. Quit 종료 지연은 Phase 3에서 인계된 별도 관찰 사항이며 이 작업에서 원인을 확정하거나 수정하지 않는다.

# 작업 전 상태

- Roadmap 006 Phase 1~3 및 Phase 3 Step 1~10은 완료 상태다.
- 사용자 보고 기준 Script Compilation 성공, Edit Mode 692개·Play Mode 226개 성공, 예상하지 않은 Error/Warning 없음이 마지막 검증 기준이다. Phase 4 성공 결과로 재사용하지 않는다.
- `HowToPlay.md`에는 Main Menu 재열람·복귀와 최초 Run 전 자동 안내 규칙이 있다. 단말기별 완료 저장은 Roadmap 7 범위다.
- `GameNavigationState.SubmitModeSelect()`는 Mode 선택 후 `BeginInitialization()`을 호출한다. 첫 안내를 거치는 생산 상태는 아직 없다.
- `GameSystem.SelectHowToPlay()`와 `UIManagementSystem`의 How To Play Root·Back 참조가 있다. Scene에는 `How To Play is in preparation.` 문구가 남아 있다.
- Settings는 Keyboard Jump, Momentum Landing, Navigate 4방향을 재지정한다. Submit·Cancel은 고정 입력이며 Player Move는 제거됐다.
- Player/UI System에는 실제 Binding Action 조회 경계가 있고 Settings에는 Binding 표시 경로 조회가 있다. 안내 전용 입력 Asset 복제본을 새로 만들 필요가 있는지부터 검토해야 한다.
- Gamepad는 사용자 장치 미보유로 Phase 3 실제 장치 검증에서 제외됐다. Phase 4에서도 자동 검증과 실제 장치 검증을 구분한다.

# 조사 내용

`AI/README.md`의 Project·Rules 확인 순서와 Roadmap 006, Phase 3 인계, HowToPlay·GameEntryNavigation Feature, GameSystem·UIManagementSystem·UIInputSystem 문서를 확인했다. Navigation 코드, GameSystem 진입 흐름, Binding 조회 경계, 기존 Test 목록과 Scene의 안내 자리표시자를 정적으로 확인했다.

이 문서는 여러 System·Feature의 구현과 수동 Scene 편집을 조정하는 실행 계획이므로 GENERAL_TASK_TEMPLATE을 사용한다. 세부 API·Scene 값은 구현 전에 임의로 확정하지 않고 Step 4에서 실제 코드 기준의 단일 편집표로 제공한다.

# 작업 내용

AI는 코드·Test·문서 작성과 정적 검사를 담당한다. Unity Editor·Script Compilation·Test Runner·Build 실행은 사용자 작업이다. AI는 Scene/Prefab YAML을 편집하거나 이를 수정하는 Editor Script를 만들지 않는다. 기존 작업 트리의 Phase 3 변경은 보존한다.

## Step 1. 첫 안내와 조작 설명의 규칙을 확정한다

### AI 작업

- 기존 Feature와 생산 코드에서 확정 규칙 및 미정 규칙을 구분했다.
- 사용자 승인으로 아래 계약을 확정했다.
- 안내 문구는 실제 자동 이동·관성 착지·Score 규칙과 대조한다. 제거한 Move 조작과 Momentum Landing의 속도 관련 효과를 안내하지 않는다.

| 결정 대상 | 확정 계약 |
|---|---|
| 자동 안내 횟수 | Application 실행 중 최초 Run 전 1회이며 Stage와 InfiniteMode에 공통으로 적용한다. Application을 다시 실행하면 안내 처리 상태를 초기화한다. |
| 안내 구성 | 기존 How To Play 화면을 공용 단일 페이지로 확장한다. |
| 자동 안내 완료 | 자동 안내에는 `Start Run`만 제공한다. 요청 시 선택된 Mode의 Run을 시작하고 이번 Application의 자동 안내 처리 완료로 기록한다. |
| 자동 안내의 Back·Cancel | Back과 Skip UI를 제공하지 않는다. Cancel 입력은 무반응으로 처리한다. |
| Main Menu 재열람 | Back만 제공하고 Main Menu의 How To Play 선택으로 복귀한다. 재열람은 자동 안내 처리 상태에 영향을 주지 않는다. |
| 기본 Focus | 자동 안내는 `Start Run`, Main Menu 재열람은 `Back`을 기본 선택으로 사용한다. |
| Binding 표기 | 마지막 Keyboard 또는 Mouse 입력 뒤에는 Keyboard Binding을, 마지막 Gamepad 입력 뒤에는 Gamepad Binding을 표시한다. |
| Run 초기화 실패 | 자동 안내 처리 완료 상태를 유지하고 기존 초기화 실패 복귀 경로를 사용한다. |
| 안내 순서와 문구 | 자동 이동, Jump, Momentum Landing, Collectible, Pause, Mode별 목표 순서의 행동 중심 문구를 사용한다. Momentum Landing은 착지 직전 입력으로 수행함을 설명하고, InfiniteMode에서 연속 성공 시 거리 Score 배율이 상승함을 이점으로 안내한다. 속도·속도 유지·속도 증가 표현은 사용하지 않는다. |

### 사용자 수동 작업

1. 확정 계약을 검토한다. 변경이 필요하면 구현 전에 전달한다.

### 완료 조건

- [x] 표시 횟수, 진입 출처, 완료·Cancel, 실패 재시도, Focus 및 표시 문구가 확정됐다.

## Step 2. 상태 계약과 자동 검증 명세를 작성한다

### AI 작업

- HowToPlay와 GameEntryNavigation의 관계를 정리했다. Mode 선택 후 자동 안내가 필요한 경우 Run 요청을 보류하는 예외를 Feature 문서에 명시했다.
- 자동 안내 상태, 보류 Mode, 이번 Application의 안내 처리 여부는 `GameNavigationState`가 단일 책임으로 관리한다. Run 데이터 정리·Retry·Main Menu 복귀는 이 상태를 초기화하지 않는다.
- 자동 안내는 기존 Main Menu 재열람 How To Play과 구분되는 Navigation 표시 상태를 사용한다. 두 상태는 공용 안내 본문을 사용하지만 버튼 구성과 Submit·Cancel 의미가 다르다.
- GameSystem은 전이와 Action Map 요청, UIManagementSystem은 표시·Focus 반영, PlayerInputSystem·UIInputSystem은 Action Map과 실제 입력 수집, SettingsSystem은 실제 Binding 조회를 담당한다. 입력 소비와 전이는 관측 가능한 State로 판정하며 고정 프레임 대기나 임의 지연으로 중복 시작을 막지 않는다.

### 확정 상태 계약

| 상태 또는 데이터 | 소유자 | 진입·변경 규칙 | 불변 조건 |
|---|---|---|---|
| 자동 안내 처리 여부 | `GameNavigationState` | 새 Application에서는 미처리다. 자동 안내의 Start Run 요청 시 처리로 변경한다. | 재열람, Run 데이터 정리, Retry, Main Menu 복귀와 초기화 실패는 처리 여부를 변경하지 않는다. |
| 보류 Mode | `GameNavigationState` | 자동 안내 미처리 상태에서 Stage 또는 Infinite 선택을 Submit하면 선택 Mode를 보존한다. | Start Run은 보류 Mode만 시작한다. 유효하지 않은 선택·중복 요청·Cancel은 값을 변경하지 않는다. |
| 자동 안내 표시 | `GameNavigationState` | 미처리 상태의 유효 Mode Submit에서 진입한다. | Run Runtime Data, Stage, Timer는 Start Run 전 생성·시작하지 않는다. Player 입력은 비활성, UI 입력은 활성이다. |
| 자동 안내 Start Run | `GameNavigationState`와 GameSystem | Start Run Submit은 처리 여부를 기록하고 한 번의 Run 초기화 요청만 만든다. | 같은 요청의 중복 Submit·Click은 두 번째 Run 초기화 요청을 만들지 않는다. 완료 입력은 Player Jump 또는 Pause로 재사용되지 않는다. |
| 자동 안내 Cancel | `GameNavigationState` | Cancel 입력은 소비하되 상태·선택·보류 Mode·처리 여부를 변경하지 않는다. | Back·Skip UI와 Main Menu 또는 Mode Select 복귀 전이를 만들지 않는다. |
| Main Menu 재열람 | `GameNavigationState` | Main Menu How To Play Submit에서 진입하며 Back 또는 Cancel로 How To Play 선택에 복귀한다. | 자동 안내 처리 여부·보류 Mode·Run 요청을 변경하지 않는다. |
| Run 초기화 실패 | GameSystem과 `GameNavigationState` | 기존 실패 정리 후 Main Menu Play 선택으로 복귀한다. | 자동 안내 처리 여부는 Start Run 요청 때 기록된 상태를 유지한다. |

### System 경계 계약

| 담당자 | Step 2에서 확정한 책임 | 담당하지 않는 책임 |
|---|---|---|
| GameSystem | Navigation 전이를 GameNavigationState에 요청하고, 자동 안내 중 Player Action Map 비활성·UI Action Map 활성 상태를 요청하며 Start Run 뒤 기존 초기화 순서를 시작한다. | 안내 처리 여부·보류 Mode를 별도 bool로 중복 소유하거나 UI 선택을 직접 관리하지 않는다. |
| UIManagementSystem | 자동 안내와 재열람의 Root·버튼 구성·현재 Binding 표기·기본 Focus를 표시한다. | 어떤 입력이 Run을 시작하는지, 안내 처리 여부와 보류 Mode를 결정하지 않는다. |
| PlayerInputSystem / UIInputSystem | GameSystem 요청에 따라 각 Action Map을 관리하고 실제 Keyboard·Mouse·Gamepad 입력을 수집한다. | 자동 안내의 전이 의미, Binding 표시 문구 또는 안내 완료 상태를 결정하지 않는다. |
| SettingsSystem | 실제 Player/UI Action의 기본 Binding과 Override를 조회한다. | 안내 전용 Input Action Asset을 만들거나 마지막 입력 장치 상태를 소유하지 않는다. |

### 자동 검증 명세

| 대상 | 검증 계층 | 필수 검증 |
|---|---|---|
| 최초 안내 | Edit Mode | 새 `GameNavigationState`에서 Stage와 Infinite 각각의 최초 선택이 자동 안내로 전이하고 Run 요청을 만들지 않는지, 재열람과 자동 안내를 구분하는지 검증한다. |
| 완료·Cancel | Edit Mode | 보류 Mode 보존, Start Run 한 번만 허용, Cancel 무반응, 중복 Submit·Click 거부를 검증한다. |
| 반복 진입 | Edit Mode | Retry·Main Menu 복귀·Mode 변경·새 `GameNavigationState`·초기화 실패에서 안내 처리 여부와 보류 Mode가 계약대로 유지·초기화되는지 검증한다. |
| Binding 표시 | Edit Mode 및 Input System 경계 Test | 기본값·Override·Restore Defaults, Keyboard·Mouse 대 Gamepad 장치 분류, 고정 Submit·Cancel, Binding 누락 폴백을 검증한다. 기본 키 문자열을 정답 원천으로 사용하지 않는다. |
| 안내 중 진행 차단 | Play Mode | 생산 Scene에서 Run 데이터 미생성, Stage·Timer 미시작, Player Action Map 비활성, UI Action Map 활성 상태를 검증한다. |
| 안내 종료 후 시작 | Play Mode | 보류된 Stage 또는 InfiniteMode가 한 번만 시작되고, 정상 Action Map·HUD로 전환되며 Start Run 입력이 Jump·Pause로 남지 않는지 검증한다. |
| 실제 표시·Focus | Play Mode | 재열람에서 최신 Binding, Back 복귀 선택, 자동 안내 Start Run 기본 Focus, Keyboard Navigate·Mouse Hover·Click 경로를 검증한다. |
| Scene 연결 | 정적 YAML 및 Play Mode 구성 Test | Step 4 확정 편집표 이후 Root·Component·직렬화 참조·OnClick·Navigation·초기 활성·Raycast를 검증한다. |
| 기존 흐름 | 기존 및 확장 Edit/Play Mode | Menu·두 Mode·Pause·Settings·Rebind·Result·Retry·Main Menu 회귀를 검증한다. |

### 사용자 수동 작업

없음. 이 Step은 문서와 자동 검증 명세 작성·대조만 수행하며 Unity Editor, Scene 편집, Test Runner와 Build 실행을 요구하지 않는다.

### 완료 조건

- [x] 확정 규칙의 상태 전이와 정상·거부·경계 사례가 Test 명세에 연결됐다.

## Step 3. 순수 상태와 Unit Test를 구현한다

### AI 작업

- 승인된 안내 상태와 시작 보류를 기존 `GameNavigationState`에 구현했다. 자동 안내는 `AutomaticHowToPlay`, 유일한 선택 항목은 `StartRun`으로 표현한다.
- `GameNavigationState`가 자동 안내 처리 여부와 보류 Mode를 단일 책임으로 소유하게 하고, Start Run·Cancel·초기화 실패·재열람 경계를 구현했다.
- Scene·프레임에 의존하지 않는 Edit Mode Test로 Step 2 상태 표를 검증하는 Test를 작성했다.
- Binding 표시의 순수 변환은 Unit Test로, 실제 Action/Override 조회는 Input System 경계 Test로 구분한다. 기본 키 문자열을 정답 원천으로 하드코딩하지 않는다.
- 기존 `GameNavigationStateTests`에서 Mode 선택 즉시 시작을 전제로 한 Test를 새 계약에 맞게 조정한다. 최초 안내를 우회하는 테스트 편의 경로로 생산 흐름 검증을 대체하지 않는다.
- 기존 Settings 재지정 범위와 실제 Gamepad Binding을 보존한다.

### 사용자 수동 작업

없음. 이 Step은 Scene·Unity Editor·Build를 사용하지 않는다. Step 5에서 지정할 집중 Edit Mode Fixture를 Unity Test Runner로 실행하기 전에는 수동 작업이 필요 없다.

### 완료 조건

- [x] 상태·횟수·취소·중복·실패 경계를 검증하는 Unit Test 코드가 작성되고 정적 검사를 통과했다.

## Step 4. 생산 연결·통합 Test와 단일 Scene 편집표를 준비한다

### AI 작업

- Menu에서 여는 안내와 Run 전 자동 안내를 생산 흐름에 연결하고, 안내 중에는 Run 생성을 보류했다.
- 실제 사용하는 Player/UI Action의 현재 Binding을 표시하며 Settings 변경·복원 후 재열람에도 반영하도록 구현했다. 마지막 UI 입력이 Keyboard 또는 Mouse이면 Keyboard Binding을, Gamepad이면 Gamepad Binding을 표시한다.
- 생산 Scene 통합 Test를 작성하고 기존 `GameLifecycleIntegrationTests`, `ModeUISceneConfigurationTests`, Settings Test 등의 시작 전제를 점검했다.
- 입력 Test는 가상 장치에서 누름·유지·뗌을 수행하고 상태 조건과 최대 대기 시간으로 결과를 판정한다. 장치·Input 설정·Callback·Scene 상태는 종료 시 복구한다. 단독 실행뿐 아니라 전체 실행 간 오염을 검증한다.
- 기존 Player 직렬화 속도를 사용한다. 안내 검증을 위해 속도·전역 시간 배율을 변경하지 않는다.
- 아래 단일 Scene 편집표를 작성했다. 기존 Settings/Pause의 다크 패널 스타일을 재사용하고 공용 본문과 진입 출처별 버튼 영역을 구분한다.

### 확정 Scene 편집표

`Assets/Scenes/SampleScene.unity`의 `UIRoot/MenuCanvas/HowToPlayPanel`만 편집한다. 기존 `MenuList`와 그 하위의 자리표시자 Text·BackButton은 제거하고, 아래 Object를 만든다. `HowToPlayPanel` Root의 Image는 유지한다.

| 경로 | 생성 타입·Component | RectTransform / 표시 | 초기 활성·Raycast·Navigation | 직렬화·이벤트 연결 |
|---|---|---|---|---|
| `HowToPlayPanel` | 기존 Image Root 유지 | Anchor Min `(0,0)`, Max `(1,1)`, Pivot `(0.5,0.5)`, Position `(0,0)`, Size `(0,0)`, 기존 검정 `#000000DD` 유지 | Root `false`, Image Raycast `true` | `UIManagementSystem._howToPlayPanel`에 기존 Root 유지 |
| `HowToPlayPanel/TitleText` | 기존 `TextMeshProUGUI` 재사용 | Anchor/Pivot `(0.5,1)`, Position `(0,-96)`, Size `(900,64)` | `true`, Raycast `false` | Text `HOW TO PLAY`, Font Size `40`, Center/Middle, Wrap `false`, 기존 제목 색상 유지 |
| `HowToPlayPanel/InstructionText` | 새 `TextMeshProUGUI` | Anchor/Pivot `(0.5,1)`, Position `(0,-184)`, Size `(920,330)` | `true`, Raycast `false` | Font Size `24`, Left/Top, Wrap `true`, Color `#F8FAFC`; 아래 공용 문구 사용 |
| `HowToPlayPanel/JumpBindingText` | 새 `TextMeshProUGUI` | Anchor/Pivot `(0.5,1)`, Position `(0,-538)`, Size `(920,32)` | `true`, Raycast `false` | Font Size `24`, Center/Middle, Wrap `false`, Color `#F8FAFC`, 초기 Text `Jump: Unassigned`; `UIManagementSystem._howToPlayJumpBindingText` 연결 |
| `HowToPlayPanel/MomentumLandingBindingText` | 새 `TextMeshProUGUI` | Anchor/Pivot `(0.5,1)`, Position `(0,-582)`, Size `(920,32)` | `true`, Raycast `false` | Font Size `24`, Center/Middle, Wrap `false`, Color `#F8FAFC`, 초기 Text `Momentum Landing: Unassigned`; `UIManagementSystem._howToPlayMomentumLandingBindingText` 연결 |
| `HowToPlayPanel/PauseBindingText` | 새 `TextMeshProUGUI` | Anchor/Pivot `(0.5,1)`, Position `(0,-626)`, Size `(920,32)` | `true`, Raycast `false` | Font Size `24`, Center/Middle, Wrap `false`, Color `#F8FAFC`, 초기 Text `Pause: Unassigned`; `UIManagementSystem._howToPlayPauseBindingText` 연결 |
| `HowToPlayPanel/StartRunButton` | 새 Image + Button + 자식 `TextMeshProUGUI` | Button Anchor/Pivot `(0.5,1)`, Position `(0,-716)`, Size `(360,52)`; 자식 Text Stretch, Position `(0,0)`, Size `(0,0)` | Button Object `true`, Image Raycast `true`, Button Navigation `None` | Button Text `START RUN`, Text Font Size `24`, Center/Middle, Color `#F8FAFC`; `UIManagementSystem._howToPlayStartRunButton` 연결; OnClick Persistent: `GameSystem.SelectStartRun()` |
| `HowToPlayPanel/BackButton` | 새 Image + Button + 자식 `TextMeshProUGUI` | Button Anchor/Pivot `(0.5,1)`, Position `(0,-716)`, Size `(360,52)`; 자식 Text Stretch, Position `(0,0)`, Size `(0,0)` | Button Object `true`, Image Raycast `true`, Button Navigation `None` | Button Text `BACK`, Text Font Size `24`, Center/Middle, Color `#F8FAFC`; `UIManagementSystem._howToPlayBackButton` 연결; OnClick Persistent: `GameSystem.SelectBack()` |

공용 `InstructionText` 문구는 아래처럼 입력한다. 속도·속도 유지·속도 증가 표현은 사용하지 않는다.

```text
Your character moves automatically.

Use Jump to clear terrain and obstacles.
Press Momentum Landing just before landing to perform the action.
Collectibles add to your score.
Use Pause to pause the game.

Stage Mode: finish the stage as quickly as you can.
Infinite Mode: consecutive Momentum Landing successes increase the distance score multiplier.
```

`UIManagementSystem` Component에는 새 Text·Button 참조 외에도 동일 Scene의 `PlayerInputSystem` Component를 `_playerInputSystem`에, `UIInputSystem` Component를 `_uiInputSystem`에 연결한다. 자동 안내에서는 Runtime이 `StartRunButton`만 활성화하고 기본 선택하며, Main Menu 재열람에서는 `BackButton`만 활성화하고 기본 선택한다. 두 버튼은 같은 위치를 사용하므로 동시에 활성화하지 않는다.

### 사용자 수동 작업

없음. Step 6 전에는 Scene을 수정하지 않는다. 위 편집표는 Step 6에서 그대로 수행할 작업 방법이다.

### 완료 조건

- [x] 생산 연결과 통합 Test가 작성됐다.
- [x] 실제 API와 일치하는 Scene 편집표가 본 문서에 추가됐다. 이 조건 전에는 Step 6을 시작하지 않는다.

## Step 5. Scene 편집 전 컴파일과 Unit Test를 확인한다

### AI 작업

- C# 참조·asmdef·입력 Asset/Wrapper 일치, 직렬화 필드, 문서 계약 및 diff 공백 오류를 정적으로 검사한다.
- 사용자에게 Scene 편집 없이 실행 가능한 집중 Edit Mode Fixture의 실제 이름을 제공한다. Test 개수는 구현 후 산정하고 예상과 실제 실행 결과를 구분한다.

### AI 정적 검사 결과

- `Features`와 `EditModeTests`는 별도 `.asmdef`를 사용한다. `HowToPlayBindingFormatter`와 해당 Unit Test의 `InputAction` 사용에 맞춰 두 Assembly에 `Unity.InputSystem` 참조를 추가했다. Core enum은 기존 Core Assembly에 유지한다.
- `UIManagementSystem`의 새 직렬화 필드와 `PlayerInputSystem.TryGetPlayerAction`, `UIInputSystem.TryGetUIAction`, 마지막 입력 장치 기록, `HowToPlayBindingFormatter` 호출을 대조했다. Jump·Momentum Landing·Pause 표기 원천은 각각 `Player/Jump`·`Player/MomentumLanding`·`UI/Cancel`이다.
- 입력 Asset에서 Jump와 Momentum Landing의 Keyboard·Gamepad Binding을 확인했다. Mouse 입력은 `UIInputSystem`에서 KeyboardMouse로 기록되어 Keyboard Binding 표기로 전환된다.
- 자동 안내의 단일 실행·Cancel 무반응·Start Run 완료 기록·재열람 Back·초기화 실패 후 완료 상태 유지와, Momentum Landing 문구에서 속도 표현을 제거한 문서 계약을 코드 및 Test와 대조했다.
- `git diff --check`는 공백 오류를 보고하지 않았다. 출력된 LF/CRLF 메시지는 작업 트리 줄바꿈 변환 경고이며 diff 오류가 아니다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료 후 예상하지 않은 Error/Warning이 없는지 확인한다.
2. Test Runner의 EditMode에서 다음 집중 Fixture만 실행한다. 현재 소스 기준 예상 Test 수는 총 26개이며, 실제 실행 수는 Test Runner 결과를 기준으로 기록한다.
   - `FlowState.Tests.EditMode.GameNavigationStateTests` (23개)
   - `FlowState.Tests.EditMode.HowToPlayBindingFormatterTests` (3개)
3. 실행·성공·실패 개수와 오류 로그를 전달한다. 실패 시 AI 수정 후 해당 검증을 다시 수행한다.

### 완료 조건

- [x] 컴파일과 집중 Unit Test가 성공하고 예상하지 않은 Error/Warning이 없다.

## Step 6. How To Play Scene UI를 구성한다

### AI 작업

- Step 4 편집표와 실제 코드의 일치를 재확인한다. 기존 Canvas와 EventSystem을 재사용하는 구성을 우선한다.

### AI 사전 대조 결과

- `UIManagementSystem`의 `_howToPlayPanel`, `_howToPlayBackButton`, `_howToPlayStartRunButton`, 세 Binding Text, `_playerInputSystem`, `_uiInputSystem` 필드와 편집표의 연결 대상을 대조했다.
- `GameSystem.SelectStartRun()`과 `GameSystem.SelectBack()`은 Inspector Button `OnClick`에 연결하는 parameterless public 메서드임을 확인했다. Dynamic 인자는 사용하지 않는다.
- 자동 안내는 Start Run만, Main Menu 재열람은 Back만 활성화하며 코드가 각 화면의 Focus를 직접 지정한다. 두 Button의 Inspector Navigation은 `None`으로 유지한다.
- 현재 Scene에는 새 How To Play Button·Binding Text와 `UIManagementSystem` 참조가 저장돼 있고, Start Run·Back의 OnClick도 각각 `GameSystem.SelectStartRun()`·`GameSystem.SelectBack()`에 연결돼 있다.
- `InstructionText`와 세 Binding Text의 Pivot은 현재 `(0.5, 0.5)`로 저장돼 있어 편집표의 상단 기준 배치와 다르다. 각 Pivot을 `(0.5, 1)`로 수정해야 한다.
- Scene YAML의 `InstructionText` single-quoted 다중 행 표기는 Unity의 문장 접기 저장 형식이며 Inspector의 강제 줄바꿈을 뜻하지 않는다. Inspector에서 Step 4 공용 문구와 문단 구분이 보이면 추가 수정하지 않는다.
- 코드에서 제거한 `_quitButton`, `_pauseQuitButton`은 Inspector에 표시되지 않는다. YAML에 남은 null 항목은 현재 Component가 읽지 않는 잔존 직렬화 데이터이므로 Step 6 완료 조건의 불일치가 아니다. AI는 Scene YAML을 수정하지 않는다.

### 사용자 수동 작업

1. Play Mode를 종료하고 `Assets/Scenes/SampleScene.unity`를 연다.
2. `UIRoot/MenuCanvas/HowToPlayPanel`의 기존 자리표시자 내용을 Step 4 편집표의 안내 UI로 교체한다.
3. 자동 이동·Jump·Momentum Landing·Collectible·Pause 및 Mode별 목표 안내와 Binding 표시 요소를 생성한다.
4. 확정된 진입 출처별 구성에 따라 자동 안내에는 Start Run만, Main Menu 재열람에는 Back만 생성한다.
5. `InstructionText`와 세 Binding Text의 Pivot을 `(0.5, 1)`로 설정한다. `InstructionText`는 Position `(0, -184)`, Size `(920, 330)`, Left/Top 정렬을 사용한다.
6. 편집표의 나머지 RectTransform·색상·Text·초기 활성·Raycast·Navigation 값을 적용한다. 표시되지 않는 버튼으로 Focus가 이동하지 않게 출처별 설정을 따른다.
7. 실제 구현된 Component를 붙이고 직렬화 참조와 OnClick을 연결한다. Start Run은 `GameSystem.SelectStartRun()`, Back은 `GameSystem.SelectBack()`의 parameterless 항목을 선택하며 Dynamic 인자는 사용하지 않는다.
8. Unity Script Compilation 후 Scene을 저장하고 완료 사실을 전달한다.

### 완료 조건

- [x] 확정 편집표의 UI와 Inspector 연결이 사용자에 의해 저장됐다.

## Step 7. 저장한 Scene과 입력 연결을 정적으로 검사한다

### AI 작업

- Scene YAML의 fileID/GUID·Component·직렬화 참조·이벤트·Navigation·초기 상태·누락 Script를 대조한다.
- 공용 안내의 출처별 버튼·Focus 경로와 실제 Action 표시 원천을 검사한다.
- Test는 장식용 Object 이름 대신 직렬화 참조·Component·소속·행동을 검증한다. 색상 양자화 등 허용 가능한 오차와 실제 계약 위반을 구분한다.
- 정적으로 확인된 항목을 사용자에게 Inspector에서 재확인하도록 요구하지 않는다.

### 사용자 수동 작업

정적 불일치가 있을 때만 AI가 지정한 Object·Component·Field를 지정값으로 수정하고 저장한다. 불일치가 없으면 수동 작업은 없다.

### AI 정적 검사 결과

- `HowToPlayPanel`은 초기 비활성 상태이며 Canvas 아래의 전체 화면 Image와 7개 자식(제목·안내 문구·세 Binding Text·Start Run·Back)으로 구성되어 있다. 안내 문구와 Binding Text의 상위는 모두 이 Panel이다.
- `UIManagementSystem`의 `_howToPlayPanel`, 두 Button, 세 Binding Text, `PlayerInputSystem`, `UIInputSystem` 직렬화 참조는 모두 유효한 Component를 가리킨다. 제거된 `_quitButton`, `_pauseQuitButton`의 YAML null 잔재는 현재 Component의 직렬화 필드가 아니므로 검사 대상이 아니다.
- Start Run과 Back은 모두 Navigation `None`이며, Persistent OnClick 하나씩을 통해 같은 `GameSystem`의 parameterless `SelectStartRun()` 및 `SelectBack()`을 호출한다.
- 세 Binding Text는 Raycast Target이 꺼져 있고, Panel 전환·출처별 Button 표시·Focus 선택과 마지막 Keyboard/Mouse·Gamepad 입력별 표기는 `UIManagementSystem`과 `GameSystem`의 현재 계약과 일치한다.
- `HowToPlayIntegrationTests`에 직렬화 참조·상위 Panel·입력 System·Button Navigation·OnClick 대상/메서드·Binding Text Raycast 설정을 검증하는 Play Mode Test를 추가했다. Test Runner는 실행하지 않았다.

### 완료 조건

- [x] Scene 연결과 승인된 UI 구성이 정적 검사에서 일치한다.

## Step 8. 전체 자동 Test 회귀를 실행한다

### AI 작업

- 최종 변경 기준 집중 Fixture와 전체 회귀 목록을 제공하고 실패 원인을 코드·Test·Scene으로 구분한다.
- 최초 안내 완료·Cancel 후 재선택, 두 Mode, 재열람과 Binding 변경·복원, 입력 잔류 및 시작 실패를 자동 검증에 포함한다.
- Scene 수정은 사용자에게 정확한 변경값을 제공하고 코드·Test 수정은 AI가 처리한다.

### AI 정적 검사 결과

- 집중 Edit Mode Fixture는 `FlowState.Tests.EditMode.GameNavigationStateTests`, `FlowState.Tests.EditMode.HowToPlayBindingFormatterTests`다. 전자는 최초 자동 안내·Cancel 무반응·Stage/Infinite·완료 상태 유지·재열람·초기화 실패를, 후자는 Keyboard/Mouse·Gamepad별 Binding 선택과 미할당 표기를 검증한다.
- 집중 Play Mode Fixture는 `FlowState.Tests.PlayMode.HowToPlayIntegrationTests`, `FlowState.Tests.PlayMode.UIInputSystemTests`, `FlowState.Tests.PlayMode.SettingsInteractiveRebindTests`다. 각각 Scene 직렬화 계약·자동 안내 시작·Binding 변경/복원, Keyboard·Gamepad·Mouse 장치별 표시 장치 기록 매핑, 실제 Settings 재바인딩·기본값 복원을 검증한다.
- `HowToPlayIntegrationTests`에 Jump Keyboard Binding 변경 후 안내 문구 갱신과 Override 제거 후 기본 표기 복원을 검증하는 Test를 추가했다. `UIInputSystemTests`에는 가상 Keyboard·Gamepad·Mouse Device를 `RecordInputDevice`에 적용했을 때 `LastInputDisplayDevice`가 Keyboard/Mouse·Gamepad·Keyboard/Mouse 순서로 바뀌는 Test를 추가했다. 실제 UI Action callback·장치 조작은 Step 9의 실제 장치 확인 범위다.
- `GameNavigationStateTests`의 `[Test]` 23개와 `[TestCase]` 6개, Binding Formatter의 4개, How To Play 통합의 3개 Unity Test, UI Input의 4개 Test, Settings 재바인딩의 1개 Unity Test를 정적으로 확인했다. Test Runner가 실제 표시하는 발견·실행 개수와 Error/Warning을 결과 기준으로 기록한다.
- 이번 Step에서 Scene YAML은 변경하지 않았으며, 집중 Fixture의 참조 Assembly(`FlowState.Runtime.Core`, `FlowState.Runtime.Features`, `Unity.TextMeshPro`, `Unity.InputSystem`)와 C# 괄호 균형·diff 공백 오류를 정적으로 대조했다.
- 전체 Play Mode 실행에서 보고된 111개 실패는 공용 `ProductionSceneGameModeTestUtility.RestartInMode()`가 Mode 선택 직후 Playing을 가정한 공통 원인이다. 유틸리티가 최초 Mode 선택의 `AutomaticHowToPlay` 화면을 감지하면 `StartRun`을 한 번 선택하도록 수정했다. 이는 실제 사용자 진입 계약을 따르며, 해당 유틸리티를 사용하는 16개 Fixture의 기존 Setup 전제를 복구한다. 수정 후 Test Runner 결과는 아직 없다.
- `InfiniteModeIntegrationTests`는 공용 유틸리티를 사용하지 않고 첫 Run에 `StartGame()`을 직접 호출해 동일한 전제를 재도입하고 있었다. Setup을 `RestartInMode(Infinite)`로 통일했다. 이후 재시작 `StartGame()` 호출은 이미 자동 안내 완료 상태에서 실행되므로 변경하지 않았다.
- `GameEntryBootIntegrationTests`도 Mode Submit 직후 Playing을 기대하고 있었다. 이 Test는 자동 안내 화면·Runtime Data 미생성 상태를 먼저 확인한 다음 `StartRun`을 선택하도록 바꿨고, 같은 Fixture의 Pause 시작은 공용 Start Mode helper로 통일했다.
- `PausePanelSceneConfigurationTests`는 `SetGameState`와 `SetUIState`만 직접 호출해 Pause Focus를 검증하고 있었다. Focus는 현재 `SetNavigationScreen(Pause, Resume)`가 담당하므로, 테스트를 이 실제 화면·선택 경로로 변경했다.
- `SettingsInteractiveRebindTests`의 재바인딩 후 Player Action 상태 확인은 Play Mode 프레임 순서에 의존해 간헐적으로 실패했다. 해당 Action이 enable 상태에서 재바인딩된 대상 control을 해석하는지로 검증을 한정했다. 대화형 재바인딩 입력은 bitfield Gamepad Button도 지원하는 전체 Keyboard/Gamepad 상태 이벤트를 유지한다.
- 같은 Fixture의 UI EventSystem 선택 이동 확인은 다른 Fixture와 함께 실행할 때 잔류 입력 프레임 상태에 의존했다. Settings 재바인딩 범위를 벗어나고 별도 Navigation 검증과 중복되므로 제거했다. 재바인딩 성공·충돌 거부·Binding 표기·기본값 복원 검증은 유지한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료 후 예상하지 않은 Error/Warning 유무를 확인한다.
2. Test Runner에서 다음 집중 Edit Mode Fixture를 실행한다.
   - `FlowState.Tests.EditMode.GameNavigationStateTests`
   - `FlowState.Tests.EditMode.HowToPlayBindingFormatterTests`
3. 이어서 다음 집중 Play Mode Fixture를 실행한다.
   - `FlowState.Tests.PlayMode.HowToPlayIntegrationTests`
   - `FlowState.Tests.PlayMode.UIInputSystemTests`
   - `FlowState.Tests.PlayMode.SettingsInteractiveRebindTests`
4. 집중 Fixture가 모두 통과하면 전체 Edit Mode와 전체 Play Mode를 실행한다.
5. 각 실행의 발견·성공·실패 개수와 예상하지 않은 Error/Warning을 전달한다. 실패하면 Test 이름·메시지·Stack Trace를 함께 전달한다.

### 최종 실행 결과

- 초기 실행에서 Unity Script Compilation 성공, 전체 Edit Mode 691개 성공, 전체 Play Mode 229개 성공과 예상하지 않은 Error/Warning 부재를 확인했다.
- Pause Cancel Binding 분리, Formatter Test 추가, Input System UI Module Asset 참조 복구 후 최신 변경 기준으로 Unity Script Compilation 성공과 예상하지 않은 Error/Warning 부재를 다시 확인했다.
- 최신 변경 기준 전체 Edit Mode Test 692개와 전체 Play Mode Test 229개를 실행하여 모두 성공했고, 각 실행에서 예상하지 않은 Error/Warning이 없었다.
- 전체 실행은 집중 Fixture를 포함하므로, 실행하지 않은 Test를 통과로 기록하지 않았다.

### 완료 조건

- [x] 최종 코드·Scene 기준 집중 및 전체 Test가 성공했다.
- [x] 예상하지 않은 Error/Warning이 없다. 실행하지 않은 Test를 통과로 기록하지 않았다.

## Step 9. 안내의 이해도·가독성과 Player Build를 확인한다

### AI 작업

- 수동 대상은 문구 이해도, 화면 배치, Focus 가시성, 실제 입력 조작감과 Player Build 표현으로 제한한다.
- State·Mode·횟수·Binding 값·진행 차단은 Step 8 자동 Test 결과로 판정한다. 사용자에게 정밀 타이밍 조작이나 디버거 조작을 요구하지 않는다.

### AI 정적 검사 결과

- Step 8에서 최신 전체 Edit Mode 692개와 Play Mode 229개가 성공했으므로, 자동 안내 횟수·Cancel 무반응·Start Run 진행 차단·두 Mode의 상태 전이·Binding 변경/복원 값은 재수동 검증 대상에서 제외한다.
- `HowToPlayPanel`은 ScrollRect가 없는 단일 화면이다. 자동 안내는 Start Run만, Main Menu 재열람은 Back만 활성화하고 각각 해당 Button을 Focus한다. 따라서 스크롤·휠 검증은 필요 없다.
- Keyboard와 Mouse는 Keyboard Binding 표기로, Gamepad는 Gamepad Binding 표기로 기록된다. 실제 장치의 표기·Focus·Submit·Cancel 감각만 확인한다.
- Infinite Difficulty Text는 Scene 설정 `_showDifficultyInDevelopment: 1`일 때 Editor·Development Player에서 표시하고, Development Build가 아닌 Player에서는 컴파일 분기로 항상 숨긴다. 일반 Player에서는 Infinite Mode 진입 후 이 Text가 보이지 않는지만 확인하면 된다.
- 화면 문제가 발견되면 사용한 해상도·진입 방식·겹치거나 잘린 Object·재현 절차를 보고한다. 아래 ContentCard 외의 Scene 변경은 하지 않는다.
- 이전 재열람 화면 캡처에서 안내 Text와 Binding이 게임 오브젝트·지형과 겹치고 `Pause: Unassigned`가 표시되는 결함을 확인했다. `UI/Cancel`을 Keyboard Escape와 Gamepad East Button Binding으로 분리했고, 해당 Device별 Formatter Unit Test를 추가했다. 당시 생산 `EventSystem/Input System UI Input Module`은 현재 `Assets/InputSystem_Actions.inputactions`의 GUID `052faaac586de48259a63d0c4782560b`가 아닌, Assets에서 해석되지 않는 이전 GUID `ca9f5fa95ffab41fb9a615ab714db018`을 참조했다. 따라서 수정한 Cancel Binding이 런타임 모듈에 연결되지 않아 `Unassigned`가 계속 표시됐다.
- 최신 Scene 정적 대조에서 `ContentCard`는 `HowToPlayPanel`의 첫 번째 Sibling이며, 지정한 Anchor/Pivot `(0.5,1)`, Position `(0,-48)`, Size `(1040,760)`, 색상 `#0B1220` Alpha `0.95`, Raycast 비활성 상태로 저장된 것을 확인했다. EventSystem의 Actions Asset 및 Point·Navigate·Submit·Cancel을 포함한 UI Action Reference와 Project Settings의 Input Actions Asset은 모두 GUID `052faaac586de48259a63d0c4782560b`을 참조한다. 이전 GUID 참조는 Assets에 남아 있지 않다.
- 활성 Build Scene은 `Assets/Scenes/SampleScene.unity` 하나이며 GUID가 Scene meta와 일치한다. `UIManagementSystem`은 `UNITY_EDITOR || DEVELOPMENT_BUILD`에서만 `_showDifficultyInDevelopment` 값을 적용하고, 일반 Player Build에서는 Infinite Difficulty Text를 항상 비활성화한다.

### 사용자 수동 작업

ContentCard와 EventSystem Input Actions 연결은 이미 저장됐고 정적 대조도 완료됐다. 추가 Scene 작업은 없다. 다음 실제 화면·장치·Build 검증만 수행한다.

1. 새 Play 세션에서 Main Menu → How To Play을 열고, 카드 위에서 행동 문구와 Stage/Infinite 목표를 읽을 수 있는지 확인한 뒤 Back으로 복귀한다. Keyboard 또는 Mouse를 마지막으로 입력한 상태에서 Pause Binding이 `Pause: Escape`로 표시되는지 확인한다.
2. 별도 새 Play 세션에서 첫 Mode 선택 후 자동 안내를 열고, 카드·Start Run·문구·Focus가 읽기 쉬운지와 Pause Binding이 `Unassigned`가 아닌지 확인한다. Cancel, Back, Skip의 상태 전이는 자동 Test 결과를 사용한다.
3. Settings에서 Jump 또는 Momentum Landing Binding을 변경한 뒤 안내를 다시 열어 변경된 표기가 읽기 쉬운지만 확인한다. Restore Defaults 후 기본 표기도 읽기 쉬운지 확인한다.
4. Keyboard와 Mouse로 안내 진입·Back·Hover·Click을 수행해 Focus가 명확한지와 안내에서 정상 복귀하는지만 확인한다.
5. `1920×1080`, `1280×720`, 최소 한 개의 비 16:9 Game View에서 자동 안내와 재열람 안내, Main Menu·Settings·Pause·Stage/Infinite Result에 잘림·겹침이 없는지 확인한다. 사용한 비 16:9 해상도를 기록한다.
6. Gamepad 보유 시 실제 장치로 Binding 표기·Focus·Submit·Cancel 조작감을 확인한다. 없으면 “Gamepad 없음”과 검증 제외 사유를 기록한다.
7. Unity Editor에서 Development Player Build와 Development Build를 끈 일반 Player Build를 각각 만들고 실행한다. 두 Build에서 안내 진입·재열람·Start Run·Settings 변경 표기·Stage/Infinite 진입을 확인한다. 일반 Player에서는 Infinite Difficulty Text가 보이지 않는지 확인한다.
8. 사용한 해상도·장치 또는 제외 사유·두 Build 결과·화면 문제·예상하지 않은 Error/Warning을 전달한다.

### 완료 조건

- [x] 안내를 읽고 조작·목표를 이해할 수 있으며 필요한 화면비에서 잘림과 Focus 문제가 없다.
- [x] 두 Player Build의 안내와 기본 흐름이 정상이며 일반 Build의 개발자 UI가 숨겨진다.
- [x] 실제 장치 검증 결과와 승인된 제외 사유가 기록됐다.

### 수행 결과

- Keyboard에서 Pause Binding이 `Esc`로 표시되는 것을 확인했다.
- Gamepad는 사용 가능한 장치가 없어 Binding 표기·Focus·Submit·Cancel 실기기 검증에서 제외한다. 사용자가 이 제외를 승인했다.
- `16:9` Game View에서 How To Play과 관련 UI의 가독성·배치가 적절함을 확인했다. 비 `16:9` 해상도 검증은 사용자 승인으로 제외한다.
- Development Player Build와 일반 Player Build를 확인했고 안내와 기본 흐름이 적절했다. 일반 Build의 개발자 UI 숨김도 완료 조건에 따라 확인했다.

## Step 10. Phase 4 결과와 Roadmap 006 완료 근거를 기록한다

### AI 작업

- Roadmap 완료 조건마다 정적 검사·Unit Test·통합 Test·수동 화면·Build 결과를 연결한다.
- 실제 Test 개수, Scene 변경, 화면비, 장치 제외, Build 종류 및 남은 관찰 사항을 기록한다.
- 모든 필수 근거 충족 시 Phase 4 완료와 Roadmap 006 진행 상태를 갱신한다. 영구 저장·실제 Leaderboard를 완료에 포함하지 않는다.

### 사용자 수동 작업

없음. 필수 근거가 모두 기록됐으므로 추가 Unity Editor, Scene, Test Runner 또는 Build 작업은 필요 없다.

### 완료 조건

- [x] Phase 4 및 Roadmap 006의 완료 근거와 후속 범위가 문서화됐다.

### 완료 근거

| Roadmap 006 Phase 4 완료 조건 | 근거 |
|---|---|
| 핵심 조작과 Score 목적 이해 | Step 1의 확정 행동 중심 문구와 Step 9의 How To Play 가독성·배치 확인. Momentum Landing은 속도 효과가 아닌 InfiniteMode 거리 Score 배율 이점만 안내한다. |
| 현재 Binding 표시 | `HowToPlayBindingFormatter`의 Keyboard·Mouse·Gamepad 및 Cancel Binding Formatter Test, `UIInputSystemTests`의 마지막 입력 장치 전환 Test, `HowToPlayIntegrationTests`의 표시·Override·복원 Test와 Step 8 전체 Test 성공. Step 9에서 Keyboard Pause가 `Esc`로 표시됨을 실기기 확인했다. |
| 안내 중 진행 차단과 Start Run | `GameNavigationStateTests` 및 `HowToPlayIntegrationTests`가 자동 안내의 보류 Mode, Cancel 무반응, Run 미생성, Start Run 단일 시작과 초기화 실패 경계를 검증했고 Step 8 전체 회귀가 성공했다. |
| 재열람과 전체 UI 흐름 | Main Menu 재열람 Back 경로, Stage·InfiniteMode·Pause·Result 회귀를 Edit/Play Mode Test와 Step 9 화면 확인으로 검증했다. |
| Scene·입력 Asset 연결 | Step 7 정적 검사와 Step 9 재검사에서 ContentCard, EventSystem UI Action Reference, Project Settings Input Actions Asset 및 활성 Build Scene을 대조했다. |
| Build·실제 화면 | 사용자가 Development Player Build와 일반 Player Build의 안내·기본 흐름을 확인했고, 일반 Build에서 Difficulty UI가 숨겨짐을 확인했다. 16:9 화면의 가독성·배치도 확인했다. |

승인된 검증 제외는 Gamepad 실기기 미보유과 비 16:9 화면이다. Gamepad의 Binding 분류·표시 계약은 자동 Test로 검증했으며, 실제 장치 체감 검증만 제외했다. Tutorial 완료 영구 저장, Settings 영구 저장과 실제 Leaderboard는 Roadmap 7 범위로 남긴다.

# 영향 범위

이번 변경은 Task 문서 추가다. 향후 Phase 4 수행 시 관련 Feature·System 문서, Runtime 코드, Test, 사용자 Scene과 Roadmap이 변경될 수 있다. Rules·Package·ProjectSettings 변경은 계획에 포함하지 않는다.

# 검증 내용

- Roadmap 006 Phase 4의 구현 대상과 완료 조건을 Step 1~10에 배치했다.
- 상태·횟수·현재 Binding·진행 차단·Scene 연결을 정적 검사 또는 자동 Test 대상으로 명시했다.
- 수동 작업을 제품 선택, Scene 편집, Unity 검증 실행, 실제 화면·장치·Build 확인으로 제한했다.
- 기존 미완성 How To Play 경로와 Phase 3 검증 결과를 구분하고 구현 전인 항목을 완료 처리하지 않았다.

# 검증 결과

Step 1의 제품 규칙 확정, Step 2의 상태 계약·자동 검증 명세 작성, Step 3의 순수 Navigation 상태와 Edit Mode Unit Test 코드 작성, Step 4의 생산 연결·통합 Test 코드·Scene 편집표 작성 및 문서·코드 정적 대조를 완료했다. Step 5의 C# 참조·입력 Asset·직렬화 필드·문서 계약·diff 정적 대조와 집중 Edit Mode Fixture 지정도 완료했다. 초기 검증에서 사용자는 Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 없음을 확인했고, Edit Mode Test 701개를 모두 성공했다. Step 6의 확정 편집표 UI와 Inspector 연결은 저장 완료됐다. 이후 YAML 대조에서 보인 InstructionText 다중 행 표기는 Unity의 문장 접기 형식이고, 제거된 Quit 필드는 Inspector에 없는 비활성 잔존 직렬화 데이터임을 확인해 완료 판단에서 제외했다. Step 7의 Scene 정적 검사를 완료했다. Pause Cancel Binding을 명시 Binding으로 분리했고, EventSystem UI Input Module이 현재 `InputSystem_Actions` Asset과 모든 UI Action을 참조하도록 사용자가 Scene을 복구했다. Step 8 최종 검증에서 사용자는 최신 변경 기준 Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 없음을 확인했고, 전체 Edit Mode Test 692개와 전체 Play Mode Test 229개를 모두 성공했다. Step 9에서 사용자는 Keyboard Pause 표기 `Esc`, 16:9 화면의 가독성·배치, Development 및 일반 Player Build의 안내·기본 흐름을 확인했다. Gamepad와 비 16:9 화면은 장치 미보유 및 사용자 승인으로 제외했다. Step 10에서 위 근거와 Roadmap 006 Phase 4 완료 상태를 연결했다. Tutorial·Settings 영구 저장과 실제 Leaderboard는 Roadmap 7 범위로 남긴다. AI는 Unity 컴파일·Test Runner·Build를 실행하지 않았고 Scene은 수정하지 않았다.

# 후속 작업

Roadmap 7의 영구 저장 및 실제 Leaderboard 범위를 별도 계획으로 준비한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/03_Features/HowToPlay.md`
- `AI/03_Features/GameEntryNavigation.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_006.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_6/20260921_01_Phase3ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 작업을 실행 순서대로 Step에 작성했다.
- [x] AI 작업·사용자 수동 작업·완료 조건을 각 Step에서 구분했다.
- [x] 정적 검사와 Unit/통합 Test를 우선하고 수동 중복 검증을 줄였다.
- [x] 승인 전 제안과 현재 구현 사실을 구분했다.
- [x] Scene 편집표의 확정 시점과 편집 시작 조건을 명시했다.
