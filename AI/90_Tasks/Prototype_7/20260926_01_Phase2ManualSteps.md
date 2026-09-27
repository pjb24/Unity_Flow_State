# 작업 정보

## 작업명

Prototype 7 Phase 2 — 로컬 기록·설정 저장 계층 수동 작업 계획

## 작업 일자

20260926

## 작업 담당자

AI, 사용자

## 작업 상태

Step 1~8 및 Step 5-1 완료. Roadmap 007 Phase 2는 완료됐으며, 다음 범위는 Phase 3이다.

# 작업 목적

Roadmap 007 Phase 2에서 게임 로직과 저장 수단을 분리한 로컬 기록·설정 저장 계층을 구축한다. 수치 계산, 상태 전이, 직렬화, Migration, 손상 복구 및 중복 방지는 정적 검사와 Unity Edit Mode Unit Test로 우선 검증한다. 재실행과 실제 사용자 저장 경로처럼 실행 환경이 필요한 사항만 사용자 수동 작업으로 남긴다.

# 작업 대상

- Record Submission Service와 Memory·Local·Online Record Repository 경계
- 개인 최고 기록과 Offline 제출 대기열
- Tutorial 완료, Settings 및 Input Binding 영구 저장
- Save Version, Migration, 누락·손상·지원하지 않는 Save의 안전한 기본값 복구
- 재실행 후에도 유지되는 Run ID와 중복 제출 방지
- 저장 계층과 기존 Result·Settings·Navigation Runtime 상태의 연결
- 관련 계약 문서, 생산 코드와 Edit Mode·Play Mode Test

Unity Gaming Services Authentication·실제 온라인 제출·조회는 Phase 3 대상이며, Leaderboard 화면은 Phase 4 대상이다. Phase 1에서 확정하지 않은 Save 형식·Migration 세부 규칙은 구현 전 조사와 필요한 결정으로 처리한다.

# 작업 전 상태

- Roadmap 007의 Phase 2는 대기 상태다.
- Phase 1 Step 1~8은 완료됐다. 저장 위치·계정·기록·Version·Offline 운영 정책과 순수 정책 모델의 Unity Script Compilation 및 Edit Mode Test 716개 성공이 기록됐다.
- 현재 Run의 `ResultData`는 Runtime 전용이다. Settings, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록과 제출 대기열은 로컬 영구 저장 대상으로 확정됐으나 실제 저장 연결은 아직 없다.
- 현재 `GameNavigationState.CurrentRunId`는 Navigation 인스턴스 내에서만 증가하므로 영구 중복 제출 식별자가 아니다.
- 현재 프로젝트에는 PlayerPrefs, 파일 기반 저장, Authentication 및 Leaderboard SDK 구현이 없다.

# 조사 내용

`IMPLEMENTATION_ROADMAP_007.md`는 Phase 2에서 로컬 저장 계층과 Repository 경계를 구현하고, 직렬화·Migration·손상 복구·중복 방지는 Edit Mode Test로, 재실행 복구는 Play Mode Test로 검증하도록 정의한다. `20260924_01_Phase1ManualSteps.md`는 Phase 1 정책 확정과 Unity 검증을 완료했고, 실제 로컬 저장·Run 식별자 연결을 Phase 2로 넘겼다.

# 작업 내용

AI는 기존 코드·문서의 정적 조사, 계약·생산 코드·자동 Test 작성, 정적 대조와 실패 분석을 담당한다. 사용자는 아래에 명시된 Unity Editor의 Compile·Test Runner 실행 및 대상 플랫폼 Player 확인만 수행한다. 필요한 Scene·Inspector 변경은 AI가 대상과 설정 방법을 안내하고 사용자가 수행한다. AI는 Unity Editor, Test Runner, Player Build, Scene 편집 또는 실제 사용자 저장 파일 조작을 수행하지 않는다.

## Step 1. Phase 1 선행 정책과 구현 경계를 정적으로 확인한다

### AI 작업

1. Phase 1에서 승인된 저장 위치, 계정, Record Key, Version, Offline·삭제 정책을 계약 문서에서 확인한다.
2. `ResultData`, Settings, Input Binding, Tutorial, Navigation과 Runtime Data의 현재 생산·초기화·폐기 경로를 다시 대조한다.
3. 로컬 저장 구현만 Phase 2에 포함하고, 실제 SDK·온라인 전송·Leaderboard UI가 Phase 3~4에 남는지 확인한다.

### 사용자 수동 작업

없음. 이미 확정된 Phase 1 정책은 재결정하지 않는다. Save 형식·Migration처럼 아직 계약이 없는 선택이 실제 구현에 필요하면 AI가 대안과 영향을 정리해 해당 항목만 요청한다.

### 완료 조건

