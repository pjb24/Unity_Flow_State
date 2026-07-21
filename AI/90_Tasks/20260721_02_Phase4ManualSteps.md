# 작업 정보

## 작업명

Phase 4 Manual Steps

---

## 작업 일자

20260721

---

## 작업 담당자

AI

---

## 작업 상태

완료

---

# 작업 목적

Phase 4: 게임 플레이 결과 처리를 사용자가 Unity Editor에서 수동으로 구성하고 검증할 Step으로 정리한다.

Roadmap에 정의된 TimerSystem, ResultSystem과 TimeRecord의 책임 및 완료 조건을 기준으로 Scene 구성, 참조 연결, 결과 UI 구성, 실행 확인과 저장 순서를 명확하게 한다.

---

# 작업 대상

- Phase 4: 게임 플레이 결과 처리
- TimerSystem
- ResultSystem
- TimeRecord
- StageSystem과 GameSystem의 시작·종료 흐름
- SampleScene의 StageHUD와 ResultPanel
- Unity Test Runner와 Play Mode 수동 검증

---

# 작업 전 상태

- Roadmap의 Phase 1, Phase 2와 Phase 3은 완료 상태이다.
- SampleScene에서 일반 Stage를 시작하고 Goal 도달로 Stage를 클리어 및 종료할 수 있다.
- StageSystem은 Stage 시작, 클리어와 종료 이벤트를 각각 제공한다.
- GameSystem은 Stage 종료 이벤트를 수신하여 게임 종료와 Result UI State 전환을 수행한다.
- UIManagementSystem에는 StageHUD와 ResultPanel이 연결되어 있다.
- TimerSystem, ResultSystem과 TimeRecord 생산 코드는 아직 존재하지 않는다.
- ResultPanel은 존재하지만 확정된 클리어 시간을 표시하는 연결은 존재하지 않는다.
- Phase 4 수동 작업 순서는 별도 문서로 정리되어 있지 않았다.

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
- AI/02_Systems/GameSystem.md
- AI/02_Systems/StageSystem.md
- AI/02_Systems/TimerSystem.md
- AI/02_Systems/ResultSystem.md
- AI/02_Systems/UIManagementSystem.md
- AI/03_Features/README.md
- AI/03_Features/StagePlay.md
- AI/03_Features/StageClear.md
- AI/03_Features/TimeRecord.md
- AI/04_Implementation_Roadmap/README.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/90_Tasks/20260720_02_Phase3ManualSteps.md
- AI/90_Tasks/20260721_01_Phase3VerificationResult.md
- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

확인한 기준은 아래와 같다.

- TimerSystem은 요청받은 Timer의 생성, 시작, 일시정지, 재시작, 종료, 제거와 시간 제공만 담당한다.
- 기본 플레이 시간 측정에는 하나의 활성 Timer를 사용한다.
- TimerSystem은 Timer의 사용 목적을 판단하거나 기록 및 결과 데이터를 생성하지 않는다.
- ResultSystem은 Stage 종료 이벤트와 확정된 클리어 시간을 이용해 Result Data를 생성하고 제공한다.
- ResultSystem은 결과를 저장, 표시 또는 평가하지 않는다.
- TimeRecord는 일반 Stage가 클리어되고 클리어 시간이 확정된 경우 Stage Play당 한 번만 수행한다.
- UIManagementSystem은 Result Data를 생성하지 않고 전달받은 데이터를 Unity UI에 반영한다.
- Runtime Data는 게임 실행 중에만 사용하며 로컬 또는 서버 저장을 추가하지 않는다.
- 상태값과 실행 횟수는 Test Runner로 검증하고 최종 화면 구성만 수동으로 확인한다.

---

# 현재 구현 상태

Phase 3 생산 코드와 Test는 존재하며, StageSystem의 Stage 시작·클리어·종료 이벤트가 Phase 4의 연결 지점으로 준비되어 있다.

Phase 4 생산 코드와 Test는 아직 작성되지 않았다. 아래 Step은 Phase 4 구현이 완료된 후 Unity Editor에서 수행한다.

구현 후 실제 Component 이름, Inspector Field 또는 Result Data 구조가 이 문서의 표현과 다르면 임의로 비슷한 참조를 연결하지 않는다. 관련 System·Feature 문서와 생산 코드를 다시 확인하고 필요한 경우 이 문서를 먼저 갱신한다.

---

# 수동 작업 전 원칙

- 기존 SampleScene과 Phase 1~3 구성을 재사용한다.
- TimerSystem과 ResultSystem의 책임을 GameSystem, StageSystem 또는 UIManagementSystem에 중복 구현하지 않는다.
- Timer는 Stage 시작 이후에 시작하고 Stage 종료 시점에 한 번만 종료한다.
- Result Data는 Timer 종료로 확정된 시간과 Stage 종료 정보를 사용한다.
- ResultPanel은 전달받은 Result Data만 표시한다.
- Play Mode 중 변경한 Inspector 값은 저장되지 않으므로 참조 연결은 Edit Mode에서 수행한다.
- 생산 코드에 정의되지 않은 Timer Key, Stage Mode, 시간 형식 또는 임시 데이터를 임의로 만들지 않는다.
- 로컬 저장, 서버 저장, Leaderboard와 무한 모드 점수 처리를 추가하지 않는다.
- 자동 판정 가능한 시간 상태, 결과 생성 횟수와 중복 방지는 Test Runner 결과로 판단한다.

---

# Unity 수동 실행 Step

## Step 진행 상태

