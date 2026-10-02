# 작업 정보

## 작업명

Prototype 8 Phase 2 — 공개 번호·논리 계정·기기 이전 구현 및 수동 적용 절차

## 작업 일자

20261003

## 작업 담당자

AI: 구현·정적 검증·Unit Test 작성·Node 실행·정확한 수동 적용 지침 작성.
사용자: Unity 컴파일·Test Runner 실행, Scene/Inspector 적용, verification 서비스 적용 및 화면 확인.

## 작업 상태

계획 작성 및 Step 분할 검토 완료. 아래 12개 상위 Step과 21개 하위 Step의 구현·적용·실행은 모두 대기다. Phase 1은 완료이며, 이 문서 작성으로 Phase 2 구현 또는 검증이 완료되는 것은 아니다.

# 작업 목적

Roadmap 008 Phase 2를 실제로 수행할 순서로 나누고, 코드로 검증할 항목과 사람이 직접 적용·확인할 항목을 구분한다. Phase 1의 확정 정책을 다시 선택하지 않고 실제 저장·서비스·Client·UI에 연결한다.

# 작업 대상

- 공개 번호 발급·조회·기존 계정 전환, 논리 계정 C와 단일 활성 연결, 고정 Leaderboard 소유 행.
- 서버 발급 이전 코드·인증값, 잠금·재발급·취소·만료·B 연결, 제출 예약과 연결 교체 경합.
- 계정 이전 전후 Local Save·개인 최고·인증 상태, 공개 번호·오류·이전 UI.
- verification에서의 제한된 서비스 적용과 실제 서비스·UI 확인.

Phase 3의 180일 receipt 분할·정리, 128건/100명 제한 해소, Phase 4의 Production 구성·배포와 30일 로그 sink, Phase 5의 최종 Windows Player Build·운영 후보 판정은 후속 작업이다. Phase 2에서는 기존 제한을 숨기거나 해소했다고 기록하지 않는다.

# 작업 전 상태

- Phase 1 Task Step 1~8 완료. 공개 번호는 인증 직후 서버 발급하는 10자리 십진 문자열이다.
- 이전 코드는 Crockford Base32 8자리, 인증값은 십진 9자리, 만료는 90일, 반복 검증 간격은 5초다. 활성 코드 공간은 환경별 천만 개 동시 활성 목표를 가진다. 정책의 코드 공간 계산과 실제 서비스 용량 입증은 구분한다.
- B의 기존 온라인 기록은 이전을 막지 않는다. B의 Local Save Pending은 이전을 막으며 전송 완료 또는 명시적 폐기 후 다시 시도하도록 안내한다.
- 이전은 C의 활성 연결만 교체한다. 고정 Leaderboard 행·공개 번호·최고 기록·수락 시각은 유지한다. B의 로컬 개인 최고는 C의 서버 개인 최고로 덮어쓰며, C 기록이 없으면 비운다.
- 현재 서비스 코드와 Client는 UGS Player ID 귀속 경로를 사용하며 C 해석·공개 번호·이전 서비스는 미구현이다. Phase 1 정책 Test 통과는 이 통합의 검증 근거가 아니다.

# 조사 내용

- 설계 근거는 Phase 1 Task의 2-1~2-10과 Step 7 대응표·Step 8 인계다.
- 기존 서버 파일: `UGS/CloudCode/submit-record.js`, `UGS/CloudCode/query-records.js`.
- 기존 Client 파일은 `Assets/Scripts/Runtime/Features/`의 `OnlineAccountState.cs`, `UgsOnlineAuthenticationGateway.cs`, `CloudCodeRecordRepository.cs`, `OnlineRecordCoordinator.cs`, `LocalSaveJsonCodec.cs` 등에 있다.
- 기존 Scene은 `Assets/Scenes/SampleScene.unity`다. `Canvas`, `OnlineRecoveryNoticePanel`, Result 제출 상태·Retry UI가 존재한다. 새로운 이전 UI의 위치·Component·Inspector 필드는 구현 후 Step 8에서 정확히 지정한다.
- 기존 검증 Test는 `Assets/Tests/EditMode/OnlineRecordRepositoryTests.cs`, `LocalSaveJsonCodecTests.cs`, `LeaderboardViewStateTests.cs`, `RecordLeaderboardPolicyTests.cs` 및 `Assets/Tests/PlayMode/PlayModeRecordIsolationTests.cs` 등에 있다.
- 원격 적용 기준은 `UGS/VERIFICATION_DEPLOYMENT.md`와 `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`다. Prototype 7의 제한된 재게시 안내를 Prototype 8의 신규 배포 목록으로 그대로 사용하지 않는다.

# 작업 내용

## 수행 원칙과 순서

각 Step은 AI 구현·정적 검증·Unit Test 준비 후, 필요한 사용자 적용·실행을 수행한다. 사용자 작업이 없는 Step은 AI가 검증 후 완료 처리한다. 사용자 작업이 있는 Step은 실제 결과를 받은 뒤 완료 처리한다.