- [x] 구현할 저장 항목, 금지된 외부 서비스 범위 및 Phase 1 정책 근거가 목록화됐다.

### 수행 결과

- Phase 1 완료 기록과 Project·System·Feature 계약을 대조했다. 로컬 영구 저장 대상은 Settings 값, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록 캐시와 제출 대기열이며, 현재 Run Runtime Data와 `ResultData` 원본은 Runtime 전용이다.
- Stage는 `Cleared` 결과만 불변 Stage ID·Stage Rules Version·밀리초 Clear Time으로 제출 후보가 된다. InfiniteMode는 유효한 Total Score·점수 구성 요소·Scoring Version·Run 시간을 포함한 확정 Result만 후보가 된다. Board Key는 Stage의 Mode·Stage ID·Stage Rules Version 및 InfiniteMode의 Mode·Scoring Version으로 분리한다.
- `GameSystem`은 Stage 또는 InfiniteMode Result를 생성한 뒤 현재 `UIManagementSystem`에만 전달한다. `ResultSystem`은 저장 또는 제출을 담당하지 않는다. `RecordSubmissionPolicy`와 `RecordSubmissionQueue`는 순수 정책 모델로 존재하지만, `RecordSubmissionSystem`, Repository 및 영구 Save 연결의 생산 코드는 아직 없다.
- `SettingsSystem`은 실행 시 새 `SettingsState`를 만들며, Settings·Binding 변경을 영구 저장소에 요청하거나 복원하는 연결은 없다. 자동 Tutorial 완료 상태는 `GameNavigationState` 인스턴스의 `_hasAutomaticHowToPlayCompleted`에만 존재한다.
- `GameNavigationState.CurrentRunId`는 Run 초기화 성공 때 증가하는 메모리 값이므로 재실행 뒤에도 유지되는 영구 제출 ID가 아니다.
- `Assets/Scripts`와 `Assets/Tests`에서 PlayerPrefs, 파일·JSON 저장, `persistentDataPath`, Unity Services, Authentication 및 Leaderboard SDK 직접 참조를 정적으로 검색했으며 발견하지 못했다.
- 따라서 Phase 2에는 로컬 Save·Repository 경계·영구 제출 ID·Settings/Binding/Tutorial 복원 연결만 포함한다. 실제 Authentication·온라인 전송/조회는 Phase 3, Leaderboard 화면은 Phase 4 범위로 유지한다.

## Step 2. 저장소 경계와 Save 계약을 구현한다

### AI 작업

1. 게임 로직이 특정 파일 형식·경로·서버 SDK에 직접 의존하지 않도록 Submission Service와 Memory·Local Repository 및 후속 Online Repository 계약을 정의한다. 실제 온라인 구현은 추가하지 않는다.
2. Save 모델에 Save Version, 후보의 계정 귀속 식별자, 개인 최고 기록, 제출 대기열, Tutorial 완료, Settings와 Binding을 정책에 맞게 포함한다. 인증 토큰·서비스 Secret·개인정보는 제외한다.
3. 영구 제출 ID와 현재 Runtime Run ID의 역할을 구분하고, 같은 Run에서 후보가 중복 생성되지 않도록 보존 범위와 거부 규칙을 구현한다.
4. 실제 파일 경로와 플랫폼 API는 Local Repository 구현 내부에만 둔다. Save 쓰기 중단 시 기존의 유효한 데이터가 손상되지 않도록 저장 절차를 정의한다.

### 사용자 수동 작업

없음. 계약과 코드의 책임 분리는 AI가 정적 검토로 처리한다.

### 완료 조건

- [x] 저장소 경계가 게임 로직과 분리되고 Save 계약이 정책과 일치한다.

### 수행 결과

- `IRecordRepository`를 공통 계약으로 두고, `ILocalRecordRepository`와 향후 온라인 구현용 `IOnlineRecordRepository`를 분리했다. `MemoryRecordRepository`는 개인 최고 기록과 Pending 후보를 메모리에서 관리하며, `RecordSubmissionService`는 후보를 대기열에 넣은 뒤 더 나은 개인 최고 기록만 갱신하도록 Repository 경계를 통해 요청한다.
- `LocalSaveData`는 Save Version, 후보의 계정 귀속 ID, `LocalSettingsData`, Tutorial 완료 상태, 개인 최고 기록과 Pending 제출 후보를 소유한다. 현재 Save Version은 `1`이다. 인증 토큰, 서비스 Secret, 개인정보와 Runtime `ResultData` 원본은 포함하지 않는다.
- 저장 형식은 승인된 UTF-8 JSON 단일 파일로 확정했다. 실제 Local Repository는 `Application.persistentDataPath/flow-state-save.json`만 사용하며, 같은 디렉터리의 임시 파일에 완성된 내용을 쓴 뒤 교체해야 한다. 쓰기·교체가 실패하면 기존 유효 Save를 유지하며, 실제 직렬화·파일 교체·Migration·손상 복구 구현은 Step 3에서 수행한다.
- 영구 제출 ID는 `RecordSubmissionCandidate.SubmissionId`를 사용하며, 후보 생성 뒤 재시도·재실행 중 변경하지 않는 계약을 유지한다. `GameNavigationState.CurrentRunId`는 Save 계약에 포함하지 않는다.
- GameSystem, ResultSystem, SettingsSystem, Scene 및 외부 SDK에는 직접 의존 또는 변경을 추가하지 않았다. 실제 Authentication·온라인 전송은 Phase 3, Leaderboard UI는 Phase 4 범위로 유지한다.

