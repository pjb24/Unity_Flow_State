# 작업 정보

## 작업명

Prototype 3 Phase 1 Manual Steps

## 작업 일자

20260903

## 작업 담당자

AI, 사용자

## 작업 상태

진행 중

---

# 작업 목적

Prototype 3 Phase 1의 벽 충돌과 낙하 동작 안정화를 구현하고 검증하는 순서를 정의한다.

정적 검증과 Unit Test로 판정할 수 있는 항목은 자동화하고, Unity Editor에서만 가능한 Scene 구성, Build와 최종 물리 화면 확인만 사용자 수동 작업으로 분리한다.

---

# 작업 범위

- Ground와 Wall 접촉 구분
- 공중 Wall 접촉 중 아래 방향 낙하 유지
- Wall 방향 수평 이동 제한
- 벽 및 모서리 고정 방지
- Wall 이탈 후 Ground 착지 상태 복구
- 착지 후 Jump, Momentum Landing과 Normal Landing 복구
- Stage Mode와 InfiniteMode 충돌 회귀
- 기존 Ground, Platform, Goal, Pause, Result와 Retry 회귀
- Compile, 전체 Test, Build와 최소 물리 화면 검증

---

# 제외 범위

- Wall Jump
- Wall Slide 및 Wall Cling
- 벽 반동, 벽 타기와 매달리기
- Prototype 3 Phase 2 자동 이동
- Player 좌우 이동 입력 제거
- Prototype 3 Phase 3 Collectible
- Prototype 3 Phase 4 Score 및 UI 확장
- 이동, 중력과 Jump의 최종 밸런스 조정
- 신규 Map Pattern
- CharacterController 전환
- Save, Leaderboard와 신규 상용 아트

---

# 현재 기준 상태

- Prototype 2 Phase 4가 완료되었다.
- Unity Script Compilation 성공과 예상하지 않은 Error·Warning 부재가 확인되었다.
- Edit Mode Test 기준은 `222 Passed, 0 Failed`이다.
- Play Mode Test 기준은 `107 Passed, 0 Failed`이다.
- Player는 CapsuleCollider와 Rigidbody를 사용하며 Rigidbody 기본 중력을 사용하지 않는다.
- PlayerMovementSystem이 중력과 이동 결과를 계산하고 PlayerControllerSystem이 결과 속도를 Rigidbody에 적용한다.
- CollisionSystem은 아래 방향 SphereCast로 Ground 후보와 실제 접촉을 판정한다.
- 현재 Player Collider에는 Physic Material이 연결되어 있지 않다.
- Ground와 Wall을 구분하는 명시적 표면 분류 계약은 아직 없다.

---

# 검증 원칙

- 실패하는 Test 또는 변경될 기대값을 생산 코드보다 먼저 작성한다.
- 벡터, 표면 법선과 속도 보정 규칙은 순수 Edit Mode Unit Test로 우선 검증한다.
- Unity Physics가 필요한 접촉, 낙하, 모서리와 착지는 임시 GameObject를 사용하는 Play Mode Test로 검증한다.
- 생산 Scene을 요구하지 않는 물리 검증은 Test가 임시 Collider와 Rigidbody를 구성한다.
- 실제 Scene 참조와 구조는 Scene 변경 전에 생산 Scene 구조 Test로 확정한다.
- 정적 검사와 자동 Test로 판정한 수치, 상태, 호출 횟수와 Frame 경계는 수동 작업으로 반복하지 않는다.
- 수동 검증은 자연스러운 낙하가 눈에 보이는지, 떨림·고정·관통이 없는지 확인하는 최소 범위로 제한한다.
- AI는 Unity Build와 Unity Test Runner를 실행하지 않는다.
- AI는 Scene과 Input Action Asset을 수정하지 않고 필요한 경우 정확한 사용자 작업 절차만 제공한다.

---

# Step 구성

## Step 1. Wall 접촉과 낙하 동작 규칙을 확정한다

- 진행 상태: **완료**

### 결정 항목

1. Ground로 인정할 표면 법선 또는 최대 경사 기준
2. Wall로 인정할 표면 법선 기준
3. Ground와 Wall이 동시에 감지되는 모서리의 우선순위
4. Wall 방향 수평 속도의 제거 또는 제한 방식
5. Wall 접촉 중 수직 속도와 중력 유지 방식
6. Physic Material 사용 여부와 마찰 규칙
7. 벽 이탈 및 Ground 착지 후 상태 복구 시점
8. 수동 화면 검증에 사용할 최소 벽 시나리오

### 권장 기본 원칙

- Ground는 위쪽을 향하는 유효한 표면만 인정한다.
- Wall 접촉만으로 `IsGrounded`가 true가 되지 않는다.
- Ground와 Wall이 동시에 존재하면 실제 Ground 접촉을 우선하되 Wall 방향 침투 속도만 제한한다.
- Wall 접촉 중 현재의 아래 방향 속도와 중력 누적을 유지한다.
- Wall 방향 성분만 제거하고 벽에서 멀어지는 성분은 허용한다.
- 별도의 Wall Slide 속도, Wall Jump 힘과 Wall 체공 시간을 추가하지 않는다.
- Physic Material은 코드 및 Collider 설정만으로 고정 문제가 해결되지 않을 때만 도입한다.

### 확정 규칙

1. Ground는 유효하고 정규화할 수 있는 표면 법선과 `Vector3.up` 사이의 각도가 45도 이하인 표면으로 분류한다.
   - 45도 경계는 Ground에 포함한다.
   - zero, NaN 또는 Infinity 성분이 있는 법선은 Ground로 분류하지 않는다.
   - Step 2에서 기존 경사 기준이 확인되면 회귀 방지를 위해 기존 값과 비교하여 재검토한다.
2. Wall은 유효하고 정규화할 수 있는 표면 법선과 `Vector3.up` 사이의 각도가 80도 이상 100도 이하인 표면으로 분류한다.
   - 80도와 100도 경계는 Wall에 포함한다.
   - 45도 초과 80도 미만의 급경사와 100도 초과 표면은 Ground와 Wall 어느 쪽으로도 분류하지 않는다.
   - Wall 접촉만으로 `IsGrounded`를 true로 만들지 않는다.
3. Ground와 Wall이 동시에 감지되면 실제 Ground 접촉을 Ground 상태 판정에서 우선한다.
   - Wall 접촉 정보는 함께 유지하되 Grounded인 동안 Ground 수평 이동을 우선하여 Wall 속도 제한을 적용하지 않는다.
   - 모서리에서 Ground 접촉이 사라져 공중 상태가 되면 Wall 안쪽 속도 제한을 즉시 적용한다.
   - Ground와 Wall 중 하나의 접촉 정보를 선택 과정에서 버리지 않는다.
4. 공중 Wall 접촉 중에는 Wall 안쪽으로 향하는 수평 속도 성분만 제거한다.
   - Wall과 평행한 수평 성분과 Wall에서 멀어지는 수평 성분은 유지한다.
   - 여러 Wall이 동시에 감지되면 모든 활성 Wall의 침투 제한을 결정적으로 적용한다.
   - 양쪽 Wall 또는 모서리에서 허용 가능한 수평 성분이 없으면 수평 속도를 0으로 만들되 수직 속도는 변경하지 않는다.
   - 별도의 벽 반동 속도를 추가하지 않는다.
5. Wall 접촉은 수직 속도와 기존 중력 계산을 변경하지 않는다.
   - 상승 중에는 기존 Jump 수직 속도를 유지한다.
   - 하강 중에는 기존 하강 속도와 중력 누적을 유지한다.
   - 별도의 Wall Slide 속도, 체공 시간 또는 낙하 제한값을 추가하지 않는다.
6. Phase 1의 기본 구현에는 신규 Physic Material을 사용하지 않는다.
   - 자동 Physics Test와 생산 Scene 정적 검사에서 마찰로 인한 고정이 남는다는 근거가 확인된 경우에만 Player Collider용 마찰 0 Physic Material을 조건부로 검토한다.
   - Step 10 화면 검증에서 우측 입력 중 모서리 고정과 입력 해제 후 느린 하강이 재현되어 조건부 도입 기준을 충족했다.
   - Player Collider에는 정적·동적 마찰 0과 Minimum 결합을 사용하는 `PlayerZeroFriction`을 적용한다.
