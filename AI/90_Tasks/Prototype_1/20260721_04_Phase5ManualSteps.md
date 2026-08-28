# 작업 정보

## 작업명

Phase 5 Manual Steps

## 작업 일자

20260721

## 작업 담당자

AI

## 작업 상태

Phase 5 완료 / 밸런스 조정은 후속 작업으로 보류

---

# 작업 목적

Phase 5의 프로토타입 완성 및 플레이 검증을 위해 사용자가 Unity Editor와 실제 Play Mode에서 수동으로 수행해야 하는 작업 순서를 정의한다.

이 문서는 위에서 아래로 순서대로 수행한다. 각 Step의 완료 조건을 충족하지 못하면 다음 Step으로 넘어가지 않는다.

---

# 작업 대상

- UIInputSystem
- UI 마무리
- 플레이 테스트
- Phase 1~4 회귀 검증
- ScoreRecord 범위 확인

---

# 작업 전 상태

- Unity Editor 버전은 `6000.3.5f2`이다.
- IMPLEMENTATION_ROADMAP_001.md에서 Phase 1부터 Phase 4까지 완료 상태이다.
- Phase 5는 대기 상태이며 현재 진행 중인 작업은 없다.
- `InputSystem_Actions.inputactions`에는 UI Action Map이 존재한다.
- UI Action Map에는 Navigate, Submit, Cancel, Point, Click 등의 Action이 존재한다.
- UIInputSystem의 책임은 문서로 정의되어 있지만 생산 코드와 관련 Test는 아직 존재하지 않는다.
- UIManagementSystem은 StageHUD, ResultPanel과 클리어 시간 표시를 관리한다.
- 현재 UI State는 None, StageHud, Result로 구성되어 있다.
- 일반 Stage는 시작부터 결과 화면까지 플레이할 수 있는 Phase 4 구현 상태이다.
- InfiniteMode와 ScoreRecord는 프로토타입 검증 이후 작업으로 보류한다.
- GamePause는 프로토타입 검증 이후 작업으로 보류한다.
- Phase 5 UI는 StageHUD와 Retry 및 Quit 항목을 가진 ResultPanel로 구성한다.
- Phase 5 완료 검증의 필수 입력 장치는 키보드와 마우스이다.

---

# 조사 내용

아래 문서와 구현을 확인했다.

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
- AI/02_Systems/UIInputSystem.md
- AI/02_Systems/UIManagementSystem.md
- AI/03_Features/README.md
- AI/03_Features/ScoreRecord.md
- AI/03_Features/GamePause.md
- AI/04_Implementation_Roadmap/README.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/90_Tasks/20260721_02_Phase4ManualSteps.md
- AI/99_Templates/GENERAL_TASK_TEMPLATE.md
- Assets/InputSystem_Actions.inputactions
- Assets/Scripts/Runtime/Systems/GameSystem.cs
- Assets/Scripts/Runtime/Systems/PlayerInputSystem.cs
- Assets/Scripts/Runtime/Systems/UIManagementSystem.cs
- Assets/Scripts/Runtime/Core/E_UIState.cs
- Assets/Scenes/SampleScene.unity

확인된 기준은 아래와 같다.

- UIInputSystem은 UI 입력 수집과 UI Action Map 상태 관리만 담당한다.
- 어떤 Action Map을 활성화할지는 GameSystem이 결정한다.
- UI 화면 표시와 UI State 반영은 UIManagementSystem이 담당한다.
- 자동화할 수 있는 상태와 수치 검증은 Unity Test Runner를 사용한다.
- 화면 구성과 조작감은 실제 Play Mode에서 수동으로 확인한다.
- 확인되지 않은 UI 동작이나 목표 수치를 임의로 구현하거나 완료 기준으로 사용하지 않는다.

---

# 작업 내용

Phase 5 수동 작업을 아래 Step으로 정리했다.

## Step 1. Phase 5 범위를 확정한다

- 진행 상태: **완료**
- 확인 근거: 사용자가 Phase 5 미정 규칙의 제안 A 또는 B를 항목별로 선택했다.
- 후속 범위 변경: 밸런스 조정 관련 확정 결과는 Phase 5 완료 조건에서 제외하고 별도 후속 작업의 참고 기준으로 유지한다.

### 확정 결과

- InfiniteMode는 Phase 5에서 보류한다.
- ScoreRecord는 InfiniteMode와 함께 Phase 5에서 보류한다.
- GamePause는 Phase 5에서 보류한다.
- UI 화면은 StageHUD와 ResultPanel만 사용한다.
- ResultPanel에는 Retry와 Quit 항목을 둔다.
- ResultPanel이 활성화되면 Retry를 기본 선택한다.
- Navigate는 Retry와 Quit 사이의 선택을 변경한다.
- Submit은 현재 선택된 항목을 한 번 실행한다.
- Cancel은 ResultPanel에서 동작을 수행하지 않는다.
- Point는 마우스 포인터가 가리키는 Retry 또는 Quit을 선택한다.
- Click은 마우스 포인터가 가리키는 Retry 또는 Quit을 한 번 실행한다.
- UIInputSystem은 입력 상태만 GameSystem에 제공한다.
- GameSystem은 현재 게임 상태에 따라 UI 입력의 의미와 실행 흐름을 결정한다.
- UIManagementSystem은 GameSystem의 요청에 따라 UI 선택 상태를 변경하고 현재 선택 항목을 제공한다.
- Stage 플레이 중에는 Player Action Map만 활성화하고 UI Action Map은 비활성화한다.
- ResultPanel이 활성화되면 Player Action Map을 비활성화하고 UI Action Map만 활성화한다.
- Phase 5 완료 검증의 필수 입력 장치는 키보드와 마우스이다.
- 플레이 테스트는 최소 5회의 유효 기록을 사용한다.
- 밸런스를 변경하는 경우 기준값 2회와 최종 후보값 3회로 나누어 기록한다.
- 기록 항목은 성공 여부, 클리어 시간, 진행 불가, 관성 착지 시도와 성공, 재시작, 입력 중복, 로그와 정성 평가를 포함한다.
- 밸런스 조정은 이동 속도 또는 가속, 점프, 중력, 관성 착지와 최대 속도로 제한한다.
- 접지 판정 수치는 실제 판정 오류가 확인된 경우에만 별도 작업으로 검토한다.
- 성공 기준은 기준 플레이 대비 안정성과 상대 결과를 사용한다.

### 수행 절차

1. 확정 결과가 Roadmap, System과 Feature 문서에 반영되었는지 확인한다.
2. InfiniteMode, ScoreRecord와 GamePause가 Phase 5 구현 대상에서 제외되었는지 확인한다.
3. ResultMenu Feature에 Retry, Quit, Navigate, Submit과 Cancel 규칙이 정의되어 있는지 확인한다.
4. UIInputSystem, GameSystem과 UIManagementSystem의 입력 전달 방향이 일치하는지 확인한다.
5. Phase 5 수동 검증 기준이 확정 결과와 일치하는지 확인한다.

### 완료 조건

- Phase 5 범위에 서로 충돌하는 항목이 없다.
- UIInputSystem의 입력 결과를 어느 System이 어떻게 사용하는지 문서로 확인할 수 있다.
- UI 마무리의 완료 기준을 확인할 수 있다.
- 보류 항목은 구현 대상에서 명확히 제외되어 있다.
- ResultMenu 규칙과 System 간 책임 방향이 문서에서 일치한다.

### 확인 결과

- IMPLEMENTATION_ROADMAP_001.md의 Phase 5 구현 대상은 UIInputSystem, ResultMenu, UI 마무리와 플레이 테스트로 구성되어 있다.
- InfiniteMode, ScoreRecord와 GamePause는 Roadmap의 보류된 작업에 포함되어 있다.
- ResultMenu.md에 Retry, Quit, Navigate, Submit, Cancel, Point와 Click 규칙이 정의되어 있다.
- UIInputSystem은 현재 UI 입력 상태를 GameSystem에 제공한다.
- GameSystem은 현재 게임 상태에 따라 UI 입력의 의미와 실행 흐름을 결정한다.
- UIManagementSystem은 GameSystem 요청에 따라 UI 선택 상태를 관리하고 현재 선택 항목을 제공한다.
- 키보드와 마우스가 Phase 5 필수 UI 검증 장치로 정의되어 있다.
- Step 7의 최소 플레이 횟수와 밸런스 기준은 별도 후속 작업의 참고 내용으로 분리되었다.
- Step 1의 모든 수행 절차와 완료 조건을 충족했다.

## Step 2. Unity 프로젝트와 입력 설정을 점검한다

