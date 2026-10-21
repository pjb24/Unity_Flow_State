# 환경 중립 Cloud Code Module 구성

## 목적

로컬 `FlowStateVerification` source는 호출별 환경 scope를 사용하는 구조로 전환했다.
원격 verification 배포본과 새 source는 같은 verification Secret을 사용한다. Production에는 아래
공통 이름의 Secret을 준비한다. Step 8의 판정은 아직 `PRODUCTION_DEPLOY_BLOCKED`다.

## 구현 순서와 의존 관계

환경 중립화는 C# host만 바꾸는 작업이 아니다. 현재 C# Module이 내부 리소스로 실행하는 JavaScript에도
동일한 호출 scope를 전달해야 한다.

```text
C# IExecutionContext
  → ServerServices REST 대상
  → embedded JavaScript context
    → account-store scope
      → account / receipt / legacy ledger 저장값
      → transfer HMAC 입력
      → query / submit Leaderboard API 대상
C# IExecutionContext.EnvironmentName → Safe LogSink 환경 표시 이름
```

각 화살표는 함수 인수 또는 객체의 immutable `scope`로 전달한다. process 전역 변수와 cache 초기화 시점의
환경 선택은 사용하지 않는다. 상세 작업 번호는 Phase 4 Manual Steps의 `8-A-3-1`부터 `8-A-3-6`까지다.
Jint Engine과 cache는 기존처럼 호출마다 생성된다. LogSink는 환경 표시 이름 `verification` 또는
`production`만 허용하며 Project/Environment 식별자는 이벤트에 넣지 않는다.

## 환경별 Secret 이름 — 현재 적용 기준

현재 verification Secret 값을 보존하기 위해 C# host가 환경 표시 이름으로 UGS Secret 이름을 선택한다.
실행 context의 EnvironmentName을 정규화하고, 사용할 수 없으면 같은 환경의 `FS_ENVIRONMENT_NAME_V1`을
읽어 표시 이름을 보완한다. JS 내부의 논리 이름은 공통이며, JS는 실제 환경별 이름을 선택하지 않는다.

| 용도 | verification에서 읽는 이름 | Production에서 읽는 이름 |
| --- | --- | --- |
| 계정 이전 HMAC | `FS_TRANSFER_HMAC_VERIFICATION_V1` | `FS_TRANSFER_HMAC_V1` |
| LogSink 요청 서명 | `FS_LOG_SINK_HMAC_VERIFICATION_V1` | `FS_LOG_SINK_HMAC_V1` |
| LogSink base URL | `FS_LOG_SINK_URL_VERIFICATION_V1` | `FS_LOG_SINK_URL_V1` |
| 환경 표시 이름 보완 설정 | `FS_ENVIRONMENT_NAME_V1` 값 `verification` | `FS_ENVIRONMENT_NAME_V1` 값 `production` |

모든 항목의 서비스 액세스는 Cloud Code만 사용한다. 환경 간 값은 공유하지 않는다.
verification/production 외의 환경이나 허용된 세 이름 외의 요청은 거부한다.

## verification Secret 준비 — 8-D 완료

2026-10-20 사용자가 위 verification 이름 세 개의 존재와 Cloud Code 전용 접근을 확인했다.
새 Module도 같은 항목을 읽도록 source를 변경했다. Secret 값을 알아내거나 새 이름으로 복사할 필요가 없다.

`EnvironmentCheck` 실패 보정에는 아래 설정을 사용할 수 있다. 후속 Probe는 `Accepted`로 성공했다.
이 출력만으로 context 정규화와 설정 보완 중 어느 경로가 사용됐는지, 설정 Secret이 실제로 생성됐는지를
추정하지 않는다. 환경 해석과 기존 LogSink Secret 접근이 성공했으므로 8-D는 완료한다.

| 설정 항목 | 입력 내용 |
| --- | --- |
| UGS 대상 환경 | verification |
| Secret 이름 | `FS_ENVIRONMENT_NAME_V1` |
| 값 | `verification` |
| 설명 | `Verification server environment name` |
| 서비스 액세스 | Cloud Code만 |

이 값은 HMAC 키가 아닌 환경 표시 이름이다. 기존 Secret 세 개·GCP·Local Docker 설정은 유지한다.

### 앞서 안내한 이름 복사 작업의 처리

