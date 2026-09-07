# 작업 정보

## 작업명

Prototype 3 Phase 2 Manual Steps

## 작업 일자

20260904

## 작업 담당자

AI, 사용자

## 작업 상태

완료

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

- 진행 상태: **완료** (20260905 사용자 권장안 채택)

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

### 확정 규칙

20260905 사용자가 채팅으로 제시된 권장안 `1A, 2A, 3A, 4A, 5A, 6A, 7A, 8A, 9A+9C, 10A`를 채택했다.

아래는 Phase 2 구현에 적용할 확정 계약이다. Runtime, Test, Asset 및 Scene에 구현된 상태를 의미하지 않는다. 현재 System 및 Feature 문서와의 차이는 해당 구현 Step에서 함께 반영한다.

| 항목 | 확정 내용 |
|------|----------|
| 1. 속도 및 소유 위치 | PlayerMovementSystem의 기존 설정을 사용한다. 기본 목표 속도는 `8`, 최대 수평 속도는 `14`이며 Stage와 InfiniteMode에 공통 적용한다. 별도 공통 설정 Asset은 추가하지 않는다. |
| 2. Run 시작 | 자동 방향은 World +X로 고정한다. Playing 시작 시 수평 속도 `0`에서 기본 목표 속도까지 가속한다. |
| 3. Ground 및 Air 가속 | 기존 Ground 가속도 `50`, Air 가속도 `25`를 유지하고 같은 계산 경로에서 접지 상태에 따라 선택한다. |
| 4. Jump 및 Landing | Jump와 Normal Landing은 수평 속도를 별도로 초기화하지 않는다. 기본 속도 `8` 미만에서는 자동 회복하고, 관성 착지로 얻은 우측 초과 속도는 최대 `14`까지 보존한다. Momentum Landing은 기존 배율 `1.15`를 유지한다. 자동 가속 자체로 기본 목표 속도를 초과하지 않는다. |
| 5. Wall 복구 | 공중에서 Wall 안쪽 X 속도만 `0`으로 제한하고 수직 속도와 중력 낙하는 유지한다. 충돌 상태 갱신에서 접촉 해제가 확인된 첫 물리 단계부터 현재 속도로 가속한다. 충돌 전 속도를 저장하여 복원하거나 추가 대기 단계를 두지 않는다. |
| 6. Ground 우선 | Ground와 Wall이 동시에 검출되면 기존 Ground 이동 계산을 우선한다. 실제 장애물 충돌은 Rigidbody가 처리하며 추가 벽 높이 및 접촉 위치 분류는 도입하지 않는다. |
| 7. Pause 및 Resume | 속도, Jump 및 Landing 진행 상태와 종료 유예 시간을 보존한다. Pause 중 이동 계산, 물리 진행과 관련 Timer를 중단하고 Resume에서 같은 Run 상태를 이어간다. 입력 잔류는 기존 규칙대로 정리한다. |
| 8. Retry 및 새 Run | 새 Runtime Data를 생성한다. 속도, 착지 입력, 이전 Wall 상태와 종료 유예 시간을 초기화하고 새 시작 위치에서 충돌 상태를 다시 판정한다. 수평 속도 `0`에서 자동 가속을 시작한다. |
| 9. InfiniteMode 종료 | 물리 결과를 반영한 실제 X 속도의 우측 성분 `max(0, vx)`를 측정한다. 최소 속도 `2`, 시작 유예 `1초`, 일반 저속 유예 `0.5초`, 공중 Wall 제한 추가 유예 예산 `1초`를 적용한다. 상세 시간 계약은 아래에 정의한다. |
| 10. Player Move 제거 | Runtime의 Move 입력 상태, Callback과 이동 계산 의존성을 제거한다. Input Action Asset의 Move 정의는 보존하고 Player Action Map을 활성화할 때마다 Move만 비활성화한다. Keyboard 및 Gamepad Move는 플레이에 영향을 주지 않는다. Jump, Momentum Landing, UI Navigate와 UI Action Map은 유지한다. 생성 Wrapper는 직접 편집하지 않는다. |

