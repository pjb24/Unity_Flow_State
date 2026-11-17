# 작업 정보

## 작업명

Prototype 9 Phase 2 — Stage 플레이 접근 차단과 Infinite 시작 흐름 전환 절차

## 작업 일자

20261112

## 작업 담당자

AI: 생산 코드·Test 조사/수정, 정적 검사, 로컬 대역 Test 실행, 적용 방법·검증 결과 정리.
사용자: Unity Compile·Scene 적용·Test Runner·Player 빌드/실행 및 시각적 확인.

## 작업 상태

Step 1~10 완료, Phase 2 완료(2026-11-17). 최신 Pause Retry 확인창 변경 후 사용자 EditMode 919개/PlayMode 288개 전체 통과와 Windows-production Candidate02의 빌드/플레이/화면·로그 정상 범위 보고를 받았다. 최종 Scene/소스/프로필 정적 검사를 통과했다.
최신 후보는 사용자 결정으로 `Build/ProductionQA/Candidate02/`에 덮어쓴 빌드다. 이전 Candidate02 실물 보존/별도 Candidate03 생성을 요구하지 않는다. 이전 916/287 결과는 변경 전 이력이며 최종 검증 수는 919/288이다.
다음은 Phase 3 잔여 Stage 기록 Repository/제출/Editor 도구 경계와 Infinite 저장/계정 이전 회귀다. Phase 3 전체·Phase 4·Roadmap 9 전체는 미완료다.
Step 2~6 완료는 코드/Test/정적 검사/적용표 준비 완료이며 Unity 실행 PASS나 전체 Stage 기록 접근 차단/Phase 2 완료를 뜻하지 않는다.
사용자 추가 UI 정책의 코드 준비 결과는 4-6에, 실제 Scene 적용은 Step 7-2~7-4에 통합했다. Step 6에서 Controller의 실제 소속을 대조해 Hover 12개·Rebind 6개·Outline 2개(총 20개)로 적용 목록을 정정했다.
2026-11-17 사용자 요청으로 Leaderboard Stage 접근 제거와 Pause Retry 확인창을 반영했다. 최종 Scene/실행 근거는 9-3과 Step 10에 기록했다. Stage Outline은 선택 표시에서 제외하고 Infinite Outline을 켠다. Phase 3의 Repository/제출/도구 전체 차단은 별도 잔여 범위다.

# 작업 목적

Main Menu부터 직접 실행·과거 기본값·최초 안내·Pause/Result Retry까지 Stage 플레이 진입을 차단한다.
Infinite의 공통 실행 기반을 유지하면서 정적/자동 검증을 먼저 준비하고, 필요한 사용자 작업만 순서대로 수행하도록 한다.

# 작업 대상

P9-C01~03·P9-C05·P9-C09의 플레이/안내/UI 경계와 관련 생산 코드·EditMode/PlayMode Test·SampleScene.
계약과 파일별 인계는 [Phase 1 Task](20261111_02_Phase1ManualSteps.md)의 Step 2·5~7을 따른다.

# 작업 전 상태

- Phase 1은 완료했고 Phase 2~4는 대기다. Stage 소스는 남아 있으며 아직 비권장 표기·생산 차단을 적용하지 않았다.
- Play는 Mode Select를 경유한다. GameNavigationState와 여러 무인자 초기화의 기본 Mode는 Stage다.
- SampleScene의 GameSystem 직렬화 Mode는 Infinite지만 메뉴/직접 API의 Stage 진입이 차단된 상태는 아니다.
- Phase 1 로컬 기준선은 제출/조회 28/28·snapshot 18/18 PASS다. 향후 C# 변경 후 Unity 성공 근거로 재사용하지 않는다.

# 조사 내용

- Unity 버전은 `ProjectSettings/ProjectVersion.txt`의 `6000.3.5f2`다. 생산 Scene은 `Assets/Scenes/SampleScene.unity`다.
- `Assets/Settings/Build Profiles/Windows-production.asset`이 존재한다. 실제 활성 프로필·출력 경로는 빌드 시 확인한다.
- `StageSystem.Initialize(E_GameMode)`는 현재 Mode 검사 전에 Collectible 해제/Goal listener 제거/초기화 플래그 변경을 수행한다.
  단순히 검사 끝에 Stage 거절을 넣으면 기존 Run이 훼손될 수 있어 거절 위치와 상태 불변 Test가 필요하다.
- 기존 UI 필수 참조 검사는 Mode Select/Stage Button도 요구한다. 패널을 숨기는 것과 실행/참조 계약 변경을 함께 다뤄야 한다.
- Stage 시작 helper를 쓰는 PlayMode Test와 Stage 후보 factory를 공통 fixture로 사용하는 Test가 있다.
  일괄 Infinite 치환이나 검증 생략 대신 책임별 fixture 변경이 필요하다.

# 작업 내용

## 공통 원칙과 Phase 경계

- 아래 Step은 수행 순서다. 각 Step은 AI가 준비를 마친 뒤 필요한 사용자 작업을 구체적으로 안내한다. 이 문서를 보고 지금 Scene/빌드를 시작할 필요는 없다.
- Stage 전용 소스/선언/분기/직렬화 참조를 삭제하지 않는다. 적용 가능한 `Obsolete`와 사유/대체 경로 주석을 사용하고 실제 접근 차단은 별도로 구현한다.
- StageSystem·Timer·Runtime/Result·입력·이동·카메라·Infinite Pattern 등 공통 기반 전체를 Deprecated로 만들지 않는다.
- enum 값·GUID·Infinite Board/Scoring Version·물리/점수/Pattern 규칙은 유지한다. 전역 경고 억제나 Test용 생산 접근 예외를 추가하지 않는다.
- 확정된 사용자 정책을 다시 질문하지 않는다. 구 Client는 미지원이고 Stage 기록이 없고 앞으로도 제출되지 않는다고 전제한다. Stage 보관/이관·서버 자료 조사·삭제는 추가하지 않는다.
- **Phase 3 소유:** Stage Leaderboard 탭/직접 조회·기록 후보/제출·온라인 probe 차단, Infinite 저장/계정 이전 회귀. Phase 2 완료를 전체 Stage 기록 접근 차단 완료로 기록하지 않는다.
- 동일한 `SelectStage`가 플레이 선택과 Leaderboard 선택에 쓰이는 현재 구조를 구분한다. Phase 2는 Stage 플레이 요청을 차단하고 기록 화면 계약 변경은 Phase 3에 인계한다.
- 공유 함수의 변경으로 Phase 3 Test가 깨지면 필요한 fixture/공통 회귀는 이번에 수정·검증한다. 이를 이유로 기록 기능 전체를 미리 구현하지 않는다.
- AI는 Unity Compile/Build/Test Runner와 Scene/Prefab 편집을 수행하지 않는다. Scene은 읽기 전용 정적 조사만 한다. 사용자 실행은 Step 7~9에 배치한다.
- 로컬 Node Test는 메모리/SDK 대역만 사용한다. 자동 Test는 기존 격리 설정으로 실제 인증·원격 UGS·사용자 영구 저장과 분리한다.
- Player 확인도 기존 QA 기기/저장 자료를 보존해 수행하며, Stage 서버 호출·실제 계정 이전·원격 게시를 검증 수단으로 요구하지 않는다.

## Step 1. 착수 기준선과 변경 목록을 확인한다

### AI 작업

1. Phase 1의 P9-C 계약·파일표·13개 Stage restart helper 호출 Test와 현재 코드를 다시 대조한다.
2. 기존 변경을 보존하며 git 상태, 기본값·ContextMenu·직접 API·UnityEvent·필수 참조의 현재 위치를 기록한다.
3. 다음 검색을 시작점으로 호출자/이벤트/분기까지 추적한다. Stage 문자열 0건을 완료 기준으로 삼지 않는다.

```powershell
rg -n 'E_GameMode.Stage|ModeSelect|StartGame|StartRequestedRun|RetryGame|StartStage' Assets/Scripts Assets/Editor Assets/Tests
rg -n '_selectedGameMode|_stageModeRoot|_infiniteModeRoot|_modeSelect|m_MethodName' Assets/Scenes/SampleScene.unity
rg -n 'RestartInMode\(E_GameMode.Stage\)|TryCreateStageCandidate' Assets/Tests
```

4. 실행 범위가 로컬임을 확인한 뒤 기존 검사로 기준선을 확보한다. 최신 변경에 해당하는 실패와 기존 실패를 구분한다.

```powershell
node UGS/Tests/client-unity-preflight.cjs
node UGS/Tests/cloud-code.test.cjs
node UGS/Tests/account-personal-bests.test.cjs
```

### 사용자 수동 작업

없음. 코드/직렬화 값·목록을 사용자에게 다시 조사하도록 요구하지 않는다.

### 완료 조건

- [x] 현재 경로·영향 파일/Test·기준선 결과와 미확인 사항이 기록됐다.

### Step 1 수행 결과 — 2026-11-12

#### 1-1. 확정 계약·작업 트리·실행 경계 확인 — 완료

- Phase 1 Step 2·5~7과 P9-C01~10을 현재 코드와 대조했다. 이번 대상은 P9-C01~03/05/09의 준비이며 P9-C04/06~08/10의 기록 접근·저장/온라인 계약은 Phase 3 인계를 유지한다.
- 착수 HEAD는 `6c4f71d`, Node는 `v24.21.0`이다. 착수 당시 변경은 Roadmap 9 수정 1개와 이 Task의 미추적 파일 1개였다. 기존 내용을 보존하며 이번 조사 결과를 추가했다. 생산 코드/Test/Scene의 선행 변경은 없었다.
- 지정 Node 검사의 소스와 `account-sdk-double.cjs`를 확인했다. preflight는 파일 읽기와 git 조회만 수행하며, 두 서비스 suite는 메모리 Map/SDK 대역을 주입한다. 실제 인증·UGS·사용자 저장·Unity 프로세스를 실행하지 않는다.
- Project/Rules와 관련 Feature/System의 Roadmap 9 적용 예정 계약을 확인했다. 미래 계약을 현재 생산 동작으로 바꾸어 기록하지 않는다.

#### 1-2. 기본값·생산 시작/Retry·직접 API 경로 대조 — 완료

아래 경로는 현재 구현 상태이며 아직 차단하지 않았다. 경로는 `Assets/Scripts/Runtime/` 기준이다.

| 경계/현재 근거 | 확인 결과 | 후속 변경·검증 |
| --- | --- | --- |
| `Core/GameNavigationState.cs:19,30,304,341,378` | 선택/대기 기본값 Stage. Play→ModeSelect→Stage/Infinite 선택, 미완료 Tutorial은 선택 Mode를 대기시킨 뒤 StartRun에서 사용 | Step 2: Play→Infinite/안내 전환, 과거 값 정규화, 중복 요청·실패 복귀 Test |
| `Systems/GameSystem.cs:12,159,181,268,536,1063` | 코드 기본 Stage. ContextMenu/public StartGame→BeginRunFromMenu, 메뉴 버튼/공개 RequestNavigationSelection→HandleModeSelectSelection. Infinite 이외 값은 현재 Stage 항목으로 매핑됨 | Step 2~4: 과거 선택 정규화와 명시적 Stage/미정의 요청 거절을 구분 |
| `Systems/GameSystem.cs:560,584,595,745,1182` | StartRequestedRun이 선택 Mode로 Runtime 생성·Stage/Infinite 초기화를 수행. Pause Retry는 EndRun(false) 후 재시작, Result Retry는 BeginRunFromResult 요청 후 재시작 | Step 2~3: 상태 변경 전 실행 경계 점검, Infinite 새 Run·Pause/Result Retry 보존 |
| `Core/GameRuntimeData.cs:30,35,84`, `Systems/RuntimeDataSystem.cs:14,19` | 무인자 생성/초기화가 Stage. 명시 인자 거절 없음. GameRuntimeData.Initialize는 기존 Collectible을 먼저 정리하며 Clear는 Mode를 Stage로 복원. RuntimeDataSystem은 기존 Run이면 인자 검사 없이 기존 객체 반환 | Step 3: 거절을 생성/정리/기존 Run 반환보다 앞에서 판정, void/반환값 계약과 상태 불변 검증 |
| `Systems/StageSystem.cs:84,89,151` | 무인자 Initialize는 Stage. 명시 초기화가 Collectible 해제·Goal listener 제거·초기화 플래그 변경 후 Mode 검사. StartStage는 Stage 실행을 허용 | Step 3: 부작용 전 거절과 미초기화/Ready/Playing/Paused/Result 상태·Root/listener/scope 보존 Test |
| `Features/InfiniteModeState.cs:58,64`, `Systems/StageSystem.cs` | 내부 초기화 Mode Stage가 남고 SetGameMode가 별도 존재. StageSystem의 Infinite 종료/수집/Root/Pattern 경계는 공통 기반 | Step 3: 내부 비활성 상태와 생산 실행 구분. 전체 클래스 Deprecated/enum 재번호 금지 |
| `Core/UIVisibilityState.cs:60,80`, `Systems/UIManagementSystem.cs:278,400,1165` | Stage HUD/Result 표시 가능. 실제 필수 검사명은 `HasNavigationUIConfiguration`이며 ModeSelectPanel/Stage·Infinite·Back Button도 필수. SetNavigationScreen/GetNavigationButton에서 ModeSelect 표시/포커스 가능 | Step 4: 필수 참조·표시·선택 복원/탐색을 함께 수정. Phase 1의 HasNavigationReferences 표현은 이 실제 프로퍼티를 가리키는 것으로 정정 인계 |
| `Systems/GameSystem.cs:268`, `Features/LeaderboardViewState.cs`, `Assets/Editor/OnlineRecordVerificationWindow.cs:227,368,563,757,765` | SelectStage는 플레이/Leaderboard에서 공유. Editor 창의 Stage Board/후보/direct-write probe는 기록 요청이며 검색한 Editor 소스에 별도 StartGame/StartStage 호출은 없음. 격리 예약은 사용자의 수동 Play를 전제로 함 | Step 4: 실제 플레이 시작 경계 확인. Stage Tab·온라인 probe·Repository guard는 Phase 3이며 이번에 원격 실행하지 않음 |

Phase 1의 생산/Editor 18파일 검색 집합이 현재도 일치한다. Step 2~4의 직접 변경 출발점은
`GameNavigationState`, `GameRuntimeData`, `RuntimeDataSystem`, `GameSystem`, `StageSystem`, `InfiniteModeState`,
`ResultData`, `ResultTextFormatter`, `UIVisibilityState`, `UIManagementSystem` 10파일이다.
나머지 8파일(`LeaderboardViewState`, `RecordSubmissionPolicy`, `RecordBoardKey`, `OnlineRecordConfiguration`,
`RecordLeaderboardPolicy`, `LocalSaveJsonCodec`, `OnlineAccountCoordinator`, `OnlineRecordVerificationWindow`)은
Phase 3 또는 공유 경계 영향 확인 대상으로 유지한다.
추가 선언 `E_GameMode`·`StageGoal`·`StageRecordIdentity`·`TimeRecord`의 소스/값 보존·Deprecated 범위도 Phase 1 5-2를 따른다.
실제 수정 집합과 실행 suite는 구현 후 확정하며 공통 Timer/물리/카메라/Pattern/점수/서버 계약을 변경 대상으로 확대하지 않는다.

#### 1-3. Scene 직렬화·UnityEvent·프로필 기준선 확인 — 완료

`Assets/Scenes/SampleScene.unity`를 읽기 전용으로 파싱해 참조 대상 11개와 지정 GameSystem 이벤트 9개를 재확인했다.
자체 활성값은 부모를 포함한 실제 화면 노출 상태와 구분한다.

| 객체/필드 | 현재 fileID·값 | Step 7 인계 |
| --- | --- | --- |
| ModeSelectPanel / StageHUD | `1386715964` / `714045101`, 자체 활성값 각각 0 | 비활성 보존과 생산 재활성/포커스 차단 |
| StageResultContent / StageModeRoot | `1501656893` / `901191967`, 자체 활성값 각각 1 | 사용자 적용 시 비활성 보존. 현재 이미 비활성이라고 기록하지 않음 |
| InfiniteModeRoot | `172987142`, 자체 활성값 1 | 참조 보존, Run 코드가 활성/초기화를 제어 |
| Goal 참조 | `1090618367`은 StageGoal 컴포넌트, 소속 GameObject는 `1090618365`(Goal) | 컴포넌트/Root/참조 삭제 없음 |
| GameSystem Selected Game Mode | 컴포넌트 `1202804280`, `_selectedGameMode: 1`(Infinite) | 코드 기본 Stage와 구분, 과거 값 정규화는 Test로 판정 |
| Mode Select Stage/Infinite/Back Button | `761562235` / `593037429` / `1247354411` | 참조 보존, 숨긴 버튼 경유 시작 제거 |
| InfiniteHUD / InfiniteResultContent | `264752311` / `1417628400` | 공통 HUD/결과 참조 유지 |

9개 UnityEvent는 모두 GameSystem `1202804280` 대상이며 현재 메서드 선언이 존재한다.
`593037429→SelectInfinite`, `677628985→SelectStartRun`, `761562235→SelectStage`,
`1242588979→SelectStage`(Leaderboard), `1363044890→SelectRetry`, `1467062858→SelectPlay`,
`1861140804→SelectRetry`, `2045885536→SelectInfinite`(Leaderboard), `2048432086→SelectResultLeaderboard`다.
Scene 전체 이벤트/Navigation 검증 완료라는 뜻은 아니며 최종 문구·입력 탐색·변경 후 참조는 Step 4/6/7에서 확인한다.

임시 Scene 검사식의 첫 실행은 서로 다른 이벤트 사이를 넘겨 매칭하여 assertion에 실패했다.
이벤트의 연속된 Target/Assembly/Method 필드로 검색 범위를 제한한 뒤 위 9건과 11개 참조 검사가 통과했다.
검사식 오류이며 생산 코드/Scene 실패로 분류하지 않는다. Scene을 수정하지 않았다.

Unity 버전은 `6000.3.5f2`다. `Windows-production.asset`은 `FLOW_STATE_PRODUCTION`, Development=0,
전역 Scene 목록 사용(`m_OverrideGlobalSceneList: 0`)이며 `EditorBuildSettings.asset`은 SampleScene 1개가 enabled=1이다.
이는 저장된 설정의 정적 기준선이다. 실제 활성 프로필/출력 경로·빌드/실행 성공은 미확인이고 Step 9 사용자 결과로 받는다.

#### 1-4. 영향 Test와 fixture 재분류 — 완료

`RestartInMode(E_GameMode.Stage)` 직접 호출은 현재도 13개 파일/각 1곳이다. 아래 이름은 PlayMode suite다.

| 분류 | 현재 호출 suite | Step 5 처리 기준 |
| --- | --- | --- |
| 공통 이동/입력/카메라 | `AutoMovementIntegrationTests`, `CameraFollowIntegrationTests`, `MomentumLandingIntegrationTests`, `PlayerJumpIntegrationTests`, `WallLandingRecoveryIntegrationTests` | Infinite 생산 구성 또는 책임별 독립 fixture. Stage 지형/좌표 기대는 개별 검토 |
| 공통 생명주기/메뉴 | `GameLifecycleIntegrationTests`, `GamePauseOrchestrationTests`, `PauseMenuIntegrationTests`, `ResultMenuIntegrationTests` | Infinite 시작·Pause/Retry/종료/초기화·입력/Timer 보존 |
| 수집/Stage 배치·Goal/충돌 | `CollectibleLifecycleIntegrationTests`, `StageCollectibleLayoutIntegrationTests`, `StageCollisionConfigurationTests`, `StageGoalIntegrationTests` | 공통 수집/충돌은 Infinite/독립 fixture, Stage 배치/Goal은 생산 Run 없는 합성 검증. 고정 지형을 일괄 치환하지 않음 |

