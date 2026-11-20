# 목적

프로젝트에서 구현 작업을 수행할 때 적용하는 규칙을 정의한다.

프로젝트 전체에서 동일하게 적용되는 구현 기준을 정의한다.

구현 품질과 구조 일관성을 유지하기 위한 기준을 정의한다.

---

# 적용 범위

이 문서의 규칙은 프로젝트의 모든 구현 작업에 적용한다.

예외가 필요한 경우 별도의 Rule 문서에서 정의한다.

---

# 구현 원칙

구현하기 전에 관련 Project, Rules, System, Feature 문서를 확인한다.

문서에 정의된 내용을 기준으로 구현한다.

확인되지 않은 내용을 추측하여 구현하지 않는다.

구현 전에 동일한 책임을 가진 기존 System 또는 Feature가 존재하는지 확인한다.

구현 대상의 책임 범위를 벗어나지 않는다.

구현은 현재 프로젝트 규모에 적합한 단순한 구조를 우선한다.

문서에 정의되지 않은 구조는 구현과 함께 관련 문서에도 정의한다.

기존 기능의 동작을 변경하는 경우 관련 문서를 함께 수정한다.

구현 완료 시 문서와 구현이 서로 모순되지 않는 상태를 유지한다.

---

# 구현 범위 결정 기준

작업 요청에 포함된 범위만 구현한다.

기존 구조를 수정하여 해결 가능한 경우 새로운 구조를 추가하지 않는다.

새로운 구조가 필요한 경우 기존 구조로 해결할 수 없는 근거가 있어야 한다.

작업 범위와 직접 관련 없는 코드는 수정하지 않는다.

구현 범위는 회귀 영향을 최소화하는 방향으로 결정한다.

---

# 기존 구조 재사용 기준

기존 구조를 우선 재사용한다.

기존 구조를 확장하여 해결 가능한 경우 확장을 우선한다.

동일한 목적의 구조를 새로 생성하지 않는다.

기존 구조의 책임을 유지할 수 없는 경우에만 새로운 구조를 생성한다.

---

# 구조 변경 기준

구조 변경은 기존 구조로 해결할 수 없는 경우에만 수행한다.

구조 분리는 책임이 명확하게 분리되는 경우에만 수행한다.

구조 통합은 동일한 책임을 가지는 경우에만 수행한다.

기존 구조의 책임을 변경하는 경우 관련 문서를 함께 수정한다.

작업 요청과 관계없는 구조 변경은 수행하지 않는다.

---

# 의존성 관리 기준

참조 방향은 기존 구조를 유지한다.

순환 참조를 만들지 않는다.

불필요한 의존성을 추가하지 않는다.

직접 참조가 필요한 경우에만 직접 참조를 사용한다.

이벤트는 `EVENT_RULE.md`의 기준에 따라 사용한다.

결합도를 증가시키는 구조보다 책임을 분리하는 구조를 우선한다.

---

# 수정 범위 관리 기준

수정 대상과 직접 관련된 코드만 수정한다.

기존 기능의 동작을 변경하는 경우 변경 영향을 확인한다.

회귀 가능성이 있는 경우 관련 기능도 함께 검토한다.

작업 요청 범위를 벗어나는 리팩토링은 수행하지 않는다.

관련 문서와 구현 내용의 일관성을 유지한다.

---

# 외부 애셋 관리 기준

향후 도입 애셋의 기록/경계 설계 기준이다. 실제 ignore/index·원본/폴더/meta 이동·재도입 절차 적용은 Roadmap 10 Phase 2, 독립 복원은 Phase 3에서 검증한다.

## 기록과 갱신

- 단일 관리 위치는 [애셋·의존성 관리 기록](../00_Project/ASSET_DEPENDENCIES.md)이며 [등록 형식](../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)을 사용한다. 프로젝트 소유자는 도입/정책 결정, AI는 확인 가능한 metadata·검사 준비를 담당한다.
- 후보 선택 시 ID·공식 출처/버전·예정 목적을 등록한다. 복사/도입 전에 공급 조건·재취득 방법·Git/meta 경계를 확인하고, 첫 게임 적용 때 실제 사용처/직접·동적 참조를 갱신한다.
- 원본 수정/파생·Importer 변경·업데이트/재도입·경로/사용처 변경 시 같은 ID의 기록을 갱신한다. 제거 후에도 폐기 상태와 참조/Attribution·재복원 의무 처리 근거를 남긴다. 배포/복원 전에는 해당 버전의 조건과 검증 상태를 재확인한다.
- `미확인: 이유·필요 근거·다음 시점`, `해당 없음: 이유`를 구분한다. 답변/실행 결과가 없으면 확인/성공으로 바꾸지 않는다. 개인 취득값·영수증/계정/키·비밀 URL·원본 payload는 기록하지 않는다.

