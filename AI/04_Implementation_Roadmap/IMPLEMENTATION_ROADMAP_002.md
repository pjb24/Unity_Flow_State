# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

2차 프로토타입

하나의 Map Pattern을 사용하는 InfiniteMode의 반복 플레이 구조를 구축한다.

InfiniteMode의 이동 거리를 기록하고 이동 거리에 따라 증가하는 Score를 제공한다.

Stage Play를 안전하게 중단하고 재개할 수 있는 GamePause와 Mode별 UI 흐름을 완성한다.

---

# 개발 단계

## Phase 1

### 목표

InfiniteMode의 기본 플레이 흐름을 구축한다.

### 구현 대상

- InfiniteMode Feature
- InfiniteMode 상태 흐름
- InfiniteMode 전용 Map Pattern 1개
- InfiniteMode 시작, 종료와 Retry
- Stage Mode 회귀 검증

### 완료 조건

- InfiniteMode를 시작할 수 있다.
- 하나의 Map Pattern으로 InfiniteMode를 계속 진행할 수 있다.
- InfiniteMode에서 Goal을 사용하지 않는다.
- 진행 지속 조건을 잃으면 InfiniteMode가 종료된다.
- InfiniteMode 종료 후 같은 실행 세션에서 Retry할 수 있다.
- Stage Mode의 기존 시작, Clear와 Retry 흐름이 유지된다.

### 상태

완료

---

## Phase 2

### 목표

InfiniteMode의 이동 거리와 Score를 기록한다.

### 구현 대상

- InfiniteMode 이동 거리 기록
- 이동 거리 기반 Score
- ScoreRecord Feature
- InfiniteMode 결과 데이터
- Retry 시 기록 초기화

### 완료 조건

- InfiniteMode Run의 이동 거리를 기록할 수 있다.
- 이동 거리에 따라 Score가 증가한다.
- 지나간 Map Pattern 개수는 Score 계산에 사용하지 않는다.
- InfiniteMode 종료 시 최종 이동 거리와 최종 Score가 확정된다.
- 하나의 Run에서 ScoreRecord가 한 번만 수행된다.
- Retry 시 이전 Run의 이동 거리와 Score가 남지 않는다.
- Stage Mode의 TimeRecord와 InfiniteMode의 ScoreRecord가 서로 충돌하지 않는다.

### 상태

대기

---

## Phase 3

### 목표

Stage Play를 중단하고 기존 상태를 유지한 채 재개할 수 있도록 한다.

### 구현 대상

- GamePause Feature
- Pause 상태 전환
- PausePanel
- Resume
- Pause 상태의 Retry와 Quit
- Stage Mode와 InfiniteMode의 Pause 흐름

### 완료 조건

- Stage Mode와 InfiniteMode 플레이 중 GamePause를 시작할 수 있다.
- Pause 동안 Player 입력과 Stage 진행이 중단된다.
- Pause 동안 Time과 InfiniteMode 기록이 증가하지 않는다.
- Resume 후 중단 이전 상태에서 플레이가 계속된다.
- Retry 시 현재 Play 정보를 초기화하고 같은 Mode를 다시 시작한다.
- Quit이 확정된 종료 흐름을 수행한다.
- Result 상태에서는 GamePause가 시작되지 않는다.

### 상태

대기

---

## Phase 4

### 목표

Prototype 2의 Mode별 UI와 전체 반복 플레이 흐름을 완성한다.

### 구현 대상

- Stage Mode와 InfiniteMode UI 구분
- InfiniteMode HUD
- 이동 거리와 현재 Score 표시
- InfiniteMode ResultPanel
- PausePanel UI 마무리
- Keyboard와 Mouse UI 입력
- 전체 회귀 검증

### 완료 조건

- 현재 Mode와 게임 상태에 맞는 UI만 표시된다.
- InfiniteMode HUD에서 이동 거리와 현재 Score를 확인할 수 있다.
- InfiniteMode 종료 후 최종 이동 거리와 최종 Score를 확인할 수 있다.
- PausePanel에서 Resume, Retry와 Quit을 조작할 수 있다.
- Keyboard와 Mouse로 모든 확정 UI 흐름을 조작할 수 있다.
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

