# 기능 개요

## 기능명

Settings

---

## 목적

플레이어가 Main Menu와 Pause에서 동일한 Settings 화면에 접근하도록 한다.

---

## 기능 규칙

- Main Menu와 Pause는 동일한 Settings 화면을 사용한다.
- Settings는 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있는 Back UI 항목을 제공한다.
- Main Menu에서 Settings를 열면 Back UI Submit 또는 Cancel은 Main Menu의 Settings 선택으로 복귀한다.
- Pause에서 Settings를 열면 Run과 Paused 상태를 유지한다.
- Pause에서 연 Settings의 Back UI Submit 또는 Cancel은 PausePanel의 Settings 선택으로 복귀한다.
- Pause에서 Settings를 여는 동안 Player, Stage, Timer와 InfiniteMode 진행은 재개하지 않는다.
- Settings 세부 항목이 아직 구현되지 않은 경우 Back을 기본 선택으로 사용한다.

---

## 시작 조건

- Main Menu 또는 Pause에서 Settings 요청이 수행되었다.

---

## 종료 조건

## 정상 종료

- Back 또는 Cancel로 진입 화면에 복귀한다.

## 강제 종료

- Application이 종료된다.

---

## 수행 결과

- 진입 출처에 맞게 Navigation을 복귀한다.
- Pause에서 진입한 경우 기존 Run을 변경하지 않는다.

---

## 예외 사항

- Result에서는 Settings를 열지 않는다.
- 저장과 Input Rebinding은 Phase 3 및 Roadmap 7 범위에서 정의한다.

---

## 관련 System

- GameSystem
- UIManagementSystem
- UIInputSystem

---

## 제약 사항

- Settings 값은 영구 저장하지 않는다.
- 실제 Settings UI와 세부 설정 항목은 Phase 3에서 구현한다.

---

## 검증 항목

- Main Menu와 Pause가 같은 Settings 화면으로 진입하는지 확인한다.
- 각 진입 출처로 정확히 복귀하는지 확인한다.
- Pause에서 Settings를 열고 닫아도 Run과 Paused 상태가 보존되는지 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
