# Template Documents

이 문서는 Templates 영역의 진입 문서이다.

Templates 영역에 존재하는 문서와 각 문서의 책임을 정의한다.

---

# 목적

Templates 영역은 문서 작성 형식을 관리한다.

Templates 영역은 설계 문서 템플릿을 관리한다.

Templates 영역은 작업 기록 템플릿을 관리한다.

Templates 영역은 실제 작업 내용을 저장하지 않는다.

---

# 문서 목록

Templates 영역에는 아래 문서가 존재해야 한다.

```text
SYSTEM_DOCUMENT_TEMPLATE.md

FEATURE_DOCUMENT_TEMPLATE.md

SYSTEM_TASK_TEMPLATE.md

FEATURE_TASK_TEMPLATE.md

GENERAL_TASK_TEMPLATE.md

BUGFIX_TASK_TEMPLATE.md
```

---

# 설계 문서 템플릿

설계 문서 템플릿은 새로운 문서를 생성할 때 사용한다.

설계 문서 템플릿은 문서 구조만 정의한다.

설계 문서 템플릿은 실제 내용을 저장하지 않는다.

---

## SYSTEM_DOCUMENT_TEMPLATE.md

System 문서 작성 형식을 정의한다.

새로운 System 문서를 생성할 때 사용한다.

생성 대상은 Systems 영역의 문서이다.

---

## FEATURE_DOCUMENT_TEMPLATE.md

Feature 문서 작성 형식을 정의한다.

새로운 Feature 문서를 생성할 때 사용한다.

생성 대상은 Features 영역의 문서이다.

---

# 작업 기록 템플릿

작업 기록 템플릿은 작업 내용을 기록할 때 사용한다.

작업 기록 템플릿은 기록 형식만 정의한다.

작업 기록 템플릿은 실제 작업 내용을 저장하지 않는다.

---

## SYSTEM_TASK_TEMPLATE.md

System 관련 작업 기록 형식을 정의한다.

System 수정 작업에 사용한다.

System 구조 변경 작업에 사용한다.

System 리팩토링 작업에 사용한다.

---

## FEATURE_TASK_TEMPLATE.md

Feature 관련 작업 기록 형식을 정의한다.

Feature 구현 작업에 사용한다.

Feature 수정 작업에 사용한다.

Feature 확장 작업에 사용한다.

---

## GENERAL_TASK_TEMPLATE.md

일반 작업 기록 형식을 정의한다.

System 또는 Feature로 분류되지 않는 작업에 사용한다.

문서 정리 작업에 사용한다.

구조 검토 작업에 사용한다.

규칙 정리 작업에 사용한다.

---

## BUGFIX_TASK_TEMPLATE.md

버그 수정 기록 형식을 정의한다.

버그 원인 분석 작업에 사용한다.

버그 수정 작업에 사용한다.

버그 검증 작업에 사용한다.

---

# 작업 기록 저장 위치

작업 기록은 Templates 영역에 저장하지 않는다.

작업 기록은 아래 위치에 저장한다.

```text
90_Tasks
```

Templates 영역은 기록 형식만 관리한다.

90_Tasks 영역은 실제 작업 기록만 관리한다.

---

# 작업 기록 파일명 규칙

모든 작업 기록 문서는 아래 형식을 사용한다.

```text
YYYYMMDD_<그 날 작업 번호>_작업명.md
```

예시

```text
20260604_01_PlayerStateRefactor.md

20260604_02_WallRunRuleUpdate.md

20260604_03_SaveSystemCleanup.md
```

그 날 작업 번호는 해당 날짜에 생성된 작업 기록의 순서를 의미한다.

그 날 작업 번호는 01부터 시작한다.

그 날 작업 번호는 같은 날짜 내에서 중복될 수 없다.

---

# 템플릿 선택 기준

새로운 System 문서를 생성하는 경우

```text
SYSTEM_DOCUMENT_TEMPLATE.md
```

를 사용한다.

---

새로운 Feature 문서를 생성하는 경우

```text
FEATURE_DOCUMENT_TEMPLATE.md
```

를 사용한다.

---

System 관련 작업을 기록하는 경우

```text
SYSTEM_TASK_TEMPLATE.md
```

를 사용한다.

---

Feature 관련 작업을 기록하는 경우

```text
FEATURE_TASK_TEMPLATE.md
```

를 사용한다.

---

System 또는 Feature로 분류할 수 없는 작업을 기록하는 경우

```text
GENERAL_TASK_TEMPLATE.md
```

를 사용한다.

---

버그 수정 작업을 기록하는 경우

```text
BUGFIX_TASK_TEMPLATE.md
```

를 사용한다.

---

# 문서 생성 흐름

새로운 System 문서를 생성하는 경우

```text
1. SYSTEM_DOCUMENT_TEMPLATE.md를 사용한다.
2. 02_Systems/<SystemName>.md 형식으로 네이밍을 사용한다.
```

---

새로운 Feature 문서를 생성하는 경우

```text
1. FEATURE_DOCUMENT_TEMPLATE.md를 사용한다.
2. 03_Features/<FeatureName>.md 형식으로 네이밍을 사용한다.
```

---

System 작업을 기록하는 경우

```text
1. SYSTEM_TASK_TEMPLATE.md를 사용한다.
2. 90_Tasks/YYYYMMDD_<그 날 작업 번호>_작업명.md 형식으로 네이밍을 사용한다.
```

---

Feature 작업을 기록하는 경우

```text
1. FEATURE_TASK_TEMPLATE.md를 사용한다.
2. 90_Tasks/YYYYMMDD_<그 날 작업 번호>_작업명.md 형식으로 네이밍을 사용한다.
```

---

일반 작업을 기록하는 경우

```text
1. GENERAL_TASK_TEMPLATE.md를 사용한다.
2. 90_Tasks/YYYYMMDD_<그 날 작업 번호>_작업명.md 형식으로 네이밍을 사용한다.
```

---

버그 수정 작업을 기록하는 경우

```text
1. BUGFIX_TASK_TEMPLATE.md를 사용한다.
2. 90_Tasks/YYYYMMDD_<그 날 작업 번호>_작업명.md 형식으로 네이밍을 사용한다.
```

---

# Templates 와 다른 영역의 관계

Project 영역은 프로젝트 정보를 관리한다.

Rules 영역은 프로젝트 규칙을 관리한다.

Systems 영역은 시스템 책임을 관리한다.

Features 영역은 기능 규칙을 관리한다.

Tasks 영역은 실제 작업 기록을 관리한다.

Templates 영역은 문서 작성 형식을 관리한다.

동일한 내용을 여러 영역에 중복 저장하지 않는다.

---