## Step 3. 직렬화·복구·제출 대기 규칙을 Unit Test로 구현한다

### AI 작업

1. 정상 Save의 직렬화·역직렬화, 지원하는 구버전의 Migration, 누락 필드, 손상 데이터 및 지원하지 않는 Version의 안전한 복구를 생산 코드와 Edit Mode Unit Test로 구현한다. 복구 시 보존하거나 초기화하는 항목은 계약에 명시한다.
2. 개인 최고 기록의 갱신, Record Key 분리, 같은 Run 중복 후보 거부, 영구 제출 ID 유지, Offline 대기열 추가·제거·재시도 상태를 생산 코드 호출로 Test한다.
3. 저장 중단·부분 파일·반복 로드/저장과 이전 유효 Save 보존을 격리된 임시 저장소 또는 대역으로 Test한다. 실제 사용자 저장 파일은 Test 대상에 넣지 않는다.
4. Test에 독립적인 직렬화·정렬·중복 판정 로직을 복제하지 않고, 외부 Scene·프레임·네트워크·실제 사용자 파일에 의존하지 않는지 정적으로 확인한다.

### 사용자 수동 작업

없음. 자동 판정 가능한 데이터·상태 규칙은 수동 조작으로 검증하지 않는다.

### 완료 조건

- [x] Save 복구, Migration, 쓰기 중단, 최고 기록, 대기열과 중복 방지의 정상·거부·경계 사례가 Edit Mode Unit Test에 연결됐다.

### 수행 결과

- `LocalSaveJsonCodec`은 Version 1 Save를 UTF-8 JSON으로 직렬화·역직렬화한다. Settings·Binding Override, Tutorial 완료, 계정 귀속 ID, 개인 최고 기록 및 Pending 후보를 보존하며 Runtime `ResultData`, 인증 토큰, Secret과 개인정보는 직렬화하지 않는다.
- Version `0` 또는 Version 필드가 없는 기존 Save는 Version 1의 안전한 기본값으로 Migration한다. 손상 JSON, 지원하지 않는 미래 Version, 유효하지 않은 Binding 및 후보는 기본값 또는 해당 항목 제외로 복구한다. 현재 Version의 누락 Settings는 기본 Volume `100`, Windowed 상태와 빈 Binding Override로 복구한다.
- `PersistentLocalSaveFileStore`는 `Application.persistentDataPath`의 승인된 Save 파일과 같은 디렉터리에 임시 파일을 완성한 뒤 `File.Replace` 또는 최초 `File.Move`로 교체한다. 쓰기 실패 시 기존 Save를 덮어쓰지 않고 Warning을 남긴다.
- `LocalSaveJsonCodecTests` Edit Mode Fixture를 추가했다. 정상 직렬화·역직렬화, Version 0 Migration, 손상·지원하지 않는 Version 복구, 대역의 쓰기 실패 시 기존 Save 보존 및 손상 Save 로드 시 기본값 복구를 생산 코드 호출로 검증한다.
- 기존 `RecordSubmissionPolicyTests`는 개인 최고 기록 정책, Pending 대기열, 동일 Player ID·Submission ID 중복 거부와 재시도 상태를 이미 생산 코드로 검증한다. 이번 변경에서 해당 규칙을 Test 코드에 복제하지 않았다.

## Step 4. Settings·Binding·Tutorial 저장 연결과 Play Mode Test를 구현한다

### AI 작업

1. 시작 시 저장 상태를 적용하고 변경 시 정책에 맞게 저장 요청하는 연결을 구현한다.
2. Settings, Input Binding 및 Tutorial 완료 상태가 기존 Runtime 책임과 충돌하지 않는지 정적으로 확인한다.
3. 실제 생산 구성의 초기화·저장·재생성 뒤 상태 복구를 검증할 Play Mode Test를 작성한다. 프로세스 종료 후 재시작 자체와 플랫폼 저장 경로는 Step 7에서 확인한다.

### 사용자 수동 작업

없음. 실행 흐름의 자동 판정은 Play Mode Test로 처리한다.

