# 작업 정보

## 작업명

Phase 2 Manual Steps

## 최초 작성 일자

20260710

## 최종 갱신 일자

20260720

## 작업 담당자

AI

## 작업 상태

완료

---

# 작업 목적

이미 작성된 Phase 2 코드를 Unity Scene과 Inspector에 연결하고, 플레이어 이동·점프·착지·카메라 추적을 실제 Play Mode에서 검증하기 위한 수동 작업 순서를 정의한다.

이 문서는 위에서 아래로 순서대로 수행한다. 각 Step의 완료 조건을 충족하지 못하면 다음 Step으로 넘어가지 않는다.

---

# 현재 구현 상태

- Unity Editor: `6000.3.5f2`
- Input System: `1.17.0`
- Cinemachine: `3.1.7`
- Input Actions: `Assets/InputSystem_Actions.inputactions`
- Phase 2 Core, System, Feature 스크립트: 작성됨
- GameSystem의 Phase 2 연결 흐름: 작성됨
- 중력 설정 원본: `PlayerMovementSystem.Gravity Acceleration` 한 곳으로 통합됨
- 남은 작업: Scene/Inspector 연결 재확인, 접지 판정 보정, Cinemachine 위치 제어 구성, Play Mode 검증

현재 확인된 주의 사항은 아래와 같다.

- Player의 시작 Y가 `1.1`이고 점프 후 Y가 약 `1.33`에서 멈추는 현상이 있다.
- 현재 CollisionSystem은 하나의 하향 SphereCast 결과를 접지 판정과 착지 예측에 함께 사용한다.
- SphereCast 반지름과 Grounded Distance가 실제 Collider 접촉보다 먼저 `IsGrounded`를 참으로 만들 수 있다.
- Cinemachine Camera에는 Tracking Target뿐 아니라 실제 위치를 계산하는 Position Control Component가 필요하다.

---

# 수동 작업 전 원칙

- Scene 백업 또는 버전 관리 상태를 확인한 뒤 시작한다.
- Console의 기존 메시지를 지우고 새로운 Compile Error부터 해결한다.
- Inspector 값은 이 문서의 시험값으로 시작하되, 실제 판정 결과를 기준으로 보정한다.
- Rigidbody의 `Use Gravity`는 끈다. 중력은 PlayerMovementSystem만 적용한다.
- 일회성 입력은 이동 계산과 Runtime Data 갱신이 끝난 뒤 소비한다.
- 실제 접지와 착지 후보 예측은 서로 다른 판정으로 취급한다.
- 모든 검증을 통과하기 전에는 Roadmap의 Phase 2를 완료로 변경하지 않는다.

---

# Test 기반 검증 원칙

앞으로 기능 검증은 Unity Test Runner를 기본 수단으로 사용한다.

검증 순서는 아래와 같다.

1. 문서와 코드의 정적 검사를 수행한다.
2. Unity Script Compile Error가 없는지 확인한다.
3. 계산식과 Feature 상태 규칙을 Edit Mode Test로 검증한다.
4. Scene, System 실행 순서, Rigidbody, 입력, 충돌 연동을 Play Mode Test로 검증한다.
5. 화면 표시와 조작감처럼 자동 판정이 적절하지 않은 항목만 수동으로 확인한다.

새 기능이나 버그 수정에는 변경된 책임을 검증하는 Test를 함께 추가한다. 기존 동작에 영향을 줄 수 있으면 관련 Test 전체를 다시 실행한다.

- Test가 실패하면 해당 Step을 완료로 표시하지 않는다.
- Test 이름과 실패 메시지를 기준으로 원인을 수정한다.
- 실패를 숨기기 위해 `LogAssert.Expect`로 예상 처리하지 않는다. 요구사항상 반드시 발생해야 하는 로그만 예외로 한다.
- Test가 실제 생산 코드와 Scene을 사용하도록 구성하고 계산식을 Test에 중복 구현하지 않는다.
- Test Runner 실행 결과와 필요한 최소 수동 확인을 모두 통과해야 Step을 완료한다.

---

# Unity 수동 실행 Step

## Step 진행 상태