| Step | 작업 | 상태 |
| --- | --- | --- |
| 1 | Phase 4 구현·Compile·기준 상태 확인 | 완료 |
| 2 | TimerSystem 구성 | 완료 |
| 3 | ResultSystem 구성 및 게임 흐름 연결 | 완료 |
| 4 | Result UI 구성 및 데이터 표시 연결 | 완료 |
| 5 | Edit Mode와 Play Mode 자동화 Test 실행 | 완료 |
| 6 | 전체 플레이 결과 흐름 검증 | 완료 |
| 7 | 예외·재시작·Phase 1~3 회귀 검증 | 완료 |
| 8 | Scene 저장 및 검증 결과 기록 | 완료 |

## Step 1. Phase 4 구현·Compile·기준 상태를 확인한다

- 진행 상태: **완료**
- 확인 근거: Phase 4 생산 코드와 Test 작성 및 AI 정적 검사 완료
- 사용자 확인 근거: Unity Compile과 Build 성공, Console Error 및 Warning 없음, Edit Mode Test 36개와 Play Mode Test 22개 표시, 수동 확인 6개 항목 완료

### 구현과 Compile 확인

1. Phase 4 생산 코드와 관련 Test가 구현되었는지 확인한다.
2. Project 창에서 TimerSystem, ResultSystem과 Result Data 관련 Script가 존재하는지 확인한다.
3. Unity Script Compile이 완료될 때까지 기다린다.
4. Console에 Compile Error와 예상하지 않은 Warning이 없는지 확인한다.
5. `Add Component` 검색에서 TimerSystem과 ResultSystem이 표시되는지 확인한다.
6. Error 또는 Warning이 있으면 Scene 구성을 시작하지 않고 원인을 기록한다.

완료 조건:

- Phase 4 Component를 Scene에 추가할 수 있다.
- Console에 Compile Error와 예상하지 않은 Warning이 없다.
- Phase 4 Test가 Test Runner에 표시된다.

Step 1 확인 결과:

- TimerSystem, ResultSystem, TimeRecord, Timer Runtime Data와 Result Data 생산 코드를 작성했다.
- Timer 상태 및 시간 계산과 TimeRecord 1회 제한을 검증하는 Edit Mode Test를 작성했다.
- GameSystem에 Stage 시작 후 Timer 시작, Stage 종료 후 시간 확정과 Result Data 생성 순서를 연결했다.
- Unity Script Compile과 Build가 성공했다.
- Console에 별도 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 36개와 Play Mode Test 22개가 Test Runner에 표시됨을 확인했다.
- TimerSystem과 ResultSystem을 Add Component에서 검색할 수 있음을 확인했다.
- SampleScene의 기존 구성과 참조에 Missing Script가 없음을 확인했다.
- Step 1의 Unity Editor 수동 확인 6개 항목이 모두 성공했다.

### SampleScene과 Phase 3 기준 상태 확인

1. `Assets/Scenes/SampleScene.unity`를 연다.
2. Hierarchy에서 GameRoot, Systems, World, Stage_01, Player와 UIRoot를 확인한다.
3. GameSystem, StageSystem, UIManagementSystem, StageHUD와 ResultPanel이 유지되는지 확인한다.
4. StageSystem의 Stage Goal 참조와 GameSystem의 기존 필수 System 참조가 유지되는지 확인한다.
5. Inspector에 Missing Script 또는 끊어진 기존 참조가 없는지 확인한다.
6. Scene을 저장하고 Phase 4 Scene 수정 전 Git 상태를 확인한다.

완료 조건:

- Phase 3 Stage 시작부터 Goal 종료까지의 구성이 유지되어 있다.
- 기존 Component에 Missing Script나 누락된 필수 참조가 없다.
- Phase 4 변경 전 기준 상태를 확인했다.

## Step 2. TimerSystem을 구성한다

- 진행 상태: **완료**
- 확인 근거: TimerSystem 생산 코드와 SampleScene의 현재 Systems 계층 정적 검사 완료
- 사용자 확인 근거: Unity Editor 수동 작업 9개 항목과 사용자 Build 모두 성공

1. Hierarchy의 `GameRoot/Systems` 아래에 `TimerSystem` 오브젝트를 생성한다.
2. 생성한 오브젝트에 생산 코드의 TimerSystem Component를 추가한다.
3. TimerSystem에 Inspector 설정 항목이 있으면 생산 코드와 TimerSystem 문서에 정의된 값만 설정한다.
4. 기본 플레이 Timer Key가 코드에서 고정 또는 열거형으로 제공되면 해당 정의를 그대로 사용한다.
5. 여러 Timer가 필요하지 않은 현재 Phase에서 임의의 추가 Timer를 생성하거나 Scene에 중복 Component를 배치하지 않는다.
6. 오브젝트의 Transform에 의도하지 않은 Position, Rotation 또는 Scale이 없는지 확인한다.

최소 계층 기준:

```text
GameRoot
└─ Systems
   ├─ GameSystem
   ├─ StageSystem
   └─ TimerSystem
```

ResultSystem은 Step 3에서 별도로 추가한다.

완료 조건:

- Scene에 활성 TimerSystem이 하나만 존재한다.
- TimerSystem의 필수 Inspector 항목이 모두 연결되어 있다.
- 문서에 정의되지 않은 Timer가 추가되지 않았다.

AI 정적 확인 결과:

- SampleScene의 `GameRoot/Systems` 아래에 GameSystem과 StageSystem이 존재한다.
- SampleScene에는 TimerSystem 오브젝트와 Component가 아직 존재하지 않는다.
- 현재 TimerSystem에는 `[SerializeField]`로 노출된 Inspector 설정 항목이 없다.
- 기본 플레이 Timer Key는 `E_TimerKey.PlayTimer`로 생산 코드에 정의되어 있으므로 Inspector에서 별도 Key를 만들 필요가 없다.
- Timer의 생성과 시작은 GameSystem이 요청하므로 Unity Editor에서 Timer Runtime Data를 직접 생성하지 않는다.
- SampleScene에 활성 TimerSystem 오브젝트와 Component가 하나만 존재함을 정적으로 확인했다.
- TimerSystem Transform이 Position `(0, 0, 0)`, Rotation `(0, 0, 0)`, Scale `(1, 1, 1)`로 저장되었음을 확인했다.
- Scene의 TimerSystem Component가 생산 Script의 GUID를 참조함을 확인했다.
- Unity Editor 수동 작업 9개 항목과 사용자 Build가 모두 성공했다.

## Step 3. ResultSystem을 구성하고 게임 흐름을 연결한다

- 진행 상태: **완료**
- 확인 근거: ResultSystem, GameSystem 실행 흐름과 SampleScene의 Phase 4 참조 상태 정적 검사 완료
- 사용자 확인 근거: 최초 수동 작업 12개 및 사용자 Build 성공, 참조 재연결 작업 6개 완료

### ResultSystem Component 구성

1. `GameRoot/Systems` 아래에 `ResultSystem` 오브젝트를 생성한다.
2. 생성한 오브젝트에 생산 코드의 ResultSystem Component를 추가한다.
3. 현재 ResultSystem에는 Inspector 설정 항목이 없으므로 별도 참조를 추가하지 않는다.
4. Result Data의 저장 대상이나 외부 저장 경로를 새로 만들지 않는다.
5. 필수 참조가 아닌 임시 오브젝트나 가짜 결과 데이터를 연결하지 않는다.
6. ResultSystem 오브젝트의 Transform을 기본값으로 유지한다.

완료 조건:

- Scene에 활성 ResultSystem이 하나만 존재한다.
- ResultSystem에 문서와 생산 코드에 정의되지 않은 참조가 추가되지 않았다.
- 저장 또는 평가 책임이 ResultSystem에 추가되지 않았다.

### GameSystem과 Phase 4 System 참조 연결

1. Hierarchy에서 GameSystem 오브젝트를 선택한다.
2. Phase 4 구현으로 TimerSystem 또는 ResultSystem Inspector Field가 추가되었는지 확인한다.
3. 추가된 Field에 Step 2와 현재 Step에서 구성한 실제 Component를 연결한다.
4. 기존 RuntimeDataSystem, UIManagementSystem, Player 관련 System, StageSystem과 Camera 참조를 변경하지 않는다.
5. GameSystem이 Stage 종료 이벤트를 기준으로 결과 생성과 종료 절차의 실행 순서만 연결하는지 확인한다.
6. GameSystem이 직접 시간을 계산하거나 Result Data를 생성하도록 Scene 설정을 우회하지 않는다.

완료 조건:

- GameSystem의 모든 필수 참조가 연결되어 있다.
- 기존 Phase 1~3 참조가 유지되어 있다.
- GameSystem은 Phase 4 System의 실행 흐름만 연결한다.

### Stage 시작·종료와 Timer 연결 확인

1. StageSystem 또는 연결을 담당하는 생산 Component에서 Stage 시작 이벤트와 TimerSystem 연결 방식을 확인한다.
2. Stage가 실제로 시작된 뒤 기본 플레이 Timer가 생성 및 시작되도록 연결한다.
3. Goal 도달로 Stage가 클리어 및 종료될 때 같은 Timer가 한 번만 종료되도록 연결한다.
4. Stage 시작 전에 Timer가 증가하지 않는지 확인할 수 있도록 Inspector 또는 생산 코드의 공개 상태를 확인한다.
5. Stage 종료 후 Timer 시간이 더 증가하지 않는지 확인할 수 있도록 최종 시간 제공 경로를 확인한다.
6. StageSystem에 시간 계산이나 Result Data 생성 책임을 추가하지 않는다.

완료 조건:

- Stage 시작과 플레이 Timer 시작이 연결되어 있다.
- Stage 종료와 플레이 Timer 종료가 연결되어 있다.
- 하나의 Stage Play에서 기본 플레이 Timer가 한 번만 사용된다.

AI 정적 확인 결과:

- SampleScene에는 ResultSystem 오브젝트와 Component가 아직 존재하지 않는다.
- 현재 ResultSystem에는 `[SerializeField]`로 노출된 Inspector 설정 항목이 없다.
- GameSystem의 기존 Phase 1~3 참조는 유지되어 있다.
- GameSystem의 `_timerSystem`과 `_resultSystem`은 현재 미할당 상태이다.
- GameSystem은 Stage 시작 성공 후 `E_TimerKey.PlayTimer`를 생성 및 시작한다.
- GameSystem은 Stage 종료 시 Timer를 먼저 정지하고 클리어된 Stage에 한해 Result Data를 생성한다.
- GameSystem은 시간을 직접 계산하지 않고 TimerSystem이 제공한 최종 시간을 ResultSystem에 전달한다.
- 실제 실행 흐름과 중복 방지는 Step 5와 Step 6에서 Test Runner 및 Play Mode로 검증한다.
- 사용자의 Scene 구성, 참조 연결과 저장 확인이 완료되기 전에는 Step 3을 완료로 변경하지 않는다.

사용자 수행 후 확인 결과:

- Unity Editor 수동 작업 12개 항목과 사용자 Build가 성공했다는 결과를 전달받았다.
- SampleScene에 활성 TimerSystem과 ResultSystem이 각각 하나만 존재함을 정적으로 확인했다.
- ResultSystem Transform과 생산 Script 연결이 정상적으로 저장되었음을 확인했다.
- 저장된 SampleScene에서 GameSystem의 `_timerSystem`과 `_resultSystem`이 모두 `{fileID: 0}`으로 확인되었다.
- 두 Phase 4 참조가 Scene에 저장되지 않아 사용자에게 재연결과 Scene 재확인을 요청했다.
- 재연결 후 `_timerSystem`은 실제 TimerSystem Component를 참조한다.
- 재연결 후 `_resultSystem`은 실제 ResultSystem Component를 참조한다.
- Scene을 다시 저장하고 열어도 두 참조가 유지됨을 사용자가 확인했다.
- Step 3의 모든 완료 조건을 충족했다.

## Step 4. Result UI를 구성하고 데이터 표시를 연결한다

- 진행 상태: **완료**
- 확인 근거: 기존 ResultPanel과 TextMeshProUGUI 구성 확인, 사용자 표시 형식 승인, Result Data 표시 코드 및 Test 작성 완료
- 사용자 확인 근거: Unity Editor 수동 작업 14개 항목과 사용자 Build 모두 성공

### ResultPanel의 클리어 시간 표시 구성

1. UIRoot 아래의 기존 ResultPanel을 선택한다.
2. ResultPanel 안에 클리어 시간을 표시할 Text 또는 TextMeshPro UI가 있는지 확인한다.
3. 생산 코드가 요구하는 UI 타입과 일치하는 표시 Component를 재사용한다.
4. 표시 Component가 없을 때만 ResultPanel 아래에 클리어 시간 표시용 UI를 하나 생성한다.
5. 화면에서 클리어 시간임을 식별할 수 있는 Label과 값 영역을 구성한다.
6. 시간의 소수점 자리수와 표시 형식은 생산 코드 또는 관련 문서에 정의된 형식만 사용한다.
7. 승인된 `Clear Time: 12.345 s` 형식을 사용한다.
8. ResultPanel이 StageHUD와 동시에 겹쳐 보이지 않도록 기존 UI State 구성을 유지한다.

권장 계층 표현은 역할만 나타내며 실제 생산 코드의 Field와 일치시킨다.

```text
UIRoot
├─ StageHUD
└─ ResultPanel
   └─ ClearTimeText
```

완료 조건:

- ResultPanel에 클리어 시간을 표시할 UI Component가 존재한다.
- 해당 Component가 생산 코드에서 사용하는 UI 타입과 일치한다.
- 임의의 고정 결과값을 최종 표시값으로 사용하지 않는다.

### UIManagementSystem과 Result Data 표시 참조 연결

1. UIManagementSystem 오브젝트를 선택한다.
2. 기존 Stage HUD와 Result Panel 참조가 유지되는지 확인한다.
3. Phase 4 구현에서 클리어 시간 표시 Component 또는 Result Data 수신 대상 Field가 추가되었는지 확인한다.
4. 추가된 Field에 현재 Step에서 구성한 실제 UI Component를 연결한다.
5. ResultSystem 참조가 필요한 구조라면 Scene의 실제 ResultSystem을 연결한다.
6. UIManagementSystem이 Result Data를 생성하거나 클리어 시간을 다시 계산하지 않는지 확인한다.
7. Play Mode 진입 전 ResultPanel의 초기 활성 상태가 UIManagementSystem 초기화로 제어되는지 확인한다.

완료 조건:

- UIManagementSystem의 Phase 4 필수 참조가 모두 연결되어 있다.
- StageHUD와 ResultPanel의 기존 UI State 전환이 유지되어 있다.
- ResultPanel은 ResultSystem이 제공한 결과를 표시하도록 연결되어 있다.

AI 정적 확인 결과:

- 기존 ResultPanel 아래에 재사용 가능한 TextMeshProUGUI가 하나 존재한다.
- 기존 TextMeshProUGUI의 표시 문구는 `Result Panel`이다.
- 사용자가 초 단위, 소수점 셋째 자리의 `Clear Time: 12.345 s` 형식을 승인했다.
- ResultTextFormatter에 승인된 고정 표시 형식을 구현했다.
- UIManagementSystem에 Result Data 수신과 `_clearTimeText` 반영을 구현했다.
- GameSystem이 Result Data 생성 성공 후 UIManagementSystem에 데이터를 전달하도록 연결했다.
- 승인된 표시 형식의 반올림과 고정 소수점 자릿수를 검증하는 Edit Mode Test를 작성했다.
- ResultPanel의 기존 TextMeshProUGUI가 `Clear Time Text`로 저장되었음을 확인했다.
- UIManagementSystem의 `_clearTimeText`가 실제 `Clear Time Text`의 TextMeshProUGUI Component를 참조함을 확인했다.
- 기존 `_stageHud`, `_resultPanel`, `_timerSystem`과 `_resultSystem` 참조가 유지됨을 확인했다.
- ResultTextFormatterTests 3개가 Test Runner에 표시됨을 사용자가 확인했다.
- Unity Editor 수동 작업 14개 항목과 사용자 Build가 모두 성공했다.
- Step 4의 모든 완료 조건을 충족했다.

## Step 5. Edit Mode와 Play Mode 자동화 Test를 실행한다

- 진행 상태: **완료**
- 확인 근거: Phase 4 Test 범위 정적 검사, TimerSystem Play Mode Test 추가 및 StageGoal 통합 Test 확장 완료
- 사용자 확인 근거: Unity Compile과 Build 성공, Edit Mode 39개 및 Play Mode 24개 전체 통과, 수동 확인 9개 항목 완료

### Edit Mode Test 실행