## 독립 상태

실제 등록부/경계 정책의 기계 문법은 등록 형식의 자동 검사 계약을 따른다. Phase 2 Step 2의 registry 검사는 각 상태의 근거·목록/본문/정책 연결을 확인하고 실제 제품 0개도 허용한다. 근거가 부족하면 보존/보류하며 새 정책만으로 도입·권리·경계 적용·독립 복원을 연쇄 승인하지 않는다. 버전/source/license/hash가 달라지면 이전 등록부를 명시한 transition 검사로 기존 상태/결정 근거의 재사용을 거절한다. snapshot 검사만으로 과거 상태 비교·실제 Git 원본 유입 차단·권리/복원 성공을 선언하지 않는다.

| 축 | 상태와 전환 근거 |
| --- | --- |
| adoption | 후보 / 보류 / 도입 / 폐기. 후보 등록은 구매/도입 승인이나 실제 적용이 아님 |
| rights | 미확인 / 근거 확인 / 제한·차단. 버전별 사용/수정/게임 배포·원본 재배포 조건·Attribution을 구분하고 license 이름만으로 권한을 확정하지 않음 |
| boundary | 초안 / 판정 대기 / 확정 / 적용. 출처/변경/meta 보존 근거와 필요한 사용자 결정으로 확정, 실제 Git/복원 절차 반영을 별도 적용으로 기록 |
| restore | 미검증 / 정적 확인 / 독립 복원 확인. cache/GUID 해석·Unit Test만으로 독립 복원으로 전환하지 않음 |

## 원본·프로젝트 소유 자료 경계

- 원본은 Git 비추적 보관/재취득을 기본 방향으로 한다. 향후 도입 경로안은 `Assets/ThirdParty/<공급자>/<제품>/`, 자체 변형/연결 자료는 `Assets/ProjectAssets/<asset_id>/`다. 현재 폴더를 생성/이동하지 않으며 Phase 2에서 경로/규칙을 검증한다. 버전은 기록으로 구분하고 같은 GUID의 여러 버전을 동시에 넣지 않는다.
- 경로/파일명/ignore만으로 원본 소유·제외를 확정하지 않는다. 혼합/변경/파생·조건 불명 자료는 보존/판정 대기다. 수정 원본을 순수 자체 소유로 재분류하지 않으며 자체 변형은 원본과 분리해 출처·변경 이유·사용처를 연결한다.
- 자산/폴더 meta는 각각 판단한다. 동일 버전/GUID·Import/platform 설정을 재취득/재현할 근거가 있으면 원본 meta를 비추적 후보로 둘 수 있다. 프로젝트 생성 GUID/Import 변경·연결 설정은 보존 또는 검증된 재적용 정보가 필요하다. 근거 없이 meta 전체를 제외하거나 새 파생물에 원본 GUID를 복제하지 않는다.
- manifest/lock·ProjectSettings·자체 Script/Scene/Prefab/Material·서버 빌드/EmbeddedResource 선언과 연결 설정은 유지한다. 필요한 license/Attribution도 배포 시 보존/재취득할 수 있어야 한다. 이 의무를 유료 원본/허용 불명 자료의 Git 복제 근거로 삼지 않는다.
- 참조/조건/변경 보존을 확정하지 못한 자료는 자동 제외/폐기하지 않는다. 추적 해제와 이력 재작성, 사용처 제거와 자료 삭제, 기록과 구매/배포 승인을 구분한다.

## 외부 애셋 도입·활용 실행 절차

Roadmap 10 Phase 2 Step 3에서 확정한 절차다. 현재 실제 후보는 0개이므로 아래 절차를 위해 빈 폴더/Scene·제품 항목을 만들지 않는다. AI는 metadata·해시/경로·참조 정적 검사와 적용표를 준비하고, 소유자는 실제 취득/권리 결정과 필요한 Unity import·Scene/Prefab 적용/실행을 담당한다. 원본 보존 복사 기능은 Step 4, 현재 Git 유입 차단 gate는 Step 5, 이식 복원 입력은 Step 6의 구현/검증 이후에만 사용한다.

