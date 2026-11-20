# 작업 정보

## 작업명

Prototype 10 Phase 3 — 별도 환경의 독립 복원 검증과 후속 애셋 도입 인계

## 작업 일자

20261120 — 실행 계획 작성일. 실제 Step 수행일/환경/입력은 각 결과에 기록한다.

## 작업 담당자

- AI: 기준선·경로/권리 근거/필수 입력 대조, 정적 검사·Node Unit Test/복원 보조 도구 준비, 안전 전달/사용자 적용표·최소 Unity Test 준비, 실제 결과 판정/기록.
- 사용자: 정적으로 확인할 수 없는 실제 취득/계정·조건 정보, 별도 환경의 필요한 권한/사용자 데이터 분리, 실제 Unity Editor/Compile/Build/Test Runner·Scene/Prefab 적용과 필요한 설치/lock 생성 실행.

## 작업 상태

**Task 문서 작성 완료. Step 1~10은 모두 미착수다.** Phase 1~2는 완료했으며 Phase 3·독립 복원·Roadmap 10 전체는 미완료다. 아래 명령은 실행 단계의 안내이고 이번 문서 작성에서 수행하지 않았다. 이번 요청은 별도 환경 생성/복사·자산 적용·Unity·설치·배포 실행을 의미하지 않는다.

# 작업 목적

원래 작업본·변경본/보존본·QA/저장/Secret을 유지하면서, 기존 Library/node_modules/obj 전체를 복제하지 않는 별도 환경에서 같은 의존성과 프로젝트 자료를 준비하고 필수 참조/Import 설정을 검증한다. 정적 검사와 Unit Test로 해결 가능한 작업은 AI가 수행하고, Unity 해석/실제 취득/설치처럼 필요한 실행만 사용자에게 연결한다.

현재 신규 Third Party 제품은 0개다. 가상 제품/권리 확인을 만들거나 복원 검증을 위해 제품을 구매하지 않는다. 기존 UPM/TMP·서버 보조 의존성을 기술 기준선으로 검증하고, 이후 Roadmap 11~14 실제 애셋 도입에 재사용할 절차/제약을 인계한다.

# 작업 대상

- 원래 작업본: `C:\Unity\Unity_Flow_State` — 생산 자산/설정·선언/lock·QA/저장/Secret·HEAD/index 보존.
- 별도 환경: Step 2에서 구체적인 `candidateRoot`·사용자 데이터/출력 경로·권한을 확정. 아직 생성하지 않았다.
- Phase 2 도구/정책·TMP schema 2 보존본·고정 archive·최신 문서/미커밋 파일과 실제 복원 후보.
- Task metadata/필요한 보조 도구·Unit Test, 조건부 사용자 Unity Test/Scene 적용표와 최종 판정.

# 작업 전 상태

- [Phase 2 Task](20261119_05_Phase2ManualSteps.md) Step 1~10 완료(2026-11-20), [Phase 3 인계](20261120_02_Phase3Handoff.md)와 [최종 기준선](20261120_03_Phase2Step10Verification.json)을 받았다.
- 기준선은 안전 hash 1,046파일·별도 환경의 안전 전달 준비 목록 961파일·그 밖의 추적 경로 11개다. 이 수치는 선행 관측값이며 실행 시 건수를 고정해 통과시키지 않는다. 미커밋/미추적 도구/문서가 있으므로 HEAD만 checkout하면 현재 결과가 빠진다.
- Editor 6000.3.5f2, 현재 UPM 61개/edge 91개, com.unity.ugui 2.0.0의 TMP 공급 archive와 schema 2 보존본 83파일/폴더 meta 8개, Cloud Code 2.10.4의 npm tgz 27개가 고정 근거다. 개별 hash·alias/version/integrity는 기존 spec/metadata를 참조한다.
- URP 11 GUID/15회·TMP graph 1 GUID/16회 미해결과 sample orphan 1개가 남아 inventory는 수집 완료의 exit 1이었다. 정적/Unit Test 통과를 이 참조의 실제 정상으로 승계하지 않는다.
- 실제 npm 경로 이식·새 lock/설치·NuGet 새 restore/lock·별도 환경의 Unity 검증은 미실행이다. 기존 local cache/obj·과거 Player/Runner 성공은 이번 독립 복원 성공이 아니다.

