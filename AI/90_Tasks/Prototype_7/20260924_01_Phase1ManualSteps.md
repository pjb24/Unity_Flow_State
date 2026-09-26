# 작업 정보

## 작업명

Prototype 7 Phase 1 — 저장 범위와 Leaderboard 운영 정책 수동 작업 계획

## 작업 일자

20260924

## 작업 담당자

AI, 사용자

## 작업 상태

Step 1~8 정적 조사, 정책 결정, 문서 계약, 순수 정책 구현, Unity 검증과 완료 근거 정리를 완료했다. Roadmap 007 Phase 1은 완료 상태다.

# 작업 목적

Roadmap 007 Phase 1에서 영구 저장 대상, 기록 구조, 순위와 계정·Offline 운영 정책을 확정한다. 사용자는 제품·운영 선택만 수행하고, 기존 계약 조사와 결정 가능한 규칙의 판정은 정적 검사 및 Edit Mode Unit Test로 처리한다.

# 작업 대상

- 로컬·서버·Runtime 전용 데이터의 경계
- Player 식별, Anonymous 계정과 기기 변경·복구 정책
- Stage별 Clear Time 및 InfiniteMode Total Score 기록과 순위 정책
- 동점, Player별 최고 기록, Scoring Version·Game Version 및 기록 초기화
- Offline 제출 대기·중복 방지·재시도·서비스 장애·삭제 정책
- Client Score 신뢰 수준과 서버 검증 범위
- 관련 Project·Feature·System 문서, 순수 정책 모델과 Edit Mode Unit Test

로컬 저장소 구현, Unity Gaming Services 연결, 실제 인증·전송, Leaderboard 화면, Scene 편집과 Player Build는 Phase 1 대상이 아니다.

# 작업 전 상태

- Roadmap 005·006은 완료됐다. Roadmap 007 Phase 1~4는 대기 중이다.
- `PROJECT_OVERVIEW.md`, `ARCHITECTURE.md`, `PROJECT_MEMORY.md`에는 현재 버전의 로컬·서버 저장 제외와 Runtime 전용 데이터 규칙이 남아 있다. Phase 1 결정 후 해당 범위를 갱신해야 한다.
- `TimeRecord`는 Stage의 `Cleared` 또는 `Fell` 결과를 확정한다. Stage Leaderboard에 제출할 결과 원인은 별도 정책으로 확정해야 한다.
- `ScoreRecord`는 InfiniteMode의 Total Score와 Scoring Version을 확정한다. 서로 다른 Scoring Version의 기록을 비교하지 않는 규칙이 이미 있다.
- 현재 `ResultData`에는 Stage ID가 없다. `GameNavigationState.CurrentRunId`는 현재 Navigation 인스턴스의 Run 시작마다 증가하며 영구 제출 ID 계약은 없다. Stage 구분 Key와 재시도 간 유지되는 제출 식별자는 Phase 1에서 정의하고 구현은 후속 Phase에 배치한다.
- Prototype 6 Phase 4는 자동 안내·재열람·Binding 표기와 전체 회귀를 완료했다. Tutorial 완료 상태, Settings 및 Binding의 영구 저장과 실제 Leaderboard는 Roadmap 007에 남겨 두었다.

# 조사 내용

`AI/README.md`의 Project·Rules 확인 순서, Roadmap 007, Prototype 6 Phase 4 완료 기록, `TimeRecord.md`, `ScoreRecord.md`, `Leaderboard.md`, `ResultSystem.md`와 현재 `ResultData`·Navigation 상태를 확인했다. `INVESTIGATION_RULE.md`와 `VERIFICATION_RULE.md`에 따라 확정 정책과 미정 정책을 구분하고, 자동 판정 가능한 상태·정렬·중복 규칙은 Unit Test 대상으로 배치한다.

# 작업 내용

AI는 제안·장단점, 정적 조사, 계약 문서, 순수 정책 코드와 Unit Test를 담당한다. 사용자는 아래 Step에서 명시한 정책 선택과 Unity Editor의 Compile·Test Runner 실행만 담당한다. AI는 Unity Editor, Test Runner 또는 Build를 실행하지 않는다.

