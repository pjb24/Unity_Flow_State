# 작업 정보

## 작업명

Prototype 4 Phase 4 수동 작업 및 검증 계획

## 작업 일자

20260914

## 작업 담당자

AI, 사용자

## 작업 상태

Step 1–7 완료 · 사용자 Build 성공 결과 추가

---

# 작업 목적

네 Map Pattern과 연결 구간에 Collectible 안내 경로를 적용하고 Difficulty 상태 UI, Score 유지와 Stage Mode 회귀를 검증한다. 사용자는 Unity Editor에서 실제 Asset·UI 제작과 화면 판단만 수행한다. 수치, 참조, 상태 전환과 물리 결과는 AI의 정적 검사 및 Unity Test Runner의 자동 Test로 판정한다.

# 작업 전 상태

- Phase 3은 완료 (Build 제외)로 기록됐다. 전체 Edit Mode `483`개, Play Mode `210`개 Test와 결정적 `Flat → SingleRise` 화면 확인이 통과했다.
- 생산 Scene은 `Assets/Scenes/SampleScene.unity`이고, 네 원본은 `Assets/Prefabs/InfinitePatterns/`에 있다. 각 원본의 `CollectibleRoot`는 비어 있다. 두 Slot의 Scope 생성·해제·재연결은 이미 동작한다.
- `ScoreCollectible`은 ID, Sphere Trigger, Renderer와 Player Layer 참조가 필요하다. Stage Mode의 Collectible 오브젝트는 현재 Scene에 존재한다.
- InfiniteHUD와 InfiniteResultContent에는 Distance Score, Collectible Score와 Total Score 텍스트 및 참조가 있다. `UIManagementSystem`은 합계 표시와 Result 반영 코드를 이미 갖고 있다. 현재 Difficulty를 나타내는 UI 필드와 생산 Scene 텍스트는 없다.
- 기존 `InfiniteCollectibleLayoutIntegrationTests.ProductionPatterns_KeepEmptyPhase2RootsAndTwoScopes`는 빈 Root를 기대하므로 Phase 4 배치 후 생산 계약에 맞춰 수정해야 한다.

# 작업 원칙

- AI는 문서·코드·Scene YAML·Prefab·`.meta`·GUID를 먼저 정적으로 조사하고, 배치 좌표·ID·수량과 UI 문구·참조를 구현 전에 문서로 확정한다. 사용자가 Inspector 값을 읽거나 눈대중으로 수치를 정하게 하지 않는다.
- 실제 수치가 확정되기 전에는 Prefab이나 UI를 임의로 제작하지 않는다. AI는 사용자에게 편집할 Asset·GameObject·Component·필드·값을 표로 전달한다.
- 후보 배치의 도달 가능성, 입력 구간, Score 포화·중복·초기화, Difficulty 전환과 UI 문자열은 Edit Mode Unit Test를 우선한다. 생산 Prefab의 실제 Collider·Trigger·획득·연결 및 Scene UI는 Play Mode Test로 판정한다.
- 자동 판정 가능한 Pattern 출현, 경계 타이밍, Collectible 개수·좌표, Score 숫자와 UI 상태를 수동 플레이로 확인하지 않는다. 화면 가독성·안내성·시각적 연속성만 수동으로 본다.
- 새 Runtime·Test에는 LINQ를 사용하지 않는다. Test 전용 선택·점수 알고리즘을 복제하지 않고 생산 코드를 호출한다. Scene·Prefab 편집 후 `.meta`·GUID·Layer·참조·변경 파일 범위를 정적으로 재확인한다.
- AI는 Unity Editor의 Test Runner를 대신 실행하지 않는다. 사용자는 지정된 Test를 Editor에서 직접 실행하고 결과를 전달한다. Phase 4에서는 Build 관련 작업을 수행하지 않는다.

# 수행 Step

## Step 1. Phase 4 생산 계약과 실제 편집 범위를 확정한다

### AI 작업

- `InfiniteMode.md`, `ScoreCollectible.md`, `ScoreRecord.md`, `UIManagementSystem.md`와 Roadmap Phase 4를 현재 Runtime·생산 Scene·네 Prefab에 대조한다.
- Pattern별 Jump 구간과 경계 Gap, Player 속도·Jump 궤적, `CollectibleRoot`, Stage Collectible 구성, 기존 HUD·Result 참조를 정적으로 조사한다.
- 총개수·좌표·ID, 기존 Score/UI 재사용 범위, Difficulty 표시 형식과 수명주기 중 문서에 확정되지 않은 결정을 분리한다. 구현 결과에 영향을 주는 미정 정책은 배치·UI 제작 전에 사용자와 확정한다.
- 사용자가 실제로 편집해야 하는 대상과 AI 코드·Test로 처리할 대상을 구분한다.

### 사용자 수동 작업

- 없음. 이 단계는 문서·코드·직렬화 파일 검사로 처리한다. 필요한 정책 결정이 생기면 AI가 구체적인 선택지를 제시한다.

### 완료 조건

- [x] 생산 구조와 필수 Asset·UI 편집 범위가 명확하다.
- [x] 미정 UI 정책을 사용자와 확정했다. 수치는 Inspector 설정이나 임의의 배치로 대체하지 않는다.

### Step 1 조사 결과 (2026-09-14)

