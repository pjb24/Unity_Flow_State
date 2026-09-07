# 작업 정보

## 작업명

Prototype 3 Phase 2 Step 2 입력 및 물리 경로 정적 조사

## 작업 일자

20260905

## 작업 담당자

AI

## 작업 상태

Step 2 정적 조사 완료. Runtime 구현 및 Unity 실행 검증은 수행하지 않았다.

# 작업 목적

Step 1에서 확정한 자동 이동 계약을 적용할 기존 경로, 책임, Test 변경 지점과 조건부 Asset 및 Scene 변경 후보를 확인한다.

# 작업 대상

- PlayerInputSystem, UIInputSystem, PlayerInputState 및 Input Action Asset/Wrapper
- PlayerMovementSystem, PlayerMovementMath, PlayerControllerSystem 및 CollisionSystem
- GameSystem, StageSystem, RuntimeDataSystem 및 InfiniteModeSystem/InfiniteModeState
- CameraSystem 및 CameraFollow
- 관련 Edit Mode/Play Mode Test와 SampleScene Serialized Reference

# 작업 전 상태

- Step 1은 완료되었고 자동 이동 및 InfiniteMode 종료 규칙이 확정되어 있다.
- Runtime은 Player Move 입력에 의존하는 기존 구현이다. Step 1의 확정 계약은 아직 구현되지 않았다.
- 작업 시작 시 Manual Steps와 Roadmap에 이전 Step 1의 문서 변경이 존재했다. 해당 변경을 보존했다.

# 조사 내용

## 1. 입력 수집 및 소비 경로

근거: `Assets/Scripts/Runtime/Systems/PlayerInputSystem.cs`, `UIInputSystem.cs`, `Assets/Scripts/Runtime/Core/PlayerInputState.cs`, `Assets/InputSystem_Actions.cs`, `Assets/InputSystem_Actions.inputactions`.

- PlayerInputSystem.Initialize는 Wrapper 인스턴스를 생성하고 Callback을 등록한다. 중복 Initialize는 반환한다.
- Move.performed는 `_moveInput`을 설정하고 Move.canceled는 이를 0으로 만든다. GetInputState가 X를 HorizontalInput으로 전달한다.
- Jump.performed 및 MomentumLanding.performed는 각각 transient bool을 설정한다. 이동 단계가 끝나면 ConsumeTransientInput이 두 bool을 지운다. Move는 이 함수에서 소비하지 않는다.
- Player Action Map 활성화 및 비활성화는 ResetInputState를 호출한다. Pause와 Resume 경계의 입력 잔류 정리 경로로 재사용한다.
- OnDestroy는 Action 비활성화, Callback 해제, Wrapper Dispose 및 상태 초기화를 수행한다.
- UIInputSystem은 별도의 Wrapper 인스턴스와 UI Action Map을 사용한다. Navigate는 별도의 Vector2이며 UI transient 소비 시 초기화된다.
- GameSystem은 Playing에서도 UI Action Map을 활성화해 Cancel을 받는다. Pause/Result 메뉴는 NavigateInput.y, Submit, Cancel, Pointer 및 Click으로 처리한다.
- 확정 10A는 Player Map Enable 직후 Player.Move.Disable을 적용하는 Runtime 변경으로 구현 가능하다. Start와 Resume의 공통 EnablePlayerActionMap 경로에 적용해야 한다.
- Runtime에서 제거할 대상은 `_moveInput`, Move Callback 등록/해제 및 메서드, PlayerInputState.HorizontalInput과 생성자 인자, 이동 계산의 HorizontalInput 의존성이다. Asset 및 생성 Wrapper의 Move 정의는 확정 규칙대로 유지한다.
- PlayerInputState 생성자 변경은 JumpFeatureTests와 MomentumLandingFeatureTests의 입력 구성에도 영향을 준다.

### Action Map 및 Action 식별자