## Step 1. 저장·기록 경계를 정적으로 조사한다

### AI 작업

1. Runtime Data, `ResultData`, Settings·Input Binding·Tutorial 상태와 기록 생성 경로를 코드·문서에서 목록화한다.
2. 현재 Stage ID, Player ID, Game Version, 제출 ID와 영구 저장 수단의 존재 여부를 검색하고, 확인된 것과 미구현 항목을 구분한다.
3. 기존 저장 제외 문구와 Roadmap 007의 변경 대상, Phase 2~4로 넘길 구현 범위를 대조한다.

### 사용자 수동 작업

없음. 파일·코드·문서로 판정할 수 있는 사항은 AI가 조사한다.

### 완료 조건

- [x] 저장 후보와 기존 생산 데이터 원천, 누락된 식별자 및 문서 충돌 목록이 작성됐다.

### 수행 결과

- `GameRuntimeData`는 Mode별 Runtime Data, InfiniteMode Score와 Collectible Score를 생성하고 `RuntimeDataSystem`이 Run 종료 시 제거한다.
- `ResultSystem`은 현재 Run의 `ResultData`만 보유한다. `GameSystem`은 일반 Stage에서 `Cleared` 또는 `Fell`, 경과 시간과 Collectible Score를 전달하고, InfiniteMode에서 Scoring Version과 최종 Score 구성 요소를 전달한다.
- `SettingsState`와 `SettingsSystem`은 Volume, Fullscreen 및 Input Binding Override를 Runtime 상태로만 관리한다. 자동 How To Play 완료 상태도 `GameNavigationState` 인스턴스에만 존재한다.
- `PlayerPrefs`, 파일 기반 저장, `persistentDataPath`, Authentication, Unity Services, Leaderboards API, Player ID, Stage ID, Game Version 및 영구 제출 ID 구현은 `Assets/Scripts`와 `Assets/Tests` 정적 검색에서 발견되지 않았다.
- `ResultData`에 Stage ID, Player ID, Game Version 및 제출 ID는 없다. `CurrentRunId`는 `GameNavigationState` 인스턴스의 Run 시작마다 증가하므로 재실행 후에도 유지되는 제출 식별자가 아니다.
- `ScoringVersion.Current`는 `2`이며, 기존 ScoreRecord 계약은 서로 다른 Scoring Version 기록의 비교를 금지한다.
- `PROJECT_OVERVIEW.md`, `ARCHITECTURE.md`, `PROJECT_MEMORY.md`의 Runtime 전용·로컬 저장 제외·서버 저장 제외 규칙은 Roadmap 007의 Phase 1 저장 정책 확정 후 갱신해야 하는 문서 충돌이다.
- 로컬 저장소는 Phase 2, 실제 Authentication 및 온라인 제출·조회는 Phase 3, Leaderboard UI와 Build 검증은 Phase 4 범위로 유지한다.

## Step 2. 저장 대상과 계정 정책을 결정한다

### AI 작업

1. 데이터 항목별 `Runtime 전용 / 로컬 영구 / 서버` 배치안을 제시한다. Settings 값·Input Binding·Tutorial 완료·개인 최고 기록·제출 대기열·계정 식별 정보·Run 결과를 빠짐없이 포함한다.
2. Anonymous Authentication의 최초 생성, 재설치·기기 변경 시 기록 접근, 계정 연결·복구 지원 여부와 사용자에게 표시할 제한을 선택지와 장단점으로 제시한다.
3. 불필요한 개인정보·Secret 저장을 요구하는 안이 없는지 계약과 대조한다.

### 사용자 수동 작업

1. AI의 데이터 항목별 표를 보고 각 항목의 저장 위치와 보존 여부를 결정한다.
2. Anonymous 계정만 사용할지, 계정 연결·복구를 제공할지, 기기 변경·재설치 시 기존 기록을 복구할 수 없는 경우 어떤 안내를 할지 결정한다.
3. 결정 내용과 남겨 둘 예외를 채팅으로 전달한다. Unity Editor나 서비스 Console 작업은 하지 않는다.

