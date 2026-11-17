# 시스템 개요

## 시스템명

StageSystem

---

## 목적

Stage의 진행 상태를 관리한다.

Stage의 시작과 종료를 관리한다.

Stage 종료 시점을 판단하고 Stage 종료 이벤트를 발생시킨다.

여기서 Stage는 Infinite에서도 사용하는 공통 Run 실행 단위를 뜻한다. StageSystem/이벤트 이름은 보존하며 사용자에게 Stage Mode를 시작시키는 책임은 없다.

---

## Roadmap 9 적용 상태

상태: Phase 2의 Infinite 기본 초기화·명시적 Stage/무효 Mode 초기화/시작 거절과 상태 불변 검증은 완료했다. 공통 생명주기와 Deprecated Goal/소스를 유지한다. 실제 실행 근거는 [Phase 2 Task](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md)를 따른다.

- Infinite의 시작·Pause/Resume·종료·이벤트·Root/Collectible/Pattern 연결을 계속 담당한다. System 전체를 Deprecated로 만들지 않는다.
- 명시적 Stage 생산 초기화/실행은 거절하고 Goal/Cleared·Stage 추락 전용 책임의 소스는 비권장 표시/사유 주석으로 보존한다.
- Stage Root/Goal 참조를 단독 삭제해 Infinite 초기화 계약을 깨뜨리지 않는다. Scene 적용은 후속 사용자 절차로 수행한다.

검증 계약: P9-C02/P9-C05/P9-C09. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

# 시스템 책임

- Stage를 시작한다.
- 유효한 Infinite 초기화에서 Infinite Root를 활성화하고 보존된 Stage Root를 비활성화한다.
- Stage 진행 상태를 관리한다.
- Stage Object 상태를 관리한다.
- GameSystem의 요청에 따라 Stage 진행 판정을 일시 중단하고 재개한다.
- Stage 종료 시점을 판단한다.
- Stage 종료 이벤트를 발생시킨다.
- 명시적 Stage Mode/미정의 Mode 초기화·시작 요청을 기존 Run·listener·Root·Collectible binding을 변경하기 전에 거절한다. 유효한 Infinite 공통 초기화/시작은 허용한다.

---

# Stage Object

Stage Object는 Stage를 구성하는 모든 오브젝트를 의미한다.

StageSystem은 Stage Object의 동작을 수행하지 않는다.

StageSystem은 Stage Object가 전달한 상태와 이벤트를 관리한다.

---

# 시작 조건

- GameSystem이 Stage 시작을 요청한다.

---

# 종료 조건

## 정상 종료

- Stage 종료 이벤트가 발생하였다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- 현재 Stage
- 현재 Stage의 게임 Mode
- Stage 진행 상태
- Stage Object 상태
- Stage 진행의 일시 중단 상태

---

# 입력

| 입력 | 출처 |
|------|------|
| Stage 시작 요청 | GameSystem |
| 현재 게임 Mode | GameSystem |
| Stage Object 상태 | Stage Object |
| Stage Object 이벤트 | Stage Object |
| Stage 진행 중단 및 재개 요청 | GameSystem |
| Player Rigidbody 물리 위치 | PlayerControllerSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| Stage 시작 알림 | TimerSystem |
| Stage 종료 이벤트 | GameSystem |
| Stage 종료 이벤트 | ResultSystem |
| Stage 종료 이벤트 | UIManagementSystem |

---

# 시스템 경계

## 담당 범위

- Stage 시작
- Infinite Stage Object 상태 활성화
- Stage 진행 상태 관리
- Stage Object 상태 관리
- Stage 종료 시점 판단
- Stage 종료 이벤트 발생
- Stage 진행 판정 중단 및 재개
- 비권장 Stage 컴포넌트/참조 소스 보존과 생산 실행 경계 구분

---

## 담당하지 않는 범위

- 게임 전체 흐름 관리
- 플레이어 이동
- 충돌 판정
- 클리어 시간 측정
- 결과 데이터 생성
- UI 표시
- Stage Object의 동작
- Feature 규칙 정의

---

# 관련 System

- GameSystem
- TimerSystem
- ResultSystem
- UIManagementSystem

---

# 제약 사항

- 게임 전체 흐름을 관리하지 않는다.
- Stage Object를 직접 제어하지 않는다.
- Stage Object의 종류를 관리하지 않는다.
- 게임 Mode를 결정하지 않는다.
- Stage Object가 전달한 상태와 이벤트만 사용한다.
- Stage 종료 조건을 정의하지 않는다.
- Stage 종료 시점만 판단한다.
- 클리어 시간을 측정하지 않는다.
- 결과 데이터를 생성하지 않는다.
- UI를 직접 제어하지 않는다.
- Feature 규칙을 정의하지 않는다.
- Stage 종료 시에는 반드시 Stage 종료 이벤트를 발생시킨다.
- 일시 중단 중에는 Stage Object의 종료 요청을 확정하지 않는다.
- 재개 시 일시 중단 이전의 Stage 진행 상태를 유지한다.
- 무인자 초기화는 Infinite이며 Stage와 미정의 Mode는 해제/상태 변경 전에 실패한다. 보존된 Stage 상태의 StartStage도 초기화 상태/중복 실행 검사와 Reset/Goal/binding 작업 전에 거절한다.
- 보존된 Stage 전용 Goal/Cleared·추락 판정 소스는 Deprecated/사유 주석과 독립 합성 Test 대상으로 남기며 생산 Stage Run을 열지 않는다.
- StageSystem 전체와 StageStarted/StageEnded·Pause/Resume·Infinite Pattern/Collectible 생명주기는 공통 기반으로 유지한다.
- InfiniteMode 추락 판정은 InfiniteModeSystem이 담당한다.

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
