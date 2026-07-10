# 시스템 개요

## 시스템명

CameraSystem

---

## 목적

게임에서 사용하는 Cinemachine Camera를 관리한다.

3D 오소그래픽 횡스크롤 카메라 설정과 상태를 관리한다.

Camera 상태 변경 결과를 Cinemachine Camera에 반영한다.

---

# 시스템 책임

- Cinemachine Camera를 관리한다.
- Camera 설정을 관리한다.
- Camera 상태를 관리한다.
- Camera 위치를 갱신한다.
- Camera 추적 대상을 관리한다.
- Camera 결과를 Cinemachine Camera에 반영한다.

---

# Camera 설정

Camera 설정은 런타임 중 자주 변경되지 않는 기준값이다.

- Projection
- Orthographic 설정
- 기본 Camera Size
- Near Clip
- Far Clip
- 기본 Follow Offset
- 기본 Look At 대상

---

# Camera 상태

Camera 상태는 런타임 중 변경될 수 있는 값이다.

- 현재 활성 Cinemachine Camera
- 현재 Position
- 현재 Rotation
- 현재 Camera Size
- 현재 Follow 대상
- 현재 Look At 대상
- 현재 Camera 상태

---

# 시작 조건

- GameSystem이 System 초기화를 요청한다.
- Cinemachine Camera가 준비된다.

---

# 종료 조건

## 정상 종료

- GameSystem이 게임 종료 절차를 시작한다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- Cinemachine Camera
- Camera 설정
- Camera 상태
- Camera Follow 대상
- Camera Look At 대상

---

# 입력

| 입력 | 출처 |
|------|------|
| Camera 제어 요청 | Camera 사용자 |
| Player 위치 또는 Transform | PlayerControllerSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| Cinemachine Camera 상태 변경 | Unity Cinemachine |
| Camera 상태 | Camera 사용자 |

---

# Camera 사용자

Camera 사용자는 Camera 상태 변경을 요청하는 System 또는 Feature를 의미한다.

예를 들어 아래 항목이 Camera 사용자가 될 수 있다.

- Camera Follow
- Camera Zoom
- Camera Shake
- Stage Event
- Cut Scene
- Replay

CameraSystem은 Camera 사용자의 목적을 판단하지 않는다.

CameraSystem은 Camera 사용자가 요청한 Camera 상태만 관리한다.

---

# 시스템 경계

## 담당 범위

- Cinemachine Camera 관리
- Camera 설정 관리
- Camera 상태 관리
- Camera Follow 대상 관리
- Camera Look At 대상 관리
- Camera 상태 변경 반영

---

## 담당하지 않는 범위

- 플레이어 이동
- 플레이어 입력
- Camera 추적 규칙
- Camera 흔들림 규칙
- Camera 확대 및 축소 규칙
- Stage 연출 규칙
- Cut Scene 규칙
- Replay 규칙
- 게임 전체 흐름 관리
- Feature 규칙 정의

---

# 관련 System

- GameSystem
- PlayerControllerSystem

---

# 제약 사항

- Unity 기본 Camera를 직접 제어 대상으로 삼지 않는다.
- Camera 제어는 Cinemachine Camera를 기준으로 처리한다.
- Camera 동작 규칙을 정의하지 않는다.
- Camera 사용 목적을 판단하지 않는다.
- Feature 규칙을 정의하지 않는다.
- Camera 설정과 Camera 상태를 구분하여 관리한다.
- 프로젝트에서는 3D 오소그래픽 횡스크롤 Camera를 사용한다.
- Camera 상태 변경 요청은 Cinemachine Camera에 반영 가능한 값만 사용한다.

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