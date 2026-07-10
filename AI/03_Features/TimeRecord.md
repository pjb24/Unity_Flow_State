# 기능 개요

## 기능명

TimeRecord

---

## 목적

일반 Stage Play의 클리어 시간을 기록으로 확정한다.

반복 플레이를 통한 클리어 시간 단축의 기준을 제공한다.

---

# 기능 규칙

- TimeRecord는 일반 Stage에서만 수행한다.
- TimeRecord는 Stage Play가 클리어된 경우에만 수행한다.
- TimeRecord는 Stage Play마다 한 번만 수행한다.
- TimeRecord는 Stage Play의 클리어 시간을 기록으로 확정한다.
- Stage Play가 클리어되지 않은 경우 TimeRecord를 수행하지 않는다.
- TimeRecord는 무한 모드의 점수를 기록하지 않는다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 TimeRecord를 시작한다.

- 일반 Stage Play가 클리어되었다.
- 클리어 시간이 확정되었다.

---

# 종료 조건

## 정상 종료

- TimeRecord 처리가 완료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- Stage Play의 클리어 시간이 기록으로 확정된다.

---

# 예외 사항

- 무한 모드에서는 TimeRecord를 수행하지 않는다.
- Stage Play가 클리어되지 않은 경우 수행하지 않는다.
- 하나의 Stage Play에서 두 번 이상 수행하지 않는다.
- 게임 진행이 중단된 상태에서는 수행하지 않는다.

---

# 관련 System

- TimerSystem
- ResultSystem

---

# 제약 사항

- TimeRecord는 일반 Stage에서만 수행한다.
- 하나의 Stage Play에 대해 한 번만 수행한다.
- 클리어 시간이 확정된 이후에만 수행한다.
- TimeRecord는 클리어 시간만 기록으로 확정한다.
- 무한 모드의 점수는 TimeRecord에서 처리하지 않는다.
- 기록 저장과 Leaderboard 반영은 다른 Feature 또는 System에서 수행한다.
- 결과 생성과 결과 화면 표시는 다른 Feature 또는 System에서 수행한다.

---

# 검증 항목

- 일반 Stage를 클리어하면 TimeRecord가 수행되는지 확인한다.
- 클리어 시간이 기록으로 정상 확정되는지 확인한다.
- 하나의 Stage Play에서 두 번 이상 수행되지 않는지 확인한다.
- Stage Play를 클리어하지 않으면 TimeRecord가 수행되지 않는지 확인한다.
- 무한 모드에서는 TimeRecord가 수행되지 않는지 확인한다.
- 무한 모드 점수가 TimeRecord에 기록되지 않는지 확인한다.

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