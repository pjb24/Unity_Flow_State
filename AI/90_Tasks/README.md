# Task Documents

이 문서는 Tasks 영역의 진입 문서이다.

Tasks 영역의 역할을 정의한다.

---

# 목적

Tasks 영역은 실제 작업 기록을 관리한다.

Tasks 영역은 작업 수행 결과를 관리한다.

Tasks 영역은 작업 이력을 관리한다.

Tasks 영역은 문서 작성 형식을 관리하지 않는다.

Tasks 영역은 프로젝트 정보를 관리하지 않는다.

Tasks 영역은 프로젝트 규칙을 관리하지 않는다.

Tasks 영역은 System의 책임을 관리하지 않는다.

Tasks 영역은 Feature의 규칙을 관리하지 않는다.

---

# 저장 대상

Tasks 영역에는 작업 기록만 저장한다.

작업 기록은 작업 완료 후 생성한다.

작업 기록은 Templates 영역의 Task Template을 사용하여 작성한다.

---

# 파일명 규칙

모든 작업 기록은 아래 형식을 사용한다.

```text
YYYYMMDD_<그 날 작업 번호>_작업명.md
```

예시

```text
20260706_01_PlayerMovementRefactor.md

20260706_02_WallRunImplementation.md

20260706_03_DocumentStructureUpdate.md
```

그 날 작업 번호는 해당 날짜에 생성된 작업 기록의 순서를 의미한다.

그 날 작업 번호는 01부터 시작한다.

그 날 작업 번호는 같은 날짜 내에서 중복될 수 없다.

---

# 작업 기록 생성 기준

System 작업은 `SYSTEM_TASK_TEMPLATE.md`를 사용한다.

Feature 작업은 `FEATURE_TASK_TEMPLATE.md`를 사용한다.

버그 수정 작업은 `BUGFIX_TASK_TEMPLATE.md`를 사용한다.

그 외 작업은 `GENERAL_TASK_TEMPLATE.md`를 사용한다.

---

# 문서 작성 원칙

작업이 완료된 후 작업 기록을 작성한다.

현재 수행한 작업만 기록한다.

확인되지 않은 내용을 기록하지 않는다.

추측을 기록하지 않는다.

동일한 내용을 여러 작업 기록에 중복 작성하지 않는다.
