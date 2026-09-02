# 작업 정보

## 작업명

Prototype 2 Phase 4 Manual Steps

## 작업 일자

20260902

## 작업 담당자

AI, 사용자

## 작업 상태

준비 완료

---

# 작업 목적

Prototype 2 Phase 4의 Mode별 UI와 전체 반복 플레이 흐름을 구현하고 검증하는 순서를 정의한다.

정적 검증과 Unit Test로 판정할 수 있는 항목은 자동화하고 Unity Editor에서만 가능한 Scene 구성, Build와 최종 화면 확인만 사용자 수동 작업으로 분리한다.

---

# 작업 범위

- Stage Mode와 InfiniteMode UI 구분
- InfiniteMode HUD
- 현재 이동 거리와 Score 표시
- InfiniteMode Result 표시
- PausePanel UI 마무리
- Keyboard와 Mouse UI 입력 회귀
- Stage Mode와 InfiniteMode 반복 플레이
- Compile, 전체 Test와 Build 검증

---

# 제외 범위

- Save와 Leaderboard
- 신규 게임 Mode
- Prototype 3 자동 이동과 Collectible
- Prototype 4 Map Pattern 추가
- 최종 상용 아트, 다국어, 해상도별 완성 레이아웃
- 현재 Phase 목표와 무관한 이동·물리 변경

---

# 현재 기준 상태

- Prototype 2 Phase 3가 완료되었다.
- Unity Script Compilation 성공과 예상하지 않은 Error·Warning 부재가 확인되었다.
- Edit Mode Test 기준은 `177 Passed, 0 Failed`이다.
- Play Mode Test 기준은 `87 Passed, 0 Failed`이다.
- `StageHUD`, 공용 `ResultPanel`과 최소 `PausePanel`이 생산 Scene에 존재한다.
- Stage Result는 Clear Time을 표시하지만 InfiniteMode의 현재 거리·Score HUD와 최종 결과 Text는 없다.
- ResultData에는 Stage Clear Time과 InfiniteMode Final Distance·Final Score가 이미 Mode별로 구분되어 있다.
- Keyboard와 Mouse의 ResultMenu 및 PauseMenu 입력 흐름은 Phase 3 Test로 검증되어 있다.

---

# 검증 원칙

- 실패하는 Test 또는 변경될 기대값을 생산 코드보다 먼저 작성한다.
- 문자열 형식, Mode별 표시 규칙과 UI 상태 매핑은 Edit Mode Unit Test를 우선한다.
- 실제 Scene 참조, HUD 갱신, Result 전환, Retry와 입력 통합은 Play Mode Test로 검증한다.
- 빠른 연속 조작, 동일·인접 프레임 입력, 입력 잔류와 중복 실행은 수동으로 요구하지 않고 Play Mode Test로 검증한다.
- Scene 변경 전에 필요한 계층, Component, 이름과 Serialized Field를 코드와 생산 Scene 구조 Test로 확정한다.
- 화면 배치와 가독성처럼 자동 판정이 부적절한 항목만 최소 수동 플레이로 확인한다.
- AI는 Unity Build와 Unity Test Runner를 실행하지 않는다.
- AI는 Scene을 수정하지 않고 정확한 사용자 작업 절차와 변경 후 정적 검증을 제공한다.

---

# Step 구성

## Step 1. Phase 4 UI 표시 규칙을 확정한다

- 진행 상태: **대기**

### 결정 항목

1. Infinite HUD의 거리와 Score 표시 형식을 결정한다.
2. Infinite Result의 최종 거리와 최종 Score 표시 형식을 결정한다.
3. 거리의 소수 자릿수와 반올림 규칙을 결정한다.
4. Stage와 InfiniteMode에서 활성화할 HUD 및 Result Content 조합을 결정한다.
5. PausePanel 마무리 범위와 Phase 4에서 요구할 최소 가독성 기준을 결정한다.
6. HUD 갱신 주기와 값이 없는 상태의 표시 규칙을 결정한다.
7. 지원할 화면 크기와 Build 대상의 최소 검증 범위를 결정한다.

