# Dependency audit

처음 Third Party 파일을 관리하는 작업자는 [통합 가이드](../../THIRD_PARTY_GUIDE.md)에서 시작한다. 이 문서는 검사 명령/API와 검사 한계가 필요할 때 참조한다.

Roadmap 10 Phase 1~3은 완료(2026-11-25)했다. [Phase 3 Task Step 10](../../AI/90_Tasks/Prototype_10/20261120_04_Phase3ManualSteps.md)·[실측 평가](../../AI/90_Tasks/Prototype_10/20261125_36_Phase3Step10Assessment.json)·[최종 인계](../../AI/90_Tasks/Prototype_10/20261125_37_Phase3RestoreAndAdoptionHandoff.md)에 별도 snapshot/UPM·TMP 재적용·실제 npm lock/ci/NuGet restore·사용자 Unity 결과와 원래 자료 보존을 연결했다. 현재 기술 기준선 복원 완료이며 후보 raw audit는 기존 GUID 31회로 exit 1/orphan 0개다. 원래 작업본 선언/lock은 그대로이며 기존 spec pin과 후보 이식 입력을 혼용하지 않는다. 제품별 권리/모든 Player/variant·원격 복구 완료는 아니다. 추가 수동 실행은 없으며 과거 스크립트는 새 환경의 입력/경로/출력 확인 없이 반복하지 않는다.

외부 package 없이 Node 내장 모듈로 실행하는 Phase 1 읽기 전용 metadata 검사다. Unity/컴파일러/Test Runner·Player·서비스/원격·설치/복원을 실행하지 않는다. Git은 고정된 read 명령만 허용하고 optional lock·fsmonitor·external diff/textconv 실행을 차단한다. 검사 자체는 파일을 쓰지 않으며 JSON을 stdout으로 반환한다.

```powershell
node Tools/DependencyAudit/dependency-audit.test.cjs
node Tools/DependencyAudit/dependency-audit.test.cjs --policy AI/90_Tasks/Prototype_10/20261119_03_Phase1Step8Policy.json
node Tools/DependencyAudit/dependency-audit.cjs --check
node Tools/DependencyAudit/dependency-audit.cjs --check --baseline AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json
```

`--root`는 조사할 Git 저장소 루트를 명시할 때만 쓴다. `--baseline`은 그 루트 안의 상대 경로이며 생략하면 과거 Task의 기준선을 자동으로 정답으로 쓰지 않는다. 실제 index는 매 실행 전후 비교한다. 결과를 보존할 경우 호출자가 지정한 Task metadata 경로에 UTF-8로 저장한다. Windows PowerShell의 기본 `>` 인코딩을 그대로 사용하지 않는다.

| 종료 코드 | 의미 |
| --- | --- |
| 0 | 구현된 정적 검사 범위에 진단 없음. 권리/분류 확정·Unity 해석·독립 복원 성공은 아님 |
| 1 | 수집은 완료했으나 unresolved GUID·orphan 등 finding 존재 |
| 2 | 접근/열거/JSON·Git/형식 오류·불완전 metadata·기준선 변경 등으로 완전한 결과라고 할 수 없음 |

Assets/Packages/ProjectSettings/Tools/UGS와 루트 ignore/npm 선언을 조사한다. cache는 package.json의 ID/버전으로 찾아 패키지 meta와 현재 자산에서 연결되는 직렬화 자료만 읽는다. 일반 Library/Build·bin/obj/node_modules·사용자 설정 등은 제외한다. symlink/junction은 따라가지 않고 오류로 남긴다. Secret/로컬 production 설정은 본문을 읽지 않으며 meta는 GUID/Importer 참조 metadata로만 사용한다.

현재 구현은 Git/ignore·패키지 graph/설치 metadata·GUID/meta·연결된 YAML/ShaderGraph/Importer·프로젝트 소스의 이름/동적/binary 한계·등록 형식·보수적 제외 판정을 검사한다. package의 임의 동적 로딩·Unity 타입/subasset/렌더링이나 권리 판단을 대신하지 않는다. `ownership`은 출처/권리/Import 재현 근거를 자동 추정하지 않아 실제 목록을 `preserve-pending`으로 둔다. 별도의 분류 함수도 필수 근거/사용자 결정이 없으면 제외 후보를 확정하지 않는다. 실제 기록과 판단은 [관리 기록](../../AI/00_Project/ASSET_DEPENDENCIES.md) 및 [Phase 1 Task](../../AI/90_Tasks/Prototype_10/20261118_02_Phase1ManualSteps.md)로 연결한다.

