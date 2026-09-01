# 작업 정보

## 작업명

Prototype 2 Phase 3 Manual Steps

## 작업 일자

20260901

## 작업 담당자

AI, 사용자

## 작업 상태

준비 완료

---

# 작업 목적

Prototype 2 Phase 3의 GamePause, Resume, Retry와 Quit 구현 순서를 정의한다.

정적 검증과 Unit Test로 판정할 수 있는 항목을 자동화하고 Unity Editor에서만 확인할 수 있는 작업만 수동 작업으로 분리한다.

---

# 작업 대상

- GamePause Feature
- Pause 상태 전환과 Runtime Data
- GameSystem Pause 흐름
- PlayerInputSystem과 UIInputSystem Action Map 전환
- TimerSystem Pause와 Resume
- Stage Mode와 InfiniteMode 진행 중단
- PausePanel
- Pause 상태의 Resume, Retry와 Quit
- Edit Mode 및 Play Mode Test
- 필요한 최소 Scene 연결

---

# 작업 전 상태

- Prototype 2 Phase 1과 Phase 2가 완료되었다.
- Unity Script Compilation 최종 확인 결과는 성공이다.
- Edit Mode 기준은 `145 Passed, 0 Failed`이다.
- Play Mode 기준은 `67 Passed, 0 Failed`이다.
- GamePause Feature 문서에는 기본 Pause와 Resume 규칙만 정의되어 있다.
- 현재 `E_GameState`와 `E_UIState`에는 Pause 상태가 없다.
- TimerSystem에는 Pause와 Resume API가 이미 존재한다.
- PlayerInputSystem과 UIInputSystem은 Action Map별 입력 활성 상태를 관리한다.
- 현재 생산 Scene에는 Phase 3 PausePanel 연결이 없다.

---

# 조사 내용

- GameSystem은 Playing 상태에서 Player Action Map을, Ended 상태에서 UI Action Map을 사용한다.
- GameSystem은 Retry 시 선택된 Game Mode로 Runtime Data와 관련 System을 새로 초기화한다.
- Stage Mode만 PlayTimer를 사용하고 InfiniteMode는 거리와 Score를 물리 갱신마다 기록한다.
- PlayerMovementSystem, InfiniteModeSystem과 StageSystem의 진행을 Pause 동안 중단할 연결이 필요하다.
- UIInputState에는 Cancel 입력이 존재하지만 Pause 요청 입력의 소유자와 Action Map은 확정되지 않았다.
- Phase 3 Roadmap에는 PausePanel이 포함되지만 Phase 4에는 PausePanel UI 마무리가 별도 범위로 남아 있다.
- 상태값과 호출 횟수는 Test로 판정하고 화면 구성과 실제 조작 흐름만 수동으로 확인해야 한다.

---

# 작업 내용

아래 Step 순서로 Phase 3를 수행한다.

각 구현 Step에서는 실패하는 Test를 먼저 작성하고 최소 생산 코드를 구현한 뒤 관련 회귀 Test를 확인한다.

Unity Script Compilation과 Unity Test Runner 실행은 사용자가 Unity Editor에서 수행한다.

Scene 수정은 사용자가 수행하며 AI는 필요한 오브젝트, Component와 Inspector 연결 방법만 안내한다.

IDE 디버그 기능을 수동 검증 절차로 사용하지 않는다.

---

## Step 1. Phase 3 미정 규칙을 확정한다

- 진행 상태: **대기**

### 결정 항목