`ProductionSceneGameModeTestUtility`는 현재 Play→Mode 선택→필요 시 StartRun을 실제 public Navigation API로 호출한다.
단일 Infinite 시작으로 helper를 바꾸고 명시적 Stage 거절 검증과 분리해야 한다.
13개 외에도 직접 Stage 생성/초기화가 있는 `StageSystemTests`, `ModeResultDisplayIntegrationTests`,
`ModeUISceneConfigurationTests`, `InfiniteHudIntegrationTests`, `InfiniteModeSystemTests` 및 EditMode
`GameRuntimeDataTests`, `MomentumProductionStateTests`, `InfiniteModeStateTests`를 영향 목록에 포함한다.
기본값/메뉴/UI·Result·최초 안내 suite는 Step 5 대응표와 Phase 1 5-4를 함께 사용한다.

`TryCreateStageCandidate` 호출 파일 10개도 확인했다. 공통 저장/계정 fixture는 EditMode
`AccountTransferControllerTests`, `AccountTransferCompletionTests`, `LocalSaveJsonCodecTests`,
`OnlineAccountCoordinatorTests`, `OnlineLocalSaveScopeTests`, `OnlineRecordRepositoryTests`와 PlayMode
`AccountTransferUIIsolationTests`이며, 기록/검증 책임은 EditMode `RecordLeaderboardPolicyTests`,
`RecordSubmissionPolicyTests`, `VerificationSessionTests`다. 함수 호출 총수와 파일 수는 구분한다.
생산 factory 차단은 Phase 3이며 Phase 2 공유 경계 변경으로 영향받는 fixture만 필요한 범위에서 전환한다.

PlayMode 공통 `PlayModeRecordIsolationSetup`의 setup/teardown, GameSystem 메모리 저장 분기 및 온라인 초기화 전 return을 확인했다.
preflight의 지정 suite 격리 검사도 통과했다. 향후 변경 Test의 transport/store 격리는 Step 5~6에서 재검사한다.
이번에는 C# Test를 실행하거나 새 Test를 작성하지 않았다.

#### 1-5. 로컬 기준선·잔여 한계·완료 판정 — 완료

| 이번 실제 실행 명령 | 결과 |
| --- | --- |
| `node UGS/Tests/client-unity-preflight.cjs` | 종료 코드 0. 변경/신규 C# 0개, asmdef/격리 검사 PASS. Edit 183/Play 23은 지정 suite의 소스 준비 수이며 미실행 |
| `node UGS/Tests/cloud-code.test.cjs` | 종료 코드 0, 제출/조회 SDK 대역 28/28 PASS |
| `node UGS/Tests/account-personal-bests.test.cjs` | 종료 코드 0, snapshot/이전 SDK 대역 18/18 PASS |
| Scene 읽기 전용 참조/이벤트 검사 | 수정한 검사식으로 11개 참조 대상/9개 지정 이벤트 PASS |
| 문서 링크/상태·`git diff --check` | 이번 기록 반영 후 검사 PASS |

세 지정 검사에서 기존 실패나 새 실패는 없었다. preflight의 변경 C# 검사 범위가 0개임을 명시하며 전체 C# 컴파일 성공으로 해석하지 않는다.
미확인 사항은 실제 Unity Compile·EditMode/PlayMode 실행·Scene 적용 후 화면/탐색·새 Player 결과다.
이는 후속 Step 7~9에서 확인할 항목이며 Step 1의 코드/직렬화 조사 누락을 사용자에게 전가하지 않는다.
Stage 차단·Deprecated 적용·fixture 변경은 Step 2 이후 미구현 상태로 남긴다.

현재 경로/영향 파일·Test/기준선과 미확인 범위를 기록했으므로 Step 1을 완료 처리한다.
현재 필요한 사용자 수동 작업은 없다. Unity Compile/Build/Test Runner·Scene/Prefab 편집·실제 저장/원격 요청은 수행하지 않았다.
다음은 Step 2 Infinite Navigation과 기본값 구현이다.

## Step 2. Infinite Navigation과 기본값을 구현한다

### AI 작업

1. `GameNavigationState.cs`와 `GameSystem.cs`의 Play·최초 안내·StartRun·재시작 요청을 단일 Infinite 흐름으로 연결한다.
2. 과거 Inspector/Runtime 선택·대기 값은 정상 시작 경계에서 Infinite로 정규화한다. 명시적 Stage 플레이 요청 및 정의되지 않은 Mode는 실행하지 않는다.
3. 기존 Tutorial 완료 복원/저장을 유지한다. 미완료이면 StartRun 전 Run 0건, 완료이면 Mode Select 없이 시작한다. Main Menu How To Play 재열람은 완료 상태를 바꾸지 않는다.
4. Main Menu/Play 복귀·Pause Settings 복귀·자동 안내 Cancel 무동작과 Run 요청 중복 방지를 유지한다.
5. `GameNavigationStateTests.cs`에 아래 상태 전환을 실제 생산 클래스로 검증한다.

| 입력/상태 | 기대 결과 |
| --- | --- |
| Boot/Play·Tutorial 미완료 | Main Menu 후 Infinite 자동 안내, Run 생성 0건 |
| StartRun·같은/인접 입력 sequence 중복 | Infinite 초기화 요청 1회, Run ID 중복 증가 없음 |
| Tutorial 완료 복원 후 Play | Mode Select 없이 Infinite 시작, 완료값 보존 |
| 자동 안내 Cancel·일반 How To Play Back/Cancel | 기존 상태/복귀 선택과 완료값 의미 유지 |
| 과거 Stage 선택/대기 값·직접 Stage 플레이 요청 | 정규화와 거절을 경계별로 구분, Stage 시작 0건 |
| 초기화 실패·재진입 | 기존 실패 복귀 보존, 실패 때문에 Stage 폴백하지 않음 |

### 사용자 수동 작업

없음. Compile/Runner 결과는 코드·Test 준비와 정적 검사를 마친 Step 7~8에서 받는다.

### 완료 조건

- [x] Navigation 생산 변경과 해당 Test를 준비했고 단일 시작/안내 전환표가 연결됐다. Test 실행 상태는 별도로 기록한다.

### Step 2 수행 결과 — 2026-11-12

#### 2-1. Infinite 시작·과거 값·명시적 요청 경계 구현 — 완료

- `GameNavigationState`의 선택/대기 기본값을 Infinite로 바꾸고 Play가 `SubmitInfinitePlay`로 직접 안내/초기화를 선택하게 했다. 마지막 Mode 기억 필드는 더 이상 사용하지 않으므로 제거했다. 기존 화면/enum/Stage 전용 API 소스는 보존한다.
- `TrySelect(Stage)`는 Leaderboard 외 화면에서 상태 변경 전에 거절한다. 보존된 ModeSelect의 Stage Submit도 거절하고 Infinite/Back/Cancel만 기존 의미에 맞춰 처리한다. ModeSelect의 표시/탐색 목록 자체 차단은 Step 4 대상이다.
- 과거 Stage 선택은 Play/Retry의 정상 시작 경계에서 Infinite로, 과거 대기 Stage는 StartRun에서 Infinite로 처리한다. 미정의 대기 Mode는 안내/완료값을 바꾸지 않고 거절한다. Tutorial 완료값은 기존 Restore/Save 흐름을 유지하며 일반 HowToPlay 재열람은 이를 변경하지 않는다.
- `GameSystem`의 코드 기본값과 매개변수 없는 StartGame 시작은 Infinite다. 과거 Inspector Stage는 정상 시작에서 Infinite로 정규화한다. 미정의 Inspector Mode의 StartGame은 정확한 Warning 후 상태 변경 없이 거절한다.
- 명시 인자가 있는 기존 private `BeginRunFromMenu`는 Stage/미정의 Mode를 Boot·선택 변경 전에 거절한다. 기존 매개변수/반환형은 유지하고 공개 실행 API를 추가하지 않았다.
- `HandleMainMenuSelection`이 완료 Tutorial의 Play 요청을 실제 `StartRequestedRun`으로 연결한다. 이 공통 시작 메서드는 Navigation의 유효한 Infinite 시작 요청을 먼저 확인한 뒤 선택값/Run 상태를 변경한다. Pause/Result Retry도 이 경계를 사용한다.
- 기존 SaveLocalState/RestoreAutomaticHowToPlayCompleted·Pause/Settings·실패 복귀/Run ID 증가 순서는 유지한다. 정상 시작의 과거 Stage 처리는 확정한 값 전환이며 초기화 실패 후 Stage 폴백을 추가하지 않았다.
- 공통 GameRuntimeData/RuntimeDataSystem/StageSystem의 직접 API 거절/기본값 변경은 Step 3, UI/안내 문구·Result 처리는 Step 4다. Stage 기록 탭/조회/제출과 Editor 온라인 probe는 Phase 3으로 남겼다.

#### 2-2. 실제 Navigation·GameSystem Test 소스 준비 — 완료

기존 `GameNavigationStateTests`의 ModeSelect/Stage 시작 전제를 Infinite Play/안내 fixture로 전환했다.
독립 Navigation 상태 클래스만 사용하며 과거 값은 Test의 reflection으로 주입한다. 생산 코드에 Test 전용 차단 예외는 없다.

| 입력/상태 | 구현/기대 결과 | 준비한 주요 Test |
| --- | --- | --- |
| Boot·미완료 Play | MainMenu 유지 후 Infinite 자동 안내, Run/요청 0건 | `CompleteBoot_InitialState_ShowsMainMenuWithoutRun`, `Play_TutorialIncomplete_OpensInfiniteGuideWithoutRun` |
| 완료값 복원 뒤 Play | ModeSelect/안내 없이 Infinite 초기화 요청, 완료값 보존 | `Play_TutorialRestored_RequestsInfiniteWithoutModeSelectOrGuide` |
| StartRun·같은/인접 sequence | 초기화 요청 1회, 성공 후 Run ID 1회 증가, 중복 완료 거절 | `AutomaticHowToPlay_StartRun_RequestsInfiniteOnce`, `StartRun_RepeatedSequenceAndAdjacentInput_CreateOneRun` |
| 안내 Cancel/일반 재열람 Back·Cancel | 안내 Cancel 무동작, 일반 재열람은 완료값 false/true 및 복귀 선택 보존 | `AutomaticHowToPlay_Cancel_IsRejectedWithoutMutation`, `MainMenuHowToPlay_Return_PreservesTutorialAndSelection` |
| 과거 선택/대기 Stage·Retry | Infinite 정규화, 미정의 대기 거절 | `LegacyStageSelection_Play_NormalizesToInfiniteGuide`, `LegacyStagePending_StartRun_NormalizesBeforeRequest`, `UnknownPending_StartRun_IsRejectedWithoutMutation`, `LegacyStageSelection_Retry_RequestsInfinite` |
| 명시적 Stage/미정의 항목·보존된 ModeSelect | 선택/요청/기존 Run 불변, Stage Submit 거절, Infinite/Back/Cancel 의미 유지 | `ExplicitPlaySelection_IsRejectedWithoutChangingGuide`, `StagePlaySelection_IsRejectedAndPreservesExistingState`, `LegacyModeSelect_*` |
| 초기화 실패·재진입 | MainMenu 복귀·완료값 유지·Run 0건, 이후 Infinite 요청·성공 시에만 ID 증가 | `InitializationFailure_PreservesCompletionAndReentryRequestsInfinite` |
| Pause/Settings·Result·순위 복귀 | 같은 Run/복귀 선택과 Infinite Retry 보존. Stage 기록 선택은 유지 | 기존 Pause/Result/Leaderboard Test의 공통 판정 유지 |

실제 SampleScene/GameSystem 연결은 기존 `GameEntryBootIntegrationTests`에 다음 검증을 준비했다.

- `Play_CreatesInfiniteOnlyAfterGuideStartRun`: Play 직후 안내/Run 0건·명시 Stage/미정의 항목 무효·StartRun 후 실제 Infinite Runtime.
- `CompletedTutorial_Play_StartsInfiniteOnceWithLegacyInspectorSelection`: 과거 Inspector Stage와 복원 완료값에서 Play 즉시 Infinite, 같은/다음 프레임 중복 선택에도 Runtime 객체/Run ID 유지.
- `StartGame_LegacyInspector_OpensGuideAndPersistsCompletionOnce`: 매개변수 없는 시작→안내→Infinite, 메모리 저장소 재로드로 Tutorial 완료값 저장 확인, 중복 StartRun은 같은 Runtime 유지.
- `ExplicitStageAndUnknownRequest_DoNotChangeMenuOrCreateRuntime`: 명시 Stage/미정의 메뉴 인자 거절, 요청 없는 직접 초기화 무동작, 미정의 Inspector StartGame 거절.
- 기존 Pause/Settings/확인 취소·MainMenu 정리 Test의 시작 fixture를 Infinite로 전환했다. `HowToPlayIntegrationTests.FirstPlay_BlocksRunUntilStartRun`도 ModeSelect 호출 없이 검증하게 바꿨다.
- 두 PlayMode suite에 UnityTearDown의 SampleScene unload를 추가해 Run/객체/입력·콜백이 다음 Test에 남지 않게 준비했다. 실제 Scene 파일은 수정하지 않았다. 기존 전역 PlayMode 메모리 저장/온라인 초기화 건너뛰기 격리를 그대로 사용한다.

작성 소스 기준은 EditMode `GameNavigationStateTests` 57건(기존 39건), PlayMode `GameEntryBootIntegrationTests` 7건(기존 4건),
`HowToPlayIntegrationTests` 3건(기존 3건)이다. 합계 67건은 attribute 기준 준비 수이며 Unity Test Runner 실행/통과 수가 아니다.
예상 로그는 미정의 Inspector 거절 Test의 `[GameSystem] Game Mode is invalid.` Warning 1개만 정확히 Expect한다.
그 밖의 예상 밖 Error/Warning은 허용하지 않으며 광범위한 로그 억제는 추가하지 않았다.

#### 2-3. 정적/로컬 검증·문서 반영 — 완료

| 이번 실제 실행 | 결과 |
| --- | --- |
| `node UGS/Tests/client-unity-preflight.cjs` | PASS, 변경 C# 5개·중복 멤버/구분자/meta/asmdef·기존 격리 검사. 출력 Edit 183/Play 23은 별도 지정 suite의 소스 준비 수 |
| `node UGS/Tests/cloud-code.test.cjs` | SDK 대역 제출/조회 28/28 PASS |
| `node UGS/Tests/account-personal-bests.test.cjs` | SDK 대역 snapshot/이전 18/18 PASS |
| `node UGS/Tests/static-contracts.cjs` | 기존 서버/공통 계약 정적 검사 PASS. 출력의 과거 Phase/Step 명칭을 Roadmap 9 구현 실행 결과로 해석하지 않음 |
| `node UGS/Tests/client-local-save-contracts.test.cjs` | v7/격리/기존 저장 계약 정적 검사 PASS. 출력 C# Test 준비 수는 미실행 |
| 문서 링크/Step 체크·소스 차이/영향 검사·`git diff --check` | PASS |

생산 변경은 `GameNavigationState.cs`, `GameSystem.cs` 두 파일이며 Test 변경은 위 세 suite다. 새 C# 파일/asmdef/enum 번호/GUID 변경은 없다.
생산 코드의 시작 요청 guard가 필수 참조/Runtime 생성/상태 변경보다 앞서는지, Play/안내/Retry의 실제 호출 연결과 Stage 기록 경계 보존을 정적으로 대조했다.
관련 `GameEntryNavigation`, `HowToPlay`, `GameSystem` 문서는 반영한 시작 계약만 현행화하고 미완료 UI/직접 API/Unity 검증을 구분했다.
Scene/Prefab·Build Profile·실제 저장/UGS/Module·점수/Pattern/물리는 변경하지 않았다.

#### 2-4. 완료 판정·수동 작업과 후속 인계 — 완료

Step 2의 완료 조건인 Navigation 생산 변경·해당 Test 준비·전환표 연결을 충족했으므로 Step 2를 완료 처리한다.
Unity Compile/EditMode/PlayMode는 미실행이며 성공으로 기록하지 않는다. Phase 2 전체는 미완료다.

현재 사용자 수동 작업은 없다. 공통 생성/Stage API 차단은 Step 3, UI/문구는 Step 4, 기존 Stage helper 13파일과 나머지 fixture 전환은 Step 5에서 계속한다.
아직 이전 ModeSelect/Stage 시작을 기대하는 나머지 suite는 실행 준비 완료로 넘기지 않는다. 전체 영향 목록과 실제 실행 필터는 Step 6~8에서 확정한다.
사용자 Compile/Scene 적용/Test Runner/Player 확인은 Step 7~9에 유지한다. 지금 Scene 버튼/문구/Inspector를 바꾸거나 수동 Stage 재현·빌드를 할 필요는 없다.
최종 Scene 적용 시 기존 SelectPlay/SelectStartRun/SelectRetry 어댑터를 유지하는 방향이며 정확한 적용표는 Step 4/6 뒤 제공한다.
다음은 Step 3 공통 생성·실행 경계와 Deprecated 처리다.

## Step 3. 공통 생성·실행 경계와 Deprecated 처리를 구현한다

### AI 작업

1. `GameRuntimeData.cs`, `RuntimeDataSystem.cs`, `StageSystem.cs`, `GameSystem.cs`의 무인자 기본값을 Infinite 흐름에 맞춘다.
2. 명시적 Stage 생산 생성/초기화/시작은 **부작용 전에** 거절한다. 특히 StageSystem의 Collectible/listener 해제·Root 활성·상태 초기화 이전에 검사한다.
3. 기존 API 반환/상태 계약에 맞게 거절한다. bool이면 false, void 등은 기존 Run/요청/화면을 훼손하지 않아야 한다. 새 구조/시그니처 변경이 필요하면 기존 구조로 해결할 수 없는 근거와 호출자 영향을 먼저 기록한다.
4. `InfiniteModeState.cs`의 초기화 전/Reset 상태를 점검한다. 단순히 내부 값이 Stage라는 이유로 활성 Infinite Run이나 enum 번호를 잘못 바꾸지 않는다.
5. `StageGoal.cs`, Stage 전용 결과/TimeRecord 경계와 관련 선언을 Deprecated/주석으로 남긴다. 공통 이벤트·Timer·Result·Collectible·Pattern·World Rebase는 유지한다.
6. `GameRuntimeDataTests.cs`, `StageSystemTests.cs` 및 관련 실제 클래스 Test에서 미초기화/Ready/Playing/Paused/Result별 Stage 거절과 기존 상태·Root·listener·Collectible scope 불변을 확인한다.
7. 명시적 거절 진단을 Test로 다룰 때 요구된 로그만 정확히 기대한다. 예상 밖 Error/Warning을 통과시키려고 광범위한 `LogAssert.Expect`나 `#pragma`를 사용하지 않는다.

### 사용자 수동 작업

없음. StageRoot/Goal 컴포넌트나 참조를 삭제하지 않는다.

### 완료 조건

- [x] Stage 거절 위치·소스 보존/사유·공통 생명주기와 상태 불변 Test가 준비됐다.

### Step 3 수행 결과 — 2026-11-12

#### 3-1. 생성/초기화·시작의 거절 계약과 위치 확정 — 완료

기존 API 시그니처를 유지하고 아래 경계를 구현했다. 명시적 Stage 요청을 Infinite 성공으로 바꾸지 않는다.

