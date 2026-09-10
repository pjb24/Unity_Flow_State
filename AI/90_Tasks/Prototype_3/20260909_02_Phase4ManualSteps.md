# 작업 정보

## 작업명

Prototype 3 Phase 4 수동 작업 및 검증 계획

## 작업 일자

20260909

## 작업 담당자

AI, 사용자

## 작업 상태

완료

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
- `20260828_04_Prototype3Roadmap.md`는 Prototype 3를 벽 충돌 안정화, 자동 이동, Score Collectible, Mode별 Score 및 UI 통합의 네 Phase로 구분한다. Map Pattern 확장, 난이도 증가와 밸런스 검증은 후속 범위로 분리되어 있다.
- Stage Mode 추락은 Stage 전체 플레이 흐름과 실패 Result 및 Retry에 영향을 주므로 Prototype 3 Phase 4에서 함께 처리할 수 있다.
- Stage 추락은 InfiniteMode 거리 진행 규칙을 사용하지 않는다. Stage Mode에서만 Y 임계값을 평가하고 기존 Stage 종료 이벤트와 Result 생성 흐름을 재사용한다.
- 정확한 Score 값, 합산, 포화, Pause 고정, Result 확정과 Retry 초기화는 정적 검사와 Test로 판정할 수 있다.
- UI 계층, Serialized Field, 텍스트 가독성과 화면 겹침은 생산 Scene 저장 또는 실제 화면 확인이 필요한 경우에만 사용자 작업으로 요청한다.

---

# 확정 구현 규칙

Step 1 조사와 사용자 결정을 기준으로 아래 계약을 Phase 4 구현 규칙으로 사용한다.

## Score와 Result Data

- 기존 Infinite Result의 `FinalScore`는 제거하고 `DistanceScore`, `CollectibleScore`와 `TotalScore`를 독립 필드로 사용한다.
- Infinite Total Score는 `ScoreRecord`가 `Distance Score + Collectible Score`로 계산하고 `int.MaxValue`에서 포화한다.
- `ResultData`의 기존 생성자는 유지하지 않고 Mode별 새 계약을 모두 요구하는 생성자로 교체한다.
- Stage Result는 경과 시간을 `ElapsedTime`으로 보존한다.
- Stage 성공은 경과 시간을 `Clear Time`으로 표시하고 추락 실패는 `Run Time`으로 표시한다.
- Stage 결과 원인은 enum으로 관리하고 이번 범위에서는 `Cleared`와 `Fell`을 구분한다.
- Result 진입 시 확정 값을 Result Data에 한 번 복사하고 이후 Runtime Data 정리의 영향을 받지 않게 한다.
- Retry와 새 Run은 이전 Distance, Collectible 획득 상태와 모든 Score 상태를 유지하지 않는다.

## Stage 추락

- `StageSystem`이 Stage Mode Playing 중 `FixedUpdate`에서 Player Rigidbody의 물리 위치를 평가한다.
- Stage 추락 임계값은 `StageSystem`의 Serialized Field로 관리하고 기본값은 `-3.0`으로 사용한다.
- Player Rigidbody의 Y가 임계값 이하인 `Y <= threshold`에서 추락 실패를 요청한다.
- Goal과 추락이 같은 물리 구간에 발생하면 Goal 성공을 우선하여 하나의 Stage 결과만 확정한다.
- Pause, Result와 Ended에서는 추락을 판정하지 않고 Resume 후 다시 평가한다.
- Stage 추락 Result는 추락 시점의 `ElapsedTime`과 Collectible Score를 확정하고 Retry를 허용한다.

## HUD와 Result 표시

- Stage Mode HUD는 현재 Run의 Collectible Score를 표시한다.
- InfiniteMode HUD는 Distance, Distance Score, Collectible Score와 Total Score를 구분하여 표시한다.
- HUD는 Playing 중 Runtime Data를 화면 프레임마다 확인하고 실제 표시값이 변경된 경우에만 Text를 갱신한다.
- Pause 동안 Distance Score와 Collectible Score는 증가하지 않으며 마지막 HUD 표시값을 유지한다.
- Stage Result는 Result Status, 경과 시간과 Collectible Score를 독립 Text로 표시한다.
- Infinite Result는 Final Distance, Distance Score, Collectible Score와 Total Score를 독립 Text로 표시한다.
- Stage Result의 Result Status는 성공과 실패를 구분하여 표시한다.
- HUD와 Result의 Score 항목명은 `Distance Score`, `Collectible Score`와 `Total Score`로 동일하게 사용한다.
- 미초기화 상태와 잘못된 Score 값은 `--` Placeholder로 표시하며 실제 0점과 구분한다.
- 기존 생산 Scene Text는 의미가 일치하는 항목에 재사용하고 부족한 항목은 독립 TMP Text와 Serialized Reference로 추가한다.

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

