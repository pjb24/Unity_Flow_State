# 작업 정보

## 작업명

Prototype 7 Phase 4 — Leaderboard UI·기록 경쟁 흐름·출시 후보 검증 작업 계획

## 작업 일자

20260928

## 작업 담당자

AI, 사용자

## 작업 상태

2026-09-30 Step 1 사용자 결정 반영, Step 2 코드·Test·Scene 연결 명세 준비, Step 3 정적 대조·로컬 Cloud Code 대역 Test, Step 4 Unity 컴파일 확인·Scene 연결 정적 대조, Step 5 verification Cloud Code 적용, Step 6 전체 Unity Test Runner, Step 7 실제 서비스·Offline 복구, Step 8 Editor 1920×1080 화면·입력 확인 및 Step 9 Windows x64 Player Build·실행 확인을 완료했다. Step 10만 미완료다. Phase 4는 완료 근거 정리 단계다.

# 작업 목적

Roadmap 007 Phase 4의 Main Menu Leaderboard, Result 기록·순위·제출 상태, 계정 안내와 오류·재시도 경험을 구현하고 대상 플랫폼에서 검증한다. 정적 검사와 Unit Test로 판정 가능한 항목은 AI가 검사 또는 테스트를 준비한다. 사용자는 필요한 결정, Scene/Inspector 적용, Unity 컴파일·Test Runner·Build 실행, 실제 서비스 연결과 시각적 확인을 수행한다.

# 작업 대상

- Main Menu Leaderboard: Stage/Infinite Tab, 고정 Stage 선택, 상위·내 주변 독립 조회, Back/Cancel. 내 최고 전용 영역·요청은 제외한다.
- Result: 새 로컬 최고 기록과 서버 최고 기록 구분, 현재 Run 제출 상태, 자신의 기록·순위를 확인하는 경로.
- Loading, Empty, Offline, Error, Retry 및 Anonymous 복구 제한 동의·Settings 재안내.
- Menu·Run·Result·Settings·로컬 저장·온라인 서비스 회귀, 실제 verification 통합, 대상 플랫폼 Player.

# 작업 전 상태

- Roadmap과 Phase 3 Task에는 Phase 3 및 Step 1~8 완료가 기록돼 있다. 사용자 보고 기반 Edit Mode 702개, Play Mode 232개 성공은 해당 시점의 근거이며 Phase 4 변경 뒤의 회귀 결과로 재사용하지 않는다.
- `UIManagementSystem`과 `GameNavigationState`는 기존 Leaderboard 안내 화면 및 Back 경로를 가진다. `IOnlineRecordRepository`는 제출·상위·내 주변·내 최고 비동기 API를 제공하고, 실제 검증은 `OnlineRecordVerificationWindow`로 수행했다.
- 현재 Stage는 `stage-001` / Rules Version `1` / `fs-stage-stage-001-r1`, Infinite는 Scoring Version `2` / `fs-infinite-v2`다. 미구현 Stage나 다른 Version의 Board를 임의로 추가하지 않는다.
- 서비스 대상은 Project `c76d55cf-7846-494b-9dce-a0797b179b36`, 환경 `verification` / `a20a46fa-1edb-4d79-9c35-02f2fed31896`이다.
- 최초 Protected ledger 수동 생성, 계정별 128 submission ID, 보드 100명 조회 제한이 남아 있다. 이번 Phase는 verification 후보로 한정하고 공개 운영 준비는 다음 Prototype으로 분리하기로 결정했다.

# 조사 내용

- `Leaderboard.md`의 확정 기록이 있어야 조회하는 규칙을 유지하고 실제 UI 계약으로 갱신했다. Empty는 성공한 원격 조회의 0건 결과로 한정한다.
- `RecordSubmission.md`는 최초 온라인 요청 전 명시적 동의, 1회 계정 귀속, 다른 계정 후보 전송 금지, Pending 보존, Rejected 자동 재시도 금지를 정의한다.
- `Assets/Scenes/SampleScene.unity`가 존재한다. 정확한 연결 대상 Hierarchy와 필드·이벤트·레이아웃 값은 구현 완료 후 읽기 전용 조사로 확정해 제공한다. 아직 존재하지 않는 Component 이름을 연결 지시로 사용하지 않는다.
- `VERIFICATION_RULE.md`에 따라 수치·정렬·상태·중복·비동기 경합은 자동 Test로 판정한다. 사람이 빠르게 Tab을 누르거나 동일 프레임 조작을 재현하도록 요구하지 않는다.

# 작업 내용

## 공통 진행 원칙

- 각 Step은 AI 준비, 사용자 실행, 결과 판정 순서로 진행한다. AI 산출물이 준비되기 전에 해당 사용자 작업을 요청하지 않는다.
- AI는 Scene을 생성·수정하거나 Unity Build·Test Runner를 실행하지 않는다. Scene 참조·직렬화 값의 읽기 전용 정적 검사는 AI가 한다. UGS 원격 적용은 사용자가 한다.
- 결과 보고에는 Step, 실행 범위, 성공/실패, 예상하지 않은 Error/Warning, 실패한 Test 이름과 Stack Trace를 남긴다. token·secret·전체 Player ID·전체 사용자 저장 파일을 공유하지 않는다.
- 실제 서비스 검증은 verification만 사용한다. 기존 Board·ledger·계정·저장 파일을 초기화하거나 삭제해 테스트를 통과시키지 않는다.
- 사용자 작업이 없는 Step은 정적 검사·구현·자동 Test 작성만으로 진행한다. Test 작성 완료와 Test 실행 성공은 따로 기록한다.

## Step 1. UI 계약과 검증·출시 후보 범위를 확정한다

### AI 작업

1. Feature·System·Roadmap과 현재 화면 전환·Result·Repository를 대조하고 UI 상태표와 변경 파일 목록을 작성한다.
2. 기록이 없는 사용자에게 허용할 조회, Empty와 미제출 상태, Result에서 순위를 여는 방식, Stage가 하나인 선택 UI, 계정 표시 형식과 동의 취소 동작을 제안한다. 기존 규칙 변경이 필요한 부분은 장단점과 함께 명시한다.
3. Stage 시간 단위·표시 정밀도, Infinite 점수, 공동 순위, 로컬 새 최고와 서버 최고, Pending/Submitted/Rejected 표현을 계약으로 고정한다. Pending 0건만으로 Submitted 성공을 판단하지 않는다.
4. 검증 환경 한도 내 출시 후보인지 공개 운영 후보인지 구분한다. 후자라면 ledger 자동 초기화·보관 한도·순위 조회 확장·운영 배포를 별도 선행 작업으로 기록한다. 필수 범위를 미정 상태로 넘기지 않는다.

### 사용자 수동 작업

1. AI의 상태표·미정 항목 제안에서 사용자 선택이 필요한 항목만 결정한다.
2. 대상 플랫폼·아키텍처, 확인할 화면 해상도·창 모드와 입력 장치를 결정한다. 아래 결정에 따라 성능·응답 시간 수용 기준은 이번 검증에서 제외한다. 이전 Windows Player 검증 이력만으로 최종 플랫폼을 확정하지 않는다.
3. 검증 환경 한도를 가진 후보의 허용 범위를 결정한다. 이 결정은 production 배포 승인이 아니다.

### 완료 조건

- [x] 화면 계약, 플랫폼·검증 기준, 운영 제한의 처리 범위가 확정됐다.

### 2026-09-29 결정 반영

- 기록 없는 사용자의 조회 A, Empty 정의 A, 상위·내 주변 독립 조회, 원인 표시 Error, 명시적 Result 순위 보기, 단일 고정 Stage 선택, 동의 취소 시 원래 화면 복귀를 확정했다. 세부 기능 계약은 `Leaderboard.md`, `ResultMenu.md`, `RecordSubmission.md`에서 관리한다.
- Main Menu의 내 최고 전용 조회는 제외하고 Result의 서버 최고 표시는 유지한다. `New local best`와 `Online best` 및 해당 Run의 제출 상태를 분리한다.
- 같은 Score(Stage에서는 정수 밀리초 Clear Time)는 수락 시각에 관계없이 같은 순위를 사용한다. 동일 순위 안에서만 서버 수락 시각 오름차순으로 정렬한다. 이 변경은 다음 Prototype이 아닌 이번 Phase 4 구현 대상이다.
- 현재 계정·행 식별자는 마스킹하며 본인 행에 `(You)`를 표시한다. 전체 십진 Public Player Number 발급·표시는 다음 Prototype으로 이관한다.
- Rejected는 사유·재시도 불가를 표시하며 제출 Retry를 제공하지 않는다. 제출 Retry는 Pending에만 제공한다. 실패한 목록의 조회 Retry와 구분한다.
- Windows x64, 1920×1080 Windowed 단일 조건, Keyboard + Mouse를 필수 조건으로 확정했다. 현재 Player 기본 설정의 1024×768 Fullscreen Window는 이 검증 조건을 대체하지 않으며 Step 9에서 사용자가 적용·확인한다. Gamepad는 지원 선언 시 포함한다.
- 성능·응답 시간 측정은 사용자 결정으로 제외한다. Timeout·Offline 복구·오류 상태 자동 검증과 화면 조작성 검증은 유지한다.
- 이번 후보는 verification 한도 내 후보이다. 공개 운영 준비와 다음 Prototype 작업 목록은 Roadmap 007의 `다음 Prototype` 절에서 관리한다.

### UI 상태표

| 조건 | 표시·요청 계약 | 복귀·재시도 |
| --- | --- | --- |
| 제출 가능한 확정 기록 없음 | 기록 확정 안내, 원격 조회 없음 | Back/Cancel로 진입 화면 복귀 |
| 동의 필요 | 복구 제한 안내, 저장 전 온라인 요청 차단 | 취소는 요청 없이 진입 화면 복귀 |
| Loading | 해당 조회 영역만 로딩 표시 | 화면 닫기 허용, 중복 요청 억제 |
| 성공·목록 존재 | 상위·내 주변 각각 결과 표시 | 다른 영역 실패와 독립 |
| 성공·목록 0건 | 해당 영역 Empty, 내 온라인 기록 없음 안내와 문맥 구분 | 새로 조회 가능 |
| NotReachable | Offline | 조회 재시도, Pending 보존 |
| Timeout·인증·서비스 실패 | Error와 확인 가능한 원인·안전한 코드 | 실패한 조회 재시도 |
| 새 로컬 최고 | 이번 Run의 로컬 최고 갱신 표시 | 서버 수락 여부와 독립 |
| 서버 최고 조회 성공 | Result의 Online best 값·순위 | 실패·미조회는 확정값으로 표시하지 않음 |
| Pending | 제출 대기 표시 | 제출 Retry 제공 |
| Submitted | 해당 제출 ID의 수락 완료 | 최고 갱신·순위 확인으로 간주하지 않음 |
| Rejected | 거부 사유·재시도 불가 | 제출 Retry 없음 |

### 변경 대상과 구현 전 차이

| 대상 | 후속 구현·검증 |
| --- | --- |
| `Assets/Scripts/Runtime/Core/GameNavigationState.cs`, `E_NavigationScreen.cs`, `E_NavigationItem.cs` | Leaderboard 진입 출처·Result 복귀·선택 상태 |
| `Assets/Scripts/Runtime/Systems/GameSystem.cs`, `UIManagementSystem.cs` | 조회·동의·Result 상태·Settings 안내 연결 |
| `Assets/Scripts/Runtime/Features/CloudCodeRecordRepository.cs`, `OnlineRecordCoordinator.cs`, `OnlineAccountState.cs`, `LocalRecordRepository.cs` | 원인 전달·Run별 제출 결과·거부 사유 보존 경로 검토; 기존 enum·완료 ID만으로 사유를 복원할 수 없음 |
| `UGS/CloudCode/query-records.js`, `Assets/Scripts/Runtime/Features/RecordLeaderboardPolicy.cs` | 기존 수락 시각까지 같아야 공동 순위인 로직을 새 계약과 대조·수정 |
| `Assets/Tests/EditMode`, `Assets/Tests/PlayMode`, `UGS/Tests` | 새 동점 규칙·UI 상태·입력·비동기·회귀 테스트 |
| `Assets/Scenes/SampleScene.unity` | AI 읽기 전용 조사 후 사용자 연결; 상세 명세는 Step 2~3에서 제공 |

### Step 1 정적 대조 결과

