# 작업 정보

## 작업명

Prototype 3 Phase 4 수동 작업 및 검증 계획

## 작업 일자

20260909

## 작업 담당자

AI, 사용자

## 작업 상태

대기

---

# 작업 목적

Prototype 3 Phase 4의 Mode별 Score와 UI 통합, Stage Mode 추락 처리 및 전체 플레이 흐름 검증을 실행 가능한 Step으로 정리한다.

정적 검사와 Unity Test Runner로 판정할 수 있는 항목은 자동 검증하고, Scene 저장과 화면 표현처럼 Unity Editor가 필요한 작업만 사용자 작업으로 남긴다.

---

# 작업 대상

- Stage Mode Collectible Score 표시
- InfiniteMode Distance Score, Collectible Score와 Total Score 계산 및 표시
- Stage Mode와 InfiniteMode Result Data 및 ResultPanel 확장
- Pause, Result, Retry와 새 Run의 Score 생명주기
- Stage Mode 추락 실패 처리
- StageHUD, InfiniteHUD와 ResultPanel 생산 Scene 구성
- 관련 Edit Mode 및 Play Mode Test
- 관련 System 및 Feature 문서

---

# 작업 전 상태

- Prototype 3 Phase 3까지 완료되었으며 Script Compilation, Edit Mode Test 314개와 Play Mode Test 179개가 통과했다.
- `CollectibleRuntimeData.CurrentScore`는 Run별 Collectible Score를 소유하지만 HUD와 Result에는 표시하지 않는다.
- InfiniteMode HUD 및 Result는 기존 `InfiniteModeRuntimeData.CurrentScore` 거리 Score를 표시한다.
- Stage Result는 Clear 여부와 Clear Time을 소유하고 Collectible Score를 포함하지 않는다.
- Infinite Result는 최종 거리와 거리 Score를 소유하고 Collectible Score 및 Total Score를 포함하지 않는다.
- Pause 중에는 `GameState`가 `Playing`이 아니므로 Collectible 획득이 중단되고 Infinite 거리 갱신도 중단된다.
- Retry와 새 Run은 `GameRuntimeData`를 다시 생성하여 Collectible 및 InfiniteMode Runtime Data를 초기화한다.
- InfiniteMode에는 Y 추락 임계값 종료가 있지만 Stage Mode에는 바닥 아래로 떨어진 Player를 종료하는 처리가 없다.

---

# 조사 내용

- `IMPLEMENTATION_ROADMAP_003.md` Phase 4는 Mode별 Score, HUD, Result, Pause, Retry와 전체 회귀를 대상으로 한다.
- `20260828_05_Prototype4Roadmap.md`는 Map Pattern 확장과 진행도 기반 난이도를 Prototype 4로 분리한다. Phase 4에서는 Pattern 종류, 선택 규칙과 난이도를 확장하지 않는다.
- Stage Mode 추락은 Stage 전체 플레이 흐름과 실패 Result 및 Retry에 영향을 주므로 Prototype 3 Phase 4에서 함께 처리할 수 있다.
- Stage 추락은 InfiniteMode 거리 진행 규칙을 사용하지 않는다. Stage Mode에서만 Y 임계값을 평가하고 기존 Stage 종료 이벤트와 Result 생성 흐름을 재사용한다.
- 정확한 Score 값, 합산, 포화, Pause 고정, Result 확정과 Retry 초기화는 정적 검사와 Test로 판정할 수 있다.
- UI 계층, Serialized Field, 텍스트 가독성과 화면 겹침은 생산 Scene 저장 또는 실제 화면 확인이 필요한 경우에만 사용자 작업으로 요청한다.

---

# 선행 구현 규칙

아래 규칙을 Phase 4의 기본안으로 사용한다. 구현 전 기존 문서와 코드에서 충돌이 발견되면 Step 1에서 대안을 제시하고 확정한다.