# 조사 내용

## Roadmap 완료 조건과 연결

| Phase 3 완료 조건 | 실제로 확보할 증거 |
| --- | --- |
| 필요한 외부 입력 도입 후 필수 참조 누락 없이 복원됨 | Step 2 필수 입력/조건, Step 4~6 별도 환경 준비/실제 의존성 결과, Step 7 참조/Import 대조, Step 8 필요한 사용자 Unity 결과 |
| 원본 비추적과 프로젝트 소유 자료 보존 | Step 1 원래 기준선, Step 3 안전 경로/보존 Test, Step 4 전달 목록과 후보 Git 구성, Step 9 실제 HEAD/index/staged·원래 자료 전후 검사 |
| 복원 제약/수동 단계와 결과 기록 | Step 2 부족 입력, Step 5~8 실제 사용자 명령/적용표·결과, Step 9 재현/원복 증거, Step 10 영구 기록·후속 도입 인계 |

## 현재 도구와 추가 구현의 구분

`Tools/DependencyAudit`의 audit/registry/adoption/preservation/ingress/restore/lifecycle 모듈과 7개 Node suite는 존재한다. 기존 README/CLI를 재사용한다. 계획에 필요한 **안전 snapshot 전달·허용 변경 비교·후보 실행 결과 집계는 아직 통합 구현/검증되지 않았다.** Step 3에서 기존 함수로 가능한지 먼저 확인하고 필요한 부분만 구현한다. 새 옵션/파일/명령은 구현/감사/Test 후에만 실행 명령으로 기록한다.

audit/registry/ingress/lifecycle에는 `--root`가 있지만 preservation/restore CLI에는 없다. 후자는 복사된 도구의 위치로 root를 정하거나 기존 API의 명시적 root를 쓰는 검증된 adapter가 필요하다. 없는 `--root`/apply 옵션을 제안 명령처럼 실행하지 않는다. preservation의 기존 reapplyToTemporary는 제한된 OS 임시 폴더용이며 실제 candidate의 Assets 재적용 도구가 아니다.

# 작업 내용

아래 Step을 순서대로 수행한다. Step이 크면 해당 Step 안에서 `3-1`, `3-2`처럼 나누고 결과/미실행 조건을 개별 기록한다. 단계 완료는 필요한 실행/입력·검사와 결과 근거가 모두 있을 때만 표시한다.

## Step 1. Phase 3 시작 기준선과 원래 작업본 보호 범위를 고정한다

1-1. AI가 Phase 2 최종 JSON 자체의 SHA-256을 새 metadata에 고정하고 parent/check/handoff·이전 metadata·도구 hash 연결을 대조한다. 기존 JSON은 덮어쓰지 않는다.

1-2. `restorationBaseline.originalWorkspace.files`·HEAD/index/tracked/staged·ignore·QA 실행 파일·보존본/archive를 현재 상태와 비교한다. 이번 계획 문서/연결 문서의 의도된 변경은 별도 경로/hash·이유로 기록하고, 예상 밖 변경은 사용자 작업을 덮어쓰지 않고 분리한다. 새로운 문서 상태를 과거 baseline에 넣어 검사 실패를 숨기지 않는다.

1-3. 민감 본문/실제 저장은 수집하지 않는다. QA 전체 디렉터리는 기존 실행 파일 hash 범위와 구분한다. 원래 생산 경로·선언/lock·원본/변경본·저장/Secret·HEAD/index의 불변 계약과 허용할 Task/보조 도구 변경 경로를 고정한다.