- 생산 Scene `SampleScene.unity`는 두 Slot과 네 독립 Pattern Prefab을 GUID로 참조한다. 네 Prefab의 `CollectibleRoot`는 모두 비어 있다. 따라서 Collectible 제작 대상은 네 Prefab이며 Scene에 Pattern Collectible을 직접 추가할 필요는 없다.
- Pattern 길이는 모두 44이고 경계 Ground Gap은 4이다. 내부 Jump 대상은 `SingleRise` 상승, `LegacySteps`의 두 Platform 진입·이탈, `InternalGap` 내부 Gap이다. `Flat`은 첫 정지 출발에서 Jump가 필요 없고, 대체 Pattern일 때는 앞뒤 경계 안내가 필요하다. 네 Pattern의 모든 앞뒤 연결을 Step 2 계산에 포함한다.
- 생산 Player 값은 기본 속도 8, 최대 속도 14, Jump 높이 3, 중력 25, Capsule 반지름 0.5·높이 2이고 최소 Jump 입력 구간은 0.10초이다. Stage Collectible 10개는 Scene 오브젝트이며 SphereCollider 반지름 0.5, Trigger, 같은 오브젝트의 Renderer와 `ScoreCollectible`, Player Layer 마스크 1을 사용한다. 별도 재사용 Prefab은 확인되지 않았다.
- InfiniteHUD 및 InfiniteResultContent에는 Distance·Distance Score·Collectible Score·Total Score용 텍스트와 `UIManagementSystem` 참조가 있다. 현재 Difficulty용 TMP 텍스트나 직렬화 필드는 없다. Difficulty 상태는 `InfiniteModeSystem` 내부에 있고 HUD가 읽는 `InfiniteModeRuntimeData`에는 없다. 따라서 Difficulty 표시에는 AI의 상태 노출·UI 바인딩 코드와 사용자의 Scene UI 편집이 필요하다. 기존 점수 텍스트는 의미가 고정되어 있으므로 Difficulty 표시에 재사용하지 않는다.
- `ScoreRecord`의 Total Score 포화 계산과 HUD·Result의 합계 표시 코드는 이미 존재한다. Phase 4에서는 이를 재사용하고 전환·초기화·회귀 Test를 보강한다. 생산 Prefab에 Collectible을 추가하면 빈 Root를 기대하는 기존 `InfiniteCollectibleLayoutIntegrationTests`를 변경해야 한다.
- 조사 당시 Pattern별 Collectible 개수·ID·좌표·경계 소유·충돌 반지름과 Difficulty UI 정책이 미정이었다. 아래 사용자 결정을 정책으로 사용하고, 실제 수치와 Scene 작업표는 후속 Step에서 확정한다. 이 Step 자체에 필요한 사용자 Unity Editor 수동 작업은 없다.

### Step 1 확정 정책 (사용자 결정)

- Collectible은 유효 Jump 구간마다 도약 전 1개·공중 3개·착지 부근 1개의 5개 묶음을 출발점으로 삼고, 짧은 구간은 도달 가능성 검증에 따라 줄인다. 정확한 개수와 좌표는 Step 2에서 확정한다.
- 연결 경계의 도약 안내는 앞 Pattern, 착지 안내는 뒤 Pattern이 소유한다. 경계의 공중 안내는 Step 2에서 두 Pattern의 재배치·활성 시점과 모든 연결 조합을 검증해 소유를 확정한다.
- Pattern 내부 ID는 역할을 읽을 수 있도록 `jump-구간-순번`과 `entry/exit-순번` 계열을 사용한다. 실제 ID 목록은 Pattern 내부 중복이 없도록 Step 2에서 확정한다.
- SphereCollider 반지름은 Stage Collectible과 같은 `0.5`를 기본 후보로 사용하고, 경로 검증에 필요한 경우에만 변경한다. 실제 Prefab별 값은 Step 2 배치표에 기록한다.
- Difficulty는 플레이어에게 제공하는 게임 정보가 아니라 개발자 확인 정보이다. 개발 중에는 InfiniteHUD의 기존 정보 묶음에 별도 행으로 `Difficulty: D1`, `Difficulty: D2`, `Difficulty: D3`를 표시한다. 일반 플레이어용 실행에서는 표시하지 않는다.
- 개발자 표시가 활성화된 동안 Pause와 Result에서는 마지막 Difficulty를 유지하고 Retry·새 Run에서 D1으로 초기화한다. 일반 플레이어용 실행에서는 Pause·Result를 포함해 Difficulty를 표시하지 않는다.
- Difficulty UI의 구체적인 표시 활성화 방법과 Scene 편집 필드·배치는 Step 5에서 기존 UI 구조와 함께 확정한다. 사용자에게 Inspector에서 개발자 표시 여부를 수동 전환하도록 요구하지 않는다.

## Step 2. Collectible 배치표와 Unit Test를 확정한다

### AI 작업

- 네 Pattern 각각의 내부 Jump 구간과 모든 연결 경계에 대해 도약 전·공중·착지 안내 후보를 계산한다. `ScoreCollectible.md`의 기본 5개 묶음을 출발점으로 사용하되 유효 입력 구간과 실제 통과 가능성을 기준으로 수량을 조정한다.
- 각 Prefab의 `CollectibleRoot` 하위에 만들 오브젝트의 이름, Pattern 내부 고유 ID, Local Position, SphereCollider 반지름·Layer·Renderer 조건을 표로 문서화한다. 첫 `Flat` 정지 출발, `Flat` 대체와 네 Pattern의 모든 앞뒤 연결을 포함한다.
- 생산 이동·Jump 계산과 후보 배치를 사용하는 Edit Mode Unit Test로 경계·도달 가능성·중복 ID·누락 경로·비정상 값을 검증한다. 좌표를 Test 코드에 복제한 뒤 같은 값을 서로 비교하는 형식은 피한다.

### 사용자 수동 작업

- 없음. 아직 Prefab을 편집하지 않는다.

### 완료 조건

- [x] 모든 Pattern 및 연결 경계의 배치표가 수치와 ID까지 확정됐다.
- [x] 배치·통과 계산 관련 Unit Test가 준비됐고 정적 검사를 통과했다. 실행 결과는 Step 4에서 확인한다.

### Step 2 확정 배치 계약 (2026-09-14)

