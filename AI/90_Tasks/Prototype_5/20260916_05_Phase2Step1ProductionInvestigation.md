# 작업 정보

## 작업명

Prototype 5 Phase 2 Step 1 생산 연결 및 UI 참조 정적 조사

## 작업 일자

20260916

## 작업 담당자

AI

---

# 작업 목적

Momentum Landing 속도 효과 제거, Scoring Version 2 생산 연결과 UI 확장에 필요한 변경 지점을 확인한다. Step 1의 조사만 수행하며 구현, Scene 편집, Unity 실행은 수행하지 않는다.

---

# 작업 대상

- `Assets/Scripts/Runtime`의 Momentum Landing, 이동, InfiniteMode, Runtime Data, Result와 UI 경로
- `Assets/Tests/EditMode`, `Assets/Tests/PlayMode`의 관련 기존 Test
- `Assets/Scenes/SampleScene.unity`의 저장된 YAML과 Script GUID·fileID 참조: 읽기 전용
- Phase 2 계획과 관련 System·Feature 계약

---

# 작업 전 상태

- 작업 시작 시 `git status --short` 출력이 없었다.
- Phase 1 순수 모델은 존재하지만 생산 System에서 Momentum·새 Score 모델을 생성하거나 호출하지 않는다.
- Phase 1의 Compilation 및 Edit Mode 559개 통과는 이전 기록이다. 이번 작업에서는 재실행하거나 현재 Test 통과를 판정하지 않았다.

---

# 조사 내용

## 1. 속도 효과와 착지 성공 전달

생산 경로는 다음과 같다.

`PlayerMovementSystem.FixedUpdate → ProcessMovementStep → CalculateMovementResult → UpdateMomentumLanding → ResolveLanding → MomentumLandingFeature.TryCompleteLanding → ApplyMovementResult → PlayerControllerSystem.ApplyMovement`

- `MomentumLandingFeature.cs:82`의 `TryCompleteLanding`은 성공 시 입력 속도의 절댓값에 `_speedMultiplier`를 곱하고 `_maximumHorizontalSpeed`로 제한한다. 기본값과 Scene 값은 각각 `1.15`, `14`이다. `_momentumLandingWindow`는 `0.15`이다.
- `PlayerMovementSystem.cs:310`에서 성공 여부와 결과 속도를 받고, `ResolveLanding`에서 결과 속도를 실제 이동 결과에 반영한다. Step 2의 속도 증가 제거 지점이다.
- Feature의 `_hasLandingResolved`는 동일 점프의 중복 착지 판정을 막는다. 그러나 생산 경로에는 `MomentumScoreState.TryStep`에 전달할 단조 증가 성공 요청 ID가 없다.
- `PlayerMovementRuntimeData.IsLastLandingMomentum`은 마지막 착지 종류를 유지하는 값이다. 매 물리 프레임 이를 성공 이벤트로 처리하면 동일 성공을 반복 반영할 수 있다. `ResolveLanding`의 실제 성공 순간을 기준으로 Run별 ID를 전달해야 한다.
- `MomentumScoreState`는 Version 2, 중복·역행 ID 거부, 성공 우선, 만료, Pause·Resume·Finalize·Reset을 이미 제공한다. 일반 착지·Wall 접촉 통지 API는 상태를 초기화하지 않는다. 기존 모델을 생산 상태에 연결하는 것이 후속 작업이다.
- Feature와 이동 System은 Stage·Infinite 공통 경로다. 속도 효과 제거의 Stage 회귀도 확인하고, Score 배율 갱신은 Infinite Run으로 제한해야 한다.

### 자동 이동과 Move 입력 계약