Prototype 2 Phase 2

InfiniteMode 이동 거리와 Score 기록

---

## 보류된 작업

### Prototype 3

- 벽 충돌 상태에서 자연스럽게 바닥으로 떨어지는 물리 동작 안정화
- Stage Mode와 InfiniteMode의 좌측에서 우측 자동 이동
- Player의 좌우 이동 입력 제거
- Jump와 Momentum Landing 중심 조작
- Score를 추가하는 Collectible
- Collectible을 이용한 점프 시점과 이동 거리 안내
- Collectible Score와 UI

보류 이유

- 자동 이동과 Collectible은 InfiniteMode 기본 흐름과 이동 거리 기반 Score가 검증된 후 구현한다.

---

### Prototype 4

- InfiniteMode Map Pattern 추가
- Map Pattern 조합
- 진행에 따른 난이도 증가 구조

보류 이유

- 자동 이동과 Collectible 경로가 확정된 후 Map Pattern을 확장한다.

---

### 밸런스 검증

- Player 이동 수치 조정
- 이동 거리 Score 환산 값 조정
- InfiniteMode 난이도 조정

보류 이유

- Prototype 2에서는 기능과 데이터 흐름만 검증한다.
- 수치 비교와 최종 밸런스 선택은 더 나중의 별도 작업으로 수행한다.

---

### Leaderboard

보류 이유

- Runtime ScoreRecord가 안정된 후 구현한다.

---

### SaveSystem

보류 이유

- Prototype 2는 Runtime Data만 사용한다.

---

## 완료된 단계

### Prototype 2 Phase 1

- InfiniteMode 기본 상태 흐름
- InfiniteMode 전용 Map Pattern 1개
- InfiniteMode 시작, 종료와 Retry
- Stage Mode 회귀 검증
- Unity Script Compilation 성공
- Edit Mode Test `68 Passed, 0 Failed`
- Play Mode Test `60 Passed, 0 Failed`
- 최종 수동 검증 완료

---

# 구현 우선순위

1.

InfiniteMode 기본 상태 흐름

InfiniteMode 전용 Map Pattern 1개

시작, 종료와 Retry

---

2.

이동 거리 기록

이동 거리 기반 Score

ScoreRecord

---

3.

GamePause

Resume, Retry와 Quit

---

4.

InfiniteMode HUD와 ResultPanel

PausePanel

Mode별 UI 마무리

전체 회귀 검증

---

# 완료 기준

다음 조건을 모두 만족하면 2차 프로토타입을 완료한 것으로 판단한다.

- 하나의 Map Pattern으로 InfiniteMode를 시작하고 종료할 수 있다.
- InfiniteMode를 같은 실행 세션에서 반복 플레이할 수 있다.
- InfiniteMode의 이동 거리를 기록할 수 있다.
- 이동 거리에 따라 Score가 증가한다.
- Map Pattern 통과 개수를 Score로 사용하지 않는다.
- InfiniteMode 종료 시 최종 이동 거리와 최종 Score를 확인할 수 있다.
- Stage Mode와 InfiniteMode에서 GamePause와 Resume이 정상적으로 동작한다.
- Mode와 상태에 맞는 HUD, PausePanel과 ResultPanel이 표시된다.
- Keyboard와 Mouse로 확정된 UI를 조작할 수 있다.
- Stage Mode의 기존 기능에 회귀가 없다.
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
- PlayerMovementSystem
- StageSystem
- TimerSystem
- ResultSystem

---

## Features

- InfiniteMode
- ScoreRecord
- GamePause
- StagePlay
- TimeRecord
- ResultMenu

---

# 작성 완료 기준

- Prototype 2의 현재 구현 계획만 작성했다.
- 각 Phase의 목표, 구현 대상, 완료 조건과 상태를 작성했다.
- 구현 방법과 작업 기록을 작성하지 않았다.
- Prototype 3, Prototype 4와 밸런스 검증을 보류된 작업으로 구분했다.
- 확인되지 않은 세부 수치와 밸런스 기준을 작성하지 않았다.
