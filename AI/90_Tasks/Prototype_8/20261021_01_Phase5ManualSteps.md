# Prototype 8 - Phase 5 통합 검증과 공개 운영 후보 판정

## 작업 정보

- 작업일: 2026-10-21
- 범위: Flow State / Prototype 8 / Phase 5
- 담당: AI(정적 대조·Unit Test·도구/절차 준비), 사용자(Unity 컴파일/Test Runner/Build·실제 서비스 요청·화면 조작)
- 선행: [Phase 4 Manual Steps](20261007_09_Phase4ManualSteps.md) Step 10 완료.
- 기준 source: Git `8774b2db2defa76c7e6a4ddd73f0f37f187c7cee`.
- 상태: **Task 문서 작성 완료 / Step 1~13 실행 미착수 / Production 검증 실행 승인 대기**.

## 작업 목적과 작업 전 상태

실제로 빌드한 Windows Player에서 계정·기록·Leaderboard·Offline 복구·UI를 확인하고,
Production 서비스와 연결한 공개 운영 후보의 근거를 모은다. 테스트로 판정 가능한 로직을 사람이 반복 확인하지 않는다.

Phase 4에서 환경 중립 Module, 별도 Production Secret/보드/Cloud Run LogSink, 30일 보관 정책과
운영 조회 권한을 준비했고 사용자가 production의 `FlowStateVerification` Module 하나를 배포했다.
verification 계정/순위/합성 로그 수집·Unity 컴파일/전체 테스트 성공은 기존 근거로 보존한다.
다음 항목은 아직 확인하지 않았다.

- Production Module의 실제 호출·Secret 읽기·서명 로그 수집 성공.
- Production Windows Player Build 및 그 Player의 계정·제출·조회·Offline 복구.
- 실제 배포 복구 후 데이터 보존. 첫 배포 이전 Production Module은 존재하지 않는다.
- 격리 Editor 실행에서 Continue 후 Online Record Notice가 반복되는 UI의 개선 또는 수용 여부.

문서 작성은 위 작업의 실행이나 일반 사용자 공개 승인이 아니다.

## 조사 내용과 적용 경계

- Module reference는 `Assets/CloudCode/FlowStateVerification.ccmr`이며 C# host와 embedded JS를 함께 배포한다.
- `OnlineRecordConfiguration`의 `FLOW_STATE_PRODUCTION` 분기가 Player의 환경을 정한다.
  Deployment 창의 환경·GCP 프로젝트 선택은 Player의 환경 선택을 대신하지 않는다.
- Production 기본 인증 프로필은 승인된 Production 설정을 따른다. `--fs-verification-session=A/B/Legacy/Phase3New`는
  verification 전용이며 Production 새 계정 생성용 인수로 사용하지 않는다.
- 기존 `Invoke-VerificationSafeLogProbe.ps1`, Online Verification A/B 창, live storage probe는 verification 전용이다.
  이 도구를 수정해 Production 대상에 바로 실행하는 절차는 제공하지 않는다.
- Windows에서는 실행 파일 폴더만 복사해도 Local Save·Authentication 저장이 분리된다고 보장할 수 없다.
  새 계정/두 기기 검증은 전용 테스트 기기·OS 사용자 등 실제로 격리된 실행 공간을 확보한다.
- 실제 식별자·Secret/인증 키·token·Player ID·이전 코드/인증값·원시 저장/로그는 Task·Git·채팅에 기록하지 않는다.
  번호·기준 자료는 필요한 경우 로컬 비공개 비교에만 사용하고 결과만 보고한다.

## 검증 책임 분리

