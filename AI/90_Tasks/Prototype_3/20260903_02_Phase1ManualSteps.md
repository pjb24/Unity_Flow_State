# 작업 정보

## 작업명

Prototype 3 Phase 1 Manual Steps

## 작업 일자

20260903

## 작업 담당자

AI, 사용자

## 작업 상태

진행 중

---

# 작업 목적

Prototype 3 Phase 1의 벽 충돌과 낙하 동작 안정화를 구현하고 검증하는 순서를 정의한다.

정적 검증과 Unit Test로 판정할 수 있는 항목은 자동화하고, Unity Editor에서만 가능한 Scene 구성, Build와 최종 물리 화면 확인만 사용자 수동 작업으로 분리한다.

---

# 작업 범위

- Ground와 Wall 접촉 구분
- 공중 Wall 접촉 중 아래 방향 낙하 유지
- Wall 방향 수평 이동 제한
- 벽 및 모서리 고정 방지
- Wall 이탈 후 Ground 착지 상태 복구
- 착지 후 Jump, Momentum Landing과 Normal Landing 복구
- Stage Mode와 InfiniteMode 충돌 회귀
- 기존 Ground, Platform, Goal, Pause, Result와 Retry 회귀
- Compile, 전체 Test, Build와 최소 물리 화면 검증

---

# 제외 범위

- Wall Jump
- Wall Slide 및 Wall Cling
- 벽 반동, 벽 타기와 매달리기
- Prototype 3 Phase 2 자동 이동
- Player 좌우 이동 입력 제거
- Prototype 3 Phase 3 Collectible
- Prototype 3 Phase 4 Score 및 UI 확장
- 이동, 중력과 Jump의 최종 밸런스 조정
- 신규 Map Pattern
- CharacterController 전환
- Save, Leaderboard와 신규 상용 아트

---

# 현재 기준 상태

- Prototype 2 Phase 4가 완료되었다.
- Unity Script Compilation 성공과 예상하지 않은 Error·Warning 부재가 확인되었다.
- Edit Mode Test 기준은 `222 Passed, 0 Failed`이다.
- Play Mode Test 기준은 `107 Passed, 0 Failed`이다.
- Player는 CapsuleCollider와 Rigidbody를 사용하며 Rigidbody 기본 중력을 사용하지 않는다.
- PlayerMovementSystem이 중력과 이동 결과를 계산하고 PlayerControllerSystem이 결과 속도를 Rigidbody에 적용한다.
- CollisionSystem은 아래 방향 SphereCast로 Ground 후보와 실제 접촉을 판정한다.
- 현재 Player Collider에는 Physic Material이 연결되어 있지 않다.
- Ground와 Wall을 구분하는 명시적 표면 분류 계약은 아직 없다.

---

# 검증 원칙

- 실패하는 Test 또는 변경될 기대값을 생산 코드보다 먼저 작성한다.
- 벡터, 표면 법선과 속도 보정 규칙은 순수 Edit Mode Unit Test로 우선 검증한다.
- Unity Physics가 필요한 접촉, 낙하, 모서리와 착지는 임시 GameObject를 사용하는 Play Mode Test로 검증한다.
- 생산 Scene을 요구하지 않는 물리 검증은 Test가 임시 Collider와 Rigidbody를 구성한다.
- 실제 Scene 참조와 구조는 Scene 변경 전에 생산 Scene 구조 Test로 확정한다.
- 정적 검사와 자동 Test로 판정한 수치, 상태, 호출 횟수와 Frame 경계는 수동 작업으로 반복하지 않는다.
- 수동 검증은 자연스러운 낙하가 눈에 보이는지, 떨림·고정·관통이 없는지 확인하는 최소 범위로 제한한다.
- AI는 Unity Build와 Unity Test Runner를 실행하지 않는다.
- AI는 Scene과 Input Action Asset을 수정하지 않고 필요한 경우 정확한 사용자 작업 절차만 제공한다.

---

# Step 구성

## Step 1. Wall 접촉과 낙하 동작 규칙을 확정한다

- 진행 상태: **대기**

### 결정 항목

1. Ground로 인정할 표면 법선 또는 최대 경사 기준
2. Wall로 인정할 표면 법선 기준
3. Ground와 Wall이 동시에 감지되는 모서리의 우선순위
4. Wall 방향 수평 속도의 제거 또는 제한 방식
5. Wall 접촉 중 수직 속도와 중력 유지 방식
6. Physic Material 사용 여부와 마찰 규칙
7. 벽 이탈 및 Ground 착지 후 상태 복구 시점
8. 수동 화면 검증에 사용할 최소 벽 시나리오

