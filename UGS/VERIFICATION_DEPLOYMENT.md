# Phase 3 verification 적용 안내

이 문서는 Step 4-1의 **검증용 소스 준비 결과**다. AI는 UGS에 접속·배포하지 않았으며 Unity Build·Test Runner·Scene/Inspector 변경도 하지 않았다. 일반 사용자/운영 환경 배포용 완료 선언이 아니다.

## 대상과 현재 제한

| 항목 | 값 |
| --- | --- |
| Project ID | `c76d55cf-7846-494b-9dce-a0797b179b36` |
| 환경 | `verification` |
| Environment ID | `a20a46fa-1edb-4d79-9c35-02f2fed31896` |
| Stage | `fs-stage-stage-001-r1`, ascending, keepBest, rules 1 |
| Infinite | `fs-infinite-v2`, descending, keepBest, scoring 2 |
| Cloud Code JS scripts | `submit-record`, `query-records` |
| 두 스크립트의 입력 | `request`: String, Required, 기본값 없음. JSON을 문자열로 전달 |
| Protected Player Data key | `fs_submission_ledger_v1` |

검증용 안전 제한은 아래와 같다. 이를 해소하지 않고 운영으로 전환하지 않는다.

- 새 테스트 계정은 서버 ledger를 **사용자가 한 번 초기화**한다. 최초 키 생성은 writeLock 비교로 안전하게 보호할 수 있다는 근거가 없어 런타임에서 자동 생성하지 않는다. 미생성/손상 키는 `LedgerNotProvisioned` → Pending이다.
- ledger는 계정별 최대 128개 ID를 보존한다. 기존 ID는 만료·삭제·덮어쓰기하지 않으며 초과 신규 ID는 `LedgerCapacity` → Pending이다. 무제한 운영 보관/분할과 신규 계정 자동 초기화는 후속 서버 설계 항목이다.
- 조회는 보드 전체 100명 이하일 때 한 페이지를 받아 서버 접수 시각까지 정렬한다. 초과/불완전 페이지는 `VerificationBoardCapacity`로 실패한다. 일반 규모의 안정적인 전체 순위 조회는 아직 미완료다.
- 서버는 정수·구성 합계·Version·시간 대비 논리 상한을 검증한다. 사용자가 보고한 플레이 시간이나 Stage 실제 클리어의 진위를 증명하지는 않는다. 서버 권위 게임 시뮬레이션/완전한 부정행위 방지를 구현했다고 간주하지 않는다.
- 동점 접수 시각은 서버 UTC epoch milliseconds다. 점수와 시각이 모두 같으면 공동 순위(다음 순위 건너뜀)이며, Player ID는 표시 순서를 고정하는 용도로만 쓴다. Dashboard 원래 rank와 커스텀 조회 rank가 다를 수 있다.
- 스크립트 서비스 토큰/SDK, keepBest의 metadata 보존, 실제 Cloud Save CAS 및 정책 강제력은 Step 7에서 별도로 확인한다. 로컬 대역 테스트로 서비스 보장을 입증하지 않는다.

## 1. Unity 소스 컴파일 확인 — 사용자

Unity Editor가 소스를 가져온 뒤 Script Compilation Error/Warning을 확인한다. Build는 필요 없다. 기존에 패키지만 설치했을 때의 성공 결과는 이번 변경의 컴파일 결과가 아니다.

추가 패키지는 없다. Authentication `3.8.0`, Cloud Code `2.10.4`, 기존 Core를 사용한다. Features asmdef에 실제 설치된 UGS assembly 참조를 추가했다. Leaderboards/Cloud Save Unity 클라이언트 SDK는 필요 없다.

## 2. Cloud Code와 Cloud Save 활성화 — 사용자

Unity Dashboard에서 위 Project와 **verification**을 선택하고 Cloud Code 및 Cloud Save를 사용 가능 상태로 만든다. 기존 두 Leaderboard는 재생성하지 않는다. Reset/Bucket/Tier 설정도 추가하지 않는다.

Cloud Code의 **JavaScript Scripts**에서 다음 이름으로 스크립트를 생성한다. C# Module이 아니다.

1. `submit-record`: `UGS/CloudCode/submit-record.js`의 전체 내용을 붙여 넣는다.
2. `query-records`: `UGS/CloudCode/query-records.js`의 전체 내용을 붙여 넣는다.
3. 각 스크립트의 입력은 `request`, 형식 `String`, Required이다. 파일 끝 `module.exports.params`와 Dashboard 입력 정의가 일치해야 한다. 예전 `boardId/submissionId/score` 개별 입력 초안이 있다면 제거한다.
4. 저장 후 Publish한다. `submit-record`는 `@unity-services/cloud-save-1.4`, 두 스크립트는 `@unity-services/leaderboards-1.1`을 사용한다.

