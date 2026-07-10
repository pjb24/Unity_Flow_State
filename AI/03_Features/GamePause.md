# 기능 개요

## 기능명

GamePause

---

## 목적

플레이어가 Stage Play를 일시적으로 중단할 수 있도록 한다.

일시정지 중에는 현재 Stage Play의 진행 상태를 유지한다.

---

# 기능 규칙

- GamePause는 Stage Play가 진행 중일 때만 수행한다.
- 플레이어는 언제든 GamePause를 시작할 수 있다.
- GamePause가 시작되면 Stage Play를 일시 중단한다.
- GamePause 동안에는 플레이어를 조작할 수 없다.
- GamePause 동안에는 Stage Play가 진행되지 않는다.
- GamePause 동안에는 클리어 시간이 증가하지 않는다.
- GamePause가 종료되면 일시정지 이전의 Stage Play를 이어서 진행한다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 GamePause를 시작한다.

- Stage Play가 진행 중이다.
- 플레이어가 일시정지 요청을 수행하였다.

---

# 종료 조건

## 정상 종료

- 플레이어가 게임 재개 요청을 수행하였다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- Stage Play가 일시 중단된다.
- GamePause 종료 후 기존 Stage Play를 이어서 진행한다.

---

# 예외 사항

- Stage Play가 진행 중이 아니면 수행하지 않는다.
- GamePause가 이미 수행 중이면 다시 수행하지 않는다.
- 게임이 종료된 이후에는 수행하지 않는다.

---

# 관련 System

- GameSystem
- PlayerMovementSystem
- TimerSystem
- UIManagementSystem

---

# 제약 사항

- GamePause는 Stage Play 동안만 수행한다.
- 하나의 Stage Play에서는 하나의 GamePause 상태만 유지할 수 있다.
- GamePause는 Stage Play의 진행 상태를 초기화하지 않는다.
- GamePause 종료 후에는 일시정지 이전의 Stage Play를 이어서 진행한다.

---

# 검증 항목

- Stage Play 진행 중 GamePause가 시작되는지 확인한다.
- GamePause 동안 플레이어를 조작할 수 없는지 확인한다.
- GamePause 동안 Stage Play가 진행되지 않는지 확인한다.
- GamePause 동안 클리어 시간이 증가하지 않는지 확인한다.
- GamePause 종료 후 Stage Play가 중단된 시점부터 이어서 진행되는지 확인한다.
- Stage Play가 진행 중이 아닐 때 GamePause가 수행되지 않는지 확인한다.
- GamePause가 수행 중일 때 다시 GamePause가 시작되지 않는지 확인한다.

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