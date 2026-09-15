# 작업 정보

## 작업명

Prototype 5 Phase 2 수동 작업 및 검증 계획

---

## 작업 일자

20260916

---

## 작업 담당자

AI, 사용자

---

# 작업 목적

Momentum Landing의 속도 증가 효과를 제거하고 Phase 1에서 확정한 Score 배율 모델을 생산 InfiniteMode, Runtime Data, Result와 UI에 연결한다. 정적으로 판정할 수 있는 구조와 수치 규칙은 AI가 검사하고, 계산·상태·생명주기는 Unit Test 및 통합 Test로 검증한다. 사용자는 Unity Editor가 필요한 Compile·Test 실행, 생산 Scene UI 구성과 최소 화면 확인만 수행한다.

---

# 작업 대상

- `MomentumLandingFeature`, `PlayerMovementSystem`의 속도 효과 제거
- `MomentumScoreState`, `InfiniteScoreState`의 생산 InfiniteMode 연결
- Scoring Version `2`의 Run·Runtime Data·Result 전달
- Base Distance Score, Momentum Bonus, Distance Score, Collectible Score와 Total Score
- Pause·Resume·Result·Retry·새 Run 생명주기
- 기존 InfiniteHUD와 분리된 우측 하단 Momentum HUD
- InfiniteHUD 및 Infinite Result의 새 Score 구성 요소
- 관련 Edit Mode·Play Mode Test와 생산 `SampleScene` UI 구성

World Rebase 생산 연결은 Phase 3 범위이며 이번 작업에 포함하지 않는다.

---

# 작업 전 상태

- Phase 1의 순수 상태·계산 모델과 Unit Test가 구현되어 있다.
- Unity Script Compilation과 전체 Edit Mode Test `559`개가 통과했다.
- 생산 `MomentumLandingFeature`에는 속도 배율 설정과 착지 성공 시 수평 속도 증가 처리가 남아 있다.
- 생산 `InfiniteModeSystem`은 기존 거리 기반 Score와 Scoring Version `1` 경로를 사용한다.
- 생산 Runtime Data, ResultSystem, Formatter와 UIManagementSystem은 새 Score 구성 요소와 Momentum 상태를 완전히 연결하지 않았다.
- 생산 `SampleScene`에는 독립 Momentum HUD와 새 Result 표시 항목 및 참조가 없다.

---

# 작업 원칙

- AI는 코드·문서·Test·Scene YAML을 먼저 정적으로 조사하고 변경 대상과 직렬화 참조를 확정한다.
- Score 계산, 배율 시간, 상태 전환, 중복 처리, 포화, Version과 생명주기는 수동 플레이가 아니라 Edit Mode Unit Test로 판정한다.
- 실제 System 실행 순서, 입력·착지·Pause·Result·Retry와 생산 Scene UI 연결은 Play Mode Test로 판정한다.
- Test는 생산 코드를 호출하며 계산식을 Test 안에 복제하지 않는다.
- AI는 Scene 또는 Prefab을 편집하지 않는다. 생산 Scene 변경은 아래에 명시한 항목만 사용자가 수행한다.
- 사용자가 Scene을 편집하기 전에 AI가 관련 Script와 Test를 먼저 구현하고 정적 검증을 완료한다.
- 사용자는 Inspector의 정책 수치를 임의로 정하지 않는다. 배율 단계와 유지 시간은 Phase 1 순수 모델의 확정값을 사용한다.
- 빠른 입력, 정확한 프레임, Score 숫자 비교와 Timer 만료는 수동 재현을 요구하지 않는다.
- Unity Editor, Unity Test Runner와 Build는 사용자가 직접 실행한다. Phase 2에서는 Build를 완료 조건으로 요구하지 않는다.

---

# 수행 Step

## Step 1. 생산 연결 지점과 Scene 참조를 정적으로 조사한다

### AI 작업

- Momentum Landing 성공에서 수평 속도가 변경되는 생산 호출 경로와 관련 설정·Test를 추적한다.
- InfiniteMode Run 시작, Playing 갱신, Pause·Resume, 종료 확정, Retry·새 Run의 Runtime Data 및 Result 경로를 추적한다.
- 기존 Distance Score와 UI Text가 갱신되는 경로를 새 Score 구성 요소별로 분류한다.
- `SampleScene.unity`를 읽기 전용으로 조사하여 InfiniteHUD, InfiniteResultContent와 UIManagementSystem의 현재 직렬화 참조를 기록한다.
- 변경 대상과 영향받지 않는 World Rebase·Pattern·Camera 범위를 구분한다.