| 대상 | Map ID | Action ID | 주요 Binding |
|------|--------|-----------|-------------|
| Player.Move | df70fa95-8a34-4494-b137-73ab6b9c7d37 | 351f2ccd-1f9f-44bf-9bec-d62ac5c5f408 | Keyboard WASD/방향키, Gamepad leftStick, XR Primary2DAxis, Joystick stick |
| Player.Jump | 동일 Player Map | f1ba0d36-48eb-4cd5-b651-1c94a6531f70 | Keyboard space, Gamepad buttonSouth, XR secondaryButton |
| Player.MomentumLanding | 동일 Player Map | 19afd3be-8bb6-4a1d-ac02-f7dbd804ffe5 | Keyboard leftShift, Gamepad buttonWest |
| UI.Navigate | 272f6d14-89ba-496f-b7ff-215263d3219f | c95b2375-e6d9-4b88-9c4c-c5e76515df4b | Keyboard WASD/방향키, Gamepad 양쪽 Stick 방향 및 dpad, Joystick 방향 |

### 겹치는 장치 경로의 Binding 분리 근거

| 장치 경로 | Player.Move Binding ID | UI.Navigate Binding ID |
|-----------|------------------------|------------------------|
| Keyboard a | d2581a9b-1d11-4566-b27d-b92aff5fabbc | 74214943-c580-44e4-98eb-ad7eebe17902 |
| Keyboard d | fcfe95b8-67b9-4526-84b5-5d0bc98d6400 | 8607c725-d935-4808-84b1-8354e29bab63 |
| Keyboard leftArrow | 2e46982e-44cc-431b-9f0b-c11910bf467a | cea9b045-a000-445b-95b8-0c171af70a3b |
| Keyboard rightArrow | 77bff152-3580-4b21-b6de-dcd0c7e41164 | 4cda81dc-9edd-4e03-9d7c-a71a14345d0b |
| Gamepad leftStick | 978bfe49-cc26-4a3d-ab7b-7d7a29327403 | UI는 방향별 Composite Part 사용: left는 8ba04515-75aa-45de-966d-393d9bbd1c14, right는 fcd248ae-a788-4676-a12e-f4d81205600b |

Wrapper의 FromJson에 포함된 JSON을 추출하여 Asset JSON과 전체 구조를 비교했고 일치했다. Player Move와 UI Navigate의 Map, Action 및 Binding은 독립적이다. UI Binding 수정이나 Wrapper 재생성은 현재 필요하지 않다.

## 2. 입력부터 Rigidbody까지의 적용 순서

근거: `Assets/Scripts/Runtime/Systems/PlayerMovementSystem.cs`, `PlayerControllerSystem.cs`, `CollisionSystem.cs`, `Assets/Scripts/Runtime/Core/PlayerMovementMath.cs`.

1. PlayerMovementSystem.FixedUpdate는 `_isRunning && !_isPaused`일 때 ProcessMovementStep을 호출한다.
2. GatherMovementStepInput은 Player 입력을 읽고 CollisionSystem.RefreshCollisionState를 호출한 다음 Controller.GetVelocity와 충돌 상태를 읽는다.
3. NormalizeMovementState와 Jump Coyote Time 갱신 후 CalculateHorizontalSpeed가 입력, 현재 X 속도, 접지 상태와 가속 설정으로 수평 결과를 계산한다.
4. Jump 시작 또는 중력을 계산하고 공중 진행 상태를 갱신한다.
5. Momentum Landing Window 갱신과 입력 Buffer 후 Momentum/Normal Landing을 판정한다.
6. 관성 착지 성공 시 수평 배율과 상한을 적용한다. 일반 착지는 추가 수평 보정 없이 현재 계산 결과를 유지한다.
7. ConstrainVelocityByWalls가 마지막으로 공중 Wall 안쪽 X 속도를 제한한다. Ground가 검출되면 이 제한은 적용하지 않는다.
8. Controller.ApplyMovement가 Z를 0으로 만들고 수평 가속도를 계산한 뒤 Rigidbody.linearVelocity에 결과를 쓴다.
9. MovementSystem.UpdateRuntimeData는 movementResult.Velocity를 저장하고 Player transient 입력을 소비한다.

