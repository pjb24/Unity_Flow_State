# 작업 정보

## 작업명

Prototype 8 Phase 3 — 제출·조회 제한 해소 및 수동 적용 절차

## 작업 일자

20261006

## 작업 담당자

- AI: 구현, 정적 검사, Unit Test 작성, 로컬 대역 Test 실행, 사용자 실행 절차 준비.
- 사용자: Unity 컴파일·Test Runner 실행, verification Module 빌드·게시, 실제 서비스·화면 확인, 필요한 Scene 수정.

## 작업 상태

**계획 작성 완료 / Phase 3 구현·실행 검증 대기.** 아래 체크박스는 해당 Step의 실제 결과를 확인한 뒤 완료 처리한다. 이번 작업은 실행 계획 작성이며 서버·Client·Scene·서비스 설정을 변경하지 않았다.

# 작업 목적

제출 이력이 128건에 도달하면 막히는 문제와 보드에 100명보다 많은 기록이 있으면 조회가 실패하는 문제를 해결한다. 오래된 제출 이력을 정리하면서 기존 최고 기록·공개 번호·기기 이전·중복 제출 방지를 유지한다.

사용자는 수백 번 플레이하거나 180일을 기다리는 대신, 자동 Test 결과를 확인하고 실제 계정으로 제출·조회·화면을 확인한다.

# 작업 대상

- 서버: `UGS/CloudCode`, `UGS/Modules/FlowStateVerification`.
- Client: 제출 후보·Local Save·Pending 재시도·Leaderboard 조회/표시.
- 검증: `UGS/Tests`, `Assets/Tests/EditMode`, `Assets/Tests/PlayMode`, 필요한 격리 검증 도구.
- 게시 대상: `Assets/CloudCode/FlowStateVerification.ccmr`의 C# Module.

# 작업 전 상태

- `submit-record.js`는 논리 계정 ledger의 entries가 128건이면 `LedgerCapacity`로 실패한다.
- `query-records.js`는 offset 0, limit 100 단일 페이지를 읽고 total이 100보다 크면 `VerificationBoardCapacity`로 실패한다.
- 계정 자동 준비와 논리 계정 ledger 생성은 Phase 2의 `account-provisioning-service.js`에 이미 있다. Phase 3에서는 이를 재사용하고 직접 첫 제출 경로·부분 실패 복구를 보강한다.
- `submission-retention-policy.js`와 `leaderboard-query-policy.js`는 정책 계산이다. 실제 receipt 분할·정리와 다중 페이지 조회의 구현 완료 근거가 아니다.
- Local Save 현재 version은 6이다. Pending 생성 시각 저장·이전 파일 migration은 구현 시 실제 모델과 대조한다.
- 최신 사용자 보고: Unity 컴파일 및 예상 밖 Error/Warning 없음, EditMode 887개·PlayMode 254개 성공. 이는 Phase 3 변경 전 기준이다.
- 사용자 임시 관리자 Key 폐기·역할 회수 보고는 확보돼 있다. Phase 2 문서 일부에는 이전 대기 상태가 남아 있으므로 Step 1에서 최신 보고와 상태를 맞춘다.

# 조사 내용

로드맵 Phase 3, Phase 1 Step 3·4 및 후속 인계, Phase 2 Task, RecordSubmission·Leaderboard 계약, 실제 제출·조회·계정 준비 소스를 대조했다.

계약의 핵심은 다음과 같다.

