# 작업 정보

## 작업명

Prototype 5 Phase 3 수동 작업 및 검증 계획

---

## 작업 일자

20260916

---

## 작업 담당자

AI, 사용자

---

## 작업 상태

작업 준비 완료

---

# 작업 목적

InfiniteMode 생산 경로에 누적 논리 거리와 World Rebase를 연결한다. 계산, 상태 생명주기, 실행 순서와 생산 Scene 연동은 정적 검사와 Unity Test Runner로 최대한 자동 검증한다. 사용자의 수동 작업은 Unity Editor에서만 가능한 Compile·Test 실행, 기존 Scene 참조 연결, Rebase 순간의 시각적 연속성 확인으로 제한한다.

---

# 작업 대상

- `WorldRebaseState`와 생산 `InfiniteModeSystem` 연결
- Player Rigidbody World X와 누적 Rebase Offset을 사용한 논리 거리
- `PlayerControllerSystem`의 Rigidbody 위치 이동과 물리 상태 보존
- `InfiniteMapPattern`의 InfiniteModeRoot·Pattern Slot·Boundary·Collectible 일괄 이동
- `CameraSystem`의 CameraRig 이동과 Cinemachine Target Warp 통지
- Rebase 프레임의 거리·Score·Difficulty·Pattern 진행 순서
- Pause·Result·Stage Mode 실행 거부와 Retry·새 Run 초기화
- 생산 `SampleScene`의 기존 System 참조
- Edit Mode Unit Test와 Play Mode 통합 Test

대상 플랫폼 Build와 장시간 성능 검증은 Phase 4 범위이며 Phase 3 완료 조건에 포함하지 않는다.

---

# 작업 전 상태

- Phase 1에서 Rebase 임계값 `880`, 기본 Offset `880`, 논리 거리 계산과 상태 보존 계약이 확정되었다.
- `WorldRebaseState`와 경계값·반복 Rebase·비정상 입력·누적 거리 보존 Edit Mode Test가 존재한다.
- 생산 `InfiniteModeSystem`은 아직 `WorldRebaseState`를 소유하거나 Rebase를 실행하지 않는다.
- `PlayerControllerSystem`, `InfiniteMapPattern`, `CameraSystem`에는 생산 Rebase API가 없다.
- 생산 `SampleScene`에는 네 대상 객체가 이미 있으므로 새로운 공통 World Root를 만들 필요가 없다.
- Prototype 5 Phase 2 검증에서 Unity Script Compilation, Edit Mode Test `629/629`, Play Mode Test `221/221`이 통과했다.

---

# 작업 원칙

- AI는 코드, 문서, Test와 Scene YAML을 먼저 조사하고 정적으로 확인 가능한 항목을 사용자 수동 작업으로 넘기지 않는다.
- 계산식, 경계값, 입력 거부, 상태 전환과 생명주기는 Edit Mode Unit Test로 검증한다.
- 생산 Scene, Rigidbody, Trigger, System 실행 순서와 Cinemachine 연동은 Play Mode Test로 검증한다.
- Test는 생산 API를 호출하며 계산식을 Test 안에 다시 구현하지 않는다.
- 사용자는 Unity Editor에서 Script Compilation과 Unity Test Runner 실행 결과만 확인한다. AI는 Unity Editor, Unity Test Runner와 Build를 실행하지 않는다.
- AI는 Scene을 수정하지 않는다. Scene 변경은 Compile과 관련 Edit Mode Test가 통과한 뒤 사용자만 수행한다.
- 수치, 상태값, 호출 횟수와 상대 위치는 Test로 판정한다. 수동 플레이로 숫자를 비교하거나 정확한 프레임 입력을 재현하지 않는다.
- 수동 화면 확인은 Rebase 순간의 화면 끊김·떨림·지형 겹침처럼 자동 판정이 적절하지 않은 항목에만 사용한다.
- Stage Mode 자동 이동과 Move 입력 비활성 계약을 유지한다. 감속과 정지는 이번 작업 요구사항이 아니다.
- Rebase는 Score 결과를 바꾸지 않으므로 Scoring Version `2`를 유지한다.

