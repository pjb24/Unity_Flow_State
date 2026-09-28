# 작업 정보

## 작업명

Prototype 7 Phase 4 — Leaderboard UI·기록 경쟁 흐름·출시 후보 검증 작업 계획

## 작업 일자

20260928

## 작업 담당자

AI, 사용자

## 작업 상태

계획 문서 작성 완료. 아래 Step 1~10은 미실행이며 Phase 4는 대기 상태다. 이 문서는 구현·Scene 연결·서비스 적용·테스트·Build 성공을 의미하지 않는다.

# 작업 목적

Roadmap 007 Phase 4의 Main Menu Leaderboard, Result 기록·순위·제출 상태, 계정 안내와 오류·재시도 경험을 구현하고 대상 플랫폼에서 검증한다. 정적 검사와 Unit Test로 판정 가능한 항목은 AI가 검사 또는 테스트를 준비한다. 사용자는 필요한 결정, Scene/Inspector 적용, Unity 컴파일·Test Runner·Build 실행, 실제 서비스 연결과 시각적 확인을 수행한다.

# 작업 대상

- Main Menu Leaderboard: Stage/Infinite Tab, Stage 선택, 상위·내 주변·내 최고 기록, Back/Cancel.
- Result: 새 로컬 최고 기록과 서버 최고 기록 구분, 현재 Run 제출 상태, 자신의 기록·순위를 확인하는 경로.
- Loading, Empty, Offline, Error, Retry 및 Anonymous 복구 제한 동의·Settings 재안내.
- Menu·Run·Result·Settings·로컬 저장·온라인 서비스 회귀, 실제 verification 통합, 대상 플랫폼 Player.

# 작업 전 상태

- Roadmap과 Phase 3 Task에는 Phase 3 및 Step 1~8 완료가 기록돼 있다. 사용자 보고 기반 Edit Mode 702개, Play Mode 232개 성공은 해당 시점의 근거이며 Phase 4 변경 뒤의 회귀 결과로 재사용하지 않는다.
- `UIManagementSystem`과 `GameNavigationState`는 기존 Leaderboard 안내 화면 및 Back 경로를 가진다. `IOnlineRecordRepository`는 제출·상위·내 주변·내 최고 비동기 API를 제공하고, 실제 검증은 `OnlineRecordVerificationWindow`로 수행했다.
- 현재 Stage는 `stage-001` / Rules Version `1` / `fs-stage-stage-001-r1`, Infinite는 Scoring Version `2` / `fs-infinite-v2`다. 미구현 Stage나 다른 Version의 Board를 임의로 추가하지 않는다.
- 서비스 대상은 Project `c76d55cf-7846-494b-9dce-a0797b179b36`, 환경 `verification` / `a20a46fa-1edb-4d79-9c35-02f2fed31896`이다.
- 최초 Protected ledger 수동 생성, 계정별 128 submission ID, 보드 100명 조회 제한이 남아 있다. 검증 환경 UI 완성과 공개 운영 준비를 구분하며, 출시 후보 범위와 제한 처리를 Step 1에서 결정한다.

# 조사 내용

- `Leaderboard.md`에는 제출 가능한 확정 기록이 있어야 조회한다는 규칙과 미구현 안내 화면 규칙이 있다. Empty의 의미 및 실제 화면 도입 시 갱신할 계약을 먼저 정리해야 한다.
- `RecordSubmission.md`는 최초 온라인 요청 전 명시적 동의, 1회 계정 귀속, 다른 계정 후보 전송 금지, Pending 보존, Rejected 자동 재시도 금지를 정의한다.
- `Assets/Scenes/SampleScene.unity`가 존재한다. 정확한 연결 대상 Hierarchy와 필드·이벤트·레이아웃 값은 구현 완료 후 읽기 전용 조사로 확정해 제공한다. 아직 존재하지 않는 Component 이름을 연결 지시로 사용하지 않는다.
- `VERIFICATION_RULE.md`에 따라 수치·정렬·상태·중복·비동기 경합은 자동 Test로 판정한다. 사람이 빠르게 Tab을 누르거나 동일 프레임 조작을 재현하도록 요구하지 않는다.

# 작업 내용

## 공통 진행 원칙

