# 작업 정보

## 작업명

Phase 1 Manual Steps

---

## 작업 일자

20260709

---

## 작업 담당자

AI

---

## 작업 상태

완료

---

# 작업 목적

Phase 1: 프로젝트 기본 실행 환경 구축을 사용자가 Unity에서 수동으로 수행할 때 확인해야 할 Step을 정리한다.

Roadmap에 정의된 Phase 1 목표와 완료 조건을 기준으로 수동 작업 순서를 명확하게 한다.

---

# 작업 대상

- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- Phase 1: 프로젝트 기본 실행 환경 구축
- GameSystem
- RuntimeDataSystem
- UIManagementSystem
- 기본 Scene 구성
- 프로젝트 공통 설정

---

# 작업 전 상태

IMPLEMENTATION_ROADMAP_001.md에는 Phase 1의 목표, 구현 대상, 완료 조건이 정의되어 있다.

사용자가 Unity에서 수동으로 수행할 구체적인 확인 Step은 별도 문서로 정리되어 있지 않았다.

---

# 조사 내용

아래 문서를 확인했다.

- AI/README.md
- AI/00_Project/README.md
- AI/00_Project/PROJECT_OVERVIEW.md
- AI/00_Project/ARCHITECTURE.md
- AI/00_Project/PROJECT_MEMORY.md
- AI/01_Rules/README.md
- AI/01_Rules/AI_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/01_Rules/INVESTIGATION_RULE.md
- AI/01_Rules/CODING_STYLE.md
- AI/04_Implementation_Roadmap/README.md
- AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_001.md
- AI/02_Systems/README.md
- AI/02_Systems/GameSystem.md
- AI/02_Systems/RuntimeDataSystem.md
- AI/02_Systems/UIManagementSystem.md
- AI/99_Templates/README.md
- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

확인한 기준은 아래와 같다.

- Phase 1의 목표는 프로젝트의 기본 실행 환경 구축이다.
- Phase 1의 구현 대상은 GameSystem, RuntimeDataSystem, UIManagementSystem, 기본 Scene 구성, 프로젝트 공통 설정이다.
- Phase 1의 완료 조건은 게임 실행 가능, 게임 시작부터 종료까지의 기본 흐름 존재, 각 System 정상 초기화이다.
- GameSystem은 게임 전체 실행 흐름과 현재 게임 상태를 관리한다.
- RuntimeDataSystem은 Runtime Data 생성, 제공, 제거를 담당한다.
- UIManagementSystem은 UI Configuration, UI State, 현재 활성 UI를 관리한다.
- 현재 프로젝트는 Runtime Data만 사용하며 로컬 저장과 서버 저장을 사용하지 않는다.

---

# 작업 내용

Phase 1 수동 수행 Step을 아래와 같이 구체화했다.

## 1. Unity 프로젝트 실행 확인

- Unity에서 프로젝트를 연다.
- `Assets/Scenes/SampleScene.unity`가 정상적으로 열리는지 확인한다.
- Console에 치명적인 Error가 없는지 확인한다.
- `Assets` 하위에 C# 스크립트가 없는 초기 상태인지 확인한다.

## 2. Phase 1용 폴더 생성

Unity Project 창에서 아래 폴더를 생성한다.

- `Assets/Scripts`
- `Assets/Scripts/Runtime`
- `Assets/Scripts/Runtime/Core`
- `Assets/Scripts/Runtime/Systems`
- `Assets/Scripts/Runtime/UI`
- `Assets/Scenes`

폴더 책임은 아래와 같이 사용한다.

- `Runtime/Core`: 공통 Enum, Runtime Data 같은 공통 실행 구조를 둔다.
- `Runtime/Systems`: GameSystem, RuntimeDataSystem, UIManagementSystem 스크립트를 둔다.
- `Runtime/UI`: Phase 1에서 UI State 확인에 필요한 최소 UI 스크립트를 둔다.

## 3. Core 스크립트 생성

`Assets/Scripts/Runtime/Core`에 아래 스크립트를 생성한다.

- `E_GameState.cs`
- `E_UIState.cs`
- `GameRuntimeData.cs`

각 스크립트 작성 방향은 아래와 같다.

