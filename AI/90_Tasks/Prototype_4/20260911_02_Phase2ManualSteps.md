# 작업 정보

## 작업명

Prototype 4 Phase 2 수동 작업 및 검증 계획

## 작업 일자

20260911

## 작업 담당자

AI, 사용자

## 작업 상태

대기

---

# 작업 목적

Phase 1에서 확정한 `Flat`, `SingleRise`, `LegacySteps`, `InternalGap` 네 Map Pattern을 생산 구성에 구현하고 두 재사용 Slot에서 안전하게 연결한다.

정적 검사와 Unit Test로 판정할 수 있는 계약은 Scene 수동 확인에서 제외하고, Unity Editor 수동 작업은 직렬화된 Scene 또는 Prefab 구성과 자동 판정이 어려운 최종 화면 확인으로 제한한다.

---

# 작업 범위

- 네 Pattern의 생산 지형과 공통 Authoring 구조
- Pattern 원본과 두 재사용 Slot의 생성·교체·정리
- StartAnchor, EndAnchor와 AdvanceBoundary 연결
- Ground 및 Platform Collider와 Layer
- Pattern 내부 및 Pattern 경계 통과
- Pattern별 자동 이동, Jump, 착지, Wall 접촉 해제와 낙하 방지
- 네 Pattern의 전체 16개 연결 조합
- 기존 Camera, Collision, Collectible Scope, Pause, Result와 Retry 회귀
- 관련 System, Feature, Roadmap 및 Task 문서

---

# 작업 전 상태

- Phase 1에서 Pattern ID, 공통 길이, 연결 Gap, 통과 계산, Difficulty와 선택 계약을 확정했다.
- 생산 Scene의 `InfiniteMapPattern`은 `Pattern_0`과 `Pattern_1` 두 인스턴스를 번갈아 재배치한다.
- 각 기존 Pattern에는 StartAnchor, EndAnchor, AdvanceBoundary, Ground, 두 Platform과 Collectible Scope가 있다.
- 현재 `InfiniteMapPattern`은 두 Transform과 Anchor 및 Boundary만 직접 참조하며 네 Pattern 원본을 선택해 Slot 지형을 교체하는 구조는 없다.
- 생산 Scene에는 별도 Pattern Prefab이 없다.
- Phase 2에서는 Difficulty 기반 무작위 선택과 Run별 선택 상태를 `InfiniteModeSystem`에 연결하지 않는다. 해당 책임은 Phase 3이다.
- Phase 2에서는 Pattern별 Collectible 안내 경로와 UI를 확정하지 않는다. 해당 책임은 Phase 4이다.

---

# 작업 원칙

- AI는 Unity Editor 빌드와 Unity Test Runner를 실행하지 않는다.
- AI는 Scene, Prefab과 Inspector 값을 직접 변경하지 않는다.
- 사용자는 AI가 정적 검사와 Unit Test로 값과 구조를 확정한 뒤 제공하는 정확한 절차에 따라서만 Scene 또는 Prefab을 편집한다.
- Scene YAML, `.meta`, GUID, Serialized Reference, Transform, Collider, Layer와 Pattern 수치는 가능한 범위에서 AI가 정적으로 검사한다.
- 순수 계산, 유효성, 경계값, 상태 전환, 생성·정리 요청, 중복 요청과 초기화는 Edit Mode Unit Test로 검증한다.
- Rigidbody, FixedUpdate, Collision, Trigger, 실제 착지, Wall 접촉 해제, Camera와 생산 Scene 연동만 Play Mode Test로 검증한다.
- Test가 생산 계약 객체를 호출하게 하며 Test 안에 같은 기하 계산이나 별도 Pattern 선택 알고리즘을 복제하지 않는다.
- 빠른 입력, 정확한 Jump 시점과 같은 사람이 안정적으로 재현하기 어려운 조건을 수동 작업으로 요구하지 않는다.
- 모든 관련 Test가 통과한 뒤에만 전체 Edit Mode와 Play Mode Test를 각각 한 번 실행한다.
- Scene 작업 전후로 의도하지 않은 Package, Input Action, ProjectSettings와 다른 Scene 변경이 없는지 확인한다.

---

# 수행 Step

## Step 1. Pattern 원본과 재사용 Slot의 Authoring 구조를 결정한다

- 진행 상태: **완료**

### AI 작업

- 현재 `InfiniteMapPattern`, 생산 Scene 계층, Collectible Scope, Boundary와 Retry 초기화 흐름을 조사한다.
- 네 Pattern 원본을 Scene 내부 비활성 Template, Prefab 또는 직렬화 데이터 기반으로 관리하는 선택지를 제시한다.
- 각 선택지에 대해 Serialized Reference 안정성, 두 Slot 교체 비용, Test 구성, Scene 복잡도, Phase 3 선택 연동과 Phase 4 Collectible 확장 영향을 설명한다.
- 두 재사용 Slot에서 Pattern ID를 교체할 때 Transform, Collider, Boundary와 Collectible 상태가 남지 않는 구조를 권장안으로 제시한다.
- Pattern 원본 보관 위치, Slot 구성 단위, Runtime 생성 방식과 Destroy 또는 Pooling 범위를 사용자 결정 항목으로 분리한다.

### 사용자 작업

- AI가 제시한 선택지 중 사용할 Authoring 구조를 결정한다.
- 화면 비교가 필요한 선택지만 남은 경우에만 AI가 지정한 최소 Blockout을 별도 Scene 사본에서 비교한다.

### AI 조사 결과

- 생산 Scene은 `InfiniteMapPattern` 아래 `Pattern_0`, `Pattern_1` 두 재사용 Slot을 직접 참조하고 각 Slot을 이동하여 번갈아 재사용한다.
- 현재 각 Slot은 Geometry, StartAnchor, EndAnchor, AdvanceBoundary와 Collectible을 자식으로 가지며 Pattern 종류를 교체하는 구조는 없다.
- `AdvanceBoundary`는 Slot ID와 `InfiniteMapPattern` 참조를 가지며, Pattern 진행 시 사용이 끝난 Slot의 Boundary 상태를 초기화한다.
- Collectible은 Slot 재사용 전에 기존 Scope에서 해제되고 이동 후 새로운 Scope에 등록된다. Scope는 같은 Pattern과 Collectible ID의 반복 또는 동시 출현을 구분하고 종료된 Pattern의 늦은 획득 요청을 거부하며 등록 상태를 묶어서 정리한다.
- Retry는 두 Slot의 최초 Transform, Boundary와 Collectible 연결 상태를 초기화한다.
- 생산 Scene 외 별도 Pattern Prefab은 없으며, Step 1 조사에서는 Scene, Prefab, Runtime과 Test를 변경하지 않았다.
- 선택지 비교 결과 별도 화면 Blockout 비교는 필요하지 않다고 판단했다.

### 사용자 결정

1. `Flat`, `SingleRise`, `LegacySteps`, `InternalGap` 원본은 Pattern별 독립 Prefab 네 개로 관리한다.
2. Pattern Prefab은 Geometry Root, StartAnchor, EndAnchor, `AdvanceBoundaryPoint`와 Phase 4 확장을 위한 Collectible Root를 소유한다. 실제 `AdvanceBoundary` Trigger Component, Slot ID와 진행 상태는 Slot이 소유한다. Pattern이 교체될 때 Trigger의 위치와 방향을 해당 Pattern Prefab의 `AdvanceBoundaryPoint`에 정렬하며 Boundary 위치를 Pattern 공통 고정값으로 제한하지 않는다.
3. 두 재사용 Slot은 각각 독립적인 Slot Component로 구성하고 `InfiniteMapPattern`이 두 Slot의 진행을 조정한다.
4. 각 Slot은 네 Pattern 인스턴스를 초기화 시 한 번 생성하여 캐시하고 요청된 Pattern 하나만 활성화한다. Run 중 Pattern 전환 경로에서는 Instantiate와 Destroy를 반복하지 않는다.
5. Pattern 교체와 Retry는 동일한 명시적 Reset 경로를 사용한다. 기존 Collectible Scope 해제, 기존 Pattern 비활성화와 상태 초기화, 요청 Pattern 초기화와 활성화, Anchor 정렬, Boundary 위치·상태 초기화, 새 Collectible Scope 연결과 현재 Pattern ID 갱신 순서를 하나의 교체 계약으로 관리한다.
6. Phase 2는 Collectible Root와 기존 Collectible Scope의 등록·해제·재연결 및 Reset 생명주기를 유지한다. Pattern별 최종 Collectible 배치, 안내 경로와 점수 규칙은 Phase 4 책임으로 남긴다.
7. Phase 2 진행 구조는 요청 ID와 명시적 Pattern ID를 받는 API만 제공한다. Difficulty, 무작위 선택, 반복 제한과 대체 후보 결정은 Phase 3 책임으로 분리한다.

- Pattern Prefab 저장 경로와 정확한 Root 이름은 Step 5 생산 Scene 작업 명세에서 확정한다.
- Slot Component와 Pattern Authoring 계약의 정확한 Class, Field와 API 이름은 Step 2 구현에서 프로젝트 Naming Rule에 따라 확정한다.
- 캐시된 Pattern 인스턴스는 InfiniteMode Root의 생명주기를 따르며 Run 중 개별 Destroy하지 않는다.

### 완료 조건

- [x] Pattern 원본 저장 위치와 형식이 확정되었다.
- [x] 두 재사용 Slot의 생성·교체·정리 방식이 확정되었다.
- [x] Phase 3 선택 연동과 Phase 4 Collectible 책임이 Phase 2 구현에서 분리되었다.