**산출물:** 새로운 Phase3 Step1 baseline/차이 metadata(실제 실행일의 Task 경로), 현재 대상/입력·범위/제외 목록. **완료:** 기준선 비교와 의도/예상 밖 변경을 구분하고 필수 입력 손실이 없다.

- [ ] 원래 작업본의 현재성·보호 범위와 새 Step 1 기준선을 고정했다.

## Step 2. 전달 제외 11개·필수 입력·별도 환경/저장 분리를 판정한다

2-1. AI가 `candidateTransfer.excludedTrackedPaths`의 각 경로를 파일 역할·Git metadata·선언/EmbeddedResource/코드 참조로 조사한다. 이름 때문에 본문 차단된 일반 소스와 실제 비밀값/로컬 설정·복원 비필수 환경 파일을 구분한다. 정적으로 가능한 필요성 조사를 사용자에게 넘기지 않는다.

2-2. 공개/일반 코드의 추가 검사가 필요하면 최소 경로·읽기 정책을 검토하고 비밀값을 출력/복제하지 않는 방법을 먼저 검증한다. 기존 민감 경로 guard를 일괄 해제하지 않는다. 필요한 코드/설정 누락을 stub/자동 생성·샘플 값으로 감춰 성공시키지 않는다. 각 항목의 `필수 여부/근거/안전한 제공 방법/담당/준비 상태`를 기록하고 필수 자료가 없으면 후속 적용을 보류한다.

2-3. 고정 Editor/provider/archive와 npm/.NET toolchain·사용 조건/취득 경로·계정/네트워크/오프라인 제한을 대조한다. AI가 확인할 수 없는 실제 취득/계정/권리 정보만 `대상/확인된 사실/부족 정보/선택/영향`으로 사용자에게 요청한다. 기존 보존 결정을 반복 승인받지 않는다.

2-4. 구체적인 candidateRoot/출력/별도 저장·테스트 계정 경로와 권한을 실행 환경에서 확정한다. 원래 root와 동일/상하위 관계·junction/symlink·기존 데이터가 있는 대상은 거절한다. 현 workspace 밖 쓰기는 실제 환경의 권한을 확보한 후 진행한다. Editor 초기화/Player가 원래 저장·production 서비스/계정을 건드리지 않는 조건을 먼저 확인한다.

**산출물:** 입력 완전성/제외 경로 판정표·실제 root/환경 계약·필요한 사용자 질문. **완료:** 현재 단계에 필요한 결정/경로·필수 자료가 확정됐다. 미응답은 승인으로 바꾸지 않는다.

- [ ] 필수 입력/제외 경로와 별도 root·저장/계정 분리·권한을 확정했다.

## Step 3. 안전 전달·후보 비교 도구를 최소 구현하고 Unit Test로 검증한다

3-1. 기존 모듈의 쓰기/프로세스/네트워크·require/main을 먼저 감사한다. 기본 check/plan은 읽기 전용, 명시적 전달/재적용은 별도 API/모드로 분리한다. 원래 root·허용 밖 대상에 쓰는 입력은 실패시킨다. 보존본 verify와 실제 자산/Scene 적용을 혼동하지 않는다.

3-2. 기존 reader/경로/Git/preservation/restore 함수로 해결되지 않는 안전 snapshot/허용 변경 비교만 추가한다. 외부 package 없이 Node/assert·합성 metadata·권한/부분 읽기 주입·임시 저장소를 우선 사용한다. fixture의 Git 초기화/임시 정리는 실제 원래 작업본에 적용하지 않는다.