| 순서 | 실행/담당 | 기록·통과 조건과 중단 시 처리 |
| --- | --- | --- |
| 1. 후보 등록 | 소유자가 후보/공식 공급자를 정하고 AI가 관리 기록의 영구 ID·종류·예정 목적을 작성 | 현재 version/source와 예정 사용처를 구분한다. 미확인은 사유·필요 근거·다음 시점을 기록하며 구매/도입 상태로 자동 전환하지 않는다 |
| 2. 공급 조건 확인 | 소유자가 해당 버전의 사용·수정·게임 배포/원본 재배포·Attribution 조건과 취득/재취득 제한을 확인, AI가 공개 근거·archive SHA-256을 대조 | 공급자가 부여한 조건을 확인하며 프로젝트가 임의 license를 채택/재부여하지 않는다. version/source/hash·조건 불명/상충이면 도입을 보류한다 |
| 3. 원본 보관 | 명시한 입력/출력의 보존 사전 점검 후 취득 배포물과 필요한 선택 원본을 Assets 밖의 비추적 보관 위치에 둠 | `Ignore/DependencySources/<asset_id>/<archive_sha256>/`를 도입용 보관 경로로 사용한다. archive와 선택 파일/meta의 해시·출처 관계, 실제 ignore/비추적·junction/충돌을 확인한다. 기존 보관본 덮어쓰기·이동/삭제 금지. 기존 TMP 보존본 경로는 별도 Step 4 계약 |
| 4. 선택 범위 도입 | AI가 선택 파일 목록·구성요소별 source→target/원본·파생 역할·기대 hash/GUID/meta·직접 참조를 고정하고 검사, 소유자가 필요한 Unity import 실행 | 순수 공급본은 `Assets/ThirdParty/<supplier>/<product>/`, 자체 연결/변형/설정은 `Assets/ProjectAssets/<asset_id>/`. 전체 vendor 폴더를 무조건 넣지 않는다. 선택된 필수 파일/meta·참조가 빠지거나 기존 목적지/동일 GUID가 있으면 중단한다 |
| 5. Import/참조 대조 | AI가 byte/hash·GUID·선언된 참조를 정적 대조, 소유자가 필요한 Unity Import/타입/subasset·Console를 확인 | 공급 원본 GUID/Importer를 임의로 바꾸지 않으며 같은 GUID의 두 버전을 동시에 넣지 않는다. 새 파생/연결 애셋에는 공급본의 어떤 GUID도 복제하지 않는다. 폴더/자산 meta는 각각 기록하고 원본의 재현과 자체 meta 보존을 구분한다 |
| 6. 게임 사용/확장 기록 | AI가 purpose/game_usage/references/notices/변경 이력을 갱신하고 소유자가 정확한 적용표에 따라 연결 | 실제 Scene/Prefab·Hierarchy·컴포넌트/필드와 게임 흐름, Resources/Shader 이름·동적/binary 한계, 배포물 내 license/Attribution 위치를 기록한다. 아직 미적용이면 예정으로 표시하며 사용처/조건 미기록 상태로 완료하지 않는다 |
| 7. Git/검증 gate | AI가 registry 검사와 inventory를 실행하고 구현된 Step 5의 현재 추적/staged 유입 검사를 별도로 수행 | 형식 통과·원본 제외·참조 정상·권리·독립 복원은 각각 별도 판정이다. 필요한 사용자 Import/Scene/Runner 결과는 Step 9로 연결한다. 필수 실패/예상 밖 로그는 중단하고 정확한 파일별 변경을 보존/원복하며 전체 reset/clean·cache 삭제로 처리하지 않는다 |

supplier/product는 도입 계획에서 정한 영문 소문자/숫자/밑줄/하이픈 경로 segment이며 실제 공급자·제품/버전의 정답은 관리 기록에 둔다. 버전 변경은 같은 ID의 이력으로 관리하고, 기존 설치와 새 버전의 GUID가 겹치면 보존/사용처/원복 계획을 먼저 확정한다. 검사 실패를 피하려고 GUID를 바꾸거나 전체 vendor tree를 이동하지 않는다.