스크립트가 `context.playerId`를 요구하므로 Player context 없는 Dashboard 실행은 정상 제출 검증이 아니다. `playerId`를 입력 파라미터에 임의 추가하지 않는다. 초기화·검증은 아래 Editor 창에서 인증한 테스트 계정으로 수행한다.

## 3. Access Control 적용 — 사용자

`UGS/AccessControl/project-policy.json`은 Player의 Leaderboards 전체 Write와 Cloud Save 전체 Write를 거부한다. Cloud Code 서비스 토큰은 Player가 아니므로 이 Deny 대상이 아니다. Cloud Code 호출을 추가로 Allow하는 광범위 정책은 넣지 않는다.

정책 적용은 [Unity의 공식 UGS CLI 절차](https://docs.unity.com/en-us/cloud-code/scripts/how-to-guides/access-control)를 사용한다. Dashboard에 임의의 정책 편집 메뉴가 있다고 가정하지 않는다. CLI 인증은 사용자 컴퓨터에서만 준비한다. 서비스 계정에는 Project Resource Policy Reader/Editor 및 Unity Environments Viewer 권한이 필요하다. 키·토큰·암호를 코드/명령 파일/채팅에 붙이지 않는다.

기존 정책을 먼저 조회·백업하고 같은 Sid 및 더 구체적인 Player Allow 정책이 있는지 확인한다. 충돌이 있으면 기존 정책을 무작정 삭제하지 말고 중단한다. **아래 명령은 사용자가 직접 실행하며 AI는 실행하지 않았다.** 프로젝트 루트에서 실행한다.

```powershell
ugs config set project-id c76d55cf-7846-494b-9dce-a0797b179b36
ugs config set environment-name verification
ugs access upsert-project-policy UGS/AccessControl/project-policy.json
```

원격 정책의 대상은 위 환경 ID여야 한다. 필요하면 [Resource Policy API](https://docs.unity.com/en-us/oas-access-resource-policy/1.0.0)의 다음 주소로 GET 확인 후 동일 환경에 PATCH한다. 정책 문장 전체를 다른 환경에 복제하지 않는다.

```text
https://services.api.unity.com/access/resource-policy/v1/projects/c76d55cf-7846-494b-9dce-a0797b179b36/environments/a20a46fa-1edb-4d79-9c35-02f2fed31896/resource-policy
```

두 Deny 문장이 반영되어도 실제 Player 403 검증 전에는 접근 차단 성공으로 완료 처리하지 않는다.

## 4. 테스트 계정 귀속과 최초 ledger 생성 — 사용자

1. 기존 게임 Scene에서 Play Mode로 들어간다. Scene/Inspector 수정은 필요 없다.
2. 상단 `Flow State > Online Record Verification` 창을 연다.
3. 복구 제한 안내를 읽고 동의 체크 후 **동의 저장 및 인증 / Pending 재시도**를 누른다. 인증은 `flow-state-verification` 전용 프로필과 명시된 환경으로 시작한다. 다른 코드가 환경 확인 없이 UGS를 이미 초기화한 경우 안전하게 실패한다.
4. **테스트 Player ID를 클립보드에 복사**를 누른다. 전체 Player ID는 사용자 Dashboard 검색에만 사용하고 채팅/로그에 공유하지 않는다. 처음에는 ledger가 없어 Pending인 것이 정상이다.
5. 진행 중인 재시도가 끝나도록 기다린 뒤 Play Mode를 종료한다. 다른 장치/창에서 같은 계정의 제출·초기화를 동시에 실행하지 않는다.
6. Dashboard의 Cloud Save → Player Data/Manage Players에서 **verification**과 해당 Player ID를 선택한다. Protected access class의 `fs_submission_ledger_v1` 유무를 확인한다. 이미 존재하면 **초기값으로 덮어쓰지 않는다**.
7. 새 Protected 키 생성 UI가 제공되는 경우에만 키를 추가하고, 값에 `UGS/CloudSave/verification-ledger-seed.json`의 JSON **object**를 넣는다. JSON 문자열로 이중 인코딩하거나 Default/Public 키로 만들지 않는다.
8. Dashboard에서 신규 Protected 키를 만들 수 없는 경우 [Cloud Save Admin API](https://docs.unity.com/en-us/oas-cloud-save-admin/1.0.0)의 `Get Protected Player Items`로 부재를 확인한 뒤 `Set Protected Player Item`을 사용자 로컬 관리자 도구에서 한 번 호출한다. Cloud Save Editor 권한이 필요하다. 아래 주소의 `<TEST_PLAYER_ID>`만 본인 테스트 계정으로 바꾼다.

```text
GET/POST https://services.api.unity.com/cloud-save/v1/data/projects/c76d55cf-7846-494b-9dce-a0797b179b36/environments/a20a46fa-1edb-4d79-9c35-02f2fed31896/players/<TEST_PLAYER_ID>/protected/items
```

POST body(최초 생성에만 사용, 기존 값이 있으면 중단):

```json
{
  "key": "fs_submission_ledger_v1",
  "value": { "version": 1, "active": "", "entries": [], "best": {} }
}
```

조회 결과에서 object 값과 서비스가 생성한 writeLock을 확인한다. writeLock을 임의 값으로 만들거나 null로 갱신하지 않는다. 보관 한도/오류를 해결하려고 ledger나 기존 최고 기록을 삭제하지 않는다.

최초 생성 중단/동시 초기화 가능성이 있다면 무조건 재POST하지 말고 GET으로 확인한다. 정상 런타임 갱신은 읽은 writeLock을 사용하는 비교 후 저장(CAS)만 수행한다. [Cloud Save write lock 동작](https://docs.unity.com/en-us/cloud-save/concepts/write-locks)을 근거로 최초 생성과 갱신을 구분했다.

## 5. 실제 제출·조회 — Step 7 사용자 검증

Play Mode에서 Stage를 클리어하거나 Infinite 기록을 만든 뒤 검증 창의 **Pending 재시도**를 누른다. 상위·내 주변·내 최고 조회 버튼으로 결과를 확인한다. 현재 Result 화면은 Phase 4 UI이므로 이 창을 검증 경로로 사용한다.

- 제출 성공 후 Protected ledger의 해당 ID가 `Submitted`, active가 빈 문자열이고 최고 기록의 metadata에 `submissionId/acceptedAt`가 있어야 한다.
- 재시작·다시 재시도해도 완료 후보가 재전송되지 않아야 한다. 서비스 응답 유실의 중복 재요청은 같은 ID·같은 점수·같은 접수 시각으로 복구한다. 네트워크 요청 횟수 자체가 정확히 1회라는 보장은 아니다.
- 거부 후보는 Rejected receipt로 저장되고 자동 재시도되지 않는다. Timeout/Offline/403/429/서비스 오류는 Pending이다. 계기당 최대 3회, 재시도 간격 1초·2초, SDK 호출 응답 대기 15초 후 Pending으로 돌아간다.
- 다른 인증 계정으로 바뀌면 기존 로컬 후보는 전송하지 않는다. 로컬 UUID와 제출 ID는 변경하지 않는다. 인증 정보 삭제로 시험하면 익명 계정을 잃을 수 있으므로 사전 동의 없이 삭제하지 않는다.
- 기존 보드 데이터에 서버 metadata가 없으면 조회가 실패한다. 기존 데이터를 자동 삭제하거나 정상으로 추정하지 말고 해당 상태만 전달한다.
- 실패/직접 Write 403/서버 거부의 실제 서비스 회귀 절차는 Step 7의 별도 확인 대상이다. 로컬 테스트가 이를 대체하지 않는다.

## 정적 검증과 테스트

AI 실행 완료:

```text
node UGS/Tests/static-contracts.cjs
node UGS/Tests/cloud-code.test.cjs
git diff --check
```

Cloud Code SDK 대역 테스트 **21/21 통과**. 서버 구문, 동시 CAS, 중복 ID/변조, projection 응답 유실, 최종 journal 저장 실패, 거부 재호출, 보관 한도, 점수 경계, 조회 동점/페이지 한도를 포함한다. 외부 네트워크·원격 서비스는 호출하지 않는다. 설치된 Unity 도구 폴더의 Node 실행 파일만 사용했으며 Unity Editor/빌드 도구는 실행하지 않았다.

Unity Test Runner **작성만 완료, 실행하지 않음**:

- `OnlineRecordRepositoryTests`
- `OnlineRecordConfigurationTests`
- `LocalSaveJsonCodecTests`
- 기존 `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests` 및 전체 Edit Mode/관련 Play Mode 회귀

새 테스트는 동의·귀속·계정 불일치, 저장 실패, 인증 실패·Timeout, 상태 매핑, 재시도 횟수/백오프, 재실행 receipt, v1→v2 migration, 조회 계약을 검증한다. 사용자 Step 6에서 실행한다.

전달할 결과는 컴파일 Error/Warning 유무, 두 스크립트 Publish 여부, 정책 대상 환경/반영 여부, Protected key 생성 여부, 호출 상태/건수만으로 충분하다. 비밀값이나 전체 Player ID는 공유하지 않는다.
