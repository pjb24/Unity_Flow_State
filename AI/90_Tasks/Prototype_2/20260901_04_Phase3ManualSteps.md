# 작업 정보

## 작업명

Prototype 2 Phase 3 Manual Steps

## 작업 일자

20260901

## 작업 담당자

AI, 사용자

## 작업 상태

진행 중

---

# 작업 목적

Prototype 2 Phase 3의 GamePause, Resume, Retry와 Quit 구현 순서를 정의한다.

정적 검증과 Unit Test로 판정할 수 있는 항목을 자동화하고 Unity Editor에서만 확인할 수 있는 작업만 수동 작업으로 분리한다.

---

# 작업 대상

- GamePause Feature
- Pause 상태 전환과 Runtime Data
- GameSystem Pause 흐름
- PlayerInputSystem과 UIInputSystem Action Map 전환
- TimerSystem Pause와 Resume
- Stage Mode와 InfiniteMode 진행 중단
- PausePanel
- Pause 상태의 Resume, Retry와 Quit
- Edit Mode 및 Play Mode Test
- 필요한 최소 Scene 연결

---

# 작업 전 상태

- Prototype 2 Phase 1과 Phase 2가 완료되었다.
- Unity Script Compilation 최종 확인 결과는 성공이다.
- Edit Mode 기준은 `145 Passed, 0 Failed`이다.
- Play Mode 기준은 `67 Passed, 0 Failed`이다.
- GamePause Feature 문서에는 기본 Pause와 Resume 규칙만 정의되어 있다.
- 현재 `E_GameState`와 `E_UIState`에는 Pause 상태가 없다.
- TimerSystem에는 Pause와 Resume API가 이미 존재한다.
- PlayerInputSystem과 UIInputSystem은 Action Map별 입력 활성 상태를 관리한다.
- 현재 생산 Scene에는 Phase 3 PausePanel 연결이 없다.

---

# 조사 내용

- GameSystem은 Playing 상태에서 Player Action Map을, Ended 상태에서 UI Action Map을 사용한다.
- GameSystem은 Retry 시 선택된 Game Mode로 Runtime Data와 관련 System을 새로 초기화한다.
- Stage Mode만 PlayTimer를 사용하고 InfiniteMode는 거리와 Score를 물리 갱신마다 기록한다.
- PlayerMovementSystem, InfiniteModeSystem과 StageSystem의 진행을 Pause 동안 중단할 연결이 필요하다.
- PlayerControllerSystem에 Pause 직전 Rigidbody 물리 상태를 보존하고 Resume 시 복원할 연결이 필요하다.
- UIInputState에는 Cancel 입력이 존재하지만 Pause 요청 입력의 소유자와 Action Map은 확정되지 않았다.
- Phase 3 Roadmap에는 PausePanel이 포함되지만 Phase 4에는 PausePanel UI 마무리가 별도 범위로 남아 있다.
- 상태값과 호출 횟수는 Test로 판정하고 화면 구성과 실제 조작 흐름만 수동으로 확인해야 한다.

---

# 작업 내용

아래 Step 순서로 Phase 3를 수행한다.

각 구현 Step에서는 실패하는 Test를 먼저 작성하고 최소 생산 코드를 구현한 뒤 관련 회귀 Test를 확인한다.

Unity Script Compilation과 Unity Test Runner 실행은 사용자가 Unity Editor에서 수행한다.

Scene 수정은 사용자가 수행하며 AI는 필요한 오브젝트, Component와 Inspector 연결 방법만 안내한다.

IDE 디버그 기능을 수동 검증 절차로 사용하지 않는다.

---

## Step 1. Phase 3 미정 규칙을 확정한다

- 진행 상태: **완료**

### 결정 항목

1. Pause 요청 입력과 기본 키를 결정한다.
2. Pause와 Resume 요청을 Player Action Map 또는 별도 공용 Action Map 중 어디에서 수집할지 결정한다.
3. Pause 시 `Time.timeScale`을 사용할지 System별 명시적 중단을 사용할지 결정한다.
4. Pause 상태를 `E_GameState`와 Runtime Data에 어떻게 표현할지 결정한다.
5. PausePanel의 Phase 3 최소 표시 범위와 Phase 4 마무리 범위를 구분한다.
6. PausePanel의 기본 선택 항목과 Keyboard·Mouse 입력 범위를 결정한다.
7. Resume, Retry와 Quit의 선택 및 확인 규칙을 결정한다.
8. Quit을 Editor와 Player 환경에서 어떻게 처리할지 결정한다.
9. Pause 중 허용하거나 차단할 입력과 중복 요청 규칙을 결정한다.
10. Stage Mode Timer와 InfiniteMode 거리·Score의 정지 기준을 결정한다.
11. Pause 직전 Rigidbody 속도와 이동 상태의 보존·복원 규칙을 결정한다.
12. Result, Initializing, Ready, Ending과 Ended 상태에서 Pause 요청 처리 규칙을 결정한다.

### 결정 결과

1. 키보드의 기본 Pause와 Resume 입력은 `Escape`를 사용한다.
2. 기존 UI Action Map의 Cancel을 Pause와 Resume 요청으로 사용한다. Playing에서는 Player와 UI Action Map을 활성화하고 Paused에서는 UI Action Map만 활성화한다.
3. `Time.timeScale`은 변경하지 않고 관련 System을 명시적으로 중단하고 재개한다.
4. Pause 상태는 `E_GameState.Paused`로 표현하고 Runtime Data의 Game State에 동일하게 반영한다. 별도의 Pause bool은 추가하지 않는다.
5. Phase 3의 PausePanel은 Pause 제목, Resume, Retry와 Quit 및 선택 식별에 필요한 최소 화면만 제공한다. 최종 레이아웃, 아트와 애니메이션은 Phase 4에서 수행한다.
6. PausePanel의 기본 선택은 Resume이다. Keyboard의 Navigate, Submit과 Cancel 및 Mouse의 Point와 Click을 지원한다.
7. Submit과 유효한 Click은 현재 선택을 한 번 즉시 실행한다. Cancel은 현재 선택과 관계없이 Resume을 한 번 실행한다. Phase 3에서는 별도 확인창을 추가하지 않는다.
8. Quit 요청은 하나의 실행 경로에서 처리한다. Player에서는 Application 종료를 요청하고 Editor에서는 Play Mode 종료를 요청한다.
9. Pause 중에는 PausePanel UI 입력만 허용하고 Player 입력은 차단한다. 상태 전환 시 transient 입력을 소비하며 하나의 입력으로 둘 이상의 실행 흐름을 시작하지 않는다.
10. Pause 전환이 승인된 시점부터 Resume 전까지 Stage Mode의 PlayTimer와 InfiniteMode의 이동 거리, Score, 진행 지속 시간, 추락 종료 판정 및 Map Pattern 재배치를 갱신하지 않는다.
11. Pause 직전의 Player 위치, Rigidbody 속도, 물리 상태와 이동 상태를 보존하고 Resume 시 복원한다. 종료용 `StopMovement`로 Pause를 처리하지 않는다.
12. Playing에서만 Pause를 허용하고 Paused에서만 Resume을 허용한다. None, Initializing, Ready, Ending과 Ended에서는 Pause를 거부한다. Pause와 Stage 종료가 동시에 확정되면 Stage 종료를 우선한다.

### 정적 검증

- 확정 규칙이 GamePause Feature와 관련 System 책임을 혼합하지 않는지 확인한다.
- Phase 4 UI 마무리, 저장, Leaderboard와 Prototype 3 기능이 포함되지 않는지 확인한다.

### 수동 작업

사용자가 제안별 장단점을 검토하고 미정 규칙을 선택한다. Unity Editor 작업은 없다.

### 완료 조건

- [x] 모든 미정 규칙이 확정되었다.
- [x] 관련 Feature와 System 문서에 확정 규칙이 반영되었다.

## Step 2. 현재 Pause 확장 지점과 회귀 기준을 정적으로 조사한다

- 진행 상태: **완료**

### 조사 대상

