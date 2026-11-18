# 작업 정보

## 작업명

Prototype 9 Phase 4 — 통합 회귀와 Infinite 단일화 전환 후보 판정

## 작업 일자

20261118

## 작업 담당자

- AI: 기준선/변경 영향 조사, 코드·Unit Test 보완, 정적 검증·로컬 SDK 대역 실행, Scene 읽기 전용 검사, 결과/잔여 범위 기록.
- 사용자: 필요한 Unity Compile·Test Runner·Scene 적용, 최신 Player 빌드와 실행/화면 확인.

## 작업 상태

**실행 절차 작성 완료. Step 1~10은 미착수이며 Phase 4와 Roadmap 9 전체는 미완료다.** 문서 작성 중 소스/설정/기존 검증 문서를 읽었으며 Phase 4의 검사·Test·빌드·Player 실행을 완료한 것으로 처리하지 않는다.

# 작업 목적

Phase 1~3에서 완료한 Stage 플레이/기록 접근 차단과 Infinite·공통 저장/계정 계약을 최신 소스·자동 검증·실제 Player 후보에 연결한다.
정적으로 확인할 수 있는 파일/참조/조건은 AI가 검사하고, 상태/계산/빠른 입력·비동기 경합은 Unit Test로 판정한다.
사용자 실행은 실제 Unity 컴파일/테스트·빌드와 Player 화면/조작 확인에 모으고 최종 전환 후보의 검증 범위와 미검증 범위를 명확히 남긴다.

# 작업 대상

- 생산 진입: GameSystem, RuntimeDataSystem, StageSystem, InfiniteModeSystem, UIManagementSystem, ResultSystem과 실제 직렬화된 UI/입력 연결.
- 기록/계정: RecordSubmissionPolicy/Service, Local/CloudCode Repository, OnlineRecordCoordinator, 계정 이전·전체 개인 최고 snapshot·환경별 저장 경계.
- Editor: OnlineRecordVerificationWindow와 verification 격리/자동 실행 금지·권한·timeout 보호.
- Scene/빌드: `Assets/Scenes/SampleScene.unity`, `ProjectSettings/EditorBuildSettings.asset`, `Assets/Settings/Build Profiles/Windows-production.asset`, production 설정과 실제 후보 출력.
- 자동 검증: 기존 EditMode/PlayMode assembly 전체와 아래 영향별 suite, `UGS/Tests/`의 읽기 전용 소스 계약/오프라인 SDK 대역.
- 결과 문서: 이 Task, Roadmap 9, Project Memory와 실제 계약이 달라진 Feature/System.

# 작업 전 상태

1. Phase 1~3은 완료했다. 최신 Phase 3 근거는 **사용자 Script Compilation 성공·EditMode 1031/1031·PlayMode 316/316 성공 및 각 예상 밖 Error/Warning 없음** 보고다. AI의 직접 실행/XML·로그 원문 검토 결과가 아니다.
2. Phase 3 최종 정적/로컬 16스크립트가 통과했다. Scene/Prefab 추가 적용은 없었고 SampleScene은 기준본과 동일했다. Stage/Infinite enum·내부 자료형·v7 저장·전체 snapshot과 `fs-infinite-v2`/Scoring Version 2를 보존했다.
3. Phase 2의 `Build/ProductionQA/Candidate02/`는 당시 Windows-production Player 기준선이다. **Phase 3 최신 소스를 포함한 Player 검증으로 승계하지 않는다.** 사용자는 과거 Candidate02 실물 보존이 불필요하다고 결정했다. 새 폴더 생성/옛 빌드 보존을 다시 의무화하지 않는다.
4. 기존 Stage 기록이 없고 앞으로도 제출되지 않는 사용자 전제와 구 Client 미지원 정책을 유지한다. Stage 자료 조사/보관/이관·혼합 큐 호환·보드 삭제/폐기·Infinite 변환/병합 기능을 추가하지 않는다.
5. 이번 전환에 서버 계약/Module 변경은 필요하지 않다는 선행 판정을 유지한다. Phase 4라는 이유만으로 배포/복구 시험·원격 probe·계정 이전/자료 삭제를 시작하지 않는다. 실제 변경 필요성이 발견되면 원인·최소 변경·검증/복구 조건을 먼저 준비한다.

## 작업 방식

