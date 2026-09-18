# 작업 정보

## 작업명

프로젝트 Inspector 직렬화 값 Runtime 적용 감사

---

## 작업 일자

20260916

---

## 작업 담당자

AI

---

## 작업 상태

완료

---

# 작업 목적

프로젝트의 custom Runtime Component가 Inspector에 노출하는 직렬화 값이 실제 Runtime 동작에 전달되는지 확인한다. 선언만 남은 필드, Runtime 상수로 무조건 대체되는 필드, 조건에 따라 사용되지 않는 필드와 의도된 유효성 보정을 구분한다.

---

# 조사 범위

- `Assets/Scripts/Runtime`의 `[SerializeField]` 121개
- `[SerializeField]`를 선언한 Runtime C# 17개
- `Assets/Scenes`의 Scene YAML
- `Assets/Prefabs`의 Prefab YAML
- Inspector 값이 전달되는 초기화, FixedUpdate, UI 갱신, 물리 판정과 Pattern 생성 경로
- Runtime에서 수행하는 Clamp, 유효성 검사, 비활성화와 Fallback

Unity 기본 Component의 내부 직렬화 필드는 Unity Engine이 소비하므로 이번 custom 코드 감사 대상에서 제외했다. TMP Text의 초기 문자열처럼 Runtime 상태 표시 과정에서 의도적으로 갱신되는 표현 값도 custom `[SerializeField]` 미사용으로 분류하지 않았다.

---

# 조사 방법

1. 전체 Runtime C#에서 `[SerializeField]` 선언을 추출했다.
2. 각 필드의 같은 파일 내 읽기와 Runtime 재할당 여부를 확인했다.
3. 수치 필드가 계산 또는 상태 객체 초기화까지 전달되는지 추적했다.
4. 참조 필드가 유효성 검사 후 실제 Component, Transform, Collider, UI 또는 System 호출에 사용되는지 추적했다.
5. Scene과 Prefab YAML의 custom 필드명이 현재 C# 선언에 존재하는지 대조했다.
6. Runtime 상수, `Mathf.Max`, 조건부 컴파일과 이중 구현 경로가 Inspector 값을 대체하는지 확인했다.

Unity Editor, Unity Test Runner와 Build는 실행하지 않았다.

---

# 결론

- 선언만 존재하고 Runtime에서 전혀 읽히지 않는 `[SerializeField]`는 발견하지 않았다.
- 유효한 Inspector 값을 Runtime 상수나 기본 Gradient로 무조건 교체하는 경로는 발견하지 않았다.
- Scene과 Prefab YAML에서 현재 C# 선언에 없는 custom `_fieldName` 직렬화 항목은 발견하지 않았다.
- 생산 `SampleScene`의 Momentum Gradient는 `UIManagementSystem._momentumDurationGradient`에서 `MomentumGradientEffect.SetGradient`로 직접 전달된다.
- Inspector 값이 조건에 따라 사용되지 않는 두 설계 경로는 의도된 동작이다.
- Component 누락 시 Inspector Gradient만 조용히 적용되지 않을 수 있는 잠재 구성 오류 한 건을 발견했다. 현재 생산 Scene 구성에는 해당 문제가 없다.

---

# 확인 결과

## 1. Runtime 수치 설정

| Component | Inspector 값 | Runtime 적용 경로 | 판정 |
|---|---|---|---|
| `GameSystem` | `_selectedGameMode` | Runtime Data, Stage, InfiniteMode와 Timer 초기화 | 적용됨 |
| `PlayerMovementSystem` | 이동 속도, 지상·공중 가속도, 최대 수평 속도, 중력 | `PlayerMovementMath`와 Jump 계산 | 적용됨 |
| `JumpFeature` | Jump 높이, Coyote Time | 수직 속도 계산과 Coyote 상태 | 적용됨 |
| `MomentumLandingFeature` | Momentum Landing Window | 착지 예상 시간과 입력 Window 비교 | 적용됨 |
| `CollisionSystem` | Ground 반경, 접지 거리, 예측 거리와 LayerMask | SphereCast와 충돌 필터 | 적용됨 |
| `CameraSystem` | Orthographic Size | Cinemachine Lens 초기화 | 적용됨 |
| `StageSystem` | Fall Threshold Y | Stage 추락 판정 | 적용됨 |
| `InfiniteModeSystem` | Fall Threshold, 최소 속도, 시작·저속·Wall 유예 시간, Score 비율 | Run 상태, 추락, 거리 Score 초기화 | 적용됨 |
| `UIManagementSystem` | Development Difficulty 표시, Momentum Gradient | UI 활성 상태와 Gradient Mesh | 조건부 적용됨 |
| `ScoreCollectible` | Player LayerMask | Player Collider 수집 필터 | 적용됨 |

