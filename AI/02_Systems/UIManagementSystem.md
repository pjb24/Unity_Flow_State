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
- 현재 UI 선택 상태를 관리한다.
- GameSystem의 요청에 따라 UI 선택 상태를 변경한다.
- Pause UI State와 PausePanel 선택 상태를 관리한다.
- 현재 게임 Mode와 게임 상태에 맞는 HUD, PausePanel, ResultPanel과 Result Content의 표시 조합을 관리한다.
- InfiniteMode Runtime Data의 현재 거리와 현재 Score를 InfiniteHUD에 표시한다.
- Result Data의 최종 거리와 최종 Score를 InfiniteMode Result Content에 표시한다.

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
- Pause
- Loading

UI State의 변경 시점은 GameSystem 또는 관련 System이 결정한다.

UIManagementSystem은 현재 UI State를 기준으로 UI를 활성화하거나 비활성화한다.

---

# UI 선택 상태

UI 선택 상태는 현재 UI State에서 선택된 UI 항목을 의미한다.

UIManagementSystem은 GameSystem의 요청에 따라 UI 선택 상태를 변경한다.

UIManagementSystem은 현재 선택된 UI 항목을 GameSystem에 제공한다.

UIManagementSystem은 선택된 항목이 수행할 게임 동작을 결정하지 않는다.

Pause UI State에는 Resume, Retry와 Quit 선택 항목이 존재한다.

Pause UI State가 활성화되면 Resume을 기본 선택 항목으로 사용한다.

PausePanel 선택 상태는 ResultMenu 선택 상태와 독립적으로 관리한다.

---

# Result Data 반영

UIManagementSystem은 ResultSystem이 제공한 Result Data를 Result UI에 반영한다.

일반 Stage의 클리어 시간은 초 단위로 소수점 셋째 자리까지 표시한다.

표시 형식은 아래와 같다.

```text
Clear Time: 12.345 s
```

UIManagementSystem은 클리어 시간을 다시 계산하지 않는다.

InfiniteMode의 최종 이동 거리와 최종 Score는 아래 형식으로 표시한다.

```text
Final Distance: 12
Final Score: 123
```

거리는 소수점 없이 내림 처리하여 표시한다.

표시를 위한 내림은 원본 Result Data를 변경하지 않는다.

최종 Score는 Result Data의 `int` 값을 다시 계산하지 않고 그대로 표시한다.

---

# InfiniteMode HUD 반영

InfiniteMode의 현재 이동 거리와 현재 Score는 아래 형식으로 표시한다.

```text
Distance: 12
Score: 123
```

거리는 소수점 없이 내림 처리하여 표시한다.

표시를 위한 내림은 원본 Runtime Data를 변경하지 않는다.

현재 Score는 Runtime Data의 `int` 값을 다시 계산하지 않고 그대로 표시한다.

Runtime Data가 없거나 초기화되지 않은 경우와 유효하지 않은 값은 `--`로 표시한다.

InfiniteMode Playing 동안 표시 대상 값을 화면 프레임마다 확인하되 실제 표시값이 변경된 경우에만 Text를 갱신한다.

Pause, Ending, Result와 Ended에서는 마지막 HUD 표시값을 유지한다.

---

# Mode와 상태별 표시 조합

- Initializing과 Ready에서는 HUD, PausePanel과 ResultPanel을 표시하지 않는다.
- Stage Mode Playing에서는 StageHUD만 표시한다.
- InfiniteMode Playing에서는 InfiniteHUD만 표시한다.
- Paused에서는 현재 Mode의 HUD와 PausePanel을 함께 표시한다.
- Ending에서는 현재 Mode의 HUD를 유지한다.
- Result와 Ended에서는 현재 Mode의 HUD와 ResultPanel을 함께 표시한다.
- Stage Result에서는 StageResultContent만 표시한다.
- InfiniteMode Result에서는 InfiniteResultContent만 표시한다.
- PausePanel과 ResultPanel은 현재 Mode의 HUD보다 앞에 표시한다.
- 현재 Mode가 아닌 HUD와 Result Content는 표시하지 않는다.

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

## UI 선택 상태

- 현재 선택된 UI 항목
- UI 선택 표시 상태
- 현재 PausePanel 선택 항목

---

# 입력

| 입력 | 출처 |
|------|------|
| UI State 변경 요청 | GameSystem |
| UI 선택 상태 변경 요청 | GameSystem |
| 결과 데이터 | ResultSystem |
| 현재 게임 Mode와 상태 | GameSystem |
| InfiniteMode 현재 이동 거리와 현재 Score | Runtime Data |

---

# 출력

| 출력 | 대상 |
|------|------|
| UI State 반영 | Unity UI |
| 현재 UI State | GameSystem |
| 현재 선택된 UI 항목 | GameSystem |

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
- UI 선택 상태 관리
- UI 선택 상태 변경
- UI 선택 표시 반영
- PausePanel 선택 상태 관리 및 표시 반영
- Mode와 상태별 UI 표시 조합 관리
- InfiniteMode HUD와 Result Text 표시

---

## 담당하지 않는 범위

- UI State 변경 시점 결정
- UI 입력 수집
- UI 입력 의미 판단
- 선택된 UI 항목의 게임 동작 결정
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
- 선택된 UI 항목이 수행할 게임 동작을 결정하지 않는다.
- PausePanel과 ResultMenu의 선택 상태를 공유하지 않는다.
- 결과 데이터를 생성하지 않는다.
- InfiniteMode 이동 거리와 Score를 계산하지 않는다.
- UI State 변경 요청을 받은 경우에만 UI State를 변경한다.
- UI Configuration과 UI State를 구분하여 관리한다.
- UI State를 기반으로 Unity UI를 갱신한다.
- UI 표시를 위한 거리 내림은 Runtime Data와 Result Data의 원본 값을 변경하지 않는다.

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