- 서버 terminal receipt: Submitted/Rejected 확정 서버 시각부터 정확히 180일 보관. 논리 계정별 시간 분할 저장.
- Local Pending: 생성 시각부터 180일에 `SubmissionExpired`로 정리. 생성 시각 없는 이전 파일은 최초 업그레이드 로드 시각을 한 번 저장.
- 보관 중 같은 계정·ID·payload는 기존 결과, 다른 payload는 `SubmissionIdConflict`. receipt가 실제 정리된 뒤 같은 ID는 새 제출로 처리.
- 진행 중 제출·최고 기록·계정·공개 번호·Leaderboard 행과 metadata는 terminal receipt 정리와 분리해 보존.
- TOP 최대 10행, AROUND YOU 최대 7행. 전체 순위는 공동 순위이며 동점 표시 순서는 score → acceptedAt → 정확한 공개 번호 순서.
- 자동 타이머 갱신 대신 최초 진입·명시적 Retry/새로고침·재진입에서 조회.
- 기존 5초 작업 제한과 기기 이전의 활성 연결 검사를 유지. 대량 조회를 위해 대기 제한을 늘리는 방식으로 완료 처리하지 않는다.

# 작업 내용

## 진행 순서

Step 1~6은 AI 구현·검증 작업이다. **사용자는 Step 7부터 직접 실행한다.** Step 7~9는 앞 단계 준비가 완료된 후 진행한다.

| Step | 목적 | 직접 수행자 |
| --- | --- | --- |
| 1 | 기존 구현·서비스 제약·변경 목록 확정 | AI |
| 2 | 신규 계정 첫 제출 자동 준비 확인·보강 | AI |
| 3 | 128건 제한을 receipt 분할·180일 정리로 대체 | AI |
| 4 | Local Pending 생성 시각·만료·migration 구현 | AI |
| 5 | 100명 초과 조회·전역 순위 구현 | AI |
| 6 | 자동 Test·정적 검사·수동 실행 도구 준비 | AI |
| 7 | Unity 컴파일·전체 Edit/PlayMode Test 실행 | 사용자 |
| 8 | 게시 전 백업·verification Module 게시 | 사용자 |
| 9 | 실제 제출·조회·Offline 복구·화면 확인 | 사용자 |
| 10 | 근거 대조·Phase 3 완료 판정 | AI + 사용자 결과 |

## Step 1. 구현 시작 전에 변경 범위를 확정한다

**목적:** 기존 기능을 재사용하고 저장·조회 변경에 필요한 제약을 먼저 확인한다.

AI 작업:

1. Phase 2 최신 사용자 성공 보고·Key/역할 회수 보고와 Task/Roadmap의 현재 상태를 대조해 과거 대기 표기를 구분한다.
2. 신규 계정 준비, 논리 계정 ledger, 기기 이전의 제출 예약, Client Pending, Module 서비스 adapter의 실제 호출 경로를 정리한다.
3. 실제 사용하는 Unity 공식 API 문서와 설치 SDK에서 페이지 조회·저장 item 크기/개수·CAS·삭제·시간 제한을 확인하고 출처·확인일을 기록한다.
4. 시간 분할 단위·receipt 검색 방법·정리 실행 계기·기존 ledger 전환 절차·조회 snapshot 일관성 방법을 확정한다. 단일 객체 무한 누적이나 전체 행을 무제한 순회하는 임시 해법과 구분한다.
5. 삭제 API/페이지 API가 Module adapter에 필요하면 변경 목록에 포함한다. 필요한 실제 서비스 검증이 대역만으로 충분하지 않다면 Step 9에 안전한 격리 검사를 별도로 준비한다.

사용자 작업: 현재 없음. 추가 권한·데이터 변경 승인이 필요해지면 정확한 대상과 이유를 제시한다.

완료 조건: [ ] 구현 방식·변경 파일·기존 자료 보존·API 근거와 미확인 사항이 정리됐다.

## Step 2. 새 계정의 첫 제출을 자동으로 준비한다

**목적:** Dashboard에서 ledger를 만들어 주지 않아도 첫 기록을 제출할 수 있게 한다.

AI 작업:

1. 기존 계정 준비 코드를 재사용해 첫 온라인 제출에 필요한 저장 자료를 준비한다.
2. 번호 조회가 선행되지 않은 첫 제출, 동시 첫 요청, 각 저장 경계의 실패·응답 유실 뒤 복구를 테스트한다.
3. 기존 계정과 이전 후 새 활성 기기의 기록·번호·소유 행을 보존한다.

