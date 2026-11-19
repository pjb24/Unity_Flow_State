# 작업 정보

## 작업명

Prototype 10 Phase 2 — 형상 관리 제외와 애셋 도입·복원 절차

## 작업 일자

20261119

## 작업 담당자

- AI: 기준선/자료 대조, 영구 절차와 검사 도구·Unit Test 구현, 정적/격리된 오프라인 검증, 필요 사용자 작업 최소화·결과/Phase 3 인계.
- 사용자: 실제 후보의 취득/정책 중 확인 불가 정보, 필요한 실제 Unity Compile/Test Runner·Scene/Prefab 적용·빌드, 구체적으로 필요한 의존성 취득/설치 실행.

## 작업 상태

**실행 절차 문서 작성 완료. Step 1~10은 미착수이며 Phase 2/Roadmap 10은 미완료다.** 이번 작업은 이 Task와 다음 작업 링크의 작성이다. 새 도구/정책·Git 제외·원본 보관/재도입·설치/복원 성공으로 기록하지 않는다.

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

- [ ] 현재 입력/보존/허용 변경 목록과 Phase 2/3 경계를 확보했다.

## Step 2. 실제 기록과 경계 정책을 자동 검사한다

2-1. 기존 관리 기록/빈 형식을 확장해 실제 항목의 필수 필드·ID/버전·근거/미확인·상태 전환을 검증하는 parser/순수 함수를 만든다. 현재 실제 제품 항목은 비워 두고 정상/실패 예제는 합성 fixture에서만 사용한다.

2-2. 제품/조건/사용처의 단일 정답은 관리 기록에 둔다. 기계용 경계가 필요하면 `Tools/DependencyAudit/dependency-policy.json`을 제안 경로로 사용하되 ID와 경로·역할/유지/제외·meta/재현 근거 참조만 넣는다. 제품/버전·license/권리 값을 다른 register에 중복 수동 관리하지 않는다. schema/정책 충돌·등록 미연결은 실패로 판정한다.

2-3. 원본·수정/파생·자체 연결·혼합/미확인과 자산/폴더 meta를 경로별로 나눈다. 미확인·변경본/설정에 제외/적용 상태를 줄 수 없고, 근거/필요 결정 누락을 보류로 출력한다. 향후 정책 변경 시 `adoption/rights/boundary/restore`가 자동 성공으로 연쇄 전환되지 않게 한다.

2-4. 누락/중복 ID/필드·잘못된 표/escape/개행·잘못된 상태·계정/credential URL·정책 충돌·빈 등록부·권리 미확인/사용자 결정 전후를 실제 parser/판정 함수의 Unit Test로 확인한다.

- [ ] 단일 기록과 경로 정책의 연결을 검증하고 빈 실제 등록부·미확인 상태를 올바르게 처리했다.

## Step 3. 원본 보관·프로젝트 복사·활용/확장 절차를 정한다

3-1. Implementation Rule의 외부 애셋 기준을 영구 실행 절차로 확장한다. 후보 등록→공식 버전/조건/해시 확인→비추적 원본 보관→선택한 범위 도입→GUID/Import/참조 대조→게임 사용처 기록→Git 검사 순서를 적는다. 공급자가 부여한 조건을 확인하며 프로젝트가 임의 license를 채택/재부여하지 않는다.

3-2. 도입 경로 `Assets/ThirdParty/<공급자>/<제품>/`, 자체 변형/연결 자료 `Assets/ProjectAssets/<asset_id>/`, Unity 밖 비추적 보관 위치의 실제 사용 규칙을 정한다. 새 폴더 생성이 필요한 때만 만들고 현재 없는 제품용 빈 폴더/Scene을 채우지 않는다. 같은 GUID의 두 버전 동시 임포트·파생물의 원본 GUID 복제를 막는다.

3-3. 공급 원본을 직접 고치는 대신 프로젝트 소유 Material preset/설정·연결 애셋/확장 코드 또는 재현 가능한 변경 정보를 분리한다. 외부 소스/변경본의 권리·기술적 분리가 불명확하면 보존/보류한다. 필요한 Attribution·빌드 배포 위치와 Resources/Shader 이름·사용 목적/게임 위치를 기록한다.