---

# 수행 Step

## Step 1. 생산 Rebase 연결 지점과 Scene 참조를 조사한다

### AI 작업

- `InfiniteModeSystem.FixedUpdate`의 거리, Score, Pattern 진행, 추락 처리 순서를 추적한다.
- `StageSystem.InfiniteMapPattern`, `PlayerControllerSystem`, `CameraSystem`의 현재 접근 경로와 초기화 순서를 확인한다.
- 생산 `SampleScene`을 읽기 전용으로 검사하여 Player, `InfiniteModeRoot`, Pattern Slot, Boundary, Collectible Scope, CameraRig와 Follow Target 계층을 기록한다.
- 새 직렬화 참조가 필요한지, 기존 System 참조로 안전하게 접근할 수 있는지 결정한다.
- Rebase 책임 밖의 Phase 2 Score·Momentum·UI 경로와 Stage Mode 경로를 구분한다.
- 조사 결과와 최종 변경 대상을 별도 Step 결과 문서에 기록한다.

### 사용자 수동 작업

없음. 코드와 Scene YAML로 확인 가능한 내용은 AI가 처리한다.

### 완료 조건

- [x] Rebase 전후 생산 실행 순서가 확인되었다.
- [x] 각 이동 대상의 소유 System과 이동 API 책임이 확인되었다.
- [x] 사용자에게 필요한 Scene 참조 목록이 현재 C# 필드와 객체명을 기준으로 확인되었다.
- [x] 변경하지 않을 Stage Mode·Score·Momentum·UI 범위가 기록되었다.

---

## Step 2. 생산 논리 거리와 Rebase 조정 상태를 구현한다

### AI 작업

- `InfiniteModeSystem`이 Run별 `WorldRebaseState`를 소유하고 초기화하도록 연결한다.
- Player Rigidbody World X와 누적 Rebase Offset으로 현재 논리 X와 최대 전진 거리를 갱신한다.
- Rebase 전에 현재 위치까지 거리와 Score 증가분을 한 번 반영하고, Rebase 후 동일 논리 거리를 Difficulty와 Pattern 진행에 전달한다.
- InfiniteMode Playing에서만 Rebase를 요청하고 Pause, Result, Ended, Stage Mode와 실행 중 중복 요청을 거부한다.
- Retry, 새 Run, 초기화 실패와 Stop에서 Offset, 논리 거리와 실행 상태가 누출되지 않도록 한다.
- 관련 Edit Mode Unit Test를 추가하거나 보강한다.

### 사용자 수동 작업

없음. 계산과 상태 검증은 AI의 정적 검사와 Unit Test 작성 범위다.

### Unit Test 우선 검증

- `879.999...`, `880`, `880`의 여러 배와 한 프레임 큰 이동
- 반복 Rebase 이후 현재 논리 X와 최대 전진 거리 연속성
- Rebase 프레임 거리·Base Distance Score·Momentum Bonus 중복 또는 누락 방지
- Rebase 전후 Difficulty 입력 동일성
- Pause·Result·Ended·Stage Mode·중복 실행 거부
- 음수·`NaN`·Infinity·Overflow 입력 거부와 상태 불변
- Retry·새 Run·Stop 이후 Offset과 실행 상태 초기화

### 완료 조건

- [x] 생산 거리 계산이 Player World X 단독 값에 의존하지 않는다.
- [x] Rebase 전후 거리, Score와 Difficulty 입력이 보존된다.
- [x] 상태 경계와 비정상 입력이 Unit Test로 판정 가능하다.

---

## Step 3. Player와 Infinite World 이동 API를 구현한다

### AI 작업