사용자 작업: 현재 없음. 실제 새 계정의 첫 제출은 Step 9-A에서 확인한다.

완료 조건: [ ] 첫 제출·동시 초기화·부분 실패·기존 계정 보존 Test 통과.

## Step 3. 서버 제출 이력을 분할하고 오래된 receipt를 정리한다

**목적:** 129번째 이후에도 제출을 처리하고, 완료 이력을 영구 누적하지 않게 한다.

AI 작업:

1. terminal receipt를 계정별 시간 분할 저장으로 전환하고, 진행 중 요청·Board best와 분리한다.
2. 기존 entries를 전환하는 중단·재시도에서도 ID·payload·결과·서버 시각을 보존한다.
3. 180일 경계에서 만료 receipt의 저장 삭제 성공을 확인한 후 신규 제출처럼 처리한다. 삭제 실패·응답 미확인은 복구 경로로 처리한다.
4. 동시 같은 ID·payload 충돌·응답 유실·제출/정리/이전 경합에 CAS와 활성 연결 검사를 적용한다.
5. `GetAccountPersonalBests` 및 이전 복구가 새 저장 형식을 읽도록 연동한다. 이전 완료 receipt와 제출 terminal receipt를 구분한다.

사용자 작업: 현재 없음. 128건 초과와 시간 경계는 Step 6의 자동 Test로 판정한다.

완료 조건: [ ] 128건 초과·정리·migration·복구·기기 이전 회귀 Test 통과.

## Step 4. Local Pending의 생성 시각과 만료를 구현한다

**목적:** 오래된 대기 기록을 명확히 정리하고 정상 Pending의 재시도 정보를 유지한다.

AI 작업:

1. 후보 생성 때 생성 시각을 저장하고 재시도·재시작에서 동일 시각·제출 ID·계정 귀속을 유지한다.
2. 생성 시각 없는 이전 Local Save를 전환한다. 다시 로드해도 시작 시각이 바뀌지 않도록 한다.
3. 180일 경계에서 `SubmissionExpired`를 표시하고 해당 항목만 저장 제거한다. 제거 저장 실패를 검증한다.
4. 다른 계정·다른 환경의 Pending과 기기 공용 Settings·입력 설정·Tutorial을 보존한다.
5. 만료 안내는 영어 문구로 작성하고 표시/선택 상태를 Test로 검증한다.

사용자 작업: 현재 없음. 만료 화면은 Step 6에서 준비하는 **복제 저장 + 가짜 시계의 로컬 전용 검증**으로 Step 9-C에서 확인한다.

완료 조건: [ ] Local Save migration·만료·저장 실패·재시작·환경/계정 격리 Test 통과.

## Step 5. 대규모 보드에서 TOP과 AROUND YOU를 조회한다

**목적:** 100명 초과 보드와 페이지 경계에서도 정확한 목록·순위를 표시한다.

AI 작업:

1. 상위 10·주변 7을 반환하도록 실제 조회 경로와 Module adapter를 구현한다. 검증용 `me` 조회·개인 최고 조회와의 호환성을 유지한다.
2. Stage 오름차순·Infinite 내림차순에서 전역 competition rank를 계산한다. API rank를 사용한다면 계약과 일치함을 증명한다.
3. 페이지를 가로지르는 동점과 acceptedAt 동률에서 공개 번호 순서를 보존한다. 부분 페이지의 첫 행을 무조건 1위로 처리하지 않는다.
4. 읽는 동안 행이 변경되거나 페이지 누락·중복·번호 연결 실패가 있으면 불완전 결과를 성공/Empty로 표시하지 않도록 한다.
5. TOP/AROUND YOU 독립 상태, 명시적 갱신, 안전한 오류·내 행 표시·계정 이전 회귀를 유지한다.

