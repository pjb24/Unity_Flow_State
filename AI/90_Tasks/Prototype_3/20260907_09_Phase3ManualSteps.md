# 작업 정보

## 작업명

Prototype 3 Phase 3 Manual Steps

## 작업 일자

20260907

## 작업 담당자

AI, 사용자

## 작업 상태

진행 중 — Step 1~9 완료, Step 10 대기

---

# 작업 목적

Prototype 3 Phase 3의 Score Collectible을 Stage Mode와 InfiniteMode에 구현하고 검증하는 순서를 정의한다.

실질적인 사용자 작업을 규칙 결정, Unity Script Compilation 및 Test Runner 실행, 필요한 Scene 배치와 최소 화면 검증으로 제한한다.

계산, 상태 전환, 중복 획득 방지, Run 초기화, 점수 합산과 회귀는 정적 검증 및 Unit Test로 판정한다.

---

# 작업 대상

- Score Collectible 규칙과 책임
- Collectible 획득 및 같은 Run의 중복 획득 방지
- Run별 Collectible 상태 및 Score 초기화
- Stage Mode Collectible 배치
- InfiniteMode Map Pattern Collectible 배치와 Pattern 재사용
- 점프 시작, 공중 이동 경로와 착지 지점 안내
- Collectible을 놓친 경우의 Stage Play 지속
- InfiniteMode 거리 Score 보존
- Pause, Result와 Retry 회귀
- Compile, 전체 Test와 최소 화면 검증

---

# 작업 전 상태

- Prototype 3 Phase 2가 완료되었다.
- Stage Mode와 InfiniteMode에서 Player가 입력 없이 오른쪽으로 자동 이동한다.
- Jump와 Momentum Landing 입력이 유지되고 Player 좌우 이동 입력은 플레이에 영향을 주지 않는다.
- InfiniteMode에는 이동 거리 기반 ScoreCalculator, Runtime Score와 ScoreRecord가 존재한다.
- Stage Mode 결과는 Clear Time을 사용하고 InfiniteMode 결과는 최종 이동 거리와 최종 Score를 사용한다.
- InfiniteMode Map Pattern은 두 Pattern을 재배치하여 반복 사용한다.
- Score Collectible 생산 코드와 Scene 배치는 아직 없다.
- Phase 4에서 Mode별 Collectible Score 표시와 Total Score UI를 통합한다.
- 문서 작성 당시 최근 테스트 정리 후 정적 Test 수는 Edit Mode 285개, Play Mode 144개였으며 변경 후 Unity 검증은 미수행 상태였다. 이후 Step 1 사용자 결과와 정적 확인은 해당 Step에 기록한다.

---

# 조사 내용

- Roadmap Phase 3는 두 Mode 획득, 한 Run에서 한 번만 획득, Retry 초기화와 경로 안내를 요구한다.
- Collectible Score는 두 Mode에 필요하지만 기존 InfiniteMode 거리 Score를 대체하면 안 된다.
- Collectible 획득 상태와 Score는 Run에 속하므로 정적 Scene Object가 영구 상태를 소유해서는 안 된다.
- InfiniteMode Pattern Object는 같은 Run에서도 재사용되므로 단순 GameObject 비활성화만으로 획득 여부를 관리하면 다음 Pattern 순환에서 복구 시점을 구분하기 어렵다.
- Phase 3에서는 획득과 Runtime Score를 구현하고, HUD와 Result의 Mode별 Score 통합은 Phase 4에 남겨야 한다.
- Collectible 위치의 도달 가능성은 이동 및 Jump 수치로 자동 검증할 수 있고, 안내성 및 화면 가독성만 수동으로 확인해야 한다.
- Scene과 Input Asset 변경은 코드 및 YAML 정적 검사로 필요성이 확인된 경우에만 사용자 작업으로 요청한다.

---

# 작업 원칙

- 각 구현 Step은 실패하는 관련 Unit Test를 먼저 작성하거나 기존 Test의 명시적 실패 근거를 확보한 뒤 생산 코드를 변경한다.
- 수치 계산, 상태, 획득 횟수, Score, 초기화, Pattern 재사용과 입력 타이밍은 Edit Mode 또는 Play Mode Test로 판정한다.
- 생산 Scene 참조는 YAML과 `.meta` GUID로 먼저 검사하고, Editor에서만 가능한 배치 및 저장만 사용자에게 요청한다.
- 사용자는 Unity Editor에서 Script Compilation과 Test Runner를 실행한다. AI는 Unity Editor와 Test Runner를 실행하지 않는다.
- AI는 Scene을 직접 수정하지 않는다. Scene 변경이 필요하면 Hierarchy, Component, Field와 값 단위의 절차를 제공한다.
- Test가 실패하면 실패 이름, 메시지와 Stack Trace를 근거로 수정하고 관련 Test를 다시 실행한다.
- 모든 관련 Test가 통과하기 전에는 다음 책임의 구현으로 넘어가지 않는다.
- 수동 화면 확인은 전체 정적 검증 및 전체 Test 통과 후 한 번 수행한다.

---

# 작업 Step

## Step 1. Phase 3 기준선을 확정한다

- 진행 상태: **완료**

### AI 작업

- 최근 Test 정리에서 제거한 3개 Play Mode Test의 계약이 더 강한 기존 Test에 포함되는지 재확인한다.
- Edit Mode 285개와 Play Mode 144개의 정적 수를 확인한다.
- Ignore, Explicit, 임의 성공, 조건부 제외, 중복 GUID와 누락 `.meta`를 검사한다.
- Runtime, Scene, Input Action Asset과 Package가 Phase 2 완료 시점 이후 의도하지 않게 변경되지 않았는지 확인한다.

### 사용자 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
2. 전체 Play Mode Test 144개를 실행하고 모두 성공하는지 확인한다.

Edit Mode 생산 코드와 Test는 최근 기준선 이후 변경되지 않았으므로 이 Step에서 재실행하지 않는다. 정적 검사에서 영향이 발견되면 AI가 전체 Edit Mode 실행 필요성을 별도로 명시한다.

### 완료 조건

- [x] Phase 3 시작 전 정적 기준선이 확인되었다.
- [x] Unity Script Compilation이 통과한다.
- [x] 전체 Play Mode 144개가 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

### 기준선 확인 결과 (20260907)

- 사용자가 Step 1에서 요구한 테스트 전체 성공과 예상하지 않은 Error 및 Warning 부재를 보고했다. Unity 실행 결과는 사용자 보고에 근거하며 AI가 실행한 결과가 아니다.
- AI 정적 재집계: Edit Mode 285개, Play Mode 144개. `[Test]`, `[UnityTest]`와 각 `[TestCase]`를 집계했다.
- Assets 파일 및 폴더의 `.meta` 누락 0개, Assets `.meta`의 중복 GUID 0개를 확인했다.
- Tests에서 Ignore/Explicit 속성, Assert.Pass/Ignore/Inconclusive, 조건부 컴파일 및 동적 TestCaseSource/ValueSource 사용을 찾지 못했다.
- `git diff 5e93fe0 --name-only -- Assets Packages ProjectSettings`에 내용 변경이 없다. 이번 작업은 생산 코드, Scene, Input Action Asset과 Package를 변경하지 않았다.
- `5e93fe0`의 Test 변경과 현재 Test 본문을 비교했다. 제거된 `StageMode_PauseAndResume_PreservesSameRun`과 `InfiniteMode_PauseAndResume_PreservesSameRun`의 동일 Run 유지 경로는 `AutoMovementIntegrationTests.StagePauseResume_FreezesPositionTimerAndClearsTransientInput` 및 `InfinitePauseResume_FreezesPositionDistanceAndScore`에서 물리 진행 전후까지 검증한다. 상태 및 Action Map 전환은 `GameLifecycleIntegrationTests`와 `GamePauseOrchestrationTests`가 함께 검증한다.
- 제거된 `PausedState_Retry_StartsIndependentRunInSameMode`의 새 Run 생성 경로는 `AutoMovementIntegrationTests.StagePausedRetry_Twice_CreatesIndependentAutomaticRuns`에서 두 번 반복하고 이동 초기화까지 검증한다. UI 상태와 Mode별 Retry는 `PauseMenuIntegrationTests`가 보완한다.
- Edit Mode 변경 영향이 없어 전체 Edit Mode 재실행은 요청하지 않는다.

## Step 2. 미정 Collectible 규칙을 결정한다

- 진행 상태: **완료 (규칙 결정 및 문서 반영)**

### AI 작업

- 아래 결정 항목마다 구현 가능한 제안, 권장안과 장단점을 채팅으로 제시한다.
- 사용자가 선택하기 전에는 수치나 소유 구조를 추측하여 생산 코드에 반영하지 않는다.
- 결정 결과를 관련 System 및 Feature 문서와 이 Task에 반영한다.

### 결정 항목

