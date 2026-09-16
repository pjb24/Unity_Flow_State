# 작업 정보

## 작업명

Prototype 5 Phase 2 Step 6 생산 Scene UI 구성 체크리스트

## 작업 일자

20260916

## 작업 담당자

AI 조사 및 사용자 Scene 편집

---

# 현재 Scene 조사 결과

생산 Scene은 `Assets/Scenes/SampleScene.unity`이다.

- HUD Root는 `UIRoot` 아래 `StageHUD`, `InfiniteHUD`, `ResultPanel`, `PausePanel`로 구성되어 있다.
- 기존 Infinite HUD 계층은 `UIRoot/InfiniteHUD/Canvas/Image`이다.
- `Image`는 우측 상단 Anchor·Pivot `(1,1)`, 크기 `400×250`이며 높이 `50`인 Text 5개를 Vertical Layout으로 배치한다.
- 현재 행 순서는 Distance, Distance Score, Collectible Score, Total Score, Difficulty이다.
- Infinite Result 계층은 `ResultPanel/Canvas/Panel/InfiniteResultContent/Infinite Result Image`이다.
- `Infinite Result Image`는 크기 `400×200`이며 높이 `50`인 Text 4개를 Vertical Layout으로 배치한다.
- 현재 행 순서는 Final Distance, Distance Score, Collectible Score, Total Score이다.
- `UIManagementSystem`은 `GameRoot/Systems/UIManagementSystem`에 있다.

---

# 사용자 Scene 편집 절차

## 1. InfiniteHUD Score 행

`UIRoot/InfiniteHUD/Canvas/Image`에서 다음 순서가 되도록 구성한다.

1. `DistanceText` — 기존 유지
2. `BaseDistanceScoreText` — 신규
3. `MomentumBonusText` — 신규
4. `ScoreText` — 기존 Distance Score
5. `InfiniteCollectibleScoreText` — 기존
6. `InfiniteTotalScoreText` — 기존
7. `InfiniteDifficultyText` — 기존

신규 Text는 기존 Text를 복제하여 TextMeshProUGUI, Font Asset, Material, Font Size, Alignment와 색상을 동일하게 유지한다.

| 항목 | 값 |
|---|---|
| 각 Text Size Delta | `400×50` |
| `BaseDistanceScoreText` 초기 문자열 | `Base Distance Score: --` |
| `MomentumBonusText` 초기 문자열 | `Momentum Bonus: --` |
| 부모 `Image` Size Delta | `400×350` |
| 부모 Anchor Min / Max | `(1,1)` / `(1,1)` 유지 |
| 부모 Pivot | `(1,1)` 유지 |

`UIManagementSystem` 연결:

- `_baseDistanceScoreText` → `BaseDistanceScoreText`의 TextMeshProUGUI
- `_momentumBonusText` → `MomentumBonusText`의 TextMeshProUGUI

## 2. Infinite Result 행

`ResultPanel/Canvas/Panel/InfiniteResultContent/Infinite Result Image`에서 다음 순서가 되도록 구성한다.

1. `FinalDistanceText` — 기존
2. `InfiniteResultBaseDistanceScoreText` — 신규
3. `InfiniteResultMomentumBonusText` — 신규
4. `FinalScoreText` — 기존 Distance Score
5. `InfiniteResultCollectibleScoreText` — 기존
6. `InfiniteResultTotalScoreText` — 기존
7. `InfiniteResultMaximumMomentumText` — 신규

신규 Text는 기존 Result Text를 복제하여 스타일을 동일하게 유지한다.

| 항목 | 값 |
|---|---|
| 각 Text Size Delta | `400×50` |
| Base 초기 문자열 | `Base Distance Score: --` |
| Bonus 초기 문자열 | `Momentum Bonus: --` |
| 최고 배율 초기 문자열 | `Max Momentum: x--` |
| `Infinite Result Image` Size Delta | `400×350` |
| Anchor Min / Max | `(0.5,0.5)` / `(0.5,0.5)` 유지 |
| Pivot | `(0.5,0.5)` 유지 |

