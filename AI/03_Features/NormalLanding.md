# 기능 개요

## 기능명

NormalLanding

---

## 목적

플레이어가 점프를 마친 후 기본적인 착지 동작을 수행하도록 한다.

Momentum Landing이 수행되지 않은 경우 지면 이동으로 자연스럽게 전환한다.

---

# 기능 규칙

- Normal Landing은 점프 이후 착지 시 판정된다.
- Momentum Landing이 수행되지 않은 경우 Normal Landing을 수행한다.
- Momentum Landing Window 동안 관성 착지 입력이 수행되지 않으면 Normal Landing을 수행한다.
- Momentum Landing Window 종료 후 착지한 경우 Normal Landing을 수행한다.
- Normal Landing이 수행되면 플레이어는 지면 이동 상태가 된다.
- 하나의 점프에서는 하나의 Normal Landing만 수행할 수 있다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 Normal Landing을 시작한다.

- 플레이어가 공중 상태이다.
- 플레이어가 지면에 착지하였다.
- Momentum Landing이 수행되지 않았다.

---

# 종료 조건

## 정상 종료

- Normal Landing 처리가 완료된다.
- 플레이어가 지면 이동 상태가 된다.

## 강제 종료

- Stage가 종료된다.
- 게임이 종료된다.

---

# 수행 결과

- 플레이어가 지면 이동 상태가 된다.
- 이후 일반 지면 이동을 계속 수행할 수 있다.

---

# 예외 사항

- Momentum Landing이 수행된 경우 Normal Landing을 수행하지 않는다.
- 공중 상태가 아니면 수행하지 않는다.
- 하나의 점프에서 두 번 이상 수행하지 않는다.
- 게임 진행이 중단된 상태에서는 수행하지 않는다.

---

# 관련 System

- PlayerMovementSystem
- CollisionSystem

---

# 제약 사항

- 하나의 착지에 대해 한 번만 판정한다.
- Normal Landing과 Momentum Landing은 동시에 수행할 수 없다.
- 모든 점프는 Momentum Landing 또는 Normal Landing 중 하나로 종료되어야 한다.

---

# 검증 항목

- Momentum Landing이 수행되지 않으면 Normal Landing이 수행되는지 확인한다.
- Momentum Landing Window 동안 입력하지 않으면 Normal Landing이 수행되는지 확인한다.
- Momentum Landing Window 종료 후 착지하면 Normal Landing이 수행되는지 확인한다.
- Momentum Landing이 수행된 경우 Normal Landing이 수행되지 않는지 확인한다.
- 하나의 착지에서 두 번 이상 수행되지 않는지 확인한다.
- Normal Landing 후 정상적으로 지면 이동이 가능한지 확인한다.
- Stage 종료 시 Normal Landing이 종료되는지 확인한다.

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