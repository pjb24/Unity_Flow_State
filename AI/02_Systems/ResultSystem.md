# 시스템 개요

## 시스템명

ResultSystem

---

## 목적

Stage 종료 결과를 기반으로 Result Data를 생성한다.

생성한 Result Data를 필요한 System에 제공한다.

---

# 시스템 책임

- Result Data를 생성한다.
- Result Data를 관리한다.
- Result Data를 제공한다.

---

# Result Data

Result Data는 Stage 종료 시 생성되는 결과 데이터를 의미한다.

Result Data는 Stage 종료 시점의 정보를 하나의 데이터로 구성한다.

ResultSystem은 Result Data만 생성하고 관리한다.

Result Data의 저장, 표시 및 평가는 다른 System이 담당한다.

---

# 시작 조건

- StageSystem이 Stage 종료 이벤트를 전달하였다.

---

# 종료 조건

## 정상 종료

- Result Data 생성이 완료된다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 관리 대상

- Result Data
- Result Data 생성 상태

---

# 입력

| 입력 | 출처 |
|------|------|
| Stage 종료 이벤트 | StageSystem |
| Stage 결과 정보 | StageSystem |
| 클리어 시간 | TimerSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| Result Data | UIManagementSystem |

---

# 시스템 경계

## 담당 범위

- Result Data 생성
- Result Data 관리
- Result Data 제공

---

## 담당하지 않는 범위

- Stage 종료 조건 판단
- Stage 종료 결정
- Result Data 저장
- Result Data 표시
- Result Data 평가
- 게임 진행 관리
- Feature 규칙 정의

---

# 관련 System

- StageSystem
- TimerSystem
- UIManagementSystem

---

# 제약 사항

- Result Data만 생성하고 관리한다.
- Stage 종료 조건을 판단하지 않는다.
- StageSystem이 전달한 Stage 종료 이벤트만 사용한다.
- Result Data를 저장하지 않는다.
- Result Data를 화면에 표시하지 않는다.
- Result Data를 평가하지 않는다.
- Feature 규칙을 정의하지 않는다.

---

# 문서 작성 원칙

현재 System의 정의만 작성한다.

System의 책임만 작성한다.

Feature 규칙을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

변경 이력을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

System 하나당 문서 하나를 사용한다.