| 실제 경계 | 기본값/거절 결과 | 보호하는 선행 부작용 |
| --- | --- | --- |
| `GameRuntimeData.Initialize()` / `Initialize(E_GameMode)` | 무인자·초기/정리 후 Mode는 Infinite. Stage/미정의 인자는 void 반환으로 거절 | 기존 Collectible Clear·객체 교체·이동/Infinite 데이터/상태 변경 전 return |
| `RuntimeDataSystem.CreateRuntimeData()` / 인자 overload | 무인자 Infinite. Stage/미정의 요청은 Warning과 null | 기존 Run 반환/새 객체 생성 전 거절. 거절을 기존 Run 반환으로 위장하지 않으며 RuntimeData 참조는 유지 |
| `StageSystem.Initialize()` / 인자 overload | 무인자와 초기 Mode는 Infinite. Stage는 Warning/false, 미정의 Mode는 기존 invalid Error/false | Collectible 해제·Goal listener 제거·초기화 플래그/Mode·Root 활성/재초기화 전 거절 |
| `StageSystem.StartStage()` | 보존된 Stage/미정의 현재 Mode는 Warning/false | 초기화/중복 실행 검사와 ResetStageState·Goal Reset·Collectible binding·시작 이벤트 전 거절 |
| `StageSystem.ConfigureCollectibles` | Infinite Runtime만 수락 | 명시적/합성 Stage Runtime을 통한 생산 binding 진입을 거절, 기존 참조 불변 |
| `InfiniteModeSystem.Initialize(E_GameMode)` | Stage/미정의 요청은 Warning/false | 기존 진행 참조·Momentum/점수·Pattern/World Rebase·초기화 플래그 정리 전 거절 |
| `GameSystem`의 시작 | Step 2 Infinite 코드 기본/유효 요청 guard 유지 | 메뉴/ContextMenu/Retry에서 정상화한 Infinite 인자만 공통 생성/초기화로 전달 |

추가 조사에서 InfiniteModeSystem도 Stage 요청을 받은 뒤 활성 Infinite 상태를 먼저 정리할 수 있음을 확인해 같은 실행 경계로 보강했다.
새 구조/서비스/API 또는 Test 전용 생산 예외는 추가하지 않았다. Core GameRuntimeData는 기존 void 계약과 Unity 비의존성을 유지하며 정상적인 인자 거절에 Unity 로그 의존성을 만들지 않았다.
실제 컴포넌트 경계의 거절 진단은 아래 Test의 정확한 로그 기대와 연결했다.

#### 3-2. 비권장 소스/선언과 공통 상태 보존 — 완료

- `StageGoal`의 컴포넌트/필드/trigger 소스·GUID를 보존하고 Infinite에 Goal이 없다는 사유/대체 종료 경로를 주석으로 남겼다. `OnTriggerEnter`에 Obsolete를 적용했다.
- StageSystem의 Stage 전용 `FixedUpdate` 추락 callback과 `AddStageClearedListener`/`RemoveStageClearedListener`를 Obsolete로 표시했다. Goal 초기화/완료·추락·Stage Root/고정 수집 분기 소스는 삭제하지 않았다.
- `E_GameMode.Stage`·`E_StageResultType`, `StageRecordIdentity`의 비권장 사유/보존 범위를 주석으로 명시했다. enum 값 Stage=0/Infinite=1 및 기존 Stage ID/규칙 번호를 바꾸지 않았다.
- `TimeRecord`, Stage 전용 ResultData 생성자, ResultSystem의 Stage factory/모델과 GameSystem의 Stage 결과 분기를 사유 주석으로 보존했다. Infinite는 ScoreRecord 경로를 사용하며 공통 Result/Timer를 비권장으로 만들지 않는다.
- Obsolete 4개 선언의 정적 호출 검색에서 직접 형식 호출을 추가하지 않았음을 확인했다. 보존된 컴포넌트 타입·enum·독립 결과 factory 전체에 Obsolete를 붙여 기존 강한 형식 참조/합성 Test에 경고를 전파하지 않았다. 이 부분은 사유 주석으로 비권장임을 명시하며 전역/국소 pragma 경고 억제는 추가하지 않았다. 실제 컴파일 경고 여부는 사용자 Compile 결과로 판정한다.
- `InfiniteModeState`의 초기/Initialize Stage 값은 비활성 sentinel로 유지하고 사유를 주석으로 남겼다. 생산 InfiniteModeSystem은 Infinite 설정 후 Start한다. 기존 `NewState_DefaultMode_IsStage`, `SetGameMode_DuringPlay_IsRejected`, `Reset_AfterEnd_ClearsRunStateAndRetainsMode`의 독립 상태 의미를 재확인했다. Reset은 활성 Mode 값을 Stage로 되돌리지 않는다.
- StageSystem의 Infinite 종료/일시정지/이벤트/Root·Collectible/Pattern 공통 생명주기와 InfiniteModeSystem의 계산·World Rebase/점수 규칙은 유지했다. Scene/Prefab·필수 참조를 삭제/편집하지 않았다.

#### 3-3. 거절 후 상태 불변과 공통 생명주기 Test 준비 — 완료

| 기존 실제 클래스/suite | 이번 준비한 검증 |
| --- | --- |
| EditMode `GameRuntimeDataTests` | 무인자 Infinite, Clear 후 Infinite/미생성. Stage/미정의 요청을 최초/정리 후 및 None·Ready·Playing·Paused·Ended(Result UI) 상태에 적용해 이동/Infinite/Collectible 객체·점수/최종화·scope/미수집 항목/상태/UI 보존 확인 |
| PlayMode `StageSystemTests` | 무인자 Infinite/Goal 초기화 안 함. None(미초기화)·Ready·Playing·Paused·Ended에서 Stage/미정의 초기화와 과거 Mode의 StartStage 거절. 모든 private 필드·Root 활성·실제 Goal handler delegate·bound ScoreCollectible/list·scope 보존, 거절 뒤 실제 기존 scope 획득 가능 |
| 같은 `StageSystemTests`의 RuntimeDataSystem 검증 | 위 다섯 상태에서 거절 반환 null과 기존 Runtime 참조/상태/UI/scope 보존. 거절 뒤 유효 Infinite 요청은 기존 객체를 반환하고 빈 System의 무인자 생성은 Infinite 성공 |
| PlayMode `InfiniteModeSystemTests` | Stage 요청 후 실제 Infinite 거리/점수/진행 보존과 이후 진행 갱신. Stage/미정의 요청을 활성/Stop 후 상태에 적용해 필드 불변. 참조 없는 미초기화 System도 기존 준비 작업 전에 거절 |
| PlayMode `GameEntryBootIntegrationTests.StageRequests_WhilePlayingAndPaused_PreserveProductionPatternAndRun` | 실제 SampleScene의 Infinite Playing/Paused에서 Runtime 생성·Stage/Infinite 초기화의 Stage 요청 거절. 같은 Runtime/Run ID/StageEnded 구독·활성 Root/Pattern 참조/수집 scope/일시정지 보존과 Resume. 요청 없는 GameSystem 직접 초기화도 무동작 |

StageSystemTests의 기존 Goal/추락/같은 구간 Goal 우선 경계는 생산 Stage Run을 만들지 않는 독립 합성 snapshot으로 전환했다.
Test reflection은 비권장 상태의 관찰/trigger·FixedUpdate 판정에만 사용한다. Stage Initialize/Start를 성공시키는 helper나 생산 guard 예외를 만들지 않았다.
과거 Clear/추락 뒤 Stage 재시작 성공 기대는 거절/기존 결과 불변으로 바꾸고, 실제 Infinite 재시작/종료 1회·Pause/Resume Test를 유지했다.
GameRuntimeDataTests의 Stage 생성 성공 조합은 Infinite의 공통 데이터 교체 검증과 Stage 거절/보존 사례로 분리했다.

현재 attribute 기준 준비 수는 `GameRuntimeDataTests` 19건, `StageSystemTests` 36건, `InfiniteModeSystemTests` 17건,
`GameEntryBootIntegrationTests` 8건으로 합계 80건이다. 이는 변경 suite 전체의 준비 수이며 신규 건수나 Unity 실행 PASS 수가 아니다.
Step 2 당시 GameEntry suite는 7건이었고 이번에 1건 추가했다. 기존 InfiniteModeState·TimeRecord/Result 합성 Test도 후속 실행 영향 목록에 유지한다.

정확히 기대하는 새 거절 로그는 다음과 같다. 과거 중복 시작/Goal 누락 등의 독립 검증에서 필요했던 기존 기대 로그는 해당 사례의 책임에 맞춰 유지/변경했다.

| 경계 | 기대 로그 |
| --- | --- |
| RuntimeDataSystem Stage/미정의 생성 | Warning `[RuntimeDataSystem] Only Infinite Mode runtime data can be created.` |
| StageSystem Stage 초기화 | Warning `[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.` |
| StageSystem 미정의 초기화 | Error `[StageSystem] Game Mode is invalid.` |
| StageSystem 보존된 Stage/미정의 상태 시작 | Warning `[StageSystem] Only Infinite Mode can be started.` |
| InfiniteModeSystem Stage/미정의 초기화 | Warning `[InfiniteModeSystem] Only Infinite Mode can be initialized.` |

각 호출의 요구된 로그만 Expect하며 예상 밖 Warning/Error를 광범위하게 허용하지 않는다.
Test는 기존 PlayMode 메모리 저장/온라인 초기화 차단 격리를 사용하고 새 실제 인증/transport/file store를 연결하지 않았다.
독립 fixture는 TearDown에서 생성 객체를 파괴하고 Scene 통합 Test는 기존 UnityTearDown에서 Scene을 unload하도록 준비했다.
AI는 실제 Scene 로드·Test Runner를 실행하지 않았다.

#### 3-4. 정적/로컬 검증·영향 문서 — 완료

| 이번 실제 실행/대조 | 결과 |
| --- | --- |
| `node UGS/Tests/client-unity-preflight.cjs` | PASS. 누적 변경 C# 19개(Step 2 포함)·중복 멤버/구분자/meta/asmdef·기존 격리 검사. Edit 183/Play 23 출력은 별도 지정 suite의 미실행 소스 수 |
| `node UGS/Tests/cloud-code.test.cjs` | 메모리/SDK 대역 제출·조회 28/28 PASS |
| `node UGS/Tests/account-personal-bests.test.cjs` | 메모리/SDK 대역 snapshot·이전 18/18 PASS |
| `node UGS/Tests/static-contracts.cjs` | 기존 식별자/서버/공통 계약 정적 검사 PASS. 출력의 과거 Phase/Step 이름을 Roadmap 9 Unity 실행 결과로 취급하지 않음 |
| `node UGS/Tests/client-local-save-contracts.test.cjs` | v7/격리/기존 저장 계약 정적 검사 PASS. 출력 27건은 별도 C# suite의 소스 준비 수이며 미실행 |
| 읽기 전용 guard 순서/식별자 검사 | Runtime/RuntimeSystem/Stage Initialize·Start/Infinite Initialize의 거절 위치가 해제·생성·상태 변경보다 앞섬. enum/ResultData/StageRecordIdentity는 주석 제외 기존 token 동일 |
| 문서 링크/상태·변경 소스/meta/Scene 경계·`git diff --check` | PASS |

이번 Step에서 생산 12파일(11파일 추가 수정과 기존 GameSystem의 사유 주석)과 Test 4파일을 변경했다. Step 2부터 누적 C# 변경은 19파일이다.
새 C# 파일·시그니처/asmdef/meta/GUID 변경은 없다. 관련 StageSystem/RuntimeDataSystem/InfiniteModeSystem/ResultSystem/GameSystem과
GameEntryNavigation/StagePlay/StageClear/TimeRecord 문서를 실제 생산 경계·보존된 합성 모델·미실행 한계에 맞춰 갱신했다.
새 물리/점수/Pattern/서버 계약이나 저장/온라인 기록 정책을 추가하지 않았다.

#### 3-5. 완료 판정·수동 작업과 후속 인계 — 완료

Step 3의 완료 조건인 부작용 전 거절·소스/사유 보존·공통 생명주기 및 상태 불변 Test 준비를 충족했으므로 완료 처리한다.
실제 Unity Compile/Runner·새 Player 근거는 아직 없으며 전체 Stage 접근 차단/Phase 2 완료로 확대하지 않는다.

현재 사용자 수동 작업은 없다. Scene의 StageRoot/Goal 컴포넌트·참조를 삭제하거나 지금 값을 바꿀 필요는 없다.
Step 4에서 UI/Result/문구·개발용 시작 경계를 정리하고, Step 5에서 나머지 Stage helper 13파일·기본 생성에 의존한 기존 fixture를 책임별로 전환한다.
아직 기존 Stage 생성 성공을 기대하는 전체 suite는 사용자 실행 준비 완료 상태가 아니다. Step 6 이후 정확한 Compile/Scene 적용표·영향 suite를 안내하고 사용자 실행은 Step 7~9에서 받는다.
Phase 3 Stage 기록 탭/직접 조회·제출·온라인 probe와 Infinite 저장/계정 이전 회귀는 미구현 인계로 유지한다.
다음은 Step 4 UI·안내·Result와 개발용 플레이 경로 연결이다.

## Step 4. UI·안내·Result와 개발용 플레이 경로를 연결한다

### AI 작업

1. `UIVisibilityState.cs`, `UIManagementSystem.cs`의 Mode Select·StageHUD·StageResultContent 생산 표시/선택을 차단하고 필수 참조 검사를 새 메뉴 계약에 맞춘다.
2. InfiniteHUD·ResultPanel·Pause/Settings/How To Play·공통 버튼/Binding 참조를 유지한다. 숨긴 UI가 마우스/Keyboard Navigation이나 이전 선택 복원으로 다시 활성화되지 않게 한다.
3. `GameSystem`의 ContextMenu/public StartGame·Pause/Result Retry와 `ProductionSceneGameModeTestUtility`의 실제 시작 경계를 대조한다. Editor 도구에 플레이 우회가 있으면 차단한다.
4. Stage 전용 Result/formatter 소스는 비권장으로 보존한다. Infinite 결과 확정·Timer 정리·HUD 최종값·새 Run 초기화는 유지한다.
5. 안내의 최종 문구와 Scene 적용 목록을 준비한다. 자동 이동·Jump·Momentum Landing·Collectible·거리/점수·Pause·Infinite 종료를 설명하며 Stage Goal/Clear Time 설명은 제공하지 않는다.
6. 관련 Feature/System 문서는 실제 완료한 부분만 현행 규칙으로 갱신한다. Phase 3의 Stage Tab/직접 기록 조회·제출/온라인 probe 부분은 미구현으로 남긴다.

### 사용자 수동 작업

없음. Scene Text나 Button 연결을 AI가 변경하지 않고 Step 7의 적용 목록으로 제공한다.

### 완료 조건

- [x] UI/Result/플레이 우회 코드와 Test·최종 Scene 변경 목록/안내 문구가 준비됐다. 기록 접근의 Phase 3 경계가 명시됐다.

### Step 4 수행 결과 — 2026-11-12

#### 4-1. UI 표시/선택·필수 참조와 Result/개발용 경계 구현 — 완료

- `UIVisibilityState`는 Infinite만 유효한 표시 Mode로 수락한다. Stage/미정의 요청은 모든 표시 flag를 숨기고 false를 반환하며 StageHUD/StageResultContent를 표시하지 않는다.
- `UIManagementSystem`은 Menu 기본 Mode를 Infinite로 유지한다. ModeSelect 직접 표시 요청은 MainMenu/Play 표시로 정규화하고 비권장 패널/Stage HUD·Result Root는 초기화/화면 전환/상태 반영/결과 표시 및 Update에서 숨긴다.
- 보존된 Mode Select 세 버튼은 비활성·interactable=false·Navigation=None으로 유지한다. 숨긴 Root/버튼 아래의 EventSystem 현재 선택을 해제하고 `GetNavigationButton(ModeSelect, ...)`는 null을 반환한다. 이전 Focus 복원과 Keyboard/Mouse 진입은 생산 요청 guard와 함께 차단한다.
- `HasNavigationUIConfiguration`의 필수 목록에서 ModeSelect 패널/세 버튼을 제외했다. InfiniteHUD/InfiniteResultContent와 Pause/Result·Settings/안내의 공통 참조·Binding은 유지한다. Stage HUD/Result 참조는 없어도 경고 없이 숨긴다. 추가 정책(4-6)에 따라 Result 레이아웃은 Scene 값만 사용한다.
- 생성되지 않았거나 Infinite가 아닌 Runtime의 UI 초기화는 기존 Infinite 표시를 변경하기 전에 거절한다. Stage `SetResultData`도 기존 Infinite Text 갱신 전에 Warning/false로 거절한다. Stage formatter/레이아웃/HUD 분기 소스·직렬화 참조는 사유 주석과 함께 보존한다.
- `GameSystem.HandleStageEnded`의 비 Infinite Runtime callback은 Timer 정지·결과/기록 처리 전에 거절한다. Infinite 종료 확정·공통 Timer 제거·Result 최종값·새 Run 초기화의 기존 순서는 유지한다. `ResultTextFormatter`의 Stage 문구/모델은 비권장 사유를 명시하고 독립 formatter Test를 유지한다.
- ContextMenu/public StartGame·Pause Retry·Result Retry는 Step 2~3의 유효 Infinite 요청/생성 경계를 사용한다. 별도 플레이 우회 호출은 Editor 소스 검색에서 발견하지 않았다. Editor 검증 창의 Stage 조회/제출/probe는 플레이 시작 API가 아니며 Phase 3에 남긴다.
- `ProductionSceneGameModeTestUtility.RestartInMode`는 Stage/미정의 요청을 기존 Run 종료/선택 변경 전에 assertion으로 거절한다. 유효 Infinite는 Play→필요 시 StartRun을 호출하며 실제 Runtime Mode도 확인한다. Stage 요청이 조용히 Infinite를 시작하는 Test fixture는 허용하지 않는다.
- Leaderboard의 Stage 탭/색상/직접 기록 조회·제출/온라인 probe와 저장·계정 계약은 이번에 변경하지 않았다. Phase 2 UI 차단은 플레이/결과 경계이며 Phase 3 완료가 아니다.

#### 4-2. 실제 UI/생산 실행 Test 준비 — 완료

| suite | 준비한 내용 |
| --- | --- |
| EditMode `UIVisibilityStateTests` | Infinite 표시/Reset/유효성 검증 유지. Stage Playing/Result 거절과 None·Initializing·Ready·Playing·Paused·Ending·Ended에서 이전 Infinite 표시 flag 제거 |
| PlayMode `ModeResultDisplayIntegrationTests` | Stage Cleared/Fell 결과 거절·공백 유지, 합성 Stage Runtime/결과 요청 후 기존 Infinite 최종 Text 보존. ModeSelect/Stage Result 하위 선택 해제, 여러 화면 요청에서 숨긴 버튼 비활성/탐색 제외·재활성 방지 |
| PlayMode `ModeUISceneConfigurationTests` | 실제 Scene의 Stage 표시 성공 기대를 거절/비활성 보존으로 전환. Infinite Playing/Pause/Result와 최종 Text·공통 버튼/레이아웃 유지. ModeSelect/Stage HUD·Result 참조를 잠시 제외해도 필수 구성과 Infinite Result 레이아웃 동작 검증, finally에서 참조 복원 |
| PlayMode `GameEntryBootIntegrationTests` | helper Stage/미정의 요청이 현재 Run/ID를 변경하지 않는지, Pause Retry·실제 Infinite 추락 종료 후 Result Retry/Context Start가 새 Infinite Runtime/Timer를 만들고 이전 점수/Result를 초기화하는지. 최종 Result Text·Timer 제거/새 Timer Running 확인. 합성 Stage callback 거절 시 Timer/Run/Result 보존 |
| 기존 EditMode `ResultTextFormatterTests`/`ResultSystemTests` | 비권장 독립 결과 모델/formatter 합성 검증 유지. Stage formatter의 존재를 생산 UI 접근으로 해석하지 않음 |

최초 Step 4의 source attribute 준비 수는 UIVisibility 23건·ModeResultDisplay 8건·ModeUISceneConfiguration 4건·GameEntryBoot 11건으로 합계 46건이었다. 4-6에서 Scene 구성 선존재 검증을 1건 추가해 ModeUISceneConfiguration은 5건, 합계 47건이다.
이는 관련 변경 suite의 전체 준비 수이며 신규/실행 PASS 수가 아니다. 공통 helper는 attribute Test 수에 포함하지 않는다.
ModeUISceneConfiguration에 UnityTearDown Scene unload를 추가했고 독립 UI fixture는 이전 EventSystem 선택을 복원하고 생성 객체를 정리한다.
기존 메모리 저장/온라인 초기화 차단 격리를 유지하며 실제 인증/UGS/사용자 영구 저장을 연결하지 않았다.