기본 Unit Test는 합성 metadata·주입 fs/Git 실패·임시 저장소만 사용한다. 임시 저장소의 초기화/staging/commit·파일 생성/정리는 fixture 준비이며 실제 작업본 index/설정에는 적용하지 않는다. 이름/개수는 고정된 과거 프로젝트 수치의 복제가 아니라 검사 책임의 정상/실패 입력으로 검증한다.

선택적인 Test `--policy`는 저장소 내 보존 정책 metadata를 실제 분류 함수에 재생한다. 승인/제외·권리 부여나 실제 애셋/저장소 변경을 하지 않으며, 보존 판정과 조건 미확인·후속 의무가 함께 유지되는지 확인한다. 기본 suite와 이 정책 검증 결과를 Unity Runner/독립 복원 결과로 해석하지 않는다.

## 전체 검증 — Phase 2 Step 8

프로젝트 루트에서 아래 suite를 실행한다. Test는 합성 입력·임시 저장소를 사용하고 실제 index/원본을 변경하지 않는다. 명시적 보존/복사 함수의 정상/실패도 임시 fixture에서 검증한다.

```powershell
node Tools/DependencyAudit/dependency-audit.test.cjs --policy AI/90_Tasks/Prototype_10/20261119_03_Phase1Step8Policy.json
node Tools/DependencyAudit/dependency-registry.test.cjs
node Tools/DependencyAudit/dependency-adoption.test.cjs
node Tools/DependencyAudit/dependency-preservation.test.cjs
node Tools/DependencyAudit/dependency-ingress.test.cjs
node Tools/DependencyAudit/dependency-restore.test.cjs
node Tools/DependencyAudit/dependency-lifecycle.test.cjs
```

Step 8 실행 당시 7개 suite/261개 통과이며 건수를 다음 실행의 고정 성공 조건으로 쓰지 않는다. 실제 읽기 전용 7개 CLI의 정확한 명령/반복 출력·require/쓰기/네트워크 차단 감사와 Test별 source는 [검사 metadata](../../AI/90_Tasks/Prototype_10/20261119_26_Phase2Step8Check.json), 기존 입력/보존본/archive·HEAD/index 불변은 [최종 검증](../../AI/90_Tasks/Prototype_10/20261119_27_Phase2Step8Verification.json)에 연결한다. 보존본 create/임시 reapply·archive prepare는 기본 check와 분리된 명시적 쓰기이며 기본 검증 묶음에 포함하지 않는다.

Phase 2 당시 inventory는 기존 미해결 참조 31회와 sample orphan으로 exit 1이었다. 최신 후보는 Phase 3 최종 인계를 따르며 기존 31회/orphan 0개로 exit 1이다. 유입 gate/복원 입력/lifecycle check의 exit 0과 독립적으로 기록하며 진단을 지워 전체 성공으로 만들지 않는다. 실제 Scene/Prefab/Unity·설치·권리/독립 복원은 별도 증거가 필요하다. 현재 Step의 변경은 metadata/문서이므로 사용자 Unity 실행은 요청하지 않는다.

## 실제 등록부/경계 정책 검사 — Phase 2 Step 2

```powershell
node Tools/DependencyAudit/dependency-registry.test.cjs
node Tools/DependencyAudit/dependency-registry.cjs --check
```

별도 registry CLI는 실제 `AI/00_Project/ASSET_DEPENDENCIES.md`와 `Tools/DependencyAudit/dependency-policy.json`을 고정 입력으로 읽는다. `--root <저장소>`와 `--previous <저장소 상대 이전 등록부.md>`만 추가 지원한다. 출력은 값/원본 본문 대신 입력 해시·항목 수·경로 판정/사유 코드이며 exit 0은 등록/정책 형식·연결 정상, exit 2는 수집/형식/상충 실패다. 기존 inventory CLI의 참조 finding/exit와 분리한다. 기본 snapshot 검사에서는 transitionCompared=false이며 이전 상태 비교를 수행했다고 주장하지 않는다.

parser/evaluate/validatePolicy/validateTransition은 순수 함수이며 require 시 실행하지 않는다. CLI는 기존 reader의 민감 본문/상위 경로·symlink/junction 차단과 입력/index 전후 해시 검사를 재사용한다. 세부 셀 문법은 [등록 형식](../../AI/99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)의 자동 검사 계약을 따른다. JSON/table/schema 오류는 입력 값을 노출하지 않는 코드로 반환한다. URL의 계정/credential·query/fragment를 거절하지만 임의 문장 속 모든 Secret을 탐지한다는 보장은 없으므로 비밀값을 입력하지 않는다.

