# 시스템 개요

## 시스템명

RecordSubmissionSystem

---

## 목적

확정 Result Data를 저장·온라인 제출 가능한 후보로 연결한다.

로컬 제출 대기열과 온라인 제출 수단 사이의 책임 경계를 관리한다.

---

## Roadmap 9 현재 상태와 Phase 3 목표

기존 로컬 저장/온라인 제출·재시도·계정 연결은 구현돼 있다. Phase 2의 생산 Stage 플레이/결과 진입 차단은 완료했고, Phase 3의 Stage 후보 생성·직접 저장/Repository/Editor 실행 경계 차단과 Infinite 회귀는 미착수다. 아래 기존 공통 책임과 Phase 3의 목표를 구분한다.

### Phase 3 목표 — 직접 실행 경계는 미구현

- 신규 Stage 제출 후보의 생산 진입을 차단하고 전용 소스를 비권장 표시·사유 주석으로 보존한다.
- Infinite 후보/귀속/ID·대기열·제출 상태/재시도 경계를 유지한다. Stage 자료 보관/이관을 위한 새 책임은 추가하지 않는다.

검증 계약: P9-C04/P9-C06/P9-C07. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

# 시스템 책임

- 제출 후보와 제출 상태를 관리한다.
- 제출 후보의 계정 귀속, 제출 ID와 Pending 생성 시각을 관리한다.
- 로컬 대기열 저장소와 온라인 제출 저장소에 요청한다.
- 저장소 결과에 따라 Pending, Submitted 또는 Rejected 상태를 제공한다. Rejected 사유는 현재 실행 중 Result에만 보존하며, Rejected는 재시도 대상이 아니다.

---

# 시작 조건

- GameSystem이 확정 Result Data를 제출 후보 연결 경계에 제공한다.
- Application 시작, 온라인 복구, 인증 성공 또는 사용자 Retry 요청이 발생한다.

---

# 종료 조건

## 정상 종료

- 제출 후보가 Submitted 또는 Rejected 상태가 된다.

## 강제 종료

- Unity가 게임을 종료한다. Pending 후보는 로컬 대기열에 유지한다.

---

# 관리 대상

- 제출 후보
- 제출 상태
- 제출 후보의 계정 귀속 정보
- 제출 ID와 Pending 생성 시각

---

# 입력

| 입력 | 출처 |
|------|------|
| 확정 Result Data | GameSystem |
| 현재 계정 식별 정보 | Phase 2 Local Save, Phase 3 인증 경계 |
| 활성 연결 상태 | AccountConnectionSystem |
| 제출·저장소 결과 | 로컬·온라인 Repository 경계 |
| Retry 요청 | UIManagementSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| 제출 상태 | UIManagementSystem |
| 로컬 저장·대기열 요청 | Local Record Repository |
| 온라인 제출 요청 | Online Record Repository |

---

# 시스템 경계

## 담당 범위

- 제출 후보와 대기열 상태 관리
- 저장소·온라인 제출 요청 경계
- 계정 귀속 및 제출 ID 관리

## 담당하지 않는 범위

- Result Data 생성
- Stage Clear Time 또는 InfiniteMode Score 계산
- Leaderboard 순위 계산과 조회 표시
- 인증 토큰·서비스 Secret 관리
- 파일 형식·저장 경로·서버 SDK 구현
- Feature 규칙 정의

---

# 관련 System

- ResultSystem
- UIManagementSystem
- SettingsSystem
- AccountConnectionSystem

---

# 제약 사항

- 제출 후보는 Result Data를 변경하지 않는다.
- 후보의 로컬 소유 ID는 Local Save에 보존한 UUID v4다. Anonymous 인증 Player ID와 서버의 고정 Leaderboard 소유 ID는 별도 역할이며 동일하다고 가정하지 않는다.
- 같은 계정과 제출 ID는 서버 terminal receipt 보관 기간 180일 안에서만 중복 판정한다.
- terminal receipt가 180일 뒤 실제로 삭제된 경우에만 같은 제출 ID를 신규 제출로 허용한다. 삭제 실패·응답 유실은 기존 receipt가 남아 있는 것으로 처리한다.
- 다른 계정에 귀속된 후보를 전송하지 않는다.
- 현재 활성 연결이 아닌 인증 Player ID의 요청은 온라인 제출하지 않는다. 후보의 로컬 소유 ID와 인증 세션 권한은 별도로 검증한다.
- 사용자 동의·활성 계정 연결 없이 후보를 전송하지 않으며 다른 로컬 소유자/환경의 후보를 자동 재귀속하지 않는다.
- 동의·인증/연결 변경을 이유로 기존 후보의 소유 ID나 제출 ID를 임의 변경하지 않는다. 활성 연결 검증과 후보 소유 검증을 분리한다.
- Offline·인증·Timeout·서비스 실패는 Pending 생성 뒤 180일 전까지 Pending으로 유지하고 게임 진행을 차단하지 않는다. 180일에 도달한 Pending은 `SubmissionExpired`로 제거한다.
- C별 신규 제출은 60초에 최대 3회, 동일 제출 ID 재호출은 5초 간격으로 제한한다. `TooManyRequests`는 Client 자동 재시도 대상이 아니며 사용자 재시도 시각만 제공한다.
- 제출 거부 후보는 자동 재시도하지 않는다.
- 인증 토큰과 서비스 Secret을 저장하거나 전달하지 않는다.

---

# 문서 작성 원칙

현재 System의 정의만 작성한다.

System의 책임만 작성한다.

Feature 규칙을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

System 하나당 문서 하나를 사용한다.