### 완료 조건

- [x] 로컬·서버·Runtime 전용 범위와 Anonymous 계정의 복구 제한이 확정됐다.

### 수행 결과

- Settings 값, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록 캐시와 제출 대기열은 로컬 영구 저장 대상으로 확정했다.
- 현재 Run Runtime Data와 `ResultData` 원본은 Runtime 전용으로 유지한다. 전체 Result 이력은 저장하지 않으며, 제출 후보와 개인 최고 기록만 영구 보존한다.
- 계정에 귀속된 온라인 최고 기록과 순위는 서버 저장 대상으로 확정했다. 로컬 개인 최고 기록 캐시는 Offline 표시를 위한 값이며 서버 기록과 일시적으로 다를 수 있다.
- 온라인 Leaderboard 조회 또는 첫 제출 요청 시 Anonymous 계정을 생성한다. 인증 실패는 Offline 플레이, Result 확인 또는 Menu 진행을 차단하지 않는다.
- 초기 버전은 Anonymous 계정 연결·복구를 지원하지 않는다. 재설치 또는 기기 변경 뒤 기존 온라인 기록을 복구할 수 없다는 제한은 첫 온라인 제출·조회 전 안내하고, 이후에도 Settings에서 언제든지 확인할 수 있게 한다.
- 이메일, 실명, 전화번호, 임의 표시명을 수집·저장하지 않는다. 인증 토큰과 서비스 Secret은 앱 저장소 또는 제출 대기열에 저장하지 않고 서비스 SDK가 관리한다.
- 현재 `Assets/Scripts`와 `Assets/Tests`에는 위 저장·인증 구현이 없음을 정적 검색으로 재확인했다. 로컬 저장소 구현은 Phase 2, 실제 Authentication·온라인 저장은 Phase 3, 복구 제한 안내 UI는 Phase 4 범위로 유지한다.

## Step 3. 제출 기록과 순위 정책을 결정한다

### AI 작업

1. Stage 제출 가능 원인(`Cleared`/`Fell`), Stage 구분 Key, Clear Time 저장 단위·반올림 기준, InfiniteMode 제출 가능한 최종 결과와 Total Score 필드를 확인해 선택지로 제시한다.
2. Stage별 Clear Time 오름차순, InfiniteMode Total Score 내림차순의 확정 기준 위에서 동점의 순위·표시 순서, Player별 최고 기록 교체 조건, 동일 점수의 재제출 처리안을 장단점과 함께 제시한다.
3. 기록 Key에 들어갈 Mode·Stage·Scoring Version·Game Version의 역할과 규칙 변경 시 새 Leaderboard 분리 또는 초기화 정책을 제시한다. 서로 다른 Scoring Version을 비교하지 않는 기존 계약은 유지한다.

### 사용자 수동 작업

1. Stage 제출 원인, Stage ID 형식, 시간 정밀도와 반올림, InfiniteMode 제출 결과 범위를 결정한다.
2. 동점 순위와 표시 순서, Player 최고 기록 교체·동점 재제출 규칙을 결정한다.
3. Score 규칙 및 Game Version 변경 시 기존 기록 보존·분리·초기화 정책을 결정하고 채팅으로 전달한다. 실제 기록이나 서버 데이터를 만들거나 삭제하지 않는다.

### 완료 조건

- [x] 두 Mode의 Record 구조·순위·동점·최고 기록·Version 정책이 모순 없이 확정됐다.

### 수행 결과