verification에 공통 이름 세 개를 추가하고 값을 복사하라는 이전 안내는 아래 적용 기준으로 대체한다.

| 항목 | 처리 |
| --- | --- |
| verification UGS Secret | 기존 이름·값·접근 권한 유지 |
| GCP Secret / Cloud Run 연결 | 기존 이름·값·연결 유지 |
| Local Docker `.env` | 로컬 전용 값 유지 |
| 기존 계정 이전 credential | 같은 scope·같은 Secret·같은 HMAC 형식으로 호환성 유지 |

실제 새 Module의 Secret 접근과 LogSink 연결 성공은 8-E의 verification 재배포 뒤에 확인한다.
현재 8-D 완료는 이름·접근 권한 준비의 완료를 뜻한다.

## Production Secret 준비 — 8-F

Production에는 표 오른쪽 서비스 Secret 세 개와 환경 표시 이름 설정 한 개를 준비한다.
계정 이전 HMAC은 verification과 독립적이어야 하고,
LogSink HMAC은 Production UGS와 Production Cloud Run 사이에서만 같은 값을 사용한다.
Production URL은 Production LogSink의 HTTPS base URL이다. 값·URL·token은 보고에 포함하지 않는다.

### 8-F 진행 방법과 기록 기준

2026-10-21 사용자 요청으로 아래 작업을 한 항목씩 질문할 수 있게 나눴다.
진행 순서는 8-F-1 → 8-F-2 → 8-F-3 → 8-F-4 → 8-F-5 → 8-F-6이다.
예를 들어 `8-F-4-2 수행 방법을 알려줘`라고 요청하면 해당 작업만 구체적으로 안내한다.
완료 체크는 [Phase 4 Manual Steps](../AI/90_Tasks/Prototype_8/20261007_09_Phase4ManualSteps.md)의
동일 번호에 기록한다. 2026-10-21 사용자가 8-F-1-1의 Production 전용 프로젝트 생성·선택 완료를 보고했다.
지역 선택 화면이 없다는 보고는 프로젝트 생성 단계의 정상 동작이다. 후속 8-F-4-1 완료 보고에서
서울 `asia-northeast3` 선택을 확인해 8-F-1-1도 완료했다. Cloud Run의 실제 지역은 배포 시 별도 확인한다.
8-F-1-2 결제 계정 연결·예산 알림 설정도 같은 날 사용자 완료 보고로 완료 처리했다.
8-F-1-3의 네 API 활성화도 사용자 완료 보고로 완료 처리했다.
8-F-2-1은 같은 날 새 Production LogSink HMAC 생성·GCP/UGS 동일 값 저장·UGS Cloud Code 전용 접근·
클립보드 비우기 완료 보고를 받아 완료 처리했다. 실제 값은 수신·열람하지 않았다.
8-F-3-1은 같은 날 Production LogSink 전용 서비스 계정 생성·사용 설정 상태를 사용자 확인해 완료 처리했다.
8-F-3-2도 같은 날 해당 서비스 계정에 `FS_LOG_SINK_HMAC_V1` Secret 단위의 Secret Accessor 권한을
부여했다는 사용자 보고로 완료 처리했다. 8-F-3 전체 완료이며 실제 Secret 읽기 성공은 배포 시 확인한다.
8-F-4-1은 같은 날 `flow-state-logsink-production` 저장소의 Docker / 표준 / 서울 지역 생성 보고로 완료했다.
8-F-4-3은 같은 날 사용자 보고로 완료했다. Production Cloud Run은 서울·production-v1·요청 기반·
서비스 최소 0/최대 1·전용 서비스 계정·HMAC Secret 고정 버전 연결이며 상태 정상이다.
8-F-4-4는 같은 날 사용자 Production `/health` 응답 `204 No Content`로 완료했다.
8-F-5-1은 같은 날 사용자 Production `_Default` 로그 버킷 보관 기간 30일 확인으로 완료했다.
8-F-5-2의 프로젝트 범위는 같은 날 사용자 보고에서 기본 싱크 두 개·현재 프로젝트 동명 버킷 대상으로 확인했다.
후속 사용자 보고로 생성자 한 명만 있는 조직에 소속됨을 확인했다. 이어 조직 싱크 없음·중간 폴더
보이지 않음을 보고받아 조직의 추가 경로 없음·중간 폴더 확인 해당 없음으로 기록하고 8-F-5-2를 완료했다.
8-F-5-3은 사용자 보고에서 로그 조회 가능한 사람·그룹이 생성자 한 명뿐이고 수집용 서비스 계정에는
로그 조회 역할이 없으며 추가 검토할 권한도 없음을 확인해 완료했다. 별도 부정 접근 시험은 요구하지 않는다.
8-F-5 전체도 완료했다. 8-F-6-1은 새 Production 계정 이전 키 생성·`FS_TRANSFER_HMAC_V1` 저장·
Cloud Code 전용 접근·클립보드 비우기 사용자 보고로 완료했다. 값은 수신·열람하지 않았다.
8-F-6-2도 사용자 보고로 완료했다. UGS production의 `FS_LOG_SINK_URL_V1`에 Production HTTPS
기본 URL을 경로·포트 번호 없이 저장하고 Cloud Code 전용 접근을 설정했다. URL 자체는 수신하지 않았다.
8-F-6-3은 UGS production `FS_ENVIRONMENT_NAME_V1` 값 `production` 및 Cloud Code 전용 접근의
사용자 완료 보고로 확인했다. 8-F-6-4는 기존 보고를 AI가 대조해 네 항목의 이름·용도·환경·접근 권한,
GCP/API·LogSink 이미지/배포·Secret 연결·health 204·30일 보관·전송 경로·조회 권한 준비가 일치함을 확인했다.
8-F-6 및 8-F 전체 준비 완료다. 추가 사용자 재설정이나 값 열람은 필요하지 않다.
Production Module 배포·실제 이벤트 수집 확인과 source commit 연결·호환성 승인 등은 후속 조건으로 유지한다.

