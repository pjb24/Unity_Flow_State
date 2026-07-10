# 시스템 개요

## 시스템명

TimerSystem

---

## 목적

게임에서 사용하는 Timer를 관리한다.

Timer의 실행 상태를 관리하고 측정된 시간을 필요한 System에 제공한다.

현재 프로젝트에서는 기본적으로 하나의 활성 Timer를 사용한다.

추후 버프 지속시간, 체크포인트 구간 기록, 튜토리얼 대기 시간, 리플레이 타임라인, 행동 쿨타임 등을 위해 여러 Timer를 관리할 수 있도록 확장 가능성을 유지한다.

---

# 시스템 책임

- Timer를 생성한다.
- Timer를 시작한다.
- Timer를 일시정지한다.
- Timer를 다시 시작한다.
- Timer를 종료한다.
- Timer를 제거한다.
- Timer의 실행 시간을 측정한다.
- Timer 상태를 관리한다.
- Timer Key를 기준으로 Timer를 구분한다.
- 측정된 시간을 필요한 System에 제공한다.

---

# Timer Key

Timer Key는 Timer를 구분하기 위한 식별자이다.

TimerSystem은 Timer Key를 기준으로 Timer를 생성하고 관리한다.

현재 프로젝트에서는 Stage Mode와 Infinite Mode가 동시에 진행되지 않으므로 기본 플레이 시간 측정에는 하나의 활성 Timer만 사용한다.

추후 여러 시간 측정이 동시에 필요해지는 경우 Timer Key를 추가하여 여러 Timer를 관리한다.

예를 들어 아래 항목이 Timer Key가 될 수 있다.

- PlayTimer
- CheckpointTimer
- BuffTimer
- TutorialTimer
- ReplayTimer
- CooldownTimer

TimerSystem은 Timer Key의 사용 목적을 판단하지 않는다.

Timer Key를 어떤 목적으로 사용할지는 Timer를 요청하는 System 또는 Feature가 결정한다.

---

# 시작 조건

- Timer 생성 요청을 수신한다.
- Timer 시작 요청을 수신한다.

---

# 종료 조건

## 정상 종료

- Timer 종료 요청을 수신한다.
- Timer 제거 요청을 수신한다.

## 강제 종료

- GameSystem이 게임 종료 절차를 시작한다.

---

# 관리 대상

- Timer Key
- 현재 활성 Timer
- 생성된 Timer 목록
- Timer 실행 상태
- Timer 시작 시각
- 현재 측정 시간
- Timer 일시정지 시각
- Timer 누적 일시정지 시간
- Timer 종료 시각
- 최종 측정 시간

---

# 입력

| 입력 | 출처 |
|------|------|
| Timer 생성 요청 | Timer 사용자 |
| Timer 시작 요청 | Timer 사용자 |
| Timer 일시정지 요청 | Timer 사용자 |
| Timer 재시작 요청 | Timer 사용자 |
| Timer 종료 요청 | Timer 사용자 |
| Timer 제거 요청 | Timer 사용자 |

---

# 출력

| 출력 | 대상 |
|------|------|
| 현재 측정 시간 | Timer 사용자 |
| 최종 측정 시간 | Timer 사용자 |
| Timer 실행 상태 | Timer 사용자 |

---

# Timer 사용자

Timer 사용자는 TimerSystem에 Timer 생성, 시작, 일시정지, 재시작, 종료, 제거를 요청하는 System을 의미한다.

예를 들어 아래 System이 Timer 사용자가 될 수 있다.

- StageSystem
- GameSystem
- ResultSystem
- UIManagementSystem

추후 아래 System이 추가될 경우 Timer 사용자가 될 수 있다.

- ReplaySystem
- TutorialSystem
- BuffSystem
- CooldownSystem

TimerSystem은 Timer 사용자의 내부 동작을 관리하지 않는다.

TimerSystem은 Timer 사용자가 요청한 Timer 상태만 관리한다.

---

# 시스템 경계

## 담당 범위

- Timer 생성
- Timer 시작
- Timer 일시정지
- Timer 재시작
- Timer 종료
- Timer 제거
- Timer Key 관리
- Timer 실행 상태 관리
- Timer 시간 측정
- Timer 시간 제공

---

## 담당하지 않는 범위

- Stage 시작
- Stage 종료
- 게임 일시정지 처리
- 버프 효과 처리
- 체크포인트 기록 판정
- 튜토리얼 진행
- 리플레이 재생
- 행동 쿨타임 사용 가능 여부 판단
- 결과 데이터 생성
- 기록 저장
- 시간 UI 표시
- 게임 전체 흐름 관리
- Feature 규칙 정의

---

# 관련 System

- GameSystem
- StageSystem
- ResultSystem
- UIManagementSystem

---

# 제약 사항

- Timer의 사용 목적을 판단하지 않는다.
- Timer의 실행 시간을 평가하지 않는다.
- 기록을 생성하지 않는다.
- Timer 데이터를 저장하지 않는다.
- Feature 규칙을 정의하지 않는다.
- Timer는 요청을 받은 경우에만 상태를 변경한다.
- 일시정지 상태의 Timer는 측정 시간을 증가시키지 않는다.
- 동일한 Timer Key로 중복 Timer를 생성하지 않는다.
- 존재하지 않는 Timer Key에 대한 요청은 처리하지 않는다.
- 폴백이 필요한 경우 Warning 로그로 폴백 발생을 명확히 기록한다.

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