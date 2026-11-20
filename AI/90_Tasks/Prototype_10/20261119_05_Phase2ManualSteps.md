# 작업 정보

## 작업명

Prototype 10 Phase 2 — 형상 관리 제외와 애셋 도입·복원 절차

## 작업 일자

20261119~20261120

## 작업 담당자

- AI: 기준선/자료 대조, 영구 절차와 검사 도구·Unit Test 구현, 정적/격리된 오프라인 검증, 필요 사용자 작업 최소화·결과/Phase 3 인계.
- 사용자: 실제 후보의 취득/정책 중 확인 불가 정보, 필요한 실제 Unity Compile/Test Runner·Scene/Prefab 적용·빌드, 구체적으로 필요한 의존성 취득/설치 실행.

## 작업 상태

**Step 1~10 및 Phase 2 완료(2026-11-20). Phase 3은 미착수이며 Roadmap 10 전체는 미완료다.** 관리/보존·유입/누락/버전 검사와 도입/확장/lifecycle·재도입 절차, 현재 사용자 실행 불필요 근거·최종 판정/Phase 3 인계를 완료했다. 실제 원본 추적 해제/삭제·lifecycle 적용·선언/lock 변경·설치·권리 확인·독립 Unity 복원 성공은 아니다.

# 작업 목적

앞으로 새 애셋을 선택했을 때 **등록→조건 확인→원본 보관/복사→Git 검사→게임 적용 기록→업데이트/제거→재취득/복원**을 반복할 수 있는 실제 절차와 검사를 만든다. 현재 도입한 Third Party 애셋은 없으므로 구매/도입을 착수 조건으로 삼지 않는다. 기존 UPM/TMP·서버 보조 의존성은 별도 기술적 기준선으로 보존한다.

Phase 1의 경계/형식을 실제 정책·명령·실패 판정에 연결한다. 실제 후보가 0개여도 등록 오류/강제 staging/누락·버전 불일치·변경본 손실을 합성 fixture와 임시 저장소에서 검증하고, 최종 실제 저장소의 현재 상태도 검사한다. 독립 환경에서 실제 프로젝트가 복원되는지의 성공 판정은 Phase 3이다.

# 작업 대상

- [관리 기록](../../00_Project/ASSET_DEPENDENCIES.md), [등록 형식](../../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md), [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)의 지속 관리/도입 절차.
- `Tools/DependencyAudit`의 수집/분류·Test·README와 필요한 최소 정책/보존/복원 보조 기능.
- `.gitignore`/하위 ignore·HEAD/index/worktree·실제 파일·원본/파생/자체 자료와 자산/폴더 meta.
- TMP 공급 archive·변경된 폰트 asset 2개·TMP Settings/Material/atlas·Resources/Shader 이름과 참조 계약.
- Unity manifest/lock/Editor 버전·루트 npm 선언/lock·Cloud Code csproj/EmbeddedResource·LogSink 선언의 복원 절차.

# 작업 전 상태

1. [Phase 1 Task](20261118_02_Phase1ManualSteps.md) Step 1~10/Phase 1은 2026-11-19 완료했다. Step 10-3/10-4의 실제 적용·복원 의무를 인계한다. 현재 실제 도입/제외 후보·관리 기록의 제품 항목은 0개다.
2. 현 `Assets/ThirdParty`/루트 meta는 없고 TMP는 비추적/무시다. 원본과 byte가 다른 TMP 폰트 asset 2개는 현재 ignore 아래 있어 보존/재적용 절차가 필요하다. 현재 ignore만으로 그 변경본이 복원된다고 할 수 없다.
3. 기존 Node 기본 Unit Test 54개·정책 포함 55개 통과는 Phase 1 근거다. 최신 actual CLI는 collectionComplete=true/error 0·exit 1이며 기존 미해결 참조 31회(URP 11종/15회·TMP graph texture 1종/16회)와 sample orphan 1개가 남아 있다.
4. Phase 2는 아직 미착수다. 기존 미커밋 문서·metadata·Tools를 보존하며 커밋 유무를 착수 조건으로 삼지 않는다. 실제 입력 목록/해시와 허용 변경 목록으로 기준선을 식별한다.

## 작업 범위와 실행 경계

- AI는 Unity Compile/Build/Test Runner·Player/Module 빌드·서비스/원격 게시를 실행하지 않는다. Scene/Prefab은 읽기 전용이며 실제 변경이 필요할 때 정확한 사용자 적용표를 작성한다.
- 현재 Task 작성은 실제 원본 복사/추적 해제·Git 이력 재작성·자료/라이선스 삭제·구매/배포 승인이 아니다. Step 실행 요청 범위에서 필요한 가역 작업부터 구체화하며 범위 밖 파괴적 조작은 수행하지 않는다.
- 검사 CLI는 읽기 전용을 유지한다. 필요한 보존/경로 적용 보조 기능은 별도 명시적 입력/출력 경로와 실행 모드로 분리하고, 검사/require 시 자동 실행하지 않는다. 원본/기존 파일 덮어쓰기·폴더 재귀 삭제·cache 청소를 기본 동작으로 두지 않는다.
- 원본/계정/영수증·Secret·유료 payload를 Git 문서/Test fixture에 넣지 않는다. 공개 metadata/해시·검증 상태만 기록한다. 보관 자료도 Unity가 임포트하는 Assets 아래에 중복 GUID로 넣지 않는다.
- 현재 제외 후보가 없으면 실제 index 변경을 하지 않는다. 원본 위치/이름만으로 소유권을 확정하지 않으며 기존 자료의 권리 미확인을 새 사용/재배포 승인으로 바꾸지 않는다.
- 권리/자료/참조·원복 근거가 실제 적용을 막으면 그 작업을 보류한다. 기존 finding 수 일치·Node Test·cache 존재를 새 Unity 실행/독립 복원 성공으로 쓰지 않는다.

# 조사 내용

| 확인한 기존 자료 | 현재 확인한 사실 | Phase 2에 필요한 변화 |
| --- | --- | --- |
| Phase 1 Step 10 인계 | 현재 후보 0개, 관리/보존 범위와 미확인/후속 gate가 있음 | 실제 적용 순서·실패/중단·보존/복원 명령과 검증으로 연결 |
| 관리 기록/템플릿 | 단일 Markdown 기록, 필수 23필드·경로별 표·독립 상태 4축 | 실제 항목 유효성·상태/근거·경로 연결을 자동 검사. 제품 없는 상태 유지 |
| 현재 DependencyAudit | 읽기 전용 inventory/graph/GUID·등록 형식·보수적 분류. 현 CLI 옵션은 `--check`, `--root`, `--baseline` | 실제 기록/경계와 HEAD·staged 원본을 연결한 유입 차단/복원 입력 검사 필요. 현재 inventory 자체가 유입 차단 완성은 아님 |
| TMP/복사 자료 | 82파일 중 80개 같은 원본, 폰트 asset 2개 차이·meta 45개 동일 | 변경본·연결 설정을 잃지 않는 보존본/재적용 metadata·검증 절차 필요 |
| 보조 의존성 | npm local tgz 27개는 Cloud Code cache 경로에 의존, NuGet/서비스 조건 일부 미확인 | provider ID/버전·해시로 다시 찾는 이식 가능한 단계·실패 판정 필요 |

아래 보조 파일/CLI 이름은 **제안**이며 현재 존재/실행 성공이 아니다. 기존 책임을 재사용해 최종 인터페이스를 정한 후 그 명령만 실행/기록한다. 새 상시 Runtime System·외부 package는 필요하지 않다.

# 작업 내용

## 실행 순서

| Step | 실제 수행할 작업 | 주 산출물 |
| --- | --- | --- |
| 1 | Phase 1 인계·현재 상태/허용 변경 기준선 확보 | Phase 2 입력 해시·보존/변경 목록 |
| 2 | 등록 항목·관리 정책의 검증 가능한 형식 확정 | 기록 parser/유효성 검사·최소 경계 정책 |
| 3 | 보관·복사·게임 적용/수정의 실제 순서 작성 | 영구 도입/확장 절차·경로 계약 |
| 4 | TMP 변경본/설정 보존과 재적용 준비 | 비추적 보존본 또는 검증된 재적용 정보 |
| 5 | ignore·현재 추적/staged 원본 유입 검사 적용 | 정책 기반 유입 검사·필요한 최소 ignore 보완 |
| 6 | 의존성/애셋 누락·버전 불일치와 재취득 절차 구현 | 복원 입력 검사·이식 가능한 취득/설정 순서 |
| 7 | 업데이트·경로 변경·제거·원복 절차 연결 | lifecycle/참조 영향·실패 시 복구 절차 |
| 8 | 최소 도구/Unit Test·실제 저장소 검사 실행 | 정상/실패·읽기 전용/보존 근거 |
| 9 | 필요한 사용자 결정/조건부 Unity 검증 연결 | 필수 미확인 해소·최종 적용/검증 범위 |
| 10 | Phase 2 완료 판정·Phase 3 검증 인계 | 완료 조건 표·독립 복원 입력/중단 기준 |

## Step 1. 입력과 허용 변경 범위를 확정한다

1-1. Phase 1 Step 10과 관리 규칙·과거 metadata를 현재 파일에 대조한다. HEAD/status·추적/staged/비무시 신규 경로·ignore·manifest/lock·ProjectVersion·Scene/설정·TMP 변경본/meta·Tools의 해시를 확보한다. Secret/실제 저장은 제외한다.

1-2. 이전 결과의 당시 완료/미완료 상태를 고쳐 쓰지 않고 Phase 2 기준선을 새 metadata로 남긴다. 예상 변경은 문서/Tools/최소 정책·필요한 ignore/복원 선언 등 경로별로 정하고, 생산 C#/Scene/Prefab·기존 원본/후보의 보존을 확인한다.

1-3. 같은 조사 scope·symlink/junction 제외/부분 실패 처리를 유지한다. 실행 출력은 이 Task 폴더의 명시한 UTF-8 metadata 경로로 제한하고, 원본 보관/보조 적용의 출력 경로는 별도 계약으로 정한다.

- [x] 현재 입력/보존/허용 변경 목록과 Phase 2/3 경계를 확보했다.

### Step 1 수행 결과 — 2026-11-19

#### 1-1. 인계 대조와 현재 입력 확보 — 완료

Phase 1 Step 10·관리 기록/규칙과 과거 metadata를 대조하고, 새 [입력 기준선](20261119_06_Phase2Step1Baseline.json)에 HEAD·추적/HEAD 경로 목록·staged·비무시 신규 경로·초기 status·index SHA-256을 기록했다. 시작 HEAD는 `5a9b34d583bb744209a4444f69cd051ae88523df`, 추적 경로는 933개이며 시작 staged/비무시 신규/변경은 없다. 과거 문서의 미커밋 상태를 현재 상태로 추정하지 않았다.

Assets/Packages/ProjectSettings/Tools/UGS와 추적 AI 문서·루트 선언·TMP 공급 archive·Candidate02 실행 파일 등 1,007개 파일의 경로/크기/SHA-256을 확보했다. 민감 본문 10개와 실제 저장은 읽지 않았다. 자산/폴더 meta와 ignore 판정은 [현재 정적 검사](20261119_07_Phase2Step1Audit.json)의 inventory에 연결한다.