### 정적 검증

- 기존 ResultData와 InfiniteModeRuntimeData 값을 다시 계산하지 않고 표시만 하는 규칙인지 확인한다.
- UIManagementSystem이 게임 규칙이나 Score 계산을 소유하지 않는지 확인한다.
- Phase 4 범위 밖 저장, Leaderboard와 신규 아트 시스템이 포함되지 않는지 확인한다.

### 수동 작업

- 사용자는 AI가 제시하는 규칙별 장단점을 검토하고 표시 형식 및 최소 화면 범위를 선택한다.
- Unity Editor 작업은 없다.

### 완료 조건

- [ ] 모든 UI 표시 규칙이 확정되었다.
- [ ] 관련 Feature와 System 문서에 규칙이 반영되었다.

## Step 2. 현재 UI와 Scene 확장 지점을 정적으로 조사한다

- 진행 상태: **대기**

### AI 작업

- UIManagementSystem의 State, Mode, Text와 Button 책임을 조사한다.
- StageHUD, ResultPanel, PausePanel, UIRoot와 EventSystem 계층을 조사한다.
- GameSystem의 Runtime Data 전달 시점과 Result Data 전달 시점을 조사한다.
- 기존 ResultMenu, PauseMenu와 Mode별 통합 Test의 회귀 범위를 정한다.
- 신규·수정 파일, Serialized Field와 사용자 Scene 작업 후보를 확정한다.

### 정적 검증

- Scene fileID, 기존 참조, GameObject와 Component 기준 수를 기록한다.
- 기존 Input Action Asset과 생성 Wrapper 변경 필요 여부를 확인한다.
- 기존 Test 177/87개의 직접 영향 범위를 기록한다.

### 수동 작업

- 없음.

### 완료 조건

- [ ] 코드와 Scene 확장 지점이 확정되었다.
- [ ] 사용자 Scene 작업 범위가 구체화되었다.

## Step 3. UI 표시 문자열과 Mode별 표시 모델을 Unit Test로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Infinite 현재 거리 문자열
- Infinite 현재 Score 문자열
- Infinite 최종 거리 문자열
- Infinite 최종 Score 문자열
- 0, 경계값, 큰 값과 유효하지 않은 값 처리
- Stage Result와 Infinite Result의 데이터 혼용 방지

### AI 작업

- 순수 Formatter 또는 표시 모델 Test를 먼저 추가한다.
- 최소 생산 코드를 구현한다.
- UI 코드에서 거리와 Score를 다시 계산하지 않고 Runtime/Result 값을 전달하도록 한다.

### 수동 작업

- Unity Script Compilation과 관련 Edit Mode Test 실행만 사용자가 수행한다.

### 완료 조건

- [ ] 표시 형식 Unit Test가 통과한다.
- [ ] 기존 ResultTextFormatter와 ResultData Test가 통과한다.

## Step 4. Mode별 UI State 매핑을 Unit Test로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Stage Playing에서 StageHUD만 표시
- Infinite Playing에서 InfiniteHUD만 표시
- Stage Result에서 Stage Result Content만 표시
- Infinite Result에서 Infinite Result Content만 표시
- Pause에서 현재 Mode와 관계없이 PausePanel 표시
- Resume, Retry와 Initialize 시 UI State Reset
- Mode 변경과 ResultMenu·PauseMenu 선택 상태의 독립성

### AI 작업

- Unity GameObject에 불필요하게 의존하지 않는 Mode별 UI 매핑 상태를 먼저 Test한다.
- UIManagementSystem은 확정된 매핑 결과만 Scene Object에 반영하도록 한다.

### 수동 작업

- Unity Script Compilation과 관련 Edit Mode Test 실행만 사용자가 수행한다.

### 완료 조건

- [ ] Mode별 UI 매핑 Unit Test가 통과한다.
- [ ] PauseMenu와 ResultMenu Unit Test가 통과한다.