| Step | 작업 | 상태 | 확인 근거 |
| --- | --- | --- | --- |
| 1 | 프로젝트와 Scene을 연다 | 완료 | 20260720 사용자 수동 확인 완료 |
| 2 | Input Actions를 확인한다 | 완료 | 20260720 정적 검사 및 사용자 수동 확인 완료 |
| 3 | Scene 계층과 Component를 확인한다 | 완료 | 20260720 정적 검사 및 사용자 수동 확인 완료 |
| 4 | System과 Feature 참조를 연결한다 | 완료 | 20260720 정적 검사 및 사용자 수동 확인 완료 |
| 5 | Player Rigidbody와 Collider를 설정한다 | 완료 | 20260720 정적 검사, 설정 수정 및 사용자 수동 확인 완료 |
| 6 | Ground와 접지 판정을 설정한다 | 완료 | 20260720 정적 검사, Query 분리 구현 및 사용자 수동 검증 완료 |
| 7 | 이동과 Feature 시험값을 확인한다 | 완료 | 20260720 정적 검사 및 사용자 수동 확인 완료 |
| 8 | Cinemachine 위치 제어를 구성한다 | 완료 | 20260720 정적 검사, 설정 수정 및 사용자 수동 검증 완료 |
| 9 | Phase 1 시작 흐름을 확인한다 | 완료 | 20260720 시작 흐름 정적 검사 및 사용자 Play Mode 확인 완료 |
| 10 | 수평 이동을 검증한다 | 완료 | 20260720 이동 및 Inspector 속도·가속도 표시 사용자 Play Mode 검증 완료 |
| 11 | 점프와 착지 높이를 검증한다 | 완료 | 20260720 컴파일, Edit Mode 15개, Play Mode 3개 및 사용자 동작 검증 완료 |
| 12 | 관성 착지를 검증한다 | 완료 | 20260720 Edit Mode 26개와 Play Mode 6개 통과, 사용자 완료 승인. 임시 비주얼 항목은 후속 재검증 |
| 13 | CameraFollow를 검증한다 | 완료 | 20260720 컴파일, Edit Mode 28개, Play Mode 9개 및 사용자 수동 검증 완료 |
| 14 | 종료와 Phase 1 회귀를 검증한다 | 완료 | 20260720 컴파일, Edit Mode 28개, Play Mode 11개 및 사용자 수동 검증 완료 |
| 15 | Scene과 검증 결과를 저장한다 | 완료 | 20260720 파일 검사, 검증 결과 기록 및 사용자 Unity Editor 최종 저장 확인 완료 |

상태 기준:

- `완료`: 정적 검사와 필요한 Unity Editor 수동 확인을 모두 통과했다.
- `미완료`: 작업을 수행하지 않았거나, 정적 검사는 통과했지만 Unity Editor 수동 확인이 남아 있다.

---

## Step 1. 프로젝트와 Scene을 연다

- 진행 상태: **완료**
- 확인 근거: 20260720 사용자 수동 확인 완료

1. Unity Hub에서 프로젝트를 연다.
2. `Assets/Scenes/SampleScene.unity`를 연다.
3. Script Compile이 끝날 때까지 기다린다.
4. Console을 열고 `Clear`를 누른다.
5. Compile Error와 Missing Script가 없는지 확인한다.
6. Scene을 저장한다.

완료 조건:

- Console에 Compile Error가 없다.
- SampleScene이 정상적으로 열리고 기존 Phase 1 오브젝트가 유지된다.

## Step 2. Input Actions를 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사 및 사용자 수동 확인 완료

1. `Assets/InputSystem_Actions.inputactions`를 연다.
2. `Player` Action Map을 선택한다.
3. 아래 Action과 바인딩을 확인한다.

| Action | Type | Keyboard | Gamepad |
| --- | --- | --- | --- |
| Move | Value / Vector2 | A/D, Left/Right Arrow | Left Stick |
| Jump | Button | Space | Button South |
| MomentumLanding | Button | Left Shift | Button West |

4. Input Actions Asset Inspector에서 `Generate C# Class`가 활성화되어 있는지 확인한다.
5. Class Name이 `InputSystem_Actions`, Namespace가 `FlowState.Input`인지 확인한다.
6. 변경한 항목이 있으면 `Save Asset` 또는 `Apply`를 누른다.
7. Console에 Input Actions 관련 오류가 없는지 확인한다.

완료 조건:

- 세 Action이 존재하고 C# Wrapper가 정상적으로 생성된다.

## Step 3. Scene 계층과 Component를 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사 및 사용자 수동 확인 완료

Hierarchy에서 아래 구성이 존재하는지 확인한다.

```text
GameRoot
Systems
  GameSystem
  RuntimeDataSystem
  UIManagementSystem
  PlayerInputSystem
  PlayerMovementSystem
Player
  Visual
  GroundCheck
World
  Ground
CameraRig
  CameraFollowTarget
  CinemachineCamera
Main Camera
UIRoot
```

Component 배치는 아래와 같이 확인한다.

- `Systems/PlayerInputSystem`: PlayerInputSystem
- `Systems/PlayerMovementSystem`: PlayerMovementSystem, JumpFeature, MomentumLandingFeature, NormalLandingFeature
- `Player`: Rigidbody, CapsuleCollider, PlayerControllerSystem, CollisionSystem
- `Systems/CameraSystem`: CameraSystem
- `CameraRig`: CameraFollow
- `CameraRig/CinemachineCamera`: CinemachineCamera와 Position Control Component
- `Main Camera`: Camera, CinemachineBrain

완료 조건:

- Missing Script가 없고 각 Component가 지정된 오브젝트에 한 번만 존재한다.

## Step 4. System과 Feature 참조를 연결한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사 및 사용자 수동 확인 완료

Inspector에서 아래 참조가 비어 있지 않은지 확인한다.

### GameSystem

- Runtime Data System
- UI Management System
- Player Input System
- Player Movement System
- Player Controller System
- Collision System
- Camera System
- Camera Follow

### PlayerMovementSystem

- Player Input System
- Player Controller System
- Collision System
- Runtime Data System
- Jump Feature
- Momentum Landing Feature
- Normal Landing Feature

### PlayerControllerSystem

- Player Rigidbody: `Player`의 Rigidbody

### CollisionSystem

- Player Collider: `Player`의 CapsuleCollider
- Ground Check: `Player/GroundCheck`
- Ground Layer: `Ground`

### CameraSystem

- Cinemachine Camera: `CameraRig/CinemachineCamera`
- Follow Target: `CameraRig/CameraFollowTarget`

### CameraFollow

- Player: `Player`
- Follow Target: `CameraRig/CameraFollowTarget`

완료 조건:

- 위 Serialized Field에 `None`이 없다.
- Play Mode 진입 시 필수 참조 누락 Error가 발생하지 않는다.

## Step 5. Player Rigidbody와 Collider를 설정한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사, 설정 수정 및 사용자 수동 확인 완료

1. `Player`를 선택한다.
2. Rigidbody를 아래와 같이 설정한다.

- Use Gravity: 비활성화
- Is Kinematic: 비활성화
- Interpolate: `Interpolate`
- Collision Detection: `Continuous`
- Freeze Position Z: 활성화
- Freeze Rotation X/Y/Z: 활성화

3. CapsuleCollider의 Center와 Height가 Visual과 일치하는지 확인한다.
4. Player의 Scale이 `(1, 1, 1)`인지 확인한다.
5. Player의 발바닥이 Ground 윗면에 닿도록 시작 위치를 맞춘다.

CapsuleCollider의 Height가 `2`, Center Y가 `0`, Ground 윗면 Y가 `0`이면 Player 중심 Y의 물리적 접촉 위치는 원칙적으로 `1`이다. 임의의 공중 위치를 시작 기준으로 사용하지 않는다.

완료 조건:

- Player가 Z축으로 이동하거나 회전할 수 없다.
- Unity Rigidbody 중력과 PlayerMovementSystem 중력이 중복 적용되지 않는다.

## Step 6. Ground와 접지 판정을 설정한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사, Query 분리 구현 및 사용자 수동 검증 완료

1. `World/Ground`를 선택한다.
2. Layer가 `Ground`인지 확인한다.
3. BoxCollider의 `Is Trigger`를 비활성화한다.
4. Ground에 Rigidbody가 없는지 확인한다.
5. `Player/GroundCheck`의 Local Position을 `(0, -0.74, 0)`으로 설정한다.
6. CollisionSystem의 Ground Layer에 `Ground`만 포함한다.
7. 초기 시험값을 확인한다.

