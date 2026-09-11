# 기능 개요

## 기능명

InfiniteMode

---

## 목적

플레이어가 종료 조건에 도달할 때까지 Stage를 계속 진행할 수 있도록 한다.

속도를 유지하는 플레이를 통해 높은 점수를 획득하는 플레이 경험을 제공한다.

---

# 기능 규칙

- InfiniteMode는 Goal을 사용하지 않는다.
- InfiniteMode는 Stage Play가 시작되면 수행한다.
- InfiniteMode는 `Flat`, `SingleRise`, `LegacySteps`, `InternalGap` 네 종류의 Map Pattern을 사용한다.
- 첫 Pattern은 `Flat`을 사용하고 이후 Pattern은 현재 Difficulty의 허용 후보에서 선택한다.
- Map Pattern은 Player가 현재 이용 중인 지형을 잃지 않도록 다음 진행 구간을 먼저 제공한다.
- Player가 접촉 중이거나 아직 완전히 지나가지 않은 Map Pattern은 재배치하지 않는다.
- 플레이어의 점수는 프로젝트에서 정의한 점수 규칙에 따라 증가한다.
- 플레이어는 진행 지속 조건을 유지하는 동안 InfiniteMode를 계속 수행한다.
- 플레이어가 진행 지속 조건을 잃으면 InfiniteMode를 종료한다.

---

# Map Pattern 규칙

## Pattern 목록

| ID | 목적 | 최초 허용 Difficulty |
|---|---|---|
| `Flat` | Run 시작, 자동 이동 가속과 대체 후보를 위한 평탄 구간을 제공한다. | D1 |
| `SingleRise` | 한 번의 상승 Jump와 넓은 착지 구간을 제공한다. | D1 |
| `LegacySteps` | 기존 두 Platform을 사용하는 연속 Jump와 착지 흐름을 유지한다. | D2 |
| `InternalGap` | Pattern 내부 Gap에서 Jump 시점과 수평 이동을 판단하게 한다. | D3 |

모든 Pattern의 StartAnchor와 EndAnchor 사이 X 길이는 `44`이다.

모든 Pattern은 새로운 장애물, 이동 Platform 또는 별도의 특수 이동 규칙을 요구하지 않는다.

## Pattern별 계약

### Flat

- 내부 지형은 높이 변화와 Gap이 없는 평탄 Ground를 사용한다.
- 첫 Pattern에서는 Jump 입력 없이 정지 상태부터 자동 가속하여 진행할 수 있어야 한다.
- 다른 Pattern과 연결되는 경계 Gap에서는 일반 Jump를 사용한다.
- 모든 Difficulty에서 시작 Pattern과 대체 후보로 사용할 수 있다.
- 실제 정지 출발과 경계 Gap 통과는 Phase 2 Play Mode Test로 검증한다.

### SingleRise

- 한 번의 상승 Platform 진입과 이후의 평탄한 이탈 구간을 사용한다.
- 일반 Jump 한 번으로 상승하고 Momentum Landing 없이 착지할 수 있어야 한다.
- 이륙 Ground와 착지면은 기본 속도와 최대 속도에서 최소 `0.10`초의 유효 입력 구간을 제공해야 한다.
- 상승 높이와 착지면 길이는 `InfinitePatternTraversalMath` 계약을 만족해야 한다.
- 실제 Platform 전면 충돌, 모서리 고정과 착지는 Phase 2 Play Mode Test로 검증한다.

### LegacySteps

- 기존 Pattern의 두 고정 Platform과 두 번의 Jump 및 착지 흐름을 유지한다.
- 새로운 장애물, 이동 Platform 또는 특수 이동 규칙을 추가하지 않는다.
- 각 Platform 진입과 이탈을 독립된 통과 구간으로 검사해야 한다.
- 현재 생산 Pattern을 새 공통 계약에 맞춘 뒤 전체 구간 통과를 Phase 2 Play Mode Test로 검증한다.

### InternalGap

