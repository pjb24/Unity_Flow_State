# 기능 개요

## 기능명

ScoreRecord

---

## 목적

InfiniteMode Stage Play의 최종 이동 거리와 최종 Score를 기록으로 확정한다.

반복 플레이를 통한 점수 경쟁의 기준을 제공한다.

---

# 기능 규칙

- ScoreRecord는 InfiniteMode에서만 수행한다.
- ScoreRecord는 InfiniteMode Stage Play가 종료된 경우에만 수행한다.
- ScoreRecord는 Stage Play마다 한 번만 수행한다.
- ScoreRecord는 InfiniteMode Stage Play의 최종 이동 거리와 최종 Score를 하나의 결과로 확정한다.
- ScoreRecord가 하나의 Run에서 기록 완료 상태를 소유하고 중복 기록 요청을 거부한다.
- InfiniteMode Stage Play가 종료되지 않은 경우 ScoreRecord를 수행하지 않는다.
- ScoreRecord는 일반 Stage의 클리어 시간을 기록하지 않는다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 ScoreRecord를 시작한다.

- InfiniteMode Stage Play가 종료되었다.
- 최종 이동 거리가 확정되었다.
- 최종 점수가 확정되었다.

---

# 종료 조건

## 정상 종료

- ScoreRecord 처리가 완료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- InfiniteMode Stage Play의 Mode, 최종 이동 거리와 최종 Score가 Result Data로 확정된다.
- Phase 2에서는 확정된 Result Data를 화면에 표시하지 않는다.

---

# 예외 사항

- 일반 Stage에서는 ScoreRecord를 수행하지 않는다.
- InfiniteMode Stage Play가 종료되지 않은 경우 수행하지 않는다.
- 하나의 Stage Play에서 두 번 이상 수행하지 않는다.
- 게임 진행이 중단된 상태에서는 수행하지 않는다.

---

# 관련 System

- ResultSystem

---

# 제약 사항

- ScoreRecord는 InfiniteMode에서만 수행한다.
- 하나의 Stage Play에 대해 한 번만 수행한다.
- 최종 점수가 확정된 이후에만 수행한다.
- 최종 이동 거리와 최종 Score가 모두 확정된 이후에만 수행한다.
- InfiniteMode Result Data에는 Mode, 최종 이동 거리와 최종 Score만 포함한다.
- Stage Result와 InfiniteMode Result는 하나의 Result Data 구조에서 Mode에 따라 유효한 결과를 구분한다.
- Stage 결과 필드와 InfiniteMode 결과 필드는 하나의 Result Data에서 동시에 유효할 수 없다.
- 일반 Stage의 클리어 시간은 TimeRecord Feature에서 처리한다.
- 기록 저장과 Leaderboard 반영은 다른 Feature 또는 System에서 수행한다.
- 결과 생성과 결과 화면 표시는 다른 Feature 또는 System에서 수행한다.

---

# 검증 항목

- InfiniteMode Stage 선택 후 Stage Play 종료 시 ScoreRecord가 수행되는지 확인한다.
- InfiniteMode Stage Play 종료 후 최종 점수가 기록으로 정상 확정되는지 확인한다.
- InfiniteMode Stage Play 종료 후 Mode, 최종 이동 거리와 최종 Score가 Result Data로 확정되는지 확인한다.
- 하나의 Stage Play에서 두 번 이상 수행되지 않는지 확인한다.
- InfiniteMode Stage Play가 종료되지 않으면 ScoreRecord가 수행되지 않는지 확인한다.
- 일반 Stage에서는 ScoreRecord가 수행되지 않는지 확인한다.
- TimeRecord와 ScoreRecord가 동시에 수행되지 않는지 확인한다.
- Stage 결과 필드와 InfiniteMode 결과 필드가 동시에 유효한 Result Data를 생성할 수 없는지 확인한다.
- Retry 후 기록 완료 상태가 초기화되어 다음 Run을 기록할 수 있는지 확인한다.

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