정책 entries는 실제 등록부에 있는 경계 행과 일대일 연결하며 제품·버전/license·권리 값의 두 번째 정답을 만들지 않는다. 현재 entries는 0개다. registry CLI 자체는 Git 유입 차단 기능이 아니며 unregisteredRoots를 별도 Step 5 ingress CLI에 연결한다. unknown/mixed·변경/파생/설정·사용자 결정/근거 부족은 보존/보류하고, 제외 요청과 모순되면 실패한다. 통과해도 applied/rightsGranted/independentRestore를 자동 true로 바꾸지 않는다.

## 도입/확장 사전 검사 — Phase 2 Step 3

```powershell
node Tools/DependencyAudit/dependency-adoption.test.cjs
```

영구 실행 절차는 [Implementation Rule](../../AI/01_Rules/IMPLEMENTATION_RULE.md)의 외부 애셋 도입·활용 실행 절차다. `dependency-adoption.cjs`는 require 시 실행하지 않는 보조 함수이며 CLI·복사/추출/적용 기능은 없다. 실제 후보가 생기면 기존 registry parser/validatePolicy로 확인한 record와 명시적 계획을 입력한다. 현재 실제 제품이 없으므로 계획 파일/원본 보관 폴더를 만들지 않는다.

planSchema의 입력은 schemaVersion 1, asset_id/version, supplier/product 경로 segment, sourceRoot, archivePath/archiveSha256, selectedSources, mappings다. sourceRoot는 `Ignore/DependencySources/<asset_id>/<archiveSha256>`이며 archivePath와 선택 원본은 그 아래 정규 상대 경로다. mappings의 각 항목은 source/target/kind/sourceSha256/sourceMetaSha256/sourceGuid/targetGuid/references(GUID 배열)/usageField를 가진다. usageField는 game_usage로 고정해 영구 등록부를 참조한다. 계획은 해당 실행의 입력이며 version/hash는 등록부와 대조한다. 제품/license의 별도 영구 등록부가 아니다.

inspect(root, plan)는 고정된 Git read 명령과 기존 reader를 재사용해 보관 입력의 ignore/비추적, archive·선택 원본/meta 실제 해시·GUID, Assets 내 기존 경로/meta를 읽는다. 원본 meta 전체 해시로 Import 설정 변화를 대조하고 관측된 직렬화/Importer GUID가 계획의 references에서 누락되면 거절한다. symlink/junction·민감 본문·부분 열거/읽기 실패는 거절하고 입력/목록·HEAD/index 전후 불변을 확인한다. validate(record, plan, files, existingMeta)는 순수 함수로 원본/확장 경로, 범위 누락·원본 쓰기/덮어쓰기·중복 GUID/다른 원본 GUID를 복제한 파생물·선언 참조·사용처/조건 미기록·수정본 제외를 판정한다. provider GUID는 보존하고 파생물은 새로운 GUID를 사용한다.

검사 범위는 명시한 파일 목록·직접 GUID에 한정한다. Resources/Shader 이름·동적/binary·fileID/subasset/Import 타입은 등록부에 따로 기록하고 필요한 실제 검증을 Step 9/Phase 3으로 연결한다. inspector의 비추적 보관 확인은 별도 ingress CLI의 HEAD/index 검사를 대신하지 않는다. 자체 Test는 합성 자료/임시 Git 저장소만 쓰며 실제 원본 복사·사용자 작업본 staging·Unity를 실행하지 않는다.

## TMP 보존·임시 재적용 — Phase 2 Step 4

`dependency-preservation.cjs`는 TMP 작업본 전체와 해당 공급 archive의 명시적 보존 도구다. 기본 동작/require에는 복사가 없으며 prepare/verify는 읽기 전용이다. create만 명시한 Assets 밖 비추적 경로에 복사한다. 원본 이동/삭제·기존 출력 덮어쓰기·cache 청소·Unity/설치·Scene 변경은 제공하지 않는다.

```powershell
node Tools/DependencyAudit/dependency-preservation.test.cjs
node Tools/DependencyAudit/dependency-preservation.cjs --check AI/90_Tasks/Prototype_10/20261119_13_Phase2Step4PreservationSpec.json
```

현재 보존본은 이미 생성했다. 반복 생성은 overwrite 오류로 거절한다. 재검증 명령은 다음과 같다.

```powershell
$bundle = 'Ignore/DependencyPreservation/tmp-baseline/29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c'
node Tools/DependencyAudit/dependency-preservation.cjs --verify AI/90_Tasks/Prototype_10/20261119_14_Phase2Step4Check.json $bundle
```

새 보존본이 필요한 경우 `--check <spec.json>`의 예상 output을 검토한 다음 `--create <spec.json> <그 output>`을 명시한다. spec은 schemaVersion/id/editorVersion/provider(id/version/archivePath/archiveSha256)/files(path/sha256)다. 현재 TMP 파일·root meta 전부와 공급 archive의 완전한 목록을 요구해 설정/Material/meta 누락을 실패로 판정한다. 현재 cache suffix는 이번 입력 위치일 뿐 복원 계약이 아니며, provider ID/버전으로 새 공급물을 찾는 작업은 Step 6에서 구현한다.