1. Pause 요청 입력과 기본 키를 결정한다.
2. Pause와 Resume 요청을 Player Action Map 또는 별도 공용 Action Map 중 어디에서 수집할지 결정한다.
3. Pause 시 `Time.timeScale`을 사용할지 System별 명시적 중단을 사용할지 결정한다.
4. Pause 상태를 `E_GameState`와 Runtime Data에 어떻게 표현할지 결정한다.
5. PausePanel의 Phase 3 최소 표시 범위와 Phase 4 마무리 범위를 구분한다.
6. PausePanel의 기본 선택 항목과 Keyboard·Mouse 입력 범위를 결정한다.
7. Resume, Retry와 Quit의 선택 및 확인 규칙을 결정한다.
8. Quit을 Editor와 Player 환경에서 어떻게 처리할지 결정한다.
9. Pause 중 허용하거나 차단할 입력과 중복 요청 규칙을 결정한다.
10. Stage Mode Timer와 InfiniteMode 거리·Score의 정지 기준을 결정한다.
11. Pause 직전 Rigidbody 속도와 이동 상태의 보존·복원 규칙을 결정한다.
12. Result, Initializing, Ready, Ending과 Ended 상태에서 Pause 요청 처리 규칙을 결정한다.

### 정적 검증

- 확정 규칙이 GamePause Feature와 관련 System 책임을 혼합하지 않는지 확인한다.
- Phase 4 UI 마무리, 저장, Leaderboard와 Prototype 3 기능이 포함되지 않는지 확인한다.

### 수동 작업

사용자가 제안별 장단점을 검토하고 미정 규칙을 선택한다. Unity Editor 작업은 없다.

### 완료 조건

- [ ] 모든 미정 규칙이 확정되었다.
- [ ] 관련 Feature와 System 문서에 확정 규칙이 반영되었다.

## Step 2. 현재 Pause 확장 지점과 회귀 기준을 정적으로 조사한다

- 진행 상태: **대기**

### 조사 대상

- GameSystem 상태 전환과 Action Map 정책
- GameRuntimeData의 Game State
- PlayerInputSystem과 UIInputSystem 입력 구조
- PlayerMovementSystem의 이동 중단·재개 API
- StageSystem과 InfiniteModeSystem의 진행 상태
- TimerSystem의 Pause·Resume 계약
- UIManagementSystem의 UI State와 선택 처리
- Result 상태의 Retry와 Quit 흐름
- SampleScene의 UI Root, EventSystem과 Serialized Reference

### 정적 검증

- 기존 Public API와 변경 예상 지점을 파일별로 기록한다.
- Pause 책임의 중복 구현 후보를 찾는다.
- Scene에서 재사용할 수 있는 UI와 새로 필요한 최소 오브젝트를 구분한다.
- 기존 Test 145/67개의 직접 영향 범위를 정한다.

### 수동 작업

없음.

### 완료 조건

- [ ] 기존 계약과 Phase 3 확장 지점이 정리되었다.
- [ ] 신규·수정 파일 후보와 Scene 수동 범위가 확정되었다.

## Step 3. GamePause 상태와 Runtime Data 계약을 Unit Test로 먼저 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Playing에서 Pause 성공
- Pause 중 중복 Pause 거부
- Pause에서 Resume 성공
- Playing이 아닌 상태의 Pause 거부
- Pause가 아닌 상태의 Resume 거부
- Reset 후 초기 상태 복원
- Retry와 종료 요청에 필요한 상태 전환
- 잘못된 전환에서 상태 불변
- Game State의 Pause 표현
- Pause 중 Runtime Data 보존
- Retry와 EndGame에서 Pause 상태 제거
- Stage와 Infinite Mode의 동일한 상태 계약
- 기존 Game State 전환 회귀

### 구현 원칙

- Scene과 MonoBehaviour에 의존하지 않는 순수 상태 객체를 우선한다.
- 상태 전환 규칙과 GameSystem orchestration을 분리한다.
- Runtime Data에는 System 간 공유 상태만 포함한다.
- 실패 Test를 먼저 확인한 후 최소 생산 코드를 구현한다.

### 정적 검증

- 신규 Class가 파일당 하나인지 확인한다.
- `.meta` 존재와 GUID 중복을 확인한다.
- UnityEngine, UI와 Scene 의존성이 없는지 확인한다.
- Pause 상태의 소유자가 한 곳인지 확인한다.
- Mode별 중복 Pause Data가 생기지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation 결과를 확인한다.
- Unity Test Runner에서 전체 Edit Mode Test를 실행한다.

### 완료 조건