- Ground Check Radius: `0.25`
- Grounded Distance: `0.02`
- Ground Prediction Distance: `3.00`

8. 위 숫자를 완료값으로 간주하지 말고 아래 판정 조건을 확인한다.

- 실제 간격이 남아 있을 때 `IsGrounded`는 `false`여야 한다.
- 공중에서 예측 범위 안에 지면이 있으면 `IsGrounded = false`이면서 Ground Distance는 유한할 수 있다.
- 실제로 Collider가 지면에 닿았거나 허용한 아주 작은 접지 오차 안에 있을 때만 `IsGrounded`가 `true`여야 한다.

중요:

CollisionSystem은 짧은 실제 접지 SphereCast와 긴 착지 예측 SphereCast를 별도로 실행한다. `IsGrounded`는 실제 접지 Query 결과로만 결정하고, Ground Distance와 후보 지면 정보는 착지 예측 Query 결과로 생성한다. Player가 점프 후 시작 높이와 다른 위치에서 멈추면 다음 Step으로 넘어가지 않는다.

권장 분리 방향:

- 실제 접지 Query: CapsuleCollider 하단에 맞춘 GroundCheck에서 `0.02`의 짧은 거리만 검사한다.
- 착지 예측 Query: GroundCheck에서 아래 방향으로 별도 SphereCast를 수행해 Momentum Landing용 거리를 구한다.
- 예측 Query가 지면을 찾았다는 사실만으로 `IsGrounded`를 변경하지 않는다.

완료 조건:

- 정지 위치와 점프 후 착지 위치가 허용 오차 안에서 같다.
- 공중의 물리적 간격이 남아 있는 동안 `IsGrounded`가 참이 되지 않는다.

## Step 7. 이동과 Feature 시험값을 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사 및 사용자 수동 확인 완료

PlayerMovementSystem:

- Move Speed: `8`
- Ground Acceleration: `50`
- Air Acceleration: `25`
- Maximum Horizontal Speed: `14`
- Gravity Acceleration: `25`

JumpFeature:

- Jump Height: `3`
- Coyote Time: `0.10`
- Gravity Acceleration 필드가 없어야 한다.

MomentumLandingFeature:

- Momentum Landing Window: `0.15`
- Speed Multiplier: `1.15`
- Maximum Horizontal Speed: `14`

완료 조건:

- 중력 설정은 PlayerMovementSystem에만 존재한다.
- JumpFeature와 Rigidbody가 별도의 중력을 적용하지 않는다.

## Step 8. Cinemachine 위치 제어를 구성한다

- 진행 상태: **완료**
- 확인 근거: 20260720 정적 검사, 설정 수정 및 사용자 수동 검증 완료

1. `Main Camera`에 CinemachineBrain이 있는지 확인한다.
2. `CameraRig/CinemachineCamera`를 선택한다.
3. Tracking Target에 `CameraRig/CameraFollowTarget`을 연결한다.
4. `Add Component`에서 `Cinemachine Follow`을 추가한다.
5. Follow Offset을 초기값 `(0, 0, -10)`으로 설정한다.
6. 최초 기능 검증 동안 Position Damping X/Y/Z를 모두 `0`으로 설정한다.
7. CameraSystem의 Orthographic Size를 `5`로 설정한다.
8. `CameraFollowTarget`의 초기 위치를 `(Player X, 2, 0)`으로 설정한다.
9. Game View에서 Player가 보이도록 카메라 방향을 확인한다.

CameraFollow는 카메라 자체를 이동시키지 않는다. Player X를 따라가는 중간 기준점인 CameraFollowTarget을 이동시키고, Cinemachine Follow가 그 기준점으로부터 Offset을 유지하며 실제 카메라 위치를 계산한다.

완료 조건:

- CinemachineCamera에 Tracking Target과 Position Control이 모두 존재한다.
- Player가 움직일 때 CameraFollowTarget과 화면이 함께 X축으로 이동한다.

## Step 9. Phase 1 시작 흐름을 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 시작 흐름 정적 검사 및 사용자 Play Mode 확인 완료