## Step 2. Pattern Authoring과 Slot 계약을 코드 및 Unit Test로 확정한다

- 진행 상태: **완료**

### AI 작업

- Step 1 결정에 따라 Pattern ID, Geometry Root, StartAnchor, EndAnchor, AdvanceBoundary와 지형 Collider를 표현하는 최소 계약을 구현한다.
- 누락 및 중복 ID, 잘못된 Difficulty, 잘못된 Anchor, Boundary, Collider, Layer와 Trigger 설정을 실행 전에 거부한다.
- 두 Slot이 서로 다른 인스턴스를 소유하고 현재 및 후행 Pattern ID를 명확히 노출하도록 한다.
- 초기화 실패 시 부분 상태가 남지 않고 재초기화와 Retry가 동일한 시작 상태를 만들도록 한다.
- 유효, 누락, 중복, 잘못된 참조, 중복 초기화와 초기화 실패 복구를 Edit Mode Unit Test로 검증한다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Edit Mode Test만 실행한다.

### AI 수행 결과

- `InfinitePatternAuthoring`을 추가하여 Pattern ID, 최초 Difficulty, Geometry Root, StartAnchor, EndAnchor, `AdvanceBoundaryPoint`, Collectible Root와 지형 Collider 목록을 표현했다.
- Pattern ID가 Phase 1 Catalog에 없거나 최초 Difficulty가 Catalog와 다르면 Authoring 구성을 거부하도록 했다.
- 필수 Transform 누락, Pattern Root 외부 참조, 동일 Transform의 잘못된 중복 사용과 NaN 또는 Infinity 위치·회전을 거부하도록 했다. Anchor 길이, 접선과 Pattern별 기하 수치는 Step 3에서 확정한다.
- 지형 Collider 누락·중복, Geometry Root 외부 Collider, Trigger Collider와 `Ground`가 아닌 Layer를 거부하도록 했다.
- `InfinitePatternSlot`을 추가하여 Slot ID, Content Root, Slot이 소유하는 실제 AdvanceBoundary와 현재 Pattern ID를 표현했다.
- Slot은 Phase 1 Catalog의 네 Pattern 캐시 인스턴스를 정확히 한 개씩 소유해야 하며 누락·중복 ID, 잘못된 Authoring, Slot 외부 인스턴스와 Boundary ID·Trigger 구성을 거부하도록 했다.
- Slot 초기화는 전체 입력을 먼저 검사한 후 상태를 반영하여 실패 시 부분 캐시와 활성 상태가 남지 않도록 했다. 중복 초기화 요청은 기존 시작 상태를 보존하며, 실패 원인을 수정한 뒤 다시 초기화할 수 있다.
- Retry에서 재사용할 `ResetToInitialPattern()` 계약을 추가하여 최초 Pattern 활성 상태, 현재 Pattern ID와 Boundary 처리 상태를 동일하게 복구하도록 했다.
- 두 Slot이 같은 Slot ID 또는 같은 Pattern 인스턴스를 공유하지 않는지 `CanPairWith()`로 검사할 수 있게 했다.
- Pattern 생성·교체, Anchor 및 Boundary Point 정렬, Collectible Scope 재연결과 명시적 Pattern 요청 처리는 Step 4 책임으로 남겼다.
- 신규 Edit Mode Test는 유효 구성, 필수 참조, ID·Difficulty, Anchor, Collider·Layer·Trigger, Catalog 완전성, 중복 ID, Boundary ID, 중복 초기화, 실패 복구, Retry 시작 상태와 두 Slot 인스턴스 분리를 검증한다.
- 첫 Test 실행에서 `IsValid_NonFiniteAnchor_IsRejected`가 실패했다. Unity가 NaN `Transform.localPosition` 할당을 거부해 기존 유효 위치를 유지했으므로, Test가 비정상 Anchor를 실제로 구성하지 못한 것이 원인이다. 이 Test를 Pattern Root 외부 Anchor 참조 거부 검증으로 교체했고 재검증에서 통과했다.
- Unity Test Runner와 Build는 실행하지 않았고 Scene 및 Prefab은 변경하지 않았다.

### Scene 및 Prefab 작성 시 입력할 Field 목록

- Pattern Prefab의 `InfinitePatternAuthoring`: Pattern Id, Minimum Difficulty, Geometry Root, Start Anchor, End Anchor, Advance Boundary Point, Collectible Root, Terrain Colliders
- 재사용 Slot의 `InfinitePatternSlot`: Slot Id, Content Root, Advance Boundary
- `AdvanceBoundary`는 Slot의 자식이면서 Content Root 밖에 두고, 같은 Slot ID를 입력하며 활성화된 단일 Trigger Collider를 사용한다.
- 실제 Hierarchy 경로와 Transform·Collider 값은 Step 3 계산 및 Step 5 명세 전에는 Scene이나 Prefab에 입력하지 않는다.

### 사용자 검증 결과

- Unity Script Compilation 성공을 확인했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없음을 확인했다.
- 전체 Edit Mode Test `443`개를 실행하여 모두 성공함을 확인했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없음을 확인했다.

### 완료 조건

- [x] Scene 작성 전에 필요한 Pattern Authoring과 Slot 계약이 코드로 고정되었다.
- [x] 잘못된 구성과 초기화 경계가 Unit Test로 검증된다.
- [x] Scene에서 사용자가 입력해야 할 Field와 참조 목록이 확정되었다.

## Step 3. 네 Pattern의 기하 수치를 계산하고 Unit Test로 확정한다

- 진행 상태: **완료**

### AI 작업

- 모든 Pattern의 StartAnchor와 EndAnchor 간 X 길이 `44`, 같은 Y·Z, +X 접선과 연결 Ground 조건을 적용한다.
- `Flat`의 평탄 Ground, `SingleRise`의 한 번의 상승과 넓은 착지, `LegacySteps`의 기존 두 Platform, `InternalGap`의 같은 높이 내부 Gap 수치를 계산한다.
- 기본 속도 `8`과 최대 속도 `14`, Jump 높이 `3`, 중력 `25`, Capsule 반지름 `0.5`, 높이 `2`, 최소 Jump 입력 Window `0.10`초를 사용한다.
- Pattern 내부의 모든 이륙 Ground, Gap, 상승·하강, 착지면과 수직 장애물을 `InfinitePatternTraversalMath`로 검사한다.
- 최소·최대 경계와 경계 밖 값을 Edit Mode Unit Test로 검증하고 Test에서 계산식을 복제하지 않는다.
- 확정된 Pattern별 Transform, Collider 크기와 위치를 문서 표로 작성한다.

### 사용자 작업

- Scene 또는 Prefab 작업은 없다.
- AI가 Unity Test Runner를 실행하지 않으므로, 아래 관련 Edit Mode Test와 Unity Script Compilation 결과만 확인한다.

### 확정 좌표 계약

- 아래 값은 모두 Pattern Prefab Root 기준 Local 값이다. Pattern Root, Geometry Root, Collectible Root의 Local Position과 Rotation은 `(0, 0, 0)`, Local Scale은 `(1, 1, 1)`이다.
- 모든 Pattern의 StartAnchor는 `(-22, 0, 0)`, EndAnchor는 `(22, 0, 0)`이며 Rotation은 `(0, 0, 0)`, Scale은 `(1, 1, 1)`이다. 따라서 Anchor 간 길이는 `44`, 같은 Y·Z와 +X 접선을 유지한다.
- 아래 지형 오브젝트는 Geometry Root의 자식이며 Rotation `(0, 0, 0)`, Scale `(1, 1, 1)`, Layer `Ground`이다. BoxCollider의 Center는 모두 `(0, 0, 0)`, Is Trigger는 `false`이고 아래 Size를 사용한다. 이 수치는 GameObject Scale이 아니라 Collider Size에 입력한다.
- 기본 Ground 상단 Y는 `0.5`, 상승 Platform 상단 Y는 `1.5`이다. 모든 지형의 Z 폭은 `4`, Collider 높이는 `1`이다.

| Pattern | 지형 순서와 이름 | Local Position | BoxCollider Size | 지형 X 범위 |
|---|---|---|---|---|
| `Flat` | `Ground_0` | `(0, 0, 0)` | `(40, 1, 4)` | `-20`~`20` |
| `SingleRise` | `Ground_0` | `(-14, 0, 0)` | `(12, 1, 4)` | `-20`~`-8` |
| `SingleRise` | `Platform_0` | `(1, 1, 0)` | `(14, 1, 4)` | `-6`~`8` |
| `SingleRise` | `Ground_1` | `(15, 0, 0)` | `(10, 1, 4)` | `10`~`20` |
| `LegacySteps` | `Ground_0` | `(-16, 0, 0)` | `(8, 1, 4)` | `-20`~`-12` |
| `LegacySteps` | `Platform_0` | `(-6, 1, 0)` | `(8, 1, 4)` | `-10`~`-2` |
| `LegacySteps` | `Platform_1` | `(4, 1, 0)` | `(8, 1, 4)` | `0`~`8` |
| `LegacySteps` | `Ground_1` | `(15, 0, 0)` | `(10, 1, 4)` | `10`~`20` |
| `InternalGap` | `Ground_0` | `(-11.5, 0, 0)` | `(17, 1, 4)` | `-20`~`-3` |
| `InternalGap` | `Ground_1` | `(11.5, 0, 0)` | `(17, 1, 4)` | `3`~`20` |