- 진행 상태: **완료**

### AI 작업

- `ResultData`, `ResultSystem`, `GameSystem`, `UIManagementSystem`, Formatter와 생산 Scene UI 참조를 조사한다.
- Stage 및 Infinite HUD와 Result Content의 현재 Text 필드, 활성 상태와 갱신 경로를 정리한다.
- 확정 구현 규칙의 Score 합산, 포화, 표시 항목과 Stage 추락 계약이 기존 구조와 충돌하는지 확인한다.
- 기존 Test의 책임과 누락 계약을 표로 정리하고 추가할 Test만 결정한다.
- Prototype 4의 Pattern 및 Difficulty 범위가 섞이지 않았는지 확인한다.

### 조사 결과

| 대상 | 재사용 지점 | 변경 지점 | Test 책임 |
|------|-------------|-----------|-----------|
| Result Data | Mode별 결과 구분과 단일 확정 구조 | Stage 결과 원인과 `ElapsedTime`, Mode별 Score 필드 및 생성자 계약 | Result 필드, 잘못된 값 거부와 Mode별 독립성은 Edit Mode Test |
| ScoreRecord | Infinite Result의 단일 기록과 중복 거부 | Distance Score와 Collectible Score 입력 및 포화 Total Score 계산 | 정상값, 0, 음수와 `int.MaxValue` 경계는 Edit Mode Test |
| GameSystem | Stage 종료 이벤트에서 Result 생성 후 Runtime Data 정리 | Stage 성공과 추락 실패의 확정값 전달 | Result 확정, Runtime 정리 후 보존과 Retry는 Play Mode Test |
| UIManagementSystem | Mode별 HUD, Result Content 활성 상태와 변경값 캐시 | Stage 및 Infinite Score Text, Result Status와 Mode별 표시 갱신 | 문자열은 Edit Mode Test, 상태와 생산 Scene 참조는 Play Mode Test |
| StageSystem | Stage 상태, Pause, Goal과 단일 종료 방지 | Stage Mode 물리 위치 판정, 추락 원인과 Goal 성공 우선 경합 | 임계값과 중복 방지는 Edit Mode Test, 물리·Pause·Retry 흐름은 Play Mode Test |
| 생산 Scene UI | 기존 HUD와 Mode별 Result Content | 부족한 독립 TMP Text와 Serialized Reference | 계층과 참조는 Play Mode Test, 가독성과 겹침은 최종 수동 확인 |

- 기존 Test는 Phase 3의 거리 Score, 단일 Result, HUD 갱신, Mode별 UI 활성 상태와 Retry 초기화 책임을 재사용한다.
- 새 Test는 Mode별 Score 필드, 포화 합산, Stage 성공·추락 결과, Goal 성공 우선 경합과 새 생산 Scene UI 참조만 추가한다.
- Pattern 조합, Difficulty 증가와 Score 밸런스 조정은 Prototype 4 또는 후속 작업 범위로 유지한다.

### 사용자 작업

- 제시된 14개 계약 중 Goal과 추락 경합은 Goal 성공 우선을 선택했다.
- 나머지 13개 계약은 AI 권장안을 선택했다.

### 완료 조건

- [x] Score, Result, UI와 Stage 추락 계약이 확정되었다.
- [x] 기존 구조의 재사용 지점과 변경 지점이 확인되었다.
- [x] 자동 Test와 수동 검증의 책임이 구분되었다.

## Step 2. Total Score 계산과 Result Data 계약을 구현한다

- 진행 상태: **완료 — 사용자 Compile 및 전체 Edit Mode Test 325개 통과 확인**

### AI 작업

- Infinite Total Score의 포화 합산을 생산 계산 경로에 구현한다.
- Stage Result에 Collectible Score를, Infinite Result에 Distance Score, Collectible Score와 Total Score를 독립 필드로 추가한다.
- 기존 생성자를 Mode별 새 계약을 모두 요구하는 생성자로 교체하고 모든 호출부를 갱신한다.
- 정상값, 0, 음수 거부, `int.MaxValue` 경계와 포화 합산을 Edit Mode Test로 검증한다.
- 기존 `ResultDataTests`, Score 계산 Test와 중복되는 Test를 만들지 않는다.

### 사용자 작업

- Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- Unity Test Runner의 Edit Mode에서 `ResultDataTests`, `ScoreRecordTests`, `TimeRecordTests`, `ResultSystemTests`, `GameRuntimeDataTests`와 `ResultTextFormatterTests`를 실행한다.
- 이 Step에는 Scene 변경과 Build가 필요하지 않다.

### 구현 결과