현재 수평 수학 함수는 같은 방향 입력에서 현재 속도가 기본 속도보다 크면 그 속도를 유지한다. 확정된 관성 속도 보존에 재사용할 수 있다. 자동 이동 API에서는 Move 입력 인자를 제거하고 World +X 목표를 계산해야 한다.

CollisionSystem은 OnCollisionEnter/Stay에서 벽 접촉을 수집하고 Exit에서 제거한다. RefreshCollisionState는 SphereCastNonAlloc 기반 Ground/착지 후보와 수집된 Wall을 결합한다. Initialize에서 Wall 캐시를 지우므로 Retry의 기존 초기화 경로를 재사용한다.

### 책임 및 상태 소유 위치

| 책임 | 소유자 | 적용 방향 |
|------|--------|-----------|
| 자동 목표 속도, Ground/Air 가속, 이동 상태 | PlayerMovementSystem | 기존 설정 및 공통 계산 경로 재사용 |
| 순수 수평 계산 및 Wall 제한 | PlayerMovementMath | 입력 독립 API 및 경계 Test 추가 |
| Rigidbody 읽기/적용, Pause 물리 보존 | PlayerControllerSystem | 자동 이동 규칙을 추가하지 않음 |
| Ground/Wall 판정 | CollisionSystem | InfiniteMode에서 재판정하지 않음 |
| 시작/저속/Wall 예산 상태 | InfiniteModeState | 기존 타이머 상태 소유자에 유한 예산 확장 |
| Infinite 설정 및 종료 요청 연결 | InfiniteModeSystem | 확정 실제 속도와 Wall 제한 상태 전달 |
| Mode 시작, 중단, 재시작 | GameSystem/StageSystem | Mode별 자동 속도 계산을 복제하지 않음 |

## 3. Run 시작 및 종료 순서

근거: `Assets/Scripts/Runtime/Systems/GameSystem.cs`, `StageSystem.cs`, `RuntimeDataSystem.cs`.

- 공통 StartGame: Initializing → 새 Runtime Data → UI/Result 초기화 → Controller → Collision → Stage(Mode Root 전환) → Movement → InfiniteMode → Camera 초기화 → 입력 System 초기화 및 UI Map 활성화 → Ready/HUD → StageEnded 구독 → StartStage → Stage Mode 전용 Timer 시작 → Player Map 활성화 → CameraFollow.StartFollowing → Playing.
- Movement.Initialize는 `_isRunning=true`, InfiniteMode.Initialize는 내부 State.Start를 호출한다. 두 System 모두 게임 Playing 전 동기 초기화 안에서 실행 상태가 켜진다. 현재 StartGame은 중간 yield가 없지만 독립 Initialize 호출과 실패 경로까지 Playing 계약으로 보장하는 Test가 필요하다.
- Controller.Initialize는 시작점으로 Rigidbody를 옮기고 선속도/각속도/수평 가속도를 0으로 만든다. Movement는 Jump/Landing 플래그를 초기화하고 현재 충돌을 다시 읽는다.
- Stage Mode는 Goal → StageSystem.HandleGoalReached → EndStage → StageEnded → GameSystem.HandleStageEnded → Clear Time Result → EndGame으로 종료한다.
- InfiniteMode는 속도 또는 추락 종료 → 최종 거리/Score 확정 → StageSystem.TryEndInfiniteStage → StageEnded → Infinite Result 생성 → EndGame으로 종료한다.
- EndGame은 Ending/Result 전환 → Timer 정지 → Stage 정지 → Player Map 비활성화/UI Map 활성화 → CameraFollow 중단 → InfiniteMode 정지 → Movement/Controller 정지 → Timer 제거 → Runtime Data 제거 → Ended 순서다.
- Retry는 Paused이면 먼저 EndGame을 호출하고, Ended에서 StartGame을 호출한다. RuntimeDataSystem은 기존 데이터가 제거된 뒤 새 GameRuntimeData를 만든다. 같은 Mode를 유지한다.
- StageEnded의 중복/재진입은 GameSystem 상태와 StageSystem._hasEnded 경계로 방어한다. 기존 단일 종료/단일 Retry Test를 보존한다.

