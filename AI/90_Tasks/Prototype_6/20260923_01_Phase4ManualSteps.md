# 작업 정보

## 작업명

Prototype 6 Phase 4 — How To Play, 첫 플레이 안내 및 전체 UI 흐름 수동 작업 계획

## 작업 일자

20260923

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. Phase 4 구현과 아래 Step 수행은 대기 상태다. 각 Step은 실제 수행 근거가 확보된 뒤 완료 처리한다.

# 작업 목적

Roadmap 006 Phase 4를 실행할 순서와 사용자 수동 작업을 정의한다. 상태·수치·연결은 정적 검사와 Unit/통합 Test로 검증하고, 사용자는 제품 규칙 결정, Scene 편집, Unity 검증 실행 및 실제 화면·입력 확인을 수행한다.

# 작업 대상

- 기존 How To Play 화면과 Main Menu 진입·복귀
- 첫 Run 전 안내, 건너뛰기, 선택 Mode 보존 및 Run 시작 연결
- 자동 이동, Jump, Momentum Landing, Collectible, Pause와 Mode별 목표 안내
- 실제 Player/UI Input Binding을 반영하는 Keyboard·Gamepad 조작 표시
- Navigation 상태, GameSystem, UIManagementSystem, PlayerInputSystem, UIInputSystem 및 Settings 연계
- Edit Mode Unit Test, 생산 Scene 기반 Play Mode Test, Scene 정적 검사
- 사용자가 편집할 `Assets/Scenes/SampleScene.unity`

별도 Tutorial Stage, 영구 저장, 실제 Leaderboard 통신은 포함하지 않는다. Quit 종료 지연은 Phase 3에서 인계된 별도 관찰 사항이며 이 작업에서 원인을 확정하거나 수정하지 않는다.

# 작업 전 상태

- Roadmap 006 Phase 1~3 및 Phase 3 Step 1~10은 완료 상태다.
- 사용자 보고 기준 Script Compilation 성공, Edit Mode 692개·Play Mode 226개 성공, 예상하지 않은 Error/Warning 없음이 마지막 검증 기준이다. Phase 4 성공 결과로 재사용하지 않는다.
- `HowToPlay.md`에는 Main Menu 재열람·복귀와 최초 Run 전 자동 안내 규칙이 있다. 단말기별 완료 저장은 Roadmap 7 범위다.
- `GameNavigationState.SubmitModeSelect()`는 Mode 선택 후 `BeginInitialization()`을 호출한다. 첫 안내를 거치는 생산 상태는 아직 없다.
- `GameSystem.SelectHowToPlay()`와 `UIManagementSystem`의 How To Play Root·Back 참조가 있다. Scene에는 `How To Play is in preparation.` 문구가 남아 있다.
- Settings는 Keyboard Jump, Momentum Landing, Navigate 4방향을 재지정한다. Submit·Cancel은 고정 입력이며 Player Move는 제거됐다.
- Player/UI System에는 실제 Binding Action 조회 경계가 있고 Settings에는 Binding 표시 경로 조회가 있다. 안내 전용 입력 Asset 복제본을 새로 만들 필요가 있는지부터 검토해야 한다.
- Gamepad는 사용자 장치 미보유로 Phase 3 실제 장치 검증에서 제외됐다. Phase 4에서도 자동 검증과 실제 장치 검증을 구분한다.

# 조사 내용

`AI/README.md`의 Project·Rules 확인 순서와 Roadmap 006, Phase 3 인계, HowToPlay·GameEntryNavigation Feature, GameSystem·UIManagementSystem·UIInputSystem 문서를 확인했다. Navigation 코드, GameSystem 진입 흐름, Binding 조회 경계, 기존 Test 목록과 Scene의 안내 자리표시자를 정적으로 확인했다.

이 문서는 여러 System·Feature의 구현과 수동 Scene 편집을 조정하는 실행 계획이므로 GENERAL_TASK_TEMPLATE을 사용한다. 세부 API·Scene 값은 구현 전에 임의로 확정하지 않고 Step 4에서 실제 코드 기준의 단일 편집표로 제공한다.

# 작업 내용

AI는 코드·Test·문서 작성과 정적 검사를 담당한다. Unity Editor·Script Compilation·Test Runner·Build 실행은 사용자 작업이다. AI는 Scene/Prefab YAML을 편집하거나 이를 수정하는 Editor Script를 만들지 않는다. 기존 작업 트리의 Phase 3 변경은 보존한다.

## Step 1. 첫 안내와 조작 설명의 미정 규칙을 확정한다