1. Play Mode에 진입한다.
2. GameSystem이 Playing 상태까지 전환되는지 확인한다.
3. Runtime Data가 한 번만 생성되는지 확인한다.
4. StageHUD가 활성화되는지 확인한다.
5. Player Action Map이 활성화되는지 확인한다.
6. CameraFollow가 시작되는지 확인한다.
7. Console에 NullReferenceException과 필수 참조 Error가 없는지 확인한다.

완료 조건:

- Phase 1 흐름이 유지된 상태에서 Phase 2 System이 초기화된다.

## Step 10. 수평 이동을 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260720 이동 및 Inspector 속도·가속도 표시 사용자 Play Mode 검증 완료

1. A/D와 좌우 방향키로 이동한다.
2. Gamepad Left Stick으로 이동한다.
3. 입력을 놓고 감속을 확인한다.
4. 이동 중 Player의 Position Z와 Rotation을 확인한다.
5. Player의 PlayerControllerSystem Inspector에서 `Runtime Movement Debug`를 확인한다.
6. `Current Velocity`에서 현재 X/Y/Z 속도를 확인한다.
7. `Horizontal Acceleration (Signed)`에서 X축 가속도의 부호를 확인한다.

가속도 표시는 오른쪽을 양의 X축으로 하는 부호 있는 값이다.

- 오른쪽으로 가속: 양수
- 오른쪽 이동 중 감속: 음수
- 왼쪽으로 가속: 음수
- 왼쪽 이동 중 감속: 양수
- 목표 속도 유지 또는 정지 완료: `0`에 가까운 값

완료 조건:

- X축 이동과 감속이 동작한다.
- Z축 위치와 회전이 변하지 않는다.
- Inspector에서 현재 속도와 부호 있는 수평 가속도를 확인할 수 있다.

## Step 11. 점프와 착지 높이를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260720 컴파일 성공, Edit Mode 15개와 Play Mode 3개 통과, 오류 없음 및 사용자 동작 검증 완료

### Test Runner 자동 검증

1. Unity 메뉴에서 `Window > General > Test Runner`를 연다.
2. `EditMode` 탭을 선택한다.
3. `FlowState.EditModeTests`가 표시되는지 확인한다.
4. `Run All`을 누른다.
5. 아래 15개 Test Case가 모두 통과하는지 확인한다.

PlayerMovementMathTests:

- 기본 설정의 점프 초기 수직 속도
- 중력 `10`, `25`, `40`에서 목표 점프 높이 `3` 유지
- 지상 오른쪽 가속도
- 공중 오른쪽 가속도
- 양의 속도와 음의 속도에서 입력 해제 감속
- 오른쪽 감속과 왼쪽 감속의 가속도 부호

JumpFeatureTests:

- 지면 점프의 초기 수직 속도
- 공중 중복 점프 차단
- 코요테 타임 안의 점프 허용
- 코요테 타임 이후 점프 차단
- 착지 완료 후 다음 점프 허용

Test Runner 결과에 실패가 있으면 해당 Test 이름과 실패 메시지를 작업 기록에 남기고 다음 검증으로 넘어가지 않는다.

### Play Mode Test Runner 자동 검증

1. Test Runner에서 `PlayMode` 탭을 선택한다.
2. `FlowState.PlayModeTests`가 표시되는지 확인한다.
3. `Run All`을 누른다.
4. 아래 3개 통합 Test가 모두 통과하는지 확인한다.

- SampleScene 기본 점프가 목표 높이 `3 ± 0.25`에 도달하고 시작 Y로 복귀한다.
- 중력 `10`과 `40`에서도 실제 FixedUpdate 점프 높이가 `3 ± 0.25`를 유지한다.
- 하강 중 두 번째 점프 입력이 수직 속도를 다시 양수로 만들지 않는다.

Play Mode Test는 Build Settings에 등록된 `SampleScene`을 매 Test 시작 시 새로 로드한다. Scene의 GameSystem, PlayerMovementSystem, CollisionSystem, Rigidbody 설정을 실제 실행 순서로 사용한다.

### Play Mode 통합 확인

1. Player가 완전히 정지한 뒤 시작 Y를 기록한다.
2. Space로 한 번 점프한다.
3. 공중에서 Space를 다시 눌러 중복 점프가 발생하지 않는지 확인한다.
4. 완전히 착지한 뒤 Y를 다시 기록한다.
5. 시작 Y와 착지 Y를 비교한다.
6. Ground 가장자리에서 코요테 타임 안과 밖의 점프를 각각 확인한다.
7. PlayerMovementSystem의 Gravity Acceleration만 변경하고 목표 Jump Height가 유지되는지 확인한다.