- `PlayerControllerSystem`에 유효한 음의 X Offset을 Player Rigidbody에 적용하는 API를 구현한다.
- 이동 전후 선형 속도, 수직 속도, 회전, 각속도와 Constraints가 보존되도록 한다.
- `InfiniteMapPattern`에 `InfiniteModeRoot`를 같은 Offset으로 이동하는 API를 구현한다.
- Pattern Slot, 활성 Pattern, Anchor, Collider, Boundary와 Pattern 자식 Collectible이 계층 이동으로 정확히 한 번만 이동하는지 확인한다.
- Rebase가 Pattern 진행, Slot 재사용, Boundary 초기화, Collectible Scope 재생성과 획득 상태 변경을 일으키지 않도록 한다.
- 각 API의 유효성, 실패 시 무변경과 상태 보존을 Edit Mode Unit Test로 검증한다.

### 사용자 수동 작업

없음. Scene 배치는 이 Step에서 변경하지 않는다.

### Unit Test 우선 검증

- Player X만 Offset만큼 변경되고 Y/Z와 물리 상태가 보존됨
- Pattern 두 Slot의 간격 `44`와 Anchor·Collider 상대 위치 보존
- Boundary ID, Trigger 상태, `AdvanceCount`, Pattern ID와 요청 ID 보존
- Collectible Scope ID, Local ID, 획득 상태와 Score 보존
- 잘못된 Offset, 미초기화와 누락 참조에서 부분 이동이 발생하지 않음

### 완료 조건

- [x] Player와 Infinite World가 동일한 Offset을 적용할 수 있다.
- [x] Pattern·Boundary·Collectible의 논리 상태가 Rebase와 분리되어 있다.
- [x] 물리 상태와 계층 상대 위치를 자동 Test로 검증할 수 있다.

---

## Step 4. Camera 이동과 Rebase 원자적 실행 순서를 연결한다

### AI 작업

- `CameraSystem`에 CameraRig와 Follow Target을 Player와 같은 Offset으로 이동하는 API를 구현한다.
- Cinemachine Target Warp API를 사용하여 이전 World 위치를 향한 Damping 이동을 방지한다.
- `InfiniteModeSystem`에서 Player, Infinite World, Camera 이동이 모두 성공한 뒤 `Physics.SyncTransforms()`를 한 번 호출하고 Target Warp를 통지하도록 실행 순서를 연결한다.
- 부분 적용을 막기 위한 참조·입력 사전 검증과 실행 중 상태 해제를 구현한다.
- Rebase 이후 Follow Target과 Player의 상대 위치, Camera Y/Z, Orthographic Size와 활성 상태가 유지되는지 Test 가능하게 한다.

### 사용자 수동 작업

없음. Camera 동작 코드와 자동 검증을 먼저 완성한다.

### Unit Test 우선 검증

- CameraRig·Follow Target의 X Offset과 Y/Z 보존
- Player와 Camera의 Rebase 전후 상대 위치 보존
- Target Warp 통지 대상과 Offset 일치
- `Physics.SyncTransforms()`와 Camera 보정의 실행 순서 및 중복 방지
- 실패 경로에서 Rebase 상태가 영구 잠기지 않음

### 완료 조건

- [x] Rebase 전체 실행 순서가 Phase 1 계약과 일치한다.
- [x] Camera Damping이 Rebase Offset을 실제 이동으로 해석하지 않도록 보정한다.
- [x] 자동 판정 가능한 Camera 상태를 수동 확인 항목으로 남기지 않는다.

---

## Step 5. AI 정적 검증 후 Compile과 Edit Mode Test를 확인한다

### AI 작업