- [ ] 신규 GamePause Unit Test가 통과한다.
- [ ] Pause Runtime Data Test가 통과한다.
- [ ] 기존 Edit Mode 전체 회귀가 통과한다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

## Step 4. Pause 입력과 Action Map 정책을 Test 우선으로 연결한다

- 진행 상태: **대기**

### Test 우선 항목

- Playing에서 Pause 요청 1회 수집
- Pause 진입 후 Player Action Map 비활성화
- Pause 중 필요한 UI 또는 Pause Action Map 활성화
- Resume 후 Player Action Map 복원
- transient 입력 소비와 중복 요청 방지
- Result 상태 입력 흐름 회귀

### 구현 원칙

- 입력 수집은 InputSystem이, 입력 의미 판단은 GameSystem이 담당한다.
- 생성된 Input Action C#을 직접 임시 수정하지 않는다.
- Input Action Asset 변경이 필요하면 Asset을 원본으로 수정하고 Wrapper를 재생성한다.

### 정적 검증

- Callback 등록과 해제 쌍을 확인한다.
- Action Map 동시 활성 정책을 확인한다.
- 기존 Move, Jump와 UI 입력 Binding이 손상되지 않았는지 확인한다.

### 수동 작업

- Input Action Asset 변경이 필요한 경우 Unity Input Actions Editor에서 확정 Binding을 추가한다.
- Generate C# Class를 실행하여 Wrapper를 갱신한다.
- Unity Script Compilation과 관련 입력 Test를 실행한다.

### 완료 조건

- [ ] Pause 요청과 Action Map 전환 Test가 통과한다.
- [ ] 기존 Player 및 UI 입력 Test가 통과한다.

## Step 5. GameSystem Pause·Resume orchestration을 Play Mode Test로 먼저 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Stage Mode Playing에서 Pause 진입
- InfiniteMode Playing에서 Pause 진입
- Pause에서 Resume 후 같은 Mode와 Run 유지
- Pause 중 중복 Pause 거부
- Result와 종료 상태에서 Pause 거부
- Pause와 Stage 종료가 같은 프레임에 요청될 때 단일 유효 전환
- EndGame과 Retry 후 Pause 상태 제거

### 정적 검증

- GameSystem이 실행 순서만 조정하고 각 System 내부 상태를 직접 변경하지 않는지 확인한다.
- Pause와 기존 EndGame·Result 경로가 중복 실행되지 않는지 확인한다.
- 정상 Update 경로에 반복 로그가 추가되지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation 결과를 확인한다.
- 관련 Play Mode Test를 실행한다.

### 완료 조건

- [ ] Mode별 Pause·Resume 통합 Test가 통과한다.
- [ ] 기존 시작, 종료와 Retry Test가 통과한다.

## Step 6. Pause 동안 Stage와 InfiniteMode 진행을 정지한다

- 진행 상태: **대기**

### Test 우선 항목

- Pause 동안 PlayTimer 경과 시간 불변
- Resume 후 기존 경과 시간부터 증가
- Pause 동안 Player 입력과 이동 중단
- Pause 동안 Goal 도달과 Stage 종료 처리 차단 또는 확정 규칙 적용
- Retry 후 새 Timer 생성과 초기화
- 여러 번 Pause·Resume 시 누적 시간 정확성
- Pause 동안 Pattern 재배치 중단
- Pause 동안 InfiniteMode 저속 및 추락 종료 판정 중단
- Pause 동안 거리와 Score 불변
- Resume 후 기존 거리와 Score부터 갱신
- InfiniteMode Retry 후 거리, Score와 확정 상태 초기화
- Pause 직후와 Resume 직후 FixedUpdate 경계

### 정적 검증

- Timer 계산을 Test에 다시 구현하지 않고 TimerSystem Public 결과를 사용한다.
- PauseTimer와 ResumeTimer의 기존 책임을 재사용하는지 확인한다.
- Stage Mode에 Infinite 기록 경로가 연결되지 않는지 확인한다.
- Pause가 Score 계산식이나 거리 규칙을 변경하지 않는지 확인한다.
- InfiniteModeSystem, PlayerMovementSystem과 StageSystem 중단 책임이 중복되지 않는지 확인한다.
- Pattern과 Score가 새 의존성을 만들지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation, 관련 Play Mode Test와 전체 회귀 Test를 실행한다.