- GameSystem 상태 전환과 Action Map 정책
- GameRuntimeData의 Game State
- PlayerInputSystem과 UIInputSystem 입력 구조
- PlayerMovementSystem의 이동 중단·재개 API
- StageSystem과 InfiniteModeSystem의 진행 상태
- TimerSystem의 Pause·Resume 계약
- UIManagementSystem의 UI State와 선택 처리
- Result 상태의 Retry와 Quit 흐름
- SampleScene의 UI Root, EventSystem과 Serialized Reference

### 기존 계약 조사 결과

#### Game State와 Runtime Data

- `E_GameState`에는 None, Initializing, Ready, Playing, Ending과 Ended만 존재하며 Paused는 없다.
- `GameSystem`은 `_currentGameState`를 현재 상태로 관리하고 `GameRuntimeData.SetGameState`로 같은 값을 Runtime Data에 반영한다.
- `GameRuntimeData`에는 별도의 Pause bool이 없으므로 확정 규칙대로 Paused enum만 추가하여 상태를 공유할 수 있다.
- 현재 `GameSystem.Update`는 Ended 상태의 ResultMenu 입력만 처리한다. Playing의 Cancel과 Paused의 PausePanel 입력을 처리할 분기가 필요하다.
- `StartGame`은 Playing만 중복 시작으로 거부하므로 Pause Retry는 기존 Runtime Data를 먼저 정리한 뒤 같은 Mode의 기존 시작 흐름을 재사용해야 한다.
- `EndGame`은 Ending과 Ended만 중복 종료로 거부한다. Paused에서 Quit 또는 종료할 때도 동일한 정리 경로를 사용할 수 있도록 상태 전환 순서를 분리해야 한다.

#### Input과 Action Map

- `PlayerInputSystem`과 `UIInputSystem`은 각각 자신의 Action Map만 관리한다.
- 두 InputSystem의 Enable과 Disable은 입력 상태를 Reset하며 transient 입력 소비 API가 이미 존재한다.
- UI Action Map에는 Navigate, Submit, Cancel, Point와 Click이 존재하고 `UIInputState`가 모두 제공한다.
- Input Action Asset의 Cancel은 `*/{Cancel}`에 연결되어 있어 Keyboard Escape를 포함하는 기존 Cancel 입력을 재사용할 수 있다.
- 새 Input Action, 새 Action Map과 생성 Wrapper 변경은 필요하지 않다.
- 기존 `GameLifecycleIntegrationTests.PlayingState_DisablesUIInputAndKeepsStateEmpty`와 `AssertPlayingState`는 Playing에서 UI Action Map이 비활성이라고 기대하므로 확정 규칙과 직접 충돌한다.
- 기존 `PlayingState_IgnoresForcedUITransientInput`은 UI transient 입력을 Playing에서 소비하지 않는 현재 동작을 기대하므로 Cancel만 해석하고 나머지를 소비하는 새 정책에 맞게 변경해야 한다.

#### Player 이동과 물리

- `PlayerMovementSystem.FixedUpdate`는 `_isRunning`만으로 이동 계산을 차단한다.
- 기존 `StopMovement`는 이동 상태, 점프 진행 상태, 착지 상태와 Player Movement Runtime Data를 초기화하므로 Pause에 재사용할 수 없다.
- `PlayerControllerSystem.StopMovement`는 Rigidbody linearVelocity, angularVelocity와 가속도를 0으로 만들며 복원 데이터를 보존하지 않는다.
- Pause 전용 이동 계산 중단·재개 API와 Rigidbody 상태 보존·복원 API가 각각 필요하다.
- Pause 상태의 단일 소유자는 Game State로 유지하고 각 System의 중단 값은 자신의 Update 또는 물리 적용을 차단하기 위한 내부 실행 상태로만 사용해야 한다.

#### Stage와 InfiniteMode

- `StageSystem`은 `_isPlaying`과 `_hasEnded`로 Goal 및 Infinite 종료 요청을 판정하지만 일시 중단 상태는 없다.
- `StageGoal` Trigger가 Pause 중 Stage 종료를 확정하지 않도록 StageSystem의 종료 판정 차단이 필요하다.
- `InfiniteModeSystem.FixedUpdate`는 거리·Score, 저속 진행 시간과 추락 종료 판정을 연속 수행하며 일시 중단 상태는 없다.
- `InfiniteMapPattern`은 `InfinitePatternBoundary.OnTriggerEnter`가 호출한 `TryAdvance`에서 즉시 Pattern을 재배치하며 Pause 차단 계약이 없다.
- InfiniteModeSystem의 Pause/Resume 경로에서 InfiniteMapPattern의 재배치 허용 상태도 함께 전환하면 GameSystem에 Feature 직접 참조를 추가하지 않고 기존 실행 연결을 유지할 수 있다.

#### Timer

- `TimerSystem`에는 `PauseTimer`와 `ResumeTimer`가 이미 존재한다.
- `TimerRuntimeData`는 Pause 기간을 경과 시간에서 제외하며 관련 Edit Mode Test가 존재한다.
- Stage Mode의 PlayTimer만 GameSystem에서 Pause/Resume하면 되며 TimerSystem 생산 코드의 신규 API는 필요하지 않다.
- InfiniteMode는 PlayTimer를 생성하지 않으므로 Timer Pause 요청 대상이 아니다.

#### UI와 종료 흐름

- `E_UIState`에는 None, StageHud와 Result만 존재한다.
- `UIManagementSystem`은 StageHUD와 ResultPanel, Retry와 Quit Button만 Serialized Field로 가진다.
- ResultMenu 선택은 `E_ResultMenuSelection`으로 관리되므로 PausePanel에는 독립적인 `E_PauseMenuSelection`이 필요하다.
- ResultMenu Quit은 `GameSystem.ExecuteResultMenuSelection`에서 `Application.Quit`을 직접 호출한다. Pause Quit과 Result Quit이 공유하는 단일 종료 요청 메서드로 모아야 한다.
- ResultMenu의 Cancel 무동작 규칙은 유지하고 Paused 상태에서만 Cancel을 Resume으로 해석해야 한다.

### 신규 파일 후보

| 파일 | 목적 |
|---|---|
| `Assets/Scripts/Runtime/Core/GameState.cs` | `E_GameState`의 단일 현재 값, Pause·Resume 유효 전환과 Reset을 Scene 의존성 없이 관리한다. GameSystem은 이 객체를 소유하고 Runtime Data에는 결과 enum만 반영한다. |
| `Assets/Scripts/Runtime/Core/E_PauseMenuSelection.cs` | Resume, Retry와 Quit 선택을 ResultMenu 선택과 분리한다. |
| `Assets/Tests/EditMode/GameStateTests.cs` | Playing→Paused, Paused→Playing, 잘못된 전환, 중복 요청과 Reset을 검증한다. |
| `Assets/Tests/PlayMode/GamePauseIntegrationTests.cs` | 생산 Scene에서 Mode별 Pause·Resume orchestration과 진행 중단을 검증한다. |

### 수정 파일 후보