| 필수 Unit Test 영역 | 정상/실패 입력과 확인할 책임 |
| --- | --- |
| 경로/전달 | 상하위/절대/공백·한글·대소문자·junction/symlink·중복/parent-file·기존 비어 있지 않은 대상/overwrite·허용 밖 Secret/생산 경로 거절 |
| 입력/복사 | 누락/추가·hash 오염·실제 body 읽기 실패·부분 열거/복사·권한 거절·미커밋 파일 포함·전달 제외 필수 입력을 빈 성공으로 만들지 않음·부분 출력 보존 |
| 복원 계약 | 폴더 meta/빈 디렉터리·GUID/Importer/Resources/Shader 이름·같은 GUID의 중복 버전·원본/파생 경계·공급 경로 suffix 변경과 오염/잘못된 버전 |
| 선언/결과 | BOM/JSON·npm alias/version/integrity/graph·경로만 바뀐 후보와 예상 밖 변경·NuGet 직접/전이/feed/resource drift·사용자 결과 누락/nonzero/예상 밖 로그 처리 |
| 보존/통합 | 원래 파일/HEAD/index 불변·읽기 전용 require/check·동일 입력 결과·실패 CLI nonzero·비밀값/원본 body 미노출·before/attempt/current 부분 원복과 늦은 변경 보호 |

3-3. 재사용할 기존 명령은 아래와 같다. 실행 시 실제 건수/실패 이름·원인/exit를 기록한다. 선행 261개를 정답으로 고정하지 않는다. 추가 suite/옵션은 실제 구현 후 기록한다. 코드/판정이 바뀌면 영향 suite를 재실행하고, 변경/실패가 없는데 동일 suite를 반복하지 않는다.

```powershell
node Tools/DependencyAudit/dependency-audit.test.cjs --policy AI/90_Tasks/Prototype_10/20261119_03_Phase1Step8Policy.json
node Tools/DependencyAudit/dependency-registry.test.cjs
node Tools/DependencyAudit/dependency-adoption.test.cjs
node Tools/DependencyAudit/dependency-preservation.test.cjs
node Tools/DependencyAudit/dependency-ingress.test.cjs
node Tools/DependencyAudit/dependency-restore.test.cjs
node Tools/DependencyAudit/dependency-lifecycle.test.cjs
```

**산출물:** 필요한 최소 도구/Test·실행/부작용 감사·정상/실패 결과. **완료:** 후보 쓰기 전 필요한 경계/실패 Test가 통과하고 원래 자료 보호를 입증했다.

- [ ] 필요한 보조 도구와 정상/실패 Unit Test·읽기 전용/쓰기 경계를 검증했다.

## Step 4. 검증된 안전 목록으로 별도 snapshot을 준비한다

4-1. Step 2의 root/권한과 Step 3 도구 검증 후 안전 전달 목록을 구체화한다. Phase 2의 961개는 시작점이며 새 Task/도구/metadata와 승인된 필수 경로를 명시적으로 추가한 새 목록/hash를 기록한다. 과거 목록을 고쳐 쓰지 않는다.

4-2. 원래 .git/history·전체 Library/Ignore/Build·node_modules/obj/bin·실제 저장/Secret을 통째로 복사하지 않는다. 검사된 안전 파일과 미커밋 문서/도구를 별도 대상으로 전달하고, 보존본/archive는 필요한 제한 입력으로만 전달한다. exclusive 쓰기/전후 hash·목록·권한/부분 실패를 검사한다.

4-3. 후보에서 HEAD/index/staged를 검사하려면 후보만의 Git 구성을 명시적으로 준비한다. 실제 사용자 결정에 따라 검토된 안전 경로만 후보의 초기 추적 대상으로 구성하고 원본/보존본/archive는 비추적으로 둔다. 원래 index를 복사하거나 원래 저장소를 stage/commit하지 않는다. 후보 HEAD가 없으면 현재 Git CLI의 실패를 빈 통과로 바꾸지 않는다.

**산출물:** 실제 전달 manifest/후보 안전 입력·Git/ignore/실패 출력 상태·원래 작업본 비교. **완료:** 필수 코드/설정·미커밋 도구가 준비되고 원래 작업본 불변/원본 비추적·후보 경계가 확인됐다.

- [ ] 별도 환경의 실제 안전 snapshot·입력 완전성/Git 경계를 확인했다.

## Step 5. 고정 UPM·TMP 공급/변경본과 Import 설정을 준비한다