7. Wall 접촉 정보는 접촉이 사라진 첫 물리 단계에 즉시 해제한다.
   - 실제 Ground 접촉이 확인된 첫 물리 단계에 Grounded로 전환하고 Landing을 한 번만 판정한다.
   - 별도의 Wall 이탈 유예 시간을 추가하지 않는다.
   - Retry와 새로운 Run에서는 이전 Wall 및 Landing 상태를 초기화한다.
8. 최소 수동 화면 검증은 Stage Mode와 InfiniteMode의 대표적인 기존 수직 벽 하나에서 각각 한 번 수행한다.
   - 공중에서 벽에 접근하여 벽 방향 이동을 유지한 상태로 자연스럽게 낙하하는지 확인한다.
   - 지속 고정, 관통과 눈에 띄는 모서리 떨림이 없는지 확인한다.
   - Ground 착지 후 Jump와 Momentum Landing을 각각 한 번 수행할 수 있는지 확인한다.
   - 정확한 속도, 위치, 법선, Frame 수와 호출 횟수는 수동으로 판정하지 않고 자동 Test 결과를 사용한다.

### 정적 검증

- 확정 규칙이 CollisionSystem, PlayerMovementSystem과 PlayerControllerSystem의 책임 경계를 침범하지 않는지 확인한다.
- Phase 2 자동 이동과 입력 제거가 섞이지 않았는지 확인한다.
- 미정 수치가 근거 없이 생산 코드에 들어가지 않았는지 확인한다.

### 수동 작업

- 사용자는 AI가 제시하는 선택지와 장단점을 검토하고 위 8개 규칙을 결정한다.
- Unity Editor, Scene, Build와 Test Runner 작업은 수행하지 않는다.

### 완료 조건

- [x] Ground와 Wall 분류 규칙이 확정되었다.
- [x] Wall 접촉 중 허용·제한할 속도 성분이 확정되었다.
- [x] Physic Material과 최소 수동 검증 범위가 확정되었다.

## Step 2. 현재 충돌 및 이동 경로를 정적으로 조사한다

- 진행 상태: **완료**

### AI 작업

- Player CapsuleCollider, Rigidbody, Constraints와 Layer 설정을 조사한다.
- CollisionSystem의 SphereCast, 자기 Collider 제외와 Ground 후보 선택 경로를 조사한다.
- PlayerMovementSystem의 중력, 수평 이동, Jump와 Landing 상태 전환 경로를 조사한다.
- PlayerControllerSystem의 Rigidbody 속도 적용과 Pause·Resume 복원 경로를 조사한다.
- Stage, Platform, Goal Collider와 Infinite Pattern의 벽 후보를 조사한다.
- 기존 Unit 및 Integration Test의 재사용 지점과 누락 시나리오를 정리한다.

### 정적 조사 결과

#### Player 충돌 데이터와 CollisionSystem

- `PlayerCollisionState`는 `IsGrounded`, `GroundDistance`, `ContactPoint`와 `SurfaceNormal`만 제공한다.
- Wall 접촉 여부, Wall 접촉점, Wall 법선과 여러 동시 접촉을 표현하는 데이터는 없다.
- `CollisionSystem`은 `GroundCheck` 위치에서 `Vector3.down` 방향으로 두 번의 `Physics.SphereCastNonAlloc`을 수행한다.
  - `_groundedDistance` 범위의 결과로 실제 Ground 접촉 여부를 생성한다.
  - `_groundPredictionDistance` 범위의 결과로 가장 가까운 착지 후보 거리, 접촉점과 법선을 생성한다.
- 두 Query는 별도 고정 크기 16 Hit Buffer를 사용하고 `QueryTriggerInteraction.Ignore`를 적용한다.
- Hit 선택은 Player Collider 자신, 같은 Transform과 자식 Collider를 제외한 뒤 가장 가까운 Hit를 사용한다.
- 현재 Hit 선택에는 표면 법선 분류가 없으므로 `_groundLayer`에 포함되고 아래 방향 SphereCast에 검출된 모든 유효 Hit가 Ground 후보가 된다.
- `CollisionSystem`은 충돌 Callback을 사용하지 않으며 측면 Wall 접촉을 별도로 수집하지 않는다.
- Ground 후보가 없으면 `GroundDistance`는 `float.PositiveInfinity`, 접촉점은 `Vector3.zero`, 법선은 `Vector3.up`으로 초기화된다.

#### Player 이동, 중력과 Landing 경로

- `PlayerMovementSystem.FixedUpdate`는 실행 중이고 Pause가 아닐 때 입력, 최신 충돌 상태와 Rigidbody 현재 속도를 수집한다.
- 수평 속도는 `PlayerMovementMath.CalculateHorizontalSpeed`가 현재 X 속도, 수평 입력, Ground 여부와 가속도 설정으로 계산한다.
- 수직 속도는 Rigidbody 현재 Y 속도에서 시작한다.
  - Jump가 시작되면 `JumpFeature`가 계산한 초기 수직 속도로 교체한다.
  - Ground 접촉 중이면 0으로 만든다.
  - 공중이면 PlayerMovementSystem이 소유한 중력 가속도를 매 FixedUpdate에 누적한다.
- 현재 이동 계산에는 Wall 접촉 입력과 Wall 방향 수평 속도 보정이 없다.
- `PlayerMovementSystem`은 Ground 이탈을 확인한 Jump Sequence가 하강 중 실제 Ground에 접촉했을 때 Momentum Landing을 먼저 시도하고, 성공하지 않으면 Normal Landing을 시도한다.
- Landing 성공 시 수직 속도를 0으로 만들고 Jump, Momentum Landing과 Normal Landing 상태를 한 번의 착지로 종료한다.
- 계산 결과는 `PlayerMovementResult`로 생성되며 Z 속도는 항상 0이다.

#### Rigidbody 적용, Pause, Resume와 Retry 경로

- `PlayerControllerSystem`은 계산된 전체 속도를 Rigidbody `linearVelocity`에 적용하고 X 속도 변화로 수평 가속도를 기록한다.
- 초기화 시 Rigidbody 기본 중력을 끄고 `FreezePositionZ`와 모든 회전 Freeze를 추가하며 시작 위치에서 선형·각속도를 0으로 초기화한다.
- Pause는 Rigidbody 속도, 수평 가속도와 Constraints를 저장한 뒤 속도를 0으로 만들고 `FreezeAll`을 적용한다.
- Resume은 Pause 직전 Constraints, 속도와 수평 가속도를 복원한다.
- 종료는 PlayerMovementSystem의 Jump와 Landing 상태 및 Runtime Data를 초기화하고 PlayerControllerSystem의 선형·각속도를 0으로 만든다.
- Retry는 기존 Run을 종료한 뒤 새 Runtime Data를 생성하고 PlayerControllerSystem, CollisionSystem과 PlayerMovementSystem을 다시 초기화한다.

#### 기존 Test 재사용 지점

- `PlayerMovementMathTests`는 점프 초기 속도, Ground와 Air 수평 가속도 및 수평 가속도 계산의 순수 Unit Test 위치로 재사용할 수 있다.
- `JumpFeatureTests`, `MomentumLandingFeatureTests`와 `NormalLandingFeatureTests`는 Jump 가능 상태, 착지 후보 거리, 단일 Landing 및 착지 후 다음 Jump 계약을 검증한다.
- `StageCollisionConfigurationTests`는 생산 Scene의 Ground, Platform, Goal Layer·Collider·Trigger와 CollisionSystem 참조 및 실제 Ground 접촉을 검증한다.
- `PlayerJumpIntegrationTests`와 `MomentumLandingIntegrationTests`는 생산 Scene의 Jump 높이, 공중 재점프 방지, Momentum Landing과 Normal Landing 복구를 검증한다.
- `GamePauseOrchestrationTests`와 `GameLifecycleIntegrationTests`는 Pause·Resume 물리 상태 보존, 종료와 Retry의 독립 Runtime 상태를 검증한다.
- `StageGoalIntegrationTests`와 `InfiniteModeIntegrationTests`는 Stage Goal 단일 종료, InfiniteMode의 Goal 비활성, 종료와 연속 Run 독립성을 검증한다.
- `InfiniteMapPatternTests`는 임시 Collider를 생성하고 Pattern 이동과 Boundary Trigger 초기화를 검증하므로 Phase 1 임시 Physics Fixture 구성 방식을 재사용할 수 있다.

