# 작업 정보

## 작업명

Prototype 9 Phase 1 — Stage 제거 범위와 자료 호환 정책 확정 절차

## 작업 일자

20261111

## 작업 담당자

AI: 문서·코드 조사, 선택지 제시, 정적 검증, Unit Test 작성·로컬 대역 실행, 결정 사항 반영.
사용자: 사용자 흐름·자료 호환 정책 결정, 필요한 Unity 컴파일·Test Runner 실행 및 결과 전달.

## 작업 상태

Step 1 완료(2026-11-11). 1-1~1-5의 정적 경로 조사·기존 로컬 기준선 검증을 완료했다.
Step 2 완료(2026-11-11). 사용자가 권장안 전체와 Stage 소스의 Deprecated 보존·사유 주석 방침을 확정했다.
Step 3 완료(2026-11-11). 사용자 전제로 Stage 기록을 고려 범위에서 제외하고 별도 보관/이관 정책을 추가하지 않기로 했다.
Step 4 완료(2026-11-11). 구 Client 미지원·Infinite 공통 계약 유지·현재 서버 변경 불필요를 판정하고 로컬 대역 Test 3건을 추가해 검증했다.
Step 5 완료(2026-11-11). 공통/Stage 전용 경계·Phase별 변경 파일/Test와 사용자 Scene 적용 절차를 확정했다.
Step 6 완료(2026-11-11). P9-C01~10 계약과 후속 검증 경계를 고정하고 Infinite 서비스 대역 Test 6건을 추가·검증했다.
Step 7 완료(2026-11-11). Feature 8개·System 5개에 적용 예정 계약을 반영하고 Phase별 코드/Test·사용자 작업·서비스 복구 인계를 대조했다.
Step 8 및 Phase 1 완료(2026-11-11). 범위/정책·필요 로컬 검증·후속 인계를 최종 대조했다.
C# 생산 전환·Deprecated 적용·관련 실행 Test는 Phase 2~3에서 수행하며 Phase 2~4는 대기다. Phase 1 완료는 Stage 접근 제거 구현 완료를 뜻하지 않는다.

# 작업 목적

Roadmap 009 Phase 1을 실제로 수행할 수 있는 순서로 정리한다. 사용자가 해야 할 결정과 실행을 명시하고,
코드·저장·계약에서 확인할 수 있는 사항은 AI의 정적 검사와 자동 Test로 판정한다.

# 작업 대상

Stage Mode 플레이·진입·기록 접근의 제거 목록, InfiniteMode 공통 기반 보존 목록,
기존 Stage 개인 최고·Pending·서버 자료·계정 이전 및 구 Client 호환 정책.
생산 코드의 접근 차단과 UI 변경은 Phase 2~3, 통합 후보 판정은 Phase 4로 인계한다.

# 작업 전 상태

- Roadmap 8은 예외·잔여 한계를 포함해 완료됐고, 다음 대상은 Roadmap 9 Phase 1이다.
- Roadmap 9는 계획 확정·구현 미착수 상태다. 각 Phase는 대기이며 Stage 제거가 완료된 상태가 아니다.
- 기존 Feature는 Stage와 InfiniteMode를 모두 제공한다. 미래 정책과 현재 구현 사실을 구분해야 한다.

# 조사 내용

절차 작성 시 아래 근거를 확인했다. 이는 전체 경로 조사나 호환 검증의 완료 근거가 아니다.

| 확인한 근거 | 현재 확인한 사실 | Phase 1에서 확인할 영향 |
| --- | --- | --- |
| `Assets/Scripts/Runtime/Systems/GameSystem.cs` | `_selectedGameMode`의 초기값은 `E_GameMode.Stage` | Inspector·직접 실행·Retry를 포함한 진입 경로 |
| `Assets/Scripts/Runtime/Core/GameNavigationState.cs` | 선택 Mode와 대기 Mode의 초기값이 Stage | 메뉴·최초 안내·복귀·저장/직렬화 값 처리 |
| `Assets/Scripts/Runtime/Systems/StageSystem.cs` | 기본 Initialize는 Stage이며 Infinite 초기화·종료도 담당 | Stage 전용 Goal과 공통 생명주기 분리 |
| `Assets/Scripts/Runtime/Features/LocalSaveJsonCodec.cs` | Stage와 Infinite Board Key를 읽음 | 과거 자료를 버리지 않는 읽기·재저장 호환 |
| `Assets/Scripts/Runtime/Features/RecordSubmissionPolicy.cs` | Stage 후보 생성 함수가 존재 | 신규 후보 차단과 기존 Pending 처리의 분리 |
| `Assets/Scripts/Runtime/Features/AccountTransferPendingGate.cs` 및 `AccountTransfer.md` | 로컬 Pending과 계정 이전 요청이 연결됨 | 숨긴 Stage Pending이 이전을 영구 차단하는지 |
| `UGS/CloudCode/get-account-personal-bests.js` | Stage와 Infinite 보드를 처리함 | 구/새 Client와 서버 snapshot의 호환 |
| `UGS/Tests/account-personal-bests.test.cjs` | 두 보드의 snapshot과 서버 자료 불변 검사가 존재 | 기존 계약 유지 및 전환 계약의 테스트 배치 |

# 작업 내용

## 공통 진행 원칙

- 각 Step은 **AI 선행 조사·선택지 → 사용자 수동 작업 → AI 반영·검증** 순서로 수행한다.
- AI가 코드에서 확인 가능한 값이나 목록을 사용자에게 다시 조사하도록 요구하지 않는다. 사용자는 제시된 근거와 선택지에 답한다.
- 정책 선택지는 장단점·권장안·기존 Client/자료 영향을 함께 제시한다. 미응답 항목은 미정으로 남긴다.
- 정적 검사, Unit Test 준비, Unit Test 실행, 실제 Unity/서비스 검증을 구분해 기록한다. 소스에 Test가 있다는 이유로 PASS 처리하지 않는다.
- AI는 로컬 정적 검사와 메모리·서비스 대역 Test를 실행한다. Unity 컴파일·Test Runner·Build·Scene 적용·원격 게시 실행은 사용자가 담당한다.
- Phase 1의 기본 수동 작업은 정책 결정과 필요한 Edit Mode 실행이다. Scene 수정·Player 빌드·원격 호출을 기본 절차에 넣지 않는다.
- 실제 저장 파일·서버 계정·보드를 수정하거나 삭제해 시험하지 않는다. 합성 fixture와 메모리 저장소로 재현한다.
- 기존 자료 자동 삭제·보드 초기화·Infinite 기록 변환/병합은 범위 밖이다. 공통 클래스는 이름에 Stage가 있다는 이유로 삭제하지 않는다.
- Step 2 확정 이후 이 문서의 Stage 제거는 사용자 접근/실행 차단을 뜻한다. 관련 소스는 Deprecated와 사유 주석으로 보존하며, 기존 경로표도 이 기준으로 해석한다.
- Step 3 확정 이후 Stage 기록은 없고 모든 Client에서 추가 제출되지 않는다는 사용자 전제를 적용한다. 앞선 Stage 자료 호환/보관 조사·검증 제안은 현재 구현 요구보다 우선하지 않는다. 별도 Stage 보관/이관 기능은 추가하지 않는다.
- Roadmap 8의 검증 면제를 이번 작업에 자동 적용하지 않는다. Module 변경이 필요하면 기존 복구 시험 후속 의무를 별도로 인계한다.

## Step 1. 현재 경로와 검증 기준선을 조사한다

### AI 선행 작업

1. 관련 Feature/System 문서, 생산 코드, Editor 도구, Scene/Prefab 직렬화 자료와 Test를 읽기 전용으로 대조한다.
2. 아래 검색을 출발점으로 호출자·이벤트 연결·기본값·Inspector 값까지 추적한다. 문자열 검색 결과만으로 경로 전체를 조사했다고 판단하지 않는다.

```powershell
rg -n 'E_GameMode.Stage|StartStage|TryCreateStageCandidate|ModeSelect|StageLeaderboard' Assets/Scripts Assets/Editor Assets/Tests
rg -n 'Stage|Infinite|_selectedGameMode|m_MethodName|guid:' Assets/Scenes Assets/Prefabs ProjectSettings/EditorBuildSettings.asset
rg -n 'stage|Stage|pending|personalBests' UGS/CloudCode UGS/Modules UGS/Tests
```

3. 각 경로에 `파일/심볼 또는 직렬화 위치 → 호출자 → 사용자 도달 가능 여부 → 제거/내부 호환/공통 유지 → 후속 Phase → 검증 방법`을 기록한다.
4. 직접 Editor 실행, 검증 도구, Test 전용 진입을 생산 경로와 구분한다. 구 저장값이 실제로 존재하는지 확인하고, 존재하지 않는 저장 경로를 사실로 만들지 않는다.
5. 기존 검사 파일의 실행 범위를 읽어 원격 호출이 없는 아래 로컬 기준선을 실행하고 종료 코드·실행 수·실패 내용을 기록한다. Node가 없으면 상태만 보고하고 설치를 임의로 진행하지 않는다.

```powershell
node UGS/Tests/static-contracts.cjs
node UGS/Tests/cloud-code.test.cjs
node UGS/Tests/account-personal-bests.test.cjs
node UGS/Tests/client-unity-preflight.cjs
```

### 사용자 수동 작업

기본적으로 없음. 저장소로 확인할 수 없는 실제 배포 Client 지원 현황이 정책 결정을 막을 때만,
AI가 질문을 한 번에 정리하면 현재 배포/사용 중인 Client 버전과 유지할 지원 범위를 알려 준다.
인증 정보·개인 저장 원문·Secret을 전달하지 않는다.

### 완료 조건

- [x] 경로 목록과 기존 Test 대응표가 작성됐고, 미확인 경로·기준선 실패가 분리돼 있다.

### Step 1 수행 결과 — 2026-11-11

조사량을 아래 1-1~1-5로 나누어 모두 수행했다. 이 결과의 제거/보존 분류는 후속 정책 결정을 위한
대상 식별이다. Step 2~4에서 결정할 전환 방식이나 구 Client 지원 종료를 확정한 것은 아니다.

#### 1-1. 생산 메뉴·직접 실행·Retry 경로 조사 — 완료

아래 `Core/`, `Systems/`, `Features/`는 `Assets/Scripts/Runtime/` 기준이다.
Test 파일은 별도 표시가 없으면 `Assets/Tests/EditMode/` 기준이며, 기존 Test는 존재·책임을 대조했을 뿐 Unity에서 실행하지 않았다.

| 경로와 근거 | 호출·사용자 도달 여부 | 분류 / 후속 Phase | 검증 연결 |
| --- | --- | --- | --- |
| `Systems/GameSystem.cs:263` SelectPlay/SelectStage → RequestNavigationSelection → HandleModeSelectSelection → StartRequestedRun | Main Menu Play → Mode Select Stage, Keyboard/Mouse 생산 경로 | 사용자 Stage 진입 제거 / Phase 2 | `GameNavigationStateTests.cs`, PlayMode `GameEntryBootIntegrationTests.cs` |
| `Core/GameNavigationState.cs:341` SubmitModeSelect → `_pendingGameMode` → SubmitAutomaticHowToPlay | 최초 안내 중 선택 Mode를 보존한 뒤 StartRun으로 시작 | 사용자 Stage 대기 요청 제거 / Phase 2 | `GameNavigationStateTests.cs`, PlayMode `HowToPlayIntegrationTests.cs` |
| `Systems/GameSystem.cs:160` StartGame → BeginRunFromMenu(`_selectedGameMode`) 또는 BeginRunFromResult | public API 및 ContextMenu Start Game. Inspector/default Mode 사용; 일반 메뉴와 별도 진입 | 개발/Editor 우회도 차단 대상 / Phase 2 | PlayMode `GameLifecycleIntegrationTests.cs`; 기본값 정적 검사 |
| `Systems/GameSystem.cs:745` RetryGame → Pause 재시작 또는 BeginRunFromResult → StartRequestedRun | Pause Retry·Result Retry·public RetryGame 모두 현재 Mode 재사용 | Stage 재시작 제거, Infinite Retry 유지 / Phase 2 | PlayMode `PauseMenuIntegrationTests.cs`, `ResultMenuIntegrationTests.cs` |
| `Systems/RuntimeDataSystem.cs:15` CreateRuntimeData(), `Core/GameRuntimeData.cs:30` Initialize(), `Systems/StageSystem.cs:84` Initialize() | 무인자 기본값 Stage. 생성/초기화 API이며 단독 호출이 완성된 Run 시작을 뜻하지는 않음 | 공통 API의 Stage 기본/직접 요청 처리 결정 / Phase 2 | `GameRuntimeDataTests.cs`, PlayMode `StageSystemTests.cs` |
| `Systems/GameSystem.cs:560` StartRequestedRun → Runtime·Stage·Infinite·입력·Timer·Camera 초기화, StageEnded 구독 | 모든 Run이 사용하는 공통 실행 기반 | 공통 유지, Stage 선택 차단 경계 확인 / Phase 2·4 | PlayMode `InfiniteModeIntegrationTests.cs`, `GameLifecycleIntegrationTests.cs` |

`GameNavigationState`의 마지막 선택/대기 Mode는 Runtime이다. `GameEntryNavigation.md`의
Application 종료 후 저장하지 않는 규칙과 일치한다. `LocalSaveJsonCodec.SaveFile`에는 선택 Mode 필드가 없고,
`GameSystem.SaveLocalState`는 Settings·Tutorial 완료 및 기록 영역을 저장한다.
따라서 현재 확인된 과거 Mode 값은 Scene의 직렬화 값과 같은 실행 중 Navigation 상태이며,
선택 Mode가 Local Save에 저장된다는 전제의 마이그레이션은 만들지 않는다.
단, 기록 후보의 `gameMode` 필드는 실제 영구 저장 자료이므로 1-3에서 별도로 다룬다.

#### 1-2. Scene·Prefab·UI·Editor 도구의 연결 조사 — 완료

Scene/Prefab YAML을 읽기 전용으로 조사했다. Scene 로드·변경·저장은 수행하지 않았다.
`ProjectSettings/EditorBuildSettings.asset`의 enabled Scene은 `Assets/Scenes/SampleScene.unity` 한 개다.
이 파일의 Unity 객체 블록을 fileID로 연결하고 대상 컴포넌트의 script GUID를 `.cs.meta`와 대조했다.