수치 공식과 Feature 상태 규칙은 Test Runner로 판정한다. Play Mode에서는 실제 Scene 연결 결과인 점프 동작, 착지 위치, Rigidbody 반응만 확인한다.

완료 조건:

- 시작 Y와 착지 후 Y가 작은 물리 허용 오차 안에서 같다.
- `1.1 → 1.33`처럼 눈에 띄는 차이가 생기지 않는다.
- 공중 중복 점프가 없고 착지 후 다시 점프할 수 있다.
- 중력값을 바꿔도 목표 최고 높이는 유지된다.

## Step 12. 관성 착지를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260720 컴파일 성공, Edit Mode 26개와 Play Mode 6개 통과, 오류 없음 및 사용자 완료 승인
- 유보 사항: 최소 수동 확인 3·4·5번은 현재 Capsule과 Box 임시 비주얼로 판정하기 어려우므로 실제 Player 비주얼과 착지 연출 적용 후 다시 검증한다.

### Test Runner 자동 검증

1. Unity Script Compile이 끝난 뒤 Console에 Error와 Warning이 없는지 확인한다.
2. Test Runner의 `EditMode` 탭에서 `Run All`을 실행한다.
3. 전체 `26 Passed, 0 Failed`인지 확인한다.
4. Test Runner의 `PlayMode` 탭에서 `Run All`을 실행한다.
5. 전체 `6 Passed, 0 Failed`인지 확인한다.

Step 12에서 추가된 Edit Mode Test는 아래 규칙을 검증한다.

- 하강 중 지면 도달 시간이 Window 이내일 때만 Window 활성화
- 상승 중이거나 예측 지면이 없으면 Window 비활성화
- Window 안에서 입력한 경우에만 관성 착지 성공
- 양수와 음수 수평 속도에 동일한 배율과 방향 적용
- 최대 수평 속도 `14` 제한
- Window 이전 입력과 만료 이후 입력 무시
- 한 점프에서 관성 착지 한 번만 처리
- 관성 착지 실패 시 Normal Landing 처리
- 관성 착지 성공 시 Normal Landing 중복 처리 차단

Step 12에서 추가된 Play Mode Test는 실제 SampleScene에서 아래 흐름을 검증한다.

- Window 입력 성공 시 Runtime Data의 마지막 착지 유형과 Rigidbody 속도 증가
- 입력 없는 착지가 Normal Landing으로 종료
- Window 이전 입력이 관성 착지로 처리되지 않음

### 최소 수동 확인

1. 수평 이동 중 점프한다.
2. 착지 직전에 Left Shift를 누르고 착지한다.
3. 관성 착지 성공 후 수평 이동이 자연스럽게 이어지는지 확인한다.
4. 속도 변화가 화면에서 비정상적인 순간 이동으로 보이지 않는지 확인한다.
5. 입력하지 않은 일반 착지에서도 시각적인 떨림이나 중복 착지가 보이지 않는지 확인한다.
6. Console에 Error와 Warning이 없는지 확인한다.

IsGrounded, Ground Distance, Window 시간, Window 이전·이후 입력, 입력 없는 착지, 속도 배율과 중복 착지 판정은 Test Runner 결과로 판정한다.

현재 Phase 2에서는 자동 Test 결과를 주 검증 근거로 Step 12를 완료 처리한다. 수평 이동의 자연스러운 연결, 순간 이동 인상, 착지 떨림은 실제 Player 비주얼과 착지 연출 적용 후 후속 시각 검증 대상으로 유지한다.

완료 조건:

- 유효한 Window 입력만 Momentum Landing이 된다.
- 실패한 경우 Normal Landing이 된다.
- 한 점프에서 착지 판정은 한 번만 발생한다.
- Momentum Landing과 Normal Landing이 동시에 발생하지 않는다.

상태 확인 로그가 필요하면 상태 전환 순간에만 출력한다. 매 FixedUpdate 로그는 추가하지 않는다.