출력은 `Ignore/DependencyPreservation/<id>/<manifest_sha256>/files/<원래 상대 경로>`와 manifest.json이다. manifest에는 파일 크기/해시·meta GUID·Resources 상대 이름/Shader 이름·Editor/provider 고정 정보를 기록한다. create는 경로/ignore/비추적·junction·입력/index 불변을 먼저 확인하고, 파일을 exclusive 생성한 뒤 마지막에 manifest를 쓴다. 실패 시 부분 디렉터리를 자동 삭제/덮어쓰지 않는다. 부분 출력은 성공한 보존본이 아니며 verify가 실패한다. source/manifest/payload의 hash·coverage·GUID/Import/name 불일치도 실패다.

manifest schemaVersion 2는 folderMeta도 기록한다. 폴더 meta의 원래 디렉터리 존재를 확인하고 빈 폴더까지 보존/임시 재적용하며, 보존본에서 폴더가 빠져도 실패한다. 최초 검토 중 생성한 schema 1 보존본은 빈 폴더 계약이 없어 최종 복원 입력으로 사용하지 않는다. 덮어쓰거나 지우지 않고 별도 로컬 이력으로 남겼으며 Task Step 4 검사에서 superseded 상태로 구분한다.

`--verify <expected.json> <output>`은 고정 expected 계약(id/manifestSha256/editorVersion/providerId/providerVersion/archiveSha256) 또는 그 expected 필드를 읽는다. 원래 작업본이 없어도 검증 가능하며 현재 source에서 기대 해시를 다시 만들어 오염된 보존본을 정답으로 삼지 않는다. expected는 Git의 Task metadata, payload/manifest는 비추적 로컬 보존본에 분리한다. exit 0은 해당 보존/검증 범위 통과, exit 2는 실패이며 오류는 입력 본문을 노출하지 않는 코드다.

`reapplyToTemporary(root, output, expected, target)`는 별도 os.tmpdir 바로 아래 `flowstate-preservation-reapply-` 접두사의 빈 실제 디렉터리에만 재적용한다. Assets/Library 상대 구조를 그 임시 대상 안에서만 재구성하고 byte/hash·GUID/meta/Import bytes·Resources/Shader 이름을 대조한다. 실제 Assets 복원/덮어쓰기는 지원하지 않는다. 완료 후 임시 대상 정리는 호출자가 절대 경로·임시 루트/접두사를 검증해 수행한다. require/check/verify는 쓰지 않으며 실패 시 부분 재적용 대상도 자동 삭제하지 않는다.

로컬 보관·임시 byte 재적용은 공급 조건/재배포 승인이나 Unity Import/타입/subasset·참조/렌더링/독립 복원 성공을 뜻하지 않는다. 원본/Secret·유료 payload는 문서/Test/Git에 복제하지 않는다. 현재 보존본/제한과 해시는 [Phase 2 Task Step 4](../../AI/90_Tasks/Prototype_10/20261119_05_Phase2ManualSteps.md)에 연결한다.

## Git 원본 유입 검사 — Phase 2 Step 5

```powershell
node Tools/DependencyAudit/dependency-ingress.test.cjs
node Tools/DependencyAudit/dependency-ingress.cjs --check-managed --evidence AI/90_Tasks/Prototype_10/20261119_22_Phase2Step6KnownOrigins.json
node Tools/DependencyAudit/dependency-ingress.cjs --check-staged --evidence AI/90_Tasks/Prototype_10/20261119_22_Phase2Step6KnownOrigins.json
```

기존 inventory CLI에 새 옵션을 붙이지 않고 별도 ingress CLI를 사용한다. `--root <저장소>`도 지원한다. `--check-managed`는 HEAD와 현재 index 양쪽을, `--check-staged`는 전체 projected index를 gate로 판정하며 HEAD 결과와 staged add/modify/rename/copy/delete 목록은 별도로 제공한다. 삭제되어 index에서 사라진 원본은 staged gate에서 차단하지 않지만 HEAD에 남은 추적 상태는 managed gate가 드러낸다.

고정된 읽기 전용 Git 명령으로 tree/index의 mode·object ID·NUL 경로와 실제 blob을 읽는다. worktree의 같은 이름 파일을 staged 내용으로 대신하지 않는다. blob hash와 UTF-8 text의 CRLF→LF 정규화 hash, meta GUID로 알려진 원본의 다른 경로 복사/rename을 찾는다. 중복 GUID·대소문자 충돌·unmerged stage·link/submodule·Git/형식/증거 오류는 빈 성공이 아닌 불완전 실패다. 현재 `check-ignore --no-index`의 source/line/pattern과 tracked+ignored를 함께 출력하며 시작/종료 ignore 결과·입력/HEAD/index 불변을 확인한다. ignore 여부 자체가 원본 추적을 허용하지 않는다.

