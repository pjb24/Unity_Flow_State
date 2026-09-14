# 작업 정보

## 작업명

Prototype 4 Phase 4 수동 작업 및 검증 계획

## 작업 일자

20260914

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료 · 구현 대기

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

- [ ] 생산 구조와 필수 Asset·UI 편집 범위가 명확하다.
- [ ] 확정되지 않은 수치와 UI 정책을 Inspector 설정이나 임의의 배치로 대체하지 않는다.

## Step 2. Collectible 배치표와 Unit Test를 확정한다

### AI 작업

- 네 Pattern 각각의 내부 Jump 구간과 모든 연결 경계에 대해 도약 전·공중·착지 안내 후보를 계산한다. `ScoreCollectible.md`의 기본 5개 묶음을 출발점으로 사용하되 유효 입력 구간과 실제 통과 가능성을 기준으로 수량을 조정한다.
- 각 Prefab의 `CollectibleRoot` 하위에 만들 오브젝트의 이름, Pattern 내부 고유 ID, Local Position, SphereCollider 반지름·Layer·Renderer 조건을 표로 문서화한다. 첫 `Flat` 정지 출발, `Flat` 대체와 네 Pattern의 모든 앞뒤 연결을 포함한다.
- 생산 이동·Jump 계산과 후보 배치를 사용하는 Edit Mode Unit Test로 경계·도달 가능성·중복 ID·누락 경로·비정상 값을 검증한다. 좌표를 Test 코드에 복제한 뒤 같은 값을 서로 비교하는 형식은 피한다.

### 사용자 수동 작업

- 없음. 아직 Prefab을 편집하지 않는다.

### 완료 조건

- [ ] 모든 Pattern 및 연결 경계의 배치표가 수치와 ID까지 확정됐다.
- [ ] 배치·통과 계산 관련 Unit Test가 준비됐고 정적 검사를 통과했다. 실행 결과는 Step 4에서 확인한다.

## Step 3. 네 Pattern Prefab의 Collectible을 제작한다

### AI 작업

- Step 2의 확정 표를 `InfinitePattern_Flat.prefab`, `InfinitePattern_SingleRise.prefab`, `InfinitePattern_LegacySteps.prefab`, `InfinitePattern_InternalGap.prefab`별 Editor 작업표로 제공한다. 각 행에 부모 `CollectibleRoot`, 이름, ID, Local Position, SphereCollider 값, Trigger·Layer, `ScoreCollectible` 필드 할당 대상을 명시한다.
- 기존 Stage Collectible의 형태·색상·Player 판정 구성을 재사용할 수 있는지 확인하고, 달라야 하는 설정만 명시한다.

### 사용자 수동 작업

- Unity Editor에서 네 Prefab을 각각 열고 확정된 작업표대로 `CollectibleRoot` 하위에 Collectible 오브젝트를 만든다. 각 오브젝트에 Renderer, `SphereCollider`의 **Is Trigger**, `ScoreCollectible`을 설정하고 ID·Collider·Renderer·Player Layer 참조를 할당한 뒤 Prefab을 저장한다.
- 좌표·개수·반지름을 눈대중으로 조정하지 않는다. 연결 경계의 Collectible이 어느 Pattern 원본에 속하는지도 작업표에 따른다.
- 저장 후 Unity Script Compilation 성공과 예상하지 않은 Error·Warning 여부만 전달한다.

### 완료 조건

- [ ] 네 Prefab이 확정된 작업표대로 저장됐다.
- [ ] AI의 Prefab YAML·GUID·Layer·Component·ID·좌표 정적 검사와 Unity Script Compilation 결과가 확인됐다. 물리 Play Mode Test는 Step 4에서 확인한다.

## Step 4. 획득·연결·Score 생명주기를 자동 Test로 검증한다

### AI 작업

- 빈 Root를 전제한 기존 Phase 2 Test를 현재 배치 계약에 맞게 바꾸고, 생산 Prefab을 그대로 로드하는 Test에서 네 Pattern 및 연결 경계의 Trigger·획득·재배치 후 Scope 복구를 확인한다.
- Collectible을 전부 놓쳐도 진행하는 경로, Pause·Resume·Result·Retry의 획득 상태와 Score 보존·초기화, 중복 획득·상한 포화는 Edit Mode와 Play Mode로 나눠 검증한다.
- Pattern 전환 전후 Distance Score, Collectible Score와 Total Score가 누적되고 Pattern 통과 횟수가 Score에 더해지지 않음을 생산 Score·Result 경로에서 확인한다.

### 사용자 수동 작업

- AI가 지정한 관련 Edit Mode 및 Play Mode Test를 Unity Test Runner에서 실행한다. 클래스별 Passed/Failed 수, 실패 Test명·메시지·Stack Trace와 예상하지 않은 Error·Warning을 전달한다.

### 완료 조건

- [ ] 배치·물리 획득·Score·생명주기 관련 Test가 모두 통과한다.
- [ ] 실패 또는 예상하지 않은 Error·Warning을 수정·재검증 전까지 완료로 처리하지 않는다.

## Step 5. Difficulty 상태 UI를 생산 HUD에 연결한다

### AI 작업

