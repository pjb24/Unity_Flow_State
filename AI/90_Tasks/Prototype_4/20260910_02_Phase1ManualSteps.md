# 작업 정보

## 작업명

Prototype 4 Phase 1 수동 작업 및 검증 계획

## 작업 일자

20260910

## 작업 담당자

AI, 사용자

## 작업 상태

대기

---

# 작업 목적

Prototype 4 Phase 1의 Map Pattern 확장과 Difficulty 증가 기준을 추측 없이 확정하는 실행 순서를 정의한다.

문서와 코드의 정적 검사를 먼저 수행하고, 수치·경계·상태·선택 규칙은 가능한 경우 독립적인 계약 객체와 Edit Mode Unit Test로 검증한다. Unity Editor가 필요한 작업은 정적 검사와 Test로 판정할 수 없는 Scene 저장 및 최종 화면 확인으로 제한한다.

---

# 작업 대상

- Map Pattern 공통 조건
- Pattern 시작점과 종료점 연결 기준
- 자동 이동 상태의 Pattern 통과 가능 판정 기준
- Prototype 4에서 추가할 Pattern 목록과 각 Pattern의 목적
- Difficulty 단계와 전환 조건
- 진행도에 따른 Pattern 선택 기준
- Pattern 반복 제한과 제외 기준
- Phase 2 및 Phase 3에서 사용할 자동 검증 계약
- 관련 System, Feature, Roadmap 및 Task 문서

---

# 작업 전 상태

- Prototype 3의 자동 이동, Jump, Momentum Landing, Wall 낙하, Infinite Pattern 반복, Collectible 안내, Mode별 Score와 UI가 완료되었다.
- 생산 Scene의 `InfiniteMapPattern`은 `Pattern_0`과 `Pattern_1` 두 인스턴스를 번갈아 재배치하며 현재 하나의 Pattern 종류를 반복한다.
- 기존 Pattern은 StartAnchor, EndAnchor, AdvanceBoundary와 Pattern별 Collectible Scope를 사용한다.
- 추가 Pattern 목록, Pattern 개수, 공통 길이 여부, 연결 호환 조건, Difficulty 단계 수와 전환 수치, 선택 방식 및 반복 제한은 확정되지 않았다.
- Pattern 통과 개수는 Distance 또는 Score 계산에 사용하지 않는다.
- Player 자동 이동, Jump와 Momentum Landing 수치 및 Score 환산 값 조정은 별도 밸런스 작업이다.

---

# 작업 원칙

- 미정 값은 코드나 문서에 임의로 확정하지 않고, 조사 근거와 복수 제안의 장단점을 제시한 뒤 사용자 결정을 받는다.
- 정적 검사로 확인 가능한 Scene 계층, Anchor, Collider, Layer, 길이, 높이와 참조는 사용자에게 재확인시키지 않는다.
- 계산, 유효성, 경계, Difficulty 전환, 선택 가능 여부, 반복 제한과 초기화는 Edit Mode Unit Test를 우선한다.
- Rigidbody, 자동 이동, Collision, Pattern 연결 및 생산 Scene 연동처럼 프레임 실행이 필요한 항목만 Play Mode Test로 검증한다.
- Test는 생산 계약 객체와 선택 로직을 호출하며 Test 안에 같은 계산식이나 별도 선택 알고리즘을 복제하지 않는다.
- Random 선택은 재현 가능한 후보 입력 또는 주입 가능한 난수 원본으로 검증하고 특정 무작위 결과에 의존하지 않는다.
- 빠른 Trigger, 같은 Frame 전환, Pause와 Retry 경계는 수동으로 판정하지 않는다.
- 모든 관련 Test 통과 후에만 전체 Edit Mode 및 Play Mode Test를 한 번 실행한다.
- Phase 1에서는 실제 추가 Pattern 제작, 생산 Scene Pattern 배치, Difficulty Runtime 연동과 UI 구현을 수행하지 않는다.
- Build, 조작감 및 최종 밸런스 비교는 Phase 1 완료 조건에 포함하지 않는다.

---

# 수행 Step

## Step 1. 기존 Pattern 구조와 Phase 1 미정 계약을 조사한다

- 진행 상태: **대기**

### AI 작업

