# 작업 정보

## 작업명

Prototype 3 Phase 3 Manual Steps

## 작업 일자

20260907

## 작업 담당자

AI, 사용자

## 작업 상태

작업 준비 완료

---

# 작업 목적

Prototype 3 Phase 3의 Score Collectible을 Stage Mode와 InfiniteMode에 구현하고 검증하는 순서를 정의한다.

실질적인 사용자 작업을 규칙 결정, Unity Script Compilation 및 Test Runner 실행, 필요한 Scene 배치, Build와 최소 화면 검증으로 제한한다.

계산, 상태 전환, 중복 획득 방지, Run 초기화, 점수 합산과 회귀는 정적 검증 및 Unit Test로 판정한다.

---

# 작업 대상

- Score Collectible 규칙과 책임
- Collectible 획득 및 같은 Run의 중복 획득 방지
- Run별 Collectible 상태 및 Score 초기화
- Stage Mode Collectible 배치
- InfiniteMode Map Pattern Collectible 배치와 Pattern 재사용
- 점프 시작, 공중 이동 경로와 착지 지점 안내
- Collectible을 놓친 경우의 Stage Play 지속
- InfiniteMode 거리 Score 보존
- Pause, Result와 Retry 회귀
- Compile, 전체 Test, Build와 최소 화면 검증

---

# 작업 전 상태

- Prototype 3 Phase 2가 완료되었다.
- Stage Mode와 InfiniteMode에서 Player가 입력 없이 오른쪽으로 자동 이동한다.
- Jump와 Momentum Landing 입력이 유지되고 Player 좌우 이동 입력은 플레이에 영향을 주지 않는다.
- InfiniteMode에는 이동 거리 기반 ScoreCalculator, Runtime Score와 ScoreRecord가 존재한다.
- Stage Mode 결과는 Clear Time을 사용하고 InfiniteMode 결과는 최종 이동 거리와 최종 Score를 사용한다.
- InfiniteMode Map Pattern은 두 Pattern을 재배치하여 반복 사용한다.
- Score Collectible 생산 코드와 Scene 배치는 아직 없다.
- Phase 4에서 Mode별 Collectible Score 표시와 Total Score UI를 통합한다.
- 최근 테스트 정리 후 정적 Test 수는 Edit Mode 285개, Play Mode 144개이며 변경 후 Unity 검증은 아직 수행하지 않았다.

---

# 조사 내용

- Roadmap Phase 3는 두 Mode 획득, 한 Run에서 한 번만 획득, Retry 초기화와 경로 안내를 요구한다.
- Collectible Score는 두 Mode에 필요하지만 기존 InfiniteMode 거리 Score를 대체하면 안 된다.
- Collectible 획득 상태와 Score는 Run에 속하므로 정적 Scene Object가 영구 상태를 소유해서는 안 된다.
- InfiniteMode Pattern Object는 같은 Run에서도 재사용되므로 단순 GameObject 비활성화만으로 획득 여부를 관리하면 다음 Pattern 순환에서 복구 시점을 구분하기 어렵다.
- Phase 3에서는 획득과 Runtime Score를 구현하고, HUD와 Result의 Mode별 Score 통합은 Phase 4에 남겨야 한다.
- Collectible 위치의 도달 가능성은 이동 및 Jump 수치로 자동 검증할 수 있고, 안내성 및 화면 가독성만 수동으로 확인해야 한다.
- Scene과 Input Asset 변경은 코드 및 YAML 정적 검사로 필요성이 확인된 경우에만 사용자 작업으로 요청한다.

---

# 작업 원칙