### E_GameState.cs

- 게임 전체 상태를 나타내는 Enum을 작성한다.
- Enum 이름은 `E_GameState`를 사용한다.
- Phase 1에서 필요한 최소 상태만 포함한다.
- 포함할 상태는 초기 상태, 초기화 중, 준비 완료, 플레이 중, 종료 중, 종료 완료를 구분할 수 있어야 한다.
- 실제 플레이 규칙이나 Stage 규칙은 작성하지 않는다.

### E_UIState.cs

- 현재 활성화할 UI 상태를 나타내는 Enum을 작성한다.
- Enum 이름은 `E_UIState`를 사용한다.
- Phase 1에서 필요한 최소 상태만 포함한다.
- 포함할 상태는 UI 없음, 기본 HUD, 결과 또는 종료 확인 상태를 구분할 수 있어야 한다.
- UI 동작 규칙이나 입력 규칙은 작성하지 않는다.

### GameRuntimeData.cs

- 게임 실행 중 공유할 Runtime Data를 담는 Class를 작성한다.
- Class 이름은 파일명과 동일하게 `GameRuntimeData`를 사용한다.
- 현재 게임 상태, 현재 UI 상태, Runtime Data 생성 여부를 확인할 수 있는 값을 포함한다.
- 외부에서 마음대로 값을 바꾸지 않도록 Field는 `private`으로 두고 필요한 경우 Property나 Method로 접근한다.
- 초기화 Method를 제공한다.
- 종료 또는 제거 시 값을 초기 상태로 되돌리는 Method를 제공한다.
- 로컬 저장, 서버 저장, 파일 저장 관련 코드는 작성하지 않는다.
- 각 System의 내부 상태를 이 Class에 넣지 않는다.

## 4. Systems 스크립트 생성

`Assets/Scripts/Runtime/Systems`에 아래 스크립트를 생성한다.

- `GameSystem.cs`
- `RuntimeDataSystem.cs`
- `UIManagementSystem.cs`

각 스크립트는 하나의 Class만 정의한다.

Namespace를 사용할 경우 프로젝트 구조를 반영하여 `FlowState.Runtime.Systems`처럼 작성한다.

### RuntimeDataSystem.cs

아래 책임만 코드로 작성한다.

- `GameRuntimeData`를 생성한다.
- 생성된 Runtime Data를 보관한다.
- 다른 System이 Runtime Data를 요청할 수 있도록 제공한다.
- Runtime Data 생성 여부를 확인할 수 있게 한다.
- GameSystem의 요청을 받아 Runtime Data를 제거한다.

작성 방식은 아래 기준을 따른다.

- Unity Component로 Scene 오브젝트에 부착할 수 있도록 작성한다.
- Runtime Data 생성 Method를 Public Method로 제공한다.
- Runtime Data 제공 Method 또는 Property를 제공한다.
- Runtime Data 제거 Method를 Public Method로 제공한다.
- 이미 Runtime Data가 생성된 상태에서 다시 생성 요청이 들어오면 중복 생성하지 않도록 처리한다.
- Runtime Data가 없는 상태에서 제거 요청이 들어와도 치명적인 Error가 나지 않도록 처리한다.
- 생성, 제공, 제거 결과를 확인할 수 있는 최소 로그를 남긴다.
- 저장 기능은 작성하지 않는다.

### UIManagementSystem.cs

아래 책임만 코드로 작성한다.

- UI State를 관리한다.
- 현재 활성 UI State를 보관한다.
- UI State 변경 요청을 받는다.
- UI State 변경 결과를 Unity UI 오브젝트 활성화 상태에 반영한다.

작성 방식은 아래 기준을 따른다.

- Unity Component로 Scene 오브젝트에 부착할 수 있도록 작성한다.
- Inspector에서 Phase 1용 UI 오브젝트를 연결할 수 있는 Serialized Field를 둔다.
- 연결 대상은 기본 HUD 오브젝트와 결과 또는 종료 확인 오브젝트로 제한한다.
- 초기화 Method를 Public Method로 제공한다.
- UI State 변경 Method를 Public Method로 제공한다.
- UI State가 변경되면 연결된 UI 오브젝트의 활성화 상태를 갱신한다.
- 연결되지 않은 UI 오브젝트가 있어도 Console에서 원인을 확인할 수 있게 Warning을 남긴다.
- UI 입력 처리 코드는 작성하지 않는다.
- 결과 데이터 생성 코드는 작성하지 않는다.