5-1. 소유자가 분리된 환경에서 같은 Editor 6000.3.5f2로 manifest/lock에 따른 정확한 UPM을 준비한다. AI는 설치 metadata의 ID/version·선언/graph/source를 확인하고 suffix가 다른 cache도 다시 발견한다. 원래 Library의 통째 복사나 다른 버전 자동 선택은 하지 않는다.

5-2. AI가 com.unity.ugui 2.0.0의 archive hash와 schema 2 TMP 보존본을 전달 전후 검증한다. 현재 원래 환경의 기존 검증 명령은 다음과 같다. root 결정 방식은 앞서 감사한 구현을 따른다.

```powershell
node Tools/DependencyAudit/dependency-preservation.cjs --verify AI/90_Tasks/Prototype_10/20261119_14_Phase2Step4Check.json Ignore/DependencyPreservation/tmp-baseline/29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c
node Tools/DependencyAudit/dependency-restore.cjs --check AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json
```

5-3. 대상에 이미 존재하는 파일/동일 GUID와 before 상태를 조사하고, 변경 폰트/설정·자산/폴더 meta·빈 폴더·GUID/Import/platform·Resources/Shader 이름을 재적용할 정확한 표를 준비한다. 필요한 실제 Unity import/자산 적용은 사용자가 수행하고 AI는 전후 byte/metadata·참조를 대조한다. schema 1 bundle/공급 archive만으로 변경본 재현을 선언하지 않는다. TMP bundle의 Library 경로까지 candidate에 무조건 복사하지 않는다.

**산출물:** 실제 provider/공급 hash·재적용표/사용자 결과·TMP/설정 대조. **완료:** 필요한 원본/변경본·설정/meta가 같은 버전의 후보에 준비됐고 필수 자료 손실/충돌이 없다.

- [ ] 실제 후보의 고정 UPM/TMP·변경본/Import·폴더 보존을 확인했다.

## Step 6. npm 이식 경로와 새 lock·NuGet 보조 의존성을 검증한다

6-1. AI가 Cloud Code 2.10.4의 정확한 tgz 27개를 provider metadata로 찾아 내부 name/version·alias·SHA-256/SHA-512 integrity/dependencies와 대조하고, 별도 환경의 비추적 안정 경로를 확인한다. 재취득 제한/누락이면 구체 원인으로 중단한다.

6-2. Step 2에서 고정한 npm/toolchain·후보 root에서 기존 [manifest patch](20261119_21_Phase2Step6NpmPathMigration.patch)의 적용 가능성/예상 diff·원복 입력을 AI가 검사하고, 사용자가 검토된 경로 변경과 실제 lock 생성을 수행한다. 다음 명령은 **후보 root에서 필요한 시점의 사용자 실행**이다.

```powershell
npm install --package-lock-only --ignore-scripts --no-audit --no-fund
```

6-3. 후보의 실제 package.json/package-lock.json 두 JSON을 검증된 경로로 받아 원래 선언과 대조한다. 기존 CLI는 원래 root의 spec/선언을 읽으므로 원래 환경의 `Ignore/DependencyRestoreCandidate/`에 두 후보 JSON만 검토용 metadata로 준비했을 때 다음 기존 명령을 쓸 수 있다. 원래 manifest/lock은 덮어쓰지 않는다.

```powershell
node Tools/DependencyAudit/dependency-restore.cjs --verify-migration AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json Ignore/DependencyRestoreCandidate/package.json Ignore/DependencyRestoreCandidate/package-lock.json
```

6-4. 지정 경로 외 alias/version/integrity/graph·다른 필드 변화는 실패/검토 대상으로 기록한다. mapping을 손으로 적용한 lock을 실제 생성/설치 성공으로 취급하지 않는다. 필요한 사용자 `npm ci --ignore-scripts --no-audit --no-fund`의 실제 결과를 연결한다. 의도된 경로 이식 후에는 새 후보 기준선으로 검사하고 기존 spec hash를 새 파일로 덮어쓰지 않는다.