기존 자원이 있으면 먼저 이름·용도·설정을 확인하고 재사용 여부를 결정한다. 기존 Secret 값이
보이지 않는다는 이유로 덮어쓰거나 회전하지 않는다. 보고에는 값·URL·credential 대신 완료 여부만 적는다.

### 8-F-1 Production GCP 기반 준비

| 질문할 작업 번호 | 수행할 일 | 완료 기준 |
| --- | --- | --- |
| 8-F-1-1 | GCP 프로젝트 선택 메뉴에서 Production LogSink 전용 프로젝트 생성. 권장 표시 이름 `Flow State LogSink Production`, 지역 결정은 서울 `asia-northeast3`. | verification과 별도 프로젝트가 준비되고 지역을 결정함. 지역은 이후 저장소·서비스 생성 시 선택함. |
| 8-F-1-2 | 결제 계정을 연결하고 이 프로젝트의 전체 서비스 비용에 대한 예산 알림 설정. | 결제 연결·알림 수신 설정 확인. 무료 이용이나 고정 지출 한도를 보장하는 설정으로 해석하지 않음. |
| 8-F-1-3 | APIs & Services에서 Cloud Run Admin·Cloud Logging·Secret Manager·Artifact Registry 활성화. | 네 API가 Enabled임. |

### 8-F-2 Production 로그 수집 키 준비

**8-F-2-1은 생성과 두 곳 저장을 한 작업으로 처리한다.** 중간에 새 값을 생성하면 서로 달라질 수 있다.

1. 프로젝트 루트에서 사용자 전용 `node .\UGS\Verification\new-transfer-hmac-secret.cjs`를 한 번 실행한다.
   파일명에 transfer가 있지만 도구는 32 random bytes의 canonical Base64 키를 생성해 클립보드에 넣는다.
2. Production GCP Secret Manager에서 `FS_LOG_SINK_HMAC_V1`을 만들고 생성 값을 저장한다.
3. UGS **production** 환경에도 `FS_LOG_SINK_HMAC_V1`을 만들고 클립보드의 같은 값을 저장한다.
   설명은 `Production LogSink HMAC v1`, 서비스 액세스는 Cloud Code만 선택한다.
4. 두 곳 저장 후 `Set-Clipboard -Value ""`로 클립보드를 비운다.

완료 기준: Production GCP와 UGS에 같은 새 LogSink 키가 저장됨. 이 키는 verification·로컬 Docker·
계정 이전용 키와 별개다. 값 자체를 다시 조회해 보고하지 않는다.