- Stage Result Data에 `StageResultType`, `ElapsedTime`과 `CollectibleScore`를 추가하고 기존 Stage 생성자를 새 계약으로 교체했다.
- Infinite Result Data의 `FinalScore`를 제거하고 `DistanceScore`, `CollectibleScore`와 `TotalScore`를 독립 필드로 추가했다.
- `ScoreRecord`가 음수 Score를 거부하고 Total Score를 `int.MaxValue`에서 포화 합산하도록 구현했다.
- `TimeRecord`가 `Cleared`와 `Fell` 결과, 유효한 경과 시간과 Collectible Score만 한 번 기록하도록 확장했다.
- `ResultSystem`, `GameSystem`, Formatter와 기존 Test 호출부를 새 Result 계약으로 갱신했다.
- 기존 Result 생성자, `FinalScore`, `ClearTime`과 `IsStageCleared` 생산 참조가 남지 않았음을 정적으로 확인했다.
- `git diff --check`를 통과했다.
- Unity Script Compilation과 Unity Test Runner는 실행하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation이 성공했다.
- Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 325개가 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없었다.

### 완료 조건

- [x] Mode별 Result Data가 필요한 Score를 독립적으로 보존한다.
- [x] Total Score가 정확하게 포화 합산된다.
- [x] 관련 Edit Mode Test가 통과한다.

## Step 3. HUD와 Result 표시 변환 규칙을 구현한다

- 진행 상태: **완료 — 사용자 Compile 및 전체 Edit Mode Test 330개 통과 확인**

### AI 작업

- Mode별 Score 표시 문자열과 미초기화 Placeholder 규칙을 Formatter에 구현한다.
- Stage HUD, Stage Result, Infinite HUD와 Infinite Result의 표시 상태를 UI 상태 객체에 반영한다.
- 숫자 서식, Placeholder, Mode별 필드 선택과 잘못된 값 거부를 Edit Mode Test로 검증한다.
- Stage 성공의 Clear Time과 추락 실패의 Run Time을 구분하고 기존 Final Distance 표시 규칙을 유지한다.

### 사용자 작업

- Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- Unity Test Runner의 Edit Mode에서 `ResultTextFormatterTests`를 실행한다.
- 이 Step에는 Scene 변경과 Build가 필요하지 않다.

### 구현 결과

- Stage 성공 Status와 Clear Time, 추락 실패 Status와 Run Time 표시 규칙을 구현했다.
- Distance Score, Collectible Score와 Total Score의 공통 HUD 및 Result 표시 문자열을 구현했다.
- 현재 거리와 Final Distance의 기존 내림 표시 규칙을 유지했다.
- 미초기화 또는 잘못된 시간, 거리와 Score 값에 Mode별 `--` Placeholder를 적용했다.
- Stage Result와 Infinite Result가 자신의 필드만 선택하여 출력하고 다른 Mode 데이터는 거부하도록 구현했다.
- `UIManagementSystem`에 Stage 및 InfiniteMode의 독립 HUD와 Result Text 참조 및 초기 표시 상태를 반영했다.
- 기존 Result 및 Infinite HUD 통합 Test 구성을 새 Formatter 계약에 맞게 갱신했다.
- 생산 Scene은 변경하지 않았으며 새 Serialized Reference 연결은 Step 6의 정적 판정 후 수행한다.
- 제거한 `FormatCurrentScore`와 `FormatFinalScore` 호출이 남지 않았음을 정적으로 확인했다.
- `git diff --check`를 통과했다.
- Unity Script Compilation과 Unity Test Runner는 실행하지 않았다.

### 사용자 검증 결과

- Unity Script Compilation이 성공했다.
- Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 330개가 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없었다.

### 완료 조건

- [x] Mode별 표시 문자열과 Placeholder 계약이 구현되었다.
- [x] 기존 시간 및 거리 표시 계약이 유지된다.
- [x] 관련 Edit Mode Test가 통과한다.

## Step 4. Runtime Score를 HUD와 Result 생명주기에 연결한다

- 진행 상태: **완료 — Play Mode Test 실패 3개는 Step 6 이후 처리**

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

- [x] HUD가 현재 Run의 Mode별 Score만 표시한다.
- [x] Result가 종료 시점 값을 보존한다.
- [x] Pause와 Retry 생명주기가 유지된다.
- [ ] 관련 Play Mode Test가 통과한다. (실패 3개를 Step 6 이후 처리)

### 구현 결과

- `UIManagementSystem`이 Playing 중 현재 Mode의 Runtime Data만 읽어 Stage Collectible Score 또는 Infinite Distance, Distance Score, Collectible Score와 Total Score를 갱신하도록 연결했다.
- Pause, Ending과 Ended에서는 HUD 갱신을 중단하여 마지막 표시 값을 유지하고, 새 Run 초기화 시 모든 HUD 표시 캐시와 Placeholder를 초기화하도록 했다.
- Infinite HUD의 Total Score와 Result의 Total Score가 `ScoreRecord.TryCalculateTotalScore`의 동일한 포화 합산 규칙을 사용하도록 했다.
- 기존 GameSystem 종료 흐름이 Runtime Data 정리 전에 Stage 또는 Infinite 결과 값을 ResultData에 복사하며, UI에는 ResultData의 복사본을 표시하는 것을 정적으로 확인했다.
- `InfiniteHudIntegrationTests`에 Stage/Infinite Mode 분리, Collectible/Total Score 갱신, Pause 고정과 Retry 초기화 검증을 추가했다.
- 생산 Scene과 Prefab은 변경하지 않았다. Serialized UI 참조의 생산 Scene 반영 여부는 Step 6에서 정적으로 판정한다.