공급본을 직접 고치는 대신 자체 Material preset·Importer 재적용 설정·연결 애셋·확장 코드를 분리한다. 외부 코드를 복사/수정한 자료는 순수 자체 소유로 재분류하지 않는다. 변경/파생 관계와 수정 조건·파일별 재현 방법을 기록하고 권리/기술적 분리가 불명확하면 보존/보류한다. 수정본/설정·연결 자료를 원본과 함께 통째로 ignore하거나 제외하지 않는다. 원본 필요 파일이나 필수 Attribution의 누락도 선택 범위 축소로 정당화하지 않는다.

재현 가능한 변경 정보에는 원본 version/hash·변경 파일·변경 이유·전후 설정·담당/근거·원복 순서를 남긴다. 유료/허용 불명 원본을 patch/test payload에 복제하지 않는다. 연결 변경이 필요하면 실제 Hierarchy/컴포넌트/필드·기존→최종값·저장/원복·정적 확인과 필요한 Unity suite를 사용자 적용표로 작성한다. 현재 실제 연결 대상은 없으므로 적용표·수동 Unity 작업은 없다.

`Tools/DependencyAudit/dependency-adoption.cjs`의 planSchema/validate/inspect는 이 단계의 읽기 전용 사전 검사 보조 함수다. 선택 목록에 대한 매핑·원본 쓰기 금지·기존 목적지/중복 GUID·선언 참조·사용처/조건/수정본 제외를 검사하며 복사/압축 해제·적용 기능과 CLI는 제공하지 않는다. 사람이 작성한 선택 목록 밖의 모든 의존성·동적 이름/fileID/Import 의미나 실제 권리 진위를 증명하지 않는다. 정상 사전 검사도 applied/rightsGranted/independentRestore를 true로 바꾸지 않는다.

## TMP 변경본·연결 설정 보존

Phase 2 Step 4의 보존본은 공급 archive만으로 재현된다고 보장할 수 없는 현재 TMP 변경본/설정·Material·meta를 함께 보관한다. 명시한 입력 해시·Editor/provider와 전체 경로 목록을 기준으로 준비 검사→명시적 exclusive 복사→고정 manifest 해시 검증→별도 임시 재적용 대조 순서를 따른다. 보관 경로는 `Ignore/DependencyPreservation/<id>/<manifest_sha256>/`이며 Assets 밖 ignore/비추적과 원본/HEAD/index 불변을 확인한다. manifest/payload는 로컬 비추적, 고정 expected 해시는 Task metadata에 둔다.

원본을 먼저 옮기거나 지워 보존 여부를 시험하지 않는다. 기존 출력·파일 덮어쓰기와 외부/상위 경로·junction을 거절하고, 부분 실패는 성공으로 표시하거나 자동 정리하지 않는다. 실제 원본 재적용/Unity import는 이 도구에서 수행하지 않으며 임시 경로 재적용과 구분한다. 로컬 보존은 권리/재배포 또는 독립 복원 승인이 아니다. 현재 보존 근거·검증 결과·재취득 제한은 Phase 2 Task Step 4와 DependencyAudit 안내를 따른다.

## Git 원본 유입 검사

Phase 2 Step 5의 별도 ingress CLI로 HEAD/index blob과 staged 변경을 읽어 현재 원본 추적/신규 유입을 검사한다. 미등록 ThirdParty/root meta·등록 원본/미확인 경계와 알려진 원본 hash/GUID의 다른 경로 복사도 검사하며 `.gitignore`만으로 안전을 판단하지 않는다. 자산/폴더 meta의 별도 명시 경계가 있으면 그 근거를 우선 대조한다. 프로젝트 보존 예외는 등록부/정책과 명확한 권리·변경·Git/meta 보존 검토 근거가 필요하며 원본 byte의 Git 복제를 자동 허용하지 않는다.

원본 지문은 명시한 공개 metadata 해시로 고정하고 제품/조건의 단일 기록을 복제하지 않는다. 검사 결과에 HEAD와 projected index·현재 ignore 근거·입력 해시·판정 범위/제약을 구분한다. 지문/정책 미연결·중복 GUID·불완전 수집은 빈 성공으로 처리하지 않는다. 일반 참조 finding·권리/복원 정상과 유입 gate 통과는 별도다. 현재 추적 해제 대상 0개이므로 index/ignore를 바꾸지 않으며, 이후 새 후보가 확인되면 정확한 목록과 재취득/변경본 보존·참조 근거 및 요청 범위를 먼저 확정한다. 임의 hook·전역 설정·강제 staging·추적 해제·이력 재작성/삭제를 하지 않는다.

