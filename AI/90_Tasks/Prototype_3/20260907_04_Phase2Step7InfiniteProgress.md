# 작업 정보

## 작업명

Prototype 3 Phase 2 Step 7 InfiniteMode 진행 지속 조건

## 작업 상태

완료. Scene 설정과 사용자 Compile, Edit Mode 285개 및 Play Mode 146개 통과를 확인했다.

# 구현 내용

- InfiniteModeSystem은 계산 목표 속도 대신 `max(0, Rigidbody.linearVelocity.x)`를 진행 속도로 사용한다.
- CollisionSystem의 기존 Wall 접촉 결과를 InfiniteModeState에 전달한다.
- 최소 속도 2, 시작 유예 1초, 저속 유예 0.5초, Wall 추가 유예 1초를 적용했다.
- 시작 유예는 다른 유예를 소비하지 않는다.
- Wall 접촉 중 저속이면 Run당 Wall 추가 유예를 먼저 소비한다. 접촉 토글로 사용량을 복구하지 않는다.
- 실제 속도가 2 이상으로 회복되면 저속 누적과 Wall 추가 유예 사용량을 초기화한다.
- Pause는 모든 진행 시간을 동결하고 Retry는 State 초기화로 유예 상태를 초기화한다.
- 추락, 최대 거리와 Score 규칙은 유지했다.

# Test

- 실제 Rigidbody 속도와 Runtime 계산 속도가 다를 때 실제 속도를 사용하는 System Test 2개를 추가했다.
- Wall 유예 선소비, 접촉 토글, 실제 속도 회복 및 시작 유예 비소비 State Test 4개를 추가했다.
- 음의 X 속도는 진행 속도 0으로 판정하도록 기존 경계 Test를 갱신했다.
- 정적 예상 수는 Edit Mode 285개, Play Mode 146개다.

# Scene 수동 설정

SampleScene의 `InfiniteModeSystem` GameObject에서 다음을 설정한다.

1. `Collision System`에 `Player` GameObject의 `CollisionSystem` Component를 연결한다.
2. `Minimum Horizontal Speed`가 `2`인지 확인한다.
3. `Start Grace Duration`이 `1`인지 확인한다.
4. `Below Speed Grace Duration`이 `0.5`인지 확인한다.
5. `Wall Speed Grace Duration`을 `1`로 설정한다.
6. Scene을 저장한다.

# 사용자 검증

사용자가 다음 결과를 확인했다.

- SampleScene의 CollisionSystem 참조와 최소 속도/유예 Field가 지정 값으로 저장됨
- Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 없음
- 전체 Edit Mode Test 285개 실행 및 성공, 예상하지 않은 Error/Warning 없음
- 전체 Play Mode Test 146개 실행 및 성공, 예상하지 않은 Error/Warning 없음

# 최초 Edit Mode 실패와 수정

- `UpdateProgress_HorizontalSpeedBoundary_UsesPositiveVelocity(4.999f, True)`가 실패했다.
- 최소 속도 상수는 2로 변경했지만 TestCase가 이전 기준 5 주변의 값을 사용한 Test 데이터 오류였다.
- 양의 경계값을 1.999, 2.0, 2.001로 수정했다.
- 음의 X 속도는 크기와 관계없이 진행 속도 0으로 취급하므로 -1.999, -2.0, -2.001의 종료 기대값은 모두 true로 유지했다.
- 생산 코드는 변경하지 않았다.

Build는 이 Step에서 요구하지 않는다.

# 완료 조건

- [x] Scene Field가 지정 값과 참조로 설정되었다.
- [x] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.
- [x] 전체 Edit Mode 285개가 통과한다.
- [x] 전체 Play Mode 146개가 통과한다.