### 권장 기본 원칙

- Ground는 위쪽을 향하는 유효한 표면만 인정한다.
- Wall 접촉만으로 `IsGrounded`가 true가 되지 않는다.
- Ground와 Wall이 동시에 존재하면 실제 Ground 접촉을 우선하되 Wall 방향 침투 속도만 제한한다.
- Wall 접촉 중 현재의 아래 방향 속도와 중력 누적을 유지한다.
- Wall 방향 성분만 제거하고 벽에서 멀어지는 성분은 허용한다.
- 별도의 Wall Slide 속도, Wall Jump 힘과 Wall 체공 시간을 추가하지 않는다.
- Physic Material은 코드 및 Collider 설정만으로 고정 문제가 해결되지 않을 때만 도입한다.

### 정적 검증

- 확정 규칙이 CollisionSystem, PlayerMovementSystem과 PlayerControllerSystem의 책임 경계를 침범하지 않는지 확인한다.
- Phase 2 자동 이동과 입력 제거가 섞이지 않았는지 확인한다.
- 미정 수치가 근거 없이 생산 코드에 들어가지 않았는지 확인한다.

### 수동 작업

- 사용자는 AI가 제시하는 선택지와 장단점을 검토하고 위 8개 규칙을 결정한다.
- Unity Editor, Scene, Build와 Test Runner 작업은 수행하지 않는다.

### 완료 조건

- [ ] Ground와 Wall 분류 규칙이 확정되었다.
- [ ] Wall 접촉 중 허용·제한할 속도 성분이 확정되었다.
- [ ] Physic Material과 최소 수동 검증 범위가 확정되었다.

## Step 2. 현재 충돌 및 이동 경로를 정적으로 조사한다

- 진행 상태: **대기**

### AI 작업

- Player CapsuleCollider, Rigidbody, Constraints와 Layer 설정을 조사한다.
- CollisionSystem의 SphereCast, 자기 Collider 제외와 Ground 후보 선택 경로를 조사한다.
- PlayerMovementSystem의 중력, 수평 이동, Jump와 Landing 상태 전환 경로를 조사한다.
- PlayerControllerSystem의 Rigidbody 속도 적용과 Pause·Resume 복원 경로를 조사한다.
- Stage, Platform, Goal Collider와 Infinite Pattern의 벽 후보를 조사한다.
- 기존 Unit 및 Integration Test의 재사용 지점과 누락 시나리오를 정리한다.

### 정적 검증

- 벽 고정의 가능한 원인을 마찰, 표면 분류, 속도 보정과 Rigidbody 제약으로 분리한다.
- 변경 책임이 CollisionSystem, PlayerMovementSystem 또는 PlayerControllerSystem 중 어디에 속하는지 확정한다.
- Scene 변경 없이 재현 가능한 부분을 식별한다.

### 수동 작업

- 없음.

### 완료 조건

- [ ] 현재 충돌 데이터와 이동 적용 경로가 확인되었다.
- [ ] Test 우선 변경 지점과 책임 System이 확정되었다.

## Step 3. 표면 분류와 CollisionSystem 접촉 분리를 Test 우선으로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- 위쪽 Ground 법선
- 수직 Wall 법선
- 경계 각도 바로 안쪽과 바깥쪽
- 정규화되지 않은 법선
- zero, NaN과 Infinity 법선
- Ground와 Wall 어느 쪽에도 속하지 않는 표면

### 구현 원칙

- Unity Physics Query와 독립적인 순수 계산 API를 우선한다.
- Ground와 Wall 분류 기준의 소유 위치를 한 곳으로 제한한다.
- 허용 각도 또는 dot 기준은 중복 Serialized Field로 두지 않는다.

### 정적 검증

- Culture, Frame과 Scene에 의존하지 않는 결정적 Test인지 확인한다.
- 경계값과 유효하지 않은 값이 포함되었는지 확인한다.

### CollisionSystem Test 우선 항목

- 바닥 SphereCast는 Ground로 판정
- 수직 벽 Hit는 Ground로 판정하지 않음
- Wall만 접촉한 상태의 `IsGrounded == false`
- Ground 후보 거리와 Wall 접촉 정보의 혼용 방지
- 자기 Collider 및 자식 Collider 제외 유지
- Trigger 무시 유지
- Hit Buffer 경계와 가장 가까운 유효 Ground 선택 유지

### CollisionSystem 구현 원칙