### AI 작업

- 기존 Feature와 생산 코드에서 확정 규칙 및 미정 규칙을 구분한다.
- 아래 제안을 장단점과 함께 사용자에게 제시하고, 선택 결과를 계약으로 기록한다. 아래 표는 승인 전 제안이며 확정된 제품 규칙이 아니다.
- 안내 문구는 실제 자동 이동·관성 착지·Score 규칙과 대조한다. 제거한 Move 조작이나 확인되지 않은 속도 증가 조작을 안내하지 않는다.

| 결정 대상 | 제안 | 결정할 내용 |
|---|---|---|
| 자동 안내 횟수 | Application 실행 중 최초 Run 전 1회, 두 Mode 공통 | Mode별 횟수 여부, 새 Application에서 초기화됨을 명시 |
| 안내 구성 | 기존 How To Play 화면을 공용으로 재사용 | 한 화면 또는 여러 페이지, 최종 문구와 정보 순서 |
| 완료·Skip | 선택한 Mode의 Run을 한 번 시작하고 이번 실행의 안내 처리 완료로 기록 | Skip도 재표시를 막는지, 완료 기록 시점 |
| 자동 안내의 Back·Cancel | Run 없이 Mode Select로 복귀하고 선택한 Mode 복원 | 복귀 시 안내 처리 여부, 다음 Mode 선택 시 재표시 여부 |
| Main Menu 재열람 | Back·Cancel로 How To Play 항목에 복귀하고 Run을 시작하지 않음 | 재열람이 최초 자동 안내 처리 여부에 영향을 주는지 |
| 기본 Focus | 재열람은 Back, 자동 안내는 시작/완료 항목 | Skip·Back 포함 순서와 화면별 버튼 표시 |
| Binding 표기 | Keyboard·Gamepad를 함께 표시 | 동시 표시 또는 현재 장치 표시, 장치 미연결 시 표기 |
| Run 초기화 실패 | 기존 실패 복귀 경로 유지 | 안내 완료 후 실패·재시도에서 자동 안내 재표시 여부 |

### 사용자 수동 작업

1. AI가 제시한 각 규칙과 안내 문구를 검토하고 선택·수정한다.
2. 승인 결과를 전달한다. 이 Step에서는 Unity 편집을 하지 않는다.

### 완료 조건

- [ ] 표시 횟수, 진입 출처, 완료·Skip·Cancel, 실패 재시도, Focus 및 표시 문구가 확정됐다.

## Step 2. 상태 계약과 자동 검증 명세를 작성한다

### AI 작업

- HowToPlay와 GameEntryNavigation의 관계를 정리한다. Mode 선택 후 안내가 필요한 경우 Run 요청을 보류한다는 예외를 문서에 명시한다.
- 안내 상태, 보류된 Mode, 이번 Application의 안내 처리 여부를 단일 책임으로 관리한다. Run 데이터 제거·Retry가 안내 처리 상태를 잘못 초기화하지 않게 한다.
- GameSystem은 시작 순서, UIManagementSystem은 표시·Focus, 입력 System은 실제 Action 조회·입력을 담당하도록 기존 경계를 유지한다.
- 입력 소비와 전이를 관측 가능한 State로 정의한다. 고정 프레임 대기나 임의 지연으로 중복 시작을 막지 않는다.

| 대상 | 검증 계층 | 필수 검증 |
|---|---|---|
| 최초 안내 | Edit Mode | Stage/Infinite 최초 진입, 승인된 횟수, 재열람과 자동 안내 구분 |
| 완료·Skip·취소 | Edit Mode | 보류 Mode 보존, Cancel 복귀 Focus, 승인된 처리 여부, 중복 요청 거부 |
| 반복 진입 | Edit Mode | Retry·Main Menu 복귀·Mode 변경·새 세션·초기화 실패의 안내 상태 |
| Binding 표시 | Edit Mode 및 Input System 경계 Test | 기본값·Override·복원, Keyboard/Gamepad 구분, 고정 Submit/Cancel, 누락 처리 |
| 안내 중 진행 차단 | Play Mode | Run 데이터 미생성, Stage·Timer 미시작, Player 입력 비활성, UI 입력 활성 |
| 안내 종료 후 시작 | Play Mode | 선택한 Mode로 한 번만 시작, 정상 Action Map·HUD, 완료 입력이 Jump/Pause로 재사용되지 않음 |
| 실제 표시·Focus | Play Mode | 재열람 시 최신 Binding, Back 복귀 항목, Keyboard 선택·Mouse Hover·Click 경로 |
| Scene 연결 | 정적 YAML 및 Play Mode 구성 Test | Root·Component·직렬화 참조·OnClick·Navigation·초기 활성·Raycast |
| 기존 흐름 | 기존 및 확장 Edit/Play Mode | Menu·두 Mode·Pause·Settings·Rebind·Result·Retry·Main Menu 회귀 |

