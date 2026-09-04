# 작업 정보

## 작업명

Prototype 3 Phase 2 Manual Steps

## 작업 일자

20260904

## 작업 담당자

AI, 사용자

## 작업 상태

작업 절차 작성 완료

---

# 작업 목적

Prototype 3 Phase 2의 Player 수평 자동 이동과 입력 단순화를 구현하고 검증하는 순서를 정의한다.

실질적인 사용자 작업을 규칙 결정, Unity Compile 및 Test Runner, 조건부 Scene 설정, Build와 최소 화면 검증으로 제한한다.

정적 검증과 Unit Test로 판정 가능한 계산, 상태, 입력 계약과 회귀를 수동 판정으로 넘기지 않는다.

---

# 작업 대상

- Stage Mode와 InfiniteMode의 좌측에서 우측 자동 이동
- 자동 이동의 시작, 가속, 목표 속도와 복구 규칙
- Player 좌우 이동 입력 제거
- Jump와 Momentum Landing 입력 유지
- UI Navigate 입력 보존
- Pause, Resume, Result, Retry와 연속 Run의 자동 이동 상태
- InfiniteMode 최소 속도 종료 조건과 Wall 접촉의 관계
- Camera Follow와 Phase 1 충돌 및 낙하 회귀
- Compile, 전체 Test, Build와 최소 화면 검증

---

# 작업 전 상태

- Prototype 3 Phase 1이 완료되었다.
- Player의 수평 속도는 PlayerInputSystem의 Move 입력과 PlayerMovementMath의 Ground 및 Air 가속 계산으로 결정된다.
- PlayerMovementSystem이 이동 결과를 계산하고 PlayerControllerSystem이 Rigidbody에 적용한다.
- PlayerInputSystem은 Move, Jump와 Momentum Landing 입력을 수집한다.
- UIInputSystem의 Navigate 입력은 PlayerInputSystem과 별도 Action Map에서 처리한다.
- Pause는 이동 및 Rigidbody 상태를 보존하고 Resume은 보존된 상태를 복원한다.
- Retry는 새 Runtime Data와 Player 이동 상태를 생성한다.
- InfiniteMode는 수평 속도의 절댓값이 최소 속도 미만인 시간이 유예 시간을 넘으면 종료한다.
- Camera는 Player Transform을 추적한다.
- Phase 1 최종 기준은 Edit Mode `246 Passed, 0 Failed`, Play Mode `124 Passed, 0 Failed`이다.
- Windows Standalone Development Build와 벽 및 모서리 최소 화면 검증이 통과했다.

---

# 조사 내용

- Roadmap Phase 2는 두 Mode 공통 자동 이동, Player 좌우 입력 제거, Jump 및 Momentum Landing 유지, Pause·Retry, Camera와 충돌 회귀를 요구한다.
- Player 수평 이동 계산과 상태의 현재 소유자는 PlayerMovementSystem이다.
- PlayerControllerSystem은 계산된 이동 결과를 Rigidbody에 적용하므로 자동 이동 규칙을 소유하지 않는다.
- Player Move와 UI Navigate는 서로 다른 System 및 Action Map에서 관리되므로 독립적으로 변경하고 검증해야 한다.
- InfiniteMode의 최소 속도 종료 조건은 자동 이동 및 Wall 제한과 직접 상호작용하므로 별도 규칙 확정과 Test가 필요하다.
- Phase 1의 Wall 제한, Ground 우선 이동과 마찰 0 설정은 Phase 2에서도 회귀 검증이 필요하다.

---

# 검증 원칙

- 변경될 계산과 상태 계약은 생산 코드보다 Unit Test로 먼저 고정한다.
- 속도, 가속도, 방향, Frame 경계, 상태 전환과 입력 무시는 자동 Test로 판정한다.
- 실제 Rigidbody, Action Map, Mode 흐름, Pause, Retry, Camera와 충돌 연동은 Play Mode Test로 검증한다.
- 생산 Scene과 Input Action Asset은 정적 검사와 실패 Test로 필요성이 입증된 경우에만 변경한다.
- Test 기대값 삭제, 완화, Ignore와 임의 통과로 기존 회귀를 숨기지 않는다.
- Unity Build와 Test Runner는 사용자가 수행하고 AI는 실행하지 않는다.
- Scene 수정이 필요하면 AI는 Object, Component, Field와 값을 명세하고 사용자가 Unity Editor에서 수행한다.
- Phase 3의 Collectible, Score 통합과 배치 작업은 포함하지 않는다.