### 사용자 수동 작업

- 없음. 코드와 Scene YAML로 확인할 수 있는 내용은 AI가 처리한다.

### 완료 조건

- [ ] 속도 효과 제거와 Score 생산 연결 지점이 확인됐다.
- [ ] Runtime·Result·UI의 기존 Version `1` 경로와 Version `2` 전환 지점이 확인됐다.
- [ ] 사용자가 추가할 Scene UI 객체와 직렬화 참조 목록이 확정됐다.

---

## Step 2. 속도 효과 제거와 Momentum 생산 상태를 구현한다

### AI 작업

- Momentum Landing 성공이 수평 속도를 증가시키지 않도록 생산 코드를 변경하고 불필요한 속도 배율 설정을 제거한다.
- 착지 성공 ID가 한 번만 Momentum 상태에 전달되도록 연결한다.
- InfiniteMode에서만 Momentum 배율을 갱신하고 Stage Mode 이동에는 Score 배율을 적용하지 않는다.
- 일반 착지와 Wall 접촉은 배율을 초기화하지 않고, 유지 시간 만료만 기본 배율로 초기화하도록 연결한다.
- Pause·Result에서는 Timer를 정지하고 Retry·새 Run에서는 상태를 초기화한다.
- 기존 순수 모델 Test를 유지하면서 생산 연결 경계에 필요한 Edit Mode Unit Test를 추가한다.

### 사용자 수동 작업

- 없음. 코드 작성과 수치·상태 검증은 AI가 처리한다.

### Unit Test 우선 검증

- 성공 전후 수평 속도 입력값과 결과값 동일
- 동일 착지 성공의 중복 반영 거부
- Stage Mode에서 배율 상태 미변경
- 일반 착지·Wall 접촉 상태 보존
- 단계별 만료, Pause·Resume, Result, Retry·새 Run
- 음수·`NaN`·Infinity delta time 거부

### 완료 조건

- [ ] 속도 증가 코드와 생산 설정 의존성이 제거됐다.
- [ ] 생산 Momentum 상태 생명주기가 순수 모델 계약을 사용한다.
- [ ] 계산·상태 규칙을 수동 플레이 없이 Unit Test로 판정할 수 있다.

---

## Step 3. InfiniteMode Score와 Scoring Version `2`를 생산 연결한다

### AI 작업

- 새 InfiniteMode Run을 Scoring Version `2`로 초기화하고 Run 중 변경되지 않도록 한다.
- 최대 전진 거리 증가분, 현재 Momentum 배율과 거리당 점수를 `InfiniteScoreState`에 전달한다.
- Base Distance Score, Momentum Bonus, Distance Score, Collectible Score와 Total Score를 Runtime Data에 분리하여 반영한다.
- 종료 직전에 최종 Score와 최고 배율을 한 번 확정하고 Result Data로 전달한다.
- Runtime·Score·Result Version 불일치와 중복 종료 요청을 거부한다.
- 기존 Version `1` 생성 경로는 호환 API로 유지하되 새 생산 Run에서는 사용하지 않도록 Test로 고정한다.

### 사용자 수동 작업

- 없음. Score 값과 Version 전달은 AI 정적 검사 및 Unit Test로 처리한다.

### Unit Test 우선 검증

- 거리 증가분별 Base와 Momentum Bonus 정밀 누적
- Collectible Score 배율 미적용
- 각 Score 구성 요소와 Total Score의 `int.MaxValue` 포화
- Version `2` Run·Runtime·Result 불변 전달
- Version 없음·미지원·불일치 요청 거부
- 종료 확정 후 변경 거부와 Retry 이후 새 기록 허용

### 완료 조건

- [ ] 새 생산 Run이 Version `2`와 새 Score 모델을 사용한다.
- [ ] Runtime Data와 Result Data의 구성 요소 및 합계가 일치한다.
- [ ] Score 수치와 경계값이 Unit Test로 검증 가능하다.

---

## Step 4. UI 코드와 표시 Formatter를 구현한다

### AI 작업

