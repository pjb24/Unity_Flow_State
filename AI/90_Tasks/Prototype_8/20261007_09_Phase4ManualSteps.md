# Prototype 8 - Phase 4 운영 환경·배포·복구 준비

## 작업 정보

- 작업일: 2026-10-07
- 범위: Prototype 8 Phase 4
- 담당: AI(정적 검사·Unit Test·문서·절차), 사용자(Unity/UGS Dashboard·CLI·외부 로그 sink·실제 배포·복구 확인)
- 선행 조건: Phase 1~3 완료. Phase 3의 verification 환경 결과를 Production 완료 근거로 사용하지 않는다.

## 목적

verification과 Production을 환경, 데이터, 권한, 배포 이력, 로그 보존 측면에서 분리한다. Production 변경은 대상 확인·복구 자료·권한·로그 보존·호환성 판정이 모두 준비된 경우에만 사용자가 수행한다.

이 문서는 Production 공개 또는 Windows Player Build 검증을 완료 처리하지 않는다. 이는 Phase 5 범위다.

## 현재 확인된 사실과 미확정 값

- verification 대상은 Project `Unity_Flow_State`, Environment `verification`이며 C# Module `FlowStateVerification` 하나가 현재 게시 단위다.
- Phase 3에서 verification의 신규 계정 제출, Offline Pending 복구, Stage/Infinite TOP·AROUND YOU를 확인했다.
- Dashboard는 활성 C# Module source를 내려받아 로컬 Git source와 정확히 대조하는 기능을 제공하지 않았다. Git `b2953d9`는 과거 rollback 후보일 뿐 활성 원격 source의 동일성 증명이 아니다.
- Production Project ID, Environment ID, 조직의 Secret/권한 담당자, 로그 sink 제품·접근 방식·30일 보존/삭제 설정은 아직 확인되지 않았다. 이 값들은 추정하거나 문서에 임의 입력하지 않는다.
- verification의 계정·Local Save·Cloud Save·Leaderboard 행·Secret 값은 Production으로 복사하지 않는다.

## 공통 안전 규칙

1. 비밀값, Access Token, Service Account Key, Player ID, 이전 코드·인증값, 요청 전문, 원격 source 전문을 채팅·Task·커밋·명령 이력에 기록하지 않는다.
2. 한 번에 하나의 Project/Environment와 하나의 배포 단위만 다룬다. `Deploy All`, Board/Cloud Save 초기화, 기존 데이터 덮어쓰기, 환경 간 자료 복사는 금지한다.
3. 자동 검사는 실제 UGS 인증·Secret 읽기·원격 게시를 수행하지 않는다. 실제 원격 작업은 사용자가 직접 수행하고, 성공 여부·대상 식별자·버전/게시 시각·안전한 오류 분류만 공유한다.
4. 설정 또는 코드가 바뀌면 먼저 정적 검사와 Unit Test를 갱신한다. Unity Script Compilation·Test Runner·Player Build는 AI가 실행하지 않는다.
5. Production의 운영 테스트 계정·기록 생성 여부는 별도 승인 없이는 가정하지 않는다. 실제 Player 쓰기 거부 검증은 verification 격리 계정에서 수행하며, Production에서는 정책의 읽기 전용 대조와 배포 절차 재현성으로 분리한다.

## 진행 순서

| Step | 목적 | 주 담당 | 원격 변경 |
| --- | --- | --- | --- |
| 1 | 범위·현행 배포 모델·정적 기준 확정 | AI | 없음 |
| 2 | 환경 분리·호환성·정책 Unit Test 준비 | AI | 없음 |
| 3 | 대상 inventory와 복구 자료를 읽기 전용으로 확보 | 사용자 | 없음 |
| 4 | Production 환경 경계와 Client 선택 값을 구성 | 사용자 + AI | 승인된 설정만 |
| 5 | 최소 권한·Secret 이름/권한·직접 쓰기 차단 확인 | 사용자 + AI | 승인된 설정만 |
| 6 | 30일 안전 로그 sink·삭제 절차를 준비 | 사용자 + AI | 외부 운영 설정 |
| 7 | verification에서 배포·rollback 절차를 연습 | 사용자 + AI | verification만 |
| 8 | Production 배포 준비 상태를 대조하고 승인 게이트를 판정 | AI + 사용자 | 없음 |
| 9 | 승인된 Production 적용 및 복구 가능성 기록 | 사용자 | Production, 명시 승인 뒤만 |
| 10 | Phase 4 완료 판정과 Phase 5 인계 | AI + 사용자 | 없음 |

