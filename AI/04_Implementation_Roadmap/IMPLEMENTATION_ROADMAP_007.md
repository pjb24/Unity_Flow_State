# 목적

Flow State의 기록을 영구 저장하고 Stage Mode와 InfiniteMode의 온라인 Leaderboard를 제공한다.

---

# 프로젝트 정보

## 프로젝트명

Flow State

---

## 목표

7차 프로토타입

로컬 설정·진행 상태와 확정된 게임 기록을 보존한다.

Stage별 Clear Time과 InfiniteMode Total Score를 독립된 온라인 Leaderboard로 제출하고 조회한다.

---

# 개발 단계

## Phase 1

### 목표

저장 범위, 기록 모델과 Leaderboard 운영 정책을 확정한다.

### 구현 대상

- 기존 Runtime 전용·로컬 저장 제외·서버 저장 제외 범위 변경
- Player 식별과 계정·Anonymous Authentication 정책
- Stage Mode와 InfiniteMode Record 구조
- Stage별 Clear Time과 InfiniteMode Total Score 순위 기준
- 동점 처리와 Player별 최고 기록 유지 규칙
- Scoring Version·Game Version과 Leaderboard 분리 정책
- Offline·중복 제출·재시도·서비스 장애 정책
- 기록 삭제·계정 복구·데이터 운영 범위
- Client Score 신뢰 수준과 서버 검증 범위

### 완료 조건

- 로컬과 서버에 저장할 데이터가 명확히 구분되어 있다.
- 일반 Stage는 Stage별 Clear Time 오름차순, InfiniteMode는 Total Score 내림차순을 사용한다고 확정되어 있다.
- 동점, 최고 기록, Score Version과 기록 초기화 정책이 확정되어 있다.
- Anonymous 계정의 기기 변경·복구 제한을 처리하는 정책이 있다.
- Offline과 서비스 실패가 Run 결과 또는 Menu 진행을 차단하지 않는다고 명시되어 있다.
- 프로젝트 문서의 저장 제외 범위가 새 결정과 일치한다.

### 검증 책임

- 기록 정렬·동점·Version·중복 제출 규칙은 순수 상태 Test 대상으로 정의한다.
- 외부 서비스 연결이나 실제 데이터 전송은 이 Phase에서 수행하지 않는다.

### 상태

대기

---

## Phase 2

### 목표

게임 로직과 저장 수단을 분리한 로컬 기록·설정 저장 계층을 구축한다.

### 구현 대상

- Record Submission Service와 저장소 경계
- Memory·Local·Online Record Repository 계약
- Player 최고 기록과 제출 대기열
- Tutorial 완료, Settings와 Input Binding 저장
- Save Version과 Migration 정책
- 손상·누락·구버전 Save 복구
- 중복 Run 제출을 막는 Run ID

### 완료 조건

- 게임 로직이 특정 서버 SDK에 직접 의존하지 않는다.
- 게임 재실행 후 개인 최고 기록, Settings, Binding과 Tutorial 상태가 복구된다.
- Offline 기록을 중복 없이 제출 대기열에 보관할 수 있다.
- 손상되거나 지원하지 않는 Save를 안전한 기본값으로 복구한다.
- Save Version 변경을 자동 Test로 검증할 수 있다.

### 검증 책임

- 직렬화·역직렬화, Migration, 손상 복구와 중복 방지는 Edit Mode Test로 검증한다.
- 재실행·Settings·Binding·Tutorial 상태 복구는 Play Mode Test로 검증한다.
- 실제 사용자 저장 경로와 파일 수명은 대상 플랫폼 Player에서 확인한다.

### 상태

대기

---

## Phase 3

### 목표

Authentication과 온라인 Leaderboard 제출·조회를 연결한다.

### 구현 대상

- Unity Gaming Services 프로젝트와 환경 구분
- Authentication 초기화와 Player ID
- Stage별 Leaderboard와 InfiniteMode Leaderboard
- 최고 기록 제출과 현재 순위 조회
- 상위 기록·내 주변 기록·내 최고 기록 조회
- Timeout·Offline·재시도와 중복 제출 방지
- Scoring Version별 Leaderboard 분리
- 서비스 설정과 Secret을 저장소에 노출하지 않는 운영 방식

### 완료 조건