## 의존성 재취득·복원 입력

Phase 2 Step 6에서는 고정 Editor·manifest/lock·현재 source/hash와 공급자 metadata를 기준으로 정확한 입력을 다시 찾는다. cache suffix나 개인 절대 경로를 계약으로 삼지 않는다. `dependency-restore.cjs`의 check는 읽기 전용이며 prepare-archives만 명시적으로 검증한 tgz를 Assets 밖의 ignore/비추적 경로에 exclusive 복사한다. 동일 hash의 기존 target은 재검증 후 재사용하지만 다른 내용은 덮어쓰거나 지우지 않는다. 실패 시 입력/부분 출력을 보존하고 원인·필요 작업을 출력하며 대체 버전 자동 설치로 숨기지 않는다.

| 순서/대상 | 같은 의존성을 준비하는 계약 | 중단·담당/검증 경계 |
| --- | --- | --- |
| 1. Editor/UPM | ProjectVersion·Unity manifest/lock을 보존하고 소유자가 같은 Editor로 정확한 provider를 resolve한다. AI는 package.json의 ID/version으로 cache를 재발견하고 graph/source·설치 metadata를 대조한다 | provider 누락/중복·버전/graph 불일치·junction/부분 읽기는 중단. 네트워크/계정/권리는 실제 재취득 시 소유자가 확인하며 현재 cache 존재로 성공 처리하지 않음 |
| 2. TMP | com.unity.ugui 2.0.0의 공급 archive SHA-256과 GUID/meta/Import·Resources/Shader 계약을 확인하고, Step 4의 고정 보존본을 검증한 뒤 변경 폰트/설정을 적용할 순서를 정한다 | 공급 archive만으로 변경본 재현을 주장하지 않음. 실제 Assets 재적용과 Unity Import/타입/subasset·참조 검증은 소유자/Step 9·Phase 3. 원본은 먼저 지우거나 이동하지 않음 |
| 3. npm local tgz | Cloud Code provider ID/version에서 UnityServicesNpmLocal~를 찾아 내부 package.json name/version·SHA-256·lock SHA-512 integrity와 dependencies를 대조한다. 안정 경로는 Ignore/DependencyArchives/npm/provider_id/provider_version/archive_sha256/filename | 내부 이름과 설치 alias를 구분. 권리/오프라인/계정 제한·archive 누락/오염·alias/lock mismatch는 구체 원인으로 보류. 원본·Secret·유료 payload는 Git/Test에 넣지 않음 |
| 4. npm 경로 이식 | AI가 package.json patch와 lock 대응 경로 mapping을 읽기 전용으로 준비한다. alias/version/integrity/graph는 그대로 두며 실제 선언/lock은 먼저 보존한다 | 현재 작업본에는 적용/설치하지 않음. 적용이 필수이면 별도 checkout에서 소유자가 검토된 manifest patch 적용→npm lock 재생성→AI의 실제 결과 정적 대조→필요한 사용자 npm ci를 수행. 부분 lock 편집·제안 diff만으로 npm 성공 판정 금지 |
| 5. NuGet/EmbeddedResource | net9.0·4개 PackageReference·CloudCode wildcard/runtime.js 논리 리소스를 유지한다. 기존 obj graph는 metadata 근거로만 사용하고 고정 ID/version graph를 별도 기록한다 | obj/cache는 lock이 아님. 별도 환경에서 소유자의 dotnet restore --use-lock-file로 graph/lock 확보 후 기존 13개 graph와 직접/전이 버전·feed를 대조. feed 접근·동봉 조건·새 graph는 미확인으로 남기며 Module 빌드/원격 게시·복구를 섞지 않음 |
| 6. LogSink | 기존 package.json·Dockerfile·index.js와 입력/config 경계를 보존. 현재 외부 npm 의존성 0개, container base tag node:22-alpine은 별도 입력이다 | immutable image digest·계정/네트워크·실제 container 동작은 현재 미검증. 기존 비밀값 보존/운영 경계를 유지하며 AI는 .env/계정/Secret 수집·Docker/원격 실행을 하지 않음 |
| 7. 결과/원복 | source/hash·현재 입력과 의도한 경로 변경, 사용자 실행 여부를 구분하고 새 입력/실행 결과를 Task에 기록한다 | 필수 실패·권리/자료 부족이면 중단. 원복은 정해진 선언/lock 후보와 새 출력 경로만 대상으로 하며 저장소 reset/clean·cache 청소·버전 대체로 처리하지 않음 |

