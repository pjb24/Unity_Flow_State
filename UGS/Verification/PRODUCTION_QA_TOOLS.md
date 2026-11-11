# Phase 5 Production QA 도구

Step 1 승인 범위에 한정한다. 운영자는 사용자 1인이며 별도 실행 시간대 관리는 하지 않는다.
역할 별칭은 **Production QA 출발**(이전 전)과 **Production QA 수신**(이전 후)이다.
이 문서는 도구 준비 절차이며 실제 서비스 성공 기록이 아니다.

## Production 대상 대조

`production-scope.cjs`는 Git 제외 `OnlineRecordProductionConfiguration.cs`의 Production 설정,
Project의 Cloud 연결을 로컬에서 대조한다.
환경 이름은 production, 인증 profile은 flow-state-production이어야 하며 verification 환경과 같으면 거부한다.
파일 누락·빈 값·중복 선언·불일치 시 중단한다. Project/Environment 식별자는 출력하지 않는다.
Editor 환경 선택은 이 도구의 대상 판정에 사용하지 않는다. 점검 요청은 비공개 Production 설정으로 고정하고,
Player는 `Windows-production`의 define으로 선택한다. Editor selector가 verification이어도 설정을 바꿀 필요 없다.

읽기 전용 사전 확인(인증·원격 요청 없음):

```powershell
node .\UGS\Verification\production-safe-log-probe.cjs --plan
```

`ScopeMatched`만 출력하며 설정값은 표시하지 않는다. 이 결과는 원격 설정·권한·Secret 읽기의 성공 근거가 아니다.

## Step 5: 합성 로그 점검 1건

1. UGS에서 승인 Project에 대한 점검용 Service Account Key ID/Secret Key를 비공개로 준비한다.
   Dashboard의 Service Accounts에서 Project에 필요한 Cloud Code 호출 권한만 부여한다.
   조직 전체 관리·Cloud Code 배포/삭제·Cloud Save 쓰기 권한은 이 도구에 필요하지 않다.
   실제 부여 권한을 Dashboard에서 확인하고 권한 오류 시 안전한 실패 분류만 보고한다.
   기존 verification 점검 도구의 대상은 변경하지 않으며 게임 HMAC Secret을 인증값으로 사용하지 않는다.
