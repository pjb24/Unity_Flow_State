# 작업 정보

## 작업명

Prototype 7 Phase 3 — Authentication·온라인 Leaderboard 연결 수동 작업 계획

## 작업 일자

20260927

## 작업 담당자

AI, 사용자

## 작업 상태

Step 1~7 및 Step 4-1 완료. 실제 verification에서 인증, Board별 제출·조회, 중복·변조·서버 거부, Player 직접 Write 차단, 계정 불일치와 Offline Pending 복구를 확인했다. Phase 3 전체 완료 판정은 Step 8에 남아 있다.

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

- [x] 기존 계약과 서비스 기능의 대응표, 미결 정책 및 검증 가능한 구현 범위가 기록됐다.

### Step 1 수행 결과 (2026-09-27)

#### 정적 대조 결과

| 기존 계약 또는 구현 | 현재 확인된 상태 | UGS 제공 기능 또는 후속 경계 |
|---|---|---|
| `RecordSubmissionPolicy`와 `RecordBoardKey` | Stage `stage-001`/Rules Version `1`과 InfiniteMode Scoring Version `2`를 서로 다른 Board Key로 만들며, 후보 생성 전에 시간·점수·Version을 검사한다. | Leaderboard ID를 Board Key에 매핑하는 설정이 필요하다. Stage는 오름차순, InfiniteMode는 내림차순과 Player별 최고 기록 전략을 Dashboard에서 설정한다. |
| `RecordSubmissionQueue`와 Local Save | 같은 로컬 후보 귀속 ID·제출 ID는 중복 저장하지 않고, Pending·Submitted·Rejected와 Retry 계기당 최대 3회 상태를 보존한다. | SDK의 `AddPlayerScoreAsync`는 플레이어 기록을 추가 또는 갱신한다. 공식 SDK 문서에는 제출 ID 기반 멱등성 판정 API가 확인되지 않았다. 동일 제출 ID의 서버 중복 방지는 별도 서버 저장·검증 경계가 필요하다. |
| `LocalRecordRepository`와 `RecordSubmissionService` | 후보와 개인 최고 기록을 Local Save에 보존한다. `RecordSubmissionService`는 로컬 저장만 호출한다. | 실제 `IOnlineRecordRepository` 구현·인증 시작·Pending 소비 경로가 없다. |
| `IOnlineRecordRepository` | `TrySubmit` 하나만 선언되어 있고, 구현체·생성·호출 지점이 없다. 상위 기록·내 주변 기록·내 최고 기록 조회 계약도 없다. | Leaderboards SDK는 `AddPlayerScoreAsync`, `GetScoresAsync`, `GetPlayerRangeAsync`, `GetPlayerScoreAsync`를 제공한다. Phase 3 구현 전에 Repository 조회 결과 모델과 비동기 실패 경계를 확장해야 한다. |
| 계정 귀속 | 후보는 Local Save의 UUID v4에 귀속된다. UGS Player ID와 같다고 가정하지 않으며 불일치 후보는 전송하면 안 된다. | Anonymous sign-in은 캐시된 세션 토큰이 있을 때만 기존 계정을 복구하고, 토큰 유실·재설치 뒤에는 복구할 수 없다. 최초 온라인 요청 전 안내가 필요하다. |
| 환경·패키지 | `Packages/manifest.json`에 UGS Authentication 및 Leaderboards 패키지가 없고, Runtime asmdef도 없다. | UGS 초기화 시 `InitializationOptions.SetEnvironmentName`으로 검증 환경을 명시해야 한다. 지정하지 않으면 Editor 선택값 또는 production이 적용될 수 있다. |
| InfiniteMode 점수 상한 | `InfiniteScoreLimit`은 시간·고정 속도·배율·Pattern·Collectible 한계로 후보를 클라이언트에서 사전 거부한다. | 이 검사는 신뢰할 수 없는 클라이언트 검사다. Cloud Code에서 같은 검증을 수행하고 Leaderboards 직접 쓰기를 Access Control로 차단해야 서버 강제로 판정할 수 있다. |

#### 미결 정책 및 제한

- Leaderboards SDK 문서에서 제출 ID를 서버가 멱등 키로 처리하거나, 수락 시각 기반 동점 규칙을 강제하는 기능은 확인하지 못했다. `SubmissionId` 중복 판정은 Cloud Code와 서버 저장 상태를 포함한 별도 설계가 필요하다.
- Leaderboards SDK의 score metadata는 최대 1 KB이지만, metadata에 제출 ID를 넣는 것만으로 서버 중복 방지나 점수 검증이 보장되지는 않는다.
- 현재 문서의 동점 표시 규칙(서버 수락 시각 오름차순, 그 뒤 competition ranking)은 Dashboard 정렬·Update Strategy만으로 강제되는지 확인되지 않았다. 구현 전에 표시 데이터와 동점 처리 책임을 확정해야 한다.
- Phase 4 UI의 Loading·Empty·Offline·Error·Retry 표현은 현재 Step 1의 구현 범위 밖이며, Phase 3에서는 조회·제출 결과 계약만 준비한다.

#### 검증 가능한 다음 구현 범위

1. 검증 환경 이름과 Board ID를 코드 설정으로 주입하고, UGS Authentication·Leaderboards 패키지를 추가한다.
2. 첫 온라인 요청에서만 명시적 검증 환경으로 UGS를 초기화하고 Anonymous sign-in을 수행하는 인증 경계를 만든다.
3. Online Repository에 제출·상위 기록·내 주변 기록·내 최고 기록의 비동기 계약과 SDK 대역 Test를 추가한다.
4. Cloud Code에서 계정·Board Key·Version·점수 상한·제출 ID를 검증하고, Access Control로 Player의 Leaderboard 직접 쓰기를 거부한다. 이 경계가 검증되기 전에는 서버 검증과 중복 방지를 완료로 판정하지 않는다.

#### Step 1 수동 작업

없음. 이 단계는 문서·소스·공식 SDK 문서의 정적 대조만으로 완료했다. Unity Editor 빌드, Unity Test Runner 실행, Scene 또는 Inspector 수정은 수행하지 않았다.

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

- [x] 검증 환경과 비밀값이 아닌 식별 정보, 기존 후보 처리 정책, 최초 온라인 요청 안내 조건이 확정됐다.

### Step 2 결정 결과 (2026-09-27)

- UGS 프로젝트는 기존 Flow State 프로젝트를 사용하고, Phase 3 실제 시험은 그 프로젝트의 `verification` 환경에서만 수행한다. Runtime UGS 초기화는 `verification` 환경명을 명시한다.
- Board Key는 다음 두 개로 확정한다. Stage는 `stage-001`/Rules Version `1`, InfiniteMode는 Scoring Version `2`다. Step 3 Board ID는 각각 `fs-stage-stage-001-r1`, `fs-infinite-v2`를 제안값으로 사용하며, 실제 Dashboard 생성 전 ID 사용 가능 여부를 확인한다.
- 로컬 UUID와 UGS Player ID는 분리한다. 사용자에게 Anonymous 계정 복구 제한을 안내하고 명시적 동의를 받은 경우에만 현재 인증 Player ID를 제출 대상으로 1회 귀속한다. 기존 후보의 Submission ID는 유지하며, 자동 재귀속·자동 전송은 허용하지 않는다.
- 복구 제한 확인 상태가 저장되기 전에는 생산 온라인 조회·제출을 시작하지 않는다. Phase 3 검증 경로는 확인 상태를 명시적으로 제공할 때에만 온라인 요청을 허용한다. 일반 사용자 안내와 Settings UI는 Phase 4 범위다.
- 서버 검증은 Cloud Code에서 수행하고, Access Control로 Player의 Leaderboard 직접 쓰기를 거부한다. 서버는 Submission ID 중복, Board·Version과 InfiniteMode 점수 상한을 검증한다.

