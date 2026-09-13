# 작업 정보

## 작업명

Prototype 4 Phase 3 수동 작업 및 검증 계획

## 작업 일자

20260914

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료 · 구현 대기

---

# 작업 목적

Phase 2의 명시적 Pattern 요청 구조에 진행도·Difficulty·Pattern 자동 선택을 연결한다. 사용자가 실제로 수행해야 할 Unity Editor 작업만 Step별로 남기고, 코드·직렬화 구조·상태 규칙은 정적 검사와 자동 Test로 판정한다.

# 작업 대상

- `InfiniteModeSystem`의 Run 진행도와 Pattern 요청 연결
- `InfiniteDistanceState`, `InfiniteDifficultyState`, `InfinitePatternCatalog`, `InfinitePatternSelectionState`의 기존 계약 활용
- `InfiniteMapPattern.TryRequestNextPattern(requestId, patternId)`와 Boundary 진행 연결
- 관련 Edit Mode·Play Mode Test, 생산 Scene 참조와 최종 화면 확인

# 작업 전 상태

- Phase 2는 네 Pattern Prefab과 두 Slot, 명시적 Pattern ID 요청 및 Boundary 물리 진행을 구현했다.
- 생산 Scene의 두 Slot은 `Flat`으로 시작한다. 일반 플레이에서 자동으로 다음 Pattern을 선택·요청하는 생산 호출자는 아직 없다.
- Phase 1의 Difficulty 경계, 후보·반복·대체·Seed·초기화 규칙은 `InfiniteMode.md`에 확정돼 있다.
- Phase 2 검증은 Build를 사용자 결정으로 제외한 상태에서 완료됐다. Phase 3의 Build 수행 여부는 별도 검증 계획에서 결정하며, 이 문서가 Build 성공을 전제하지 않는다.

# 조사 내용

- `IMPLEMENTATION_ROADMAP_004.md`의 Phase 3 목표·완료 조건·검증 책임을 확인했다.
- `InfiniteMode.md`의 D1/D2/D3 경계(`0`, `220`, `440`), Pattern 허용 범위, 같은 Pattern ID의 최대 2회 연속 선택, 연결 가능한 `Flat` 대체, Run별 새 Seed와 중복 요청 처리 규칙을 확인했다.
- `InfiniteModeSystem.md`는 최대 전진 거리와 Difficulty·선택 상태의 Run 생명주기를 `InfiniteModeSystem` 책임으로 두고, 실제 선택 규칙은 Feature에 둔다.
- Phase 2의 화면 확인 Test는 `Flat → Flat`이므로 서로 다른 Pattern의 일반 플레이 연속 출현을 증명하지 않는다.

# 작업 원칙

- AI가 코드·Test·문서·Scene YAML을 정적으로 검사하고 생산 계약의 유효값, 참조, 누락 여부를 먼저 확인한다. 수치나 상태를 사람이 Inspector에서 읽어 판정하도록 요구하지 않는다.
- 계산, 후보 선택, Seed 재현성, 요청 ID, Pause·Result·Retry 상태는 Edit Mode Unit Test를 우선한다. 실제 Scene 실행 순서, Trigger, Pattern 요청과 교체, 게임 종료·재시작은 Play Mode Test로 확인한다.
- Test는 생산 선택·진행 코드를 호출한다. Test 전용 선택 알고리즘을 복제하거나 확률적인 결과 한 번으로 전체 허용 범위를 판정하지 않는다.
- 정확한 Boundary 통과 타이밍, D2/D3 거리 경계 도달, 빠른 Pause/Resume, 다수 Pattern 반복을 수동 조작으로 재현하게 하지 않는다.
- Scene·Prefab 수동 편집은 정적 검사 결과 현재 직렬화 구성만으로 연결할 수 없는 경우에만 별도 Step에서 정확한 대상·필드·값을 제시한 후 수행한다. 필요 없으면 편집 Step을 건너뛴다.
- AI는 Unity Editor의 Build 또는 Unity Test Runner를 대신 실행하지 않는다. 사용자는 지정된 Test를 Unity Test Runner에서 실행하고 결과를 전달한다.
- AI는 Scene 관련 편집을 직접 수행하지 않고 방법을 안내한다. 관련 Runtime 및 Test에는 LINQ를 사용하지 않는다.

