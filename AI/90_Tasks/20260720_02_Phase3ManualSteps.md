# 작업 정보

## 작업명

Phase 3 Manual Steps

---

## 작업 일자

20260720

---

## 작업 담당자

AI

---

## 작업 상태

완료

---

# 작업 목적

Phase 3: 스테이지 플레이 구성을 사용자가 Unity Editor에서 수동으로 수행하고 검증할 Step으로 정리한다.

Roadmap에 정의된 StageSystem, CollisionSystem, StagePlay, StageClear의 책임과 완료 조건을 기준으로 Scene 구성, 참조 연결, 실행 확인과 저장 순서를 명확하게 한다.

---

# 작업 대상

- Phase 3: 스테이지 플레이 구성
- StageSystem
- CollisionSystem
- StagePlay
- StageClear
- SampleScene의 Stage Object와 Goal
- Unity Test Runner와 Play Mode 수동 검증

---

# 작업 전 상태

- Roadmap의 Phase 1과 Phase 2는 완료 상태이다.
- SampleScene에는 Player, Ground, 이동 System, CollisionSystem과 Camera 구성이 존재한다.
- CollisionSystem은 Player의 실제 지면 접촉과 착지 후보 지면 탐지를 제공한다.
- StageSystem 생산 코드는 아직 존재하지 않는다.
- Stage 시작, 진행, Goal 도달, 클리어 확정과 Stage 종료 흐름은 아직 Scene에 구성되어 있지 않다.
- Phase 3 수동 작업 순서는 별도 문서로 정리되어 있지 않았다.

---

# 조사 내용

아래 문서와 구현 상태를 확인했다.

- AI/README.md
- AI/00_Project/PROJECT_OVERVIEW.md
- AI/00_Project/ARCHITECTURE.md
- AI/00_Project/PROJECT_MEMORY.md
- AI/01_Rules/AI_RULE.md
- AI/01_Rules/CODING_STYLE.md
- AI/01_Rules/EVENT_RULE.md
- AI/01_Rules/LOGGING_RULE.md
- AI/01_Rules/INVESTIGATION_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/02_Systems/README.md
- AI/02_Systems/StageSystem.md
- AI/02_Systems/CollisionSystem.md
- AI/03_Features/README.md
- AI/03_Features/StagePlay.md
- AI/03_Features/StageClear.md
- AI/04_Implementation_Roadmap/README.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/90_Tasks/20260710_01_Phase2ManualSteps.md
- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

확인한 기준은 아래와 같다.

- StageSystem은 Stage 시작, 진행 상태, Stage Object 상태와 종료 시점을 관리한다.
- StageSystem은 Stage Object를 직접 제어하거나 충돌을 판정하지 않는다.
- 일반 Stage Play는 Player가 Goal에 도달하면 종료된다.
- StageClear는 일반 Stage에서 하나의 Stage Play당 한 번만 수행한다.
- StageClear 자체는 결과 생성이나 결과 화면 전환을 담당하지 않는다.
- 충돌과 Trigger 연동은 Play Mode Test를 우선 사용하고, 화면 구성과 플레이 감각만 수동으로 확인한다.

---

# 현재 구현 상태

StageSystem, StageGoal과 Stage 상태 규칙을 검증하는 Play Mode Test 코드가 작성되어 있다.

Unity Editor Script Compile과 Test Runner 실행 결과는 아직 확인하지 않았다. Step 1은 사용자가 Unity Editor에서 Compile, Console과 Add Component 검색을 확인한 뒤 완료할 수 있다.

구현 후 실제 Component 이름과 Inspector Field가 이 문서의 표현과 다르면 임의로 유사한 참조를 연결하지 않는다. 관련 System·Feature 문서와 생산 코드를 다시 확인한 뒤 이 문서를 먼저 갱신한다.

---

# 수동 작업 전 원칙

- 기존 SampleScene과 Phase 1·2 구성을 재사용한다.
- 기존 Ground를 삭제하거나 Player 물리 설정을 임의로 변경하지 않는다.
- Stage Object는 동작 결과를 StageSystem에 전달하고, StageSystem이 Stage Object의 동작을 대신 수행하게 만들지 않는다.
- Goal 도달 판정은 구현에서 정의한 Collider 또는 Trigger 방식과 일치시킨다.
- Layer, Tag와 시험값은 생산 코드나 관련 문서에 정의된 값만 사용한다.
- Play Mode 중 변경한 Inspector 값은 저장되지 않으므로 필요한 설정은 Edit Mode에서 적용한다.
- 자동으로 판정 가능한 상태와 실행 횟수는 Test Runner 결과로 판단한다.

---

# Unity 수동 실행 Step

## Step 진행 상태

| Step | 작업 | 상태 |
| --- | --- | --- |
| 1 | Phase 3 구현과 Unity Compile 상태 확인 | 완료 |
| 2 | SampleScene 백업 기준과 기존 구성 확인 | 완료 |
| 3 | Stage 계층 구성 | 완료 |
| 4 | Stage 이동 지형 구성 | 완료 |
| 5 | Goal 구성 | 완료 |
| 6 | StageSystem Component와 참조 연결 | 완료 |
| 7 | GameSystem과 Stage 시작·종료 연결 확인 | 완료 |
| 8 | CollisionSystem과 Stage 지형 연결 확인 | 완료 |
| 9 | Stage 시작 흐름 검증 | 완료 |
| 10 | 지형 이동과 충돌 검증 | 완료 |
| 11 | Goal 도달과 StageClear 검증 | 완료 |
| 12 | 중복 종료와 예외 상태 검증 | 완료 |
| 13 | Phase 1·2 회귀 검증 | 완료 |
| 14 | Scene과 검증 결과 저장 | 완료 |

## Step 1. Phase 3 구현과 Unity Compile 상태를 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 StageSystem, StageGoal과 관련 Play Mode Test 코드 작성 및 정적 검사 완료
- 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `16 Passed, 0 Failed`, Step 1 수동 확인 5개 항목 완료

1. Phase 3 생산 코드와 관련 Test가 구현되었는지 확인한다.
2. StageSystem Component가 Unity의 `Add Component` 검색에 표시되는지 확인한다.
3. Unity Script Compile이 완료될 때까지 기다린다.
4. Console에 Compile Error와 Warning이 없는지 확인한다.
5. Error 또는 Warning이 있으면 Scene 구성을 시작하지 않고 원인을 기록한다.

완료 조건:

- Phase 3 Component를 Scene에 추가할 수 있다.
- Console에 Compile Error와 예상하지 않은 Warning이 없다.

확인 결과:

- Unity Script Compile에 성공했다.
- Console에 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `16`개가 성공했다.
- Step 1의 수동 확인 사항 `5`개가 모두 성공했다.

## Step 2. SampleScene 백업 기준과 기존 구성을 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 SampleScene 파일, 필수 계층, Phase 1·2 생산 Script 참조와 Git 변경 기준 정적 검사 완료
- 사용자 확인 근거: Unity Editor에서 Step 2 수동 확인 사항 6개 항목 모두 성공

1. `Assets/Scenes/SampleScene.unity`를 연다.
2. Hierarchy에서 GameRoot, Systems, World, Player와 CameraRig를 확인한다.
3. Ground, Player, CollisionSystem과 Phase 2 이동 구성이 유지되는지 확인한다.
4. Scene을 저장하고 변경 전 Git 상태를 확인한다.
5. 기존 오브젝트에 누락된 Script 또는 끊어진 참조가 없는지 Inspector에서 확인한다.

완료 조건:

- Phase 1·2 Scene 구성을 유지한 상태에서 Phase 3 구성을 시작할 수 있다.
- 기존 Scene의 누락된 참조가 없다.

정적 확인 결과:

- SampleScene 파일이 존재한다.
- GameRoot, Systems, World, Player, CameraRig와 Ground가 존재한다.
- GameSystem, RuntimeDataSystem, UIManagementSystem, PlayerInputSystem, PlayerMovementSystem, PlayerControllerSystem, CollisionSystem, CameraSystem과 Phase 2 Feature Script가 Scene에 연결되어 있다.
- `fileID: 0` 또는 빈 GUID 형태의 명백한 Missing Script 참조가 없다.
- Step 2 시작 시점의 Git 변경 기준은 Phase 3 문서, Roadmap, StageSystem, StageGoal과 StageSystemTests 관련 파일이다.

수동 확인 결과:

- SampleScene 열기에 성공했다.
- 필수 Hierarchy 구성을 확인했다.
- Inspector에 Missing Script가 없음을 확인했다.
- 기존 Component의 필수 참조가 누락되지 않았음을 확인했다.
- Edit Mode에서 Scene 저장을 완료했다.
- Scene을 다시 열어 기존 구성과 참조가 유지됨을 확인했다.

## Step 3. Stage 계층을 구성한다

- 진행 상태: **완료**
- 확인 근거: 20260720 현재 StageSystem과 StageGoal 구현에는 별도의 Stage Object 목록 또는 등록용 부모가 필요하지 않음을 확인
- 사용자 확인 근거: Unity Editor에서 Step 3 수동 확인 사항 6개 항목 모두 성공

1. World 아래에 일반 Stage 한 개를 나타내는 루트 오브젝트를 생성한다.
2. 구현에서 Stage Object 등록용 부모 또는 목록을 요구하면 해당 구조를 Stage 루트 아래에 생성한다.
3. 시작 지점, 이동 지형과 Goal을 Stage 루트 아래에서 구분한다.
4. Stage 루트의 Transform에 의도하지 않은 Position, Rotation 또는 Scale이 적용되지 않았는지 확인한다.
5. 오브젝트 이름은 역할을 식별할 수 있게 작성한다.

이번 Step에서 생성할 최소 계층은 아래와 같다.

```text
World
└─ Stage_01
   ├─ StartPoint
   ├─ Terrain
   └─ Goal
```

- `Stage_01`은 하나의 일반 Stage 범위를 나타내는 루트이다.
- `StartPoint`는 시작 위치를 구분하기 위한 빈 오브젝트이다.
- `Terrain`은 Step 4에서 이동 지형을 배치할 부모이다.
- `Goal`은 Step 5에서 Goal 판정 Component와 Trigger를 구성할 오브젝트이다.
- 이번 Step에서는 Collider, StageGoal 또는 StageSystem Component를 추가하지 않는다.

완료 조건:

- 하나의 일반 Stage를 구성하는 오브젝트 범위를 Hierarchy에서 식별할 수 있다.
- Stage Object가 기존 Systems 또는 UI 계층과 혼합되지 않는다.

확인 결과:

- World 아래에 Stage_01을 생성했다.
- Stage_01 아래에 StartPoint, Terrain과 Goal을 생성했다.
- 생성한 오브젝트의 Transform을 초기화했다.
- Stage 계층이 Systems 및 UI 계층과 분리되어 있음을 확인했다.
- Scene 저장을 완료했다.
- Scene을 다시 열어 계층과 Transform이 유지됨을 확인했다.

## Step 4. Stage 이동 지형을 구성한다

- 진행 상태: **완료**
- 확인 근거: 20260720 기존 Ground, Player 시작 위치, Ground Layer와 Collider 설정 정적 확인 완료
- 사용자 확인 근거: Unity Editor에서 Step 4 수동 확인 사항 6개 항목 모두 성공 및 지형 Y 기준 조정 완료

1. 기존 Ground를 시작 구간으로 재사용하거나 Stage 루트 아래에 포함할지 구현 구조에 맞춰 결정한다.
2. Player가 이동과 점프로 통과할 수 있는 지형을 배치한다.
3. 각 지형에 필요한 Collider를 설정한다.
4. 지형 Collider가 Trigger로 잘못 설정되지 않았는지 확인한다.
5. CollisionSystem이 사용하는 Ground Layer를 지형에 적용한다.
6. 3D 오소그래픽 횡스크롤 플레이를 벗어나는 Z축 경로가 생기지 않았는지 Scene View에서 확인한다.
7. 지형 사이에 의도하지 않은 틈, 겹침 또는 Player가 빠질 수 있는 구간이 없는지 확인한다.

Phase 2 검증 구성을 유지하기 위해 기존 Ground의 이름, Scale, Layer와 Collider를 재사용한다. Hierarchy의 부모를 `Terrain`으로 변경하고 지형 높이 기준에 맞춰 중심 Y를 `0`으로 조정한다.

초기 Stage 검증용 지형은 아래와 같이 구성한다.

| 오브젝트 | 부모 | Position | Rotation | Scale | Layer | Collider |
| --- | --- | --- | --- | --- | --- | --- |
| Ground | Terrain | `(0, 0, 0)` | `(0, 0, 0)` | `(40, 1, 4)` | Ground | 기존 BoxCollider, Is Trigger 해제 |
| Platform_01 | Terrain | `(6, 1, 0)` | `(0, 0, 0)` | `(4, 1, 4)` | Ground | BoxCollider, Is Trigger 해제 |
| Platform_02 | Terrain | `(12, 2, 0)` | `(0, 0, 0)` | `(4, 1, 4)` | Ground | BoxCollider, Is Trigger 해제 |