- Stage Mode HUD는 현재 Run의 Collectible Score를 표시한다.
- Stage Result는 Clear Time과 Collectible Score를 서로 다른 값으로 표시한다.
- InfiniteMode HUD는 Distance Score, Collectible Score와 Total Score를 구분하여 표시한다.
- Infinite Result는 Final Distance, Distance Score, Collectible Score와 Total Score를 구분하여 표시한다.
- Infinite Total Score는 `Distance Score + Collectible Score`이며 `int.MaxValue`에서 포화한다.
- Pause 동안 Distance Score와 Collectible Score는 증가하지 않는다.
- Result 진입 시 Score를 Result Data에 복사하고 이후 Runtime Data 정리의 영향을 받지 않게 한다.
- Retry와 새 Run은 이전 Distance, Collectible 획득 상태, Collectible Score와 Total Score를 유지하지 않는다.
- Stage Mode에서 Player 물리 위치의 Y가 Stage 추락 임계값 이하가 되면 미클리어 상태로 Stage를 한 번만 종료한다.
- Stage 추락 Result에는 추락 시점까지의 Clear Time과 Collectible Score를 확정하며 Retry를 허용한다.
- Stage 추락 임계값은 생산 지형보다 충분히 아래에 두고 InfiniteMode 임계값과 같은 기본값 `-3.0`을 우선 검토하되, Scene 지형 정적 검사 후 확정한다.

---

# 작업 원칙

- 각 구현 Step은 문서와 코드 정적 검사, 필요한 Edit Mode Test, 필요한 Play Mode Test 순서로 진행한다.
- 수치 계산, 포화, 상태 전환, 초기화와 중복 방지는 Edit Mode Test로 검증한다.
- 실제 생산 Scene 참조, System 실행 순서, Pause, Result, Retry, 물리 위치와 UI 연결은 Play Mode Test로 검증한다.
- Test는 생산 계산과 상태 객체를 사용하며 구현 계산식을 Test에 복사하지 않는다.
- 빠른 입력, 같은 Frame 상태 변화와 임계값 경계는 수동으로 판정하지 않는다.
- Scene 변경 필요성은 YAML과 Script Serialized Field를 먼저 검사한 후 확정한다.
- AI는 Scene을 직접 수정하지 않는다. 필요한 경우 Hierarchy, Component, Field와 값 단위의 사용자 절차를 제공한다.
- 모든 관련 Test가 통과한 뒤에만 전체 Edit Mode 및 Play Mode Test를 한 번 실행한다.
- 화면 가독성, 텍스트 겹침과 시각적 안정성만 최종 수동 확인 대상으로 남긴다.

---

# 수행 Step

## Step 1. Phase 4 계약과 현재 UI 및 Result 구조를 확정한다

- 진행 상태: **대기**

### AI 작업

- `ResultData`, `ResultSystem`, `GameSystem`, `UIManagementSystem`, Formatter와 생산 Scene UI 참조를 조사한다.
- Stage 및 Infinite HUD와 Result Content의 현재 Text 필드, 활성 상태와 갱신 경로를 정리한다.
- 선행 구현 규칙의 Score 합산, 포화, 표시 항목과 Stage 추락 계약이 기존 구조와 충돌하는지 확인한다.
- 기존 Test의 책임과 누락 계약을 표로 정리하고 추가할 Test만 결정한다.
- Prototype 4의 Pattern 및 Difficulty 범위가 섞이지 않았는지 확인한다.

### 사용자 작업

- AI 조사에서 선행 구현 규칙과 충돌하거나 선택이 필요한 사항이 발견된 경우에만 제안 중 하나를 결정한다.

### 완료 조건

- [ ] Score, Result, UI와 Stage 추락 계약이 확정되었다.
- [ ] 기존 구조의 재사용 지점과 변경 지점이 확인되었다.
- [ ] 자동 Test와 수동 검증의 책임이 구분되었다.

## Step 2. Total Score 계산과 Result Data 계약을 구현한다

- 진행 상태: **대기**

### AI 작업

- Infinite Total Score의 포화 합산을 생산 계산 경로에 구현한다.
- Stage Result에 Collectible Score를, Infinite Result에 Distance Score, Collectible Score와 Total Score를 독립 필드로 추가한다.
- 기존 생성자 또는 호출부의 호환성을 검토하고 하나의 확정 Result만 생성하도록 유지한다.
- 정상값, 0, 음수 거부, `int.MaxValue` 경계와 포화 합산을 Edit Mode Test로 검증한다.
- 기존 `ResultDataTests`, Score 계산 Test와 중복되는 Test를 만들지 않는다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] Mode별 Result Data가 필요한 Score를 독립적으로 보존한다.
- [ ] Total Score가 정확하게 포화 합산된다.
- [ ] 관련 Edit Mode Test가 통과한다.

## Step 3. HUD와 Result 표시 변환 규칙을 구현한다

- 진행 상태: **대기**

### AI 작업

