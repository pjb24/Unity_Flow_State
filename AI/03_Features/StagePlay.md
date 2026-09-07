# 기능 개요

## 기능명

StagePlay

---

## 목적

플레이어가 하나의 Stage를 선택하여 Stage Play를 시작하고 종료할 때까지의 플레이 흐름을 제공한다.

Stage Play를 하나의 플레이 경험으로 구성한다.

---

# 기능 규칙

- Stage Play는 시작 조건을 만족하면 시작된다.
- Stage Mode와 InfiniteMode를 서로 다른 게임 Mode로 구분한다.
- 기본 게임 Mode는 Stage Mode이다.
- Stage Play가 시작되면 플레이어는 선택한 Stage를 진행할 수 있다.
- Stage Mode와 InfiniteMode는 Playing 시작 시 수평 속도 0에서 World +X 방향으로 자동 가속한다.
- 두 Mode의 기본 목표 속도는 8, 최대 수평 속도는 14이며 Ground 가속도 50과 Air 가속도 25를 사용한다.
- 기본 속도 미만이면 자동 회복하고 관성 착지로 얻은 기본 속도 초과의 우측 속도는 최대 속도 안에서 보존한다.
- 플레이어는 자동 이동 중 Jump와 Momentum Landing을 입력할 수 있다. Player 좌우 입력은 수평 이동 계산에 영향을 주지 않는다.
- Jump와 Normal Landing은 수평 자동 이동을 별도로 초기화하지 않는다.
- 공중 Wall 접촉은 Wall 안쪽 수평 속도만 제한한다. 접촉 해제가 확인된 첫 물리 단계부터 자동 가속을 재개한다.
- Ground와 Wall이 함께 검출되면 Ground 이동 계산을 우선하며 실제 장애물 충돌은 유지한다.
- Playing 외 상태에서는 자동 이동 계산을 수행하지 않는다.
- 점프는 Momentum Landing 또는 Normal Landing으로 연결된다.
- 일반 Stage Play는 플레이어가 Stage의 Goal에 도달하면 종료된다.
- InfiniteMode Stage Play는 Goal을 사용하지 않는다.
- InfiniteMode Stage Play는 플레이어가 진행 지속 조건을 잃으면 종료된다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 Stage Play를 시작한다.

- Stage Mode 또는 InfiniteMode가 선택되었다.
- Stage Play 시작 요청이 수행되었다.

---

# 종료 조건

## 정상 종료

- 일반 Stage Play에서 플레이어가 Stage의 Goal에 도달하였다.
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
- Stage Play 종료 이후에는 다시 진행 상태로 돌아갈 수 없다.
- Retry를 실행하면 종료된 Stage Play와 같은 게임 Mode로 새로운 Stage Play를 시작한다.
- Retry로 시작한 Stage Play는 이전 Stage Play의 Runtime 상태를 유지하지 않는다.
- 진행 지속 조건은 InfiniteMode Feature에서 정의한다.
- 결과 생성과 결과 화면 표시는 다른 Feature 또는 System에서 수행한다.

---

# 검증 항목

- Stage Mode 또는 InfiniteMode 선택 후 해당 Mode의 Stage Play가 정상적으로 시작되는지 확인한다.
- Stage Play 진행 중 플레이어가 정상적으로 이동할 수 있는지 확인한다.
- 두 Mode에서 Move 입력 없이 우측 가속이 시작되고 Jump 및 Landing 후에도 이어지는지 확인한다.
- 중복 시작/초기화 요청이 같은 Run의 이동 및 Jump 상태를 변경하지 않는지 확인한다.
- 종료 후 여러 물리 단계가 지나도 자동 이동이 재개되지 않는지 확인한다.
- 점프가 Momentum Landing 또는 Normal Landing으로 정상 연결되는지 확인한다.
- 일반 Stage Play에서 Stage의 Goal에 도달하면 종료되는지 확인한다.
- InfiniteMode Stage Play에서 진행 지속 조건을 잃으면 종료되는지 확인한다.
- Stage Play 종료 이후 Retry를 실행하면 같은 게임 Mode로 새로운 Stage Play를 시작하는지 확인한다.
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
