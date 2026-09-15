# 작업 정보

## 작업명

Prototype 5 Phase 1 Step 1 정적 조사

## 작업 일자

20260915

## 작업 담당자

AI

## 작업 상태

완료

---

# 작업 목적

Momentum Landing에서 InfiniteMode Score Result까지의 변경 영향 경로와 World Rebase에 영향을 받는 World 좌표 의존 구조를 구현 전에 확인한다.

---

# 작업 대상

- Momentum Landing 성공과 수평 속도 반영 경로
- 이동 거리, Distance Score, Collectible Score, Total Score와 Result 전달 경로
- Pause, Result, Retry와 새 Run의 현재 상태 생명주기
- Player, Camera, Infinite Pattern Slot, Boundary와 Collectible의 World 좌표 의존성
- 후속 변경이 필요한 기존 Edit Mode 및 Play Mode Test
- 사용자 정책 결정 항목과 정적으로 확정 가능한 현재 사실의 분리

---

# 작업 전 상태

- Momentum Landing은 성공 시 수평 속도를 배율로 증가시키고 최대 수평 속도로 제한한다.
- InfiniteMode 이동 거리는 Run 시작 Player World X와 현재 Player World X의 차이 중 최댓값이다.
- Distance Score는 이동 거리와 단위 점수만 사용하며 Momentum 보너스를 별도로 보유하지 않는다.
- World Rebase 상태와 Scoring Version은 구현되어 있지 않다.

---

# 조사 내용

## Momentum Landing에서 이동 결과까지

| 단계 | 현재 동작 | 근거 |
|---|---|---|
| Window 설정 | `MomentumLandingFeature` Inspector 값은 Window `0.15`, 속도 배율 `1.15`, 최대 속도 `14`이다. | `MomentumLandingFeature.cs`, `SampleScene.unity` |
| 성공 판정 | 점프 중 한 번 활성화된 Window에서 입력을 Buffer하고 실제 Ground 접촉 때 한 번만 성공한다. | `MomentumLandingFeature.UpdateWindow`, `BufferInput`, `TryCompleteLanding` |
| 속도 효과 | 성공 시 `abs(horizontalSpeed) * speedMultiplier`를 최대 속도로 제한하고 기존 부호를 복원한다. | `MomentumLandingFeature.TryCompleteLanding` |
| 이동 반영 | `PlayerMovementSystem.ResolveLanding`이 반환 속도를 `MovementCalculation.HorizontalSpeed`에 대입한 뒤 Controller와 Runtime Data에 전달한다. | `PlayerMovementSystem.ResolveLanding`, `ApplyMovementResult` |
| Runtime 상태 | 마지막 착지가 Momentum인지 Bool로만 보유한다. 연속 성공 횟수나 Score 배율 상태는 없다. | `PlayerMovementRuntimeData` |
| 초기화 | 새 초기화, 이동 종료와 새 Jump에서 Feature 상태가 초기화된다. Pause와 Resume은 이동 및 Feature 상태를 보존한다. | `PlayerMovementSystem.Initialize`, `StopMovement`, `PauseMovement`, `ResumeMovement`; `MomentumLandingFeature.BeginJump` |

현재 Momentum 성공은 Score 계층에 전달되지 않는다. `PlayerMovementRuntimeData.IsLastLandingMomentum`은 UI, ScoreCalculator, ScoreRecord가 소비하지 않는다.

## Score와 Result 영향 경로

```text
Player Rigidbody.position.x
  -> InfiniteDistanceState 최대 전진 거리
  -> ScoreCalculator(distance * scorePerUnit, 내림, int.MaxValue 포화)
  -> InfiniteModeRuntimeData(CurrentDistance, CurrentScore)
  -> GameSystem.HandleStageEnded
  -> ResultSystem.CreateInfiniteResultData
  -> ScoreRecord(Total = DistanceScore + CollectibleScore, int.MaxValue 포화)
  -> ResultData
  -> UIManagementSystem / ResultTextFormatter
```

- `InfiniteModeSystem`의 `_scorePerUnit` 생산 Scene 값은 `10`이며 `ScoreCalculator`가 이동 거리만 입력받는다.
- Base Distance Score와 Momentum Bonus를 구분하는 Runtime 또는 Result 필드는 없다.
- Collectible Score는 `CollectibleRuntimeData`가 Scope와 ID를 기준으로 별도 누적하며 개당 기본 `10`점이고 `int.MaxValue`에서 포화한다.
- `ScoreRecord`만 Distance Score와 Collectible Score를 합산하며 Collectible Score에 Momentum 배율을 적용하는 경로는 없다.
- `ResultData`는 Final Distance, Distance Score, Collectible Score와 Total Score만 가진다. Momentum Bonus와 Scoring Version 필드는 없다.
- Pause는 Runtime Data를 유지하고 계산을 중단한다. Result는 종료 직전 거리와 Score를 확정한다. Retry와 새 Run은 기존 Runtime Data를 제거하고 새로 생성하며 `ResultSystem.Initialize`가 이전 기록을 초기화한다.