Phase 1 기준 입력 66개·TMP 로컬 자료 82개(meta 포함)는 당시 해시와 모두 일치한다. TMP 폰트 변경본 2개는 기존 변경 상태로 보존하며 공급본과 같은 것으로 바꾸지 않았다. Candidate02 실행 파일도 이전 해시와 일치한다. 후보 전체 디렉터리는 해시 조사하지 않았으며 새 Player 실행/QA 성공을 주장하지 않는다.

기존 CLI `node Tools/DependencyAudit/dependency-audit.cjs --check --baseline AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json`를 실행했다. 수집 완료/error 0·exit 1이며 미해결 참조 31회와 sample orphan 1개를 포함한 finding 목록 전체가 Phase 1 Step 9와 일치한다. 기존 finding을 숨기거나 참조 정상/권리·독립 복원 PASS로 바꾸지 않았다.

#### 1-2. 보존 목록과 변경 후보 경로 확정 — 완료

기준선의 `changeContract`에 현재 Step의 출력과 후속 Step의 조건부 변경 후보를 구분했다.

| 범위 | 경로와 조건 |
| --- | --- |
| 현재 Step | 본 Task·Roadmap 10·공통 안내·Project Memory와 새 기준선/검사/최종 검증 JSON 3개만 갱신 |
| Step 2/3/7 | ASSET_DEPENDENCIES.md·ASSET_DEPENDENCY_TEMPLATE.md·IMPLEMENTATION_RULE.md의 형식/절차. 실제 제품 등록은 0개 유지 |
| Step 2/4/5/6/8 | Tools/DependencyAudit/의 최소 parser/정책·보존/복원 보조 기능·Test/안내. require/검사 시 쓰기 금지 |
| Step 5 | 실제 누락 근거가 있는 경우만 .gitignore 보완. 파생/연결 자료·meta 보존 |
| Step 6 | package.json/package-lock.json은 정확한 mapping·예상 diff·원복을 먼저 정한 경우만 후보. 버전/integrity 임의 변경·AI 설치/lock 재생성 금지 |
| 보존 | 생산 C#/Scene/Prefab·TMP 원본/변경본·meta·Packages 선언/lock·ProjectSettings·UGS·기존 Phase 1 metadata·QA 후보·HEAD/index. Secret/실제 저장은 접근 제외 |

후속 후보 목록은 이번 Step의 실제 변경/설치·보관 복사 범위가 아니다. 원본 제외 후보 0개이므로 추적 해제·staging·Git 이력 변경도 하지 않았다. 기존 파일은 허용 문서 4개를 제외하고 전후 해시를 대조했으며 상세 결과는 [최종 검증](20261119_08_Phase2Step1Verification.json)에 남긴다.

#### 1-3. 조사/출력 경계와 후속 검증 인계 — 완료

기존 reader/CLI의 조사 scope·제외 디렉터리·민감 본문 차단을 재사용했다. symlink/junction은 따라가지 않으며 열거/읽기 실패를 부분 성공으로 취급하지 않는다. 이번 범위의 열거/읽기 오류와 링크 진단은 없다. 결과는 이 Task 폴더의 명시한 UTF-8 JSON 3개로 한정하고 기존 metadata를 덮어쓰지 않았다.

원본 보관은 아직 하지 않았다. `Ignore/DependencyPreservation/<id>/<hash>/`는 Step 4의 제안 출력이며 Assets 밖 경로·ignore·충돌·junction·덮어쓰기 거절을 검증한 뒤 명시적 입력/출력으로만 사용한다. 이식 archive 출력도 Step 6에서 provider/버전/해시·정확한 경로 계약을 먼저 정한다.

독립 checkout/실제 재도입과 Unity Import/타입/참조 검증은 Phase 3에 남기며 기존 원격 Module 복구 시험 의무도 유지한다. 문서/metadata만 변경하여 Unity Compile/Build/Test Runner·Scene/Prefab 적용·의존성 설치 등 사용자 수동 작업은 필요하지 않다. 새 검사 로직을 구현하지 않았으므로 기존 Unit Test는 반복하지 않았다.

**Step 1만 완료하며 다음 작업은 Step 2 실제 기록/경계 정책 자동 검사다.**

## Step 2. 실제 기록과 경계 정책을 자동 검사한다

2-1. 기존 관리 기록/빈 형식을 확장해 실제 항목의 필수 필드·ID/버전·근거/미확인·상태 전환을 검증하는 parser/순수 함수를 만든다. 현재 실제 제품 항목은 비워 두고 정상/실패 예제는 합성 fixture에서만 사용한다.

2-2. 제품/조건/사용처의 단일 정답은 관리 기록에 둔다. 기계용 경계가 필요하면 `Tools/DependencyAudit/dependency-policy.json`을 제안 경로로 사용하되 ID와 경로·역할/유지/제외·meta/재현 근거 참조만 넣는다. 제품/버전·license/권리 값을 다른 register에 중복 수동 관리하지 않는다. schema/정책 충돌·등록 미연결은 실패로 판정한다.

2-3. 원본·수정/파생·자체 연결·혼합/미확인과 자산/폴더 meta를 경로별로 나눈다. 미확인·변경본/설정에 제외/적용 상태를 줄 수 없고, 근거/필요 결정 누락을 보류로 출력한다. 향후 정책 변경 시 `adoption/rights/boundary/restore`가 자동 성공으로 연쇄 전환되지 않게 한다.

2-4. 누락/중복 ID/필드·잘못된 표/escape/개행·잘못된 상태·계정/credential URL·정책 충돌·빈 등록부·권리 미확인/사용자 결정 전후를 실제 parser/판정 함수의 Unit Test로 확인한다.

- [x] 단일 기록과 경로 정책의 연결을 검증하고 빈 실제 등록부·미확인 상태를 올바르게 처리했다.

### Step 2 수행 결과 — 2026-11-19

#### 2-1. 실제 등록부 parser와 상태 검증 — 완료

`Tools/DependencyAudit/dependency-registry.cjs`에 parseRegistry/evaluate/validateTransition 순수 함수를 추가했다. 필수 23필드·ID/목록 링크·제품/버전 대응·한 줄 표/escape·근거/미확인 사유·SHA-256·계정/credential URL과 독립 상태를 검사한다. import_path와 경로 표를 연결하며 중복·대소문자·상하위 경로 충돌을 거절한다. 제품별 실제 기록은 ASSET_DEPENDENCIES.md 한 곳에 유지하고 항목 수는 0개로 보존했다.

등록 형식에 실제 셀 문법을 정의했다. status는 네 독립 축과 사용자 decision의 state/evidence JSON이며, 경로별 표에서 원본/변경/파생/자체/혼합/미확인/설정 역할과 자산/폴더 meta를 구분한다. 확인/적용 주장에는 그 상태의 근거가 필요하다. 이전 등록부를 명시하는 transition 검사는 version/source/license/hash 변경 시 과거 확인/결정 근거의 자동 승계를 거절한다. 이전 입력이 없는 실제 빈 등록부 검사는 snapshot 검사로 표시한다.

#### 2-2. 단일 기록과 최소 경계 정책 연결 — 완료

`Tools/DependencyAudit/dependency-policy.json`을 schemaVersion 1·entries 0개로 추가했다. 정책에는 ID·경로/역할·보존 방향·자산/폴더 meta와 등록부 필드 참조만 허용하며 제품/버전/license·권리 값을 복제하지 않는다. validatePolicy는 등록부 경계 행과 일대일 연결하고 schema/ID/path/meta/action/근거 참조의 충돌·누락·중복을 실패로 판정한다.

`unregisteredRoots=["Assets/ThirdParty"]`는 Step 5에 넘길 조사 범위 선언이다. 현재 registry 검사는 Git HEAD/index/staged 원본 유입을 차단하는 기능이 아니며 결과에 gitIngressChecked=false를 명시한다. 기존 inventory CLI와 별도인 읽기 전용 `node Tools/DependencyAudit/dependency-registry.cjs --check`를 구현하고 README에 실제 명령과 exit 0/2의 의미를 기록했다.

#### 2-3. 보존/보류와 독립 상태 판정 — 완료

미확인/혼합은 확정·적용 상태를 거절하고, 변경/파생/설정의 적용 주장과 모든 비원본의 제외 요청을 차단한다. 원본도 권리·출처/버전·hash·meta/복원/사용처 근거와 명시적 사용자 결정이 부족하면 preserve-pending이다. 유효한 비추적 후보여도 applied/rightsGranted/independentRestore는 false로 남는다. 근거 부족·사용자 결정 전의 제외 요청은 POLICY_EXCLUSION_BLOCKED와 보류 사유로 실패하며 정책 수정이 도입/권리/복원 축을 자동 변경하지 않는다.

이 검사는 기록된 metadata의 형식/상충을 확인한다. 근거의 진위·법적 권한·사용자 신원이나 실제 독립 복원 성공을 인증하지 않으며, URL 검사도 임의 문장 속 모든 Secret 탐지를 보장하지 않는다. 원본/계정/Secret은 계속 입력 금지다.

#### 2-4. 정상/실패 Test와 실제 저장소 검사 — 완료

새 `dependency-registry.test.cjs` 49개와 기존 DependencyAudit 정책 포함 55개가 모두 exit 0으로 통과했다. 합성 제품·임시 디렉터리를 사용해 필드/ID/경로 중복·표/escape/개행·BOM/상태/근거·URL·권리 미확인·정책 연결/충돌·버전 전환·명시적 결정 전후·실제 CLI와 require 부작용을 검사했다. 초기 CLI return 문법 오류와 목록 요약의 미확인 상태를 일반 필드 값으로 오판한 결함은 수정 후 새 suite 전체를 재실행해 해결했다.

실제 registry CLI는 항목 0개·경계 0개·exit 0이다. 실제 inventory CLI는 collectionComplete=true/error 0·exit 1이며 기존 미해결 참조 31회/sample orphan을 포함한 전체 진단 목록이 Step 1과 일치한다. 상세 명령/결과·source 해시는 [Step 2 검사](20261119_09_Phase2Step2Check.json)에, 변경/보존·문서 검증은 [Step 2 최종 검증](20261119_10_Phase2Step2Verification.json)에 남긴다.

기존 Step 1의 문서/metadata·미커밋 변경을 보존했다. 허용 문서 갱신과 새 도구/정책·Test/metadata 외 생산 C#/Scene/Prefab·원본/meta·선언/lock·QA 후보·HEAD/index는 변경하지 않았다. 사용자 수동 작업은 없다. Unity Compile/Build/Test Runner·설치·Scene/Prefab 적용을 실행하지 않았다.

**Step 2만 추가 완료하며 다음은 Step 3 도입/보관·복사·확장 절차다.**

## Step 3. 원본 보관·프로젝트 복사·활용/확장 절차를 정한다

3-1. Implementation Rule의 외부 애셋 기준을 영구 실행 절차로 확장한다. 후보 등록→공식 버전/조건/해시 확인→비추적 원본 보관→선택한 범위 도입→GUID/Import/참조 대조→게임 사용처 기록→Git 검사 순서를 적는다. 공급자가 부여한 조건을 확인하며 프로젝트가 임의 license를 채택/재부여하지 않는다.