- 각 구현 Step은 실패하는 관련 Unit Test를 먼저 작성하거나 기존 Test의 명시적 실패 근거를 확보한 뒤 생산 코드를 변경한다.
- 수치 계산, 상태, 획득 횟수, Score, 초기화, Pattern 재사용과 입력 타이밍은 Edit Mode 또는 Play Mode Test로 판정한다.
- 생산 Scene 참조는 YAML과 `.meta` GUID로 먼저 검사하고, Editor에서만 가능한 배치 및 저장만 사용자에게 요청한다.
- 사용자는 Unity Editor에서 Script Compilation과 Test Runner를 실행한다. AI는 Unity Editor, Test Runner와 Build를 실행하지 않는다.
- AI는 Scene을 직접 수정하지 않는다. Scene 변경이 필요하면 Hierarchy, Component, Field와 값 단위의 절차를 제공한다.
- Test가 실패하면 실패 이름, 메시지와 Stack Trace를 근거로 수정하고 관련 Test를 다시 실행한다.
- 모든 관련 Test가 통과하기 전에는 다음 책임의 구현으로 넘어가지 않는다.
- Build와 수동 화면 확인은 전체 정적 검증 및 전체 Test 통과 후 한 번 수행한다.

---

# 작업 Step

## Step 1. Phase 3 기준선을 확정한다

- 진행 상태: **대기**

### AI 작업

- 최근 Test 정리에서 제거한 3개 Play Mode Test의 계약이 더 강한 기존 Test에 포함되는지 재확인한다.
- Edit Mode 285개와 Play Mode 144개의 정적 수를 확인한다.
- Ignore, Explicit, 임의 성공, 조건부 제외, 중복 GUID와 누락 `.meta`를 검사한다.
- Runtime, Scene, Input Action Asset과 Package가 Phase 2 완료 시점 이후 의도하지 않게 변경되지 않았는지 확인한다.

### 사용자 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
2. 전체 Play Mode Test 144개를 실행하고 모두 성공하는지 확인한다.

Edit Mode 생산 코드와 Test는 최근 기준선 이후 변경되지 않았으므로 이 Step에서 재실행하지 않는다. 정적 검사에서 영향이 발견되면 AI가 전체 Edit Mode 실행 필요성을 별도로 명시한다.

### 완료 조건

- [ ] Phase 3 시작 전 정적 기준선이 확인되었다.
- [ ] Unity Script Compilation이 통과한다.
- [ ] 전체 Play Mode 144개가 통과한다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

## Step 2. 미정 Collectible 규칙을 결정한다

- 진행 상태: **대기**

### AI 작업

- 아래 결정 항목마다 구현 가능한 제안, 권장안과 장단점을 채팅으로 제시한다.
- 사용자가 선택하기 전에는 수치나 소유 구조를 추측하여 생산 코드에 반영하지 않는다.
- 결정 결과를 관련 System 및 Feature 문서와 이 Task에 반영한다.

### 결정 항목

1. Collectible 한 개의 Score와 모든 Collectible의 동일 값 사용 여부
2. Stage Mode Collectible Score와 InfiniteMode Collectible Score의 Runtime 소유 위치
3. InfiniteMode 기존 거리 Score와 Collectible Score의 분리 및 Phase 3 임시 합산 범위
4. Collectible 고유 식별 방식과 같은 Run 중복 획득 판정
5. Retry, 새 Run과 Infinite Pattern 재사용 시 Collectible 복구 규칙
6. Pause, Ending과 Ended 상태에서 Trigger가 들어온 경우 처리 규칙
7. Player 판정 방식과 Layer, Tag 또는 Component 기준
8. 획득 후 표시 상태와 Phase 3에서 허용할 최소 시각 표현
9. Stage 및 Pattern별 Collectible 개수와 안내 배치 원칙
10. 획득하지 못한 Collectible의 처리와 Stage 진행 영향 없음 규칙

### 사용자 작업

AI가 제시한 항목별 권장안 또는 대안을 선택한다.

### 완료 조건

- [ ] 구현에 필요한 모든 Collectible 규칙이 확정되었다.
- [ ] Phase 3와 Phase 4의 Score 및 UI 경계가 확정되었다.
- [ ] 관련 문서가 결정 결과와 일치한다.

## Step 3. 기존 Score와 생명주기 경로를 정적으로 조사한다