- 각 Step의 AI 작업·Unit Test·사용자 작업과 완료 근거를 기록한 뒤 체크한다. 문서 작성만으로 실행 체크를 하지 않는다.
- Phase 1~3 완료 항목을 재구현하지 않는다. 새 변경이 없고 검증 조건이 같으면 기존 최신 Compile/1031·316 결과를 사용한다. 새 코드/Test/Scene·빌드 조건 변경이나 미해결 문제에 근거가 있을 때만 해당 검증을 갱신한다.
- AI는 Unity Compile·Build·Test Runner·Player를 실행하지 않는다. Scene/Prefab은 읽기 전용으로 검사하며 변경이 필요하면 정확한 적용표를 사용자에게 제공한다.
- UI GameObject/컴포넌트 동적 생성·누락 보충·Runtime 고정 레이아웃 덮어쓰기는 금지한다. Text/활성/포커스·입력/스크롤 상태 갱신은 유지한다.
- Unit Test는 실제 정책/생산 Scene과 fake 인증/전송/clock/지연·메모리 저장을 사용한다. 실제 사용자 저장·UGS 인증·전송/이전은 자동 Test에서 실행하지 않는다.
- 같은/인접 프레임·중복 click/Submit·응답 유실/지연·저장 실패/경합은 자동 Test로 판정하며 사람에게 빠른 입력 재현을 요구하지 않는다.
- 실패 Test 비활성화·광범위한 로그 무시·과거 검증 면제 자동 승계·필수 실패의 N/A 처리는 하지 않는다. source assertion/로컬 대역 성공을 Unity/원격 성공으로 바꾸지 않는다.
- 실제 저장/기기 계정·기존 온라인 자료를 초기화하거나 삭제해 새 사용자 조건을 만들지 않는다. 신규 사용자/손상 자료/오래된 선택은 격리된 Test fixture로 검증한다.

# 조사 내용

문서 작성 시 다음 사실을 읽기 전용으로 확인했다. 아래 내용을 Phase 4 실행 결과로 오인하지 않는다.

| 확인한 자료 | 현재 내용 | 실행 연결 |
| --- | --- | --- |
| Roadmap 9 Phase 4 | 통합 회귀/사용자 실행 근거 연결·계약 보존·후보 변경/잔여 위험 판정이 완료 조건 | Step 1, 10 |
| Phase 3 Task 8-1~8-5, 9~11 | 최종 경로 폐쇄·16스크립트 통과·사용자 Compile/1031·316 성공·최신 Player 후보 인계 | Step 1~8 |
| Windows-production.asset | FLOW_STATE_PRODUCTION 포함, 자체 Scene 목록 override 없음, Development/Debugging 비활성 | Step 2, 6 |
| 기존 Test 소스/assembly | 실제 SampleScene 기반 진입/모드 UI/Pause/Result·계정 격리 Test와 정책/저장/전송 Test 존재 | Step 3, 5 |
| Project Memory/공통 Roadmap 안내 | AI/사용자 책임 분리·Stage 자료/구 Client 범위 제외·Module 변경 시 별도 보존/복구 경계 | 전체 Step |

# 작업 내용

## 실행 순서

| Step | 수행할 실제 작업 | 담당 |
| --- | --- | --- |
| 1 | Phase 3 기준선·현재 변경·증거/필수 판정표 확정 | AI |
| 2 | 생산 진입·Scene/빌드/환경 설정의 최종 정적 감사 | AI |
| 3 | 자동 회귀 범위/공백 대조와 필요한 코드·Unit Test 보완 | AI |
| 4 | 영향별 정적 검사·오프라인 SDK 대역 실행과 실패 해결 | AI |
| 5 | 필요한 최신 Compile/Scene 적용·Unity 회귀 결과 확보 | 사용자 + AI |
| 6 | 최신 소스/설정과 연결되는 Windows-production 후보 빌드 | 사용자 + AI |
| 7 | Player의 Infinite 흐름·화면/포커스·재시작 확인 | 사용자 + AI |
| 8 | 온라인/저장/이전의 증거 구분과 필요한 제한적 실서비스 범위 판정 | AI + 필요한 사용자 실행 |
| 9 | 발견한 결함 수정·영향 회귀·후보 갱신과 재확인 | AI + 사용자 |
| 10 | Phase 4/Roadmap 9 최종 판정·잔여 의무·다음 Roadmap 인계 | AI |

## Step 1. 검증 기준선과 필수 증거를 확정한다

### AI 작업

1-1. Phase 1 정책, Phase 2 최종 Candidate02 보고, Phase 3 8-1/11-1~11-2와 사용자 최신 Compile/1031·316 성공을 대조한다. 과거 Player와 최신 소스/자동 Test 근거를 분리한다.