- `InfiniteMapPattern`, `InfinitePatternBoundary`, `StageSystem`, Infinite Runtime Data와 생산 Scene의 현재 Pattern 계층 및 참조를 조사한다.
- 기존 자동 이동, Jump, Momentum Landing, Wall, Camera, Collectible과 Pattern 재사용 Test의 책임을 표로 정리한다.
- 현재 Pattern의 Anchor 위치, 길이, Ground 및 Platform Collider, Layer, Boundary와 Collectible 구성을 Scene YAML에서 정적으로 측정한다.
- 다음 미정 항목마다 결정 가능한 제안과 장점, 단점 및 기존 구조 영향 범위를 채팅으로 제시한다.
  - 추가 Pattern 목록, 개수와 각 Pattern의 목적
  - Pattern 길이 고정 또는 가변 여부
  - 시작점과 종료점의 위치, 높이, 접선 및 Ground 연결 허용 오차
  - 자동 이동 상태의 통과 가능 판정 기준
  - Difficulty 단계 수와 단계 전환에 사용할 진행도
  - Difficulty별 허용 Pattern 집합과 선택 방식
  - 동일 Pattern 연속 반복 제한
  - 직전 Pattern과의 연결 제외 및 대체 후보 처리
  - 첫 Pattern, Retry와 새 Run의 초기 상태
- Pattern 통과 개수와 Score 분리, Player 수치 고정 및 Phase 2~4 경계를 확인한다.

### 사용자 작업

- AI가 제시한 미정 계약별 제안 중 사용할 안을 결정한다.
- 제안만으로 결정할 수 없는 체감 항목이 있는 경우에만 어떤 항목에 실제 화면 비교가 필요한지 지정한다.

### 완료 조건

- [ ] 현재 Pattern 구조와 재사용 지점이 확인되었다.
- [ ] Phase 1의 모든 미정 계약에 선택 가능한 제안과 장단점이 제시되었다.
- [ ] 사용자 결정이 필요한 항목과 자동 판정 항목이 구분되었다.

## Step 2. Map Pattern 공통 정의와 연결 계약을 확정한다

- 진행 상태: **대기**

### AI 작업

- Step 1 결정을 기준으로 Pattern 식별자, 목적, Difficulty 등급, Anchor와 연결 조건의 최소 데이터 계약을 정의한다.
- 누락 ID, 중복 ID, 잘못된 Anchor, 허용 범위 밖 길이·높이·연결 오차와 자기모순 설정을 거부하는 규칙을 정한다.
- 생산 코드에서 실행 가능한 계약 객체가 필요한 경우 기존 구조를 확장하고 유효·경계·잘못된 입력 Unit Test를 먼저 추가한다.
- Phase 2의 Scene Pattern 구현이 계약을 따르는지 정적으로 검사할 수 있는 체크 항목을 문서화한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] 모든 Pattern에 적용되는 최소 데이터와 연결 계약이 확정되었다.
- [ ] 유효성 및 경계 규칙이 Unit Test로 검증된다.
- [ ] Phase 2 Scene 검사의 판정 기준이 문서에 있다.

## Step 3. 자동 이동 상태의 Pattern 통과 가능 기준을 확정한다

- 진행 상태: **대기**

### AI 작업

- 현재 Player 자동 이동, Jump와 Momentum Landing 수치를 읽기 전용 기준값으로 조사한다.
- Ground 연속 구간, Gap, 높이 변화, Platform 진입·이탈, Wall 및 낙하 위험을 판정할 최소 기하 조건을 정의한다.
- 단일 Pattern뿐 아니라 앞 Pattern의 EndAnchor와 뒤 Pattern의 StartAnchor 조합을 통과 가능 판정 대상으로 포함한다.
- 계산 가능한 기하 및 경계 규칙은 순수 계산 함수와 Edit Mode Unit Test로 검증한다.
- 실제 Rigidbody 결과가 필요한 최소 사례만 Play Mode Test 대상으로 분리한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 Edit Mode Test와 필요한 경우에만 최소 Play Mode Test를 실행한다.

### 완료 조건

- [ ] 자동 이동 상태의 통과 가능 여부를 수치와 조건으로 판정할 수 있다.
- [ ] 경계값과 연결 조합이 Unit Test로 검증된다.
- [ ] 체감 판단과 자동 판정의 책임이 구분되었다.

## Step 4. 추가 Pattern 목록과 목적을 확정한다

- 진행 상태: **대기**

### AI 작업