### Step 2 사용자 수동 작업 결과

1. Unity Dashboard에서 기존 Flow State UGS 프로젝트의 `verification` 환경을 생성·확인했다. 운영 환경은 선택·수정하지 않았다.
2. 비밀값이 아닌 Project ID와 검증 환경 식별 정보를 전달했다. 서비스 계정 키·토큰·암호는 전달하지 않았다.
3. Leaderboards·Cloud Code·Access Control 설정 권한 보유와 UGS Dashboard 수동 작업 방침을 확인했다.

### Step 2 환경 식별 정보 수신 (2026-09-27)

- UGS Project ID: `c76d55cf-7846-494b-9dce-a0797b179b36`
- 검증 환경 이름: `verification`
- 검증 환경 ID: `a20a46fa-1edb-4d79-9c35-02f2fed31896`
- 운영 환경은 이 작업에서 선택하거나 수정하지 않는다.
- 사용자는 Leaderboards, Cloud Code 및 Access Control 설정 권한을 보유하며, 모든 UGS Dashboard 작업을 수동으로 수행한다.
- 검증 환경 식별 정보, 후보 귀속 정책 및 최초 온라인 요청 안내 조건이 모두 확정됐으므로 Step 2를 완료 처리한다.

## Step 3. 검증 환경의 Leaderboard를 구성한다

### AI 작업

1. 확정한 `RecordBoardKey`마다 충돌 없는 Board ID와 설정표를 작성한다. 현재 대상은 `stage-001` Rules Version `1`과 InfiniteMode Scoring Version `2`의 두 Board다.
2. Stage는 밀리초 Clear Time 오름차순, InfiniteMode는 Total Score 내림차순이며 Player별 Best Score만 유지하도록 설정값을 정적 대조한다. 규칙 Version이 다른 Board를 재사용하지 않도록 매핑을 검증한다.
3. Board ID·환경 이름을 코드와 설정에 연결하되 인증 토큰·서비스 Secret을 넣지 않는다.

### 사용자 수동 작업

1. Unity Dashboard에서 **검증 환경**을 선택한 뒤 AI가 제공한 설정표대로 Stage와 InfiniteMode Leaderboard를 각각 만든다. 각 Board의 ID, 정렬 방향 및 Best Score 설정을 확인한다.
2. 생성한 Board의 ID와 설정값만 AI에게 전달한다. AI가 코드 매핑과 대조할 때까지 운영 환경에 같은 Board를 만들거나 기존 Board를 초기화·삭제하지 않는다.

### 완료 조건

- [x] 검증 환경의 Board ID·정렬·갱신 설정이 확정된 Board Key 계약과 일치한다.

### Step 3 AI 설정표 및 정적 대조 결과 (2026-09-27)

| Board Key | Dashboard 표시 이름 | 생성할 Board ID | Sort Order | Update Strategy | 추가 설정 |
|---|---|---|---|---|---|
| Stage / `stage-001` / Rules Version `1` | `Flow State Stage stage-001 Rules v1` | `fs-stage-stage-001-r1` | Ascending (`asc`) | Keep Best (`keepBest`) | Buckets 없음, Scheduled Reset 없음, Tiers 없음 |
| InfiniteMode / Scoring Version `2` | `Flow State Infinite Scoring v2` | `fs-infinite-v2` | Descending (`desc`) | Keep Best (`keepBest`) | Buckets 없음, Scheduled Reset 없음, Tiers 없음 |

- `RecordBoardKey`는 Stage에서 Stage ID·Rules Version을, InfiniteMode에서 Scoring Version을 비교하므로 위 두 Board는 서로 재사용하거나 통합할 수 없다.
- `RecordLeaderboardPolicy`는 Stage Ranking Value를 오름차순, InfiniteMode Ranking Value를 내림차순으로 비교하고 같은 Board Key의 더 좋은 기록만 개인 최고 기록으로 교체한다. 설정표의 `asc`/`desc`와 `keepBest`는 이 규칙과 일치한다.
- `OnlineRecordConfiguration`은 `verification` 환경명과 현재 Board Key의 Board ID를 정적으로 매핑한다. UGS Project ID는 Unity Services 프로젝트 연결이 담당하며 코드에 중복 저장하지 않는다. 인증 토큰과 서비스 Secret은 코드·설정에 넣지 않는다.

### Step 3 사용자 수동 작업

1. Unity Dashboard에서 Project ID `c76d55cf-7846-494b-9dce-a0797b179b36`를 열고, Leaderboards의 환경 선택기가 `verification`인지 확인한다. `production` 환경에서는 생성·수정하지 않는다.
2. **Add Leaderboard**를 두 번 실행해 위 설정표의 Name, ID, Sort Order 및 Update Strategy를 각각 입력한다. Buckets, Scheduled Reset 및 Tiers는 설정하지 않는다.
3. 생성 뒤 Overview와 각 Board Details에서 환경 `verification`, Board ID, Sort Order 및 Update Strategy가 설정표와 같은지 확인한다. 기존 Board를 초기화·삭제하지 않는다.
4. 두 Board의 실제 ID와 각 Board의 Sort Order·Update Strategy 확인 결과만 전달한다. 서비스 계정 키·토큰·암호와 운영 환경 데이터는 전달하지 않는다.

### Step 3 완료 근거

사용자가 `verification` 환경에서 설정표와 같은 두 Board를 생성했음을 확인했다. `OnlineRecordConfiguration`과 Edit Mode Test는 현재 Stage·InfiniteMode Board Key가 각각의 실제 Board ID로만 매핑되고 다른 Stage Rules Version은 거부하도록 작성했다. Unity Test Runner 실행은 사용자 수행 단계로 남긴다.

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

- [x] 인증·제출·조회·재시도와 서버 검증 경계가 구현되고, 검증 환경 구성과 일치한다.

### Step 4 사전 정적 대조 결과 (2026-09-27)

아래는 구현 전 조사 기록이다. 패키지·asmdef·Local Save의 현재 상태는 뒤의 Step 4-1 수행 결과로 대체한다.