위 수치는 Phase 2 초기 구현 기준으로 확정했으며 최종 밸런스 검증 결과가 아니다.

### InfiniteMode 유예 시간 계약

- 시작 유예 `1초` 동안 최소 속도 종료를 판정하지 않고 일반 저속 시간 및 Wall 추가 예산을 소비하지 않는다.
- 시작 유예 종료 후 실제 우측 속도가 최소 속도 `2` 미만이면 저속 상태로 처리한다. 음의 X 속도는 전진으로 인정하지 않는다.
- 저속 상태에서 공중 Wall 제한이 적용되는 동안 남은 추가 예산을 먼저 소비하고 일반 저속 시간 누적은 보류한다.
- 추가 예산이 소진되면 Wall 접촉 중에도 일반 저속 시간을 누적한다. 한 물리 단계에서 예산이 소진되면 남은 시간만 일반 저속 시간에 반영한다.
- 공중 Wall 제한이 없는 저속 상태에서는 일반 저속 시간을 누적한다. 기존에 누적한 저속 시간은 Wall 접촉으로 초기화하지 않는다.
- 일반 저속 시간이 `0.5초` 이상이면 종료한다.
- Wall 해제 및 재접촉만으로 추가 예산을 복원하지 않는다. 실제 우측 속도가 최소 속도 이상으로 회복되면 일반 저속 시간을 `0`으로, 추가 예산을 `1초`로 초기화한다.
- Pause 중 시작 유예, 일반 저속 시간과 추가 예산은 진행하지 않는다. Retry 및 새 Run에서 모두 초기화한다.
- 추락 임계값 종료는 시작 유예 및 Wall 추가 유예와 관계없이 즉시 적용한다.
- Wall 제한 여부는 CollisionSystem의 결과와 Phase 1 제한 규칙을 사용한다. InfiniteModeSystem이 별도로 물리 충돌을 판정하지 않는다.
- 실제 물리 결과 속도의 측정 시점과 전달 경로는 Step 2에서 조사한다. 목표 속도나 Rigidbody 적용 전 이동 계산 결과를 실제 물리 결과로 대체하지 않는다.
- 거리와 Score 계산 규칙은 유지한다.

### 결정 근거 및 구현 시 대조 사항

- 기존 PlayerMovementSystem 설정과 Ground/Air 계산을 재사용하여 두 Mode의 계산 중복을 방지한다.
- 기본 속도와 최대 속도를 구분하여 Momentum Landing의 속도 보상을 유지한다.
- Phase 1의 공중 Wall 제한, 중력 낙하와 Ground 우선 규칙을 보존한다.
- Pause 상태 보존과 새 Run 초기화를 구분하여 Run 간 상태 잔류를 방지한다.
- 실제 우측 속도와 유한한 Wall 추가 예산으로 지상 막힘, 후진 및 반복 Wall 접촉에 의한 잘못된 진행 지속을 방지한다.
- SampleScene의 최소 속도는 `2`이나 InfiniteModeSystem 코드 기본값은 `5`이다. 구현 시 확정 기준 `2`와 대조한다.
- 현재 InfiniteMode는 이동 계산 결과가 저장된 Runtime Data와 속도 절댓값을 사용한다. 실제 물리 결과의 우측 속도를 사용하는 계약으로 변경해야 한다.
- 기존 System 및 Feature 문서는 현재 구현 정의를 유지한다. Step 3~7 구현 시 자동 이동 책임, 입력 범위와 InfiniteMode 종료 규칙을 함께 갱신한다.

### AI 정적 검증