| 직렬화/도구 근거 | 정적으로 확인한 연결·접근 | 분류 / 후속 Phase | 검증 연결 |
| --- | --- | --- | --- |
| SampleScene `:16608`, GameSystem fileID `1202804280` | `_selectedGameMode: 1`; 현재 enum은 Stage=0, Infinite=1. 생산 Scene 값은 Infinite이나 코드 기본값과 메뉴 최초 선택은 Stage | 기본값·메뉴·직접 실행을 함께 정리 / Phase 2 | enum/Inspector 값 정적 대조, PlayMode `GameEntryBootIntegrationTests.cs` |
| SampleScene `:10534`, `:17660` | Mode Select Button `761562235`, Leaderboard Button `1242588979`의 SelectStage는 모두 GameSystem `1202804280`을 호출 | 플레이/기록 사용자 경로 제거 / Phase 2·3 | UnityEvent 대상·GUID 정적 검사, PlayMode `ModeUISceneConfigurationTests.cs` |
| SampleScene `:9818`, `:20379`, `:29388`, `:32012` | StartRun `677628985`, Retry `1363044890`/`1861140804`, ResultLeaderboard `2048432086`도 같은 GameSystem을 호출 | 최초 안내·Pause/Result 우회 경로 포함 / Phase 2·3 | PlayMode `HowToPlayIntegrationTests.cs`, `ResultMenuIntegrationTests.cs` |
| SampleScene `:25547`, `:30492`, `:30503` | StageSystem Goal·양쪽 Mode Root 참조와 UIManagementSystem의 Mode Select/Leaderboard Stage Button 참조가 존재 | Stage 전용 표시/Goal 정리, 공통 참조 보존 / Phase 2·3 | 직렬화 참조 검사, PlayMode `ModeResultDisplayIntegrationTests.cs` |
| SampleScene `:23590` 부근 How To Play Text | Stage를 빨리 끝내라는 안내와 Infinite 연속 Momentum 설명을 함께 포함 | Infinite 기준 안내로 정리 / Phase 2 | 문자열/Binding 참조 정적 검사, PlayMode `HowToPlayIntegrationTests.cs` |
| `Systems/UIManagementSystem.cs` HasNavigationReferences·GetNavigationButton·SetResultData/표시 분기 | Stage Button·HUD·Result Content와 최고 기록 표시가 연결됨. 버튼만 지우면 필수 참조 검사도 영향을 받음 | UI 코드/Scene 참조 계약 함께 변경 / Phase 2·3 | `UIVisibilityStateTests.cs`, `ResultTextFormatterTests.cs`, PlayMode UI 구성 Test |
| `Assets/Editor/OnlineRecordVerificationWindow.cs:187`, `:477`, `:765` | Stage 조회·Mode toggle·격리 Stage 후보 생성/전송 probe가 존재. 일반 Player 메뉴와 별도인 Editor 검증 경로이며 실제 온라인 호출 가능 | 사용자/개발용 Stage 접근과 합성 내부 호환 probe의 구분 필요 / Phase 2·3·4 | `VerificationSessionTests.cs`, PlayMode `VerificationToolIsolationTests.cs`; 도구 정적 계약 |
| `Assets/Tests/PlayMode/ProductionSceneGameModeTestUtility.cs` RestartInMode | 생산 Scene의 public Navigation 호출로 Stage/Infinite 실행을 요청하는 Test 전용 helper | 금지 경로 거절 Test와 남길 공통/호환 Test로 재분류 / Phase 2·4 | helper 호출자 및 PlayMode Mode Test 정적 대조 |

현재 직렬화 값과 enum은 일치한다. 다만 Scene의 Infinite 값만으로 Stage 접근이 차단된 상태는 아니다.
Test helper는 `Assets/Tests` 전용이며 일반 Player UI로 분류하지 않았다.
Prefab 검색에서 발견하는 Infinite Pattern/Collectible 참조는 이름만으로 Stage 전용 삭제 목록에 넣지 않았다.

#### 1-3. 기록·Pending·계정 이전·서버 호환 경로 조사 — 완료

| 경로와 근거 | 호출·현재 동작 / 사용자 접근 | 분류 / 후속 Phase | 검증 연결 |
| --- | --- | --- | --- |
| `GameSystem.TryStoreCurrentResultCandidate` → `RecordSubmissionPolicy.TryCreateStageCandidate` → `RecordSubmissionService.TryStoreCandidate` | Stage Cleared Result가 로컬 최고·Pending 후보가 됨 | 신규 Stage 생성 차단 / Phase 3 | `RecordSubmissionPolicyTests.cs`, `OnlineRecordRepositoryTests.cs` |
| `GameSystem.HandleLeaderboardSelection`, QueryLeaderboardAsync → CloudCodeRecordRepository GetTop/GetAround | Stage Tab 또는 직접 Repository 호출로 Stage Board 조회 가능; LeaderboardViewState 기본값도 Stage | 사용자 탭/직접 조회 경계 차단, 서버 내부 호환과 분리 / Phase 3 | `LeaderboardViewStateTests.cs`, `OnlineRecordRepositoryTests.cs` |
| `GameSystem.HandleResultSelection`, RefreshResultOnlineBestAsync → GetPersonalBest | 실제 Result Mode의 순위 링크·Online best 조회; Stage 최고 기록 표시 연결 | Stage 표시/링크 제거 / Phase 3 | `RecordLeaderboardPolicyTests.cs`, PlayMode `ModeResultDisplayIntegrationTests.cs`, `ResultMenuIntegrationTests.cs` |
| `Features/LocalSaveJsonCodec.cs` CandidateFile·TryCreateBoardKey, `LocalSaveData.CurrentVersion=7` | 후보의 Mode 정수(0/1)·Stage ID·Version·값·제출 ID·시각을 보존; active/inactive 환경 영역에도 최고/Pending 존재 | 구 자료 읽기/재저장 호환 유지 대상 / Phase 3 | `LocalSaveJsonCodecTests.cs`, `OnlineLocalSaveScopeTests.cs` |
| `GameSystem.InitializeLocalState/Update/RetryOnlineRecordsAsync` → OnlineRecordCoordinator | 앱 초기화·온라인 복구·동의/인증·수동 Retry로 복구 진입. 현재 Mode 필터 없이 같은 계정의 Pending 처리 | 구 Stage Pending 전송 정책은 Step 3 결정 / Phase 3 | `OnlineRecordRepositoryTests.cs`, PlayMode `LocalPersistenceIntegrationTests.cs` |
| `OnlineRecordCoordinator.ProcessPendingAsync` → LocalRecordRepository TryExpirePending/TryRemovePending | 180일 만료 정리, Submitted/Rejected 확정 뒤 제거; 3회 retry. 자동 복구는 남은 실패 후보에서 멈출 수 있고 RetryAll은 다음 후보도 순회 | 만료/terminal 삭제와 전송 종료를 별도 결정 / Phase 3 | `OnlineRecordRepositoryTests.cs`, Node `submission-retention-policy.test.cjs` |
| `LocalRecordRepository.TryBeginTransferRequest`, `AccountTransferPendingGate` → OnlineAccountCoordinator RequestAsync | Mode와 무관하게 Pending가 한 건이라도 있으면 이전 요청 gate 차단 | 숨긴 Stage-only/혼합 큐가 이전을 막는 문제 결정 / Phase 3 | `AccountTransferControllerTests.cs`, `AccountTransferCompletionTests.cs` |
| `AccountTransferController.RequestDiscard/ConfirmDiscard` → TryDiscardPending | 사용자 확인 후 현재 환경 Pending 전체를 폐기하는 조작이 존재 | Stage 자료 보존/조작 제거 정책과 대조 필요 / Phase 3 | `AccountTransferControllerTests.cs`, PlayMode `AccountTransferUIIsolationTests.cs` |
| `OnlineAccountCoordinator.FinishCompletionAsync/TryReadSnapshot` → LocalRecordRepository의 계정/최고 교체 | 최대 2보드 snapshot을 읽고 Stage도 해석. 잘못된/중복 보드는 전체 실패. 이전 후 전체 캐시 교체로 계정 귀속을 맞춤 | Stage 해석을 삭제하면 유효 구 snapshot 전체 실패 위험 / Phase 3 | `AccountTransferCompletionTests.cs`, `OnlineAccountCoordinatorTests.cs` |
| `OnlineRecordConfiguration/RecordBoardKey`, `UGS/CloudCode/submit-record.js`, `query-records.js` | `fs-stage-stage-001-r1`, `fs-infinite-v2`를 구분. Stage 서버 제출·조회 계약이 남아 있음 | 구 Client 지원 정책은 Step 4 결정; 무조건 서버 제거하지 않음 / Phase 3·4 | Node `cloud-code.test.cjs`, `record-account-integration.test.cjs` |
| `UGS/CloudCode/get-account-personal-bests.js` | 계정 전체 Stage/Infinite snapshot; pending/reservation·연결 변경 시 부분 성공으로 덮어쓰지 않음 | 내부 읽기 호환 후보 / Phase 3 | Node `account-personal-bests.test.cjs` |
| `UGS/Modules/FlowStateVerification/FlowStateVerification.csproj` EmbeddedResource와 Module endpoint wrapper | CloudCode JS를 Module에 포함하여 실행함. JS 변경만으로 원격 Module이 바뀌지는 않음 | Module 필요 여부는 Step 4; 게시/복구는 별도 인계 / Phase 4 | Module routing/embedded source 정적 대조; 현재 원격 동일성은 미판정 |

이번에는 저장 필드·호출 경계의 존재를 조사했다. Stage 포함 저장 round-trip과 전환 후 후보 차단의 새 Unit Test 실행은
Step 6 및 Phase 2~3의 책임이다. 기존 Stage 저장 파서를 없애는 변경이나 enum 재번호 부여는 호환 위험이 있다.

#### 1-4. 공통 기반과 기존 Test 영향 조사 — 완료

StageSystem의 StartStage/PauseStage/ResumeStage/StageEnded/TryEndInfiniteStage는 Infinite도 사용한다.
InfiniteModeSystem은 StageSystem을 참조하며 진행 지속 실패·추락 시 TryEndInfiniteStage를 호출한다.
GameSystem은 공통 Timer·이동·카메라·Collectible을 초기화하고 종료 시 Result 및 제출로 이어 준다.
따라서 StageSystem·RuntimeDataSystem·TimerSystem·ResultSystem 전체를 삭제할 근거는 없다.
Stage 전용 Goal/Cleared·Clear Time UI와 Infinite의 Run 시간·최종 점수는 구분해야 한다.

| 기존 Test 묶음 | 유지/변경 시 확인할 책임 | 후속 시점 |
| --- | --- | --- |
| EditMode `GameNavigationStateTests`, `GameRuntimeDataTests`, `UIVisibilityStateTests` | Stage 기본/선택 기대를 목표 정책으로 변경; Infinite와 중복 요청/복귀 보존 | Phase 2 |
| PlayMode `StageSystemTests`, `StageGoalIntegrationTests`, `ModeUISceneConfigurationTests`, `ModeResultDisplayIntegrationTests` | Stage 생산 실행 성공 Test를 그대로 유지하지 않음. 금지 경로 거절·공통 생명주기·내부 호환 Test로 목적을 분류 | Phase 2·4 |
| PlayMode `InfiniteModeIntegrationTests`, `InfiniteHudIntegrationTests`, `PauseMenuIntegrationTests`, `ResultMenuIntegrationTests` | Infinite 시작·Pause·종료·Retry·HUD/최종 Score 회귀 | Phase 2·4 |
| EditMode `LocalSaveJsonCodecTests`, `OnlineLocalSaveScopeTests`, `OnlineRecordRepositoryTests` | 구 Stage/혼합 자료 보존, 환경 격리, 신규 Stage 차단과 구 Pending 처리 분리 | Phase 3 |
| EditMode `AccountTransferCompletionTests`, `AccountTransferControllerTests`, `OnlineAccountCoordinatorTests` | 이전 gate·폐기·계정 전체 snapshot·과거 응답 무효화 | Phase 3 |
| Node `cloud-code.test.cjs`, `account-personal-bests.test.cjs` 및 관련 계정/receipt Test | Stage 포함 구 계약과 Infinite 제출·조회가 정책별로 유지되는지 | Phase 3·4 |

Test를 현재 상태에서 삭제하거나 기대값만 바꾸지 않았다. Pattern 연결·물리·점수 계산 변경은 조사 후속 범위에 포함하지 않는다.

#### 1-5. 로컬 정적·서비스 대역 기준선 실행 — 완료

Node v24.21.0에서 아래 네 명령을 실행했다. 검사 파일과 의존 대역을 읽어 로컬 파일 검사/SDK 대역 사용을 확인했다.
`client-unity-preflight.cjs`의 child process는 변경 파일 목록을 얻는 git 호출이며 Unity/컴파일러 실행이 아니다.

| 실행 명령 | 종료 코드 / 결과 | 의미·한계 |
| --- | --- | --- |
| `node UGS/Tests/static-contracts.cjs` | 0 / PASS 출력 8개 | 식별자·정책·권한·CAS·Module 의존성 등 기존 정적 계약. Stage 차단 완료 판정은 아님 |
| `node UGS/Tests/cloud-code.test.cjs` | 0 / 22/22 PASS | 실제 제출/조회 서비스 함수와 SDK 대역. Stage/Infinite 검증·재시도·중복·보존·조회 경계 |
| `node UGS/Tests/account-personal-bests.test.cjs` | 0 / 15/15 PASS | 두 보드/빈 snapshot·계정 이전·stale 응답·행/ledger 보존 계약 |
| `node UGS/Tests/client-unity-preflight.cjs` | 0 / PASS | 변경 C# 0개, asmdef·격리 설정 등 정적 검사. EditMode 183건/PlayMode 23건은 일부 지정 Test의 소스 준비 수이며 미실행 |

기준선 실패는 없다. 새 전환 정책의 Unit Test를 추가하거나 Unity Test Runner를 실행한 결과는 아니다.

### Step 1의 미확인 사항과 수동 작업

- 현재 로컬 저장소에서 식별한 경로의 조사와 기준선 검증은 완료했다. 생산 실행에서 Stage 차단이 완료됐다고 판정하지 않는다.
- 조사 당시 실제 구 Client 지원 의사는 미확인이었다. 이후 Step 4에서 사용자가 구 Client 미지원을 확정했으므로 구 버전 현황 재조사를 요구하지 않는다.
- 원격 Module의 현재 artifact 동일성·실제 프로젝트 권한·실서비스 자료 및 Player 동작은 이번 정적 조사로 확인하지 않았다. 후속 서비스 변경 필요성 판단/통합 후보 검증과 구분한다.
- 현재 Step 1에 필요한 사용자 수동 작업은 없다. Unity Build·Test Runner·Inspector 재확인·Scene 수정·원격 probe를 요구하지 않는다.
- Scene 작업이 필요한 Phase 2~3에는 AI가 해당 참조/UI의 적용 방법을 제공하고 사용자가 Editor에서 반영한다. 이번에는 변경 절차를 실행하지 않는다.