- 각 Step은 AI 준비, 사용자 실행, 결과 판정 순서로 진행한다. AI 산출물이 준비되기 전에 해당 사용자 작업을 요청하지 않는다.
- AI는 Scene을 생성·수정하거나 Unity Build·Test Runner를 실행하지 않는다. Scene 참조·직렬화 값의 읽기 전용 정적 검사는 AI가 한다. UGS 원격 적용은 사용자가 한다.
- 결과 보고에는 Step, 실행 범위, 성공/실패, 예상하지 않은 Error/Warning, 실패한 Test 이름과 Stack Trace를 남긴다. token·secret·전체 Player ID·전체 사용자 저장 파일을 공유하지 않는다.
- 실제 서비스 검증은 verification만 사용한다. 기존 Board·ledger·계정·저장 파일을 초기화하거나 삭제해 테스트를 통과시키지 않는다.
- 사용자 작업이 없는 Step은 정적 검사·구현·자동 Test 작성만으로 진행한다. Test 작성 완료와 Test 실행 성공은 따로 기록한다.

## Step 1. UI 계약과 검증·출시 후보 범위를 확정한다

### AI 작업

1. Feature·System·Roadmap과 현재 화면 전환·Result·Repository를 대조하고 UI 상태표와 변경 파일 목록을 작성한다.
2. 기록이 없는 사용자에게 허용할 조회, Empty와 미제출 상태, Result에서 순위를 여는 방식, Stage가 하나인 선택 UI, 계정 표시 형식과 동의 취소 동작을 제안한다. 기존 규칙 변경이 필요한 부분은 장단점과 함께 명시한다.
3. Stage 시간 단위·표시 정밀도, Infinite 점수, 공동 순위, 로컬 새 최고와 서버 최고, Pending/Submitted/Rejected 표현을 계약으로 고정한다. Pending 0건만으로 Submitted 성공을 판단하지 않는다.
4. 검증 환경 한도 내 출시 후보인지 공개 운영 후보인지 구분한다. 후자라면 ledger 자동 초기화·보관 한도·순위 조회 확장·운영 배포를 별도 선행 작업으로 기록한다. 필수 범위를 미정 상태로 넘기지 않는다.

### 사용자 수동 작업

1. AI의 상태표·미정 항목 제안에서 사용자 선택이 필요한 항목만 결정한다.
2. 대상 플랫폼·아키텍처, 확인할 화면 해상도·창 모드와 입력 장치, 성능/응답 시간의 수용 기준을 알려준다. 이전 Windows Player 검증 이력만으로 최종 플랫폼을 확정하지 않는다.
3. 검증 환경 한도를 가진 후보의 허용 범위를 결정한다. 이 결정은 production 배포 승인이 아니다.

### 완료 조건

- [ ] 화면 계약, 플랫폼·검증 기준, 운영 제한의 처리 범위가 확정됐다.

## Step 2. UI 연결 코드와 Unit Test를 준비한다

### AI 작업

1. Repository 경계 위에 Leaderboard 상태·화면 연결을 구현한다. Main Menu 진입, Mode/Stage 전환, 상위·주변·개인 최고, Back/Cancel을 기존 입력·Navigation과 연결한다.
2. Result의 로컬 최고·현재 Run 제출 상태·개인 기록/순위 확인 경로와 Settings 계정 안내를 구현한다. 첫 온라인 요청 전 동의를 저장하고, 미동의·계정 불일치 상태에서 온라인 전송을 시작하지 않는다.
3. Loading/Empty/Offline/Error/Retry를 구분한다. 화면을 닫거나 Board를 바꾼 뒤 늦은 응답이 새 화면을 덮어쓰지 않도록 한다. 중복 요청·구독 누적·자동/수동 Retry 경합을 처리한다.
4. 동작 변경마다 Edit Mode와 필요한 Play Mode Test를 함께 작성한다. 외부 서비스 대역으로 실패·지연·Timeout·역순 응답을 재현하며 SDK 내부 로그를 전역으로 숨기지 않는다.
5. 사용자가 따라 할 Scene 연결 명세를 작성한다. 대상 Scene/Hierarchy, 추가 GameObject, 구현된 Component, Inspector 필드와 참조, Button 이벤트, Navigation, 초기 활성 상태, RectTransform·스크롤·문구 값을 정확히 기재한다. 설정 검증용 Test도 함께 준비한다.

