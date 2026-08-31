# 작업 정보

## 작업명

Prototype 2 Phase 2 Manual Steps

---

## 작업 일자

20260901

---

## 작업 담당자

AI

---

## 작업 상태

작업 준비 완료

---

# 작업 목적

Prototype 2 Phase 2의 InfiniteMode 이동 거리, 이동 거리 기반 Score, ScoreRecord와 InfiniteMode 결과 데이터를 구현하기 위한 작업을 순서가 있는 Step으로 정리한다.

순수 계산과 상태 규칙은 구현 전에 Edit Mode Unit Test로 확정하고, System 연결과 Retry 및 Mode 분리는 Play Mode Test로 검증한다.

Unity Editor에서 직접 확인해야 하는 실제 플레이 결과만 수동 작업으로 남기고, 파일 구조, 코드 참조, Scene 직렬화와 규칙 일치는 정적 검증으로 처리한다.

---

# 작업 대상

## Roadmap

- Prototype 2 Phase 2
- InfiniteMode 이동 거리 기록
- 이동 거리 기반 Score
- ScoreRecord Feature
- InfiniteMode 결과 데이터
- Retry 시 기록 초기화
- Stage Mode TimeRecord 회귀

## 예상 영향 System

- GameSystem
- InfiniteModeSystem
- RuntimeDataSystem
- ResultSystem
- UIManagementSystem

## 예상 영향 Feature

- InfiniteMode
- ScoreRecord
- TimeRecord
- ResultMenu

## 예상 영향 Asset

- Runtime Core 코드
- Runtime Feature 코드
- Runtime System 코드
- Edit Mode Test
- Play Mode Test
- 필요할 경우 SampleScene의 직렬화 설정

---

# 작업 전 상태

- Prototype 2 Phase 1은 완료 상태이다.
- InfiniteMode는 최소 속도 미달 또는 Player Y 추락 임계값으로 종료할 수 있다.
- 하나의 Map Pattern 종류로 만든 두 인스턴스를 반복 재배치한다.
- 현재 Runtime Data에는 Game Mode와 Player Movement Runtime Data가 존재한다.
- Player Movement Runtime Data는 현재 수평 속도를 제공한다.
- 현재 이동 거리와 Score를 관리하는 Runtime Data는 없다.
- ResultSystem은 TimeRecord를 사용하여 Stage Clear 결과만 생성한다.
- ResultData는 Stage Clear 여부와 Clear Time만 제공한다.
- ScoreRecord Feature 문서는 존재하지만 생산 구현과 Unit Test는 없다.
- InfiniteMode 종료 결과는 아직 `Run Ended` 안내만 사용하며 Result Data를 생성하지 않는다.
- Phase 2 Roadmap은 최종 이동 거리와 최종 Score 확정을 요구하지만 HUD와 ResultPanel 표시는 Phase 4 범위이다.
- 현재 기준 전체 Test 결과는 Edit Mode `68 Passed`, Play Mode `60 Passed`이다.
- SaveSystem, Leaderboard, GamePause, InfiniteMode HUD와 다중 Map Pattern은 Phase 2 범위가 아니다.

---

# 조사 내용

## 확인 문서

