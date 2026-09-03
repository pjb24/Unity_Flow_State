# 작업 정보

## 작업명

Prototype 2 Phase 4 Manual Steps

## 작업 일자

20260902

## 작업 담당자

AI, 사용자

## 작업 상태

진행 중

---

# 작업 목적

Prototype 2 Phase 4의 Mode별 UI와 전체 반복 플레이 흐름을 구현하고 검증하는 순서를 정의한다.

정적 검증과 Unit Test로 판정할 수 있는 항목은 자동화하고 Unity Editor에서만 가능한 Scene 구성, Build와 최종 화면 확인만 사용자 수동 작업으로 분리한다.

---

# 작업 범위

- Stage Mode와 InfiniteMode UI 구분
- InfiniteMode HUD
- 현재 이동 거리와 Score 표시
- InfiniteMode Result 표시
- PausePanel UI 마무리
- Keyboard와 Mouse UI 입력 회귀
- Stage Mode와 InfiniteMode 반복 플레이
- Compile, 전체 Test와 Build 검증

---

# 제외 범위

- Save와 Leaderboard
- 신규 게임 Mode
- Prototype 3 자동 이동과 Collectible
- Prototype 4 Map Pattern 추가
- 최종 상용 아트, 다국어, 해상도별 완성 레이아웃
- 현재 Phase 목표와 무관한 이동·물리 변경

---

# 현재 기준 상태

- Prototype 2 Phase 3가 완료되었다.
- Unity Script Compilation 성공과 예상하지 않은 Error·Warning 부재가 확인되었다.
- Edit Mode Test 기준은 `177 Passed, 0 Failed`이다.
- Play Mode Test 기준은 `87 Passed, 0 Failed`이다.
- `StageHUD`, 공용 `ResultPanel`과 최소 `PausePanel`이 생산 Scene에 존재한다.
- Stage Result는 Clear Time을 표시하지만 InfiniteMode의 현재 거리·Score HUD와 최종 결과 Text는 없다.
- ResultData에는 Stage Clear Time과 InfiniteMode Final Distance·Final Score가 이미 Mode별로 구분되어 있다.
- Keyboard와 Mouse의 ResultMenu 및 PauseMenu 입력 흐름은 Phase 3 Test로 검증되어 있다.

---

# 검증 원칙

- 실패하는 Test 또는 변경될 기대값을 생산 코드보다 먼저 작성한다.
- 문자열 형식, Mode별 표시 규칙과 UI 상태 매핑은 Edit Mode Unit Test를 우선한다.
- 실제 Scene 참조, HUD 갱신, Result 전환, Retry와 입력 통합은 Play Mode Test로 검증한다.
- 빠른 연속 조작, 동일·인접 프레임 입력, 입력 잔류와 중복 실행은 수동으로 요구하지 않고 Play Mode Test로 검증한다.
- Scene 변경 전에 필요한 계층, Component, 이름과 Serialized Field를 코드와 생산 Scene 구조 Test로 확정한다.
- 화면 배치와 가독성처럼 자동 판정이 부적절한 항목만 최소 수동 플레이로 확인한다.
- AI는 Unity Build와 Unity Test Runner를 실행하지 않는다.
- AI는 Scene을 수정하지 않고 정확한 사용자 작업 절차와 변경 후 정적 검증을 제공한다.

---

# Step 구성

## Step 1. Phase 4 UI 표시 규칙을 확정한다

- 진행 상태: **완료**

### 결정 항목

1. Infinite HUD의 거리와 Score 표시 형식을 결정한다.
2. Infinite Result의 최종 거리와 최종 Score 표시 형식을 결정한다.
3. 거리의 소수 자릿수와 반올림 규칙을 결정한다.
4. Stage와 InfiniteMode에서 활성화할 HUD 및 Result Content 조합을 결정한다.
5. PausePanel 마무리 범위와 Phase 4에서 요구할 최소 가독성 기준을 결정한다.
6. HUD 갱신 주기와 값이 없는 상태의 표시 규칙을 결정한다.
7. 지원할 화면 크기와 Build 대상의 최소 검증 범위를 결정한다.

### 확정 규칙

#### InfiniteMode HUD와 Result 표시

- InfiniteMode HUD는 `Distance: 12`, `Score: 123` 형식을 사용한다.
- InfiniteMode Result는 `Final Distance: 12`, `Final Score: 123` 형식을 사용한다.
- 거리는 소수점 없이 내림 처리하여 표시한다.
- 거리 내림은 UI 표시 단계에만 적용하고 Runtime Data와 Result Data의 원본 `float` 값은 변경하지 않는다.
- Score는 확정된 `int` 값을 다시 계산하지 않고 그대로 표시한다.
- 문자열은 `InvariantCulture`를 사용한다.
- Runtime Data가 없거나 초기화되지 않은 경우와 유효하지 않은 값은 `--`로 표시한다.

#### Mode와 상태별 UI 조합

| 게임 상태 | Stage Mode | InfiniteMode |
|---|---|---|
| Initializing, Ready | 모두 숨김 | 모두 숨김 |
| Playing | `StageHUD` | `InfiniteHUD` |
| Paused | `StageHUD`, `PausePanel` | `InfiniteHUD`, `PausePanel` |
| Ending | `StageHUD` | `InfiniteHUD` |
| Result, Ended | `StageHUD`, `ResultPanel`, `StageResultContent` | `InfiniteHUD`, `ResultPanel`, `InfiniteResultContent` |

- Pause, Ending, Result와 Ended에서도 현재 Mode의 HUD를 유지한다.
- PausePanel과 ResultPanel은 현재 Mode의 HUD보다 앞에 표시한다.
- StageResultContent와 InfiniteResultContent는 동시에 표시하지 않는다.
- Result의 Retry와 Quit Button은 두 Mode에서 공용으로 사용한다.
- 현재 Mode가 아닌 HUD는 표시하지 않는다.
- Ending에서 HUD를 숨기거나 초기화하지 않고 마지막 표시값을 유지한다.
- Result Data가 확정되기 전에는 ResultPanel을 표시하지 않는다.

#### PausePanel 최소 마무리 범위

- `Pause` 제목과 `Resume`, `Retry`, `Quit` Button을 식별 가능하게 표시한다.
- Button은 세로로 배치하고 서로 겹치거나 화면 밖으로 벗어나지 않게 한다.
- Keyboard와 Mouse의 현재 선택 상태를 명확하게 구분한다.
- Text와 배경 사이에 최소한의 가독성을 확보한다.
- 신규 상용 아트와 애니메이션은 추가하지 않는다.
- 기존 자산만으로 안전하게 구성할 수 있는 경우 반투명 배경을 사용할 수 있다.
- 반투명 배경은 Mouse 입력을 차단하지 않도록 Raycast 구성을 검증한다.

#### HUD 갱신과 초기 표시

- InfiniteMode Playing 동안 화면 프레임마다 Runtime Data의 표시 대상 값을 확인한다.
- 실제 표시값이 변경된 경우에만 문자열을 생성하고 Text에 할당한다.
- Pause, Ending, Result와 Ended에서는 HUD 갱신을 중단하고 마지막 표시값을 유지한다.
- Retry 시 새 Runtime Data를 기준으로 `Distance: 0`, `Score: 0`을 표시한다.
- Stage Mode에서는 InfiniteHUD 갱신 경로를 사용하지 않는다.
- ResultPanel은 HUD와 별도로 Result Data의 확정값을 표시한다.

#### 화면과 Build 최소 검증 범위

- Build 대상은 Windows Standalone 64-bit로 한다.
- Build Scene은 `SampleScene`을 사용한다.
- `1920 x 1080`, `1280 x 720`, `1024 x 768` 화면에서 최소 화면 검증을 수행한다.
- 각 화면에서 HUD, PausePanel과 ResultPanel의 식별 가능 여부, 겹침과 잘림을 확인한다.
- Build를 한 번 수행하고 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 정적 검증

- 기존 ResultData와 InfiniteModeRuntimeData 값을 다시 계산하지 않고 표시만 하는 규칙인지 확인한다.
- UIManagementSystem이 게임 규칙이나 Score 계산을 소유하지 않는지 확인한다.
- Phase 4 범위 밖 저장, Leaderboard와 신규 아트 시스템이 포함되지 않는지 확인한다.

### 수동 작업

- 사용자는 AI가 제시하는 규칙별 장단점을 검토하고 표시 형식 및 최소 화면 범위를 선택한다.
- Unity Editor 작업은 없다.

### 완료 조건

- [x] 모든 UI 표시 규칙이 확정되었다.
- [x] 관련 Feature와 System 문서에 규칙이 반영되었다.

## Step 2. 현재 UI와 Scene 확장 지점을 정적으로 조사한다

- 진행 상태: **완료**

### AI 작업

- UIManagementSystem의 State, Mode, Text와 Button 책임을 조사한다.
- StageHUD, ResultPanel, PausePanel, UIRoot와 EventSystem 계층을 조사한다.
- GameSystem의 Runtime Data 전달 시점과 Result Data 전달 시점을 조사한다.
- 기존 ResultMenu, PauseMenu와 Mode별 통합 Test의 회귀 범위를 정한다.
- 신규·수정 파일, Serialized Field와 사용자 Scene 작업 후보를 확정한다.