1-2. 현재 작업 트리/변경 파일·소스/Scene/빌드 설정을 기록한다. 미커밋 상태도 가능하며 기존 변경을 보존한다. commit 유무만으로 검증을 멈추지 않는다. 필요하면 대상 파일 해시/파일 목록으로 사용자 실행 시점을 연결하며 Secret/실제 저장 payload를 수집하지 않는다.

1-3. 아래 판정 영역별로 `필수 증거 / 기존 충족 근거 / 공백 / 수행 Step / 실제 결과` 표를 만든다. 조사 결과에 따라 범위를 좁히되 미검증 필수 항목을 완료로 표시하지 않는다.

| 판정 영역 | 기본 근거 | 남는 실제 확인 |
| --- | --- | --- |
| Stage 시작/기록/UI 직접 진입 차단 | 소스 경계·Stage/미정의 값 거절 Unit Test·생산 Scene 연결 | 최신 Player에 해당 소스/설정 반영 |
| Infinite 시작/Pause/Result/복귀 | Navigation/생산 Scene/입력 자동 Test | Player의 정상 속도 조작/화면/포커스 |
| 저장/Pending/이전·전체 snapshot | codec/환경/내구 저장/경합 Test·실제 서비스 로컬 SDK 대역 | Player 재시작의 기본 설정 보존·필요한 원격 범위 분리 |
| 빌드/환경/후보 귀속 | Build Profile·Scene 목록·production 설정 정적 감사 | 사용자 최신 빌드 성공과 실행한 후보 경로/보고 |

### 사용자 수동 작업

없음. 이미 보고한 Compile/Test/정책을 다시 질문하지 않는다.

### 완료 조건

- [ ] 최신 소스/설정·과거 후보·1031/316 결과의 범위와 필수 증거/공백 표를 기록했다.

## Step 2. 생산 경로·Scene·빌드 환경을 정적으로 감사한다

### AI 작업

2-1. Stage 시작/재시작·과거 선택·직접 API·Result·Leaderboard·Editor probe의 생산 호출망을 Phase 3 폐쇄 표와 대조한다. 남은 Stage 참조는 차단된 본문·Deprecated 모델·codec/snapshot 내부 복원·합성 Test로 분류한다. enum/형식/GUID 보존을 이유로 실행 허용을 추정하지 않는다.

2-2. SampleScene/관련 Prefab을 읽기 전용으로 조사해 필수 참조 타입·GUID/fileID·이벤트 대상/메서드·Navigation 방향·초기 비활성·숨긴 Stage UI의 포커스 차단·Settings/이전 패널 연결을 확인한다. Phase 3의 block/참조 수는 비교 근거이며 고정 개수만 맞추는 검사를 하지 않는다. 패키지/TMP metadata가 검색 ignore로 빠지지 않게 한다.

2-3. Build Profile과 EditorBuildSettings의 실제 Scene 목록/활성 여부·platform·FLOW_STATE_PRODUCTION·production 설정의 환경 분리와 서비스 진입 조건을 대조한다. profile의 m_Scenes가 비어 있어도 전역 목록을 쓰는지 먼저 확인한다. 환경 ID 존재만으로 원격 서비스 준비/동작 성공을 판정하지 않는다. Secret 원문/로그는 출력하지 않는다.

2-4. 컴파일 조건별 online 활성화, 자동 Test/Editor 검증의 격리·권한/동의·timeout·late 응답 보호, v7/현재 Scoring Version과 기존 계정 전체 snapshot을 대조한다. Deprecated 코드의 이름만 보고 공통 Infinite 기반을 제거하지 않는다.

2-5. 결함이 없으면 코드/Scene을 변경하지 않는다. Scene 변경이 필요하면 `Hierarchy / 컴포넌트 / 필드 / 기존값→최종값 / onClick·Navigation / 저장·미리보기 원복` 적용표를 준비하고 Step 5에 전달한다. AI가 Scene을 편집하지 않는다.

### 정적 검증

`git diff --check`, meta/GUID·직렬화/asmdef·using/반사 대상·변수 선언/사용·async 반환을 대조한다. C# 소스 검사를 이름 해석/컴파일 성공으로 표현하지 않는다.

### 사용자 수동 작업

없음. 파일/GUID·Scene YAML·설정 조사 결과를 사용자 수동 점검표로 넘기지 않는다.

### 완료 조건