사용자 작업: 현재 없음. 순위 계산과 페이지 경계는 자동 Test로 판정한다.

완료 조건: [ ] 100명 초과·동점·내 주변 경계·부분 실패·5초 제한 Test 통과.

## Step 6. 자동 Test와 수동 확인 준비를 끝낸다

**목적:** 사람의 반복 플레이·정밀 클릭·장기간 대기 없이 경계를 검증한다.

AI는 아래 사례를 기존 Test에 보강한다. 독립된 새 책임만 별도 fixture로 나누고 단순 중복 Test를 추가하지 않는다.

| 검증 대상 | 자동 검증할 사례 | 실행 수단 |
| --- | --- | --- |
| 신규 계정 준비 | 첫 직접 제출, 동시 준비, 저장 전/후 실패·복구 | 서버 대역 Unit Test |
| 제출 수량 | 127·128·129·256건, 여러 분할 구간 | 서버 대역 Unit Test |
| 중복 판정 | 같은 ID/payload, 다른 payload, 응답 유실 뒤 동일 ID | 서버 대역 + EditMode |
| receipt 만료 | 180일 직전·동일·직후, 삭제 실패/응답 유실·동시 정리 | 가짜 시계·저장소 대역 |
| 기존 자료 전환 | 일부 전환 후 중단/재시도, best/metadata 유지 | 서버 대역 Unit Test |
| Local Pending | 생성 시각 보존, 이전 version 전환 1회, 만료 저장 실패 | EditMode |
| 격리 | 계정/환경 혼합 방지, 만료로 Settings/입력 설정 유지 | EditMode + 기존 PlayMode |
| 조회 수량 | 0·1·7·10·99·100·101·201행, 서비스 페이지 크기 경계 | 서버 대역 Unit Test |
| 전역 순위 | 1,1,3, 페이지 경계 동점, acceptedAt 동률·공개 번호 정렬 | 서버 대역 Unit Test |
| 내 주변 | 첫 행·중간·마지막·내 행 없음, 최대 7행 | 서버 대역 + EditMode |
| 조회 실패 | 페이지 누락/중복/변동·번호 연결 실패·Timeout·불완전 성공 차단 | 서버 대역 Unit Test |
| UI 연동 | TOP/AROUND YOU 독립 상태·영어 안내·명시적 갱신·포커스 | EditMode + PlayMode |
| 기기 이전 | 분할 receipt/best 유지·이전과 제출/정리 경합·옛 기기 거부 | 기존 이전 회귀 확장 |

추가 AI 준비:

1. 소스 문법·중복 C# 멤버·asmdef/meta·저장 version·고정 Project/Environment·9함수 연결·비밀값 노출·문서 링크를 정적으로 검사한다.
2. 실제 서버 원본/Module shim을 사용하는 로컬 Node Test를 실행한다. 대역 결과와 실제 .NET/서비스 결과를 구분해 기록한다.
3. Unit Test는 가짜 시간·가짜 서비스·메모리/임시 저장을 사용한다. 실제 인증·원격 게시와 자동 Test를 분리한다.
4. 수동 확인용 새 계정 프로필 `flow-state-phase3-new`와 기존 계정 회귀 경로를 격리 도구에 준비한다. 새 프로필은 최초 번호 조회보다 **첫 직접 제출**을 먼저 시험할 수 있게 한다.
5. 실제 사용 가능한 버튼 이름·저장 전체 경로·안전한 PASS/FAIL 메시지를 이 문서 Step 9에 갱신한다. **현재 Phase 3 전용 프로필/버튼/만료 미리보기는 아직 준비 전이다.** 사용자에게 없는 버튼을 찾게 하지 않는다.
6. Scene 참조 변경이 필요한 경우 사용자용 별도 하위 Step에 Hierarchy 전체 경로·컴포넌트·참조 슬롯·영어 Text·RectTransform·색·Navigation 값을 지정한다. 기존 Scene을 읽기 전용 대조하고 실제 수정은 사용자에게 맡긴다.