#### 확인된 Test 누락 시나리오

- 표면 법선을 Ground, Wall 또는 어느 쪽도 아닌 표면으로 분류하는 순수 Unit Test가 없다.
- zero, NaN, Infinity 및 정규화되지 않은 법선 Test가 없다.
- Wall 접촉이 `IsGrounded`를 만들지 않는 CollisionSystem Test가 없다.
- Wall 접촉 데이터의 수집, 좌우 Wall 대칭, 동시 Wall과 모서리 접촉 Test가 없다.
- Wall 안쪽 수평 성분만 제거하고 수직 속도와 중력을 유지하는 Unit Test가 없다.
- 임시 Rigidbody, CapsuleCollider, Ground와 Wall을 함께 사용하는 Wall 낙하 Physics Fixture Test가 없다.
- Wall 이탈 후 Ground 착지, Landing 단일 실행과 다음 Jump를 하나의 흐름으로 검증하는 Test가 없다.
- Wall 상태가 Pause, Retry와 연속 Run 사이에 남지 않는 Test가 없다.

#### 원인 후보 분리 결과

- 표면 분류: 현재 명시적 법선 분류와 Wall 상태가 없다는 사실이 확인되었다.
- 속도 보정: 현재 Wall 접촉을 입력으로 받는 수평 속도 보정이 없다는 사실이 확인되었다.
- Rigidbody 제약: Runtime 코드가 Z 위치와 회전을 제한하지만 X와 Y 위치는 제한하지 않는다는 사실이 확인되었다.
- 마찰: 생산 Scene의 Physic Material 연결과 실제 Collider 마찰 설정은 Scene 확인 전에는 판정하지 않는다.
- 물리 접촉: 현재 Wall에서의 실제 고정 현상이 마찰, 접촉 법선, 속도 재적용 중 어느 하나로 발생한다고 정적 코드만으로 단정하지 않는다.

#### 변경 책임 확정

- 표면 법선 유효성 검사와 Ground·Wall 분류 기준은 중복되지 않는 하나의 순수 계산 위치에서 소유한다.
- `CollisionSystem`은 Physics 접촉을 수집하고 분류하여 Ground와 Wall 결과 데이터를 제공한다.
- `PlayerCollisionState`는 PlayerMovementSystem이 필요한 최소 Wall 접촉 결과를 표현하도록 확장 대상이 된다.
- Wall 안쪽 수평 성분 제거는 `PlayerMovementMath`의 순수 계산 책임으로 추가한다.
- `PlayerMovementSystem`은 CollisionSystem 결과를 사용해 계산된 수평 속도에 Wall 제한을 반영하고 기존 수직 속도와 중력 계산을 유지한다.
- `PlayerControllerSystem`은 보정된 최종 속도를 적용하는 현재 책임을 유지하며 Wall 판정이나 이동 보정을 추가하지 않는다.
- Scene 변경과 Physic Material 추가는 현재 조사만으로 필요성이 입증되지 않았으므로 구현 대상에 포함하지 않는다.

### 생산 Scene YAML 정적 검사 결과

- Build Settings에는 `Assets/Scenes/SampleScene.unity` 하나가 활성 Scene으로 등록되어 있다.
- Player
  - Layer는 Default 0이고 Tag는 Untagged이다.
  - Transform 위치는 `(0, 1.5, 0)`이고 Scale은 `(1, 1, 1)`이다.
  - CapsuleCollider는 Enabled, Non-Trigger, Y축 방향, Center `(0, 0, 0)`, Radius `0.5`, Height `2`이다.
  - CapsuleCollider에 Physic Material이 연결되어 있지 않다.
  - Rigidbody는 Mass `1`, Linear Damping `0`, Angular Damping `0.05`, Use Gravity false, Is Kinematic false이다.
  - Rigidbody Interpolate는 Interpolate이고 Collision Detection은 Continuous이다.
  - Rigidbody Constraints 값은 `120`으로 Freeze Position Z와 Freeze Rotation X, Y, Z에 해당하며 X와 Y 위치는 고정하지 않는다.
- GroundCheck
  - Player의 자식이며 Local Position은 `(0, -0.74, 0)`이다.
- CollisionSystem
  - `_playerCollider`는 Player CapsuleCollider를 참조한다.
  - `_groundCheck`는 Player 자식 GroundCheck를 참조한다.
  - `_groundLayer` 값은 `64`로 Ground Layer 6만 포함한다.
  - Ground Check Radius는 `0.25`, Grounded Distance는 `0.05`, Ground Prediction Distance는 `3`이다.
- Stage Mode Terrain
  - `World/StageModeRoot/Terrain/Ground`, `Platform_01`과 `Platform_02`는 Ground Layer 6의 Enabled Non-Trigger BoxCollider이다.
  - 세 Collider 모두 Physic Material이 연결되어 있지 않다.
  - Ground Scale은 `(40, 1, 4)`, Platform Scale은 각각 `(4, 1, 4)`이다.
  - `Platform_01`은 Local Position `(6, 1, 0)`, `Platform_02`는 `(12, 2, 0)`이다.
- Stage Goal
  - `World/StageModeRoot/Goal`은 Default Layer 0의 Enabled Trigger BoxCollider이다.
  - Goal Collider에 Physic Material이 연결되어 있지 않으며 Player CapsuleCollider 참조가 유지되어 있다.
- InfiniteMode
  - `InfiniteModeRoot`는 Scene 기본 상태에서 비활성이고 그 아래 `InfiniteMapPattern`에 `Pattern_0`과 `Pattern_1`이 연결되어 있다.
  - 두 Pattern의 `Terrain/Ground`, `Platform_01`과 `Platform_02`는 Stage Mode와 같은 Ground Layer 6의 Enabled Non-Trigger BoxCollider이며 Physic Material이 없다.
  - `Pattern_0`의 위치는 `(0, 0, 0)`, `Pattern_1`의 위치는 `(44, 0, 0)`이다.
  - 각 `AdvanceBoundary`는 Default Layer 0의 Enabled Trigger BoxCollider이고 Player CapsuleCollider와 InfiniteMapPattern 참조 및 Boundary ID 0, 1이 유지되어 있다.
- Wall 후보
  - 별도 이름의 Wall GameObject나 전용 Wall Collider는 Scene YAML에 없다.
  - Stage Mode의 기존 `Platform_01`과 `Platform_02` BoxCollider 측면이 수직 Wall 접촉 후보이다.
  - InfiniteMode의 각 Pattern에 있는 `Platform_01`과 `Platform_02` BoxCollider 측면도 같은 수직 Wall 접촉 후보이다.
  - `AdvanceBoundary`와 Goal은 Trigger이므로 Wall 접촉 후보가 아니다.
- Project Physics의 Default Physic Material도 연결되어 있지 않고 Layer Collision Matrix는 모든 Layer 조합을 허용한다.
- Scene YAML과 ProjectSettings는 읽기만 했으며 Scene과 Serialized Field를 수정하지 않았다.

### 정적 검증

- 벽 고정의 가능한 원인을 마찰, 표면 분류, 속도 보정과 Rigidbody 제약으로 분리한다.
- 변경 책임이 CollisionSystem, PlayerMovementSystem 또는 PlayerControllerSystem 중 어디에 속하는지 확정한다.
- Scene 변경 없이 재현 가능한 부분을 식별한다.

### 수동 작업

- 없음.

### 완료 조건

- [x] 현재 코드의 충돌 데이터와 이동 적용 경로가 확인되었다.
- [x] Test 우선 변경 지점과 책임 System이 확정되었다.
- [x] 생산 Scene의 Collider, Rigidbody, Layer, Physic Material과 Wall 후보 확인 결과가 기록되었다.