`UIManagementSystem` 연결:

- `_infiniteResultBaseDistanceScoreText` → `InfiniteResultBaseDistanceScoreText`의 TextMeshProUGUI
- `_infiniteResultMomentumBonusText` → `InfiniteResultMomentumBonusText`의 TextMeshProUGUI
- `_infiniteResultMaximumMomentumText` → `InfiniteResultMaximumMomentumText`의 TextMeshProUGUI

## 3. 독립 MomentumHUD

`UIRoot/InfiniteHUD`를 복제하면 기존 HUD와 같은 Canvas 설정을 쉽게 유지할 수 있다. 복제한 Root를 `MomentumHUD`로 이름을 바꾸고 Canvas 아래 기존 Score Panel을 제거한 뒤 다음 구조를 만든다.

```text
UIRoot
└─ MomentumHUD
   └─ Canvas
      └─ MomentumPanel
         ├─ MomentumMultiplierText
         └─ MomentumDurationBackground
            └─ MomentumDurationFill
```

`MomentumPanel` 권장 RectTransform:

| 항목 | 값 |
|---|---|
| Anchor Min / Max | `(1,0)` / `(1,0)` |
| Pivot | `(1,0)` |
| Anchored Position | `(-40,40)` |
| Size Delta | `(320,110)` |

위치는 화면 우측·하단에서 각각 40px 여백을 둔 시작값이다. 최종 가독성과 겹침은 Step 8에서 확인한다.

`MomentumMultiplierText`:

| 항목 | 값 |
|---|---|
| Component | TextMeshProUGUI |
| 초기 문자열 | `x1.00` |
| Anchor | Panel 상단 Stretch 또는 상단 중앙 |
| 높이 | `40` |

`MomentumDurationBackground`:

| 항목 | 값 |
|---|---|
| Component | Image |
| Anchor | Panel 하단 가로 Stretch |
| 좌우 여백 | 각 `20` |
| 하단 여백 | `16` |
| 높이 | `24` |

`MomentumDurationFill`:

| 항목 | 값 |
|---|---|
| Component | Image + `MomentumGradientEffect` |
| RectTransform | Background 전체 Stretch, Offset 모두 `0` |
| Image Type | `Filled` |
| Fill Method | `Horizontal` |
| Fill Origin | `Left` |
| Fill Amount | `0` |
| Color | White `(1,1,1,1)` |
| Raycast Target | Off |

`UIManagementSystem` 연결:

- `_momentumHud` → `MomentumHUD` Root GameObject
- `_momentumMultiplierText` → `MomentumMultiplierText`의 TextMeshProUGUI
- `_momentumDurationFillImage` → `MomentumDurationFill`의 Image

## 4. Momentum Gradient

`UIManagementSystem._momentumDurationGradient`를 다음과 같이 설정한다.

- Mode: `Blend`
- Alpha Key: Time `0.00`, Alpha `1`; Time `1.00`, Alpha `1`

| Time | Color | RGB |
|---:|---|---|
| `0.00` | Red | `(1,0,0)` |
| `0.10` | Orange | `(1,0.5,0)` |
| `0.30` | Yellow | `(1,1,0)` |
| `0.60` | Green | `(0,1,0)` |
| `1.00` | Cyan | `(0,1,1)` |

Gradient는 시간에 따라 단색으로 바뀌는 값이 아니다. `MomentumGradientEffect`가 Bar 전체 좌측에서 우측으로 이 Gradient를 그리며, Runtime은 남은 시간에 따라 Fill 길이만 변경한다.

---

# 저장 전 확인

- 기존 Text와 UI 객체를 삭제하지 않았다.
- 신규 Text 5개 이름이 정확하다.
- Momentum HUD 객체 4개 이름과 계층이 정확하다.
- `MomentumDurationFill`에 Image와 `MomentumGradientEffect`가 함께 있다.
- `UIManagementSystem`의 신규 직렬화 필드 9개가 모두 연결됐다.
- Gradient의 Color Key 5개, Alpha Key 2개와 Blend Mode가 정확하다.
- `MomentumHUD`는 `InfiniteHUD`의 자식이 아니라 `UIRoot`의 형제 Root이다.
- Scene을 저장했다.