- 진행 상태: **대기**

### AI 작업

- GameRuntimeData, InfiniteModeRuntimeData, ScoreCalculator, ScoreRecord, ResultData와 ResultSystem의 Score 흐름을 추적한다.
- GameSystem의 Start, Pause, Resume, End와 Retry 호출 순서를 조사한다.
- StageSystem, InfiniteMapPattern과 InfinitePatternBoundary의 초기화 및 재사용 경로를 조사한다.
- Stage 및 Infinite Root, Player Collider와 Layer, Pattern Anchor의 생산 Scene 참조를 YAML로 검사한다.
- 새 책임이 필요한 위치와 기존 구조를 확장할 위치를 확정한다.
- 필요한 Edit Mode 및 Play Mode Test 목록과 Scene 변경 후보를 작성한다.

### 사용자 작업

없음. 코드, Asset과 Scene의 읽기 전용 정적 검사로 처리한다.

### 완료 조건

- [ ] 입력부터 Trigger, Runtime Data와 Result까지의 경로가 확인되었다.
- [ ] Pattern 재사용과 Run 초기화 경로가 확인되었다.
- [ ] Test 우선 변경 지점과 조건부 Scene 작업이 확정되었다.

## Step 4. Collectible 순수 상태와 Score 규칙을 Test 우선으로 구현한다

- 진행 상태: **대기**

### AI 작업

- Collectible ID 등록, 한 번 획득, 중복 거부와 Reset을 Scene 비의존 Edit Mode Test로 먼저 작성한다.
- 0개, 첫 획득, 여러 개, 중복 ID, 잘못된 ID, 음수 또는 비정상 설정과 정수 상한 등 확정 규칙의 경계값을 검증한다.
- Stage와 InfiniteMode의 Collectible Score 분리 및 기존 거리 Score 보존 계산을 Edit Mode Test로 검증한다.
- 동일 계산을 Test에 복제하지 않고 입력과 관찰 가능한 결과를 검증한다.
- 실패 Test에 필요한 최소 Runtime Data 또는 Feature만 구현한다.

### 사용자 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
2. AI가 지정한 신규 및 영향받는 Edit Mode Test를 실행한다.

### 완료 조건

- [ ] Collectible은 같은 Run에서 한 번만 획득된다.
- [ ] 잘못된 요청과 중복 요청이 Score를 변경하지 않는다.
- [ ] Reset 후 다음 Run의 독립 상태가 생성된다.
- [ ] 기존 InfiniteMode 거리 Score가 Collectible Score와 독립적으로 유지된다.

## Step 5. Collectible Trigger와 Player 판정을 Test 우선으로 구현한다

- 진행 상태: **대기**

### AI 작업

- 실제 Collider와 Trigger를 사용하는 Play Mode Test를 먼저 작성한다.
- Player 진입 1회 획득, 중복 Trigger 거부, 비 Player 무시와 비활성 상태 무시를 검증한다.
- 여러 Collider를 가진 Player가 진입해도 한 번만 획득되는지 검증한다.
- Pause, Ending과 Ended 상태에서 Score가 변경되지 않는지 검증한다.
- Callback 등록 및 해제와 Object 파괴 후 잔여 호출 부재를 검증한다.
- 실패 Test를 통과시키는 최소 Collectible Component 및 연결 책임을 구현한다.

### 사용자 작업

1. Unity Script Compilation을 확인한다.
2. AI가 지정한 관련 Play Mode Test를 실행한다.

### 완료 조건

- [ ] 실제 Trigger 경로에서 Player만 Collectible을 획득한다.
- [ ] 한 Run의 중복 획득 및 다중 Collider 중복 Score가 차단된다.
- [ ] 플레이 불가 상태에서는 획득되지 않는다.
- [ ] 생성 및 파괴 후 Callback 잔류가 없다.

## Step 6. Mode 공통 Runtime Data와 Run 생명주기를 통합한다

- 진행 상태: **대기**