- 확정 규칙이 PlayerInputSystem, PlayerMovementSystem, PlayerControllerSystem과 InfiniteModeSystem의 책임 경계를 유지하는지 확인한다.
- Mode별로 같은 계산을 중복 구현하지 않는지 확인한다.
- Phase 1 Wall 및 Ground 우선 규칙과 모순되지 않는지 확인한다.

### 사용자 수동 작업

- 사용자는 AI가 제시하는 선택지와 장단점을 검토하여 위 10개 규칙을 결정한다.
- Unity Editor, Scene, Build와 Test Runner 작업은 수행하지 않는다.

### 완료 조건

- [x] 자동 이동의 속도, 가속, 시작과 복구 규칙이 확정되었다.
- [x] Pause, Retry와 InfiniteMode 종료 규칙이 확정되었다.
- [x] Player 입력 제거 범위와 UI 입력 보존 범위가 확정되었다.

## Step 2. 현재 입력부터 Rigidbody까지의 경로를 정적으로 조사한다

- 진행 상태: **완료** (20260905 정적 조사)

### 수행 결과

- 상세 근거: [Phase2Step2Investigation](20260905_01_Phase2Step2Investigation.md)
- 입력 수집 → 충돌/현재 속도 수집 → 수평 계산 → Jump/중력 → Landing → Wall 제한 → Rigidbody 적용 → Runtime Data 갱신 경로와 책임을 확인했다.
- 자동 이동 설정과 상태는 PlayerMovementSystem에서 소유하고, InfiniteMode 유예 상태는 InfiniteModeState에서 확장한다.
- Input Action Asset과 생성 Wrapper의 JSON이 일치하고 Player Move/UI Navigate가 독립적으로 정의되어 있다. 입력 Asset 변경 없이 Runtime에서 Move만 비활성화할 수 있다.
- InfiniteMode의 현재 속도는 물리 결과가 아닌 이동 계산 결과이다. 측정/적용 순서를 명시하고 실제 속도 관측을 검증할 후속 변경 지점을 기록했다.
- 기존 Test의 private Move 상태 주입, 속도 절댓값 계약, Infinite 유예 완화와 실제 장치 Test 누락을 분리했다.
- 관련 Scene Serialized Reference와 재질 참조를 읽기 전용으로 확인했다. 새 Wall 예산 Field 등은 구현 및 실패 Test 이후 조건부 변경 후보로 남겼다.
- 사용자 수동 작업은 없다. Runtime, Scene과 Asset을 변경하지 않았고 Unity Compile, Test Runner 및 Build를 실행하지 않았다.

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

- [x] 입력부터 Rigidbody까지의 전체 경로와 책임이 확인되었다.
- [x] Test 우선 변경 지점과 재사용할 기존 Test가 확정되었다.
- [x] Scene 및 Input Action Asset 변경 필요 후보가 분리되었다.

## Step 3. 자동 수평 이동 계산을 Unit Test 우선으로 구현한다

- 진행 상태: **완료 — 사용자 Compile 및 전체 Edit Mode 281개 통과 확인**

### 수행 결과

- 상세 기록 및 사용자 실행 절차: [Phase2Step3AutoMovementMath](20260905_02_Phase2Step3AutoMovementMath.md)
- PlayerMovementMathTests에 자동 계산 Test 35개 case를 먼저 추가한 뒤 CalculateAutoHorizontalSpeed를 구현했다. Unity Test Runner는 실행하지 않았다.
- 기본 속도 회복, 관성 초과 속도 보존, Ground/Air 가속, 비유한 값/음수 설정 방어 및 기존 Wall 제한과의 조합을 Test로 작성했다.
- 기존 수학 함수와 기존 Math Test 21개 case를 보존했다. 생산 PlayerMovementSystem 연결은 Step 4에서 수행한다.
- 사용자 검증 대상은 PlayerMovementMathTests 56, JumpFeatureTests 5, MomentumLandingFeatureTests 9, NormalLandingFeatureTests 2로 정적 집계 총 72개 case이다. 실행 성공 수가 아니다.
- 사용자 보고: Unity Script Compilation 성공, 전체 Edit Mode `281 Passed, 0 Failed, Total 281`. Compile 및 Test 관련 예상하지 않은 Error/Warning 없음.
- 사용자 채팅 보고를 근거로 완료 처리했다. AI는 Test Runner 및 Build를 실행하지 않았다. Step 3의 추가 수동 작업은 없다.

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

