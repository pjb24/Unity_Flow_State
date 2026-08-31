# 작업 정보

## 작업명

Prototype 2 Phase 1 Manual Steps

---

## 작업 일자

20260828

---

## 작업 담당자

AI

---

## 작업 상태

완료

---

# 작업 목적

Prototype 2 Phase 1의 InfiniteMode 기본 플레이 흐름을 구현하기 위해 사용자가 수동으로 수행할 작업을 순서가 있는 Step으로 정리한다.

정적 검증과 Unity Test Runner를 구현 과정 전반에 사용하여 문서, 코드, Scene과 설정의 불일치를 조기에 확인한다.

자동 판정 가능한 규칙은 Unit Test와 Play Mode Test로 검증하고, Unity Editor 조작과 실제 플레이 감각처럼 자동 판정하기 어려운 항목만 수동 검증으로 남긴다.

---

# 작업 대상

## Roadmap

- Prototype 2 Phase 1
- InfiniteMode 기본 플레이 흐름
- InfiniteMode 전용 Map Pattern 1개
- InfiniteMode 시작, 종료와 Retry
- Stage Mode 회귀 검증

## 예상 영향 System

- GameSystem
- RuntimeDataSystem
- StageSystem
- PlayerMovementSystem
- ResultSystem
- UIManagementSystem

## 예상 영향 Feature

- InfiniteMode
- StagePlay
- ResultMenu

## 예상 영향 Asset

- Runtime Core 코드
- Runtime System 및 Feature 코드
- Edit Mode Test
- Play Mode Test
- InfiniteMode를 구성할 Scene 또는 Stage Asset
- 기존 `Assets/Scenes/SampleScene.unity`

---

# 작업 전 상태

- 기존 구현은 일반 Stage 하나를 시작하고 Goal 도달 시 종료하는 흐름을 제공한다.
- `StageSystem`은 `StageGoal` 참조를 필수로 요구한다.
- `GameSystem`은 Stage 종료 시 Clear Time Result Data를 생성한 뒤 Result 상태로 전환한다.
- Retry는 같은 실행 세션에서 `GameSystem.StartGame()`을 다시 실행하는 방식으로 일반 Stage를 초기화한다.
- 현재 코드에는 게임 Mode, InfiniteMode 상태, InfiniteMode 종료 판정과 Map Pattern 반복 구조가 없다.
- `SampleScene`에는 `Goal`과 일반 Stage용 구성만 존재한다.
- 기존 Edit Mode Test와 Play Mode Test는 일반 Stage, 이동, Timer, Result와 Retry 흐름을 검증한다.
- InfiniteMode의 최소 이동 속도 판정 세부 기준, Stage 이탈 경계와 Mode 선택 방식은 현재 문서에 확정되어 있지 않다.
- Phase 1은 ScoreRecord, GamePause, InfiniteMode HUD와 다중 Map Pattern을 구현하지 않는다.

---

# 조사 내용

아래 문서와 구현을 확인했다.

## Project

- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`

## Rules

- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/EVENT_RULE.md`
- `AI/01_Rules/LOGGING_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`

## Systems와 Features

- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/ResultMenu.md`

## Roadmap과 Template

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

## 현재 구현

- `Assets/Scripts/Runtime/Core/E_GameState.cs`
- `Assets/Scripts/Runtime/Core/GameRuntimeData.cs`
- `Assets/Scripts/Runtime/Systems/GameSystem.cs`
- `Assets/Scripts/Runtime/Systems/RuntimeDataSystem.cs`
- `Assets/Scripts/Runtime/Systems/StageSystem.cs`
- `Assets/Scripts/Runtime/Systems/PlayerMovementSystem.cs`
- `Assets/Scripts/Runtime/Systems/ResultSystem.cs`
- `Assets/Tests/EditMode`
- `Assets/Tests/PlayMode`
- `Assets/Scenes/SampleScene.unity`

확인된 구현 기준은 아래와 같다.

- System은 하나의 책임만 담당한다.
- Feature 규칙은 Feature 문서에서 관리하고 System 책임과 중복 작성하지 않는다.
- Runtime Data만 사용하며 로컬 또는 서버 저장을 추가하지 않는다.
- 이벤트는 발생 사실만 전달하며 등록한 Listener는 반드시 해제한다.
- Stage 종료는 `StageSystem`의 Stage 종료 이벤트를 기준으로 GameSystem에 전달한다.
- 자동화 가능한 검증은 Unity Test Runner를 우선 사용한다.
- 계산과 독립 상태 규칙은 Edit Mode Test로 검증한다.
- Scene, Component 참조, 실행 순서, 물리, Trigger와 Retry 통합 흐름은 Play Mode Test로 검증한다.
- 기존 Stage Mode 전체 Test를 회귀 검증으로 다시 실행해야 한다.

---

# 작업 내용

Phase 1 수동 수행 작업을 아래 Step으로 진행한다.

각 Step은 이전 Step의 완료 조건을 만족한 후 시작한다.

## Step 1. 미확정 InfiniteMode 규칙을 결정하고 문서에 반영한다

- 진행 상태: **완료**

### 수동 결정 항목

아래 항목은 현재 문서만으로 구현 방법을 하나로 결정할 수 없으므로 사용자가 먼저 확정한다.

1. Stage Mode와 InfiniteMode를 구분할 Mode 값과 명칭
2. 이번 Phase에서 Mode를 선택하거나 지정하는 방법
3. 게임 최초 실행 시 사용할 기본 Mode
4. Retry 시 유지할 Mode의 출처
5. 최소 이동 속도를 판정할 실제 속도 값의 종류
6. 최소 속도 미만을 즉시 종료로 볼지, 별도 유예 조건을 둘지 여부
7. InfiniteMode 시작 직후 최소 속도 판정을 시작할 시점
8. Stage 밖 이탈을 판단할 공간 경계와 Trigger 또는 판정 방식
9. Map Pattern 1개를 반복시키는 시점과 재배치 기준
10. Pattern 재배치 시 Player와 기존 지형의 충돌 안정성을 보장할 기준
11. InfiniteMode 종료 후 Phase 1 Result 화면에 표시할 최소 내용
12. 일반 Stage와 InfiniteMode를 같은 Scene에서 구성할지 별도 Scene 또는 Stage 구성으로 분리할지 여부

### 확정 결정

1. 게임 Mode는 `Stage`와 `Infinite`로 구분하고 구현 명칭은 `E_GameMode.Stage`, `E_GameMode.Infinite`를 사용한다.
2. Phase 1에서는 GameSystem의 Unity Inspector 설정으로 시작 Mode를 지정한다.
3. 최초 실행의 기본 Mode는 `Stage`이다.
4. GameSystem이 선택된 Mode를 보관하고 Retry 시 같은 Mode를 새 Runtime Data에 반영한다.
5. 최소 이동 속도 판정에는 수평 진행축 이동 속도의 절댓값을 사용한다.
6. 최소 이동 속도 미만인 상태가 프로젝트 설정 값으로 정의한 연속 유예 시간 이상 유지되면 종료한다.
7. 최소 이동 속도 판정은 프로젝트 설정 값으로 정의한 시작 유예 시간이 지난 후 시작한다. Stage 이탈 판정은 Stage Play 시작 직후부터 적용한다.
8. Stage 이탈은 Scene에 명시적으로 배치한 OutOfBounds Trigger 진입으로 판정한다. 아래쪽 경계는 필수로 사용하고 좌우 경계는 실제 Pattern 구조에 필요한 범위만 사용한다.
9. 하나의 Map Pattern 종류로 만든 인스턴스 2개를 교대로 재사용한다. Player가 앞 Pattern의 안전 기준점을 통과하면 후미 Pattern을 다음 연결 Anchor로 재배치한다.
10. Player가 후미 Pattern을 완전히 이탈하고 해당 Pattern과 접촉하지 않을 때만 재배치한다. 시작·종료 Anchor를 정렬하고 같은 경계에서 재배치를 한 번만 수행한다.
11. Phase 1 InfiniteMode Result에는 `Run Ended`, Retry와 Quit만 표시한다. Clear Time, 이동 거리와 Score는 표시하지 않는다.
12. `SampleScene` 안에서 `StageModeRoot`와 `InfiniteModeRoot`를 분리하고 현재 Mode의 Root만 활성화한다. Player, Systems, Camera와 공통 UI는 함께 사용한다.

시작 유예 시간, 최소 이동 속도 미만 연속 유예 시간과 최소 이동 속도의 실제 수치는 프로젝트 설정 값으로 관리한다. 수치 선택과 밸런스 검증은 별도 작업 범위이며 Phase 1에서는 설정 변경에 따라 판정 기준이 함께 변경되는 구조를 검증한다.

### 문서 반영

1. 확정한 기능 규칙은 `InfiniteMode.md` 또는 `StagePlay.md`의 기존 책임 범위에 반영한다.
2. 새 System 책임이 필요한 경우에만 관련 System 문서를 수정하거나 생성한다.
3. 프로젝트 전체 구조 결정이 바뀌는 경우에만 Project 문서를 수정한다.
4. 구현 방법과 작업 과정은 Feature 또는 System 문서에 기록하지 않는다.
5. 확인되지 않은 값은 임시 기본값으로 구현하지 않는다.

### 적극적 정적 검증

1. 확정한 각 규칙이 Project, Rules, Systems, Features와 Roadmap 중 정확히 한 책임 영역에만 정의되었는지 확인한다.
2. `rg`로 `InfiniteMode`, `StagePlay`, `Goal`, `Retry`, `Stage 종료` 관련 문구를 검색하여 충돌하는 정의가 없는지 확인한다.
3. Roadmap Phase 1 범위에 ScoreRecord, GamePause, 자동 이동, Collectible 또는 다중 Pattern이 섞이지 않았는지 확인한다.
4. 문서에 구현 Class, Method 또는 Scene 계층 같은 구현 방법을 잘못 추가하지 않았는지 확인한다.

### 완료 조건

- [x] 모든 수동 결정 항목이 사용자 결정 또는 기존 문서 근거로 확정되었다.
- [x] 관련 Feature와 System 문서가 확정 내용과 모순되지 않는다.
- [x] 미확정 규칙을 추측한 구현 항목이 남아 있지 않다.

## Step 2. 현재 구조의 변경 지점을 정적으로 확정한다

- 진행 상태: **완료**

### 수행 절차

1. 현재 `GameSystem`, `StageSystem`, `RuntimeDataSystem`, `PlayerMovementSystem`과 `ResultSystem`의 Public API와 직렬화 참조를 목록화한다.
2. `SampleScene.unity`에서 GameSystem과 각 System의 fileID 참조를 확인한다.
3. 기존 Goal 기반 종료와 Retry 흐름을 호출 순서로 정리한다.
4. 기존 Test가 직접 사용하는 Public Property, Method, 오브젝트 이름과 Scene 이름을 확인한다.
5. 기존 책임을 확장해 해결할 수 있는 항목과 새로운 책임이 필요한 항목을 구분한다.
6. Mode, InfiniteMode Run 상태와 Pattern 상태 중 System 간 공유가 필요한 데이터만 Runtime Data 후보로 분류한다.
7. System 내부에서만 사용하는 상태는 Runtime Data에 넣지 않는다.
8. 기존 Stage Mode API와 Test를 불필요하게 변경하지 않는 최소 수정 범위를 확정한다.

### 적극적 정적 검증

1. `rg --files Assets/Scripts Assets/Tests`로 생산 코드와 Test 파일 목록을 확보한다.
2. `rg -n`으로 `StartGame`, `StartStage`, `StopStage`, `HandleStageEnded`, `StageGoal`, `CreateResultData`와 Retry 호출 위치를 확인한다.
3. Scene YAML에서 `GameSystem`, `StageSystem`, `Goal`, `StartPoint`, `StageHUD`와 `ResultPanel`의 존재 개수와 직렬화 참조를 확인한다.
4. Test Attribute 수와 기존 Test Fixture 목록을 기록한다.
5. 변경 예정 파일이 Phase 1 책임과 직접 관련되는지 하나씩 확인한다.

### 완료 조건

- [x] 수정 대상과 유지 대상이 파일 단위로 구분되었다.
- [x] 기존 Stage Mode 회귀 지점이 Test 단위로 식별되었다.
- [x] 동일 책임의 중복 System 또는 Feature를 추가하지 않는 구조가 정해졌다.

### Step 2 정적 조사 결과

#### 현재 시작과 종료 호출 순서

현재 Stage Mode 시작 흐름은 아래 순서이다.

1. `GameSystem.Start()`가 `StartGame()`을 호출한다.
2. GameSystem이 필수 System 참조를 검사한다.
3. RuntimeDataSystem이 `GameRuntimeData`를 생성한다.
4. UIManagementSystem과 ResultSystem을 초기화한다.
5. PlayerControllerSystem, CollisionSystem, StageSystem, PlayerMovementSystem과 CameraSystem을 초기화한다.
6. PlayerInputSystem과 UIInputSystem을 초기화한다.
7. StageSystem의 Stage 종료 Listener를 등록한다.
8. `StageSystem.StartStage()`가 Goal 상태를 초기화하고 Stage를 시작한다.
9. Play Timer를 생성하고 시작한다.
10. Player Action Map과 Camera Follow를 활성화한다.
11. 게임 상태를 `Playing`으로 변경한다.

현재 Goal 종료와 Retry 흐름은 아래 순서이다.

1. StageGoal이 Player Trigger 진입을 감지한다.
2. StageSystem이 Clear와 End를 각각 한 번 확정하고 Stage 종료 이벤트를 발생시킨다.
3. GameSystem이 Play Timer를 정지한다.
4. Stage가 Clear된 경우 ResultSystem이 Clear Time Result Data를 생성한다.
5. UIManagementSystem이 Result Data를 Result UI에 반영한다.
6. GameSystem이 입력, Camera, Player 이동과 Stage를 중단하고 Runtime Data와 Timer를 제거한다.
7. 게임 상태가 `Ended`가 되고 ResultMenu 입력을 처리한다.
8. Retry 선택 시 `GameSystem.StartGame()`을 다시 호출한다.
9. 새 Runtime Data, Stage, Timer, Player 위치와 Result 상태를 초기화한다.

#### 현재 Public API 계약

Phase 1에서 유지해야 하는 주요 Public API는 아래와 같다.

| 대상 | 현재 Public API | 유지 이유 |
| --- | --- | --- |
| GameSystem | `CurrentGameState`, `StartGame()`, `EndGame()` | Lifecycle 및 ResultMenu Test가 직접 사용한다. |
| RuntimeDataSystem | `HasRuntimeData`, `RuntimeData`, `CreateRuntimeData()`, `GetRuntimeData()`, `ClearRuntimeData()` | GameSystem과 기존 Integration Test가 사용한다. |
| StageSystem | `IsInitialized`, `IsPlaying`, `IsCleared`, `HasEnded`, `Initialize()`, `StartStage()`, `StopStage()` | Stage 및 Lifecycle Test가 상태와 재시작을 검증한다. |
| StageSystem | Stage Started, Cleared, Ended Listener 등록과 해제 Method | GameSystem과 Stage Test가 종료 1회를 검증한다. |
| PlayerMovementSystem | `IsRunning`, `Initialize()`, `StopMovement()` | Lifecycle Test가 시작과 종료 상태를 검증한다. |
| ResultSystem | `HasResultData`, `CurrentResultData`, `Initialize()`, `CreateResultData()` | 일반 Stage Clear Time Test가 사용한다. |
| UIManagementSystem | `CurrentUIState`, `CurrentResultMenuSelection`, UI State·Result Data·ResultMenu 선택 Method | ResultMenu Test가 직접 사용한다. |

`PlayerMovementRuntimeData.CurrentHorizontalSpeed`가 이미 존재하므로 InfiniteMode의 수평 속도 판정을 위해 PlayerMovementSystem에 중복 속도 API를 추가하지 않는다.

#### 저장된 SampleScene 상태

- GameSystem, RuntimeDataSystem, StageSystem, PlayerMovementSystem, ResultSystem과 UIManagementSystem은 각각 정확히 1개 존재한다.
- Player, StartPoint, Goal, StageHUD, ResultPanel, RetryButton과 QuitButton의 기존 생산 오브젝트가 존재한다.
- GameSystem의 RuntimeDataSystem, UIManagementSystem, PlayerMovementSystem, StageSystem과 ResultSystem 참조는 0이 아닌 fileID로 연결되어 있다.
- StageSystem의 StageGoal 참조는 0이 아닌 fileID로 연결되어 있다.
- UIManagementSystem의 StageHUD, ResultPanel, Clear Time Text, RetryButton과 QuitButton 참조는 0이 아닌 fileID로 연결되어 있다.
- `StageModeRoot`와 `InfiniteModeRoot`는 아직 존재하지 않는다.
- InfiniteMode Pattern, Pattern Anchor와 OutOfBounds Trigger는 아직 존재하지 않는다.

#### 기존 자동 Test 기준

- Edit Mode Test는 8개 Fixture, Test Attribute 기준 39개이다.
- Play Mode Test는 10개 Fixture, Test Attribute 기준 34개이다.
- `StageSystemTests`는 StageGoal 필수 참조, Goal 종료 1회, 시작 전 Goal 무시와 Clear 후 재시작을 검증한다.
- `StageGoalIntegrationTests`는 `SampleScene`, Player, Goal, StartPoint, StageHUD, ResultPanel과 Clear Time Text 이름을 직접 사용한다.
- `GameLifecycleIntegrationTests`는 `StartGame()`, `EndGame()`, `CurrentGameState`, Runtime Data, 입력, Player 이동, Stage, Camera와 UI 상태를 직접 검증한다.
- `ResultMenuIntegrationTests`는 RetryButton, QuitButton과 같은 실행 세션의 Retry를 직접 검증한다.
- `StageCollisionConfigurationTests`는 Goal의 Trigger와 Layer 설정, Ground 구성을 검증한다.
- 기존 Stage Mode 오브젝트 이름과 Public API를 변경하면 다수의 회귀 Test에 직접 영향을 준다.
- Edit Mode Test Assembly는 Runtime Core와 Runtime Features Assembly를 직접 참조하므로 `InfiniteModeStateTests`에서 생산 Feature 코드를 직접 검증할 수 있다.
- Play Mode Test Assembly는 Runtime Core만 직접 참조하고 기존 System 통합 Test는 Reflection을 사용하므로 신규 System 통합 Test도 현재 참조 방향을 유지한다.

#### 확정한 수정 대상

| 파일 또는 영역 | 변경 목적 |
| --- | --- |
| `Assets/Scripts/Runtime/Core/E_GameMode.cs` 신규 | `Stage`와 `Infinite` Mode를 중복 없이 표현한다. |
| `Assets/Scripts/Runtime/Core/GameRuntimeData.cs` | 현재 Run의 Game Mode를 공유 Runtime Data로 제공하고 Clear 시 제거한다. |
| `Assets/Scripts/Runtime/Systems/GameSystem.cs` | Inspector 시작 Mode, 기본 Stage Mode, Runtime Data 및 StageSystem 전달과 Retry Mode 유지를 담당한다. |
| `Assets/Scripts/Runtime/Systems/StageSystem.cs` | Mode별 Stage Root 상태, Stage Mode Goal과 InfiniteMode 종료 사실을 하나의 Stage 종료 계약으로 관리한다. |
| `Assets/Scripts/Runtime/Features/InfiniteModeState.cs` 신규 | 시작 유예, 최소 속도 연속 유예, 이탈과 종료 1회 상태를 Scene 비의존 Unit Test 대상으로 관리한다. |
| `Assets/Scripts/Runtime/Features/InfiniteMapPattern.cs` 신규 | 동일 Pattern 인스턴스 2개의 안전 기준점, 후미 Pattern 재배치와 Retry 초기화를 담당한다. |
| `Assets/Scripts/Runtime/Features/StageOutOfBounds.cs` 신규 | Player가 이탈 영역에 진입한 사실만 전달한다. InfiniteMode 종료 규칙은 담당하지 않는다. |
| `Assets/Scripts/Runtime/Systems/UIManagementSystem.cs` | Mode에 따라 Clear Time 또는 `Run Ended`를 표시하고 기존 ResultMenu 선택 상태를 유지한다. |
| `Assets/Scenes/SampleScene.unity` | StageModeRoot, InfiniteModeRoot, Pattern 인스턴스 2개, Anchor와 OutOfBounds Trigger를 구성한다. |
| `Assets/Tests/EditMode/InfiniteModeStateTests.cs` 신규 | InfiniteMode의 순수 상태, 경계값, 중복 종료와 Retry 초기화를 검증한다. |
| `Assets/Tests/PlayMode/StageSystemTests.cs` | 기존 Stage Mode 계약을 유지하면서 Mode별 초기화와 종료 경로를 추가 검증한다. |
| `Assets/Tests/PlayMode/GameLifecycleIntegrationTests.cs` | Mode 전달, InfiniteMode 종료와 같은 Mode Retry의 System 통합 흐름을 검증한다. |
| `Assets/Tests/PlayMode/InfiniteModeIntegrationTests.cs` 신규 | 생산 Scene의 Pattern 반복, OutOfBounds, Goal 미사용과 Retry 초기화를 검증한다. |
| `Assets/Tests/PlayMode/ResultMenuIntegrationTests.cs` | InfiniteMode의 `Run Ended`, Retry와 Quit 흐름을 추가 검증한다. |

#### 유지 대상과 변경 금지 기준

- PlayerInputSystem, UIInputSystem, CollisionSystem, PlayerControllerSystem, CameraSystem과 TimerSystem의 책임 및 Public API는 Phase 1에서 변경하지 않는다.
- Jump, MomentumLanding, NormalLanding, CameraFollow와 TimeRecord 계산 규칙은 변경하지 않는다.
- ResultSystem과 ResultData의 Stage Mode Clear Time 계약은 유지한다.
- InfiniteMode Phase 1은 Clear Time Result Data를 생성하지 않으므로 ResultSystem에 Score나 이동 거리 책임을 추가하지 않는다.
- 기존 `SampleScene` 이름과 Player, StartPoint, Goal, StageHUD, ResultPanel, RetryButton, QuitButton 및 Clear Time Text 이름을 유지한다.
- 기존 Stage Mode Test를 삭제, Ignore 또는 기대값 완화로 통과시키지 않는다.

#### 책임 배치 결론

- GameSystem은 Mode를 선택하고 Retry 동안 선택 Mode를 유지한다.
- GameRuntimeData는 현재 Run의 Mode만 공유하며 종료 시 다른 Runtime 상태와 함께 제거된다.
- StageSystem은 Mode별 Stage Object 상태와 Stage 종료 이벤트를 관리하지만 InfiniteMode의 속도 규칙을 정의하지 않는다.
- InfiniteMode 순수 규칙은 속도와 이탈 상태를 입력받아 진행 가능 여부만 판단한다.
- PlayerMovementSystem은 기존처럼 이동 결과와 Runtime 수평 속도만 제공한다.
- OutOfBounds Stage Object는 이탈 사실만 전달한다.
- Pattern 동작 코드는 Pattern 배치와 초기화만 담당하며 Stage 종료를 결정하지 않는다.
- UIManagementSystem은 전달받은 Mode와 UI State에 맞는 결과 표시만 담당한다.
- ResultSystem은 Phase 1에서 일반 Stage Clear Time Result 계약을 그대로 유지한다.

#### Step 2 수동 작업

없음.

현재 구조의 Public API, Scene 오브젝트, 직렬화 참조, Test 수와 회귀 지점은 저장된 코드와 Scene YAML을 사용한 정적 검증으로 확인했다.

Unity Editor 실행, Scene 수동 확인과 Build는 Step 2 완료에 필요하지 않다.

## Step 3. InfiniteMode의 순수 상태 규칙을 Unit Test로 먼저 작성한다

- 진행 상태: **완료**

### Test 우선 대상

확정된 설계를 기준으로 Scene과 MonoBehaviour에 의존하지 않는 규칙을 가능한 한 순수 C# 구조로 분리하고 Edit Mode Unit Test를 먼저 작성한다.

1. Mode 초기값과 Mode 변경 규칙
2. Stage Mode와 InfiniteMode의 종료 조건 분기
3. InfiniteMode에서 Goal 종료를 무시하는 규칙
4. 최소 속도 경계값 바로 아래, 같은 값과 바로 위의 판정
5. Stage 이탈 여부에 따른 종료 판정
6. 여러 종료 조건이 같은 시점에 들어와도 종료 결과가 한 번만 확정되는 규칙
7. 시작 전과 종료 후에는 종료 판정을 수행하지 않는 규칙
8. Retry 초기화 시 이전 Run 상태가 남지 않는 규칙
9. Pattern 반복 요청이 동일 경계에서 중복 생성되지 않는 규칙
10. Stage Mode에서는 InfiniteMode 전용 판정과 Pattern 반복이 수행되지 않는 규칙

### 수행 절차

1. 실패하는 Edit Mode Test를 먼저 추가한다.
2. Test 이름은 조건과 기대 결과가 드러나도록 작성한다.
3. 생산 계산식을 Test 코드에 복제하지 않고 생산 API의 결과를 검증한다.
4. 경계값과 중복 호출 Test를 반드시 포함한다.
5. 최소 생산 코드를 작성하여 Test를 통과시킨다.
6. 관련 Edit Mode Test Fixture만 먼저 실행한다.
7. 해당 Fixture가 통과하면 Edit Mode 전체 Test를 실행한다.
8. 실패 Test의 이름, 메시지와 Stack Trace를 기록하고 원인 확인 전 기대값을 임의로 바꾸지 않는다.

### Unit Test 작성 기준

- 한 Test는 하나의 규칙을 검증한다.
- 외부 Scene, 프레임, 물리와 실제 Input System이 필요하지 않은 규칙은 Edit Mode에 둔다.
- `TestCase`로 표현 가능한 경계값은 중복 Test Method 대신 입력 사례로 구성한다.
- 초기화, 종료와 Retry는 같은 객체의 반복 호출도 검증한다.
- 내부 구현 Field보다 외부에서 관찰 가능한 상태와 결과를 우선 검증한다.
- 단순 코드 줄 실행이 아니라 Feature 완료 조건을 판정하는 Assertion을 작성한다.

### 완료 조건

- [x] 확정된 독립 상태 규칙마다 대응하는 Edit Mode Unit Test가 있다.
- [x] 신규 Edit Mode Test가 모두 통과한다.
- [x] 기존 Edit Mode 전체 Test가 모두 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

### Step 3 구현 결과

아래 생산 코드를 추가했다.

- `Assets/Scripts/Runtime/Core/E_GameMode.cs`
- `Assets/Scripts/Runtime/Features/InfiniteModeState.cs`

`E_GameMode`는 `Stage`와 `Infinite`를 정의한다.

`InfiniteModeState`는 아래 Scene 비의존 상태 규칙만 담당한다.

- 기본 Mode는 Stage이다.
- Play 시작 전에는 Mode를 변경할 수 있다.
- Play 중에는 현재 Run의 Mode를 변경할 수 없다.
- InfiniteMode에서 수평 진행축 속도의 절댓값을 최소 속도와 비교한다.
- 시작 유예 시간 동안에는 속도 종료 판정을 수행하지 않는다.
- 최소 속도 미만 연속 시간을 누적하고 속도가 회복되면 누적 시간을 초기화한다.
- OutOfBounds는 시작 유예 시간과 관계없이 InfiniteMode를 종료한다.
- InfiniteMode에서 Goal 통지는 종료를 발생시키지 않는다.
- 종료 후 다른 종료 조건이 들어와도 두 번째 종료를 발생시키지 않는다.
- 시작 전과 Stage Mode에서는 InfiniteMode 전용 조건을 처리하지 않는다.
- 같은 Pattern 경계 ID의 반복 요청을 한 번만 허용한다.
- Reset은 Mode와 설정을 유지하고 Run 상태와 Pattern 경계 상태를 초기화한다.

Scene, Rigidbody, Trigger, 실제 Pattern Transform과 System 연결은 Step 3 범위에 포함하지 않았다.

### Step 3 Unit Test 결과

아래 Edit Mode Test를 먼저 추가했다.

- `Assets/Tests/EditMode/InfiniteModeStateTests.cs`

신규 Fixture는 Plain Test 17개와 TestCase 6개로 총 23개 NUnit Case를 정의한다.

검증 대상은 아래와 같다.

- Mode 기본값, 변경 성공과 Play 중 변경 거부
- 양수와 음수 수평 속도의 최소 속도 경계값
- 저속 연속 유예 시간의 직전값과 경계값
- 속도 회복 시 저속 누적 시간 초기화
- 시작 유예 시간
- 시작 유예 중 OutOfBounds 즉시 종료
- InfiniteMode Goal 무시
- 복수 종료 조건의 종료 1회 보장
- 시작 전 InfiniteMode 조건 무시
- Reset 후 Mode 유지와 Run 및 Pattern 상태 초기화
- 같은 Pattern 경계 요청 중복 방지
- Stage Mode의 속도, OutOfBounds와 Pattern 요청 무시

AI가 Unity Test Runner의 신규 Fixture 실행을 Build 없이 시도했으나 Unity Editor 라이선스가 활성화되어 있지 않아 Test가 시작되기 전에 종료되었다.

확인된 로그는 아래와 같다.

```text
No valid Unity Editor license found. Please activate your license.
Application will terminate with return code 198
```

AI의 실행에서는 Test Result XML이 생성되지 않았다.

이후 사용자가 Unity Editor에서 Script Compilation과 Unity Test Runner 검증을 수행했다.

사용자가 확인한 최종 결과는 아래와 같다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error와 Warning 없음
- Edit Mode Test 62개 시도, 62개 성공
- Edit Mode Test에서 예상하지 않은 Error와 Warning 없음
- Play Mode Test 34개 시도, 34개 성공
- Play Mode Test에서 예상하지 않은 Error와 Warning 없음

기존 Edit Mode 39개와 신규 23개를 합친 최종 Edit Mode Test 수는 62개로 확인되었다.

### Step 3 정적 검증 결과

- 신규 Class와 Enum의 파일명과 타입명이 일치한다.
- Runtime Core, Runtime Features와 Edit Mode Test의 기존 Assembly 참조 방향을 유지한다.
- `InfiniteModeState`는 UnityEngine, Scene과 MonoBehaviour에 의존하지 않는다.
- 생산 계산식을 Test에 복제하지 않고 생산 API의 반환 상태를 검증한다.
- 속도 경계값은 TestCase로 구성했다.
- 한 Test가 여러 Stage Mode 규칙을 함께 판정하지 않도록 분리했다.
- nullable `?`, null 조건부 `?.`와 null 병합 `??` 문법을 추가하지 않았다.
- 신규 `.cs` 파일마다 고유한 `.meta` GUID가 존재한다.
- 신규 파일의 공백 오류가 없다.
- 사용자의 Unity Editor 검증으로 Unity Script Compile 성공을 확인했다.

### Step 3 수동 작업

없음.

사용자가 Unity Script Compilation, Edit Mode 전체 62개와 Play Mode 전체 34개의 성공을 확인했다.

Build는 Step 3 검증에 포함하지 않았다.

## Step 4. Mode와 Runtime 상태를 구현한다

- 진행 상태: **완료**

### 수행 절차

1. 확정된 Mode 표현을 Runtime Core에 추가한다.
2. 현재 Mode처럼 System 간 공유가 필요한 값만 `GameRuntimeData` 또는 책임에 맞는 Runtime Data에 추가한다.
3. Run 시작 시 Mode와 InfiniteMode 상태를 명시적으로 초기화한다.
4. Retry 시 같은 Mode를 유지하되 이전 Run의 상태는 초기화한다.
5. Stage Mode 기본 흐름이 기존 동작을 유지하도록 분기를 최소화한다.
6. 중복 시작, 종료 후 재종료와 잘못된 상태 요청을 방어한다.
7. 정상 프레임마다 반복 로그를 남기지 않는다.
8. 폴백이 필요한 경우 Warning을 남기고, 기능 수행 불가 시 Error를 남긴다.

### 적극적 정적 검증

1. Mode를 나타내는 중복 Enum, bool 또는 문자열이 여러 위치에 생기지 않았는지 검색한다.
2. Runtime Data에 System 내부 상태가 들어가지 않았는지 검토한다.
3. 저장, PlayerPrefs, 파일 또는 서버 API가 추가되지 않았는지 검색한다.
4. 신규 Public API가 실제 호출 또는 Test 근거 없이 노출되지 않았는지 확인한다.
5. 기존 Stage Mode 분기가 기본 동작을 유지하는지 코드 경로를 대조한다.

### Unit Test

1. Step 3의 Mode 및 초기화 Test를 다시 실행한다.
2. Runtime Data 생성, 제거와 재생성 Test를 추가한다.
3. Stage Mode → Retry와 InfiniteMode → Retry를 각각 독립적으로 검증한다.
4. 잘못된 순서의 요청이 상태를 오염시키지 않는지 검증한다.

### 완료 조건

- [x] 현재 Mode를 한 곳의 명확한 Runtime 상태로 확인할 수 있다.
- [x] Retry 후 Mode는 유지되고 이전 Run 상태는 제거된다.
- [x] Runtime Data 책임과 기존 저장 제약을 위반하지 않는다.
- [x] 관련 Unit Test와 기존 Edit Mode 전체 Test가 통과한다.

### Step 4 구현 결과

아래 코드를 변경했다.

- `Assets/Scripts/Runtime/Core/GameRuntimeData.cs`
- `Assets/Scripts/Runtime/Systems/RuntimeDataSystem.cs`
- `Assets/Scripts/Runtime/Systems/GameSystem.cs`

구현 결과는 아래와 같다.

- `GameRuntimeData.GameMode`에서 현재 Run의 Mode를 확인할 수 있다.
- `GameRuntimeData.Initialize()`는 기존 호환성을 위해 Stage Mode로 초기화한다.
- `GameRuntimeData.Initialize(E_GameMode)`는 요청받은 Mode로 새 Run 상태를 초기화한다.
- GameRuntimeData는 Mode를 생성 시점에만 받고 실행 중 변경 Setter를 제공하지 않는다.
- GameRuntimeData Clear 시 Game State, Game Mode, UI State와 Player Movement Runtime Data를 초기 상태로 제거한다.
- 생성 전 또는 Clear 후 Game State와 UI State 변경 요청은 Runtime 상태를 변경하지 않는다.
- `RuntimeDataSystem.CreateRuntimeData()`는 기존 Stage Mode 기본 동작을 유지한다.
- `RuntimeDataSystem.CreateRuntimeData(E_GameMode)`는 요청 Mode로 Runtime Data를 생성한다.
- GameSystem은 Inspector에 노출한 `_selectedGameMode`를 보관한다.
- `_selectedGameMode`의 기본값은 `E_GameMode.Stage`이다.
- GameSystem은 새 Run마다 선택 Mode를 RuntimeDataSystem에 전달한다.
- Retry 시 Runtime Data는 새로 생성하지만 GameSystem의 선택 Mode는 유지한다.
- 정상 프레임 반복 로그, 저장 기능과 신규 폴백은 추가하지 않았다.

`InfiniteModeState`의 최소 속도, 시작 유예 시간과 저속 연속 유예 시간 설정 및 StageSystem 연결은 Step 5에서 수행한다. Step 4에서는 확인되지 않은 실제 수치를 추가하지 않았다.

### Step 4 Unit Test 추가 결과

아래 Edit Mode Test를 추가했다.

- `Assets/Tests/EditMode/GameRuntimeDataTests.cs`

신규 Test는 Plain Test 4개와 TestCase 2개로 총 6개 NUnit Case를 정의한다.

검증 대상은 아래와 같다.

- Mode 인자 없는 초기화의 Stage Mode 기본값
- InfiniteMode 초기화와 현재 Run Mode 저장
- Clear 시 모든 Runtime 상태 제거
- 생성 전 상태 변경 요청 무시
- Stage Mode Clear 후 같은 Mode의 새 Run 초기화
- InfiniteMode Clear 후 같은 Mode의 새 Run 초기화
- 이전 Run의 Player Movement Runtime Data가 새 Run에 남지 않음

Step 3 완료 기준 Edit Mode 62개에 신규 6개를 더한 예상 Edit Mode 전체 Test 수는 68개이다.

AI는 사용자 요청에 따라 Unity Test Runner를 실행하거나 시도하지 않았다.

이후 사용자가 Unity Editor에서 Script Compilation과 Unity Test Runner 검증을 수행했다.

사용자가 확인한 최종 결과는 아래와 같다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error와 Warning 없음
- Edit Mode Test 68개 시도, 68개 성공
- Edit Mode Test에서 예상하지 않은 Error와 Warning 없음
- Play Mode Test 34개 시도, 34개 성공
- Play Mode Test에서 예상하지 않은 Error와 Warning 없음

### Step 4 정적 검증 결과

- `E_GameMode` Enum 정의는 Runtime Core에 1개만 존재한다.
- Mode를 표현하는 중복 bool 또는 문자열 상태를 추가하지 않았다.
- GameRuntimeData에는 System 내부 상태가 아니라 현재 Run의 공유 Mode만 추가했다.
- PlayerMovementSystem의 기존 수평 속도 Runtime Data를 변경하거나 중복하지 않았다.
- PlayerPrefs, 파일, 서버, Save 또는 Load API를 추가하지 않았다.
- 기존 `CreateRuntimeData()`와 `GameRuntimeData.Initialize()` API를 유지했다.
- 기존 Stage Mode의 기본 실행 경로는 Mode 인자가 없는 경우와 GameSystem 기본 설정 모두 Stage Mode이다.
- 신규 Test는 Runtime Core 생산 API를 직접 사용하고 계산식을 복제하지 않는다.
- 신규 `.cs` 파일의 `.meta` GUID는 고유하다.
- 변경 파일에서 공백 오류가 확인되지 않았다.

### Step 4 수동 작업

없음.

사용자가 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 34개의 성공을 확인했다.

Build는 Step 4 검증에 포함하지 않았다.

## Step 5. StageSystem에 Mode별 시작과 종료 흐름을 연결한다

- 진행 상태: **완료**

### 수행 절차

1. StageSystem이 현재 Mode에 필요한 Stage 구성만 초기화하도록 확장한다.
2. 일반 Stage에서는 기존 `StageGoal` 초기화와 Goal 종료를 유지한다.
3. InfiniteMode에서는 Goal을 요구하거나 Goal 이벤트를 종료 조건으로 사용하지 않도록 한다.
4. InfiniteMode의 확정된 진행 지속 조건을 입력받아 Stage 종료 시점만 판단한다.
5. Stage 종료 이벤트는 하나의 Run에서 한 번만 발생하도록 유지한다.
6. Stage 종료 원인이나 결과 정보가 다른 System에 필요하다면 최소 데이터만 전달한다.
7. Listener 등록과 해제 위치를 확인하고 중복 등록을 방지한다.
8. Stop과 Retry 뒤 내부 Stage 상태를 새 Run 기준으로 초기화한다.

### 적극적 정적 검증

1. `StageGoal` 직접 참조가 Stage Mode 경로에만 남는지 검색한다.
2. InfiniteMode Feature 규칙이 StageSystem 내부에 중복 정의되지 않았는지 확인한다.
3. 이벤트가 `Action` 또는 프로젝트 규칙에 맞는 캡슐화 방식이며 외부 호출이 불가능한지 확인한다.
4. 모든 AddListener에 대응하는 RemoveListener가 있는지 확인한다.
5. 핵심 게임 흐름이 긴 이벤트 체인으로 숨겨지지 않았는지 확인한다.

### Unit Test와 Play Mode Test

1. Stage Mode 초기화에는 Goal이 필요하고 InfiniteMode 초기화에는 Goal이 필요하지 않음을 검증한다.
2. Stage Mode의 Goal 도달이 기존처럼 Clear와 End를 한 번 발생시키는지 검증한다.
3. InfiniteMode의 Goal 접촉이 종료를 발생시키지 않는지 검증한다.
4. 최소 속도 미만과 Stage 이탈이 각각 InfiniteMode End를 한 번 발생시키는지 검증한다.
5. 시작 전 종료 신호와 종료 후 중복 신호가 무시되는지 검증한다.
6. End 후 Start가 새 상태로 정상 시작되는지 검증한다.

### 완료 조건

- [x] Mode별 Stage 시작과 종료 조건이 분리되어 있다.
- [x] InfiniteMode가 Goal 없이 시작된다.
- [x] InfiniteMode 종료 이벤트는 한 Run에서 한 번만 발생한다.
- [x] 기존 StageSystem Test와 신규 Test가 모두 통과한다.

### Step 5 구현 결과

아래 코드를 변경했다.

- `Assets/Scripts/Runtime/Systems/StageSystem.cs`
- `Assets/Scripts/Runtime/Systems/GameSystem.cs`

StageSystem 변경 결과는 아래와 같다.

- 기존 `Initialize()`는 Stage Mode 기본 초기화 API로 유지한다.
- `Initialize(E_GameMode)` Overload로 현재 Run의 Mode를 전달받는다.
- `CurrentGameMode`에서 StageSystem이 초기화된 Mode를 확인할 수 있다.
- Stage Mode 초기화는 기존처럼 StageGoal 참조와 StageGoal 초기화를 필수로 요구한다.
- InfiniteMode 초기화는 StageGoal 참조를 요구하거나 StageGoal Listener를 등록하지 않는다.
- Mode 재초기화 전에 기존 StageGoal Listener를 제거하여 중복 등록을 방지한다.
- Stage Mode에서만 Stage 시작 시 Goal 상태를 초기화한다.
- Stage Mode에서만 Goal 도달을 Clear와 Stage End로 처리한다.
- `TryEndInfiniteStage()`는 초기화된 InfiniteMode가 Playing 상태일 때만 Stage를 종료한다.
- 시작 전, Stage Mode, 이미 종료된 상태의 Infinite 종료 요청은 거부한다.
- InfiniteMode 종료는 Clear를 발생시키지 않는다.
- Stage End 이벤트는 기존 `_hasEnded` Guard를 사용하여 한 Run에서 한 번만 발생한다.
- Stop 또는 End 후 `StartStage()`를 다시 실행하면 Stage 상태를 새 Run 기준으로 초기화한다.
- Stage Started, Cleared와 Ended 이벤트는 private 상태를 유지한다.
- 모든 Listener 등록 Method에 대응하는 해제 Method를 유지한다.

GameSystem은 StageSystem 초기화 시 Step 4에서 보관한 `_selectedGameMode`를 전달한다.

InfiniteMode의 최소 속도와 OutOfBounds 규칙은 Step 3의 `InfiniteModeState`가 판단한다. 해당 규칙의 종료 결과를 `TryEndInfiniteStage()`에 연결하는 실행 흐름은 Step 7에서 구현한다.

### Step 5 Test 추가 결과

아래 Play Mode Test를 변경했다.

- `Assets/Tests/PlayMode/StageSystemTests.cs`

기존 6개 Test를 유지하고 아래 신규 7개 Test를 추가했다.

- Stage Mode에서 Goal 참조가 없으면 초기화 거부
- InfiniteMode에서 Goal 참조 없이 초기화 성공
- InfiniteMode에서 Goal 도달을 Stage 종료로 처리하지 않음
- InfiniteMode 시작 전 종료 요청 거부
- Stage Mode에서 Infinite 종료 요청 거부
- Playing InfiniteMode의 종료 요청 1회 처리
- InfiniteMode End 후 Start의 Stage 상태 초기화

Step 4 완료 기준 Play Mode 34개에 신규 7개를 더한 예상 Play Mode 전체 Test 수는 41개이다.

Edit Mode Test는 변경하지 않았으며 예상 전체 Test 수는 68개이다.

AI는 사용자 요청에 따라 Unity Test Runner를 실행하거나 시도하지 않았다.

이후 사용자가 Unity Editor에서 Script Compilation과 Unity Test Runner 검증을 수행했다.

사용자가 확인한 최종 결과는 아래와 같다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error와 Warning 없음
- Edit Mode Test 68개 시도, 68개 성공
- Edit Mode Test에서 예상하지 않은 Error와 Warning 없음
- Play Mode Test 41개 시도, 41개 성공
- Play Mode Test에서 예상하지 않은 Error와 Warning 없음

### Step 5 정적 검증 결과

- StageGoal 초기화, Reset과 Goal 종료 처리는 Stage Mode 분기 안에 있다.
- InfiniteMode 초기화는 StageGoal을 요구하지 않는다.
- StageGoal Listener는 Mode 초기화 전과 OnDestroy에서 제거한다.
- StageGoal Listener 등록은 Stage Mode 초기화 성공 후에만 수행한다.
- Stage Started, Cleared와 Ended 이벤트는 외부에 직접 노출되지 않는다.
- AddListener와 RemoveListener API가 대응한다.
- InfiniteMode의 속도, 유예 시간과 OutOfBounds 규칙을 StageSystem에 중복 구현하지 않았다.
- 기존 `Initialize()`, `StartStage()`, `StopStage()`와 상태 Property를 유지했다.
- GameSystem의 Stage Mode 기본 실행 경로를 유지했다.
- 변경한 생산 코드와 Test에 nullable `?`, null 조건부 `?.` 또는 null 병합 `??` 문법을 추가하지 않았다.
- 변경 파일에서 공백 오류가 확인되지 않았다.

### Step 5 수동 작업

없음.

사용자가 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 41개의 성공을 확인했다.

Build는 Step 5 검증에 포함하지 않았다.

## Step 6. InfiniteMode 전용 Map Pattern 1개를 구현하고 반복 연결한다

- 진행 상태: **완료**

### 수행 절차

1. Step 1에서 확정한 Scene 또는 Stage 구성 방식에 따라 InfiniteMode 전용 구성을 만든다.
2. Pattern 하나가 Player의 현재 이동 기능으로 통과 가능한 지형인지 확인한다.
3. Goal과 Stage Mode 전용 종료 Trigger를 InfiniteMode 구성에 포함하지 않는다.
4. 확정한 기준 지점에서 동일 Pattern을 다음 위치로 재배치하거나 재사용한다.
5. 이미 지난 Pattern만 재사용하고 Player가 서 있거나 충돌 중인 Pattern은 이동하지 않는다.
6. 한 경계 통과가 한 번의 Pattern 갱신만 발생시키도록 한다.
7. Retry 시 Pattern 위치와 반복 상태를 최초 상태로 되돌린다.
8. 다중 Pattern 선택, 난이도 증가, 랜덤 조합과 Object Pool 확장은 추가하지 않는다.

### Unity Editor 수동 작업

1. InfiniteMode용 Root와 Pattern 오브젝트를 식별 가능한 이름으로 구성한다.
2. Pattern의 Collider Layer와 Trigger 설정을 기존 Player 및 Ground 설정과 대조한다.
3. 필요한 Component를 정확히 한 번씩 부착한다.
4. Inspector의 필수 참조를 연결한다.
5. Pattern 시작점, 끝점과 반복 기준 Transform을 Scene View에서 확인한다.
6. Mode별로 Goal과 InfiniteMode 전용 오브젝트가 올바르게 활성화되는지 확인한다.
7. Scene 또는 관련 Asset을 저장한다.

### 적극적 정적 검증

1. 저장된 Scene YAML에서 InfiniteMode Root, Pattern, 기준점과 Component의 존재 개수를 확인한다.
2. 필수 Serialized Field의 fileID가 `0`이 아닌지 확인한다.
3. Goal이 InfiniteMode 구성에 연결되지 않았는지 확인한다.
4. 동일 책임 Component가 중복 부착되지 않았는지 확인한다.
5. 새 Script와 Test의 `.meta` 파일이 함께 존재하는지 확인한다.

### Play Mode Test

1. 생산 Scene 또는 생산 구성과 같은 조건에서 InfiniteMode를 시작한다.
2. Pattern 경계를 통과하면 다음 진행 구간이 준비되는지 검증한다.
3. 같은 경계에서 Pattern 갱신이 중복 수행되지 않는지 검증한다.
4. Pattern 반복 후에도 Player와 Ground 충돌이 유지되는지 검증한다.
5. Retry 후 Pattern 위치와 상태가 최초 값으로 복원되는지 검증한다.
6. Stage Mode Scene의 Goal과 지형 구성이 변경되지 않았는지 회귀 검증한다.

### 완료 조건

- [x] InfiniteMode에서 하나의 Map Pattern으로 진행을 반복할 수 있다.
- [x] Pattern 반복 시 지형 공백, 중복 배치와 Player 강제 이동이 발생하지 않는다.
- [x] Retry 후 최초 Pattern 상태가 복원된다.
- [x] Scene 정적 검증과 관련 Play Mode Test가 통과한다.

### Step 6 코드 구현 결과

아래 생산 코드를 추가했다.

- `Assets/Scripts/Runtime/Features/InfiniteMapPattern.cs`
- `Assets/Scripts/Runtime/Features/InfinitePatternBoundary.cs`

아래 생산 코드를 변경했다.

- `Assets/Scripts/Runtime/Systems/StageSystem.cs`

`InfiniteMapPattern`의 책임은 아래와 같다.

- 동일한 Map Pattern 종류로 만든 두 Pattern 인스턴스를 관리한다.
- 첫 번째 Pattern을 초기 후미 Pattern, 두 번째 Pattern을 초기 선두 Pattern으로 관리한다.
- 현재 선두 Pattern의 Boundary 요청만 수락한다.
- 선두 Pattern Boundary 통과 시 후미 Pattern의 StartAnchor를 선두 Pattern의 EndAnchor에 정렬한다.
- 재배치 직후 `Physics.SyncTransforms()`로 이동한 Collider Transform을 물리 상태에 반영한다.
- 재배치된 Pattern의 Boundary 상태를 다음 사용을 위해 초기화한다.
- Pattern Boundary 요청은 첫 번째와 두 번째 ID를 교대로 수락한다.
- 최초 Pattern 위치와 회전을 보관한다.
- Reset 시 두 Pattern의 위치, 회전, Boundary 상태와 반복 횟수를 최초 상태로 복원한다.
- Pattern Transform만 이동하며 Player Transform을 변경하지 않는다.

`InfinitePatternBoundary`의 책임은 아래와 같다.

- 지정된 Player Collider의 Trigger 진입만 처리한다.
- 자신의 Boundary ID를 InfiniteMapPattern에 전달한다.
- InfiniteMapPattern이 요청을 수락한 경우 같은 활성 주기에서 중복 전달하지 않는다.
- Stage 종료, 속도 판정과 OutOfBounds를 처리하지 않는다.

StageSystem의 Mode Root 처리는 아래와 같다.

- StageModeRoot와 InfiniteModeRoot가 모두 연결되면 현재 Mode의 Root만 활성화한다.
- Root 두 개가 모두 없는 기존 Test 구성은 기존 동작을 유지한다.
- Root 하나만 연결된 잘못된 구성은 Error로 초기화를 중단한다.
- InfiniteMode Retry에서 이미 활성화된 InfiniteModeRoot를 비활성화한 뒤 다시 활성화한다.
- InfiniteMapPattern은 최초 활성화 후 Start에서 초기화한다.
- 최초 Transform이 확보된 이후 InfiniteModeRoot가 재활성화되면 Pattern 상태를 초기화한다.

Scene Trigger가 Pattern 관리자에 경계 통과 사실을 전달하려면 인스턴스별 Component가 필요하므로 Step 2 예상 목록에 없던 `InfinitePatternBoundary`를 추가했다. 이 Component는 Pattern 재배치나 Stage 종료를 직접 수행하지 않는다.

### Step 6 Test 추가 결과

아래 Test Assembly 참조를 확장했다.

- `Assets/Tests/PlayMode/FlowState.PlayModeTests.asmdef`

Play Mode Test가 분리된 Runtime Features 생산 Assembly를 직접 검증할 수 있도록 `FlowState.Runtime.Features` 참조를 추가했다.

아래 Play Mode Test를 추가했다.

- `Assets/Tests/PlayMode/InfiniteMapPatternTests.cs`

신규 7개 Test는 아래 항목을 검증한다.

- 유효한 Pattern 인스턴스 2개의 초기화
- 선두 Boundary 요청 시 후미 Pattern의 Anchor 정렬
- Pattern 재배치 후 Ground Collider 활성 상태 유지
- Pattern 재배치 시 Player Transform 불변
- 같은 Boundary 요청 중복 거부
- 두 Boundary의 교대 요청과 두 Pattern 재사용
- Reset 후 최초 Transform과 반복 상태 복원
- Player Trigger의 Pattern 갱신 1회 처리
- Player가 아닌 Collider의 Trigger 무시

`StageSystemTests`에는 아래 신규 2개 Test를 추가했다.

- Stage Mode에서 StageModeRoot만 활성화
- InfiniteMode에서 InfiniteModeRoot만 활성화

Step 5 완료 기준 Play Mode 41개에 신규 9개를 더한 예상 Play Mode 전체 Test 수는 50개이다.

Edit Mode Test는 변경하지 않았으며 예상 전체 Test 수는 68개이다.

AI는 사용자 요청에 따라 Unity Test Runner를 실행하거나 시도하지 않았다.

### Step 6 정적 검증 결과

- 신규 생산 Class는 파일별로 하나만 정의했다.
- InfiniteMapPattern과 InfinitePatternBoundary는 기존 Runtime Features Assembly에 포함된다.
- Play Mode Test Assembly의 JSON 구조와 Runtime Features 참조가 유효하다.
- Pattern Boundary ID는 첫 번째 `0`, 두 번째 `1`만 유효한 구성으로 검사한다.
- 현재 선두 Pattern의 Boundary ID와 일치하는 요청만 Pattern 재배치를 수행한다.
- 같은 Boundary ID의 연속 요청은 재배치를 수행하지 않는다.
- Pattern 재배치는 Anchor의 World Position 차이만큼 후미 Pattern을 이동한다.
- Player Transform을 변경하는 코드가 없다.
- Pattern 재배치 후 Physics Transform을 동기화한다.
- Reset은 위치, 회전, Boundary와 반복 횟수를 초기화한다.
- StageSystem은 Mode Root가 모두 연결된 경우에만 Root 활성 상태를 관리한다.
- 신규 Script와 Test에 고유한 `.meta` GUID가 존재한다.
- 변경한 코드와 Test에 nullable `?`, null 조건부 `?.` 또는 null 병합 `??` 문법을 추가하지 않았다.
- PlayerPrefs, 파일, 서버 또는 저장 API를 추가하지 않았다.
- 변경 파일에서 공백 오류가 확인되지 않았다.
- 저장된 SampleScene에서 StageModeRoot와 InfiniteModeRoot가 존재하고 StageSystem의 두 Inspector 참조가 연결된 것을 확인했다.
- 저장된 SampleScene에서 InfiniteMapPattern의 Pattern 2개, Start/End Anchor 4개와 Boundary 2개 참조가 모두 연결된 것을 확인했다.
- 두 InfinitePatternBoundary의 Player Collider와 Map Pattern 참조가 연결되고 Boundary ID가 각각 `0`, `1`인 것을 확인했다.
- StageModeRoot는 활성, InfiniteModeRoot는 비활성 상태로 저장된 것을 확인했다.

### Step 6 최초 Play Mode 검증 실패 및 조치

사용자가 `StageTerrainSurfaces_ProvideActualGroundContact`에서 `Platform_01`의 예상 표면 높이 `1.0`과 실제 접촉점 `0.0`이 달라 실패한 것을 확인했다.

InfiniteMode Pattern에 Stage 지형과 같은 이름의 비활성 복제본이 추가되었지만, 기존 Scene 검색 Test가 활성 계층을 구분하지 않은 것이 원인이었다. 비활성 복제본에서 예상 높이를 얻은 뒤 활성 Stage 지형에 대한 물리 접촉점과 비교하여 서로 다른 오브젝트의 값이 섞였다.

`StageCollisionConfigurationTests.FindSceneGameObject`가 `activeInHierarchy`인 오브젝트만 반환하도록 수정했다. 이에 따라 Stage Mode 검증은 실제 물리 판정에 참여하는 활성 Stage 지형만 대상으로 하며, 지형 높이 및 접촉점 검증 기준은 완화하지 않았다.

수정 후 사용자가 아래 최종 검증 결과를 확인했다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error 및 Warning 없음
- Edit Mode 전체 68개 Test 성공
- Edit Mode Test에서 예상하지 않은 Error 및 Warning 없음
- Play Mode 전체 50개 Test 성공
- Play Mode Test에서 예상하지 않은 Error 및 Warning 없음

이에 따라 Step 6 완료 조건을 모두 충족했다.

### Step 6 수동 Scene 작업

아래 작업은 실제 지형 크기, Collider와 Anchor 위치를 Unity Scene View에서 결정해야 하므로 수동 작업이 필요하다.

#### Mode Root 구성

1. `SampleScene`을 연다.
2. `World/Stage_01`의 이름을 `StageModeRoot`로 변경한다.
3. `World` 아래에 빈 GameObject `InfiniteModeRoot`를 생성한다.
4. `StageModeRoot`는 활성 상태, `InfiniteModeRoot`는 비활성 상태로 저장한다.
5. `Systems/StageSystem`의 Stage Mode Root에 `World/StageModeRoot`를 연결한다.
6. `Systems/StageSystem`의 Infinite Mode Root에 `World/InfiniteModeRoot`를 연결한다.
7. 기존 StartPoint, Terrain과 Goal은 `StageModeRoot` 아래에 그대로 유지한다.

#### InfiniteMode Pattern 구성

1. `InfiniteModeRoot` 아래에 빈 GameObject `InfiniteMapPattern`을 생성한다.
2. `InfiniteMapPattern` Component를 부착한다.
3. `InfiniteMapPattern` 아래에 `Pattern_0`을 생성한다.
4. `Pattern_0` 아래에 Player가 통과할 수 있는 Ground와 Platform을 구성한다.
5. 모든 지형 Collider는 Trigger를 해제하고 기존 `Ground` Layer를 사용한다.
6. `Pattern_0` 아래에 빈 Transform `StartAnchor`, `EndAnchor`를 생성한다.
7. StartAnchor는 Pattern의 연결 시작점, EndAnchor는 다음 Pattern의 StartAnchor가 연결될 끝점에 둔다.
8. `Pattern_0` 아래에 `AdvanceBoundary`를 생성한다.
9. AdvanceBoundary에 BoxCollider를 추가하고 `Is Trigger`를 활성화한다.
10. AdvanceBoundary가 Player가 이전 Pattern을 완전히 벗어난 뒤 통과하는 위치에 있도록 배치한다.
11. AdvanceBoundary의 Collider는 Ground Layer를 사용하지 않는다.
12. AdvanceBoundary에 `InfinitePatternBoundary` Component를 부착한다.
13. Boundary ID를 `0`으로 설정한다.
14. Player Collider에 `Player`의 CapsuleCollider를 연결한다.
15. Map Pattern에 `InfiniteModeRoot/InfiniteMapPattern`의 InfiniteMapPattern Component를 연결한다.
16. `Pattern_0` 전체를 복제하여 `Pattern_1`을 만든다.
17. `Pattern_1/StartAnchor`가 `Pattern_0/EndAnchor`와 정확히 같은 World Position이 되도록 `Pattern_1`을 이동한다.
18. `Pattern_1/AdvanceBoundary`의 Boundary ID를 `1`로 변경한다.
19. 두 Pattern의 회전과 Scale을 동일하게 유지한다.

#### InfiniteMapPattern Inspector 연결

1. First Pattern에 `Pattern_0`을 연결한다.
2. First Start Anchor와 First End Anchor에 `Pattern_0`의 Anchor를 연결한다.
3. First Boundary에 `Pattern_0/AdvanceBoundary`의 InfinitePatternBoundary를 연결한다.
4. Second Pattern에 `Pattern_1`을 연결한다.
5. Second Start Anchor와 Second End Anchor에 `Pattern_1`의 Anchor를 연결한다.
6. Second Boundary에 `Pattern_1/AdvanceBoundary`의 InfinitePatternBoundary를 연결한다.
7. 모든 필수 참조가 비어 있지 않은지 확인한다.
8. Goal 또는 StageGoal을 InfiniteModeRoot에 복제하거나 연결하지 않는다.
9. Scene을 저장한다.

#### Unity 자동 검증

Scene 저장 후 아래 자동 검증을 수행한다.

1. Unity Script Compilation 성공 여부와 예상하지 않은 Error 및 Warning을 확인한다.
2. Edit Mode 전체 68개 Test를 실행한다.
3. Play Mode 전체 50개 Test를 실행한다.
4. Test에서 예상하지 않은 Error와 Warning이 없는지 확인한다.

Build는 Step 6 검증에 포함하지 않는다.

Scene 구성이 완료되면 AI가 저장된 Scene YAML을 정적으로 검사하여 Root, Pattern, Anchor, Boundary, Layer와 Inspector 참조를 확인한다.

## Step 7. InfiniteMode 진행 지속 조건과 종료 흐름을 통합한다

- 진행 상태: **완료**

### 수행 절차

1. PlayerMovementSystem이 이미 제공하는 이동 결과를 사용하여 확정된 속도 값을 얻는다.
2. PlayerMovementSystem에 InfiniteMode 종료 규칙을 직접 넣지 않는다.
3. 확정된 Stage 이탈 판정에서 이탈 사실만 전달한다.
4. InfiniteMode 규칙이 진행 지속 조건을 평가한다.
5. 종료 조건이 충족되면 StageSystem이 Stage 종료 이벤트를 한 번 발생시킨다.
6. GameSystem은 기존 Stage 종료 이벤트 경로로 Player 입력, Camera, 이동과 Stage 진행을 중단한다.
7. Phase 1에서는 Score와 이동 거리 Result Data를 생성하지 않는다.
8. 일반 Stage의 TimeRecord와 Clear Result 경로는 유지한다.

### 적극적 정적 검증

1. 속도 설정이 PlayerMovementSystem 또는 확정한 설정 소유자 외에 중복 정의되지 않았는지 검색한다.
2. `Update` 또는 `FixedUpdate`에 정상 상태 반복 로그가 추가되지 않았는지 확인한다.
3. InfiniteMode 종료 후 TimeRecord 또는 Stage Clear가 잘못 생성되는 코드 경로가 없는지 확인한다.
4. ResultSystem이 Phase 1에서 ScoreRecord 책임을 미리 가지지 않는지 확인한다.
5. Mode별 종료 분기가 동일한 Stage 종료 이벤트 계약을 유지하는지 확인한다.

### Unit Test와 Play Mode Test

1. 속도 경계값과 Stage 이탈의 순수 판정은 Edit Mode Unit Test로 검증한다.
2. 실제 Rigidbody 이동 결과가 종료 판정으로 전달되는 흐름은 Play Mode Test로 검증한다.
3. 종료 시 Player 입력, 이동, Camera와 Stage 진행이 중단되는지 검증한다.
4. InfiniteMode 종료 시 Stage Clear와 Clear Time Result가 생성되지 않는지 검증한다.
5. 두 종료 조건이 같은 프레임에 발생해도 종료 이벤트와 게임 종료가 한 번인지 검증한다.

### 완료 조건

- [x] 진행 지속 조건을 만족하는 동안 InfiniteMode가 계속된다.
- [x] 확정된 종료 조건에서 InfiniteMode가 한 번 종료된다.
- [x] InfiniteMode 종료가 Stage Clear로 처리되지 않는다.
- [x] 관련 Edit Mode와 Play Mode Test가 통과한다.

### Step 7 코드 구현 결과

아래 생산 코드를 추가했다.

- `Assets/Scripts/Runtime/Systems/InfiniteModeSystem.cs`
- `Assets/Scripts/Runtime/Features/StageOutOfBounds.cs`

아래 생산 코드를 변경했다.

- `Assets/Scripts/Runtime/Systems/GameSystem.cs`

`InfiniteModeSystem`은 PlayerMovementSystem이 기록한 Player Movement Runtime Data의 현재 수평 속도를 읽어 기존 `InfiniteModeState`에 전달한다. 최소 속도, 시작 유예 시간과 저속 연속 유예 시간은 이 System의 Inspector 설정으로 한 번만 소유한다.

속도 또는 이탈 종료 조건이 충족되면 `StageSystem.TryEndInfiniteStage()`를 호출한다. StageSystem이 기존 Stage 종료 이벤트를 발생시키므로 GameSystem의 기존 종료 절차가 Player 입력, 이동, Camera와 Stage 진행을 중단한다.

`StageOutOfBounds`는 지정된 Player Collider의 최초 Trigger 진입만 전달한다. InfiniteMode 규칙, Stage 종료와 Result 생성을 담당하지 않는다.

GameSystem은 시작 시 선택된 Mode로 InfiniteModeSystem을 초기화하고, 종료 또는 시작 실패 정리 시 상태와 이탈 Listener를 제거한다.

### Step 7 Test 추가 결과

아래 Play Mode Test를 추가했다.

- `Assets/Tests/PlayMode/InfiniteModeSystemTests.cs`

신규 5개 Test는 아래 항목을 검증한다.

- 최소 속도 경계에서 Infinite Stage 진행 유지
- 최소 속도 미만 연속 유예 시간 충족 시 Stage 종료 1회
- Player OutOfBounds 진입 시 Clear 없이 Infinite Stage 종료
- Player가 아닌 Collider 진입 무시
- 속도와 OutOfBounds 종료 요청이 연속 발생해도 Stage 종료 이벤트 1회

Step 6 완료 기준 Play Mode 50개에 신규 5개를 더한 예상 Play Mode 전체 Test 수는 55개이다.

Edit Mode Test는 기존 `InfiniteModeStateTests`가 속도 경계, 시작 유예, 저속 연속 유예, 회복, 이탈과 중복 종료를 이미 검증하므로 추가하지 않았다. 예상 Edit Mode 전체 Test 수는 68개이다.

AI는 사용자 요청에 따라 Unity Test Runner를 실행하거나 시도하지 않았다.

### Step 7 최초 Play Mode 검증 실패 및 조치

사용자가 신규 `InfiniteModeSystemTests` 5개 모두의 SetUp에서 `AmbiguousMatchException`이 발생한 것을 확인했다.

생산 코드가 아니라 Test Reflection 보조 함수가 `RuntimeDataSystem.CreateRuntimeData()`의 무인자 및 Mode 인자 오버로드를 이름만으로 검색한 것이 원인이었다.

`InvokeMethod`가 전달 인자의 실제 타입 배열을 함께 사용하여 정확한 Method 오버로드를 선택하도록 수정했다. 생산 코드의 기대값이나 Test 검증 기준은 변경하지 않았다.

### Step 7 정적 검증 결과

- 최소 속도, 시작 유예 시간과 저속 연속 유예 시간 설정은 InfiniteModeSystem에만 직렬화했다.
- PlayerMovementSystem의 이동 계산과 설정을 변경하지 않았다.
- InfiniteModeSystem은 Player Movement Runtime Data의 현재 수평 속도를 사용한다.
- StageOutOfBounds는 Player 진입 사실만 전달하며 종료 규칙을 포함하지 않는다.
- InfiniteMode 종료는 기존 StageSystem의 Stage 종료 이벤트 계약을 사용한다.
- GameSystem은 Stage가 Clear된 경우에만 TimeRecord Result Data를 생성하므로 InfiniteMode 종료에는 Clear Time을 생성하지 않는다.
- ResultSystem에 ScoreRecord 또는 이동 거리 책임을 추가하지 않았다.
- FixedUpdate에 정상 상태 반복 로그를 추가하지 않았다.
- 신규 Class는 파일별로 하나만 정의했다.
- nullable `?`, null 조건부 `?.` 또는 null 병합 `??` 문법을 추가하지 않았다.
- 저장된 SampleScene에서 InfiniteModeSystem이 Systems 아래에 존재하고 GameSystem 참조가 연결된 것을 확인했다.
- InfiniteModeSystem의 RuntimeDataSystem, StageSystem과 StageOutOfBounds 참조가 모두 연결된 것을 확인했다.
- 최소 속도, 시작 유예 시간과 저속 연속 유예 시간이 각각 `5`, `1`, `0.5`로 저장된 것을 확인했다.
- StageOutOfBounds가 InfiniteModeRoot 아래 기본 Layer에 존재하고 BoxCollider의 Trigger가 활성화된 것을 확인했다.
- StageOutOfBounds의 Player Collider가 생산 Player CapsuleCollider에 연결된 것을 확인했다.
- StageOutOfBounds BoxCollider의 크기와 중심이 각각 `(200, 1, 7)`, `(0, -3, 0)`으로 저장된 것을 확인했다.

### Step 7 최종 Unity 검증 결과

사용자가 Reflection 오버로드 검색 문제 수정 후 아래 결과를 확인했다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error 및 Warning 없음
- Edit Mode 전체 68개 Test 성공
- Edit Mode Test에서 예상하지 않은 Error 및 Warning 없음
- Play Mode 전체 55개 Test 성공
- Play Mode Test에서 예상하지 않은 Error 및 Warning 없음

이에 따라 Step 7 완료 조건을 모두 충족했다.

### Step 7 수동 Scene 작업

아래 작업은 OutOfBounds Trigger의 실제 공간 범위와 위치를 Scene View에서 결정해야 하므로 수동 작업이 필요하다.

#### InfiniteModeSystem 구성

1. `SampleScene`의 `Systems` 아래에 빈 GameObject `InfiniteModeSystem`을 생성한다.
2. `InfiniteModeSystem` Component를 부착한다.
3. Runtime Data System에 `Systems/RuntimeDataSystem`을 연결한다.
4. Stage System에 `Systems/StageSystem`을 연결한다.
5. Minimum Horizontal Speed, Start Grace Duration과 Below Speed Grace Duration은 기본값 `5`, `1`, `0.5`를 유지한다. 수치 밸런스 조정은 Phase 1 범위가 아니다.
6. `Systems/GameSystem`의 Infinite Mode System에 생성한 Component를 연결한다.

#### StageOutOfBounds 구성

1. `World/InfiniteModeRoot` 아래에 빈 GameObject `StageOutOfBounds`를 생성한다.
2. `StageOutOfBounds`에 BoxCollider를 추가하고 `Is Trigger`를 활성화한다.
3. Ground Layer가 아닌 기본 Layer를 사용한다.
4. Trigger를 두 Pattern의 아래쪽 낙하 영역 전체를 덮도록 배치한다.
5. Player가 정상 Pattern 위를 이동하거나 점프할 때 Trigger에 닿지 않고, Pattern 아래로 이탈하면 진입하도록 위치와 크기를 정한다.
6. `StageOutOfBounds` Component를 부착한다.
7. Player Collider에 `Player`의 CapsuleCollider를 연결한다.
8. `Systems/InfiniteModeSystem`의 Stage Out Of Bounds에 이 Component를 연결한다.
9. Scene을 저장한다.

Scene 저장 후 AI가 Component 개수, Layer, Trigger, Player Collider와 System Inspector 참조를 Scene YAML로 정적 검증한다. 따라서 이 연결 상태를 별도의 수동 확인 항목으로 두지 않는다.

#### Unity 자동 검증

1. Unity Script Compilation 성공 여부와 예상하지 않은 Error 및 Warning을 확인한다.
2. Edit Mode 전체 68개 Test를 실행한다.
3. Play Mode 전체 55개 Test를 실행한다.
4. Test에서 예상하지 않은 Error와 Warning이 없는지 확인한다.

Build는 Step 7 검증에 포함하지 않는다.

## Step 8. InfiniteMode 시작, 종료와 Retry 전체 흐름을 자동 검증한다

- 진행 상태: **완료**

### Play Mode Test 시나리오

1. InfiniteMode 선택 상태에서 게임을 시작하면 `Playing` 상태가 된다.
2. InfiniteMode 시작 시 Goal 없이 Stage가 진행된다.
3. Player Action Map은 활성화되고 UI Action Map은 비활성화된다.
4. 하나의 Pattern을 반복한 후에도 Stage가 계속 진행된다.
5. Goal 위치 또는 Goal Trigger 접촉으로 InfiniteMode가 종료되지 않는다.
6. 최소 속도 종료 조건에서 게임이 Result 상태로 전환된다.
7. Stage 이탈 종료 조건에서 게임이 Result 상태로 전환된다.
8. 종료 시 Player, 입력, 이동, Camera와 Stage가 정지한다.
9. ResultMenu의 Retry를 실행하면 같은 Mode로 새 Run이 시작된다.
10. Retry 후 Player 위치, 속도, Stage 상태, Pattern 상태와 Result 상태가 초기화된다.
11. 두 번째 Run 종료가 첫 번째 Run과 독립적이다.
12. 같은 실행 세션에서 최소 두 번 Retry할 수 있다.

### Test 작성 원칙

1. 생산 Scene과 실제 생산 Component를 사용한다.
2. Scene 오브젝트와 Inspector 참조를 Test 전용 복제 코드로 대체하지 않는다.
3. 프레임 실행, Rigidbody, Collider와 Trigger가 필요한 검증은 Play Mode에 둔다.
4. 단순 상태 계산은 Play Mode Test에 중복 작성하지 않고 Step 3 Unit Test를 재사용한다.
5. Test 간 상태가 누출되지 않도록 Scene 재로드 또는 명시적인 정리를 수행한다.
6. 반복 실행에서 간헐적으로 실패하는 시간 기반 Assertion을 만들지 않는다.

### 완료 조건

- [x] 신규 InfiniteMode 통합 Test가 모두 통과한다.
- [x] Retry를 포함한 반복 Run이 자동 검증된다.
- [x] 예상하지 않은 Error와 Warning이 없다.

### Step 8 Test 작성 결과

생산 Scene과 실제 생산 Component를 사용하는 아래 Play Mode Test를 추가했다.

- `Assets/Tests/PlayMode/InfiniteModeIntegrationTests.cs`

신규 4개 Test는 아래 시나리오를 검증한다.

- InfiniteMode 시작 상태, Mode Root, Action Map, 이동과 Camera 활성 상태
- Goal Trigger 무시와 생산 Pattern Boundary를 통한 Pattern 반복 후 진행 유지
- 실제 FixedUpdate에서 최소 속도 미만 연속 유예 충족 시 Result 상태 전환
- 생산 StageOutOfBounds BoxCollider에 Player를 진입시킨 물리 종료
- 종료 시 Stage Clear 및 Result Data 미생성
- 종료 시 Player 입력, 이동, Rigidbody, Camera와 Stage 진행 정지
- ResultMenu Submit Retry를 같은 실행 세션에서 두 번 수행
- Retry마다 Infinite Mode, Player 위치와 속도, Stage, Pattern 및 Result 상태 초기화
- 각 Retry 후 다음 Run이 독립적으로 종료되는 흐름

Step 7 완료 기준 Play Mode 55개에 신규 4개를 더한 예상 Play Mode 전체 Test 수는 59개이다.

Edit Mode Test는 변경하지 않았으며 예상 전체 Test 수는 68개이다.

AI는 사용자 요청에 따라 Unity Test Runner를 실행하거나 시도하지 않았다.

### Step 8 정적 검증 결과

- Test는 매 Test마다 SampleScene을 Single Mode로 다시 로드하여 Run 상태 누출을 방지한다.
- Test 전용 생산 Component 복제본을 만들지 않고 저장된 SampleScene의 Component와 Inspector 참조를 사용한다.
- 저속 종료는 `Time.fixedDeltaTime`과 생산 PlayerMovementRuntimeData 갱신 흐름을 사용한다.
- OutOfBounds 종료는 저장된 생산 BoxCollider 위치에 Player를 이동하고 FixedUpdate 물리 판정을 사용한다.
- ResultMenu Retry는 UIInputSystem의 Submit 입력을 GameSystem Update가 처리하는 기존 경로를 사용한다.
- Pattern 반복은 생산 InfiniteMapPattern 및 InfinitePatternBoundary를 사용한다.
- 순수 속도 경계 계산을 Test 코드에 다시 구현하지 않았다.
- 기존 Test를 삭제, Ignore하거나 기대값을 완화하지 않았다.
- 생산 코드는 변경하지 않았다.
- nullable `?`, null 조건부 `?.` 또는 null 병합 `??` 문법을 추가하지 않았다.
- 신규 Test와 고유한 `.meta` GUID가 존재한다.

### Step 8 수동 작업

Scene 또는 Inspector 수동 작업은 없다. Step 7에서 저장하고 정적으로 검증한 생산 Scene 구성을 그대로 사용한다.

Unity Editor에서 아래 자동 검증 결과만 확인한다.

1. Unity Script Compilation 성공 여부와 예상하지 않은 Error 및 Warning을 확인한다.
2. Edit Mode 전체 68개 Test를 실행한다.
3. Play Mode 전체 59개 Test를 실행한다.
4. Test에서 예상하지 않은 Error와 Warning이 없는지 확인한다.

Build는 Step 8 검증에 포함하지 않는다.

### Step 8 최종 Unity 검증 결과

사용자가 아래 결과를 확인했다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error 및 Warning 없음
- Edit Mode 전체 68개 Test 성공
- Edit Mode Test에서 예상하지 않은 Error 및 Warning 없음
- Play Mode 전체 59개 Test 성공
- Play Mode Test에서 예상하지 않은 Error 및 Warning 없음

이에 따라 Step 8 완료 조건을 모두 충족했다.

## Step 9. Stage Mode 전체 회귀 Test를 수행한다

- 진행 상태: **완료**

### 자동 검증 순서

1. Unity Script Compile Error와 Warning을 확인한다.
2. Edit Mode 전체 Test를 실행한다.
3. Play Mode 전체 Test를 실행한다.
4. 실패 시 신규 Test뿐 아니라 영향받은 기존 Fixture를 함께 확인한다.
5. `StageSystemTests`의 Goal, 중복 종료와 재시작 Test를 확인한다.
6. `StageGoalIntegrationTests`의 Clear, Result Data와 Retry Test를 확인한다.
7. `GameLifecycleIntegrationTests`의 Start, End와 입력 상태 Test를 확인한다.
8. Timer, ResultMenu, Player 이동, Jump, Landing, Collision과 Camera Test 결과를 확인한다.

### 적극적 정적 검증

1. 생산 코드에서 기존 API가 삭제되거나 이름이 바뀐 곳이 없는지 Diff를 확인한다.
2. `SampleScene.unity`의 기존 Stage Mode 참조가 유지되는지 fileID를 확인한다.
3. Goal, StartPoint, Player, StageHUD와 ResultPanel이 각각 필요한 개수로 존재하는지 확인한다.
4. 기존 Test를 삭제, Ignore 또는 기대값 완화로 통과시킨 변경이 없는지 확인한다.
5. Phase 1 범위 밖 파일 변경이 없는지 Git 변경 목록을 확인한다.

### 완료 조건

- [x] Compile이 성공한다.
- [x] Edit Mode 전체 Test가 통과한다.
- [x] Play Mode 전체 Test가 통과한다.
- [x] 일반 Stage의 시작, Goal Clear, TimeRecord, Result와 Retry에 회귀가 없다.
- [x] 기존 Test를 비활성화하거나 약화시키지 않았다.

### Step 9 검증 결과

Step 8 직후 생산 코드, Scene과 Test 변경 없이 Step 9를 수행했다. 사용자가 Step 8에서 확인한 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 59개 결과는 Step 9가 요구하는 전체 회귀 범위를 이미 포함한다. 동일 상태에 대한 Unity Test Runner 중복 실행은 수동 작업으로 요구하지 않았다.

사용자가 확인한 전체 회귀 결과는 아래와 같다.

- Unity Script Compilation 성공
- Unity Script Compilation에서 예상하지 않은 Error 및 Warning 없음
- Edit Mode 전체 68개 Test 성공
- Edit Mode Test에서 예상하지 않은 Error 및 Warning 없음
- Play Mode 전체 59개 Test 성공
- Play Mode Test에서 예상하지 않은 Error 및 Warning 없음

전체 Play Mode 결과에는 아래 기존 Stage Mode 회귀 Fixture가 포함된다.

- `StageSystemTests`
- `StageGoalIntegrationTests`
- `GameLifecycleIntegrationTests`
- `StageCollisionConfigurationTests`
- `TimerSystemTests`
- `ResultMenuIntegrationTests`
- `PlayerJumpIntegrationTests`
- `MomentumLandingIntegrationTests`
- `CameraFollowIntegrationTests`

### Step 9 정적 검증 결과

- 기존 `StageSystem.Initialize()` 무인자 API를 유지하며 기본 Stage Mode로 위임한다.
- 기존 Stage 시작, 중지, Stage Started, Cleared와 Ended Listener API를 삭제하거나 이름 변경하지 않았다.
- GameSystem의 기본 선택 Mode는 `Stage`로 유지된다.
- Stage Mode에서 기존 Goal 초기화, Clear 이벤트, Stage 종료 이벤트와 TimeRecord Result 생성 경로가 유지된다.
- InfiniteMode 종료에서만 Stage Clear와 TimeRecord Result 생성을 건너뛴다.
- SampleScene의 StageSystem에 기존 Goal과 StageModeRoot 참조가 유효한 fileID로 연결되어 있다.
- `Goal`, `StartPoint`, `Player`, `StageHUD`와 `ResultPanel`은 SampleScene에 각각 정확히 1개 존재한다.
- StageModeRoot와 InfiniteModeRoot도 각각 정확히 1개 존재한다.
- Test에서 `Ignore`, `Explicit` 또는 `Assert.Ignore` 사용이 확인되지 않았다.
- 기존 Stage Test를 삭제하지 않았고 신규 Mode 회귀 Test만 추가했다.
- Stage 지형 Test의 활성 계층 조건은 비활성 Infinite 복제본을 제외하도록 대상을 명확히 한 것이며 접촉 높이 기대값을 완화하지 않았다.
- 변경 파일은 Prototype 2 Phase 1의 문서, Mode Runtime Data, 관련 System, 생산 Scene과 관련 Test 범위에 한정된다.

### Step 9 수동 작업

없음.

Step 8 최종 전체 Test 이후 생산 코드, Scene과 Test 변경이 없으므로 Unity Script Compilation과 전체 Test를 중복 실행할 필요가 없다.

AI는 사용자 요청에 따라 Unity Test Runner와 Build를 실행하거나 시도하지 않았다.

### Ground Contact Grace 적용 후 수동 작업

Scene, Collider, Pattern, Anchor와 Inspector 참조 변경은 필요하지 않다.

Unity Editor에서 아래 항목만 확인한다.

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning이 없는지 확인한다.
2. Edit Mode 전체 74개 Test를 실행한다.
3. Play Mode 전체 60개 Test를 실행한다.
4. InfiniteMode에서 Pattern Ground 연결부를 최소 3회 통과하고 아래쪽 덜컥임이 사라졌는지 확인한다.
5. 실제 발판 끝에서 낙하가 즉시 시작되고 기존 Coyote Time 안의 Jump가 정상 동작하는지 확인한다.
6. Test와 수동 플레이에서 예상하지 않은 Error 및 Warning이 없는지 확인한다.

## Step 10. Unity Editor에서 최소 수동 플레이 검증을 수행한다

- 진행 상태: **Ground 연결부 흔들림 수정 완료, Unity 검증 대기**

자동 Test로 판정하기 어려운 실제 화면과 플레이 연결만 수동으로 확인한다.

### 수행 절차

1. Console을 비우고 확정한 방법으로 InfiniteMode를 선택한다.
2. Play Mode를 시작하고 InfiniteMode 전용 Stage 구성이 표시되는지 확인한다.
3. Goal 또는 일반 Stage 전용 오브젝트가 InfiniteMode 진행에 사용되지 않는지 확인한다.
4. Player 이동, Jump, Normal Landing과 Momentum Landing을 수행한다.
5. Pattern 경계를 여러 번 통과하여 같은 Pattern으로 진행이 이어지는지 확인한다.
6. Pattern 재배치 순간에 화면 점프, 지형 공백, Player 관통 또는 충돌 끊김이 없는지 확인한다.
7. 확정된 최소 속도 종료 조건을 재현한다.
8. 다시 시작한 뒤 Stage 이탈 종료 조건을 재현한다.
9. 각 종료 후 ResultMenu에서 Retry를 수행한다.
10. 같은 실행 세션에서 두 번 이상 새 Run을 시작한다.
11. Stage Mode로 전환하여 시작, Goal Clear와 Retry를 한 번 수행한다.
12. Console에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 수동 검증 범위

- Pattern 연결부가 실제 화면에서 자연스럽게 이어지는지 확인한다.
- Pattern 반복 중 Camera 구도와 충돌이 플레이를 방해하지 않는지 확인한다.
- Mode 전환과 Retry 후 잘못된 Stage 오브젝트가 보이지 않는지 확인한다.

상태 전환, 종료 1회 보장, 초기화, Goal 무시와 Retry 데이터 분리는 자동 Test 결과로 판정한다.

### 완료 조건

- 하나의 Pattern으로 InfiniteMode를 계속 진행할 수 있다.
- 두 종료 조건을 각각 재현할 수 있다.
- 같은 실행 세션에서 Retry가 반복 동작한다.
- Stage Mode의 기존 플레이가 유지된다.
- 치명적인 오류와 예상하지 않은 Warning이 없다.

### Step 10 수행 준비 결과

저장된 SampleScene을 정적으로 확인한 결과는 아래와 같다.

- GameSystem의 Selected Game Mode는 현재 `Stage`이다.
- StageModeRoot는 활성, InfiniteModeRoot는 비활성 상태로 저장되어 있다.
- InfiniteMode Pattern 2개, AdvanceBoundary 2개와 StageOutOfBounds가 저장되어 있다.
- 최소 수평 속도는 `5`, 시작 유예 시간은 `1`초, 저속 연속 유예 시간은 `0.5`초이다.
- 필수 Component와 Inspector 참조는 Step 7에서 정적으로 검증했다.
- 상태 전환, Goal 무시, Pattern 반복, 두 종료 조건, 정지와 Retry 초기화는 Step 8 자동 Test에서 검증했다.

따라서 Step 10에서는 화면, 조작감, Pattern 연결 순간, Camera 구도와 실제 Mode 전환만 수동으로 확인한다.

### Step 10 최소 수동 작업

#### InfiniteMode 플레이

1. Play Mode가 아닌 상태에서 `Systems/GameSystem`의 Selected Game Mode를 `Infinite`로 변경한다.
2. Console을 비운 뒤 Play Mode를 시작한다.
3. 시작 직후 수평 이동 입력을 유지한다. 시작 유예 시간은 `1`초이므로 그 안에 최소 속도 `5` 이상으로 가속한다.
4. Infinite 지형만 표시되고 일반 Stage의 Goal과 Stage 전용 지형이 화면에 사용되지 않는지 확인한다.
5. 이동, Jump, Normal Landing과 Momentum Landing을 각각 수행한다.
6. AdvanceBoundary를 여러 번 통과하여 Pattern이 최소 3회 이상 재배치되도록 진행한다.
7. 재배치 순간에 화면 점프, 지형 공백, 중복 지형, Player 관통, 충돌 끊김 또는 방해되는 Camera 구도가 없는지 확인한다.

#### 종료와 Retry

1. 시작 유예 시간이 지난 Run에서 이동 속도를 `0.5`초 이상 최소 속도 아래로 유지하여 저속 종료를 재현한다.
2. ResultMenu에 `Run Ended`, Retry와 Quit가 표시되고 Clear Time이 표시되지 않는지 확인한다.
3. Retry를 선택하고 새 Run이 정상 시작되는지 확인한다.
4. 다음 Run에서 Pattern 아래로 낙하하여 StageOutOfBounds 종료를 재현한다.
5. 다시 Retry를 선택하고 세 번째 Run이 정상 시작되는지 확인한다.
6. Retry 후 잘못된 Stage 오브젝트가 표시되거나 Player, Pattern 또는 Camera가 이전 Run 위치에서 이어지지 않는지 확인한다.

#### Stage Mode 회귀 플레이

1. Play Mode를 종료하고 GameSystem의 Selected Game Mode를 `Stage`로 되돌린다.
2. Play Mode를 시작하여 기존 Stage 지형과 Goal이 표시되는지 확인한다.
3. Goal에 도달하여 Clear Time Result가 표시되는지 확인한다.
4. Retry를 한 번 수행하여 Stage가 정상 초기화되는지 확인한다.
5. Play Mode를 종료한다.
6. 전체 과정에서 Console에 예상하지 않은 Error와 Warning이 없었는지 확인한다.

Build와 Unity Test Runner 실행은 Step 10 수동 검증에 포함하지 않는다.

### Step 10 Ground 연결부 흔들림 수정

수동 플레이 중 Pattern_0 End와 Pattern_1 Start의 Ground가 정확히 연결되어 있어도 Player가 아래로 잠깐 흔들리는 현상을 확인했다. Grounded Distance를 `0.05`로 늘린 비교에서도 증상이 유지되어 단순 Ground SphereCast 거리 부족을 원인에서 제외했다.

사용자는 Collider 크기 변경과 Pattern 또는 Collider 중첩 방식을 사용하지 않기로 결정했다.

기존 Jump Coyote Time은 실제 발판 이탈 후 점프 입력 허용만 담당하며 중력과 Ground 이동 상태를 유지하지 않는다. 이 책임을 변경하지 않고 아래 `GroundContactGrace`를 별도로 추가했다.

- `Assets/Scripts/Runtime/Features/GroundContactGrace.cs`
- `Assets/Tests/EditMode/GroundContactGraceTests.cs`

PlayerMovementSystem 변경 내용은 아래와 같다.

- 실제 Ground 접촉 시 Ground Contact Grace 시간을 충전한다.
- 실제 접지가 순간적으로 사라져도 가까운 정상 지면 후보가 유지되면 최대 `0.04`초 동안 안정화된 Grounded 상태를 사용한다.
- Ground 후보 거리는 최대 `0.08`, Surface Normal Y는 최소 `0.7`을 사용한다.
- 안정화된 Grounded 상태에서는 Jump를 시작하지 않은 수직 속도를 `0`으로 보정한다.
- 실제 지면이 멀거나 급경사인 경우 Grace를 즉시 취소한다.
- Jump Sequence가 상승 중이거나 실제 공중 진행 중이면 Grace를 적용하지 않는다.
- Stop과 Retry 초기화에서 남은 Grace 상태를 제거한다.
- 기존 Jump Coyote Time `0.1`초와 상태 및 설정을 공유하지 않는다.

신규 Edit Mode Unit Test 6개는 아래 항목을 검증한다.

- 실제 Ground 접촉 시 Grace 충전
- 가까운 평면 Ground 후보에서 FixedUpdate 2회 안정화
- Grace 시간 종료 후 Airborne 판정
- 먼 Ground 후보 거부
- 급경사 Surface 후보 거부
- 상승 Jump와 Reset에서 Grace 비활성

Step 9 완료 기준 Edit Mode 전체 68개에 신규 6개를 더한 예상 Edit Mode 전체 Test 수는 74개이다.

`InfiniteModeIntegrationTests`에 생산 Scene의 실제 Pattern Ground 연결부를 Rigidbody가 통과하는 Play Mode 회귀 Test 1개를 추가했다. 연결 전후 Player Y 위치, 수직 속도와 Player Movement Runtime Data의 Grounded 상태를 검증한다. 예상 Play Mode 전체 Test 수는 60개이다.

### Ground Contact Grace 정적 검증 결과

- GroundContactGrace는 Scene과 독립적인 순수 상태 Class이며 파일에 하나의 Class만 정의했다.
- PlayerMovementSystem의 기존 중력 설정과 JumpFeature의 Coyote Time 설정을 변경하지 않았다.
- Collider 크기, 위치, Pattern Transform과 Anchor를 변경하지 않았다.
- 가까운 Ground 후보가 없으면 실제 발판 이탈 즉시 Airborne 상태를 사용한다.
- 상승 Jump에서는 Raw Grounded 여부보다 Jump 진행 상태를 우선하여 Grace를 취소한다.
- 정상 프레임 반복 로그를 추가하지 않았다.
- nullable `?`, null 조건부 `?.` 또는 null 병합 `??` 문법을 추가하지 않았다.
- 신규 Script와 Test의 고유한 `.meta` GUID가 존재한다.

### Ground Contact Grace 최초 Unity 검증 실패 및 추가 조치

사용자가 Play Mode 전체 실행에서 5개 실패를 확인했다.

`PatternGroundSeam_KeepsPlayerVerticallyStable`은 연결부에서 Player 수직 속도가 예상 `0` 대신 `0.842835307`이 되어 실패했다. 양수 수직 속도이므로 접지 누락에 따른 중력 적용이 아니라 Ground Contact Grace 이동 계산 이후 PhysX Solver가 독립된 Collider 내부 경계 접촉에서 수직 속도를 다시 만든 것으로 판정했다.

PlayerControllerSystem이 최근 적용한 MovementState가 Grounded 또는 Landing이고 `OnCollisionStay`에서 위쪽 정상 바닥 접촉을 확인한 경우에만 Solver 이후 Rigidbody 수직 속도를 `0`으로 안정화하도록 추가했다. Jump와 Airborne MovementState는 보정하지 않는다.

나머지 Stage 및 Jump 관련 4개 실패는 수동 InfiniteMode 검증 과정에서 SampleScene의 GameSystem Selected Game Mode가 `Infinite`로 저장된 것이 원인이었다.

- `Goal_UsesNonGroundTriggerWithoutVisualCollider`: InfiniteModeRoot 활성화로 Stage Goal이 비활성 상태
- `PlayerEntersGoal_ClearsStageAndEndsGameOnce`: InfiniteMode가 Goal을 종료 조건으로 사용하지 않음
- `StartGame_AfterGoalClear_RestoresNewStagePlay`: InfiniteMode에서 Goal Clear가 발생하지 않아 실행 중 StartGame 요청이 거부됨
- `Jump_GravityChanges_PreservesIntegratedJumpHeight`: InfiniteMode 저속 종료가 Jump 반복 측정 중 Run을 종료함

프로젝트의 저장 기본 Mode 규칙에 따라 SampleScene의 Selected Game Mode를 `Stage`로 복원했다. InfiniteMode 통합 Test는 Test 내부에서 Mode를 명시적으로 Infinite로 변경하므로 영향을 받지 않는다.

정적 검증에서 SampleScene의 Grounded Distance `0.05`가 유지되고 Selected Game Mode가 `Stage`로 복원된 것을 확인했다.

추가 조치 후에는 최초 실패한 5개 Test와 전체 Play Mode 60개를 다시 검증해야 한다.

AI는 사용자 요청에 따라 Unity Test Runner와 Build를 실행하거나 시도하지 않았다.

### Step 10 최종 결정: 연결부를 점프 구간으로 변경

후속 수동 확인에서도 독립된 Ground Collider의 내부 경계에서 덜컥임이 유지되었다. 이 게임은 Jump를 기본 조작으로 사용하므로 사용자는 연결부를 평지처럼 통과시키지 않고 명시적인 점프 구간으로 구성하기로 결정했다.

이에 앞서 추가한 Ground 연결부 보정은 최종 구현에서 제거했다.

- `GroundContactGrace` 생산 코드와 Edit Mode Unit Test 6개 제거
- PlayerMovementSystem의 Ground Contact Grace 설정과 판정 제거
- PlayerControllerSystem의 Solver 이후 수직 속도 및 상향 변위 보정 제거
- `PatternGroundSeam_KeepsPlayerVerticallyStable` Play Mode Test 제거
- `PatternGroundConnections_ProvideJumpGap` Play Mode Scene 구성 Test 추가

Pattern의 Ground는 각각 로컬 X `-20~20`을 유지한다. Collider 크기와 Ground Transform은 변경하지 않는다. 각 Pattern의 StartAnchor와 EndAnchor를 로컬 X `-22/22`로 옮겨 Pattern 정렬 간격을 `44`로 변경했다. Pattern_1 초기 위치도 X `44`로 변경하여 Pattern_0과 Pattern_1 사이에 `4` unit의 실제 점프 구간을 구성했다.

Scene 저장 Mode와 관계없이 Stage 전용 Play Mode Test는 Test 내부에서 Stage Mode로 재시작하며 InfiniteMode 통합 Test는 Infinite Mode를 명시한다.

최종 예상 Test 수는 Edit Mode 68개, Play Mode 60개이다.

### Step 10 최종 Unity 검증 결과

- Unity Script Compilation에 성공했다.
- Script Compilation에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- Edit Mode Test 전체 68개를 실행했고 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- Play Mode Test 전체 60개를 실행했고 모두 성공했다.
- Play Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- Pattern 점프 구간 변경과 기존 Ground 연결부 보정 제거에 대한 Compile 및 자동 Test 완료 조건을 충족했다.

## Step 11. 저장 상태와 Phase 1 완료 근거를 정리한다

- 진행 상태: **완료**

### 수행 절차

1. Play Mode를 종료한다.
2. 변경한 Scene과 Asset을 저장한다.
3. Scene을 닫았다가 다시 열어 Component와 Inspector 참조가 유지되는지 확인한다.
4. 저장된 Scene YAML을 다시 정적으로 검사한다.
5. Unity Script Compile 결과를 확인한다.
6. Edit Mode와 Play Mode 전체 Test를 최종 실행한다.
7. Test 수, 성공·실패, Error와 Warning 결과를 기록한다.
8. Phase 1 수동 검증 결과를 별도 Verification Result Task 문서로 작성한다.
9. 구현으로 책임이나 Feature 규칙이 변경되었다면 관련 문서가 코드와 일치하는지 최종 대조한다.
10. 모든 Phase 1 완료 조건을 충족한 경우에만 Roadmap의 Phase 1 상태를 완료로 변경한다.
11. 미완료 항목이 있으면 Roadmap을 완료로 표시하지 않고 근거와 후속 작업을 기록한다.

### 완료 조건

- Scene과 Asset 저장 후 참조가 유지된다.
- 정적 검증, Compile, Edit Mode 전체 Test와 Play Mode 전체 Test가 통과한다.
- 수동 플레이 결과와 미해결 사항이 기록되어 있다.
- Roadmap 상태와 실제 완료 상태가 일치한다.

### Step 11 수행 결과

- 저장된 SampleScene YAML의 필수 Component, 직렬화 `fileID`, Pattern 및 Anchor 배치를 정적으로 검증했다.
- Phase 1 신규 Script와 Test의 `.meta` 파일 존재를 정적으로 검증했다.
- 생산 코드와 관련 System 및 Feature 문서의 책임을 대조했다.
- 사용자가 확인한 Unity Script Compilation, Edit Mode 68개와 Play Mode 60개의 전체 성공 결과를 최종 자동 검증 근거로 사용했다.
- 별도 Verification Result 문서 `20260901_01_Phase1VerificationResult.md`를 작성했다.
- 사용자가 Scene 재개방과 최종 수동 플레이 체크리스트 전체의 성공을 확인했다.
- 최종 Compile, 자동 Test와 수동 검증에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 모든 완료 조건을 충족하여 Roadmap Phase 1 상태를 `완료`로 변경했다.

### Step 11 InfiniteMode 추락 판정 보완

고정된 StageOutOfBounds Trigger는 Player가 계속 수평 이동하는 InfiniteMode에서 유효 범위를 벗어날 수 있으므로 최종 추락 판정에서 제거했다.

- InfiniteModeSystem이 Player Transform과 Fall Threshold Y `-3`을 소유한다.
- InfiniteMode 진행 중 Player Y가 임계값 이하이면 X 위치와 관계없이 추락 종료를 요청한다.
- 시작 유예 시간은 기존과 같이 속도 판정에만 적용하며 추락 판정에는 적용하지 않는다.
- 생산 `StageOutOfBounds` Script, Scene GameObject, Collider와 Listener 연결을 제거했다.
- 기존 OutOfBounds 상태 및 Play Mode Test를 Y 임계값 Test로 교체했다.
- 큰 X 위치 `10000`에서 임계값 이하 추락 종료를 검증한다.
- Player가 임계값보다 높으면 종료되지 않고, 속도 종료와 추락 종료가 연속 요청되어도 종료 이벤트는 1회임을 검증한다.
- 예상 Test 수는 Edit Mode 68개, Play Mode 60개로 유지된다.

---

# 정적 검증 체크리스트

- [x] Phase 1 미확정 규칙이 구현 전에 확정되었다.
- [x] 문서 책임 간 중복과 충돌이 없다.
- [x] Mode 표현이 한 곳에 정의되어 있다.
- [x] InfiniteMode 전용 상태가 Stage Mode 상태와 혼합되지 않는다.
- [x] Runtime Data에는 공유 데이터만 존재한다.
- [x] 저장 또는 서버 기능이 추가되지 않았다.
- [x] InfiniteMode 코드가 Goal을 종료 조건으로 참조하지 않는다.
- [x] Stage Mode의 Goal 경로가 유지된다.
- [x] 최소 속도 설정의 소유자가 중복되지 않는다.
- [x] 이벤트 Listener 등록과 해제가 대응한다.
- [x] 정상 프레임 반복 로그가 추가되지 않았다.
- [x] Scene의 필수 Component가 정확한 개수로 존재한다.
- [x] 필수 Serialized Field의 fileID가 유효하다.
- [x] 신규 Script와 Test의 `.meta` 파일이 존재한다.
- [x] 요구 변경으로 제거한 Ground 이음매 Test를 점프 간격 구성 Test로 대체했으며 다른 기존 Test는 삭제, Ignore 또는 약화하지 않았다.
- [x] Phase 1 범위 밖 변경이 없다.

---

# 자동 Test 체크리스트

## Edit Mode Unit Test

- [x] Mode 초기값과 변경 규칙
- [x] Stage Mode와 InfiniteMode 종료 조건 분기
- [x] InfiniteMode Goal 무시 규칙
- [x] 최소 속도 경계값
- [x] Stage 이탈 판정
- [x] 종료 1회 보장
- [x] 시작 전과 종료 후 입력 무시
- [x] Retry 상태 초기화
- [x] Pattern 반복 요청 중복 방지
- [x] Stage Mode에서 InfiniteMode 규칙 비활성
- [x] Runtime Data 생성, 제거와 재생성
- [x] Edit Mode 전체 회귀 Test

## Play Mode Test

- [x] Mode별 실제 Scene 또는 생산 구성 초기화
- [x] InfiniteMode Goal 미사용
- [x] 실제 Rigidbody 속도와 종료 판정 연결
- [x] 실제 Stage 이탈과 종료 판정 연결
- [x] Pattern 반복과 충돌 유지
- [x] 종료 시 Player, 입력, 이동, Camera와 Stage 정지
- [x] 같은 Mode Retry
- [x] Retry 후 Player, Pattern, Stage와 Result 초기화
- [x] 같은 실행 세션의 반복 Run
- [x] Stage Mode Goal Clear와 TimeRecord 회귀
- [x] Play Mode 전체 회귀 Test

---

# 수동 검증 체크리스트

- [x] InfiniteMode 선택 또는 지정 방법이 실제 Unity 실행에서 동작한다.
- [x] InfiniteMode에서 Goal을 사용하지 않는다.
- [x] 하나의 Pattern으로 여러 구간을 진행할 수 있다.
- [x] Pattern 사이의 의도된 4 unit 간격을 Jump로 통과할 수 있고 재배치 후에도 같은 간격이 유지된다.
- [x] 최소 속도 종료 조건을 실제 플레이로 재현할 수 있다.
- [x] Player의 X 위치와 관계없이 Y `-3` 이하 추락 종료를 실제 플레이로 재현할 수 있다.
- [x] 종료 후 같은 Mode로 Retry할 수 있다.
- [x] 같은 실행 세션에서 두 번 이상 Retry할 수 있다.
- [x] Stage Mode의 시작, Goal Clear와 Retry가 유지된다.
- [x] Console에 치명적인 Error와 예상하지 않은 Warning이 없다.

---

# 문제 발생 시 확인 순서

| 문제 | 먼저 확인할 항목 |
| --- | --- |
| InfiniteMode가 시작되지 않음 | Mode 초기화, Runtime Data, GameSystem 시작 분기, Stage 구성 참조 |
| InfiniteMode에서 Goal로 종료됨 | StageSystem Mode 분기, StageGoal Listener 등록 조건, 생산 Scene 구성 |
| 속도가 충분한데 종료됨 | 판정 대상 속도, 경계값, 판정 시작 시점, Unit Test 입력 |
| 정지해도 종료되지 않음 | 진행 지속 조건 갱신, 이동 결과 전달, StageSystem 종료 요청 |
| Stage 이탈이 감지되지 않음 | 경계 Collider, Layer, Trigger 설정, Player Collider, Physics Matrix |
| Stage 종료가 두 번 발생함 | 종료 상태 Guard, Listener 중복 등록, 같은 프레임 복수 종료 조건 |
| Pattern이 중복 생성됨 | 경계 통과 1회 처리, Pattern 상태 초기화, Trigger 재진입 |
| Pattern 연결부에서 추락함 | 기준 Transform, Pattern 길이, Collider 간격, 재배치 시점 |
| Retry 후 즉시 종료됨 | 이전 속도 판정, 이탈 상태, Pattern 상태와 종료 Flag 초기화 |
| Retry 후 Mode가 바뀜 | Mode 보존 위치, Runtime Data 재생성 순서, 기본 Mode 덮어쓰기 |
| Stage Mode Goal이 동작하지 않음 | Stage Mode 분기, StageGoal 초기화, Listener와 Scene 참조 |
| InfiniteMode에서 Clear Time이 생성됨 | GameSystem Stage 종료 처리, IsCleared 조건, ResultSystem 호출 분기 |
| Test가 간헐적으로 실패함 | 프레임 대기, Physics.SyncTransforms, Scene 초기화와 Test 상태 누출 |
| Scene 재개방 후 참조가 없음 | Scene 저장, Serialized Field fileID, Script `.meta` GUID |

문제가 발생하면 Console 메시지, 실패한 Test 이름, 실패 메시지, Stack Trace와 재현 Step을 먼저 기록한다.

원인이 확인되기 전에 생산 코드, Scene 참조 또는 Test 기대값을 임의로 변경하지 않는다.

---

# 영향 범위

## Tasks

- Prototype 2 Phase 1 수동 작업 순서 문서

실제 구현은 아직 수행하지 않았으므로 Systems, Features와 Assets의 영향은 예상 범위로만 기록했다.

---

# 검증 내용

- Prototype 2 Roadmap의 Phase 1 목표, 구현 대상과 완료 조건을 확인했다.
- InfiniteMode와 StagePlay Feature 규칙을 확인했다.
- 관련 System의 책임과 현재 생산 코드 흐름을 대조했다.
- 현재 Scene, Edit Mode Test와 Play Mode Test 구조를 정적으로 조사했다.
- Step 2에서 Public API, 직렬화 참조, Scene 오브젝트, 실행 순서와 Test 의존성을 정적으로 확인했다.
- Step 2의 수정 대상, 유지 대상, 신규 파일과 회귀 Test 범위를 파일 단위로 확정했다.
- Step 3에서 InfiniteMode의 Scene 비의존 상태 규칙을 Test 우선으로 구현했다.
- 신규 Edit Mode Test 23개 Case와 최소 생산 코드를 추가하고 정적으로 검사했다.
- Unity Test Runner는 라이선스 문제로 Test 시작 전에 종료되어 실행 결과를 확보하지 못했다.
- 사용자가 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 없음을 확인했다.
- 사용자가 Edit Mode Test 62개와 Play Mode Test 34개의 전체 성공을 확인했다.
- 사용자가 Edit Mode와 Play Mode Test에서 예상하지 않은 Error 및 Warning 없음을 확인했다.
- Step 4에서 Game Mode를 Runtime Data 생성 상태에 연결하고 Stage Mode 기본 호환성을 유지했다.
- GameRuntimeData의 생성, Clear, 재생성과 잘못된 순서 요청을 검증하는 Edit Mode Test 6개 Case를 추가했다.
- Step 4 변경은 정적으로 검증했으며 사용자 요청에 따라 Unity Test Runner와 Build를 실행하지 않았다.
- 사용자가 Step 4 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 없음을 확인했다.
- 사용자가 Step 4 Edit Mode Test 68개와 Play Mode Test 34개의 전체 성공을 확인했다.
- 사용자가 Step 4 Edit Mode와 Play Mode Test에서 예상하지 않은 Error 및 Warning 없음을 확인했다.
- Step 5에서 StageSystem의 Stage Mode Goal 경로와 InfiniteMode Goal 비의존 종료 경로를 분리했다.
- StageSystem 기존 6개 Test를 유지하고 Mode별 초기화, Goal 무시, 종료 1회와 재시작 Test 7개를 추가했다.
- Step 5 변경은 정적으로 검증했으며 사용자 요청에 따라 Unity Test Runner와 Build를 실행하지 않았다.
- 사용자가 Step 5 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 없음을 확인했다.
- 사용자가 Step 5 Edit Mode Test 68개와 Play Mode Test 41개의 전체 성공을 확인했다.
- 사용자가 Step 5 Edit Mode와 Play Mode Test에서 예상하지 않은 Error 및 Warning 없음을 확인했다.
- Step 6에서 Pattern 인스턴스 2개의 Anchor 정렬, Boundary 중복 방지와 Reset을 담당하는 생산 코드를 추가했다.
- Pattern 재배치 Test 7개와 Mode Root 활성화 Test 2개를 추가했다.
- Step 6 코드와 Test는 정적으로 검증했으며 사용자 요청에 따라 Unity Test Runner와 Build를 실행하지 않았다.
- Step 6 생산 Scene 구성과 저장된 Scene YAML 정적 검증을 완료했다.
- 최초 Play Mode 실행에서 비활성 Infinite Pattern 지형을 선택한 기존 Test 검색 문제를 확인하고 활성 계층만 검사하도록 수정했다.
- 사용자가 Step 6 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 50개의 성공을 확인했다.
- 사용자가 Compilation, Edit Mode 및 Play Mode Test에서 예상하지 않은 Error 및 Warning 없음을 확인했다.
- 문서에 확정되지 않은 구현 결정과 확인된 기존 구조를 구분했다.
- 자동 판정 가능한 항목을 Edit Mode Unit Test와 Play Mode Test로 배치했다.
- Unity Editor에서 실제로 수행할 작업을 11개 순차 Step으로 작성했다.
- 최종 수동 검증 범위를 화면, 물리 연결과 실제 플레이 확인으로 제한했다.

---

# 검증 결과

- Prototype 2 Phase 1 수동 작업 Step 문서 작성이 완료되었다.
- Step 1의 12개 미확정 규칙을 권장안 조합으로 확정했다.
- 확정된 기능 규칙과 System 책임을 관련 문서에 반영했다.
- Step 2의 현재 구조 변경 지점 조사가 완료되었다.
- Step 2에서 Unity Editor 또는 Build가 필요한 수동 작업은 확인되지 않았다.
- Step 3 생산 코드와 Unit Test 작성은 완료되었다.
- Step 3 신규 23개를 포함한 Edit Mode 전체 62개 Test가 통과했다.
- 기존 Play Mode 전체 34개 Test가 통과했다.
- Step 3의 Compile, Test와 Error 및 Warning 완료 조건을 모두 충족했다.
- Step 4 생산 코드와 Unit Test 작성은 완료되었다.
- Step 4 Edit Mode 전체 68개와 Play Mode 전체 34개 Test가 통과했다.
- Step 4의 Compile, Test와 Error 및 Warning 완료 조건을 모두 충족했다.
- Step 5 생산 코드와 Play Mode Test 작성은 완료되었다.
- Step 5 Edit Mode 전체 68개와 Play Mode 전체 41개 Test가 통과했다.
- Step 5의 Compile, Test와 Error 및 Warning 완료 조건을 모두 충족했다.
- Step 6 코드와 Play Mode Test 작성은 완료되었다.
- Step 6 예상 Test 수는 Edit Mode 68개와 Play Mode 50개이다.
- Step 6 Scene 구성과 Scene 정적 검증은 완료되었다.
- 최초 Play Mode 검증 실패 원인을 수정한 뒤 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 50개가 통과했다.
- Step 6의 Compile, Test, Scene 정적 검증과 Error 및 Warning 완료 조건을 모두 충족했다.
- Step 7에서 이동 Runtime Data와 OutOfBounds를 InfiniteMode 종료 규칙 및 StageSystem 종료 경로에 연결했다.
- Step 7 Play Mode Test 5개를 추가했으며 코드와 문서는 정적으로 검증했다.
- Step 7 생산 Scene 구성과 저장된 Scene YAML 정적 검증을 완료했다.
- 신규 Test의 Reflection 오버로드 검색 문제를 수정한 뒤 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 55개가 통과했다.
- Step 7의 Compile, Test, Scene 정적 검증과 Error 및 Warning 완료 조건을 모두 충족했다.
- Phase 1의 Step 3부터 Step 9까지 구현과 Unity 검증을 완료했다.
- Step 8에서 생산 Scene 기반 InfiniteMode 시작, 종료, 정지와 Retry 반복 통합 Test 4개를 추가했다.
- Step 8 Test와 문서는 정적으로 검증했으며 사용자 요청에 따라 Unity Test Runner와 Build를 실행하지 않았다.
- 사용자가 Step 8 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 59개의 성공을 확인했다.
- 사용자가 Compilation, Edit Mode 및 Play Mode Test에서 예상하지 않은 Error 및 Warning 없음을 확인했다.
- Step 8의 Compile, Test와 Error 및 Warning 완료 조건을 모두 충족했다.
- Step 9에서 기존 Stage API, Scene 참조와 오브젝트 개수, Test 비활성화 여부 및 변경 범위를 정적으로 검증했다.
- Step 8 직후 동일한 생산 상태에서 확인된 Compile, Edit Mode 전체 68개와 Play Mode 전체 59개 결과로 Stage Mode 전체 회귀 성공을 확인했다.
- Step 9에는 추가 수동 작업이 없으며 완료 조건을 모두 충족했다.
- Step 10의 저장 Scene 설정과 자동 검증 완료 범위를 정적으로 확인하고 실제 화면과 플레이 감각만 남긴 최소 수동 절차를 확정했다.
- Pattern Ground 연결부 보정 시도는 최종 구현에서 제거하고 Pattern 사이에 4 unit 점프 구간을 구성했다.
- InfiniteMode 추락 판정을 고정 Trigger에서 X 위치에 독립적인 Player Y `-3` 임계값으로 변경했다.
- 최종 Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 60개가 통과했다.
- Step 11 수동 검증 체크리스트 전체가 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Prototype 2 Phase 1 완료 조건을 모두 충족했다.
- Roadmap의 Phase 1 상태를 `완료`로 변경했다.

---

# 후속 작업

1. Unity Script Compilation, Edit Mode 전체 68개와 Play Mode 전체 60개 Test 결과를 확인한다.
2. Pattern 사이의 4 unit 구간을 Jump로 통과할 수 있는지 최소 수동 플레이로 확인한다.

---

# 관련 문서

## Project

- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`

## Rules

- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/EVENT_RULE.md`
- `AI/01_Rules/LOGGING_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`

## Systems

- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/UIManagementSystem.md`

## Features

- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/ResultMenu.md`

## Roadmap

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`

## Template

- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_2/20260828_03_Prototype2Roadmap.md`
- `AI/90_Tasks/Prototype_1/20260721_04_Phase5ManualSteps.md`

---

# 작성 완료 기준

- `GENERAL_TASK_TEMPLATE.md`의 필수 섹션을 작성했다.
- 확인된 문서와 현재 구현에 근거한 내용만 작성했다.
- 실제 수동 작업을 순서가 있는 Step으로 표현했다.
- 각 Step에 수행 절차, 정적 검증, 자동 Test와 완료 조건을 작성했다.
- 정적 검증을 구현 전, 구현 중, Scene 저장 후와 최종 검증 단계에 배치했다.
- Unit Test를 상태 규칙 구현 전에 작성하도록 배치했다.
- Scene과 통합 흐름은 Play Mode Test로 검증하도록 구분했다.
- 확인되지 않은 요구사항을 선행 결정 항목으로 분리했다.
- 실제 구현과 수동 검증 결과를 완료로 기록하지 않았다.