| 대상 | AI가 먼저 수행할 정적 검사·Unit Test | 사용자에게 남기는 실제 확인 |
| --- | --- | --- |
| 환경/계정 격리 | Build Profile define·승인 설정·프로필 allowlist·Local Save/Pending scope·오환경 거부 | 승인한 Player와 QA 계정으로 실행 |
| 계정 준비/이전 | 동의 저장 실패·인증/서버 timeout·이전 만료/취소/재발급·CAS·A/B 연결·stale 응답·최고 기록 보존 | 실제 두 실행 공간에서 번호 표시·한 번의 승인된 이전·재시작 |
| 기록 제출/조회 | 점수 검증·멱등성·응답 유실·동일 제출 ID·TOP 10/AROUND 7·동점·100행 초과·조회 오류 상태 | 실제 플레이로 두 Mode 제출 및 TOP/AROUND YOU 표시 |
| Offline/Pending | v7 migration·180일 경계·가짜 시계·저장 실패·재시작·중복 제거 | 네트워크 단절 상태의 Player 재시작과 복구 |
| 로그 | 7필드·HMAC/시각·오류 비노출·고정 대상·timeout·키 분리 | 승인된 합성 호출 1건과 실제 Cloud Logging 저장 |
| UI | 실제 Scene 참조 정적 대조·상태 전환·입력 잔류·중복 클릭·모달/해제·해상도별 레이아웃 검증 가능 부분 | 실제 Player의 가독성·겹침·Keyboard/Mouse 동작 |
| 복구 | 이전 reference·호환성·migration 금지 조합·자료 전후 비교기 | 기존 배포가 있는 경우에만 승인된 실제 복구 배포 |

시간 경계에 맞춘 빠른 클릭, 180일 대기/PC 시계 변경, 수백 번 수동 제출, 직접 저장 JSON 변조는 수동 확인 항목이 아니다.
Unit Test는 생산 코드/서버 대역·합성 저장을 사용하며 실제 서비스 계정·Production 데이터를 호출하지 않는다.

## 진행 순서

아래 번호로 한 항목씩 요청한다. 예: `Phase 5 Step 1 수행` 또는 `Step 7-1 수행 방법을 알려줘`.
체크는 실제 결과를 받은 뒤 갱신한다. 도구가 아직 없거나 승인 범위가 불명확하면 해당 작업의 착수 조건부터 해결한다.

| Step | 작업 | 담당 | 실제 서비스 변경 |
| --- | --- | --- | --- |
| 1 | 대상·QA 자료·중단 기준·실행 승인 확정 | 사용자 + AI | 없음 |
| 2 | 정적 검사·Unit Test·점검 도구·알려진 UI 문제 준비 | AI, 필요 시 사용자 Scene 작업 | 아직 없음 |
| 3 | Unity 컴파일·전체 EditMode/PlayMode 결과 확정 | 사용자 | 없음 |
| 4 | 승인된 Windows Player 빌드 | 사용자 | 없음 |
| 5 | Production 합성 로그 실제 연결 확인 | 사용자 | 합성 로그 1건 |
| 6 | 신규 Production QA 계정 준비 | 사용자 | 승인된 QA 계정/서버 자료 생성 |
| 7 | Stage/Infinite 실제 제출·순위 조회 | 사용자 | 승인된 QA 기록 제출 |
| 8 | Offline Pending·종료/재시작·복구 | 사용자 | QA Pending 재전송 |
| 9 | 기존 QA 계정 재실행·자료 보존 | 사용자 + AI 비교 | 상태 조회에 따른 정상 서버 갱신 가능 |
| 10 | QA A→B 계정 이전·이전 후 보존 | 사용자 + AI 비교 | 두 QA 계정의 연결 변경 |
| 11 | Player Keyboard/Mouse·화면 최종 확인 | 사용자 | 이미 승인된 확인 범위만 |
| 12 | 배포 복구와 자료 보존 검증 | 사용자 + AI | 별도 승인된 Module 재배포만 |
| 13 | 완료 근거·남은 위험·공개 여부 판정 | AI + 사용자 | 별도 공개 승인 전 없음 |

## Step 1. 검증 대상·실행 공간과 승인을 확정한다

### 1-1 AI: 기존 근거·로컬 대상 대조

- Phase 4의 배포 SHA·source 이후 변경·API 계약·Production 설정과 Build Profile을 읽기 전용으로 대조한다.
- 기존 설정·보관 기간·보드·검증 결과를 다시 만들거나 전부 재시험시키지 않는다. 변경된 항목만 영향 범위를 정한다.
- 대상이 바뀌었으면 사용자에게 해당 차이만 보고한다. 비밀값은 읽거나 출력하지 않는다.