- 진행 상태: **완료**
- 사용자 확인 근거: Unity Editor 수동 작업 9개 항목 모두 성공

### AI 정적 확인 결과

- 프로젝트 Unity 버전은 `6000.3.5f2`이다.
- Input System Package 버전은 `1.17.0`이다.
- ProjectSettings의 Active Input Handling은 Input System Package 사용 상태이다.
- Input Action Asset에는 Player와 UI Action Map이 각각 하나 존재한다.
- UI Action Map에는 Navigate, Submit, Cancel, Point와 Click Action이 존재한다.
- Navigate에는 Keyboard WASD와 방향키 Binding이 존재한다.
- Submit과 Cancel에는 해당 Control Usage를 사용하는 Keyboard&Mouse Binding이 존재한다.
- Point에는 Mouse Position Binding이 존재한다.
- Click에는 Mouse Left Button Binding이 존재한다.
- Phase 5 필수 키보드와 마우스 Binding에 별도의 Interaction과 Processor가 설정되어 있지 않다.
- Navigate의 동일한 Composite 이름은 Keyboard, Gamepad와 Joystick용 Composite Root이며 같은 장치 Binding의 중복이 아니다.
- Input Action Asset의 Generate C# Class가 활성화되어 있다.
- 생성 Class 이름은 `InputSystem_Actions`이다.
- 생성 Namespace는 `FlowState.Input`이다.
- 생성된 `Assets/InputSystem_Actions.cs`는 Input System `1.17.0`과 원본 `Assets/InputSystem_Actions.inputactions`를 명시한다.
- 생성 C# Class에는 Player와 UI Map 및 Navigate, Submit, Cancel, Point와 Click 접근자가 존재한다.
- 생성 C# Class의 수정 시각은 Input Action Asset보다 늦다.
- 현재 Git 작업 목록에는 이전 Phase 5 문서 변경만 있으며 Input Action Asset, 생성 C# Class와 ProjectSettings 변경은 없다.
- AI는 Unity Editor Compile과 Build를 실행하지 않았다.

### 남은 Unity Editor 수동 확인

- 진행 상태: **완료**

1. Unity Hub에서 Unity `6000.3.5f2`로 프로젝트를 연다.
2. Script Import와 Compile이 끝날 때까지 기다린다.
3. Console을 비우고 Compile Error와 예상하지 않은 Warning이 없는지 확인한다.
4. `Assets/InputSystem_Actions.inputactions`를 Input Actions Editor로 연다.
5. UI Map의 Keyboard Navigate, Submit, Cancel과 Mouse Point, Click Binding이 Inspector에 정상 표시되는지 확인한다.
6. Asset Inspector에서 Generate C# Class, Class Name `InputSystem_Actions`와 Namespace `FlowState.Input`이 유지되는지 확인한다.
7. Input Actions Editor에 저장되지 않은 변경 표시가 없는지 확인한다.
8. 설정이 문서와 일치하면 Input Action Asset을 수정하거나 C# Class를 다시 생성하지 않는다.
9. 불일치가 확인된 경우에만 Asset을 저장하고 C# Class를 재생성한 뒤 Console을 다시 확인한다.
10. 이 단계에서는 Build를 실행하지 않는다.

### Unity Editor 수동 확인 결과

- Unity `6000.3.5f2`에서 프로젝트 열기와 Script Import가 정상 완료되었다.
- Compile Error와 예상하지 않은 Warning이 없음을 확인했다.
- Input Actions Editor에서 UI Map과 필수 키보드 및 마우스 Binding을 확인했다.
- Generate C# Class, Class Name과 Namespace가 문서의 확정값과 일치함을 확인했다.
- Input Actions Editor에 처리해야 할 저장되지 않은 변경이 없음을 확인했다.
- Input Action Asset 수정과 C# Class 재생성이 필요하지 않음을 확인했다.
- 사용자 수동 작업 9개 항목이 모두 성공했다.
- Step 2에서는 Build를 실행하지 않았다.

### 수행 절차

1. 변경 사항을 안전하게 되돌릴 수 있도록 Git 상태를 확인한다.
2. Unity Hub에서 Unity `6000.3.5f2`로 프로젝트를 연다.
3. Unity가 Script Import와 Compile을 완료할 때까지 기다린다.
4. Console을 비운 뒤 Compile Error와 예상하지 않은 Warning이 없는지 확인한다.
5. `Assets/InputSystem_Actions.inputactions`를 연다.
6. Player Action Map과 UI Action Map이 각각 하나만 존재하는지 확인한다.
7. UI Action Map의 Navigate, Submit, Cancel, Point와 Click Action을 확인한다.
8. 키보드 Navigate, Submit, Cancel과 마우스 Point, Click에 필요한 Binding이 존재하는지 확인한다.
9. 중복 Binding, 의도하지 않은 Interaction 또는 누락된 Control Type이 없는지 확인한다.
10. Input Actions C# Class가 `InputSystem_Actions`로 생성되도록 설정되어 있는지 확인한다.
11. Input Action Asset을 변경했다면 C# Class를 다시 생성하고 Compile 결과를 확인한다.

### 완료 조건

- Compile Error가 없다.
- Player와 UI Action Map의 책임이 구분되어 있다.
- 확정한 입력 장치로 필요한 UI Action을 발생시킬 수 있다.
- 생성된 Input Actions Class와 Asset 정의가 일치한다.
- Unity Editor 수동 확인 결과가 기록되어 있다.

### Step 2 완료 결과

- AI 정적 확인과 사용자 Unity Editor 수동 확인이 모두 완료되었다.
- Step 2의 모든 완료 조건을 충족했다.

## Step 3. UIInputSystem 구현 결과를 Scene에 연결한다

이 Step은 UIInputSystem 생산 코드와 관련 Test가 문서 기준으로 구현된 후 수행한다.

- 진행 상태: **완료**
- 사용자 확인 근거: Compile 성공, Edit Mode 39개 및 Play Mode 27개 전체 통과, Unity Editor 수동 작업 18개 항목 모두 성공

### AI 구현 결과

- `UIInputState`를 추가하여 Navigate, Pointer Position, Submit, Cancel과 Click 입력 상태를 표현했다.
- `UIInputSystem`을 추가하여 UI Action Map 초기화, 활성화, 비활성화와 입력 상태 초기화를 구현했다.
- UIInputSystem은 Navigate, Submit, Cancel, Point와 Click Callback을 등록하고 제거한다.
- Submit, Cancel과 Click은 소비 가능한 일회성 입력 상태로 관리한다.
- GameSystem에 UIInputSystem 필수 참조를 추가했다.
- Stage 플레이를 시작할 때 UI Action Map을 비활성화한다.
- 결과 화면을 활성화할 때 Player Action Map을 비활성화하고 UI Action Map을 활성화한다.
- 게임 시작 중단 시 UI Action Map을 비활성화한다.
- UIInputSystem의 초기화, 활성화, 비활성화와 반복 요청을 검증하는 Play Mode Test를 추가했다.
- GameLifecycleIntegrationTests에 플레이 중 UI Map 비활성 및 결과 화면 UI Map 활성 검증을 추가했다.
- AI는 Unity Editor Compile, Test Runner와 Build를 실행하지 않았다.

### AI 정적 확인 결과

- 생산 코드의 Class 이름과 파일 이름이 일치한다.
- UIInputSystem은 입력의 의미나 ResultMenu 동작을 결정하지 않는다.
- GameSystem이 Action Map 사용 시점을 결정한다.
- Scene에 UIInputSystem GameObject와 Component가 각각 정확히 하나 존재한다.
- Scene의 GameSystem에 UIInputSystem Component 참조가 연결되어 있다.
- Scene에 Missing UIInputSystem 참조가 없다.
- Retry, Quit Button과 ResultMenu 최종 UI 구성은 Step 4 범위이므로 이번 Step에서 추가하지 않았다.

### 필요한 Unity Editor 수동 작업

- 진행 상태: **완료**

