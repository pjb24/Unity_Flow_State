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
- Settings는 Runtime Master Volume 한 항목을 제공한다. 기본값은 100%이고 값 범위는 0%부터 100%까지다.
- Settings는 시작 시점의 Fullscreen 상태를 사용하며 Fullscreen On/Off 한 항목을 제공한다.
- Settings는 Player Jump와 Momentum Landing, UI Navigate의 Keyboard Binding만 재지정한다.
- Player Move는 자동 이동 구조에서 플레이 입력으로 사용하지 않으므로 Settings의 재지정 대상에 포함하지 않는다.
- UI Navigate의 기본 Keyboard Binding은 WASD 4방향만 사용한다. 방향키 보조 Binding은 제공하지 않는다.
- UI Submit과 Cancel은 Settings 재지정 대상에 포함하지 않으며 기존 기본 Binding을 유지한다. UI 접근과 복구 경로를 보장하기 위한 고정 안전 입력이다.
- Gamepad의 기본 Binding은 유지하며 Settings에서 재지정하지 않는다.
- 같은 Action Map과 같은 장치 그룹에서 재지정 대상 Binding이 중복되면 새 Binding을 거부하고 기존 Binding을 유지한다. Player와 UI Action Map 사이의 같은 Keyboard Binding은 허용한다.
- Rebinding 대기 중 UI Cancel 또는 Rebinding 전용 취소 요청이 발생하면 Override를 적용하지 않고 이전 표시와 Focus를 복귀한다.
- Restore Defaults는 확인 후 모든 Phase 3 Settings 값과 Binding Override를 Application 시작 기본값으로 복원한다.

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
- 영구 저장은 Roadmap 7 범위에서 정의한다.

---

## 관련 System

- GameSystem
- UIManagementSystem
- UIInputSystem

---

## 제약 사항

- Settings 값은 영구 저장하지 않는다.
- Settings 값과 Binding Override는 Application 실행 중에만 유지한다.

---

## 검증 항목

- Main Menu와 Pause가 같은 Settings 화면으로 진입하는지 확인한다.
- 각 진입 출처로 정확히 복귀하는지 확인한다.
- Pause에서 Settings를 열고 닫아도 Run과 Paused 상태가 보존되는지 확인한다.
- Master Volume 범위, Fullscreen 적용, 재지정 대상과 UI Navigate의 WASD 기본 Binding을 확인한다.
- 같은 Action Map과 장치 그룹의 Binding 충돌 거부, 취소, 전체 기본값 복원을 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