### 1-2 사용자: QA A/B 실행 공간 지정

1. 일반 사용자 자료가 없는 전용 테스트 실행 공간 A를 지정한다.
2. 계정 이전에는 별도 인증 상태를 가진 실행 공간 B도 지정한다. B는 Step 10까지 실행을 미뤄도 된다.
3. 기존 사용자 Local Save/인증을 지워 새 계정을 만드는 대신 격리된 공간을 사용한다.
4. 사용할 환경이 production인지, 계정 별칭 A/B와 실행할 빌드가 무엇인지만 보고한다. 실제 번호·경로는 공유하지 않는다.

### 1-3 사용자: 실제 변경 범위 승인

다음 중 승인한 범위를 명시한다. Phase 4의 Module 배포 승인이 아래 실행을 자동 승인하지 않는다.

- Production 합성 로그 점검 1건과 필요한 최소 권한의 점검용 인증 사용.
- QA A/B 최대 두 실행 공간의 익명 계정·서버 자료 생성, 각 Step의 정상 플레이 제출/조회.
- Offline QA Pending 재전송과 QA A→B 이전.
- 기존 자료 보존, 실패 시 추가 제출/이전 중단, 운영자 1인 담당과 실행 시간대.
- 실제 Module 복구 배포는 Step 12에서 후보·변경 창을 별도로 승인한다.

QA 자료는 정리 승인이 있기 전까지 남겨 둔다. 확인 종료를 이유로 서버 자료나 계정을 자동 삭제하지 않는다.

완료 조건: [ ] 대상·A/B 격리 방법·승인된 쓰기 범위·담당/시간대·중단 기준 기록.

## Step 2. 수동 확인 전에 정적 검사·Unit Test·도구를 준비한다

### 2-1 AI: 기존 자동 검사를 대조하고 부족한 사례만 보강

- Node 검사와 로컬 LogSink 검사를 재사용한다. Phase 4 최종 기준은 Node 52파일 통과이며 이후 수치는 실제 실행 결과로 갱신한다.
- `OnlineRecordConfigurationTests`, `OnlineLocalSaveScopeTests`, `OnlineRecordRepositoryTests`,
  `OnlineAccountCoordinatorTests`, `AccountTransferControllerTests`, `AccountTransferCompletionTests`,
  `LeaderboardViewStateTests` 및 격리 PlayMode 테스트를 책임 표와 대조한다.
- 테스트 자료만 정상인 별도 로직을 만들지 않고 실제 coordinator/store/codec/transport·Scene 참조를 검증한다.
- 결과 미확인·저장 실패·취소·중복·동시 요청은 대역/가짜 시계로 판정한다. 실제 사용자에게 정밀한 버튼 타이밍을 요구하지 않는다.

### 2-2 AI: Production 점검 도구와 비공개 자료 비교 준비

- 현재 Production 전용 수동 LogSink 점검 도구는 이 문서 작성 시점에 준비되지 않았다.
  승인한 Project/Environment·허용 endpoint·인증 방식·합성 7필드를 확정하고 별도 도구를 준비한 뒤 로컬 대역으로 검사한다.
- verification 전용 도구는 고정 대상을 유지한다. Production 도구는 대상 불일치 거부, 명시적 실행 옵션,
  숨김 credential 입력·정리, 자동 재시도 없음, 오류 원문/Secret 비출력을 Unit Test로 확인한다.
- 계정/최고 기록·Pending 비교는 기존 도구의 적용 범위를 확인하고, 지원되지 않으면 Production 자료를
  기존 verification 도구에 넣지 않는다. 필요한 비교기를 AI가 준비해 로컬 비공개 기준 자료를 자동 비교한다.
- 공개 번호·점수·수락 시각·제출 ID·귀속 등은 원문을 채팅으로 받아 대조하지 않고 동일 여부·차이 분류만 출력한다.
- 도구가 없거나 안전한 인증/대상 대조가 안 되면 Step 5 및 해당 비교 Step은 **준비 대기**다.

### 2-3 AI + 사용자: 격리 Notice UI의 개선 또는 수용 결정