- 사용자 결정과 Step 2~3 계약에 따라 Prototype 4에서 구현할 Pattern 목록을 작성한다.
- 각 Pattern에 ID, 목적, 주요 지형 특성, 요구 입력, 연결 가능 범위, 최초 허용 Difficulty와 제외 조건을 기록한다.
- 목록의 중복 목적, 계약 위반, 특수 이동 규칙·새 장애물·이동 Platform 의존성과 Phase 범위 초과를 정적으로 검사한다.
- 목록과 계약만으로 통과 가능성을 판정할 수 없는 항목은 Phase 2의 명시적인 Test 대상으로 기록한다.

### 사용자 작업

- 확정 목록이 Step 1에서 선택한 Pattern 종류와 목적을 정확히 반영하는지 확인한다.
- 시각적 Blockout 비교가 필요하다고 Step 1에서 결정한 항목이 있을 때만 AI가 제공한 최소 Scene 작업 절차를 수행한다.

### 완료 조건

- [ ] 추가 Pattern 목록과 각 Pattern의 목적이 확정되었다.
- [ ] 모든 항목이 공통 연결 및 통과 가능 계약을 만족하거나 Phase 2 검증 대상으로 기록되었다.
- [ ] Phase 범위 밖 Pattern이 포함되지 않았다.

## Step 5. Difficulty 단계와 진행도 전환 계약을 확정한다

- 진행 상태: **대기**

### AI 작업

- Difficulty 단계, 최초 단계, 전환에 사용할 진행도 단위와 경계 포함 여부를 Step 1 결정대로 정의한다.
- 단계가 역행하거나 건너뛰는지, 경계값에서 어느 단계를 선택하는지, 최대 단계 이후 상태를 명확히 한다.
- 정상 진행, 정확한 경계, 경계 직전·직후, 큰 진행도, 잘못된 값, Pause와 Result 고정 및 Retry 초기화 계약을 Edit Mode Unit Test로 검증한다.
- Pattern 통과 개수가 Distance Score, Collectible Score 또는 Total Score에 들어가지 않는지 기존 Score 계약과 대조한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] Difficulty 단계와 전환 조건을 문서와 Test에서 확인할 수 있다.
- [ ] 경계, 고정 및 초기화 규칙이 Unit Test로 검증된다.
- [ ] Difficulty 진행도와 Score가 분리되어 있다.

## Step 6. Pattern 선택, 반복 및 제외 계약을 확정한다

- 진행 상태: **대기**

### AI 작업

- 현재 Difficulty에서 허용된 후보, 이전 Pattern 연결 호환성, 반복 제한과 제외 조건을 적용하는 순서를 정의한다.
- 후보가 하나일 때, 후보가 모두 제외될 때, 직전 Pattern만 가능한 경우와 최대 Difficulty의 대체 규칙을 확정한다.
- 선택 결과가 항상 허용 후보 안에 있는지, 금지 조합과 반복 제한을 지키는지, 같은 입력에서 재현 가능한지 Unit Test로 검증한다.
- Retry와 새 Run의 선택 이력 초기화, Pause와 Result 중 불변 및 중복 진행 요청 거부를 Unit Test로 검증한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 관련 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] Difficulty별 Pattern 선택 순서와 후보 규칙이 확정되었다.
- [ ] 반복, 제외, 대체 후보와 초기화 규칙이 Unit Test로 검증된다.
- [ ] 선택 불가 또는 무한 재선택 상태가 없다.

## Step 7. Phase 1 계약 문서와 후속 Phase 검증 책임을 정리한다

- 진행 상태: **대기**

### AI 작업

- 확정된 Pattern 공통 조건과 목록을 관련 Feature 문서에 반영한다.
- Difficulty와 선택 상태의 소유 주체, 입력과 출력 및 생명주기를 관련 System 문서에 반영한다.
- Phase 2의 Pattern 제작·연결·물리 통과, Phase 3의 Runtime 선택·진행도, Phase 4의 Collectible·UI·전체 회귀 책임을 구분한다.
- 문서의 Pattern 개수, ID, 단계, 경계, 반복 및 제외 규칙이 코드와 Test의 단일 계약과 일치하는지 정적으로 검사한다.

### 사용자 작업

- 없음

### 완료 조건

- [ ] 관련 System, Feature와 Roadmap 문서가 서로 모순되지 않는다.
- [ ] 확정 수치와 조건의 근거를 문서에서 확인할 수 있다.
- [ ] Phase 2~4의 구현 및 검증 책임이 분리되어 있다.