## 4. Pause 및 Resume

- PausePlaySystems 순서: Stage Timer → Stage → Infinite(해당 Mode만) → Movement → Controller 물리.
- Controller는 선속도, 각속도, 수평 가속도 및 Constraints를 저장하고 속도 0/FreezeAll로 바꾼다. Movement는 paused flag만 설정하므로 Jump/착지 Window와 내부 상태가 보존된다.
- Pause 성공 후 Player Map을 비활성화하고 입력 상태를 초기화한다. UI는 활성 상태다.
- ResumePlaySystems는 Controller 물리 복원 → Movement → Infinite → Stage → Timer 순서다. 이후 Player Map을 다시 활성화한다.
- Move 비활성화는 Resume에서도 다시 적용해야 한다. 새 Run 초기화 함수를 Pause 처리에 재사용하지 않는다.

## 5. InfiniteMode 실제 속도 및 종료 조건 위험

근거: `Assets/Scripts/Runtime/Systems/InfiniteModeSystem.cs`, `Assets/Scripts/Runtime/Features/InfiniteModeState.cs`, `Assets/Scripts/Runtime/Core/PlayerMovementRuntimeData.cs`.

- InfiniteModeSystem.FixedUpdate는 거리/Score 갱신 → ProcessProgress → 추락 순서다. ProcessProgress는 Runtime Data.CurrentHorizontalSpeed를 읽는다.
- 그 값은 Movement가 Rigidbody에 쓴 이동 결과이다. 물리 충돌 이후 실제 속도를 별도로 기록하는 경로가 없다.
- InfiniteModeState는 현재 Math.Abs(speed)를 최소 속도와 비교한다. `_playDuration`과 `_belowSpeedDuration`을 소유하고 시작 유예를 넘는 단계에서는 초과 시간만 저속 판정에 사용한다. Wall 상태/예산은 없다.
- Movement와 Infinite의 별도 FixedUpdate 사이에 명시적인 실행 순서 지정은 발견되지 않았다. 두 Script의 meta에도 executionOrder가 없다. TimeManager의 Fixed Timestep은 `0.02`, DynamicsManager의 m_AutoSimulation은 `1`이며 Runtime에 수동 Physics.Simulate 호출은 없다.
- 따라서 단순히 InfiniteModeSystem에서 Rigidbody를 읽도록 바꿔도 Movement가 이미 속도를 덮어쓴 뒤 읽으면 실제 충돌 결과라는 보장이 없다. 측정과 적용 순서를 함께 고정해야 한다.

### 구현 경로 제안 및 Test로 고정할 경계

- 기존 GatherMovementStepInput의 Controller.GetVelocity가 새 이동 결과 적용 전 물리 속도 읽기 지점이다. 이 지점에서 얻은 속도와 갱신한 충돌 상태를 같은 물리 단계의 관측값으로 전달하는 경로를 우선 검토한다.
- Runtime Data로 전달한다면 기존 계산 결과 필드와 관측 속도 필드를 구분하고, Infinite 판정이 해당 관측값 생성 이후 한 단계당 한 번만 진행하도록 호출 순서 또는 코드 실행 순서를 명시한다.
- Controller를 별도 참조로 직접 읽는 대안도 읽기 시점을 이동 결과 적용보다 앞서 고정해야 한다. Scene 참조 추가 없이 기존 Movement/Runtime Data 연결을 활용하는 경로를 우선한다.
- 최초 Run에서 아직 완료된 물리 단계가 없는 상태, 시작 유예를 넘는 단계, Pause/Resume 및 Retry에서 오래된 관측값이 소비되지 않는지 Play Mode Test로 판정한다.
- 공중 Wall 제한과 실제 속도는 동일한 단계 기준으로 전달한다. 단순 HasWallContact만으로 지상 모서리나 반대쪽 벽까지 유예 대상으로 확대하지 않는다.
- 위 내용은 변경 지점과 검증 경계의 확정이다. 새 필드/API 이름이나 호출 방식은 Step 4/7 구현 시 Test와 함께 정한다.