Step 1~12는 전체 흐름이며, 큰 작업은 `2-1`, `2-2`처럼 독립적인 하위 Step으로 수행한다. 각 하위 Step의 산출물·검증·완료 조건을 기록하고 모두 완료된 경우에만 상위 Step을 완료한다. 사용자 수동 작업과 공통 조건은 상위 Step의 안내를 따른다. 하위 Step 완료 시 Node 실행 결과와 C# Test 작성·정적 확인 결과를 구분하며, C# 실행 검증은 Step 7에서 확인한다.

### Step 분할 검토 결과

- 서버 저장 기반·번호 발급·기존 자료 전환, 이전 발급·재발급·취소/만료, 제출·조회, Client 저장·인증·UI는 각각 별도 하위 Step으로 나눈다. 서로 다른 실패 복구와 Test를 독립적으로 검토할 수 있어야 한다.
- verification 적용은 백업·설정·게시, 실제 서비스 확인은 신규 계정·기존 계정·이전으로 나눈다. 한 부분의 실패로 다른 부분의 완료 근거가 섞이지 않게 한다.
- verification 실행 도구는 Step 6에서 준비하고 Step 7에서 컴파일·격리 Test를 확인한다. Step 10에서는 준비된 도구를 사용한다.
- Step 7의 Play Mode Test는 Scene 미연결 상태에서도 실행 가능한 격리 구성만 검사한다. 실제 Scene 참조 Test는 Step 8에서 사용자 연결 후 실행한다.
- Phase 3~5 작업을 새 Step으로 추가하지 않는다. 현재 계획은 12개 상위 Step과 21개 하위 Step으로 구분한다.

- AI는 Unity Editor Build와 Unity Test Runner를 실행하지 않는다. Scene·Prefab을 직접 수정하지 않는다.
- 정적 확인으로 알 수 있는 경로·참조·직렬화·입력 정의·환경 상수·권한 정책을 사용자에게 재확인시키지 않는다. 원격 설정과 실행 결과만 수동으로 확인한다.
- Unit Test는 생산 코드와 메모리 저장소·서비스 대역·가짜 시계를 사용한다. Test 내부에 구현을 복제하지 않는다. 실제 계정·사용자 Local Save·원격 서비스와 분리한다.
- 동시 발급, 두 B 경합, 제출과 이전 충돌, 응답 유실, 90일·5초 경계는 자동 검증한다. 빠른 클릭이나 90일 대기를 요구하지 않는다.
- 원격 수동 작업은 verification에 한정한다. 기존 번호·계정·행·ledger를 삭제하거나 초기화하지 않는다. 자격 증명·Secret 값·토큰은 채팅·저장소·로그로 전달하지 않는다.
- 신규 서버 Script/키/Component/Test 이름은 해당 Step 구현 후 실제 파일과 일치하는 적용표로 제공한다. 아래 예정 Test 책임은 아직 존재하는 fixture 이름을 의미하지 않는다.
- 코드 변경 뒤에는 Step 7을 수행한다. 이후 Step에서 코드가 바뀌면 영향받는 컴파일·Test를 다시 확인한다.

| Step | 작업 | 사용자가 직접 할 일 |
| --- | --- | --- |
| 1 | 기존 코드·SDK·저장 원자성 구현 계획 | 기본 없음 |
| 2 (2-1~2-3) | 서버 저장 기반·공개 번호·기존 자료 전환 | 없음 |
| 3 (3-1~3-3) | 이전 발급·재발급·취소·만료 | 없음 |
| 4 (4-1~4-2) | B 연결·경합·부분 실패 복구 | 없음 |
| 5 (5-1~5-2) | 제출·조회 경로 C 귀속 전환 | 없음 |
| 6 (6-1~6-5) | Local Save·인증·완료 상태·UI·검증 도구 | 없음 |
| 7 | Unity 컴파일·자동 Test 실행 | Editor Console, Edit/Play Mode Test Runner |
| 8 | Scene/Inspector 적용 | UI 생성·Component 참조·Navigation 연결 |
| 9 (9-1~9-3) | verification 서비스 적용 | 대상 확인·백업·Secret 권한·변경 목록 게시 |
| 10 (10-1~10-3) | verification 실제 서비스 검증 | 격리된 인증 세션으로 실제 검증 도구 실행 |
| 11 | 두 기기 UI 흐름 확인 | 발급·Pending 안내·이전·A 새 시작·Offline 화면 |
| 12 | Phase 2 완료 판정·후속 인계 | 미보고 실행 결과 제출만 |

## Step 1. 기존 구현과 서버 저장 원자성 계획을 확정한다

### AI 수행

1. Phase 1 계약과 Client·서버·SDK·기존 Test를 대조하고 변경 파일·API·저장 단위·호환 순서를 정리한다.
2. Cloud Save 최초 생성, write lock, 다중 저장 단위의 부분 실패, 고정 소유 ID의 Leaderboards 쓰기, Script 의존성 배포 방법을 현재 SDK 및 공식 문서로 확인한다. 동일 항목 CAS를 다중 항목 transaction으로 간주하지 않는다.
3. Account를 권한·terminal 판정의 기준으로 삼고 PlayerBinding·번호 매핑·TransferLookup 불일치의 복구 순서와 fail-closed 조건을 구체화한다.
4. 최초 번호 예약·중단 뒤 재개·번호 비재사용, 환경별 코드 유일 예약, 제출 예약 복구의 Test 사례를 정한다. UGS에서 입증하지 못한 제약은 미확인으로 기록한다.

