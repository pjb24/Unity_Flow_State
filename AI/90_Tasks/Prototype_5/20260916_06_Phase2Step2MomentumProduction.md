# 작업 정보

## 작업명

Prototype 5 Phase 2 Step 2 속도 효과 제거와 Momentum 생산 상태 연결

## 작업 일자

20260916

## 작업 담당자

AI

---

# 작업 목적

기존 Momentum Landing의 수평 속도 보상을 제거하고, 실제 성공 한 번을 InfiniteMode의 Momentum 순수 모델에 한 번 전달한다. Pause·종료·Retry·새 Run의 생산 생명주기를 연결한다.

---

# 작업 대상

- MomentumLandingFeature, PlayerMovementSystem, PlayerMovementRuntimeData
- InfiniteModeSystem과 기존 MomentumScoreState
- 관련 Edit Mode·Play Mode Test와 System 문서

---

# 작업 전 상태

- 착지 성공 시 Feature가 속도에 `1.15`를 곱하고 `14`로 제한했다.
- 생산 System에는 MomentumScoreState 호출과 성공 요청 ID 전달 경로가 없었다.
- Step 1의 Tasks 문서 2개 변경이 작업 시작 시 존재했으며 보존했다.

---

# 조사 내용

- `MomentumScoreState`의 단계·유지 시간·만료·중복 ID 거부·Pause·Finalize 계약을 재사용했다.
- `IsLastLandingMomentum`은 마지막 착지 종류를 유지하므로 신규 성공 신호로 사용할 수 없었다.
- Movement와 InfiniteMode는 별도 FixedUpdate를 사용하므로 같은 물리 갱신의 성공을 만료보다 먼저 전달할 순서가 필요했다.
- 정상 종료 후 GameSystem은 InfiniteModeSystem을 Stop하고 Runtime Data를 Clear한다. 최종 Momentum 상태를 Stop에서 즉시 Reset하면 Result용 상태가 소실된다.
- Edit Mode asmdef는 Core·Features를 참조하고 System은 Assembly-CSharp에 있다. 기존 ResultSystemTests와 같은 Reflection 방식으로 생산 System의 실제 메서드를 호출한다. asmdef 구조는 변경하지 않았다.
- Step 1에서 자동 전진·Move 비활성화가 기존 생산 계약임을 확인했다. Momentum 속도 효과만 제거하고 해당 자동 이동 계약은 유지했다.

---

# 작업 내용

## 속도 효과 제거와 성공 ID

- MomentumLandingFeature에서 `_speedMultiplier`, `_maximumHorizontalSpeed` 설정과 속도 배율 계산을 제거했다.
- 성공·실패 모두 전달받은 수평 속도를 그대로 반환한다. 일반 이동의 속도 제한은 PlayerMovementSystem의 기존 설정 책임으로 유지한다.
- Feature의 `SuccessId`는 실제 성공에서만 증가하고 `BeginJump`에서는 보존하며 `Initialize`에서 초기화한다. 중복 착지는 기존 판정 상태로 차단하고 ID overflow는 성공으로 반영하지 않는다.
- PlayerMovementSystem은 실제 성공 지점에서 PlayerMovementRuntimeData의 `TryRecordMomentumLanding`으로 ID를 공개한다. Runtime Data는 동일·역행·0·음수 ID를 거부한다.
- 일반 착지, Wall 제약, 매 프레임 Runtime 표시 갱신은 성공 ID를 증가시키거나 지우지 않는다.

## 생산 Momentum 상태