### 기존 조건이 잘못 지속되거나 종료되는 경로

| 경로 | 기존 코드에서의 문제 또는 확정 계약과의 차이 | 필요한 검증 |
|------|------------------------------------------|-------------|
| 목표 속도 8을 계속 측정값으로 사용 | 실제 정지와 무관하게 최소 2 이상으로 인식하여 저속 종료가 발생하지 않음 | 물리 결과가 0인 막힘에서도 목표 속도로 판정하지 않음 |
| Ground+Wall에서 이동 계산값 사용 | Ground 우선 계산은 양의 속도를 출력할 수 있으므로 실제 막힘을 표현하지 못함. 현 설정 지상 가속 한 단계 증가량은 1이므로 모든 막힘이 무조건 지속된다고 단정하지 않음 | 실제 지상 막힘과 계산값 불일치 |
| 공중 Wall X 제한 | 계산값 0이 일반 저속 시간에 바로 누적되어 확정된 추가 1초 예산 없이 종료 | 낙하 중 유한 추가 유예, 이탈 후 복구 |
| 음의 실제 속도 | 기존 절댓값 규칙은 빠른 후진을 진행 지속으로 인정 | max(0, vx), 양/음/0 및 최소값 경계 |
| 접촉 중 저속 시간을 무조건 중단하는 변경 | 무기한 Wall 유예 가능 | 예산 소진 및 재접촉으로 재충전되지 않음 |

## 6. Camera 경로

근거: `Assets/Scripts/Runtime/Systems/CameraSystem.cs`, `Assets/Scripts/Runtime/Features/CameraFollow.cs`.

- CameraSystem.Initialize는 Cinemachine Camera의 TrackingTarget에 별도 Follow Target을 지정하고 Orthographic Size를 적용한다.
- CameraFollow.StartFollowing은 Follow Target의 Y/Z를 보존하고 추적을 활성화한다. LateUpdate는 Player X를 별도 Follow Target X에 복사한다. Player Transform을 움직이지 않는다.
- GameSystem의 Pause/Resume은 CameraFollow를 중단/재시작하지 않는다. Player 물리가 정지한 상태에서 추적 상태와 Target 참조를 유지한다. Camera 댐핑까지 완전히 정지한다고 단정하지 않는다.
- EndGame은 StopFollowing, Retry는 CameraSystem.Initialize 및 StartFollowing을 다시 호출한다. 재시작 직후 첫 LateUpdate와 Camera 갱신의 추적 복구를 Test로 추가한다.

## 7. SampleScene 읽기 전용 검사

근거: `Assets/Scenes/SampleScene.unity`, 관련 Script meta 및 `Assets/PhysicsMaterials/PlayerZeroFriction.physicMaterial`.

| Object / Component | fileID 및 확인 내용 |
|--------------------|--------------------|
| GameSystem | 1202804280: Runtime, UI, Input, Movement, Controller, Collision, Stage, Infinite, Timer, Result, Camera/Follow 참조의 대상 fileID가 모두 존재함. 선택 Mode는 0(Stage) |
| PlayerInputSystem | 1769325111: 별도 Serialized Input Asset 참조 없이 Runtime Wrapper 생성 |
| PlayerMovementSystem | 635192153: Input 1769325111, Controller 2089254633, Collision 2089254632, Runtime 383414368, Jump 635192156, Momentum 635192155, Normal 635192154 |
| Movement 설정 | moveSpeed 8, Ground 가속 50, Air 가속 25, 최대 속도 14, 중력 25 |
| Player / Controller | 2089254633: Rigidbody 2089254635, StartPoint Transform 410616028 |
| Player / Rigidbody | 2089254635: useGravity 0, isKinematic 0, LinearDamping 0, Constraints 120, Interpolate 1, CollisionDetection 1 |
| Player / Collision | 2089254632: CapsuleCollider 2089254634, GroundCheck 121742471, Ground mask 64, 반경 0.25, 접지 거리 0.05, 예측 거리 3 |
| Player / CapsuleCollider | 2089254634: 비Trigger, 반경 0.5, 높이 2. PlayerZeroFriction GUID 38eeb7cff83e4b16b608e97bc694ba7b 참조 |
| PlayerZeroFriction | 실제 meta GUID 일치. dynamic/static friction 0, bounciness 0 |
| InfiniteModeSystem | 107985694: Runtime 383414368, Stage 1671477403, Player Transform 2089254631. 최소 속도 2, 시작 1초, 저속 0.5초, 추락 Y -3, Score 비율 10 |
| CameraSystem | 1659597220: CinemachineCamera 2049310296, Follow Target 1649076997, Size 5 |
| CameraRig / CameraFollow | 2040863081: Player Transform 2089254631, Follow Target 1649076997 |
| CinemachineCamera | TrackingTarget 1649076997로 CameraSystem/CameraFollow와 동일 대상 |