- `AI/README.md`
- `AI/00_Project/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/README.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/EVENT_RULE.md`
- `AI/01_Rules/LOGGING_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/README.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/03_Features/README.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/TimeRecord.md`
- `AI/03_Features/ResultMenu.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`
- `AI/99_Templates/README.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

## 확인 구현

- `Assets/Scripts/Runtime/Core/GameRuntimeData.cs`
- `Assets/Scripts/Runtime/Core/PlayerMovementRuntimeData.cs`
- `Assets/Scripts/Runtime/Core/ResultData.cs`
- `Assets/Scripts/Runtime/Features/InfiniteModeState.cs`
- `Assets/Scripts/Runtime/Features/TimeRecord.cs`
- `Assets/Scripts/Runtime/Systems/GameSystem.cs`
- `Assets/Scripts/Runtime/Systems/InfiniteModeSystem.cs`
- `Assets/Scripts/Runtime/Systems/RuntimeDataSystem.cs`
- `Assets/Scripts/Runtime/Systems/ResultSystem.cs`
- `Assets/Scripts/Runtime/Systems/UIManagementSystem.cs`
- `Assets/Tests/EditMode`
- `Assets/Tests/PlayMode`
- `Assets/Scenes/SampleScene.unity`

## 확인된 기준

- 이동 거리와 Score의 순수 계산 규칙은 Scene이나 MonoBehaviour에 의존하지 않아야 한다.
- System 간 공유가 필요한 현재 Run 데이터만 Runtime Data로 관리한다.
- ScoreRecord는 InfiniteMode 종료 후 하나의 Run에서 한 번만 최종 기록을 확정한다.
- TimeRecord는 Stage Mode Clear만 담당하고 InfiniteMode Score를 처리하지 않는다.
- ResultSystem은 결과 데이터를 생성하고 관리하지만 결과 UI 표시는 담당하지 않는다.
- Map Pattern 통과 횟수는 Score 계산 입력으로 사용하지 않는다.
- Runtime 결과를 SaveSystem 또는 Leaderboard에 저장하지 않는다.
- UI 완성은 Phase 4 범위이므로 Phase 2에서 UI 범위를 확장하려면 Step 1에서 명시적인 결정이 필요하다.
- 계산과 상태는 Edit Mode Unit Test, 실제 Run 연결과 Retry는 Play Mode Test로 구분한다.

---

# 작업 내용

Phase 2 작업은 아래 Step을 순서대로 수행한다.

각 Step은 이전 Step의 완료 조건을 충족한 후 시작한다.

## Step 1. 이동 거리와 Score의 미확정 규칙을 결정한다

- 진행 상태: **대기**

### 수동 결정 항목

아래 항목은 현재 Roadmap만으로 하나의 구현으로 확정할 수 없으므로 사용자가 결정해야 한다.

1. 이동 거리를 시작 위치부터의 최대 전진 거리로 계산할지, 매 프레임 이동량의 누적으로 계산할지
2. 이동 거리의 진행축을 World X로 고정할지 별도 진행축 설정으로 관리할지
3. Run 시작 거리 원점을 Player 시작 위치로 사용할지 별도 기준 Transform을 사용할지
4. Player가 뒤로 이동할 때 기록 거리를 유지할지 감소시킬지
5. 점프 중 수평 이동과 공중 이동을 거리 기록에 포함할지
6. Pattern 재배치 횟수나 Pattern 위치를 거리 계산에서 완전히 제외할지
7. 이동 거리의 내부 자료형과 소수 정밀도
8. 이동 거리에서 Score로 환산하는 공식과 Inspector 설정 값
9. Score 자료형, 소수점 처리, 반올림 방식과 최소·최대 제한 여부
10. 현재 거리와 Score를 매 FixedUpdate에 갱신할지 다른 갱신 단위를 사용할지
11. InfiniteMode 종료 요청 시점과 GameSystem 종료 처리 중 어느 시점에 최종 값을 고정할지
12. ResultData가 Mode별 필드를 하나의 구조로 가질지 Stage와 Infinite 결과 타입을 분리할지
13. InfiniteMode Result Data에 최종 거리와 최종 Score 외에 포함할 값
14. ScoreRecord 중복 요청을 어느 객체가 차단할지
15. Retry 시 거리, Score, 최종 확정 상태를 초기화하는 소유자와 순서
16. Phase 2에서 ResultPanel에 숫자를 표시할지, 데이터 생성까지만 수행하고 표시는 Phase 4로 유지할지

### 결정 반영 절차

1. 확정된 사용자 경험 규칙은 `InfiniteMode.md`와 `ScoreRecord.md`에 책임이 겹치지 않게 반영한다.
2. TimeRecord와의 분리 규칙은 `TimeRecord.md`에서 기존 계약과 모순되는지 확인한다.
3. System 책임이 변경되는 경우에만 관련 System 문서를 갱신한다.
4. 구현 Class, Method와 직렬화 필드 이름은 Task 문서에만 기록한다.
5. 확정되지 않은 수치나 환산 공식을 임의의 기본값으로 구현하지 않는다.

### 적극적 정적 검증

1. `Distance`, `Score`, `ScoreRecord`, `TimeRecord`, `ResultData`를 전체 검색한다.
2. 같은 계산 규칙이나 설정 값이 두 Feature 또는 System에 중복 정의되지 않았는지 확인한다.
3. Pattern 통과 횟수가 Score 입력으로 정의된 문서가 없는지 확인한다.
4. Phase 2에 SaveSystem, Leaderboard, Pause와 Phase 4 UI 구현이 섞이지 않았는지 확인한다.

### 완료 조건

- [ ] 16개 결정 항목이 모두 확정되었다.
- [ ] Feature 규칙과 System 책임이 충돌하지 않는다.
- [ ] 미확정 수치나 타입을 가정한 구현 항목이 없다.

### 수동 작업

사용자의 규칙 결정이 필요하다. Unity Editor 작업은 필요하지 않다.

## Step 2. 현재 구조와 변경 범위를 정적으로 확정한다

- 진행 상태: **대기**

### 수행 절차

1. GameRuntimeData, PlayerMovementRuntimeData와 ResultData의 현재 필드와 Public API를 목록화한다.
2. InfiniteModeSystem의 시작, FixedUpdate, 종료 요청과 Stop 순서를 정리한다.
3. StageSystem 종료 이벤트에서 GameSystem과 ResultSystem으로 이어지는 호출 순서를 정리한다.
4. TimeRecord와 Stage Result 생성 경로를 변경 없이 유지할 API를 확인한다.
5. 기존 Edit Mode와 Play Mode Test가 직접 사용하는 ResultData 계약을 확인한다.
6. 현재 Run 동안 공유할 거리와 Score, System 내부 계산 상태, 최종 Result Data를 구분한다.
7. 새 순수 Feature, Runtime Data, System 변경과 Test 파일 후보를 파일 단위로 정리한다.
8. Scene 직렬화 참조가 필요한 변경과 코드만으로 가능한 변경을 구분한다.

### 적극적 정적 검증

1. `rg`로 ResultData 생성자와 ResultSystem API의 모든 호출부를 검색한다.
2. GameSystem의 Mode별 종료 처리와 Timer 제거 순서를 확인한다.
3. InfiniteMode Retry가 Runtime Data를 새로 생성하는지 확인한다.
4. 기존 Stage Mode Test가 요구하는 이름, 생성자와 Property를 유지할 수 있는지 확인한다.
5. 변경 후보가 Phase 2 책임과 직접 관련되는지 Git 변경 목록 기준으로 대조한다.

### 완료 조건

- [ ] 기존 Stage 결과 계약과 Phase 2 확장 지점이 구분되었다.
- [ ] 신규 및 수정 파일 후보가 확정되었다.
- [ ] Scene 수동 변경 필요 여부가 근거와 함께 확정되었다.

### 수동 작업

없음. 코드와 저장된 Scene의 정적 조사로 처리한다.

## Step 3. 이동 거리 상태 규칙을 Unit Test 우선으로 구현한다

- 진행 상태: **대기**

### Test 선행 절차

1. Scene과 MonoBehaviour에 의존하지 않는 이동 거리 상태 Feature의 Test를 먼저 작성한다.
2. 초기화 전 갱신 거부를 검증한다.
3. Run 시작 시 거리 원점과 현재 거리가 초기화되는지 검증한다.
4. 확정된 진행 방향의 전진 이동이 거리로 반영되는지 검증한다.
5. 후진, 정지, 점프와 공중 이동을 Step 1 결정대로 처리하는지 검증한다.
6. 큰 World X와 Pattern 반복 이후에도 연속적인 거리 결과를 제공하는지 검증한다.
7. 잘못된 수치, 음수 delta 또는 비정상 입력 처리 규칙을 검증한다.
8. 종료 후 추가 갱신이 무시되는지 검증한다.
9. Reset 후 이전 Run 거리가 남지 않는지 검증한다.

### 생산 코드 절차

1. 실패하는 Unit Test를 기준으로 최소 생산 코드를 작성한다.
2. 이동 거리 상태는 자신의 초기화, 갱신, 확정과 Reset만 담당하게 한다.
3. Player Transform, Rigidbody, Pattern과 Scene 참조를 순수 상태 Class에 넣지 않는다.
4. Pattern AdvanceCount를 거리 입력으로 사용하지 않는다.
5. 정상 갱신마다 로그를 출력하지 않는다.

### 적극적 정적 검증

1. 신규 Class가 파일 하나에 하나만 존재하는지 확인한다.
2. UnityEngine 의존성이 필요한지 확인하고 불필요하면 제거한다.
3. nullable 금지 문법과 코드 스타일 위반을 검색한다.
4. Pattern 관련 타입 참조가 거리 상태 Feature에 없는지 확인한다.
5. 신규 Script와 Test `.meta` GUID의 존재와 중복 여부를 확인한다.

### 완료 조건

- [ ] 이동 거리 규칙의 정상, 경계, 거부와 Reset Test가 먼저 작성되었다.
- [ ] 신규 Unit Test가 통과한다.
- [ ] 이동 거리 계산이 Scene과 Pattern 횟수에 의존하지 않는다.

### 수동 작업

없음. 순수 규칙은 Edit Mode Unit Test와 정적 검증으로 처리한다.

## Step 4. Score 계산 규칙을 Unit Test 우선으로 구현한다

- 진행 상태: **대기**

### Test 선행 절차

1. 거리 입력을 Score로 변환하는 순수 계산 Test를 먼저 작성한다.
2. 거리 `0`, 최소 양수, 경계값과 큰 값의 Score를 검증한다.
3. Step 1에서 결정한 환산 비율과 반올림 규칙을 검증한다.
4. 음수, NaN, Infinity와 범위 초과 입력 처리 규칙을 검증한다.
5. 같은 거리에 항상 같은 Score가 생성되는지 검증한다.
6. Pattern 통과 횟수를 입력으로 받지 않는 API인지 검증한다.

### 생산 코드 절차

1. 실패하는 Unit Test를 통과시키는 최소 Score 계산 Feature를 작성한다.
2. 환산 설정의 소유자를 한 곳으로 제한한다.
3. Score 계산과 최종 기록 확정을 분리한다.
4. UI 문자열 형식과 저장 기능을 Score 계산에 포함하지 않는다.

### 적극적 정적 검증

1. Score 환산 수치가 여러 Class나 Scene에 중복 직렬화되지 않았는지 검색한다.
2. Score 계산 Class가 ResultSystem, UI와 Pattern을 참조하지 않는지 확인한다.
3. 자료형 변환에서 overflow 또는 정밀도 손실 가능성을 확인한다.
4. 신규 Test가 구현 세부가 아니라 확정 규칙을 검증하는지 확인한다.

### 완료 조건

- [ ] Score 환산과 반올림 규칙을 검증하는 Unit Test가 먼저 작성되었다.
- [ ] 경계 및 잘못된 입력 Test가 통과한다.
- [ ] Score 계산이 이동 거리 외의 진행 지표에 의존하지 않는다.

### 수동 작업

없음. 계산 규칙은 Edit Mode Unit Test로 처리한다.

## Step 5. InfiniteMode Run Runtime Data를 Unit Test 우선으로 확장한다

- 진행 상태: **대기**

### Test 선행 절차

1. 현재 거리, 현재 Score와 최종 확정 상태의 Runtime Data Test를 먼저 작성한다.
2. Stage Mode와 InfiniteMode 생성 시 초기 상태를 각각 검증한다.
3. InfiniteMode Run 중 거리와 Score 갱신을 검증한다.
4. 최종 확정 후 값이 변경되지 않는지 검증한다.
5. Runtime Data Clear와 Retry 재생성 시 모든 값이 초기화되는지 검증한다.
6. 이전 Run 객체가 새 Run 데이터에 영향을 주지 않는지 검증한다.

### 생산 코드 절차

1. System 간 공유가 필요한 값만 Runtime Data에 추가한다.
2. 계산 Feature의 내부 상태와 Result Data를 Runtime Data에 중복 저장하지 않는다.
3. GameRuntimeData 생성, 초기화와 Clear 계약을 유지한다.
4. Stage Mode에서 Phase 2 데이터가 활성 진행 상태처럼 보이지 않게 한다.

### 적극적 정적 검증

1. Runtime Data 필드와 Property가 실제 System 간 공유되는지 호출부를 확인한다.
2. 같은 거리 또는 Score가 여러 Runtime Data 객체에 중복 저장되지 않았는지 확인한다.
3. 생성과 Reset 경로에서 누락된 필드가 없는지 대조한다.
4. 기존 GameRuntimeDataTests와 Public API 호환성을 확인한다.

### 완료 조건

- [ ] Runtime Data 생성, 갱신, 확정, Clear와 재생성 Test가 통과한다.
- [ ] Retry 후 이전 Run 값이 남지 않는다.
- [ ] Stage Mode와 InfiniteMode 데이터 상태가 혼합되지 않는다.

### 수동 작업

없음. Runtime Data 상태는 Unit Test로 처리한다.

## Step 6. ScoreRecord와 Mode별 Result Data를 Unit Test 우선으로 구현한다

- 진행 상태: **대기**

### Test 선행 절차

1. ScoreRecord의 성공 조건과 결과를 검증하는 Unit Test를 먼저 작성한다.
2. InfiniteMode가 종료되고 최종 거리와 Score가 확정된 경우만 기록되는지 검증한다.
3. 하나의 Run에서 두 번째 기록 요청이 거부되는지 검증한다.
4. 종료 전 요청, Stage Mode 요청과 잘못된 값이 거부되는지 검증한다.
5. Reset 후 다음 Run 기록이 가능한지 검증한다.
6. TimeRecord와 ScoreRecord가 같은 종료에서 동시에 기록되지 않는지 검증한다.
7. ResultData가 Stage Clear Time과 InfiniteMode 거리·Score를 명확히 구분하는지 검증한다.
8. 기존 TimeRecordTests를 수정하지 않고 통과시키는 호환 방식을 우선 검토한다.

### 생산 코드 절차

1. 실패하는 Unit Test를 기준으로 ScoreRecord 생산 코드를 작성한다.
2. ResultSystem이 Mode에 맞는 Record Feature 하나만 사용하도록 확장한다.
3. ResultData에는 Step 1에서 확정한 Mode별 결과 계약만 포함한다.
4. 기록 저장, Leaderboard, UI 문자열과 평가는 구현하지 않는다.
5. ResultSystem Initialize에서 두 Record의 이전 Run 상태를 모두 제거한다.

### 적극적 정적 검증

1. ResultData 생성자와 Property의 모든 사용처를 검색한다.
2. TimeRecord와 ScoreRecord가 서로의 데이터를 생성하지 않는지 확인한다.
3. ResultSystem의 Mode 분기가 Feature 규칙을 중복 계산하지 않는지 확인한다.
4. 기록 완료 Guard가 하나의 책임 객체에만 존재하는지 확인한다.
5. 기존 StageGoalIntegrationTests의 기대값이 불필요하게 변경되지 않았는지 확인한다.

### 완료 조건

- [ ] ScoreRecord 정상, 중복, Mode 분리와 Reset Unit Test가 통과한다.
- [ ] InfiniteMode Result Data에 최종 거리와 Score가 존재한다.
- [ ] Stage TimeRecord 회귀 Test가 통과한다.

### 수동 작업

없음. 기록 규칙과 Result Data 계약은 Unit Test로 처리한다.

## Step 7. InfiniteModeSystem에 거리와 Score 갱신을 연결한다

- 진행 상태: **대기**

### 수행 절차

1. Run 시작 시 Player 진행축 원점과 거리 상태를 초기화한다.
2. InfiniteMode Playing 상태에서만 Player 위치를 거리 Feature에 전달한다.
3. 계산된 현재 거리와 Score를 Runtime Data에 반영한다.
4. Stage Mode에서는 거리와 Score 갱신을 수행하지 않는다.
5. 저속 종료와 Y 추락 종료 모두 같은 최종 확정 경로를 사용한다.
6. 종료 요청이 중복되어도 거리와 Score를 한 번만 확정한다.
7. Stop과 Retry에서 계산 상태와 Runtime Data 연결을 초기화한다.
8. 신규 직렬화 설정이나 참조가 실제로 필요한 경우에만 SampleScene에 추가한다.

### Play Mode Test

1. 실제 Player 수평 이동으로 거리가 증가하는지 검증한다.
2. Player가 뒤로 이동할 때 Step 1 규칙대로 거리가 처리되는지 검증한다.
3. 큰 World X에서도 거리와 Score가 정상 갱신되는지 검증한다.
4. Pattern 재배치 횟수와 무관하게 Player 진행 거리만 반영되는지 검증한다.
5. Stage Mode에서는 Phase 2 진행 데이터가 갱신되지 않는지 검증한다.

### 적극적 정적 검증

1. InfiniteModeSystem이 Score 공식이나 Result Data 생성을 직접 구현하지 않는지 확인한다.
2. FixedUpdate 정상 경로에 반복 로그가 없는지 확인한다.
3. Player, RuntimeDataSystem과 설정 필드의 Scene 참조를 확인한다.
4. 기존 최소 속도와 Y 추락 판정 순서가 변경되지 않았는지 대조한다.
5. Scene을 변경했다면 신규 fileID, 필수 Component 개수와 Missing Script 가능성을 검사한다.

### 완료 조건

- [ ] 실제 Player 진행이 Runtime Data 거리와 Score에 연결된다.
- [ ] Stage Mode에서는 갱신되지 않는다.
- [ ] 기존 InfiniteMode 종료 조건이 유지된다.

### 수동 작업

없음. 실행 연결은 Play Mode Test와 Scene YAML 정적 검증으로 처리한다.

## Step 8. GameSystem 종료 흐름에 ScoreRecord를 연결한다

- 진행 상태: **대기**

### 수행 절차

1. Stage 종료 이벤트 처리 시 현재 Mode를 확인한다.
2. Stage Mode Clear는 기존 Timer와 TimeRecord 경로를 유지한다.
3. InfiniteMode 종료는 최종 거리와 Score를 확정한 뒤 ScoreRecord를 한 번 수행한다.
4. InfiniteMode에서는 Clear Time Result를 생성하지 않는다.
5. 생성된 InfiniteMode Result Data를 기존 UI 전달 경로에 제공한다.
6. Result Data 생성 실패 시 기존 오류 처리 규칙을 따른다.
7. Retry 시작 시 이전 Result Data와 Phase 2 Runtime Data가 제거되는지 확인한다.
8. Step 1에서 데이터 생성까지만 수행하기로 결정한 경우 새 UI를 추가하지 않는다.
9. 표시가 포함된 경우에도 확정된 최소 필드만 기존 ResultPanel 경로에 전달하고 Phase 4 UI를 선행 구현하지 않는다.

### Play Mode Test

1. 저속 종료 시 최종 거리·Score Result Data가 한 번 생성되는지 검증한다.
2. Y 추락 종료 시 동일한 Result Data 계약을 사용하는지 검증한다.
3. 같은 프레임의 복수 종료 요청에도 ScoreRecord가 한 번만 수행되는지 검증한다.
4. InfiniteMode에서 Clear Time이 생성되지 않는지 검증한다.
5. Retry 후 Result Data와 현재 거리·Score가 초기화되는지 검증한다.
6. 두 번째 Run의 결과가 첫 번째 Run과 독립적인지 검증한다.
7. Stage Mode Goal Clear가 기존 Clear Time Result를 유지하는지 검증한다.

### 적극적 정적 검증

1. GameSystem의 Mode 분기가 TimeRecord와 ScoreRecord를 동시에 호출하지 않는지 확인한다.
2. ResultSystem API 호출 실패 경로에서 게임 상태가 불완전하게 남지 않는지 확인한다.
3. TimerSystem의 Stage Mode 기존 사용과 InfiniteMode 불필요 사용 여부를 확인한다.
4. Retry 초기화 순서가 ResultSystem, RuntimeDataSystem과 InfiniteModeSystem에 일관되게 적용되는지 확인한다.
5. StageModeRoot와 InfiniteModeRoot 활성 규칙이 유지되는지 확인한다.

### 완료 조건

- [ ] InfiniteMode 종료 결과가 정확히 한 번 생성된다.
- [ ] Retry에서 현재 진행과 최종 결과가 초기화된다.
- [ ] Stage Mode TimeRecord와 InfiniteMode ScoreRecord가 충돌하지 않는다.

### 수동 작업

없음. Mode별 종료와 Retry는 Play Mode Test로 처리한다.

## Step 9. 전체 정적 검증과 자동 회귀 Test를 수행한다

- 진행 상태: **대기**

### 정적 검증 체크리스트

- [ ] 이동 거리 정의와 Score 환산 공식의 소유자가 각각 한 곳이다.
- [ ] Pattern 통과 횟수가 거리나 Score 계산에 사용되지 않는다.
- [ ] Runtime Data에는 System 간 공유 값만 존재한다.
- [ ] ScoreRecord는 InfiniteMode에서만 사용된다.
- [ ] TimeRecord는 Stage Mode Clear에서만 사용된다.
- [ ] 하나의 종료에서 두 Record가 동시에 실행되지 않는다.
- [ ] Retry 초기화가 거리, Score, 확정 상태와 Result Data를 모두 제거한다.
- [ ] SaveSystem, Leaderboard와 Phase 3·4 기능이 추가되지 않았다.
- [ ] 정상 프레임 반복 로그가 추가되지 않았다.
- [ ] 신규 Script와 Test `.meta`가 존재하고 GUID가 중복되지 않는다.
- [ ] Scene의 필수 Serialized Field와 fileID가 유효하다.
- [ ] Step 1에서 확정한 Phase 2 표시 범위만 반영되었다.
- [ ] StageModeRoot와 InfiniteModeRoot 활성 규칙이 유지된다.
- [ ] Phase 4 UI 오브젝트가 선행 추가되지 않았다.
- [ ] 기존 Test가 Ignore되거나 기대값이 약화되지 않았다.
- [ ] 변경 파일이 Phase 2 범위에 한정된다.

### 자동 Test 체크리스트

#### Edit Mode

- [ ] 이동 거리 초기화, 진행, 경계, 종료와 Reset
- [ ] 후진, 정지와 공중 이동의 확정 규칙
- [ ] Score 환산, 반올림, 잘못된 값과 큰 값
- [ ] Runtime Data 생성, 갱신, 확정, Clear와 재생성
- [ ] ScoreRecord 정상 기록과 중복 방지
- [ ] ScoreRecord와 TimeRecord Mode 분리
- [ ] ResultData Mode별 계약
- [ ] 기존 Edit Mode 전체 회귀

#### Play Mode

- [ ] 실제 Player 위치와 거리·Score 갱신 연결
- [ ] Pattern 반복과 독립적인 거리 기록
- [ ] 큰 World X에서 연속 기록
- [ ] 저속 및 Y 추락 종료의 최종 결과 생성
- [ ] 종료 1회 보장
- [ ] Retry 후 진행 및 결과 초기화
- [ ] 같은 실행 세션의 두 Run 독립성
- [ ] Stage Mode Goal Clear와 TimeRecord 회귀
- [ ] 기존 Play Mode 전체 회귀

### 완료 조건

- [ ] 정적 검증 체크리스트가 모두 통과한다.
- [ ] 신규 Unit Test와 Play Mode Test가 모두 통과한다.
- [ ] 기존 기준 Edit Mode 68개와 Play Mode 60개를 포함한 전체 회귀 Test가 통과한다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

### 수동 작업

Unity Script Compilation과 Unity Test Runner 전체 실행은 사용자가 Unity Editor에서 수행한다. AI는 정적 검증 결과와 예상 Test 수를 제공한다.

## Step 10. 최소 수동 플레이로 Phase 2 결과를 확인한다

- 진행 상태: **대기**

### 수행 절차

1. InfiniteMode로 Play Mode를 시작한다.
2. 시작 직후 거리와 Score가 초기값인지 확인한다.
3. 한 방향으로 이동하여 현재 거리와 Score가 증가하는지 확인한다.
4. Pattern 사이를 Jump로 통과하고 Pattern을 최소 3회 재배치한다.
5. Pattern 재배치 순간 거리나 Score가 급증하거나 초기화되지 않는지 확인한다.
6. Step 1에서 확정한 후진 규칙을 실제 이동으로 확인한다.
7. 저속 종료 또는 Y 추락 종료를 발생시킨다.
8. 확정된 최종 거리와 Score가 Run 종료 직전 값과 일치하는지 확인한다.
9. Retry 후 현재 거리, Score와 이전 Result Data가 초기화되는지 확인한다.
10. 두 번째 Run에서 첫 번째 Run과 다른 거리로 종료하여 결과가 독립적인지 확인한다.
11. Stage Mode로 전환하여 Goal Clear Time과 Retry가 유지되는지 확인한다.
12. 전체 과정에서 예상하지 않은 Error와 Warning이 없는지 확인한다.

### 정적 검증으로 대체하는 항목

- Component 개수와 Inspector fileID 확인
- 설정 값 중복 여부
- ResultData Property와 생성자 사용처 확인
- Pattern AdvanceCount 비사용 확인
- 신규 Script와 `.meta` 확인
- Test Ignore 및 삭제 여부 확인

### 완료 조건

- [ ] 실제 Run의 거리와 Score 증가가 확정 규칙과 일치한다.
- [ ] Pattern 재배치가 거리와 Score에 잘못 반영되지 않는다.
- [ ] 종료 결과와 Retry 초기화가 정상이다.
- [ ] Stage Mode TimeRecord 회귀가 없다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

### 수동 작업

실제 숫자 변화, 플레이 감각과 화면에 표시하도록 확정된 정보만 Unity Editor에서 확인한다. 자동 Test와 정적 검증으로 판정한 구조 항목은 반복 확인하지 않는다.

## Step 11. Phase 2 완료 근거를 정리한다

- 진행 상태: **대기**

### 수행 절차

1. 변경한 Scene과 Asset이 있다면 저장한다.
2. Scene을 수정했다면 닫았다가 다시 열어 참조 유지 여부를 확인한다.
3. 최종 정적 검증 결과를 기록한다.
4. Unity Script Compilation 결과를 기록한다.
5. Edit Mode와 Play Mode 전체 Test 수와 결과를 기록한다.
6. Step 10 수동 검증 결과와 미해결 사항을 기록한다.
7. 별도 Phase 2 Verification Result Task 문서를 작성한다.
8. 관련 Feature와 System 문서가 최종 구현과 일치하는지 대조한다.
9. 모든 완료 조건을 충족한 경우에만 Roadmap Phase 2를 완료로 변경한다.
10. 미완료 항목이 있으면 Roadmap을 완료로 표시하지 않고 근거와 후속 작업을 기록한다.

### 완료 조건

- [ ] 정적 검증, Compile과 전체 Test가 통과한다.
- [ ] 수동 플레이 결과가 기록되어 있다.
- [ ] Phase 2 범위 밖 기능이 포함되지 않았다.
- [ ] Roadmap 상태와 실제 완료 상태가 일치한다.

### 수동 작업

Scene을 실제로 변경한 경우의 저장·재개방, Unity Compile/Test 결과 확인과 최종 플레이 확인만 필요하다.

---

# 영향 범위

## 문서

- Prototype 2 Phase 2 수동 작업 순서 문서

## 예상 구현 영향

- Core Runtime Data와 Result Data
- InfiniteMode 및 ScoreRecord Feature
- InfiniteModeSystem, ResultSystem과 GameSystem
- Edit Mode 및 Play Mode Test
- Step 1 결정에 따라 제한적인 Scene 또는 UI 연결

---

# 검증 내용

- AI 문서 진입 순서와 Task 문서 템플릿을 확인했다.
- Prototype 2 Roadmap의 Phase 2 목표, 구현 대상과 완료 조건을 확인했다.
- Phase 1 완료 상태와 최종 Test 기준을 확인했다.
- 현재 Runtime Data, InfiniteMode 종료, TimeRecord, ResultData와 ResultSystem 구조를 조사했다.
- 자동화 가능한 순수 규칙을 구현 전 Unit Test Step으로 배치했다.
- System 연결, Mode 분리와 Retry를 Play Mode Test Step으로 배치했다.
- Scene과 파일 구조 검증을 정적 검증으로 배치했다.
- 사용자 결정이 필요한 규칙과 Unity Editor에서 실제로 확인할 항목을 분리했다.

---

# 검증 결과

- Prototype 2 Phase 2 Manual Steps 문서 작성을 완료했다.
- Phase 2 구현은 아직 수행하지 않았다.
- Step 1의 미확정 규칙 결정이 다음 작업이다.
- Roadmap Phase 2 상태는 `대기`로 유지한다.

---

# 후속 작업

1. Step 1의 이동 거리, Score, Result Data와 UI 범위 규칙을 확정한다.
2. 확정 규칙을 문서에 반영한다.
3. Step 2부터 순서대로 정적 조사, Unit Test 우선 구현과 통합 검증을 수행한다.

---

# 관련 문서

## Project

- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`

## Rules

- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/EVENT_RULE.md`
- `AI/01_Rules/LOGGING_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`

## Systems

- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/UIManagementSystem.md`

## Features

- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/TimeRecord.md`
- `AI/03_Features/ResultMenu.md`

## Roadmap

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_002.md`

## Template

- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_2/20260828_03_Prototype2Roadmap.md`
- `AI/90_Tasks/Prototype_2/20260828_06_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_2/20260901_01_Phase1VerificationResult.md`

---

# 작성 완료 기준

- General Task Template의 필수 섹션을 모두 작성했다.
- 실제 수행 순서를 11개 Step으로 작성했다.
- 사용자 결정, 정적 검증, Unit Test, Play Mode Test와 최소 수동 플레이를 구분했다.
- 순수 계산과 상태 규칙은 생산 코드보다 Unit Test를 먼저 작성하도록 배치했다.
- Scene 및 코드 구조에서 정적으로 판정 가능한 항목을 수동 작업으로 남기지 않았다.
- 확인되지 않은 규칙과 구현 결과를 완료로 기록하지 않았다.
