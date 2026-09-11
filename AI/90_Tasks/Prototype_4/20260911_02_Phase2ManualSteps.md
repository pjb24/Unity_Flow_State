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

- 진행 상태: **대기**

### AI 작업

- 현재 `InfiniteMapPattern`, 생산 Scene 계층, Collectible Scope, Boundary와 Retry 초기화 흐름을 조사한다.
- 네 Pattern 원본을 Scene 내부 비활성 Template, Prefab 또는 직렬화 데이터 기반으로 관리하는 선택지를 제시한다.
- 각 선택지에 대해 Serialized Reference 안정성, 두 Slot 교체 비용, Test 구성, Scene 복잡도, Phase 3 선택 연동과 Phase 4 Collectible 확장 영향을 설명한다.
- 두 재사용 Slot에서 Pattern ID를 교체할 때 Transform, Collider, Boundary와 Collectible 상태가 남지 않는 구조를 권장안으로 제시한다.
- Pattern 원본 보관 위치, Slot 구성 단위, Runtime 생성 방식과 Destroy 또는 Pooling 범위를 사용자 결정 항목으로 분리한다.

### 사용자 작업

- AI가 제시한 선택지 중 사용할 Authoring 구조를 결정한다.
- 화면 비교가 필요한 선택지만 남은 경우에만 AI가 지정한 최소 Blockout을 별도 Scene 사본에서 비교한다.

### 완료 조건

- [ ] Pattern 원본 저장 위치와 형식이 확정되었다.
- [ ] 두 재사용 Slot의 생성·교체·정리 방식이 확정되었다.
- [ ] Phase 3 선택 연동과 Phase 4 Collectible 책임이 Phase 2 구현에서 분리되었다.

## Step 2. Pattern Authoring과 Slot 계약을 코드 및 Unit Test로 확정한다

- 진행 상태: **대기**

### AI 작업

- Step 1 결정에 따라 Pattern ID, Geometry Root, StartAnchor, EndAnchor, AdvanceBoundary와 지형 Collider를 표현하는 최소 계약을 구현한다.
- 누락 및 중복 ID, 잘못된 Difficulty, 잘못된 Anchor, Boundary, Collider, Layer와 Trigger 설정을 실행 전에 거부한다.
- 두 Slot이 서로 다른 인스턴스를 소유하고 현재 및 후행 Pattern ID를 명확히 노출하도록 한다.
- 초기화 실패 시 부분 상태가 남지 않고 재초기화와 Retry가 동일한 시작 상태를 만들도록 한다.
- 유효, 누락, 중복, 잘못된 참조, 중복 초기화와 초기화 실패 복구를 Edit Mode Unit Test로 검증한다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] Scene 작성 전에 필요한 Pattern Authoring과 Slot 계약이 코드로 고정되었다.
- [ ] 잘못된 구성과 초기화 경계가 Unit Test로 검증된다.
- [ ] Scene에서 사용자가 입력해야 할 Field와 참조 목록이 확정되었다.

## Step 3. 네 Pattern의 기하 수치를 계산하고 Unit Test로 확정한다

- 진행 상태: **대기**

### AI 작업

- 모든 Pattern의 StartAnchor와 EndAnchor 간 X 길이 `44`, 같은 Y·Z, +X 접선과 연결 Ground 조건을 적용한다.
- `Flat`의 평탄 Ground, `SingleRise`의 한 번의 상승과 넓은 착지, `LegacySteps`의 기존 두 Platform, `InternalGap`의 같은 높이 내부 Gap 수치를 계산한다.
- 기본 속도 `8`과 최대 속도 `14`, Jump 높이 `3`, 중력 `25`, Capsule 반지름 `0.5`, 높이 `2`, 최소 Jump 입력 Window `0.10`초를 사용한다.
- Pattern 내부의 모든 이륙 Ground, Gap, 상승·하강, 착지면과 수직 장애물을 `InfinitePatternTraversalMath`로 검사한다.
- 최소·최대 경계와 경계 밖 값을 Edit Mode Unit Test로 검증하고 Test에서 계산식을 복제하지 않는다.
- 확정된 Pattern별 Transform, Collider 크기와 위치를 문서 표로 작성한다.

