# 기능 개요

## 기능명

GameEntryNavigation

---

## 목적

플레이어가 Main Menu에서 게임 Mode와 구현된 기능 화면으로 일관되게 이동할 수 있도록 한다.

---

## 기능 규칙

- Boot가 완료되면 Run 없이 Main Menu를 표시한다.
- Main Menu 항목 순서는 Play, How To Play, Leaderboard, Settings, Quit이다.
- Main Menu의 최초 기본 선택은 Play다.
- Main Menu의 Cancel은 동작하지 않는다.
- Mode Select 항목 순서는 Stage, Infinite, Back이다.
- Mode Select는 최초에 Stage를 선택하고, 같은 Application 실행 중 다시 열면 마지막으로 선택한 Mode를 기본 선택으로 사용한다.
- Mode Select에서 유효한 Mode를 Submit하면 별도 Start 확인 없이 해당 Mode의 Run 시작을 요청한다.
- Mode Select의 Back Submit 또는 Cancel은 Main Menu의 Play 선택으로 복귀한다.
- Main Menu, Mode Select와 모든 세로 메뉴의 Navigate 경계는 clamp다. 첫 항목에서 위로, 마지막 항목에서 아래로 Navigate해도 선택은 유지한다.
- Main Menu에서 How To Play, Leaderboard 또는 Settings를 열었다가 Back 또는 Cancel로 복귀하면 해당 Main Menu 항목을 다시 선택한다.
- Main Menu의 Quit만 Application 종료를 요청한다.
- 이전 화면으로 돌아갈 수 있는 모든 하위 화면은 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있는 Back UI 항목을 제공한다.
- 화면 선택과 마지막 Mode 기억은 Application 실행 중 Runtime Navigation 상태로만 유지하며, Application 종료 후 저장하거나 복원하지 않는다.

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

- Main Menu와 Mode Select에서 Run 생성 전의 Navigation을 제공한다.
- 선택된 Mode의 Submit 요청이 성공한 경우에만 Run 시작 흐름으로 진행한다.

---

## 예외 사항

- 유효하지 않은 Mode Submit과 중복 시작 요청은 현재 화면과 선택을 변경하지 않는다.
- Run이 없는 화면에서 Run Runtime Data를 생성하지 않는다.

---

## 관련 System

- GameSystem
- UIManagementSystem
- UIInputSystem

---

## 제약 사항

- 실제 Scene UI와 Action Map 생산 연결은 Phase 2에서 수행한다.
- Leaderboard 조회·제출과 Settings의 세부 기능은 이 Feature가 수행하지 않는다.

---

## 검증 항목

- Boot 후 Main Menu가 Run 없이 표시되는지 확인한다.
- Main Menu와 Mode Select의 기본 선택, clamp 경계, Back UI와 Cancel 복귀를 확인한다.
- Mode Select Submit 전에는 Run 생성 요청이 없는지 확인한다.
- 마지막 Mode와 화면 선택이 Application 실행 중에만 복귀되는지 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
