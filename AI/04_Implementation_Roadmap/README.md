# Implementation Roadmap Documents

이 문서는 Implementation Roadmap 영역의 진입 문서이다.

Implementation Roadmap 영역에 존재하는 문서의 역할과 책임을 정의한다.

---

# 목적

Implementation Roadmap 영역은 구현 순서를 관리한다.

Implementation Roadmap 영역은 개발 단계를 관리한다.

Implementation Roadmap 영역은 구현 우선순위를 관리한다.

Implementation Roadmap 영역은 현재 개발 진행 상태를 관리한다.

Implementation Roadmap 영역은 다음 작업 대상을 관리한다.

Implementation Roadmap 영역은 1차 프로토타입부터 최종 구현까지의 개발 계획을 관리한다.

Implementation Roadmap 영역은 프로젝트 정보를 관리하지 않는다.

Implementation Roadmap 영역은 프로젝트 규칙을 관리하지 않는다.

Implementation Roadmap 영역은 System의 책임을 관리하지 않는다.

Implementation Roadmap 영역은 Feature의 규칙을 관리하지 않는다.

Implementation Roadmap 영역은 작업 기록을 관리하지 않는다.

---

# Implementation Roadmap 정의

Implementation Roadmap은 프로젝트의 개발 계획을 관리하는 문서이다.

Implementation Roadmap 문서는 어떤 순서로 개발해야 하는지를 설명한다.

Implementation Roadmap 문서는 현재 개발 진행 상태를 설명한다.

Implementation Roadmap 문서는 다음에 구현해야 하는 작업을 설명한다.

Implementation Roadmap 문서는 구현 우선순위를 설명한다.

Implementation Roadmap 문서는 개발 단계를 설명한다.

Implementation Roadmap 문서는 구현 방법을 설명하지 않는다.

Implementation Roadmap 문서는 System의 책임을 설명하지 않는다.

Implementation Roadmap 문서는 Feature의 규칙을 설명하지 않는다.

Implementation Roadmap 문서는 작업 기록을 저장하지 않는다.

---

# 문서 생성 기준

아래 조건 중 하나 이상을 만족하는 경우 새로운 Roadmap 문서를 생성할 수 있다.

```text
프로토타입 구현 순서를 정의해야 한다.

개발 단계를 구분하여 관리해야 한다.

구현 우선순위를 변경해야 한다.

대규모 기능 추가에 따른 개발 계획이 필요하다.
```

Roadmap 문서는 프로젝트 전체에서 필요한 최소 개수만 생성한다.

동일한 개발 계획을 여러 Roadmap 문서로 분리하지 않는다.

기존 Roadmap으로 관리 가능한 경우 새로운 Roadmap 문서를 생성하지 않는다.

---

# 문서 작성 형식

새로운 Roadmap 문서를 생성하는 경우 아래 템플릿을 사용한다.

```text
99_Templates/IMPLEMENTATION_ROADMAP_TEMPLATE.md
```

Roadmap 문서는 템플릿 형식을 따라 작성한다.

Roadmap 문서 구조는 Templates 영역에서 관리한다.

---

# 관리 대상

Implementation Roadmap에서는 아래 내용을 관리한다.

```text
개발 단계

현재 개발 진행 상태

다음 작업 대상

구현 우선순위

구현 순서

진행 중인 작업

완료된 작업

보류된 작업

각 단계의 목표

단계 완료 기준
```

---

Implementation Roadmap에서는 아래 내용을 관리하지 않는다.

```text
프로젝트 정보

프로젝트 규칙

System 책임

Feature 규칙

구현 방법

작업 기록

버그 수정 내역
```

---

# 상태 변경 기준

다음 작업을 시작하면 해당 작업을 진행 중인 작업으로 변경한다.

작업이 완료되면 진행 중인 작업에서 완료된 작업으로 변경한다.

작업이 중단되면 해당 작업을 보류된 작업으로 변경한다.

보류된 작업을 다시 시작하면 진행 중인 작업으로 변경한다.

구현 우선순위가 변경되면 Roadmap 문서를 먼저 수정한다.

작업 기록은 Roadmap에 작성하지 않는다.

작업 완료 후 Tasks 영역에 작업 기록을 작성한다.

---

# 문서 작성 원칙

Roadmap 하나당 하나의 개발 계획만 관리한다.

Roadmap은 구현 계획과 개발 진행 상태만 관리한다.

Roadmap은 구현 방법을 관리하지 않는다.

System 책임은 Systems 영역에서 관리한다.

Feature 규칙은 Features 영역에서 관리한다.

프로젝트 정보는 Project 영역에서 관리한다.

프로젝트 규칙은 Rules 영역에서 관리한다.

작업 기록은 Tasks 영역에서 관리한다.

문서 작성 형식은 Templates 영역에서 관리한다.

동일한 내용을 여러 문서에 중복 작성하지 않는다.

각 문서는 자신의 책임 범위만 관리한다.

---

# 작업 기록

Roadmap 관련 작업 기록은 Roadmap 영역에 저장하지 않는다.

Roadmap 관련 작업 기록은 아래 위치에 저장한다.

```text
90_Tasks
```

작업 기록은 적절한 Task Template을 사용하여 작성한다.

---

# 관련 문서

Project 영역은 프로젝트 정보를 관리한다.

Rules 영역은 프로젝트 규칙을 관리한다.

Systems 영역은 시스템 책임을 관리한다.

Features 영역은 기능 규칙을 관리한다.

Implementation Roadmap 영역은 개발 계획과 개발 진행 상태를 관리한다.

Tasks 영역은 실제 작업 기록을 관리한다.

Templates 영역은 문서 작성 형식을 관리한다.

동일한 내용을 여러 영역에 중복 저장하지 않는다.