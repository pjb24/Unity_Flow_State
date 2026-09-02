# 시스템 개요

## 시스템명

RuntimeDataSystem

---

## 목적

게임 실행 중 여러 System이 공유해야 하는 Runtime Data를 관리한다.

Runtime Data를 생성하고 필요한 System에 제공하며 게임 종료 시 제거한다.

---

# 시스템 책임

- Runtime Data를 생성한다.
- Runtime Data를 관리한다.
- Runtime Data를 제공한다.
- Runtime Data를 제거한다.
- Runtime Data 생성 상태를 관리한다.

---

# Runtime Data

Runtime Data는 게임 실행 중 여러 System이 공유해야 하는 데이터를 의미한다.

Runtime Data는 게임이 종료되면 제거된다.

RuntimeDataSystem은 System 간 공유가 필요한 Runtime Data만 관리한다.

각 System의 내부 상태는 해당 System이 직접 관리한다.

RuntimeDataSystem은 각 System의 내부 상태를 소유하지 않는다.

예를 들어 아래 데이터가 Runtime Data가 될 수 있다.

- 현재 게임 모드
- 현재 Stage ID
- 현재 Stage 진행 상태
- 현재 Player Runtime ID
- 현재 플레이 세션 ID
- 현재 입력 가능 상태
- 현재 게임 상태
- 현재 Result Data 참조

---

# 시작 조건

- GameSystem이 Runtime Data 생성을 요청한다.

---

# 종료 조건

## 정상 종료

- GameSystem이 Runtime Data 제거를 요청한다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- Runtime Data
- Runtime Data 생성 상태

---

# 입력

| 입력 | 출처 |
|------|------|
| Runtime Data 생성 요청 | GameSystem |
| Runtime Data 제거 요청 | GameSystem |
| Runtime Data 접근 요청 | 관련 System |

---

# 출력

| 출력 | 대상 |
|------|------|
| Runtime Data | 관련 System |
| Runtime Data 생성 완료 | GameSystem |
| Runtime Data 제거 완료 | GameSystem |

---

# 시스템 경계

## 담당 범위

- Runtime Data 생성
- Runtime Data 관리
- Runtime Data 제공
- Runtime Data 제거
- Runtime Data 생성 상태 관리
- System 간 공유 Runtime Data 관리

---

## 담당하지 않는 범위

- 각 System의 내부 상태 관리
- Runtime Data 사용 규칙
- Runtime Data 변경 규칙
- 결과 데이터 생성
- 저장 데이터 관리
- 게임 진행 관리
- Feature 규칙 정의

---

# 관련 System

- GameSystem
- StageSystem
- ResultSystem
- TimerSystem
- PlayerControllerSystem
- PlayerInputSystem
- UIInputSystem
- UIManagementSystem
- CameraSystem
- CollisionSystem

---

# 제약 사항

- Runtime Data를 저장하지 않는다.
- Runtime Data를 영속화하지 않는다.
- 각 System의 내부 상태를 소유하지 않는다.
- Runtime Data의 사용 목적을 판단하지 않는다.
- Runtime Data의 변경 규칙을 정의하지 않는다.
- Feature 규칙을 정의하지 않는다.
- GameSystem의 요청에 따라 Runtime Data를 생성하거나 제거한다.
- Runtime Data는 게임 실행 중에만 유지한다.

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