### 사용자 작업

- 없음

### 완료 조건

- [ ] 네 Pattern의 모든 Transform과 Collider 수치가 추측 없이 문서와 코드에서 확인된다.
- [ ] 모든 내부 구간과 경계 구간의 기하 계약이 Unit Test를 통과한다.
- [ ] Scene 수동 작업 전에 미정 수치가 남아 있지 않다.

## Step 4. Pattern 생성·교체·정리 로직을 구현하고 Unit Test로 검증한다

- 진행 상태: **대기**

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

### 완료 조건

- [ ] 두 Slot이 요청된 Pattern을 번갈아 안전하게 구성한다.
- [ ] 사용이 끝난 Pattern의 상태와 Collider가 남지 않는다.
- [ ] Phase 3 책임을 구현하지 않고 명시적 Pattern 요청 경계만 제공한다.

## Step 5. AI가 생산 Scene 작업 명세를 확정한다

- 진행 상태: **대기**

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

### 완료 조건

- [ ] 사용자가 추측하지 않고 수행할 수 있는 Hierarchy·Component·Field·값 명세가 준비되었다.
- [ ] 변경 대상과 유지 대상이 구분되었다.
- [ ] Scene 작업 후 AI가 정적으로 판정할 체크 항목이 준비되었다.

## Step 6. 네 Pattern의 생산 Scene 또는 Prefab 구성을 수동으로 작성한다

- 진행 상태: **대기**

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

### 완료 조건

- [ ] 네 Pattern과 두 Slot이 지정된 Hierarchy에 존재한다.
- [ ] 모든 Transform, Collider, Layer, Boundary와 Serialized Reference가 입력되었다.
- [ ] 지정된 Scene 및 Prefab 외 Asset 변경이 없다.

## Step 7. 생산 Scene과 Prefab 구성을 정적으로 검사하고 수정 사항을 확정한다

- 진행 상태: **대기**

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

### 완료 조건

- [ ] 생산 구성의 모든 정적 계약이 통과한다.
- [ ] Missing 참조와 의도하지 않은 Asset 변경이 없다.
- [ ] 정적 검사로 판정 가능한 항목이 후속 수동 검증에 남아 있지 않다.

## Step 8. Pattern별 실제 물리 통과를 Play Mode Test로 검증한다

- 진행 상태: **대기**

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

### 완료 조건

- [ ] 네 Pattern을 기본 및 최대 속도 조건에서 통과한다.
- [ ] 실제 착지, Collision, Wall 접촉 해제와 Ground 복구가 검증된다.
- [ ] 사람의 Jump 타이밍 재현을 요구하지 않는다.

## Step 9. 전체 16개 연결 조합과 연속 진행을 Play Mode Test로 검증한다

- 진행 상태: **대기**

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

### 완료 조건

- [ ] 전체 16개 연결 조합이 연결 및 실제 통과 Test를 통과한다.
- [ ] 장시간 교대 진행에서 잔여 Pattern과 중복 Boundary 처리가 없다.
- [ ] 기존 Camera, Collision, Collectible Scope, Pause, Result와 Retry에 회귀가 없다.

## Step 10. 문서와 Phase 경계를 최종 정리한다

- 진행 상태: **대기**

### AI 작업

- 생산 Pattern의 실제 Hierarchy, Transform, Collider와 참조 계약을 `InfiniteMode.md`에 반영한다.
- `InfiniteModeSystem.md`와 Roadmap에 Phase 2가 소유하는 생성·연결·정리 책임과 Phase 3 입력 경계를 반영한다.
- Pattern ID, 기하 수치, Scene 값, Unit Test와 Play Mode Test가 서로 일치하는지 정적으로 검사한다.
- Phase 3 Difficulty 선택 연동과 Phase 4 Collectible·UI 구현이 섞이지 않았는지 확인한다.