evidence 입력은 schemaVersion 1, evidence(근거 metadata path/sha256), origins(asset_id/path/sha256/textSha256/guid/kind), preserveExceptions(asset_id/path)다. 제품/license·권리의 별도 등록부가 아니라 명시적으로 고정한 원본 지문·보존 근거 참조다. 현재 110개 지문은 Step 4의 TMP 83개와 Step 6의 Cloud Code tgz 27개 기록에서 만들었고 metadata 해시를 대조한다. 원본 경계가 등록되어 있는데 지문 근거가 없으면 오류다. 실제 새 도입/버전 변경 시 동일 등록 ID의 관련 지문/근거를 갱신하고 검사한다. 권리 판단이나 완전한 원본 탐지를 자동 추론하지 않는다.

등록된 original 경계·그 root/child meta와 미등록 ThirdParty/root meta, 미확인/혼합 경계를 차단한다. 명시적인 개별 meta 경계는 상위/자산 경계의 암묵적 meta 매칭보다 우선한다. 프로젝트 보존 예외는 기존 등록부/정책의 modified/derived/project/settings·preserve, 확인된 권리/경계와 git_boundary/modifications/meta_import/license/evidence/owner_review 근거가 필요하다. 알려진 순수 원본 byte가 같으면 예외가 있어도 차단한다. 예외는 원본의 자동 Git 복제 승인으로 사용하지 않는다.

exit 0은 이 유입 gate 범위 통과, exit 1은 원본/검토 필요 후보 차단, exit 2는 입력/수집/판정 불완전이다. 일반 참조 inventory의 exit 1과 구분하며 referenceHealthPass/rightsGranted/independentRestore는 false다. hooks/전역 Git 설정·staging/추적 해제·자료 삭제는 하지 않는다. 현재 실제 추적 해제 대상은 0개이며 `.gitignore` 추가 변경도 필요하지 않았다.

현재 ignore와 registry/policy/catalog는 명시한 worktree 입력이다. HEAD/index 속 ignore나 서로 다른 staged 정책 버전을 재현하는 commit hook은 아니며 각 입력 해시/제약을 결과에 표시한다. namespace 밖의 출처 불명 원본, 바뀐 내용의 미등록 파생물, LFS payload·민감 본문·동적/binary/fileID/Unity 참조 해석 전체는 보장하지 않는다. 현재 원본 지문 목록 밖의 모든 외부 소스를 완전히 식별할 수 있다고 주장하지 않는다.

## 누락/버전·재취득/복원 입력 — Phase 2 Step 6

```powershell
node Tools/DependencyAudit/dependency-restore.test.cjs
node Tools/DependencyAudit/dependency-restore.cjs --check AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json
```

`--check`는 고정 spec의 Editor/declaration hash·UPM graph와 provider metadata를 읽고 TMP archive 및 npm tgz의 name/version/SHA-256·SHA-512 integrity/dependencies·설치 alias를 검사한다. supplier package.json ID/version으로 cache를 찾으므로 폴더 suffix가 바뀌어도 발견한다. provider 중복/누락·wrong version·JSON/BOM/graph/source/alias/lock 불일치·오염/누락·junction/부분 읽기는 실패이며 exit 2로 원인/가능한 입력 경로·다음 작업을 반환한다. cache·registry/계정/오프라인 제한을 설치/복원 성공으로 바꾸지 않는다.

현재 정확한 tgz 27개를 준비한 안정 경로는 `Ignore/DependencyArchives/npm/com.unity.services.cloudcode/2.10.4/<archive_sha256>/<filename>`이다. 명시적 생성/재검증은 다음 명령이다. 현재는 모두 존재하며 같은 byte를 검증해 재사용한다.

```powershell
node Tools/DependencyAudit/dependency-restore.cjs --prepare-archives AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json
```

이 모드만 tgz를 exclusive 복사한다. source/hash·ignore/비추적·target 충돌/링크·HEAD/index 불변을 확인하고, 부분 실패는 자동 삭제하지 않는다. 다른 byte는 거절하고 같은 byte만 재사용한다. npm install/ci·dotnet restore·Docker·Unity·lock 생성/선언 적용을 실행하는 옵션은 없다. require/check는 복사하지 않는다.