# 수행 Step

## Step 1. Phase 3 연결 지점과 수동 편집 필요성을 확정한다

### AI 작업

- Phase 1 규칙, Phase 2 `InfiniteMapPattern` 요청·Boundary 계약, `InfiniteModeSystem`의 시작·업데이트·Pause·Result·Retry 흐름을 대조한다.
- 생산 Scene의 기존 참조와 Component만으로 연결 가능한지 Scene YAML, `.meta`, GUID와 코드 경로를 정적으로 검사한다.
- Seed 생성 위치, 요청 ID 생성·중복 방지, Boundary 전에 다음 Pattern을 준비할 시점, 요청 실패 시 선택 상태 보존 방식을 기존 계약과 대조한다. 문서로 확정되지 않은 의미 있는 선택이 있으면 구현 전에 사용자 결정을 받는다.

### 사용자 수동 작업

- 없음. 이 단계의 판정은 문서·코드·직렬화 파일 검사로 처리한다.

### 완료 조건

- [ ] 생산 연결 경로와 Scene·Prefab 수동 편집 필요 여부가 명확하다.
- [ ] 미정 정책을 임의의 Inspector 설정이나 수동 플레이 결과로 대신하지 않는다.

## Step 2. Difficulty와 Pattern 선택 상태를 생산 Run에 연결한다

### AI 작업

- 최대 전진 거리로 D1/D2/D3를 갱신하되, Pattern 통과 횟수나 Score를 Difficulty 입력으로 사용하지 않는다.
- 기존 Catalog·선택 상태를 사용해 현재 Difficulty, 연결 가능성, 반복 제한 순으로 후보를 거르고, 선택 결과를 명시적 Pattern 요청 API에 전달한다.
- 현재 두 Slot의 초기 `Flat`과 Run의 첫 `Flat` 선택 이력 사이의 관계를 확정된 계약대로 처리한다. Player 이동 수치는 변경하지 않는다.
- 신규·변경 Runtime 코드에 LINQ가 없는지 정적으로 검사한다.

### 사용자 수동 작업

- 코드 반영 후 Unity Editor에서 Script Compilation 결과와 예상하지 않은 Error·Warning 여부만 확인해 전달한다. 수치·후보 목록을 Inspector에서 수동 확인하지 않는다.

### 완료 조건

- [ ] 일반 플레이의 진행 경로에서 선택된 Pattern ID가 요청 API에 도달한다.
- [ ] Script Compilation 성공과 예상하지 않은 Error·Warning 부재가 확인된다.

## Step 3. Difficulty·선택 규칙을 Edit Mode Unit Test로 검증한다

### AI 작업

- D1/D2/D3의 양쪽 경계와 한 번에 여러 경계를 넘는 입력, 역행·음수·NaN·무한대 입력을 Test로 검증한다.
- Difficulty별 허용 후보, 연결 불가 조합 배제, 같은 Pattern ID 2회 반복 제한, 일반 후보가 없을 때만 가능한 `Flat` 대체와 대체 불가능 설정 거부를 검증한다.
- 같은 Catalog·Seed·요청 순서의 재현성, 중복 요청 시 선택 이력·난수 상태 보존, 새 Run·Retry의 초기화와 Pause·Resume·Result 상태를 검증한다.
- 무작위 선택의 특정 1회 결과에 의존하지 않고 유효 후보 불변식과 Seed 재현성을 검증한다.

### 사용자 수동 작업