### 완료 조건

- [ ] Stage Mode Pause 중 시간과 진행 정지 Test가 통과한다.
- [ ] InfiniteMode Pause 중 모든 진행 정지 Test가 통과한다.
- [ ] Mode별 Resume과 Retry 회귀 Test가 통과한다.
- [ ] 기존 Edit Mode와 Play Mode 전체 회귀가 통과한다.

## Step 7. PausePanel 상태와 선택 규칙을 Unit Test로 먼저 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Pause UI State 진입과 종료
- 기본 선택 항목
- Resume, Retry와 Quit 선택 이동
- Submit과 Cancel의 확정 동작
- Pointer 선택과 Click 처리
- PausePanel 비활성 상태의 입력 거부
- ResultMenu 선택 상태와 독립성
- 초기화와 Retry 후 선택 상태 Reset

### 정적 검증

- UIManagementSystem이 GamePause 규칙을 직접 수행하지 않는지 확인한다.
- Pause 선택 enum과 Result 선택 enum의 책임을 구분한다.
- Phase 4 HUD와 Infinite ResultPanel을 선행 구현하지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation과 관련 PausePanel 및 ResultMenu Edit Mode Test를 실행한다.

### 완료 조건

- [ ] PausePanel 상태 및 선택 Unit Test가 통과한다.
- [ ] 기존 ResultMenu Unit Test가 통과한다.

## Step 8. PausePanel 최소 Scene 구성을 연결한다

- 진행 상태: **대기**

### AI 정적 준비

- 필요한 GameObject, Component, 이름, 계층과 Serialized Field를 명시한다.
- 기존 Canvas, EventSystem과 UIManagementSystem 재사용 여부를 확인한다.
- Scene 변경 전후 YAML의 fileID, Component 개수와 참조를 검사한다.
- 생산 Scene 구조를 검증하는 Play Mode Test를 먼저 추가한다.

### 사용자 Scene 작업

1. AI가 안내한 계층으로 PausePanel Root를 생성한다.
2. 최소한의 Resume, Retry와 Quit Button 및 식별 가능한 Text를 배치한다.
3. 기본 선택과 EventSystem Navigation을 연결한다.
4. UIManagementSystem 또는 필요한 담당 Component의 Serialized Field를 연결한다.
5. PausePanel은 시작 시 비활성 상태로 저장한다.
6. Scene을 저장하고 닫았다가 다시 연다.
7. Missing Script, Missing Reference와 Inspector 값 유지를 확인한다.

### 제한 범위

- Phase 3에서는 기능 검증이 가능한 최소 화면만 구성한다.
- 최종 레이아웃, 아트, 애니메이션과 Mode별 UI 마무리는 Phase 4로 남긴다.

### 완료 조건

- [ ] 저장·재개방 후 PausePanel 참조가 유지된다.
- [ ] 생산 Scene 구조 Play Mode Test가 통과한다.
- [ ] Phase 4 UI가 선행 추가되지 않았다.

## Step 9. Pause 상태의 Resume, Retry와 Quit 통합 흐름을 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Resume 선택 시 동일 Run 복원
- Retry 선택 시 같은 Mode의 새 Run 시작
- Retry 시 Timer, 거리, Score, 물리와 UI 상태 초기화
- Quit 선택 시 확정된 종료 흐름 1회 수행
- Cancel 입력의 확정 동작
- Keyboard Submit과 Mouse Click의 동일 결과
- 빠른 중복 입력에서 실행 1회 보장
- Result 상태의 Retry와 Quit 회귀

### 정적 검증

- Application 종료 호출 지점이 한 곳인지 확인한다.
- Pause Retry와 Result Retry가 공통 시작 흐름을 재사용하는지 확인한다.
- PausePanel이 Result Data를 생성하거나 수정하지 않는지 확인한다.