경로 제안은 [Step 6 검사](../../AI/90_Tasks/Prototype_10/20261119_20_Phase2Step6Check.json)의 proposal과 [package.json patch](../../AI/90_Tasks/Prototype_10/20261119_21_Phase2Step6NpmPathMigration.patch)다. manifest 27곳과 lock root/resolved 대응 54곳의 경로만 변경하는 제안이며 현재 원본 manifest/lock은 보존했다. patch의 git apply --check도 통과했다. lockOperations는 검토용 mapping이며 설치 가능한 lock 파일을 손으로 부분 편집해 만들라는 뜻이 아니다.

Phase 2 Step 9에서 현재 생산 자산/설정·C#·Scene/Prefab·선언/lock 변경과 실제 도입 요청이 없어 사용자 Compile/Runner/Build·설치/lock 생성이 불필요함을 판정했다. [Step 9 판정](../../AI/90_Tasks/Prototype_10/20261119_28_Phase2Step9Check.json)과 [최종 검증](../../AI/90_Tasks/Prototype_10/20261119_29_Phase2Step9Verification.json)을 따른다. 실제 이식 적용/독립 복원이 필요해질 때의 조건부 사용자 순서는 다음과 같다. 지금 실행할 작업은 아니며 실제 lock 생성/설치 성공은 미확인이다.

1. 원래 작업본·QA/저장/Secret을 보존한 별도 checkout에 정확한 Editor/UPM provider·필요 archive를 준비한다. vendor 조건·계정/네트워크 접근과 버전/hash를 확인하고 부족하면 중단한다.
2. AI가 고정 입력과 patch의 적용 가능성/예상 diff·원복할 package.json/package-lock.json을 먼저 확인한다. 소유자가 별도 checkout에서 검토된 package.json patch를 적용한다. 현재 원래 작업본에는 적용하지 않는다.
3. 소유자가 그 checkout에서 `npm install --package-lock-only --ignore-scripts --no-audit --no-fund`로 실제 lock을 생성한다. 실패·버전/graph 변경은 중단하며 부분 lock 편집이나 임의 버전 대체로 우회하지 않는다.
4. AI가 원래 baseline workspace에서 새 후보 manifest/lock의 metadata를 검사한다. 후보 파일이 아래 예시 경로에 있을 때의 명령은 다음과 같다. 원래 spec/선언을 덮어써 새 결과를 정답으로 만들지 않는다.

```powershell
node Tools/DependencyAudit/dependency-restore.cjs --verify-migration AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json Ignore/DependencyRestoreCandidate/package.json Ignore/DependencyRestoreCandidate/package-lock.json
```

5. 검사는 alias/version/integrity/graph와 지정한 경로 외 모든 필드가 기준선과 같은지 확인한다. 변화가 있으면 검토 대상으로 실패한다. 필요한 실제 `npm ci --ignore-scripts --no-audit --no-fund`는 사용자 실행이며 설치/lock 생성 결과를 받은 후에만 성공을 기록한다. 현재 실제 생성 lock/설치 성공을 주장하지 않는다.

NuGet은 csproj의 net9.0·직접 4개 버전/EmbeddedResource 2개 선언과 JS 경로·runtime.js 존재를 대조한다. 현재 local obj의 13개 graph/공개 feed는 고정 metadata로 기록하고 직접/전이 drift를 검사하지만 해당 graph는 새 restore나 committed lock이 아니다. 실제 graph/lock 확보는 별도 환경에서 사용자 `dotnet restore UGS/Modules/FlowStateVerification/FlowStateVerification.csproj --use-lock-file`로 수행하며 AI는 이를 실행하지 않는다. 결과의 버전/feed·동봉 조건과 원래 source/resource 계약을 다시 확인한다.

LogSink는 현재 외부 npm 의존성 0개, node:22-alpine tag만 확인했다. image digest/registry 접근·실제 container 동작과 기존 Secret/권한은 별도 미확인 경계이며 [기존 LogSink 안내](../../UGS/LogSink/README.md)를 유지한다. Module 빌드/배포·복구나 Phase 3 전체 복원을 이 검사에 섞지 않는다.

Step 6에서 알려진 원본 지문은 TMP 83개와 Cloud Code tgz 27개를 합친 110개로 확장했다. 새 입력은 [복원 지문](../../AI/90_Tasks/Prototype_10/20261119_22_Phase2Step6KnownOrigins.json)이며, 기존 Step 5의 83개 metadata는 과거 근거로 보존한다. 최신 ingress 검사는 위 --evidence 인자에 이 새 파일을 지정한다. technical-cloudcode-baseline은 실제 제품 도입 항목이 아닌 기술 기준선 지문 ID다.

## 업데이트·이동·제거·원복 판정 — Phase 2 Step 7

```powershell
node Tools/DependencyAudit/dependency-lifecycle.test.cjs
node Tools/DependencyAudit/dependency-lifecycle.cjs --check
```