- Mode별 Score 표시 문자열과 미초기화 Placeholder 규칙을 Formatter에 구현한다.
- Stage HUD, Stage Result, Infinite HUD와 Infinite Result의 표시 상태를 UI 상태 객체에 반영한다.
- 숫자 서식, Placeholder, Mode별 필드 선택과 잘못된 값 거부를 Edit Mode Test로 검증한다.
- 기존 Clear Time, Final Distance와 거리 Score 표시 규칙을 유지한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] Mode별 표시 문자열과 Placeholder 계약이 구현되었다.
- [ ] 기존 시간 및 거리 표시 계약이 유지된다.
- [ ] 관련 Edit Mode Test가 통과한다.

## Step 4. Runtime Score를 HUD와 Result 생명주기에 연결한다

- 진행 상태: **대기**

### AI 작업

- Playing 중 현재 Mode의 Runtime Score만 HUD에 전달한다.
- Pause 동안 마지막 HUD 값을 유지하고 Score 갱신을 중단한다.
- Stage 종료 및 Infinite 종료 시 Runtime 값을 Result Data에 한 번 복사한 뒤 표시한다.
- Runtime Data 정리 후에도 Result 값이 바뀌지 않게 한다.
- Retry 및 연속 Run에서 이전 Score와 UI 값이 남지 않게 한다.
- Mode 전환, Pause, Result, Retry와 연속 Run을 Play Mode Test로 검증한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 Play Mode Test만 실행한다.

### 완료 조건

- [ ] HUD가 현재 Run의 Mode별 Score만 표시한다.
- [ ] Result가 종료 시점 값을 보존한다.
- [ ] Pause와 Retry 생명주기가 유지된다.
- [ ] 관련 Play Mode Test가 통과한다.

## Step 5. Stage Mode 추락 실패 처리를 구현한다

- 진행 상태: **대기**

### AI 작업

- 생산 Stage 지형의 최저 Y와 Player 물리 위치 기준을 정적으로 조사하여 Stage 추락 임계값을 확정한다.
- Stage Mode Playing 중에만 Player Rigidbody의 Y 임계값을 평가한다.
- 임계값 이하에서 Stage를 미클리어 상태로 한 번만 종료하고 기존 GameSystem Result 흐름을 사용한다.
- Pause, Result와 Ended에서는 추락 판정을 수행하지 않고 Resume 후 다시 평가한다.
- Goal과 추락 요청이 인접 Frame에 발생해도 하나의 Stage 결과만 확정되도록 한다.
- Retry 후 Player 위치, Timer, Collectible과 추락 판정 상태가 새 Run으로 초기화되게 한다.
- 임계값 직전/동일/직후, Pause, Goal 경합, 중복 요청과 Retry를 Edit Mode 및 Play Mode Test로 검증한다.

### 사용자 작업

- AI 변경 후 Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- AI가 지정한 Edit Mode 및 Play Mode Test만 실행한다.
- 정적 검사 결과 Serialized Field 저장이 반드시 필요한 경우에만 AI가 제공한 값으로 `StageSystem`의 Stage 추락 임계값을 설정하고 Scene을 저장한다.

### 완료 조건

- [ ] Stage Mode 추락이 미클리어 Result로 한 번만 종료된다.
- [ ] Pause, Goal과 Retry 경계가 정의된 계약을 따른다.
- [ ] InfiniteMode 추락 및 거리 진행 계약에 회귀가 없다.
- [ ] 관련 Edit Mode 및 Play Mode Test가 통과한다.

## Step 6. 생산 Scene UI 변경 필요성을 정적으로 판정한다

- 진행 상태: **대기**

### AI 작업

- 생산 Scene YAML에서 StageHUD, InfiniteHUD, StageResultContent와 InfiniteResultContent의 계층 및 TMP Text 참조를 검사한다.
- 기존 Text 재사용 가능 여부와 새 Text가 필요한 위치를 판정한다.
- UI Script의 Serialized Field와 Scene 참조를 대조하여 필요한 Scene 작업만 작성한다.
- 누락 참조, 중복 EventSystem, Canvas 정렬, Mode별 활성 상태와 기존 버튼 입력 경로를 정적으로 검사한다.

### 사용자 작업

- 정적 검사에서 새 UI Object 또는 Serialized Reference가 필요하다고 확인된 경우에만 AI가 제공하는 Hierarchy, Component, Anchor, Text와 Field 절차에 따라 Scene을 수정하고 저장한다.
- Scene 저장 후 Unity Script Compilation 성공과 Missing Reference 부재를 확인한다.