## Step 8. 생산 Scene 변경 필요성과 Phase 1 회귀 범위를 확정한다

- 진행 상태: **대기**

### AI 작업

- 생산 Scene, Prefab 및 Asset의 현재 Pattern 참조가 Phase 1 계약 정의만으로 변경되어야 하는지 정적으로 검사한다.
- Phase 1에서 실제 Pattern을 제작하지 않는 원칙에 따라 불필요한 Scene, Prefab과 Input Action 변경이 없는지 확인한다.
- 기존 Infinite Pattern, 자동 이동, Jump, Momentum Landing, Wall, Camera, Collectible, Score, Pause, Result와 Retry Test를 재사용하고 새 계약과 직접 관련된 누락 Test만 추가한다.
- `Ignore`, `Explicit`, 임의 성공, 조건부 제외, 약화된 기대값과 중복 Test를 검사한다.

### 사용자 작업

- 정적 검사에서 반드시 필요한 Scene 변경이 확인된 경우에만 AI가 지정한 Hierarchy, Component, Field와 값 단위 절차를 수행하고 저장한다.
- Scene 변경이 없다면 사용자 작업은 없다.

### 완료 조건

- [ ] Phase 1의 Scene 변경 필요 여부가 근거와 함께 확정되었다.
- [ ] 기존 회귀 Test와 신규 Unit Test의 책임이 중복되지 않는다.
- [ ] Package와 Input Action Asset에 의도하지 않은 변경이 없다.

## Step 9. 전체 정적 검사와 전체 Test를 수행한다

- 진행 상태: **대기**

### AI 작업

- Runtime, Test, 문서와 대응 `.meta` 및 GUID를 검사한다.
- Pattern ID, Difficulty 경계, 후보 집합, 연결 행렬과 문서 수치의 일치를 검사한다.
- Update, FixedUpdate와 Trigger 반복 경로의 LINQ, 매 Frame 컬렉션 생성과 정상 흐름 Log를 검사한다.
- Phase 2의 실제 Pattern 제작, Phase 3의 Runtime 연동, Phase 4의 UI 및 Collectible 배치와 밸런스 조정이 섞이지 않았는지 검사한다.
- `git diff --check`를 수행한다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- 전체 Edit Mode Test를 한 번 실행하고 모두 성공하는지 확인한다.
- 전체 Play Mode Test를 한 번 실행하고 모두 성공하는지 확인한다.
- Test 관련 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 완료 조건

- [ ] 전체 정적 검증이 통과한다.
- [ ] 전체 Edit Mode 및 Play Mode Test가 통과한다.
- [ ] 예상하지 않은 Compile 및 Test Error와 Warning이 없다.
- [ ] Phase 1 범위 밖 구현이 포함되지 않았다.

## Step 10. 최소 수동 확인과 Phase 1 완료 근거를 정리한다

- 진행 상태: **대기**

### AI 작업

- Pattern 규칙, 기하 경계, Difficulty 전환, 선택, 반복, 제외와 초기화는 정적 검사 및 Unit Test 결과를 사용하고 수동 체크리스트에서 제외한다.
- 생산 Scene 및 Asset 변경 여부와 전체 검증 결과를 확인한다.

### 사용자 작업

- Phase 1에서 Scene과 Runtime 플레이 동작을 변경하지 않았다면 추가 화면 검증을 수행하지 않는다.
- Step 1에서 시각적 Blockout 비교가 필요하다고 결정했고 실제 Scene 작업을 수행한 경우에만 AI가 지정한 Pattern 연결 형태와 화면 식별 가능성을 확인한다.
- 화면 확인을 수행한 경우 Console의 예상하지 않은 Error와 Warning 부재를 확인한다.

### 화면 검증 후 AI 작업

- 최종 정적 검증, Compile, 전체 Test와 필요한 최소 화면 결과를 기록한다.
- Asset 및 Scene 변경과 미해결 사항을 기록한다.
- 별도 Prototype 4 Phase 1 Verification Result Task 문서를 작성한다.
- 모든 완료 조건을 충족한 경우에만 Roadmap Phase 1을 `완료`로 변경한다.

### 완료 조건

