# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

3차 프로토타입

Player의 기본 이동을 좌측에서 우측으로 자동화하고 사용자 조작을 Jump와 Momentum Landing에 집중한다.

Stage Mode와 InfiniteMode에 Score Collectible을 배치하여 점프 시점, 공중 이동 경로와 착지 지점을 안내한다.

벽 충돌 상태에서도 Player가 벽에 고정되지 않고 자연스럽게 바닥으로 떨어지는 이동 기반을 구축한다.

---

# 개발 단계

## Phase 1

### 목표

자동 이동 전환 전에 벽 충돌과 낙하 동작을 안정화한다.

### 구현 대상

- 벽과 Player의 충돌 동작
- 벽 접촉 중 중력 낙하
- Ground와 Wall 접촉 구분
- 벽 및 모서리 고정 방지
- 기존 이동과 충돌 회귀 검증

### 완료 조건

- 공중에서 벽에 충돌한 Player가 벽에 고정되지 않는다.
- 벽 방향 이동이 제한되어도 아래 방향 낙하는 계속된다.
- Wall 접촉이 Ground 접촉으로 처리되지 않는다.
- 벽에서 떨어져 Ground에 착지한 후 Jump와 Momentum Landing 상태가 정상적으로 복구된다.
- 별도의 Wall Jump와 Wall Slide가 추가되지 않는다.
- 기존 Ground, Platform과 Goal 충돌에 회귀가 없다.

### 상태

대기

---

## Phase 2

### 목표

Player의 수평 이동을 자동화하고 핵심 조작을 Jump와 Momentum Landing으로 제한한다.

### 구현 대상

- Stage Mode 자동 이동
- InfiniteMode 자동 이동
- Player 좌우 이동 입력 제거
- Jump 입력
- Momentum Landing 입력
- 자동 이동 상태의 Pause, Result와 Retry
- Camera 및 충돌 회귀 검증

### 완료 조건

- Stage Play가 시작되면 Player가 좌측에서 우측으로 자동 이동한다.
- Stage Mode와 InfiniteMode에 동일한 자동 이동 원칙이 적용된다.
- Player의 좌우 이동 입력이 게임 플레이에 영향을 주지 않는다.
- Jump와 Momentum Landing은 사용자 입력으로 수행한다.
- UI의 Navigate 입력은 Player 좌우 이동 입력 제거의 영향을 받지 않는다.
- Pause와 Result 상태에서는 자동 이동이 중단된다.
- Resume 후 자동 이동이 정상적으로 복구된다.
- Retry 시 자동 이동 상태가 초기화된다.
- 벽 충돌 중에도 자연스러운 낙하가 유지된다.
- Camera가 자동 이동하는 Player를 정상적으로 추적한다.

### 상태

대기

---

## Phase 3

### 목표

Score Collectible로 점프 시점과 이동 경로를 안내한다.

### 구현 대상

- Score Collectible
- Collectible 획득과 Run별 초기화
- Stage Mode Collectible 배치
- InfiniteMode Map Pattern의 Collectible 배치
- 점프 시작 시점 안내
- 공중 이동 경로와 착지 지점 안내
- Collectible Score

### 완료 조건

- Stage Mode와 InfiniteMode에서 Collectible을 획득할 수 있다.
- Collectible 획득 시 Score가 한 번만 증가한다.
- 획득한 Collectible은 같은 Run에서 다시 획득할 수 없다.
- Retry와 새로운 Run에서 Collectible 상태가 초기화된다.
- 기본 경로의 Collectible이 점프 시작 시점, 공중 이동 경로와 착지 지점을 일관되게 안내한다.
- Collectible을 놓쳐도 Stage Play를 계속할 수 있다.
- Collectible 배치가 통과할 수 없는 점프를 요구하지 않는다.
- InfiniteMode의 이동 거리 기반 Score가 유지된다.

### 상태

대기

---

## Phase 4

### 목표

Mode별 Score와 UI를 통합하고 Prototype 3의 전체 플레이 흐름을 검증한다.

### 구현 대상

- Stage Mode Collectible Score
- InfiniteMode Distance Score와 Collectible Score
- Total Score
- StageHUD와 InfiniteMode HUD 확장
- Stage Mode와 InfiniteMode ResultPanel 확장
- Pause 및 Retry 상태의 Score 유지와 초기화
- 전체 회귀 검증

### 완료 조건

