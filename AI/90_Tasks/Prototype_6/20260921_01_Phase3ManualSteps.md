# 작업 정보

## 작업명

Prototype 6 Phase 3 — 기능 접근 UI, Settings 및 Input Rebinding 수동 작업 계획

## 작업 일자

20260921

## 작업 담당자

AI, 사용자

## 작업 상태

완료 (20260923). Step 1~10의 완료 근거와 승인된 검증 제외를 기록했다.

# 작업 목적

Roadmap 006 Phase 3의 기능 접근 UI와 Settings를 완성하고 Input Rebinding을 생산 입력에 연결한다. 정적 검사와 Unit Test로 판정할 수 있는 상태·경계·연결은 자동화하고, 사용자는 제품 규칙 선택, Scene UI 편집, Unity Script Compilation·Test Runner 실행, 실제 화면·입력 장치·Build 확인만 수행한다.

# 작업 대상

- Main Menu, Mode Select, Leaderboard, Settings, Pause와 Result의 기존 접근·복귀 흐름
- Settings의 Audio·화면·조작 범위와 Runtime 설정 상태
- `Assets/InputSystem_Actions.inputactions`의 Player/UI Action 및 Binding Override, 미사용 Player `Move` Action 제거
- GameSystem, UIManagementSystem, UIInputSystem, PlayerInputSystem과 신규 Settings 책임
- Edit Mode Unit Test, 생산 Scene 기반 Play Mode Test 및 Scene 정적 구성 검사
- 사용자가 편집할 `Assets/Scenes/SampleScene.unity`의 Settings·Leaderboard UI와 직렬화 참조

How To Play의 실제 내용과 첫 Run 자동 안내는 Phase 4, Settings 영구 저장과 실제 Leaderboard 조회·제출·Offline 판정은 Roadmap 7 범위다.

# 작업 전 상태

- Prototype 6 Phase 2 완료 기준으로 Unity Script Compilation, Edit Mode 672개, Play Mode 228개가 모두 성공했고 예상하지 않은 Error/Warning이 없었다. Phase 3 변경 후 다시 검증해야 한다.
- Main Menu에서 Play, How To Play, Leaderboard, Settings와 Quit에 접근할 수 있다. 현재 단일 Stage는 Main Menu → Play → Mode Select → Stage 경로로 시작하므로 별도 Stage Select가 필요하다는 근거는 없다.
- Main Menu와 Pause는 같은 `SettingsPanel`을 사용하며 Back 또는 Cancel로 진입 출처에 복귀한다. Pause에서 Run과 Paused 상태를 보존하는 자동 Test가 존재한다.
- `SettingsPanel`과 `LeaderboardPanel`은 `MenuCanvas` 아래 비활성 Root이며 현재 각각 안내 내용과 Back Button 중심의 자리표시자다.
- Settings 값 모델, Audio·화면 설정 적용 계층, Rebinding 상태·충돌·취소·기본값 복원 구현은 없다.
- Input Action Asset에는 Player `Move`, `Jump`, `MomentumLanding`과 UI `Navigate`, `Submit`, `Cancel`, `Point`, `Click`이 있다. `Move`는 자동 이동 구조에서 사용하지 않으므로 Phase 3에서 Action·Binding·생성 Wrapper·생산 코드·Test 참조를 제거해야 한다. 나머지 Action은 Keyboard&Mouse와 Gamepad Binding을 유지하며 생성 Wrapper와 연결돼 있다.
- Scene에는 AudioListener가 있지만 AudioMixer 또는 생산 AudioSource는 확인되지 않았다. Audio 범위는 존재하지 않는 BGM/SFX 구조를 전제로 확정하지 않는다.
- 개발자 Difficulty Text는 `UNITY_EDITOR || DEVELOPMENT_BUILD`에서만 설정값에 따라 활성화되고 일반 빌드에서는 비활성화하는 조건부 코드가 이미 있다.

# 조사 내용

`AI/README.md`의 진입 순서에 따라 Project·Rules 문서, General Task Template, Roadmap 006, Phase 2 인계, 관련 System·Feature 문서와 현재 Runtime 코드·Test·Input Action Asset·Scene YAML을 확인했다. 이 작업은 여러 System과 Feature 및 사용자 Scene 편집을 함께 조정하므로 GENERAL_TASK_TEMPLATE을 사용한다.

