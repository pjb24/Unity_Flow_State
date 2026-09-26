# 기능 개요

## 기능명

RecordSubmission

---

## 목적

플레이어의 제출 가능한 기록을 보존하고 온라인 Leaderboard에 한 번만 제출한다.

---

# 기능 규칙

- Stage는 `Cleared` 결과만 제출 후보가 된다.
- Stage 후보는 불변 Stage ID, Stage Rules Version과 가장 가까운 밀리초로 반올림한 Clear Time을 포함한다.
- InfiniteMode 후보는 유효하게 확정된 종료 Result의 Total Score, Score 구성 요소, Scoring Version과 Run 시간을 포함한다.
- 제출 후보는 생성 시 UUID v4 제출 ID와 계정 귀속 정보를 받고 재시도·재실행 중 변경하지 않는다.
- 온라인 조회 또는 첫 제출 요청 시 Anonymous 계정을 사용한다. 초기 버전은 계정 연결·복구를 제공하지 않는다.
- 재설치·기기 변경 뒤 온라인 기록을 복구할 수 없다는 제한은 첫 온라인 요청 전 안내하고 Settings에서 다시 확인할 수 있다.
- 같은 계정과 제출 ID의 후보는 한 번만 제출한다.
- Offline, Authentication 실패, Timeout과 서비스 실패는 후보를 Pending으로 보존하며 게임 진행을 차단하지 않는다.
- Pending 후보는 앱 시작, 온라인 복구, 인증 성공과 사용자 Retry에서 최대 세 번의 지수 백오프 재시도를 수행한다.
- 서버 유효성 거부 후보는 Rejected가 되며 자동 재시도하지 않는다.
- 다른 계정에 귀속된 후보는 전송하지 않는다.
- 로컬 데이터 초기화는 제출 대기열을 삭제하며 온라인 순위와 서버 기록은 유지됨을 고지한다.
- InfiniteMode 서버 검증은 Run 시간과 규칙 Version의 최대 배율·속도·Pattern·Collectible 한계로 계산한 논리적 최대 Total Score를 넘거나 상한 입력이 누락·불일치한 후보를 거부한다.

---

# 시작 조건

- 제출 가능한 확정 Result가 생성되었다.
- Pending 후보에 재시도 계기가 발생했다.

---

# 종료 조건

## 정상 종료

- 후보가 Submitted 또는 Rejected 상태가 된다.

## 강제 종료

- Application이 종료되어 Pending 후보가 로컬 대기열에 남는다.

---

# 수행 결과

- 제출 가능한 기록은 계정 귀속 대기열 또는 온라인 최고 기록으로 한 번만 반영된다.
- 서비스 실패가 발생해도 Result와 Offline 플레이는 유지된다.

---

# 예외 사항

- `Fell` Stage Result는 제출 후보가 아니다.
- 지원하지 않거나 일치하지 않는 규칙 Version은 제출하지 않는다.
- 계정 귀속이 현재 계정과 다르면 제출하지 않는다.
- 인증 토큰·서비스 Secret·개인정보는 제출 후보에 포함하지 않는다.

---

# 관련 System

- ResultSystem
- RecordSubmissionSystem
- UIManagementSystem

---

# 제약 사항

- RecordSubmission은 Result Data와 Score를 다시 계산하거나 변경하지 않는다.
- Leaderboard 조회와 순위 표시는 Leaderboard Feature가 담당한다.
- 실제 로컬 저장은 Phase 2, 실제 Authentication과 온라인 전송은 Phase 3에서 연결한다.

---

# 검증 항목

- `Cleared` Stage와 유효 InfiniteMode Result만 후보가 되는지 확인한다.
- Board Key 분리, 밀리초 시간 변환, Version 불일치와 점수 상한 거부를 확인한다.
- 더 나은 개인 최고 기록만 교체되고 동점 재제출은 유지되는지 확인한다.
- 동일 제출 ID 거부, Pending·Submitted·Rejected 상태 전이와 재시도 상한을 확인한다.
- 계정 귀속 불일치, Offline·Timeout·서비스 실패가 게임 진행을 차단하지 않는지 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