### 완료 조건

- [x] Settings·Binding·Tutorial·개인 최고 기록의 저장·재생성 후 복구를 검증하는 생산 경로 기반 Play Mode Test가 작성됐다.

### 수행 결과

- `GameSystem`은 Boot에서 `LocalRecordRepository`로 Save를 읽고, Player·UI Input 초기화 뒤 Settings·Binding 및 Tutorial 완료 상태를 기존 Runtime 상태에 적용한다. Settings 변경과 자동 Tutorial 완료 뒤에는 현재 Runtime Settings와 Tutorial 상태를 Save로 다시 요청한다.
- `SettingsSystem`은 복원용 `LocalSettingsData` 생성·적용 경계를 제공한다. 복원 Binding은 기존 정의·충돌 검증을 거쳐 Player 또는 UI Input Action에 적용하며, 저장 상태 적용 자체는 재저장 이벤트를 발생시키지 않는다.
- `LocalRecordRepository`는 로드된 개인 최고 기록과 Pending 후보를 `MemoryRecordRepository`에 복원한다. 기존 Runtime `ResultData`는 로컬 Save에 포함하거나 복원하지 않는다.
- `LocalPersistenceIntegrationTests` Play Mode Fixture를 추가했다. 새 Runtime SettingsSystem과 Navigation 상태를 생성해 저장된 Volume과 Tutorial 완료 상태가 재생성 뒤 적용되는지 생산 코드 호출로 검증한다. `LocalSaveJsonCodecTests`에는 Repository 로드 뒤 개인 최고 기록을 조회하는 사례를 추가했다.
- Scene·Inspector 변경은 필요하지 않다. 실제 프로세스 종료·재시작 및 대상 플랫폼 저장 경로 확인은 Step 7 범위로 유지한다.

## Step 5. 문서·코드·Test 정적 대조를 수행한다

### AI 작업

1. Project 문서의 저장 범위, 관련 Feature·System 문서의 책임, 코드와 Test 이름을 대조한다.
2. 저장 계층에서 SDK 직접 참조, 비밀값 저장, Runtime 결과의 중복 소유, 정책과 다른 Record Key가 없는지 정적 검색한다.
3. 변경된 asmdef 참조와 Test의 생산 코드 참조를 정적으로 검사하고, 집중 및 회귀 Test 범위를 지정한다.

### 사용자 수동 작업

없음. 문서·코드·참조 관계는 정적 검증으로 처리한다.

### 완료 조건

- [x] 문서·구현·Test의 책임과 참조 방향이 일치하고, 정적 검사에서 범위 외 SDK·Scene 의존이 발견되지 않았다.

### 수행 결과

- `PersistentLocalSaveFileStore`만 `Application.persistentDataPath`, `System.IO` 및 `JsonUtility`를 직접 사용한다. GameSystem, ResultSystem, SettingsSystem과 정책 모델에는 파일 경로·파일 API·PlayerPrefs 직접 참조가 없다.
- Phase 2 변경 파일과 Test에서 Unity Services, Authentication, Leaderboard SDK, Secret·Token 저장 및 Scene API 직접 참조를 발견하지 못했다. 변경된 asmdef는 없으며, 새 Edit Mode·Play Mode Test는 실제 `LocalSaveJsonCodec`, `LocalRecordRepository`, `SettingsSystem` 및 `GameNavigationState`를 호출한다.
- Runtime `ResultData`는 `LocalSaveData`·JSON Save에 포함되지 않는다. Board Key와 Scoring Version은 기존 `RecordSubmissionCandidate`·`RecordBoardKey` 계약을 그대로 사용하며, `CurrentRunId`를 영구 제출 ID로 사용하지 않는다.
- 그러나 현재 `GameSystem`은 Result를 `UIManagementSystem`에만 전달하고 `RecordSubmissionService`를 호출하지 않는다. 제출 후보 생성에는 불변 Stage ID와 후보 계정 귀속 ID가 필요하지만, 현재 Runtime 생산 경로에는 두 값의 공급원이 없다. 따라서 RecordSubmissionSystem 문서의 Result 입력과 현재 생산 코드가 아직 일치하지 않는다.
- 위 미결 연결은 Phase 2 범위이므로 Step 5를 완료 처리하지 않는다. Stage ID 공급 위치와 로컬 후보 계정 귀속 ID의 생성·보존 규칙을 확정한 뒤 제출 후보 생성·저장 연결과 관련 Test를 추가해야 한다.

### 완료 결과

