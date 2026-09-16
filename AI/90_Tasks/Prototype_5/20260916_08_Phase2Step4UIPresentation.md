# 작업 정보

## 작업명

Prototype 5 Phase 2 Step 4 UI 표시 생산 연결

## 작업 일자

20260916

## 작업 담당자

AI

---

# 작업 목적

Scoring Version `2`의 Score 구성 요소와 Momentum 상태를 Scene과 독립적인 표시 계약으로 만들고 `UIManagementSystem`의 HUD·Result 경로에 연결한다.

---

# 작업 내용

## 표시 모델과 Formatter

- `MomentumHudPresenter`를 추가하여 `x1.00` 형식의 배율 문자열과 `0..1` Fill 제한을 담당하게 했다.
- 기본 배율 `1.00`은 항상 빈 Bar로 표시한다.
- 승인 Gradient는 Blend Mode, 불투명 Alpha와 다음 Color Key를 사용한다. 이 Gradient는 시간 비율에 따라 단색을 선택하는 용도가 아니라 Bar 전체의 좌측에서 우측으로 그리는 고정 수평 색상이다.

| Fill | 색상 | RGB |
|---:|---|---|
| `0.00` | 빨강 | `(1, 0, 0)` |
| `0.10` | 주황 | `(1, 0.5, 0)` |
| `0.30` | 노랑 | `(1, 1, 0)` |
| `0.60` | 초록 | `(0, 1, 0)` |
| `1.00` | 청록 | `(0, 1, 1)` |

- `ResultTextFormatter`에 Base Distance Score, Momentum Bonus와 최고 Momentum 배율 Formatter를 추가했다.
- Version `2` Infinite Result Formatter는 최종 거리, Base·Bonus·Distance·Collectible·Total Score와 최고 배율을 한 번에 반환한다.
- 기존 Version `1` Result Formatter는 호환 경로로 유지했다.

## UIManagementSystem

- InfiniteHUD는 Runtime Data의 Base Distance Score, Momentum Bonus, Distance Score, Collectible Score와 Total Score를 같은 Snapshot에서 표시한다.
- Infinite Result는 Version `2` 전체 구성 요소와 최고 배율을 표시한다.
- Momentum HUD는 InfiniteMode HUD와 같은 표시 생명주기를 사용한다. Playing에서 갱신하고 Paused, Ending, Result와 Ended에서는 마지막 값을 유지한다.
- `MomentumGradientEffect`가 Fill Image Mesh의 각 Vertex를 Bar 전체 좌표에 따라 평가하여 좌측 빨강부터 우측 청록까지 색을 보간한다. 시간 경과는 Image Fill 길이만 변경한다.
- Retry와 새 Run에서는 `x1.00`과 빈 Bar로 초기화한다.
- Momentum HUD 필수 참조 또는 Gradient가 잘못되면 초기화 시 한 번 Warning을 기록하고 Root를 비활성화한다. 매 프레임 Error·Warning을 반복하지 않는다.

---

# Test

- `MomentumHudPresentationTests`에 배율 문자열, Fill 제한, 무효값 Fallback, Gradient 기준점·보간 및 변경된 기준점 거부 사례를 추가했다.
- `ResultTextFormatterTests`에 새 Score 문자열, 최고 배율과 Version `2` 전체 Result 표시 계약을 추가했다.
- `InfiniteHudIntegrationTests`의 코드 생성 UI에 새 참조를 추가하고 Run 시작, Momentum 갱신, Pause·Result 보존과 Runtime Score Snapshot 표시를 검증하도록 갱신했다.
- AI는 Unity Test Runner를 실행하지 않았으므로 Test Passed로 판정하지 않는다.

---

# 사용자 Scene 작업

Step 4 자체에는 Scene 작업이 없다. Step 5의 Compile·Edit Mode Test 성공 후 Step 6에서 `SampleScene`을 다음과 같이 편집한다.

## InfiniteHUD 추가 Text

