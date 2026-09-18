# 작업 정보

## 작업명

Prototype 5 Phase 3 Step 1 생산 Rebase 연결 지점 및 Scene 참조 조사

---

## 작업 일자

20260917

---

## 작업 담당자

AI

---

# 작업 목적

Phase 3 World Rebase 구현 전에 생산 코드의 실행 순서와 `SampleScene`의 기존 참조·계층을 읽기 전용으로 확인한다.

---

# 작업 대상

- `InfiniteModeSystem`의 InfiniteMode FixedUpdate 실행 경로
- `PlayerControllerSystem`, `InfiniteMapPattern`, `CameraSystem`의 현재 API와 참조
- `Assets/Scenes/SampleScene.unity`의 Player, InfiniteModeRoot, Pattern Slot, Boundary, Collectible, CameraRig 및 Follow Target 계층

---

# 작업 전 상태

- `WorldRebaseState`에는 임계값 `880`, Offset 계산과 누적 논리 거리 상태가 구현되어 있다.
- 생산 `InfiniteModeSystem`은 `WorldRebaseState`를 소유하거나 호출하지 않는다.
- Player, Infinite World 및 Camera의 생산 Rebase 이동 API는 구현되어 있지 않다.

---

# 조사 내용

## 현재 InfiniteMode 실행 순서

`InfiniteModeSystem.FixedUpdate`는 InfiniteMode Playing에서 아래 순서로 실행된다.

1. `ProcessRunMetrics`: Player Rigidbody X로 거리와 Score를 갱신·게시한다.
2. `ProcessMomentumStep`
3. `PublishRunMetrics`
4. `ProcessProgress`
5. `ProcessFallThreshold`
6. `ProcessPatternProgression`: 현재 거리로 Difficulty를 갱신하고 다음 Pattern 요청을 처리한다.

현재 거리 초기화와 갱신은 `InfiniteDistanceState`에 Player Rigidbody의 물리 X를 직접 전달한다. Rebase 구현은 거리·Score를 한 번 반영한 뒤, 같은 논리 거리를 Difficulty와 Pattern 진행에 전달하고, 이후 Player·World·Camera 이동 및 물리 동기화를 수행하는 위치에 연결되어야 한다.

## 이동 대상 책임

| 대상 | 소유 System 또는 Component | Phase 3 API 책임 |
|---|---|---|
| Player Rigidbody | `PlayerControllerSystem` | 유효한 음의 X Offset 적용 및 Rigidbody 물리 상태 보존 |
| InfiniteModeRoot | `InfiniteMapPattern` | Root 한 번 이동으로 Slot, 활성 Pattern, Anchor, Boundary 및 Pattern 자식 Collectible을 함께 이동 |
| CameraRig와 Follow Target | `CameraSystem` | CameraRig 이동 및 Cinemachine Target Warp 통지 |
| 실행 순서·Offset 계산 | `InfiniteModeSystem` | 상태 검사, 거리 반영, 세 대상 요청, 물리 동기화 및 실패 상태 해제 |

`StageSystem.InfiniteMapPattern`은 InfiniteMode 시작 후 생산 `InfiniteMapPattern`을 제공한다. 따라서 InfiniteModeRoot를 위한 별도 직렬화 참조는 필요하지 않다.

`InfiniteModeSystem`에는 현재 `PlayerControllerSystem`과 `CameraSystem` 참조가 없다. 두 System의 생산 API를 직접 요청하려면 해당 두 직렬화 필드를 추가하고 Scene의 기존 Component를 연결해야 한다.

`CameraSystem`은 이미 `_followTarget`을 보유하고 있으며, Scene에서 그 부모는 `CameraRig`이다. CameraRig 전용 직렬화 필드는 필요하지 않다. 구현은 기존 Follow Target 참조에서 부모 CameraRig를 검증해 사용해야 한다.

## SampleScene 읽기 전용 계층 검사

- Player는 최상위 GameObject이며 `PlayerControllerSystem`의 `_playerRigidbody`는 Player Rigidbody를 참조한다.
- `StageSystem._infiniteModeRoot`는 `World/InfiniteModeRoot`를 참조한다.
- `InfiniteModeRoot`의 유일한 자식은 `InfiniteMapPattern`이다.
- `InfiniteMapPattern`의 두 Slot은 `_firstSlot`, `_secondSlot`으로 연결되어 있고, 각 Slot은 Content Root와 `AdvanceBoundary`를 가진다. Boundary는 Player Collider와 같은 `InfiniteMapPattern`을 참조한다.
- Pattern의 Collectible은 활성 Pattern 자식에서 Scope로 바인딩된다. 따라서 `InfiniteModeRoot` 이동은 Collectible의 Transform만 이동하며 Scope·획득 상태를 재생성하지 않는다.
- `CameraRig`는 최상위 Root이며 자식으로 `CameraFollowTarget`과 Cinemachine Camera를 가진다. `CameraFollow`는 Player와 Follow Target을 참조하고, `CameraSystem`도 같은 Follow Target과 Cinemachine Camera를 참조한다.
- Player, InfiniteModeRoot 및 CameraRig는 서로 부모·자식 관계가 아니다. 새 공통 부모를 만들지 않고 각각의 소유 API를 통해 같은 Offset을 적용해야 한다.
- Stage Mode Root, Goal, Start Point, UI Canvas 및 EventSystem은 InfiniteModeRoot·CameraRig의 자식이 아니며 Rebase 대상이 아니다.

## 변경하지 않을 범위

- Stage Mode 자동 이동, Move 입력 비활성, Goal 및 Stage Root
- Phase 2 Momentum, Score UI, Result UI 및 Scoring Version `2`
- Pattern 선택 규칙, Boundary 진행 규칙과 Collectible Scope 생명주기

---

# 작업 내용

- 관련 System·Feature 문서, 생산 C# 및 `SampleScene.unity`를 읽기 전용으로 조사했다.
- Unity Editor, Unity Test Runner, Build 및 Scene 수정은 수행하지 않았다.
- 조사 결과를 이 작업 기록에 정리했다.

---

# 영향 범위

- Tasks: Phase 3 Step 1 조사 결과 기록

---

# 검증 내용

- C# 직렬화 필드와 API 존재 여부를 정적으로 확인했다.
- `SampleScene.unity` YAML에서 System 참조와 대상 계층을 읽기 전용으로 대조했다.

---

# 검증 결과

- Rebase 전후 생산 실행 순서와 이동 대상 책임을 확인했다.
- Player·InfiniteModeRoot·CameraRig의 기존 계층 관계와 Rebase 제외 대상을 확인했다.
- 현재 코드에는 Phase 3 생산 연결과 이동 API가 없음을 확인했다.
- Unity Compile, Test Runner, Build 및 화면 검증은 이번 Step의 수행 대상이 아니며 실행하지 않았다.

---

# 후속 작업

- Step 2에서 `InfiniteModeSystem`의 논리 거리·Rebase 상태 및 `PlayerControllerSystem`, `CameraSystem` 직렬화 참조를 구현한다.
- Step 3과 Step 4에서 세 이동 API와 원자적 실행 순서를 구현한다.
- 구현과 정적 검증이 끝난 뒤에만 사용자가 `InfiniteModeSystem`의 새 PlayerControllerSystem·CameraSystem 필드에 기존 Component를 연결한다.

---

# 관련 문서

- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260916_14_Phase3ManualSteps.md`

---

# 작성 완료 기준

- Step 1에서 실제로 확인한 생산 코드와 Scene YAML만 기록했다.
- Scene, Build 및 Test Runner를 변경하거나 실행하지 않았다.