- InfiniteModeSystem이 MomentumScoreState를 소유한다. 새 Infinite Run에서만 상태를 시작하고 Stage에서는 기본 상태를 유지한다.
- `ProcessMomentumStep`은 유효한 현재 Run의 Infinite Playing에서만 실행한다. 새로운 ID가 있으면 성공을 한 번 처리하고, 같은 ID를 다시 읽으면 성공 없이 Timer만 진행한다.
- 순수 모델이 거부한 음수·NaN·무한대 delta time은 성공 ID 소비나 Timer 변경을 일으키지 않는다.
- PlayerMovementSystem의 기본 실행 순서를 `-100`, InfiniteModeSystem을 `0`으로 명시했다. Scene·ProjectSettings의 실행 순서를 편집하지 않았다.
- InfiniteMode의 FixedUpdate는 기존 거리 갱신 다음 Momentum을 처리하고 종료 조건을 평가한다. Step 3에서도 착지 이전 거리 증가분을 새 배율로 소급 계산하지 않도록 이 경계를 유지한다.
- Pause·Resume은 순수 모델의 해당 API로 상태를 보존·재개한다. 같은 실행 중 Run에 대한 중복 Initialize도 Momentum과 Pause 상태를 보존한다.
- 종료 확정과 Stop은 Momentum을 Finalize하며 최종 배율·최고 배율·남은 시간을 다음 Run 초기화 전까지 보존한다. Stop 이후에는 시간·성공을 갱신하지 않는다.
- 새 Run/Retry의 Initialize는 Momentum과 마지막 처리 ID를 Reset한다. GameSystem의 기존 새 Runtime 생성·이동 Feature 초기화 경로가 생산자 ID도 초기화한다.
- 현재/최고 배율, 유지 시간·남은 시간·비율을 읽기 전용 System 속성으로 제공한다. 외부에서 순수 상태를 직접 변경할 수 있도록 노출하지 않았다.

## Step 3·4와의 경계

- 이번 Step은 배율 상태 연결이며 실제 거리 Score에는 아직 배율을 적용하지 않는다.
- GameRuntimeData의 Infinite Score Version과 Result는 기존 `1`이다. 새 Momentum 상태는 현재 Momentum 규칙 `2`로 시작하지만 기존 Score·Result와 결합하지 않는다.
- Step 3에서 생산 Run의 Version을 명시적으로 `2`로 전환하고 동일 Version을 Momentum·InfiniteScoreState·Runtime·Result에 전달해야 한다. 현재의 단계별 구현 상태를 완성된 Version 2 Score 경로로 판정하지 않는다.
- Momentum 표시용 Runtime 데이터 전달, 최종 HUD 동기화, 새 Result·HUD 참조 연결은 후속 Step이다.
- 자동 전진·Move 비활성화와 기존 입력 바인딩을 변경하지 않았다.

## Test

- `MomentumLandingFeatureTests`: 양수·음수·0·기존 상한 초과 입력 속도 보존, 성공 ID 증가·초기화·실패·중복 거부 기대값을 작성했다.
- 신규 `MomentumProductionStateTests`: 31개 사례. 실제 Feature와 PlayerMovementSystem의 ResolveLanding, InfiniteModeSystem의 Initialize·FixedUpdate·ProcessMomentumStep·Pause·Resume·FinalizeRunMetrics·Stop을 호출한다.
- 신규 사례는 성공 전달·동일 ID 재조회·마지막 착지 플래그만 있는 경우, Stage 배율 미적용, 일반 착지·Wall 속도 제약, 9가지 연속 성공 단계·상한·만료, 성공과 만료 동시 발생, Pause·비Playing·비정상 시간, 종료 보존·새 Run 초기화·중복 초기화·실행 순서를 다룬다.
- `MomentumLandingIntegrationTests`의 기존 Stage 착지 성공 기대값을 속도 증가에서 `8` 유지로 변경하고 ID가 한 번만 전달되는 Assertion을 추가했다.
- 기존 MomentumScoreStateTests는 수정하지 않았다. Test 안에 배율 계산식을 복제하지 않았으며 생산 메서드 결과를 명시된 계약값과 비교한다.

---

# 영향 범위

- Momentum Landing의 속도 효과 제거는 Stage·Infinite 공통 이동에 적용된다.
- Momentum 배율 갱신은 InfiniteMode에만 적용된다.
- PlayerMovementSystem·InfiniteModeSystem의 실행 순서와 종료 경계 변경에 대해 기존 이동·Pause·InfiniteMode 회귀 확인이 필요하다.
- World Rebase, Pattern·Camera 알고리즘, Collectible 보상, Score Version 전환, Scene·Prefab·ProjectSettings는 변경하지 않았다.

---

# 검증 내용