### 8-F-3 Production LogSink 실행 계정 준비

| 질문할 작업 번호 | 수행할 일 | 완료 기준 |
| --- | --- | --- |
| 8-F-3-1 | IAM 및 관리자 → 서비스 계정에서 LogSink 전용 계정 생성. 권장 표시 이름 `Flow State LogSink Production`. | 전용 계정 존재. 프로젝트 역할과 JSON 키는 추가하지 않음. |
| 8-F-3-2 | Secret Manager의 `FS_LOG_SINK_HMAC_V1` 권한에서 해당 계정에 `Secret Manager Secret Accessor` 부여. | 해당 Secret 단위 접근 권한 확인. |

### 8-F-4 Production LogSink 배포

| 질문할 작업 번호 | 수행할 일 | 완료 기준 |
| --- | --- | --- |
| 8-F-4-1 | Production Artifact Registry에 Docker 저장소 생성. 권장 이름 `flow-state-logsink-production`, 서울 지역. | Production Docker 저장소 준비. |
| 8-F-4-2 | 정상 동작 중인 verification Cloud Run의 이미지 digest를 확인. 해당 이미지를 다운로드하고 Production 저장소 주소로 태그를 붙여 업로드. | 검증한 동일 이미지 복사 완료. 소스 재빌드 대신 digest 기준으로 대조. 실제 주소를 확인한 뒤 명령을 안내함. |
| 8-F-4-3 | Cloud Run에서 복사한 이미지로 Production 서비스 배포. 아래 설정표 적용. | 서비스 준비 완료·전용 계정 및 고정 Secret 버전 연결 확인. |
| 8-F-4-4 | Production 서비스 URL에 `/health`를 붙여 PowerShell 요청. | HTTP 204. 서버 실행 확인이며 HMAC 서명·로그 수집 성공 확인은 아님. |

8-F-4-2 비교 기준 보정 — 2026-10-21: 사용자 보고에서 verification-v3는 OCI image index이고,
Production production-v1은 단일 image manifest였다. Production manifest는 verification의 단일
manifest와 같은 digest다. 후속 사용자 `imagetools inspect` 출력으로 원본 index의 linux/amd64 항목과
Production manifest의 일치를 확인해 8-F-4-2를 완료했다. 함께 표시된 unknown/unknown 항목은
해당 실행 manifest를 참조하는 attestation-manifest이며 Production 실행 이미지로 선택할 대상이 아니다.
`docker buildx imagetools inspect`로 index의 `Manifests`와 `Platform`을 조회할 수 있다.
index 자체와 플랫폼 manifest의 digest는 다를 수 있으므로 앞선 단순 동일 digest 안내를 대체한다.

8-F-4-3 권장 설정:

| 설정 항목 | 입력 내용 |
| --- | --- |
| 서비스 이름 | `safe-log-sink-production` |
| 지역 | `asia-northeast3` |
| 결제 방식 | 요청 기반 |
| 서비스 확장 | 최소 0, 최대 1 — 초기 준비용 설정이며 비용 상한 보장은 아님 |
| CPU / 메모리 / 포트 | 1 vCPU / 512 MiB / 8080 |
| 서비스 계정 | 8-F-3에서 만든 Production LogSink 전용 계정 |
| 인그레스 / 인증 | 모두 / 공개 액세스 허용. 현재 Module은 Google ID token 대신 애플리케이션 HMAC 서명으로 인증함. |
| Secret 환경 변수 | `FLOW_STATE_LOG_SINK_HMAC` → GCP `FS_LOG_SINK_HMAC_V1`의 고정 버전 번호 |

8-F-4-4 요청 예시에서 자리표시자를 실제 서비스 URL로 바꾼다. 포트 번호는 붙이지 않는다.

```powershell
Invoke-WebRequest "Production_서비스_URL/health" -UseBasicParsing
```

### 8-F-5 Production 로그 관리 설정

