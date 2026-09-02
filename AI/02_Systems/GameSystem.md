# 시스템 개요

## 시스템명

GameSystem

---

## 목적

게임 전체의 실행 흐름을 관리한다.

게임 시작부터 게임 종료까지 각 System의 실행 순서를 관리한다.

System 간의 실행 흐름을 연결한다.

현재 게임 Mode를 결정하고 관리한다.

현재 게임 상태에 따라 사용할 입력 Action Map을 결정한다.

---

# 시스템 책임

- 게임 시작을 관리한다.
- 게임 종료를 관리한다.
- 선택된 게임 Mode를 관리한다.
- 게임 시작 시 선택된 게임 Mode를 RuntimeDataSystem과 StageSystem에 전달한다.
- Retry 시 종료된 Stage Play와 같은 게임 Mode로 새로운 게임 실행 흐름을 시작한다.
- 현재 게임 상태를 관리한다.
- Playing과 Paused 사이의 상태 전환을 관리한다.
- 현재 게임 상태에 따라 사용할 Action Map을 결정한다.
- PlayerInputSystem에 Player Action Map 활성화 또는 비활성화를 요청한다.
- UIInputSystem에 UI Action Map 활성화 또는 비활성화를 요청한다.
- UIInputSystem이 제공한 UI 입력 상태를 현재 게임 상태에 따라 해석한다.
- UIManagementSystem에 UI 선택 상태 변경을 요청한다.
- 선택된 UI 항목에 해당하는 게임 실행 흐름을 시작한다.
- Pause와 Resume 시 관련 System의 중단 및 재개 순서를 관리한다.
- Pause 상태의 Retry를 기존 게임 시작 흐름에 연결한다.
- Result와 Pause 상태의 Quit을 하나의 Application 종료 요청 흐름에 연결한다.
- RuntimeDataSystem에 Runtime Data 생성 및 제거를 요청한다.
- 필요한 System의 초기화를 요청한다.
- InfiniteModeSystem에 선택된 게임 Mode와 진행 초기화를 요청한다.
- StageSystem에 Stage 시작을 요청한다.
- StageSystem의 Stage 종료 이벤트를 수신한다.
- Stage 종료 이벤트를 기준으로 게임 종료 절차를 시작한다.

---

# Action Map 관리

GameSystem은 현재 게임 상태에 따라 어떤 Action Map을 사용할지 결정한다.

GameSystem은 Action Map을 직접 관리하지 않는다.

GameSystem은 PlayerInputSystem과 UIInputSystem에 Action Map 상태 변경을 요청한다.

Player Action Map은 PlayerInputSystem이 관리한다.

UI Action Map은 UIInputSystem이 관리한다.

Playing 상태에서는 Player Action Map과 UI Action Map을 활성화한다.

Paused 상태에서는 Player Action Map을 비활성화하고 UI Action Map을 활성화한다.

Ended 상태에서는 Player Action Map을 비활성화하고 UI Action Map을 활성화한다.

GameSystem은 Playing 상태에서 UI Cancel 입력만 Pause 요청으로 해석한다.

GameSystem은 Paused 상태에서 UI 입력을 PausePanel 조작으로 해석한다.

상태가 변경되는 프레임의 transient 입력은 다음 상태의 동작으로 이어지지 않도록 소비한다.

---

# 시작 조건

- Unity가 게임 실행을 시작한다.

---

# 종료 조건

## 정상 종료

- 게임 종료 절차가 완료된다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- 게임 실행 상태
- 게임 시작 상태
- 게임 종료 상태
- 현재 게임 상태
- 선택된 게임 Mode
- 현재 입력 상태 정책
- PausePanel 실행 요청 상태

---

# 입력