| Pattern | `AdvanceBoundaryPoint` Local Position | Rotation | Scale |
|---|---|---|---|
| `Flat` | `(-4, 5.5, 0)` | `(0, 0, 0)` | `(1, 1, 1)` |
| `SingleRise` | `(0, 5.5, 0)` | `(0, 0, 0)` | `(1, 1, 1)` |
| `LegacySteps` | `(2, 5.5, 0)` | `(0, 0, 0)` | `(1, 1, 1)` |
| `InternalGap` | `(4, 5.5, 0)` | `(0, 0, 0)` | `(1, 1, 1)` |

- Boundary Point는 Pattern별 위치 정보만 담는다. 실제 Trigger Collider는 Slot이 소유하며 Pattern 교체 시 Point에 정렬한다. Trigger BoxCollider Size는 기존 생산 Scene과 동일한 `(1, 10, 4)`, Center `(0, 0, 0)`, Is Trigger `true`로 유지한다.
- 첫·마지막 Ground는 각각 Anchor에서 안쪽으로 `2` 들어간 `-20`과 `20`에서 끝나므로 모든 연결 조합의 경계 Ground Gap은 `4`이고 Ground 상단 Y는 동일하다.

### 기하 계산 및 정적 검증

- `InfinitePatternGeometry`에 위 지형 Transform과 Collider Size, Pattern별 Boundary Point 위치 및 내부 통과 판정을 단일 생산 계약으로 추가했다. Test는 해당 API에서 수치를 읽고 `InfinitePatternTraversalMath`를 호출하며 계산식을 복제하지 않는다.
- `SingleRise`의 내부 두 구간은 Gap `2`, 상승 `1` 및 하강 `1`이다. `LegacySteps`의 세 구간은 Gap `2`, 상승 `1`, 같은 높이, 하강 `1`이다. `InternalGap`은 같은 높이 Gap `6`이다.
- 기존 계산식으로 구한 최소 Jump 입력 Window는 `InternalGap`의 기본 속도에서 약 `0.167`초이다. `LegacySteps`의 같은 높이 Platform 사이 최대 속도에서는 약 `0.270`초이다. 모두 최소 `0.10`초를 넘는다.
- `Flat` 내부는 단일 연속 Ground이며 첫 Pattern 정지 출발의 실제 가속은 Step 8 Play Mode Test에서 확인한다.
- 모든 Pattern의 첫·마지막 Ground와 경계 Gap `4`는 16개 조합에서 동일한 연결 조건을 가진다. 실제 Rigidbody, Collider 모서리, Wall과 착지는 Step 8~9 Play Mode Test 책임이다.
- 현재 생산 Scene의 기존 `LegacySteps` 형태는 위 공통 계약값으로 Step 6에서 재작성한다. 이번 Step에서는 Scene을 수정하지 않는다.

### 사용자 검증 결과

- Unity Script Compilation 성공을 확인했다.
- Unity Script Compilation에서 예상치 못한 Error와 Warning이 없음을 확인했다.
- 전체 Edit Mode Test `466`개를 실행하여 모두 성공함을 확인했다.
- Edit Mode Test에서 예상치 못한 Error와 Warning이 없음을 확인했다.

### 완료 조건

- [x] 네 Pattern의 모든 Transform과 Collider 수치가 추측 없이 문서와 코드에서 확인된다.
- [x] 모든 내부 구간과 경계 구간의 기하 계약이 Unit Test를 통과한다.
- [x] Scene 수동 작업 전에 미정 수치가 남아 있지 않다.

## Step 4. Pattern 생성·교체·정리 로직을 구현하고 Unit Test로 검증한다

- 진행 상태: **완료**

### AI 작업

- 명시적으로 요청된 Pattern ID를 후행 Slot에 구성하고 StartAnchor를 선행 EndAnchor에 정렬하는 로직을 구현한다.
- Phase 3이 선택 결과를 전달할 수 있는 API만 제공하고 Difficulty 또는 무작위 선택 로직은 연결하지 않는다.
- Pattern 교체 시 이전 Geometry, Collider, Boundary 상태와 Pattern별 Runtime 상태가 남지 않도록 한다.
- 접촉 중이거나 아직 완전히 통과하지 않은 Pattern은 재사용하지 않고 동일 Boundary 요청을 중복 처리하지 않는다.
- 초기 두 Slot, 정상 교대, 여러 번 연속 진행, 잘못된 ID, 연결 불가, 중복 요청, 초기화 실패, Retry와 정리를 Unit Test로 검증한다.
- 반복 경로에서 LINQ, 매 호출 컬렉션 생성, 불필요한 Instantiate·Destroy와 정상 흐름 Log가 없는지 검사한다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Edit Mode Test만 실행한다.

### AI 수행 결과

- `InfinitePatternSlot.InitializeFromPrefabs()`가 네 Pattern Prefab 원본을 먼저 검사한 뒤 Slot별 네 인스턴스를 한 번씩 생성·캐시하도록 했다. 교체 경로는 기존 캐시의 활성 상태만 바꾸며 Instantiate·Destroy하지 않는다.
- Slot은 현재 Pattern ID와 인스턴스를 노출하고, 요청 Pattern 활성화 시 다른 캐시 인스턴스를 비활성화하며 실제 AdvanceBoundary를 해당 Pattern의 `AdvanceBoundaryPoint` 위치·방향에 정렬한다.
- `InfiniteMapPattern`에 기존 Scene 경로와 분리된 두 Slot·Prefab 입력 경로를 추가했다. 새 Slot 참조가 없는 현재 생산 Scene은 기존 경로를 유지한다.
- 새 경로는 첫 두 Slot을 `Flat`으로 시작하고, 후행 Slot의 StartAnchor를 선행 Slot의 EndAnchor에 정렬한다. 명시적 요청 ID와 Pattern ID는 `TryRequestNextPattern()`으로 전달하며 Difficulty 또는 무작위 선택을 실행하지 않는다.
- 현재 앞 Slot의 Boundary만 교체를 진행할 수 있다. Player Collider의 전체 X 범위가 재사용할 뒤 Slot의 EndAnchor를 지나지 않았으면 재사용을 거부한다.
- 잘못된 ID, 처리 완료한 요청 ID 이하의 중복·역행 요청, 이미 대기 중인 요청과 연결 불가 Pattern은 거부한다. Boundary를 여러 번 호출해도 한 요청은 한 번만 처리한다.
- Pattern 교체 전 기존 Collectible Scope를 해제하고 새 Pattern 활성화·위치 정렬 후 새 Scope를 연결한다. 실패 시 이전 Pattern ID, 위치와 Collectible 연결 상태를 복구하도록 했다.
- Retry와 재초기화는 두 Slot의 최초 위치·Pattern·Boundary 상태, 진행 횟수와 요청 상태를 복구한다. `ClearPatternSlots()`는 소유한 캐시 인스턴스를 정리하고 재초기화를 허용한다.
- `InfinitePatternSlotProgressionTests`에 초기 캐시, 인스턴스 분리, 교대·연속 진행, Anchor·Boundary 정렬, 잘못된 ID·중복 요청·연결 불가, Player의 미통과 상태, Retry, 정리 및 초기화 실패 복구 검증을 추가했다.
- 생산 Scene과 Prefab은 변경하지 않았고 Unity Build와 Test Runner를 실행하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation 성공을 확인했다.
- Unity Script Compilation에서 예상치 못한 Error와 Warning이 없음을 확인했다.
- 전체 Edit Mode Test `477`개를 실행하여 모두 성공함을 확인했다.
- Edit Mode Test에서 예상치 못한 Error와 Warning이 없음을 확인했다.
- 관련 Runtime 및 Test 코드에 LINQ 사용이 없음을 정적으로 확인했다. 사용자의 추가 요청에 따라 `CODING_STYLE.md`에 LINQ 사용 금지를 명시했다.
- 실제 생산 Scene Prefab 구성과 물리 통과는 Step 5~9 책임이며 이번 Step에서 Scene 편집이나 Build는 수행하지 않았다.

### 완료 조건

- [x] 두 Slot이 요청된 Pattern을 번갈아 안전하게 구성한다.
- [x] 사용이 끝난 Pattern의 상태와 Collider가 남지 않는다.
- [x] Phase 3 책임을 구현하지 않고 명시적 Pattern 요청 경계만 제공한다.

## Step 5. AI가 생산 Scene 작업 명세를 확정한다

- 진행 상태: **완료**

### AI 작업

- Step 1~4 결과와 생산 Scene YAML을 대조한다.
- 사용자가 생성, 이동, 이름 변경 또는 삭제할 GameObject를 전체 Hierarchy 경로로 나열한다.
- 각 GameObject의 Local Position, Rotation, Scale, Layer, 활성 상태를 값 단위로 제시한다.
- 각 Collider의 Type, Center, Size, Is Trigger와 Material을 값 단위로 제시한다.
- 각 Component와 Serialized Field에 연결할 대상의 전체 Hierarchy 경로를 제시한다.
- 기존 `Pattern_0`, `Pattern_1`, StartAnchor, EndAnchor, AdvanceBoundary와 Collectible 중 유지·이동·제거 대상을 구분한다.
- Scene 외 Prefab이 필요한 결정이라면 Prefab 저장 경로, Root 이름, Variant 여부와 Scene 참조 절차를 함께 제시한다.
- 작업 전 Scene 백업이 아니라 Git 변경 상태를 확인하고, 작업 후 저장할 정확한 Scene 및 Prefab 목록을 제시한다.