## Step 2. Stage 제거 후 사용자 흐름을 결정한다

### AI 선행 작업

Step 1 목록을 이용해 Main Menu 시작, Mode Select 처리, 최초 How To Play, Settings 안내,
Pause·종료·Result·Retry·Leaderboard 복귀 흐름을 제안한다. 직접 Stage 시작 요청과 과거 선택값에는
Infinite로 정규화할지 요청을 거절할지 경계별 대안을 제시한다. Stage가 실행되는 폴백은 허용하지 않는다.

### 사용자 수동 작업

1. 제안된 시작 흐름을 보고 `시작 → Infinite` 또는 필요한 다른 흐름을 선택한다.
2. Mode Select 제거 여부, 최초 안내와 관련 문구, Result·Leaderboard의 복귀 흐름을 확인한다.
3. 직접 Stage 요청·과거 Stage 선택의 처리 대안과 사용자 안내 유무를 결정한다.

답변 형식: `시작 흐름=…, Mode Select=…, 과거 선택/직접 요청=…, 안내=…, 복귀 흐름=…`.

### AI 반영·검증

정적 경로표에 기본값·Retry·안내 중 대기 Mode·직접 API·Editor 우회를 연결한다.
Step 6의 전환표 Test 입력과 기대 결과로 바꾼다. 실제 메뉴·Scene 변경은 Phase 2에 인계한다.

### 완료 조건

- [x] Stage를 시작/재시작할 수 없는 목표 흐름과 Infinite 시작·Pause·종료·Retry 보존 조건이 확정됐다.

### Step 2 수행 현황 — 2026-11-11

#### 2-1. 현재 동작과 변경 경계 대조 — 완료

- Step 1 경로표와 `GameNavigationState.SubmitMainMenu/SubmitModeSelect/SubmitAutomaticHowToPlay/SubmitPause/SubmitResult/ReturnFromLeaderboard`,
  `GameSystem.BeginRunFromMenu/StartRequestedRun/RetryGame`을 대조했다.
- 현재 Play는 Mode Select를 열며, 최초 Stage 선택과 자동 안내의 대기 Mode가 Stage 시작으로 이어질 수 있다.
- Tutorial 완료는 `GameSystem.InitializeLocalState`에서 복원하고 `SaveLocalState`로 보존한다.
  기존 완료 사용자를 다시 안내하려면 별도 정책 변경이 필요하다. 권장안은 기존 완료 상태를 유지한다.
- 자동 안내 Cancel 무동작, Main Menu How To Play에서 Back/Cancel 복귀, Pause의 Settings 복귀,
  Main Menu 이탈 확인, Result의 Cancel 무동작은 기존 규칙이다. 권장안은 이 계약을 유지한다.
- Mode 선택은 Local Save에 저장되지 않는다. 과거 값 처리는 Scene Inspector와 실행 중 선택/대기 상태에 한정한다.
  후보에 저장된 `gameMode`는 기록 호환 자료이며 이 Step의 시작 Mode 정규화 대상에 포함하지 않는다.

#### 2-2. 사용자 흐름 선택 — 완료

사용자가 2026-11-11 아래 권장안 전체를 선택했다. 권장안 열이 확정 정책이며 대안 열은 비교 이력이다.

| 결정 항목 | 권장안 | 대안과 차이 |
| --- | --- | --- |
| 시작 | Boot → Main Menu 유지. Play → Infinite 시작 요청. Mode Select 제거 | Infinite/Back 선택 화면을 유지할 수 있으나 하나의 Mode를 선택하는 단계가 추가됨 |
| 최초 안내 | Tutorial 미완료이면 Play → Infinite 안내 → Start Run. 완료이면 바로 시작. 기존 완료 상태 유지 | 완료 상태 초기화 후 재안내는 기존 사용자에게 추가 절차가 생기므로 권장하지 않음 |
| 안내 내용 | 점프·관성 착지·Collectible·거리/점수·Pause 및 Infinite 종료 조건을 기존 규칙대로 설명. Stage Goal·Clear Time 설명 제거. Main Menu How To Play 유지 | 화면 디자인 변경은 이 Step에서 확정하지 않음 |
| 과거 Mode 값 | Inspector/Runtime 선택·대기 Mode의 Stage 값은 생산 Run 시작 전 Infinite로 정규화 | 거절만 하면 이전 설정으로 정상 Play가 막힐 수 있음 |
| 명시적 Stage 실행 | public API·Stage 선택 요청·Editor 실행 도구의 명시적 Stage 실행은 거절. Run/요청/화면·현재 실행 상태를 변경하지 않음 | 요청을 Infinite로 바꾸면 호출자가 요청하지 않은 Run이 시작될 수 있음 |
| 전환 안내 | 일반 사용자용 Stage 폐지 팝업 없음. 개발용 명시적 Stage 거절은 필요한 경우 안전한 진단만 제공 | 팝업을 추가하면 매끄러운 Play 흐름에 새 확인 단계가 생김 |
| Pause/Retry/종료 | Infinite Pause/Resume·Pause Retry·Result Retry 유지. Retry는 새 Run이며 Timer·입력·점수·Result를 초기화. Pause Main Menu 확인·Result Cancel 무동작 유지 | 구 Stage Pending 제출 Retry 정책은 Step 3에서 결정 |
| 복귀/순위 | Main Menu 복귀 시 Play 선택. Leaderboard는 Infinite만 표시. Back/Cancel은 진입한 Main Menu의 Leaderboard 또는 Result의 순위 보기로 복귀 | 기록 전송/서버 호환은 Step 3~4 범위 |

명시적 Stage 요청과 과거 기본값 복원은 서로 다른 입력 경계다. `StartGame()`처럼 Mode 인자가 없는
실행 진입은 과거 Inspector 값을 정규화하는 대상으로 제안하며, `Initialize(Stage)` 등 명시적 생산 실행 요청은 거절 대상으로 제안한다.
저장 codec·snapshot의 Stage 읽기와 순수 합성 호환 Test는 생산 Stage 실행 요청으로 취급하지 않는다.
Editor 도구의 Stage 조회·제출 probe 처리는 Step 3~5에서 별도로 결정하고, 이 Step에서는 Stage 플레이 실행의 우회 차단을 다룬다.

사용자 추가 결정: Stage 관련 소스 코드는 삭제하지 않고 Deprecated로 설정하며, 비권장 이유를 적절한 주석으로 설명한다.
사용자 경로의 제거는 UI 노출·실행·조회 접근을 차단하는 뜻이다. Stage 구현 소스의 물리 삭제를 뜻하지 않는다.

후속 구현 기준:

- Stage 전용 API·분기·표시 코드·식별자는 남기고 비권장 상태와 사유를 명시한다. 생산 실행/접근 차단은 별도로 구현하며 Deprecated 표시만으로 차단됐다고 판정하지 않는다.
- C#에서 독립된 Stage 전용 선언에 적용 가능한 `Obsolete` 등 표기와 분기/필드의 주석 범위는 Phase 2~3에서 호출 관계·직렬화·필수 참조를 대조해 정한다. 기존 enum 정수값·GUID·직렬화 계약을 보존한다.
- 주석은 Infinite 단일화로 신규 Stage 사용을 중단한다는 이유, 기존 자료 호환을 위해 남긴다는 이유, 사용할 Infinite 경로 또는 허용된 내부 읽기 범위를 설명한다. JS에도 Stage 전용 코드의 비권장 이유와 호환 목적을 주석으로 남긴다.
- Infinite가 사용하는 StageSystem 등 공통 기반 전체를 Deprecated로 표시하지 않는다. Stage 전용 책임과 구 자료 호환 책임을 구분한다.
- 남겨야 할 저장 codec/snapshot 파서의 내부 Stage 읽기는 보존한다. Deprecated 호출로 발생하는 예상 경고의 처리 범위는 국소적으로 검토하며, 전역 경고 억제로 새 사용이나 실제 문제를 숨기지 않는다.
- Stage 소스를 남긴다는 결정이 구 Client 서비스 유지/종료, 기존 Pending 전송/폐기/만료를 확정한 것은 아니다. 이는 Step 3~4에서 결정한다.

#### 2-3. 목표 전환표와 Unit Test 인계 준비 — 완료

| 입력/선행 상태 | 확정 정책의 기대 결과 | 검증 위치/시점 |
| --- | --- | --- |
| Boot 완료 | Main Menu/Play, Run 없음 | `GameNavigationStateTests`, Phase 2 |
| Play, Tutorial 미완료 | Infinite 자동 안내, Run 생성 전; Start Run으로 Infinite 한 번 시작 | `GameNavigationStateTests`, PlayMode `HowToPlayIntegrationTests`, Phase 2 |
| Play, Tutorial 완료 | Mode Select 없이 Infinite 한 번 시작; 완료값 유지 | `GameNavigationStateTests`, PlayMode `GameEntryBootIntegrationTests`, Phase 2 |
| 자동 안내 Cancel/Start Run 중복 입력 | Cancel 무동작; 대기 Mode는 Infinite; 시작 요청 중복 없음 | `GameNavigationStateTests`, Phase 2 |
| Main Menu How To Play 열기/닫기 | Run 없이 Main Menu How To Play 선택으로 복귀, 자동 안내 완료값 변경 없음 | `GameNavigationStateTests`, Phase 2 |
| Inspector/Runtime에 Stage 선택·대기 값이 남음 | 정상 시작 경계에서 Infinite로 정규화, enum 저장값 재번호 부여 없음 | 기본값 정적 검사·`GameRuntimeDataTests`와 Navigation Test, Phase 2 |
| 명시적 Stage 선택/생산 초기화 요청, 준비 중/Playing/Paused/Result 상태 | Stage Run 생성/재시작 거절, 기존 Run·화면·선택·요청 수 보존 | 각 실제 생산 API의 EditMode/PlayMode Test, Phase 2 |
| Infinite Pause→Settings→Back/Cancel | Pause로 복귀하고 기존 Run 유지; Resume으로 재개 | `GameNavigationStateTests`, PlayMode `PauseMenuIntegrationTests`, Phase 2 |
| Infinite Pause Retry·Result Retry | Infinite 새 Run 1개, 이전 Timer/점수/입력/Result 미승계 | PlayMode `ResultMenuIntegrationTests`, `GameLifecycleIntegrationTests`, Phase 2 |
| Pause Main Menu 취소/확정, Result Main Menu | 취소 시 Pause, 확정 시 Run 정리 후 Main Menu/Play; Result Cancel은 무동작 | `GameNavigationStateTests`, PlayMode `PauseMenuIntegrationTests`, Phase 2 |
| Main Menu/Result에서 순위 진입 후 Back/Cancel | Infinite Board만 사용, 진입 출처와 선택 보존; 새 Run 없음 | `LeaderboardViewStateTests`, Navigation/Result 통합 Test, Phase 3 |
| 제출 Retry | Run 시작/Run ID 갱신 없음. 구 Stage Pending 대상·건수는 Step 3 결정 후 고정 | `OnlineRecordRepositoryTests` 및 Result 통합 Test, Phase 3 |

정적 검증에서는 Step 1의 메뉴·기본값·대기 Mode·직접 API·Editor·Retry 경로가 위 전환표 중 어디에 해당하는지 대조한다.
실제 전환 기능은 Phase 2~3에서 구현한다. Step 6은 확정 정책과 재사용 가능한 검증 경계를 고정하고,
생산 구현에 연결되지 않은 Test 전용 전환 로직을 만들어 PASS 처리하지 않는다. 현재 전환용 Unit Test는 작성/실행하지 않았다.

Deprecated 소스 보존에 대한 후속 검증도 추가한다. Phase 2~3에서 Stage 전용 선언/분기 목록의 코드 잔존·비권장 표기·사유 주석,
enum/직렬화 값 불변을 정적으로 검사하고, 실제 생산 API의 Stage 실행/조회 차단과 Infinite 보존은 Unit/통합 Test로 검증한다.
소스가 남아 있다는 사실이나 `Obsolete` 존재만으로 접근 차단 Test를 대체하지 않는다.

#### 2-4. 확정 반영과 완료 판정 — 완료

사용자 답변을 이 Task에 확정했고, Roadmap/Project Memory의 다음 작업을 Step 3으로 갱신한다.
이 Step에서는 현재 Feature 동작을 이미 변경된 것으로 고쳐 쓰지 않는다. Phase 2~3 인계에 위 전환표와 실제 적용 파일을 연결한다.
실제 Scene 적용은 Phase 2에서 사용자에게 오브젝트·참조·버튼 연결 변경 방법을 제공하고 사용자가 수행한다.

사용자 정책 선택을 받아 Step 2 전체를 완료 처리했다. 현재 추가 수동 작업은 없다.
빌드·Test Runner·Scene/Inspector 조작은 필요하지 않다. Stage Deprecated 표기·주석·차단의 실제 소스 수정은 Phase 2~3 인계이며 미수행이다.

답변 수신 전 정적 검증 결과: Node 읽기 전용 검사로 2-1~2-4 순서·정책 답변 대기 표시·Step 1만 완료 체크된 상태,
상대 링크·참조 Test 파일·Navigation 함수 존재를 확인했다. `git diff --check`와 새 Task 파일의 공백 검사를 통과했다.
`Assets`·`UGS`·`ProjectSettings`의 tracked 변경은 없다. Unity 컴파일·Test Runner·Build·Scene 적용·원격 요청은 수행하지 않았다.

사용자 결정 반영 후 Node 읽기 전용 검사로 Step 1~2만 완료 체크, 2-1~2-4 완료 상태, 상대 링크,
확정 정책·Deprecated 보존 인계와 다음 Step 3을 확인했다. Task/Roadmap/Project Memory의 상태를 대조했고 공백 검사를 통과했다.
생산 소스/Scene 변경은 없으며 새 Unit Test·Unity 실행 결과를 추가한 것으로 기록하지 않는다.

## Step 3. 기존 Stage Pending의 수명과 계정 이전 정책을 결정한다

### AI 선행 작업

1. 후보 생성, 앱 시작·온라인 복구·인증 성공·사용자 Retry, terminal 응답, 180일 만료,
   Pending 건수 표시와 계정 이전 gate를 하나의 처리표로 대조한다.
2. 구 Stage Pending의 내부 재전송을 유지하는 방안과 보존하되 전송을 종료하는 방안을 비교한다.
   자료 삭제와 사용자 기록 조작 기능 없이 각 방안이 가능한지, 서비스 변경이 필요한지 명시한다.
3. 계정 이전에 대해 Stage-only·Infinite-only·혼합 Pending을 각각 제시한다.
   전송을 중단한 Stage Pending이 이전을 영구 차단하는 방안은 해결책과 함께 제시해야 한다.