## Step 3. 표면 분류와 CollisionSystem 접촉 분리를 Test 우선으로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- 위쪽 Ground 법선
- 수직 Wall 법선
- 경계 각도 바로 안쪽과 바깥쪽
- 정규화되지 않은 법선
- zero, NaN과 Infinity 법선
- Ground와 Wall 어느 쪽에도 속하지 않는 표면

### 구현 원칙

- Unity Physics Query와 독립적인 순수 계산 API를 우선한다.
- Ground와 Wall 분류 기준의 소유 위치를 한 곳으로 제한한다.
- 허용 각도 또는 dot 기준은 중복 Serialized Field로 두지 않는다.

### 정적 검증

- Culture, Frame과 Scene에 의존하지 않는 결정적 Test인지 확인한다.
- 경계값과 유효하지 않은 값이 포함되었는지 확인한다.

### CollisionSystem Test 우선 항목

- 바닥 SphereCast는 Ground로 판정
- 수직 벽 Hit는 Ground로 판정하지 않음
- Wall만 접촉한 상태의 `IsGrounded == false`
- Ground 후보 거리와 Wall 접촉 정보의 혼용 방지
- 자기 Collider 및 자식 Collider 제외 유지
- Trigger 무시 유지
- Hit Buffer 경계와 가장 가까운 유효 Ground 선택 유지

### CollisionSystem 구현 원칙

- CollisionSystem은 접촉을 분류하고 결과 데이터만 제공한다.
- CollisionSystem이 Rigidbody 속도나 Player Transform을 직접 변경하지 않는다.
- Momentum Landing Window와 이동 행동은 결정하지 않는다.

### 구현 결과

- `PlayerSurfaceMath`를 Runtime Core에 추가하여 법선 유효성, Ground 45도 이하와 Wall 80~100도 분류를 한 곳에서 관리한다.
- `PlayerWallContactState`를 Runtime Core에 추가하여 좌측 및 우측 Wall 접촉 여부와 각 표면 법선을 값 형식으로 제공한다.
- 기존 `PlayerCollisionState` 생성자 호환을 유지하면서 Wall 접촉 결과를 추가했다.
- `CollisionSystem`의 Ground SphereCast Hit 선택에 Ground 표면 분류를 적용했다.
- `CollisionSystem`이 Collision Callback의 고정 크기 Contact Buffer로 Wall 접촉을 수집하고 Collider별 접촉 상태를 관리하도록 확장했다.
- Wall 접촉이 사라지거나 해당 Collider의 접촉 법선이 Wall 범위를 벗어나면 저장된 Wall 접촉을 제거한다.
- Ground와 Wall 결과는 하나를 버리지 않고 같은 `PlayerCollisionState`에 함께 제공한다.
- Wall 접촉 수집은 Rigidbody 속도, Player Transform, 이동 상태와 Landing 상태를 변경하지 않는다.

### 추가 Test

- Edit Mode `PlayerSurfaceMathTests`
  - 위쪽 Ground 법선과 45도 경계
  - Ground 경계 바깥쪽
  - Wall 80도, 90도와 100도 경계
  - Wall 경계 바깥쪽
  - Ground와 Wall 어느 쪽도 아닌 급경사
  - 정규화되지 않은 법선
  - zero, NaN과 Infinity 법선
- Play Mode `CollisionSystemContactTests`
  - 임시 Ground 접촉은 Ground만 제공
  - 임시 수직 Wall 접촉은 Wall만 제공하고 `IsGrounded == false` 유지
  - Ground와 Wall 동시 접촉 결과 유지
  - Trigger Ground 후보 무시
  - Player 자식 Collider 제외 유지
  - Hit Buffer 크기 16 경계에서 가장 가까운 유효 Ground 선택
- 신규 Script와 Test의 `.meta` 파일을 함께 추가했다.

### 검증 중 수정

- 최초 Play Mode 실행에서 `RefreshCollisionState_TriggerBelow_IsIgnored`가 예상 `Infinity`, 실제 `0.5`로 실패했다.
- 실제 값 `0.5`는 원점에 있는 생산 `SampleScene` Ground의 상단 높이와 일치했다.
- 임시 Fixture가 원점에서 생산 Scene 지형과 함께 Physics Query에 포함된 테스트 격리 문제로 확인했다.
- 모든 `CollisionSystemContactTests` Fixture를 생산 Scene 지형과 떨어진 `(10000, 1000, 0)` 기준 위치로 이동했다.
- 생산 코드와 Scene은 이 실패에 대응하여 변경하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation에 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test `235 Passed, 0 Failed`를 확인했다.
- Edit Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test `113 Passed, 0 Failed`를 확인했다.
- Play Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.

### 수동 작업

- 사용자는 구현 후 Unity Script Compilation을 확인한다.
- 사용자는 지정된 Edit Mode 표면 분류 Test와 Play Mode CollisionSystem Test를 실행한다.

### 완료 조건

- [x] `PlayerSurfaceMathTests`가 포함된 전체 Edit Mode Test가 통과한다.
- [x] `CollisionSystemContactTests`가 포함된 전체 Play Mode Test가 통과한다.
- [x] 기존 `StageCollisionConfigurationTests`가 포함된 전체 Play Mode Test가 통과한다.
- [x] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.

## Step 4. Wall 방향 이동 제한과 중력 보존을 Unit Test로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Wall 방향 수평 성분만 제한
- Wall에서 멀어지는 수평 성분 유지
- 아래 방향 수직 속도 유지
- 중력 누적 후 수직 속도 감소 유지
- 위로 이동 중 Wall 접촉 시 Jump 수직 속도 보존
- Ground와 Wall 동시 접촉 시 Ground 이동 유지
- 양쪽 Wall 또는 모서리 입력의 결정적 결과

### 구현 원칙

- 속도 벡터 보정은 가능한 한 순수 계산으로 분리한다.
- PlayerMovementSystem이 이동 결과를 계산하고 PlayerControllerSystem은 결과를 적용한다.
- 벽 접촉을 이유로 전체 속도를 zero로 만들지 않는다.
- Frame rate에 종속된 임의 보정과 Transform 직접 이동을 추가하지 않는다.

### 구현 결과

- `PlayerMovementMath.CalculateVerticalSpeed`로 기존 Ground 수직 속도 초기화와 공중 중력 누적 계산을 순수 함수로 분리했다.
- `PlayerMovementMath.ConstrainVelocityByWalls`로 좌측 또는 우측 Wall 안쪽을 향하는 X 속도만 제거한다.
- Wall과 반대 방향으로 이동하는 X 속도는 유지한다.
- 양쪽 Wall이 동시에 감지되면 어느 방향의 X 속도도 Wall 안쪽 속도이므로 0으로 제한한다.
- Wall 속도 보정은 전달된 Vector의 Y와 Z 성분을 변경하지 않는다.
- `PlayerMovementSystem`은 Jump, 중력과 Landing 계산을 마친 최종 이동 결과에 Wall 제한을 적용한다.
- Ground와 Wall이 동시에 감지되어도 기존 Ground 및 Landing 상태 계산은 유지하고 Wall 안쪽 X 속도만 제한한다.
- `PlayerControllerSystem`은 계산된 최종 속도를 Rigidbody에 적용하는 기존 책임을 유지한다.
- Transform 직접 이동, Wall 반동, Wall Slide, Wall Jump와 신규 Serialized Field를 추가하지 않았다.

### 추가 Test

- `PlayerMovementMathTests`
  - 우측 Wall 방향 속도 제거
  - 우측 Wall에서 멀어지는 속도 유지
  - 좌측 Wall 방향 속도 제거
  - Wall 접촉 중 아래 방향 수직 속도 유지
  - Wall 접촉 중 위 방향 수직 속도 유지
  - 양쪽 Wall의 좌우 속도 제한과 수직 속도 유지
  - Ground와 Wall 동시 접촉 중 Wall에서 멀어지는 Ground 이동 유지
  - 공중 중력 누적 후 수직 속도 감소
  - Ground 접촉 중 수직 속도 0 유지

### 사용자 검증 결과

- Unity Script Compilation에 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test `245 Passed, 0 Failed`를 확인했다.
- Edit Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test `113 Passed, 0 Failed`를 확인했다.
- Play Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.

### 수동 작업

- 사용자는 Script Compilation과 지정된 Edit Mode Test만 실행한다.

