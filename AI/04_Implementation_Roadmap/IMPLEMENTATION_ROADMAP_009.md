# 목적

Stage Mode의 플레이·진입·기록 접근 기능을 제거하고 InfiniteMode만 제공하는 후보로 전환한다.

# 프로젝트 정보

## 프로젝트명

Flow State

## 목표

Roadmap 9 — InfiniteMode 단일화와 Stage Mode 접근 제거.

계획 확정일: 2026-11-11. **Phase 1 완료·생산 전환 구현 미착수**다. Phase 2~4는 대기다.
선행: [Roadmap 8](IMPLEMENTATION_ROADMAP_008.md) 완료 후 순차 착수한다.
공통 순서·승인/검증 경계는 [로드맵 안내](README.md)를 따른다.

## 범위 밖

Stage 서버 보드/기존 자료 삭제, 공통 StageSystem의 명칭만을 근거로 한 삭제, Pattern/점수 규칙 변경, 기존 기록의 Infinite 기록 전환.

2026-11-11 Step 2 추가 확정: Stage 소스는 삭제하지 않고 Deprecated 및 사유 주석으로 보존한다.
사용자 접근/명시적 실행 차단은 별도로 구현하며 Infinite 공통 기반과 기존 자료의 내부 읽기 호환은 구분한다.

2026-11-11 Step 3 범위 변경: 사용자는 구 Client에 Stage 기록이 없고 모든 Client에서 앞으로도 제출되지 않는다고 전제했다.
Stage 기록 보관/이관·Pending 호환은 고려 대상에서 제외한다. 아래 최초 계획의 관련 항목보다 이 결정이 우선한다.
자료 부재/전체 Client 중단을 실제 검증한 결과는 아니며 자료 삭제·보드 초기화 작업은 수행하지 않는다.

2026-11-11 Step 4 확정: 구 Client는 미지원이다. Infinite 저장/서버 계약을 유지하며 현재 전환 범위에서 서비스 계약 변경은 필요하지 않다.
구 Client의 강제 접근 차단을 구현한 것은 아니다. 신규 Stage 접근은 새 Client에서 차단하고 소스는 Deprecated로 보존한다.

# 개발 단계

## Phase 1. 제거 범위와 자료 호환 정책 확정

### 목표

기능 제거와 기존 자료 삭제를 구분하고 제거 후 사용 흐름을 확정한다.

### 구현 대상

- Stage Mode 실행 기능·모든 진입 경로·기록 조회/표시/조작 기능의 제거 범위 조사.
- 사용자 전제에 따른 Stage 기록 처리 제외·구 Client 미지원 및 Infinite 공통 계약 유지 범위 결정.
- InfiniteMode가 사용하는 StageSystem 등 공통 기반과 Stage 전용 기능 구분.

### 완료 조건

- 제거할 사용자 경로와 유지할 내부 호환 경로가 식별되어 있다.
- Stage 기록 없음/추가 제출 없음의 사용자 전제와 별도 기록 처리 제외, 구 Client 미지원이 명시되어 있다.
- 자료 삭제·보드 초기화 없이 전환하는 기준과 필요한 서비스 변경이 확정되어 있다.

### 상태

완료 — 2026-11-11. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md) Step 1~8에 정책·정적/로컬 검증·후속 인계와 최종 판정 근거를 기록했다.
이는 범위/정책·검증 계약 준비 완료이며 실제 접근 차단/Deprecated 적용·Unity 실행/후보 판정 완료가 아니다.

## Phase 2. Stage Mode 플레이·접근 경로 제거

### 목표

새 Run 생성과 우회 진입을 포함해 Stage Mode를 실행할 수 없게 한다.

### 구현 대상

- Main Menu·선택 화면·Retry·저장된 선택/기본값·검증/개발용 실행 경로의 Stage 진입 정리.
- Stage 전용 Goal·Clear Time 결과 흐름 정리와 InfiniteMode 공통 기능 유지.
- How To Play·최초 안내·Settings 등 관련 안내의 InfiniteMode 기준 정리.

### 완료 조건

- 생산 Player와 대상 Editor 실행 경로에서 Stage Mode를 시작/재시작할 수 없다.
- 이전 상태를 읽어도 Stage로 복귀하지 않고 InfiniteMode 흐름을 유지한다.
- InfiniteMode의 시작·Pause·종료·Retry가 유지된다.

### 상태

대기 — 구현/실행 검증 미착수.

## Phase 3. Stage 기록 접근 차단과 Infinite 기록 보존

### 목표

사용자에게 Stage 기록을 제공하지 않고 Infinite 기록·저장·계정 이전의 기존 계약을 유지한다.

