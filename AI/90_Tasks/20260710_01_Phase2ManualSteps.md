# 작업 정보

## 작업명

Phase 2 Manual Steps

---

## 작업 일자

20260710

---

## 작업 담당자

AI

---

## 작업 상태

작성 완료

---

# 작업 목적

Phase 2: 플레이어의 핵심 이동 구현을 사용자가 Unity에서 수동으로 수행할 수 있도록 구체적인 Step을 정리한다.

플레이어 이동, 점프, 관성 착지, 카메라 추적을 구현하고 검증하는 순서를 명확하게 한다.

---

# 작업 대상

- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- Phase 2: 플레이어의 핵심 이동 구현
- PlayerInputSystem
- PlayerControllerSystem
- PlayerMovementSystem
- CameraSystem
- CameraFollow Feature
- Jump Feature
- MomentumLanding Feature
- NormalLanding Feature
- Phase 2 지면 판정에 필요한 최소 CollisionSystem

---

# 작업 전 상태

Phase 1이 완료되어 게임 시작과 종료 흐름, Runtime Data, 기본 UI 흐름이 존재한다.

현재 프로젝트에서 확인한 실행 환경은 아래와 같다.

- Unity Editor: `6000.3.5f2`
- Input System Package: `1.17.0`
- 기존 Input Actions: `Assets/InputSystem_Actions.inputactions`
- 기존 Player Action Map: `Player`
- 기존 이동 Action: `Move`
- 기존 점프 Action: `Jump`
- Cinemachine Package: 설치되어 있지 않음
- 플레이어 이동 관련 스크립트: 아직 생성되어 있지 않음

---

# 조사 내용

아래 문서를 확인했다.

- AI/README.md
- AI/00_Project/ARCHITECTURE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/01_Rules/CODING_STYLE.md
- AI/02_Systems/README.md
- AI/02_Systems/GameSystem.md
- AI/02_Systems/PlayerInputSystem.md
- AI/02_Systems/PlayerControllerSystem.md
- AI/02_Systems/PlayerMovementSystem.md
- AI/02_Systems/CollisionSystem.md
- AI/02_Systems/CameraSystem.md
- AI/03_Features/README.md
- AI/03_Features/Jump.md
- AI/03_Features/MomentumLanding.md
- AI/03_Features/NormalLanding.md
- AI/03_Features/CameraFollow.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

확인된 책임 경계는 아래와 같다.

- PlayerInputSystem은 Player Action Map과 입력 상태만 관리한다.
- PlayerMovementSystem은 입력 상태와 충돌 상태를 이용하여 이동 결과만 계산한다.
- PlayerControllerSystem은 계산된 이동 결과를 Rigidbody에 적용한다.
- CollisionSystem은 Unity Physics 충돌 정보를 판정하고 결과를 제공한다.
- CameraSystem은 Cinemachine Camera 설정과 상태를 관리한다.
- Jump, MomentumLanding, NormalLanding, CameraFollow의 동작 규칙은 Feature에서 관리한다.
- PlayerMovementSystem이 Rigidbody를 직접 제어하거나 PlayerControllerSystem이 이동 결과를 계산하면 안 된다.

---

# Phase 2 선행 결정 및 의존성

## 관성 착지 입력

현재 Feature 문서에는 관성 착지 입력의 장치별 버튼이 정의되어 있지 않다.

Phase 2 기능 검증을 위해 아래 바인딩을 초기 시험값으로 사용한다.

- Keyboard: `Left Shift`
- Gamepad: `Button West`
- Action 이름: `MomentumLanding`
- Action Type: `Button`

이 바인딩은 기능 규칙이 아니라 Phase 2 시험용 설정이다. 최종 조작 체계가 결정되면 Input Actions와 관련 문서를 함께 갱신한다.

## CollisionSystem 선행 범위

Phase 2 완료 조건에는 점프와 착지가 포함되지만 CollisionSystem은 Roadmap의 Phase 3 구현 대상이다.

PlayerMovementSystem이 직접 충돌 판정을 하면 System 책임을 위반하므로 Phase 2에서는 CollisionSystem의 아래 범위만 먼저 구현한다.

- Player가 지면에 닿아 있는지 판정한다.
- Player와 지면 사이의 거리를 제공한다.
- 접촉 지점과 표면 법선을 제공한다.
- 이동, 점프, 관성 착지 여부는 결정하지 않는다.