- Stage와 InfiniteMode 기록이 서로 다른 Leaderboard에 제출된다.
- 서로 다른 Stage와 Scoring Version의 기록이 섞이지 않는다.
- 같은 Run의 재시도가 중복 기록을 만들지 않는다.
- 서비스 실패 시 로컬 결과는 유지되고 UI에서 재시도할 수 있다.
- Authentication 실패가 게임의 Offline 플레이를 막지 않는다.
- 개발·검증·운영 환경의 데이터가 분리된다.

### 검증 책임

- 외부 SDK는 Repository 대역을 사용한 자동 Test로 성공·실패·Timeout을 검증한다.
- 별도 검증 환경에서 실제 Authentication·제출·조회 통합을 확인한다.
- 운영 데이터 변경은 명시적으로 승인된 배포 절차에서만 수행한다.

### 상태

대기

---

## Phase 4

### 목표

Leaderboard UI와 기록 경쟁 흐름을 완성하고 출시 후보 품질을 검증한다.

### 구현 대상

- Main Menu의 Leaderboard 화면
- Stage·InfiniteMode Tab과 Stage 선택
- 상위 기록·내 주변 기록·내 최고 기록
- Result의 새 최고 기록과 제출 상태
- Loading·Empty·Offline·Error·Retry UI
- 계정 식별·복구 정책 안내
- 전체 Menu·Run·Result·저장·서버 회귀
- 대상 플랫폼 Build와 성능·네트워크 확인

### 완료 조건

- 플레이어가 Main Menu와 Result에서 자신의 기록과 순위를 확인할 수 있다.
- Loading·Empty·Offline·Error 상태가 구분되어 표시된다.
- 네트워크가 없어도 Stage와 InfiniteMode를 플레이하고 결과를 보존할 수 있다.
- 연결 복구 후 대기 기록을 한 번만 제출한다.
- 기록 정렬, 내 순위와 최고 기록이 확정된 정책과 일치한다.
- 전체 자동 Test, 실제 서비스 검증과 대상 플랫폼 Build가 통과한다.

### 검증 책임

- UI 상태와 Repository 결과 연결은 Play Mode Test로 검증한다.
- 검증 환경에서 실제 계정·제출·조회·Offline 복구를 확인한다.
- 서비스 응답 시간, UI 가독성과 대상 플랫폼 동작은 수동으로 확인한다.

### 상태

대기

---

# 현재 개발 진행 상태

## 진행 중인 작업

없음

---

## 다음 작업

Roadmap 5–6 완료 후 Phase 1 저장 범위와 Leaderboard 운영 정책 확정

---

## 보류된 작업

- 자체 Backend 구축은 Unity Gaming Services로 요구사항을 충족할 수 없는 경우에만 검토한다.
- 강화된 부정행위 방지와 Server Authoritative Run 검증은 공개 경쟁 운영 수준이 확정될 때 결정한다.
- 친구·시즌·지역별 Leaderboard와 보상 지급은 기본 순위 서비스 안정화 후 검토한다.

---

## 완료된 단계

- Prototype 4: InfiniteMode Score와 Result Data 기반 확보

---

# 구현 우선순위

1. 저장 범위, Record와 Leaderboard 정책
2. 저장소 경계와 로컬 저장
3. Authentication과 온라인 제출·조회
4. Leaderboard UI와 출시 후보 검증

---

# 완료 기준

- 확정된 기록과 사용자 설정이 게임 재실행 후 복구된다.
- Stage별 Clear Time과 InfiniteMode Total Score Leaderboard가 분리된다.
- Scoring Version이 다른 기록이 섞이지 않는다.
- Offline·Timeout·중복 제출을 안전하게 처리한다.
- Main Menu와 Result에서 기록 제출 상태와 순위를 확인할 수 있다.
- 게임 로직과 외부 서비스 구현이 Repository 경계로 분리된다.
- 실제 검증 환경의 Authentication·제출·조회와 대상 플랫폼 Build가 통과한다.

---

# 관련 문서

- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/TimeRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_006.md`

---

# 작성 완료 기준

- 7차 프로토타입의 현재 구현 계획과 순서를 작성했다.
- 구현 방법과 작업 기록을 포함하지 않았다.
- 저장 정책 확정, 로컬 기반, 서버 연결과 UI 단계를 분리했다.