### 사용자 작업

- 없음

### 생산 Scene 조사 및 변경 범위

- 생산 Scene은 `Assets/Scenes/SampleScene.unity` 한 개이며, 현재 `World/InfiniteModeRoot/InfiniteMapPattern` 아래 `Pattern_0`(Local X `0`)과 `Pattern_1`(Local X `44`)이 있다.
- 각 기존 Pattern은 Terrain, StartAnchor, EndAnchor, AdvanceBoundary와 `PatternCollectible` 10개를 소유한다. 두 AdvanceBoundary는 각각 Player Capsule Collider, 같은 `InfiniteMapPattern`과 Boundary ID `0`/`1`을 참조하며 BoxCollider는 Trigger, Size `(1, 10, 4)`, Physics Material `None`이다.
- 현재 Scene에는 Pattern Prefab이 없고 새 Slot 또는 Prefab 참조도 없다. `Ground` Layer는 `6`이며 Player는 Scene Root `Player`의 CapsuleCollider이다.
- Step 5 시작 시 Git 변경 상태를 확인했다. 생산 Scene, Prefab, Package, Input Action과 ProjectSettings의 내용 변경은 없으며 현재 변경은 Phase 2 Runtime·Test·문서 및 Coding Style 규칙에 한정된다. Scene 백업 사본은 만들지 않는다.
- 아래 명세는 Step 6에서만 사용자가 Unity Editor로 수행한다. 이번 Step에서 Scene과 Prefab은 수정하지 않는다.

### Prefab 저장 위치와 공통 구성

- 저장 폴더: `Assets/Prefabs/InfinitePatterns/`
- 저장 Asset 및 Root 이름: `InfinitePattern_Flat.prefab` / `InfinitePattern_Flat`, `InfinitePattern_SingleRise.prefab` / `InfinitePattern_SingleRise`, `InfinitePattern_LegacySteps.prefab` / `InfinitePattern_LegacySteps`, `InfinitePattern_InternalGap.prefab` / `InfinitePattern_InternalGap`
- 네 Asset은 각각 독립 Prefab 원본이며 Variant가 아니다. Scene에 원본 Template이나 Preview 인스턴스를 남기지 않는다. `InfiniteMapPattern`의 Prefab 배열에 Project 창의 네 Prefab Asset Root에 붙은 `InfinitePatternAuthoring` Component를 연결한다.
- 아래 표에서 별도 언급이 없는 Empty GameObject는 Local Position `(0, 0, 0)`, Local Rotation `(0, 0, 0)`, Local Scale `(1, 1, 1)`, Layer `Default`, Active `true`이다. Pattern Root도 같은 값이다.

| Prefab 내부 전체 Hierarchy 경로 패턴 | 생성 방식·Component | 위치·상태 |
|---|---|---|
| `InfinitePattern_<ID>` | Empty Root + `InfinitePatternAuthoring` | 공통 기본값 |
| `InfinitePattern_<ID>/GeometryRoot` | Empty | 공통 기본값 |
| `InfinitePattern_<ID>/StartAnchor` | Empty Transform | Local Position `(-22, 0, 0)`, 나머지 기본값 |
| `InfinitePattern_<ID>/EndAnchor` | Empty Transform | Local Position `(22, 0, 0)`, 나머지 기본값 |
| `InfinitePattern_<ID>/AdvanceBoundaryPoint` | Empty Transform | Pattern별 X는 아래 표, Y `5.5`, Z `0`; Rotation `(0, 0, 0)`, Scale `(1, 1, 1)`, Active `true` |
| `InfinitePattern_<ID>/CollectibleRoot` | Empty, 자식 없음 | 공통 기본값; 최종 Collectible 배치는 Phase 4 |
| `InfinitePattern_<ID>/GeometryRoot/<지형 이름>` | Empty + `BoxCollider`만 추가 | Pattern별 Position과 Collider Size는 아래 표; Rotation `(0, 0, 0)`, Scale `(1, 1, 1)`, Layer `Ground`(6), Active `true` |
| `InfinitePattern_<ID>/GeometryRoot/<지형 이름>/Visual` | 3D Cube의 BoxCollider를 제거하여 MeshFilter와 MeshRenderer만 유지 | Local Position `(0, 0, 0)`, Rotation `(0, 0, 0)`, Scale은 부모 지형의 BoxCollider Size와 동일, Layer `Ground`(6), Active `true`; 기본 Cube Mesh와 기본 Material 사용 |

- `<ID>`는 위 네 Prefab 이름의 `Flat`, `SingleRise`, `LegacySteps`, `InternalGap`을 뜻한다. 지형 오브젝트는 Collider Size를 사용하고 Transform Scale을 `(1, 1, 1)`로 유지한다. 시각적 Cube만 해당 Size로 Scale을 설정해 Collider와 겹치게 한다. Visual 자식에는 Collider가 없어야 한다.
- 모든 지형 BoxCollider는 Enabled `true`, Center `(0, 0, 0)`, Is Trigger `false`, Physics Material `None`이다. BoxCollider 이외 Terrain Collider를 추가하지 않는다.

| Prefab | 지형 전체 경로의 마지막 이름 | Local Position | BoxCollider Size |
|---|---|---|---|
| `InfinitePattern_Flat` | `GeometryRoot/Ground_0` | `(0, 0, 0)` | `(40, 1, 4)` |
| `InfinitePattern_SingleRise` | `GeometryRoot/Ground_0` | `(-14, 0, 0)` | `(12, 1, 4)` |
| `InfinitePattern_SingleRise` | `GeometryRoot/Platform_0` | `(1, 1, 0)` | `(14, 1, 4)` |
| `InfinitePattern_SingleRise` | `GeometryRoot/Ground_1` | `(15, 0, 0)` | `(10, 1, 4)` |
| `InfinitePattern_LegacySteps` | `GeometryRoot/Ground_0` | `(-16, 0, 0)` | `(8, 1, 4)` |
| `InfinitePattern_LegacySteps` | `GeometryRoot/Platform_0` | `(-6, 1, 0)` | `(8, 1, 4)` |
| `InfinitePattern_LegacySteps` | `GeometryRoot/Platform_1` | `(4, 1, 0)` | `(8, 1, 4)` |
| `InfinitePattern_LegacySteps` | `GeometryRoot/Ground_1` | `(15, 0, 0)` | `(10, 1, 4)` |
| `InfinitePattern_InternalGap` | `GeometryRoot/Ground_0` | `(-11.5, 0, 0)` | `(17, 1, 4)` |
| `InfinitePattern_InternalGap` | `GeometryRoot/Ground_1` | `(11.5, 0, 0)` | `(17, 1, 4)` |

| Prefab | `AdvanceBoundaryPoint` Local Position | `InfinitePatternAuthoring` Pattern Id | Minimum Difficulty | Terrain Colliders 순서 |
|---|---|---|---|---|
| `InfinitePattern_Flat` | `(-4, 5.5, 0)` | `Flat` | D1 | `Ground_0` BoxCollider |
| `InfinitePattern_SingleRise` | `(0, 5.5, 0)` | `SingleRise` | D1 | `Ground_0`, `Platform_0`, `Ground_1` BoxCollider |
| `InfinitePattern_LegacySteps` | `(2, 5.5, 0)` | `LegacySteps` | D2 | `Ground_0`, `Platform_0`, `Platform_1`, `Ground_1` BoxCollider |
| `InfinitePattern_InternalGap` | `(4, 5.5, 0)` | `InternalGap` | D3 | `Ground_0`, `Ground_1` BoxCollider |

- 네 Prefab Root의 `InfinitePatternAuthoring` 필드는 각각 `Geometry Root` → 자신의 `GeometryRoot`, `Start Anchor` → 자신의 `StartAnchor`, `End Anchor` → 자신의 `EndAnchor`, `Advance Boundary Point` → 자신의 `AdvanceBoundaryPoint`, `Collectible Root` → 자신의 `CollectibleRoot`, `Terrain Colliders` → 위 표 순서의 자기 Prefab BoxCollider Component로 연결한다. 외부 Scene 참조는 Prefab에 연결하지 않는다.

### 생산 Scene Hierarchy 변경 명세

- 유지: `World`, `World/InfiniteModeRoot`, `World/InfiniteModeRoot/InfiniteMapPattern`, Scene Root `Player`, StageModeRoot와 다른 모든 GameObject. `InfiniteMapPattern`의 Component 자체도 유지한다.
- 생성: `World/InfiniteModeRoot/InfiniteMapPattern/Slot_0`, `.../Slot_0/ContentRoot`, `World/InfiniteModeRoot/InfiniteMapPattern/Slot_1`, `.../Slot_1/ContentRoot`.
- 이동: 기존 `World/InfiniteModeRoot/InfiniteMapPattern/Pattern_0/AdvanceBoundary`를 `.../Slot_0/AdvanceBoundary`로, 기존 `Pattern_1/AdvanceBoundary`를 `.../Slot_1/AdvanceBoundary`로 각각 Hierarchy Drag하여 Component와 Scene 참조를 보존한다.
- 제거: 이동한 Boundary를 제외한 기존 `World/InfiniteModeRoot/InfiniteMapPattern/Pattern_0`, `Pattern_1` 전체 계층. 각 Pattern의 기존 Terrain, Anchor 및 `PatternCollectible` 10개도 함께 제거한다. 새 네 Prefab의 Collectible Root는 Phase 2에서 빈 상태이므로 기존 Jump 경로를 새 지형에 복사하지 않는다.
- 제거 전 새 Slot과 Prefab의 참조를 모두 연결한 뒤 삭제한다. 기존 Jump Collectible Layout을 검사하는 Play Mode Test와 `Pattern_0/1` 이름을 전제로 하는 기존 생산 Scene Test는 Step 8~9에서 새 생산 구조 기준으로 갱신해야 하며, 그 전 전체 Play Mode Test 성공을 주장하지 않는다.

