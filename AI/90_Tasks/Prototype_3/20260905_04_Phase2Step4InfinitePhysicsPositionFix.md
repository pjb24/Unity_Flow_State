# 작업 정보

## 작업명

Phase 2 Step 4 InfiniteMode 거리 판정 물리 위치 통일

## 작업 일자

20260905

## 작업 담당자

AI: 코드 조사, 수정 및 정적 검증

사용자: Unity Compile 및 Test Runner 재검증

## 작업 상태

완료. 수정 후 사용자 Compile, Edit Mode 281개 및 Play Mode 141개 통과를 확인했다.

# 문제 현상

사용자가 Play Mode Test 1개 실패를 보고했다.

- Test: `PlayerFallsAtLargeX_EndsAndStopsPlaySystems`
- 위치: `Assets/Tests/PlayMode/InfiniteModeIntegrationTests.cs:196` (보고 당시)
- 기대 FinalDistance: `10000.0f`
- 실제 FinalDistance: `9999.9834f`
- 로그에는 GameSystem의 시작, 종료, 재시작 및 종료 흐름이 있다.
- 사용자는 이번 메시지에 전체 Test 수, Passed 수 및 Compile/Error/Warning 검증 결과를 별도로 제공하지 않았다. 이를 추정하여 통과로 기록하지 않는다.

# 재현 조건

- 자동 이동이 연결된 Step 4 코드에서 SampleScene을 로드한다.
- 기존 Run을 종료하고 Infinite Run을 시작한다.
- 큰 X 위치 및 추락 임계값 아래로 Player를 옮겨 종료를 유도한다.
- Result.FinalDistance를 StartPoint 기준 기대값과 비교한다.
- Scene의 Player Rigidbody는 보간이 활성화되어 있다. 보고된 단일 실행 외 반복 재현 빈도는 확인하지 않았다.

# 원인 조사

- PlayerControllerSystem.ResetToStartPoint는 Rigidbody.position을 시작점으로 변경한다.
- InfiniteModeSystem.InitializeRunMetrics는 Transform.position.x를 원점으로 저장했다.
- 같은 System의 UpdateRunMetrics와 ProcessFallThreshold도 Transform.position을 사용했다.
- InfiniteDistanceState는 최대 전진 위치에서 시작 원점을 뺀 값을 저장하므로 원점이나 관측 위치가 다르면 거리 및 Score에 그대로 반영된다.
- 실패 Test 역시 Transform.position으로 위치를 바꾸고 물리 단계 후 정확한 거리를 기대했다.
- 물리 위치의 초기화와 표시 Transform의 관측을 섞는 경로를 코드에서 확인했다. 로그에는 원점과 종료 시 관측 좌표가 없으므로 `0.0166`의 차이를 원점 오차와 종료 위치 오차 중 하나로 단정하지 않는다.

# 원인

Run 위치 초기화와 거리/추락 관측이 동일한 위치 소스를 사용하지 않는 구조적 불일치다. 물리 위치를 기준으로 진행해야 하는 경로가 Transform 갱신 시점에 의존했다.

정확한 실행 시점별 수치 변화와 수정 후 비재현 여부는 사용자 재검증으로 확인한다. 부동소수점 허용 오차 부족으로 판단하여 기대값을 완화하지 않는다.

# 수정 내용

- InfiniteModeSystem은 기존 `_player` 참조에서 Rigidbody를 초기화 시 한 번 찾고 보관한다. 새 Serialized Reference는 추가하지 않는다.
- 시작 원점, 거리 갱신, 최종 거리 및 추락 위치는 Rigidbody.position을 사용한다.
- Rigidbody가 없으면 초기화를 실패시키고 명시적 Error를 기록한다. Transform으로 대체하여 오류를 숨기지 않는다.
- InfiniteModeSystem은 Rigidbody를 읽기만 하며 Player 이동 및 물리 적용 책임은 Controller에 유지한다.
- 기존 실패 Test와 관련 종료/Retry Test는 Rigidbody.position으로 물리 위치를 설정한다. 실패 Test의 `10000` 기대값과 정확 비교는 유지한다.
- InfiniteModeSystemTests의 Test Player에도 Rigidbody를 구성하고 거리/추락 입력을 같은 물리 위치 기준으로 변경했다. 기존 거리/Score 기대값은 유지한다.
- 신규 재시작 Test 2개는 Interpolate/None에서 실제 자동 이동 후 Run을 재시작하고 원점, 초기 거리 0, 물리 위치 변경 직후 거리 10/Score 100, 추락 최종 거리 10000/Score 100000을 검사한다. 보간 설정은 finally에서 복원한다.
- 신규 초기화 Test 1개는 Rigidbody 누락 시 실패 및 해당 Error를 검증한다. LogAssert.Expect는 의도적으로 누락시킨 필수 Component에 대한 명시적 Error 한 건만 대상으로 한다.
- System/Feature 문서에 물리 위치를 사용하는 책임과 규칙을 반영했다.

# 영향 범위

- `Assets/Scripts/Runtime/Systems/InfiniteModeSystem.cs`
- `Assets/Tests/PlayMode/InfiniteModeIntegrationTests.cs`
- `Assets/Tests/PlayMode/InfiniteModeSystemTests.cs`
- InfiniteModeSystem/InfiniteMode 문서 및 Step 4 작업 기록

# 검증 내용

- InfiniteModeSystem의 위치 판정 3곳이 Rigidbody.position을 읽으며 Transform.position 판정이 남아 있지 않음을 확인했다.
- SampleScene YAML을 읽기 전용으로 확인했다. Player Transform(2089254631)과 Rigidbody(2089254635)는 같은 Object(2089254630)에 있고 보간 설정은 1이다.
- 기존 실패 기대값과 두 Infinite Test 파일의 기존 메서드가 보존되었음을 확인했다. 정적 집계는 Play Mode 141개, Edit Mode 281개다.
- 변경 파일 목록에서 Scene/Asset/ProjectSettings 및 거리/Score 순수 계산의 무변경을 확인했다.
- 관련 문서 경로가 존재하고 git diff --check가 종료 코드 0으로 통과했음을 확인했다.

# 검증 결과

- 위 정적 검사를 통과했다.
- AI는 Unity Compile 및 Test Runner를 실행하지 않았다.
- 사용자가 Unity Script Compilation 성공과 예상하지 않은 Error/Warning 부재를 확인했다.
- 사용자가 전체 Edit Mode 281개 및 전체 Play Mode 141개를 실행하여 모두 성공했고 예상하지 않은 Error/Warning이 없음을 확인했다.
- Scene 변경이나 사용자 Serialized Field 설정은 필요하지 않다.
- InfiniteMode 실제 속도 관측과 Wall 유예의 Step 7 계약은 이번 위치 수정과 구분하여 유지한다.

# 후속 작업

추가 사용자 작업은 없다. Step 5를 진행한다.

# 관련 문서

- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/99_Templates/BUGFIX_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260904_02_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_3/20260905_03_Phase2Step4AutoMovementIntegration.md`

# 작성 완료 기준

- 사용자 실패 증거와 코드에서 확인한 불일치를 기록했다.
- 관측하지 않은 세부 수치를 원인으로 단정하지 않았다.
- 정확한 거리 기대값을 유지한 상태로 재검증 통과를 기록했다.