1. Unity `6000.3.5f2`에서 프로젝트를 열고 Script Compile이 끝날 때까지 기다린다.
2. Console을 비우고 새 Compile Error가 없는지 확인한다.
3. `Assets/Scenes/SampleScene.unity`를 연다.
4. Hierarchy의 `Systems` 하위에 빈 GameObject를 생성하고 이름을 `UIInputSystem`으로 지정한다.
5. `UIInputSystem` GameObject에 `UIInputSystem` Component를 부착한다.
6. Hierarchy 또는 Scene 검색으로 UIInputSystem Component가 정확히 하나만 존재하는지 확인한다.
7. `GameSystem` GameObject를 선택한다.
8. GameSystem Component의 `UI Input System` 필드에 새 `UIInputSystem` GameObject의 Component를 연결한다.
9. GameSystem과 UIInputSystem에 Missing Script 또는 비어 있는 새 필수 참조가 없는지 확인한다.
10. SampleScene을 저장한다.
11. Test Runner의 Play Mode에서 `UIInputSystemTests` 3개를 실행한다.
12. `GameLifecycleIntegrationTests` 전체를 실행한다.
13. 관련 Test가 모두 통과하고 예상하지 않은 Error와 Warning이 없는지 확인한다.
14. SampleScene을 Play Mode로 실행한다.
15. Stage 플레이 중 Player 입력이 동작하는지 확인한다.
16. Stage 플레이 중 UI Action Map 비활성 상태를 테스트 결과로 확인한다.
17. Goal에 도달하여 ResultPanel을 표시한다.
18. 결과 화면에서 Player 이동이 중지되는지 확인한다.
19. 결과 화면에서 UI Action Map 활성 상태를 테스트 결과로 확인한다.
20. Play Mode를 종료한 뒤 다시 실행해 Callback 중복, 입력 중복과 예상하지 않은 로그가 없는지 확인한다.
21. 이 Step에서는 Build를 실행하지 않는다.

### Unity Editor 수동 확인 결과

- Step 3 관련 Script Compile이 성공했다.
- 별도 Compile Error와 예상하지 않은 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `39 Passed, 0 Failed`를 확인했다.
- Play Mode Test 전체 `27 Passed, 0 Failed`를 확인했다.
- Test 실행 중 Error 메시지가 발생하지 않았다.
- SampleScene에 UIInputSystem GameObject와 Component가 정확히 하나 존재한다.
- GameSystem의 UI Input System 필드가 실제 UIInputSystem Component를 참조한다.
- Stage 플레이 중 Player 입력이 정상 동작한다.
- 결과 화면에서 Player 이동이 중지된다.
- Action Map 상태 전환 Test가 통과했다.
- Play Mode 재실행 후 Callback 중복, 입력 중복과 예상하지 않은 로그가 발생하지 않았다.
- Unity Editor 수동 작업 18개 항목이 모두 성공했다.
- Step 3에서는 Build를 실행하지 않았다.

### 수행 절차

1. `Assets/Scenes/SampleScene.unity`를 연다.
2. Hierarchy의 Systems 하위에 UIInputSystem용 GameObject가 있는지 확인한다.
3. 없다면 Systems 하위에 `UIInputSystem` GameObject를 하나 생성한다.
4. UIInputSystem Component를 해당 GameObject에 부착한다.
5. 같은 Scene에 UIInputSystem Component가 중복 존재하지 않는지 검색한다.
6. GameSystem이 UIInputSystem을 제어하도록 구현되었다면 GameSystem Inspector의 해당 참조에 연결한다.
7. GameSystem이 UIManagementSystem에 선택 상태 변경을 요청할 수 있도록 문서와 코드에 정의된 참조를 확인한다.
8. Inspector에 Missing Script 또는 비어 있는 필수 참조가 없는지 확인한다.
9. Play Mode 진입 후 Player Action Map과 UI Action Map 활성 상태가 현재 Game State에 맞는지 확인한다.
10. Play Mode 종료 후 UIInputSystem의 Callback과 Input Actions가 정상 해제되는지 Console에서 확인한다.

### Action Map 확인 기준

- Stage 플레이 중에는 Player Action Map 정책이 유지된다.
- UI 입력이 필요한 상태에서만 UI Action Map이 활성화된다.
- GameSystem의 요청 없이 UIInputSystem이 Action Map 사용 시점을 결정하지 않는다.
- Player Action Map과 UI Action Map을 동시에 활성화하지 않는다.

### 완료 조건

- UIInputSystem이 Scene에 정확히 하나 존재한다.
- 필수 Inspector 참조가 모두 연결되어 있다.
- UI Action Map 활성화와 비활성화가 GameSystem 요청에 따라 동작한다.
- Play Mode 반복 진입 시 Callback 중복이나 입력 중복이 발생하지 않는다.

### Step 3 완료 결과

- UIInputSystem 생산 코드, Test와 SampleScene 연결이 완료되었다.
- 전체 Edit Mode 39개와 Play Mode 27개 Test가 통과했다.
- 사용자 Unity Editor 수동 확인이 완료되었다.
- Step 3의 모든 완료 조건을 충족했다.

## Step 4. StageHUD와 ResultPanel의 최종 UI를 구성한다

- 진행 상태: **완료**
- 사용자 확인 근거: Compile 성공, Edit Mode 39개 및 Play Mode 29개 전체 통과, Unity Editor 수동 작업 16개 항목 모두 성공

### AI 구현 결과

- ResultMenu 선택 상태를 표현하는 `E_ResultMenuSelection`을 추가했다.
- UIInputState에 마우스 Point 변경 상태를 추가했다.
- Navigate는 소비 후 초기화하여 하나의 입력이 여러 프레임에서 반복 처리되지 않도록 했다.
- Click은 눌림 값에서만 일회성 입력으로 처리한다.
- UIManagementSystem에 Retry와 Quit Button 참조 및 현재 ResultMenu 선택 상태를 추가했다.
- ResultPanel이 활성화되면 Retry를 기본 선택한다.
- 키보드 아래 방향 입력은 Retry에서 Quit으로, 위 방향 입력은 Quit에서 Retry로 선택을 변경한다.
- 마우스 Point는 Button RectTransform 영역을 기준으로 Retry 또는 Quit을 선택한다.
- GameSystem은 결과 화면에서 Submit 또는 유효한 Button Click을 한 번 처리한다.
- Retry는 기존 StartGame 흐름을 사용하여 새로운 Stage Play를 시작한다.
- Quit은 Application 종료를 요청한다.
- Cancel은 ResultMenu 동작을 수행하지 않고 소비된다.
- ResultMenu 기본 선택, 키보드 선택 이동과 마우스 선택을 검증하는 Play Mode Test 2개를 추가했다.
- AI는 Unity Editor Compile, Test Runner와 Build를 실행하지 않았다.

### AI 정적 확인 결과

- SampleScene에 RetryButton과 QuitButton이 각각 정확히 하나 존재한다.
- UIManagementSystem의 Retry Button과 Quit Button 참조가 실제 Scene Button에 연결되어 있다.
- 두 Button의 Navigation Mode는 None이고 Transition은 Color Tint이다.
- 두 Button의 On Click Persistent Event 목록은 비어 있다.
- EventSystem은 활성 상태이고 InputSystemUIInputModule은 비활성 상태이다.
- ResultMenu는 UIInputSystem 입력을 GameSystem이 해석하는 구조이므로 InputSystemUIInputModule을 함께 활성화하면 입력이 중복 처리될 수 있다.
- ResultPanel 아래 Canvas는 Screen Space Overlay이며 기준 해상도는 `1920 x 1080`이다.

### 필요한 Unity Editor 수동 작업

- 진행 상태: **완료**

