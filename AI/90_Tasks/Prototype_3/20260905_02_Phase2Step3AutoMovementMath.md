# 작업 정보

## 작업명

Prototype 3 Phase 2 Step 3 자동 수평 이동 순수 계산

## 작업 일자

20260905

## 작업 담당자

AI: Test 작성, 계산 구현 및 정적 검증

사용자: Unity Script Compilation 및 Edit Mode Test 실행

## 작업 상태

Step 3 완료. 계산 API 및 Test 작성, 정적 검증과 사용자 Unity Compile/Edit Mode 검증 결과 확인 완료.

# 작업 목적

Step 1의 자동 우측 가속, 관성 속도 보존과 Step 3의 경계값 방어 계약을 입력 장치 및 Scene에 의존하지 않는 순수 계산으로 제공한다.

# 작업 대상

- `Assets/Scripts/Runtime/Core/PlayerMovementMath.cs`
- `Assets/Tests/EditMode/PlayerMovementMathTests.cs`
- Phase 2 Manual Steps 및 Roadmap 진행 상태

# 작업 전 상태

- Step 1 규칙 확정 및 Step 2 정적 조사는 완료되었다.
- 기존 CalculateHorizontalSpeed는 HorizontalInput에 의존하며 PlayerMovementSystem이 사용한다.
- 기존 PlayerMovementMathTests에는 정적 집계 기준 21개 Test case가 있다.

# 조사 내용

- PlayerMovementMath의 기존 Ground/Air 계산, 관성 속도 유지, 수직/중력 및 Wall 제한 함수를 확인했다.
- 기존 Test 기대값과 Core/Features를 참조하는 Edit Mode asmdef를 확인했다.
- Step 4에서 PlayerMovementSystem 연결, Step 5에서 Move 입력 제거를 수행하므로 Step 3에서는 기존 생산 호출 경로를 변경하지 않는다.
- 새 API는 Step 3 Test가 직접 사용하며 Step 4의 공통 이동 계산 연결 대상이다. 기존 입력 기반 API는 현재 생산 호출을 유지하기 위해 남겼고, 관련 호출 제거 후 정리한다.

# 작업 내용

## Test 우선 작성

PlayerMovementMathTests에 먼저 새 API 호출과 기대값을 작성한 뒤 CalculateAutoHorizontalSpeed를 구현했다. Unity Test Runner는 실행하지 않았으므로 실패 후 통과하는 실행 결과를 관찰한 것으로 기록하지 않는다.

추가한 Test case는 정적 집계 기준 35개다.

| 검증 대상 | Test 내용 |
|-----------|-----------|
| 자동 방향 및 가속 | 정지에서 Ground 1, Air 0.5 증가(0.02초), 음의 속도에서 우측 회복 및 최종 +8 도달 |
| 목표와 상한 | 7.9에서 8로 도달, 8 유지, 9.2 관성 보존, 14 상한 및 20에서 제한, 기본 목표가 최대값보다 큰 경우 |
| Ground/Air 전환 | 접지 상태가 바뀌어도 관성 속도 9.2 유지 |
| 시간 | dt 0/음수/NaN/무한대에서 가속하지 않음, 같은 총 시간의 1회/10회 계산 결과 비교 |
| 설정 방어 | 음수/NaN/무한대 목표, 지상 가속, 공중 가속, 최대 속도를 각각 검사. 사용하지 않는 가속 설정은 현재 상태에 영향 없음 |
| 속도 방어 | 비유한 현재 속도는 0에서 재시작. dt 0에서도 최대 속도 범위 제한 |
| 극단값 | 큰 유한 설정/시간의 곱이 범위를 넘는 경우에도 유한 목표 반환 |
| Wall 조합 | 새 수평 계산 후 기존 Wall 제한 호출: 우측 벽 X=0 및 중력 낙하, 이탈 후 Air 가속 0.5 복구, Ground 우선, 왼쪽 벽에서 우측 진행 |

기존 21개 Test case의 본문과 기대값은 모두 보존했다. Jump/착지의 실제 상태 전환은 기존 Feature Test 및 후속 Step 4 통합 Test 범위이며, 새 API에는 Jump/Landing/Input 인자가 없어 자동 목표 방향이 이들 입력에 의존하지 않는다.

## 계산 API

`PlayerMovementMath.CalculateAutoHorizontalSpeed(currentSpeed, isGrounded, deltaTime, moveSpeed, groundAcceleration, airAcceleration, maximumHorizontalSpeed)`를 추가했다.

- 기본 목표와 현재 우측 속도 중 큰 값을 목표로 사용하여 관성 착지 보상을 보존한다.
- Ground/Air에 따라 선택한 가속도와 전달받은 dt로 MoveTowards를 사용한다. 자동 가속으로 기본 목표를 초과하지 않는다.
- 최대 수평 속도는 입력 속도 정규화와 최종 결과에 적용한다.
- 기존 CalculateHorizontalSpeed, Jump, 수직/중력, 수평 가속도 및 Wall 제한 함수는 변경하지 않았다.
- Transform/Rigidbody/Scene/입력 장치/Time에 접근하지 않는다. 반복 경로에 Log, LINQ, 컬렉션 또는 객체 생성이 없다.

### 방어 동작

Step 3의 유효하지 않은 값 방어를 다음과 같이 구체화하고 Test에 고정했다.