- Platform_01과 Platform_02는 Unity 3D Cube로 생성한다.
- 표의 Position과 Scale은 Transform의 Local 값이다. Stage_01과 Terrain의 Transform이 초기화되어 있으므로 World 값과 같다.
- 플랫폼은 기존 Ground 위에 배치하여 낙하 구간을 만들지 않고 점프와 착지 경로만 제공한다.
- Ground의 중심 Y를 `0`으로 사용하고 플랫폼 중심 Y를 `1` 단위로 높이는 기준을 사용한다.
- StartPoint와 Player의 Local Position은 `(0, 1.5, 0)`으로 설정한다.
- 초기 검증값은 Phase 3 연결과 충돌 확인을 위한 값이며 밸런스 확정값이 아니다.

완료 조건:

- Player가 시작 지점부터 Goal 앞까지 이동 가능한 연속 경로가 존재한다.
- 모든 이동 지형이 CollisionSystem의 Ground 탐지 대상이다.

확인 결과:

- 기존 Ground를 Terrain 아래로 이동했다.
- Platform_01과 Platform_02를 생성하고 Ground Layer와 비 Trigger BoxCollider를 설정했다.
- Ground 중심 Y를 `0`으로 사용하는 지형 높이 기준을 적용했다.
- Player와 StartPoint를 `(0, 1.5, 0)`으로 일치시켰다.
- 모든 이동 지형의 Z Position이 `0`임을 확인했다.
- Scene 저장 후 계층과 설정이 유지됨을 확인했다.

## Step 5. Goal을 구성한다

- 진행 상태: **완료**
- 확인 근거: 20260720 StageGoal의 Player Collider 참조 방식, Ground 범위와 Stage 지형 높이 기준 확인 완료
- 사용자 확인: Step 5 수동 확인 사항 7개 항목 성공 보고
- 재확인 결과: Goal BoxCollider의 `Is Trigger` 활성화, Scene 저장 및 재열기 포함 수동 확인 사항 4개 항목 성공

1. Stage의 마지막 지점에 Goal 오브젝트를 배치한다.
2. 구현에서 요구하는 Goal 판정 Component를 추가한다.
3. 구현에서 요구하는 Collider 또는 Trigger 설정을 적용한다.
4. Player만 Goal 도달 대상으로 판정되도록 구현에서 정의한 Layer, Tag 또는 참조를 설정한다.
5. Goal이 시작 지점에서 즉시 판정되지 않는 위치인지 확인한다.
6. Goal의 시각적 위치와 실제 판정 영역이 지나치게 어긋나지 않았는지 확인한다.

초기 Stage 검증용 Goal은 아래와 같이 구성한다.

### Goal 오브젝트

| 항목 | 설정값 |
| --- | --- |
| Parent | `World/Stage_01` |
| Local Position | `(18, 1.5, 0)` |
| Local Rotation | `(0, 0, 0)` |
| Local Scale | `(1, 1, 1)` |
| Layer | `Default` |
| Component | `BoxCollider`, `StageGoal` |
| BoxCollider Center | `(0, 0, 0)` |
| BoxCollider Size | `(1, 2, 4)` |
| BoxCollider Is Trigger | 활성화 |
| StageGoal Player Collider | `Player`의 `CapsuleCollider` |

- Goal은 Ground Layer를 사용하지 않는다.
- StageGoal은 Tag나 이름이 아니라 Inspector에 연결된 Player Collider와 동일한 Collider가 진입한 경우에만 Goal 도달로 판정한다.
- Goal Position Y=`1.5`와 Collider Size Y=`2`를 사용하면 판정 영역의 아래쪽이 Ground 윗면 Y=`0.5`에 맞는다.

### GoalVisual 오브젝트

Goal의 판정 영역을 Game View에서 식별하기 위해 Goal 아래에 3D Cube를 생성한다.

| 항목 | 설정값 |
| --- | --- |
| Name | `GoalVisual` |
| Parent | `Goal` |
| Local Position | `(0, 0, 0)` |
| Local Rotation | `(0, 0, 0)` |
| Local Scale | `(1, 2, 4)` |
| Collider | 제거 |

- GoalVisual의 BoxCollider는 제거하여 Trigger 판정과 지면 탐지에 영향을 주지 않게 한다.
- GoalVisual은 초기 검증용 임시 시각 오브젝트이며 최종 연출이 아니다.

완료 조건:

- Player가 Goal 영역에 도달했을 때만 Goal 도달 사실을 전달할 수 있다.
- 지형이나 다른 Stage Object가 Goal 도달로 오인되지 않는다.

확인 결과:

- Goal Transform과 Default Layer 설정을 확인했다.
- Goal BoxCollider의 Size가 `(1, 2, 4)`이고 `Is Trigger`가 활성화되어 있음을 확인했다.
- StageGoal의 Player Collider에 Player CapsuleCollider가 연결되어 있음을 확인했다.
- GoalVisual에 Collider가 없음을 확인했다.
- Scene 저장 후 Goal Component와 참조가 유지됨을 확인했다.

## Step 6. StageSystem Component와 참조를 연결한다

- 진행 상태: **완료**
- 확인 근거: 20260720 StageSystem의 필수 Inspector 참조가 Stage Goal 하나이며 SampleScene에는 아직 StageSystem이 구성되지 않았음을 확인
- 사용자 확인 근거: Unity Editor에서 Step 6 수동 확인 사항 7개 항목 모두 성공

1. Systems 아래에 StageSystem 오브젝트를 생성하거나 기존 Systems 구성 방식에 맞게 Component를 추가한다.
2. 현재 Stage 참조를 요구하면 Step 3에서 만든 일반 Stage를 연결한다.
3. Stage Object 목록 또는 이벤트 전달 대상 참조를 요구하면 Stage 루트의 실제 오브젝트를 연결한다.
4. 모든 필수 Inspector Field가 할당되었는지 확인한다.
5. 동일한 Stage Object가 중복 등록되지 않았는지 확인한다.
6. StageSystem이 Stage Object의 이동이나 Trigger 동작을 직접 수행하도록 별도 설정하지 않는다.

현재 구현에서는 아래 구성만 수행한다.

| 항목 | 설정값 |
| --- | --- |
| GameObject Name | `StageSystem` |
| Parent | `GameRoot/Systems` |
| Local Position | `(0, 0, 0)` |
| Local Rotation | `(0, 0, 0)` |
| Local Scale | `(1, 1, 1)` |
| Component | `StageSystem` |
| Stage Goal | `World/Stage_01/Goal`의 `StageGoal` Component |