| Scene 경로 | Component | Local Position | Rotation | Scale | Layer | Active |
|---|---|---|---|---|---|---|
| `World/InfiniteModeRoot/InfiniteMapPattern` | 기존 `InfiniteMapPattern` | `(0, 0, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |
| `.../Slot_0` | `InfinitePatternSlot` | `(0, 0, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |
| `.../Slot_0/ContentRoot` | Transform만 | `(0, 0, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |
| `.../Slot_0/AdvanceBoundary` | 기존 `InfinitePatternBoundary` + BoxCollider | `(-4, 5.5, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |
| `.../Slot_1` | `InfinitePatternSlot` | `(44, 0, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |
| `.../Slot_1/ContentRoot` | Transform만 | `(0, 0, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |
| `.../Slot_1/AdvanceBoundary` | 기존 `InfinitePatternBoundary` + BoxCollider | `(-4, 5.5, 0)` | `(0, 0, 0)` | `(1, 1, 1)` | Default | true |

- 위 `...`는 모두 `World/InfiniteModeRoot/InfiniteMapPattern` 전체 경로를 뜻한다. `ContentRoot`는 Edit Mode에서 비어 있어야 하며 Runtime에 Slot마다 Prefab 네 인스턴스가 생성된다. Scene에 네 Prefab 인스턴스를 미리 배치하지 않는다.
- 두 AdvanceBoundary BoxCollider는 각각 Enabled `true`, Type `BoxCollider`, Center `(0, 0, 0)`, Size `(1, 10, 4)`, Is Trigger `true`, Physics Material `None`으로 설정한다. 초기 Local X `-4`는 첫 활성 `Flat`의 Boundary Point 값이며, Pattern 교체 시 해당 Pattern의 Point로 Runtime에서 이동한다.

### Scene Serialized Field 연결 명세

| 소유 Component 전체 경로 | Serialized Field | 연결 대상 전체 경로 또는 Asset |
|---|---|---|
| `World/InfiniteModeRoot/InfiniteMapPattern`의 `InfiniteMapPattern` | `First Slot` | `World/InfiniteModeRoot/InfiniteMapPattern/Slot_0`의 `InfinitePatternSlot` |
| 동일 | `Second Slot` | `World/InfiniteModeRoot/InfiniteMapPattern/Slot_1`의 `InfinitePatternSlot` |
| 동일 | `Pattern Prefabs` Size `4`, Element `0` | `Assets/Prefabs/InfinitePatterns/InfinitePattern_Flat.prefab` Root의 `InfinitePatternAuthoring` |
| 동일 | `Pattern Prefabs` Element `1` | `Assets/Prefabs/InfinitePatterns/InfinitePattern_SingleRise.prefab` Root의 `InfinitePatternAuthoring` |
| 동일 | `Pattern Prefabs` Element `2` | `Assets/Prefabs/InfinitePatterns/InfinitePattern_LegacySteps.prefab` Root의 `InfinitePatternAuthoring` |
| 동일 | `Pattern Prefabs` Element `3` | `Assets/Prefabs/InfinitePatterns/InfinitePattern_InternalGap.prefab` Root의 `InfinitePatternAuthoring` |
| 동일 | `Phase2 Player Collider` | Scene Root `Player`의 `CapsuleCollider` |
| 동일 | 기존 `First Pattern`, `First Start Anchor`, `First End Anchor`, `First Boundary`, `Second Pattern`, `Second Start Anchor`, `Second End Anchor`, `Second Boundary` | 모두 `None`; 새 Slot 경로에서 사용하지 않음 |
| `.../Slot_0`의 `InfinitePatternSlot` | `Slot Id` / `Content Root` / `Advance Boundary` | `0` / `.../Slot_0/ContentRoot` Transform / `.../Slot_0/AdvanceBoundary` Component |
| `.../Slot_1`의 `InfinitePatternSlot` | `Slot Id` / `Content Root` / `Advance Boundary` | `1` / `.../Slot_1/ContentRoot` Transform / `.../Slot_1/AdvanceBoundary` Component |
| `.../Slot_0/AdvanceBoundary`의 `InfinitePatternBoundary` | `Player Collider` / `Map Pattern` / `Boundary Id` | Scene Root `Player`의 CapsuleCollider / `World/InfiniteModeRoot/InfiniteMapPattern` Component / `0` |
| `.../Slot_1/AdvanceBoundary`의 `InfinitePatternBoundary` | `Player Collider` / `Map Pattern` / `Boundary Id` | Scene Root `Player`의 CapsuleCollider / `World/InfiniteModeRoot/InfiniteMapPattern` Component / `1` |

### 저장 대상과 정적 검사 기준

- Step 6에서 저장할 Asset은 정확히 `Assets/Scenes/SampleScene.unity`와 위 네 `.prefab` 및 Unity가 생성하는 해당 폴더·Prefab `.meta`이다. Package, Input Action, ProjectSettings, StageModeRoot와 다른 Scene은 저장·수정하지 않는다.
- 사용자는 Inspector의 Missing Script, Missing Reference와 필수 Field `None` 여부만 Scene 저장 직전에 확인한다. 위치·길이·Collider 값, GUID·fileID, Layer, 16개 연결 조합, 중복 지형과 잔여 Collider는 저장 후 AI가 YAML과 코드로 정적으로 판정한다.
- AI는 Step 7에서 위 전체 Hierarchy, 네 Prefab의 ID·Difficulty·지형값·Boundary Point·Collider·Layer·활성 상태·참조, 두 Slot 참조, Player 연결, 기존 Pattern 잔여물, GUID·fileID와 의도하지 않은 Asset 변경을 검사한다.
- Scene 저장 직후 Play Mode, Test Runner 또는 Build를 실행하지 않는다. AI의 Step 7 정적 검사 결과를 먼저 요청한다.

### 완료 조건

- [x] 사용자가 추측하지 않고 수행할 수 있는 Hierarchy·Component·Field·값 명세가 준비되었다.
- [x] 변경 대상과 유지 대상이 구분되었다.
- [x] Scene 작업 후 AI가 정적으로 판정할 체크 항목이 준비되었다.

## Step 6. 네 Pattern의 생산 Scene 또는 Prefab 구성을 수동으로 작성한다

- 진행 상태: **완료**

### 선행 조건

- Step 5의 전체 작업 명세가 확정되기 전에는 이 Step을 시작하지 않는다.

### 사용자 작업

- AI가 Step 5에서 지정한 Scene을 Unity Editor에서 연다.
- 지정된 전체 Hierarchy 경로에 네 Pattern 원본과 두 재사용 Slot을 생성하거나 기존 객체를 정리한다.
- AI가 지정한 Transform, Layer, 활성 상태와 이름을 그대로 입력한다.
- Ground와 Platform에 지정된 Collider, Is Trigger와 Material 값을 적용한다.
- StartAnchor, EndAnchor와 AdvanceBoundary를 지정된 위치와 방향으로 배치한다.
- `InfiniteMapPattern`과 관련 Component의 모든 Serialized Field에 AI가 지정한 참조를 연결한다.
- Missing Script, Missing Reference와 None 상태로 남은 필수 Field가 없는지 Inspector에서 확인한다.
- AI가 지정한 Scene과 Prefab만 저장한다.
- 저장 직후 Play Mode를 실행하거나 임의로 위치를 조정하지 않고 AI의 정적 검사를 요청한다.

### AI 작업

- Scene 또는 Prefab은 직접 수정하지 않는다.
- 사용자가 저장한 YAML과 `.meta`를 다음 Step에서 검사한다.

### 사용자 작업 및 확인 결과

- 사용자가 `SampleScene.unity`의 두 Slot과 네 독립 Pattern Prefab을 Unity Editor에서 작성·저장했다.
- 첫 저장에서 네 Prefab의 `InfinitePatternAuthoring.Terrain Colliders` 원소가 모두 `None`인 것을 AI가 확인했다. 사용자가 각 지형 BoxCollider를 지정 순서로 연결하고 네 Prefab을 다시 저장했다.
- 최종 Hierarchy·값·참조 및 변경 범위는 Step 7 정적 검사에서 확인했다. AI는 Scene·Prefab을 수정하거나 Unity Build·Test Runner를 실행하지 않았다.

### 완료 조건

- [x] 네 Pattern과 두 Slot이 지정된 Hierarchy에 존재한다.
- [x] 모든 Transform, Collider, Layer, Boundary와 Serialized Reference가 입력되었다.
- [x] 지정된 Scene 및 Prefab 외 Asset 변경이 없다.

## Step 7. 생산 Scene과 Prefab 구성을 정적으로 검사하고 수정 사항을 확정한다

- 진행 상태: **완료**

### AI 작업

- Scene 및 Prefab YAML에서 네 Pattern ID, Hierarchy, 활성 상태, Transform, Collider, Layer, Trigger와 Serialized Reference를 검사한다.
- StartAnchor와 EndAnchor 길이·높이·접선, 연결 Ground Gap·폭·높이와 AdvanceBoundary 위치를 계산한다.
- fileID와 GUID가 실제 대상 및 `.meta`를 가리키고 Missing Script 또는 끊긴 참조가 없는지 검사한다.
- 전체 16개 연결 조합이 공통 연결 계약을 만족하는지 생산 구성 값으로 검사한다.
- 중복 Collider, 겹친 Ground, Slot 밖 잔여 Geometry와 의도하지 않은 Scene·Prefab·Package·Input Action 변경을 검사한다.
- 오류가 있으면 전체 Hierarchy 경로, Component, Field, 현재 값과 수정 값만 사용자에게 제시한다.

### 사용자 작업

- AI가 정적 검사에서 식별한 Scene 또는 Prefab 오류가 있을 때만 지정된 값을 수정하고 저장한다.
- 오류가 없다면 사용자 작업은 없다.

### AI 정적 검사 결과

- 네 독립 Prefab의 ID·Difficulty·Hierarchy·Transform·활성 상태, 지형 Collider와 분리된 Visual의 수치·Layer·Component 구성을 Step 5 명세와 대조했다. `Terrain Colliders` 배열은 Flat 1개, SingleRise 3개, LegacySteps 4개, InternalGap 2개가 지정 순서의 실제 BoxCollider를 참조한다.
- 모든 StartAnchor `(-22, 0, 0)`와 EndAnchor `(22, 0, 0)`의 길이 `44`, 공통 Ground 상단 Y `0.5`, 폭 `4`, 경계 Gap `4`를 확인했다. 네 Pattern의 첫·마지막 Ground가 동일한 연결 계약이므로 16개 순서 조합에서 경계 기하가 같다. Pattern별 AdvanceBoundaryPoint 위치도 명세와 일치한다.
- 생산 Scene의 `Slot_0` X `0`, `Slot_1` X `44`, 비어 있는 ContentRoot, 각 Slot의 AdvanceBoundary 이동·ID·Player Collider·Trigger 설정, `InfiniteMapPattern`의 네 Prefab·두 Slot 참조와 기존 여덟 필드 `None`을 확인했다. `Pattern_0/1`과 기존 Pattern Collectible은 제거되고 StageModeRoot와 Stage Collectible은 유지되었다.
- Scene과 네 Prefab에서 GUID가 없는 양의 로컬 fileID 참조가 모두 실제 YAML 객체를 가리킨다. Script GUID는 대응 `.meta`, 네 Prefab GUID는 각 Prefab `.meta`와 일치하며 Visual Material GUID는 설치된 URP 패키지의 Lit Material에 존재한다.
- Scene·Prefab·Package·Input Action·ProjectSettings 변경 범위를 확인했다. 이번 Scene 작성으로 변경된 생산 Asset은 `SampleScene.unity`, 네 Prefab 및 해당 `.meta`뿐이다. 기존 Phase 2 Runtime·Test·문서 변경은 별도 선행 작업으로 유지했다.
- 정적 검사에서 남은 수정 사항은 없다. Unity Script Compilation, Play Mode, Test Runner와 Build는 수행하지 않았으며 실제 물리 통과는 Step 8~9에서 검증한다.

### 완료 조건

- [x] 생산 구성의 모든 정적 계약이 통과한다.
- [x] Missing 참조와 의도하지 않은 Asset 변경이 없다.
- [x] 정적 검사로 판정 가능한 항목이 후속 수동 검증에 남아 있지 않다.

## Step 8. Pattern별 실제 물리 통과를 Play Mode Test로 검증한다

- 진행 상태: **완료**

### AI 작업

- 생산 Pattern 구성을 사용하는 Play Mode Test를 작성한다.
- `Flat`의 정지 출발 자동 가속과 평탄 진행을 검증한다.
- `SingleRise`의 기본 및 최대 속도 상승 진입, 착지와 이탈을 검증한다.
- `LegacySteps`의 두 Platform 각각에 대한 진입, 착지와 이탈을 검증한다.
- `InternalGap`의 기본 및 최대 속도 Gap 통과와 착지를 검증한다.
- 각 Test에서 실제 Player Rigidbody, Jump, Collision과 Ground 상태를 사용한다.
- Wall 전면 또는 모서리 접촉 후 낙하가 계속되고 Ground 착지 시 접촉 상태가 복구되는지 검증한다.
- Test가 생성하거나 변경한 객체와 상태를 TearDown에서 정리한다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Play Mode Test만 실행한다.

### AI 수행 결과

- 생산 Scene에서 실행 중 생성된 네 Pattern Prefab 캐시를 사용하는 `InfinitePatternTraversalIntegrationTests`를 추가했다. Scene·Prefab은 수정하지 않았다.
- Flat 정지 출발 자동 가속, SingleRise 기본·최대 속도 상승 착지, LegacySteps 두 Platform의 진입·착지, InternalGap 기본·최대 속도 통과·착지, SingleRise 전면 Wall 접촉 후 낙하와 Ground 복구를 검증한다.
- 실제 Player Rigidbody, Jump 입력, CollisionSystem의 Ground·Wall 상태를 사용한다. Test가 시작 위치·속도와 Jump 입력을 제어하므로 사람의 타이밍 재현이 필요하지 않다.
- 각 Test는 생산 Scene을 새로 로드하고 변경한 InfiniteModeSystem 설정을 TearDown에서 복구한다. Unity Build와 Test Runner는 AI가 실행하지 않았다.

### 이번 Step의 사용자 검증 요청

- Unity Editor의 Script Compilation 결과와 예상치 못한 Error·Warning 부재를 확인한다.
- Play Mode Test Runner에서 `InfinitePatternTraversalIntegrationTests` 클래스의 8개 Test만 실행하고 성공 수 및 예상치 못한 Error·Warning을 알려준다.
- 기존 `Pattern_0/1`과 Pattern Collectible을 전제로 하는 Play Mode Test는 Step 9에서 갱신하므로, 이번에는 전체 Play Mode Suite를 실행할 필요가 없다.

### 첫 Play Mode 검증 및 수정

- 사용자가 8개 중 `InternalGap_BaseSpeed_CrossesAndLands`, `InternalGap_MaximumSpeed_CrossesAndLands` 두 Test의 실패를 보고했다. 둘 다 Jump 전 시작 Ground 확인에서 실패했으며 실제 Gap 통과 판정까지 진행하지 않았다.
- Test가 Pattern 전환과 Player 순간이동 직후 물리 Fixed Step 없이 Ground 상태를 읽던 순서를 수정했다. 이제 위치를 지정하고 Fixed Step 한 번 후 Ground를 확인한 뒤 목표 속도와 Jump 입력을 설정한다. 실패 시 Player와 Slot 위치를 출력하도록 했다.
- 생산 Prefab과 Scene의 InternalGap 지형값은 앞선 정적 검사와 일치하므로 이번 수정에서는 Scene·Prefab을 변경하지 않았다. 실제 물리 결과는 사용자 재실행으로 확인한다.
- 재실행에서 `InternalGap_MaximumSpeed_CrossesAndLands`는 통과했고 `InternalGap_BaseSpeed_CrossesAndLands`만 착지 판정에 실패했다. 기본 속도 `8`에서 같은 높이 Jump의 약 `7.84` 유닛 비행 거리에 비해 기존 Test 출발 X `-6`은 착지 Ground 시작 X `3`까지 `9` 유닛 떨어져 있어 너무 이른 Jump 입력이었다. 출발 X를 기존 Ground 끝 `-3`에서 `1.2` 유닛 앞인 `-4.2`로 조정했다. 실패 메시지에는 최종 Player 위치와 속도를 추가했다. 생산 지형 수치는 변경하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation 성공과 예상치 못한 Error·Warning 부재를 확인했다.
- `InfinitePatternTraversalIntegrationTests` 클래스의 Play Mode Test 전체 성공을 확인했다.
- Unity Build는 AI가 수행하지 않았고 Scene·Prefab도 AI가 변경하지 않았다.

### 완료 조건

- [x] 네 Pattern을 기본 및 최대 속도 조건에서 통과한다.
- [x] 실제 착지, Collision, Wall 접촉 해제와 Ground 복구가 검증된다.
- [x] 사람의 Jump 타이밍 재현을 요구하지 않는다.

## Step 9. 전체 16개 연결 조합과 연속 진행을 Play Mode Test로 검증한다

- 진행 상태: **완료**

### AI 작업

- 앞 Pattern 4종과 뒤 Pattern 4종의 전체 16개 조합을 생산 구성으로 생성한다.
- 앞 EndAnchor와 뒤 StartAnchor 정렬, 경계 Gap, Camera 추적, Collision과 Boundary 진행을 검증한다.
- 기본 및 최대 속도에서 필요한 경계 Jump와 다음 Pattern 착지를 검증한다.
- 여러 번 Slot을 교대하여 사용이 끝난 Pattern이 Player 진행을 방해하지 않고 AdvanceBoundary가 한 번만 처리되는지 검증한다.
- Pattern 전환 중 Collectible Scope가 중복 또는 누수되지 않는 기존 계약을 회귀 검증하되 Phase 4 안내 경로는 추가하지 않는다.
- Pause, Resume, Result와 Retry에서 Pattern Slot과 Boundary 상태가 올바르게 유지 또는 초기화되는지 검증한다.

### 사용자 작업

- AI가 지정한 관련 Play Mode Test만 실행한다.
- Test에서 예상하지 않은 Error 및 Warning이 없는지 확인한다.

### AI 수행 결과

- 생산 Scene의 두 Slot에 네 Pattern 캐시를 조합하는 `InfinitePatternConnectionIntegrationTests`를 추가했다. 기본 속도 `8`과 최대 속도 `14`에서 각각 16개 연결의 Anchor·경계 Ground·실제 Jump·착지·Camera 추적을 검사한다.
- 실제 Player가 Front Boundary Trigger를 통과해 요청을 한 번만 진행하는 Test, 명시적 요청 8회에 따른 Slot 교대·중복 처리 거부·Collectible Scope 두 개 유지·Retry Reset Test, Pause·Resume·Result 후 재시작 Test를 추가했다. Phase 3 자동 선택과 Phase 4 Collectible 배치는 구현하지 않았다.
- 기존 생산 Scene Play Mode Test의 `Pattern_0/1` 참조를 `Slot_0/1`과 현재 Prefab Authoring 참조로 갱신했다. Phase 2의 빈 Collectible Root를 전제로 기존 Collectible Layout Test를 갱신해 빈 등록 목록과 두 Scope를 검사한다.
- 전체 Edit·Play Mode Test의 생산 Scene 이름·Hierarchy·등록 수 가정을 정적으로 검색했다. `CollectibleLifecycleIntegrationTests`에 남아 있던 Infinite Mode의 옛 Collectible 20개 등록 기대값을 등록 0개와 활성 Scope 2개로 변경했다. `InfiniteCollectibleLayoutIntegrationTests`는 두 Slot의 캐시된 네 Pattern 각각에 대해 빈 Collectible Root와 Slot 간 인스턴스 분리를 확인한다. 기존 `InfiniteMapPatternTests`의 Pattern0/1은 생산 Scene 참조가 아닌 독립 Legacy 경로 Fixture이므로 유지했다.
- Scene·Prefab은 AI가 수정하지 않았고 Unity Build·Test Runner도 실행하지 않았다. 관련 코드에는 LINQ를 사용하지 않았다.

### 이번 Step의 사용자 검증 요청

- Unity Script Compilation 성공과 예상치 못한 Error·Warning 부재를 확인한다.
- Play Mode Test Runner에서 `InfinitePatternConnectionIntegrationTests` 5개, `InfiniteModeIntegrationTests`, `InfiniteCollectibleLayoutIntegrationTests`, `CollectibleLifecycleIntegrationTests` 클래스만 실행하고 각 클래스의 성공·실패 수 및 예상치 못한 Error·Warning을 알려준다.
- Test의 물리 Jump 타이밍과 요청 ID는 코드가 자동으로 입력한다. Scene 수동 작업은 없으며 정적 값 재확인은 필요하지 않다.

### 사용자 검증 결과 및 실행 시간 판단

- Unity Script Compilation 성공과 예상치 못한 Error·Warning 부재를 확인했다.
- 전체 Edit Mode Test `477`개, Play Mode Test `207`개를 실행해 모두 성공하고 예상치 못한 Error·Warning이 없음을 확인했다.
- `AllSixteenConnections` 두 Test는 각각 16개 조합을 실제 Player Rigidbody와 Fixed Step으로 통과시켜 총 32회 물리 Jump·착지를 확인한다. 대기 시간의 대부분은 착지까지 필요한 물리 시간이며, 각 조합의 Camera 확인에도 렌더 프레임이 필요하다. 16개 정적 연결 검사와 소수 대표 조합의 물리 통과로 줄이면 Step 9의 모든 조합 실제 통과 보장이 약해지므로 현재 검증 범위를 유지한다.

### 완료 조건

- [x] 전체 16개 연결 조합이 연결 및 실제 통과 Test를 통과한다.
- [x] 장시간 교대 진행에서 잔여 Pattern과 중복 Boundary 처리가 없다.
- [x] 기존 Camera, Collision, Collectible Scope, Pause, Result와 Retry에 회귀가 없다.

## Step 10. 문서와 Phase 경계를 최종 정리한다

- 진행 상태: **완료**

### AI 작업

- 생산 Pattern의 실제 Hierarchy, Transform, Collider와 참조 계약을 `InfiniteMode.md`에 반영한다.
- `InfiniteModeSystem.md`와 Roadmap에 Phase 2가 소유하는 생성·연결·정리 책임과 Phase 3 입력 경계를 반영한다.
- Pattern ID, 기하 수치, Scene 값, Unit Test와 Play Mode Test가 서로 일치하는지 정적으로 검사한다.
- Phase 3 Difficulty 선택 연동과 Phase 4 Collectible·UI 구현이 섞이지 않았는지 확인한다.

### 사용자 작업

- 없음

### AI 수행 결과

- `InfiniteMode.md`에 생산 Scene의 두 Slot, 네 독립 Prefab의 전체 지형·Anchor·Boundary Point 수치, Collider·Layer, 빈 Collectible Root와 Scope 유지 계약을 반영했다. Pattern별 Boundary Point가 실제 Slot Trigger 위치를 결정함을 명시했다.
- `InfiniteModeSystem.md`에 Phase 2의 `InfiniteMapPattern`·Slot 생성·캐시·연결·정리 책임과 명시적 Pattern 요청 API를 명시했다. Difficulty·난수·반복 제한과 선택 상태의 `InfiniteModeSystem` 연동은 Phase 3 후속 책임으로 분리했다.
- `IMPLEMENTATION_ROADMAP_004.md`의 Phase 2 생산 구조와 검증 범위를 현재 구현에 맞추고, 일반 Jump가 통과 조건이며 Momentum Landing은 필수가 아님을 명시했다. Phase 2는 Step 11 Build·최종 검증 전까지 진행 중으로 유지하고 Phase 3 선택·Phase 4 Collectible 안내/UI 책임을 분리했다.
- Pattern ID·Difficulty, 지형 수치·Boundary Point, 두 Slot과 네 Prefab 참조를 생산 코드·Prefab YAML·Scene YAML 및 Edit·Play Mode Test와 정적으로 대조했다. Step 9에서 보고된 Edit Mode `477`개와 Play Mode `207`개 성공은 유지되며 이번 Step에서 Unity Test Runner 또는 Build를 실행하지 않았다.
- Scene·Prefab·Runtime·Test는 변경하지 않았고 문서만 수정했다. 이번 Step의 수동 작업은 없다.

### 완료 조건

- [x] System, Feature, Roadmap, Scene과 Test 계약이 일치한다.
- [x] Phase 3과 Phase 4의 후속 책임이 명확하다.
- [x] 확정 수치와 생산 참조를 문서에서 확인할 수 있다.

## Step 11. 전체 정적 검사와 전체 자동 Test를 수행한다

- 진행 상태: **완료 (Build 제외 결정 기록)**

### AI 작업

- Runtime, Test, Scene, Prefab, 문서와 대응 `.meta` 및 GUID를 검사한다.
- Serialized Reference, Pattern ID, 기하 수치, Collider, Layer, Boundary와 전체 연결 행렬을 검사한다.
- Update, FixedUpdate와 Trigger 반복 경로의 LINQ, 매 Frame 할당, 불필요한 Instantiate·Destroy와 정상 흐름 Log를 검사한다.
- `Ignore`, `Explicit`, 임의 성공, 조건부 제외, 약화된 기대값과 중복 Test를 검사한다.
- 변경 파일이 Phase 2 범위 안에 있는지 확인하고 `git diff --check`를 수행한다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- 전체 Edit Mode Test를 한 번 실행하고 모두 성공하는지 확인한다.
- 전체 Play Mode Test를 한 번 실행하고 모두 성공하는지 확인한다.
- Test 관련 예상하지 않은 Error 및 Warning 부재를 확인한다.
- 사용자 결정에 따라 이번 Phase에서는 Unity Build를 실행하지 않는다. Build 검증은 Phase 2 완료 판정의 필수 조건에서 제외하고 미검증 사항으로 기록한다.

### AI 정적 검사 결과

- Runtime·Test·Scene·네 Prefab·문서와 대응 `.meta`를 대조했다. 네 Pattern ID·Difficulty·지형 수치와 Collider·Layer, Pattern별 Boundary Point, 두 Slot의 Player·Boundary·Prefab Serialized Reference는 생산 계약과 일치한다.
- Scene 및 네 Prefab의 GUID 없는 양의 로컬 fileID는 모두 실제 YAML 객체에 대응한다. 네 Prefab GUID는 각 `.meta`와 Scene 배열에 일치하며 새 Script GUID도 대응 `.meta` 및 Component 참조에 일치한다. 첫·마지막 Ground는 모든 Pattern에서 동일한 경계 조건을 제공하여 16개 연결 조합의 Anchor 정렬과 Ground Gap `4`를 유지한다.
- 관련 Runtime과 Test에서 LINQ, `Ignore`·`Explicit`·`Assert.Ignore`·`Assert.Inconclusive`·임의 성공·조건부 제외를 발견하지 못했다. Pattern 전환 반복 경로에서 Instantiate·Destroy를 수행하지 않으며, 네 캐시 생성은 Slot 초기화 시에만, Destroy는 명시적 정리 시에만 수행한다. 정상 진행 반복 경로에 Log가 없다. Pattern별 물리와 16개 연결 Test는 서로 다른 책임을 검증한다.
- Scene·Prefab·Package·Input Action·ProjectSettings 변경 범위를 확인했다. 생산 변경은 사용자가 작성한 `SampleScene.unity`와 네 Prefab·`.meta`이고, 코드·Test·문서는 Phase 2 및 사용자가 요청한 LINQ 규칙 범위이다. Package·Input Action·ProjectSettings 내용 변경은 없다.
- `git diff --check` 전체 실행에서 Unity가 새로 직렬화한 `SampleScene.unity`의 빈 `m_Name:` 6줄만 후행 공백으로 보고했다. 다른 파일은 통과했다. 같은 형태의 줄은 기존 Scene YAML에도 다수 있으며 동작·참조에 영향이 없는 Unity 직렬화 형식이므로 Scene 수정 없이 생성물 예외로 기록한다.
- Step 9 이후 Runtime·Test·Scene·Prefab 변경이 없으므로, 사용자가 이미 확인한 Unity Script Compilation 성공, Edit Mode `477`개·Play Mode `207`개 전체 성공 및 예상치 못한 Error·Warning 부재를 이번 Step의 자동 Test 근거로 재사용한다. AI는 Unity Test Runner 또는 Build를 실행하지 않았다.

### 남은 사용자 작업

- 없음. 사용자 결정에 따라 이번 Phase에서 Build를 실행하지 않으며 Build 성공 여부와 Build Error·Warning 부재는 미검증으로 남긴다. Scene 편집, 기하 수치 재확인, Edit Mode·Play Mode Test 재실행도 필요하지 않다.

### 완료 조건

- [x] 전체 정적 검증이 통과한다. Unity Scene YAML의 생성된 빈 `m_Name:` 후행 공백 6줄은 위 예외로 기록한다.
- [x] 전체 Edit Mode 및 Play Mode Test가 통과한다.
- [x] Unity Build는 사용자 결정에 따라 이번 Phase에서 제외했고 성공 여부는 주장하지 않는다.
- [x] 예상하지 않은 Compile 및 Test Error와 Warning이 없다. Build 결과는 미검증으로 구분한다.
- [x] Phase 2 범위 밖 구현이 포함되지 않았다.

## Step 12. 최소 화면 확인과 Phase 2 완료 근거를 정리한다

- 진행 상태: **완료 (Build 제외)**

### AI 작업

- 기하 수치, 참조, 연결, 물리 통과와 상태 초기화는 정적 검사 및 자동 Test 결과를 사용하고 수동 체크리스트에서 제외한다.
- 자동 판정이 어려운 Pattern 식별성, Camera 화면 전환과 시각적 떨림만 최소 수동 확인 대상으로 지정한다.
- 생산 Scene 및 Asset 변경과 전체 검증 결과를 확인한다.

### 사용자 작업

- Unity Editor의 Prefab Mode에서 `InfinitePattern_Flat`, `InfinitePattern_SingleRise`, `InfinitePattern_LegacySteps`, `InfinitePattern_InternalGap`을 이 순서로 한 번 열어 네 지형 형태가 서로 구분되는지만 확인한다.
- Play Mode Test Runner의 `InfinitePatternConnectionIntegrationTests.FrontBoundary_PhysicalTrigger_AdvancesOnlyOnce`를 Game 화면을 보며 한 번 실행한다. `Flat → Flat` 연결 중 Camera 또는 지형에 눈에 띄는 순간 이동, 겹침과 빈 화면이 없는지만 관찰한다. Test 시작·종료 또는 다른 Test 사이의 화면 전환은 판정 대상이 아니다.
- Phase 2 Runtime에는 다음 Pattern의 자동 선택 호출자가 없으므로 일반 플레이에서 네 Pattern의 고정된 연속 출현을 요구하지 않는다. Phase 3에서 자동 선택 연결 후 실제 연속 연출을 별도로 확인한다.
- 입력 타이밍, 수치, 전체 16개 조합과 장시간 반복을 수동으로 재검증하지 않는다.
- 화면 확인 중 Console에 예상하지 않은 Error 및 Warning이 없는지 확인한다.

### 화면 검증 후 AI 작업

- 정적 검사, Compile, 전체 Test, Build 제외 결정과 최소 화면 검증 결과를 기록한다.
- Asset 및 Scene 변경과 미해결 사항을 기록한다.
- 별도 Prototype 4 Phase 2 Verification Result Task 문서를 작성한다.
- 모든 완료 조건을 충족한 경우에만 Roadmap Phase 2를 `완료`로 변경한다.

### AI 준비 결과

- Step 7·11의 정적 검사, Unity Script Compilation, Edit Mode `477`개와 Play Mode `207`개 전체 성공 결과를 확인했다. 생산 Scene 변경은 `SampleScene.unity`, 네 독립 Prefab과 해당 `.meta`에 한정되며 Package·Input Action·ProjectSettings 내용 변경은 없다.
- 사용자의 이번 Phase Build 미수행 결정을 Step 11 완료 조건, Roadmap 및 `20260913_01_Phase2VerificationResult.md`에 미검증 사항으로 기록했다. Build 성공은 주장하지 않는다.
- Phase 2 Runtime에는 `TryRequestNextPattern`의 생산 호출자가 없으므로 일반 Play Mode에서 네 Pattern의 자동 연속 출현은 관찰할 수 없다. 네 Prefab 형태 확인과 기존 생산 Scene의 `Flat → Flat` 실제 Trigger 진행 화면만 최소 수동 확인 대상으로 지정했다. Scene·Prefab·Runtime·Test는 AI가 수정하지 않았다.
- 사용자 확인 결과 네 Prefab은 모두 시각적으로 구분된다. `FrontBoundary_PhysicalTrigger_AdvancesOnlyOnce` 실행 중에는 시각적 변화가 없었다. 현재 생산 Scene의 양쪽 Slot이 `Flat`이고 재사용 Slot은 화면 앞쪽으로 이동하므로, 이 관찰은 예상과 일치한다. 다만 다른 형태의 Pattern이 화면에 등장하는 연출까지 확인한 결과는 아니다. 실제 Boundary 진행은 자동 Test의 상태 단언으로 검증하며, 일반 플레이 자동 선택과 서로 다른 Pattern의 연속 화면 확인은 Phase 3에서 수행한다.
- 확인 결과를 Verification Result와 Roadmap에 반영했다. Build 검증은 이번 Phase에서 제외한다.

### 완료 조건

- [x] 네 Pattern 제작, 연결, 생성·정리와 실제 통과가 검증되었다.
- [x] 자동 판정 가능한 항목이 수동 결과에 의존하지 않는다.
- [x] 필요한 정적, Compile, Test, Build 제외 결정과 최소 화면 결과가 기록되었다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 수동 작업 요약

사용자가 직접 수행하는 작업은 아래 항목으로 제한한다.

1. Step 1에서 Pattern Authoring 구조를 결정한다.
2. 코드 변경 Step마다 Unity Script Compilation과 AI가 지정한 관련 Test를 실행한다.
3. Step 6에서 AI가 확정한 값과 참조대로 생산 Scene 또는 Prefab을 편집하고 저장한다.
4. Step 7에서 정적 오류가 발견된 경우에만 지정된 Scene 또는 Prefab 값을 수정한다.
5. Step 11에서 전체 Edit Mode 및 Play Mode Test 결과를 확인한다. 사용자 결정에 따라 이번 Phase에서는 Unity Build를 실행하지 않는다.
6. Step 12에서 Unity Editor로 네 Prefab 식별성과 연결 순간의 화면 이상만 한 번 확인한다.

Pattern 기하 계산, Anchor 오차, Collider 수치, 연결 16개 조합, Jump 타이밍, 중복 요청, Retry 초기화와 장시간 반복은 수동 작업에 포함하지 않고 정적 검사 및 자동 Test로 처리한다.

---

# 예상 변경 대상

## Runtime

- `Assets/Scripts/Runtime/Features/InfiniteMapPattern.cs`
- Phase 2 Authoring 또는 Slot 계약을 위한 신규 Runtime 파일과 `.meta`

## Test

- 기존 Infinite Pattern 관련 Edit Mode 및 Play Mode Test
- Pattern Authoring, Slot, Geometry와 전체 연결 조합을 위한 신규 Test와 `.meta`

## Scene 및 Prefab

- `Assets/Scenes/SampleScene.unity`
- Step 1 결정에 따라 추가되는 Pattern Prefab

## 문서

- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_004.md`
- 현재 Task 문서
- Phase 2 Verification Result Task 문서

---

# 제외 범위

- Difficulty 기반 Pattern 무작위 선택의 `InfiniteModeSystem` 연동
- Run 진행도와 Difficulty Runtime Data UI 연결
- Pattern별 최종 Collectible 안내 경로
- Distance Score, Collectible Score와 Total Score 변경
- Player 자동 이동, Jump와 Momentum Landing 수치 변경
- Pattern별 등장 빈도와 난이도 밸런스 조정
- 새로운 장애물, 이동 Platform과 특수 이동 규칙
- Input Action 변경

---

# 후속 작업

Phase 2 완료 후 Prototype 4 Phase 3에서 진행도, Difficulty와 Pattern 선택 상태를 생산 Runtime 흐름에 연결한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_004.md`
- `AI/90_Tasks/Prototype_4/20260910_02_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_4/20260911_01_Phase1VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 작성 완료 기준

- Roadmap Phase 2 목표와 완료 조건을 실행 가능한 Step으로 분해했다.
- 실제 Scene 및 Unity Editor 수동 작업의 선행 조건과 수행 내용을 명시했다.
- 정적 검사, Edit Mode Unit Test, Play Mode Test와 수동 검증 책임을 구분했다.
- 자동 판정 가능한 항목을 수동 작업으로 요구하지 않았다.
- Phase 3, Phase 4와 밸런스 조정을 Phase 2 범위에서 제외했다.