- 같은 높이의 Ground 사이에 Pattern 내부 Gap 하나를 사용한다.
- 일반 Jump 한 번으로 Gap을 통과할 수 있어야 한다.
- Gap 전 이륙 Ground와 Gap 후 착지면은 기본 속도와 최대 속도에서 최소 `0.10`초의 유효 입력 구간을 제공해야 한다.
- 내부 Gap의 실제 길이와 착지면 길이는 Phase 2 제작 시 `InfinitePatternTraversalMath` 계약으로 확정한다.
- 실제 Gap 통과와 낙하 여부는 Phase 2 Play Mode Test로 검증한다.

모든 Pattern은 `Flat`, `SingleRise`, `LegacySteps`, `InternalGap` 중 어느 Pattern 뒤에도 연결할 수 있어야 한다.

공통 연결 계약이나 일반 Jump 통과 계약을 만족하지 못하거나 새로운 장애물, 이동 Platform 또는 특수 이동 규칙을 요구하는 구현은 Prototype 4 목록에서 제외한다.

## Pattern 연결 규칙

- 진행 방향은 World +X이다.
- StartAnchor와 EndAnchor는 같은 높이와 +X 접선을 사용한다.
- 연결 시 다음 Pattern의 StartAnchor를 이전 Pattern의 EndAnchor 위치에 맞춘다.
- Anchor의 축별 위치 오차는 `0.01` 이하이다.
- Anchor의 접선 방향 오차는 `0.1`도 이하이다.
- 연결 Ground 상단의 높이 차이는 `0.01` 이하이다.
- 연결 지형의 Z 폭은 `4`이다.
- Pattern 경계에는 X 길이 `4`의 Ground Gap을 허용한다.
- 경계 Gap은 앞 Pattern의 출구 상태와 뒤 Pattern의 착지면을 함께 사용하여 통과 가능 여부를 판정한다.
- 연결 및 통과 조건을 만족하지 않는 Pattern 조합은 선택하지 않는다.

## 통과 가능 규칙

- 모든 Pattern과 허용된 연결 조합은 현재 Player 이동 수치를 변경하지 않고 일반 Jump로 통과할 수 있어야 한다.
- Momentum Landing은 통과의 필수 조건으로 사용하지 않는다.
- Pattern은 기본 수평 속도 `8`과 최대 수평 속도 `14`의 진입 상태를 모두 고려한다.
- 첫 `Flat` Pattern은 정지 상태에서 자동 가속하여 진행할 수 있어야 한다.
- Jump가 필요한 구간은 유효한 Jump 입력 구간을 최소 `0.10`초 제공해야 한다.
- Player 통과 계산은 Jump 높이 `3`, 중력 가속도 `25`, Capsule 반지름 `0.5`와 높이 `2`를 사용한다.
- Ground 연속 구간은 수평 간격과 높이 차이가 각각 `0.01` 이하일 때 연속으로 판정한다.
- Gap, 상승 및 하강 구간은 이륙 전 Ground 길이, Gap 길이, 착지 높이와 착지면 길이를 함께 판정한다.
- 착지면 길이는 Player Capsule 지름 `1` 이상이어야 한다.
- 기본 속도와 최대 속도에서 계산한 유효 Jump 입력 구간이 모두 `0.10`초 이상이어야 한다.
- Platform 진입과 이탈은 각각 별도의 Jump 구간으로 판정한다.
- 진행 경로에 수직 장애물이 있으면 Jump 궤적에서 Player 하단이 장애물 상단을 통과할 수 있어야 한다.
- 통과 계산을 만족하지 않는 Gap과 착지면은 낙하 위험 구간으로 판정하고 허용하지 않는다.
- 기하 조건은 정적 검사와 Edit Mode Test로 판정하고 실제 Rigidbody 통과 결과는 Play Mode Test로 판정한다.
- Collider 모서리 접촉, Wall 고정, Ground 판정 전환과 실제 착지 성공은 Play Mode Test로 판정한다.

---

# Difficulty와 Pattern 선택 규칙

## Difficulty 전환