### GameSystem.cs

아래 책임만 코드로 작성한다.

- 게임 시작 흐름을 시작한다.
- 현재 게임 상태를 관리한다.
- RuntimeDataSystem에 Runtime Data 생성을 요청한다.
- RuntimeDataSystem에서 Runtime Data를 받아 현재 상태를 반영한다.
- UIManagementSystem에 초기 UI State 반영을 요청한다.
- 테스트용 종료 흐름을 실행할 수 있게 한다.
- 종료 시 RuntimeDataSystem에 Runtime Data 제거를 요청한다.

작성 방식은 아래 기준을 따른다.

- Unity Component로 Scene 오브젝트에 부착할 수 있도록 작성한다.
- Inspector에서 RuntimeDataSystem과 UIManagementSystem을 연결할 수 있는 Serialized Field를 둔다.
- `Start` 또는 명시적인 시작 Method에서 초기화 흐름을 실행한다.
- 초기화 순서는 GameSystem 상태 변경, Runtime Data 생성 요청, UIManagementSystem 초기화 요청, UI State 변경 요청 순서로 구성한다.
- 현재 상태 변경은 별도 Method로 분리한다.
- 종료 흐름은 테스트할 수 있도록 Public Method 또는 Context Menu로 호출 가능하게 만든다.
- GameSystem이 Runtime Data를 직접 생성하지 않도록 한다.
- GameSystem이 UI GameObject를 직접 켜고 끄지 않도록 한다.
- PlayerInputSystem, UIInputSystem, StageSystem, ResultSystem 기능은 Phase 1에서 구현하지 않는다.

## 5. Phase 1 Scene 준비

`Assets/Scenes/SampleScene.unity`를 Phase 1 테스트 Scene으로 사용한다.

Scene에서 아래 오브젝트를 만든다.

- `GameRoot`
- `Systems`
- `UIRoot`

오브젝트 구성은 아래와 같이 한다.

- `GameRoot`: 게임 실행 진입점 역할을 맡는다.
- `Systems`: Phase 1 System 오브젝트를 묶는 부모 오브젝트로 사용한다.
- `UIRoot`: Phase 1 UI 오브젝트를 묶는 부모 오브젝트로 사용한다.

`Systems` 하위에 아래 자식 오브젝트를 만든다.

- `GameSystem`
- `RuntimeDataSystem`
- `UIManagementSystem`

`UIRoot` 하위에 아래 자식 오브젝트를 만든다.

- `StageHUD`
- `ResultPanel`

UI 오브젝트는 Phase 1 검증을 위한 최소 형태로 만든다.

- `StageHUD`는 게임 실행 중 UI State가 반영되는지 확인하는 용도로 사용한다.
- `ResultPanel`은 종료 흐름에서 UI State가 바뀌는지 확인하는 용도로 사용한다.
- 텍스트, 버튼, 디자인 구성은 Phase 1 완료 조건에 필요한 최소 수준만 둔다.

## 6. 오브젝트에 스크립트 부착

아래 기준으로 스크립트를 부착한다.

- `Systems/GameSystem` 오브젝트에 `GameSystem` 스크립트를 부착한다.
- `Systems/RuntimeDataSystem` 오브젝트에 `RuntimeDataSystem` 스크립트를 부착한다.
- `Systems/UIManagementSystem` 오브젝트에 `UIManagementSystem` 스크립트를 부착한다.

Inspector 연결은 아래와 같이 한다.

- `GameSystem` 스크립트의 RuntimeDataSystem 참조에 `Systems/RuntimeDataSystem` 오브젝트를 연결한다.
- `GameSystem` 스크립트의 UIManagementSystem 참조에 `Systems/UIManagementSystem` 오브젝트를 연결한다.
- `UIManagementSystem` 스크립트의 Stage HUD 참조에 `UIRoot/StageHUD` 오브젝트를 연결한다.
- `UIManagementSystem` 스크립트의 Result Panel 참조에 `UIRoot/ResultPanel` 오브젝트를 연결한다.