### AI 작업

- Stage Mode와 InfiniteMode가 같은 Collectible 획득 원칙을 사용하도록 GameRuntimeData와 System 경계를 연결한다.
- Pause 전후 획득 상태 및 Score 보존, Result 이후 불변, Retry와 연속 Run 초기화를 Play Mode Test로 먼저 검증한다.
- Stage와 InfiniteMode 전환 시 이전 Mode의 Collectible 상태가 남지 않는지 검증한다.
- InfiniteMode 거리, 거리 Score와 종료 기록이 기존 계약을 유지하는지 검증한다.
- Phase 4의 HUD 및 Result 표시 책임을 조기 구현하지 않는다.

### 사용자 작업

1. Unity Script Compilation을 확인한다.
2. AI가 지정한 관련 Edit Mode 및 Play Mode Test를 실행한다.

### 완료 조건

- [ ] 두 Mode의 Collectible 상태와 Score가 Run 단위로 관리된다.
- [ ] Pause와 Resume은 같은 Run 상태를 보존한다.
- [ ] Retry, 새 Run과 Mode 전환은 이전 획득 상태를 제거한다.
- [ ] InfiniteMode 거리 Score와 종료 흐름에 회귀가 없다.

## Step 7. Stage Mode Collectible 생산 구성을 확정한다

- 진행 상태: **대기**

### AI 작업

- Prefab, Material, Collider, Layer와 Component 구성을 정적으로 검사한다.
- Stage의 자동 이동 속도, Jump 궤적, Collider 크기와 Platform 위치로 Collectible 후보 위치의 도달 가능성을 계산하거나 Test한다.
- 생산 Scene 변경이 필요한 경우 사용자에게 GameObject, Parent, Component, Field, 값과 배치 좌표 단위로 절차를 제공한다.
- Scene 저장 후 YAML로 개수, ID 고유성, 참조, Trigger와 Layer를 검증한다.

### 사용자 Scene 작업

AI가 정적 검사로 필요성을 확인한 경우에만 아래 작업을 수행한다.

1. 안내받은 Collectible Prefab 또는 GameObject를 Stage Root 아래에 배치한다.
2. 안내받은 고유 ID, Collider, Layer와 Component Field를 설정한다.
3. 점프 시작, 공중 경로와 착지 지점 후보 위치에 배치한다.
4. Scene을 저장하고 재개방하여 Missing Reference가 없는지 확인한다.

### 사용자 Test 작업

1. Unity Script Compilation을 확인한다.
2. AI가 지정한 Stage Collectible 생산 Scene Play Mode Test를 실행한다.

### 완료 조건

- [ ] Stage Collectible 구성과 참조가 정적으로 유효하다.
- [ ] ID가 고유하고 Trigger 및 Player 판정 설정이 일치한다.
- [ ] 기본 이동 및 Jump 범위 안에서 획득 가능함이 자동 검증된다.
- [ ] Collectible을 놓쳐도 Goal과 Stage 종료 흐름이 유지된다.

## Step 8. InfiniteMode Pattern Collectible 생산 구성을 확정한다

- 진행 상태: **대기**

### AI 작업

- Pattern별 Collectible 소유, ID와 Pattern 순환 시 복구 경로를 Play Mode Test로 먼저 검증한다.
- 앞 Pattern과 뒤 Pattern의 Collectible이 독립적으로 한 번씩 획득되는지 검증한다.
- 같은 Pattern Object가 재배치될 때 확정 규칙에 따라 다음 구간 Collectible로 복구되는지 검증한다.
- Pattern 경계 Trigger와 Collectible Trigger가 서로 간섭하지 않는지 검증한다.
- 생산 Scene 변경이 필요한 경우 사용자에게 Pattern, 자식 Object, Field와 좌표 단위로 절차를 제공한다.
- Scene 저장 후 YAML로 Pattern별 개수, 참조, ID 규칙과 Trigger 구성을 검증한다.

### 사용자 Scene 작업