- [x] 자동 이동 순수 계산 Test가 통과한다.
- [x] Wall, Ground, Jump와 Landing 계산 회귀가 통과한다.
- [x] Unity Script Compilation에 예상하지 않은 Error와 Warning이 없다.

## Step 4. PlayerMovementSystem에 Mode 공통 자동 이동 상태를 구현한다

- 진행 상태: **완료 — 사용자 Compile, Edit Mode 281개 및 Play Mode 141개 통과 확인**

### 수행 결과

- 상세 기록: [Phase2Step4AutoMovementIntegration](20260905_03_Phase2Step4AutoMovementIntegration.md)
- PlayerMovementSystem이 두 Mode에서 CalculateAutoHorizontalSpeed를 사용하며 Playing 상태에서만 계산한다.
- 같은 Run의 중복 Initialize는 Jump/Landing/Pause를 보존하고, 종료 시 Run/이동 데이터 참조를 해제한다.
- 신규 AutoMovementIntegrationTests 14개 case와 기존 Jump/Landing/Wall/Camera/Retry Test 전제 변경을 작성했다. 기존 회귀 기대값의 삭제나 허용 오차 완화로 대응하지 않았다.
- Scene은 변경하지 않았다. Test 실행 시 생성되는 임시 지형만 자동 이동 조건에 맞게 조정했다.
- 사용자 Play Mode 1개 실패(거리 기대 10000/실제 9999.9834) 보고 후 Infinite 원점/거리/추락 판정을 Rigidbody.position으로 통일했다. 상세: [InfinitePhysicsPositionFix](20260905_04_Phase2Step4InfinitePhysicsPositionFix.md).
- 사용자 재검증 결과 Unity Script Compilation, 전체 Edit Mode 281개 및 전체 Play Mode 141개가 모두 성공했다. Compile과 Test 관련 예상하지 않은 Error/Warning도 없었다.
- AI는 Test Runner 및 Build를 실행하지 않았다. 입력 Action 비활성화는 Step 5, InfiniteMode 실제 속도 및 Wall 추가 유예는 Step 7 범위로 유지한다.

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

- [x] 두 Mode가 같은 자동 이동 경로를 사용한다.
- [x] Jump, Landing과 Wall 회귀 Test가 통과한다.
- [x] 종료 상태에서 자동 이동이 적용되지 않는다.

사용자가 전체 Play Mode Test 141개 성공과 예상하지 않은 Error/Warning 부재를 확인했으므로 위 완료 조건을 충족했다.

## Step 5. Player 좌우 입력을 제거하고 UI Navigate를 보존한다

- 진행 상태: **완료 — 사용자 Compile, Edit Mode 281개 및 Play Mode 141개 통과 확인**

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

### 수행 결과

- 상세 기록: [Phase2Step5PlayerMoveRemoval](20260907_01_Phase2Step5PlayerMoveRemoval.md)
- PlayerInputState에서 HorizontalInput을 제거하고 Jump와 Momentum Landing transient 상태만 유지했다.
- PlayerInputSystem에서 Move 상태와 performed/canceled Callback을 제거했다.
- Player Action Map을 활성화할 때마다 Move Action을 비활성화한다. Jump와 Momentum Landing Action은 활성 상태를 유지한다.
- GameLifecycleIntegrationTests의 Playing, Pause 및 Resume 경계에서 Move, Jump와 Momentum Landing Action 상태를 검증하도록 갱신했다.
- Input Action Asset, 생성 Wrapper, UIInputSystem 및 Scene은 변경하지 않았다.
- 사용자 검증 결과 Unity Script Compilation, 전체 Edit Mode 281개 및 전체 Play Mode 141개가 모두 성공했다. Compile과 Test 관련 예상하지 않은 Error/Warning도 없었다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 입력 Play Mode Test를 실행한다.
- Input Action Asset 변경이 Test로 필요하다고 확정된 경우에만 AI가 제공한 Field 단위 절차를 수행한다.

