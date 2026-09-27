# 작업 정보

## 작업명

Prototype 7 Phase 3 — Authentication·온라인 Leaderboard 연결 수동 작업 계획

## 작업 일자

20260927

## 작업 담당자

AI, 사용자

## 작업 상태

계획 작성 완료. Phase 3 구현과 아래 Step의 완료 판정은 대기 상태다.

# 작업 목적

Roadmap 007 Phase 3의 Anonymous Authentication, 온라인 기록 제출·조회, Offline 재시도 및 검증 환경 연결을 수행한다. 문서·코드·설정 파일의 대조와 결정적인 상태·예외 판정은 정적 검사와 Unit Test로 처리한다. 사용자는 Unity Dashboard의 프로젝트·환경 설정, Unity Editor의 Compile·Test Runner 실행 및 실제 검증 환경의 서비스 연결만 수행한다.

# 작업 대상

- Unity Gaming Services(UGS) 프로젝트·환경과 Stage·InfiniteMode Leaderboard 구성
- Anonymous Authentication과 로컬 UUID 후보 귀속의 연결 정책
- Online Record Repository, 제출·조회 경계, 재시도·중복 방지
- 서버 측 제출 유효성·점수 상한 검증 수단과 운영 환경 보호
- 생산 코드, Edit Mode·Play Mode Unit Test, 관련 System·Feature 계약

Leaderboard 화면의 완성, Result의 순위·제출 상태 표시와 출시 후보 전체 회귀는 Phase 4 범위다. 검증 환경의 실제 제출·조회에 필요한 최소 실행 경로는 Phase 3에서 준비한다.

# 작업 전 상태

- Roadmap 007 Phase 1~2는 완료됐고 Phase 3은 대기 상태다. Phase 2에서 Local Save, 개인 최고 기록·Pending 대기열 및 `IOnlineRecordRepository` 계약을 만들었다.
- 현재 `Packages/manifest.json`에는 UGS Authentication·Leaderboards 패키지가 없고, `Assets/Scripts`에는 실제 인증·온라인 제출·조회 구현이 없다.
- Phase 2 후보의 `PlayerId`는 로컬 UUID v4이며, 이 값은 UGS Anonymous Authentication의 Player ID와 같다고 가정하지 않는다. 계정 귀속이 다른 후보는 자동 전송하지 않는 정책이다.
- 현재 단일 Stage의 Key는 `stage-001`과 Stage Rules Version `1`이며, 현행 InfiniteMode Scoring Version은 `2`다. 실제 Board ID는 아직 정하지 않았다.
- Phase 2 Unity Script Compilation, Edit Mode 679개와 Play Mode 230개가 성공했다. Windows Player에서 저장 파일 생성과 Settings·Tutorial 재실행 복원을 확인했다. 이 결과는 Phase 3 온라인 연결의 성공 근거로 사용하지 않는다.

# 조사 내용

- `IMPLEMENTATION_ROADMAP_007.md` Phase 3은 Authentication, Board·환경 분리, 제출·조회, Timeout·Offline·재시도와 중복 방지, 실제 검증 환경 통합을 완료 조건으로 둔다.
- `RecordSubmission.md`는 첫 온라인 요청 시 Anonymous 계정, 다른 계정 후보 전송 금지, 계기별 최대 3회 재시도, 서버 거부 시 `Rejected`, 인증 실패 시 `Pending` 보존을 정의한다.
- Unity 공식 문서에 따르면 Leaderboard는 환경별로 만들며 ID·정렬 방향·기록 갱신 전략을 설정한다. 환경을 명시하지 않은 UGS 초기화에는 기본 환경이 적용될 수 있으므로 검증 환경을 명시적으로 선택해야 한다. Anonymous 계정은 SDK의 세션 토큰을 잃으면 복구할 수 없다.
- 현재 Unity Leaderboards SDK는 점수 제출과 조회 API를 제공한다. 제출 ID의 서버 중복 판정, 수락 시각 동점 정책 및 규칙별 서버 검증을 어떤 서비스 경계에서 강제할 수 있는지는 구현 전에 확인해야 한다. 확인 전에는 클라이언트의 사전 검사만으로 서버 검증 완료를 선언하지 않는다.

# 작업 내용

AI는 계약·코드·테스트·검증 도구를 준비하고 정적 검사를 수행한다. 사용자는 아래 Step에 명시된 서비스 Console과 Unity Editor 작업만 수행한다. 서비스 계정 비밀값, 인증 토큰, 실제 사용자 저장 파일의 내용 및 운영 환경의 기록 데이터는 채팅이나 저장소에 공유하지 않는다. Unity Editor Build와 Test Runner 실행, Scene·Inspector 수정이 필요하면 사용자가 수행한다.