AI가 정적 검사로 필요성을 확인한 경우에만 아래 작업을 수행한다.

1. 안내받은 Collectible을 각 InfiniteMode Pattern 아래에 배치한다.
2. Pattern별 ID 또는 재사용 식별 Field, Collider와 Layer를 설정한다.
3. 점프 시작, 공중 경로와 착지 지점 후보 위치에 배치한다.
4. Scene을 저장하고 재개방하여 Missing Reference가 없는지 확인한다.

### 사용자 Test 작업

1. Unity Script Compilation을 확인한다.
2. AI가 지정한 Pattern 재사용 및 InfiniteMode 통합 Play Mode Test를 실행한다.

### 완료 조건

- [ ] 두 Pattern의 Collectible 상태가 독립적이다.
- [ ] Pattern 재사용 시 획득 가능 상태가 정해진 시점에 복구된다.
- [ ] 중복 획득과 중복 Score가 발생하지 않는다.
- [ ] 거리 Score, Pattern 이동과 InfiniteMode 종료 흐름이 유지된다.

## Step 9. 안내 경로의 도달 가능성을 자동 검증한다

- 진행 상태: **대기**

### AI 작업

- 생산 Scene 및 Pattern의 Collectible 위치를 추출한다.
- Player 자동 속도, Jump 높이 및 지속 시간, Collider 범위와 Platform 표면을 기준으로 기본 경로의 도달 가능성을 검증한다.
- 점프 시작 안내, 공중 경로와 착지 안내 구간의 순서가 역전되거나 통과 불가능한 위치가 없는지 Test한다.
- 배치 개수, 간격, 지면 관통, Collider 중첩과 경로 밖 좌표를 정적으로 검사한다.
- 실패 근거가 있는 위치만 사용자에게 수정 좌표로 제시한다.

### 사용자 작업

자동 검증 실패로 Scene 좌표 변경이 필요한 경우에만 AI가 지정한 Object의 Transform을 수정하고 Scene을 저장한다.

### 완료 조건

- [ ] Stage 및 InfiniteMode 기본 경로의 Collectible이 도달 가능하다.
- [ ] Collectible 순서가 점프 시작, 공중 이동과 착지 흐름에 맞는다.
- [ ] 통과 불가능한 점프와 진행 차단 배치가 없다.

## Step 10. 기존 기능, Phase 4 경계와 전체 회귀를 검증한다

- 진행 상태: **대기**

### AI 작업

- Jump, Momentum Landing, 자동 이동, Wall 낙하, Goal, Pattern, Pause, Retry, Camera, Result와 UI 입력 회귀 Test를 검토하고 필요한 Test만 보강한다.
- Collectible을 모두 획득, 일부 획득 또는 모두 놓친 경우 Stage Play를 계속할 수 있는지 검증한다.
- InfiniteMode 거리 Score 및 기존 Result Data가 Phase 3 변경 전 계약을 유지하는지 검증한다.
- Stage Collectible Score, Infinite Collectible Score와 Total Score의 HUD 및 Result 표시는 Phase 4 범위로 남아 있는지 정적으로 확인한다.
- 유사 Test는 계약과 실행 경로를 비교하여 중복을 만들지 않는다.
- Runtime, Test, Prefab, Material과 대응 `.meta` 및 GUID를 검사한다.
- Collectible ID 고유성, Scene 참조, Trigger, Layer, Pattern 자식 구성과 활성 상태를 검사한다.
- Ignore, Explicit, 임의 성공, 조건부 제외, 약화된 기대값과 불필요한 중복 Test를 검사한다.
- Update, FixedUpdate와 Trigger 반복 경로의 LINQ, 매 Frame 컬렉션 생성과 정상 흐름 Log를 검사한다.
- 관련 System 및 Feature 문서와 구현 일치를 확인한다.
- Package, Input Action Asset와 Build Settings의 의도하지 않은 변경을 확인한다.
- Phase 4 및 Prototype 4 범위가 포함되지 않았는지 확인한다.
- `git diff --check`를 수행한다.