### 조사 결과

#### UIManagementSystem과 UI State

- `E_UIState`에는 `None`, `StageHud`, `Pause`, `Result`가 존재한다.
- 현재 `UIManagementSystem`은 `_stageHud`, `_resultPanel`, `_pausePanel` 중 현재 UI State에 해당하는 하나만 활성화한다.
- 현재 `UIManagementSystem`은 게임 Mode와 `E_GameState`를 입력받지 않는다.
- 현재 `UIManagementSystem.SetResultData()`는 `ResultData.ClearTime`만 `Clear Time: 12.345 s` 형식으로 표시한다.
- InfiniteMode HUD 갱신, InfiniteMode Result 표시와 Mode별 Result Content 전환 경로는 존재하지 않는다.
- PauseMenu와 ResultMenu는 서로 독립적인 선택 상태를 사용하며 기존 공용 Result Button과 Pause Button을 재사용할 수 있다.
- Step 1의 복합 UI 조합에는 Mode, 게임 상태, UI State와 Result Data 존재 여부를 함께 사용하는 표시 매핑이 필요하다.
- Unity GameObject와 분리된 순수 표시 매핑 상태를 추가하고 `UIManagementSystem`은 그 결과만 Scene Object에 반영하는 확장 지점으로 확정한다.

#### GameSystem과 데이터 전달 시점

- `GameSystem.StartGame()`은 선택 Mode로 `GameRuntimeData`를 생성한 뒤 `UIManagementSystem.Initialize()`를 호출한다.
- Ready 직후 UI State를 `StageHud`로 설정하고 Stage 시작 완료 후 게임 상태를 Playing으로 전환한다.
- Pause와 Resume은 UI State를 각각 `Pause`, `StageHud`로 설정한다.
- `HandleStageEnded()`는 Stage Mode Result Data를 생성한 뒤 `UIManagementSystem.SetResultData()`로 전달한다.
- InfiniteMode는 `CreateInfiniteResultData()`로 Result Data를 생성하지만 현재 UIManagementSystem에 전달하지 않는다.
- `EndGame()`은 게임 상태를 Ending으로 전환하고 UI State를 Result로 설정한 뒤 Play System과 Runtime Data를 정리하고 게임 상태를 Ended로 전환한다.
- Ending부터 Ended까지 한 번의 동기 호출로 진행되므로 GameSystem이 게임 상태 변경도 UIManagementSystem에 명시적으로 전달해야 확정된 상태별 표시 매핑을 적용할 수 있다.
- Runtime Data는 Ended 전환 전에 제거되므로 UIManagementSystem은 Playing 중 표시한 HUD 문자열을 보존하고 Pause, Ending, Result와 Ended에서 마지막 표시값을 유지해야 한다.
- Retry에서는 새 GameRuntimeData를 UIManagementSystem에 다시 전달하여 InfiniteMode HUD를 `Distance: 0`, `Score: 0`으로 초기화해야 한다.
- ResultSystem과 ResultData는 이미 Mode별 결과를 분리하므로 변경하지 않고 회귀 Test 대상으로 유지한다.

#### Formatter와 표시 데이터 경계

- 기존 `ResultTextFormatter`는 `InvariantCulture`와 Stage Clear Time 표시만 담당한다.
- InfiniteMode 현재 거리, 현재 Score, 최종 거리와 최종 Score 형식을 같은 순수 Formatter의 확장 지점으로 사용한다.
- 거리 내림은 Formatter의 표시 변환으로만 수행하고 `InfiniteModeRuntimeData`와 `ResultData`는 변경하지 않는다.
- UIManagementSystem은 Score 계산과 거리 누적을 수행하지 않고 전달받은 값을 Formatter에 전달한다.
- 같은 표시 문자열을 매 프레임 다시 할당하지 않도록 마지막 표시 문자열 또는 마지막 표시 정수값을 UIManagementSystem에서 비교한다.

#### 현재 생산 Scene UI 기준

현재 `SampleScene`의 확인된 UI 계층은 아래와 같다.

```text
UIRoot (GameObject 1051984166, Transform 1051984167)
├─ StageHUD (GameObject 714045101, Transform 714045102)
│  └─ Canvas (GameObject 1800455874, Canvas 1800455878)
├─ ResultPanel (GameObject 547905304, Transform 547905305)
│  └─ Canvas (GameObject 1421791936, Canvas 1421791940)
│     └─ Panel (GameObject 1895893077, Transform 1895893078)
│        ├─ Clear Time Text Image
│        │  └─ Clear Time Text
│        ├─ RetryButton
│        └─ QuitButton
└─ PausePanel (GameObject 895601724, Transform 895601725)
   └─ Canvas (GameObject 24339021, Canvas 24339025)
      └─ Panel (GameObject 1301848279, Transform 1301848280)
         ├─ Pause Title Text Image
         │  └─ Pause Title Text
         ├─ ResumeButton
         ├─ RetryButton
         └─ QuitButton
```

- `UIRoot`, `StageHUD`, `ResultPanel`, `PausePanel`과 `EventSystem`은 각각 하나만 존재한다.
- Canvas, CanvasScaler와 GraphicRaycaster는 각각 3개이며 StageHUD, ResultPanel과 PausePanel에 하나씩 존재한다.
- 세 Canvas는 Screen Space Overlay, Sorting Order `0`, Target Display `0`을 사용한다.
- 세 CanvasScaler는 Scale With Screen Size, 기준 해상도 `1920 x 1080`, Screen Match Mode `Match Width Or Height`, Match `0`을 사용한다.
- `UIRoot`의 자식 순서는 StageHUD, ResultPanel, PausePanel이며 현재 계층 순서에서 ResultPanel과 PausePanel은 StageHUD 뒤에 존재한다.
- ResultPanel과 PausePanel의 중앙 Panel은 각각 `480 x 360`이며 VerticalLayoutGroup을 사용한다.
- `InfiniteHUD`, `StageResultContent`와 `InfiniteResultContent`는 존재하지 않는다.
- 기존 Scene의 최상위 UI Object는 저장 시 활성 상태지만 `UIManagementSystem.Initialize()`가 런타임 UI State를 적용한다.

#### 현재 Serialized Reference 기준

`UIManagementSystem` Component fileID는 `1961406790`이며 현재 참조는 아래와 같다.

| Serialized Field | fileID | 현재 대상 |
|---|---:|---|
| `_stageHud` | 714045101 | `UIRoot/StageHUD` GameObject |
| `_resultPanel` | 547905304 | `UIRoot/ResultPanel` GameObject |
| `_pausePanel` | 895601724 | `UIRoot/PausePanel` GameObject |
| `_clearTimeText` | 1207588081 | `Clear Time Text` TMP_Text |
| `_retryButton` | 1861140804 | ResultPanel `RetryButton` Button |
| `_quitButton` | 2129891216 | ResultPanel `QuitButton` Button |
| `_pauseResumeButton` | 771433999 | PausePanel `ResumeButton` Button |
| `_pauseRetryButton` | 1363044890 | PausePanel `RetryButton` Button |
| `_pauseQuitButton` | 656707378 | PausePanel `QuitButton` Button |

- 위 기존 참조 대상은 Scene YAML에 각각 하나의 유효한 Object로 존재한다.
- `GameSystem`은 UIManagementSystem fileID `1961406790`, RuntimeDataSystem, UIInputSystem과 ResultSystem을 이미 참조한다.
- 생산 Scene의 `_selectedGameMode` 값은 `1`로 InfiniteMode를 가리킨다.

#### EventSystem과 입력 자산

- EventSystem GameObject는 fileID `2083408191`로 하나만 존재한다.
- EventSystem에는 EventSystem Component와 InputSystemUIInputModule이 각각 하나씩 존재한다.
- InputSystemUIInputModule은 Input System Package의 `DefaultInputActions.inputactions`를 참조한다.
- 프로젝트의 `InputSystem_Actions.inputactions`에는 UI `Navigate`, `Submit`, `Cancel`, `Point`와 `Click` Action 및 Keyboard와 Mouse Binding이 이미 존재한다.
- `InputSystem_Actions.cs` 생성 Wrapper에도 같은 UI Action 접근자가 존재한다.
- `UIInputSystem`이 해당 Wrapper를 사용해 입력을 수집하고 기존 ResultMenu와 PauseMenu가 이 입력을 사용한다.
- Phase 4 UI 표시 추가에는 신규 Action이 필요하지 않으므로 Input Action Asset과 생성 Wrapper를 수정하지 않는다.
- EventSystem을 추가하거나 교체하지 않는다.

#### Test 영향 범위

