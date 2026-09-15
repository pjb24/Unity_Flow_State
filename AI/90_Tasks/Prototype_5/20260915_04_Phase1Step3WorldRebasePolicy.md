# 작업 정보

## 작업명

Prototype 5 Phase 1 Step 3 World Rebase 정책 확정

## 작업 일자

20260915

## 작업 담당자

AI, 사용자

## 작업 상태

완료

---

# 작업 목적

InfiniteMode의 큰 World X를 원점 부근으로 이동하면서 누적 거리, Score, Difficulty, Pattern, Collectible, Physics와 Camera 상태를 유지할 World Rebase 계약을 확정한다.

---

# 작업 대상

- Rebase 임계값, Offset, 기준점과 실행 상태
- 누적 논리 거리 자료형과 계산식
- Player, InfiniteModeRoot와 CameraRig의 이동 책임
- Pattern, Boundary와 Collectible 상태 보존
- Rebase 프레임의 Physics와 Cinemachine 처리
- Retry와 새 Run 초기화

---

# 작업 전 상태

- Player World X와 Run 시작 World X의 `float` 차이를 최대 전진 거리로 사용했다.
- Player, InfiniteModeRoot와 CameraRig는 서로 다른 Scene Root였다.
- Pattern Slot은 World 위치로 재배치되고 Boundary와 Collectible은 Slot 계층에 포함됐다.
- World Rebase Offset과 실행 상태가 없었다.

---

# 조사 내용

- Pattern 길이는 `44`이고 두 Slot은 `44` 간격으로 재사용된다.
- Difficulty 경계는 누적 거리 `220/440`이다.
- `880`은 Pattern 길이의 `20`배이며 두 Difficulty 경계 이후에 첫 Rebase가 발생한다.
- Collectible 식별과 획득 상태는 World X가 아니라 Scope ID와 Local ID를 사용한다.
- Cinemachine 패키지 버전은 `3.1.7`이다.

---

# 작업 내용

## 확정 정책

- Player Rigidbody World X가 `880` 이상이면 Rebase를 실행한다.
- 기본 Offset은 `880`이고 큰 X 입력은 필요한 `880` 배수를 한 번에 적용한다.
- Rebase 기준은 Player Rigidbody의 물리 X 위치이다.
- 유효한 InfiniteMode Playing 상태에서만 실행한다.
- Rebase 전 현재 위치까지 논리 거리와 Score 증가분을 먼저 반영한다.
- 현재 논리 X는 `Player World X + 누적 Rebase Offset`이다.
- 최대 전진 거리는 Run 시작 논리 X 이후 현재 논리 X가 도달한 최댓값이다.
- 논리 위치와 누적 Offset은 `double`, Unity World 위치는 `float`을 사용한다.
- Player Rigidbody, InfiniteModeRoot와 CameraRig를 같은 Offset으로 이동한다.
- 새로운 공통 World Root를 만들지 않는다.
- Pattern ID, 요청, 선택, AdvanceCount와 Boundary 상태를 보존한다.
- Collectible Scope, ID, 획득 상태와 Score를 보존한다.
- 모든 이동 후 Physics Transform을 한 번 동기화한다.
- Player Rigidbody의 속도, 회전, 각속도와 Constraints를 보존한다.
- CameraRig를 함께 이동하고 Cinemachine에 Target Warp를 통지한다.
- Rebase 횟수는 Distance, Difficulty, Score, Pattern 또는 Collectible 보상에 사용하지 않는다.
- Pause와 Result에서는 Rebase하지 않고 Retry와 새 Run에서 Offset, 논리 거리와 실행 상태를 초기화한다.
- 표시 거리는 원본 `double`을 변경하지 않고 UI에서 내림 처리한다.

## 책임 경계

- InfiniteModeSystem은 Rebase 계산과 실행 순서를 조정한다.
- PlayerControllerSystem은 Player Rigidbody 위치 변경과 물리 상태 보존을 담당한다.
- InfiniteMapPattern은 InfiniteModeRoot, Slot, Boundary와 Collectible의 같은 Offset 이동을 담당한다.
- CameraSystem은 CameraRig 이동과 Cinemachine Target Warp 통지를 담당한다.
- Scene 계층은 기존 구조를 유지한다.

---

# 영향 범위

- Feature: InfiniteMode
- System: InfiniteModeSystem, PlayerControllerSystem, CameraSystem
- Task: Phase 1 Step 3 상태와 결정 기록
- 후속 Runtime: 논리 거리, 누적 Offset과 Rebase 상태
- 후속 Test: 거리 계산, 반복 Rebase, Physics, Pattern, Collectible과 Camera 통합

---

# 검증 내용

- 임계값과 Offset이 Pattern 길이의 정수 배수인지 계산했다.
- Rebase 전후 논리 X가 같은 수식인지 대조했다.
- 이동 대상과 제외 대상을 현재 생산 Scene 계층에 대조했다.
- Pattern 및 Collectible 식별 상태가 World X에 의존하지 않는지 확인했다.
- Unity Editor, Unity Test Runner와 Build는 실행하지 않았다.

---

# 검증 결과

- Step 3의 네 완료 조건을 충족했다.
- 정책 문서만 변경했고 Runtime 코드, Test, Scene과 Prefab은 변경하지 않았다.
- 실제 Physics, Trigger와 Camera 연동은 Phase 3 Play Mode 검증 책임으로 남아 있다.

---

# 후속 작업

- Step 4에서 Scoring Version과 호환 정책을 확정한다.
- Step 5에서 누적 논리 거리와 Rebase Offset의 순수 상태 및 계산 API와 Unit Test를 작성한다.
- 생산 Rebase 연결과 Scene 구성 검증은 Roadmap Phase 3에서 수행한다.

---

# 관련 문서

- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_5/20260915_02_Phase1Step1Investigation.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260915_02_Phase1Step1Investigation.md`
- `AI/90_Tasks/Prototype_5/20260915_03_Phase1Step2MomentumScorePolicy.md`

---

# 작성 완료 기준

- Rebase 수치, 상태, 계산식, 이동 대상과 실행 순서를 기록했다.
- 각 System의 Rebase 책임을 구분했다.
- 구현하지 않은 Runtime, Scene, Physics와 Camera 동작을 완료로 기록하지 않았다.