- [ ] 호출망/직렬화·환경/빌드 조건 감사와 Scene 변경 없음 또는 정확한 적용표를 기록했다.

## Step 3. 회귀 Test 공백만 보완한다

### AI 작업

3-1. 아래 기존 suite와 Step 1 필수 증거를 대조한다. 기존 검증으로 충분하면 Test를 중복 추가하지 않는다. 누락된 책임/버그를 실제 생산 코드로 재현하는 case만 추가하고 필요한 최소 코드 수정을 수행한다.

| 영역 | 기존 EditMode / PlayMode suite | 판정할 책임 |
| --- | --- | --- |
| 진입/탐색 | GameNavigationStateTests / GameEntryBootIntegrationTests | Infinite 기본 시작·과거/무효 선택 거절·가이드 완료 보존·출처별 복귀 |
| Pause/Result | GameNavigationStateTests, ResultTextFormatterTests / GamePauseOrchestrationTests, PauseMenuIntegrationTests, ResultMenuIntegrationTests | Pause Retry Cancel 기본값·동일 Run 보존·확인 시 1회 실행·Result 즉시 Retry |
| 화면/포커스 | LeaderboardViewStateTests / ModeUISceneConfigurationTests, ModeResultDisplayIntegrationTests, PausePanelSceneConfigurationTests, InfiniteHudIntegrationTests | 직렬화 UI·숨긴 Stage 포커스 차단·Infinite 표시·독립 조회 상태 |
| 생성/저장/전송 | RecordSubmissionPolicyTests, RecordLeaderboardPolicyTests, LocalSaveJsonCodecTests, OnlineRecordConfigurationTests, OnlineRecordRepositoryTests, OnlineLocalSaveScopeTests | 부작용 전 거절·원본/ID/귀속 보존·점수 상한·동일 ID/timeout·재시도/만료 |
| 계정/이전 | OnlineAccountCoordinatorTests, AccountTransferCompletionTests, AccountTransferControllerTests / AccountTransferUIIsolationTests, OnlineRecoveryNoticeFlowTests | Pending/동의/내구 저장 gate·전체 snapshot 교체·환경 분리·입력/복사/취소·지연 결과 |
| 도구/자동 격리 | VerificationSessionTests, CloudCodeModuleRoutingTests / VerificationToolIsolationTests, PlayModeRecordIsolationTests | 과거 probe 직접 거절·raw 전송 차단·실제 저장/UGS 호출 없음·권한/timeout |

3-2. 같은/인접 프레임·click+Submit 중복·Cancel/Back·응답 후 화면/계정 이동·저장 실패를 fake clock/TaskCompletionSource/메모리 store·카운터/상태 snapshot으로 검사한다. 시간이 빠른 사람의 조작이나 실제 서버 실패 주입으로 대체하지 않는다.

3-3. 생산 SampleScene/직렬화 컴포넌트·기존 격리 setup을 사용한다. 자동 Test의 production guard를 bypass하거나 UI를 동적 생성해 참조 누락을 숨기지 않는다. Stage 순수 formatter/내부 저장 Test를 일괄 삭제하지 않는다.

3-4. 예상 로그는 해당 거절/장애 Test에 정확한 LogAssert.Expect로 연결한다. 바뀐 suite·case·기대 결과/로그·수정 책임을 기록하고 Step 5의 재실행 필요성을 결정한다.

### 사용자 수동 작업

없음. Test 소스 준비와 Unity Test 실행을 구분한다.

### 완료 조건

- [ ] 기존 회귀/공백 표와 필요한 코드/Test 준비를 마쳤으며 자동 판정 항목을 수동 재현으로 넘기지 않았다.

## Step 4. 정적 검사와 오프라인 서비스 대역을 실행한다

### AI 작업

4-1. 실행 파일과 의존성의 SDK/VM 주입·파일/프로세스/네트워크 사용을 먼저 감사한다. 아래는 Phase 3에서 사용한 시작 목록이며 새 호출 경로가 생기면 영향에 맞춰 조정한다. 이름에 test가 있다는 이유로 `UGS/Tests` 전체나 live/probe 스크립트를 일괄 실행하지 않는다.