1. `Window > General > Test Runner`를 연다.
2. Edit Mode 탭에서 전체 Test를 실행한다.
3. Timer의 생성, 시작, 일시정지, 재시작, 종료와 제거 상태 Test를 확인한다.
4. 일시정지 동안 시간이 증가하지 않는 계산 규칙 Test를 확인한다.
5. Result Data 생성과 TimeRecord의 1회 제한 Test를 확인한다.
6. 승인된 클리어 시간 표시 형식 Test를 확인한다.
7. 실패한 Test가 있으면 Scene 수동 검증으로 넘어가지 않고 실패 이름, 메시지와 Stack Trace를 기록한다.

완료 조건:

- Phase 4 관련 Edit Mode Test가 모두 통과한다.
- 기존 Edit Mode Test가 모두 통과한다.
- 예상하지 않은 Error 또는 Warning이 없다.

### Play Mode Test 실행

1. Test Runner의 Play Mode 탭에서 전체 Test를 실행한다.
2. 실제 Stage 시작으로 Timer가 시작되는 통합 Test를 확인한다.
3. Timer 생성, 시작, 일시정지, 재시작, 종료와 제거 요청 Test를 확인한다.
4. 동일 Timer Key 중복 생성 방지와 존재하지 않는 Key 요청 Test를 확인한다.
5. Goal 도달과 Stage 종료로 Timer가 종료되는 통합 Test를 확인한다.
6. 클리어 시간 확정, Result Data 생성과 Result UI 전환 순서 Test를 확인한다.
7. Stage 종료 이벤트가 반복되어도 결과가 한 번만 생성되는 Test를 확인한다.
8. 재시작 시 이전 플레이 결과가 새 플레이 시간에 섞이지 않는 Test를 확인한다.
9. 기존 GameLifecycle, StageSystem과 StageGoal 통합 Test가 모두 통과하는지 확인한다.

완료 조건:

- Phase 4 관련 Play Mode Test가 모두 통과한다.
- 기존 Play Mode Test가 모두 통과한다.
- Test 종료 후 Scene, Callback과 생성 객체가 정상적으로 정리된다.

AI 정적 확인 결과:

- TimerRuntimeDataTests가 시작, 일시정지, 재시작, 종료와 중복 시작 방지를 검증한다.
- TimeRecordTests가 일반 Stage 기록 생성, 미클리어 거부, 중복 기록 방지와 초기화를 검증한다.
- ResultTextFormatterTests가 승인된 표시 형식과 소수점 셋째 자리 반올림을 검증한다.
- TimerSystemTests를 추가하여 실제 Component의 전체 Timer 요청 순서와 중복·미존재 Key 방어를 검증한다.
- StageGoalIntegrationTests를 확장하여 Result Data 생성, 양수 클리어 시간과 Result UI 문자열을 검증한다.
- Stage 재시작 시 이전 Result Data가 초기화되는 검증을 추가했다.
- Unity Script Compile과 사용자 Build가 성공했다.
- Edit Mode Test 전체 `39 Passed, 0 Failed`를 확인했다.
- Play Mode Test 전체 `24 Passed, 0 Failed`를 확인했다.
- Test 실행 후 별도 Error와 예상하지 않은 Warning이 발생하지 않았다.
- Unity Editor 수동 확인 9개 항목이 모두 성공했다.
- Step 5의 모든 완료 조건을 충족했다.

## Step 6. 전체 플레이 결과 흐름을 검증한다

- 진행 상태: **완료**
- 확인 근거: SampleScene의 Phase 4 참조, 자동 Test 결과와 수동 관찰 가능 항목 정적 검사 완료
- 사용자 확인 근거: Unity Compile과 Build 성공, 실제 Play Mode 수동 확인 16개 항목 완료

### Stage 시작과 플레이 시간 측정 검증

1. Console을 비우고 SampleScene을 Play Mode로 실행한다.
2. 정상적으로 Player를 조작할 수 있고 Console에 `[GameSystem] Game started.`가 표시되는지 확인한다.
3. Stage 시작 후 최소 2초 이상 대기하고 대기 시간을 기록한다.
4. Player가 이동과 점프를 정상적으로 수행할 수 있는지 확인한다.
5. Stage 진행 중 ResultPanel이 비활성이고 StageHUD가 활성인지 확인한다.
6. Console에 Timer 중복 생성, 존재하지 않는 Key 또는 누락 참조 Warning이 없는지 확인한다.

완료 조건:

- Stage 시작 이후에만 플레이 시간이 증가한다.
- Stage 진행 중 기존 플레이와 StageHUD가 정상 동작한다.
- Phase 4 구성으로 인한 예상하지 않은 로그가 없다.

### Goal 도달, TimeRecord와 Result Data 검증

1. 일반 Stage에서 Player를 Goal까지 이동시킨다.
2. Goal 도달 시 StageClear와 Stage 종료가 각각 한 번 수행되는지 확인한다.
3. ResultPanel에 표시된 시간이 Step 시작 후 기다리고 플레이한 실제 시간보다 명백히 짧지 않은지 확인한다.
4. 표시된 클리어 시간을 기록한다.
5. Result 화면에서 1초 이상 기다린 뒤 표시 시간이 더 증가하지 않는지 확인한다.
6. Goal 진입 후 결과 화면 전환이나 결과 문자열이 반복 갱신되지 않는지 확인한다.

Timer 상태, TimeRecord 1회 제한, Result Data 생성 횟수와 데이터 일치는 Step 5의 자동 Test 통과 결과를 근거로 사용한다.

완료 조건:

- 일반 Stage 클리어 시간이 한 번만 확정된다.
- Result Data가 한 번만 생성된다.
- Result Data의 시간과 TimerSystem의 최종 시간이 일치한다.