- 변경된 C#의 namespace, asmdef 경계, 직렬화 필드, API 호출부, `.meta`, GUID와 누락 참조를 정적으로 검사한다.
- Rebase 대상이 중복 이동되거나 이동 대상에서 빠질 수 있는 부모·자식 계층을 Scene YAML과 대조한다.
- `Physics.SyncTransforms()` 호출 수, Cinemachine Target Warp API 사용과 FixedUpdate 실행 순서를 검사한다.
- 논리 거리·Score·Difficulty가 World X 또는 Rebase 횟수를 직접 사용하는 잔여 경로를 검색한다.
- Phase 1 계약, System 문서, Feature 문서와 구현을 대조한다.
- 관련 Edit Mode Test 목록과 전체 Edit Mode 회귀 범위를 지정하고 `git diff --check`를 수행한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료를 확인한다.
2. Console에 예상하지 않은 Compile Error와 Warning이 없는지 확인한다.
3. AI가 지정한 관련 Edit Mode Test를 실행한다.
4. 전체 Edit Mode Test를 한 번 실행한다.
5. Run·Passed·Failed 수와 예상하지 않은 Error·Warning 유무를 AI에게 전달한다.
6. 실패가 있으면 Test 이름, 메시지와 Stack Trace를 전달한다.

사용자는 계산값이나 내부 상태를 수동 플레이로 확인하지 않는다.

### 완료 조건

- [x] AI 정적 검증이 통과했다.
- [x] Unity Script Compilation이 성공했다.
- [x] 관련 Test와 전체 Edit Mode Test `648/648`이 통과했다.
- [x] 예상하지 않은 Error와 Warning이 없다.

---

## Step 6. 생산 Scene의 기존 객체 참조를 연결한다

이 Step은 Step 5가 통과한 뒤 수행한다.

### AI 작업

- Step 1과 최종 C# 구현을 기준으로 정확한 Inspector 필드명, 대상 객체와 기대 Component를 체크리스트로 제공한다.
- 사용자가 저장한 `SampleScene.unity`를 읽기 전용으로 검사하여 직렬화 참조, 중복 객체, Missing Script와 계층 포함 관계를 확인한다.
- Scene 파일을 직접 수정하지 않는다.

### 사용자 수동 작업

1. `SampleScene`을 열고 작업 전 Scene을 저장한다.
2. 기존 `InfiniteModeSystem` Component의 `_playerControllerSystem`에 Player의 `PlayerControllerSystem` Component를 연결한다.
3. 같은 Component의 `_cameraSystem`에 `CameraSystem` GameObject의 `CameraSystem` Component를 연결한다.
4. `InfiniteMapPattern`과 CameraRig의 새 직렬화 참조는 연결하지 않는다. InfiniteMapPattern은 기존 `StageSystem.InfiniteMapPattern` 접근 경로를 사용하고, CameraSystem은 기존 Follow Target의 부모 CameraRig를 사용한다. Pattern Slot, Boundary와 Collectible을 별도 Rebase 대상으로 등록하지 않는다.
5. Player, InfiniteModeRoot와 CameraRig를 새 공통 부모 아래로 이동하지 않는다.
6. Stage Mode Root, Goal, Start Point, UI Canvas와 EventSystem을 Rebase 대상에 연결하지 않는다.
7. Inspector에 `None` 또는 Missing 참조가 없는지 확인하고 Scene을 저장한다.
8. 변경된 Scene 파일을 AI가 정적으로 확인할 수 있도록 알린다.

최종 구현에서 기존 참조만으로 접근하도록 확정된 필드는 만들거나 연결하지 않는다. AI가 Step 6 시작 시 제공하는 최종 필드 목록을 기준으로 한다.

### 완료 조건

- [x] 필요한 기존 System과 Root 참조가 모두 연결되었다.
- [x] 이동 대상의 부모·자식 중복 등록이 없다.
- [x] Rebase 제외 대상이 기존 위치와 계층을 유지한다.
- [x] AI의 Scene YAML 정적 검사가 통과했다.

---

## Step 7. 생산 Scene World Rebase Play Mode Test를 검증한다

### AI 작업

- 실제 `SampleScene`과 생산 Component를 사용하는 Play Mode 통합 Test를 작성한다.
- Player를 임계값 인근으로 배치하여 장시간 대기 없이 Rebase를 결정적으로 발생시킨다.
- 필요한 경우 한 프레임 큰 X 입력으로 여러 배 Offset을 한 번에 처리하는 경로를 검증한다.
- 상대 위치, 상태 보존, 호출 순서와 Retry를 자동 판정하고 수동 숫자 비교를 요구하지 않는다.
- 기존 InfiniteMode, Pattern, Collectible, Camera, Pause와 Mode 전환 회귀 Test 범위를 지정한다.