완료 조건: [ ] 로컬 서버 Test·정적 검사 통과, Unity Test 작성 완료, Step 9 실제 실행 경로 확정.

## Step 7. Unity 컴파일과 전체 Test를 실행한다 — 사용자

**목적:** 실제 Unity에서 변경 코드와 기존 게임/UI가 함께 동작하는지 확인한다.

1. Unity에서 프로젝트를 열고 Script Compilation이 끝날 때까지 기다린다.
2. Console에서 새 Error/Warning을 확인한다. 오류가 있으면 메시지를 전달해 수정한 뒤 다음 순서를 진행한다.
3. **Window → General → Test Runner → EditMode → Run All**을 실행한다.
4. **PlayMode → Run All**을 실행한다.
5. 아래 형식으로 결과를 알려 준다. 수량은 실행 화면에 표시된 실제 수량을 적는다.

```text
Script Compilation: 성공 / 실패
예상 밖 컴파일 Error·Warning: 없음 / 메시지
EditMode: 성공 수 / 전체 수
PlayMode: 성공 수 / 전체 수
예상 밖 테스트 Error·Warning: 없음 / 메시지
```

완료 조건: [ ] 최신 변경 컴파일·전체 Edit/PlayMode 성공 및 예상 밖 Error/Warning 없음.

## Step 8. 백업하고 verification Module을 게시한다 — 사용자

**목적:** 새 저장·조회 코드를 실제 verification 서비스에서 사용한다.

### 8-A. 게시 전 백업한다

1. Play를 종료한다. 테스트 앱을 닫는다.
2. `C:\Unity\Unity_Flow_State\Ignore\StepPhase3\Before` 폴더를 만든다.
3. 현재 게시된 Module을 재게시할 수 있는 이전 소스/게시 산출물과 활성 버전 정보를 이 폴더에 보관한다. API 스펙만 보관하는 것과 실행 코드 백업을 구분한다.
4. Step 6 도구가 표시하는 기존 검증 계정의 Local Save·계정/ledger 자료·Stage/Infinite 최고 행을 이 폴더에 저장한다. 전체 경로와 최소 캡처 항목은 Step 6에서 확정해 이 절차에 추가한다.
5. Unity Dashboard에서 프로젝트 **Unity_Flow_State**, 환경 **verification**을 선택하고 Project/Environment ID를 아래 값과 비교한다.

```text
Project ID: c76d55cf-7846-494b-9dce-a0797b179b36
Environment ID: a20a46fa-1edb-4d79-9c35-02f2fed31896
```

### 8-B. Module을 게시한다

1. 기존 Phase 2 게시 절차로 새 `FlowStateVerification` Module을 빌드한다.
2. **Window → Services → Deployment**를 연다. 대상 환경을 **verification**으로 선택한다.
3. **FlowStateVerification** Module 하나를 선택하고 **Deploy Selected**를 실행한다.
4. 성공 여부·활성 버전·게시 시각을 기록해 알려 준다.

저장 형식 변경은 Step 1에서 확정한 전환 방식으로 적용한다. 코드와 데이터 형식이 호환되지 않는 경우의 복구 절차를 게시 전에 확정한다. 이전 Module 재게시만으로 새 저장 자료와 호환된다고 가정하지 않는다.

완료 조건: [ ] 이전 실행 코드/검증 자료 백업·대상 일치·Module 게시 성공.

## Step 9. 실제 제출·조회와 화면을 확인한다 — 사용자

**진행 조건:** Step 6의 도구·버튼·경로 확정과 Step 7~8 성공 후 실행한다. 다음은 확인 순서이며 실행용 상세 버튼은 Step 6에서 준비 후 갱신한다.

### 9-A. 새 계정으로 첫 기록을 제출한다

**목적:** 수동 ledger 생성 없이 신규 계정이 실제 서비스를 이용하는지 확인한다.