### 사용자 검증 결과

- Unity Script Compilation이 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 334개가 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test 181개 중 178개가 성공하고 3개가 실패했다.
- 실패한 3개 Test는 Scene 수정을 수행하는 Step 6 이후 원인 확인과 수정을 진행한다.

## Step 5. Stage Mode 추락 실패 처리를 구현한다

- 진행 상태: **구현 및 정적 검증 완료 — Play Mode 실패 5개는 Step 6 이후 처리**

### AI 작업

- 생산 Stage 지형의 최저 Y와 Player 물리 위치 기준을 정적으로 조사하여 Stage 추락 임계값을 확정한다.
- Stage Mode Playing 중에만 Player Rigidbody의 Y 임계값을 평가한다.
- 임계값 이하에서 Stage를 미클리어 상태로 한 번만 종료하고 기존 GameSystem Result 흐름을 사용한다.
- Pause, Result와 Ended에서는 추락 판정을 수행하지 않고 Resume 후 다시 평가한다.
- Goal과 추락 요청이 같은 물리 구간에 발생하면 Goal 성공을 우선하고 하나의 Stage 결과만 확정되도록 한다.
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

### 구현 결과

- 생산 Scene YAML에서 Stage Ground 중심 Y가 `0`, BoxCollider 높이가 `1`, Player Rigidbody 시작 Y가 `1.5`인 것을 확인했다.
- 바닥 하단 Y `-0.5`보다 2.5 unit 낮고 기존 InfiniteMode와 동일한 `-3.0`을 Stage 추락 임계값으로 확정했다.
- `StageSystem`이 Stage Mode Playing 중 Player Rigidbody의 물리 위치 Y를 `<= -3.0` 경계로 판정하도록 구현했다.
- 추락 요청을 다음 FixedUpdate에서 확정하여 같은 물리 구간에 Goal이 도달하면 Goal 성공이 먼저 Stage를 종료하도록 구현했다.
- Pause 중에는 판정을 중단하고 Resume 후 재평가하며, Retry 초기화 시 보류 중인 추락 상태를 제거하도록 했다.
- `GameSystem`이 미클리어 Stage 종료를 `E_StageResultType.Fell`로 ResultData에 복사하도록 연결했다.
- 임계값 직전/동일/직후, Pause, Goal 경합, InfiniteMode 제외, 중복 종료와 Retry를 `StageSystemTests`에 추가했다.
- 생산 Scene의 `StageSystem` 직렬화 데이터에는 새 `_fallThresholdY` 필드가 아직 없으므로 사용자가 Inspector에서 `-3`을 저장해야 한다.

### Play Mode Test 실패 분류

- `CollectibleLifecycleIntegrationTests.ModeSwitch_CreatesEmptyIndependentInfiniteRun`은 새 Infinite Run의 Collectible Score가 `0`이어야 하나 `10`으로 확인된 Runtime 생명주기 Assertion 실패다. Scene Result 참조 실패와 원인이 다르며, 사용자 결정에 따라 Step 6 이후 별도로 처리한다.
- `GamePauseOrchestrationTests.PausedState_StageEndRequest_PrioritizesSingleEndFlow`는 Stage Result Text Serialized Reference 누락으로 실패하므로 Step 6 Scene UI 연결 이후 처리한다.
- `StageGoalIntegrationTests` 2개는 Stage Result Text Serialized Reference 누락으로 실패하므로 Step 6 Scene UI 연결 이후 처리한다.
- `WallLandingRecoveryIntegrationTests.InfiniteMode_WallContact_PreservesMaximumDistanceAndFallEnd`는 Infinite Result Text Serialized Reference 누락으로 실패하므로 Step 6 Scene UI 연결 이후 처리한다.
- 네 실패 모두 추락 임계값, Pause, Goal 우선순위 또는 Wall Landing 로직의 Assertion 실패가 아니라 `UIManagementSystem.SetResultData`의 생산 Scene 참조 검증 Error로 확인했다.
- `StageGoalIntegrationTests`에 남아 있던 이전 `ResultData.ClearTime` 참조는 현재 계약인 `ResultData.ElapsedTime`으로 수정했다.

### 사용자 검증 결과