---

# 작업 내용

## Step 1. 자동 이동과 InfiniteMode의 미정 규칙을 확정한다

- 진행 상태: **대기**

### 결정 항목

1. 자동 이동 목표 속도와 설정 소유 위치
2. Run 시작 직후 목표 속도 적용 또는 가속 적용 여부
3. Ground와 Air에서 같은 가속 규칙을 사용할지 여부
4. Jump와 Landing 동안 자동 이동 속도를 유지하는 방식
5. 공중 Wall 접촉으로 X 속도가 제한된 뒤 자동 이동을 재개하는 시점
6. Ground 모서리에서 Ground 우선 이동을 유지하는 방식
7. Pause 및 Resume에서 자동 이동 상태와 속도를 보존하는 방식
8. Retry와 새 Run에서 자동 이동 상태를 초기화하는 방식
9. 자동 이동 중 InfiniteMode 최소 속도 미달 종료 조건의 의미
10. Player Move Action 제거 또는 비활성화 범위와 Gamepad 영향

### 권장 기본 원칙

- 자동 이동 방향은 World X 양의 방향으로 고정한다.
- 목표 속도와 가속도는 PlayerMovementSystem이 소유하고 두 Mode에 같은 규칙을 적용한다.
- Run 시작은 기존 Ground 가속을 사용하고 Jump 및 공중에서는 기존 Air 가속으로 목표 속도를 회복한다.
- Jump와 Landing 입력은 수평 자동 이동을 중단하지 않는다.
- 공중 Wall 접촉 중에는 Phase 1 규칙에 따라 Wall 안쪽 X 속도를 0으로 제한하고, 접촉이 사라진 다음 물리 단계부터 자동 가속을 재개한다.
- Ground와 Wall이 함께 검출되면 Ground 이동 우선 규칙을 유지한다.
- Pause는 계산과 Rigidbody 적용을 중단하고 Resume은 같은 Run의 자동 이동 상태를 이어간다.
- Retry와 새 Run은 이전 속도 누적이나 Wall 상태 없이 자동 이동을 새로 시작한다.
- InfiniteMode의 최소 속도 종료는 Wall 접촉으로 강제 제한된 시간만으로 즉시 종료되지 않도록 자동 이동 계약과 함께 재정의한다.
- Player Move 입력은 플레이에 전달하지 않되 UI Navigate Action과 UI Action Map은 변경하지 않는다.

### AI 정적 검증

- 확정 규칙이 PlayerInputSystem, PlayerMovementSystem, PlayerControllerSystem과 InfiniteModeSystem의 책임 경계를 유지하는지 확인한다.
- Mode별로 같은 계산을 중복 구현하지 않는지 확인한다.
- Phase 1 Wall 및 Ground 우선 규칙과 모순되지 않는지 확인한다.

### 사용자 수동 작업

- 사용자는 AI가 제시하는 선택지와 장단점을 검토하여 위 10개 규칙을 결정한다.
- Unity Editor, Scene, Build와 Test Runner 작업은 수행하지 않는다.

### 완료 조건

- [ ] 자동 이동의 속도, 가속, 시작과 복구 규칙이 확정되었다.
- [ ] Pause, Retry와 InfiniteMode 종료 규칙이 확정되었다.
- [ ] Player 입력 제거 범위와 UI 입력 보존 범위가 확정되었다.

## Step 2. 현재 입력부터 Rigidbody까지의 경로를 정적으로 조사한다

- 진행 상태: **대기**

### AI 작업

