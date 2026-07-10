# Feature Documents

이 문서는 Features 영역의 진입 문서이다.

Features 영역에 존재하는 문서의 역할과 책임을 정의한다.

---

# 목적

Features 영역은 기능 규칙을 관리한다.

Features 영역은 기능 동작을 관리한다.

Features 영역은 기능 조건을 관리한다.

Features 영역은 기능 결과를 관리한다.

Features 영역은 시스템 책임을 관리하지 않는다.

Features 영역은 프로젝트 규칙을 관리하지 않는다.

Features 영역은 작업 기록을 관리하지 않는다.

---

# Feature 정의

Feature는 사용자가 경험하는 기능이다.

Feature 문서는 기능이 어떻게 동작해야 하는지를 설명한다.

Feature 문서는 기능 규칙을 설명한다.

Feature 문서는 시스템 책임을 설명하지 않는다.

Feature 문서는 구현 방법을 설명하지 않는다.

Feature 문서는 작업 기록을 저장하지 않는다.

---

# Feature 문서 생성 기준

아래 조건 중 하나 이상을 만족하는 경우 새로운 Feature 문서를 생성할 수 있다.

```text
사용자가 인식할 수 있는 기능이다.

독립적인 기능 규칙이 존재한다.

독립적으로 관리할 필요가 있다.

여러 System이 함께 동작한다.

기능 규칙 변경 가능성이 존재한다.
```

---

# Feature 문서 작성 형식

새로운 Feature 문서를 생성하는 경우 아래 템플릿을 사용한다.

```text
99_Templates/FEATURE_DOCUMENT_TEMPLATE.md
```

Feature 문서는 템플릿 형식을 따라 작성한다.

Feature 문서 구조는 Features 영역에서 정의하지 않는다.

Feature 문서 구조는 FEATURE_DOCUMENT_TEMPLATE.md에서 정의한다.

---

# Systems 와 Features

Systems 문서는 책임을 설명한다.

Features 문서는 기능 규칙을 설명한다.

Systems 문서는 아래 질문에 답한다.

```text
누가 담당하는가?
```

Features 문서는 아래 질문에 답한다.

```text
어떻게 동작해야 하는가?
```

동일한 내용을 Systems 와 Features 양쪽에 작성하지 않는다.

---

# 작업 기록

Feature 관련 작업 기록은 Features 영역에 저장하지 않는다.

Feature 관련 작업 기록은 아래 위치에 저장한다.

```text
90_Tasks
```

Feature 관련 작업 기록을 작성하는 경우 아래 템플릿을 사용한다.

```text
99_Templates/FEATURE_TASK_TEMPLATE.md
```

---

# 문서 확인 순서

특정 Feature를 수정하는 경우 아래 순서로 문서를 확인한다.

```text
1. Features 영역 README

2. 관련 Feature 문서

3. 관련 System 문서
```

---

# 문서 작성 원칙

Feature 하나당 문서 하나를 사용한다.

하나의 Feature 문서에 여러 Feature를 정의하지 않는다.

Feature 문서는 기능 규칙만 설명한다.

시스템 책임은 Systems 영역에서 관리한다.

프로젝트 규칙은 Rules 영역에서 관리한다.

작업 기록은 Tasks 영역에서 관리한다.

문서 작성 형식은 Templates 영역에서 관리한다.

동일한 내용을 여러 문서에 중복 작성하지 않는다.

각 문서는 자신의 책임 범위만 관리한다.