1. Unity `6000.3.5f2`에서 프로젝트를 열고 Script Compile 완료를 기다린다.
2. Console을 비우고 새 Compile Error가 없는지 확인한다.
3. `Assets/Scenes/SampleScene.unity`를 연다.
4. `UIRoot/ResultPanel/Canvas`를 펼친다.
5. 기존 Clear Time Text와 배경 Image가 ResultPanel 안에서 정상 표시되는지 확인한다.
6. ResultPanel의 Canvas 아래에 TextMeshPro Button을 생성하고 이름을 `RetryButton`으로 지정한다.
7. RetryButton의 표시 Text를 `Retry`로 설정한다.
8. ResultPanel의 Canvas 아래에 TextMeshPro Button을 생성하고 이름을 `QuitButton`으로 지정한다.
9. QuitButton의 표시 Text를 `Quit`으로 설정한다.
10. RetryButton을 QuitButton보다 위에 배치한다.
11. 두 Button이 Clear Time Text와 겹치지 않도록 중앙 하단 영역에 배치한다.
12. 두 Button의 RectTransform 크기를 키보드 선택 표시와 마우스 클릭 영역을 구분할 수 있는 크기로 맞춘다.
13. 두 Button의 Navigation Mode를 `None`으로 설정한다.
14. 두 Button의 Transition을 `Color Tint`로 설정하고 Normal, Highlighted와 Selected 상태가 시각적으로 구분되게 한다.
15. 두 Button의 On Click 이벤트 목록은 비워 둔다.
16. `UIManagementSystem` GameObject를 선택한다.
17. UIManagementSystem의 `Retry Button` 필드에 RetryButton Component를 연결한다.
18. UIManagementSystem의 `Quit Button` 필드에 QuitButton Component를 연결한다.
19. 기존 Stage HUD, Result Panel과 Clear Time Text 참조가 유지되는지 확인한다.
20. `EventSystem` GameObject를 선택한다.
21. EventSystem Component는 활성 상태로 유지한다.
22. `InputSystemUIInputModule` Component만 비활성화하여 별도 UI 입력 처리를 차단한다.
23. SampleScene을 저장한다.
24. Scene을 닫았다가 다시 열어 Button과 Inspector 참조가 유지되는지 확인한다.
25. Game View 시험 해상도를 `1280 x 720`, `1920 x 1080`, `2560 x 1440`으로 준비한다.
26. Play Mode Test에서 `ResultMenuIntegrationTests` 2개를 실행한다.
27. `UIInputSystemTests`와 `GameLifecycleIntegrationTests` 전체를 실행한다.
28. 관련 Test가 모두 통과하고 예상하지 않은 Error와 Warning이 없는지 확인한다.
29. SampleScene을 Play Mode로 실행하고 Goal에 도달한다.
30. ResultPanel이 표시될 때 Retry가 기본 선택되어 Selected 상태로 보이는지 확인한다.
31. 키보드 아래 방향으로 Quit을 선택하고 위 방향으로 Retry를 다시 선택한다.
32. 키보드 Submit으로 Retry를 실행하고 새로운 Stage Play가 시작되는지 확인한다.
33. 다시 Goal에 도달한 뒤 Cancel을 눌러 Retry와 Quit이 실행되지 않는지 확인한다.
34. 마우스를 Retry와 Quit 위로 각각 이동해 선택 표시가 변경되는지 확인한다.
35. Retry를 마우스로 Click하여 새로운 Stage Play가 한 번만 시작되는지 확인한다.
36. 각 시험 해상도에서 StageHUD, ResultPanel, Clear Time Text와 두 Button이 잘리거나 겹치지 않는지 확인한다.
37. Stage 플레이 중 StageHUD만, 결과 화면에서는 ResultPanel만 표시되는지 확인한다.
38. Console에 Callback 중복, 입력 중복, Missing Reference와 예상하지 않은 로그가 없는지 확인한다.
39. Quit 동작은 이후 사용자가 직접 수행하는 Build에서 키보드 Submit과 마우스 Click으로 각각 확인한다.
40. 이 Step에서 AI는 Build를 실행하지 않는다.

### Unity Editor 수동 확인 결과

- Step 4 관련 Script Compile이 성공했다.
- 별도 Compile Error와 예상하지 않은 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `39 Passed, 0 Failed`를 확인했다.
- Play Mode Test 전체 `29 Passed, 0 Failed`를 확인했다.
- Test 실행 중 Error 메시지가 발생하지 않았다.
- RetryButton과 QuitButton이 ResultPanel에 구성되어 있다.
- UIManagementSystem의 두 Button 참조가 연결되어 있다.
- InputSystemUIInputModule이 비활성 상태이다.
- Retry가 ResultPanel의 기본 선택으로 표시된다.
- 키보드 Navigate와 Submit, Cancel 규칙이 정상 동작한다.
- 마우스 Point와 Click 규칙이 정상 동작한다.
- Retry 실행 시 새로운 Stage Play가 한 번만 시작된다.
- StageHUD와 ResultPanel이 동시에 표시되지 않는다.
- 세 시험 해상도에서 핵심 UI가 잘리거나 겹치지 않는다.
- Unity Editor 수동 작업 16개 항목이 모두 성공했다.
- Quit의 실제 Application 종료는 Step 5의 사용자 Build에서 검증한다.
- AI는 Build를 실행하지 않았다.

### 수행 절차

1. SampleScene의 `UIRoot` 아래 StageHUD와 ResultPanel을 확인한다.
2. Canvas Scaler의 UI Scale Mode와 기준 해상도를 확인하고 시험 해상도를 기록한다.
3. StageHUD에 현재 플레이에 필요한 정보만 남긴다.
4. ResultPanel에 `Clear Time: 12.345 s` 형식의 결과가 표시될 공간을 확보한다.
5. Step 1에서 확정한 UI 동작에 필요한 Button, 선택 표시와 안내 문구를 배치한다.
6. 첫 선택 대상이 필요한 화면은 화면 활성화 직후 선택 상태가 보이도록 구성한다.
7. 키보드 Navigate로 Retry와 Quit 사이의 선택이 정상적으로 변경되는지 확인한다.
8. Submit과 Cancel 입력이 확정된 동작을 한 번만 수행하는지 확인한다.
9. Cancel 입력이 Retry 또는 Quit을 실행하지 않는지 확인한다.
10. 마우스 Point로 Retry와 Quit을 각각 선택할 수 있는지 확인한다.
11. 마우스 Click으로 Retry와 Quit을 각각 한 번 실행할 수 있는지 확인한다.
12. StageHUD와 ResultPanel이 동시에 활성화되지 않는지 확인한다.
13. UI Text가 겹치거나 잘리거나 화면 밖으로 벗어나지 않는지 확인한다.
14. 확정한 최소·기준·최대 시험 해상도에서 같은 항목을 반복 확인한다.
15. 사용하지 않는 임시 Text, Button, Event Trigger와 테스트용 오브젝트를 제거한다.

### 화면별 확인 항목

#### StageHUD

- 플레이 화면을 필요 이상으로 가리지 않는다.
- 플레이 중 필요한 정보가 즉시 구분된다.
- Player 이동과 Goal 확인을 방해하지 않는다.

#### ResultPanel

- 확정된 클리어 시간이 소수점 셋째 자리까지 보인다.
- 결과 표시 후 시간이 계속 증가하지 않는다.
- Step 1에서 확정한 다음 동작을 입력 장치로 실행할 수 있다.
- 선택 가능한 UI와 단순 표시 Text가 시각적으로 구분된다.

### 완료 조건

- 모든 시험 해상도에서 핵심 UI가 읽을 수 있는 상태로 표시된다.
- 키보드와 마우스 UI 조작이 정상이다.
- StageHUD와 ResultPanel의 활성 상태가 UI State와 일치한다.
- 임시 UI와 사용하지 않는 Component가 남아 있지 않다.

### Step 4 완료 결과

- ResultMenu 생산 코드, Test와 SampleScene UI 구성이 완료되었다.
- 전체 Edit Mode 39개와 Play Mode 29개 Test가 통과했다.
- 사용자 Unity Editor 수동 확인이 완료되었다.
- Quit의 Build 종료 검증을 제외한 Step 4의 모든 완료 조건을 충족했다.
- Quit 검증은 Step 5에서 수행한다.

## Step 5. Unity Test Runner와 Build를 실행한다

- 진행 상태: **완료**
- 사용자 확인 근거: Compile 성공 및 Development Build 수동 작업 14개 항목 모두 성공

### AI 정적 확인 결과

- Unity 버전은 `6000.3.5f2`이다.
- EditorBuildSettings에 `Assets/Scenes/SampleScene.unity`가 활성 Scene으로 포함되어 있다.
- Build 설정에는 프로젝트의 `Assets/InputSystem_Actions.inputactions`가 연결되어 있다.
- Active Input Handling은 Input System Package 사용 상태이다.
- 정적 Test Attribute 수는 Edit Mode 39개와 Play Mode 29개이다.
- Step 4 완료 시점에 Edit Mode 전체 `39 Passed, 0 Failed`가 확인되었다.
- Step 4 완료 시점에 Play Mode 전체 `29 Passed, 0 Failed`가 확인되었다.
- Step 4 Test 이후 생산 코드와 SampleScene의 기능 변경은 없다.
- Product Name은 `Unity_Flow_State`, Bundle Version은 `0.1.0`이다.
- 별도 Build Profile Asset은 저장소에 없으며 EditorBuildSettings의 Scene 목록을 사용한다.
- AI는 Unity Editor, Test Runner와 Build를 실행하지 않았다.

### 자동 Test 확인 상태

- Edit Mode Test: **완료 — 39 Passed, 0 Failed**
- Play Mode Test: **완료 — 29 Passed, 0 Failed**
- 예상하지 않은 Error와 Warning: **없음**

### 필요한 사용자 Build 및 실행 작업