- `PlayerInputSystem.EnablePlayerActionMap`은 Player Map을 활성화한 뒤 `Move.Disable()`을 호출한다.
- `PlayerInputState`에는 Jump와 Momentum Landing 버튼 상태만 있다.
- `PlayerMovementSystem.CreateInitialCalculation`은 입력값을 받지 않는 `PlayerMovementMath.CalculateAutoHorizontalSpeed`를 호출한다. 이 함수는 기본 속도와 현재 속도 중 큰 값을 목표로 하여 획득 속도를 유지한다.
- 입력 기반 `PlayerMovementMath.CalculateHorizontalSpeed`는 존재하지만 생산 호출부가 없다. 기존 Input Actions에는 Move의 키보드·Gamepad 바인딩이 존재한다.
- `PlayerMovementSystem.md`, `PlayerInputSystem.md`와 `StagePlay.md`는 자동 이동·Move 비활성화를 확정 계약으로 명시한다.
- Roadmap 005에 기록됐던 입력 감속·정지는 요구사항이 아닌 것으로 사용자 확인 후 정정했다. Input·이동 코드는 변경하지 않고 기존 자동 이동 계약을 유지한다.

## 2. Run·Score·Version 연결 지점

| 경계 | 현재 생산 동작 | Phase 2 후속 연결 지점 |
|---|---|---|
| 새 Run | `GameSystem.StartGame → RuntimeDataSystem.CreateRuntimeData → GameRuntimeData.Initialize` | 생산 Infinite Run에서 현재 Version을 명시적으로 선택 |
| Version | `GameRuntimeData`가 `InfiniteModeRuntimeData.Initialize()`를 호출하고 무인자 API가 Legacy `1` 선택 | `Initialize(ScoringVersion.Current)` 경로 연결. 무인자 Legacy API와 호환 Test는 별도 유지 |
| 시작 거리·Score | `InfiniteModeSystem.InitializeRunMetrics`가 Rigidbody X 원점, `ScoreCalculator`, 0점 초기화 | `MomentumScoreState.StartRun`과 `InfiniteScoreState.Initialize`를 같은 Run Version으로 연결 |
| 매 물리 갱신 | `FixedUpdate → ProcessRunMetrics → UpdateRunMetrics`, 최대 전진 거리 전체를 `ScoreCalculator.TryCalculate`로 계산 | `InfiniteScoreState.TryUpdate(version, distance, multiplier)` 사용. 모델 내부에서 거리 증가분·Bonus 정밀 누적 수행 |
| Runtime 공개 값 | `CurrentDistance`, `CurrentScore`, Difficulty, Version, 최종 확정 여부 | Base·Bonus·Distance·Collectible·Total과 현재/최고 배율·남은 시간 표시용 값을 연결 |
| Collectible | `GameRuntimeData.CollectibleRuntimeData.CurrentScore`가 공통 소유 | 공통 획득 상태를 유지하고 배율 없이 합산. 별도 획득 상태를 중복 생성하지 않음 |
| 최종 확정 | `FinalizeRunMetrics`에서 최종 거리 갱신 후 Distance 및 Runtime 최종화 | Score·Momentum·Runtime의 같은 Version과 최종 값을 한 번 확정한 뒤 종료 요청 |
| 결과 요청 | `GameSystem.CreateInfiniteResultData → ResultSystem.CreateInfiniteResultData` | Run Version, Base·Bonus·Distance·Collectible, 최고 배율을 전달하는 생산 API 연결 |
| 결과 생성 | `ResultSystem`이 Legacy `ScoreRecord.TryRecord`를 호출 | 이미 있는 Version 2 `ScoreRecord.TryRecord`와 `ResultData` 생성자 재사용 |

`ScoringVersion.Current`는 이미 `2`이다. `ResultData`에는 Version 2 구성 요소가 있지만, 현재 생산 ResultSystem은 이를 전달하지 않는다. 단순히 Runtime Version 숫자만 바꾸면 기존 계산·결과 경로가 남으므로 생산 경로 전체를 함께 전환해야 한다.

현재 `InfiniteModeRuntimeData.TryUpdate(distance, score)`에는 요청 Version 인자가 없다. `ScoreRecord`의 Version 2 API는 전달된 Version과 구성 요소를 검증하지만 Runtime 객체를 직접 받지 않는다. Runtime·계산 상태·Result 요청 간 Version 일치 검증은 후속 연결 경계에서 보강해야 한다.

## 3. Pause·종료·Retry와 물리 갱신 순서

