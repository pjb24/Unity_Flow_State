# 작업 정보

## 작업명

Prototype 10 Phase 2 최종 인계 — Phase 3 독립 복원 입력·보존·실행 계획

## 작업 일자

20261120

## 작업 담당자

- AI: 안전한 입력/해시·경계/참조 정적 대조, 최소 사용자 적용표/Test 준비, 실제 결과 판정.
- 사용자: 필요 취득/조건 확인, 별도 환경 준비·Unity/설치 실행·Scene/Prefab 적용과 실제 결과 전달.

# 작업 목적

Phase 2의 관리/보존·유입/누락/버전 검사·도입/확장/lifecycle 절차를 Phase 3의 실제 독립 복원에 인계한다. **이 문서는 실행 계획이며 Phase 3 착수/복원 성공 기록이 아니다.** 현재 사용자 실행은 요청하지 않는다. Phase 2 완료는 [원 Task Step 10](20261119_05_Phase2ManualSteps.md), 현재 검사와 고정 입력은 [Step 10 검사](20261120_01_Phase2Step10Check.json), 최종 새 기준선은 [최종 검증](20261120_03_Phase2Step10Verification.json)을 따른다.

# 작업 대상

원래 작업본 `C:\Unity\Unity_Flow_State`, 그와 분리된 향후 복원 환경, 고정 Editor/선언·lock/공급 archive·TMP 변경본/설정·GUID/Import/사용처와 기술 의존성이다. 신규 도입 제품/제외 후보는 0개이며 가상 제품을 등록하지 않는다.

# 작업 전 상태

Phase 1 완료(2026-11-19), Phase 2 관리 절차/도구·정상/실패 검증 완료(2026-11-20), Phase 3 실제 복원 미착수다. 원래 HEAD는 Step 10 metadata의 값이고 index/staged는 보존했다. Phase 2의 문서/도구/metadata에는 미커밋·미추적 파일이 있으므로 **HEAD만 checkout하면 최신 결과가 빠진다.** 원본 삭제/추적 해제·manifest/lock 경로 적용·새 lock/설치·Scene 수정은 수행하지 않았다.

# 조사 내용

| 입력/계약 | 고정 근거와 제한 |
| --- | --- |
| 원래 작업본 기준선 | 최종 검증의 `restorationBaseline.originalWorkspace.files`는 기존 1,007파일의 현재 hash와 새 도구/정책·이전 metadata·이번 인계 파일을 포함한다. `candidateTransfer`는 별도 환경으로 옮길 수 있는 검사된 안전 경로의 준비 목록이다. 현재 metadata 자체의 hash는 다음 실행에서 외부 증거로 고정한다. 과거 기준선을 새 상태로 덮어쓰지 않는다 |
| 선언/버전 | [복원 spec](20261119_19_Phase2Step6RestoreSpec.json)의 Editor **6000.3.5f2**, manifest/lock·보조 선언 8개 SHA-256을 보존한다. UPM 현재 graph 61개/edge 91개다. cache 폴더 suffix/개인 절대 경로는 복원 계약이 아니다 |
| 공급/TMP 변경본 | com.unity.ugui **2.0.0**의 TMP archive SHA-256은 `58564ec64cef8a8cea3d0b8ffb5e6fecf4176d544a2f6a29a2b43a0a33256431`이다. [Step 4 pin](20261119_14_Phase2Step4Check.json)의 schema 2 보존본은 83 payload 파일·meta 45개/폴더 meta 8개와 변경 폰트/Importer/설정·이름을 포함한다. manifest hash는 `29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c`이다. schema 1 보존본은 유지하되 최종 복원 입력으로 쓰지 않는다 |
| Cloud Code/npm | com.unity.services.cloudcode **2.10.4**, tgz 27개의 내부 이름/설치 alias/version/SHA-256/SHA-512 integrity/graph는 spec과 현재 검사에 고정했다. 준비된 각 경로는 `Ignore/DependencyArchives/npm/com.unity.services.cloudcode/2.10.4/<sha256>/<filename>`이다. 원래 cache 전체/node_modules를 복사해 독립 복원으로 처리하지 않는다 |
| NuGet/LogSink | .NET net9.0·직접 4개/기존 local graph 13개와 EmbeddedResource 2개 선언을 보존한다. 기존 obj는 새 restore/lock이 아니다. LogSink 외부 npm 의존성 0개와 node:22-alpine tag만 확인했으며 image digest/접근·container 동작은 미확인이다. 실제 Module 배포/복구는 별도 작업이다 |
| 권리/취득 | 제품 등록 0개·기술 기준선 보존이며 현재 cache/고정 hash는 재취득 권리나 계정/네트워크 가용성 증거가 아니다. 취득 조건/공급원·오프라인 자료·계정 권한을 실제 필요한 시점에 소유자가 확인한다. 없는 자료를 다른 버전으로 자동 대체하지 않는다 |
| 참조 한계 | 현재 URP 11 GUID/15회·TMP graph 1 GUID/16회 미해결과 sample orphan 1개를 raw 진단 그대로 유지한다. 원본 검사 exit 1을 참조 정상으로 바꾸지 않는다. 직접 GUID/fileID·Resources 상대 이름·Shader/asmdef 및 동적/binary/Unity 타입·subasset·렌더링의 실제 영향/필수 여부를 복원 전후 확인한다 |