- 일반 Stage는 `Cleared` 결과만 Leaderboard 제출 후보로 인정한다. `Fell` 결과는 Result 표시만 하고 개인 최고 기록, 제출 대기열과 온라인 순위에 넣지 않는다.
- Stage 구분 Key는 Scene 이름·경로·Build Index와 독립적인 불변 문자열 ID를 사용한다. 형식은 `stage-001`과 같이 정하고, 새 Stage에 명시적으로 부여한다.
- Stage 순위 값은 원본 경과 시간을 가장 가까운 밀리초로 반올림한 정수 `clearTimeMilliseconds`이며 오름차순으로 비교한다. 표시 형식은 `mm:ss.fff`를 사용한다.
- InfiniteMode는 종료와 점수 구성 요소 검증이 모두 완료된 Result만 제출 후보로 인정한다. 순위 값은 `TotalScore`이며 내림차순으로 비교하고, 점수 구성 요소와 Scoring Version은 검증·표시용 스냅샷으로 보존한다.
- 동일한 Stage Clear Time 또는 InfiniteMode Total Score는 서버 수락 시각 오름차순으로 보조 정렬한다. 보조 정렬 값도 같으면 공동 순위로 처리하며, 다음 순위는 공동 순위 인원 수만큼 건너뛰는 competition ranking을 사용한다. 제출 ID는 중복 방지용이며 순위 강제 분리에는 사용하지 않는다.
- Player별 최고 기록은 Stage의 더 짧은 시간 또는 InfiniteMode의 더 높은 Total Score일 때만 교체한다. 동일 값 재제출은 기존 최고 기록과 최초 수락 순서를 유지한다.
- Board Key는 규칙 버전으로만 분리한다. Stage는 `mode`, 불변 `stageId`, `stageRulesVersion`을, InfiniteMode는 `mode`, `scoringVersion`을 사용한다. `gameVersion`은 기록 메타데이터로만 보존하고 일반 앱 업데이트는 Board를 분리하지 않는다.
- Score 또는 시간 비교 규칙이 바뀌면 새 규칙 Version Board를 만들고 기존 Board는 보존한다. 특히 서로 다른 InfiniteMode Scoring Version 기록은 순위·최고 기록·동점 판정을 수행하지 않는다.
- 현재 `TimeRecord`가 `Cleared`와 `Fell`을 모두 결과로 확정하고 `ScoreRecord`가 유효한 InfiniteMode 종료·Total Score·Scoring Version을 검증하는 기존 계약을 정적으로 확인했다. Stage ID, 규칙 Version, 영구 제출 ID와 실제 Board 연결은 후속 Phase 구현 범위로 유지한다.

## Step 4. Offline·운영·신뢰 정책을 결정한다

### AI 작업

1. Offline 또는 Authentication·서비스 실패 시 로컬 Result 유지, 제출 대기, 재시도 트리거·횟수·영구 실패 처리와 같은 Run의 중복 전송 방지안을 제시한다.
2. 제출 ID의 생성 시점·재실행 후 유지 범위, 대기열 보존·삭제·계정 전환 시 귀속 규칙을 제시한다. 현재 `CurrentRunId`만으로 영구 중복 방지가 가능한지 정적으로 판정한다.
3. 로컬·서버 기록 삭제, 계정 초기화·복구, Client Score 신뢰 수준과 서버에서 검증할 최소 필드를 제시한다. 강화된 부정행위 방지의 후속 범위도 분리한다.

### 사용자 수동 작업

1. Offline 기록의 보존·자동/수동 재시도·포기 조건과 서비스 장애가 Run 결과 또는 Menu를 막지 않는 정책을 결정한다.
2. 중복 제출 방지, 계정 변경 시 대기 기록 처리, 기록 삭제 요청과 계정 복구 범위를 결정한다.
3. Client가 계산한 기록을 어느 수준까지 신뢰할지와 Phase 3 서비스 연결 시 필요한 검증·운영 범위를 결정해 채팅으로 전달한다. 네트워크 차단 실험이나 실제 서비스 설정은 하지 않는다.

### 완료 조건

- [x] Offline·재시도·중복 방지·삭제·복구·신뢰 정책이 확정됐고 플레이 차단 금지 조건이 명시됐다.

### 수행 결과

