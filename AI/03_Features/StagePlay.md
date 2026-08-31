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
- Stage Play 진행 중 플레이어는 이동과 점프를 수행할 수 있다.
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
