# 기능 개요

## 기능명

HowToPlay

---

## 목적

플레이어가 조작법과 핵심 게임 규칙을 확인할 수 있도록 한다.

---

## Roadmap 9 적용 상태

상태: Phase 2의 Infinite 자동 안내 진입/시작·Tutorial 완료값 보존·UI 연결/문구는 적용·검증을 완료했다. 실행/화면 확인 근거는 [Phase 2 Task](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md)를 따른다.

- 자동 안내는 저장된 Tutorial 미완료 사용자의 Infinite 첫 시작 전에만 표시한다. 이미 완료한 사용자의 상태를 초기화하지 않는다.
- 안내는 자동 이동·점프·관성 착지·Collectible·거리/점수·Pause·Infinite 종료 규칙을 설명하고 Stage Goal/Clear Time 설명은 제공하지 않는다.
- Main Menu 재열람·Binding 표시·자동 안내 Cancel 무동작·Start Run 한 번 처리와 완료 상태 보존을 유지한다.

검증 계약: P9-C01/P9-C03. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

## 기능 규칙

- How To Play는 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있는 Back UI 항목을 제공한다.
- Main Menu에서 How To Play를 열면 Back UI Submit 또는 Cancel은 Main Menu의 How To Play 선택으로 복귀한다.
- Main Menu 재열람은 Back을 기본 선택으로 사용하며, 자동 안내 처리 상태에 영향을 주지 않는다.
- Local Save에서 복원한 Tutorial 완료 상태가 미완료이면 Play가 Infinite Run 시작 전 단일 페이지 How To Play를 자동으로 표시하고 Run을 생성하지 않는다. 완료 상태이면 Play가 안내 없이 Infinite 시작을 요청한다.
- 자동 안내는 Start Run만 기본 선택으로 제공한다. Start Run은 Infinite Run을 요청하고 자동 안내 처리 완료로 기록한다. 과거 대기 Stage 값도 Infinite로 처리한다.
- 자동 안내에는 Back 또는 Skip UI를 제공하지 않으며 Cancel 입력은 동작하지 않는다.
- 자동 안내 처리 완료 뒤에는 같은 Application 실행 중 다시 자동으로 표시하지 않는다.
- 자동 안내에는 마지막 Keyboard 또는 Mouse 입력 뒤 Keyboard Binding을, 마지막 Gamepad 입력 뒤 Gamepad Binding을 표시한다. Pause는 Keyboard/Mouse에서 Escape, Gamepad에서 Cancel Button의 구체적 Binding을 표시한다.
- Infinite 전용 안내는 자동 이동, Jump, 착지 직전 Momentum Landing과 연속 성공의 거리 점수 배율, Collectible/거리/Total Score, Pause/Retry, 추락·전진 지속 실패에 따른 종료를 설명한다. Stage Goal/Clear Time 설명은 제공하지 않는다.

---

## 시작 조건

- Main Menu에서 How To Play 요청이 수행되었다.
- 복원한 Tutorial 미완료 상태에서 Play 요청이 수행되었다.

---

## 종료 조건

## 정상 종료

- Main Menu 재열람의 Back 또는 Cancel 요청이 처리되었다.
- 자동 안내의 Start Run 요청이 처리되었다.

## 강제 종료

- Application이 종료된다.

---

## 수행 결과

- Main Menu에서 연 화면은 Main Menu로 복귀한다.
- 자동 안내 중에는 Start Run 요청 전까지 Run을 시작하지 않는다.

---

## 예외 사항

- Tutorial 완료 상태는 Local Save에서 복원하며 일반 저장 흐름으로 보존한다. 저장이 실패하면 다음 실행의 복원 성공을 보장하지 않는다.
- Start Run 뒤 Run 초기화에 실패해도 자동 안내 처리 완료 상태는 유지하고 기존 실패 복귀 경로를 사용한다.

---

## 관련 System

- GameSystem
- UIManagementSystem
- UIInputSystem

---

## 제약 사항

- Infinite 자동 안내 진입은 생산 코드에 연결돼 있다. 현재 저장된 Scene에는 기존 Stage 목표 문구가 남아 있으며 [Phase 2 Task](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md)의 Step 7-2 최종 Text를 사용자가 적용한다. 준비 문구를 이미 적용된 Scene으로 취급하지 않는다.
- 안내 완료 상태를 현재 Runtime Data에 영구 저장하지 않는다.

---

## 검증 항목

- Main Menu의 How To Play 진입과 복귀 선택을 확인한다.
- 자동 안내 중 Run 생성 요청이 없는지 확인한다.
- 자동 안내의 Start Run만 Run을 요청하고 Cancel은 무반응인지 확인한다.
- Keyboard·Mouse와 Gamepad의 마지막 입력에 따라 현재 Binding 표기가 전환되는지 확인한다.
- 영구 저장 도입 후 단말기별 자동 표시 1회 규칙을 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