- `Packages/manifest.json`에는 UGS Authentication, Cloud Code 및 Leaderboards 패키지가 없으며, 실제 `IOnlineRecordRepository` 구현체·인증 시작·온라인 조회 계약도 없다.
- 확정 정책에 따라 클라이언트는 Leaderboards에 직접 점수를 쓰지 않는다. 클라이언트는 Anonymous Authentication으로 인증한 뒤 Cloud Code 제출·조회 경계만 호출하고, Cloud Code가 Leaderboards 쓰기를 수행해야 한다.
- Submission ID 중복을 서버에서 보존하려면 Cloud Code가 Player별 서버 저장소를 사용해야 한다. 검증 환경에서는 Cloud Save의 Protected Player Data를 사용하고, Player의 직접 쓰기는 Access Control로 차단한다.
- 현재 Local Save는 로컬 UUID `AccountId`만 보존한다. 명시 동의 뒤 인증 Player ID를 제출 대상으로 귀속하고, 복구 제한 확인 상태·제출 대상 Player ID·기존 Submission ID를 재실행 뒤에도 보존하도록 Save 모델과 Migration을 확장해야 한다.
- Unity Editor에서 패키지 해석과 UGS 프로젝트 연결이 완료되기 전에는 UGS SDK 의존 생산 코드를 컴파일하거나 서비스 배포 구성을 완료로 판정할 수 없다.

### Step 4 사용자 수동 사전 작업

1. Unity Editor의 **Window > Package Manager > Unity Registry**에서 `com.unity.services.authentication`과 `com.unity.services.cloudcode`를 설치한다. 각 패키지는 Unity 6000.3과 호환되는 최신 Released 버전을 선택한다. Leaderboards 클라이언트 패키지는 직접 쓰기를 하지 않으므로 이 단계에서 설치하지 않는다.
2. **Edit > Project Settings > Services**에서 기존 UGS Project ID `c76d55cf-7846-494b-9dce-a0797b179b36`에 프로젝트를 연결하고, Services 환경 선택기가 `verification`인지 확인한다. `production`을 선택하거나 수정하지 않는다.
3. Package Manager 해석과 Script Compilation이 끝난 뒤, 설치한 패키지명·버전, Services에 표시된 Project ID·환경 이름, Compile Error·Warning 유무만 전달한다. Build와 Unity Test Runner는 실행하지 않는다.

### Step 4 완료 보류 근거

2026-09-28 사용자 보고로 verification 배포 작업과 테스트 계정 ledger 초기화 완료를 접수했고, Stage 제출의 Submitted ledger를 확인했다. 남은 항목은 조회·Infinite 제출·정책 강제력·실패 복구의 검증 근거와 검증용 제한의 후속 처리다. 아래 완료 작업표에 따라 근거를 보완하며, 완료된 배포·계정 생성·초기화를 반복하지 않는다.

### Step 4 클라이언트 사전 작업 결과 (2026-09-27)

- `com.unity.services.authentication` `3.8.0`과 `com.unity.services.cloudcode` `2.10.4` 설치을 확인했다.
- Unity Services 연결 Project ID는 `c76d55cf-7846-494b-9dce-a0797b179b36`이며, 선택 환경은 `verification`이다.
- 패키지 해석과 Script Compilation에서 Error·Warning이 없음을 사용자가 확인했다.
- `UgsOnlineAuthenticationGateway`는 첫 온라인 요청에서만 호출할 Authentication 경계로 추가했다. `verification` 환경을 명시해 UGS를 초기화하고 Anonymous sign-in 성공 시에만 Player ID를 반환한다. 실패는 로컬 Pending 처리 경계가 판단할 수 있도록 인증 실패 결과로 반환한다.
- Cloud Code·Cloud Save·Access Control의 검증 환경 배포와 실제 서비스 검증은 아직 수행하지 않았다.

### Step 4 검증 환경 제출 결과 (2026-09-28)

- 사용자가 `UGS/VERIFICATION_DEPLOYMENT.md`의 verification 적용 작업을 완료했다고 보고했다. 운영 환경 변경은 보고되지 않았다.
- `OnlineRecordVerificationWindow`로 새 Anonymous Player를 만들고 Stage 기록을 제출했다. Cloud Save Protected Player Data의 `fs_submission_ledger_v1`에서 `version`은 `1`, `active`는 빈 문자열, 제출 ID는 `08d01f7c-a410-4a0b-96cc-832c06c1a2ad`, 상태는 `Submitted`로 확인됐다.
- ledger의 `best.fs-stage-stage-001-r1`와 entry는 같은 제출 ID·점수 `2287`·접수 시각 `1790568567239`를 보존한다. entry payload의 Board ID와 Rules Version `1`도 Stage 계약에 일치한다. 이는 인증 context, Cloud Code 제출 검증, Protected ledger CAS 완료 및 최고 기록 갱신 경로의 실제 검증 근거다.
- 이 보고만으로는 `query-records` 상위·내 주변·내 최고 조회, Leaderboards metadata 보존, 직접 Write Access Control 403, 거부·중복·Timeout/Offline·다른 계정 회귀, 이번 변경 뒤 Unity Script Compilation 및 Unity Test Runner 결과를 확인하지 않았다. 따라서 Step 4와 Phase 3 전체는 완료 처리하지 않는다.

### Step 4 조회 결과와 Infinite 타이머 수정 (2026-09-28)

- 사용자 보고: Stage 내 주변·내 최고 조회는 각각 1건, 순위 1, 값 2287로 성공했다. Stage 상위 조회는 `TransientFailure / Unavailable`로 실패했다. Infinite 세 조회는 모두 성공했지만 기록이 없었다. Infinite 제출 성공으로 판정하지 않는다.
- Infinite 종료 경고의 원인: GameSystem은 Stage에서만 PlayTimer를 생성하는데 Infinite 제출 후보 생성이 PlayTimer를 읽었다. TimerSystem의 누락 타이머 반환값은 0초이므로 양수 점수 후보가 시간별 점수 상한 검사에서 탈락할 수 있었다. 타이머가 종료 전에 제거됐다는 증거는 없다.
- 수정: Infinite는 `InfiniteRunTimer`를 생성·시작하고 Pause/Resume·종료·제거에 연결한다. Stage PlayTimer 사용은 유지한다. 타이머가 없는 비정상 상태에서는 -1을 반환해 후보 생성을 거부하고 경고한다. 기존 누락 시간으로 만들지 못한 후보의 Run 시간을 사후 추정하지 않는다.
- Stage 상위 조회: 기존 `Unavailable`은 인증 실패·응답 없음·호출 예외를 합친 클라이언트 결과라 원인을 확정할 수 없다. 조회 이유를 `AuthenticationUnavailable`, `EmptyResponse`, `Timeout`, `RequestFailed:<오류 코드>`, `ClientFailure`로 구분하도록 수정했다. 예외 메시지·토큰은 출력하지 않는다. 이 변경은 진단 개선이며 상위 조회 원인 해결을 입증하지 않는다.
- 테스트 작성: `InfiniteModeIntegrationTests`에 전용 Timer 생성, Pause 시간 제외, 종료 시 양수 Run 시간 후보 생성·Timer 제거·Retry 초기화를 추가했다. `OnlineRecordRepositoryTests`에 조회 Timeout 이유·후속 조회 성공, 인증 실패 시 요청 미발생을 추가했다. Unity Test Runner는 실행하지 않았다.
- 사용자 재검증: 컴파일 확인 후 Infinite를 새로 플레이해 종료 경고가 없는지 확인하고, `Pending 재시도` 후 Infinite 세 조회 결과를 전달한다. Stage 조회도 다시 실행하여 실패 시 새 reason과 해당 Warning을 전달한다. 이후 위 두 Fixture와 `GamePauseOrchestrationTests`, `GameLifecycleIntegrationTests`, `AutoMovementIntegrationTests` 등 관련 회귀를 Step 6에서 사용자 실행한다.
- 이번 수정은 클라이언트 코드와 테스트·문서에 한정된다. Cloud Code 재배포·ledger 재생성·Scene 변경·Build는 필요 없다. Step 4는 재검증 대기로 유지한다.