### 사용자 수동 작업

없음. 이 Step에서는 Scene 연결이나 Dashboard 적용을 시작하지 않는다.

### 완료 조건

- [ ] 구현·테스트 코드·Scene 연결 명세가 준비되고 정적 검사에서 누락이 없다.

## Step 3. 코드와 서비스 구성을 정적으로 검증한다

### AI 작업

1. 참조/asmdef·직렬화 필드·이벤트 시그니처, Board/Version/환경 매핑, SDK 의존 방향, 비밀값 미포함을 검사한다.
2. 화면 계약별 테스트 대응, 미동의 전 요청 차단, 계정 불일치, 제출 상태 보존, 늦은 응답 폐기, 객체 해제·이벤트 해제를 대조한다.
3. 기존 Scene을 읽어 연결 명세가 실제 대상과 일치하는지 확인한다. 정적으로 확인 가능한 오브젝트 경로·참조·상수를 사용자에게 재확인시키지 않는다.
4. 서버 변경이 있다면 로컬 Cloud Code 대역 테스트·정적 계약 검사를 수행하고 변경된 스크립트·정책·데이터 호환성과 적용 순서를 명시한다. 실행 도구가 없으면 성공으로 기록하지 않고 AI가 사용 가능한 실행 경로를 조사한다.

### 사용자 수동 작업

없음.

### 완료 조건

- [ ] 정적 검사와 필요한 로컬 서버 테스트가 통과하고 Scene 연결·원격 적용 대상이 확정됐다.

## Step 4. Unity에서 컴파일하고 UI를 Scene에 연결한다

### 사용자 수동 작업

1. Play Mode를 종료하고 Unity Editor의 Script Compilation 완료를 확인한다. Compile Error가 있으면 연결 작업을 멈추고 오류를 전달한다.
2. Step 2~3에서 제공한 명세의 Scene을 연다. 지정된 Leaderboard·Result·Settings UI 오브젝트에 명세의 Component·표시 요소·Button을 추가 또는 연결한다.
3. 명세대로 Inspector 참조, Button 이벤트, Keyboard Navigation, 초기 표시 상태 및 레이아웃을 설정하고 Scene을 저장한다. 명세에 없는 대상을 추측해 연결하지 않는다.
4. 적용 완료 여부와 컴파일의 예상하지 않은 Error/Warning을 전달한다.

### AI 후속 작업

저장된 Scene diff를 읽기 전용으로 검사해 누락 참조·중복 이벤트·잘못된 대상·범위 밖 변경을 식별하고 필요한 수정 방법만 안내한다. Scene을 직접 고치지 않는다.

### 완료 조건

- [ ] 컴파일과 Scene 연결 정적 검사가 통과했다. 실제 설정 테스트는 Step 6에서 수행한다.

## Step 5. 필요한 서비스 변경만 verification에 수동 적용한다

### AI 작업

Phase 3 배포와 변경 목록을 비교한다. 변경이 없으면 사용자 작업 없이 이 Step을 '해당 없음'으로 판정한다. 변경이 있으면 정확한 파일·endpoint·설정·적용 순서·되돌리기 절차를 제공한다.

### 사용자 수동 작업 — 변경이 있는 경우만

1. Unity Dashboard에서 위 Project와 verification 환경을 선택한다.
2. AI가 확정한 Cloud Code·Cloud Save·Access Control 변경만 적용한다. UI 추가만으로 Board를 새로 만들거나 기존 ledger를 초기화하지 않는다.
3. 새 테스트 계정이 꼭 필요하고 최초 ledger 수동 생성 제한이 유지된다면 `UGS/VERIFICATION_DEPLOYMENT.md`의 절차로 해당 계정만 준비한다.
4. 배포 대상·환경·성공/실패를 전달한다. 인증 정보는 전달하지 않는다.

### 완료 조건

- [ ] 필요한 원격 변경이 적용됐거나 변경 없음이 명시됐다.

## Step 6. Unity Test Runner로 UI·저장·게임 회귀를 실행한다

### AI 작업