1. Step 6에서 준비한 새 계정 격리 실행을 시작한다. 프로필이 `flow-state-phase3-new`인지 확인한다.
2. 복구 안내에 동의하고 verification 원격 요청을 허용한다.
3. 첫 직접 Stage 제출 검사를 실행한다. 이 검사는 유효 후보 하나를 만들어 생산 제출 경로로 보내고 서버의 본인 행을 확인하도록 Step 6에서 준비한다.
4. `Submitted` 확정과 서버 본인 행·점수·수락 시각 확인 결과를 복사한다.
5. 공개 번호 조회 후 종료·재시작하여 같은 번호·행을 확인한다.

완료 조건: [ ] 새 계정 첫 제출·서버 행 확인·재시작 보존 통과.

### 9-B. 기존 계정의 최고 기록과 조회를 확인한다

**목적:** 기존 자료 전환 후 기록과 화면이 그대로 유지되는지 확인한다.

1. Editor를 종료하고 기존 A용 바로가기로 실행한다. 기존 인증 프로필·저장 파일을 사용한다.
2. 검증 창의 기존 기준 자료 비교 기능으로 게시 전 공개 번호·최고 score·acceptedAt를 비교한다. 별도 새 기록을 제출하기 전에 비교한다.
3. 일반 게임 화면 확인은 격리 실행 예약을 해제한 다음 Play로 시작한다. Main Menu의 **Leaderboard → Stage**를 연다.
4. **TOP**, **AROUND YOU**의 공개 번호·`(You)`·행·시간 표시를 확인한다. 명시적 **Retry**로 목록을 다시 불러온다.
5. **Infinite** 탭을 열고 점수·공개 번호·본인 행·독립 목록을 확인한다. 자기 기록이 없는 경우에는 그 상태 안내를 확인한다.
6. Keyboard 이동/선택과 Mouse 클릭으로 Retry·Back을 사용한다. 영어 안내의 잘림·깨짐을 확인한다.

최대 10·7보다 적은 실제 기록은 존재하는 수만 표시한다. 100명 초과·동점 전역 순위의 판정 근거는 Step 6 Test로 기록한다. 실제 서비스의 대규모 경계 검사가 추가로 필요하다고 Step 1에서 판단하면, 전용 자료·승인·실행·정리 절차를 준비해 별도 하위 Step으로 제공한다.

완료 조건: [ ] 기존 기준 보존·두 Mode 조회·화면/조작 확인 통과.

### 9-C. Offline Pending 복구와 만료 안내를 확인한다

**목적:** 정상 대기 기록은 복구되고 오래된 대기 기록의 안내가 읽히는지 확인한다.

1. 기존 검증 계정으로 일반 게임을 실행한다. 인터넷 연결을 끊고 Stage를 클리어한다.
2. Result에서 Pending 안내와 제출 재시도 버튼을 확인한다. Play를 종료하고 다시 실행해 같은 Pending이 남아 있는지 확인한다.
3. 인터넷을 연결하고 **Refresh Status**로 계정 상태를 확인한다. **Retry Pending Uploads**로 저장된 후보를 재전송한다.
4. 확정 응답과 해당 Pending 제거 결과를 복사한다. Timeout이면 현재 결과를 전달하고 기존 Pending 재시도 절차로 이어간다.
5. Step 6에서 준비한 **로컬 전용 만료 미리보기**를 실행한다. 복제 저장·가짜 시계의 오래된 후보로 `SubmissionExpired`에 대응하는 영어 안내가 잘 보이는지 확인한다.
6. 만료 안내 후 버튼/포커스·남은 Pending 표시를 확인하고 일반 게임으로 복귀한다.

180일의 계산·동일 ID·삭제 저장 실패는 자동 Test 결과로 판정한다. 수동 미리보기는 화면 가독성 확인이며 원격 receipt 삭제 성공의 근거와 구분한다.

완료 조건: [ ] 실제 Offline 복구·확정 응답·재시작 보존·만료 안내 확인 통과.