Stage 충돌, 목표 지점, 경사면 규칙 등 나머지 CollisionSystem 범위는 Phase 3에서 구현한다.

## CameraFollow 시작 조건

CameraFollow Feature의 정식 시작 조건은 Stage Play 시작이지만 StageSystem은 Phase 3 대상이다.

Phase 2에서는 Play Mode 시작 후 GameSystem이 플레이 상태가 되었을 때 CameraFollow를 시작하여 추적 기능만 검증한다.

Phase 3에서 StageSystem이 구현되면 CameraFollow 시작과 종료 요청을 Stage Play 시작 및 종료 흐름으로 이전한다.

---

# 작업 내용

## 1. Phase 1 완료 상태 확인

Unity에서 `Assets/Scenes/SampleScene.unity`를 연다.

아래 항목을 확인한다.

- Play Mode에 진입할 수 있다.
- GameSystem이 `Playing` 상태까지 전환된다.
- Runtime Data 생성 로그가 출력된다.
- StageHUD가 활성화된다.
- Console에 Compile Error와 Null Reference Error가 없다.

Phase 1 오류가 남아 있으면 Phase 2 오브젝트를 추가하기 전에 먼저 해결한다.

## 2. Cinemachine 설치

Unity 메뉴에서 `Window > Package Management > Package Manager`를 연다.

아래 순서로 설치한다.

1. Packages 목록을 `Unity Registry`로 변경한다.
2. `Cinemachine`을 검색한다.
3. 현재 Unity Editor와 호환되는 표시 버전을 설치한다.
4. 설치 후 Console에 Package Error가 없는지 확인한다.
5. `Packages/manifest.json`에 Cinemachine 항목이 추가되었는지 확인한다.

버전 번호를 문서에서 임의로 고정하지 않는다. Package Manager가 현재 Editor에 제공하는 호환 버전을 사용한다.

## 3. Phase 2 폴더 생성

Unity Project 창에서 아래 폴더를 생성한다.

- `Assets/Scripts/Runtime/Features`
- `Assets/Prefabs`
- `Assets/Prefabs/Player`
- `Assets/PhysicsMaterials`

기존 폴더는 다시 만들지 않는다.

- 공통 상태와 System 간 전달 데이터: `Assets/Scripts/Runtime/Core`
- System 구현: `Assets/Scripts/Runtime/Systems`
- Feature 규칙 구현: `Assets/Scripts/Runtime/Features`

## 4. Input Actions 구성

`Assets/InputSystem_Actions.inputactions`를 연다.

### Player Action Map 정리

기존 `Player` Action Map에서 아래 Action을 사용한다.

- `Move`: Value, Vector2
- `Jump`: Button
- `MomentumLanding`: Button

기존 `Sprint` Action은 Phase 2에서 사용하지 않는다. `Sprint`를 `MomentumLanding`으로 이름을 변경하고 바인딩을 교체하거나, 별도의 `MomentumLanding` Action을 추가한다.

혼동을 방지하기 위해 Phase 2 코드에서는 반드시 `MomentumLanding` 이름만 참조한다.

### Move 바인딩

아래 바인딩이 존재하는지 확인한다.

- Keyboard `A/D`
- Keyboard `Left Arrow/Right Arrow`
- Gamepad `Left Stick`

3D 횡스크롤 이동에서는 `Move`의 X 값만 사용하고 Y 값은 이동 계산에 사용하지 않는다.

### Jump 바인딩

아래 바인딩이 존재하는지 확인한다.

- Keyboard `Space`
- Gamepad `Button South`

### MomentumLanding 바인딩

아래 시험용 바인딩을 추가한다.

- Keyboard `Left Shift`
- Gamepad `Button West`

### C# Wrapper 생성

Input Actions Asset Inspector에서 아래 항목을 설정한다.

- `Generate C# Class`: 활성화
- Class Name: `InputSystem_Actions`
- Namespace: `FlowState.Input`

`Apply`를 눌러 C# Wrapper를 생성한다.

생성 후 Console에 Input Actions 관련 Compile Error가 없는지 확인한다.

## 5. Core 데이터 스크립트 생성

`Assets/Scripts/Runtime/Core`에 아래 스크립트를 생성한다.