## World 좌표 의존 구조

| 대상 | 현재 World 좌표 의존 | Rebase 영향 |
|---|---|---|
| Player | `Rigidbody.position.x`가 거리 원점과 현재 진행 위치이며 `position.y`가 추락 판정이다. | Rebase 시 Rigidbody 물리 위치 이동과 논리 거리 보존이 필요하다. Y는 X Rebase와 독립적으로 유지해야 한다. |
| Distance | `InfiniteDistanceState`가 `float originWorldX`와 현재 World X의 차이를 최대 거리로 저장한다. | World X를 줄이면 현재 수식만으로 Rebase 이후 진행 거리를 이어갈 수 없다. 누적 Offset 또는 논리 좌표가 필요하다. |
| Difficulty | `InfiniteModeSystem`이 `InfiniteDistanceState.CurrentDistance`를 그대로 `InfiniteDifficultyState`에 전달한다. | 논리 누적 거리를 제공하면 기존 `220/440` 경계 계약을 재사용할 수 있다. |
| Pattern Slot | 뒤 Slot을 앞 Pattern `EndAnchor.position`에 맞춰 World 위치로 이동하며 이동 후 `Physics.SyncTransforms()`를 호출한다. | 활성 두 Slot과 그 자식 Pattern을 함께 같은 Offset으로 옮겨야 Anchor 연결이 유지된다. |
| Boundary | Slot 자식이지만 `ContentRoot` 밖에 있고 현재 Pattern의 World `AdvanceBoundaryPoint.position/rotation`에 정렬된다. | Slot 이동에 포함되어야 하며 Rebase 뒤 Trigger 중복 상태와 물리 동기화 계약이 필요하다. |
| Collectible | Pattern 자식 위치를 사용하지만 식별과 점수는 World X가 아니라 발급된 `long scopeId`와 Local ID를 사용한다. | Transform은 Slot과 함께 이동해야 한다. Scope ID와 획득 점수는 Rebase 때문에 초기화하면 안 된다. |
| Camera Follow Target | 매 LateUpdate에 Follow Target World X를 Player Transform World X로 덮어쓴다. | Rebase 프레임에 Player와 Follow Target/Camera의 상대 위치 보존 및 Cinemachine 보정 계약이 필요하다. |
| Camera Rig | 생산 Scene에서 `CameraRig`는 별도 Scene Root이다. | `World` 계층 이동만으로 Camera Rig가 이동하지 않는다. 이동 대상 API가 필요하다. |
| World 계층 | 생산 Scene에서 `InfiniteMapPattern`은 `World/InfiniteModeRoot` 아래지만 Player와 CameraRig는 각각 별도 Root이다. | 단일 기존 Root 이동만으로 전체 Rebase 대상을 처리할 수 없다. World Root 재구성 또는 명시적 다중 대상 이동 중 정책 결정이 필요하다. |

`StageSystem`과 `InfiniteModeSystem`의 추락 판정은 World Y만 사용하므로 X Rebase 횟수와 직접 결합되지 않는다. Pattern 선택 및 Collectible Scope도 Rebase 횟수를 Score나 Difficulty 입력으로 사용하지 않는다.

## 기존 Test 영향

### Momentum 속도 효과를 현재 계약으로 기대하는 Test

- `MomentumLandingFeatureTests.TryCompleteLanding_BufferedInput_AppliesSignedMultiplier`
- `MomentumLandingFeatureTests.TryCompleteLanding_ResultSpeed_DoesNotExceedMaximum`
- `MomentumLandingIntegrationTests.MomentumLanding_WindowInput_AppliesMomentumLanding`

위 Test는 Phase 2에서 속도 증가 계약 제거에 따라 교체 또는 수정이 필요하다. Window, 입력 시점, Ground 접촉, 한 점프 한 번 판정 Test는 재사용 가능하다.

### 절대 World X 또는 기존 거리 수식을 기대하는 Test

- `InfiniteDistanceStateTests` 전체는 `OriginWorldX`와 현재 World X의 차이를 계약으로 사용한다.
- `InfiniteModeSystemTests.ProcessRunMetrics_PlayerWorldX_UpdatesDistanceAndScore`
- `InfiniteModeSystemTests.ProcessRunMetrics_LargeWorldX_WorksWithoutPatternData`
- `InfiniteModeIntegrationTests.PlayerFallsAtLargeX_EndsAndStopsPlaySystems`
- `InfiniteModeIntegrationTests.RestartAfterMovement_*_UsesPhysicsOriginAndDistance`
- `InfiniteMapPatternTests`의 Slot World X 배치 및 Reset 검증
- `InfinitePatternConnectionIntegrationTests`의 Slot `0/44`, Player 절대 X, Anchor와 Collider Bounds X 검증
- `CameraFollowIntegrationTests`의 Player, Follow Target과 Camera World X 동일성 검증

기존 연결부의 상대 간격과 Camera 상대 위치 검증은 유지할 수 있지만, Rebase 이후에도 같은 관계가 유지되는 새 Test가 Phase 3에 필요하다.

