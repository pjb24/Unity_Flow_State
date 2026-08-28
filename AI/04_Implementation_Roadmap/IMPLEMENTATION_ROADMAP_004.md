# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

4차 프로토타입

InfiniteMode에 여러 Map Pattern을 추가하여 Run마다 변화하는 플레이 흐름을 제공한다.

진행도에 따라 Map Pattern의 선택과 조합이 변화하는 난이도 증가 구조를 구축한다.

자동 이동과 Collectible 안내 경로를 유지하면서 모든 Pattern이 정상적으로 연결되고 통과 가능한지 검증한다.

---

# 개발 단계

## Phase 1

### 목표

Map Pattern 확장과 난이도 증가 기준을 확정한다.

### 구현 대상

- Map Pattern 공통 조건
- Pattern 시작점과 종료점 연결 기준
- Pattern별 통과 가능 조건
- 추가할 Map Pattern 목록
- Difficulty 단계 구분
- 진행도에 따른 Pattern 선택 기준
- Pattern 반복과 제외 기준

### 완료 조건

- 추가할 Map Pattern 목록과 각 Pattern의 목적이 확정되어 있다.
- 모든 Pattern에 공통으로 적용할 연결 조건을 확인할 수 있다.
- 자동 이동 상태에서 Pattern의 통과 가능 여부를 판정할 기준이 있다.
- Difficulty 단계와 단계 전환 조건이 확정되어 있다.
- 진행도에 따라 사용할 Pattern의 선택 기준이 확정되어 있다.
- Pattern 개수, 등장 조건과 난이도 수치를 추측하지 않고 문서로 확인할 수 있다.

### 상태

대기

---

## Phase 2

### 목표

확정된 Map Pattern을 구현하고 안전하게 연결한다.

### 구현 대상

- 기존 InfiniteMode Map Pattern의 공통 구조 반영
- 확정된 추가 Map Pattern
- Pattern 연결
- Pattern 생성과 정리
- 연속 Pattern 진행
- Pattern별 자동 이동 검증
- Pattern별 벽 충돌과 낙하 검증

### 완료 조건

- 확정된 모든 Map Pattern이 InfiniteMode에서 생성된다.
- 이전 Pattern의 종료점과 다음 Pattern의 시작점이 정상적으로 연결된다.
- Pattern 연결 지점에서 Player, Camera와 Collision 흐름이 끊기지 않는다.
- 사용이 끝난 Pattern이 InfiniteMode 진행을 방해하지 않는다.
- 각 Pattern을 자동 이동, Jump와 Momentum Landing으로 통과할 수 있다.
- 벽이나 모서리에서 Player가 지속적으로 고정되지 않는다.
- 통과할 수 없는 Pattern 또는 연결 조합이 선택되지 않는다.

### 상태

대기

---

## Phase 3

### 목표

진행도에 따라 Map Pattern의 선택과 조합이 변화하도록 한다.

### 구현 대상

- InfiniteMode 진행도
- Difficulty 단계 전환
- Difficulty별 Pattern 선택
- Pattern 반복 제한
- 이전 Pattern과 다음 Pattern의 연결 가능 여부
- Run별 Pattern 선택 상태 초기화

### 완료 조건

- InfiniteMode 진행도에 따라 Difficulty 단계가 전환된다.
- 현재 Difficulty에서 허용된 Pattern만 선택된다.
- 같은 Pattern의 반복이 확정된 기준을 따른다.
- 연결할 수 없는 Pattern 조합이 생성되지 않는다.
- Retry와 새로운 Run에서 진행도, Difficulty와 Pattern 선택 상태가 초기화된다.
- Pause와 Result 상태에서는 Pattern 진행 상태가 변경되지 않는다.
- Pattern 통과 개수는 Score 계산에 사용되지 않는다.
- Player 이동 수치를 변경하지 않고 Pattern 선택과 조합으로 난이도가 변화한다.

### 상태

대기

---

## Phase 4

### 목표

추가된 Map Pattern에 Collectible 안내 경로를 적용하고 전체 흐름을 검증한다.

### 구현 대상

- Pattern별 Collectible 배치
- Pattern 연결 구간의 Collectible 경로
- 점프 시작 시점, 공중 이동 경로와 착지 지점 안내
- Distance Score와 Collectible Score 유지
- Difficulty 및 Pattern 상태 UI
- InfiniteMode 전체 회귀 검증

### 완료 조건