- 현재 StageSystem에는 현재 Stage, Stage 루트 또는 Stage Object 목록을 연결하는 Inspector Field가 없다.
- Goal GameObject 자체가 아니라 Goal에 추가된 StageGoal Component를 `Stage Goal` Field에 연결한다.
- StageSystem GameObject에는 Collider, Rigidbody 또는 StageGoal을 추가하지 않는다.
- 이번 Step에서는 GameSystem에 StageSystem을 연결하지 않는다. GameSystem 실행 흐름 연결은 Step 7에서 수행한다.

완료 조건:

- StageSystem의 필수 참조가 모두 연결된다.
- StageSystem이 현재 Stage와 Stage Object 상태를 받을 준비가 된다.

확인 결과:

- GameRoot/Systems 아래에 StageSystem 오브젝트를 생성했다.
- StageSystem Transform이 초기화되어 있음을 확인했다.
- StageSystem Component가 하나만 연결되어 있음을 확인했다.
- Stage Goal Field에 Goal의 StageGoal Component가 연결되어 있음을 확인했다.
- StageSystem 오브젝트에 불필요한 Collider, Rigidbody 또는 StageGoal이 없음을 확인했다.
- Scene 저장 후 Component와 참조가 유지됨을 확인했다.

## Step 7. GameSystem과 Stage 시작·종료 연결을 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260720 GameSystem에 StageSystem 초기화, Stage 시작, Stage 종료 이벤트 수신과 게임 종료 연결 구현 및 생명주기 Test 확장 완료
- 검증 중 발견 사항: Step 4에서 Player 시작 Y를 `1.5`로 변경했으나 PlayerJumpIntegrationTests의 고정 기대값이 기존 `1.0`으로 남아 Play Mode Test 1개 실패
- 수정 결과: Jump 기본 설정 Test의 기대 시작 높이를 현재 SampleScene 기준인 `1.5`로 갱신
- 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `16 Passed, 0 Failed`, Step 7 수동 확인 5개 항목 완료

1. GameSystem Inspector에서 StageSystem 참조를 요구하는 경우 연결한다.
2. GameSystem의 시작 흐름이 Stage 시작 요청을 한 번 수행하도록 연결 상태를 확인한다.
3. Stage 종료 사실이 GameSystem에 전달되도록 구현된 연결을 확인한다.
4. Phase 4 대상인 TimerSystem, ResultSystem을 임시로 구현하거나 가짜 참조로 연결하지 않는다.
5. UIManagementSystem 연결이 Phase 3 구현 범위에 포함된 경우에만 실제 참조를 연결한다.
6. 직접 호출과 이벤트의 사용 방식이 생산 코드 및 EVENT_RULE과 일치하는지 확인한다.

현재 구현에서 Unity Editor로 연결할 Inspector Field는 아래와 같다.

| 대상 | Field | 연결 대상 |
| --- | --- | --- |
| GameSystem | Stage System | `GameRoot/Systems/StageSystem`의 `StageSystem` Component |

- GameSystem은 게임 시작 중 StageSystem을 초기화하고 Stage 시작을 한 번 요청한다.
- StageSystem의 Stage 종료 이벤트를 수신하면 기존 GameSystem 종료 절차를 시작한다.
- 명시적으로 게임을 종료하는 경우에도 진행 중인 Stage를 먼저 정지한다.
- TimerSystem과 ResultSystem은 추가하거나 연결하지 않는다.
- 기존 ResultPanel은 GameSystem의 기존 종료 UI 상태 전환만 유지하며 Phase 3에서 결과 데이터를 생성하지 않는다.

완료 조건:

- 게임 시작 시 Stage 시작 요청이 한 번 수행될 연결이 존재한다.
- Stage 종료 사실을 GameSystem이 받을 수 있다.
- Phase 4 책임을 Phase 3 Scene 구성에 임의로 추가하지 않는다.

확인 결과:

- GameSystem의 Stage System Field에 StageSystem Component가 연결되어 있음을 확인했다.
- Unity Script Compile에 성공하고 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `16`개가 성공했다.
- GameSystem 시작·종료와 StageSystem 생명주기 연결 Test가 통과했다.
- TimerSystem과 ResultSystem을 추가하지 않았음을 확인했다.
- Scene 저장 후 StageSystem 참조가 유지됨을 확인했다.

## Step 8. CollisionSystem과 Stage 지형 연결을 확인한다

- 진행 상태: **완료**
- 확인 근거: 20260721 CollisionSystem 참조와 LayerMask, Stage 지형 Layer, Goal Trigger Layer 정적 확인 및 Scene 구성 Play Mode Test 2개 작성 완료
- 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `18 Passed, 0 Failed`, Step 8 수동 확인 8개 항목 완료

1. CollisionSystem의 Player Collider와 GroundCheck 참조가 기존 설정을 유지하는지 확인한다.
2. Ground Layer에 Step 4의 모든 이동 지형이 포함되는지 확인한다.
3. Goal Trigger가 Ground Layer에 포함되어 접지 후보로 탐지되지 않는지 확인한다.
4. Player 시작 위치에서 실제 지면 접촉 상태가 정상인지 확인한다.
5. Goal 앞 마지막 지형에서도 실제 접지와 착지 후보 탐지가 가능한지 확인한다.

### Test Runner 자동 검증

`StageCollisionConfigurationTests`의 아래 2개 Test를 실행한다.

- Ground, Platform_01과 Platform_02가 Ground Layer와 비 Trigger BoxCollider를 사용하는지 확인한다.
- CollisionSystem의 Ground LayerMask가 Ground Layer를 포함하고 Player Collider와 GroundCheck 참조가 연결되어 있는지 확인한다.
- Goal이 Ground Layer를 사용하지 않고 Trigger Collider를 사용하는지 확인한다.
- GoalVisual에 Collider가 없는지 확인한다.

Step 8 코드 추가 후 예상 전체 Test 수는 Edit Mode `28`개, Play Mode `18`개이다.

### Unity Inspector 수동 확인

- Player의 CollisionSystem에서 Player Collider, Ground Check와 Ground Layer가 할당되어 있는지 확인한다.
- Ground, Platform_01과 Platform_02의 Layer가 Ground이고 BoxCollider의 Is Trigger가 해제되어 있는지 확인한다.
- Goal의 Layer가 Default이고 BoxCollider의 Is Trigger가 활성화되어 있는지 확인한다.
- GoalVisual에 Collider가 없는지 확인한다.

### 최소 Play Mode 확인