### 9-D. Scene 변경이 생긴 경우 적용한다

Step 6에서 참조/레이아웃 변경이 확인된 경우에만 준비한 Scene 작업표를 따라 적용한다. 사용자는 Play 종료 → 대상 Scene 열기 → 지정 개체/슬롯 수정 → Scene 저장 → Step 7 Test 재실행 → 관련 Step 9 화면 확인 순서로 진행한다. 변경이 없으면 AI가 읽기 전용 대조 근거로 **해당 없음** 처리한다.

## Step 10. Phase 3 완료를 판정한다

**목적:** 구현·대역·Unity·실제 서비스·화면 근거를 분리해 완료를 결정한다.

AI 작업:

1. Step 1~9 결과를 로드맵 Phase 3 완료 조건과 대조한다.
2. 128건 초과, 100명 초과, 중복/정리/180일 Pending, 페이지 경계 순위의 자동 Test 근거를 기록한다.
3. 실제 신규 제출·기존 자료 보존·서비스 조회·Offline 복구·UI 결과를 별도로 기록한다.
4. 필수 실패와 미확인 항목을 정리하고 Task·Roadmap·Project Memory 상태를 갱신한다.

완료 조건: [ ] 필수 구현·자동 Test·Unity·verification·화면 확인 완료, 미해결 필수 실패 없음.

# 영향 범위

이번 작업은 Phase 3 실행 계획 문서와 로드맵의 계획 연결에 한정한다. 실제 구현 영향 범위는 각 Step에서 확정한다. Production 배포·로그 보존/운영 복구는 Phase 4, Windows Player Build 통합 판정은 Phase 5에 연결한다.

# 검증 내용

- Phase 3 로드맵의 구현 대상·완료 조건과 Step 1~10 및 Test 대응표를 대조했다.
- 기존 128/100 제한, 자동 계정 준비, Module 게시 방식, 현재 검증 프로필을 소스에서 확인했다.
- 수동 작업과 자동 경계 검증을 분리하고 아직 없는 실행 도구는 준비 전으로 표시했다.
- 문서 정적 검사에서 Step 1~10 순서, 모든 로컬 링크 대상 존재, 기간/용량/Test/준비 조건, 문서 공백 검사를 통과했다. 로드맵 변경의 `git diff --check`도 통과했다.

# 검증 결과

계획 문서 작성 완료. Phase 3 구현·Unit Test 작성/실행·Unity 실행·실서비스 검증은 미수행이다. 기존 Phase 2의 결과를 Phase 3 완료 근거로 사용하지 않는다.

# 후속 작업

Step 1부터 수행한다. Step 6에서 실제 수동 버튼·자료 경로를 확정한 뒤 사용자가 Step 7~9를 진행한다.

# 관련 문서

- [Roadmap 008](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [RecordSubmission](../../03_Features/RecordSubmission.md)
- [Leaderboard](../../03_Features/Leaderboard.md)
- [RecordSubmissionSystem](../../02_Systems/RecordSubmissionSystem.md)
- [검증 규칙](../../01_Rules/VERIFICATION_RULE.md)
- [구현 규칙](../../01_Rules/IMPLEMENTATION_RULE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
- [Module 게시 안내](../../../UGS/VERIFICATION_DEPLOYMENT.md)

# 관련 작업 기록

- [Phase 1 정책·인계](20260930_01_Phase1ManualSteps.md)
- [Phase 2 실행·검증](20261003_01_Phase2ManualSteps.md)

# 작성 완료 기준

- [x] Phase 3 수행 순서와 담당자를 Step으로 표현했다.
- [x] 사용자가 직접 해야 하는 작업에 목적·동작·기대 결과를 명시했다.
- [x] 수량·시간·경합·migration을 정적 검사/Unit Test 중심으로 배치했다.
- [x] 계획 완료와 구현·Unity·원격 검증 완료를 구분했다.