- Phase 4의 `IsolationArmed=True` Notice 반복을 Unit Test로 재현하고 일반 Play 흐름과 분리한다.
- AI는 수정안/회귀 검사를 준비하고 필요 시 사용자에게 개선 범위 승인을 받는다. Scene을 직접 수정하지 않는다.
- Scene 변경이 실제 필요한 경우에만 연결 대상·Inspector 항목·확인법을 작성하고 사용자가 적용한다.
  참조 여부는 AI 정적/자동 검사로 확인한다.
- 개선을 보류한다면 Editor 격리 도구에 한정된 제한이라는 근거와 사용자의 수용 결정을 기록한다.
  일반 Player에도 같은 문제가 나타나면 필수 실패로 분류한다.

완료 조건: [ ] 필요한 정적/Node 검사 통과·Unity 테스트 준비·Production 도구 준비·알려진 문제 처리 기준 기록.
이 Step은 준비이며 AI가 Unity Test Runner·원격 요청을 실행하지 않는다.

## Step 3. Unity 컴파일·전체 EditMode/PlayMode 결과를 확정한다

사용자 수행:

1. Step 2에서 승인된 Client/Scene 변경을 적용했다면 Unity 컴파일이 끝날 때까지 기다린다.
2. Console의 예상 밖 Error/Warning을 확인한다.
3. Test Runner에서 전체 EditMode와 전체 PlayMode를 각각 실행한다. 실서비스 호출이 없는 격리 테스트 구성을 사용한다.
4. 실행 개수·실패 수·예상 밖 오류/경고 여부를 보고한다. 실패 시 이름·안전한 메시지를 먼저 전달한다.

변경이 없고 Phase 4 이후 같은 source의 결과를 재사용할 수 있으면 AI가 그 근거를 연결하며,
단순 문서 변경·Git commit 때문에 다시 실행시키지 않는다. 기존 개수 미보고를 이유로 숫자를 추정하지 않는다.

완료 조건: [ ] 빌드 후보에 필요한 컴파일/전체 Unity 테스트 결과가 source와 연결됨.

## Step 4. Windows Player를 빌드하고 실행 정보를 기록한다

착수 조건: Step 1~3 완료. 코드가 바뀌었다면 새 SHA와 검증 결과를 먼저 고정하고 필요한 Module 배포 승인도 별도로 받는다.

### 4-1 AI 사전 검사

Production Build Profile의 `FLOW_STATE_PRODUCTION`, 승인된 Production 설정, Scene 목록/참조,
온라인 Local Save scope, 로그의 비밀값 비노출 및 test-only 경로 제외를 정적으로 확인한다.
식별자를 보여 주기 위한 Scene·UI를 사용자에게 새로 만들도록 요구하지 않는다.

### 4-2 사용자 실제 빌드

1. Phase 4에서 준비한 **Production 전용 Build Profile**을 선택한다.
2. 대상은 **Windows x64**, 화면 기준은 **1920×1080 Windowed**로 설정한다.
3. 기존 배포물을 덮어쓰지 않는 빈 전용 출력 폴더를 선택하고 **Build**를 실행한다.
4. 성공/실패, 사용한 profile·source SHA·빌드 시각을 기록한다.
5. Step 1 승인 전에는 Player를 실행하지 않는다. 초기화/자동 인증도 서버 자료를 만들 수 있다.

빌드 설정으로 해상도를 고정할 수 없는 경우 사용자에게 Player의 실제 지원 설정/창 크기 조정 방법만 안내한다.
지원하지 않는 command-line 옵션이나 아직 없는 진단 기능을 있다고 가정하지 않는다.

완료 조건: [ ] 사용자 Player Build 성공·1920×1080 Windowed 실행 후보·source/출력물 연결 기록.

## Step 5. Production 합성 로그의 실제 연결을 확인한다

착수 조건: Step 1의 합성 호출 승인 및 Step 2-2 전용 절차/도구 로컬 검사 완료.

사용자 수행:

1. Step 2에서 확정한 Production 전용 도구와 인증으로 합성 이벤트를 한 번 보낸다.
   실행할 실제 파일/명령은 도구 준비 후 이 절에 기록한다. 현재 임의 실행 명령은 없다.