### Result 화면 표시 검증

1. Stage 종료 후 UI State가 Result로 변경되는지 확인한다.
2. StageHUD가 비활성화되고 ResultPanel이 활성화되는지 확인한다.
3. ResultPanel에 클리어 시간이 표시되는지 확인한다.
4. 화면에 `Clear Time: 12.345 s`와 같은 승인된 형식으로 시간이 표시되는지 확인한다.
5. 시간이 잘리거나 UI 영역 밖으로 벗어나지 않는지 확인한다.
6. Result 화면이 표시된 상태에서 Player 이동이 중지되고 Player Action Map이 비활성인지 확인한다.
7. Console에 결과 데이터 또는 UI 참조 누락 Error와 Warning이 없는지 확인한다.

완료 조건:

- 결과 화면에서 확정된 클리어 시간을 확인할 수 있다.
- StageHUD와 ResultPanel이 동시에 표시되지 않는다.
- 게임 종료 후 플레이 입력과 이동이 중지된다.

AI 정적 확인 결과:

- SampleScene의 GameSystem에 TimerSystem과 ResultSystem이 연결되어 있다.
- UIManagementSystem에 StageHUD, ResultPanel과 Clear Time Text가 연결되어 있다.
- Step 5에서 Edit Mode 39개와 Play Mode 24개 전체 통과가 확인되었다.
- Timer 내부 상태와 Result Data는 Inspector 직렬화 대상이 아니므로 수동 관찰을 위해 임시 Debug Field를 추가하지 않는다.
- 내부 상태·횟수·데이터 일치는 자동 Test로 판단하고, Step 6은 실제 플레이 감각과 사용자 화면 결과를 확인한다.
- Unity Script Compile과 사용자 Build가 성공했다.
- Stage 진행 중 StageHUD 표시, ResultPanel 비활성과 Player 이동·점프 정상 동작을 확인했다.
- 최소 2초 대기 후 Goal에 도달하여 승인된 형식의 클리어 시간이 표시됨을 확인했다.
- 결과 화면에서 시간이 고정되고 Player 입력과 이동이 중지됨을 확인했다.
- 결과 화면과 결과 문자열이 반복 전환 또는 갱신되지 않음을 확인했다.
- 별도 Error와 예상하지 않은 Warning이 발생하지 않았다.
- Unity Editor 수동 확인 16개 항목이 모두 성공했다.
- Step 6의 모든 완료 조건을 충족했다.

## Step 7. 예외·재시작·Phase 1~3 회귀를 검증한다

- 진행 상태: **완료**
- 확인 근거: GameSystem 재시작 흐름, Phase 4 방어 Test와 Phase 1~3 회귀 Test 정적 검사 완료
- 사용자 확인 근거: Unity Compile과 Build 성공, 예외·재시작·Phase 1~3 회귀 수동 확인 24개 항목 완료

### 중복 처리와 예외 상태 검증

1. SampleScene을 Play Mode로 실행하고 Stage 진행 중 ResultPanel이 표시되지 않는지 확인한다.
2. Goal에 도달한 뒤 이동 입력을 계속 유지해도 결과 화면과 표시 시간이 반복 갱신되지 않는지 확인한다.
3. 결과 화면에서 2초 이상 기다려도 표시 시간이 고정되어 있는지 확인한다.
4. Console에 Timer 중복, 존재하지 않는 Key, 중복 Result Data 또는 반복 종료 Warning이 없는지 확인한다.
5. Project와 Scene에 저장, Leaderboard 또는 무한 모드 점수용 Component와 임시 데이터가 추가되지 않았는지 확인한다.

Stage 시작 전 요청 방어, 미존재 Timer Key, 미클리어 TimeRecord 거부와 중복 기록 방지는 Step 5의 자동 Test 통과 결과를 근거로 사용한다.

완료 조건:

- 하나의 Stage Play에서 시간 확정과 결과 생성이 각각 한 번만 수행된다.
- 클리어되지 않은 Stage에는 TimeRecord가 생성되지 않는다.
- Phase 4 범위 밖의 저장, Leaderboard 또는 무한 모드 점수 처리가 없다.

### 재시작과 Phase 1~3 회귀 검증

1. 첫 결과 화면을 표시한 상태에서 Hierarchy의 GameSystem 오브젝트를 선택한다.
2. GameSystem Component의 Context Menu에서 `Start Game`을 실행한다.
3. ResultPanel이 비활성화되고 StageHUD가 다시 활성화되는지 확인한다.
4. Player가 StartPoint로 돌아가고 다시 이동할 수 있는지 확인한다.
5. 첫 번째 플레이와 다른 시간 동안 대기한 뒤 이동, 점프, 일반 착지와 관성 착지를 수행한다.
6. Camera Follow가 Player의 수평 이동을 정상적으로 추적하는지 확인한다.
7. Ground와 Stage 지형 충돌이 정상이고 Player가 지형을 통과하지 않는지 확인한다.
8. 두 번째 Goal 도달 시 새로운 클리어 시간이 한 번 표시되는지 확인한다.
9. 두 번째 시간이 첫 번째 결과를 이어서 증가시킨 값이 아니라 새 Stage Play의 독립된 시간인지 확인한다.
10. Play Mode를 종료한 뒤 다시 실행한다.
11. 세 번째 실행에서도 StageHUD, Player 시작 위치와 조작 상태가 정상 초기화되는지 확인한다.
12. Console에 예상하지 않은 Error와 Warning이 없는지 확인하고 Play Mode를 종료한다.

완료 조건:

- 반복 실행 시 이전 Runtime Data가 남지 않는다.
- 두 번째 Stage Play도 독립된 클리어 시간을 가진다.
- Phase 1~3 핵심 기능에 회귀가 없다.

AI 정적 확인 결과:

- GameSystem의 `Start Game` Context Menu가 Runtime Data, UI, Result Data, Stage, Timer, Player와 Camera를 다시 초기화한다.
- StageGoalIntegrationTests가 Goal 종료 후 같은 Play Mode 세션에서 새 Stage Play 복구와 이전 Result Data 초기화를 검증한다.
- GameLifecycleIntegrationTests가 종료 후 재시작 시 Runtime Data와 Phase 2 System 복구를 검증한다.
- TimerSystemTests와 TimeRecordTests가 중복 Timer, 미존재 Key, 미클리어 기록과 중복 기록 방어를 검증한다.
- Edit Mode 39개와 Play Mode 24개 전체 통과로 이동, 점프, 착지, Camera, 충돌과 Stage 회귀 범위를 자동 검증했다.
- 프로젝트에는 저장, Leaderboard 또는 무한 모드 점수 생산 코드가 추가되지 않았다.
- Unity Script Compile과 사용자 Build가 성공했다.
- Goal 도달 후 결과 화면과 시간이 반복 갱신되지 않고 고정됨을 확인했다.
- 같은 Play Mode 세션의 `Start Game` 재시작에서 UI, Player, Stage, Timer와 Result 상태가 초기화됨을 확인했다.
- 두 번째 Stage Play의 클리어 시간이 첫 번째 결과와 독립적으로 측정됨을 확인했다.
- Play Mode 종료 후 재실행에서도 StageHUD, Player 시작 위치와 조작 상태가 정상 초기화됨을 확인했다.
- 이동, 점프, 일반 착지, 관성 착지, Camera Follow와 지형 충돌에 회귀가 없음을 확인했다.
- 저장, Leaderboard 또는 무한 모드 점수용 임시 Component와 데이터가 추가되지 않았음을 확인했다.
- 별도 Error와 예상하지 않은 Warning이 발생하지 않았다.
- Unity Editor 수동 확인 24개 항목이 모두 성공했다.
- Step 7의 모든 완료 조건을 충족했다.

## Step 8. Scene을 저장하고 검증 결과를 기록한다

- 진행 상태: **완료**
- 확인 근거: SampleScene의 Phase 4 오브젝트·참조와 Git 변경 범위 정적 검사 완료
- 사용자 확인 근거: 최종 Unity Compile과 Build 성공, Scene 저장·재열기 수동 확인 10개 항목 완료

1. Play Mode를 종료한다.
2. TimerSystem, ResultSystem, GameSystem과 UIManagementSystem의 참조가 Edit Mode에 유지되는지 확인한다.
3. SampleScene을 저장한다.
4. Scene을 닫았다가 다시 열어 Phase 4 Component와 참조가 유지되는지 확인한다.
5. Console을 다시 확인하여 예상하지 않은 Error와 Warning이 없는지 확인한다.
6. Git 변경 목록에서 Phase 4 범위 밖의 Scene, 설정 또는 Asset 변경이 없는지 확인한다.
7. 실제 Test 수, 성공·실패 결과와 수동 확인 결과를 별도의 Phase 4 검증 결과 Task 문서에 기록한다.
8. 모든 검증이 완료된 뒤 Roadmap의 Phase 4 상태를 갱신한다.

완료 조건:

- SampleScene 저장 후 모든 Phase 4 참조가 유지된다.
- 변경 범위가 Phase 4에 한정되어 있다.
- 자동 및 수동 검증 결과가 확인 가능한 근거로 기록된다.

AI 정적 확인 결과:

- SampleScene에 활성 TimerSystem과 ResultSystem이 각각 하나만 존재한다.
- GameSystem의 `_timerSystem`과 `_resultSystem`이 실제 생산 Component를 참조한다.
- UIManagementSystem의 `_stageHud`, `_resultPanel`과 `_clearTimeText`가 실제 Scene Component를 참조한다.
- ResultPanel 아래에 `Clear Time Text` TextMeshProUGUI가 하나 존재한다.
- 변경 목록은 Phase 4 문서, 생산 코드, Test와 SampleScene으로 한정되어 있다.
- ProjectSettings에는 내용 변경이 없다.
- Unity가 생성한 Scene YAML의 빈 `m_Name` 값에 trailing whitespace가 있으나 Missing Script 또는 누락 참조는 아니다.
- Edit Mode 39개와 Play Mode 24개 전체 통과 결과가 확인되어 있다.
- Step 1부터 Step 7까지 사용자 Compile, Build 및 수동 검증 성공 결과가 기록되어 있다.
- 최종 Unity Script Compile과 사용자 Build가 성공했다.
- Scene 저장과 재열기 후 모든 Phase 4 Component와 Inspector 참조가 유지됨을 확인했다.
- Inspector에 Missing Script와 미할당 Phase 4 필드가 없음을 확인했다.
- 별도 Error와 예상하지 않은 Warning이 발생하지 않았다.
- Unity Editor 수동 확인 10개 항목이 모두 성공했다.
- Phase 4 검증 결과를 `20260721_03_Phase4VerificationResult.md`에 기록했다.
- Step 8과 Phase 4의 모든 완료 조건을 충족했다.

---

# 수동 검증 체크리스트