- 제출 후보는 생성 즉시 로컬 대기열에 보존한다. Offline, Authentication 실패, Timeout과 서비스 오류는 Run 결과 확인, Menu 진행 또는 Offline 플레이를 차단하지 않으며 후보를 `Pending`으로 유지한다.
- 자동 재시도는 앱 시작, 온라인 복구와 인증 성공에서 수행하고, 사용자는 수동 Retry도 요청할 수 있다. 하나의 재시도 계기에서는 지수 백오프로 최대 세 번 시도한 뒤 다음 재시도 계기까지 대기한다.
- 서버가 기록 유효성을 거부한 후보는 `Rejected`로 종료하고 자동 재시도하지 않는다. 네트워크·인증·서비스 실패는 영구 실패로 바꾸지 않는다.
- 제출 후보가 만들어지는 순간 UUID v4 제출 ID를 한 번 생성한다. 해당 ID는 로컬 대기열에 저장하며 재시도와 재실행에서도 변경하지 않는다. 서버 중복 판정은 동일 Player ID와 제출 ID 조합을 사용한다.
- 대기 후보는 생성 당시 계정에 귀속한다. 현재 인증 계정이 다르면 다른 계정으로 전송하지 않고 보류하며, 명시적인 로컬 데이터 초기화에서만 삭제한다.
- 사용자가 로컬 데이터를 초기화하면 Settings, Input Binding, Tutorial 완료 상태, 개인 최고 기록 캐시와 제출 대기열을 삭제한다. 온라인 순위와 서버 기록은 유지됨을 초기화 전과 Settings에서 명확히 고지한다. 초기 버전에는 온라인 개별 기록 또는 계정 삭제를 제공하지 않는다.
- Client가 계산한 기록을 기본 신뢰하되 서버는 인증 Player ID, Board Key·규칙 Version, 제출 ID 중복, 제출 가능 결과, 시간·점수 범위, Score 구성 합계를 검증한다. 서버는 실제 플레이 입력이나 물리를 재현하지 않는다.
- InfiniteMode 제출에는 Run 시간과 Scoring Version 규칙 데이터를 함께 포함한다. 서버는 Run 시간, 최대 Momentum Landing 배율, 최대 이동 속도, 시간 내 가능한 최대 Pattern 수 및 Pattern별 가능한 최대 Collectible 수를 조합해 계산한 논리적 최대 Total Score를 상한으로 사용한다. 이 상한을 넘거나 상한 계산에 필요한 값이 누락·불일치하면 거부한다.
- 위 상한은 Scoring Version별 불변 규칙 데이터로 계산한다. 점수·이동·Pattern·Collectible 규칙이 변경되면 새 Scoring Version Board와 새 상한을 사용하며 기존 Version의 상한을 소급 변경하지 않는다.
- 현재 `ResultData`에는 InfiniteMode Run 시간이 없고 영구 제출 ID·대기열·Authentication·온라인 제출 구현도 없다. Run 시간과 규칙 상한 입력의 생산·저장 연결은 Phase 2, 실제 서버 검증은 Phase 3 범위로 유지한다.

## Step 5. 확정 정책을 문서 계약과 Test 명세로 반영한다

### AI 작업

1. 확정된 저장 범위만 Project 문서에 반영하고, 기록·Leaderboard 동작 규칙은 해당 Feature 문서에 반영한다. Result 생성과 저장·제출의 System 책임을 분리한다.
2. 결정마다 정상·거부·경계 사례를 작성한다. 기록 Key 분리, 정렬·동점, 최고 기록 갱신, Version 불일치, 동일 제출 ID, Offline 대기·재시도를 Edit Mode Unit Test 항목에 연결한다.
3. Stage ID·영구 제출 ID·실제 저장소·SDK·UI 연결 가운데 Phase 2~4 구현 대상과 현재 Phase 1 순수 정책 대상의 경계를 명시한다.

### 사용자 수동 작업

없음. 사용자가 Step 2~4에서 승인한 결정은 AI가 문서와 Test 명세로 반영한다.

### 완료 조건

- [x] Project·Feature·System 문서가 확정 정책과 일치하고, 모든 결정 가능한 규칙에 Test 사례가 연결됐다.

