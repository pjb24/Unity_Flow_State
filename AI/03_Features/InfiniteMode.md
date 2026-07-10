# 기능 개요

## 기능명

InfiniteMode

---

## 목적

플레이어가 종료 조건에 도달할 때까지 Stage를 계속 진행할 수 있도록 한다.

속도를 유지하는 플레이를 통해 높은 점수를 획득하는 플레이 경험을 제공한다.

---

# 기능 규칙

- InfiniteMode는 Goal을 사용하지 않는다.
- InfiniteMode는 Stage Play가 시작되면 수행한다.
- 플레이어는 Stage를 계속 진행할 수 있다.
- 플레이어의 점수는 프로젝트에서 정의한 점수 규칙에 따라 증가한다.
- 플레이어는 진행 지속 조건을 유지하는 동안 InfiniteMode를 계속 수행한다.
- 플레이어가 진행 지속 조건을 잃으면 InfiniteMode를 종료한다.

---

# 진행 지속 조건

플레이어는 아래 조건을 모두 만족하는 동안 진행을 계속할 수 있다.

- 플레이어가 프로젝트 설정 값으로 정의한 최소 이동 속도 이상을 유지한다.
- 플레이어가 Stage 밖으로 이탈하지 않았다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 InfiniteMode를 시작한다.

- InfiniteMode Stage가 선택되었다.
- Stage Play가 시작되었다.

---

# 종료 조건

## 정상 종료

아래 조건 중 하나를 만족하면 InfiniteMode를 종료한다.

- 플레이어가 프로젝트 설정 값으로 정의한 최소 이동 속도 미만이 되었다.
- 플레이어가 Stage 밖으로 이탈하였다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- InfiniteMode Stage Play가 종료된다.
- 최종 점수가 확정된다.

---

# 예외 사항

- 일반 Stage에서는 InfiniteMode를 수행하지 않는다.
- Stage Play가 진행 중이 아니면 수행하지 않는다.
- 게임이 종료된 이후에는 수행하지 않는다.

---

# 관련 System

- StageSystem
- PlayerMovementSystem
- ResultSystem

---

# 제약 사항

- InfiniteMode는 Goal을 사용하지 않는다.
- InfiniteMode는 하나의 Stage Play 동안만 수행한다.
- InfiniteMode는 진행 지속 조건을 만족하는 동안 계속 수행한다.
- 최소 이동 속도는 프로젝트 설정 값으로 정의한다.
- 점수는 프로젝트에서 정의한 점수 규칙에 따라 증가한다.
- InfiniteMode 종료 후에는 Stage Play가 종료된다.
- 점수 기록은 ScoreRecord Feature에서 수행한다.

---

# 검증 항목

- InfiniteMode Stage 선택 시 InfiniteMode가 시작되는지 확인한다.
- InfiniteMode 동안 Goal이 사용되지 않는지 확인한다.
- 프로젝트에서 정의한 점수 규칙에 따라 점수가 증가하는지 확인한다.
- 프로젝트 설정 값으로 정의한 최소 이동 속도 이상을 유지하는 동안 Stage Play가 계속 진행되는지 확인한다.
- 프로젝트 설정 값으로 정의한 최소 이동 속도 미만이 되면 InfiniteMode가 종료되는지 확인한다.
- 플레이어가 Stage 밖으로 이탈하면 InfiniteMode가 종료되는지 확인한다.
- 최소 이동 속도 설정 값을 변경하면 종료 기준이 함께 변경되는지 확인한다.
- InfiniteMode 종료 후 최종 점수가 확정되는지 확인한다.
- 일반 Stage에서는 InfiniteMode가 수행되지 않는지 확인한다.

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