### 수동 작업

- Unity Script Compilation, 관련 Play Mode Test와 전체 회귀 Test를 실행한다.

### 완료 조건

- [ ] Resume, Retry와 Quit 통합 Test가 통과한다.
- [ ] 기존 ResultMenu 회귀 Test가 통과한다.
- [ ] 기존 Edit Mode와 Play Mode 전체 회귀가 통과한다.

## Step 10. 전체 정적 검증과 자동 회귀 Test를 수행한다

- 진행 상태: **대기**

### 정적 검증 체크리스트

- [ ] GamePause 상태와 전환 규칙의 소유자가 한 곳이다.
- [ ] GameSystem은 Pause 실행 순서만 조정한다.
- [ ] Pause 중 Action Map 활성 정책이 확정 규칙과 일치한다.
- [ ] Pause 중 Timer, Player, Stage와 Infinite 기록이 중단된다.
- [ ] Resume이 Runtime Data와 Run 기록을 초기화하지 않는다.
- [ ] Retry가 같은 Mode의 새 Run을 생성한다.
- [ ] Result 상태에서 Pause를 시작할 수 없다.
- [ ] Pause와 Result UI 상태 및 선택 정보가 분리되어 있다.
- [ ] 신규 Script와 Test `.meta`가 존재하고 GUID가 중복되지 않는다.
- [ ] Scene Serialized Reference와 fileID가 유효하다.
- [ ] 정상 프레임 반복 로그가 추가되지 않았다.
- [ ] Test가 Ignore되거나 기존 기대값이 약화되지 않았다.
- [ ] Save, Leaderboard, Phase 4 UI와 Prototype 3 기능이 추가되지 않았다.
- [ ] 관련 Feature와 System 문서가 구현과 일치한다.

### 자동 Test 체크리스트

- [ ] 신규 GamePause Edit Mode Unit Test
- [ ] Runtime Data와 UI 선택 Edit Mode Unit Test
- [ ] Stage Mode Pause·Resume Play Mode Test
- [ ] InfiniteMode Pause·Resume Play Mode Test
- [ ] Pause Retry·Quit와 입력 Play Mode Test
- [ ] 생산 Scene PausePanel 구조 Test
- [ ] 기존 Edit Mode 전체 회귀
- [ ] 기존 Play Mode 전체 회귀

### 수동 작업

- Unity Script Compilation 결과와 예상하지 않은 Error·Warning을 확인한다.
- Unity Test Runner에서 전체 Edit Mode와 Play Mode Test를 실행한다.

### 완료 조건

- [ ] 정적 검증 체크리스트가 모두 통과한다.
- [ ] 전체 자동 Test가 통과한다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

## Step 11. 최소 수동 플레이로 Pause 화면과 조작을 확인한다

- 진행 상태: **대기**

### 정적으로 대체하는 항목

- Timer, 거리와 Score의 정지 수치
- 상태 전환 성공·실패
- 호출 횟수와 중복 실행
- Action Map 활성 상태
- Retry 초기화와 Mode 유지
- Scene Component와 Serialized Reference

### 사용자 수동 플레이

1. Stage Mode Playing에서 확정 Pause 입력으로 PausePanel을 연다.
2. 화면이 고정되고 Player 조작이 반영되지 않는지 확인한다.
3. Resume 후 화면과 조작이 자연스럽게 이어지는지 확인한다.
4. Pause를 여러 번 반복하여 시각적 순간 이동이나 입력 잔류가 없는지 확인한다.
5. PausePanel의 Keyboard 선택, Submit과 Cancel을 확인한다.
6. PausePanel의 Mouse Hover와 Click을 확인한다.
7. Pause 상태에서 Retry 후 같은 Stage Mode가 정상 시작되는지 확인한다.
8. InfiniteMode에서 같은 Pause, Resume와 Retry 흐름을 확인한다.
9. Quit 동작을 안전한 환경에서 한 번 확인한다.
10. Result 상태에서 PausePanel이 열리지 않는지 확인한다.
11. 전체 과정에서 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 검증 방식 제한