- PlayerInputSystem의 Move, Jump와 Momentum Landing Callback 및 transient 상태를 조사한다.
- 생성된 Input Action Wrapper와 Input Action Asset에서 Player Move와 UI Navigate Binding을 분리해 기록한다.
- PlayerMovementSystem의 수평 속도, Ground 및 Air 가속, Jump, Landing과 Wall 제한 적용 순서를 조사한다.
- PlayerControllerSystem의 Rigidbody 적용, Pause, Resume와 Retry 경로를 조사한다.
- Stage 및 InfiniteMode 시작과 종료 시 System 호출 순서를 조사한다.
- InfiniteMode 최소 속도 측정값과 종료 유예 상태의 소유 위치를 조사한다.
- Camera Follow 시작, 중단, Resume과 Retry 경로를 조사한다.
- 기존 Edit Mode 및 Play Mode Test의 재사용 지점과 누락 시나리오를 기록한다.
- SampleScene YAML의 관련 Serialized Reference와 현재 설정을 읽기 전용으로 검사한다.

### 정적 검증

- 자동 이동 상태의 단일 소유 위치를 확정한다.
- Player Move 제거가 UI Navigate Binding이나 Action Map을 변경하지 않아도 되는지 확인한다.
- Runtime 코드만으로 구현 가능한 항목과 Asset 또는 Scene 변경이 필요한 항목을 분리한다.
- 자동 이동 때문에 기존 InfiniteMode 종료 조건이 항상 참 또는 항상 거짓이 되는 경로를 확인한다.

### 사용자 수동 작업

- 없음.

### 완료 조건

- [ ] 입력부터 Rigidbody까지의 전체 경로와 책임이 확인되었다.
- [ ] Test 우선 변경 지점과 재사용할 기존 Test가 확정되었다.
- [ ] Scene 및 Input Action Asset 변경 필요 후보가 분리되었다.

## Step 3. 자동 수평 이동 계산을 Unit Test 우선으로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- 정지 상태에서 우측 자동 가속 시작
- 목표 속도 초과 방지
- 음의 X 속도에서 우측 목표 속도로 회복
- Ground 및 Air 가속 규칙
- deltaTime 0과 음수 방어
- 음수 또는 유효하지 않은 설정값 방어
- Jump 및 Landing 여부와 독립적인 자동 목표 방향
- 공중 Wall 접촉 중 X 속도 제한
- Wall 이탈 후 우측 가속 재개
- Ground와 Wall 동시 접촉 중 Ground 우선 이동

### 구현 원칙

- 계산은 Scene, MonoBehaviour와 입력 장치에 의존하지 않는 순수 API로 둔다.
- 자동 방향, 목표 속도와 가속도 계산을 Mode별로 중복하지 않는다.
- 기존 수직 속도, 중력, Jump와 Landing 계산을 변경하지 않는다.
- Transform 직접 이동과 Frame 의존 보정을 사용하지 않는다.

### AI 정적 검증

- 경계값, 부호, overshoot와 deltaTime 독립성을 확인한다.
- 기존 Phase 1 Wall 제한 Unit Test 기대값을 유지하는지 확인한다.
- 반복 계산 경로에 할당과 Log가 없는지 확인한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Edit Mode Test를 실행한다.
- Passed, Failed, 전체 수와 예상하지 않은 Error 및 Warning을 기록한다.

### 완료 조건

- [ ] 자동 이동 순수 계산 Test가 통과한다.
- [ ] Wall, Ground, Jump와 Landing 계산 회귀가 통과한다.
- [ ] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.

## Step 4. PlayerMovementSystem에 Mode 공통 자동 이동 상태를 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Stage와 InfiniteMode에서 같은 자동 이동 계산 사용
- Playing 시작 후 우측 속도 증가
- Jump와 Momentum Landing 중 자동 이동 유지
- Wall 제한 뒤 접촉 해제 시 자동 복구
- Normal Landing 뒤 자동 이동 유지
- 중복 Initialize 또는 Start 요청의 상태 안정성
- End 후 이동 결과 정지

### 구현 원칙

- 자동 이동 상태와 설정은 PlayerMovementSystem에서 소유한다.
- PlayerControllerSystem은 계산 결과를 Rigidbody에 적용하는 기존 책임만 유지한다.
- PlayerInputSystem은 자동 이동 방향을 생성하지 않는다.
- StageSystem과 InfiniteModeSystem에 같은 속도 계산을 복제하지 않는다.