- 현재 Test 기준은 Edit Mode `177`, Play Mode `87`이며 Test Attribute 수와 Phase 3 기록이 일치한다.
- `ResultTextFormatterTests` 3개는 InfiniteMode 문자열, 내림, 경계값과 유효하지 않은 값 Test의 직접 확장 대상이다.
- 신규 순수 Mode·상태별 표시 매핑 Test를 Edit Mode에 추가한다.
- `ResultDataTests` 2개와 `ResultSystemTests` 3개는 Mode별 데이터 혼용 방지 회귀 대상으로 유지한다.
- `GameLifecycleIntegrationTests` 7개와 `StageGoalIntegrationTests` 2개에는 Result에서 StageHUD가 비활성화된다는 기존 기대가 있어 확정 규칙에 맞게 변경해야 한다.
- `PausePanelSceneConfigurationTests` 1개는 PausePanel과 현재 Mode HUD의 동시 표시 기대를 추가해야 한다.
- `InfiniteModeIntegrationTests` 7개는 Infinite HUD 초기화, 갱신, 종료 결과와 Retry Reset 통합 Test의 확장 대상이다.
- `PauseMenuIntegrationTests` 6개, `ResultMenuIntegrationTests` 5개, `GamePauseOrchestrationTests` 8개와 `UIInputSystemTests` 3개는 입력 및 상태 회귀 대상으로 유지한다.
- 생산 Scene 변경 전 별도의 Mode별 UI Scene 구조 Play Mode Test가 필요하다.

#### 확정된 코드 변경 후보

신규 파일 후보는 아래와 같다.

- `Assets/Scripts/Runtime/Core/UIVisibilityState.cs`
- `Assets/Tests/EditMode/UIVisibilityStateTests.cs`
- `Assets/Tests/PlayMode/ModeUIProductionSceneConfigurationTests.cs`

수정 파일 후보는 아래와 같다.

- `Assets/Scripts/Runtime/Features/ResultTextFormatter.cs`
- `Assets/Scripts/Runtime/Systems/UIManagementSystem.cs`
- `Assets/Scripts/Runtime/Systems/GameSystem.cs`
- `Assets/Tests/EditMode/ResultTextFormatterTests.cs`
- `Assets/Tests/PlayMode/GameLifecycleIntegrationTests.cs`
- `Assets/Tests/PlayMode/StageGoalIntegrationTests.cs`
- `Assets/Tests/PlayMode/PausePanelSceneConfigurationTests.cs`
- `Assets/Tests/PlayMode/InfiniteModeIntegrationTests.cs`
- `Assets/Tests/PlayMode/PauseMenuIntegrationTests.cs`
- `Assets/Tests/PlayMode/ResultMenuIntegrationTests.cs`

- 구현 중 확인되는 직접 회귀 Test만 최소 범위로 추가 수정하며 위 목록 밖의 이동, 물리와 Map Pattern 코드는 변경하지 않는다.
- `E_UIState`, `GameRuntimeData`, `InfiniteModeRuntimeData`, `ResultData`, `ResultSystem`, `UIInputSystem`, Input Action Asset과 생성 Wrapper는 변경하지 않는다.

#### 확정된 신규 Serialized Field 후보

`UIManagementSystem`에 아래 Inspector 참조를 추가하는 후보를 확정한다.

- `_infiniteHud`: `InfiniteHUD` GameObject
- `_stageResultContent`: `StageResultContent` GameObject
- `_infiniteResultContent`: `InfiniteResultContent` GameObject
- `_distanceText`: InfiniteHUD `DistanceText` TMP_Text
- `_scoreText`: InfiniteHUD `ScoreText` TMP_Text
- `_finalDistanceText`: InfiniteResultContent `FinalDistanceText` TMP_Text
- `_finalScoreText`: InfiniteResultContent `FinalScoreText` TMP_Text

- 기존 `_clearTimeText`, Result Retry·Quit Button과 Pause Resume·Retry·Quit Button 참조는 유지한다.

#### 사용자 Scene 작업 후보

- `UIRoot`에서 StageHUD 다음, ResultPanel 이전에 `InfiniteHUD`를 형제 Object로 추가한다.
- `InfiniteHUD` 아래에 기존 HUD와 같은 설정의 Canvas를 두고 `DistanceText`와 `ScoreText` TMP Text를 추가한다.
- 기존 ResultPanel 내부에 `StageResultContent`를 추가하고 기존 Clear Time 표시 Object를 그 아래에서 재사용한다.
- 기존 ResultPanel 내부에 `InfiniteResultContent`를 추가하고 `FinalDistanceText`와 `FinalScoreText` TMP Text를 추가한다.
- 기존 ResultPanel의 RetryButton과 QuitButton은 공용으로 유지하고 복제하지 않는다.
- 기존 PausePanel, Canvas, Pause 제목과 세 Button은 재사용하며 신규 PausePanel을 만들지 않는다.
- 기존 EventSystem을 유지하고 추가하지 않는다.
- 신규 Serialized Field를 위 대응 Object에 연결한다.
- 정확한 계층, Component, 초기 Text와 Inspector 연결 절차는 Step 8의 Test 우선 결과로 확정하며 Step 2에서는 Scene을 변경하지 않는다.

### 정적 검증

- Scene fileID, 기존 참조, GameObject와 Component 기준 수를 기록한다.
- 기존 Input Action Asset과 생성 Wrapper 변경 필요 여부를 확인한다.
- 기존 Test 177/87개의 직접 영향 범위를 기록한다.

### 정적 검증 결과

- Scene YAML의 기존 UI 계층, Component 수, fileID와 Serialized Reference를 읽기 전용으로 확인했다.
- UIManagementSystem, GameSystem, Runtime Data, Result Data와 입력 경로를 코드에서 확인했다.
- Test Attribute 기준 Edit Mode 177개와 Play Mode 87개를 확인했다.
- Input Action Asset과 생성 Wrapper의 변경이 필요하지 않음을 확인했다.
- Build와 Unity Test Runner를 실행하지 않았다.
- 생산 Scene을 수정하지 않았다.

### 수동 작업

- 없음.

### 완료 조건

- [x] 코드와 Scene 확장 지점이 확정되었다.
- [x] 사용자 Scene 작업 범위가 구체화되었다.

## Step 3. UI 표시 문자열과 Mode별 표시 모델을 Unit Test로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Infinite 현재 거리 문자열
- Infinite 현재 Score 문자열
- Infinite 최종 거리 문자열
- Infinite 최종 Score 문자열
- 0, 경계값, 큰 값과 유효하지 않은 값 처리
- Stage Result와 Infinite Result의 데이터 혼용 방지

### AI 작업

- 순수 Formatter 또는 표시 모델 Test를 먼저 추가한다.
- 최소 생산 코드를 구현한다.
- UI 코드에서 거리와 Score를 다시 계산하지 않고 Runtime/Result 값을 전달하도록 한다.

### 구현 내용

- `ResultTextFormatterTests`에 생산 코드보다 먼저 실패 기대 Test를 추가했다.
- InfiniteMode 현재 거리, 현재 Score, 최종 거리와 최종 Score의 승인된 문자열 형식을 Test한다.
- 거리 `0`, 소수 경계, `int.MaxValue`보다 큰 유효 거리와 Score `int.MaxValue`를 Test한다.
- 음수, `NaN`, 양의 무한대와 음의 무한대 거리가 `--`로 표시되는지 Test한다.
- 음수 Score가 `--`로 표시되는지 Test한다.
- Stage ResultData를 Infinite Result 형식으로 사용하거나 Infinite ResultData를 Stage Result 형식으로 사용하는 요청을 거부하는지 Test한다.
- null ResultData 요청을 거부하고 출력 문자열을 비우는지 Test한다.
- `ResultTextFormatter`에 현재 거리, 현재 Score, 최종 거리와 최종 Score Formatter를 추가했다.
- 거리 표시는 `Math.Floor`와 `InvariantCulture`를 사용하며 Runtime Data와 Result Data의 원본을 변경하지 않는다.
- `TryFormatStageResult()`와 `TryFormatInfiniteResult()`가 ResultData의 Mode 및 계약 Flag를 확인한 후 해당 Mode의 Text만 생성하도록 구현했다.
- `ResultData`, `InfiniteModeRuntimeData`, `ResultSystem`과 UI Scene은 변경하지 않았다.

### 예상 Test 기준

- 기존 `ResultTextFormatterTests` 3개에 27개 Test Case를 추가했다.
- 관련 `ResultTextFormatterTests` 예상 수는 30개다.
- 전체 Edit Mode 예상 수는 기존 177개에서 204개다.
- Play Mode Test 수는 변경하지 않았다.
- 위 정적 Test Attribute 기준과 사용자 Unity Test Runner의 전체 Test 수가 일치했다.

### 사용자 검증 결과

- Unity Script Compilation 성공을 확인했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 전체 Edit Mode Test 204개를 실행했고 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.

### 수동 작업

- 사용자는 Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
- 사용자는 Unity Test Runner에서 Edit Mode `ResultTextFormatterTests` 30개를 실행한다.
- 사용자는 기존 Edit Mode `ResultDataTests` 2개를 실행한다.
- Passed, Failed와 예상하지 않은 Error·Warning을 기록한다.
- Build, Play Mode Test와 Scene 작업은 이 Step에서 수행하지 않는다.

### 완료 조건

- [x] 표시 형식 Unit Test가 통과한다.
- [x] 기존 ResultTextFormatter와 ResultData Test가 통과한다.

