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
- 세부 안내가 아직 구현되지 않은 경우 Back을 기본 선택으로 사용한다.
- 단말기 최초 실행 시 Stage 또는 Infinite Run 시작 전 How To Play를 자동으로 표시하고 게임 진행을 시작하지 않는다.
- 단말기에서 How To Play 자동 표시가 완료된 뒤에는 다시 자동으로 표시하지 않는다.

---

## 시작 조건

- Main Menu에서 How To Play 요청이 수행되었다.
- 단말기 최초 실행의 자동 안내 조건이 충족되었다.

---

## 종료 조건

## 정상 종료

- Back, Cancel 또는 안내 완료 요청이 처리되었다.

## 강제 종료

- Application이 종료된다.

---

## 수행 결과

- Main Menu에서 연 화면은 Main Menu로 복귀한다.
- 자동 안내 중에는 Run을 시작하지 않는다.

---

## 예외 사항

- 단말기별 자동 표시 완료 상태의 영구 저장은 Roadmap 7에서 수행한다.
- 영구 저장이 도입되기 전에는 Application 종료 후 자동 표시 완료 여부를 보장하지 않는다.

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
- 영구 저장 도입 후 단말기별 자동 표시 1회 규칙을 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
