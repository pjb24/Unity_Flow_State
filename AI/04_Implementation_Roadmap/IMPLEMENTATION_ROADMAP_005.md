# 목적

Flow State의 Momentum Landing 보상을 Score 중심으로 변경하고 장시간 InfiniteMode의 좌표 안정성을 확보한다.

---

# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

5차 프로토타입

Player가 이동 속도를 능동적으로 조절할 수 있도록 Momentum Landing의 속도 증가 효과를 제거하고 연속 성공을 Score 보상으로 전환한다.

InfiniteMode의 World X가 계속 커질 때 발생할 수 있는 Transform·Physics·Camera 정밀도 문제를 누적 논리 거리와 World Rebase 구조로 해결한다.

---

# 개발 단계

## Phase 1

### 목표

Momentum Landing Score 배율과 World Rebase의 계약을 확정한다.

### 구현 대상

- Momentum Landing 성공·연속 성공·초기화 조건
- Distance Score에 적용할 배율 단계와 상한
- Base Distance Score, Momentum Bonus와 Total Score 관계
- Pause·Result·Retry·새 Run의 배율 생명주기
- World Rebase 임계값과 이동 대상 범위
- World 좌표와 누적 논리 거리의 분리
- Rebase 전후 Distance·Difficulty·Pattern·Collectible 계약
- Score 규칙 변경을 구분할 Scoring Version

### 완료 조건

- Momentum Landing이 Player 이동 속도를 증가시키지 않는다고 명시되어 있다.
- 배율은 Distance Score 증가분에 적용하고 Collectible Score에는 적용하지 않는다고 확정되어 있다.
- 배율 단계, 최대값과 초기화 조건을 추측 없이 확인할 수 있다.
- Rebase 이후에도 누적 거리와 Score가 연속되는 계산 기준이 확정되어 있다.
- Player, Camera, Pattern과 Trigger 중 Rebase 대상이 명확하다.
- 이전 Score 기록과 새 Score 기록을 구분하는 Version 규칙이 확정되어 있다.

### 검증 책임

- 수치 경계, 배율 진행·초기화와 포화 계산은 Edit Mode Unit Test 대상으로 정의한다.
- Rebase 반복, 음수·비정상 입력과 누적 거리 보존은 정적 계산 Test 대상으로 정의한다.
- 실제 Scene·Physics·Camera 이동은 이 Phase에서 검증하지 않는다.

### 상태

완료 — Unity Script Compilation 및 전체 Edit Mode Test 559개 통과

---

## Phase 2

### 목표

Momentum Landing을 속도 효과에서 Score 배율 효과로 전환한다.

### 구현 대상

- 기존 Momentum Landing 속도 증가 제거
- Momentum 배율 상태와 Runtime Data
- 이동 거리 증가분 기반 Score 계산
- Base Distance Score와 Momentum Bonus 분리
- 기존 InfiniteHUD와 분리된 우측 하단 Momentum HUD의 현재 배율과 단계별 유지 시간 Gradient Bar
- Result Data와 Result UI의 Momentum Score 정보
- Pause·Resume·Result·Retry 생명주기 연동

### 완료 조건

- Momentum Landing 성공 전후 Player 이동 속도 규칙이 동일하다.
- Stage Mode와 InfiniteMode의 기존 자동 이동 및 Move 입력 비활성 계약을 유지한다.
- 같은 이동 거리에서도 Momentum Landing 성공에 따라 Score가 증가한다.
- Collectible Score는 Momentum 배율의 영향을 받지 않는다.
- Pause와 Result는 현재 배율·Score를 유지하고 Retry와 새 Run은 초기화한다.
- HUD와 Result의 Score 구성 요소 및 Total Score가 일치한다.

### 검증 책임

- 배율 전환, 연속 성공, 초기화, Score 포화와 Version은 Edit Mode Test로 검증한다.
- 실제 Momentum Landing, 일반 착지, Wall 접촉, Pause·Result·Retry와 UI 연동은 Play Mode Test로 검증한다.
- 불필요한 가속이 없는지와 배율 UI 가독성만 화면으로 확인한다.

### 상태

완료 — Unity Script Compilation, Edit Mode `629/629`, Play Mode `221/221` 및 화면 확인 통과

---

## Phase 3

### 목표

InfiniteMode에 누적 논리 거리와 World Rebase를 적용한다.

### 구현 대상

- 누적 Rebase Offset과 논리 이동 거리
- World Rebase 실행 상태와 임계값
- Player·Camera·Infinite Pattern 이동
- Pattern Anchor·Boundary·Collectible Scope 연동
- Rebase 프레임의 Collision과 Camera 보정
- Difficulty와 Score의 논리 거리 사용
- Retry·새 Run의 Rebase 상태 초기화

