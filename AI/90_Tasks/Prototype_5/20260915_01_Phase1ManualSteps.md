# 작업 정보

## 작업명

Prototype 5 Phase 1 수동 작업 및 검증 계획

## 작업 일자

20260915

## 작업 담당자

AI, 사용자

## 작업 상태

완료 — Step 7 Unity Script Compilation 및 전체 Edit Mode Test 검증 통과

---

# 작업 목적

Momentum Landing의 속도 증가 효과를 Score 배율로 전환하기 위한 규칙과 InfiniteMode World Rebase 계약을 구현 전에 확정한다. 자동 판정할 수 있는 계산·경계·상태는 정적 검사와 Edit Mode Unit Test로 검증하고, 사용자는 게임 설계 정책 결정과 Unity Editor의 Compile·Test 실행만 수행한다.

---

# 작업 대상

- Momentum Landing 성공·연속 성공·초기화와 Score 배율 계약
- Base Distance Score, Momentum Bonus, Collectible Score와 Total Score 관계
- World Rebase 임계값, 대상 계층과 누적 논리 거리 계약
- Rebase 전후 Difficulty·Pattern·Collectible·Camera 상태 계약
- Scoring Version과 기존 기록 구분 정책
- 관련 Feature·System·Roadmap 문서와 Edit Mode Unit Test

---

# 작업 전 상태

- Momentum Landing 성공은 현재 Player의 이동 흐름과 수평 속도를 유지하거나 증가시키는 계약이다.
- InfiniteMode의 Distance는 Run 시작 Player World X를 원점으로 하는 최대 전진 거리이며 `float`으로 관리한다.
- Distance Score는 누적 Distance에 단위 점수를 곱해 계산하며 Momentum Landing 보너스를 구분하지 않는다.
- Difficulty는 Distance `220/440` 경계를 사용하고 Pattern 선택·Collectible Scope·Camera는 생산 Scene의 World 좌표를 사용한다.
- Prototype 4의 Compile, 전체 Edit Mode `490`개, Play Mode `219`개, 화면 확인과 사용자 Build가 통과했다. 이 결과는 Prototype 5 변경 전 기준선이다.
- 로컬·서버 기록은 Roadmap 7 범위이지만, 점수 규칙 변경을 구분할 Scoring Version 계약은 이번 Phase에서 먼저 확정해야 한다.

---

# 작업 원칙

- AI는 문서·코드·Test·Scene YAML을 먼저 정적으로 조사하고 현재 속도·Score·거리·좌표 의존성을 표로 정리한다.
- 설계 선택이 필요한 항목은 AI가 추천안과 대안의 장단점을 제시하고 사용자가 확정한다. Inspector 값이나 플레이 감각으로 규칙을 임의 결정하지 않는다.
- 수치 계산, 경계값, 상태 전환, 초기화, 포화와 비정상 입력은 Edit Mode Unit Test를 우선한다.
- Test는 생산 코드를 호출하며 계산식을 Test 안에 다시 구현하지 않는다. 정상값뿐 아니라 경계 직전·경계값·경계 직후, 최소·최대, 중복 요청과 잘못된 입력을 포함한다.
- Phase 1은 계약과 순수 상태·계산 모델까지만 다룬다. 생산 Scene, Rigidbody, Camera, Pattern과 UI 연결은 후속 Phase에서 수행한다.
- 자동 판정 가능한 값은 사용자에게 수동 확인을 요청하지 않는다. 사용자는 정책 결정과 Unity Script Compilation·Unity Test Runner 실행 결과만 제공한다.
- AI는 Unity Editor, Unity Test Runner 또는 Build를 실행하지 않는다. Phase 1에서는 Build와 화면·조작감 검증을 수행하지 않고 미검증으로 기록한다.

---

# 수행 Step

## Step 1. 현재 Momentum Landing·Score·World 좌표 의존성을 조사한다

### AI 작업

