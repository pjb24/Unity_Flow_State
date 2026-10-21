# 6-3.2 최소 권한과 로그 수집 Secret 분리 — 사용자

Step 8 환경 중립 source는 verification에서 아래 `_VERIFICATION_` 이름을 계속 사용한다.
기존 UGS·GCP 이름과 값은 그대로 유지한다. Production에서는 `FS_LOG_SINK_HMAC_V1`과
`FS_LOG_SINK_URL_V1`을 사용한다. 상세 환경별 구성은 `UGS/ENVIRONMENT_NEUTRAL_MODULE_CONFIGURATION.md`를 따른다.

이 단계는 verification LogSink에 필요한 symmetric HMAC과 최소 IAM만 구성한다. Cloud Run service,
Artifact Registry repository, Cloud Code Module 전송 연결은 아직 만들지 않는다.

## 가장 중요한 값 관계

로그 수집 HMAC은 **한 verification 연결 안에서는 동일한 값**이어야 한다.

| 위치 | 저장 값 | 이유 |
| --- | --- | --- |
| 로컬 Docker `.env` | `L` | 로컬 전용. 기존 값은 Cloud Run에 재사용하지 않는다. |
| GCP Secret Manager, verification LogSink | `V` | Cloud Run 수집기가 들어온 HMAC 서명을 검증한다. |
| UGS verification 환경 Secret, Cloud Code만 접근 | **같은 `V`** | 이후 Cloud Code Module이 동일한 body를 서명한다. |
| Production GCP/UGS Secret | 미래의 새 값 `P` | verification `V`와 분리한다. |
| 기존 account-transfer HMAC | 기존 값 유지 | 로그 수집 HMAC과 절대 재사용하지 않는다. |

즉, **GCP와 UGS의 verification 로그 수집 HMAC 값은 같고**, 로컬 Docker 값·Production 로그 값·계정
이전 HMAC 값과는 모두 달라야 한다. 같은 값은 두 보관소에 복사해 저장할 뿐, Task·Git·채팅·명령 인수에
입력하지 않는다.

### 기존 transfer HMAC을 바꾼 경우

`FS_TRANSFER_HMAC_VERIFICATION_V1`은 LogSink 설정 중에 수정하지 않는다. 이 Secret이 회전되면 이전
값으로 HMAC 처리된, 아직 만료되지 않은 verification 계정 이전 credential은 무효로 취급한다. 현재
Module에는 이전 transfer HMAC 값을 함께 허용하는 key ring이 없다. 활성 이전이 있었을 수 있으면 해당
credential은 새로 발급하고, Secret 값·기존 값은 복구 목적으로도 채팅·저장소에 넣지 않는다.

`UGS/LogSink/.env`의 `L`은 로컬 Docker와 `probe.ps1`만의 서명/검증 쌍이다. 로컬 파일 또는 개발 PC가
노출돼도 Cloud Run endpoint를 위조할 수 없도록 `V`와 분리한다. `L`을 실수로 `V`와 같게 만들었다면,
로컬 `.env`만 새 무작위 값으로 바꾸고 Docker local probe를 다시 통과시킨다. `V`를 다시 복사하거나
공유하지 않는다.

## 6-3.2-A — verification HMAC 생성

1. 로컬 Docker `.env`의 값을 보거나 재사용하지 않는다.
2. 새 32-byte 이상의 cryptographically-random 값을 한 번 생성한다. canonical Base64 32-byte 형식이면
   기존 HMAC 운영 규칙과 일관된다.
3. 생성 직후 값을 두 번만 사용한다: GCP Secret Manager의 새 secret version, UGS verification 환경의
   새 Environment Secret value. 화면·클립보드·파일·채팅에 남기지 않는다.
4. 두 화면에 저장한 뒤 secret value 표시 화면을 닫는다. 값 일치 확인을 위해 다시 열람하지 않는다.

권장 secret 이름은 두 보관소 모두 `FS_LOG_SINK_HMAC_VERIFICATION_V1`이다. 이름은 값이 아니며,
version/환경/목적을 명시한다. 기존 `FS_TRANSFER_HMAC_VERIFICATION_V1`은 계정 이전용이므로 사용하지
않는다.

## 6-3.2-B — GCP Secret Manager

