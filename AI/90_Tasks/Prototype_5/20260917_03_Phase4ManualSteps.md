# 작업 정보

## 작업명

Prototype 5 Phase 4 수동 작업 및 검증 계획

---

## 작업 일자

20260917

---

## 작업 담당자

AI, 사용자

---

## 작업 상태

작업 준비 완료

---

# 작업 목적

Prototype 5의 Momentum Landing Score와 World Rebase를 포함한 전체 게임 회귀를 검증한다. 정적 검사와 Unit Test로 판정할 수 있는 계약·수치·상태는 자동 검증하고, 사용자는 Unity Editor에서 Compile·Test Runner·Build를 실행한 뒤 실제 Player에서 조작감, 화면 연속성과 장시간 성능만 확인한다.

---

# 작업 대상

- Stage Mode 자동 이동, Jump, Momentum Landing, Collectible, Clear Time과 Result
- InfiniteMode Momentum 배율, Score, Difficulty, Pattern, Collectible, Pause·Resume·Result·Retry
- 누적 논리 거리와 반복 World Rebase
- Player Rigidbody, InfiniteModeRoot, CameraRig와 Cinemachine 연동
- 생산 `SampleScene`, Build Settings와 Windows Standalone Player
- 전체 Edit Mode·Play Mode 회귀 Test
- 장시간 실행 중 Camera·Physics·메모리·프레임 안정성
- Roadmap, Feature, System과 Phase 4 검증 결과 문서

Scene이나 Prefab의 새 제작·편집은 현재 예상 범위에 없다. 정적 검사 또는 자동 Test에서 생산 참조 문제가 확인될 때만 AI가 별도 작업표를 제공하고 사용자가 Unity Editor에서 수정한다.

---

# 작업 전 상태

- Phase 3에서 World Rebase 생산 연결과 논리 거리 보존을 완료했다.
- 사용자가 Unity Script Compilation 성공, Edit Mode `648/648`, Play Mode `222/222`, 예상하지 않은 Error·Warning 없음과 Rebase 순간의 화면 이상 없음을 확인했다.
- 생산 Scene은 `Assets/Scenes/SampleScene.unity`이며 Build Settings에는 이 Scene 하나가 활성화되어 있다.
- 기존 프로젝트 작업 기록에서 대상 Build는 Windows Standalone Win64/x64 Development Build를 사용했다.
- Phase 4에는 전체 회귀, 대상 플랫폼 Build와 장시간 반복 Rebase의 성능·안정성 검증이 남아 있다.
- AI는 Unity Editor, Unity Test Runner와 Build를 실행하지 않는다.

---

# 작업 원칙

- 문서·코드·Scene YAML·Prefab·ProjectSettings·`.meta`·GUID를 먼저 정적으로 검사한다.
- 계산식, 경계값, 반복 횟수, 상태 전환, Score·Difficulty·Pattern·Collectible 생명주기는 Edit Mode Unit Test를 우선한다.
- 실제 Scene, Rigidbody, Trigger, FixedUpdate, Camera, UI와 게임 흐름은 Play Mode Test로 검증한다.
- Test는 생산 API와 생산 Scene을 사용하고 계산식이나 상태 머신을 Test 안에 복제하지 않는다.
- 자동 판정 가능한 거리·Score·Offset·Pattern ID·실행 횟수·초기화 여부를 수동 플레이로 확인하지 않는다.
- 수동 확인은 실제 Player의 조작감, 화면 끊김, 프레임 저하, 메모리 증가, Camera·Physics 이상과 Build 실행 여부에 한정한다.
- Test 실패나 예상하지 않은 Error·Warning이 있으면 Build와 장시간 검증으로 진행하지 않는다.
- Scene이나 Prefab은 AI가 직접 수정하지 않는다. 필요한 경우 사용자가 편집하며 AI는 저장된 YAML을 읽기 전용으로 검사한다.
- AI는 Build와 Test Runner를 실행하지 않는다. 사용자가 Unity Editor에서 직접 실행하고 결과를 전달한다.
- Phase 4 검증 중 생산 동작이나 Score 결과가 바뀌는 수정이 발생하면 영향 범위를 다시 산정한다. Score 결과가 바뀌면 Scoring Version 검토 없이 완료 처리하지 않는다.

