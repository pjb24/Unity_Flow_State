# 기능 개요

## 기능명

GameEntryNavigation

---

## 목적

플레이어가 Main Menu에서 Infinite 플레이와 구현된 기능 화면으로 일관되게 이동할 수 있도록 한다.

---

## Roadmap 9 적용 상태

상태: Phase 2의 Infinite 단일 시작/안내·과거 값 정규화·Stage 실행/표시 차단·Pause Retry 확인은 완료했다. 실행 근거는 [Phase 2 Task](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md)를 따른다. 기록 경계는 [Phase 3 Task](../90_Tasks/Prototype_9/20261117_01_Phase3ManualSteps.md)에서 완료했으며 Phase 4의 사용자 한정 QA 후보 판정도 완료했다. 최신 Player 정상 보고와 검증 한계는 [Phase 4 Task](../90_Tasks/Prototype_9/20261118_01_Phase4ManualSteps.md)를 따른다.

- Boot 뒤 Main Menu/Play를 유지한다. Play는 Mode Select를 거치지 않고 Infinite 시작을 요청한다.
- 기존 Tutorial 완료 상태를 보존한다. 미완료 사용자는 Infinite 자동 안내의 Start Run 전까지 Run을 만들지 않는다.
- 과거 선택/대기 값은 Infinite로 정규화하고 명시적 Stage 선택/실행 요청은 거절한다. 일반 사용자 전환 팝업은 추가하지 않는다.
- Main Menu 복귀 선택과 중복 요청 방지, Pause/Settings 복귀는 유지한다.
- 2026-11-17 Player Candidate02 확인 중 사용자 요청으로 Pause Retry 확인 단계를 추가했다. PauseRetryConfirmation은 Cancel 기본 선택이며 취소 시 같은 Run의 Pause/Retry로 복귀한다. 확인 시에만 Infinite 초기화를 요청한다. 기존 Main Menu 확인 UI를 Scene에서 재사용하며 최종 Scene/Test/Player 검증은 Phase 2 Task 9-3/Step 10에서 완료했다.

검증 계약: P9-C01/P9-C02/P9-C03. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

## 기능 규칙

- Boot가 완료되면 Run 없이 Main Menu를 표시한다.
- Main Menu 항목 순서는 Play, How To Play, Leaderboard, Settings, Quit이다.
- Main Menu의 최초 기본 선택은 Play다.
- Main Menu의 Cancel은 동작하지 않는다.
- Play는 Mode Select를 열지 않는다. Tutorial 완료값이 true면 Infinite 초기화를 요청하고, false면 Infinite 자동 안내의 Start Run까지 Run 생성 요청을 보류한다.
- Navigation 선택/대기 기본값은 Infinite다. 과거 Stage 선택/대기 값은 정상 Play·Start Run·Retry에서 Infinite로 처리한다.
- 명시적 Stage 플레이 선택과 미정의 Navigation 항목은 거절한다. 보존된 Mode Select의 Stage Submit도 실행하지 않는다. 해당 화면의 Back/Cancel은 Main Menu의 Play로 복귀한다.
- Main Menu와 세로 메뉴의 Navigate 경계는 clamp다. 첫 항목에서 위로, 마지막 항목에서 아래로 Navigate해도 선택은 유지한다.
- Main Menu에서 How To Play, Leaderboard 또는 Settings를 열었다가 Back 또는 Cancel로 복귀하면 해당 Main Menu 항목을 다시 선택한다.
- Main Menu의 Quit만 Application 종료를 요청한다.
- 이전 화면으로 돌아갈 수 있는 모든 하위 화면은 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있는 Back UI 항목을 제공한다.
- 화면 선택은 Application 실행 중 Runtime Navigation 상태로 유지하며, 선택 Mode를 영구 저장하지 않는다. Tutorial 완료 여부는 기존 Local Save 계약으로 복원·보존한다.

---

## 시작 조건

- Boot가 완료되었다.
- Pause 또는 Result에서 Main Menu 복귀가 완료되었다.

---

## 종료 조건

## 정상 종료

- Play, How To Play, Leaderboard, Settings 또는 Quit 요청이 처리되었다.

## 강제 종료

- Application이 종료된다.

---

## 수행 결과

- Main Menu와 Infinite 자동 안내에서 Run 생성 전의 Navigation을 제공한다.
- 유효한 Play/Start Run/Retry는 Infinite 시작 요청을 만들고 초기화 성공 때만 Run ID를 증가시킨다.

---

## 예외 사항

- 유효하지 않은 Mode Submit과 중복 시작 요청은 현재 화면과 선택을 변경하지 않는다.
- Run이 없는 화면에서 Run Runtime Data를 생성하지 않는다.
- 자동 안내 중 Cancel 입력은 현재 화면과 선택을 변경하지 않는다.

---

## 관련 System

- GameSystem
- UIManagementSystem
- UIInputSystem

---

## 제약 사항

- Phase 2의 Scene 적용·화면/포커스 차단과 Unity 검증은 완료했다. 최신 Player 정상 보고를 포함한 Phase 4 QA 후보 판정도 완료했다.
- Leaderboard는 Infinite 선택만 제공하며 Stage 탭/직접 선택은 거절한다. 실제 조회/제출 정책은 Leaderboard/RecordSubmission Feature에서 관리한다.
- Leaderboard 조회·제출과 Settings의 세부 기능은 이 Feature가 수행하지 않는다.

---

## 검증 항목

- Boot 후 Main Menu가 Run 없이 표시되는지 확인한다.
- Main Menu의 기본 선택, clamp 경계, Back UI와 Cancel 복귀를 확인한다.
- Tutorial 미완료 Play는 안내만 열고, 완료 Play는 Mode Select 없이 Infinite 초기화를 요청하는지 확인한다.
- Start Run의 같은/인접 입력 sequence·중복 호출이 Run을 한 번만 생성하는지 확인한다.
- 과거 Stage 선택/대기/Retry 정규화와 명시적 Stage 거절, 초기화 실패 후 복귀/재진입 및 Tutorial 완료 보존을 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