## Step 1. 범위와 현행 모델을 정적으로 확정한다

### AI 작업

1. `Assets/CloudCode/FlowStateVerification.ccmr`, `UGS/Modules/FlowStateVerification`, `UGS/CloudCode`, `UGS/AccessControl/project-policy.json`의 배포 단위·함수 계약·직접 쓰기 거부 정책을 읽기 전용으로 대조한다.
2. `UGS/VERIFICATION_DEPLOYMENT.md`의 현재 C# Module 절차와 과거 raw JavaScript/개별 Script 게시 이력을 분리한다. 해당 문서의 과거 “Phase 4의 제한된 원격 변경” 절은 현재 Production 절차의 권한이 아니다.
3. Production 값이 코드·테스트 fixture·문서 예시에 하드코딩되거나 verification 값이 Production profile에 재사용되지 않도록 검사할 정적 검사 항목을 정한다.
4. Task의 수동 입력 표를 만들되 Secret 값·Player ID·원격 source 본문 입력란은 만들지 않는다.

### 사용자 작업

없음.

### 완료 조건

- [ ] 현재 C# Module 배포 모델과 과거 게시 이력이 구분된다.
- [ ] 환경/비밀값/직접 쓰기 정책의 정적 검사 항목이 확정된다.

## Step 2. 자동 검증을 먼저 준비한다

### AI 작업

다음 Unit Test와 정적 검사를 구현하고 실행한다. 실제 UGS 호출, Secret 조회, CLI 인증, Dashboard 변경은 포함하지 않는다.

1. 환경 쌍 검사: verification과 Production은 서로 다른 non-empty Project/Environment 조합이어야 하며, 동일 조합·누락·verification ID 재사용을 거부한다.
2. Client 환경 선택 검사: 빌드에 선택된 환경만 사용하고, 런타임 입력·Remote Config·사용자 Local Save가 대상 환경을 바꾸지 못하게 한다. 온라인 Local Save/Pending 경로는 `(Project ID, Environment ID)`로 분리됨을 검증한다.
3. 배포 manifest 검사: Production manifest가 C# Module 하나만 가리키며, `Deploy All`·개별 raw JavaScript 게시·입력 정의 변경을 계획에 포함하지 않음을 검증한다.
4. Access Control 정책 검사: Player의 Cloud Save Write와 Leaderboards Write Deny 문장이 유지되고, 광범위 Player Allow 또는 정책 Sid 충돌 후보를 실패로 처리한다.
5. 구조화 로그 검사: 허용 필드(environment, module/version reference, safe outcome/reason, duration bucket, correlation ID, rate-limit result)만 사용하고, Secret·token·Player ID·공개 번호·이전 자격 증명·요청 본문이 포함되면 실패한다.
6. 배포/rollback 상태 전이 검사: backup 없음, 대상 불일치, 로그 sink 미준비, 권한 불일치, 호환성 미판정에서는 Production deploy gate가 열리지 않음을 검증한다. rollback은 같은 환경의 직전 확인 버전만 선택하며 데이터 삭제/환경 간 복사를 하지 않음을 검증한다.
7. Client/Module 호환성 검사: 지원 Client 버전 범위, API 입력/응답의 backward-compatible 조건, Local Save migration marker가 있는 자료에 구버전 writer를 재도입하지 않는 조건을 표 기반 Unit Test로 검증한다.

### 사용자 작업

없음. AI가 준비한 Unity Test는 후속 Step 8 또는 10에서 사용자가 실행한다.

### 완료 조건

- [ ] 환경 분리·정책·로그 필드·배포 gate·rollback·호환성 Unit Test와 정적 검사가 통과한다.
- [ ] 자동 검사 결과를 실제 Production 설정 또는 실제 Secret 검증으로 과장하지 않는다.

## Step 3. 대상 inventory와 복구 자료를 읽기 전용으로 확보한다 — 사용자