현재 준비된 로컬 archive와 경로 patch는 설치 가능한 새 lock이나 독립 복원 성공이 아니다. 새 provider/version을 확인하면 등록부/필요 지문·복원 spec을 갱신하고 권리·동봉/Attribution·사용처를 다시 확인한다. 실제 설치/lock 생성 필요 여부는 Step 9, 별도 환경의 전체 복원은 Phase 3에서 판정한다. 그 전 단계의 미확인을 자동 승인/성공으로 바꾸지 않는다.

## 업데이트·경로 변경·제거·실패 원복

Phase 2 Step 7의 절차다. 같은 영구 ID의 변경 전 기록과 실제 입력을 먼저 보존하고 후보 기록/경계 정책을 별도로 준비한다. 제품·버전/source/license·hash뿐 아니라 조건/Attribution·복사/수정·목적/실제 사용처·직접/동적 참조·import_path/meta/Import·Git/복원·lifecycle 정보가 달라지면 확인 상태의 근거를 다시 검토한다. 기존 권리/경계 적용·복원 상태를 새 버전이나 새 경로에 자동 승계하지 않는다. 최종 등록부는 검증 후 갱신하며 사전 계획의 후보 상태를 실제 적용 완료로 기록하지 않는다.

| 작업 | 준비/실행 순서와 담당 | 중단·보존/갱신 기준 |
| --- | --- | --- |
| 업데이트 | AI가 현재 기록·version/source/hash·원본/변경본/설정·자산/폴더 meta·선언/lock·직접/이름/동적/binary 사용처를 고정하고 검증된 보존 입력을 연결한다. 소유자가 새 조건/수정·배포/Attribution·재취득 제한과 정확한 도입 범위를 결정한다. AI가 새 후보 기록·경계/참조/Import 계획과 필요한 사용자 적용표를 준비한다 | 먼저 기존 재현 입력을 남기고 새 버전 조건/참조/검증 상태를 분리한다. 원본 저장소는 직접 고치지 않으며 공급 교체와 자체 파생 변경을 구분한다. 중복 GUID·변경본/설정/보존 누락·권리/자료 불명·예상 밖 진단이면 중단하고 보존한다. 사용자 Import/필수 suite 결과 후 같은 ID의 변경 이력/사용처·조건·검증 결과를 갱신한다 |
| 경로 변경 | AI가 같은 공급 version/hash에서 정확한 출발/도착·동일 byte·meta/GUID의 이동 쌍, 참조/Resources 상대 이름·Shader/asmdef 계약과 Editor 적용/원복표를 준비한다. 소유자가 그 범위로 Editor 이동한다 | 임의 복사로 같은 GUID를 두 경로에 동시에 넣거나 새 파생물에 공급 GUID를 복제하지 않는다. 이동과 버전 변경을 한 작업에 섞지 않는다. meta/GUID/Import·이름/사용처를 재대조하고 새 import_path/경계 정책·복원 경로/이력을 갱신한다. 경로만 바뀌어도 과거 확인 근거를 자동 재사용하지 않는다 |
| 제거/폐기 | AI가 Scene/Prefab/Material/Script·Resources/Shader/asmdef·직접 GUID/fileID·동적/binary 사용과 배포 Attribution 영향을 조사하고 누가 어느 연결/선언을 바꾸는지 표로 적는다. 소유자가 권리·사용처·재복원/이력 보존 범위를 확인한다 | 미확인 참조/조건/동적·binary 한계·보존 입력이 남으면 자료를 유지하고 보류한다. 실제 파일 삭제는 기본 자동화하지 않는다. 사용처 해소와 자료/원본 삭제를 분리하고 등록부에는 같은 ID의 폐기 상태·최종 version/source·변경 이력과 필요한 Attribution/재복원 의무를 남긴다. 기록 자체를 삭제해 의무를 없애지 않는다 |
| 실패/중단 원복 | 먼저 실패 당시 상태와 변경 전의 원본/설정/meta·선언/lock을 보존한다. AI가 시도한 경로별 before/attempt hash와 현재 hash·GUID/Import/이름을 대조하고 원복할 정확한 목록/순서를 적는다. 소유자가 필요한 Editor/Scene/Prefab 적용을 수행한다 | 현재 파일이 baseline 또는 그 시도의 기대 byte가 아니면 다른/늦은 변경으로 간주해 덮어쓰지 않는다. 이미 baseline이면 건드리지 않고 새로 생긴 출력은 검토용으로 보존한다. 검증된 원본/변경 설정/meta→필요한 사용자 이동/Scene 연결표→명시한 선언/lock→GUID/이름/참조/조건/기록 재검증 순서로 처리하며 전체 reset/clean·cache 삭제로 원복하지 않는다. 원복 계획/byte 일치는 새 Unity/독립 복원 성공이 아니다 |