### 구현 대상

- Leaderboard 탭·최고 기록·Result 링크·직접 조회 등 Stage 기록 접근 경로 제거.
- Infinite 저장/서버 개인 최고 snapshot·계정 이전 계약 유지. 사용자 전제로 Stage 자료 보관/이관은 추가하지 않는다.
- 신규 Stage 제출 후보·직접 제출 진입을 차단하고 Infinite Pending의 기존 수명·재시도·이전 조건을 유지한다.

### 완료 조건

- 사용자 기능으로 Stage 기록을 조회·표시·조작할 수 없다.
- InfiniteMode 조회·제출·계정 이전·재시작을 보존한다. 구 Client와 Stage 기존 자료 호환 개발은 확정 범위에서 제외한다.
- 기존 자료를 자동 삭제하거나 Infinite 기록으로 변환/병합하지 않는다.

### 상태

대기 — 구현/실행 검증 미착수.

## Phase 4. 통합 회귀와 전환 후보 판정

### 목표

제거 경계와 남은 게임 흐름의 정상 동작을 검증한다.

### 구현 대상

- 정적 진입 경로 검사·상태/저장/온라인 계약 회귀 준비.
- 필요한 사용자 Unity 검증·Scene 반영·Player 후보 확인 범위 확정.
- Module 변경이 필요한 경우 승인된 배포·복구 시험과 데이터 보존 인계.

### 완료 조건

- 관련 자동 검증과 사용자 실행 근거가 연결되어 있다.
- Stage 접근 차단·Deprecated 소스 보존·InfiniteMode와 공통 저장/계정 계약 보존을 판정할 수 있다.
- 추가 원격 변경/자료 삭제 없이 후보의 변경 범위와 잔여 위험을 기록했다.

### 상태

대기 — 구현/실행 검증 미착수.

# 현재 개발 진행 상태

## 진행 중인 작업

없음. Phase 1 완료. Phase 2~4 생산 전환 구현은 대기다.

## 다음 작업

Phase 2의 Stage 플레이/진입 차단과 Infinite 시작/안내 흐름 구현을 준비한다.
파일·Test·사용자 Scene 작업은 [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md)의 Step 5~7 인계를 따른다. 이번에는 Phase 2에 착수하지 않았다.

## 보류된 작업

Stage 자료 삭제/보드 폐기는 수행하지 않는다. 구 Client는 미지원으로 확정했으며 원격 강제 차단/서비스 변경은 수행하지 않았다.
Pattern 연결·난이도·위험 보상 개선은 Roadmap 9~14 범위 밖으로 보류하며 후속 번호/착수 시점은 미정이다.

## 완료된 단계

Phase 1 완료(2026-11-11). 나머지 Phase 2~4는 대기이며 Roadmap 9 전체는 미완료다.

# 구현 우선순위

1. 제거 범위와 자료 호환 정책 확정.
2. Stage Mode 플레이·접근 경로 제거.
3. Stage 기록 접근 차단과 Infinite 기록 보존.
4. 통합 회귀와 전환 후보 판정.

# 완료 기준

Stage Mode의 플레이·진입·기록 접근 기능을 제거하고 InfiniteMode만 제공하는 후보로 전환한다.
각 Phase의 완료 근거와 관련 문서 반영·회귀 검증 결과를 확보하고 범위 밖 기능/데이터 변경을 수행하지 않는다.
계획 승인만으로 실제 구현/사용자 검증·원격 배포·공개 개시가 승인되거나 성공한 것으로 기록하지 않는다.

# 관련 문서

- [로드맵 안내](README.md)
- [Project Memory](../00_Project/PROJECT_MEMORY.md)
- [Verification Rule](../01_Rules/VERIFICATION_RULE.md)
- [StagePlay](../03_Features/StagePlay.md)
- [GameEntryNavigation](../03_Features/GameEntryNavigation.md)
- [ResultMenu](../03_Features/ResultMenu.md)
- [Leaderboard](../03_Features/Leaderboard.md)
- [RecordSubmission](../03_Features/RecordSubmission.md)
- [AccountTransfer](../03_Features/AccountTransfer.md)
- [InfiniteMode](../03_Features/InfiniteMode.md)
- [Roadmap 10](IMPLEMENTATION_ROADMAP_010.md)

# 작성 완료 기준

- 목표/범위·단계·완료 조건과 다음 작업이 정리되어 있다.
- 현재 기능의 사실과 향후 변경 계획을 구분하고 구현 방법/작업 이력을 로드맵에 중복 기록하지 않는다.
- 의존성/보류 범위와 계획 확정·실제 완료의 차이가 명시되어 있다.