### AI 정적 검증

- 초기화, Playing, Pause, Ending 및 Ended 경계의 실행 여부를 확인한다.
- Runtime Data에 필요한 최소 상태만 추가했는지 확인한다.
- FixedUpdate 반복 경로의 Log, LINQ와 컬렉션 할당을 확인한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Edit Mode 및 Play Mode Test를 실행한다.

### 완료 조건

- [ ] 두 Mode가 같은 자동 이동 경로를 사용한다.
- [ ] Jump, Landing과 Wall 회귀 Test가 통과한다.
- [ ] 종료 상태에서 자동 이동이 적용되지 않는다.

## Step 5. Player 좌우 입력을 제거하고 UI Navigate를 보존한다

- 진행 상태: **대기**

### Test 우선 항목

- Keyboard 좌우 입력이 Player 수평 속도에 영향을 주지 않음
- Gamepad Move 입력이 Player 수평 속도에 영향을 주지 않음
- Jump 입력 유지
- Momentum Landing 입력 유지
- UI Navigate의 위아래 선택 이동 유지
- PausePanel 및 ResultMenu의 Keyboard와 Gamepad Navigate 유지
- Player Action Map과 UI Action Map 전환 유지
- Pause 경계의 transient 입력 제거 유지

### 구현 원칙

- 플레이 이동 계산에서 Move 입력 의존성을 제거한다.
- 사용하지 않는 Player Move 상태와 Callback은 참조 조사 후 제거한다.
- UI Navigate Action, UIInputSystem과 UI Binding은 변경하지 않는다.
- Jump와 Momentum Landing Binding 및 입력 소비 규칙을 유지한다.

### AI 정적 검증

- 생성 Input Action Wrapper를 직접 편집하지 않는지 확인한다.
- Input Action Asset 변경이 필요한 경우 Player Move와 UI Navigate의 Binding ID 및 Action Map을 대조한다.
- 제거된 Player Move 참조가 Runtime, Test와 문서에 남지 않는지 검색한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 입력 Play Mode Test를 실행한다.
- Input Action Asset 변경이 Test로 필요하다고 확정된 경우에만 AI가 제공한 Field 단위 절차를 수행한다.

### 완료 조건

- [ ] Player 좌우 입력이 플레이에 영향을 주지 않는다.
- [ ] Jump와 Momentum Landing 입력이 유지된다.
- [ ] UI Navigate와 Action Map 전환 회귀가 통과한다.

## Step 6. Pause, Resume, Result와 Retry 자동 이동 회귀를 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Pause 중 Player 위치와 자동 이동 정지
- Pause 중 Stage Timer 및 Infinite 거리와 Score 정지
- Resume 후 같은 Run에서 우측 자동 이동 복구
- Resume 직후 입력 잔류가 자동 이동을 변경하지 않음
- Result 및 Ended 상태에서 이동 정지
- Stage 및 InfiniteMode Retry 후 새 Run 자동 이동 시작
- Retry 후 이전 수평 속도, 가속도와 Wall 상태 미잔류
- 연속 두 Run의 자동 이동 상태 독립성

### AI 정적 검증

- 기존 Pause 물리 상태 보존 계약과 자동 이동 초기화 계약을 구분한다.
- Retry가 새 Runtime Data와 새 자동 이동 상태를 생성하는지 확인한다.
- UI Retry의 Keyboard 및 Mouse 단일 실행 기대값을 유지한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Play Mode Test를 실행한다.

### 완료 조건

- [ ] Pause, Resume과 Result 이동 상태가 자동 판정된다.
- [ ] Retry 및 연속 Run 독립성 Test가 통과한다.
- [ ] 기존 UI 입력 회귀가 통과한다.

## Step 7. InfiniteMode 진행 지속 조건을 자동 이동에 맞게 검증한다

- 진행 상태: **대기**

### Test 우선 항목