## Step 4. Mode별 UI State 매핑을 Unit Test로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Stage Playing에서 StageHUD만 표시
- Infinite Playing에서 InfiniteHUD만 표시
- Stage Result에서 Stage Result Content만 표시
- Infinite Result에서 Infinite Result Content만 표시
- Pause에서 현재 Mode의 HUD와 PausePanel 표시
- Ending에서 현재 Mode의 HUD 표시 유지
- Result와 Ended에서 현재 Mode의 HUD와 ResultPanel 표시
- Resume, Retry와 Initialize 시 UI State Reset
- Mode 변경과 ResultMenu·PauseMenu 선택 상태의 독립성

### AI 작업

- Unity GameObject에 불필요하게 의존하지 않는 Mode별 UI 매핑 상태를 먼저 Test한다.
- UIManagementSystem은 확정된 매핑 결과만 Scene Object에 반영하도록 한다.

### 구현 내용

- 생산 코드보다 먼저 `UIVisibilityStateTests`를 추가했다.
- 신규 `UIVisibilityState`는 Unity GameObject에 의존하지 않고 게임 Mode, 게임 상태와 UI State를 받아 표시 여부만 관리한다.
- Initializing과 Ready에서는 모든 UI를 숨긴다.
- Playing에서는 현재 Mode의 HUD만 표시한다.
- Paused에서는 현재 Mode의 HUD와 PausePanel을 표시한다.
- Ending에서는 Result UI State가 먼저 전달되어도 현재 Mode의 HUD만 유지한다.
- Ended와 Result UI State가 함께 확정되면 현재 Mode의 HUD, ResultPanel과 해당 Mode Result Content를 표시한다.
- 새 Run과 Mode 변경 시 이전 Mode의 HUD, Panel과 Result Content 표시 상태를 제거한다.
- 유효하지 않은 Mode, 게임 상태 또는 UI State는 거부하고 모든 표시 상태를 제거한다.
- `UIManagementSystem`에 Mode, 게임 상태와 `UIVisibilityState`를 추가하고 매핑 결과만 GameObject 활성 상태에 적용하도록 변경했다.
- `UIManagementSystem.Initialize()`는 선택된 Mode를 받으며 ResultMenu와 PauseMenu 선택 상태 및 UI 표시 상태를 초기화한다.
- GameSystem은 모든 게임 상태 변경을 UIManagementSystem에도 전달한다.
- Runtime Data가 제거된 뒤의 Ended 상태도 UIManagementSystem에 전달하여 Result 표시 조합을 확정한다.
- 게임 시작 중단 시 UI의 게임 상태와 UI State를 `None`으로 초기화한다.
- 기존 PauseMenuState와 ResultMenu 선택 값은 서로 별도로 유지하며 Mode별 표시 매핑에 공유하지 않는다.

### 신규 Serialized Field

`UIManagementSystem`에 아래 필드를 선언했다.

- `_infiniteHud`
- `_stageResultContent`
- `_infiniteResultContent`

- 위 Scene Object는 아직 존재하지 않거나 연결되지 않았으며 Step 8의 구조 Test와 Step 9의 사용자 Scene 작업 전에는 Scene을 수정하지 않는다.
- 기존 StageHUD, ResultPanel, PausePanel과 Button 참조는 변경하지 않았다.

### 예상 Test 기준

- 신규 `UIVisibilityStateTests`는 18개다.
- 전체 Edit Mode 예상 수는 기존 204개에서 222개다.
- 기존 `PauseMenuStateTests` 10개를 선택 상태 회귀 범위로 유지한다.
- ResultMenu 선택 실행은 기존 Play Mode 통합 Test를 변경하지 않고 Step 7 회귀 검증 범위로 유지한다.
- 위 정적 Test Attribute 기준과 사용자 Unity Test Runner의 전체 Test 수가 일치했다.

### 사용자 검증 결과

- Unity Script Compilation 성공을 확인했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 전체 Edit Mode Test 222개를 실행했고 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.

### 수동 작업

- 사용자는 Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
- 사용자는 Unity Test Runner의 Edit Mode에서 `UIVisibilityStateTests` 18개를 실행한다.
- 사용자는 Edit Mode에서 기존 `PauseMenuStateTests` 10개를 실행한다.
- 가능하면 전체 Edit Mode Test 222개를 실행하여 공통 Core 회귀를 함께 확인한다.
- Passed, Failed와 예상하지 않은 Error·Warning을 기록한다.
- Build, Play Mode Test와 Scene 작업은 이 Step에서 수행하지 않는다.

### 완료 조건

- [x] Mode별 UI 매핑 Unit Test가 통과한다.
- [x] PauseMenu와 ResultMenu 선택 상태 회귀 범위가 통과한다.

## Step 5. InfiniteMode HUD 갱신 흐름을 Play Mode Test로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Infinite Run 시작 시 거리와 Score 0 표시
- Runtime Data 갱신 후 HUD Text 갱신
- 후진 시 확정된 최대 거리와 Score 표시 유지
- Pause 동안 표시 값 불변
- Resume 후 기존 값부터 갱신
- Ending, Result와 Ended 동안 마지막 HUD 표시 값 유지
- Stage Mode에서 Infinite HUD 갱신 경로 비활성
- Retry 후 HUD 0으로 Reset

### 구현 내용

- 생산 코드보다 먼저 `InfiniteHudIntegrationTests`를 추가했다.
- 생산 Scene을 사용하거나 변경하지 않고 Test가 임시 UI Object, TMP Text, Button과 GameRuntimeData를 구성한다.
- InfiniteMode Run 시작 시 `Distance: 0`, `Score: 0` 표시를 Test한다.
- Runtime Data 갱신 후 거리 내림과 현재 Score 표시를 Test한다.
- 감소하는 Runtime Data 갱신 요청이 거부된 후 최대 거리와 Score 표시가 유지되는지 Test한다.
- Pause 동안 Runtime Data 값이 달라져도 HUD 표시가 바뀌지 않고 Resume 후 새 값부터 갱신되는지 Test한다.
- Ending과 Ended에서 Runtime Data가 제거되어도 마지막 HUD 표시값이 유지되는지 Test한다.
- Stage Mode에서 StageHUD만 활성화되고 InfiniteHUD 갱신 경로가 사용되지 않는지 Test한다.
- Retry의 새 Runtime Data가 HUD를 `Distance: 0`, `Score: 0`으로 초기화하는지 Test한다.
- `UIManagementSystem.Initialize()`가 선택 Mode 대신 현재 GameRuntimeData를 받아 HUD 갱신에 필요한 Runtime Data를 보관하도록 변경했다.
- InfiniteMode Playing 상태의 화면 프레임에서만 InfiniteModeRuntimeData를 확인한다.
- 내림된 표시 거리 또는 Score가 이전 표시값과 달라진 경우에만 문자열을 생성하고 TMP Text에 할당한다.
- Pause, Ending과 Ended에서는 HUD Update 경로를 수행하지 않아 마지막 표시값을 유지한다.
- Runtime Data가 없거나 초기화되지 않은 경우 `Distance: --`, `Score: --`를 사용한다.
- `ResultTextFormatter.TryGetDisplayDistance()`가 유효성 확인과 표시용 내림값 생성을 단독 담당하도록 기존 Formatter를 확장했다.
- UIManagementSystem은 이동 거리 누적과 Score 계산을 수행하지 않는다.

### 신규 Serialized Field

`UIManagementSystem`에 아래 필드를 선언했다.

- `_distanceText`
- `_scoreText`

- 위 필드는 Step 8에서 생산 Scene 구조 Test와 정확한 연결 대상을 확정하고 Step 9에서 사용자가 연결한다.
- Step 5에서는 생산 Scene을 변경하지 않았다.

### 예상 Test 기준

- 신규 `InfiniteHudIntegrationTests`는 7개다.
- 전체 Play Mode 예상 수는 기존 87개에서 94개다.
- 전체 Edit Mode 예상 수는 222개로 유지된다.
- 위 정적 Test Attribute 기준과 사용자 Unity Test Runner의 전체 Test 수가 일치했다.

### 최초 Play Mode 회귀 실패와 수정

- 사용자가 전체 Play Mode Test를 실행했으며 3개 Test가 실패했다.
- `GameLifecycleIntegrationTests.EndGame_ClearsRuntimeAndStopsPhase2Systems`는 Result에서 StageHUD가 비활성화된다는 이전 기대값 때문에 실패했다.
- `StageGoalIntegrationTests.PlayerEntersGoal_ClearsStageAndEndsGameOnce`도 같은 이전 기대값 때문에 실패했다.
- 두 Test를 Step 1의 확정 규칙에 맞게 Result와 Ended에서 StageHUD와 ResultPanel이 함께 활성화되는 기대값으로 변경했다.
- `PausePanelSceneConfigurationTests.PausePanel_HasRequiredHierarchyReferencesAndStateMapping`은 게임 상태를 Playing으로 둔 채 UI State만 Pause로 변경하여 실패했다.
- 해당 Test가 UIManagementSystem에 게임 상태 `Paused`를 먼저 전달한 뒤 UI State `Pause`를 적용하도록 변경했다.
- Pause 해제 확인도 게임 상태 `Playing`과 UI State `StageHud`를 함께 적용하도록 변경했다.
- 세 실패는 확정된 Mode·게임 상태·UI State 조합을 생산 코드가 적용하면서 기존 단일 UI State 기대가 남아 발생한 회귀 Test 불일치였다.
- 실패 Test의 기대값을 약화하지 않고 Step 1에서 확정한 복합 UI 표시 규칙으로 교체했다.
- 로그의 신규 UI Serialized Field 미할당 Warning은 Step 9 전 생산 Scene에 해당 Object를 생성·연결하지 않은 현재 단계에서 발생했다.
- 신규 Scene 참조를 사용하는 전체 생산 Scene 회귀 Test와 Warning 부재 판정은 Step 9 이후 수행한다.