| 분류 | `UGS/Tests/` 실행 파일 |
| --- | --- |
| C#·Scene 소스 준비 | client-unity-preflight.cjs, client-local-save-contracts.test.cjs, client-account-contracts.test.cjs, client-transfer-completion-contracts.test.cjs, client-verification-tools-contracts.test.cjs, client-account-transfer-ui-contracts.test.cjs |
| 기존 정책/서버 소스 | static-contracts.cjs, phase1-policy-contracts.test.cjs |
| 제출/조회·snapshot 로컬 SDK 대역 | record-account-integration.test.cjs, account-personal-bests.test.cjs, cloud-code.test.cjs, leaderboard-query-policy.test.cjs |
| 수명/이전/경합·JSON 로컬 대역 | submission-retention-policy.test.cjs, transfer-completion.test.cjs, transfer-races.test.cjs, json-snapshot-consistency.test.cjs |

4-2. `node UGS/Tests/<파일>`로 감사한 스크립트를 실행해 파일·exit code·실제 제공된 건수/결과·검증 한계를 기록한다. 오래된 Stage 버튼 존재/성공을 요구하는 source assertion은 현행 차단 계약으로 수정하되 격리/민감 로그/권한 검사를 약화하지 않는다.

4-3. 실패 원인을 수정하고 변경 영향 검사만 재실행한다. 준비 case 집계는 Unity 실행 수가 아니며, 서비스 대역 성공은 원격 성공이 아니다. 실제 서비스 정책을 Test에 재구현해 같은 오류를 반복하지 않는다.

### 사용자 수동 작업

없음. Node 소스 검사/SDK 대역은 AI가 실행한다.

### 완료 조건

- [ ] 필요한 스크립트 결과/한계가 기록됐고 실패·선행 source 계약 불일치가 해결됐다.

## Step 5. 필요한 Unity Compile·Scene 적용·회귀 결과를 확보한다

### AI 선행 작업

5-1. Step 1~4의 실제 변경을 기준으로 Compile/Scene/Runner 갱신 여부를 결정한다. 코드/Test/Scene·관련 컴파일 조건 변경과 미해결 우려가 없으면 사용자 Phase 3의 Compile/1031·316 성공을 근거로 재사용하고 불필요한 Run All을 요구하지 않는다. production profile 전환으로 컴파일 조건이 달라지는 경우에도 영향과 기존 결과의 적용 범위를 구분한다.

5-2. 새 변경이 있으면 사용자가 실행할 assembly/suite·새 case/정확한 예상 로그와 Scene 적용표를 한 곳에 기록한다. Scene 변경이 없으면 `추가 Scene 작업 없음`을 명시한다.

### 필요한 경우의 사용자 작업

1. 최신 Script Compile 완료와 예상 밖 Error/Warning 유무를 보고한다.
2. AI가 지정한 Scene 변경이 있을 때만 정확한 적용표대로 적용/저장한다. AI는 저장 YAML을 다시 확인한다.
3. 공통 진입/기록/저장/계정에 새 변경이 있으면 `FlowState.EditModeTests`, `FlowState.PlayModeTests`를 필터 없이 각각 Run All한다. 좁은 수정의 후속 재검증은 AI가 지정한 영향 suite를 실행한다.
4. 통과/실패/미실행 수·실패 이름/메시지/Stack Trace·예상 밖 Error/Warning을 전달한다. AI는 실제 제공받은 보고/파일만 검토했다고 기록한다.

### 완료 조건

- [ ] 동일 조건의 기존 성공 근거를 재사용했거나, 필요한 최신 Compile/Scene·Runner 검증을 통과했다. 실행하지 않은 새 Test/미해결 로그가 없다.

## Step 6. 최신 Windows-production Player 후보를 빌드한다

### AI 선행 작업

6-1. Step 5 이후 소스/Scene/설정 변경 목록과 검증 결과를 고정한다. Windows-production의 FLOW_STATE_PRODUCTION·전역 Scene 목록·production 환경 분리·빌드 옵션을 소스로 재확인하고 후보와 연결할 파일 목록/해시·검증 시점을 기록한다. 과거 Candidate02가 최신이라는 가정을 하지 않는다.

6-2. 출력 경로는 기존 `Build/ProductionQA/Candidate02/`의 최신 후보 갱신 흐름을 기준으로 하되 사용자의 최신 경로 지시를 우선한다. 과거 실물 보존·새 폴더를 의무화하지 않으며 다른 QA 폴더/실제 저장을 삭제하지 않는다. AI가 빌드/출력 폴더 정리를 실행하지 않는다.

### 사용자 작업

