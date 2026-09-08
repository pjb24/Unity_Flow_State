# 기능 개요

## 기능명

ScoreCollectible

## 목적

Stage Mode와 InfiniteMode에서 Collectible로 점프 시작, 공중 이동 경로와 착지 지점을 안내하고 획득 점수를 제공한다.

---

# 기능 규칙

## 점수

- 두 Mode의 모든 Collectible은 동일하게 개당 10점을 제공한다.
- 개당 점수는 공통 설정 한 곳에서 관리한다. 10점은 Prototype 3 기능 검증용 초기값이며 최종 밸런스 값은 아니다.
- Collectible Score는 InfiniteMode의 기존 거리 Score와 독립적으로 누적한다.
- Prototype 3 Phase 3에서는 Total Score를 계산하거나 기존 Score 및 Result 값을 합산 값으로 대체하지 않는다.
- HUD, Result의 Collectible Score 표시와 Total Score 계산·표시 통합은 Phase 4에서 수행한다.
- 개당 점수 설정은 양의 int만 허용하며 0과 음수 설정은 거부한다. 정수 설정이므로 NaN/Infinity는 입력 대상이 아니다.
- 누적 Collectible Score는 기존 거리 Score와 동일하게 `int.MaxValue`에서 포화한다. 상한에 도달해도 유효한 Collectible은 획득 처리하고 중복 요청을 거부한다.

## 식별과 중복 획득

- Stage Collectible은 고정 고유 ID로 식별한다.
- InfiniteMode Collectible은 구간 식별자와 구간 내부 고유 ID의 조합으로 식별한다.
- 같은 Run의 동일 구간에 등장한 동일 Collectible은 한 번만 획득할 수 있다.
- 같은 Pattern Object가 새 구간으로 재배치되면 별개의 획득 대상으로 취급한다.
- 지나간 Infinite 구간의 획득 기록은 정리하여 장시간 플레이 중 기록이 계속 누적되지 않도록 한다. 정리된 이전 구간의 요청이 다시 획득으로 인정되어서는 안 된다.
- 구간 내부 ID는 대소문자를 구분하는 문자열이다. null, 빈 문자열과 공백만 있는 문자열은 거부하며 ID를 자동 보정하지 않는다.
- 등록되지 않은 ID 또는 같은 구간의 중복 등록은 거부한다. 중복 등록으로 이미 획득한 상태를 복구하지 않는다.

## Run과 Pattern 복구

- Retry, 새 Run과 Mode 전환 시 이전 Run의 획득 상태와 Collectible Score를 초기화한다.
- Pause와 Resume은 같은 Run의 획득 상태와 Score를 유지한다.
- Pattern은 새 위치 배치 완료 후 획득 판정 재개 전에 새 구간의 획득 가능 상태로 복구한다.
- Pattern 복구는 현재 Run의 누적 Collectible Score를 초기화하지 않는다.
- 단순 GameObject 재활성화를 새 구간 또는 새 Run의 초기화 근거로 사용하지 않는다.
- 초기화된 Collectible Data의 중복 Initialize 요청은 거부하고 현재 설정과 획득 상태를 보존한다. Clear는 등록과 점수를 제거하고 Data를 미초기화 상태로 만든다.

## 획득 판정과 표시

- Play 중에만 획득할 수 있다.
- Layer로 접촉 대상을 제한하고 현재 Run에 등록된 Player의 Component 및 소유 관계로 최종 판정한다.
- Player의 여러 Collider는 동일 Player로 취급한다.
- Pause 중 요청은 저장하지 않는다. Resume 시 실제 겹침이 유지되는 경우 현재 접촉 상태를 기준으로 획득할 수 있다.
- 획득 즉시 Renderer와 획득 Collider를 끄고 복구 시 다시 켠다.
- 획득 Collider는 SphereCollider를 사용한다. Resume 재검사는 구체 중심부터 Player Collider의 최근접점까지 거리가 구체 반지름 이하인지 판정한다.
- 최소 시각 표현은 고정된 형태와 구분되는 색상으로 구성한다. 회전, 부유와 파티클은 Phase 3에 추가하지 않는다.

## 안내 배치와 놓친 Collectible

- 유효한 점프 구간마다 도약 전 1개, 공중 경로 3개, 착지 부근 1개의 5개 묶음을 초기 배치 후보로 사용한다.
- 짧거나 낮은 점프는 도달 가능성 검증에 따라 개수와 간격을 조정한다.
- Stage 전체 및 Pattern별 총개수와 좌표는 유효한 점프 구간 조사 후 확정한다.
- 도약 전 안내는 Player가 인지하고 점프할 여유가 있는 위치를 후보로 삼는다.
- Collectible을 놓쳐도 감점이나 실패가 발생하지 않으며 획득 개수는 Stage Clear 조건에 포함하지 않는다.
- 놓친 Stage Collectible은 Run 종료까지 유지한다. Infinite Collectible은 해당 Pattern 재사용 시 정리한다.

---

# 시작 조건

- Stage Mode 또는 InfiniteMode의 Run이 시작되고 Collectible 구성이 준비되었다.
- 현재 Run의 Player가 Play 상태에서 획득 가능한 Collectible에 접촉한다.

---

# 종료 조건

- 개별 획득은 획득 상태와 Score 반영 후 종료한다.
- Run이 Ending 또는 Ended 상태가 되면 획득을 중단한다.
- 게임 종료 시 Runtime 상태를 제거한다.

---

# 수행 결과

- 유효한 획득은 해당 Collectible의 획득 상태와 현재 Run의 Collectible Score에 한 번 반영된다.
- 기존 거리 Score, Goal과 Stage 종료 흐름은 유지된다.

---

# 예외 사항

- 중복 요청, 비 Player 접촉과 비활성 Collectible 접촉은 점수를 변경하지 않는다.
- Pause, Ending과 Ended 상태의 요청은 점수를 변경하지 않는다.
- Resume 시에는 Pause 중 요청을 지연 지급하지 않는다.

---

# 관련 System

- RuntimeDataSystem
- GameSystem
- StageSystem
- InfiniteModeSystem

---

# 제약 사항

- Runtime Data만 사용한다.
- Player 판정은 현재 Run에 등록된 Rigidbody와 접촉 Collider의 소유 관계를 사용한다. 같은 Layer라는 이유만으로 획득을 허용하지 않는다.
- 배치 좌표와 총개수는 아직 확정되지 않았다. 도달 가능성은 자동 검증하고 안내성과 가독성은 화면으로 확인한다.
- 최종 밸런스, Combo, Score 배율, 희귀 Collectible과 우회 고득점 경로는 범위에 포함하지 않는다.

---

# 검증 항목

- 두 Mode의 개당 점수, 다중 Collider 및 중복 요청의 1회 획득을 검증한다.
- 기존 거리 Score 및 Result 계약과 Collectible Score의 독립성을 검증한다.
- Retry, 새 Run, Mode 전환의 초기화와 Pause 및 Resume의 상태 보존을 검증한다.
- Resume 시 유지된 겹침의 획득과 Ending 및 Ended 이후 Score 불변을 검증한다.
- Pattern별 식별 독립성, 재배치 후 복구, 이전 구간 요청 거부와 기록 정리를 검증한다.
- Renderer 및 Collider의 획득 후 비활성화와 복구를 검증한다.
- 안내 경로의 도달 가능성과 모두 놓친 경우의 진행 지속을 검증한다.
- Phase 4의 Total Score와 UI 통합이 조기 포함되지 않았는지 확인한다.
