# 작업 정보

## 작업명

Prototype 10 Phase 1 — Thirdparty 원본·프로젝트 소유 자료 경계 확정

## 작업 일자

20261118

## 작업 담당자

- AI: 기준선/추적·의존성·참조/출처 조사, 분류표·검사 도구/Unit Test 준비, 정적/오프라인 검사, 사용자 확인 대상 최소화, 결과/다음 Phase 인계.
- 사용자: 로컬 자료만으로 확인할 수 없는 취득/사용 조건·프로젝트 소유 변경의 결정, 필요한 경우에만 Unity Compile/Test Runner 실행과 Scene/Prefab 적용.

## 작업 상태

**Step 1~10 및 Phase 1 완료(2026-11-19).** 향후 애셋의 보존/제외 경계·영구 기록/갱신 형식·의존성/참조 조사와 Node 검사/정책 검증을 완료했다. 현재 사용자 도입 애셋/실제 제외 후보는 0개이며 실제 추적 제외·도입/복원 절차 적용과 독립 복원·Roadmap 10 전체는 미완료다. 기존 조건 미확인/변경본 보존과 raw finding은 실제 적용/복원 전 의무로 인계했다.

# 작업 목적

## 사용자 확정 전제와 산출물 — 2026-11-18

사용자는 현재 사용 중인 Third Party 애셋이 없으며, 앞으로 사용할 애셋의 관리 방안을 정하는 것이 이번 작업의 목적이라고 명확히 했다. 이는 사용자 도입 현황에 대한 전제이며 로컬 파일 전수 조사 결과가 아니다. 기존 UPM/TMP·보조 패키지 의존성은 관리 기준을 설계하고 프로젝트 참조를 보존하기 위한 기술적 기준선으로 구분한다.

Phase 1은 향후 애셋의 등록 항목과 원본/복사본/수정·파생물/프로젝트 소유 자료의 경계를 정의하고, Phase 2는 등록·도입·복사/수정·Git 보존/제외·업데이트/제거·복원 절차와 검사를 적용한다. Phase 3은 복원 절차를 검증하고 후속 도입 기준으로 인계한다. 실제 도입 전인 애셋의 제품·출처·라이선스·사용처를 임의로 채우거나 구매/도입을 착수 조건으로 만들지 않는다.

Step 2~5의 현 상태 조사는 위 기준 설계에 필요한 범위로 수행한다. 도입 애셋이 없다는 이유로 과거 구매/사용 내역을 사용자에게 요구하지 않는다. 해당 없는 항목은 이유와 함께 해당 없음, 향후 도입 때 필요한 정보는 미등록/도입 전 확인으로 구분한다. Step 6에서는 앞으로 반복 사용할 관리 기록의 형식·위치·갱신 시점을 반드시 정한다. Step 1의 기준선 확보 완료는 유지하며 Step 2~10의 실행 완료를 뜻하지 않는다.

어떤 외부 원본을 비추적으로 관리하고 어떤 프로젝트 소유 자료/의존성 선언을 남길지 근거와 함께 확정한다. 실제 경로/내용·출처·참조를 기준으로 분류하며 폴더명이나 현재 ignore만으로 소유권/복원 가능성을 판단하지 않는다.

정적으로 확인할 수 있는 Git/파일·GUID·패키지 정보는 AI가 조사한다. 반복 검사 도구의 판정은 실패 case를 포함한 Unit Test로 검증하고, 사용자에게는 확인 불가능한 취득 정보/정책 결정과 실제 Unity 실행만 남긴다.

# 작업 대상

- `.gitignore` 및 하위 ignore/attributes, Git HEAD/index/worktree와 실제 Assets/Packages/ProjectSettings/Tools/UGS 의존성 경계.
- Assets의 외부 원본·복사본/파생물·프로젝트 소유 Script/Scene/Prefab/Material/설정, 각 자산과 폴더의 `.meta`.
- `Packages/manifest.json`, `Packages/packages-lock.json`, 패키지의 버전/출처·전이 의존성 및 로컬 PackageCache의 참고 metadata.
- `Assets/TextMesh Pro`의 실제 자료/설정·폰트/Material/Shader·GUID 참조, 향후 `Assets/ThirdParty` 도입 경계.
- Unity 외 보조 의존성 선언: `UGS/Modules/FlowStateVerification/FlowStateVerification.csproj`, `UGS/LogSink/package.json` 등. 서버 변경/배포는 하지 않고 Unity 원본 제외 정책과 구분한다.
- 조사/분류/미확인 목록과 Phase 2·3에 넘길 원본 제외·재도입/참조/검증 계약.

# 작업 전 상태

1. Roadmap 9/Prototype 9 Phase 1~4는 2026-11-18 완료했다. 최종 Windows-production Candidate02는 사용자 15:55 KST 빌드와 Player 6항목 정상 보고에 연결된다. QA 후보/미검증 원격·로그와 차기 실제 Module 복구 의무를 유지한다.
2. Roadmap 10은 계획 확정 상태이며 세 Phase의 실제 실행은 미착수다. Phase 1은 경계/자료 조사·판정, Phase 2는 추적 제외/복원 절차 적용, Phase 3은 기존 캐시에 의존하지 않는 독립 복원 검증이다.
3. 기존 미커밋 문서 변경을 보존한다. 커밋 유무를 착수 조건으로 만들지 않고 실제 입력 목록/해시로 기준선을 식별한다.

## 작업 방식과 범위

- Phase 1에서 `.gitignore`/index·외부 원본/폴더/meta/Scene/Prefab·패키지 버전을 변경하거나 제거하지 않는다. 현재 추적 해제/폴더 이동·재임포트/독립 checkout 복원 실행은 후속 Phase다.
- AI는 Unity Compile/Build/Test Runner·Player/Module 빌드/서비스를 실행하지 않는다. Scene/Prefab은 읽기 전용이며 필요할 때만 정확한 사용자 적용표를 작성한다.
- Library/PackageCache는 현재 설치 상태의 참고 자료다. 캐시 존재/현재 Scene 참조 해석 성공을 새 checkout 복원 성공으로 바꾸지 않는다. 캐시/실제 저장을 지워 복원 조건을 만들지 않는다.
- 비밀값·계정/구매 영수증·라이선스 키·유료 원본 본문을 문서/Test fixture에 넣지 않는다. 경로·제품/버전·공식 취득 위치·사용 조건 근거/확인 상태 등 필요한 metadata만 기록한다. 파일에 라이선스 문구가 있다는 사실과 해당 사용자의 취득/사용 가능 여부를 구분한다.
- 외부 원본/파생물과 프로젝트 소유 설정의 책임을 나눈다. 파일명이나 디렉터리만으로 자동 비추적 판정하지 않고 혼합/불명확 자료는 보존·미확인으로 표시한다.
- Unit Test를 위해 새 Runtime System·외부 package를 추가하지 않는다. UI 생성/누락 참조 보충이나 자동 guard 우회로 검사를 통과시키지 않는다. Phase 1 관련 변경/실패가 없으면 Roadmap 9의 Unity/Player 검증을 무조건 반복하지 않는다.

# 조사 내용

아래는 **문서 작성 시점의 제한된 읽기 전용 확인**이다. Step 1~10의 실제 전수 조사/분류 결과가 아니며 실행 시 최신 상태와 대조한다.

| 확인한 자료 | 확인한 사실 | 후속 조사 |
| --- | --- | --- |
| 루트 `.gitignore` | Assets/ThirdParty와 TextMesh Pro 및 각각의 루트 meta에 대한 ignore 규칙이 있음 | 실제 경로·index/추적·override·루트/자식 meta 경계를 별도 대조 |
| 현재 Assets 목록/Git 목록 | `Assets/ThirdParty` 디렉터리는 현재 없고 `Assets/TextMesh Pro`는 존재. 두 경로/루트 meta의 추적 파일 목록은 비어 있음 | 다른 이름의 외부 자료/파생물·무시된 파일 전체를 조사한 결과는 아님 |
| UPM manifest/lock | 두 파일은 추적 중. lock의 현재 의존성 61개는 registry 20개/builtin 41개 | 직접/전이·요청/해결 버전·취득/사용 조건과 각 패키지 역할 구분 |
| 현재 uGUI cache metadata | com.unity.ugui 2.0.0 및 TMP Essential Resources/Examples & Extras unitypackage가 로컬에 있음 | 캐시 경로 해시 suffix를 고정 경로로 사용하지 말고 실행 시 metadata로 찾음. 로컬 TMP 자료와 배포 원본의 동일성은 미확인 |
| TMP Settings/안내 파일 | `Resources/TMP Settings.asset`에 기본 폰트 GUID/경로와 설정이 있고 LiberationSans OFL/EmojiOne Attribution 파일도 있음 | 설정 변경/폰트·Material 파생·재도입 때 필요한 GUID/Import 값과 출처를 조사. 현 자료만으로 소유권/재배포 허용 확정 금지 |
| 보조 의존성 | Cloud Code csproj는 net9.0/4개 PackageReference·EmbeddedResource 선언, LogSink package.json은 scripts를 선언 | 선언 보존/복원 경계를 조사하며 restore/npm install·Module 빌드/배포/서버 Test는 자동 실행하지 않음 |
| 기존 검사 자산 | `UGS/Tests/client-unity-preflight.cjs`에 Scene/GUID/asmdef 소스 검사, `Tools/Tests`에 리뷰/Unit Test 도구와 과거 자산 감사 Task가 있음 | 현재 역할/부작용을 먼저 읽고 재사용. 과거 suite 개수/줄 번호를 이번 인벤토리의 고정 정답으로 복제하지 않음 |

# 작업 내용

## 실행 순서

| Step | 실제 수행 작업 | 담당 |
| --- | --- | --- |
| 1 | Roadmap 9 인계·현재 작업 트리/보존 기준선과 조사 범위 확정 | AI |
| 2 | 실제 파일/추적·index/ignore 상태의 인벤토리 작성 | AI |
| 3 | 패키지·보조 의존성의 버전/출처/재취득 정보 연결 | AI |
| 4 | GUID/meta·직렬화/이름 기반/동적 참조와 Import 설정 영향 조사 | AI |
| 5 | 취득/사용 조건·원본/변경/파생 근거와 미확인 항목 정리 | AI |
| 6 | 원본/프로젝트 소유/혼합 자료 및 meta·선언 보존 경계 초안 확정 | AI |
| 7 | 최소 정적 검사 도구와 Unit Test 보완·오프라인 실행 | AI |
| 8 | 정적으로 해결되지 않는 필수 정보/정책만 사용자 확인 후 반영 | AI + 필요한 사용자 확인 |
| 9 | 참조/도구 결과 재대조와 필요한 조건부 Unity 검증 연결 | AI + 필요한 사용자 실행 |
| 10 | Phase 1 완료 조건 판정·Phase 2/3 실행 인계 | AI |

## Step 1. 기준선과 범위를 확정한다

### AI 작업

1-1. Roadmap 9 Phase 4 Task 10-1~10-4와 Roadmap 10 Phase 1의 세 완료 조건을 대조한다. QA 후보의 실제 파일/소스·검증 범위와 원격/복구 의무를 인계한다.

1-2. HEAD·`git status --short`·변경/staged/비무시 새 파일을 조사한다. 기존 변경을 보존하고 `.gitignore`, manifest/lock, ProjectVersion, 주요 Scene/설정·참조 metadata의 목록/해시를 기록한다. 실제 사용자 저장/Secret은 제외한다.

1-3. 조사 경로·재귀/제외 규칙과 결과 출력 위치를 정한다. Git 추적 목록과 무시된 로컬 파일 목록을 분리하며 Library/Build/node_modules/obj 등을 외부 원본 자체와 혼동하지 않는다. symlink/junction은 따라가지 않고 위치/미조사 사유를 기록한다.

### 완료 조건

- [x] 기준선·보존 대상·읽기/출력 범위와 Phase 1/후속 Phase 경계를 기록했다.

### Step 1 수행 결과 — 2026-11-18

#### 1-1. 선행 인계와 완료 조건 대조 — 완료

Roadmap 9 Phase 4 Task 10-1~10-4를 확인했다.

사용자 Windows-production / Unity 6000.3.5f2 / 2026-11-18 15:55 KST 빌드, Compile·EditMode 1031/1031·PlayMode 316/316·Player 필수 6항목 정상 보고와 기존 정적/오프라인 16스크립트 exit 0을 선행 근거로 인계한다. 이번 재실행 결과가 아니다. Candidate02 실행 파일은 실제 존재하며 크기·SHA-256을 기준선 JSON에 기록했다. 선행 Task 입력 6개와 현재 파일 해시도 모두 일치한다.

최신 실제 원격 제출/조회·이전/복구·원격 artifact 동일성/가용성·모든 장애/경합·컨테이너 간 원자성은 미검증이고 Unity XML/Console·별도 Player 로그/이미지 원문 미제공 한계를 유지한다. 다음 실제 Module 업데이트 전 source/artifact/hash·환경/게시 reference·새 QA 기준 보존과 별도 승인 범위의 실제 복구 시험 의무를 인계한다. QA 후보는 일반 공개/Steam 승인이 아니다.

| Phase 1 완료 조건 | 이후 확보할 증거 | Step 1 판정 |
| --- | --- | --- |
| 내용/출처 기반 제외 범위 | Step 2/4/5/6/8 목록·참조·출처·경계 결정 | 조사 범위 확정, 분류 미확정 |
| Package 선언/lock 등 복원 정보 관리 범위 | Step 3/6 직접/전이·출처·버전·보조 선언/보존 계약 | 기준 입력 확보, 보존 정책 미확정 |
| 복사본/파생물·민감 정보 기록 경계 | Step 4~8 원본 비교·설정/meta·분류/검사 | 민감/원본 payload 기록 제외, 개별 판정 미착수 |

현재 자료와 앞으로 추가될 Third Party 자료 모두에 적용할 도입·Git 보존/제외·변경/파생물·재취득/참조 검증 기준을 후속 Step/Phase에서 정리한다. 현재 ignore 상태만 확인하고 끝내지 않으며 Roadmap 11~14 도입 전 인계를 유지한다.

#### 1-2. 작업 트리와 보존 기준선 확보 — 완료

- 착수 HEAD: `8e2c6e9588161bbfd7b57cca18343c378a018337`. Git 2.55.0.windows.2 / Node v24.21.0.
- 착수 `git status --short`·unstaged/staged name-status·비무시 untracked 모두 빈 결과다. 추적 경로는 919개이며 기존 미커밋 변경은 없다. 문서 작성 시점의 미커밋 상태를 현재 사실로 승계하지 않았다.
- [기준선 metadata](20261118_02_Phase1Step1Baseline.json)에 입력 **66개**의 경로·크기·SHA-256·index 추적 여부, HEAD·추적 경로 목록의 NUL 스트림 해시·index 파일 해시·선행 6개 입력 비교 결과를 기록했다. 해시는 원문 바이트 기준이며 Git blob 해시와 구분한다.
- 입력은 루트 ignore·UPM manifest/lock·ProjectVersion 포함 ProjectSettings·주요 Scene/Settings와 meta·asmdef/meta·TMP Settings와 meta/루트 meta·루트 npm manifest/lock·UGS csproj/LogSink manifest다. TMP 무시 자료도 포함했지만 전수 인벤토리는 아니다.
- 실제 사용자 저장·Secret/계정/영수증·로컬 production 설정 원문과 유료 원본 payload는 수집/기록하지 않는다. 후보는 경로·크기·해시만 읽고 실행하지 않았다. 기존 파일/index/ignore·원본/meta·패키지·후보를 보존한다.

#### 1-3. 읽기·재귀·제외·출력 범위 확정 — 완료