| 질문할 작업 번호 | 수행할 일 | 완료 기준 |
| --- | --- | --- |
| 8-F-5-1 | Logging → 로그 스토리지에서 `_Default` 보관 기간 확인·설정. | 30일 보관 정책 적용. `_Required` 감사 로그는 별도 기본 정책 유지. |
| 8-F-5-2 | Logs Router에서 LogSink 이벤트가 다른 버킷·Cloud Storage·BigQuery·Pub/Sub로 복사되는 경로 확인. | 별도 내보내기 경로 없음. 상위 조직에서 적용한 경로가 있으면 함께 검토. |
| 8-F-5-3 | IAM에서 로그 조회 가능 주체와 상속 권한을 확인하고 운영 담당자에게 필요한 Logs Viewer 권한 적용. | 수집용 서비스 계정과 조회 권한 분리, 비운영 사용자에게 불필요한 조회 권한 없음. Owner/Editor 등 기존 광역 권한도 검토 대상. |

### 8-F-6 UGS production Secret 마무리

8-F-2의 로그 수집 Secret에 더해 아래 세 항목을 **UGS production** 환경에 준비한다.
모두 서비스 액세스는 Cloud Code만 선택한다.

| 질문할 작업 번호 | 수행할 일 | 완료 기준 |
| --- | --- | --- |
| 8-F-6-1 | 사용자 키 생성 도구를 새로 한 번 실행하고 별도 계정 이전 키를 `FS_TRANSFER_HMAC_V1`에 저장. 설명 `Production account transfer HMAC v1`. 저장 후 클립보드 비우기. | LogSink·verification 키와 독립적인 계정 이전 키 준비. 기존 동명의 Secret이 있으면 교체 여부부터 확인. |
| 8-F-6-2 | `FS_LOG_SINK_URL_V1`에 Production Cloud Run의 HTTPS base URL 저장. 설명 `Production LogSink base URL v1`. | `/health`·`/v1/events` 없이 기본 URL 저장. |
| 8-F-6-3 | `FS_ENVIRONMENT_NAME_V1`에 문자열 `production` 저장. 설명 `Production server environment name`. | 환경 표시 이름 보완 설정 준비. 이 항목은 무작위 키가 아님. |
| 8-F-6-4 | 서비스 Secret 세 개·환경 설정 한 개의 이름/접근 및 앞선 8-F 결과 대조. | 모두 준비 보고를 받아 8-F 완료 판정. |

과거 Production에 생성한 `FS_TRANSFER_HMAC_VERIFICATION_V1`은 현재 Production 조회 이름과 다르다.
이번 작업에서는 해당 항목의 값 변경·삭제 없이 새 이름의 준비 여부를 확인한다.

8-F 완료는 **Production LogSink와 연결 설정 준비 완료**다. Production Cloud Code Module 배포,
실제 Production 이벤트 수집·7필드 저장 검증 및 배포 승인 판정은 후속 단계다.
기존 `Invoke-VerificationSafeLogProbe.ps1`은 verification 전용이므로 Production 검증으로 대체 사용하지 않는다.
AI는 문서·코드 정적 대조를 담당하고, GCP·UGS 설정·배포·키 생성은 사용자가 수행한다.

## 남은 검증

로컬 전용 환경 분리 검사와 Module shim을 포함해 전체 Node 49파일을 통과했다. 기존 실패 5파일의
테스트 자료 수정 근거는 Step 8에 기록했다. 후속 Node 50파일 검사와 사용자 verification 검증은
8-E 완료 결과를 따른다. 8-F 준비도 완료했다. 남은 작업은 Production 보드 확인 → 새 source commit SHA
고정 → verification 결과 연결·호환성 검토/승인 → gate 재판정 → 승인 후 단일 Module 배포 순서다.
상태와 완료 기준은 [Phase 4 Manual Steps의 8-G-1~8-G-5](../AI/90_Tasks/Prototype_8/20261007_09_Phase4ManualSteps.md)에 모아 관리한다.
2026-10-21 8-G-1은 사용자 production 두 보드 설정 보고와 로컬 Board ID·정렬/keepBest 계약 대조로 완료했다.
다음 작업은 8-G-2의 새 source commit 고정이며, 원격 Module 배포 승인은 아직 아니다.

## 8-E 현재 결과와 LogSink Probe 실행 방법

2026-10-20 사용자가 verification Module 배포, Unity Script Compilation, 전체 Edit Mode·Play Mode 테스트
완료와 에러 없음을 보고했다. 테스트 실행 개수와 새 source commit은 아직 기록되지 않았다.
2026-10-21 계정 상태·순위 조회와 LogSink 이벤트 저장 확인까지 보고되어 Task 8-E를 완료했다.
아래 실행 방법은 재확인이 필요할 때 사용하는 절차다.