### 사용자 수동 작업

기본 없음. 공식 문서·로컬 자료로 확인할 수 없는 원격 설정이 구현을 막을 때만 AI가 대상 Project/Environment, 확인할 설정 화면과 필요한 값 목록을 제공한다. 사용자는 해당 값을 읽어 요약한다. 이 단계에서는 게시·Secret 생성·자료 수정하지 않는다.

### 완료 조건

- [ ] 변경 대상·물리 저장/API·실패 복구·자동 검증·원격 검증 지점을 정했다.
- [ ] 기존 자료 보존과 cutover/호환 롤백 조건, 미확인 서비스 제약을 기록했다.

## Step 2. 논리 계정·공개 번호·고정 소유 행과 기존 자료 전환을 구현한다

### Step 2-1. 서버 저장 기반과 권한·작업 예약을 구현한다

Step 1의 물리 저장 계획에 따라 Account·PlayerBinding·고정 소유 ID 저장소, 활성 연결/revision 대조, 제출 작업 예약과 이전 시작 차단을 구현한다. 저장소·시계·ID 생성기를 주입하고 환경 불일치·부분 매핑·예약 복구를 Unit Test로 확인한다. 이후 Step 3의 잠금은 이 기반을 사용한다.

- [ ] 저장 기반·권한·예약의 Node Unit Test와 정적 검증이 통과했다.

### Step 2-2. 공개 번호 발급·재조회·유일 예약을 구현한다

2-1의 저장소로 인증 계정의 신규 번호 발급·기존 번호 반환·중단 후 재개를 구현한다. 번호 유일 예약·비재사용·문자열 정밀도·동시 발급을 자동 검증한다.

- [ ] 발급·재조회·충돌·부분 실패 Test가 통과했다.

### Step 2-3. 기존 계정·고정 행·ledger의 전환을 구현한다

2-1~2-2 완료 후 기존 자료의 승계·재개 가능한 cutover·활성화 전 대조·호환 복구를 구현한다. 기존 행·최고 기록·수락 시각·Pending 귀속 보존과 전환 재실행을 검증한다.

- [ ] 기존 자료 전환·보존·부분 실패 복구 Test가 통과했다.

### AI 수행

인증 context로 C를 찾는 저장소·발급 서비스, 번호 유일 예약과 기존 번호 반환, 기존 UGS 행/ledger를 승계하는 재개 가능한 cutover를 구현한다. 공개 번호는 문자열로 직렬화한다. 새로운 계정의 고정 소유 ID는 서버 생성 UUID v4를 사용한다. 전환 활성화 전에 매핑·원래 기록·Pending 귀속을 대조한다.

Unit Test: 같은 계정 재요청·재시작, 다른 계정 동시 발급, 각 저장 단계 실패·응답 유실·재개, 선행 0·최대값, 번호 미재사용, 기존 기록 metadata 보존, 기존 행의 지연 번호 발급, Client 임의 소유 ID 거부. 기존 정책 단위를 재사용하고 실제 서비스 계층을 저장소 대역으로 검사한다.

### 사용자 수동 작업

없음. 번호·매핑·ledger를 Dashboard에서 수동으로 만들어 구현을 대신하지 않는다.

### 완료 조건

- [ ] 서버 저장·발급·cutover 구현과 해당 Node Unit Test가 통과했다.
- [ ] 실제 서비스의 최초 생성·유일성 보장은 Step 10 검증 대상으로 남겼다.

## Step 3. 이전 시작·자격 증명·재발급·취소·만료를 구현한다

### Step 3-1. 이전 시작·자격 증명 생성·잠금을 구현한다

2-1의 권한·예약을 사용해 `TransferPending` 전이와 CSPRNG·digest/HMAC·코드 유일 예약을 구현한다. 생성 충돌·Secret 누락·저장 중단·응답 유실에서 재개 또는 안전한 실패를 검증한다.

- [ ] 시작·잠금·원문 비보관·생성 충돌 Test가 통과했다.

### Step 3-2. 재발급과 인증 시도 제한을 구현한다

동일 이전 요청의 재발급·기존 코드 즉시 무효화·5초 검증 제한을 구현한다. 재발급과 검증 경합·실패 횟수 무제한·선행 0·입력 정규화를 검사한다.

- [ ] 재발급·즉시 무효화·검증 제한 Test가 통과했다.

### Step 3-3. 취소·만료·상태 재조회를 구현한다

90일 서버 시각 기준 만료, A 취소, 잠금 해제·자격 증명 비활성화, 재시작 뒤 상태 조회를 구현한다. 90일 경계와 취소/만료 경합·부분 실패를 검증한다. B 완료와의 경합은 4-2에서 합쳐 검사한다.

- [ ] 취소·만료·재조회·복구 Test가 통과했다.

### AI 수행

활성 A와 제출 예약/Pending 조건, `TransferPending` 잠금, CSPRNG 코드·인증값 생성, 환경별 활성 코드 유일 예약·충돌 재생성, digest/HMAC·Secret 주입, 서버 시간·5초 제한을 구현한다. 재발급은 기존 값을 즉시 무효화하며 취소·만료는 A 연결을 유지·복원한다. 원문을 저장·로그에 남기지 않고 발급 응답에서만 보여준다.