3-4. 경로/구성요소 매핑·중복 GUID·선택 범위 누락·원본에 쓰기·사용처 미기록·허용 불명 수정본의 제외를 합성 metadata/임시 파일로 검사한다. 실제 Scene/Prefab 연결이 생기면 사용자 적용표와 해당 Unity suite를 후속 Step 9에 연결한다.

- [ ] 누가 어느 경로에 무엇을 도입/분리·기록하는지 실제 순서와 실패 조건으로 정리했다.

## Step 4. 기존 TMP 변경본과 설정의 손실 방지 수단을 준비한다

4-1. 현재 폰트 변경본 2개와 TMP Settings/Material/atlas·자산/폴더 meta·직접/이름 참조의 실제 해시를 재대조하고 보존/재취득/재적용할 경로 표를 만든다. 원인/권리를 추측하거나 같은 공급 archive만으로 변경본 재현을 보장하지 않는다.

4-2. 현재 작업본을 건드리지 않는 **비추적 보존본** 또는 검증 가능한 재적용 방법을 정한다. 로컬 보관 예시는 `Ignore/DependencyPreservation/<식별자>/<해시>/`이며 실제 정책/ignore와 경로 충돌을 먼저 검증한다. Assets 밖에 보관하고 원본/Secret을 Git/문서/Test에 복제하지 않는다. 필요한 보조 기능은 `Tools/DependencyAudit/dependency-preservation.cjs` 등을 제안 경로로 분리한다.

4-3. 보존 기능이 필요하면 명시적 입력·출력과 사전 점검/적용을 분리한다. 원본 불변·기존 출력 덮어쓰기 거절·외부 경로/junction 거절·파일/meta/해시 연결·부분 복사 실패를 처리한다. 기존 원본을 먼저 옮기거나 지워 보존 여부를 시험하지 않는다. 재적용 검증은 별도 임시 대상에서 수행한다.

4-4. 자체 Test에서 누락/오염된 보존본·잘못된 버전/GUID·동일 ID 다른 내용·권한/중단·설정/Material 누락·동적 이름 변경을 검출한다. 실제 로컬 보존본을 만들 경우 전후 hash·비추적/ignore·원본 불변을 확인한다. 로컬 보관 성공은 독립 복원 성공이나 다른 환경에 대한 원본 배포 허용이 아니다.

- [ ] 변경본/설정·meta를 손실 없이 다시 준비할 입력과 실제 보존/임시 재적용 검증 근거가 있다.

## Step 5. Git 원본 유입 검사와 필요한 제외 규칙을 적용한다

5-1. 현재 inventory와 정책을 연결해 HEAD/index/staged의 원본·미등록/혼합·보존 대상 파일을 판정한다. `.gitignore`/중첩 예외를 `check-ignore --no-index`로 읽고 tracked+ignore·`git add -f`·rename/copy·삭제·대소문자·폴더/root meta를 구분한다. 실제 index는 검사에서 변경하지 않는다.

5-2. 별도 `--check-staged`/`--check-managed`는 **제안 인터페이스**다. 구현 후에만 사용한다. 유입/정책 gate와 일반 참조 finding을 별도 결과로 출력해, 원본 유입 검사 통과가 전체 참조 정상/권리·복원 PASS로 보이지 않게 한다. 중복 GUID·정책 미연결 등 실제 판정 불가능은 성공한 빈 집합으로 처리하지 않는다.

5-3. 등록된 원본 범위의 강제 staging과 알려진 원본 hash/GUID의 외부 경로 복사·rename도 검사한다. 미등록 ThirdParty 범위는 차단하고 자체 보존 예외에는 명확한 근거를 요구한다. 알려지지 않은 외부 원본을 이름/hash만으로 완전 식별할 수 있다는 보장은 하지 않는다. scope 밖/출처 불명 후보는 한계·필요 확인을 드러낸다.

5-4. `.gitignore` 보완이 필요할 때만 최소 변경한다. 파생/연결 자료와 필요한 meta를 통째로 무시하지 않는다. 현재 추적 해제 대상은 0개이므로 임의 `git rm --cached`를 실행하지 않는다. 새 추적 원본이 발견되면 정확한 목록·재취득/참조/변경본 보존 근거와 작업 범위를 먼저 확정하고, 원본 삭제/이력 재작성과 구분한다.