아래 검증 표에 따라 생산 코드를 사용하는 테스트를 작성하고, 실제 생성된 fixture 이름과 실행 순서를 제공한다. 기존 `OnlineRecordRepositoryTests`, `OnlineRecordConfigurationTests`, `LocalSaveJsonCodecTests`, `RecordSubmissionPolicyTests`, `RecordLeaderboardPolicyTests`를 재사용·확장한다. 새 fixture 이름은 구현 전 확정 사실로 기록하지 않는다.

### 사용자 수동 작업

1. 최신 Script Compilation 성공을 확인한다.
2. Test Runner의 Edit Mode에서 AI가 지정한 집중 테스트를 실행한다. 성공 후 전체 Edit Mode를 실행한다.
3. Play Mode에서 UI 상태·Scene 연결·입력·비동기 회귀를 실행한다. 성공 후 전체 Play Mode를 실행해 Menu·Run·Result·Settings·저장 영향도 확인한다.
4. 각 실행의 총수·성공·실패·무시 수와 예상하지 않은 Error/Warning을 전달한다. 실패는 Test 이름·메시지·Stack Trace를 포함한다.

### 완료 조건

- [ ] 최신 변경의 전체 Edit/Play Mode가 통과했다. 필수 사례 누락·실패·예상하지 않은 Error/Warning은 해결됐으며 무시된 테스트가 있으면 사유와 영향이 기록됐다.

## Step 7. 생산 UI의 실제 서비스 연결과 Offline 복구를 확인한다

### 사전 조건 및 AI 준비

Step 4~6 성공 후 진행한다. AI는 생산 UI를 통해 발생한 요청의 Board·현재 계정 일치 여부·후보별 Pending/Submitted/Rejected 변화·동일 제출 ID 보존을 비밀값 없이 요약하는 검증 수단을 준비한다. 자동 재시도가 수동 Retry보다 먼저 완료된 경우에도 완료 원인과 terminal 상태를 확인할 수 있어야 한다.

### 사용자 수동 작업

1. verification에서 기존 게임을 Play Mode로 시작한다. 생산 Main Menu의 Leaderboard를 연다. 최초 동의가 필요한 경우 안내를 읽고 동의/취소 경로를 확인한다. 미동의 분기의 반복 검증은 대역 테스트로 처리하며 기존 저장 파일을 수동 편집하지 않는다.
2. Stage와 Infinite에서 상위·내 주변·내 최고 보기를 실행하고 결과 요약을 전달한다. 미구현 안내 화면이나 검증 창만으로 생산 UI 성공을 판정하지 않는다.
3. Stage를 clear하고 Infinite를 정상 종료해 Result의 이번 기록·새 최고 여부·제출 상태와 개인 기록/순위 확인 경로를 사용한다. 새 최고를 만들기 위한 임의 점수 입력은 하지 않는다. 새 최고·동점 판정은 Unit Test로 검증한다.
4. 네트워크를 끄고 새 플레이 결과 하나를 만든 뒤 UI의 Retry를 누른다. Offline 표시와 Pending 보존, 게임을 계속 이용할 수 있는지 확인하고 요약을 복사한다.
5. 네트워크를 복구하고 자동 재시도 완료 또는 UI Retry를 확인한다. 요약에서 해당 후보의 Submitted/Rejected를 구분한다. Pending 0건만 보고 제출 성공으로 보고하지 않는다. 자동 재시도 중이면 완료 요약을 기다린다.
6. Settings에서 익명 계정의 복구 제한 안내를 다시 열고 Back으로 복귀한다. 예상하지 않은 로그와 UI 문제를 전달한다.

### 완료 조건

- [ ] 생산 UI의 실제 조회·제출·계정 안내·Offline 복구가 계약에 일치한다. 실패 원인은 수정 후 영향받는 Step을 다시 검증했다.

## Step 8. 화면 가독성과 입력 경험을 수동 확인한다

### 사용자 수동 작업