Unit Test: 코드 정규화·선행 0, 생성 충돌 재시도·실패, 재발급/취소/만료의 저장 중단·재개, 90일·5초 직전/동일/직후, 인증 실패 누적으로 영구 차단하지 않음, 이전 화면 종료·앱 재시작 후 원문 복원 없음. 천만 개 목표는 공간·충돌 처리 검증과 서비스 제약을 별도로 기록하며 천만 계정을 실제 생성하지 않는다.

### 사용자 수동 작업

없음. Secret 설정은 Step 9에서만 사용자에게 정확한 이름·범위·권한을 제공한다.

### 완료 조건

- [ ] 시간·잠금·자격 증명·원문 비보관의 생산 서비스 Unit Test가 통과했다.
- [ ] Secret 누락·저장 실패에서 안전하게 실패하고 연결을 잘못 바꾸지 않는다.

## Step 4. B 연결과 단일 활성 연결 검증을 구현한다

### Step 4-1. B 연결 완료와 기존 B 계정의 분리를 구현한다

Step 2~3의 기반으로 자격 증명 검증·C 연결·코드 소진·B의 기존 계정 연결 해제를 구현한다. B의 기존 온라인 기록·번호·행을 보존하고 동일 B 완료 재호출의 결과를 고정한다.

- [ ] B 연결·원래 계정 자료 보존·동일 B 재호출 Test가 통과했다.

### Step 4-2. 연결 교체 경합과 부분 실패 복구를 검증한다

두 B, 완료/취소/만료, 제출 예약/이전 시작의 경합을 실제 서비스 코드와 제어 가능한 저장소 대역으로 검증한다. 각 쓰기 실패·응답 유실·재시작·A 지연 요청을 주입해 단일 winner와 매핑 복구를 확인한다.

- [ ] 경합·중단·재개 Test가 통과했고 미확인 원격 보장을 Step 10에 연결했다.

### AI 수행

B의 Anonymous 인증과 코드·인증값 검증, terminal winner, 코드 소진, Account와 역방향 매핑의 복구를 구현한다. B의 기존 온라인 자료는 원래 계정에 남기고 C와 합치지 않는다. 동일 B의 완료 재호출은 같은 결과를 반환한다. 활성 연결·revision·제출 예약·조회 snapshot 재검증을 기록 경계에 연결한다.

Unit Test: 두 B 완료 경합, 완료/취소/만료 경합, A 지연 요청, 제출 예약과 이전 시작의 양방향 차단, 각 CAS 실패·응답 유실·재시작, 같은 B 재호출, 다른 B 거부, 다른 환경 코드, B 기존 온라인 자료 보존. Client Pending 판정은 Step 6의 저장소 대역 Test로 연결한다.

### 사용자 수동 작업

없음. 빠른 동시 조작으로 단일 활성 연결을 판정하지 않는다.

### 완료 조건

- [ ] 경합·복구·권한 검증의 Node Unit Test가 통과했다.
- [ ] 연결 교체 중 부분 매핑을 권한으로 사용하는 경로가 없다.

## Step 5. 실제 제출·조회 경로를 C와 고정 행에 연결한다

### Step 5-1. 제출·최고 기록 갱신을 고정 행에 연결한다

`submit-record`를 C·활성 연결·제출 예약에 연결한다. B 후속 제출이 동일 행을 사용하고 A 지연 제출·이전 중 제출을 거부하는지, 점수와 수락 시각 보존·응답 유실 복구가 유지되는지 검증한다.

- [ ] 생산 제출 경로의 변경·회귀 Node Test가 통과했다.

### Step 5-2. 조회·공개 번호·본인 행 판정을 연결한다

`query-records`에 활성 C·조회 후 revision 재검증·기존 행 번호 조회/발급·응답 축소를 연결한다. 현재 100명 한도 내에서 고정 행·공개 번호·본인 판정과 오류 비노출을 검증한다.

- [ ] 조회·행 표시 계약·stale 응답·번호 발급 실패 Test가 통과했다.

### AI 수행

`submit-record`·`query-records`가 인증 호출자의 활성 C를 해석하고 고정 행을 사용하도록 전환한다. 이전 중 제출·조회는 차단한다. 기존 최고 기록·수락 시각을 유지하고 행의 공개 번호와 `(You)` 판정을 반환한다. 응답은 UI에 필요한 정보로 제한한다. 기존 입력 검증·직접 Write 차단·Pending 재시도 계약을 유지한다.

Unit Test: 이전 전후 동일 행·번호·metadata, B 후속 제출도 같은 행, A 거부, 조회된 기존 행의 번호 발급 실패, 서로 다른 계정 혼합 방지, Error에서 내부 ID 비노출. 기존 Cloud Code Test 21개를 실제 변경 계약에 맞게 확장·갱신하고 정적 계약 검사도 변경 파일과 함께 갱신한다.

### 사용자 수동 작업

없음. 실제 서비스 호출은 Step 10에서 수행한다.

### 완료 조건

- [ ] 생산 제출·조회 경로의 변경·회귀 Node Test가 통과했다.
- [ ] 128/100 및 receipt 정리는 Phase 3로 인계하며 기존 제한 상태를 기록했다.