- `MomentumLanding.md`, `InfiniteMode.md`, `ScoreRecord.md`, Roadmap 5와 관련 System 문서를 현재 Runtime 코드에 대조한다.
- Momentum Landing 성공이 수평 속도에 반영되는 경로와 Inspector 설정, Runtime Data, 입력·착지 생명주기 및 기존 Test를 정적으로 추적한다.
- Distance Score의 입력·내림·포화·Result 전달 경로와 Collectible Score·Total Score 결합 지점을 조사한다.
- Player, Camera, Infinite Pattern Slot, Boundary, Collectible과 Distance·Difficulty가 World X에 의존하는 지점을 정적 검색으로 목록화한다.
- 기존 Test 중 속도 증가나 절대 World X를 계약으로 기대하여 후속 변경이 필요한 Test를 구분한다.

### 사용자 수동 작업

- 없음. 코드·문서·직렬화 파일로 확인 가능한 사항은 AI가 처리한다.

### 완료 조건

- [x] Momentum Landing에서 Score Result까지의 변경 영향 경로가 확인됐다.
- [x] Rebase 대상과 World 좌표 의존 코드·Test가 확인됐다.
- [x] 사용자 결정이 필요한 항목과 정적으로 확정 가능한 항목이 분리됐다.

---

## Step 2. Momentum Landing Score 정책을 결정한다

### AI 작업

- 아래 각 결정에 대해 권장안, 대안, 장점·단점과 기존 기능에 미치는 영향을 채팅으로 제시한다.
  - 배율 단계, 증가량과 최대 배율
  - 배율 적용 대상과 Score 구성 요소
  - 연속 성공 판정 단위
  - 일반 착지·Wall 접촉·낙하·시간 경과의 초기화 조건
  - Pause·Resume·Result·Retry·새 Run 생명주기
  - HUD와 Result에 표시할 정보
- 사용자 결정을 상호 모순, 계산 가능성, `int.MaxValue` 포화와 Leaderboard 비교 가능성 관점에서 검토한다.
- 결정 전에는 수치나 동작을 코드·Feature 문서에 확정값으로 반영하지 않는다.

### 사용자 수동 작업

- AI가 제시한 항목별 선택지에서 원하는 정책을 결정한다. Unity Editor 작업이나 수동 플레이는 필요 없다.

### 완료 조건

- [x] Momentum Landing이 이동 속도를 증가시키지 않는다고 확정됐다.
- [x] 배율 단계·상한·적용 대상·초기화 조건이 확정됐다.
- [x] Pause·Result·Retry와 HUD·Result 표시 계약이 확정됐다.
- [x] Base Distance Score, Momentum Bonus, Collectible Score와 Total Score 관계가 확정됐다.

---

## Step 3. World Rebase와 누적 논리 거리 정책을 결정한다

### AI 작업

- 아래 각 결정에 대해 권장안, 대안, 장점·단점과 생산 구조에 미치는 영향을 채팅으로 제시한다.
  - Rebase 실행 임계값과 한 번에 이동할 Offset
  - Rebase 기준점과 실행 가능한 게임 상태
  - Player·Camera·Pattern Slot·Boundary·Collectible 중 함께 이동할 대상
  - World Root 도입 여부와 Scene 계층 영향
  - 누적 논리 거리 자료형과 표시 거리 변환
  - Rebase 프레임의 Physics·Camera 처리 계약
- Pattern 길이·Slot 재사용 거리·Difficulty 경계를 정적으로 대조해 임계값 후보가 안전한지 계산한다.
- Collectible 식별과 Scope가 절대 World X에 의존하지 않는지 확인하고, 의존하면 변경 계약에 포함한다.

### 사용자 수동 작업

- AI가 제시한 정책 중 Rebase 구조와 수치 후보를 결정한다. Scene 계층 편집이나 화면 확인은 이 Step에서 수행하지 않는다.

### 완료 조건

