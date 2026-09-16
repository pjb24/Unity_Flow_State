# 작업 정보

## 작업명
Prototype 5 Phase 2 Step 8 최소 화면 확인

## 작업 일자
20260916

---

# 정적·자동 검증으로 제외한 항목

다음 항목은 사용자가 반복 확인하지 않는다.

- Score 구성 숫자와 Result 문자열 정확성
- Momentum 유지 시간과 Pause 중 Timer 정지
- Retry 및 새 Run 초기화
- HUD·Result 직렬화 참조와 Gradient Effect 연결
- Momentum Landing 전후 수평 속도 규칙

이 항목은 Scene 정적 검사와 Edit Mode `629/629`, Play Mode `221/221` 성공 결과로 판정했다.

---

# 사용자 최소 화면 확인 절차

아래 Gradient Key를 한 번 수정한 뒤 `SampleScene`을 Play하고 화면을 확인한다.

사용자 첫 화면 확인에서 5개 Color Key가 Fill 감소 중 부자연스럽게 보이는 문제가 확인됐다. 시각 계약을 좌측 Red와 우측 Cyan 두 Key의 연속 보간으로 단순화했다. Runtime은 `UIManagementSystem` Inspector에 직렬화된 Gradient를 그대로 `MomentumGradientEffect`에 전달하므로 아래 Scene 작업 결과가 즉시 표시된다.

1. `UIManagementSystem`의 `Momentum Duration Gradient`를 연다.
2. 중간 Orange, Yellow, Green Color Key를 삭제한다.
3. Color Key는 Time `0.00`의 Red `#FF0000`과 Time `1.00`의 Cyan `#00FFFF`만 남긴다.
4. Alpha Key는 Time `0.00`, `1.00` 모두 Alpha `1`로 유지하고 Mode는 `Blend`로 유지한다.
5. Scene을 저장한다.

## 1. 기본 Momentum HUD

1. InfiniteMode로 시작한다.
2. 우측 하단에 `x1.00`과 빈 Bar가 표시되는지 확인한다.
3. Text와 Bar가 화면 밖으로 잘리거나 기존 Infinite HUD와 겹치지 않는지 확인한다.
4. 사용한 Game View 해상도를 기록한다.

## 2. Momentum 성공 표시

1. `Space`로 Jump한다.
2. 착지 직전 Momentum Landing Window에서 `Left Shift`를 누른다.
3. 배율 Text가 `x1.00`보다 큰 값으로 바뀌고 Bar 길이가 보이는지 확인한다.
4. Bar 전체에서 좌측 빨강과 우측 청록 사이의 연속 보간이 자연스러운지 확인한다.
5. 시간이 지나며 Bar 길이가 줄고 기본 상태로 돌아오는 동안 Text와 색이 배경에서 읽히는지 확인한다.
6. Momentum Landing 순간 Player가 갑자기 빨라지는 체감이 없는지 확인한다.

## 3. Pause와 Result 겹침

1. Momentum HUD가 표시된 상태에서 Pause 화면을 연다.
2. Momentum HUD가 Resume, Retry, Quit 버튼이나 선택 표시를 가리지 않는지 확인한다.
3. Resume 후 HUD가 정상적으로 다시 보이는지 확인한다.
4. InfiniteMode Result 화면까지 진행한다.
5. Momentum HUD가 Result Score와 Retry, Quit 조작을 방해하지 않는지 확인한다.

---

# 자동 이동 계약 확인

- `PlayerInputSystem.EnablePlayerActionMap()`은 `Move` Action을 명시적으로 비활성화한다.
- `PlayerMovementSystem`은 수평 이동 계산에 Move 입력을 사용하지 않는다.
- 이는 Stage와 InfiniteMode의 확정된 자동 이동 계약이며 별도의 감속·정지 입력은 요구하지 않는다.

---

# 전달할 결과

다음 내용만 AI에 전달한다.

- 사용한 Game View 해상도
- 기본 `x1.00`과 빈 Bar 가독성: 성공 또는 문제 설명
- Momentum 배율·Bar 길이·Gradient 가독성: 성공 또는 문제 설명
- Pause·Result 및 기존 HUD와 겹침: 성공 또는 문제 설명
- Momentum Landing 순간 불필요한 가속 체감: 없음 또는 재현 절차

---

# Inspector 직렬화 값 적용 감사

Phase 2에서 변경한 생산 코드의 `[SerializeField]` 선언과 사용 지점을 전수 대조했다.

- `_momentumDurationGradient`는 Inspector 값을 `MomentumGradientEffect`에 직접 전달한다. 생산 코드의 승인 Gradient 생성 함수는 Runtime에서 호출하지 않으며 Test Fixture에서만 사용한다.
- Momentum Fill Image의 Inspector Color를 흰색으로 강제 변경하던 코드를 제거했다.
- `MomentumGradientEffect`는 Inspector Image Color와 Gradient 색을 곱하므로 Image tint도 Runtime에 반영한다.
- Momentum Landing Window, 이동 속도·가속도·중력·최대 속도, Infinite 종료·Grace·Score Per Unit 설정은 각 Inspector 필드를 생산 계산과 상태 초기화에 직접 전달한다.
- UI Text·Button·Root 참조와 Game/System 참조는 Inspector 참조를 사용하며 다른 Scene 객체를 자동 탐색해 대체하지 않는다.
- `_showDifficultyInDevelopment`는 Editor/Development Build에서 Inspector 값을 사용하고 Release Build에서 Difficulty Text를 숨기는 기존 빌드 계약을 유지한다.

직렬화 값을 무시하고 하드코딩 fallback으로 대체하는 생산 경로는 더 이상 발견되지 않았다. 음수 Momentum Landing Window를 `0`으로 제한하는 처리는 잘못된 Inspector 수치를 안전 범위로 제한하는 검증이며 별도 기본값으로 교체하지 않는다.

---

# 사용자 화면 확인 결과

사용자가 `UIManagementSystem` Inspector에서 Gradient 색 구성을 최종 조정했다. Runtime이 해당 직렬화 값을 직접 적용하는 상태에서 다음 화면 구성이 적절함을 확인했다.

- Momentum HUD 위치와 Bar 길이
- 조정된 Gradient 색 구성과 Fill 감소 중 가독성
- 기존 Infinite HUD, Pause 및 Result UI와의 배치

화면 가독성, 겹침과 불필요한 가속 부재 확인이 완료됐다.