- Step 5-1에서 확정 Result를 `RecordSubmissionService`와 `LocalRecordRepository`에 연결한 뒤 다시 대조했다. `GameSystem`은 `Cleared` Stage와 유효 InfiniteMode Result만 후보 생성 정책에 전달하며, `Fell`·중복 Runtime 결과·유효하지 않은 후보는 저장하지 않는다.
- 후보 저장 뒤 Repository의 개인 최고 기록과 Pending 스냅샷을 새 Local Save에 반영한다. Runtime `ResultData` 원본은 Save에 포함하지 않는다.
- `RecordSubmissionSystem`과 `RecordSubmission` 문서는 Phase 2 로컬 UUID 계정 귀속과 GameSystem Result 연결 경계를 반영했다. SDK·Secret·Token·Scene 직접 의존은 새 생산 코드에서 발견하지 못했다.
- 새 Edit Mode Test는 Repository 후보 중복 거부·Save 스냅샷 보존과 현재 Pattern의 최대 Collectible 수 계산을 생산 코드 호출로 다룬다. Unity Compile과 Test Runner 실행은 Step 6으로 남긴다.

## Step 5-1. 제출 후보 식별자와 Result 연결의 미결 계약을 구현한다

### AI 작업

1. 일반 Stage의 불변 `stageId`와 `stageRulesVersion`을 Runtime 결과 생성 경로에 제공할 책임 위치를 조사한다. Scene 이름·경로·Build Index를 사용하지 않고, Stage 구성 데이터 또는 명시적 Stage 정의에서 값을 공급하도록 계약·생산 코드를 정리한다.
2. 실제 Anonymous Authentication 이전에도 후보를 로컬 대기열에 귀속할 수 있는 로컬 후보 계정 ID의 생성·보존·초기화 규칙을 제안하고, 필요한 정책 결정을 사용자에게 요청한다. 인증 토큰·Secret·개인정보 또는 `CurrentRunId`를 계정 ID로 사용하지 않는다.
3. 확정된 Stage ID·Rules Version·로컬 계정 ID와 현재 `ResultData`를 사용해 `RecordSubmissionService`가 Stage의 `Cleared` 결과 및 유효 InfiniteMode 결과만 후보로 만들고 `LocalRecordRepository`에 보존하도록 연결한다. `Fell` 결과, 중복 후보, 유효하지 않은 Version과 계정 귀속 불일치는 거부한다.
4. 후보 생성 뒤 개인 최고 기록과 Pending 대기열이 갱신되고 Save 요청에 포함되는지 구현한다. Result 생성·점수 계산·Leaderboard 조회·실제 온라인 전송 책임은 추가하지 않는다.
5. 생성된 후보가 Run 재시작이나 앱 재생성 뒤에도 동일 `SubmissionId`를 유지하고, 같은 후보가 중복 생성되지 않는지 Edit Mode Test를 작성한다. Result → 후보 → Local Repository 연결은 필요한 경우 Play Mode Test를 작성한다.
6. RecordSubmissionSystem·RecordSubmission Feature·SettingsSystem·Project 문서와 생산 코드·Test의 책임 및 참조 방향을 다시 정적 대조한다. SDK·Scene·Secret·Runtime Result 이력 직접 저장이 없는지 확인한다.

### 사용자 수동 작업

1. AI가 제시하는 로컬 후보 계정 ID 정책을 승인한다. 권장안은 최초 로컬 실행 때 UUID v4를 한 번 생성해 Local Save의 `AccountId`에만 보존하고, 로컬 데이터 초기화 때 함께 삭제하는 방식이다. 이 값은 Phase 3의 Anonymous Authentication Player ID와 같다고 가정하지 않으며, 다른 계정으로 귀속된 Pending 후보를 자동 전송하지 않는다.

Unity Editor, Test Runner, Build, Scene 또는 Inspector 작업은 이 Step에 포함하지 않는다. 자동 판정 가능한 후보 생성·중복 방지·복원 규칙은 AI가 생산 코드와 Test로 처리한다.

### 완료 조건

- [x] 불변 Stage ID·Rules Version과 로컬 후보 계정 ID의 공급·보존 계약이 확정돼 있다.
- [x] 확정 Result가 정책에 맞는 후보·개인 최고 기록·Pending 대기열로 연결되고, 같은 Run의 중복 후보가 거부된다.
- [x] 후보·대기열·개인 최고 기록이 Save와 재생성 복원에 포함되며, 관련 Edit Mode·필요한 Play Mode Test가 작성돼 있다.
- [x] 문서·구현·Test 정적 대조에서 Result 저장 중복, SDK·Scene 직접 의존 및 Secret 저장이 발견되지 않는다.

### 선행 조건

- 사용자 정책 승인: 로컬 후보 계정 ID의 생성·보존·초기화 규칙

### 조사 결과

