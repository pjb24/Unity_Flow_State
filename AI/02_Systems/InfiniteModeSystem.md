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
- InfiniteMode의 Momentum 상태를 소유하고 실제 착지 성공 ID를 배율 규칙에 전달한다.
- 같은 착지 성공 ID는 한 번만 반영하며, Playing에서만 Momentum 유지 시간을 진행한다.
- Pause와 Resume에 Momentum 상태 보존·재개를 연결하고 종료 시 최종 상태를 고정한다.
- Retry와 새 Run에서는 Momentum 상태와 처리한 성공 ID를 초기화한다.
- 최대 전진 거리를 이동 거리·Score 규칙과 Difficulty 상태에 전달한다.
- Player Rigidbody의 물리 X와 누적 Rebase Offset을 논리 거리 상태에 전달한다.
- InfiniteMode Playing 상태에서 World Rebase 필요 여부와 이동 Offset 계산을 조정한다.
- Rebase 전 진행 거리 반영, 대상 이동 요청, Physics 동기화와 Camera 보정 요청의 실행 순서를 조정한다.
- Player Rigidbody의 실제 양의 X 속도를 사용한다.
- CollisionSystem의 Wall 접촉 결과를 진행 지속 조건에 전달한다.
- Player Y 위치가 설정된 추락 임계값 이하인지 확인한다.
- InfiniteMode 진행 지속 조건을 평가한다.
- InfiniteMode Playing 상태에서 Player World X를 이동 거리 규칙에 전달한다.
- 이동 거리 규칙이 기록한 최대 전진 거리를 Difficulty 상태에 전달한다.
- 시작 원점, 이동 거리 및 추락 판정의 위치는 Player Rigidbody의 물리 위치를 사용한다.
- 최대 전진 거리 증가분과 현재 Momentum 배율을 Version 2 Score 상태에 전달한다.
- 현재 이동 거리, Base Distance Score, Momentum Bonus, Distance Score, Collectible Score, Total Score와 Momentum 표시 상태를 Runtime Data에 함께 반영한다.
- InfiniteMode 종료 요청 직전에 같은 Version의 최종 이동 거리, Score 구성 요소와 최고 Momentum 배율의 확정을 요청한다.
- 종료 조건 충족 시 StageSystem에 종료를 요청한다.
- Retry와 새 Run 시 이전 Run의 진행 상태, 이동 거리, Score, 최종 확정 상태, Difficulty와 Pattern 선택 상태를 초기화한다.
- GameSystem의 요청에 따라 InfiniteMode 진행 판정을 일시 중단하고 재개한다.
- 일시 중단 동안 진행 상태, 이동 거리, Score, 최종 확정 상태, Difficulty와 Pattern 선택 상태를 보존한다.

## Pattern 진행 구조와 Phase 경계

- Phase 2의 Pattern 원본 검증·네 인스턴스 캐시·활성 Pattern 교체·Anchor 연결·Boundary 위치 정렬·사용이 끝난 Slot의 안전한 재사용·Collectible Scope 해제 및 재연결은 `InfiniteMapPattern`과 두 `InfinitePatternSlot`의 책임이다.
- 현재 생산 Scene의 두 Slot은 `Flat`으로 시작한다. `InfiniteMapPattern.TryRequestNextPattern(requestId, patternId)`은 명시적 Pattern ID만 받고, 중복·역행 요청과 연결할 수 없는 Pattern을 거부한다. 앞 Slot의 Boundary가 사용이 끝난 뒤 Slot을 재배치하며 Retry에서 두 Slot과 요청 상태를 초기화한다.
- `InfiniteModeSystem`은 최대 전진 거리와 선택 상태를 연결하고 성공한 선택 ID를 위 요청 API로 전달한다. 요청 거부 시 선택 상태를 되돌리고 Boundary 진행 후 다음 요청을 준비한다.
- Pattern별 Collectible 배치·안내 경로·UI는 Phase 4 책임이다. Phase 2의 빈 Collectible Root에서도 Scope 생명주기는 유지한다.

---

# 시작 조건

- GameSystem이 선택된 게임 Mode와 함께 초기화를 요청한다.
- Player Movement Runtime Data가 생성되었다.
- StageSystem이 초기화되었다.
- Player 참조 대상에 Rigidbody가 존재한다.
- CollisionSystem 참조가 존재한다.

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
| 실제 양의 X 이동 속도 | Player Rigidbody |
| Wall 접촉 상태 | CollisionSystem |
| Player Y 위치 | Player Rigidbody의 물리 위치 |
| Player World X | Player Rigidbody의 물리 위치 |
| 누적 Rebase Offset과 Rebase 실행 상태 | World Rebase 상태 |
| 최대 전진 거리 | InfiniteDistanceState |
| 현재 게임 Mode | GameSystem |
| InfiniteMode 진행 중단 및 재개 요청 | GameSystem |
| Pattern Catalog | InfinitePatternCatalogFactory |
| Pattern 진행 횟수와 현재 Pattern | InfiniteMapPattern |
| Run별 Momentum Landing 성공 ID | Player Movement Runtime Data |
| 현재 Run의 게임 상태 | Game Runtime Data |

---

# 출력

| 출력 | 대상 |
|------|------|
| InfiniteMode Stage 종료 요청 | StageSystem |
| 현재 이동 거리, Score 구성 요소, 합계와 Momentum 표시 상태 | Runtime Data |
| 선택된 다음 Pattern ID와 요청 ID | InfiniteMapPattern |
| Player Rebase 요청 | PlayerControllerSystem |
| Pattern과 Boundary Rebase 요청 | InfiniteMapPattern |
| Camera Rebase 및 Target Warp 요청 | CameraSystem |

