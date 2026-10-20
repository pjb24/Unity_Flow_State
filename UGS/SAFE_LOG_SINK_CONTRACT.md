# Phase 4 안전 로그 sink 계약

## 상태

`PRODUCTION_DEPLOY_BLOCKED` (`ExternalThirtyDaySinkRequired`) — Unity Cloud Code 기본 로그는
7일 보존이므로 30일 보존 및 삭제 요건의 증거가 될 수 없다. 조직 승인 외부 sink가 아직
선정·설정·검증되지 않았다.
선정 아키텍처는 로컬 Docker 개발과 Google Cloud Run 운영에 공통인 이 저장소의
`UGS/LogSink` 수집기다. Cloud Run의 stdout은 Cloud Logging으로 보내며, 실제 프로젝트·endpoint·
자격 증명·Secret 값은 이 문서에 기록하지 않는다.

`FlowStateVerification` C# Module에는 이 schema 전용 송신 경계가 있다. 정상 Module 요청 완료 뒤
한 번, 예외로 끝난 요청에서는 안전한 실패 분류로 한 번만 전송을 시도한다. 별도
`EmitVerificationSafeLogProbe`는 입력을 받지 않고 Player·계정 자료를 읽거나 전송하지 않는 고정 합성
이벤트용 verification 점검 함수이며, Module process마다 최소 60초 간격으로만 허용한다. Cloud Code의
인증된 호출 경계는 유지하지만, 그 인증 주체 값은 이 함수·이벤트에서 사용하지 않는다. URL과 HMAC은 각각
`FS_LOG_SINK_URL_VERIFICATION_V1`, `FS_LOG_SINK_HMAC_VERIFICATION_V1`에서만 런타임에 읽으며,
embedded JavaScript에는 이 두 Secret을 읽는 경로를 제공하지 않는다. 전송 경로는 `/v1/events`로
고정하고, HTTPS Cloud Run URL·HTTP 202·250 ms 이내의 남은 요청 예산을 확인한다. 송신 실패는
상수 `SafeLogSinkUnavailable`만 Cloud Code 로그에 남기며, 기존 게임 요청의 성공 응답을 실패로
바꾸지 않는다.

일반 게임 요청의 전송 시도는 최대 250 ms를 유지한다. Player·계정 자료를 사용하지 않는
`EmitVerificationSafeLogProbe`만 Cloud Run 최소 인스턴스 0의 첫 기동과 Secret 조회를 확인할 수 있게
최대 4000 ms를 사용하며, 전체 Cloud Code 요청 budget 5000 ms를 넘지 않는다.

이 소스 변경은 verification Module 배포 전까지 원격 수집을 시작하지 않는다. 따라서 배포 및 합성
이벤트 확인 전에는 Production 배포 gate를 열지 않는다.

## 수집 schema

아래 일곱 필드만 한 이벤트에 허용한다. `UGS/CloudCode/operational-safety-policy.js`의
`isSafePhase4StructuredLog`가 같은 closed schema를 정적으로 검사한다.

| 필드 | 형식 | 의미 |
| --- | --- | --- |
| `environment` | non-empty string | 환경의 표시 이름만. 식별자·Project ID는 넣지 않는다. |
| `moduleReference` | non-empty string | 배포 단위의 안전한 module/version reference. 원격 source 본문은 넣지 않는다. |
| `outcome` | non-empty string | 안전한 결과 분류. |
| `reason` | non-empty string | 안전한 사유 분류. |
| `durationBucket` | zero 이상의 정수 | 처리 시간의 구간값. 원시 시각이나 요청 내용은 넣지 않는다. |
| `correlationId` | non-empty string | 서버가 생성한 불투명 상관관계 ID. Player/계정/공개 번호에서 유도하지 않는다. |
| `rateLimitResult` | non-empty string | 허용 또는 제한 결과 분류. |

현재 송신기는 `environment=verification`, `moduleReference=FlowStateVerification`,
`outcome=Completed` 또는 `Failed`, `reason=ModuleInvocation` 또는 `ModuleFailure`,
`rateLimitResult=NotApplied`만 만든다. `durationBucket`은 250 ms 단위이며 최대 5000 ms이고,
`correlationId`는 매 전송마다 서버가 새로 만든 UUID다.
합성 점검 함수는 `outcome=Accepted`, `reason=SyntheticProbe`, `rateLimitResult=Allowed`만 만든다.

허용되지 않는 키는 schema 단계에서 거부한다. 특히 Secret·token·HMAC·digest, transfer
code/verification credential, Player ID·계정 ID·공개 번호, raw request/response body,
Cloud Save 값, 원시 오류 전문 및 원격 source를 수집하지 않는다. sink의 필드 확장·원시
오류 자동 첨부·payload 캡처도 사용하지 않는다.

## 30일 보존 및 삭제 계약

선정할 외부 sink는 다음 모두를 만족해야 한다.

1. 위 closed schema 이벤트만 수신하고, 수신 자격 증명은 승인된 비공개 보관소에서만 관리한다.
2. 이벤트는 수신 시점부터 정확히 30일 보관한 뒤 자동 삭제한다. 이 이벤트의 archive,
   export, backup 및 별도 장기 보관 경로는 만들지 않는다.