3-2. 도입 경로 `Assets/ThirdParty/<공급자>/<제품>/`, 자체 변형/연결 자료 `Assets/ProjectAssets/<asset_id>/`, Unity 밖 비추적 보관 위치의 실제 사용 규칙을 정한다. 새 폴더 생성이 필요한 때만 만들고 현재 없는 제품용 빈 폴더/Scene을 채우지 않는다. 같은 GUID의 두 버전 동시 임포트·파생물의 원본 GUID 복제를 막는다.

3-3. 공급 원본을 직접 고치는 대신 프로젝트 소유 Material preset/설정·연결 애셋/확장 코드 또는 재현 가능한 변경 정보를 분리한다. 외부 소스/변경본의 권리·기술적 분리가 불명확하면 보존/보류한다. 필요한 Attribution·빌드 배포 위치와 Resources/Shader 이름·사용 목적/게임 위치를 기록한다.

3-4. 경로/구성요소 매핑·중복 GUID·선택 범위 누락·원본에 쓰기·사용처 미기록·허용 불명 수정본의 제외를 합성 metadata/임시 파일로 검사한다. 실제 Scene/Prefab 연결이 생기면 사용자 적용표와 해당 Unity suite를 후속 Step 9에 연결한다.

- [x] 누가 어느 경로에 무엇을 도입/분리·기록하는지 실제 순서와 실패 조건으로 정리했다.

### Step 3 수행 결과 — 2026-11-19

#### 3-1. 영구 실행 절차와 담당/중단 조건 — 완료

Implementation Rule의 외부 애셋 관리 기준에 등록→공식 버전/조건/hash 확인→Assets 밖 비추적 원본 보관→선택 범위 도입→GUID/Import/참조 대조→게임 사용/확장 기록→Git/검증 gate의 일곱 단계 표를 추가했다. 각 단계의 AI/소유자 담당과 통과/중단·보존/원복 조건을 명시했다. 공급 조건을 확인하며 프로젝트가 임의 license를 채택/재부여하지 않는다. 원본 보존 복사는 Step 4, Git 유입 차단은 Step 5, 이식 복원은 Step 6의 후속 구현/검증과 연결한다.

#### 3-2. 보관·복사/확장 경로와 GUID 계약 — 완료

도입용 공급 배포물/선택 원본은 `Ignore/DependencySources/<asset_id>/<archive_sha256>/`, 순수 공급본은 `Assets/ThirdParty/<supplier>/<product>/`, 프로젝트 변형/연결 자료는 `Assets/ProjectAssets/<asset_id>/`로 구분했다. 기존 TMP 보존본 출력은 Step 4의 별도 계약을 유지한다. 공급 version/hash와 선택 원본/meta·source→target 매핑을 고정하고 실제 ignore/비추적·경로 충돌/링크·덮어쓰기 여부를 먼저 확인한다.

같은 GUID의 두 버전 동시 import와 새 파생물에 공급본의 어떤 GUID를 복제하는 경우도 거절한다. 원본 GUID는 보존하며 자산/폴더 meta와 Import 변경을 각각 기록한다. 현재 실제 후보 0개이므로 계획 파일·제품 항목·빈 폴더/Scene과 원본 복사본은 만들지 않았다.

#### 3-3. 확장/사용처·배포 조건과 사용자 적용 경계 — 완료

공급 원본을 직접 고치지 않고 프로젝트 소유 Material preset·설정/연결 애셋·확장 코드 또는 재현 가능한 변경 정보로 분리하는 순서를 정했다. 복사/수정한 외부 자료를 순수 자체 소유로 바꾸지 않으며 권리/분리 불명은 보존/보류한다. purpose/game_usage/references/notices에 실제 사용 위치·Resources/Shader 이름·직접/동적/binary 한계·배포 시 license/Attribution 위치와 변경/원복 근거를 연결한다.

필요한 Scene/Prefab 연결은 AI가 편집하지 않고 Hierarchy/컴포넌트/필드·기존→최종값·저장/원복·정적 확인표와 관련 Unity suite를 Step 9에 준비한다. 현재 연결 대상이 없어 적용표나 수동 Unity 작업은 없다.

#### 3-4. 도입 사전 검사와 합성/임시 파일 검증 — 완료

`Tools/DependencyAudit/dependency-adoption.cjs`에 순수 planSchema/validate와 읽기 전용 inspect를 추가했다. 기존 registry parser/분류 함수와 reader/Git read 명령을 재사용하며 실제 copy/apply CLI는 만들지 않았다. source/version/archive hash·선택 매핑/원본 쓰기·기존 목적지/meta·GUID 충돌·공급 GUID를 복제한 파생물·사용처/조건 누락·허용 불명 수정본 제외를 검사한다. inspect는 실제 원본/meta hash와 관측된 YAML/JSON/Importer GUID를 읽고 계획에서 누락된 선언을 거절하며 ignore/비추적·HEAD/index/입력 불변을 확인한다.

새 adoption Test 36개, 기존 registry 49개와 정책 포함 DependencyAudit 55개가 모두 exit 0으로 통과했다. 합성 metadata·임시 Git 저장소의 파일/meta/강제 staging·junction으로 정상/실패를 검증했다. 실제 직렬화 참조 Test가 기존 references 함수의 인자 순서 오용을 검출했고 수정 후 새 suite 전체 36개를 재실행해 해결했다. 사용자 작업본 staging/reset·원본 복사/이동·Unity 실행은 하지 않았다.

실제 빈 등록부 검사와 inventory 결과·source hash는 [Step 3 검사](20261119_11_Phase2Step3Check.json), 생산/기존 미커밋 산출물 보존·UTF-8/링크/공백 검증은 [최종 검증](20261119_12_Phase2Step3Verification.json)에 남긴다. 선언한 파일/GUID 밖의 모든 의존성·동적/binary·fileID/Import 의미와 권리 진위를 인증하는 검사는 아니며 정상 결과도 실제 적용/독립 복원 PASS가 아니다.

**Step 3만 추가 완료하며 다음은 Step 4 TMP 변경본/설정의 손실 방지 수단이다. 현재 필요한 수동 작업은 없다.**

## Step 4. 기존 TMP 변경본과 설정의 손실 방지 수단을 준비한다

4-1. 현재 폰트 변경본 2개와 TMP Settings/Material/atlas·자산/폴더 meta·직접/이름 참조의 실제 해시를 재대조하고 보존/재취득/재적용할 경로 표를 만든다. 원인/권리를 추측하거나 같은 공급 archive만으로 변경본 재현을 보장하지 않는다.

4-2. 현재 작업본을 건드리지 않는 **비추적 보존본** 또는 검증 가능한 재적용 방법을 정한다. 로컬 보관 예시는 `Ignore/DependencyPreservation/<식별자>/<해시>/`이며 실제 정책/ignore와 경로 충돌을 먼저 검증한다. Assets 밖에 보관하고 원본/Secret을 Git/문서/Test에 복제하지 않는다. 필요한 보조 기능은 `Tools/DependencyAudit/dependency-preservation.cjs` 등을 제안 경로로 분리한다.

4-3. 보존 기능이 필요하면 명시적 입력·출력과 사전 점검/적용을 분리한다. 원본 불변·기존 출력 덮어쓰기 거절·외부 경로/junction 거절·파일/meta/해시 연결·부분 복사 실패를 처리한다. 기존 원본을 먼저 옮기거나 지워 보존 여부를 시험하지 않는다. 재적용 검증은 별도 임시 대상에서 수행한다.

4-4. 자체 Test에서 누락/오염된 보존본·잘못된 버전/GUID·동일 ID 다른 내용·권한/중단·설정/Material 누락·동적 이름 변경을 검출한다. 실제 로컬 보존본을 만들 경우 전후 hash·비추적/ignore·원본 불변을 확인한다. 로컬 보관 성공은 독립 복원 성공이나 다른 환경에 대한 원본 배포 허용이 아니다.

- [x] 변경본/설정·meta를 손실 없이 다시 준비할 입력과 실제 보존/임시 재적용 검증 근거가 있다.

### Step 4 수행 결과 — 2026-11-19

#### 4-1. TMP 변경본/연결 설정·경로 계약 대조 — 완료

현재 TMP 82개 파일(root/자산/폴더 meta 포함)과 공급 archive 1개의 해시를 Phase 1/Phase 2 Step 1 기준선과 대조해 모두 일치를 확인했다. 입력은 [보존 spec](20261119_13_Phase2Step4PreservationSpec.json), 실제 명령/결과·manifest 고정 해시·직접/이름 참조 계약은 [Step 4 검사](20261119_14_Phase2Step4Check.json)에 기록했다. Editor `6000.3.5f2`, provider `com.unity.ugui 2.0.0`, archive SHA-256 `58564ec64cef8a8cea3d0b8ffb5e6fecf4176d544a2f6a29a2b43a0a33256431`을 고정한다. 현재 cache 경로 suffix는 이번 입력 위치이며 이식 복원 계약은 아니다.

| 경로/구성요소 | 이번 보존/후속 재취득·재적용 범위 |
| --- | --- |
| Resources/Fonts & Materials/LiberationSans SDF.asset 및 LiberationSans SDF - Fallback.asset | 공급본과 byte가 다른 기존 폰트 2개를 현재 hash 그대로 보존. 내부 atlas/subasset·폰트 설정도 파일 전체에 포함하며 원인/권리는 추측하지 않음 |
| Resources/TMP Settings.asset·Material·Sprite/Style/LineBreaking 및 Shader/ShaderGraph·Font/Sprite 원본 | 기존 TMP 경로 전체를 보존해 연결 설정/Resources 상대 이름·Shader 이름 누락을 방지. 실제 재도입은 같은 공급 버전/hash와 보존 변경본·설정 재적용 순서를 Phase 3에 연결 |
| Assets/TextMesh Pro.meta 및 자산/폴더 meta 45개 | 전체 byte/hash와 GUID를 함께 보존해 GUID/Importer/platform 설정 변형을 검출 |
| TMP Essential Resources.unitypackage | 재취득용 정확한 공급 archive를 함께 로컬 비추적 보존. archive만으로 변경본 2개가 재현된다는 주장은 하지 않음 |
| SampleScene·Assets/Settings·ProjectSettings·Packages 선언/lock | 이미 추적 중인 연결/생산 입력을 이동/복사하지 않고 기준 hash 일치를 확인. Phase 1 incoming GUID/fileID·Resources/Shader 계약은 그대로 인계하며 새 Unity 해석 성공으로 바꾸지 않음 |

#### 4-2. 실제 로컬 비추적 보존본 생성 — 완료

`Tools/DependencyAudit/dependency-preservation.cjs`를 추가하고 준비/생성/검증을 분리했다. 새 보존본은 `Ignore/DependencyPreservation/tmp-baseline/29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c/`이다. 83개 입력 파일, 45개 meta, payload 합계 4,989,600 bytes를 원래 상대 경로로 보관했다. 기존 `.gitignore`의 `/Ignore` 규칙으로 전체 output의 ignore/비추적을 확인했고 ignore/index 변경은 하지 않았다. 유료/외부 원본 본문은 Git 문서/Test에 넣지 않았다.

