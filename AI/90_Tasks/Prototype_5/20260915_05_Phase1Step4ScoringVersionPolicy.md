# 작업 정보

## 작업명

Prototype 5 Phase 1 Step 4 Scoring Version 정책 확정

## 작업 일자

20260915

## 작업 담당자

AI, 사용자

## 작업 상태

완료

---

# 작업 목적

기존 Distance Score와 새 Momentum Score를 같은 기록으로 비교하지 않도록 Scoring Version과 호환 정책을 확정한다.

---

# 작업 대상

- Scoring Version 형식과 현재 값
- Version의 단일 소유 위치와 Runtime 및 Result 전달
- Pause, Result, Retry와 새 Run 생명주기
- 불일치 및 Version 없는 기록 처리
- Version 증가 조건
- Roadmap 7 저장, 서버와 Leaderboard 책임

---

# 작업 전 상태

- Runtime Data, Score 계산과 Result Data에 Scoring Version이 없었다.
- 기존 점수와 Momentum Bonus가 포함된 새 점수를 구분할 계약이 없었다.
- 로컬 저장, 서버와 Leaderboard는 아직 구현되지 않았다.

---

# 조사 내용

- 현재 Score 관련 생산 코드와 Test에는 Scoring Version 필드나 검증이 없다.
- 일반 Stage는 TimeRecord를 사용하고 InfiniteMode만 ScoreRecord를 사용한다.
- 저장과 Leaderboard 구현은 Roadmap 7 범위이다.

---

# 작업 내용

## 확정 정책

- Scoring Version은 양의 `int`를 사용하고 `0`은 유효하지 않거나 적용되지 않는 값이다.
- 기존 Momentum Bonus가 없는 Score 규칙은 Version `1`이다.
- 새 Momentum Score 규칙은 Version `2`이다.
- 현재 Version은 단일 불변 Scoring 규칙 정의가 소유한다.
- 새 InfiniteMode Run은 현재 Version `2`를 Runtime Data에 복사하여 고정한다.
- Playing, Pause, Resume와 Result에서 한 Run의 Version을 변경하지 않는다.
- Retry와 새 Run은 새 Runtime Data에 현재 Version을 다시 설정한다.
- Stage Mode는 Scoring Version을 적용하지 않고 값 `0`을 사용한다.
- Runtime, Score 계산 상태와 Result 요청의 Version이 다르면 요청을 거부하고 상태를 유지한다.
- Version 없음, `0`, 지원하지 않는 Version과 Run 중 Version 변경을 거부한다.
- Version이 없는 기존 기록은 Version `1`로 추정하지 않고 무효로 처리한다.
- 동일 플레이의 최종 Score가 달라질 수 있는 규칙 변경과 버그 수정은 Version을 증가시킨다.
- UI 전용 변경, Score 불변 성능 개선, 논리 거리를 보존하는 Rebase, Camera, Test와 문서 변경은 Version을 증가시키지 않는다.
- 일반 플레이 HUD에는 Scoring Version을 표시하지 않는다.

## Roadmap 7 계약

- 저장 데이터와 서버 요청에는 Game Mode와 Scoring Version을 함께 포함한다.
- Leaderboard는 Game Mode와 Scoring Version 조합으로 분리한다.
- 다른 Scoring Version의 기록끼리는 순위, 최고 점수와 동점을 비교하지 않는다.
- Version이 없는 기록은 저장 또는 Leaderboard 입력에서 무효로 처리한다.
- 저장 형식, 서버 검증, 조회, 표시와 지원 종료 정책의 실제 구현은 Roadmap 7 책임이다.

## Phase 1 최소 계약

- 현재 Scoring Version `2`와 무효값 `0`을 정의한다.
- InfiniteMode Runtime Data에 Version을 고정한다.
- Score 상태와 Runtime Version의 일치를 검증한다.
- Result Data에 Version을 전달한다.
- 중복, 무효와 불일치 요청을 거부한다.
- 위 계약을 Scene 없이 Edit Mode Unit Test로 검증한다.

---

# 영향 범위

- Feature: InfiniteMode, ScoreRecord
- System: RuntimeDataSystem, ResultSystem
- Task: Phase 1 Step 4 상태와 결정 기록
- 후속 Runtime: Scoring Version 정의, Runtime Data와 Result Data
- 후속 Roadmap 7: 저장, 서버와 Leaderboard 분리

---

# 검증 내용

- 기존 규칙 Version `1`과 신규 규칙 Version `2`의 의미를 구분했다.
- Run 생성부터 Result 전달까지 Version이 변하지 않는지 생명주기를 대조했다.
- 무효, 지원하지 않는 값과 불일치 요청의 거부 계약을 확인했다.
- Phase 1 Runtime 계약과 Roadmap 7 영속화 책임을 분리했다.
- Unity Editor, Unity Test Runner와 Build는 실행하지 않았다.

---

# 검증 결과

- Step 4의 세 완료 조건을 충족했다.
- 정책 문서만 변경했고 Runtime 코드, Test, Scene과 Prefab은 변경하지 않았다.
- 저장, 서버와 Leaderboard는 구현하지 않았다.

---

# 후속 작업

- Step 5에서 Scoring Version을 순수 상태 및 계산 코드와 Edit Mode Unit Test로 고정한다.
- Roadmap 7에서 저장, 서버와 Leaderboard 분리를 구현한다.

---

# 관련 문서

- `AI/02_Systems/RuntimeDataSystem.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260915_03_Phase1Step2MomentumScorePolicy.md`
- `AI/90_Tasks/Prototype_5/20260915_04_Phase1Step3WorldRebasePolicy.md`

---

# 작성 완료 기준

- Scoring Version 값, 소유권, 생명주기와 거부 조건을 기록했다.
- Version 없는 기록을 무효 처리하는 사용자 결정을 기록했다.
- Phase 1과 Roadmap 7 책임을 구분했다.
- 구현하지 않은 Runtime, 저장, 서버와 Leaderboard를 완료로 기록하지 않았다.