### 사용자 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
2. 전체 Edit Mode Test를 실행하고 모두 성공하는지 확인한다.
3. 전체 Play Mode Test를 실행하고 모두 성공하는지 확인한다.
4. Test 관련 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 완료 조건

- [ ] Phase 1 및 Phase 2 기능 회귀가 없다.
- [ ] Collectible 획득 여부가 Stage 진행을 차단하지 않는다.
- [ ] 기존 InfiniteMode 거리 Score와 Result 계약이 유지된다.
- [ ] Phase 4 UI 및 Total Score 기능이 조기 포함되지 않았다.
- [ ] 전체 정적 검증이 통과한다.
- [ ] 전체 Edit Mode 및 Play Mode Test가 통과한다.
- [ ] 예상하지 않은 Compile 및 Test Error와 Warning이 없다.

## Step 11. Build와 최소 화면을 검증하고 완료 근거를 정리한다

- 진행 상태: **대기**

### Build 전 AI 작업

- 활성 Build Scene, Asset과 Scene 참조 및 전체 검증 결과를 확인한다.
- 획득 수, Score, 중복 처리, 초기화와 정확한 위치 판정은 자동 Test 결과를 사용하고 수동 체크리스트에서 제외한다.

### 사용자 Build 및 최소 화면 검증

1. Windows Standalone Win64/x64 Development Build를 한 번 수행한다.
2. Build 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
3. Stage Mode에서 Collectible이 점프 시작, 공중 이동 경로와 착지 지점을 알아보기 쉽게 안내하는지 확인한다.
4. Stage Collectible 일부를 놓쳐도 플레이와 Goal 도달이 자연스러운지 확인한다.
5. InfiniteMode에서 Pattern Collectible이 반복 구간에도 보이고 이동 경로를 자연스럽게 안내하는지 확인한다.
6. Collectible이 화면에서 구분되고 눈에 띄는 떨림, 순간 이동 또는 부자연스러운 겹침이 없는지 확인한다.
7. 기존 자동 이동, Jump, Momentum Landing, Pause, Retry, Wall 낙하와 Camera 추적에 체감 회귀가 없는지 한 번 확인한다.
8. Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### Build 검증 후 AI 작업

- 최종 정적 검증, Compile, 전체 Test, Build와 최소 화면 결과를 기록한다.
- Asset 및 Scene 변경과 미해결 사항을 기록한다.
- 별도 Phase 3 Verification Result Task 문서를 작성한다.
- 모든 완료 조건을 충족한 경우에만 Roadmap Phase 3를 `완료`로 변경한다.

### 완료 조건

- [ ] Build와 최소 화면 검증 결과가 기록되어 있다.
- [ ] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [ ] Phase 3 범위 밖 기능이 포함되지 않았다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 Unity Script Compilation 및 전체 Play Mode 기준선 확인
2. Step 2의 미정 Collectible 규칙 선택
3. 구현 Step 이후 AI가 지정한 Unity Script Compilation 확인
4. AI가 지정한 관련 및 전체 Unity Test Runner 실행
5. Step 7~9에서 정적 검사로 필요성이 확인된 Stage 및 Infinite Pattern Scene 배치와 저장
6. Scene 변경 후 재개방과 Missing Reference 확인
7. Step 11의 Windows Standalone Development Build 실행
8. 두 Mode의 안내성, 가독성, 자연스러운 배치와 기존 조작감 확인
9. Build와 Player의 예상하지 않은 Error 및 Warning 확인

Collectible ID, 획득 횟수, Score 값, 합산, 중복 방지, Pause 및 Retry 상태, Pattern 복구 시점, 정확한 좌표 도달 가능성과 기존 거리 Score는 수동 작업에 포함하지 않고 정적 검증 또는 Unit Test로 처리한다.

---

# 영향 범위