---

# 수행 Step

## Step 1. Phase 4 기준선과 검증 범위를 확정한다

### AI 작업

- Roadmap Phase 4, Phase 3 결과, 관련 Feature·System 문서와 현재 생산 구현을 대조한다.
- 현재 전체 Test 목록과 Stage·Infinite·Rebase·UI·Result·Retry의 자동 검증 범위를 분류한다.
- Build Settings의 활성 Scene, Unity Version, 대상 플랫폼 관련 설정과 변경 파일 범위를 정적으로 확인한다.
- 장시간 실행 전에 자동화할 항목과 실제 Player에서만 확인할 항목을 분리한다.
- 장시간 수동 검증의 운영 기준은 Windows Standalone Development Build에서 연속 `20분 이상` 실행으로 사용한다. 이 값은 게임 규칙이 아니라 Phase 4 검증 시간 기준이다.

### 사용자 수동 작업

- 기존 대상 플랫폼이 Windows Standalone Win64/x64가 아니라면 실제 대상 플랫폼을 AI에게 알린다. 기존 대상이 유지되면 별도 작업은 없다.

### 완료 조건

- [ ] Phase 4 검증 범위와 Phase 3 기준선이 명확하다.
- [ ] 자동 검증과 수동 검증의 책임이 구분되었다.
- [ ] 대상 플랫폼과 장시간 실행 기준이 확정되었다.

---

## Step 2. 코드·문서·생산 Asset을 정적으로 감사한다

### AI 작업

- Momentum 배율, 논리 거리, Score, Difficulty가 생산 경로에서 단일 기준을 사용하는지 검색한다.
- World X와 Rebase 횟수를 Score·Difficulty·Pattern 선택에 직접 사용하는 잔여 경로가 없는지 확인한다.
- Player, InfiniteModeRoot와 CameraRig가 정확히 한 번씩 이동하고 제외 대상이 이동하지 않는지 Scene 계층과 코드 호출을 대조한다.
- `Physics.SyncTransforms()`와 Cinemachine Target Warp 호출 순서, Pause·Result·Retry·새 Run의 상태 경계를 검사한다.
- Scene·Prefab의 Missing Script, 누락 직렬화 참조, 중복 대상, `.meta`·GUID와 Build Scene 참조를 검사한다.
- 변경된 Runtime·Test의 asmdef 경계, namespace와 에디터 전용 API의 Runtime 유입 여부를 검사한다.
- `git diff --check`와 변경 파일 범위를 확인하고 의도하지 않은 ProjectSettings·Package 변경을 분리한다.
- Feature·System 문서와 구현의 차이가 있으면 Test 작성 전에 정리한다.

### 사용자 수동 작업

없음. 파일과 직렬화 데이터로 확인 가능한 항목은 AI가 처리한다.

### 완료 조건

- [ ] 생산 코드·문서·Scene·Prefab·Build Settings의 계약이 일치한다.
- [ ] Missing 참조, 중복 Rebase 대상과 의도하지 않은 설정 변경이 없다.
- [ ] 정적 검사에서 확인된 자동 Test 공백이 기록되었다.

---

## Step 3. Edit Mode Unit Test 회귀 범위를 보강한다

### AI 작업

