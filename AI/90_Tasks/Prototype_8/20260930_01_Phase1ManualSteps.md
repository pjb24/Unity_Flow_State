# 작업 정보

## 작업명

Prototype 8 Phase 1 — 공개 운영·공개 번호 정책 확정 절차

## 작업 일자

20260930

## 작업 담당자

AI: 조사·제안·정적 검증·Unit Test 준비·문서 반영. 사용자: 정책 선택·필요한 Unity 실행.

## 작업 상태

Phase 1 완료. Step 1 정적 대조, Step 2 정책 확정·2-1~2-10 순수 정책 Test, Step 3 제출 receipt 180일 보관·Pending 만료·재호출 정책 Test, Step 4 조회 규모·동점·스냅샷 정책 Test, Step 5 환경·자료 격리·배포 승인 정책 Test, Step 6 운영 안전 정책 Test, Step 7 문서·정책 Test·Phase 2~5 인계 대조 및 Step 8 Phase 1 완료 판정·Phase 2 인계를 완료했다. 실제 서비스·Unity 구현은 Phase 2~5 대기 상태이며, Unity Test Runner·Build·서비스 적용은 수행하지 않았다.

# 작업 목적

Roadmap 008 Phase 1의 미정 정책을 사용자가 결정할 수 있는 순서로 정리한다. 코드·문서에서 판정 가능한 사항은 AI가 확인하고, 수치·경계·중복·경합은 자동 Test로 검증한다.

# 작업 대상

공개 번호 발급·표시, 논리 계정의 단일 활성 연결과 기기 이전, 고정 Leaderboard 소유 행, 서버 제출 이력 보관, 순위 조회, 환경·데이터 분리, 운영 관측 및 부정행위 방지 범위의 계약. 번호 발급·계정 이전 서비스와 운영 배포 등 본 구현은 후속 Phase에서 수행한다.

# 작업 전 상태

- Prototype 7 Phase 4는 사용자 보고 기준 Unity 컴파일·Edit Mode 729/729·Play Mode 234/234·verification UI·Windows Player 확인을 완료했다. 이 결과를 Prototype 8 변경 후의 성공으로 재사용하지 않는다.
- 내부 Player ID는 현재 마스킹 표시하며, 공개 십진 번호 도입은 Prototype 8 목표다.
- Pending은 Local Save에 보존하고 확정 응답 뒤 제거한다. Submitted/Rejected 완료 ID·거절 사유의 영구 로컬 저장은 추가하지 않는다.

# 조사 내용

- `UGS/CloudCode/submit-record.js`는 ledger entries 128건 이상에서 `LedgerCapacity`를 반환한다.
- `UGS/CloudCode/query-records.js`는 100명 한도를 가진 전체 목록을 조회하며, 부분 목록으로 전체 순위를 계산하지 않도록 검사한다. 현재 조회 요청 limit은 1~20이다.
- 현재 조회 정렬은 Score, 서버 수락 시각, 내부 Player ID 순서를 사용한다. Score가 같으면 공동 순위다. Step 4 확정 정책은 최종 표시 순서를 정확한 공개 번호 오름차순으로 전환한다.
- 기존 검증 경로는 `UGS/Tests/static-contracts.cjs`, `UGS/Tests/cloud-code.test.cjs`, `Assets/Tests/EditMode` 및 `Assets/Tests/PlayMode`다.

# 작업 내용

## 공통 진행 원칙

- 각 Step은 **AI의 근거·선택지 제공 → 사용자 결정 → AI 반영·검증** 순서로 진행한다. 제안이 나오기 전에 사용자가 기술 설계를 대신 작성할 필요는 없다.
- AI는 선택지마다 장점·단점·권장안·기존 동작에 미치는 영향을 제시한다. 사용자는 항목별 선택 또는 수정 내용만 답한다. 응답하지 않은 정책은 확정으로 기록하지 않는다.
- 이미 확정된 UI·동점·동의·Pending 계약과 Windows x64·1920×1080 Windowed·Keyboard/Mouse 기준은 다시 선택하지 않는다. 성능·응답 시간 측정 제외도 유지한다.
- AI는 Unity Build·Unity Test Runner를 실행하지 않는다. Scene을 편집하지 않으며 필요한 경우에만 사용자 적용 절차를 제공한다. Phase 1에는 Scene 편집·Build·실제 기록 제출이 기본적으로 필요하지 않다.
- 서버·계정·보드·저장 파일을 삭제하거나 초기화하여 검증하지 않는다. Unit Test는 메모리 저장소와 서비스 대역을 사용한다.

## Step 1. 현재 구조와 UGS 제약을 정적으로 대조한다

### AI 선행 작업

1. Roadmap·Feature·System 계약과 인증·계정 귀속·제출·조회·Local Save·기존 Test를 읽기 전용으로 대조한다.
2. 128/100 제한이 프로젝트 자체 제한인지, 서비스 한도인지 구분한다. 번호의 원자적 발급, 저장 충돌, 조회 페이지·순위 및 환경별 인증 범위는 현재 사용 SDK와 UGS 공식 문서로 확인한다. 출처·확인일·미확인 사항을 기록한다.
3. 기존 UGS에서 가능한 방안과 추가 검증이 필요한 제약을 정리한다. 기능을 추정하여 구현 가능으로 판정하지 않는다.
4. 로컬 Node가 있으면 아래 기존 검사를 AI가 실행하고 결과를 기록한다. 없으면 먼저 설치 상태를 확인하고 필요한 설치 절차만 제공한다.

```powershell
node UGS/Tests/static-contracts.cjs
node UGS/Tests/cloud-code.test.cjs
```

### 사용자 수동 작업

기본적으로 없음. 로컬 자료·공식 문서로 확인할 수 없는 실제 프로젝트 설정이 의사결정을 막는 경우에만 AI가 Dashboard의 정확한 확인 화면과 필요한 값 목록을 제시한다. 사용자는 지정된 설정을 읽고 요약만 전달한다. 아직 Production 환경을 만들거나 Cloud Code를 게시하지 않는다.

### 완료 조건

- [x] 현재 계약·서비스 제약·변경 필요 지점과 기존 로컬 검사 결과가 기록됐다. 실패가 있으면 원인과 후속 처리 범위가 구분됐다.

### 수행 결과 (2026-10-01)