- `E_PlayerMovementState.cs`
- `PlayerInputState.cs`
- `PlayerCollisionState.cs`
- `PlayerMovementResult.cs`
- `PlayerMovementRuntimeData.cs`

실제 C# 코드는 이 문서에 작성하지 않는다. 각 파일은 아래 방향으로 작성한다.

### E_PlayerMovementState.cs

플레이어 이동 상태를 나타내는 Enum을 작성한다.

아래 상태를 구분할 수 있어야 한다.

- 초기 상태
- 지면 이동
- 공중 이동
- 관성 착지
- 일반 착지

Enum 이름은 프로젝트 규칙에 따라 `E_` 접두어를 사용한다.

### PlayerInputState.cs

한 번의 이동 계산에 필요한 입력 Snapshot을 표현한다.

아래 값만 포함한다.

- 수평 이동 입력값
- 이번 계산 주기에 점프 입력이 시작되었는지 여부
- 이번 계산 주기에 관성 착지 입력이 시작되었는지 여부

키보드 KeyCode나 Gamepad 버튼 경로는 포함하지 않는다. 장치별 처리는 PlayerInputSystem 내부에만 둔다.

### PlayerCollisionState.cs

CollisionSystem이 PlayerMovementSystem에 제공할 충돌 결과를 표현한다.

아래 값을 포함한다.

- 지면 접촉 여부
- 지면까지의 거리
- 접촉 지점
- 접촉 표면 법선

점프 가능 여부나 관성 착지 성공 여부는 포함하지 않는다. 해당 판단은 Feature 규칙이다.

### PlayerMovementResult.cs

PlayerMovementSystem이 PlayerControllerSystem에 제공할 계산 결과를 표현한다.

아래 값을 포함한다.

- 최종 속도 벡터
- 현재 이동 상태
- 이번 계산에서 점프가 시작되었는지 여부
- 이번 계산에서 착지가 발생했는지 여부
- 착지 종류

Rigidbody 참조나 Transform 참조는 포함하지 않는다.

### PlayerMovementRuntimeData.cs

게임 실행 중 확인할 플레이어 이동 상태를 보관한다.

아래 값을 포함한다.

- 현재 이동 상태
- 현재 수평 속도
- 현재 수직 속도
- 지면 여부
- Momentum Landing Window 활성 여부
- 마지막 착지가 관성 착지인지 여부

초기화 Method와 제거 시 값을 초기 상태로 되돌리는 Method를 제공한다.

GameRuntimeData가 PlayerMovementRuntimeData를 소유하도록 확장하되 저장 기능은 추가하지 않는다.

## 6. Feature 스크립트 생성

`Assets/Scripts/Runtime/Features`에 아래 스크립트를 생성한다.

- `JumpFeature.cs`
- `MomentumLandingFeature.cs`
- `NormalLandingFeature.cs`
- `CameraFollow.cs`

Feature 스크립트에는 해당 Feature의 규칙과 판정만 작성한다.

### JumpFeature.cs

아래 규칙을 처리한다.

- 지면 상태 또는 코요테 타임 동안만 점프를 허용한다.
- 공중에서 중복 점프를 허용하지 않는다.
- 점프가 시작되면 공중 상태로 전환한다.
- 지정된 점프 높이와 중력 가속도로 초기 수직 속도를 계산한다.
- 중력 가속도 값이 변경되어도 목표 점프 높이가 유지되도록 계산한다.
- 착지하면 다음 점프를 허용한다.

점프 높이, 중력 가속도, 코요테 타임은 외부에서 초기화할 수 있는 설정값으로 둔다.

Rigidbody를 직접 변경하거나 Input System을 직접 읽지 않는다.

### MomentumLandingFeature.cs

아래 규칙을 처리한다.

- 플레이어가 하강 중이고 착지가 임박했을 때 Momentum Landing Window를 한 번 활성화한다.
- Window 안에서 들어온 관성 착지 입력을 한 번만 기억한다.
- 입력이 기억된 상태에서 지면 착지가 발생하면 관성 착지 성공으로 판정한다.
- 성공 시 착지 직전 수평 속도를 유지하거나 배율만큼 증가시킨다.
- 하나의 점프에서 한 번만 성공하도록 상태를 잠근다.
- 착지 또는 게임 종료 시 Window와 입력 상태를 초기화한다.