6-5. .NET 보조 의존성이 복원 범위에서 필수라면 소유자가 후보 root에서 `dotnet restore UGS/Modules/FlowStateVerification/FlowStateVerification.csproj --use-lock-file`를 실행한다. AI는 실제 graph/lock의 net9.0·직접 4개/기존 전이 13개·feed·EmbeddedResource 2개 선언/실제 source를 대조한다. local obj를 새 restore로 승계하지 않는다. LogSink의 node:22-alpine tag/digest·동봉 조건·container 실행 필요성도 분리해 기록한다. Module 빌드/배포·복구나 실제 서비스 호출은 이 복원 작업에 섞지 않는다. 비필수라면 근거와 미검증 범위를 남기고 전체 보조 체계 성공을 주장하지 않는다.

**산출물:** 실제 후보 manifest/lock·필요 설치/restore 결과·graph/경로/자원 검증·미확인/비필수 근거. **완료:** 필요한 실제 결과가 있고 필수 실패/미실행을 통과로 처리하지 않았다.

- [ ] 필수 npm/보조 의존성의 실제 경로/lock/설치 결과 또는 비필수 근거를 검증했다.

## Step 7. 직접·이름·동적 참조와 기존 진단의 실제 영향을 정적으로 판정한다

7-1. AI가 원래 자료와 복원 후보의 파일/meta 목록·GUID/중복·subasset/fileID·Importer/platform·Scene/Prefab/Material/Animation/ShaderGraph/asmdef·Resources/Shader 이름과 사용처를 대조한다. YAML만으로 Unity 타입/subasset 해석을 확정하지 않는다.

7-2. 기존 31회 참조 미해결/sample orphan을 raw 항목별로 `사용처/후보 재현/필수 여부/정적 근거/부족한 Unity 해석/필요 Test`에 연결한다. 새 미해결/누락과 기존 진단을 구분한다. allowlist·진단 삭제·필요 파일 제거로 통과시키지 않는다. 비필수 판단도 실제 범위/근거를 남기며 필수인지 모르면 보류한다.

7-3. 동적 로딩·binary·렌더링·코드/리소스 연결을 추적하고, 정적으로 판정 불가능한 구체 대상만 Step 8에 넘긴다. 실제 Scene/Prefab 연결 변경이 필요하면 AI는 읽기 전용 조사와 Hierarchy/컴포넌트/필드·기존→최종값·onClick/Navigation(해당 시)·저장/원복·정적 확인표를 준비하고 사용자에게 적용을 연결한다. 동적 UI 생성/누락 참조 자동 보충은 하지 않는다.

**산출물:** 후보 raw 참조 inventory/원래 대비 차이·필수/비필수 판정·최소 Unity 검증/적용표. **완료:** 정적으로 가능한 조사를 끝내고 필수 미확인과 사용자 검증 범위를 구체화했다. 참조 정상/Phase 3 완료 판정은 필요한 Step 8 결과 후에 한다.

- [ ] 실제 참조/Import·기존 진단의 영향과 최소 사용자 검증 대상을 구분했다.

## Step 8. 필요한 Unity Test·Editor/Build 결과만 사용자에게 연결한다

8-1. AI가 실제 변경/해석 책임에 맞는 최소 Test를 준비한다. metadata/순수 판정·파일 실패는 Node Unit Test, Unity Import/type/fileID·asset 로딩은 가능한 읽기 전용 EditMode, 실제 Scene/시스템 연결은 관련 PlayMode로 검증한다. 새 Unity Test가 자산/저장/서비스를 수정하지 않는지 정적 감사한다.

8-2. 사용자 안내에 `후보 root/Editor/선행 입력·분리 상태/Test 위치·이름/예상 결과/실패 시 중단·원복/전달할 로그`를 적는다. Build가 필수면 별도 출력 경로·타깃/설정·성공 기준과 저장 분리를 확정하고 사용자가 수행한다. AI는 Unity Editor/Compile/Build/Test Runner를 실행하지 않는다.