- Unity Script Compilation이 성공했다.
- Unity Script Compilation에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 334개가 모두 성공했다.
- Edit Mode Test에서 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test 191개 중 186개가 성공하고 5개가 실패했다.
- 실패한 5개 Test는 사용자 결정에 따라 Step 6 이후 처리한다.

## Step 6. 생산 Scene UI 변경 필요성을 정적으로 판정한다

- 진행 상태: **완료 — Scene 참조, 배치 및 Unity 검증 완료**

### AI 작업

- 생산 Scene YAML에서 StageHUD, InfiniteHUD, StageResultContent와 InfiniteResultContent의 계층 및 TMP Text 참조를 검사한다.
- 기존 Text 재사용 가능 여부와 새 Text가 필요한 위치를 판정한다.
- UI Script의 Serialized Field와 Scene 참조를 대조하여 필요한 Scene 작업만 작성한다.
- 누락 참조, 중복 EventSystem, Canvas 정렬, Mode별 활성 상태와 기존 버튼 입력 경로를 정적으로 검사한다.

### 사용자 작업

- 정적 검사에서 새 UI Object 또는 Serialized Reference가 필요하다고 확인된 경우에만 AI가 제공하는 Hierarchy, Component, Anchor, Text와 Field 절차에 따라 Scene을 수정하고 저장한다.
- Scene 저장 후 Unity Script Compilation 성공과 Missing Reference 부재를 확인한다.

### 완료 조건

- [x] 생산 Scene UI 변경 필요성이 정적으로 판정되었다.
- [x] 필요한 최소 Scene 작업만 완료되었다.
- [x] 모든 UI Serialized Reference가 유효하다.

### 정적 판정 결과

- `UIRoot` 아래에 `StageHUD`, `InfiniteHUD`, `ResultPanel`, `PausePanel`이 각각 하나씩 존재하고 각 Panel의 Canvas, CanvasScaler와 GraphicRaycaster 구성이 유지되어 있다.
- EventSystem은 생산 Scene에 하나만 존재한다.
- 기존 `StageHUD/Canvas/Image/Text (TMP)`는 Stage Collectible Score Text로 재사용할 수 있다.
- 기존 `InfiniteHUD`의 `DistanceText`와 `ScoreText`는 현재 거리와 Distance Score Text로 재사용할 수 있다.
- 기존 `StageResultContent`의 `ClearTimeText`는 Clear Time 또는 Run Time Text로 재사용할 수 있다.
- 기존 `InfiniteResultContent`의 `FinalDistanceText`와 `FinalScoreText`는 Final Distance와 Distance Score Text로 재사용할 수 있다.
- Stage HUD에는 새 Text가 필요하지 않고, Infinite HUD 2개, Stage Result 2개, Infinite Result 2개로 총 6개의 TMP Text Object가 새로 필요하다.
- `UIManagementSystem`에는 `_stageCollectibleScoreText`, `_resultStatusText`, `_stageResultCollectibleScoreText`, `_infiniteCollectibleScoreText`, `_infiniteTotalScoreText`, `_infiniteResultCollectibleScoreText`, `_infiniteResultTotalScoreText`의 7개 참조가 누락되어 있다.
- 현재 UIRoot 자식 순서는 InfiniteHUD, ResultPanel, StageHUD, PausePanel이다. 같은 Sorting Order에서 ResultPanel이 HUD보다 앞에 표시되도록 StageHUD, InfiniteHUD, ResultPanel, PausePanel 순서로 정리해야 한다.
- 활성 상태는 GameSystem 시작 시 UIVisibilityState가 Mode와 Game State에 맞게 다시 적용하므로 Scene의 현재 활성값을 별도로 수정할 필요가 없다.
- 생산 Scene의 StageSystem에는 `_fallThresholdY`가 직렬화되어 있지 않으므로 `-3`을 설정하고 저장해야 한다.
- 기존 버튼, Pause 입력 경로와 단일 EventSystem 구성에는 변경이 필요하지 않다.

### Scene 작업 후 정적 검증 결과

- 새 TMP Text Object 6개와 기존 재사용 Text가 계획한 부모 계층에 존재한다.
- `UIManagementSystem`의 새 Serialized Reference 7개가 모두 유효한 TMP Text fileID로 저장되었다.
- UIRoot 자식 순서가 StageHUD, InfiniteHUD, ResultPanel, PausePanel로 저장되었다.
- EventSystem은 하나만 존재하고 StageSystem의 `_fallThresholdY`는 `-3`으로 저장되었다.
- HUD와 Result의 여러 Text RectTransform 수치가 같지만, Unity 화면에서 Text가 시각적으로 겹치지 않는 것을 사용자가 확인하여 추가 배치 보정은 수행하지 않는다.

### 사용자 검증 결과

- Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 334개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test 191개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Unity 화면에서 UI가 시각적으로 겹치지 않는 것을 확인했다.

## Step 7. 생산 Scene의 Mode별 UI 연결을 자동 검증한다