### 완료 조건

- [ ] 생산 Scene UI 변경 필요성이 정적으로 판정되었다.
- [ ] 필요한 최소 Scene 작업만 완료되었다.
- [ ] 모든 UI Serialized Reference가 유효하다.

## Step 7. 생산 Scene의 Mode별 UI 연결을 자동 검증한다

- 진행 상태: **대기**

### AI 작업

- 생산 Scene에서 Stage 및 Infinite HUD와 Result Text 참조 및 초기 활성 상태를 검증하는 Play Mode Test를 보강한다.
- Stage Playing, Stage Result, Infinite Playing, Infinite Result와 Pause 상태의 표시 조합을 검증한다.
- Keyboard와 Mouse의 기존 Pause 및 Result UI 입력 경로가 새 Text 추가의 영향을 받지 않는지 검증한다.
- UI Test가 화면 문구를 생산 Formatter 계약과 별도로 복제하지 않게 한다.

### 사용자 작업

- AI가 지정한 Play Mode Test를 실행하고 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 완료 조건

- [ ] 생산 Scene의 Mode별 UI 참조와 상태가 자동 검증된다.
- [ ] 기존 Keyboard와 Mouse UI 입력 회귀가 없다.
- [ ] 관련 Play Mode Test가 통과한다.

## Step 8. Score 생명주기와 전체 플레이 회귀를 검증한다

- 진행 상태: **대기**

### AI 작업

- 획득 없음, 일부 및 전체 획득의 Stage와 Infinite Result를 검증한다.
- Pause 중 Distance 및 Collectible Score 고정, Result 확정, Retry 초기화와 연속 Run 독립성을 검증한다.
- Jump, Momentum Landing, 자동 이동, Wall 낙하, Goal, Pattern 재사용, Camera, Pause와 Result 입력 회귀 Test를 검토한다.
- 기존 Test로 보장되는 계약은 재사용하고 실행 경로가 다른 누락 Test만 추가한다.
- Ignore, Explicit, 임의 성공, 조건부 제외, 약화된 기대값과 불필요한 중복 Test를 검사한다.

### 사용자 작업

- AI가 지정한 관련 Edit Mode 및 Play Mode Test를 실행한다.

### 완료 조건

- [ ] Mode별 Score와 Result 생명주기가 자동 검증된다.
- [ ] Stage 추락과 기존 기능의 회귀가 없다.
- [ ] 관련 Edit Mode 및 Play Mode Test가 통과한다.

## Step 9. 전체 정적 검사와 전체 Test를 수행한다

- 진행 상태: **대기**

### AI 작업

- Runtime, Test, Scene과 대응 `.meta` 및 GUID를 검사한다.
- UI 및 Stage 추락 Serialized Reference, Layer, 활성 상태와 Mode별 계층을 검사한다.
- Update, FixedUpdate와 Trigger 반복 경로의 LINQ, 매 Frame 컬렉션 생성과 정상 흐름 Log를 검사한다.
- 관련 System 및 Feature 문서와 구현 일치를 확인한다.
- Package와 Input Action Asset의 의도하지 않은 변경을 확인한다.
- Prototype 4 Pattern 및 Difficulty 확장, 밸런스 조정과 기타 범위 밖 기능이 포함되지 않았는지 확인한다.
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
- [ ] Phase 4 범위 밖 기능이 포함되지 않았다.

## Step 10. 최소 화면을 검증하고 완료 근거를 정리한다

- 진행 상태: **대기**

### AI 작업

- 정확한 Score 값, 합산, Pause, Result와 Retry는 자동 Test 결과를 사용하고 수동 체크리스트에서 제외한다.
- 생산 Scene, Asset 참조와 전체 검증 결과를 확인한다.

### 사용자 작업

1. Unity Editor Play Mode에서 Stage HUD의 Collectible Score가 읽기 쉽고 기존 플레이 화면을 가리지 않는지 확인한다.
2. Stage Clear 및 추락 실패 Result에서 Clear Time과 Collectible Score가 구분되는지 확인한다.
3. Infinite HUD와 Result에서 Distance, Collectible과 Total Score가 서로 구분되는지 확인한다.
4. PausePanel과 ResultPanel이 새 Text와 겹치지 않고 Keyboard 및 Mouse 조작이 자연스러운지 확인한다.
5. Stage 추락 후 실패 Result와 Retry 흐름이 화면상 자연스러운지 확인한다.
6. Stage Mode와 InfiniteMode를 반복 플레이하며 텍스트 떨림, 잘림과 순간적인 잘못된 값 표시가 없는지 확인한다.
7. Console에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 화면 검증 후 AI 작업