현재 문서에는 Settings가 Runtime 범위라는 경계와 진입·복귀 규칙만 있고, Audio 항목, 화면 항목, 재지정 대상 Binding, 충돌 판정 단위와 취소 규칙의 세부값은 확정돼 있지 않다. 이 값은 정적 검사로 결정할 수 없는 제품 규칙이므로 Step 1에서 사용자 승인을 받아야 한다. 반면 현재 Action/Binding, Scene 계층, 직렬화 참조, 조건부 컴파일과 Navigation 경로는 AI가 정적으로 판정한다.

# 작업 내용

AI는 코드·Test·문서 작성과 정적 검사를 수행한다. 사용자는 명시된 제품 규칙 승인, Scene 편집, Unity Script Compilation·Test Runner·Build 및 실제 장치 화면 확인만 수행한다. AI는 Unity Editor, Unity Test Runner와 Build를 실행하거나 Scene/Prefab을 수정하지 않는다.

## Step 1. Phase 3 Settings와 Rebinding 제품 규칙을 확정한다

### AI 작업

- 현재 생산 Action, Audio 자산, Display 설정, Navigation 상태와 Roadmap 경계를 정적으로 대조한다.
- 아래 결정표의 권장 최소 범위가 기존 구조와 충돌하지 않는지 확인하고, 사용자 선택 결과를 Settings Feature와 관련 System 문서의 계약으로 반영한다.
- 실제 Leaderboard 통신이 없는 동안 표시할 준비·미구현·Offline 상태의 의미를 구분한다. Runtime에서 판정 근거가 없는 Offline을 임의로 표시하지 않는다.
- 현재 단일 Stage 시작 경로가 이미 존재하므로 별도 Stage Select를 Phase 3에 추가하지 않는다. 여러 Stage 목록 요구가 새로 확정된 경우에만 별도 후속 범위로 기록한다.

### 사용자 수동 작업

아래 미확정 제품 규칙을 승인하거나 변경한다. Unity Editor 작업은 없다.

| 결정 대상 | 권장 최소 범위 | 선택이 필요한 이유 |
|---|---|---|
| Audio | Runtime Master Volume 한 항목, 기본값 100%, 0~100% | 현재 AudioMixer·BGM·SFX 생산 구조가 없어 Category별 설정 근거가 없다. |
| 화면 | Fullscreen On/Off 한 항목, 시작 시 현재 `Screen` 상태 사용 | Resolution·Refresh Rate 목록 정책이 문서화되지 않았다. 범위를 늘리려면 정렬·중복·지원 해상도 규칙도 함께 확정해야 한다. |
| 조작 대상 | Player Jump, Momentum Landing, UI Navigate의 Keyboard WASD 4방향 | 자동 이동 구조에서 사용하지 않는 Player Move와 Starter Asset Action, Pointer 위치·Click을 설정에 노출하지 않는다. Submit·Cancel은 UI 접근과 복구를 보장하는 고정 안전 입력으로 유지한다. Gamepad 기본 Binding은 유지하되 재지정하지 않는다. |
| Binding 슬롯 | 각 재지정 대상 Action은 Keyboard 기본 Binding 하나만 사용한다. UI Navigate는 WASD 4방향만 사용한다. | 여러 대체 슬롯을 모두 노출하면 UI와 충돌 규칙이 크게 늘어난다. |
| 충돌 | 같은 Action Map·같은 장치 그룹의 재지정 대상끼리 중복을 거부하고 기존 Binding을 유지. Player와 UI Map 사이의 동일 키는 허용 | Player와 UI Map은 역할에 따라 교대로 활성화된다. |
| 취소 | UI Cancel 또는 Rebind 전용 취소 조작 시 Override 없이 종료하고 이전 표시·Focus 복원 | 취소 입력 자체가 새 Binding으로 채택되는 것을 막아야 한다. |
| 기본값 복원 | 모든 Phase 3 Binding Override와 Settings 값을 이번 Application 시작 기본값으로 복원하며 확인 UI를 거친다 | 영구 저장은 Roadmap 7 범위이고 부분 복원 규칙은 아직 없다. |
| Leaderboard | 현재는 `Not Available` 상태와 Back만 표시. `Ready`와 `Offline` 표시 모델·영역은 준비하되 실제 전환은 Roadmap 7 데이터 계층에서 연결 | 현재 조회 서비스가 없어 Ready/Offline을 사실대로 판정할 수 없다. |

### 완료 조건