### Step 4 재검증 결과 (2026-09-28)

- 사용자가 이번 Infinite 타이머 변경 뒤 Unity Script Compilation 성공을 확인했다. Compile Error 및 예상하지 않은 Warning 없음 여부는 별도로 보고되지 않았으므로 확인하지 않은 사실로 남긴다.
- Infinite `fs-infinite-v2`는 상위·내 주변·내 최고 조회가 모두 성공했고, 각각 1건·순위 1·값 254를 반환했다. 기존 Stage Board와 독립적인 Infinite 제출 및 조회 경로의 실제 검증 근거다.
- Stage `fs-stage-stage-001-r1`도 상위·내 주변·내 최고 조회가 모두 성공했고, 각각 1건·순위 1·값 2287을 반환했다. 이전의 `TransientFailure / Unavailable` 보고는 사용자 정정에 따라 잘못된 정보로 기록하지 않는다.
- 두 Board의 제출·상위·내 주변·내 최고 조회 경로는 verification에서 성공했다. 현재 `CloudCodeRecordRepository`의 세분화된 실패 사유 표시는 이후 실제 오류 분석에 사용한다.
- 이후 전달된 ledger에서 Infinite `best`는 score `1051`의 Submitted receipt를 가리키고 `active`는 비어 있음을 확인했다. Leaderboard metadata 일치와 Offline 복귀 결과는 아직 남아 있어 Step 4는 미완료다.

### Step 4 완료를 위한 남은 작업 (2026-09-28)

Step 4는 구현과 검증 환경 구성의 일치를 판정한다. 테스트 작성은 Step 5, Unity 컴파일·Test Runner 실행은 Step 6, 실제 UGS 동작 검증은 Step 7에서 수행하고 그 결과를 Step 4의 근거로 연결한다. 동일 검사를 단계마다 반복하지 않는다. 이번 요청에서는 필요한 작업을 정리하며, 아래 미실행 항목을 실행 완료로 표시하지 않는다.

#### 이미 확인된 범위

- [x] 패키지 설치·프로젝트 연결·verification 환경 및 두 Board 생성에 대한 사용자 보고.
- [x] 이번 변경 뒤 Unity Script Compilation 성공 및 예상하지 않은 Error·Warning 없음.
- [x] 배포 안내서의 사용자 작업 완료 보고와 테스트 Anonymous 계정의 ledger 초기화.
- [x] Stage 제출 ID `08d01f7c-a410-4a0b-96cc-832c06c1a2ad`의 `Submitted`, `active = ""`, 최고 기록 `2287`ms(2.287초) 보존.
- [x] Step 4-1 소스·정적 검사 및 로컬 서버 대역 테스트 21개 통과 기록.

제공된 JSON은 Cloud Save ledger다. Leaderboards의 실제 행·metadata, 조회 응답, 동시 요청에서의 CAS 충돌 처리와 직접 Write 차단까지 개별 확인한 자료는 아니다. 창 실행·제출 성공은 실행 가능한 컴파일 상태의 근거이지만, Console Warning 없음이나 전체 Test 성공을 뜻하지 않는다.

#### AI가 먼저 준비·판정할 작업

| 작업 | 필요한 산출물·판정 |
| --- | --- |
| 구현·설정 최종 대조 | 제출/조회 요청·응답, 환경·Board·Version, 상태 분류·동의·귀속·저장·재시도의 코드 경로를 확인한다. 정적으로 확인 가능한 값을 사용자에게 수동 계산시키지 않는다. |
| 검증 도구 보완 | 동일 ID 서버 재호출, 같은 ID의 다른 payload, 잘못된 Version·합계·상한 초과, Player 직접 Write 403을 자동 판정하는 verification 전용 실행 경로와 사용법을 준비한다. 기존 창에는 이 기능이 없다. |
| 재시도·계정 테스트 보완 | Pending/Submitted/Rejected, 계기별 3회·1/2초 간격, 재실행 동일 ID, 인증 실패·다른 계정·저장 실패를 자동 테스트로 확인할 수 있게 한다. 토큰 삭제나 로컬 저장 파일 수동 편집을 요구하지 않는다. |
| 검증용 제한 처리 | 최초 ledger 수동 생성·128 ID 보관·100명 조회 제한이 검증 범위에서 어떻게 적용되는지 확인한다. 일반 계정 자동 초기화·보관 확장·전체 순위 처리는 해결 방안과 후속 범위를 명시한다. 기록 삭제나 단순 한도 상향으로 해결했다고 간주하지 않는다. |
| 결과 검토 및 수정 | 사용자가 전달한 컴파일·테스트·서비스 결과를 대조하고 실패 원인을 수정한다. 원격 변경이 필요할 때만 변경된 파일과 재배포 대상을 안내한다. |

기존 `Pending 재시도` 버튼은 로컬 완료 후보를 제외한다. 따라서 이미 Submitted인 후보에서 버튼을 다시 눌렀다는 사실만으로 **서버의 동일 ID 중복 처리**를 검증했다고 판정하지 않는다. 별도의 서버 재호출 검증 도구가 필요하다.

#### 사용자가 현재 도구로 수행할 작업

1. **컴파일 결과 전달:** 현재 변경을 가져온 Editor의 Console에 Compile Error 및 예상하지 않은 Warning이 있는지 알려준다. 이미 확인했다면 그 결과만 전달하면 된다. Build는 필요 없다.
2. **Stage 조회:** 기존 테스트 계정으로 Play Mode에 들어가 `Flow State > Online Record Verification`의 `Stage 상위·내 주변·내 최고 조회`를 누른다. 세 조회의 성공/실패, 건수, 표시 값을 전달한다. 이후 더 좋은 Stage 기록을 제출하지 않았다면 내 최고 값은 `2287`이어야 한다. 다른 계정 기록이 있을 수 있으므로 순위 1을 고정 기대값으로 사용하지 않는다.
3. **Infinite 제출·조회:** 같은 테스트 계정으로 Infinite 플레이 결과를 만들고 `Pending 재시도`를 누른 뒤 `Infinite 상위·내 주변·내 최고 조회`를 실행한다. `fs-infinite-v2`의 제출 완료와 조회 결과를 전달한다. 기존 Stage 기록과 분리되어 있어야 한다. 이미 수행했다면 재실행 대신 결과를 전달한다.
4. **Leaderboard 행 확인:** verification의 두 Board에서 해당 테스트 계정의 score와 metadata를 확인한다. 해당 Board의 ledger `best`와 `submissionId/acceptedAt`가 일치하는지 결과만 전달한다. 전체 Player ID나 인증 정보는 공유하지 않는다.