준비 검사 후 명시적인 `--create <spec> <정확한 output>`을 실행했다. 대상 존재/덮어쓰기·부모 junction·상위/외부 경로·tracked/unignored 출력을 거절하며 파일 exclusive 생성 후 manifest.json을 마지막에 쓴다. 부분 실패 디렉터리는 자동 삭제/덮어쓰지 않고 검증 실패로 남긴다. 실제 작업본은 복사 원본으로만 읽었으며 이동/삭제·원인 불명 설정 수정을 하지 않았다.

#### 4-3. 고정 expected 검증과 별도 임시 재적용 — 완료

`--verify <Step4Check.json> <output>`은 Task의 고정 expected digest/ID/Editor/provider/archive 조건으로 manifest와 전체 payload/coverage·GUID/meta·Resources/Shader 이름을 검사한다. 현재 source를 정답으로 다시 만들지 않아 source가 없어도 보존본을 검증할 수 있다. 실제 검증 exit 0과 입력 83개 전후 hash 불변을 확인했다.

실제 보존본의 83개 파일을 작업본과 분리된 OS 임시 디렉터리로 재적용하고 hash/byte·GUID/meta/Import bytes·Resources/Shader 이름이 일치함을 확인했다. 실제 Assets는 변경하지 않았고, 임시 대상은 절대 경로·임시 root/전용 접두사를 검증한 뒤 정리했다. 로컬 보관/임시 재적용은 독립 checkout·Unity Import/타입/subasset/렌더링·권리/원본 배포 성공이나 승인이 아니다.

최종 검토에서 폴더 meta가 가리키는 빈 폴더의 재생성 누락 가능성을 발견해 manifest schema 2와 폴더 쌍/누락 Test를 추가했다. 최종 보존본은 folder meta 8개에 대응하는 실제 디렉터리까지 검증/재적용한다. 최초 schema 1 보존본 `b186c4e72d21a125b32fc1c01c130c8d618f5b509d92b858ec48d8230bf67581`은 덮어쓰기/삭제하지 않고 superseded로 기록했으며 최종 복원 입력이 아니다. 입력 원본은 변경하지 않았고, 수정 후 전체 새 suite와 실제 생성/검증/임시 재적용을 다시 수행했다.

#### 4-4. 정상/실패 Test와 전후 보존 검증 — 완료

새 보존 Test 20개와 기존 adoption 36개·registry 49개·정책 포함 DependencyAudit 55개가 모두 통과했다. 합성 fixture/임시 Git 저장소에서 보존본 누락/오염·version/ID/GUID 변조·동일 ID 다른 내용·설정/Material 누락·동적 이름 변경·overwrite·권한/부분 복사·부분 재적용 실패·강제 staging·junction·source 없는 재적용·require 부작용을 검사했다. 실패를 숨기는 allowlist나 실제 작업본 staging/reset·Unity 실행은 하지 않았다.

실제 registry/inventory 결과와 전후 생산/기존 산출물·HEAD/index·UTF-8/링크 검증은 [Step 4 최종 검증](20261119_15_Phase2Step4Verification.json)에 기록한다. 별도 사용자 수동 작업은 없다. 실제 참조/Unity 복원 검증과 provider 재취득 절차는 후속 Step/Phase 3에 남긴다.

**Step 4만 추가 완료하며 다음은 Step 5 Git 원본 유입 검사와 필요한 제외 규칙 적용이다.**

## Step 5. Git 원본 유입 검사와 필요한 제외 규칙을 적용한다

5-1. 현재 inventory와 정책을 연결해 HEAD/index/staged의 원본·미등록/혼합·보존 대상 파일을 판정한다. `.gitignore`/중첩 예외를 `check-ignore --no-index`로 읽고 tracked+ignore·`git add -f`·rename/copy·삭제·대소문자·폴더/root meta를 구분한다. 실제 index는 검사에서 변경하지 않는다.

5-2. 별도 `dependency-ingress.cjs`의 `--check-staged`/`--check-managed`를 사용한다(아래 Step 5 결과에서 구현/검증). 유입/정책 gate와 일반 참조 finding을 별도 결과로 출력해, 원본 유입 검사 통과가 전체 참조 정상/권리·복원 PASS로 보이지 않게 한다. 중복 GUID·정책 미연결 등 실제 판정 불가능은 성공한 빈 집합으로 처리하지 않는다.

5-3. 등록된 원본 범위의 강제 staging과 알려진 원본 hash/GUID의 외부 경로 복사·rename도 검사한다. 미등록 ThirdParty 범위는 차단하고 자체 보존 예외에는 명확한 근거를 요구한다. 알려지지 않은 외부 원본을 이름/hash만으로 완전 식별할 수 있다는 보장은 하지 않는다. scope 밖/출처 불명 후보는 한계·필요 확인을 드러낸다.

5-4. `.gitignore` 보완이 필요할 때만 최소 변경한다. 파생/연결 자료와 필요한 meta를 통째로 무시하지 않는다. 현재 추적 해제 대상은 0개이므로 임의 `git rm --cached`를 실행하지 않는다. 새 추적 원본이 발견되면 정확한 목록·재취득/참조/변경본 보존 근거와 작업 범위를 먼저 확정하고, 원본 삭제/이력 재작성과 구분한다.

5-5. 실제 현재 repo에서는 read 검사와 staged 정책 결과·차이를 기록하고, mutation/ignore/예외 실패 사례는 임시 Git 저장소에서 Unit Test한다. 사용자 작업본에 합성 파일을 stage하거나 임의 hook/전역 Git 설정을 설치하지 않는다.

- [x] 현재 추적/staged 원본 유입과 자체 보존 경계를 검사하며 필요한 최소 규칙만 적용했다.

### Step 5 수행 결과 — 2026-11-19

#### 5-1. HEAD/index 실제 blob·ignore·경계 대조 — 완료

`Tools/DependencyAudit/dependency-ingress.cjs`를 추가했다. Git tree와 stage 0 index의 mode/object ID·NUL 경로 및 실제 blob을 고정 read 명령으로 읽는다. staged 후보를 같은 이름의 worktree 파일로 대체하지 않는다. 전체 HEAD/index 각각의 meta GUID 중복·대소문자 충돌·잘못된 mode/unmerged stage·불완전 blob을 실패로 처리한다. root/자식·개별 meta와 등록 경계의 연결을 확인하며 별도 명시 meta 경계는 암묵적 meta 매칭보다 우선한다.

현재 `check-ignore --no-index`의 source/line/pattern·tracked+ignored와 중첩 예외를 별도 출력한다. ignore는 원본 추적 허용 근거가 아니며 HEAD/index 내부 ignore 규칙 재현으로 주장하지 않는다. 입력·HEAD/index·ignore 판정 전후 불변을 확인하고 민감 본문은 blob batch 입력에서 제외한다.

#### 5-2. 실제 명령과 독립 gate 결과 — 완료

새 CLI의 실제 명령은 아래와 같다. 기존 inventory CLI에 미구현 옵션을 전달하지 않는다.

```powershell
node Tools/DependencyAudit/dependency-ingress.cjs --check-managed --evidence AI/90_Tasks/Prototype_10/20261119_16_Phase2Step5KnownOrigins.json
node Tools/DependencyAudit/dependency-ingress.cjs --check-staged --evidence AI/90_Tasks/Prototype_10/20261119_16_Phase2Step5KnownOrigins.json
```

managed gate는 HEAD와 index 양쪽의 차단을 판정하고, staged gate는 전체 projected index를 판정한다. HEAD 결과·실제 add/modify/rename/copy/delete는 별도로 표시해 staged 삭제와 현재 HEAD 추적 상태를 구분한다. exit 0은 해당 gate 통과, 1은 원본/검토 필요 후보 차단, 2는 수집/형식/정책/판정 불완전이다. referenceHealthPass/rightsGranted/independentRestore는 false이며 일반 inventory finding/exit와 독립이다.

#### 5-3. 알려진 원본 지문과 보존 예외 — 완료

[원본 지문 metadata](20261119_16_Phase2Step5KnownOrigins.json)는 Step 4의 고정 manifest/변경 폰트 기록에서 TMP/공급물 83개 경로·SHA-256·UTF-8 CRLF→LF hash·meta GUID·원본/변경 역할을 추출하고 근거 metadata의 해시를 연결한다. 제품/버전/license·권리 값의 별도 등록부가 아니다. 미등록 ThirdParty/root meta·등록 original/unknown/mixed 범위를 차단하고 알려진 원본의 다른 경로 복사/rename와 강제 staging도 검사한다.

보존 예외는 등록부/정책의 modified/derived/project/settings·preserve와 권리/경계·git_boundary/modifications/meta_import/license/evidence/owner_review 근거가 있는 명시 경로만 허용한다. 현재 실제 예외는 0개다. 알려진 순수 원본 byte가 같으면 예외가 있어도 차단하며, 근거 불명 변경본을 자체 소유/추적 허용으로 자동 변경하지 않는다. namespace 밖의 출처 불명 자료·바뀐 payload/LFS·동적/binary·fileID/Unity 타입 전체를 식별한다는 보장은 없다.

#### 5-4. 실제 저장소 결과와 ignore/index 보존 — 완료

두 실제 CLI 모두 complete=true·exit 0이다. 현재 HEAD 933개/index 933개·staged 0개에서 원본/보류 차단 후보는 0개이며 83개 알려진 지문을 대조했다. `.gitignore`는 ThirdParty/root meta·TMP·Ignore 보관 범위를 이미 처리하고 파생/연결 자료는 일괄 제외하지 않는다. 추가 규칙이나 실제 추적 해제 대상이 없어 ignore/index·원본/기존 meta를 변경하지 않았다. `git rm --cached`·실제 staging·hook/전역 Git 설정도 실행하지 않았다.

registry CLI는 실제 항목 0개·exit 0, 일반 inventory는 기존 미해결 참조 31회/sample orphan을 유지한 수집 완료/error 0·exit 1이다. 유입 gate 통과를 참조 정상으로 기록하지 않는다. `.gitignore`와 registry/policy/catalog는 명시한 현재 worktree 입력이며 다른 staged 정책 버전을 보증하는 commit hook은 아니다. 해당 입력 해시와 제약을 결과에 표시한다.

#### 5-5. 임시 Git 정상/실패 Test와 인계 — 완료

새 ingress Test 27개와 기존 registry 49개·정책 포함 DependencyAudit 55개가 모두 통과했다. 강제 staging·tracked+ignore·중첩 예외·rename/copy·삭제·staged/worktree 불일치·Unicode/공백·NUL/개행/tab 파싱·case/mode/unmerged·중복 GUID·원본의 외부 경로 복사·Windows 줄바꿈 정규화·명시 meta 경계·보존 예외의 근거/권리 전후·실제 CLI nonzero·민감 본문/require 부작용을 검증했다. 실제 Windows fixture의 ignored root meta는 force-add를 사용하도록 준비 오류를 수정했다. mutation은 임시 Git 저장소 안에서만 수행했다.

실제 명령/결과와 source hash는 [Step 5 검사](20261119_17_Phase2Step5Check.json), 생산/기존 미커밋 산출물·TMP 보존본/HEAD/index 및 문서 검증은 [최종 검증](20261119_18_Phase2Step5Verification.json)에 기록한다. 사용자 수동 작업은 없다. Unity Compile/Build/Test Runner·Scene/Prefab 변경·의존성 설치/원격 호출은 하지 않았다.