- 최종 정적 검증, Compile, 전체 Test와 최소 화면 결과를 기록한다.
- Asset 및 Scene 변경과 미해결 사항을 기록한다.
- 별도 Phase 4 Verification Result Task 문서를 작성한다.
- 모든 완료 조건을 충족한 경우에만 Roadmap Phase 4를 `완료`로 변경한다.

### 완료 조건

- [ ] 최소 화면 검증 결과가 기록되어 있다.
- [ ] Mode별 HUD와 Result의 가독성이 확인되었다.
- [ ] Stage 추락 실패 및 Retry 화면 흐름이 확인되었다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행하는 작업은 아래로 제한한다.

1. AI가 구현을 변경한 Step의 Unity Script Compilation 확인
2. AI가 지정한 관련 Edit Mode 또는 Play Mode Test 실행
3. Step 6의 정적 검사로 필요성이 확인된 최소 UI 또는 Serialized Field Scene 작업과 저장
4. Scene 변경 후 Missing Reference 및 Compilation 확인
5. Step 9의 전체 Edit Mode 및 Play Mode Test 실행
6. Step 10의 UI 가독성, 화면 겹침, Stage 추락 및 Retry 화면 흐름 확인
7. Compile, Test와 Play Mode Console의 예상하지 않은 Error 및 Warning 확인

Score 계산, 포화, Result 확정, Pause 고정, Retry 초기화, Stage 추락 임계값 경계와 중복 종료는 수동 작업에 포함하지 않고 정적 검사와 Unit Test로 처리한다.

---

# 영향 범위

- ResultData와 Mode별 Runtime Data
- Score 계산 및 표시 Formatter
- GameSystem, StageSystem, ResultSystem과 UIManagementSystem
- Stage 추락 종료 처리
- StageHUD, InfiniteHUD와 ResultPanel 생산 Scene 구성
- Edit Mode 및 Play Mode Test
- 관련 System 및 Feature 문서

---

# 검증 내용

- Prototype 3 Phase 4 Roadmap의 목표와 완료 조건을 10개 실행 Step으로 배치했다.
- `20260828_05_Prototype4Roadmap.md`를 참조하여 Prototype 4의 Pattern 및 Difficulty 확장을 제외했다.
- Stage Mode 추락 문제를 Phase 4의 종료, Result 및 Retry 범위에 포함했다.
- 수치, 상태, 경계, 중복 실행과 생명주기는 Edit Mode 및 Play Mode Test가 담당하도록 했다.
- Scene 저장과 최종 화면 가독성 외의 자동 판정 항목을 사용자 작업에서 제외했다.
- 전체 Test는 관련 Test 통과 후 한 번만 실행하도록 배치했다.

---

# 검증 결과

- Phase 4 수행 순서와 사용자 수동 작업 범위가 정의되었다.
- Score 및 Result 계약, UI 연결과 Stage 추락 처리의 자동 검증 책임이 정의되었다.
- Stage 추락 처리를 Phase 4에 포함할 수 있음을 확인했다.
- 추가 구현과 Unity 검증은 아직 수행하지 않았다.

---

# 후속 작업

Step 1에서 Phase 4 계약과 현재 UI 및 Result 구조를 조사한다.

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ResultMenu.md`
- `AI/03_Features/ScoreCollectible.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/StagePlay.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/90_Tasks/Prototype_3/20260909_01_Phase3VerificationResult.md`
- `AI/90_Tasks/Prototype_4/20260828_05_Prototype4Roadmap.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260828_04_Prototype3Roadmap.md`
- `AI/90_Tasks/Prototype_3/20260907_09_Phase3ManualSteps.md`
- `AI/90_Tasks/Prototype_3/20260909_01_Phase3VerificationResult.md`

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 4의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 검증보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 작업으로 넘기지 않았다.
- Scene 작업을 정적으로 필요성이 확인된 최소 범위로 제한했다.
- Stage Mode 추락 처리의 계약, 자동 검증과 화면 검증을 포함했다.
- Prototype 4와 밸런스 확장 범위를 분리했다.