### 최종 사용자 검증 결과

- 수정 후 Unity Script Compilation 성공을 확인했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 전체 Edit Mode Test 222개를 실행했고 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 전체 Play Mode Test 94개를 실행했고 모두 성공했다.
- Play Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 최초 컴파일 실패와 Play Mode 회귀 실패는 수정 후 재발하지 않았다.

### 컴파일 실패와 수정

- 최초 사용자 Unity Script Compilation에서 `InfiniteHudIntegrationTests.cs`의 `TMPro`와 `TMP_Text`를 찾지 못하는 `CS0246` 4건이 발생했다.
- 원인은 `FlowState.PlayModeTests.asmdef`가 Test에서 직접 사용하는 `Unity.TextMeshPro` 어셈블리를 참조하지 않은 것이었다.
- Unity Package Cache의 `Unity.TextMeshPro.asmdef`에서 실제 Assembly Definition 이름이 `Unity.TextMeshPro`임을 확인했다.
- `FlowState.PlayModeTests.asmdef`의 references에 `Unity.TextMeshPro`를 추가했다.
- Runtime Assembly, Package, Scene과 TMP Asset은 변경하지 않았다.
- 수정 후 Unity Script Compilation과 `InfiniteHudIntegrationTests` 재실행 결과는 아직 확인되지 않았다.

### 정적 검증

- UI가 InfiniteMode 거리와 Score 계산을 중복하지 않는지 확인한다.
- 정상 프레임마다 불필요한 로그와 객체 할당을 추가하지 않는지 확인한다.
- Runtime Data 읽기와 UI 표시 책임을 구분한다.

### 수동 작업

- 사용자는 Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
- 사용자는 Unity Test Runner의 Play Mode에서 `InfiniteHudIntegrationTests` 7개만 실행한다.
- 이 Test Class 안에서 InfiniteMode HUD와 Stage Mode UI 회귀를 함께 검증한다.
- Passed, Failed와 예상하지 않은 Error·Warning을 기록한다.
- 아직 생산 Scene에 신규 Serialized Field가 연결되지 않았으므로 전체 Play Mode Test는 Step 9 이후에 실행한다.
- Build와 Scene 작업은 이 Step에서 수행하지 않는다.

### 완료 조건

- [x] Infinite HUD 통합 Test가 통과한다.
- [x] Stage Mode UI 회귀 Test가 통과한다.

## Step 6. InfiniteMode Result 표시 흐름을 Play Mode Test로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Infinite 종료 후 Final Distance와 Final Score 표시
- Stage Result에서는 Clear Time만 표시
- Infinite Result에서는 Stage Clear Time을 표시하지 않음
- Retry 후 이전 Result Text와 Result Data 제거
- 서로 다른 두 Run의 Result 표시 독립성
- 공용 Retry와 Quit Button 선택 흐름 유지

### 구현 내용

- 생산 코드보다 먼저 `ModeResultDisplayIntegrationTests`를 추가했다.
- 생산 Scene을 사용하거나 변경하지 않고 Test가 임시 TMP Text, Panel, Button과 GameRuntimeData를 구성한다.
- Stage ResultData는 Clear Time만 표시하고 Infinite 최종 Text를 비우는지 Test한다.
- Infinite ResultData는 Final Distance와 Final Score만 표시하고 Stage Clear Time을 비우는지 Test한다.
- UIManagementSystem 재초기화 시 이전 Result Text가 모두 제거되는지 Test한다.
- 서로 다른 두 Infinite Run이 이전 Result Text를 사용하지 않고 각 ResultData의 확정값을 표시하는지 Test한다.
- Infinite Run 뒤 Stage Run을 시작해도 이전 Infinite Result Text가 남지 않는지 Test한다.
- 공용 Retry와 Quit 선택 상태가 Mode와 독립적이고 새 Run에서 Retry로 초기화되는지 Test한다.
- `UIManagementSystem.SetResultData()`가 `TryFormatStageResult()`와 `TryFormatInfiniteResult()`로 Mode 계약을 구분하도록 변경했다.
- Stage Result에서는 `_clearTimeText`만 설정하고 Infinite Result Text를 제거한다.
- Infinite Result에서는 `_finalDistanceText`와 `_finalScoreText`만 설정하고 Clear Time Text를 제거한다.
- `UIManagementSystem.Initialize()`가 이전 Run의 Stage 및 Infinite Result Text를 모두 제거한다.
- `GameSystem.CreateInfiniteResultData()`가 ResultSystem 생성 성공 후 확정된 ResultData를 UIManagementSystem에 전달하도록 연결했다.
- 기존 `InfiniteModeIntegrationTests`에 임시 Final Text를 주입하고 GameSystem, ResultSystem과 UIManagementSystem을 거친 최종 문자열을 검증하도록 보강했다.
- UIManagementSystem과 Test는 ResultData의 확정값을 표시할 뿐 거리와 Score를 다시 계산하지 않는다.
- ResultSystem, ResultData, TimeRecord와 ScoreRecord는 변경하지 않았다.

### 신규 Serialized Field

`UIManagementSystem`에 아래 필드를 선언했다.

- `_finalDistanceText`
- `_finalScoreText`

- 위 필드는 Step 8에서 정확한 생산 Scene 구조와 연결 대상을 Test로 확정하고 Step 9에서 사용자가 연결한다.
- 기존 `_clearTimeText`, `_retryButton`과 `_quitButton`은 그대로 재사용한다.
- Step 6에서는 생산 Scene을 변경하지 않았다.

### 예상 Test 기준

- 신규 `ModeResultDisplayIntegrationTests`는 6개다.
- 기존 `InfiniteModeIntegrationTests` 7개는 실제 Infinite Result UI 전달 검증을 포함하도록 보강했다.
- 전체 Play Mode 예상 수는 기존 94개에서 100개다.
- 전체 Edit Mode 예상 수는 222개로 유지된다.
- 위 수는 정적 Test Attribute 기준이며 Unity Test Runner 통과 결과로 확정하지 않았다.

### 정적 검증

- [x] ResultData의 확정값을 그대로 표시하는지 확인했다.
- [x] ResultPanel이 결과 생성과 기록 책임을 갖지 않는지 확인했다.
- [x] Stage TimeRecord와 Infinite ScoreRecord가 혼용되지 않는지 확인했다.
- [x] Test Attribute 기준 Edit Mode 222개와 Play Mode 100개를 확인했다.
- [x] 신규 `ModeResultDisplayIntegrationTests` 6개를 확인했다.
- [x] `.meta` GUID 162개에 중복이 없음을 확인했다.
- [x] Play Mode Test asmdef JSON과 TextMeshPro 참조를 확인했다.
- [x] Scene, Input Action과 Package 파일을 변경하지 않았음을 확인했다.
- [x] 비활성화된 Test와 `git diff --check` 오류가 없음을 확인했다.

### 수동 작업

- 사용자는 Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
- 사용자는 Unity Test Runner의 Play Mode에서 `ModeResultDisplayIntegrationTests` 6개를 실행한다.
- 사용자는 Play Mode에서 `InfiniteModeIntegrationTests` 7개를 실행한다.
- 사용자는 Play Mode에서 기존 `ResultMenuIntegrationTests` 5개를 실행한다.
- 가능하면 전체 Play Mode Test 100개를 실행한다.
- Passed, Failed와 예상하지 않은 Error·Warning을 기록한다.
- Build와 Scene 작업은 이 Step에서 수행하지 않는다.

### 완료 조건

- [x] Mode별 Result 표시 Test가 통과한다.
- [x] 기존 ResultMenu 회귀 Test가 통과한다.

### 사용자 검증 결과

- Unity Script Compilation 성공
- Unity Script Compilation 관련 예상하지 않은 Error·Warning 없음
- Edit Mode Test `222 Passed, 0 Failed`
- Edit Mode Test 관련 예상하지 않은 Error·Warning 없음
- Play Mode Test `100 Passed, 0 Failed`
- Play Mode Test 관련 예상하지 않은 Error·Warning 없음

## Step 7. Keyboard와 Mouse의 전체 UI 입력 회귀를 자동 Test로 확정한다

- 진행 상태: **완료**

### Test 우선 항목

- Stage 및 Infinite HUD 상태에서 Pause 입력
- PausePanel Keyboard Navigate, Submit과 Cancel
- PausePanel Mouse Point와 Click
- Mode별 ResultMenu Keyboard와 Mouse 입력
- 빠른 중복 Submit·Click의 단일 실행
- UI State 전환 경계의 transient 입력 소비
- Result에서 Pause 거부

