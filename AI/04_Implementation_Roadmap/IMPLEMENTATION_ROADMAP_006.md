# 목적

Flow State의 게임 진입 흐름과 기능 접근 UI를 구성하고 조작 방법을 플레이어에게 제공한다.

---

# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

6차 프로토타입

게임 실행 직후 Play가 시작되는 흐름을 Main Menu 중심 흐름으로 변경한다.

Stage Mode, InfiniteMode, Leaderboard, How To Play, Settings와 종료 기능에 일관된 UI 경로를 제공한다.

---

# 개발 단계

## Phase 1

### 목표

게임 진입 상태와 UI Navigation 계약을 확정한다.

### 구현 대상

- Boot·Main Menu·Mode Select·Playing·Pause·Result 상태 흐름
- Runtime Data와 Stage Play 생성 시점
- Main Menu·Pause·Result 간 이동 규칙
- Keyboard·Gamepad 선택, Submit과 Cancel 규칙
- 각 기능의 진입 위치와 돌아가기 경로
- 일반 UI와 개발자 UI의 표시 경계

### 완료 조건

- 실행부터 Run 시작·종료·Retry·Main Menu 복귀까지 상태 전이가 명확하다.
- Run은 Mode 선택과 시작 요청 이후에만 생성된다.
- 모든 화면의 기본 선택 항목과 Cancel 목적지가 확정되어 있다.
- Stage Mode와 InfiniteMode 진입 경로가 명확하다.
- Leaderboard가 준비되지 않은 상태의 UI 처리 기준이 있다.

### 검증 책임

- 상태 전이, 허용·거부 입력과 기본 선택은 Edit Mode 상태 Test 대상으로 정의한다.
- Scene UI 제작과 화면 가독성은 이 Phase에서 검증하지 않는다.

### 상태

완료 (20260920)

---

## Phase 2

### 목표

Boot, Main Menu와 Mode Select를 생산 흐름에 연결한다.

### 구현 대상

- Main Menu UI
- Play·Leaderboard·How To Play·Settings·Quit 항목
- Stage Mode와 InfiniteMode 선택 UI
- 게임 시작 전 Player·Stage·Timer 비활성 상태
- 선택한 Mode의 Runtime Data와 Run 시작
- Pause·Result의 Retry와 Main Menu 복귀
- UI와 Player Input Action Map 전환

### 완료 조건

- 게임 실행 직후 Player와 Stage가 움직이지 않는다.
- Main Menu에서 Mode를 선택한 뒤에만 Run이 시작된다.
- Retry는 같은 Mode의 새 Run을 시작한다.
- Main Menu 복귀는 이전 Run의 Runtime Data를 정리한다.
- Keyboard로 모든 메뉴를 탐색할 수 있다. GamePad는 보유하지 않은 경우 미확인 사유를 기록한 사용자 승인 검증 제외로 처리한다.
- Quit 이외의 기능 접근에 불필요한 확인 단계를 추가하지 않는다.

### 검증 책임

- Game State·UI State·Action Map과 Runtime Data 생명주기는 Play Mode Test로 검증한다.
- Scene의 UI 참조와 EventSystem 구성을 정적으로 검사한다.
- Focus 표시와 입력 장치별 탐색만 화면으로 확인한다.

### 상태

완료 (20260921)

---

## Phase 3

### 목표

각 기능의 접근 UI와 Settings를 완성한다.

### 구현 대상

- Main Menu의 각 기능 화면 연결
- Stage Select 또는 현재 단일 Stage 시작 경로
- Leaderboard 준비·미구현·Offline 상태 표시 영역
- Settings의 Audio·화면·조작 항목 범위 확정
- Input Rebinding과 기본값 복원
- Pause와 Main Menu에서 Settings 공유
- 일반 빌드의 개발자 Difficulty UI 비활성 확인

### 완료 조건

- 모든 구현 기능에 Main Menu 또는 게임 내 접근 경로가 있다.
- 기능 화면에서 이전 화면으로 돌아갈 수 있다.
- Input Rebinding 후 UI와 Player 입력이 새 Binding을 사용한다.
- Binding 충돌·취소·기본값 복원 규칙이 동작한다.
- 일반 빌드에서 개발자 전용 정보가 표시되지 않는다.