### 완료 조건

- World X가 임계값을 넘으면 진행 World가 원점 부근으로 이동한다.
- Rebase 전후 누적 거리, Score와 Difficulty가 감소하거나 중복되지 않는다.
- Pattern 연결과 Collectible 획득·재사용이 Rebase와 충돌하지 않는다.
- Player와 Camera의 상대 위치가 유지된다.
- Retry와 새 Run은 Rebase Offset과 논리 거리를 초기화한다.
- Rebase 횟수는 Score 또는 Difficulty에 직접 사용되지 않는다.

### 검증 책임

- 여러 번 Rebase한 누적 거리와 경계값은 Edit Mode Test로 검증한다.
- 생산 Scene의 Player·Camera·Pattern·Boundary·Collectible 연동은 Play Mode Test로 검증한다.
- Rebase 순간의 화면 끊김·떨림·지형 겹침만 결정적 화면 경로로 확인한다.

### 상태

대기

---

## Phase 4

### 목표

새 Score 규칙과 World Rebase를 포함한 전체 게임 회귀를 검증한다.

### 구현 대상

- Stage Mode 이동·Momentum Landing·Clear Time 회귀
- InfiniteMode 장시간 진행과 반복 Rebase
- Pattern·Collectible·Difficulty·Score·UI 회귀
- Pause·Resume·Result·Retry 전체 흐름
- 대상 플랫폼 Build와 장시간 성능 확인

### 완료 조건

- 전체 Edit Mode와 Play Mode Test가 통과한다.
- Stage Mode의 이동과 Clear Time에 의도하지 않은 변화가 없다.
- InfiniteMode에서 여러 번 Rebase한 뒤에도 정상 진행할 수 있다.
- 장시간 Run에서 좌표 정밀도에 따른 Camera·Physics 이상이 확인되지 않는다.
- 대상 플랫폼 Build와 핵심 플레이 경로가 통과한다.

### 검증 책임

- 코드·문서·Scene·Prefab의 계약과 변경 범위를 정적으로 검사한다.
- 전체 자동 Test를 한 번 실행해 기존 기능 회귀를 검증한다.
- 대상 플랫폼 Player에서 Rebase와 성능을 확인한다.

### 상태

대기

---

# 현재 개발 진행 상태

## 진행 중인 작업

없음

---

## 다음 작업

Phase 2 Momentum Landing Score 전환과 UI 생산 연결

---

## 보류된 작업

- 확정된 초기 배율·유지 시간·Rebase 임계값·Score 값의 후속 밸런스 조정은 기능 검증 후 수행하며, Score 결과가 달라지면 Scoring Version을 증가시킨다.
- 추가 Pattern과 Collectible 보상 확장은 좌표·Score 기반 안정화 후 수행한다.
- 기록 저장과 Leaderboard는 Scoring Version이 확정되는 Roadmap 7에서 수행한다.

---

## 완료된 단계

- Prototype 4: Pattern 확장, Difficulty, Collectible 안내와 Score·UI 통합
- Prototype 5 Phase 1: Momentum Landing Score·Scoring Version·World Rebase 계약과 순수 모델 확정, 전체 Edit Mode Test `559`개 통과

---

# 구현 우선순위

1. Momentum Landing 배율과 World Rebase 계약
2. Momentum Landing Score 전환과 UI
3. 누적 논리 거리와 World Rebase
4. 전체 회귀, 장시간 플레이와 Build 검증

---

# 완료 기준

- Momentum Landing이 이동 속도가 아닌 Score 배율을 제공한다.
- 기존 자동 이동과 Move 입력 비활성 계약이 유지된다.
- Score 구성 요소와 Scoring Version이 명확하다.
- InfiniteMode의 큰 World X가 주기적으로 안전하게 Rebase된다.
- Rebase 이후에도 거리·Difficulty·Score·Pattern·Collectible 상태가 유지된다.
- Stage Mode와 Prototype 4 기능에 회귀가 없다.
- Compile, 전체 Test, 장시간 화면 확인과 대상 플랫폼 Build가 통과한다.

---

# 관련 문서

- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/90_Tasks/Prototype_5/20260916_04_Phase2ManualSteps.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_004.md`

---

# 작성 완료 기준

- 5차 프로토타입의 현재 구현 계획과 순서를 작성했다.
- 구현 방법과 작업 기록을 포함하지 않았다.
- 확정할 규칙과 구현·검증 단계를 분리했다.