현재 lifecycle 대상/계획은 0개다. check는 실제 등록부/정책을 읽고 입력/index 불변을 확인하지만, 요청을 고르지 않은 상태를 업데이트/제거/원복 성공으로 표시하지 않는다. 실제 후보가 생기면 AI가 입력/적용표를 준비해 다음 읽기 전용 preflight를 수행한다. 예시 request 경로는 현재 만들지 않았다.

```powershell
node Tools/DependencyAudit/dependency-lifecycle.cjs --plan Ignore/LifecycleRequest/request.json
```

request는 asset_id, previousRegistry/candidateRegistry(저장소 상대 Markdown snapshot), 선택적 candidatePolicy, context다. --root도 지원한다. 현재 등록부의 대상과 before snapshot, 다른 ID 기록·후보 정책을 대조해 가상/늦은 등록부 교체나 관련 없는 항목의 삭제/변경을 거절한다. context.current는 요청의 주장을 신뢰하지 않고 실제 파일 byte/hash·meta GUID/Import hash·Resources 상대 이름/Shader 이름에서 읽는다. 새 출력의 정상 부재와 junction/접근 오류를 구분하고 원래 자산/meta 쌍도 확인한다.

context의 schemaVersion은 1이며 operation은 update/relocate/remove/rollback이다. baseline/current는 path/sha256(null은 미존재), changes는 path/beforeSha256/afterSha256이다. baseline은 실제 기존 자산과 companion meta·변경 설정/선언 범위를 포함한다. backup은 verified/manifestSha256/evidence/files이며 실제로 검증한 보존 입력을 참조한다. referenceReview는 complete/evidence/incoming과 dynamic/binary의 status(resolved 또는 not-applicable)/evidence를 가진다. incoming은 from/target/kind(guid/fileID/resources/shader/asmdef), resolvedAfter 또는 제거 시 detachedAfter로 준비한다. 미해결/불완전 범위는 보류한다.

resourceIdentity는 path/before/after/planned/evidence이며 identity의 guid/importerSha256/resourcesName/shaderNames를 비교한다. reviews는 rights/attribution/references/guidImport/names/restore의 state/evidence/digest, decision은 operation/evidence/digest다. state=confirmed와 명시 근거가 필요하며 scopeDigest(before, after, context) 결과에 각각 바인딩한다. 현재 관측 상태는 digest 대상에서 제외하고 baseline/attempt와 대조하여 부분 실패 원복을 허용하되 다른 작업의 변경은 보호한다. scope에는 기록·조건·사용처·경로·변경/backup·참조/이름·검토 근거와 적용표가 포함되어 늦은 변경이 검토를 무효화한다.

relocate는 moves의 from/to/sha256 쌍과 moveApplication evidence/table을 요구하고 같은 version/source/hash에서 meta/GUID·byte를 유지한다. 출발의 부재를 단독 파일 삭제와 혼동하지 않으며 실제 이동은 사용자 Editor 적용표에 남긴다. Scene/Prefab 변경 계획에는 sceneApplication evidence/table이 필요하다. rollback은 failureEvidence와 현재 baseline/attempt hash 대조를 요구하고 명시된 경로에만 restore-verified-preserved-bytes/already-baseline/retain-created-output-for-review 계획을 출력한다. 새 출력은 자동 삭제하지 않는다. 제거도 폐기 기록/의무를 보존하며 실제 삭제 옵션은 없다.

exit 0은 현재 registry 확인 또는 선택한 요청의 검토 가능한 준비 계획, exit 1은 보존/보류 사유, exit 2는 잘못된 입력/정책/수집 실패다. 적용·삭제·원복·등록부 쓰기/Unity 실행을 수행하는 옵션은 없으며 require/evaluate는 부작용이 없다. 결과의 applied/filesDeleted/registryWritten/rightsGranted/independentRestore는 false다. 실제 적용 후 재검증/필요 사용자 Unity 결과 없이 상태를 완료로 갱신하지 않는다. 영구 담당/절차·중단/원복 기준은 Implementation Rule의 업데이트·경로 변경·제거·실패 원복을 따른다.

## 안전 snapshot·허용 변경 비교 — Phase 3 Step 3

`dependency-snapshot.cjs`는 기존 reader/Git·보존본 verify를 재사용하고 snapshot 전달/비교만 추가한다. 외부 npm 의존성은 없다. `require`와 `prepare`는 쓰기/설치/네트워크 없이 동작한다. 실제 원래 작업본/확정 후보에 대한 복사는 Step 4이며, Step 3에서는 임시 합성 fixture만 복사했다.

```powershell
node Tools/DependencyAudit/dependency-snapshot.test.cjs
```