2. 안전한 결과가 Accepted/HTTP 202인지 확인한다. 실패하면 분류만 보고하고 키를 임의 회전하지 않는다.
3. GCP Production 프로젝트의 Logs Explorer에서 실행 시각을 포함해 아래 조건으로 검색한다.

   ```text
   resource.type="cloud_run_revision"
   jsonPayload.environment="production"
   jsonPayload.reason="SyntheticProbe"
   ```

4. jsonPayload가 environment/moduleReference/outcome/reason/durationBucket/correlationId/rateLimitResult의
   정확히 7필드인지, production·승인 Module·Accepted·SyntheticProbe·Allowed인지 확인한다.
5. 자동 재시도하지 않는다. RateLimited면 표시된 허용 시각 이후 승인된 재실행 한 번만 수행한다.

Phase 4의 health 204나 verification Accepted는 이 Step의 성공 근거를 대신하지 않는다.
Production 일반 요청의 250 ms 로그 전송은 최선 시도이며 합성 Probe 성공이 무손실 수집 보장은 아니다.

완료 조건: [ ] Production 합성 호출·서명 수락 및 실제 7필드 저장 확인. 원문 이벤트는 보고하지 않음.

## Step 6. 신규 Production QA 계정을 준비한다

착수 조건: Step 1에서 지정한 실행 공간 A·계정 생성 승인, Step 4 Player 준비, Step 5 연결 점검 완료.

사용자 수행:

1. A에서 승인한 Production Player를 실행한다.
2. 계정 화면을 열어 복구 제한 안내에 동의한다. 공개 번호만으로 복구할 수 없다는 안내를 확인한다.
3. 준비 상태 Ready/오류 없음과 공개 번호 표시를 확인한다. 번호는 로컬 비공개 기준으로만 보관한다.
4. 필요하면 Refresh를 한 번 수행한다. timeout이면 추가 플레이/이전 전에 결과를 보고한다.
5. 새 계정에 수동 Cloud Save container·key·ledger를 만들지 않는다. 정상 호출로 준비되는지 확인한다.

Player가 진단 상태를 표시하지 않으면 존재하지 않는 버튼을 찾게 하지 않고, 실제 계정 화면과
Step 2 도구의 안전한 판정으로 확인한다. 준비 실패는 신규 계정 생성 반복으로 해결하지 않는다.

완료 조건: [ ] A의 정상 동의·인증·번호 준비와 안전한 결과 기록.

## Step 7. 실제 Stage·Infinite 기록을 제출하고 순위를 확인한다

### 7-1 Stage — 사용자

1. A에서 `stage-001`을 정상 플레이로 클리어한다.
2. 제출 완료를 확인한다. Pending/오류이면 새 Run으로 덮지 말고 상태를 보고한다.
3. Leaderboard에서 **Stage / TOP**, **Stage / AROUND YOU**를 각각 연다.
4. 내 행·공개 번호의 `(You)` 표시·낮은 시간이 좋은 순위임을 확인한다.
5. 화면의 Retry가 제공되는 경우 한 번 재조회하고 Back으로 복귀한다.

### 7-2 Infinite — 사용자

1. 정상 Infinite 플레이를 종료해 기록을 제출한다.
2. 제출 완료 후 **Infinite / TOP**, **Infinite / AROUND YOU**를 각각 연다.
3. 내 행·점수·높은 점수가 좋은 순위임을 확인하고 Back으로 복귀한다.

AI는 TOP 10/AROUND 7 요청·전역 순위·동점·대규모 보드를 Unit Test로 판정한다.
Production에 100명 이상의 QA 계정이나 128건 이상의 기록을 만들도록 요구하지 않는다.
빈 보드라도 내 최초 행이 생기면 충분하며 최대 행 수 채우기는 수동 조건이 아니다.

완료 조건: [ ] Stage 제출/두 목록, [ ] Infinite 제출/두 목록 정상. 점수 원문 대신 성공 여부만 보고.

## Step 8. Offline Pending의 보존·재시작·재전송을 확인한다

착수 조건: A의 온라인 계정 준비·기록 성공을 이미 확인했고 Pending 없는 기준 상태를 확보함.