1. Collectible 한 개의 Score와 모든 Collectible의 동일 값 사용 여부
2. Stage Mode Collectible Score와 InfiniteMode Collectible Score의 Runtime 소유 위치
3. InfiniteMode 기존 거리 Score와 Collectible Score의 분리 및 Phase 3 임시 합산 범위
4. Collectible 고유 식별 방식과 같은 Run 중복 획득 판정
5. Retry, 새 Run과 Infinite Pattern 재사용 시 Collectible 복구 규칙
6. Pause, Ending과 Ended 상태에서 Trigger가 들어온 경우 처리 규칙
7. Player 판정 방식과 Layer, Tag 또는 Component 기준
8. 획득 후 표시 상태와 Phase 3에서 허용할 최소 시각 표현
9. Stage 및 Pattern별 Collectible 개수와 안내 배치 원칙
10. 획득하지 못한 Collectible의 처리와 Stage 진행 영향 없음 규칙

### 사용자 작업

AI가 제시한 항목별 권장안 또는 대안을 선택한다.

### 완료 조건

- [x] 구현에 필요한 모든 Collectible 규칙이 확정되었다.
- [x] Phase 3와 Phase 4의 Score 및 UI 경계가 확정되었다.
- [x] 관련 문서가 결정 결과와 일치한다.

### 결정 결과 (20260907)

- 사용자가 채팅으로 제시한 10개 항목의 권장안 A를 모두 승인했다.
- 1번 점수, 3번 Phase 경계, 4번 식별, 5번 복구, 6번 상태별 판정, 7번 Player 판정, 8번 표시, 9번 배치와 10번 미획득 처리는 `AI/03_Features/ScoreCollectible.md`에 반영했다.
- 2번 공통 Runtime 소유 위치는 `AI/02_Systems/RuntimeDataSystem.md`에 반영했다.
- 기존 거리 Score와 Result 계약의 경계는 `AI/03_Features/InfiniteMode.md`와 `AI/03_Features/ScoreRecord.md`에 반영했다.
- 개당 10점은 기능 검증용 초기값으로 승인되었으며 최종 밸런스는 별도 작업이다.
- 점프 구간별 5개 묶음은 초기 배치 후보로 승인되었다. 실제 총개수와 좌표는 이후 조사 및 도달 가능성 검증으로 확정한다.
- 구체적인 클래스 구성과 기존 Player 식별 구조 재사용은 Step 3에서 조사한다.
- 이 결정 시점의 완료는 규칙 결정과 문서 반영만 의미한다. 이후 Step 1 검증 결과와 Step 3 조사 결과는 각 Step에 별도로 기록한다.

## Step 3. 기존 Score와 생명주기 경로를 정적으로 조사한다

- 진행 상태: **완료 (정적 조사 및 변경 지점 확정)**

### AI 작업

- GameRuntimeData, InfiniteModeRuntimeData, ScoreCalculator, ScoreRecord, ResultData와 ResultSystem의 Score 흐름을 추적한다.
- GameSystem의 Start, Pause, Resume, End와 Retry 호출 순서를 조사한다.
- StageSystem, InfiniteMapPattern과 InfinitePatternBoundary의 초기화 및 재사용 경로를 조사한다.
- Stage 및 Infinite Root, Player Collider와 Layer, Pattern Anchor의 생산 Scene 참조를 YAML로 검사한다.
- 새 책임이 필요한 위치와 기존 구조를 확장할 위치를 확정한다.
- 필요한 Edit Mode 및 Play Mode Test 목록과 Scene 변경 후보를 작성한다.

### 사용자 작업

없음. 코드, Asset과 Scene의 읽기 전용 정적 검사로 처리한다.

### 완료 조건

- [x] 입력부터 Trigger, Runtime Data와 Result까지의 경로가 확인되었다.
- [x] Pattern 재사용과 Run 초기화 경로가 확인되었다.
- [x] Test 우선 변경 지점과 조건부 Scene 작업이 확정되었다.

### 기존 입력·Score·Result 경로

아래는 현재 코드에서 확인한 경로다. 아직 존재하지 않는 Collectible 경로는 다음 변경 지점 표에서 구분한다.

| 구간 | 확인한 경로와 근거 |
|---|---|
| 입력 → 이동 | `Systems/PlayerInputSystem.cs`의 Jump/MomentumLanding performed Callback → `GetInputState` → `Systems/PlayerMovementSystem.cs`의 Playing 상태 `FixedUpdate` → `GatherMovementStepInput` → `CalculateMovementResult` → `PlayerControllerSystem.ApplyMovement` → Rigidbody. Move Action은 비활성화하며 수평 이동은 `CalculateAutoHorizontalSpeed`를 사용한다. |
| 기존 Goal Trigger | `Features/StageGoal.cs.OnTriggerEnter`는 지정 Collider와 같은 객체인지 확인하고 GoalReached를 전달한다. `StageSystem.HandleGoalReached` → `EndStage` → GameSystem의 StageEnded 수신으로 이어진다. |
| 기존 Pattern Trigger | `Features/InfinitePatternBoundary.cs.OnTriggerEnter` → `InfiniteMapPattern.TryAdvance(boundaryId)`. 지정 Collider와의 직접 비교이며 다중 Player Collider의 일반 판정 구조는 아니다. |
| Runtime 생성 | `RuntimeDataSystem.CreateRuntimeData(mode)` → `GameRuntimeData.Initialize(mode)`. 이동 Data는 두 Mode에 생성하고 `InfiniteModeRuntimeData`는 Infinite에서만 생성한다. Collectible Data는 아직 없다. |
| 거리 Score | `InfiniteModeSystem.FixedUpdate` → `ProcessRunMetrics` → `InfiniteDistanceState.TryUpdate(Rigidbody.position.x)` → `ScoreCalculator.TryCalculate` → `InfiniteModeRuntimeData.TryUpdate`. 거리 × 환산 비율을 내림하며 기존 Score는 int 상한으로 제한한다. |
| Infinite 종료 기록 | 저속 또는 추락 판정 → `FinalizeRunMetrics` → `StageSystem.TryEndInfiniteStage` → `GameSystem.HandleStageEnded` → `CreateInfiniteResultData` → `ResultSystem` → `ScoreRecord.TryRecord` → `ResultData`. 최종 거리와 거리 Score를 먼저 확정한 뒤 종료 이벤트를 전달한다. |
| Stage 종료 기록 | Goal 종료 → `GameSystem.HandleStageEnded` → Timer 정지 → `ResultSystem.CreateResultData` → `TimeRecord` → Stage용 `ResultData`. |
| 결과 유지 | `ResultData`는 읽기 전용 프로퍼티를 가지며 ResultSystem과 UI가 참조한다. `GameSystem.EndGame`은 Result 전달 후 Run Runtime Data를 Clear한다. Phase 3 Collectible 점수를 기존 FinalScore에 넣지 않는다. |

코드 경로의 기준 디렉터리는 `Assets/Scripts/Runtime`이다. `ScoreCalculator`, `ScoreRecord`, `ResultData`, `ResultSystem`, 기존 Infinite HUD 및 Result 표시는 Phase 3 생산 변경 대상으로 선정하지 않는다.

### 기존 Run 생명주기와 연결 시점

1. `GameSystem.StartGame`: Initializing → Runtime 생성 → UI/Result 초기화 → Controller(시작 위치 복구), Collision, Stage(Mode Root 전환), Movement, InfiniteMode, Camera 초기화 → Input 초기화 → Ready → StageEnded Listener 등록 → StartStage → Stage Timer 시작 → Player 입력/Camera 활성화 → Playing.
2. Pause: Stage Timer → Stage → InfiniteMode → Movement → Controller Physics 중단 → GameState Paused → 입력과 UI 전환. 전역 timeScale을 바꾸지 않으며 Controller는 FreezeAll을 사용한다.
3. Resume: Controller Physics → Movement → InfiniteMode → Stage → Stage Timer 재개 → GameState Playing → 입력과 UI 전환. 따라서 Collectible의 Resume 겹침 재판정은 `ResumeStage` 내부가 아니라 Playing 전환 성공 이후에 연결해야 한다.
4. 정상 종료: StageEnded 수신 → 기존 Result 생성/전달 → EndGame의 Ending 전환 → Stage/Input/Camera/InfiniteMode/Movement 중단 → Timer 제거 → Runtime Clear → Ended. 직접 EndGame과 Pause Retry는 정상 Clear/Infinite 기록 생성 경로와 다를 수 있다.
5. Retry: Paused이면 EndGame을 먼저 수행하고 Ended에서 같은 선택 Mode로 StartGame을 수행한다. Mode 전환 검증은 기존 `ProductionSceneGameModeTestUtility.RestartInMode`의 End → Mode 변경 → Start 경로를 재사용한다.
6. 시작 실패: `AbortGameStart`가 Stage와 관련 System을 중단하고 Runtime을 제거한다. Collectible의 부분 초기화 및 참조 해제도 이 경로에서 검증해야 한다.