8-3. 사용자가 실제 Console/실패 Test 이름·오류/예상 밖 로그·실행 결과를 전달하면 AI가 해당 입력/버전에 연결한다. 영향 코드가 바뀌면 관련 Unit Test/정적 검사를 다시 하고 필요한 사용자 Test만 재요청한다. 과거 Compile/Runner/QA 성공이나 무응답을 새 복원 성공으로 바꾸지 않는다.

**산출물:** 실제 필요한 Unity Test·사용자 적용/실행표와 결과 또는 구체적인 불필요 근거. **완료:** 필수 사용자 실행이 모두 확인됐다. 미실행/실패·필수 참조/타입 미확인이 남으면 Phase 3은 미완료다.

- [ ] 필요한 실제 Unity/Scene/빌드 결과를 연결하거나 불필요 근거를 확정했다.

## Step 9. 후보 Git/복원 전체성과 원래 자료 보존·원복 가능성을 최종 검증한다

9-1. 실제 후보의 정책/등록·HEAD/index/staged·ignore와 알려진 원본 지문, 프로젝트 소유 자료 보존을 AI가 검사한다. registry/audit/ingress/lifecycle의 실제 `--root`를 사용하고 입력 pin을 해당 root에서 대조한다. 현재 worktree 정책 검사와 staged 정책 재현을 구분한다.

9-2. 전체 필수 의존성/참조·실제 설치/사용자 결과·보존본/설정 hash·원래 작업본 보호 계약을 함께 대조한다. registry/유입 gate/자료·권리/참조 건강/Unity/독립 복원은 독립 결과로 기록한다. 필수 누락/실행 실패가 있으면 다른 gate의 exit 0으로 전체 성공시키지 않는다.

9-3. 예상 실패/원복은 우선 임시 fixture Unit Test로 입증한다. 실제 실패가 있으면 이름 붙인 before/attempt/current hash와 검증된 보존본으로 필요한 목록/순서만 준비하고, 다른/늦은 변경을 덮어쓰지 않는다. 새/부분 출력·실패 로그를 유지하며 자동 삭제·전체 reset/clean·cache 청소를 하지 않는다. Scene/Prefab 원복은 사용자 담당이다.

**산출물:** 실제 후보/원래 작업본 전후 검사·실제 결과 집계·부분 원복 증거/한계. **완료:** 필수 영역이 모두 검증되고 원래 자료 손실/예상 밖 변경이 없다.

- [ ] 후보 전체 검증·원본 비추적/자체 자료 보존·원래 작업본/원복 근거를 확보했다.

## Step 10. Phase 3·Roadmap 10 완료를 판정하고 후속 도입에 인계한다

10-1. 위 Roadmap 세 완료 조건에 실제 후보 root/입력·필수 참조·사용자 Unity/설치 결과·원본 비추적/자체 자료/원래 작업본 보존·제약을 연결한다. 준비 도구/합성 Test만으로 독립 복원 완료를 선언하지 않는다.

10-2. 모두 충족한 경우에만 이 Task·Roadmap 10/공통 안내·Project Memory·관리 기록/영구 절차를 실제 결과로 갱신한다. 미충족이면 필수 남은 작업/담당·정확한 차단 이유와 보존 상태를 기록하고 완료를 보류한다. 신규 제품 0개라는 사실로 기존 기술 의존성의 필수 복원을 면제하지 않는다.

10-3. 이후 도입자가 사용할 등록→조건 확인→원본 비추적 보관→필수 파일 도입→확장/게임 사용처 기록→Git/참조 검사→업데이트/제거/원복→독립 복원 절차와 실제 버전/source/hash·취득/오프라인·수동 단계를 인계한다. Roadmap 11~14 실제 원본 도입은 Roadmap 10 전체 완료 뒤에 진행한다. 다음 실제 Module 업데이트 전 복구 시험 의무도 유지한다.