| 입력 | 출처 |
|------|------|
| 게임 실행 | Unity |
| 시작 게임 Mode 설정 | Unity Inspector |
| Stage 종료 이벤트 | StageSystem |
| 현재 UI 입력 상태 | UIInputSystem |
| 현재 선택된 UI 항목 | UIManagementSystem |
| Pause와 Resume 요청 | UIInputSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| Runtime Data 생성 요청 | RuntimeDataSystem |
| 현재 게임 Mode | RuntimeDataSystem |
| System 초기화 요청 | 관련 System |
| 현재 게임 Mode와 Stage 시작 요청 | StageSystem |
| Player Action Map 활성화 요청 | PlayerInputSystem |
| Player Action Map 비활성화 요청 | PlayerInputSystem |
| UI Action Map 활성화 요청 | UIInputSystem |
| UI Action Map 비활성화 요청 | UIInputSystem |
| UI 선택 상태 변경 요청 | UIManagementSystem |
| 관련 System 중단 및 재개 요청 | 관련 System |
| Stage 재시작 요청 | 관련 System |
| Application 종료 요청 | Unity |
| 게임 종료 절차 시작 | ResultSystem |
| 게임 종료 절차 시작 | UIManagementSystem |
| Runtime Data 제거 요청 | RuntimeDataSystem |

---

# 시스템 경계

## 담당 범위

- 게임 시작 관리
- 게임 종료 관리
- 현재 게임 상태 관리
- 선택된 게임 Mode 관리
- Retry에서 게임 Mode 유지
- Action Map 사용 여부 결정
- 게임 전체 실행 순서 관리
- System 간 실행 흐름 연결
- 현재 게임 상태에 따른 UI 입력 의미 판단
- 선택된 UI 항목에 해당하는 실행 흐름 시작
- Pause와 Resume 실행 순서 관리
- Application 종료 요청 지점 관리

## 담당하지 않는 범위

- Player Action Map 직접 관리
- UI Action Map 직접 관리
- 플레이어 입력 수집
- UI 입력 수집
- UI 선택 상태 직접 관리
- 플레이어 이동
- 점프 처리
- 관성 착지 처리
- 충돌 판정
- Stage 진행
- Stage 종료 조건 판단
- 결과 데이터 생성
- UI 표시
- Runtime Data 관리
- 게임 Mode별 Feature 규칙 정의

---

# 관련 System

- RuntimeDataSystem
- StageSystem
- ResultSystem
- UIManagementSystem
- PlayerInputSystem
- UIInputSystem
- InfiniteModeSystem

---

# 제약 사항

- Feature의 규칙을 수행하지 않는다.
- 기본 게임 Mode는 Stage Mode를 사용한다.
- 게임 시작 시 선택된 게임 Mode를 Runtime Data에 반영한다.
- Retry 시 선택된 게임 Mode를 유지하고 이전 Stage Play의 Runtime 상태는 유지하지 않는다.
- 다른 System의 내부 상태를 직접 변경하지 않는다.
- Runtime Data를 직접 생성하거나 제거하지 않는다.
- Action Map을 직접 활성화하거나 비활성화하지 않는다.
- 어떤 Action Map을 사용할지만 결정한다.
- Action Map 상태 변경은 담당 InputSystem에 요청한다.
- UI 입력은 현재 게임 상태에서 허용된 실행 흐름으로만 해석한다.
- Playing에서만 Pause를 시작하고 Paused에서만 Resume을 수행한다.
- Initializing, Ready, Ending과 Ended에서는 Pause 요청을 수행하지 않는다.
- Pause와 Stage 종료가 동시에 확정되는 경우 Stage 종료를 우선한다.
- Pause 상태 전환은 `E_GameState.Paused`로 표현하고 별도의 Pause bool을 관리하지 않는다.
- 전역 시간 배율을 변경하지 않는다.
- Application 종료 요청은 하나의 실행 경로에서 처리한다.
- UI 선택 상태 변경과 조회는 UIManagementSystem을 사용한다.
- Stage 종료 여부는 StageSystem이 전달한 종료 이벤트만 사용한다.
- 게임 종료 절차의 실행 순서만 관리한다.

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