- CollisionSystem은 접촉을 분류하고 결과 데이터만 제공한다.
- CollisionSystem이 Rigidbody 속도나 Player Transform을 직접 변경하지 않는다.
- Momentum Landing Window와 이동 행동은 결정하지 않는다.

### 수동 작업

- 사용자는 구현 후 Unity Script Compilation을 확인한다.
- 사용자는 지정된 Edit Mode 표면 분류 Test와 Play Mode CollisionSystem Test를 실행한다.

### 완료 조건

- [ ] 표면 분류 Unit Test가 통과한다.
- [ ] Ground와 Wall 경계값이 자동 판정된다.
- [ ] Wall 접촉이 Ground 상태를 만들지 않는다.
- [ ] 기존 Ground 탐지와 예측 거리에 회귀가 없다.

## Step 4. Wall 방향 이동 제한과 중력 보존을 Unit Test로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Wall 방향 수평 성분만 제한
- Wall에서 멀어지는 수평 성분 유지
- 아래 방향 수직 속도 유지
- 중력 누적 후 수직 속도 감소 유지
- 위로 이동 중 Wall 접촉 시 Jump 수직 속도 보존
- Ground와 Wall 동시 접촉 시 Ground 이동 유지
- 양쪽 Wall 또는 모서리 입력의 결정적 결과

### 구현 원칙

- 속도 벡터 보정은 가능한 한 순수 계산으로 분리한다.
- PlayerMovementSystem이 이동 결과를 계산하고 PlayerControllerSystem은 결과를 적용한다.
- 벽 접촉을 이유로 전체 속도를 zero로 만들지 않는다.
- Frame rate에 종속된 임의 보정과 Transform 직접 이동을 추가하지 않는다.

### 수동 작업

- 사용자는 Script Compilation과 지정된 Edit Mode Test만 실행한다.

### 완료 조건

- [ ] 벽 방향 이동 제한과 수직 낙하가 동시에 성립한다.
- [ ] Jump와 Ground 이동 계산에 회귀가 없다.

## Step 5. 임시 Physics Fixture로 벽 낙하 통합 Test를 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- 공중 Player가 수직 벽에 접근해 충돌
- 여러 FixedUpdate 동안 Y 위치가 계속 감소
- 벽 접촉 중 Player가 일정 높이에 고정되지 않음
- 벽 Collider 관통 없음
- 벽 하단 또는 모서리 이탈 후 낙하 지속
- 좌측 및 우측 벽 방향의 대칭 동작
- 큰 수평 속도와 작은 수평 속도의 동일한 규칙

### 구현 원칙

- Test가 임시 Rigidbody, CapsuleCollider, Ground와 Wall Collider를 생성하고 종료 시 제거한다.
- 생산 Scene 위치나 사용자의 조작 속도에 의존하지 않는다.
- 위치 변화, 속도 성분과 상태를 수치로 자동 판정한다.

### 수동 작업

- 사용자는 Unity Test Runner에서 지정된 Play Mode Test만 실행한다.

### 완료 조건

- [ ] 벽 접촉 중 낙하와 비관통 Test가 통과한다.
- [ ] 좌우 Wall 방향 회귀가 통과한다.

## Step 6. Wall 이탈, Landing 복구와 기존 충돌 회귀를 자동 Test로 구현한다

- 진행 상태: **대기**

### Test 우선 항목

- Wall 접촉 중 `IsGrounded == false`
- Wall에서 떨어져 Ground 접촉 시 Grounded 전환
- Normal Landing 한 번 발생
- Momentum Landing Window가 유효할 때 Momentum Landing 한 번 발생
- Normal과 Momentum Landing 동시 발생 방지
- 착지 후 다음 Jump 시작 가능
- Retry 후 이전 Wall 및 Landing 상태 제거

### 정적 검증

- Jump, MomentumLanding과 NormalLanding Feature의 기존 계약을 재사용하는지 확인한다.
- Wall 전용 Jump 또는 Landing 상태가 추가되지 않았는지 확인한다.

### 기존 충돌 회귀 Test 우선 항목

- 평지 Grounded 및 GroundDistance
- Platform 위 이동과 낙하
- 경사 또는 모서리 Ground 판정
- Goal Trigger의 Stage Clear 단일 실행
- InfiniteMode Goal 비활성 규칙
- 추락 종료 판정
- Stage 및 Infinite Retry
- Pause 중 물리 정지와 Resume 복구

### 기존 충돌 회귀 정적 검증

