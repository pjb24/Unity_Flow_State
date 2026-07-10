# 시스템 개요

## 시스템명

UIManagementSystem

---

## 목적

게임에서 사용하는 UI를 관리한다.

UI Configuration과 UI State를 관리하고 현재 UI State를 Unity UI에 반영한다.

---

# 시스템 책임

- UI Configuration을 관리한다.
- UI State를 관리한다.
- UI State를 변경한다.
- 현재 활성 UI를 관리한다.
- UI State를 Unity UI에 반영한다.

---

# UI Configuration

UI Configuration은 UI의 고정 정보를 의미한다.

UI Configuration은 런타임 동안 자주 변경되지 않는다.

예를 들어 아래 정보를 포함한다.

- 등록된 UI 목록
- UI State와 UI의 매핑 정보
- 기본 UI State

UIManagementSystem은 UI Configuration을 기준으로 UI를 관리한다.

---

# UI State

UI State는 현재 활성화되어야 하는 UI 구성을 의미한다.

예를 들어 아래 상태가 존재할 수 있다.

- Main Menu
- Stage HUD
- Pause
- Result
- Loading

UI State의 변경 시점은 GameSystem 또는 관련 System이 결정한다.

UIManagementSystem은 현재 UI State를 기준으로 UI를 활성화하거나 비활성화한다.

---

# 시작 조건

- GameSystem이 System 초기화를 요청한다.

---

# 종료 조건

## 정상 종료

- 게임 종료 절차가 완료된다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

## UI Configuration

- 등록된 UI 목록
- UI State와 UI의 매핑 정보
- 기본 UI State

---

## UI State

- 현재 UI State
- 현재 활성 UI
- UI 표시 상태
- UI 전환 상태

---

# 입력

| 입력 | 출처 |
|------|------|
| UI State 변경 요청 | GameSystem |
| UI 입력 상태 | UIInputSystem |
| 결과 데이터 | ResultSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| UI State 반영 | Unity UI |
| 현재 UI State | GameSystem |

---

# 시스템 경계

## 담당 범위

- UI Configuration 관리
- UI State 관리
- UI State 변경
- UI 활성화
- UI 비활성화
- UI 표시 상태 관리
- UI State 반영

---

## 담당하지 않는 범위

- UI State 변경 시점 결정
- UI 입력 수집
- 게임 상태 관리
- 플레이어 이동
- 결과 데이터 생성
- Action Map 관리
- Feature 규칙 정의

---

# 관련 System

- GameSystem
- UIInputSystem
- ResultSystem

---

# 제약 사항

- UI 동작 규칙을 정의하지 않는다.
- 게임 상태를 결정하지 않는다.
- UI 입력을 직접 처리하지 않는다.
- 결과 데이터를 생성하지 않는다.
- UI State 변경 요청을 받은 경우에만 UI State를 변경한다.
- UI Configuration과 UI State를 구분하여 관리한다.
- UI State를 기반으로 Unity UI를 갱신한다.

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