- 자동 이동 중 최소 속도 이상이면 InfiniteMode 지속
- 시작 유예 시간 유지
- Wall 접촉에 의한 강제 X 제한이 종료 조건을 잘못 확정하지 않음
- Wall 이탈 후 자동 속도 복구와 유예 상태 정상화
- 실제 복구 불가능 상태에서는 확정 규칙에 따라 종료
- 추락 임계값 종료는 자동 이동과 관계없이 유지
- 최대 전진 거리와 Score 비감소 유지
- Pause 동안 속도 유예 시간, 거리와 Score 정지
- Retry 후 거리, Score와 종료 유예 상태 초기화

### 구현 원칙

- 자동 이동 때문에 기존 종료 조건을 삭제하거나 무조건 통과시키지 않는다.
- Wall 접촉 상태는 CollisionSystem 결과를 사용하고 InfiniteModeSystem에서 다시 물리 판정하지 않는다.
- 거리와 Score 계산 규칙을 변경하지 않는다.

### AI 정적 검증

- 종료 조건에 사용하는 속도가 Rigidbody 실제 X 속도인지 계산 목표 속도인지 확정 규칙과 대조한다.
- Wall, Pause와 시작 유예 시간이 서로의 Timer를 잘못 누적하지 않는지 확인한다.
- Stage Mode에 InfiniteMode 전용 상태가 영향을 주지 않는지 확인한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Edit Mode 및 Play Mode Test를 실행한다.

### 완료 조건

- [ ] InfiniteMode가 자동 이동 중 정상적으로 지속된다.
- [ ] Wall 접촉이 잘못된 종료 또는 무한 유예를 만들지 않는다.
- [ ] 추락, 기록과 Retry 회귀가 통과한다.

## Step 8. Camera와 Phase 1 충돌 동작을 통합 회귀로 검증한다

- 진행 상태: **대기**

### Test 우선 항목

- Camera Follow Target이 자동 이동 Player로 유지
- Player 우측 이동에 따른 Camera X 추적
- Pause 동안 Camera 추적 상태 보존
- Resume 및 Retry 후 Camera 추적 복구
- 공중 Wall 접촉 중 자연스러운 낙하
- Ground 모서리 우측 진행
- Wall 이탈 후 자동 이동 및 Landing 복구
- Stage Goal과 Infinite 추락 종료 유지

### AI 정적 검증

- Camera가 Transform을 직접 이동시키거나 Player 속도를 계산하지 않는지 확인한다.
- Phase 1의 `PlayerZeroFriction`, Collider와 Wall 분류 설정이 유지되는지 확인한다.
- 기존 Camera, Collision, Jump, Landing과 Mode Integration Test 기대값을 유지하는지 확인한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Play Mode Test를 실행한다.

### 완료 조건

- [ ] Camera가 자동 이동 Player를 정상 추적한다.
- [ ] Phase 1 벽, 모서리, 낙하와 Landing 회귀가 통과한다.
- [ ] Mode별 기존 종료 흐름이 유지된다.

## Step 9. 생산 Asset과 Scene 변경 필요성을 정적 검사와 Test로 판정한다

- 진행 상태: **대기**

### AI 작업

- Input Action Asset의 Player Move, Jump, Momentum Landing과 UI Navigate Binding을 정적으로 검사한다.
- SampleScene의 PlayerInputSystem, PlayerMovementSystem, Rigidbody와 Camera Serialized Reference를 YAML로 검사한다.
- Runtime 및 Test만으로 완료 조건을 충족하면 Asset과 Scene 변경 불필요를 기록한다.
- Serialized 자동 이동 설정이나 Input Action 변경이 필요한 경우 실패 Test와 정확한 Field 단위 사용자 작업을 먼저 작성한다.

### 조건부 사용자 작업

- AI가 변경 필요성을 정적 근거와 Test로 확정한 경우에만 수행한다.
- 사용자는 지정된 Asset, Object, Component, Field와 값만 Unity Editor에서 변경한다.
- 저장 후 Asset 또는 Scene을 다시 열어 참조와 값 유지를 확인한다.
- Missing Script, Missing Reference, Binding 손실과 의도하지 않은 Scene 변경이 없는지 확인한다.

### 완료 조건

