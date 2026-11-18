# 작업 정보

## 작업명

Prototype 10 Phase 1 — Thirdparty 원본·프로젝트 소유 자료 경계 확정

## 작업 일자

20261118

## 작업 담당자

- AI: 기준선/추적·의존성·참조/출처 조사, 분류표·검사 도구/Unit Test 준비, 정적/오프라인 검사, 사용자 확인 대상 최소화, 결과/다음 Phase 인계.
- 사용자: 로컬 자료만으로 확인할 수 없는 취득/사용 조건·프로젝트 소유 변경의 결정, 필요한 경우에만 Unity Compile/Test Runner 실행과 Scene/Prefab 적용.

## 작업 상태

**실행 절차 작성 완료. Step 1~10은 미착수이며 Phase 1/Roadmap 10은 미완료다.** 이번에는 문서 작성에 필요한 일부 파일/목록을 읽기 전용으로 확인했다. 전수 조사·경계 확정·Unit Test 구현/실행·현재 추적 해제·복원 성공으로 기록하지 않는다.

# 작업 목적

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

- [ ] 기준선·보존 대상·읽기/출력 범위와 Phase 1/후속 Phase 경계를 기록했다.

## Step 2. 추적/실제 파일/ignore 인벤토리를 만든다

### AI 작업

2-1. `git ls-files -z`, `git diff --cached --name-status`, `git ls-files --others --exclude-standard`와 제한된 실제 파일 열거를 대조한다. Assets/ThirdParty·TextMesh Pro뿐 아니라 Plugins/샘플/폰트/Shader·이름이 바뀐 외부 자료 후보와 폴더 meta를 찾는다.

2-2. 무시된 대상은 `rg --files --hidden --no-ignore` 등을 **정한 경로에서만** 사용한다. `.gitignore`의 문구뿐 아니라 `git check-ignore -v --no-index -- <대상>`으로 규칙의 적용 위치를 확인한다. 출력/버전/exit code 의미를 구분하며 추적 파일은 ignore에 매칭돼도 계속 추적된다는 점을 별도 표기한다.

2-3. 항목마다 `경로/자료 종류/실제 존재/HEAD 추적/index 상태/ignore 규칙/잠정 출처/관련 meta/조사 근거/미확인`을 기록한다. 폴더 없음·외부 원본 없음·추적 없음·규칙 매칭을 서로 다른 결과로 남긴다. 대소문자/공백/한글 경로를 보존한다.

### 완료 조건

- [ ] 정한 범위의 실제 파일·추적/index·무시 자료가 연결되고 누락/범위 밖 항목이 명시됐다.

## Step 3. 의존성 선언과 재취득 경로를 연결한다

### AI 작업

3-1. manifest의 직접 의존성·scoped registry/local/git 등 출처 선언과 lock의 해결 버전/전이 관계/source를 실제 JSON으로 읽는다. 현재는 registry/builtin이지만 다른 유형이 발견되면 별도 판정한다. 형식 오류/누락/순환·해결 불일치를 성공으로 숨기지 않는다.

3-2. package.json/캐시 metadata·ProjectVersion을 대조해 `제품/패키지 ID/요청 버전/해결 버전/source/공식 취득 위치/필요 Editor/재도입 방식/근거/확인 상태`를 만든다. Library cache의 이름/존재는 참고로만 쓰고 외부 URL을 임의로 만들어 쓰지 않는다.

3-3. TMP Essential Resources 등 패키지 밖 Assets로 가져온 자료와 UPM package 원본을 연결한다. 원본 archive가 있다는 것만으로 프로젝트 설정/파생물·GUID 재현이 보장된다고 하지 않는다.

3-4. UGS csproj/EmbeddedResource·LogSink package.json 등 보조 의존성 선언은 별도 행으로 기록한다. Unity 외부 원본 제외 때문에 프로젝트 소유 빌드 선언/서버 소스를 일괄 제거하지 않는다. 네트워크 restore/install·CLI 인증/게시를 수행하지 않는다.

### 완료 조건

- [ ] 직접/전이 의존성·복사 도입 자료·보조 선언의 역할/버전/취득 근거와 보존 필요성이 정리됐다.

## Step 4. 참조와 meta/Import 영향 범위를 조사한다

### AI 작업