Window 시간, 속도 배율, 최대 수평 속도는 외부 설정값으로 둔다.

지면 판정은 직접 수행하지 않고 PlayerCollisionState를 사용한다.

### NormalLandingFeature.cs

아래 규칙을 처리한다.

- 공중 상태에서 지면 접촉이 시작되었는지 확인한다.
- 같은 착지에서 MomentumLandingFeature가 성공하지 않은 경우에만 일반 착지로 판정한다.
- 하나의 점프에서 한 번만 일반 착지를 수행한다.
- 착지 후 지면 이동 상태로 전환한다.

수평 속도 증가나 관성 착지 판정을 직접 수행하지 않는다.

### CameraFollow.cs

아래 규칙을 처리한다.

- Player Transform의 X 위치만 추적한다.
- 카메라 Follow Target의 Y와 Z는 초기값을 유지한다.
- 추적 시작 전 Player와 Follow Target 참조를 확인한다.
- 추적 중 Player가 사라지면 갱신을 중단하고 Warning을 남긴다.
- 추적 시작과 종료를 Public Method로 제공한다.

Cinemachine Camera 설정을 직접 생성하거나 변경하지 않는다. CameraSystem이 관리하는 Follow Target의 위치만 갱신한다.

## 7. System 스크립트 생성

`Assets/Scripts/Runtime/Systems`에 아래 스크립트를 생성한다.

- `PlayerInputSystem.cs`
- `PlayerMovementSystem.cs`
- `PlayerControllerSystem.cs`
- `CollisionSystem.cs`
- `CameraSystem.cs`

### PlayerInputSystem.cs

아래 책임만 작성한다.

- 생성된 `InputSystem_Actions` Wrapper를 생성하고 관리한다.
- Player Action Map 활성화와 비활성화 Method를 제공한다.
- `Move`, `Jump`, `MomentumLanding` 입력을 수집한다.
- 이동 입력은 현재 값을 유지한다.
- 점프와 관성 착지 입력은 다음 물리 계산에서 한 번 소비할 수 있도록 임시 저장한다.
- PlayerInputState를 반환하는 Method를 제공한다.
- 입력이 비활성화되면 저장된 입력값을 초기화한다.
- OnDestroy에서 등록한 Input Action Callback을 해제한다.

이 System은 이동, 점프 가능 여부, 착지 성공 여부를 판단하지 않는다.

### CollisionSystem.cs

Phase 2에서는 아래 최소 책임만 작성한다.

- Inspector에서 Player Collider, Ground Check Transform, Ground Layer를 연결받는다.
- Unity Physics Query 또는 Collider 접촉 정보를 이용해 지면 상태를 갱신한다.
- PlayerCollisionState를 제공한다.
- 자신의 Collider는 지면 판정에서 제외한다.
- Trigger는 지면으로 판정하지 않는다.

CollisionSystem은 Player를 이동시키거나 점프 및 착지 종류를 결정하지 않는다.

### PlayerMovementSystem.cs

아래 책임만 작성한다.

- PlayerInputSystem에서 PlayerInputState를 받는다.
- CollisionSystem에서 PlayerCollisionState를 받는다.
- 현재 속도는 PlayerControllerSystem에서 제공받는다.
- 수평 입력과 가속도 설정을 이용해 목표 수평 속도를 계산한다.
- JumpFeature, MomentumLandingFeature, NormalLandingFeature를 이용해 수직 속도와 착지 결과를 계산한다.
- 계산 결과를 PlayerMovementResult로 만든다.
- PlayerControllerSystem에 결과 적용을 요청한다.
- PlayerMovementRuntimeData를 갱신한다.

물리 계산은 `FixedUpdate` 주기에 맞춘다.

Player GameObject의 Transform이나 Rigidbody를 직접 변경하지 않는다.

### PlayerControllerSystem.cs

아래 책임만 작성한다.

- Inspector에서 Player Rigidbody를 연결받는다.
- 현재 Rigidbody 속도와 Player 위치를 제공한다.
- PlayerMovementResult를 받아 Rigidbody 속도에 반영한다.
- Z축 위치와 회전이 변하지 않도록 Rigidbody 설정을 유지한다.
- Rigidbody가 연결되지 않았을 때 명확한 Error를 남긴다.