### 사용자 수동 작업

없음. AI가 문서와 자동 검증 명세를 작성하고 대조한다.

### 완료 조건

- [ ] 확정 규칙의 상태 전이와 정상·거부·경계 사례가 Test 명세에 연결됐다.

## Step 3. 순수 상태와 Unit Test를 구현한다

### AI 작업

- 승인된 안내 상태와 시작 보류를 기존 Navigation 구조에 구현한다.
- Scene·프레임에 의존하지 않는 Edit Mode Test로 Step 2 상태 표를 검증한다.
- Binding 표시의 순수 변환은 Unit Test로, 실제 Action/Override 조회는 Input System 경계 Test로 구분한다. 기본 키 문자열을 정답 원천으로 하드코딩하지 않는다.
- 기존 `GameNavigationStateTests`에서 Mode 선택 즉시 시작을 전제로 한 Test를 새 계약에 맞게 조정한다. 최초 안내를 우회하는 테스트 편의 경로로 생산 흐름 검증을 대체하지 않는다.
- 기존 Settings 재지정 범위와 실제 Gamepad Binding을 보존한다.

### 사용자 수동 작업

없음.

### 완료 조건

- [ ] 상태·횟수·취소·중복·실패 경계를 검증하는 Unit Test 코드가 작성되고 정적 검사를 통과했다.

## Step 4. 생산 연결·통합 Test와 단일 Scene 편집표를 준비한다

### AI 작업

- Menu에서 여는 안내와 Run 전 자동 안내를 생산 흐름에 연결하고, 안내 중에는 Run 생성을 보류한다.
- 실제 사용하는 Player/UI Action의 현재 Binding을 표시하며 Settings 변경·복원 후 재열람에도 반영한다.
- 생산 Scene 통합 Test를 작성하고 기존 `GameLifecycleIntegrationTests`, `ModeUISceneConfigurationTests`, Settings Test 등의 시작 전제를 점검한다.
- 입력 Test는 가상 장치에서 누름·유지·뗌을 수행하고 상태 조건과 최대 대기 시간으로 결과를 판정한다. 장치·Input 설정·Callback·Scene 상태는 종료 시 복구한다. 단독 실행뿐 아니라 전체 실행 간 오염을 검증한다.
- 기존 Player 직렬화 속도를 사용한다. 안내 검증을 위해 속도·전역 시간 배율을 변경하지 않는다.
- 이 문서에 다음 값을 갖춘 하나의 확정 Scene 편집표를 추가한다: 경로, 생성 타입, Component, Anchor/Pivot/위치/크기, 색상, 글꼴 크기·정렬·줄바꿈, 초기 활성, Raycast, Navigation, 직렬화 필드, 연결 대상, OnClick의 실제 메서드.
- 기존 Settings/Pause 다크 패널 스타일을 재사용하고 최종 문구와 페이지 수에 맞춰 편집표를 한 번에 제공한다. 공용 영역과 진입 출처별 버튼 영역을 구분한다.

### 사용자 수동 작업

없음. 아직 Scene을 수정하지 않는다.

### 완료 조건

- [ ] 생산 연결과 통합 Test가 작성됐다.
- [ ] 실제 API와 일치하는 Scene 편집표가 본 문서에 추가됐다. 이 조건 전에는 Step 6을 시작하지 않는다.

## Step 5. Scene 편집 전 컴파일과 Unit Test를 확인한다

### AI 작업

- C# 참조·asmdef·입력 Asset/Wrapper 일치, 직렬화 필드, 문서 계약 및 diff 공백 오류를 정적으로 검사한다.
- 사용자에게 Scene 편집 없이 실행 가능한 집중 Edit Mode Fixture의 실제 이름을 제공한다. Test 개수는 구현 후 산정하고 예상과 실제 실행 결과를 구분한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료 후 예상하지 않은 Error/Warning이 없는지 확인한다.
2. Test Runner의 EditMode에서 AI가 지정한 집중 Fixture를 실행한다.
3. 실행·성공·실패 개수와 오류 로그를 전달한다. 실패 시 AI 수정 후 해당 검증을 다시 수행한다.

### 완료 조건