| Difficulty | 최대 전진 거리 | 허용 Pattern |
|---|---|---|
| D1 | `0` 이상 `220` 미만 | `Flat`, `SingleRise` |
| D2 | `220` 이상 `440` 미만 | D1 후보와 `LegacySteps` |
| D3 | `440` 이상 | D2 후보와 `InternalGap` |

- 새 Run은 D1에서 시작한다.
- Difficulty 진행도 입력은 Run 원점부터 기록한 최대 전진 거리이다.
- 경계값에 도달하면 높은 Difficulty를 적용한다.
- 진행도가 여러 경계를 한 번에 넘으면 해당하는 Difficulty를 바로 적용한다.
- Difficulty는 Run 중 역행하지 않으며 D3 이후에는 D3를 유지한다.
- 변경된 Difficulty는 다음 Pattern 선택부터 적용한다.
- Pause와 Result에서는 Difficulty와 Pattern 선택 상태를 변경하지 않는다.
- Retry와 새 Run에서는 최대 전진 거리를 `0`으로 초기화하고 D1에서 다시 시작한다.
- 음수, `NaN`과 무한대 진행도는 거부하고 현재 Difficulty와 진행도를 유지한다.
- Difficulty 진행도는 Distance Score, Collectible Score와 Total Score에 포함하지 않는다.
- Pattern 통과 개수와 Pattern 재배치 횟수는 Difficulty 진행도에 사용하지 않는다.

## 후보 선택

- 후보는 현재 Difficulty, 이전 Pattern과의 연결 및 통과 가능 조건, 동일 Pattern 반복 제한 순서로 걸러낸다.
- 남은 후보 중 하나를 균등 무작위로 선택한다.
- 같은 후보 목록, 난수 원본과 선택 상태에서는 같은 결과를 재현할 수 있어야 한다.
- 같은 Pattern ID는 최대 두 번까지 연속으로 선택할 수 있다.
- 반복 제한은 재사용 인스턴스가 아닌 Pattern ID를 기준으로 적용한다.
- 일반 후보가 없으면 연결 및 통과 조건을 만족하는 `Flat`을 대체 후보로 사용한다.
- `Flat` 대체 시에만 동일 Pattern 반복 제한을 완화한다.
- `Flat`도 연결할 수 없는 설정은 실행 전에 거부한다.

## Run 초기화

- 첫 Pattern은 `Flat`이며 Pattern 선택 이력에 포함한다.
- 첫 Pattern 다음부터 D1 후보 선택 규칙을 적용한다.
- Retry와 새 Run은 각각 새로운 난수 Seed를 사용한다.
- Retry와 새 Run에서 진행도, Difficulty, 선택 이력, 연속 반복 횟수와 중복 진행 요청 상태를 초기화한다.
- Pattern 배치, Boundary와 Pattern별 Collectible Scope를 초기화한다.
- Pause와 Resume에서는 선택 이력과 난수 상태를 유지한다.
- Result에서는 Pattern 선택과 진행 요청을 거부한다.
- 직전에 처리한 Pattern 진행 요청과 같은 ID는 중복으로 거부하고 선택 이력과 난수 상태를 변경하지 않는다.
- 같은 Pattern Catalog, Seed, Difficulty와 요청 순서는 같은 Pattern 선택 순서를 만든다.

## Phase 2 Scene 정적 검사 기준

- Pattern ID와 목적이 비어 있지 않고 전체 목록에서 ID가 중복되지 않아야 한다.
- Pattern의 최초 허용 Difficulty가 D1, D2 또는 D3 중 하나여야 한다.
- StartAnchor에서 EndAnchor까지의 X 길이가 `44 ± 0.01`이어야 한다.
- StartAnchor와 EndAnchor의 Y 및 Z 차이가 각각 `0.01` 이하여야 한다.
- 두 Anchor의 접선이 +X 방향에서 각각 `0.1`도 이내여야 한다.
- 연결부 Ground의 Z 폭이 `4 ± 0.01`이어야 한다.
- 앞 Pattern의 종료 Ground Inset과 다음 Pattern의 시작 Ground Inset 합이 `4 ± 0.01`이어야 한다.
- 연결 Ground 상단 높이 차이가 `0.01` 이하여야 한다.
- Ground와 Platform Collider는 Ground Layer의 비 Trigger Collider여야 한다.
- AdvanceBoundary는 Player가 Pattern을 완전히 지나기 전에 후행 Pattern을 재배치하지 않는 위치와 Trigger 구성을 사용해야 한다.
- 모든 허용 Pattern 조합과 `Flat` 대체 조합이 연결 계약을 만족해야 한다.
- Scene 수치가 정적 계약을 만족한 뒤 실제 Gap 통과와 Rigidbody 결과를 Play Mode Test로 검증해야 한다.