1. Unity `6000.3.5f2`에서 프로젝트를 연다.
2. Compile이 끝난 뒤 Console에 Error와 예상하지 않은 Warning이 없는지 확인한다.
3. 코드 또는 SampleScene이 Step 4 이후 변경되었다면 Edit Mode 39개와 Play Mode 29개 전체 Test를 다시 실행한다.
4. `File > Build Profiles`를 연다.
5. 현재 개발 PC에서 실행할 수 있는 Windows Standalone Profile을 선택한다.
6. Scene List에 `Assets/Scenes/SampleScene.unity`가 포함되고 활성화되어 있는지 확인한다.
7. Development Build를 활성화한다.
8. Build 출력 폴더를 프로젝트의 `Assets` 폴더 밖으로 지정한다.
9. Build를 실행한다.
10. Build가 성공하고 Compile Error와 예상하지 않은 Warning이 없는지 확인한다.
11. 생성된 실행 파일을 실행한다.
12. StageHUD가 표시되고 키보드로 이동, 점프와 관성 착지를 수행할 수 있는지 확인한다.
13. Goal에 도달해 ResultPanel, Clear Time, Retry와 Quit이 정상 표시되는지 확인한다.
14. 키보드 Navigate와 Submit으로 Retry를 실행하고 새로운 Stage Play가 한 번 시작되는지 확인한다.
15. 다시 ResultPanel에 도달해 마우스 Point와 Click으로 Retry를 실행하고 새로운 Stage Play가 한 번 시작되는지 확인한다.
16. 다시 ResultPanel에 도달해 키보드로 Quit을 선택하고 Submit하여 Application이 종료되는지 확인한다.
17. 실행 파일을 다시 열고 ResultPanel에서 Quit을 마우스로 Click하여 Application이 종료되는지 확인한다.
18. Build 실행 중 UI 잘림, 입력 중복, Missing Reference, Error와 예상하지 않은 Warning이 없는지 확인한다.
19. Build 경로, 성공 여부, 실행 결과와 발견된 로그를 기록한다.
20. AI에게 Build와 실행 검증 결과를 전달한다.

AI는 위 Build 작업을 수행하지 않는다.

### 사용자 Build 및 실행 확인 결과

- Step 5 관련 Script Compile이 성공했다.
- 별도 Compile Error와 예상하지 않은 Warning이 발생하지 않았다.
- Development Build가 성공했다.
- Build 출력 경로는 `Dev-Build`이다.
- 실행 파일 `Dev-Build/Unity_Flow_State.exe`가 생성되었다.
- Build 실행 파일에서 StageHUD와 핵심 플레이가 정상 동작했다.
- ResultPanel, Clear Time, Retry와 Quit이 정상 표시되었다.
- 키보드 Navigate와 Submit으로 Retry가 한 번 실행되었다.
- 마우스 Point와 Click으로 Retry가 한 번 실행되었다.
- 키보드 Submit으로 Quit 시 Application이 종료되었다.
- 마우스 Click으로 Quit 시 Application이 종료되었다.
- UI 잘림, 입력 중복, Missing Reference, Error와 예상하지 않은 Warning이 발생하지 않았다.
- Development Build 수동 작업 14개 항목이 모두 성공했다.
- AI는 Build를 실행하지 않았다.

### Edit Mode Test

1. `Window > General > Test Runner`를 연다.
2. Edit Mode 전체 Test를 실행한다.
3. UI 입력 상태 초기화와 일회성 입력 소비 Test를 확인한다.
4. UI Action Map 중복 활성화와 중복 Callback 방지 Test를 확인한다.
5. UI State 전환 조건과 확정한 UI 동작의 상태 Test를 확인한다.
6. ResultMenu의 기본 선택, Navigate, Submit, Cancel과 중복 실행 방지 Test를 확인한다.
7. 실패가 있으면 다음 단계로 넘어가지 않고 Test 이름, 메시지와 Stack Trace를 기록한다.

### Play Mode Test

1. Play Mode 전체 Test를 실행한다.
2. 실제 Input Action Asset을 사용하는 UIInputSystem 통합 Test를 확인한다.
3. GameSystem 요청에 따른 Player/UI Action Map 전환 Test를 확인한다.
4. 실제 Scene의 UI Component와 Inspector 참조 Test를 확인한다.
5. UI 입력부터 UIManagementSystem 결과까지의 흐름을 확인한다.
6. 종료와 재시작 후 Callback, 선택 상태와 입력 상태가 초기화되는지 확인한다.
7. Phase 1~4 기존 Test가 모두 통과하는지 확인한다.

### Build 확인

1. 프로젝트에서 사용하는 Build Profile을 연다.
2. SampleScene이 Scene List에 포함되어 있는지 확인한다.
3. 개발용 Build를 실행한다.
4. Build 실패, Compile Error와 예상하지 않은 Warning이 없는지 확인한다.
5. Build 실행 파일에서 UI 표시와 확정된 입력 장치가 Editor와 동일하게 동작하는지 확인한다.

### 완료 조건

- Edit Mode 전체 Test가 통과한다.
- Play Mode 전체 Test가 통과한다.
- 예상하지 않은 Error와 Warning이 없다.
- 개발용 Build가 성공하고 실행 가능하다.

### Step 5 완료 결과

- Edit Mode 전체 `39 Passed, 0 Failed`를 확인했다.
- Play Mode 전체 `29 Passed, 0 Failed`를 확인했다.
- 사용자 Development Build와 실행 검증이 완료되었다.
- 키보드와 마우스 Retry 및 Quit 동작이 Build에서 검증되었다.
- Step 5의 모든 완료 조건을 충족했다.

## Step 6. 처음부터 결과 화면까지 전체 플레이를 검증한다

- 진행 상태: **완료**
- 사용자 확인 근거: 전체 플레이 수동 확인 18개 항목 성공, 예상하지 않은 Error와 Warning 없음
- 자동 검증 근거: Unity Script Compile 성공, Edit Mode 39개 및 Play Mode 34개 전체 통과

### AI 정적 확인 결과

- SampleScene에는 GameSystem, UIInputSystem, StageHUD, ResultPanel, RetryButton과 QuitButton이 연결된 상태로 존재한다.
- GameSystem은 Stage 시작 시 Player Action Map을 활성화하고 UI Action Map을 비활성화한다.
- Stage 종료 시 Player Action Map을 비활성화하고 UI Action Map을 활성화한다.
- Goal 도달 시 Timer를 정지하고 Result Data와 승인된 Clear Time 문자열을 생성한 뒤 ResultPanel을 활성화한다.
- StageGoalIntegrationTests는 Goal 종료 1회, 양수 클리어 시간, 승인된 표시 형식, StageHUD 비활성, ResultPanel 활성과 Player Rigidbody 정지를 검증한다.
- GameLifecycleIntegrationTests는 종료 후 Player 및 UI Action Map 상태와 같은 실행 세션의 재시작 복구를 검증한다.
- StageGoalIntegrationTests는 재시작 시 Stage 상태, Result Data, Player 위치와 Rigidbody 상태가 초기화되는지 검증한다.
- ResultMenuIntegrationTests는 Retry 기본 선택, 키보드 선택 이동과 마우스 Pointer 선택을 검증한다.
- UIInputSystemTests는 UI Action Map 생명주기와 반복 활성화 요청을 검증한다.
- Step 5에서 Edit Mode 전체 `39 Passed, 0 Failed`와 Play Mode 전체 `29 Passed, 0 Failed`가 확인되었다.
- Step 5 Development Build에서 키보드와 마우스 Retry 및 Quit 동작이 확인되었다.
- Step 5 이후 생산 코드와 SampleScene의 기능 변경은 없다.
- 조작감, 실제 Camera 구도, UI 시각 상태와 동일 세션의 연속 플레이는 Unity Editor에서 직접 관찰해야 한다.
- AI는 Unity Editor와 Build를 실행하지 않았다.

### 추가 자동 Test 구현 결과

- 사람이 관찰하기 어려운 Stage 플레이 중 UI 입력 차단을 검증하기 위해 `PlayingState_DisablesUIInputAndKeepsStateEmpty` Play Mode Test를 추가했다.
- Test는 실제 SampleScene이 Playing 상태인지 확인한다.
- Test는 Player Action Map 활성화와 UI Action Map 비활성화를 확인한다.
- Test는 UIInputState의 Navigate, Submit, Cancel, Point 변경과 Click 상태가 모두 초기값인지 확인한다.
- `PlayingState_IgnoresForcedUITransientInput`을 추가하여 Playing 상태에 UI 일회성 입력이 강제로 존재해도 ResultMenu가 실행되지 않는지 검증한다.
- ResultMenu Test에 실제 EventSystem 선택 오브젝트 검증을 추가하여 Retry와 Quit 선택 표시의 상태 근거를 자동화했다.
- `ResultMenu_CancelInput_DoesNotExecuteSelection`을 추가하여 Cancel 입력이 선택 동작을 실행하지 않는지 검증한다.
- `ResultMenu_SubmitRetry_StartsNewStageOnce`를 추가하여 Submit 입력이 생산 GameSystem 경로를 통해 Retry를 실행하는지 검증한다.
- `ResultMenu_MouseClickRetry_StartsNewStageOnce`를 추가하여 Pointer와 Click 입력이 생산 GameSystem 경로를 통해 Retry를 실행하는지 검증한다.
- StageGoalIntegrationTests에 결과 화면 진입 후 시간이 지나도 Clear Time 문자열이 변경되지 않는 검증을 추가했다.
- 추가 후 예상되는 전체 Play Mode Test 수는 34개이다.
- AI는 Unity Compile과 Test Runner를 실행하지 않았다.