### 사용자 작업

- 없음

### 완료 조건

- [ ] System, Feature, Roadmap, Scene과 Test 계약이 일치한다.
- [ ] Phase 3과 Phase 4의 후속 책임이 명확하다.
- [ ] 확정 수치와 생산 참조를 문서에서 확인할 수 있다.

## Step 11. 전체 정적 검사와 전체 자동 Test를 수행한다

- 진행 상태: **대기**

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
- Unity Editor에서 사용자가 직접 Build를 실행하고 성공 여부와 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 완료 조건

- [ ] 전체 정적 검증이 통과한다.
- [ ] 전체 Edit Mode 및 Play Mode Test가 통과한다.
- [ ] Unity Build가 성공한다.
- [ ] 예상하지 않은 Compile, Test 및 Build Error와 Warning이 없다.
- [ ] Phase 2 범위 밖 구현이 포함되지 않았다.

## Step 12. 최소 화면 확인과 Phase 2 완료 근거를 정리한다

- 진행 상태: **대기**

### AI 작업

- 기하 수치, 참조, 연결, 물리 통과와 상태 초기화는 정적 검사 및 자동 Test 결과를 사용하고 수동 체크리스트에서 제외한다.
- 자동 판정이 어려운 Pattern 식별성, Camera 화면 전환과 시각적 떨림만 최소 수동 확인 대상으로 지정한다.
- 생산 Scene 및 Asset 변경과 전체 검증 결과를 확인한다.

### 사용자 작업

- Build 또는 Unity Editor에서 AI가 지정한 고정된 Pattern 순서를 한 번 관찰한다.
- 네 Pattern이 의도한 지형 형태로 서로 구분되는지 확인한다.
- Pattern 연결 순간 Camera 또는 지형에 눈에 띄는 순간 이동, 겹침과 빈 화면이 없는지 확인한다.
- 입력 타이밍, 수치, 전체 16개 조합과 장시간 반복을 수동으로 재검증하지 않는다.
- 화면 확인 중 Console에 예상하지 않은 Error 및 Warning이 없는지 확인한다.

### 화면 검증 후 AI 작업

- 정적 검사, Compile, 전체 Test, Build와 최소 화면 검증 결과를 기록한다.
- Asset 및 Scene 변경과 미해결 사항을 기록한다.
- 별도 Prototype 4 Phase 2 Verification Result Task 문서를 작성한다.
- 모든 완료 조건을 충족한 경우에만 Roadmap Phase 2를 `완료`로 변경한다.

### 완료 조건

- [ ] 네 Pattern 제작, 연결, 생성·정리와 실제 통과가 검증되었다.
- [ ] 자동 판정 가능한 항목이 수동 결과에 의존하지 않는다.
- [ ] 필요한 정적, Compile, Test, Build와 최소 화면 결과가 기록되었다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 수동 작업 요약

사용자가 직접 수행하는 작업은 아래 항목으로 제한한다.

1. Step 1에서 Pattern Authoring 구조를 결정한다.
2. 코드 변경 Step마다 Unity Script Compilation과 AI가 지정한 관련 Test를 실행한다.
3. Step 6에서 AI가 확정한 값과 참조대로 생산 Scene 또는 Prefab을 편집하고 저장한다.
4. Step 7에서 정적 오류가 발견된 경우에만 지정된 Scene 또는 Prefab 값을 수정한다.
5. Step 11에서 전체 Edit Mode Test, 전체 Play Mode Test와 Unity Build를 실행한다.
6. Step 12에서 네 Pattern 식별성과 연결 순간의 화면 이상만 한 번 확인한다.

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