- 시작: Runtime 생성 → UI·Result 초기화 → Controller·Collision·Stage·Collectible 연결 → Movement → InfiniteMode → Camera 초기화 → 입력 활성화 → Playing 전환이다.
- Pause: `GameSystem.PausePlaySystems`가 Stage, InfiniteMode, Movement, Physics를 중단한다. InfiniteMode의 Pause는 현재 Difficulty·Pattern 상태를 보존한다. Momentum Timer 보존도 이 경계에 연결한다.
- Resume: Physics → Movement → InfiniteMode → Stage 순으로 재개한다. Momentum 상태를 새로 만들지 않고 보존 상태에서 재개해야 한다.
- 정상 Infinite 종료: 진행 속도 또는 추락 조건 → `FinalizeRunMetrics` → `TryEndInfiniteStage` → Stage 종료 통지 → `GameSystem.HandleStageEnded` → Result 생성·UI 전달 → `EndGame` 순이다.
- `EndGame`은 Ending으로 전환하고 System을 정지한 뒤 Runtime Data를 Clear하고 Ended로 전환한다. `InfiniteModeSystem.Stop`도 내부 거리·Score 상태를 Reset한다. Result의 배율·Bar를 유지하려면 정지·Clear 전에 최종 UI 상태를 전달·보존해야 한다.
- 현재 UI `Update`는 Playing에서만 HUD를 읽는다. 마지막 FixedUpdate에서 확정된 값이 다음 UI Update 전에 종료될 수 있으므로, 마지막 화면 캐시가 곧 최종 수치라고 가정하지 않는다. 종료 직전 최종 HUD 동기화 경계가 필요하다.
- Pause 중 Retry는 `EndGame` 후 `StartGame`으로 새 Runtime·Result를 생성한다. 직접 EndGame 경로는 먼저 Ending으로 바뀌므로 `HandleStageEnded`의 Playing/Paused 조건을 통과하지 않는다. 정상 결과 생성 경로와 재시작을 위한 정리 경로를 구분해야 한다.
- Movement와 InfiniteMode는 각각 별도 `FixedUpdate`를 사용한다. 해당 Script와 `.meta` 조사에서 명시적 실행 순서를 확인하지 못했다. 착지 이전 이동분에 새 배율을 소급 적용하지 않도록 거리 확정 → 성공 반영 → 이후 거리 증가분 적용 순서를 명시적으로 보장하고 Test로 검증해야 한다. 성공과 만료가 겹치면 기존 순수 모델의 성공 우선 규칙을 사용한다.

## 4. 표시 경로와 구성 요소

| 표시 요소 | 현재 경로 | 후속 작업 |
|---|---|---|
| 현재 거리 | Runtime `CurrentDistance` → `UpdateInfiniteHud` → `FormatCurrentDistance` | 기존 최대 전진 거리 표시 유지 |
| Distance Score | Runtime `CurrentScore` → `_scoreText` → `FormatDistanceScore` | Base+Bonus의 포화 합 표시로 연결 |
| Collectible Score | 공통 Collectible Runtime → `_infiniteCollectibleScoreText` | 배율 미적용 값 유지 |
| Total Score | UI에서 `ScoreRecord.TryCalculateTotalScore` 호출 | Runtime 공개 합계와 동일 값 사용하도록 정리 |
| Base·Bonus | 생산 Runtime/Formatter/HUD 경로 없음 | Runtime 값, Formatter, Text 참조 추가 |
| 현재 배율·Fill·Gradient | 생산 UI 경로 없음 | 별도 Momentum HUD 및 순수 표시 검증 추가 |
| Result | `SetResultData → TryFormatInfiniteResult`에서 거리·Distance·Collectible·Total 표시 | 기존 필드 유지, Base·Bonus·최고 배율 추가 |

`UIVisibilityState`는 Playing·Paused·Ending·Ended에서 현재 Mode HUD를 유지하고, Ended + Result UI에서 ResultPanel을 표시한다. 별도 `E_GameState.Result`는 없으며 Result는 `E_UIState`이다. Momentum HUD도 이 기존 상태 표현에 연결한다.

## 5. 저장된 Scene의 현재 참조

