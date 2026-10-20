# Production 환경 Inventory

## 목적과 기록 경계

Prototype 8 Phase 4에서 Production 배포 전 읽기 전용으로 대조한 사실과 미확인 항목을
관리한다. 이 문서는 배포 지시서가 아니며, `READY_FOR_APPROVAL` 전에는 Production을
생성·수정·게시하는 권한이 아니다.

Secret 값, access token, Player ID, Environment ID, 원격 source 본문, write lock은 이
저장소에 기록하지 않는다. 식별값은 승인된 비공개 inventory에만 보관하고, 이 문서에는
대조 결과와 그 보관 여부만 기록한다.

## 현재 판정

`PRODUCTION_DEPLOY_BLOCKED` — Production의 배포 baseline, rollback reference, 최소 권한,
Secret 접근, 30일 로그/삭제 절차, 호환성 승인, verification rehearsal이 준비되지 않았다.

## Production 읽기 전용 baseline — 2026-10-09

사용자가 Unity Dashboard의 `production` 환경에서 확인한 첫 배포 전 상태다. 확인 시점에는
Production에 변경을 가하지 않았으며, 이 기록은 생성·게시·배포 승인 권한이 아니다.

| 영역 | 관찰 결과 | 복구/배포 기준 |
| --- | --- | --- |
| Cloud Code / C# Modules | 등록 Module 없음 | 활성 Module·게시 reference·rollback reference 없음 |
| Access Control | 등록 정책 없음 | policy revision/reference 없음 |
| Leaderboards | 등록 보드 없음 | Board version/reference 없음 |
| Cloud Save | 데이터 없음 | container·key·schema baseline 없음 |
| 개발 환경 Secret | 등록 Secret 없음 | Secret reference 없음 |

따라서 첫 Production 배포 전에는 rollback할 원격 버전이 없으며, 기존 데이터를 덮어쓰거나
환경 간 자료를 복사해서는 안 된다.

## Production 최소 권한 설정 보고 — 2026-10-09

baseline 기록 이후 사용자가 Production 환경에서 다음 설정을 만들었다고 보고했다. Secret 값은
열람·기록하지 않았다.

| 영역 | 사용자 보고 | 읽기 전용 대조 결과 |
| --- | --- | --- |
| Access Control | Leaderboards 및 Cloud Save Player Write Deny 정책 생성 | `DenyPlayerCloudSaveWrites` / `DenyPlayerLeaderboardWrites`; 각각 Player 전체 Cloud Save/Leaderboards `Write` `Deny`, `Expires=Never`. 추가 Player Allow/Sid 충돌 보고 없음. |
| 개발 환경 Secret | 독립 Secret 값 생성, 서비스 액세스 Cloud Code만 | 이름 `FS_TRANSFER_HMAC_VERIFICATION_V1`, 설명 `Production account transfer HMAC v1`, 서비스 액세스 Cloud Code만. 값은 열람·기록하지 않음. |

이 설정은 Production Module 배포, Module Secret 읽기 성공, 운영 Player 호출 또는 deploy gate 승인을 뜻하지 않는다.

## verification Player 직접 Write 강제력 — 2026-10-09

격리 verification session에서 Player token의 Leaderboards 및 Cloud Save 직접 Write probe가 각각
HTTP 403으로 거부됐다. 최초 계정 상태 조회의 window timeout은 이 결과의 성공 근거가 아니며,
명시적 상태 확인 뒤 `Ready`가 된 후 한 번 실행한 probe의 결과만 기록한다. Production에서는
운영 데이터 오염 가능성 때문에 이 probe를 실행하지 않는다.

## 대상 분리

| 항목 | 현재 기록 | 상태 |
| --- | --- | --- |
| Production 환경 표시 이름 | `production` | verification과 구분됨 |
| Production 환경 식별자 | 승인된 비공개 inventory에만 보관 | Task·코드·이 문서에는 미기록 |
| verification 대상 | 별도 환경으로 확인됨 | Production과 데이터 복사 금지 |

## 읽기 전용 관찰 결과