4-1. 파일/폴더 meta의 GUID→경로 map을 만들고 프로젝트 소유 Scene/Prefab/Material/설정·asmdef·외부 자료의 GUID/fileID 참조를 대조한다. Assets/Packages/PackageCache의 검색 ignore 누락과 Unity builtin GUID를 구분한다. 중복/잘못된 GUID·missing/orphan meta는 오류/미해결로 표시한다.

4-2. TMP Settings 기본 폰트/Material·fallback/sprite/line-breaking 설정과 Shader 참조를 실제 대상까지 연결한다. 현재 cache로 해결되는 GUID와 Git checkout만으로 제공되는 GUID를 나누고 원본/폴더 meta의 제외 시 영향·동일 GUID 재취득 근거를 기록한다.

4-3. Resources.Load/Shader.Find·경로/이름 문자열, reflection/asmdef/DLL·런타임 생성/직접 코드 의존성을 조사한다. 텍스트 GUID가 없다는 이유로 미사용이라고 하지 않는다. binary/동적 참조로 정적 판정이 제한되면 대상/한계와 Step 9에서 필요한 자동 확인을 지정한다.

4-4. `.meta`의 importer/platform 설정과 프로젝트 변경 여부를 비교 가능한 원본 metadata가 있을 때만 대조한다. 원본 비교가 불가능한 파일은 변경 없음으로 단정하지 않는다. Scene/Prefab을 바꾸지 않고 필요한 후속 적용/복원 검증 항목만 기록한다.

### 완료 조건

- [ ] 제외 후보→참조 사용처·meta/Import/GUID 보존/재현 요구와 정적 조사 한계가 연결됐다.

## Step 5. 원본/변경/파생·취득/사용 조건의 근거를 정리한다

### AI 작업

5-1. 제품/패키지 metadata, 기존 도입 기록, LICENSE/NOTICE/Attribution과 공식 배포 정보를 확인한다. 원본 본문을 새로 복사하지 않고 문서 경로/필요한 조건 요약·근거 위치·확인 상태를 기록한다. 라이선스 파일의 존재만으로 해당 사용자의 구매/취득·재배포 권한을 증명하지 않는다.

5-2. 비교 가능한 원본/버전과 hash·meta를 대조해 `동일 원본/프로젝트 수정 원본/새 파생물/프로젝트 소유 연결 설정/미확인`을 나눈다. TMP Settings/Material/폰트 atlas·preset 등은 이름이 외부 폴더 아래 있다는 이유로 버리지 않는다. 변경 원본의 별도 확장/설정 재현 필요성은 Phase 2 설계 항목으로 기록한다.

5-3. 재취득에 필요한 제품/버전/공식 위치·도입 담당·오프라인/계정 제한·증거 없는 항목을 표시한다. 개인 계정/영수증/키·비밀 URL은 문서화하지 않는다. 실제 원본 다운로드·유료 구매/계약·법적 권한의 추정 판정은 하지 않는다.

### 완료 조건

- [ ] 출처/변경/파생 근거와 취득·재취득/사용 조건의 확인/미확인 항목이 분리됐다.

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

6-3. 영구 의존성 register가 별도 책임으로 필요한지 기존 Project 문서와 대조한다. Phase 1 조사/판정 이력은 본 Task에 기록하고 중복 register를 여러 곳에 만들지 않는다. machine-readable 결과가 필요하면 metadata만 포함하고 실제 원본/비밀값은 넣지 않는다.

### 완료 조건

- [ ] 원본/파생/프로젝트 소유·meta·의존성 선언의 유지/제외 경계안과 근거/보류 조건이 작성됐다.

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

- [ ] 재사용/최소 도구와 실제 판정 함수의 Unit Test가 준비·검증됐고 실패/누락을 감추지 않는 inventory 검사 결과가 기록됐다.

## Step 8. 필요한 사용자 정보/정책만 확정한다

### AI 선행 작업

8-1. 앞선 근거로 해결하지 못한 **실제 경계 결정을 막는 항목만** 모은다. `대상/확인한 사실/부족한 정보/가능한 보존 방향/후속 영향`을 준비하고 한 번에 이해할 수 있게 제시한다. 로컬 파일/출처 문서/Git으로 확인 가능한 사항을 사용자에게 다시 조사하게 하지 않는다.