# 작업 내용

## 1. 원래 작업본 보호와 별도 환경 준비

실행 전에 Phase 3 Task에서 구체적인 `candidateRoot`와 별도 사용자 데이터/로그 경로를 고정한다. 원래 프로젝트 및 그 하위/상위 포함 관계·junction/symlink·기존 프로젝트/데이터가 있는 경로는 대상에서 제외한다. 현 작업의 쓰기 허용 범위는 원래 workspace뿐이므로 별도 대상 생성/쓰기 권한은 Phase 3 실행 환경에서 확보한다. **이번 작업에서 별도 폴더 생성/clone/copy나 기존 파일 이동은 하지 않았다.**

원래 작업본·TMP 변경본/보존본/archive·QA Candidate02·원래 저장/Secret은 보존하고 Phase 3의 쓰기는 별도 환경과 새 Task metadata로 한정한다. QA는 실행 파일 hash만 확인했으며 전체 디렉터리 검증/새 빌드가 아니다. 원래 저장 경로/실제 계정·서비스를 재사용하지 않고 분리 방법을 먼저 확인한다. 분리할 수 없으면 Player/서비스 연동을 시작하지 않는다.

최종 기준선의 `candidateTransfer.files`에 있는 검사된 안전 파일로 새 snapshot을 준비하는 방식을 사용한다. 최신 미커밋 문서 8개와 새 Tools/정책·metadata도 포함하고 경로/byte hash를 대조한다. 원래 `.git`/전체 history·Ignore/Library/Build·node_modules/obj/bin·사용자 저장/Secret을 통째로 복제하지 않는다. 새 저장소의 HEAD/index 구성은 Phase 3의 구체 범위로 정하고 현재 저장소를 stage/commit/reset/clean하지 않는다.

기존 추적 경로 933개 중 최초 안전 hash 기준선 밖 경로는 **11개**다. 최종 기준선의 `candidateTransfer.excludedTrackedPaths`는 그 경로 metadata만 기록한다. 본문 미검사/민감 경로 차단 및 조사 범위 제외를 구분해 필요성·안전한 제공 방법을 Phase 3에서 확인하고, 빌드/리소스에 필수인데 준비되지 않았으면 중단한다. 누락 코드를 대체 생성하거나 제외 상태를 완전한 checkout/복원 성공으로 처리하지 않는다. 원본/Secret을 인계 문서/Test/Git에 복제하지 않는다.

## 2. 고정 공급 입력·보존본 검증과 의존성 준비

소유자는 필요한 합법적 취득/계정·네트워크/오프라인 입력을 준비한다. AI는 공급 metadata ID/version·archive hash를 대조한다. 원래 Library의 전체 복사 대신 별도 환경에서 같은 Editor로 정확한 UPM을 준비하고 provider를 다시 발견한다. Editor 실행은 사용자 담당이다.

TMP 보존본과 tgz는 공개/커밋 없이 접근을 통제한 별도 입력으로 전달하고 전달 전후 hash를 비교한다. TMP bundle은 아래 명령으로 현재 원래 환경에서 확인할 수 있다. 별도 환경에서는 그 환경의 정확한 root에서 검증하고 실제 Assets 재적용 전 사용자 적용표를 작성한다. 현재 provider cache 경로를 가진 Step 4 spec을 새 경로로 자동 덮어쓰지 않는다. 공급 archive만으로 변경 폰트/설정의 재현을 주장하지 않는다.