- 현재 `InfiniteHUD`의 기존 Distance·Score 텍스트를 재사용할 수 있는지, Difficulty 표시를 위한 TMP 텍스트가 별도로 필요한지 정적 검사로 확정한다. 표시 형식과 D1/D2/D3 갱신·Pause·Result·Retry 계약을 문서화한다.
- 필요한 Runtime UI 바인딩과 문자열 생성·상태 Unit Test를 작성한다. 기존 Distance·Collectible·Total Score 표시와 Result Content는 유지한다.
- 새 UI 오브젝트나 직렬화 참조가 실제로 필요할 때만 정확한 Scene 경로, TMP 컴포넌트, 표시 문자열, RectTransform·Style 기준과 `UIManagementSystem` 할당 필드를 작업표로 제공한다.

### 사용자 수동 작업

- AI 작업표에서 새 UI 오브젝트가 필요하다고 확정한 경우에만 Unity Editor의 `SampleScene.unity`에서 지정한 `InfiniteHUD` 하위 TMP 텍스트를 만들고, 지정한 `UIManagementSystem` 필드에 할당해 Scene을 저장한다. 기존 오브젝트 재사용으로 해결되면 이 편집은 건너뛴다.
- 저장 후 Unity Script Compilation과 AI가 지정한 UI Play Mode Test 결과를 전달한다. D2/D3 거리 경계를 손으로 달리며 판정하지 않는다.

### 완료 조건

- [ ] 생산 UI에 현재 Difficulty 또는 확정된 진행 상태가 표시되고 자동 Test로 상태 전환이 검증됐다.
- [ ] 필요한 직렬화 참조가 YAML·GUID·Play Mode Test에서 확인됐다. 편집이 필요 없다면 그 근거를 기록한다.

## Step 6. 전체 회귀와 최소 화면 확인을 수행한다

### AI 작업

- 변경 파일·문서·코드·Prefab·Scene의 일관성, LINQ 부재, `git diff --check`와 무관한 설정 변경 부재를 검사한다.
- 관련 Test 통과 후 전체 Edit Mode와 Play Mode Test를 각각 한 번 요청한다. Stage Mode의 자동 이동·Collectible·Clear Time·Score와 InfiniteMode의 Pattern 진행·Score·UI·Pause·Result·Retry를 포함한다.
- 결정적 Pattern·Collectible·Difficulty 화면 확인 경로를 Test나 재현 가능한 시작 설정으로 제공한다. 무작위 플레이로 원하는 Pattern 출현을 기다리게 하지 않는다.

### 사용자 수동 작업

- Unity Editor에서 Script Compilation과 전체 Edit Mode·Play Mode Test를 실행하고 결과 및 예상하지 않은 Error·Warning 여부를 전달한다.
- 자동 Test 통과 후 AI가 지정한 결정적 화면 경로를 한 번 실행해 Collectible 안내의 눈에 띄는 끊김·겹침, Jump 유도 가독성, Difficulty UI 가독성, Camera·전환 표현만 관찰한다. 개수·좌표·점수·Difficulty 경계는 화면으로 판정하지 않는다.
- Phase 4에서는 Build를 수행하지 않는다. 검증 결과에서 Build를 범위 제외·미검증으로 기록한다.

### 완료 조건

- [ ] 정적 검사, Compile, 관련·전체 Test 및 화면 관찰 결과가 기록됐다.
- [ ] Stage Mode 회귀와 Phase 4 전체 흐름의 자동 검증이 통과했다.
- [ ] Build 제외·미검증 상태가 Phase 4 결과에 기록됐다.

## Step 7. 결과와 Phase 경계를 기록한다

### AI 작업

- Phase 4 검증 결과 Task 문서에 구현, Test 수, 화면 관찰, Build 제외·미검증 상태와 미해결 사항을 구분해 기록한다.
- `InfiniteMode.md`, `ScoreCollectible.md`, `ScoreRecord.md`, `UIManagementSystem.md` 및 Roadmap을 실제 구현에 맞춰 갱신한다.
- Phase 4의 Build 제외 범위에서 완료 조건을 모두 확인했을 때만 Phase 4 상태를 완료로 바꾼다. Prototype 4 전체 완료 여부는 이 단계에서 판정하지 않는다.

### 사용자 수동 작업

- 없음. 새 실패가 발견되면 해당 Test 또는 화면 확인 결과를 제공한다.

### 완료 조건

- [ ] 확인하지 않은 기능이나 Build를 통과로 기록하지 않는다.
- [ ] 검증 결과와 Roadmap 상태가 일치한다.

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

- 이 문서는 실행 계획이다. 작성 시점에 Phase 4 Prefab·Scene·Runtime·Test 변경, Unity Script Compilation, Test Runner 또는 수동 화면 확인은 수행하지 않았다. Build는 Phase 4 범위에서 제외한다.
- 각 Step 완료 시 실제 결과를 기록한다. Test 실패는 이름·메시지·Stack Trace와 함께 남긴다.

# 검증 결과

- 계획 문서 작성 완료. Phase 4 구현·검증은 아직 시작하지 않았다.

# 후속 작업

- Step 1에서 미정 배치·UI 정책과 실제 편집 범위를 확정한 후 순서대로 수행한다.

# 관련 문서

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_004.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreCollectible.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/90_Tasks/Prototype_4/20260914_02_Phase3VerificationResult.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