- [ ] Input Action Asset 및 Scene 변경 필요 여부가 근거로 확정되었다.
- [ ] 필요한 경우에만 사용자 작업 명세가 Field 단위로 작성되었다.

## Step 10. 전체 정적 검증과 자동 회귀 Test를 수행한다

- 진행 상태: **대기**

### AI 정적 검증

- 신규 Script, Test와 Asset의 `.meta` 및 GUID를 검사한다.
- Test Ignore, 삭제, 임의 통과, 조건부 제외와 기대값 약화를 검사한다.
- 전체 Player Move 참조와 UI Navigate 보존 상태를 검색한다.
- Serialized Reference, Input Action Binding ID와 Scene fileID를 검사한다.
- Update 및 FixedUpdate의 반복 Log, LINQ와 불필요한 할당을 검사한다.
- Phase 1 충돌 및 마찰 설정과 관련 문서 일치를 검사한다.
- Phase 3 Collectible과 Score 통합 기능이 포함되지 않았는지 확인한다.
- Package manifest, lock JSON과 Build Scene 참조를 검사한다.

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

## Step 11. 사용자가 Build와 최소 화면을 검증하고 AI가 완료 근거를 정리한다

- 진행 상태: **대기**

### Build 전 AI 정적 확인

- 활성 Build Scene, Windows Standalone 설정과 Asset 참조를 확인한다.
- 자동 Test로 판정한 속도 수치, 상태, Frame과 입력 무시 여부를 수동 체크리스트에서 제외한다.

### 사용자 Build 및 최소 화면 검증

1. Windows Standalone Win64/x64 Development Build를 한 번 수행한다.
2. Build 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
3. Stage Mode 시작 후 입력 없이 Player가 좌측에서 우측으로 이동하는지 확인한다.
4. 자동 이동 중 Jump와 Momentum Landing을 각각 한 번 수행한다.
5. Pause 동안 Player가 멈추고 Resume 후 우측 이동을 이어가는지 확인한다.
6. Retry 후 새 Run에서 자동 이동이 다시 시작되는지 확인한다.
7. InfiniteMode에서도 입력 없는 우측 이동, Jump와 Wall 접촉 후 낙하를 한 번 확인한다.
8. Camera가 Player를 놓치거나 눈에 띄게 순간 이동하지 않고 추적하는지 확인한다.
9. Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 수동 검증 제한

- 정확한 목표 속도, 가속도, 위치, 거리, Score, Frame 수와 상태 전환 횟수는 자동 Test 결과를 사용한다.
- 사용자는 자동 이동 방향, 조작감, Camera 추적, 눈에 띄는 떨림과 화면 이탈만 확인한다.
- 빠른 입력, 정밀 타이밍과 반복 횟수를 요구하지 않는다.

### Build 검증 후 AI 작업

- 최종 정적 검증, Compile, Test 수, Build와 최소 화면 결과를 기록한다.
- Asset 및 Scene 변경 여부와 미해결 사항을 기록한다.
- 별도 Phase 2 Verification Result Task 문서를 작성한다.
- 모든 완료 조건 충족 시에만 Roadmap Phase 2를 `완료`로 변경한다.

### 완료 조건

- [ ] Build와 최소 화면 검증 결과가 기록되어 있다.
- [ ] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [ ] Phase 2 범위 밖 기능이 포함되지 않았다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 자동 속도, 가속, Wall 복구, Pause, Retry, Infinite 종료와 입력 제거 규칙 결정
2. 구현 Step 이후 AI가 지정한 Unity Script Compilation 확인
3. AI가 지정한 관련 및 전체 Unity Test Runner 실행
4. Step 9에서 필요성이 입증된 경우에만 Input Action Asset 또는 Scene Field 변경
5. Asset 또는 Scene 변경 시 저장·재개방과 Missing Reference 확인
6. Step 11의 Windows Standalone Development Build 실행
7. 두 Mode의 입력 없는 우측 이동, Jump, Momentum Landing, Pause, Retry, Wall 낙하와 Camera 추적 확인
8. Build와 Player의 예상하지 않은 Error 및 Warning 확인

