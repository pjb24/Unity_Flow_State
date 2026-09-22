# 시스템 개요

## 시스템명

SettingsSystem

---

## 목적

Application 실행 중 Settings 값과 Binding Override 상태를 관리한다.

Settings 변경을 Audio, Display와 실제 Input Action 인스턴스에 적용한다.

---

# 시스템 책임

- Runtime Master Volume과 Fullscreen 상태를 관리한다.
- Settings 값의 적용과 Application 시작 기본값 복원을 관리한다.
- Rebinding 대기, 완료, 취소와 충돌 거부 상태를 관리한다.
- PlayerInputSystem과 UIInputSystem에 해당 Action Map Binding Override의 적용 또는 제거를 요청한다.
- Audio와 Display 플랫폼 API 적용 경계를 관리한다.

---

# 시작 조건

- Application이 SettingsSystem 초기화를 요청한다.
- Settings 화면에서 값 변경, Rebinding, 취소 또는 기본값 복원 요청이 수행된다.

---

# 종료 조건

## 정상 종료

- Application이 종료된다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- Runtime Master Volume 값
- Runtime Fullscreen 상태
- Rebinding 진행 상태
- Rebinding 대상과 대기 전 Binding Override 상태
- Application 시작 Settings 기본값

---

# 입력

| 입력 | 출처 |
|------|------|
| Settings 값 변경 요청 | GameSystem |
| Rebinding 요청과 취소 요청 | GameSystem |
| 기본값 복원 요청 | GameSystem |
| Player Binding Override 적용 결과 | PlayerInputSystem |
| UI Binding Override 적용 결과 | UIInputSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| 현재 Settings 값과 Rebinding 상태 | UIManagementSystem |
| Player Binding Override 적용 또는 제거 요청 | PlayerInputSystem |
| UI Binding Override 적용 또는 제거 요청 | UIInputSystem |
| Audio와 Display 적용 요청 | 플랫폼 적용 경계 |

---

# 시스템 경계

## 담당 범위

- Runtime Settings 상태 관리
- Rebinding 상태와 충돌 검증 관리
- Settings 기본값 복원 관리
- Audio와 Display 적용 요청

## 담당하지 않는 범위

- Settings 화면 Navigation 결정
- UI 표시와 Focus 관리
- Player 또는 UI 입력 수집
- Player 이동과 UI 동작 의미 판단
- Settings 값과 Binding Override의 영구 저장
- Leaderboard 상태 판정 또는 조회

---

# 관련 System

- GameSystem
- UIManagementSystem
- PlayerInputSystem
- UIInputSystem

---

# 제약 사항

- Settings 상태는 Runtime에서만 사용하고 Application 종료 후 저장하거나 복원하지 않는다.
- Player Move Binding은 Settings 재지정 또는 복원 대상에 포함하지 않는다.
- UI Navigate의 Keyboard 기본 Binding은 WASD 4방향만 사용한다.
- UI Submit과 Cancel Binding은 Settings 재지정 또는 복원 대상에 포함하지 않는다.
- Gamepad Binding은 유지하지만 Settings 재지정 대상에 포함하지 않는다.
- 같은 Action Map과 같은 장치 그룹의 재지정 대상 Binding 중복은 적용하지 않는다.
- Player와 UI Action Map 사이의 같은 Keyboard Binding은 충돌로 처리하지 않는다.
- Rebinding 취소는 기존 Binding Override를 변경하지 않는다.
- 기본값 복원은 모든 Phase 3 Settings 값과 Binding Override에 함께 적용한다.

---

# 문서 작성 원칙

현재 System의 정의만 작성한다.

System의 책임만 작성한다.

Feature 규칙을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

변경 이력을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

System 하나당 문서 하나를 사용한다.
