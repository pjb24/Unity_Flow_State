# 시스템 개요

## 시스템명

PlayerMovementSystem

---

## 목적

플레이어 이동 결과를 계산한다.

플레이 입력 상태와 충돌 상태를 이용하여 플레이어의 이동 방향과 이동 속도를 계산한다.

계산된 이동 결과를 필요한 System에 제공한다.

---

# 시스템 책임

- 플레이어 이동 방향을 계산한다.
- 플레이어 이동 속도를 계산한다.
- 플레이어 수직 속도를 계산한다.
- 이동 결과를 생성한다.
- 충돌 상태를 반영하여 이동 결과를 보정한다.

---

# 시작 조건

- GameSystem이 System 초기화를 요청한다.
- 플레이 가능한 상태가 된다.

---

# 종료 조건

## 정상 종료

- 플레이가 종료된다.
- GameSystem이 게임 종료 절차를 시작한다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- 현재 이동 방향
- 현재 이동 속도
- 현재 수직 속도
- 현재 이동 결과

---

# 입력

| 입력 | 출처 |
|------|------|
| 플레이 입력 상태 | PlayerInputSystem |
| 충돌 상태 | CollisionSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| 플레이어 이동 결과 | PlayerControllerSystem |
| Player Movement Runtime Data | RuntimeDataSystem |

---

# 시스템 경계

## 담당 범위

- 플레이어 이동 계산
- 플레이어 속도 계산
- 플레이어 이동 방향 계산
- 충돌 상태를 반영한 이동 결과 보정
- 이동 결과 제공

---

## 담당하지 않는 범위

- 플레이 입력 수집
- 충돌 판정
- Player GameObject 제어
- Transform 갱신
- CharacterController 제어
- Rigidbody 제어
- 점프 규칙 정의
- 관성 착지 규칙 정의
- Stage 진행
- UI 처리
- 게임 전체 흐름 관리

---

# 관련 System

- GameSystem
- PlayerInputSystem
- CollisionSystem
- PlayerControllerSystem
- RuntimeDataSystem

---

# 제약 사항

- 입력을 직접 수집하지 않는다.
- 충돌을 직접 판정하지 않는다.
- Player GameObject를 직접 제어하지 않는다.
- Transform을 직접 갱신하지 않는다.
- Feature 규칙을 정의하지 않는다.
- 이동 계산에 필요한 입력과 충돌 상태만 사용한다.
- 계산 결과는 Runtime에서만 사용한다.

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