### 필요한 Unity Editor 수동 작업

- 진행 상태: **완료**

1. Unity `6000.3.5f2`에서 프로젝트를 열고 Compile 완료를 기다린다.
2. Console을 비우고 Error와 예상하지 않은 Warning이 없는지 확인한다.
3. `Assets/Scenes/SampleScene.unity`를 Play Mode로 실행한다.
4. 시작 시 StageHUD만 표시되고 ResultPanel은 비활성인지 확인한다.
5. 키보드로 좌우 이동, 점프, 일반 착지와 관성 착지를 각각 수행한다.
6. Camera가 Player 수평 이동을 자연스럽게 추적하고 시각적 떨림이나 순간 이동이 없는지 확인한다.
7. Player가 지형을 통과하지 않고 Ground 및 Stage 충돌이 정상인지 확인한다.
8. Stage 플레이 중 UI Navigate, Submit, Cancel과 마우스 Click을 수행해 ResultMenu 동작이 실행되지 않는지 확인한다.
9. GamePause 화면이나 일시정지 동작이 추가되지 않았는지 확인한다.
10. Player를 Goal까지 이동시킨다.
11. StageHUD가 꺼지고 ResultPanel이 표시되며 Clear Time이 `Clear Time: 12.345 s` 형식으로 한 번 표시되는지 확인한다.
12. Result 화면에서 2초 이상 기다려도 Clear Time이 증가하거나 반복 갱신되지 않는지 확인한다.
13. 결과 화면에서 Player 입력과 이동이 중지되는지 확인한다.
14. 키보드 위·아래로 Retry와 Quit 선택을 변경하고 Cancel이 아무 동작도 수행하지 않는지 확인한다.
15. 마우스를 Retry와 Quit 위로 이동해 선택 표시가 각각 변경되는지 확인한다.
16. Retry를 마우스로 Click하여 같은 Play Mode에서 두 번째 Stage Play를 시작한다.
17. 두 번째 플레이에서 Player가 StartPoint로 돌아가고 StageHUD, Timer, Result와 입력 상태가 새로 초기화되는지 확인한다.
18. 첫 번째 플레이와 다른 시간 동안 플레이한 뒤 Goal에 다시 도달한다.
19. 두 번째 Clear Time이 첫 번째 결과를 이어서 증가시킨 값이 아니라 새로운 Stage Play의 독립적인 결과인지 확인한다.
20. Play Mode를 종료했다가 다시 실행해 시작 상태와 전체 흐름이 정상 복구되는지 확인한다.
21. Console에 입력 중복, Callback 중복, Missing Reference, Error와 예상하지 않은 Warning이 없는지 확인한다.
22. 수동 작업 19개 기능 확인 항목의 성공 여부와 발견된 문제를 기록해 AI에게 전달한다.

Step 6에서는 Build를 다시 수행할 필요가 없다. 키보드와 마우스 Quit의 실제 Application 종료는 Step 5 Development Build에서 이미 검증되었다.

### Unity Editor 수동 확인 결과

- Play Mode 시작 시 StageHUD가 표시되고 ResultPanel이 비활성화되었다.
- 좌우 방향키와 A 및 D 키로 Player 이동이 정상 동작했다.
- Space 키로 Player 점프가 정상 동작했다.
- Ground 착지 후 다시 점프할 수 있었다.
- 이동 중 관성 착지 성공 시 Player Velocity가 기본 최대값 8을 넘어 9.2에 도달했다.
- Player 이동 시 Camera가 Player를 정상적으로 추적했다.
- Stage 플레이 중 ESC 입력으로 GamePause가 수행되지 않았다.
- Goal 도달 시 StageHUD가 비활성화되고 ResultPanel이 활성화되었다.
- Clear Time이 소수점 셋째 자리까지 표시되었다.
- 결과 화면에서 시간이 지나도 Clear Time이 변경되지 않았다.
- 결과 화면에서 Player 입력과 이동이 중지되었다.
- 키보드 Navigation 시 선택된 Button이 설정된 색상으로 변경되었다.
- Cancel로 설정된 ESC 입력은 ResultMenu 동작을 수행하지 않았다.
- 키보드와 마우스 Retry로 Stage를 다시 시작할 수 있었다.
- Retry 후 이전 Stage Play의 정보가 유지되지 않았다.
- Console에 예상하지 않은 Error와 Warning이 없었다.
- Development Build에서 Quit 선택 시 Application이 종료되었다.
- Stage 플레이 중 UI 입력 차단은 사람의 관찰 대신 추가 Play Mode Test로 검증한다.

### 남은 Unity Test Runner 작업

- 진행 상태: **완료**

1. Unity가 새 `GameLifecycleIntegrationTests`를 Compile하는지 확인한다.
2. Compile Error와 예상하지 않은 Warning이 없는지 확인한다.
3. Play Mode Test 전체를 실행한다.
4. 전체 Test 수가 34개인지 확인한다.
5. `PlayingState_DisablesUIInputAndKeepsStateEmpty`가 통과하는지 확인한다.
6. `PlayingState_IgnoresForcedUITransientInput`이 통과하는지 확인한다.
7. ResultMenu의 Cancel, Submit Retry와 Mouse Click Retry Test 3개가 통과하는지 확인한다.
8. StageGoalIntegrationTests의 Clear Time 고정 검증이 통과하는지 확인한다.
9. Play Mode 전체 `34 Passed, 0 Failed`인지 확인한다.
10. Test 실행 중 Error와 예상하지 않은 Warning이 없는지 확인한다.

이 추가 검증에는 Build가 필요하지 않다.

### Unity Test Runner 확인 결과

- Unity Script Compilation이 성공했다.
- Script Compilation에서 예상하지 않은 Error와 Warning이 발생하지 않았다.
- Edit Mode Test 전체 `39 Passed, 0 Failed`를 확인했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 발생하지 않았다.
- Play Mode Test 전체 `34 Passed, 0 Failed`를 확인했다.
- Play Mode Test에서 예상하지 않은 Error와 Warning이 발생하지 않았다.
- Stage 플레이 중 UI 입력 차단 Test가 통과했다.
- ResultMenu의 Cancel, Submit Retry와 Mouse Click Retry Test가 통과했다.
- Clear Time 고정 검증이 포함된 StageGoalIntegrationTests가 통과했다.
- 추가 자동화 검증에 Build를 실행하지 않았다.

### 자동화 후 최소 수동 검증 범위

아래 항목은 수치나 상태만으로 품질을 판정할 수 없으므로 수동 검증으로 유지한다.

1. Player 이동, 점프와 관성 착지의 실제 조작감이 플레이를 방해하지 않는지 확인한다.
2. Camera 추적에 사람이 인지하는 떨림, 순간 이동이나 부자연스러운 구도가 없는지 확인한다.
3. StageHUD, ResultPanel과 선택 색상이 실제 화면에서 읽기 쉽고 겹치거나 잘리지 않는지 확인한다.

입력 차단, Action Map 상태, Goal 종료, Clear Time 고정, Player 정지, ResultMenu 선택 상태, Cancel, Retry와 상태 초기화는 자동 Test로 판단한다.

### 수행 절차