사용자 수행:

1. 테스트 실행 공간의 네트워크 연결을 끊는다.
2. Stage를 한 번 정상 클리어해 제출 대기 기록을 만든다. UI의 Pending 또는 실패 안내를 확인한다.
3. 게임을 정상 종료한 뒤 네트워크를 끊은 상태로 다시 실행해 해당 대기가 보존되는지 확인한다.
4. 네트워크를 복구하고 실제 계정 화면의 Refresh Status 후 Retry Pending Uploads를 한 번 실행한다.
5. 제출 확정·해당 대기 항목 제거·Leaderboard 정상 조회를 확인한다.

원래 제출 ID·생성 시각·귀속의 보존과 재전송 멱등성·정확한 제거는 AI가 Unit Test와 준비한 로컬 비교기로 대조한다.
재연결 시 실제 구현에 의해 먼저 처리됐으면 남은 대기 없음 결과를 기록하며 시험을 맞추려고 추가 기록을 만들지 않는다.
timeout/응답 미확인이면 Pending을 남기고 결과를 보고한다. 수동 저장 파일 편집·강제 중복 제출은 필요 없다.

완료 조건: [ ] Offline 생성·재시작 보존·온라인 복구의 실제 Player 경로와 자동 비교 결과 연결.

## Step 9. 기존 Production QA 계정의 재실행·자료 보존을 확인한다

기존 계정은 Step 6~8에서 만든 **같은 Production A**다. verification 계정을 Production으로 복사하는 뜻이 아니다.

사용자 수행:

1. Step 2의 비공개 비교 절차로 A의 공개 번호·두 Mode 최고 기록·안전하게 비교할 수 있는 서버 수락 정보와 Pending 기준을 보관한다.
2. Player를 정상 종료하고 같은 실행 공간/빌드로 재실행한다.
3. 계정 화면과 두 Leaderboard를 열고 준비한 도구로 기준 자료와 비교한다. 이 사이 새 기록 제출은 하지 않는다.
4. Settings·Input Binding·Tutorial의 보존은 자동 테스트/codec 대조를 우선하고 화면에서는 실제 적용 여부만 확인한다.

Local Save 구버전 migration은 복제 자료·가짜 시계 Unit Test로 확인한다. 실제 legacy 자료가 없는 경우
서비스 migration 전후 비교는 N/A로 기록하며 존재하지 않는 오래된 계정을 만들거나 운영 저장을 변조하지 않는다.

완료 조건: [ ] 동일 Production QA 계정 재연결·기록/Pending 보존. 실제 legacy 자료 여부와 N/A 근거 구분.

## Step 10. QA A→B 이전과 기록 보존을 확인한다

착수 조건: Step 1의 A/B 생성·연결 변경 명시 승인, 두 실행 공간의 인증 격리, 비공개 기준 비교 도구 준비.

### 10-1 사용자: B 준비와 A 기준 보관

- B에서 같은 승인 Player로 계정 준비·동의를 완료한다. B의 기존 자료가 있으면 승인된 QA 자료인지 확인하고 별도 기준을 보관한다.
- A의 Step 9 기준을 유지한다. 실제 사용자 B나 verification 프로필은 이 작업에 사용하지 않는다.

### 10-2 사용자: 정상 이전 한 번 수행

1. A 계정 화면에서 실제 제공되는 이전 코드 발급 동작을 실행한다.
2. 코드와 검증값은 B에서만 비공개로 입력한다. 채팅·스크린샷·결과 로그에 붙이지 않는다.
3. B에서 이전 완료를 실행하고 완료 결과를 기다린다. 응답 미확인이면 Refresh로 명시 확인하며 새 이전을 시작하지 않는다.
4. B에서 A의 번호·두 Mode 기록/내 행이 유지됐는지 로컬 비교기로 확인한다. 기존 B 기록과 합쳐지지 않는지 판정한다.
5. A에서 원래 계정에 대한 활성 권한이 끝났다는 안내를 확인한다. 재시작해 B에서도 이전 결과가 보존되는지 확인한다.

