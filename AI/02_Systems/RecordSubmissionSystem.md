# 시스템 개요

## 시스템명

RecordSubmissionSystem

---

## 목적

확정 Result Data를 저장·온라인 제출 가능한 후보로 연결한다.

로컬 제출 대기열과 온라인 제출 수단 사이의 책임 경계를 관리한다.

---

# 시스템 책임

- 제출 후보와 제출 상태를 관리한다.
- 제출 후보의 계정 귀속과 영구 제출 ID를 관리한다.
- 로컬 대기열 저장소와 온라인 제출 저장소에 요청한다.
- 저장소 결과에 따라 Pending, Submitted 또는 Rejected 상태를 제공한다.

---

# 시작 조건

- ResultSystem이 확정 Result Data를 제공한다.
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
- 영구 제출 ID

---

# 입력

| 입력 | 출처 |
|------|------|
| 확정 Result Data | ResultSystem |
| 현재 계정 식별 정보 | 인증 경계 |
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

---

# 제약 사항

- 제출 후보는 Result Data를 변경하지 않는다.
- 같은 계정과 제출 ID의 후보를 중복 제출하지 않는다.
- 다른 계정에 귀속된 후보를 전송하지 않는다.
- Offline·인증·Timeout·서비스 실패는 Pending 상태로 유지하고 게임 진행을 차단하지 않는다.
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