**Step 5만 추가 완료하며 다음은 Step 6 누락/버전 검사와 이식 가능한 재취득·복원 단계다.**

## Step 6. 누락/버전 불일치 검사와 재취득·복원 단계를 만든다

6-1. ID/요청·해결 버전/source·공식 취득 위치/확인 상태·Editor·공급 archive hash·GUID/Import와 필요한 재적용 정보를 검사한다. 누락/버전·해시 불일치·권리/계정/오프라인 제한을 구체 원인/필요 작업으로 출력한다. 재취득 불가를 대체 버전 자동 설치로 숨기지 않는다.

6-2. UPM/builtin 준비 후 TMP archive를 **provider ID/버전 metadata로 발견**하고 도입·변경본/설정 재적용 순서를 정한다. npm local tgz 27개도 Cloud Code provider/버전·archive hash로 찾는다. 현재 cache suffix/Windows 절대 경로를 고정 복원 계약으로 쓰지 않는다.

6-3. npm의 이식성은 안정적인 비추적 archive 준비 경로와 manifest/lock 경로 연결로 해결안을 구체화한다. mapping/제안 diff는 먼저 읽기 전용으로 생성하고 패키지 버전/alias/integrity·동일 archive를 검증한다. 선언/lock 실제 변경이 필요하면 허용 경로/예상 diff·원복을 정한 뒤 적용한다. lock 재생성이 필요하면 그 도구/조건과 필요한 사용자 실행을 지정하며, 임의 version 변경·부분 lock 편집으로 성공 처리하지 않는다.

6-4. NuGet csproj/EmbeddedResource·LogSink 선언은 Unity 원본 제외와 별개로 유지한다. source/feed·해결 graph 확보 방법·동봉 조건·Node/container 입력을 절차로 남긴다. AI가 실제 npm install/ci·dotnet restore·서비스/Docker·원격 게시/빌드를 실행하지 않는다. 실제 필요한 설치/lock 생성은 Step 9 사용자 실행으로 분리하고 독립 checkout 실행은 Phase 3에 둔다.

6-5. cache 경로 suffix 변경·provider 중복/누락·잘못된 버전·archive 오염·상대 경로/플랫폼 separator·alias/lock 불일치·네트워크/계정 제한을 합성 fixture/주입 adapter로 검사한다. manifest/lock은 보존하고 변경 전 기준선과 의도된 경로 변화만 구분한다.

- [x] 필요한 입력 누락/불일치를 식별하고 같은 의존성을 적법하게 다시 준비할 구체 절차/검사 근거가 있다.

### Step 6 수행 결과 — 2026-11-19

#### 6-1. 고정 입력과 누락/버전·graph 판정 — 완료

[복원 spec](20261119_19_Phase2Step6RestoreSpec.json)에 Editor·핵심 선언/lock hash와 TMP/Cloud Code provider ID·version·archive hash/integrity·npm 내부 name/version/alias 및 기존 NuGet graph를 고정했다. 새 `Tools/DependencyAudit/dependency-restore.cjs`는 package metadata에서 provider를 발견하고 Editor·UPM 요청/해결/source/graph·cache dependency graph·필요 archive와 alias/lock 관계를 검사한다. 누락/오염·중복/다른 version·path/graph 불일치와 권리/계정/오프라인 제한은 구체 code/필요 작업으로 출력하며 대체 버전을 설치하지 않는다.

현재 Editor 6000.3.5f2·UPM 61개/91관계의 provider metadata/version/dependencies가 일치한다. 네트워크/계정·원격 endpoint·권리/동봉 조건의 실제 재취득 검증을 이 결과로 대신하지 않는다. 실제 제품 항목은 계속 0개다.

#### 6-2. 공급자 발견과 TMP 재적용 입력 연결 — 완료

TMP archive는 com.unity.ugui 2.0.0의 package.json으로 찾아 기존 고정 SHA-256과 대조했다. Cloud Code tgz 27개도 com.unity.services.cloudcode 2.10.4 metadata로 찾아 gzip/tar를 파일 추출 없이 읽고 package/package.json name/version·archive SHA-256·lock SHA-512 integrity/dependencies를 검사했다. 기존 cache suffix와 Windows 절대 경로를 복원 계약으로 쓰지 않는다.

UPM 준비→같은 TMP archive 확인/도입→Step 4 고정 보존본 검증→변경 폰트/설정/meta/Resources·Shader 계약 재적용→실제 Unity 참조 검사 순서로 영구 절차를 연결했다. 현재 실제 Assets 재적용/Unity 실행은 하지 않았다. source bytes가 같은 공급 archive만으로 변경본 2개가 재현된다는 주장도 하지 않는다.

#### 6-3. 안정적인 npm archive 준비와 읽기 전용 이식 제안 — 완료

정확한 tgz 27개를 `Ignore/DependencyArchives/npm/com.unity.services.cloudcode/2.10.4/<archive_sha256>/<filename>`에 명시적으로 exclusive 복사하고, 재검증 시 동일 byte 27개를 재사용했다. 실제 ignore/비추적·source hash·target 충돌/junction·입력/index 불변을 확인했다. 다른 내용은 덮어쓰지 않으며 부분 실패는 보존하고 일치하는 target만 재사용한다. 원본/유료 payload는 Git/문서/Test에 넣지 않았다.

[검사/제안 결과](20261119_20_Phase2Step6Check.json)에는 manifest 27곳과 lock root/resolved 대응 54곳의 경로 mapping을 기록했다. alias/version/integrity/graph 등 다른 데이터가 바뀌지 않음을 역대조했다. [package.json patch](20261119_21_Phase2Step6NpmPathMigration.patch)는 `git apply --check`를 통과했으며 현재 선언/lock에는 적용하지 않았다. 최초 patch 생성에서 원래 파일의 마지막 newline 부재를 처리하지 못한 준비 오류는 CRLF/EOF marker 보존으로 수정했고 읽기 전용 검사에서 확인했다.

lockOperations는 손으로 부분 lock을 만들어 설치 성공으로 처리하는 방법이 아니다. 실제 이식 적용이 필수가 되면 Step 9/Phase 3의 별도 checkout에서 검토된 manifest patch→사용자 npm lock 생성→실제 후보의 --verify-migration 정적 비교→필요한 사용자 npm ci 순서를 따른다. 원복은 미리 보존한 그 후보의 package.json/package-lock.json에만 한정하고 원래 작업본/QA/저장이나 cache를 reset/clean하지 않는다. 현재 npm install/ci·lock 생성/패키지 버전 변경은 하지 않았다.

#### 6-4. NuGet/EmbeddedResource·LogSink와 실행 경계 — 완료

net9.0·직접 PackageReference 4개와 기존 obj의 직접/전이 graph 13개를 고정/대조하고 공개 feed만 metadata로 기록했다. EmbeddedResource 2개 선언·CloudCode JS 경로 39개와 runtime.js 입력을 확인했다. obj graph는 committed lock이나 이번 restore 결과가 아니다. 실제 graph/lock 확보가 필요하면 별도 환경에서 사용자 dotnet restore --use-lock-file 후 버전/graph/feed·동봉 조건을 대조한다. AI는 restore·Module 빌드/원격 게시·복구를 실행하지 않았다.

LogSink의 외부 npm 의존성 0개와 node:22-alpine 입력을 확인했다. 고정 image digest·계정/네트워크·실제 container 동작은 미검증이며 .env/Secret은 읽거나 복제하지 않았다. 기존 운영/Module 복구 의무는 그대로 남긴다. 영구 순서/담당·중단·원복·조건부 사용자 명령은 Implementation Rule과 DependencyAudit README에 연결했다.

#### 6-5. 정상/실패 Test·Git 지문 연결과 최종 판정 — 완료

새 restore Test 30개, Cloud Code 기술 기준선 연결을 포함한 ingress Test 28개와 기존 정책 포함 DependencyAudit 55개가 모두 통과했다. cache suffix 변경·provider 중복/누락·같은 version의 cache graph 변조·archive 오염/누락·tar path/checksum/link/중복 JSON·BOM/구분자·alias/lock/version/integrity·권한/부분 복사·동일 target 재사용/충돌·junction/ignore·graph drift·네트워크/entitlement 제한과 실제 후보 migration metadata를 합성/임시 fixture로 검증했다.

Cloud Code tgz 지문을 추가한 [복원 원본 지문](20261119_22_Phase2Step6KnownOrigins.json)은 기존 TMP 83개와 27개를 합친 110개이며 근거 metadata hash로 고정했다. 기술 기준선 ID를 기존 ingress 판정에 연결했으며 실제 도입 제품을 만들지 않았다. Step 5의 과거 지문/검사 metadata는 고쳐 쓰지 않는다. 최신 managed/staged gate도 같은 HEAD/index에서 대조한다.

실제 명령·27 archive/제안 patch·원본 선언 보존 근거는 Step 6 Check, 이전 산출물/생산/보존본·HEAD/index·일반 inventory·문서 검증은 [최종 검증](20261119_23_Phase2Step6Verification.json)에 기록한다. 기존 참조 finding과 원격 취득/설치·lock 생성/독립 복원 미검증을 유지한다. 지금 필요한 사용자 수동 작업은 없다. 실제 설치/새 lock이 Phase 2 판정에 필수이면 Step 9에서 필요한 최소 사용자 실행과 결과를 연결하며 완료 전까지 미실행으로 남긴다.

**Step 6만 추가 완료하며 다음은 Step 7 업데이트·제거·원복과 기록 갱신 절차다.**

## Step 7. 업데이트·제거·원복과 기록 갱신을 연결한다

7-1. 같은 ID의 version/source/license·hash·Import/meta·직접/동적 사용처가 바뀌는 순서를 정의한다. 업데이트 전 현재 재현/보존 입력을 남기고, 새 조건/참조/검증 상태를 다시 확인한다. 과거 rights/restore 상태를 새 버전에 자동 승계하지 않는다.

7-2. 제거 전 Scene/Prefab/Material/Script·Resources/Shader/asmdef·동적/binary 한계와 Attribution 영향을 조사한다. 사용처/권한·재복원 제약을 확인할 수 없으면 보존/보류한다. 등록부에는 폐기/변경 이력과 필요한 재복원 정보를 남기며, 실제 자료 삭제를 기본 자동화하지 않는다.

7-3. 실패/중단 시 복구할 원본/설정/meta·선언과 적용 순서를 적는다. 다른 변경을 덮는 저장소 전체 reset/clean·cache 제거로 원복하지 않는다. 원복·부분 적용 실패·GUID/이름 불일치·늦은 설정/조건 변경을 같은 판정 함수의 Unit Test로 검증한다.

- [x] 정상 도입뿐 아니라 업데이트/제거/실패 시 보존·원복·등록 갱신 절차가 정리됐다.

### Step 7 수행 결과 — 2026-11-19

#### 7-1. 같은 ID의 업데이트/이동과 상태 재검토 — 완료