### Play Mode Test 우선 검증

- `880` 직전에는 미실행, `880`과 직후에는 한 번 실행
- 큰 X에서 필요한 `880` 배수 Offset을 한 번에 적용하고 Player X가 `880` 미만이 됨
- Player·InfiniteModeRoot·Pattern·Boundary·Collectible·Camera 상대 위치 보존
- Rigidbody 속도·회전·각속도·Constraints 보존
- 거리·Base Distance Score·Momentum Bonus·Total Score·Difficulty 연속성
- Pattern ID·선택 이력·AdvanceCount·Boundary 상태 보존
- Collectible 획득·재사용·Scope와 Score 보존
- Pause·Result·Stage Mode에서 미실행
- Retry·새 Run에서 Offset과 논리 거리 초기화
- Rebase 직후 다음 물리·카메라 갱신과 추가 반복 Rebase

### 사용자 수동 작업

1. AI가 지정한 World Rebase 관련 Play Mode Test를 실행한다.
2. 관련 Test가 통과하면 전체 Play Mode Test를 한 번 실행한다.
3. Run·Passed·Failed 수와 예상하지 않은 Error·Warning 유무를 AI에게 전달한다.
4. 실패가 있으면 Test 이름, 메시지, Stack Trace와 관련 Console Log를 전달한다.

정확한 위치 이동, Score 값, 실행 횟수와 상태 보존은 Test가 판정하므로 사용자가 플레이 화면에서 수치로 검증하지 않는다.

### 완료 조건

- [x] 생산 Scene Rebase 관련 Play Mode Test가 통과했다.
- [x] 전체 Play Mode Test `222/222`가 통과했다.
- [x] 예상하지 않은 Error와 Warning이 없다.

---

## Step 8. Rebase 순간의 화면 연속성만 수동 확인한다

이 Step은 Step 7이 통과한 뒤 수행한다.

### AI 작업

- 개발용으로 임계값 직전 상태를 결정적으로 만들 수 있는 기존 Test 또는 안전한 확인 경로를 안내한다.
- Scene이나 생산 수치를 임시로 변경해야 하는 절차는 요구하지 않는다.
- 수동 판정 대상과 자동 Test에서 이미 판정한 항목을 구분한다.

### 사용자 수동 작업

1. `SampleScene`에서 InfiniteMode를 시작한다.
2. AI가 제공한 확인 경로로 Rebase 임계값을 한 번 통과한다.
3. Rebase 순간 화면이 좌우로 튀거나 이전 위치를 향해 끌려가는 Camera 떨림이 없는지 확인한다.
4. Rebase 직전과 직후 지형이 겹치거나 틈이 생기지 않는지 확인한다.
5. Player, Pattern과 Collectible이 한 프레임 동안 서로 다른 Offset으로 보이는 현상이 없는지 확인한다.
6. 가능하면 한 번 더 Rebase하여 같은 시각 문제가 반복되지 않는지 확인한다.
7. 적절하면 결과를 알리고, 문제가 있으면 현상과 발생 시점만 전달한다.

수동 확인에서는 정확한 거리, Score, Difficulty, Offset, Pattern ID나 Collectible 상태를 판정하지 않는다. 해당 항목은 Step 5와 Step 7의 자동 Test 결과를 사용한다.

### 완료 조건

- [x] Rebase 순간 Camera 끊김과 떨림이 눈에 띄지 않는다.
- [x] 지형의 겹침과 틈이 보이지 않는다.
- [x] Rebase 대상이 서로 다른 프레임에 이동한 것처럼 보이지 않는다.

---

## Step 9. Phase 3 결과를 확정하고 문서를 정리한다