4. 현재 180일 자동 만료 계약과 Roadmap 9의 자료 자동 삭제 금지 사이의 적용 범위를 명확히 제안한다.
   숨김·처리 종료·건수 제외·실제 삭제는 서로 다른 조치로 기록한다.

### 사용자 수동 작업

1. 기존 Stage Pending을 내부 재전송할지, 자료를 보존하고 전송을 종료할지 선택한다.
2. 앱 시작/복구/수동 Retry의 대상, 화면 건수·안내, terminal 응답/만료/저장 실패 때의 보존 규칙을 확인한다.
3. Stage-only·혼합 Pending이 있는 기기의 이전 시작과 이전 완료 처리안을 선택한다.
4. 기존 Pending의 사용자 폐기 기능이 Stage 기록 조작 제거 범위와 충돌하는지 확인하고 목표 정책을 결정한다.

답변 형식: `구 Stage Pending 전송=…, 보존/만료=…, Retry·건수·안내=…, 이전 시작/완료=…, 폐기 기능=…`.

### AI 반영·검증

후보 ID·귀속·생성 시각·payload 불변, 중복 응답, 재시작, 다른 계정/환경,
만료 직전·정확한 경계·직후를 Step 6 Test에 연결한다. 정책 충돌이 남으면 이 Step은 완료하지 않는다.
신규 Stage 후보 생성 차단의 생산 구현은 Phase 3에 인계한다.

### 완료 조건

- [x] 사용자 전제로 Stage 기록 처리 범위를 제외했다. Infinite의 기존 만료·이전 정책 유지와 별도 Stage 보관/이관 미추가를 확정했다.

### Step 3 수행 현황 — 2026-11-11

#### 3-1. 현재 수명·화면·이전 gate 대조 — 완료

| 경계 | 현재 근거/동작 | 필요한 변경 판단 |
| --- | --- | --- |
| 큐 생성 | `RecordSubmissionPolicy.TryCreateStageCandidate` → `RecordSubmissionService` → `LocalRecordRepository.TryEnqueuePending` | 신규 Stage 후보 차단은 Phase 3; 기존 자료 읽기는 보존 |
| 로드/시각 | `LocalSaveJsonCodec.ReadPendingCandidates/CopyWithCreatedAt` 및 LocalRecordRepository의 로드 | 기존 시각 누락 보충과 새 Stage 보존 정책의 적용 순서 결정 필요 |
| 자동/수동 재전송 | `OnlineRecordCoordinator.RetryPendingAsync/RetrySubmissionAsync/RetryAllPendingAsync` → ProcessPendingAsync | 같은 귀속 후보를 처리하며 현재 Mode 필터 없음. 현재 Run Retry와 저장 큐 전체 Retry를 구분 |
| 만료 | `LocalRecordRepository.TryExpirePending` | 현재 환경 큐의 모든 Mode에 180일 규칙 적용. 저장 성공 전 메모리 제거 안 함 |
| 확정 응답 | `OnlineRecordCoordinator.ProcessPendingAsync` → TryRemovePending | Submitted/Rejected만 저장 후 큐 제거. 실패/응답 유실은 원본 ID로 재시도 |
| 사용자 건수 | `GameSystem.PendingOnlineRecordCount`·Result/Leaderboard 표시, `AccountTransferController.Render/Can` | 모두 공통 Pending snapshot에 의존. UI 버튼만 숨겨도 건수/gate는 남음 |
| 명시적 폐기 | `AccountTransferController.ConfirmDiscard` → LocalRecordRepository.TryDiscardPending | 현재 환경 전체 Pending을 저장 후 제거. Stage를 유지하려면 필터/보존 경계 변경 필요 |
| 이전 시작/완료 | `AccountTransferPendingGate`·LocalRecordRepository.TryBeginTransferRequest/TryBeginAccountTransition/TryApplyAccountTransition | Pending가 한 건이라도 있으면 차단. 원본 보관과 큐 제외를 안전하게 연결해야 함 |
| 이전 후 갱신 | `OnlineAccountCoordinator.FinishCompletionAsync` 및 계정/최고 캐시 교체 | 보관 Stage가 새 계정 귀속/새 서버 제출로 재생성되지 않게 별도 보존 필요 |

`Assets/Scripts/Runtime/Features/`의 위 클래스와 기존 Feature의 180일/명시적 폐기/이전 규칙을 대조했다.
현재 Stage 자료 자동 삭제 금지와 모든 Mode에 적용하는 기존 만료/폐기 계약의 충돌을 식별했다.
이 Step에서는 기존 생산 동작을 바꾸지 않았다.

#### 3-2. 기존 Stage Pending 처리 방안 비교 — 완료

| 방안 | 이점 | 제약/후속 작업 |
| --- | --- | --- |
| A. 내부 재전송 유지 | 미제출 Stage 기록을 서버에 반영할 기회가 남음. 기존 재시도 흐름 일부 재사용 | Stage 쓰기 서비스 지원에 의존. Stage 실패가 Infinite 복구를 막지 않게 분리해야 하며 terminal/만료 시 원본 보존과 이전 gate 정책이 별도로 필요 |
| B. 전송 종료·원본 별도 보관(권장) | 새 Client의 Stage 기록 조작을 중단하고, 자료 보존과 Infinite 전송/이전을 분리 | 로컬 보관 영역·이관·저장 실패 복구·기존 Client downgrade/백업 복원 영향 설계가 필요 |

단순히 Stage를 숨기거나 전송만 끄고 공통 큐에 두는 방식은 무기한 이전 차단이 생길 수 있어 권장하지 않는다.
보관은 자료 삭제와 다르며, 활성 큐에서 제외하는 것은 서버 제출 성공을 뜻하지 않는다.
이 비교는 사용자 결정 전 조사 이력이다. 이후 3-3의 범위 조정으로 A/B 모두 채택하지 않았으며 보관 스키마를 설계/구현하지 않는다.

#### 3-3. 사용자 정책 선택 — 완료

사용자는 제안 A/B 대신 다음 전제로 작업 범위를 변경했다.

1. Stage 관련 기록은 고려 사항에서 제외한다.
2. 모든 Client에서 앞으로 Stage 기록이 제출되지 않는다고 전제한다.
3. 구 Client에도 Stage 기록이 없다고 전제한다.

이는 사용자 확정 작업 전제다. 실제 기기/원격 자료를 조사해 Stage 기록 부재를 입증한 결과는 아니다.
앞선 3-2의 A/B 비교와 Stage 원본 별도 보관 권장안은 채택하지 않았다.

확정 범위:

- Stage Pending의 별도 보관 영역·영구 보관·이관·재전송·전용 만료/폐기 정책은 추가하지 않는다.
- Stage-only/혼합 큐·구 Stage 응답 경합·과거 Stage 백업 복원은 새 개발/검증 요구에서 제외한다.
- 이전 gate를 Stage 보관 정책 때문에 개편하지 않는다. Infinite Pending의 기존 0건 조건, 명시적 폐기, 저장 실패 시 차단을 유지한다.
- Infinite의 앱 시작/인증/온라인 복구/수동 Retry, 동일 ID 재시도, terminal 확정 후 정리, 180일 만료와 환경/귀속 격리를 유지한다.
- 새 Client의 신규 Stage 후보 생성·제출 접근은 Phase 3에서 차단한다. 구 Client의 추가 제출 중단은 사용자 전제로 적용하며, 원격 차단을 실제 구현/검증했다고 기록하지 않는다.
- Step 2의 Stage 소스 Deprecated 보존·사유 주석 방침은 유지한다. 기록이 없다는 전제만으로 기존 소스/파서를 삭제하거나 enum 값을 바꾸지 않는다.
- 이 결정은 저장/서버 자료·보드 삭제나 초기화 작업 지시가 아니다. 해당 조작은 수행하지 않는다.

Step 4도 같은 전제를 적용한다. Stage 자료 보관·이관이나 구 Stage 제출 지원 여부를 다시 선택하도록 요청하지 않는다.
Infinite 조회·제출·계정 이전과 공통 저장/서버 계약의 변경 필요 여부를 확인하는 범위로 정리한다.

#### 3-4. 정책별 정적 검사·Unit Test 인계 — 완료

| 검증 대상 | 확정한 기대 결과 | 검증 위치/시점 |
| --- | --- | --- |
| 신규 Stage 후보·제출 진입 | 생산 경로에서 후보 생성/원격 제출 없음. 남은 소스는 Deprecated와 사유 주석으로 보존 | 정적 호출 경로, `RecordSubmissionPolicyTests`, `OnlineRecordRepositoryTests`; Phase 3 |
| Infinite 자동 복구/수동 Retry/현재 Result Retry | 기존 시도 횟수·동일 ID·중복 방지·대상 선택 유지 | `OnlineRecordRepositoryTests`; Phase 3 회귀 |
| Infinite terminal·만료 | 확정 응답 뒤 내구 저장 성공 시 정리, 180일 직전/경계/직후, 실패 시 원본 보존 | `OnlineLocalSaveScopeTests`, 기존 Node 수명 정책 Test |
| Infinite Pending·계정 이전 | 1건 이상이면 이전 차단. 0건이며 저장 성공이면 기존 절차 적용. 명시적 폐기 취소/실패 시 보존 | `AccountTransferControllerTests`, `AccountTransferCompletionTests`, `OnlineLocalSaveScopeTests` |
| 귀속/환경·재시작·계정 이전 | 다른 귀속/환경으로 제출 안 함, Settings/Tutorial 및 기존 저장 실패 보호 유지 | `OnlineAccountCoordinatorTests`, `LocalSaveJsonCodecTests`, `OnlineLocalSaveScopeTests` |

위 C# Test는 `Assets/Tests/EditMode/`의 기존 검증 출발점이다. 현재 실제 전환 코드는 변경하지 않았고 새로운 C# Test도 작성/실행하지 않았다.
Step 6/Phase 3에서 실제 생산 코드의 변경 경계에 연결한다. Stage 보관 기능과 보관 성공 후 이전 허용 Test는 개발 대상에서 제외한다.

#### 3-5. 기존 계약의 로컬 검증 — 완료

| 실행 명령 | 결과 | 한계 |
| --- | --- | --- |
| `node UGS/Tests/submission-retention-policy.test.cjs` | 종료 0, PASS. terminal replay/만료/정리 후 ID 처리의 기존 경계 검증 | 기존 순수 정책 Test이며 새 Stage 보관 기능은 미검증 |
| `node UGS/Tests/transfer-client-state-policy.test.cjs` | 종료 0, PASS. 기존 Pending gate·source reset·계정 최고 교체 계약 | 보관 Stage를 제외하는 새 gate의 PASS가 아님 |
| `node UGS/Tests/client-local-save-contracts.test.cjs` | 종료 0, PASS. v7 환경 격리·시각 보충·폐기/이전 gate 정적 계약 | 출력의 EditMode 27건은 소스 준비 수이며 미실행 |

검사 파일은 순수 정책 함수 또는 로컬 소스 읽기만 사용한다. 원격 서비스·실제 사용자 저장을 호출/변경하지 않았다.

#### 3-6. 확정 반영/완료 판정 — 완료

사용자 전제로 Stage 기록 처리를 범위에서 제외했고 3-3/3-4와 후속 범위를 갱신했다.
이 범위 조정으로 기존 완료 조건을 충족한 것으로 판정하고 Step 3을 완료 처리한다.
현재 추가 수동 작업은 없다. Unity Build·Test Runner·Scene/Inspector·Dashboard 작업은 필요하지 않다.

3-5는 기존 계약 검사 실행 이력이다. Stage 보관 기능이나 실제 모든 Client의 제출 중단을 검증한 결과가 아니다.
Node 읽기 전용 검사로 문서 링크·3-1~3-6 순서·Step 1~3 완료 체크·사용자 전제와 보관안 미채택·다음 Step 4 연결을 확인했다.
diff 공백 검사를 통과했고 `Assets`·`UGS`·`ProjectSettings`의 tracked 변경은 없다. 문서 변경만 수행했으며 추가 Unit Test 실행은 필요하지 않았다.

## Step 4. Infinite·공통 계약과 Client 지원 범위를 확정한다

### 적용 전제와 완료 조건

사용자 지시: 구 Client는 지원하지 않는다. Step 3의 Stage 기록 부재·모든 Client의 추가 Stage 제출 없음 전제를 적용한다.
Stage 보관/이관·Stage-only/혼합 fixture나 구 Stage 제출 지원을 새 요구로 추가하지 않는다.

- [x] 구/새 Client 지원 범위와 Infinite 저장·snapshot·계정 이전 계약, 필요한 서비스 변경 유무를 확인했다.

### Step 4 수행 결과 — 2026-11-11

#### 4-1. 지원 범위와 전제 확정 — 완료

| 조합 | 정책/검증 범위 |
| --- | --- |
| 구 Client + 기존 서버 | 미지원. 동작 보장·호환 수정·구 버전용 회귀/실서비스 검증을 하지 않음 |
| 전환 후보 새 Client + 기존 서버 | 목표 지원 조합. Infinite와 공통 계약을 유지하고 Phase 2~3 생산 변경 후 Phase 4에서 후보 검증 |
| 구 Client + 변경 서버 | 미지원. 구 Client 지원만을 위한 서버 변경을 추가하지 않음 |
| 전환 후보 새 Client + 변경 서버 | 현재 기능 전환에 서버 계약 변경 필요성을 발견하지 못했으므로 신규 배포를 기본 작업으로 넣지 않음. 후속 구현에서 필요가 확인되면 별도 범위로 검토 |

여기서 구 Client는 Roadmap 9 전환 이전 Client, 새 Client는 이후 전환 후보를 뜻한다. 서버에 전달하는 새 버전 번호나 별도 저장 스키마를 만든 것은 아니다.
구 Client 미지원은 지원 정책이며 실제 서비스 접근의 강제 차단 완료를 뜻하지 않는다.
현재 `OnlineRecordRequest`와 서버 검증에는 Client build/version에 따른 차단 계약이 없다.
이번 요청에 버전 검증·강제 업데이트 화면·원격 접근 차단/계정 변경을 추가하지 않는다.
Stage 제출이 더 이상 없다는 사용자 전제를 코드/원격 검증 결과로 바꾸지 않는다.

#### 4-2. 저장·Board·요청/응답 계약 대조 — 완료

아래 C# 경로는 `Assets/Scripts/Runtime/Features/`, 서버 경로는 `UGS/CloudCode/` 기준이다.