| 범위 | 조사/제외 계약 |
| --- | --- |
| Git | HEAD/index/worktree·추적/staged/비무시 신규 목록은 저장소 기준으로 읽는다. 실제 파일/무시 목록과 별도 대조하며 대소문자·공백/한글·NUL 구분을 보존한다. global ignore는 필요한 규칙 위치만 확인하고 변경하지 않는다. |
| 실제 자료 | Assets/Packages/ProjectSettings/Tools/UGS 아래를 제한적으로 재귀 조사한다. Assets 전체 후보와 TextMesh Pro·각 폴더/자산 meta를 포함한다. ignore/attributes는 루트와 범위 내 하위 파일을 포함한다. ThirdParty가 없어도 향후 도입 기준을 정리한다. |
| 보조 선언 | 루트 package.json/package-lock.json도 포함한다. 로컬 Library/PackageCache tgz 의존성 선언을 읽기 확인했으며 역할/복원 가능성은 Step 3에서 판정한다. install/restore·서버 실행/배포는 하지 않는다. |
| 캐시 예외 | Library 전체는 조사하지 않는다. Step 3/4에서 필요한 Library/PackageCache의 package.json·라이선스/참조 meta·배포 archive만 metadata로 찾은 경로에 한해 읽는다. 경로 suffix를 복원 계약으로 고정하지 않고 캐시 존재를 독립 복원 성공으로 보지 않는다. |
| 제외 | .git 내부는 기준선용 index/읽기 Git 명령 외 조사하지 않는다. .vs/.vscode/.agents/.codex/.aws, Library/Temp/Build/Builds/Logs/UserSettings/Ignore·node_modules/obj/bin·사용자 저장/Secret·.env 등은 일반 재귀/본문 조사에서 제외한다. Build는 이번 후보 존재/해시 확인만 예외다. 제외 디렉터리 안의 추적 경로가 발견되면 metadata와 미조사 사유를 별도로 기록한다. |
| 링크/실패 | symlink/junction은 따라가지 않고 위치/미조사 사유를 기록한다. 위 5개 루트의 디렉터리 경계 검사에서 링크 0개·읽기 오류 0개, .vs/bin/obj 제외 3경로를 확인해 JSON에 기록했다. Library/PackageCache는 이번 검사 밖이며 필요 시 동일 방식으로 확인한다. 접근 오류/부분 열거를 빈 목록 성공으로 처리하지 않는다. |
| 출력 | 이 Task를 조사·판정 이력의 단일 기록으로 사용한다. 기준선 JSON과 필요한 후속 metadata만 같은 Prototype_10 폴더에 기록한다. 영구 register는 Step 6, Tools/DependencyAudit 제안 도구는 Step 7에서 기존 책임/도구 검토 후 결정한다. 원본/민감 payload는 출력하지 않는다. |

초기 경로 탐색에서 `Assets/Resources`는 존재하지 않아 rg가 오류를 보고했다. 빈 디렉터리나 참조 없음으로 판정하지 않았다. TMP Resources는 `Assets/TextMesh Pro/Resources`이며 실제 TMP Settings를 기준 입력으로 확보했다. 경계 검사는 파일 수/내용·출처·GUID/라이선스 전수 조사가 아니며 Step 2~5를 대신하지 않는다.

Phase 1은 조사/분류·검사 준비, Phase 2는 실제 추적 제외/도입·복원 절차 적용, Phase 3은 별도 환경의 독립 복원 검증이다. 이번 Step 1은 문서/metadata만 변경하므로 Unity Compile/Build/Runner·Player·Scene/Prefab 적용이 필요하지 않다. **사용자 수동 작업 없음.** Step 1 완료 조건을 충족했으며 다음은 Step 2다.

## Step 2. 추적/실제 파일/ignore 인벤토리를 만든다

### AI 작업

2-1. `git ls-files -z`, `git diff --cached --name-status`, `git ls-files --others --exclude-standard`와 제한된 실제 파일 열거를 대조한다. Assets/ThirdParty·TextMesh Pro뿐 아니라 Plugins/샘플/폰트/Shader·이름이 바뀐 외부 자료 후보와 폴더 meta를 찾는다.

2-2. 무시된 대상은 `rg --files --hidden --no-ignore` 등을 **정한 경로에서만** 사용한다. `.gitignore`의 문구뿐 아니라 `git check-ignore -v --no-index -- <대상>`으로 규칙의 적용 위치를 확인한다. 출력/버전/exit code 의미를 구분하며 추적 파일은 ignore에 매칭돼도 계속 추적된다는 점을 별도 표기한다.

2-3. 항목마다 `경로/자료 종류/실제 존재/HEAD 추적/index 상태/ignore 규칙/잠정 출처/관련 meta/조사 근거/미확인`을 기록한다. 폴더 없음·외부 원본 없음·추적 없음·규칙 매칭을 서로 다른 결과로 남긴다. 대소문자/공백/한글 경로를 보존한다.

### 완료 조건

- [x] 정한 범위의 실제 파일·추적/index·무시 자료가 연결되고 누락/범위 밖 항목이 명시됐다.

### Step 2 수행 결과 — 2026-11-18

#### 2-1. 실제 파일과 HEAD/index/worktree 목록 대조 — 완료

[경로별 인벤토리 metadata](20261118_02_Phase1Step2Inventory.json)에 파일·폴더·부재 확인 경로를 기록했다. Step 1에서 정한 5개 루트를 lstat/readdir로 열거하고 symlink/junction은 따라가지 않았다. Git은 `ls-tree -r --name-only -z HEAD`, `ls-files -z`, staged/unstaged `diff --name-status -z`, `ls-files --others --exclude-standard -z`로 읽었다. NUL 구분과 원래 대소문자·공백/한글 경로를 보존했다.

착수 HEAD는 Step 1과 동일하며 추적 경로 919개·staged/unstaged 생산 자산 변경 0개다. 기존 변경 문서 4개와 비무시 신규 Step 1 기준선 JSON 1개를 보존했다. 조사 범위 밖 추적 경로 179개는 AI 문서 등을 포함하며 이 인벤토리의 실제 파일 조사에서 제외한다. 이번 출력 JSON은 착수 목록 수집 후 생성했으므로 착수 목록에는 없다.

| 실제 조사 범위 | 파일 | index 추적 | ignore 매칭 | 비추적·비무시 |
| --- | ---: | ---: | ---: | ---: |
| Assets | 634 | 550 | 84 | 0 |
| Packages | 2 | 2 | 0 | 0 |
| ProjectSettings | 28 | 28 | 0 | 0 |
| Tools | 5 | 5 | 0 | 0 |
| UGS | 153 | 152 | 1 | 0 |
| 5개 루트 합계 | 822 | 737 | 85 | 0 |

루트 `.gitignore`·`package.json`·`package-lock.json` 3개를 더해 실제 파일 825개와 실제 디렉터리 49개를 기록했다. 루트 `.gitattributes` 부재도 별도 행이다. 제외 3경로는 `UGS/Modules/FlowStateVerification/.vs`, `bin`, `obj`이며 그 안의 추적 경로는 0개다. 링크/열거 오류 0개·누락된 추적 경로 0개다. Library/PackageCache·Build·node_modules 등 Step 1 제외 범위는 계속 미조사이며 저장/Secret 본문을 읽지 않았다.

#### 2-2. ignore 적용 위치와 예외 확인 — 완료

전체 행을 `git check-ignore --no-index -v -z --non-matching --stdin`으로 확인했다. 이는 Step 2-2의 경로별 `check-ignore -v --no-index -- <대상>`과 같은 판정을 NUL 배치 입력으로 수집한 것이다. Git 2.55.0.windows.2에서 exit 0은 매칭 존재, 1은 매칭 없음, 나머지/프로세스 오류는 조사 실패로 구분했다. 이번 실제 배치 exit는 0이다. 각 행에 규칙 파일·줄·패턴·부정 예외·실제 무시 여부를 남겼으며 `!` 예외를 무시 판정으로 처리하지 않았다.

| 대상 | 실제 상태 | HEAD/index | 적용 규칙·판정 |
| --- | --- | --- | --- |
| `Assets/ThirdParty/` | 폴더 없음 | 추적 없음 | `.gitignore:110` 폴더 규칙 매칭. 원본 존재를 뜻하지 않음 |
| `Assets/ThirdParty.meta` | 파일 없음 | 추적 없음 | `.gitignore:111` 루트 meta 규칙 매칭 |
| `Assets/TextMesh Pro/` | 폴더/하위 파일 81개 존재 | 추적 없음 | `.gitignore:113` 폴더 규칙 매칭 |
| `Assets/TextMesh Pro.meta` | 파일 존재 | 추적 없음 | `.gitignore:114` 루트 meta 규칙 매칭 |
| Cloud Code `FlowStateVerification.csproj` | 파일 존재 | 추적 중 | 하위 `.gitignore:2`의 `!*.csproj` 예외로 비무시 |
| 로컬 production 설정·meta / LogSink `.env` | 파일 3개 존재 | 추적 없음 | 루트 ignore 매칭. 경로/stat만 조사하고 본문 제외 |

조사 범위에서 발견한 ignore/attributes 파일은 루트 `.gitignore`와 UGS Module 하위 `.gitignore` 두 개이며 `.gitattributes`는 없었다. 추적 중이면서 ignore 매칭인 실제 사례는 0개지만, 향후 그런 경우 ignore만으로 추적이 해제되지 않는다는 관리 기준을 유지한다. 중첩 부정 예외는 위 csproj에서 실제 확인했고, rename/대소문자 충돌 등 실패 입력 검증은 Step 7에 남긴다.

#### 2-3. 항목별 기록·후속 조사·향후 관리 기준 연결 — 완료

JSON 각 행에 `path/kind/exists/headTracked/indexTracked/indexChange/ignore/provisionalOrigin/meta/evidence/unknown`을 기록했다. 폴더 없음·미도입이라는 사용자 전제·추적 없음·규칙 매칭을 분리했다. 폴더/자산과 meta를 실제 경로로 연결했으며 Assets에서 missing/orphan meta 경로는 발견되지 않았다. GUID/Import 내용과 참조 해석의 검증은 Step 4다.

| 자료군 | 이번 확인 | 출처/분류의 미확인 및 다음 작업 |
| --- | --- | --- |
| 향후 Third Party | 사용자 전제상 도입 애셋 없음, ThirdParty 폴더/meta 부재 | 가상 제품/라이선스를 채우지 않음. Step 6 등록 형식·Phase 2 도입 검사에 연결 |
| TMP 기술적 기준선 | 폰트·Material/폰트 atlas asset·Shader/include/graph·sprite PNG/JSON·설정·안내 파일과 meta 존재 | 81개 하위 파일+루트 meta는 비추적/무시. 라이선스/출처·원본 동일성·설정 변경·실사용은 Step 3~5에서 확인 |
| 프로젝트 트리의 연결 자료 | Material/PhysicsMaterial·Pattern Prefab 4개·SampleScene·InputActions·렌더링/Build Settings·Cloud Code 연결/asmdef 존재 | 경로/종류만으로 프로젝트 소유나 순수 원본을 확정하지 않음. Step 4~6 참조/출처/보존 경계로 인계 |
| Plugins/샘플·이름 변경 후보 | Assets 전체를 열거했고 Plugins 디렉터리·TMP 밖 폰트/Shader/이미지 파일은 발견되지 않음 | Settings/SampleScene 등 기본 제공·파생 가능 자료는 보존/판정 대기. 이름이 바뀐 외부 자료가 없다는 증명은 아님 |
| 패키지/보조 선언 | UPM manifest/lock·루트 npm manifest/lock·UGS csproj/LogSink manifest 추적 중 | 버전/전이/source·로컬 캐시 의존성과 복원 정보 역할은 Step 3 |

별도 제한 텍스트 검색은 TMP 밖 Assets/Tools에서 copyright/license 표식을 찾지 못해 rg exit 1이었다. 이는 검색어 매칭 없음이며 출처/권리나 외부 복사본 부재를 증명하지 않는다. 인벤토리의 잠정 출처는 조사 대상 구분이며 최종 소유/제외 승인이 아니다.

독립 대조로 `rg --files --hidden --no-ignore`에 같은 제외 glob을 적용한 파일 822개가 실제 열거와 정확히 일치했다. `git ls-files --others --ignored --exclude-standard -z -- <5개 루트>`에서 선언한 제외 경로를 뺀 85개도 경로별 ignore 판정과 정확히 일치했다. 이 대조는 이번 인벤토리의 정적 검증이며 Step 7 반복 도구/Unit Test 완료를 뜻하지 않는다.

누락/범위 밖·잠정 판정을 명시해 Step 2 완료 조건을 충족했다. **사용자 수동 작업 없음.** 생산 자산·ignore/index·Scene/Prefab을 변경하지 않았으며 Unity/Build/Test Runner·서버를 실행하지 않았다. 다음은 Step 3 의존성·출처·재취득 정보 연결이다.

## Step 3. 의존성 선언과 재취득 경로를 연결한다

### AI 작업

3-1. manifest의 직접 의존성·scoped registry/local/git 등 출처 선언과 lock의 해결 버전/전이 관계/source를 실제 JSON으로 읽는다. 현재는 registry/builtin이지만 다른 유형이 발견되면 별도 판정한다. 형식 오류/누락/순환·해결 불일치를 성공으로 숨기지 않는다.

3-2. package.json/캐시 metadata·ProjectVersion을 대조해 `제품/패키지 ID/요청 버전/해결 버전/source/공식 취득 위치/필요 Editor/재도입 방식/근거/확인 상태`를 만든다. Library cache의 이름/존재는 참고로만 쓰고 외부 URL을 임의로 만들어 쓰지 않는다.

3-3. TMP Essential Resources 등 패키지 밖 Assets로 가져온 자료와 UPM package 원본을 연결한다. 원본 archive가 있다는 것만으로 프로젝트 설정/파생물·GUID 재현이 보장된다고 하지 않는다.

3-4. UGS csproj/EmbeddedResource·LogSink package.json 등 보조 의존성 선언은 별도 행으로 기록한다. Unity 외부 원본 제외 때문에 프로젝트 소유 빌드 선언/서버 소스를 일괄 제거하지 않는다. 네트워크 restore/install·CLI 인증/게시를 수행하지 않는다.

### 완료 조건

- [x] 직접/전이 의존성·복사 도입 자료·보조 선언의 역할/버전/취득 근거와 보존 필요성이 정리됐다.

### Step 3 수행 결과 — 2026-11-18

#### 3-1. UPM 직접/전이 graph 대조 — 완료

[의존성 metadata](20261118_02_Phase1Step3Dependencies.json)에 UPM 61행과 요청→해결 버전 관계 91개를 기록했다. manifest 직접 의존성은 43개, 전이 전용은 18개이며 lock source는 registry 20개/builtin 41개다. manifest에 scoped registry·local/git·사용자 registry·별도 resolutionStrategy/lock 비활성 선언은 없다. 직접 요청과 lock 버전은 모두 일치하며 누락된 직접/전이 대상·순환은 0개다. JSON 파싱/읽기 오류는 성공한 빈 목록으로 처리하지 않았다.

전이 요청과 최종 해결 버전이 다른 관계는 28개다. 이를 숨기거나 정확한 문자열 불일치만으로 오류라고 하지 않았다. 예를 들어 Cinemachine은 Splines 2.0.0을 요청하지만 lock은 2.8.2이고, Authentication은 uGUI 1.0.0을 요청하지만 직접 선언/lock은 2.0.0이다. 이 요청·해결 값을 각각 남겼다. Unity의 [의존성 설명](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/dependencies-lp/upm-dependencies)과 [해결 우선순위](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/dependencies-lp/upm-conflicts)는 전체 graph와 source/직접 의존성을 고려한 해결을 설명한다. 이번 정적 대조를 Unity resolver 재실행이나 Runtime 호환성 증명으로 해석하지 않는다.

#### 3-2. 제품/버전·출처·Editor·재취득 정보 연결 — 완료

Library/PackageCache 직하의 실제 package.json을 ID/버전으로 찾아 lock 61개와 연결했다. 패키지 버전과 dependencies는 61개 모두 일치했다. 경로 suffix는 이번 증거 위치일 뿐 복원할 고정 경로가 아니다. package.json 해시·공개 documentationUrl(있는 경우)·명시된 최소 Editor·직접/전이 여부·source·공식 registry 주소·보존 파일·재취득 방법/확인 상태를 각 행에 남겼다. 내부 개발용 repository 주소를 사용자의 취득 위치로 기록하지 않았다. builtin에 registry URL을 임의 생성하지 않았다.

