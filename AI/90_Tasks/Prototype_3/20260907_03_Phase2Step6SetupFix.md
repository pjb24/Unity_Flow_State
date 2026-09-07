# 작업 정보

## 작업명

Phase 2 Step 6 AutoMovementIntegrationTests 공통 SetUp 수정

## 작업 일자

20260907

## 작업 상태

완료. 수정 후 사용자 Compile 및 Play Mode 144개 통과를 확인했다.

# 문제 현상

- 사용자가 Play Mode Test 17개 실패를 보고했다.
- 보고된 각 실패는 개별 Test 본문에 진입하기 전에 AutoMovementIntegrationTests.SetUp에서 발생했다.
- 실패 메시지는 `Expected: not null, But was: null`이다.
- Stack Trace는 `FindSystem`을 거쳐 SetUp의 PlayerControllerSystem 조회를 가리킨다.

# 원인

Step 6 Test가 PlayerControllerSystem Component를 얻기 위해 `PlayerControllerSystem`이라는 GameObject를 조회했다. SampleScene에는 해당 이름의 GameObject가 없으며 PlayerControllerSystem은 `Player` GameObject에 연결되어 있다.

공통 SetUp이 실패하여 AutoMovementIntegrationTests의 기존 및 신규 case 17개가 모두 실행되지 못했다. 생산 자동 이동, Pause 또는 Retry 동작의 실패 증거는 아니다.

# 수정 내용

- `FindSystem("PlayerControllerSystem", "PlayerControllerSystem")`을 `FindSystem("Player", "PlayerControllerSystem")`으로 변경했다.
- 기존 FindSystem 검증 방식과 Component type 검사를 유지했다.
- 생산 Runtime과 SampleScene은 변경하지 않았다.

# 정적 검증

- SampleScene YAML에서 `Player` GameObject와 연결된 PlayerControllerSystem Component를 확인했다.
- SampleScene에 `PlayerControllerSystem` 이름의 GameObject가 없음을 확인했다.
- TimerSystem은 기존 조회 이름과 일치함을 확인했다.
- 변경은 AutoMovementIntegrationTests의 SetUp 한 줄로 제한했다.
- Play Mode 정적 예상 총수는 144개로 유지된다.
- git diff --check를 확인했다.

# 사용자 재검증

사용자가 Unity Script Compilation 및 전체 Play Mode Test 144개 성공을 확인했다. Compile과 Test 관련 예상하지 않은 Error/Warning은 없었다.

이번 수정은 Play Mode Test Setup만 변경했다. Build와 Scene 작업은 필요하지 않았다.

# 관련 작업

- `AI/90_Tasks/Prototype_3/20260907_02_Phase2Step6LifecycleRegression.md`
- `AI/90_Tasks/Prototype_3/20260904_02_Phase2ManualSteps.md`
