# 기능 개요

## 기능명

StagePlay

---

## 목적

Infinite Run을 시작하고 종료할 때까지의 공통 플레이 흐름을 제공한다.

문서명 StagePlay와 본문의 Stage Play는 공통 Run 생명주기를 뜻한다. 사용자에게 선택 가능한 Stage Mode를 제공한다는 의미가 아니며 현재 생산 플레이는 Infinite다.

---

## Roadmap 9 적용 상태

Phase 2의 Infinite 시작/공통 실행·Stage 실행/표시 차단과 사용자 Compile/Scene/Test/Player 확인은 완료했다. 아래 생산 규칙은 Infinite 기준이며 Stage 전용 Goal/추락 소스는 Deprecated 독립 검증 대상으로 보존한다. 남은 기록 직접 접근은 Phase 3, 전체 후보 판정은 Phase 4 범위다. 실행 근거는 [Phase 2 Task](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md)를 따른다.

- 새 Client는 InfiniteMode만 시작/재시작하며 Stage Mode의 명시적 생산 실행 요청은 거절한다.
- 과거 기본/선택/대기 Mode는 Infinite 시작 흐름으로 정규화한다. 기존 자료의 enum 값 변경이나 Stage 소스 삭제를 뜻하지 않는다.
- Infinite의 이동·착지·점수·Pattern·공통 생명주기는 유지한다. 시작/안내는 GameEntryNavigation, Pause Retry 확인은 GamePause, Result Retry는 ResultMenu 규칙을 따른다.

검증 계약: P9-C02/P9-C09. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

# 기능 규칙

- Stage Play는 시작 조건을 만족하면 시작된다.
- 내부 enum/자료형에서는 Stage Mode와 InfiniteMode 값을 구분해 보존하지만 사용자 Mode 선택은 제공하지 않는다.
- 기본/생산 게임 Mode는 Infinite다. 명시적 Stage 생성·초기화/시작은 기존 상태와 공통 Run을 변경하지 않고 거절한다.
- 유효한 Infinite Play 요청이 시작되면 플레이어는 Infinite 지형을 진행한다.
- InfiniteMode는 Playing 중 PlayerMovementSystem의 직렬화된 이동 속도를 World +X 방향의 고정 수평 속도로 사용한다.
- 수평 속도 증가, 지상/공중 가속, 최대 수평 속도 설정은 사용하지 않는다. Momentum Landing도 수평 속도를 변경하지 않는다.
- 플레이어는 자동 이동 중 Jump와 Momentum Landing을 입력할 수 있다. Player 좌우 입력은 수평 이동 계산에 영향을 주지 않는다.
- Jump와 Normal Landing은 수평 자동 이동을 별도로 초기화하지 않는다.
- 공중 Wall 접촉은 Wall 안쪽 수평 속도만 제한한다. 접촉 해제가 확인된 첫 물리 단계부터 고정 수평 속도를 다시 적용한다.
- Ground와 Wall이 함께 검출되면 Ground 이동 계산을 우선하며 실제 장애물 충돌은 유지한다.
- Playing 외 상태에서는 자동 이동 계산을 수행하지 않는다.
- 점프는 Momentum Landing 또는 Normal Landing으로 연결된다.
- 보존된 Stage Goal/추락 판정은 생산 플레이에서 사용하지 않는다. 이전 Goal 우선·추락 경계는 독립 합성 Test로만 보존한다.
- InfiniteMode Stage Play는 Goal을 사용하지 않는다.
- InfiniteMode Stage Play는 플레이어가 진행 지속 조건을 잃으면 종료된다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 Stage Play를 시작한다.

- InfiniteMode의 유효한 시작 요청이 확정되었다.
- Stage Play 시작 요청이 수행되었다.

---

# 종료 조건

## 정상 종료

- InfiniteMode Stage Play에서 플레이어가 진행 지속 조건을 잃었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- Stage Play가 종료된다.

---

# 예외 사항

- Stage Play가 시작되지 않은 상태에서는 Stage를 진행하지 않는다.
- Stage Play가 종료된 이후에는 Stage를 계속 진행하지 않는다.
- 게임 진행이 중단된 상태에서는 Stage Play를 진행하지 않는다.
- Pause 중에는 추락 판정을 수행하지 않고 Resume 후 다시 평가한다.

---

# 관련 System

- GameSystem
- StageSystem
- PlayerMovementSystem
- TimerSystem
- ResultSystem
- UIManagementSystem

---

# 제약 사항

- Stage Play가 진행 중인 동안에는 동일한 Stage를 다시 시작할 수 없다.
- 하나의 Stage Play는 하나의 종료 결과만 가진다.
- 생산 Infinite 추락 종료 규칙은 InfiniteMode Feature에서 관리한다.
- Stage Play 종료 이후에는 다시 진행 상태로 돌아갈 수 없다.
- Retry를 실행하면 Infinite Mode로 새로운 Stage Play를 시작한다.
- Retry로 시작한 Stage Play는 이전 Stage Play의 Runtime 상태를 유지하지 않는다.
- 진행 지속 조건은 InfiniteMode Feature에서 정의한다.
- 결과 생성과 결과 화면 표시는 다른 Feature 또는 System에서 수행한다.

---

# 검증 항목

- Infinite 시작과 Stage/미정의 Mode 거절·기존 상태/수집 scope 불변을 확인한다.
- Stage Play 진행 중 플레이어가 정상적으로 이동할 수 있는지 확인한다.
- Infinite에서 Move 입력 없이 직렬화된 고정 수평 속도가 적용되고 Jump 및 Landing 후에도 유지되는지 확인한다.
- 중복 시작/초기화 요청이 같은 Run의 이동 및 Jump 상태를 변경하지 않는지 확인한다.
- 종료 후 여러 물리 단계가 지나도 자동 이동이 재개되지 않는지 확인한다.
- 점프가 Momentum Landing 또는 Normal Landing으로 정상 연결되는지 확인한다.
- 비권장 Stage Goal/추락 경계·같은 구간 Goal 우선의 독립 합성 검증과 생산 Stage 실행 거절 검증을 분리한다.
- Pause 중에는 추락하지 않고 Resume 후 다시 평가하는지 확인한다.
- InfiniteMode Stage Play에서 진행 지속 조건을 잃으면 종료되는지 확인한다.
- Stage Play 종료 이후 Retry를 실행하면 Infinite Mode로 새로운 Stage Play를 시작하는지 확인한다.
- Retry로 시작한 Stage Play에 이전 Stage Play의 Runtime 상태가 남지 않는지 확인한다.
- Stage Play가 진행 중인 동안 동일한 Stage를 다시 시작할 수 없는지 확인한다.
- Stage Play 종료만으로 결과 생성이나 결과 화면 표시가 수행되지 않는지 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의만 작성한다.

Feature의 규칙만 작성한다.

System의 책임을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

변경 이력을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

Feature 하나당 문서 하나를 사용한다.