- 현재 계약: 후보는 Local Save의 계정 귀속 UUID v4 제출 ID를 재시작 뒤에도 유지하고, Submitted/Rejected 응답 뒤 해당 Pending만 제거한다. Offline·인증·Timeout·서비스 실패는 플레이를 막지 않고 Pending으로 남는다. 같은 점수는 공동 순위이며 서버 수락 시각 오름차순으로 표시한다.
- 현재 구현: `submit-record.js`는 Player Protected Cloud Save의 `fs_submission_ledger_v1`을 write lock으로 갱신하고, 최대 128 entries에서 `LedgerCapacity`를 반환한다. 키가 없을 때는 `LedgerNotProvisioned`으로 실패하므로 신규 계정 ledger는 현재 수동 생성이 필요하다. 128은 Cloud Save 또는 Leaderboards의 확인된 서비스 한도가 아니라 현재 ledger 구현의 고정 제한이다.
- 현재 구현: `query-records.js`는 `offset=0, limit=100`의 전체 단일 페이지 스냅샷만 허용하고, `total > 100` 또는 반환 수와 total이 다르면 `VerificationBoardCapacity`로 실패한다. UI 요청 limit은 1~20이다. 100은 현재 프로젝트 조회 구현의 고정 제한이다. Unity Leaderboards는 offset/limit 페이지 조회와 metadata 포함 조회를 제공하므로, Phase 3에서는 부분 페이지로 전체 순위를 재계산하지 않는 서버 조회 설계와 경계 Test가 필요하다.
- UGS 가능 범위: Cloud Code JavaScript의 `DataApi(context)`는 서비스 토큰으로 Cloud Save와 Leaderboards를 호출할 수 있고, Cloud Save Game Data/Custom Item은 Cloud Code에서 갱신할 수 있다. write lock은 동일 항목의 동시 갱신 충돌을 감지한다. 따라서 서버 측 공개 번호 매핑과 공유 발급 상태의 저장 후보는 존재하지만, 현재 코드에는 없다. 전역 발급 순서의 원자성·재시도 복구는 Phase 2 서버 대역 및 실제 서비스 검증으로 별도 입증해야 한다.
- 환경 범위: Client와 두 Cloud Code 스크립트는 Project ID와 `verification` Environment ID를 고정 대조하며, 명시한 `verification` 환경과 `flow-state-verification` 프로필로만 초기화한다. Production 설정·환경별 계정 관계·원격 정책 실제 적용은 로컬 코드와 공식 문서만으로 확인할 수 없으므로 Phase 4의 실제 설정 검증 대상이다.
- 기존 로컬 검사: Node `v24.21.0`에서 `node UGS/Tests/static-contracts.cjs` 통과, `node UGS/Tests/cloud-code.test.cjs` 21/21 통과, `git diff --check` 통과. 모든 검사는 읽기 전용 파일 또는 로컬 서비스 대역만 사용했으며 Unity Editor, Unity Test Runner, Build, Scene, 원격 UGS는 실행·변경하지 않았다.
- 공식 문서 확인일: 2026-10-01. [Cloud Save와 Cloud Code](https://docs.unity.com/en-us/cloud-save/tutorials/cloud-code), [Cloud Save write locks](https://docs.unity.com/en-us/cloud-save/concepts/write-locks), [Leaderboards 점수 조회](https://docs.unity.com/en-us/leaderboards/tutorials/unity-sdk/get-score), [Leaderboards API](https://docs.unity.com/en-us/oas-leaderboards/1.0.0)를 확인했다. 서비스의 실제 페이지 최대 크기와 현재 Project의 환경·권한·Leaderboard 설정값은 이 정적 대조로 확정하지 않았다.

## Step 2. 공개 번호 발급·표시 정책을 결정한다

### AI 선행 작업

Step 1의 가능한 방안으로 아래 항목별 선택지와 장단점을 제공한다. 내부 UID를 숫자로 변환하는 방안을 서버 발급 공개 번호와 혼동하지 않는다.

### 사용자 수동 작업

1. 번호 발급 시점과 유일성 범위(환경별 또는 환경 간 공유)를 선택한다.
2. 번호 자릿수·허용 범위·선행 0 표시, 번호 재발급·재사용 허용 여부를 선택한다.
3. 기존 계정에 번호를 부여하는 시점과 번호 미발급·조회 실패 시 화면 표시를 선택한다.
4. 전체 번호를 표시할 계정 안내·행 UI와 안내 문구를 확인한다. 본인 행은 `<공개 번호> (You)`이며 번호만으로 계정이 복구되는 것은 아니다.

답변 형식: `발급 시점=…, 유일성 범위=…, 번호 형식=…, 재사용=…, 기존 계정=…, 실패 표시=…`.

### AI 반영·검증

번호 정책의 충돌, 최대값의 직렬화·표시 정밀도, 기존 계정·Pending 귀속 보존을 정적으로 대조한다. 유일성·동시 발급의 성공은 문서 검사만으로 주장하지 않고 Step 7 자동 검증에 연결한다.

### 완료 조건

- [x] 발급·형식·수명·기존 계정·실패 표시 정책이 확정됐다.

### 수행 결과 (2026-10-01)

- 발급 시점: 복구 제한 동의 뒤 Anonymous 인증이 성공하면 서버가 즉시 공개 번호를 발급하거나 기존 번호를 반환한다.
- 유일성 범위: `verification`과 Production은 서로 독립된 번호 공간을 사용한다.
- 번호 형식: `0000000001`~`9999999999`의 10자리 고정 십진 문자열이며 선행 0을 표시한다.
- 재발급·재사용: 공개 번호는 재발급하거나 다른 논리 계정에 재사용하지 않는다.
- 기존 계정: 최초 Anonymous 인증 때 발급하며, 기존 Leaderboard 행이 처음 조회될 때도 서버가 발급해 표시한다.
- 실패 표시: 계정 안내와 Leaderboard 영역은 안전한 Error와 Retry를 제공하고, 내부 Player ID·마스킹 ID·다른 계정 번호로 폴백하지 않는다.
- 추가 확정: 공개 번호는 서버 소유 논리 계정에 귀속한다. 활성 연결·기기 이전 시작·이전 중 잠금·서버 발급 이전 자격 증명·기기 B 연결·이전 완료·취소·만료·분실 기기·Local Save·환경·감사 정책은 `AccountTransfer.md`와 `AccountConnectionSystem.md`에 반영했다.
- 이전 자격 증명 보안: 인증값은 9자리이며 90일 뒤 만료한다. 인증 실패 횟수는 제한하지 않고 같은 이전 요청의 반복 검증은 5초 이내에 허용하지 않는다. 새 코드 발급은 이전 코드와 인증값을 즉시 무효화한다.
- Leaderboard 이전: 기기 이전 때 A→B 기록 이전을 수행하지 않고 계정 C의 고정 Leaderboard 행을 계속 사용한다. 활성 연결 교체 뒤 별도 기록 복구는 수행하지 않으며, 기기 이전은 동점·서버 수락 시각 metadata를 변경하지 않는다.

### 확정 정책 수행을 위한 작업 계획

Step 2의 정책 결정은 완료다. 아래 항목은 확정 정책을 구현·검증하기 위한 작업이며, 각 항목의 상태에서 설계 완료와 구현 완료를 구분한다. 정책의 기준은 `AccountTransfer.md`, 책임의 기준은 `AccountConnectionSystem.md`를 사용한다.

#### 2-1. 계정 저장 구조 설계

환경·논리 계정 C·공개 번호·활성 Player ID·고정 Leaderboard 소유 ID의 관계와 서버 전용 저장 경계를 정의한다. 호출자의 인증 정보에서 C를 찾으며 클라이언트가 임의로 소유 ID를 지정하지 못하게 한다. 기존 Player Protected ledger의 C 귀속 전환도 Step 3 설계에 연결한다.

- 수행 시점: Phase 1 Step 7~8 설계, Phase 2 구현, ledger 수명은 Phase 3
- 상태: 계정 저장 구조 설계 완료. Phase 2 저장·연결 구현과 Phase 3 ledger 수명 구현은 미수행.

##### 현재 구현 대조 결과

- `Assets/Scripts/Runtime/Features/OnlineAccountState.cs`는 동의 여부와 UGS Player ID만 보관한다. `LocalSaveJsonCodec.cs`의 `onlinePlayerId`·`pendingSubmissions`는 로컬 귀속 자료이며 C의 서버 연결을 증명하지 않는다.
- `CloudCodeRecordRepository.cs`는 저장된 Player ID와 인증 결과를 비교한다. 현재 제출 요청에는 별도 C 또는 고정 소유 ID가 없으며, 서버가 호출자를 기준으로 소유자를 판단하는 경계는 유지할 수 있다.
- `UGS/CloudCode/submit-record.js`는 `context.playerId`의 Protected `fs_submission_ledger_v1`을 읽고 쓰며 같은 Player ID에 Leaderboard 점수를 반영한다. `query-records.js`의 내 기록 검색도 `context.playerId`와 행 Player ID를 비교한다. 두 경로 모두 C를 해석하는 과정이 필요하다.
- `UGS/AccessControl/project-policy.json`은 Player의 Cloud Save·Leaderboard 쓰기를 거부한다. Private Game Data 접근과 신규 Cloud Code 권한의 실제 적용은 2-10에서 검증한다.

##### 식별자와 영구 소유 관계

| 구분 | 역할·소유 범위 | 기기 이전 시 처리 |
| --- | --- | --- |
| Project ID + Environment ID | 모든 계정·번호·매핑·ledger의 서버 네임스페이스 | 다른 환경으로 연결하지 않음 |
| accountId (C) | 서버가 관리하는 불변 내부 논리 계정 ID | 유지 |
| publicPlayerNumber | C의 환경 내 유일한 10자리 표시 번호. 인증 수단으로 사용하지 않음 | 유지·재사용 금지 |
| currentPlayerId | C의 현재 활성 UGS 인증 주체 | A에서 B로 교체 |
| leaderboardOwnerId | 신규 C는 서버 생성 불투명 UUID v4, 기존 온라인 C는 최초 전환 때 기존 UGS Player ID 행을 승계. Client 입력과는 동일시하지 않음 | 유지; Cloud Code 서비스 토큰 경로만 점수 쓰기에 사용 |
| Local Save owner / submissionId | 기기 로컬 자료의 귀속과 재시도 중 변하지 않는 제출 ID | B로 자동 복사하지 않음 |

##### 서버 저장 모델

다음 이름은 논리 스키마이며 배포된 키나 API 이름이 아니다. 서버 저장 후보는 Cloud Save Private Game Data/Custom Item이다. 물리 키 구성·샤딩·원자적 생성 방법은 해당 구현 단계에서 SDK 제약과 함께 확정한다.

| 논리 저장 단위 | 키 범위·주요 필드 | 판정 책임 |
| --- | --- | --- |
| Account | 환경 + C: schemaVersion, accountId, publicPlayerNumber, leaderboardOwnerId, currentPlayerId, connectionRevision, status, transfer | 활성 연결과 이전 상태의 기준 본문. 불변 식별자와 변경 가능한 연결 상태를 구분 |
| PlayerBinding | 환경 + 인증 Player ID: accountId, connectionRevision | 호출자로부터 C를 찾는 역방향 매핑. 이것만으로 권한을 부여하지 않음 |
| PublicNumberBinding | 환경 + 공개 번호: accountId | 번호 유일성·비재사용과 기존 번호 조회. 계정 본문과 충돌 시 임의 덮어쓰기 금지 |
| LeaderboardOwnerBinding | 환경 + 고정 소유 ID: accountId | 조회된 행의 공개 번호·본인 여부 판정. 기기 변경으로 재귀속하지 않음 |
| SubmissionLedger | 환경 + C + 제출 ID: payload 판정 자료, status, reason, terminalAt; 진행 중 처리와 Board별 best 참조 | terminal receipt는 180일 중복 판정 뒤 삭제. 진행 중 처리와 Board별 best는 별도 보존 |
| TransferLookup | 환경 + 이전 코드: accountId, 이전 요청 식별 정보 | B가 이전 대상을 찾는 경로. Account의 현재 transfer와 대조하고 별도 인증값을 검증해야 연결 가능 |

Account의 transfer에는 이전 요청 ID, 발급·만료 시각, 인증값 검증용 해시, 사용 상태, 반복 제한·실패 횟수 판정에 필요한 상태를 둔다. 활성 연결 교체와 코드 소진의 최종 판정은 같은 계정 본문에서 이루어지도록 설계하고, 외부 조회 매핑의 오래된 값은 권한으로 인정하지 않는다. 필드 명세와 시간 경계는 2-5~2-6에서 구체화한다.

connectionRevision은 연결 변경 전후를 구분하는 서버 관리 버전이다. writeLock과 별개이며 자체적으로 다중 항목 트랜잭션이나 단일 물리 기기 인증을 보장하지 않는다.

##### 요청과 접근 경계

1. 서버가 실행 context의 Project ID·Environment ID·인증 Player ID를 확인한다. Client의 환경명·계정 ID를 권한 근거로 채택하지 않는다.
2. 일반 제출·개인 조회는 PlayerBinding에서 C를 찾고 Account를 읽는다. 환경·C·현재 Player ID·연결 버전이 맞고 온라인 요청을 허용하는 상태인지 확인한다. 매핑이 없거나 충돌하면 다른 계정을 추정해 처리하지 않는다.
3. 허용된 요청만 C 귀속 ledger와 서버가 읽은 leaderboardOwnerId에 접근한다. Client가 보낸 다른 소유 ID는 쓰기 대상을 바꾸지 못한다.
4. B의 이전 완료 요청은 아직 C의 활성 연결이 아니므로 별도 경로로 인증 Player ID와 이전 코드·인증값을 검증한다. TransferLookup만으로 일반 제출·조회 권한을 부여하지 않는다.
5. 변경 시 계정 상태와 버전을 재검증한다. 검사 뒤 A→B 연결이 바뀌는 경우와 Leaderboard 호출의 경합은 2-7에서 예약·재시도 경계로 검증한다. 사전 검사 한 번으로 해결됐다고 기록하지 않는다.
6. Client 응답에는 공개 번호·안전한 상태·필요한 순위 결과만 제공한다. 소유 ID·매핑·인증값 해시를 UI나 일반 로그에 노출하지 않는다. 코드와 인증값은 확정 정책에 따른 발급 화면에서만 제공한다.

서버 전용 기준 데이터는 Client가 직접 읽거나 쓸 수 없는 경계로 둔다. Local Save의 번호·계정 표시 캐시를 추가하더라도 서버 권한의 근거로 사용하지 않으며, 환경·연결 변경 뒤 다른 계정의 캐시를 사용하지 않도록 2-8에서 연결한다.

##### 저장 일관성과 후속 범위

- Account 본문은 상태 판정의 기준이고 역방향 매핑은 조회 보조다. 매핑만 B로 바뀌었거나 A의 매핑이 남아 있어도 본문과 맞지 않으면 권한을 부여하지 않는다. 중간 상태의 재조회·복구는 2-6에서 구현한다.
- 최초 번호 발급과 여러 매핑의 생성은 이미 존재하는 항목의 writeLock 갱신과 다르다. get-then-create만으로 유일성을 보장하지 않는다. 원자적 최초 발급은 2-2, 연결과 코드 소진의 동시성은 2-6~2-7의 필수 검증 대상으로 남긴다.
- 고정 행 유지와 연결 상태는 같은 C에 귀속되지만 물리적으로 하나의 트랜잭션이라고 가정하지 않는다. 기기 이전에는 점수 복사·삭제가 필요 없으며, 연결 처리의 부분 실패·응답 유실은 여전히 복구해야 한다.
- B가 인증 때 이미 부여받은 별도 계정과 C의 관계, 동일 인증 정보의 복수 기기 사용 범위는 기존 구현 전 구체화 항목으로 유지한다. 이 설계에서 B의 기록 병합·번호 회수 정책을 추가하지 않는다.

##### 기존 ledger의 C 귀속 전환 — Step 3·2-4 인계

1. 기존 환경 + Player ID와 C의 매핑을 확인하고, 원본 Protected ledger를 보존한 상태로 스키마 버전·전환 상태·원본 참조를 기록하는 절차를 준비한다.
2. entries의 제출 ID·payload·확정 상태·거절 사유·acceptedAt, active와 Board별 best를 C 귀속으로 보존한다. 기기 이전을 새 제출로 처리하거나 ID·수락 시각을 재생성하지 않는다.
3. 기존 서버 active 처리와 지연 요청이 있는 동안 복사본만 활성화해 중복 처리를 만들지 않도록 단일 쓰기 경로 전환·재실행 검증을 2-4에 포함한다. 기존 저장소와 신규 저장소를 동시에 독립 판정자로 사용하지 않는다.
4. 128건 한도를 단순 삭제하거나 C 전체의 이력을 무한한 단일 객체에 누적하지 않는다. terminal receipt는 C별 시간 분할로 180일 보관·정리하고, 오래된 Pending은 Client에서 `SubmissionExpired`로 제거한다.
5. Client Pending의 기존 로컬 귀속·제출 ID는 보존한다. C와의 연결 확인 없이 다른 계정으로 재귀속하거나 B로 자동 전송하지 않는다. 기존 서버 ledger 전환은 새 Local Save 이전 기능을 의미하지 않는다.

##### 정적 검증 결과와 완료 판정

- [x] 환경·C·공개 번호·활성 Player ID·고정 소유 ID의 관계와 변경 가능 범위를 정의했다.
- [x] 서버 전용 저장 경계, 인증 context에서 C를 해석하는 순서와 Client 소유 ID 위조 차단 조건을 정의했다.
- [x] 기존 코드의 소유 경로와 변경 지점을 대조하고 ledger 전환을 Step 3·2-4에 연결했다.
- [x] 설계 근거와 후속 구현·원자성 검증 항목을 구분했다. 2-1의 설계 완료 조건을 충족한다.

공식 근거 확인일: 2026-10-01. [Cloud Save Game Data](https://docs.unity.com/en-us/cloud-save/concepts/game-data)는 Private Custom Item의 서버 전용 접근을 명시한다. [Write locks](https://docs.unity.com/en-us/cloud-save/concepts/write-locks)는 기존 항목 충돌 검사와 신규 항목 생성 시 lock 생략을 설명한다. 두 문서는 여러 계정·매핑·Leaderboard 변경을 하나의 트랜잭션으로 처리한다는 근거가 아니다.

현재 수동 작업은 없다. 이번 결과는 저장 구조 설계이며 신규 코드·Test fixture·서비스 설정·Scene 변경을 포함하지 않는다. Unity 컴파일·Test Runner·Build는 2-1 완료에 필요하지 않다. 실제 서비스·동시성 검증은 후속 항목에서 수행한다.

#### 2-2. 공개 번호 발급

Anonymous 인증 직후 발급·기존 번호 반환 경로를 연결한다. 환경별 유일성, 동시 최초 발급, 실패 후 재시도, 번호 소진을 처리한다. 기존 계정 인증과 기존 행 조회가 같은 계정에 서로 다른 번호를 발급하지 않도록 검증한다.

- 수행 시점: Phase 1 순수 정책 Test, Phase 2 발급·표시 통합
- 상태: Phase 1 순수 정책·Node Test 완료. Phase 2 서버 발급·계정 매핑·표시 통합과 실제 서비스 원자성 검증은 미수행.

##### Phase 1 공개 번호 정책 단위

- 공개 번호는 `0000000001`부터 `9999999999`까지의 10자리 문자열이다. 번호 `0`, 음수, 소수, 10자리 초과 값과 안전하지 않은 정수는 발급 번호로 형식화하지 않는다.
- 서버가 이미 발급한 수열 값은 `tryFormatIssuedNumber`으로 문자열화한다. Client·Local Save·UI가 수열 값을 만들어 내거나 문자열 번호를 수치 연산에 사용하지 않는다.
- 서버·Client가 수신한 공개 번호는 `tryNormalizePublicPlayerNumber`으로 정확히 10자리인지 확인한다. 선행 0이 있는 문자열을 정수로 바꾸었다가 다시 채우는 방식으로 원문을 대체하지 않는다.
- `9999999999`는 JavaScript 안전 정수 범위 안에 있다. 이 값 다음에는 새 번호를 할당하지 않고 서버 발급 경로가 명시적인 번호 소진 상태를 반환해야 한다.
- 이 정책 단위는 계정 C의 기존 번호 반환, 번호 유일성, 동시 최초 발급, Player ID·공개 번호 역방향 매핑 또는 저장 실패 복구를 구현하지 않는다. 해당 동작은 서버 저장소와 원자성이 필요하므로 Phase 2 대역·실제 서비스 검증 대상이다.

##### 추가·변경 파일

- `UGS/CloudCode/public-player-number-policy.js`: 10자리 문자열 형식화와 정규화만 담당하는 순수 정책 모듈이다.
- `UGS/Tests/public-player-number-policy.test.cjs`: 최소·최대·선행 0·10자리 형식·안전하지 않은 수·범위 밖 값·비문자열을 Node로 검증한다.
- `UGS/Tests/static-contracts.cjs`: 정책 모듈의 형식 상수·정규화 함수·난수 미사용을 정적으로 확인한다.

##### 검증 결과

```powershell
node UGS/Tests/public-player-number-policy.test.cjs
node UGS/Tests/static-contracts.cjs
node UGS/Tests/cloud-code.test.cjs
```

위 검사는 로컬 Node와 서비스 대역만 사용한다. Unity Editor·Unity Test Runner·Build·Scene·원격 UGS는 사용하지 않는다. 번호 발급의 동시성·서비스 저장 충돌·기존 계정 반환은 이 검사로 통과 처리하지 않는다.

##### 사용자 수동 작업

현재 없음. Phase 2에서 실제 번호 발급·표시 코드와 Unity fixture가 추가된 뒤에만 AI가 Unity 컴파일·Edit Mode 실행 범위와 UI 적용 절차를 제공한다. Scene 변경은 2-2 순수 정책 Test에 필요하지 않다.

#### 2-3. 고정 Leaderboard 행

기기와 무관한 C의 고정 소유 ID를 제출·개인 최고·내 주변·본인 행 판정 경로에 연결한다. UGS가 허용하는 소유 ID의 생성·유지·권한 조건을 실제 SDK·공식 계약으로 확인한다. 임의 C ID를 UGS Player ID로 사용 가능하다고 가정하지 않는다.

- 수행 시점: Phase 1 기술 대조 보완, Phase 2 구현·서비스 검증
- 상태: Phase 1 공식 계약 대조·순수 정책 Test 완료. Phase 2 서버 구현·서비스 검증은 미수행.

##### Phase 1 기술 대조와 확정 경계 (2026-10-01)

- Unity Leaderboards Cloud Code JavaScript SDK는 Cloud Code `context`로 초기화할 때 서비스 토큰의 관리자 권한을 사용한다. 이 경로는 호출자의 Player ID와 다른 Player ID, 즉 서버가 지정한 임의의 Player ID에도 점수를 갱신·조회할 수 있다. 반대로 `accessToken`으로 초기화한 호출자 범위 경로는 호출자의 Player ID로 제한한다.
- 따라서 신규 C의 `leaderboardOwnerId`는 UGS Authentication에서 발급한 Player ID일 필요가 없다. 신규 C에는 서버가 별도로 발급한 UUID v4를 불투명 고정 소유 ID로 저장한다. 공개 번호, `accountId`(C), `currentPlayerId` 및 Client 입력과 동일시하거나 파생하지 않는다. 다만 2-4 최초 전환에서 기존 행을 보존해야 하는 C는 해당 기존 UGS Player ID를 고정 소유 ID로 승계한다.
- 새 소유 ID를 Client가 만들거나 전달하지 않는다. 일반 제출·개인 최고·내 주변·본인 행 판정은 인증 호출자의 PlayerBinding에서 C를 해석한 뒤 Account에서 읽은 `leaderboardOwnerId`만 사용한다. 목록 행의 공개 번호와 `(You)` 판정은 `LeaderboardOwnerBinding`과 현재 C를 대조한다.
- Cloud Code의 서비스 토큰 경로만 고정 소유 ID에 점수를 쓰며, Player의 Leaderboards 직접 Write Deny는 유지한다. 현재 `submit-record.js`·`query-records.js`는 아직 `context.playerId`를 사용하므로 고정 행 서비스 구현은 하지 않았다.
- 실제 서비스가 UUID v4 소유 ID 행을 생성·유지하는지, A→B 뒤 B의 제출이 같은 행을 갱신하는지, A가 차단되는지는 Phase 2의 배포된 Cloud Code 대역·서비스 검증으로 확인한다. 이 Phase 1 대조는 실제 서비스 동작을 주장하지 않는다.

##### 순수 정책 검증

- `UGS/CloudCode/leaderboard-owner-policy.js`는 신규 고정 소유 ID를 UUID v4 문자열만 허용하고 소문자로 정규화한다. 기존 행 승계는 인증된 서버 context의 기존 Player ID와 정확히 같은 값만 허용한다. 이 모듈은 발급·저장·UGS 호출을 수행하지 않는다.
- `UGS/Tests/leaderboard-owner-policy.test.cjs`는 유효 UUID v4, 버전·variant 오류, 공개 번호·C 입력, 인증되지 않은 기존 행 소유 ID를 정적으로 검사한다.
- 실행: `node UGS/Tests/leaderboard-owner-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.
- 근거: [Unity Leaderboards — Access via Cloud Code JavaScript Scripts](https://docs.unity.com/en-us/leaderboards/access-cloud-code). 해당 공식 문서는 Cloud Code `context`의 관리자 수준 접근이 호출자와 다른 임의 Player ID를 갱신할 수 있음을 명시한다.

##### 완료 판정

- [x] 서비스 토큰과 access token의 소유 ID 권한 차이를 공식 계약으로 대조했다.
- [x] 신규 C의 UUID v4 고정 소유 ID와 기존 행의 인증된 Player ID 승계, Client 입력 차단, 행·본인 판정 경계를 확정하고 순수 정책 Test로 검증했다.
- [ ] C 귀속 ledger·제출·조회 코드 전환, 실제 UGS 행 생성·유지와 A→B 서비스 검증은 Phase 2에서 수행한다.

##### 사용자 수동 작업

현재 없음. 이 단계는 공식 계약 대조와 로컬 Node 정적 검증만으로 완료된다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 2 구현 뒤 실제 서비스 검증이 준비되면 테스트 환경에서 A→B 연결과 동일 행 유지 확인 절차를 별도로 제공한다.

#### 2-4. 기존 자료 전환

기존 Player ID별 행·ledger·공개 번호 매핑을 C 중심 구조로 연결하는 최초 전환 절차와 중단·재개·롤백 근거를 준비한다. 기존 행을 고정 소유 행으로 그대로 사용할 수 있는지도 확인한다. 이 작업은 반복되는 기기 이전과 구분하며 기록 삭제·초기화는 수행하지 않는다.

- 수행 시점: Phase 1 Step 5·8 계획, Phase 2 전환 구현, Phase 4 적용·복구 검증
- 상태: Phase 1 전환·중단·재개·롤백 설계와 순수 정책 Test 완료. Phase 2 전환 구현 및 Phase 4 실제 적용·복구 검증은 미수행.

##### 기존 자료와 고정 행의 판정 (2026-10-01)

- 기존 구현은 `submit-record.js`가 `context.playerId`의 Protected `fs_submission_ledger_v1`을 읽고 같은 Player ID에 Leaderboard 점수를 기록한다. `query-records.js`도 행 Player ID와 `context.playerId`를 비교한다. 따라서 기존 온라인 자료가 있는 계정의 행을 새 UUID로 복사하면 중복 행·기록 삭제·수락 시각 재구성이 필요하다.
- 최초 전환 대상은 인증된 현재 Player ID에서 유효한 기존 Protected ledger 또는 Stage·Infinite Leaderboard 행이 확인된 계정이다. 이 기준은 기기 이전이나 새 Anonymous 계정 B의 자료를 합치는 기준이 아니다.
- 대상 C는 기존 `context.playerId`를 `leaderboardOwnerId`로 한 번만 승계한다. 그 결과 기존 Leaderboard 행·점수·동점 metadata·서버 수락 시각은 쓰거나 복사하거나 삭제하지 않고 그대로 C의 고정 행이 된다. 이후 A→B 기기 이전도 이 값을 바꾸지 않는다.
- 기존 온라인 자료가 전혀 없는 신규 C만 서버 생성 UUID v4를 `leaderboardOwnerId`로 사용한다. UUID와 기존 Player ID의 두 소유 ID 종류는 Account에 `ownerKind`(`legacy` 또는 `generated`)와 함께 서버 전용으로 저장한다. Client는 어느 값을 선택·전달·표시하지 않는다.
- 결론: 기존 행은 고정 소유 행으로 **그대로 사용할 수 있다.** 이 방식은 최초 전환에도 Leaderboard 기록 복사·삭제·metadata 변경을 만들지 않으며, 확정한 기기 이전의 무복사 정책과 일관된다.

##### 최초 전환 절차 — Phase 2 구현 명세

1. 인증된 호출자·프로젝트·환경을 확인하고, Account/PlayerBinding이 이미 있으면 기존 C를 반환한다. Client가 C·소유 ID·이전 Player ID를 입력으로 지정할 수 없다.
2. 매핑이 없으면 기존 ledger와 두 Board의 현재 Player ID 행을 읽어 불변 source snapshot(원본 Player ID, ledger 버전·write lock 참조, entries·active·best, Board별 행·metadata)을 기록한다. 원본 ledger와 Leaderboard는 이 단계에서 변경하지 않는다.
3. source snapshot에 기존 온라인 자료가 있으면 `ownerKind=legacy`, `leaderboardOwnerId=sourcePlayerId`를 계획한다. 자료가 없으면 `ownerKind=generated`, 서버 생성 UUID v4를 계획한다. 공개 번호·C·Account·역방향 매핑은 아직 활성 권한이 아니다.
4. C 귀속 destination ledger를 source snapshot의 제출 ID·payload·status·reason·acceptedAt·active·best를 그대로 보존해 작성한다. 원본 Protected ledger는 읽기 전용 원본으로 남기며, 원본과 destination을 동시에 제출 판정자로 사용하지 않는다.
5. Account, PlayerBinding, PublicNumberBinding, LeaderboardOwnerBinding 및 전환 journal을 기록하고 각각 source snapshot·migration ID·스키마 버전과 대조한다. 생성/갱신 충돌은 다른 C를 추정하거나 덮어쓰지 않고 중단 상태로 남긴다.
6. source snapshot, destination ledger, Account와 세 역방향 매핑이 모두 일치할 때만 Account의 `migration.cutoverCompleted`를 설정하고 새 C 경로를 활성화한다. 이후 제출·조회는 C ledger와 고정 `leaderboardOwnerId`만 사용한다.
7. 전환 뒤 source ledger와 기존 Leaderboard 행을 삭제·초기화·재작성하지 않는다. source Player ID는 활성 연결이 아니라 전환 원본 참조로만 보관한다. 기기 이전은 이미 고정된 owner ID를 재승계하거나 변경하지 않는다.

##### 중단·재개·롤백 기준

| 상태 | 서버 동작 | 자료 보존·복구 기준 |
| --- | --- | --- |
| snapshot 전 실패 | 기존 V1 경로 유지 | 원본 ledger·행에 쓰지 않았으므로 재시도 가능 |
| snapshot 후 cutover 전 실패 | C를 활성화하지 않고 전환 journal 유지 | 같은 migration ID·source snapshot으로 멱등 재개. 원본은 계속 V1 단일 판정자 |
| cutover 전 취소/롤백 | V1 경로로 되돌림 | 원본이 변하지 않은 경우만 허용. destination 복사본은 삭제하지 않고 비활성 진단 자료로 보존 |
| cutover 후 실패·응답 유실 | C 경로의 상태 재조회·전방 복구 | V1 원본을 다시 활성화하거나 C 자료를 원본에 역복사하지 않음. 서버 배포 롤백도 C 경로를 읽을 수 있어야 함 |

- `cutoverCompleted` 전의 롤백은 원본 ledger와 기존 행을 변경하지 않았다는 증거가 있을 때만 안전하다.
- cutover 뒤에는 C ledger에 새 제출이 존재할 수 있으므로 데이터 롤백을 허용하지 않는다. 문제 해결은 journal과 source snapshot을 사용한 전방 복구·재배포 호환성으로 제한한다.
- 기존/신규 경로를 동시에 쓰는 기간은 없다. 상태에 따라 단 하나의 활성 제출 판정자만 선택하며, cutover 자체의 다중 항목 원자성은 Phase 2 대역·실제 서비스 검증 전에는 보장되었다고 주장하지 않는다.

##### 순수 정책 검증

- `UGS/CloudCode/legacy-account-migration-policy.js`는 기존 ledger 또는 Board 행이 있으면 인증된 기존 Player ID를 소유 행으로 승계하고, 자료가 없을 때만 생성 UUID를 쓰는 선택 규칙을 검사한다. 또한 모든 destination·매핑 기록 전 cutover 금지와 cutover 전 원본 보존 시에만 롤백 가능함을 판정한다.
- `UGS/Tests/legacy-account-migration-policy.test.cjs`는 기존 ledger/행 승계, 신규 UUID, 불완전 기록의 cutover 거부, cutover 전후 롤백 경계를 Node로 검증한다.
- 실행: `node UGS/Tests/legacy-account-migration-policy.test.cjs`, `node UGS/Tests/leaderboard-owner-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.

##### 완료 판정

- [x] 기존 행을 UUID 행으로 복사하지 않고 인증된 기존 Player ID를 고정 소유 ID로 승계하는 최초 전환 정책을 확정했다.
- [x] 원본 보존, destination ledger 복사, 단일 판정자 cutover, 멱등 재개와 cutover 전·후 복구 경계를 설계하고 순수 정책 Test로 검증했다.
- [ ] 실제 Cloud Save Private 자료·Leaderboard 행·매핑의 원자적 적용, 충돌·응답 유실 복구와 Phase 4 테스트 환경의 전환/호환 배포는 미수행이다.

##### 사용자 수동 작업

현재 없음. 이 단계는 설계와 로컬 Node 정적 검증만으로 완료된다. Unity Editor, Unity Test Runner, Build, Scene 편집 및 UGS Dashboard 변경은 필요하지 않다. Phase 2 전환 코드와 테스트 fixture가 준비된 뒤에만 AI가 테스트 환경의 전환 전 상태 확인, 단일 전환 실행, 재시작·복구 확인 절차를 제공한다. 기존 운영 자료에는 그 전까지 어떤 전환·삭제·초기화도 수행하지 않는다.

#### 2-5. 이전 시작·자격 증명

활성 A와 Pending 조건을 확인하고 서버에서 TransferPending 잠금, 이전 코드·9자리 인증값 발급, 인증값 해시 저장을 처리한다. 서버 시각 기준 90일 만료, 실패 횟수 무제한, 5초 반복 제한, 새 코드 발급 시 이전 자격 증명 무효화를 구현한다.

- 수행 시점: Phase 1 시간·형식 정책 Test, Phase 2 서버 구현
- 상태: Phase 1 시간·형식·잠금·재발급 정책 Test 완료. Phase 2 서버 발급·HMAC 저장·잠금 구현은 미수행.

##### 자격 증명 형식과 비밀값 경계 (2026-10-01)

- 이전 코드는 서버 CSPRNG가 생성한 Crockford Base32 8자리다. 문자 집합은 `0123456789ABCDEFGHJKMNPQRSTVWXYZ`이며, 생성값은 모호한 `I`, `L`, `O`, `U`를 쓰지 않는다. A에는 `ABCD-2345`처럼 4자리씩 표시한다. B 입력은 하이픈 유무·대소문자를 정규화하고 `O→0`, `I/L→1` 별칭을 허용하지만 공백·다른 문자·길이 오류는 거부한다. 공간은 `32^8 = 1,099,511,627,776`개이며 1,000만 동시 활성에서는 무작위 충돌 쌍이 평균 약 45.5개이므로, 서버는 활성 코드 HMAC digest의 환경별 유일성 충돌 시 CSPRNG로 새 코드를 재생성해야 한다.
- 인증값은 정확한 9자리 십진 문자열이다. `000000000`을 포함해 선행 0을 보존하며 수치로 변환하지 않는다. 이전 코드와 인증값은 함께 있어야 하며, 공개 번호·C·Player ID는 이를 대체할 수 없다.
- 서버는 원문 코드·인증값을 Account, TransferLookup, 로그, 일반 오류 또는 고객센터 경로에 저장하지 않는다. 코드 조회에는 서버 보유 비밀을 사용한 HMAC digest를, 최종 검증에는 `transferId`·정규화 코드·인증값을 결합한 별도 HMAC 검증값을 사용한다. 발급 응답 직후 A에 한 번만 원문을 보이고 재조회 응답에는 원문을 돌려주지 않는다.
- 이 형식은 기존 확정 정책의 인증값 9자리·90일·실패 횟수 무제한을 보완한다. 실제 CSPRNG·HMAC 비밀 관리와 저장 API는 Phase 2·2-10의 서버 구현·운영 경계다.

##### TransferPending 시작·재발급 상태 규칙

1. A의 인증 context가 C의 `currentPlayerId`·연결 버전과 일치하고 C가 활성 상태일 때만 이전을 시작한다. A의 Local Save에 Pending이 있으면 Client는 시작 요청을 보내지 않으며, 확정 처리 또는 명시적 폐기 저장 성공 후에만 다시 시도한다. 서버는 Local Save의 미전송 후보를 독자적으로 증명할 수 없으므로 이 Client 저장 조건을 서버 보안 근거로 취급하지 않는다.
2. 서버는 새 `transferId`와 `TransferPending` 상태를 Account 본문에 기록하고, 코드 digest로 찾는 TransferLookup 및 발급 시각·만료 시각·연결 버전·원본 활성 Player ID·자격 증명 HMAC을 같은 이전 요청에 귀속한다. 활성화가 확정되기 전에는 코드·인증값을 응답하지 않는다.
3. `TransferPending`이 되면 일반 제출·조회·추가 **새** 이전 요청을 차단한다. 취소, 만료 처리, B의 완료 요청과 A의 동일 `transferId` 자격 증명 재발급만 허용한다.
4. A가 자격 증명 재발급을 요청하면 서버는 원본 A·같은 `transferId`·미만료 상태를 다시 확인하고, 기존 코드 digest·인증 HMAC을 먼저 비활성화한 뒤 새 자격 증명을 기록한다. 잠금·만료 시각·`transferId`는 유지하며 두 유효 코드가 공존하지 않는다. 만료 뒤에는 먼저 A 연결을 복원한 뒤 새 이전 시작으로 처리한다.
5. 서버 시각에서 `now >= issuedAt + 90일`이면 만료다. 정확히 만료 시각인 요청도 만료로 처리하며, 만료 처리가 이뤄지면 자격 증명을 폐기하고 A 연결을 유지·복원한다.
6. B의 같은 `transferId` 검증 시도는 마지막 검증 시각부터 5,000ms 이상 지난 때만 허용한다. 정확히 5,000ms에는 허용하고, 서버 시각이 과거로 역행하거나 시각 자료가 없으면 거부한다. 실패 횟수는 영구 차단 조건으로 쓰지 않으며 감사용으로만 누적한다.

##### 순수 정책 검증

- `UGS/CloudCode/transfer-credential-policy.js`는 코드·인증값 정규화, 90일 만료의 직전/동일 경계, 5초 제한의 직전/동일 경계, 활성 연결·Pending·재발급 조건만 판정한다. 난수 생성·HMAC·저장·로그·네트워크 호출은 하지 않는다.
- `UGS/Tests/transfer-credential-policy.test.cjs`는 모호 문자·형식 오류·선행 0, 만료와 제한의 경계, 비활성 A·Pending·기존 TransferPending 차단 및 유효 재발급을 Node로 검증한다.
- 실행: `node UGS/Tests/transfer-credential-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.

##### 완료 판정

- [x] 서버 생성 8자리 Base32 코드·9자리 인증값, 1,000만 동시 활성 코드 충돌 재생성, 원문 비저장 HMAC 경계와 B 입력 정규화 정책을 확정했다.
- [x] TransferPending 시작, 동일 요청의 재발급, 90일·5초 정확한 경계와 무제한 실패 정책을 순수 정책 Test로 검증했다.
- [ ] 실제 CSPRNG·HMAC 비밀·Account/TransferLookup 쓰기 잠금, 재발급 원자성·응답 유실과 실제 서비스 시간 검증은 Phase 2·2-6·2-10에서 수행한다.

##### 사용자 수동 작업

현재 없음. 코드·인증값 형식과 시간 경계는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 2 구현 뒤 실제 이전 화면이 준비되면 테스트 환경에서 A의 코드 표시, B의 입력, 재발급으로 이전 값 무효화, 만료·취소 복구 절차를 별도로 제공한다.

#### 2-6. B 연결·취소·만료

B의 Anonymous 인증 뒤 이전 자격 증명을 검증하고 활성 연결 교체·코드 소진을 일관되게 확정한다. 취소·만료 시 A 연결을 유지·복원한다. 두 B의 동시 연결, 완료와 취소·만료 경합, 응답 유실·재시작을 검증한다.

- 수행 시점: Phase 2 서버 대역·실제 서비스 검증
- 상태: Phase 1 상태 전이·경합·재시도 정책 Test 완료. Phase 2 서버 대역·실제 서비스 검증은 미수행.

##### B 대상 계정과 연결 교체 정책 (2026-10-02)

- B는 Anonymous 인증 직후 공개 번호와 임시 논리 계정을 이미 가질 수 있다. C의 이전은 B 계정과 C를 병합하지 않으며, B의 공개 번호·Leaderboard 행·ledger·Local Save Settings·Pending을 C에 복사하지 않는다. 단, 이전 완료 뒤 B의 로컬 개인 최고는 C의 서버 귀속 개인 최고 기록으로 덮어쓴다.
- B의 임시 계정에 서버 온라인 자료가 있어도 C 이전을 승인한다. B의 기존 공개 번호·Leaderboard 행·ledger는 기존 논리 계정에 그대로 남기며 C와 병합·복사·삭제하지 않는다. B의 기존 논리 계정은 B Player ID를 잃어 활성 연결이 없는 상태가 된다.
- B Local Save에 Pending이 있으면 이전 완료를 차단한다. 이전 화면은 `이 기기에 전송 대기 중인 기록이 있습니다. 전송을 완료하거나 폐기한 뒤 계정 이전을 다시 시도하세요.`를 표시하고 C 연결 요청을 보내지 않는다. Pending을 확정 처리하거나 명시적으로 폐기해 0건이 된 뒤에만 다시 시도할 수 있다.
- B Local Save Pending이 없을 때 서버는 B의 Player ID binding을 C로 교체한다. B의 임시 공개 번호는 이미 발급된 번호로 남지만 재사용·C 병합·표시하지 않는다.
- 완료 후 C의 `currentPlayerId`는 B가 되고 `connectionRevision`은 정확히 1 증가한다. `leaderboardOwnerId`, 공개 번호, C ledger, Board 최고 기록·동점·서버 수락 시각 metadata는 바뀌지 않는다. A binding은 비활성화하며 A 요청은 `ActiveDeviceRequired`로 거부한다.

##### terminal 상태와 경합 판정

| 요청 | Pending 상태에서의 서버 조건 | 확정 결과 |
| --- | --- | --- |
| B 완료 | 코드·인증값·환경·만료·5초 제한이 유효하고, B가 적격이며 Account의 예상 `connectionRevision`이 일치 | `Completed`; 코드 소진, A 비활성, B 활성 |
| A 취소 | A가 원본 활성 Player ID이고 같은 `transferId`가 Pending | `Cancelled`; 코드 소진, A 연결 유지 |
| 만료 | 서버 시각이 `expiresAt` 이상 | `Expired`; 코드 소진, A 연결 유지·복원 |

- Account 본문의 `transferId`, `connectionRevision`, `status`와 write lock을 compare-and-set 경계로 사용한다. terminal 상태를 처음 확정한 요청만 연결·자격 증명을 변경한다. 두 B 중 하나가 완료 확정을 선점하면 다른 B는 `Consumed`을 받고 연결되지 않는다.
- 완료와 취소·만료가 경합하면 서버의 terminal write 순서가 결과를 결정한다. 완료가 먼저 확정되면 취소·만료는 B 연결을 되돌리지 않는다. 취소 또는 만료가 먼저 확정되면 B 완료는 C를 연결하지 않는다.
- B 완료 응답이 유실되거나 앱이 재시작해도 같은 B가 같은 자격 증명으로 상태를 재호출하면 `AlreadyCompleted` 결과와 C의 현재 공개 번호만 재확인한다. 다른 B에는 성공 여부를 노출하지 않고 `Consumed` 또는 안전한 실패만 반환한다.
- 코드·인증값이 만료 시각에 도달한 B 완료 요청은 먼저 `Expired` terminal 상태로 확정해 A 연결을 유지·복원한다. `TransferPending`을 단순히 다시 Active로 보인다고 해도 코드·HMAC·lookup은 반드시 비활성 상태여야 한다.

##### 순수 정책 검증

- `UGS/CloudCode/transfer-resolution-policy.js`는 B Local Save Pending 차단, 완료·취소·만료 terminal winner, 완료 뒤 같은 B의 멱등 재호출, 다른 B 차단과 만료 경계를 판정한다. B의 기존 온라인 자료는 완료 차단 조건이 아니다. 실제 HMAC 검증·Account write lock·PlayerBinding 저장·Clock·네트워크 호출은 하지 않는다.
- `UGS/Tests/transfer-resolution-policy.test.cjs`는 B의 기존 온라인 자료 허용·Local Save Pending 차단, 두 B의 완료 경쟁 결과, 완료 후 취소 차단, 취소·만료 결과, 정확한 만료 시각 완료 거부를 Node로 검증한다.
- 실행: `node UGS/Tests/transfer-resolution-policy.test.cjs`, `node UGS/Tests/transfer-credential-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.

##### 완료 판정

- [x] B의 기존 온라인 자료를 병합하지 않고 C 이전을 허용하며, B Local Save Pending만 차단·안내하는 C 연결 교체 정책을 확정했다.
- [x] 완료·취소·만료의 terminal winner, 두 B·응답 유실·재시작·정확한 만료 경계를 순수 정책 Test로 검증했다.
- [ ] 실제 Account·PlayerBinding·TransferLookup의 CAS 순서, B Local Save UI 판정, HMAC·시간 서비스·응답 유실의 원격 서비스 검증은 Phase 2에서 수행한다.

##### 사용자 수동 작업

현재 없음. 상태 전이·경합·만료 경계는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 2 구현 뒤 테스트 환경에서 A→B 완료, B의 기존 온라인 기록이 있어도 이전 허용, B의 Local Save Pending이 있으면 이전 차단, A 취소, 만료 후 A 유지, 응답 유실 재호출 확인 절차를 제공한다. 두 B 경합·90일 대기는 수동으로 요구하지 않는다.

#### 2-7. 활성 연결 강제

모든 온라인 제출·조회에서 활성 연결과 잠금을 확인한다. A가 검사를 통과한 직후 이전이 발생하는 경우에도 오래된 요청이 C를 잘못 갱신하지 않도록 동시성 경계를 설계한다. 고정 행이 유지돼도 연결 교체의 부분 실패·응답 유실 처리는 필요하다.

- 수행 시점: Phase 2, Phase 3 제출·조회 회귀, Phase 4 권한 검증
- 상태: Phase 1 활성 연결·작업 예약·stale 요청 정책 Test 완료. Phase 2~4 구현·서비스 검증은 미수행.

##### 활성 연결 판정과 작업 예약 (2026-10-02)

- 모든 Cloud Code 제출·조회는 `context`의 환경·인증 Player ID에서 PlayerBinding을 찾고 Account를 읽는다. Account의 `status=Active`, `currentPlayerId`, `connectionRevision`과 PlayerBinding의 accountId·연결 버전이 모두 일치할 때만 허용한다. Client가 보낸 C·소유 ID·연결 버전은 권한 근거가 아니다.
- A가 C의 활성 Player ID·연결 버전과 일치하지 않으면 `ActiveDeviceRequired`로 거부한다. Account가 `TransferPending`이면 원본 A라도 일반 제출·조회는 `TransferPending`으로 거부한다. 매핑 누락·충돌은 다른 C를 추정하지 않고 `AccountUnavailable`으로 안전하게 실패한다.
- 점수 제출은 Account에 `onlineOperation`(operation ID, C, A Player ID, connectionRevision, submission ID)을 write lock으로 예약한 뒤 C ledger·고정 Leaderboard 소유 행을 갱신한다. 동일 submission ID의 복구 호출은 같은 예약을 재개할 수 있지만, 다른 제출은 `OnlineOperationInProgress`로 대기한다.
- 이전 시작은 `onlineOperation`과 C ledger의 진행 중 제출이 모두 없을 때만 `Active→TransferPending` compare-and-set을 수행한다. 그러므로 A의 제출이 점수 반영 중이면 B 연결이 C의 활성 연결을 교체할 수 없고, 반대로 `TransferPending`이 먼저 확정되면 A의 새 제출 예약은 생성되지 않는다.
- 조회는 점수·행을 읽기 전 Account snapshot(C, A, connectionRevision)을 잡고, 결과를 반환하기 직전에 Account·PlayerBinding을 다시 대조한다. 그 사이 A→B 연결이 바뀌면 결과를 반환하지 않고 `ActiveDeviceRequired` 또는 안전한 재시도 상태로 끝낸다.
- 제출이 서버 응답 유실 뒤에도 `onlineOperation` 또는 ledger active를 남기면 같은 submission ID의 복구만 예약을 재개할 수 있다. 임의 시간 만료로 다른 A/B 요청에 잠금을 넘기지 않는다. Pending을 확정하거나 명시적으로 폐기한 뒤에만 이전을 시작한다.

##### A→B 경합 순서

| 순서 | 결과 |
| --- | --- |
| A가 제출 예약을 먼저 확정 | B 이전 시작은 `OnlineOperationInProgress`로 보류된다. A가 동일 ID를 확정·복구해 예약과 ledger active를 해제한 뒤 A가 다시 이전을 요청한다. |
| A가 조회 snapshot을 잡은 뒤 B 완료 | B 완료는 연결 버전을 증가시킨다. A는 반환 전 재검증에서 stale 상태가 되어 결과를 반환하지 않는다. |
| A와 B가 동시에 완료/취소/만료 | 2-6의 Account terminal compare-and-set winner만 연결을 변경한다. A의 제출 예약이 있으면 terminal 완료는 시작할 수 없다. |
| B 완료 뒤 지연된 A 제출 | A binding 또는 연결 버전이 불일치하므로 Leaderboard·C ledger 쓰기 전에 `ActiveDeviceRequired`로 거부된다. |

고정 `leaderboardOwnerId`가 기기 이전으로 유지된다는 사실은 오래된 A의 쓰기를 허용하는 근거가 아니다. 쓰기 권한은 고정 행 ID가 아니라 현재 Account 연결·예약으로만 판정한다.

##### 순수 정책 검증

- `UGS/CloudCode/active-connection-policy.js`는 활성 Account·PlayerBinding·연결 버전 대조, 제출 예약·동일 ID 복구·다른 제출 대기, 이전 시작 gate와 조회 후 stale snapshot 거부를 판정한다. 실제 Account/ledger write lock, Cloud Code context, Leaderboard 호출과 응답 유실 복구는 하지 않는다.
- `UGS/Tests/active-connection-policy.test.cjs`는 A의 예약과 B 이전 차단, 동일 요청 재개, ledger 진행 중 차단, B 완료 뒤 A stale 거부, B의 새 연결 허용 및 TransferPending 일반 요청 거부를 Node로 검증한다.
- 실행: `node UGS/Tests/active-connection-policy.test.cjs`, `node UGS/Tests/transfer-resolution-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.

##### 완료 판정

- [x] 활성 연결·PlayerBinding·연결 버전의 서버 권한 경계와 A→B 뒤 오래된 A 요청 거부를 확정했다.
- [x] 제출 예약, 진행 중 ledger와 이전 시작의 상호 차단, 조회 후 stale 재검증을 순수 정책 Test로 검증했다.
- [ ] 실제 Account와 ledger의 write lock 순서, Cloud Code의 예약·복구 구현, Leaderboard 쓰기 직전 재검증 및 Phase 3~4 회귀·권한 검증은 미수행이다.

##### 사용자 수동 작업

현재 없음. 활성 연결·예약·연결 버전 경합은 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 2~4 구현 뒤 테스트 환경에서 A 제출 중 이전 거부, 이전 뒤 A 거부·B 제출, 조회 중 이전의 stale 응답 차단을 자동 대역 Test와 필요한 사용자 확인 절차로 제공한다.

#### 2-8. Client·Local Save 연결

이전 요청·코드 표시·B 입력·취소·만료·활성 기기 아님 UI를 연결한다. A는 이전 완료 뒤 새 Anonymous 계정으로 시작하고 C의 온라인 캐시를 새 계정 것으로 표시하지 않는다. Pending은 확정 처리 또는 명시적 폐기 후에만 이전을 시작하고, 폐기 저장 실패 시 진행하지 않는다. Settings·Pending은 B로 자동 복사하지 않으며, B의 로컬 개인 최고는 이전 완료 C의 서버 귀속 개인 최고 기록으로 덮어쓴다.

- 수행 시점: Phase 2 구현·UI 검증
- 상태: Phase 1 Client·Local Save 상태 분리 정책 Test 완료. Phase 2 Client·UI 구현과 사용자 검증은 미수행.

##### 현재 Client 저장 경로 대조 (2026-10-02)

- `LocalSaveJsonCodec`은 `recoveryNoticeConfirmed`, `onlinePlayerId`, Settings·튜토리얼·개인 최고·`pendingSubmissions`를 단일 로컬 파일에 저장한다. 이 값들은 C의 서버 연결, 공개 번호, 고정 소유 ID 또는 이전 자격 증명을 증명하지 않는다.
- `OnlineRecordCoordinator`는 온라인 인증 성공 Player ID를 `OnlineAccountState.PlayerId`에 저장하고, Submitted/Rejected 응답 뒤에만 해당 Pending을 저장소에서 제거한다. 현재에는 이전 시작·명시적 Pending 폐기·A/B 연결 교체·계정 표시 캐시 API가 없다.
- 따라서 Phase 2는 기존 `onlinePlayerId`를 C 식별자나 이전 권한으로 재활용하지 않는다. 이전 코드·인증값·C 내부 ID·HMAC·TransferPending 상태를 Local Save에 추가하지 않는다.

##### Client 상태와 화면 흐름 정책

1. A 또는 B에서 이전 시작/완료를 누르면 Local Save Pending 개수와 마지막 저장 성공 상태를 먼저 확인한다. Pending이 1건 이상이면 서버 요청을 보내지 않고 `이 기기에 전송 대기 중인 기록이 있습니다. 전송을 완료하거나 폐기한 뒤 계정 이전을 다시 시도하세요.`를 표시한다. 명시적 폐기는 Pending 제거가 Local Save에 성공적으로 기록된 경우에만 완료이고, 저장 실패면 이전을 계속 차단한다.
2. A의 이전 요청이 성공하면 코드·인증값은 현재 화면 메모리에만 표시한다. Local Save·PlayerPrefs·일반 로그·복사 가능한 진단 UI에 저장하지 않는다. 취소·만료·실패·앱 재시작에서는 자격 증명 표시를 지우고 A의 기존 인증·C 표시 캐시는 유지하거나 서버 상태를 다시 조회한다.
3. B는 Anonymous 인증 뒤 코드·인증값을 입력한다. B Local Save Pending이 0건일 때만 완료 요청을 보낸다. B의 기존 온라인 자료가 있어도 서버 이전은 허용하지만 B의 로컬 Settings·튜토리얼·Pending은 C에 병합·복사하지 않는다.
4. B 완료가 서버에서 확인되면 B는 이전 전 임시 계정의 공개 번호·온라인 순위·연결 상태 캐시와 로컬 개인 최고를 먼저 무효화하고, 현재 B Player ID로 C의 공개 번호·활성 연결·순위·서버 귀속 개인 최고를 다시 조회한다. 조회 결과로 B의 개인 최고 전체를 C의 값으로 덮어쓴다. C에 해당 Board 개인 최고가 없으면 B의 해당 값을 비운다. 저장 실패·응답 유실 중에는 이전 B의 개인 최고를 C의 기록으로 표시하지 않고 재조회·저장 재시도를 요구한다.
5. A 완료가 서버에서 확인되거나 A가 `ActiveDeviceRequired`를 받으면 A는 C 표시 캐시·`onlinePlayerId`·로컬 개인 최고를 저장소에서 지운다. Unity Authentication은 `SignOut(true)` 또는 동등한 session token 제거 뒤 새 Anonymous 인증을 수행한다. 세션 토큰을 남긴 단순 SignOut은 이전 A로 재인증되므로 사용하지 않는다.
6. A의 Settings·튜토리얼은 기기 로컬 자료로 유지한다. Pending은 이전 시작 전 0건이므로 B로 복사하지 않는다. 개인 최고만 C의 서버 귀속 값으로 B에 덮어쓰며, B의 이전 개인 최고와 A의 C 반영 개인 최고는 병합·유지하지 않는다.
7. `TransferPending`, `ActiveDeviceRequired`, 코드·인증값 형식 오류, 만료·취소·재발급 결과는 UI 상태로만 보관하고 다음 서버 상태 조회로 갱신한다. UI 캐시가 서버의 연결 권한을 판정하지 않는다.

##### 순수 정책 검증

- `UGS/CloudCode/transfer-client-state-policy.js`는 Pending 0건·저장 성공의 이전 시작 gate, A 완료 뒤 캐시·개인 최고·인증 세션 제거와 새 Anonymous 시작, B 완료 뒤 C 서버 개인 최고로의 덮어쓰기·재조회, Settings·튜토리얼의 로컬 보존을 판정한다. Local Save 파일·Unity Authentication·UI·Scene·네트워크를 호출하지 않는다.
- `UGS/Tests/transfer-client-state-policy.test.cjs`는 Pending/저장 실패 차단, A의 서버 확인 전 세션 유지와 확인 후 새 Anonymous 시작, B의 Pending 0건·C 개인 최고 덮어쓰기, Settings·튜토리얼의 비복사를 Node로 검증한다.
- 근거: [Unity Authentication sessions](https://docs.unity.com/en-us/authentication/session-management)는 `SignOut(true)`가 session token을 삭제해 새 Anonymous Player 생성을 가능하게 하며, 기본 SignOut은 session token을 유지한다고 설명한다.
- 실행: `node UGS/Tests/transfer-client-state-policy.test.cjs`, `node UGS/Tests/active-connection-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.

##### 완료 판정

- [x] A/B의 Pending·저장 성공 gate, 자격 증명 비영속화, A의 새 Anonymous 시작과 B의 C 서버 개인 최고 덮어쓰기·재조회 정책을 확정했다.
- [x] 이전 중 Local Save·인증 세션·로컬 자료의 보존/비복사 경계를 순수 정책 Test로 검증했다.
- [ ] 실제 LocalSaveData 버전·명시적 Pending 폐기 API·Authentication gateway SignOut(true)·이전 UI·오류 화면·서버 상태 조회와 Scene 참조 연결은 Phase 2에서 구현한다.

##### 사용자 수동 작업

현재 없음. Client 상태·저장·세션 경계는 Node 정적 검사와 공식 Authentication 계약 대조로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 2 UI 구현 뒤에만 사용자가 이전 코드 표시·B 입력·Pending 차단 문구·A의 새 Anonymous 시작·B의 C 재조회 흐름을 테스트 환경에서 확인한다. Scene 배치가 필요하면 Hierarchy·Component·Inspector 단위 절차를 그때 제공한다.

#### 2-9. 번호·오류 화면

전체 10자리와 선행 0, 본인 행 `(You)`, 번호만으로 복구 불가 안내를 적용한다. 발급·조회 실패는 해당 계정 안내 또는 목록 영역의 Error와 Retry로 표시하고 내부 ID로 폴백하지 않는다. Offline 플레이가 번호 실패로 차단되지 않음을 확인한다.

- 수행 시점: Phase 2 표시 통합, Phase 5 Player 확인
- 상태: Phase 1 표시·오류 상태 정책 Test 완료. Phase 2 표시 통합·Scene 연결과 Phase 5 Player 확인은 미수행.

##### 표시·오류 계약 (2026-10-02)

- 공개 번호는 서버가 반환한 정확한 10자리 문자열만 표시한다. 선행 0을 제거하거나 수치로 다시 형식화하지 않는다. 유효한 일반 행은 `<공개 번호>`, 현재 C의 행은 `<공개 번호> (You)`다.
- `currentPlayerId`, `leaderboardOwnerId`, `accountId`, 마스킹 ID, B의 이전 전 번호 또는 다른 계정 번호는 계정 안내·Leaderboard 행·오류·일반 로그에 표시하지 않는다. 번호가 누락·형식 오류이면 빈 값이나 내부 ID로 대체하지 않고 해당 영역을 Error로 전환한다.
- 공개 번호 패널의 상태는 `Loading public number...`, `0000000042 (You)`, `Could not load public number.` + Retry다. Leaderboard 목록 번호 해석·조회 실패는 `Could not load leaderboard.` + Retry다. Error 원인에 내부 식별자·HMAC·코드·인증값을 붙이지 않는다.
- 계정·이전 화면은 `공개 번호만으로 계정을 복구할 수 없습니다. 이전 코드와 인증값을 사용하세요.`를 표시한다. 이 안내는 공개 번호 발급 성공 여부와 무관하며, 공개 번호 입력 UI를 복구 수단으로 제공하지 않는다.
- 번호 발급·조회 Error, Loading, Retry 실패 또는 Offline은 Run 시작·Offline 플레이·Local Save 보존을 차단하지 않는다. 온라인 계정·Leaderboard 영역만 해당 Error/Retry 상태가 되며, Retry는 새 번호를 만들지 않고 서버의 기존 C 번호 조회/발급 경로만 다시 호출한다.

##### 현재 구현 대조

- 현재 `GameSystem.OnlinePlayerId`와 `OnlineAccountState.PlayerId`는 UGS 인증 Player ID를 보관하며 공개 표시값이 아니다. `query-records.js`도 내부 `playerId` 행을 반환한다. Phase 2는 Cloud Code가 내부 ID 대신 공개 번호와 현재 행 여부만 Client에 반환하도록 전환해야 한다.
- 현재 Leaderboard/Result UI와 Scene은 기존 Online Player ID·조회 결과 경로를 사용한다. 이번 단계에서는 Scene·TMP Text·Button 또는 Navigation을 수정하지 않는다. 새 패널·행·Retry 연결은 Phase 2 구현 뒤에만 사용자 절차로 제공한다.

##### 순수 정책 검증

- `UGS/CloudCode/public-number-display-policy.js`는 정확한 10자리 공개 번호·`(You)` 표시, Loading·Error·Retry 상태, 복구 불가 안내와 Offline 플레이 허용만 판정한다. 내부 Player ID, UI·Scene·네트워크·Local Save를 사용하지 않는다.
- `UGS/Tests/public-number-display-policy.test.cjs`는 선행 0·본인 행·형식 오류의 Error 폴백, Error Retry, 복구 불가 안내와 Offline 허용을 Node로 검증한다.
- 실행: `node UGS/Tests/public-number-display-policy.test.cjs`, `node UGS/Tests/public-player-number-policy.test.cjs`, `node UGS/Tests/static-contracts.cjs`, `node UGS/Tests/cloud-code.test.cjs`.

##### 완료 판정

- [x] 공개 번호·본인 행·복구 불가 안내, 내부 ID 비표시와 Error/Retry·Offline 비차단 계약을 확정했다.
- [x] 문자열 형식·오류 폴백·Retry·Offline 허용 경계를 순수 정책 Test로 검증했다.
- [ ] 실제 Cloud Code 응답 축소, Client 표시 모델·TMP/버튼/Navigation·Scene 참조 및 Player 화면 확인은 Phase 2·5에서 수행한다.

##### 사용자 수동 작업

현재 없음. 표시 문자열·오류·Retry·Offline 경계는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 2 표시 구현 뒤에만 계정 안내 패널, Leaderboard 행의 `(You)`, Error Retry와 복구 불가 안내를 연결하는 Hierarchy·Component·Inspector 절차를 제공한다.

#### 2-10. 환경·감사·운영

다른 환경의 코드·번호·연결 사용을 차단하고 이전 요청·성공·취소·만료·실패 횟수를 기록한다. 발급 직후 본인에게 보여주는 인증값 외에 비밀값이 로그·일반 오류·고객센터 경로로 유출되지 않도록 확인한다. 실제 서비스 설정과 배포·롤백 절차를 준비한다.

- 수행 시점: Phase 1 Step 5~6, Phase 4 운영 검증, Phase 5 통합
- 상태: Phase 1 환경·감사·배포/롤백 정책 Test 완료. Phase 2 서비스 구현, Phase 4 실제 UGS 적용·운영 검증과 Phase 5 Player 통합은 미수행.

##### 환경·감사·운영 계약 (2026-10-02)

- Account, 공개 번호, 활성 연결, TransferLookup·코드 digest·인증 HMAC과 감사 행의 유일성 경계는 `(Project ID, Environment ID)`다. Cloud Code 실행 context의 Project ID·Environment ID가 Account와 Transfer 저장 행 모두에 정확히 일치할 때만 처리한다. Client가 보낸 환경·C·공개 번호·Leaderboard 소유 ID는 권한 근거가 아니며, 누락·불일치·타 환경 자격 증명은 안전한 일반 실패로 처리한다.
- verification과 production은 별도 계정·번호·이전 자격 증명·감사 자료를 갖는다. 환경 사이의 계정 연결, 코드 검증, 자료 복사·승계, 운영 자료의 verification 조회를 하지 않는다. 기존 verification 환경값이 있는 것은 Phase 2 서비스의 production 준비나 선택을 뜻하지 않는다.
- 감사 이벤트는 `Requested`, `CredentialReissued`, `Completed`, `Cancelled`, `Expired`, `VerificationFailed`이고, 행에는 이벤트·결과·서버 생성 `transferId`·Environment ID·서버 시각·실패 누적 수·안전한 사유 분류만 둔다. 코드 원문, 9자리 인증값, 인증/서비스 토큰, HMAC, digest, 내부 Player ID, 공개 번호와 전체 요청 본문은 감사·일반 로그·오류·고객센터 경로에 남기지 않는다.
- 일반 오류는 코드·인증값의 존재·일치, 다른 B의 완료 여부 또는 C의 연결 상태를 밝히지 않는다. Cloud Code context의 correlation ID는 운영 로그 내부 상관관계 용도이며 Client·고객센터 표시값으로 사용하지 않는다. UGS Cloud Code 로그는 개인정보 장기 저장소가 아니므로 Production 적용 전에 현재 보관 기간과 별도 보관·삭제 필요 여부를 운영자가 확인한다.
- 배포 전에는 대상 환경을 명시하고, 대상 script source·활성 version·입력 정의·Access Control 정책·Secret 이름/권한(값 제외)을 백업한다. Node 검사 통과, Player 직접 Leaderboard/Cloud Save Write 거부 정책 검토, HMAC Secret Manager 접근 확인 뒤 한 환경씩 게시한다. 배포 뒤 익명 테스트 계정으로 자격 증명 비노출과 이전 흐름을 확인한다.
- 롤백은 계정·번호·Leaderboard·Cloud Save를 삭제·초기화·타 환경 자료로 덮어쓰지 않는다. 문제 발생 시 기록한 동일 환경의 직전 Cloud Code version을 재게시하고, 비밀 노출 우려가 있으면 Secret을 회전한 뒤 영향 범위를 검토한다.

##### 순수 정책 검증

- `UGS/CloudCode/transfer-operations-policy.js`는 환경 3자 일치, 허용 감사 필드·일반 오류 금지 식별자와 배포/롤백 사전조건만 판정한다. UGS, Secret, Logger, 네트워크 또는 실제 데이터를 사용하지 않는다.
- `UGS/Tests/transfer-operations-policy.test.cjs`는 같은/다른 환경, 허용·거부 감사 행, 비밀/내부 ID 오류문 차단과 배포/롤백 gate를 Node로 검증한다.
- `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`는 Phase 4 사용자가 실제 UGS에서 수행할 대상 확인·백업·게시·확인·롤백 절차를 분리한다.

##### 완료 판정

- [x] 환경 경계, 최소 감사 필드·비밀값 비노출과 배포/롤백 원칙을 확정했다.
- [x] 환경 일치·감사 출력·일반 오류·배포/롤백 사전조건을 순수 정책 Test로 검증했다.
- [ ] 실제 Cloud Code context 검증·Secret Manager HMAC·logger·저장소 감사, Access Control 적용, 환경별 게시/롤백과 Player 확인은 Phase 2·4·5에서 수행한다.

##### 사용자 수동 작업

현재 없음. 환경·감사·비밀값 경계와 배포 gate는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 4에서만 `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`의 대상 환경 확인, 원격 백업, Secret/권한 확인, 게시·실제 계정 확인 및 롤백 절차를 사용자가 수행한다.


### 구현 전 구체화할 사항

확정 정책을 다시 선택하지 않고, 아래 연결 지점은 Step 7~8에서 설계·검증 대상으로 명시한다. 사용자 경험이나 데이터 보존에 영향을 주는 추가 선택이 필요한 경우 해당 항목만 제시한다.

- B도 인증 직후 번호를 받으므로, B가 기존에 가진 논리 계정·공개 번호·Pending과 C 연결의 관계를 정리한다. 기존 B 기록을 C로 자동 병합하거나 번호를 재사용한다고 가정하지 않는다.
- 인증값의 허용 문자·선행 0, 이전 코드 형식, 5초 제한의 적용 단위와 정확한 경계 판정을 명시한다. 확정한 9자리·90일·실패 횟수 무제한 조건은 유지한다.
- TransferPending 중 추가 이전 요청 금지와 새 코드 발급 시 무효화 조건이 함께 성립하도록 취소 후 재요청 등 발급 경로를 정리한다.
- 활성 Player ID 하나라는 저장 조건과 실제 단일 기기 사용 조건을 구분해 확인한다. 동일 인증 정보가 여러 기기에서 사용되는 경우까지 막아야 하는 연결 검증 범위를 확인한다.
- 계정 C의 행이 유지되므로 기기 이전에 따른 점수 복사·삭제·metadata 복구 작업은 만들지 않는다. 연결 교체 실패의 재시도·상태 확인은 별도 필요 작업으로 유지한다.

### 사용자 수동 작업 시점

- 현재 문서 계획 반영 단계에는 수동 작업이 없다.
- Phase 2 구현 후 AI가 실제 fixture 이름과 실행 범위를 제공하면 사용자가 Unity 컴파일·Test Runner를 실행한다. Scene 참조와 UI 배치가 필요하면 AI가 Hierarchy·Component·Inspector 항목별 적용 절차를 작성하고 사용자가 적용한다.
- 코드 발급·A→B 연결·A 차단·새 Anonymous 시작의 화면 흐름은 구현 후 지정된 테스트 계정과 환경에서 사용자가 확인한다. 정밀한 경합·90일 대기·5초 경계 확인을 수동으로 요구하지 않는다.
- 실제 UGS 설정·게시와 Windows Build·Player 확인은 해당 Phase의 절차가 준비된 뒤 사용자가 수행한다. 이번 계획 갱신은 서비스 적용을 실행하는 단계가 아니다.

### 수행 체크리스트

- [x] Step 2 확정 정책의 수행 작업·책임·수행 Phase를 정리했다.
- [x] 2-1 계정 저장 구조 설계, 2-2 공개 번호 순수 정책 Test, 2-3 고정 행 공식 계약 대조·순수 정책 Test, 2-4 기존 자료 전환 설계·순수 정책 Test, 2-5 이전 자격 증명 정책·Node Test, 2-6 B 연결·terminal 상태 정책·Node Test, 2-7 활성 연결·작업 예약 정책·Node Test, 2-8 Client·Local Save 상태 분리 정책·Node Test, 2-9 공개 번호 표시·오류 상태 정책·Node Test 및 2-10 환경·감사·운영 정책·Node Test를 준비·검증했다.
- [x] 2-10의 기술 대조와 구현 전 구체화 사항을 Step 7~8 인계 자료로 준비했다.
- [x] Step 3 receipt 보관·Pending 만료·재호출 정책의 순수 정책 Test를 작성·검증했다.
- [x] Step 4 조회 규모·동점 경계·스냅샷 갱신·공개 번호 최종 정렬의 순수 정책 Test를 작성·검증했다.
- [x] Step 5 환경 scope·Local Save 격리·legacy migration·Production 테스트 금지·롤백 gate의 순수 정책 Test를 작성·검증했다.
- [x] Step 6 30일 최소 로그 gate·안전한 로그 필드·중간 강도 요청 제한·자동 재시도 금지의 순수 정책 Test를 작성·검증했다.
- [x] Step 7 정책–Test–후속 구현 대응표를 작성·정적 검증했다.
- [ ] Phase 2~5의 서버·Client·서비스·Player 구현 및 검증을 해당 수행 시점에 완료했다.

## Step 3. 서버 중복 제출 정보의 보관·정리 정책을 결정한다

### AI 선행 작업

서버 ledger 보관 방법의 장단점을 제시한다. 보관 정보를 지운 뒤 같은 ID가 다시 도착하는 사례, 서버 반영 후 응답 유실, Local Save 삭제 실패, 오래된 Offline Pending을 포함한다. 서버 중복 판정 정보와 로컬 Pending을 구분해서 설명한다. Step 2의 논리 계정 C 귀속 구조를 기준으로, 기기 이전 뒤에도 중복 판정 이력이 끊기지 않는 저장 경계를 검토한다. 2-1의 ledger 전환 인계를 사용해 기존 entries·active·best 보존과 C 귀속 보관·분할·정리 방안을 제시한다.

### 확정 정책 (2026-10-02)

- 보관 기간: C 귀속 서버 terminal receipt는 `Submitted` 또는 `Rejected`가 확정된 서버 시각부터 정확히 180일 보관한다. 128건은 현재 구현의 고정 제한이므로 Phase 3에서 C별 시간 분할 receipt 저장으로 대체한다.
- 정리 조건: 180일에 도달하면 terminal receipt의 제출 ID·payload 판정 자료·결과·서버 시각·거절 분류를 삭제한다. Account, 공개 번호, 고정 Leaderboard 소유 행, Board 최고 기록과 해당 행의 metadata는 receipt 정리 대상이 아니다.
- 오래된 Pending: Local Save Pending은 생성 시각부터 180일 전까지 재시도한다. 180일에 도달하면 Client가 `SubmissionExpired`를 표시하고 해당 Pending을 제거한다. 이전 Local Save에 생성 시각이 없으면 최초 업그레이드 로드 시각을 저장해 180일 기산점으로 사용한다. Client 생성 시각은 서버 권한이나 기록 진위 검증 근거가 아니다.
- 재호출 응답: receipt가 남아 있는 180일 안에 같은 C·같은 제출 ID·같은 payload는 기존 `Submitted`/`Rejected` 결과를 반환한다. 같은 ID·다른 payload는 `SubmissionIdConflict`로 거부한다. receipt 삭제 뒤 동일 ID는 새 제출로 처리한다.
- 수용한 경계: 180일 뒤 복원된 백업, 오래된 Pending 또는 ID 재사용은 신규 요청과 구분하지 않으며 유효성 검사를 통과하면 새 기록이 될 수 있다. 이 정책은 영구 receipt 저장을 하지 않는 대가로 180일 이후의 정확히 한 번 처리와 변조 ID 판정을 보장하지 않는다.
- 기기 이전: receipt와 Pending의 180일 경계는 C 귀속 제출에 적용한다. B Local Save Pending은 이전 전 0건이어야 하며, A/B의 기기 로컬 Pending을 자동 이전하지 않는다.

### AI 반영·검증

- `UGS/CloudCode/submission-retention-policy.js`는 180일의 정확한 경계, Pending 만료와 receipt 존재·payload 일치 여부에 따른 기존 결과·충돌 거부·새 제출 판정만 수행한다. 실제 저장소 삭제·C 분할·시계·네트워크는 구현하지 않는다.
- `UGS/Tests/submission-retention-policy.test.cjs`는 만료 직전/동일 시각, Pending 만료, 180일 안 같은/다른 payload와 정리 뒤 신규 제출을 Node로 검증한다.
- 실제 Phase 3 구현은 Client candidate와 Local Save에 생성 시각을 version migration으로 추가하고, 서버가 C별 terminal receipt를 180일 뒤 원자적으로 정리한 뒤에만 신규 ID처럼 처리한다. 서버 시각은 terminal receipt 보관 판정에 사용하며 Client 시각은 Local Pending UI 정리에만 사용한다.

### 완료 조건

- [x] 128건 제한을 대체할 180일 receipt 보관·정리 정책과 오래된 Pending·재호출 기준을 확정했다.
- [x] 만료 경계, 동일 ID 결과 복구·payload 충돌과 정리 뒤 신규 처리의 순수 정책 Test를 작성·검증했다.
- [ ] C별 receipt 분할 저장, Client 생성 시각 migration, 실제 정리 원자성·응답 유실·기기 이전 경합은 Phase 3 구현·서비스 대역 검증에서 수행한다.

### 사용자 수동 작업

현재 없음. 180일 경계와 재호출 판정은 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 3 구현 뒤에만 사용자는 migration된 Pending의 `SubmissionExpired` 안내와 180일 내 응답 유실 재호출을 테스트 환경에서 확인한다.

## Step 4. 순위 조회·페이지·동점 경계 정책을 결정한다

### AI 선행 작업

현재 상위·내 주변 목록과 전체 순위 계산을 설명하고, 표시 건수·페이지 제공 방안의 장단점을 제시한다. 공개 번호가 표시용인지 동률 정렬에도 사용되는지 구분한다.

### 확정 정책 (2026-10-02)

- 상위 건수는 10건, 내 주변 건수는 7건이다. 두 영역은 서로 독립적으로 Loading·성공·Empty·Offline·Error 상태를 가진다.
- 사용자 페이지 UI는 제공하지 않는다. 서버는 대규모 Board에서 상위·내 주변을 얻기 위한 내부 offset/cursor 페이지를 사용할 수 있지만, Client는 Previous/Next·페이지 번호·임의 순위 탐색을 제공하지 않는다.
- 동점 그룹은 상위 10건 또는 내 주변 7건 경계에서 분리할 수 있다. 분리된 모든 행은 서버가 계산한 전역 competition rank를 유지한다. Client는 부분 목록에서 rank를 재계산하지 않는다.
- 한 조회 응답은 스냅샷으로 표시한다. 최초 진입, 명시적 Retry·새로고침 또는 Leaderboard 재진입만 새 조회를 시작한다. 타이머·제출 완료만으로 자동 갱신하지 않는다.
- Stage Clear Time 또는 Infinite Score와 서버 수락 시각이 모두 같으면 정확한 10자리 공개 번호 오름차순으로 표시한다. 공개 번호는 최종 표시 순서에만 사용하며 권한·복구·기록 소유 근거가 아니다.

### AI 반영·검증

- `UGS/CloudCode/leaderboard-query-policy.js`는 상위 10·내 주변 7, 사용자 페이지 UI 부재, 동점 분리·전역 rank 유지, 명시적 스냅샷 갱신과 정확한 공개 번호 최종 정렬만 판정한다. 실제 Leaderboards 조회·cursor·순위 계산·UI·Scene을 사용하지 않는다.
- `UGS/Tests/leaderboard-query-policy.test.cjs`는 목록 크기, 자동 갱신 금지, 동점 경계 표시와 공개 번호 정렬의 형식·순서를 Node로 검증한다.
- Phase 3 구현은 100명 초과 Board에서 서버가 전역 rank를 계산·반환하고, public number를 행에 결합한 뒤 Score·acceptedAt·public number 순서로 정렬해야 한다. partial 페이지로 전역 rank를 재계산하거나 내부 Player ID를 Client 응답·표시 순서 근거로 반환하지 않는다.

### 완료 조건

- [x] 조회 규모 확대 후에도 기존 competition rank 계약을 유지하는 10/7 표시·페이지·동점·갱신·최종 정렬 정책을 확정했다.
- [x] 목록 크기, 스냅샷 갱신과 공개 번호 최종 정렬의 순수 정책 Test를 작성·검증했다.
- [ ] 실제 cursor/offset 조회, 100명 초과 전역 rank, 번호 결합·응답 축소, Client 표시 모델·UI/Scene 연결과 서비스 검증은 Phase 3·5에서 수행한다.

### 사용자 수동 작업

현재 없음. 조회 정책 경계는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 3·5 구현 뒤에만 사용자는 상위 10·내 주변 7, 동점 경계의 같은 rank, 명시적 새로고침과 공개 번호 최종 순서를 테스트 환경에서 확인한다.

## Step 5. 환경과 기존 데이터 전환 정책을 결정한다

### AI 선행 작업

UGS의 실제 환경별 격리 범위를 근거로 계정·공개 번호·Board·Local Save·Pending의 소속을 표로 제시한다. 환경명이 다르다는 이유만으로 모든 인증 데이터가 분리된다고 가정하지 않는다.

### 확정 정책 (2026-10-02)

- 데이터 이전: verification의 C·공개 번호·Leaderboard·terminal receipt·Pending은 Production으로 이전·복사하지 않는다. 기존 verification 자료는 삭제·초기화하지 않으며, Production은 새 C·새 공개 번호·새 기록으로 시작한다.
- 테스트 데이터: 운영 테스트 계정·기록은 verification에만 둔다. Production에는 출시 전 테스트 계정·기록을 만들지 않고 일반 사용자 기록만 사용한다.
- Client 환경: 사용자 환경 전환 UI를 제공하지 않는다. UGS 환경은 서버가 Client에 강제 전환하는 값이 아니라 빌드의 명시적 초기화 구성이다. verification용 빌드와 Production용 빌드는 각각 하나의 대상 환경으로만 초기화한다.
- Local Save: Settings·튜토리얼·입력 설정은 기기 공용으로 유지한다. 인증 Player ID·공개 번호 캐시·개인 최고·Pending은 `(Project ID, Environment ID)`별 Local Save 영역에 분리한다. 활성 환경과 다른 영역의 온라인 자료는 조회·전송·병합·자동 재귀속하지 않는다. 기존 환경 표식 없는 온라인 자료는 현재 배포된 verification 자료로 migration한다.
- 롤백·승인: 배포 전 같은 대상 환경의 Cloud Code source·활성 version·입력 정의·Access Control·Secret 이름/권한을 백업한다. 문제 시 동일 환경의 직전 Cloud Code version만 재게시하며, 계정·번호·Leaderboard·receipt·Pending은 삭제·초기화·다른 환경 자료로 덮어쓰지 않는다. 1인 개발자인 사용자가 verification 확인과 대상 환경 백업 뒤 배포를 명시적으로 승인한다.

### AI 반영·검증

- `UGS/CloudCode/environment-data-policy.js`는 Project/Environment 온라인 scope의 유효성·동등성, 다른 scope Local Save 사용 차단, 기존 표식 없는 자료의 verification 귀속, 환경 간 복사 금지와 Production 테스트 자료 금지만 판정한다. 빌드 구성·Unity Authentication·파일 I/O·UGS 호출을 하지 않는다.
- `UGS/Tests/environment-data-policy.test.cjs`는 scope 일치/불일치, verification→Production 차단, legacy 자료의 verification migration, 환경 간 복사·Production 테스트 금지를 Node로 검증한다.
- Phase 2~4 구현은 온라인 Local Save에 scope를 직렬화·migration하고, 빌드 구성에서 대상 Environment ID·프로필·Board를 명시한다. Production UGS 실제 생성·게시·롤백은 Phase 4의 사용자 승인 뒤 수행한다.

### 완료 조건

- [x] verification 비이전·비삭제, verification 전용 테스트, 빌드 고정 환경과 Local Save 격리, 동일 환경 롤백·사용자 승인 정책을 확정했다.
- [x] 환경 scope·legacy migration·환경 간 복사 금지·Production 테스트 금지의 순수 정책 Test를 작성·검증했다.
- [ ] 실제 빌드 구성 분리, Local Save scope migration, Production 환경·Board·Access Control·Secret 설정, 원격 게시/롤백과 Player 확인은 Phase 2·4·5에서 수행한다.

### 사용자 수동 작업

현재 없음. 환경·자료 격리와 롤백 gate는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 4에서만 사용자는 verification 확인, 동일 대상 환경의 원격 백업, 명시적 배포 승인과 필요 시 같은 환경 버전 롤백을 수행한다.

## Step 6. 운영 관측·장애 대응·부정행위 방지 범위를 결정한다

### AI 선행 작업

기존 서버 검증 수준과 추가 가능한 검증을 구분한다. 오류 코드·민감정보 제외·요청 제한·장애 복구 방안 및 비용/운영 부담을 제시한다. 점수 상한 검사만으로 모든 부정행위를 방지한다고 설명하지 않는다.

### 확정 정책 (2026-10-03)

- 부정행위 방지 범위: Prototype 8 필수 범위는 Cloud Code 서버 입력 검증, Player 직접 Leaderboard/Cloud Save Write 차단, C 활성 연결, 제출 ID·Board·Version·구성 합계·점수 상한 검증이다. 서버 권위 Run 시뮬레이션·입력 이벤트 재현·자동 계정 정지는 후속 범위다.
- 관측·보관: 환경·script version·안전한 outcome/reason 분류·처리 시간 구간·서버 correlation ID·요청 제한 결과만 가진 최소 구조화 로그를 30일 보관한다. 공개 번호·내부 Player ID·이전 코드·인증값·토큰·HMAC·digest·payload 원문은 로그에 넣지 않는다. 30일보다 긴 archive는 만들지 않는다.
- 30일 저장 경계: UGS Cloud Code 기본 로그 보관이 30일보다 짧으면 Production 적용 전에 개인정보·비밀값을 받지 않는 최소 외부 로그 sink와 정확한 30일 삭제 절차를 준비해야 한다. 이 준비가 없으면 Production 배포 gate는 `Blocked`다.
- 대응 담당·처리: 사용자 1인이 수동으로 대응한다. Timeout·서비스 오류는 Pending 보존과 기존 제한 재시도로 처리한다. 데이터 정합성·비밀 노출·보안 이상은 배포 중단, 동일 환경의 이전 Cloud Code version 재게시, 필요 시 Secret 회전 순서로 처리한다. 계정·번호·Leaderboard·receipt·Pending은 삭제·초기화하지 않는다.
- 요청 제한: C·목록 종류별 조회는 5초에 1회, C별 신규 제출은 60초에 최대 3회, 동일 제출 ID 재호출은 5초 간격이다. 이전 자격 증명 검증은 이미 확정한 5초 제한·실패 횟수 무제한을 유지한다. 제한 초과는 `TooManyRequests`와 안전한 재시도 시각으로 응답하고 Client는 자동 재시도하지 않는다.

### AI 반영·검증

- `UGS/CloudCode/operational-safety-policy.js`는 30일 로그 저장 gate, 안전한 로그 필드, 5초·60초/3회 경계와 `TooManyRequests`·자동 재시도 금지만 판정한다. Logger·외부 sink·rate-limit 저장·UGS 호출은 구현하지 않는다.
- `UGS/Tests/operational-safety-policy.test.cjs`는 7일 기본 로그의 30일 gate 차단, 금지 식별자 로그 거부, 제한 직전/동일 경계, 제한 응답과 자동 재시도 금지를 Node로 검증한다.
- Phase 2~4 구현은 C 귀속 서버 rate-limit 상태의 원자성, Client의 `TooManyRequests` 표시·자동 재시도 제외, 실제 구조화 logger와 최소 30일 sink·삭제, Access Control의 원격 강제력과 롤백/Secret 회전을 검증한다.

### 완료 조건

- [x] 부정행위 방지 포함/제외 범위, 30일 최소 로그·비밀값 제외, 1인 수동 대응과 중간 강도 요청 제한을 확정했다.
- [x] 로그 보관 gate·필드 제외·요청 제한·자동 재시도 금지의 순수 정책 Test를 작성·검증했다.
- [ ] 실제 서버 rate-limit 상태·logger·30일 sink/삭제·Access Control·배포 중단/롤백/Secret 회전과 Player 확인은 Phase 2·4·5에서 수행한다.

### 사용자 수동 작업

현재 없음. 운영 정책 경계는 Node 정적 검사로 처리했다. Unity Editor, Unity Test Runner, Build, Scene 편집 또는 UGS Dashboard 변경은 필요하지 않다. Phase 4에서만 사용자는 30일 최소 로그 sink·삭제 절차 준비 여부를 확인하고, verification 장애 대응·동일 환경 롤백·Secret 회전 절차를 검토한다.

## Step 7. 결정 사항을 문서와 Unit Test 계약으로 고정한다

### AI 작업

1. Step 2~6의 선택을 관련 Feature·System·Roadmap에 반영한다. 현재 구현과 향후 목표를 구분하고 아직 구현하지 않은 기능을 완료로 적지 않는다.
2. 결정 항목 → 계약 문서 → 실제 검증 대상 → Test 사례 → 수행 Phase를 대응시킨다. 아래 표는 작성할 사례이며 새로운 fixture가 이미 존재함을 뜻하지 않는다.
3. Phase 1에서 검증할 수 있는 확정 정책은 실제 재사용할 순수 정책 단위와 Unit Test로 준비한다. 아직 없는 서버 기능을 Test 내부에 복제해 통과시키지 않는다. 통합 구현이 필요한 사례는 후속 Phase의 필수 Test로 명시한다.

| 검증 대상 | 자동 검증 사례 | 수행 위치·시점 |
| --- | --- | --- |
| 공개 번호 형식 | 최대 자릿수·선행 0·왕복 직렬화·정밀도·전체 표시 | 정책 확정 후 Phase 1 순수 정책 Test, 표시 통합은 Phase 2 |
| 발급 유일성 | 동일 계정 재요청·다른 계정 동시 발급·저장 중단 후 복구 | Phase 2 서버 대역 Test, 서비스 원자성은 별도 통합 검증 |
| 이전 자격 증명 정책 | 9자리 검증, 90일·5초 직전/동일/직후, 실패 횟수로 영구 차단하지 않음 | Phase 1 순수 정책 Test; 가짜 시계 사용, 서버 제한 통합은 Phase 2 |
| 이전 시작·Pending | 비활성 A 거부, Pending 존재 시 차단, 명시적 폐기 저장 실패, 잠금 뒤 제출·조회 차단 | Phase 2 서버 대역·Client 저장소 대역 Test |
| 연결 교체 | 두 B의 동시 요청, 완료/취소/만료 경합, 코드 재사용·재발급, 서버 확정 후 응답 유실·재시작 | Phase 2 서버 대역 Test 및 실제 원자성 검증 |
| 고정 소유 행 | 이전 전후 공개 번호·소유 행·최고 기록·수락 시각 동일, B의 후속 제출도 동일 행, 중복 행 없음 | Phase 2 생산 제출·조회 경로 대역 Test, Phase 3 순위 회귀 |
| 이전 뒤 A와 B | A의 지연 요청 거부, A 새 Anonymous 시작, C 캐시 혼합 방지, B Local Save 자동 이전 없음 | Phase 2 Edit Mode·필요한 Play Mode Test 및 사용자 UI 확인 |
| 기존 자료 전환 | 기존 행·ledger 귀속 유지, 전환 재실행·부분 실패·롤백, 이전 중 기록 복사·삭제 호출 없음 | Phase 2 전환 대역 Test, Phase 4 실제 설정·보존 검증 |
| ledger 수명 | 기간 직전/동일/직후, 오래된 Pending, 동일 ID·변조 payload | Phase 1 기간 정책 Test, 실제 정리·저장은 Phase 3 |
| 중복·부분 실패 | 응답 유실·삭제 저장 실패·동시 Retry·128건 초과 | 기존 OnlineRecordRepositoryTests 및 Phase 3 서버 대역 확장 |
| 순위 | 100명 초과·페이지 경계 동점·수락 시각 동률·내 주변 경계 | 기존 RecordLeaderboardPolicyTests 및 Phase 3 조회 Test 확장 |
| 환경 격리·운영 | 환경/계정 불일치 차단·권한 거부·rate limit·Timeout·롤백 호환 | Phase 4 정책·서버 대역 및 실제 설정 검증 |

##### Phase 1 정책–Test–후속 구현 대응표

이 표는 각 확정 정책이 문서화된 위치, 현재 정적으로 검증하는 Node Test, 실제 구현과 통합 검증을 수행할 후속 Phase를 한 행에 연결한 대응표다. 따라서 표의 항목만으로 기능 구현이 완료되었다는 뜻은 아니며, 후속 Phase에서 표에 적힌 서버·서비스·Player 검증을 별도로 수행한다.

계약 문서의 위치: 계정·공개 번호·이전·Local Save·환경·감사는 `AI/03_Features/AccountTransfer.md`와 `AI/02_Systems/AccountConnectionSystem.md`, receipt·Pending 만료는 `AI/03_Features/RecordSubmission.md`와 `AI/02_Systems/RecordSubmissionSystem.md`, 조회·동점은 `AI/03_Features/Leaderboard.md`를 기준으로 한다. 환경·운영 절차는 `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`, 후속 Phase의 범위와 완료 조건은 `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md`에 연결한다. 아래 Node Test 파일은 모두 `UGS/Tests/`에 있다.

아래 Test는 외부 서비스·Unity·Scene을 사용하지 않는 순수 정책 또는 읽기 전용 계약 검사다. 통합 구현을 대체하지 않으며, 각 후속 Phase에서 표기한 대역·서비스·Player 검증을 별도로 수행한다.

| 확정 계약 | Phase 1 Node Test | 후속 필수 검증 |
| --- | --- | --- |
| 10자리 공개 번호·선행 0·범위 | `public-player-number-policy.test.cjs` | Phase 2 원자 발급·기존 C 재조회·표시 통합 |
| 고정 Leaderboard 소유 ID | `leaderboard-owner-policy.test.cjs` | Phase 2 C 귀속 쓰기·기존 행 승계·Phase 3 조회 |
| 기존 자료 cutover | `legacy-account-migration-policy.test.cjs` | Phase 2 원자 cutover·부분 실패·Phase 4 보존 |
| 이전 코드·9자리 인증값·90일·5초 | `transfer-credential-policy.test.cjs` | Phase 2 CSPRNG·HMAC Secret·저장 잠금·실제 서버 시간 |
| B 연결·terminal winner | `transfer-resolution-policy.test.cjs` | Phase 2 두 B 경합·응답 유실·실제 Account/Binding CAS |
| 활성 연결·제출 예약 | `active-connection-policy.test.cjs` | Phase 2 예약/연결 교체 원자성·Phase 3~4 회귀 |
| A/B Local Save·개인 최고 | `transfer-client-state-policy.test.cjs` | Phase 2 Local Save migration·Authentication/UI·Player 확인 |
| 공개 번호 표시·오류 | `public-number-display-policy.test.cjs` | Phase 2 응답 축소·Client 모델·Phase 5 Scene/Player |
| 환경·감사·배포/롤백 gate | `transfer-operations-policy.test.cjs` | Phase 2 context/Secret/logger·Phase 4 원격 게시/롤백 |
| receipt 180일·Pending 만료 | `submission-retention-policy.test.cjs` | Phase 3 C별 분할·정리 원자성·응답 유실 |
| 상위 10·주변 7·동점/스냅샷 | `leaderboard-query-policy.test.cjs` | Phase 3 100명 초과 전역 rank·Phase 5 표시 |
| 빌드 환경·온라인 Local Save scope | `environment-data-policy.test.cjs` | Phase 2 scope migration·Phase 4 Production 설정 |
| 30일 로그·요청 제한·수동 대응 | `operational-safety-policy.test.cjs` | Phase 2 서버 rate limit·Phase 4 30일 sink/삭제·롤백 |
| 파일·식별자·정책 연결 | `phase1-policy-contracts.test.cjs`, `static-contracts.cjs` | Phase 2~5 구현별 대역·서비스·Player 검증 |

4. 외부 서비스와 실제 저장소를 사용하지 않는지 확인하고 Node 검사·서버 대역 Test는 AI가 실행한다. 시간 경계·경합은 가짜 시계와 제어 가능한 대역으로 검사한다.
5. Unity 코드가 변경됐다면 실행할 실제 fixture와 영향받는 회귀 범위를 사용자에게 제시한다. 문서만 바뀌었다면 Unity 실행을 요구하지 않는다.

### 사용자 수동 작업

현재 없음. Step 7은 문서·순수 Node 정책 Test의 인계 대조이며 Unity 코드·Scene을 변경하지 않는다. Unity Editor, Unity Test Runner, Build 또는 UGS Dashboard 확인을 요구하지 않는다. Phase 2~5 구현에서 C# 또는 Scene이 실제로 변경된 경우에만 AI가 정확한 fixture·Hierarchy·Component·Inspector 절차를 제공한다.

### 완료 조건

- [x] 확정 정책이 Feature·System·Project·Roadmap·Task 문서에 반영됐다.
- [x] Phase 1 정책 Test와 파일·문서·후속 Phase 인계 매핑을 정적 계약 검사로 검증했다.
- [x] Phase 2~5의 미구현 Test 위치·시점·기대 결과를 대응표에 명시했다.

## Step 8. Phase 1 완료를 판정하고 Phase 2에 인계한다

### AI 작업

Step 1~7 근거를 Roadmap 008 Phase 1의 세 완료 조건과 대조한다. 미정 정책·검증 실패가 남으면 해당 Step을 미완료로 유지한다. 조건 충족 시 Task·Roadmap·Project Memory를 갱신하고 Phase 2의 구현·Scene·서비스 작업을 별도 계획으로 연결한다.

Step 2의 2-1~2-10과 Step 3~6을 인계 목록에 포함하고, 각 항목의 변경 대상·설계 근거·자동 Test 위치·사용자 적용 시점·미확인 서비스 제약을 연결한다. 기존 Step 1의 대조만으로 이후 추가된 논리 계정·고정 행·기기 이전의 구현 가능성과 원자성을 검증했다고 처리하지 않는다.

### 사용자 수동 작업

없음. 이미 답한 정책을 다시 승인할 필요는 없다. 새로운 정책 선택이 필요한 경우에만 해당 항목을 제시한다.

### Phase 1 판정 및 Phase 2 인계

Roadmap 008의 Phase 1 완료 조건을 다음처럼 판정한다.

| 완료 조건 | 판정 근거 | 판정 |
| --- | --- | --- |
| 미정 정책 결정 및 계약 문서 반영 | Step 2-1~2-10, Step 3~6의 확정 정책을 Feature·System·Project·Roadmap·Task 문서와 정책 Test에 반영했다. | 충족 |
| 기존 UGS 범위와 추가 제약 구분 | UGS Authentication·Cloud Code·Cloud Save·Leaderboard·Access Control을 계속 사용한다. 그러나 C별 원자 저장/연결 교체, CSPRNG·HMAC Secret, 180일 receipt 정리, 100명 초과 전역 순위, 환경별 Local Save, 30일 로그 sink는 기존 구현에 없으며 Phase 2~4에서 구현·실서비스 검증이 필요하다. | 충족 — 범위와 제약을 구분했으며 구현 완료를 주장하지 않음 |
| 강화된 서버 검증 범위와 완료 조건 | 이번 범위는 서버 입력 검증·직접 Write 차단·활성 연결·제출 ID·점수 상한 검증이다. Server Authoritative Run·자동 부정행위 판정은 범위 밖으로 보류한다. 후속 완료 조건은 Cloud Code와 Access Control이 이 경계를 실제 운영 환경에서 유지함을 검증하는 것이다. | 충족 |

Phase 2는 다음 순서로 인계한다. 서비스·Scene·Unity 통합은 아직 수행하지 않는다.

1. C, 고정 Leaderboard 소유 ID, 10자리 공개 번호의 원자 발급·기존 자료 cutover를 Cloud Code 저장소에 구현하고 동시 요청·부분 실패 대역 Test를 작성한다.
2. 8자리 Base32 이전 코드, 9자리 인증값, 90일 만료, 5초 제한, 단일 활성 연결 교체를 CSPRNG·HMAC Secret·서버 시간·CAS로 구현하고 두 B 경합·응답 유실을 검증한다.
3. `(Project ID, Environment ID)`별 Local Save migration과 이전 완료 뒤 A/B 상태·개인 최고 기록 규칙을 구현한다. Scene을 변경해야 할 경우에는 별도 Step에서 사용자에게 Hierarchy·Component·Inspector 절차를 제공한다.
4. 공개 번호 표시·오류 모델과 `(You)` 표시를 Client에 연결한다. 실제 Unity 컴파일·Test Runner·Player/화면 확인은 사용자 책임의 후속 Phase 검증으로 남긴다.

### 완료 조건

- [x] 미정 정책이 결정되고 관련 계약 문서에 반영됐다.
- [x] 기존 UGS로 충족 가능한 범위와 추가 제약이 구분됐다.
- [x] 강화된 서버 검증의 필요성·범위·완료 조건이 정해졌다.
- [x] Phase 1 정책 검증 결과와 Phase 2~5 인계 항목이 기록됐다.

# 영향 범위

이번 Step 8 갱신은 Roadmap 008 Phase 1 완료 조건과 Step 1~7 근거를 대조하고, 정책 완료와 실제 구현·운영 검증 미수행 상태를 분리해 Phase 2~5에 인계한 문서 작업이다. 실제 Cloud Code·Client·Scene·원격 서비스 구현 또는 Unity 실행은 변경·수행하지 않았다.

# 검증 내용

Roadmap 008의 세 Phase 1 완료 조건을 Step 1~7의 정책 문서·정책 Test·인계 대응표와 대조한다. 정책 결정 완료·구현 미수행 상태 및 AI/사용자 책임을 분리하고, 로컬 참조 경로와 문서 변경의 공백 오류를 정적으로 확인한다.

# 검증 결과

### Phase 1 범위 재점검 (2026-10-03)

- Step 1~8의 Phase 1 완료 상태를 재확인했다. 미완료 체크 항목은 Phase 2~5의 실제 구현·서비스·Unity 통합 검증이며 이번 작업 범위에 포함하지 않는다.
- Roadmap의 보류 범위를 외부 ID 연결·자격 증명 없는 분실 기기 복구로 바로잡고, Anonymous 계정의 코드 기반 기기 이전이 확정 범위임을 명시했다.
- Step 2-6 후속 수동 안내를 B의 기존 온라인 기록은 허용하고 Local Save Pending만 차단하는 확정 정책과 일치시켰다. Step 7 대응표에는 계약 문서 경로와 Node Test 기본 경로를 추가했다.
- 정책 Node Test 13개, 인계 계약 검사, 정적 계약 검사, 기존 Cloud Code 대역 Test 21/21 및 `git diff --check`가 통과했다. Unity Build·Test Runner·Scene 편집·원격 UGS 작업은 수행하지 않았다. 필요한 Phase 1 수동 작업은 없다.

2-1 계정 저장 구조 설계부터 2-10 환경·감사·운영 정책, Step 3 receipt 보관, Step 4 조회, Step 5 환경 격리, Step 6 운영 안전, Step 7 인계 대응표 및 Step 8 완료 판정을 완료했다. `node UGS/Tests/public-player-number-policy.test.cjs`, `leaderboard-owner-policy.test.cjs`, `legacy-account-migration-policy.test.cjs`, `transfer-credential-policy.test.cjs`, `transfer-resolution-policy.test.cjs`, `active-connection-policy.test.cjs`, `transfer-client-state-policy.test.cjs`, `public-number-display-policy.test.cjs`, `transfer-operations-policy.test.cjs`, `submission-retention-policy.test.cjs`, `leaderboard-query-policy.test.cjs`, `environment-data-policy.test.cjs`, `operational-safety-policy.test.cjs`와 `phase1-policy-contracts.test.cjs`는 각각 번호·고정 행·전환·자격 증명·B 연결·활성 연결·Local Save·표시·환경/감사/운영·receipt 수명·조회·환경 scope·운영 안전·문서 인계 경계를 통과했다. `node UGS/Tests/static-contracts.cjs`와 `node UGS/Tests/cloud-code.test.cjs`도 각각 통과와 21/21 통과를 확인했고 `git diff --check`도 통과했다. 이 결과는 순수 정책과 기존 구현의 회귀 근거이며, C 귀속 제출·조회 전환, Client 생성 시각·환경 scope migration, C별 시간 분할·실제 정리 원자성·응답 유실·기기 이전 경합, 100명 초과 전역 rank·공개 번호 행 결합·실제 CSPRNG·HMAC Secret Manager·동시성·계정 매핑·30일 로그 sink/삭제·서버 rate-limit·원격 서비스 원자성·환경 게시/롤백·UI 통합의 검증 결과가 아니다. Unity Editor·Unity Test Runner·Build·Scene·원격 UGS는 실행하지 않았다.

# 후속 작업

Phase 2에서는 공개 번호와 단일 활성 연결·기기 이전·고정 Leaderboard 행을 구현·검증한다. Phase 3에서는 receipt 수명·100명 초과 조회·동점 페이지 경계를 검증한다. 환경 적용·복구와 30일 로그 sink는 Phase 4에서, Windows Player 통합 확인은 Phase 5에서 수행한다.

Phase 2 실행 계획은 `AI/90_Tasks/Prototype_8/20261003_01_Phase2ManualSteps.md`의 Step 1~12로 연결한다. 계획 문서 작성 완료이며 구현·수동 적용·실행 검증은 아직 수행하지 않았다.

# 관련 문서

- `AI/README.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/ResultMenu.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/03_Features/AccountTransfer.md`
- `AI/02_Systems/RecordSubmissionSystem.md`
- `AI/02_Systems/AccountConnectionSystem.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_7/20260928_01_Phase4ManualSteps.md`

# 작성 완료 기준

- [x] 사용자 조작·답변 형식과 각 Step의 완료 조건을 명시했다.
- [x] 정적 검사·Unit Test로 가능한 검증을 수동 작업에서 분리했다.
- [x] 계획 작성 완료와 Phase 1 수행 완료를 구분했다.
