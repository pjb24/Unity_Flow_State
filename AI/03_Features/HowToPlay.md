# 기능 개요

## 기능명

HowToPlay

---

## 목적

플레이어가 조작법과 핵심 게임 규칙을 확인할 수 있도록 한다.

---

## 기능 규칙

- How To Play는 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있는 Back UI 항목을 제공한다.
- Main Menu에서 How To Play를 열면 Back UI Submit 또는 Cancel은 Main Menu의 How To Play 선택으로 복귀한다.
- Main Menu 재열람은 Back을 기본 선택으로 사용하며, 자동 안내 처리 상태에 영향을 주지 않는다.
- Application 실행 중 최초 Stage 또는 Infinite Run 시작 전 공용 단일 페이지 How To Play를 자동으로 표시하고 게임 진행을 시작하지 않는다.
- 자동 안내는 Start Run만 기본 선택으로 제공한다. Start Run은 선택된 Mode의 Run을 시작하고 자동 안내 처리 완료로 기록한다.
- 자동 안내에는 Back 또는 Skip UI를 제공하지 않으며 Cancel 입력은 동작하지 않는다.
- 자동 안내 처리 완료 뒤에는 같은 Application 실행 중 다시 자동으로 표시하지 않는다.
- 자동 안내에는 마지막 Keyboard 또는 Mouse 입력 뒤 Keyboard Binding을, 마지막 Gamepad 입력 뒤 Gamepad Binding을 표시한다. Pause는 Keyboard/Mouse에서 Escape, Gamepad에서 Cancel Button의 구체적 Binding을 표시한다.
- 안내 문구는 자동 이동, Jump, Momentum Landing, Collectible, Pause와 Mode별 목표 순서로 표시한다. Momentum Landing은 착지 직전 입력으로 수행하며 InfiniteMode의 연속 성공이 거리 Score 배율을 높임을 안내한다.

---

## 시작 조건

- Main Menu에서 How To Play 요청이 수행되었다.
- Application 실행 중 자동 안내 처리 전 Mode 선택 요청이 수행되었다.

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

- 단말기별 자동 표시 완료 상태의 영구 저장은 Roadmap 7에서 수행한다.
- 영구 저장이 도입되기 전에는 Application 종료 후 자동 표시 완료 여부를 보장하지 않는다.
- Start Run 뒤 Run 초기화에 실패해도 자동 안내 처리 완료 상태는 유지하고 기존 실패 복귀 경로를 사용한다.

---

## 관련 System

- GameSystem
- UIManagementSystem
- UIInputSystem

---

## 제약 사항

- 실제 안내 내용과 첫 Run 안내 생산 연결은 Phase 4에서 구현한다.
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
