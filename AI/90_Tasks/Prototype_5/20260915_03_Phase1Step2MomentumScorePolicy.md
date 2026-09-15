# 작업 정보

## 작업명

Prototype 5 Phase 1 Step 2 Momentum Landing Score 정책 확정

## 작업 일자

20260915

## 작업 담당자

AI, 사용자

## 작업 상태

완료

---

# 작업 목적

Momentum Landing의 속도 증가 효과를 제거하고 연속 성공을 InfiniteMode의 시간제 거리 Score 배율로 보상하기 위한 규칙을 확정한다.

---

# 작업 대상

- Momentum Landing 이동 속도와 연속 성공 판정
- 배율 단계, 상한과 배율별 유지 시간
- 일반 착지, Wall 접촉, 시간 만료와 Run 생명주기
- Base Distance Score, Momentum Bonus, Collectible Score와 Total Score 관계
- InfiniteHUD와 분리된 Momentum HUD 및 Result 표시

---

# 작업 전 상태

- Momentum Landing 성공은 수평 속도를 `1.15`배로 증가시키고 최대 `14`로 제한했다.
- Distance Score는 이동 거리와 단위 점수만 사용했고 Momentum Bonus를 구분하지 않았다.
- Momentum 연속 성공, 배율, 유지 시간과 최고 배율 상태가 없었다.
- 기존 InfiniteHUD와 Result에는 Distance Score, Collectible Score와 Total Score만 표시했다.

---

# 조사 내용

- 모든 Pattern의 길이는 `44`이고 기본 속도 `8`에서 한 Pattern 이동 시간은 약 `5.5`초이다.
- 최대 배율의 유지 시간을 `6.5`초로 정하면 기본 속도의 한 Pattern 이동 시간보다 `1.0`초 길다.
- Collectible Score는 Momentum 상태와 World X를 참조하지 않는 독립된 Runtime Data에서 누적된다.
- 현재 Result Data에는 Momentum Bonus와 최고 배율 필드가 없다.

---

# 작업 내용

## 확정 정책

- Momentum Landing은 이동 속도를 증가시키지 않는다.
- 실제 Ground 접촉으로 완료된 Momentum Landing 한 번을 연속 성공 한 번으로 판정한다.
- 배율은 `1.00x`에서 시작해 성공마다 `0.25x` 증가하고 `3.00x`에서 고정한다.
- 배율별 유지 시간은 `1.25x`부터 `10.0`, `9.5`, `9.0`, `8.5`, `8.0`, `7.5`, `7.0`, `6.5`초이다.
- 성공 시 새 배율의 전체 유지 시간으로 갱신하고 최대 배율의 성공은 `6.5`초만 갱신한다.
- 일반 착지와 Wall 접촉만으로 배율을 초기화하지 않는다.
- 유지 시간이 만료되면 연속 성공과 배율을 `0회`, `1.00x`로 초기화한다.
- Pause와 Resume은 상태를 보존하고 Pause 중 시간을 차감하지 않는다.
- Result와 낙하는 획득한 Score를 보존하고 Retry와 새 Run은 전체 상태를 초기화한다.
- 같은 물리 갱신의 성공과 만료는 성공을 우선한다.
- 새 배율은 성공 착지 이후 발생한 거리 증가분부터 적용한다.
- Momentum 배율은 Distance Score 증가분에만 적용하고 Collectible Score에는 적용하지 않는다.
- Momentum Bonus는 정밀값으로 누적하고 정수로 표시하거나 기록할 때만 내림한다.
- Distance Score는 Base Distance Score와 Momentum Bonus의 포화 합이다.
- Total Score는 Distance Score와 Collectible Score의 포화 합이다.
- 모든 정수 Score는 `0`부터 `int.MaxValue`까지로 제한한다.
- HUD는 기존 InfiniteHUD와 분리된 Momentum HUD를 우측 하단에 표시한다.
- Momentum HUD는 현재 배율과 유지 시간 Fill Bar를 표시한다.
- Bar는 남은 시간 비율에 따라 청록색, 초록색, 노란색, 주황색, 빨간색 순서의 연속 Gradient를 사용한다.
- Result는 Base Distance Score, Momentum Bonus, Collectible Score, Total Score와 최고 Momentum 배율을 표시한다.

## 책임 경계

- `MomentumLanding.md`는 성공, 연속 상태, 배율 단계와 유지 시간 생명주기를 관리한다.
- `InfiniteMode.md`는 Score 계산 구성과 Momentum HUD 표시 규칙을 관리한다.
- `ScoreRecord.md`는 최종 Score 구성 요소와 최고 배율의 기록 계약을 관리한다.
- Scoring Version 정책은 Step 4에서 별도로 확정한다.
- Runtime 코드, Test, Scene, Prefab과 UI 연결은 아직 변경하지 않는다.

---

# 영향 범위

- Features: MomentumLanding, InfiniteMode, ScoreRecord
- Tasks: Phase 1 Step 2 상태와 결정 기록
- 후속 Runtime Data: Momentum 상태, Score 구성과 최고 배율
- 후속 UI: 우측 하단 Momentum HUD와 InfiniteMode Result

---

# 검증 내용

- 확정 정책의 배율 단계, 유지 시간과 초기화 조건이 서로 모순되지 않는지 정적으로 대조했다.
- Distance Score와 Total Score 관계 및 `int.MaxValue` 포화 지점을 구분했다.
- Pause, Result, Retry와 새 Run의 상태 생명주기를 구분했다.
- Feature 문서별 책임이 중복되지 않도록 분리했다.
- Unity Editor, Unity Test Runner와 Build는 실행하지 않았다.

---

# 검증 결과

- Step 2의 네 완료 조건을 충족했다.
- 정책 문서만 변경했고 Runtime 코드, Test, Scene과 Prefab은 변경하지 않았다.
- 생산 연동과 자동 Test는 Step 5 및 후속 Phase 책임으로 남아 있다.

---

# 후속 작업

- Step 3에서 World Rebase와 누적 논리 거리 정책을 확정한다.
- Step 4에서 Scoring Version과 호환 정책을 확정한다.
- Step 5에서 확정 정책을 순수 상태 및 계산 코드와 Edit Mode Unit Test로 고정한다.

---

# 관련 문서

- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_5/20260915_02_Phase1Step1Investigation.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260915_02_Phase1Step1Investigation.md`

---

# 작성 완료 기준

- 사용자가 확정한 Momentum Landing Score 정책을 기록했다.
- 수치, 적용 범위, 초기화, 생명주기와 표시 계약을 추측 없이 기록했다.
- 후속 구현과 검증을 완료한 것으로 기록하지 않았다.