`SampleScene.unity:7333`의 UIManagementSystem Component fileID는 `1961406790`, 소유 객체는 `GameRoot/Systems/UIManagementSystem`이다. Script GUID `d1b7b1da0529c9a44baeb8f13332d504`가 실제 Script `.meta`와 일치한다.

| 직렬화 필드 | fileID | 실제 객체 경로 또는 이름 |
|---|---:|---|
| `_infiniteHud` | 264752311 | `UIRoot/InfiniteHUD` |
| `_infiniteResultContent` | 1417628400 | `UIRoot/ResultPanel/Canvas/Panel/InfiniteResultContent` |
| `_distanceText` | 1321891947 | `UIRoot/InfiniteHUD/Canvas/Image/DistanceText` |
| `_scoreText` | 164516341 | 같은 Image 아래 `ScoreText` |
| `_infiniteCollectibleScoreText` | 854532779 | 같은 Image 아래 `InfiniteCollectibleScoreText` |
| `_infiniteTotalScoreText` | 1078575310 | 같은 Image 아래 `InfiniteTotalScoreText` |
| `_infiniteDifficultyText` | 1316098895 | 같은 Image 아래 `InfiniteDifficultyText` |
| `_finalDistanceText` | 2069854255 | InfiniteResultContent의 `Infinite Result Image/FinalDistanceText` |
| `_finalScoreText` | 307487768 | 같은 Image 아래 `FinalScoreText` |
| `_infiniteResultCollectibleScoreText` | 59012279 | 같은 Image 아래 `InfiniteResultCollectibleScoreText` |
| `_infiniteResultTotalScoreText` | 2102303984 | 같은 Image 아래 `InfiniteResultTotalScoreText` |

- UIManagementSystem의 현재 객체 참조 24개 모두 Scene 내 fileID로 해석된다. Root 참조는 GameObject, Text 참조는 동일 TMP Script GUID `f4688fdb7df04437aeb418b961361dc5`, Button 참조는 `4e29b1a8efbd4b44bb3f3716e73f07ff`를 가리킨다.
- 실제 구조는 `UIRoot` 아래 StageHUD·InfiniteHUD·PausePanel·ResultPanel이 각각 자신의 Canvas를 갖는다. 공통 HUD Canvas 아래에 InfiniteHUD가 놓인 구조가 아니다.
- 네 Canvas의 저장된 Render Mode는 `0`, Sorting Order는 `0`이다. 후속 Momentum Canvas를 추가할 때 Pause·Result의 전면 표시를 함께 유지해야 한다.
- 새 MomentumHUD, 배율 Text, Fill과 Base·Bonus·최고 배율 Text 이름은 Scene에 없고, 이를 받을 UIManagementSystem 필드도 없다.
- MomentumLandingFeature의 기존 속도 설정은 `SampleScene.unity:3146`에 저장되어 있다. 코드 필드 제거와 Scene 저장 내용 정리는 별도 단계이며 AI가 Scene YAML을 직접 편집하지 않는다.

## 6. 후속 Scene 구성 대상과 방법

**지금 수행할 수동 작업은 없다. 아래 구성은 Step 4 구현과 Step 5 사용자 Compile·Edit Mode Test 통과 후 Step 6에서 수행한다.**

현 구조를 유지하는 구성안은 `UIRoot/MomentumHUD/Canvas/Panel`이다. MomentumHUD를 InfiniteHUD의 형제인 독립 활성화 Root로 두고, 별도 Canvas 아래 Panel의 RectTransform Anchor Min/Max와 Pivot을 `(1, 0)`으로 설정한다. 일반 Transform인 기존 HUD Root와 화면 배치용 RectTransform을 구분한다. 이는 계획서의 공통 HUD Canvas 가정을 실제 Scene 구조에 맞춘 후속 안내이며 아직 생성된 객체가 아니다.