새 계정을 만들거나 기존 ledger를 다시 초기화할 필요는 없다. 기록 조회·설정 확인에 Scene/Inspector 수정은 필요 없다.

#### AI 준비 후 사용자가 실행할 작업

| 검증 | 기대 결과 | 단계 |
| --- | --- | --- |
| 집중 및 회귀 Unity 테스트 | `OnlineRecordRepositoryTests`, `OnlineRecordConfigurationTests`, `LocalSaveJsonCodecTests`, 제출·순위 정책 및 영향받는 Edit/Play Mode 회귀 성공. 실행·성공·실패 수와 예상하지 않은 로그를 전달한다. | Step 6 |
| 같은 ID·같은 payload 재호출 | 같은 Submitted 결과, receipt 추가 없음, 기존 최고 기록과 접수 시각 유지. | Step 7 |
| 같은 ID·다른 payload 및 잘못된 점수/Version | Rejected, 기존 정상 기록 변경 없음. 로컬 서버 대역 성공과 실제 서비스 성공을 구분한다. | Step 7 |
| Player 직접 Leaderboards/Cloud Save Write | 403, 데이터 변경 없음. Cloud Code를 통한 정상 제출은 계속 성공. 관리 권한 호출의 성공/실패를 Player 권한 검증으로 사용하지 않는다. | Step 7 |
| Offline·Timeout·복귀·재실행 | 실패 중 로컬 결과와 동일 ID Pending 유지, 복귀 후 제출 완료. 자동 계측으로 요청 횟수·간격과 중복 여부를 판정한다. | Step 6~7 |
| 인증 실패·계정 불일치 | Offline 플레이 유지, 다른 계정으로 후보 전송 없음. 대역 테스트를 우선 사용하고 실서비스 계정 분리 검증은 AI가 준비한 격리 경로로 수행한다. | Step 6~7 |
| 최고 기록과 동점 | Stage는 작은 값, Infinite는 큰 값만 갱신. 같은 값은 최초 최고 기록의 접수 시각 보존. 조회는 점수→접수 시각 및 공동 순위 계약에 일치. | Step 5~7 |

현재 창에서 지원하지 않는 검증은 AI가 도구와 정확한 절차를 제공하기 전까지 사용자 수동 작업으로 요청하지 않는다. UGS 호출과 Unity Test Runner 실행은 사용자가 수행하고 AI는 결과 분석·코드·정적 검사를 담당한다.

#### 완료 판정

- [x] Stage와 Infinite의 제출·상위/내 주변/내 최고 조회 및 저장 값이 각 Board 계약에 일치한다.
- [x] 서버 중복·거부와 Player 직접 Write 차단에 필요한 근거가 확보된다.
- [x] 귀속·동의·실패 분류·재시도·재실행 보존의 구현 및 관련 검증 근거가 확보된다.
- [x] 관련 컴파일/테스트 실패 및 예상하지 않은 Error·Warning이 해소된다.
- [x] 검증용 제한과 미완료 일반화 항목의 처리 범위가 명시되어, 제한된 검증 성공을 운영 준비 완료로 기록하지 않는다.

위 근거를 모아 Step 4 완료 여부를 재판정한다. Step 4-1의 완료 상태는 유지한다. Phase 3 전체 완료는 Step 8에서 별도로 판정하며, Build와 Phase 4 UI 완성은 이번 Step 4 완료 작업에 추가하지 않는다.

## Step 4-1. 서버·Repository 구현과 배포 명세를 준비한다

### 목적

검증 환경에 적용하기 전에 Cloud Code 서버 검증, Cloud Save 제출 ID 보존, Access Control 정책 및 클라이언트 Online Repository의 소스와 정적 계약을 준비한다.

### AI 작업

1. Cloud Code `submit-record` 엔드포인트 소스를 작성한다. 엔드포인트는 인증된 호출 Player ID만 사용하고, Board ID·규칙 Version·제출 ID·Stage Clear Time 또는 InfiniteMode 점수 구성 요소를 검증한다.
2. Cloud Save Protected Player Data에 Submission ID 처리 상태를 보존하는 서버 측 중복 방지 경계를 작성한다. 동일 ID의 재호출은 같은 최종 결과를 반환하고, 서버 거부는 Rejected로 분류한다.
3. Stage와 InfiniteMode의 허용 Board ID, Version, 정렬 값과 InfiniteMode 논리적 최대 점수 검사를 서버 코드에 구현한다. 클라이언트가 전달한 Player ID, Board ID 또는 점수만으로 검증을 생략하지 않는다.
4. Player의 Leaderboard 직접 Write와 Cloud Save 직접 Write를 거부하고 Cloud Code 호출만 허용하는 Access Control 정책 파일을 작성한다.
5. `IOnlineRecordRepository`의 제출·상위 기록·내 주변 기록·내 최고 기록 비동기 계약과 Cloud Code 호출 구현을 작성한다. 인증 실패·Timeout·서비스 실패는 Pending, 서버 유효성 거부는 Rejected로 연결한다.
6. 명시적 동의 뒤 인증 Player ID를 제출 대상으로 귀속하고 복구 제한 확인 상태를 Local Save에 보존하는 모델·Migration을 작성한다. 기존 Submission ID는 변경하지 않는다.
7. 서버 소스·클라이언트 코드·정책 파일의 환경명, Board ID, Secret·Token 미포함 여부와 의존 방향을 정적으로 검사한다.

### 사용자 수동 작업

없음. 이 단계에서는 Unity Dashboard, Cloud Code, Cloud Save, Access Control, Unity Editor Build, Unity Test Runner 및 Scene·Inspector를 사용하지 않는다.

### 산출물

- Cloud Code 서버 스크립트와 배포 대상 이름
- Cloud Save 중복 방지 데이터 형식
- Access Control 정책 파일
- Online Repository·인증·귀속·재시도 생산 코드와 Unit Test
- 검증 환경 Dashboard 적용 절차와 기대 결과

### 완료 조건

- [x] 서버·클라이언트·정책 소스가 검증 환경 식별자와 Board 계약에 일치하고, 수동 배포에 필요한 정확한 대상·순서·확인값이 기록됐다. 검증용 준비 완료이며 서비스·Unity 검증 완료를 의미하지 않는다.

### Step 4-1 수행 결과 (2026-09-27)