## Step 6. Client·Local Save·인증·표시·이전 UI 코드를 구현한다

### Step 6-1. Local Save migration과 Pending gate를 구현한다

환경별 온라인 저장 영역·기존 자료 migration·Pending 귀속·명시적 폐기 API를 구현한다. 저장 실패 시 차단, 다른 환경 자료 재귀속 금지, A/B Pending 존재 시 요청 0회를 생산 코드 Edit Mode Test로 작성한다.

- [ ] Codec·저장소·Pending gate 코드와 Test를 작성하고 정적으로 대조했다.

### Step 6-2. 인증·서버 요청·공개 번호 모델을 구현한다

인증 직후 발급/조회, 이전 transport·서버 상태 조회, 번호 문자열·오류·Retry 모델을 구현한다. Offline Run 허용·계정 불일치·stale 응답·안전한 응답만 표시하는 Edit Mode Test를 준비한다.

- [ ] 인증·transport·표시 모델 코드와 Test를 작성하고 정적으로 대조했다.

### Step 6-3. 이전 완료 뒤 A/B 상태와 개인 최고를 반영한다

서버 완료 확인 뒤 A 인증 세션·C 캐시 초기화 및 새 Anonymous 시작, B의 C 재조회·개인 최고 전체 교체/비움을 구현한다. 확인 전 Timeout의 상태 보존과 재시작 복구도 검사한다.

- [ ] A/B 완료 처리·개인 최고·새 시작·Timeout 복구의 코드와 Test를 준비했다.

### Step 6-4. 이전 화면 Controller/View와 격리 UI Test를 준비한다

시작·복구 입력·재발급·취소·결과 화면의 코드, 단일 이벤트 등록·화면 닫힘·원문 수명·중복 클릭 방지를 구현한다. Scene 없이 생성 가능한 UI 구성으로 Edit/Play Mode Test를 준비하고 Step 8 적용표에 사용할 실제 필드를 확정한다.

- [ ] UI 코드·격리 Test·Scene 적용표에 필요한 Component/필드를 준비했다.

### Step 6-5. verification 검증 도구와 격리 세션을 준비한다

Step 10의 실서비스 실행 도구를 준비하고 기기 A/B의 테스트 세션·인증 프로필·Local Save 경로·테스트 Board·결과 비교 항목을 지정한다. 자동 Test에서는 원격 호출을 끄고, 명시적인 사용자 실행에서만 verification에 요청한다. 기존 도구 확장 여부·실제 메뉴·버튼·기대 결과를 문서화한다.

- [ ] 도구·세션 분리·원격 호출 차단 Test와 실행 지침을 준비했다.

### AI 수행

1. 인증 직후 번호 발급/조회, 오류·Retry·Offline 허용, C 기준 본인 행 표시와 연결 상태 모델을 구현한다.
2. 온라인 자료를 `(Project ID, Environment ID)`로 구분하는 Local Save migration, Pending 귀속 유지, Pending 폐기 저장 성공 뒤에만 이전 허용을 구현한다.
3. 이전 시작/입력/재발급/취소/결과 UI의 Controller/View를 구현한다. 코드·인증값은 Local Save에 저장하지 않는다.
4. B 완료 확인 뒤 서버 개인 최고로 로컬 최고를 완전히 덮어쓰고 기록이 없으면 비운다. A 완료 확인 뒤 C 캐시·로컬 최고와 이전 인증 세션을 지우고 새 Anonymous로 시작한다. Settings·튜토리얼·입력 설정은 보존한다.
5. 서버 확정 여부가 불명확한 Timeout에서는 임의 연결 전환·기록 초기화를 하지 않고 서버 상태 재조회로 복구한다.

Unit Test: Codec migration·다른 환경 자료 차단, Pending 있는 A/B의 요청 0회, 폐기 저장 실패, B 기존 온라인 기록 허용, 개인 최고 교체/비움, A 새 인증·캐시 제거, 이전 원문 미저장, stale 응답 무시, Offline Run 허용. 실제 Controller·저장소·인증 gateway를 대역으로 검사하는 Edit Mode Test를 작성한다.

### 사용자 수동 작업

없음. Scene 수정 없이 코드·Test를 먼저 준비한다.

### 완료 조건

- [ ] Client·직렬화·UI 코드와 Edit Mode Test를 작성하고 정적으로 대조했다.
- [ ] 실행 결과는 Step 7에서 사용자 Test Runner로 확인한다.

## Step 7. Unity 컴파일과 Edit/Play Mode Test를 실행한다

### AI 선행 검증

모든 변경 C#·asmdef·직렬화 경로·정책 참조를 검사하고 Node Unit Test·정적 계약 검사를 실행한다. 사용자에게 실제 추가/변경 fixture 이름, 영향받는 기존 Test, 예상 Warning/Error를 파일 기준으로 제공한다. Play Mode Test는 격리된 저장소·인증·서비스 대역을 사용한다.

### 사용자 수동 작업