---

# Phase 3 Difficulty와 Pattern 선택 상태 생명주기

- InfiniteModeSystem이 최대 전진 거리, 현재 Difficulty, 현재 Pattern ID, 연속 반복 횟수, 마지막 처리 요청 ID와 난수 상태를 한 Run의 상태로 소유한다.
- 새 Run과 Retry 시작 시 최대 전진 거리와 Difficulty를 초기화하고 새로운 Pattern 난수 Seed로 첫 `Flat`과 선택 이력을 생성한다.
- Pattern 진행 요청을 처리할 때 현재 Difficulty와 요청 ID를 선택 규칙에 전달하고 성공한 다음 Pattern ID만 진행 구조에 전달한다.
- 직전에 처리한 요청과 같은 ID는 중복 요청으로 거부한다.
- Pause에서는 Difficulty와 Pattern 선택 상태를 보존하고 요청을 처리하지 않으며 Resume 후 보존된 상태에서 계속한다.
- Result에서는 추가 진행 요청을 거부하고 다음 Run이 시작될 때 상태를 초기화한다.

---

# 시스템 경계

## 담당 범위

- InfiniteMode 진행 상태 관리
- Momentum 상태의 생산 생명주기와 착지 성공 전달 관리
- Phase 3 Difficulty와 Pattern 선택 상태의 생명주기 관리
- Phase 3 Pattern 선택 규칙 실행 연결
- InfiniteMode 설정 관리
- 이동 결과와 추락 임계값의 규칙 평가 연결
- Player 위치와 이동 거리 규칙의 연결
- 누적 논리 거리와 World Rebase 상태의 생명주기 관리
- Rebase 대상 System 요청과 원자적 실행 순서 조정
- Phase 3 최대 전진 거리와 Difficulty 전환 규칙의 연결
- 현재 이동 거리, Score 구성 요소와 Momentum 표시 상태의 Runtime Data 반영
- 종료 요청 전 동일 Version의 최종 이동 거리·Score·최고 배율 확정 요청
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
- Camera 위치 이동과 Cinemachine Warp 직접 처리
- 이동 거리 또는 Score 계산 규칙 정의
- Momentum 배율 단계·유지 시간 규칙 정의
- Difficulty 전환, Pattern 후보, 반복과 대체 후보 규칙 정의
- Pattern 지형 제작과 배치

---

# 관련 System

- GameSystem
- PlayerMovementSystem
- RuntimeDataSystem
- StageSystem
- PlayerControllerSystem
- CameraSystem

---

# 제약 사항

- PlayerMovementSystem의 이동 계산을 변경하지 않는다.
- 진행 속도는 `max(0, Rigidbody.linearVelocity.x)`를 사용한다.
- Wall 접촉은 CollisionSystem 결과를 사용하고 별도 물리 판정을 만들지 않는다.
- 위치 판정은 보간된 표시 Transform 대신 Rigidbody.position을 사용한다. Rigidbody 위치나 속도를 직접 변경하지 않고 PlayerControllerSystem에 Rebase를 요청한다.
- 추락 판정은 Player의 X 위치와 관계없이 Y 임계값으로 수행한다.
- Stage 종료는 StageSystem에 요청한다.
- 정상 프레임마다 로그를 출력하지 않는다.
- 이동 거리와 Score 계산 규칙을 직접 구현하지 않는다.
- 새 생산 Infinite Run은 Scoring Version `2`만 사용하고 Runtime·Score 상태의 Version이 다르면 초기화 또는 갱신하지 않는다.
- Base Distance Score와 Momentum Bonus는 `InfiniteScoreState`의 결과만 Runtime Data에 전달한다.
- Collectible Score는 공통 Collectible Runtime Data에서 읽고 Momentum 배율을 적용하지 않는다.
- 종료 확정 후 Score와 Runtime Data를 다시 갱신하지 않는다.
- Result Data를 생성하지 않는다.
- 일시 중단 동안 이동 거리, Score, 저속 진행 시간과 추락 종료 판정을 갱신하지 않는다.
- Momentum 배율은 InfiniteMode Playing에서만 갱신한다. Stage Mode에서는 배율을 시작하거나 진행하지 않는다.
- 일반 착지와 Wall 접촉을 Momentum 초기화 사유로 사용하지 않는다.
- 종료 시 고정한 Momentum 상태는 다음 Run 초기화 전까지 보존한다.
- 같은 Runtime Data로 실행 중인 System의 중복 초기화는 Momentum·Pause 상태와 성공 처리 이력을 보존한다.
- 재개 시 일시 중단 이전의 Run 기록과 진행 상태를 유지한다.
- Pause와 Result에서는 Difficulty 및 Pattern 선택 상태를 변경하지 않는다.
- Pattern 선택 결과는 Distance Score, Collectible Score와 Total Score 계산에 전달하지 않는다.
- World Rebase는 InfiniteMode Playing에서만 조정하고 Pause, Result와 Stage Mode에서는 요청하지 않는다.
- Rebase 전후 같은 누적 논리 거리를 Difficulty와 Score에 전달한다.
- Retry와 새 Run에서 누적 Rebase Offset과 Rebase 실행 상태를 초기화한다.

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