### 필요한 경우의 사용자 작업

- 로컬/공식 자료에서 확인할 수 없는 실제 취득 경로/버전·재취득 가능 여부·담당 또는 프로젝트가 의도적으로 수정한 외부 원본의 유지 방향만 확인한다. 비밀값/계정 화면/영수증 원문은 보내지 않는다.
- 권한/출처 근거가 없거나 경계가 불명확하면 보존/보류 방향과 후속에서 확정할 대상을 결정한다. 현재 추적 해제/삭제를 수행하는 승인이 아니다.

### AI 결과 반영

8-2. 답변/근거·적용 범위를 분류표와 Unit Test의 정책 입력에 반영한다. 사용자 답변이 필요한 필수 항목은 기다리고, 답변 없음/시간 경과를 승인으로 처리하지 않는다. 필수 제외 경계를 확정하지 못하면 해당 범위는 미해결로 남겨 Phase 1 완료를 막는다. 비차단 후속 의무는 적용 대상/제한을 명시해 인계한다.

### 완료 조건

- [ ] 필요한 사용자 결정이 반영됐거나 사용자 작업 불필요 근거가 있고, 필수 미확정 경계를 완료로 표시하지 않았다.

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

- [ ] 최종 inventory/분류·정적/Unit Test 근거와 조건부 Unity 필요성/실제 결과가 연결되고 필수 실패/미실행이 없다.

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

- [ ] Phase 1의 실제 완료/미완료 판정과 Phase 2/3의 실행 대상·보존/검증·미확인 의무를 정확히 인계했다.

# 영향 범위

현재 변경은 이 실행 절차 문서 작성과 Roadmap 10/공통 안내/Project Memory의 다음 작업 링크다. 기존 Roadmap 9 완료 문서 변경을 보존한다. 생산 코드/Test/Tools 도구·ignore/index·외부 원본/meta·Scene/Prefab·패키지·실제 저장/서버·후보 파일은 바꾸지 않는다. Step별 실제 실행 후 변경/이유·결과를 해당 Step에 기록한다.

# 검증 내용

- Step 1~10 순서·담당/사용자 작업과 각 완료 조건이 Phase 1의 세 완료 조건에 연결되는지 대조한다.
- Phase 1 조사/판정과 Phase 2 실제 추적 제외·Phase 3 독립 복원 검증을 구분한다.
- 정적 검사와 실제 판정 함수 Unit Test의 정상/오류·오프라인/읽기 전용 경계, 조건부 Unity Test 범위를 확인한다.
- 실제 확인 자료/예정 경로를 구분하고 기존 경로/상대 링크·일자/파일 번호·체크 상태/인코딩·공백을 검사한다.

# 검증 결과

문서 작성에 필요한 기존 경로/규칙·Roadmap 10의 Phase별 책임과 일부 실제 자료를 읽기 전용으로 확인했다. 문서 4개(본 Task·Roadmap 10·공통 안내·Project Memory)의 상대 링크 64개, Step 1~10 순서·완료 체크 0개/미체크 10개, 템플릿 섹션·예정 도구/Test 3경로와 실제 입력 13경로, 같은 날짜 Task 번호 01/02의 중복 없음·인코딩을 정적으로 검사했다. 현재 lock의 registry 20개/builtin 41개와 실행 자산/Tools·ignore 변경 없음도 대조했다. 변경된 기존 문서의 `git -c core.safecrlf=false diff --check`와 신규 Task의 공백/인코딩 검사도 통과했다.

이는 실행 절차 문서 작성의 검증 결과다. Phase 1 인벤토리 전수 조사·경계 확정·도구/Unit Test 구현/실행·Unity/빌드·추적 해제/복원·원격은 미실행이며 기존 Roadmap 9의 완료 근거/변경 문서를 보존했다.

# 후속 작업

**Step 1 기준선/범위 확정부터 시작한다.** 현재 Step 1~10은 미착수다. 문서 작성만으로 Phase 1을 완료하거나 실제 제외/Unity/복원 실행을 요청하지 않는다. 지금 사용자 수동 작업은 없다.

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
- 문서 준비만 완료했으며 Step 실행/분류 확정·Phase 1/전체 Roadmap 완료·현재 추적 해제/독립 복원 성공을 미리 기록하지 않았다.