### 구현 내용

- 생산 코드 변경 전에 기존 `PauseMenuIntegrationTests`와 `ResultMenuIntegrationTests`에 Step 7 회귀 Test를 추가했다.
- `InfiniteHud_Cancel_OpensPausePanel`이 Infinite HUD에서 Cancel 입력으로 Pause 상태와 PausePanel에 진입하고 입력을 소비하는지 검증한다.
- `PlayingCancelAndSubmit_ConsumesSubmitAtPauseBoundary`가 Playing에서 같은 Frame에 들어온 Cancel과 Submit 중 Pause만 실행하고 남은 Submit이 다음 Frame의 Resume으로 이어지지 않는지 검증한다.
- `PauseClickAndSubmit_RetryExecutesOnlyOnce`가 PausePanel에서 같은 Frame의 Click과 Submit으로 새 Run을 한 번만 생성하는지 검증한다.
- Infinite Result에서 Keyboard Submit과 Mouse Click Retry가 각각 Infinite Mode의 새 Run을 시작하는지 검증한다.
- `ResultClickAndSubmit_RetryExecutesOnlyOnce`가 ResultMenu에서 같은 Frame의 Click과 Submit으로 새 Run을 한 번만 생성하는지 검증한다.
- 기존 Test가 Stage 및 Infinite HUD Pause, PausePanel Navigate·Submit·Cancel, Pointer·Click, Mode별 Retry, Quit, Result의 Cancel 거부를 계속 담당한다.
- 기존 `InputSystem_Actions`, `UIInputSystem`, `UIInputState`와 Scene UI 참조를 그대로 재사용한다.
- 상태별 입력은 `GameSystem.Update()`의 Playing, Paused, Ended 단일 분기에서만 처리되고 각 처리 경로가 실행 전후 transient 입력을 한 번 소비하므로 생산 코드 변경은 필요하지 않았다.
- Scene과 Input Action Asset은 변경하지 않았다.

### 예상 Test 기준

- `PauseMenuIntegrationTests`는 기존 6개에서 9개다.
- `ResultMenuIntegrationTests`는 기존 5개에서 8개다.
- 전체 Play Mode 예상 수는 기존 100개에서 106개다.
- 전체 Edit Mode 예상 수는 222개로 유지된다.
- 위 수는 정적 Test Attribute 기준이며 Unity Test Runner 통과 결과로 확정하지 않았다.

### 정적 검증

- [x] 기존 Input Action Asset과 Wrapper를 재사용하는지 확인했다.
- [x] 빠른 중복 Submit·Click과 상태 전환 경계를 자동 Test에 포함했다.
- [x] 하나의 입력 Frame이 둘 이상의 실행 경로를 시작하지 않는지 코드 경로를 확인했다.
- [x] Test Attribute 기준 Edit Mode 222개와 Play Mode 106개를 확인했다.
- [x] PauseMenu Test 9개와 ResultMenu Test 8개를 확인했다.
- [x] Scene, Input Action과 Package 파일을 변경하지 않았음을 확인했다.
- [x] 비활성화된 Test가 없음을 확인했다.

### 수동 작업

- 사용자는 Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
- 사용자는 Unity Test Runner의 Play Mode에서 `PauseMenuIntegrationTests` 9개를 실행한다.
- 사용자는 Play Mode에서 `ResultMenuIntegrationTests` 8개를 실행한다.
- 가능하면 전체 Play Mode Test 106개를 실행한다.
- Passed, Failed와 예상하지 않은 Error·Warning을 기록한다.
- Build와 Scene 작업은 이 Step에서 수행하지 않는다.

### 완료 조건

- [x] Keyboard와 Mouse UI 입력 Test가 통과한다.
- [x] 빠른 입력과 중복 실행 Test가 통과한다.

### 사용자 검증 결과

- Unity Script Compilation 성공
- Unity Script Compilation 관련 예상하지 않은 Error·Warning 없음
- Edit Mode Test `222 Passed, 0 Failed`
- Edit Mode Test 관련 예상하지 않은 Error·Warning 없음
- Play Mode Test `106 Passed, 0 Failed`
- Play Mode Test 관련 예상하지 않은 Error·Warning 없음

## Step 8. 생산 Scene UI 구조 Test와 사용자 작업 명세를 확정한다

- 진행 상태: **완료**

### AI 작업

- 생산 Scene 변경 전에 구조 Play Mode Test를 추가한다.
- 필요한 GameObject, Component, 이름, 계층과 Serialized Field를 명시한다.
- 기존 UIRoot, Canvas, EventSystem, Result Button과 Pause Button 재사용 범위를 명시한다.
- Scene 변경 전 YAML Object, Component와 참조 기준을 기록한다.

### 예상 최소 계층

```text
UIRoot
├─ StageHUD
├─ ResultPanel
│  └─ Canvas
│     └─ Panel
│        ├─ StageResultContent
│        │  └─ Clear Time Text Image
│        │     └─ ClearTimeText
│        ├─ InfiniteResultContent
│        │  └─ Infinite Result Image
│        │     ├─ FinalDistanceText
│        │     └─ FinalScoreText
│        ├─ RetryButton
│        └─ QuitButton
├─ PausePanel
   └─ Canvas
      └─ Panel
         ├─ Pause Title Text Image
         │  └─ Pause Title Text
         ├─ ResumeButton
         ├─ RetryButton
         └─ QuitButton
└─ InfiniteHUD
   └─ Canvas
      └─ Image
         ├─ DistanceText
         └─ ScoreText
```

### 확정한 Scene 계약

- `UIRoot`, `StageHUD`, `ResultPanel`, `PausePanel`과 각 기존 `Canvas`는 유지한다.
- `InfiniteHUD`는 `UIRoot`의 직접 자식이며 그 직접 자식으로 `Canvas` 하나를 둔다.
- Infinite Canvas의 직접 자식 `Image`는 HUD Text의 식별성을 위한 배경이며 `Image` Component가 있어야 한다.
- `DistanceText`와 `ScoreText`는 HUD 배경 `Image`의 직접 자식이며 각각 `TMP_Text`가 있어야 한다.
- 기존 ResultPanel의 `Canvas/Panel` 아래에 `StageResultContent`와 `InfiniteResultContent`를 직접 자식으로 추가한다.
- 기존 `Clear Time Text Image`를 `StageResultContent` 아래로 이동하고 기존 TMP Object의 이름을 `ClearTimeText`로 변경한다.
- `InfiniteResultContent`의 직접 자식 `Infinite Result Image`는 Result Text의 식별성을 위한 배경이며 `Image` Component가 있어야 한다.
- `FinalDistanceText`와 `FinalScoreText`는 Result 배경 `Infinite Result Image`의 직접 자식이며 각각 `TMP_Text`가 있어야 한다.
- ResultPanel의 기존 `RetryButton`과 `QuitButton`은 `Canvas/Panel`의 직접 자식으로 유지하고 Mode별로 복제하지 않는다.
- PausePanel의 기존 Canvas, Panel, Title 및 Resume·Retry·Quit Button 구조와 참조는 변경하지 않는다.
- Scene 전체의 EventSystem은 기존 한 개만 유지한다.
- 각 HUD와 Content의 Inspector 초기 활성 여부는 계약으로 고정하지 않고 Runtime UI 상태 매핑을 기준으로 한다.

### 생산 Scene 구조 Test

- 생산 코드나 Scene을 변경하기 전에 `ModeUISceneConfigurationTests` 1개를 추가했다.
- Test는 정확한 직접 부모 관계, 필수 Canvas 구성, TMP 및 Button Component, UIManagementSystem의 16개 Serialized Reference와 단일 EventSystem을 검증한다.
- 이 Test는 Step 9 Scene 작업 전에는 `InfiniteHUD`가 없으므로 실패하는 것이 정상이다.
- Scene 작업 후 전체 Play Mode 예상 수는 기존 106개에서 107개다.
- Edit Mode 예상 수는 222개로 유지된다.

### UIManagementSystem Inspector 연결표

| Field | 연결 Object 또는 Component |
|---|---|
| `_stageHud` | `UIRoot/StageHUD` GameObject |
| `_infiniteHud` | `UIRoot/InfiniteHUD` GameObject |
| `_resultPanel` | `UIRoot/ResultPanel` GameObject |
| `_pausePanel` | `UIRoot/PausePanel` GameObject |
| `_stageResultContent` | `ResultPanel/Canvas/Panel/StageResultContent` GameObject |
| `_infiniteResultContent` | `ResultPanel/Canvas/Panel/InfiniteResultContent` GameObject |
| `_clearTimeText` | `StageResultContent/.../ClearTimeText` TMP Component |
| `_distanceText` | `InfiniteHUD/Canvas/Image/DistanceText` TMP Component |
| `_scoreText` | `InfiniteHUD/Canvas/Image/ScoreText` TMP Component |
| `_finalDistanceText` | `InfiniteResultContent/Infinite Result Image/FinalDistanceText` TMP Component |
| `_finalScoreText` | `InfiniteResultContent/Infinite Result Image/FinalScoreText` TMP Component |
| `_retryButton` | 기존 Result `RetryButton` Button Component |
| `_quitButton` | 기존 Result `QuitButton` Button Component |
| `_pauseResumeButton` | 기존 Pause `ResumeButton` Button Component |
| `_pauseRetryButton` | 기존 Pause `RetryButton` Button Component |
| `_pauseQuitButton` | 기존 Pause `QuitButton` Button Component |