위 관련 Component의 로컬 Serialized Reference 존재를 스크립트로 검사했다. 전체 Scene의 모든 기능 또는 Unity Import 성공을 검증한 결과로 확대하지 않는다.

## 8. 재사용 Test 및 추가 시나리오

근거: `Assets/Tests/EditMode`, `Assets/Tests/PlayMode`의 해당 Test 본문과 입력/속도 설정 Helper 검색. Test 실행 결과가 아니라 정적 조사 결과다.

| 구현 Step | 재사용 대상 | 수정 또는 추가할 검증 |
|-----------|-------------|------------------------|
| 3 수평 계산 | PlayerMovementMathTests | 자동 우측 가속, Ground/Air, 음의 속도 복구, 기본 목표 overshoot 방지, 관성 초과 속도 유지/상한, dt 0/음수, NaN/Infinity 및 음수 설정 방어 |
| 3 충돌 및 수직 | PlayerMovementMathTests의 ConstrainVelocity 및 CalculateVerticalSpeed, JumpFeatureTests, MomentumLandingFeatureTests, NormalLandingFeatureTests | 기존 수직/Wall 기대값 보존. HorizontalInput 생성자 변경만 관련 입력 Helper에 반영 |
| 4 Mode 공통 이동 | GameLifecycleIntegrationTests, PlayerJumpIntegrationTests, MomentumLandingIntegrationTests, ProductionSceneGameModeTestUtility | 두 Mode의 무입력 가속, Playing 시작 경계, 중복 초기화/Start, Jump/착지 후 속도, 종료 뒤 이동 없음 |
| 5 입력 | UIInputSystemTests, PauseMenuIntegrationTests, ResultMenuIntegrationTests | 실제 Keyboard/Gamepad 장치 이벤트부터 Action/Callback/이동 결과까지 새 Test. Move Disable이 Start/Resume/Retry에서 유지되고 Jump/Momentum/UI Navigate는 동작함 |
| 6 생명주기 | GamePauseOrchestrationTests, GameLifecycleIntegrationTests, PauseMenuIntegrationTests, ResultMenuIntegrationTests | 실제 자동 가속 및 관성 착지 속도로 Pause/Resume, transient 잔류 차단, 연속 2 Run의 속도/Wall/Timer 독립성 |
| 7 종료 계산 | InfiniteModeStateTests | 절댓값 경계 Test를 확정 우측 속도 계약으로 변경. 시작/일반 저속/Wall 예산 경계, 예산 소진 단계의 잔여 dt, 기존 저속 누적 보존, 반복 접촉, 실제 회복 시 초기화 |
| 7 물리 통합 | InfiniteModeSystemTests, InfiniteModeIntegrationTests, InfiniteModeRuntimeDataTests | Runtime 계산값 주입만으로 실제 물리 검증을 대체하지 않음. 지상 벽 막힘, 공중 벽 복구, 속도 측정 순서, Pause/Retry 예산, 단일 종료와 최종 기록 보존 |
| 8 충돌 | WallFallPhysicsTests, CollisionSystemContactTests, WallLandingRecoveryIntegrationTests, StageCollisionConfigurationTests, StageGoalIntegrationTests | 실제 자동 이동 경로의 벽 낙하/이탈/모서리/착지 및 Goal 종료 |
| 8 Camera | CameraFollowIntegrationTests | 무입력 X 추적, Pause 상태/Target 보존, Resume/Retry 및 첫 LateUpdate 복구. Y/Z와 투영 검증 유지 |