Implementation Rule에 업데이트·경로 변경·제거·실패 원복의 담당/순서·중단·보존/갱신 기준을 영구 절차로 정리했다. 업데이트 전 현재 version/source/hash·원본/변경/설정·자산/폴더 meta·선언/lock과 참조/Attribution·재현 입력을 보존하고 새 조건/사용처/Import·검증 상태를 같은 ID의 후보 기록으로 분리한다. source 조건·version/hash가 바뀐 것을 과거 rights/restore 상태로 자동 승인하지 않는다.

기존 registry.validateTransition의 재검토 입력을 version/source/license/hash 외에 usage_conditions/notices/modifications/purpose/import_path/game_usage/references/meta_import/git_boundary/restore/lifecycle까지 확장했다. 이전 확인 상태를 유지하려면 바뀐 입력에 대한 새 근거가 필요하며 상태 네 축을 연쇄 성공으로 바꾸지 않는다. 빈 실제 등록부는 그대로 유지했다.

`dependency-lifecycle.cjs`의 scopeDigest/evaluate는 before/candidate 기록·실제 baseline·변경/backup·참조/한계·GUID/Import/name·적용표와 검토 근거를 한 digest에 묶는다. 소유자 결정과 여섯 검토(rights/attribution/references/guidImport/names/restore)가 같은 범위를 가리키지 않거나 늦은 조건/경로/설정 변경이 있으면 보존/보류한다. 현재 등록부와 후보 snapshot·다른 ID 기록/정책도 대조한다.

경로 이동은 같은 source/version/hash에서 동일 byte와 meta/GUID를 유지하는 출발/도착 쌍 및 사용자 Editor 적용표를 요구한다. 버전 변경과 단독 삭제를 이동으로 숨기지 않으며 이 도구가 실제 파일을 이동/삭제하지 않는다. 새 경로와 Resources 상대 이름/Shader·사용처/복원 경계를 재검토한다.

#### 7-2. 제거·사용처 해소·등록/Attribution 보존 — 완료

제거 전 Scene/Prefab/Material/Script·Resources/Shader/asmdef·GUID/fileID·동적/binary 한계와 배포 고지/재복원 제약을 확인한다. 불완전 참조 scope·미해결 incoming edge·권리/Attribution/보존 근거 부족은 보류한다. 이름 기반 사용/동적·binary의 해당 없음도 사유 근거를 요구하며 조용히 빈 성공으로 취급하지 않는다.

제거 후보 기록은 같은 ID의 폐기 상태·최종 version/source와 필요한 변경 이력·Attribution/재복원 의무를 남긴다. 기록 자체를 삭제하지 않는다. 연결 해소와 실제 자료 삭제를 분리하며 삭제는 기본 자동화하지 않는다. Scene/Prefab 변경이 필요하면 AI는 정확한 Hierarchy/컴포넌트/필드·기존→최종값·저장/원복·정적 확인표와 필요한 Unity suite를 준비하고 사용자가 적용한다.

#### 7-3. 실패/부분 적용 원복·정상/실패 검증 — 완료

원복은 검증된 before 입력과 시도한 after hash·현재 hash를 경로별로 대조한다. 현재가 before이면 그대로 두고, 그 시도의 after일 때만 명시 경로의 보존 byte 재적용 계획을 만든다. 다른/늦은 파일·선언/등록부 변경이면 덮어쓰지 않는다. 새 출력은 검토용으로 보존하며 자동 삭제하지 않는다. 원본/설정/meta→필요한 사용자 이동/Scene 연결표→명시한 선언/lock→GUID/Import/name·참조/조건·기록 재검증 순서를 정리했고 저장소 전체 reset/clean·cache 삭제를 원복으로 사용하지 않는다.

현재 관측은 --plan 요청의 주장이 아니라 실제 작업본 hash·meta GUID/Import bytes·Resources/Shader 이름에서 읽는다. 자산/companion meta·Import hash와 before/attempt 입력, Resource 물리 경로/이름을 연결해 틀린 metadata를 검출한다. partial/late 상태·잘못된 GUID/name·늦은 설정/조건·미검증 backup·원복 범위 불일치는 보류한다. 소유자/metadata 주장은 권리 진위·backup payload·Unity 실행을 인증하지 않으며 실제 적용 직전/직후 재검증이 필요하다.

새 lifecycle Test 42개, 확장 registry 50개, ingress 28개와 기존 정책 포함 DependencyAudit 55개가 모두 통과했다. 같은 evaluate를 사용해 업데이트·이동 쌍·제거·부분/반복 원복·관련 없는 변경·새 출력 보존·실제 CLI 관측·가상/늦은 등록부·GUID/Import/이름/조건 변경·Scene 적용표·junction/미존재·민감 오류/require 부작용을 합성 metadata/임시 Git 저장소로 검사했다.

실제 lifecycle --check는 registry 항목 0개·실제 lifecycle 요청 0개·exit 0이며 업데이트/제거/원복 적용 성공으로 기록하지 않는다. 실제 명령/결과·source hash는 [Step 7 검사](20261119_24_Phase2Step7Check.json), 생산/기존 산출물·보존본/archive·HEAD/index·문서 검증은 [최종 검증](20261119_25_Phase2Step7Verification.json)에 기록한다. 현재 사용자 수동 작업/실제 자산 적용표는 없다. Unity Compile/Build/Test Runner·Scene/Prefab 변경·실제 자산/등록부/선언 원복·설치·원격 호출은 하지 않았다.

**Step 7만 추가 완료하며 다음은 Step 8 부작용 감사·전체 적용 범위 Test/현재 저장소 검증이다.**

## Step 8. Unit Test와 현재 저장소 검사로 적용 결과를 검증한다

8-1. 쓰기/삭제·프로세스/네트워크·Unity 호출을 먼저 감사한다. 수집/판정·제안 출력과 명시적 보존/적용 기능을 분리하고, require/main·기본 check의 읽기 전용 성질을 유지한다. 기존 suite를 재사용/확장하며 과거 건수를 정답으로 고정하지 않는다.

8-2. 아래 정상/실패를 실제 구현 함수·CLI로 검증한다. 실패를 고친 후 영향 suite를 다시 실행하고 Test 이름/원인·건수/exit·한계를 기록한다. 새 외부 package 없이 Node/assert·합성 metadata/임시 repo를 우선 사용한다.

| Unit Test 영역 | 필수 확인 |
| --- | --- |
| 등록/정책 | 필드/ID/경로 중복·표 escape/개행·잘못된 상태/근거·미등록/미확인·0개 항목·명시적 사용자 결정 전후 |
| Git 유입 | tracked+ignore·강제 staging·중첩 예외·rename/copy·삭제·공백/한글/NUL/대소문자·원본과 파생/root/자식 meta 경계 |
| 보존/경로 | 외부/상위 경로·junction/symlink·권한/부분 열거·overwrite 거절·부분 복사·오염/버전/GUID/Import/설정 손실·원본 불변 |
| 복원 입력 | JSON/BOM·graph/source/alias/lock·cache/provider/버전/해시 불일치·재취득 제한·source 경로 변경·누락을 빈 성공으로 숨기지 않음 |
| 참조/lifecycle | GUID/subasset·Resources/Shader·dynamic/binary 한계·변경/업데이트/제거·원복·고지/권리 상태 재확인 |
| 보안/통합 | 원본/비밀값 미노출·read 전후 index/기준 파일 동일·실제 CLI/adapter 오류·실패 nonzero·동일 입력 출력·유입 gate와 참조/복원 결과 독립 |

8-3. 현재 `node Tools/DependencyAudit/dependency-audit.test.cjs`와 `--check`는 존재하는 명령이다. 추가한 옵션/보조 CLI는 실제 구현/감사 후 정확한 명령으로 기록한다. 결과는 명시한 Task metadata 경로에 UTF-8로 보존한다.

8-4. 실제 repo의 변경/staged·현재 정책/등록/ignore·누락/버전 검사와 보존본/필요 설정을 대조한다. 기존 raw finding을 allowlist로 지워 전체 성공을 만들지 않는다. 의도된 ignore/선언 변경은 새 예상 상태로 검증하고, 과거 baseline JSON을 덮어써 검사를 통과시키지 않는다.

- [x] 필요한 정상/실패 Unit Test와 실제 적용 범위의 검사·전후 보존 근거가 있으며 도구 결함을 해결했다.

### Step 8 수행 결과 — 2026-11-19

**8-1. 부작용 감사.** 생산 모듈 7개의 쓰기/삭제·프로세스·네트워크·require/main을 확인했다. 쓰기는 명시적 preservation.create, preservation.reapplyToTemporary, restore.prepareArchives에 한정되고 생산 삭제 기능은 없다. 기본 check/verify·도입/lifecycle 판정은 읽기 전용이며 프로세스는 고정 Git read 명령만 허용한다. 전체 모듈의 require를 쓰기/프로세스/네트워크 차단 상태에서 확인했다. 실제 check도 쓰기/네트워크·허용되지 않은 Git 호출을 차단한 상태에서 동일 결과를 얻었으며 Git read 48회만 실행됐다. 동시 공격적 파일 교체를 막는 OS sandbox 검증은 아니다.

**8-2. 전체 정상/실패 Test.** 기존 Node/assert·합성 metadata/실패 주입·임시 Git fixture의 7개 suite를 재사용했다. audit(정책 포함) 55, registry 50, adoption 36, preservation 20, ingress 28, restore 30, lifecycle 42개로 **총 261개 통과/실패 0·모두 exit 0**이다. 등록/상태/근거·Git 경계/강제 유입/Unicode·meta/GUID/Import/이름·경로/link/부분 실패·archive/graph/alias/lock·업데이트/제거/원복·비밀값/실제 CLI 실패를 검증했다. 새 외부 package나 생산 코드 변경 없이 전체 suite가 통과했으며 이번 실행에서 추가 도구 결함은 발견되지 않았다. Test 이름(리터럴 목록과 생성형 case의 고정 source), 명령/건수/exit와 한계는 아래 검사 metadata를 따른다. 과거 건수를 성공 조건으로 고정하지 않았다.

**8-3. 실제 CLI/독립 판정.** inventory·registry·보존본 verify·managed/staged ingress·restore·lifecycle의 7개 실제 CLI를 각각 2회 실행해 같은 입력의 stdout 일치를 확인했다. inventory는 수집 완료·오류 없이 **exit 1**이며 기존 미해결 참조 31회와 sample orphan 1개의 raw 진단이 Step 1과 정확히 같다. allowlist로 삭제하거나 참조 정상으로 바꾸지 않았다. 나머지 검사는 exit 0이다. registry/실제 lifecycle 작업은 0개, ingress의 알려진 지문은 110개, 복원 입력은 UPM 61개/edge 91개·tgz 27개이며 보존본 83파일/meta/빈 폴더를 검증했다. 도구 통과는 실제 도입·권리/동적·binary/Unity 해석·독립 복원 성공이 아니다.

**8-4. 전후 보존/완료.** [Step 8 검사](20261119_26_Phase2Step8Check.json)에 전체 suite·정확한 명령/출력·부작용 차단·입력 1,041개 전후 비교를, [최종 검증](20261119_27_Phase2Step8Verification.json)에 기존 1,007파일·누적 허용 문서 8개/그 외 999개 불변·이전 metadata/도구·TMP 보존본/npm archive 27개·선언/lock/ignore·HEAD/index/staged 대조와 최종 문서 상태를 연결한다. 이전 기준선/metadata를 덮어쓰지 않았다. 새 검사 metadata/문서만 변경했고 Scene/Prefab·C#/설정·원본/저장/Secret·QA 실행 파일은 보존했다. 이번 Step에 필요한 사용자 수동 작업은 없으며 Unity Build/Test Runner·설치/원격·실제 자산 적용은 수행하지 않았다.