- 기존 Test로 충분한 항목은 재사용하고 실제 공백이 있는 경우에만 Test를 추가하거나 보강한다.
- 순수 상태와 계산은 Scene이나 프레임 실행에 의존하지 않는 Edit Mode Unit Test로 검증한다.
- 반복 Rebase에서 누적 Offset·논리 거리·최대 전진 거리의 단조성과 부동소수 경계를 검증한다.
- Momentum 배율, Base Distance Score, Momentum Bonus, Collectible Score와 Total Score가 반복 Rebase 전후 동일 계약을 유지하는지 검증한다.
- Difficulty·Pattern 선택이 논리 거리를 사용하고 Rebase 횟수에 의존하지 않는지 검증한다.
- Pause·Resume·Finalize·Retry·새 Run·초기화 실패와 비정상 입력에서 상태가 누출되지 않는지 검증한다.
- Pattern Scope·Collectible 획득 기록 정리가 장시간 반복에서 무한히 증가하지 않고 이전 Scope 요청을 다시 인정하지 않는지 검증한다.
- 동일한 입력을 반복 실행해도 중복 Score, 중복 Pattern 요청이나 부분 Rebase가 발생하지 않는지 검증한다.

### 사용자 수동 작업

없음. 이 Step에서는 Unity Test Runner를 아직 실행하지 않는다.

### Unit Test 우선 검증

- `879.999...`, `880`, 여러 `880` 배수와 큰 단일 이동
- 다회 Rebase 후 논리 거리·Score·Difficulty 연속성
- Momentum 활성·만료·Pause·Result·Retry 생명주기
- Score 포화, 음수·`NaN`·Infinity·Overflow 거부
- Pattern 요청 ID·선택 이력·AdvanceCount와 Collectible Scope 보존
- Retry와 새 Run의 모든 Run 전용 상태 초기화
- Stage Mode에서 Infinite 전용 상태와 Rebase 미실행

### 완료 조건

- [ ] Phase 4 계산·상태 회귀가 Edit Mode Unit Test로 판정 가능하다.
- [ ] 수동으로 확인할 수치·경계값·실행 횟수가 남아 있지 않다.
- [ ] 새 Test가 생산 코드를 호출하고 계산식을 복제하지 않는다.

---

## Step 4. 생산 Scene Play Mode 회귀 Test를 보강한다

### AI 작업

- 실제 `SampleScene`을 사용하는 기존 Play Mode Test의 Phase 4 범위를 대조한다.
- 필요하면 장시간 대기 없이 여러 Rebase를 결정적으로 반복하는 생산 Scene 통합 Test를 추가한다.
- Stage Mode의 자동 이동·Jump·Momentum Landing·Collectible·Goal·Clear Time·Result·Retry를 검증한다.
- InfiniteMode의 Pattern 전환·Collectible·Score·Difficulty·HUD·Pause·Resume·Result·Retry를 검증한다.
- 반복 Rebase 후 Player·Pattern·Boundary·Collectible·Camera 상대 위치와 Rigidbody 상태가 유지되는지 검증한다.
- Mode 전환과 재시작 후 Rebase Offset, 논리 거리, Momentum, Score, Pattern·Collectible Scope가 새 Run 상태인지 검증한다.
- Test가 생성한 입력 장치·오브젝트·Callback·Scene 상태를 TearDown에서 정리하는지 확인한다.

### 사용자 수동 작업

없음. 이 Step에서는 Test 코드와 정적 검사만 처리한다.

### Play Mode Test 우선 검증

- Stage 시작부터 Goal·Result·Retry까지의 생산 흐름
- Infinite 시작부터 Momentum·Pattern·Collectible·Pause·Result·Retry까지의 생산 흐름
- 같은 Run에서 여러 번 Rebase한 뒤 물리·Camera·Trigger 정상 동작
- Rebase 직전·직후 Score·Difficulty·Pattern·Collectible 상태 연속성
- Result·Paused·Stage Mode의 Rebase 거부
- Retry·새 Run 후 첫 Pattern과 Run 전용 상태 초기화
- UI 문자열과 Runtime·Result Data의 일치

### 완료 조건