| 영역 | verification 관찰 | Production 관찰 | Production 적용 전 필요한 대조 |
| --- | --- | --- | --- |
| Authentication (프로젝트 범위) | Identity Providers 목록은 비어 있다. Client는 허용된 verification 전용 profile에서만 `SignInAnonymouslyAsync`를 수행하며, 외부 Identity Provider가 없는 것은 Anonymous sign-in에 정상이다. | 환경별 Production Authentication 설정으로 분리하지 않는다. Client의 Production profile은 환경 지원 서비스에만 승인된 Production Project/Environment 조합을 사용해야 한다. | Identity Provider가 프로젝트 범위임을 내부 inventory에 기록하고, 목록이 비어 있음을 읽기 전용으로 대조한다. Player 생성·로그인·삭제는 하지 않는다. |
| C# Module | `FlowStateVerification` 하나가 확인됨. Dashboard는 이름·생성/업로드 시각만 제공하고 source download/version reference는 제공하지 않음. | Module 없음 | 배포 전의 빈 baseline과, 첫 배포 뒤 rollback reference가 없음을 기록. 로컬 Git은 원격 source 동일성 증명이 아님. |
| Access Control | Player Cloud Save/Leaderboards `Write` Deny 두 문장이 로컬 artifact와 일치. | 정책 없음 | 정책 artifact·대상 환경·revision/reference를 읽기 전용 대조. Player Allow 또는 Sid 충돌이면 중단. |
| Secret | `FS_TRANSFER_HMAC_VERIFICATION_V1` 이름의 Secret이 존재하며 서비스 액세스는 Cloud Code만이다. 값은 열람·기록하지 않았다. | Secret 없음 | 필요한 Secret의 **이름과 Cloud Code 읽기 권한만** 대조. 값은 열람·기록·공유 금지. |
| Leaderboards | Stage/Infinite 보드가 로컬 계약과 일치. | 보드·행·설정 없음 | 빈 baseline을 보관. 설정은 승인된 Step 4/5 이후에만 생성하며 verification 자료를 복사하지 않음. |
| Cloud Save | `fs8-*`, receipt journal, legacy Protected ledger 및 `p2v-*` probe namespace가 로컬 계약과 일치. | Game/Player Data·namespace 없음 | 빈 baseline을 보관. key·container·index·Player 자료를 확인 목적으로 만들지 않음. |
| Logs | Unity Cloud Code 기본 로그는 7일 보존으로 확인됨. 로컬 Docker `UGS/LogSink`는 health `204`, 합성 안전 이벤트 `202` 및 모든 예정 거부 상태(`400`/`401`/`413`/`415`/`404)를 확인함. | Cloud Run 외부 sink·30일 보존·삭제 절차 없음 | 로컬 HTTP 계약 확인은 배포/보존 증거가 아니다. 재기동/출력 노출 확인 뒤에만 Cloud Run staging을 준비한다. 기본 로그만으로는 요건을 충족하지 못한다. `UGS/SAFE_LOG_SINK_CONTRACT.md`에 따라 외부 sink, 접근 제어, 30일 보존·삭제를 대조하기 전까지 deploy gate를 닫는다. |

### LogSink 로컬 Docker 완성도 확인 — 2026-10-09

사용자가 `docker compose down` 뒤 수집기를 재기동하고 `probe.ps1` 전체를 다시 통과시켰다.
health `204`, 합성 안전 이벤트 `202`, 예정된 거부 상태(`400`/`401`/`413`/`415`/`404`)가 모두
재현됐다. `docker compose logs`에는 허용된 일곱 합성 필드만 있었으며 HMAC, request body,
Player 식별자, 원시 오류 전문은 없었다. 이 결과는 Cloud Run staging 준비 근거일 뿐 30일 보존,
삭제, 외부 IAM 설정의 증거는 아니다.

### LogSink GCP verification 기반 구성 — 2026-10-10

verification 전용 GCP project가 격리되어 있고 Cloud Run, Cloud Logging, Secret Manager, Artifact
Registry API가 활성화됐다고 사용자 보고로 대조했다. Cloud Run region은 결정만 기록됐으며, GCP project
자체에는 전역 region 설정이 없으므로 아직 리소스에 적용하지 않았다. Cloud Run service, repository,
Secret, IAM binding, UGS Module, Production 변경은 이 단계에서 수행하지 않았다. Project ID, endpoint,
credential은 기록하지 않는다.

## Authentication 확인 절차 — 사용자, 읽기 전용

1. Unity Dashboard에서 Project를 선택한다. Player Authentication의 Identity Provider 설정은 프로젝트 범위로 취급하며 `production` 환경 선택 여부의 증거로 사용하지 않는다.
2. **Player Authentication**을 열어 Identity Provider 목록을 확인한다.
3. 목록과 Player Management의 존재 여부를 확인한다. 이 프로젝트의 현재 Client는
   Anonymous sign-in만 사용하므로, 외부 Identity Provider가 없는 것은 오류가 아니다.
4. 다음만 내부 inventory에 적는다: 프로젝트 범위 설정임, Identity Provider `없음` 또는 provider 종류,
   Player 자료 `없음` 또는 존재 여부, 확인 시각. Player ID·token·개인정보는 기록하지 않는다.
5. Player 생성·익명 로그인·provider 추가/삭제·player 삭제는 하지 않는다.

Unity Authentication은 Anonymous sign-in으로 Player 문맥을 만든다. Client 초기화에서 선택한 환경은
Cloud Code·Cloud Save·Leaderboards 등 환경 지원 서비스의 대상이며 Identity Provider 설정 자체를 바꾸지 않는다.
Production build profile은 Step 4의 명시 승인 전에는 추가하지 않는다.

## 로컬 변경 기준

| 항목 | 현재 로컬 기준 |
| --- | --- |
| Client baseline commit | `a6258ef` (로컬 Git HEAD; release candidate 승인 또는 원격 Module 동일성 증명이 아님) |
| Module manifest | `Assets/CloudCode/FlowStateVerification.ccmr`의 `FlowStateVerification` C# Module 하나 |
| Access Control artifact | `UGS/AccessControl/project-policy.json` |
| Cloud Save namespace 설명 | `UGS/CLOUD_SAVE_DATA_REFERENCE.md` |
| Production deploy gate | `UGS/CloudCode/operational-safety-policy.js`의 `getProductionDeployGate` |

## Step 3 호환성 연결 기록 — 2026-10-09

이 기록은 첫 Production 배포 전의 로컬 release candidate 연결 기준이다. 아래 Git commit은
원격 C# Module과의 동일성을 증명하지 않으며, Production 배포 승인이나 rollback reference도 아니다.

| 연결 항목 | 기준 버전 또는 reference | 대조 근거 |
| --- | --- | --- |
| Client release candidate | Git `a6258ef5a6e1c22126fa3bd6ec98caeb74e8db6c` | 현재 로컬 Git `HEAD` (2026-10-07) |
| Module source candidate | Git `a6258ef5a6e1c22126fa3bd6ec98caeb74e8db6c` | `Assets/CloudCode/FlowStateVerification.ccmr`, `UGS/Modules/FlowStateVerification`, `UGS/CloudCode`의 마지막 공통 변경 commit |
| Client Local Save | format v7; environment scope는 v6부터 | `Assets/Scripts/Runtime/Features/LocalSaveData.cs`, `LocalSaveJsonCodec.cs` |
| Cloud Code 저장 계약 | account/receipt scope `schemaVersion: 1`; receipt manifest `version: 1` | `UGS/CloudCode/account-store.js`, `receipt-store.js` |
| 호환성 표 | Phase 4 local contract v1 | request/response backward-compatible, migration marker 뒤 legacy writer 재도입 금지 — `UGS/CloudCode/operational-safety-policy.js`와 `UGS/Tests/phase4-step2-policy.test.cjs` |

호환성 표 v1의 승인 대상은 위 Client·Module source candidate 조합뿐이다. 다른 commit,
환경, schema 또는 Local Save writer를 사용하려면 이 표를 갱신하고 다시 대조해야 한다.

## 완료 전 필수 증거

- [ ] Cloud Code·Cloud Save·Leaderboards·Access Control 대상이 같은 Production 환경임을 읽기 전용으로 대조했고, Authentication Identity Provider의 프로젝트 범위 상태를 별도로 기록했다.
- [ ] 현재 Production baseline과 rollback 가능 여부를 내부 보관소에 기록했다.
- [ ] Access Control과 필요한 Secret 이름/Cloud Code 읽기 권한을 대조했다.
- [ ] 30일 안전 로그 sink와 삭제 절차를 대조했다.
- [ ] Client/Module/Local Save 호환성 표를 승인했다.
- [ ] verification 단일 Module 배포/rollback rehearsal 결과를 기록했다.

위 항목 중 하나라도 없으면 `PRODUCTION_DEPLOY_BLOCKED`를 유지한다.

## 관련 문서

- `AI/90_Tasks/Prototype_8/20261007_09_Phase4ManualSteps.md`
- `UGS/CLOUD_SAVE_DATA_REFERENCE.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`
- `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`