속도, 가속도, 방향 부호, Frame 수, 입력 소비, 상태 전환, 거리, Score와 Retry 초기화는 수동 작업에 포함하지 않고 정적 검증 또는 자동 Test로 처리한다.

---

# 영향 범위

- PlayerInputSystem
- PlayerMovementSystem
- PlayerControllerSystem
- PlayerMovementMath와 Player Movement Runtime Data
- GameSystem과 GamePause
- Stage Mode와 InfiniteMode
- CameraSystem과 Camera Follow
- Phase 1 CollisionSystem, Wall 및 Ground 이동 회귀
- 조건부 Input Action Asset과 SampleScene Serialized Field
- Edit Mode 및 Play Mode Test
- 관련 System, Feature, Roadmap과 Task 문서

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/PlayerInputSystem.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/03_Features/StagePlay.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_3/20260903_02_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_3/20260904_01_Phase1VerificationResult.md`

---

# 검증 내용

- General Task Template의 필수 섹션 포함 여부를 확인했다.
- Roadmap Phase 2의 구현 대상과 완료 조건이 각 Step 및 완료 조건에 포함되는지 대조했다.
- 계산과 상태는 Edit Mode Unit Test, 실제 System 및 물리는 Play Mode Test, 조작감과 Camera 화면은 최소 수동 검증으로 분리했다.
- 자동 판정 가능한 입력 무시, 속도, 상태, 거리와 Score를 Build 화면 수동 판정에서 제외했다.
- 조건부 Asset 및 Scene 작업에 사전 정적 근거와 실패 Test를 요구하도록 구성했다.
- 관련 문서 경로가 실제로 존재하고 `git diff --check`가 통과함을 확인했다.

## Step 수 적정성 검토

- Step 1~2는 구현 전에 필요한 규칙 확정과 현재 경로 조사로, 확인되지 않은 규칙을 추측하여 구현하지 않기 위해 각각 유지한다.
- Step 3~5는 순수 자동 이동 계산, PlayerMovementSystem 통합, Player 입력 제거와 UI 입력 보존으로 변경 책임과 실패 원인이 달라 분리한다.
- Step 6~8은 Pause 및 Run 생명주기, InfiniteMode 종료 조건, Camera와 Phase 1 충돌 회귀로 서로 다른 System 경계를 검증하므로 각각 유지한다.
- Step 9는 실패 근거가 있을 때만 수행하는 Asset 및 Scene 변경 판정으로, Runtime 구현 Step과 분리해야 사용자 수동 변경을 최소화할 수 있다.
- Step 10은 전체 정적 및 자동 회귀 검증이고 Step 11은 Unity Build와 조작감 및 화면 확인이므로 자동 검증과 수동 검증의 책임을 분리하기 위해 각각 유지한다.
- 11개 Step 사이에 동일한 구현 책임을 중복 수행하는 Step이 없고 Roadmap Phase 2 완료 조건의 누락도 없다.
- 따라서 Step을 병합하거나 추가하지 않고 현재 11개 구성을 유지한다.

---

# 검증 결과

- Roadmap Prototype 3 Phase 2 목표와 완료 조건을 11개 실행 Step으로 분리했다.
- 자동 이동 계산, 상태, 입력 무시와 InfiniteMode 종료 계약을 Unit Test 및 Integration Test 우선 범위로 배치했다.
- Camera 추적과 Phase 1 충돌 회귀를 별도 통합 검증 범위로 배치했다.
- Asset과 Scene 변경은 정적 검사 및 실패 Test로 필요성이 확인된 경우에만 수행하도록 제한했다.
- 사용자의 실제 수동 작업을 규칙 결정, Unity Compile/Test Runner, 조건부 Editor 설정, Build와 최소 화면 확인으로 제한했다.
- Phase 2 구현은 아직 수행하지 않았다.

---

# 후속 작업

Step 1에서 자동 이동과 InfiniteMode의 미정 규칙을 확정한다.

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 2의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 작업보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 검증으로 넘기지 않았다.
- Asset과 Scene 작업을 조건부 최소 범위로 제한했다.
- Phase 3 이후 범위를 분리했다.