- 음수 또는 비유한(NaN/Infinity) 목표 속도, 가속도, 최대 속도 및 dt는 각각 0으로 정규화한다.
- 현재 속도는 음수를 허용하되 비유한 값만 0으로 정규화한다.
- dt가 유효하지 않으면 가속하지 않는다. 최대 속도에 대한 안전 제한은 dt와 관계없이 적용한다.
- 최대 속도가 유효하지 않으면 결과는 0이다.
- 기본 목표가 0이어도 현재 양의 관성 속도는 유효한 최대 속도 안에서 보존한다. 선택된 가속도가 0이면 범위 안의 현재 속도를 유지한다.
- 가속도/시간의 곱이 매우 커져도 유한한 목표로 도달하도록 MoveTowards를 사용한다.

# 영향 범위

- Runtime Core: 순수 자동 수평 계산 API 추가
- Edit Mode Test: 기존 Test 파일에 자동 계산 및 Wall 조합 검증 추가
- Tasks/Roadmap: Step 3 완료 및 사용자 검증 결과 기록

# 검증 내용

- 기존 Test 부분을 추가 전 HEAD와 비교하여 본문 및 기대값 유지 여부를 검사했다.
- 기존 수학 함수 부분을 공백 정규화 후 HEAD와 비교하여 보존을 확인했다.
- Test 이름 중복 및 정적 Test case 수를 검사했다.
- 신규 계산의 입력 방향, 속도 부호, 목표/상한 구분과 유한 값 방어를 코드로 검토했다.
- 생산 API의 Scene, Input, Time, Log 및 할당 의존성을 검색했다.
- 기존 Script/Test meta가 존재하며 새 C# 파일이나 GUID가 필요하지 않음을 확인했다.
- git diff --check, 문서 링크 존재, Test 이름 중복 및 기존 meta GUID의 유일성을 검사했고 통과했다.

# 검증 결과

- 기존 계산 함수와 기존 21개 Math Test case가 유지되었다.
- Math Test case는 정적 집계 21 → 56개(35개 추가)이며 실행 Passed 수가 아니다.
- Scene, Input Asset, Wrapper, PlayerMovementSystem 및 Jump/Landing Feature는 변경하지 않았다.
- AI는 Unity Compile, Edit Mode/Play Mode Test 및 Build를 실행하지 않았다. 자동 이동이 실제 플레이에 연결되었다고 판단하지 않는다.
- 사용자 보고에 따라 Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 부재를 확인했다.
- 사용자 보고에 따라 전체 Edit Mode `281 Passed, 0 Failed, Total 281` 및 예상하지 않은 Error/Warning 부재를 확인했다. 이는 지정한 4개 Class만의 정적 집계 72개보다 넓은 전체 Edit Mode 실행 결과다.
- 이전 Phase 1 기준 246개에 이번 추가 35개를 더한 281개와 사용자 보고 수가 일치한다.
- 검증 근거는 사용자의 채팅 보고이며 AI가 Test Runner 또는 결과 파일을 직접 확인한 것은 아니다.
- Step 3의 Compile 및 자동 계산/관련 회귀 Test 완료 조건을 충족하여 완료 처리했다. Play Mode와 Build 결과는 이번 보고에 포함되지 않는다.

# 후속 작업

## 사용자 검증 절차 및 이행 상태

아래 안내 이후 사용자가 전체 Edit Mode 281개 실행 성공을 보고했다. Step 3을 위한 추가 실행은 필요하지 않다.

1. Unity Editor로 돌아가 Script Compilation이 끝날 때까지 기다린다. Console에 새 Compile Error 또는 예상하지 않은 Warning이 있는지 확인한다.
2. Unity Test Runner의 Edit Mode에서 아래 네 Test Class를 실행한다. 이미 열린 Test Runner를 사용하거나 `Window > General > Test Runner`에서 연다.
3. 각 Class의 Passed/Failed/전체 수와 예상하지 않은 Error/Warning을 알려준다. 실패 시 Test 이름, 메시지와 Stack Trace를 함께 제공한다.

| Test Class | 정적 case 수 | 목적 |
|------------|--------------|------|
| PlayerMovementMathTests | 56 | 자동 수평 계산, Ground/Wall 및 수직 계산 회귀 |
| JumpFeatureTests | 5 | Jump/Coyote 및 재점프 제한 회귀 |
| MomentumLandingFeatureTests | 9 | 관성 착지 성공/실패, Window와 속도 보상 회귀 |
| NormalLandingFeatureTests | 2 | 일반 착지 및 중복 판정 회귀 |
| 합계 | 72 | 실제 Test Runner 발견/실행 수는 사용자 결과로 확인 |

Step 3에서 Scene 설정/저장, Input Action 재생성, 수동 플레이 속도 측정 및 Build는 필요하지 않다. Play Mode 연동 검증은 Step 4 이후 해당 구현에 맞춰 수행한다.

다음 작업은 Step 4의 PlayerMovementSystem Mode 공통 자동 이동 통합이다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260904_02_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_3/20260905_01_Phase2Step2Investigation.md`

# 작성 완료 기준

- 확정 계약, 방어 동작, 구현 범위와 검증 범위를 기록했다.
- Test 작성과 실제 실행 결과를 구분했다.
- 필요한 사용자 Compile/Test 작업만 명세하고 Scene/Build 작업을 요구하지 않았다.