연결 후 확인할 내용은 아래와 같다.

- Inspector에 비어 있는 필수 참조가 없어야 한다.
- Play Mode 진입 시 Null Reference Error가 없어야 한다.
- GameSystem이 다른 System의 내부 Field를 직접 변경하지 않아야 한다.

## 7. 시작 흐름 작성 기준

GameSystem의 시작 흐름은 아래 순서로 작성한다.

1. 현재 게임 상태를 초기화 중 상태로 변경한다.
2. RuntimeDataSystem에 Runtime Data 생성을 요청한다.
3. 생성된 Runtime Data를 가져온다.
4. Runtime Data에 현재 게임 상태와 UI 상태를 반영한다.
5. UIManagementSystem을 초기화한다.
6. UIManagementSystem에 기본 HUD 상태 반영을 요청한다.
7. 현재 게임 상태를 준비 완료 또는 플레이 중 상태로 변경한다.
8. 각 단계가 Console에서 확인될 수 있도록 최소 로그를 남긴다.

코드 작성 시 주의할 점은 아래와 같다.

- 초기화 순서가 한 Method 안에서 지나치게 길어지면 단계별 Private Method로 나눈다.
- 실패 가능성이 있는 참조는 Null 확인을 한다.
- 참조 누락은 Error 또는 Warning 로그로 원인을 확인할 수 있게 한다.
- 확인되지 않은 Stage 시작, Player 입력, 결과 계산 로직은 작성하지 않는다.

## 8. 종료 흐름 작성 기준

GameSystem의 종료 흐름은 아래 순서로 작성한다.

1. 현재 게임 상태를 종료 중 상태로 변경한다.
2. UIManagementSystem에 결과 또는 종료 확인 UI State 반영을 요청한다.
3. RuntimeDataSystem에 Runtime Data 제거를 요청한다.
4. 현재 게임 상태를 종료 완료 상태로 변경한다.
5. 각 단계가 Console에서 확인될 수 있도록 최소 로그를 남긴다.

종료 흐름은 Phase 1 검증용으로 호출 가능해야 한다.

- Inspector Context Menu로 실행할 수 있게 하거나, 임시 테스트용 Public Method로 호출할 수 있게 한다.
- UI 버튼으로 연결하는 경우 버튼은 `ResultPanel` 표시 확인 용도로만 사용한다.
- Application 종료 코드는 Phase 1 필수 조건이 아니므로 작성하지 않아도 된다.

## 9. UI 구성 기준

`StageHUD`에는 아래 요소만 둔다.

- 현재 UI State가 HUD임을 확인할 수 있는 Text
- 현재 게임이 실행 중임을 확인할 수 있는 Text

`ResultPanel`에는 아래 요소만 둔다.

- 종료 또는 결과 상태임을 확인할 수 있는 Text
- 종료 흐름이 실행되었음을 확인할 수 있는 Text

UI 작성 시 주의할 점은 아래와 같다.

- UI 디자인 완성도는 Phase 1 범위가 아니다.
- UI 입력 수집은 Phase 1 범위가 아니다.
- 결과 데이터 표시와 클리어 타임 표시는 Phase 1 범위가 아니다.
- UIManagementSystem은 UI State에 따라 연결된 오브젝트를 활성화하거나 비활성화하는 책임만 가진다.

## 10. Play Mode 검증

- Play Mode를 실행한다.
- Console에서 GameSystem 시작 로그가 출력되는지 확인한다.
- Console에서 RuntimeDataSystem의 Runtime Data 생성 로그가 출력되는지 확인한다.
- Console에서 UIManagementSystem 초기화 또는 UI State 변경 로그가 출력되는지 확인한다.
- Scene에서 `StageHUD`가 활성화되는지 확인한다.
- Scene에서 `ResultPanel`이 시작 시 비활성 상태인지 확인한다.
- 종료 흐름을 호출한다.
- 종료 흐름 호출 후 `ResultPanel`이 활성화되는지 확인한다.
- 종료 흐름 호출 후 Runtime Data 제거 로그가 출력되는지 확인한다.
- Console에 치명적인 Error가 없는지 확인한다.