1. Step 1에서 확정한 해상도·창 모드로 Main Menu, Leaderboard, Result, Settings를 확인한다. 글자 잘림, 행 겹침, 스크롤, 선택 강조, 안내와 버튼의 가독성을 기록한다.
2. Keyboard Navigate/Submit/Cancel과 Mouse Point/Click으로 Tab·Stage 선택·조회·Retry·Back을 사용한다. 지원하기로 확정한 다른 입력 장치가 있으면 같은 경로를 확인한다.
3. Loading/Empty/Offline/Error 화면은 AI가 제공한 재현 경로나 테스트 미리보기로 확인한다. 서비스 장애를 만들거나 기록을 지워 Empty를 만들지 않는다.
4. 읽기 어렵거나 조작이 막히는 화면의 조건과 필요시 스크린샷을 전달한다. 비밀값과 전체 Player ID는 가린다.

### AI 후속 작업

표현 코드 문제는 수정하고 Scene 수정이 필요한 경우 정확한 적용 방법을 제공한다. 상태 분기와 빠른 입력 경합의 정확성은 Step 6의 자동 Test로 판정한다.

### 완료 조건

- [ ] 확정한 화면·입력 조건에서 가독성 및 조작에 차단 문제가 없다.

## Step 9. 대상 플랫폼을 Build하고 Player에서 검증한다

### AI 준비

Build에 들어갈 Scene 목록, 플랫폼·아키텍처·구성 및 verification 선택을 정적으로 대조하고 실제 설정값과 출력 위치를 안내한다. Editor 전용 코드의 Runtime 의존과 테스트/비밀값의 포함 여부를 검사한다. 최종 후보에 들어간 변경으로 Step 6~8을 통과했는지 확인한다.

### 사용자 수동 작업

1. Unity Editor의 Build Profiles/Build Settings에서 확정한 플랫폼·Scene·구성을 선택하고 안내된 별도 출력 폴더로 Build한다. 기존 배포물을 덮어쓰지 않는다.
2. Build 성공/실패와 예상하지 않은 로그를 전달한다. 실패하면 원인을 수정한 후 영향받는 테스트와 Build를 다시 실행한다.
3. 생성한 Player를 실행해 Main Menu → Leaderboard → Run → Result → Main Menu와 Settings 안내를 이용한다. Editor 전용 검증 창 없이 완료할 수 있어야 한다.
4. Player에서 verification 인증·두 Mode 조회/제출·Offline 유지·복귀를 확인한다. 별도 Anonymous 계정에 ledger 준비가 필요하면 Step 5의 계정별 절차를 사용한다.
5. 후보가 Pending인 상태에서 앱을 정상 종료하고 다시 실행해 로컬 기록·Settings·Binding·Tutorial 상태 보존 및 복구 뒤 제출을 확인한다. AI가 준비한 요약으로 ID 보존·중복 여부를 판정한다.
6. 합의한 기기·해상도·입력 조건에서 프레임/응답 시간 측정과 가독성을 확인한다. Build 구성, 측정 조건, 결과를 전달한다. 측정용 Development Build만 검증했다면 최종 후보 구성을 통과했다고 기록하지 않는다.

### 완료 조건

- [ ] 합의한 대상 플랫폼·최종 후보 구성에서 Build, 실제 서비스·재실행·Offline·성능 검증이 통과했다.

## Step 10. Phase 4와 Prototype 7의 완료 근거를 정리한다

### AI 작업

1. Roadmap의 여섯 완료 조건을 최신 정적 검사·전체 Unity Test·생산 UI 서비스 결과·Player Build 결과와 대응시킨다.
2. 기존 Phase 3 결과와 Phase 4 변경 후 결과를 구분하고, 수정 후 재검증이 누락된 범위가 없는지 확인한다. 남은 실패·미확인 필수 사례가 있으면 완료 처리하지 않는다.
3. Step 1에서 정한 운영 제한 처리와 출시 후보 범위를 대조한다. 문서에 제한을 적는 것만으로 공개 운영 준비를 완료 처리하지 않는다.
4. 조건 충족 시 Task·Roadmap·Project 상태를 갱신하고 미승인 운영 배포·추가 플랫폼·추가 기능은 별도 후속 작업으로 남긴다.

### 사용자 수동 작업

없음. 근거가 누락된 경우에만 해당 Step의 구체적인 실행 항목을 요청한다.

### 완료 조건

- [ ] Phase 4의 완료 근거와 한계가 기록되고 Roadmap의 상태와 일치한다.

# 영향 범위