### 완료 조건

- [x] Player 좌우 입력이 플레이에 영향을 주지 않는다.
- [x] Jump와 Momentum Landing 입력이 유지된다.
- [x] UI Navigate와 Action Map 전환 회귀가 통과한다.

사용자가 전체 Play Mode Test 141개 성공과 예상하지 않은 Error/Warning 부재를 확인했으므로 위 완료 조건을 충족했다.

## Step 6. Pause, Resume, Result와 Retry 자동 이동 회귀를 구현한다

- 진행 상태: **완료 — SetUp 수정 후 사용자 Compile 및 Play Mode 144개 통과 확인**

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

### 수행 결과

- 상세 기록: [Phase2Step6LifecycleRegression](20260907_02_Phase2Step6LifecycleRegression.md)
- Stage Pause 동안 Player 위치, Rigidbody 정지와 PlayTimer 고정을 검증하고 Resume 시 같은 Runtime과 속도 복구 및 transient 입력 제거를 확인하는 Test를 추가했다.
- InfiniteMode Pause 동안 Player 위치, 거리와 Score 고정을 검증하고 Resume 후 같은 Run의 자동 이동과 거리 갱신 복구를 확인하는 Test를 추가했다.
- Stage에서 Pause Retry를 두 번 반복하여 매 Run의 Runtime Data, 수평 속도, 가속도와 Landing 결과가 독립적으로 초기화되고 자동 이동이 다시 시작되는지 검증하는 Test를 추가했다.
- 기존 Ended/Result 정지, Wall 상태 초기화, Infinite Retry 및 Keyboard/Mouse UI 단일 실행 Test를 보존했다.
- 생산 Runtime, Input Action Asset, 생성 Wrapper 및 Scene은 변경하지 않았다.
- 최초 Play Mode 실행에서 AutoMovementIntegrationTests 17개가 공통 SetUp 실패했다. 존재하지 않는 `PlayerControllerSystem` GameObject를 조회한 Test 오류였으며, 실제 Component가 있는 `Player` Object를 조회하도록 수정했다. 상세: [Step6SetupFix](20260907_03_Phase2Step6SetupFix.md).
- SetUp 수정 후 사용자가 Unity Script Compilation과 전체 Play Mode 144개 성공 및 예상하지 않은 Error/Warning 부재를 확인했다. Step 6은 Edit Mode 대상과 생산 코드를 변경하지 않았으므로 직전 전체 Edit Mode 281개 성공 결과를 유지한다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Play Mode Test를 실행한다.

### 완료 조건

- [x] Pause, Resume과 Result 이동 상태가 자동 판정된다.
- [x] Retry 및 연속 Run 독립성 Test가 통과한다.
- [x] 기존 UI 입력 회귀가 통과한다.

사용자가 수정 후 전체 Play Mode Test 144개 성공과 예상하지 않은 Error/Warning 부재를 확인했으므로 위 완료 조건을 충족했다.

## Step 7. InfiniteMode 진행 지속 조건을 자동 이동에 맞게 검증한다

- 진행 상태: **완료 — Scene 설정 및 사용자 Compile/Edit Mode 285개/Play Mode 146개 통과 확인**

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

### 수행 결과