3. 조회 권한은 지정된 운영 담당자에게만 부여하며, ingestion 권한과 조회 권한을 분리한다.
   역할·담당자·권한 검토일은 승인된 내부 운영 기록에만 남긴다.
4. 자동 삭제가 제공되지 않으면 30일 도래 이벤트를 식별·삭제하고 결과를 남기는 수동 runbook을
   설정한다. 삭제 실패는 Production deploy gate를 닫는 조건이다.
5. 로그 제품명, endpoint, 접근 주체 식별값, credential 및 실제 이벤트 내용은 이 저장소에
   기록하지 않는다.

## 사용자 수동 작업 — Step 6-2 / 6-3

1. 조직 승인 sink와 운영 담당자를 정하고, 위 schema만 허용하는 ingestion 규칙을 만든다.
2. retention 30일 및 자동 삭제(또는 승인된 수동 삭제 runbook)를 설정하고, archive/export/backup의
   장기 보관이 없음을 내부 운영 기록에서 대조한다.
3. verification에서 합성된 안전 이벤트 한 건으로 ingestion, 담당자 조회 가능, 비담당자 조회
   불가, 삭제 규칙을 확인한다. 실제 Player, Secret, 요청/응답 본문을 사용하지 않는다.
4. 다음의 민감하지 않은 결과만 보고한다: sink가 승인되었는지, retention=30일인지,
   삭제 방식(자동/수동)과 점검 결과, 접근 제어 점검 결과. 값·endpoint·이벤트 본문은 공유하지 않는다.

### verification 합성 Probe 호출

`EmitVerificationSafeLogProbe`는 Cloud Code의 인증된 호출이 필요하다. 합성 점검에는 실제 Player
인증 대신 verification 전용 Unity Service Account에서 발급한 짧은 수명의 Stateless Token을 사용한다.
서비스 계정 key ID·secret key·Authorization header·Stateless Token은 Secret이며, 화면에 한 번만
표시되는 값도 이 저장소·채팅·명령 기록에 넣지 않는다.

1. Unity Dashboard 조직 관리에서 verification 전용 Service Account를 만들고, Module 호출에 필요한
   최소 project role만 부여한다. 더 좁은 caller role이 Dashboard에 없으면 Editor/Admin 권한을 임의로
   부여하지 말고 필요한 역할 후보만 내부 승인 절차로 확인한다.
2. Service Account key 생성 화면이 제공한 `Authorization header`를 그대로 Token Exchange API의
   `Authorization` header로 사용해 짧은 수명의 Stateless Token을 얻는다. Key ID와 Secret Key를
   로컬에서 다시 조합하는 것보다 입력 오류를 줄인다. Cloud Code Client API 호출에는 environment ID를
   포함한다.
3. Module의 `EmitVerificationSafeLogProbe` endpoint에 POST를 한 번 보낸다. `Content-Type`은
   `application/json`이며 body는 빈 객체 `{}`만 사용한다. 함수 매개변수가 없으므로 Player·계정·게임 요청
   데이터를 넣지 않는다. `Accepted`만 수집 성공 후보로 본다.
4. `RateLimited`면 Module process의 60초 guard에 따른 결과이므로 60초 뒤 한 번만 재시도한다.
   `Unavailable`이면 Cloud Run/UGS Secret 설정을 바꾸지 말고 HTTP 상태와 안전한 실패 분류만 기록한다.

Cloud Code Module은 인증된 호출만 수락하며, Service Account를 Stateless Token으로 교환해 Client
API를 호출할 수 있다. 구체적 API 경로와 토큰 값은 운영 비공개 보관소에서만 관리한다.

Cloud Run 수동 설정 및 Docker 수집기 사용법은 `UGS/LogSink/README.md`를 따른다. Cloud Code Module의
실제 전송 코드는 endpoint와 Secret이 준비된 뒤 별도 승인된 변경으로 구현한다. credential은 공유하지 않는다.
Cloud Run verification staging 기반 구성은 `UGS/LogSink/CLOUD_RUN_STAGING_SETUP.md`의 6-3.1 절차를
따른다.
6-3.2의 LogSink HMAC 값 관계와 secret-level 최소 IAM 절차는
`UGS/LogSink/CLOUD_RUN_SECRET_AND_IAM.md`를 따른다.

30일 경과를 기다릴 수 없다면 sink가 제공하는 TTL/retention 설정 화면 또는 삭제 runbook의
승인 기록으로 설정을 대조한다. 설정을 입증할 수 없거나 합성 이벤트가 schema 밖 필드를
포함하면 `PRODUCTION_DEPLOY_BLOCKED`를 유지한다.

## 참고

- Unity Cloud Code Logging overview: <https://docs.unity.com/en-us/cloud-code/logging/overview>
- Unity Cloud Code Logs in Dashboard: <https://docs.unity.com/en-us/cloud-code/logging/tutorials/retrieve-logs/unity-dashboard>