## Step 5. InfiniteMode HUD 갱신 흐름을 Play Mode Test로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Infinite Run 시작 시 거리와 Score 0 표시
- Runtime Data 갱신 후 HUD Text 갱신
- 후진 시 확정된 최대 거리와 Score 표시 유지
- Pause 동안 표시 값 불변
- Resume 후 기존 값부터 갱신
- Stage Mode에서 Infinite HUD 갱신 경로 비활성
- Retry 후 HUD 0으로 Reset

### 정적 검증

- UI가 InfiniteMode 거리와 Score 계산을 중복하지 않는지 확인한다.
- 정상 프레임마다 불필요한 로그와 객체 할당을 추가하지 않는지 확인한다.
- Runtime Data 읽기와 UI 표시 책임을 구분한다.

### 수동 작업

- Unity Script Compilation과 관련 Play Mode Test 실행만 사용자가 수행한다.

### 완료 조건

- [ ] Infinite HUD 통합 Test가 통과한다.
- [ ] Stage Mode UI 회귀 Test가 통과한다.

## Step 6. InfiniteMode Result 표시 흐름을 Play Mode Test로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Infinite 종료 후 Final Distance와 Final Score 표시
- Stage Result에서는 Clear Time만 표시
- Infinite Result에서는 Stage Clear Time을 표시하지 않음
- Retry 후 이전 Result Text와 Result Data 제거
- 서로 다른 두 Run의 Result 표시 독립성
- 공용 Retry와 Quit Button 선택 흐름 유지

### 정적 검증

- ResultData의 확정값을 그대로 표시하는지 확인한다.
- ResultPanel이 결과 생성과 기록 책임을 갖지 않는지 확인한다.
- Stage TimeRecord와 Infinite ScoreRecord가 혼용되지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation과 관련 Play Mode Test 실행만 사용자가 수행한다.

### 완료 조건

- [ ] Mode별 Result 표시 Test가 통과한다.
- [ ] 기존 ResultMenu 회귀 Test가 통과한다.

## Step 7. Keyboard와 Mouse의 전체 UI 입력 회귀를 자동 Test로 확정한다

- 진행 상태: **대기**

### Test 우선 항목

- Stage 및 Infinite HUD 상태에서 Pause 입력
- PausePanel Keyboard Navigate, Submit과 Cancel
- PausePanel Mouse Point와 Click
- Mode별 ResultMenu Keyboard와 Mouse 입력
- 빠른 중복 Submit·Click의 단일 실행
- UI State 전환 경계의 transient 입력 소비
- Result에서 Pause 거부

### 정적 검증

- 기존 Input Action Asset과 Wrapper를 재사용하는지 확인한다.
- 빠른 조작을 수동 검증으로 넘기지 않았는지 확인한다.
- 하나의 입력이 둘 이상의 실행 경로를 시작하지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation과 관련 Play Mode Test 실행만 사용자가 수행한다.

### 완료 조건

- [ ] Keyboard와 Mouse UI 입력 Test가 통과한다.
- [ ] 빠른 입력과 중복 실행 Test가 통과한다.

## Step 8. 생산 Scene UI 구조 Test와 사용자 작업 명세를 확정한다

- 진행 상태: **대기**

### AI 작업

- 생산 Scene 변경 전에 구조 Play Mode Test를 추가한다.
- 필요한 GameObject, Component, 이름, 계층과 Serialized Field를 명시한다.
- 기존 UIRoot, Canvas, EventSystem, Result Button과 Pause Button 재사용 범위를 명시한다.
- Scene 변경 전 YAML Object, Component와 참조 기준을 기록한다.

### 예상 최소 계층

```text
UIRoot
├─ StageHUD
├─ InfiniteHUD
│  └─ Canvas
│     ├─ DistanceText
│     └─ ScoreText
├─ ResultPanel
│  └─ Canvas
│     ├─ StageResultContent
│     │  └─ ClearTimeText
│     ├─ InfiniteResultContent
│     │  ├─ FinalDistanceText
│     │  └─ FinalScoreText
│     ├─ RetryButton
│     └─ QuitButton
└─ PausePanel
   └─ Canvas
      ├─ PauseTitle
      ├─ ResumeButton
      ├─ RetryButton
      └─ QuitButton
```