1. Unity Build Profiles에서 `Windows-production`을 선택한다. 정적으로 이미 확인한 개별 GUID/환경 값을 수동으로 다시 조사하지 않는다.
2. 정한 출력 경로에 최신 Player를 빌드한다. 프로필 전환에 따른 Compile 실패/예상 밖 로그가 있으면 성공으로 진행하지 않고 전달한다.
3. `Profile / 출력 경로·실행 파일 / Build=성공·실패 / 예상 밖 Error·Warning / 실행 시점`을 보고한다. token/Secret/실제 저장 원문은 보내지 않는다.

### AI 결과 반영

받은 빌드 보고를 해당 소스/설정 기준선과 연결한다. 파일이 실제 제공되면 확인한 범위에서만 검사한다. 보고 이후 코드/Scene/설정이 바뀌면 기존 후보에 자동 승계하지 않는다.

### 완료 조건

- [ ] 최신 소스/설정과 연결되는 사용자 빌드 성공 근거·실행할 후보 경로가 확정됐다.

## Step 7. Player의 정상 흐름과 화면을 확인한다

### AI 선행 작업

7-1. Step 1의 자동 검증/공백 표에서 실제 Player가 필요한 항목만 아래 목록으로 확정한다. 수치/횟수·정밀 입력/경합의 PASS를 사람이 눈으로 대신 판정하게 하지 않는다.

### 사용자 작업

Step 6에서 확정한 Player를 정상 속도로 실행한다. 현재 기기 데이터/튜토리얼 상태를 존중하며 신규 사용자 조건을 위해 실제 저장을 삭제하지 않는다.

| 순서 | 정상 조작 | Player에서 확인/보고할 내용 | 자동 검증과의 분담 |
| --- | --- | --- | --- |
| 1 | Player 시작·Main Menu 확인 | 화면/글자/버튼이 표시되고 Stage/ModeSelect가 사용자 경로에 나타나지 않음 | 직접 API/과거 enum 거절은 Test/정적 감사 |
| 2 | 현재 튜토리얼 상태에 따른 Play/안내 진행 | Infinite 진입과 HUD/카메라/기본 입력의 정상 표현 | 신규/완료 상태 분기는 격리 Test |
| 3 | Pause → Resume, Pause → Retry 확인/취소 | 확인창 Cancel 기본 표현·취소 뒤 Pause 복귀·확인 뒤 정상 재시작 | 동일 Run/중복·인접 입력 횟수는 자동 Test |
| 4 | 정상 종료 → Result → Retry / Main Menu | Infinite 결과 구성·Retry 후 정상 HUD·메뉴 복귀·숨긴 Stage UI 없음 | 점수 계산/기록 ID/보존은 자동 Test |
| 5 | Main Menu/Result → Leaderboard → Back | 기존 Infinite 화면·공개 번호/상태 문구·가독성/포커스·출처별 복귀 | Top/Around/Me·late/Offline/Empty 분리는 Test, 실제 원격 성공은 Step 8 |
| 6 | Settings/HowToPlay 열기·닫기, 안전한 설정 유지 후 종료/재실행 | 잘림/배치/스크롤/선택 표시·닫기/재진입·기본 설정 보존 | 환경별 save/snapshot/손상 복구는 Test |

7-2. 실행 후보 경로·확인한 화면/입력 범위·정상/문제와 실제 로그 유무를 보고한다. 예상과 다르면 화면/상황·재현 가능한 정상 조작 순서와 로그를 전달한다. 이미지/로그 원문은 제공받은 것만 확인했다고 기록하며 개인정보/자격 증명은 제외한다.

### 완료 조건

- [ ] 최신 Player의 필수 화면/정상 흐름 보고가 있고 예상 밖 오류·화면/포커스 문제가 해결됐다. 빠른 입력/수치 판정은 자동 근거에 연결됐다.

## Step 8. 온라인·저장/이전의 증거와 실서비스 필요성을 정리한다

### AI 작업

8-1. Infinite 조회/제출·동일 ID·Pending/만료·이전/전체 snapshot·재시작·환경 분리의 각 근거를 `정적 / Unity fake Test / 서비스 SDK 대역 / 실제 Player·원격 보고`로 구분한다. 원격이 미실행이면 그대로 기록하고 로컬 성공을 원격 PASS로 바꾸지 않는다.

8-2. 이번 Client 전환에서 Module/서버 변경이 필요한지 실제 실패/계약 근거로 판정한다. 기존 불필요 판정을 뒤집을 근거가 없으면 변경/배포를 하지 않는다. 원격까지 필수로 주장할 검증 범위가 있거나 재현 가능한 원격 문제가 있으면 최소 대상/환경·작업·성공/중단 기준·자료 영향·복구 조건을 구체화한다.