- [x] Audio·화면·조작 범위와 Binding 충돌·취소·복원 규칙이 사용자 승인된 계약으로 기록됐다.

## Step 2. 계약 문서와 자동 검증 명세를 먼저 정리한다

### AI 작업

- `Settings.md`에 확정된 값, 변경 단위, 적용 시점, Runtime 생명주기, Main Menu/Pause 공용 화면과 Rebinding 규칙을 기록한다.
- 관련 System 문서에 상태 소유자, Input Action Asset 적용 책임, UI 표시 책임과 플랫폼 API 경계를 배치한다. 같은 규칙을 여러 System에 중복시키지 않는다.
- Leaderboard의 상태 표시는 실제 데이터 접근과 분리하고 `Not Available`이 현재 생산 상태임을 기록한다.
- Unit Test와 통합 Test의 assertion 표를 확정한다. 숫자·상태·실행 횟수·취소 원자성·충돌 판정을 수동 검증 항목에 남기지 않는다.

### 자동 검증 명세

| 대상 | 우선 검증 계층 | 핵심 assertion |
|---|---|---|
| Settings 값 | Edit Mode Unit Test | 기본값, 유효 범위, clamp, 같은 값 중복 적용 방지, Runtime 초기화·전체 복원 |
| Audio·화면 적용 | Edit Mode Unit Test + Adapter 대역 | 상태 변경 성공 시 한 번 적용, 거부·복원 시 정확한 값 적용, 플랫폼 API를 Unit Test에서 직접 호출하지 않음 |
| Rebinding 상태 | Edit Mode Unit Test | 시작 가능 조건, 완료, 취소, 잘못된 대상 거부, 진행 중 중복 요청 거부, 이전 Override 보존 |
| Binding 충돌 | Edit Mode Unit Test | 같은 Map·장치 그룹 중복 거부, 다른 Map 허용, Composite Part별 판정, 거부 시 원자성 |
| 기본값 복원 | Edit Mode Unit Test | Player/UI Override 전체 제거, Settings 초기값 복원, 표시 문자열 갱신 |
| 실제 Action 적용 | Play Mode Test | Override 후 Player Jump·Momentum Landing과 UI Navigate가 새 Control을 사용하고 이전 Control은 대상 슬롯에서 동작하지 않음 |
| 미사용 Move 제거 | 정적 검사 + Edit Mode Test | Asset·생성 Wrapper·생산 코드·Test에 Move Action·Binding·접근자가 없고 나머지 대상 Action이 유지됨 |
| Navigation·Pause | 기존/신규 Edit·Play Mode Test | 양쪽 진입점이 같은 화면 사용, Back 목적지·Focus 복원, Pause Run과 시간·Score 정지 유지 |
| Scene 구성 | Play Mode 구성 Test + 정적 YAML | 필수 Component·참조·Button 이벤트·Navigation·초기 활성 상태 |
| Difficulty 표시 | Edit Mode 정적 조건 검사 + Play Mode 개발 환경 Test + 일반 Build 수동 확인 | 개발 표시 정책과 일반 빌드 강제 비활성 |

### 사용자 수동 작업

없음. 문서와 Test 명세는 AI가 정적으로 작성하고 대조한다.

### 완료 조건

- [x] 확정 계약과 자동 검증 책임이 관련 문서에 반영되고 모순이 없다.

## Step 3. 순수 Settings·Binding 상태와 Edit Mode Unit Test를 구현한다

### AI 작업

- Unity UI와 Scene에 의존하지 않는 Settings 값 상태, Rebinding 대상 식별, 충돌 검사와 복원 상태를 구현한다.
- 생산 코드와 함께 Edit Mode Unit Test를 작성한다. 정상 사례뿐 아니라 경계값, 중복 요청, 취소, 충돌, 잘못된 Action/Binding ID와 복원 후 재시도를 포함한다.
- AudioListener, `Screen`과 Input System 정적 API를 순수 상태에 직접 결합하지 않고 얇은 적용 경계로 분리해 Unit Test에서 대역으로 검증한다.
- Action 이름이나 배열 순서만으로 Binding을 식별하지 않고 Asset의 안정적인 Action/Binding ID와 Composite Part 정보를 사용한다.
- 생성 Wrapper를 수작업으로 편집하지 않는다. Input Action Asset 자체 변경이 불필요하면 기존 Asset과 Wrapper를 보존한다.
- Player Move 제거는 Input Action Asset 변경이 필요한 확정 범위다. Asset에서 Move와 모든 Binding을 제거한 뒤 생성 Wrapper를 재생성하고, Move를 비활성화하거나 참조하는 생산 코드와 Test를 함께 제거한다.