## Phase 2 Pattern별 Test 대상

- `Flat`: 정지 출발 자동 가속, 평탄 Ground 진행과 모든 Pattern 경계 Gap 통과
- `SingleRise`: 기본 및 최대 속도의 상승 Platform 진입, 착지와 이탈
- `LegacySteps`: 기존 두 Platform의 각 진입·착지·이탈과 앞뒤 Pattern 연결
- `InternalGap`: 기본 및 최대 속도의 내부 Gap 통과, 착지와 낙하 방지
- 전체 조합: 앞 Pattern EndAnchor에서 뒤 Pattern StartAnchor까지 16개 조합의 연결 및 통과
- 공통 물리: Platform 전면과 모서리의 Wall 접촉 해제, Ground 상태 복구와 추락 임계값 도달

## Phase별 검증 책임

- Phase 1은 Pattern ID와 개수, 연결 수치, 통과 계산 경계, Difficulty 경계, 후보 순서, 반복·대체·초기화 계약을 정적 검사와 Edit Mode Unit Test로 확정한다.
- Phase 2는 생산 Scene의 네 Pattern 제작과 Anchor·Collider·Layer·Boundary 구성을 정적으로 검사하고, 각 Pattern 및 16개 연결 조합의 실제 Rigidbody 통과를 Play Mode Test로 검증한다.
- Phase 3은 InfiniteModeSystem과 진행 구조에 Difficulty 및 Pattern 선택 상태를 연결하고, Retry·새 Run·Pause·Result 생명주기를 Edit Mode 상태 Test와 Play Mode 통합 Test로 검증한다.
- Phase 4는 Pattern별 Collectible 경로, Difficulty 및 Pattern UI, Score 유지와 Stage Mode를 포함한 전체 회귀를 Play Mode Test로 검증하고 화면 표현만 수동으로 확인한다.

---

# 이동 거리 규칙

- 이동 거리는 Run 시작 시점의 Player World X를 원점으로 사용한다.
- 시작 원점과 진행 위치는 같은 물리 위치 기준을 사용하며 표시 보간 위치를 거리 또는 추락 판정에 사용하지 않는다.
- 이동 거리는 원점부터 Player가 도달한 최대 World X까지의 전진 거리로 계산한다.
- Player가 뒤로 이동해도 이미 기록된 이동 거리는 감소하지 않는다.
- 점프 중 수평 이동과 공중 이동을 이동 거리에 포함한다.
- Map Pattern의 위치, 통과 개수와 재배치 횟수는 이동 거리 계산에 사용하지 않는다.
- 이동 거리는 내부에서 `float`으로 관리하고 계산 과정에서 반올림하지 않는다.
- 이동 거리는 InfiniteMode Playing 상태의 물리 갱신마다 갱신한다.
- InfiniteMode 종료 요청 직전에 최종 이동 거리를 확정한다.
- 최종 확정된 이동 거리는 해당 Run이 종료될 때까지 변경하지 않는다.

---

# Score 규칙

이 절의 Score는 기존 이동 거리 기반 Score를 의미한다. Collectible 획득 점수 규칙은 `ScoreCollectible.md`에서 관리한다. Prototype 3 Phase 3에서는 두 점수를 분리하며 기존 HUD와 Result의 Score는 거리 Score를 유지한다. Total Score 계산 및 표시는 Phase 4에서 통합한다.