| 제품/역할 | 요청=해결 버전 | lock source | 취득/보존 계약 |
| --- | --- | --- | --- |
| Cinemachine / 카메라 기반 | 3.1.7 | registry | lock의 `https://packages.unity.com`, manifest/lock 유지 |
| Rider / Visual Studio 연동 | 3.0.38 / 2.0.27 | registry | 동일 registry, Editor 도구 의존성 구분 |
| Input System | 1.17.0 | registry | 동일 registry, 입력 설정/코드 보존 |
| URP / 렌더링 기반 | 17.3.0 | builtin | 해당 Editor/내장 패키지 조합, 프로젝트 렌더링 설정 보존 |
| Authentication / Cloud Code / Deployment | 3.8.0 / 2.10.4 / 1.6.2 | registry | 동일 registry, Client SDK와 서버 Module 선언 구분 |
| Test Framework / uGUI(TMP 포함) | 1.6.0 / 2.0.0 | builtin | 해당 Editor/내장 패키지 조합, Test/Scene/설정 보존 |
| Timeline | 1.8.12 | registry | 동일 registry, 프로젝트 연결 자료 보존 |
| 직접 선언된 엔진 Module 32개 | 각 1.0.0 | builtin | ID별 행은 JSON, 임의 외부 원본 제외 금지 |

현재 프로젝트 Editor는 `6000.3.5f2 (3fa8bc678cb0)`다. cache의 최소 Editor 표시는 선언 정보이며 이 정보만으로 Compile/호환 성공을 판정하지 않았다. [Unity lock 안내](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/dependencies-lp/upm-conflicts-auto)에 따라 manifest/lock·ProjectVersion을 재현 입력으로 유지한다. 실제 취득/권한·라이선스 확인은 Step 5, 신규 환경 복원은 Phase 3이며 현재 캐시 일치만으로 성공 처리하지 않는다.

#### 3-3. TMP Assets 복사 도입과 공급 패키지 연결 — 완료

uGUI 2.0.0의 실제 package metadata와 `Editor/TMP/TMP_PackageUtilities.cs`에서 `Window/TextMeshPro/Import TMP Essential Resources` 메뉴가 Package Resources의 archive를 ImportPackage로 도입하는 경로를 확인했다. Essential Resources/Examples & Extras archive 2개의 위치·크기·해시는 JSON에 남겼으며 import/추출·Scene 변경을 실행하지 않았다.

`Assets/TextMesh Pro`의 폰트/Material/Shader/sprite·TMP Settings는 UPM 패키지 밖에 복사 도입되는 기술적 기준선으로 구분했다. 공급 패키지/메뉴와 현재 Assets가 연결된다는 근거이며, 실제 과거 도입 경로·archive와 로컬 자료의 동일성·수정 없음·동일 GUID/Import 재현을 증명한 것은 아니다. Step 4~5에서 비교한다. importer 소스는 기존 TMP Settings를 저장/backup하는 동작도 포함하므로 향후 재도입은 단순 폴더 복사가 아닌 프로젝트 설정 보존/재적용 계약이 필요하다. 현재 사용자에게 import를 요청하지 않는다.

#### 3-4. 보조 의존성 선언·복원 경계와 후속 의무 — 완료

| 자료 | 확인 결과 | 보존/미확인·후속 계약 |
| --- | --- | --- |
| 루트 npm manifest/lock | 직접 33개(일반 dependencies 31개/devDependencies 2개), lock v3의 root 포함 58행. root 선언 불일치 0개 | manifest/lock 보존. root 제외 lock 57개는 registry 30개/local tgz 27개. 필수 dependency 연결 65개가 lock 안에서 해석됨. semver/Runtime 호환성·install 성공은 별도 |
| Cloud Code SDK 공급 tgz | root 직접 local tgz 27개가 모두 존재. archive package.json의 ID/버전·라이선스 표시·archive 해시를 읽기 전용으로 수집했고 버전은 npm lock과 일치 | 공급자는 Cloud Code 2.10.4의 UnityServicesNpmLocal~. 현재 Windows 경로와 cache suffix에 의존하므로 재취득 후 ID/버전·archive 해시로 다시 찾아 연결하는 절차를 Phase 2에서 마련. archive 본문을 출력/복사하지 않음 |
| Cloud Code Module csproj | net9.0; CloudCode.Core 0.0.7, CloudCode.Apis 0.0.27, Jint 4.16.4, Newtonsoft.Json 13.0.3 | NuGet PackageReference 요청 선언으로 보존. feed 설정/해결된 전이 버전은 이 선언에 없으며 restore 미실행. 공식 feed/해결 graph는 실제 복원 준비 때 확인하며 임의 URL을 취득 증거로 추가하지 않음 |
| EmbeddedResource | `../../CloudCode/*.js`의 실제 39파일과 `runtime.js`, LogicalName 선언 | 자체 서버 소스/리소스 경계다. 원본 제외 대상으로 일괄 제거하지 않고 csproj와 함께 보존. source/artifact 동일성·원격 복구를 증명하지 않음 |
| LogSink | 자체 package 1.0.0, 외부 npm dependencies/devDependencies 선언 없음, start/test scripts 있음 | package.json/index.js·Dockerfile 등 자체 소스/선언 보존. Dockerfile의 `node:22-alpine`은 digest 고정 아님을 후속 제약으로 기록. .env/Secret은 조사/기록 제외, 서비스/컨테이너/시험 실행 없음 |

향후 애셋 관리에는 `공식 공급 위치 + 요청/해결 버전 + 공급 의존성 + 복사 도입 위치/재적용 정보 + 확인 상태`를 분리해 기록한다. 현재 사용자 도입 애셋은 없으므로 가상 제품·취득/사용 권한을 채우지 않는다. root npm 캐시 경로의 이식성, NuGet feed/해결 graph, Docker image digest·원격 가용성은 후속 도입/복원 의무이며 Step 3의 선언 조사 완료를 막는 사용자 입력은 아니다. 추적/제외 경계 확정은 Step 6이다.

Step 3의 metadata 연결/보존 필요성 정리를 완료했다. **사용자 수동 작업 없음.** 네트워크 package 다운로드·npm install/ci·dotnet restore·Unity/Runner·Build·서비스/배포·Scene/Prefab 변경은 수행하지 않았다. 다음은 Step 4 참조와 meta/Import 영향 조사다.

## Step 4. 참조와 meta/Import 영향 범위를 조사한다

### AI 작업

4-1. 파일/폴더 meta의 GUID→경로 map을 만들고 프로젝트 소유 Scene/Prefab/Material/설정·asmdef·외부 자료의 GUID/fileID 참조를 대조한다. Assets/Packages/PackageCache의 검색 ignore 누락과 Unity builtin GUID를 구분한다. 중복/잘못된 GUID·missing/orphan meta는 오류/미해결로 표시한다.

4-2. TMP Settings 기본 폰트/Material·fallback/sprite/line-breaking 설정과 Shader 참조를 실제 대상까지 연결한다. 현재 cache로 해결되는 GUID와 Git checkout만으로 제공되는 GUID를 나누고 원본/폴더 meta의 제외 시 영향·동일 GUID 재취득 근거를 기록한다.

4-3. Resources.Load/Shader.Find·경로/이름 문자열, reflection/asmdef/DLL·런타임 생성/직접 코드 의존성을 조사한다. 텍스트 GUID가 없다는 이유로 미사용이라고 하지 않는다. binary/동적 참조로 정적 판정이 제한되면 대상/한계와 Step 9에서 필요한 자동 확인을 지정한다.

4-4. `.meta`의 importer/platform 설정과 프로젝트 변경 여부를 비교 가능한 원본 metadata가 있을 때만 대조한다. 원본 비교가 불가능한 파일은 변경 없음으로 단정하지 않는다. Scene/Prefab을 바꾸지 않고 필요한 후속 적용/복원 검증 항목만 기록한다.

### 완료 조건

- [x] 제외 후보→참조 사용처·meta/Import/GUID 보존/재현 요구와 정적 조사 한계가 연결됐다.

### Step 4 수행 결과 — 2026-11-18

#### 4-1. GUID/meta·직렬화 참조와 제공 경로 조사 — 완료

[참조/Import metadata](20261118_02_Phase1Step4References.json)에 Assets/Packages와 Step 3에서 찾은 설치 패키지 61개의 GUID map·실제 참조·원본 비교 결과를 기록했다. ignore에 의존하지 않는 lstat/readdir로 열거했으며 symlink/junction은 따라가지 않았다. Library 전체/사용자 저장/Secret을 조사하거나 원본을 수정하지 않았다.

GUID/meta 11,816개(Assets 331개)를 수집해 중복/형식 오류 0개를 확인했다. 결과에는 Assets 전체와 직렬화/Importer 참조에 필요한 package GUID 554행을 보존하고, 나머지 무관한 package map은 개수/전체 metadata digest로 남겨 불필요한 캐시 목록 복제를 줄였다. Assets/ProjectSettings에서 시작해 실제 GUID로 연결되는 package 직렬화 자료까지 56파일·GUID 참조 1,270회를 대조했다. Unity builtin/null GUID와 Git 제공·무시된 로컬 자료·package cache 필요를 분리했다. GUID가 있는 YAML 대상의 fileID 354회는 실제 subasset anchor에 존재했지만 Unity 타입/LoadAsset 해석 성공을 뜻하지 않는다. 자산 meta importer 참조 5개는 InputActionImporter/ShaderGraphImporter의 실제 package script로 연결됐다.

| 조사 항목 | 결과·보존/미해결 계약 |
| --- | --- |
| Assets meta | missing/orphan 없음. 파일/폴더 meta 모두 경로와 GUID를 유지해야 하며 폴더명만 보고 버리지 않음 |
| package meta | Test Framework `Samples~/15_TestCases_15.meta`의 대상 폴더 부재 1개. 현재 조사한 프로젝트 참조 없음. cache는 보존하고 Step 9에서 범위/영향 판정 |
| DefaultVolumeProfile | 프로젝트 설정과 URP package의 기본 profile 양쪽에 같은 미해결 script GUID 4종(총 8회)이 있음. 이름은 OutlineVolumeComponent/TestAnimationCurveVolumeComponent/OasisFogVolumeComponent/TestVolume. 실제 Missing Script 영향은 미판정 |
| PC_Renderer | probeVolumeResources 내부 GUID 7종(7회) 미해결. 현재 URP `Runtime/Deprecated.cs`에서 해당 필드/자원을 Obsolete·이동 대상으로 명시한 근거는 있음. Runtime 미사용/정상 migration을 증명한 것은 아님 |

따라서 미해결 GUID는 **11종/15회(프로젝트 11회/package 4회)**이며 성공으로 숨기지 않는다. 기존 기준 파일에 있던 자료이고 이번 변경으로 생긴 누락은 아니다. 참조를 임의 삭제/대체하거나 패키지 원본/Scene을 고쳐 통과시키지 않는다. Step 9에서 선행 검증 근거와 영향 범위를 대조하고 필수 판단 여부를 결정한다. 필수 미해결을 남긴 채 Phase 1을 완료하지 않는다.

#### 4-2. TMP Settings·폰트/Material·Shader 연결 — 완료

TMP Settings의 script는 uGUI package의 TMP_Settings.cs로, 기본 폰트는 LiberationSans SDF.asset으로 해석된다. 기본 sprite는 EmojiOne.asset, style은 Default Style Sheet.asset, 줄바꿈은 Leading/Following Characters.txt로 연결된다. Settings의 fallback 목록은 비어 있지만 기본 폰트의 fallback table은 LiberationSans SDF - Fallback.asset을 참조한다. 폰트 안의 Material/atlas subasset fileID와 Shader·fallback의 source TTF까지 metadata에 연결했다.

SampleScene은 기본 폰트 GUID를 **244회** 참조한다(객체 수가 아닌 font/material 등의 GUID 발생 횟수). font asset 내 Material은 TMP_SDF-Mobile.shader로 연결된다. 이 TMP 자료들은 현재 Git 비추적/무시 상태이며 checkout만으로 제공되지 않는다. Scene/C#은 보존하고 uGUI 2.0.0과 같은 GUID의 Assets 자료를 재도입/재적용하는 계약이 필요하다. 캐시/로컬 GUID 해석을 새 checkout 복원 PASS로 승계하지 않는다.

#### 4-3. 이름/동적·코드/assembly 의존성과 한계 — 완료

프로젝트 Runtime/Editor 소스에서 직접 Resources.Load/Shader.Find 호출은 발견되지 않았다. Runtime UI 3파일(AccountTransferView/SettingsUIController/UIManagementSystem)은 TMPro를 사용하며, Editor 검증 창에는 TestRunner API의 Type.GetType 경로가 있다. Test 코드의 reflection/Editor 호출은 생산 애셋 로딩과 구분해 위치만 기록했다. 프로젝트 asmdef 4개의 명시적 참조는 현재 프로젝트/package asmdef 이름으로 모두 연결됐으며 이 정적 이름 연결은 Compile 증명이 아니다.

uGUI/TMP 소스에서는 `Resources.Load("TMP Settings")`, Fonts & Materials·Sprite Assets 등 경로 기반 도입과 Shader.Find 의존성을 확인했다. `TextMeshPro/Mobile/Distance Field`, `TextMeshPro/Mobile/Bitmap`, `TextMeshPro/Sprite`는 실제 로컬 shader 선언 이름과 일치한다. Resources 상대 경로·이름과 shader 이름도 복원 계약에 포함하며, 텍스트 GUID가 없다는 이유로 미사용으로 분류하지 않는다.

InfinitePatternSlot.InitializeFromPrefabs의 Instantiate 경로는 선언된 Pattern Prefab을 Runtime에 생성한다. 따라서 생성된다는 이유로 원본 Prefab과 Scene 연결을 인벤토리/복원 범위에서 빼지 않는다. TMP ShaderGraph 4개의 중첩 JSON texture 참조도 별도 확인했다. 24회 중 4회는 builtin, 4회는 기존 폰트, 나머지 **1종/16회 texture GUID(dda5bcb0d1e9515498f6e4e038bbefe6)**는 전체 meta map에서 미해결이다. 4개 graph는 공급 archive와 byte 일치하고 현재 조사한 Scene/YAML에서 graph로 향하는 GUID 참조는 없지만, 동적 variant 미사용이나 importer 정상 해석을 단정하지 않는다. URP 11종과 구분해 Step 9/Phase 3의 필요성 판정 대상으로 남긴다.

Assets의 binary 자료는 TMP의 TTF/PNG 2개이고 DLL은 발견되지 않았다. 조사한 직렬화 56파일은 텍스트로 읽혔으나 TTF/이미지/Shader의 실제 Unity importer·타입·렌더링 해석은 정적 해시/이름 조사로 증명할 수 없다. 전체 package Runtime의 임의 동적 로딩까지 전수 증명한 것은 아니다. Step 9에서 필요할 때만 최소 읽기 전용 AssetDatabase/EditMode 검증을 준비하고, 독립 복원/최종 표시 확인은 Phase 3에 연결한다.

#### 4-4. 원본 대비 meta/Import·변경 보존 요구 — 완료

설치된 uGUI 2.0.0의 TMP Essential Resources archive를 메모리에서만 읽어 pathname/asset/meta 해시를 비교했다. archive 본문/atlas payload를 문서나 fixture에 복사하거나 디스크 추출·Unity import를 하지 않았다. 로컬 TMP 파일 82개 모두 대응 원본이 있었고 **80개 byte 일치/2개 내용 차이**였다. 자산/폴더 meta **45개는 모두 byte 일치하며 GUID도 같다**. 따라서 현재 로컬 배포물과의 동일 GUID/meta 재취득 근거는 있지만 공식 원격 배포물의 진위·사용 권한이나 향후 importer 결과까지 보장한 것은 아니다.

차이가 있는 파일은 `LiberationSans SDF.asset`과 `LiberationSans SDF - Fallback.asset`이다. 개행만의 차이가 아니며 Material/폰트 feature·직렬화/atlas 관련 필드 차이를 metadata로 남겼다. 작성자·의도적 수정인지 Unity 변환인지 원인은 단정하지 않는다. 동일 원본으로 보고 폐기하지 않고 보존/판정 대기로 두며 Step 5~6에서 유지·재현 정보를 정한다. TMP Settings·standalone Material·TTF/PNG·shader 및 각 meta의 일치는 해당 로컬 archive 비교 범위에 한한다. 비교 원본이 없는 프로젝트 Scene/Material/설정은 변경 없음/순수 자체 소유로 확정하지 않는다.