### Score 및 생명주기 회귀 Test

- `ScoreCalculatorTests`: 선형 거리 점수, 내림, 비정상 입력과 포화
- `ScoreRecordTests`: 두 Score 합산, 중복 기록 거부, 포화와 Reset
- `InfiniteDifficultyStateTests`: `220/440` 경계, 단조 진행, Pause/Result/새 Run
- `InfiniteModeRuntimeDataTests`: 거리와 Score 단조 증가, 확정과 Clear/Initialize
- `InfiniteModeIntegrationTests`: Result 전달, Collectible 분리, Retry 독립성

이 Test들은 새 Base Distance Score, Momentum Bonus, Scoring Version과 논리 거리 계약에 맞춰 확장 또는 변경해야 한다.

## 사용자 결정이 필요한 항목

- Momentum 배율 단계, 증가량, 상한과 연속 성공 단위
- 일반 착지, Wall 접촉, 낙하와 시간 경과에 따른 배율 초기화
- Pause, Result, Retry와 새 Run에서 배율 상태의 보존 또는 초기화
- HUD와 Result에 표시할 Base Distance Score, Momentum Bonus와 배율 정보
- Rebase 임계값, 이동 Offset, 실행 기준점과 실행 가능한 게임 상태
- 기존 World 계층 재구성 여부와 Player, Camera, Pattern, Trigger의 이동 방식
- 누적 논리 거리 자료형과 표시 변환
- Rebase 프레임의 Rigidbody, Trigger, Cinemachine 처리
- Scoring Version 값, 소유 위치와 Roadmap 7 기록 분리 정책

## 정적으로 확정 가능한 현재 사실

- Momentum Landing 속도 증가 제거는 `MomentumLandingFeature.TryCompleteLanding`과 `PlayerMovementSystem.ResolveLanding` 사이가 직접 변경 경계이다.
- 현재 Score는 거리 기반 Score와 Collectible Score가 분리되어 있고 Collectible은 Momentum 상태를 참조하지 않는다.
- 거리, Difficulty와 Distance Score는 같은 `InfiniteDistanceState.CurrentDistance` 흐름에 연결되어 있다.
- Player, Pattern/Boundary와 Camera는 하나의 공통 Scene Root에 묶여 있지 않다.
- Collectible 식별과 Scope는 절대 World X에 의존하지 않는다.
- Scoring Version, Momentum 연속 상태, Momentum Bonus와 Rebase Offset은 현재 코드와 Result Data에 존재하지 않는다.

---

# 작업 내용

- 관련 Project, Rule, Feature, System과 Roadmap 문서를 Runtime 코드에 대조했다.
- Runtime C#과 기존 Edit Mode 및 Play Mode Test를 정적으로 검색하고 직접 확인했다.
- `SampleScene.unity`를 읽기 전용으로 확인해 Inspector 값과 주요 Root 계층을 대조했다.
- 실행 계획 문서에서 Step 1 완료 조건만 갱신했다.
- Runtime 코드, Test와 Scene은 변경하지 않았다.

---

# 영향 범위

- Tasks: Step 1 조사 결과와 계획 진행 상태
- 후속 Feature: MomentumLanding, InfiniteMode, ScoreRecord
- 후속 System: PlayerMovementSystem, InfiniteModeSystem, ResultSystem, CameraSystem
- 후속 Test: Momentum, Distance, Score, Difficulty, Pattern, Camera와 Result 관련 Test

---

# 검증 내용

- 문서와 코드의 현재 동작을 정적 대조했다.
- Runtime 전체의 위치 읽기 및 위치 변경 지점을 정적 검색했다.
- 기존 Test의 속도 증가, 절대 World X와 Score 구성 기대를 정적 검색했다.
- `git diff --check` 대상으로 문서 변경 형식을 확인한다.
- Unity Editor, Unity Test Runner와 Build는 실행하지 않았다.

---

# 검증 결과

- Step 1의 세 완료 조건을 충족했다.
- Step 1은 읽기 및 문서 기록 작업이므로 사용자 수동 작업이 없다.
- 구현과 자동 Test 실행은 수행하지 않았다.
- Scene, Prefab, Runtime 코드와 Test 파일은 변경하지 않았다.

---

# 후속 작업

- Step 2에서 Momentum Landing Score 정책을 사용자와 확정한다.
- Step 3에서 World Rebase와 누적 논리 거리 정책을 사용자와 확정한다.
- Step 4에서 Scoring Version과 호환 정책을 사용자와 확정한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_4/20260914_04_Phase4VerificationResult.md`

---

# 작성 완료 기준

- Momentum Landing에서 Score Result까지의 현재 영향 경로를 기록했다.
- Rebase 대상과 World 좌표 의존 코드 및 Test를 기록했다.
- 사용자 결정 항목과 정적으로 확인 가능한 현재 사실을 분리했다.
- 수행하지 않은 구현, Test Runner, Scene 작업과 Build를 완료로 기록하지 않았다.
