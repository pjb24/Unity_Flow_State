# 작업 정보

## 작업명

Prototype 5 Phase 2 Step 3 Version 2 Score 생산 연결

## 작업 일자

20260916

## 작업 담당자

AI

---

# 작업 목적

새 InfiniteMode Run을 Scoring Version `2`로 고정하고 Momentum 배율이 적용된 거리 Score 구성 요소를 Runtime Data와 Result Data까지 일관되게 전달한다.

---

# 작업 대상

- GameRuntimeData, InfiniteModeRuntimeData
- InfiniteModeSystem, ResultSystem, GameSystem
- InfiniteScoreState와 ScoreRecord 생산 연결
- 관련 Edit Mode·Play Mode Test와 System 문서

---

# 작업 전 상태

- Step 2에서 생산 Momentum 상태는 Version `2`로 시작했지만 거리 Score는 Legacy ScoreCalculator와 Runtime Version `1` 경로를 사용했다.
- Runtime Data는 거리와 단일 Distance Score만 공개했고 ResultSystem의 생산 호출도 Version `1` API를 사용했다.
- Phase 1에는 InfiniteScoreState와 ScoreRecord의 Version `2` API 및 순수 모델 Test가 존재했다.

---

# 조사 내용

- 생산 Run 생성 경로는 `RuntimeDataSystem → GameRuntimeData.Initialize(E_GameMode.Infinite)`이다.
- InfiniteModeSystem은 거리 갱신, Momentum 성공 처리, 종료 조건을 같은 FixedUpdate에서 순서대로 실행한다.
- 착지 성공 이전 거리 증가분에 새 배율을 소급하지 않으려면 거리 Score 갱신 후 성공 상태를 처리해야 한다.
- Collectible Score는 GameRuntimeData의 공통 CollectibleRuntimeData가 소유하며 InfiniteScoreState는 전달받은 값을 배율 없이 Total에 합산한다.
- Stage 종료 Listener 안에서 Result Data를 생성한 뒤 GameSystem이 System Stop과 Runtime Clear를 수행하므로 최종 Runtime 값을 종료 요청 전에 확정해야 한다.

---

# 작업 내용

## Version과 Runtime Data

- 생산 `GameRuntimeData.Initialize(E_GameMode.Infinite)`가 InfiniteModeRuntimeData를 `ScoringVersion.Current`, 즉 `2`로 초기화한다.
- `InfiniteModeRuntimeData.Initialize()`는 Version `1` 호환 API로 유지했다.
- Version `2` Runtime 갱신 API는 Version, 거리, Base Distance Score, Momentum Bonus, Distance Score, Collectible Score, Total Score, 현재·최고 배율과 유지 시간을 한 요청으로 받는다.
- Version 불일치, 감소한 누적 값, Score 구성 요소·합계 불일치, 유효하지 않은 배율·시간 또는 최종 확정 후 갱신은 전체 요청을 거부한다.
- Legacy 두 인자 갱신 API는 Version `1` Runtime Data에서만 동작한다. Version별 API를 교차 사용할 수 없다.

## 생산 Score

- InfiniteModeSystem의 Legacy ScoreCalculator를 InfiniteScoreState로 교체했다.
- Run 초기화 시 Runtime·Momentum·Score가 모두 Version `2`인지 확인한다.
- 각 물리 갱신은 현재 최대 전진 거리를 당시 Momentum 배율과 함께 InfiniteScoreState에 전달한다. 그 다음 착지 성공을 Momentum 상태에 반영하므로 성공 전 이동분에 새 배율이 소급되지 않는다.
- Base Distance Score와 Momentum Bonus는 InfiniteScoreState의 값을 사용하며, Collectible Score는 공통 Runtime Data에서 읽는다. Total Score는 InfiniteScoreState의 포화 합 결과를 사용한다.
- Runtime Data에는 같은 시점의 Score 구성 요소, 합계와 Momentum 표시 상태를 함께 게시한다.
- Score와 Momentum을 종료 직전에 한 번 최종화한 뒤 Runtime Data를 최종화한다. 이후 갱신은 거부된다.

## Result

- ResultSystem에 Version `2` 생산 API를 추가하고 기존 Version `1` API는 호환 경로로 유지했다.
- GameSystem은 최종 Runtime Data의 Version, Base·Bonus·Distance·Collectible Score와 최고 배율을 ResultSystem에 전달한다.
- ScoreRecord가 구성 요소 관계, Version과 최고 배율을 검증하고 Total Score를 포화 합산한다.
- 새 생산 Run은 Legacy Result API를 호출하지 않는다.

## Test