- 변경 생산 코드·Test의 호출부, 생성자 인자, Reflection 대상 멤버, 초기화·Pause·종료·재시작 경계를 정적으로 검토했다.
- 변경 C# 7개 파일의 괄호 짝, `.meta` 존재와 금지된 LINQ·null 연산자 사용 여부를 텍스트 수준에서 확인했다. 이 검사는 C# 컴파일을 대체하지 않는다.
- 신규 Test `.meta` GUID `5f2281fa97194a5fbc5fe3cf63bb30f8`가 Assets 내에서 한 번만 정의됨을 확인했다.
- 삭제한 속도 설정을 참조하는 생산 코드·Test가 남아 있지 않은지 검색했다.
- `git diff --check`와 Scene·Prefab·ProjectSettings 변경 부재를 확인했다.

---

# 검증 결과

- Step 2 코드·Test 작성과 정적 검사를 완료했다.
- Unity Editor, Compilation, Build, Unity Test Runner, 수동 플레이를 실행하지 않았다.
- 신규 Test 31개는 작성된 사례 수이며 Passed 수가 아니다. Compile·Test 통과 여부는 아직 확인되지 않았다.
- 전체 Phase 2 완료를 판정하지 않는다. 실제 배율 Score·Version 2 전달과 UI 연결이 남아 있다.

---

# 후속 작업

## 사용자 수동 작업

Step 2 구현을 위해 지금 Scene에서 생성하거나 연결할 객체는 없다. 신규 Inspector 참조도 없다.

Step 5에서 사용자가 Unity Editor의 Compilation 결과와 아래 Edit Mode Test 결과를 확인한다. 이번 Step만 먼저 검증하는 경우에도 같은 목록을 사용한다.

1. Compile Error 및 예상하지 않은 Warning 유무를 확인한다.
2. `MomentumLandingFeatureTests`, `MomentumProductionStateTests`, `MomentumScoreStateTests`, `PlayerMovementMathTests`, `GameRuntimeDataTests`를 실행한다.
3. Tests Run·Passed·Failed와 Error·Warning 여부를 전달한다. 실패하면 Test 이름·메시지·Stack Trace를 전달한다.

생산 통합 회귀는 Step 7에서 `MomentumLandingIntegrationTests`, `AutoMovementIntegrationTests`, `InfiniteModeSystemTests`, `InfiniteModeIntegrationTests`, `GamePauseOrchestrationTests`를 포함한다. 현재 Step만 검증할 경우 사용자가 이 목록을 먼저 실행할 수 있다. AI는 Test Runner를 실행하지 않는다.

Scene YAML에는 제거된 Feature 속도 필드의 이전 저장값이 남아 있다. 코드 의존성은 제거했으므로 값을 수동으로 `1`로 바꾸거나 Component를 제거·재추가할 필요가 없다. Step 6의 사용자 Scene 편집·저장 시 해당 Feature에는 Momentum Landing Window만 남고, PlayerMovementSystem의 별도 Maximum Horizontal Speed 설정은 유지되는지 확인한다. AI는 Scene 파일을 편집하지 않는다.

수치·Timer·동일 착지 중복 여부를 수동 플레이로 재현할 필요는 없다. Build는 Step 2 검증 조건이 아니며 사용자가 원하는 시점에 직접 실행한다.

## AI 후속 작업

- Step 3의 Score·Version 2 생산 연결을 수행한다.
- Step 4에서 Momentum 표시용 Runtime 데이터와 HUD·Result 연결을 완료한다.
- 기존 자동 이동·Move 비활성 계약을 회귀 Test로 유지한다.
- 사용자 Compile·Test 결과가 제공되면 실제 결과만 기록하고 실패를 수정한다.

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/CODING_STYLE.md`, `EVENT_RULE.md`, `LOGGING_RULE.md`, `VERIFICATION_RULE.md`
- `AI/02_Systems/PlayerMovementSystem.md`, `InfiniteModeSystem.md`, `PlayerInputSystem.md`
- `AI/03_Features/MomentumLanding.md`, `InfiniteMode.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/99_Templates/FEATURE_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `20260916_04_Phase2ManualSteps.md`
- `20260916_05_Phase2Step1ProductionInvestigation.md`

---

# 작성 완료 기준

- Step 2 구현과 후속 Score·UI·입력 작업을 구분했다.
- 정적 검사와 미실행 Unity 검증을 구분했다.
- 수동 작업은 Unity 실행 결과 확인과 후속 Scene 편집에 한정했다.