실제 계층은 기존 Scene 구조와 Test 우선 구현 결과를 확인한 뒤 확정하며, 중복 Canvas 또는 EventSystem을 만들지 않는다.

### 수동 작업

- 없음. 다음 Step의 Scene 작업을 위한 명세만 검토한다.

### 완료 조건

- [ ] 생산 Scene 구조 Test가 먼저 작성되었다.
- [ ] 사용자 Scene 작업 절차가 Inspector Field 단위로 확정되었다.

## Step 9. 사용자가 Mode별 UI를 Scene에 구성하고 연결한다

- 진행 상태: **대기**

### 사용자 Scene 작업

1. AI가 Step 8에서 확정한 계층과 정확한 이름으로 `InfiniteHUD` 및 Mode별 Result Content를 생성한다.
2. TextMeshPro Text를 사용해 Distance, Score, Final Distance와 Final Score 표시 Object를 생성한다.
3. StageResultContent에는 기존 ClearTimeText를 유지하고 InfiniteResultContent와 겹치지 않게 구성한다.
4. 기존 Result Retry·Quit Button은 공용으로 재사용하고 중복 Button을 추가하지 않는다.
5. PausePanel은 Resume, Retry와 Quit의 선택 표시가 명확하도록 최소 레이아웃을 정리한다.
6. 기존 EventSystem을 유지하고 새 EventSystem이 자동 생성되면 삭제한다.
7. UIManagementSystem의 신규 Serialized Field를 AI가 지정한 대응 Object에 연결한다.
8. 초기 활성 상태보다 Runtime UI State 매핑 결과를 우선하며, Inspector에 Missing Reference가 없게 한다.
9. Scene을 저장하고 닫았다가 다시 열어 계층과 Serialized Reference 유지를 확인한다.
10. Missing Script, Missing Reference와 중복 EventSystem이 없는지 확인한다.

### AI 후속 정적 검증

- 사용자가 저장한 Scene YAML의 이름, 계층, Component와 fileID를 검사한다.
- Phase 4 범위 밖 UI와 Component가 추가되지 않았는지 확인한다.
- 생산 Scene 구조 Test의 기대와 실제 Scene을 대조한다.

### 완료 조건

- [ ] 저장·재개방 후 모든 UI 참조가 유지된다.
- [ ] 생산 Scene 구조 Test가 통과한다.
- [ ] Scene에 Missing 또는 중복 UI 구성이 없다.

## Step 10. 전체 Compile과 자동 회귀 Test를 수행한다

- 진행 상태: **대기**

### AI 정적 검증

- 신규 Script와 Test `.meta` 및 GUID를 검사한다.
- Test Ignore, 삭제와 기대값 약화 여부를 검사한다.
- Scene Serialized Reference와 Mode별 UI 매핑을 검사한다.
- 정상 프레임 반복 로그와 범위 밖 기능 추가 여부를 검사한다.
- 관련 Feature, System과 Roadmap 문서가 구현과 일치하는지 확인한다.

### 사용자 자동 검증

1. Unity Editor에서 Script Compilation 성공을 확인한다.
2. 예상하지 않은 Compile Error와 Warning이 없는지 확인한다.
3. Unity Test Runner에서 전체 Edit Mode Test를 실행한다.
4. Unity Test Runner에서 전체 Play Mode Test를 실행한다.
5. Passed, Failed와 전체 Test 수를 기록한다.
6. Test 실행 중 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 완료 조건

- [ ] 전체 정적 검증이 통과한다.
- [ ] 전체 Edit Mode와 Play Mode Test가 통과한다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

## Step 11. 사용자가 Build와 최소 화면 검증을 수행한다

- 진행 상태: **대기**

### Build 전 AI 정적 확인

- Build Settings의 Scene 포함 여부와 현재 대상 Platform 설정을 정적으로 확인한다.
- Build에 필요한 Scene, Script와 Asset 참조 누락 가능성을 확인한다.
- 자동 Test로 확인한 상태, 수치와 빠른 입력을 수동 체크리스트에서 제외한다.