- InfiniteHUD에 Base Distance Score와 Momentum Bonus를 추가하고 기존 Distance·Collectible·Total 표시와 일치시킨다.
- Infinite Result에 Base Distance Score, Momentum Bonus와 최고 배율을 추가한다.
- 별도 Momentum HUD Root, 배율 Text, 유지 시간 Fill Image와 Gradient를 받을 직렬화 참조를 UIManagementSystem에 추가한다.
- 배율은 `x1.00` 형식, Fill은 `0..1` 제한값으로 표시하고 Gradient 기준점을 확정 계약대로 평가한다.
- Playing, Paused, Ending, Result, Ended, Retry와 새 Run별 표시 상태를 연결한다.
- 문자열, Fill 비율, Gradient 기준점과 UI 상태는 Formatter 또는 순수 표시 모델의 Edit Mode Unit Test로 검증한다.
- 누락된 참조가 정상 플레이 중 반복 Error를 발생시키지 않도록 시작 시 검증과 비활성 폴백을 적용한다.

### 사용자 수동 작업

- 없음. 이 Step에서는 Scene을 편집하지 않는다.

### 완료 조건

- [ ] UI 코드가 Scene과 독립적으로 Compile 가능한 참조 계약을 제공한다.
- [ ] Text·Fill·Gradient·표시 상태가 Edit Mode Unit Test로 검증된다.
- [ ] 사용자가 연결할 정확한 Inspector 필드 목록이 확정됐다.

---

## Step 5. 코드와 Unit Test의 정적 검증 결과를 확인한다

### AI 작업

- 변경된 C#의 namespace, asmdef 참조, 직렬화 필드, `.meta`, GUID, 중복 API와 호출부를 정적으로 검사한다.
- Feature·System 문서와 생산 코드의 Score·Version·생명주기 계약을 대조한다.
- 새 Edit Mode Unit Test와 영향받는 기존 회귀 Test Class를 지정한다.
- `git diff --check`와 Scene 변경 부재를 확인한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료를 확인한다.
2. Console에 예상하지 않은 Compile Error·Warning이 없는지 확인한다.
3. AI가 지정한 새 Edit Mode Unit Test와 영향받는 Edit Mode 회귀 Test를 실행한다.
4. 가능하면 전체 Edit Mode Test를 한 번 실행한다.
5. Tests Run·Passed·Failed 수와 예상하지 않은 Error·Warning 여부를 AI에 전달한다.
6. 실패가 있으면 Test 이름, 메시지와 Stack Trace를 전달한다.

Scene 편집은 Compile과 Edit Mode Test가 통과한 뒤에만 수행한다.

### 완료 조건

- [ ] 정적 검사가 통과했다.
- [ ] Unity Script Compilation이 성공했다.
- [ ] 지정된 Edit Mode Test가 모두 통과했다.
- [ ] 예상하지 않은 Error·Warning이 없다.

---

## Step 6. 생산 Scene에 Momentum HUD와 Result 표시를 구성한다

### AI 작업

- Step 4에서 확정된 필드명과 현재 `SampleScene` 계층을 기준으로 사용자가 연결할 최종 체크리스트를 제공한다.
- 사용자가 저장한 Scene YAML에서 객체, Component, Anchor와 직렬화 참조 누락을 정적으로 검증한다.
- Scene 파일은 직접 수정하지 않는다.

### 사용자 수동 작업

1. `SampleScene`을 열고 작업 전 Scene을 저장한다.
2. HUD Canvas 아래에 기존 `InfiniteHUD`와 형제인 `MomentumHUD` Root를 생성한다.
3. `MomentumHUD`의 Anchor와 Pivot을 우측 하단으로 설정하고 화면 가장자리에서 읽을 수 있는 여백을 둔다.
4. `MomentumHUD` 아래에 현재 배율용 TextMeshProUGUI 객체 `MomentumMultiplierText`를 생성하고 초기 문구를 `x1.00`으로 설정한다.
5. 유지 시간 Bar 배경 Image와 자식 Fill Image `MomentumDurationFill`을 생성한다.
6. Fill Image를 가로 방향 Filled Image로 설정하고 초기 Fill Amount를 `0`으로 설정한다. 색상은 임의 애니메이션으로 만들지 않고 Runtime Gradient가 적용되도록 기본 흰색으로 둔다.
7. UIManagementSystem의 Momentum HUD Root, 배율 Text, 유지 시간 Fill Image와 Gradient 필드에 위 객체를 연결한다.
8. Gradient Key를 Fill 비율 `1.00` 청록색, `0.60` 초록색, `0.30` 노란색, `0.10` 주황색, `0.00` 빨간색 순서로 설정한다. Unity Gradient 시간축은 `0`에서 `1` 방향이므로 Inspector에는 빨강→주황→노랑→초록→청록 순으로 배치한다.
9. 기존 `InfiniteHUD`에 `BaseDistanceScoreText`와 `MomentumBonusText`를 추가하고 UIManagementSystem의 해당 필드에 연결한다.
10. `InfiniteResultContent`에 `InfiniteResultBaseDistanceScoreText`, `InfiniteResultMomentumBonusText`, `InfiniteResultMaximumMultiplierText`를 추가하고 해당 필드에 연결한다.
11. 기존 Distance Score, Collectible Score와 Total Score 객체를 제거하지 말고 새 항목과 겹치지 않도록 배치한다.
12. Scene을 저장한 뒤 AI에 Scene 구성 완료 사실을 알린다.