| 추가 대상 | Component/값 | UIManagementSystem에 추가할 참조 역할 |
|---|---|---|
| `UIRoot/MomentumHUD` | GameObject, 자식 Canvas·Panel | 독립 Momentum HUD Root |
| Panel 아래 `MomentumMultiplierText` | TextMeshProUGUI, `x1.00` | 현재 배율 TMP_Text |
| Panel 아래 Bar 배경과 `MomentumDurationFill` | Image, Fill은 Horizontal/Left, 초기 0, 흰색 | 유지 시간 Fill Image |
| Gradient | 0 빨강, 0.10 주황, 0.30 노랑, 0.60 초록, 1 청록 | 직렬화 Gradient 값. 객체 참조가 아님 |
| `InfiniteHUD/Canvas/Image/BaseDistanceScoreText` | TextMeshProUGUI | HUD Base Distance Score |
| 같은 Image 아래 `MomentumBonusText` | TextMeshProUGUI | HUD Momentum Bonus |
| `InfiniteResultContent/Infinite Result Image/InfiniteResultBaseDistanceScoreText` | TextMeshProUGUI | Result Base Distance Score |
| 같은 Image 아래 `InfiniteResultMomentumBonusText` | TextMeshProUGUI | Result Momentum Bonus |
| 같은 Image 아래 `InfiniteResultMaximumMultiplierText` | TextMeshProUGUI | Result 최고 배율 |

새 연결 대상은 GameObject 1개, TMP_Text 6개, Image 1개와 Gradient 설정 1개이다. 정확한 C# 필드명은 아직 존재하지 않으므로 Step 4 구현 결과로 확정한다. 기존 Distance·Collectible·Total·Difficulty 참조는 유지한다. Step 6에서는 위 객체 생성, 텍스트 행 배치, Filled Image용 Sprite 지정, 비조작 UI의 Raycast Target 해제, Pause·Result보다 뒤쪽 표시, Inspector 연결 후 저장 순으로 사용자에게 안내한다. Score 배율·유지 시간 정책은 Inspector에 중복 입력하지 않는다.

## 7. 기존 Test 영향

- 변경 대상: `MomentumLandingFeatureTests.TryCompleteLanding_BufferedInput_AppliesSignedMultiplier`, `TryCompleteLanding_ResultSpeed_DoesNotExceedMaximum`, `MomentumLandingIntegrationTests`의 성공 후 속도 8 초과·14 이하 Assertion은 기존 속도 보상 계약이다. 성공 판정 Test를 유지하면서 속도 불변 계약으로 갱신해야 한다.
- 순수 모델 재사용: `MomentumScoreStateTests`, `InfiniteScoreStateTests`, `ScoringVersionTests`. 기존 Version 1 초기화·생성자 호환 Test를 일괄 Version 2로 바꾸지 않는다.
- 데이터·결과·표시 영향: `GameRuntimeDataTests`, `InfiniteModeRuntimeDataTests`, `ScoreRecordTests`, `ResultDataTests`, `ResultSystemTests`, `ResultTextFormatterTests`, `UIVisibilityStateTests`.
- 생산 연결 회귀: `InfiniteModeSystemTests`, `InfiniteModeIntegrationTests`, `InfiniteHudIntegrationTests`, `ModeResultDisplayIntegrationTests`, `GamePauseOrchestrationTests`, `ResultMenuIntegrationTests`, `CollectibleLifecycleIntegrationTests`.
- 입력 정책 정리 시 추가 영향: `PlayerMovementMathTests`, `AutoMovementIntegrationTests` 및 PlayerInputSystem 문서. 현재 자동 이동을 검증하는 Test를 새 정책 결정 없이 삭제하거나 통과 조건을 완화하지 않는다.
- 후속 추가 검증: 착지 성공 ID 한 번 전달, 성공 전 거리 소급 배율 금지, 성공·만료 동시 처리, Stage 배율 미적용, Version 일치·최종 확정·최종 HUD 일치, 실제 Scene 새 참조·표시 상태.

## 8. 변경 범위 경계