- `GameNavigationState`는 Main Menu의 `LeaderboardUnavailable`과 Result의 Retry·Main Menu 두 항목만 제공한다. Leaderboard 진입 출처·Result 복귀·순위 보기·제출 Retry의 Navigation 항목과 선택 보존 구현이 Step 2 대상이다.
- `UIManagementSystem`은 기존 Leaderboard 안내 Panel과 Back Button, Result의 기존 Text·Button 참조만 직렬화한다. Leaderboard 상태별 표시, 마스킹 계정, Result의 온라인·제출 표시와 신규 Button 참조는 Step 2에서 추가하고 Step 3에서 실제 Scene 참조를 대조한다.
- `IOnlineRecordRepository`는 상위·내 주변·개인 최고 API를 제공한다. Main Menu는 상위·내 주변만 호출하도록 연결하고, Result의 Online best는 개인 최고 API를 사용할 수 있다. 현재 Repository는 제출 거부 사유를 `E_RecordSubmissionResult`로 축약하고 로컬 계정은 완료 ID만 보존하므로, Rejected 사유 표시에는 제출 응답·저장 모델 확장이 필요하다.
- 현재 `query-records.js`와 `RecordLeaderboardPolicy`는 수락 시각이 달라지면 다른 순위로 판정한다. 같은 Score의 공동 순위 및 수락 시각 표시 순서 계약과 다르므로 Step 2에서 서버·정책·자동 Test를 함께 수정한다. verification 원격 적용은 Step 5에서만 사용자가 수행한다.
- `SampleScene.unity`에는 기존 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList/BackButton`, `UIRoot/ResultPanel/Canvas/ResultWindow`, Retry·Main Menu Button과 `UIManagementSystem` 참조가 있다. 새 오브젝트·Button·직렬화 필드는 아직 존재하지 않아 Scene 연결 명세는 구현 뒤 Step 2~3에서 제공한다. AI는 Scene을 수정하지 않는다.
- 현재 Player 설정은 1024×768·Fullscreen Window다. 1920×1080 Windowed 검증 조건은 Step 9에서 사용자가 Build Profile/Player 설정에 적용한다. 이 Step에서는 Unity Editor·Build를 실행하지 않았다.
- Unity Test Runner, Unity Compile, Build, Scene 수정, UGS 원격 적용과 Cloud Code 테스트 실행은 수행하지 않았다. 문서·코드·Scene의 읽기 전용 정적 대조와 `git diff --check`만 수행했다.

## Step 2. UI 연결 코드와 Unit Test를 준비한다

Step 2는 서로 의존하는 다섯 하위 단계로 진행한다. 각 하위 단계는 코드·정적 검사·Test 작성 상태를 따로 기록하며, Unity Test Runner 실행은 Step 6에서 사용자가 수행한다.

### Step 2-1. Leaderboard 상태·정렬 정책을 준비한다

#### AI 작업

1. 상위·내 주변의 독립 조회 상태, Empty·Offline·Error 분기와 늦은 응답 폐기를 구현한다.
2. 같은 Score의 공동 순위 및 서버 수락 시각 오름차순 표시를 정책·Cloud Code에 반영한다.
3. 순수 상태와 공동 순위를 Edit Mode Test 및 Cloud Code 대역 Test에 작성한다.

#### 상태

- [x] 코드·Test 작성 완료. `LeaderboardViewState`, `E_LeaderboardQueryState`, `RecordLeaderboardPolicy`, `query-records.js`와 관련 Test를 변경했다.
- [x] Step 3의 Node/Cloud Code 정적 검사와 로컬 대역 Test를 통과했다.
- [ ] Step 6의 Unity Test Runner 실행은 미완료다.

### Step 2-2. Navigation·조회 요청을 연결한다

#### AI 작업

1. Main Menu의 Leaderboard 진입, Stage/Infinite Tab, 고정 Stage 1, 상위·내 주변 독립 조회, Back/Cancel과 Result 진입·복귀를 연결한다.
2. Main Menu의 내 최고 전용 요청을 추가하지 않고, Result의 Online best 조회 경로만 분리한다.
3. 화면을 닫거나 Mode를 바꾼 뒤의 응답이 현재 화면을 덮지 않게 한다.

#### 상태

- [x] Main Menu 진입, Stage/Infinite Mode 전환, 고정 `stage-001`·Rules v1 조회 Key, 상위·내 주변 독립 요청, Retry, Back/Cancel, Result 진입·복귀와 Result Online best 조회 경로를 구현했다.
- [ ] Scene Text·Button에 Tab·고정 `Stage 1` 표시를 연결하는 작업은 Step 2-4에서 수행한다. Result Online best의 실제 표시도 Step 2-3에서 수행한다.
- [ ] Unity Test Runner 실행은 Step 6에서 사용자가 수행한다. 이 하위 단계에서는 Navigation 상태 Test를 작성했지만 실행하지 않았다.

### Step 2-3. Result·제출 상태를 연결한다

#### AI 작업

1. 로컬 새 최고, 이번 Run의 Pending·Submitted·Rejected와 Online best·순위를 분리해 표시한다.
2. Rejected 사유를 현재 실행 중 Result 상태에만 보존하고 `재시도 불가`로 표시한다. Pending에만 제출 Retry를 제공한다.
3. 복구 제한 동의·취소·Settings 재안내를 생산 UI에 연결한다.

#### 상태

- [x] 최신 사용자 정책 반영: Local Save v5는 Pending 후보를 저장·복원하고 Submitted/Rejected 확정 응답을 받으면 해당 항목만 원자적으로 삭제 저장한다. 삭제 저장 실패 시 메모리·디스크의 Pending을 유지하여 동일 ID로 재확인한다. 완료 ID 목록·거절 사유는 계속 실행 중 메모리에만 보존한다. v1~v4에 Pending 필드가 있으면 복원하고, v4에서 이미 폐기된 후보는 복구할 수 없다. 설정·계정·로컬 최고는 유지한다.
- [x] Pending 재시작 복원, 확정 응답 뒤 삭제, 일시 실패 뒤 동일 ID 재시도, 삭제 저장 실패 시 보존, 기존 Save 이관 Test를 작성·갱신했다. 정적 검증과 Unity 실행 결과는 구분한다.
- [x] Result 표현 상태가 `New local best`, 현재 Run의 Pending/Submitted/Rejected, Rejected 사유·재시도 불가, 별도 조회한 Online best를 분리한다. `RetryPendingSubmission`은 현재 Run이 Pending일 때만 요청하며, `ConfirmOnlineRecoveryNotice`·`CancelOnlineRecoveryNotice` Inspector 어댑터는 명시적 동의/취소 흐름만 제공한다.
- [ ] Result Text와 동의 안내·Settings 재확인·Pending Retry Button의 실제 Scene 연결은 Step 2-4에서 사용자가 수행한다. AI는 Scene을 수정하지 않는다.
- [ ] Unity Test Runner 실행은 Step 6에서 사용자가 수행한다. `OnlineRecordRepositoryTests.RejectedResult_KeepsServerReasonOnlyForCurrentSession`을 추가했지만 실행하지 않았다.

### Step 2-4. UI Component·Scene 연결 명세를 준비한다

#### AI 작업

1. UIManagementSystem이 상위·내 주변 상태를 Text에 반영하도록 구현하고 누락 참조가 게임 진행을 막지 않게 한다.
2. 구현된 Component·직렬화 필드·Button 어댑터 기준으로 SampleScene 연결 명세를 작성한다.
3. Scene 연결 구성 Test를 작성한다.

#### 상태

- [x] `UIManagementSystem`에 Leaderboard Tab·조회 Retry, Result 순위 보기·Pending 제출 Retry·기록 상태, 복구 제한 안내·확인·취소·Settings 재안내의 선택적 직렬화 참조와 null-safe 반영을 추가했다. 새 참조가 비어 있어도 기존 메뉴 부팅·진행은 막지 않는다.
- [x] Result의 Keyboard Navigation은 `Retry → 순위 보기 → Pending일 때만 제출 재시도 → Main Menu` 순서다. Pending이 종료되면 숨겨진 제출 재시도 선택은 순위 보기로 되돌린다. Leaderboard는 `Stage 1 → Infinite → 조회 재시도 → Back` 순서다.
- [x] `ModeUISceneConfigurationTests.Phase4LeaderboardAndResultControls_AreWired`를 작성했다. 이 Test는 아래 이름·참조·Button OnClick을 Scene에 적용한 뒤 Step 6에서 실행한다. 현재 AI는 Scene을 수정하지 않았으므로 Step 4 적용 전 실행하면 실패하는 것이 정상이다.
- [x] 아래 Scene 연결 명세를 확정했다. 사용자 수동 작업은 이 하위 단계가 완료된 뒤 Step 4에서만 수행한다.

#### Step 4 Scene 연결 명세

`SampleScene`의 `UIRoot`에서 다음 오브젝트를 생성하거나 기존 오브젝트를 이 이름으로 구성한다. 모든 새 Text는 TextMeshProUGUI, 모든 새 Button은 Unity `Button`을 사용한다. 새 Button의 Navigation은 `Explicit`으로 설정하고 아래 순서의 이전·다음만 연결한다. Button의 기존 OnClick을 임의로 유지하지 않고 명세의 `GameSystem` 메서드 하나만 등록한다.

| 위치 | 오브젝트 이름·표시 | `UIManagementSystem` 필드 | Button OnClick | Navigation |
| --- | --- | --- | --- | --- |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | `LeaderboardStageButton` / `Stage 1` | `_leaderboardStageButton` | `GameSystem.SelectStage` | Infinite 이전 없음 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | `LeaderboardInfiniteButton` / `Infinite` | `_leaderboardInfiniteButton` | `GameSystem.SelectInfinite` | Stage 이전, Retry 다음 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | `LeaderboardRetryButton` / `Retry` | `_leaderboardRetryButton` | `GameSystem.SelectLeaderboardRetry` | Infinite 이전, Pending Retry 다음 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | `LeaderboardPendingRetryButton` / `Retry Pending (0)` | `_leaderboardPendingRetryButton`, `_leaderboardPendingRetryText` | `GameSystem.SelectLeaderboardPendingRetry` | 조회 Retry 이전, Back 다음. 새 플레이 없이 전체 Pending 재전송 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | 기존 `BackButton` / `Back` | `_leaderboardBackButton` | 기존 `GameSystem.SelectBack` 유지 | Pending Retry 이전, 다음 없음 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | `LeaderboardAccountText` | `_leaderboardAccountText` | 해당 없음 | 해당 없음 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | 기존 `InformationText`를 이름 변경한 `LeaderboardTopText` | `_leaderboardTopText` | 해당 없음 | 해당 없음 |
| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` | `LeaderboardAroundText` | `_leaderboardAroundText` | 해당 없음 | 해당 없음 |
| `UIRoot/ResultPanel/Canvas/ResultWindow` | `ResultRecordStatusText` | `_resultRecordStatusText` | 해당 없음 | 해당 없음 |
| `UIRoot/ResultPanel/Canvas/ResultWindow` | `ResultLeaderboardButton` / `View Leaderboard` | `_resultLeaderboardButton` | `GameSystem.SelectResultLeaderboard` | Retry 이전, Submission Retry 또는 Main Menu 다음 |
| `UIRoot/ResultPanel/Canvas/ResultWindow` | `ResultSubmissionRetryButton` / `Retry Submission` | `_resultSubmissionRetryButton` | `GameSystem.RetryPendingSubmission` | Result Leaderboard 이전, Main Menu 다음. Scene에서는 활성 상태로 두며 Runtime이 Pending이 아닐 때 숨긴다. |
| `UIRoot/ResultPanel/Canvas/ResultWindow` | 기존 `ResultMainMenuButton` / `Main Menu` | `_resultMainMenuButton` | 기존 `GameSystem.SelectMainMenu` 유지 | Submission Retry 또는 Result Leaderboard 이전, 다음 없음 |
| `UIRoot/OnlineRecoveryNoticeCanvas/OnlineRecoveryNoticePanel` | `OnlineRecoveryNoticePanel` | `_onlineRecoveryNoticePanel` | 해당 없음 | 최초 비활성 |
| `UIRoot/OnlineRecoveryNoticeCanvas/OnlineRecoveryNoticePanel` | `OnlineRecoveryNoticeConfirmButton` / `Continue` | `_onlineRecoveryNoticeConfirmButton` | `GameSystem.ConfirmOnlineRecoveryNotice` | Cancel 다음 |
| `UIRoot/OnlineRecoveryNoticeCanvas/OnlineRecoveryNoticePanel` | `OnlineRecoveryNoticeCancelButton` / `Cancel` | `_onlineRecoveryNoticeCancelButton` | `GameSystem.CancelOnlineRecoveryNotice` | Confirm 이전 |
| `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow` | `SettingsRecoveryNoticeButton` / `Online Record Notice` | `_settingsRecoveryNoticeButton` | `GameSystem.OpenOnlineRecoveryNoticeFromSettings` | 기존 Settings Back과 별도 Mouse 접근. Keyboard 접근은 아래 별도 코드 작업이 완료된 뒤 추가 |

`ResultRecordStatusText`, `ResultLeaderboardButton`, `ResultSubmissionRetryButton`은 반드시 기존 `ResultWindow`의 직접 자식으로 둔다. 세 항목이 모두 연결되면 Runtime이 다음 RectTransform을 적용한다. Stage는 ResultWindow `520×620`, StageResultContent `472×150, y=-36`, RecordStatus `472×82, y=-204`, Retry `y=-302`, View Leaderboard `y=-360`, Submission Retry `y=-418`, Main Menu `y=-476`이다. Infinite는 ResultWindow `520×740`, InfiniteResultContent `472×278, y=-36`, RecordStatus `472×82, y=-334`, Retry `y=-432`, View Leaderboard `y=-490`, Submission Retry `y=-548`, Main Menu `y=-606`이다. 각 Button 폭·높이는 `472×48`이며, anchor와 pivot은 상단 중앙 `(0.5, 1)`이다. `ResultWindow`는 중앙 `(0.5, 0.5)` anchor·pivot을 사용한다.