### 수행 결과

- Project 문서에서 Runtime·로컬 영구·서버 데이터의 소유 경계를 확정 정책에 맞게 갱신했다. 기존 Runtime 전용·로컬 저장 제외·서버 저장 제외 문구를 제거했다.
- `RecordSubmission` Feature와 `RecordSubmissionSystem` 문서를 추가해 제출 후보 규칙과 Result 생성, 저장·대기열·온라인 제출 책임을 분리했다. ResultSystem은 Result Data 생성·제공만 담당하고 저장·제출을 담당하지 않는다.
- Settings와 Binding Override는 로컬 영구 저장 정책으로 갱신했고, SettingsSystem은 저장 수단을 소유하지 않는 적용·저장 요청 경계로 정의했다.
- Leaderboard는 Stage의 `Cleared` 밀리초 Clear Time, InfiniteMode의 유효 Total Score, Board Key·Version 분리, 동점 보조 정렬·competition ranking과 Player별 최고 기록 유지 규칙을 반영했다.
- 다음 Edit Mode Unit Test 명세를 확정했다.
  - Stage 후보: `Cleared`와 유효 불변 Stage ID·Rules Version·밀리초 시간이 후보가 되고, `Fell`, 누락·지원하지 않는 Version과 유효하지 않은 시간은 거부한다.
  - Infinite 후보: 유효 종료·Scoring Version·점수 구성 합계·Run 시간이 후보가 되고, 구성 합계·Version·논리적 최대 Total Score 상한의 정상·경계·초과·누락 사례를 판정한다.
  - Board·순위: Mode·Stage ID·Rules/Scoring Version 분리, Stage 오름차순·Infinite 내림차순, 서버 수락 시각 보조 정렬, 완전 동점의 competition ranking을 판정한다.
  - 최고 기록: Stage의 더 짧은 시간과 InfiniteMode의 더 높은 점수만 교체하고 동일 값은 기존 기록을 유지한다.
  - 제출 상태: 동일 Player ID·Submission ID 거부, Pending·Submitted·Rejected 전이, 계정 귀속 불일치, 재시도 계기별 최대 세 번과 영구 실패가 아닌 오류의 Pending 보존을 판정한다.
- Stage ID, 영구 제출 ID, Run 시간, 규칙 상한 입력, 실제 저장소·SDK·UI 연결은 각각 Phase 2~4 구현 대상이며, 이번 Step에서는 계약과 Test 명세만 작성했다.

## Step 6. 순수 정책 모델과 Edit Mode Unit Test를 작성한다

### AI 작업

1. 확정 정책 중 순수하게 계산·판정 가능한 Record 유효성, Board Key, 정렬·동점, 최고 기록 선택, Version 분리, 동일 제출 ID 거부와 Offline 제출 상태 전이를 필요한 최소 모델로 구현한다.
2. 실제 생산 모델을 호출하는 Edit Mode Unit Test로 정상·거부·경계값을 검증한다. Test 안에 생산 정렬·계산 로직을 복제하지 않는다.
3. Scene·프레임·외부 SDK·네트워크·실제 파일에 의존하지 않는지, 기존 기록 생성과의 책임 중복이 없는지 정적으로 검사한다.

### 사용자 수동 작업

없음. 코드·Test 작성과 정적 검사는 AI가 수행한다.

### 완료 조건

- [x] 확정된 순수 정책의 생산 코드와 대응 Unit Test가 작성되고 정적 대조를 통과했다.

### 수행 결과