---

# AI 후속 정적 검사

사용자가 Scene 저장 완료를 알리면 AI가 YAML에서 다음을 확인한다.

- 객체 이름의 누락과 중복
- 부모·자식 계층과 우측 하단 Anchor·Pivot
- TMP Text, Image와 `MomentumGradientEffect` Component
- Filled, Horizontal, Left, Fill Amount `0`, White 색상
- Gradient Mode, Color·Alpha Key
- `UIManagementSystem` 신규 참조 9개의 non-null 연결과 대상 Component 일치
- 기존 HUD·Result 참조 유지

AI는 Scene을 수정하지 않는다.

---

# 최초 저장 Scene 정적 검사

확인 완료:

- 신규 객체 9개가 각각 한 번 존재한다.
- `MomentumHUD`가 `UIRoot` 아래에서 `InfiniteHUD`와 형제 Root로 존재한다.
- 신규 직렬화 필드 9개가 모두 non-null 참조로 연결됐다.
- `MomentumDurationFill`에 Image와 `MomentumGradientEffect`가 있다.
- Fill은 White, Filled, Horizontal, Left, Amount `0`이다.
- Gradient는 Blend Mode, Color Key 5개와 Alpha Key 2개를 사용한다.
- `MomentumDurationBackground` Alpha `0`은 자식 Fill 표시에 영향을 주지 않으므로 허용한다.
- InfiniteHUD와 Infinite Result 부모 높이가 각각 `350`으로 확장됐다.

최초 검사에서 확인된 사용자 수정 항목:

1. Momentum 배율 영역과 Bar가 우측·하단 화면 경계에 바로 붙어 있다. 두 RectTransform의 Anchored Position X를 `-40`으로 옮기고, Bar는 Y를 `40`, 배율 영역은 Y를 `140` 정도로 옮겨 여백을 확보한다.
2. `MomentumDurationFill`의 Raycast Target을 끈다.
3. InfiniteHUD의 자식 순서를 `DistanceText`, `BaseDistanceScoreText`, `MomentumBonusText`, `ScoreText`, `InfiniteCollectibleScoreText`, `InfiniteTotalScoreText`, `InfiniteDifficultyText`로 변경한다.
4. Infinite Result의 자식 순서를 `FinalDistanceText`, `InfiniteResultBaseDistanceScoreText`, `InfiniteResultMomentumBonusText`, `FinalScoreText`, `InfiniteResultCollectibleScoreText`, `InfiniteResultTotalScoreText`, `InfiniteResultMaximumMomentumText`로 변경한다.

정적 검사 중 주황색 `#FF8000`의 Unity 값 `0.5019608`과 승인 코드의 기존 `0.5`가 불일치하는 문제를 발견했다. Scene의 올바른 `#FF8000` 값은 유지하고 승인 코드와 Test 기대값을 `128/255`로 수정했다. 이 코드 변경 후 Script Compilation과 해당 Edit Mode Test 재확인이 필요하다.

## 수정 후 최종 검사

- 배율 영역 Anchored Position `(-40,140)`과 Bar Anchored Position `(-40,40)`을 확인했다.
- `MomentumDurationFill`의 Raycast Target 비활성화를 확인했다.
- InfiniteHUD 7개 행이 Distance, Base, Bonus, Distance Score, Collectible, Total, Difficulty 순서임을 확인했다.
- Infinite Result 7개 행이 Final Distance, Base, Bonus, Distance Score, Collectible, Total, Maximum Momentum 순서임을 확인했다.
- 신규 참조 9개가 모두 유지되고 기존 참조가 변경되지 않았음을 확인했다.
- 사용자가 `ApprovedGradient_UsesHorizontalContractKeysAndInterpolation` 성공을 확인했다.
- Step 6 Scene YAML 정적 검사 완료 조건을 충족했다.
