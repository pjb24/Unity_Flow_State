# Project Documents

이 문서는 Project 영역의 진입 문서이다.

Project 영역에 존재하는 문서와 각 문서의 책임을 정의한다.

---

# 목적

Project 영역은 프로젝트 정보를 관리한다.

Project 영역은 프로젝트 목표를 관리한다.

Project 영역은 프로젝트 구조를 관리한다.

Project 영역은 프로젝트 설계를 관리한다.

Project 영역은 프로젝트 규칙을 관리하지 않는다.

Project 영역은 시스템 책임을 관리하지 않는다.

Project 영역은 기능 규칙을 관리하지 않는다.

Project 영역은 작업 기록을 관리하지 않는다.

---

# 문서 목록

Project 영역에는 아래 문서가 존재해야 한다.

```text
PROJECT_OVERVIEW.md

ARCHITECTURE.md

PROJECT_MEMORY.md
```

---

# PROJECT_OVERVIEW.md

프로젝트의 목표를 설명한다.

프로젝트의 범위를 설명한다.

프로젝트의 핵심 개념을 설명한다.

프로젝트가 제공해야 하는 내용을 설명한다.

프로젝트가 제공하지 않는 내용을 설명한다.

---

# ARCHITECTURE.md

프로젝트의 구조를 설명한다.

프로젝트를 구성하는 주요 요소를 설명한다.

프로젝트 수준의 설계 결정을 설명한다.

프로젝트 전반에 적용되는 구조를 설명한다.

---

# PROJECT_MEMORY.md

프로젝트 진행 과정에서 결정된 사항을 기록한다.

프로젝트 진행 과정에서 변경된 사항을 기록한다.

향후 작업 시 반드시 알아야 하는 사항을 기록한다.

프로젝트 진행 과정에서 확인된 사실을 기록한다.

---

# 문서 확인 순서

새로운 작업을 시작하는 경우 아래 순서로 문서를 확인한다.

```text
1. PROJECT_OVERVIEW.md

2. ARCHITECTURE.md

3. PROJECT_MEMORY.md
```

---

# 문서 확인 목적

PROJECT_OVERVIEW.md를 통해 아래 내용을 확인한다.

```text
무엇을 만드는가?
```

---

ARCHITECTURE.md를 통해 아래 내용을 확인한다.

```text
어떤 구조로 만드는가?
```

---

PROJECT_MEMORY.md를 통해 아래 내용을 확인한다.

```text
현재까지 무엇이 결정되었는가?
```

---

# 문서 생성 위치

Project 문서는 아래 위치에 저장한다.

```text
00_Project
```

---

# 문서 작성 책임

동일한 내용을 여러 문서에 중복 작성하지 않는다.

각 문서는 자신의 책임 범위만 관리한다.

---

# Project 와 다른 영역의 관계

Project 영역은 프로젝트 정보를 관리한다.

Rules 영역은 프로젝트 규칙을 관리한다.

Systems 영역은 시스템 책임을 관리한다.

Features 영역은 기능 규칙을 관리한다.

Tasks 영역은 실제 작업 기록을 관리한다.

Templates 영역은 문서 작성 형식을 관리한다.

동일한 내용을 여러 영역에 중복 저장하지 않는다.

---

# 문서 작성 원칙

Project 영역 문서는 프로젝트 수준의 정보만 관리한다.

시스템 수준 정보는 Systems 영역에서 관리한다.

기능 수준 정보는 Features 영역에서 관리한다.

규칙은 Rules 영역에서 관리한다.

작업 기록은 Tasks 영역에서 관리한다.

문서 작성 형식은 Templates 영역에서 관리한다.

문서에 정의된 내용을 우선한다.