취소/만료·실패 누적·정밀 경합·응답 유실의 모든 분기를 운영 계정에서 수동 재현하지 않는다. 해당 정확성은 Unit Test로 판정한다.
이전 성공 뒤 후속 새 기록 제출은 승인 범위와 기준 비교 완료 여부를 확인한 후 필요한 경우에만 수행한다.

완료 조건: [ ] A→B 실제 이전·기록 귀속/보존·A 비활성 안내·B 재시작 결과와 자동 비교 근거 연결.

## Step 11. Player 화면과 Keyboard/Mouse 조작을 확인한다

Step 6~10에서 이미 연 화면을 재사용해 불필요한 제출·이전을 추가하지 않는다.

사용자 확인:

- [ ] 1920×1080 Windowed에서 Main Menu·Result·Settings·계정/이전 화면·Leaderboard 글자/버튼이 읽힘.
- [ ] Keyboard 선택·Submit/Cancel·Back과 Mouse 클릭이 정상 동작하고 포커스·모달 닫기가 자연스러움.
- [ ] Stage/Infinite·TOP/AROUND YOU 전환과 Loading/정상/Empty/Error/Pending 안내가 구분됨.
- [ ] Retry·Pending Retry·Back이 실제 표시 행과 겹치지 않음. 최대 표시 행의 재현은 자동 테스트 또는 로컬 대역을 우선함.
- [ ] 일반 Player에서 Continue 후 Notice가 반복되지 않음. 격리 Editor UI 제한은 Step 2-3 결정과 별도 구분함.

임의 장애를 Production에 유발해 모든 오류 화면을 수동으로 만들지 않는다. 자동/로컬 대역으로 이미 검증한 화면은 그 근거를 연결한다.
Scene 문제가 발견되면 AI는 방법·자동 검사만 준비하고 사용자가 Scene을 수정한다. 수정 후 필요한 컴파일/Test/재빌드 범위를 다시 정한다.

완료 조건: [ ] 실제 Player 가독성·조작 수동 결과 및 자동 검증 근거 연결. 필수 UI 실패 없음.

## Step 12. 배포 복구와 자료 보존을 검증한다

### 12-1 AI: 복구 후보와 호환성 대조

- 최초 Production 배포 전 원격 Module이 없었음을 유지한다. 최초 배포 실패는 수정 후 재배포이지 이전 버전 rollback이 아니다.
- 실제 다음 변경이 있다면 직전 성공 배포의 source/reference·Local Save/Cloud Save writer·migration marker·Secret 계약을 대조한다.
- 승인된 QA 자료 기준·복구 후 비교 항목을 준비한다. source만 같다는 이유로 rollback 동작을 실행 검증했다고 기록하지 않는다.

### 12-2 사용자: 실제 복구 배포가 가능한 경우 별도 승인 후 수행

1. 현재/이전 reference·중단 기준·검증 자료·변경 창을 승인한다.
2. 같은 Production 환경에 승인된 Module 하나만 복구 배포한다.
3. QA 계정/기록/Pending의 비공개 기준과 복구 후 상태를 비교하고 손실·병합·삭제가 없는지 확인한다.
4. 복구 뒤 어떤 버전을 유지할지 결정하고 배포 결과를 기록한다. 데이터 삭제나 환경 간 덮어쓰기는 하지 않는다.

실제 이전 성공 배포가 없거나 시험 변경을 승인하지 않은 경우 **실제 복구 후 보존은 미검증**으로 남긴다.
이 시험만을 위해 의미 없는 코드 변경·Production 장애·가짜 이전 버전을 만들지 않는다.
Phase 4에서 제외한 rollback 시험을 재도입하는 것이 아니라, 로드맵 Phase 5의 실제 복구/보존 요구를 별도로 다루는 단계다.
현재 후보에 적용할 수 없어 완료 기준을 면제/연기하려면 사용자의 명시적인 기준 변경 승인과 로드맵 갱신이 필요하다.
미실시를 성공으로 표시하거나 문서 작성만으로 Phase 5 전체를 완료하지 않는다.

완료 조건: [ ] 실제 복구·보존 근거 확보, 또는 [ ] 명시적 완료 기준 변경 승인·미검증 위험/후속 조건 기록.