- 모든 Map Pattern에 통과 가능한 Collectible 안내 경로가 존재한다.
- Pattern 연결 구간에서도 Collectible 안내가 끊기거나 잘못된 경로를 제시하지 않는다.
- Collectible을 놓쳐도 InfiniteMode 진행을 계속할 수 있다.
- Distance Score, Collectible Score와 Total Score가 Pattern 전환 후에도 정상적으로 누적된다.
- 현재 Difficulty 또는 진행 상태를 확정된 UI로 확인할 수 있다.
- Pause, Resume, Result와 Retry가 Pattern 진행 상태를 올바르게 유지하거나 초기화한다.
- Stage Mode의 기존 자동 이동, Collectible, Clear Time과 Score에 회귀가 없다.
- 치명적인 오류 없이 Compile, 관련 Test, Build와 반복 플레이 검증을 통과한다.

### 상태

대기

---

# 현재 개발 진행 상태

## 진행 중인 작업

없음

---

## 다음 작업

Prototype 4 Phase 1

Map Pattern 확장과 난이도 증가 기준 확정

---

## 보류된 작업

### 밸런스 검증

- Difficulty 단계 전환 시점 조정
- Pattern별 등장 빈도 조정
- Pattern별 난이도 수치 조정
- Player 자동 이동, Jump와 Momentum Landing 수치 조정
- Collectible 위치와 획득 판정 범위 조정
- Distance Score와 Collectible Score 환산 값 조정

보류 이유

- Prototype 4에서는 Pattern 확장과 난이도 증가 구조의 정상 동작을 우선 검증한다.
- 수치 비교와 최종 밸런스 선택은 더 나중의 별도 작업으로 수행한다.

---

### Map Pattern 확장

- Prototype 4에서 확정하지 않은 추가 Pattern
- 특수 이동 규칙이 필요한 Pattern
- 새로운 장애물이나 이동 Platform을 사용하는 Pattern

보류 이유

- 확정된 기본 Pattern과 Difficulty 구조가 검증된 후 확장한다.

---

### Collectible 확장

- Combo
- Score 배율
- 희귀 Collectible
- 우회 고득점 경로

보류 이유

- 기본 Collectible 안내 경로와 Pattern 조합이 안정된 후 확장한다.

---

### Leaderboard

보류 이유

- InfiniteMode의 Pattern 진행과 Score 규칙이 안정된 후 구현한다.

---

### SaveSystem

보류 이유

- Prototype 4는 Runtime Data만 사용한다.

---

## 완료된 단계

없음

---

# 구현 우선순위

1.

Map Pattern 공통 조건

추가 Pattern 목록

Difficulty 및 Pattern 선택 기준

---

2.

확정된 Map Pattern 구현

Pattern 연결, 생성과 정리

Pattern별 통과 가능 검증

---

3.

진행도와 Difficulty 단계 전환

Difficulty별 Pattern 선택

Pattern 조합 검증

---

4.

Pattern별 Collectible 안내 경로

Score 및 UI 통합

전체 회귀 검증

---

# 완료 기준

다음 조건을 모두 만족하면 4차 프로토타입을 완료한 것으로 판단한다.

- 확정된 여러 Map Pattern이 InfiniteMode에서 생성된다.
- 모든 Map Pattern과 연결 조합을 통과할 수 있다.
- 진행도에 따라 Difficulty 단계와 Pattern 선택 범위가 변화한다.
- Pattern 선택과 조합으로 난이도가 증가한다.
- Retry와 새로운 Run에서 Pattern, 진행도와 Difficulty 상태가 초기화된다.
- 모든 Pattern에서 Collectible이 점프 시점과 이동 경로를 안내한다.
- Distance Score, Collectible Score와 Total Score가 Pattern 전환 후에도 유지된다.
- Pause, Result와 Retry가 Pattern 진행 상태와 충돌하지 않는다.
- Pattern 통과 개수는 Score로 사용하지 않는다.
- Stage Mode와 Prototype 3의 핵심 기능에 회귀가 없다.
- 치명적인 오류 없이 Compile, Test, Build와 반복 플레이 검증을 통과한다.

---

# 관련 문서

## Project

- PROJECT_OVERVIEW.md
- ARCHITECTURE.md
- PROJECT_MEMORY.md

---

## Rules

- AI_RULE.md
- IMPLEMENTATION_RULE.md
- VERIFICATION_RULE.md

---

## Systems

- GameSystem
- RuntimeDataSystem
- UIManagementSystem
- UIInputSystem
- PlayerMovementSystem
- CollisionSystem
- CameraSystem
- StageSystem
- ResultSystem

---

## Features

- InfiniteMode
- ScoreRecord
- GamePause
- Jump
- MomentumLanding
- StagePlay
- ResultMenu

---

# 작성 완료 기준

- Prototype 4의 현재 구현 계획만 작성했다.
- 각 Phase의 목표, 구현 대상, 완료 조건과 상태를 작성했다.
- 구현 방법과 작업 기록을 작성하지 않았다.
- 확정되지 않은 Pattern 목록, 개수와 난이도 수치를 Phase 1 선행 확정 대상으로 구분했다.
- 밸런스 검증과 추가 확장 기능을 보류된 작업으로 구분했다.
