# 작업 정보

## 작업명

Prototype 5 Phase 1 Step 5 순수 상태·계산 모델 구현

## 작업 일자

20260916

## 작업 담당자

AI

## 작업 상태

코드 및 정적 검증 완료 · Unity 검증 대기

---

# 작업 목적

확정된 Momentum Score, 누적 논리 거리, World Rebase와 Scoring Version 계약을 Scene과 물리에 의존하지 않는 생산 코드 및 Edit Mode Unit Test로 고정한다.

---

# 작업 대상

- Momentum 배율 단계, 유지 시간과 생명주기 상태
- Base Distance Score, Momentum Bonus, Distance Score와 Total Score 계산
- 누적 논리 거리와 World Rebase Offset 상태
- Scoring Version 정의, Runtime Data와 Result Data API 경계
- 위 책임의 Edit Mode Unit Test

---

# 작업 전 상태

- Step 2–4에서 정책은 확정됐지만 이를 표현하는 순수 생산 코드가 없었다.
- 기존 `ScoreCalculator`와 `InfiniteDistanceState`는 Version 1의 `float` 거리 점수와 절대 World X 계약을 사용했다.
- Runtime Data와 Result Data에는 Scoring Version, Base Distance Score, Momentum Bonus와 최고 Momentum 배율이 없었다.

---

# 조사 내용

- Runtime Features Assembly는 Runtime Core Assembly를 참조하며 Edit Mode Test Assembly는 두 Assembly를 모두 참조한다.
- 기존 생산 System 연결을 변경하지 않고 새 Version 2 API를 추가할 수 있다.
- 기존 Result와 ScoreRecord API는 Version 1 회귀를 위해 유지할 필요가 있다.

---

# 작업 내용

## 생산 코드

- `ScoringVersion`에 무효값 `0`, 기존 규칙 `1`, 현재 규칙 `2`를 정의했다.
- `MomentumScoreState`에 `1.00x`부터 `3.00x`까지의 단계, `10.0`초부터 `6.5`초까지의 유지 시간, 성공 우선, 중복 요청 거부, Pause·Resume·Finalize·Reset을 구현했다.
- `InfiniteScoreState`에 논리 거리 구간별 Base Distance Score와 정밀 Momentum Bonus 누적, 정수 내림, Collectible 비적용과 `int.MaxValue` 포화를 구현했다.
- `WorldRebaseState`에 임계값 `880`, 여러 배 Offset 계산, `double` 누적 Offset 및 논리 거리, Rebase 적용 분리와 생명주기를 구현했다.
- `InfiniteModeRuntimeData`에 명시적 Scoring Version 초기화 및 불변 API 경계를 추가했다. 기존 생산 진입점은 Phase 2의 원자적 전환 전까지 Version 1을 유지한다.
- `ResultData`에 Scoring Version, Base Distance Score, Momentum Bonus와 최고 Momentum 배율을 추가했다.
- `ScoreRecord`에 Version 2 Score 구성 요소의 일치와 유효성을 검사하는 Overload를 추가했다.
- 기존 생산 System, Scene, Prefab과 UI 연결은 변경하지 않았다.

## Edit Mode Unit Test

- `MomentumScoreStateTests`
  - 모든 배율과 유지 시간 단계
  - 최대 배율 갱신, 중복 성공, 정확한 만료 경계와 성공 우선
  - Pause·Resume, Finalize, Reset, 비정상 시간과 Version 불일치
- `InfiniteScoreStateTests`
  - Base 및 Bonus 분리, 새 거리 증가분 배율 적용
  - 프레임 분할과 무관한 정밀 Bonus, Collectible 비적용
  - 포화, 감소 거리, 비정상 입력, Version 불일치와 Finalize
- `WorldRebaseStateTests`
  - 임계값 직전·정확한 경계·직후와 여러 배 Offset
  - 반복 Rebase의 거리 보존과 단조 증가
  - 동일 Score 및 Difficulty 입력, 비정상 Offset과 Overflow
  - Pause·Resume, Finalize, Reset과 중복 생명주기 요청
- `ScoringVersionTests`
  - Version 기본값과 지원 범위
  - Runtime Data 고정 및 Clear
  - Stage, Legacy와 Version 2 Result 구분
  - Result 전달, 지원하지 않는 Version과 Score 구성 불일치 거부

---

# 영향 범위

- Runtime Core: ScoringVersion, InfiniteModeRuntimeData, ResultData
- Runtime Features: MomentumScoreState, InfiniteScoreState, WorldRebaseState, ScoreRecord
- Edit Mode Tests: 신규 순수 모델 Test 4개
- Tasks: Phase 1 Step 5 상태와 구현 기록

---

# 검증 내용

- 새 C# 파일의 중괄호 수와 파일당 Class 구성을 정적으로 확인했다.
- 새 코드에 LINQ, nullable 참조, null 조건부와 null 병합 문법이 없는지 검색했다.
- 신규 Meta GUID가 기존 GUID와 중복되지 않는지 확인했다.
- Test가 Scene, 프레임, 물리와 Unity 시간 API를 사용하지 않는지 확인했다.
- `git diff --check`를 실행했다.
- Unity Editor, Unity Test Runner와 Build는 실행하지 않았다.

---

# 검증 결과

- 정적 검사와 `git diff --check`가 통과했다.
- 신규 Test는 생산 코드를 직접 호출하며 계산식을 Test에 재구현하지 않는다.
- Unity Script Compilation과 Edit Mode Test 실행 결과는 미검증이다.
- 생산 Scene, Rigidbody, Physics, Camera, Pattern, UI와 Build는 이번 Step 범위가 아니다.

---

# 후속 작업

- Step 6에서 계약 문서와 코드의 일치 및 후속 Phase 경계를 최종 점검한다.
- Step 7에서 사용자가 Unity Script Compilation과 지정된 Edit Mode Test를 실행한다.
- Phase 2와 3에서 생산 System, UI, Physics와 Camera 연결을 구현한다.

---

# 관련 문서

- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260915_03_Phase1Step2MomentumScorePolicy.md`
- `AI/90_Tasks/Prototype_5/20260915_04_Phase1Step3WorldRebasePolicy.md`
- `AI/90_Tasks/Prototype_5/20260915_05_Phase1Step4ScoringVersionPolicy.md`

---

# 작성 완료 기준

- 확정된 Phase 1 계약을 순수 생산 코드로 표현했다.
- 정상, 경계, 중복, 비정상, 포화와 생명주기 Unit Test를 작성했다.
- 생산 연결과 Unity 검증을 완료한 것으로 기록하지 않았다.
