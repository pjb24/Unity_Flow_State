# 기능 개요

## 기능명

RecordSubmission

---

## 목적

플레이어의 제출 가능한 기록을 보존하고 온라인 Leaderboard에 한 번만 제출한다.

---

## Roadmap 9 현재 상태와 Phase 3 목표

현재 생산 플레이는 Infinite 단일 흐름이며 Stage 후보/신규 저장·직접 전송/재시도·Editor/Result 진입 차단과 기존 Infinite 저장/이전·전체 snapshot 보존을 완료했다. 사용자 최신 Compile 성공·EditMode 1031/1031·PlayMode 316/316 성공 및 각 예상 밖 Error/Warning 없음 보고를 최종 정적/로컬 검증과 연결해 Phase 3을 완료했다. Phase 4의 사용자 한정 QA 후보 판정도 완료했다. 최신 실제 원격 제출/조회·이전/복구 미검증 한계는 [Phase 4 Task](../90_Tasks/Prototype_9/20261118_01_Phase4ManualSteps.md)를 따른다. 아래 지원 규칙과 별도 Deprecated 내부 계약을 구분한다.

### 신규 생성/저장 규칙

- Stage 후보 생성 API는 정상 Cleared 입력도 false/out null로 거절한다. Stage/무효 Result는 제출 ID 생성 전에 거절하고, 합성 Stage/무효 Mode·현재 계약과 다른 Board를 service/Local 신규 저장 API에 주입해도 시각 부여·최고/Pending 변경 전에 거절한다.
- 내부 codec/Memory의 자료 복원은 보존하며 신규 저장 거절을 이유로 기존 항목을 지우거나 Infinite로 바꾸지 않는다. 직접 Stage 제출은 TransientFailure/StageModeUnsupported로 인증 전에 거절하고 raw SDK/Stage HTTP 요청도 전송 전에 거절한다. 계정 endpoint는 이 record 필터에서 제외한다. Editor의 Stage 도구/과거 probe도 인증·기준선·저장 전에 거절하며 Infinite 검증/계정 도구는 유지한다.
- 재시도는 전체 Pending의 Board·귀속/환경을 시계/만료 정리·checkpoint·인증 전에 검사한다. 미지원 자료가 있으면 원본을 삭제/변환하거나 terminal로 집계하지 않고 해당 trigger를 종료한다. 정상 Infinite는 동일 ID·최대 3회·내구 저장 실패 보호를 유지한다. 이는 Stage 큐 이관/전용 수명 정책의 추가가 아니다.
- Stage 기록은 없고 모든 Client의 추가 Stage 제출도 없다는 사용자 전제를 적용한다. 별도 Stage 보관/이관·전용 만료/폐기 기능을 추가하지 않는다.
- Infinite Pending의 ID/귀속/환경·재시도·확정 응답 후 정리·180일 만료·저장 실패 보호를 유지한다. 건수 0을 제출 성공으로 간주하지 않는다.

검증 계약: P9-C04/P9-C06/P9-C07. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

# 기능 규칙