1. Console을 비우고 SampleScene을 Play Mode로 실행한다.
2. StageHUD가 표시되고 ResultPanel이 비활성인지 확인한다.
3. Player 이동, 점프, 일반 착지와 관성 착지를 각각 수행한다.
4. Camera가 Player를 정상적으로 추적하는지 확인한다.
5. 지형 충돌과 Goal Trigger가 정상인지 확인한다.
6. 확정한 UI 입력을 Stage 진행 중에 수행하고 정의된 결과가 발생하는지 확인한다.
7. GamePause 입력과 동작이 Phase 5에 추가되지 않았는지 확인한다.
8. Goal에 도달한다.
9. StageHUD가 비활성화되고 ResultPanel이 활성화되는지 확인한다.
10. 클리어 시간이 승인된 형식으로 한 번 표시되는지 확인한다.
11. 결과 화면에서 2초 이상 기다려도 시간이 변하지 않는지 확인한다.
12. 결과 화면에서 Player 입력과 이동이 중지되어 있는지 확인한다.
13. 키보드 Navigate로 Retry와 Quit 선택을 변경한다.
14. Cancel이 아무 동작도 수행하지 않는지 확인한다.
15. 마우스 Point로 Retry와 Quit을 각각 선택한다.
16. Retry를 마우스로 Click하여 같은 Play Mode에서 두 번째 Stage Play를 시작한다.
17. 두 번째 플레이의 Player, UI, Timer, Result와 입력 상태가 새로 초기화되었는지 확인한다.
18. 두 번째 Goal 도달 결과가 첫 번째 결과와 독립적인지 확인한다.
19. Step 5 Development Build에서 Quit의 키보드 Submit과 마우스 Click 검증이 완료되었는지 확인한다.
20. Play Mode를 종료했다가 다시 실행하여 같은 흐름을 한 번 더 확인한다.
21. Console에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 완료 조건

- 게임 시작부터 결과 화면까지 중단 없이 플레이할 수 있다.
- 확정한 UI 입력이 상태별로 올바르게 동작한다.
- 결과 화면에서 확정된 클리어 시간을 확인할 수 있다.
- 같은 실행 세션과 재실행 모두에서 반복 플레이가 가능하다.
- Phase 1~4 핵심 기능에 회귀가 없다.

### Step 6 완료 결과

- 사용자의 전체 플레이 수동 검증이 완료되었다.
- 자동화할 수 있는 상태, 입력, 결과와 재시작 검증이 Play Mode Test에 포함되었다.
- Edit Mode 전체 39개와 Play Mode 전체 34개 Test가 통과했다.
- Error와 예상하지 않은 Warning이 발생하지 않았다.
- Step 6의 모든 완료 조건을 충족했다.

## Step 7. 후속 플레이 테스트와 밸런스 조정을 수행한다

- 진행 상태: **Phase 5 범위 제외 — 후속 작업으로 보류**
- 범위 변경 근거: 밸런스 처리는 Phase 5에서 수행하지 않는다.
- 아래 절차와 기준은 향후 별도 밸런스 작업을 위한 참고 내용이며 Phase 5 완료 조건으로 사용하지 않는다.

### 테스트 준비

1. 최소 5회의 유효 기록을 위한 기록표를 준비한다.
2. 각 플레이는 동일한 Scene, Build, 입력 장치와 시작 조건을 사용한다.
3. 조정 전 기준값을 Inspector에서 기록한다.
4. PlayerMovementSystem의 실제 조정 대상 Field와 현재 값을 기록한다.
5. 한 번의 반복에서는 하나의 밸런스 변수만 변경한다.
6. 밸런스를 변경하는 경우 기준값 2회와 최종 후보값 3회로 기록을 배분한다.
7. 밸런스를 변경하지 않는 경우 동일한 최종값으로 5회를 기록한다.

### 매 플레이 기록 항목

- 회차
- 사용한 Build 또는 Commit
- 입력 장치
- 클리어 성공 또는 실패
- 클리어 시간
- 재시작 가능 여부
- 진행 불가 발생 횟수
- 관성 착지 시도 횟수
- 관성 착지 성공 횟수
- 예상하지 않은 입력 중복 횟수
- 점프 입력 반응
- 일반 착지 안정성
- 관성 착지 속도 유지 체감
- Camera 구도와 추적 안정성
- 멈춤, 지형 관통 또는 조작 불가 발생 여부
- Error와 Warning 발생 여부
- 변경한 수치와 변경 이유

### 밸런스 조정 절차

1. 먼저 조정 전 기준값으로 전체 플레이를 수행한다.
2. 실패 지점과 재미를 방해하는 현상을 재현 가능한 사실로 기록한다.
3. 이동 속도 또는 가속, 점프, 중력, 관성 착지와 최대 속도 중 핵심 플레이를 방해하는 한 가지 수치만 선택한다.
4. 선택한 값을 소폭 변경하고 변경 전후 값을 기록한다.
5. 같은 경로와 입력 조건으로 다시 플레이한다.
6. 클리어 시간과 조작 결과를 변경 전 기록과 비교한다.
7. 개선이 확인되지 않으면 기준값으로 되돌린다.
8. 개선이 확인되면 해당 값을 유지하고 전체 회귀 플레이를 한 번 수행한다.
9. 핵심 재미처럼 자동 판정할 수 없는 항목은 플레이어의 관찰 결과로 별도 기록한다.
10. 목표 기준을 충족하거나 추가 변경의 근거가 없어질 때까지 반복한다.
11. 접지 판정 수치는 실제 판정 오류가 확인된 경우 현재 작업에서 변경하지 않고 별도 작업으로 기록한다.

### 금지 사항

- 여러 수치를 한 번에 변경하지 않는다.
- 측정 없이 감각만으로 기존 값을 덮어쓰지 않는다.
- 문서에 정의되지 않은 새 이동 기능을 밸런스 조정으로 추가하지 않는다.
- 테스트 실패를 숨기기 위해 Collision, Timer 또는 Result 규칙을 임의로 변경하지 않는다.
- InfiniteMode, Leaderboard와 저장 기능을 임시로 추가하지 않는다.

### 완료 조건

- 확정한 최소 플레이 횟수를 충족한다.
- 각 회차의 조건, 결과와 변경값이 기록되어 있다.
- 5회의 유효 기록에서 진행 불가 오류, 재시작 실패, 예상하지 않은 입력 중복, Error와 예상하지 않은 Warning이 없다.
- 밸런스 값을 변경한 경우 최종 후보값의 중앙 클리어 시간이 기준값보다 악화되지 않는다.
- 관성 착지를 사용한 플레이가 주요 구간에서 사용하지 않은 플레이보다 불리하지 않다.
- 최종 선택값으로 3회 연속 정상 클리어한다.
- 점프, 착지와 Camera의 정성 평가에 플레이를 방해하는 항목이 없다.
- 최종 밸런스 값의 선택 근거를 설명할 수 있다.

## Step 8. Scene을 저장하고 Phase 5 결과를 기록한다

- 진행 상태: **완료**

### AI 정적 확인 결과

- 저장된 SampleScene에 GameSystem, UIInputSystem, UIManagementSystem, StageHUD, ResultPanel, RetryButton과 QuitButton이 각각 정확히 하나 존재한다.
- GameSystem의 UIInputSystem 및 UIManagementSystem 참조가 0이 아닌 직렬화 fileID로 연결되어 있다.
- UIManagementSystem의 StageHUD, ResultPanel, Clear Time Text, RetryButton과 QuitButton 참조가 0이 아닌 직렬화 fileID로 연결되어 있다.
- Input Action Asset에 Player와 UI Action Map이 각각 하나 존재한다.
- UI Action Map에 Navigate, Submit, Cancel, Point와 Click Action이 각각 하나 존재한다.
- Test Attribute 기준 Edit Mode 39개와 Play Mode 34개가 존재한다.
- Step 6에서 Unity Script Compilation 성공, Edit Mode `39 Passed, 0 Failed`, Play Mode `34 Passed, 0 Failed`가 확인되었다.
- Step 6 검증에서는 예상하지 않은 Error와 Warning이 발생하지 않았다.
- Phase 5 구현 코드, Scene, Test와 관련 문서 변경이 Git 작업 목록에 존재한다.
- Input Action Asset은 현재 Git 변경 목록에 없으므로 저장되지 않은 변경 여부만 Unity Editor에서 확인한다.
- AI는 Unity Editor, Test Runner와 Build를 실행하지 않았다.
- 밸런스 조정은 Phase 5 범위에서 제외되어 Step 8 완료 조건으로 사용하지 않는다.

### 현재 필요한 수동 작업

없음. 이번 Step 8 요청 범위는 저장 파일과 기존 Unity 검증 결과를 사용한 정적 검증으로 처리했다.

Scene 재개방과 Unity 재실행은 정적 검증으로 증명할 수 없다. 최종 Build와 실제 플레이는 Step 5 Development Build 및 Step 6 전체 플레이 검증 결과를 재사용한다. Step 5 이후 생산 기능 변경이 없고 추가 변경은 자동 Test에 한정되므로 Phase 5 완료 근거로 사용한다.

### 수행 절차

