# 기능 개요

## 기능명

CameraFollow

---

## 목적

플레이어가 이동하는 동안 카메라가 플레이에 필요한 화면을 유지하도록 한다.

3D 오소그래픽 횡스크롤 환경에서 안정적인 시야를 제공한다.

---

# 기능 규칙

- CameraFollow는 Stage Play가 시작되면 수행한다.
- 카메라는 플레이어를 기준으로 이동한다.
- Stage Play 동안 카메라는 진행 축(X축)을 따라 플레이어를 추적한다.
- 높이(Y축)와 거리(Z축)는 CameraSystem에서 정의한 기본 값을 유지한다.
- 카메라는 Stage 진행에 필요한 화면을 유지한다.
- CameraFollow는 Stage Play가 종료될 때까지 계속 수행한다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 CameraFollow를 시작한다.

- Stage Play가 시작되었다.
- 플레이어가 생성되었다.
- Cinemachine Camera가 활성화되었다.

---

# 종료 조건

## 정상 종료

- Stage Play가 종료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- 카메라가 진행 축(X축)을 따라 플레이어를 지속적으로 추적한다.
- 카메라의 높이(Y축)가 CameraSystem에서 정의한 기본 값을 유지한다.
- 카메라와 플레이어 사이의 거리(Z축)가 CameraSystem에서 정의한 기본 값을 유지한다.
- 플레이어는 진행 방향을 계속 확인할 수 있다.

---

# 예외 사항

- 플레이어가 생성되지 않은 경우 수행하지 않는다.
- Cinemachine Camera가 비활성화된 경우 수행하지 않는다.
- Stage Play가 진행 중이 아닌 경우 수행하지 않는다.

---

# 관련 System

- CameraSystem
- PlayerMovementSystem
- StageSystem

---

# 제약 사항

- CameraFollow는 Stage Play 동안만 수행한다.
- CameraFollow는 하나의 활성 Cinemachine Camera에 대해서만 수행한다.
- Stage Play 동안 카메라는 진행 축(X축)만 플레이어를 추적한다.
- 높이(Y축)와 거리(Z축)는 CameraSystem에서 정의한 기본 값을 유지한다.
- 카메라는 3D 오소그래픽 투영을 유지한다.
- CameraFollow는 카메라의 생성, 제거 또는 전환을 수행하지 않는다.

---

# 검증 항목

- Stage Play 시작 시 CameraFollow가 시작되는지 확인한다.
- 플레이어 이동에 따라 카메라가 진행 축(X축)을 정상적으로 추적하는지 확인한다.
- 플레이어가 점프하여도 카메라의 높이(Y축)가 CameraSystem에서 정의한 기본 값을 유지하는지 확인한다.
- 카메라와 플레이어 사이의 거리(Z축)가 CameraSystem에서 정의한 기본 값을 유지하는지 확인한다.
- 플레이어가 진행 방향을 지속적으로 확인할 수 있는지 확인한다.
- Stage Play 종료 시 CameraFollow가 종료되는지 확인한다.
- 플레이어가 없는 경우 CameraFollow가 수행되지 않는지 확인한다.
- Cinemachine Camera가 활성화되지 않은 경우 CameraFollow가 수행되지 않는지 확인한다.
- 카메라가 3D 오소그래픽 투영을 유지하는지 확인한다.

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