**산출물:** 세 조건 최종 판정·Task/영구 문서·후속 도입 점검표/남은 의무. **완료:** 실제 결과와 문서 상태가 일치하고 필수 실패/미실행·원래 자료 손실을 성공으로 표시하지 않았다.

- [ ] 실제 증거로 Phase 3/Roadmap 10 완료 여부와 후속 도입 인계를 판정했다.

# 영향 범위

현재 요청은 이 실행 계획과 연결 문서/작성 검증 metadata만 변경한다. 실제 Step 수행 시 원래 생산 자산/설정·Scene/Prefab·선언/lock·저장/Secret·HEAD/index는 보존하며, 구체화한 보조 도구/Task metadata와 별도 후보에만 허용된 변경을 적용한다. 실제 Unity/설치·원격/서비스 배포·cache 정리는 이번 문서 작성에서 수행하지 않는다.

# 검증 내용

- 일반 Task 필수 13개 섹션·Step 1~10 순서와 각 산출물/완료 조건·담당/중단·보존 계약을 확인한다.
- Roadmap 세 조건과 실제 후보 준비·정적 검사/Unit Test·필수 사용자 실행/참조/원복·최종 판정의 연결을 확인한다.
- 현재 존재하는 CLI/옵션과 미구현 제안을 구분하고, 실제 source/spec/경로와 상대 링크·날짜/번호·UTF-8·공백을 확인한다.
- 문서 작성 전후 Phase 2 기준선·생산/도구·이전 metadata·HEAD/index 보존을 확인하고 의도된 연결 문서 변경만 허용한다.

# 검증 결과

작성 검증은 [Task 작성 검증 metadata](20261120_05_Phase3TaskVerification.json)에 기록한다. Phase 2의 현재 hash 기준선과 source/기존 CLI를 확인한 문서 준비 결과다. 선행 Node Test 261개는 과거 증거이며 이번에 재실행하지 않았다. Step 1~10·새 도구/Test·별도 snapshot/설치·Unity·독립 복원은 미실행이다.

# 후속 작업

**다음은 Step 1 실행**이다. 현재 문서 작성에 필요한 사용자 수동 작업은 없다. 실제 후보 위치/권한·필수 정보는 Step 2에서 정적 준비 후 필요한 것만 요청하고, 사용자 설치/Unity는 해당 Step에서 구체 절차를 준비한 뒤 연결한다.

# 관련 문서

- [AI 진입점](../../README.md)
- [Project 안내](../../00_Project/README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [관리 기록](../../00_Project/ASSET_DEPENDENCIES.md)
- [등록 형식](../../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)
- [Roadmap 10](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_010.md)
- [공통 진행/검증 경계](../../04_Implementation_Roadmap/README.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [구현 규칙](../../01_Rules/IMPLEMENTATION_RULE.md)
- [검증 규칙](../../01_Rules/VERIFICATION_RULE.md)
- [일반 Task 형식](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
- [실제 도구/명령](../../../Tools/DependencyAudit/README.md)

# 관련 작업 기록

- [Phase 2 최종 Task](20261119_05_Phase2ManualSteps.md)
- [Phase 2 전체 Unit Test/현재 검사](20261119_26_Phase2Step8Check.json)
- [필요 사용자 작업/미실행 경계](20261119_28_Phase2Step9Check.json)
- [Phase 3 인계](20261120_02_Phase3Handoff.md)
- [Phase 2 최종 검사/안전 전달 기준선](20261120_03_Phase2Step10Verification.json)
- [Roadmap 9 QA/운영·복구 의무](../Prototype_9/20261118_01_Phase4ManualSteps.md)

# 작성 완료 기준

실제 작업 순서·기존 도구/추가 구현·정적/Unit Test·필수 사용자 작업과 산출물/완료/중단/보존 조건을 Step으로 표현했다. 문서 작성 완료와 실제 Step·Phase 3/독립 복원·Roadmap 전체 완료를 구분한다.