- `UGS/CloudCode/submit-record.js`: 인증 context·환경·Board/Version·UUID·점수 합계·Infinite 시간별 최대값 검증, Protected ledger writeLock 예약/완료, 같은 ID 결과 재사용, 응답 유실 복구를 구현했다. Player token 대신 서비스 토큰을 사용한다.
- `UGS/CloudCode/query-records.js`: 상위/내 주변/내 최고 비동기 조회의 서버 경계를 추가했다. 점수→서버 접수 시각 정렬 및 동일 시각 공동 순위를 계산한다.
- `UGS/AccessControl/project-policy.json`: Player의 Leaderboards/Cloud Save 직접 Write Deny. 원격 적용은 하지 않았다.
- `CloudCodeRecordRepository`, `OnlineRecordCoordinator`, `UgsOnlineRecordTransport`와 GameSystem 연결: 명시적 동의 뒤 인증, 계정 일치 확인, 시작/연결 복귀/사용자 요청 재시도, 3회 제한 및 1·2초 백오프, Timeout Pending 처리.
- Local Save v2: 동의, 1회 귀속 UGS ID, Submitted/Rejected ID 보존. v1 후보·설정·Submission ID는 유지하고 동의 기본값은 false다. 요청 전 Pending 디스크 저장과 완료 receipt 저장 실패 시 보존한다.
- `Assets/Editor/OnlineRecordVerificationWindow.cs`: `Flow State > Online Record Verification` 메뉴. Scene 변경 없이 동의·인증·재시도·조회 검증 경로를 제공한다. AI는 창을 실행하지 않았다.
- 정적 계약 검사 통과, Cloud Code 로컬 대역 테스트 21/21 통과, `git diff --check` 공백 오류 없음. Unity Test Runner용 테스트는 작성했고 실행하지 않았다.
- 중요한 검증용 제한: 최초 Protected ledger는 테스트 계정별 수동 생성, ID 최대 128개 보존 후 신규 제출 Pending, 보드 100명 초과 조회 실패. 기존 ID를 삭제해 우회하지 않는다. 신규 계정 자동 초기화·보관 확장·대규모 순위 및 실제 서비스 검증 없이 운영 완료로 간주하지 않는다.
- 정확한 수동 절차와 SDK/API 근거: [UGS/VERIFICATION_DEPLOYMENT.md](../../../UGS/VERIFICATION_DEPLOYMENT.md).

### 다음 단계 경계

Step 4-1 완료 뒤에만 사용자가 `verification` 환경에 Cloud Code, Cloud Save 및 Access Control을 적용한다. 적용 뒤의 Unity Script Compilation은 Step 6, 실제 서비스 호출은 Step 7에서 수행한다.

## Step 5. 정적 검사와 Unit Test를 작성한다

### AI 작업

1. 패키지·asmdef 참조, Board ID·환경 매핑, SDK 의존 방향, Secret·Token 저장 여부와 운영 환경으로의 우발적 요청 경로를 정적으로 검사한다.
2. Edit Mode Unit Test에서 인증 성공·실패, 계정 불일치, Board Key 분리, 최고 기록 갱신, 동일 제출 ID, Pending·Submitted·Rejected, Timeout·Offline·서비스 거부, 재시도 계기·횟수·백오프와 재실행 복원을 Repository·SDK 대역으로 검증한다.
3. 서비스 검증 로직은 정상·경계·초과·누락·Version 불일치 사례로 Test한다. 클라이언트 Test가 서버의 실제 강제력을 증명한다고 간주하지 않는다.
4. 필요한 Play Mode Test로 생산 구성의 초기화 순서와 온라인 실패 시 게임 진행·로컬 결과 보존을 검증한다. Scene 의존 테스트가 필요하면 작성 방법과 사용자 실행 범위를 지정한다.

### 사용자 수동 작업

없음. 정적 판정과 Unit Test 작성은 AI가 처리한다.

### 완료 조건

- [x] 정상·거부·경계·회귀 사례가 생산 코드와 서비스 경계의 자동 Test에 연결되고 정적 검사에서 범위 밖 의존이 없다.

### Step 5 수행 결과 (2026-09-28)

- Edit Mode Test를 작성·보완했다. `OnlineRecordRepositoryTests`는 동의·인증·계정 불일치, 제출 상태 분류, Pending/Submitted/Rejected receipt, 재실행 복원, Timeout, 저장 실패, 3회 재시도와 1/2초 백오프, 상위·내 주변·내 최고 조회 계약을 다룬다.
- `OnlineRecordConfigurationTests`는 현행 Stage·Infinite Board 매핑뿐 아니라 Stage ID·Stage Rules Version·Legacy Infinite Scoring Version 불일치를 거부한다.
- 기존 `RecordSubmissionPolicyTests`와 `RecordLeaderboardPolicyTests`는 제출 후보·점수 상한·Board 분리·개인 최고 기록·동점/순위 정책을 계속 검증한다. `InfiniteModeIntegrationTests`에는 Infinite 전용 Run timer, Pause 시간 제외, 종료 후보 생성·Retry 수명 회귀를 추가했다.
- `UGS/Tests/cloud-code.test.cjs`의 Cloud Code SDK 대역 테스트 21/21이 통과했다. 동일/변조 Submission ID, CAS 동시성, 응답 유실 복구, 점수·Version·합계·상한 거부, ledger 용량, 순위·동점·페이지 제한을 검증한다.
- `UGS/Tests/static-contracts.cjs`가 통과했다. Project/Environment/Board ID, package·asmdef, service-token/CAS, Player Write Deny 정책, verification 전용 초기화·프로필, Cloud Code endpoint 의존, 재시도 계약, Local Save migration 및 Secret/금지 문법을 정적으로 검사한다.
- `git diff --check`가 통과했다. 이 결과는 정적·대역 검증이며 Unity Test Runner나 실제 UGS의 정책 강제력을 대체하지 않는다.

### 사용자 수동 작업

없음. Step 5에서는 Unity Editor Build, Unity Test Runner, Scene·Inspector 또는 UGS Dashboard 작업을 수행하지 않는다. 작성된 Unity Test는 Step 6에서 사용자가 실행한다.

### Step 6 실행 범위

1. 집중 Edit Mode: `OnlineRecordRepositoryTests`, `OnlineRecordConfigurationTests`, `LocalSaveJsonCodecTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`.
2. 영향받는 Play Mode: `InfiniteModeIntegrationTests`, `GamePauseOrchestrationTests`, `GameLifecycleIntegrationTests`, `AutoMovementIntegrationTests`.
3. 집중 범위 성공 뒤 전체 Edit Mode 및 영향받는 Play Mode 회귀를 실행한다. Compile Error, Warning, 실패 Test가 있으면 이름·메시지·Stack Trace를 전달한다.

## Step 6. Unity Script Compilation과 자동 Test를 실행한다

### AI 작업

1. 사용자에게 집중 Edit Mode Fixture, 전체 Edit Mode 및 영향받는 Play Mode 회귀 범위를 지정한다.
2. 전달받은 Compile·Test 실패와 예상하지 않은 Error·Warning을 분석·수정한다. 수정 뒤 필요한 Test 재실행 범위를 다시 지정한다.

### 사용자 수동 작업

1. Unity Editor에서 패키지 해석과 Script Compilation의 완료 상태를 확인한다. 패키지 다운로드나 프로젝트 연결이 Editor에서 요구되면 AI가 지정한 패키지·UGS 프로젝트·환경만 선택한다.
2. Unity Test Runner에서 AI가 지정한 집중 Edit Mode, 전체 Edit Mode 및 관련 Play Mode Test를 실행한다.
3. 각각의 시도·성공·실패 개수와 예상하지 않은 Error·Warning을 전달한다. 실패 시 Test 이름·메시지·Stack Trace를 전달한다.

