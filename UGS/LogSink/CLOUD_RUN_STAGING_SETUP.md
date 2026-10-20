# 6-3.1 Cloud Run verification staging 기반 구성 — 사용자

이 문서는 로컬 Docker 완성도 확인 뒤의 **기반 구성만** 다룬다. Cloud Run 서비스 배포는 6-3.4,
로그 수집 HMAC 생성·Secret 주입은 6-3.2, Cloud Code Module 연결은 6-3.5에서 수행한다.
Project ID, endpoint, IAM principal, Secret 값은 이 저장소나 채팅에 기록하지 않는다.

## 권장 대상

- 별도 GCP project: verification LogSink 전용이다. Production 또는 개인 실험 자원과 섞지 않는다.
- 권장 Cloud Run region: 한국 초기 출시 대상이면 `asia-northeast3` (Seoul). 출시 대상·데이터
  소재 요구가 다르면 그 요구를 우선한다.
- Cloud Logging 대상: 이 staging project의 안전 이벤트 전용 로그 경로다. 30일 보존은 6-3.3에서
  별도로 설정·대조한다.

Cloud Run은 `asia-northeast3`을 지원한다. 별도 project를 써야 테스트 자원과 billable resource를
독립적으로 제거·관리할 수 있다.

## 6-3.1-A — GCP project 및 결제 경계

1. Google Cloud Console의 project selector에서 새 project를 만든다. display name에는
   `Flow State LogSink Verify`처럼 목적과 verification을 식별할 수 있는 30자 이하 이름을 사용한다.
2. 이 project만 대상으로 결제 계정을 연결한다. 무료 할당량을 넘어설 가능성이 있으므로 아래 두 예산을
   별도로 설정한다.
   - **Spend cap enforcement**: scope는 이 project 하나와 서비스 **Cloud Run** 하나로 선택한다.
     Cloud Run Functions, Gemini API, Vertex AI는 선택하지 않는다. 한도에 도달하면 이 project의 Cloud
     Run workload가 일시 중지되어 새 요청은 실패할 수 있다. verification staging이므로 이 중단은
     허용되는 보호 동작이다. 집행은 즉시적이지 않아 소액 초과 비용은 생길 수 있고, 계산은 credits를
     제외한 gross estimated cost 기준이므로 무료 할당량이 있어도 낮은 한도는 일찍 중단될 수 있다.
   - **Alerts only**: scope는 같은 project의 **all services**로 둔다. Cloud Logging, Artifact Registry,
     Secret Manager 등 Cloud Run spend cap 밖 서비스의 비용을 이 예산으로 감시한다. 50%, 80%, 100%
     이메일 알림을 켠다. alerts-only는 사용을 중지시키지 않는다.
   Spend cap 금액은 발생해도 감당 가능한 월 최대 비용보다 낮게 정한다. 0원 보장을 제공하는 기능은
   아니므로, 알림과 사용량 확인을 병행한다.
3. 선택한 region, project가 verification 전용이라는 사실, 결제 알림 설정 여부만 승인된 내부 inventory에
   기록한다. Project ID, billing account ID, 예산 액수는 저장소·채팅에 기록하지 않는다.

## 6-3.1-B — 필요한 API만 활성화

Console의 **APIs & Services > Library**에서 다음 API를 이 project에만 활성화한다.

| API | 6-3.1에서 필요한 이유 |
| --- | --- |
| Cloud Run Admin API (`run.googleapis.com`) | 6-3.4의 컨테이너 서비스 배포 대상 |
| Cloud Logging API (`logging.googleapis.com`) | Cloud Run stdout의 안전 이벤트 저장·조회 기반 |
| Secret Manager API (`secretmanager.googleapis.com`) | 6-3.2의 HMAC 비공개 주입 기반 |
| Artifact Registry API (`artifactregistry.googleapis.com`) | 6-3.4의 private container image 저장소 기반 |

Cloud Build API는 Cloud Build를 실제 선택할 때만 추가한다. 현재 로컬 Docker image를 Artifact Registry에
push하는 경로에는 이 단계에서 필요하지 않다.

## 6-3.1-C — 완료 확인 및 중단 기준

다음만 확인한다.

- verification 전용 GCP project가 선택되어 있다.
- 위 네 API가 **Enabled** 상태다.
- 선택 region이 운영 대상과 맞는다.
- Cloud Run service, Artifact Registry repository, Secret 값, IAM role binding, Cloud Code Module은 아직
  만들거나 변경하지 않았다.

다른 project가 선택되었거나, 결제/region 조건이 불명확하거나, 조직 정책이 API 활성화를 막으면 여기서
중단한다. Project ID나 오류 전문을 공유하지 말고, 안전한 오류 분류만 보고한다.

## 완료 보고 형식

다음 형식의 비민감 결과만 기록한다.

```text
6-3.1 / Verification GCP project isolated: PASS|BLOCKED
Region decision recorded: PASS|BLOCKED
Required APIs enabled (Run, Logging, Secret Manager, Artifact Registry): PASS|BLOCKED
No service/repository/secret/IAM/Module deployment performed: PASS|BLOCKED
```

## 공식 참고

- [Cloud Run locations](https://docs.cloud.google.com/run/docs/locations)
- [Enable the Logging API](https://docs.cloud.google.com/logging/docs/api/enable-api)
- [Enable the Secret Manager API](https://docs.cloud.google.com/secret-manager/docs/configuring-secret-manager)
- [Deploy container images to Cloud Run](https://docs.cloud.google.com/run/docs/deploying)