필드의 실제 이름이나 요구 Component가 Step 4 구현 결과와 다르면 AI가 제공하는 최종 체크리스트를 우선한다. 배율 단계·유지 시간·Score 수치는 Scene Inspector에 중복 입력하지 않는다.

### 완료 조건

- [ ] Momentum HUD가 InfiniteHUD와 별도 Root로 존재하고 우측 하단에 Anchor됐다.
- [ ] 배율 Text, Fill Image와 Gradient 참조가 모두 연결됐다.
- [ ] InfiniteHUD와 Infinite Result의 새 Score Text 참조가 모두 연결됐다.
- [ ] AI의 Scene YAML 정적 검사에서 누락·중복 참조가 없다.

---

## Step 7. 생산 연결을 Play Mode Test로 검증한다

### AI 작업

- 실제 생산 Scene과 생산 Component를 사용하는 Play Mode 통합 Test를 작성하거나 갱신한다.
- 실제 Momentum 성공, 일반 착지, Wall 접촉, Pause·Resume, Result, Retry와 UI 갱신을 자동화한다.
- 속도 효과 제거, Score 구성 요소, Version, HUD Fill과 Result 값은 수치 Assertion으로 판정한다.
- 변경된 책임과 영향받는 기존 Play Mode 회귀 Test Class를 지정한다.

### 사용자 수동 작업

1. Unity Test Runner의 PlayMode에서 AI가 지정한 새 통합 Test와 영향받는 회귀 Test를 실행한다.
2. 가능하면 전체 Play Mode Test를 한 번 실행한다.
3. Tests Run·Passed·Failed 수와 예상하지 않은 Error·Warning 여부를 AI에 전달한다.
4. 실패가 있으면 Test 이름, 메시지와 Stack Trace를 전달한다.

정확한 착지 타이밍, Timer 만료, 빠른 Pause·Retry와 Score 숫자를 사람이 반복 재현하지 않는다. 이 항목은 Test가 판정한다.

### 완료 조건

- [ ] Momentum Landing 전후 속도 규칙과 입력 감속·정지가 자동 Test를 통과했다.
- [ ] Momentum·Score·Version·Result 생명주기 통합 Test가 통과했다.
- [ ] 생산 Scene의 HUD·Result 참조 및 표시값 Test가 통과했다.
- [ ] 예상하지 않은 Error·Warning이 없다.

---

## Step 8. 최소 화면 확인을 수행한다

### AI 작업

- 자동 판정할 수 없는 UI 가독성·배치·색상 구분 항목만 수동 체크리스트로 지정한다.
- 수치나 상태 결과를 화면 관찰만으로 통과 처리하지 않는다.

### 사용자 수동 작업

1. InfiniteMode로 진입하여 우측 하단 Momentum HUD가 기존 HUD, PausePanel과 ResultPanel의 조작을 방해하지 않는지 확인한다.
2. 기본 상태에서 `x1.00`과 빈 Bar가 읽기 쉬운지 확인한다.
3. Momentum Landing 성공 후 배율 Text와 Bar 길이가 눈에 잘 보이는지 확인한다.
4. Bar가 감소할 때 청록→초록→노랑→주황→빨강 변화가 배경과 구분되는지 확인한다.
5. 일반 이동 입력으로 Player가 감속하거나 정지할 수 있고, Momentum Landing 때문에 체감상 갑작스러운 가속이 발생하지 않는지 확인한다.
6. Pause 및 Result 화면에서 HUD가 겹치거나 가려야 할 조작 요소를 가리지 않는지 확인한다.
7. 확인 결과와 문제가 있으면 화면 상태·해상도·재현 절차를 AI에 전달한다.

Score 숫자 정확성, 유지 시간 초 단위, Pause 중 Timer 정지와 Retry 초기화는 이 화면 확인으로 판정하지 않는다. 해당 항목은 Unit Test와 Play Mode Test 결과를 사용한다.

### 완료 조건