- 진행 상태: **완료 — 사용자 Compile, 전체 Edit Mode Test 334개 및 Play Mode Test 192개 통과 확인**

### AI 작업

- 생산 Scene에서 Stage 및 Infinite HUD와 Result Text 참조 및 초기 활성 상태를 검증하는 Play Mode Test를 보강한다.
- Stage Playing, Stage Result, Infinite Playing, Infinite Result와 Pause 상태의 표시 조합을 검증한다.
- Keyboard와 Mouse의 기존 Pause 및 Result UI 입력 경로가 새 Text 추가의 영향을 받지 않는지 검증한다.
- UI Test가 화면 문구를 생산 Formatter 계약과 별도로 복제하지 않게 한다.

### 구현 결과

- `ModeUISceneConfigurationTests`가 생산 Scene의 Stage 및 Infinite HUD, Result Content와 Score Text Serialized Reference를 검증하도록 보강했다.
- Stage와 Infinite 각각의 Playing, Pause, Result 표시 조합과 Result Content 전환을 검증한다.
- Result 문구의 기대값은 `ResultTextFormatter`를 사용해 계산하여 생산 Formatter 계약을 중복 정의하지 않는다.
- 기존 `PauseMenuIntegrationTests`와 `ResultMenuIntegrationTests`를 재사용하여 Keyboard와 Mouse 입력 회귀를 검증한다.
- Scene 변경 없이 Test 코드만 수정했으며 정적 검증을 완료했다.

### 사용자 작업

- AI가 지정한 Play Mode Test를 실행하고 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 검증 결과

- Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 334개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test 192개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.

### 완료 조건

- [x] 생산 Scene의 Mode별 UI 참조와 상태가 자동 검증된다.
- [x] 기존 Keyboard와 Mouse UI 입력 회귀가 없다.
- [x] 관련 Play Mode Test가 통과한다.

## Step 8. Score 생명주기와 전체 플레이 회귀를 검증한다

- 진행 상태: **완료 — 사용자 Compile, 전체 Edit Mode Test 340개 및 Play Mode Test 192개 통과 확인**

### AI 작업

- 획득 없음, 일부 및 전체 획득의 Stage와 Infinite Result를 검증한다.
- Pause 중 Distance 및 Collectible Score 고정, Result 확정, Retry 초기화와 연속 Run 독립성을 검증한다.
- Jump, Momentum Landing, 자동 이동, Wall 낙하, Goal, Pattern 재사용, Camera, Pause와 Result 입력 회귀 Test를 검토한다.
- 기존 Test로 보장되는 계약은 재사용하고 실행 경로가 다른 누락 Test만 추가한다.
- Ignore, Explicit, 임의 성공, 조건부 제외, 약화된 기대값과 불필요한 중복 Test를 검사한다.

### 구현 결과

- `ResultSystemTests`에 Stage Collectible Score 0, 30, 100점과 Infinite Collectible Score 0, 30, 200점 결과 확정 사례를 추가했다.
- Infinite 결과는 각 Collectible Score가 보존되고 Total Score가 Distance Score와 정확히 합산되는지도 함께 검증한다.
- `CollectibleLifecycleIntegrationTests`, `InfiniteHudIntegrationTests`, `GamePauseOrchestrationTests`, `AutoMovementIntegrationTests`와 기존 Result Test가 Pause 고정, Result 확정, Retry 초기화 및 연속 Run 독립성을 이미 검증함을 확인했다.
- Jump, Momentum Landing, 자동 이동, Wall 낙하, Goal, Pattern 재사용, Camera와 Pause 및 Result 입력은 기존 통합 Test를 재사용한다.
- 전체 Test에서 `Ignore`, `Explicit`과 `Assert.Pass`가 없음을 확인했다. 두 `yield break`는 성공 조건 도달 시 종료하고 제한 시간 초과 시 실패하는 대기 도우미에만 존재한다.
- Scene과 Runtime 코드는 변경하지 않았으며 Test 코드 및 문서를 정적으로 검증했다.

### 사용자 작업

- AI가 지정한 관련 Edit Mode 및 Play Mode Test를 실행한다.

### 검증 결과

- Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 340개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test 192개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.

### 완료 조건

- [x] Mode별 Score와 Result 생명주기가 자동 검증된다.
- [x] Stage 추락과 기존 기능의 회귀가 없다.
- [x] 관련 Edit Mode 및 Play Mode Test가 통과한다.

## Step 9. 전체 정적 검사와 전체 Test를 수행한다

- 진행 상태: **완료 — 전체 정적 검사와 사용자 Compile 및 전체 Test 검증 완료**

### AI 작업