1. Unity Editor에서 프로젝트를 열고 소스 Import·컴파일 완료를 기다린다. Console의 Compile Error 및 예상하지 않은 Warning/Error 유무를 확인한다.
2. `Window > General > Test Runner`에서 EditMode를 선택하고 변경된 Test와 영향받는 기존 Test를 실행한다. 공통 Local Save/인증/제출 구조가 바뀌었다면 해당 기존 Test 전체를 포함한다.
3. PlayMode 탭에서 AI가 제공한 격리된 UI 상태·수명·중복 클릭·원격 요청 차단 Test를 실행한다. 실제 서비스 검증 도구와 혼동하지 않는다.
4. 실행 수·통과/실패 수를 전달한다. 실패가 있으면 Test 이름·메시지·Stack Trace를 전달한다. 토큰이나 실제 요청 자격 증명은 제외한다.

### 완료 조건

- [ ] 최신 변경으로 컴파일과 관련 Edit/Play Mode Test가 통과했다.
- [ ] 실패 수정 뒤 영향받는 Test를 재실행했다. 미실행 결과를 통과로 처리하지 않았다.

## Step 8. 계정·이전 UI를 Scene과 Inspector에 연결한다

### AI 선행 작업

Scene을 읽기 전용으로 대조하고 실제 구현된 Component·직렬화 필드·UI 위치·Navigation·초기 활성 상태를 적은 적용표를 제공한다. 적용표에는 부모 Hierarchy 경로, 생성할 오브젝트 이름, Component 타입, Inspector 필드별 연결 대상, Button 이벤트 등록 방식, 저장할 Scene을 포함한다. 아직 없는 이름을 기존 Scene에 있다고 안내하지 않는다.

### 사용자 수동 작업

1. `Assets/Scenes/SampleScene.unity`를 열고 AI가 제공한 실제 Hierarchy 위치에서 계정 번호 안내·Error/Retry·이전 시작/복구 진입 UI를 만든다.
2. 제공된 부모 경로에 이전 발급/입력/결과 패널을 만들고 TMP 텍스트·입력 필드·버튼과 실제 View/Controller Component를 추가한다.
3. Inspector 적용표대로 번호/코드/인증값/안내·버튼·패널 참조를 연결한다. 코드와 인증값 입력은 문자열로 취급한다. 버튼은 코드 등록과 Inspector 등록이 중복되지 않도록 적용표의 한 방식만 사용한다.
4. Keyboard/Mouse Navigation, 돌아가기·취소 동작, 초기 비활성 패널 상태를 설정하고 Scene을 저장한다.
5. 저장 후 AI의 Scene 정적 참조 검사와 사용자 Test Runner의 Scene 통합 Test를 수행한다. 화면 배치·10자리 가독성은 Step 11에서 확인한다.

### 완료 조건

- [ ] 정확한 적용표대로 Scene 참조를 연결하고 저장했다.
- [ ] 누락 참조·이벤트 중복·초기 상태는 정적 검사 및 격리된 Play Mode Test를 통과했다.

## Step 9. verification에 필요한 서비스 변경만 적용한다

### Step 9-1. 대상 확인·백업·적용표를 확정한다

사용자는 아래 수동 절차 1~2로 verification 대상과 기존 버전을 확인·백업한다. AI는 로컬 파일·입력 정의·의존성·호환 복구 순서를 정적으로 대조한다.

- [ ] 적용 대상·백업·실제 변경표·복구 기준을 기록했다.

### Step 9-2. Secret·권한·필요 설정을 준비한다

사용자는 아래 수동 절차 3 및 5의 필요한 설정을 수행한다. 신규 Script 권한을 게시 후 연결해야 하는 경우에는 적용표에 의존 순서를 명시한다. 기존 Player 직접 Write Deny는 유지한다.

- [ ] 대상 환경의 Secret·권한·설정이 적용표와 일치한다.

### Step 9-3. 호환 순서로 게시하고 적용 결과를 기록한다

사용자는 적용표 순서대로 아래 수동 절차 4와 6을 수행한다. 설정/게시 순서는 상호 의존성을 따른다. 저장 자료의 cutover 뒤에는 무조건 구버전을 재게시하지 않고 Step 1의 호환 복구 기준으로 판정한다.

- [ ] Script·입력 정의·의존성·version 적용 결과를 기록했다.

### AI 선행 작업

Step 1에서 확정한 실제 SDK/Script 배포 방식을 기준으로 파일→원격 Script 이름→입력 정의→의존성→Secret 이름/권한→필요 저장소 설정→버전 호환→복구 순서의 변경표를 제공한다. 원격 검증 도구와 실행 방법도 이때 제공한다. production 배포·30일 sink 구축은 Phase 4에 남긴다.

### 사용자 수동 작업

1. Unity Dashboard에서 Project `c76d55cf-7846-494b-9dce-a0797b179b36`, `verification` Environment `a20a46fa-1edb-4d79-9c35-02f2fed31896`을 선택한다.
2. 변경표 대상의 기존 Script source·활성 version·입력 정의·권한 정책·Secret 이름/권한을 사용자 로컬에 백업한다. 기존 계정·번호·행·ledger를 초기화하지 않는다.
3. 변경표의 정확한 Secret 이름·대상 환경·접근 권한으로 HMAC 비밀을 준비한다. 비밀값은 사용자 환경에서만 입력하며 채팅이나 저장소에 넣지 않는다.
4. 변경표의 신규/변경 Script와 의존성을 실제 배포 방식으로 적용한다. 입력 정의도 로컬 `params` 및 transport와 일치시킨다. 이름이나 입력을 임의로 만들어 게시하지 않는다.
5. 필요한 권한 변경만 기존 정책과 비교해 적용한다. 기존 Player Cloud Save·Leaderboard Write Deny를 유지한다. CLI/API가 필요한 설정은 AI가 공식 문서로 확인한 정확한 명령을 제공한 후 실행한다.
6. 적용한 version·입력 정의 일치 여부·안전한 오류 분류를 전달한다. 예상치 못한 실패에서는 변경표의 호환 가능한 복구 절차를 수행하고 데이터 삭제로 해결하지 않는다.