### 검증 책임

- Navigation, Settings 값과 Binding 적용·복원은 자동 Test로 검증한다.
- 실제 입력 장치 전환, 선택 표시와 화면 배치는 수동으로 확인한다.
- Settings 저장은 Roadmap 7 저장 계층 도입 전에는 Runtime 범위로 제한한다.

### 상태

대기

---

## Phase 4

### 목표

How To Play과 첫 플레이 안내를 제공하고 전체 UI 흐름을 검증한다.

### 구현 대상

- 자동 이동·Jump·Momentum Landing·Collectible·Pause 안내
- 현재 Input Binding을 반영하는 조작 표기
- 첫 Run 전 간단한 안내와 건너뛰기
- Main Menu의 How To Play 재열람
- 안내 중 게임 진행과 Player 입력 차단
- 전체 Menu·Stage·InfiniteMode·Pause·Result 흐름 회귀

### 완료 조건

- 플레이어가 안내만 보고 핵심 조작과 Score 목적을 이해할 수 있다.
- Keyboard와 Gamepad의 현재 Binding이 올바르게 표시된다.
- 안내 중 Stage, Timer와 Player가 진행되지 않는다.
- 안내를 건너뛰거나 다시 열 수 있다.
- 전체 UI 흐름의 자동 Test, 화면 확인과 Build가 통과한다.

### 검증 책임

- Binding 표시와 안내 상태 생명주기는 자동 Test로 검증한다.
- 문구 이해도, 가독성, Focus와 다양한 화면 비율은 수동으로 확인한다.
- Tutorial 완료 영구 저장은 Roadmap 7에서 검증한다.

### 상태

대기

---

# 현재 개발 진행 상태

## 진행 중인 작업

없음

---

## 다음 작업

Phase 3 기능 접근 UI·Settings 범위 확정·Input Rebinding 작업 준비

---

## 보류된 작업

- 별도 Tutorial Stage는 How To Play과 첫 플레이 안내의 효과를 확인한 뒤 결정한다.
- Settings와 Tutorial 완료 상태의 영구 저장은 Roadmap 7에서 수행한다.
- 실제 Leaderboard 조회·제출은 Roadmap 7에서 수행한다.

---

## 완료된 단계

- Prototype 4: InfiniteMode 생산 흐름과 HUD·Result 구성
- Prototype 6 Phase 1: 게임 진입 상태와 UI Navigation 계약 확정
- Prototype 6 Phase 2: Boot·Main Menu·Mode Select 생산 연결 및 Pause·Result 복귀 흐름 완성

---

# 구현 우선순위

1. 게임 진입·UI Navigation 계약
2. Main Menu와 Mode Select
3. 기능 접근 UI, Settings와 Input Rebinding
4. How To Play, 첫 플레이 안내와 전체 회귀

---

# 완료 기준

- 게임 실행 직후 Main Menu가 표시되고 게임은 시작되지 않는다.
- Stage Mode와 InfiniteMode를 UI에서 선택해 시작할 수 있다.
- Pause와 Result에서 Retry 또는 Main Menu로 이동할 수 있다.
- 모든 구현 기능에 일관된 접근과 복귀 경로가 있다.
- Keyboard와 Gamepad로 전체 UI를 사용할 수 있다.
- 핵심 조작과 게임 규칙을 안내하는 화면이 제공된다.
- 일반 빌드에서 개발자 전용 UI가 표시되지 않는다.
- Compile, 전체 Test, 화면 비율·입력 장치 확인과 Build가 통과한다.

---

# 관련 문서

- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/03_Features/StagePlay.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`

---

# 작성 완료 기준

- 6차 프로토타입의 현재 구현 계획과 순서를 작성했다.
- 구현 방법과 작업 기록을 포함하지 않았다.
- 영구 저장과 실제 Leaderboard 구현을 Roadmap 7 범위로 구분했다.