종료 후 Collectible의 늦은 Trigger는 제거된 Runtime을 다시 조회하거나 변경해서는 안 된다. Phase 3에서는 기존 Runtime 제거 계약을 보존하고, Final Collectible Score를 Result에 보관하는 확장은 Phase 4에서 수행한다. 종료 직전 점수와 종료 이후 요청 거부를 구분하여 테스트한다.

### Pattern 초기화와 재사용

- `StageSystem.ApplyModeRootState`는 Infinite 시작 시 Root를 껐다 켠다.
- `InfiniteMapPattern.OnEnable`은 초기 Transform을 이미 보관한 경우 Initialize를 수행한다. 최초 실행은 Start에서 초기화한다. Collectible 연결이 이 암묵적 Start 순서에 의존하지 않도록 StartStage 이전에 준비 완료를 보장한다.
- `Initialize`는 참조 및 Boundary ID 0/1을 검사하고 초기 Transform을 최초 1회 보관한다. `ResetPatterns`는 두 위치/회전, Boundary 상태, trailing index와 AdvanceCount를 초기화한다.
- `TryAdvance`는 현재 앞 Pattern의 Boundary만 허용한다. 뒤 Pattern의 StartAnchor를 앞 Pattern의 EndAnchor에 맞추고 `Physics.SyncTransforms` 후 이동된 Pattern의 Boundary를 Reset한다. 그 다음 trailing index를 교체하고 AdvanceCount를 증가시킨다.
- Collectible은 재배치 전 해당 구간의 획득 판정을 중단하고, 이전 구간 등록을 제거한 다음 위치 정렬 완료 후 새 구간 ID로 복구해야 한다. 누적 Score는 유지한다.
- `OnEnable`은 Collectible 획득 상태 초기화 신호로 사용하지 않는다. 새 Run 준비와 같은 Run의 구간 재사용을 명시적으로 구분한다.

### 생산 Scene 정적 확인

검사 대상은 생산 Scene인 `Assets/Scenes/SampleScene.unity`와 대응 Script `.meta`, `ProjectSettings/TagManager.asset`, `DynamicsManager.asset`이다.

| 대상 | YAML 확인 결과 |
|---|---|
| Mode 설정 | GameSystem Component fileID `1202804280`의 `_selectedGameMode: 1`은 `E_GameMode.Infinite`다. Scene 저장 상태에서는 Stage Root 활성, Infinite Root 비활성이며 실행 시 StageSystem이 선택 Mode에 맞게 전환한다. |
| Stage Root | `World/StageModeRoot`, GameObject `901191967`, Transform `901191968`. StageSystem의 `_stageModeRoot`와 일치한다. |
| Infinite Root | `World/InfiniteModeRoot`, GameObject `172987142`, Transform `172987143`. StageSystem의 `_infiniteModeRoot`와 일치한다. |
| Player | Root `Player`, Transform `2089254631`, CapsuleCollider `2089254634`, Rigidbody `2089254635`. Layer 0(Default), Untagged, Collider는 non-trigger, radius 0.5, height 2, center 0. Rigidbody는 non-kinematic, useGravity 0이다. |
| Player 참조 | Controller, Collision, Goal과 두 Boundary가 같은 Player Rigidbody/Collider 계열을 참조한다. 별도 Player 식별 Component 없이 등록된 Rigidbody와 Collider.attachedRigidbody 소유 관계를 재사용할 수 있다. Controller의 Rigidbody 참조는 현재 private이므로 필요한 읽기 전용 제공 지점은 확장 대상이다. |
| 시작/Goal | `World/StageModeRoot/StartPoint`는 (0, 1.5, 0). Goal은 (18, 1.5, 0), BoxCollider size (1, 2, 4), isTrigger 1이며 StageSystem의 Goal 참조가 일치한다. |
| Pattern 소유 | `World/InfiniteModeRoot/InfiniteMapPattern` Component `203989859` → Pattern_0 Transform `202057602`, Pattern_1 Transform `806441812`. 두 Pattern은 별개의 자식 Object다. |
| Anchor | Pattern_0 위치 X=0, Pattern_1 위치 X=44. 각 StartAnchor local X=-22, EndAnchor local X=22. 첫 End와 둘째 Start의 초기 World X는 22로 맞는다. 참조된 네 Anchor의 부모가 해당 Pattern과 일치한다. |
| Boundary | 각 Pattern/AdvanceBoundary의 local position (-3, 5.5, 0), BoxCollider size (1, 10, 4), isTrigger 1. Component `63845727`/`864348079`의 ID는 0/1이고 같은 MapPattern 및 Player Collider를 참조한다. |
| 이동 설정 | 생산 값은 moveSpeed 8, groundAcceleration 50, airAcceleration 25, maximumHorizontalSpeed 14, gravityAcceleration 25, jumpHeight 3이다. 이 값만으로 최종 경로 도달 가능성을 통과 처리하지 않는다. |
| Layer/충돌 | Ground는 Layer 6이며 CollisionSystem ground mask는 64다. 현재 Player는 Default다. Physics Layer Collision Matrix는 모두 허용이다. Collectible은 Ground Layer를 사용하지 않고 Default 대상 필터와 등록 Player 소유 관계를 함께 사용할 수 있으므로 새 Layer/Tag 생성은 필수가 아니다. |
| 참조 검사 | Scene 내부의 비영(0이 아닌) local fileID 참조 누락 0개. 조사한 프로젝트 Component의 Script GUID는 해당 `.cs.meta`와 연결된다. 패키지 전체 Script의 동작 검증을 의미하지 않는다. |
| Collectible | Runtime Script와 생산 Scene에 Collectible 구현/배치가 없다. 따라서 현재는 획득부터 Runtime까지 연결된 생산 경로가 존재하지 않는다. |

### Test 우선 변경 지점 확정

아래는 정적 조사로 선정한 후속 구현 지점이며 이번 Step에서 생산 코드를 변경한 결과가 아니다.

| 위치 | 신규/확장 범위와 책임 |
|---|---|
| Core/CollectibleRuntimeData.cs (신규) | Scene 비의존 획득 등록, 활성 구간 식별, 중복 거부, Collectible Score와 초기화 상태를 관리한다. 종료된 구간의 요청을 거부하고 활성 구간에 필요한 기록만 유지한다. |
| Core/GameRuntimeData.cs | Mode 공통 CollectibleRuntimeData를 생성·제공하고 Clear에서 해제한다. 기존 InfiniteModeRuntimeData는 거리 전용으로 유지한다. |
| Features/ScoreCollectible.cs (신규) | 개별 Trigger, 등록된 Player 판정, Runtime의 획득 요청 결과에 따른 Renderer/Collider 표시를 담당한다. OnEnable만으로 점수나 획득 상태를 Reset하지 않는다. |
| Systems/StageSystem.cs | 기존 Stage Object 생명주기 책임을 확장해 선택 Mode의 Collectible 준비·연결·해제를 담당한다. 점수 계산은 Runtime의 단일 획득 처리 경로를 사용하고 Stage Clear 조건에는 개수를 추가하지 않는다. 공통 개당 점수 설정은 이 연결 지점 한 곳에서 관리한다. |
| Systems/GameSystem.cs | 기존 Start 초기화 흐름에서 Runtime과 등록 Player를 Stage 준비에 전달한다. Resume의 Playing 전환 성공 후 현재 겹침 재판정을 요청한다. End/Abort에서는 Runtime Clear 전에 Collectible 연결을 해제한다. |
| Systems/PlayerControllerSystem.cs | 기존 등록 Rigidbody를 읽기 전용으로 제공하는 최소 연결만 추가한다. 이동 계산이나 Player의 Scene Layer를 변경하지 않는다. |
| Features/InfiniteMapPattern.cs | 기존 두 Pattern 소유 구조를 재사용해 Collectible 구간 준비/재사용을 명시적으로 호출한다. 위치 이동 전 판정 중단 → 이전 구간 제거 → 정렬 → 새 구간 등록/표시 복구 순서를 보장한다. |
| Features/InfinitePatternBoundary.cs | 기존 재배치 요청 경로를 재사용한다. Collectible Score 계산이나 획득 판정을 Boundary에 추가하지 않는다. 기존 단일 Collider 비교 변경은 이번 범위의 필수 변경이 아니다. |

- Features asmdef는 Core만 참조하고 Systems는 기본 Assembly에서 Features/Core를 사용한다. ScoreCollectible에서 StageSystem 타입을 역참조하지 않고 초기화에 필요한 Core Data와 Unity Rigidbody만 전달받는다.
- 새 전역 Manager, 별도 Collectible System, Player Tag 또는 입력 Action을 추가할 필요가 없다. 명확한 1:1 초기화·복구·해제는 직접 호출하며 새 이벤트를 만들지 않는다.
- Resume 겹침 처리는 현재 물리 접촉을 확인하는 경로로 구현하고 Pause 중 요청 큐를 만들지 않는다. Trigger Stay 또는 명시적 겹침 재검사 방식의 실제 물리 결과를 Step 5 테스트로 확정한다.