- `RuntimeDataSystem.md`는 현재 Stage ID를 Runtime Data의 예시로 언급하지만, 실제 `GameRuntimeData`, `RuntimeDataSystem`, `StageSystem` 및 `GameSystem`에는 Stage ID 또는 Stage Rules Version 필드·생성 경로가 없다.
- 현재 일반 Stage는 하나의 Stage Mode Root와 Goal을 사용하며, Stage 구분에 Scene 이름·경로·Build Index를 사용할 수 없다. 불변 ID는 명시적 Stage 구성 데이터 또는 코드 기본값으로 제공해야 한다.
- `RecordSubmissionPolicy`는 Stage 후보 생성에 Player ID, UUID v4 Submission ID, Stage ID와 Rules Version을 모두 요구한다. Phase 3 Authentication 전에는 후보 계정 귀속 ID의 생성·보존 정책이 추가로 필요하다.
- Unity Editor·Build·Test Runner·Scene·Inspector 수동 작업은 조사 단계에 필요하지 않았다.

### 수행 결과

- 현재 단일 Stage는 명시적 코드 기본값 `stage-001`과 Stage Rules Version `1`을 사용한다. Scene 이름·경로·Build Index는 후보 식별에 사용하지 않는다.
- 최초 Local Save에 Account ID가 없으면 UUID v4를 생성해 보존한다. 이 로컬 후보 계정 ID는 Phase 3 Anonymous Authentication Player ID와 같다고 가정하지 않으며, 로컬 데이터 초기화 시 함께 삭제된다.
- GameSystem은 확정 Result 생성 직후 후보를 한 번만 생성한다. Stage는 `Cleared`만, InfiniteMode는 Result 구성·Run 시간·직렬화된 고정 속도·현재 Pattern의 최대 Collectible 수(`20`)를 반영한 상한 검증을 통과한 경우만 `RecordSubmissionService`에 전달한다.
- LocalRecordRepository는 Memory Repository의 개인 최고 기록과 Pending 후보 스냅샷을 Save에 반영한다. 동일 후보의 재저장 요청은 Pending 대기열에서 거부된다.
- `LocalSaveJsonCodecTests`에 후보 저장·중복 거부·Save 보존 사례를, `InfiniteCollectibleLayoutTests`에 현재 Pattern 최대 Collectible 수 사례를 추가했다. Test 실행은 Step 6에서 수행한다.

### 후속 관계

- Step 5-1 완료 뒤 Step 5의 문서·코드·Test 정적 대조를 다시 수행한다.
- Unity Script Compilation 및 Test Runner 실행은 Step 6, 대상 플랫폼 저장 경로와 실제 프로세스 재시작 확인은 Step 7에서 수행한다.

## Step 6. Unity Script Compilation과 자동 Test를 확인한다

### AI 작업

1. 사용자에게 실행할 집중 Edit Mode Fixture, 관련 전체 Edit Mode 및 필요한 Play Mode 회귀 범위를 지정한다.
2. 전달받은 Test 실패 이름·메시지·Stack Trace와 예상하지 않은 Console Error/Warning을 분석해 수정 범위를 제시한다.

### AI 수행 결과

- 정적 검증으로 `git diff --check` 오류는 없었다. Phase 2 변경 Runtime·Test에서 Unity Services, Authentication, Leaderboard SDK, PlayerPrefs, Secret·Token 직접 참조는 발견하지 못했다. `PersistentLocalSaveFileStore`의 `Application.persistentDataPath`·`System.IO` 사용은 로컬 파일 저장 경계로서 의도된 참조다.
- 집중 Edit Mode Fixture는 `LocalSaveJsonCodecTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`, `InfiniteCollectibleLayoutTests`, `InfinitePatternGeometryTests`, `PlayerMovementMathTests`, `GameNavigationStateTests`, `SettingsSystemTests`다.
- 집중 Fixture 성공 뒤 전체 Edit Mode를 실행한다. Phase 2 저장·기록·설정의 공용 계약 및 이번 고정 속도·Pattern 산식 변경의 회귀를 함께 확인하기 위함이다.
- 관련 Play Mode 회귀 범위는 `LocalPersistenceIntegrationTests`, `GameEntryBootIntegrationTests`, `GameLifecycleIntegrationTests`, `StageGoalIntegrationTests`, `InfiniteModeIntegrationTests`, `AutoMovementIntegrationTests`, `InfiniteCollectibleLayoutIntegrationTests`, `InfinitePatternTraversalIntegrationTests`, `InfinitePatternConnectionIntegrationTests`, `SettingsPanelIntegrationTests`, `HowToPlayIntegrationTests`다.
- Unity Script Compilation과 Test Runner는 실행하지 않았다. Scene·Inspector 수동 설정도 필요하지 않다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 결과와 예상하지 않은 Error/Warning 유무를 확인한다.
2. AI가 지정한 집중 Edit Mode Fixture를 Unity Test Runner에서 실행한다.
3. AI가 변경 영향에 따라 지정한 전체 Edit Mode 및 관련 Play Mode 회귀를 실행한다.
4. 각 실행의 시도·성공·실패 개수와 예상하지 않은 Error/Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 함께 전달한다.