- Score는 이동 거리에 Score 환산 비율을 곱한 값을 내림하여 계산한다.
- Score 환산 비율의 Prototype 2 초기값은 World X 거리 1당 10점이다.
- Score 환산 비율은 하나의 설정 값으로 관리한다.
- Score는 `int`로 관리한다.
- Score의 최솟값은 `0`, 최댓값은 `int.MaxValue`이다.
- 이동 거리 외의 진행 지표는 Score 계산에 사용하지 않는다.
- 최종 Score는 InfiniteMode 종료 요청 직전에 최종 이동 거리를 기준으로 확정한다.
- 최종 확정된 Score는 해당 Run이 종료될 때까지 변경하지 않는다.

---

# 진행 지속 조건

플레이어는 아래 조건을 모두 만족하는 동안 진행을 계속할 수 있다.

- 시작 유예 시간이 지난 후 실제 양의 X 이동 속도가 최소 이동 속도 `2` 이상으로 유지된다.
- 최소 이동 속도 미만인 상태가 프로젝트 설정 값으로 정의한 연속 유예 시간보다 짧다.
- 플레이어의 Y 위치가 프로젝트 설정으로 정의한 추락 임계값보다 높다.

시작 유예 시간은 최소 이동 속도 판정에만 적용한다.

시작 유예 시간은 `1`초이며 이 동안 저속 또는 Wall 유예 시간을 소비하지 않는다.

저속 연속 유예 시간은 `0.5`초다. Wall 접촉 중 저속이면 Run당 최대 `1`초의 Wall 추가 유예를 먼저 소비하고 남은 시간부터 저속 유예에 누적한다.

Wall 접촉이 끊겼다가 다시 시작되어도 사용한 Wall 추가 유예는 복구되지 않는다. 실제 양의 X 이동 속도가 최소 속도 이상으로 회복되면 Wall 추가 유예 사용량과 저속 누적 시간을 모두 초기화한다.

추락 판정은 InfiniteMode Stage Play가 시작되면 즉시 적용한다.

---

# 시작 조건

다음 조건을 모두 만족하는 경우 InfiniteMode를 시작한다.

- InfiniteMode Stage가 선택되었다.
- Stage Play가 시작되었다.

---

# 종료 조건

## 정상 종료

아래 조건 중 하나를 만족하면 InfiniteMode를 종료한다.

- 시작 및 적용 가능한 Wall 유예가 지난 후 실제 양의 X 이동 속도가 최소 이동 속도 미만인 상태로 저속 유예 시간 이상 유지되었다.
- 플레이어의 Y 위치가 프로젝트 설정으로 정의한 추락 임계값 이하가 되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- InfiniteMode Stage Play가 종료된다.
- ResultMenu가 활성화된다.
- 최종 이동 거리와 최종 Score가 확정된다.
- Playing 동안 현재 이동 거리와 현재 Score가 InfiniteHUD에 표시된다.
- Ending에서는 InfiniteHUD가 사라지지 않고 마지막 표시값을 유지한다.
- Result와 Ended에서는 InfiniteHUD를 유지하고 최종 이동 거리와 최종 Score를 ResultPanel에 표시한다.
- HUD는 `Distance: 12`, `Score: 123` 형식을 사용한다.
- Result는 `Final Distance: 12`, `Final Score: 123` 형식을 사용한다.
- 표시 거리는 원본 값을 변경하지 않고 소수점 없이 내림 처리한다.

---

# 예외 사항

- 일반 Stage에서는 InfiniteMode를 수행하지 않는다.
- Stage Play가 진행 중이 아니면 수행하지 않는다.
- 게임이 종료된 이후에는 수행하지 않는다.
- 시작 유예 시간 동안에도 추락 판정은 중단하지 않는다.

---

# 관련 System

- StageSystem
- InfiniteModeSystem
- PlayerMovementSystem
- ResultSystem

---

# 제약 사항