### 완료 조건

- [ ] verification에 변경표대로 적용했고 대상·백업·version·호환 복구 근거를 기록했다.
- [ ] 원격 Secret/권한은 확인했고 실제 강제력·저장 원자성은 Step 10에서 검증한다.

## Step 10. 실제 서비스의 인증·저장·연결·기록 경계를 검증한다

### Step 10-1. 신규 계정·발급·실제 권한을 확인한다

사용자는 준비된 검증 도구로 격리된 신규 계정의 인증·발급·재조회·고정 행 쓰기·Player 직접 Write 거부를 실행한다. 번호·저장 비교는 도구가 자동 판정한다.

- [ ] 신규 발급·재시작 유지·서비스 토큰/Player 권한의 실제 결과를 기록했다.

### Step 10-2. 기존 자료 전환과 기록 보존을 확인한다

사용자는 신규 계정과 구분된 기존 verification 사례로 cutover·번호 부여·행/최고 기록/수락 시각 보존을 확인한다. 이미 전환된 계정으로 최초 전환 검증을 대체하지 않는다.

- [ ] 기존 자료 전환·재실행·보존의 실제 결과를 기록했다.

### Step 10-3. A/B 이전·고정 행·서비스 복구 경계를 확인한다

사용자는 준비된 A/B 세션으로 잠금·연결 교체·A 거부·B 후속 제출·코드 비활성화를 확인한다. 자동 비교로 행·metadata 보존을 판정하고, 제어 가능한 원격 실패와 미확인 서비스 보장을 구분한다.

- [ ] 이전 뒤 단일 활성 연결·기록 보존·실서비스 확인 범위를 기록했다.

### AI 선행 작업

6-5에서 준비하고 Step 7에서 컴파일·격리 Test를 확인한 verification 실행 도구의 지침을 제공한다. 이 Step에서 코드 수정이 필요하면 게시/실행 전에 영향받는 Step 7 검증을 다시 수행한다. Player context 없는 Dashboard 실행이나 Client가 지정한 임의 Player ID를 인증 성공 근거로 사용하지 않는다.

### 사용자 수동 작업

1. 도구에 지정된 별도 Anonymous 프로필 A/B와 격리된 Local Save로 인증한다. 같은 로컬 프로필을 공유하는 Editor/Player를 두 기기로 간주하지 않는다.
2. 도구의 발급·재조회 검증을 실행하고 재시작 뒤 같은 번호가 돌아오는지 확인한다. 신규·기존 계정 전환은 서로 별도 사례로 실행한다.
3. 검증 도구의 실제 서비스 시나리오를 실행한다: 기존 기록 승계, A 이전 요청·잠금, B 연결, B 후속 제출·조회, A 온라인 거부, 코드 비활성화, 기존 최고·수락 시각 보존.
4. 서비스 토큰 경로의 고정 소유 행 쓰기와 실제 Player 직접 Write 거부도 도구로 확인한다. 새 행 생성/유지 및 CAS·최초 생성·부분 실패 복구의 서비스 관찰 결과는 도구가 비교하고 안전한 통과/실패 분류만 출력한다.
5. 결과 분류·실행 시각·대상 환경·Script version을 기록한다. 공개 번호·Player ID·코드·인증값을 결과 보고에 붙이지 않는다.

실제 서비스가 제어된 장애/경합 주입을 지원하지 않으면 해당 보장을 입증했다고 기록하지 않는다. 로컬 대역 결과와 원격 확인 결과를 구분하고, 미확인 원자성이 Phase 2 필수 조건을 막으면 Step 12를 미완료로 유지한다. 90일·5초 정밀 경계는 Unit Test로 확인한다.

### 완료 조건

- [ ] 신규/기존 계정의 실제 발급·고정 행·서비스 권한·A/B 연결 경계를 확인했다.
- [ ] 필수 서비스 제약의 확인 결과와 미확인 사항을 기록했다.

## Step 11. 두 기기에서 실제 화면 흐름과 Offline 안내를 확인한다

### 사용자 수동 작업

