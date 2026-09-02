# 시스템 개요

## 시스템명

InfiniteModeSystem

---

# 목적

InfiniteMode의 진행 지속 상태를 관리한다.

Player 이동 결과와 현재 Y 위치를 InfiniteMode 규칙에 전달한다.

InfiniteMode 종료 시 StageSystem에 Stage 종료를 요청한다.

---

# 시스템 책임

- InfiniteMode 진행 상태를 초기화하고 종료한다.
- Player Movement Runtime Data의 현재 수평 속도를 사용한다.
- Player Y 위치가 설정된 추락 임계값 이하인지 확인한다.
- InfiniteMode 진행 지속 조건을 평가한다.
- InfiniteMode Playing 상태에서 Player World X를 이동 거리 규칙에 전달한다.
- 현재 이동 거리와 현재 Score를 Runtime Data에 반영한다.
- InfiniteMode 종료 요청 직전에 최종 이동 거리와 최종 Score의 확정을 요청한다.
- 종료 조건 충족 시 StageSystem에 종료를 요청한다.
- Retry 시 이전 Run의 진행 상태, 이동 거리, Score와 최종 확정 상태를 초기화한다.
- GameSystem의 요청에 따라 InfiniteMode 진행 판정을 일시 중단하고 재개한다.
- 일시 중단 동안 진행 상태, 이동 거리, Score와 최종 확정 상태를 보존한다.

---

# 시작 조건

- GameSystem이 선택된 게임 Mode와 함께 초기화를 요청한다.
- Player Movement Runtime Data가 생성되었다.
- StageSystem이 초기화되었다.

---

# 종료 조건

## 정상 종료

- InfiniteMode 종료 조건이 충족되었다.
- GameSystem이 게임 종료 절차를 시작한다.

## 강제 종료

- Unity가 게임을 종료한다.

---

# 입력

| 입력 | 출처 |
|------|------|
| 현재 수평 이동 속도 | Player Movement Runtime Data |
| Player Y 위치 | Player Transform |
| Player World X | Player Transform |
| 현재 게임 Mode | GameSystem |
| InfiniteMode 진행 중단 및 재개 요청 | GameSystem |

---

# 출력

| 출력 | 대상 |
|------|------|
| InfiniteMode Stage 종료 요청 | StageSystem |
| 현재 이동 거리와 현재 Score | Runtime Data |

---

# 시스템 경계

## 담당 범위

- InfiniteMode 진행 상태 관리
- InfiniteMode 설정 관리
- 이동 결과와 추락 임계값의 규칙 평가 연결
- Player 위치와 이동 거리 규칙의 연결
- 현재 이동 거리와 현재 Score의 Runtime Data 반영
- 종료 요청 전 최종 이동 거리와 최종 Score 확정 요청
- InfiniteMode Stage 종료 요청
- InfiniteMode 진행 판정 중단 및 재개

## 담당하지 않는 범위

- Player 이동 계산
- Rigidbody 또는 Transform 제어
- Player Transform 제어
- Stage 종료 이벤트 발생
- 게임 전체 종료 흐름
- Result Data 생성
- UI 표시
- 이동 거리 또는 Score 계산 규칙 정의

---

# 관련 System

- GameSystem
- PlayerMovementSystem
- RuntimeDataSystem
- StageSystem

---

# 제약 사항

- PlayerMovementSystem의 이동 계산을 변경하지 않는다.
- Player Movement Runtime Data가 제공하는 수평 속도를 사용한다.
- 추락 판정은 Player의 X 위치와 관계없이 Y 임계값으로 수행한다.
- Stage 종료는 StageSystem에 요청한다.
- 정상 프레임마다 로그를 출력하지 않는다.
- 이동 거리와 Score 계산 규칙을 직접 구현하지 않는다.
- Result Data를 생성하지 않는다.
- 일시 중단 동안 이동 거리, Score, 저속 진행 시간과 추락 종료 판정을 갱신하지 않는다.
- 재개 시 일시 중단 이전의 Run 기록과 진행 상태를 유지한다.

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