## Step 13. CameraFollow를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260720 컴파일 성공, Edit Mode 28개와 Play Mode 9개 통과, 오류 없음 및 사용자 수동 검증 완료

### Test Runner 자동 검증

1. Unity Script Compile이 끝난 뒤 Console에 Error와 Warning이 없는지 확인한다.
2. Test Runner의 `EditMode` 탭에서 `Run All`을 실행한다.
3. 전체 `28 Passed, 0 Failed`인지 확인한다.
4. Test Runner의 `PlayMode` 탭에서 `Run All`을 실행한다.
5. 전체 `9 Passed, 0 Failed`인지 확인한다.

Step 13에서 추가된 Test는 아래 항목을 자동 검증한다.

- CameraFollow가 Player X만 CameraFollowTarget에 반영한다.
- StopFollowing 이후에는 CameraFollowTarget을 변경하지 않는다.
- 실제 SampleScene 수평 이동에서 CameraFollowTarget과 Main Camera가 Player X를 추적한다.
- 점프 중 CameraFollowTarget과 Main Camera의 Y/Z 기준값이 유지된다.
- Main Camera가 Orthographic Projection과 Size `5`를 유지한다.

### 최소 수동 확인

1. Player를 좌우로 이동시킨다.
2. Game View에서 Player가 의도한 화면 기준 위치에 유지되는지 확인한다.
3. 수평 이동 중 카메라 떨림이나 순간 이동이 보이지 않는지 확인한다.
4. 점프할 때 카메라가 위아래로 따라가지 않는 구도가 자연스러운지 확인한다.
5. Console에 Error와 Warning이 없는지 확인한다.

좌표 추적, Y/Z 고정, Projection과 Size 값은 Test Runner 결과로 판정한다.

완료 조건:

- 카메라는 X축 이동만 따라가고 점프 높이에는 따라 올라가지 않는다.
- Player가 화면의 의도한 기준 위치에 유지된다.

## Step 14. 종료와 Phase 1 회귀를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260720 컴파일 성공, Edit Mode 28개와 Play Mode 11개 통과, 오류 없음 및 사용자 수동 검증 완료

### Test Runner 자동 검증

1. Unity Script Compile이 끝난 뒤 Console에 Error와 Warning이 없는지 확인한다.
2. Test Runner의 `EditMode` 탭에서 `Run All`을 실행한다.
3. 전체 `28 Passed, 0 Failed`인지 확인한다.
4. Test Runner의 `PlayMode` 탭에서 `Run All`을 실행한다.
5. 전체 `11 Passed, 0 Failed`인지 확인한다.

Step 14에서 추가된 Play Mode Test는 실제 SampleScene에서 아래 항목을 자동 검증한다.

- End Game 이후 Game State가 Ended가 된다.
- Runtime Data가 제거된다.
- Player Action Map이 비활성화된다.
- PlayerMovementSystem이 정지한다.
- CameraFollow가 정지한다.
- StageHUD가 비활성화되고 ResultPanel이 활성화된다.
- End Game 이후 Start Game을 다시 호출하면 Playing 상태와 Phase 2 System이 복구된다.

### 최소 수동 확인

1. GameSystem의 End Game 흐름을 실행한다.
2. StageHUD가 사라지고 ResultPanel이 의도한 화면 구성으로 표시되는지 확인한다.
3. 종료 화면에서 Player 조작과 카메라 추적이 계속되는 것처럼 보이지 않는지 확인한다.
4. 화면 전환에 시각적 떨림이나 잘못된 UI 중첩이 없는지 확인한다.
5. Console에 Error와 Warning이 없는지 확인한다.

Game State, Runtime Data, 입력, 이동, CameraFollow, UI 활성 상태와 재시작 복구는 Test Runner 결과로 판정한다.

완료 조건:

- 기존 Phase 1 시작·종료 흐름이 손상되지 않는다.
- Console에 치명적인 Runtime Error가 없다.

## Step 15. Scene과 검증 결과를 저장한다

- 진행 상태: **완료**
- 확인 근거: 20260720 파일 검사, 검증 결과 기록 및 사용자 Unity Editor 최종 저장 확인 완료