### 완료 조건

- [x] 지정한 Compile·Edit Mode·Play Mode Test가 모두 성공하고 예상하지 않은 Error/Warning이 없다.

### 완료 결과

- 2026-09-27 사용자 확인: Unity Script Compilation이 성공했고 예상하지 않은 Console Error·Warning이 없었다.
- Edit Mode Test는 679개를 시도해 모두 성공했으며, 관련 예상하지 않은 Error·Warning이 없었다.
- Play Mode Test는 230개를 시도해 모두 성공했으며, 관련 예상하지 않은 Error·Warning이 없었다.

## Step 7. 대상 플랫폼 Player의 저장 경로와 파일 수명을 확인한다

### AI 작업

1. 구현된 저장 위치와 예상되는 생성·갱신·보존 조건, 생산 화면에서 확인할 수 있는 데이터 항목을 사용자에게 제공한다.
2. 사용자가 보고한 결과가 Save 계약 및 자동 Test 결과와 일치하는지 분석한다.

### AI 수행 결과

- 저장소는 `Application.persistentDataPath/flow-state-save.json` 단일 UTF-8 JSON 파일을 사용한다. 임시 파일 `flow-state-save.tmp`에 먼저 쓴 뒤, 최초 저장은 이동하고 기존 파일 갱신은 교체한다. 파일 내용·임시 파일은 사용자가 열거나 조작할 필요가 없다.
- 현재 Project Settings의 Company Name은 `DefaultCompany`, Product Name은 `Unity_Flow_State`다. 따라서 권장 대상인 Windows Standalone Player의 예상 경로는 `%USERPROFILE%\AppData\LocalLow\DefaultCompany\Unity_Flow_State\flow-state-save.json`이다.
- 최초 Player 실행 때 Account ID가 없으면 로컬 UUID와 기본 Settings·Tutorial 상태가 저장된다. Settings 값·Binding 변경은 즉시 저장 요청되고, Automatic How To Play에서 Start Run을 선택하면 Tutorial 완료 상태가 저장된다. 유효한 Stage Clear 또는 InfiniteMode 결과 후보도 개인 최고 기록·Pending 대기열과 함께 저장 요청된다.
- 다음 Player 실행 시 Settings·Binding·Tutorial 완료 상태와 로컬 기록 스냅샷을 복원한다. 실제 온라인 전송·Leaderboard 조회는 이 Step의 확인 대상이 아니다.

### 사용자 수동 작업

1. 권장 대상 플랫폼은 Windows Standalone이다. Unity Editor에서 직접 Player를 Build해 실행하고, 사용한 플랫폼과 Build 결과를 알린다.
2. Player의 Settings 화면에서 Volume 또는 Fullscreen을 변경하고, 가능하면 Binding 하나를 변경한다. 새 저장 상태에서는 Mode Select에서 Automatic How To Play가 열릴 때 Start Run을 한 번 선택한다. Stage Clear 또는 InfiniteMode 종료로 기록 생성을 시도하되, 생산 화면에서 확인할 수 없는 기록 항목은 그 이유만 기록한다.
3. Player를 정상 종료한 뒤 `%USERPROFILE%\AppData\LocalLow\DefaultCompany\Unity_Flow_State\flow-state-save.json`의 생성 또는 수정 시각 갱신과 종료 뒤 파일 보존 여부만 확인한다. 파일 내용을 열거나 직접 수정·손상·삭제하지 않는다.
4. 같은 Player를 다시 실행한다. 변경한 Settings·Binding과 Tutorial 완료 상태가 유지되는지 확인한다. 기록은 생산 화면에서 표시되는 항목이 있는 경우에만 확인한다.
5. 수행한 플랫폼, Build 결과, 파일 생성·갱신·보존 결과, 재실행 복원 결과 및 예상하지 않은 Error/Warning을 전달한다.

수치·상태 전이·중복 방지·손상 복구·삭제 규칙은 자동 Test로 판정한다. 대상 플랫폼에서만 드러나는 저장 경로, 파일 생성·갱신·보존 및 실제 프로세스 재시작 후 복구를 수동 확인한다. Scene·Inspector 연결이 추가로 필요하면 AI가 정확한 대상과 설정값을 제시한 뒤 사용자가 적용한다.

### 완료 조건

- [x] 대상 플랫폼 Player에서 저장 파일의 생성·갱신·보존과 프로세스 재시작 후 접근 가능한 상태의 복구를 확인했다.

### 완료 결과