정확히 기대하는 추가 Warning은 다음 세 진단이다. helper의 지원되지 않는 Mode assertion은 `Assert.Throws<AssertionException>`로만 검증하며 Unity 로그를 허용하지 않는다.

```text
[UIManagementSystem] Only created Infinite runtime data can be displayed.
[UIManagementSystem] Only Infinite Result Data can be displayed.
[GameSystem] Only an Infinite Run can produce a gameplay result.
```

예상 밖 Error/Warning 허용이나 pragma 억제는 추가하지 않았다. 기존 Stage 시작 helper 호출 13파일과 나머지 Stage 성공 fixture 전환은 Step 5 대상이므로 전체 suite 실행 준비 완료로 넘기지 않는다.

#### 4-3. 최종 Scene 적용표·Infinite 안내 문구 준비 — 완료

최종 Scene 적용표·Infinite 안내 문구를 준비했다. 상세 객체/Binding/버튼 연결과 최종 Text는 Step 7-2에 한 번만 보관한다. 현재 Scene 적용은 대기이며 Step 5~6 이후 수행한다.

#### 4-4. 정적/로컬 검사·문서 반영 — 완료

| 이번 실제 실행/대조 | 결과 |
| --- | --- |
| `node UGS/Tests/client-unity-preflight.cjs` | PASS. 누적 C# 26파일의 중복 멤버/구분자/meta/asmdef와 기존 격리 검사. 출력 Edit 183/Play 23은 별도 지정 suite의 미실행 준비 수 |
| `node UGS/Tests/cloud-code.test.cjs` | SDK 대역 제출/조회 28/28 PASS |
| `node UGS/Tests/account-personal-bests.test.cjs` | SDK 대역 snapshot/이전 18/18 PASS |
| `node UGS/Tests/static-contracts.cjs` | 기존 식별자·서버/공통 계약 PASS. 과거 출력 Phase 이름은 이번 Unity 실행 결과가 아님 |
| `node UGS/Tests/client-verification-tools-contracts.test.cjs` | Editor 도구/격리의 기존 소스 계약 PASS. 출력 Edit 60/Play 14는 미실행 준비 수 |
| `node UGS/Tests/client-local-save-contracts.test.cjs` | v7/격리/기존 저장 정적 계약 PASS. 출력 27건은 미실행 C# 준비 수 |
| UI/Result/helper guard·Scene 읽기 전용 대조 | Stage 결과 guard가 Text/Timer 변경보다 앞섬. helper의 Mode 검사가 기존 Run 종료보다 앞섬. Scene 지정 이벤트 9개와 Text/Binding/Root 기준선·계층/적용표 일치 |
| 문서 링크/Step 체크·source counts·경계·`git diff --check` | PASS |

추가 진단에서 출력 축약용 child-process wrapper에 Git 환경 변수를 별도 지정한 실행은 `Could not access HEAD`로 실패했다.
해당 wrapper를 사용하지 않고 원래 `node UGS/Tests/client-unity-preflight.cjs`를 다시 실행해 PASS와 HEAD `6c4f71d`를 확인했다. 저장소/생산 소스나 검사 스크립트를 변경해 우회하지 않았고 이 실패를 Unity/C# 실행 실패로 분류하지 않는다.

생산 변경은 UIVisibilityState/UIManagementSystem/GameSystem/ResultTextFormatter 4파일, Test/helper는 5파일이다. 신규 C# 파일이나 meta/asmdef 변경은 없다.
관련 UIManagementSystem/GameSystem/ResultSystem·HowToPlay/ResultMenu 문서의 실제 반영 부분을 현행화하고 Scene 미적용·Phase 3 기록 경계를 명시했다.
Scene/Prefab/Build Profile·물리/Pattern/점수·저장/서버 계약·실제 UGS/사용자 저장은 변경하지 않았다.

#### 4-5. 완료 판정·수동 작업과 후속 인계 — 완료

Step 4의 완료 조건인 UI/Result/플레이 우회 코드·해당 Test·최종 Scene 적용표/문구 준비와 Phase 3 경계 명시를 충족했으므로 완료 처리한다.
실제 Unity Compile/Runner·Scene 적용·Player 근거는 아직 없으며 Phase 2 전체나 Stage 기록 접근 차단을 완료 처리하지 않는다.

현재 사용자 수동 작업은 없다. 추가 UI 정책의 Scene 구성 적용도 Step 7로 이동했으며, Step 5~6을 마친 후 7-2~7-4의 통합 목록을 수행한다. Unity 실행 결과를 준비 완료로 대신하지 않는다.
다음은 Step 5의 기존 fixture 목적별 전환이다. 강화한 생산 helper의 Stage 호출 13파일과 직접 초기화/표시 성공 기대·저장/계정 fixture 영향을 개별 처리한다.
공통 helper가 바뀌었으므로 Step 6~8 실행 목록에는 영향을 받는 전체 suite를 포함하고 필터 밖 실패를 숨기지 않는다.
최종 사용자 Compile/Scene 적용은 Step 7, Runner는 Step 8, Player/화면은 Step 9에서 안내한다.

#### 4-6. 사용자 추가 요청 — UI 생성 금지·Editor 구성 유지

2026-11-12 사용자 추가 지시로 이 절이 앞선 Runtime 레이아웃 설명보다 우선한다.
UI의 구조/컴포넌트/고정 배치는 Scene/Prefab에서 관리하며 Runtime은 기존 UI의 Text/활성/포커스/입력/스크롤 상태만 갱신한다.

**AI 코드 준비 — 완료, Scene 반영/Unity 검증 — 미완료:**

- UIManagementSystem의 `AddComponent<Outline>`와 `ApplyResultLayoutForCurrentMode`/`TryApplyExpandedResultLayout`/`SetResultRect`를 제거했다. Result 위치/크기/Anchor/Pivot/부모는 Runtime에서 덮어쓰지 않는다. Outline도 생성/스타일 보정하지 않고 Scene 컴포넌트의 선택 enabled 상태만 갱신한다.
- 기존 SettingsUIController의 Hover/Rebind handler 자동 AddComponent와 Runtime 참조 주입을 제거했다. Editor에서 붙일 수 있도록 `SettingsButtonHoverFocusHandler.cs`, `SettingsRebindButtonFocusHandler.cs` 및 고유 meta를 분리했다. Controller/Button/Index는 Inspector에서 저장하고 생산 코드는 일치 여부만 검증한다. 기존 SettingsUIController의 GUID는 유지했다.
- 이 Phase에서 변경한 ModeResultDisplayIntegrationTests의 `new GameObject`/UI AddComponent fixture를 제거하고 UnitySetUp에서 실제 SampleScene의 기존 UI를 참조한다. ModeUISceneConfigurationTests에 컴포넌트 선존재/저장 참조·사용 전후 컴포넌트 수 유지 검증을 추가했다. Test는 Scene 메모리 상태만 일시 변경하고 종료 시 unload한다.
- Runtime 전체 검색에서 남은 Instantiate는 InfinitePatternSlot의 맵 Pattern 생성이며 UI가 아니다. 생산 UI GameObject/컴포넌트 생성은 남기지 않았다. UI Text/선택 색상·활성/스크롤 등의 상태 갱신은 유지한다.
- Step 4 최초 작업에서 UIManagementSystem의 Result 보정을 수정했고 UI Test에서 UI 객체를 추가한 부분도 있었다. 이번 요청으로 해당 보정/생성을 제거했다. 기존 Phase에서 남아 있던 Outline/Settings 자동 추가도 함께 제거했다.

실제 컴포넌트 추가/참조·Index 지정·배치 미리보기/저장 절차와 전체 대상 표는 Step 7-3~7-4로 이동했다. 이 절은 코드 준비/검증 기록만 보관하며 지금 Editor 적용을 요청하지 않는다.