LogSink Probe는 Module의 `EmitVerificationSafeLogProbe`가 Cloud Run에 합성 점검 이벤트 한 건을 보내는
작업이다. 게임 기록이나 계정 이전을 실행하지 않는다. 6-3.6.3에서 이전에 성공한 확인을 새 배포본에 반복한다.

1. PowerShell을 프로젝트 루트에서 열고 아래 명령을 실행한다.

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\UGS\Verification\Invoke-VerificationSafeLogProbe.ps1 -Run
   ```

2. `UGS Service Account Key ID`, `UGS Service Account Secret Key` 입력란에는 이전 합성 Probe에서 사용한
   **UGS Service Account의 Key ID와 Secret Key**를 입력한다. 입력은 숨김 처리된다. 이 두 값은
   `FS_TRANSFER_*` 또는 `FS_LOG_SINK_*` HMAC 값이 아니다. 보유하고 있지 않으면 이 실행에 필요한
   Service Account credential을 먼저 준비해야 하며, 게임용 HMAC Secret을 대신 입력하지 않는다.
3. 성공 출력은 `PASS / VERIFICATION_SAFE_LOG_PROBE / Status=Accepted`다. 도구는 verification으로
   범위를 한정한 Stateless Token을 발급받아 고정 Module 함수에 JSON `{ "params": {} }`를 한 번 보낸다.
   `RateLimited`면 60초 뒤 한 번 재실행한다. 다른 실패에서는 출력된 안전한 FAIL 행만 보고한다.
4. Cloud Run의 verification LogSink 로그에서 최근 실행 시각을 확인한다. Logs Explorer에서는 아래를
   사용해 합성 이벤트를 찾을 수 있다.

   ```text
   resource.type="cloud_run_revision"
   jsonPayload.environment="verification"
   jsonPayload.reason="SyntheticProbe"
   ```

5. 해당 시각 request 로그의 `POST /v1/events`가 HTTP 202이고, 이벤트에 허용된 7필드만 있는지 확인한다.
   필드는 `environment`, `moduleReference`, `outcome`, `reason`, `durationBucket`, `correlationId`,
   `rateLimitResult`다. `outcome=Accepted`, `reason=SyntheticProbe`, `rateLimitResult=Allowed`를 확인한다.

보고할 내용은 `Probe Accepted`, `Cloud Run HTTP 202`, `7필드 확인`의 완료 여부뿐이다.
도구 파일은 Module 내부에 포함되지 않으므로 이 도구를 실행하기 위해 Module을 다시 배포하지 않는다.

### `Unavailable` 발생 후 진단 보정 — 2026-10-20

최초 실행에서 `Status=Unavailable`을 받았다. endpoint 인증·호출은 응답을 받았지만, 이 결과에는
실패 위치가 없어 원인을 확정할 수 없다. 합성 Probe의 C# 응답에 안전한 진단을 추가했으므로
이번에는 **보정 Module을 사용자 빌드해 verification에 재배포한 뒤** 같은 명령을 한 번 실행한다.
위의 '도구만 추가하면 재배포 불필요' 안내와 달리, 이 보정은 Module source 변경이다.

실패 출력에는 `Phase`, `Reason`, `HTTP`가 추가된다. 이전 배포본이면 `Unknown`으로 표시될 수 있다.

| Phase | 실패 위치 |
| --- | --- |
| `EnvironmentCheck` | 실행 환경 이름 검증 |
| `UrlSecretRead` / `HmacSecretRead` | 해당 Secret 조회 또는 조회 예산 |
| `UrlValidation` / `HmacValidation` | 반환받은 설정의 형식 검증 |
| `EventPreparation` | 안전 이벤트 작성·서명 |
| `HttpRequest` | Cloud Run 요청 또는 HTTP 응답 |

`Reason=Timeout`이면 해당 단계의 시간 제한, `Reason=HttpRejected`이면 HTTP 응답 거부다.
`HTTP=0`은 HTTP 응답을 확보하지 못했다는 뜻이며 Cloud Run 설정 오류의 증거는 아니다.
진단 FAIL 행만 공유하고 Secret·GCP 설정은 원인이 구분되기 전에 변경하지 않는다.

### `EnvironmentCheck` 실패 보정과 다음 실행

사용자 후속 결과는 `Phase=EnvironmentCheck / Reason=Failed / HTTP=0`이었다. 표시 이름이 정확히
두 문자열 중 하나여야 한다는 조건 때문에 Secret 조회 전에 중단됐다. 이름이 누락됐는지 다른 표기인지는
보고 결과로 구분되지 않지만, 그 메타데이터만을 필수 조건으로 삼은 로직은 수정했다.

1. 위 표에 따라 verification에 `FS_ENVIRONMENT_NAME_V1`을 값 `verification`, Cloud Code 전용으로 추가한다.
2. 이번 보정 Module을 사용자 빌드해 verification에 재배포한다.
3. 같은 `Invoke-VerificationSafeLogProbe.ps1 -Run` 명령을 한 번 실행한다.

새 코드는 context 이름을 Trim·소문자 정규화한 뒤 검사한다. 인식할 수 없는 이름이면 현재 호출 환경의
설정에서만 보완한다. 설정도 없거나 `verification`/`production` 외 값이면
`Phase=EnvironmentCheck / Reason=EnvironmentConfigurationUnavailable`로 실패한다.
조회 시간 제한이면 `Reason=Timeout`이다. 다른 환경으로의 조회·기본 Production 선택은 하지 않는다.

Cloud Save·Leaderboards 대상과 저장값 scope는 계속 context의 Project/Environment ID를 사용한다.
표시 이름 보완은 호출별 객체에만 보관하고, 별도 네트워크 재시도나 공유 cache는 추가하지 않았다.
이번 보정의 Node 회귀·정적 검사 50파일이 통과했으며 C# 컴파일·실제 Secret 조회는 사용자 검증 대기다.

### 보정 후 결과

2026-10-20 사용자가 같은 도구에서 `PASS / VERIFICATION_SAFE_LOG_PROBE / Status=Accepted`를 보고했다.
서버의 성공 반환 조건으로 HTTP 202 수신까지 확인했다.
2026-10-20 사용자는 일반 Play(`IsolationArmed=False`)의 Leaderboard에서 Loading 뒤 각 순위 내용이
정상 표시됨을 보고했다. 게임 화면 조회 결과는 Task 8-E-4-B에 기록했다.
2026-10-21 계정 상태는 `Ready / Reason=None / Consent=True / Pending=0 / LocalSave=Ready / RemoteAllowed=True`,
인증 완료와 공개 번호 응답 성공으로 확인했다. 같은 날 사용자가 제공한 2026-10-20 07:14:02 UTC의
Cloud Logging stdout 이벤트에는 위 7필드만 있었으며 verification / FlowStateVerification /
Accepted / SyntheticProbe / Allowed와 durationBucket=250, 비어 있지 않은 correlationId를 확인했다.
Task 8-E-4-A·8-E-5-B 및 8-E 전체를 완료했다. Production 배포 승인을 의미하지는 않는다.
격리 실행(`True`)은 게임 화면의 온라인 기능을 초기화하지 않으므로 게임 화면의 동의·Leaderboard 확인에
사용하지 않는다. 해당 모드의 Continue 뒤 Notice 반복은 일반 Play의 조회 실패 또는 원격 환경 혼선의 증거가 아니다.

인증 및 JSON 호출 계약의 근거: [Unity Service Account 인증](https://docs.unity.com/en-us/services-web-apis/service-account-auth),
[Cloud Code Client API](https://docs.unity.com/en-us/oas-cloud-code-client/1.0.0).

## 첫 Production 배포의 복구 기준

첫 Production 배포에는 이전 원격 Module이 없으므로 rollback을 요구하지 않는다.

1. 배포 또는 최초 읽기 전용 확인이 실패하면 공개를 진행하지 않는다.
2. 데이터 생성·삭제·환경 간 복사는 하지 않는다.
3. 원인을 수정한 새 Module commit으로 다시 배포한다.
4. 첫 배포가 성공해 Module reference가 생긴 뒤의 변경부터는 그 직전 reference를 rollback 후보로 기록한다.

이는 이전 버전을 되돌리는 rollback이 아니라, 첫 배포에만 적용하는 수정 후 재배포 방식이다.