배치표의 좌표는 각 Prefab Root 및 `CollectibleRoot` 기준 Local Position이다. 모든 오브젝트의 부모는 해당 Prefab의 `CollectibleRoot`, Layer는 `Default`(0), Local Rotation은 `(0, 0, 0)`, Local Scale은 `(1, 1, 1)`이다. 각 오브젝트는 같은 GameObject의 `SphereCollider`(Center `(0, 0, 0)`, Radius `0.5`, Is Trigger), Renderer, `ScoreCollectible`을 사용한다. `ScoreCollectible`의 `_collectibleId`는 표의 ID, `_triggerCollider`와 `_visual`은 같은 오브젝트의 컴포넌트, `_playerLayers`는 `Default` Layer 마스크 `1`이다. Stage Collectible의 형태와 Material은 제작 시 시각 기준으로 재사용한다. 이 표의 이름은 ID와 동일하게 사용한다.

| Prefab | 이름 및 Pattern 내부 ID | Local Position |
|---|---|---|
| Flat | `entry-land-01` | `(-16.66, 1.5, 0)` |
| Flat | `exit-pre-01` | `(17, 1.5, 0)` |
| Flat | `exit-air-01` | `(21, 3.36, 0)` |
| Flat | `exit-air-02` | `(23, 4.47, 0)` |
| Flat | `exit-air-03` | `(25, 4.01, 0)` |
| SingleRise | `entry-land-01` | `(-16.66, 1.5, 0)` |
| SingleRise | `jump-01-pre-01` | `(-10, 1.5, 0)` |
| SingleRise | `jump-01-air-01` | `(-6.72, 3.61, 0)` |
| SingleRise | `jump-01-air-02` | `(-4.94, 4.47, 0)` |
| SingleRise | `jump-01-air-03` | `(-3.16, 4.11, 0)` |
| SingleRise | `jump-01-land-01` | `(-1.38, 2.5, 0)` |
| SingleRise | `jump-02-pre-01` | `(6, 2.5, 0)` |
| SingleRise | `jump-02-air-01` | `(9.61, 4.86, 0)` |
| SingleRise | `jump-02-air-02` | `(11.72, 5.48, 0)` |
| SingleRise | `jump-02-air-03` | `(13.83, 4.36, 0)` |
| SingleRise | `jump-02-land-01` | `(15.94, 1.5, 0)` |
| SingleRise | `exit-pre-01` | `(17, 1.5, 0)` |
| SingleRise | `exit-air-01` | `(21, 3.36, 0)` |
| SingleRise | `exit-air-02` | `(23, 4.47, 0)` |
| SingleRise | `exit-air-03` | `(25, 4.01, 0)` |
| LegacySteps | `entry-land-01` | `(-16.66, 1.5, 0)` |
| LegacySteps | `jump-01-pre-01` | `(-14, 1.5, 0)` |
| LegacySteps | `jump-01-air-01` | `(-10.72, 3.61, 0)` |
| LegacySteps | `jump-01-air-02` | `(-8.94, 4.47, 0)` |
| LegacySteps | `jump-01-air-03` | `(-7.16, 4.11, 0)` |
| LegacySteps | `jump-01-land-01` | `(-5.38, 2.5, 0)` |
| LegacySteps | `jump-02-pre-01` | `(-4, 2.5, 0)` |
| LegacySteps | `jump-02-air-01` | `(-0.54, 4.75, 0)` |
| LegacySteps | `jump-02-air-02` | `(1.42, 5.5, 0)` |
| LegacySteps | `jump-02-air-03` | `(3.38, 4.75, 0)` |
| LegacySteps | `jump-02-land-01` | `(5.34, 2.5, 0)` |
| LegacySteps | `jump-03-pre-01` | `(6, 2.5, 0)` |
| LegacySteps | `jump-03-air-01` | `(9.61, 4.86, 0)` |
| LegacySteps | `jump-03-air-02` | `(11.72, 5.48, 0)` |
| LegacySteps | `jump-03-air-03` | `(13.83, 4.36, 0)` |
| LegacySteps | `jump-03-land-01` | `(15.94, 1.5, 0)` |
| LegacySteps | `exit-pre-01` | `(17, 1.5, 0)` |
| LegacySteps | `exit-air-01` | `(21, 3.36, 0)` |
| LegacySteps | `exit-air-02` | `(23, 4.47, 0)` |
| LegacySteps | `exit-air-03` | `(25, 4.01, 0)` |
| InternalGap | `entry-land-01` | `(-16.66, 1.5, 0)` |
| InternalGap | `jump-01-pre-01` | `(-5, 1.5, 0)` |
| InternalGap | `jump-01-air-01` | `(-1.54, 3.75, 0)` |
| InternalGap | `jump-01-air-02` | `(0.42, 4.5, 0)` |
| InternalGap | `jump-01-air-03` | `(2.38, 3.75, 0)` |
| InternalGap | `jump-01-land-01` | `(4.34, 1.5, 0)` |
| InternalGap | `exit-pre-01` | `(17, 1.5, 0)` |
| InternalGap | `exit-air-01` | `(21, 3.36, 0)` |
| InternalGap | `exit-air-02` | `(23, 4.47, 0)` |
| InternalGap | `exit-air-03` | `(25, 4.01, 0)` |

각 연결은 앞 Pattern의 `exit-pre-01`, `exit-air-01`~`03`과 뒤 Pattern의 `entry-land-01`로 5개 안내 묶음을 이룬다. 네 Pattern의 출구·입구가 공통 지형 높이와 44 Local X 간격을 사용하므로 16개 연결 조합에 같은 경계 배치를 적용한다. 첫 `Flat`의 `entry-land-01`은 평지의 진행 표시이며 정지 출발에 Jump를 요구하지 않는다. `Flat` 대체에서도 같은 경계 안내를 사용한다. 앞 Pattern이 재사용되기 전 Player Capsule이 출구를 완전히 지나야 한다는 기존 진행 조건을 유지한다.

배치 수치의 단일 계산 계약은 `InfiniteCollectibleLayout`이다. 생산 지형은 `InfinitePatternGeometry`, Jump 속도는 `PlayerMovementMath`, 통과 조건은 `InfinitePatternTraversalMath`를 사용한다. `InfiniteCollectibleLayoutTests`는 네 Pattern의 누락·중복·비정상 좌표, 내부 Jump 경로와 양 속도 통과 계약, 16개 연결의 안내 순서를 검사한다. 이 Test는 아직 Unity Test Runner에서 실행하지 않았다. Prefab이 저장된 후 생산 Prefab의 실제 자식·좌표·Collider와 계산 계약을 Play Mode Test로 대조한다.