- [ ] 컴파일과 집중 Unit Test가 성공하고 예상하지 않은 Error/Warning이 없다.

## Step 6. How To Play Scene UI를 구성한다

### AI 작업

- Step 4 편집표와 실제 코드의 일치를 재확인한다. 기존 Canvas와 EventSystem을 재사용하는 구성을 우선한다.

### 사용자 수동 작업

1. Play Mode를 종료하고 `Assets/Scenes/SampleScene.unity`를 연다.
2. `UIRoot/MenuCanvas/HowToPlayPanel`의 기존 자리표시자 내용을 Step 4 편집표의 안내 UI로 교체한다.
3. 자동 이동·Jump·Momentum Landing·Collectible·Pause 및 Mode별 목표 안내와 Binding 표시 요소를 생성한다.
4. 확정된 페이지 구성에 따라 Back, 시작/완료, Skip 등 필요한 버튼만 생성한다.
5. 편집표의 RectTransform·색상·Text·초기 활성·Raycast·Navigation 값을 적용한다. 표시되지 않는 버튼으로 Focus가 이동하지 않게 출처별 설정을 따른다.
6. 실제 구현된 Component를 붙이고 직렬화 참조와 OnClick을 연결한다. 동적 인자가 필요한 이벤트는 편집표에 명시한 Dynamic 항목을 선택한다.
7. Scene을 저장하고 완료 사실을 전달한다.

### 완료 조건

- [ ] 확정 편집표의 UI와 Inspector 연결이 사용자에 의해 저장됐다.

## Step 7. 저장한 Scene과 입력 연결을 정적으로 검사한다

### AI 작업

- Scene YAML의 fileID/GUID·Component·직렬화 참조·이벤트·Navigation·초기 상태·누락 Script를 대조한다.
- 공용 안내의 출처별 버튼·Focus 경로와 실제 Action 표시 원천을 검사한다.
- Test는 장식용 Object 이름 대신 직렬화 참조·Component·소속·행동을 검증한다. 색상 양자화 등 허용 가능한 오차와 실제 계약 위반을 구분한다.
- 정적으로 확인된 항목을 사용자에게 Inspector에서 재확인하도록 요구하지 않는다.

### 사용자 수동 작업

정적 불일치가 있을 때만 AI가 지정한 Object·Component·Field를 지정값으로 수정하고 저장한다. 불일치가 없으면 수동 작업은 없다.

### 완료 조건

- [ ] Scene 연결과 승인된 UI 구성이 정적 검사에서 일치한다.

## Step 8. 전체 자동 Test 회귀를 실행한다

### AI 작업

- 최종 변경 기준 집중 Fixture와 전체 회귀 목록을 제공하고 실패 원인을 코드·Test·Scene으로 구분한다.
- 최초 안내 완료·Skip·취소 후 재선택, 두 Mode, 재열람과 Binding 변경·복원, 입력 잔류 및 시작 실패를 자동 검증에 포함한다.
- Scene 수정은 사용자에게 정확한 변경값을 제공하고 코드·Test 수정은 AI가 처리한다.

### 사용자 수동 작업

1. Unity Script Compilation 및 예상하지 않은 Error/Warning 유무를 확인한다.
2. 지정된 집중 Edit Mode, 집중 Play Mode Test를 순서대로 실행한다.
3. 집중 Test 통과 후 전체 Edit Mode와 전체 Play Mode를 실행한다.
4. 각 실행 개수·성공·실패·Error/Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 함께 전달한다.

### 완료 조건

- [ ] 최종 코드·Scene 기준 집중 및 전체 Test가 성공했다.
- [ ] 예상하지 않은 Error/Warning이 없다. 실행하지 않은 Test를 통과로 기록하지 않았다.

## Step 9. 안내의 이해도·가독성과 Player Build를 확인한다

### AI 작업

- 수동 대상은 문구 이해도, 화면 배치, Focus 가시성, 실제 입력 조작감과 Player Build 표현으로 제한한다.
- State·Mode·횟수·Binding 값·진행 차단은 Step 8 자동 Test 결과로 판정한다. 사용자에게 정밀 타이밍 조작이나 디버거 조작을 요구하지 않는다.

### 사용자 수동 작업