```powershell
node Tools/DependencyAudit/dependency-preservation.cjs --verify AI/90_Tasks/Prototype_10/20261119_14_Phase2Step4Check.json Ignore/DependencyPreservation/tmp-baseline/29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c
node Tools/DependencyAudit/dependency-restore.cjs --check AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json
```

두 명령의 현재 기대 exit는 0이며 check는 복사/설치를 하지 않는다. 원래 spec은 원래 선언 hash를 고정하므로 실제 경로 이식 후에는 기존 check를 억지로 통과시키기 위해 pin을 바꾸지 않는다. 원래 환경에서 실제 후보 manifest/lock을 `--verify-migration`으로 비교한 후 별도 환경의 의도된 새 상태 기준선을 만든다.

## 3. 조건부 lock 생성/설치와 자산 적용

정확한 [npm patch](20261119_21_Phase2Step6NpmPathMigration.patch)·mapping, 명령/경로·실행 주체·중단/원복은 [Phase 2 Step 9](20261119_05_Phase2ManualSteps.md)와 [실제 도구 안내](../../../Tools/DependencyAudit/README.md)를 따른다. 별도 환경에서 사용자 manifest patch 적용→실제 lock 생성→AI의 후보 alias/version/integrity/graph·경로 외 필드 대조→필요한 사용자 npm ci 순서다. NuGet 새 restore/lock도 사용자 실행 후 실제 graph/feed/resource를 대조한다. 이번 인계는 실행 요청이나 생성된 lock/설치 성공이 아니다.

자산/설정은 적용 전 baseline·시도할 최종 byte·meta/GUID/Import/Resources/Shader 이름·사용처·허용 경로를 확정한다. Scene/Prefab 변경이 필요하면 AI는 실제 Hierarchy/컴포넌트/필드·기존→최종값·해당 onClick/Navigation·저장/원복·정적 비교표를 작성하고 사용자가 Editor에서 수행한다. 가상 적용표·동적 UI/자동 연결 보충으로 검증을 대체하지 않는다.

## 4. 검사·Unity 결과와 성공/중단 판정

현재 원래 환경의 읽기 전용 검사 명령은 다음과 같다. 다른 후보에서는 root·입력 pin·실제 새 상태를 먼저 확인한다. ingress는 현재 worktree 정책/등록부/지문을 쓰는 gate이며 새로운 HEAD/staged 정책을 재현하는 hook이 아니다.

```powershell
node Tools/DependencyAudit/dependency-registry.cjs --check
node Tools/DependencyAudit/dependency-ingress.cjs --check-managed --evidence AI/90_Tasks/Prototype_10/20261119_22_Phase2Step6KnownOrigins.json
node Tools/DependencyAudit/dependency-ingress.cjs --check-staged --evidence AI/90_Tasks/Prototype_10/20261119_22_Phase2Step6KnownOrigins.json
node Tools/DependencyAudit/dependency-lifecycle.cjs --check
node Tools/DependencyAudit/dependency-audit.cjs --check
```

현재 기대값은 앞 4개 명령 exit 0, inventory는 수집 완료·기존 raw 진단을 유지한 exit 1이다. Phase 3에서는 유입 gate/권리·입력 완전성/참조 건강/실제 설치·독립 복원 결과를 각각 기록한다. 최소 EditMode/관련 PlayMode는 실제 변경/해석 대상에 맞춰 AI가 준비하고 사용자가 Runner/Build를 수행한다. 새 C#/자산/설정 영향이 없으면 불필요한 전체 재실행을 요구하지 않는다. 실패 Test 이름·정확한 오류/예상 밖 로그·실행 환경/입력 hash를 받는다.