LeaderboardPanel은 기존 전체 화면 overlay를 유지한다. 새 `LeaderboardWindow`는 만들지 않는다. 기존 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList`를 중앙 `860×860` 컨테이너로 확장하고, 위에서부터 Account, Tab 두 개, Top Text, Around Text, 조회 Retry, Pending 제출 Retry, Back, 제출 상태 Text 순서로 배치한다. Top과 Around Text는 서로 독립적인 영역으로 충분한 높이를 주고, Text의 Raycast Target은 끈다. Account는 마스킹 계정 또는 동의 필요 안내만 표시하므로 전체 내부 Player ID를 직접 작성하지 않는다. 복구 안내 overlay는 LeaderboardPanel과 ResultPanel보다 앞의 sibling으로 두고, Panel의 Image Raycast Target을 켜서 뒤 화면 클릭을 막는다.

#### 현재 Scene 기준 상세 UI 구성값

현재 모든 UI Canvas의 `Canvas Scaler`는 `Scale With Screen Size`, Reference Resolution `1920×1080`, Match `Width`다. 아래 값은 이 기준의 `RectTransform` 값이다. 새 Text는 기존 UI의 TextMeshProUGUI font asset과 기본 흰색을 사용하고, Button은 같은 Panel의 기존 Button을 Duplicate해 Image·색상 전환·Text style을 유지한다. 새 Text의 `Raycast Target`은 끄고, 새 Button과 복구 Panel Image의 `Raycast Target`은 켠다.

현재 Scene의 실제 경로는 `UIRoot/MenuCanvas/LeaderboardPanel`이다. 이 Panel의 직접 자식은 `TitleText`와 `MenuList`뿐이며, `MenuList`의 직접 자식은 `InformationText`, `BackButton`뿐이다. 다음 계층으로 완성한다.

```
UIRoot
└─ MenuCanvas
   └─ LeaderboardPanel
      ├─ TitleText                                   (기존: 유지·재사용)
      └─ MenuList                                    (기존: 이름 유지·크기 확장)
         ├─ InformationText → LeaderboardTopText    (기존: 이름 변경·재사용)
         ├─ BackButton                               (기존: 이름·OnClick 유지)
         ├─ LeaderboardAccountText                   (새로 생성)
         ├─ LeaderboardStageButton                   (새로 생성)
         ├─ LeaderboardInfiniteButton                (새로 생성)
         ├─ LeaderboardAroundText                    (새로 생성)
         ├─ LeaderboardRetryButton                   (조회 Retry)
         ├─ LeaderboardPendingRetryButton            (Pending 제출 Retry; 추가)
         └─ LeaderboardPendingRetryStatusText        (마지막 수동 재전송 결과; 추가)
```

`TitleText`, `MenuList`, `InformationText`를 새로 복제하지 않는다. `TitleText`는 제목으로 재배치하고, `MenuList`는 이름을 바꾸지 않은 채 중앙 `860×860`으로 확장한다. 기존 `InformationText`는 **반드시** `LeaderboardTopText`로 이름을 변경하고 기존 TMP 컴포넌트를 재사용한다. 기존 `BackButton`은 이름과 `GameSystem.SelectBack` OnClick을 유지한 채 위치·크기·Navigation만 바꾼다. `LeaderboardPendingRetryButton`과 상태 Text는 새로 추가하며 기존 조회 Retry Button을 대체하지 않는다.

현재 `MenuList`에 붙어 있는 `VerticalLayoutGroup`은 이 표의 절대 `RectTransform` 배치와 충돌한다. **Component를 삭제하지 말고 Enabled 체크를 해제**한다. 이후 각 자식의 Anchor, Position, Size는 아래 표의 값으로 직접 입력한다. `MenuList`의 이름·Transform·기존 자식 자체는 유지한다.

| `UIRoot/MenuCanvas/LeaderboardPanel/MenuList` 내부 대상 | 초기 표시 문자열 | Anchor / Pivot | Anchored Position | Size | TMP 설정 또는 Button Navigation |
| --- | --- | --- | --- | --- | --- |
| 상위 `LeaderboardPanel`의 기존 `TitleText` | `Leaderboard` | top-center / `(0.5, 0.5)` | `(0, -72)` | `780×60` | Font 48, 가운데 정렬, Raycast 끔 |
| 기존 `MenuList` | 해당 없음 | center / `(0.5, 0.5)` | `(0, 0)` | `860×860` | 내부 배치의 컨테이너 |
| `LeaderboardAccountText` | `Anonymous account: consent required` | top-center / `(0.5, 1)` | `(0, -32)` | `780×32` | Font 24, 가운데 정렬, Runtime이 마스킹 계정 또는 동의 문구로 갱신 |
| `LeaderboardStageButton` | `Stage 1` | top-center / `(0.5, 1)` | `(-104, -82)` | `200×48` | Explicit: Down `LeaderboardInfiniteButton` |
| `LeaderboardInfiniteButton` | `Infinite` | top-center / `(0.5, 1)` | `(104, -82)` | `200×48` | Explicit: Up `LeaderboardStageButton`, Down `LeaderboardRetryButton` |
| 기존 `InformationText`에서 이름 변경한 `LeaderboardTopText` | `TOP\n--` | top-center / `(0.5, 1)` | `(0, -150)` | `780×270` | Font 24, left/top 정렬, word wrap 켬, Runtime 갱신 |
| `LeaderboardAroundText` | `AROUND YOU\n--` | top-center / `(0.5, 1)` | `(0, -436)` | `780×160` | Font 24, left/top 정렬, word wrap 켬, Runtime 갱신 |
| `LeaderboardRetryButton` | `Retry` | top-center / `(0.5, 1)` | `(0, -612)` | `360×48` | 조회 전용; OnClick `GameSystem.SelectLeaderboardRetry`; Explicit: Up `LeaderboardInfiniteButton`, Down `LeaderboardPendingRetryButton` |
| `LeaderboardPendingRetryButton` | `Retry Pending (0)` | top-center / `(0.5, 1)` | `(0, -672)` | `360×48` | 새 Button; OnClick `GameSystem.SelectLeaderboardPendingRetry`; Explicit: Up `LeaderboardRetryButton`, Down 기존 `BackButton`. 자식 TMP 이름 `LeaderboardPendingRetryButtonText` |
| 기존 `MenuList/BackButton` | `Back` 유지 | top-center / `(0.5, 1)` | `(0, -732)` | `360×48` | OnClick `GameSystem.SelectBack` 유지; Explicit: Up `LeaderboardPendingRetryButton` |
| `LeaderboardPendingRetryStatusText` | `No manual retry yet` | top-center / `(0.5, 1)` | `(0, -790)` | `780×32` | Font 20, center/middle, Raycast 끔; 수동 Retry 결과의 Submitted·Rejected·Pending 건수 표시 |

`LeaderboardAccountText`, `LeaderboardTopText`, `LeaderboardAroundText`, 각 Button은 `UIManagementSystem`의 해당 직렬화 필드에 연결한다. 새 Pending Button은 `_leaderboardPendingRetryButton`, 그 자식 TMP는 `_leaderboardPendingRetryText`, 상태 TMP는 `_leaderboardPendingRetryStatusText`에 연결한다. `Retry Pending (N)`의 N은 Local Save에서 복원된 Pending을 포함한 현재 개수다. **N=0이면 Button은 숨겨지고 선택 대상에서도 빠진다.** N>0일 때만 표시된다. 클릭 즉시 `Retrying pending...`으로 바뀌며 응답을 기다리는 동안 비활성화되고 Keyboard 선택에서도 빠진다. Pending이 남은 채 시도가 종료되면 다시 활성화된다. 처리 후 N=0이 되어 Button이 사라질 때 포커스가 그 Button에 있었다면 조회 `Retry`로 옮긴다. 완료 후 상태 Text는 마지막 수동 Retry의 Submitted·Rejected·남은 Pending 건수를 표시한다. 조회 Retry는 서버 목록만 다시 조회하고, Pending Retry는 새 플레이 없이 저장된 제출 후보를 재전송한다. Top과 Around의 실제 문자열은 Runtime이 갱신하므로 Scene에는 위 초기값만 둔다. 표시 중 Player ID를 직접 작성하지 않는다.

`ResultWindow`의 저장된 Scene 초기값은 Stage 또는 Infinite 레이아웃으로 강제하지 않는다. Stage/Infinite Content, `RetryButton`, `MainMenuButton`의 직접 자식에 아래 세 오브젝트를 추가한다. 실행 중 Runtime이 현재 Mode에 따라 모든 Result RectTransform을 아래 값으로 다시 설정한다. 저장된 Scene 값은 구조·참조 확인만 대상으로 하고, Stage/Infinite 좌표는 Runtime Test에서 각각 검증한다.

| `ResultWindow` 직접 자식 | 초기 표시 문자열 | Anchor / Pivot | Stage 위치·크기 | Infinite 위치·크기 | 연결 |
| --- | --- | --- | --- | --- | --- |
| `ResultRecordStatusText` | 빈 문자열 | top-center / `(0.5, 1)` | `y=-204`, `472×82` | `y=-334`, `472×82` | `_resultRecordStatusText`; Font 20, left/top, word wrap 켬, Raycast 끔 |
| 기존 `RetryButton` | `Retry` 유지 | top-center / `(0.5, 1)` | `y=-302`, `472×48` | `y=-432`, `472×48` | Explicit: Down `ResultLeaderboardButton` |
| `ResultLeaderboardButton` | `View Leaderboard` | top-center / `(0.5, 1)` | `y=-360`, `472×48` | `y=-490`, `472×48` | `_resultLeaderboardButton`; OnClick `GameSystem.SelectResultLeaderboard`; Explicit: Up Retry, Down Submission Retry |
| `ResultSubmissionRetryButton` | `Retry Submission` | top-center / `(0.5, 1)` | `y=-418`, `472×48` | `y=-548`, `472×48` | `_resultSubmissionRetryButton`; OnClick `GameSystem.RetryPendingSubmission`; Explicit: Up Result Leaderboard, Down Main Menu; Scene에서는 활성, Runtime이 Pending이 아닐 때 숨김 |
| 기존 `MainMenuButton` | `Main Menu` 유지 | top-center / `(0.5, 1)` | `y=-476`, `472×48` | `y=-606`, `472×48` | Explicit: Up Submission Retry. Submission Retry가 숨겨졌을 때 Runtime Navigation은 Result Leaderboard로 이동 |

`ResultWindow`는 Stage에서 `520×620`, Infinite에서 `520×740`이다. Stage Result Content는 `y=-36`, `472×150`, Infinite Result Content는 `y=-36`, `472×278`을 유지한다. 기존 `RetryButton`과 `MainMenuButton`의 OnClick은 변경하지 않는다.

`UIRoot` 자체에는 Canvas가 없으므로 `OnlineRecoveryNoticePanel`을 직접 자식으로 만들지 않는다. `UIRoot`의 마지막 sibling으로 `OnlineRecoveryNoticeCanvas`를 새로 만들고, Canvas·Graphic Raycaster·Canvas Scaler를 추가한다. 이 Canvas는 루트 Canvas이므로 Screen Space - Overlay, Sorting Order `2`로 설정하고 **Override Sorting은 기본값(끔)**으로 둔다. Canvas Scaler는 `Scale With Screen Size`, Reference Resolution `1920×1080`, Screen Match Mode `Match Width Or Height`, Match `0`으로 설정한다. 그 아래에 `OnlineRecoveryNoticePanel`을 만들고 full stretch anchor `(0,0)~(1,1)`, pivot `(0.5,0.5)`, position `(0,0)`, size delta `(0,0)`, Image 색상 `#000000DD`, Raycast Target 켬, 최초 비활성으로 둔다. 자식 `RecoveryWindow`는 center anchor/pivot, position `(0,0)`, size `520×300`, Image 색상 `#111827FF`를 사용한다.

| `RecoveryWindow` 자식 | 초기 표시 문자열 | Anchor / Pivot | Anchored Position | Size | 연결·Navigation |
| --- | --- | --- | --- | --- | --- |
| `RecoveryNoticeTitleText` | `Online Record Notice` | top-center / `(0.5, 1)` | `(0, -30)` | `456×42` | Font 32, 가운데 정렬, Raycast 끔 |
| `RecoveryNoticeBodyText` | `Online records are tied to this anonymous account.\nThey cannot be recovered after app data is lost.\nContinue to enable online record access.` | top-center / `(0.5, 1)` | `(0, -88)` | `456×104` | Font 20, 가운데 정렬, word wrap 켬, Raycast 끔 |
| `OnlineRecoveryNoticeCancelButton` | `Cancel` | top-center / `(0.5, 1)` | `(-120, -232)` | `220×48` | `_onlineRecoveryNoticeCancelButton`; OnClick `GameSystem.CancelOnlineRecoveryNotice`; Explicit: Right Confirm |
| `OnlineRecoveryNoticeConfirmButton` | `Continue` | top-center / `(0.5, 1)` | `(120, -232)` | `220×48` | `_onlineRecoveryNoticeConfirmButton`; OnClick `GameSystem.ConfirmOnlineRecoveryNotice`; Explicit: Left Cancel |