1. Step 10의 격리된 A/B 인증 세션을 사용한다. 한 PC라면 서로 다른 인증 프로필·Local Save 경로를 사용하는 도구 구성을 적용한다. 필요한 검증 Player Build는 AI가 절차를 제공하고 사용자가 Editor에서 실행한다. 최종 Build 판정은 Phase 5다.
2. A에서 복구 제한 안내 동의 후 공개 번호 10자리와 선행 0이 잘리지 않고 보이는지, Leaderboard 본인 행만 `(You)`인지 확인한다.
3. Pending이 있는 A/B에서 이전 버튼을 눌러 전송 완료 또는 폐기 안내를 확인한다. 명시적 폐기를 선택한 경우 저장 성공 후 다시 진행한다. Pending 생성·저장 실패 주입은 Unit Test로 판정한다.
4. B에는 기존 온라인 기록을 가진 사례를 사용한다. Pending이 0건이면 C 이전 입력 화면으로 진행할 수 있어야 한다.
5. A에서 이전을 요청하고 `ABCD-2345` 형식 코드·9자리 인증값·이전 중 안내를 확인한다. B에 직접 입력해 완료한다. 코드·인증값은 테스트 화면에서만 사용한다.
6. B에 C의 기존 공개 번호·서버 개인 최고가 표시되고, A에는 활성 기기 아님 안내와 새 Anonymous 시작 흐름이 나타나는지 확인한다. Settings·튜토리얼·입력 설정과 돌아가기/Navigation을 확인한다.
7. 별도 이전 사례로 A 취소를 확인한다. 발급 화면을 닫거나 앱을 재시작했을 때 원문이 복원되지 않고 재발급·취소 안내로 진행되는지 확인한다.
8. 네트워크를 끊은 상태에서 번호/온라인 기능 Error·Retry 안내와 Offline Run 시작이 가능한지 확인하고, 연결을 복구한다. 명시적 조회 Retry와 제출 Pending Retry를 혼동하지 않는지 확인한다.

수치·서버 행 ID·동시성·최고 기록 덮어쓰기 정확성은 Step 7/10의 자동 비교로 판정한다. 이 Step은 문구·가독성·조작·화면 흐름만 수동 확인한다.

### 완료 조건

- [ ] Keyboard/Mouse 화면 흐름·10자리 표시·Pending 안내·A/B 완료·취소·Offline 안내를 확인했다.
- [ ] 발견한 UI 문제 수정 후 영향받는 정적 검사·Test·화면 사례를 다시 확인했다.

## Step 12. Phase 2 완료를 판정하고 Phase 3~5에 인계한다

### AI 수행

Step 1~11 근거를 Roadmap Phase 2의 번호 유일성/유지·전체 표시·내부 ID와 구분·Offline/계정 혼합 방지·단일 활성 연결·고정 행 metadata·자동 Test/UI 완료 조건과 대조한다. Task·Roadmap·Project Memory를 실제 결과로 갱신한다. 구현 완료·대역 통과·Unity 실행·verification 원격 확인을 별도로 기록한다.

### 사용자 수동 작업

미보고된 컴파일·Test Runner·Scene 적용·서비스·화면 결과가 있을 때만 해당 결과를 전달한다. 이미 확정한 정책을 다시 승인하지 않는다.

### 완료 조건

- [ ] Step 1~11의 필수 구현·검증·사용자 결과가 모두 기록됐다.
- [ ] 미확인 원자성·권한·서비스 제약 또는 필수 실패가 없다.
- [ ] Phase 3의 receipt·조회 한도, Phase 4의 Production/30일 로그·복구, Phase 5의 전체 Test/Build 인계를 기록했다.

# 영향 범위

이번 문서 작성은 Task 및 Roadmap/Phase 1 Task의 후속 경로 연결에 한정한다. 구현 파일·Scene·서비스를 변경하지 않는다.

# 검증 내용

문서의 Step 순서·담당·기존 경로·Phase 1 정책·Roadmap 범위를 정적으로 대조한다. 문서 작성 시 Unity 실행이나 서비스 적용을 요구하지 않는다. 향후 구현 Test는 해당 Step 수행 때 작성·실행한다.

# 검증 결과

Step 1~12 순서, 기존 참조 경로, 각 Step의 사용자 수동 작업·완료 조건을 정적으로 확인했다. 분할 검토에서 21개 하위 Step에 산출물·검증·완료 조건을 추가했고, 서버 저장/예약 기반을 이전 시작보다 먼저 구현하며 검증 도구 준비를 Unity 검증 이전으로 옮겼다. Scene 적용 전후 Test 범위와 cutover 뒤 호환 복구 조건을 명확히 했다. Phase 1 인계 계약 검사와 저장소 정적 계약 검사, `git diff --check`가 통과했다. Step 수행 체크는 모두 미완료로 유지했다. 구현·Unit Test 신규 작성·Unity 실행·원격 적용 결과는 아직 없다.

# 후속 작업

이 문서의 Step 1부터 수행한다. Phase 1 정책 Test를 출발점으로 사용하고 실제 서버·Client 경로 Test로 확장한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md`
- `AI/03_Features/AccountTransfer.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/02_Systems/AccountConnectionSystem.md`
- `AI/02_Systems/RecordSubmissionSystem.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`
- `UGS/TRANSFER_OPERATIONS_RUNBOOK.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_8/20260930_01_Phase1ManualSteps.md`

# 작성 완료 기준

- [x] Phase 1 확정 계약과 Phase 2 수행 범위를 대응시켰다.
- [x] 사용자 수동 작업을 실제 적용·실행·화면 확인 Step으로 나눴다.
- [x] 정적 검증·생산 코드 Unit Test와 사용자 Unity Test Runner의 책임을 지정했다.
- [x] 계획 작성 완료와 Phase 2 수행 완료를 구분했다.