| 판정 | 기준 |
| --- | --- |
| 허용 변경 | 별도 candidate의 정확히 검토한 npm 경로/생성 lock·NuGet 생성 graph/lock·UPM cache/새 local 출력, TMP 보존본의 지정 자산/설정/meta, 사용자가 적용한 구체 Scene/Prefab 표와 필요한 Task metadata만 허용한다. 구체 파일/전후 hash·범위는 해당 시도 시작 전에 고정한다. 원래 작업본의 생산 파일/원본/QA/저장/Secret·HEAD/index 변경은 허용하지 않는다 |
| 성공 | 필수 자료/취득 조건이 충족되고 실제 별도 환경에서 동일 버전·meta/GUID/Import/이름/사용처와 필요한 사용자 결과를 확보한다. 필수 참조 누락/실행 실패/예상 밖 로그가 없어야 한다. 기존 URP/TMP/sample 진단은 실제 영향/필수 여부·관련 증거로 해소하거나 비필수 범위를 명시해야 하며 raw 진단을 삭제/allowlist하여 성공시키지 않는다. 해당 대상의 참조 검증이 불가능하면 그 성공은 보류한다 |
| 중단 | 필수 입력/권리·계정/오프라인 자료 부족, baseline/버전/source/hash/GUID/Import/name drift, 미확인 필수 참조/타입·binary 한계, 원래 작업본/저장 연결, 허용 밖 변경, 명령/Test 실패나 예상 밖 로그가 있으면 중단한다. 자료 누락을 빈 성공이나 임의 버전 교체로 숨기지 않는다 |
| 원복 | 별도 환경의 그 시도에서 이름 붙인 파일만 before/attempt/current hash로 대조하고 검증된 보존 byte로 되돌린다. 다른/늦은 변경이면 덮어쓰지 않고 보류한다. 새/부분 출력은 증거로 남기며 자동 삭제·reset/clean·cache 정리는 하지 않는다. Scene/Prefab 원복은 사용자가 표대로 수행한다 |

# 영향 범위

Phase 2 최종 문서·검사/새 기준선 metadata와 이 인계 계획만 추가/갱신한다. 별도 환경·Unity·Scene/Prefab·원본/meta·선언/lock·저장/Secret·서버/QA에는 쓰지 않는다.

# 검증 내용

현재 CLI 7개·고정 입력/이전 metadata·도구/정책·생산 inventory·TMP schema 2/폴더·tgz 27개·HEAD/index를 대조한다. 전체 Node suite는 Step 8의 고정 source/261개 통과를 연결하며 변경 없는 코드를 다시 실행한 것으로 기록하지 않는다. UTF-8/JSON·로컬 링크·Step 체크 상태와 안전 전달 목록·누락 추적 경로를 정적으로 검증한다.

# 검증 결과

현재 결과는 [Step 10 검사](20261120_01_Phase2Step10Check.json)와 [최종 검증](20261120_03_Phase2Step10Verification.json)을 따른다. Phase 2의 세 조건은 관리 절차/구현된 검사·보존 범위에서 충족한다. Phase 3 실행/취득 조건·Unity 해석/실제 lock/설치·독립 복원은 미검증이며 Roadmap 10 전체는 미완료다.

# 후속 작업

Phase 3 착수 시 먼저 이 인계 기준선의 현재성·별도 root/저장 분리와 제외된 추적 입력의 필요성/안전한 제공 여부를 확인하고 실행 Task를 만든다. 그 후 실제 입력/필수 검증이 준비됐을 때만 사용자 실행을 연결한다. Roadmap 11~14 실제 원본 도입은 Roadmap 10 전체 완료 뒤에 진행하며, 다음 실제 Module 업데이트 전 복구 시험 의무를 유지한다.

# 관련 문서

- [AI 진입점](../../README.md)
- [Roadmap 10](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_010.md)
- [관리 기록](../../00_Project/ASSET_DEPENDENCIES.md)
- [구현 규칙](../../01_Rules/IMPLEMENTATION_RULE.md)
- [검증 규칙](../../01_Rules/VERIFICATION_RULE.md)
- [일반 Task 형식](../../99_Templates/GENERAL_TASK_TEMPLATE.md)

# 관련 작업 기록

- [Phase 2 전체/Step 10 판정](20261119_05_Phase2ManualSteps.md)
- [Step 8 전체 검사](20261119_26_Phase2Step8Check.json)
- [Step 9 사용자 작업 판정](20261119_28_Phase2Step9Check.json)
- [Roadmap 9 최종 QA/운영·복구 의무](../Prototype_9/20261118_01_Phase4ManualSteps.md)

# 작성 완료 기준

현재 고정 입력·안전 전달/제외·원래 자료 보호·조건부 명령/담당·성공/중단/원복·미확인 의무를 실제 근거에 연결했다. 이 문서의 작성 완료를 Phase 3 실행/권리·독립 복원 성공으로 기록하지 않는다.