### 사용자 수동 작업

없음. 이 Step에서는 Scene, Prefab, ProjectSettings와 Input Actions Editor를 수정하지 않는다.

### 완료 조건

- [x] Phase 3 순수 상태와 경계 조건을 검증하는 Edit Mode Unit Test가 작성됐다.
- [x] 정적 코드 검사에서 책임 중복, Scene 의존 Unit Test와 임의 Binding Index 의존이 없다.

## Step 3-A. 미사용 Player Move Action을 제거한다

### AI 작업

- 자동 이동 구조에서 Player Move 입력을 사용하지 않는 근거와 Player Move를 비활성화하거나 참조하는 생산 코드·Test 목록을 정적으로 대조한다.
- Move Action·Binding 제거 후 생성 Wrapper에서 Move 접근자가 사라진 상태를 기준으로 생산 코드와 Test의 참조를 제거한다.
- Jump와 Momentum Landing, UI Navigate·Submit·Cancel의 Action ID와 Binding이 보존되는지 정적으로 검사한다.

### 사용자 수동 작업

1. Unity Input Actions Editor에서 Player `Move` Action과 해당하는 모든 Binding을 제거한다.
2. 같은 `Assets/InputSystem_Actions.inputactions` Asset에서 C# Wrapper를 재생성하고 저장한다.

### 완료 조건

- [x] Input Action Asset과 생성 Wrapper에 Player Move 및 해당 Binding이 없다.
- [x] 생산 코드와 Test에 Player Move를 비활성화하거나 참조하는 코드가 없다.
- [x] Jump와 Momentum Landing, UI Navigate·Submit·Cancel이 유지된다.

## Step 4. Runtime Settings UI 연결과 Play Mode Test를 구현한다

### AI 작업

- Main Menu와 Pause가 하나의 Settings 상태와 화면을 공유하도록 GameSystem·UIManagementSystem·입력 책임을 연결한다.
- Rebind 시작 동안 일반 UI 입력이 중복 실행되지 않게 하고 완료·취소 후 정확한 Button으로 Focus를 복원한다.
- Rebinding Override를 UI Action과 Player Action의 실제 사용 인스턴스에 적용한다. 별도 Input Action 복제본에만 적용되는 구현을 허용하지 않는다.
- Settings 진입·복귀, Pause 보존, 실제 Binding 적용·취소·복원, Leaderboard 상태 표시와 개발 Difficulty 표시의 Play Mode Test를 작성하거나 기존 Test를 보강한다.
- Scene 필드가 확정되면 Step 6에 **Scene 경로 → Component 타입 → Inspector 필드 → 연결 대상 → 초기값 → OnClick 메서드** 표를 실제 코드명으로 추가한다. 존재하지 않는 필드나 메서드를 사용자에게 미리 만들도록 요구하지 않는다.

### 사용자 수동 작업

없음. AI는 Unity Editor와 Test Runner를 실행하지 않는다.

### 완료 조건

- [x] 생산 연결과 영향받은 Edit/Play Mode Test 코드가 작성됐다.
- [x] Scene 수동 편집표가 실제 직렬화 필드와 public Button 메서드를 기준으로 확정됐다.

## Step 5. Scene 편집 전 Script Compilation과 집중 Unit Test를 확인한다

### AI 작업

- C# 참조, asmdef, Input Action/Wrapper ID, 직렬화 필드와 Test 목록을 정적으로 검사한다.
- Scene 참조가 없어도 실행 가능한 Edit Mode Unit Test Fixture와 예상 개수를 실제 작성 결과로 제공한다.
- `git diff --check`와 범위 밖 Package·ProjectSettings·How To Play·저장·실제 Leaderboard 변경 여부를 확인한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error/Warning이 없는지 확인한다.
2. AI가 Step 4 완료 후 지정한 Phase 3 Edit Mode Unit Test Fixture를 Test Runner에서 실행한다.
3. 시도·성공·실패 개수와 예상하지 않은 Error/Warning 유무를 전달한다.

Scene 참조 누락을 전제로 하는 Play Mode 구성 Test와 전체 회귀는 이 Step에서 실행하지 않는다.

### 완료 조건