| 계약 | 확인한 실제 근거 | 전환 판단/후속 작업 |
| --- | --- | --- |
| 로컬 저장 | `LocalSaveData.CurrentVersion=7`, `LocalSaveJsonCodec`의 환경별 최고·Pending·Settings/Tutorial 직렬화 | 새 Stage 보관 영역/Version 추가 불필요. Infinite 기존 필드·설정·환경 격리 유지 |
| Mode/Board | `OnlineRecordConfiguration.TryGetLeaderboardId`, `RecordBoardKey`; Infinite는 `fs-infinite-v2`, Rules/Scoring Version 2 | ID·enum 정수값·점수 규칙 유지. Stage 신규 접근 차단과 Deprecated는 Phase 3에서 적용 |
| 제출 | `CloudCodeRecordRepository.SubmitAsync` → `submit-record.js`/v2 경로 | 기존 submissionId·점수 구성·Run 시간·인증 귀속 계약 유지. 서버 API 입력 변경 불필요 |
| 조회 | 같은 Repository의 GetTop/GetAround/GetPersonalBest → `query-records.js` | 기존 top 10/around 7/me와 공개 번호·응답/실패 형식 유지. 사용자 Board 선택만 Infinite로 제한하는 것은 Client 변경 |
| 계정 snapshot | `OnlinePersonalBestSnapshot`, `get-account-personal-bests.js` | 빈 배열 또는 Infinite 한 행을 기존 응답으로 처리 가능. Stage 필드를 없애려고 서버 응답 스키마를 바꾸지 않음 |
| snapshot 검증 | `OnlineAccountCoordinator.TryReadSnapshot` | Success·유효 공개 번호·비음수 정수 score/UUID·중복 Board/잘못된 응답 검사 유지. 읽기 오류를 빈 성공으로 대체하지 않음 |
| 계정 이전 | `OnlineAccountCoordinator.FinishCompletionAsync` → `LocalRecordRepository.TryApplyAccountTransition` | 현재 연결 계정 전체 최고로 교체, Pending 0건·내구 저장·generation/binding 검사 유지. 이전 기기/기존 계정 최고를 합산/병합하지 않음 |
| 서버 저장 | 계정/고정 Leaderboard owner·ledger·terminal receipt | 재귀속·리셋·자료 삭제/보드 초기화 불필요. 기존 서버 형식 유지 |
| 실행 배포 | `UgsOnlineRecordTransport`, Module wrapper와 EmbeddedResource | 기존 endpoint/Module 라우팅 유지. 이번 Step에서 Module 빌드/게시 불필요 |

Stage 관련 소스/파서는 Step 2 지시대로 보존하며 실제 Deprecated 표기·사유 주석을 Phase 2~3에서 적용한다.
Stage 문자열이나 분기를 지우기 위한 서버 재배포를 요구하지 않는다. 소스 주석/Module 후보를 실제로 바꾸어 게시할 때에는 기존 복구 인계 의무를 적용한다.
구 Client 지원을 종료해도 새 Client의 Infinite 기록·계정·Settings를 초기화할 이유는 없다.

#### 4-3. Infinite 전용 로컬 대역 Unit Test 보강 — 완료

`UGS/Tests/account-personal-bests.test.cjs`에 아래 3건을 추가했다.
모두 실제 snapshot/계정 이전 서비스 함수와 기존 SDK 메모리 대역을 사용하며 새 정책을 Test 내부에 재구현하지 않는다.

1. Infinite-only v1 ledger: 최대 유효 점수·ID를 정확히 반환하고 ledger/쓰기 수가 불변인지 확인.
2. Infinite-only v2 ledger: 기존 v2 구조에서 같은 Infinite snapshot 읽기가 가능하고 자료를 쓰지 않는지 확인.
3. Infinite-only 계정 이전: recipient의 이전 계정에 더 높은 점수가 있어도 연결 계정의 점수/공개 번호/owner만 읽는지 확인.
   이전 source 접근은 거절하며, snapshot 읽기가 양쪽 ledger나 Leaderboard 행을 다시 쓰지 않는지 확인.

기존 Stage 합성 Test는 삭제하지 않았다. 해당 기존 테스트의 잔존은 구 Client 지원이나 Stage 기록 호환 개발을 다시 추가한다는 뜻이 아니다.
현재 생산 C#/서버 서비스 코드는 수정하지 않았다. C# 실행과 실제 서비스의 원자성/장애 복구를 Node 결과로 입증하지 않는다.

#### 4-4. 정적 검사·로컬 Test 실행 — 완료

| 실행 명령 | 결과 | 확인 범위 |
| --- | --- | --- |
| `node UGS/Tests/account-personal-bests.test.cjs` | 종료 0 / 18/18 PASS | 신규 Infinite-only 3건과 기존 snapshot/계정 전환 회귀 |
| `node UGS/Tests/cloud-code.test.cjs` | 종료 0 / 22/22 PASS | 실제 제출·조회 서비스와 SDK 대역; Infinite 점수 상한/구성·Version 등 기존 계약 |
| `node UGS/Tests/client-transfer-completion-contracts.test.cjs` | 종료 0 / PASS | C# 전체 최고 교체·저장 순서·세대/연결·SDK API 정적 계약. EditMode 30건은 준비 수이며 미실행 |
| `node UGS/Tests/client-local-save-contracts.test.cjs` | 종료 0 / PASS | v7 저장·환경/귀속·시각·폐기/gate 정적 계약. EditMode 27건은 준비 수이며 미실행 |

위 검사는 실제 인증·원격 UGS·사용자 저장 파일을 사용하지 않았다. Unity 컴파일·Test Runner·Player/Module 빌드·Scene 적용은 수행하지 않았다.
정적 대조와 로컬 대역 결과로 현재 목표 계약에 서버 변경이 필수라는 근거는 없다.
새 Client의 실제 생산 동작 성공은 Phase 2~4 구현/검증 이후 판정한다.

문서 정적 검사도 통과했다. 상대 링크·4-1~4-5 순서·Step 1~4만 완료 체크·다음 Step 5 연결·검사 파일 존재를 확인했다.
`git diff --check`와 새 Task 공백 검사를 통과했고, 문서 외 변경은 위 SDK 대역 Test 파일 한 개뿐임을 확인했다.

#### 4-5. 인계·완료 판정 — 완료

- 구 Client 미지원과 Step 3 전제는 확정됐고 추가 사용자 정책 선택은 없다.
- Phase 3은 Infinite 요청/응답·저장 계약을 보존하며 Stage 사용자 조회·표시·제출 진입만 차단한다. 소스는 Deprecated·주석으로 남긴다.
- Phase 4는 전환 후보 새 Client와 현재 서버를 대상으로 검증한다. 구 Client 검증·Stage 자료 조사/보관·서버 재배포는 기본 절차에서 제외한다.
- Step 6은 확정 정책과 실제 생산 코드 검증 경계를 연결한다. 새 Client의 변경 후 EditMode/PlayMode 실행 목록을 준비하되 실행은 사용자 담당이다.
- 현재 필요한 수동 작업은 없다. Dashboard/기기 원본 조사·Scene 조작·Unity Build/Test Runner를 요구하지 않는다.

Step 4를 완료 처리하고 다음은 Step 5 공통 기반과 후속 변경 목록 확정으로 진행한다.
Phase 1 전체와 실제 전환 구현은 아직 완료하지 않았다.

## Step 5. 공통 기반과 후속 변경 목록을 확정한다

### 적용 기준과 완료 조건

Step 2~4의 사용자 확정을 그대로 적용한다. Stage 소스는 Deprecated·사유 주석으로 보존하고 사용자 접근/명시적 실행은 차단한다.
Stage 기록 부재·추가 제출 없음 전제로 별도 자료 보관/이관은 제외하며 구 Client는 미지원이다.
이번 Step은 후속 변경 목록을 확정하는 작업이다. 생산 코드·Test 기대값·Scene을 실제 전환하지 않는다.

- [x] 접근 차단/Deprecated 보존/공통 유지 목록과 Phase 2~4의 구현·검증 인계를 연결했다.

### Step 5 수행 결과 — 2026-11-11

#### 5-1. 공통 실행 의존성 확정 — 완료

다음 연결을 생산 코드와 System 문서에서 대조했다.

| 연결 | 공통 유지 이유 | Stage 전용 경계 |
| --- | --- | --- |
| GameSystem → RuntimeDataSystem/GameRuntimeData → StageSystem/InfiniteModeSystem | Run 생성·초기화·Ready/Playing과 중복 시작 방지에 공통 사용 | 무인자 Stage 기본값과 명시적 Stage 인자 처리 변경 |
| GameSystem → StageSystem.StartStage/PauseStage/ResumeStage/StopStage, StageEnded 구독/해제 | Infinite 생명주기와 Pause rollback·종료/비활성 정리에 사용 | Goal/Cleared·Stage 추락 분기와 Stage 실행 요청은 비권장·차단 |
| InfiniteModeSystem → StageSystem.TryEndInfiniteStage/IsPlaying/InfiniteMapPattern | Infinite 지속 실패·추락 종료, Difficulty/Pattern 및 World Rebase에 실제 사용 | StageSystem 전체를 Deprecated/삭제하지 않음 |
| StageSystem.ConfigureCollectibles/BindCollectibles → InfiniteMapPattern | Infinite Slot의 Collectible scope/재바인딩과 겹침 재검사에 사용 | Stage 고정 배치 활성화는 실행되지 않게 함 |
| GameSystem → TimerSystem → ResultSystem/CreateInfiniteResultData | Infinite Run 시간·제출 상한·Pause 제외 시간과 최종 결과 확정에 필요 | Stage Clear Time 표시/TimeRecord 전용 흐름은 비권장·생산 접근 차단 |
| PlayerController/Movement·Collision·Camera/CameraFollow·WorldRebaseState | Infinite 이동·착지·카메라 추적과 좌표 보정 유지 | Stage 이름만으로 이동/카메라를 분리하거나 계산식을 변경하지 않음 |
| Score/Momentum·Collectible·Infinite Pattern 네 원본 | Scoring Version 2·Difficulty/연결 규칙 유지 | Pattern/물리/점수 규칙 변경 없음 |

`StageSystem.ApplyModeRootState`는 현재 양쪽 Root 중 하나만 null이면 실패한다.
따라서 StageModeRoot 참조만 제거하는 Scene 변경은 하지 않는다. 후속 코드는 Infinite 경계와 비권장 Root 처리를 함께 검토한다.
`UIManagementSystem.HasNavigationReferences`도 현재 Mode Select의 Stage/Infinite/Back을 필수로 검사하므로,
버튼 비활성/참조 보존과 무관하게 새 메뉴 계약에 맞는 필수 검사·선택/Navigation 목록을 함께 수정해야 한다.

#### 5-2. 생산 파일별 차단·Deprecated·유지 범위 확정 — 완료

아래 `Core/`, `Features/`, `Systems/`는 `Assets/Scripts/Runtime/` 기준이다.
`E_GameMode.Stage|TryCreateStageCandidate|TryCreateStage(` 검색으로 발견한 생산/Editor 파일 18개를 모두 아래 표에 분류했다.
이는 전체 의존성을 그 문자열 검색만으로 판정했다는 뜻은 아니다. 5-1과 Scene/도구 호출 연결도 함께 대조했다.

| 대상 파일 | 확정할 후속 변경 | Phase |
| --- | --- | --- |
| `Core/GameNavigationState.cs` | Mode Select 우회·Infinite 기본/대기 값·Stage 선택 거절·출처별 복귀/Retry 유지 | 2 |
| `Core/GameRuntimeData.cs`, `Systems/RuntimeDataSystem.cs` | 무인자 기본을 Infinite로, 명시적 Stage 생산 생성/초기화 거절. 직렬화 값/공통 데이터 보존 | 2 |
| `Systems/GameSystem.cs` | 메뉴/ContextMenu/Retry의 Infinite 시작 보장, 명시적 Stage 우회 차단; Stage Result/기록 후보·조회 연결은 남기되 생산 경로 차단 | 2·3 |
| `Systems/StageSystem.cs` | Infinite 생명주기·Root·Collectible 유지, Stage 명시적 초기화/실행 거절. Goal·추락·Cleared 전용 부분 Deprecated/주석 | 2 |
| `Features/InfiniteModeState.cs` | 내부 Stage 초기/reset 값의 실제 사용 경계 검토. 초기화 전 비활성 상태와 생산 Infinite 전환을 혼동하지 않으며 공통 상태 규칙 유지 | 2 |
| `Core/ResultData.cs`, `Features/ResultTextFormatter.cs` | Stage 전용 생성/문구는 보존·비권장. Infinite 결과/점수·Run 시간 규칙 유지 | 2·3 |
| `Core/UIVisibilityState.cs`, `Systems/UIManagementSystem.cs` | Stage HUD/Result/Mode Select/Stage Tab 노출·포커스 경로 차단, 공통 Pause/Settings/Result/Infinite HUD 유지 | 2·3 |
| `Features/LeaderboardViewState.cs` | Infinite 기본/선택만 생산 사용, Stage 직접 선택/늦은 응답 경계 차단 | 3 |
| `Features/RecordSubmissionPolicy.cs` | 신규 Stage 후보 생성 거절, Infinite 생성·검증 유지. Stage 소스 비권장 표기/사유 | 3 |
| `Features/RecordBoardKey.cs`, `Features/OnlineRecordConfiguration.cs`, `Features/RecordLeaderboardPolicy.cs` | Stage 식별자/규칙 소스 보존·비권장 경계 설명. 실제 실행/조회 차단과 내부 값 표현은 구분 | 3 |
| `Features/LocalSaveJsonCodec.cs`, `Features/OnlineAccountCoordinator.cs` | 기존 파서/공통 저장·snapshot 유지. Stage 기록을 위한 새 보관/이관·지원 정책을 추가하지 않음. Stage 전용 부분에 사유 주석 | 3 |
| `Assets/Editor/OnlineRecordVerificationWindow.cs` | Stage toggle·조회·제출·Stage direct-write probe 진입 비활성/거절; 원격 호출 전 guard. Infinite 검증 유지 | 2·3 |

추가 연결 파일도 다음과 같이 인계한다.

- `Core/E_GameMode.cs`: Stage enum 값은 남기고 비권장 이유를 설명한다. 정수 재번호/기존 식별자 변경은 하지 않는다.
- `Features/StageGoal.cs`, `Core/StageRecordIdentity.cs`, `Features/TimeRecord.cs`: Stage 전용 책임을 Deprecated/주석으로 보존하며 생산 Stage 실행에 연결하지 않는다.
- `Features/CloudCodeRecordRepository.cs`: 직접 Stage Board 조회·제출 요청이 인증/transport 전에 차단되는지 확인하고 필요 guard를 추가한다. 매핑 함수/메뉴 차단만으로 Repository 우회가 막혔다고 판정하지 않는다.
- `Features/VerificationRecordComparison.cs`, `Features/VerificationPendingRetry.cs`, Editor probe의 helper: 남기는 합성 비교와 실제 온라인 Stage 접근을 구분한다. 과거 probe 번호를 호출해도 Stage 원격 작업이 시작되지 않도록 검사한다.
- `UGS/CloudCode/`의 Stage 분기: 서비스 소스를 삭제하지 않는다. 적용 가능한 비권장 사유 주석을 인계하되 Infinite 계약을 바꾸거나 Stage를 없애기 위한 원격 재배포는 기본 작업에 포함하지 않는다.
- 공통 Repository/Coordinator·Timer·입력·물리·카메라·Pattern은 새 요구 없이 리팩토링하지 않는다.