- Player가 시작 Ground 위에서 떨어지지 않고 안정적으로 서 있는지 확인한다.
- Platform_01과 Platform_02 위에 착지했을 때 바닥을 통과하지 않는지 확인한다.
- Goal 앞 Ground에서 이동과 점프가 정상적으로 유지되는지 확인한다.
- Console에 충돌 관련 Error와 반복 Warning이 없는지 확인한다.

완료 조건:

- Stage 지형은 접지 대상으로 탐지된다.
- Goal 판정 영역은 지면 접지 결과를 오염시키지 않는다.

확인 결과:

- CollisionSystem의 Player Collider, GroundCheck와 Ground Layer 참조가 유지됨을 확인했다.
- Ground, Platform_01과 Platform_02가 Ground Layer와 비 Trigger BoxCollider를 사용함을 확인했다.
- Goal이 Default Layer와 Trigger BoxCollider를 사용함을 확인했다.
- GoalVisual에 Collider가 없음을 확인했다.
- Unity Script Compile에 성공하고 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `18`개가 성공했다.
- 시작 Ground, 두 Platform과 Goal 앞 Ground에서 이동·점프·착지가 정상임을 확인했다.

## Step 9. Stage 시작 흐름을 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260721 StageSystem 초기화·중복 시작 차단 Test, GameSystem 실제 Scene 시작·재시작 Test와 SampleScene 참조 정적 확인 완료
- 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `18 Passed, 0 Failed`, Step 9 수동 확인 7개 항목 완료

### Test Runner 자동 검증

1. `Window > General > Test Runner`를 연다.
2. Edit Mode Test에서 Stage 상태 초기화와 중복 시작 방지 Test를 실행한다.
3. Play Mode Test에서 SampleScene의 GameSystem 시작과 StageSystem 연동 Test를 실행한다.
4. 관련 Test가 모두 통과하는지 확인한다.

Step 9에서 직접 확인할 기존 Play Mode Test는 아래와 같다.

- `StageSystemTests.Initialize_ValidReferences_PreparesStage`
- `StageSystemTests.StartStage_SecondRequest_IsRejected`
- `GameLifecycleIntegrationTests.StartGame_AfterEndGame_RestoresPlayingState`

전체 회귀를 포함한 예상 Test 수는 Edit Mode `28`개, Play Mode `18`개이다.

### 최소 수동 확인

1. SampleScene에서 Play Mode에 진입한다.
2. 현재 Stage가 준비되고 진행 상태로 전환되는지 Inspector의 공개 상태 또는 구현된 디버그 표시로 확인한다.
3. Player가 기존 시작 위치에서 이동할 수 있는지 확인한다.
4. Console에 필수 참조 Error와 예상하지 않은 Warning이 없는지 확인한다.

StageSystem의 런타임 상태는 일반 Inspector 표시가 아니라 위 Test 결과로 판정한다. 수동 확인에서는 Player가 시작 위치에서 조작 가능하고 StageHUD가 표시되는 실제 화면 결과를 확인한다.

완료 조건:

- 게임 시작 시 Stage Play가 한 번 시작된다.
- 시작되지 않은 Stage가 진행 상태로 잘못 표시되지 않는다.

확인 결과:

- Unity Script Compile에 성공하고 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `18`개가 성공했다.
- StageSystem 초기화와 중복 시작 차단 Test가 통과했다.
- GameSystem 시작 및 재시작 시 Stage가 진행 상태가 되는 Test가 통과했다.
- Play Mode에서 StageHUD가 표시되고 ResultPanel이 숨겨짐을 확인했다.
- Player가 시작 위치에서 정상적으로 이동함을 확인했다.

## Step 10. 지형 이동과 충돌을 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260721 SampleScene의 Ground와 두 Platform 실제 접지 지점 및 CollisionSystem 결과를 검증하는 Play Mode Test 작성 완료
- 검증 중 발견 사항: 실제 Physics Collider 위치가 갱신되기 전에 접지 Query를 실행하여 Platform_01 대신 기존 시작점의 Ground 접촉점 Y=`0.5`를 읽는 Test 실패
- 원인 재확인: `Rigidbody.position`을 변경한 같은 프레임에는 테스트가 기대한 Collider 위치 동기화가 완료되지 않음
- 수정 결과: 생산 CollisionSystem 변경을 되돌리고, Test에서 Player Transform 이동 후 `Physics.SyncTransforms()`를 호출하여 실제 Collider 위치를 동기화
- 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `19 Passed, 0 Failed`, Step 10 수동 확인 7개 항목 완료

### Test Runner 자동 검증

1. Stage 지형의 Collider와 Ground Layer를 사용하는 Play Mode Test를 실행한다.
2. Player가 지형 위에서 실제 접지 상태를 얻는지 확인한다.
3. Goal Trigger가 CollisionSystem의 Ground 결과에 포함되지 않는지 확인한다.

Step 10에서 추가한 `StageTerrainSurfaces_ProvideActualGroundContact` Test는 아래 항목을 실제 Unity Physics로 검증한다.

- Player를 Ground, Platform_01과 Platform_02 표면에 각각 배치한다.
- CollisionSystem이 각 지형에서 실제 접지 상태를 반환하는지 확인한다.
- CollisionSystem의 접촉 지점 Y가 각 Collider의 실제 윗면과 일치하는지 확인한다.

Step 10 코드 추가 후 예상 전체 Test 수는 Edit Mode `28`개, Play Mode `19`개이다.

### 최소 수동 확인

1. Player를 시작 지점부터 Goal 앞까지 이동시킨다.
2. 평지와 배치한 점프 구간에서 이동, 점프, 일반 착지와 관성 착지를 수행한다.
3. 지형 경계에서 Player가 끼이거나 의도하지 않게 통과하지 않는지 확인한다.
4. 카메라가 전체 이동 구간에서 Player의 X축 이동을 정상적으로 추적하는지 확인한다.
5. Console에 충돌 관련 Error와 반복 Warning이 없는지 확인한다.

완료 조건:

- Player가 Stage 지형을 따라 Goal 앞까지 진행할 수 있다.
- 기존 이동, 착지와 카메라 추적이 Stage 지형에서도 유지된다.

확인 결과:

- Unity Script Compile에 성공하고 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `19`개가 성공했다.
- Ground, Platform_01과 Platform_02에서 실제 접지 지점 검증이 통과했다.
- 시작 지점부터 Goal 앞까지 이동할 수 있음을 확인했다.
- 평지와 두 Platform에서 점프, 일반 착지와 관성 착지가 정상임을 확인했다.
- 지형 관통·끼임 없이 CameraFollow가 전체 이동 구간을 정상 추적함을 확인했다.