- [ ] 생산 Scene의 Phase 4 핵심 흐름을 Play Mode Test로 판정할 수 있다.
- [ ] 반복 Rebase와 Mode·Run 생명주기의 자동 회귀 경로가 존재한다.
- [ ] 화면 감각 외의 생산 상태를 수동 판정으로 남기지 않는다.

---

## Step 5. Script Compilation과 전체 Edit Mode Test를 확인한다

이 Step은 Step 2–4의 정적 검사가 통과한 뒤 수행한다.

### AI 작업

- 관련 Edit Mode Test 목록과 전체 회귀 범위를 사용자에게 제공한다.
- 사용자 결과에서 실패 Test, Compile Error와 Warning을 분석하고 필요한 코드·Test만 수정한다.
- 수정이 발생하면 관련 Test와 전체 Edit Mode Test의 재실행 범위를 다시 지정한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 완료를 확인한다.
2. Console에 예상하지 않은 Compile Error와 Warning이 없는지 확인한다.
3. AI가 지정한 World Rebase·Momentum·Score·Pattern·Collectible 관련 Edit Mode Test를 실행한다.
4. 관련 Test가 통과하면 전체 Edit Mode Test를 한 번 실행한다.
5. Run·Passed·Failed 수와 예상하지 않은 Error·Warning 유무를 AI에게 전달한다.
6. 실패가 있으면 Test 이름, 메시지와 Stack Trace를 전달한다.

### 완료 조건

- [ ] Unity Script Compilation이 성공했다.
- [ ] 관련 Edit Mode Test가 통과했다.
- [ ] 전체 Edit Mode Test가 통과했다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

---

## Step 6. 전체 Play Mode Test를 확인한다

이 Step은 전체 Edit Mode Test가 통과한 뒤 수행한다.

### AI 작업

- 관련 Play Mode Test 목록과 전체 회귀 범위를 사용자에게 제공한다.
- 실패가 있으면 생산 코드 문제와 Test 픽스처·프레임 순서 문제를 구분한다.
- 수정 뒤 영향을 받는 관련 Test와 전체 Play Mode Test의 재실행 범위를 지정한다.

### 사용자 수동 작업

1. AI가 지정한 Stage·Infinite·World Rebase 관련 Play Mode Test를 실행한다.
2. 관련 Test가 통과하면 전체 Play Mode Test를 한 번 실행한다.
3. Run·Passed·Failed 수와 예상하지 않은 Error·Warning 유무를 AI에게 전달한다.
4. 실패가 있으면 Test 이름, 메시지, Stack Trace와 관련 Console Log를 전달한다.

### 완료 조건

- [ ] 관련 Play Mode Test가 통과했다.
- [ ] 전체 Play Mode Test가 통과했다.
- [ ] 예상하지 않은 Error와 Warning이 없다.

---

## Step 7. Build 전 최종 정적 검사를 수행한다

이 Step은 전체 Edit Mode·Play Mode Test가 통과한 뒤 수행한다.

### AI 작업

- 최신 코드·Test·문서·Scene·Prefab·ProjectSettings 변경 범위를 다시 검사한다.
- Build Settings에서 `Assets/Scenes/SampleScene.unity`가 활성화되어 있고 GUID가 유효한지 확인한다.
- Windows Standalone Player에 포함되면 안 되는 Editor 전용 참조와 Test 전용 코드의 Runtime 유입을 검사한다.
- Scene 직렬화 참조와 Missing Script, Package·asmdef 의존성, Graphics·Input 설정을 정적으로 확인한다.
- `git diff --check`를 수행하고 Build 전에 해결해야 할 문제와 무관한 줄바꿈 경고를 구분한다.
- Build 이후 수동 확인할 핵심 플레이 경로와 기록 항목을 최종 체크리스트로 제공한다.

### 사용자 수동 작업

없음. 이 Step이 통과하기 전에는 Build를 실행하지 않는다.

### 완료 조건