- AI가 지정한 관련 Edit Mode Test를 Unity Test Runner에서 실행하고 Passed/Failed 수, 실패 Test명·메시지·Stack Trace 및 예상하지 않은 Error·Warning을 전달한다.

### 완료 조건

- [ ] 관련 Edit Mode Test가 모두 통과한다.
- [ ] 실패 또는 예상하지 않은 Error·Warning은 수정·재검증 전까지 완료로 처리하지 않는다.

## Step 4. 생산 Scene의 Pattern 진행·생명주기를 Play Mode Test로 검증한다

### AI 작업

- 실제 생산 Scene에서 Boundary 통과 전 다음 Pattern 요청, 물리 Trigger 후 단 한 번의 진행, Pattern ID·Slot 교체, 연결 가능성과 진행 요청 순서를 검증한다.
- Pause 중 요청 거부와 Resume 후 보존, Result 중 추가 진행 거부, Retry·새 Run에서 첫 `Flat`·Difficulty·선택 이력·요청 상태 초기화를 검증한다.
- 서로 다른 Pattern ID가 실제로 등장하는 결정적 Seed 또는 제어된 선택 입력의 Test를 구성하되, Test 전용 Pattern 선택 알고리즘은 만들지 않는다.
- 기존 Pattern 연결·통과·Collectible Scope·Score·Stage 회귀 영향 범위를 확인한다.

### 사용자 수동 작업

- AI가 지정한 관련 Play Mode Test를 Unity Test Runner에서 실행하고 Passed/Failed 수, 실패 정보 및 예상하지 않은 Error·Warning을 전달한다.
- 사람의 이동 타이밍, 특정 거리 도달 또는 여러 번의 Boundary 통과로 상태를 수동 판정하지 않는다.

### 완료 조건

- [ ] 생산 Scene의 자동 요청부터 물리 진행까지 관련 Play Mode Test가 통과한다.
- [ ] Pause·Result·Retry 및 영향받는 기존 기능의 회귀 Test가 통과한다.

## Step 5. 필요한 경우에만 Scene·Prefab 직렬화 참조를 편집한다

### AI 작업

- Step 1~4의 정적 검사·Test로 누락된 직렬화 참조나 Component가 실제로 확인된 경우에만 편집할 Scene·GameObject·Component·필드·할당 대상을 정확하게 제시한다.
- 편집 후 Scene YAML과 Prefab, `.meta`, GUID, Layer, 참조 및 변경 파일 범위를 다시 정적으로 확인한다.

### 사용자 수동 작업

- AI가 구체적으로 지정한 항목이 있을 때에만 Unity Editor에서 해당 Scene 또는 Prefab을 열어 안내한 참조를 할당하고 저장한다. 지정된 변경이 없으면 이 Step은 건너뛴다.
- 편집을 수행했다면 Unity Script Compilation 결과와 관련 Play Mode Test 결과를 다시 전달한다.

### 완료 조건

- [ ] 필요했던 직렬화 참조가 정적으로 확인되고 관련 Test가 다시 통과한다. 편집이 필요 없었다면 그 근거를 기록한다.
- [ ] 무관한 Scene, Package, Input Action, ProjectSettings 변경이 없다.

## Step 6. 전체 회귀와 최소 화면 확인을 수행한다

### AI 작업

- 변경 파일·문서·코드의 정적 일관성, LINQ 부재, `git diff --check` 및 회귀 범위를 확인한다.
- 관련 Test가 통과한 다음 전체 Edit Mode·Play Mode Test를 각각 한 번 요청한다. 실패 시 원인 수정 후 영향받는 범위와 전체 회귀를 다시 확인한다.
- 서로 다른 Pattern이 반드시 등장하도록 생산 선택 규칙을 사용한 결정적 화면 확인 경로를 Test 또는 재현 가능한 시작 설정으로 제공하고, 관찰 범위를 생산 Scene 상태에 맞게 안내한다. 무작위 일반 플레이에서 우연히 특정 Pattern이 나오길 기다리게 하지 않는다.