- InfiniteMode 후보는 유효하게 확정된 종료 Result의 Total Score, Score 구성 요소, Scoring Version과 Run 시간을 포함한다.
- 제출 후보는 생성 시 UUID v4 제출 ID와 계정 귀속 정보를 받고 재시도·재실행 중 변경하지 않는다.
- 후보의 로컬 소유 ID는 Local Save에 보존한 UUID v4이며 Anonymous Authentication Player ID나 서버의 고정 Leaderboard 소유 ID와 혼동하지 않는다. 다른 소유자/환경의 Pending을 자동 재귀속하거나 전송하지 않는다.
- 온라인 요청에는 Anonymous 인증과 현재 활성 논리 계정 연결을 사용한다. 기존 기기에서 발급한 이전 코드/인증값으로 새 기기에 연결을 이전할 수 있으며 세부 규칙은 AccountTransfer Feature를 따른다. 외부 ID 연결은 지원하지 않는다.
- 기존 기기를 사용할 수 없어 이전 자격 증명을 발급할 수 없으면 공개 번호만으로 복구할 수 없다. 이 제한은 첫 온라인 요청 전 안내하고 Settings에서 다시 확인할 수 있다.
- 사용자가 복구 제한에 동의한 뒤 현재 인증 세션/계정 연결의 요청만 허용한다. 동의·연결 처리를 이유로 후보의 로컬 소유 ID나 제출 ID를 임의 변경하지 않는다.
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
- 과거 Stage/구 Version/미정의 후보나 선택은 Result 상태 표시·제출 Retry·개인 최고 조회로 다시 진입하지 않는다. 다른 Result/화면/runtime으로 이동한 뒤의 조회 응답은 현재 표시를 덮어쓰지 않는다. 정상 Infinite의 Local best·제출 상태와 별도 Online best는 유지한다.
- 동의 취소 시 온라인 요청을 시작하지 않고 진입 화면으로 복귀한다. Settings에서 복구 제한 안내를 다시 확인할 수 있다.
- Editor 격리 실행의 복구 제한 확인은 해당 실행의 메모리 저장소에만 남기며 온라인 인증·제출 권한으로 사용하지 않는다.
- 복구 제한 Continue는 동의의 로컬 저장에 성공하면 인증/번호 준비·Pending 전송을 기다리지 않고 안내를 닫는다.
  저장 실패 시 동의 없는 온라인 요청은 시작하지 않는다. 이어진 Leaderboard는 계정 준비 동안 Loading을 표시한다.
- 다른 계정에 귀속된 후보는 전송하지 않는다.
- 기기 이전 뒤 활성 연결이 아닌 인증 Player ID의 요청은 온라인 제출하지 않는다. 후보의 로컬 소유와 서버 요청 권한은 별도로 검증한다.
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

- 생산 Stage Result는 제출 대상으로 제공하지 않는다. 보존된 Stage factory와 직접 저장/전송 경계도 부작용 전에 거절한다.
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
- 기존 로컬 저장·Authentication·온라인 전송을 유지한다. Roadmap 9 Phase 3은 이 연결을 새로 만드는 단계가 아니라 Stage 직접 접근을 닫고 Infinite 계약을 보존하는 단계다.

---

# 보존된 Stage 내부 계약 — Deprecated

- 보존된 Stage 후보 형식은 `Cleared` 결과, 불변 Stage ID/Rules Version과 밀리초 Clear Time을 사용하며 `Fell`은 후보가 아니다. 이는 기존 자료형/정책 소스 설명이며 새 Client의 사용자 제출 기능이 아니다.
- TryCreateStageCandidate와 Deprecated 선언/이유·내부 형식은 소스에 남아 있다. factory는 false/out null이며 보존된 실행 본문은 지원 종료 guard로 차단한다.
- 순수 형식/변환 검증과 생산 Stage 요청 거절 검증을 구분한다. Stage 자료 보관/이관이나 별도 Pending 호환 기능을 추가하지 않는다.

---

# 검증 항목

- 유효 InfiniteMode Result의 후보 생성과 무효 점수/Version/소유자 거절을 확인한다.
- Phase 3에서 Stage 후보·직접 저장/조회/제출이 부작용 전에 거절되고 인증/전송/저장 호출이 발생하지 않는지 검증한다.
- 보존된 Board Key/밀리초 형식의 독립 검증과 지원되는 Infinite 점수 상한 검증을 구분한다.
- 더 나은 개인 최고 기록만 교체되고 동점 재제출은 유지되는지 확인한다.
- 같은 제출 ID의 중복 Pending 등록 거절과 동일 payload 재호출의 기존 receipt 반환을 구분한다. payload 변조 거절·Pending/Submitted/Rejected 상태 전이와 재시도 상한을 확인한다.
- 계정 귀속 불일치, Offline·Timeout·서비스 실패가 게임 진행을 차단하지 않는지 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의와 규칙만 작성한다.

System 책임, 구현 방법과 작업 기록을 작성하지 않는다.

동일한 내용을 다른 문서와 중복 작성하지 않는다.