- [ ] Pattern 목록, 연결, 통과, Difficulty와 선택 계약이 모두 확정되었다.
- [ ] 자동 판정 가능한 계약이 Unit Test로 검증되었다.
- [ ] 필요한 정적, Compile, 전체 Test 및 최소 수동 검증 결과가 기록되었다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행하는 작업은 아래로 제한한다.

1. Step 1에서 AI가 제시한 미정 계약별 제안 선택
2. 코드 또는 Test가 변경된 Step의 Unity Script Compilation 확인
3. AI가 지정한 관련 Edit Mode Unit Test 및 필요한 최소 Play Mode Test 실행
4. 정적 검사에서 필요성이 확인된 경우에만 최소 Scene 작업과 저장
5. Step 9의 전체 Edit Mode 및 Play Mode Test 실행
6. 실제 Scene 또는 Runtime 표현을 변경한 경우에만 Step 10의 최소 화면 확인
7. Compile, Test와 Play Mode Console의 예상하지 않은 Error 및 Warning 확인

Pattern 수치 계산, 연결 허용 여부, 통과 가능 경계, Difficulty 전환, 후보 선택, 반복 제한, 제외, Pause 고정, Retry 초기화와 중복 요청은 수동 작업에 포함하지 않고 정적 검사와 Unit Test로 처리한다.

---

# Scene 작업 원칙

- Phase 1의 기본 범위에는 생산 Scene Pattern 제작과 배치가 없다.
- Scene 변경이 필요하다고 정적 검사에서 확인된 경우 AI는 변경 이유와 함께 정확한 Hierarchy 경로, Component, Serialized Field와 값을 제공한다.
- 사용자는 Unity Editor에서 해당 항목만 변경하고 저장한 뒤 Missing Reference와 예상하지 않은 Console Error 및 Warning 부재를 확인한다.
- AI는 Scene 파일을 직접 수정하지 않고 저장 결과를 정적으로 검사한다.

---

# 영향 범위

- Infinite Map Pattern 공통 계약
- Pattern 연결 및 통과 가능 판정 기준
- Difficulty 단계와 전환 계약
- Pattern 선택, 반복과 제외 계약
- 관련 순수 Runtime Data 또는 규칙 객체
- Edit Mode Unit Test 및 필요한 최소 Play Mode Test
- 관련 System, Feature, Roadmap과 Task 문서

---

# 검증 내용

- `IMPLEMENTATION_ROADMAP_004.md` Phase 1의 목표와 완료 조건을 10개 실행 Step으로 배치했다.
- 미정 Pattern 목록, 개수, 연결 수치, Difficulty 및 선택 기준을 Step 1 사용자 결정 전에 구현하지 않도록 분리했다.
- 정적 Scene 측정과 코드 조사는 AI 작업으로, 미정 계약 선택과 Unity 실행 결과 확인만 사용자 작업으로 배치했다.
- 계산, 경계, 상태, 선택과 생명주기는 Edit Mode Unit Test를 우선하도록 했다.
- 생산 Scene 물리와 실제 System 연결이 필요한 최소 항목만 Play Mode Test로 배치했다.
- 실제 Pattern 제작, Runtime Difficulty 연동, Collectible 및 UI와 밸런스 조정을 후속 Phase와 별도 작업으로 유지했다.

---

# 검증 결과

- Prototype 4 Phase 1의 실행 순서와 사용자 수동 작업 범위를 정의했다.
- Pattern 및 Difficulty 미정 계약의 결정 시점과 자동 검증 책임을 정의했다.
- 추가 구현과 Unity 검증은 아직 수행하지 않았다.

---

# 후속 작업

Step 1에서 기존 Pattern 구조와 Phase 1 미정 계약을 조사한다.

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
- `AI/90_Tasks/Prototype_4/20260828_05_Prototype4Roadmap.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260910_01_Phase4VerificationResult.md`

---

# 작성 완료 기준

- Roadmap Phase 1의 목표와 완료 조건을 실행 가능한 Step으로 나눴다.
- 정적 검사와 Unit Test 우선 원칙을 각 Step의 작업과 완료 조건에 반영했다.
- 사용자에게 필요한 결정, Unity 실행과 예외적인 Scene 작업만 수동 작업으로 남겼다.
- 미정 Pattern, Difficulty 및 선택 수치를 임의로 확정하지 않았다.
- 후속 Phase와 밸런스 작업을 Phase 1 범위에 포함하지 않았다.