입력 수집, 이동 속도 계산, 충돌 판정은 작성하지 않는다.

Unity 6 Rigidbody API에서 현재 프로젝트가 사용하는 속도 Property 이름을 Editor 자동 완성과 Package API로 확인한 후 사용한다.

### CameraSystem.cs

아래 책임만 작성한다.

- Inspector에서 Cinemachine Camera와 Camera Follow Target을 연결받는다.
- 오소그래픽 Projection과 기본 Camera Size를 적용한다.
- Follow Target을 Cinemachine Camera의 Tracking Target으로 연결한다.
- 현재 Follow Target과 카메라 활성 상태를 제공한다.
- 필수 참조가 누락되면 Error를 남긴다.

Player Transform을 매 프레임 직접 추적하는 규칙은 작성하지 않는다. 해당 규칙은 CameraFollow가 담당한다.

## 8. Phase 2 시험용 설정값

아래 값은 기능 연결을 확인하기 위한 초기 시험값이다. 최종 게임 규칙이나 밸런스 값이 아니다.

- Move Speed: `8`
- Ground Acceleration: `50`
- Air Acceleration: `25`
- Maximum Horizontal Speed: `14`
- Jump Height: `3`
- Gravity Acceleration: `25`
- Coyote Time: `0.10`초
- Momentum Landing Window: `0.15`초
- Momentum Speed Multiplier: `1.15`
- Ground Check Radius: `0.25`
- Ground Check Distance: `0.20`
- Orthographic Camera Size: `5`

각 값은 `[SerializeField] private` 설정으로 노출하고 Inspector에서 조정할 수 있게 한다.

중력은 PlayerMovementSystem의 계산과 Rigidbody 양쪽에서 중복 적용하지 않는다.

## 9. Scene 계층 구성

`Assets/Scenes/SampleScene.unity`에 아래 구조를 추가한다.