- InfiniteMode는 Goal을 사용하지 않는다.
- InfiniteMode는 하나의 Stage Play 동안만 수행한다.
- InfiniteMode는 확정된 네 종류의 Map Pattern만 사용한다.
- InfiniteMode는 진행 지속 조건을 만족하는 동안 계속 수행한다.
- 최소 이동 속도는 프로젝트 설정 값으로 정의한다.
- 최소 이동 속도 판정에는 실제 양의 X 이동 속도를 사용한다.
- 시작 유예 시간과 최소 이동 속도 미만 연속 유예 시간은 프로젝트 설정 값으로 정의한다.
- 점수는 프로젝트에서 정의한 점수 규칙에 따라 증가한다.
- InfiniteMode 종료 후에는 Stage Play가 종료된다.
- 점수 기록은 ScoreRecord Feature에서 수행한다.
- Retry 시 이전 Run의 이동 거리, Score와 최종 확정 상태를 유지하지 않는다.

---

# 검증 항목

- InfiniteMode Stage 선택 시 InfiniteMode가 시작되는지 확인한다.
- InfiniteMode 동안 Goal이 사용되지 않는지 확인한다.
- Difficulty와 연결 및 반복 규칙에 따라 네 종류의 Map Pattern을 선택하며 Stage를 계속 진행할 수 있는지 확인한다.
- Player가 접촉 중이거나 아직 완전히 지나가지 않은 Map Pattern이 재배치되지 않는지 확인한다.
- 프로젝트에서 정의한 점수 규칙에 따라 점수가 증가하는지 확인한다.
- 이동 거리가 Run 시작 시점의 Player World X부터 최대 전진 거리로 계산되는지 확인한다.
- Player가 뒤로 이동해도 이동 거리와 Score가 감소하지 않는지 확인한다.
- 점프 중 수평 이동과 공중 이동이 이동 거리에 포함되는지 확인한다.
- Map Pattern의 위치, 통과 개수와 재배치 횟수가 이동 거리와 Score에 반영되지 않는지 확인한다.
- 이동 거리 1당 10점의 비율과 내림 규칙으로 Score가 계산되는지 확인한다.
- 수평 진행축 이동 속도의 절댓값이 최소 이동 속도 이상인 동안 Stage Play가 계속 진행되는지 확인한다.
- 최소 이동 속도 미만인 상태가 연속 유예 시간보다 짧으면 Stage Play가 계속 진행되는지 확인한다.
- 시작 유예 시간이 지난 후 최소 이동 속도 미만인 상태가 연속 유예 시간 이상 유지되면 InfiniteMode가 종료되는지 확인한다.
- 시작 유예 시간 동안에는 최소 이동 속도로 인해 종료되지 않는지 확인한다.
- 시작 유예 시간 동안 추락 임계값 이하가 되면 InfiniteMode가 종료되는지 확인한다.
- 플레이어의 X 위치와 관계없이 Y 위치가 추락 임계값 이하가 되면 InfiniteMode가 종료되는지 확인한다.
- 최소 이동 속도 설정 값을 변경하면 종료 기준이 함께 변경되는지 확인한다.
- InfiniteMode 종료 요청 직전에 최종 이동 거리와 최종 Score가 한 번 확정되는지 확인한다.
- 현재 이동 거리와 현재 Score가 InfiniteHUD에 표시되는지 확인한다.
- Ending에서 InfiniteHUD가 사라지거나 초기화되지 않는지 확인한다.
- Result와 Ended에서 InfiniteHUD와 InfiniteMode Result가 함께 표시되는지 확인한다.
- HUD와 Result의 거리가 원본 데이터를 변경하지 않고 소수점 없이 내림 표시되는지 확인한다.
- Retry 후 이전 Run의 이동 거리, Score와 최종 확정 상태가 남지 않는지 확인한다.
- 일반 Stage에서는 InfiniteMode가 수행되지 않는지 확인한다.

---

# 문서 작성 원칙

현재 Feature의 정의만 작성한다.

Feature의 규칙만 작성한다.

System의 책임을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

변경 이력을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

Feature 하나당 문서 하나를 사용한다.