- Runtime, Test, Scene과 대응 `.meta` 및 GUID를 검사한다.
- UI 및 Stage 추락 Serialized Reference, Layer, 활성 상태와 Mode별 계층을 검사한다.
- Update, FixedUpdate와 Trigger 반복 경로의 LINQ, 매 Frame 컬렉션 생성과 정상 흐름 Log를 검사한다.
- 관련 System 및 Feature 문서와 구현 일치를 확인한다.
- Package와 Input Action Asset의 의도하지 않은 변경을 확인한다.
- Prototype 4 Pattern 및 Difficulty 확장, 밸런스 조정과 기타 범위 밖 기능이 포함되지 않았는지 확인한다.
- `git diff --check`를 수행한다.

### 정적 검증 결과

- Runtime과 Test 파일의 대응 `.meta` 누락 및 전체 Asset GUID 중복이 없음을 확인했다.
- 생산 Scene이 참조하는 프로젝트 외부 GUID는 Unity 내장 리소스 또는 설치 Package 리소스로 확인했다.
- 생산 Scene의 UI Serialized Reference 18개가 모두 0이 아닌 유일한 fileID를 가리키고, 단일 EventSystem과 `_fallThresholdY: -3` 저장을 확인했다.
- HUD `Update`, Stage `FixedUpdate`와 Collectible Trigger 경로에 새 LINQ, 매 Frame 컬렉션 생성 및 정상 흐름 Log가 없음을 확인했다.
- 관련 System 및 Feature 문서가 Mode별 Result Data, Score 합산, HUD와 Result 표시 및 Stage 추락 계약과 일치함을 확인했다.
- Package와 Input Action Asset 변경이 없으며 Prototype 4 Pattern 및 Difficulty 확장과 Score 밸런스 변경이 포함되지 않았음을 확인했다.
- 전체 `git diff --check`에서 생산 Scene의 빈 `m_Name:` 7개에 Unity YAML 끝 공백이 발견되었다. Scene 파일을 제외한 변경 파일은 통과했다.
- 사용자는 빈 `m_Name:` 끝 공백이 게임 진행에 영향을 주지 않으므로 유지하기로 결정했다. 재검사 결과 전체 `git diff --check`의 지적은 이 7개뿐이며 기능 및 Serialized Reference 문제는 없다.

### 사용자 작업

- Unity Script Compilation 성공과 예상하지 않은 Error 및 Warning 부재를 확인한다.
- 전체 Edit Mode Test를 한 번 실행하고 모두 성공하는지 확인한다.
- 전체 Play Mode Test를 한 번 실행하고 모두 성공하는지 확인한다.
- Test 관련 예상하지 않은 Error 및 Warning 부재를 확인한다.

### 검증 결과

- Unity Script Compilation이 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Edit Mode Test 340개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 Play Mode Test 192개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 생산 Scene의 빈 `m_Name:` 끝 공백 7개는 게임 진행에 영향이 없는 Unity YAML 형식이며 사용자 결정에 따라 유지한다.

### 완료 조건

- [x] 전체 정적 검증이 통과한다.
- [x] 전체 Edit Mode 및 Play Mode Test가 통과한다.
- [x] 예상하지 않은 Compile 및 Test Error와 Warning이 없다.
- [x] Phase 4 범위 밖 기능이 포함되지 않았다.

## Step 10. 최소 화면을 검증하고 완료 근거를 정리한다

- 진행 상태: **완료 — 최소 화면 검증 및 Phase 4 완료 근거 기록 완료**

### AI 작업

- 정확한 Score 값, 합산, Pause, Result와 Retry는 자동 Test 결과를 사용하고 수동 체크리스트에서 제외한다.
- 생산 Scene, Asset 참조와 전체 검증 결과를 확인한다.

### 사용자 작업

1. Unity Editor Play Mode에서 Stage HUD의 Collectible Score가 읽기 쉽고 기존 플레이 화면을 가리지 않는지 확인한다.
2. Stage Clear Result의 Clear Time, 추락 실패 Result의 Run Time과 Collectible Score가 구분되는지 확인한다.
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

### 준비 결과

- Step 9의 Unity Script Compilation과 전체 Edit Mode Test 340개 및 Play Mode Test 192개 성공 결과를 최종 자동 검증 근거로 재사용한다.
- 생산 Scene의 Mode별 UI Serialized Reference와 Stage 추락 임계값이 유지됨을 정적으로 재확인했다.
- `20260910_01_Phase4VerificationResult.md`를 작성하고 최소 화면 검증 전 확인된 결과와 남은 항목을 기록했다.
- 정확한 Score, 합산, Pause 고정, Result 확정, Retry 초기화와 빠른 입력 경계는 수동 확인 항목에서 제외했다.

### 최소 화면 검증 결과

- 사용자가 Stage 및 InfiniteMode의 플레이와 Mode별 HUD 및 Result 표시가 정상임을 확인했다.
- Stage 성공과 추락 실패 Result, Retry 및 Pause와 Result 조작 흐름이 정상임을 확인했다.
- 반복 플레이 중 Text 가독성, 겹침, 잘림, 떨림과 순간적인 잘못된 값 표시 문제가 없음을 확인했다.
- Play Mode Console에 예상하지 않은 Error와 Warning이 없음을 확인했다.

