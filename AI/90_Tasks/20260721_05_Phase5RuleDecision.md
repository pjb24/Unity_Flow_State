# 작업 정보

## 작업명

Phase 5 Rule Decision

---

## 작업 일자

20260721

---

## 작업 담당자

AI

---

# 작업 목적

Phase 5 Manual Steps의 미정 규칙에 대한 사용자 선택을 검토하고 관련 Roadmap, System, Feature와 Task 문서에 일관되게 반영한다.

---

# 작업 대상

- Phase 5 구현 범위와 보류 범위
- ResultMenu 기능 규칙
- UI 입력 전달과 처리 책임
- Phase 5 수동 검증 및 밸런스 기준

---

# 작업 전 상태

Phase 5에는 InfiniteMode가 보류되어 있으면서 InfiniteMode 전용 ScoreRecord가 구현 대상으로 포함되어 있었다.

UIInputSystem의 입력 상태 출력 대상과 사용자가 선택한 GameSystem 중심 입력 해석 책임이 일치하지 않았다.

ResultPanel의 Navigate, Submit과 Cancel 동작, 필수 입력 장치, 플레이 횟수와 밸런스 성공 기준이 확정되어 있지 않았다.

---

# 조사 내용

- 사용자가 선택한 11개 Phase 5 규칙을 상호 비교했다.
- IMPLEMENTATION_ROADMAP_001.md의 Phase 5와 보류 항목을 확인했다.
- GameSystem, UIInputSystem과 UIManagementSystem의 입력 및 출력 책임을 확인했다.
- ScoreRecord, InfiniteMode와 GamePause Feature 규칙을 확인했다.
- Phase 5 Manual Steps의 미정 항목과 후속 검증 절차를 확인했다.

---

# 작업 내용

- InfiniteMode, ScoreRecord와 GamePause를 Phase 5에서 보류했다.
- Phase 5 구현 대상의 ScoreRecord를 ResultMenu로 변경했다.
- ResultMenu Feature 문서를 생성하여 Retry, Quit, Navigate, Submit과 Cancel 규칙을 정의했다.
- UIInputSystem의 입력 상태 출력 대상을 GameSystem으로 변경했다.
- GameSystem이 UI 입력 의미와 실행 흐름을 결정하도록 책임을 반영했다.
- UIManagementSystem이 UI 선택 상태만 관리하고 선택 결과의 게임 동작은 결정하지 않도록 책임을 반영했다.
- Phase 5 필수 입력 장치를 키보드와 마우스로 확정했다.
- 최소 5회의 플레이를 기준값 2회와 최종 후보값 3회로 배분했다.
- 기록 항목, 조정 대상 값과 상대 성공 기준을 Phase 5 Manual Steps에 반영했다.

---

# 영향 범위

- Systems
- Features
- Implementation Roadmap
- Tasks

---

# 검증 내용

- ResultMenu의 Retry와 Quit이 Navigate 대상 두 개를 제공하는지 확인했다.
- 키보드 Navigate·Submit·Cancel과 마우스 Point·Click을 필수 검증 대상으로 사용하고 게임패드는 완료 조건에서 제외했는지 확인했다.
- 플레이 중 Player Action Map만, 결과 화면에서 UI Action Map만 활성화하도록 정책을 확인했다.
- Result UI State가 아닐 때 키보드와 마우스 UI 동작을 처리하지 않고 Submit과 Click을 중복 실행하지 않는 예외 규칙을 확인했다.
- 최소 5회가 기준값 2회와 최종 후보값 3회로 상대 비교 및 최종 3회 연속 클리어 기준을 충족할 수 있는지 확인했다.
- `git diff --check`로 문서 변경의 공백 오류가 없는지 확인했다.

---

# 검증 결과

- 후속 결정으로 밸런스 처리는 Phase 5 범위에서 제외되었다.
- 이 문서의 플레이 횟수와 밸런스 성공 기준은 향후 별도 밸런스 작업에서 참고하며 Phase 5 완료 조건으로 사용하지 않는다.

선택한 규칙은 확정된 해석을 적용하면 서로 충돌하지 않는다.

관련 문서의 책임 방향, 구현 범위, 보류 범위와 검증 기준이 일치한다.

실제 Phase 5 구현과 Unity 검증은 아직 수행하지 않았다.

---

# 후속 작업

- Phase 5는 완료되었으며 밸런스 조정은 별도 후속 작업으로 관리한다.
- UIInputSystem과 ResultMenu를 구현하고 관련 Test를 추가한다.
- Phase 5 Manual Steps의 Step 2부터 순서대로 수행한다.

---

# 관련 문서

## Rules

- AI/01_Rules/AI_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md

## Systems

- AI/02_Systems/GameSystem.md
- AI/02_Systems/UIInputSystem.md
- AI/02_Systems/UIManagementSystem.md

## Features

- AI/03_Features/ResultMenu.md
- AI/03_Features/ScoreRecord.md
- AI/03_Features/InfiniteMode.md
- AI/03_Features/GamePause.md

## Roadmap

- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md

## Template

- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

---

# 관련 작업 기록

- AI/90_Tasks/20260721_04_Phase5ManualSteps.md

---

# 작성 완료 기준

- GENERAL_TASK_TEMPLATE.md의 필수 섹션을 작성했다.
- 확인한 선택과 문서에 근거한 내용만 기록했다.
- 실제 구현과 검증을 완료한 것으로 기록하지 않았다.