## Step 3. 네 Pattern Prefab의 Collectible을 제작한다

### AI 작업

- Step 2의 확정 표를 `InfinitePattern_Flat.prefab`, `InfinitePattern_SingleRise.prefab`, `InfinitePattern_LegacySteps.prefab`, `InfinitePattern_InternalGap.prefab`별 Editor 작업표로 제공한다. 각 행에 부모 `CollectibleRoot`, 이름, ID, Local Position, SphereCollider 값, Trigger·Layer, `ScoreCollectible` 필드 할당 대상을 명시한다.
- 기존 Stage Collectible의 형태·색상·Player 판정 구성을 재사용할 수 있는지 확인하고, 달라야 하는 설정만 명시한다.

### Step 3 Editor 작업표와 순서 (2026-09-14)

Step 2의 50행 표가 네 Prefab별 개별 오브젝트 작업표이다. 각 행의 이름·ID·Local Position을 그대로 사용한다. 공통 필드는 표 바로 위의 배치 계약을 적용한다. `Flat` 5개, `SingleRise` 15개, `LegacySteps` 20개, `InternalGap` 10개를 만든다.

1. Unity Editor에서 `Assets/Prefabs/InfinitePatterns/InfinitePattern_Flat.prefab`을 Prefab Mode로 열고, `CollectibleRoot`의 자식으로 3D Sphere 오브젝트 하나를 만든다. 기본 Sphere의 `SphereCollider` 하나를 유지하고 다른 Collider는 추가하지 않는다.
2. 첫 Sphere의 Layer를 `Default`(0), Local Rotation을 `(0, 0, 0)`, Local Scale을 `(1, 1, 1)`, SphereCollider Center를 `(0, 0, 0)`, Radius를 `0.5`, Is Trigger를 On으로 설정한다. `ScoreCollectible`을 추가해 `_triggerCollider`에는 같은 SphereCollider, `_visual`에는 같은 MeshRenderer, `_playerLayers`에는 `Default`만 지정한다.
3. 첫 Sphere를 복제하여 필요한 개수만큼 만들고 각 행의 이름, `_collectibleId`, Local Position을 입력한다. 네 Prefab 각각에서 같은 순서로 작업한다. Prefab Mode에서 저장하고 닫는다. Stage Mode의 Scene 오브젝트는 수정하지 않는다.
4. Stage Collectible은 Default Layer·Sphere Mesh·반지름 `0.5` Trigger·`ScoreCollectible`의 Player Layer 마스크 `1` 구조를 재사용할 수 있다. 다만 생산 Scene의 Stage Renderer가 참조하는 Material GUID `31321ba15b8f8eb4c954353edc038b1d`에 대응하는 `.meta`는 현재 프로젝트에서 찾지 못했으므로 동일 Material 재사용은 확정할 수 없다. 새 Collectible의 구분되는 색상은 `Assets/Materials/InfiniteCollectibleYellow.mat` 공통 Material 하나를 Unity Editor에서 만들어 모든 Sphere에 할당한다. Shader는 `Universal Render Pipeline/Lit`, Base Map 색상은 불투명 노랑 `#FFD54F`로 설정한다.
5. 네 Prefab 저장 후 Unity Script Compilation 성공 여부와 예상하지 않은 Error·Warning만 AI에 전달한다. 각 ID·좌표·컴포넌트·GUID·Layer 및 Material 경로는 AI가 저장된 파일로 정적으로 대조한다. Test Runner의 물리 획득 검증은 Step 4에서 별도로 진행한다.

### 사용자 수동 작업

- Unity Editor에서 네 Prefab을 각각 열고 확정된 작업표대로 `CollectibleRoot` 하위에 Collectible 오브젝트를 만든다. 각 오브젝트에 Renderer, `SphereCollider`의 **Is Trigger**, `ScoreCollectible`을 설정하고 ID·Collider·Renderer·Player Layer 참조를 할당한 뒤 Prefab을 저장한다.
- 좌표·개수·반지름을 눈대중으로 조정하지 않는다. 연결 경계의 Collectible이 어느 Pattern 원본에 속하는지도 작업표에 따른다.
- 저장 후 Unity Script Compilation 성공과 예상하지 않은 Error·Warning 여부만 전달한다.

### 완료 조건

- [x] 네 Prefab이 확정된 작업표대로 저장됐다.
- [x] AI의 Prefab YAML·GUID·Layer·Component·ID·좌표 정적 검사와 Unity Script Compilation 결과가 확인됐다. 물리 Play Mode Test는 Step 4에서 확인한다.

### Step 3 저장 후 정적 검사 (2026-09-14)

- 사용자가 네 Prefab에 총 50개를 제작했다. AI가 저장된 YAML을 Step 2의 이름·ID·Local Position·`CollectibleRoot` 부모, 같은 오브젝트의 Collider·Renderer 참조, Player Layer 마스크, Mesh, Material GUID와 대조했다.
- 최초 저장본에서 `Flat` 5개는 Trigger였지만 `SingleRise` 15개, `LegacySteps` 20개, `InternalGap` 10개의 SphereCollider는 `Is Trigger: 0`이었다. AI가 Scene은 수정하지 않고 해당 세 Prefab의 SphereCollider 45개만 `Is Trigger: 1`로 보정했다. 보정 후 50개 모두 Trigger·반지름 `0.5`·Center `(0, 0, 0)`·활성 Collider 조건을 정적으로 재확인했다.
- `Assets/Materials/InfiniteCollectibleYellow.mat`과 `.meta`의 GUID는 50개 Renderer 참조와 일치한다. Material의 Base Color는 `#FFD54F`, Alpha는 `1`, Surface는 Opaque로 저장됐다. 사용자가 Scene의 Stage Collectible 10개 Renderer도 이 Material로 교체했으며, AI는 Scene을 편집하지 않았다.
- 사용자가 Unity Script Compilation 성공과 예상하지 않은 Error·Warning 없음으로 보고했다. 따라서 Step 3의 정적 검사·Compile 완료 조건을 충족했다. Unity Test Runner의 물리·획득 결과는 아직 확인하지 않았으며 Step 4에서 검증한다.