**Step 8만 추가 완료하며 다음은 Step 9 필요한 사용자 작업과 실제 Unity 검증의 필요성 판정/연결이다.**

## Step 9. 필요한 사용자 작업과 실제 Unity 검증만 연결한다

9-1. 자동/정적으로 해결 불가하고 실제 적용을 막는 항목만 `대상/사실/부족한 정보/선택/영향`으로 제시한다. 기존 보존 기준으로 진행할 수 있으면 재확인을 요구하지 않는다. 필수 결정은 답변을 기다리고 무응답을 승인으로 바꾸지 않는다.

9-2. 문서/Tools·metadata만 바뀌고 생산 자산/설정·C# 변화가 없으면 불필요한 Compile/Runner/Build를 요청하지 않는다. C# Test·Import/연결 자산/설정 변화나 실제 타입/참조 해석이 필수면 최소 읽기 전용 EditMode/관련 PlayMode suite를 준비하고 사용자가 실행한다.

9-3. Scene/Prefab은 AI가 수정하지 않는다. 필요할 때 Hierarchy/컴포넌트/필드·기존→최종값·onClick/Navigation·저장/원복·정적 전후 확인표를 작성한다. UI 동적 생성/자동 참조 보충으로 검증을 우회하지 않는다. 사용자는 지정한 실행 결과/정확한 실패·예상 밖 로그만 전달한다.

9-4. 실제 lock 생성/설치가 Phase 2 완료에 필요하면 최소 명령·경로·환경·변경/원복·성공/중단 기준을 먼저 작성하고 해당 사용자 결과를 연결한다. 서비스/Module 배포·복구나 독립 복원 전체를 이 Step에 섞지 않는다. 필수 미실행/실패가 남으면 Phase 2 미완료다.

- [x] 필요한 결정/사용자 실행은 연결됐거나 불필요 근거가 있고 필수 실패/미실행을 완료로 표시하지 않았다.

### Step 9 수행 결과 — 2026-11-19

**9-1. 현재 필수 결정/사용자 작업 0개.** 실제 등록 제품·도입/제외·lifecycle 요청은 0개다. 현재 기술 기준선의 변경본/조건 미확인 자료는 기존 결정대로 보존하며 다시 승인을 요구하지 않는다. 정책/registry·1,007개 기준 파일과 생산 inventory 경로/추적/ignore를 대조했고 누적 문서 8개 외 999파일, 생산 자산/설정·C#·선언/lock은 그대로다. 권리/계정/재취득 조건을 새로 확인한 것으로 기록하지 않는다. 현재 적용을 막는 새 사용자 결정은 없으며 향후 실제 요청이 생기면 대상/사실/부족한 정보/선택/영향을 구체화한다.

**9-2. 현재 Unity 실행 불필요.** 변경은 문서·Node Tools·metadata이며 Step 8 이후 생산/Test 코드 변경도 없다. 새 C# Test·Import/연결 자산/설정 변경이 없어 Compile/EditMode/PlayMode/Build를 요청하지 않는다. Step 8의 7개 suite/261개 통과는 source hash와 이전 metadata를 대조해 유지했으며 동일 Test를 재실행하지 않았다. 현재 inventory는 수집 완료·기존 참조 미해결 31회/sample orphan 1개로 exit 1을 유지한다. 정적 검사로 Unity 타입/subasset·동적/binary 참조·렌더링 정상이나 독립 복원을 증명하지 않는다. 실제 관련 자산 변경/도입·독립 복원 전 영향과 최소 Unity suite 필요성을 다시 판정한다.

**9-3. 현재 Scene/Prefab 적용표 불필요.** 변경 대상이 없으므로 가상 Hierarchy/컴포넌트/필드·onClick/Navigation 수정을 만들지 않는다. 향후 실제 작업에서 필요하면 AI가 대상 Scene/Prefab·Hierarchy 경로/컴포넌트/필드·기존→최종값·저장/원복·정적 전후 확인표와 최소 관련 Test를 준비하고 사용자가 적용/실행한다. 동적 UI 생성이나 자동 참조 보충으로 대체하지 않는다. AI는 Scene/Prefab을 수정하지 않았다.

**9-4. lock/설치 필요성 판정과 조건부 연결.** 현재 범위는 도입/복원 관리 절차·입력 준비이며 실제 적용 요청은 없다. 원래 선언/lock을 유지한 채 provider/고정 archive·보존본과 경로 제안/실패 검증을 확보했으므로 이번 Step에 새 lock 생성/설치를 필수로 요구하지 않는다. Step 6의 package.json patch/lock mapping은 제안이고 실제 npm lock 생성·npm 설치·NuGet 새 restore/lock·독립 복원은 **미실행**이다. 최종 Phase 2 조건 판정은 Step 10에 남기며 이후 필수 실행으로 판정되면 결과 없이 완료 처리하지 않는다.

| 조건부 후속 대상 — 지금 실행할 작업 아님 | 발동 조건과 실행/검증 연결 | 성공/중단·원복 |
| --- | --- | --- |
| npm 경로 이식/lock | 실제 별도 checkout의 경로 이식/복원이 필요할 때 [도구 안내의 사용자 순서](../../../Tools/DependencyAudit/README.md)를 따른다. 정확한 Editor/provider/archive·사용 조건과 npm/toolchain을 확인하고 원래 작업본·QA/저장/Secret을 보호한다. 검토된 manifest patch 적용 후 그 checkout 루트에서 사용자 `npm install --package-lock-only --ignore-scripts --no-audit --no-fund`, AI의 실제 후보 `--verify-migration`, 필요한 사용자 `npm ci --ignore-scripts --no-audit --no-fund` 순서다 | 실제 exit/후보 manifest/lock·오류/예상 밖 로그를 연결한다. 지정 경로 외 alias/version/integrity/graph 변경·자료/권리 부족·실행 실패는 중단한다. 원래 baseline을 새 결과로 덮어쓰지 않는다. 별도 환경의 그 시도에 해당하는 package.json/package-lock.json만 검증된 before hash로 원복하고 새 출력/실패 근거를 보존한다. reset/clean·cache 청소는 하지 않는다 |
| NuGet graph/lock | 실제 별도 환경의 새 .NET 복원이 필요할 때 그 프로젝트 루트에서 사용자 `dotnet restore UGS/Modules/FlowStateVerification/FlowStateVerification.csproj --use-lock-file` 실행 후 실제 graph/lock·exit/로그를 연결한다. 현재 local obj를 새 restore 성공으로 사용하지 않는다 | net9.0·직접 4개/기존 전이 13개 graph·feed/EmbeddedResource와 대조한다. 버전/feed/리소스 drift·실패·예상 밖 로그는 중단한다. csproj/source를 유지하고 그 시도의 변경만 before 상태로 원복하며 새 graph/실패 출력을 보존한다. Module 빌드/배포·복구는 범위 밖이다 |
| Unity/Scene 검증 | 실제 C#/Import/설정/연결 자산 변화 또는 타입/subasset/렌더링 해석이 필수일 때 AI가 최소 EditMode/관련 PlayMode와 필요한 사용자 적용표를 준비한다 | 사용자가 Editor/Runner/Build를 실행하고 실제 결과/정확한 실패·예상 밖 로그를 전달한다. 필수 미실행/실패를 통과로 처리하지 않는다. 해당 시도의 자산/meta/설정과 사용자 Scene/Prefab을 정해진 보존본/표로 원복한다 |

[Step 9 판정](20261119_28_Phase2Step9Check.json)에 9-1~9-4의 대상/사실/미확인/선택/영향·조건부 명령과 1,043개 입력 전후 비교를, [최종 검증](20261119_29_Phase2Step9Verification.json)에 문서 상태/링크·이전 metadata/도구·생산 입력·TMP 83파일/폴더 meta 8개·archive 27개·HEAD/index 보존을 연결한다. 사용자 응답을 가정하거나 과거 Unity 결과를 새 성공으로 승계하지 않았다.

**Step 9만 추가 완료하며 다음은 Step 10 Phase 2 최종 판정과 Phase 3 인계다. 현재 수행할 사용자 수동 작업은 없다.**

## Step 10. Phase 2 완료를 판정하고 Phase 3에 인계한다

10-1. Roadmap의 세 조건을 아래 증거에 연결한다.

| Phase 2 완료 조건 | 최종 증거 |
| --- | --- |
| 관리 대상 원본의 현재 추적/신규 staged 유입 검사 | Step 2/5/8 실제 정책/경계·현재 결과·강제 유입 등 실패 Unit Test. 현재 대상 0개와 실제 추적 해제/원본 삭제를 구분 |
| 누락/버전 불일치 식별과 적법한 재도입 절차 | Step 3/4/6/8/9 공급/버전/hash·보존/재적용 입력·구체 실행/중단·조건/제한과 필요한 실제 결과 |
| 원본 직접 수정 없는 확장 경계·설정 재현 | Step 3/4/7~9 원본/변형/설정·meta/GUID/Import·사용처·보존/업데이트/원복·검증 계약 |

10-2. 모두 충족하면 본 Task/Roadmap/Memory·영구 절차/기록을 갱신해 **Phase 2만 완료**한다. 도구 준비/기존 cache·일반 참조 finding과 실제 적용/권리·독립 복원 성공을 구분한다. 실제 제외 대상이 없다는 이유로 보존/유입 검사·복원 절차를 생략하지 않는다.

10-3. Phase 3에 새 기준선/허용 변경·고정 Editor/버전/source/hash·취득/계정/오프라인 제한·필요 보존 입력·명령/예상 결과·같은 GUID/Import/직접·동적 참조·조건부 Unity suite/사용자 적용표를 넘긴다. 원본/Secret을 인계 문서에 복제하지 않는다.

10-4. 원래 작업본/변경본/QA/저장을 보존하는 별도 환경 계획·성공/중단 기준을 확정한다. 필수 자료/권리/참조/실행 실패나 예상 밖 로그가 있으면 멈추며, URP/TMP graph 미해결 31회와 sample orphan은 실제 영향/필수 여부를 재검증한다. Phase 3 실제 독립 복원 전에 그 성공을 선언하지 않는다. Roadmap 11~14 실제 원본 도입 전에는 Roadmap 10 전체 완료가 필요하다. 기존 실제 Module 업데이트 전 복구 시험 의무도 유지한다.

- [x] Phase 2의 실제 판정과 Phase 3 입력/보존·성공/중단·미확인 의무를 정확히 기록했다.

### Step 10 수행 결과 — 2026-11-20

**10-1. 세 완료 조건 판정.** [현재 검사](20261120_01_Phase2Step10Check.json)에 실제 CLI 7개와 세 조건별 증거/범위를 기록했다. 판정 범위는 현재 제품/요청 0개에서 향후 반복할 관리 절차·구현된 검사와 보존 입력 준비다.