```text
GameRoot
Systems
  GameSystem
  RuntimeDataSystem
  UIManagementSystem
  PlayerInputSystem
  PlayerMovementSystem
  CameraSystem
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

기존 Phase 1 오브젝트는 삭제하거나 이름을 바꾸지 않는다.

## 10. Ground 오브젝트 구성

`World/Ground`를 3D Cube로 생성한다.

시험용 Transform은 아래 값으로 설정한다.

- Position: `(0, -0.5, 0)`
- Scale: `(40, 1, 4)`

아래 설정을 적용한다.

- `BoxCollider` 유지
- 새 Layer `Ground` 생성 후 적용
- `Is Trigger` 비활성화
- Rigidbody는 추가하지 않음

## 11. Player 오브젝트 구성

빈 GameObject를 만들고 이름을 `Player`로 지정한다.

시험용 Transform은 아래 값으로 설정한다.

- Position: `(0, 1, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(1, 1, 1)`

Player에 아래 Component를 추가한다.

- `Rigidbody`
- `CapsuleCollider`
- `PlayerControllerSystem`
- `CollisionSystem`

Rigidbody는 아래와 같이 설정한다.

- Use Gravity: 비활성화
- Is Kinematic: 비활성화
- Interpolate: `Interpolate`
- Collision Detection: `Continuous`
- Constraints Position Z: 고정
- Constraints Rotation X/Y/Z: 고정

CapsuleCollider는 Visual 크기에 맞추고 Player의 발 위치가 Ground 위에 놓이도록 Center와 Height를 조정한다.

`Player/Visual`에는 Phase 2 확인용 Capsule 또는 Cube Mesh를 둔다. Collider는 부모 Player에만 둔다.

`Player/GroundCheck`는 Player 발바닥 중앙보다 약간 아래에 배치한다.

## 12. Player System 오브젝트와 스크립트 부착

아래 기준으로 부착한다.

- `Systems/PlayerInputSystem`: PlayerInputSystem
- `Systems/PlayerMovementSystem`: PlayerMovementSystem
- `Player`: PlayerControllerSystem
- `Player`: CollisionSystem

Inspector 참조는 아래와 같이 연결한다.

### PlayerControllerSystem

- Player Rigidbody: `Player`의 Rigidbody

### CollisionSystem

- Player Collider: `Player`의 CapsuleCollider
- Ground Check: `Player/GroundCheck`
- Ground Layer: `Ground`

### PlayerMovementSystem

- Player Input System: `Systems/PlayerInputSystem`
- Player Controller System: `Player`의 PlayerControllerSystem
- Collision System: `Player`의 CollisionSystem
- Runtime Data System: `Systems/RuntimeDataSystem`
- 이동 및 Feature 시험값: 이 문서의 Phase 2 시험용 설정값

## 13. Cinemachine Camera 구성

기존 `Main Camera`는 유지하고 아래 사항을 확인한다.

- Main Camera Tag가 `MainCamera`이다.
- Cinemachine이 요구하는 Brain Component가 Main Camera에 존재한다.
- Projection 결과가 Orthographic으로 표시된다.

`CameraRig/CinemachineCamera`에 설치된 Cinemachine 버전의 Cinemachine Camera Component를 추가한다.

`CameraRig/CameraFollowTarget`의 초기 위치는 Player를 화면에 표시할 수 있도록 설정한다.

예시 초기값은 아래와 같다.

- Position X: Player 시작 X와 동일
- Position Y: `2`
- Position Z: `0`

Cinemachine Camera의 카메라 거리 또는 Follow Offset에서 Z축 거리를 확보한다. Main Camera가 횡스크롤 평면을 바라보도록 Rotation을 설정한다.

CameraSystem과 CameraFollow를 아래와 같이 부착한다.

- `Systems/CameraSystem`: CameraSystem
- `CameraRig`: CameraFollow

Inspector 참조는 아래와 같이 연결한다.

### CameraSystem

- Cinemachine Camera: `CameraRig/CinemachineCamera`
- Follow Target: `CameraRig/CameraFollowTarget`
- Orthographic Size: `5`

### CameraFollow

- Player: `Player`
- Follow Target: `CameraRig/CameraFollowTarget`

Cinemachine 버전에 따라 Component와 Inspector Field 이름이 달라질 수 있으므로 설치된 버전의 `Tracking Target` 또는 같은 역할의 Follow 설정에 `CameraFollowTarget`을 연결한다.

## 14. GameSystem 연결 확장

GameSystem에 아래 Serialized Field를 추가한다.

- PlayerInputSystem
- PlayerMovementSystem
- PlayerControllerSystem
- CollisionSystem
- CameraSystem
- CameraFollow

GameSystem의 시작 흐름에 아래 요청을 추가한다.

1. 기존 Phase 1 Runtime Data를 생성한다.
2. Player 이동 Runtime Data를 초기화한다.
3. PlayerControllerSystem과 CollisionSystem을 초기화한다.
4. PlayerMovementSystem을 초기화한다.
5. CameraSystem을 초기화한다.
6. Player Action Map을 활성화한다.
7. Phase 2 검증을 위해 CameraFollow를 시작한다.
8. 게임 상태를 Playing으로 변경한다.

종료 흐름에는 아래 요청을 추가한다.

1. Player Action Map을 비활성화한다.
2. CameraFollow를 종료한다.
3. PlayerMovementSystem의 실행 상태를 초기화한다.
4. 기존 Runtime Data 제거 흐름을 실행한다.

GameSystem이 Rigidbody를 직접 변경하거나 CameraFollowTarget을 직접 이동시키면 안 된다.

## 15. 물리 계산 실행 순서

한 번의 `FixedUpdate`에서 아래 순서를 유지한다.

1. PlayerInputSystem에서 현재 PlayerInputState를 가져온다.
2. CollisionSystem에서 현재 PlayerCollisionState를 가져온다.
3. PlayerControllerSystem에서 현재 Rigidbody 속도를 가져온다.
4. PlayerMovementSystem이 수평 속도를 계산한다.
5. JumpFeature가 점프 시작과 수직 속도를 계산한다.
6. MomentumLandingFeature가 Window 및 관성 착지 성공 여부를 계산한다.
7. MomentumLanding이 성공하지 않은 착지는 NormalLandingFeature가 처리한다.
8. PlayerMovementResult를 생성한다.
9. PlayerControllerSystem에 결과 적용을 요청한다.
10. PlayerMovementRuntimeData를 갱신한다.
11. 일회성 점프 및 관성 착지 입력을 소비한다.

입력 수집은 프레임 사이의 짧은 버튼 입력을 놓치지 않도록 Callback에서 임시 저장하고 물리 계산 후 소비한다.

## 16. 이동 검증

Play Mode에서 아래 항목을 확인한다.

- `A/D` 또는 방향키 좌우 입력으로 X축 이동이 가능하다.
- Gamepad Left Stick 좌우 입력으로 X축 이동이 가능하다.
- 입력하지 않으면 목표 속도를 향해 감속한다.
- Player의 Z 위치가 변하지 않는다.
- Player가 회전하지 않는다.
- PlayerMovementSystem이 Rigidbody를 직접 변경하지 않는다.
- PlayerControllerSystem이 입력을 직접 읽지 않는다.

## 17. 점프 검증

아래 순서로 확인한다.

1. 지면에서 `Space`를 눌러 점프한다.
2. 공중에서 다시 `Space`를 눌러도 중복 점프가 발생하지 않는지 확인한다.
3. 지면 가장자리를 벗어난 직후 코요테 타임 안에 점프가 가능한지 확인한다.
4. 코요테 타임 이후에는 점프가 시작되지 않는지 확인한다.
5. 착지 후 다시 점프할 수 있는지 확인한다.
6. Gravity Acceleration을 다른 값으로 바꾼 뒤 Jump Height가 동일하게 유지되는지 확인한다.

점프 최고 높이는 Scene View 또는 Debug 표시로 시작 위치 대비 높이를 비교한다.

## 18. 관성 착지 검증

아래 순서로 확인한다.

1. 수평 이동 중 점프한다.
2. 하강하며 지면에 가까워질 때 Momentum Landing Window가 활성화되는지 Runtime Data 또는 Debug 표시로 확인한다.
3. Window 안에서 `Left Shift`를 누른 뒤 착지한다.
4. 관성 착지 후 수평 속도가 유지되거나 시험 배율만큼 증가하는지 확인한다.
5. Window 전에 입력하면 성공하지 않는지 확인한다.
6. Window가 끝난 후 입력하면 성공하지 않는지 확인한다.
7. 입력하지 않고 착지하면 Normal Landing으로 판정되는지 확인한다.
8. 하나의 점프에서 착지 판정이 두 번 발생하지 않는지 확인한다.
9. Momentum Landing과 Normal Landing이 동시에 발생하지 않는지 확인한다.

판정 확인용 로그는 상태가 실제로 전환되는 순간에만 남긴다. 매 프레임 로그를 출력하지 않는다.

## 19. CameraFollow 검증

아래 항목을 확인한다.

- Play Mode 시작 후 Cinemachine Camera가 활성화된다.
- Player가 X축으로 이동하면 카메라가 X축을 따라간다.
- Player가 점프해도 CameraFollowTarget의 Y 값은 변하지 않는다.
- CameraFollowTarget의 Z 값은 변하지 않는다.
- 카메라가 Orthographic Projection을 유지한다.
- Player가 화면에서 확인 가능한 위치에 유지된다.
- Player 참조가 없을 때 Null Reference Error 대신 명확한 Warning 또는 Error가 출력된다.

## 20. Phase 1 회귀 검증

Phase 2 구현 후 아래 Phase 1 흐름을 다시 확인한다.

- 게임 시작 시 Runtime Data가 한 번만 생성된다.
- StageHUD가 정상적으로 표시된다.
- GameSystem의 `End Game`을 호출할 수 있다.
- 종료 시 Player Action Map이 비활성화된다.
- 종료 시 CameraFollow가 멈춘다.
- ResultPanel이 표시된다.
- Runtime Data가 제거된다.
- Console에 치명적인 Error가 없다.

## 21. 완료 조건 확인

아래 조건을 모두 직접 확인한 경우에만 Phase 2 구현 완료로 판단한다.

- 플레이어가 키보드 또는 게임패드 입력으로 이동할 수 있다.
- 플레이어가 점프할 수 있다.
- 점프 높이가 중력 설정과 무관하게 목표값을 유지한다.
- 코요테 타임이 동작한다.
- Momentum Landing Window가 착지 직전에 활성화된다.
- Window 입력 성공 시 관성 착지가 적용된다.
- 실패 시 Normal Landing으로 연결된다.
- 하나의 점프가 하나의 착지로 종료된다.
- 카메라가 X축으로 Player를 추적하고 Y/Z 기준값을 유지한다.
- Phase 1 시작 및 종료 흐름이 계속 동작한다.
- Console에 Compile Error와 치명적인 Runtime Error가 없다.

## 22. Roadmap 및 작업 기록 갱신

Phase 2 구현을 시작할 때 Roadmap의 Phase 2 상태를 `진행 중`으로 변경한다.

모든 완료 조건과 회귀 검증을 통과한 후에만 아래 내용을 반영한다.

- Phase 2 상태를 `완료`로 변경한다.
- 현재 개발 진행 상태의 다음 작업을 Phase 3으로 변경한다.
- 완료된 단계에 Phase 2를 추가한다.
- 실제 구현 및 검증 결과는 별도의 Task 문서에 기록한다.

이 Manual Steps 문서의 작성 완료는 Phase 2 구현 완료를 의미하지 않는다.

---

# 영향 범위

- Tasks
- Implementation Roadmap

---

# 검증 내용

- Roadmap의 Phase 2 구현 대상과 완료 조건을 확인했다.
- 관련 System과 Feature 문서의 책임 및 동작 규칙을 확인했다.
- 현재 Unity 버전, Input System 버전, Input Actions 구성, Cinemachine 미설치 상태를 확인했다.
- 폴더, 스크립트, 데이터 구조, Scene 오브젝트, Component, Inspector 참조, 실행 순서, 검증 절차가 포함되었는지 확인했다.
- 실제 C# 코드를 문서에 직접 작성하지 않았는지 확인했다.
- Phase 2에 필요한 CollisionSystem과 StageSystem 의존성을 별도로 표시했는지 확인했다.

---

# 검증 결과

Phase 2 수동 수행 절차를 현재 프로젝트 문서와 실행 환경을 기준으로 작성했다.

관성 착지 입력 바인딩은 문서에 확정된 요구사항이 아니므로 Phase 2 시험용 설정임을 명시했다.

착지 판정에 필요한 CollisionSystem은 책임 중복을 피하기 위해 지면 판정 범위만 Phase 2 선행 작업으로 구분했다.

CameraFollow의 정식 Stage Play 연동은 StageSystem이 구현되는 Phase 3 후속 작업으로 구분했다.

---

# 후속 작업

- Phase 2 구현 시작 시 Roadmap 상태를 진행 중으로 변경한다.
- 이 문서 순서에 따라 Unity Scene과 스크립트를 구현한다.
- Phase 2 검증 결과를 별도의 구현 작업 기록에 작성한다.
- Phase 3에서 CameraFollow 시작 조건을 Stage Play 흐름으로 이전한다.
- Phase 3에서 CollisionSystem의 나머지 충돌 책임을 구현한다.

---

# 관련 문서

## Project

- AI/00_Project/ARCHITECTURE.md
- AI/00_Project/PROJECT_OVERVIEW.md
- AI/00_Project/PROJECT_MEMORY.md

## Rules

- AI/01_Rules/AI_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/01_Rules/CODING_STYLE.md

## Systems

- AI/02_Systems/GameSystem.md
- AI/02_Systems/RuntimeDataSystem.md
- AI/02_Systems/PlayerInputSystem.md
- AI/02_Systems/PlayerControllerSystem.md
- AI/02_Systems/PlayerMovementSystem.md
- AI/02_Systems/CollisionSystem.md
- AI/02_Systems/CameraSystem.md

## Features

- AI/03_Features/Jump.md
- AI/03_Features/MomentumLanding.md
- AI/03_Features/NormalLanding.md
- AI/03_Features/CameraFollow.md

## Template

- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

---

# 관련 작업 기록

- AI/90_Tasks/20260709_01_Phase1ManualSteps.md

---

# 작성 완료 기준

모든 섹션을 작성했다.

확인한 문서와 프로젝트 상태에 근거한 내용을 작성했다.

문서에 정의되지 않은 시험값과 임시 연결은 최종 요구사항과 구분했다.

실제 C# 코드를 작성하지 않고 코드의 책임, 입력, 출력, 실행 순서만 설명했다.

System 작업, Feature 작업, 버그 수정 작업이 아닌 수동 절차 정리이므로 GENERAL_TASK_TEMPLATE.md를 사용했다.