1. Base Distance Score Text를 만들고 `UIManagementSystem._baseDistanceScoreText`에 연결한다.
2. Momentum Bonus Text를 만들고 `UIManagementSystem._momentumBonusText`에 연결한다.
3. 기존 Distance Score Text는 `UIManagementSystem._scoreText` 연결을 유지한다.

## InfiniteResultContent 추가 Text

1. Base Distance Score Text를 만들고 `_infiniteResultBaseDistanceScoreText`에 연결한다.
2. Momentum Bonus Text를 만들고 `_infiniteResultMomentumBonusText`에 연결한다.
3. 최고 Momentum 배율 Text를 만들고 `_infiniteResultMaximumMomentumText`에 연결한다.
4. 기존 Final Distance, Distance Score, Collectible Score와 Total Score 연결을 유지한다.

## 독립 Momentum HUD

1. Canvas 아래 기존 InfiniteHUD와 별도의 `MomentumHUD` Root를 만들고 화면 우측 하단에 배치한다.
2. Root를 `_momentumHud`에 연결한다.
3. 배율 TMP Text를 만들고 `_momentumMultiplierText`에 연결한다.
4. Background Image 아래 Filled Image를 만들고 Image Type을 `Filled`, Fill Method를 `Horizontal`, Fill Origin을 `Left`로 설정한다.
5. Filled Image에 `MomentumGradientEffect` Component를 추가한 뒤 Image를 `_momentumDurationFillImage`에 연결한다.
6. `_momentumDurationGradient`에 위 표의 Color Key와 Alpha Key `0.00/1.00 = 1`을 입력하고 Mode를 `Blend`로 설정한다.

Scene 작업은 Step 6에서 수행하고 저장한다. AI는 Scene을 수정하지 않는다.

---

# 정적 검증 결과

- 신규 C#과 `.meta`의 경로·GUID 및 참조 이름을 확인했다.
- Formatter Overload 호출부와 Runtime Data 표시 입력을 대조했다.
- Scene, Prefab과 ProjectSettings는 변경하지 않았다.
- Unity Script Compilation, Build와 Unity Test Runner는 실행하지 않았다.

## 사용자 Test 결과와 수정

- 최초 실행에서 Edit Mode `ApprovedGradient_UsesContractKeysAndContinuousEvaluation`가 Unity 색 공간의 Gradient 중간 보간값을 정확히 `1.0`으로 가정하여 실패했다. 사용자 설명에 따라 Bar 전체가 좌측 빨강부터 우측 청록까지 고정 Gradient를 사용하고 중간 영역을 보간하는 계약으로 명확히 수정했다.
- 최초 실행에서 Play Mode `MomentumHud_UpdatesAndFreezesAcrossPauseAndResult`가 남은 시간 `1.0`, 전체 시간 `10.0`을 입력하면서 Fill `1.0`을 기대하여 실패했다. 성공 직후 계약에 맞게 `10.0 / 10.0`을 입력하도록 수정했다.
- 두 실패는 Scene 참조 누락이나 생산 코드 결함이 아니며 Scene 작업 없이 수정했다.
- 최초 구현처럼 Fill 비율로 Image 단색을 바꾸는 처리는 제거하고, `MomentumGradientEffect`가 Bar Mesh 자체에 수평 Gradient를 그리도록 수정했다.
- 사용자가 수정 후 다음 두 Test의 성공을 확인했다.
  - Edit Mode `ApprovedGradient_UsesHorizontalContractKeysAndInterpolation`
  - Play Mode `MomentumHud_UpdatesAndFreezesAcrossPauseAndResult`
- 전체 Step 5 회귀 Test 결과는 아직 제공되지 않았다.

---

# 후속 작업

- Step 5에서 사용자가 Unity Script Compilation과 전체 Edit Mode Test를 실행한다.
- Step 6에서 사용자가 위 Inspector 필드와 UI 객체를 Scene에 구성한다.
- Step 6 Scene 저장 후 AI가 Scene YAML 참조와 Gradient 값을 정적으로 검사한다.