변경 전에 Dashboard 또는 조직의 승인된 관리 도구에서 아래 항목을 **읽기 전용**으로 확인한다. 값 자체가 민감하거나 식별 가능한 경우에는 안전한 내부 보관소에만 저장하고 채팅에는 성공/실패만 보고한다.

1. Production Project와 Production Environment의 표시 이름 및 ID가 verification과 다른지 확인한다.
2. Authentication, Cloud Code, Cloud Save, Leaderboards, Access Control의 대상 환경이 모두 Production인지 확인한다.
3. 현재 활성 Module의 표시 이름, 배포/게시 시각, Dashboard가 제공하는 version/reference, 입력 정의를 보관한다. source 다운로드가 안 되면 그 사실과 Git commit/archive hash를 별도로 기록한다.
4. Access Control 정책 전문, Secret의 **이름과 Cloud Code 접근 권한만** 보관한다. Secret 값은 열람·내보내기·공유하지 않는다.
5. Stage/Infinite Leaderboard 설정과 Cloud Save schema/key namespace를 읽기 전용으로 기록한다. 계정, ledger, Leaderboard 행, Board를 초기화하거나 verification 데이터를 복사하지 않는다.
6. 현재 Client release candidate의 Git commit, Module source commit, 호환성 표의 버전을 같은 내부 변경 기록에 연결한다.

중단 조건: Project/Environment를 확정할 수 없거나, 원격 대상이 verification과 구분되지 않거나, 복구 자료의 저장 위치가 없으면 이후 Step을 진행하지 않는다.

### 완료 조건

- [ ] Production 대상과 verification 대상의 분리가 증명 가능한 내부 inventory로 보관된다.
- [ ] 배포 전 복구 후보와 현재 설정의 읽기 전용 기준이 있다.

## Step 4. Production 환경 경계와 Client 선택 값을 구성한다 — 사용자

Step 2의 자동 검사와 Step 3 inventory가 완료된 뒤에만 수행한다.

1. Unity Dashboard에서 Production Environment를 선택한 뒤 Authentication·Cloud Code·Cloud Save·Leaderboards가 모두 같은 Production Environment를 가리키는지 확인한다.
2. Client의 Production build profile에는 승인된 Production Project/Environment 쌍만 넣고, verification profile과 별도 build profile/Local Save namespace를 사용한다. 한 build가 사용자 입력으로 두 환경을 전환하지 않게 한다.
3. Production에 verification 계정, Local Save, Cloud Save 행, Leaderboard 기록, transfer code, Secret을 복사하지 않는다.
4. 환경값 또는 build profile을 새로 추가/수정했다면 그 값은 안전한 내부 배포 설정에만 저장한다. 값은 Task·소스·테스트 fixture에 복사하지 않는다.
5. 코드 변경이 생긴 경우 AI가 정적 검사와 Unit Test를 갱신한 뒤, 사용자는 Unity Script Compilation과 전체 EditMode/PlayMode Test를 실행한다.

### 완료 조건

- [ ] Production build profile과 verification build profile이 서로 다른 환경 조합과 Local Save/Pending namespace를 사용한다.
- [ ] 사용자 조작이나 Local Save로 환경을 교차 선택할 수 없다.

## Step 5. 최소 권한·Secret 접근·직접 쓰기 차단을 확인한다 — 사용자

1. Step 3에서 백업한 정책과 `UGS/AccessControl/project-policy.json`을 대조한다. Player의 Leaderboards Write와 Cloud Save Write Deny가 유지되는지 확인한다.
2. Player Write를 다시 Allow하는 넓은 정책, 같은 Sid의 충돌 정책, 대상 환경 불일치를 발견하면 정책을 삭제/덮어쓰지 말고 중단한다.
3. Cloud Code가 필요한 Secret의 **이름과 읽기 권한**만 확인한다. 값, key, token, HMAC은 표시·복사·로그 기록하지 않는다.
4. 정책을 수정해야 한다면 Step 3 backup 뒤 조직의 승인된 CLI/관리 절차로 Production에만 적용한다. 적용 직후 대상 Environment와 정책 revision/reference를 다시 읽어 대조한다.
5. Player 직접 쓰기 거부의 실제 403 검증은 Production 데이터에 영향을 주지 않는 verification 격리 계정에서 수행한다. Production에서는 읽기 전용 정책 대조와 동일 policy artifact의 정적/Unit Test로 제한한다.