- `RecordBoardKey`, `RecordSubmissionCandidate`, `RecordSubmissionPolicy`, `InfiniteScoreLimit`, `RecordSubmissionQueue`와 `RecordLeaderboardPolicy`를 순수 Runtime Feature 모델로 작성했다. 모델은 Stage 후보·밀리초 변환, InfiniteMode Score 구성·상한, Board 분리, 정렬·공동 순위, 최고 기록, 제출 ID 중복 및 Pending·Submitted·Rejected 상태를 생산 코드에서 판정한다.
- Stage 후보는 `Cleared`와 UUID v4 제출 ID, 불변 Stage ID·Rules Version, 유효 시간을 요구한다. InfiniteMode 후보는 Current Scoring Version, Run 시간, 일치하는 Score 구성·최대 배율과 Version별 논리적 최대 Total Score를 요구한다.
- `InfiniteScoreLimit`은 Run 시간, 최대 속도, 거리당 Score, 최대 배율, 최소 Pattern 길이, Pattern별 최대 Collectible과 Collectible Score로 상한을 계산한다. 상한 입력이 누락·무효이거나 Total Score가 상한을 넘으면 후보 생성을 거부한다.
- `RecordSubmissionPolicyTests`는 Stage 정상·거부·밀리초 반올림, InfiniteMode 상한 경계·초과·Version·구성 불일치, UUID v4, 동일 제출 ID, 재시도 계기별 세 번 제한, Rejected 종료와 계정 귀속 불일치를 검증한다.
- `RecordLeaderboardPolicyTests`는 Stage 오름차순, InfiniteMode 내림차순, 서버 수락 시각 보조 정렬, 완전 동점 competition ranking, Board 분리와 Mode별 엄격한 개인 최고 기록 갱신을 검증한다.
- 새 모델과 Test에는 UnityEngine, MonoBehaviour, Scene, 실제 파일, SDK·Authentication·Leaderboard API 의존이 없음을 정적 검색으로 확인했다. 새 Asset meta GUID 충돌이 없고 `git diff --check`를 통과했다.

## Step 7. Unity Script Compilation과 자동 Test를 확인한다

### AI 작업

1. 코드·asmdef·문서·Test 이름과 변경 범위를 정적으로 검사하고, 집중 Edit Mode Fixture와 영향받는 전체 회귀 범위를 지정한다.
2. 사용자가 전달한 실패 Test 이름·메시지·Stack Trace를 분석해 수정한 뒤 해당 범위를 다시 지정한다.
3. 실제 실행 결과와 예상하지 않은 Error/Warning 유무만 완료 근거로 기록한다.

### 사용자 수동 작업

1. Unity Editor에서 Script Compilation 성공과 예상하지 않은 Error/Warning 유무를 확인한다.
2. AI가 지정한 집중 Edit Mode Fixture를 Unity Test Runner에서 실행한다.
3. 공통 Core·Record 계약 변경의 영향 범위에 따라 AI가 지정한 전체 Edit Mode 및 필요한 Play Mode 회귀를 실행한다.
4. 각 실행의 시도·성공·실패 개수와 예상하지 않은 Error/Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 전달한다.

Phase 1에서 Scene과 생산 플레이 경로를 변경하지 않은 경우 화면 확인과 Player Build는 요구하지 않는다. 정적 검사나 Unit Test로 판정할 수 있는 수치·상태를 수동 조작으로 중복 검증하지 않는다.

### 완료 조건

- [x] 최신 변경 기준 Script Compilation과 지정된 자동 Test가 모두 성공하고 예상하지 않은 Error/Warning이 없다.

### 수행 결과

- 사용자가 Unity Editor에서 최신 변경 기준 Script Compilation 성공을 확인했고 예상하지 않은 Error와 Warning이 없다고 보고했다.
- 사용자가 Unity Test Runner Edit Mode 전체 회귀 716개를 실행해 모두 성공했고 예상하지 않은 Error와 Warning이 없다고 보고했다. 전체 실행에는 `RecordSubmissionPolicyTests`와 `RecordLeaderboardPolicyTests`가 포함된다.
- 이번 변경은 순수 정책 모델과 Edit Mode Test만 추가했으며 Scene·생산 플레이 경로를 변경하지 않았다. 따라서 Play Mode Test, 화면 확인과 Player Build는 Step 7 완료 조건에 포함하지 않았다.

## Step 8. Phase 1 완료 근거와 후속 범위를 정리한다

### AI 작업