- Stage Mode에서 Collectible Score를 확인할 수 있다.
- Stage Mode의 Clear Time과 Collectible Score가 서로 독립적으로 표시된다.
- InfiniteMode에서 Distance Score와 Collectible Score를 구분하여 확인할 수 있다.
- InfiniteMode의 Total Score에 Distance Score와 Collectible Score가 반영된다.
- Pause 동안 이동 거리와 Score가 증가하지 않는다.
- Result 상태에서 최종 기록이 변경되지 않는다.
- Retry 시 이전 Run의 Distance, Collectible과 Score 상태가 남지 않는다.
- Keyboard와 Mouse로 기존 Pause 및 Result UI를 조작할 수 있다.
- Stage Mode와 InfiniteMode를 치명적인 오류 없이 반복 플레이할 수 있다.
- Compile, 관련 Test와 Build 검증이 통과한다.

### 상태

대기

---

# 현재 개발 진행 상태

## 진행 중인 작업

없음

---

## 다음 작업

Prototype 3 Phase 1

벽 충돌과 낙하 동작 안정화

---

## 보류된 작업

### Prototype 4

- InfiniteMode Map Pattern 추가
- Map Pattern 조합
- 진행에 따른 난이도 증가 구조

보류 이유

- 자동 이동과 Collectible 안내 경로가 검증된 후 Map Pattern을 확장한다.

---

### 밸런스 검증

- Player 자동 이동 수치 조정
- Jump와 Momentum Landing 수치 조정
- Distance Score와 Collectible Score 환산 값 조정
- Collectible 위치와 획득 판정 범위 조정
- InfiniteMode 난이도 조정

보류 이유

- Prototype 3에서는 기능, 조작 전환과 데이터 흐름을 우선 검증한다.
- 수치 비교와 최종 밸런스 선택은 더 나중의 별도 작업으로 수행한다.

---

### Collectible 확장

- Combo
- Score 배율
- 희귀 Collectible
- 우회 고득점 경로

보류 이유

- 기본 Collectible의 안내와 Score 기능이 검증된 후 확장한다.

---

### Leaderboard

보류 이유

- Mode별 Score 규칙과 Runtime ScoreRecord가 안정된 후 구현한다.

---

### SaveSystem

보류 이유

- Prototype 3는 Runtime Data만 사용한다.

---

## 완료된 단계

없음

---

# 구현 우선순위

1.

벽 충돌과 낙하 안정화

Ground와 Wall 접촉 구분

충돌 회귀 검증

---

2.

Stage Mode와 InfiniteMode 자동 이동

Player 좌우 이동 입력 제거

Jump와 Momentum Landing 입력 유지

---

3.

Score Collectible

점프 경로 안내 배치

Run별 획득 상태 초기화

---

4.

Mode별 Score와 UI 통합

Pause, Result와 Retry 검증

전체 회귀 검증

---

# 완료 기준

다음 조건을 모두 만족하면 3차 프로토타입을 완료한 것으로 판단한다.

- 벽에 충돌한 Player가 자연스럽게 바닥으로 떨어진다.
- Stage Mode와 InfiniteMode에서 Player가 좌측에서 우측으로 자동 이동한다.
- 사용자 Player 입력은 Jump와 Momentum Landing에 집중된다.
- UI Navigate 입력은 정상적으로 유지된다.
- Stage Mode와 InfiniteMode에서 Collectible을 획득할 수 있다.
- Collectible 배치가 점프 시점, 공중 이동 경로와 착지 지점을 안내한다.
- Collectible을 놓쳐도 플레이를 계속할 수 있다.
- Stage Mode에서 Clear Time과 Collectible Score를 확인할 수 있다.
- InfiniteMode에서 Distance Score, Collectible Score와 Total Score를 확인할 수 있다.
- Pause, Result와 Retry에서 이동 및 Score 상태가 올바르게 유지되거나 초기화된다.
- Stage Mode와 InfiniteMode의 기존 완료 및 반복 플레이 흐름에 회귀가 없다.
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
- PlayerInputSystem
- PlayerControllerSystem
- PlayerMovementSystem
- CollisionSystem
- CameraSystem
- StageSystem
- TimerSystem
- ResultSystem

---

## Features

- Jump
- MomentumLanding
- NormalLanding
- InfiniteMode
- ScoreRecord
- GamePause
- StagePlay
- StageClear
- TimeRecord
- ResultMenu

---

# 작성 완료 기준

- Prototype 3의 현재 구현 계획만 작성했다.
- 각 Phase의 목표, 구현 대상, 완료 조건과 상태를 작성했다.
- 구현 방법과 작업 기록을 작성하지 않았다.
- Prototype 4, 밸런스 검증과 Collectible 확장을 보류된 작업으로 구분했다.
- 확인되지 않은 Score 수치, 이동 수치와 밸런스 기준을 작성하지 않았다.