### 완료 조건

- [x] 변경된 `PlayerMovementMathTests`가 포함된 전체 Edit Mode Test가 통과한다.
- [x] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.
- [x] 벽 방향 이동 제한과 수직 낙하가 동시에 성립한다.
- [x] Jump와 Ground 이동 계산에 회귀가 없다.

## Step 5. 임시 Physics Fixture로 벽 낙하 통합 Test를 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- 공중 Player가 수직 벽에 접근해 충돌
- 여러 FixedUpdate 동안 Y 위치가 계속 감소
- 벽 접촉 중 Player가 일정 높이에 고정되지 않음
- 벽 Collider 관통 없음
- 벽 하단 또는 모서리 이탈 후 낙하 지속
- 좌측 및 우측 벽 방향의 대칭 동작
- 큰 수평 속도와 작은 수평 속도의 동일한 규칙

### 구현 원칙

- Test가 임시 Rigidbody, CapsuleCollider, Ground와 Wall Collider를 생성하고 종료 시 제거한다.
- 생산 Scene 위치나 사용자의 조작 속도에 의존하지 않는다.
- 위치 변화, 속도 성분과 상태를 수치로 자동 판정한다.

### 구현 결과

- 생산 Scene과 분리된 좌표 `(12000, 1000, 0)`에 임시 Player, Rigidbody, CapsuleCollider, Ground와 BoxCollider Wall을 생성하는 `WallFallPhysicsTests`를 추가했다.
- 임시 Player는 생산 Player와 동일하게 Rigidbody 기본 중력을 사용하지 않고 Z 위치와 회전을 제한하며 Continuous Collision Detection을 사용한다.
- 생산 `CollisionSystem`, `PlayerMovementMath.CalculateVerticalSpeed`와 `PlayerMovementMath.ConstrainVelocityByWalls`를 함께 사용한다.
- 각 FixedUpdate에서 중력을 누적하고 Wall 접촉 결과로 최종 속도를 보정한 뒤 Rigidbody에 적용한다.
- Wall 접촉 중 연속 Y 위치 감소, 3회 이상의 낙하 진행, 0.5 이상 높이 감소와 Collider 비관통을 수치로 판정한다.
- Wall 하단을 완전히 벗어난 뒤 Wall 접촉이 해제되고 추가 FixedUpdate에서도 Y 위치가 계속 감소하는지 판정한다.
- 오른쪽과 왼쪽 Wall을 각각 검증한다.
- 작은 접근 속도 `2`와 큰 접근 속도 `14`에 동일한 규칙을 적용한다.
- 각 Test 종료 시 이름 Prefix로 생성한 모든 임시 GameObject를 제거한다.
- 생산 Scene, 생산 Collider, Rigidbody 설정과 Physic Material은 변경하지 않았다.

### 추가 Test

- `RightWall_SmallApproachSpeed_FallsAndExits`
- `LeftWall_SmallApproachSpeed_FallsAndExits`
- `RightWall_LargeApproachSpeed_FallsWithoutPenetration`
- `LeftWall_LargeApproachSpeed_FallsWithoutPenetration`

### 사용자 검증 결과

- Unity Script Compilation에 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test `245 Passed, 0 Failed`를 확인했다.
- Edit Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test `117 Passed, 0 Failed`를 확인했다.
- Play Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.

### 수동 작업

- 사용자는 Unity Test Runner에서 지정된 Play Mode Test만 실행한다.

### 완료 조건

- [x] `WallFallPhysicsTests` 4개가 포함된 전체 Play Mode Test가 통과한다.
- [x] 벽 접촉 중 낙하와 비관통 Test가 통과한다.
- [x] Wall 하단 이탈 후 낙하 지속 Test가 통과한다.
- [x] 좌우 Wall 방향과 작은·큰 접근 속도 회귀가 통과한다.
- [x] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.

## Step 6. Wall 이탈, Landing 복구와 기존 충돌 회귀를 자동 Test로 구현한다

- 진행 상태: **완료**

### Test 우선 항목

- Wall 접촉 중 `IsGrounded == false`
- Wall에서 떨어져 Ground 접촉 시 Grounded 전환
- Normal Landing 한 번 발생
- Momentum Landing Window가 유효할 때 Momentum Landing 한 번 발생
- Normal과 Momentum Landing 동시 발생 방지
- 착지 후 다음 Jump 시작 가능
- Retry 후 이전 Wall 및 Landing 상태 제거

### 정적 검증

- Jump, MomentumLanding과 NormalLanding Feature의 기존 계약을 재사용하는지 확인한다.
- Wall 전용 Jump 또는 Landing 상태가 추가되지 않았는지 확인한다.

### 기존 충돌 회귀 Test 우선 항목

- 평지 Grounded 및 GroundDistance
- Platform 위 이동과 낙하
- 경사 또는 모서리 Ground 판정
- Goal Trigger의 Stage Clear 단일 실행
- InfiniteMode Goal 비활성 규칙
- 추락 종료 판정
- Stage 및 Infinite Retry
- Pause 중 물리 정지와 Resume 복구

### 기존 충돌 회귀 정적 검증

- 기존 Test 기대값을 삭제하거나 약화하지 않았는지 확인한다.
- Wall 안정화가 Goal, Timer, Score와 UI 책임에 영향을 주지 않는지 확인한다.

### 구현 결과

- 생산 `SampleScene`을 로드하되 Scene Object를 수정하지 않고 격리 좌표 `(14000, 1000, 0)`에 임시 Ground와 Wall을 생성하는 `WallLandingRecoveryIntegrationTests`를 추가했다.
- 생산 Player, CollisionSystem, PlayerMovementSystem, Jump, Momentum Landing, Normal Landing과 Runtime Data 흐름을 그대로 사용한다.
- 각 Test는 Ground에서 실제 Jump 입력으로 Jump Sequence를 시작한 뒤 공중에서 임시 Wall과 접촉하고 Ground에 착지한다.
- Wall만 접촉한 동안 `IsGrounded == false`인 상태를 확인한다.
- 입력이 없을 때 Normal Landing이 정확히 한 번 발생하고 Momentum Landing은 발생하지 않는지 확인한다.
- Momentum Landing Window 입력 시 Momentum Landing이 정확히 한 번 발생하고 Normal Landing은 발생하지 않는지 확인한다.
- Normal Landing 후 Wall에서 멀어져 다음 Jump를 시작할 수 있는지 확인한다.
- Wall 접촉과 Momentum Landing 이후 Pause 상태에서 Retry하여 새 Runtime Data가 생성되고 이전 Wall 및 Landing 상태가 제거되는지 확인한다.
- 각 Test 종료 시 Prefix로 생성한 임시 Ground와 Wall을 제거한다.
- 생산 Scene, Scene 참조, Collider, Rigidbody와 Physic Material은 변경하지 않았다.

### 추가 Test

- `WallExit_NormalLanding_AllowsNextJump`
- `WallExit_WindowInput_AppliesMomentumLandingOnce`
- `WallContact_PausedRetry_ClearsPreviousContactAndLandingState`

### 기존 회귀 Test 적용 범위

- `PlayerSurfaceMathTests`와 `CollisionSystemContactTests`: Ground·Wall 분류, Trigger·자기 Collider 제외와 Ground 후보 선택
- `PlayerJumpIntegrationTests`: Jump 높이, Ground 복귀와 공중 재점프 방지
- `MomentumLandingIntegrationTests`: Momentum Landing, Normal Landing과 입력 Window
- `StageCollisionConfigurationTests`: 생산 Ground와 Platform 접촉 및 Goal Trigger 분리
- `StageGoalIntegrationTests`: Goal 단일 Stage 종료와 새 Stage Play 복구
- `InfiniteModeIntegrationTests`: Infinite Goal 비활성, 추락·속도 종료, 기록과 연속 Run 독립성
- `GamePauseOrchestrationTests`와 `GameLifecycleIntegrationTests`: Pause·Resume 물리 보존, 종료와 Retry 초기화
- 기존 Test 기대값을 삭제하거나 완화하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation에 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test `245 Passed, 0 Failed`를 확인했다.
- Edit Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test `120 Passed, 0 Failed`를 확인했다.
- Play Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.

