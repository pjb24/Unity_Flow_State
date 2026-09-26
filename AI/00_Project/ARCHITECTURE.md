# 프로젝트 구조

프로젝트는 아래 영역으로 구성한다.

## Game

게임의 전체 실행을 관리한다.

각 System을 연결하여 게임을 진행한다.

---

## Systems

게임의 핵심 책임을 담당하는 System을 관리한다.

각 System은 하나의 책임만 담당한다.

---

## Features

사용자가 경험하는 기능을 관리한다.

각 Feature는 하나 이상의 System을 이용하여 동작한다.

---

## Runtime Data

게임 실행 중 사용하는 데이터를 관리한다.

게임 종료 시 모든 데이터는 제거된다.

---

# 시스템 구조

프로젝트는 여러 개의 독립적인 System으로 구성한다.

각 System은 자신의 책임만 담당한다.

System 간 통신은 필요한 데이터만 전달한다.

System의 세부 책임은 System 문서에서 관리한다.

---

# 기능 구조

프로젝트는 사용자가 경험하는 Feature로 구성한다.

Feature는 하나 이상의 System을 이용하여 동작한다.

Feature의 동작 규칙은 Feature 문서에서 관리한다.

---

# 데이터 구조

현재 Run 데이터와 확정 Result Data는 Runtime Data로 관리한다.

Runtime Data는 게임 시작 시 생성되고 게임 종료 시 제거된다.

Settings, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록 캐시와 제출 대기열은 로컬 영구 데이터로 관리한다.

온라인 Leaderboard의 계정 귀속 최고 기록과 순위는 서버 데이터로 관리한다.

---

# 실행 흐름

## 게임 시작

게임을 시작한다.

게임 실행에 필요한 Runtime Data를 생성한다.

필요한 System을 초기화한다.

Stage를 시작할 준비를 완료한다.

---

## 게임 진행

플레이어 입력을 처리한다.

입력 결과에 따라 필요한 Feature를 수행한다.

Feature 수행 결과를 Runtime Data에 반영한다.

필요한 System 간 데이터를 전달한다.

Stage 종료 조건을 담당하는 System이 종료 조건을 판단한다.

종료 조건이 만족되면 Stage 종료 이벤트를 발생시킨다.

게임 종료 처리는 Stage 종료 이벤트를 기준으로 시작한다.

---

## 게임 종료

Stage 종료 조건을 만족하면 게임을 종료한다.

필요한 결과 데이터를 생성한다.

Runtime Data를 제거한다.

---

# 저장 구조

ResultSystem은 Mode별 Result Data를 생성하고 관리한다.

RecordSubmissionSystem은 확정 Result를 제출 후보로 판정하고, 로컬 대기열과 온라인 제출 경계를 관리한다.

저장소와 온라인 서비스 구현은 게임 로직과 Repository 경계로 분리한다.

게임 종료 시 현재 Run Runtime Data는 제거하지만, 로컬 영구 데이터와 제출 대기열은 제거하지 않는다.

---

# 외부 시스템

## 게임 엔진

- Unity

---

## 개발 환경

- C#

---

## 버전 관리

- Git

---

# 구조 제약 사항

각 System은 하나의 책임만 담당한다.

Feature의 규칙은 Feature 문서에서 관리한다.

System의 책임은 System 문서에서 관리한다.

Project 문서에는 프로젝트 수준의 구조만 작성한다.

현재 Run Runtime Data와 로컬·서버 영구 데이터의 소유 경계를 분리한다.

게임 로직은 특정 파일 형식, 저장 경로 또는 서버 SDK를 직접 참조하지 않는다.

리더보드 기록 생성·저장·제출·조회 책임을 분리한다.

---

# 관련 문서

## Project

- PROJECT_OVERVIEW.md
- PROJECT_MEMORY.md

---

## Rules

- AI_RULE.md
- IMPLEMENTATION_RULE.md

---

## Systems

- ResultSystem.md
- RecordSubmissionSystem.md

---

## Features

- Leaderboard.md
- RecordSubmission.md
