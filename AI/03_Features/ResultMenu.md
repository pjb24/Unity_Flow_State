# 기능 개요

## 기능명

ResultMenu

---

## 목적

Stage Mode와 InfiniteMode의 결과 화면에서 다음 실행 흐름을 키보드와 마우스로 선택할 수 있도록 한다.

반복 플레이와 Main Menu 복귀를 결과 화면에서 수행할 수 있도록 한다.

---

# 기능 규칙

- ResultMenu는 Stage Mode와 InfiniteMode의 결과 화면에서 활성화한다.
- ResultMenu에는 Retry, Main Menu와 `순위 보기` 항목이 존재한다. Pending 후보에는 별도의 제출 Retry를 제공한다.
- ResultMenu가 활성화되면 Retry를 기본 선택 항목으로 사용한다.
- Navigate 입력은 표시된 항목 사이의 선택을 변경하며, 목록 경계에서는 현재 선택을 유지한다.
- Submit 입력은 현재 선택된 항목을 한 번 실행한다.
- Retry가 실행되면 종료된 Stage Play와 같은 게임 Mode로 새로운 Stage Play를 시작한다.
- Main Menu가 실행되면 Result 표시를 종료하고 Main Menu로 이동한다.
- Cancel 입력은 ResultMenu에서 동작을 수행하지 않는다.
- Point 입력은 마우스 포인터가 가리키는 항목을 선택한다.
- Click 입력은 마우스 포인터가 가리키는 항목을 한 번 실행한다.
- `순위 보기`는 Leaderboard Feature에 현재 Mode·Stage·Version의 조회 화면 진입을 요청한다. Result 진입만으로 순위 화면을 자동으로 열지 않는다. Back/Cancel은 Result와 기존 선택으로 복귀한다.
- `New local best`는 이번 Run이 로컬 영구 최고 기록을 갱신한 경우에만 표시한다.
- 서버 조회에 성공한 해당 계정의 최고 기록과 순위만 `Online best`로 표시한다. 조회 전·실패 시에는 확정값으로 표시하지 않는다. 로컬 최고 및 이번 Run 제출 상태와 구분한다.
- `Online best` 값은 Leaderboard와 동일하게 Stage 정수 밀리초를 `0.016 s` 형식으로, Infinite 정수 점수를 `1,234` 형식으로 표시한다. `View Leaderboard`는 마지막으로 선택했던 Tab이 아니라 실제 종료한 Run의 Mode Board를 연다.
- 온라인·동의 상태의 Run 종료 시 별도 로딩 UI와 실제 시간 기준 남은 대기시간(`8s → 7s → …`)을 표시하며 현재 제출 ID를 자동 시도한다. 제출과 개인 최고 조회가 8초 이내에 끝나면 즉시 Result를 열고, 8초를 넘으면 현재까지 확인된 상태로 Result를 열어 늦은 결과를 반영한다. Result가 열릴 때 로딩 UI를 닫는다. Offline·미동의 상태에서는 온라인 응답을 기다리지 않고 Pending Result를 연다.
- 제출 상태·거부 사유·제출 Retry 조건은 RecordSubmission 규칙을 따른다. Run Retry와 제출 Retry는 서로 다른 동작이다.
- ResultMenu는 현재 게임 Mode의 HUD와 함께 표시한다.
- Stage Mode에서는 StageHUD와 Stage Result Content를 표시한다.
- InfiniteMode에서는 InfiniteHUD와 InfiniteMode Result Content를 표시한다.
- Stage Result Content는 성공 또는 실패 Status, Clear Time 또는 Run Time과 Collectible Score를 표시한다.
- InfiniteMode Result Content는 Final Distance, Distance Score, Collectible Score와 Total Score를 표시한다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 ResultMenu를 시작한다.

- Stage Mode 또는 InfiniteMode의 Stage Play가 종료되었다.
- Stage Mode에서는 결과 데이터가 확정되었다.
- Result UI State가 활성화되었다.

---

# 종료 조건

## 정상 종료

- Retry가 실행되어 새로운 Stage Play가 시작된다.
- Main Menu가 실행되어 Result 표시가 종료된다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 수행 결과

- Retry 실행 시 이전 Stage Play의 Runtime 상태를 사용하지 않는 같은 게임 Mode의 새로운 Stage Play가 시작된다.
- Main Menu 실행 시 Run을 유지하지 않고 Main Menu로 이동한다.

---

# 예외 사항

- Stage Play 진행 중에는 ResultMenu 입력을 처리하지 않는다.
- Result UI State가 아니면 Navigate, Submit, Cancel, Point와 Click을 ResultMenu 동작으로 처리하지 않는다.
- Submit 입력 하나로 선택된 항목을 두 번 이상 실행하지 않는다.
- Click 입력 하나로 선택된 항목을 두 번 이상 실행하지 않는다.
- Cancel 입력으로 Retry 또는 Main Menu를 실행하지 않는다.

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
- ResultMenu는 ScoreRecord, 저장 또는 Application 종료를 수행하지 않는다. 순위 조회는 Leaderboard에, 제출 재시도는 RecordSubmission에 요청하며 직접 기록이나 순위를 계산하지 않는다.
- Retry는 이전 플레이의 Timer, Result Data와 입력 상태를 새로운 플레이에 유지하지 않는다.
- ResultPanel은 현재 Mode의 HUD보다 앞에 표시한다.
- Stage Result Content와 InfiniteMode Result Content를 동시에 표시하지 않는다.

---

# 검증 항목

- Result UI State에서 Retry가 기본 선택되는지 확인한다.
- 키보드 Navigate 입력으로 Retry와 Main Menu 선택이 변경되는지 확인한다.
- 키보드 Submit 입력으로 현재 선택된 항목이 한 번만 실행되는지 확인한다.
- 키보드 Cancel 입력이 Retry 또는 Main Menu를 실행하지 않는지 확인한다.
- 마우스 Point 입력으로 Retry와 Main Menu를 선택할 수 있는지 확인한다.
- 마우스 Click 입력으로 Retry와 Main Menu가 각각 한 번만 실행되는지 확인한다.
- Stage Mode와 InfiniteMode에서 ResultMenu가 활성화되는지 확인한다.
- Retry 실행 후 같은 게임 Mode의 새로운 Stage Play가 이전 Runtime 상태 없이 시작되는지 확인한다.
- Stage Mode Result에서 StageHUD와 Stage Result Content가 함께 표시되는지 확인한다.
- InfiniteMode Result에서 InfiniteHUD, Final Distance, Distance Score, Collectible Score와 Total Score가 함께 표시되는지 확인한다.
- Main Menu 실행 시 Run을 유지하지 않고 Main Menu로 이동하는지 확인한다.
- Stage Play 진행 중 ResultMenu 입력이 처리되지 않는지 확인한다.
- 현재 Mode가 아닌 HUD와 Result Content가 표시되지 않는지 확인한다.
- 순위 보기의 Board·진입 출처와 Result 복귀 선택이 보존되는지 확인한다.
- 로컬 새 최고와 조회된 Online best·순위·제출 상태를 구분하고, Rejected에는 사유만 표시하며 Pending에만 제출 Retry가 제공되는지 확인한다.

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
