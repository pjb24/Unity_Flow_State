# Flow State Cloud Save 데이터 참조

## 목적

Cloud Save Dashboard에 보이는 Game Data와 Player Data의 이름을 사람이 판독할 수 있도록
정리한다. 이 문서는 **로컬 Cloud Code 구현의 저장 계약**이며, 특정 환경에 실제로 어떤
값이 존재하는지 또는 Production에 설정이 완료됐음을 뜻하지 않는다.

값·Player ID·write lock·이전 자격 증명·Secret은 이 문서나 작업 기록에 복사하지 않는다.
Dashboard에서는 이름, access class, 존재 여부, 안전한 상태만 읽기 전용으로 대조한다.

## 먼저 알아둘 점

Cloud Save에는 이 프로젝트용 사전 등록 schema가 없다. 각 저장값은 key/value JSON이고,
아래의 container 이름과 key, `schemaVersion`, `projectId`, `environmentId` 검사가 실제
schema 계약이다.

| 저장 종류 | Cloud Save 분류 | 누가 읽고 쓰는가 | 용도 |
| --- | --- | --- | --- |
| 논리 계정·매핑·제출 ledger·이전 receipt | Game Data / Private Custom | Cloud Code만 | 계정 귀속, 공개 번호, 제출 중복 판정, 기기 이전 |
| 기존 ledger와 cutover marker | Player Data / Protected | Cloud Code와 승인된 관리자 도구 | 기존 Player 기반 자료를 논리 계정으로 안전하게 전환 |

클라이언트 Player의 Cloud Save/Leaderboards 직접 Write는 Access Control에서 거부된다.
Cloud Code는 서버 문맥으로만 이 데이터를 처리한다.

## Private Custom container 이름

아래 `<…>`는 실제 식별자 자리표시자다. 실제 값을 문서·채팅·일반 로그에 넣지 않는다.

| container 패턴 | 고정 key | 사람이 읽을 수 있는 의미 |
| --- | --- | --- |
| `fs8-account-<논리 계정 ID>` | `fs_account_v1` | 논리 계정 C의 상태. 현재 활성 Player 연결, 공개 번호, 고정 Leaderboard 소유 ID, 온라인 작업 예약, 이전 상태를 보관한다. |
| `fs8-player-<Player ID>` | `fs_account_v1` | Anonymous Authentication Player와 논리 계정 C의 연결표. 현재 연결 revision과 Active/Inactive 상태를 보관한다. |
| `fs8-owner-<고정 소유 ID>` | `fs_account_v1` | Leaderboard 소유 행과 논리 계정 C의 역방향 매핑이다. |
| `fs8-number-<10자리 공개 번호>` | `fs_account_v1` | 공개 번호와 논리 계정 C의 역방향 매핑이다. 공개 번호는 표시용이며 권한 근거가 아니다. |
| `fs8-ledger-<논리 계정 ID>` | `fs_account_v1` | 제출 ledger. 개인 최고, 진행 중 제출, terminal receipt directory와 v1→v2 migration 상태를 보관한다. |
| `fs8-allocator-public` | `fs_account_v1` | 다음에 발급할 공개 번호의 순번이다. 재설정하거나 감소시키면 안 된다. |
| `fs8-t-<이전 코드 digest>` | `fs_account_v1` | 이전 코드 조회용 인덱스다. raw 이전 코드는 없고, 계정·이전 요청·credential revision의 매핑만 가진다. |
| `fs8-receipt-<이전 요청 ID>` | `fs_account_v1` | 완료된 기기 이전의 terminal receipt다. 재호출 복구와 이전 기기 비활성 판정에 사용한다. |
| `p2v-<검사 실행 ID>-<해시>` | `fs_account_v1`, `fs_creation_guard_v1` | `LIVE_TRANSFER_STORAGE_PROBE`가 실제 계정·Leaderboard와 분리해 만든 verification 전용 저장 검사 namespace다. 앱의 런타임 namespace가 아니다. |

모든 `fs8-*` 값은 `schemaVersion: 1` 및 저장 당시의 Project/Environment scope가 일치해야
유효하다. 다른 환경의 값은 사용하거나 복사하지 않는다.

`p2v-*`는 Cloud Save의 저장 경계·CAS 복구를 검사하기 위한 별도 자료다. probe는 timeout 뒤에도
저장이 반영됐을 가능성을 보존하기 위해 자동 삭제하지 않는다. 따라서 `p2v-*`를 일반 계정 자료로
해석하거나, 확인만을 위해 삭제·재사용하지 않는다.

### `fs_account_v1` 값의 공통 해석

`fs_account_v1`은 여러 container에서 재사용되는 **key 이름**이다. container 종류가 의미를
결정한다. 예를 들어 `fs8-player-*`는 Player 연결표이고 `fs8-ledger-*`는 제출 상태다.

- account: 계정 C 본문과 활성 연결·이전 상태
- player: Player → C 연결표
- owner: Leaderboard owner → C 연결표
- number: 공개 번호 → C 연결표
- ledger: 제출·개인 최고·receipt directory
- allocator: 다음 공개 번호
- t: 이전 코드 digest → 이전 요청 연결표
- receipt: 완료된 이전 요청의 terminal receipt

원본 이전 코드, 인증값, Secret은 저장하지 않는다. 이전이 진행 중일 때 account 값에는
검증용 digest/HMAC이 있을 수 있으므로 Dashboard에서 값을 열거나 복사하지 않는다.