- [ ] Build Scene과 대상 플랫폼 의존성이 유효하다.
- [ ] Editor·Test 전용 코드가 생산 Player 경로에 유입되지 않았다.
- [ ] Build를 막는 정적 문제와 의도하지 않은 설정 변경이 없다.

---

## Step 8. Windows Standalone Development Build를 수행한다

### AI 작업

- Build를 실행하지 않는다.
- 사용자가 전달한 Build 결과와 Console 내용을 검증 기록에 반영한다.
- Build 실패 시 오류 메시지와 Editor Log를 기준으로 원인을 분석하고 수정 후 재실행 범위를 안내한다.

### 사용자 수동 작업

1. Unity Editor의 Build Profiles 또는 Build Settings를 연다.
2. Platform이 Windows Standalone, Architecture가 Intel 64-bit(x86_64)인지 확인한다.
3. 활성 Scene이 `Assets/Scenes/SampleScene.unity` 하나인지 확인한다.
4. `Development Build`를 활성화한다. 장시간 성능 기록에 Profiler를 사용할 경우 `Autoconnect Profiler`도 활성화하고 Deep Profiling은 끈다.
5. 새로운 빈 출력 폴더를 선택하여 Build를 한 번 수행한다.
6. Build 성공 여부와 예상하지 않은 Error·Warning 유무를 AI에게 전달한다.
7. 실패하면 첫 오류부터 관련 메시지와 Editor Log 구간을 전달한다.

대상 플랫폼이 Windows Standalone이 아니라고 Step 1에서 확정된 경우, 이 Step의 플랫폼과 Architecture만 해당 대상으로 바꾸고 나머지 절차를 유지한다.

### 완료 조건

- [ ] 대상 플랫폼 Development Build가 성공했다.
- [ ] Build에 예상하지 않은 Error와 Warning이 없다.
- [ ] 생성된 Player가 실행된다.

---

## Step 9. Development Build에서 핵심 플레이 회귀를 확인한다

정확한 수치·Score·Offset·Pattern ID는 화면으로 판정하지 않는다. 해당 항목은 Step 5–6의 자동 Test 결과를 사용한다.

### AI 작업

- 자동 Test에서 이미 판정한 항목을 제외한 최소 화면·조작 체크리스트를 제공한다.
- 사용자가 보고한 현상을 재현 가능한 Mode·시점·조작으로 정리한다.

### 사용자 수동 작업

1. Development Build Player를 실행한다.
2. Stage Mode를 시작해 자동 이동, Jump와 일반·Momentum Landing의 기본 조작감이 기존과 다른지 확인한다.
3. Pause와 Resume을 한 번 수행하고 입력·물리·Camera가 정상 복귀하는지 확인한다.
4. Goal에 도달해 Result가 표시되는지 확인하고 Retry 후 새 Run이 정상 시작되는지 확인한다.
5. InfiniteMode를 시작해 Pattern 전환, Collectible 표시, HUD와 Camera가 정상인지 확인한다.
6. Momentum Landing, Pause·Resume과 Retry를 각각 한 번 확인한다.
7. 눈에 띄는 화면 겹침, UI 누락, 입력 정지, Camera 튐, 물리 폭주 또는 예상하지 않은 로그가 있으면 Mode와 발생 시점을 기록한다.

### 완료 조건

- [ ] Stage Mode 핵심 플레이와 Result·Retry에 눈에 띄는 회귀가 없다.
- [ ] InfiniteMode 핵심 플레이와 UI·Pause·Retry에 눈에 띄는 회귀가 없다.
- [ ] Development Build에서만 발생하는 입력·Camera·물리 문제가 없다.

---

## Step 10. Development Build에서 장시간 반복 Rebase와 성능을 확인한다

이 Step은 Step 9가 통과한 뒤 수행한다. 정확한 Rebase 횟수와 내부 상태는 자동 Test가 판정하므로 사용자가 세지 않는다.

### AI 작업