8-3. 필요한 제한적 실제 확인은 사용자에게 정확한 기존 Infinite 조작만 안내한다. Stage 제출/과거 direct-write probe·대량 합성 제출/조사·원격 데이터 삭제를 요구하지 않는다. 실제 계정 이전은 연결을 바꾸므로 대상/자격 증명 처리·원본/recipient 영향과 기존 승인을 먼저 확인한다. 계획 문서 작성만으로 원격 전송/이전·배포 권한을 추가하지 않는다.

8-4. Module 변경이 실제 필요하면 성공 source/artifact/reference·QA 기준 보존, 호환성/시험·중단/복구 범위와 승인 경계를 준비한 뒤 별도 실행으로 다룬다. Prototype 8의 연기된 실제 복구 시험을 이 Task의 정적 검사로 완료 처리하지 않는다.

### 사용자 수동 작업

기본은 없음. 실제 확인이 필요하다고 확정된 경우에만 준비된 최소 범위를 수행한다. 필요한 원격 증거가 없는데 임의 N/A로 필수 조건을 통과시키지 않는다.

### 완료 조건

- [ ] 로컬/실제 원격 증거와 미검증 한계가 구분되고 서버 변경 여부가 판정됐다. 필수로 정한 실제 확인은 완료됐거나 미해결 필수 조건으로 명시됐다.

## Step 9. 발견한 결함을 수정하고 후보를 갱신한다

### AI 작업

9-1. Step 4~8의 실패/예상 밖 로그·화면 문제를 원인·수정·영향 Test·후보 버전·재확인 범위에 연결한다. 결함이 없으면 `추가 수정 없음`과 기존 결과를 기록한다.

9-2. 결함이 있으면 가능하면 먼저 실제 생산 코드/Scene에서 실패를 재현하는 회귀 Test를 준비하고 최소 수정한다. Scene은 적용표를 사용자에게 전달한다. 이전 승인을 전체 저장/계정 삭제나 다른 후보 변경에 확대하지 않는다.

9-3. 관련 정적/로컬 검사와 사용자 Compile/영향 suite를 통과한 뒤, Player에 반영해야 하는 변경이면 최신 후보를 다시 빌드해 해당 화면/흐름만 재확인한다. 새 변경/실패/미해결 우려가 없으면 무조건 전체 Test/Player 절차를 반복하지 않는다. 최종 소스와 후보의 불일치가 남으면 완료하지 않는다.

### 완료 조건

- [ ] 발견한 필수 실패/미실행·예상 밖 로그가 해결되고 최종 소스·자동 검증·최신 후보/보고가 일치한다. 결함 없음도 근거를 남겼다.

## Step 10. Phase 4와 Roadmap 9 완료를 판정한다

### AI 작업

10-1. 아래 완료 표를 Step별 실제 근거에 연결한다. 문서 계획/코드 준비/Node PASS만으로 Player 후보를 통과시키지 않는다.

| Roadmap 9 Phase 4 완료 조건 | 필요한 최종 근거 |
| --- | --- |
| 자동 검증과 사용자 실행 연결 | 최신 소스/설정·동일 조건의 유효 Compile/Runner·오프라인 결과·사용자 최신 빌드/Player 보고 |
| Stage 차단/Deprecated 보존·Infinite/공통 저장/계정 유지 | Phase 1~3 완료 근거·Step 2 호출망/직렬화·Step 3~5 회귀·Step 7 정상 Player 흐름·Step 8 증거 구분 |
| 후보 변경 범위·잔여 위험 기록, 불필요한 원격/삭제 없음 | 후보 경로/검증 시점·수정/재빌드 이력·미검증 원격/복구 의무·실제 서버/자료 변경 유무 |

10-2. 필수 결과/결함 해결이 모두 있으면 Phase 4와 Roadmap 9를 완료 처리하고 Roadmap/Feature/System/Project Memory를 갱신한다. 미해결 필수 조건이 있으면 해당 조건을 명시하고 미완료로 유지한다. 단순히 원격 미실행이라는 이유로 로컬/Player 결과를 지우거나, 반대로 미실행 원격 성공을 주장하지 않는다.

10-3. 최신 후보/환경·사용자 실행 근거·자동 검증·잔여 한계를 인계한다. 사용자 한정 QA 후보 판정을 일반 공개/Steam 출시 승인으로 확대하지 않는다. 보류된 Pattern/난이도 작업이나 Roadmap 11~14 기능을 섞지 않는다.