- 기존 Test 기대값을 삭제하거나 약화하지 않았는지 확인한다.
- Wall 안정화가 Goal, Timer, Score와 UI 책임에 영향을 주지 않는지 확인한다.

### 수동 작업

- 사용자는 지정된 Edit Mode 및 Play Mode Test만 실행한다.

### 완료 조건

- [ ] Wall 이탈 후 착지 상태가 정상 복구된다.
- [ ] 착지 후 Jump와 Momentum Landing을 다시 수행할 수 있다.
- [ ] 기존 Ground, Platform과 Goal 회귀 Test가 통과한다.
- [ ] Pause, Result와 Retry 흐름에 회귀가 없다.

## Step 7. Stage Mode와 InfiniteMode 벽 충돌 회귀를 자동화한다

- 진행 상태: **대기**

### Test 우선 항목

- 두 Mode에서 동일한 Ground와 Wall 분류
- 두 Mode에서 Wall 접촉 중 낙하 유지
- InfiniteMode 이동 거리와 Score가 실제 최대 전진 거리만 사용
- Wall 접촉이 Infinite 종료 조건을 우회하지 않음
- Stage Goal 도달과 Infinite 추락 종료 유지
- 연속 두 Run의 충돌 및 Landing 상태 독립성

### 수동 작업

- 사용자는 지정된 Play Mode Test만 실행한다.

### 완료 조건

- [ ] Stage와 InfiniteMode 벽 충돌 회귀가 통과한다.
- [ ] Mode별 종료와 기록 흐름이 유지된다.

## Step 8. 생산 Scene 변경 필요성을 Test와 정적 검사로 판정한다

- 진행 상태: **대기**

### AI 작업

- 생산 Scene의 Player Collider, Rigidbody, Ground Layer와 벽 후보 Collider를 YAML로 검사한다.
- 임시 Physics Fixture Test와 생산 Scene 구조를 대조한다.
- 코드와 Test만으로 완료 조건을 충족하면 Scene 변경 불필요를 기록한다.
- Physic Material 또는 Collider 설정 변경이 필요한 경우 먼저 생산 Scene 구조 Test와 정확한 사용자 작업 명세를 작성한다.

### 조건부 사용자 Scene 작업

- AI가 Scene 변경 필요를 정적 근거와 실패 Test로 확정한 경우에만 수행한다.
- 사용자는 AI가 지정한 Object, Component, Field와 값만 Unity Editor에서 변경한다.
- Scene 저장 후 닫았다가 다시 열어 참조 유지를 확인한다.
- Missing Script, Missing Reference와 의도하지 않은 Rigidbody·Collider 변경이 없는지 확인한다.

### 완료 조건

- [ ] Scene 변경 필요 여부가 자동 검증 근거로 확정되었다.
- [ ] 필요한 경우에만 사용자 Scene 작업 명세가 Field 단위로 작성되었다.

## Step 9. 전체 Compile과 자동 회귀 Test를 수행한다

- 진행 상태: **대기**

### AI 정적 검증

- 신규 Script와 Test의 `.meta` 및 GUID를 검사한다.
- Test Ignore, 삭제, 임의 통과와 기대값 약화를 검사한다.
- Collider, Rigidbody와 Serialized Reference를 검사한다.
- Update 및 FixedUpdate의 반복 Log와 불필요한 할당을 검사한다.
- Phase 2 자동 이동, 입력 제거와 Phase 3 Collectible이 포함되지 않았는지 확인한다.
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

## Step 10. 사용자가 Build와 최소 물리 화면을 검증하고 AI가 완료 근거를 정리한다

- 진행 상태: **대기**

### Build 전 AI 정적 확인

- Build Settings의 Scene, Windows Win64/x64 대상과 Asset 참조를 검사한다.
- 자동 Test로 판정한 수치, 상태와 Frame 경계를 수동 체크리스트에서 제외한다.

### 사용자 Build 작업

1. Unity Editor에서 Windows Standalone Win64/x64 Development Build를 한 번 수행한다.
2. Build 성공과 예상하지 않은 Error·Warning 부재를 확인한다.
3. Stage Mode에서 공중 상태로 기존 수직 벽에 접근한다.
4. 벽 방향 이동을 유지해도 Player가 벽에 고정되지 않고 아래로 떨어지는지 확인한다.
5. Ground에 착지한 뒤 Jump와 Momentum Landing을 각각 한 번 수행한다.
6. InfiniteMode에서도 같은 벽 접촉과 낙하를 한 번 확인한다.
7. Player가 벽을 관통하거나 모서리에서 지속적으로 떨리지 않는지 확인한다.
8. Wall Jump, Wall Slide 또는 의도하지 않은 체공이 생기지 않았는지 확인한다.
9. 종료 후 Build Player Log에 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 수동 검증 제한