## Step 1. Phase 2 경계와 UGS 제공 기능을 정적으로 조사한다

### AI 작업

1. Phase 1~2 정책과 `RecordSubmissionQueue`, `LocalRecordRepository`, `IOnlineRecordRepository`, 후보·Board Key 및 asmdef를 대조한다.
2. 현재 UGS Authentication·Leaderboards의 패키지/API, 환경 선택, Board 설정, 최고 기록 갱신, 조회, 서버 측 검증 및 제출 ID 중복 처리 수단을 공식 문서와 대조한다.
3. SDK만으로 보장되지 않는 동점·중복·서버 검증 규칙과 Phase 4 UI 의존 사항을 목록화한다. 필요한 서비스 경계 또는 정책 결정을 Step 2 전에 명시한다.

### 사용자 수동 작업

없음. 문서·코드·SDK 기능 조사는 AI가 수행한다.

### 완료 조건

- [ ] 기존 계약과 서비스 기능의 대응표, 미결 정책 및 검증 가능한 구현 범위가 기록됐다.

## Step 2. 프로젝트·환경·계정 귀속 정책을 확정한다

### AI 작업

1. 현재 단일 Stage와 InfiniteMode Scoring Version을 코드에서 확인해 필요한 Board 목록, 각 Board의 정렬 방향과 Best Score 전략을 제시한다.
2. 기존 로컬 UUID 후보와 인증 Player ID의 연결안을 제시한다. 기존 후보의 영구 제출 ID를 유지하며, 계정 귀속 불일치 후보를 묵시적으로 재귀속하거나 전송하지 않는 방법을 검토한다.
3. 첫 온라인 요청 전 Anonymous 계정 복구 제한 안내와 Phase 4 화면 작업의 경계를 정한다. 안내가 준비되지 않은 생산 경로에서는 요청을 시작하지 않는 조건을 명시한다.

### 사용자 수동 작업

1. Unity Dashboard에서 이 게임에 사용할 UGS 프로젝트를 선택하거나 생성하고, 개발·검증 환경을 운영 환경과 구분한다. 필요한 개발·검증 환경이 없으면 생성한다. Phase 3 실제 시험에는 검증 환경만 사용하며 운영 환경의 설정과 기록은 변경하지 않는다.
2. AI가 제시한 기존 로컬 UUID 후보와 인증 Player ID의 처리안 중 하나를 결정한다. 다른 계정 후보의 자동 전송을 허용하는 결정은 기존 정책 변경이므로 영향 범위를 함께 검토한다.
3. AI에게 UGS Project ID, 환경 이름, 사용 가능한 환경 권한 및 결정한 후보 처리 정책만 전달한다. 서비스 계정 키·토큰·암호는 전달하지 않는다.

### 완료 조건

- [ ] 검증 환경과 비밀값이 아닌 식별 정보, 기존 후보 처리 정책, 최초 온라인 요청 안내 조건이 확정됐다.

## Step 3. 검증 환경의 Leaderboard를 구성한다

### AI 작업

1. 확정한 `RecordBoardKey`마다 충돌 없는 Board ID와 설정표를 작성한다. 현재 대상은 `stage-001` Rules Version `1`과 InfiniteMode Scoring Version `2`의 두 Board다.
2. Stage는 밀리초 Clear Time 오름차순, InfiniteMode는 Total Score 내림차순이며 Player별 Best Score만 유지하도록 설정값을 정적 대조한다. 규칙 Version이 다른 Board를 재사용하지 않도록 매핑을 검증한다.
3. Board ID·환경 이름을 코드와 설정에 연결하되 인증 토큰·서비스 Secret을 넣지 않는다.

### 사용자 수동 작업

1. Unity Dashboard에서 **검증 환경**을 선택한 뒤 AI가 제공한 설정표대로 Stage와 InfiniteMode Leaderboard를 각각 만든다. 각 Board의 ID, 정렬 방향 및 Best Score 설정을 확인한다.
2. 생성한 Board의 ID와 설정값만 AI에게 전달한다. AI가 코드 매핑과 대조할 때까지 운영 환경에 같은 Board를 만들거나 기존 Board를 초기화·삭제하지 않는다.

### 완료 조건

- [ ] 검증 환경의 Board ID·정렬·갱신 설정이 확정된 Board Key 계약과 일치한다.

## Step 4. 인증·제출·조회와 서버 검증 경계를 구현한다

### AI 작업

