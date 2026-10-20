# Safe Log Sink — Docker 개발과 Cloud Run 운영 공용

이 수집기는 Phase 4의 안전 이벤트 일곱 필드만 받고, HMAC 서명·5분 timestamp window를
검사한 뒤 JSON 한 줄을 stdout에 출력한다. Cloud Run에서는 이 stdout이 Cloud Logging으로
수집된다. raw request body와 거부 사유는 출력하지 않는다.

## 로컬 Docker 준비 — 사용자

1. `.env.example`을 `.env`로 복사한다. `.env`는 Git에 포함되지 않는다.
2. **새로운** 32자 이상 로그 수집 전용 HMAC을 `.env`에만 넣는다. 계정 이전 Secret은 재사용하지 않는다.
3. `docker compose up --build`로 로컬에서만 시작한다. `127.0.0.1` 바인딩이므로 인터넷에는 노출되지 않는다.
4. 아래의 Cloud Run 전 로컬 완료 기준을 모두 확인한다. 실제 Player·Secret·요청 본문은 쓰지 않는다.

컨테이너가 실행 중인 별도 PowerShell에서 아래 probe를 실행하면 모든 HTTP 상태 코드만 출력한다.

```powershell
.\probe.ps1
```

이 probe는 로컬 `.env`의 HMAC을 메모리에서만 읽고 Secret·서명·요청 본문을 출력하지 않는다.
Windows PowerShell 5.1 및 PowerShell 7 모두에서 동작하도록 필요한 표준 HTTP 어셈블리를 probe가
직접 로드한다.

AI는 Docker 실행, Secret 생성·입력, Cloud Run 배포를 수행하지 않는다.

Cloud Run 배포는 아래 로컬 완료 기준을 모두 통과한 뒤에만 수행한다. 로컬 확인이 진행 중인 동안에는
외부 endpoint·Cloud Run·GCP Secret/IAM을 만들거나 Module 전송을 연결하지 않는다.

### Cloud Run 전 로컬 완료 기준

- `GET /health`가 `204`를 반환한다. Cloud Run 앞단에서 `/healthz`가 가로채지는 사례가 있으므로
  로컬 및 Cloud Run 상태 확인 경로를 `/health`로 통일한다.
- 올바르게 서명된 합성 안전 이벤트가 `202`를 반환하고, Docker stdout에는 그 일곱 필드만 나온다.
- 추가 필드가 있으면 `400`, 잘못된 HMAC 또는 5분 초과 timestamp면 `401`을 반환한다.
- 잘못된 JSON은 `400`, 2 KiB 초과 body는 `413`, JSON이 아닌 content type은 `415`, 다른 경로는 `404`를 반환한다.
- `docker compose down` 후 다시 기동해 위 health와 합성 성공을 재확인한다.
- Docker stdout/stderr에 HMAC, request body, Player 식별자, 원시 오류 전문이 없는지 확인한다.

## 요청 계약

- `POST /v1/events`
- `Content-Type: application/json`
- `X-Flow-State-Timestamp`: Unix milliseconds, 현재 시각 ±5분
- `X-Flow-State-Signature`: Base64(HMAC-SHA256(`timestamp + "." + raw UTF-8 body`))
- body: 정확히 `environment`, `moduleReference`, `outcome`, `reason`, `durationBucket`, `correlationId`, `rateLimitResult`

성공은 `202`, 잘못된 schema/JSON은 `400`, 인증/만료/서명 불일치는 `401`, 2 KiB 초과는 `413`,
JSON 이외 content type은 `415`이다. 수집기는 오류 본문을 반환·기록하지 않는다. 5분 안의 동일 요청
재전송 방지는 이 최소 수집기에 구현하지 않았다. 이는
안전 로그의 중복 가능성만 만들며 게임 데이터 변경 권한은 제공하지 않는다.

## Cloud Run 수동 배포 전제

6-3.1의 project/API/region 기반 구성은 [CLOUD_RUN_STAGING_SETUP.md](CLOUD_RUN_STAGING_SETUP.md)를
먼저 따른다. 이 문서의 실제 service 배포는 6-3.4에서만 수행한다.

1. GCP 전용 프로젝트를 만들고 Cloud Run과 Cloud Logging을 활성화한다.
2. Cloud Run service identity에는 LogSink HMAC 한 개의 Secret Manager Secret Accessor만 secret 수준으로
   부여한다. stdout/stderr 로그는 Cloud Run이 자동 수집하므로 Logging Writer 권한은 부여하지 않는다.
   운영 담당자의 조회 권한과 분리한다.
3. 수집 HMAC은 Cloud Run Secret Manager와 UGS Cloud Code Secret에 각각 별도 보관한다. 값은 이 저장소,
   Docker image, build argument, command history에 넣지 않는다.
4. Cloud Run의 `PORT` 환경 변수와 이 컨테이너의 기본 `8080`은 호환된다. 최소 instance는 0으로 둔다.
5. Cloud Logging 보존을 30일로 대조하고, export/archive/장기 backup을 만들지 않는다.
6. endpoint에는 query string을 사용하지 않고, 수집기 외의 상세 요청 로그를 추가하지 않는다.

Cloud Code Module에서 이 endpoint로 안전 이벤트를 보내는 코드는 아직 연결하지 않았다. endpoint와
Secret이 준비된 뒤 별도 승인된 변경으로 구현하고 verification에서 합성 이벤트만으로 점검한다.

## 무비용 한계

Cloud Run과 Cloud Logging의 무료 할당량 안에서는 0원이 될 수 있지만, 한도 초과 비용을 기술적으로
0으로 보장하지는 않는다. 결제 예산 경보, 최소 로그량, `max instances` 제한을 설정하고 사용량을
운영자가 주기적으로 대조한다.