5-5. 실제 현재 repo에서는 read 검사와 staged 정책 결과·차이를 기록하고, mutation/ignore/예외 실패 사례는 임시 Git 저장소에서 Unit Test한다. 사용자 작업본에 합성 파일을 stage하거나 임의 hook/전역 Git 설정을 설치하지 않는다.

- [ ] 현재 추적/staged 원본 유입과 자체 보존 경계를 검사하며 필요한 최소 규칙만 적용했다.

## Step 6. 누락/버전 불일치 검사와 재취득·복원 단계를 만든다

6-1. ID/요청·해결 버전/source·공식 취득 위치/확인 상태·Editor·공급 archive hash·GUID/Import와 필요한 재적용 정보를 검사한다. 누락/버전·해시 불일치·권리/계정/오프라인 제한을 구체 원인/필요 작업으로 출력한다. 재취득 불가를 대체 버전 자동 설치로 숨기지 않는다.

6-2. UPM/builtin 준비 후 TMP archive를 **provider ID/버전 metadata로 발견**하고 도입·변경본/설정 재적용 순서를 정한다. npm local tgz 27개도 Cloud Code provider/버전·archive hash로 찾는다. 현재 cache suffix/Windows 절대 경로를 고정 복원 계약으로 쓰지 않는다.

6-3. npm의 이식성은 안정적인 비추적 archive 준비 경로와 manifest/lock 경로 연결로 해결안을 구체화한다. mapping/제안 diff는 먼저 읽기 전용으로 생성하고 패키지 버전/alias/integrity·동일 archive를 검증한다. 선언/lock 실제 변경이 필요하면 허용 경로/예상 diff·원복을 정한 뒤 적용한다. lock 재생성이 필요하면 그 도구/조건과 필요한 사용자 실행을 지정하며, 임의 version 변경·부분 lock 편집으로 성공 처리하지 않는다.

6-4. NuGet csproj/EmbeddedResource·LogSink 선언은 Unity 원본 제외와 별개로 유지한다. source/feed·해결 graph 확보 방법·동봉 조건·Node/container 입력을 절차로 남긴다. AI가 실제 npm install/ci·dotnet restore·서비스/Docker·원격 게시/빌드를 실행하지 않는다. 실제 필요한 설치/lock 생성은 Step 9 사용자 실행으로 분리하고 독립 checkout 실행은 Phase 3에 둔다.

6-5. cache 경로 suffix 변경·provider 중복/누락·잘못된 버전·archive 오염·상대 경로/플랫폼 separator·alias/lock 불일치·네트워크/계정 제한을 합성 fixture/주입 adapter로 검사한다. manifest/lock은 보존하고 변경 전 기준선과 의도된 경로 변화만 구분한다.

- [ ] 필요한 입력 누락/불일치를 식별하고 같은 의존성을 적법하게 다시 준비할 구체 절차/검사 근거가 있다.

## Step 7. 업데이트·제거·원복과 기록 갱신을 연결한다

7-1. 같은 ID의 version/source/license·hash·Import/meta·직접/동적 사용처가 바뀌는 순서를 정의한다. 업데이트 전 현재 재현/보존 입력을 남기고, 새 조건/참조/검증 상태를 다시 확인한다. 과거 rights/restore 상태를 새 버전에 자동 승계하지 않는다.

7-2. 제거 전 Scene/Prefab/Material/Script·Resources/Shader/asmdef·동적/binary 한계와 Attribution 영향을 조사한다. 사용처/권한·재복원 제약을 확인할 수 없으면 보존/보류한다. 등록부에는 폐기/변경 이력과 필요한 재복원 정보를 남기며, 실제 자료 삭제를 기본 자동화하지 않는다.

7-3. 실패/중단 시 복구할 원본/설정/meta·선언과 적용 순서를 적는다. 다른 변경을 덮는 저장소 전체 reset/clean·cache 제거로 원복하지 않는다. 원복·부분 적용 실패·GUID/이름 불일치·늦은 설정/조건 변경을 같은 판정 함수의 Unit Test로 검증한다.

- [ ] 정상 도입뿐 아니라 업데이트/제거/실패 시 보존·원복·등록 갱신 절차가 정리됐다.

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

- [ ] 필요한 정상/실패 Unit Test와 실제 적용 범위의 검사·전후 보존 근거가 있으며 도구 결함을 해결했다.

## Step 9. 필요한 사용자 작업과 실제 Unity 검증만 연결한다