음수 또는 비정상 수치에 대한 Clamp와 거부는 Inspector 값을 다른 정상 기본값으로 대체하는 Fallback이 아니다. 다음과 같이 명시적인 유효 범위 처리로 사용된다.

- `CameraSystem._orthographicSize`는 최소 `0.01`로 제한한다.
- `CollisionSystem._groundCheckRadius`와 `_groundedDistance`는 최소 `0`으로 제한하고 예측 거리는 접지 거리보다 작지 않게 한다.
- `JumpFeature._coyoteTime`과 `MomentumLandingFeature._momentumLandingWindow`는 최소 `0`으로 제한한다.
- `StageSystem`과 `InfiniteModeSystem`은 `NaN`, Infinity 또는 상태 모델이 거부하는 설정에서 초기화를 실패시킨다.
- `InfiniteModeSystem._scorePerUnit`은 `InfiniteScoreState.Initialize`에 직접 전달하며 유효하지 않으면 Run 초기화를 실패시킨다.

## 2. Runtime 참조 설정

System, Rigidbody, Transform, Collider, Button, TMP Text, Image, Pattern Prefab과 Pattern Authoring 참조는 모두 다음 중 하나 이상의 실제 Runtime 동작에 사용된다.

- 필수 참조 유효성 검사
- System 초기화 또는 상태 전환 호출
- 물리 위치·속도·충돌 판정
- Scene Root 및 HUD 활성화
- UI Text와 Fill 갱신
- Pattern 생성, Anchor 연결, Boundary와 Collectible 구성
- Retry, Pause, Result와 종료 처리

참조 값이 Runtime 시작 시 다른 객체 검색 결과로 덮어써지는 경로는 발견하지 않았다. `StageSystem`이 `InfiniteModeRoot`에서 `InfiniteMapPattern`을 찾는 것은 `_infiniteModeRoot` Inspector 참조를 시작점으로 사용하는 파생 참조이며 Inspector 값을 우회하지 않는다.

## 3. Momentum Gradient

현재 경로는 다음과 같다.

```text
UIManagementSystem._momentumDurationGradient
→ ConfigureMomentumHud()
→ MomentumGradientEffect.SetGradient()
→ ModifyMesh()
→ Gradient.Evaluate(horizontalRatio)
```

- Runtime 기본 Gradient나 승인 색상 상수로 Inspector 값을 교체하지 않는다.
- `MomentumGradientEffect`는 Image의 Inspector Tint와 Gradient 평가 색상을 곱한다.
- 시간 변화는 `Image.fillAmount`에만 반영하며 Gradient Key 자체를 변경하지 않는다.
- 현재 `SampleScene`의 `MomentumDurationFill`에는 `MomentumGradientEffect`가 연결되어 있으므로 Inspector 변경이 Runtime Mesh에 적용된다.

### 잠재 구성 오류

`UIManagementSystem.ConfigureMomentumHud`의 `_isMomentumHudConfigured` 판정은 Momentum HUD Root, TMP Text와 Fill Image만 검사한다. Fill Image에 `MomentumGradientEffect`가 없으면 다음 상태가 된다.

- Momentum HUD는 활성화 가능한 구성으로 판정된다.
- `_momentumDurationGradient`는 어떤 Runtime 대상에도 전달되지 않는다.
- Warning 또는 초기화 실패 없이 단색 Fill이 표시될 수 있다.

현재 생산 Scene에는 Component가 있으므로 실제 결함은 발생하지 않는다. 향후 Scene 복제 또는 Component 제거 시 Inspector Gradient가 반영되지 않는 원인을 발견하기 어렵다는 위험이 있다.

권장 후속 조치는 `_momentumGradientEffect != null`을 Momentum HUD 구성 조건에 포함하고, InfiniteMode에서 누락 시 한 번의 명확한 Warning과 HUD 비활성화를 적용하는 것이다. 이 수정은 별도 코드 변경과 관련 Edit Mode·Play Mode Test 갱신으로 처리한다.

## 4. 의도된 조건부 적용

### Development Difficulty 표시

`UIManagementSystem._showDifficultyInDevelopment`는 `UNITY_EDITOR` 또는 `DEVELOPMENT_BUILD`에서만 사용된다. Release Build에서는 Difficulty Text를 항상 비활성화한다.

필드명과 기능 계약이 Development 전용임을 명시하므로 정상이다. Release Build에서 Inspector 값을 무시하는 동작은 의도된 조건부 컴파일이다.