- [x] Script Compilation이 성공하고 예상하지 않은 Error/Warning이 없다.
- [x] Phase 3 집중 Edit Mode Unit Test가 모두 성공한다.

## Step 6. Settings와 Leaderboard Scene UI를 구성한다

### AI 작업

- Step 4에서 확정한 UI 요소만 대상으로 생성 타입, 계층, RectTransform, Text, Selectable Navigation, Color Tint, 초기 활성 상태와 접근성 기준을 표로 제공한다.
- 기존 `MenuCanvas`와 `SettingsPanel`·`LeaderboardPanel`을 재사용하고 새 Canvas 또는 EventSystem이 필요하지 않은지 정적으로 판단한다.
- UI 값 표시와 상태 갱신이 자동 Test 대상인지, 실제 가독성만 수동 대상인지 구분한다.

### 사용자 수동 작업

1. AI가 이 문서에 추가한 확정 표에 따라 `Assets/Scenes/SampleScene.unity`만 편집한다.
2. 기존 `MenuCanvas/SettingsPanel` 아래에 확정된 Audio·화면·조작 UI, Rebind 대기·충돌 안내, Restore Defaults 확인 UI와 Back을 구성한다.
3. 기존 `LeaderboardPanel`에 현재 `Not Available` 상태를 표시하는 영역과 Back을 유지하고, 확정된 경우 상태 Text 참조 영역을 구성한다.
4. 모든 Button/Slider/Toggle/Dropdown의 Selectable Navigation을 명시적으로 연결하고 Keyboard/Gamepad Focus 순서가 시각적 순서와 같게 한다.
5. Settings와 Leaderboard Root는 비활성 상태로 저장하고 Scene을 저장한다.

Scene을 수정하는 Editor Script를 만들거나 AI가 Scene YAML을 직접 편집하지 않는다.

### 완료 조건

- [x] 확정 표의 Scene UI 계층과 표시 요소가 사용자에 의해 구성됐다.

## Step 7. Inspector 참조와 UI 이벤트를 연결하고 정적으로 검증한다

### AI 작업

- 사용자가 저장한 Scene YAML에서 Root/Component/fileID/GUID, 직렬화 참조, Button OnClick, Selectable Navigation, 초기 활성 상태와 누락 Script를 검사한다.
- Input Action Asset과 생성 Wrapper의 Action/Binding ID 일치, Control Scheme, Composite Part와 유지 대상 Action의 기존 Gamepad Binding 보존을 정적으로 대조한다.
- 정적으로 확인 가능한 항목을 사용자에게 Inspector에서 다시 확인하도록 요구하지 않는다. 불일치가 있을 때만 정확한 Object·Component·Field·값을 안내한다.

### 사용자 수동 작업

1. Step 4의 확정 연결표에 따라 Inspector 필드와 OnClick을 연결하고 Scene을 저장한다.
2. AI 정적 검사에서 수정 요청이 있는 경우 해당 항목만 수정하고 다시 저장한다.
3. Settings Controls에는 Jump, Momentum Landing, Navigate Up, Navigate Down, Navigate Left, Navigate Right의 Rebind 행만 둔다. Submit·Cancel Rebind 행은 제거한다.
4. `SettingsUIController`의 `_bindingTexts`와 `_rebindButtons` 배열 크기를 각각 6으로 맞추고, 위 행 순서와 동일하게 0~5번 요소를 연결한다.
5. Explicit Navigation에서는 Navigate Right RebindButton의 Down을 Restore Defaults Button으로, Restore Defaults Button의 Up을 Navigate Right RebindButton으로 연결한다.

Input Actions Editor에서 Player Move 제거와 Wrapper 재생성은 Step 3-A에서 수행한다. 그 밖의 Binding은 기존 Binding Override 구현만 사용하므로 수동 Asset 편집과 Wrapper 재생성을 하지 않는다.

### 완료 조건

- [x] Scene과 Input Action의 필수 참조·이벤트·Navigation이 정적 검사에서 일치한다.

## Step 8. 전체 자동 Test 회귀를 확인한다

### AI 작업

- 최종 Scene 기준으로 집중 Edit Mode, 집중 Play Mode, 전체 Edit Mode와 전체 Play Mode 순서 및 실제 Fixture·예상 개수를 제공한다.
- 실패 로그를 받으면 먼저 Test 기대값, 생산 코드와 Scene 연결 중 원인을 분리하고 정적으로 수정 가능한 부분을 처리한다.
- 자동 Test로 확인할 수 있는 값, 상태, 입력 발생 횟수와 Pause 보존을 수동 재현하도록 요구하지 않는다.