- Step 2~4의 주요 생산 변경 후보: MomentumLandingFeature, PlayerMovementSystem, PlayerMovementRuntimeData, InfiniteModeSystem, GameRuntimeData, InfiniteModeRuntimeData, GameSystem, ResultSystem, ResultTextFormatter, UIManagementSystem, UIVisibilityState 및 관련 Test·문서.
- RuntimeDataSystem은 현재 생성 요청을 GameRuntimeData로 위임한다. 새 데이터 구조 때문에 반드시 직접 수정해야 하는 것은 아니다.
- ScoreRecord·ResultData의 Version 2 API와 MomentumScoreState·InfiniteScoreState는 기존 계약을 우선 재사용한다.
- World Rebase 실행, 누적 Offset 생산 연결, Player·Camera Warp, Pattern·Boundary 이동·재사용·선택 알고리즘 변경은 이 Step 및 Phase 2 생산 연결 범위 밖이다. Difficulty·Pattern은 기존 최대 전진 거리를 계속 사용한다.
- 공통 종료·입력·UI 변경에 대한 Stage·Pattern·Collectible 회귀 확인은 필요하다. 직접 수정하지 않는다는 이유만으로 회귀 영향이 없다고 단정하지 않는다.

---

# 작업 내용

- 생산 호출 경로와 문서 계약을 대조하고 변경 지점을 기록했다.
- Scene YAML을 읽기 전용으로 파싱하여 기존 UI 참조, 계층과 새 객체 부재를 확인했다.
- 후속 UI 객체·참조 역할과 Scene 구성 방법을 정리했다.
- Phase 2 계획의 Step 1 체크리스트와 검증 결과·후속 시작 지점을 갱신했다.

---

# 영향 범위

Tasks 문서만 변경했다. 생산 코드, Test 코드, Scene, Prefab과 ProjectSettings는 변경하지 않았다.

---

# 검증 내용

- Script 메서드·호출부, 기존 Test Assertion, Scene의 Script GUID·객체 fileID·계층을 정적으로 대조했다.
- 기존 UI 참조 24개의 Scene 대상 존재를 확인했다.
- `git diff --check`에서 공백 오류가 없었고, 신규 기록도 줄 끝 공백이 없음을 별도로 확인했다. 변경 목록은 Tasks 문서 2개뿐이다.
- Unity Editor, Compilation, Build와 Unity Test Runner는 실행하지 않는다.

---

# 검증 결과

- Step 1의 속도 효과 제거·Score 생산 연결 지점, Version 1→2 전환 지점, 추가 Scene UI 객체·참조 역할 목록을 확인했다.
- 자동 이동·Move 비활성 계약과 계획서의 공통 HUD Canvas 가정 및 실제 Scene 구조의 차이를 기록했다.
- 구현·런타임 검증은 미수행이다. Phase 2 전체 완료를 의미하지 않는다.

---

# 후속 작업

- Step 2: 속도 효과 제거와 생산 Momentum 상태를 연결하고 기존 자동 이동 계약을 유지한다.
- Step 3~4: Version 2 Score·Result·UI 연결과 Test 구현, 정확한 Inspector 필드명 확정.
- Step 5~8: 사용자가 Unity Compile·Test Runner, Scene 구성과 최소 화면 확인을 수행한다. 정적으로 확인 가능한 사항은 AI가 검사한다.
- 현재 Step 1에 필요한 사용자 수동 작업은 없다. Build는 이번 단계의 완료 조건이 아니며 AI가 시도하지 않는다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`, `ARCHITECTURE.md`, `PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`, `INVESTIGATION_RULE.md`, `IMPLEMENTATION_RULE.md`, `VERIFICATION_RULE.md`
- `AI/02_Systems/PlayerMovementSystem.md`, `PlayerInputSystem.md`, `InfiniteModeSystem.md`, `RuntimeDataSystem.md`, `ResultSystem.md`, `UIManagementSystem.md`
- `AI/03_Features/MomentumLanding.md`, `InfiniteMode.md`, `ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `20260916_04_Phase2ManualSteps.md`
- `20260916_03_Phase1VerificationResult.md`

---

# 작성 완료 기준

- 정적 조사 근거와 후속 구현 제안을 구분했다.
- Step 1 완료와 Phase 2 구현·검증 미수행을 구분했다.
- 사용자 작업 시점과 AI의 Scene 편집·Unity 실행 미수행 범위를 명시했다.