### AI 작업

- 변경 코드, Test, Scene 참조와 사용자 검증 결과를 최종 정적으로 대조한다.
- `git diff --check`, 변경 파일 범위, Scene·Prefab·ProjectSettings의 의도하지 않은 변경을 확인한다.
- `InfiniteMode`, `InfiniteModeSystem`, `PlayerControllerSystem`, `CameraSystem` 문서를 실제 구현과 일치하도록 갱신한다.
- `IMPLEMENTATION_ROADMAP_005.md`의 Phase 3 상태와 실제 Test 수를 갱신한다.
- 별도 Phase 3 검증 결과 문서에 Compile, Edit Mode, Play Mode와 수동 화면 확인 결과를 기록한다.
- 미해결 항목이 있으면 Phase 3 완료로 표시하지 않고 재현 조건과 후속 Step을 기록한다.

### 사용자 수동 작업

없음. 이미 전달된 Unity 검증 결과를 AI가 문서화한다.

### 완료 조건

- [x] Roadmap, Feature, System 문서와 생산 구현이 일치한다.
- [x] Compile, 전체 Edit Mode와 전체 Play Mode 결과가 기록되었다.
- [x] 최소 수동 화면 확인 결과가 기록되었다.
- [x] Phase 3 완료 여부와 Phase 4로 넘길 Build·장시간 성능 범위가 명확하다.

---

# 검증 책임 요약

| 검증 항목 | 검증 방법 | 실행 주체 |
|---|---|---|
| 계약·의존성·직렬화 참조·호출 순서 | 정적 검사 | AI |
| 임계값·Offset·논리 거리·상태 생명주기 | Edit Mode Unit Test | AI 작성, 사용자 실행 |
| Player·Pattern·Boundary·Collectible 이동 API | Edit Mode Unit Test | AI 작성, 사용자 실행 |
| 생산 Scene·Rigidbody·Trigger·Camera 연동 | Play Mode Test | AI 작성, 사용자 실행 |
| 전체 기존 기능 회귀 | 전체 Edit Mode·Play Mode Test | 사용자 실행 |
| Scene Inspector 참조 연결 | Unity Editor | 사용자 |
| Scene YAML 참조와 계층 결과 | 정적 검사 | AI |
| 화면 끊김·떨림·지형 겹침 | 최소 수동 플레이 | 사용자 |
| 대상 플랫폼 Build·장시간 성능 | Phase 4 | 사용자 |

---

# 영향 범위

- Runtime: `InfiniteModeSystem`, `PlayerControllerSystem`, `CameraSystem`
- Feature: `InfiniteMapPattern`, `WorldRebaseState`, InfiniteMode 논리 거리
- Scene: `Assets/Scenes/SampleScene.unity`의 기존 Component 참조
- Test: Edit Mode World Rebase 및 생산 상태 Test, Play Mode 생산 Scene 통합 Test
- 문서: InfiniteMode Feature, 관련 System, Roadmap와 Phase 3 작업 기록

---

# 관련 문서

- `AI/README.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/90_Tasks/Prototype_5/20260915_04_Phase1Step3WorldRebasePolicy.md`
- `AI/90_Tasks/Prototype_5/20260916_01_Phase1Step5PureModels.md`
- `AI/90_Tasks/Prototype_5/20260916_13_Phase2VerificationResult.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260915_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_5/20260916_04_Phase2ManualSteps.md`

---

# 작성 완료 기준

- Phase 3의 실제 수행 순서가 독립적인 Step으로 구분되어 있다.
- 각 Step에서 AI 작업, 사용자 수동 작업, 자동 검증과 완료 조건을 확인할 수 있다.
- 정적 검사와 Unit Test로 처리할 항목을 수동 확인으로 요구하지 않는다.
- Scene 작업은 사용자가 수행할 정확한 대상과 금지할 중복 구성을 설명한다.
- Build와 장시간 성능 검증이 Phase 4 범위로 분리되어 있다.