### 완료 조건

- [x] Unity Script Compilation과 지정된 Edit Mode·Play Mode Test가 모두 성공하고 예상하지 않은 Error·Warning이 없다.

### Step 6 수행 결과 (2026-09-28)

- 사용자가 Unity Script Compilation 성공을 확인했다. 예상하지 않은 Compile Error·Warning은 없었다.
- Edit Mode Test 702개를 실행했고 모두 성공했다. 관련 예상하지 않은 Error·Warning은 없었다.
- Play Mode Test 232개를 실행했고 모두 성공했다. 관련 예상하지 않은 Error·Warning은 없었다.
- Step 5에서 지정한 집중 fixture와 전체 회귀를 포함한 결과로 접수했다. AI는 Unity Editor Build와 Unity Test Runner를 실행하지 않았다.
- 해당 시점의 컴파일·자동 회귀 근거를 확보했다. 이후 추가된 검증 창 결과 복사·Pending 요약·Offline 사전 차단 변경도 Editor에서 실행되어 Script Compilation 성공 근거를 확보했다. 추가 Test Runner 실행은 Step 7에서 요구하지 않았다.

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

- [x] 검증 환경에서 Authentication, Board별 제출·조회, Offline 복귀·재시도, 계정 불일치·중복·서버 거부가 계약대로 동작한다.

### Step 7 AI 준비 결과 (2026-09-28)

- `Assets/Editor/OnlineRecordVerificationWindow.cs`에 Scene 변경 없이 실행하는 verification 전용 검증 버튼을 추가했다. 기존 인증·Board 조회에 더해 서버 중복/변조/Version 거부, Player 직접 Write 차단, 계정 불일치 차단을 결과 요약으로 확인한다. AI는 이 창, Unity Editor Build, Unity Test Runner를 실행하지 않았다.
- **서버 중복·변조·버전 거부 검증**은 새 Stage 제출 ID로 유효한 요청을 두 번 호출한 뒤, 같은 ID의 score 변조 및 별도 ID의 Rules Version `99` 요청을 호출한다. 기대 결과는 차례로 `Submitted`, `Submitted`, `Rejected:SubmissionIdConflict`, `Rejected:InvalidVersion`이다. probe score `900000`은 현재 Stage 최고 기록보다 불리하므로 `keepBest` 최고 기록을 갱신하지 않는다.
- **Player 직접 Write 403 검증**은 현재 테스트 Anonymous Player bearer로 Stage Leaderboard 및 Cloud Save Default Player Data에 직접 POST한다. 둘 다 `HTTP 403`이어야 한다. Cloud Save는 Protected ledger가 아니라 Default Player Data endpoint를 사용하므로 Access Control의 Player Write Deny를 직접 검증한다. 둘 중 하나라도 `2xx`이면 보안 검증 실패이며 `fs_verification_direct_write_probe` 키가 생성됐을 수 있으므로 verification Dashboard에서 해당 key만 삭제한다.
- **계정 불일치 차단 검증**은 `flow-state-mismatch-probe` 프로필의 별도 Anonymous 계정으로 잠시 로그인하여 조회한다. Local Save에 귀속된 계정과 다르므로 Cloud Code 호출 전에 `TransientFailure / AuthenticationUnavailable`이어야 한다. 검증 직후 `flow-state-verification` 프로필의 원래 cached Anonymous 계정으로 복구하고, 원래 Player ID와 일치할 때만 성공으로 표시한다. 복구 실패 시 Play Mode를 중지하고 다시 시작한 뒤 인증부터 다시 확인한다.
- 위 버튼들은 submission ID, access token, 전체 Player ID, 응답 본문을 표시하거나 복사하지 않는다. verification 환경과 테스트 Anonymous 계정에서만 사용한다.
- 검증 창의 **표시 결과 복사** 버튼으로 현재 결과 요약 전체를 클립보드에 복사할 수 있다. **Pending 재시도**는 Unity의 네트워크 판정, 재시도 전·후 대기 건수, 이번 호출에서 추가된 Submitted/Rejected 수와 `대기 기록 없음`, `모든 대기 기록 완료`, `미완료 기록 유지` 중 하나를 표시한다.

### Step 7 사용자 수동 작업

1. `verification` 환경 연결 상태에서 Play Mode로 기존 게임을 시작한다. Scene 또는 Inspector를 변경하지 않는다.
2. `Flow State > Online Record Verification`을 열고 **동의 저장 및 인증 / Pending 재시도**를 한 번 실행한다. `인증·귀속 성공`을 확인한다.
3. 이미 보고된 Stage/Infinite 상위·내 주변·내 최고 조회 결과를 보존하고, 이번에는 **서버 중복·변조·버전 거부 검증**, **Player 직접 Write 403 검증**, **계정 불일치 차단 검증 (원래 계정 복구)**을 각각 한 번 실행한다. 각 창 요약의 상태 코드만 공유한다. token, 전체 Player ID, HTTP 응답 본문은 공유하지 않는다.
4. Offline 복귀는 실제 네트워크 상태 변경이 필요한 유일한 항목이다. 인증된 상태에서 네트워크를 끊고 Stage clear 또는 Infinite 종료로 새 결과 후보를 하나 만든다. 창에서 **Pending 재시도**를 실행해 Pending이 유지되는 것을 확인한다. 네트워크를 복구한 뒤 같은 버튼을 다시 실행하고 해당 Board 조회가 성공하는지 확인한다. 이 과정에서 production 환경, 다른 계정, 기존 기록을 수정하지 않는다.
5. 각 검증의 성공/실패 요약, 예상하지 않은 Error/Warning, Offline 중 Pending 유지와 복구 뒤 제출 여부만 전달한다. Unity Script Compilation과 Test Runner는 이미 Step 6에서 완료됐으므로 재실행하지 않는다.

아래 실제 버튼 결과와 Offline 복귀 결과를 모두 접수했으므로 Step 7 및 이에 의존하던 Step 4를 완료 처리한다.

### Step 7 실제 서비스 검증 결과 (2026-09-28)