## 11. 스크립트 작성 규칙 확인

스크립트 작성 후 아래 규칙을 확인한다.

- Class 이름과 파일 이름이 동일해야 한다.
- 하나의 `.cs` 파일에는 Class를 하나만 정의한다.
- Enum 이름은 `E_` 접두어와 PascalCase를 사용한다.
- Private Field는 `_` 접두어와 camelCase를 사용한다.
- Inspector 노출 Field는 `public` Field가 아니라 `[SerializeField] private` Field로 작성한다.
- Method 이름은 PascalCase를 사용하고 동사로 시작한다.
- Null 가능성이 있는 참조는 명확하게 처리한다.
- 주석은 필요한 경우 왜 필요한지를 설명할 때만 작성한다.
- 사용하지 않는 코드와 주석 처리된 코드는 남기지 않는다.

## 12. Roadmap 상태 갱신

- Phase 1 작업을 시작했다면 IMPLEMENTATION_ROADMAP_001.md에서 Phase 1 상태를 진행 중으로 변경한다.
- Phase 1 완료 조건을 만족하면 Phase 1 상태를 완료로 변경한다.
- 현재 개발 진행 상태의 진행 중인 작업, 다음 작업, 완료된 단계를 함께 갱신한다.

## 13. 작업 기록 작성

- Phase 1 작업 완료 후 AI/90_Tasks에 작업 기록을 작성한다.
- Roadmap에는 작업 기록을 작성하지 않는다.
- 작업 기록은 Templates 영역의 적절한 Task Template을 사용한다.

---

# 영향 범위

- Tasks

---

# 검증 내용

- GENERAL_TASK_TEMPLATE.md의 주요 섹션을 기준으로 문서를 작성했는지 확인했다.
- Phase 1 수동 수행 Step이 IMPLEMENTATION_ROADMAP_001.md의 Phase 1 구현 대상과 완료 조건을 기준으로 작성되었는지 확인했다.
- System 책임 내용이 GameSystem, RuntimeDataSystem, UIManagementSystem 문서와 충돌하지 않는지 확인했다.
- 사용자가 따라할 수 있도록 폴더 생성, 스크립트 생성, Scene 오브젝트 생성, 스크립트 부착, Inspector 연결, Play Mode 검증 순서가 포함되었는지 확인했다.
- 문서에 실제 C# 코드를 직접 작성하지 않고 코드 작성 방향만 설명했는지 확인했다.

---

# 검증 결과

Phase 1 수동 수행 Step 문서를 구체화했다.

문서 내용은 확인한 Roadmap, Rules, System 문서 기준과 일치한다.

문서에는 실제 코드가 아니라 작성해야 할 코드의 책임, 구조, 흐름, 주의사항만 포함되어 있다.

사용자가 Phase 1 수동 작업을 완료했음을 확인하여 작업 상태를 완료로 변경했다.

---

# 후속 작업

Phase 2: 플레이어의 핵심 이동 구현을 수행한다.

---

# 관련 문서

## Project

- AI/00_Project/PROJECT_OVERVIEW.md
- AI/00_Project/ARCHITECTURE.md
- AI/00_Project/PROJECT_MEMORY.md

## Rules

- AI/01_Rules/AI_RULE.md
- AI/01_Rules/IMPLEMENTATION_RULE.md
- AI/01_Rules/VERIFICATION_RULE.md
- AI/01_Rules/INVESTIGATION_RULE.md
- AI/01_Rules/CODING_STYLE.md

## Systems

- AI/02_Systems/GameSystem.md
- AI/02_Systems/RuntimeDataSystem.md
- AI/02_Systems/UIManagementSystem.md

## Feature

- 없음

## Template

- AI/99_Templates/GENERAL_TASK_TEMPLATE.md

---

# 관련 작업 기록

없음

---

# 작성 완료 기준

모든 섹션을 작성했다.

확인한 문서에 근거한 내용만 작성했다.

System 작업, Feature 작업, 버그 수정 작업이 아니므로 GENERAL_TASK_TEMPLATE.md를 사용했다.
