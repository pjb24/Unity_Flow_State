# 기능 개요

## 기능명

ResultMenu

---

## 목적

일반 Stage의 결과 화면에서 다음 실행 흐름을 키보드와 마우스로 선택할 수 있도록 한다.

반복 플레이와 Application 종료를 결과 화면에서 수행할 수 있도록 한다.

---

# 기능 규칙

- ResultMenu는 일반 Stage 결과 화면에서만 활성화한다.
- ResultMenu에는 Retry와 Quit 항목이 존재한다.
- ResultMenu가 활성화되면 Retry를 기본 선택 항목으로 사용한다.
- Navigate 입력은 Retry와 Quit 사이의 선택을 변경한다.
- Submit 입력은 현재 선택된 항목을 한 번 실행한다.
- Retry가 실행되면 새로운 Stage Play를 시작한다.
- Quit이 실행되면 Application 종료를 요청한다.
- Cancel 입력은 ResultMenu에서 동작을 수행하지 않는다.
- Point 입력은 마우스 포인터가 가리키는 Retry 또는 Quit을 선택한다.
- Click 입력은 마우스 포인터가 가리키는 Retry 또는 Quit을 한 번 실행한다.
- StageHUD와 ResultMenu는 동시에 표시하지 않는다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 ResultMenu를 시작한다.

- 일반 Stage Play가 종료되었다.
- 결과 데이터가 확정되었다.
- Result UI State가 활성화되었다.

---

# 종료 조건

## 정상 종료

- Retry가 실행되어 새로운 Stage Play가 시작된다.
- Quit이 실행되어 Application 종료 절차가 시작된다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 수행 결과

- Retry 실행 시 이전 Stage Play의 Runtime 상태를 사용하지 않는 새로운 Stage Play가 시작된다.
- Quit 실행 시 Application 종료가 요청된다.

---

# 예외 사항

- Stage Play 진행 중에는 ResultMenu 입력을 처리하지 않는다.
- Result UI State가 아니면 Navigate, Submit, Cancel, Point와 Click을 ResultMenu 동작으로 처리하지 않는다.
- Submit 입력 하나로 선택된 항목을 두 번 이상 실행하지 않는다.
- Click 입력 하나로 선택된 항목을 두 번 이상 실행하지 않는다.
- Cancel 입력으로 Retry 또는 Quit을 실행하지 않는다.

---

# 관련 System

- GameSystem
- UIInputSystem
- UIManagementSystem

---

# 제약 사항

- Phase 5 완료 검증의 필수 입력 장치는 키보드와 마우스이다.
- 게임패드 동작은 Phase 5 완료 조건에 포함하지 않는다.
- ResultMenu는 GamePause를 수행하지 않는다.
- ResultMenu는 InfiniteMode, ScoreRecord, Leaderboard 또는 저장 기능을 수행하지 않는다.
- Retry는 이전 플레이의 Timer, Result Data와 입력 상태를 새로운 플레이에 유지하지 않는다.

---

# 검증 항목

- Result UI State에서 Retry가 기본 선택되는지 확인한다.
- 키보드 Navigate 입력으로 Retry와 Quit 선택이 변경되는지 확인한다.
- 키보드 Submit 입력으로 현재 선택된 항목이 한 번만 실행되는지 확인한다.
- 키보드 Cancel 입력이 Retry 또는 Quit을 실행하지 않는지 확인한다.
- 마우스 Point 입력으로 Retry와 Quit을 선택할 수 있는지 확인한다.
- 마우스 Click 입력으로 Retry와 Quit이 각각 한 번만 실행되는지 확인한다.
- Retry 실행 후 새로운 Stage Play가 이전 Runtime 상태 없이 시작되는지 확인한다.
- Quit 실행 시 Application 종료가 요청되는지 Build에서 확인한다.
- Stage Play 진행 중 ResultMenu 입력이 처리되지 않는지 확인한다.
- StageHUD와 ResultMenu가 동시에 표시되지 않는지 확인한다.

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