1. UGS 초기화 시 검증 환경을 명시하고, 첫 온라인 요청에서만 Anonymous 인증을 시작하는 경계를 구현한다. 인증 실패에도 Menu·Run·Result와 로컬 Pending 상태를 유지한다.
2. 합의된 계정 귀속 정책을 적용하고, 온라인 제출·상위 기록·내 주변 기록·내 최고 기록 조회를 Repository 경계 뒤에 구현한다.
3. 동일 Player ID·Submission ID 재시도, Board·Version 분리, 제출 가능 결과와 InfiniteMode 논리적 최대 점수의 서버 측 판정 수단을 구현한다. 서버 측 수단이 필요한 경우 AI가 배포 가능한 구성과 검증 절차를 준비한다. 서비스가 요구 규칙을 강제할 수 없으면 대안을 확정하기 전까지 Phase 3 완료로 판정하지 않는다.
4. SDK 실패·Timeout·인증 실패·서버 거부를 Pending 또는 Rejected로 분류하고, 계기별 최대 3회 지수 백오프 및 재실행 후 동일 제출 ID 보존을 연결한다.

### 사용자 수동 작업

1. AI가 서버 측 검증 구성의 배포 필요성과 정확한 대상·설정을 제시한 경우에만, Unity Dashboard 또는 안내된 Unity 도구에서 **검증 환경**에 적용한다. 적용한 서비스 구성의 이름·버전과 성공·실패 결과를 전달한다.
2. 추가 서비스 권한이 필요한 경우 대상 환경과 필요한 권한을 AI에게 알린다. 서비스 계정 비밀값은 공유하지 않는다.

### 완료 조건

- [ ] 인증·제출·조회·재시도와 서버 검증 경계가 구현되고, 검증 환경 구성과 일치한다.

## Step 5. 정적 검사와 Unit Test를 작성한다

### AI 작업

1. 패키지·asmdef 참조, Board ID·환경 매핑, SDK 의존 방향, Secret·Token 저장 여부와 운영 환경으로의 우발적 요청 경로를 정적으로 검사한다.
2. Edit Mode Unit Test에서 인증 성공·실패, 계정 불일치, Board Key 분리, 최고 기록 갱신, 동일 제출 ID, Pending·Submitted·Rejected, Timeout·Offline·서비스 거부, 재시도 계기·횟수·백오프와 재실행 복원을 Repository·SDK 대역으로 검증한다.
3. 서비스 검증 로직은 정상·경계·초과·누락·Version 불일치 사례로 Test한다. 클라이언트 Test가 서버의 실제 강제력을 증명한다고 간주하지 않는다.
4. 필요한 Play Mode Test로 생산 구성의 초기화 순서와 온라인 실패 시 게임 진행·로컬 결과 보존을 검증한다. Scene 의존 테스트가 필요하면 작성 방법과 사용자 실행 범위를 지정한다.

### 사용자 수동 작업

없음. 정적 판정과 Unit Test 작성은 AI가 처리한다.

### 완료 조건

- [ ] 정상·거부·경계·회귀 사례가 생산 코드와 서비스 경계의 자동 Test에 연결되고 정적 검사에서 범위 밖 의존이 없다.

## Step 6. Unity Script Compilation과 자동 Test를 실행한다

### AI 작업

1. 사용자에게 집중 Edit Mode Fixture, 전체 Edit Mode 및 영향받는 Play Mode 회귀 범위를 지정한다.
2. 전달받은 Compile·Test 실패와 예상하지 않은 Error·Warning을 분석·수정한다. 수정 뒤 필요한 Test 재실행 범위를 다시 지정한다.

### 사용자 수동 작업

1. Unity Editor에서 패키지 해석과 Script Compilation의 완료 상태를 확인한다. 패키지 다운로드나 프로젝트 연결이 Editor에서 요구되면 AI가 지정한 패키지·UGS 프로젝트·환경만 선택한다.
2. Unity Test Runner에서 AI가 지정한 집중 Edit Mode, 전체 Edit Mode 및 관련 Play Mode Test를 실행한다.
3. 각각의 시도·성공·실패 개수와 예상하지 않은 Error·Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 전달한다.

### 완료 조건

- [ ] Unity Script Compilation과 지정된 Edit Mode·Play Mode Test가 모두 성공하고 예상하지 않은 Error·Warning이 없다.

## Step 7. 검증 환경에서 실제 인증·제출·조회를 확인한다

### AI 작업

1. 생산 Scene 변경 없이 실행 가능한 검증 경로를 우선 준비하고, 자동 판정 가능한 호출 결과·Board ID·Player ID 일치·제출 ID 중복·조회 값을 검증 도구가 요약하도록 한다.
2. 성공·인증 실패·서비스 실패·재시도·계정 불일치 사례의 검증 순서와 기대 결과를 제공한다. 네트워크 차단처럼 사용자 환경 조작이 꼭 필요한 사례만 수동 절차로 남긴다.
3. 사용자가 전달한 검증 환경의 결과와 서비스 설정을 대조하고, 서버 측 거부·중복 방지가 실제 서비스에서 확인됐는지 별도로 판정한다.