### 수동 작업

- 사용자는 지정된 Edit Mode 및 Play Mode Test만 실행한다.

### 완료 조건

- [x] `WallLandingRecoveryIntegrationTests` 3개가 포함된 전체 Play Mode Test가 통과한다.
- [x] Wall 이탈 후 착지 상태가 정상 복구된다.
- [x] 착지 후 Jump와 Momentum Landing을 다시 수행할 수 있다.
- [x] 기존 Ground, Platform과 Goal 회귀 Test가 통과한다.
- [x] Pause, Result와 Retry 흐름에 회귀가 없다.
- [x] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.

## Step 7. Stage Mode와 InfiniteMode 벽 충돌 회귀를 자동화한다

- 진행 상태: **완료**

### Test 우선 항목

- 두 Mode에서 동일한 Ground와 Wall 분류
- 두 Mode에서 Wall 접촉 중 낙하 유지
- InfiniteMode 이동 거리와 Score가 실제 최대 전진 거리만 사용
- Wall 접촉이 Infinite 종료 조건을 우회하지 않음
- Stage Goal 도달과 Infinite 추락 종료 유지
- 연속 두 Run의 충돌 및 Landing 상태 독립성

### 수동 작업

- Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error 및 Warning이 없는지 확인한다.
- Unity Test Runner에서 전체 Play Mode Test를 실행한다.
- `WallLandingRecoveryIntegrationTests` 5개를 포함하여 전체 122개가 통과하는지 확인한다.
- Test 실행 중 예상하지 않은 Error 및 Warning이 없는지 확인한다.

### 구현 결과

- 기존 Stage Mode 회귀 3개와 같은 생산 Scene 및 생산 System을 사용하는 InfiniteMode 회귀 2개를 `WallLandingRecoveryIntegrationTests`에 추가했다.
- InfiniteMode에서도 공중 Wall 접촉 중 높이가 감소하고 Ground 착지 상태가 복구되는지 검증한다.
- Wall 접촉 뒤 Player를 이전 최대 전진 위치보다 뒤로 이동시켜 `CurrentDistance`와 `CurrentScore`가 감소하거나 다시 계산되지 않는지 검증한다.
- 같은 Run에서 Player가 추락 임계값 아래로 내려가면 Wall 접촉 이력과 관계없이 InfiniteMode가 종료되는지 검증한다.
- 기존 `ProductionSceneGameModeTests`와 `InfiniteModeIntegrationTests`가 Stage Goal, Infinite 추락 종료, 기록과 연속 Run 초기화를 계속 담당한다.
- 두 Mode 모두 같은 `CollisionSystem`과 `PlayerSurfaceMath`를 사용하므로 Ground 및 Wall 분류 규칙이 Mode별로 분기되지 않음을 정적으로 확인했다.
- 임시 Fixture만 Test 실행 중 생성하며 생산 Scene은 변경하지 않았다.

### 검증 중 수정

- 전체 Play Mode Test에서 기존 `MouseClickRetry_MatchesKeyboardRetry`가 Pause 상태에 남아 1회 실패했다.
- Step 7 충돌 구현과 해당 Test 사이에는 생산 코드 및 상태 공유 경로가 없고 각 Test가 `SampleScene`을 새로 로드함을 확인했다.
- 실패 Test가 private 입력 값을 주입한 뒤 다음 Frame의 `GameSystem.Update`를 기다려 실제 Input System Point Callback이 주입 위치를 덮어쓸 수 있는 순서 경쟁을 확인했다.
- 마우스 Pause Retry Test 2개가 입력 주입 직후 생산 `ProcessPausedInput` 경로를 동기적으로 한 번 처리하도록 변경했다.
- Retry 상태, UI 상태, 독립 Runtime Data와 입력 단일 소비 기대값은 유지했으며 생산 코드와 Scene은 변경하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation에 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test `245 Passed, 0 Failed`를 확인했다.
- Edit Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test `122 Passed, 0 Failed`를 확인했다.
- Play Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.

### 완료 조건

- [x] `WallLandingRecoveryIntegrationTests` 5개가 포함된 전체 Play Mode Test 122개가 통과한다.
- [x] Stage와 InfiniteMode 벽 충돌 회귀가 통과한다.
- [x] Mode별 종료와 기록 흐름이 유지된다.
- [x] Unity Script Compilation 및 Test 실행에 예상하지 않은 Error와 Warning이 없다.

## Step 8. 생산 Scene 변경 필요성을 Test와 정적 검사로 판정한다

- 진행 상태: **완료**

### AI 작업

- 생산 Scene의 Player Collider, Rigidbody, Ground Layer와 벽 후보 Collider를 YAML로 검사한다.
- 임시 Physics Fixture Test와 생산 Scene 구조를 대조한다.
- 코드와 Test만으로 완료 조건을 충족하면 Scene 변경 불필요를 기록한다.
- Physic Material 또는 Collider 설정 변경이 필요한 경우 먼저 생산 Scene 구조 Test와 정확한 사용자 작업 명세를 작성한다.

### 조건부 사용자 Scene 작업

- AI가 Scene 변경 필요를 정적 근거와 실패 Test로 확정한 경우에만 수행한다.
- 사용자는 AI가 지정한 Object, Component, Field와 값만 Unity Editor에서 변경한다.
- Scene 저장 후 닫았다가 다시 열어 참조 유지를 확인한다.
- Missing Script, Missing Reference와 의도하지 않은 Rigidbody·Collider 변경이 없는지 확인한다.

### 정적 판정 결과

- `Assets/Scenes/SampleScene.unity`를 YAML로 다시 검사했으며 Scene 파일은 수정하지 않았다.
- 생산 Player는 Layer 0의 활성 Object이며, Enabled Non-Trigger CapsuleCollider의 Radius `0.5`, Height `2`, Direction Y와 Material 미지정 상태를 유지한다.
- 생산 Rigidbody는 Use Gravity false, Is Kinematic false, Interpolate, Continuous Collision Detection과 Constraints `120`을 사용한다.
  - Constraints `120`은 Position Z와 Rotation X, Y, Z만 고정하므로 Wall 접촉 중 Y 낙하를 막지 않는다.
- 생산 CollisionSystem의 Player Collider 및 GroundCheck 참조가 유지되어 있고 Ground Layer Mask `64`, Radius `0.25`, Grounded Distance `0.05`, Prediction Distance `3`이 Fixture Test와 일치한다.
- Stage 및 InfiniteMode의 Ground와 Platform은 Ground Layer 6의 Enabled Non-Trigger BoxCollider이고 Physic Material이 지정되지 않았다.
- Platform 측면은 별도 Scene Wall Component 없이도 수직 법선 접촉을 제공하며, Goal과 AdvanceBoundary는 Trigger이므로 Wall 수집 대상에서 제외된다.
- `WallFallPhysicsTests`는 생산 Player와 같은 CapsuleCollider, 기본 중력 비활성, Continuous Collision Detection 및 Z·회전 고정 조건을 재현한다.
- `WallLandingRecoveryIntegrationTests`는 생산 Player와 생산 System을 직접 사용하고 Ground Layer 6의 임시 BoxCollider Ground 및 Wall만 격리 좌표에 생성한다.
- 전체 Play Mode Test `122 Passed, 0 Failed`에서 좌우 Wall 낙하, 비관통, Wall 이탈, Landing 복구, Stage·InfiniteMode 종료와 반복 Run 회귀가 통과했다.
- 코드 및 Test만으로 Phase 1의 현재 완료 조건을 충족하며 마찰 고정이나 생산 Collider 설정 결함을 나타내는 실패 근거가 없다.

### Scene 변경 판정

- 최초 자동 검증 단계에서는 생산 Scene 변경을 불필요로 판정했다.
- Step 10 화면 검증에서 우측 입력 중 모서리 고정과 입력 해제 후 느린 하강이 확인되어 마찰에 대한 실제 실패 근거가 추가되었다.
- Grounded 모서리의 속도 제한 해제와 함께 Player CapsuleCollider에 마찰 0 Physic Material을 적용해야 한다.
- Rigidbody, Ground 및 Platform Collider와 다른 Scene Field는 변경하지 않는다.

### 사용자 Scene 작업 명세