| Phase 2 조건 | 완료 근거 | 실제 적용/복원과 구분 |
| --- | --- | --- |
| 원본 현재 추적/신규 staged 유입 검사 | Step 2 registry/정책·Step 5 Git blob/강제 유입 실패 Test·Step 6 지문 110개·Step 8 전체 검사와 오늘 managed/staged CLI exit 0 | 현재 추적 경로 933개/staged 0개·실제 제외 후보 0개다. 대상이 없다는 이유로 검사를 생략하지 않았으며 실제 untrack/delete는 하지 않았다 |
| 누락/버전 불일치 식별·적법한 재도입 절차 | Step 3 사전 검사·Step 4 TMP schema 2/83파일/폴더 보존·임시 재적용·Step 6 공급 metadata/고정 tgz 27개·graph/alias/hash/lock/누락 실패와 취득 제한·Step 9 조건부 실행/중단/원복 | 현재 check로 UPM 61개/edge 91개·고정 Editor/provider/archive를 대조했다. 실제 계정/조건 확인·새 lock/설치와 독립 복원은 미실행이며 해당 시점의 필수 증거 없이 성공 처리하지 않는다 |
| 원본 직접 수정 없는 확장 경계·설정 재현 방법 | Step 3 별도 project root/파생 GUID·source/meta hash·Step 4 변경본/설정/GUID/Import/이름/폴더 보존·Step 7 업데이트/이동/제거/부분 원복·Step 8 정상/실패 suite·Step 9 사용자 적용/검증 계약 | 실제 원본/생산 자산·Scene/Prefab·선언/lock을 바꾸지 않았다. 타입/subasset·동적/binary·렌더링과 실제 자산 적용 성공은 별도 검증이다 |

**10-2. Phase 2만 완료.** Step 1~9 증거와 오늘 현재성 대조로 세 조건을 충족해 이 Task와 Roadmap/Memory·관리 기록·도구 안내를 갱신한다. 생산/Test 코드가 Step 8 이후 같아 7개 suite/261개 통과 근거를 hash로 연결했고 재실행하지 않았다. 현재 registry/managed/staged/보존본 verify/restore/lifecycle은 exit 0, inventory는 수집 완료·기존 raw 진단과 동일한 exit 1이다. 일반 참조 finding을 제거하거나 참조/권리/독립 복원 통과로 합치지 않는다. 현재 필요한 사용자 작업은 0개이며 Unity Build/Runner·Scene/Prefab 수정·설치·원격 실행은 하지 않았다.

**10-3. 고정 입력/새 기준선 인계.** [Phase 3 인계 계획](20261120_02_Phase3Handoff.md)에 Editor 6000.3.5f2·UPM/TMP/Cloud Code 버전/source/hash·선언/lock·보존본/폴더·27개 archive·GUID/Import/이름/참조/조건 미확인·실제 명령/예상 결과·조건부 사용자 표/Test·취득/계정/오프라인 경계를 연결했다. [최종 검증](20261120_03_Phase2Step10Verification.json)의 새 기준선은 원래 작업본 보호용 전체 안전 hash 목록과 별도 환경의 안전 전달/허용 변경 계약을 구분한다. 미커밋 도구/문서가 HEAD만으로 전달되지 않는 한계와 안전 hash 기준선 밖 추적 경로 11개도 인계하며 미검사 본문을 복제하지 않는다.

**10-4. 별도 환경/성공·중단 확정.** 별도 root·사용자 데이터 분리를 Phase 3 시작 전에 구체화하고, 원래 작업본/QA/저장/Secret·HEAD/index를 보존하며 안전 경로 snapshot과 검증된 비추적 입력만 전달한다. 전체 history/cache/원본/Secret 복제·원본 삭제·reset/clean은 하지 않는다. 필수 자료/권리·경로/버전/hash/GUID/Import drift·미확인 필수 참조·실행 실패/예상 밖 로그는 중단한다. URP 11 GUID/15회·TMP graph 1 GUID/16회 미해결과 sample orphan 1개는 실제 영향/필수 여부를 다시 검증하고 불가능한 대상의 복원 성공은 보류한다. 부분 원복은 그 시도의 이름 붙인 before/attempt/current 파일만 대조하고 새 출력/실패 근거를 보존한다. 다음 실제 Module 업데이트 전 복구 시험, Roadmap 11~14 실제 원본 도입 전 Roadmap 10 전체 완료 의무를 유지한다.

**Step 10 및 Phase 2 완료. 다음은 Phase 3 착수 시 인계 기준선/별도 환경·필수 입력을 확인하고 실행 Task를 준비하는 것이다. Phase 3 실제 실행/독립 복원과 Roadmap 10 전체는 미완료다.**

# 영향 범위

누적 변경은 Step 1~7의 관리/보존·복원/lifecycle 도구·절차에 이어 Step 8의 전체 검사와 Step 9의 사용자 실행 필요성 판정/조건부 후속 실행·각 검사/최종 검증 metadata와 문서 상태/명령 안내다. Step 10에서 현재 완료 조건/Phase 3 인계 계획과 새 기준선을 추가했다. Step 8~10에서는 생산/Test 코드를 변경하지 않았다. 이전 metadata와 inventory/ingress/adoption/preservation/restore 코드·TMP 보존본/npm archive를 보존한다. 실제 제품 등록·ignore/index·원본/meta·Scene/Prefab·package 선언/lock·실제 저장/Secret·서버/QA 후보는 바꾸지 않는다. 실제 lifecycle 적용/삭제/원복은 하지 않았다. 각 Step의 변경/이유와 검증 결과는 해당 Step에 기록한다.

# 검증 내용

- 템플릿 섹션·Step 1~10 순서/미체크·담당·실제 입력/출력·완료 조건과 Roadmap 3조건의 연결 확인.
- 현재 존재하는 파일/명령과 제안 경로/옵션을 구분하고 상대 링크·날짜/번호·인코딩/공백 확인.
- 실제 애셋 없음·기존 변경/미확인 보존·유입 검사와 참조/권리/독립 복원의 독립 판정 확인.
- 정상/실패 Unit Test·격리/읽기 전용·명시적 보존/적용 범위와 필요한 사용자 작업·Scene/Unity 경계 확인.

# 검증 결과

작성 시 Phase 1 최종 인계·Roadmap·관리 기록/형식·규칙·현재 CLI를 읽기 전용으로 대조했다. 관련 문서 5개·로컬 상대 링크 106개·일반 Task 템플릿의 필수 섹션 13개, Step 1~10 순서·완료 0개/미완료 10개와 같은 날짜 번호 01~05의 중복 없음을 확인했다. 제안 정책/보존 helper는 아직 없고 기존 CLI/도구·실행 파일은 변경하지 않았다. 기존 기준 입력 66개·HEAD/index/QA 후보 보존·생산/Tools 실행 파일 변경·staged 변경 0개와 UTF-8/공백, `git -c core.safecrlf=false diff --check`를 확인했다. 기존 Unit Test/검사 결과는 선행 근거로만 연결했으며 같은 suite를 다시 실행하지 않았다. 실제 Step 1~10·새 Unit Test/CLI·원본 보존/추적 해제·Unity/설치·독립 복원은 미실행이다.

# 후속 작업

Step 1 실행 검증은 새 기준선·현재 검사와 [최종 검증](20261119_08_Phase2Step1Verification.json)을 따른다. 위 검증 결과 문단은 최초 절차 문서 작성 당시 기록으로 보존한다. Step 1 당시 완료 1개/미완료 9개였으며 Unity·빌드·Runner·설치·Scene 변경은 하지 않았다.

Step 2 실행 검증은 [검사 결과](20261119_09_Phase2Step2Check.json)와 [최종 검증](20261119_10_Phase2Step2Verification.json)을 따른다. Step 2 당시 완료 2개/미완료 8개였다.

Step 3 실행 검증은 [검사 결과](20261119_11_Phase2Step3Check.json)와 [최종 검증](20261119_12_Phase2Step3Verification.json)을 따른다. Step 3 당시 완료 3개/미완료 7개였다.

Step 4의 보존 spec/검사와 [최종 검증](20261119_15_Phase2Step4Verification.json)을 연결했다. Step 4 당시 완료 4개/미완료 6개였다.

Step 5의 [검사](20261119_17_Phase2Step5Check.json)와 [최종 검증](20261119_18_Phase2Step5Verification.json)을 연결했다. Step 5 당시 완료 5개/미완료 5개였다.

Step 6의 spec/검사/제안 patch와 [최종 검증](20261119_23_Phase2Step6Verification.json)을 연결했다. Step 6 당시 완료 6개/미완료 4개였다.

Step 7의 [검사](20261119_24_Phase2Step7Check.json)와 [최종 검증](20261119_25_Phase2Step7Verification.json)을 연결했다. Step 7 당시 완료 7개/미완료 3개였다.

Step 8의 [전체 검사](20261119_26_Phase2Step8Check.json)와 [최종 검증](20261119_27_Phase2Step8Verification.json)을 연결했다. Step 8 당시 완료 8개/미완료 2개였다.

Step 9의 [실행 필요성 판정](20261119_28_Phase2Step9Check.json)과 [최종 검증](20261119_29_Phase2Step9Verification.json)을 연결했다. Step 9 당시 완료 9개/미완료 1개였다.

Step 10의 [현재 검사/완료 조건](20261120_01_Phase2Step10Check.json)·[Phase 3 인계](20261120_02_Phase3Handoff.md)·[최종 검증/새 기준선](20261120_03_Phase2Step10Verification.json)을 연결했다. 현재 완료 10개/미완료 0개이며 **Phase 2만 완료(2026-11-20)**다.

**다음은 Phase 3 착수 시 인계 기준선·별도 환경/필수 입력 확인과 실행 Task 준비다.** 현재 사용자 수동 작업은 없으며 독립 복원/Roadmap 10 전체는 미완료다.

# 관련 문서

- [AI 진입점](../../README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [Project 안내](../../00_Project/README.md)
- [관리 기록](../../00_Project/ASSET_DEPENDENCIES.md)
- [등록 형식](../../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)
- [Roadmap 10](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_010.md)
- [공통 순서/검증 경계](../../04_Implementation_Roadmap/README.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [Investigation Rule](../../01_Rules/INVESTIGATION_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [Coding Style](../../01_Rules/CODING_STYLE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
- [Tasks 안내](../README.md)
- [현재 검사 도구](../../../Tools/DependencyAudit/README.md)

# 관련 작업 기록

- [Phase 1 완료/최종 인계](20261118_02_Phase1ManualSteps.md)
- [기존 최종 정적 검토/한계](20261119_04_Phase1Step9Review.json)
- [기존 보존 정책](20261119_03_Phase1Step8Policy.json)
- [Roadmap 9 QA/운영 의무](../Prototype_9/20261118_01_Phase4ManualSteps.md)

# 작성 완료 기준

- 실제 순서/산출물·정상/실패 검사·필요 사용자 작업·완료/중단 조건을 Step으로 명시한다.
- 원본/프로젝트 소유/변경/설정·조건/metadata와 코드·실제 적용/복원을 구분한다.
- 최초 작성과 각 Step의 실제 수행 증거를 구분한다. 현재 Step 1~10/Phase 2 완료 근거를 연결했으며 미실행 Phase 3·독립 복원·Roadmap 전체를 완료로 기록하지 않는다.