| 파일 | 예상 변경 책임 |
|---|---|
| `Assets/Scripts/Runtime/Core/E_GameState.cs` | Paused 상태 추가 |
| `Assets/Scripts/Runtime/Core/E_UIState.cs` | Pause UI State 추가 |
| `Assets/Scripts/Runtime/Core/GameRuntimeData.cs` | 새 Game State와 UI State의 초기화·보존·Clear 회귀 확인. 별도 Pause bool은 추가하지 않음 |
| `Assets/Scripts/Runtime/Systems/GameSystem.cs` | Pause/Resume 승인, 상태 반영, Action Map 전환, System 실행 순서, Retry와 공용 Quit 경로 |
| `Assets/Scripts/Runtime/Systems/PlayerMovementSystem.cs` | 이동 계산 Pause/Resume과 내부 이동 상태 보존 |
| `Assets/Scripts/Runtime/Systems/PlayerControllerSystem.cs` | Rigidbody 속도·물리 상태 보존, 물리 고정과 복원 |
| `Assets/Scripts/Runtime/Systems/StageSystem.cs` | Pause 중 Goal 및 종료 요청 차단과 Resume |
| `Assets/Scripts/Runtime/Systems/InfiniteModeSystem.cs` | FixedUpdate 진행 중단, Run 기록 보존과 Map Pattern Pause 연결 |
| `Assets/Scripts/Runtime/Features/InfiniteMapPattern.cs` | Pause 중 `TryAdvance` 거부와 Resume |
| `Assets/Scripts/Runtime/Systems/UIManagementSystem.cs` | Pause UI State, 독립 선택 상태, Keyboard와 Pointer 선택 반영 |
| `Assets/Tests/EditMode/GameRuntimeDataTests.cs` | Paused 상태 반영·보존·Clear 회귀 |
| `Assets/Tests/PlayMode/GameLifecycleIntegrationTests.cs` | Playing의 Player+UI Action Map 정책과 transient 입력 소비 기대값 변경 |
| `Assets/Tests/PlayMode/UIInputSystemTests.cs` | UI Action Map 전환과 입력 Reset 회귀 |
| `Assets/Tests/PlayMode/StageSystemTests.cs` | Pause 중 Goal·종료 요청 거부와 Resume 회귀 |
| `Assets/Tests/PlayMode/InfiniteModeSystemTests.cs` | Pause 중 거리·Score·저속 시간·추락 판정 불변 |
| `Assets/Tests/PlayMode/InfiniteMapPatternTests.cs` | Pause 중 Pattern 재배치 거부와 Resume |
| `Assets/Tests/PlayMode/ResultMenuIntegrationTests.cs` | PausePanel과 독립성, 공용 Retry·Quit 경로의 기존 ResultMenu 회귀 |
| `Assets/Scenes/SampleScene.unity` | 사용자 작업으로 최소 PausePanel과 필요한 Serialized Reference 연결 |

### 변경이 필요하지 않은 후보

- `Assets/InputSystem_Actions.inputactions`: 기존 UI Cancel과 `{Cancel}` Binding을 재사용한다.
- `Assets/InputSystem_Actions.cs`: Input Action Asset 변경이 없으므로 생성 Wrapper를 수정하거나 재생성하지 않는다.
- `Assets/Scripts/Runtime/Systems/TimerSystem.cs`: 기존 Pause/Resume API를 재사용한다.
- `Assets/Scripts/Runtime/Core/TimerRuntimeData.cs`: 기존 Pause 기간 제외 계약을 재사용한다.
- `Assets/Scripts/Runtime/Systems/ResultSystem.cs`: PausePanel은 Result Data를 생성하거나 변경하지 않는다.
- `Assets/Scripts/Runtime/Systems/CameraSystem.cs`와 `CameraFollow.cs`: Player 물리가 고정되므로 Pause 전용 상태를 추가하지 않는다.
- `Assets/Scripts/Runtime/Systems/CollisionSystem.cs`: PlayerMovementSystem의 계산 중단으로 반복 갱신되지 않으며 Resume 시 기존 경로에서 다시 갱신한다.

### 책임 중복 위험과 적용 기준

- Game State와 별도 `IsPaused` bool을 동시에 저장하지 않는다.
- GameSystem은 Pause 실행 순서만 조정하고 Player, Stage, InfiniteMode와 UI의 내부 상태를 직접 변경하지 않는다.
- `Time.timeScale`을 변경하지 않고 Timer와 각 진행 System의 명시적 API를 사용한다.
- 종료용 `StopMovement`, `StopStage`와 `InfiniteModeSystem.Stop`을 Pause에 사용하지 않는다.
- PausePanel 선택 enum과 ResultMenu 선택 enum을 공유하지 않는다.
- GameSystem이 InfiniteMapPattern을 직접 참조하지 않고 InfiniteModeSystem을 통해 중단·재개한다.
- Pause와 Resume은 실행 순서가 필요한 1:1 요청이므로 신규 이벤트를 만들지 않는다.
- 정상적으로 거부되는 중복 Pause와 상태 밖 요청에 프레임 반복 로그를 추가하지 않는다.

### SampleScene 정적 조사 결과

- `UIRoot` 아래에 `StageHUD`와 `ResultPanel`이 있고 각각 Canvas를 자식으로 가진다.
- Scene에는 Canvas 2개, ResultMenu Button 2개와 EventSystem 1개가 존재한다.
- EventSystem에는 InputSystemUIInputModule이 있으나 현재 비활성 상태이며 기존 UI 선택과 Pointer 판정은 UIManagementSystem과 UIInputSystem이 수행한다.
- UIManagementSystem에는 StageHUD, ResultPanel, ClearTimeText, RetryButton과 QuitButton 참조가 모두 연결되어 있다.
- GameSystem에는 현재 필요한 System 참조가 연결되어 있다.
- InfiniteModeSystem에는 InfiniteMapPattern 참조가 없고 PausePanel GameObject도 존재하지 않는다.
- 기존 `UIRoot`, EventSystem과 Button 구성은 재사용할 수 있다.

### Scene 사용자 작업 확정 범위

Scene 작업은 Step 8에서 사용자가 Unity Editor로 수행한다.

1. `UIRoot` 아래에 `PausePanel`을 `StageHUD`, `ResultPanel`과 같은 UI State Root로 추가한다.
2. PausePanel 아래에 기능 확인용 Canvas, Pause 제목, ResumeButton, RetryButton과 QuitButton을 구성한다.
3. ResumeButton을 기본 선택으로 식별할 수 있게 Navigation 순서를 Resume→Retry→Quit으로 연결한다.
4. UIManagementSystem에 추가될 PausePanel과 세 Button Serialized Field를 연결한다.
5. InfiniteModeSystem에 추가될 InfiniteMapPattern Serialized Field를 Scene의 기존 InfiniteMapPattern Component에 연결한다.
6. PausePanel은 시작 시 비활성 상태로 저장한다.
7. Scene을 저장하고 닫았다가 다시 연 뒤 Missing Script, Missing Reference와 Inspector 값 유지를 확인한다.

Input Action Asset 수정과 Generate C# Class 실행은 현재 확정된 범위에 필요하지 않다.

### 회귀 Test 영향 범위

- 기존 기준 `145 Edit Mode / 67 Play Mode`는 이전 Unity Test Runner 성공 결과로 유지한다. 이번 Step에서는 Test Runner를 실행하지 않았다.
- 소스의 NUnit Attribute를 정적으로 집계하면 Edit Mode는 Test 94개와 TestCase 51개로 총 145개 Case이며 Play Mode는 Test 34개와 UnityTest 33개로 총 67개 Case이다.
- 직접 기대값 수정 대상은 `GameLifecycleIntegrationTests`의 Playing UI Action Map 및 transient 입력 Test이다.
- 직접 확장 대상은 `GameRuntimeDataTests`, `UIInputSystemTests`, `StageSystemTests`, `InfiniteModeSystemTests`, `InfiniteMapPatternTests`와 신규 Pause Test이다.
- `TimerRuntimeDataTests.PauseAndResume_ValidRequests_ExcludePausedDuration`과 `TimerSystemTests.TimerRequests_ValidSequence_ManageMeasuredTime`은 기존 Timer 계약의 직접 회귀 기준이다.
- `ResultMenuIntegrationTests`는 Result 상태 Cancel 무동작, Retry와 Mouse Click의 단일 실행을 유지해야 한다.
- `PlayerJumpIntegrationTests`와 `MomentumLandingIntegrationTests`는 Resume 후 이동 상태 및 transient 입력 잔류가 없는지 확인할 이동 회귀 기준이다.
- `InfiniteModeIntegrationTests`는 같은 Mode Retry, 단일 종료와 Result Data 회귀 기준이다.
- 생산 Scene 구조 Test에는 PausePanel 존재, 시작 비활성, Button 3개와 Serialized Reference를 추가해야 한다.

### 정적 검증

- 기존 Public API와 변경 예상 지점을 파일별로 기록한다.
- Pause 책임의 중복 구현 후보를 찾는다.
- Scene에서 재사용할 수 있는 UI와 새로 필요한 최소 오브젝트를 구분한다.
- 기존 Test 145/67개의 직접 영향 범위를 정한다.