- [x] Rebase 임계값·Offset·실행 시점이 확정됐다.
- [x] 함께 이동할 대상과 이동하지 않을 대상이 명확하다.
- [x] 누적 논리 거리와 현재 World X의 관계가 수식으로 정의됐다.
- [x] Distance·Difficulty·Score·Pattern·Collectible·Camera의 Rebase 전후 계약이 확정됐다.

---

## Step 4. Scoring Version과 호환 정책을 확정한다

### AI 작업

- 기존 Distance Score와 새 Momentum Score가 같은 기록으로 비교되지 않도록 Scoring Version 후보를 제시한다.
- Version의 소유 위치, Runtime Data·Result Data 전달, Retry·새 Run 유지와 Roadmap 7 Leaderboard 분리 계약을 정의한다.
- 아직 저장 기능이 없는 현재 범위에서 구현할 최소 계약과 Roadmap 7로 넘길 저장·서버 책임을 구분한다.

### 사용자 수동 작업

- Score 규칙 변경 시 Version을 증가시키고 Leaderboard를 분리하는 정책을 승인하거나 대안을 결정한다. Unity Editor 작업은 필요 없다.

### 완료 조건

- [x] 현재와 새 Score 규칙을 구분할 Scoring Version이 확정됐다.
- [x] Version이 Run과 Result에서 유지되는 범위가 명확하다.
- [x] 저장·서버 연동은 Roadmap 7 책임으로 분리됐다.

---

## Step 5. 확정 계약을 순수 상태·계산 코드와 Unit Test로 고정한다

### AI 작업

- Momentum 배율 상태, 구간별 Score 계산, 논리 거리와 Rebase Offset을 Scene에 의존하지 않는 순수 코드로 작성한다.
- 다음 Edit Mode Unit Test를 작성한다.
  - 새 Run 기본 배율과 연속 Momentum 성공별 단계 전환
  - 최대 배율 고정, 중복 성공 거부와 모든 초기화 조건
  - Base Distance Score·Momentum Bonus·Collectible Score·Total Score 분리 및 포화
  - Pause·Resume·Result 보존과 Retry·새 Run 초기화
  - Rebase 경계 직전·정확한 경계·직후와 여러 번의 Rebase
  - Rebase 전후 누적 논리 거리 단조 증가와 동일 Score·Difficulty 입력
  - 음수, `NaN`, Infinity, Overflow와 잘못된 상태 요청 거부
  - Scoring Version 기본값·전달·불일치 거부
- Test가 생산 계산을 호출하고 기대값만 명시하는지, LINQ·시간·Scene·물리 의존성이 없는지 정적으로 검사한다.
- Phase 2·3의 생산 연결 전에 필요한 API 경계만 작성하며 PlayerMovementSystem·Scene·Prefab·UI는 변경하지 않는다.

### 사용자 수동 작업

- 없음. 코드 작성과 정적 검증은 AI가 수행한다.

### 완료 조건

- [x] 확정된 모든 수치 경계와 상태 규칙에 Unit Test가 존재한다.
- [x] Test가 Scene과 프레임 실행 없이 결정적으로 수행 가능하다.
- [x] 생산 연결을 구현하지 않고도 Phase 1 계약을 코드로 확인할 수 있다.
- [x] 정적 검사와 `git diff --check`가 통과한다.

---

## Step 6. 계약 문서와 Roadmap 상태를 갱신한다

### AI 작업

- `MomentumLanding.md`, `InfiniteMode.md`, `ScoreRecord.md`와 관련 System 문서를 확정된 규칙에 맞춰 갱신한다.
- Roadmap 5 Phase 1의 완료 조건과 실제 구현·검증 범위를 대조한다.
- Phase 2의 속도 효과 제거·Score 연결과 Phase 3의 생산 World Rebase에 필요한 후속 작업을 구분한다.
- 구현하지 않은 Scene·Physics·Camera·UI 동작을 완료된 것으로 기록하지 않는다.

### 사용자 수동 작업