향후 애셋 기록에는 GUID/meta/Import 설정과 직접 참조뿐 아니라 이름 기반 경로·subasset·원본 대비 변경·재적용 요구를 포함한다. 이번 Step은 **조사와 보존/검증 요구 연결을 완료**한 것이며 전체 참조 정상 판정은 아니다. 미해결 URP 11종·TMP ShaderGraph texture 1종과 package sample orphan은 Step 9의 명시적 판정 의무로 남긴다. **현재 사용자 수동 작업 없음.** AI는 Unity Compile/Build/Runner·Scene/Prefab 변경을 수행하지 않았다. 다음은 Step 5 출처/사용 조건·원본/변경 근거 정리다.

## Step 5. 원본/변경/파생·취득/사용 조건의 근거를 정리한다

### AI 작업

5-1. 제품/패키지 metadata, 기존 도입 기록, LICENSE/NOTICE/Attribution과 공식 배포 정보를 확인한다. 원본 본문을 새로 복사하지 않고 문서 경로/필요한 조건 요약·근거 위치·확인 상태를 기록한다. 라이선스 파일의 존재만으로 해당 사용자의 구매/취득·재배포 권한을 증명하지 않는다.

5-2. 비교 가능한 원본/버전과 hash·meta를 대조해 `동일 원본/프로젝트 수정 원본/새 파생물/프로젝트 소유 연결 설정/미확인`을 나눈다. TMP Settings/Material/폰트 atlas·preset 등은 이름이 외부 폴더 아래 있다는 이유로 버리지 않는다. 변경 원본의 별도 확장/설정 재현 필요성은 Phase 2 설계 항목으로 기록한다.

5-3. 재취득에 필요한 제품/버전/공식 위치·도입 담당·오프라인/계정 제한·증거 없는 항목을 표시한다. 개인 계정/영수증/키·비밀 URL은 문서화하지 않는다. 실제 원본 다운로드·유료 구매/계약·법적 권한의 추정 판정은 하지 않는다.

### 완료 조건

- [x] 출처/변경/파생 근거와 취득·재취득/사용 조건의 확인/미확인 항목이 분리됐다.

### Step 5 수행 결과 — 2026-11-19

#### 5-1. 출처·라이선스/Notice 근거와 확인 상태 분리 — 완료

[출처/변경/사용 조건 metadata](20261119_01_Phase1Step5Provenance.json)에 Step 3~4 입력 해시와 공식 확인 위치·확인일·로컬 조건 자료의 경로/해시를 기록했다. 현재 사용자 도입 Third Party 애셋이 없다는 전제를 유지하며 UPM/TMP/보조 의존성을 기술적 기준선으로 조사했다. 원본/라이선스 본문을 새로 복제하지 않았고 개인 취득 정보·계정·키·영수증을 조사하지 않았다.

UPM 61개 중 27개의 package 루트에서 LICENSE/Notice 41파일을 확인했다. 나머지 34개는 해당 위치에 개별 license 파일이 없다는 결과이며 조건 없음/자유 사용으로 판정하지 않는다. metadata에 package ID/버전·근거 위치와 미확인 상태를 남겼다. 본 조사는 package 루트 자료의 색인이며 package 내부 모든 구성요소의 법적 조건 검토 완료가 아니다.