이번 보강 후 preflight(누적 C# 29파일), 계정 이전 UI/로컬 저장 소스 계약, UI 생성/레이아웃 제거·Scene 26개 버튼 목록/새 GUID 미적용·문서 링크와 공백 검사를 통과했다.
문서 집계용 임시 PowerShell→Node 문자열 치환은 비 ASCII 입력 변환으로 한 번 실패했고 파일 변경 전에 중단됐다. apply_patch로 집계를 정정하고 실제 Scene 버튼 수/표를 다시 대조했다. Unity Compile/Test Runner/빌드는 실행하지 않았다.

이후 사용자 요청으로 Scene 실제 적용 절차·대상 표·최종 안내 문구·레이아웃 목록은 Step 7-2~7-4로 통합했다. Step 4에는 준비 결과만 남겼다. 이번 재배치는 문서만 변경했고 코드/Scene 수정·Unity 실행·로컬 대역 검사를 수행하지 않았다. 문서 순서·내용 보존·링크/공백·비문서 파일 불변을 정적으로 검증했다.

## Step 5. 기존 Test의 fixture를 목적별로 전환한다

### AI 작업

1. Phase 1 Step 5-4의 13개 Stage restart helper 호출 Test와 직접 Stage 초기화 Test를 개별 분류한다.
2. 공통 이동/카메라/착지/입력/생명주기 Test는 Infinite 생산 구성 또는 책임에 맞는 독립 fixture로 전환한다. Stage 지형 가정이 있는 Test를 일괄 Mode 치환하지 않는다.
3. StageGoal/고정 배치 등 비권장 컴포넌트의 독립 Test는 생산 Stage Run을 시작하지 않는 합성 검증으로 남긴다. 생산 경계의 Stage 거절 Test를 별도로 유지한다.
4. Stage candidate factory를 공통 fixture로 쓰는 저장/계정 Test가 영향받으면 필요한 Infinite fixture로 바꾼다. Phase 3의 실제 기록 차단을 테스트 편의로 앞당기거나 Stage 생산 접근 예외를 열지 않는다.
5. 기존 PlayMode 격리 설정과 메모리 transport/file store 사용을 확인한다. 실제 인증/사용자 저장에 연결되는 Test는 실행 준비 완료로 넘기지 않는다.

### 자동 검증 대응표

| 책임 | 기존 Test 출발점 | 검증할 내용 |
| --- | --- | --- |
| 기본값/안내/복귀·중복 요청 | EditMode `GameNavigationStateTests`, `GameRuntimeDataTests` | Step 2 전환표·과거 값·명시 Stage 거절·초기화 실패 |
| 공통 실행/Stage 거절 | PlayMode `StageSystemTests`, `GameLifecycleIntegrationTests`, `GameEntryBootIntegrationTests` | 거절 부작용 0·Infinite 시작/종료·listener/초기화 정리 |
| UI/결과 | EditMode `UIVisibilityStateTests`, `ResultSystemTests`, `ResultTextFormatterTests`; PlayMode `ModeUISceneConfigurationTests`, `ModeResultDisplayIntegrationTests` | 비활성 UI/포커스·Infinite Result·현재 필수 참조 |
| 안내·Pause/Retry | PlayMode `HowToPlayIntegrationTests`, `PauseMenuIntegrationTests`, `ResultMenuIntegrationTests`, `GamePauseOrchestrationTests` | 실제 입력과 Run 생성 수·Pause 복귀·Timer/입력/점수 재설정 |
| 이동/물리·수집/카메라 | PlayMode `AutoMovementIntegrationTests`, `PlayerJumpIntegrationTests`, `MomentumLandingIntegrationTests`, `WallLandingRecoveryIntegrationTests`, `CollectibleLifecycleIntegrationTests`, `CameraFollowIntegrationTests` | 실제 물리/충돌/자동 이동·camera·scope 보존 |
| Infinite 구조 | PlayMode `InfiniteModeIntegrationTests`, `InfiniteHudIntegrationTests`, `InfinitePatternConnectionIntegrationTests` | 실제 종료·HUD/Pattern·World Rebase 영향 회귀 |
| 기본/참조/fixture 영향 | 관련 EditMode/PlayMode의 나머지 기존 suite | Phase 1에서 분류한 Stage 배치/Goal·저장/계정/검증 helper의 책임 보존 |

위 이름은 존재하는 기존 suite의 출발점이다. 새 Test 이름·실행 필터·영향 전체 목록은 실제 변경 후 AI가 확정한다.
필터 밖 실패를 숨기지 않으며 공통 초기화/fixture를 바꿨으면 관련 suite 전체를 사용자 실행 목록에 포함한다.

### 사용자 수동 작업

없음. 소스 Test 작성 완료와 실행 PASS를 구분하고, 실제 실행은 Step 8에서 한다.

### 완료 조건

- [x] 영향 fixture와 suite를 분류했고 실제 생산 코드에 연결된 Test·격리·실행 목록이 준비됐다(2026-11-12). Unity 실행과 Scene 적용은 미수행이다.

### 5-1. 기존 Stage 시작 fixture 13개 분류 — 완료

아래 파일은 모두 `Assets/Tests/PlayMode/`에 있다. 생산 helper는 Infinite만 허용하며, Test를 통과시키기 위한 Stage 시작 예외는 추가하지 않았다.

| 기존 suite | 전환한 fixture와 보존한 책임 |
| --- | --- |
| `AutoMovementIntegrationTests` | Infinite 자동 이동·동일 Run 초기화·Pause/Retry. Stage helper 요청은 assertion 거절 후 기존 Run/속도 보존으로 전환. 생산 Timer는 InfiniteRunTimer 사용 |
| `CameraFollowIntegrationTests` | 실제 Infinite 시작·이동/점프·Pause/Retry의 수평 추적과 고정 수직 기준 |
| `CollectibleLifecycleIntegrationTests` | 실제 Infinite의 활성/미수집 코인 선택·Pause 겹침·종료 해제·Retry scope 분리. 재시작 후 코인은 다시 찾고 Stage 코인의 비활성/미결합을 별도 확인 |
| `GameLifecycleIntegrationTests` | Infinite 실행/종료·입력/카메라·Pause 중복. 활성 HUD 기대를 InfiniteHUD로 변경 |
| `GamePauseOrchestrationTests` | InfiniteRunTimer와 물리 정지/복귀, 공통 StageSystem.StopStage의 멱등 종료 계약 유지 |
| `MomentumLandingIntegrationTests` | 공통 착지는 별도 평지에서 측정. 실제 Infinite 점수/HUD 착지는 진행 System을 다시 활성화하고 생산 시작점에서 재시작 |
| `PauseMenuIntegrationTests` | Infinite Run에서 실제 버튼·입력·중복 Retry·Settings/메뉴 흐름 |
| `PlayerJumpIntegrationTests` | 실제 Infinite 데이터/입력/이동 + 기존 독립 평지/4m gap. Stage 지형에는 의존하지 않음 |
| `ResultMenuIntegrationTests` | Infinite 종료 화면의 실제 Keyboard/Mouse·같은 프레임 Retry와 다음 프레임 동일 runtime 보존 |
| `StageCollectibleLayoutIntegrationTests` | 생산 Run 없이 비활성 Stage 원본 10개 ID/배치·trigger/visual/layer·기존 jump arc 보존과 미결합 검사 |
| `StageCollisionConfigurationTests` | 생산 Run 없이 보존 지형/Goal/마찰 설정 확인. 접촉 검사는 Test 메모리에서만 기존 지형을 활성화하고 CollisionSystem.Initialize/실제 sphere cast 사용 |
| `StageGoalIntegrationTests` | 생산 Run 없는 실제 Scene Goal의 합성 trigger 관찰·정확한 Collider/초기화/중복 구독/1회 알림/Reset/구독 해제. 별도 Infinite 실행에서 Goal 관찰·Stage 초기화 거절이 Run을 종료/교체하지 않는지 확인 |
| `WallLandingRecoveryIntegrationTests` | 기존 독립 벽/평지의 실제 접촉·재점프·착지 1회·Retry. Infinite 전용 두 사례는 진행 System 재활성화·fixture Root 연결 후 거리/추락 종료 검증 |

Stage 지형을 Infinite로 단순 치환하지 않았다. 독립 물리 fixture에서는 InfiniteModeSystem의 프레임 진행만 비활성화해 먼 좌표의 World Rebase/종료 조건이 접촉 측정을 바꾸지 않게 한다. 실제 Infinite 데이터·이동/입력/충돌은 사용하며, 해당 fixture를 Infinite 통합 진행 검증으로 계산하지 않는다.

### 5-2. 직접 초기화·UI fixture 보완 — 완료

- `Assets/Tests/EditMode/MomentumProductionStateTests.cs`: 성공을 기대하던 Stage StartRun을 제거했다. 실행 중 InfiniteModeSystem의 Stage 초기화 거절 후 같은 runtime의 실제 착지 처리·Momentum 보상이 유지되는지 검증한다.
- `InfiniteHudIntegrationTests`: Stage 초기화 성공/HUD 활성 기대를 거절 후 Infinite HUD/runtime 보존, Infinite 수집 점수와 숨겨진 Stage HUD 검증으로 전환했다. 독립 runtime 데이터로 생산 UIManagementSystem을 구동한다.
- `InfiniteHudIntegrationTests`와 `InfiniteModeIntegrationTests`: Text/Image/Button/EventSystem을 동적 생성하거나 UI 필수 참조를 교체하던 fixture를 제거했다. SampleScene의 기존 직렬화 참조를 읽고 필수 참조를 assertion으로 확인한다. 런타임에서 누락 UI를 보충하지 않는다.
- 변경한 Scene suite는 UnityTearDown에서 SampleScene을 unload한다. 독립 물리 객체와 Time.timeScale도 정리한다. 공통 utility에 비활성 객체 검색·생산 runtime 부재 확인·물리 진행 격리·Scene unload를 모았다. Scene/Prefab 파일은 변경하지 않았다.
- 앞선 Step의 `GameEntryBootIntegrationTests`·`StageSystemTests`·`InfiniteModeSystemTests`·`ModeResultDisplayIntegrationTests`는 직접 Stage/미정의 요청 거절·listener/scope/화면 불변 Test를 유지한다. `InfiniteModeStateTests`의 Stage는 독립 비활성 sentinel이며 생산 Stage 시작이 아니다.

### 5-3. 저장/계정·기록 factory 영향 판정 — 완료

| suite | 판정 |
| --- | --- |
| `AccountTransferCompletionTests`, `AccountTransferControllerTests`, `OnlineAccountCoordinatorTests`, `OnlineLocalSaveScopeTests` | 메모리 저장/인증/transport의 합성 candidate로 계정 잠금·이전·snapshot/저장 격리를 검사한다. GameRuntimeData/StageSystem 시작을 호출하지 않아 Phase 2 guard 영향 없음 |
| `LocalSaveJsonCodecTests`, `OnlineRecordRepositoryTests` | codec/저장 후보 및 fake transport/인증·repository 상태 검증. 생산 Stage Run 없음 |
| `RecordLeaderboardPolicyTests`, `RecordSubmissionPolicyTests` | Stage 정렬/반올림/검증·queue의 독립 모델 계약. Phase 3 변경 대상이므로 현재 기대 유지 |
| `VerificationSessionTests` | 메모리 파일/가짜 서비스와 probe 응답 비교. 실제 인증/원격 연결 없음 |
| PlayMode `AccountTransferUIIsolationTests` | SampleScene UI와 메모리 FileStore/Authentication/Transport 사용. 합성 Stage candidate가 생산 Stage를 시작하지 않음 |

이 10개 suite의 Stage candidate는 Phase 2 초기화 변경으로 영향받지 않아 유지했다. 성공 기대를 위해 생산 guard를 풀지 않았으며, Phase 3 candidate/조회 차단을 앞당기지 않았다. `OnlineRecoveryNoticeFlowTests`의 Stage 인자는 기록 응답 표시 모델이며 플레이 시작이 아니다. 독립 `TimeRecordTests`/`ResultDataTests`/`ResultSystemTests`/`ResultTextFormatterTests`/`ScoreRecordTests`와 일반 `TimerSystemTests`의 PlayTimer key도 보존한다.

### 5-4. 실행 격리와 사용자 실행 범위 — 완료

- `PlayModeRecordIsolationSetup`의 OneTimeSetUp이 `GameSystem.UseIsolatedRecordsForPlayModeTests`를 설정하고 TearDown에서 복구한다. GameSystem은 TestMemorySaveFileStore를 쓰고 실제 인증/UGS coordinator를 만들기 전에 반환한다. `PlayModeRecordIsolationTests`가 영구 저장/온라인 경계 부재를 확인한다.
- 저장/계정 suite의 FileStore/인증/Transport는 테스트 내부 대역이다. `VerificationToolIsolationTests`는 자동 Test에서 실제 SDK 세션/원격 허가를 열 수 없음을 검사한다. UI 통합 Test가 Scene을 load해도 사용자 저장·인증에 연결하지 않는다. 이 구성은 정적 확인했으며 C# 실행 결과는 아직 없다.
- **Step 7 Compile·Scene 적용 완료 뒤 Step 8에서**, Test Runner의 `FlowState.EditModeTests`와 `FlowState.PlayModeTests`를 각각 필터 없이 **Run All**한다. 공유 기본값/생산 helper/UI fixture 변경 때문에 일부 이름만 필터링해 완료 판정하지 않는다. 실제 원격 verification 창 실행은 요구하지 않는다.
- 직접 변경한 suite 외에도 `GameNavigationStateTests`, `GameRuntimeDataTests`, `UIVisibilityStateTests`, `GameEntryBootIntegrationTests`, `HowToPlayIntegrationTests`, `StageSystemTests`, `InfiniteModeSystemTests`, `ModeResultDisplayIntegrationTests`, `ModeUISceneConfigurationTests` 전체를 포함한다.
- 관련 회귀는 위 5-3의 10개 저장/계정 suite와 `InfiniteModeStateTests`, `InfiniteMapPatternTests`, `InfiniteCollectibleLayoutIntegrationTests`, `InfinitePatternConnectionIntegrationTests`, `InfinitePatternTraversalIntegrationTests`, `ScoreCollectibleTests`, `CollisionSystemContactTests`, `WallFallPhysicsTests`, `UIInputSystemTests`, `TimerSystemTests`, `LocalPersistenceIntegrationTests`, `OnlineRecoveryNoticeFlowTests`, `SettingsPanelIntegrationTests`, `SettingsInteractiveRebindTests`, `PausePanelSceneConfigurationTests`, `PlayModeRecordIsolationTests`, `VerificationToolIsolationTests`, 앞 문단의 독립 결과/Timer suite를 포함한다. Run All은 그 밖의 기존 suite도 포함한다.

이번 수정 suite의 소스 사례 수는 아래와 같다. attribute 기준 준비 수이며 **실행/성공 수가 아니다**. 다른 Step에서 바꾼 suite와 전체 Runner 사례 수는 포함하지 않는다.

| 분류 | suite별 준비 수 |
| --- | --- |
| EditMode | MomentumProductionState 38 |
| PlayMode 공통 실행/입력 | AutoMovement 16, CameraFollow 4, GameLifecycle 7, GamePauseOrchestration 5, PauseMenu 8, ResultMenu 7 |
| PlayMode 물리/수집 | CollectibleLifecycle 5, MomentumLanding 4, PlayerJump 4, WallLandingRecovery 5 |
| PlayMode 보존 Stage 컴포넌트 | StageCollectibleLayout 1, StageCollisionConfiguration 4, StageGoal 3 |
| PlayMode Infinite | InfiniteHud 11, InfiniteModeIntegration 17 |

합계는 EditMode 38·PlayMode 101건 준비다. Scene UI 의존 suite는 Step 7의 Outline 2개·Hover 12개·Rebind 6개와 필수 연결을 적용한 뒤 실행해야 한다. 현재 Scene 누락을 숨기거나 테스트 생성으로 대신하지 않는다. 최초 Hover 26개 집계는 Step 6에서 Controller의 실제 소속을 대조해 12개로 정정했다.

Stage 거절/중복 시작 Warning 등은 해당 사례의 정확한 `LogAssert.Expect`로 처리한다. assertion으로 거절하는 helper 호출은 `Assert.Throws`로 판정한다. 예상 밖 Error/Warning을 전역 허용하지 않는다. 사용자에게 정밀 물리/입력 타이밍이나 API 호출을 수동 재현하도록 요구하지 않는다.

### 5-5. 정적 검증·완료 판정 — 완료

- `node UGS/Tests/client-unity-preflight.cjs`: 누적 변경/신규 C# 45파일·meta/구분자/asmdef·기존 격리 설정 검사 PASS. 출력의 Edit 183/Play 23은 해당 도구가 고정 관리하는 별도 source 사례이며 위 표와 합산하지 않는다.
- `node UGS/Tests/static-contracts.cjs`, `client-account-transfer-ui-contracts.test.cjs`, `client-local-save-contracts.test.cjs`, `client-verification-tools-contracts.test.cjs`(모두 UGS/Tests): PASS. 잘못 추정한 `client-static-contracts.test.cjs` 경로는 파일 없음으로 실행 실패했고 실제 `static-contracts.cjs`를 찾아 실행해 통과했다.
- 기존 helper 13개와 직접 Stage 초기화/Stage 모델 호출을 개별 대조했다. 성공을 기대하는 생산 Stage 시작은 남기지 않았다. UI 생성 제거·Scene 비변경·정확한 예상 로그/새 using·기존 meta/asmdef·diff 공백을 정적으로 확인했다.
- Unity Compile/Test Runner/빌드·Scene 저장·실제 인증/사용자 저장/원격 호출은 수행하지 않았다. Step 5는 fixture 분류·코드/격리/실행 목록 준비 조건으로 완료하며, 최종 정적 검증은 Step 6, 사용자 Compile/Scene은 Step 7, 실제 Test 결과는 Step 8이다.

## Step 6. 사용자 실행 전 정적 검증을 통과시킨다

### AI 작업

1. Step 1 경로표의 모든 생산 시작/Retry·기본값·직접 API·Editor 플레이 요청에 차단 또는 정규화 근거와 Test를 연결한다.
2. Stage 소스의 잔존/Deprecated·이유 주석·enum 값·GUID·공통 기반 보존을 검사한다. 단순 키워드 수나 `Obsolete` 존재를 실행 차단 PASS로 삼지 않는다.
3. UI 필수 참조·숨길 패널/Root·UnityEvent·Navigation을 YAML/코드에서 읽기 전용 대조한다. 변경 후 사용자 적용 항목과 기존 상태를 구분한다.
4. 다음 검사를 직접 실행하고, 변경과 관련된 다른 기존 source-contract 검사도 확인한다. 현재 변경으로 깨진 문자열 기대는 책임을 유지해 수정하며 검사 자체를 삭제해 통과시키지 않는다.

```powershell
node UGS/Tests/client-unity-preflight.cjs
node UGS/Tests/cloud-code.test.cjs
node UGS/Tests/account-personal-bests.test.cjs
git diff --check
```

5. C# 추가 파일의 `.meta`/asmdef·중복 선언·접근성·호출 시그니처와 Test 격리를 확인한다. Node/preflight 통과는 Unity Compile 성공과 구분한다.
6. 사용자에게 최종 변경 요약·Compile 요청·Step 7 적용 목록·Step 8 실행 suite/기대 로그를 제공한다.

### 사용자 수동 작업

없음. 정적으로 찾은 누락 참조·기본값·Test 위치 확인을 Inspector 수동 조사로 넘기지 않는다.

### 완료 조건

- [x] 관련 정적/로컬 검사 실패를 해결했고 C# 실행/Scene 적용 전 준비가 끝났다. 미검증 한계는 구분됐다(2026-11-12).

### 6-1. 생산 시작 경로·차단 위치·Test 대응 — 완료

Step 1의 경로표는 변경 전 기준선이다. 아래는 변경 후 실제 경계이며, Test 이름의 Stage는 모델/과거 값/공통 StageSystem 명칭과 생산 Stage 시작을 구분한다.

| 생산 경계 | 현재 차단/정규화 근거 | 연결한 Test |
| --- | --- | --- |
| Main Menu Play·과거 ModeSelect | GameNavigationState의 Play는 Infinite 안내/초기화로 진행. 명시 Stage/미정의 선택은 거절하고 과거 선택은 Play에서 Infinite 정규화 | GameNavigationStateTests의 Play/ExplicitPlaySelection/LegacyModeSelect/StagePlaySelection, Boot의 ExplicitStageAndUnknownRequest |
| 최초 안내·복원된 Tutorial·과거 대기 Stage | SubmitAutomaticHowToPlay가 과거 Stage를 Infinite로 정규화. 알 수 없는 pending은 거절. 일반 안내 열람은 Tutorial 완료를 바꾸지 않음 | Navigation의 LegacyStagePending/UnknownPending/MainMenuHowToPlay, GameEntryBootIntegrationTests의 Play/CompletedTutorial, HowToPlayIntegrationTests |
| ContextMenu/public StartGame·Inspector 과거 Stage | StartGame은 유효 enum만 처리하고 Infinite 메뉴/결과 요청 사용. StartRequestedRun은 유효 Infinite Navigation 요청을 상태 변경 전에 요구 | Boot의 StartGame_LegacyInspector, PauseAndResultRetryAndContextStart, ProductionHelper_RejectsRetiredMode |
| Pause/Result Retry | BeginInitialization에서 Infinite 정규화. Pause는 기존 Run을 정리한 뒤 새 요청, Result는 새 요청. Stage 상태를 되살리지 않음 | Navigation의 LegacyStageSelection_Retry, Boot의 PauseAndResultRetryAndContextStart, PauseMenu/ResultMenu/AutoMovement suite |
| GameRuntimeData/RuntimeDataSystem 직접 API·무인자 | 무인자/Clear/기본 Mode는 Infinite. 명시 Stage/미정의 요청은 기존 Collectible 정리·기존 객체 반환/신규 생성보다 먼저 거절(void 유지/null 반환) | GameRuntimeDataTests, StageSystemTests의 RuntimeDataSystem_UnsupportedCreation, Boot의 StageRequests_WhilePlayingAndPaused |
| StageSystem Initialize/Start/ConfigureCollectibles | Initialize는 Root/listener/수집 해제 전 거절. StartStage는 상태 Reset 전에 Infinite 확인. ConfigureCollectibles도 생성된 Infinite runtime만 결합 | StageSystemTests의 UnsupportedInitializeAndStart/Initialize_StageMode/StartStage_Deprecated snapshot, Boot의 StageRequests |
| InfiniteModeSystem Initialize·내부 상태 | 명시 Stage/미정의 초기화는 기존 metrics/Pattern/Momentum 정리 전에 거절. InfiniteModeState의 Stage sentinel은 비활성 독립 상태로 유지 | InfiniteModeSystemTests의 거절/기존 상태 보존, MomentumProductionStateTests의 RejectedStageInitialization, InfiniteModeStateTests |
| UI/Result·Goal·Editor 경계 | UIVisibility/UIManagement는 Stage 표시·포커스/필수 참조 의존을 차단. GameSystem 결과 callback은 Timer 변경 전 Infinite 확인. Editor 소스에는 별도 플레이 시작 API 호출 없음 | UIVisibilityStateTests, ModeResultDisplay/ModeUISceneConfiguration, Boot의 RetiredRuntimeEndCallback, StageGoalIntegrationTests |

생산 guard 7개는 본문을 추출해 부작용보다 앞에 있는지 정적으로 확인했다. ContextMenu는 같은 StartGame 경로다. `Assets/Editor`의 Stage Board/candidate/direct-write/온라인 probe는 플레이 시작이 아니라 Phase 3 기록 접근 대상이며 이번에 실행하거나 차단 구현을 앞당기지 않았다.

### 6-2. 소스·식별자·공통 계약·C# 경계 — 완료

- Stage enum은 Stage=0/Infinite=1의 기존 순서를 유지한다. StageGoal/StageRecordIdentity/TimeRecord/ResultData/formatter와 Stage 전용 분기는 소스/GUID·사유 주석을 보존한다. StageGoal trigger·StageSystem의 옛 FixedUpdate/Cleared 구독 API는 Obsolete이며, 실행 차단 근거는 별도의 생성/초기화/시작 guard다.
- 기존 변경 C#의 `.meta`는 HEAD와 동일하다. 새 handler 두 파일은 고유 meta와 파일/타입명을 갖고 중복 선언이 없다. Systems는 Assembly-CSharp 소속, Core/Features asmdef 경계는 유지한다. Test는 Systems 타입을 reflection으로 접근하며 새 handler의 공개 IsConfiguredFor/Controller callback 시그니처를 대조했다.
- 생산 Timer/이동·물리/Pattern/ScoringVersion·Editor·UGS/CloudCode·ProjectSettings 변경은 없다. InfiniteModeSystem 변경은 시작 경계 거절이며 기존 점수/진행/World Rebase 수식 변경은 없다. 기존 컴포넌트/직렬화 참조를 삭제하지 않았다.
- Step 5의 메모리 저장/인증/transport·PlayMode SetUpFixture 격리를 다시 확인했다. preflight 출력의 고정 source 수와 전체 Test Runner 실행 수를 구분한다. Unity C# 컴파일러는 실행하지 않았다.

### 6-3. Scene 읽기 전용 대조와 적용표 정정 — 완료

- SampleScene YAML 1,252개 블록을 읽어 UIManagementSystem 필수 Navigation 참조 25개, GameSystem 대상 UnityEvent 31개의 public void/async void 메서드 존재, Navigation 방향 232개의 참조 대상 존재를 확인했다. 숨길 ModeSelect 버튼으로 외부 Navigation이 들어오는 연결은 없다. Play/StartRun/Pause Retry/Result Retry는 Step 7-2의 기존 이벤트와 일치한다.
- Step 1의 이벤트 9개는 당시 지정 부분집합이며 이번 31개는 전체 GameSystem 대상 집계다. 안내 Binding 참조와 보존 Goal/Mode Root/Mode enum·현재 활성값은 Step 7 표와 대조했다. StageRoot/StageResultContent는 현재 active=1이므로 사용자가 꺼야 한다. 이미 적용됐다고 기록하지 않는다.
- **기존 안내 오류 수정:** SettingsUIController `1615873151`의 GameObject는 `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow`다. HEAD의 자동 추가 범위와 현 ValidateButtonFocusHandlers는 이 객체의 자손 Button **12개**다. 형제 AccountTransferHost 아래 버튼 14개를 포함한 이전 26개 집계는 잘못됐다. Scene 적용표와 Memory를 **Hover 12 + Rebind 6 + Outline 2 = 총 20개**로 정정했다. Rebind 6개의 직렬화 순서/Index도 표와 일치한다.
- `ModeUISceneConfigurationTests.UIComponents_AreSerializedBeforeUseAndNeverAddedAtRuntime`가 Controller를 SettingsPanel에서 찾던 오류를 SettingsWindow로 수정했다. 이 Step에서 변경한 C#은 이 Test의 조회 위치 1곳이며 생산 코드/Scene 수정은 없다.
- Outline 2개와 handler 18개는 현재 Scene에 없다. 이 누락은 Step 7 사용자 적용 대상으로 남기고 런타임 생성으로 대체하지 않는다. Result RectTransform 7개의 크기/위치는 표와 일치해 유지한다. UIManagement/Settings에는 UI 생성/강제 Result 배치 코드가 없고 Runtime의 남은 Instantiate는 맵 Pattern 생성이다.
- 임시 검사 초기식이 async void 이벤트를 제외했고, 다음 임시식에는 배열 구문 오류가 있었다. 검사식을 수정한 후 위 실제 Controller 계층 차이를 발견해 문서/Test를 정정했고 최종 대조는 통과했다. 이를 Unity 실패나 실행 PASS로 기록하지 않는다. Scene/Prefab 파일은 변경하지 않았다.

### 6-4. 필수·관련 로컬 검사 결과 — 완료

| 실제 실행한 검사 | 결과/한계 |
| --- | --- |
| `node UGS/Tests/client-unity-preflight.cjs` | PASS: 누적 변경/신규 C# 45파일·meta/구분자/asmdef·격리 설정. Edit 183/Play 23은 도구의 별도 고정 source 준비 수이며 미실행 |
| `node UGS/Tests/cloud-code.test.cjs` | 28/28 PASS. 로컬 SDK/메모리 대역이며 실제 UGS 호출 없음 |
| `node UGS/Tests/account-personal-bests.test.cjs` | 18/18 PASS. 로컬 snapshot 계약이며 실제 계정 조회/이전 없음 |
| `static-contracts.cjs` | PASS. 기존 source/서버 계약 검사 |
| `client-local-save-contracts.test.cjs`, `client-account-contracts.test.cjs`, `client-transfer-completion-contracts.test.cjs` | 모두 PASS. 저장·계정·이전 source 계약 |
| `client-account-transfer-ui-contracts.test.cjs`, `client-verification-tools-contracts.test.cjs` | 모두 PASS. UI·검증 도구 격리 source 계약 |
| 임시 Node 읽기 전용 경로/Scene/식별자 대조 | 최종 PASS. 6-1~6-3의 근거. Unity API/컴파일 실행 없음 |
| `git diff --check` | PASS. Git의 LF→CRLF 알림은 줄 끝 설정 알림이며 Unity Compile Warning과 구분 |

짧게 표기한 검사 파일은 모두 `UGS/Tests/`에 있다. C# 수정 후 preflight를 다시 실행해 통과했다. 실제 Compile/Runner/Player·원격 동작을 이 결과로 대신하지 않는다.

### 6-5. 사용자 인계·미검증 한계 — 완료

Step 6은 정적 검사/적용 전 준비 조건으로 완료한다. 현재 다음 작업은 **Step 7-1의 사용자 Compile**이다. Compile 성공 후 Step 7-2~7-3의 Root/UI/문구와 20개 컴포넌트를 적용·저장하고 완료 보고를 받으면 AI가 YAML을 다시 검사한다. Scene에 이미 일치하는 참조/이벤트/Result 배치를 다시 구성할 필요는 없다. 상세 대상/Index/7줄 문구는 아래 Step 7 표가 단일 기준이다.

Step 7 뒤 Step 8 실행 대상은 5-4의 `FlowState.EditModeTests`와 `FlowState.PlayModeTests` **각각 필터 없는 Run All**이다. 변경/회귀 suite 전체 목록도 5-1~5-4에 있다. 지금 Runner나 빌드를 요청하지 않는다.

직접 거절 사례는 `[RuntimeDataSystem] Only Infinite Mode runtime data can be created.`, `[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.`, `[StageSystem] Only Infinite Mode can be started.`, `[InfiniteModeSystem] Only Infinite Mode can be initialized.`를 정확한 LogAssert.Expect로 처리한다. UI/결과 거절·미정의 enum Error·중복 시작 경고도 각 Test의 기대 로그로 처리하며, helper 거절은 Assert.Throws로 판정한다. 예상 밖 Error/Warning이나 누락 UI 구성 Error를 일반 허용하지 않는다.

Unity Compile·직렬화 후 Inspector 인식·실제 UI/입력/물리/프레임 순서·Test 성공·Player 빌드/화면은 미검증이다. Step 7~9 결과가 필요하며 Phase 2 전체·Phase 3~4는 미완료다.

### 6-6. 사용자 요청에 따른 Step 6 전체 재검토 — 완료 (2026-11-12)

- 6-1~6-5의 완료 근거와 생산 코드/관련 Test·사용자 적용표를 다시 대조했다. 생산 guard 7개가 부작용보다 먼저 실행되는지 재확인했으며 추가 코드 결함은 찾지 못했다. 기존 코드와 Step 6 완료 상태는 유지한다.
- Scene 필수 참조 25개는 대상 존재뿐 아니라 GameObject/Button/TMP_Text 타입도 대조했다. GameSystem 이벤트 31개는 무인자 void 29개와 Slider float/Toggle bool 동적 이벤트 2개의 시그니처를 구분해 확인했다. Navigation 방향 232개의 비어 있지 않은 대상은 실제 컴포넌트다. Hover 대상 12개 fileID·Rebind 6개 순서/Index와 Controller 소속도 재확인했다.
- Result 7개 RectTransform과 InstructionText의 Anchor Min/Max·Pivot·크기·위치, 미리보기 뒤 복원할 저장 활성값을 읽기 전용으로 확인했다. Step 7 표의 상대 경로 기준을 SettingsWindow까지 명시하고, 미리보기 후 MainMenuPanel=켬/ResultPanel=끔/InfiniteResultContent=켬으로 되돌리도록 안내를 보완했다. Scene/Prefab은 수정하지 않았다.
- 6-4의 9개 로컬/소스 검사와 `git diff --check`를 다시 실행해 모두 통과했다(preflight 누적 C# 45파일, 로컬 제출/조회 28/28·snapshot 18/18). 이번 재검토 수정은 Task/Memory의 안내/기록뿐이며 C#·Unity 실행 결과를 추가하지 않는다. 사용자 수행 대기는 Step 7로 유지한다.

### 6-7. 사용자 Compile 실패 후 fixture 수정 — 소스 수정 완료, 재컴파일 대기 (2026-11-12)

- 사용자가 `StageSystemTests.cs(538,58)`의 playerY·`(541,17)`의 gameMode 미정의 CS0103 두 건을 보고했다. 앞선 정적 검토에서 매개변수 없는 `FallThreshold_PausedStage_WaitsUntilResume`에 잘못 들어간 helper 본문을 놓쳤다. preflight는 C# 이름 해석/컴파일 검사가 아니며 이전 PASS로 이 오류를 확인할 수 없었다.
- Pause Test를 원래 책임인 임계 높이 준비→Pause 중 두 번 관찰해 pending/end 없음→Resume 후 두 번 관찰해 추락 종료 확인으로 복원했다. 미정의 두 식별자는 제거했다.
- 실제 `PrepareStageFallCheck(float playerY, E_GameMode gameMode)`도 수정했다. Stage 분기는 독립 합성 snapshot과 Rigidbody 참조만 준비하고 반환하며 생산 Stage Initialize/Start를 호출하지 않는다. Infinite 분기는 기존 runtime 생성/ConfigureCollectibles/실제 시작을 유지한다. 이는 컴파일 수정 후 Stage 성공 기대에서 다시 실패하지 않도록 누락된 fixture 전환을 바로잡은 것이다.
- 파일의 playerY/gameMode 사용 범위·Pause/Resume assertion 복원·Stage 합성 분기의 조기 반환과 생산 호출 부재를 정적으로 대조했다. preflight(누적 C# 45파일)·`git diff --check`를 재통과했다. Unity Compile/Runner/빌드는 AI가 실행하지 않았고 사용자 재컴파일 성공은 아직 미보고다. Step 7~10은 미완료로 유지한다.

## Step 7. 사용자가 Compile 후 Scene을 적용한다

### AI 선행 작업과 수행 순서

Step 6 정적 검증을 완료했으며 Step 7은 사용자 수행 대기다. 아래 최신 fileID·객체/필드·최종 문구/연결·컴포넌트 목록을 다시 대조했다. 이제 7-1 Compile부터 진행할 수 있다. Runner/빌드는 Step 8~9 순서를 따른다.
사용자 작업은 아래 7-1 → 7-2 → 7-3 → 7-4 순서로 진행한다. 기존 참조/컴포넌트를 삭제하지 않으며 AI는 Scene/Prefab을 편집하지 않는다.

### 사용자 수동 작업

#### 7-1. 사용자 Script Compile

1. Unity `6000.3.5f2`에서 프로젝트를 열고 Script Compile이 끝날 때까지 기다린다. Error·예상 밖 Warning이 있으면 로그를 전달하고 Scene 적용 전에 AI 수정/재검사를 받는다.
2. 현재 첫 Compile은 CS0103 두 건으로 실패 보고됐다. 6-7의 Test 소스 수정 후 재컴파일하고 결과를 전달한다. 성공 전에는 아래 Scene 적용을 진행하지 않는다.
3. Compile 성공 후 **Play Mode를 종료한 상태**에서 Project 창의 `Assets/Scenes/SampleScene.unity`를 연다. 아래 작업은 이 Scene의 Hierarchy/Inspector에서만 수행한다. Prefab 편집 화면에서 작업하지 않는다.

이하 **GameObject 활성**은 Inspector 맨 위 이름 왼쪽 체크박스다. **컴포넌트 Enabled**는 해당 컴포넌트 제목 왼쪽 체크박스다. 두 항목을 구분한다. 비활성 객체도 Hierarchy에서 선택해 컴포넌트와 필드를 편집할 수 있으므로 편집을 위해 부모 패널을 켤 필요는 없다. fileID는 AI 재검사용 식별자이며 사용자가 Inspector에 입력할 값이 아니다.

#### 7-2. 사용자 기본 Scene 상태·안내 문구·버튼 연결 적용

아래는 **Step 6 정적 검사 완료 후, Step 7-1 Compile 통과 뒤** 사용자가 적용할 최종 목록이다.
AI는 현재 저장된 `Assets/Scenes/SampleScene.unity`의 객체/컴포넌트/UnityEvent/Binding을 읽기 전용으로 대조했고 파일을 수정하지 않았다.
fileID는 GameObject와 Component를 구분한다. Inspector에서는 아래 이름/계층으로 찾고 기존 컴포넌트·참조·OnClick을 삭제하지 않는다.

**실제로 변경할 작업은 다음 네 가지다.** 아래 순서대로 수행한다.

1. Hierarchy의 `World/StageModeRoot`를 선택해 **GameObject 활성 체크를 끈다**. Goal은 자식으로 그대로 둔다.
2. `UIRoot/ResultPanel/Canvas/ResultWindow/StageResultContent`를 선택해 **GameObject 활성 체크를 끈다**.
3. `UIRoot/MenuCanvas/ModeSelectPanel/MenuList/` 아래 `StageButton`, `InfiniteButton`, `BackButton` **각각**을 선택한다. GameObject 활성 체크를 끄고, 기존 **Button** 컴포넌트의 **Interactable** 체크를 끈 뒤 **Navigation**을 펼쳐 **Mode = None**으로 바꾼다. On Click() 목록은 유지한다. 부모 ModeSelectPanel은 이미 꺼져 있으므로 계속 끈 상태로 둔다.
4. `UIRoot/MenuCanvas/HowToPlayPanel/InstructionText`를 선택한다. 기존 **TextMeshPro - Text (UI)** 컴포넌트의 **Text Input** 내용 전체를 아래 7줄 문구로 교체한다. RectTransform·Font Asset·Font Size·Auto Size는 변경하지 않는다.

아래 표는 위 변경의 목표값과 **수정 없이 유지할 값**을 함께 정리한 참조표다. 이미 일치하는 항목을 재설정하거나 참조를 다시 연결할 필요는 없다.

| 대상/Inspector 항목 | 현재 저장 기준선 | Step 7 사용자 적용 |
| --- | --- | --- |
| `UIRoot/MenuCanvas/ModeSelectPanel` | GameObject `1386715964`, active=0 | 끔 유지 |
| `UIRoot/StageHUD` | GameObject `714045101`, active=0 | 끔 유지 |
| `UIRoot/ResultPanel/Canvas/ResultWindow/StageResultContent` | GameObject `1501656893`, active=1 | **켬 → 끔** |
| `World/StageModeRoot` / 그 아래 `Goal` | Root GameObject `901191967` active=1. Goal 컴포넌트 `1090618367`, GameObject `1090618365` | **Root만 켬 → 끔**. Goal 자체 활성값·컴포넌트·Root/Goal 참조는 유지 |
| `World/InfiniteModeRoot` | GameObject `172987142`, active=1 | 켬/참조 유지. 실제 Run 코드가 활성/재초기화 제어 |
| `GameRoot/Systems/GameSystem`의 GameSystem → Selected Game Mode | GameSystem 컴포넌트 `1202804280`, 값 1(Infinite) | Infinite 유지 |
| ModeSelectPanel 아래 Stage/Infinite/Back Button | Button 컴포넌트 `761562235` / `593037429` / `1247354411` | 비활성 보존, Interactable 끔, Navigation=None. 참조/OnClick은 해제하지 않음. 생산 코드도 동일하게 차단 |
| `UIRoot/MenuCanvas/HowToPlayPanel/InstructionText` | GameObject `1574313433`, TMP 컴포넌트 `1574313435`. 기존 Stage 목표 설명 포함 | 아래 최종 7줄을 TMP의 Text Input에 교체 |

InstructionText의 현재 RectTransform(`1574313434`)은 920×330, 위치 (0,-184), Font Size=24, Auto Size=끔이다.
최종 문구는 명시적 7줄로 준비했다. 이번 적용에서 폰트/레이아웃 변경은 지정하지 않으며 실제 줄바꿈/가독성은 Step 9 화면 확인으로 판정한다.

```text
Move forward automatically. Use Jump to clear obstacles.
Press Momentum Landing just before touching down.
Consecutive Momentum Landings boost the distance score multiplier.
Collectibles add to your Total Score.
Travel farther to earn distance score.
Use Pause to pause or resume. Retry starts a new Infinite Run.
The Run ends if you fall or cannot keep moving forward.
```

다음 Button.onClick은 모두 **기존 연결 유지**가 최종 적용이다. 대상은 같은 Scene GameSystem 컴포넌트 `1202804280`이며 숨긴 Mode Select 버튼을 경유하지 않는다.

| 버튼의 실제 계층 | Button fileID | 유지할 메서드 |
| --- | --- | --- |
| `UIRoot/MenuCanvas/MainMenuPanel/MenuList/PlayButton` | `1467062858` | `GameSystem.SelectPlay()` |
| `UIRoot/MenuCanvas/HowToPlayPanel/StartRunButton` | `677628985` | `GameSystem.SelectStartRun()` |
| `UIRoot/PausePanel/Canvas/PauseWindow/RetryButton` | `1363044890` | `GameSystem.SelectRetry()` |
| `UIRoot/ResultPanel/Canvas/ResultWindow/RetryButton` | `1861140804` | `GameSystem.SelectRetry()` |

Jump/Momentum Landing/Pause Binding TMP 참조는 각각 `1801611000`/`522232955`/`1901269933`으로 유지한다.
이 문구는 입력 Binding을 고정 문자열로 대체하지 않는다. HowToPlay의 Back/StartRun·Pause/Settings·공개 번호/계정/결과 링크도 유지한다.
기존 연결이 이미 위와 일치하므로 사용자가 다시 버튼 연결을 만들 필요는 없다. 최종 안내는 Code/Scene 적용 전후를 구분해 Step 6~7에서 다시 대조한다.
2026-11-17 사용자 요청에 따라 Stage Leaderboard 탭도 비활성 대상에 포함한다. 정확한 추가 적용은 7-6을 따른다. 기록 자료 삭제는 수행하지 않는다.

#### 7-3. 사용자 UI 컴포넌트·참조·정적 레이아웃 적용

아래는 기존 GameObject **14개**에 컴포넌트 **20개**를 추가하는 작업이다. Leaderboard 버튼 2개에는 Outline을 1개씩, Settings 버튼 12개에는 Hover를 1개씩 추가하고, 그중 Rebind 버튼 6개에는 Rebind handler를 1개씩 더 추가한다. 새 UI GameObject는 만들지 않는다. 이번 작업을 이미 일부 적용했다면 같은 컴포넌트를 중복 추가하지 않고 기존 필드만 맞춘다.

##### 7-3-1. Leaderboard 버튼 2개에 Outline 추가

Hierarchy의 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList/` 아래에서 다음 두 버튼을 작업한다. 2026-11-17 Stage 탭 접근 제거 요청에 따라 **Stage 버튼 GameObject는 끄고 Infinite 버튼은 켬을 유지**한다. 자식 Text에 Outline을 붙이지 않는다. 이미 추가한 Stage Outline은 보존해도 되며, 없으면 새로 추가할 필요가 없다.

| 선택할 GameObject | 추가할 컴포넌트 | 컴포넌트 Enabled 최종값 |
| --- | --- | --- |
| `LeaderboardStageButton` (`1242588977`) | 기존 UI `Outline`은 선택 사항 | **끔** |
| `LeaderboardInfiniteButton` (`2045885534`) | UI `Outline` 1개 필수 | **켬** |

Infinite 버튼에 Outline이 없으면 Inspector의 **Add Component** → `Outline` 검색 → UI Outline을 추가하고 아래 값을 설정한다. 기존 Outline은 중복 추가하지 않는다. 마지막으로 위 표대로 **Outline 컴포넌트 제목의 Enabled 체크**를 설정한다. Infinite 버튼의 GameObject를 끄는 작업이 아니다.

| Outline Inspector 필드 | 두 버튼에 설정할 값 |
| --- | --- |
| Effect Color | RGBA = (1, 0.82, 0.25, 1), 0~1 기준 |
| Effect Distance → X / Y | 3 / -3 |
| Use Graphic Alpha | 체크 끔 |

실제 기록 화면에서는 Infinite Outline만 켠다. Stage 탭 비활성/Navigation 차단과 Infinite 연결 수정은 7-6을 따른다. 기존 Stage OnClick/직렬화 참조는 보존해도 되며 코드에서 Stage 선택을 거절한다.

##### 7-3-2. Settings 버튼 12개에 Hover handler와 참조 지정

먼저 Controller 대상의 위치를 확인한다. `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow`에 **기존 SettingsUIController**가 있다. 새로운 Controller를 추가하거나 SettingsPanel에 옮기지 않는다. 아래 12개 버튼의 Controller 필드에 모두 **이 동일한 SettingsWindow 객체**를 드래그해 연결한다.

아래 표의 버튼 **각각**에서 다음 절차를 반복한다.

1. Hierarchy에서 버튼 GameObject를 선택한다. 그 자식 Text나 부모 Row를 선택하지 않는다.
2. **Add Component**에서 `SettingsButtonHoverFocusHandler`를 찾아 1개 추가한다(Inspector 제목은 `Settings Button Hover Focus Handler`). 컴포넌트 Enabled는 켬으로 둔다.
3. 새 컴포넌트의 **Controller** 필드에 Hierarchy의 `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow` 객체를 드래그한다. 필드가 SettingsUIController 참조로 연결된다.
4. 같은 컴포넌트의 **Button** 필드에는 **지금 handler를 붙인 자기 버튼 GameObject**를 드래그한다. 기존 Button 컴포넌트 참조로 연결된다. Controller 객체나 자식 Text를 넣지 않는다.

Inspector에 Controller/Button이 `None`으로 남아 있으면 해당 연결 작업이 끝난 것이 아니다. 패널/버튼 자체의 활성값과 기존 OnClick은 그대로 유지한다.

##### 7-3-3. Rebind 버튼 6개에 Rebind handler와 Index 지정

아래 표의 **Index가 0~5인 처음 6개 버튼**에는 Hover 외에 컴포넌트를 하나 더 추가한다.

1. 같은 버튼 GameObject 선택 → **Add Component** → `SettingsRebindButtonFocusHandler` 추가(Inspector 제목은 `Settings Rebind Button Focus Handler`). Enabled는 켬으로 둔다.
2. **Controller**에 위와 같은 `SettingsWindow` 객체를 드래그한다.
3. **Index**에 표의 숫자를 입력한다. Jump는 **0**, 마지막 NavigateRight는 **5**다. 1부터 시작하지 않는다. 이 컴포넌트에는 Button 필드가 없다.

따라서 이 6개 버튼에는 기존 Button에 더해 **Hover handler 1개 + Rebind handler 1개**가 있어야 한다. 나머지 6개 버튼에는 Hover만 추가한다.

아래 경로는 `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow/` 기준이며 ID는 Button 컴포넌트다. 같은 이름의 Rebind/Confirm/Cancel은 부모 경로로 구분한다.

| SettingsWindow 하위 대상 — 모두 Hover 추가 | Button fileID(참고) | Rebind handler 추가 시 Index |
| --- | --- | --- |
| `ControlsScrollView/Viewport/Content/JumpRow/RebindButton` | `1684983054` | 0 |
| `ControlsScrollView/Viewport/Content/MomentumLandingRow/RebindButton` | `94556469` | 1 |
| `ControlsScrollView/Viewport/Content/NavigateUpRow/RebindButton` | `725436442` | 2 |
| `ControlsScrollView/Viewport/Content/NavigateDownRow/RebindButton` | `109202233` | 3 |
| `ControlsScrollView/Viewport/Content/NavigateLeftRow/RebindButton` | `1402378043` | 4 |
| `ControlsScrollView/Viewport/Content/NavigateRightRow/RebindButton` | `2105477288` | 5 |
| `OpenAccountTransferButton` | `221472029` | 없음 |
| `SettingsRecoveryNoticeButton` | `354306739` | 없음 |
| `RestoreDefaultsButton` | `574333905` | 없음 |
| `RestoreDefaultsConfirmationPanel/Dialog/ConfirmButton` | `897454321` | 없음 |
| `RestoreDefaultsConfirmationPanel/Dialog/CancelButton` | `2033557006` | 없음 |
| `BackButton` | `1846830885` | 없음 |

`AccountTransferHost`는 SettingsWindow의 형제다. 그 아래 계정 이전 버튼 14개는 Controller의 기존 `GetComponentsInChildren<Button>(true)` 대상이 아니므로 handler를 추가하지 않는다. Step 6에서 실제 Controller 소속과 HEAD의 자동 추가 범위를 대조해 종전 26개 안내를 정정했다.

##### 7-3-4. 기존 배치·스타일 유지 — 재입력 작업 없음

ResultWindow/버튼의 저장된 배치는 아래 값과 이미 일치하므로 **유지**한다. 크기/위치/Anchor/Pivot를 다시 입력할 필요는 없다. 코드가 이제 수정하지 않으므로 Editor에서 보이는 배치가 기준이다.

| RectTransform | fileID | 크기 | 위치 | Anchor/Pivot |
| --- | --- | --- | --- | --- |
| ResultWindow | `1895893078` | 520×740 | (0,0) | (0.5,0.5) |
| InfiniteResultContent | `1417628401` | 472×278 | (0,-36) | (0.5,1) |
| ResultRecordStatusText | `125647258` | 472×82 | (0,-334) | (0.5,1) |
| Result Retry | `1861140803` | 472×48 | (0,-432) | (0.5,1) |
| Result Leaderboard | `2048432089` | 472×48 | (0,-490) | (0.5,1) |
| Result Submission Retry | `36564352` | 472×48 | (0,-548) | (0.5,1) |
| Result Main Menu | `2129891215` | 472×48 | (0,-606) | (0.5,1) |

##### 7-3-5. 필요할 때만 Editor 미리보기·원복

Play를 끈 상태에서 배치를 미리 보려면 MainMenuPanel을 잠시 끄고 ResultPanel/InfiniteResultContent를 켠다. 확인 뒤 아래 값으로 복원한다. **미리보기를 하지 않았다면 이 항목의 활성값을 변경할 필요는 없다.**

| Hierarchy 대상 | 저장할 최종 GameObject 활성값 |
| --- | --- |
| `UIRoot/MenuCanvas` | 켬 유지 |
| `UIRoot/MenuCanvas/MainMenuPanel` (`176742822`) | 켬 |
| `UIRoot/ResultPanel` (`547905304`) | 끔 |
| `UIRoot/ResultPanel/Canvas/ResultWindow/InfiniteResultContent` (`1417628400`) | 켬 |

Stage UI/Root는 7-2대로 비활성을 유지한다. InfiniteResultContent의 자체 활성값이 켜져 있어도 부모 ResultPanel이 꺼져 있으면 초기 화면에 표시되지 않는다. 편집을 위해 다른 패널을 임시로 켰다면 작업 전 활성값으로 복원한다.

Leaderboard 선택 상태 색은 UIManagementSystem의 `Leaderboard Active ... Color` 직렬화 필드 기본값을 **유지**한다. 이번 작업에서 색상 변경이나 Button의 Color Tint 재설정은 필요 없다. Outline 자체의 색/거리는 7-3-1에서 추가한 컴포넌트에만 설정한다.

현재 Scene에는 위 Outline/새 handler가 없음을 정적으로 확인했다. 적용 전 해당 UI 경로는 구성 누락 Error를 내며 자동 생성하지 않는다. Scene 적용과 실행 성공을 완료로 기록하지 않는다.
최초 적용 집계는 Hover 12개+Rebind 6개+Outline 2개로 20개다. 2026-11-17 Stage 탭 제거 후 필수 구성은 Hover 12개+Rebind 6개+Infinite Outline 1개로 19개이며 기존 Stage Outline 1개는 비활성 보존할 수 있다. 앞선 33개/34개 안내는 Controller 소속 계층을 잘못 해석한 집계이며 이 정적 집계/표로 정정한다.

#### 7-4. 사용자 저장·적용 보고와 AI 정적 재검사

1. 위 작업을 끝내고 Play가 꺼져 있는 상태에서 **Ctrl+S로 SampleScene을 저장**한다. Scene 탭의 미저장 표시가 없어야 한다. 컴포넌트/문구 수정이 저장되지 않으면 AI가 파일에서 확인할 수 없다.
2. 다음 항목의 수행 여부를 짧게 전달한다. 아래 목록은 작업 범위 요약이며 fileID·참조·인덱스·YAML을 직접 조사해 보고할 필요는 없다.

```text
Compile: 성공 / 실패(로그 첨부)
Scene: SampleScene 저장 완료 / 미완료
7-2: Stage Root·Stage Result 비활성, ModeSelect 버튼 3개 설정, 안내 7줄 적용
7-3: Outline 2개 + Hover 12개 + Rebind 6개, Controller/Button/Index 연결 완료
미리보기 임시 활성값: 원복 완료 / 미리보기 안 함
예상 밖 Error/Warning: 없음 / 있음(로그 첨부)
```

AI가 저장된 YAML의 참조·비활성 상태·이벤트·Text·Navigation을 정적으로 다시 검사한다.
3. AI가 Infinite Outline 1개·Hover 12개·Rebind 6개의 존재/GUID·Controller/Button/Index, Inspector 스타일·Result 레이아웃과 7-2/7-6의 비활성/연결/안내 문구를 읽기 전용으로 확인한다. 기존 Stage Outline은 선택 사항이다. 정적으로 판정할 수 있는 항목은 사용자 반복 조사로 넘기지 않는다.
4. 참조/설정 누락이 있으면 해당 항목만 수정 안내하고 재검사한다. Scene 적용만으로 Unity Test/Player 성공을 기록하지 않으며 Runner는 Step 8, Player/화면 확인은 Step 9에서 수행한다.

#### 7-5. 사용자 적용 보고 후 정적 확인 (2026-11-17)

- 사용자에게 Compile 성공, SampleScene 저장, 7-2/7-3 적용, 미리보기 원복, 예상 밖 Error/Warning 없음을 보고받았다.
- 저장된 YAML에서 Hover 12개·Rebind 6개·Outline 2개와 Controller/Button/Index 연결을 확인했다. Stage Root/Stage Result·ModeSelect/StageHUD 비활성, Infinite 선택값과 ModeSelect 버튼 3개의 비활성/Interactable/Navigation 설정을 확인했다. 안내 문구는 YAML 줄 접기를 고려하면 7개 문장과 일치한다.
- 당시 두 저장값은 안내한 최종값과 달랐다. `UIRoot/ResultPanel` 활성과 Infinite Outline Enabled가 모두 켜져 있었다. 이후 사용자 요청으로 **Infinite Outline 최종값은 켬으로 변경**했으므로 이 Outline을 끄라는 이전 안내는 폐기한다. ResultPanel은 끔이 최종값이다.
- Step 7은 아래 최신 7-6의 수정·저장 및 후속 정적 확인 전까지 미완료로 유지한다. Scene/Prefab 편집과 Unity Compile/Runner/빌드는 AI가 수행하지 않았다.

#### 7-6. Leaderboard Stage 접근 제거 추가 적용 (2026-11-17)

사용자가 Leaderboard에서도 Stage Mode 접근 제거가 목표에 맞음을 확인했다. Phase 3의 사용자 탭/조회 경계를 선행 반영하며 다음 목록이 7-3-1/7-5보다 우선한다.

**AI 코드 준비:** GameNavigationState의 Stage 선택/Submit·키보드 탐색을 차단하고 Main Menu/Result의 첫 선택을 Infinite로 통일했다. LeaderboardViewState는 Infinite 기본값이며 Stage/무효 전환을 거절한다. GameSystem의 Top/Around는 Infinite Scoring Version 2만 조회하고 Stage Result 개인 최고 요청은 거절한다. UIManagementSystem은 Stage 탭을 숨기고 입력/Navigation을 끄며 Infinite/조회 Retry 경로를 보정한다. 동적 UI 생성/레이아웃 변경은 추가하지 않았다.

**사용자 적용 순서:**

1. 변경된 C#의 Unity Compile 완료와 Error/예상 밖 Warning 없음을 확인한다. 앞선 성공 보고는 이번 코드 변경 전 결과다. Runner/빌드는 아직 실행하지 않는다.
2. Play를 끈 상태에서 아래 Inspector 값을 적용한다. 버튼 경로 기준은 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList/`다.

| 대상 | Inspector 최종값 |
| --- | --- |
| `LeaderboardStageButton` (GameObject `1242588977`, Button `1242588979`) | GameObject 활성 **끔**. Button Interactable **끔**. Navigation **None**. 기존 Outline Enabled **끔**(없으면 추가 불필요). 자식 Text·OnClick·UIManagementSystem 참조는 보존 가능. |
| `LeaderboardInfiniteButton` (Button `2045885536`) | GameObject 활성 **켬**, Button Interactable **켬**. 기존 Outline Enabled **켬**. Navigation **Explicit**, Up=None, Down=`LeaderboardRetryButton`, Left=None, Right=None. |
| `LeaderboardRetryButton` (Button `704930304`) | Navigation **Explicit**, Up=`LeaderboardInfiniteButton` 유지. Down의 기존 Pending 연결은 유지하며 실제 Pending 유무에 따라 코드가 Back/Pending으로 갱신한다. |
| `UIRoot/ResultPanel` (GameObject `547905304`) | GameObject 활성 **끔**. 미리보기 원복 누락도 이 값으로 수정한다. |

3. Infinite Outline의 색/거리/Graphic Alpha는 7-3-1 값을 유지한다. Stage 버튼을 숨긴 뒤 Infinite 버튼의 가로 중앙 배치는 아래 7-7을 따른다. Settings Hover 12개/Rebind 6개와 기존 Controller/Button/Index는 그대로 유지한다.
4. SampleScene을 저장하고 `재Compile 성공 / Scene 추가 적용·저장 완료 / 예상 밖 Error·Warning 없음`을 보고한다. AI가 저장 YAML의 비활성/Interactable/Navigation/Outline과 기존 연결을 읽기 전용으로 확인한다.

**검증 준비와 한계:** GameNavigationStateTests의 Stage 선택 거절·Infinite 진입·Pending 탐색, LeaderboardViewStateTests의 Stage/무효 전환 거절·요청 보존, ModeUISceneConfigurationTests의 숨김/단일 선택/Navigation/컴포넌트 선존재, OnlineRecoveryNoticeFlowTests의 Infinite 조회 Board·Stage Result 개인 최고 요청 없음 검증 소스를 반영했다. Unity Test Runner는 AI가 실행하지 않으며 기존 Step 8 Run All에서 검증한다. Repository/직접 제출/검증 도구·Pending 전체 정책은 Phase 3 잔여 범위이고 Phase 3 전체 완료로 기록하지 않는다.

**정적 확인 결과:** client-unity-preflight(누적 변경/신규 C# 48파일)의 선언 중복·구분자·GUID·asmdef·격리 설정 검사를 통과했다. 별도 소스 검사로 Infinite 기본값/Stage 선택 거절·Main Menu/Result/Submit·Top/Around/개인 최고의 Infinite Board·Stage Result guard·Stage 탭 숨김·UI 생성 없음과 최신 적용표 공백 검사를 통과했다. 변경한 추적 C#/문서는 한정 `git diff --check`를 통과했다. 이는 C# 컴파일/Unity Test 성공 판정이 아니다.

#### 7-7. Infinite Leaderboard 버튼 중앙 배치 (2026-11-17)

사용자가 7-6 Scene 작업 완료를 보고했다. 저장된 YAML에서 Stage 탭 active=0/Interactable=0/Navigation=None/Outline 꺼짐, Infinite 탭 active=1/Interactable=1/Outline 켜짐/Left=None/Down=조회 Retry, 조회 Retry Up=Infinite, ResultPanel active=0을 확인했다. 추가 코드 변경 후 Compile/Error/Warning 결과는 별도 미보고이므로 Step 7 전체 완료로 기록하지 않는다.

Stage/Infinite 두 버튼의 기존 위치는 각각 X=-104/+104, Y=-82였다. Stage 탭을 숨긴 뒤 Infinite만 오른쪽에 남아 있으므로 아래처럼 가로 중앙으로 이동한다. 이전 7-6의 위치 변경 불필요 안내를 이 절로 정정한다.

1. Play를 끈 상태에서 Hierarchy의 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList/LeaderboardInfiniteButton`을 선택한다. 자식 Text 대신 **버튼 GameObject**를 선택한다.
2. Inspector의 **Rect Transform → Pos X만 `104`에서 `0`으로 변경**한다. Anchor Preset을 다시 선택하거나 Transform을 Reset하지 않는다.

| Rect Transform 항목 | 최종값 |
| --- | --- |
| Anchors Min / Max | 둘 다 X=0.5, Y=1 유지 |
| Pivot | X=0.5, Y=1 유지 |
| Pos X / Pos Y | **0 / -82** |
| Width / Height | 200 / 48 유지 |

3. Stage 버튼은 비활성으로 보존하고 Infinite Outline은 켬을 유지한다. 다른 UI 위치/Navigation/이벤트는 변경하지 않는다. SampleScene을 저장한다.
4. 배치를 미리 보기 위해 LeaderboardPanel 등을 임시로 켰다면 저장 전에 원래 활성값으로 복원한다. 최종값은 MainMenuPanel=켬, LeaderboardPanel=끔, ResultPanel=끔이다.

부모 MenuList는 중앙 Anchor/Pivot이고 가로 크기는 860이다. Infinite 버튼의 Anchor/Pivot X=0.5에서 Pos X=0으로 설정하면 부모 가로 중앙에 놓이며, 기존 Y/크기로 다른 행과의 간격을 보존한다. 코드의 위치/크기 덮어쓰기는 추가하지 않고 AI는 Scene/Prefab을 수정하지 않는다. 저장 후 AI가 RectTransform 값을 정적으로 확인한다.

#### 7-8. Compile·최종 Scene 적용 확인 및 완료 (2026-11-17)

- 사용자에게 최신 코드 Compile 성공·예상 밖 Warning/Error 없음·Scene 적용 완료를 보고받았다.
- SampleScene YAML 1,272개 블록을 읽기 전용으로 재검사했다. 필수 UI 참조 25개, GameSystem 이벤트 메서드 31개, Navigation 방향 232개의 대상과 비활성 버튼으로의 외부 연결 부재, HowToPlay Binding 3개·안내 7개 문장을 확인했다.
- Hover 12개/Rebind 6개의 GUID·Enabled·Controller/자기 Button/Index 연결, 기존 Outline 2개(Stage 끔/Infinite 켬)와 스타일을 확인했다. Stage Root/Stage Result/ModeSelect/StageHUD 및 Stage Leaderboard 탭 비활성·Interactable/Navigation 차단, Infinite 기본 선택·입력/조회 Retry 연결을 확인했다.
- Infinite 버튼 RectTransform은 Pos=(0,-82), Size=(200,48), Anchor Min/Max/Pivot=(0.5,1)로 중앙 배치됐다. 기존 Result 배치 7개도 유지됐다. MainMenuPanel=켬, LeaderboardPanel=끔, ResultPanel=끔, InfiniteResultContent=켬의 미리보기 원복값을 확인했다.
- 위 항목의 자동 정적 검사 675건과 LeaderboardPanel 비활성 추가 검사가 통과했다. 사용자 Compile 보고와 변경 후 Scene 검사를 근거로 Step 7을 완료 처리한다. AI는 Scene/Prefab을 편집하거나 Compile/Test Runner/빌드를 실행하지 않았다. Step 8~10·Phase 2 전체·Phase 3 전체는 미완료다.

### 완료 조건

- [x] 사용자 Compile Error/예상 밖 Warning 없음과 Scene 적용 결과를 받았고, AI의 변경 후 Scene 정적 검사가 통과했다.

## Step 8. 사용자가 EditMode·PlayMode Test를 실행한다

### AI 선행 작업

실행 대상은 5-4의 `FlowState.EditModeTests`/`FlowState.PlayModeTests` 각각 필터 없는 Run All이다. 관련 suite/영향 범위와 준비 수는 5-1~5-4, 예상 로그는 6-5를 따른다. 사용자 PlayMode 실패 보고와 수정 내역은 8-1, 수정 후 전체 통과 결과는 8-2에 기록한다.

### 사용자 수동 작업

1. `Window → General → Test Runner`를 열고 AI가 제시한 관련 **EditMode suite 전체**를 실행한다.
2. 실제 SampleScene/입력/물리·카메라/초기화·Retry·Scene 구성은 관련 **PlayMode suite 전체**로 실행한다.
3. Run별 통과/실패/미실행 수와 실패 Test 이름·메시지·Stack Trace, 예상 밖 Error/Warning을 전달한다. 결과 파일을 보관할 수 있으면 함께 보관한다.
4. 실패 시 AI가 원인을 수정/정적 검사한 뒤 영향 suite를 재실행한다. 실행하지 않은 Test를 통과로 기입하지 않는다.

### AI 반영·검증

수동 빠른 입력 시험 대신 자동 Test에서 같은/인접 프레임 중복·거절 후 상태 불변·입력 잔류·Pause rollback을 판정한다.
생산 Scene Test가 실제 생산 구성과 동일한 조건을 사용하는지, 종료 시 객체/입력 장치/콜백/활성 상태를 정리하는지 확인한다.

#### 8-1. StageSystemTests 준비 오류 수정 (2026-11-17)

- 사용자가 `UnsupportedInitializeAndStart_PreserveRunRootsListenersAndCollectibles`의 Ready/Playing/Paused/Ended 네 경우에서 56행 `collectible.Bind(...)`가 false인 실패를 보고했다. 전체 통과/실패/미실행 수와 EditMode 결과는 미보고다.
- 원인은 테스트가 수집물 Trigger를 BoxCollider로 만든 것이다. 생산 ScoreCollectible.Bind는 HasSupportedTriggerShape에서 SphereCollider만 허용하며, 겹침 검사도 SphereCollider를 사용한다. None 경우는 수집물 준비 분기를 건너뛰므로 같은 실패가 발생하지 않는다.
- 테스트 수집물만 SphereCollider로 정정했다. 생산 Bind 검증·Stage 거절 guard·기존 바인딩/Scope/Root/리스너 보존 assertion은 유지했다. Scene/Prefab은 수정하지 않았다.
- 정적 확인: client-unity-preflight(누적 C# 48파일), 수정 추적 파일의 git diff --check와 Task 공백 검사, 지원 Trigger 형식/상태·Scope·바인딩 assertion 보존 소스 검사가 통과했다. Unity 실행 통과로 기록하지 않는다.
- 사용자 재Compile 후 PlayMode `StageSystemTests` suite 전체를 재실행한다. 영향은 이 fixture에 한정되므로 이미 통과한 다른 suite의 반복 실행은 필요하지 않다. 최초 전체 Run 결과와 이번 재실행 결과를 함께 받아 Step 8 완료 여부를 판단한다. AI는 Compile/Runner/빌드를 실행하지 않는다.

#### 8-2. 전체 Test 통과 보고 및 완료 (2026-11-17)

- 사용자가 EditMode 916개·PlayMode 287개 전부 성공을 보고했다. 합계 1,203개 통과이며 앞선 StageSystemTests 네 실패는 수정 후 해소됐다.
- 근거는 사용자 Unity Test Runner 실행 보고다. AI가 Runner를 실행하거나 XML/로그 파일을 직접 검사한 결과로 기록하지 않는다. Step 8을 완료 처리하고 Step 9 Player 빌드/화면 확인으로 진행한다. Player/Phase 2 전체 완료로 확대하지 않는다.

### 완료 조건

- [x] 필요한 EditMode/PlayMode 결과를 받고 모든 관련 Test가 통과했다. 실패·미실행/예상 밖 로그가 남아 있지 않다.

## Step 9. 사용자가 Player 후보와 화면을 확인한다

### AI 선행 작업

생산 Player에서의 Stage 플레이 차단이라는 Phase 2 완료 조건을 확인할 수 있게 코드/define/Scene 포함 범위를 정적으로 대조한다.
기존 Player가 변경 후 결과를 대표하지 못하면 새 후보가 필요함을 명시한다. AI는 빌드하지 않는다.
Leaderboard Stage 탭은 7-6~7-8에서 차단·적용 확인했다. Phase 3의 Repository/제출/도구 전체 경계는 잔여 범위임을 플레이 흐름 검증과 구분한다.

2026-11-17 읽기 전용 준비 확인: Windows-production 프로필은 FLOW_STATE_PRODUCTION define, Development/Debugging/Profiler 끔이다. m_OverrideGlobalSceneList=0이므로 프로필의 빈 m_Scenes 대신 전역 EditorBuildSettings Scene 목록을 사용한다. 전역 목록은 SampleScene 1개 enabled이며 Scene meta GUID와 일치한다. RuntimeDataSystem/StageSystem의 Stage 생성/시작 거절은 UNITY_EDITOR 조건 밖에 있어 Player에도 적용된다. 기존 후보는 이번 코드/Scene 변경을 대표하지 못하므로 사용자가 새 출력 폴더로 빌드한다. AI는 프로필/Scene을 수정하거나 빌드를 실행하지 않았다.

### 사용자 수동 작업

1. Step 8 통과 뒤 `File → Build Profiles`에서 기존 Windows 프로필을 확인한다. 기준 파일은 `Assets/Settings/Build Profiles/Windows-production.asset`이다.
   기존 UGS 환경/Production define·Scene 목록을 임의 변경하지 않는다. 프로필/포함 Scene의 정적 판정은 AI가 먼저 제공한다.
2. 기존 성공 후보·QA 자료를 덮어쓰지 않는 새 출력 폴더에 사용자가 Player를 빌드한다. 빌드 실패나 예상 밖 Warning은 로그로 전달한다.
3. 기존 QA 기기/저장 영역을 보존하고 네트워크 연결을 끊은 상태에서 새 후보를 실행한다. 실제 인증/서버 제출·계정 이전을 이번 확인의 필수 절차로 수행하지 않는다.
   로컬 저장 변화는 기존 QA 범위 안에서 관리하며 Tutorial을 다시 보기 위해 실제 저장을 삭제/초기화하지 않는다.
4. Main Menu Play → Infinite, Pause/Resume, Pause Retry, 종료 후 Result Retry, Main Menu 복귀를 한 번씩 확인한다.
   Stage Mode 선택 화면·Stage HUD/Goal/Clear Time 결과가 나타나지 않고 Infinite의 거리/점수·Result가 읽히는지 확인한다.
5. Main Menu How To Play·Pause Settings 복귀와 Keyboard/Mouse Focus·문구 줄바꿈·HUD/Result 겹침을 확인한다.
   최초/완료 Tutorial 분기, 직접 Stage API 거절·중복/타이밍은 Step 8 Test 결과를 사용한다. 사용자가 저장/Inspector를 조작해 반복 재현하지 않는다.
6. 후보 경로/프로필·실행 결과·예상 밖 로그와 시각적 문제 유무를 전달한다. 빠른 조작이나 정밀 타이밍 입력을 요구하지 않는다.

#### 9-1. Candidate02 사용자 Player 확인 보고 (2026-11-17)

- 프로필: Windows-production. 사용자 빌드 위치: `Build/ProductionQA/Candidate02/`. 사용자가 빌드된 Player를 직접 실행했다.
- Main Menu Play가 모드 선택 UI 없이 Infinite를 시작함, Pause/Resume, Pause Retry, Result Retry, Pause/Result의 Main Menu 복귀와 Stage 관련 UI 미표시를 정상으로 보고했다.
- Pause Retry가 즉시 실행되는 동작에 사용자 확인이 필요하다는 추가 요청을 받았다. 아래 9-2를 반영한다. Candidate02는 이 변경 전 기준선으로 보존하고 덮어쓰지 않는다.
- 네트워크 차단 여부·HowToPlay/Settings 복귀·최종 UI 가독성/겹침과 빌드/Player의 예상 밖 로그 전체는 별도 미보고다. 이 항목이나 원격 기능 성공을 추정하지 않는다. 추가 확인창의 최신 Scene/Test/Player 검증이 남았으므로 Step 9/Phase 2 전체는 미완료다.

#### 9-2. Pause Retry 확인창 추가 및 재검증 (2026-11-17)

**동작/코드 준비:** Pause Retry 선택은 PauseRetryConfirmation을 열고 현재 Run/일시정지를 유지한다. 기본 선택은 Cancel이다. Cancel 버튼/Escape는 Pause의 Retry 선택으로 돌아가고, 확인만 이전 Run을 정리하고 새 Infinite Run을 시작한다. Result Retry는 즉시 재시작을 유지한다. 기존 Main Menu 복귀 확인창의 객체/배치를 재사용하고 문구/확인 동작을 전환한다. Enum은 끝에 추가해 기존 직렬화 숫자를 보존한다. UI 생성/추가 컴포넌트/Runtime 배치 덮어쓰기는 없다.

숨겨진 Pause Retry 버튼의 반복 SelectRetry 이벤트는 확인으로 처리하지 않고 진입 때 transient 입력을 소비한다. 명시적인 RetryGame 생명주기 API의 기존 직접 호출 계약은 유지하며 Pause UI는 확인 후에만 이 명령을 호출한다.

정적 검증 결과: client-unity-preflight(누적 변경/신규 C# 50파일)의 선언 중복/구분자/GUID/asmdef/격리 설정, 변경 추적 C#/문서의 git diff --check와 Task 공백 검사를 통과했다. 별도 소스 대조로 확인 전 초기화/Run 제거 없음·취소 경로·입력 잔류/숨겨진 버튼 guard·공유 TMP 필드/동적 생성 없음·기존 Enum 숫자 보존을 확인했다. 기존 Scene의 새 TMP 필드 부재/기존 OnClick은 사용자 적용 대상으로 확인했다. Unity Compile/Runner/Player 통과로 기록하지 않는다.

이후 사용자 Compile에서 PausePanelSceneConfigurationTests의 CS0841 다섯 건이 보고됐다. 새 확인창 검증을 uiManagementSystem 지역 변수 선언 앞에 추가한 오류다. 해당 선언/조회만 최초 사용 앞으로 이동하고 나머지 참조·화면 검증은 유지했다. 기존 preflight는 C# 이름 해석/컴파일 검사가 아니므로 이 선언 순서 오류를 검출하지 못했다. AI는 Unity Compile/Runner를 실행하지 않으며 사용자 재Compile 결과를 기다린다.

**9-2-1. 사용자 Compile·Scene 적용:**

1. Unity의 최신 C# Compile 완료/예상 밖 Error·Warning 없음을 확인하고 Play를 끈 상태에서 SampleScene을 연다.
2. `GameRoot/Systems/UIManagementSystem`의 기존 **UIManagementSystem** 컴포넌트에 새 필드 두 개를 연결한다. 새 컴포넌트나 새 확인창을 추가하지 않는다.

| Inspector 필드 (코드 필드) | 드래그할 기존 TextMeshProUGUI 대상 |
| --- | --- |
| Pause Confirmation Title Text (`_pauseConfirmationTitleText`) | `UIRoot/MenuCanvas/PauseMainMenuConfirmPanel/TitleText` (TMP `661031891`) |
| Pause Confirmation Action Text (`_pauseConfirmationActionText`) | `UIRoot/MenuCanvas/PauseMainMenuConfirmPanel/MenuList/MainMenuButton/MainMenuButtonText` (TMP `1940924472`) |

3. `UIRoot/MenuCanvas/PauseMainMenuConfirmPanel/MenuList/MainMenuButton`을 선택한다. 이 객체의 이름은 보존하며 이제 두 확인창의 **공유 확인 버튼**으로 사용한다. Button(`805748649`)의 **On Click() 기존 한 항목**에서 대상 GameSystem은 유지하고 함수를 `GameSystem.SelectMainMenu`에서 **`GameSystem.SelectPauseConfirmation()`**으로 교체한다. 기존 SelectMainMenu를 남긴 채 이벤트를 추가하면 안 된다. 최종 OnClick 항목은 이 함수 한 개다.
4. 위 패널의 `TitleText` Text Input을 **`Retry this Run?`**, `MainMenuButton/MainMenuButtonText` Text Input을 **`Retry`**로 저장해 Editor에서 Retry 확인 화면을 예측할 수 있게 한다. CancelButton의 Text=`Cancel`/OnClick=`GameSystem.SelectCancel`은 유지한다. 실행 중 Main Menu 확인에서는 코드가 제목=`Return to Main Menu?`, 확인 문구=`Main Menu`로 갱신한다.
5. 기존 Button Navigation은 유지한다(확인 Down=Cancel, Cancel Up=확인). PausePanel의 Retry OnClick=`SelectRetry`, Result Retry OnClick=`SelectRetry`도 유지한다. 패널 이름/참조/RectTransform을 변경하지 않는다.
6. 미리보기를 위해 확인 패널을 켰다면 원복한다. 최종 저장 활성값은 **MainMenuPanel=켬, PauseMainMenuConfirmPanel=끔, PausePanel=끔, ResultPanel=끔**이다. SampleScene을 저장한 뒤 Compile 결과와 Scene 적용 완료를 보고한다. AI가 새 TMP 참조/OnClick/문구/활성/Navigation을 정적으로 확인한다. 현재 Scene에는 새 TMP 필드가 없고 확인 OnClick도 SelectMainMenu이므로 아직 적용 완료가 아니다.

**9-2-2. 사용자 Test Runner 재검증:**

- Scene 정적 확인 뒤 EditMode/PlayMode 각각 필터 없는 Run All 결과를 받는다. 새 필수 UI 참조와 공유 Navigation/Retry API 변경이 생산 Scene fixture 전반에 영향을 주므로 이번에는 전체 실행한다. 앞선 916/287 결과를 최신 변경의 통과로 복사하지 않는다.
- GameNavigationStateTests에 취소/기본 Cancel/키보드 확인/Run ID 증가·중복 거절을 준비했다. PauseMenuIntegrationTests에 확인 전 Runtime 유지/문구/포커스, Cancel 버튼·Escape, 숨겨진 버튼 중복/인접 Submit, 확인 후 단일 재시작을 준비했다. PausePanelSceneConfigurationTests는 새 참조/공유 OnClick과 두 화면의 문구/포커스를 확인하며 GameEntryBootIntegrationTests의 Pause Retry도 확인 단계를 거친다. 기존 Main Menu 확인/Result Retry·직접 RetryGame 회귀는 유지한다. AI는 Runner를 실행하지 않는다.

**9-2-3. 사용자 새 Player 확인:**

- Compile/Scene/Test 통과 뒤 같은 Windows-production 프로필로 새 출력 폴더(예: `Build/ProductionQA/Candidate03/`)에 직접 빌드한다. Candidate02와 기존 QA 자료는 보존한다.
- 새 Player에서 Pause Retry → Cancel/Escape(같은 Run 일시정지 유지), Pause Retry → Retry 확인(새 Infinite Run), Pause Main Menu 확인(문구/복귀), Result Retry 즉시 실행을 한 번씩 확인한다. 기존 정상 흐름과 Stage UI 미표시도 유지돼야 한다.
- 아직 미보고인 Main Menu HowToPlay/Back, Pause Settings/Back과 Keyboard/Mouse 포커스·안내/HUD/Result 가독성/겹침, 후보 경로·빌드/Player 예상 밖 로그도 함께 보고한다. 실제 저장 삭제나 Inspector 조작을 통한 Tutorial 재현은 요구하지 않으며 원격 제출/계정 이전은 이번 확인의 필수 절차가 아니다. AI는 빌드를 수행하지 않는다.

#### 9-3. 최신 테스트·Candidate02 최종 확인 및 완료 (2026-11-17)

- 사용자 최신 보고: EditMode **919개**, PlayMode **288개** 전부 통과(합계 **1,207개**). 확인창 Test의 CS0841 선언 순서 수정 이후 결과이며 기존 916/287을 대체하는 최신 검증 근거다.
- 사용자는 이전 Candidate02 보존이 불필요하다고 결정하고 같은 Windows-production 프로필로 **`Build/ProductionQA/Candidate02/`에 덮어써 빌드**했다. 이전 후보 보존/새 폴더 지침보다 사용자 지시가 우선한다. 이 경로의 최신 빌드가 최종 확인 후보이며 Candidate03은 요구하지 않는다.
- 사용자가 기존 기능 정상, Pause Retry 확인 요구, 확인 후 Retry/메뉴 복귀 정상과 Main Menu HowToPlay/Back·Pause Settings/Back·Keyboard/Mouse 포커스·안내/HUD/Result 가독성/겹침·후보 경로/빌드/Player 예상 밖 로그 모두 정상 범위라고 확인했다. 이전 Player 보고의 Infinite 직행/Stage UI 미표시와 연결한다. Cancel/Escape/중복·Run 보존의 정밀 판정은 최신 전체 Test 결과를 사용하며 수동 반복 시험을 요구하지 않는다.
- AI는 최신 SampleScene 1,272개 블록과 소스/프로필을 읽기 전용 검사했다. 필수 UI 참조 **27개**(새 TMP 2개 포함), GameSystem 이벤트 **31개**, Navigation 방향 **232개**, 공유 확인 OnClick 1개=SelectPauseConfirmation/취소=SelectCancel, 확인/취소 Navigation과 초기 비활성을 확인했다. Stage 경로 차단·Infinite 중앙 배치·Result 배치·Hover/Rebind/Outline 연결도 유지됐으며 정적 검사 **592건**이 통과했다.
- Scene의 저장 문구는 기존 Main Menu 템플릿(`Return to Main Menu?`/`Main Menu`)이다. 공유 확인창의 유효한 기본 예시이므로 그대로 수용한다. Retry 화면에서는 코드가 `Retry this Run?`/`Retry`로 갱신하고 사용자 Player/전체 Test로 확인됐다. 9-2-1의 Retry 템플릿 저장 안내는 권장 예시로 정정하며 추가 Scene 변경을 요구하지 않는다.
- 근거는 사용자 Unity Test Runner/빌드/Player 실행 보고와 AI의 읽기 전용 정적 검사다. AI가 Runner/빌드/Player를 실행하거나 실행 로그/XML을 직접 확인했다고 기록하지 않는다. 네트워크 차단 여부나 원격 제출/이전 성공을 추정하지 않는다. 이번 사용자 덮어쓰기는 해당 Candidate02에 대한 지시이며 다른 QA 자료 삭제/이력 정리로 확대하지 않는다.
- 위 근거로 Step 9를 완료 처리한다. AI는 코드/Scene/프로필을 추가 변경하지 않았다.

### 완료 조건

- [x] 변경 후 Player의 빌드/실행 근거와 화면 확인 결과가 연결됐다. 실제 원격 기능 성공/Stage 기록 접근 제거 완료로 확대 해석하지 않았다. 9-2 확인창 변경 후 Compile/Scene/Test/Player 검증이 통과했다.

## Step 10. Phase 2 완료를 판정하고 Phase 3에 인계한다

### AI 작업

1. Roadmap 9 Phase 2의 세 완료 조건을 Step 1~9의 실제 근거와 대조한다.
2. Stage 플레이는 생산 Player/대상 Editor에서 시작/재시작되지 않는지, 과거 값은 Infinite로 처리하는지, Infinite 시작·Pause·종료·Retry가 유지되는지 확인한다.
3. 관련 정적 검사·필수 Unity/Player 결과가 없거나 실패가 남으면 Phase 2를 완료 처리하지 않는다. 정책 면제/과거 후보 성공을 자동 승계하지 않는다.
4. 실제 변경한 Feature/System 현행 규칙·Task·Project Memory·Roadmap을 갱신하고 미구현 Phase 3~4 경계를 명시한다.
5. 남은 Stage 기록 UI/직접 Repository/제출/Editor 온라인 probe와 Infinite 저장/계정 이전 회귀를 Phase 3에 인계한다.
6. 서버 계약/Module 변경은 기본 범위에 없다. 새 필요가 확인되면 복구 기준 source/artifact/reference·QA 보존 및 별도 배포/복구 승인 경계를 적용한다.

### 사용자 수동 작업

Step 7~9 결과가 이미 있으면 추가 작업 없음. 미보고가 있으면 AI가 정확히 지정한 결과만 전달한다. 확정 정책을 다시 승인하지 않는다.

### 최종 확인표

- [x] Stage 소스/참조는 보존·비권장 처리했고 생산 플레이/직접 실행/Retry 경로는 차단됐다.
- [x] 과거 기본/선택/대기 Mode가 Stage Run을 되살리지 않는다. Infinite 시작/안내/종료·Retry가 유지된다.
- [x] 정적 검사·EditMode/PlayMode·새 Player 근거와 필요한 화면 확인이 모두 연결됐다.
- [x] 물리/Pattern/점수·Infinite 저장/서버 계약을 범위 밖으로 변경하지 않았다.
- [x] Phase 3 기록 접근 작업과 Phase 4 통합 후보 판정의 잔여 범위를 명시했다.

### 최종 판정·Phase 3 인계 (2026-11-17)

| Roadmap 9 Phase 2 완료 조건 | 근거 |
| --- | --- |
| 생산 Player/대상 Editor에서 Stage 시작·재시작 불가 | 2-1~4-5/6-1~6-4의 진입·직접 API/ContextMenu/시작 guard, 최신 919/288 전체 통과, Candidate02의 Infinite 직행·Stage UI 미표시 보고 |
| 이전 상태에서 Stage 복귀 없이 Infinite 유지 | 과거 선택/대기/기본값 정규화와 명시적 Stage/무효 Mode 거절 Test, RuntimeDataSystem/StageSystem의 부작용 전 guard 소스 확인 |
| Infinite 시작·Pause·종료·Retry 유지 | 최신 전체 Test와 Candidate02 Player 보고, Pause Retry 확인/취소/중복 제어 및 Result Retry 유지, HowToPlay/Settings 복귀·UI 가독성/포커스 정상 확인 |

세 완료 조건을 충족하므로 **Step 10과 Phase 2를 완료 처리**한다. 서버/Module·Board/자료 삭제·저장 계약·물리/Pattern/점수 규칙 변경을 추가하지 않았다. 완료를 Roadmap 9 전체 완료나 원격 기능 검증으로 확대하지 않는다.

Phase 3은 이미 반영한 Stage Leaderboard 탭/Navigation/사용자 Top·Around 조회 고정을 재구현하지 않는다. 잔여 범위는 Stage 기록 후보 생성·직접 제출·Repository 직접 조회/조작·Result 개인 최고 등 우회 경계 전수 확인/차단, OnlineRecordVerificationWindow의 Stage 조회/제출/direct-write probe 정리, Infinite Pending/저장/서버 개인 최고 snapshot·계정 이전 계약 회귀다. Stage Deprecated 소스와 내부 자료형을 유지하며 기존 자료/Board 삭제·Infinite 변환·원격 서비스 변경은 하지 않는다. Phase 4 통합 후보 판정도 별도로 남는다.

# 영향 범위

Step 1~10 결과와 Roadmap/Feature/System/Project Memory 상태를 갱신했다. Navigation·공통 실행/UI/결과 경계·Deprecated 보존·Scene 기반 UI·Pause Retry 확인창과 회귀 Test를 반영했으며 누적 변경/신규 C# 50파일이다.
Scene은 사용자가 편집/저장했고 AI는 읽기 전용으로 확인했다. Unity Compile/Runner/Player 빌드·실행도 사용자 수행이다. 서버/Module·실제 자료 삭제/계약 변경은 수행하지 않았다. Phase 2 완료, Phase 3 전체/Phase 4는 미완료다.

# 검증 내용

Task의 Step 순서·담당/수동 작업/완료 조건, 관련 파일/Test/문서 링크·날짜/작업 번호,
Phase 1 정책 및 Phase 2/3 경계·미실행 상태·diff 공백을 정적으로 확인한다.

# 검증 결과

Node 읽기 전용 문서 검사로 Step 1~10 순서·담당/수동 작업/완료 조건·미체크 실행 상태·상대 링크·참조 소스/Test/Build Profile 경로·날짜/번호 중복 없음을 확인했다.
Roadmap Phase 2~4의 대기 상태·생산/Scene 변경 없음·문서 공백 검사와 `git diff --check`를 통과했다.
위 내용은 최초 절차 작성 시점의 검증이다. 이후 Step 1에서 세 기준선 명령을 실제 실행했고 결과는 1-5에 기록했다.
Step 2~6의 생산 변경·관련 Test 준비와 정적/로컬 검사 결과는 2-1~2-4, 3-1~3-5, 4-1~4-6, 5-1~5-5, 6-1~6-5에 기록했다.
이후 사용자 Compile/Scene 적용·최신 EditMode 919개/PlayMode 288개 전체 통과·Windows-production 최신 Candidate02 빌드/실행·화면 정상 범위 보고와 AI 최종 정적 검사 592건을 9-3에 기록했다. AI의 Unity 실행 결과나 이전 Phase/후보 결과를 최신 결과로 대체하지 않았다.

# 후속 작업

Step 1~10·Phase 2 완료. 다음은 Phase 3 잔여 기록 접근/제출/Editor 도구 차단과 Infinite 저장·계정 이전 회귀다. Phase 4와 Roadmap 9 전체는 미완료다.

# 관련 문서

- [Roadmap 9](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md)
- [공통 검증·승인 경계](../../04_Implementation_Roadmap/README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [StagePlay](../../03_Features/StagePlay.md)
- [GameEntryNavigation](../../03_Features/GameEntryNavigation.md)
- [HowToPlay](../../03_Features/HowToPlay.md)
- [ResultMenu](../../03_Features/ResultMenu.md)
- [GamePause](../../03_Features/GamePause.md)
- [InfiniteMode](../../03_Features/InfiniteMode.md)
- [GameSystem](../../02_Systems/GameSystem.md)
- [StageSystem](../../02_Systems/StageSystem.md)
- [UIManagementSystem](../../02_Systems/UIManagementSystem.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)

# 관련 작업 기록

- [Phase 1 정책·조사·검증·완료 인계](20261111_02_Phase1ManualSteps.md)

# 작성 완료 기준

- 실제 수동 작업을 Step으로 구분하고 AI 선행 작업·정적 검사·Unit Test 및 완료 조건에 연결했다.
- Phase 2와 Phase 3의 경계, Deprecated 소스 보존·공통 기반 유지와 실제 접근 차단을 구분했다.
- 문서 작성 완료를 구현/사용자 실행/원격 작업 완료로 기록하지 않았다.