- 없음. 문서와 코드의 대조는 AI가 수행한다.

### 완료 조건

- [x] Feature·System 문서와 순수 상태·계산 코드가 일치한다.
- [x] Roadmap Phase 1과 후속 Phase 경계가 명확하다.
- [x] 확인하지 않은 생산 연동이나 수동 체감을 통과로 기록하지 않는다.

---

## Step 7. Unity Compile과 Unit Test 결과를 확인한다

### AI 작업

- 변경된 책임에 필요한 Edit Mode Test Class와 영향받는 기존 회귀 Test Class를 정확히 지정한다.
- 사용자 결과에서 시도·성공·실패 수, 예상하지 않은 Error·Warning 여부를 기록한다.
- 실패 시 Test 이름·메시지·Stack Trace를 분석하고 수정한 뒤 영향받는 Test 전체를 다시 요청한다.
- 모든 결과가 확인된 뒤에만 Phase 1 완료 여부를 판정하고 검증 결과 Task 문서를 작성한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료를 확인한다.
2. Console에서 예상하지 않은 Error와 Warning이 없는지 확인한다.
3. AI가 지정한 Edit Mode Unit Test Class를 Unity Test Runner에서 실행한다.
4. AI가 회귀 범위로 지정한 기존 Edit Mode Test Class를 실행한다.
5. 각 실행의 Tests Run·Passed·Failed 수와 예상하지 않은 Error·Warning 여부를 전달한다.
6. 실패가 있으면 Test 이름, 실패 메시지와 Stack Trace를 함께 전달한다.

### 지정된 새 Edit Mode Unit Test

- `FlowState.Tests.EditMode.MomentumScoreStateTests` — 예상 `22` cases
- `FlowState.Tests.EditMode.InfiniteScoreStateTests` — 예상 `14` cases
- `FlowState.Tests.EditMode.WorldRebaseStateTests` — 예상 `24` cases
- `FlowState.Tests.EditMode.ScoringVersionTests` — 예상 `9` cases

합계 예상 `69` cases이다.

### 지정된 Edit Mode 회귀 Test

- `FlowState.Tests.EditMode.InfiniteModeRuntimeDataTests` — 예상 `19` cases
- `FlowState.Tests.EditMode.GameRuntimeDataTests` — 예상 `18` cases
- `FlowState.Tests.EditMode.ResultDataTests` — 예상 `2` cases
- `FlowState.Tests.EditMode.ScoreRecordTests` — 예상 `20` cases
- `FlowState.Tests.EditMode.ResultSystemTests` — 예상 `9` cases
- `FlowState.Tests.EditMode.ResultTextFormatterTests` — 예상 `35` cases

합계 예상 `103` cases이다. 예상 수는 정적 Attribute 집계값이며 실제 Unity Test Runner의 Tests Run 수를 최종 근거로 사용한다.

Phase 1에는 생산 Scene·Physics·Camera 연결 변경이 없으므로 Play Mode Test, 화면 확인과 Build는 수행하지 않는다. 정적 검사나 Unit Test로 판정할 항목을 수동 플레이로 확인하지 않는다.

### 완료 조건

- [x] Unity Script Compilation이 성공했다.
- [x] 지정된 새 Unit Test와 영향받는 Edit Mode 회귀 Test가 모두 통과했다.
- [x] 예상하지 않은 Error·Warning이 없다.
- [x] 미검증인 생산 연동·Play Mode·화면·Build 범위가 결과에 명시됐다.

---

# 수동 작업 요약

- Step 2에서 Momentum Landing Score 배율·초기화·표시 정책 결정
- Step 3에서 World Rebase 구조·임계값·Offset 정책 결정
- Step 4에서 Scoring Version과 기록 분리 정책 결정
- Step 7에서 Unity Script Compilation과 지정된 Edit Mode Unit Test 실행 결과 전달

Inspector 값 조사, Scene·Prefab 편집, 수치 계산, World X 이동 반복 플레이, Difficulty·Score 경계 확인과 빠른 입력 재현은 수동 작업에 포함하지 않는다.