- 상세 기록: [Phase2Step7InfiniteProgress](20260907_04_Phase2Step7InfiniteProgress.md)
- 진행 속도를 `max(0, Rigidbody.linearVelocity.x)`로 변경했다.
- 시작 유예 1초, 최소 속도 2, 저속 유예 0.5초와 Run당 Wall 추가 유예 1초 계약을 구현했다.
- Wall 추가 유예는 접촉 토글로 복구하지 않고 실제 속도 회복 시 저속 누적과 함께 초기화한다.
- 거리, Score 및 추락 종료 규칙은 변경하지 않았다.
- InfiniteModeSystem이 CollisionSystem의 기존 Wall 접촉 결과를 사용하도록 참조를 추가했다.
- 최초 Edit Mode 실행에서 최소 속도 2와 이전 경계 데이터 4.999가 불일치하여 1개가 실패했다. 경계 TestCase를 1.999, 2.0, 2.001 기준으로 수정했으며 생산 코드는 변경하지 않았다.
- SampleScene의 CollisionSystem 참조와 최소 속도/유예 Field가 지정 값으로 저장됐음을 정적으로 확인했다.
- 사용자가 Unity Script Compilation, 전체 Edit Mode 285개 및 전체 Play Mode 146개 성공과 예상하지 않은 Error/Warning 부재를 확인했다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Edit Mode 및 Play Mode Test를 실행한다.

### 완료 조건

- [x] InfiniteMode가 자동 이동 중 정상적으로 지속된다.
- [x] Wall 접촉이 잘못된 종료 또는 무한 유예를 만들지 않는다.
- [x] 추락, 기록과 Retry 회귀가 통과한다.

사용자가 전체 Edit Mode 285개와 Play Mode 146개 성공 및 예상하지 않은 Error/Warning 부재를 확인했으므로 위 완료 조건을 충족했다.

## Step 8. Camera와 Phase 1 충돌 동작을 통합 회귀로 검증한다

- 진행 상태: **완료 — 사용자 Compile, Edit Mode 285개 및 Play Mode 147개 통과 확인**

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

### 수행 결과

- 상세 기록: [Phase2Step8CameraCollisionRegression](20260907_05_Phase2Step8CameraCollisionRegression.md)
- 실제 SampleScene에서 Pause 동안 Player와 Follow Target 위치 고정 및 추적 상태 보존을 검증하는 Camera Test를 추가했다.
- Resume 후 자동 이동 Player X를 Follow Target이 다시 추적하고, Pause Retry 후에도 추적이 활성화되어 Player X와 일치하는지 검증한다.
- 기존 자동 이동 Camera X 추적, Jump 중 고정 Y/Z와 Orthographic 설정 Test를 보존했다.
- 기존 Wall 낙하, 모서리/지형 접촉, Wall 이탈 후 Landing, Stage Goal 및 Infinite 추락 종료 Test를 보존했다.
- Camera 생산 코드, 충돌 설정과 Scene을 AI가 변경하지 않았다.
- 최초 Play Mode 실행에서 Pause 위치의 Vector3 정확 비교가 표시 자릿수 이하 차이로 실패했다. 기존 Camera 검증과 같은 0.05 거리 허용 기준으로 수정했으며 물리 위치 정지는 Step 6 Test의 정확 비교를 유지한다.
- 같은 Vector3 실패가 다시 보고됐지만 현재 157줄은 거리 비교이며 정확 비교 코드는 남아 있지 않았다. 수정 전 Test Assembly가 실행된 결과로 판정하여 현재 Script의 Reimport 및 Compile 후 재실행을 대기한다.
- 현재 Script 재컴파일 후 사용자가 전체 Edit Mode 285개와 Play Mode 147개 성공 및 예상하지 않은 Error/Warning 부재를 확인했다.

### 사용자 수동 작업

- Unity Script Compilation과 지정된 Play Mode Test를 실행한다.

### 완료 조건

- [x] Camera가 자동 이동 Player를 정상 추적한다.
- [x] Phase 1 벽, 모서리, 낙하와 Landing 회귀가 통과한다.
- [x] Mode별 기존 종료 흐름이 유지된다.

