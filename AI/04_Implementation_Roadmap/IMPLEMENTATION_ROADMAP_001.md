# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

1차 프로토타입

플레이어가 점프와 관성 착지를 이용하여 스테이지를 빠르게 돌파하는 핵심 플레이를 검증한다.

---

# 개발 단계

## Phase 1

### 목표

프로젝트의 기본 실행 환경을 구축한다.

### 구현 대상

- GameSystem
- RuntimeDataSystem
- UIManagementSystem
- 기본 Scene 구성
- 프로젝트 공통 설정

### 완료 조건

- 게임을 실행할 수 있다.
- 게임 시작부터 종료까지 기본 흐름이 존재한다.
- 각 System이 정상적으로 초기화된다.

### 상태

완료

---

## Phase 2

### 목표

플레이어의 핵심 이동을 구현한다.

### 구현 대상

- PlayerInputSystem
- PlayerControllerSystem
- PlayerMovementSystem
- CameraSystem
- CameraFollow Feature

### 완료 조건

- 플레이어가 이동할 수 있다.
- 플레이어가 점프할 수 있다.
- 관성 착지가 적용된다.
- 카메라가 플레이어를 정상적으로 추적한다.

### 상태

완료

---

## Phase 3

### 목표

스테이지 플레이가 가능하도록 구성한다.

### 구현 대상

- StageSystem
- CollisionSystem
- Stage Feature

### 완료 조건

- 스테이지를 시작할 수 있다.
- 충돌이 정상적으로 처리된다.
- 목표 지점까지 이동하여 스테이지를 종료할 수 있다.

### 상태

대기

---

## Phase 4

### 목표

게임 플레이 결과를 처리한다.

### 구현 대상

- TimerSystem
- ResultSystem
- TimeRecord Feature

### 완료 조건

- 플레이 시간을 측정한다.
- 스테이지 종료 시 결과 화면을 표시한다.
- 클리어 시간을 확인할 수 있다.

### 상태

대기

---

## Phase 5

### 목표

프로토타입 완성 및 플레이 검증

### 구현 대상

- UIInputSystem
- ScoreRecord Feature
- UI 마무리
- 플레이 테스트
- 밸런스 조정

### 완료 조건

- 게임 시작부터 결과 화면까지 플레이 가능하다.
- 치명적인 오류 없이 반복 플레이가 가능하다.
- 핵심 재미를 검증할 수 있다.

### 상태

대기

---

# 현재 개발 진행 상태

## 진행 중인 작업

없음

---

## 다음 작업

Phase 3

스테이지 플레이 구성

---

## 보류된 작업

LeaderBoard

보류 이유

- 프로토타입 검증 이후 구현한다.

---

SaveSystem

보류 이유

- 현재 프로젝트는 Runtime Data만 사용한다.

---

무한 모드

보류 이유

- 핵심 플레이 검증 이후 구현한다.

---

## 완료된 단계

Phase 1

프로젝트 기본 실행 환경 구축

---

Phase 2

플레이어의 핵심 이동 구현

---

# 구현 우선순위

1.

GameSystem

RuntimeDataSystem

UIManagementSystem

---

2.

PlayerInputSystem

PlayerControllerSystem

PlayerMovementSystem

CameraSystem

CameraFollow

---

3.

StageSystem

CollisionSystem

Stage

---

4.

TimerSystem

ResultSystem

TimeRecord

---

5.

UIInputSystem

ScoreRecord

플레이 테스트

밸런스 조정

---

# 완료 기준

다음 조건을 모두 만족하면 1차 프로토타입을 완료한 것으로 판단한다.

- 게임 실행이 가능하다.
- 플레이어가 이동할 수 있다.
- 점프와 관성 착지가 정상적으로 동작한다.
- 하나의 스테이지를 시작부터 종료까지 플레이할 수 있다.
- 플레이 시간을 측정할 수 있다.
- 결과 화면을 표시할 수 있다.
- 반복 플레이가 가능하다.
- 핵심 플레이의 재미를 검증할 수 있다.

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
- CameraSystem
- StageSystem
- CollisionSystem
- TimerSystem
- ResultSystem

---

## Features

- CameraFollow
- Stage
- TimeRecord
- ScoreRecord