- [x] 서버 중복·변조·Version 거부: 동일 ID 최초 호출과 동일 payload 재호출은 모두 `Submitted:Accepted`, 같은 ID의 score 변조는 `Rejected:SubmissionIdConflict`, 별도 ID의 Rules Version `99` 요청은 `Rejected:InvalidVersion`이었다.
- [x] Cloud Save Protected ledger에는 정상 probe receipt가 한 건만 추가됐다. 같은 payload 재호출과 같은 ID 변조는 추가 receipt를 만들지 않았고 정상 receipt를 변경하지 않았다. Version `99` 요청은 별도의 `Rejected / InvalidVersion` receipt 한 건으로 기록됐다.
- [x] Player 직접 Write 차단: Stage Leaderboard와 Cloud Save Default Player Data 직접 POST가 모두 `HTTP 403`이었다. Access Control의 두 Player Write Deny가 실제 verification 서비스에서 강제됨을 확인했다.
- [x] 계정 불일치: 별도 Anonymous 프로필에서 조회가 `TransientFailure / AuthenticationUnavailable`로 차단됐고, 원래 cached Anonymous 계정 복구가 성공했다. 다른 계정으로 Cloud Code 조회를 계속하지 않았다.
- [x] ledger의 현재 최고 기록은 Stage score `17`, Infinite score `1051`이며 각 `best`가 동일 Board의 Submitted receipt를 가리킨다. `active`는 빈 값으로 남아 완료되지 않은 서버 예약이 없다.
- [x] Offline 상태에서 Pending 1건, Submitted/Rejected 0건으로 보존됐고 네트워크 복구 후 같은 대기열이 Pending 0건, Submitted 1건, Rejected 0건으로 완료됐다. 데이터 보존·복구 계약은 실제 verification에서 성공했다.
- [x] Offline 사전 차단 재검증에서 `네트워크 판정: NotReachable`, Pending 1건 유지, Submitted/Rejected 0건이 표시됐고 Error 메시지는 발생하지 않았다. 네트워크 복구 전환 중에는 자동 재시도가 먼저 실행되어 수동 요청과 합쳐졌으며, 복구 완료 뒤 Pending은 0건이었다.

따라서 중복·변조·서버 거부·직접 Write 차단·계정 불일치, Offline 사전 차단과 데이터 복구를 완료 근거로 접수하고 Step 7을 완료 처리한다. 연결 복구 중 Pending이 잠시 유지된 표시는 실패가 아니라 이미 실행 중인 자동 재시도와 수동 요청이 중복 실행되지 않고 합쳐진 결과다.

## Step 8. Phase 3 완료 근거와 후속 범위를 정리한다

### AI 작업

1. Roadmap 007 Phase 3 완료 조건을 정적 검사, Edit Mode·Play Mode Test, 검증 환경의 실제 서비스 결과에 연결한다.
2. 미결 계정 정책, 강제되지 않는 서버 검증, 실패한 Test 또는 미확인 서비스 동작이 있으면 Phase 3을 완료 처리하지 않는다.
3. 완료 조건이 충족되면 Roadmap·Task 상태를 갱신하고 Phase 4 Leaderboard UI·전체 회귀·대상 플랫폼 Build 범위를 기록한다.

### 사용자 수동 작업

없음. Step 2~3·6~7의 근거가 부족한 경우에만 미충족 항목을 특정해 요청한다.

### 완료 조건

- [x] Phase 3의 계정·환경·Board·제출·조회·오류·중복·서버 검증과 자동 Test·실서비스 근거가 모두 기록됐다.

### Step 8 수행 결과 (2026-09-28)

| Roadmap 007 Phase 3 완료 조건 | 완료 근거 |
| --- | --- |
| Stage·Infinite Board 분리 및 Version 혼합 방지 | `fs-stage-stage-001-r1`(오름차순)과 `fs-infinite-v2`(내림차순)를 verification에 분리 구성했다. 실제 각 Board 제출·상위/내 주변/내 최고 조회가 성공했고, Stage Version `99`은 `Rejected:InvalidVersion`이었다. |
| 같은 Run 재시도의 중복 미생성 | 동일 submission ID·payload 재호출은 `Submitted:Accepted`를 재사용했고, ledger에는 정상 receipt 한 건만 남았다. 같은 ID의 변조 요청은 `Rejected:SubmissionIdConflict`였다. |
| 서비스 실패 보존·재시도 | Offline에서 Pending 1건과 Submitted/Rejected 0건을 보존하고, `NotReachable`에서는 원격 호출을 시작하지 않아 Error 없이 유지했다. 연결 복구 뒤 자동 재시도로 Pending 0건이 됐다. |
| Authentication 실패가 Offline 플레이를 막지 않음 | 별도 Anonymous 계정은 `AuthenticationUnavailable`로 요청 전에 차단됐고, 원래 계정을 복구했다. 인증·서비스 실패의 로컬 보존·재시도 규칙은 Edit/Play Mode 회귀와 실제 Offline 결과로 확인했다. |
| 환경 분리와 Secret 비노출 | verification Project/Environment와 두 Board만 사용했고 production 변경은 없었다. 코드·문서·정적 계약 검사에서 token·secret을 저장소에 두지 않는 경로를 확인했다. |

- Unity Script Compilation 성공, Edit Mode 702개 성공, Play Mode 232개 성공 및 예상하지 않은 Error·Warning 없음 결과를 Step 6 근거로 사용했다. AI는 Build와 Unity Test Runner를 실행하지 않았다.
- 검증 환경 전용 제한은 여전히 남아 있다. 최초 Protected ledger 수동 생성, 계정별 128 submission ID, `query-records` 100명 제한은 공개 운영·대규모 순위·계정 복구를 위한 해결책이 아니며, Phase 4 UI 및 별도 운영 작업에서 확대·정책 결정을 한다.
- Phase 4는 Leaderboard UI, Result 제출 상태, Loading/Empty/Offline/Error/Retry 표현, Settings 안내, 전체 회귀와 대상 플랫폼 Build를 별도로 수행한다. 이 Step에서는 Scene·Inspector 변경, Build, 추가 Unity Test Runner 실행이 필요하지 않다.

# 영향 범위

이번 작업은 `AI/90_Tasks/Prototype_7`에 Phase 3 실행 계획을 추가한다. 후속 Step 수행 시 관련 Project·System·Feature 문서, 패키지 설정, Runtime 코드, 테스트 및 검증 환경 서비스 구성이 변경될 수 있다.

# 검증 내용

- Roadmap 007 Phase 3의 구현 대상과 완료 조건을 Step 1~8에 대응시킨다.
- 수치·상태·오류 분류·재시도·중복 규칙은 정적 검사와 Unit Test에 배치한다.
- Unity Compile·Test Runner 실행과 검증 환경의 실제 서비스 연결만 사용자 실행 단계로 둔다.
- 운영 환경 변경과 Phase 4의 화면 완성·대상 플랫폼 Build를 Phase 3 완료 근거에 포함하지 않는다.

# 검증 결과

Step 1~8 및 Step 4-1의 구현·배포·정적 검사·Unity 자동 Test·실서비스 근거를 확보했다. verification에서 Stage·Infinite 제출과 조회, 서버 중복·변조·Version 거부, Leaderboard/Cloud Save Player 직접 Write 403, 계정 불일치 차단과 원래 계정 복구가 계약대로 동작했다. Protected ledger의 정상·거부 receipt 및 Board별 `best`, 빈 `active`도 확인했다. Offline은 원격 호출 없이 Pending을 보존했고 연결 복구 뒤 자동 재시도로 Pending 0건이 됐다. 검증 환경 한계와 Phase 4 범위를 분리해 기록했으며 Phase 3을 완료 처리한다.

# 후속 작업

다음 작업은 Roadmap 007 Phase 4의 Leaderboard UI·제출 상태 표현·대상 플랫폼 Build 범위다. 추가 사용자 작업은 현재 없다. AI의 Scene 수정·Build·Test Runner 실행은 없다.

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