- 장시간 실행 전에 반복 Rebase의 상태 보존과 Scope 정리가 자동 Test에 포함됐는지 최종 확인한다.
- 사용자 성능 기록에서 Rebase 시점의 반복적인 프레임 저하, 메모리 증가와 Error·Warning을 구분한다.
- 성능 문제가 보고되면 Profiler Marker와 생산 호출 경로를 정적으로 대조하고 별도 최적화 작업이 필요한지 판단한다.

### 사용자 수동 작업

1. Development Build에서 InfiniteMode를 새로 시작한다.
2. Player를 강제로 이동하거나 Scene·Inspector 수치를 변경하지 않고 연속 `20분 이상` 플레이한다.
3. Rebase 순간 Camera 튐·끌림, 지형 틈·겹침, Player·Pattern·Collectible의 프레임 불일치가 반복되는지 관찰한다.
4. 시간이 지날수록 입력 반응, 물리, Pattern 전환 또는 UI 갱신이 느려지는지 확인한다.
5. 가능하면 Unity Profiler에서 시작 직후와 종료 직전 구간을 각각 기록하고 CPU Frame Time, GC Alloc과 Memory가 계속 증가하는지 비교한다. Deep Profiling은 사용하지 않는다.
6. Crash, Freeze, Console Error·Warning, 눈에 띄는 프레임 저하나 지속적인 메모리 증가가 있으면 발생 시간과 직전 상황을 기록한다.
7. 정상이라면 실행 시간, 완료 여부와 특이사항 없음을 AI에게 전달한다.

### 완료 조건

- [ ] 장시간 InfiniteMode에서 반복 Rebase 후에도 정상 진행할 수 있다.
- [ ] 좌표 정밀도에 따른 Camera·Physics·지형 이상이 보이지 않는다.
- [ ] 반복 Rebase와 함께 악화되는 프레임 저하나 지속적인 메모리 증가가 확인되지 않는다.
- [ ] Crash, Freeze와 예상하지 않은 Error·Warning이 없다.

---

## Step 11. Phase 4 결과를 확정하고 문서를 정리한다

### AI 작업

- 코드·Test·Scene·Prefab·ProjectSettings와 사용자 검증 결과를 최종 정적으로 대조한다.
- `git diff --check`와 변경 파일 범위를 다시 확인한다.
- 실제 Test 수, Build 대상·결과, 핵심 플레이와 장시간 실행 결과를 별도 Phase 4 검증 결과 문서에 기록한다.
- `IMPLEMENTATION_ROADMAP_005.md`의 Phase 4 상태와 현재 개발 진행 상태를 실제 결과에 맞게 갱신한다.
- 관련 Feature·System 문서가 구현과 다를 때만 해당 책임 문서를 수정한다.
- 미해결 문제가 있으면 Phase 4 또는 Prototype 5를 완료 처리하지 않고 재현 조건과 후속 작업을 기록한다.

### 사용자 수동 작업

없음. 이미 전달된 Compile·Test·Build·Player 검증 결과를 AI가 문서화한다.

### 완료 조건

- [ ] 정적 검사, Compile, 전체 Edit Mode·Play Mode Test 결과가 기록되었다.
- [ ] 대상 플랫폼 Build와 핵심 플레이 결과가 기록되었다.
- [ ] 장시간 반복 Rebase와 성능 결과가 기록되었다.
- [ ] Roadmap, Feature, System 문서와 생산 구현이 일치한다.
- [ ] Phase 4와 Prototype 5 완료 여부가 확인된 근거로 판정되었다.

---

# 사용자 수동 작업 요약

1. Script Compilation과 예상하지 않은 Error·Warning 확인
2. AI가 지정한 관련 Edit Mode Test와 전체 Edit Mode Test 실행
3. AI가 지정한 관련 Play Mode Test와 전체 Play Mode Test 실행
4. Windows Standalone Win64/x64 Development Build 실행
5. Development Build에서 Stage·Infinite 핵심 플레이 최소 회귀 확인
6. Development Build에서 InfiniteMode를 `20분 이상` 연속 실행해 반복 Rebase와 성능 확인
7. 각 단계의 성공 여부, Test 수, Error·Warning과 이상 발생 시점 전달