- [x] Unity Script Compile Error와 예상하지 않은 Warning이 없다.
- [x] TimerSystem과 ResultSystem이 Scene에 각각 하나만 존재한다.
- [x] 모든 Phase 4 Inspector 참조가 실제 생산 Component에 연결되어 있다.
- [x] Stage 시작 후 플레이 시간이 증가한다.
- [x] Stage 종료 후 플레이 시간이 증가하지 않는다.
- [x] 일반 Stage 클리어 시간이 한 번만 확정된다.
- [x] Result Data가 Stage Play당 한 번만 생성된다.
- [x] Result Data의 시간과 TimerSystem의 최종 시간이 일치한다.
- [x] StageHUD에서 ResultPanel로 정상 전환된다.
- [x] ResultPanel에 확정된 클리어 시간이 표시된다.
- [x] Stage 종료 후 Player 입력과 이동이 중지된다.
- [x] 재실행 시 이전 Timer와 Result Data가 남지 않는다.
- [x] Edit Mode Test 전체가 통과한다.
- [x] Play Mode Test 전체가 통과한다.
- [x] Phase 1~3 핵심 기능에 회귀가 없다.
- [x] 로컬 저장, 서버 저장, Leaderboard 또는 무한 모드 점수 처리가 추가되지 않았다.

---

# 문제 발생 시 확인 순서

| 문제 | 먼저 확인할 항목 |
| --- | --- |
| Timer가 시작되지 않음 | Stage 시작 성공 여부, Stage 시작 연결, Timer Key와 TimerSystem 참조 |
| Timer가 종료되지 않음 | Stage 종료 이벤트, Timer 종료 연결, 중복 종료 방어 상태 |
| 시간이 계속 증가함 | Timer 실행 상태, 종료 요청 처리, 일시정지·종료 시간 계산 |
| Result Data가 없음 | Stage 종료 이벤트 수신, Timer 최종 시간 확정 순서, ResultSystem 참조 |
| Result Data가 여러 번 생성됨 | Stage 종료 이벤트 중복, Result 생성 상태, TimeRecord 1회 제한 |
| 결과 시간이 0 또는 다름 | Timer 시작 시점, 종료 시점, Result Data에 전달된 최종 시간 |
| ResultPanel이 보이지 않음 | GameSystem의 Result UI State 요청, UIManagementSystem의 ResultPanel 참조 |
| 결과 텍스트가 갱신되지 않음 | Result Data 전달, 표시 Component 참조, UI 갱신 호출 순서 |
| Missing Reference 발생 | Scene 저장 상태, Inspector Field, Script 재컴파일 후 참조 유지 여부 |
| 재시작 시 이전 결과가 남음 | Timer 제거·초기화, Result Data 초기화, Runtime Data 생명주기 |
| 기존 플레이가 동작하지 않음 | GameSystem 필수 참조, Player Action Map, Stage와 Camera 초기화 순서 |

문제가 발생하면 Console 메시지, 실패한 Test 이름, 실패 메시지와 Stack Trace를 먼저 기록한다. 확인되지 않은 원인을 추측하여 Scene 참조나 생산 코드를 임의로 변경하지 않는다.

---

# 영향 범위

## System

- GameSystem
- StageSystem
- TimerSystem
- ResultSystem
- UIManagementSystem
- RuntimeDataSystem

## Feature

- StagePlay
- StageClear
- TimeRecord

## Scene

- Assets/Scenes/SampleScene.unity

---

# 검증 내용

- Phase 4 관련 문서와 현재 생산 코드의 연결 지점을 확인했다.
- SampleScene에 StageHUD와 ResultPanel이 존재하고 UIManagementSystem에 연결된 상태를 확인했다.
- TimerSystem, ResultSystem과 Result Data 표시 연결이 아직 구현되지 않은 상태를 확인했다.
- Unity Editor에서 필요한 Scene 구성, 참조 연결, Test Runner 실행과 Play Mode 확인 항목을 8개 Step으로 통합하여 정리했다.

---

# 검증 결과

- Phase 4 수동 작업 문서 작성이 완료되었다.
- 문서의 모든 실행 Step은 아직 수행 전이므로 `대기` 상태이다.
- Unity Compile, Test Runner와 Play Mode 결과는 Phase 4 구현 후 실제 실행 결과로 갱신해야 한다.

---

# 후속 작업

1. TimerSystem, ResultSystem과 TimeRecord를 문서 기준으로 구현한다.
2. Phase 4 변경 책임을 검증하는 Edit Mode 및 Play Mode Test를 작성한다.
3. 이 문서의 Step 1부터 Step 8까지 Unity Editor에서 순서대로 수행한다.
4. 실제 검증 결과를 Phase 4 검증 결과 Task 문서에 기록한다.
5. Phase 4 완료 조건 충족 후 Roadmap 상태를 갱신한다.

---

# 관련 문서

## Project

- AI/00_Project/PROJECT_OVERVIEW.md
- AI/00_Project/ARCHITECTURE.md
- AI/00_Project/PROJECT_MEMORY.md

## Rules

- AI/01_Rules/AI_RULE.md
- AI/01_Rules/CODING_STYLE.md
- AI/01_Rules/EVENT_RULE.md
- AI/01_Rules/LOGGING_RULE.md
- AI/01_Rules/INVESTIGATION_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md

## Systems

- AI/02_Systems/GameSystem.md
- AI/02_Systems/StageSystem.md
- AI/02_Systems/TimerSystem.md
- AI/02_Systems/ResultSystem.md
- AI/02_Systems/UIManagementSystem.md
- AI/02_Systems/RuntimeDataSystem.md

## Features

- AI/03_Features/StagePlay.md
- AI/03_Features/StageClear.md
- AI/03_Features/TimeRecord.md

## Roadmap

- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md

---

# 관련 작업 기록

- AI/90_Tasks/20260720_02_Phase3ManualSteps.md
- AI/90_Tasks/20260721_01_Phase3VerificationResult.md