### 완료 조건

- [ ] Player 직접 쓰기를 차단하는 정책과 Cloud Code의 필요한 Secret 접근이 대상 환경에서 대조된다.
- [ ] Secret 값은 어떤 확인 자료에도 포함되지 않는다.

## Step 6. 30일 안전 로그 sink와 삭제 절차를 준비한다 — 사용자

1. 조직이 승인한 로그 sink를 선정하고 접근 담당자, 보관 기간 30일, 자동 삭제 또는 삭제 runbook, 접근 제어 방식을 내부 운영 기록에 명시한다.
2. UGS Cloud Code 기본 로그만으로 30일 요건을 충족하는지 공식 보존 설정/계약으로 확인한다. 충족하지 못하면 PII·비밀값을 받지 않는 최소 외부 sink를 먼저 준비한다.
3. Step 2의 허용 필드만 sink schema로 등록한다. raw request/response, Player ID, 공개 번호, Secret, token, transfer credential은 수집 금지로 설정한다.
4. verification에서 합성한 안전 이벤트로 수집·조회·권한 제한·삭제 절차를 점검한다. 실제 계정/Secret/요청 본문을 사용하지 않는다.
5. 30일 보존 또는 삭제 절차를 입증할 수 없으면 Production 배포 gate를 닫아 둔다.

### 완료 조건

- [ ] 최소 구조화 로그의 허용 필드·접근 권한·30일 보존·삭제 절차가 문서와 설정으로 대조된다.
- [ ] sink가 준비되지 않았으면 Production 배포가 명시적으로 차단된다.

## Step 7. verification에서 배포·rollback 절차를 연습한다 — 사용자

1. Step 3 방식으로 verification의 현행 대상·배포 reference·입력 정의·정책 reference를 읽기 전용으로 보관한다.
2. 변경이 있는 경우에만 `FlowStateVerification` C# Module 하나를 빌드하고, verification을 재확인한 뒤 **Deploy Selected**를 실행한다. `Deploy All`, raw JavaScript 개별 게시, Board/ledger 초기화는 하지 않는다.
3. 배포 후에는 안전한 verification 격리 계정으로 계정 상태 조회와 Stage/Infinite TOP·AROUND YOU 조회를 한 번씩 확인한다. 새 기록 제출·계정 이전·자료 삭제는 이 rehearsal에 포함하지 않는다.
4. 실패, 대상 불일치, 예상 밖 경고, 비밀 노출 우려가 있으면 새 배포를 중단한다. 같은 verification 환경의 Step 1에서 보관한 직전 확인 Module reference만 재게시한다. Cloud Save·Leaderboard·계정 데이터를 삭제하거나 다른 환경 자료로 덮어쓰지 않는다.
5. rollback 뒤 같은 읽기 전용 조회로 정상 복귀 여부를 확인하고, 배포/rollback 시각·대상·reference·안전한 결과만 기록한다.

### 완료 조건

- [ ] verification에서 한 환경·한 Module 단위의 배포와 rollback 절차가 재현된다.
- [ ] rollback이 데이터 삭제나 환경 간 복사 없이 수행됨을 확인한다.

## Step 8. Production 적용 가능 여부를 대조한다

### AI 작업

1. Step 1~7의 확인 근거가 모두 있고, Step 2의 정적 검사/Unit Test가 통과하며, 최신 코드 변경이 있으면 Unity 회귀가 통과했는지 대조한다.
2. 다음 중 하나라도 없으면 `PRODUCTION_DEPLOY_BLOCKED`로 판정한다: 대상 inventory, 복구 후보, 최소 권한 대조, Secret 권한 대조, 30일 로그/삭제 절차, 호환성 표, verification rehearsal 결과.
3. 허용된 Production 변경 단위·직전 rollback reference·담당자·중단 기준만 정리한다. Secret 값, Player 식별자, 원격 source 본문은 기록하지 않는다.

### 사용자 작업

AI의 대조 결과가 `READY_FOR_APPROVAL`일 때에만 Production 적용 권한과 변경 창을 승인한다. 승인 전에는 Step 9를 수행하지 않는다.