사용자가 전체 Play Mode Test 147개 성공과 예상하지 않은 Error/Warning 부재를 확인했으므로 위 완료 조건을 충족했다.

## Step 9. 생산 Asset과 Scene 변경 필요성을 정적 검사와 Test로 판정한다

- 진행 상태: **완료 — Asset/Wrapper/Scene 정적 검사 완료, 추가 변경 불필요**

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

### 수행 결과

- 상세 기록: [Phase2Step9AssetSceneAudit](20260907_06_Phase2Step9AssetSceneAudit.md)
- Player Move, Jump, Momentum Landing과 UI Navigate를 포함한 대상 Action 및 Binding ID가 생성 Wrapper에 모두 존재함을 확인했다.
- Input Action Asset과 생성 Wrapper는 Step 5 방식대로 보존하며 추가 변경하지 않는다.
- SampleScene의 PlayerInputSystem, PlayerMovementSystem, Rigidbody, Camera 및 InfiniteMode 참조가 실제 Component로 해석됨을 확인했다.
- PlayerZeroFriction과 Ground Layer 설정이 유지됨을 확인했다.
- Step 7에서 필요했던 CollisionSystem 참조와 최소 속도/유예 값은 사용자 Scene 설정 후 저장된 상태다.
- 추가 Asset 또는 Scene 변경과 사용자 작업은 필요하지 않다.

### 완료 조건

- [x] Input Action Asset 및 Scene 변경 필요 여부가 근거로 확정되었다.
- [x] 필요한 경우에만 사용자 작업 명세가 Field 단위로 작성되었다.

Step 7의 필수 Scene Field 작업은 Field 단위 절차로 완료됐고, Step 9 검사에서 추가 변경이 불필요함을 확인했다.

## Step 10. 전체 정적 검증과 자동 회귀 Test를 수행한다

- 진행 상태: **완료 — 전체 정적 검증 및 직전 전체 자동 회귀 결과 확인**

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

### 수행 결과

- 상세 기록: [Phase2Step10Verification](20260907_07_Phase2Step10Verification.md)
- 전체 `.meta` 171개의 GUID 중복이 없고 신규 Script의 `.meta` 누락이 없음을 확인했다.
- Test Ignore, Assert.Pass, 조건부 플랫폼 제외, 중복 Test 메서드와 제거된 Move 입력 상태가 없음을 확인했다.
- UI Navigate Callback과 Player Move Runtime 비활성화 경로가 유지됨을 확인했다.
- Serialized Reference, Input Action/Wrapper ID, Scene fileID, PlayerZeroFriction과 Ground Layer 설정을 확인했다.
- 반복 FixedUpdate 경로에 정상 프레임 Log, LINQ 또는 컬렉션 할당이 추가되지 않았음을 확인했다.
- Phase 3 Collectible/Score 통합 생산 코드가 포함되지 않았음을 확인했다.
- Package manifest/lock과 Build Scene 목록이 변경되지 않았고 SampleScene이 활성 Build Scene임을 확인했다.
- 정적 Test 수는 Edit Mode 285개, Play Mode 147개다.
- Step 8 전체 검증 이후 Runtime, Test, Asset과 Scene 변경이 없으므로 사용자 Compile/Edit Mode 285개/Play Mode 147개 성공 결과를 Step 10 근거로 적용했다.

### 완료 조건

- [x] 전체 정적 검증이 통과한다.
- [x] 전체 Edit Mode와 Play Mode Test가 통과한다.
- [x] 예상하지 않은 Error와 Warning이 없다.

직전 전체 자동 회귀 이후 검증 대상을 변경하지 않았으므로 중복 Test 실행 없이 위 완료 조건을 충족했다.

## Step 11. 사용자가 Build와 최소 화면을 검증하고 AI가 완료 근거를 정리한다