1. Unity Editor Project 창에서 `Assets/PhysicsMaterials/PlayerZeroFriction`을 확인한다.
2. `SampleScene`의 Hierarchy에서 루트 `Player`를 선택한다.
3. Inspector의 `Capsule Collider` Component에서 `Material` Field에 `PlayerZeroFriction`을 지정한다.
4. 다른 Capsule Collider Field, Rigidbody, Transform과 자식 Object는 변경하지 않는다.
5. Scene을 저장한 뒤 닫았다가 다시 열어 Material 참조가 유지되는지 확인한다.
6. Console에 Missing Script 또는 Missing Reference가 없고 의도하지 않은 Scene 변경이 없는지 확인한다.

### 완료 조건

- [x] 화면 실패 근거에 따라 Player Collider의 마찰 0 Material 적용 필요성이 확정되었다.
- [x] 사용자 Scene 작업 명세를 Player CapsuleCollider의 Material Field 하나로 제한하여 작성했다.
- [x] 사용자가 Material을 연결했고 생산 Scene 설정 Test 통과로 참조 유지를 확인했다.

## Step 9. 전체 Compile과 자동 회귀 Test를 수행한다

- 진행 상태: **완료**

### AI 정적 검증

- 신규 Script와 Test의 `.meta` 및 GUID를 검사한다.
- Test Ignore, 삭제, 임의 통과와 기대값 약화를 검사한다.
- Collider, Rigidbody와 Serialized Reference를 검사한다.
- Update 및 FixedUpdate의 반복 Log와 불필요한 할당을 검사한다.
- Phase 2 자동 이동, 입력 제거와 Phase 3 Collectible이 포함되지 않았는지 확인한다.
- 관련 Feature, System과 Roadmap 문서가 구현과 일치하는지 확인한다.

### 사용자 자동 검증

1. Unity Editor에서 Script Compilation 성공을 확인한다.
2. 예상하지 않은 Compile Error와 Warning이 없는지 확인한다.
3. Unity Test Runner에서 전체 Edit Mode Test를 실행한다.
4. Unity Test Runner에서 전체 Play Mode Test를 실행한다.
5. Passed, Failed와 전체 Test 수를 기록한다.
6. Test 실행 중 예상하지 않은 Error와 Warning이 없는지 확인한다.

### AI 정적 검증 결과

- 신규 Runtime Script 2개와 Test Script 4개 모두 대응하는 `.meta` 파일과 형식에 맞는 32자리 GUID를 보유한다.
- Assets, Packages와 ProjectSettings 범위에서 신규 GUID를 포함한 `.meta` GUID 중복이 없음을 확인했다.
- 전체 Test에서 `Ignore`, `Explicit`, `Assert.Pass`, `Assert.Ignore`와 조건부 컴파일을 이용한 Test 제외가 없음을 확인했다.
- 기존 Test를 삭제하지 않았고 Pause 마우스 Test 수정 후에도 Playing 및 StageHud 전환, 독립 Runtime Data 생성과 입력 단일 소비 기대값을 유지했다.
- CollisionSystem은 기존 Serialized Player Collider, GroundCheck, Ground Layer와 거리 설정을 유지하며 신규 Serialized Field를 추가하지 않았다.
- 생산 Scene YAML의 Player Collider, Rigidbody, CollisionSystem 참조와 Stage 및 InfiniteMode Collider 구성이 유효함을 확인했다.
- Collision Callback은 사전 할당된 Contact Buffer와 Dictionary를 재사용하고 Ground Query는 기존 고정 크기 Hit Buffer를 재사용한다.
- 신규 Runtime 변경에는 `Update`, `FixedUpdate`와 Collision Callback에서 실행되는 반복 Log 또는 LINQ가 없다.
- 반복 이동 계산은 값 형식만 사용하며 매 FixedUpdate 배열, List 또는 Dictionary를 생성하지 않는다.
- Runtime 변경 범위는 표면 분류, Wall 접촉 상태, Wall 방향 속도 제한과 기존 중력 계산 분리로 한정된다.
- Phase 2 자동 이동 및 Player 좌우 입력 제거와 Phase 3 Collectible 기능은 포함하지 않았다.
- CollisionSystem, PlayerMovementSystem과 Roadmap 및 Phase 1 실행 문서의 책임과 확정 규칙이 구현과 일치한다.
- `git diff --check`가 공백 오류 없이 통과했으며 생산 Scene 파일은 변경되지 않았다.

### 사용자 검증 결과 재사용

- Step 7에서 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인했다.
- 전체 Edit Mode Test `245 Passed, 0 Failed`를 확인했다.
- 전체 Play Mode Test `122 Passed, 0 Failed`를 확인했다.
- 두 Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- Step 9 최초 완료 시점에는 Runtime 및 Test 코드 변경이 없어 위 결과를 전체 회귀 근거로 재사용했다.

### Step 10 화면 검증 후 변경

- 사용자가 벽에 공중 접촉했을 때 고정되지 않지만 Ground와 Wall이 함께 검출되는 모서리에서 우측 이동이 차단되는 현상을 확인했다.
- 원인은 `ConstrainVelocityByWalls`가 Grounded 상태에서도 Wall 안쪽 X 속도를 0으로 제한한 데 있다.
- Grounded 상태에서는 Ground 이동을 우선하여 전체 속도를 유지하고, 공중 상태에서만 기존 Wall 안쪽 X 속도 제한을 적용하도록 변경했다.
- `ConstrainVelocity_GroundedAtWallMovingIntoWall_PreservesMovement` Edit Mode Test를 추가했다.
- 정적·동적 마찰 0, Minimum 결합과 반발 0인 `PlayerZeroFriction.physicMaterial` Asset을 추가했다.
- 생산 Player Collider의 Material 설정을 검증하는 `PlayerCollider_UsesZeroFrictionMaterial` Play Mode Test를 추가했다.
- 마찰 0 Player가 우측 Wall을 향해 지속적인 속도를 받아도 접촉 중 0.5 이상 낙하하고 관통하지 않는 `RightWall_HeldInputWithZeroFriction_ContinuesFalling` Play Mode Test를 추가했다.
- 이 변경으로 기존 검증 결과는 이력으로만 유지하며 현재 코드에는 전체 Compile 및 Test 재검증이 필요하다.

### 모서리 수정 후 사용자 재검증 결과

- Unity Script Compilation에 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test `246 Passed, 0 Failed`를 확인했다.
- Edit Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test `124 Passed, 0 Failed`를 확인했다.
- Play Mode Test 실행에서 예상하지 않은 Error와 Warning이 없었다.
- 사용자가 모서리 이동과 벽 접촉 처리가 만족스럽게 해결되었음을 확인했다.

### 완료 조건

- [x] 전체 정적 검증이 통과한다.
- [x] 전체 Edit Mode 246개와 Play Mode 124개 Test가 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

## Step 10. 사용자가 Build와 최소 물리 화면을 검증하고 AI가 완료 근거를 정리한다

- 진행 상태: **완료**

### Build 전 AI 정적 확인

- Build Settings의 Scene, Windows Win64/x64 대상과 Asset 참조를 검사한다.
- 자동 Test로 판정한 수치, 상태와 Frame 경계를 수동 체크리스트에서 제외한다.

### Build 전 AI 정적 확인 결과

- `ProjectSettings/EditorBuildSettings.asset`에는 `Assets/Scenes/SampleScene.unity` 하나만 활성 Build Scene으로 등록되어 있다.
- 등록된 Scene 경로, Scene 파일, `.meta`와 GUID `99c9720ab356a0642a771bea13969a05`가 서로 일치한다.
- SampleScene의 생산 Script와 Package Script GUID가 모두 Assets 또는 설치된 Package Cache의 실제 `.meta`로 해석된다.
- SampleScene YAML에 Missing Script를 나타내는 비어 있거나 zero인 Script 참조가 없다.
- `Packages/manifest.json`과 `Packages/packages-lock.json`의 JSON 구문이 유효하다.
- ProjectSettings에 Standalone Build Target 설정이 존재한다.
- 생산 Scene과 Build Settings는 수정하지 않았다.
- Step 9 정적 검증, Script Compilation, Edit Mode `245 Passed`와 Play Mode `122 Passed`는 모서리 수정 전 기준으로 기록되어 있다.