C#의 `Obsolete`는 독립된 Stage 전용 선언에 적용 가능한 범위에서 사용한다. 공통 API 전체를 비권장으로 만들지 않으며
분기/직렬화 참조에는 사유 주석과 실행 guard를 함께 둔다. 예상 경고의 국소 처리 필요성은 호출자별로 확인하고 전역 경고 억제로 숨기지 않는다.
주석에는 Infinite 단일화에 따른 신규 사용 중단 이유·대체 경로·남긴 소스의 범위를 명시한다.

#### 5-3. Scene·Prefab 정적 검사와 사용자 적용 절차 인계 — 완료

`SampleScene.unity` YAML의 fileID·스크립트 GUID·UnityEvent 대상과 `.cs.meta`를 읽기 전용으로 대조했다.
주요 참조 11개가 실제 객체로 연결됐고 아래 버튼 계열 UnityEvent 9개는 실제 GameSystem 컴포넌트와 존재하는 메서드를 가리켰다.

| 현재 객체/참조 | fileID | 후속 Editor 적용 방향 |
| --- | --- | --- |
| Goal, StageModeRoot, InfiniteModeRoot | `1090618367`, `901191967`, `172987142` | Stage Root/Goal 소스·참조 보존. Stage Root 비활성 유지, Infinite Root는 실제 Run 초기화 코드로 제어 |
| StageHUD, StageResultContent | `714045101`, `1501656893` | 비활성 보존하고 생산 표시에서 제외. InfiniteHUD/InfiniteResultContent와 공통 ResultPanel 유지 |
| ModeSelectPanel | `1386715964` | 비활성 보존, Play에서 열지 않음. 연결 필드를 임의 null로 만들지 않음 |
| Mode Select Stage/Infinite/Back Button | `761562235`, `593037429`, `1247354411` | 패널과 함께 비활성 보존. 기존 Play/StartRun에서 버튼을 경유하지 않도록 코드 변경 |
| Leaderboard Stage/Infinite Button | `1242588979`, `2045885536` | Stage Tab 비활성/Navigation 제외. Infinite 선택은 단일 Board UI 계약에 맞게 유지 |

9개 연결 검사는 SelectStage/SelectInfinite/SelectPlay/SelectRetry/SelectStartRun/SelectResultLeaderboard에 한정하며
Scene의 모든 UnityEvent가 검증됐다는 뜻은 아니다. GameSystem/StageSystem/InfiniteModeSystem 컴포넌트와 공통 참조를 삭제하지 않는다.
`Assets/Prefabs/InfinitePatterns/`의 Flat/SingleRise/LegacySteps/InternalGap 네 원본에서 script GUID가 로컬 스크립트에 연결됨을 확인했다.
GUID 존재 검사는 실제 Inspector 로드·물리·그림/조작감 검증을 대신하지 않는다.

**Phase 2~3에서 사용자가 수행할 Scene 적용 순서(지금은 실행하지 않음):**

1. AI가 실제 생산 코드·Test·정적 검사와 최종 적용 목록을 준비한 뒤 Unity에서 `Assets/Scenes/SampleScene.unity`를 연다.
2. 위 이름/fileID에 해당하는 객체를 찾아 ModeSelectPanel·StageHUD·StageResultContent·LeaderboardStageButton 및 StageModeRoot를 비활성 보존한다.
   같은 이름의 StageButton은 Mode Select 패널의 자식인지 구분한다. 소스/컴포넌트·Root 참조를 삭제하지 않는다.
3. GameSystem의 기본 Mode가 Infinite인지 확인하고, Play/StartRun·Retry·공통 메뉴 버튼 연결은 AI가 준비한 단일 Infinite 흐름에 맞춘다.
   SelectStage가 남은 비활성 버튼을 다시 활성화하거나 필수 공통 참조를 해제하지 않는다.
4. HowToPlayPanel의 기존 Text에서 Stage 속도/Goal·Clear Time 설명을 제거하고, 확정한 Infinite 조작/점수/종료 안내를 적용한다.
   Input Binding Text 참조·Pause/Settings/계정 화면 연결은 유지한다.
5. 수정한 Scene만 저장한다. AI가 YAML의 참조·비활성 상태·Navigation·UnityEvent를 정적으로 재검사한다.
   필수 입력/전환/물리 상태는 준비된 Test로 확인하며 화면 가독성 등 자동 판정이 어려운 부분만 사용자 수동 확인으로 남긴다.

이 절차는 최종 구현 후 적용할 인계다. 현재는 Scene 수정·저장·Inspector 반복 확인이 필요하지 않다.
네 Pattern Prefab은 변경 대상으로 지정하지 않는다.

#### 5-4. 기존 Test 재분류와 Phase별 인계 확정 — 완료

Stage 생산 시작 성공을 기대하는 기존 Test를 그대로 두면 차단 구현과 충돌한다.
`RestartInMode(E_GameMode.Stage)` 호출 Test 13개를 찾아 다음과 같이 분류했다. 일괄 Infinite 치환으로 물리/배치 전제까지 바꾸지 않는다.