`dependency-lifecycle.cjs`는 순수 evaluate와 읽기 전용 check/plan이다. before/candidate 기록·baseline/changes/검증된 backup·참조/한계·GUID/Import/name·적용표와 review 근거를 한 scope digest로 묶고, 소유자 결정과 각 검토가 그 digest를 가리키는지 확인한다. 늦은 조건/설정/경로/근거 변경은 기존 검토를 무효화한다. 현재 기록과 과거/후보 snapshot 및 다른 ID의 보존도 대조한다. 근거 없는 보존/제외·미해결 참조/유효하지 않은 이동·부분 적용/다른 작업의 변경은 보존/보류한다.

도구는 자산/Scene/Prefab·선언/lock·등록부를 쓰거나 삭제하지 않는다. 실제 Scene/Prefab 변경은 Hierarchy/컴포넌트/필드·기존→최종값·onClick/Navigation(해당 시)·저장/원복·정적 전후 확인과 필요한 Unity suite를 사용자 적용표로 연결한다. 정적으로 확인 가능한 조사를 사용자에게 넘기지 않는다. 현재 항목/작업 0개이므로 가상 제품/계획을 실제 등록부에 만들거나 불필요한 수동 검증을 요구하지 않는다.

scope digest와 metadata 주장은 권리 진위/소유자 신원·실제 backup payload나 Unity 실행 성공을 인증하지 않는다. 실제 적용 전 검증된 보존본/공급 입력과 현재 상태를 다시 확인하고 실제 결과를 받아 독립 상태를 판정한다. 이 단계의 readyForReviewedPreparation은 검토된 계획일 뿐 적용/삭제·권리 부여·독립 복원 승인이 아니다.

---

# 금지 사항

## Scene 기반 UI 구성

- UI GameObject·표시/입력 컴포넌트는 Scene/Prefab에 미리 구성한다. 생산 UI에서 `new GameObject`, `Instantiate`, `AddComponent`로 UI를 동적 생성하거나 누락 참조를 자동 보충하지 않는다.
- UI의 고정 배치·크기·Anchor·Pivot·부모/스타일 기본값은 Editor에서 유지한다. Runtime에서 하드코딩한 레이아웃으로 덮어쓰지 않아 Play 전에도 최종 화면을 예측할 수 있어야 한다.
- Runtime은 기존 UI의 Text/점수·선택/포커스·활성/입력 가능 상태·스크롤 등 상태를 갱신한다. 필수 구성 누락은 진단하고 Editor 적용 항목으로 안내한다.
- UI Test도 생산 Scene의 기존 객체/참조를 사용하며 생산 구성을 대신할 UI 생성 fixture를 추가하지 않는다. AI는 Scene/Prefab을 편집하지 않고 정확한 사용자 적용 방법과 정적 확인 결과를 제공한다.

확인되지 않은 요구사항을 추측하여 구현하지 않는다.

동일한 목적의 구조를 중복 구현하지 않는다.

기존 구조를 충분히 검토하지 않고 새로운 구조를 추가하지 않는다.

문서와 다른 동작을 구현하지 않는다.

작업 요청과 무관한 리팩토링을 수행하지 않는다.

임시 방편을 최종 구현으로 남겨두지 않는다.

구조 변경이 필요한 경우 관련 문서를 수정하지 않은 채 구현만 변경하지 않는다.

---

# 관련 문서

## Rules

- AI_RULE.md
- CODING_STYLE.md
- EVENT_RULE.md
- INVESTIGATION_RULE.md
- LOGGING_RULE.md
- VERIFICATION_RULE.md

---

## Project

- PROJECT_OVERVIEW.md
- ARCHITECTURE.md
- PROJECT_MEMORY.md
