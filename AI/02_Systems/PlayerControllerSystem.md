# 시스템 개요

## 시스템명

PlayerControllerSystem

---

## 목적

계산된 플레이어 이동 결과를 실제 Player GameObject에 적용한다.

Player GameObject의 물리 상태를 Unity Rigidbody와 연결한다.

---

# 시스템 책임

- Player GameObject의 Rigidbody를 관리한다.
- PlayerMovementSystem이 계산한 이동 결과를 Rigidbody에 적용한다.
- Player GameObject의 실제 위치와 물리 상태를 Unity 엔진에 반영한다.
- Player GameObject의 현재 상태를 필요한 System에 제공한다.
- 플레이 시작 시 Player를 Stage 시작 위치로 이동하고 물리 상태를 초기화한다.

---

# 시작 조건

- GameSystem이 System 초기화를 요청한다.
- 플레이 가능한 상태가 된다.
- Player GameObject와 Rigidbody가 준비된다.

---

# 종료 조건

## 정상 종료

- 플레이가 종료된다.
- GameSystem이 게임 종료 절차를 시작한다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- Player GameObject
- Player Transform
- Player Rigidbody

---

# 입력

| 입력 | 출처 |
|------|------|
| 플레이어 이동 결과 | PlayerMovementSystem |
| Player 시작 위치 | Stage Object |

---

# 출력

| 출력 | 대상 |
|------|------|
| Player Rigidbody 상태 변경 | Unity |
| 현재 Player 위치 | CollisionSystem |
| 현재 Player 물리 상태 | CollisionSystem |
| 현재 Player 위치 | CameraSystem |

---

# 시스템 경계

## 담당 범위

- Player GameObject 제어
- Player Rigidbody 관리
- Rigidbody를 이용한 이동 결과 적용
- Player Transform 상태 제공
- Player 물리 상태 제공
- Player 시작 위치 적용
- Player 물리 상태 초기화

---

## 담당하지 않는 범위

- 플레이 입력 수집
- 이동 결과 계산
- 충돌 판정
- 점프 규칙 정의
- 관성 착지 규칙 정의
- Stage 진행
- UI 처리
- 게임 전체 흐름 관리
- Camera 제어

---

# 관련 System

- GameSystem
- PlayerMovementSystem
- CollisionSystem
- CameraSystem

---

# 제약 사항

- CharacterController를 사용하지 않는다.
- Player 이동 적용은 Rigidbody를 기준으로 처리한다.
- 이동 결과를 직접 계산하지 않는다.
- 입력을 직접 처리하지 않는다.
- 충돌을 직접 판정하지 않는다.
- Feature 규칙을 정의하지 않는다.
- PlayerMovementSystem이 계산한 이동 결과만 적용한다.
- Player GameObject 제어만 담당한다.
- 플레이 시작 위치가 준비되어야 한다.

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