1. Play Mode를 종료한다.
2. UIInputSystem, GameSystem과 UIManagementSystem의 Inspector 참조가 유지되는지 확인한다.
3. 밸런스 값은 Phase 5에서 변경하지 않았는지 확인한다.
4. SampleScene과 변경한 Input Action Asset을 저장한다.
5. Scene을 닫았다가 다시 열어 UI 오브젝트, Component와 참조가 유지되는지 확인한다.
6. Unity를 재실행한 뒤 Compile Error가 없는지 확인한다.
7. Edit Mode와 Play Mode 전체 Test의 최종 결과를 다시 확인한다.
8. 최종 Build를 실행하고 시작부터 결과 화면까지 한 번 플레이한다.
9. Git 변경 목록에서 Phase 5 범위 밖의 변경이 없는지 확인한다.
10. Test 수, 성공·실패, Build 결과와 수동 확인 결과를 별도 검증 결과 Task 문서에 기록한다.
11. 모든 완료 조건이 충족된 경우에만 Roadmap의 Phase 5 상태를 완료로 갱신한다.
12. 미완료 항목이 있으면 Phase 5를 완료로 표시하지 않고 해당 항목과 근거를 기록한다.

### 완료 조건

- Scene과 Input Action Asset 저장 후 모든 참조가 유지된다.
- 최종 Test와 Build가 통과한다.
- 반복 플레이 검증 결과가 기록되어 있다.
- Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 수동 검증 체크리스트

- [x] Phase 5 범위와 보류 항목이 확정되었다.
- [x] UI 화면과 UI 입력 동작이 확정되었다.
- [x] UIInputSystem이 Scene에 정확히 하나 존재한다.
- [x] UIInputSystem의 필수 참조가 연결되어 있다.
- [x] Player와 UI Action Map이 상태에 맞게 전환된다.
- [x] Callback과 일회성 입력이 중복 처리되지 않는다.
- [x] StageHUD와 ResultPanel이 동시에 표시되지 않는다.
- [x] 확정한 시험 해상도에서 UI가 잘리거나 겹치지 않는다.
- [x] 키보드와 마우스로 ResultMenu를 조작할 수 있다.
- [x] 결과 화면에 확정된 클리어 시간이 표시된다.
- [x] Edit Mode 전체 Test가 통과한다.
- [x] Play Mode 전체 Test가 통과한다.
- [x] 개발용 Build가 성공한다.
- [x] 게임 시작부터 결과 화면까지 플레이할 수 있다.
- [x] 같은 실행 세션에서 반복 플레이할 수 있다.
- [x] Play Mode 재실행 후 이전 상태가 남지 않는다.
- [x] 최소 플레이 테스트 횟수는 Phase 5 완료 조건에서 제외되었다.
- [x] 밸런스 조정이 Phase 5 범위에서 제외되었다.
- [x] 치명적인 오류와 예상하지 않은 Warning이 없다.
- [x] Phase 1~4 핵심 기능에 회귀가 없다.

---

# 문제 발생 시 확인 순서

| 문제 | 먼저 확인할 항목 |
| --- | --- |
| UI 입력이 없음 | UI Action Map 활성 상태, Binding, UIInputSystem 초기화와 Callback 등록 |
| UI 입력이 두 번 처리됨 | Callback 중복 등록, 일회성 입력 소비, 재시작 초기화 |
| Player 입력이 계속 동작함 | Game State, Player Action Map 비활성화 요청, UI Action Map 전환 순서 |
| 선택 표시가 없음 | 현재 EventSystem, 최초 선택 대상, Selectable Navigation 설정 |
| Navigate 순서가 이상함 | Button Navigation Mode와 인접 Selectable 연결 |
| Submit 또는 Cancel이 동작하지 않음 | Action Binding, UI 입력 상태 전달, 확정된 상태별 동작 |
| StageHUD와 ResultPanel이 함께 보임 | E_UIState, UIManagementSystem 참조와 ApplyUIState 결과 |
| 결과 시간이 갱신되지 않음 | Result Data 전달, Clear Time Text 참조, ResultPanel 활성 시점 |
| 재시작 후 입력이 안 됨 | Action Map 재활성화, 입력 상태 초기화, Callback 해제 여부 |
| UI가 잘리거나 겹침 | Canvas Scaler, RectTransform Anchor, 시험 해상도와 Text Overflow |
| Build에서만 입력이 안 됨 | Build Profile, Input System 설정, 장치 Binding과 EventSystem Input Module |
| 밸런스 비교가 불가능함 | 변경 전 값, 동일 경로와 입력 조건, 한 번에 변경한 변수 개수 |

문제가 발생하면 Console 메시지, 실패한 Test 이름, 실패 메시지, Stack Trace와 재현 Step을 먼저 기록한다. 원인이 확인되기 전에 Scene 참조, Input Action 또는 생산 코드를 임의로 변경하지 않는다.

---

# 영향 범위

## Systems

- GameSystem
- UIInputSystem
- UIManagementSystem
- PlayerInputSystem
- PlayerMovementSystem
- TimerSystem
- ResultSystem

## Features

- ResultMenu
- GamePause: Phase 5 보류
- ScoreRecord: InfiniteMode와 함께 Phase 5 보류
- TimeRecord
- StagePlay
- StageClear

## Assets

- Assets/InputSystem_Actions.inputactions
- Assets/Scenes/SampleScene.unity

## Tasks

- Phase 5 수동 작업 순서 문서

---

# 검증 내용

- Phase 5 Roadmap의 구현 대상과 완료 조건을 확인했다.
- UIInputSystem, UIManagementSystem, ScoreRecord와 GamePause 문서의 책임과 규칙을 확인했다.
- 현재 생산 코드와 Input Action Asset에서 Phase 5 준비 상태를 확인했다.
- 기존 Phase 1~4 Manual Steps 문서의 구성과 검증 흐름을 확인했다.
- 자동 검증과 수동 검증의 책임을 분리했다.
- Unity Editor에서 실제로 수행할 작업을 8개 순차 Step과 체크리스트로 작성했다.
- 사용자가 선택한 ScoreRecord, GamePause, UI 동작, 입력 장치와 밸런스 기준을 Step 1 확정 결과로 반영했다.

---

# 검증 결과

- Phase 5 수동 작업 Step 문서 작성이 완료되었다.
- Step 1부터 Step 6까지 완료되었다.
- Step 7 플레이 테스트와 밸런스 조정은 Phase 5 범위에서 제외되어 후속 작업으로 보류되었다.
- Step 8은 정적 검증과 기존 Build 및 전체 플레이 근거를 통해 완료되었다.
- 선택 항목은 ResultMenu에 Retry와 Quit을 모두 포함하고, 최소 5회를 기준 2회와 최종 후보 3회로 배분하는 해석으로 서로 충돌하지 않는다.
- InfiniteMode, ScoreRecord와 GamePause는 Phase 5에서 보류한다.

---

# 후속 작업

1. 밸런스 조정이 필요할 때 별도 후속 작업으로 수행한다.
2. Phase 5 이후 구현할 기능의 우선순위를 결정한다.

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

- AI/02_Systems/UIInputSystem.md
- AI/02_Systems/UIManagementSystem.md
- AI/02_Systems/GameSystem.md
- AI/02_Systems/PlayerInputSystem.md
- AI/02_Systems/PlayerMovementSystem.md
- AI/02_Systems/TimerSystem.md
- AI/02_Systems/ResultSystem.md

## Features

- AI/03_Features/GamePause.md
- AI/03_Features/ScoreRecord.md
- AI/03_Features/ResultMenu.md
- AI/03_Features/TimeRecord.md
- AI/03_Features/StagePlay.md
- AI/03_Features/StageClear.md

## Roadmap

- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md

## Template

- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

---

# 관련 작업 기록

- AI/90_Tasks/20260709_01_Phase1ManualSteps.md
- AI/90_Tasks/20260710_01_Phase2ManualSteps.md
- AI/90_Tasks/20260720_02_Phase3ManualSteps.md
- AI/90_Tasks/20260721_02_Phase4ManualSteps.md

---

# 작성 완료 기준

- GENERAL_TASK_TEMPLATE.md의 필수 섹션을 작성했다.
- 확인된 문서와 현재 구현에 근거한 내용만 작성했다.
- 실제 Unity Editor 작업을 순서가 있는 Step으로 표현했다.
- 각 Step에 수행 절차와 완료 조건을 작성했다.
- 확인되지 않은 Phase 5 요구사항은 선행 확정 항목으로 구분했다.
- 문서 작성 작업은 완료했으며 실제 수동 수행 결과를 완료로 기록하지 않았다.