1. 새 Play 세션에서 Main Menu → How To Play를 열어 조작·두 Mode 목표를 이해할 수 있는지 읽고 Back으로 복귀한다.
2. 첫 Mode 선택의 자동 안내에서 시작/완료를 사용한다. 별도 새 세션에서 Skip과 Back/Cancel의 표시·조작감을 확인한다. 횟수 판정은 자동 Test를 사용한다.
3. Settings에서 Jump 또는 Momentum Landing을 바꾼 뒤 안내를 다시 열고 변경된 표기가 읽기 쉬운지 확인한다. Restore Defaults 후 표기도 확인한다.
4. Keyboard와 Mouse로 버튼을 이동·Hover·Click하여 Focus가 잘 보이고 안내를 벗어날 수 있는지 확인한다. 스크롤 UI를 채택한 경우 키보드 선택 시 가시성과 휠 조작감을 확인한다.
5. `1920×1080`, `1280×720`, 최소 한 개의 비 16:9 Game View에서 안내의 두 진입 방식 및 Main Menu·Settings·Pause·Stage/Infinite Result에 잘림·겹침이 없는지 확인한다. 사용한 비 16:9 해상도를 기록한다.
6. Gamepad 보유 시 실제 장치로 표기·Focus·Submit·Cancel을 확인한다. 미보유 시 사유를 기록하여 검증 제외를 확인한다. 가상 Gamepad 자동 Test 성공을 실제 장치 확인으로 기록하지 않는다.
7. Unity Editor에서 Development Player Build와 Development Build를 끈 일반 Player Build를 각각 만들고 실행한다. 최초 안내·재열람·Skip·Settings 후 표기·두 Mode 진입을 확인한다. 일반 Build에서는 Infinite Difficulty UI 비표시도 확인한다.
8. 사용한 해상도·장치·Build 종류·화면 문제와 예상하지 않은 Error/Warning을 전달한다.

### 완료 조건

- [ ] 안내를 읽고 조작·목표를 이해할 수 있으며 필요한 화면비에서 잘림과 Focus 문제가 없다.
- [ ] 두 Player Build의 안내와 기본 흐름이 정상이며 일반 Build의 개발자 UI가 숨겨진다.
- [ ] 실제 장치 검증 결과와 승인된 제외 사유가 기록됐다.

## Step 10. Phase 4 결과와 Roadmap 006 완료 근거를 기록한다

### AI 작업

- Roadmap 완료 조건마다 정적 검사·Unit Test·통합 Test·수동 화면·Build 결과를 연결한다.
- 실제 Test 개수, Scene 변경, 화면비, 장치 제외, Build 종류 및 남은 관찰 사항을 기록한다.
- 모든 필수 근거 충족 시 Phase 4 완료와 Roadmap 006 진행 상태를 갱신한다. 영구 저장·실제 Leaderboard를 완료에 포함하지 않는다.

### 사용자 수동 작업

없음. 필수 근거가 누락됐다면 해당 항목만 확인한다.

### 완료 조건

- [ ] Phase 4 및 Roadmap 006의 완료 근거와 후속 범위가 문서화됐다.

# 영향 범위

이번 변경은 Task 문서 추가다. 향후 Phase 4 수행 시 관련 Feature·System 문서, Runtime 코드, Test, 사용자 Scene과 Roadmap이 변경될 수 있다. Rules·Package·ProjectSettings 변경은 계획에 포함하지 않는다.

# 검증 내용

- Roadmap 006 Phase 4의 구현 대상과 완료 조건을 Step 1~10에 배치했다.
- 상태·횟수·현재 Binding·진행 차단·Scene 연결을 정적 검사 또는 자동 Test 대상으로 명시했다.
- 수동 작업을 제품 선택, Scene 편집, Unity 검증 실행, 실제 화면·장치·Build 확인으로 제한했다.
- 기존 미완성 How To Play 경로와 Phase 3 검증 결과를 구분하고 구현 전인 항목을 완료 처리하지 않았다.

# 검증 결과

실행 계획 작성과 문서 정적 대조를 완료했다. Phase 4 코드는 구현하지 않았고 Unity 컴파일·Test Runner·Build는 실행하지 않았다. Phase 4 Step 완료 조건은 모두 미완료로 유지한다.

# 후속 작업

Step 1의 미정 규칙과 안내 문구 제안을 검토하여 확정한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/03_Features/HowToPlay.md`
- `AI/03_Features/GameEntryNavigation.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_006.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_6/20260921_01_Phase3ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 작업을 실행 순서대로 Step에 작성했다.
- [x] AI 작업·사용자 수동 작업·완료 조건을 각 Step에서 구분했다.
- [x] 정적 검사와 Unit/통합 Test를 우선하고 수동 중복 검증을 줄였다.
- [x] 승인 전 제안과 현재 구현 사실을 구분했다.
- [x] Scene 편집표의 확정 시점과 편집 시작 조건을 명시했다.