### 최초 화면 검증 결과와 수정

- 사용자가 공중 Wall 접촉 중 Player가 벽에 고정되지 않음을 확인했다.
- Ground와 Wall이 함께 검출되는 벽 모서리에 올라갔을 때 우측 이동이 불가능한 현상을 확인했다.
- Grounded 상태에서도 Wall 안쪽 수평 속도를 제거하던 원인 코드를 수정하고 Edit Mode 회귀 Test를 추가했다.
- 우측 입력이 만드는 지속 법선 힘과 기본 Collider 마찰이 수직 낙하를 상쇄하는 두 번째 원인을 확인했다.
- Player용 마찰 0 Physic Material과 생산 Scene 연결 검증 Test를 추가했다.
- Runtime 코드가 변경되었으므로 이전 Build 결과는 최종 완료 근거로 사용하지 않고 재검증을 대기한다.

### 수정 후 사용자 검증 결과

- Unity Script Compilation과 전체 Edit Mode 246개 및 Play Mode 124개 Test가 예상하지 않은 Error와 Warning 없이 통과했다.
- 생산 Player Collider의 `PlayerZeroFriction` Material 연결이 Play Mode 설정 Test로 확인되었다.
- 사용자가 모서리 관련 이동과 Wall 접촉 처리가 만족스럽게 해결되었음을 확인했다.
- Windows Standalone Development Build에 성공했다.
- Build에서 예상하지 않은 Error와 Warning이 없었다.

### 사용자 Build 작업

1. Unity Editor에서 Windows Standalone Win64/x64 Development Build를 한 번 수행한다.
2. Build 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
3. Stage Mode에서 공중 상태로 기존 수직 벽에 접근한다.
4. 벽 방향 이동을 유지해도 Player가 벽에 고정되지 않고 아래로 떨어지는지 확인한다.
5. Ground에 착지한 뒤 Jump와 Momentum Landing을 각각 한 번 수행한다.
6. InfiniteMode에서도 같은 벽 접촉과 낙하를 한 번 확인한다.
7. Player가 벽을 관통하거나 모서리에서 지속적으로 떨리지 않는지 확인한다.
8. Wall Jump, Wall Slide 또는 의도하지 않은 체공이 생기지 않았는지 확인한다.
9. 종료 후 Build Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 수동 검증 제한

- 정확한 속도, 위치, 법선, 접촉 Frame 수와 Landing 호출 횟수는 자동 Test 결과를 사용한다.
- 사용자는 자연스러운 낙하, 지속 고정, 눈에 띄는 떨림과 관통 여부만 확인한다.
- 빠른 입력, 정밀한 타이밍과 반복 횟수를 요구하지 않는다.

### Build 화면 판정 항목

- Build 성공
- 두 Mode에서 벽 고정 없이 자연스럽게 낙하
- 착지 후 Jump와 Momentum Landing 가능
- 관통, 지속 떨림과 예상하지 않은 Error·Warning 부재

### Build 검증 후 AI 작업

- 최종 정적 검증, Compile, Test 수, Build와 최소 물리 화면 결과를 기록한다.
- Scene 변경 여부와 미해결 사항을 기록한다.
- 별도 Phase 1 Verification Result Task 문서를 작성한다.
- 모든 완료 조건 충족 시에만 Roadmap Phase 1을 `완료`로 변경한다.

### 수동 작업

- 이전 Step에서 확인하지 못한 새 수동 작업을 추가하지 않는다.

### 이번 Step의 사용자 수동 작업

1. `File > Build Profiles` 또는 사용 중인 Unity 버전의 `File > Build Settings`를 연다.
2. Platform이 Windows Standalone이고 Architecture가 `Intel 64-bit (x86_64)`인지 확인한다.
3. `Development Build`를 활성화하고 Build를 한 번 수행한다.
4. Build 성공과 Build 과정의 예상하지 않은 Error 및 Warning 부재를 확인한다.
5. Build Player에서 Stage Mode를 시작하고 `Platform_01` 또는 `Platform_02`의 수직 측면에 공중 상태로 접근한다.
6. 벽 방향 입력을 유지하면서 자연스럽게 아래로 떨어지고 지속 고정, 관통과 눈에 띄는 지속 떨림이 없는지 확인한다.
7. Ground 착지 후 Jump와 Momentum Landing을 각각 한 번 수행할 수 있는지 확인한다.
8. InfiniteMode에서도 Platform의 수직 측면을 대상으로 5~7번을 한 번 반복한다.
9. Wall Jump, Wall Slide 또는 의도하지 않은 체공이 발생하지 않는지 확인한다.
10. Player 종료 후 Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

Scene Hierarchy, Component, Collider, Rigidbody, Layer, Physic Material과 Serialized Field는 변경하지 않는다.

### 완료 조건

- [x] Build와 최소 물리 화면 검증 결과가 기록되어 있다.
- [x] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [x] Phase 1 범위 밖 기능이 포함되지 않았다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 Wall, Ground, 속도와 Physic Material 규칙 결정
2. 구현 Step 이후 Unity Script Compilation 결과 확인
3. AI가 지정한 관련 및 전체 Unity Test Runner 실행
4. Step 8에서 자동 검증으로 필요성이 입증된 경우에만 Scene Field 변경
5. Scene 변경 시 저장·재개방과 Missing Reference 확인
6. Step 10의 Unity Development Build 실행
7. 두 Mode의 자연스러운 벽 낙하, 착지 복구, 관통과 지속 떨림 확인
8. Build와 Player의 예상하지 않은 Error·Warning 확인

법선, 속도, 위치, Frame 수, 상태 전환, 호출 횟수와 Retry 초기화는 수동 작업에 포함하지 않고 정적 검증 또는 자동 Test로 처리한다.

---

# 영향 범위

- CollisionSystem
- PlayerMovementSystem
- PlayerControllerSystem
- PlayerCollisionState와 이동 계산 Core
- Jump, MomentumLanding과 NormalLanding
- Stage Mode와 InfiniteMode
- 조건부 SampleScene Rigidbody, Collider 또는 Physic Material
- Edit Mode 및 Play Mode Test
- 관련 Feature, System 문서와 Roadmap

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/CollisionSystem.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/03_Features/Jump.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/NormalLanding.md`
- `AI/03_Features/StagePlay.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/90_Tasks/Prototype_2/20260903_01_Phase4VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 검증 결과

- Roadmap Prototype 3 Phase 1 목표와 완료 조건을 10개 실행 Step으로 분리했다.
- 순수 표면 분류와 속도 보정을 Edit Mode Unit Test 우선 범위로 고정했다.
- 실제 Physics 접촉과 Landing 복구를 임시 Fixture 기반 Play Mode Test 범위로 고정했다.
- 생산 Scene 변경은 자동 Test와 정적 검사로 필요성이 입증된 경우에만 수행하도록 제한했다.
- Unity Compile/Test Runner, 조건부 Scene 작업, Build와 최소 물리 화면 확인만 사용자 수동 작업으로 분리했다.
- Step 1부터 Step 7까지 구현과 사용자 검증을 완료했다.
- Step 8에서 생산 Scene 변경이 불필요함을 정적 검사와 기존 자동 검증 결과로 확정했다.
- Step 9 전체 정적 검증을 완료하고 기존 전체 Compile 및 Test 성공 결과가 현재 코드에 유효함을 확인했다.
- Step 10 화면 검증에서 발견된 모서리 이동과 마찰 문제를 수정하고 Player용 마찰 0 Material을 생산 Scene에 연결했다.
- 수정 후 Script Compilation, Edit Mode 246개, Play Mode 124개와 Windows Standalone Development Build가 통과했다.
- 사용자가 모서리 이동과 벽 접촉 처리 결과가 만족스러움을 확인했다.
- Prototype 3 Phase 1의 미해결 사항은 없다.

---

# 후속 작업

Prototype 3 Phase 2 실행 계획을 작성한다.

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 1의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 작업보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 검증으로 넘기지 않았다.
- Scene 작업을 조건부 최소 범위로 제한했다.
- Prototype 3 Phase 2 이후 범위를 분리했다.