`UIRoot/OnlineRecoveryNoticeCanvas/OnlineRecoveryNoticePanel`과 두 Button을 `UIManagementSystem`에 연결한다. Panel이 활성화되면 Runtime은 Confirm을 선택한다.

#### SettingsWindow 전체 배치값

실제 경로는 `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow`다. `SettingsWindow`는 center anchor/pivot, position `(0,0)`, size `960×920`을 유지한다. 새 `SettingsRecoveryNoticeButton`을 추가하면 기존 `RestoreDefaultsButton`의 현재 범위와 겹친다. 하단 세 Button을 아래의 3열 배치로 **함께** 옮긴다. 나머지 기존 오브젝트는 현재 값을 유지한다.

| `SettingsWindow` 직접 자식 | Anchor / Pivot | Anchored Position | Size | 처리 |
| --- | --- | --- | --- | --- |
| `TitleText` | center / `(0.5, 0.5)` | `(0, 398)` | `860×50` | 유지 |
| `SubtitleText` | center / `(0.5, 0.5)` | `(0, 360)` | `860×24` | 유지 |
| `AudioSectionTitle` | middle-left / `(0, 0.5)` | `(50, 302)` | `300×28` | 유지 |
| `MasterVolumeRow` | center / `(0.5, 0.5)` | `(0, 252)` | `860×62` | 유지 |
| `DisplaySectionTitle` | middle-left / `(0, 0.5)` | `(50, 192)` | `300×28` | 유지 |
| `FullscreenRow` | center / `(0.5, 0.5)` | `(0, 142)` | `860×62` | 유지 |
| `ControlsSectionTitle` | middle-left / `(0, 0.5)` | `(50, 82)` | `300×28` | 유지 |
| `ControlsGuideText` | middle-left / `(0, 0.5)` | `(80, 52)` | `840×22` | 유지 |
| `ControlsScrollView` | center / `(0.5, 0.5)` | `(0, -96)` | `860×252` | 내부 Viewport·Content·Binding 행의 위치와 크기는 변경하지 않음 |
| `RebindStatusText` | center / `(0.5, 0.5)` | `(0, -248)` | `860×24` | 유지 |
| 기존 `RestoreDefaultsButton` | center / `(0.5, 0.5)` | `(0, -330)` | `220×44` | 기존 `SettingsUIController` 참조·OnClick 유지. 현재 `(-120,-330)`, `250×44`에서 변경 |
| `SettingsRecoveryNoticeButton` | center / `(0.5, 0.5)` | `(-240, -330)` | `220×44` | 표시 `Online Record Notice`; `_settingsRecoveryNoticeButton`; OnClick `GameSystem.OpenOnlineRecoveryNoticeFromSettings` |
| 기존 `BackButton` | center / `(0.5, 0.5)` | `(240, -330)` | `220×44` | 기존 `GameSystem.SelectBack` OnClick 유지. 현재 `(180,-330)`에서 변경 |
| `RestoreDefaultsConfirmationPanel` | stretch / `(0.5, 0.5)` | `(0, 0)` | `(0, 0)` | 최상위 sibling으로 유지. 비활성 시작; 내부 Window와 Button 값은 변경하지 않음 |

세 하단 Button의 가로 범위는 각각 `[-350,-130]`, `[-110,110]`, `[130,350]`이다. 따라서 서로 20px 간격을 유지하고, `SettingsWindow`의 좌우 경계(`-480`, `480`) 안에 들어간다. 새 Button만 `(-180,-330)`, 기존 Restore Defaults를 `(-120,-330)`에 유지하면 두 Button이 겹치므로 사용하지 않는다.

`SettingsRecoveryNoticeButton`을 `SettingsWindow`의 직접 자식으로 만들고 위 표의 값과 OnClick을 설정한다. `_settingsRecoveryNoticeButton`에 연결한다.

현재 `GameNavigationState`의 Settings 화면은 Back만 선택 항목으로 가진다. 따라서 위 Settings 안내 Button은 Scene 연결만으로는 Mouse Click만 지원하며 Keyboard Navigation에 들어가지 않는다. Keyboard로 이 Button까지 접근하게 하려면 별도 코드 작업으로 Settings용 Navigation Item·상태 전환·UI button mapping·자동 Test를 추가해야 한다. 이 작업이 완료되기 전에는 Settings 안내 Button을 Keyboard 필수 경로로 판정하지 않는다.

### Step 2-5. 자동 Test와 정적 검사 준비를 마무리한다

#### AI 작업

1. Repository 대역으로 성공·Empty·Offline·Timeout·인증 실패·역순 응답을 재현하는 Test를 작성한다.
2. Navigation·Result·Scene 구성의 Edit/Play Mode Test를 작성하고 Step 6 실행 순서를 확정한다.
3. 코드·asmdef·Cloud Code 정적 검사를 수행해 Step 3 입력을 준비한다.

#### 상태

- [x] Repository 대역의 Submitted·Rejected·TransientFailure·Timeout·인증 실패·Empty 성공 응답과 Pending 저장/재시작/삭제 실패를 `OnlineRecordRepositoryTests`와 `LocalSaveJsonCodecTests`에 작성했다. 일반 연결 실패는 `ClientFailure`, Timeout은 `Timeout`으로 분리해 확인한다.
- [x] 공동 순위·Empty·Offline·Error·늦은 응답 무시를 `RecordLeaderboardPolicyTests`와 `LeaderboardViewStateTests`로, Result Pending 전용 Retry와 Result↔Leaderboard 복귀를 `GameNavigationStateTests`로 작성했다. Scene 참조·OnClick 연결은 `ModeUISceneConfigurationTests.Phase4LeaderboardAndResultControls_AreWired`로 작성했다.
- [x] 코드/asmdef/Cloud Code 정적 검사 입력을 준비했다. 이후 Step 3에서 Node 설치 확인 후 `UGS/Tests/static-contracts.cjs` 및 `UGS/Tests/cloud-code.test.cjs` 실행을 완료했다. Unity Test Runner와 Scene 수정은 수행하지 않았다.

#### Step 6 실행 순서

1. Unity Script Compilation 성공 후 Edit Mode에서 다음 집중 fixture를 실행한다: `OnlineRecordRepositoryTests`, `OnlineRecordConfigurationTests`, `LocalSaveJsonCodecTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`, `LeaderboardViewStateTests`, `GameNavigationStateTests`.
2. 위 결과가 성공하면 전체 Edit Mode를 실행한다.
3. Step 4의 Scene 연결이 끝난 뒤 Play Mode에서 `ModeUISceneConfigurationTests`, `LocalPersistenceIntegrationTests`, `InfiniteModeIntegrationTests`를 실행한다. `Phase4LeaderboardAndResultControls_AreWired`는 새 Scene 오브젝트·Inspector 참조·OnClick을 검사하므로 Step 4 전에는 실패가 정상이다.
4. 위 결과가 성공하면 전체 Play Mode를 실행한다. 각 실행의 총수·성공·실패·무시 수와 예상하지 않은 Error/Warning을 기록한다.

### Step 2 완료 조건

- [x] Step 2-1~2-5의 코드·Test·Scene 연결 명세가 준비되고, AI가 실행 가능한 정적 검사에서 누락이 없다. Unity 컴파일/Scene 적용/Test Runner 실행은 사용자 담당 Step 4·6에서 진행한다.

## Step 3. 코드와 서비스 구성을 정적으로 검증한다

### AI 작업

1. 참조/asmdef·직렬화 필드·이벤트 시그니처, Board/Version/환경 매핑, SDK 의존 방향, 비밀값 미포함을 검사한다.
2. 화면 계약별 테스트 대응, 미동의 전 요청 차단, 계정 불일치, 제출 상태 보존, 늦은 응답 폐기, 객체 해제·이벤트 해제를 대조한다.
3. 기존 Scene을 읽어 연결 명세가 실제 대상과 일치하는지 확인한다. 정적으로 확인 가능한 오브젝트 경로·참조·상수를 사용자에게 재확인시키지 않는다.
4. 서버 변경이 있다면 로컬 Cloud Code 대역 테스트·정적 계약 검사를 수행하고 변경된 스크립트·정책·데이터 호환성과 적용 순서를 명시한다. 실행 도구가 없으면 성공으로 기록하지 않고 AI가 사용 가능한 실행 경로를 조사한다.

### 사용자 수동 작업

Node.js가 설치되지 않은 개발 PC에서는 아래 절차로 실행 환경을 한 번 구성한다. 설치 후 정적 검사와 Cloud Code 대역 테스트 실행은 AI가 담당한다.

#### Windows Node runtime 구성

Node.js는 이 프로젝트의 `.cjs` 검사 파일을 개발 PC에서 실행하는 용도다. 별도 원격 서버, UGS 설정 변경, 게임 배포 파일에 Node 포함, 상시 실행 프로세스는 필요 없다.