- 정확한 속도, 위치, 법선, 접촉 Frame 수와 Landing 호출 횟수는 자동 Test 결과를 사용한다.
- 사용자는 자연스러운 낙하, 지속 고정, 눈에 띄는 떨림과 관통 여부만 확인한다.
- 빠른 입력, 정밀한 타이밍과 반복 횟수를 요구하지 않는다.

### Build 화면 판정 항목

- Build 성공
- 두 Mode에서 벽 고정 없이 자연스럽게 낙하
- 착지 후 Jump와 Momentum Landing 가능
- 관통, 지속 떨림과 예상하지 않은 Error·Warning 부재

### Build 검증 후 AI 작업

- 최종 정적 검증, Compile, Test 수, Build와 최소 물리 화면 결과를 기록한다.
- Scene 변경 여부와 미해결 사항을 기록한다.
- 별도 Phase 1 Verification Result Task 문서를 작성한다.
- 모든 완료 조건 충족 시에만 Roadmap Phase 1을 `완료`로 변경한다.

### 수동 작업

- 이전 Step에서 확인하지 못한 새 수동 작업을 추가하지 않는다.

### 완료 조건

- [ ] Build와 최소 물리 화면 검증 결과가 기록되어 있다.
- [ ] 정적 검증, Compile, 전체 Test와 Build가 통과한다.
- [ ] Phase 1 범위 밖 기능이 포함되지 않았다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

---

# 실제 수동 작업 요약

사용자가 직접 수행해야 하는 작업은 아래로 제한한다.

1. Step 1의 Wall, Ground, 속도와 Physic Material 규칙 결정
2. 구현 Step 이후 Unity Script Compilation 결과 확인
3. AI가 지정한 관련 및 전체 Unity Test Runner 실행
4. Step 8에서 자동 검증으로 필요성이 입증된 경우에만 Scene Field 변경
5. Scene 변경 시 저장·재개방과 Missing Reference 확인
6. Step 10의 Unity Development Build 실행
7. 두 Mode의 자연스러운 벽 낙하, 착지 복구, 관통과 지속 떨림 확인
8. Build와 Player의 예상하지 않은 Error·Warning 확인

법선, 속도, 위치, Frame 수, 상태 전환, 호출 횟수와 Retry 초기화는 수동 작업에 포함하지 않고 정적 검증 또는 자동 Test로 처리한다.

---

# 영향 범위

- CollisionSystem
- PlayerMovementSystem
- PlayerControllerSystem
- PlayerCollisionState와 이동 계산 Core
- Jump, MomentumLanding과 NormalLanding
- Stage Mode와 InfiniteMode
- 조건부 SampleScene Rigidbody, Collider 또는 Physic Material
- Edit Mode 및 Play Mode Test
- 관련 Feature, System 문서와 Roadmap

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/CollisionSystem.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/03_Features/Jump.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/NormalLanding.md`
- `AI/03_Features/StagePlay.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_003.md`
- `AI/90_Tasks/Prototype_2/20260903_01_Phase4VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 검증 결과

- Roadmap Prototype 3 Phase 1 목표와 완료 조건을 10개 실행 Step으로 분리했다.
- 순수 표면 분류와 속도 보정을 Edit Mode Unit Test 우선 범위로 고정했다.
- 실제 Physics 접촉과 Landing 복구를 임시 Fixture 기반 Play Mode Test 범위로 고정했다.
- 생산 Scene 변경은 자동 Test와 정적 검사로 필요성이 입증된 경우에만 수행하도록 제한했다.
- Unity Compile/Test Runner, 조건부 Scene 작업, Build와 최소 물리 화면 확인만 사용자 수동 작업으로 분리했다.
- Phase 1 구현은 아직 수행하지 않았다.

---

# 후속 작업

Step 1에서 Wall 접촉과 낙하 동작의 미정 규칙을 확정한다.

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 작성했다.
- Phase 1의 실질적인 사용자 작업을 Step 단위로 작성했다.
- 정적 검증과 Unit Test를 수동 작업보다 우선하도록 구성했다.
- 자동 판정 가능한 항목을 수동 검증으로 넘기지 않았다.
- Scene 작업을 조건부 최소 범위로 제한했다.
- Prototype 3 Phase 2 이후 범위를 분리했다.