## Step 4. 획득·연결·Score 생명주기를 자동 Test로 검증한다

### AI 작업

- 빈 Root를 전제한 기존 Phase 2 Test를 현재 배치 계약에 맞게 바꾸고, 생산 Prefab을 그대로 로드하는 Test에서 네 Pattern 및 연결 경계의 Trigger·획득·재배치 후 Scope 복구를 확인한다.
- Collectible을 전부 놓쳐도 진행하는 경로, Pause·Resume·Result·Retry의 획득 상태와 Score 보존·초기화, 중복 획득·상한 포화는 Edit Mode와 Play Mode로 나눠 검증한다.
- Pattern 전환 전후 Distance Score, Collectible Score와 Total Score가 누적되고 Pattern 통과 횟수가 Score에 더해지지 않음을 생산 Score·Result 경로에서 확인한다.

### 사용자 수동 작업

- AI가 지정한 관련 Edit Mode 및 Play Mode Test를 Unity Test Runner에서 실행한다. 클래스별 Passed/Failed 수, 실패 Test명·메시지·Stack Trace와 예상하지 않은 Error·Warning을 전달한다.

### 완료 조건

- [x] 배치·물리 획득·Score·생명주기 관련 Test가 모두 통과한다.
- [x] 실패 또는 예상하지 않은 Error·Warning을 수정·재검증 전까지 완료로 처리하지 않는다.

### Step 4 AI 준비 결과 (2026-09-14)

- 빈 `CollectibleRoot`와 등록 수 0을 기대하던 `InfiniteCollectibleLayoutIntegrationTests`, `CollectibleLifecycleIntegrationTests`, `InfinitePatternConnectionIntegrationTests`를 Phase 4의 실제 생산 배치 수에 맞게 변경했다.
- `InfiniteCollectibleLayoutIntegrationTests`는 두 Slot의 네 생산 Prefab 인스턴스에서 이름·ID·Local Position·Trigger·Layer·Component 참조를 계산 계약과 대조한다. 첫 Flat의 물리 Trigger 획득과 중복 방지, 놓친 경우의 진행, 네 Pattern 재배치 뒤 새 Scope 획득, Pause·Resume·Retry 초기화, Pattern 통과 횟수와 Distance Score의 분리를 검사한다.
- `InfiniteModeIntegrationTests`에 실제 획득 후 Pattern 전환·Run 종료를 거쳐 Result Data의 Collectible Score와 Total Score 및 UI 문자열까지 도달하는 Test를 추가했다. 기존 `CollectibleRuntimeDataTests`, `ScoreRecordTests`, `InfiniteHudIntegrationTests`는 중복·포화·초기화·표시 계약을 계속 검증한다.
- Scene·Prefab은 이 Step에서 편집하지 않았다. 코드·문서의 정적 검사, 새 Test의 LINQ 부재와 `git diff --check`는 통과했다. Unity Script Compilation 및 Edit Mode·Play Mode Test Runner는 AI가 실행하지 않았으며 사용자 결과를 기다린다.

### Step 4 첫 Play Mode 결과 및 수정 (2026-09-14)

- 사용자가 새 Play Mode Test 다섯 개의 실패를 보고했다: `FourProductionPatterns_RebindAndAllowPickupAfterAdvance`, `InitialFlat_OverlappingPlayerCollectsOnceAndDisablesPresentation`, `PatternReuse_PreservesScoreAndRestoresPickupInNewScope`, `PauseResumeAndRetry_PreserveThenResetInfinitePickup`, `CollectedCoin_AfterPatternAdvance_ReachesResultScores`. 보고된 다섯 실패는 모두 Collectible 획득을 기대한 줄에서 `Expected: True, But was: False`였다. 전체 클래스별 Passed/Failed 수와 Edit Mode 결과는 아직 받지 못했다.
- 새 Test가 `Rigidbody.position`으로 Player를 이동시킨 직후 물리 갱신 전에 획득을 요구한 점을 확인했다. 기존 Stage 획득 Test와 같이 `WaitForFixedUpdate` 후 Trigger·겹침 결과를 판정하도록 수정했다. 수동 Pattern 요청 Test에서는 `InfiniteModeSystem`의 자동 FixedUpdate 선택과 충돌하지 않도록 해당 Component만 Test 실행 중 비활성화한다. Runtime·Scene·Prefab은 수정하지 않았다.
- 수정 직후에는 Script Compilation과 Test Runner 결과를 기다렸으며, 이후 전체 재검증 결과를 아래에 기록했다.

### Step 4 재검증 결과 (사용자 보고, 2026-09-14)

- Unity Script Compilation 성공. 예상하지 않은 Error·Warning 없음.
- 전체 Edit Mode Test `487`개 시도·`487`개 통과. 예상하지 않은 Error·Warning 없음.
- 전체 Play Mode Test `217`개 시도·`217`개 통과. 예상하지 않은 Error·Warning 없음.
- 첫 실행의 Play Mode 실패 5개는 테스트의 물리 갱신 시점 수정 후 전체 Play Mode 재검증에서 재현되지 않았다. Step 4의 배치·획득·Score·생명주기 검증 조건을 충족했다.
- 이 결과는 Step 4까지의 코드·Prefab·Scene 구성에 대한 검증이다. Step 5의 Difficulty 개발자 UI 연결과 그 후 변경에 대한 검증은 아직 수행하지 않았다. Build는 AI가 수행하지 않았다.

### Step 4 사용자 실행 범위

1. Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error·Warning 여부를 확인한다.
2. Test Runner의 Edit Mode에서 `InfiniteCollectibleLayoutTests`, `CollectibleRuntimeDataTests`, `ScoreRecordTests`를 실행한다.
3. Test Runner의 Play Mode에서 `InfiniteCollectibleLayoutIntegrationTests`, `InfinitePatternConnectionIntegrationTests`, `CollectibleLifecycleIntegrationTests`, `InfiniteModeIntegrationTests`, `InfiniteHudIntegrationTests`, `ScoreCollectibleTests`를 실행한다.
4. 클래스별 Passed/Failed 수와 예상하지 않은 Error·Warning 여부를 전달한다. 실패가 있으면 Test명·메시지·Stack Trace를 함께 전달한다. 빌드와 화면 수동 판정은 이 Step의 요청 범위가 아니다.

## Step 5. Difficulty 상태 UI를 생산 HUD에 연결한다

### Step 5 AI 구현 및 정적 확인 (2026-09-14)

- 기존 `DistanceText`, `ScoreText`, `InfiniteCollectibleScoreText`, `InfiniteTotalScoreText`는 각 점수 계약에 사용 중이므로 재사용하지 않는다. `InfiniteHUD/Canvas/Image`의 다섯 번째 자식으로 별도 TextMeshPro UI 텍스트 `InfiniteDifficultyText`가 필요하다.
- `InfiniteModeSystem`이 실제 진행 거리로 계산한 D1/D2/D3를 `InfiniteModeRuntimeData.CurrentDifficultyLevel`에 1/2/3으로 반영한다. Core 어셈블리는 Features의 enum을 참조하지 않는다. 새 Run과 Retry는 D1, 진행 중 D2·D3로 갱신된다. UI는 `Difficulty: D1`, `Difficulty: D2`, `Difficulty: D3` 형식을 사용하며 초기화 전에는 `Difficulty: --`를 사용한다. Pause·Result에서는 마지막 HUD 문자열을 유지한다.
- `UIManagementSystem._infiniteDifficultyText` 참조를 추가했다. `_showDifficultyInDevelopment`가 켜진 Unity Editor 또는 Development Build에서만 이 오브젝트를 활성화하며 일반 빌드에서는 비활성화한다. 기존 거리·점수와 Result Content 바인딩은 그대로 유지한다.
- 런타임 데이터의 난이도 진행·초기화·종료 계약을 Edit Mode Test에, HUD 갱신·Pause·Result·Retry·개발 표시 토글을 Play Mode Test에 추가했다. AI는 Unity Script Compilation, Test Runner, Build를 실행하지 않았다.

### Step 5 사용자 Scene 작업표

1. Unity Editor에서 `Assets/Scenes/SampleScene.unity`를 열고 Hierarchy의 `InfiniteHUD/Canvas/Image`를 찾는다. 이 `Image`는 기존 거리·세 점수 텍스트의 부모이며 `VerticalLayoutGroup`을 사용한다.
2. `Image` 하위의 `InfiniteTotalScoreText`를 복제하여 마지막 다섯 번째 자식으로 놓고 이름을 `InfiniteDifficultyText`로 바꾼다. `TextMeshPro - Text (UI)` 컴포넌트를 유지하고 초기 문자열을 `Difficulty: --`로 설정한다. 기존 텍스트처럼 글꼴 크기 30, RectTransform 크기 400×50, 좌상단 기준 배치를 사용한다.
3. 부모 `Image`의 RectTransform 높이를 200에서 250으로 늘린다. 기존 우상단 앵커·피벗과 폭 400, `VerticalLayoutGroup`의 순서는 유지하여 다섯 행을 위에서 아래로 배치한다. 에디터 Game View에서 다섯 번째 행이 잘리지 않는지만 확인한다.
4. `UIManagementSystem` Inspector의 `Infinite Difficulty Text` 필드에 새 `InfiniteDifficultyText`의 `TextMeshProUGUI` 컴포넌트를 할당한다. `Show Difficulty In Development`는 켠 채로 Scene을 저장한다. 플레이어용 일반 빌드에서는 코드가 이 행을 숨긴다.
5. 저장 후 Unity Script Compilation 결과와 `InfiniteModeRuntimeDataTests`, `InfiniteHudIntegrationTests`, `ModeUISceneConfigurationTests`의 Edit/Play Mode 결과 및 예상하지 않은 Error·Warning 여부를 전달한다. D2/D3 경계를 직접 달리며 판정할 필요는 없다.

### Step 5 완료 대기 항목

- 최초 수정본은 `Core`에서 `Features.E_InfinitePatternDifficulty`를 참조하여 사용자 Unity Script Compilation에서 CS0234/CS0246이 발생했다. `Features`가 `Core`를 참조하는 어셈블리 구조를 확인하고 Core에는 정수 Level만 저장하도록 수정했다. 수정본의 Unity Script Compilation·Test 결과는 아래에 기록했다.
- Scene의 새 TMP 참조와 부모 높이 변경은 YAML·GUID로 정적 확인했다.

### Step 5 최종 검증 결과 (사용자 보고 및 AI 정적 확인, 2026-09-14)

- Unity Script Compilation 성공. 예상하지 않은 Error·Warning 없음.
- 전체 Edit Mode Test `490`개 시도·`490`개 통과. 예상하지 않은 Error·Warning 없음.
- 전체 Play Mode Test `219`개 시도·`219`개 통과. 예상하지 않은 Error·Warning 없음.
- Scene YAML에서 `InfiniteHUD/Canvas/Image`의 다섯 번째 자식 `InfiniteDifficultyText`를 확인했다. 해당 GameObject는 `TextMeshProUGUI` 컴포넌트(fileID `1316098895`), 초기 문구 `Difficulty: --`, 글꼴 크기 30, 크기 400×50을 가진다. 부모 `Image` 높이는 250이고 `UIManagementSystem._infiniteDifficultyText`가 이 TMP 컴포넌트를 참조하며 개발 표시 플래그는 켜져 있다.
- Step 5의 생산 UI 연결과 자동 상태 전환 검증 조건을 충족했다. 실제 Game View 가독성 확인은 Step 6에서 수행한다. AI는 Build 또는 Test Runner를 실행하지 않았다.