### 수동 작업

없음.

### 완료 조건

- [x] 기존 계약과 Phase 3 확장 지점이 정리되었다.
- [x] 신규·수정 파일 후보와 Scene 수동 범위가 확정되었다.

## Step 3. GamePause 상태와 Runtime Data 계약을 Unit Test로 먼저 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Playing에서 Pause 성공
- Pause 중 중복 Pause 거부
- Pause에서 Resume 성공
- Playing이 아닌 상태의 Pause 거부
- Pause가 아닌 상태의 Resume 거부
- Reset 후 초기 상태 복원
- Retry와 종료 요청에 필요한 상태 전환
- 잘못된 전환에서 상태 불변
- Game State의 Pause 표현
- Pause 중 Runtime Data 보존
- Retry와 EndGame에서 Pause 상태 제거
- Stage와 Infinite Mode의 동일한 상태 계약
- 기존 Game State 전환 회귀

### 구현 원칙

- Scene과 MonoBehaviour에 의존하지 않는 순수 상태 객체를 우선한다.
- 상태 전환 규칙과 GameSystem orchestration을 분리한다.
- Runtime Data에는 System 간 공유 상태만 포함한다.
- 실패 Test를 먼저 확인한 후 최소 생산 코드를 구현한다.

### 수행 결과

- `GameStateTests`와 Pause Runtime Data Test를 생산 코드보다 먼저 작성했다.
- 사용자 요청에 따라 Unity Test Runner를 실행하지 않았으므로 실패 Test 실행 결과는 확인하지 않았다.
- `E_GameState.Paused`를 추가했다.
- Scene과 MonoBehaviour에 의존하지 않는 `GameState`를 추가했다.
- `GameState`가 None→Initializing→Ready→Playing, Playing↔Paused, Playing·Paused→Ending, Ending→Ended와 Ended→Initializing 전환을 관리한다.
- 잘못된 전환, 중복 Pause와 Pause가 아닌 상태의 Resume은 현재 상태를 변경하지 않고 거부한다.
- `GameSystem`의 `_currentGameState`를 제거하고 `GameState`를 유일한 Game State 저장소로 사용하도록 기존 생명주기 상태 반영 경로를 교체했다.
- `GameRuntimeData`에는 별도 Pause bool을 추가하지 않고 기존 Game State enum 반영 계약을 재사용했다.
- Stage Mode의 Player Movement Runtime Data와 InfiniteMode의 거리·Score가 Paused 상태 변경만으로 초기화되지 않는 Test를 추가했다.
- Paused 상태에서 Runtime Data Clear 시 Game State와 Mode별 Runtime Data가 제거되는 Test를 추가했다.

### 정적 검증

- [x] 신규 Class가 파일당 하나인지 확인했다.
- [x] `.meta` 존재와 GUID 중복이 없음을 확인했다.
- [x] `GameState`에 UnityEngine, UI와 Scene 의존성이 없음을 확인했다.
- [x] Game State의 실제 저장소가 `GameState` 한 곳임을 확인했다.
- [x] Mode별 Pause Data와 별도 Pause bool이 생기지 않았음을 확인했다.
- [x] `git diff --check`가 통과했다.
- [x] Scene, Input Action Asset과 생성 Wrapper가 변경되지 않았음을 확인했다.

### 수동 작업

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상하지 않은 Error와 Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 전체 Edit Mode Test 167개를 실행했다.
- [x] 실제 실행 결과 `167 Passed, 0 Failed`를 확인했다.
- [x] Edit Mode Test 실행에 예상하지 않은 Error와 Warning이 없음을 확인했다.

Step 3에서는 Build와 Scene 작업이 필요하지 않다.

### 완료 조건

- [x] 신규 GamePause Unit Test가 통과한다.
- [x] Pause Runtime Data Test가 통과한다.
- [x] 기존 Edit Mode 전체 회귀가 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

## Step 4. Pause 입력과 Action Map 정책을 Test 우선으로 연결한다

- 진행 상태: **완료**

### Test 우선 항목

- Playing에서 Pause 요청 1회 수집
- Pause 진입 후 Player Action Map 비활성화
- Pause 중 필요한 UI 또는 Pause Action Map 활성화
- Resume 후 Player Action Map 복원
- transient 입력 소비와 중복 요청 방지
- Result 상태 입력 흐름 회귀

### 구현 원칙

- 입력 수집은 InputSystem이, 입력 의미 판단은 GameSystem이 담당한다.
- 생성된 Input Action C#을 직접 임시 수정하지 않는다.
- Input Action Asset 변경이 필요하면 Asset을 원본으로 수정하고 Wrapper를 재생성한다.

### 수행 결과

- `GameLifecycleIntegrationTests`의 기존 Playing Action Map 기대값을 확정 정책에 맞게 Test에서 먼저 변경했다.
- Playing에서 Cancel 입력 1회가 Paused 전환과 Player Action Map 비활성화를 수행하는 Test를 추가했다.
- Paused에서 Cancel 입력 1회가 Playing 복원과 Player Action Map 재활성화를 수행하는 Test를 추가했다.
- 중복 Pause 요청이 상태와 Action Map을 변경하지 않고 거부되는 Test를 추가했다.
- Playing에서 사용하지 않는 Submit과 Click transient 입력이 소비되는 Test를 추가했다.
- Stage Mode와 InfiniteMode Playing에서 UI Action Map이 활성화되는 동일한 정책을 회귀 Test에 반영했다.
- `GameSystem.Update`가 Playing, Paused와 Ended의 UI 입력을 상태별로 해석하도록 분리했다.
- `PauseGame`과 `ResumeGame`은 이번 Step에서 Game State, Runtime Data와 Action Map만 전환한다.
- Playing에서는 Player와 UI Action Map을 활성화하고 Paused에서는 Player Action Map을 비활성화한 채 UI Action Map을 유지한다.
- 상태별 입력을 해석한 프레임에 UI transient 입력을 소비한다.
- 기존 UI Cancel의 `*/{Cancel}` Binding을 재사용했으며 Input Action Asset과 생성 Wrapper는 변경하지 않았다.
- Timer, Player 이동, Rigidbody, Stage와 InfiniteMode 중단·재개는 Step 5와 Step 6 범위로 남겼다.

### 정적 검증

- [x] UIInputSystem의 7개 Callback 등록과 해제 쌍이 일치함을 확인했다.
- [x] Playing의 Player+UI, Paused의 UI 전용 Action Map 정책이 코드와 Test에서 일치함을 확인했다.
- [x] Player Action Map의 Move, Jump와 MomentumLanding Binding을 변경하지 않았다.
- [x] UI Navigate, Submit, Cancel, Point와 Click Binding을 변경하지 않았다.
- [x] Input Action Asset과 생성 Wrapper가 변경되지 않았음을 확인했다.
- [x] Scene이 변경되지 않았음을 확인했다.
- [x] 기존 Result 상태의 Cancel 무동작과 UI Action Map 정책을 유지했다.
- [x] Play Mode Test의 정적 집계가 기존 67개에서 신규 3개를 포함한 70개임을 확인했다.
- [x] `git diff --check`가 통과했다.

### 수동 작업

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상하지 않은 Error와 Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 관련 Test 22개를 포함한 전체 Play Mode Test 70개를 실행했다.
- [x] 실제 실행 결과 `70 Passed, 0 Failed`를 확인했다.
- [x] Play Mode Test 실행에 예상하지 않은 Error와 Warning이 없음을 확인했다.

Input Action Asset 변경, Generate C# Class, Build와 Scene 작업은 필요하지 않다.

### 완료 조건

- [x] Pause 요청과 Action Map 전환 Test가 통과한다.
- [x] 기존 Player 및 UI 입력 Test가 통과한다.

## Step 5. GameSystem Pause·Resume orchestration을 Play Mode Test로 먼저 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Stage Mode Playing에서 Pause 진입
- InfiniteMode Playing에서 Pause 진입
- Pause에서 Resume 후 같은 Mode와 Run 유지
- Pause 중 중복 Pause 거부
- Result와 종료 상태에서 Pause 거부
- Pause와 Stage 종료가 같은 프레임에 요청될 때 단일 유효 전환
- EndGame과 Retry 후 Pause 상태 제거