- 진행 상태: **완료 — 사용자 Build 및 최소 화면 검증 통과 확인**

### Build 전 AI 정적 확인

- 활성 Build Scene, Windows Standalone 설정과 Asset 참조를 확인한다.
- 자동 Test로 판정한 속도 수치, 상태, Frame과 입력 무시 여부를 수동 체크리스트에서 제외한다.

### Build 전 AI 정적 확인 결과

- `EditorBuildSettings`의 유일한 활성 Build Scene은 `Assets/Scenes/SampleScene.unity`이다.
- 활성 Scene GUID `99c9720ab356a0642a771bea13969a05`가 `SampleScene.unity.meta`와 일치한다.
- Step 9에서 확인한 주요 System 및 Asset 참조에 누락이 없고, 이후 해당 검증 대상을 변경하지 않았다.
- `git diff --check`가 통과했다. 출력된 LF/CRLF 안내는 오류가 아니다.
- 마지막 전체 검증 결과인 Script Compilation 성공, Edit Mode 285개 성공, Play Mode 147개 성공을 적용한다.
- Build와 실제 Player 화면 확인은 사용자의 Windows Standalone 검증 결과를 적용했다.

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

### 사용자 Build 및 최소 화면 검증 결과

- Windows Standalone Development Build가 성공했다.
- Build와 Player에서 예상하지 않은 Error와 Warning이 없었다.
- Stage Mode에서 입력 없이 Player가 오른쪽으로 이동했다.
- Stage Mode에서 Jump와 Momentum Landing 입력이 정상적으로 동작했다.
- Pause 중 게임이 정지하고 Resume 후 게임이 재개되었다.
- Retry 후 Stage가 정상적으로 다시 시작되었다.
- InfiniteMode에서 입력 없이 오른쪽으로 이동하고 Jump가 정상적으로 동작했다.
- InfiniteMode에서 Wall 접촉 후 Player가 정상적으로 바닥으로 떨어졌다.
- Camera가 Player를 정상적으로 추적했으며, 아래로 추락한 Player를 따라가지 않는 기존 정상 동작도 유지되었다.

### Build 검증 후 AI 작업

- 최종 정적 검증, Compile, Test 수, Build와 최소 화면 결과를 기록한다.
- Asset 및 Scene 변경 여부와 미해결 사항을 기록한다.
- 별도 Phase 2 Verification Result Task 문서를 작성한다.
- 모든 완료 조건 충족 시에만 Roadmap Phase 2를 `완료`로 변경한다.

### 완료 조건

- [x] Build와 최소 화면 검증 결과가 기록되어 있다.
- [x] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [x] Phase 2 범위 밖 기능이 포함되지 않았다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

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
- `AI/90_Tasks/Prototype_3/20260905_01_Phase2Step2Investigation.md`
- `AI/90_Tasks/Prototype_3/20260905_02_Phase2Step3AutoMovementMath.md`
- `AI/90_Tasks/Prototype_3/20260905_03_Phase2Step4AutoMovementIntegration.md`
- `AI/90_Tasks/Prototype_3/20260905_04_Phase2Step4InfinitePhysicsPositionFix.md`

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
- Phase 2 Step 1~11을 모두 완료했다.
- 정적 검증, Script Compilation, Edit Mode 285개, Play Mode 147개와 Windows Standalone Development Build가 통과했다.
- Stage Mode와 InfiniteMode의 자동 이동, 핵심 입력, Pause, Resume, Retry, Wall 낙하와 Camera 추적을 실제 Player에서 확인했다.
- Phase 2 범위의 미해결 사항은 없다.

---

# 후속 작업

Prototype 3 Phase 3의 Score Collectible 실행 계획을 작성한다.

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 2의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 작업보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 검증으로 넘기지 않았다.
- Asset과 Scene 작업을 조건부 최소 범위로 제한했다.
- Phase 3 이후 범위를 분리했다.