### 필요한 Test 목록

| Step / 계층 | 신규 또는 확장 Test와 판정할 계약 |
|---|---|
| Step 4 / Edit Mode | 신규 `CollectibleRuntimeDataTests`: 미초기화/0개/첫 획득/여러 획득, 동일 ID 중복, 미등록·잘못된 ID, 동일 local ID의 다른 구간 독립성, 구간 폐기 후 요청 거부, Reset/새 Run, 점수 설정 및 정수 상한 경계. |
| Step 4 / Edit Mode | `GameRuntimeDataTests` 확장: 두 Mode 생성, Clear, Infinite↔Stage 전환, 이전 Data 참조 무효화, Collectible 점수 증가 시 기존 거리 Score 불변. `InfiniteModeRuntimeDataTests`, `ScoreCalculatorTests`, `ScoreRecordTests`, `ResultDataTests`, `ResultSystemTests`는 기존 거리/결과 계약 회귀로 사용한다. |
| Step 5 / Play Mode | 신규 `ScoreCollectibleTests`: 실제 Rigidbody/Collider 접촉 1회, 지속 접촉, 재진입, 다중 Collider, 동일 Layer의 비 Player, 비활성 대상, None/Initializing/Ready/Paused/Ending/Ended 거부, Resume 시 유지된 겹침만 획득, 파괴·해제 이후 요청 부재. |
| Step 6 / Play Mode | 신규 Collectible 통합 Test와 기존 `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `AutoMovementIntegrationTests`, `PauseMenuIntegrationTests`: 같은 Run 보존, 종료 직전 점수 및 종료 후 거부, Pause Retry/Result Retry/연속 Run/Mode 전환, 시작 실패와 부분 초기화 해제. |
| Step 7 / Play Mode | Stage 생산 구성: ID·참조·Layer/Trigger·표시 복구·실제 접촉, 모두/일부/0개 획득 후 Goal 흐름. 기존 `StageGoalIntegrationTests`, `StageSystemTests`를 회귀 대상으로 사용한다. |
| Step 8 / Play Mode | `InfiniteMapPatternTests` 확장 및 생산 통합: 두 Pattern 독립성, 같은 Object의 새 구간 ID, 재배치 중 판정 차단, 재사용 후 한 번 획득, 오래된 요청 거부, 여러 순환 후 기록 수 제한, 새 Run 첫 활성화/Retry 순서. `InfiniteModeIntegrationTests`와 `InfiniteModeSystemTests`로 기존 거리·종료 흐름을 검증한다. |
| Step 9 / 자동 검증 | 실제 생산 배치 좌표·Collider·이동/Jump 설정을 사용하는 도달 가능성, 순서, 지면 관통 및 진행 차단 검사. 이동 계산식을 테스트에 복제하지 않고 실제 생산 이동 경로를 검증한다. |
| Step 10 / 회귀 | 기존 Jump/MomentumLanding/Wall/Camera, InfiniteHUD, ModeResultDisplay와 UI 입력 테스트를 재사용한다. 이번 조사만으로 신규 테스트나 실행을 요청하지 않는다. |

기존 `InfiniteMapPatternTests.Boundary_*`는 Reflection으로 OnTriggerEnter를 호출한다. 상태 계약 검사로 재사용하되 실제 물리 Trigger 검증을 대체하지 않는다. 신규 Collectible 물리 Test는 실제 접촉 경로로 작성한다. 생산 Mode 전환 준비는 기존 `ProductionSceneGameModeTestUtility`를 재사용한다.

### 조건부 Scene 작업과 사용자 작업

- 현재 Step 3에서 수행할 사용자 수동 작업은 없다. Compile과 Test Runner 재실행도 요청하지 않는다.
- Step 7: `World/StageModeRoot` 아래에 Collectible을 배치해야 한다. 생산 Component 준비 후 Object/Prefab, 고정 ID, Renderer, Collider의 Is Trigger, Layer 및 Transform 좌표를 확정해 안내한다.
- Step 8: `World/InfiniteModeRoot/InfiniteMapPattern/Pattern_0` 및 `Pattern_1` 아래에 Collectible을 배치해야 한다. Pattern별 local ID와 소유 참조는 저장된 YAML로 검사하고 구간 ID는 Runtime이 부여한다.
- 기존 Player Layer/Tag, Goal, Boundary, Anchor와 Input Action Asset을 변경해야 할 근거는 발견하지 않았다.
- 새로운 Component의 실제 Field 이름과 Collider 크기, 총개수 및 좌표는 구현 및 배치 검증 후 확정한다. 아직 존재하지 않는 Inspector 항목을 사용자에게 설정하도록 요청하지 않는다.
- 이후 Scene 변경은 사용자가 Editor에서 수행하고 저장/재개방한다. AI는 저장 결과의 YAML·GUID·고유 ID·Trigger·Layer·부모 관계를 정적으로 검사한다.

## Step 4. Collectible 순수 상태와 Score 규칙을 Test 우선으로 구현한다

- 진행 상태: **완료**

### AI 작업

- Collectible ID 등록, 한 번 획득, 중복 거부와 Reset을 Scene 비의존 Edit Mode Test로 먼저 작성한다.
- 0개, 첫 획득, 여러 개, 중복 ID, 잘못된 ID, 음수 또는 비정상 설정과 정수 상한 등 확정 규칙의 경계값을 검증한다.
- Stage와 InfiniteMode의 Collectible Score 분리 및 기존 거리 Score 보존 계산을 Edit Mode Test로 검증한다.
- 동일 계산을 Test에 복제하지 않고 입력과 관찰 가능한 결과를 검증한다.
- 실패 Test에 필요한 최소 Runtime Data 또는 Feature만 구현한다.

### 사용자 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
2. AI가 지정한 신규 및 영향받는 Edit Mode Test를 실행한다.

### 완료 조건

- [x] Collectible은 같은 Run에서 한 번만 획득된다.
- [x] 잘못된 요청과 중복 요청이 Score를 변경하지 않는다.
- [x] Reset 후 다음 Run의 독립 상태가 생성된다.
- [x] 기존 InfiniteMode 거리 Score가 Collectible Score와 독립적으로 유지된다.

## Step 5. Collectible Trigger와 Player 판정을 Test 우선으로 구현한다

- 진행 상태: **완료**

### AI 작업

- 실제 Collider와 Trigger를 사용하는 Play Mode Test를 먼저 작성한다.
- Player 진입 1회 획득, 중복 Trigger 거부, 비 Player 무시와 비활성 상태 무시를 검증한다.
- 여러 Collider를 가진 Player가 진입해도 한 번만 획득되는지 검증한다.
- Pause, Ending과 Ended 상태에서 Score가 변경되지 않는지 검증한다.
- Callback 등록 및 해제와 Object 파괴 후 잔여 호출 부재를 검증한다.
- 실패 Test를 통과시키는 최소 Collectible Component 및 연결 책임을 구현한다.

### 완료 조건

- [x] 실제 Trigger 경로에서 Player만 Collectible을 획득한다.
- [x] 한 Run의 중복 획득 및 다중 Collider 중복 Score가 차단된다.
- [x] 플레이 불가 상태에서는 획득되지 않는다.
- [x] 생성 및 파괴 후 Callback 잔류가 없다.

### 필요한 사용자 수동 작업

1. `Assets/Scenes/SampleScene.unity`를 열고 Hierarchy의 `World/StageModeRoot`를 선택한다.
2. `StageModeRoot` 아래에 **3D Object > Sphere**를 생성하고 이름을 정확히 `StageCollectible_ResumeTest`로 지정한다.
3. Transform을 아래와 같이 설정한다.
   - Local Position: `(4, 1.5, 0)`
   - Local Rotation: `(0, 0, 0)`
   - Local Scale: `(1, 1, 1)`
4. Layer는 `Default`, Tag는 `Untagged`로 설정한다.
5. SphereCollider를 아래와 같이 설정한다.
   - Is Trigger: 활성화
   - Center: `(0, 0, 0)`
   - Radius: `0.5`
6. `ScoreCollectible` Component를 추가하고 아래 Field를 설정한다.
   - Collectible Id: `stage-resume-01`
   - Trigger Collider: 같은 Object의 SphereCollider
   - Visual: 같은 Object의 MeshRenderer
   - Player Layers: `Default`만 선택
7. Scene을 저장하고 닫았다 다시 연 뒤 Missing Reference가 없는지 확인한다.
8. Scene 작업 완료를 AI에게 알린다. AI의 YAML 정적 검사 후 Unity Script Compilation과 관련 Play Mode Test를 실행한다.

Scene 저장 전에는 Test를 실행하지 않는다. 구현 당시 관련 Test는 `ScoreCollectibleTests` 27개와 `ProductionScoreCollectibleIntegrationTests` 1개였으며 전체 Play Mode Test는 172개였다. Edit Mode Test는 이 Step에서 수행하지 않는다.

### 사용자 수동 작업 및 검증 결과

- `Assets/Scenes/SampleScene.unity`의 `World/StageModeRoot/StageCollectible` 아래에 Stage Collectible 생산 구성을 배치했다.
- YAML 정적 검사에서 계층, Transform, Default Layer, Untagged Tag, SphereCollider Trigger, `ScoreCollectible` 스크립트 GUID와 모든 직렬화 참조가 지정값과 일치했다.
- Unity Script Compilation이 성공했고 예상치 못한 Error와 Warning이 없었다.
- Edit Mode Test 314개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.
- Play Mode Test 172개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.
- 이후 테스트 검토에서 실제 System 연결 없이 Scope 생성과 Bind를 직접 수행하던 임시 `ProductionScoreCollectibleIntegrationTests`를 제거했다. 저장 Scene 이름 결합 대신 Step 6의 실제 Run 생명주기 통합 Test가 해당 책임을 검증한다.
- `ScoreCollectibleTests`의 비활성 Object와 같은 Runtime 재초기화 사례는 각각 기존 Disable/Unbind 및 Runtime 무효화 Test와 중복되어 제거했다. 현재 Collectible 관련 Play Mode Test는 25개이며 전체 Play Mode 정적 예상 수는 169개다.

## Step 6. Mode 공통 Runtime Data와 Run 생명주기를 통합한다

- 진행 상태: **완료**

### AI 작업

- Stage Mode와 InfiniteMode가 같은 Collectible 획득 원칙을 사용하도록 GameRuntimeData와 System 경계를 연결한다.
- Pause 전후 획득 상태 및 Score 보존, Result 이후 불변, Retry와 연속 Run 초기화를 Play Mode Test로 먼저 검증한다.
- Stage와 InfiniteMode 전환 시 이전 Mode의 Collectible 상태가 남지 않는지 검증한다.
- InfiniteMode 거리, 거리 Score와 종료 기록이 기존 계약을 유지하는지 검증한다.
- Phase 4의 HUD 및 Result 표시 책임을 조기 구현하지 않는다.

### 사용자 작업

1. Unity Script Compilation을 확인한다.
2. Edit Mode Test 전체 314개를 실행한다.
3. Play Mode Test 전체 174개를 실행한다.
4. 각 단계에서 예상치 못한 Error와 Warning이 없는지 확인한다.

### 구현 및 정적 검사 결과

- `PlayerControllerSystem`이 기존 Scene 참조의 Player Rigidbody를 제공하고, `GameSystem`이 생성된 `GameRuntimeData`와 함께 `StageSystem`에 전달하도록 연결했다.
- `StageSystem`은 선택된 Mode Root의 활성 `ScoreCollectible`을 Run Scope에 자동 등록하고 Start 실패, End, Abort와 파괴 시 Runtime Clear 전에 해제한다.
- Pause와 Resume은 기존 Scope 및 Score를 유지하며, Playing 전환 성공 후 현재 Player 겹침을 한 번 재검사한다.
- Retry와 Mode 전환은 이전 Runtime 및 Scope를 폐기하고 새 Collectible Run을 생성한다. 비활성 Mode Root의 Collectible은 등록하지 않는다.
- `CollectibleLifecycleIntegrationTests` 5개를 추가해 자동 Bind, Pause/Resume 보존, End 해제, Retry 초기화와 Stage→Infinite Mode 전환을 생산 경로로 검증하도록 했다.
- 기존 Collectible Play Mode Test 25개를 포함해 전체 Play Mode 정적 예상 수는 174개다.
- Scene과 Unity Test Runner는 AI가 실행하거나 수정하지 않았다.
- Unity Script Compilation이 성공했고 예상치 못한 Error와 Warning이 없었다.
- Edit Mode Test 314개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.
- Play Mode Test 174개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.

### 완료 조건

- [x] 두 Mode의 Collectible 상태와 Score가 Run 단위로 관리된다.
- [x] Pause와 Resume은 같은 Run 상태를 보존한다.
- [x] Retry, 새 Run과 Mode 전환은 이전 획득 상태를 제거한다.
- [x] InfiniteMode 거리 Score와 종료 흐름에 회귀가 없다.

## Step 7. Stage Mode Collectible 생산 구성을 확정한다

- 진행 상태: **완료**

### AI 작업

- Prefab, Material, Collider, Layer와 Component 구성을 정적으로 검사한다.
- Stage의 자동 이동 속도, Jump 궤적, Collider 크기와 Platform 위치로 Collectible 후보 위치의 도달 가능성을 계산하거나 Test한다.
- 생산 Scene 변경이 필요한 경우 사용자에게 GameObject, Parent, Component, Field, 값과 배치 좌표 단위로 절차를 제공한다.
- Scene 저장 후 YAML로 개수, ID 고유성, 참조, Trigger와 Layer를 검증한다.

### 사용자 Scene 작업

정적 검사 결과 생산 구성을 위해 아래 Scene 작업이 필요하다.

1. `Assets/Scenes/SampleScene.unity`를 열고 `World/StageModeRoot` 아래에 빈 GameObject `StageCollectible`을 만든다. Local Position과 Local Rotation은 `(0, 0, 0)`, Local Scale은 `(1, 1, 1)`로 설정한다.
2. 기존 `StageCollectible_ResumeTest`와 복제할 Collectible 9개는 모두 `StageCollectible`의 직접 자식으로 둔다.
3. 기존 Object 이름을 `StageCollectible_Jump01_03`으로 바꾸고 Local Position을 `(4.25, 4.46, 0)`으로 변경한다. `ScoreCollectible`의 Collectible Id를 `stage-jump-01-03`으로 변경한다.
4. 이 Object를 9개 복제해 다음 이름, ID와 Local Position을 설정한다.

| GameObject | Collectible Id | Local Position |
| --- | --- | --- |
| `StageCollectible_Jump01_01` | `stage-jump-01-01` | `(0.75, 1.5, 0)` |
| `StageCollectible_Jump01_02` | `stage-jump-01-02` | `(2.5, 3.58, 0)` |
| `StageCollectible_Jump01_04` | `stage-jump-01-04` | `(6, 4.15, 0)` |
| `StageCollectible_Jump01_05` | `stage-jump-01-05` | `(6.8, 2.5, 0)` |
| `StageCollectible_Jump02_01` | `stage-jump-02-01` | `(7.9, 2.5, 0)` |
| `StageCollectible_Jump02_02` | `stage-jump-02-02` | `(9.65, 4.58, 0)` |
| `StageCollectible_Jump02_03` | `stage-jump-02-03` | `(11.4, 5.46, 0)` |
| `StageCollectible_Jump02_04` | `stage-jump-02-04` | `(13.15, 5.15, 0)` |
| `StageCollectible_Jump02_05` | `stage-jump-02-05` | `(16.3, 1.5, 0)` |

5. 10개 모두 Local Rotation `(0, 0, 0)`, Local Scale `(1, 1, 1)`, Layer `Default`, Tag `Untagged`인지 확인한다.
6. 각 Object의 SphereCollider는 Is Trigger 활성화, Center `(0, 0, 0)`, Radius `0.5`로 유지한다.
7. 각 `ScoreCollectible`의 Trigger Collider와 Visual이 반드시 같은 Object의 SphereCollider와 MeshRenderer를 가리키고 Player Layers는 `Default`만 선택되어 있는지 확인한다.
8. Scene을 저장하고 재개방하여 Missing Reference가 없는지 확인한 뒤 작업 완료를 알린다. AI가 저장된 YAML을 정적으로 검사할 때까지 Test Runner는 실행하지 않는다.

### 사용자 Test 작업

1. Unity Script Compilation을 확인한다.
2. AI의 Scene YAML 정적 검사 후 Play Mode Test 전체 175개를 실행한다.
3. Compilation과 Test에서 예상치 못한 Error와 Warning이 없는지 확인한다.

### 정적 조사 및 Test 준비 결과

- Stage Ground는 중심 `(0, 0)`, 폭 40이고 `Platform_01`은 중심 `(6, 1)`, `Platform_02`는 중심 `(12, 2)`, Goal은 `(18, 1.5)`다.
- Player 중심 시작점은 `(0, 1.5)`, Capsule 높이는 2이며 기본 수평 속도 8, Jump 높이 3, 중력 가속도 25다.
- 초기 수직 속도 `sqrt(2 × 25 × 3)`와 기본 수평 속도를 사용해 두 점프의 공중 안내 좌표를 계산했다. 착지 안내는 Platform과 Goal 전 Ground 높이에 맞췄다.
- `StageCollectibleLayoutIntegrationTests` 1개를 추가해 `StageModeRoot/StageCollectible` 컨테이너, 생산 Scene의 10개 고유 ID, 좌표, Sphere Trigger, 자체 참조, Default Layer와 계산된 점프 궤적을 검증하도록 했다.
- 기존 Stage Goal 및 Stage 생명주기 Test가 Collectible을 놓쳐도 Goal과 종료 흐름이 유지되는 회귀 검증을 담당한다.
- 저장된 YAML에서 `StageCollectible` 컨테이너의 원점 Transform, 자식 10개, 고유 이름 10개와 고유 ID 10개를 확인했다. 각 자식의 부모 참조는 동일한 컨테이너 Transform을 가리킨다.
- Scene과 Unity Test Runner는 AI가 수정하거나 실행하지 않았다.
- Unity Script Compilation이 성공했고 예상치 못한 Error와 Warning이 없었다.
- Edit Mode Test 314개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.
- Play Mode Test 175개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.

### 완료 조건

- [x] Stage Collectible 구성과 참조가 정적으로 유효하다.
- [x] ID가 고유하고 Trigger 및 Player 판정 설정이 일치한다.
- [x] 기본 이동 및 Jump 범위 안에서 획득 가능함이 자동 검증된다.
- [x] Collectible을 놓쳐도 Goal과 Stage 종료 흐름이 유지된다.

## Step 8. InfiniteMode Pattern Collectible 생산 구성을 확정한다

- 진행 상태: **완료**

### AI 작업

- Pattern별 Collectible 소유, ID와 Pattern 순환 시 복구 경로를 Play Mode Test로 먼저 검증한다.
- 앞 Pattern과 뒤 Pattern의 Collectible이 독립적으로 한 번씩 획득되는지 검증한다.
- 같은 Pattern Object가 재배치될 때 확정 규칙에 따라 다음 구간 Collectible로 복구되는지 검증한다.
- Pattern 경계 Trigger와 Collectible Trigger가 서로 간섭하지 않는지 검증한다.
- 생산 Scene 변경이 필요한 경우 사용자에게 Pattern, 자식 Object, Field와 좌표 단위로 절차를 제공한다.
- Scene 저장 후 YAML로 Pattern별 개수, 참조, ID 규칙과 Trigger 구성을 검증한다.

### 사용자 Scene 작업

정적 검사 결과 생산 구성을 위해 아래 Scene 작업이 필요하다.

1. `Assets/Scenes/SampleScene.unity`에서 `World/InfiniteModeRoot/InfiniteMapPattern/Pattern_0`과 `Pattern_1`을 찾는다.
2. 각 Pattern 바로 아래에 빈 GameObject `PatternCollectible`을 하나씩 만들고 Local Position과 Local Rotation을 `(0, 0, 0)`, Local Scale을 `(1, 1, 1)`로 설정한다.
3. Stage의 `StageModeRoot/StageCollectible` 자식 10개를 각 `PatternCollectible` 아래로 복제한다. 원본 Stage Object는 이동하거나 삭제하지 않는다.
4. 두 Pattern에서 각 자식 이름을 `PatternCollectible_Jump01_01`부터 `PatternCollectible_Jump02_05`까지 구분되게 정리한다.
5. 두 Pattern 모두 `ScoreCollectible`의 Collectible Id에서 `stage-` 접두사를 제거해 다음 10개 local ID를 사용한다. Pattern Scope가 다르므로 Pattern_0과 Pattern_1은 같은 local ID 목록을 사용한다.
   - `jump-01-01`, `jump-01-02`, `jump-01-03`, `jump-01-04`, `jump-01-05`
   - `jump-02-01`, `jump-02-02`, `jump-02-03`, `jump-02-04`, `jump-02-05`
6. 복제한 자식의 Local Position은 Stage 원본과 동일하게 유지한다.
   - `(0.75, 1.5, 0)`, `(2.5, 3.58, 0)`, `(4.25, 4.46, 0)`, `(6, 4.15, 0)`, `(6.8, 2.5, 0)`
   - `(7.9, 2.5, 0)`, `(9.65, 4.58, 0)`, `(11.4, 5.46, 0)`, `(13.15, 5.15, 0)`, `(16.3, 1.5, 0)`
7. 20개 모두 Layer `Default`, Tag `Untagged`, SphereCollider Is Trigger 활성화, Center `(0, 0, 0)`, Radius `0.5`, Player Layers `Default`인지 확인한다.
8. 각 Trigger Collider와 Visual이 반드시 같은 자식 Object의 SphereCollider와 MeshRenderer를 가리키는지 확인한다.
9. Scene을 저장하고 재개방하여 Missing Reference가 없는지 확인한 뒤 작업 완료를 알린다. AI가 YAML을 정적으로 검사할 때까지 Test Runner는 실행하지 않는다.

### 사용자 Test 작업

1. Unity Script Compilation을 확인한다.
2. AI의 Scene YAML 정적 검사 후 Play Mode Test 전체 179개를 실행한다.
3. Compilation과 Test에서 예상치 못한 Error와 Warning이 없는지 확인한다.

### 구현 및 정적 조사 결과

- `InfiniteMapPattern`이 Pattern_0과 Pattern_1의 Collectible Scope를 따로 생성·해제하도록 구현했다.
- Pattern 재사용은 이전 Scope와 Bind를 해제한 뒤 Transform을 정렬하고 새 Scope로 해당 Pattern의 Collectible만 복구한다. 누적 Collectible Score는 유지한다.
- `StageSystem`은 InfiniteMode에서 `InfiniteMapPattern`에 Collectible 준비, Resume 겹침 재검사와 종료 해제를 위임한다.
- `InfiniteMapPatternTests`에 Pattern별 동일 local ID 독립 획득, 재사용 복구와 Collectible Trigger/Boundary 비간섭 Test 3개를 추가했다.
- `InfiniteCollectibleLayoutIntegrationTests` 1개를 추가해 두 Pattern의 컨테이너, 각 10개 local ID 및 좌표, Trigger, Layer, 자체 참조와 전체 등록 수 20을 검증하도록 했다.
- 두 Pattern은 길이 44, 동일 Terrain과 Platform 좌표를 사용하므로 Stage에서 검증한 두 점프의 local 배치를 공통으로 적용한다. Boundary는 local `(-3, 5.5, 0)`에 있어 양의 X 구간 Collectible Trigger와 공간적으로 겹치지 않는다.
- 기존 Play Mode 175개에 신규 4개가 추가되어 전체 정적 예상 수는 179개다.
- Scene과 Unity Test Runner는 AI가 수정하거나 실행하지 않았다.
- 저장된 YAML에서 Pattern_0과 Pattern_1의 `PatternCollectible` Local Position이 모두 `(0, 0, 0)`이며, 각 컨테이너에 고유 local ID 10개와 유효한 자체 참조가 있음을 확인했다.
- Unity Script Compilation이 성공했고 예상치 못한 Error와 Warning이 없었다.
- Edit Mode Test 314개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.
- Play Mode Test 179개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.

### 완료 조건

- [x] 두 Pattern의 Collectible 상태가 독립적이다.
- [x] Pattern 재사용 시 획득 가능 상태가 정해진 시점에 복구된다.
- [x] 중복 획득과 중복 Score가 발생하지 않는다.
- [x] 거리 Score, Pattern 이동과 InfiniteMode 종료 흐름이 유지된다.

## Step 9. 안내 경로의 도달 가능성을 자동 검증한다

- 진행 상태: **완료**

### AI 작업

- 생산 Scene 및 Pattern의 Collectible 위치를 추출한다.
- Player 자동 속도, Jump 높이 및 지속 시간, Collider 범위와 Platform 표면을 기준으로 기본 경로의 도달 가능성을 검증한다.
- 점프 시작 안내, 공중 경로와 착지 안내 구간의 순서가 역전되거나 통과 불가능한 위치가 없는지 Test한다.
- 배치 개수, 간격, 지면 관통, Collider 중첩과 경로 밖 좌표를 정적으로 검사한다.
- 실패 근거가 있는 위치만 사용자에게 수정 좌표로 제시한다.

### 사용자 작업

자동 검증에서 연속 묶음의 Sphere Trigger 중첩이 확인되어 다음 세 Object만 수정한다.

1. `StageModeRoot/StageCollectible/StageCollectible_Jump01_05`의 Local Position을 `(6.8, 2.5, 0)`으로 변경한다.
2. `Pattern_0/PatternCollectible/PatternCollectible_Jump01_05`의 Local Position을 `(6.8, 2.5, 0)`으로 변경한다.
3. `Pattern_1/PatternCollectible/PatternCollectible_Jump01_05`의 Local Position을 `(6.8, 2.5, 0)`으로 변경한다.
4. 다른 Collectible 좌표와 Component 설정은 변경하지 않고 Scene을 저장한 뒤 작업 완료를 알린다.

### 정적 검증 결과

- 생산 Scene에서 Stage 10개와 두 Infinite Pattern의 각 10개 위치를 추출했다.
- 기본 수평 속도 8, Jump 높이 3, 중력 가속도 25, Player Capsule과 Sphere Trigger 반지름을 기준으로 두 점프의 높이 및 순서를 검사했다.
- 모든 지점의 X 순서는 증가하고 Z는 0이며 Ground 또는 Platform 아래로 관통하는 위치는 없다.
- `Jump01_05 (7.2, 2.5)`와 `Jump02_01 (7.9, 2.5)`의 간격 0.7이 Sphere 반지름 합 1.0보다 작아 Trigger가 겹치는 문제를 발견했다.
- `Jump01_05`를 `(6.8, 2.5, 0)`으로 조정하면 다음 지점과 간격이 1.1이 되어 겹침이 제거되고 Platform_01 착지 안내 범위는 유지된다.
- 저장된 YAML에서 Stage와 두 Infinite Pattern의 `Jump01_05`가 모두 `(6.8, 2.5, 0)`으로 수정된 것을 확인했다. 전체 인접 Trigger 간격은 최소 1.1이며 순서 역전과 중첩이 없다.
- Stage 및 Infinite 생산 레이아웃 Test에 X 순서, 경로 하한과 인접 Sphere Trigger 비중첩 검사를 추가했다. Test 개수는 변하지 않아 전체 Play Mode 예상 수는 179개다.
- Scene과 Unity Test Runner는 AI가 수정하거나 실행하지 않았다.
- Unity Script Compilation이 성공했고 예상치 못한 Error와 Warning이 없었다.
- Edit Mode Test 314개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.
- Play Mode Test 179개가 모두 성공했고 예상치 못한 Error와 Warning이 없었다.

### 완료 조건

- [x] Stage 및 InfiniteMode 기본 경로의 Collectible이 도달 가능하다.
- [x] Collectible 순서가 점프 시작, 공중 이동과 착지 흐름에 맞는다.
- [x] 통과 불가능한 점프와 진행 차단 배치가 없다.

## Step 10. 기존 기능, Phase 4 경계와 전체 회귀를 검증한다

- 진행 상태: **완료**

### AI 작업

- Jump, Momentum Landing, 자동 이동, Wall 낙하, Goal, Pattern, Pause, Retry, Camera, Result와 UI 입력 회귀 Test를 검토하고 필요한 Test만 보강한다.
- Collectible을 모두 획득, 일부 획득 또는 모두 놓친 경우 Stage Play를 계속할 수 있는지 검증한다.
- InfiniteMode 거리 Score 및 기존 Result Data가 Phase 3 변경 전 계약을 유지하는지 검증한다.
- Stage Collectible Score, Infinite Collectible Score와 Total Score의 HUD 및 Result 표시는 Phase 4 범위로 남아 있는지 정적으로 확인한다.
- 유사 Test는 계약과 실행 경로를 비교하여 중복을 만들지 않는다.
- Runtime, Test, Prefab, Material과 대응 `.meta` 및 GUID를 검사한다.
- Collectible ID 고유성, Scene 참조, Trigger, Layer, Pattern 자식 구성과 활성 상태를 검사한다.
- Ignore, Explicit, 임의 성공, 조건부 제외, 약화된 기대값과 불필요한 중복 Test를 검사한다.
- Update, FixedUpdate와 Trigger 반복 경로의 LINQ, 매 Frame 컬렉션 생성과 정상 흐름 Log를 검사한다.
- 관련 System 및 Feature 문서와 구현 일치를 확인한다.
- Package와 Input Action Asset의 의도하지 않은 변경을 확인한다.
- Phase 4 및 Prototype 4 범위가 포함되지 않았는지 확인한다.
- `git diff --check`를 수행한다.

### 정적 검증 결과

- 기존 Jump, Momentum Landing, 자동 이동, Wall 낙하, Goal, Pattern, Pause, Retry, Camera, Result와 UI 입력 계약을 담당하는 Test를 검토했다. Collectible Test와 실행 경로가 중복되지 않아 추가 Test는 만들지 않았다.
- Stage Goal 종료 경로는 Collectible 획득 수와 Score를 참조하지 않는다. 모두 획득, 일부 획득 또는 모두 놓친 상태가 Stage 진행 및 Goal 도달을 차단하지 않는 구조임을 확인했다.
- Infinite Result는 기존 `InfiniteModeRuntimeData.CurrentScore`를 사용하며 Collectible Score와 결합되지 않는다. 기존 `ResultData` 생성자와 표시 계약도 유지된다.
- Collectible Score, Total Score의 HUD 및 Result 표시는 구현되지 않았으며 Phase 4 범위로 유지된다.
- Runtime 및 Test의 대응 `.meta`가 모두 존재하고 중복 GUID가 없다. 생산 Scene의 `ScoreCollectible` Script 참조 30개도 동일한 유효 GUID를 사용한다.
- 생산 Scene에는 Stage 10개와 Pattern별 10개씩 총 30개의 Collectible이 있다. 참조 누락, 비활성 오브젝트, 비 Trigger Collider, 잘못된 Player Layer Mask가 없으며 ID는 각 Scope 안에서 고유하다.
- Ignore, Explicit, 임의 성공과 Collectible 관련 조건부 제외 또는 약화된 기대값이 없다. 기존 Test와 책임이 같은 불필요한 Test도 없다.
- Collectible Trigger 반복 경로에 LINQ, 매 Frame 컬렉션 생성과 정상 흐름 Log가 없다. 기존 `GameSystem.Update`에도 Phase 3에서 추가된 할당이나 Log가 없다.
- 관련 System 및 Feature 문서가 구현된 수명 주기, Score 분리와 Pattern 재사용 규칙을 반영한다.
- Package와 Input Action Asset에는 내용 변경이 없다. Phase 4 및 Prototype 4 구현도 포함되지 않았다.
- `git diff --check`는 코드와 문서에서 통과했다. Scene에서 보고되는 항목은 Unity YAML의 빈 `m_Name` 및 `m_EditorClassIdentifier` 직렬화 후행 공백뿐이며, Scene을 AI가 수정하지 않는 원칙에 따라 Unity 직렬화 형식을 유지했다.

### 사용자 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
2. 전체 Edit Mode Test를 실행하고 모두 성공하는지 확인한다.
3. 전체 Play Mode Test를 실행하고 모두 성공하는지 확인한다.
4. Test 관련 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 완료 조건

- [x] Phase 1 및 Phase 2 기능 회귀가 없다.
- [x] Collectible 획득 여부가 Stage 진행을 차단하지 않는다.
- [x] 기존 InfiniteMode 거리 Score와 Result 계약이 유지된다.
- [x] Phase 4 UI 및 Total Score 기능이 조기 포함되지 않았다.
- [x] 전체 정적 검증이 통과한다.
- [x] 전체 Edit Mode 및 Play Mode Test가 통과한다.
- [x] 예상하지 않은 Compile 및 Test Error와 Warning이 없다.

## Step 11. 최소 화면을 검증하고 완료 근거를 정리한다

- 진행 상태: **완료**

### 화면 검증 전 AI 작업

- 생산 Scene, Asset과 Scene 참조 및 전체 검증 결과를 확인한다.
- 획득 수, Score, 중복 처리, 초기화와 정확한 위치 판정은 자동 Test 결과를 사용하고 수동 체크리스트에서 제외한다.

### 화면 검증 전 정적 검증 결과

- Step 10 이후 Runtime, Test와 Scene 기능 변경이 없으므로 사용자 Compilation, Edit Mode 314개와 Play Mode 179개 전체 통과 결과가 유효하다.
- 생산 Scene의 `ScoreCollectible` 30개에 Script, ID, Trigger Collider와 Visual 참조가 모두 있고 누락 참조가 없다.
- Stage 10개와 Infinite Pattern별 10개 구성, 위치 순서, Trigger 비중첩과 도달 가능성은 Step 7~10의 Scene 정적 검사 및 자동 Test 결과를 재사용했다.
- 코드와 문서의 `git diff --check`가 통과했다. Unity YAML 빈 직렬화 필드의 후행 공백만 기존 Scene 예외로 남아 있다.
- 획득 수, Score 값, 중복 방지, Pause, Retry, Pattern 재사용과 기존 거리 Score는 이미 자동 검증되어 수동 확인 대상에서 제외했다.

### 사용자 최소 화면 검증

1. Unity Editor에서 `Assets/Scenes/SampleScene.unity`를 열고 Play Mode를 시작한다.
2. Stage Mode에서 Collectible이 점프 시작, 공중 이동 경로와 착지 지점을 알아보기 쉽게 안내하는지 확인한다.
3. Stage Collectible 일부를 놓쳐도 플레이와 Goal 도달이 자연스러운지 확인한다.
4. InfiniteMode에서 Pattern Collectible이 반복 구간에도 보이고 이동 경로를 자연스럽게 안내하는지 확인한다.
5. Collectible이 화면에서 구분되고 눈에 띄는 떨림, 순간 이동 또는 부자연스러운 겹침이 없는지 확인한다.
6. 기존 자동 이동, Jump, Momentum Landing, Pause, Retry, Wall 낙하와 Camera 추적에 체감 회귀가 없는지 한 번 확인한다.
7. Console에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 화면 검증 후 AI 작업

- 최종 정적 검증, Compile, 전체 Test와 최소 화면 결과를 기록한다.
- Asset 및 Scene 변경과 미해결 사항을 기록한다.
- 별도 Phase 3 Verification Result Task 문서를 작성한다.
- 모든 완료 조건을 충족한 경우에만 Roadmap Phase 3를 `완료`로 변경한다.

### 사용자 화면 검증 결과

- Stage Mode Collectible의 점프 시작, 공중 이동 경로와 착지 지점 안내가 정상임을 확인했다.
- Stage Collectible 일부를 놓쳐도 플레이와 Goal 도달이 정상임을 확인했다.
- InfiniteMode Pattern 반복 구간의 Collectible 표시와 경로 안내가 정상임을 확인했다.
- Collectible 구분, 표시 안정성과 배치가 정상임을 확인했다.
- 자동 이동, Jump, Momentum Landing, Pause, Retry, Wall 낙하와 Camera 추적에 체감 회귀가 없음을 확인했다.
- Play Mode Console에 예상하지 않은 Error와 Warning이 없음을 확인했다.

### 완료 조건

- [x] 최소 화면 검증 결과가 기록되어 있다.
- [x] 정적 검증, Compile과 전체 Test가 통과한다.
- [x] Phase 3 범위 밖 기능이 포함되지 않았다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 Unity Script Compilation 및 전체 Play Mode 기준선 확인
2. Step 2의 미정 Collectible 규칙 선택
3. 구현 Step 이후 AI가 지정한 Unity Script Compilation 확인
4. AI가 지정한 관련 및 전체 Unity Test Runner 실행
5. Step 7~9에서 정적 검사로 필요성이 확인된 Stage 및 Infinite Pattern Scene 배치와 저장
6. Scene 변경 후 재개방과 Missing Reference 확인
7. Step 11에서 두 Mode의 안내성, 가독성, 자연스러운 배치와 기존 조작감 확인
8. Play Mode Console의 예상하지 않은 Error 및 Warning 확인

Collectible ID, 획득 횟수, Score 값, 합산, 중복 방지, Pause 및 Retry 상태, Pattern 복구 시점, 정확한 좌표 도달 가능성과 기존 거리 Score는 수동 작업에 포함하지 않고 정적 검증 또는 Unit Test로 처리한다.

---

# 영향 범위

- GameRuntimeData와 Mode별 Runtime Data
- Collectible 상태 및 Score 계산 Feature
- Collectible Trigger 및 획득 연결 System
- GameSystem, StageSystem, InfiniteModeSystem과 ResultSystem 생명주기
- InfiniteMapPattern과 InfinitePatternBoundary
- Stage 및 InfiniteMode 생산 Scene 구성
- 조건부 Collectible Prefab, Material, Collider와 Layer
- Edit Mode 및 Play Mode Test
- 관련 System 및 Feature 문서

---

# 검증 내용

- Roadmap Phase 3 목표와 완료 조건을 11개 실행 Step으로 분리했다.
- 정적 검사, Edit Mode Test, Play Mode Test와 수동 검증의 책임을 구분했다.
- 구현 책임마다 Test 우선 순서와 완료 조건을 배치했다.
- Scene 작업은 정적 검사로 필요성이 확인된 경우에만 사용자에게 요청하도록 제한했다.
- Phase 4의 HUD, Result와 Total Score 표시 및 Prototype 4의 Pattern 확장을 제외했다.
- 현재 테스트 정리 후 기준선 검증을 Phase 3 구현 전에 수행하도록 배치했다.

## Step 수 적정성 검토

- 기존 Step 10의 관련 기능 회귀와 기존 Step 11의 전체 정적 및 자동 회귀는 같은 구현 완료 시점과 같은 검증 대상을 사용하므로 하나의 Step으로 통합했다.
- Step 1은 문서 작성 당시 최근 Test 정리 결과가 Unity에서 검증되지 않아 Phase 3 결함과 기존 기준선 결함을 구분하기 위해 배치했다.
- Step 2는 미정 Score, 식별, 복구와 배치 규칙을 구현 전에 확정해야 하므로 유지한다.
- Step 3은 기존 Score 및 Pattern 생명주기 조사 결과가 이후 구조와 Scene 작업 범위를 결정하므로 구현 Step과 분리한다.
- Step 4~6은 순수 상태 및 계산, 실제 Trigger, Mode 및 Run 생명주기라는 서로 다른 책임과 Test 계층을 가지므로 각각 유지한다.
- Step 7과 Step 8은 고정 Stage 배치와 재사용 Infinite Pattern 배치의 초기화 규칙이 달라 분리한다.
- Step 9는 두 Mode의 배치가 완료된 뒤 전체 안내 경로를 함께 검증하고 실패 좌표만 수정하기 위해 유지한다.
- Step 10은 관련 회귀, 전체 정적 검사와 전체 Test를 한 번에 수행하여 중복 Compile 및 Test 실행을 방지한다.
- Step 11은 Test로 판정할 수 없는 가독성과 안내성만 검증하므로 자동 검증과 분리한다.
- 11개 Step에 Phase 3 완료 조건의 누락이 없고 동일한 검증 책임의 중복도 없다.

---

# 검증 결과

- Phase 3 수행 순서와 사용자 수동 작업 범위가 정의되었다.
- 자동 판정 가능한 항목은 정적 검증 및 Unit Test 범위로 배치되었다.
- Scene 배치와 화면 안내성처럼 Editor 또는 실제 Player가 필요한 항목만 수동 검증으로 남겼다.
- Step 2의 10개 권장안이 사용자 승인으로 확정되었으며 관련 System 및 Feature 문서에 반영되었다.
- Step 1의 사용자 테스트 통과 보고와 정적 기준선 확인을 기록했다.
- Step 3의 기존 코드 및 Scene 조사, 변경 지점, 자동 Test 목록과 조건부 Scene 작업 정리를 완료했다.
- Step 4의 순수 Collectible Data와 GameRuntimeData 소유 연결 및 Edit Mode 테스트를 구현했다. 정적 검사와 사용자 Compilation, Edit Mode 314개 및 Play Mode 144개 검증이 통과하여 Step 4를 완료했다.
- Step 5의 생산 Scene 수동 구성, YAML 정적 검사와 Unity 검증이 완료되었다.
- Step 6의 Mode 공통 Collectible Run 생명주기 연결과 사용자 Compilation, Edit Mode 314개 및 Play Mode 174개 검증이 완료되었다.
- Step 7의 Stage Collectible 생산 구성, YAML 정적 검사와 사용자 Compilation, Edit Mode 314개 및 Play Mode 175개 검증이 완료되었다.
- Step 8의 Infinite Pattern Collectible 생산 구성, Pattern별 Scope 재사용, YAML 정적 검사와 사용자 Compilation, Edit Mode 314개 및 Play Mode 179개 검증이 완료되었다.
- Step 9의 Stage 및 InfiniteMode 안내 경로 도달 가능성, 순서와 Trigger 비중첩 검증 및 사용자 Compilation, Edit Mode 314개와 Play Mode 179개 검증이 완료되었다.
- Step 10의 기존 기능 회귀, Collectible과 Stage 진행의 독립성, Infinite 거리 Score와 Result 계약, Phase 4 경계, 자산 및 Scene 참조 검토가 완료되었다. Step 9 완료 직후 동일 변경 상태에서 확인한 사용자 Compilation, Edit Mode 314개와 Play Mode 179개 전체 통과 결과를 사용했다.
- Step 11의 Stage 및 InfiniteMode 최소 화면 검증과 기존 조작 체감 회귀 검증이 완료되었고 예상하지 않은 Console Error와 Warning이 없었다.
- Unity Editor와 Test Runner를 AI가 실행하지 않았다.

---

# 후속 작업

별도 Phase 3 Verification Result 문서에서 완료 근거를 확인하고 Phase 4 실행 계획을 작성한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/README.md`
- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/03_Features/README.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/ScoreCollectible.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/90_Tasks/Prototype_3/20260904_02_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_3/20260907_08_Phase2VerificationResult.md`
- `AI/90_Tasks/Prototype_3/20260909_01_Phase3VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260828_04_Prototype3Roadmap.md`
- `AI/90_Tasks/Prototype_3/20260907_09_Phase3ManualSteps.md`

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 3의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 검증보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 작업으로 넘기지 않았다.
- Scene 작업을 정적으로 필요성이 확인된 최소 범위로 제한했다.
- Phase 4, Prototype 4와 밸런스 확장 범위를 분리했다.
- 확인되지 않은 Collectible Score 값과 배치 좌표를 확정값으로 작성하지 않았다.