### 사용자 수동 작업

1. AI가 제공한 검증 도구 또는 접근 가능한 생산 경로를 Unity Editor에서 실행한다. **검증 환경** 표시를 확인한 뒤 테스트용 Anonymous 계정으로 Stage·InfiniteMode의 제출과 상위·내 주변·내 최고 기록 조회를 수행한다.
2. AI가 준비한 실패·재시도 검증 절차를 따라 필요한 경우에만 네트워크 상태를 변경한다. 운영 환경이나 실제 사용자 계정의 기록에는 제출하지 않는다.
3. 각 호출의 성공·실패 요약, 대상 환경·Board ID, 재시도 후 상태, 예상하지 않은 Error·Warning을 전달한다. 토큰·Secret과 전체 Player ID는 전달하지 않는다.
4. 검증 경로에 Scene·Inspector 연결이 꼭 필요한 경우 AI가 대상 GameObject·Component·설정값을 특정한 뒤 사용자가 적용한다. AI는 Scene을 수정하지 않는다.

### 완료 조건

- [ ] 검증 환경에서 Authentication, Board별 제출·조회, Offline 복귀·재시도, 계정 불일치·중복·서버 거부가 계약대로 동작한다.

## Step 8. Phase 3 완료 근거와 후속 범위를 정리한다

### AI 작업

1. Roadmap 007 Phase 3 완료 조건을 정적 검사, Edit Mode·Play Mode Test, 검증 환경의 실제 서비스 결과에 연결한다.
2. 미결 계정 정책, 강제되지 않는 서버 검증, 실패한 Test 또는 미확인 서비스 동작이 있으면 Phase 3을 완료 처리하지 않는다.
3. 완료 조건이 충족되면 Roadmap·Task 상태를 갱신하고 Phase 4 Leaderboard UI·전체 회귀·대상 플랫폼 Build 범위를 기록한다.

### 사용자 수동 작업

없음. Step 2~3·6~7의 근거가 부족한 경우에만 미충족 항목을 특정해 요청한다.

### 완료 조건

- [ ] Phase 3의 계정·환경·Board·제출·조회·오류·중복·서버 검증과 자동 Test·실서비스 근거가 모두 기록됐다.

# 영향 범위

이번 작업은 `AI/90_Tasks/Prototype_7`에 Phase 3 실행 계획을 추가한다. 후속 Step 수행 시 관련 Project·System·Feature 문서, 패키지 설정, Runtime 코드, 테스트 및 검증 환경 서비스 구성이 변경될 수 있다.

# 검증 내용

- Roadmap 007 Phase 3의 구현 대상과 완료 조건을 Step 1~8에 대응시킨다.
- 수치·상태·오류 분류·재시도·중복 규칙은 정적 검사와 Unit Test에 배치한다.
- Unity Compile·Test Runner 실행과 검증 환경의 실제 서비스 연결만 사용자 실행 단계로 둔다.
- 운영 환경 변경과 Phase 4의 화면 완성·대상 플랫폼 Build를 Phase 3 완료 근거에 포함하지 않는다.

# 검증 결과

계획 문서의 범위와 수동·자동 검증 책임을 기존 정책·코드 및 Unity 공식 문서와 대조했다. Phase 3 구현, Unity 자동 Test 실행 및 실제 서비스 연결은 아직 수행하지 않았다.

# 후속 작업

Step 1의 기존 계약·SDK 제공 기능 정적 조사부터 수행한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/INVESTIGATION_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/02_Systems/RecordSubmissionSystem.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/03_Features/Leaderboard.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- [Unity Authentication: Anonymous sign-in](https://docs.unity.com/en-us/authentication/use-anon-sign-in)
- [Unity Services: Environments](https://docs.unity.com/en-us/services/service-environments)
- [Unity Leaderboards: Dashboard configuration](https://docs.unity.com/ugs/manual/leaderboards/manual/configuration/unity-dashboard)
- [Unity Leaderboards: SDK score submission](https://docs.unity.com/en-us/leaderboards/tutorials/unity-sdk/add-new-score)

# 관련 작업 기록

- `AI/90_Tasks/Prototype_7/20260924_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_7/20260926_01_Phase2ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 수동 작업을 서비스 환경 설정, Unity Editor 실행 및 검증 환경 연결로 특정했다.
- [x] 정적 검사와 Edit Mode·Play Mode Unit Test 우선 범위를 명시했다.
- [x] 계정 귀속 불일치와 서버 검증의 미결 문제를 선행 판정 항목으로 분리했다.
- [x] Phase 3 구현·Test·실서비스 검증 미실행 상태를 완료로 기록하지 않았다.
