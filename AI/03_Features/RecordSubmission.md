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
- Phase 2는 최초 로컬 실행 때 생성해 Local Save에 보존한 UUID v4 계정 ID를 후보 귀속에 사용한다. 이 값은 Anonymous Authentication Player ID가 아니며, 다른 계정에 귀속된 Pending 후보를 자동 전송하지 않는다.
- 온라인 조회 또는 첫 제출 요청 시 Anonymous 계정을 사용한다. 초기 버전은 계정 연결·복구를 제공하지 않는다.
- 재설치·기기 변경 뒤 온라인 기록을 복구할 수 없다는 제한은 첫 온라인 요청 전 안내하고 Settings에서 다시 확인할 수 있다.
- 사용자가 복구 제한을 확인하고 명시적으로 동의한 경우에만 현재 Anonymous Authentication Player ID를 제출 대상으로 1회 귀속한다. 로컬 UUID 후보를 묵시적으로 재귀속하거나 전송하지 않는다.
- 복구 제한 확인 상태가 저장되기 전에는 생산 온라인 조회와 제출을 시작하지 않는다. 검증 경로는 명시적으로 제공된 확인 상태에서만 요청할 수 있다.
- 같은 계정과 제출 ID의 terminal receipt는 서버 수락 또는 거부 시각부터 180일 동안만 중복 판정에 사용한다.
- Pending 후보는 Local Save에 생성 시각·동일 제출 ID·계정 귀속으로 보존하고 앱 재시작 시 복원한다. 생성 후 180일 전까지 Offline, Authentication 실패, Timeout과 서비스 실패는 Pending을 유지하며 게임 진행을 차단하지 않는다.
- 사용자에게 verification/Production 환경 전환 UI를 제공하지 않는다. 빌드는 명시된 하나의 UGS 환경으로만 초기화하며, 인증 Player ID·공개 번호 캐시·개인 최고·Pending은 `(Project ID, Environment ID)`별 Local Save 영역에 보존한다.
- Settings·튜토리얼·입력 설정은 기기 공용으로 유지한다. 환경이 다른 온라인 Local Save는 전송·병합·자동 재귀속하지 않는다. 기존 환경 표식 없는 온라인 Local Save는 verification 영역으로 전환한다.
- Pending 후보는 앱 시작, 온라인 복구, 인증 성공과 사용자 Retry에서 최대 세 번의 지수 백오프 재시도를 수행한다. 온라인·동의 상태에서 새 Run이 끝나면 해당 제출 ID도 자동 시도한다. 현재 Run의 자동·수동 제출은 다른 Pending 후보의 실패에 막히지 않도록 해당 ID만 처리한다.
- Leaderboard의 별도 `Retry Pending (N)`은 새 플레이 없이 같은 계정의 저장된 Pending 전체를 순회한다. 한 후보의 일시 실패는 다음 후보의 재시도를 막지 않는다. 사용자 요청은 이미 진행 중인 복구 요청이 끝나기를 기다린 뒤 실행하며, 완료 후 이번 요청의 Submitted·Rejected 건수와 남은 Pending 건수를 보여 준다.
- Submitted 또는 Rejected 확정 응답을 받으면 해당 Pending 항목만 Local Save에서 제거한다. 서버는 terminal receipt의 제출 ID·payload 판정 자료·결과·서버 시각·거절 분류를 180일 보관한 뒤 삭제한다. 삭제 저장이 실패하면 180일 전에는 동일 ID로 서버 결과를 다시 확인한다.
- Pending 생성 시각이 180일에 도달하면 Client는 `SubmissionExpired`로 안내하고 해당 Pending을 명시적으로 제거한다. 이전 Local Save에 생성 시각이 없으면 최초 업그레이드 로드 시각을 생성 시각으로 저장한다. 이 시각은 Client 정리용이며 서버 권한·기록 진위의 근거가 아니다.
- terminal receipt가 삭제된 뒤 같은 제출 ID가 들어오면 새 제출로 처리한다. 따라서 180일 뒤 복원된 백업 또는 ID 재사용은 새 기록이 될 수 있다.
- 서버 유효성 거부 후보는 Rejected가 되며 자동·수동 재시도하지 않는다. 거부 사유와 `재시도 불가`는 현재 Result에만 표시하고 제출 Retry 버튼은 Pending 후보에만 제공한다.
- Pending은 `Submission pending — retrying when online`, Submitted는 `Submitted`로 표시한다. Submitted는 해당 제출 ID의 서버 수락을 뜻하며 최고 기록 갱신이나 순위 조회 성공을 뜻하지 않는다. Pending 0건만으로 제출 성공을 판정하지 않는다.
- 동의 취소 시 온라인 요청을 시작하지 않고 진입 화면으로 복귀한다. Settings에서 복구 제한 안내를 다시 확인할 수 있다.
- 다른 계정에 귀속된 후보는 전송하지 않는다.
- 기기 이전 뒤 현재 활성 연결이 아닌 Player ID에 귀속된 후보는 온라인 제출하지 않는다.
- 로컬 데이터 초기화는 제출 대기열을 삭제하며 온라인 순위와 서버 기록은 유지됨을 고지한다.
- InfiniteMode 서버 검증은 Run 시간과 규칙 Version의 최대 배율·속도·Pattern·Collectible 한계로 계산한 논리적 최대 Total Score를 넘거나 상한 입력이 누락·불일치한 후보를 거부한다.
- InfiniteMode 후보의 Run 시간은 시작부터 결과 확정까지 측정하고 Pause 시간을 제외한다. 측정 시간이 없으면 0초로 대체해 제출하지 않는다.
- 온라인 쓰기는 서버 검증 경계를 통과하며, Player의 Leaderboard 직접 쓰기는 허용하지 않는다. 180일 보관 중 같은 제출 ID·같은 payload는 기존 결과를 반환하고 다른 payload는 거부하며, Board·Version 일치도 함께 검증한다.
- Prototype 8의 부정행위 방지는 서버 입력 검증, Player 직접 쓰기 차단, 활성 연결 확인, 제출 ID·Board·Version·구성 합계·점수 상한 검증까지다. Client 조작을 완전히 증명하는 서버 권위 Run 시뮬레이션은 이 범위에 포함하지 않는다.
- 서버는 C별 신규 제출을 60초에 최대 3회, 같은 제출 ID 재호출을 5초 간격으로 제한한다. 제한 초과는 `TooManyRequests`와 안전한 재시도 시각으로 응답하며 Client는 자동 재시도하지 않는다.
- 온라인 제출은 활성 Player ID가 연결된 논리 계정의 고정 Leaderboard 소유 ID에만 반영한다. 기기 이전은 기존 기록을 새 Player ID 행으로 복사하지 않는다.

---

# 시작 조건

- 제출 가능한 확정 Result가 생성되었다.
- Pending 후보에 재시도 계기가 발생했다.

---

# 종료 조건

## 정상 종료

- 후보가 Submitted 또는 Rejected 상태가 된다.

## 강제 종료

- Application이 종료되어도 저장된 Pending 후보는 유지된다. 완료 제출 상태와 거절 사유는 재시작 뒤 복원하지 않는다.

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