이번 요청은 이 Task 계획 문서를 추가한다. 후속 Step 실행 시 Runtime UI·상태 모델·테스트·관련 계약 문서가 변경되고, 사용자가 Scene 설정과 필요한 서비스 적용을 수행한다.

# 검증 내용

| 판정 대상 | 정적 검사 / AI 책임 | 자동 Test / AI 작성·사용자 실행 | 필요한 수동 확인 |
| --- | --- | --- | --- |
| Board·Stage·Version·환경 | 매핑·요청·정책 대조 | 잘못된 Key/Version 거부 | verification 실제 응답 |
| 시간·점수·순위·새 최고 | 기존 단위·순위 계약 대조 | 경계값·동점·로컬/서버 최고 구분 | 표시 가독성 |
| UI 결과 상태 | 상태표·화면 연결 대조 | Loading→성공/Empty/Offline/Error, Retry, 미제출/Rejected | 화면 표현 |
| 비동기 수명·입력 | 구독/해제·응답 식별·중복 요청 경로 | 역순 응답·닫은 화면·Tab 전환·재진입·연속 입력·취소 | 일반 조작감 |
| 동의·계정 귀속 | 저장 전 요청 차단·SDK 경계 | 동의/취소·저장 실패·다른 계정·인증 실패 | 실제 안내·원래 계정 연결 |
| Retry·재실행 | Pending/receipt 저장 순서 | 최대 3회·백오프·Timeout·동일 ID·동시 트리거·Rejected 미재시도 | 실제 Offline 복구·Player 저장 수명 |
| Scene·게임 회귀 | 참조·Button 이벤트·초기 활성 상태 | 생산 Scene 구성, Menu/Run/Result/Settings, 입력 복귀 | 사용자 Scene 연결 |
| 출시 후보 | Build 설정·Editor 의존·비밀값 검사 | 전체 Edit/Play Mode | 사용자 Build·Player·성능 측정 |

서비스 대역 테스트는 실제 Access Control 강제력의 대체 근거가 아니다. 반대로 실제 서비스 한 번의 성공만으로 수치·비동기 경합·모든 실패 분기를 검증했다고 판정하지 않는다.

# 검증 결과

Roadmap Phase 4의 구현 대상·완료 조건을 Step 1~10과 검증 책임표에 배치했다. 기존 Feature의 조회 조건과 운영 제한은 선행 결정 항목으로 남겼다. 이번 작업에서는 구현·테스트 실행·서비스 적용·Scene 수정·Build를 수행하지 않았다.

# 후속 작업

Step 1부터 수행한다. 지금 사용자에게 요구되는 Unity·UGS 수동 작업은 없다. AI가 각 단계의 코드·연결 명세·실행 범위를 준비한 뒤 해당 Step의 사용자 작업을 요청한다.

# 관련 문서

- `AI/README.md`
- `AI/00_Project/PROJECT_OVERVIEW.md`
- `AI/00_Project/ARCHITECTURE.md`
- `AI/00_Project/PROJECT_MEMORY.md`
- `AI/01_Rules/AI_RULE.md`
- `AI/01_Rules/IMPLEMENTATION_RULE.md`
- `AI/01_Rules/VERIFICATION_RULE.md`
- `AI/03_Features/Leaderboard.md`
- `AI/03_Features/RecordSubmission.md`
- `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_007.md`
- `AI/99_Templates/GENERAL_TASK_TEMPLATE.md`
- `UGS/VERIFICATION_DEPLOYMENT.md`

# 관련 작업 기록

- `AI/90_Tasks/Prototype_7/20260924_01_Phase1ManualSteps.md`
- `AI/90_Tasks/Prototype_7/20260926_01_Phase2ManualSteps.md`
- `AI/90_Tasks/Prototype_7/20260927_01_Phase3ManualSteps.md`

# 작성 완료 기준

- [x] 사용자 수동 조작은 결정·Scene 연결·Unity 실행·실서비스·화면/Player 검증으로 특정했다.
- [x] 정적 검사와 Edit/Play Mode Test를 각 단계의 선행 조건 및 완료 근거로 배치했다.
- [x] 실제 연결 명세·fixture 이름은 구현 후 제공하며 존재하지 않는 대상을 확정하지 않았다.
- [x] 계획 작성 완료와 Phase 4 수행 완료를 구분했다.