### 수행 결과

- 생산 Scene을 사용하는 `GamePauseOrchestrationTests`를 생산 코드 변경보다 먼저 추가했다.
- Stage Mode Pause·Resume이 같은 Runtime Data와 Run을 유지하는 Test를 추가했다.
- InfiniteMode Pause·Resume이 같은 Runtime Data와 Run을 유지하는 Test를 추가했다.
- Result 상태의 Pause 요청이 상태를 변경하지 않고 거부되는 Test를 추가했다.
- Paused에서 EndGame이 Pause와 Runtime Data를 제거하고 Ended로 전환하는 Test를 추가했다.
- Paused에서 Retry가 이전 Runtime Data를 제거하고 같은 Mode의 독립적인 Run을 시작하는 Test를 추가했다.
- Paused와 Stage 종료 요청이 같은 프레임에 발생하면 Stage 종료 흐름을 한 번 우선하는 Test를 추가했다.
- `RetryGame`을 Pause와 Result 상태가 공유하는 같은 Mode 재시작 경로로 추가했다.
- ResultMenu Retry가 직접 StartGame을 호출하지 않고 `RetryGame`을 재사용하도록 변경했다.
- Stage 종료 이벤트는 Playing뿐 아니라 Paused에서도 수신하여 확정된 Stage 종료를 우선하도록 변경했다.
- Pause와 Resume의 Timer, 이동, 물리, Stage 및 InfiniteMode 내부 중단은 Step 6 범위로 유지했다.

### 정적 검증

- [x] GameSystem이 공개 요청 API로 실행 순서만 조정하고 각 System 내부 상태를 직접 변경하지 않음을 확인했다.
- [x] Pause Retry와 Result Retry가 `RetryGame` 하나를 재사용함을 확인했다.
- [x] Paused EndGame이 기존 EndGame 정리 경로를 재사용함을 확인했다.
- [x] Paused 중 Stage 종료 이벤트가 기존 단일 EndGame 경로를 사용함을 확인했다.
- [x] Ending과 Ended 상태의 중복 EndGame 방지 조건이 유지됨을 확인했다.
- [x] 정상 Update 경로에 반복 로그가 추가되지 않았음을 확인했다.
- [x] 신규 Test 파일에 `.meta`가 존재하고 GUID 중복이 없음을 확인했다.
- [x] Scene과 Input Action Asset이 변경되지 않았음을 확인했다.
- [x] Play Mode Test 정적 집계가 기존 70개에서 신규 6개를 포함한 76개임을 확인했다.
- [x] `git diff --check`가 통과했다.

### 수동 작업

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상하지 않은 Error와 Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 관련 Test 25개를 포함한 전체 Play Mode Test 76개를 실행했다.
- [x] 실제 실행 결과 `76 Passed, 0 Failed`를 확인했다.
- [x] Play Mode Test 실행에 예상하지 않은 Error와 Warning이 없음을 확인했다.

Build, Input Action Asset과 Scene 작업은 필요하지 않다.

### 완료 조건

- [x] Mode별 Pause·Resume 통합 Test가 통과한다.
- [x] 기존 시작, 종료와 Retry Test가 통과한다.

## Step 6. Pause 동안 Stage와 InfiniteMode 진행을 정지한다

- 진행 상태: **완료**

### Test 우선 항목

- Pause 동안 PlayTimer 경과 시간 불변
- Resume 후 기존 경과 시간부터 증가
- Pause 동안 Player 입력과 이동 중단
- Pause 동안 Goal 도달과 Stage 종료 처리 차단 또는 확정 규칙 적용
- Retry 후 새 Timer 생성과 초기화
- 여러 번 Pause·Resume 시 누적 시간 정확성
- Pause 동안 Pattern 재배치 중단
- Pause 동안 InfiniteMode 저속 및 추락 종료 판정 중단
- Pause 동안 거리와 Score 불변
- Resume 후 기존 거리와 Score부터 갱신
- InfiniteMode Retry 후 거리, Score와 확정 상태 초기화
- Pause 직후와 Resume 직후 FixedUpdate 경계

### 정적 검증

- [x] Timer 계산을 Test에 다시 구현하지 않고 TimerSystem Public 결과를 사용한다.
- [x] PauseTimer와 ResumeTimer의 기존 책임을 재사용하는지 확인한다.
- [x] Stage Mode에 Infinite 기록 경로가 연결되지 않는지 확인한다.
- [x] Pause가 Score 계산식이나 거리 규칙을 변경하지 않는지 확인한다.
- [x] InfiniteModeSystem, PlayerMovementSystem과 StageSystem 중단 책임이 중복되지 않는지 확인한다.
- [x] Pattern과 Score가 새 의존성을 만들지 않는지 확인한다.
- [x] Scene과 Input Action Asset을 변경하지 않았음을 확인한다.
- [x] Play Mode Test 정적 집계가 기존 76개에서 신규 4개를 포함한 80개임을 확인한다.
- [x] `git diff --check`가 통과한다.

### 수행 결과

- `GameSystem`이 Mode에 따라 TimerSystem, StageSystem, InfiniteModeSystem, PlayerMovementSystem과 PlayerControllerSystem의 공개 Pause·Resume API를 조율하도록 구현했다.
- Stage Mode Pause는 PlayTimer를 기존 `PauseTimer`로 정지하고 Resume 시 `ResumeTimer`로 같은 누적 시간부터 재개한다.
- PlayerMovementSystem은 Pause 동안 FixedUpdate 이동 계산을 중단하며 기존 이동·점프·착지 Runtime 상태를 보존한다.
- PlayerControllerSystem은 Pause 직전 Rigidbody 속도, 가속도와 Constraints를 보존하고 `FreezeAll`로 물리를 정지한 뒤 Resume 시 복원한다.
- StageSystem은 Pause 동안 Goal 도달과 Infinite Stage 종료 요청을 차단한다.
- InfiniteModeSystem은 Pause 동안 거리, Score, 저속 종료와 추락 종료 판정을 갱신하지 않는다.
- Pattern 재배치는 Player Rigidbody 고정으로 Trigger 경계 진입이 발생하지 않게 하며 Pattern 또는 Scene에 새 의존성을 추가하지 않았다.
- Stage 및 Infinite 단위 Test 2개와 생산 Scene 기반 통합 Test 2개를 Test 우선으로 추가했다.

### 수동 작업

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 전체 Edit Mode Test 167개를 실행했다.
- [x] 실제 실행 결과 `167 Passed, 0 Failed`를 확인했다.
- [x] Edit Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 전체 Play Mode Test 80개를 실행했다.
- [x] 실제 실행 결과 `80 Passed, 0 Failed`를 확인했다.
- [x] Play Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.

Build와 Scene 작업은 필요하지 않다.

### 완료 조건

- [x] Stage Mode Pause 중 시간과 진행 정지 Test가 통과한다.
- [x] InfiniteMode Pause 중 모든 진행 정지 Test가 통과한다.
- [x] Mode별 Resume과 Retry 회귀 Test가 통과한다.
- [x] 기존 Edit Mode와 Play Mode 전체 회귀가 통과한다.

## Step 7. PausePanel 상태와 선택 규칙을 Unit Test로 먼저 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Pause UI State 진입과 종료
- 기본 선택 항목
- Resume, Retry와 Quit 선택 이동
- Submit과 Cancel의 확정 동작
- Pointer 선택과 Click 처리
- PausePanel 비활성 상태의 입력 거부
- ResultMenu 선택 상태와 독립성
- 초기화와 Retry 후 선택 상태 Reset

### 정적 검증