### 사용자 수동 작업

1. Unity Script Compilation 성공과 예상하지 않은 Error/Warning이 없는지 다시 확인한다.
2. AI가 지정한 Phase 3 집중 Edit Mode와 Play Mode Fixture를 실행한다.
3. 집중 Test 성공 후 전체 Edit Mode와 전체 Play Mode Test를 각각 실행한다.
4. 각 실행의 시도·성공·실패 개수와 예상하지 않은 Error/Warning 유무를 전달한다.

### 완료 조건

- [x] 최종 변경 기준 Script Compilation과 집중·전체 Edit/Play Mode Test가 모두 성공한다.
- [x] 예상하지 않은 Error/Warning이 없다.

## Step 9. 실제 입력 장치, 화면 표현과 일반 Build를 확인한다

### AI 작업

- 실제 장치 전환, Focus 가시성, Rebind 대기 안내, 화면 잘림과 일반 빌드 전용 Difficulty 비활성만 수동 검증으로 남긴다.
- 해상도·입력 장치와 무관하게 자동 판정 가능한 Binding 값, 충돌 거부, 취소 원자성, 복원 결과는 Step 8 Test 결과를 사용한다.
- Build를 실행하지 않는다.

### 사용자 수동 작업

1. Game View에서 Main Menu와 Pause 양쪽으로 Settings를 열고 동일한 값이 유지되는지, Back 후 원래 화면의 Settings Button으로 Focus가 돌아오는지 확인한다.
2. Keyboard/Mouse로 Settings 항목 이동, 값 변경, Rebind 시작·취소·성공, 충돌 안내, Restore Defaults 확인과 Back을 한 번씩 조작한다. 빠른 연타나 프레임 단위 판정은 하지 않는다.
3. Stage와 Infinite를 각각 시작해 변경한 Player Binding이 실제 조작에 사용되는지 확인한다. Pause Settings 동안 게임이 시각적으로 재개되지 않는지 확인한다.
4. Gamepad를 보유한 경우 Keyboard/Mouse와 Gamepad를 번갈아 사용해 Focus와 장치별 Binding 표시를 확인한다. 보유하지 않은 경우 미확인 사유를 기록하고 사용자의 승인된 검증 제외로 처리한다.
5. `1920 x 1080`, `1280 x 720`과 사용 가능한 최소 한 개의 비 16:9 Game View에서 Settings·Leaderboard의 겹침, 잘림, Text 가독성과 선택 표시를 확인한다.
6. Unity Editor에서 Development Build를 끈 일반 Player Build를 직접 만들고 실행한다. Infinite HUD에 Difficulty 정보가 표시되지 않으며 메뉴·Settings·Rebinding 기본 흐름이 동작하는지 확인한다.
7. 결과와 사용한 입력 장치·해상도·Build 종류, 예상하지 않은 Error/Warning 유무를 전달한다.

AI는 Build를 시도하지 않는다. Build 실패 시 사용자가 로그를 제공하면 AI가 원인을 조사하고 수정한다.

### 완료 조건

- [x] 사용 가능한 실제 장치에서 Focus·가독성·기본 조작이 정상이다.
- [x] 장치 미보유 항목은 사유와 사용자 승인된 검증 제외가 기록됐다.
- [x] 일반 Build에서 개발자 Difficulty UI가 숨겨지고 Phase 3 기본 흐름이 정상이다.

### 수행 기록

- `1920×1080`, `1280×720` Game View에서 Settings·Leaderboard의 잘림, 가독성 및 Focus 표시를 확인했고 문제가 없었다.
- Keyboard/Mouse로 Main Menu 및 Pause에서 Settings 진입, Rebind·취소·충돌·Restore Defaults·Back 흐름을 확인했고 문제가 없었다.
- Development Build와 일반 Player Build를 실행했다. Development Build를 끈 일반 Build에서 Infinite Difficulty UI가 숨겨졌고 기본 메뉴·Settings·Rebind 흐름이 정상 동작했다.
- Gamepad: 사용자가 장치를 보유하지 않음을 확인했다. Step 9의 Gamepad Focus·Binding 표시 검증은 장치 미보유 사유로 제외한다.

