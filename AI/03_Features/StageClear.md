# 기능 개요

## 기능명

StageClear

---

## 목적

플레이어가 일반 Stage를 완료할 수 있는 기준을 제공한다.

일반 Stage의 완료를 클리어로 확정한다.

---

# 기능 규칙

- StageClear는 일반 Stage에서만 수행한다.
- 플레이어가 Stage의 Goal에 도달하면 StageClear를 수행한다.
- StageClear가 수행되면 현재 Stage Play를 클리어로 확정한다.
- 하나의 Stage Play에서는 하나의 StageClear만 수행할 수 있다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 StageClear를 시작한다.

- 일반 Stage Play가 진행 중이다.
- 플레이어가 Stage의 Goal에 도달하였다.

---

# 종료 조건

## 정상 종료

- StageClear 처리가 완료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- 현재 Stage Play가 클리어로 확정된다.

---

# 예외 사항

- 무한 모드에서는 StageClear를 수행하지 않는다.
- Stage Play가 종료된 이후에는 StageClear를 수행하지 않는다.
- 하나의 Stage Play에서 두 번 이상 수행하지 않는다.
- 게임 진행이 중단된 상태에서는 StageClear를 수행하지 않는다.

---

# 관련 System

- StageSystem

---

# 제약 사항

- StageClear는 일반 Stage에서만 수행한다.
- 하나의 Stage Play에 대해 한 번만 수행한다.
- Stage의 Goal에 도달한 경우에만 수행한다.
- StageClear는 Stage Play를 클리어 상태로만 변경한다.
- Stage Play 종료와 결과 처리는 다른 Feature에서 수행한다.

---

# 검증 항목

- 일반 Stage에서 Goal에 도달하면 StageClear가 수행되는지 확인한다.
- StageClear 수행 후 Stage Play가 클리어 상태로 확정되는지 확인한다.
- 무한 모드에서는 StageClear가 수행되지 않는지 확인한다.
- 하나의 Stage Play에서 두 번 이상 수행되지 않는지 확인한다.
- StageClear 수행만으로 Stage Play 종료나 결과 화면 전환이 발생하지 않는지 확인한다.

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