# 시스템 개요

## 시스템명

StageSystem

---

## 목적

Stage의 진행 상태를 관리한다.

Stage의 시작과 종료를 관리한다.

Stage 종료 시점을 판단하고 Stage 종료 이벤트를 발생시킨다.

---

# 시스템 책임

- Stage를 시작한다.
- 전달받은 게임 Mode에 해당하는 Stage Object 상태를 활성화한다.
- Stage 진행 상태를 관리한다.
- Stage Object 상태를 관리한다.
- GameSystem의 요청에 따라 Stage 진행 판정을 일시 중단하고 재개한다.
- Stage 종료 시점을 판단한다.
- Stage 종료 이벤트를 발생시킨다.
- Stage Mode Playing 중 Player Rigidbody의 Y 위치로 추락 임계값 도달을 판단한다.
- 추락 요청을 다음 물리 구간에 확정하여 같은 물리 구간의 Goal 도달을 먼저 반영한다.

---

# Stage Object

Stage Object는 Stage를 구성하는 모든 오브젝트를 의미한다.

StageSystem은 Stage Object의 동작을 수행하지 않는다.

StageSystem은 Stage Object가 전달한 상태와 이벤트를 관리한다.

---

# 시작 조건

- GameSystem이 Stage 시작을 요청한다.

---

# 종료 조건

## 정상 종료

- Stage 종료 이벤트가 발생하였다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- 현재 Stage
- 현재 Stage의 게임 Mode
- Stage 진행 상태
- Stage Object 상태
- Stage 진행의 일시 중단 상태

---

# 입력

| 입력 | 출처 |
|------|------|
| Stage 시작 요청 | GameSystem |
| 현재 게임 Mode | GameSystem |
| Stage Object 상태 | Stage Object |
| Stage Object 이벤트 | Stage Object |
| Stage 진행 중단 및 재개 요청 | GameSystem |
| Player Rigidbody 물리 위치 | PlayerControllerSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| Stage 시작 알림 | TimerSystem |
| Stage 종료 이벤트 | GameSystem |
| Stage 종료 이벤트 | ResultSystem |
| Stage 종료 이벤트 | UIManagementSystem |

---

# 시스템 경계

## 담당 범위

- Stage 시작
- 게임 Mode별 Stage Object 상태 활성화
- Stage 진행 상태 관리
- Stage Object 상태 관리
- Stage 종료 시점 판단
- Stage 종료 이벤트 발생
- Stage 진행 판정 중단 및 재개
- Stage Mode 추락 임계값 판정과 종료 요청 보류 상태 관리

---

## 담당하지 않는 범위

- 게임 전체 흐름 관리
- 플레이어 이동
- 충돌 판정
- 클리어 시간 측정
- 결과 데이터 생성
- UI 표시
- Stage Object의 동작
- Feature 규칙 정의

---

# 관련 System

- GameSystem
- TimerSystem
- ResultSystem
- UIManagementSystem

---

# 제약 사항

- 게임 전체 흐름을 관리하지 않는다.
- Stage Object를 직접 제어하지 않는다.
- Stage Object의 종류를 관리하지 않는다.
- 게임 Mode를 결정하지 않는다.
- Stage Object가 전달한 상태와 이벤트만 사용한다.
- Stage 종료 조건을 정의하지 않는다.
- Stage 종료 시점만 판단한다.
- 클리어 시간을 측정하지 않는다.
- 결과 데이터를 생성하지 않는다.
- UI를 직접 제어하지 않는다.
- Feature 규칙을 정의하지 않는다.
- Stage 종료 시에는 반드시 Stage 종료 이벤트를 발생시킨다.
- 일시 중단 중에는 Stage Object의 종료 요청을 확정하지 않는다.
- 재개 시 일시 중단 이전의 Stage 진행 상태를 유지한다.
- Stage Mode 추락 임계값은 Player Rigidbody의 Y 위치가 설정값 이하인지 판정한다.
- Pause, Result와 Ended 상태에서는 추락 판정을 수행하지 않는다.
- Goal 도달과 추락 요청이 같은 물리 구간에 발생하면 Goal 도달을 우선한다.
- InfiniteMode 추락 판정은 InfiniteModeSystem이 담당한다.

---

# 문서 작성 원칙

현재 System의 정의만 작성한다.

System의 책임만 작성한다.

Feature 규칙을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

변경 이력을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

System 하나당 문서 하나를 사용한다.