### 사용자 수동 작업

- Unity Script Compilation과 전체 Edit Mode·Play Mode Test를 Unity Editor에서 실행하고 결과 및 예상하지 않은 Error·Warning 여부를 전달한다.
- 자동 Test 통과 후 AI가 지정한 결정적 화면 확인 경로를 한 번 실행하여 서로 다른 Pattern이 이어질 때 Camera의 눈에 띄는 순간 이동·떨림, 지형 겹침·빈 화면, 전환 연출의 부자연스러움만 확인한다. Pattern 출현 순서·Difficulty 경계·반복 횟수는 화면 관찰로 판정하지 않는다.
- 이번 Phase의 Build 수행 여부는 Phase 3 최종 검증 전에 사용자와 결정한다. Build를 제외하면 미검증으로 명시하고 성공을 주장하지 않는다.

### 완료 조건

- [ ] 정적 검사, Compile, 관련 및 전체 Test 결과가 기록된다.
- [ ] 자동 판정이 적절하지 않은 화면 항목만 수동으로 확인된다.
- [ ] Build 수행 여부와 그에 따른 검증 범위가 명확히 기록된다.

## Step 7. 결과와 Phase 경계를 기록한다

### AI 작업

- Phase 3 검증 결과 Task 문서에 구현, 테스트 수, 화면 관찰, Build 상태, 미검증·미해결 사항을 구분해 기록한다.
- `InfiniteModeSystem.md`, `InfiniteMode.md`, Roadmap이 실제 구현과 일치하는지 확인한다.
- 모든 Phase 3 완료 조건이 확인된 경우에만 Roadmap 상태를 완료로 바꾼다. Pattern별 Collectible 경로·UI는 Phase 4로 남긴다.

### 사용자 수동 작업

- 없음. 추가 실패가 발견되면 해당 Step의 검증 결과를 제공한다.

### 완료 조건

- [ ] 확인하지 않은 기능을 통과로 기록하지 않는다.
- [ ] Phase 3 상태와 검증 기록 및 Roadmap이 일치한다.

---

# 수동 작업 요약

- Unity Script Compilation 결과 확인
- AI가 지정한 관련 Edit Mode·Play Mode Test 및 최종 전체 Test 실행과 결과 전달
- 정적 검사로 실제 필요성이 확인된 경우에만 지정된 Scene·Prefab 참조 편집
- 자동 Test 통과 후 서로 다른 Pattern 연결의 최소 화면 확인
- Phase 3 Build 수행 여부 결정 및 수행 시 결과 전달

수치, 후보 적합성, 난수 재현성, 요청 중복, 정확한 입력 시점, 모든 Pattern 조합과 Run 상태 전환은 수동 체크리스트에 포함하지 않는다.

# 영향 범위

- System: `InfiniteModeSystem`의 Phase 3 Run 상태·선택 연결
- Feature: `InfiniteMode`의 Difficulty·Pattern 선택 생산 연동
- Task: Phase 3 구현·검증 순서와 사용자 수동 작업

# 검증 내용

- 이 문서는 구현 결과가 아니라 실행 계획이다. 작성 시점에는 Phase 3 Runtime 변경, Unity Script Compilation, Test Runner, Build 또는 수동 화면 확인을 수행하지 않았다.
- 각 Step 완료 시 실제 확인 결과를 기록하고, 실패는 이름·메시지·Stack Trace와 함께 남긴다.

# 검증 결과

- 계획 문서 작성 완료. Phase 3 구현·검증은 아직 시작하지 않았다.

# 후속 작업

- Step 1에서 연결 지점과 미정 정책을 확인한 후 순서대로 수행한다.

# 관련 문서

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_004.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/90_Tasks/Prototype_4/20260910_02_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_4/20260911_02_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_4/20260913_01_Phase2VerificationResult.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
