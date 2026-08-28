# 시스템 개요

## 시스템명

UIInputSystem

---

## 목적

UI 입력을 수집한다.

UI Action Map을 관리한다.

수집한 UI 입력 상태를 GameSystem에 제공한다.

UI 입력 장치와 UI System 사이의 연결을 담당한다.

---

# 시스템 책임

- UI Action Map을 관리한다.
- GameSystem의 요청에 따라 UI Action Map을 활성화한다.
- GameSystem의 요청에 따라 UI Action Map을 비활성화한다.
- UI 입력을 수집한다.
- UI 입력 상태를 관리한다.
- UI 입력 상태를 GameSystem에 전달한다.
- UI 입력 장치의 변경을 추상화한다.

---

# UI Action Map

UI Action Map은 UI 입력을 처리하기 위한 입력 집합이다.

UIInputSystem은 UI Action Map의 활성 상태를 관리한다.

UI Action Map의 활성 여부는 GameSystem이 결정한다.

UIInputSystem은 GameSystem의 요청에 따라 UI Action Map의 상태만 변경한다.

UI Action Map은 Player Action Map과 독립적으로 관리한다.

---

# 시작 조건

- GameSystem이 System 초기화를 요청한다.
- UI 입력을 받을 수 있는 상태가 된다.

---

# 종료 조건

## 정상 종료

- UI 입력이 종료된다.
- GameSystem이 게임 종료 절차를 시작한다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- UI Action Map
- UI Action Map 활성 상태
- 현재 프레임의 UI 입력 상태
- UI 선택 입력
- UI 이동 입력
- UI 확인 입력
- UI 취소 입력

---

# 입력

| 입력 | 출처 |
|------|------|
| UI Action Map 활성화 요청 | GameSystem |
| UI Action Map 비활성화 요청 | GameSystem |
| 키보드 입력 | Unity Input System |
| 마우스 입력 | Unity Input System |
| 게임패드 입력 | Unity Input System |

---

# 출력

| 출력 | 대상 |
|------|------|
| 현재 UI 입력 상태 | GameSystem |

---

# 시스템 경계

## 담당 범위

- UI Action Map 관리
- UI Action Map 활성 상태 변경
- UI 입력 수집
- UI 입력 상태 관리
- UI 입력 상태 제공
- UI 입력 장치 추상화

---

## 담당하지 않는 범위

- Player Action Map 관리
- 어떤 Action Map을 사용할지 결정
- 플레이 중 입력 처리
- 플레이어 이동
- UI 표시
- UI 화면 전환
- UI 상태 관리
- UI 동작 규칙
- Feature 규칙 수행

---

# 관련 System

- GameSystem
- PlayerInputSystem
- UIManagementSystem

---

# 제약 사항

- UI 입력만 관리한다.
- Player Action Map을 관리하지 않는다.
- 어떤 Action Map을 사용할지는 결정하지 않는다.
- GameSystem의 요청에 따라서만 UI Action Map의 상태를 변경한다.
- 플레이 중 입력을 관리하지 않는다.
- 입력의 의미를 판단하지 않는다.
- UI 입력으로 수행할 동작을 결정하지 않는다.
- UI 동작 규칙을 정의하지 않는다.
- Feature 규칙을 정의하지 않는다.
- 입력 상태만 관리한다.
- 입력 장치에 의존하는 처리는 이 System 내부에서만 관리한다.
- 입력 데이터는 Runtime에서만 사용한다.

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