## Step 13. Phase 5 완료 여부와 공개 운영 후보를 판정한다

AI가 아래 근거를 source·빌드·대상별로 대조하고 사용자가 최종 검토한다.

- [ ] Step 1 승인과 Step 2의 자동 검증/안전 도구가 실제 수행 결과와 연결됨.
- [ ] 최신 후보의 Unity 컴파일·전체 테스트·Windows Player Build 결과가 연결됨.
- [ ] 실제 Production 합성 로그 수집·신규/기존 QA 계정·두 Mode 제출/조회·Offline·이전/보존 확인.
- [ ] 실제 Player UI/Keyboard/Mouse와 격리 Notice 문제의 처리/수용 결과 기록.
- [ ] 복구 후 데이터 보존의 실제 근거 또는 명시 승인된 기준 변경 및 잔여 위험 기록.
- [ ] 예상 밖 필수 실패가 없고 timeout/응답 미확인·로그 전송 한계·원격 artifact 동일성 한계를 구분함.
- [ ] 점검 credential을 새로 발급했다면 해당 credential·추가 임시 권한만 회수하고 결과 보고.
  런타임 LogSink 서비스 계정·필수 Secret 권한은 유지하며 QA 자료 삭제는 별도 승인 대상으로 남김.

필수 근거가 없으면 미완료 사유와 다음 작업을 기록한다. verification 결과를 Production 성공으로 대체하지 않는다.
모두 충족하면 Phase 5 완료/공개 운영 **후보**를 판정한다. Steam 배포·일반 사용자 공개 개시는 별도 사용자 승인이다.

## 결과 보고 원칙과 기록표

| Step | 결과 | 기록할 내용 |
| --- | --- | --- |
| 1~2 | 미착수 | 승인 범위·도구 준비 여부·정적/Unit Test 결과 |
| 3~4 | 미착수 | source/Module/Player 기준, 테스트 개수·오류 여부, 빌드 성공 여부 |
| 5 | 미착수 | Production Accepted/202·7필드 저장 확인 여부 |
| 6~10 | 미착수 | A/B 별칭, 정상/실패 분류, 동일성·보존 비교 결과만 |
| 11~12 | 미착수 | UI 실패 분류·복구 reference/승인·비교 성공 여부 또는 미검증/변경 승인 |
| 13 | 미착수 | 완료/차단 사유·잔여 위험·공개 승인 여부 |

이 문서 작성 시점의 완료 항목은 계획 수립뿐이다. 각 Step의 승인·수동 실행·원격 성공은 아직 없다.
실행 파일/도구 경로와 버튼이 확정되지 않은 항목은 담당 Step 수행 때 코드와 대조해 구체화한다.

## 영향 범위·검증 결과·후속 작업

- 영향 범위: Tasks·Roadmap·프로젝트 진행 기록. 이번 요청에서는 런타임/Scene/정책/도구 구현을 변경하지 않는다.
- 문서 검증: Phase 4 인계·로드맵·현행 source를 대조했다. 13개 Step 연속 번호·실행 미착수 상태·승인 경계·
  참조한 기존 EditMode 테스트 파일 존재·관련 로컬 문서 링크·식별자 비기록·문서 공백/줄바꿈 정적 검사를 통과했다.
- 도구/새 Unit Test 구현과 실제 실행은 각 Step의 후속 작업이다. 계획만으로 테스트 통과를 주장하지 않는다.
- 다음 작업: **Step 1의 대상·실행 공간·승인 범위 확정**.

## 관련 문서

- [Prototype 8 Roadmap](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md)
- [Phase 4 완료·인계](20261007_09_Phase4ManualSteps.md)
- [Phase 3 실제 검증 이력](20261006_08_Phase3ManualSteps.md)
- [Production Inventory](../../../UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md)
- [환경 중립 Module 구성](../../../UGS/ENVIRONMENT_NEUTRAL_MODULE_CONFIGURATION.md)
- [Safe LogSink 계약](../../../UGS/SAFE_LOG_SINK_CONTRACT.md)
- [계정 이전 운영 안내](../../../UGS/TRANSFER_OPERATIONS_RUNBOOK.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