- InfiniteModeRuntimeDataTests에 Version `2` 전체 상태 저장, Version 불일치, 구성 불일치의 원자적 거부, Version별 API 교차 거부, 포화와 최종 확정 후 거부를 추가했다.
- GameRuntimeDataTests는 생산 Infinite Run의 Version `2`와 완전한 Result 기록 경로를 사용하도록 갱신했다.
- MomentumProductionStateTests에 성공 전·후 거리 구간 배율, Collectible 배율 미적용, 최종 Runtime→Result 전달과 최종화 후 변경 거부 사례를 추가했다.
- ResultSystemTests에 Version `2` 전체 계약 전달과 무효 Version 거부를 추가했다. Overload를 정확히 선택하도록 Reflection 보조 코드를 갱신했다.
- InfiniteModeSystemTests와 InfiniteModeIntegrationTests에 생산 Version, Score 구성 요소 및 Result 합계 Assertion을 추가했다.
- InfiniteHudIntegrationTests의 인위적 Runtime 갱신은 Version `2` API를 사용하도록 갱신했다. UI 표시 구성 확장은 Step 4 범위다.

---

# 영향 범위

- 새 InfiniteMode 생산 Run의 Score와 Result Version이 `1`에서 `2`로 변경된다.
- Stage Mode, Version `1` 호환 API, Collectible 획득 규칙, Difficulty·Pattern, World Rebase와 Scene은 변경하지 않았다.
- UIManagementSystem은 기존 Distance Score·Collectible·Total 표시 경로를 유지한다. Base·Bonus·Momentum HUD와 Runtime Total 직접 사용은 Step 4에서 연결한다.

---

# 검증 내용

- Version 생성부터 Runtime·Score·Result까지 전달되는 호출부와 인자 순서를 정적으로 대조했다.
- Score 구성 요소와 Total의 포화는 생산 InfiniteScoreState와 ScoreRecord를 호출하도록 확인했다.
- Legacy API가 생산 호출부에서 사용되지 않고 Test 호환 경로에만 남아 있는지 검색했다.
- 변경 C#의 괄호, `.meta`, 참조 이름과 Overload 호출을 정적으로 검사했다.
- `git diff --check` 및 Scene·Prefab·ProjectSettings 변경 부재를 확인했다.

---

# 검증 결과

- Step 3 코드·Test 작성과 정적 검사를 완료했다.
- 사용자가 Unity Editor Script Compilation 성공을 확인했다.
- 사용자가 전체 Edit Mode Test `610`개를 실행했으며 `610`개 모두 성공했다.
- Script Compilation과 Edit Mode Test에서 예상하지 않은 Error·Warning이 없음을 사용자가 확인했다.
- AI는 Unity Editor Compilation, Build, Unity Test Runner와 수동 플레이를 실행하지 않았다.
- Step 4 UI 연결과 후속 Unity 검증이 남아 있으므로 Phase 2 완료를 선언하지 않는다.

---

# 후속 작업

## 사용자 수동 작업

현재 Step 3을 위해 Scene에서 추가하거나 연결할 객체는 없다. Inspector 값도 변경하지 않는다.

다음 Edit Mode Test를 포함한 전체 Edit Mode Test `610`개는 사용자가 실행하여 모두 성공했다.

1. `InfiniteScoreStateTests`
2. `InfiniteModeRuntimeDataTests`
3. `GameRuntimeDataTests`
4. `ScoringVersionTests`
5. `ScoreRecordTests`
6. `ResultDataTests`
7. `ResultSystemTests`
8. `MomentumProductionStateTests`

생산 통합 Play Mode Test는 Step 7에서 별도로 지정한다. AI는 Unity Test Runner와 Build를 실행하지 않는다.

수치, 포화, Version과 중복 종료는 수동 플레이로 확인하지 않는다. Scene 편집은 Step 4 코드와 Step 5 검증 이후 Step 6에서 사용자가 수행한다.

## AI 후속 작업

- Step 4에서 Runtime Data의 Base·Bonus·Total·Momentum 표시 상태를 HUD와 Result Formatter에 연결한다.
- Step 4 변경 이후 Step 5에서 Script Compilation과 전체 Edit Mode 회귀 Test를 다시 확인한다.

---

# 관련 문서

- `AI/01_Rules/IMPLEMENTATION_RULE.md`, `VERIFICATION_RULE.md`
- `AI/02_Systems/InfiniteModeSystem.md`, `RuntimeDataSystem.md`, `ResultSystem.md`
- `AI/03_Features/InfiniteMode.md`, `ScoreRecord.md`, `MomentumLanding.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260916_04_Phase2ManualSteps.md`

---

# 관련 작업 기록

- `20260916_05_Phase2Step1ProductionInvestigation.md`
- `20260916_06_Phase2Step2MomentumProduction.md`

---

# 작성 완료 기준

- 생산 Version `2` 연결과 Legacy 호환 경로를 구분했다.
- Score·Runtime·Result의 같은 Version과 구성 요소 관계를 기록했다.
- 정적 검사와 미실행 Unity 검증을 구분했다.