- [x] UIManagementSystem이 GamePause 규칙을 직접 수행하지 않는지 확인한다.
- [x] Pause 선택 enum과 Result 선택 enum의 책임을 구분한다.
- [x] Phase 4 HUD와 Infinite ResultPanel을 선행 구현하지 않는지 확인한다.
- [x] PausePanel의 Scene 참조와 시각적 선택 반영을 Step 8 범위로 유지했다.
- [x] Resume, Retry와 Quit의 실제 실행을 Step 9 범위로 유지했다.
- [x] Scene과 Input Action Asset을 변경하지 않았음을 확인했다.
- [x] 신규 `.meta` 3개의 GUID가 존재하고 중복되지 않음을 확인했다.
- [x] Edit Mode Test 정적 집계가 기존 167개에서 신규 10개를 포함한 177개임을 확인했다.
- [x] 기존 Play Mode Test 정적 집계가 80개로 유지됨을 확인했다.
- [x] `git diff --check`가 통과했다.

### 수행 결과

- `E_UIState.Pause`와 ResultMenu 선택 enum과 독립된 `E_PauseMenuSelection`을 추가했다.
- Unity UI에 의존하지 않는 `PauseMenuState`가 활성화, 기본 Resume 선택, 비순환 Navigate, Submit, Cancel, Pointer 선택과 Click 결과를 관리하도록 구현했다.
- PauseMenuState는 비활성 상태의 모든 입력을 거부하고 비활성화 시 Resume 선택으로 Reset한다.
- `UIManagementSystem`은 Pause UI State에 따라 PauseMenuState를 활성화·비활성화하고 선택 결과만 외부에 제공한다.
- UIManagementSystem은 선택된 Resume, Retry 또는 Quit의 게임 동작을 직접 실행하지 않는다.
- `PauseMenuStateTests` 10개를 생산 코드보다 먼저 추가하여 상태 및 선택 계약을 고정했다.

### 수동 작업

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 신규 `PauseMenuStateTests` 10개를 포함한 전체 Edit Mode Test 177개를 실행했다.
- [x] 실제 실행 결과 `177 Passed, 0 Failed`를 확인했다.
- [x] Edit Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 `ResultMenuIntegrationTests` 5개를 포함한 전체 Play Mode Test 80개를 실행했다.
- [x] 실제 실행 결과 `80 Passed, 0 Failed`를 확인했다.
- [x] Play Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.

Build와 Scene 작업은 필요하지 않다.

### 완료 조건

- [x] PausePanel 상태 및 선택 Unit Test가 통과한다.
- [x] 기존 ResultMenu Unit Test가 통과한다.

## Step 8. PausePanel 최소 Scene 구성을 연결한다

- 진행 상태: **완료**

### AI 정적 준비

- [x] 필요한 GameObject, Component, 이름, 계층과 Serialized Field를 명시했다.
- [x] 기존 UIRoot, EventSystem과 UIManagementSystem을 재사용함을 확인했다.
- [x] Scene 변경 전 기준을 YAML Object 231개, GameObject 64개, MonoBehaviour 46개와 RectTransform 11개로 기록했다.
- [x] 생산 Scene 구조를 검증하는 `PausePanelSceneConfigurationTests`를 먼저 추가했다.
- [x] UIManagementSystem에 PausePanel 및 Resume, Retry와 Quit Button Serialized Field를 추가했다.
- [x] Pause UI State 활성화, 기본 Resume 선택 및 Pointer 판정의 Unity UI 반영을 구현했다.
- [x] Play Mode Test 정적 집계가 기존 80개에서 신규 1개를 포함한 81개임을 확인했다.
- [x] AI는 Scene과 Input Action Asset을 변경하지 않았다.
- [x] `git diff --check`가 통과했다.

### 사용자 Scene 작업

1. `SampleScene`의 `UIRoot` 바로 아래에 빈 GameObject `PausePanel`을 생성한다.
2. `PausePanel` 아래에 Screen Space Overlay `Canvas`를 생성한다. 새 EventSystem이 자동 생성되면 삭제하고 기존 Scene EventSystem만 유지한다.
3. `Canvas` 아래에 식별 가능한 Pause 제목 Text를 `PauseTitle` 이름으로 생성한다.
4. 같은 `Canvas` 아래에 Unity UI Button 3개를 만들고 각각 `ResumeButton`, `RetryButton`, `QuitButton`으로 지정한다. Button 자식 Text는 각각 Resume, Retry와 Quit으로 표시한다.
5. 세 Button이 겹치지 않고 클릭 가능한 크기가 되도록 세로로 배치한다. 최종 아트와 레이아웃은 Phase 4 범위이므로 기능 식별이 가능한 최소 구성만 사용한다.
6. 각 Button의 Navigation을 Explicit로 설정하여 Down 방향은 Resume→Retry→Quit, Up 방향은 Quit→Retry→Resume이 되게 연결한다. Resume의 Up과 Quit의 Down은 비워 비순환으로 유지한다.
7. `UIManagementSystem` GameObject의 Component에서 `Pause Panel`에 `PausePanel`, `Pause Resume Button`에 `ResumeButton`, `Pause Retry Button`에 PausePanel의 `RetryButton`, `Pause Quit Button`에 PausePanel의 `QuitButton`을 연결한다. 기존 Result Panel의 Retry·Quit 참조는 변경하지 않는다.
8. `PausePanel`을 비활성 상태로 설정한 뒤 Scene을 저장한다.
9. Scene을 닫았다가 다시 열어 계층, 비활성 상태와 네 Serialized Reference가 유지되는지 확인한다.
10. Inspector에 Missing Script, Missing Reference 또는 None으로 남은 새 Pause 참조가 없는지 확인한다.
11. Unity Script Compilation 성공과 예상치 못한 Error·Warning 부재를 확인한다.
12. Unity Test Runner에서 Play Mode `PausePanelSceneConfigurationTests` 1개를 실행한다. 전체 Play Mode 실행 시 예상 Test 수는 81개다.

### 수동 검증 결과

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 전체 Edit Mode Test 177개를 실행했다.
- [x] 실제 실행 결과 `177 Passed, 0 Failed`를 확인했다.
- [x] Edit Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 신규 Scene 구조 Test와 ResultMenu 버튼 범위 회귀 수정을 포함한 전체 Play Mode Test 81개를 실행했다.
- [x] 실제 실행 결과 `81 Passed, 0 Failed`를 확인했다.
- [x] Play Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.

### 제한 범위

- Phase 3에서는 기능 검증이 가능한 최소 화면만 구성한다.
- 최종 레이아웃, 아트, 애니메이션과 Mode별 UI 마무리는 Phase 4로 남긴다.

### 완료 조건

- [x] UIManagementSystem 초기화 후 PausePanel이 현재 UI State에 따라 비활성화된다.
- [x] 생산 Scene 구조 Play Mode Test가 통과한다.
- [x] Phase 4 UI가 선행 추가되지 않았다.

## Step 9. Pause 상태의 Resume, Retry와 Quit 통합 흐름을 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Resume 선택 시 동일 Run 복원
- Retry 선택 시 같은 Mode의 새 Run 시작
- Retry 시 Timer, 거리, Score, 물리와 UI 상태 초기화
- Quit 선택 시 확정된 종료 흐름 1회 수행
- Cancel 입력의 확정 동작
- Keyboard Submit과 Mouse Click의 동일 결과
- 빠른 중복 입력에서 실행 1회 보장
- Result 상태의 Retry와 Quit 회귀

### 정적 검증

- [x] Application 종료 호출 지점이 `ApplicationQuitService` 한 곳인지 확인했다.
- [x] Pause Retry와 Result Retry가 공통 `RetryGame` 시작 흐름을 재사용하는지 확인했다.
- [x] PausePanel이 Result Data를 생성하거나 수정하지 않는지 확인했다.
- [x] Cancel이 현재 선택과 관계없이 Resume을 요청하고 다른 입력과 동시에 둘 이상의 실행 흐름을 시작하지 않음을 확인했다.
- [x] Keyboard Submit과 Mouse Click이 같은 `ExecutePauseMenuSelection` 경로를 사용함을 확인했다.
- [x] Quit Test가 실제 Editor Play Mode를 종료하지 않도록 `IApplicationQuitService` 대역을 주입함을 확인했다.
- [x] Pause 진입과 종료 시 Runtime Data 및 UIManagementSystem의 UI State가 Pause와 StageHud로 함께 갱신됨을 확인했다.
- [x] Scene과 Input Action Asset을 변경하지 않았음을 확인했다.
- [x] 신규 Script 및 Test `.meta` 3개가 존재함을 확인했다.
- [x] Play Mode Test 정적 집계가 기존 81개에서 신규 6개를 포함한 87개임을 확인했다.
- [x] Scene을 제외한 Step 9 코드와 Test에 `git diff --check` 문제가 없음을 확인했다.