9-1. 자동/정적으로 해결 불가하고 실제 적용을 막는 항목만 `대상/사실/부족한 정보/선택/영향`으로 제시한다. 기존 보존 기준으로 진행할 수 있으면 재확인을 요구하지 않는다. 필수 결정은 답변을 기다리고 무응답을 승인으로 바꾸지 않는다.

9-2. 문서/Tools·metadata만 바뀌고 생산 자산/설정·C# 변화가 없으면 불필요한 Compile/Runner/Build를 요청하지 않는다. C# Test·Import/연결 자산/설정 변화나 실제 타입/참조 해석이 필수면 최소 읽기 전용 EditMode/관련 PlayMode suite를 준비하고 사용자가 실행한다.

9-3. Scene/Prefab은 AI가 수정하지 않는다. 필요할 때 Hierarchy/컴포넌트/필드·기존→최종값·onClick/Navigation·저장/원복·정적 전후 확인표를 작성한다. UI 동적 생성/자동 참조 보충으로 검증을 우회하지 않는다. 사용자는 지정한 실행 결과/정확한 실패·예상 밖 로그만 전달한다.

9-4. 실제 lock 생성/설치가 Phase 2 완료에 필요하면 최소 명령·경로·환경·변경/원복·성공/중단 기준을 먼저 작성하고 해당 사용자 결과를 연결한다. 서비스/Module 배포·복구나 독립 복원 전체를 이 Step에 섞지 않는다. 필수 미실행/실패가 남으면 Phase 2 미완료다.

- [ ] 필요한 결정/사용자 실행은 연결됐거나 불필요 근거가 있고 필수 실패/미실행을 완료로 표시하지 않았다.

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

- [ ] Phase 2의 실제 판정과 Phase 3 입력/보존·성공/중단·미확인 의무를 정확히 기록했다.

# 영향 범위

이번 변경은 **본 Task 작성과 Roadmap 10/공통 안내/Project Memory·Phase 1 후속 작업의 링크**다. 기존 미커밋 Phase 1 문서/metadata/Tools를 보존한다. 실제 등록/정책·도구/Test·ignore/index·원본/meta·Scene/Prefab·package·보관본/저장/Secret·서버/후보를 바꾸지 않는다. 위 Step별 실제 실행 후 변경/이유와 검증 결과를 해당 Step에 기록한다.

# 검증 내용

- 템플릿 섹션·Step 1~10 순서/미체크·담당·실제 입력/출력·완료 조건과 Roadmap 3조건의 연결 확인.
- 현재 존재하는 파일/명령과 제안 경로/옵션을 구분하고 상대 링크·날짜/번호·인코딩/공백 확인.
- 실제 애셋 없음·기존 변경/미확인 보존·유입 검사와 참조/권리/독립 복원의 독립 판정 확인.
- 정상/실패 Unit Test·격리/읽기 전용·명시적 보존/적용 범위와 필요한 사용자 작업·Scene/Unity 경계 확인.

# 검증 결과

작성 시 Phase 1 최종 인계·Roadmap·관리 기록/형식·규칙·현재 CLI를 읽기 전용으로 대조했다. 관련 문서 5개·로컬 상대 링크 106개·일반 Task 템플릿의 필수 섹션 13개, Step 1~10 순서·완료 0개/미완료 10개와 같은 날짜 번호 01~05의 중복 없음을 확인했다. 제안 정책/보존 helper는 아직 없고 기존 CLI/도구·실행 파일은 변경하지 않았다. 기존 기준 입력 66개·HEAD/index/QA 후보 보존·생산/Tools 실행 파일 변경·staged 변경 0개와 UTF-8/공백, `git -c core.safecrlf=false diff --check`를 확인했다. 기존 Unit Test/검사 결과는 선행 근거로만 연결했으며 같은 suite를 다시 실행하지 않았다. 실제 Step 1~10·새 Unit Test/CLI·원본 보존/추적 해제·Unity/설치·독립 복원은 미실행이다.

# 후속 작업

**Phase 2 Step 1 기준선/허용 변경 범위 확정부터 시작한다.** 현재는 절차 문서만 작성했으며 사용자 수동 작업은 없다.

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
- 문서 작성만 완료하며 Step 실행/Phase 2·독립 복원·Roadmap 전체 완료를 미리 기록하지 않는다.