### 정적 검증

- [x] Scene YAML에서 기존 UIRoot와 세 UI Panel의 부모 관계 및 Canvas 구조를 확인했다.
- [x] 기존 Result 및 Pause Button과 Serialized fileID를 확인했다.
- [x] 기존 EventSystem이 한 개임을 확인했다.
- [x] 신규 구조 Test가 기존 Canvas, Button과 EventSystem 재사용을 강제하는지 확인했다.
- [x] 생산 Scene, Input Action과 Package 파일을 변경하지 않았음을 확인했다.
- [x] Test Attribute 기준 Edit Mode 222개와 Play Mode 107개를 확인했다.
- [x] `.meta` GUID 163개에 중복이 없고 비활성화된 Test와 `git diff --check` 오류가 없음을 확인했다.

### 수동 작업

- 없음. Step 8에서는 위 명세만 검토하며 Scene 편집은 Step 9에서 수행한다.
- 신규 구조 Test는 Step 9 완료 전 실패가 정상이라 이 Step에서는 Unity Test Runner로 실행하지 않는다.

### 완료 조건

- [x] 생산 Scene 구조 Test가 먼저 작성되었다.
- [x] 사용자 Scene 작업 절차가 Inspector Field 단위로 확정되었다.

## Step 9. 사용자가 Mode별 UI를 Scene에 구성하고 연결한다

- 진행 상태: **완료**

### 사용자 Scene 작업

1. Hierarchy의 `UIRoot`를 선택하고 직접 자식으로 빈 GameObject `InfiniteHUD`를 생성한다.
2. `InfiniteHUD` 아래에 UI Canvas를 하나 생성하고 이름을 `Canvas`로 유지한다. 자동 생성된 EventSystem이 있다면 새것만 삭제해 기존 Scene EventSystem 한 개를 유지한다.
3. Infinite Canvas의 직접 자식으로 UI Image `Image`를 생성하고, 그 직접 자식으로 TextMeshPro UI Text `DistanceText`, `ScoreText`를 생성한다. Image는 글자 식별성을 보장하는 배경으로 유지한다.
4. `ResultPanel/Canvas/Panel`의 직접 자식으로 빈 GameObject `StageResultContent`, `InfiniteResultContent`를 생성한다.
5. 기존 `Clear Time Text Image`를 `StageResultContent`의 자식으로 이동하고 그 아래 기존 TMP Object `Clear Time Text`의 이름을 `ClearTimeText`로 변경한다. Component나 기존 참조는 삭제하지 않는다.
6. `InfiniteResultContent`의 직접 자식으로 UI Image `Infinite Result Image`를 생성하고, 그 직접 자식으로 TextMeshPro UI Text `FinalDistanceText`, `FinalScoreText`를 생성한다. Image는 글자 식별성을 보장하는 배경으로 유지한다.
7. 기존 Result `RetryButton`, `QuitButton`은 `ResultPanel/Canvas/Panel`의 직접 자식으로 그대로 두고 새 Button을 만들지 않는다.
8. 기존 PausePanel 계층과 Resume·Retry·Quit Button은 변경하지 않는다. 필요한 경우 배치와 크기만 조정한다.
9. `UIManagementSystem` Inspector에서 Step 8의 연결표에 따라 16개 Serialized Field를 모두 연결한다.
10. 각 새 TMP Text의 초기 문구는 식별용으로 설정할 수 있지만 Runtime 포맷이 최종 문구를 덮어쓰도록 별도 갱신 Script를 추가하지 않는다.
11. 초기 활성 상태보다 Runtime UI State 매핑 결과를 우선하며 Inspector에 Missing Reference가 없게 한다.
12. Scene을 저장하고 닫았다가 다시 열어 계층과 Serialized Reference 유지를 확인한다.
13. Missing Script, Missing Reference, 중복 이름, 중복 Canvas와 중복 EventSystem이 없는지 확인한다.

### AI 후속 정적 검증

- 사용자가 저장한 Scene YAML의 이름, 계층, Component와 fileID를 검사한다.
- Phase 4 범위 밖 UI와 Component가 추가되지 않았는지 확인한다.
- 생산 Scene 구조 Test의 기대와 실제 Scene을 대조한다.

### AI 후속 정적 검증 결과

- [x] `UIRoot` 아래 StageHUD, InfiniteHUD, ResultPanel과 PausePanel의 부모 관계를 확인했다.
- [x] Infinite HUD와 Infinite Result의 식별용 배경 Image 및 하위 TMP Text 구조를 확인했다.
- [x] Stage Clear Time UI가 StageResultContent 아래에 있고 TMP Object 이름이 `ClearTimeText`인지 확인했다.
- [x] UIManagementSystem의 16개 Serialized Field가 모두 0이 아닌 fileID로 연결되었음을 확인했다.
- [x] 각 Text 참조가 `TextMeshProUGUI` Component를 가리키는지 확인했다.
- [x] 기존 Result 및 Pause Button 참조가 유지됨을 확인했다.
- [x] Scene EventSystem이 한 개이고 Missing Script가 없음을 확인했다.
- [x] 생산 Scene 구조 Test를 배경 Image가 포함된 확정 계층과 일치시켰다.
- 저장 후 참조 유지는 Scene을 다시 로드하는 생산 Scene 구조 Test의 사용자 통과 결과로 확정했다.

### 완료 조건

- [x] 저장·재개방 후 모든 UI 참조가 유지된다.
- [x] 생산 Scene 구조 Test가 통과한다.
- [x] Scene에 Missing 또는 중복 UI 구성이 없다.

### 사용자 검증 결과

- Unity Script Compilation 성공
- Unity Script Compilation 관련 예상하지 않은 Error·Warning 없음
- Edit Mode Test `222 Passed, 0 Failed`
- Edit Mode Test 관련 예상하지 않은 Error·Warning 없음
- Play Mode Test `107 Passed, 0 Failed`
- Play Mode Test 관련 예상하지 않은 Error·Warning 없음

## Step 10. 전체 Compile과 자동 회귀 Test를 수행한다

- 진행 상태: **완료**

### AI 정적 검증

- [x] 신규 Script와 Test `.meta` 및 GUID를 검사했다.
- [x] Test Ignore, 삭제와 기대값 약화 여부를 검사했다.
- [x] Scene Serialized Reference와 Mode별 UI 매핑을 검사했다.
- [x] 정상 프레임 반복 로그와 범위 밖 기능 추가 여부를 검사했다.
- [x] 관련 Feature, System과 Roadmap 문서가 구현과 일치하는지 확인했다.

### AI 정적 검증 결과

- 신규 Asset 5개와 대응 `.meta` 5개가 모두 존재한다.
- Asset GUID 163개에 중복이 없다.
- Edit Mode Test Attribute 222개와 Play Mode Test Attribute 107개를 확인했다.
- Ignore, Explicit, Inconclusive Test와 Test Attribute 삭제가 없다.
- HUD 유지 규칙에 따라 변경된 기존 Assert는 현재 계약과 일치한다.
- 모든 asmdef, Package manifest와 lock JSON 구문이 유효하다.
- UIManagementSystem의 16개 Serialized Field가 모두 유효한 Scene fileID를 참조한다.
- Scene에 Missing Script가 없고 EventSystem은 한 개다.
- Infinite HUD와 Mode별 Result Content의 이름, 계층, TMP 및 배경 Image가 생산 Scene 구조 Test와 일치한다.
- HUD 갱신은 Infinite Playing에만 수행하며 실제 표시값이 달라질 때만 TMP Text를 갱신한다.
- 추가된 LogError는 오류 경로에만 있고 정상 Frame 반복 Log는 추가되지 않았다.
- Package, Input Action과 ProjectSettings 변경이 없고 변경 범위는 Phase 4 문서, Runtime, Test와 SampleScene에 한정된다.
- Scene 이외 변경분의 `git diff --check` 오류가 없다. Scene의 빈 `m_Name` 후행 공백은 Unity 직렬화 형식이며 Missing 참조나 실행 오류가 아니다.

### 사용자 자동 검증

1. Unity Editor에서 Script Compilation 성공을 확인한다.
2. 예상하지 않은 Compile Error와 Warning이 없는지 확인한다.
3. Unity Test Runner에서 전체 Edit Mode Test를 실행한다.
4. Unity Test Runner에서 전체 Play Mode Test를 실행한다.
5. Passed, Failed와 전체 Test 수를 기록한다.
6. Test 실행 중 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 완료 조건

- [x] 전체 정적 검증이 통과한다.
- [x] 전체 Edit Mode와 Play Mode Test가 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

### 사용자 자동 검증 결과

- Unity Script Compilation 성공
- Unity Script Compilation 관련 예상하지 않은 Error·Warning 없음
- Edit Mode Test `222 Passed, 0 Failed`
- Edit Mode Test 관련 예상하지 않은 Error·Warning 없음
- Play Mode Test `107 Passed, 0 Failed`
- Play Mode Test 관련 예상하지 않은 Error·Warning 없음