읽기 전용 CLI는 `node Tools/DependencyAudit/dependency-snapshot.cjs --check <저장소 상대 manifest 경로>`다. 현재 manifest를 임의로 생성/실행하지 않고 Step 4에서 최종 목록을 고정한다. manifest는 `schemaVersion: 1`, `candidateRoot`(절대 경로), `files`(path/sha256), `requiredPaths`(빠지면 실패할 경로 목록), `directories`(추가 빈 폴더 목록), `includeSdkPin`(boolean)이다. 성공 exit 0은 읽기 전용 계획 해시/건수, 실패 exit 2는 진단 코드만 출력한다. `--root`, `--create`, install/apply/rollback CLI는 없다.

API `prepare(sourceRoot, manifest)`는 경로/해시·필수 목록·source Git/빈 폴더/중복 GUID를 검사하고 `planSha256`을 반환한다. 기존 source/실제 원래 workspace와 같거나 상하위인 대상, 이미 있는 대상(빈 폴더 포함), link/junction, case/parent-file 충돌, cache/QA/Secret/실 production config·미승인 원본을 거절한다. 일반 소스의 읽기는 기존 reader를 사용한다. Step 2에서 검토한 소스 6개만 정확한 경로와 고정 hash로 예외 처리하며 기존 전역 sensitive guard는 그대로다. 소스가 바뀌면 검토/pin 갱신 전까지 차단한다.

`privateInputs(sourceRoot)`는 과거 근거 자체의 고정 해시와 schema 2 보존본 verify를 대조해 npm tgz 27개와 보존본 manifest/payload 84개만 반환한다. 이 목록 밖의 Ignore/Library/유료 원본을 포함시키지 않는다. private payload의 중복 GUID는 Assets에 재적용한 것이 아니므로 프로젝트 GUID 중복과 구분한다. 실제 재적용/Unity Import 검증은 Step 5/7/8에 남긴다.

명시적 API `create(sourceRoot, manifest, expectedPlanSha256)`만 복사한다. 먼저 계획을 재계산하고 검토 해시와 일치할 때 새 대상/부모를 생성하며 파일은 exclusive `wx`로 쓴다. source/Git와 target을 전후 검증하고 부분 실패 출력은 보존한다. 성공한 snapshot도 source 목록의 byte 보존이지 완전한 Unity 복원/참조 정상·권리 확인이 아니다. `includeSdkPin`은 후보에만 SDK 9.0.315/rollForward disable/allowPrerelease false의 고정 global.json을 생성한다. 원래 global.json·선언/lock·자산은 변경하지 않는다.

`verify(plan)`는 계획 해시·target의 파일/폴더 전체 목록과 byte hash를 확인한다. snapshot 직후의 정확한 목록 검사이므로 Unity/새 Git 초기화·설치 후에는 그 출력까지 검토한 새 비교 입력을 준비한다. `compare(before, actual, changes)`는 명시한 before/after 해시(없음은 null) 외 추가/누락/변경을 보고한다. npm 경로 이식 의미 검증은 기존 restore.validateMigration, GUID/Importer·직접/이름 참조는 기존 audit/adoption/preservation을 함께 사용한다. 새 비교 함수 자체가 NuGet graph/Unity 타입/subasset 의미를 해석하지는 않는다. 실제 새 graph/feed/EmbeddedResource는 Step 6에서 파싱/검토한 결과와 해시에 연결한다.

`rollbackPlan(before, attempt, current, verifiedBackupHashes)`는 검증된 보존본에서 관측한 해시 목록을 받아 원복 계획만 반환한다. backup mismatch·다른/늦은 변경은 차단하고 새 출력은 retain-new-output으로 남긴다. 함수가 backup payload 진위/사용자 승인을 인증하거나 실제 삭제/원복을 실행하지 않는다. `assessResults(required, results)`는 name/inputSha256에 결합한 executed/exitCode/unexpectedLogCount를 검사한다. 필수 결과 누락/다른 입력/nonzero/예상 밖 로그를 성공으로 바꾸지 않으며 payload/로그 원문은 받지 않는다.

도구는 권한을 부여하지 않는다. 실제 대상·쓰기 권한·저장/계정 분리를 호출 전에 확정하고 Step 4 직전 다시 확인한다. link/내용/목록 변경은 전후 guard와 해시로 검출하지만 적대적 프로세스의 모든 filesystem race를 원자적으로 봉쇄하는 도구는 아니다. 실패하면 원래 작업본·부분 출력과 근거를 보존하고 다시 검토한다. Test의 재귀 정리는 확인한 OS 임시 mkdtemp fixture에만 적용하며 원래 workspace·확정 후보에는 적용하지 않는다.