## Step 11. Goal 도달과 StageClear를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260721 실제 SampleScene Player Collider와 Goal Trigger를 사용하는 StageClear·Stage 종료·GameSystem 종료 Play Mode 통합 Test 작성 완료
- 1차 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `20 Passed, 0 Failed`, Step 11 수동 확인 5개 항목 완료
- 추가 발견 사항: Goal 도달 후 이동 계산과 카메라 추적은 중지되지만 Player Rigidbody의 기존 속도가 남아 Player가 화면 밖으로 계속 이동
- 수정 결과: PlayerControllerSystem에 Rigidbody 선속도·각속도 정지 책임을 추가하고 PlayerMovementSystem 종료 시 이를 요청하도록 수정, 게임 종료 및 Goal 통합 Test에 속도 0 검증 추가
- 최종 사용자 확인 근거: 수정 후 Step 11 재검증 사항 8개 항목 모두 성공

### Test Runner 자동 검증

1. 일반 Stage 진행 중 Goal 도달 시 StageClear가 한 번 수행되는 Test를 실행한다.
2. StageClear 후 현재 Stage Play가 클리어 상태로 확정되는지 확인한다.
3. StageClear와 Stage 종료 이벤트가 각각 한 번만 발생하는지 확인한다.
4. StageSystem이 진행을 중지하고 GameSystem이 Ended 상태가 되는지 확인한다.
5. Stage 종료 이벤트를 수신한 GameSystem이 기존 종료 UI 상태 전환을 수행하는지 확인한다.
6. Phase 4 ResultSystem 또는 결과 데이터 생성이 추가되지 않았는지 확인한다.

Step 11에서 추가한 `PlayerEntersGoal_ClearsStageAndEndsGameOnce` Test는 아래 실제 Scene 흐름을 검증한다.

- Player Rigidbody와 Collider가 Goal Trigger에 진입한다.
- StageClear와 Stage 종료 이벤트가 각각 한 번 발생한다.
- StageSystem이 Cleared 및 Ended 상태가 되고 진행을 중지한다.
- GameSystem이 Ended 상태가 된다.
- 기존 GameSystem 종료 흐름에 따라 StageHUD가 숨겨지고 ResultPanel이 표시된다.
- Stage 종료 시 Player Rigidbody의 선속도와 각속도가 0이 된다.

Step 11 코드 추가 후 예상 전체 Test 수는 Edit Mode `28`개, Play Mode `20`개이다.

### 최소 수동 확인

1. Play Mode에서 Player를 Goal까지 이동시킨다.
2. Goal에 진입한 뒤 Stage 진행이 종료되는지 확인한다.
3. Goal 영역 안에 머물러도 종료 처리가 반복되지 않는지 확인한다.
4. Goal 도달 직후 Player 조작과 카메라 추적이 중지되는지 확인한다.
5. StageHUD가 숨겨지고 기존 ResultPanel이 표시되는지 확인한다.
6. Console에 중복 이벤트나 필수 참조 관련 Error가 없는지 확인한다.

완료 조건:

- Goal 도달 시 StageClear와 Stage 종료가 각각 한 번 처리된다.
- 하나의 Stage Play는 하나의 종료 결과만 가진다.
- 기존 GameSystem 종료 UI 전환은 수행되지만 Phase 4 결과 데이터 생성은 수행되지 않는다.

확인 결과:

- Unity Script Compile에 성공하고 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `20`개가 성공했다.
- 실제 Goal Trigger 진입 시 StageClear와 Stage 종료가 각각 한 번 발생함을 확인했다.
- Stage 종료 후 Player 조작과 카메라 추적이 중지됨을 확인했다.
- StageHUD가 숨겨지고 기존 ResultPanel이 표시됨을 확인했다.
- 종료 시 Player Rigidbody의 선속도와 각속도가 즉시 0이 되어 추가 이동이 발생하지 않음을 확인했다.
- 재시작 후 Player 이동이 정상적으로 복구됨을 확인했다.

## Step 12. 중복 종료와 예외 상태를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260721 중복 시작·종료 기존 Test 확인, Goal 종료 후 Stage 상태·Player 위치·물리 상태를 복구하는 재시작 구현 및 Play Mode Test 2개 추가 완료
- 사용자 확인 근거: Unity Compile 성공, Error 및 Warning 없음, Edit Mode `28 Passed, 0 Failed`, Play Mode `22 Passed, 0 Failed`, Step 12 수동 확인 9개 항목 완료

1. Stage Play 시작 전 Goal 판정 요청이 무시되는지 자동 Test로 확인한다.
2. 진행 중 동일 Stage 시작 요청이 중복 처리되지 않는지 자동 Test로 확인한다.
3. Stage 종료 후 Goal 판정이 다시 들어와도 두 번째 StageClear와 종료 이벤트가 발생하지 않는지 확인한다.
4. 게임 진행 중단 상태에서는 Stage Play가 진행되지 않는지 확인한다.
5. 동일 Stage를 새 Stage Play로 다시 시작할 수 있는 구조가 구현된 경우 재시작 흐름을 확인한다.

Step 12 구현 후 PlayerControllerSystem Inspector의 `Start Point` Field에 `World/Stage_01/StartPoint` Transform을 연결한다.

Step 12에서 추가한 Test는 아래와 같다.

- `StageSystemTests.StartStage_AfterClear_ResetsStageState`
- `StageGoalIntegrationTests.StartGame_AfterGoalClear_RestoresNewStagePlay`

아래 항목을 자동 검증한다.

- StageClear 후 Stage를 다시 시작하면 Playing 상태로 복구된다.
- 이전 Cleared 및 Ended 상태가 새 Stage Play에 남지 않는다.
- Goal 종료 후 GameSystem을 다시 시작하면 Player가 StartPoint로 복귀한다.
- Player Rigidbody 속도가 0으로 초기화된다.

Step 12 코드 추가 후 예상 전체 Test 수는 Edit Mode `28`개, Play Mode `22`개이다.

### 최소 수동 확인

1. Play Mode에서 Goal에 도달하여 Stage를 종료한다.
2. GameSystem Component의 Context Menu에서 `Start Game`을 실행한다.
3. Player가 StartPoint로 즉시 복귀하는지 확인한다.
4. StageHUD가 다시 표시되고 ResultPanel이 숨겨지는지 확인한다.
5. Player 입력, 이동과 카메라 추적이 정상 복구되는지 확인한다.
6. 다시 Goal에 도달했을 때 두 번째 StageClear와 종료가 한 번만 수행되는지 확인한다.
7. Console에 Error 또는 예상하지 않은 Warning이 없는지 확인한다.

완료 조건:

- 시작 전, 중복 시작, 종료 후 입력이 Stage 상태를 손상시키지 않는다.
- 재시작은 이전 Stage Play의 상태와 종료 횟수를 이어받지 않는다.

확인 결과:

- PlayerControllerSystem의 Start Point에 Stage_01의 StartPoint가 연결되고 저장됨을 확인했다.
- Unity Script Compile에 성공하고 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `22`개가 성공했다.
- 시작 전 Goal, 중복 시작과 종료 후 Goal 입력이 Stage 상태를 손상시키지 않음을 확인했다.
- Goal 종료 후 재시작 시 Stage의 Cleared 및 Ended 상태가 초기화됨을 확인했다.
- Player가 StartPoint로 복귀하고 Rigidbody 속도가 초기화됨을 확인했다.
- StageHUD, Player 입력·이동과 CameraFollow가 정상 복구됨을 확인했다.
- 두 번째 Goal 도달도 StageClear와 종료를 각각 한 번만 수행함을 확인했다.

## Step 13. Phase 1·2 회귀를 검증한다

- 진행 상태: **완료**
- 확인 근거: 20260721 Phase 3 변경 영향 대상과 Phase 1·2 Edit Mode·Play Mode 회귀 Test Fixture 및 코드 diff 정적 검사 완료
- 사용자 확인 근거: Edit Mode `28 Passed, 0 Failed`, Play Mode `22 Passed, 0 Failed`, Step 13 수동 확인 9개 항목 완료, Error 메시지 없음

1. Edit Mode Test 전체를 실행한다.
2. Play Mode Test 전체를 실행한다.
3. 모든 기존 Test와 Phase 3 Test가 통과하는지 확인한다.
4. 키보드와 게임패드 수평 이동을 확인한다.
5. 점프, 코요테 타임, 일반 착지와 관성 착지를 확인한다.
6. CameraFollow의 X축 추적과 Y/Z 고정을 확인한다.
7. 기존 GameSystem 종료와 재시작 흐름이 Phase 3 변경으로 손상되지 않았는지 확인한다.
8. Console에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 자동 회귀 검증 범위

Edit Mode 전체 `28`개와 Play Mode 전체 `22`개를 실행한다.

Phase 1·2의 주요 Play Mode 회귀 대상은 아래와 같다.

- `GameLifecycleIntegrationTests`: 게임 종료, Runtime Data 정리와 재시작 복구
- `PlayerJumpIntegrationTests`: 기본 점프 높이, 중력 변경과 공중 중복 점프 차단
- `MomentumLandingIntegrationTests`: 관성 착지 성공, 일반 착지와 Window 이전 입력 무시
- `CameraFollowIntegrationTests`: X축 추적, Y/Z 기준 유지와 Orthographic 설정

### 최소 수동 회귀 확인

1. SampleScene에서 Play Mode에 진입한다.
2. 키보드와 게임패드로 좌우 이동한다.
3. 이동 중 Z축 위치와 Player 회전이 변하지 않는지 확인한다.
4. 점프, 공중 중복 점프 차단과 착지 후 재점프를 확인한다.
5. 일반 착지와 관성 착지를 각각 수행한다.
6. 카메라가 Player X축만 추적하고 점프 Y를 따라가지 않는지 확인한다.
7. GameSystem Context Menu의 `End Game`으로 종료한다.
8. 종료 시 Player와 카메라가 정지하고 StageHUD가 숨겨지는지 확인한다.
9. GameSystem Context Menu의 `Start Game`으로 재시작한다.
10. Player 위치, 입력, 이동, CameraFollow와 StageHUD가 정상 복구되는지 확인한다.
11. Console에 Error 또는 예상하지 않은 Warning이 없는지 확인한다.

이번 Step에서는 Scene Component나 Inspector 값을 변경하지 않는다.

완료 조건:

- Phase 1·2 자동 Test가 모두 통과한다.
- 기존 플레이 흐름에 회귀가 없다.

확인 결과:

- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `22`개가 성공했다.
- 키보드와 게임패드 수평 이동이 정상임을 확인했다.
- Z축 위치와 Player 회전 고정이 유지됨을 확인했다.
- 점프, 중복 점프 차단, 일반 착지와 관성 착지가 정상임을 확인했다.
- CameraFollow의 X축 추적과 Y/Z 기준 유지가 정상임을 확인했다.
- GameSystem 종료 시 Player, 카메라와 StageHUD 상태가 정상 전환됨을 확인했다.
- 재시작 시 Player 위치, 입력, 이동, CameraFollow와 StageHUD가 정상 복구됨을 확인했다.
- Console에 Error 메시지가 발생하지 않았음을 확인했다.

## Step 14. Scene과 검증 결과를 저장한다

- 진행 상태: **완료**
- 확인 근거: 20260721 SampleScene 필수 계층·참조, Missing Script, Phase 3 생성 파일, Git 변경 범위와 비 Scene diff 정적 검사 완료
- 사용자 확인 근거: Step 14 수동 확인 10개 항목 완료, Unity 빌드 성공, Edit Mode `28 Passed, 0 Failed`, Play Mode `22 Passed, 0 Failed`, Error 메시지 없음

1. Play Mode를 종료한다.
2. Play Mode 중 변경한 Inspector 값이 원복되었는지 확인한다.
3. 필요한 설정은 Edit Mode에서 다시 적용한다.
4. SampleScene과 변경된 Asset을 저장한다.
5. Scene을 다시 열어 StageSystem, Stage Object와 Goal 참조가 유지되는지 확인한다.
6. Console 최종 상태를 확인한다.
7. 실제 Test 수, 결과와 수동 확인 결과를 별도 Task 문서에 기록한다.
8. 모든 완료 조건을 만족한 뒤에만 Roadmap의 Phase 3 상태를 `완료`로 변경한다.

### 정적 확인 결과

- Stage_01, StartPoint, Terrain, 두 Platform, Goal, GoalVisual과 StageSystem이 Scene에 존재한다.
- GameSystem의 StageSystem, StageSystem의 StageGoal, StageGoal의 Player Collider와 PlayerControllerSystem의 StartPoint 참조가 연결되어 있다.
- SampleScene에 `fileID: 0` 또는 빈 GUID 형태의 명백한 Missing Script 참조가 없다.
- StageSystem, StageGoal과 Phase 3 Play Mode Test 파일 및 Meta 파일이 존재한다.
- Phase 3 작업 범위의 코드와 문서가 Git 변경 대상으로 확인된다.
- 코드와 문서의 `git diff --check`가 통과했다.

### 최종 수동 저장 확인