### 사용자 Build 작업

1. Unity Editor의 Build Settings에서 `SampleScene`이 활성 Scene으로 포함되어 있는지 확인한다.
2. Phase 4 Step 1에서 확정한 대상 Platform과 Build 설정을 사용한다.
3. 프로젝트 외부 또는 Git 추적 대상이 아닌 전용 Build 출력 폴더를 선택한다.
4. Unity Editor에서 Build를 한 번 실행한다.
5. Build 성공과 Build 과정의 예상하지 않은 Error·Warning 부재를 확인한다.
6. 생성된 Player를 실행하여 Stage Mode와 InfiniteMode의 HUD 및 Result 화면이 식별 가능한지 확인한다.
7. PausePanel과 공용 Result Button이 화면 밖으로 벗어나거나 겹치지 않는지 확인한다.
8. 종료 후 Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 수동 검증 제한

- 거리·Score 정확성, Mode 상태, Retry Reset, 빠른 UI 입력과 중복 실행은 자동 Test 결과를 사용한다.
- 수동으로는 Text 가독성, Object 겹침, 화면 잘림과 최종 Player 표시만 확인한다.
- 빠른 조작 속도나 정밀한 입력 타이밍을 요구하지 않는다.

### 완료 조건

- [ ] Build가 성공한다.
- [ ] Stage와 Infinite UI가 Build Player에서 식별 가능하다.
- [ ] UI 겹침, 잘림과 예상하지 않은 Error·Warning이 없다.

## Step 12. Phase 4 완료 근거를 정리한다

- 진행 상태: **대기**

### AI 작업

- 최종 정적 검증, Compile, Test 수와 Build 결과를 기록한다.
- 최소 화면 검증과 미해결 사항을 기록한다.
- 별도 Phase 4 Verification Result Task 문서를 작성한다.
- 모든 완료 조건 충족 시에만 Roadmap Phase 4를 `완료`로 변경한다.

### 수동 작업

- 이전 Step에서 확인하지 못한 새 수동 작업을 추가하지 않는다.

### 완료 조건

- [ ] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [ ] 최소 화면 검증 결과가 기록되어 있다.
- [ ] Phase 4 범위 밖 기능이 포함되지 않았다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 UI 표시 형식과 최소 범위 선택
2. 구현 Step 이후 Unity Script Compilation 결과 확인
3. AI가 지정한 관련 및 전체 Unity Test Runner 실행
4. Step 9의 Scene UI GameObject 생성, 배치와 Serialized Field 연결
5. Scene 저장·재개방과 Missing Reference 확인
6. Step 11의 Unity Build 실행
7. Build Player의 Text 가독성, 겹침, 잘림과 예상하지 않은 로그 확인

상태값, 수치, 호출 횟수, 빠른 입력, 중복 실행과 Retry 초기화는 수동 작업에 포함하지 않고 정적 검증 또는 자동 Test로 처리한다.

---

# 영향 범위

- UIManagementSystem과 GameSystem
- Runtime Data와 Result Data 표시 경로
- StageHUD, InfiniteHUD, ResultPanel과 PausePanel
- Keyboard와 Mouse UI 입력
- SampleScene
- Edit Mode 및 Play Mode Test
- 관련 Feature, System 문서와 Roadmap

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`
- `AI/90_Tasks/Prototype_2/20260902_01_Phase3VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 검증 결과

- Roadmap Phase 4 목표, 구현 대상과 완료 조건을 12개 실행 Step으로 분리했다.
- 정적 검증과 Unit Test를 생산 코드 및 Scene 수동 작업보다 앞에 배치했다.
- 빠른 입력과 상태 변화는 자동 Test 범위로 고정했다.
- Scene, Unity Compile/Test Runner, Build와 최종 화면 확인만 사용자 수동 작업으로 분리했다.
- Phase 4 구현은 아직 수행하지 않았다.

---

# 후속 작업

Step 1에서 Phase 4 UI 표시 규칙을 확정한다.
