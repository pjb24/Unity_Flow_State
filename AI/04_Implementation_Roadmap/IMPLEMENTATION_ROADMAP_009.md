# 목적

Stage Mode의 플레이·진입·기록 접근 기능을 제거하고 InfiniteMode만 제공하는 후보로 전환한다.

# 프로젝트 정보

## 프로젝트명

Flow State

## 목표

Roadmap 9 — InfiniteMode 단일화와 Stage Mode 접근 제거.

계획 확정일: 2026-11-11. **계획 확정·구현 미착수**이며 각 Phase의 상태는 대기다.
선행: [Roadmap 8](IMPLEMENTATION_ROADMAP_008.md) 완료 후 순차 착수한다.
공통 순서·승인/검증 경계는 [로드맵 안내](README.md)를 따른다.

## 범위 밖

Stage 서버 보드/기존 자료 삭제, 공통 StageSystem의 명칭만을 근거로 한 삭제, Pattern/점수 규칙 변경, 기존 기록의 Infinite 기록 전환.

# 개발 단계

## Phase 1. 제거 범위와 자료 호환 정책 확정

### 목표

기능 제거와 기존 자료 삭제를 구분하고 제거 후 사용 흐름을 확정한다.

### 구현 대상

- Stage Mode 실행 기능·모든 진입 경로·기록 조회/표시/조작 기능의 제거 범위 조사.
- 기존 Stage 개인 최고·Pending·서버 기록과 구 Client/새 Client의 호환 범위 결정.
- InfiniteMode가 사용하는 StageSystem 등 공통 기반과 Stage 전용 기능 구분.

### 완료 조건

- 제거할 사용자 경로와 유지할 내부 호환 경로가 식별되어 있다.
- 기존 Stage Pending의 전송/보존/처리 종료 정책과 구 Client 지원 여부가 명시되어 있다.
- 자료 삭제·보드 초기화 없이 전환하는 기준과 필요한 서비스 변경이 확정되어 있다.

### 상태

대기 — 구현/실행 검증 미착수.

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

## Phase 3. Stage 기록 접근 제거와 자료 호환

### 목표

사용자에게 Stage 기록을 제공하지 않되 기존 자료를 읽는 과정에서 장애가 발생하지 않게 한다.

### 구현 대상

- Leaderboard 탭·최고 기록·Result 링크·직접 조회 등 Stage 기록 접근 경로 제거.
- Stage 기록이 포함된 저장/서버 개인 최고 snapshot·계정 이전의 호환 처리.
- Phase 1에서 정한 기존 Pending 정책 적용과 신규 Stage 제출 후보 생성 차단.

### 완료 조건

- 사용자 기능으로 Stage 기록을 조회·표시·조작할 수 없다.
- 기존 Stage 자료가 있어도 InfiniteMode 조회·제출·계정 이전·재시작이 실패하지 않는다.
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
- Stage 제거/접근 차단·InfiniteMode 보존·기존 자료 호환을 판정할 수 있다.
- 추가 원격 변경/자료 삭제 없이 후보의 변경 범위와 잔여 위험을 기록했다.

### 상태

대기 — 구현/실행 검증 미착수.

# 현재 개발 진행 상태

## 진행 중인 작업

없음. 계획 문서 확정은 구현 착수나 구현 완료가 아니다.

## 다음 작업

선행 Roadmap 완료 후 Phase 1의 범위/영향 대조와 세부 작업 계획을 준비한다.

## 보류된 작업

기존 Stage 자료 삭제/보드 폐기와 구 Client 서비스 종료는 승인 범위 확정 전 수행하지 않는다.
Pattern 연결·난이도·위험 보상 개선은 Roadmap 9~14 범위 밖으로 보류하며 후속 번호/착수 시점은 미정이다.

## 완료된 단계

없음. 문서 작성만 완료했다.

# 구현 우선순위

1. 제거 범위와 자료 호환 정책 확정.
2. Stage Mode 플레이·접근 경로 제거.
3. Stage 기록 접근 제거와 자료 호환.
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