---

# 영향 범위

- Project: Score 및 장시간 InfiniteMode 방향
- Feature: `MomentumLanding`, `InfiniteMode`, `ScoreRecord`
- System: `PlayerMovementSystem`, `InfiniteModeSystem`, `ResultSystem`, `CameraSystem`의 후속 연결 계약
- Task: Prototype 5 Phase 1 결정·구현·검증 결과
- Test: 순수 상태·계산 Edit Mode Unit Test와 관련 회귀 Test

---

# 검증 내용

- Step 1–6의 정책 결정, 순수 Runtime 모델·Unit Test 작성과 계약 정합성 점검을 완료했다. Unity 검증은 아직 수행하지 않았다.
- 정적 검사 → Script Compilation → Edit Mode Unit Test 순서로 검증한다.
- Play Mode, 생산 Scene·Physics·Camera, 화면·조작감과 Build는 후속 Phase 책임으로 남긴다.

---

# 검증 결과

- 수동 작업 및 검증 계획 작성 완료.
- Step 1 정적 조사 완료. 조사 결과는 `20260915_02_Phase1Step1Investigation.md`에 기록했다.
- Step 2 Momentum Landing Score 정책 확정 완료. 결정 결과는 `20260915_03_Phase1Step2MomentumScorePolicy.md`에 기록했다.
- Step 3 World Rebase와 누적 논리 거리 정책 확정 완료. 결정 결과는 `20260915_04_Phase1Step3WorldRebasePolicy.md`에 기록했다.
- Step 4 Scoring Version과 호환 정책 확정 완료. 결정 결과는 `20260915_05_Phase1Step4ScoringVersionPolicy.md`에 기록했다.
- Step 5 순수 상태·계산 코드와 Edit Mode Unit Test 작성 완료. 결과는 `20260916_01_Phase1Step5PureModels.md`에 기록했다.
- Step 6 Feature·System 계약과 Roadmap Phase 경계 정합성 점검 완료. 결과는 `20260916_02_Phase1Step6ContractAlignment.md`에 기록했다.
- Step 7 정적 검증과 Test 영향 분석 완료. 새 Unit Test `4`개 Class, 예상 `69` cases와 기존 회귀 Test `6`개 Class, 예상 `103` cases를 지정했다.
- `.asmdef` 참조, 새 Script·Test의 `.meta`, GUID 중복과 변경 파일 공백 오류를 정적으로 확인했다.
- 사용자 확인 결과 Unity Script Compilation이 성공했고 예상하지 않은 Error·Warning이 없었다.
- 사용자가 전체 Edit Mode Test `559`개를 실행했으며 `559`개 모두 성공했고 예상하지 않은 Error·Warning이 없었다. 전체 실행 결과가 지정된 새 Test와 회귀 Test 범위를 포함한다.
- Step 7 및 Phase 1 검증 완료. 결과는 `20260916_03_Phase1VerificationResult.md`에 기록했다.
- 생산 Momentum·Score·UI 연결, World Rebase 연결, Scene·Physics·Camera, Play Mode·화면과 Build는 Phase 1 미검증 범위다.

---

# 후속 작업

- Roadmap 5 Phase 2에서 Momentum Landing의 속도 효과 제거와 Score·Runtime Data·UI 생산 연결을 수행한다.

---

# 관련 문서

- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/02_Systems/PlayerMovementSystem.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_4/20260914_04_Phase4VerificationResult.md`

---

# 작성 완료 기준

- Phase 1을 실제로 수행할 수 있는 순서와 완료 조건을 작성했다.
- AI 정적 작업과 사용자 수동 작업을 Step별로 분리했다.
- 자동화 가능한 검증을 Unit Test로 우선 배치했다.
- 후속 Phase의 Scene·Physics·Camera·UI 작업을 Phase 1 완료로 간주하지 않는다.