- 2026-09-27 사용자 확인: Windows 플랫폼 Player Build가 성공했다.
- Settings에서 변경한 상태는 Player 종료 후 재실행해도 유지됐다.
- Automatic How To Play가 표시된 상태에서 Start Run을 선택한 뒤 재실행하면 해당 안내가 다시 표시되지 않았다.
- Player 종료 후 `flow-state-save.json` 파일이 생성되고 보존된 것을 확인했다.

## Step 8. Phase 2 완료 근거와 후속 범위를 정리한다

### AI 작업

1. Roadmap 007 Phase 2의 완료 조건을 정책 근거, 정적 검사, Edit Mode·Play Mode Test 및 대상 플랫폼 확인 결과에 연결한다.
2. 미결 정책, 실패한 Test 또는 미확인 대상 플랫폼 항목이 있으면 Phase 2를 완료 처리하지 않는다.
3. 완료 조건 충족 시 Roadmap 상태와 Task 기록을 갱신하고, Phase 3의 Authentication·온라인 제출 범위를 분리한다.

### 사용자 수동 작업

없음. Step 6~7의 근거가 부족한 경우에만 해당 미충족 작업을 특정해 요청한다.

### 완료 조건

- [x] Phase 2의 저장·복구·대기열·중복 방지·자동 Test·대상 플랫폼 근거가 모두 기록됐다.

### 완료 결과

- Phase 1 정책과 Phase 2 구현을 대조해 로컬 Save Version·Migration·손상 복구, 개인 최고 기록·Pending 대기열, UUID 후보 식별자와 동일 Run 중복 거부의 근거를 기록했다.
- 정적 대조에서 게임 로직의 외부 SDK·PlayerPrefs·Secret·Token 직접 의존과 Scene 직접 의존을 발견하지 못했다.
- 2026-09-27 사용자 확인으로 Unity Script Compilation 성공, Edit Mode 679개 성공, Play Mode 230개 성공 및 예상하지 않은 Error·Warning 없음이 확인됐다.
- 2026-09-27 사용자 확인으로 Windows Player Build 성공, `flow-state-save.json` 생성·보존 및 Settings·Automatic How To Play 상태의 프로세스 재시작 후 복원이 확인됐다.
- `IMPLEMENTATION_ROADMAP_007.md`의 Phase 2를 완료로, 다음 작업을 Phase 3 Authentication과 온라인 Leaderboard 제출·조회 연결로 갱신했다. 실제 Authentication, 온라인 전송·조회와 Leaderboard UI는 Phase 2에 추가하지 않았다.

# 영향 범위

이번 작업은 `AI/90_Tasks/Prototype_7`의 Phase 2 실행 계획을 현재 Phase 1 완료 상태에 맞게 갱신한다. 후속 구현에서는 Project·System·Feature 문서, Runtime 코드, asmdef와 Edit Mode·Play Mode Test가 변경될 수 있다. 실제 온라인 서비스와 Leaderboard UI는 Phase 3~4 범위다.

# 검증 내용

- 각 Phase 2 구현 대상과 완료 조건을 Step 1~8에 대응시킨다.
- 직렬화, Migration, 손상 복구, 최고 기록, 대기열과 중복 방지는 Edit Mode Unit Test를 우선한다.
- 생산 구성의 저장·재생성 후 복구는 Play Mode Test를 우선하고, 실제 프로세스 재시작과 대상 플랫폼의 경로·파일 수명만 수동으로 확인한다.
- 사용자 수동 작업은 Unity Editor의 Compile·Test Runner와 대상 플랫폼 Player 확인으로 한정한다.

# 검증 결과

Phase 2의 로컬 저장·복구·대기열·중복 방지 구현과 검증이 완료됐다. Unity Script Compilation, Edit Mode 679개, Play Mode 230개 및 Windows Player의 저장 파일·재실행 복구 확인을 근거로 Roadmap 007 Phase 2를 완료로 기록한다. 실제 Authentication·온라인 제출·조회는 Phase 3, Leaderboard UI는 Phase 4 범위로 유지한다.

# 후속 작업

Roadmap 007 Phase 3의 Authentication과 온라인 Leaderboard 제출·조회 범위를 별도 작업으로 시작한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/ResultSystem.md`
- `AI/02_Systems/SettingsSystem.md`
- `AI/03_Features/TimeRecord.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/Leaderboard.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_7/20260924_01_Phase1ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 수동 작업을 Compile·Test Runner·대상 플랫폼 Player 확인으로만 구분했다.
- [x] 정적 검증과 Edit Mode·Play Mode Unit Test 우선 원칙을 명시했다.
- [x] 자동 판정 가능한 수치·상태 규칙을 수동 작업에서 제외했다.
- [x] Phase 2와 Phase 3~4의 책임 경계를 분리했다.
- [x] 구현·Test·플랫폼 검증 미실행 상태를 Phase 2 완료로 기록하지 않았다.
