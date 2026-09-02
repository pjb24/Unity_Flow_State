# 기능 개요

## 기능명

GamePause

---

## 목적

플레이어가 Stage Play를 일시적으로 중단할 수 있도록 한다.

일시정지 중에는 현재 Stage Play의 진행 상태를 유지한다.

---

# 기능 규칙

- GamePause는 Stage Mode 또는 InfiniteMode의 Stage Play가 Playing 상태일 때만 수행한다.
- 키보드의 기본 Pause와 Resume 입력은 `Escape`를 사용한다.
- Playing 상태의 Pause 입력은 GamePause를 시작한다.
- Pause 상태의 동일한 입력은 Resume 요청으로 해석한다.
- GamePause가 시작되면 Stage Play를 일시 중단한다.
- GamePause 동안에는 플레이어를 조작할 수 없다.
- GamePause 동안에는 Stage Play가 진행되지 않는다.
- GamePause 동안에는 Stage Mode의 PlayTimer가 증가하지 않는다.
- GamePause 동안에는 InfiniteMode의 이동 거리와 Score가 증가하지 않는다.
- GamePause 동안에는 InfiniteMode의 진행 지속 시간, 추락 종료 판정과 Map Pattern 재배치가 진행되지 않는다.
- GamePause 동안에는 PausePanel의 Navigate, Submit, Cancel, Point와 Click 입력만 허용한다.
- PausePanel에는 Resume, Retry와 Quit 항목이 존재한다.
- PausePanel이 활성화되면 Resume을 기본 선택 항목으로 사용한다.
- Navigate 입력은 Resume, Retry와 Quit 사이의 선택을 변경한다.
- Submit 입력은 현재 선택된 항목을 한 번 실행한다.
- Point 입력은 마우스 포인터가 가리키는 항목을 선택한다.
- Click 입력은 마우스 포인터가 가리키는 항목을 한 번 실행한다.
- Cancel 입력은 현재 선택과 관계없이 Resume을 한 번 실행한다.
- Resume은 Pause 직전의 같은 Mode와 Run을 유지한다.
- Retry는 현재 Play 정보를 제거하고 같은 게임 Mode의 새로운 Stage Play를 시작한다.
- Quit은 현재 실행 환경에 맞는 Application 종료를 요청한다.
- PausePanel의 한 입력으로 둘 이상의 항목을 실행하지 않는다.
- GamePause 동안 발생한 플레이 입력은 Resume 후 플레이에 반영하지 않는다.
- GamePause가 종료되면 일시정지 이전의 Stage Play를 이어서 진행한다.
- GamePause가 종료되면 Pause 직전의 Player 위치, 속도와 이동 상태를 이어서 사용한다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 GamePause를 시작한다.

- Stage Play가 진행 중이다.
- 플레이어가 일시정지 요청을 수행하였다.

---

# 종료 조건

## 정상 종료

- 플레이어가 게임 재개 요청을 수행하였다.
- 플레이어가 Retry를 수행하여 같은 게임 Mode의 새로운 Stage Play를 시작하였다.
- 플레이어가 Quit을 수행하여 Application 종료 절차가 시작되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- Stage Play가 일시 중단된다.
- GamePause 종료 후 기존 Stage Play를 이어서 진행한다.

---

# 예외 사항

- Stage Play가 진행 중이 아니면 수행하지 않는다.
- GamePause가 이미 수행 중이면 다시 수행하지 않는다.
- GamePause가 수행 중이 아니면 Resume을 수행하지 않는다.
- Initializing, Ready, Ending, Ended와 Result 상태에서는 GamePause를 수행하지 않는다.
- 게임이 종료된 이후에는 수행하지 않는다.

---

# 관련 System

- GameSystem
- PlayerMovementSystem
- TimerSystem
- UIManagementSystem

---

# 제약 사항

- GamePause는 Stage Play 동안만 수행한다.
- 하나의 Stage Play에서는 하나의 GamePause 상태만 유지할 수 있다.
- GamePause는 Stage Play의 진행 상태를 초기화하지 않는다.
- GamePause 종료 후에는 일시정지 이전의 Stage Play를 이어서 진행한다.
- GamePause는 전역 시간 배율을 변경하지 않는다.
- Pause 요청이 Stage 종료와 동시에 발생하면 확정된 Stage 종료를 우선한다.
- Phase 3의 PausePanel은 기능 검증에 필요한 최소 화면만 제공한다.
- PausePanel의 최종 레이아웃, 아트와 애니메이션은 Phase 4에서 수행한다.

---

# 검증 항목

- Stage Play 진행 중 GamePause가 시작되는지 확인한다.
- GamePause 동안 플레이어를 조작할 수 없는지 확인한다.
- GamePause 동안 Stage Play가 진행되지 않는지 확인한다.
- GamePause 동안 클리어 시간이 증가하지 않는지 확인한다.
- GamePause 동안 InfiniteMode의 이동 거리, Score와 종료 판정이 진행되지 않는지 확인한다.
- GamePause 종료 후 Stage Play가 중단된 시점부터 이어서 진행되는지 확인한다.
- GamePause 종료 후 Player 위치, 속도와 이동 상태가 보존되는지 확인한다.
- PausePanel에서 Keyboard와 Mouse로 Resume, Retry와 Quit을 각각 한 번만 수행하는지 확인한다.
- Cancel 입력이 Resume을 수행하는지 확인한다.
- Retry 후 같은 Mode의 새로운 Stage Play가 초기화되어 시작되는지 확인한다.
- Result 상태에서 GamePause가 수행되지 않는지 확인한다.
- Stage Play가 진행 중이 아닐 때 GamePause가 수행되지 않는지 확인한다.
- GamePause가 수행 중일 때 다시 GamePause가 시작되지 않는지 확인한다.

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