### InfiniteMapPattern 이중 구성 경로

`InfiniteMapPattern`은 두 구성 방식을 지원한다.

- Slot 경로: `_firstSlot`, `_secondSlot`, `_patternPrefabs`, `_phase2PlayerCollider`
- Legacy 경로: `_firstPattern`, Anchor, Boundary와 두 번째 Pattern 참조

Slot 또는 Pattern Prefab이 하나라도 설정되면 Slot 경로를 선택하며 Legacy 참조는 사용하지 않는다. 반대로 Slot 구성이 전혀 없을 때만 Legacy 참조를 사용한다.

생산 `SampleScene`은 Slot 경로를 사용하므로 Legacy Inspector 값이 Runtime 배치에 영향을 주지 않는 것이 현재 설계상 정상이다. 동일 Component에 두 경로의 필드가 함께 노출되어 어떤 값이 활성 경로인지 Inspector만으로 구분하기 어렵다는 유지보수 부담은 남아 있다.

권장 후속 조치는 향후 Legacy 경로 제거 시 해당 직렬화 필드도 함께 제거하거나 Custom Inspector에서 현재 활성 경로와 비활성 필드를 명확히 표시하는 것이다.

---

# Fallback 및 자동 보정 판정

| 구분 | 대상 | 판정 |
|---|---|---|
| 유효 범위 Clamp | Camera, Collision, Coyote Time, Momentum Window | 정상 |
| 필수 참조 누락 시 초기화 실패 | System, Player, Pattern, Boundary | 정상 |
| 선택 UI 참조 누락 시 해당 표시 생략 | 일부 TMP Text | 기존 정책 |
| Momentum HUD 필수 참조 누락 시 비활성화 | Root, Text, Fill Image | 정상 |
| Momentum Gradient Effect 누락 시 Gradient만 미적용 | Momentum HUD | 보완 권장 |
| Release Build에서 Difficulty 강제 숨김 | Development Difficulty | 정상 |
| Slot 구성 시 Legacy Pattern 참조 미사용 | InfiniteMapPattern | 정상, 구조 정리 후보 |

Runtime에서 Inspector 값을 무시하고 별도의 하드코딩 값으로 정상 동작을 계속하는 일반 Fallback은 발견하지 않았다.

---

# 정적 검증 결과

- Runtime `[SerializeField]`: 121개
- 대상 C# 파일: 17개
- Runtime 읽기 참조가 없는 필드: 0개
- Runtime에서 직렬화 필드 자체를 강제로 재할당하는 경로: 0개
- Scene·Prefab YAML에만 남은 미선언 custom 필드명: 0개
- 유효한 Inspector 값을 하드코딩 기본값으로 무조건 대체하는 경로: 0개
- 의도된 조건부 적용 경로: 2개
- 현재 생산 Scene에서 실제 미적용되는 값: 0개
- 향후 잘못된 Scene 구성에서 미적용될 수 있는 값: 1개 (`_momentumDurationGradient`)

---

# 수동 작업

없음. 이번 감사 결과는 코드와 Scene·Prefab YAML의 정적 검증으로 판정할 수 있다.

`MomentumGradientEffect` 누락 검증을 코드로 보강하는 후속 작업을 수행할 경우 사용자는 Unity Script Compilation과 관련 Edit Mode·Play Mode Test만 실행하면 된다. 현재 생산 Scene의 Gradient를 다시 수동 확인할 필요는 없다.

---

# 후속 작업

1. `UIManagementSystem`의 Momentum HUD 구성 조건에 `MomentumGradientEffect`를 포함한다.
2. Effect 누락 시 Inspector Gradient가 조용히 무시되지 않도록 Warning과 비활성화 동작을 Test로 고정한다.
3. 향후 `InfiniteMapPattern` Legacy 경로 제거 시 비활성 직렬화 필드를 함께 정리한다.

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/90_Tasks/Prototype_5/20260916_08_Phase2Step4UIPresentation.md`
- `AI/90_Tasks/Prototype_5/20260916_10_Phase2Step6SceneChecklist.md`
- `AI/90_Tasks/Prototype_5/20260916_13_Phase2VerificationResult.md`

---

# 작성 완료 기준

- 전체 custom Runtime `[SerializeField]` 선언과 사용 경로를 조사했다.
- 미사용, 조건부 적용, 유효성 보정과 Fallback을 구분했다.
- 생산 Scene의 실제 적용 상태와 잠재 구성 위험을 구분했다.
- 정적으로 판정 가능한 항목을 사용자 수동 작업으로 남기지 않았다.