| 자료 | 확인한 근거 | 기록/미확인 경계 |
| --- | --- | --- |
| Unity 패키지 | UCL을 가리키는 license 17개, Cinemachine 전용 code license 1개, Companion Package license 1개, Package Distribution license 1개, MIT 문구 2개, Unity 서비스 약관 참조 6개. 별도 Notice 13개 | 서로 다른 조건을 단일 Unity license로 덮지 않음. 버전별 동봉 자료와 하위 구성요소 Notice를 보존하고 계정/Engine/service 권한을 추정하지 않음 |
| uGUI/TMP | 로컬 uGUI LICENSE.md의 UCL 참조와 [공식 UCL 페이지](https://unity.com/legal/licenses/unity-companion-license) | 현재 공식 페이지 v1.4를 로컬 자료의 확정 license 버전이나 과거 취득 조건으로 소급하지 않음 |
| LiberationSans | 동봉 OFL 파일의 SIL OFL 1.1 표시와 [공식 원문](https://openfontlicense.org/open-font-license-official-text/) | 저작권/license 동봉·폰트 단독 판매 제한·수정 시 Reserved Font Name 조건 등을 도입/배포 확인 항목으로 기록. atlas 내용 차이만으로 법적 파생물/이름 변경 의무의 적용을 확정하지 않음 |
| EmojiOne | 동봉 Attribution은 공급 사이트 방문/조건 확인을 안내할 뿐 정확한 버전/license를 특정하지 않음. emojione.com은 현재 JoyPixels로 redirect됨 | legacy sample에 현재 JoyPixels 조건을 임의 적용하지 않음. 원본 버전/조건 근거 미확인, 권리 판정 보류 |
| root npm archive | Cloud Code 공급 local tgz 27개의 package.json에 Unlicense 표시 | 해당 archive metadata의 선언이며 모든 전이 의존성/Unity 서비스 사용까지 같은 조건으로 판정하지 않음. lock별 license label/미표시도 분리 기록 |
| NuGet 보조 의존성 | 요청한 정확한 버전 4개의 공식 NuGet Gallery 페이지 확인. Jint 4.16.4는 BSD-2-Clause, Newtonsoft.Json 13.0.3은 MIT 표시 | CloudCode.Core 0.0.7 / Apis 0.0.27은 License Info 본문 fetch가 cache miss로 실패해 미확인. configured feed/restore/배포 권한 검증 아님 |

NuGet 버전별 공식 URL은 JSON에 남겼다. root package.json의 ISC 표시는 프로젝트 선언이며 외부 구성요소의 재라이선스 근거로 사용하지 않는다. license 파일 존재·카탈로그 접근과 실제 사용자의 취득/사용·재배포 권한을 구분한다.

#### 5-2. 원본/변경/파생·프로젝트 연결 자료 분류 근거 — 완료

Step 4의 비교 metadata와 현재 82파일 해시를 재대조했다. 공급자는 설치된 uGUI 2.0.0의 TMP Essential Resources archive이며 동일한 로컬 배포물과의 비교다. 공식 원격 배포물 진위/과거 사용자 취득 내역을 증명한 것은 아니다.

| 분류 | 이번 근거와 보존 방향 |
| --- | --- |
| 동일 로컬 배포 원본 | TMP 80파일 byte 일치. meta 45개·GUID도 일치. 같은 공급 버전/해시·GUID를 통한 재취득 근거로 남기되 사용 조건 확인은 별도 |
| 변경된 외부 원본·원인 미확인 | LiberationSans SDF.asset / LiberationSans SDF - Fallback.asset 2개는 개행 외 내용 차이. 폰트/Material/atlas·직렬화 필드 차이는 있으나 의도적 수정/Unity 변환·작성자를 단정하지 않음. 보존/판정 대기이며 자동 제외·폐기 금지 |
| 프로젝트 연결 설정 역할 | TMP Settings는 원본과 byte 일치하지만 기본 폰트/sprite/줄바꿈 설정 역할이 있으므로 재적용 계약을 유지. 외부 폴더라는 이유로 자체 설정 손실을 허용하지 않음 |
| 신규 파생물 | 현재 근거로 새로 제작한 파생물이 확정된 항목은 없음. 변경된 원본을 자동으로 순수 자체 소유로 승격하지 않음 |
| 출처/원본 비교 미확인 | 프로젝트 Scene/Prefab/Material/렌더링 설정은 기존 자료로 보존. 경로/파일명·비교 원본 부재만으로 순수 자체 소유·무변경을 확정하지 않음 |

Phase 2는 변경 원본의 보존 또는 검증된 재적용 방법·프로젝트 설정 분리/확장 경계를 마련해야 한다. 현재 Task에서는 원본 이동/추적 변경·재임포트/Scene 수정 없이 판단 근거만 인계한다.

#### 5-3. 재취득 정보·담당/제한·미확인 항목과 후속 연결 — 완료

| 대상 | 확보한 재취득 정보 | 남은 제한/의무 |
| --- | --- | --- |
| UPM/builtin | ID/요청·해결 버전·registry/source·Editor 6000.3.5f2·manifest/lock·동봉 조건 경로 | 계정/Engine/service 조건·네트워크/오프라인 재취득 성공은 미확인. cache 존재를 독립 복원 성공으로 보지 않음 |
| TMP 복사 자료 | uGUI 2.0.0 archive 위치/해시·원본 비교·GUID/meta·Resources/Shader 이름·참조 | 폰트 asset 2개의 유지/재현과 EmojiOne 정확한 조건은 Step 6/8에 인계. 확인 불가 항목은 보존/보류 방향과 도입/배포 전 해소 조건을 정함 |
| npm/NuGet/LogSink | 선언/lock·공급 archive·정확한 NuGet 버전별 공식 위치·자체 서버 소스 경계 | cache suffix/Windows 경로 이식성·NuGet feed/해결 graph·미확인 license·Node/container 조건은 실제 복원/패키징 전 별도 의무 |
| 향후 새 애셋 | 현재는 미등록. 프로젝트 소유자가 도입/갱신을 담당하고 AI가 metadata/검사 기록을 지원 | 제품/버전·제작자/공식 출처·license 버전/조건 근거·사용 목적·복사/수정·Git/meta·재취득 방법/담당·계정/오프라인 제한을 등록 시 필수 확인. 원본/비밀값 없는 기록 형식은 Step 6 |

확인 불가능한 권한/출처를 정상으로 채우지 않았으며 사용자에게 과거 구매/사용 이력이나 영수증을 요청하지 않는다. 현재 목표는 관리 방안 준비이므로 제품 구매/다운로드·새 애셋 도입은 필요하지 않다. 필수 경계 결정에 실제 사용자 정보가 필요한지는 Step 6 초안 후 Step 8에서 최소 항목으로 판정한다. **현재 사용자 수동 작업 없음.** 미확인 조건은 완료된 권리 판정이 아니지만, 확인/미확인 근거 분리라는 Step 5 완료 조건은 충족했다. 다음은 Step 6 관리 기록 형식과 유지/제외 경계안 작성이다.

## Step 6. 비추적 원본과 프로젝트 소유 자료의 경계안을 만든다

### AI 작업

6-1. Step 2~5 결과로 아래 표를 작성한다. 근거가 부족하면 `보존/판정 대기`로 두며 확인 없이 비추적 대상을 확정하지 않는다.

| 필수 열 | 기록할 판단 |
| --- | --- |
| 항목/범위·출처 | 실제 경로와 제품/버전, 원본/파생/프로젝트 소유/혼합/미확인 |
| 추적/제외 방향 | 현 추적/index/ignore와 Phase 2의 유지/비추적 후보/추가 결정 필요. 현재 조작은 없음 |
| meta/Import | 자산·폴더 meta 각각의 소유/보존 또는 검증된 재취득 방법·GUID/Import 불일치 영향 |
| 도입/사용 조건 | 확인한 근거/재취득 방법·계정/설치 제한·미확인 정보 |
| 참조/복원 계약 | 사용처, 남길 자체 자료/설정·manifest/lock/빌드 선언, 필요한 재적용 정보 |
| 검증/후속 | 정적/Unit Test·필요한 Unity 검증, Phase 2 적용/Phase 3 독립 복원 확인 항목 |

6-2. manifest/lock·ProjectSettings·자체 Scene/Prefab/Material/Script·연결용 설정·필요한 Attribution/도입 정보의 보존 범위를 정한다. `.meta` 전체 비추적/전체 추적 같은 일괄 규칙 대신 원본 배포의 GUID 보존과 프로젝트 변경 손실을 항목별로 판단한다.

6-3. 향후 애셋을 계속 관리할 기록의 형식·단일 관리 위치·등록 담당·등록/갱신 시점을 정한다. 기존 Project 문서와 책임을 대조해 기존 문서 확장 또는 별도 register를 선택하며, 관리 기록 자체를 선택 사항으로 두지 않는다. 필수 항목은 애셋 ID/제품·버전, 제작자·공식 출처/취득 경로, 배포 라이선스·사용 조건/Attribution과 근거·확인 상태, 원본 보관/재취득 방법, 프로젝트 복사 경로·수정/파생 내역, 사용 목적·게임 내 적용 위치와 기술적 참조, Git 유지/제외·meta/GUID/Import 설정 보존, 업데이트/제거·복원/검증 이력이다. 도입 애셋이 없는 현재는 빈 등록 형식과 작성 규칙을 준비하고 실제 항목을 꾸며 넣지 않는다. Phase 1 조사/판정 이력은 본 Task에 남기고 중복 register를 만들지 않는다. metadata만 기록하며 원본/비밀값은 넣지 않는다.

### 완료 조건

- [x] 원본/파생/프로젝트 소유·meta·의존성 선언의 유지/제외 경계안과 근거/보류 조건이 작성됐다.

### Step 6 수행 결과 — 2026-11-19

#### 6-1. 자료별 유지/제외 경계안과 보류 조건 — 완료

Step 2~5의 실제 목록/추적·출처·원본 비교·참조/조건 근거로 아래 초안을 정했다. 현재 사용 중인 Third Party 애셋이 없다는 사용자 전제를 유지하고 가상 도입/구매 항목을 생성하지 않았다. 아래 방향은 경계안이며 실제 index/ignore·파일/폴더/meta·Scene/Prefab 변경이나 권리 확정이 아니다.

| 항목/출처·범위 | 현 추적/제외 방향 | meta/Import 보존 | 조건·재취득 상태 | 사용처·복원 계약 | 검증/후속 |
| --- | --- | --- | --- | --- | --- |
| 향후 Third Party 원본 | ThirdParty 폴더/meta 없음, ignore 규칙만 있음. 도입 전 등록·원본 비추적 후보와 자체 자료를 분리 | 자산/폴더 meta 각각 동일 GUID/Import 재현 근거 필요. 근거 없으면 대기 | 공식 공급 버전/사용·수정·배포 조건/담당/계정·오프라인 제한 확인 후 결정 | 원본 보관과 프로젝트 복사 위치·자체 설정/사용처를 연결 | 실제 제외 후보를 임의 만들지 않음. Phase 2 도입/유입 검사, Phase 3 복원 |
| TMP 동일 배포 자료 80파일 | 이미 비추적/무시. 동일 로컬 배포물 근거로 재취득 후보를 식별하되 구성요소 조건 미확인은 대기 | meta 45개/GUID는 로컬 archive와 동일. 향후 importer 결과는 미검증 | 공급 uGUI 2.0.0/해시 고정. 파일 종류별 UCL/OFL/Notice·EmojiOne 미확인 구분 | Scene의 폰트/Material·fallback·Resources·Shader 이름을 재현 | Step 4~5 근거. 기존 ignore만으로 복원/권리 성공 아님 |
| TMP 폰트 asset 변경본 2개 | 이미 무시지만 동일 원본으로 폐기/덮어쓰기 금지. 별도 보존/검증된 재적용 방향, 최종 경계 대기 | 기존 GUID/meta·Material/atlas subasset·폰트 feature/직렬화 차이 손실 방지 | 원인/작성자·법적 파생 취급 미확인. 원본/변경 근거를 유지 | font/fallback 참조와 연결. 순수 자체 소유로 승격하지 않음 | Step 6/8 보존 결정, Phase 2 경로/재적용 구현, Phase 3 복원 |
| TMP Settings/Material·연결 설정 | 경로가 TMP 아래여도 설정 역할을 보존. 변경 시 자체 설정/재적용 정보 관리 | 자산/폴더 GUID·Importer/platform·Resources 상대 경로 보존 | 현재 원본 byte 일치와 프로젝트 설정 책임은 별개 | 기본 font/sprite/style/줄바꿈·Shader 이름의 동일성 필요 | Step 9 필요한 실제 해석 판정. 현재 설정 편집 없음 |
| 프로젝트 Scene/Prefab/Material/Script·ProjectSettings | 추적 자료 유지. 출처 비교 불가 범위도 보존/판정 대기. 원본 제외에 섞지 않음 | 프로젝트 생성 meta와 설정 변경은 추적 보존 또는 검증된 재적용이 필요 | 경로만으로 모든 자료의 자체 소유/무변경을 확정하지 않음 | 자체 연결·Prefab 생성·UI/렌더링·Input 계약 보존 | URP 미해결 GUID 11종은 Step 9 판정. 임의 참조 삭제/대체 없음 |
| UPM/npm/NuGet·서버 선언 | manifest/lock·csproj·EmbeddedResource와 자체 서버 소스 유지. 설치 cache/bin/obj 등과 분리 | Unity 연결 meta는 유지, 서버 빌드 선언에 Unity meta 정책 일괄 적용 안 함 | 정확한 버전·source/조건·사용 권한을 구분. NuGet 일부 본문/해결 graph 미확인 | npm tgz cache 경로 이식성·provider 재취득·서버 소스 리소스 보존 | Phase 2 절차/Phase 3 독립 복원, 서비스/배포 실행 없음 |
| license/Notice/Attribution | 필요한 근거/표시·배포 정보를 보존/재취득. 유료 원본 본문을 Git에 복제하는 근거로 삼지 않음 | 연관 애셋 버전/해시와 연결, 원본 meta 여부는 해당 자료별 판단 | EmojiOne·일부 NuGet·사용자 권한은 미확인. 현재 원본 보존/권리 판정 보류 | 허용되는 고지 자료와 최종 배포 동봉/표시 위치를 Phase 2에서 정함 | Step 8 필수 정책 판단, 실제 도입/배포 전 조건 해소 |
| 비밀값·개인 취득 자료 | 기존 비추적 경계 유지. 등록에는 원문/계정/키/비밀 URL 없음 | 관련 비밀 입력 meta를 원본 재도입 계약으로 사용하지 않음 | 개인 증빙 수집 없이 공개 공급 위치·조건 확인 상태만 기록 | production 입력·실제 저장/계정과 애셋 기록 분리 | 현재 읽기/삭제/이동/새 Secret 생성 없음 |

원본/변경/자체가 혼합된 제품은 전체 폴더를 한 번에 제외하지 않고 경로/구성요소별로 나눈다. TMP ShaderGraph texture 1종과 package sample orphan도 Step 9 영향 판정 의무로 유지한다. 보류 항목을 경계 확정으로 표시하지 않으며, Step 6 완료는 초안·보존/보류 근거의 작성 완료다.

#### 6-2. 선언·자체 자료·meta/고지 보존 계약 — 완료

manifest/lock·ProjectSettings·자체 Script/Scene/Prefab/Material·서버 빌드/리소스 선언과 필요한 연결 설정은 유지한다. 비교 원본 없는 자료는 확인 없이 제거하지 않는다. 원본 자산과 폴더 meta는 각각 동일 버전/GUID·Import 설정을 다시 만들 근거가 있어야 비추적 후보가 된다. 프로젝트 생성 GUID/Importer 변경·설정과 변형은 보존 또는 검증된 재적용이 필요하다.

향후 경로안은 원본 `Assets/ThirdParty/<공급자>/<제품>/`, 자체 변형/연결 자료 `Assets/ProjectAssets/<asset_id>/`로 정했다. 현재 폴더를 생성/이동하지 않았으며 Phase 2에서 실제 구성·ignore 예외/유입 검사를 검증한다. 버전은 기록으로 관리하며 같은 GUID의 복수 버전을 동시에 도입하지 않는다. 파생물에 원본 GUID를 복제하거나 원본을 직접 고쳐 사용처를 숨기지 않는다. 고지/복원용 metadata와 실제 원본의 배포 허용 여부도 구분한다.

반복 적용할 보존/상태/갱신 기준은 [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)의 외부 애셋 관리 기준에 정리했다. 기존 코드/규칙을 무관하게 재구성하거나 원본/계정/이력을 삭제하지 않았다.

#### 6-3. 향후 관리 기록·형식·담당/갱신 시점 확정 — 완료

Project README/Overview/Architecture/Memory와 기존 Templates를 대조했다. 프로젝트 개요·구조·현재 진행 요약에 제품별 영구 기록을 누적하면 책임이 혼합되며, Task metadata는 특정 시점 조사 이력이므로 지속 관리 자료를 대신할 수 없다. 따라서 새 단일 관리 기록을 Project에 두고 형식/규칙/작업 이력을 각 영역으로 구분했다.

| 책임 | 최종 위치·내용 |
| --- | --- |
| 실제 제품/의존성 항목 | [ASSET_DEPENDENCIES.md](../../00_Project/ASSET_DEPENDENCIES.md). 현재 도입 애셋 0개·실제 항목/가상 예제 없음. 기술적 기준선은 기존 선언과 Task 증거로 연결 |
| 빈 형식 | [ASSET_DEPENDENCY_TEMPLATE.md](../../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md). 필수 필드 23개·경로별 경계·변경/검증 이력 표 |
| 관리/갱신 기준 | Implementation Rule. 프로젝트 소유자가 도입/정책 결정, AI가 metadata/정적 검사 지원. 도입·조건·경계·복원 상태를 독립 기록 |
| 진행 요약/상세 이력 | Project Memory·Roadmap은 현재 상태와 링크, 본 Task는 Step별 근거/결정. 원본 제품 기록의 사본을 만들지 않음 |

형식은 제품/버전·공식 출처·license/사용·수정·배포/고지 근거·재취득/담당/제한·원본 보관/복사·변경/파생·선택 이유/게임 사용처·직접/동적 참조·Git/meta/Import·업데이트/제거·복원/검증·결정/갱신 정보를 포함한다. 후보 선택→복사/도입 전→첫 게임 적용→원본/설정/버전/사용처 변경→제거→배포/복원 전의 갱신 시점을 정했다. 빈값을 성공으로 보지 않고 미확인/해당 없음의 이유를 요구한다.

Step 6 산출물은 기존 조사에 근거한 경계안과 반복 사용할 기록 체계다. 기존 폰트 변경본·EmojiOne/보조 조건의 필요한 결정은 Step 8, 미해결 참조/실제 Unity 해석 필요성은 Step 9, 도구/Unit Test는 Step 7에 남긴다. **현재 사용자 수동 작업 없음.** AI는 Unity Compile/Build/Runner·Scene/Prefab 편집·실제 추적 해제/애셋 이동을 수행하지 않았다. 다음은 Step 7 최소 정적 검사와 Unit Test 준비/오프라인 검증이다.

## Step 7. 정적 검사와 Unit Test로 목록/판정의 신뢰성을 확인한다

### AI 작업

7-1. 기존 `Tools/Tests`·UGS preflight의 역할/부작용·중복 기능을 먼저 읽고 재사용한다. 파일 수를 고정 assertion으로 맞추거나 문서의 결론을 Test에 복사하지 않는다. 반복할 경로/목록/참조 검증에 도구가 필요하면 순수 함수와 읽기 전용 수집부·CLI를 최소로 분리한다.

7-2. 새 도구가 필요한 경우의 **제안 경로**는 `Tools/DependencyAudit/dependency-audit.cjs`, `Tools/DependencyAudit/dependency-audit.test.cjs`다. 두 파일/CLI는 현재 없으며 실행 시 구현/재사용 선택과 최종 경로를 기록한다. require.main 보호·외부 package 없이 기존 Node/assert 방식으로 검사한다. 실제 repo는 읽기만 하고 결과 기록 위치는 명시적으로 제한한다.

7-3. 다음 실패 입력을 실제 수집/판정 함수의 Unit Test로 검증한다. 합성 metadata/주입 fs·Git 결과를 쓰며 유료 원본이나 실제 계정 파일을 fixture에 넣지 않는다. 필요한 filesystem/Git 통합 case는 격리된 임시 저장소만 쓰고 현재 index/ignore/설정을 바꾸지 않는다.

| Unit Test 영역 | 필수 정상/실패 case와 assertion |
| --- | --- |
| 경로/목록 | 공백/한글·Windows separator·NUL 경로/개행, stable sort·중복 제거, 다른 디렉터리의 동명 파일 구분 |
| 조사 범위 | `..`/절대 외부 경로·junction/symlink·권한/읽기 실패·부분 열거를 성공한 빈 목록으로 처리하지 않음 |
| Git/ignore | 추적 중이지만 ignore 매칭, 미추적/무시, staged 추가/삭제/rename·중첩 예외·대소문자 차이. Git exit 0/1/실제 오류를 구분 |
| 패키지 graph | 정상 직접/전이·builtin/registry와 local/git 유형, malformed JSON·missing/version/source 불일치·해결 불명/순환의 명시적 진단 |
| GUID/meta | 자산/폴더 meta, builtin/패키지 GUID, 중복/깨진 GUID·missing/orphan meta·해결되지 않은 참조. 무시된 TMP metadata도 실제 검색 입력에 포함 |
| 참조 한계 | GUID 없이 Resources/Shader 이름/동적·binary 자료가 남으면 미사용/복원 성공으로 판정하지 않음 |
| 소유/분류 | 혼합/수정/파생·출처 불명, meta/설정 보존 근거 없는 제외 후보는 자동 확정하지 않음. 사용자 결정 전후 상태를 분리 |
| 보안/보고 | credential/개인 취득값·원본 payload 비노출, 동일 입력의 재현 가능한 출력, 실패 시 비정상 exit/미완료 상태 유지 |
| 읽기 전용 통합 | 실제 CLI 진입·주입 adapter 오류, index/기준 파일의 전후 동일성. fixture Test를 실저장/원격 요청으로 바꾸지 않음 |

7-4. 검사 파일/의존성에 write/삭제·프로세스/네트워크·Unity 호출이 없는지 감사한 뒤 Unit Test와 현재 inventory 검사를 실행한다. 제안 CLI 채택 시 명령은 `node Tools/DependencyAudit/dependency-audit.test.cjs`, `node Tools/DependencyAudit/dependency-audit.cjs --check`이며 실제 인터페이스를 구현한 후에만 실행한다. 개수·exit code·실패 이름/원인/수정/재실행·한계를 기록한다. 테스트 가능한 규칙을 사람의 파일 열기/눈대중으로 대신하지 않는다.

### 완료 조건

- [x] 재사용/최소 도구와 실제 판정 함수의 Unit Test가 준비·검증됐고 실패/누락을 감추지 않는 inventory 검사 결과가 기록됐다.

### Step 7 수행 결과 — 2026-11-19

#### 7-1. 기존 검사 자산의 책임/부작용 검토 — 완료

Tools/Tests의 inventory·EditMode/PlayMode review와 해당 test, UGS/Tests/client-unity-preflight.cjs를 읽었다. C# Test 본문/사례 분류·과거 리뷰 CSV/고정 건수·특정 Scene 연결 assertion에 묶여 있어 신규 의존성 검사의 수집/판정 함수로 직접 호출하면 무관한 과거 전제를 실행하게 된다. 기존 파일을 수정/복제하거나 과거 건수를 현재 inventory의 assertion으로 삼지 않았다. Node 내장 fs/path/crypto·assert, require.main 보호·합성 입력 방식은 재사용하고 의존성 책임만 별도 구현했다. 기존 Unity/UGS suite를 무조건 재실행하지 않았다.

#### 7-2. 읽기 전용 수집/순수 판정·CLI 구현 — 완료

- [dependency-audit.cjs](../../../Tools/DependencyAudit/dependency-audit.cjs): 경로/NUL·staged/ignore·graph·GUID/meta/참조·한계·분류·등록 형식의 순수 함수와 fs/Git adapter·수집부·CLI를 분리했다. 외부 npm/Runtime System/C#을 추가하지 않았다.
- [dependency-audit.test.cjs](../../../Tools/DependencyAudit/dependency-audit.test.cjs): 실제 exported 수집/판정 함수를 정상/실패 입력으로 검사한다. fs/Git 실패를 주입하고 실제 Git 통합은 임시 저장소에서만 수행한다.
- [사용/범위 안내](../../../Tools/DependencyAudit/README.md): 명령·종료 코드·수집/판정 한계·기준선 선택을 기록했다. stdout JSON 외에 도구 자체의 write/삭제/추출·네트워크·Unity/서비스 호출은 없다.

Git 프로세스는 HEAD/추적/staged/ignore를 읽기 위해 고정된 argument 배열만 허용한다. add/rm/config 변경·diff output 파일 등은 차단하며 optional locks/fsmonitor·external diff/textconv도 끈다. CLI의 Node 실행과 fixture용 Git 초기화/commit/staging은 Test/검증 호출자 책임이며 실제 저장소에는 적용하지 않는다. 본래의 프로세스 감사 요구를 무시한 것이 아니라 Git read라는 필요한 예외의 범위/차단 근거를 명시한 것이다.

`--check`는 현재 목록을 읽고, `--baseline <저장소 내 상대 경로>`를 지정한 경우에만 과거 입력 해시도 확인한다. 특정 날짜 Task를 영구 정답으로 하드코딩하지 않았다. `--root`는 명시한 Git 저장소 루트 선택이며, 수집 경로/baseline의 외부 절대 경로·상위 이동·NUL은 거절한다. symlink/junction·권한/부분 열거·JSON/BOM·Git 오류는 빈 성공으로 숨기지 않는다. Secret 본문을 읽지 않으며 `.meta`는 GUID/Importer metadata로만 해석한다.

`ownership`은 folder/ignore로 소유권을 추정하지 않고 현재 실제 항목을 보존/판정 대기로 둔다. 분류 함수는 수정/파생/혼합·조건 불명·meta/Import 재현 근거 또는 필요한 사용자 결정이 없는 경우 제외 후보를 확정하지 않는다. 사용자 결정 전/후와 실제 적용을 별개로 반환한다. 채워진 제품/사용 권한을 만들어 등록하지 않았다.

#### 7-3. 정상·실패 입력 Unit Test — 완료

`node Tools/DependencyAudit/dependency-audit.test.cjs`는 **54개 통과/실패 0개, exit 0**이다. 최초 51개를 통과한 뒤 해결 버전 불명/전이 요청보다 낮은 registry 버전·선택 baseline·optional root 파일의 접근 실패 처리를 보완해 전체 54개를 다시 통과했다. 테스트 가능한 규칙을 사람의 파일 열기로 대체하지 않았다.

| 검증 영역 | 실제 확인한 정상/실패 입력 |
| --- | --- |
| 경로/목록 | 공백/한글/개행·Windows separator·NUL protocol·stable sort/중복 제거·다른 폴더의 동명 파일·대소문자 충돌 |
| 조사 경계 | 상대 상위 이동/외부 절대 경로·junction/symlink metadata·권한/읽기 오류·부분 열거·optional 파일 접근 실패 |
| Git/ignore | tracked+ignore·미추적 무시/예외·staged A/D/R/C·중첩 부정 예외·exit 0/1/128·잘린 NUL/불완전 응답·mutation/output flag 차단 |
| package graph | 직접/전이·registry/builtin/local/git·유효 JSON/BOM·잘못된 JSON/schema·누락/버전/source/해결 불명·순환·UPM 전이 해결 버전 차이 |
| GUID/meta | 자산/폴더·ignored TMP metadata 실제 검색·builtin/null/package·중복/깨진 GUID·missing/orphan·unresolved·중첩 ShaderGraph JSON |
| 참조 한계 | GUID 없는 Resources/Shader 이름·reflection/Instantiate·binary가 남으면 미사용/독립 복원 성공으로 판정하지 않음 |
| 경계 분류 | 원본/수정/파생/자체/혼합/미확인·설정 보존·권리/출처/meta/Import 근거 누락·사용자 결정 전/후·실제 적용과 권리 부여 아님 |
| 보안/출력 | 합성 credential URL/비밀값·malformed payload 비노출·민감 본문 차단·동일 입력의 동일 출력·실패 시 nonzero/미완료 |
| 읽기 전용 통합 | require 시 CLI 미실행·actual CLI·adapter 실패·임시 repo index/기준 파일 전후 동일·baseline 변경 검출 |

fixture는 합성 metadata와 시스템 임시 폴더에 만든 저장소만 사용하고, 정리 전 절대 경로/부모·고정 prefix를 확인한다. 실제 계정/저장/원격·유료 원본 fixture는 사용하지 않았다. 실제 Unity Runner 결과로 기록하지 않는다.

#### 7-4. 현재 inventory 검사·실패 분리·인계 — 완료

최종 명령은 `node Tools/DependencyAudit/dependency-audit.cjs --check --baseline AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json`이다. [현재 검사 결과](20261119_02_Phase1Step7Check.json)에 UTF-8 JSON을 보존했다. 호출자가 출력 경로를 제한해 기록했으며 도구는 저장소에 결과 파일을 쓰지 않는다. 초기 PowerShell `>` 결과는 UTF-16 인코딩이어서 UTF-8로 정정했고 최종 결과는 Node 호출자로 UTF-8 저장했다.

| 결과 | 판정 |
| --- | --- |
| Unit Test 54개 / exit 0 | 검사 도구의 구현/실패 입력 검증 통과 |
| 실제 목록 828파일·50디렉터리·ignore 85파일 | Step 2 대비 Tools/DependencyAudit의 파일 3개/폴더 1개 추가. 원본/생산 자산 변경 아님 |
| package 61개/관계 91개·meta 11,816개·직렬화 56파일 | current metadata/참조 수집, baseline 입력 해시 66개 일치 |
| 참조 1,299회 | Step 4 YAML 1,270회 + ShaderGraph 24회 + meta importer 5회 |
| REFERENCE_UNRESOLVED 31회·ORPHAN_META 1개 | Step 4의 URP 11종/15회 + TMP graph texture 1종/16회와 같은 미해결, package Samples~ orphan. 현재 자산 정상 PASS 아님 |
| CLI exit 1, collectionComplete=true / error 0개 | 수집 실패 없이 finding을 검출한 정상적인 실패 반환. 알려진 문제를 allowlist/삭제/자동 보충으로 숨기지 않음 |
| boundaryComplete=false / independentRestore=false | 사용자 결정/전체 경계·실제 Unity 해석·독립 복원은 별도이며 완료 처리하지 않음 |

최종 동일 입력으로 두 번 실행한 stdout은 byte 일치했다. 자동 테스트/도구 defect와 기존 자산 finding을 구분한다. Step 7의 완료 조건은 도구/Unit Test 신뢰성과 실패를 숨기지 않는 inventory 결과 기록이므로 충족했으며, 기존 참조의 영향/필수 여부는 계획대로 Step 9에서 판정해야 한다. 최종 경계/필수 실패가 남아 있으면 Phase 1은 완료하지 않는다. **현재 사용자 수동 작업 없음.** 다음은 Step 8 필요한 사용자 정보/정책 최소 항목 정리다. Unity/Build/Runner·Scene/Prefab 적용은 없다.

## Step 8. 필요한 사용자 정보/정책만 확정한다

### AI 선행 작업

8-1. 앞선 근거로 해결하지 못한 **실제 경계 결정을 막는 항목만** 모은다. `대상/확인한 사실/부족한 정보/가능한 보존 방향/후속 영향`을 준비하고 한 번에 이해할 수 있게 제시한다. 로컬 파일/출처 문서/Git으로 확인 가능한 사항을 사용자에게 다시 조사하게 하지 않는다.

### 필요한 경우의 사용자 작업

- 로컬/공식 자료에서 확인할 수 없는 실제 취득 경로/버전·재취득 가능 여부·담당 또는 프로젝트가 의도적으로 수정한 외부 원본의 유지 방향만 확인한다. 비밀값/계정 화면/영수증 원문은 보내지 않는다.
- 권한/출처 근거가 없거나 경계가 불명확하면 보존/보류 방향과 후속에서 확정할 대상을 결정한다. 현재 추적 해제/삭제를 수행하는 승인이 아니다.

### AI 결과 반영

8-2. 답변/근거·적용 범위를 분류표와 Unit Test의 정책 입력에 반영한다. 사용자 답변이 필요한 필수 항목은 기다리고, 답변 없음/시간 경과를 승인으로 처리하지 않는다. 필수 제외 경계를 확정하지 못하면 해당 범위는 미해결로 남겨 Phase 1 완료를 막는다. 비차단 후속 의무는 적용 대상/제한을 명시해 인계한다.

### 완료 조건

- [x] 필요한 사용자 결정이 반영됐거나 사용자 작업 불필요 근거가 있고, 필수 미확정 경계를 완료로 표시하지 않았다.

### Step 8 수행 결과 — 2026-11-19

#### 8-1. 실제 경계 결정에 필요한 정보와 사용자 작업 필요성 대조 — 완료

Step 5 미확인 목록·Step 6 보존/갱신 기준·Step 7 실제 finding을 대조했다. 현재 목적은 향후 애셋 관리 체계 준비이며 사용자 도입 애셋은 없고, 이번에 새 구매/도입·추적 제외·원본 덮어쓰기/삭제·서버 패키징/배포를 수행하지 않는다. 따라서 기존 자료/선언을 보존하는 데 수정 의도·개인 취득 증빙·새 정책 선호가 필요하지 않다. 이 범위에서는 사용자에게 추가 결정을 요구할 필수 항목이 없다고 판정했다.

이는 사용자의 새 답변/승인을 받은 결과가 아니다. 기존 사용자 범위와 Implementation Rule의 보수적 보존 기준을 적용한 것이다. 답변 없음/시간 경과를 승인으로 해석하지 않았고 확인 불가 권한/소유/재취득 성공을 채우지 않았다.

| 대상 | 확인한 사실 | 부족한 정보 | 현재 보존 방향·추가 사용자 입력 불필요 근거 | 후속 영향/재확인 시점 |
| --- | --- | --- | --- | --- |
| TMP 폰트 변경본 2개 | 원본 대비 내용 차이·같은 meta/GUID, 현재 ignore 상태 | 작성자/수정 의도·검증된 재현 방법 | 현 파일/GUID를 그대로 보존하며 제외/덮어쓰기 후보로 확정하지 않음. 수정 의도와 무관하게 기존 내용을 잃지 않는 방향은 같음 | Phase 2에서 보존/재적용 구현. 실제 상충하는 선택이 생길 때만 사용자 결정 필요 |
| EmojiOne legacy 자료 | 공급/Attribution은 있으나 버전별 license 근거 미확인 | 사용/수정/게임 배포·원본 재배포 조건 | 현 기술 기준선 보존, 새 도입/재배포 판단 보류. 현재 구매/배포/삭제 선택이 없으므로 영수증/계정 정보를 요구하지 않음 | 실제 도입/배포 또는 복원 범위에서 필요할 때 공급 근거 확인. 권리 승인 아님 |
| TMP 연결 설정 | 원본과 byte 일치하나 default font/sprite/Resources 경로 역할 존재 | 독립 환경의 Import/타입 해석 | 위치와 무관하게 기존 설정/GUID·재적용 계약 유지. 새로운 소유권/이동 결정 없음 | Step 9 검증 필요성, Phase 2 절차/Phase 3 독립 복원 |
| UPM/npm/NuGet·서버 선언 | provider/버전·manifest/lock/csproj/자체 source 경계 확인 | 일부 조건·사용자 Engine/service 권한·NuGet 해결 graph·cache 경로 이식성 | 선언/근거 보존, 권리·원격 실행 미확인 유지. 현재 서버 변경/애셋 원본 제외가 없으므로 해당 계정/게시 정보를 조사하지 않음 | 해당 실제 복원/패키징·사용/배포 전 조건 확인. Phase 2/3 의무를 제거하지 않음 |
| 향후 새 애셋 | 실제 도입 항목 0개, 등록 형식/상태/갱신 기준 존재 | 제품별 실제 출처/조건/사용처 | 등록부를 비워 두고 실제 후보 등록 시 필요한 정보만 확인. 가상 제품이나 license 선택을 요청하지 않음 | 실제 후보 취득/도입 시 관리 기록 갱신·필수 조건 해소 |

현재 Git/애셋 변경 요청·Phase 1 실제 제외 후보는 모두 0개다. 보존 처리 방향과 사용 조건의 미확인은 독립 상태로 유지한다. URP/TMP graph의 unresolved 참조와 sample orphan은 새 사용자 선호/구매 정보가 아니라 Step 9 영향 판정 대상이다. 이를 정책 확인의 이유로 사용자에게 파일 열기/빠른 수동 재현을 요구하지 않는다.

#### 8-2. 보존 정책 입력·Unit Test 연결과 후속 의무 — 완료

[정책 metadata](20261119_03_Phase1Step8Policy.json)에 5그룹의 사실/부족한 근거·보존 방향·후속/재확인 trigger와 Step 4/5/7·Rule 입력 해시를 기록했다. `mode=preservation-only`, `userInputRequired=false`, `userAnswers=[]`, 제외 후보/변경 요청 빈 목록을 명시했다. 새 승인·권리 확인이나 Phase 1 전체 완료는 false다. 이 파일은 보존 경계의 검증 입력이며 실제 도입 제품 등록부를 대체하지 않는다.

실제 `classification` 함수를 이 정책 입력으로 검증하도록 Test에 선택적 `--policy <저장소 내 상대 metadata 경로>`를 추가했다. 특정 날짜의 정책 파일을 기본 suite에 하드코딩하지 않았다. 각 그룹이 `preserve-pending`이고 실제 적용/권리 부여는 false인지, 수정본/연결 설정은 나중에 일반 근거를 채워도 자동 제외로 바뀌지 않는지 확인했다. 수집 CLI/분류 구현 자체는 수정하지 않았다.

실행: `node Tools/DependencyAudit/dependency-audit.test.cjs --policy AI/90_Tasks/Prototype_10/20261119_03_Phase1Step8Policy.json` — **기본 54개 + 실제 정책 입력 검증 1개(5그룹), 총 55개 통과/실패 0개, exit 0**. 합성/격리 테스트와 명시한 metadata 재생만 수행했으며 실제 애셋/저장·계정·서비스를 fixture로 쓰지 않았다. Step 7 inventory exit 1을 이번 정책 Test 성공으로 덮지 않는다.

[영구 관리 기록](../../00_Project/ASSET_DEPENDENCIES.md)에는 현재 보존 범위와 이 Step 근거만 연결하고 실제 제품 항목은 0개로 유지한다. 보존 방향은 기존 권한 범위에서 정리했지만 실제 제외/소유권·조건·복원 검증이 완료된 것은 아니다. 실제 도입/제외·배포/복원에서 새로운 상충 선택이나 필수 정보가 생기면 그때 사용자 결정을 받고, 필수 미확정 경계를 완료로 표시하지 않는다. Phase 1의 필수 조건 충족 여부는 Step 9~10에서 별도 판정한다.

**현재 사용자 수동 작업 없음.** 사용자에게 추가 답변/계정/영수증·Compile/Runner/Build·Scene/Prefab 적용을 요청하지 않는다. Step 8 완료 조건의 사용자 작업 불필요 근거와 미확인/보존 처리가 기록됐으므로 완료 처리한다. 다음은 Step 9 최종 정적 결과와 필요한 Unity 근거 연결이다.

## Step 9. 최종 정적 결과와 필요한 Unity 근거를 연결한다

### AI 작업

9-1. 분류표/패키지 graph·GUID/meta/Import·직접/동적 참조·근거 링크와 검사 출력이 일치하는지 대조한다. 실제 제외 후보가 0개여도 목록/출처/복원 요구를 기록하며 Roadmap 10 전체 완료로 간주하지 않는다.

9-2. Node 도구/문서만 추가되고 생산 자산/설정·C# 변경이 없다면 불필요한 Unity Compile/Runner·빌드/Player 반복을 요청하지 않는다. 현재 cache의 정적 성공과 독립 복원 성공은 계속 구분한다.

9-3. binary/실제 Unity 해석 때문에 필수 참조 판정이 남으면 최소 Editor-only EditMode Test를 준비한다. 제안 파일은 `Assets/Tests/EditMode/DependencyReferenceConfigurationTests.cs`이며 현재 미생성이다. 실제 `AssetDatabase`의 GUID→경로/LoadAsset·기존 asset/importer/package metadata를 읽고 저장/재임포트·UI 생성/씬 수정·온라인 초기화를 하지 않는다. source assertion만으로 타입 해석을 증명하지 않는다. 필요한 경우 실제 SampleScene 연동은 기존 ModeUISceneConfigurationTests/AccountTransferUIIsolationTests 등 영향 suite에 연결한다.

### 필요한 경우의 사용자 작업

1. 새 C# Test/컴파일 조건이 생긴 경우에만 최신 Compile과 AI가 지정한 Test Runner assembly/suite를 실행하고 통과/실패/미실행 수·정확한 실패/예상 밖 로그를 전달한다. AI는 실행하지 않는다.
2. Scene/Prefab 변경 필요성이 발견돼도 본 Phase에서는 변경 대신 후속 적용표를 준비한다. 실제 반영이 별도 범위로 확정될 때 `Hierarchy/컴포넌트/필드/기존→최종값/onClick·Navigation/저장·미리보기 원복`을 안내한다. 정적으로 확인 가능한 GUID 조사나 파일 열기를 수동으로 요구하지 않는다.

9-4. 실패/참조 누락·도구 문제를 수정하고 해당 영향 검사만 갱신한다. 실제 자산 이전/원본 제외·독립 복원은 Phase 2/3이며 이번 현재 설치 검사를 그 완료로 기록하지 않는다.

### 완료 조건

- [x] 최종 inventory/분류·정적/Unit Test 근거와 조건부 Unity 필요성/실제 결과가 연결되고 필수 실패/미실행이 없다.

### Step 9 수행 결과 — 2026-11-19

#### 9-1. 최종 inventory·분류/근거 연결 재대조 — 완료

[최종 정적 검토 metadata](20261119_04_Phase1Step9Review.json)에 현재 CLI 출력·이전 결과와의 동일성·미해결 항목별 영향/후속 gate·근거 20개 해시를 기록했다. 현재 사용자 도입 애셋 0개·등록 형식·보존 정책과 Step 3 graph/Step 4 참조/Step 5 출처·Step 6 경계·Step 8 정책을 대조했다. 실제 제외/변경 후보는 없으며 원본·조건 미확인·TMP 변경본/연결 설정을 유지한다.

최신 실행은 `node Tools/DependencyAudit/dependency-audit.cjs --check --baseline AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json`이다. **exit 1 / collectionComplete=true / 수집 error 0개**이며 실제 목록 828파일·50디렉터리, ignore 85파일, package 61개/관계 91개, meta 11,816개, 직렬화 56파일·참조 1,299회·baseline 해시 66개를 확인했다. Step 7과 package graph·참조 목록·diagnostic의 code/path가 정확히 일치했다. 기존 미해결 31회+orphan 1개는 여전히 finding이며 정상 PASS로 바꾸지 않았다.

#### 9-2. 기존 finding의 현재 범위/후속 영향 판정 — 완료

| 항목 | 추가로 확인한 정적 근거 | 현재 Phase 1 영향·미검증 한계 | 후속 실제 적용/복원 gate |
| --- | --- | --- | --- |
| PC_Renderer의 URP 7종/7회 | GraphicsSettings/QualitySettings→PC_RPAsset→PC_Renderer 경로 확인. 현 URP Runtime에서 `probeVolumeResources`는 Deprecated.cs의 Obsolete 선언이며, Core의 실제 resource 사용은 GraphicsSettings의 ProbeVolumeDebugResources/ProbeVolumeRuntimeResources로 연결 | 기존 자료를 보존하며 Renderer/package/Import를 바꾸지 않는 관리 경계 결정에 차단 항목은 아님. raw GUID는 미해결, 정상 migration/Runtime 로딩을 실행 증명한 것은 아님 | renderer/package 변경·재임포트/독립 복원 시 migration/현재 resource 해석과 필요한 Unity 진단 확인 |
| 기본 VolumeProfile의 script 4종/8회 | 프로젝트 global settings가 local DefaultVolumeProfile을 참조하고, 같은 GUID 누락이 공급 URP 기본 profile에도 존재. Core VolumeProfile.OnEnable에는 null component를 메모리에서 제거하는 코드가 있음 | profile을 삭제/수정하지 않고 유지하는 경계에 새 선택은 없음. null 제거 코드는 실제 LoadAsset/Console 무오류 증명이 아니며 Missing Script 영향은 미검증으로 남김 | 실제 profile 변경/재취득/복원에서는 타입/로드·예상 밖 로그를 확인. 자동 저장/원본 수리로 숨기지 않음 |
| TMP ShaderGraph texture 1종/16회 | 네 graph 모두 공급 archive와 byte 일치. 조사한 프로젝트 YAML에서 이 네 graph로 들어가는 GUID edge는 없음 | graph/모든 변형을 보존한다. 동적 variant 미사용이나 ShaderGraph Compile 정상 판정은 아님 | 해당 variant 실제 사용/재임포트·복원 시 importer/shader/texture 해석 검증 |
| package sample orphan 1개 | Test Framework의 Samples~ 폴더 meta, 현재 직렬화 closure에 incoming 참조 없음 | 제품/설정 제외 결정을 막는 항목은 아님. cache 삭제/수정 없이 finding 유지 | 동일 버전 package 재취득 시 공급 metadata와 프로젝트 복원 실패를 구분 |
| binary·subasset·이름 기반 참조 | TTF/PNG 해시·GUID/fileID anchor·Resources/Shader 이름/코드 연결의 정적 근거 유지 | 실제 asset 타입/importer/렌더링 검증은 하지 않았으나 이번에는 도입/이동/제외/사용처 변경이 없어서 현재 경계 판단이 그 실행 결과에 의존하지 않음 | Phase 2 구체 절차·Phase 3 독립 복원에서 필요한 실제 Unity 해석과 참조 확인 |

위 판정은 **현재 보존/향후 관리 방안이라는 Phase 1 범위에서의 비차단 분류**다. 애셋 정상·권리/독립 복원 PASS·이후 검증 면제/위험 수용 승인이 아니다. 과거 Roadmap 9 사용자 QA는 원래 범위/한계의 역사적 근거로 유지하고 이 raw GUID finding의 신규 검증 성공으로 사용하지 않는다. 실제 사용/원본 제외·재도입을 하게 되면 위 gate를 적용하고 필요한 근거가 없거나 실행 실패면 해당 작업을 중단한다.

#### 9-3. 조건부 Unity 검증 필요성·사용자 작업 판정 — 완료

현재 추가/변경은 Node 도구/Test·문서/metadata뿐이고 생산 C#/Unity Test·Scene/Prefab·자산/설정·package 버전·추적/ignore 변화는 없다. 사용자 확정 목표도 현재 애셋의 복원 성공이 아니라 향후 관리 방안 준비이며, 현재 결정은 모든 기준 자료 보존이다. 따라서 이 Step에서 새 Unity Compile/Runner·Build/Player로 증명해야 할 필수 변경/해석 판정은 없다.

제안됐던 `Assets/Tests/EditMode/DependencyReferenceConfigurationTests.cs`는 조건이 성립하지 않아 생성하지 않았다. 단순히 검사 finding을 0으로 만들기 위한 C# Test/Scene 수정·자산 보충/재임포트·baseline 고침도 하지 않았다. 실제 타입/LoadAsset·Import/Shader/표시 검증이 필요한 후속 적용에서는 최소 읽기 전용 EditMode Test와 정확한 사용자 실행표를 그 대상에 맞춰 준비해야 한다. 소스/캐시 검사를 실제 Unity 실행으로 기록하지 않는다.

Step 8의 정책 포함 Node Unit Test 55개/exit 0은 현재와 동일한 수집기/Test·정책 입력 해시의 근거로 연결했다. 코드/입력이 바뀌지 않아 같은 suite를 불필요하게 반복하지 않았으며 이번에 새로 실행했다고 기록하지 않는다. 이번 실제 재실행은 위 최종 inventory CLI다.

**현재 사용자 수동 작업 없음.** Unity Compile/Build/Test Runner/Player 실행이나 Scene/Prefab 적용을 요청하지 않는다. baseline/기존 자체 자료를 보존하며 현재 cache가 있는 상태와 신규 checkout 복원은 계속 구분한다.

#### 9-4. 수정 필요성·최종 Step 판정과 후속 인계 — 완료

도구 수집 오류·기존 결과 불일치·새 생산 변경이 없으므로 이번 범위에서 수정해야 할 새 결함은 없다. 기존 finding은 필요한 후속 gate/제한으로 명시했으며 임의 allowlist/자동 수정으로 통과시키지 않았다. 현재 범위의 필수 실패/미실행은 없고 Unity 검증 조건이 성립하지 않는 근거까지 연결해 **Step 9를 완료**한다. 이는 전체 참조 정상 판정이 아니며 Phase 1/Roadmap 전체 완료도 아니다.

다음은 Step 10에서 Phase 1의 세 완료 조건을 실제 산출물에 연결하고, 변경된 TMP/설정 보존·권리 미확인·실제 제외/동일 버전/GUID 재취득·관련 참조/Import 검증/중단 조건과 기존 Module 복구 의무를 Phase 2/3에 인계하는 것이다. 새 실제 적용에서 필수 미확정/실패가 생기면 완료로 처리하지 않는다.

## Step 10. Phase 1을 판정하고 Phase 2/3에 인계한다

### AI 작업

10-1. Roadmap 10 Phase 1의 세 완료 조건을 아래 결과에 연결한다.

| Phase 1 완료 조건 | 필요한 최종 증거 |
| --- | --- |
| 폴더명만이 아닌 내용/출처 기반 제외 범위 | Step 2/4/5/6/8의 실제 목록·참조·출처·분류/사용자 결정, 필수 미확인 없음 |
| Package 선언/lock 등 복원 정보 관리 범위 | Step 3/6의 요청/해결 버전·전이/source·도입/보존·보조 선언/복원 계약 |
| 복사본/파생물/민감 정보 경계 | Step 4~8의 원본 대비 변경·자체 설정/meta·원본/비밀값 없는 도입 기록·Unit Test 결과 |

10-2. 모두 충족하면 본 Task/Roadmap 10/Project Memory·공통 안내를 갱신해 **Phase 1만 완료**한다. 필수 출처/제외/meta 경계·실패가 남으면 해당 조건과 다음 작업을 명시하고 미완료로 유지한다. 장기 register가 필요한 경우 기존 문서 책임과 중복을 피한다.

10-3. Phase 2에 `실제 제외/유지 목록·meta/Import·직접/동적 사용처·동일 버전/GUID 재취득·프로젝트 소유 설정·필요한 사용자 결정·검사 명령/결과·변경 전 기준선`을 인계한다. 추적 해제 대상이 없으면 그 근거와 도입 규칙/복원 절차의 잔여 범위를 명시한다.

10-4. Phase 3에는 기존 작업본을 보존하는 별도 checkout/의존성 재도입 검증 계획과 성공/중단 기준을 인계한다. Phase 1의 cache/GUID 성공을 독립 복원 PASS로 승계하지 않는다. 후속 사운드/그래픽/UI 도입 전에 Roadmap 10 전체 완료가 필요하며, 패턴/난이도 작업이나 기존 원격 Module 복구 의무를 섞거나 완료 처리하지 않는다.

### 완료 조건

- [x] Phase 1의 실제 완료/미완료 판정과 Phase 2/3의 실행 대상·보존/검증·미확인 의무를 정확히 인계했다.

### Step 10 수행 결과 — 2026-11-19

#### 10-1. Phase 1 완료 조건과 최종 근거 대조 — 완료

사용자 확정 목표인 **현재 도입 애셋 없음·앞으로 도입할 애셋의 관리 기준 마련**에 맞춰 완료 조건을 판정했다. 현재 제외 후보가 없다는 이유로 관리 기록·정책/검사 준비를 생략하지 않았고, 기술적 기준선의 권리/복원 미확인을 사용 승인으로 바꾸지 않았다.

| Phase 1 완료 조건 | 실제 최종 근거 | 판정 |
| --- | --- | --- |
| 폴더명만이 아닌 내용/출처 기반 제외 범위 | Step 2 실제 파일/추적/ignore·Step 4 참조/원본 비교·Step 5 출처/조건·Step 6 경계·Step 8 보존 정책. 현재 실제 제외 후보/변경 요청 0개, 원본/수정/자체/혼합/미확인을 경로별로 구분하고 불명확한 자료는 모두 보존 | 충족. 알려지지 않은 원본을 임의 제외하지 않으며 도입 전 조건과 현재 보존 범위가 명확함 |
| Package 선언/lock 등 복원 정보 관리 범위 | UPM 직접 43/전체 61개·관계 91개, TMP 공급 archive/복사 자료·GUID/meta/설정, npm/NuGet/서버 선언을 Step 3/6/9로 연결. manifest/lock·ProjectVersion/자체 설정·csproj/EmbeddedResource/서버 소스 유지 | 충족. 재취득 정보와 이식성/실제 복원 미검증은 후속 의무로 분리 |
| 복사본/파생물·민감 정보 경계 | TMP 동일 배포 80/변경 2파일·meta 45개 동일 근거, 변경본/연결 설정 보존·license/Notice 확인/미확인 상태·등록 필드 23개/독립 상태 4축, 원본/비밀값 없는 metadata와 정책 포함 Unit Test 55개 통과 | 충족. 수정 원본을 순수 자체 소유로 바꾸지 않고 권리/미확인을 독립 기록 |

미확인 조건·변경본을 이번에 제외하거나 덮어쓰지 않기 때문에 그 취득/수정 의도를 추측해 정할 필수 선택은 없다(Step 8). 기존 참조 finding은 현재 보존 범위 비차단이며 실제 적용/복원 전 검증 대상으로 분류했다(Step 9). 따라서 현재 Phase 1의 필수 미확정 제외 경계·새 실패/미실행은 없다. 실제 도입/제외나 복원 단계의 필수 미확인을 없어진 것으로 처리한 것은 아니다.

#### 10-2. Phase 1 최종 판정과 문서 상태 반영 — 완료

**Step 10·Step 1~10 및 Roadmap 10 Phase 1을 완료 처리한다(2026-11-19).** 완료 범위는 조사·보존/제외 경계와 기록/검사 체계 준비다. Phase 2의 실제 규칙 적용/도입·복원 절차, Phase 3의 독립 복원과 Roadmap 10 전체는 미완료다.

영구 제품 기록은 Project의 ASSET_DEPENDENCIES.md, 빈 형식은 Templates, 갱신/보존 기준은 Implementation Rule로 유지한다. 실제 도입 항목은 0개다. 본 Task·Roadmap 10·공통 안내·Project Memory와 영구 기록의 현재 상태를 갱신하고 상세 실행 근거를 여러 곳에 복제하지 않는다. 이전 Step metadata의 `phase1Complete=false` 등은 당시 상태의 역사적 결과이므로 고쳐 쓰지 않는다. 현재 최종 판정은 이 Step과 Roadmap/Memory가 관리한다.

최신 검증 근거는 정책 포함 Unit Test 55개/exit 0과 Step 9의 actual CLI exit 1·collectionComplete=true/error 0개·기존 finding 32개다. 그 코드/입력 해시와 baseline 보존을 다시 확인했으며, 같은 Node suite/CLI·Unity 검증을 새로 반복한 결과로 기록하지 않는다. CLI exit 1은 참조 정상 PASS가 아니며 현재 관리 기준 준비 완료와 구분한다.

#### 10-3. Phase 2 실제 적용/도입·복원 절차 인계 — 완료

다음 작업은 **Phase 2 세부 실행 절차 작성**이다. 아직 그 Task/실행 Step은 만들거나 수행하지 않았다. Phase 1 산출물로 아래 구현/검증 대상을 구체화한다.

| 인계 대상 | Phase 2에서 할 일·보존/중단 조건 |
| --- | --- |
| 원본 유입/추적 검사 | 현재 신규 추적 해제 대상은 0개. 이미 무시된 TMP/없는 ThirdParty 폴더와 실제 제외 적용을 구분한다. 제품/구성요소 등록 경계에 기반한 신규 staged 원본 유입 검사를 마련하고, 기존 ignore 매칭만으로 추적 해제를 선언하지 않음 |
| 도입/원본/자체 자료 경로 | 외부 원본 보관·공식 재취득, Assets/ThirdParty 공급 자료와 Assets/ProjectAssets 자체 연결/변형이라는 Step 6 경로안을 실제 도입·수정/업데이트/제거 절차로 정리. 현재 폴더 이동/생성·Git index/ignore 변경 없음 |
| TMP 원본·변경본/설정 | uGUI 2.0.0 공급 archive/해시·원본 대비 TMP 82파일·meta/GUID/Import를 사용. 폰트 변경본 2개와 연결 설정/Material/atlas subasset·Resources/Shader 이름은 손실 없이 보존하거나 검증된 재적용을 준비. 원본 덮어쓰기/변경본 삭제로 간소화하지 않음 |
| 권리/Notice·영구 기록 | 실제 후보 선택 시 23필드·사용/수정/게임 배포/원본 재배포·고지 위치를 채움. EmojiOne/일부 NuGet·계정/Engine/service 조건 미확인은 해당 실제 도입/패키징/배포 전에 해소하거나 보류. 개인 증빙/비밀값·유료 원본 본문을 기록하지 않음 |
| 의존성/서버 선언 | manifest/lock/ProjectVersion·자체 설정/Scene/Prefab/Script/Material·csproj/EmbeddedResource/서버 소스 유지. npm tgz의 cache suffix/Windows 경로 이식성·NuGet feed/해결 graph·Node/container 조건과 재취득 제한을 실제 절차에 연결. install/restore·서비스/게시를 자동 실행하지 않음 |
| 참조/검증·기준선 | Step 1 입력 해시/HEAD·Step 9 증거와 Node 검사/정책 Test 명령을 인계. 보존 대상·필수 근거가 불명확하거나 실제 적용이 raw finding에 의존하면 최소 검증/사용자 결정을 먼저 준비하고 진행을 중단. 새 C#/Scene/자산 변화가 생길 때만 영향에 맞는 사용자 Unity 검증을 요구 |

실제 사용 목적·게임 내 적용 위치·복사/수정 내역은 제품별 등록 시 갱신한다. 임의 제품 도입/구매를 Phase 2 착수 조건으로 만들지 않으며, 현재 대상이 없더라도 반복 가능한 도입/업데이트·변경/제거·복원 절차와 검사는 완성해야 한다.

#### 10-4. Phase 3 독립 복원·후속 도입/운영 의무 인계 — 완료

Phase 3은 기존 작업본·cache·변경된 TMP/설정·사용자 저장/Secret·QA 후보를 보존하는 별도 환경에서 수행하도록 계획한다. 실제 별도 checkout/worktree·cache 제거/설치·원격 취득/빌드/Runner는 이번에 수행하지 않았다.

- 입력: 고정된 소스/선언·Editor 버전·공급 버전/해시·정당한 재취득 방법·meta/GUID/Import·변경본/연결 설정과 사용처. 기존 로컬 cache에만 의존하지 않는 준비가 필요하다.
- 검증: 원본 비추적/자체 자료 보존과 실제 필요한 자산/설정·Scene/Prefab/Material/Animation/이름 기반 참조·subasset/타입/Importer 해석을 연결한다. 필요한 Unity 실행은 사용자 담당이며 정확한 suite/적용표를 준비한다.
- 성공/중단: 필수 자료/참조·재적용·권리/취득 근거가 확인되고 필수 실행 실패/예상 밖 로그가 없어야 성공 판정한다. URP 11종·TMP graph texture 1종의 31회 raw 참조와 sample orphan은 실제 영향/필수 여부를 재평가한다. 단순히 기존 finding 수가 같다는 이유로 복원 PASS로 처리하거나 자동 allowlist/자료 삭제로 숨기지 않는다. 필요한 근거가 없거나 검증 실패면 중단/미완료로 남긴다.
- 미승계: 현재 cache/GUID/정적 해시·Node Test·기존 Roadmap 9 QA를 독립 복원/새 Unity 실행의 성공·후속 면제로 승계하지 않는다. 수정/복원 검증 계획과 결과는 해당 실제 입력에 묶는다.

Roadmap 11~14의 사운드/그래픽/UI 원본 도입 전에는 **Roadmap 10 전체 완료**가 필요하다. Pattern/난이도·위험 보상 개선은 계속 범위 밖이다. 선행 Roadmap 9 QA 후보의 원격/로그 미검증 한계와 다음 실제 Module 업데이트 전 source/artifact/hash·환경/reference·새 QA 보존 및 별도 승인 범위의 실제 복구 시험 의무도 유지한다. 이 Task는 그 원격 복구·구매/공개·원본/라이선스 삭제·Git 이력 재작성을 승인/수행하지 않는다.

**현재 사용자 수동 작업 없음.** 추가 Unity Compile/Build/Test Runner/Player·Scene/Prefab 적용이나 원본 이동을 요청하지 않는다. Phase 1만 완료했으며 다음은 Phase 2 실행 절차 준비다.

# 영향 범위

Step 1~10 변경은 본 Task·기존/Step 7~9 metadata JSON·Roadmap 10/공통 안내/Project Memory·영구 관리 기록/빈 형식·Implementation Rule·Project/Templates 안내와 Tools/DependencyAudit의 도구/Test/README다. 착수 작업 트리는 깨끗했으며 기존 Roadmap 9 완료 근거를 보존한다. 생산 C#/Unity Test·ignore/index·외부 원본/meta·Scene/Prefab·패키지·실제 저장/서버·후보 파일은 바꾸지 않는다. Step별 실제 실행 후 변경/이유·결과를 해당 Step에 기록한다.

# 검증 내용

- Step 1~10 순서·담당/사용자 작업과 각 완료 조건이 Phase 1의 세 완료 조건에 연결되는지 대조한다.
- Phase 1 조사/판정과 Phase 2 실제 추적 제외·Phase 3 독립 복원 검증을 구분한다.
- 정적 검사와 실제 판정 함수 Unit Test의 정상/오류·오프라인/읽기 전용 경계, 조건부 Unity Test 범위를 확인한다.
- 실제 확인 자료/예정 경로를 구분하고 기존 경로/상대 링크·일자/파일 번호·체크 상태/인코딩·공백을 검사한다.

# 검증 결과

문서 작성에 필요한 기존 경로/규칙·Roadmap 10의 Phase별 책임과 일부 실제 자료를 읽기 전용으로 확인했다. 문서 4개(본 Task·Roadmap 10·공통 안내·Project Memory)의 상대 링크 64개, Step 1~10 순서·완료 체크 0개/미체크 10개, 템플릿 섹션·예정 도구/Test 3경로와 실제 입력 13경로, 같은 날짜 Task 번호 01/02의 중복 없음·인코딩을 정적으로 검사했다. 현재 lock의 registry 20개/builtin 41개와 실행 자산/Tools·ignore 변경 없음도 대조했다. 변경된 기존 문서의 `git -c core.safecrlf=false diff --check`와 신규 Task의 공백/인코딩 검사도 통과했다.

이는 실행 절차 문서 작성의 검증 결과다. Phase 1 인벤토리 전수 조사·경계 확정·도구/Unit Test 구현/실행·Unity/빌드·추적 해제/복원·원격은 미실행이며 기존 Roadmap 9의 완료 근거/변경 문서를 보존했다.

Step 1 검증: 읽기 전용 Node 검사로 입력 66개 해시·선행 6개 입력 일치·HEAD/index/추적 경로 목록·후보 해시 보존, 변경 문서 4개·상대 링크 65개·Step 1~10 순서·완료 1개/미완료 9개·하위 결과 1-1~1-3·인코딩을 확인했다. `git -c core.safecrlf=false diff --check`도 통과했다. 신규 파일은 metadata JSON 1개이며 실행 자산 변경은 없다. 초기 문서 편집 스크립트는 문자열 구문 오류로 실행되지 않았고 패치 적용 후 위 검사로 확인했다. Unity/빌드/Runner·Player·원격 실행은 없다.

Step 2 최종 정적 검증: 인벤토리 877행의 필수 열·경로 중복 없음, 5개 루트 파일 822개와 독립 rg 목록 일치·무시 파일 85개와 Git 목록 일치, 문서 4개/상대 링크 66개·Step 순서/하위 결과·완료 2개/미완료 8개·JSON/문서 인코딩과 공백을 확인했다. Step 1 입력 66개·index·후보 해시 보존과 생산 자산 변경 0개·staged 변경 0개를 재확인했고 `git -c core.safecrlf=false diff --check`도 통과했다. 기존 문서 변경/기준선 JSON을 보존했으며 신규 결과는 Step 2 metadata JSON 1개다. 이는 정적 인벤토리 검증이며 Unit Test/Unity/독립 복원 PASS가 아니다.

Step 3 최종 정적 검증: UPM 61개·직접 43개·관계 91개의 원본 선언/metadata 연결과 누락/순환 0개, npm root 선언 일치·local archive 버전 일치·필수 lock 연결 65개를 확인했다. 기준 입력 66개·이번 입력 9개·cache package.json 61개·TMP archive 2개·npm archive 27개 해시, HEAD/index/후보 보존·생산 자산/staged 변경 0개를 확인했다. 문서 4개·로컬 링크 67개·Step 순서/3-1~3-4·완료 3개/미완료 7개·인코딩/JSON 공백과 `git -c core.safecrlf=false diff --check`를 통과했다. 신규 결과는 의존성 metadata JSON 1개이며 기존 결과/문서 변경을 보존했다. 반복 도구의 실패 입력 Unit Test는 Step 7에 남겼으며 이번 검사를 Unity resolver/설치/독립 복원 성공으로 기록하지 않는다.

Step 4 최종 정적 검증: 독립 rg 검색의 meta 11,816개와 수집 개수가 일치하고 보존 map 554개 GUID/해시·직렬화 56파일/해시·TMP 82파일 비교를 재확인했다. fileID anchor 354회·Importer 참조 5개·asmdef 명시적 참조는 해당 정적 검사 범위에서 연결됐다. 미해결 YAML 11종/15회와 ShaderGraph texture 1종/16회는 여전히 미해결이며 전체 참조 PASS로 표시하지 않았다. 문서 4개·로컬 링크 68개·Step 순서/4-1~4-4·완료 4개/미완료 6개·인코딩/JSON 공백과 `git -c core.safecrlf=false diff --check`를 통과했다. 기준 입력 66개·HEAD/index/후보 해시 보존·생산 자산/staged 변경 0개를 확인했다. 초기 수집은 package asmdef의 UTF-8 BOM 파싱 오류로 중단됐고 BOM 처리 후 재실행했다. Importer 참조의 최초 부분 map 조회 실패도 전체 map 근거로 보완해 재검증했으며 실제 누락으로 오기록하지 않았다. 임시 조사 helper는 제거했고 신규 보존 결과는 참조 metadata JSON 1개다. 영구 검사 도구/실패 입력 Unit Test는 Step 7, 필요한 실제 Unity 해석은 Step 9에 남긴다.

Step 5 최종 정적 검증(2026-11-19): 기준 입력 66개·기존 결과/안내 입력 5개·LICENSE/Notice 41개·TMP 82파일·npm archive 27개 해시를 확인했다. UPM 61개·원본 동일 80/차이 2개·Unlicense 표시 27개와 EmojiOne/Cloud Code NuGet 조건 미확인 상태가 metadata와 일치한다. 문서 4개·로컬 링크 69개·Step 순서/5-1~5-3·완료 5개/미완료 5개·JSON/인코딩/공백과 `git -c core.safecrlf=false diff --check`를 통과했다. HEAD/index/후보 해시 보존·생산 자산/staged 변경 0개를 확인했고 기존 문서/결과를 보존했다. 신규 결과는 20261119_01_Phase1Step5Provenance.json 1개이며 같은 날짜 번호 중복은 없다. 공식 웹 확인은 조건/버전 metadata 조회로 한정했고 package 다운로드/원본 import·Unity/Build/Runner·서비스/배포는 실행하지 않았다. NuGet license 본문 fetch 실패는 미확인으로 남겼다. 이 검사는 권리/배포 승인이나 Unit Test/독립 복원 PASS가 아니다.

Step 6 최종 정적 검증(2026-11-19): 관련 문서 9개·로컬 링크 83개·등록 필드 23개·독립 상태 축 4개·실제 도입 항목 0개, Step 순서/6-1~6-3·완료 6개/미완료 4개와 인코딩/공백을 확인했다. 기준 입력 66개·Step 5 근거 입력 5개·TMP 82파일 해시, HEAD/index/후보 보존·생산 자산/staged 변경 0개를 확인했고 `git -c core.safecrlf=false diff --check`도 통과했다. 영구 기록/빈 형식 2문서를 새로 만들고 기존 Implementation Rule·Project/Templates 안내·Task/Roadmap/Memory를 갱신했다. 신규 검사 도구/Unit Test·machine-readable 정책 파일은 이 Step에서 만들지 않았으며 기존 metadata를 보존했다. 경계안은 실제 제외/권리/복원 승인·성공이 아니고 보류 조건은 Step 8/9에 남아 있다. Unity/Build/Runner·원본 이동/재도입·Scene/Prefab 수정은 없다.

Step 7 최종 정적 검증(2026-11-19): 문서 10개·로컬 링크 90개·Step 순서/7-1~7-4·완료 7개/미완료 3개·UTF-8/공백과 Node syntax check 2파일을 확인했다. 도구 본체에 파일 write/삭제/설치·임의 실행 API가 없고 process spawn은 고정 Git read adapter 1곳임을 확인했다. 기준 입력 66개·Step 5 근거 입력 5개·TMP 82파일 해시, HEAD/index/후보 보존·생산 자산/staged 변경 0개를 재확인했고 `git -c core.safecrlf=false diff --check`도 통과했다. Unit Test 54개 exit 0과 inventory의 collectionComplete=true/exit 1·finding 32개를 구분했다. 기존 미커밋 문서/metadata는 보존하고 새 결과 JSON 1개·Tools 파일 3개를 추가했다. source SHA-256은 도구 `6cc8dc74f38a7a2a69f9245e23409c6541e8dad28fe02800e09734e99ee911ce`, Test `ca640395fc4d2e020e9d36529c2e84be6d45fa8e239cb139a654d196a1b06da4`다. Unity Compile/Build/Runner·Scene/Prefab·실제 서버/저장·원격 실행은 없다.

Step 8 최종 정적 검증(2026-11-19): 문서 10개·로컬 링크 91개·Step 순서/8-1~8-2·완료 8개/미완료 2개·정책 JSON 5그룹과 사용자 답변/새 제외 후보/변경 요청 빈 목록·권리/Phase 1 미완료 상태를 확인했다. 기준 입력 66개·정책 근거 4개·TMP 82파일 해시와 HEAD/index/후보 보존·생산 자산/staged 변경 0개를 확인했다. 정책 포함 Test 55개 통과/exit 0, Test syntax·UTF-8/공백과 `git -c core.safecrlf=false diff --check`도 통과했다. 수집기 SHA-256은 Step 7과 동일하고 Test는 선택적 정책 입력 검증을 추가한 `d751128c39c4730c23f484df413380a9facaf5f570baa0764c4384d64bbc068c`다. 신규 결과는 정책 metadata JSON 1개이며 기존 조사/검사 결과를 보존했다. Step 7의 기존 finding은 변경하지 않았고 관련 자산 변경이 없어 전체 inventory/Unity 검사를 불필요하게 반복하지 않았다. 기존 참조 finding의 필수 여부·영향은 Step 9에 남기며 이번 보존 처리를 사용 권한/자동 제외/독립 복원 승인으로 바꾸지 않는다.

Step 9 최종 정적 검증(2026-11-19): 최신 inventory CLI의 graph/참조/diagnostic이 Step 7과 같고 수집 error 0·기존 finding 32개·exit 1임을 확인했다. 현재 보존 범위의 영향·Unity 필요성/후속 중단 조건을 5그룹으로 기록했다. 기준 입력 66개·검토 근거 20개·정책 근거 4개·TMP 82파일 해시와 HEAD/index/후보 보존·생산 자산 변경/비무시 신규 파일·staged 변경 0개를 확인했다. Node 수집기/Test·정책 입력 해시는 Step 8 검증 시점과 같아 55개 통과 근거를 재사용했고 이번에 같은 suite를 다시 실행했다고 기록하지 않았다. 문서 10개·로컬 링크 92개·Step 순서/9-1~9-4·완료 9개/미완료 1개·UTF-8/공백과 `git -c core.safecrlf=false diff --check`도 통과했다. 신규 결과는 20261119_04_Phase1Step9Review.json 1개이며 기존 결과/문서를 보존했다. C# Test 미생성·Unity/Build/Runner·Scene/Prefab 적용 없음과 참조 정상/독립 복원 미검증을 유지한다. Phase 1 완료 조건의 최종 판정은 Step 10에 남긴다.

Step 10 최종 정적 검증(2026-11-19): 완료 조건 3개를 실제 산출물/보존 경계에 연결했고 문서 10개·로컬 링크 93개·Step 1~10 순서/10-1~10-4·완료 10개/미완료 0개·Roadmap Phase 3개/하위 상태 구조·등록 항목 0개와 Phase 2/3 미착수 상태를 확인했다. 기준 입력 66개·Step 9 근거 20개·정책 근거 4개·TMP 82파일 해시, HEAD/index/후보 보존·생산 자산 변경/비무시 신규 파일·staged 변경 0개를 확인했다. Node 도구/Test·정책 입력은 기존 55개 통과 및 Step 9 CLI 결과와 같은 해시여서 재실행하지 않았다. 문서/인코딩·공백과 `git -c core.safecrlf=false diff --check`를 통과했다. 최초 문서 검사 호출은 PowerShell pipe의 한글 정규식 인코딩 구문 오류로 실행되지 않았고 ASCII 검사로 정정한 재실행이 통과했다. 현재 변경은 완료/인계 문서뿐이며 기존 metadata의 당시 미완료 상태를 고쳐 쓰지 않았다. Phase 1만 완료, 실제 적용/독립 복원·권리/참조 정상 및 Roadmap 전체는 미완료라는 경계를 유지한다.

# 후속 작업

**Phase 1 완료. [Phase 2 Task](20261119_05_Phase2ManualSteps.md) 절차 문서를 작성했으며 다음은 Step 1 기준선/허용 변경 범위 확정이다.** Step 10-3/10-4 인계를 실제 도입/업데이트/제거·복원 절차로 구체화한다. Phase 2/3 실제 실행은 미착수이며 Roadmap 10 전체 완료가 아니다. 현재 사용자 수동 작업은 없다.

# 관련 문서

- [AI 문서 진입점](../../README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [Project Overview](../../00_Project/PROJECT_OVERVIEW.md)
- [Architecture](../../00_Project/ARCHITECTURE.md)
- [Roadmap 10](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_010.md)
- [공통 순서/승인 경계](../../04_Implementation_Roadmap/README.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [Investigation Rule](../../01_Rules/INVESTIGATION_RULE.md)
- [Implementation Rule](../../01_Rules/IMPLEMENTATION_RULE.md)
- [Verification Rule](../../01_Rules/VERIFICATION_RULE.md)
- [Coding Style](../../01_Rules/CODING_STYLE.md)
- [General Task Template](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
- [Tasks 안내](../README.md)

# 관련 작업 기록

- [Roadmap 9 완료·최신 후보/한계·Roadmap 10 인계](../Prototype_9/20261118_01_Phase4ManualSteps.md)
- [과거 자산/Scene 감사 기록](../Prototype_3/20260907_06_Phase2Step9AssetSceneAudit.md)

# 작성 완료 기준

- 실제 수행할 Step·입력/결과·정적/Unit Test·필요 사용자 작업·완료 조건을 명시했다.
- 확인한 사실·미확인 자료·계획한 도구/Test를 구분하고 민감/유료 원본을 새로 기록하지 않았다.
- 완료한 Step의 실제 조사/산출물과 남은 판정/검증을 구분하고 Phase 1/전체 Roadmap·추적 해제/독립 복원 성공을 미리 기록하지 않는다.