### AI 작업

- 현재 `InfiniteHUD`의 기존 Distance·Score 텍스트를 재사용할 수 있는지, Difficulty 표시를 위한 TMP 텍스트가 별도로 필요한지 정적 검사로 확정한다. 표시 형식과 D1/D2/D3 갱신·Pause·Result·Retry 계약을 문서화한다.
- 필요한 Runtime UI 바인딩과 문자열 생성·상태 Unit Test를 작성한다. 기존 Distance·Collectible·Total Score 표시와 Result Content는 유지한다.
- 새 UI 오브젝트나 직렬화 참조가 실제로 필요할 때만 정확한 Scene 경로, TMP 컴포넌트, 표시 문자열, RectTransform·Style 기준과 `UIManagementSystem` 할당 필드를 작업표로 제공한다.

### 사용자 수동 작업

- AI 작업표에서 새 UI 오브젝트가 필요하다고 확정한 경우에만 Unity Editor의 `SampleScene.unity`에서 지정한 `InfiniteHUD` 하위 TMP 텍스트를 만들고, 지정한 `UIManagementSystem` 필드에 할당해 Scene을 저장한다. 기존 오브젝트 재사용으로 해결되면 이 편집은 건너뛴다.
- 저장 후 Unity Script Compilation과 AI가 지정한 UI Play Mode Test 결과를 전달한다. D2/D3 거리 경계를 손으로 달리며 판정하지 않는다.

### 완료 조건

- [x] 생산 UI에 현재 Difficulty 또는 확정된 진행 상태가 표시되고 자동 Test로 상태 전환이 검증됐다.
- [x] 필요한 직렬화 참조가 YAML·GUID·Play Mode Test에서 확인됐다.

## Step 6. 전체 회귀와 최소 화면 확인을 수행한다

### Step 6 AI 정적 회귀 및 자동 검증 확인 (2026-09-14)

- 네 생산 Pattern Prefab의 코인·Trigger·노란 Material 참조 수는 `Flat 5/5/5`, `SingleRise 15/15/15`, `LegacySteps 20/20/20`, `InternalGap 10/10/10`으로 Step 2 배치표와 일치한다. Scene의 다섯 번째 Difficulty TMP·UI 참조·250 높이도 유지된다.
- 변경된 Runtime·Test에서 LINQ 사용은 확인되지 않았다. `git diff --check`는 Scene을 제외한 변경 파일에서 통과했다. Scene의 새 TMP 컴포넌트에 Unity YAML 관례의 빈 `m_Name: ` 행 하나가 새로 생성되어 `git diff --check`에서 trailing whitespace로 검출된다(`SampleScene.unity`의 `InfiniteDifficultyText` TMP 블록). 기능이나 참조 문제는 아니며 AI는 Scene을 편집하지 않았다.
- ProjectSettings의 의미 있는 변경은 확인되지 않았다. `URPProjectSettings.asset`은 작업 트리에서 수정으로 표시되지만 `git diff --ignore-space-at-eol` 결과는 비어 있다.
- Step 5 Scene 저장 이후 사용자가 보고한 Unity Script Compilation 성공, 전체 Edit Mode `490/490`, 전체 Play Mode `219/219` 통과, 예상하지 않은 Error·Warning 없음은 Step 6의 전체 자동 회귀 결과로 재사용한다. Scene 저장 뒤 코드 변경은 없으며, 같은 전체 Test를 다시 실행할 필요가 없다. Stage Mode와 InfiniteMode 회귀는 이 전체 결과에 포함된다. AI는 Unity Test Runner 또는 Build를 실행하지 않았다.

### Step 6 결정적 최소 화면 확인 경로

1. Unity Editor에서 `SampleScene.unity`를 연 뒤 Test Runner의 Play Mode에서 `InfiniteModeIntegrationTests.ControlledSeed_ProductionSelection_ActivatesDifferentPattern` 하나만 실행하고 Game View를 본다. 이 Test는 선택 Seed를 고정해 `SingleRise`를 활성화하고, 연결 지형을 볼 수 있는 위치에서 Player를 5초간 고정한다. 무작위 Pattern 출현을 기다릴 필요가 없다.
2. 고정된 5초 동안 노란 Collectible 안내가 지형·Jump 경로를 따라 끊기거나 겹쳐 보이지 않는지, 상승 Jump 유도가 읽히는지, 우상단 `Difficulty: D1` 행과 기존 점수 행이 서로 겹치지 않는지, Camera와 Pattern 전환 표현이 거슬리지 않는지만 관찰한다. 수치·좌표·Score·Difficulty 경계는 기존 자동 Test와 정적 검사로 검증했으므로 눈으로 판정하지 않는다.
3. 관찰 결과를 항목별 정상/이상과 함께 알려준다. Test가 실패하면 Test명·메시지·Stack Trace도 전달한다. 이 화면 관찰은 Step 6 완료 판단에 필요하다.

### Step 6 빌드 상태

- AI는 Build를 수행하지 않았다. 사용자가 Unity Editor에서 직접 시도하는 Build 결과는 전달받은 경우에만 별도로 기록한다. Phase 4의 자동 검증 통과가 Build 통과를 의미하지 않는다.

### Step 6 화면 확인 결과 (사용자 보고, 2026-09-14)

- 사용자가 지정한 화면 확인에서 정상적으로 보임을 확인했다. Collectible·Jump 안내, Difficulty UI 가독성, Camera·Pattern 전환 표현의 최소 시각 확인을 완료했다. 수치·경계·점수는 화면으로 판정하지 않았다.
- 정적 검사와 사용자 Unity Script Compilation·전체 Test 결과는 위에 기록했다. Step 6 검증 조건을 충족했으며, Build는 AI 미실행·사용자 결과 미보고로 남긴다.

### AI 작업