## 제출 terminal receipt journal

제출의 중복 판정 결과는 계정별·시간 bucket별 journal로 보관한다. 이는 128개 항목으로
고정된 legacy ledger를 대체한다.

### container

| container 패턴 | 의미 | 이 container에서 확인할 key |
| --- | --- | --- |
| `r-<논리 계정 ID>-<YYYYMM>-<001~999>` | 한 달 bucket의 receipt journal container | `manifest`, 하나 이상의 `s-<001~999>`, `fs_receipt_guard_v1` |
| `l-<논리 계정 ID>-<제출 ID 앞 3자>` | 제출 ID가 어느 journal shard에 있는지 찾는 locator container | 하나 이상의 `r-<제출 ID>`, `fs_receipt_guard_v1` |

### key

| key 패턴 | 확인할 container | 의미 |
| --- | --- | --- |
| `manifest` | `r-<논리 계정 ID>-<YYYYMM>-<001~999>` | 해당 journal container의 shard 목록, receipt 수, byte 수, cleanup 진행 상태 |
| `s-<001~999>` | `r-<논리 계정 ID>-<YYYYMM>-<001~999>` | terminal receipt entry 묶음(shard) |
| `r-<제출 ID>` | `l-<논리 계정 ID>-<제출 ID 앞 3자>` | 제출 ID → receipt journal container와 shard 위치를 가리키는 locator |
| `fs_receipt_guard_v1` | 위 두 종류의 container 모두 | 해당 container의 동시 최초 생성 중 덮어쓰기를 막는 guard |

terminal receipt는 제출 ID, 원본 payload의 동일성 판정 자료, Submitted/Rejected 결과,
안전한 사유와 서버 수락 시각을 포함한다. 180일이 지난 terminal receipt는 제한된 cleanup
과정에서 삭제할 수 있다. Dashboard에서 shard·manifest·locator를 수동 수정하거나 하나만
삭제하면 replay/cleanup 복구를 망가뜨릴 수 있다.

## 초기 준비 참조 복구 경계

서버 상태 조회가 재개하는 초기 provisioning에서 연결표는 있으나 그 계정은 없고, Protected frozen marker가
같은 Player의 다른 Preparing 계정을 가리킬 수 있다. 원본 snapshot·범위·최초 소유권·기존 매핑·연결 write lock을
검증한 경우에만 서버가 초기 연결표를 CAS로 복구한다. 이미 발급된 번호/고정 owner와 ledger/행은 보존한다.
이 복구는 Active/Detached/이전 중 계정의 재연결이나 공개 번호 기반 복구를 허용하지 않는다.
Dashboard에서 marker/연결표/account를 직접 수정·삭제해 조건을 만들지 않는다.

## 기존 Player Protected ledger

| access class | key | 의미 |
| --- | --- | --- |
| Player Data / Protected | `fs_submission_ledger_v1` | Prototype 7 이전 제출 ledger 및 논리 계정 전환(cutover) marker |
| Player Data / Protected | `fs_creation_guard_v1` | legacy ledger의 동시 최초 생성을 직렬화하는 guard |

`fs_submission_ledger_v1`의 비어 있는 최초 형태는 `version: 1`, 빈 `active`, 빈 `entries`,
빈 `best`다. 이후 처음 유효한 제출에서 논리 계정 전환 정보가 추가될 수 있다. 기존 key가
있으면 초기 seed로 덮어쓰거나 삭제하지 않는다.

## Dashboard에서 읽기 전용으로 확인하는 방법

1. 먼저 환경 선택기가 verification인지 Production인지 확인한다. 한 번에 한 환경만 본다.
2. **Game Data**에서 Private Custom data를 볼 수 있으면 `fs8-` 접두사와 위 container/key
   패턴만 대조한다. 값, write lock, Player ID, digest/HMAC은 열거나 복사하지 않는다.
3. **Player Data → Manage Players**는 승인된 기존 테스트 계정이 있을 때만 사용한다. Protected
   access class에서 `fs_submission_ledger_v1`과 `fs_creation_guard_v1`의 존재 여부만 확인한다.
4. Production이 비어 있으면 `없음`으로 inventory에 기록하고, 확인을 위해 Player·key·container·
   index를 새로 만들지 않는다.
5. 이 저장 모델은 Cloud Save query/index를 사용하지 않는다. index/Query가 없다는 사실은
   정상이며 생성할 이유가 아니다.

## 절대 하지 않는 작업

- `fs8-*`, `r-*`, `l-*`, `fs_submission_ledger_v1`을 수동 편집·삭제·초기화하지 않는다.
- verification 자료를 Production으로 복사하거나 Production 자료를 verification에 복사하지 않는다.
- receipt 보관 한도나 오류를 해결하려고 manifest, shard, locator, ledger를 지우지 않는다.
- 이전 코드, 인증값, HMAC, digest, Secret, Player ID, write lock을 Task·커밋·채팅에 기록하지 않는다.

## 구현 근거

- `UGS/CloudCode/account-store.js`
- `UGS/CloudCode/account-provisioning-service.js`
- `UGS/CloudCode/receipt-store.js`
- `UGS/CloudCode/receipt-journal.js`
- `UGS/CloudCode/legacy-account-store.js`
- `UGS/CloudCode/transfer-completion-service.js`
- `UGS/CloudCode/submission-retention-policy.js`
- `UGS/AccessControl/project-policy.json`
