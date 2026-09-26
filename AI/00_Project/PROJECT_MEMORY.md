# 목적

프로젝트 진행 과정에서 현재도 유효한 결정 사항을 기록한다.

향후 작업에 영향을 주는 결정 사항을 기록한다.

---

# 프로젝트 결정 사항

## 프로젝트 구조

- Project, Rules, Systems, Features, Tasks, Templates 영역을 분리하여 관리한다.
- Project 문서는 프로젝트 수준의 정보만 관리한다.
- System의 책임은 Systems 문서에서 관리한다.
- Feature의 규칙은 Features 문서에서 관리한다.
- 작업 기록은 Tasks 문서에서 관리한다.
- 문서 작성 형식은 Templates에서 관리한다.
- 동일한 내용을 여러 문서에 중복 작성하지 않는다.

---

## 프로젝트 규칙

- 문서는 자신의 책임 범위만 관리한다.
- 프로젝트 수준의 내용과 System, Feature 수준의 내용을 혼합하지 않는다.
- 현재 Run 데이터와 확정 Result Data는 Runtime 전용이다.
- Settings, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록 캐시와 제출 대기열은 로컬 영구 데이터다.
- 온라인 최고 기록과 순위는 계정 귀속 서버 데이터다.
- 온라인 기능 요청 시 Anonymous 계정을 사용하며, 초기 버전에는 계정 연결·복구를 지원하지 않는다.
- 재설치·기기 변경 뒤 온라인 기록을 복구할 수 없다는 제한은 첫 온라인 요청 전과 Settings에서 확인할 수 있다.
- 인증 토큰·서비스 Secret·개인정보와 임의 표시명은 앱 저장소와 제출 대기열에 저장하지 않는다.

---

## 시스템

- System은 하나의 책임만 담당한다.
- System은 독립적인 책임과 경계를 가진다.
- System의 책임은 Feature와 분리하여 관리한다.
- System 간에는 필요한 데이터만 전달한다.

---

## 기능

- 게임의 핵심 플레이는 점프와 관성 착지를 이용한 이동이다.
- 게임은 3D 오소그래픽 횡스크롤 카메라를 사용한다.
- 스테이지는 클리어 시간을 기준으로 완료를 판단한다.
- 무한 모드는 Momentum Landing 연속 성공에 따른 거리 Score 배율을 제공한다.
- Stage는 `Cleared` 결과만 불변 Stage ID와 밀리초 Clear Time으로 Leaderboard 제출 후보가 된다.
- InfiniteMode는 유효하게 확정된 Total Score와 Scoring Version으로 Leaderboard 제출 후보가 된다.
- Stage와 InfiniteMode는 규칙 Version이 다른 기록을 비교하지 않는 독립 Leaderboard를 사용한다.
- Offline·인증·서비스 실패는 플레이를 차단하지 않으며 제출 후보를 로컬 대기열에 보존한다.

---

## 기타

- 프로젝트는 1인 개발을 기준으로 진행한다.
- Unity를 사용하여 3D 게임으로 개발한다.
- AI와 협업하기 위한 문서 중심 개발 방식을 사용한다.
- 수동 검증 절차는 IDE 디버그 기능에 의존하지 않도록 구성한다.

---

# 관련 문서

## Project

- PROJECT_OVERVIEW.md
- ARCHITECTURE.md

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