- 변경 파일·문서·코드·Prefab·Scene의 일관성, LINQ 부재, `git diff --check`와 무관한 설정 변경 부재를 검사한다.
- 관련 Test 통과 후 전체 Edit Mode와 Play Mode Test를 각각 한 번 요청한다. Stage Mode의 자동 이동·Collectible·Clear Time·Score와 InfiniteMode의 Pattern 진행·Score·UI·Pause·Result·Retry를 포함한다.
- 결정적 Pattern·Collectible·Difficulty 화면 확인 경로를 Test나 재현 가능한 시작 설정으로 제공한다. 무작위 플레이로 원하는 Pattern 출현을 기다리게 하지 않는다.

### 사용자 수동 작업

- Unity Editor에서 Script Compilation과 전체 Edit Mode·Play Mode Test를 실행하고 결과 및 예상하지 않은 Error·Warning 여부를 전달한다.
- 자동 Test 통과 후 AI가 지정한 결정적 화면 경로를 한 번 실행해 Collectible 안내의 눈에 띄는 끊김·겹침, Jump 유도 가독성, Difficulty UI 가독성, Camera·전환 표현만 관찰한다. 개수·좌표·점수·Difficulty 경계는 화면으로 판정하지 않는다.
- Phase 4에서는 Build를 수행하지 않는다. 검증 결과에서 Build를 범위 제외·미검증으로 기록한다.

### 완료 조건

- [x] 정적 검사, Compile, 관련·전체 Test 및 화면 관찰 결과가 기록됐다.
- [x] Stage Mode 회귀와 Phase 4 전체 흐름의 자동 검증이 통과했다.
- [x] Build 제외·미검증 상태가 Phase 4 결과에 기록됐다.

## Step 7. 결과와 Phase 경계를 기록한다

### Step 7 결과 (2026-09-14)

- `20260914_04_Phase4VerificationResult.md`에 구현, 정적 검사, Unity Script Compilation, Edit Mode `490/490`, Play Mode `219/219`, 최소 화면 확인 정상, Scene YAML trailing whitespace 예외와 Build 미검증을 구분해 기록했다.
- `InfiniteMode.md`, `ScoreCollectible.md`, `ScoreRecord.md`, `UIManagementSystem.md`의 현재 계약을 생산 Collectible 50개, Score·Difficulty HUD 구현에 맞췄다. `IMPLEMENTATION_ROADMAP_004.md`의 Phase 4를 Build 제외 범위에서 완료로 갱신했다.
- AI는 Unity Build, Unity Test Runner 또는 Scene 편집을 수행하지 않았다. 사용자가 Unity Editor에서 직접 수행할 Build 결과는 아직 받지 못했다. Prototype 4 전체 완료 기준에는 Build 통과가 포함되므로 전체 완료로 기록하지 않는다.
- Step 7 자체에 필요한 추가 Unity Editor 수동 작업은 없다.

### AI 작업

- Phase 4 검증 결과 Task 문서에 구현, Test 수, 화면 관찰, Build 제외·미검증 상태와 미해결 사항을 구분해 기록한다.
- `InfiniteMode.md`, `ScoreCollectible.md`, `ScoreRecord.md`, `UIManagementSystem.md` 및 Roadmap을 실제 구현에 맞춰 갱신한다.
- Phase 4의 Build 제외 범위에서 완료 조건을 모두 확인했을 때만 Phase 4 상태를 완료로 바꾼다. Prototype 4 전체 완료 여부는 이 단계에서 판정하지 않는다.

### 사용자 수동 작업

- 없음. 새 실패가 발견되면 해당 Test 또는 화면 확인 결과를 제공한다.

### 완료 조건

- [x] 확인하지 않은 기능이나 Build를 통과로 기록하지 않는다.
- [x] 검증 결과와 Roadmap 상태가 일치한다.

---

# 수동 작업 요약

- 확정된 배치표를 받은 뒤 네 Pattern Prefab의 `CollectibleRoot` 하위 Collectible 제작·저장
- 새 Difficulty UI 오브젝트·참조가 필요하다고 확인된 경우에만 `SampleScene.unity`에서 지정 항목 편집·저장
- AI가 지정한 관련 Test, 최종 전체 Edit Mode·Play Mode Test 및 Script Compilation 실행 결과 전달
- 자동 Test 통과 후 결정적 화면 경로에서 안내성·가독성과 시각적 전환만 확인

수치 판정, Inspector 값 대조, 정확한 Jump·Boundary 타이밍, 특정 거리 도달, Pattern 조합과 Score 계산은 수동 체크리스트에 포함하지 않는다.

# 영향 범위

- Feature: `InfiniteMode`, `ScoreCollectible`, `ScoreRecord`
- System: `UIManagementSystem`, 기존 Pattern Scope와 Runtime Data 연동
- Asset: 네 Infinite Pattern Prefab, 필요한 경우 생산 Scene의 InfiniteHUD
- Task: Phase 4 구현·검증 결과와 사용자 수동 작업

# 검증 내용

- Step 1–7의 구현 및 검증 결과를 각 Step과 별도 Phase 4 Verification Result 문서에 기록했다.
- AI는 Unity Build 또는 Test Runner를 실행하지 않았다. Unity 검증 수치는 사용자 보고 결과다.

# 검증 결과

- Step 6까지 Unity Script Compilation 성공, Edit Mode 490/490, Play Mode 219/219 통과, 최소 화면 확인 정상. 예상하지 않은 Error·Warning 없음(사용자 보고).
- Phase 4를 완료한 뒤 사용자가 Unity Editor Build 성공을 보고했다. AI는 Build를 실행하지 않았다. Build의 Error·Warning 세부 내역은 보고받지 않았다. Prototype 4 전체 완료 기준을 충족했다.

# 후속 작업

- Build의 Error·Warning 세부 내역이 전달되면 검증 기록에 추가한다.

# 관련 문서

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_004.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreCollectible.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/90_Tasks/Prototype_4/20260914_02_Phase3VerificationResult.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/90_Tasks/Prototype_4/20260914_04_Phase4VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