1. [Node.js 공식 다운로드](https://nodejs.org/en/download)에서 **LTS**로 표시된 버전을 선택한다. Windows x64 개발 PC에서는 **Windows / x64 / Windows Installer (.msi)**를 선택한다. 게임의 대상 플랫폼과 개발 PC의 CPU 아키텍처가 다른 경우에는 개발 PC에 맞는 설치 파일을 사용한다.
2. 설치 파일을 실행한다. `Node.js runtime`과 `Add to PATH` 항목을 유지하고 설치한다. 기본 설치 위치는 일반적으로 `C:\Program Files\nodejs`다. npm은 기본값으로 함께 설치해도 되지만 이 검증에는 사용하지 않는다.
3. 선택 항목으로 native module용 추가 도구 설치가 나오면 선택하지 않아도 된다. 이 테스트는 Node 내장 모듈과 저장소의 서비스 대역을 사용하므로 `npm install`, Python, Visual Studio Build Tools 설치가 필요 없다.
4. 설치 후 새 PowerShell 창을 연다. 기존 프로세스에는 변경된 PATH가 반영되지 않을 수 있다.
5. 아래 명령으로 인식 여부를 확인한다. 버전과 `node.exe` 경로가 출력되면 설치가 인식된 것이다.

```powershell
node --version
Get-Command node | Select-Object -ExpandProperty Source
```

`node`를 찾지 못하면 기본 설치 경로를 직접 확인한다. 다른 위치에 설치했다면 실제 경로로 바꾼다.

```powershell
& 'C:\Program Files\nodejs\node.exe' --version
```

직접 경로 실행은 성공하지만 `node --version`이 실패하면 PATH 반영 문제다. 새 터미널을 사용하거나 설치 프로그램의 PATH 설정을 확인한다. AI에는 설치 완료와 `node.exe` 경로를 알려주면 된다. AI는 절대 경로로 실행할 수 있으므로 기존 AI 세션의 PATH 때문에 설치를 반복할 필요는 없다.

#### 설치 후 AI가 수행할 검사

프로젝트 루트에서 아래 두 검사를 각각 실행하고 종료 코드와 결과를 기록한다. 사용자가 테스트를 대신 수행할 필요는 없다.

```powershell
Set-Location -LiteralPath 'C:\Unity\Unity_Flow_State'
node .\UGS\Tests\static-contracts.cjs
node .\UGS\Tests\cloud-code.test.cjs
```

PATH에서 실행 파일을 찾을 수 없으면 각 명령의 `node`를 `& 'C:\Program Files\nodejs\node.exe'`로 바꾼다. 첫 검사는 `PASS static contracts: ...`, 두 번째는 개별 `PASS`와 최종 `N/N passed`를 출력하며, 둘 다 종료 코드 `0`이어야 통과다. 실패가 있으면 AI가 원인을 확인하고 필요한 수정 후 다시 실행한다.

두 검사는 네트워크 연결·UGS 인증·실제 서버 데이터 변경 없이 실행한다. Unity Build나 Unity Test Runner 실행도 포함하지 않는다. 이 결과는 실제 UGS 통합 검증을 대체하지 않으며, 설치만으로 Step 3을 완료 처리하지 않는다.

### 완료 조건

- [x] 정적 검사와 필요한 로컬 서버 테스트가 통과하고 Scene 연결·원격 적용 대상이 확정됐다. 실제 Unity 컴파일·Scene 적용·원격 배포·Unity 테스트 통과를 의미하지 않는다.

### Step 3 정적 대조 결과

- [x] Runtime class 중복, 새 Phase 4 source의 `System.Linq`·새 null 조건/병합 문법, UI adapter·Navigation·Pending 제거 경로, asmdef UGS 참조, Local Save의 Pending 전용 영속 범위를 정적으로 대조했다.
- [x] `submit-record.js`의 service token/CAS write lock, `query-records.js`의 서버 수락 시각 오름차순과 같은 Score 공동 순위, verification Project·Environment·Board ID, Player 직접 Write Deny 및 클라이언트의 Cloud Code 경계만 사용함을 대조했다. 저장소에 player access token이나 JWT 형태의 비밀값은 발견하지 못했다.
- [x] Scene을 읽기 전용으로 대조했다. 기존 Back 참조는 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList/BackButton`이며, 새 Leaderboard/Result/복구 안내 오브젝트는 Step 4에서 사용자가 추가·연결할 대상이다. 이 불완전 상태는 Step 4 전에는 정상이다.
- [x] 사용자 설치 후 `C:\Program Files\nodejs\node.exe` v24.21.0으로 두 검사를 실행했다. `static-contracts.cjs`는 PASS, `cloud-code.test.cjs`는 21/21 passed이며 각각 종료 코드 0이다. 네트워크·UGS 인증·원격 쓰기는 수행하지 않았다.
- 최초 정적 검사에서는 검사 코드가 Resource를 `urn:ugs:leaderboards:*`/`urn:ugs:cloud-save:*`로 기대해 실패했다. 기존 정책 파일의 `urn:ugs:leaderboards:/**`/`urn:ugs:cloud-save:/**`와 일치하도록 검사 기대값과 실패 메시지를 수정한 뒤 통과했다. 정책 파일 자체는 변경하지 않았다. 이전 문자열 대조만으로 통과했다고 기록했던 정책 검사는 이번 실제 실행 결과로 보완한다. 실제 원격 권한 강제력은 후속 서비스 검증 대상이다.

#### Step 5 원격 적용 대상·순서

현재 저장소 diff 기준 서버 변경은 `UGS/CloudCode/query-records.js` 하나다. 같은 Score의 순위를 공유하도록 조회 계산만 변경했고 응답 필드, 입력 `request` String, Board ID, 서버 metadata와 ledger 형식은 유지한다. 데이터 이관이나 기존 기록 삭제는 필요 없다.

1. Step 5에서 사용자가 지정된 verification 환경의 현재 `query-records` 게시 소스와 입력 정의를 백업한다. 원격 상태는 이번 로컬 검사로 확인한 것이 아니므로 대상이 다르면 적용을 중단한다.
2. 로컬 `UGS/CloudCode/query-records.js`를 같은 endpoint에 적용·게시한다. `submit-record`, Access Control, Cloud Save seed 및 기존 Board는 이번 diff에 변경이 없어 재적용·초기화하지 않는다.
3. 적용 후 Step 7의 생산 UI 통합 검증으로 조회를 확인한다. 문제가 생기면 백업한 `query-records` 소스·입력 정의를 복원·게시한다. 롤백 시 이전 동점 순위 규칙으로 돌아가므로 새 계약 검증은 미완료로 기록한다.

## Step 4. Unity에서 컴파일하고 UI를 Scene에 연결한다

### 사용자 수동 작업

1. Play Mode를 종료하고 Unity Editor의 Script Compilation 완료를 확인한다. Compile Error가 있으면 연결 작업을 멈추고 오류를 전달한다.
2. Step 2~3에서 제공한 명세의 Scene을 연다. 지정된 Leaderboard·Result·Settings UI 오브젝트에 명세의 Component·표시 요소·Button을 추가 또는 연결한다.
3. 명세대로 Inspector 참조, Button 이벤트, Keyboard Navigation, 초기 표시 상태 및 레이아웃을 설정하고 Scene을 저장한다. 명세에 없는 대상을 추측해 연결하지 않는다.
4. 적용 완료 여부와 컴파일의 예상하지 않은 Error/Warning을 전달한다.

### AI 후속 작업

저장된 Scene diff를 읽기 전용으로 검사해 누락 참조·중복 이벤트·잘못된 대상·범위 밖 변경을 식별하고 필요한 수정 방법만 안내한다. Scene을 직접 고치지 않는다.

### 완료 조건

- [x] 컴파일과 Scene 연결 정적 검사가 통과했다. ResultWindow의 Mode별 RectTransform은 저장된 Scene 초기값이 아니라 Step 6 Runtime Test에서 검사한다. 실제 설정 테스트는 Step 6에서 수행한다.

### Step 4 사전 정적 대조 결과

- [x] `SampleScene`을 읽기 전용으로 확인했다. 기존 연결 대상은 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList/BackButton`, `UIRoot/ResultPanel/Canvas/ResultWindow`, `UIRoot/MenuCanvas/SettingsPanel/SettingsWindow`이며 `_leaderboardBackButton` 참조도 존재한다.
- [x] 저장된 Scene을 다시 읽기 전용으로 대조했다. `LeaderboardTopText`, `LeaderboardAroundText`, `LeaderboardAccountText`, 세 Leaderboard Button, Result 상태 Text·두 Button, 복구 안내 Canvas·Panel·두 Button, Settings 안내 Button이 모두 정확한 부모 아래 존재한다. `UIManagementSystem`의 신규 직렬화 필드는 각각 해당 TMP/Button/Panel을 참조한다.
- [x] 각 신규 Button의 Persistent OnClick은 각각 `SelectStage`, `SelectInfinite`, `SelectLeaderboardRetry`, `SelectResultLeaderboard`, `RetryPendingSubmission`, `ConfirmOnlineRecoveryNotice`, `CancelOnlineRecoveryNotice`, `OpenOnlineRecoveryNoticeFromSettings`을 `GameSystem`에 연결한다. 중복 OnClick은 발견하지 못했다.
- [x] Leaderboard의 `MenuList`는 `860×760`이며 `VerticalLayoutGroup`이 없어 명세의 개별 RectTransform과 충돌하지 않는다. Leaderboard·Settings의 신규/이동 대상 위치와 크기, 복구 Panel의 최초 비활성·full stretch, 복구 Canvas의 `1920×1080`·Sorting Order `2`도 명세와 대조했다.
- [x] 사용자가 Unity Editor의 Script Compilation 성공 및 예상하지 않은 Error/Warning 부재를 확인했다. AI는 Unity Editor·Build를 실행하지 않았다.
- [x] Step 6 Play Mode의 ResultWindow 초기값은 `520×740`임을 확인했다. 이는 Mode 전환 Runtime 값이며 저장된 Scene을 Stage 값으로 강제할 이유가 없다. 정적 Hierarchy Test는 모드 의존 크기·위치를 검사하지 않도록 수정했고, Stage/Infinite 각각의 값은 Runtime Test에서 검사한다.
- [ ] Unity Test Runner 실행은 Step 6에서 사용자가 수행한다.

## Step 5. 필요한 서비스 변경만 verification에 수동 적용한다

### AI 작업

Phase 3 배포와 변경 목록을 비교한다. 새 공동 순위 규칙은 현재 `query-records.js`와 다르므로 Step 2~3에서 서버 코드와 테스트를 갱신하고 verification 적용 절차를 준비한다. 실제 diff를 기준으로 정확한 파일·endpoint·설정·적용 순서·되돌리기 절차를 제공한다. 적용하지 않은 서버 변경을 UI 구현만으로 완료 처리하지 않는다.

### 사용자 수동 작업 — Phase 4 원격 게시

1. Unity Dashboard에서 Project `c76d55cf-7846-494b-9dce-a0797b179b36`, Environment `verification` (`a20a46fa-1edb-4d79-9c35-02f2fed31896`)을 확인한다. 다르면 중단한다.
2. Cloud Code → JavaScript Scripts의 기존 `query-records`를 열고, 현재 게시 소스와 입력 정의를 안전한 사용자 보관 위치에 백업한다. 입력이 `request` / `String` / Required가 아니거나 예상과 다른 변경이 있으면 게시하지 말고 차이를 알린다.
3. 로컬 `UGS/CloudCode/query-records.js` 전체로 **`query-records` 하나만** 교체하고 Save 후 Publish한다. 입력 정의는 바꾸지 않는다.
4. `submit-record`, Access Control, Cloud Save, Leaderboards 및 ledger는 열거나 변경·재적용·초기화하지 않는다. Dashboard의 Player context 없는 실행 버튼으로 제출 성공을 판정하지 않는다.
5. 게시 성공/실패, 확인한 Project·Environment, `query-records` 백업 여부만 전달한다. 소스 전문, 인증 정보, token, 전체 Player ID는 전달하지 않는다. 실제 서비스 조회 검증은 Step 7에서 한다.

### 완료 조건

- [x] 필요한 원격 변경이 적용됐거나 변경 없음이 명시됐다.

### Step 5 정적 대조·적용 범위

- [x] 이번 Phase 4에서 verification 원격 변경 대상은 `query-records` JavaScript Script 하나다. `acceptedAt`까지 같을 때만 공동 순위였던 계산을 같은 Score면 공동 순위로 바꾸고, 동일 Score 안의 표시 순서는 기존 서버 수락 시각 오름차순을 유지한다.
- [x] 입력은 기존과 동일하게 `request` 하나, 형식 `String`, Required이며 `module.exports.params` 및 응답 필드를 변경하지 않았다. 따라서 Dashboard의 입력 추가·삭제·변경은 필요 없다.
- [x] `submit-record`, Access Control 정책, Cloud Save ledger/seed, 두 Leaderboard, Project/Environment 설정은 이번 변경 대상이 아니다. 재게시·재적용·초기화하지 않는다.
- [x] `C:\Program Files\nodejs\node.exe`로 `UGS/Tests/static-contracts.cjs`와 `UGS/Tests/cloud-code.test.cjs`를 다시 실행했다. 각각 PASS 및 21/21 passed, 종료 코드 0이다. 네트워크·UGS 인증·원격 쓰기는 수행하지 않았다.
- [x] 사용자가 verification Cloud Code의 `query-records.js` 내용 변경을 적용했다고 확인했다. 게시된 입력 계약·원격 응답·실제 Player context 동작은 Step 7에서 검증한다.

## Step 6. Unity Test Runner로 UI·저장·게임 회귀를 실행한다

### AI 작업

아래 검증 표에 따라 생산 코드를 사용하는 테스트를 작성하고, 실제 생성된 fixture 이름과 실행 순서를 제공한다. 기존 `OnlineRecordRepositoryTests`, `OnlineRecordConfigurationTests`, `LocalSaveJsonCodecTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`를 재사용·확장한다. 새 fixture 이름은 구현 전 확정 사실로 기록하지 않는다. Unity Test Runner 실행은 사용자만 수행한다.

### 사용자 수동 작업

1. 최신 Script Compilation 성공을 확인한다. 확인된 뒤 `Window > General > Test Runner`를 열고 Edit Mode를 선택한다.
2. 아래 Edit Mode fixture를 fixture 단위로 선택해 **Run Selected** 한다. 하나라도 실패하면 전체 Suite로 진행하지 말고 실패 Test 이름·메시지·Stack Trace를 전달한다.

   - `OnlineRecordRepositoryTests`
   - `OnlineRecordConfigurationTests`
   - `LocalSaveJsonCodecTests`
   - `RecordSubmissionPolicyTests`
   - `RecordLeaderboardPolicyTests`
   - `LeaderboardViewStateTests`
   - `GameNavigationStateTests`

3. 위 집중 실행이 모두 성공하면 Edit Mode의 검색을 지우고 **Run All**을 한 번 실행한다.
4. 이어서 Play Mode에서 아래 fixture를 fixture 단위로 **Run Selected** 한다. `ModeUISceneConfigurationTests`에는 `Phase4LeaderboardAndResultControls_AreWired`가 포함되어 있어 Scene 오브젝트·Inspector 참조·OnClick을 다시 검사한다.

   - `ModeUISceneConfigurationTests`
   - `LocalPersistenceIntegrationTests`
   - `InfiniteModeIntegrationTests`

5. 위 집중 실행이 모두 성공하면 Play Mode의 검색을 지우고 **Run All**을 한 번 실행한다. Edit/Play Mode의 각 집중 실행과 전체 실행에 대해 총수·성공·실패·무시 수 및 예상하지 않은 Error/Warning 유무를 전달한다. 실패는 Test 이름·메시지·Stack Trace를 포함한다.

Unity Editor Build, Dashboard 작업, Scene/Inspector 변경은 이 Step에 포함하지 않는다. Scene 정적 연결은 Step 4에서 완료됐으므로 Test 실패를 해결하기 위해 Scene을 추측해 수정하지 않는다.

### 완료 조건

- [x] 최신 변경의 전체 Edit/Play Mode가 통과했다. 필수 사례 누락·실패·예상하지 않은 Error/Warning은 해결됐으며 무시된 테스트가 있으면 사유와 영향이 기록됐다.

### Step 6 실행 준비 정적 대조

- [x] 집중 Edit Mode fixture `OnlineRecordRepositoryTests`, `OnlineRecordConfigurationTests`, `LocalSaveJsonCodecTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`, `LeaderboardViewStateTests`, `GameNavigationStateTests`의 소스와 Phase 4 사례를 읽기 전용으로 확인했다.
- [x] 집중 Play Mode fixture `ModeUISceneConfigurationTests`, `LocalPersistenceIntegrationTests`, `InfiniteModeIntegrationTests`의 소스와 `Phase4LeaderboardAndResultControls_AreWired` Scene 연결 검사를 읽기 전용으로 확인했다.
- [x] Test source·UGS 대역 Test source의 변경 형식은 `git diff --check`로 확인했다. 이는 Unity Test Runner 결과가 아니다.
- [x] 사용자 집중 Edit Mode 실행에서 `GameNavigationStateTests.ResultSubmissionRetry_IsNavigableOnlyWhilePending` 실패를 보고했다. Pending 가용 상태가 실제로 바뀌어도 `SetResultSubmissionRetryAvailable(true)`가 `false`를 반환하던 원인을 수정했다. 이제 가용 상태가 바뀌면 `true`, 값이 같으면 `false`를 반환하며 Submission Retry 선택이 사라질 때에는 계속 Result Leaderboard로 이동한다.
- [x] 사용자 집중 Play Mode 실행에서 두 ResultWindow 크기 기대값 불일치를 보고했다. 정적 Hierarchy Test에서 모드 의존 크기·위치를 단정하던 검사를 제거하고, Mode 전환 Runtime Test에서만 Phase 4 계약(Stage `520×620`, Infinite `520×740`)과 Result 상태·두 신규 Button 위치를 검사하도록 갱신했다. Scene 수정은 필요 없다.
- [x] 사용자가 수정 후 Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 부재를 확인했다. 전체 Edit Mode 719개와 전체 Play Mode 233개를 각각 시도해 모두 성공했고, 각 실행에서 예상하지 않은 Error/Warning도 없었다. AI는 Unity Test Runner를 실행하지 않았다.
- [x] 생산 Scene Play Mode Test가 실제 Local Save·동의 계정·verification 서버를 사용하지 않도록 Editor Test 전용 메모리 저장소와 온라인 서비스 차단을 추가했다. 최초 격리 Test의 Scene 재로드에서 `InfiniteMapPattern.OnEnable` 예외가 보고되어, 해당 Test는 Scene을 로드하지 않고 비활성 오브젝트의 기록 초기화만 검사하도록 수정했다. 사용자가 `PlayModeRecordIsolationTests.RecordInitialization_DoesNotUsePersistentSaveOrOnlineServices`와 전체 Play Mode를 포함해 컴파일 성공, Edit Mode **729/729**, Play Mode **234/234**, 예상 밖 Error/Warning 없음을 확인했다. 기존 서버의 초단기 기록은 자동 삭제하지 않는다.

## Step 7. 생산 UI의 실제 서비스 연결과 Offline 복구를 확인한다

### 사전 조건 및 AI 준비

Step 4~6 성공 후 진행한다. 여기서는 Editor의 보조 검증 창이 아니라 실제 게임 화면에서 서버 조회·제출·Offline 복구를 확인한다. AI는 Scene, Unity Build, Unity Test Runner를 실행하지 않는다.

#### Step 7 정적 준비 결과

- [x] 클라이언트는 verification Project `c76d55cf-7846-494b-9dce-a0797b179b36`, Environment `a20a46fa-1edb-4d79-9c35-02f2fed31896`, Stage `fs-stage-stage-001-r1`, Infinite `fs-infinite-v2`로만 Cloud Code `query-records`를 호출하도록 대조했다.
- [x] 생산 Leaderboard는 Top과 Around를 독립 호출한다. Main Menu에는 별도의 `내 최고 기록` 영역·조회가 없다. Top/Around는 `Loading`, `No records.`, `Offline. Retry when connected.`, `Error: <safe reason>`을 구분해 표시한다.
- [x] Result 진입 전에 현재 Run의 제출 ID를 자동으로 제출하도록 수정했다. 다른 Pending 복구와 겹쳐도 이번 제출을 기다린다. 별도 로딩 UI를 먼저 켜고, 남은 대기시간을 실제 시간 기준으로 `8s → 7s → …` 갱신한다. 제출·개인 최고 조회가 끝나면 즉시 Result를 연다. 전체 대기는 **8초**로 제한하며, 시간이 지나면 확인된 상태로 Result를 열고 늦은 응답은 도착 시 반영한다. Offline·미동의 상태는 온라인 응답을 기다리지 않는다. Unity에서 재검증해야 한다.
- [x] 사용자가 실제 플레이에서 `8s` 문구가 보이는 로딩 UI를 확인했다. 서버 응답이 빨라 초 감소는 관찰되지 않았으며 이는 정상이다. 새 Pending 재전송 Button의 Scene 연결은 아직 필요하다. AI는 Scene을 수정하지 않는다.
- [x] Result의 제출 Retry는 현재 Run의 Pending ID만 다시 시도한다. Submitted/Rejected는 Pending에서 제거한다. Rejected 사유는 현재 실행 중 Result에만 표시하고 재시도하지 않는다. Offline/Timeout 등은 Pending으로 남는다.
- [x] 사용자가 Pending 재전송 버튼 변경까지 Unity 컴파일, Edit Mode **728/728**, Play Mode **233/233** 성공과 예상 밖 Error/Warning 없음을 확인했다. 이후 추가한 Mode 강조 코드·Test는 다시 검증해야 한다.
- [x] 사용자가 Stage `Online best: #1 16`, Infinite `Online best: #1 1234`로 원시값이 표시된 것을 보고했다. Leaderboard와 같은 표시 규칙(Stage `0.016 s`, Infinite `1,234`)을 사용하도록 수정했다. Offline Stage Result에서 Infinite Board가 열린 문제는 실제 종료한 Run의 Mode로 조회하도록 수정했다.
- [x] 사용자가 `Retry Pending (N)`의 Scene 연결, Pending 0건 숨김, Pending 존재 시 표시·전송, 완료 후 숨김과 Navigation 갱신을 확인했다. 재전송 중 버튼 비활성·Navigation 우회는 Play Mode Test의 코드 판정 대상으로 두고 사람의 클릭 타이밍 테스트를 요구하지 않는다.
- [x] Stage/Infinite Button의 현재 조회 Mode 강조를 포커스와 독립적인 금색 배경·짙은 글자·Outline으로 추가했다. 사용자가 실제 화면에서 강조가 적절함을 확인했고, 이후 최신 코드로 컴파일 성공 및 Edit Mode 729/729·Play Mode 234/234 성공을 확인했다.
- [x] 실제 서비스 요청·Anonymous 인증·게임 플레이·네트워크 변경은 수행하지 않았다. 이들은 아래 사용자 검증과 Step 7 완료 근거다.

### 완료를 위한 빠른 확인표

아래 표의 **필수** 항목이 모두 통과하면 Step 7을 완료할 수 있다. 화면에서 보이는 결과만 확인하면 되며, 요청 JSON·전체 Player ID·저장 파일은 확인하거나 전달할 필요가 없다. 자세한 조작 순서는 바로 아래 `사용자 수동 작업`에 있다.

| 번호 | 현재 | 확인할 화면·행동 | 통과 기준 |
| --- | --- | --- | --- |
| 1. 준비 | 컴파일·Edit Mode 729/729·Play Mode 234/234 통과 | 완료 | 참조 누락·예상 밖 Error/Warning·Test 실패가 없다. |
| 2. Leaderboard | Pending 0건/1건 이상 표시·전송·Navigation 및 Mode 강조 확인됨 | 완료 | 현재 조회 중인 Mode Button의 금색 배경·짙은 글자·Outline이 유지된다. 재전송 중 중복 클릭 방지는 자동 Test가 판정한다. |
| 3. 본인 행 | `<마스킹된 ID> (You)` 확인됨 | 기존 결과 인정 | 같은 형식을 유지한다. |
| 4. 온라인 Stage·Infinite | 완료 | 온라인으로 각각 한 번 종료 | 10초 전에 Result로 전환되고, Stage는 `#1 0.016 s`, Infinite는 `#1 1,234`처럼 Mode 단위로 표시된다. 새 기록값이면 순위·값은 실제 조회 결과에 맞게 달라질 수 있다. |
| 5. Result 조작 | 완료 | Offline Stage Result에서 `View Leaderboard`와 `Back` 확인 | Stage 1 Board가 열리고 Back은 같은 Result로 돌아온다. Pending에서만 `Retry Submission`이 보인다. |
| 6. Offline 복구 | 완료 | 연결을 복구하고 새 플레이 없이 Leaderboard의 `Retry Pending (N)` 사용 | Button의 N과 `Last retry: S submitted, R rejected, P pending`이 갱신된다. S/R은 이번 수동 Retry의 확정 응답 건수다. 재접속 자동 재시도가 먼저 완료됐으면 N=0도 정상이다. 계속 Pending이면 미완료다. |
| 7. Settings·Console | 안내 왕복과 Console 무메시지 확인됨 | 기존 결과 인정; 새 코드 실행 중 예상 밖 메시지만 추가 확인 | Settings로 돌아오고 예상 밖 Error/Warning이 없다. |

동의 안내가 안 뜨는 기존 동의 계정은 정상이다. 서버가 매우 빨라 로딩 숫자가 바뀌기 전에 Result가 열리면 숫자 변화는 `빠른 응답`으로 적고 실패로 보지 않는다. `Submitted`인데 `Online best: Unavailable`이 계속 나오거나 로딩이 10초 이상 지속되면 완료로 판단하지 말고 해당 문구와 Console 요약을 전달한다.

### 사용자 수동 작업

#### Scene: Result 제출 로딩 UI 추가 (사용자 수행)

현재 Scene에는 `UIRoot/OnlineRecoveryNoticeCanvas`가 있고 이 Canvas는 다른 화면보다 위에 그려진다. 기존 `OnlineRecoveryNoticePanel`은 그대로 둔다. 아래 새 오브젝트만 같은 Canvas의 **마지막 자식**으로 추가한다. `ResultPanel` 아래에 만들면 대기 중 ResultPanel과 함께 숨겨지므로 그곳에는 만들지 않는다.

1. `UIRoot/OnlineRecoveryNoticeCanvas/ResultSubmissionLoadingPanel`을 UI Image로 만든다. RectTransform은 부모 전체 Stretch(Anchor Min `0,0`, Max `1,1`, Left/Right/Top/Bottom `0`, Pivot `0.5,0.5`), Image 색 `RGBA(0,0,0,0.87)`, Raycast Target `On`이다. Scene에서는 GameObject를 **비활성화**해 둔다.
2. 그 자식 `LoadingWindow`를 UI Image로 만든다. 중앙 Anchor/Pivot `0.5,0.5`, Anchored Position `(0,0)`, Size `560×220`, Image 색 `RGBA(0.067,0.094,0.153,1)`, Raycast Target `Off`이다.
3. `LoadingWindow/LoadingText`를 TextMeshPro - Text (UI)로 만든다. 중앙 Anchor/Pivot `0.5,0.5`, Anchored Position `(0,0)`, Size `520×160`, Horizontal/Vertical Alignment `Center/Middle`, Font Size `30`, Color White, Raycast Target `Off`로 한다. Inspector의 초기 문구는 `Submitting record...` 다음 줄 `Result in 8s or sooner`로 지정한다. 실행 중 코드는 이 문구의 초 숫자를 실제 시간 기준으로 갱신한다. Button이나 OnClick은 추가하지 않는다.
4. `UIManagementSystem` 컴포넌트의 `_resultSubmissionLoadingPanel`에는 `ResultSubmissionLoadingPanel` GameObject를, `_resultSubmissionLoadingText`에는 `LoadingText`의 TMP_Text 컴포넌트를 연결하고 Scene을 저장한다. 대기 중 켜지고 Result가 열릴 때 꺼지는지는 아래 Play Mode에서 확인한다.

#### Scene: 저장된 Pending 재전송 Button 추가 (사용자 수행)

기존 순위 조회 `Retry`는 그대로 둔다. 새 Button은 `UIRoot/MenuCanvas/LeaderboardPanel/MenuList`의 직접 자식이며, Result를 떠났거나 앱을 다시 실행한 뒤에도 Local Save에 남은 Pending을 보내는 용도다. 이 작업은 AI가 Scene에 직접 적용하지 않는다.

1. `MenuList`의 RectTransform Size를 `860×860`으로 바꾼다. 기존 `LeaderboardRetryButton`은 `(0,-612)`, `360×48` 그대로 둔다. 기존 `BackButton`만 `(0,-732)`, `360×48`로 아래로 옮긴다. `VerticalLayoutGroup`은 계속 Disabled로 둔다.
2. 기존 `LeaderboardRetryButton`을 Duplicate해 이름을 `LeaderboardPendingRetryButton`으로 바꾼다. RectTransform은 top-center Anchor `(0.5,1)`, Pivot `(0.5,1)`, Anchored Position `(0,-672)`, Size `360×48`이다. 자식 TMP의 이름은 `LeaderboardPendingRetryButtonText`, 초기 문구는 `Retry Pending (0)`으로 한다.
3. 새 Button의 OnClick을 **한 개만** `GameSystem.SelectLeaderboardPendingRetry`로 바꾼다. 복제된 `SelectLeaderboardRetry` OnClick이 남지 않게 한다. `UIManagementSystem._leaderboardPendingRetryButton`에는 새 Button, `_leaderboardPendingRetryText`에는 자식 TMP를 연결한다.
4. `MenuList`의 직접 자식으로 TextMeshPro - Text (UI)를 하나 더 만들고 이름을 `LeaderboardPendingRetryStatusText`로 한다. top-center Anchor `(0.5,1)`, Pivot `(0.5,1)`, Anchored Position `(0,-790)`, Size `780×32`, Font Size `20`, Center/Middle, Raycast Target `Off`, 초기 문구 `No manual retry yet`로 한다. `UIManagementSystem._leaderboardPendingRetryStatusText`에 연결한다.
5. Button Navigation을 Explicit으로 설정한다. 기존 조회 Retry는 Up `LeaderboardInfiniteButton`, Down 새 Pending Retry; 새 Pending Retry는 Up 조회 Retry, Down `BackButton`; Back은 Up 새 Pending Retry다. 이 연결은 Pending이 있을 때의 초기 Scene 설정이다. Runtime은 Pending 0건일 때 새 Button을 숨기고 조회 Retry의 Down을 Back, Back의 Up을 조회 Retry로 바꾼다. Pending이 생기면 위 초기 연결로 복원한다. Mouse Click과 Keyboard Navigate/Submit 양쪽에서 선택되는지 확인한다.

#### 0. 시작 전

1. 새 Pending Button의 Scene 연결 후 Unity Editor에서 컴파일하고, 예상하지 못한 Error/Warning이 없는지 확인한다. Edit Mode `ResultTextFormatterTests`, `LeaderboardViewStateTests`, `GameNavigationStateTests`, `OnlineRecordRepositoryTests`와 Play Mode `ModeUISceneConfigurationTests.Phase4LeaderboardAndResultControls_AreWired`를 먼저 확인한 뒤, 변경 후 전체 Edit/Play Mode Test도 확인한다. AI는 빌드·Test Runner를 실행하지 않는다.
2. **verification** 환경에서 기존 게임 Scene을 Play Mode로 실행한다. Scene이나 서버 설정을 이 절차 중에 바꾸지 않는다. 새 테스트 계정에 Protected ledger가 없다면 `UGS/VERIFICATION_DEPLOYMENT.md`의 최초 준비 절차를 먼저 수행한다. 기존 Board·기록·ledger를 삭제하지 않는다.
3. Console을 확인할 수 있게 둔다. 전체 Player ID, Access Token, 요청 JSON, Local Save 내용은 채팅에 전달하지 않는다.

#### 1. Leaderboard 조회

1. Main Menu에서 `Leaderboard`를 누른다. 이미 동의한 계정이면 안내 없이 열리는 것이 정상이다. 새 계정에서 안내가 뜨면 `Cancel`로 돌아간 뒤 다시 열어 `Continue`로 동의한다.
2. 처음 화면의 `Stage 1`에서 `TOP`과 `AROUND YOU`가 각각 목록 또는 상태 메시지로 바뀌는지 본다. `Loading...`이 계속 남으면 문제로 기록한다.
3. `Infinite`를 누르고 두 목록의 내용이 새 Mode에 맞게 바뀌는지 본다. Infinite Button의 금색 배경·짙은 글자·Outline이 `Retry`나 `Back`으로 포커스를 옮겨도 유지되는지 확인한다. `Stage 1`로 돌아오면 강조도 Stage Button으로 옮겨야 한다. 이어서 `Retry`를 눌러 두 목록이 다시 조회되는지 본다. 여기의 `Retry`는 **순위 조회** 버튼이다. 별도 `Retry Pending (N)`은 저장된 제출 후보를 서버에 다시 보내는 Button이다.
4. 본인 기록 행이 보이면 `<마스킹된 Player ID> (You)` 형식인지 본다. Main Menu에는 별도의 `내 최고 기록` 화면이 없으므로 찾을 필요가 없다.
5. Stage·Infinite 각각에서 TOP/AROUND의 최종 문구만 기록한다. 목록, `No records.`, `Offline. Retry when connected.`, `Error: <사유>` 중 무엇인지 적으면 된다.

#### 2. 온라인 플레이 결과

1. 네트워크가 연결된 상태에서 Stage를 정상 clear한다. 플레이 종료 직후 `Submitting record...` 로딩 화면이 나타나고 Result는 아직 보이지 않아야 한다. 서버 응답이 늦으면 `Result in 8s or sooner`의 숫자가 `7s`, `6s`처럼 실제로 감소하는지 본다. 게임의 Time Scale이 0이어도 실제 시간 기준으로 감소한다. 제출·개인 최고 조회가 끝나면 즉시 Result로 바뀐다. **10초 이상 로딩 화면이 유지되면 실패**로 기록한다. 코드는 8초가 지나면 현재 상태로 Result를 열고, 늦은 응답은 나중에 반영한다.
2. Result가 나타나면 `Local best`, `Submission`, `Online best` 세 줄을 그대로 기록한다. 자동 제출이 성공하면 **수동 Retry를 누르지 않아도** `Submission: Submitted`가 보여야 한다. 서버가 거절하면 `Rejected`와 사유, 일시 실패·Timeout이면 `Pending (retry available)`이 남는다.
3. Infinite도 정상 플레이·종료해 로딩 문구의 초 변화 → Result 전환과 같은 세 줄을 기록한다. 서버가 빨리 응답해 숫자가 감소하기 전에 Result가 열리는 것은 정상이다. 점수나 저장 파일을 임의 수정하지 않는다.
4. `Pending`일 때만 `Retry Submission` 버튼이 보여야 한다. `Submitted`·`Rejected`에는 버튼이 없어야 한다. `Rejected`는 재시도하지 않는다. `Online best`는 **이번 Run의 제출 결과가 아니라 서버에 저장된 개인 최고**다. 서버에 기록이 없거나 조회 실패이면 `Unavailable`일 수 있다.
5. Result에서 `View Leaderboard`를 열고 `Back`으로 돌아와 Result가 다시 보이는지 확인한다.

#### 3. Offline Pending과 복구

1. Play Mode를 종료하지 않은 채 네트워크 연결을 끊고 정상 플레이 결과 하나를 만든다. Unity가 `NotReachable`로 인식하면 서버를 기다리지 않고 Result가 열린다. 그렇지 않으면 연결 시도·Timeout 후 열릴 수 있다. `Submission: Pending (retry available)`과 `Retry Submission` 버튼을 확인한다.
2. 네트워크가 끊긴 상태에서 `Retry Submission`을 한 번 누른다. 상태가 Pending으로 유지되고 화면을 사용할 수 있는지 본다.
3. 네트워크를 다시 연결한다. **새 플레이를 시작하지 말고** Main Menu → Leaderboard로 이동한다. Pending이 아직 1건 이상이면 `Retry Pending (N)`이 보이며 Keyboard Down 이동이 조회 Retry → Pending Retry → Back 순서인지 확인한다. Button을 한 번 눌러 처리 후 N과 상태 Text의 `submitted`, `rejected`, `pending` 건수를 기록한다. 재전송 중의 짧은 비활성 시간과 중복 클릭 방지는 Play Mode Test가 검사하므로 사람이 확인할 필요가 없다. 네트워크 복구 자동 재시도가 먼저 처리해 버튼이 처음부터 보이지 않으면 N=0으로 정상 기록한다. 계속 Pending이면 상태 Text와 Console 요약을 기록한다.
4. 앞서 Offline Stage Result에서 `View Leaderboard`를 눌렀을 때 Infinite가 열렸던 경로를 다시 확인한다. 이제 Stage 1 Board가 열려야 한다. Offline일 때 TOP/AROUND에 `Offline. Retry when connected.`가 나오는 것은 정상이다.

#### 4. Settings 안내 재확인

1. Settings를 열고 Mouse로 `Online Record Notice`를 누른다. 안내가 열리면 `Cancel`을 눌러 Settings로 돌아오는지 본다.
2. 이 Button은 현재 Mouse 전용이다. Keyboard로 선택되지 않는 것은 이 Step의 실패가 아니다.

#### 이전 확인 결과 (코드 수정 전, 2026-09-30)

- 기존 동의 계정에서는 Notice 없이 Leaderboard에 진입했다. Stage 목록 조회, Infinite 전환, 조회 Retry가 동작했다.
- Stage는 처음 Pending, 수동 제출 Retry 후 Submitted였다. Infinite도 처음 Pending이었다. 두 Result의 Online best는 Unavailable이었다.
- 원인은 Result 진입 시 자동 제출·개인 최고 조회가 없었던 것이다. 이번 수정의 재검증 결과로 대체해야 하며, 과거 결과를 Step 7 완료 근거로 사용하지 않는다.
- 본인 행을 `<마스킹된 Player ID> (You)`로 표시하도록 수정했다. Unity에서 아직 재확인하지 않았다.

#### 추가 사용자 확인 결과 (표시·Mode·Pending Button 수정 전, 2026-09-30)

- Unity Script Compilation 성공, 예상 밖 Error/Warning 없음, Edit Mode 721/721 및 Play Mode 233/233 성공을 확인했다.
- Stage·Infinite TOP/AROUND에 본인 `<마스킹된 ID> (You)` 행이 보였고, Infinite 전환과 조회 Retry가 동작했다.
- Stage·Infinite 온라인 Run 모두 짧은 `8s` 로딩 후 수동 조작 없이 Submitted였다. Stage Online best는 `#1 16`, Infinite는 `#1 1234`로 원시값이어서 표시 수정이 필요했다.
- 온라인 Result의 `View Leaderboard`는 각 Mode로 열렸다. Offline Stage Result에서는 잘못 Infinite Board가 열렸다. Offline Stage·Infinite Run은 Pending, Online best는 Unavailable였고 TOP/AROUND는 Offline을 표시했다.
- Settings 안내의 Continue/Cancel 왕복 중 Console에는 메시지가 없었다. Offline Pending을 새 플레이 없이 재전송할 Button은 당시 없었다. 이번 코드·Scene 변경의 재검증 결과로 대체해야 한다.

#### 중단·기록 규칙

- 예상하지 않은 Console Error/Warning, 인증 실패, `VerificationBoardCapacity`, ledger 관련 오류, UI가 멈춤, 예상과 다른 Project/Environment가 나타나면 이후 단계를 진행하지 말고 해당 단계·표시 safe reason·Console 요약만 기록한다.
- 실제 데이터가 없어 `No records.`가 나오는 것은 조회 성공의 한 결과다. Board/기록을 삭제해 목록을 만들지 않는다.
- 네트워크를 끈 테스트에서는 같은 후보를 여러 번 수동 Retry하지 않는다. 자동 재시도와 수동 Retry의 경합은 Step 6 자동 Test가 판정했다.

결과는 아래 형식으로 요약한다. 전체 Player ID, token, Cloud Code 요청 전문, Local Save 전문은 전달하지 않는다.

```text
Step 7 / verification
1. Scene/컴파일/Test: <통과|문제 + 내용>
2. Leaderboard: Stage/Infinite 조회 Retry=<통과|문제>, Retry Pending (N)=<보인 숫자>, Mouse/Keyboard=<통과|문제>
3. 본인 행: <마스킹된 ID (You)|아직 없음|문제> (전체 ID는 보내지 않음)
4. Stage Result: 로딩=<표시/초 감소/빠른 응답 중 해당 사항>, 10초 이내=<예|아니오>, Local best=<화면 문구>, Submission=<화면 문구>, Online best=<화면 문구>
   Infinite Result: 로딩=<...>, 10초 이내=<예|아니오>, Local best=<...>, Submission=<...>, Online best=<...>
5. Result 버튼/왕복: <통과|문제>
6. Offline→복구: Pending=<예|아니오>, 새 플레이 없이 Retry Pending=<시도|자동 복구가 먼저 N=0>, 처리 후 N=<숫자>, 마지막 수동 Retry 상태 Text=<문구>
7. Settings/Console: <통과|문제 + 예상 밖 Error/Warning 요약>
```

### 완료 조건

- [x] 위 빠른 확인표 1~7이 모두 통과했다. 기존 동의 계정의 안내 생략, 빠른 응답에 따른 숫자 변화 미관찰, 발생하지 않은 Rejected는 예외적으로 실패가 아니다. 문제를 수정했다면 영향받는 항목을 다시 확인했다.

## Step 8. 화면 가독성과 입력 경험을 수동 확인한다

### AI 정적 확인 결과

- [x] `SampleScene.unity`의 모든 `CanvasScaler`는 `Scale With Screen Size`, 기준 해상도 `1920 x 1080`이다. 이는 대상 해상도에서의 배치 기준 확인이며 실제 글자 가독성 판단을 대체하지 않는다.
- [x] Scene에는 EventSystem 하나와 활성화된 Navigation Event가 있다. UI Action에는 Keyboard+Mouse의 `W/A/S/D` Navigate, Submit, `Escape` Cancel, Mouse Point와 Left Click이 연결돼 있다.
- [x] Leaderboard의 Stage/Infinite, 조회 Retry, 조건부 `Retry Pending (N)`, Back은 선택 상태에 대응하는 Button으로 동기화된다. Pending 재전송 중 버튼 비활성 및 Navigation 우회는 자동 Test가 판정한다. Gamepad는 이번 검증 대상이 아니다.
- [x] Loading·Empty·Offline·Error·Pending·Submitted·Rejected 분기는 코드와 기존 자동 Test로 확인했다. 화면 확인을 위해 서비스 장애를 만들거나 서버 기록을 삭제하지 않는다.
- [x] AI는 Scene, Unity Editor Build, Unity Test Runner를 실행하지 않았다.

### 사용자 수동 작업

**전제:** Unity Editor의 Game View를 `1920 x 1080`으로 맞춘다. 이 Step은 화면·입력 확인만 수행한다. 새 Build, 서버 데이터 삭제, 강제 Error/Empty 생성은 하지 않는다.

1. **Main Menu와 Settings**: Mouse로 `Leaderboard` → `Settings` → `Online Record Notice` → `Back`을 누른다. Keyboard는 W/S로 항목 이동, Submit으로 열기, Esc로 돌아오기를 확인한다. 글자 잘림·행 겹침·화면 밖 Button·보이지 않는 선택 표시가 없어야 한다. Settings의 Notice Button은 Mouse 전용이므로 Keyboard로 선택되지 않아도 정상이다.
2. **Leaderboard**: Mouse로 Stage 1 → Infinite → Stage 1을 누른다. 현재 조회 Mode의 금색 배경·짙은 글자·Outline은 포커스가 다른 Button에 있어도 유지돼야 한다. Keyboard W/S와 Submit으로 Mode, 조회 `Retry`, `Back`을 사용한다. `Retry Pending (N)`이 없으면 Retry의 Down은 Back으로, 있으면 Pending Retry를 거쳐 Back으로 이동해야 한다. TOP·AROUND YOU·계정·상태 문구가 겹치거나 잘리지 않아야 한다.
3. **Result와 Loading**: Stage 또는 Infinite를 한 번 끝내 `Submitting record...` Loading과 Result를 확인한다. 빠른 응답이면 8초 숫자가 감소하기 전에 Result로 전환돼도 정상이다. Result의 `Local best`, `Submission`, `Online best`, `View Leaderboard`, `Main Menu`가 읽혀야 한다. Pending일 때만 `Retry Submission`이 보이고 Keyboard 선택·Submit이 가능해야 한다. `View Leaderboard` 후 Esc 또는 Back으로 Result로 돌아온다.
4. **보고 기준**: 모두 문제없으면 `Step 8 통과`, 확인 해상도, Keyboard/Mouse 통과 여부만 보고한다. 문제면 화면 이름, 입력 방식, 조작 순서, 실제 결과를 보고한다. 스크린샷에는 전체 Player ID·token 등 비밀값을 가린다.

### AI 후속 작업

표현 코드 문제는 수정하고 Scene 수정이 필요한 경우 정확한 적용 방법을 제공한다. 상태 분기와 빠른 입력 경합은 기존 자동 Test로 확인했으므로, 사용자는 위 화면·입력 결과만 확인한다.

### 완료 조건

- [x] 사용자가 1920×1080 Game View에서 각 창의 배치와 화면 가독성이 적절함을 확인했고, Keyboard와 Mouse 입력도 확인했다. 차단 문제는 보고되지 않았다.

## Step 9. 대상 플랫폼을 Build하고 Player에서 검증한다

### AI 준비

Build에 들어갈 Scene 목록, 플랫폼·아키텍처·구성 및 verification 선택을 정적으로 대조하고 실제 설정값과 출력 위치를 안내한다. Editor 전용 코드의 Runtime 의존과 테스트/비밀값의 포함 여부를 검사한다. 최종 후보에 들어간 변경으로 Step 6~8을 통과했는지 확인한다.

### AI 정적 확인 결과

- [x] `EditorBuildSettings.asset`의 활성 Build Scene은 `Assets/Scenes/SampleScene.unity` 하나다.
- [x] Player Runtime은 `PersistentLocalSaveFileStore`를 사용한다. Play Mode 격리용 메모리 저장소와 온라인 차단은 `UNITY_EDITOR` 조건부이므로 Player에는 포함되지 않는다.
- [x] `UnityEditor` 참조는 Editor 전용 코드 또는 `UNITY_EDITOR` 조건부에 한정된다. `DEVELOPMENT_BUILD`일 때만 Infinite 난이도 Text가 표시될 수 있으므로 최종 후보 검증에는 Development Build를 사용하지 않는다.
- [x] 현재 `ProjectSettings.asset`의 기본 창 설정은 `1024 x 768`, `Fullscreen Window`다. Step 1의 `1920 x 1080`, `Windowed` 기준과 다르므로 아래 사용자 설정 변경이 필요하다.
- [x] Step 6의 최신 결과는 Edit Mode 729/729, Play Mode 234/234 성공이다. AI는 Unity Build와 Unity Test Runner를 실행하지 않았다.

### 사용자 수동 작업

1. **Unity 설정 변경**: `Edit > Project Settings > Player > Resolution and Presentation`에서 Default Screen Width `1920`, Height `1080`, Fullscreen Mode `Windowed`를 설정한다. 이는 Scene 작업이 아니다. 기존 Player가 저장한 전체 화면 설정이 있으면, Player 실행 후 Settings의 Fullscreen도 끄고 정상 종료해 저장한다.
2. **Build Profile**: `File > Build Profiles`에서 `Windows`와 `x86_64`를 선택한다. 활성 Scene 목록에는 `SampleScene` 하나만 있어야 한다. Development Build를 끄고, 기존 배포물을 덮어쓰지 않는 새 출력 폴더를 지정한다.
3. **Build**: 사용자가 Unity Editor에서 Build를 실행한다. 성공/실패, 출력 경로, 예상 밖 Error/Warning 유무만 기록한다. 실패하면 Build log의 요약만 전달하며 token·전체 Player ID·Local Save 내용은 공유하지 않는다.
4. **Player 기본 동작**: 생성된 exe를 실행한다. 창이 `1920 x 1080 Windowed`인지 확인하고, Editor 전용 검증 창 없이 Main Menu → Leaderboard → Stage/Infinite Run → Result → Main Menu 및 Settings를 사용한다. Keyboard·Mouse와 화면 가독성은 Step 8과 같은 기준으로 확인한다.
5. **verification 연결**: Player에서 기존 동의 계정으로 Leaderboard의 Stage/Infinite 조회, 각 Mode의 자동 제출 Result, Offline 조회 및 복귀를 확인한다. 이미 있는 서버 Board·ledger·기록을 삭제·초기화하지 않는다.
6. **재시작 Pending 복구**: 네트워크를 끈 상태에서 Run 하나를 종료해 Pending을 만든다. Player를 **정상 종료**하고, 네트워크를 계속 끈 채 다시 실행한다. Leaderboard에서 `Retry Pending (N)`이 남아 있는지 확인한 뒤 네트워크를 복구하고 새 플레이 없이 재전송한다. 처리 후 Pending이 0건이면 Button이 사라져야 한다. 재접속 자동 재시도가 먼저 처리해 N=0이 되는 경우도 정상으로 기록한다.
7. **재시작 설정 보존**: Fullscreen 설정과 변경한 Keyboard binding이 있다면 Player를 정상 종료·재실행한 뒤 유지되는지 확인한다. 성능·응답 시간 측정은 하지 않는다.

### 완료 조건

- [x] 사용자가 Windows x64·1920×1080 Windowed·비 Development Build 구성의 Build 성공과 Player 확인 사항의 성공을 보고했다. 실제 서비스·재시작 Pending 복구·화면·Keyboard/Mouse 검증이 통과했고, 성능·응답 시간 측정은 제외했다.

## Step 10. Phase 4와 Prototype 7의 완료 근거를 정리한다

### AI 작업

1. Roadmap의 여섯 완료 조건을 최신 정적 검사·전체 Unity Test·생산 UI 서비스 결과·Player Build 결과와 대응시킨다.
2. 기존 Phase 3 결과와 Phase 4 변경 후 결과를 구분하고, 수정 후 재검증이 누락된 범위가 없는지 확인한다. 남은 실패·미확인 필수 사례가 있으면 완료 처리하지 않는다.
3. Step 1에서 정한 운영 제한 처리와 출시 후보 범위를 대조한다. 문서에 제한을 적는 것만으로 공개 운영 준비를 완료 처리하지 않는다.
4. 조건 충족 시 Task·Roadmap·Project 상태를 갱신하고 미승인 운영 배포·추가 플랫폼·추가 기능은 별도 후속 작업으로 남긴다.

### 사용자 수동 작업

없음. 근거가 누락된 경우에만 해당 Step의 구체적인 실행 항목을 요청한다.

### 완료 근거 대조

| Roadmap Phase 4 완료 조건 | 완료 근거 |
| --- | --- |
| Main Menu·Result에서 기록·순위 확인 | Step 7 verification에서 Stage/Infinite 조회, `<마스킹된 ID> (You)`, Result Online best 및 View Leaderboard 왕복을 확인했다. |
| Loading·Empty·Offline·Error 구분 | 상태 분기는 자동 Test로, Loading·Offline 표시는 verification과 Step 8 화면에서 확인했다. Empty·Error를 만들기 위해 서버 데이터나 서비스를 변경하지 않았다. |
| Offline 플레이와 결과 보존 | Step 7 및 Step 9에서 Offline Pending과 Player 재시작 뒤 복구를 확인했다. |
| 연결 복구 후 대기 기록 1회 제출 | `Retry Pending (N)`의 처리·숨김과 중복 입력 차단을 확인했다. 재접속 자동 처리로 N=0이 될 수 있는 계약도 확인했다. |
| 정렬·내 순위·최고 기록 정책 | 정책 Test, Cloud Code 대역 Test, verification Leaderboard와 Mode별 Result 표시를 확인했다. |
| 전체 자동 Test·서비스·Player Build | 사용자 확인: Unity Compile 성공, Edit Mode 729/729, Play Mode 234/234, verification 서비스 확인, Windows x64·1920×1080 Windowed 비 Development Build 성공. |

### Phase 4 한계와 다음 Prototype 범위

- 이번 완료는 **verification 후보**의 완료이며 공개 운영 승인이나 Production 배포가 아니다.
- 성능·응답 시간 측정과 Gamepad 검증은 합의한 검증 제외 범위다.
- Production 환경·Board·보안 운영 절차, 서버 발급 십진 Public Player Number, ledger·용량 제한 해소와 대규모 순위 운영 검증은 다음 Prototype에서 수행한다.

### 완료 조건

- [x] Phase 4의 완료 근거와 한계를 기록했고, Roadmap 및 Project Memory의 상태를 완료로 갱신했다. 추가 사용자 수동 작업은 없다.

# 영향 범위

2026-09-29 확정 사항 반영은 Task·Feature·System·Roadmap·Project Memory 문서 변경이다. 후속 Step 실행 시 Runtime UI·상태 모델·Cloud Code·테스트가 변경되고, 사용자가 Scene 설정과 필요한 서비스 적용을 수행한다.

# 검증 내용

| 판정 대상 | 정적 검사 / AI 책임 | 자동 Test / AI 작성·사용자 실행 | 필요한 수동 확인 |
| --- | --- | --- | --- |
| Board·Stage·Version·환경 | 매핑·요청·정책 대조 | 잘못된 Key/Version 거부 | verification 실제 응답 |
| 시간·점수·순위·새 최고 | 기존 단위·순위 계약 대조 | 경계값·동점·로컬/서버 최고 구분 | 표시 가독성 |
| UI 결과 상태 | 상태표·화면 연결 대조 | Loading→성공/Empty/Offline/Error, Retry, 미제출/Rejected | 화면 표현 |
| 비동기 수명·입력 | 구독/해제·응답 식별·중복 요청 경로 | 역순 응답·닫은 화면·Tab 전환·재진입·연속 입력·취소 | 일반 조작감 |
| 동의·계정 귀속 | 저장 전 요청 차단·SDK 경계 | 동의/취소·저장 실패·다른 계정·인증 실패 | 실제 안내·원래 계정 연결 |
| Retry·재실행 | Pending 저장·복원·확정 응답 뒤 삭제 | 최대 3회·백오프·Timeout·동일 ID·동시 트리거·Rejected 미재시도·삭제 저장 실패 보존 | 실제 Offline 복구·Player 저장 수명 |
| Scene·게임 회귀 | 참조·Button 이벤트·초기 활성 상태 | 생산 Scene 구성, Menu/Run/Result/Settings, 입력 복귀 | 사용자 Scene 연결 |
| 출시 후보 | Build 설정·Editor 의존·비밀값 검사 | 전체 Edit/Play Mode | Windows x64 Build·Player·1920×1080 화면·입력 (성능·응답 측정 제외) |

서비스 대역 테스트는 실제 Access Control 강제력의 대체 근거가 아니다. 반대로 실제 서비스 한 번의 성공만으로 수치·비동기 경합·모든 실패 분기를 검증했다고 판정하지 않는다.

# 검증 결과

Roadmap Phase 4의 구현 대상과 여섯 완료 조건을 Step 1 사용자 결정, 최신 자동 Test, verification 서비스 확인 및 Windows Player Build 결과에 대응했다. Phase 4는 verification 후보 범위에서 완료됐으며, 공개 운영 전환과 성능·응답 시간 측정을 완료 근거로 주장하지 않는다.

# 후속 작업

다음 Prototype에서 공개 운영 환경과 배포·롤백 절차, 서버 발급 십진 Public Player Number, ledger·용량 제한 해소 및 대규모 순위 운영 검증을 별도 범위로 계획한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_7/20260924_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_7/20260926_01_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_7/20260927_01_Phase3ManualSteps.md`

# 작성 완료 기준

- [x] 사용자 수동 조작은 결정·Scene 연결·Unity 실행·실서비스·화면/Player 검증으로 특정했다.
- [x] 정적 검사와 Edit/Play Mode Test를 각 단계의 선행 조건 및 완료 근거로 배치했다.
- [x] 실제 연결 명세·fixture 이름은 구현 후 제공하며 존재하지 않는 대상을 확정하지 않았다.
- [x] 계획 작성 완료와 Phase 4 수행 완료를 구분했다.