### 기존 Test를 그대로 완료 근거로 사용할 수 없는 부분

- PlayerMovementMathTests.CalculateHorizontalSpeed_NoInput_DeceleratesTowardZero는 변경될 수동 입력 계약이다. 자동 가속 Test로 대체할 때 변경 이유를 기록하고 Wall/중력 회귀 기대값은 유지한다.
- InfiniteModeStateTests.UpdateProgress_HorizontalSpeedBoundary_UsesAbsoluteValue의 음수 통과 기대값은 확정 규칙과 다르다. 우측 속도 계약을 명시한 경계 Test로 바꾸며 단순 삭제하거나 Ignore하지 않는다.
- CameraFollowIntegrationTests, MomentumLandingIntegrationTests, WallLandingRecoveryIntegrationTests가 `_moveInput`을 reflection으로 설정한다. 입력 없는 자동 경로로 전제 조건을 바꾸되 본래 Camera/착지/낙하 검증은 보존한다.
- InfiniteModeSystemTests.SetHorizontalSpeed는 Runtime Data에 수치를 직접 쓴다. 규칙 연결 Test에는 유용하지만 실제 Rigidbody 관측 검증은 아니다.
- InfiniteModeIntegrationTests.BelowMinimumSpeed_CreatesInfiniteResultData는 시작 유예 0, 저속 유예 한 물리 단계로 바꾼다. 자동 이동에서 무입력이라는 이유만으로 종료를 기대할 수 없으므로 실제 저속 상황을 구성해야 한다.
- WallLandingRecoveryIntegrationTests의 Infinite 준비는 최소 속도 0 및 시작 유예 100초를 사용한다. Phase 1 낙하 격리 검증은 유지하되 Step 7의 확정 2/1/0.5/1 규칙을 검증하는 별도 통합 Test가 필요하다.
- WallFallPhysicsTests.RightWall_HeldInputWithZeroFriction_ContinuesFalling은 Rigidbody 속도를 직접 쓴다. 재질/낙하 검증으로 재사용하며 자동 이동 System 통합 검증으로 간주하지 않는다.
- 현재 Assets/Tests에서 Gamepad, InputTestFixture, AddDevice, QueueStateEvent 사용을 찾지 못했다. 메뉴 Test는 입력 System private 상태를 주입한다. 실제 장치 Binding 보존 검증을 추가하고 장치, Callback 및 활성 상태를 정리해야 한다.
- 자동 이동으로 Scene 로드 후 Player가 이동하므로 Jump/Camera 등 기존 Test의 위치/시간 전제도 함께 대조한다. 생산 Scene을 변경하거나 기존 회귀 허용 오차를 임의로 완화하여 해결하지 않는다.

## 9. Runtime 변경과 조건부 Editor 작업 구분