| 기존 Test/helper | 변경/유지 기준 | Phase |
| --- | --- | --- |
| PlayMode `ProductionSceneGameModeTestUtility` | Infinite 생산 시작 helper와 명시적 Stage 거절 Test 분리. helper가 실제 우회 진입을 다시 열지 않음 | 2 |
| PlayMode `AutoMovementIntegrationTests`, `CameraFollowIntegrationTests`, `MomentumLandingIntegrationTests`, `PlayerJumpIntegrationTests`, `WallLandingRecoveryIntegrationTests` | 공통 이동/입력/카메라 판정은 실제 Infinite 구성 또는 해당 책임의 독립 fixture로 보존. Stage 지형을 전제로 한 기대값은 개별 검토 | 2·4 |
| PlayMode `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `PauseMenuIntegrationTests`, `ResultMenuIntegrationTests` | 생산 실행은 Infinite 기준으로 변경하고 시작/중복/Pause/Settings/Retry/종료/정리·복귀 검증 유지 | 2·4 |
| PlayMode `CollectibleLifecycleIntegrationTests`, `StageCollectibleLayoutIntegrationTests`, `StageCollisionConfigurationTests`, `StageGoalIntegrationTests` | 공통 lifecycle/충돌은 Infinite 또는 독립 fixture로 보존. Stage 배치/Goal은 생산 Run 없이 비권장 컴포넌트의 로컬 합성 검증으로 분리 | 2·4 |
| PlayMode `StageSystemTests`, `ModeResultDisplayIntegrationTests`, `ModeUISceneConfigurationTests`, `InfiniteModeIntegrationTests` | Stage 초기화 성공·Stage HUD/Goal 요구를 차단/비활성 보존 기대와 구분. Infinite 종료/표시·참조 유지 검증 | 2·4 |
| EditMode `GameNavigationStateTests`, `GameRuntimeDataTests`, `UIVisibilityStateTests`, `ResultDataTests`, `ResultSystemTests`, `ResultTextFormatterTests` | Step 2 전환표와 Stage 거절/비활성·Infinite 최종 결과 계약 연결. 독립 formatter 합성 Test를 생산 접근으로 취급하지 않음 | 2·3 |
| EditMode `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`, `LeaderboardViewStateTests`, `OnlineRecordRepositoryTests` | 신규 Stage 생성/조회/전송 0건과 Infinite 후보/조회/상한/Retry 검증. Stage factory를 쓰는 공통 fixture는 유효 Infinite fixture로 변경 | 3 |
| EditMode `LocalSaveJsonCodecTests`, `OnlineLocalSaveScopeTests`, `OnlineAccountCoordinatorTests`, `AccountTransferControllerTests`, `AccountTransferCompletionTests` | Infinite fixture로 기존 저장/gate/계정 이전 책임 보존. 새 Stage 기록 보관/이관 Test는 추가하지 않음 | 3 |
| EditMode `VerificationSessionTests`, PlayMode `VerificationToolIsolationTests`, `AccountTransferUIIsolationTests` | Editor Stage probe 거절·transport 호출 0건과 Infinite 도구 격리 보존 | 2·3 |
| `UGS/Tests/` 기존 source-contract 및 서비스 대역 suite | C# default/표기/fixture 변경에 결합된 정적 기대를 실제 계약에 맞게 갱신. 서비스 동작을 바꾸지 않았는데 기존 Node Stage 합성 case를 무조건 삭제하지 않음 | 2·3·4 |

Test 소스는 보존하되 검증 목적을 분류하고, 공통 책임을 검증하는 fixture가 비권장 생산 factory에 의존해 실패하지 않도록 한다.
실제 생산 코드에서 실행 차단이 되는지 확인하는 Test는 남기고, Test만 통과시키려고 차단 예외를 추가하지 않는다.
Phase 4의 실행 순서는 정적 검사 → 사용자 Unity Compile/EditMode/PlayMode → 필요한 후보 확인이다.
Step 4의 로컬 suite 결과는 기준선으로 연결하며 이번 문서/조사 변경만으로 같은 suite를 다시 실행하지 않았다.

#### 5-5. 검증 결과와 완료 판정 — 완료

- 읽기 전용 Node 검사로 Scene 주요 fileID 참조 11개·위 GameSystem 이벤트 9개·네 Pattern script GUID를 확인했다.
- Stage 분기 관련 생산/Editor 18파일과 Stage restart helper 호출 13 Test의 목록을 추출해 5-2/5-4에 연결했다.
- 조사 완료는 Stage 접근 차단·Deprecated 표기·Test fixture 전환·Scene 반영이 실제 완료됐다는 뜻이 아니다.
- 문서 정적 검사로 5-1~5-5 순서·Step 1~5 완료 체크·상대 링크·참조 소스/Test 존재·검출 18파일의 분류 누락 없음·다음 Step 6을 확인했다. diff 공백 검사를 통과했다.
- 이미 확정된 사용자 흐름/기록 제외/구 Client 미지원 범위 안에서 후속 목록을 확정했으므로 추가 정책 질문이나 수동 확인은 필요하지 않다.
- 생산 소스·Test·Scene/Prefab·원격 서비스를 변경하지 않았고 Unity Build/Compile/Test Runner도 실행하지 않았다.

Step 5를 완료 처리한다. 다음은 Step 6의 확정 정책 정적 검사·Unit Test 계약 고정이다.
위 사용자 Scene 절차는 후속 Phase에서 실행할 작업이며 현재 필요한 수동 작업은 없다.

## Step 6. 확정 정책을 정적 검사와 Unit Test로 고정한다

### 범위와 완료 조건

Step 2~5의 정책을 아래 계약 ID로 고정한다. Stage 자료 보관/혼합 큐·구 Client 지원 요구는 제외한다.
Phase 1에서는 현재 생산 코드의 Infinite 계약을 검증하고, 전환 후에만 검증 가능한 시작/접근 차단은 Phase 2~4에 연결한다.
그 기능을 Test 내부에서 다시 구현해 PASS 처리하거나 아직 구현하지 않은 기능의 Test를 실행했다고 기록하지 않는다.

- [x] 확정 정책과 정적 근거·Unit Test가 대응하고 현재 실행 결과와 후속 검증 경계가 명확하다.
- [x] Phase 1에서 추가/변경한 실행 Test의 필수 검증을 완료했으며 실패/미실행이 남아 있지 않다.

### Step 6 수행 결과 — 2026-11-11

#### 6-1. 정책–생산 경계–Test 계약 확정 — 완료

기존 C# Test 위치는 별도 표시가 없으면 `Assets/Tests/EditMode/`다.
아래 후속 Test 이름은 기존 suite 안에 추가/변경할 입력·기대 결과 계약이며 새 C# Test를 작성했다는 뜻이 아니다.

| 계약 ID / 확정 정책 | 실제 생산 경계 | 고정한 검증 입력·기대 결과 | 현재 근거 / 후속 시점 |
| --- | --- | --- | --- |
| P9-C01 시작/안내 | GameNavigationState·GameSystem의 Play/StartRun | Tutorial false → Infinite 안내만 표시, Run 0건. StartRun 1회 → Infinite Run 1개. true → 직접 시작. 기존 완료값 유지 | Step 2 전환표; `GameNavigationStateTests`, PlayMode `GameEntryBootIntegrationTests`/`HowToPlayIntegrationTests`, Phase 2 |
| P9-C02 과거 값/명시 요청 | GameRuntimeData·RuntimeDataSystem·StageSystem·ContextMenu/public API | 과거 Inspector/선택/대기 Stage는 시작 전 Infinite로 정규화. 명시적 Stage 실행은 거절, 기존 Run/선택/요청 수 불변 | Step 5 파일표; `GameRuntimeDataTests`, PlayMode `StageSystemTests`/`GameLifecycleIntegrationTests`, Phase 2 |
| P9-C03 Pause/Retry/복귀 | GameNavigationState·GameSystem·Timer·Result | 중복 입력/같은 프레임 요청 1회 처리, Pause/Resume·Settings 복귀·종료 유지. Retry는 새 Infinite Run/초기 Timer·점수·입력. 순위 Back은 진입 출처 복귀 | `GameNavigationStateTests`, PlayMode `PauseMenuIntegrationTests`/`ResultMenuIntegrationTests`, Phase 2~3 |
| P9-C04 사용자/직접 기록 접근 차단 | GameSystem·LeaderboardViewState·CloudCodeRecordRepository·Editor 도구 | Stage Tab/후보/Stage Board 직접 요청과 과거 probe 번호는 Stage 원격 호출 0회. Infinite 조회/제출 유지 | `RecordSubmissionPolicyTests`, `LeaderboardViewStateTests`, `OnlineRecordRepositoryTests`, PlayMode `VerificationToolIsolationTests`, Phase 3 |
| P9-C05 Deprecated 소스 보존 | Step 5의 전용 선언·분기·enum/직렬화 | Stage 소스·값 보존, 적용 가능한 비권장 표기·이유/대체 경로 주석. 공통 기반 전체 비권장화 금지. 표기와 실제 차단을 별도 검사 | 18파일 분류표·참조/GUID 검사; 접근 거절 Test와 함께 Phase 2~3 |
| P9-C06 Infinite 점수·전송·조회 | 실제 submit-record/query-records/snapshot 함수 | 유효 Infinite 점수 제출 → top/around/me/snapshot 동일 값. 동일 ID 재호출 1행, payload 충돌 거절, 더 높은 점수만 갱신 | 이번 신규 Node Test 6건 및 기존 상한 검사 PASS; 상세 6-2 |
| P9-C07 Pending 수명/격리 | LocalRecordRepository·OnlineRecordCoordinator·실제 receipt 서비스 | Retry 3회·동일 ID, 만료 180일 전/경계/후, terminal 후 저장 성공 시 정리. 실패·다른 환경/귀속 보호 | Node 기존 정책/소스 계약 PASS. 실제 C# 저장/재시작은 `OnlineLocalSaveScopeTests`/`OnlineRecordRepositoryTests`, Phase 3 |
| P9-C08 계정 이전/snapshot | OnlineAccountCoordinator·LocalRecordRepository·실제 계정/조회 서비스 | Infinite Pending 0건 gate·내구 저장·stale 응답 보호. 연결 계정 전체 최고로 교체, 이전 recipient 최고와 합산 안 함 | Step 4 Infinite-only snapshot/이전 3건 PASS, 이번 suite 재검증. C# `AccountTransferCompletionTests`/`OnlineAccountCoordinatorTests`, Phase 3 |
| P9-C09 Scene/공통 보존 | StageSystem/InfiniteMapPattern·입력·물리·카메라·UI/UnityEvent | Stage 화면/Root 비활성·탐색 제외, Infinite 종료·Collectible·World Rebase·HUD/Score 유지. 필수 참조 유효 | Step 5 정적 기준선. PlayMode `ModeUISceneConfigurationTests`/`InfiniteModeIntegrationTests` 등, Phase 2~4 |
| P9-C10 지원/서비스 경계 | 새 Client와 기존 Module/요청/저장 계약 | 구 Client 미지원, v7·Infinite Board/Scoring v2·endpoint 유지. Stage 자료 조사/보관/이관·원격 강제 구 버전 차단 추가 없음 | Step 3~4 사용자 확정 및 6-3 정적 대조. 후보 실서비스 확인은 Phase 4에서 별도 |

P9-C01~05/09의 목표 동작은 아직 미구현이다. 이 표는 요구/검증 인계 완료이며 생산 전환 PASS가 아니다.
현재 C#은 Stage 기본값·Mode Select·Stage 후보 생성 코드를 포함한다. 그 사실을 미래 기대값과 구분해 기록했다.

#### 6-2. 실제 Infinite 서비스 경계 Unit Test 보강 — 완료

`UGS/Tests/cloud-code.test.cjs`에 Infinite 전용 6건을 추가했다. 기존 SDK 대역 fixture에 receipt store 선택만 추가했고
서버 생산 함수·점수 검증식을 Test에서 다시 구현하지 않았다.

1. Infinite 제출 이후 top 10/around 7/me 1과 계정 전체 snapshot의 점수·본인 행·순위 일치.
2. 실제 receipt store를 주입한 v1→v2 ledger 경로에서 제출/동일 ID replay/payload 충돌과 조회/snapshot, Leaderboard projection 1회.
3. 동일 ID·동일 payload replay와 다른 payload 거절 시 기존 receipt/ledger 불변·projection 1회.
4. 더 낮은 점수는 최고를 교체하지 않고 더 높은 점수는 같은 고정 owner의 한 행만 갱신.
5. projection 응답 유실 후 동일 ID 재시도로 원래 acceptedAt 복구, 새 ID는 기존 미완료 후보를 우회하지 않음.
6. 같은 ID 동시 요청과 후속 retry가 한 terminal entry/한 Leaderboard 행으로 수렴.

SDK 대역의 projection 호출 수·ledger·조회 결과를 검사한다. 합성 자료/메모리 저장소만 사용했다.
v2 Test는 실제 receipt 저장/서비스 경로를 사용하지만, 원격 서비스의 다중 항목 원자성이나 모든 장애 조합을 증명하지 않는다.
Step 4에서 추가한 Infinite-only snapshot 3건은 그대로 유지했다. Stage 신규 생성 차단을 이 서버 Test의 PASS로 대체하지 않는다.

#### 6-3. 저장/식별자/격리 정적 검증 — 완료

Node 읽기 전용 검사와 기존 source-contract 검사를 통해 다음을 확인했다.

- E_GameMode의 선언 순서 Stage=0/Infinite=1, LocalSaveData Version 7 및 환경·귀속·Tutorial·Binding·시각·최고/Pending 필드 유지.
- Local Save에 선택 Mode를 저장하는 새 필드가 없음. Infinite Board `fs-infinite-v2`와 기존 서버 식별자 일치.
- Retry 최대 3회·Pending 180일 상수 및 계정 이전의 내구 저장 후 메모리 교체/연결 검사 계약 유지.
- 새 Node Test는 기존 SDK 메모리 대역과 실제 서비스 함수만 호출하고 HTTP/실제 사용자 파일 저장 코드를 추가하지 않음.
- C# preflight의 변경/신규 C# 파일은 0개다. asmdef·기존 격리 fixture 계약은 통과했다.
- Stage 실행 경로/비권장 표기/Scene 숨김은 Phase 2~3 적용 대기다. 정적 검사 출력에서도 DEFERRED로 구분했다.

Scene 참조/GUID 기준선은 Step 5의 11참조·9이벤트·네 Pattern 검사를 연결한다.
이후 Scene 변경이 없으므로 같은 검사를 반복 실행하지 않았다.

#### 6-4. 실행 명령과 결과 — 완료

| 명령 | 결과 | 의미 |
| --- | --- | --- |
| `node UGS/Tests/cloud-code.test.cjs` | 종료 0 / 28/28 PASS | Infinite 신규 6건과 기존 제출/조회 회귀. 추가 작업 중 v1 Test 5건을 먼저 검증한 27/27은 중간 결과이며 최종은 28/28 |
| `node UGS/Tests/account-personal-bests.test.cjs` | 종료 0 / 18/18 PASS | Step 4 Infinite-only 3건 포함 snapshot/계정 전환 |
| `node UGS/Tests/submission-retention-policy.test.cjs` | 종료 0 / PASS | 실제 기존 수명 정책 함수의 경계 |
| `node UGS/Tests/transfer-client-state-policy.test.cjs` | 종료 0 / PASS | 기존 이전/Pending gate 정책 함수 |
| `node UGS/Tests/client-local-save-contracts.test.cjs` | 종료 0 / PASS | C# 소스 계약. EditMode 27건 출력은 준비 수이며 미실행 |
| `node UGS/Tests/client-transfer-completion-contracts.test.cjs` | 종료 0 / PASS | C# 이전/세대/전체 최고 교체 소스 계약. EditMode 30건 준비·미실행 |
| `node UGS/Tests/client-unity-preflight.cjs` | 종료 0 / PASS | C# 변경 0개, 지정 EditMode 183/PlayMode 23건은 준비 수·미실행 |
| `node UGS/Tests/static-contracts.cjs` | 종료 0 / PASS 출력 8개 | 식별자·권한·SDK/환경·기존 서버 정책 정적 계약 |

기존 정책/정적 검사를 포함한 명령별 종료 코드를 확인했다. 합계 Unit Test 수에 정적 검사의 PASS 출력이나 C# 소스 case 수를 더하지 않는다.
새 Test fixture 옵션의 기본값은 기존 v1 방식으로 유지해 기존 회귀도 같은 suite에서 검증했다.
원격 요청·Unity Compile/Test Runner·빌드·Scene 수정은 수행하지 않았다.

#### 6-5. 사용자 실행 인계 — 완료

현재 C# 생산/Test 변경이 없고 Node Test만 추가했으므로 이번 Step에 필요한 사용자 수동 작업은 없다.
Phase 2~3에서 실제 C#과 Test가 변경된 뒤에만 다음 절차를 적용한다.

1. AI가 변경한 실제 정책/fixture와 5-4의 영향 suite를 대상으로 정적 검사·meta/asmdef·원격 격리 검사를 먼저 수행한다.
2. 사용자는 Unity Script Compile 후 Error·예상 밖 Warning을 확인하고 `Window → General → Test Runner`에서 준비된 EditMode suite를 실행한다.
3. Scene/입력/물리/카메라/생명주기에 영향이 있으면 준비된 PlayMode suite도 실행한다. 관련 공통 경계가 바뀌면 영향 suite 전체를 실행한다.
4. 실패 Test 이름·메시지·Stack Trace를 전달한다. AI가 수정한 뒤 관련 suite를 다시 실행하며 미실행 상태를 PASS로 바꾸지 않는다.
5. Scene 적용은 5-3의 사용자 절차를 따른다. 자동 판정 가능한 상태/중복·빠른 입력은 수동 재현으로 넘기지 않는다.

별도 Player/Module 빌드·배포를 이 Step의 완료 조건으로 추가하지 않는다. 실제 필요 범위는 Phase 4에서 구분한다.

#### 6-6. 완료 판정 — 완료

P9-C01~10을 실제 생산 경계·Test 입력/기대 결과·실행 또는 후속 Phase와 연결했고, 이번 Phase 1 추가 Test는 모두 실행해 통과했다.
생산 전환을 앞당기거나 검증되지 않은 정책 함수를 Test용으로만 만들지 않았다. Step 6을 완료 처리한다.
문서의 상대 링크·6-1~6-6 순서·P9-C01~10 행·Step 1~6 완료/7~8 대기·참조 Test/명령 파일 존재와 diff 공백 검사를 통과했다.
생산 소스/Scene/Module의 tracked 변경이 없음을 확인했다.
다음은 Step 7의 확정 사항 문서 반영·후속 작업 인계다. Phase 1 전체와 실제 생산 전환은 아직 완료하지 않았다.

## Step 7. 결정 사항과 후속 작업을 관련 문서에 반영한다

### 완료 조건

- [x] 확정 정책·적용 상태·검증과 후속 Phase 인계를 대조하고 관련 문서에 반영했다.

### Step 7 수행 결과 — 2026-11-11

#### 7-1. 결정과 현재 구현 상태 대조 — 완료

Step 2~6과 P9-C01~10을 기준으로 아래 사항을 재대조했다. 기존 결정을 다시 선택하도록 요청하지 않는다.

- 미래 계약: Infinite 단일 시작/안내/Retry/순위, 명시적 Stage 거절, Stage 소스 Deprecated·사유 주석과 공통 기반 보존.
- 확정 제외: Stage 기록 부재·추가 제출 없음의 사용자 전제로 보관/이관 기능 제외, 구 Client 미지원.
- 유지 계약: Infinite v7 저장·환경/귀속·Pending 3회 retry/180일 수명·계정 이전 0건/내구 저장·Scoring Version 2·현 서버 endpoint.
- 현재 검증: 실제 서비스 함수와 메모리 SDK 대역 제출/조회 28/28·snapshot 18/18 및 관련 기존 정책/정적 계약 PASS.
- 현재 미완료: 실제 C# 전환/비권장 표기·Scene 표시 변경·실제 Unity 컴파일/Runner/Player 및 최종 후보 검증.

Stage 기록 부재/구 Client 미지원의 사용자 결정을 원격 조사·강제 버전 차단의 PASS로 기록하지 않았다.
단일화로 공통 이동·물리·Pattern·점수 계산을 바꾸거나 원격 배포 범위를 추가하지 않았다.

#### 7-2. Feature/System 문서별 적용 예정 계약 반영 — 완료

각 문서에 `Roadmap 9 적용 예정 계약`을 추가하고 적용 Phase·생산 전환 미구현·P9-C 검증 ID·Task/Roadmap 링크를 명시했다.
현재 구현을 설명하는 기존 규칙/책임은 분리해 보존했다. 미래 Feature 규칙에는 사용자 동작을, System에는 담당 경계만 반영했다.

| 영역/문서 | 반영한 소유 계약 | 적용 Phase |
| --- | --- | --- |
| Feature `StagePlay`, `GameEntryNavigation`, `HowToPlay` | Infinite 시작·과거 값/명시 요청 처리·Mode Select 우회·기존 Tutorial 완료 보존과 Infinite 안내 | 2 |
| Feature `ResultMenu`, `Leaderboard` | Infinite 결과/순위만 제공·Run/제출 Retry 분리·진입 출처별 복귀·기존 기록 표시 의미 유지 | 2~3 |
| Feature `RecordSubmission`, `AccountTransfer` | Stage 신규 생성/제출 차단·기록 보관 미추가·Infinite 수명/귀속/이전 gate와 전체 최고 교체 유지 | 3 |
| Feature `InfiniteMode` | 단일 생산 Mode와 기존 Pattern/점수/물리/World Rebase 보존 | 2~4 |
| System `GameSystem`, `StageSystem` | 시작/종료·정규화/거절·공통 생명주기/Collectible·전용 소스 비권장 경계 | 2~3 |
| System `UIManagementSystem`, `ResultSystem` | 새 화면/Focus·결과 표시 연결과 공통 결과/입력 상태 보존, Stage 전용 책임 분리 | 2~3 |
| System `RecordSubmissionSystem` | 신규 Stage 진입 차단과 Infinite 제출/대기열 경계 유지 | 3 |

위 Feature 8개·System 5개 외에 Project Overview의 후속 방향에 확정 범위 링크를 반영했다.
Project Memory에는 현재 유효한 결정/한계와 후속 위치만 요약했고 기존 실행 이력은 Task를 참조하도록 유지했다.
Roadmap/안내는 Step 1~7 완료·Phase 1 진행 중·다음 Step 8로 갱신했다.

관련 현재 문서 불일치도 코드 근거로 수정했다.
`HowToPlay`의 영구 저장 미도입/생산 연결 예정 설명은 실제 GameSystem.InitializeLocalState/SaveLocalState의 Tutorial 복원·저장 및 현재 생산 안내와 달라 바로잡았다.
`UIManagementSystem`의 마스킹 계정 표시 설명은 실제 PublicPlayerNumber 유효성 검사·표시와 맞게 공개 번호로 정정했다.
이 두 수정은 미래 기능을 구현 완료라고 기록한 것이 아니라 기존 코드와 문서의 불일치를 바로잡은 것이다.

#### 7-3. Phase별 실행·검증·사용자 작업 인계 — 완료

| 후속 작업 | AI 담당·정적/Unit Test | 사용자 담당·시점 | 인계 근거 |
| --- | --- | --- | --- |
| Phase 2 시작/안내/플레이 | 5-2 파일별 변경·P9-C01~03/05/09 및 5-4 fixture 분류 구현, 기본/직접 Stage 경로·Deprecated/참조·격리 검사 | 코드 준비 후 Unity Compile·해당 EditMode/PlayMode 실행. Scene은 5-3의 비활성 보존·안내/버튼 절차 적용 | 2-3 전환표, 5-1~5-4, 6-1/6-5 |
| Phase 3 기록 접근 | Infinite 단일 UI/직접 Repository/Editor probe guard·P9-C04/06~08/10 구현·회귀. Stage 보관/이관·구 Client 지원은 추가하지 않음 | 변경 C# Compile/Test, 필요한 기록 UI Scene 적용. 정적 판정 가능한 항목은 AI가 확인 | 3-3/3-4, 4-1~4-5, 5-2/5-4, 6-1 |
| Phase 4 통합 후보 | 변경 경계 정적 감사·영향 suite·제출/조회/snapshot 회귀와 잔여 한계 정리. 실행 결과/후속 의무 대조 | 준비된 Unity Test와 필요한 새 Client 후보 확인. 시각적 가독성 등 자동 판정이 부적절한 항목만 수동 확인 | 5-4, 6-4/6-5 및 실제 변경 후 검사 목록 |

실행 순서는 정적 검사 → 사용자 Compile/EditMode/PlayMode → 필요한 후보 확인이다.
빠른 입력·동일 프레임 중복·타이밍/상태를 수동 재현으로 전가하지 않는다. Stage 생산 실행을 Test용으로 다시 허용하지 않는다.
지금 Scene 작업·빌드·Test Runner를 수행할 필요는 없다. Step 8의 Phase 1 최종 대조를 먼저 수행한다.

#### 7-4. 서비스 변경/복구 및 미정 사항 인계 — 완료

현재 기능 전환에 서버 계약 변경은 불필요하다는 Step 4 판정을 유지한다. 구 Client 미지원은 강제 원격 차단/공개 종료 작업 승인이 아니다.
현재 요청으로 Module 빌드/게시·Secret/역할 변경·계정/보드/자료 삭제·일반 공개/Steam 작업을 추가하지 않는다.

후속 구현에서 실제 Module 업데이트가 필요해지면 Roadmap 8의 연기된 복구 시험 의무를 적용한다.
업데이트 전에 성공 source 전체·artifact/hash·환경/게시 reference·새 QA 기준을 보존하고,
호환성·시험/중단/재복구 범위·최종 유지 버전의 별도 승인을 확보한 뒤 해당 절차를 수행한다.
기존 Step 11 수동 검증 면제를 이번 변경에 승계하지 않는다. 정책 확정만으로 원격 배포나 복구 시험 성공을 주장하지 않는다.

정책 선택 대기는 없다. 남은 일은 Step 8의 최종 완료 조건 대조와 후속 Phase의 실제 구현/실행이다.
현재 사용자에게 요구하는 수동 작업은 없고, 이미 답한 정책을 다시 승인할 필요도 없다.

#### 7-5. 문서 정적 검증·완료 판정 — 완료

Node 읽기 전용 검사로 Feature 8개/System 5개의 적용 예정 절 중복 없음·미구현 표시·Phase/P9-C 연결·현행 절 보존과 상대 링크를 확인했다.
7-1~7-5 순서·Step 1~7 완료/8 대기·다음 Step 8·복구 인계를 확인했고 코드 근거로 Tutorial/공개 번호 문서 정정도 대조했다.
diff 공백 검사와 생산 코드/Scene/Module의 tracked 변경 없음 검사를 통과했다.
이번에는 문서만 수정했으므로 Node/Unity Unit Test를 반복 실행하지 않는다. 기존 6-4 실행 근거는 그대로 유지한다.
검증 결과를 기록해 Step 7을 완료 처리했다. 다음은 Step 8 Phase 1 완료 판정이다.

## Step 8. Phase 1 완료 여부를 판정한다

### Step 8 수행 결과 — 2026-11-11

#### 8-1. Roadmap 완료 조건과 사용자 범위 변경 대조 — 완료

| Phase 1 완료 조건 | 연결 근거 | 판정 |
| --- | --- | --- |
| 차단할 사용자 경로와 남길 내부/공통 경로 식별 | Step 1의 경로표, Step 2 흐름 결정, Step 5의 18파일/13 Test 분류 및 Scene 참조 인계 | 충족. 생산 차단 실행 성공을 뜻하지 않음 |
| 기존 Pending 정책·구 Client 지원 명시 | Step 3의 Stage 기록 없음/추가 제출 없음 전제와 별도 기록 처리 제외, Step 4의 구 Client 미지원 확정 | 사용자 범위 변경을 적용해 충족. Stage 자료 부재의 원격 입증이나 강제 구 버전 차단이 아님 |
| 자료 삭제/보드 초기화 없는 전환·서비스 변경 판단 | Step 4 공통 저장/Infinite 서버 계약 유지, Step 5~7 Deprecated 소스 보존·사용자 작업·복구 의무 인계 | 충족. 현재 범위에서 서비스 계약 변경 불필요, 실제 게시/자료 조작 없음 |

초기 로드맵의 Stage 보관/이관·혼합 자료 호환 요구는 사용자 지시로 제외됐다.
이 제외를 검증 생략이나 구현 PASS로 바꾸지 않고 확정 범위로 기록했다. Infinite 수명·계정 이전·환경/귀속 계약은 유지한다.

#### 8-2. 검증 근거와 필수 미실행 여부 대조 — 완료

- Step 6의 최종 Node 서비스 대역 실행: 제출/조회 28/28, 개인 최고 snapshot 18/18 PASS.
- 수명/이전 정책 함수·저장/이전/preflight/기존 서버 정적 계약 PASS. C# 준비 case 수는 실행 수에 합산하지 않았다.
- Step 5의 Scene 주요 참조 11개·GameSystem 이벤트 9개·네 Pattern GUID 및 Step 7의 Feature 8개/System 5개 미래 계약 검사를 연결했다.
- Step 6 이후 실행 코드/Test를 변경하지 않았으므로 이번 문서 최종 판정에서 Unit Test를 반복 실행하지 않았다.
- Phase 1에 C# 생산/Test 변경이 없어서 필요한 Unity Compile/Runner 결과 미보고는 없다. Phase 2~4의 실제 실행/후보 검증은 남아 있다.
- 새 Client 시작/Stage 차단·Deprecated 표기·Scene 비활성 전환은 미구현이다. 해당 목표 동작을 현재 통과한 Test로 판정하지 않는다.

최종 상태 갱신 전 읽기 전용 감사에서 Step 1~7 완료·정책 확정·13개 미래 계약 문서/링크와 생산/Scene 변경 없음을 확인했다.
Phase 1 범위의 미해결 Test 실패나 미정 정책은 없다.

#### 8-3. 후속 Phase와 실제 사용자 작업 대조 — 완료

Phase 2~4의 실제 작업은 5-2 파일표·5-3 사용자 Scene 절차·5-4 Test 재분류·6-1 P9-C01~10·6-5 실행 절차·7-3~7-4 인계를 따른다.

- 다음 대상: Phase 2의 메뉴/직접 실행/Retry·기본값·안내와 공통 기반 보존 구현 준비.
- Phase 3: 신규 Stage 후보/조회/제출·Editor probe 차단과 Infinite 기록/이전 계약 유지.
- Phase 4: 실제 변경 경계 정적 검사·관련 EditMode/PlayMode·새 Client 후보와 잔여 한계 판정.
- 실제 Module 변경이 필요해질 때만 보존 자료와 별도 승인·복구 시험 범위를 적용한다. 현재 서버 변경/빌드/원격 작업은 추가하지 않는다.

현재 필요한 사용자 수동 작업은 없다. 정책 재승인·기기/원격 Stage 자료 조사·Scene 수정·새 빌드·Test Runner 실행을 요구하지 않는다.
생산 코드/Test 준비 후 필요한 Unity 실행·Scene 적용은 사용자 담당으로 인계돼 있다.

#### 8-4. 완료 상태·잔여 한계 문서 반영 — 완료

Task의 Step 1~8 및 Phase 1을 완료 처리하고 Roadmap/안내·Project Memory를 갱신한다.
Phase 2~4는 대기 상태를 유지하며 이번 요청에서 자동 착수하지 않는다.
실제 Stage 접근 제거/Deprecated 코드 적용·Unity 컴파일/Runner·Player/Module 빌드·Scene 반영·원격 동일성/게시·공개는 완료된 것으로 기록하지 않는다.

초기 Task 요약에 남아 있던 정책/Test 미수행 표현은 과거 조사 이력으로 구분하고 최종 결과와 모순되지 않게 정리한다.
Phase 1 완료는 범위/정책/검증 계약 및 인계 완료이며 Roadmap 9 전체나 전환 후보 검증 완료는 아니다.

#### 8-5. 최종 확인표·정적 검증 — 완료

- [x] 사용자 접근 차단 경로와 Deprecated 보존/공통 유지 경로가 식별됐다.
- [x] Stage 기록 처리 제외 전제와 Infinite 수명/계정 이전 유지, 구 Client 미지원이 확정됐다.
- [x] 자료 삭제/보드 초기화 없이 전환하고 현재 서비스 계약 변경이 불필요하다는 판정이 반영됐다.
- [x] 필요한 로컬 Test 실행 근거와 Phase 2~4 정적/Unit·통합 검증·사용자 작업이 연결됐다.
- [x] 정책/준비 완료를 생산 구현·Unity 실행·원격 게시 성공과 구분했다.

최종 읽기 전용 Node 감사에서 Step 1~8 완료·8-1~8-5 순서·최종 체크 5개·13개 미래 계약 문서/상대 링크,
Roadmap Phase 1 완료/Phase 2~4 대기·미구현 표시 유지·생산/Scene/Module 미변경을 확인했다.
tracked diff와 신규 Task의 공백 검사도 통과했다. Phase 1의 필수 미해결 작업은 없다.

# 영향 범위

이번 작업은 Task의 실행 절차·Step 1~8 결과, 관련 Feature/System의 적용 예정 계약, Project Overview/Memory와 Roadmap/안내의 진행 상태·인계에 영향을 준다.
Step 4에서 `UGS/Tests/account-personal-bests.test.cjs`에 Infinite 전용 대역 Test 3건을 추가했다.
Step 6에서 `UGS/Tests/cloud-code.test.cjs`에 Infinite 서비스 대역 Test 6건과 receipt store 선택 fixture를 추가했다.
Feature/System에 미래 적용 계약을 명시했으며 현행 생산 코드·Scene·실제 저장·원격 서비스 동작은 변경하지 않았다.

# 검증 내용

문서 작성 후 Step별 담당·수동 작업·완료 조건, 상대 링크, 근거 파일 존재, 파일명 날짜/번호 중복,
Roadmap Phase 1 범위와 상태 표현, `git diff --check`를 확인한다.

# 검증 결과

최종 판정: Step 1~8 및 Phase 1 완료. Step 8의 완료 조건/정적 감사와 기존 Step 6의 28/28·18/18 로컬 실행 근거를 연결했다.
생산 전환·Unity 실행·Scene/원격 변경·Roadmap 9 전체는 미완료이며 후속 Phase에 남긴다.

Step 7은 문서 변경만 수행했다. 적용 예정 계약·현재 코드와의 구분·후속 실행/서비스 복구 경계의 정적 대조 결과는 7-5에 기록한다.

Step 6 최신 결과: 제출/조회 suite 28/28·snapshot 18/18과 수명/이전 정책·저장/이전/preflight/서버 정적 계약 통과.
현재 실행과 후속 생산 전환 검증의 경계는 6-1~6-6에 기록했다. C# Test Runner·생산 전환은 미실행이다.

Step 5 최신 결과: Scene 참조 11개·GameSystem UnityEvent 9개·네 Pattern script GUID의 정적 대조를 통과했다.
생산/Editor 18파일·Stage restart helper 호출 13 Test의 후속 변경 분류를 확정했다. 이번에는 문서만 변경했으며 기존 Unit Test를 반복 실행하지 않았다.

Step 4 최신 결과: snapshot suite 18/18, 제출/조회 suite 22/22, 계정 이전/로컬 저장 정적 계약 PASS.
상세 명령·범위·미실행 항목은 4-4에 기록했다. 이전 Step의 실행 수는 당시 이력으로 유지한다.

최초 절차 작성 당시 Node 읽기 전용 검사로 상대 링크·Step 순서·참조 소스/Test·날짜/작업 번호 중복 없음과 당시 대기 상태를 확인했다.
아래는 단계별 과거 검사 이력이며 최종 완료 상태는 8-5를 따른다.
Step 1 완료 반영 후 상대 링크·1-1~1-5 및 Step 1~8 순서·Step 1만 완료 체크·참조 파일 존재를 다시 확인했다.
`git diff --name-only -- Assets UGS ProjectSettings`에 변경이 없음을 확인했다.
최초 절차 작성 시에는 Step 1 기준선을 실행하지 않았고, 이후 Step 1 수행에서 네 기준선 검사를 실행해 모두 통과했다.
1-5의 최초 기준선 뒤 정책 결정과 Step 4/6의 Node Test 추가·실행을 완료했다. Unity 실행·생산 구현·원격 변경은 계속 미수행이다.

# 후속 작업

Phase 1의 Step 1~8을 완료했다. 다음은 Phase 2 작업 준비/착수이며 이번 요청에서 Phase 2 구현을 시작하지 않았다.
구 Client/Stage 기록 관련 선택은 다시 요청하지 않는다. 실제 적용 파일·Test·사용자 Scene 절차는 5-2~5-4와 6-1/6-5·7-3에 연결돼 있다.

# 관련 문서

- [Roadmap 9](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md)
- [로드맵 공통 경계](../../04_Implementation_Roadmap/README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [Investigation Rule](../../01_Rules/INVESTIGATION_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [StagePlay](../../03_Features/StagePlay.md)
- [GameEntryNavigation](../../03_Features/GameEntryNavigation.md)
- [ResultMenu](../../03_Features/ResultMenu.md)
- [Leaderboard](../../03_Features/Leaderboard.md)
- [RecordSubmission](../../03_Features/RecordSubmission.md)
- [AccountTransfer](../../03_Features/AccountTransfer.md)
- [InfiniteMode](../../03_Features/InfiniteMode.md)
- [StageSystem](../../02_Systems/StageSystem.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)

# 관련 작업 기록

- [Roadmap 9~14 계획 정리](../20261111_01_Roadmap9To14Planning.md)
- [Prototype 8 Phase 1 절차](../Prototype_8/20260930_01_Phase1ManualSteps.md)

# 작성 완료 기준

- 실제 수동 작업을 Step으로 구분하고 AI가 먼저 수행할 조사·검증과 연결했다.
- 각 Step의 완료 조건과 Phase 1 완료 판정 기준을 명시했다.
- 정적 검사와 Unit Test의 대상·기존 출발점·실행 담당·후속 통합 범위를 구분했다.
- 문서 작성 완료를 정책 확정·구현·실행 PASS로 기록하지 않았다.