- IDE 디버그 기능을 사용하지 않는다.
- 자동 Test로 판정한 수치와 내부 상태를 수동 관찰로 반복 판정하지 않는다.
- 화면 전환, 실제 입력과 조작의 자연스러움만 수동으로 판단한다.

### 완료 조건

- [ ] Stage와 Infinite Mode의 Pause 화면 및 조작이 정상이다.
- [ ] Resume에 시각적 순간 이동과 입력 잔류가 없다.
- [ ] Keyboard와 Mouse 조작이 정상이다.
- [ ] Retry, Quit과 Result 상태 제한이 정상이다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

## Step 12. Phase 3 완료 근거를 정리한다

- 진행 상태: **대기**

### 수행 절차

1. 저장된 Scene과 Asset 변경을 정적으로 확인한다.
2. 최종 정적 검증 결과를 기록한다.
3. Unity Script Compilation 결과를 기록한다.
4. 전체 Edit Mode와 Play Mode Test 수 및 결과를 기록한다.
5. Step 11 수동 검증 결과와 미해결 사항을 기록한다.
6. 별도 Phase 3 Verification Result Task 문서를 작성한다.
7. 모든 완료 조건 충족 시에만 Roadmap Phase 3를 `완료`로 변경한다.

### 수동 작업

Scene 변경의 저장·재개방, Unity Compile/Test 결과와 최종 화면·조작 확인만 필요하다.

### 완료 조건

- [ ] 정적 검증, Compile과 전체 Test가 통과한다.
- [ ] 최소 수동 플레이 결과가 기록되어 있다.
- [ ] Phase 3 범위 밖 기능이 포함되지 않았다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 영향 범위

- GamePause Feature
- GameSystem과 관련 Input, Timer, Movement, Stage, InfiniteMode 및 UI System
- Core Runtime Data와 상태 enum
- Input Action Asset
- 최소 PausePanel Scene 구성
- Edit Mode 및 Play Mode Test
- 관련 문서와 Roadmap

---

# 검증 내용

- Roadmap Phase 3 목표, 구현 대상과 완료 조건을 확인했다.
- GamePause, GameSystem, TimerSystem과 UIInputSystem 문서를 확인했다.
- 현재 Game State, UI State, Action Map, Timer와 Retry 구조를 조사했다.
- 계산과 상태 규칙은 Edit Mode Unit Test로 우선 검증하도록 배치했다.
- System 실행 순서, 입력, 물리와 Scene 연결은 Play Mode Test로 배치했다.
- 화면 구성과 실제 조작감만 최소 수동 검증으로 분리했다.
- IDE 디버그 기능을 검증 절차에서 제외했다.

---

# 검증 결과

- Prototype 2 Phase 3 수행을 위한 12개 Step을 작성했다.
- 상태와 Runtime Data를 하나의 Unit Test Step으로 통합했다.
- Stage와 InfiniteMode의 진행 정지를 하나의 Mode별 통합 Step으로 구성했다.
- 전체 회귀 Test는 핵심 상태, Mode별 정지, 최종 선택 흐름과 최종 검증 시점에 수행하도록 조정했다.
- 정적 검증과 Test 우선 구현 순서를 정의했다.
- Unity Editor에서 필요한 Input Action, Scene, Compile, Test와 최종 플레이 작업을 명시했다.
- Phase 3 구현은 아직 수행하지 않았다.

---

# 후속 작업

1. Step 1의 Phase 3 미정 규칙을 확정한다.
2. 확정 규칙을 GamePause Feature와 관련 System 문서에 반영한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/PlayerInputSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/02_Systems/TimerSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/StageSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/ResultMenu.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_2/20260901_03_Phase2VerificationResult.md`

---

# 작성 완료 기준

- General Task Template의 모든 필수 섹션을 작성했다.
- 실제 수행 순서를 Step으로 작성했다.
- 정적 검증과 Unit Test를 수동 검증보다 우선 배치했다.
- Unity Editor에서만 가능한 작업을 수동 작업으로 분리했다.