| 대상 | 판정 | 처리 방법 |
|------|------|-----------|
| 자동 속도/가속/관성 보존 | 기존 설정과 참조 재사용 가능 | Runtime 및 Test 변경으로 진행 |
| Move 제거/UI 보존 | Asset과 Wrapper 변경 불필요 | Runtime Callback/상태 제거와 Move.Disable, 장치 Test 추가 |
| Pause/Retry/Camera 참조 | 현재 관련 참조 누락 근거 없음 | 기존 경로 재사용 및 통합 Test 확장 |
| 실제 속도/Wall 관측 전달 | 기존 Movement가 Controller/Collision/Runtime 참조를 모두 보유 | Runtime 전달 경로 우선. 추가 Scene 참조를 선행 요구하지 않음 |
| Infinite 최소 속도 | Scene 2는 확정 규칙과 일치, 코드 기본값 5는 다름 | 코드 기본값 및 Test 설정을 구현 시 대조. Scene의 2 변경 불필요 |
| Wall 추가 유예 1초 | 현재 Serialized Field가 없음 | Step 7에서 Runtime 기본값/설정 전달을 구현. 기존 Scene에서 유효한 1초 사용 여부를 Test로 확인 |
| 조건부 새 Field/참조 | 현재는 변경 필요 확정 전 | Step 9에서 정적 근거 및 실패 Test가 있을 때만 Object/Component/Field/값을 명세하고 사용자가 Editor에서 수행 |

Step 2에서 필요한 사용자 수동 작업은 없다. Scene 저장/재설정, Input Action 재생성, Build, Compile 및 Test Runner 실행을 이 조사 완료 조건으로 요구하지 않는다.

# 작업 내용

- 관련 문서와 Runtime/Test를 읽고 입력, 물리, Mode 생명주기, 종료 및 Camera 경로를 대조했다.
- Input Action JSON 및 생성 Wrapper를 구조 비교하고 SampleScene 관련 참조와 재질 GUID를 읽기 전용으로 검사했다.
- 구현 책임과 Test 재사용/누락 시나리오를 기록했다.
- Manual Steps의 Step 2 완료 조건과 Roadmap의 다음 작업을 Step 3으로 갱신했다.

# 영향 범위

- Tasks: 이 조사 기록 및 Phase 2 Manual Steps 진행 상태
- Roadmap: 현재 준비 상태 및 다음 작업

# 검증 내용

- rg로 Runtime의 Move 의존성 및 Test reflection/생성자 참조를 검색했다.
- PowerShell JSON 파싱으로 Input Asset과 Wrapper 내 JSON의 전체 구조 일치를 확인했다.
- YAML 블록별 fileID 검사로 관련 Component의 로컬 참조 존재를 확인했다.
- Script meta GUID와 Scene Component, PlayerZeroFriction meta 및 재질 참조를 대조했다.
- Test 이름뿐 아니라 입력 주입, 설정 변경 및 결과 판정 Helper를 확인했다.
- 문서 내 37개 경로의 존재, Step 2 완료 상태와 완료 조건 3개를 검사했고 git diff --check가 통과했다.

# 검증 결과

- Step 2의 세 완료 조건에 필요한 정적 근거를 확보했다.
- Input Asset/Wrapper 일치 및 조사 범위 로컬 Serialized Reference 검사가 통과했다.
- 실제 속도 관측과 계산 결과의 혼용, 실행 순서 미지정, 실제 장치 입력 Test 부재를 후속 구현/검증 대상으로 확인했다.
- Runtime, Test, Scene, Asset 및 ProjectSettings는 변경하지 않았다.
- Unity Compile, Test Runner 및 Build는 실행하지 않았으며 성공으로 기록하지 않는다. Phase 2 구현 및 실행 검증은 아직 대기다.

# 후속 작업

Step 3에서 확정 계약을 검증하는 순수 자동 수평 이동 Test를 먼저 작성하고 계산 API를 구현한다. 이후 사용자가 지정된 Unity Compile 및 Edit Mode Test를 수행한다. 실제 속도 관측 순서/Wall 예산은 Step 4/7, 장치 입력은 Step 5, Scene 변경 최종 판정은 Step 9에서 처리한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/PlayerInputSystem.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/NormalLanding.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260904_02_Phase2ManualSteps.md`

# 작성 완료 기준

- 입력부터 물리 적용과 종료까지의 경로를 코드 근거로 기록했다.
- 현재 구현 사실과 확정 계약에 따른 변경 지점을 구분했다.
- Test 재사용 및 누락 시나리오, 조건부 Scene/Asset 변경 후보를 분리했다.
- 조사만으로 판정할 수 없는 Runtime 실행 결과를 완료로 기록하지 않았다.