1. verification LogSink GCP project가 선택된 상태에서 **Security > Secret Manager**로 이동한다.
2. **Create secret**을 선택하고 이름에 `FS_LOG_SINK_HMAC_VERIFICATION_V1`을 입력한다.
3. 6-3.2-A에서 방금 생성한 `V`를 secret value에 한 번만 붙여 넣고 Create 한다.
4. secret 상세 페이지의 **Permissions** 탭에서, 이 LogSink 전용 user-managed service account에만
   `Secret Manager Secret Accessor` (`roles/secretmanager.secretAccessor`)를 **이 secret resource에만**
   부여한다. project 전체에 부여하지 않는다.
5. 다른 principal에는 secret payload access 권한을 추가하지 않는다. 사용자 본인의 project Owner 권한은
   별도 관리 문제이며, staging 실험을 위해 새 Viewer/Editor/Accessor principal을 추가하지 않는다.

Cloud Run은 아직 없지만 service account와 secret-level binding은 먼저 만들 수 있다. Cloud Run service가
배포되는 6-3.4에서 이 service account를 service identity로 연결하고, secret은 환경 변수
`FLOW_STATE_LOG_SINK_HMAC`에 **고정된 version**으로 주입한다. `latest`는 사용하지 않는다.

## 6-3.2-C — GCP user-managed service account

1. **IAM & Admin > Service Accounts**에서 **Create service account**를 선택한다.
2. 표시 이름은 `Flow State LogSink Verification`처럼 목적만 드러내고, service account ID에는 개인 정보나
   비밀 정보를 넣지 않는다.
3. 생성 화면의 project role 부여 단계는 건너뛴다. `Owner`, `Editor`, `Logging Writer`를 부여하지 않는다.
4. 생성 뒤 6-3.2-B의 특정 secret Permissions에서만 `Secret Manager Secret Accessor`를 부여한다.
5. service account key(JSON key)는 만들거나 다운로드하지 않는다. Cloud Run에는 나중에 이 identity를
   직접 연결한다.

Cloud Run은 stdout/stderr container log를 자동 수집한다. 따라서 이 수집기의 service identity에
`Logging Writer`를 부여하지 않는다. 이 service identity의 필요한 권한은 현재 한 secret의 accessor뿐이다.

## 6-3.2-D — UGS verification Environment Secret

1. Unity Dashboard에서 **verification** 환경인지 다시 확인한다.
2. 개발 환경 Secret에 `FS_LOG_SINK_HMAC_VERIFICATION_V1`을 만든다.
3. 6-3.2-A에서 만든 **같은 `V`**를 값으로 입력한다.
4. 서비스 액세스는 **Cloud Code만** 선택한다. Client, 다른 서비스, 사람이 읽는 운영 문서에는 권한을
   주지 않는다.
5. 설명은 `Verification LogSink HMAC v1`처럼 값 없는 설명만 사용한다.

이 단계에서는 Cloud Code Module이 아직 이 secret을 읽거나 endpoint로 호출하지 않는다. UGS Secret
생성은 값이 저장됐다는 준비 사실만 뜻한다.

## 6-3.2-E — 완료 확인 및 중단 기준

다음 안전 메타데이터만 확인·보고한다.

```text
6-3.2 / Verification LogSink HMAC generated separately from local, production, and transfer HMAC: PASS|BLOCKED
GCP secret stored; Secret Accessor is granted only to LogSink service account at secret level: PASS|BLOCKED
UGS verification secret stored; service access is Cloud Code only: PASS|BLOCKED
No JSON key, Cloud Run service, repository, endpoint, Module connection, or Production change: PASS|BLOCKED
```

secret value, service-account email, Project ID, endpoint, secret version number, IAM policy export, request body는
공유하지 않는다. 잘못된 환경 선택, 기존 transfer HMAC 재사용, service account key 생성, project-wide
Secret Accessor, 예상 밖 IAM 권한이 나타나면 값을 저장하지 말고 중단한다.

## 공식 참고

- [Cloud Run secrets configuration](https://docs.cloud.google.com/run/docs/configuring/services/secrets)
- [Secret Manager IAM access control](https://docs.cloud.google.com/secret-manager/docs/access-control)
- [Cloud Run service identity](https://docs.cloud.google.com/run/docs/configuring/services/service-identity)
- [Cloud Run logging](https://docs.cloud.google.com/run/docs/logging)