## Step 11. 사용자가 Build와 최소 화면 검증을 수행한다

- 진행 상태: **완료**

### Build 전 AI 정적 확인

- [x] Build Settings의 Scene 포함 여부와 현재 대상 Platform 설정을 정적으로 확인했다.
- [x] Build에 필요한 Scene, Script와 Asset 참조 누락 가능성을 확인했다.
- [x] 자동 Test로 확인한 상태, 수치와 빠른 입력을 수동 체크리스트에서 제외했다.

### Build 전 AI 정적 확인 결과

- `EditorBuildSettings.asset`에는 활성화된 `Assets/Scenes/SampleScene.unity` 한 개가 포함되어 있다.
- Build Settings의 Scene GUID와 `SampleScene.unity.meta` GUID `99c9720ab356a0642a771bea13969a05`가 일치한다.
- 현재 사용자 Build 설정은 Windows Standalone `Win64`, Architecture `x64`다.
- Player 기본 화면 크기는 `1024 x 768`이며 Step 1의 최소 검증 해상도 중 하나와 일치한다.
- Scene에 Missing Script가 없고 UIManagementSystem의 16개 Serialized Reference가 모두 연결되어 있다.
- 직전 Script Compilation, Edit Mode 222개와 Play Mode 107개가 모두 통과했다.
- 프로젝트의 기존 `Build/` 폴더는 `.gitignore`로 제외되어 있어 출력 대상으로 사용할 수 있다.
- 기존 `Build/Unity_Flow_State.exe`는 2026-08-28 산출물이므로 이번 Build 결과와 혼동하지 않도록 새 산출물의 수정 시간을 확인한다.

### 사용자 Build 작업

1. Unity Editor의 Build Settings에서 `SampleScene`이 활성 Scene으로 포함되어 있는지 확인한다.
2. Phase 4 Step 1에서 확정한 대상 Platform과 Build 설정을 사용한다.
3. 프로젝트 외부 또는 Git 추적 대상이 아닌 전용 Build 출력 폴더를 선택한다.
4. Unity Editor에서 Build를 한 번 실행한다.
5. Build 성공과 Build 과정의 예상하지 않은 Error·Warning 부재를 확인한다.
6. 생성된 Player를 실행하여 Stage Mode와 InfiniteMode의 HUD 및 Result 화면이 식별 가능한지 확인한다.
7. PausePanel과 공용 Result Button이 화면 밖으로 벗어나거나 겹치지 않는지 확인한다.
8. 종료 후 Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 최소 화면 확인 순서

1. Stage Mode에서 HUD가 보이는지 확인한다.
2. Pause를 열어 StageHUD와 PausePanel이 함께 보이고 제목 및 세 Button이 식별되는지 확인한다.
3. Stage Result에서 StageHUD, Clear Time, Retry와 Quit이 보이는지 확인한다.
4. InfiniteMode에서 배경 Image 위 Distance와 Score가 식별되는지 확인한다.
5. InfiniteMode Pause에서 InfiniteHUD와 PausePanel이 함께 보이는지 확인한다.
6. Infinite Result에서 InfiniteHUD, Final Distance, Final Score, Retry와 Quit이 식별되는지 확인한다.
7. 위 화면을 `1920 x 1080`, `1280 x 720`, `1024 x 768`에서 확인하고 겹침과 화면 잘림 여부만 기록한다.

### 수동 검증 제한

- 거리·Score 정확성, Mode 상태, Retry Reset, 빠른 UI 입력과 중복 실행은 자동 Test 결과를 사용한다.
- 수동으로는 Text 가독성, Object 겹침, 화면 잘림과 최종 Player 표시만 확인한다.
- 빠른 조작 속도나 정밀한 입력 타이밍을 요구하지 않는다.

### 완료 조건

- [x] Build가 성공한다.
- [x] Stage와 Infinite UI가 Build Player에서 식별 가능하다.
- [x] UI 겹침, 잘림과 예상하지 않은 Error·Warning이 없다.

### 사용자 검증 결과

- Unity Editor Player의 `1920 x 1080`, `1280 x 720`, `1024 x 768`에서 적절한 UI 상태를 확인했다.
- Development Build Player에서 적절한 UI 상태를 확인했다.
- 사용자는 현재 UI의 식별성, 배치와 화면 상태가 Phase 4 최소 품질 기준에 적합하다고 판정했다.
- Build 성공과 Stage 및 Infinite UI 식별 가능 조건을 충족했다.
- Build와 Editor Player에서 예상하지 않은 Error·Warning이 없음을 확인했다.

## Step 12. Phase 4 완료 근거를 정리한다

- 진행 상태: **완료**

### AI 작업

- 최종 정적 검증, Compile, Test 수와 Build 결과를 기록한다.
- 최소 화면 검증과 미해결 사항을 기록한다.
- 별도 Phase 4 Verification Result Task 문서를 작성한다.
- 모든 완료 조건 충족 시에만 Roadmap Phase 4를 `완료`로 변경한다.

### 수동 작업

- 이전 Step에서 확인하지 못한 새 수동 작업을 추가하지 않는다.

### 완료 조건

- [x] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [x] 최소 화면 검증 결과가 기록되어 있다.
- [x] Phase 4 범위 밖 기능이 포함되지 않았다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

### 완료 근거 문서

- `AI/90_Tasks/Prototype_2/20260903_01_Phase4VerificationResult.md`
- Phase 4 범위의 미해결 사항 없음
- 추가 수동 작업 없음

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 UI 표시 형식과 최소 범위 선택
2. 구현 Step 이후 Unity Script Compilation 결과 확인
3. AI가 지정한 관련 및 전체 Unity Test Runner 실행
4. Step 9의 Scene UI GameObject 생성, 배치와 Serialized Field 연결
5. Scene 저장·재개방과 Missing Reference 확인
6. Step 11의 Unity Build 실행
7. Build Player의 Text 가독성, 겹침, 잘림과 예상하지 않은 로그 확인

상태값, 수치, 호출 횟수, 빠른 입력, 중복 실행과 Retry 초기화는 수동 작업에 포함하지 않고 정적 검증 또는 자동 Test로 처리한다.

---

# 영향 범위

- UIManagementSystem과 GameSystem
- Runtime Data와 Result Data 표시 경로
- StageHUD, InfiniteHUD, ResultPanel과 PausePanel
- Keyboard와 Mouse UI 입력
- SampleScene
- Edit Mode 및 Play Mode Test
- 관련 Feature, System 문서와 Roadmap

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`
- `AI/90_Tasks/Prototype_2/20260902_01_Phase3VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 검증 결과

- Roadmap Phase 4 목표, 구현 대상과 완료 조건을 12개 실행 Step으로 분리했다.
- 정적 검증과 Unit Test를 생산 코드 및 Scene 수동 작업보다 앞에 배치했다.
- 빠른 입력과 상태 변화는 자동 Test 범위로 고정했다.
- Scene, Unity Compile/Test Runner, Build와 최종 화면 확인만 사용자 수동 작업으로 분리했다.
- Step 1의 UI 표시 규칙을 확정하고 관련 문서에 반영했다.
- Step 2에서 UI, 데이터 전달, 입력, Test와 생산 Scene의 확장 지점을 정적으로 확정했다.
- Step 2에서는 Build, Unity Test Runner와 생산 Scene 변경을 수행하지 않았다.
- Step 3의 Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Step 3 완료 기준으로 전체 Edit Mode Test `204 Passed, 0 Failed`를 확인했다.
- Step 4의 Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Step 4 완료 기준으로 전체 Edit Mode Test `222 Passed, 0 Failed`를 확인했다.
- Step 5의 수정 후 Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Step 5 완료 기준으로 전체 Edit Mode Test `222 Passed, 0 Failed`와 전체 Play Mode Test `94 Passed, 0 Failed`를 확인했다.
- Step 6에서 Mode별 Result 표시 흐름을 구현하고 전체 Play Mode Test `100 Passed, 0 Failed`를 확인했다.
- Step 7에서 Keyboard와 Mouse 전체 UI 입력 회귀를 확정하고 전체 Play Mode Test `106 Passed, 0 Failed`를 확인했다.
- Step 8에서 생산 Scene 구조 Test와 Inspector Field 단위 작업 명세를 확정했다.
- Step 9에서 사용자가 Mode별 UI와 배경 Image를 Scene에 구성하고 16개 Serialized Field를 연결했다.
- Step 10 최종 기준으로 Script Compilation, Edit Mode `222 Passed, 0 Failed`, Play Mode `107 Passed, 0 Failed`를 확인했다.
- Step 11에서 Windows Standalone Development Build와 세 해상도의 최소 화면 검증을 완료했다.
- Build와 Editor Player에서 예상하지 않은 Error와 Warning이 없었다.
- Phase 4 범위의 미해결 사항이 없으며 모든 완료 조건을 충족했다.
- 별도 `20260903_01_Phase4VerificationResult.md`에 최종 완료 근거를 기록했다.

---

# 후속 작업

Prototype 3을 시작하기 전에 요구사항과 Implementation Roadmap을 확정한다.