- GameRuntimeData와 Mode별 Runtime Data
- Collectible 상태 및 Score 계산 Feature
- Collectible Trigger 및 획득 연결 System
- GameSystem, StageSystem, InfiniteModeSystem과 ResultSystem 생명주기
- InfiniteMapPattern과 InfinitePatternBoundary
- Stage 및 InfiniteMode 생산 Scene 구성
- 조건부 Collectible Prefab, Material, Collider와 Layer
- Edit Mode 및 Play Mode Test
- 관련 System 및 Feature 문서

---

# 검증 내용

- Roadmap Phase 3 목표와 완료 조건을 11개 실행 Step으로 분리했다.
- 정적 검사, Edit Mode Test, Play Mode Test와 수동 검증의 책임을 구분했다.
- 구현 책임마다 Test 우선 순서와 완료 조건을 배치했다.
- Scene 작업은 정적 검사로 필요성이 확인된 경우에만 사용자에게 요청하도록 제한했다.
- Phase 4의 HUD, Result와 Total Score 표시 및 Prototype 4의 Pattern 확장을 제외했다.
- 현재 테스트 정리 후 기준선 검증을 Phase 3 구현 전에 수행하도록 배치했다.

## Step 수 적정성 검토

- 기존 Step 10의 관련 기능 회귀와 기존 Step 11의 전체 정적 및 자동 회귀는 같은 구현 완료 시점과 같은 검증 대상을 사용하므로 하나의 Step으로 통합했다.
- Step 1은 최근 Test 정리 결과가 아직 Unity에서 검증되지 않아 Phase 3 결함과 기존 기준선 결함을 구분하기 위해 유지한다.
- Step 2는 미정 Score, 식별, 복구와 배치 규칙을 구현 전에 확정해야 하므로 유지한다.
- Step 3은 기존 Score 및 Pattern 생명주기 조사 결과가 이후 구조와 Scene 작업 범위를 결정하므로 구현 Step과 분리한다.
- Step 4~6은 순수 상태 및 계산, 실제 Trigger, Mode 및 Run 생명주기라는 서로 다른 책임과 Test 계층을 가지므로 각각 유지한다.
- Step 7과 Step 8은 고정 Stage 배치와 재사용 Infinite Pattern 배치의 초기화 규칙이 달라 분리한다.
- Step 9는 두 Mode의 배치가 완료된 뒤 전체 안내 경로를 함께 검증하고 실패 좌표만 수정하기 위해 유지한다.
- Step 10은 관련 회귀, 전체 정적 검사와 전체 Test를 한 번에 수행하여 중복 Compile 및 Test 실행을 방지한다.
- Step 11은 Test로 판정할 수 없는 최종 Build, 가독성과 안내성만 검증하므로 자동 검증과 분리한다.
- 11개 Step에 Phase 3 완료 조건의 누락이 없고 동일한 검증 책임의 중복도 없다.

---

# 검증 결과

- Phase 3 수행 순서와 사용자 수동 작업 범위가 정의되었다.
- 자동 판정 가능한 항목은 정적 검증 및 Unit Test 범위로 배치되었다.
- Scene 배치와 화면 안내성처럼 Editor 또는 실제 Player가 필요한 항목만 수동 검증으로 남겼다.
- Phase 3 구현과 검증은 아직 시작하지 않았다.

---

# 후속 작업

Step 1의 Phase 3 기준선 검증을 수행한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/README.md`
- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/03_Features/README.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/90_Tasks/Prototype_3/20260904_02_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_3/20260907_08_Phase2VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260828_04_Prototype3Roadmap.md`
- `AI/90_Tasks/Prototype_3/20260907_09_Phase3ManualSteps.md`

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 3의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 검증보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 작업으로 넘기지 않았다.
- Scene 작업을 정적으로 필요성이 확인된 최소 범위로 제한했다.
- Phase 4, Prototype 4와 밸런스 확장 범위를 분리했다.
- 확인되지 않은 Collectible Score 값과 배치 좌표를 확정값으로 작성하지 않았다.