다음 항목은 수동 판정하지 않는다.

- 거리, Score, Difficulty, Offset과 Pattern ID의 정확한 값
- Rebase 실행 횟수와 호출 순서
- Pattern·Boundary·Collectible Scope의 내부 상태
- Pause·Retry·새 Run의 내부 상태 초기화 값
- 부동소수 경계, 포화, 비정상 입력과 중복 요청

이 항목들은 정적 검사와 Edit Mode·Play Mode Test로 검증한다.

---

# 검증 책임 요약

| 검증 항목 | 검증 방법 | 실행 주체 |
|---|---|---|
| 문서·코드·Scene·Prefab·Build 설정 계약 | 정적 검사 | AI |
| 계산·경계값·Score·상태 생명주기 | Edit Mode Unit Test | AI 작성, 사용자 실행 |
| 생산 Scene·Physics·Trigger·Camera·UI | Play Mode Test | AI 작성, 사용자 실행 |
| 전체 기존 기능 회귀 | 전체 Edit Mode·Play Mode Test | 사용자 실행 |
| 대상 플랫폼 Build | Unity Editor Build | 사용자 |
| 실제 Player의 조작감·화면 회귀 | Development Build 수동 플레이 | 사용자 |
| 장시간 Camera·Physics·프레임·메모리 | Development Build와 Profiler | 사용자 |
| 결과 대조와 문서화 | 정적 검사 | AI |

---

# 영향 범위

- Runtime: Phase 4 검증에서 결함이 확인된 경우에만 관련 System·Feature 수정
- Test: Edit Mode 순수 상태·계산 Test, Play Mode 생산 Scene 통합 Test
- Scene·Prefab: 읽기 전용 정적 검사, 결함 확인 시에만 사용자 편집
- Build: Windows Standalone Win64/x64 Development Build
- 문서: Prototype 5 Roadmap, 관련 Feature·System, Phase 4 검증 결과

---

# 검증 내용

이 문서는 Phase 4 실행 전 계획 문서다. 실제 Compile, Test Runner, Build와 장시간 Player 결과는 아직 수행되지 않았으며 각 Step 완료 후 기록한다.

---

# 검증 결과

미실행. Step 1부터 순서대로 수행한다.

---

# 후속 작업

- Phase 4 완료 후 Prototype 5 전체 완료 여부를 확정한다.
- 미해결 Build·성능·회귀 문제가 있으면 별도 Bugfix Task로 분리한다.

---

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_005.md`
- `AI/03_Features/InfiniteMode.md`
- `AI/03_Features/MomentumLanding.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/02_Systems/InfiniteModeSystem.md`
- `AI/02_Systems/PlayerControllerSystem.md`
- `AI/02_Systems/CameraSystem.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

---

# 관련 작업 기록

- `AI/90_Tasks/Prototype_5/20260916_14_Phase3ManualSteps.md`
- `AI/90_Tasks/Prototype_5/20260917_02_Phase3VerificationResult.md`

---

# 작성 완료 기준

- Phase 4의 실제 수행 순서가 독립적인 Step으로 구분되어 있다.
- 각 Step에서 AI 작업, 사용자 수동 작업과 완료 조건을 확인할 수 있다.
- 정적 검사와 Unit Test를 수동 확인보다 먼저 사용한다.
- 자동 판정 가능한 수치·상태·실행 횟수를 사용자에게 요구하지 않는다.
- Build와 실제 Player에서만 확인 가능한 항목을 구체적인 수동 절차로 작성한다.
- 확인하지 않은 Compile·Test·Build·성능 결과를 성공으로 기록하지 않는다.
