# 기능 개요

## 기능명

Leaderboard

---

## 목적

플레이어가 기록 순위를 확인할 수 있도록 한다.

기록 경쟁의 기준을 제공한다.

---

# 기능 규칙

- Leaderboard는 확정된 기록이 존재하는 경우에만 조회할 수 있다.
- 일반 Stage와 InfiniteMode는 각각 독립적인 Leaderboard를 사용한다.
- 일반 Stage의 Leaderboard는 Stage별로 독립적으로 제공한다.
- InfiniteMode의 Leaderboard는 InfiniteMode 전체에 대해 하나의 순위를 제공한다.
- 일반 Stage는 클리어 시간을 기준으로 순위를 제공한다.
- InfiniteMode는 최종 점수를 기준으로 순위를 제공한다.
- Leaderboard는 기록을 순위 형태로 제공한다.
- Leaderboard는 기록을 생성하거나 수정하지 않는다.

---

# 시작 조건

다음 조건을 만족하는 경우 Leaderboard를 시작한다.

- 플레이어가 Leaderboard 조회를 요청하였다.

---

# 종료 조건

## 정상 종료

- Leaderboard 조회가 완료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- 현재 Leaderboard가 제공된다.
- 플레이어는 자신의 순위를 확인할 수 있다.

---

# 예외 사항

- 확정된 기록이 존재하지 않으면 Leaderboard를 제공하지 않는다.
- 게임이 종료된 이후에는 수행하지 않는다.

---

# 관련 System

- ResultSystem
- UIManagementSystem

---

# 제약 사항

- Leaderboard는 기록을 생성하거나 수정하지 않는다.
- 일반 Stage와 InfiniteMode의 Leaderboard는 서로 독립적으로 관리한다.
- 일반 Stage의 Leaderboard는 Stage별로 독립적으로 관리한다.
- InfiniteMode의 Leaderboard는 하나만 존재한다.
- 일반 Stage는 클리어 시간을 기준으로 순위를 제공한다.
- InfiniteMode는 최종 점수를 기준으로 순위를 제공한다.
- 순위는 확정된 기록만 사용한다.

---

# 검증 항목

- 일반 Stage에서 클리어 시간이 해당 Stage의 Leaderboard에 반영되는지 확인한다.
- 서로 다른 Stage의 기록이 같은 Leaderboard에 포함되지 않는지 확인한다.
- InfiniteMode에서 최종 점수가 InfiniteMode Leaderboard에 반영되는지 확인한다.
- 일반 Stage와 InfiniteMode의 Leaderboard가 서로 분리되어 있는지 확인한다.
- 확정된 기록이 없는 경우 Leaderboard가 제공되지 않는지 확인한다.
- Leaderboard 조회 시 현재 순위를 정상적으로 확인할 수 있는지 확인한다.
- Leaderboard가 기록을 생성하거나 수정하지 않는지 확인한다.
- 새로운 기록이 확정된 이후 Leaderboard를 다시 조회하면 최신 순위가 반영되는지 확인한다.

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