2. 프로젝트 루트의 PowerShell에서 사용자 본인이 실행한다.

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\UGS\Verification\Invoke-ProductionSafeLogProbe.ps1 -Run -ConfirmProduction
   ```

3. 두 credential은 숨김 입력한다. 명령 인수·채팅·Task·스크린샷에 넣지 않는다.
   Production으로 scope를 제한한 토큰 교환 1회 후 고정 Module/함수
   `FlowStateVerification.EmitVerificationSafeLogProbe`를 1회 호출한다. 함수 이름에 Verification이 있어도
   실제 환경은 Production 토큰 scope와 환경 중립 Module 설정으로 결정된다.
4. 성공은 `PASS / PRODUCTION_SAFE_LOG_PROBE / Status=Accepted / Phase=Completed / Reason=None / HTTP=202`이다.
   HTTP=202는 Cloud Code 바깥 요청의 200이 아니라 Module이 보고한 LogSink 수락 코드다.
   추가로 Phase 5 Step 5의 Logs Explorer 필터와 정확히 7필드 저장을 확인한다.
   합성 필드는 environment/moduleReference/outcome/reason/durationBucket/correlationId/rateLimitResult이며
   production/FlowStateVerification/Accepted/SyntheticProbe/Allowed를 확인한다.
5. 자동 재시도는 없다. RateLimited면 최소 60초 대기 후 승인된 수동 재실행 1회만 수행한다.
   다른 실패·timeout·응답 미확인은 중단하고 안전한 FAIL 행만 보고한다. 성공하지 않았다고 키를 회전하지 않는다.
6. Node 자식 프로세스의 credential 환경 변수는 즉시 제거하며 PowerShell은 finally에서 이전 환경을 복원하고
   SecureString/BSTR을 정리한다. 메모리의 모든 문자열을 암호학적으로 소거한다는 보장은 하지 않는다.
   새 점검 credential/추가 임시 권한은 Step 13에서 회수하며 런타임 LogSink identity/Secret은 유지한다.

인증·REST 계약 근거:
[Unity Service Account 인증](https://docs.unity.com/en-us/services-web-apis/service-account-auth),
[Cloud Code Module 인증](https://docs.unity.com/en-us/cloud-code/modules/how-to-guides/authentication),
[Cloud Code Client API](https://docs.unity.com/en-us/oas-cloud-code-client/1.0.0).

## Step 8~10: 비공개 자료 캡처·비교

모든 도구는 로컬 파일만 읽는다. SDK·네트워크·인증·서버 쓰기를 수행하지 않는다.
실제 원문은 사용자 PC의 `Ignore/Phase5Private` 아래에만 보관하고 Git/채팅에 올리지 않는다.
캡처 출력 이름은 `.json` basename만 허용하며 기존 기준 파일을 덮어쓰지 않는다.

### Local Save

Windows 사용자별 Player 저장소의 `flow-state-save.json`을 사용한다. 현재 Company/Product 설정에 따르면
각 QA Windows 사용자의 `%USERPROFILE%\AppData\LocalLow\DefaultCompany\Unity_Flow_State` 아래가 후보이며,
실제 파일 존재와 production scope는 도구가 검사한다. 실제 경로는 공유하지 않는다.
Player를 정상 종료한 상태에서 캡처한다. 실행 파일만 옮기거나 저장 파일 내용을 편집하지 않는다.

```powershell
$qaSaveFile = Read-Host '이 Windows QA 사용자의 flow-state-save.json 전체 경로(로컬 전용)'
node .\UGS\Verification\capture-production-snapshot.cjs save $qaSaveFile source-offline-before.json
```

다른 시점의 파일도 같은 방법으로 다른 이름에 캡처한다. 출발/수신 모두 같은 도구를 사용한다.
대기 생성 직후 종료하여 캡처한 파일과 Offline 재시작 후 종료하여 캡처한 파일을 비교한다.

```powershell
node .\UGS\Verification\compare-production-snapshots.cjs pending-restored .\Ignore\Phase5Private\source-offline-before.json .\Ignore\Phase5Private\source-offline-after.json
```

`pending-restored`는 기준 Pending이 1건 이상이어야 하며 ID·생성 시각·귀속·payload를 포함한 저장 자료가 같아야 통과한다.
`restart`는 같은 QA 계정 재시작의 전체 저장 자료 보존에 사용한다. settings/binding/tutorial도 비교하며
`pending-removed`는 원래 대기가 있고 나중에 0건인지만 확인한다. `REMOVED_ONLY`를 Submitted 증거로 사용하지 않는다.
그 경우 Player의 Submitted/Rejected 상태 및 서버 행 확인과 함께 판정한다.
재연결로 자동 전송되어 Offline 기준을 확보하지 못한 경우 추가 제출을 만들지 말고 해당 비교 미확인으로 기록한다.

### 서버 자료

승인된 QA 계정에 한해 Dashboard의 production 계정 value와 두 보드의 본인 행 JSON을 읽기 전용으로 로컬에 보관한다.
전후 자료에 서버 수락 시각·제출 ID가 없으면 성공으로 추정하지 않는다. 실제 파일을 공유하거나 서버 자료를 수정하지 않는다.
`capture-production-snapshot.cjs server`에 줄 로컬 manifest는 식별자 대신 아래처럼 **로컬 export 파일 경로**만 지정한다.
경로는 manifest 파일 위치 기준이며 없다는 사실을 확인한 보드만 null로 지정한다.

```json
{
  "accountFile": "account.json",
  "rowFiles": {
    "fs-stage-stage-001-r1": "stage-row.json",
    "fs-infinite-v2": "infinite-row.json"
  },
  "sourceBindingFile": "source-binding.json",
  "targetBindingFile": "recipient-binding.json"
}
```

account/binding 파일은 Cloud Save 단일 key의 `value` 본문 또는 `{ "value": 본문 }` JSON,
보드 파일은 해당 고정 owner의 단일 행 `playerId/score/metadata.acceptedAt/metadata.submissionId` JSON이다.
목록 전체·토큰·Secret·이전 코드를 넣지 않는다. 이전 전에는 binding 파일 두 항목을 생략한다.
파일명이 사용자마다 달라도 상관없다. 도구가 계정·scope·고정 행 소유권을 검증하고 snapshot을 자동 조립한다.

```powershell
$qaManifest = Read-Host '비공개 서버 export manifest 전체 경로(로컬 전용)'
node .\UGS\Verification\capture-production-snapshot.cjs server $qaManifest source-server-before.json
node .\UGS\Verification\compare-production-snapshots.cjs server-restart .\Ignore\Phase5Private\source-server-before.json .\Ignore\Phase5Private\source-server-after.json
node .\UGS\Verification\compare-production-snapshots.cjs transfer .\Ignore\Phase5Private\source-server-before.json .\Ignore\Phase5Private\recipient-server-after.json
```

출발 기준과 수신의 이전 후 snapshot을 비교하려면 두 Windows 사용자에게 접근 가능한 **비공개 로컬 저장 위치**에서
출발 기준 파일만 전달한다. 인증/Local Save를 수신의 Player 저장 위치에 복사하지 않는다.
기존 수신 계정은 이전 성공 시 `Detached`/currentPlayerId=null로 바뀌므로 활성 계정용 `server-restart`를 사용하지 않는다.
그 계정의 이전 후 자료는 같은 manifest 형식으로 `capture-production-snapshot.cjs server-detached`로 캡처하고,
활성 상태였던 이전 전 snapshot과 `compare-production-snapshots.cjs recipient-detached`로 비교한다.
이 비교는 원래 C/공개 번호/고정 owner·두 행/전체 metadata 보존, Detached 상태·연결 revision 1 증가를 검사한다.
원래 수신에 행이 없었음을 확인했다면 두 시점 모두 해당 보드의 rowFiles를 null로 지정하며 QA 기록을 일부러 만들지 않는다.
그 자료를 구할 수 없으면 미검증으로 기록하며 QA 기록을 추가 생성해 시험 조건을 만들지 않는다.

이전 비교는 계정·공개 번호·고정 owner·두 행 전체 metadata 보존, 활성 Player 변경, revision 1 증가,
출발 Inactive/수신 Active를 확인한다. 결과는 고정 분류만 출력하고 경로·JSON·식별자는 출력하지 않는다.
지원되지 않는 export 형태는 `INPUT_INVALID`로 중단하므로 원문을 채팅으로 보내지 말고 실패 분류만 보고한다.

## 로컬 검증

```powershell
node --test .\UGS\Tests\production-tools.test.cjs
powershell -NoProfile -ExecutionPolicy Bypass -File .\UGS\Tests\production-probe-wrapper.test.ps1
```

첫 명령은 합성 scope/credential과 fetch 대역만 사용한다. 두 번째는 node/숨김 입력을 함수 대역으로 바꿔
성공·실패 시 credential 환경 복원을 검사한다. Unity와 실제 UGS/GCP는 실행하지 않는다.