1. 모든 Test Runner 실행이 끝났는지 확인한다.
2. Play Mode를 종료한다.
3. Scene Hierarchy와 Inspector에서 Play Mode 중 변경한 값이 남지 않았는지 확인한다.
4. `File > Save` 또는 `Ctrl+S`로 SampleScene을 저장한다.
5. Unity Editor에서 SampleScene을 다시 연다.
6. Stage 계층과 필수 참조가 유지되는지 확인한다.
7. Console을 Clear한 뒤 Error와 예상하지 않은 Warning이 없는지 확인한다.
8. 사용자가 Unity 빌드를 실행하고 성공 여부를 확인한다.
9. 빌드 후 SampleScene과 Inspector 참조가 변경되지 않았는지 확인한다.

완료 조건:

- Scene을 다시 열어도 모든 Phase 3 참조와 설정이 유지된다.
- 검증 결과 Task 문서가 확인된 사실을 기준으로 작성된다.

확인 결과:

- Play Mode 종료 후 Edit Mode에서 SampleScene을 최종 저장했다.
- SampleScene 재열기 후 Stage 계층과 필수 Inspector 참조가 유지됨을 확인했다.
- Unity 빌드에 성공했다.
- Edit Mode Test 전체 `28`개가 성공했다.
- Play Mode Test 전체 `22`개가 성공했다.
- Console에 Error 메시지가 발생하지 않았음을 확인했다.
- Phase 3 검증 결과를 `20260721_01_Phase3VerificationResult.md`에 기록했다.

---

# 완료 체크리스트

- [x] Unity Compile Error와 예상하지 않은 Warning이 없다.
- [x] Stage 계층과 하나의 플레이 가능한 경로가 구성되어 있다.
- [x] StageSystem의 필수 참조가 연결되어 있다.
- [x] Stage 지형이 CollisionSystem의 Ground 탐지 대상이다.
- [x] Goal 판정 영역이 접지 결과를 오염시키지 않는다.
- [x] 게임 시작 시 Stage Play가 한 번 시작된다.
- [x] Player가 시작 지점부터 Goal까지 이동할 수 있다.
- [x] Goal 도달 시 StageClear가 한 번 수행된다.
- [x] Stage 종료 이벤트가 한 번 발생한다.
- [x] StageClear만으로 Phase 4 결과 데이터가 생성되지 않는다.
- [x] 중복 시작과 중복 종료가 차단된다.
- [x] Phase 1·2 회귀 Test가 모두 통과한다.
- [x] Scene과 Asset 변경 사항을 저장했다.
- [x] Phase 3 검증 결과를 별도 Task 문서에 기록했다.

모든 항목을 확인한 후에만 Roadmap의 Phase 3 상태를 `완료`로 변경한다.

---

# 문제 발생 시 중단 기준

| 현상 | 우선 확인 항목 |
| --- | --- |
| StageSystem을 추가할 수 없음 | Script Compile Error와 파일명·Class명 일치 여부 |
| Play Mode 시작 즉시 Stage가 종료됨 | Goal 위치, Trigger 중첩과 초기 Stage 상태 |
| Goal에 도달해도 종료되지 않음 | Goal 판정 Component, Player 식별 조건과 Stage Object 이벤트 전달 |
| 종료 처리가 여러 번 발생함 | Goal Trigger 반복 진입, StageClear 1회 제한과 종료 상태 방어 |
| Stage 지형에서 접지되지 않음 | Ground Layer, Collider, CollisionSystem LayerMask |
| Goal 근처에서 조기 접지함 | Goal Trigger가 Ground Layer에 포함되었는지 |
| Stage 종료 후 Player가 계속 움직임 | GameSystem 종료 수신과 PlayerMovementSystem 정지 흐름 |
| 기존 Test가 실패함 | Phase 3 변경이 GameSystem 또는 기존 Scene 참조에 준 영향 |

---

# 작업 내용

- Phase 3 Unity 수동 작업을 Scene 구성, 참조 연결, 실행 검증과 저장 순서로 정리했다.
- 자동 검증 대상과 최소 수동 확인 대상을 분리했다.
- StageSystem과 Stage Object의 책임 경계를 수동 작업 조건에 반영했다.
- Phase 4의 Timer, Result와 결과 화면 책임을 Phase 3에서 임의로 구성하지 않도록 명시했다.
- Phase 3 완료 전 Roadmap 상태를 변경하지 않도록 완료 기준을 정의했다.

---

# 영향 범위

- Tasks
- Implementation Roadmap 진행 상태

---

# 검증 내용

- Task 파일명 규칙과 당일 작업 번호를 확인했다.
- General Task Template의 필수 기록 항목을 반영했다.
- Phase 3 Roadmap 목표와 완료 조건이 각 Step에 포함되는지 확인했다.
- StageSystem, CollisionSystem, StagePlay와 StageClear 문서의 책임 경계를 확인했다.
- 현재 SampleScene과 생산 코드에 StageSystem이 없다는 사실을 반영했다.

---

# 검증 결과

- Phase 3 수동 작업을 실제 실행 순서의 14개 Step으로 정리했다.
- 각 Step에 진행 상태, 실행 절차와 완료 조건을 작성했다.
- 구현 전에는 실행할 수 없는 Step을 모두 `대기`로 구분했다.
- 확인되지 않은 Inspector Field명이나 수치값은 임의로 정의하지 않았다.

---

# 후속 작업

- Phase 3 생산 코드와 자동 Test를 구현한다.
- 구현된 Component와 Inspector Field를 기준으로 본 문서의 참조 연결 표현을 확정한다.
- 사용자가 Unity Editor에서 Step 1부터 순서대로 수행한다.
- 검증 완료 후 별도 Phase 3 Verification Result Task 문서를 작성한다.

---

# 관련 문서

- AI/README.md
- AI/00_Project/PROJECT_OVERVIEW.md
- AI/00_Project/ARCHITECTURE.md
- AI/00_Project/PROJECT_MEMORY.md
- AI/01_Rules/AI_RULE.md
- AI/01_Rules/CODING_STYLE.md
- AI/01_Rules/EVENT_RULE.md
- AI/01_Rules/LOGGING_RULE.md
- AI/01_Rules/INVESTIGATION_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/02_Systems/StageSystem.md
- AI/02_Systems/CollisionSystem.md
- AI/03_Features/StagePlay.md
- AI/03_Features/StageClear.md
- AI/04_Implementation_Roadmap/README.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

---

# 관련 작업 기록

- AI/90_Tasks/20260709_01_Phase1ManualSteps.md
- AI/90_Tasks/20260710_01_Phase2ManualSteps.md
- AI/90_Tasks/20260720_01_Phase2VerificationResult.md