1. Play Mode를 종료한다.
2. Play Mode 중 변경한 Inspector 값이 원복되었는지 확인한다.
3. 필요한 값을 Edit Mode에서 다시 입력한다.
4. Scene과 Input Actions Asset을 저장한다.
5. Console 최종 상태를 확인한다.
6. 실제 시험값과 결과를 별도 Task 문서에 기록한다.

완료 조건:

- Scene을 다시 열어도 모든 참조와 설정이 유지된다.
- 아래 완료 체크리스트를 모두 만족한다.

---

# 완료 체크리스트

- [x] Console에 Compile Error와 치명적인 Runtime Error가 없다.
- [x] Player가 키보드와 게임패드로 이동한다.
- [x] Z축 이동과 회전이 없다.
- [x] 점프와 코요테 타임이 동작한다.
- [x] 시작 Y와 점프 후 착지 Y가 같다.
- [x] 공중 간격이 남아 있을 때 조기 접지하지 않는다.
- [x] 접지 판정과 착지 예측 판정이 독립적이다.
- [x] 중력 설정은 PlayerMovementSystem 한 곳에만 있다.
- [x] Momentum Landing 성공과 실패 경로가 구분된다.
- [x] 한 점프에서 착지는 한 번만 발생한다.
- [x] CameraFollowTarget은 Player X만 추적한다.
- [x] Cinemachine이 Follow Target을 사용해 실제 화면을 추적한다.
- [x] Phase 1 시작·종료 흐름이 정상 동작한다.
- [x] Scene과 Asset 변경 사항을 저장했다.

모든 항목을 확인한 후에만 Roadmap의 Phase 2 상태를 `완료`로 변경한다.

---

# 문제 발생 시 중단 기준

아래 문제가 있으면 수치 조정만 반복하지 말고 해당 책임의 코드 또는 연결을 먼저 점검한다.

| 현상 | 우선 확인 항목 |
| --- | --- |
| 점프 후 시작점보다 높은 곳에서 멈춤 | 실제 접지 Query와 예측 SphereCast 분리 여부 |
| 지면 근처에서 중력이 일찍 멈춤 | IsGrounded가 실제 Collider 접촉보다 먼저 참이 되는지 |
| 점프 높이가 중력 변경에 따라 달라짐 | JumpFeature가 PlayerMovementSystem 중력값을 전달받는지 |
| 버튼 입력이 가끔 사라짐 | 일회성 입력을 계산 완료 전에 소비하는지 |
| 관성 착지 Window가 열리지 않음 | 공중에서 유한한 Ground Distance가 제공되는지 |
| Target은 움직이지만 화면이 안 따라옴 | Cinemachine Follow Position Control 누락 여부 |
| 카메라가 점프를 따라 올라감 | CameraFollowTarget의 Y를 갱신하고 있는지 |
| 재시작 후 입력이 두 번 실행됨 | Input Action Callback 중복 등록 여부 |

---

# 영향 범위

- Unity Scene 및 Inspector 설정
- Input Actions Asset
- Phase 2 Play Mode 검증
- Implementation Roadmap 상태

---

# 관련 문서

- AI/README.md
- AI/00_Project/ARCHITECTURE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/02_Systems/GameSystem.md
- AI/02_Systems/PlayerInputSystem.md
- AI/02_Systems/PlayerControllerSystem.md
- AI/02_Systems/PlayerMovementSystem.md
- AI/02_Systems/CollisionSystem.md
- AI/02_Systems/CameraSystem.md
- AI/03_Features/Jump.md
- AI/03_Features/MomentumLanding.md
- AI/03_Features/NormalLanding.md
- AI/03_Features/CameraFollow.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/90_Tasks/20260709_01_Phase1ManualSteps.md

---

# 문서 갱신 결과

- 과거의 코드 생성 중심 절차를 현재 구현 상태 기준의 Unity 수동 실행 절차로 변경했다.
- 각 작업을 실행 순서, 확인 방법, 완료 조건이 있는 Step으로 표현했다.
- 접지 판정과 착지 예측 판정의 분리를 필수 검증 항목으로 추가했다.
- 점프 전후 Y 위치 비교를 추가해 조기 접지 문제를 검출하도록 했다.
- Cinemachine Tracking Target 외에 Position Control 구성이 필요함을 명시했다.
- 모든 검증을 통과한 뒤 Roadmap 상태를 변경하도록 완료 조건을 엄밀하게 정리했다.