10-4. Roadmap 9가 실제 완료되면 다음 번호인 Roadmap 10의 thirdparty 원본 비추적·의존성/복원 체계 검토로 인계한다. 이 Task에서 Git 이력 재작성·원본/라이선스 삭제를 수행하지 않는다.

### 사용자 수동 작업

선행 필수 결과가 모두 있으면 없음. 같은 확인을 다시 요구하지 않는다.

### 완료 조건

- [ ] Phase 4/Roadmap 9 최종 판정·검증 범위/미검증 의무·최신 후보와 다음 Roadmap 인계를 정확히 기록했다.

# 영향 범위

현재 변경은 이 실행 절차 문서 작성과 Roadmap 9/Project Memory의 다음 작업 링크다. 기존 미커밋 생산 코드/Test·Phase 3 기록을 보존한다. 코드/Test/Scene·프로필·실제 저장/서버·후보 파일은 수정하지 않는다. Step별 실행 후 변경 파일/이유·검증 결과를 추가한다.

# 검증 내용

- Step 1~10의 순서·담당·정적/Unit Test·사용자 작업·완료 조건을 대조한다.
- Phase 3 완료를 재구현하거나 1031/316을 무조건 반복하지 않는지, 최신 후보가 필요한 근거와 조건부 재검증을 구분했는지 확인한다.
- 관련 문서/실제 suite·소스·Scene/프로필·Node 파일의 경로·상대 링크·날짜/작업 번호를 검사한다.
- 미착수 Step의 체크가 비어 있는지, 문서 작성 완료를 실행/원격/공개 승인으로 확대하지 않았는지 확인한다.
- 변경 문서의 공백/인코딩·Roadmap/Memory 역링크와 `git diff --check`를 확인한다.

# 검증 결과

문서 정적 검사로 Step 1~10 순서·완료 체크 0개/미체크 10개, 변경 문서 3개의 상대 링크 69개, 참조 Test suite 26개·Node 스크립트 16개·Scene/전역 빌드 설정/프로필 3경로의 존재를 확인했다. 문서 날짜/작업 번호와 선행 정책·AI/사용자 책임·Roadmap/Memory 역링크·공백 검사를 통과했다. 기존 문서의 `git diff --check`와 신규 Task의 공백/인코딩 검사도 통과했다.

이는 문서 작성의 검증 결과다. Phase 4의 로컬 스크립트·Unity Compile/Runner·빌드·Player·원격 작업은 아직 실행하지 않았다. 실제 실행 결과는 해당 Step에 후속 기록한다.

# 후속 작업

**Step 1 기준선/필수 증거 확정부터 시작한다.** 현재 Phase 4 Step 1~10은 미착수이며 사용자 빌드/Scene 적용·Runner 실행이나 원격 전송을 당장 시작할 필요가 없다.

# 관련 문서

- [Roadmap 9](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md)
- [공통 의존성과 승인/검증 경계](../../04_Implementation_Roadmap/README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [GameEntryNavigation](../../03_Features/GameEntryNavigation.md)
- [GamePause](../../03_Features/GamePause.md)
- [ResultMenu](../../03_Features/ResultMenu.md)
- [InfiniteMode](../../03_Features/InfiniteMode.md)
- [Leaderboard](../../03_Features/Leaderboard.md)
- [RecordSubmission](../../03_Features/RecordSubmission.md)
- [AccountTransfer](../../03_Features/AccountTransfer.md)
- [GameSystem](../../02_Systems/GameSystem.md)
- [UIManagementSystem](../../02_Systems/UIManagementSystem.md)
- [RecordSubmissionSystem](../../02_Systems/RecordSubmissionSystem.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)

# 관련 작업 기록

- [Phase 1 정책/완료 인계](20261111_02_Phase1ManualSteps.md)
- [Phase 2 완료·Candidate02 Player 기준선](20261112_01_Phase2ManualSteps.md)
- [Phase 3 완료·1031/316 최신 성공과 Phase 4 인계](20261117_01_Phase3ManualSteps.md)

# 작성 완료 기준

- 실제 작업을 Step 1~10과 하위 순서로 구분하고 정적/Unit Test·사용자 실행·완료 근거에 연결했다.
- 선행 완료/확정 범위·코드/Scene/원격/자료 보존 책임과 최신 후보 인계를 구분했다.
- 문서 작성만 완료했으며 실행 체크·Phase 4/Roadmap 9 완료를 미리 기록하지 않았다.