### 완료 조건

- [ ] Production 적용 여부가 확인 근거에 따라 `READY_FOR_APPROVAL` 또는 `PRODUCTION_DEPLOY_BLOCKED` 중 하나로 명확히 판정된다.

## Step 9. 승인된 Production 적용과 복구 가능성을 기록한다 — 사용자

이 Step은 Step 8의 `READY_FOR_APPROVAL`과 사용자의 명시적 Production 변경 승인이 모두 있을 때만 수행한다.

1. 변경 직전에 Project/Environment·Module 이름·직전 rollback reference·로그 sink 상태를 다시 확인한다.
2. 승인된 단일 배포 단위만 적용한다. 대상 불일치·버전 불명·입력 정의 차이·예상 밖 경고가 있으면 즉시 중단한다.
3. 배포 뒤에는 Dashboard가 제공하는 게시 시각/reference와 대상 Environment만 기록한다. 운영 데이터 생성, verification 자료 복사, 수동 ledger 생성, Board 초기화는 하지 않는다.
4. 오류 또는 호환성 문제가 생기면 같은 Production 환경의 직전 확인 reference로 rollback하고, 데이터 삭제·환경 간 덮어쓰기·Secret 노출 없이 Step 7과 같은 읽기 전용 확인을 수행한다.
5. Production에서 Player 기능을 실제로 실행하는 통합 검증과 Windows Player Build는 Phase 5에서 별도 승인 뒤 수행한다.

### 완료 조건

- [ ] 승인된 단일 Production 변경이 대상·reference·복구 절차와 함께 기록되거나, 중단/rollback 결과가 안전하게 기록된다.

## Step 10. Phase 4 완료를 판정하고 Phase 5로 인계한다

다음을 모두 만족할 때만 완료 처리한다.

- [ ] verification과 Production의 환경·계정·제출·조회·Local Save/Pending 경로가 섞이지 않음을 정적 검사·설정 inventory·Unit Test로 확인했다.
- [ ] Player 직접 쓰기 차단, Cloud Code Secret 최소 접근, Server 검증 경계가 대상 설정과 verification 실제 거부 검사로 대조됐다.
- [ ] 비밀값 없는 구조화 로그, 30일 보존, 삭제 절차, 1인 수동 장애 대응 및 테스트 자료 관리가 준비됐다.
- [ ] 배포 전 backup, 단일 배포 단위, rollback reference, verification rehearsal, Production 승인 gate가 재현 가능하다.
- [ ] Client/Module/Local Save 호환성 표와 데이터 보존·rollback 금지 조합이 문서·Unit Test로 대조됐다.
- [ ] 최신 변경이 있으면 Unity Script Compilation, 전체 EditMode/PlayMode Test를 사용자가 통과시켰다.

Phase 5 인계 항목은 Windows x64 1920×1080 Player Build, Keyboard/Mouse UI, 승인된 운영 대상의 신규/기존 계정 및 Offline 복구, 운영 rollback 후 데이터 보존이다. verification 결과만으로 Production 검증 완료 또는 일반 사용자 공개를 선언하지 않는다.

## AI와 사용자의 역할 분리

| 영역 | AI | 사용자 |
| --- | --- | --- |
| 코드·문서·정책 artifact | 정적 검사와 Unit Test 작성/실행, 절차 작성 | 결과 검토 |
| Unity | 실행하지 않음 | Compilation, Test Runner, Build 실행 |
| Scene | 수정하지 않음 | 필요할 때만 직접 적용 |
| UGS/외부 로그 sink | 실제 인증·조회·게시·Secret 열람을 하지 않음 | Dashboard/CLI/sink 설정·배포·rollback 수행 |
| 민감 정보 | 노출·저장·요청하지 않음 | 승인된 내부 보관소에서만 관리 |

## 관련 문서

- [Prototype 8 Roadmap](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md)
- [Phase 3 Manual Steps](20261006_08_Phase3ManualSteps.md)
- [Verification Deployment](../../../UGS/VERIFICATION_DEPLOYMENT.md)
- [Transfer Operations Runbook](../../../UGS/TRANSFER_OPERATIONS_RUNBOOK.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