## Step 10. Phase 3 결과를 기록하고 Phase 4로 인계한다

### AI 작업

- Phase 3 완료 조건을 정적 검사, Unit/통합 Test, Scene 확인, 실제 장치와 일반 Build 결과에 각각 연결한다.
- 실제 Test 개수, 사용자가 수행한 Scene 변경과 수동 검증 결과, 승인된 제외 사유를 이 문서에 기록한다.
- 모든 필수 근거가 충족된 경우에만 Roadmap 006 Phase 3을 완료로 변경하고 다음 작업을 Phase 4로 갱신한다.
- Settings 영구 저장, 실제 Leaderboard와 How To Play 내용을 Phase 3 완료로 기록하지 않는다.

### 사용자 수동 작업

없음. 미확인 필수 항목이 있으면 해당 항목만 사용자에게 요청한다.

### 완료 조건

- [x] Phase 3 완료 근거와 Phase 4 인계가 문서화됐다.

# 영향 범위

Phase 3 수행 시 Rules를 제외한 Systems, Features, Runtime 코드, Input System 연결, Edit/Play Mode Test, 생산 Scene, Roadmap과 이 Task 문서가 영향을 받을 수 있다. Package·ProjectSettings·Prefab은 확정된 구현상 필요성이 확인되지 않으면 변경하지 않는다.

# 검증 내용

계획 단계에서는 Roadmap Phase 3 대상과 현재 코드·Input Action·Scene·Test 상태를 정적으로 대조했다. 실제 구현 검증은 각 Step의 Unit Test 우선 설계, Scene 정적 검사, 집중·전체 Test와 최소 수동 확인으로 수행한다.

# 검증 결과

- Settings 범위는 Master Volume, Fullscreen, Keyboard Rebind 6개(Jump, Momentum Landing, Navigate 4방향), Restore Defaults로 확정했다. Player Move Action과 Binding, 생성 Wrapper 및 생산·Test 참조를 제거했다. Submit과 Cancel은 고정 UI 입력으로 유지했다.
- 생산 Scene의 Settings·Leaderboard·HUD·Pause·Stage Result·Infinite Result UI 및 Inspector 연결은 사용자가 구성했고, 정적 Scene 구성 Test로 확인했다.
- 사용자 보고 기준 Unity Script Compilation 성공, 예상치 못한 Error/Warning 없음, Edit Mode 692개 및 Play Mode 226개 전체 성공을 확인했다.
- 사용자는 `1920×1080`, `1280×720`에서 Settings·Leaderboard 가독성과 Focus를 확인했고, Keyboard/Mouse Settings 흐름과 일반 Player Build의 Difficulty 비표시 및 기본 흐름을 확인했다.
- Gamepad는 장치 미보유로 수동 검증에서 제외했다.
- Quit 버튼은 일반 Player Build에서 종료 요청과 정리 로그가 정상이나 창 닫힘이 Alt+F4보다 느리게 관찰됐다. 정적 검사에서 `Application.Quit()` 이전의 대기·저장·비동기·종료 거부 로직은 없었으며, 해당 관찰은 후속 품질 점검 항목으로 유지한다.

# 후속 작업

Roadmap 006 Phase 4의 How To Play, 첫 플레이 안내 및 전체 UI 흐름 회귀를 수행한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/CODING_STYLE.md`
- `AI/01_Rules/EVENT_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/LOGGING_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_006.md`
- `AI/02_Systems/GameSystem.md`
- `AI/02_Systems/UIManagementSystem.md`
- `AI/02_Systems/UIInputSystem.md`
- `AI/02_Systems/PlayerInputSystem.md`
- `AI/03_Features/GameEntryNavigation.md`
- `AI/03_Features/GamePause.md`
- `AI/03_Features/Settings.md`
- `AI/03_Features/Leaderboard.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_6/20260918_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_6/20260920_01_Phase2ManualSteps.md`

# 작성 완료 기준

- [x] Prototype 6 Phase 3의 실제 수행 순서를 Step으로 작성했다.
- [x] AI 정적 검사·Unit Test와 사용자 수동 작업의 책임을 분리했다.
- [x] Scene 편집, Unity Test Runner와 Build는 사용자 작업으로 한정했다.
- [x] 정적으로 판정 가능한 항목을 수동 확인 목록에서 제외했다.
- [x] How To Play, 영구 저장과 실제 Leaderboard를 후속 범위로 유지했다.