### 수행 결과

- GameSystem의 Paused 입력 처리가 Cancel, Click, Point, Navigate와 Submit을 우선순위에 따라 한 번 소비하고 최대 하나의 선택만 실행하도록 구현했다.
- Pause Cancel과 Resume 선택은 같은 Run을 유지하며 StageHud로 복원한다.
- Pause Retry와 Result Retry는 모두 `RetryGame`을 사용하여 같은 Mode의 독립 Run을 시작한다.
- InfiniteMode Retry 후 거리, Score와 확정 상태 초기화를 통합 Test로 고정했다.
- Pause와 Result의 Quit은 `RequestApplicationQuit`을 거쳐 하나의 `ApplicationQuitService`를 사용한다.
- Editor에서는 Play Mode 종료, Player에서는 `Application.Quit`을 요청하도록 환경별 종료 처리를 분리했다.
- `PauseMenuIntegrationTests` 6개를 추가하여 Cancel, Keyboard Retry, Mouse Retry, Pause Quit, Result Quit과 Infinite Retry를 검증한다.

### 수동 작업

- [x] Unity Editor에서 Script Compilation 성공을 확인했다.
- [x] Script Compilation에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 신규 `PauseMenuIntegrationTests` 6개를 실행했다.
- [x] Unity Test Runner에서 전체 Edit Mode Test 177개를 실행했다.
- [x] 실제 실행 결과 `177 Passed, 0 Failed`를 확인했다.
- [x] Edit Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.
- [x] Unity Test Runner에서 전체 Play Mode Test 87개를 실행했다.
- [x] 실제 실행 결과 `87 Passed, 0 Failed`를 확인했다.
- [x] Play Mode Test 실행에 예상치 못한 Error·Warning이 없음을 확인했다.

Build와 Scene 작업은 필요하지 않다.

### 완료 조건

- [x] Resume, Retry와 Quit 통합 Test가 통과한다.
- [x] 기존 ResultMenu 회귀 Test가 통과한다.
- [x] 기존 Edit Mode와 Play Mode 전체 회귀가 통과한다.

## Step 10. 전체 정적 검증과 자동 회귀 Test를 수행한다

- 진행 상태: **완료**

### 정적 검증 체크리스트

- [x] GamePause 상태와 전환 규칙의 소유자가 한 곳이다.
- [x] GameSystem은 Pause 실행 순서만 조정한다.
- [x] Pause 중 Action Map 활성 정책이 확정 규칙과 일치한다.
- [x] Pause 중 Timer, Player, Stage와 Infinite 기록이 중단된다.
- [x] Resume이 Runtime Data와 Run 기록을 초기화하지 않는다.
- [x] Retry가 같은 Mode의 새 Run을 생성한다.
- [x] Result 상태에서 Pause를 시작할 수 없다.
- [x] Pause와 Result UI 상태 및 선택 정보가 분리되어 있다.
- [x] 신규 Script와 Test `.meta`가 존재하고 GUID가 중복되지 않는다.
- [x] Scene Serialized Reference와 fileID가 유효하다.
- [x] 정상 프레임 반복 로그가 추가되지 않았다.
- [x] Test가 Ignore되거나 기존 기대값이 약화되지 않았다.
- [x] Save, Leaderboard, Phase 4 UI와 Prototype 3 기능이 추가되지 않았다.
- [x] 관련 Feature와 System 문서가 구현과 일치한다.

### 정적 검증 결과

- `GameState`가 Playing과 Paused 전환 규칙을 단독 소유하며 Runtime Data는 확정 상태를 반영한다.
- GameSystem은 각 담당 System의 공개 Pause·Resume API와 UI·Action Map 실행 순서만 조율한다.
- `Time.timeScale` 사용은 없고 Timer, 이동 계산, Rigidbody, Stage 및 InfiniteMode가 각 책임 안에서 중단된다.
- PauseMenu와 ResultMenu는 서로 다른 enum과 선택 상태를 사용한다.
- 실제 Application 종료 호출은 `ApplicationQuitService` 한 곳에만 존재한다.
- Asset `.meta`에서 수집한 GUID 158개가 모두 고유하다.
- Scene의 `_pausePanel`, `_pauseResumeButton`, `_pauseRetryButton`과 `_pauseQuitButton` fileID가 각각 하나의 유효한 YAML Object 정의를 가리킨다.
- Test에 Ignore, Explicit, `Assert.Ignore` 또는 임의 통과 처리가 없다.
- 기존 Test 변경은 확정된 UI Action Map 기대값과 ResultPanel 하위 Button 식별 범위에 한정되며 검증 기대를 약화하지 않았다.
- Scene을 제외한 코드, Test와 문서의 `git diff --check`가 통과했다. Unity가 저장한 Scene YAML의 공백 형식은 생산 Scene 구조 Test와 Unity 재개방으로 검증했다.

### 자동 Test 체크리스트

- [x] 신규 GamePause Edit Mode Unit Test
- [x] Runtime Data와 UI 선택 Edit Mode Unit Test
- [x] Stage Mode Pause·Resume Play Mode Test
- [x] InfiniteMode Pause·Resume Play Mode Test
- [x] Pause Retry·Quit와 입력 Play Mode Test
- [x] 생산 Scene PausePanel 구조 Test
- [x] 기존 Edit Mode 전체 회귀
- [x] 기존 Play Mode 전체 회귀

### 수동 작업

- Step 9 완료 직후 생산 코드 변경 없이 수행한 Unity Script Compilation 성공 결과를 재사용했다.
- Step 9 완료 직후 수행한 전체 Edit Mode `177 Passed, 0 Failed`와 Play Mode `87 Passed, 0 Failed` 결과를 재사용했다.
- 두 Test 실행과 Script Compilation에서 예상하지 않은 Error·Warning이 없음을 확인했다.
- 추가 수동 작업은 없다.

### 완료 조건

- [x] 정적 검증 체크리스트가 모두 통과한다.
- [x] 전체 자동 Test가 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

## Step 11. 최소 수동 플레이로 Pause 화면과 조작을 확인한다

- 진행 상태: **완료**

### 정적으로 대체하는 항목

- [x] Timer, 거리와 Score의 정지 수치
- [x] 상태 전환 성공·실패
- [x] 호출 횟수와 중복 실행
- [x] Action Map 활성 상태
- [x] Retry 초기화와 Mode 유지
- [x] Scene Component와 Serialized Reference

Step 10 정적 검증과 전체 Edit Mode `177 Passed, 0 Failed`, Play Mode `87 Passed, 0 Failed` 결과로 위 항목을 대체했다.

### 사용자 수동 플레이

1. Stage Mode로 Play Mode를 시작하고 `Escape`를 눌러 PausePanel이 보이며 게임 화면이 시각적으로 고정되는지 확인한다.
2. Pause 중 Move, Jump와 Momentum Landing 입력을 한 번씩 시도하고 화면상 Player가 움직이지 않는지만 확인한다. 내부 Action Map과 수치는 관찰하지 않는다.
3. Keyboard Navigate로 Resume→Retry→Quit 및 역방향 선택 표시를 확인한다. `Escape`를 눌러 현재 선택과 관계없이 Resume되는지 확인한다.
4. Resume 직후 Player의 시각적 순간 이동이 없고 Pause 중 입력이 뒤늦게 실행되지 않으며 조작이 자연스럽게 이어지는지 확인한다.
5. 다시 Pause하고 Mouse Hover로 선택 표시가 바뀌는지 확인한 뒤 Resume을 Click하여 같은 화면으로 자연스럽게 복귀하는지 확인한다.
6. 다시 Pause하고 Retry를 Keyboard Submit 또는 Mouse Click으로 한 번 실행하여 Stage Mode 시작 화면으로 자연스럽게 전환되는지 확인한다. Mode 유지와 Runtime 초기화 수치는 자동 Test로 대체한다.
7. InfiniteMode로 Play Mode를 다시 시작하고 PausePanel 표시, 시각적 화면 고정, Resume의 자연스러운 연결과 Retry 화면 전환을 각각 한 번 확인한다.
8. Stage 또는 InfiniteMode에서 Result 화면에 진입한 뒤 `Escape`를 눌러 PausePanel이 열리지 않고 Result 화면이 유지되는지 확인한다.
9. 마지막 확인으로 PausePanel의 Quit을 한 번 실행한다. Unity Editor에서는 Play Mode가 종료되는지 확인한다. 이 항목은 다른 확인을 모두 마친 뒤 수행한다.
10. 전체 과정에서 Console에 예상하지 않은 Error와 Warning이 없는지 확인한다.

