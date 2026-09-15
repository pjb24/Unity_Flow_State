# 기능 개요

## 기능명

ScoreRecord

---

## 목적

InfiniteMode Stage Play의 최종 이동 거리와 Mode별 Score를 기록으로 확정한다.

반복 플레이를 통한 점수 경쟁의 기준을 제공한다.

---

# 기능 규칙

- ScoreRecord는 InfiniteMode에서만 수행한다.
- ScoreRecord는 InfiniteMode Stage Play가 종료된 경우에만 수행한다.
- ScoreRecord는 Stage Play마다 한 번만 수행한다.
- ScoreRecord는 InfiniteMode Stage Play의 최종 이동 거리, Base Distance Score, Momentum Bonus, Distance Score, Collectible Score, Total Score와 최고 Momentum 배율을 하나의 결과로 확정한다.
- ScoreRecord가 하나의 Run에서 기록 완료 상태를 소유하고 중복 기록 요청을 거부한다.
- InfiniteMode Stage Play가 종료되지 않은 경우 ScoreRecord를 수행하지 않는다.
- ScoreRecord는 일반 Stage의 클리어 시간을 기록하지 않는다.
- 현재 Momentum Score 기록의 Scoring Version은 `2`이다.
- ScoreRecord는 Run에서 고정한 Scoring Version을 Result Data에 변경 없이 기록한다.
- Runtime Data와 Result 기록 요청의 Scoring Version이 다르면 기록하지 않는다.
- Version이 `0` 이하이거나 지원하지 않는 Version이면 기록하지 않는다.
- Scoring Version이 없는 기록은 기존 Version으로 추정하지 않고 무효로 처리한다.
- Distance Score는 `Base Distance Score + Momentum Bonus`로 계산하고 `int.MaxValue`에서 포화한다.
- Total Score는 `Distance Score + Collectible Score`로 계산하고 `int.MaxValue`에서 포화한다.
- Pattern 전환과 재사용은 현재 Run의 Distance Score 및 Collectible Score를 초기화하지 않는다.
- 음수 Base Distance Score, Momentum Bonus, Distance Score와 Collectible Score는 기록하지 않는다.
- 현재 Score 구성 요소와 전달된 합계가 일치하지 않으면 기록하지 않는다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 ScoreRecord를 시작한다.

- InfiniteMode Stage Play가 종료되었다.
- 최종 이동 거리가 확정되었다.
- Base Distance Score, Momentum Bonus, Distance Score와 Collectible Score가 확정되었다.
- 최고 Momentum 배율이 확정되었다.
- 유효한 Scoring Version이 확정되었다.

---

# 종료 조건

## 정상 종료

- ScoreRecord 처리가 완료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- InfiniteMode Stage Play의 Mode, Scoring Version, 최종 이동 거리, Base Distance Score, Momentum Bonus, Distance Score, Collectible Score, Total Score와 최고 Momentum 배율이 Result Data로 확정된다.

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
- Base Distance Score, Momentum Bonus, Distance Score와 Collectible Score가 확정된 이후에만 수행한다.
- 최종 이동 거리와 Mode별 Score가 모두 확정된 이후에만 수행한다.
- InfiniteMode Result Data에는 Mode, 최종 이동 거리, Base Distance Score, Momentum Bonus, Distance Score, Collectible Score, Total Score와 최고 Momentum 배율을 포함한다.
- InfiniteMode Result Data에는 유효한 Scoring Version을 포함한다.
- Stage Result와 InfiniteMode Result는 하나의 Result Data 구조에서 Mode에 따라 유효한 결과를 구분한다.
- Stage 결과 필드와 InfiniteMode 결과 필드는 하나의 Result Data에서 동시에 유효할 수 없다.
- 일반 Stage의 클리어 시간은 TimeRecord Feature에서 처리한다.
- 기록 저장과 Leaderboard 반영은 다른 Feature 또는 System에서 수행한다.
- 저장과 Leaderboard를 구현할 때 Game Mode와 Scoring Version을 함께 기록 구분 Key로 사용한다.
- Scoring Version이 다른 기록끼리는 순위, 최고 점수와 동점 판정을 수행하지 않는다.
- Version이 없는 기록은 저장 또는 Leaderboard 입력에서 무효로 처리한다.
- 결과 생성과 결과 화면 표시는 다른 Feature 또는 System에서 수행한다.

---

# 검증 항목

- InfiniteMode Stage 선택 후 Stage Play 종료 시 ScoreRecord가 수행되는지 확인한다.
- InfiniteMode Stage Play 종료 후 Base Distance Score, Momentum Bonus, Distance Score와 Collectible Score가 기록으로 정상 확정되는지 확인한다.
- Distance Score가 Base Distance Score와 Momentum Bonus의 포화 합인지 확인한다.
- Total Score가 두 Score의 합으로 계산되고 `int.MaxValue`에서 포화하는지 확인한다.
- 최고 Momentum 배율이 Result Data에 유지되는지 확인한다.
- Run의 Scoring Version이 Result Data에 변경 없이 전달되는지 확인한다.
- Version 없음, `0`, 지원하지 않는 Version과 Runtime Version 불일치 요청을 거부하는지 확인한다.
- Collectible 획득 후 Pattern 전환과 Run 종료를 거쳐 Result Data와 UI에 두 Score 및 Total Score가 유지되는지 확인한다.
- InfiniteMode Stage Play 종료 후 Mode, 최종 이동 거리와 Mode별 Score가 Result Data로 확정되는지 확인한다.
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