1. Roadmap 007 Phase 1의 각 완료 조건을 확정 정책, 정적 대조와 실제 Test 결과에 연결한다.
2. 결정하지 않은 항목이나 실패한 검증이 있으면 완료 처리하지 않는다. Phase 2의 로컬 저장 구현, Phase 3의 실제 서비스, Phase 4의 UI·Build 작업을 구분한다.
3. 필수 근거가 모두 충족되면 Phase 1과 Roadmap 진행 상태를 갱신하고 검증 결과를 Task 영역에 기록한다.

### 사용자 수동 작업

없음. 추가 Unity 작업이 필요한 경우에만 Step 7의 미충족 항목을 특정해 요청한다.

### 완료 조건

- [x] 저장·계정·순위·Version·Offline·운영 정책과 Unit Test 근거가 모두 기록되고 Phase 1 상태가 갱신됐다.

### 수행 결과

- 저장 대상·계정 복구 제한·Board/Version 분리·정렬/공동 순위·Offline 재시도·로컬 초기화와 서버 기록 유지·InfiniteMode 논리적 점수 상한 정책을 Project·Feature·System 문서와 이 Task에 기록했다.
- 순수 정책 모델과 Edit Mode Unit Test가 후보 유효성, 상한, Board 분리, 순위, 최고 기록, 중복 제출, 재시도와 계정 귀속을 검증한다.
- 사용자가 Unity Script Compilation 성공 및 예상하지 않은 Error/Warning 없음, Edit Mode Test 716개 전체 성공 및 예상하지 않은 Error/Warning 없음을 확인했다.
- Phase 2의 실제 로컬 저장, Phase 3의 Authentication·온라인 제출/조회, Phase 4의 UI·Player Build는 아직 수행하지 않았으며, Roadmap 007 Phase 1만 완료로 갱신했다.

# 영향 범위

이번 작업은 `AI/90_Tasks/Prototype_7`에 Phase 1 실행 계획을 추가한다. 정책 승인 후 Project·Feature·System 문서와 순수 정책 코드·Test가 변경될 수 있다. Scene, 외부 서비스 설정과 실제 사용자 데이터는 이 계획에서 변경하지 않는다.

# 검증 내용

- Roadmap 007 Phase 1의 구현 대상·완료 조건을 Step 1~8에 대응시킨다.
- 정책 선택만 사용자 수동 작업으로 남기고, 기존 데이터·코드 조사와 결정적인 규칙 판정은 정적 검사·Unit Test로 배치한다.
- 실제 Unity 실행이 필요한 Compilation과 Test Runner만 Step 7에 배치한다.
- Phase 2~4의 저장소·서비스·UI 구현을 Phase 1 완료 조건에 포함하지 않는다.

# 검증 결과

Step 1~8을 완료했다. 확정 정책은 관련 Project·Feature·System 문서에 반영했고, 순수 정책 모델과 Edit Mode Unit Test를 추가했다. 사용자가 Unity Script Compilation 성공 및 Edit Mode Test 716개 전체 성공을 확인했으며, 두 실행 모두 예상하지 않은 Error/Warning이 없었다. Roadmap 007 Phase 1 상태를 완료로 갱신했다.

# 후속 작업

Roadmap 007 Phase 2에서 로컬 기록·설정 저장 계층을 구현한다. Phase 3의 실제 Authentication·온라인 제출/조회와 Phase 4의 UI·Player Build는 후속 범위로 유지한다.

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
- `AI/03_Features/TimeRecord.md`
- `AI/03_Features/ScoreRecord.md`
- `AI/03_Features/Leaderboard.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_6/20260923_01_Phase4ManualSteps.md`

# 작성 완료 기준

- [x] 실제 사용자 선택과 Unity 실행을 Step별 수동 작업으로 구분했다.
- [x] 정적 검사와 Edit Mode Unit Test 우선 원칙을 명시했다.
- [x] Phase 1과 후속 Phase의 책임 경계를 구분했다.
- [x] 정책·Test 미실행 상태를 완료로 기록하지 않았다.