Script Compilation과 Test Runner 재실행은 필요하지 않으며 Build도 Step 11 검증 범위가 아니다. Scene을 편집하거나 저장할 필요가 없다.

### 사용자 확인 결과

- [x] Stage Mode와 InfiniteMode에서 Pause가 의도대로 동작함을 확인했다.
- [x] Pause UI Navigation이 설정한 순서대로 동작함을 확인했다.
- [x] Result 화면에서는 PausePanel이 열리지 않음을 확인했다.
- [x] Quit Button이 Unity Editor Play Mode를 종료함을 확인했다.
- [x] 전체 수동 플레이 과정에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 빠른 연속 조작과 짧은 입력 타이밍이 필요한 입력 잔류, 중복 실행 및 상태 전환 검증은 수동 확인에서 제외하고 기존 Play Mode Test 결과로 판정했다.

### 검증 방식 제한

- IDE 디버그 기능을 사용하지 않는다.
- 자동 Test로 판정한 수치와 내부 상태를 수동 관찰로 반복 판정하지 않는다.
- 빠른 연속 조작, 동일·인접 프레임 입력과 짧은 입력 Window를 수동 재현 절차로 요구하지 않고 자동 Test로 판정한다.
- 화면 전환, 실제 입력과 조작의 자연스러움만 수동으로 판단한다.

### 완료 조건

- [x] Stage와 Infinite Mode의 Pause 화면 및 조작이 정상이다.
- [x] Resume의 상태 복원과 입력 잔류 방지가 자동 Test를 통과했다.
- [x] Pause UI Navigation과 Mouse 입력 통합 Test가 정상이다.
- [x] Retry 자동 Test, Quit 수동 확인과 Result 상태 제한이 정상이다.
- [x] 예상하지 않은 Error와 Warning이 없다.

## Step 12. Phase 3 완료 근거를 정리한다

- 진행 상태: **완료**

### 수행 절차

1. 저장된 Scene과 Asset 변경을 정적으로 확인한다.
2. 최종 정적 검증 결과를 기록한다.
3. Unity Script Compilation 결과를 기록한다.
4. 전체 Edit Mode와 Play Mode Test 수 및 결과를 기록한다.
5. Step 11 수동 검증 결과와 미해결 사항을 기록한다.
6. 별도 Phase 3 Verification Result Task 문서를 작성한다.
7. 모든 완료 조건 충족 시에만 Roadmap Phase 3를 `완료`로 변경한다.

### 수동 작업

이전 Step에서 Scene 저장·재개방, Unity Compile/Test 결과와 최종 화면·조작 확인이 완료되었으므로 추가 수동 작업은 없다.

### Scene 초기 활성 상태 판정

- PausePanel의 저장 시 활성 여부는 기능 계약으로 사용하지 않는다.
- `UIManagementSystem.Initialize()`가 첫 프레임 렌더링 전 `E_UIState.None`을 적용하고 PausePanel을 비활성화한다.
- 생산 Scene Play Mode Test가 Runtime 초기 비활성 상태, Pause 진입 시 활성 상태와 StageHud 복귀 시 비활성 상태를 검증한다.
- 따라서 저장 시 활성 상태는 Phase 3 완료를 막는 미해결 사항이 아니다.

### 완료 조건

- [x] 정적 검증, Compile과 전체 Test가 통과한다.
- [x] 최소 수동 플레이 결과가 기록되어 있다.
- [x] Phase 3 범위 밖 기능이 포함되지 않았다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 영향 범위

- GamePause Feature
- GameSystem과 관련 Input, Timer, Movement, Controller, Stage, InfiniteMode 및 UI System
- Core Runtime Data와 상태 enum
- InfiniteMapPattern
- 최소 PausePanel Scene 구성
- Edit Mode 및 Play Mode Test
- 관련 문서와 Roadmap

---

# 검증 내용

- Roadmap Phase 3 목표, 구현 대상과 완료 조건을 확인했다.
- GamePause, GameSystem, TimerSystem과 UIInputSystem 문서를 확인했다.
- 현재 Game State, UI State, Action Map, Timer와 Retry 구조를 조사했다.
- Player 이동과 Rigidbody 상태, Stage 종료 판정, InfiniteMode 기록 및 Pattern 재배치 확장 지점을 조사했다.
- SampleScene의 UIRoot, Canvas, EventSystem과 Serialized Reference를 정적으로 확인했다.
- 기존 Test 145/67개의 구성과 직접 영향 범위를 정적으로 확인했다.
- 계산과 상태 규칙은 Edit Mode Unit Test로 우선 검증하도록 배치했다.
- System 실행 순서, 입력, 물리와 Scene 연결은 Play Mode Test로 배치했다.
- 화면 구성과 실제 조작감만 최소 수동 검증으로 분리했다.
- IDE 디버그 기능을 검증 절차에서 제외했다.

---

# 검증 결과

- Prototype 2 Phase 3 수행을 위한 12개 Step을 작성했다.
- 상태와 Runtime Data를 하나의 Unit Test Step으로 통합했다.
- Stage와 InfiniteMode의 진행 정지를 하나의 Mode별 통합 Step으로 구성했다.
- 전체 회귀 Test는 핵심 상태, Mode별 정지, 최종 선택 흐름과 최종 검증 시점에 수행하도록 조정했다.
- 정적 검증과 Test 우선 구현 순서를 정의했다.
- Unity Editor에서 필요한 Input Action, Scene, Compile, Test와 최종 플레이 작업을 명시했다.
- Step 1의 12개 미정 규칙을 확정하고 관련 Feature와 System 문서에 반영했다.
- Step 2의 기존 계약, 신규·수정 파일 후보, 책임 중복 위험과 Scene 사용자 작업 범위를 확정했다.
- 기존 UI Cancel Binding을 재사용하므로 Input Action Asset과 생성 Wrapper 변경은 필요하지 않음을 확인했다.
- GamePause 상태, Mode별 진행 중단, PausePanel과 Resume·Retry·Quit 통합 흐름 구현을 완료했다.
- Unity Script Compilation, Edit Mode `177 Passed, 0 Failed`와 Play Mode `87 Passed, 0 Failed`를 확인했다.
- Stage·Infinite Pause, Pause UI Navigation, Result 제한과 Editor Quit 수동 검증을 완료했다.
- 별도 `20260902_01_Phase3VerificationResult.md`에 최종 완료 근거를 기록했다.

---

# 후속 작업

Prototype 2 Phase 4의 Mode별 UI와 전체 반복 플레이 흐름 구현을 준비한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/PlayerInputSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/02_Systems/TimerSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_2/20260901_03_Phase2VerificationResult.md`
- `AI/90_Tasks/Prototype_2/20260902_01_Phase3VerificationResult.md`

---

# 작성 완료 기준

- General Task Template의 모든 필수 섹션을 작성했다.
- 실제 수행 순서를 Step으로 작성했다.
- 정적 검증과 Unit Test를 수동 검증보다 우선 배치했다.
- Unity Editor에서만 가능한 작업을 수동 작업으로 분리했다.
- Phase 3 구현, 정적 검증, 전체 자동 Test와 최소 수동 플레이 결과를 기록했다.