- [ ] Momentum HUD의 위치, Bar 길이와 Gradient를 구분할 수 있다.
- [ ] 기존 HUD·Pause·Result UI와 중요한 겹침이 없다.
- [ ] 감속·정지 조작과 불필요한 가속 제거에 명백한 체감 문제가 없다.

---

## Step 9. Phase 2 결과를 기록하고 Roadmap을 갱신한다

### AI 작업

- Compile, Edit Mode, Play Mode와 화면 확인의 실제 결과만 기록한다.
- 실패가 있으면 원인을 수정하고 영향받는 Test 범위를 다시 지정한다.
- Feature·System 문서와 최종 생산 연결을 대조한다.
- 모든 완료 조건이 확인된 뒤에만 Phase 2 완료를 판정하고 Roadmap을 갱신한다.
- World Rebase, 장시간 좌표 안정성 및 Build를 Phase 2 완료 결과로 기록하지 않는다.

### 사용자 수동 작업

- AI가 정리한 검증 결과에서 본인이 실행한 수치와 화면 확인 내용이 정확한지 확인한다.

### 완료 조건

- [ ] Compile, 지정 Edit Mode·Play Mode Test와 화면 확인 결과가 기록됐다.
- [ ] 구현하지 않거나 검증하지 않은 Phase 3·4 범위가 구분됐다.
- [ ] 확인되지 않은 결과를 포함하지 않고 Phase 2 완료 여부가 판정됐다.

---

# 수동 작업 요약

사용자가 실제로 수행할 작업은 다음 네 종류로 제한한다.

1. Step 5: Unity Script Compilation과 Edit Mode Test 실행 및 결과 전달
2. Step 6: `SampleScene`의 Momentum HUD·Score Text 생성, Inspector 참조 연결 및 Scene 저장
3. Step 7: 지정된 Play Mode Test 실행 및 결과 전달
4. Step 8: HUD 가독성·겹침과 감속·정지 체감의 최소 화면 확인

Score 계산, 배율 단계, Timer 경계, 포화, Version, 중복 성공, Pause·Retry 상태와 표시 문자열의 정확성은 수동 작업으로 확인하지 않는다.

Phase 2에서는 Build를 수행하지 않는다. 대상 플랫폼 Build와 전체 장시간 검증은 Roadmap Phase 4에서 사용자가 직접 수행한다.

---

# 영향 범위

- Systems: PlayerMovementSystem, InfiniteModeSystem, RuntimeDataSystem, ResultSystem, UIManagementSystem
- Features: MomentumLanding, InfiniteMode, ScoreRecord
- Runtime: Player 이동, Momentum·Score 상태, Runtime Data와 Result Data
- UI: InfiniteHUD, Momentum HUD, Infinite Result
- Scene: 사용자가 편집하는 `Assets/Scenes/SampleScene.unity`
- Test: 관련 Edit Mode Unit Test 및 생산 Scene Play Mode 통합·회귀 Test

---

# 검증 내용

- AI 정적 검사 → Unity Script Compilation → Edit Mode Unit Test → 사용자 Scene 구성 → Scene YAML 정적 검사 → Play Mode Test → 최소 화면 확인 순서로 검증한다.
- 자동 판정 가능한 규칙은 Unit Test 또는 Play Mode Test 결과로만 완료 처리한다.
- Scene 구성은 저장된 YAML의 참조와 Component를 먼저 정적으로 검사하고, 화면 확인은 가독성·겹침·체감에만 사용한다.
- Build, World Rebase와 장시간 좌표 안정성은 Phase 2 검증 범위에서 제외한다.

---

# 검증 결과

- Phase 2 실행 전 계획 작성 완료.
- 실제 구현, Scene 편집, Compile, Test Runner와 화면 검증은 아직 수행하지 않았다.

---

# 후속 작업

- Step 1부터 순서대로 수행한다.
- Phase 2 완료 후 Roadmap Phase 3의 World Rebase 생산 연결을 별도 Task로 계획한다.

---

# 관련 문서

- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260916_01_Phase1Step5PureModels.md`
- `AI/90_Tasks/Prototype_5/20260916_03_Phase1VerificationResult.md`

---

# 작성 완료 기준

- Phase 2의 실제 수행 순서와 완료 조건을 Step으로 작성했다.
- AI 정적 작업, 자동 Test와 사용자 수동 작업을 분리했다.
- 자동 판정 가능한 항목을 수동 확인으로 요구하지 않는다.
- Scene 편집 항목과 Inspector 연결 대상을 구체적으로 작성했다.
- Build와 후속 Phase 범위를 Phase 2 완료 조건에 포함하지 않는다.