### 완료 조건

- [x] 최소 화면 검증 결과가 기록되어 있다.
- [x] Mode별 HUD와 Result의 가독성이 확인되었다.
- [x] Stage 추락 실패 및 Retry 화면 흐름이 확인되었다.
- [x] Roadmap 상태와 실제 완료 상태가 일치한다.

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
- `20260828_04_Prototype3Roadmap.md`를 참조하여 Phase 4의 Mode별 Score 및 UI 통합 범위와 후속 Pattern, Difficulty 및 밸런스 범위를 구분했다.
- Stage Mode 추락 문제를 Phase 4의 종료, Result 및 Retry 범위에 포함했다.
- 수치, 상태, 경계, 중복 실행과 생명주기는 Edit Mode 및 Play Mode Test가 담당하도록 했다.
- Scene 저장과 최종 화면 가독성 외의 자동 판정 항목을 사용자 작업에서 제외했다.
- 전체 Test는 관련 Test 통과 후 한 번만 실행하도록 배치했다.

---

# 검증 결과

- Phase 4 수행 순서와 사용자 수동 작업 범위가 정의되었다.
- Score 및 Result 계약, UI 연결과 Stage 추락 처리의 자동 검증 책임이 확정되었다.
- Goal과 추락이 같은 물리 구간에 발생하면 Goal 성공을 우선하는 계약을 확정했다.
- 나머지 13개 계약은 Step 1에서 제시한 권장안으로 확정했다.
- Stage 추락 처리를 Phase 4에 포함할 수 있음을 확인했다.
- Step 2와 Step 3 구현 및 Unity 검증을 완료했다.
- Step 4 Runtime Score의 HUD 및 Result 생명주기 연결과 정적 검증을 완료했다.
- Step 4 Unity Script Compilation과 전체 Edit Mode Test 검증을 완료했다.
- Step 4 검증 당시 전체 Play Mode Test 181개 중 178개가 성공했으며, 실패 3개는 Step 6 이후 처리하기로 했다.
- Step 5 Stage Mode 추락 실패 처리 구현과 정적 검증을 완료했다.
- Step 5 이후 Unity Script Compilation과 전체 Edit Mode Test 334개가 성공했다.
- 전체 Play Mode Test 191개 중 186개가 성공했으며, 실패 5개는 Step 6 이후 처리하기로 했다.
- 생산 Scene의 Stage 추락 임계값 저장과 Result TMP 참조 연결은 Step 6 Scene 작업에 함께 반영한다.
- Step 6 Scene 작업 후 전체 Edit Mode Test 334개와 Play Mode Test 191개가 모두 성공하여 보류한 5개 실패가 해소되었다.
- Step 6의 Serialized Reference, UIRoot 순서, 단일 EventSystem과 Stage 추락 임계값 정적 검증을 완료했고, 사용자가 UI가 시각적으로 겹치지 않는 것을 확인했다.
- Step 7 생산 Scene Test에 Mode별 HUD, Pause, Result 표시와 모든 신규 Text 참조 및 Formatter 결과 연결 검증을 추가했다.
- Keyboard와 Mouse Pause 및 Result 입력은 기존 통합 Test를 재사용해 검증했다.
- Step 7 이후 Unity Script Compilation, 전체 Edit Mode Test 334개와 Play Mode Test 192개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Step 8에서 Stage와 Infinite의 무획득, 일부 획득 및 전체 획득 Result Score 사례 6개를 추가하고 기존 전체 플레이 회귀 Test의 책임을 대조했다.
- Step 8 이후 Unity Script Compilation, 전체 Edit Mode Test 340개와 Play Mode Test 192개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- Step 9의 `.meta`, GUID, Serialized Reference, Layer 및 계층, 반복 실행 경로, 문서 일치, Package와 Input Action 및 범위 검사를 완료했다.
- Step 9 이후 Unity Script Compilation, 전체 Edit Mode Test 340개와 Play Mode Test 192개가 모두 성공했고 예상하지 않은 Error와 Warning이 없었다.
- 전체 `git diff --check`에 남은 생산 Scene의 빈 `m_Name:` 끝 공백 7개는 기능에 영향이 없어 사용자 결정에 따라 유지한다.
- Step 10 최소 화면 검증에서 Stage 및 InfiniteMode 진행, HUD와 Result 표시, Stage 추락 실패, Retry와 UI 조작 흐름이 정상임을 확인했다.
- Phase 4의 정적 검증, Compile, 전체 Test와 최소 화면 검증을 모두 완료했다.

---

# 후속 작업

Prototype 3 Phase 4를 완료했다.

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
