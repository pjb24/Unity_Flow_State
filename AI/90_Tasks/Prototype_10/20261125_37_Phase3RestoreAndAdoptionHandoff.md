# Phase 3 최종 복원 판정과 후속 애셋 도입 인계

**새 작업자의 시작 문서:** [Third Party 파일 관리 가이드](../../../THIRD_PARTY_GUIDE.md). 이 문서는 현재 기술 기준선의 실제 입력/결과와 후속 인계 근거를 보관하며, 일반 작업 순서는 가이드에서 설명한다.

> 2026-11-25 보관 정리: 이 문서가 참조하는 선정 파일 11개는 [보관 목록·해시·검사 요약](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 통해 찾는다. 기존 JSON 경로/hash와 아래 실행 명령은 당시 기록이며 스크립트를 재실행하지 않는다. 원래 완료 판정의 범위/한계는 유지한다.

작성일: 2026-11-25. [원래 Task Step 10](20261120_04_Phase3ManualSteps.md)·[실측 조건 평가](20261125_36_Phase3Step10Assessment.json)·[최종 문서/보존 검증](20261125_38_Phase3Step10FinalVerification.json)을 함께 사용한다. 이 문서는 Task 실행 결과/인계이며 제품별 영구 기록은 [관리 기록](../../00_Project/ASSET_DEPENDENCIES.md), 절차의 규칙은 [구현 규칙](../../01_Rules/IMPLEMENTATION_RULE.md), 실제 명령/API는 [DependencyAudit 안내](../../../Tools/DependencyAudit/README.md)가 담당한다.

## 판정과 적용 범위

**Phase 3와 Roadmap 10을 현재 기술 기준선 범위에서 완료한다.** 신규 Third Party 제품은 0개다. 이를 복원 면제로 쓰지 않고 기존 UPM/TMP·npm/NuGet을 별도 후보에 실제 준비한 결과와 필수 참조/사용자 Unity 결과를 평가했다. 완료 대상은 외부 원본 관리 체계와 현재 프로젝트의 필수 입력·참조를 재현하는 복원 절차다. 기존 모든 자산의 결함 제거, 모든 플랫폼/Player/Shader variant, 제품별 권리·배포 조건의 신규 승인까지 완료한 것은 아니다.

후보는 `C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Project`다. 원래 작업본은 `C:\Unity\Unity_Flow_State`이며 변경하지 않은 생산 입력의 근거다. 후보는 미커밋 자료까지 명시적으로 고정한 snapshot에 독립 Git을 초기화했다. 원격 Git clone이나 clean checkout만으로 복원했다고 설명하지 않는다. 기존 Library/node_modules/obj 전체를 복제하지 않았다. 원래 Task가 최신 지시이며 후보 안의 문서는 Step 3 시점으로 고정되어 있다.

| Roadmap Phase 3 조건 | 최종 근거/판정 |
| --- | --- |
| 필요한 외부 입력과 필수 참조 복원 | UPM 61개/연결 91개, TMP 재적용, 실제 npm lock/ci 57개·NuGet 기본/Linux 13개. 공통 자산/meta/필수 참조 대조와 실제 타입·Prefab·profile·font/material/sprite 로딩, 사용자 컴파일·EditMode 5개·PlayMode 3개·표시 정상. 아래 기존 진단의 현재 사용 영향과 구분해 충족 |
| 원본 비추적과 프로젝트 소유 자료 보존 | private 입력 111개 ignored/untracked, registry/managed·staged ingress/lifecycle exit 0. 원래 HEAD/index·QA/생산 입력·Scene/Prefab 불변. snapshot 1,091→1,177개의 승인된 88개 작업과 사후 보호 입력 확인으로 충족 |
| 제약/수동 절차와 결과 기록 | 실제 입력별 hash/취득·실행 스크립트·실패/재개/결과·원복 계획을 보존하고 이 인계와 영구 문서에 연결해 충족 |

Step 10에서 기존 문서 6개만 갱신 대상으로 분리하고 나머지 원래 보호 파일 1,177개, 후보 보호 입력 1,113개와 승인된 project 입력 1,177개를 다시 대조했다. 앞선 Step 9의 원래 보호 파일 1,179개와 수치가 다른 것은 이번에 관리 기록·도구 안내 2개를 추가로 갱신하기 때문이다. 원래 자료 2개가 소실된 것이 아니다.

## 남은 진단의 정확한 취급

raw audit는 **exit 1, 기존 GUID 미해결 31회, orphan meta 0개**다. 수집은 완료했지만 전체 참조 건강 PASS로 바꾸지 않는다. 복원으로 새로 생긴 GUID 미해결은 없다. 원래 불완전한 자료가 byte 단위로 재현됐다는 것만으로 정상 판정하지 않고 다음 실제 사용 근거까지 확인했다.

| 기존 진단 | 현재 범위의 판단 | 재확인이 필요한 시점 |
| --- | --- | --- |
| 프로젝트/공급 DefaultVolumeProfile의 script GUID 8회 | 원래 raw 자료 보존. 실제 import된 두 profile은 각각 유효한 component 19개다. raw 21개 중 Outline/Oasis의 unavailable entry를 고정 Core의 OnEnable이 메모리에서 제거하는 동작과 일치하며 나머지 test subasset도 보존한다. 현재 pipeline/renderer/SSAO 로딩·게임 표시 결과와 함께 현재 복원 비차단으로 판단 | 해당 custom volume 효과를 실제로 사용할 때 script 취득/필수성·component·효과를 검증. 없어진 효과가 구현됐다고 주장하지 않음 |
| PC_Renderer의 probeVolumeResources 7회 | 조사한 URP/Core C# 범위에서 obsolete 선언 외 사용 없음. 실제 현재 renderer/feature 로딩 확인. 현재 복원 비차단 | probe/debug 사용·URP 변경 시 해당 리소스의 필요성과 실제 동작 재검증 |
| TMP ShaderGraph 4개의 texture GUID 16회 | 현재 프로젝트의 직렬화 incoming 사용 없음. 현재 font material은 mobile SDF shader 사용. 네 graph의 실제 imported shader supported/no current compiler messages와 현재 text/material/EmojiOne 표시 정상 확인. 현재 복원 비차단 | 해당 graph/texture·새 variant 도입 시 재취득/사용처·import/렌더링 검증 |

GUID/Scene/Prefab/YAML을 임의 수정하거나 누락 참조를 삭제해서 통과시키지 않았다. 이 판단은 미래에 그 자료를 필수로 사용하는 변경의 면제가 아니다. 상세 원본 참조는 [Step 7 평가](20261125_11_Phase3Step7ReferenceAssessment.json), 실제 Unity 해석은 [Stage A](20261125_19_Phase3Step8StageAActualVerification.json), 실행/표시는 [Stage B/C](20261125_22_Phase3Step8StageBCActualVerification.json)를 따른다.

## 재현 입력과 취득/보존 경계

아래는 실행했던 입력의 연결표다. 개별 패키지 버전/hash를 새 수동 목록으로 복제하지 않고 연결한 spec/lock/manifest를 정답으로 사용한다.

| 입력 | 실제 source/version/hash와 보존/취득 |
| --- | --- |
| Editor/UPM | Unity Hub의 Editor **6000.3.5f2**, 프로젝트 Packages manifest/lock과 실제 61개/91개 graph. ugui **2.0.0**, Cloud Code **2.10.4**. 계정/공식 package 접근 필요. [Step 5 실측](20261120_18_Phase3Step5ActualCheck.json)·[후보 gate](20261125_40_Phase3EvidenceStorage.md#archived-evidence) |
| snapshot | `Outputs/Step4PreparedPlan.json` SHA256 `7db3f4e329f9216087d3f24959a20173bd545693bbdef6c44ffb7281ca571df4`. 정확한 원래/후보 root·file/dir·hash는 [Step 4 전달](20261120_12_Phase3Step4TransferManifest.json)과 runRoot Outputs에 보존. 현재 HEAD만으로 미커밋 자료를 대체하지 않음 |
| TMP 변경본/Import/meta | `Ignore/DependencyPreservation/tmp-baseline/29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c`의 schema 2 보존본. manifest SHA256은 마지막 경로명. 83개 보존 파일에 공급 unitypackage 1개 포함, 실제 Assets 재적용 82개. archive SHA256 `58564ec64cef8a8cea3d0b8ffb5e6fecf4176d544a2f6a29a2b43a0a33256431`. 기본 TMP import로 변경본/원래 meta를 덮어쓰지 않음 |
| npm archive | 고정 Cloud Code 공급본의 tgz **27개**. `Ignore/DependencyArchives/npm/com.unity.services.cloudcode/2.10.4/<sha>/<filename>`. [기존 spec](20261119_19_Phase2Step6RestoreSpec.json)·[Step 6 계획](20261120_21_Phase3Step6RestorePlan.json)에 경로/SRI/version/hash. Git 밖에 보존하며 합법적 공식 재취득·동일성 확인 필요 |
| 실제 npm 선언/lock | 후보 manifest SHA256 `cf28825ab638ab3caf701acb95cb5d6a5334db6f05d13b5e67583e571621fec0`, lock SHA256 `0e8f4e31da9ceabf4129193bc83676435f820556c8234d80ed1c268c8c360b0a`. 경로 필드만 이식. [Step 6 최종](20261125_08_Phase3Step6FinalVerification.json)·[Step 9 설치 검증](20261125_27_Phase3Step9DependencyVerification.json)에서 57개/연결 65개·실제 payload 4,649개 확인 |
| NuGet | global SDK **9.0.315**, net9.0 + linux-x64 기본/런타임 target, 직접 4개·전체 13개. 후보 module `packages.lock.json`과 obj/cache 실제 일치. 전용 [NuGet config](20261120_23_Phase3Step6NuGet.config), nuget.org/Microsoft SDK offline feed. 전용 cache는 runRoot `Caches/NuGetPackages_Step6_01`. 원래 SDK/obj의 존재로 새 restore를 대체하지 않음 |
| 공식 package 파일 37개 | 같은 Editor 설치 `Editor/Data/Resources/PackageManager/BuiltInPackages`의 정확한 stock 파일/meta/package identity. [복원 계획](20261125_29_Phase3Step9StockFileRestorePlan.json)·[실제 사후 검증](20261125_34_Phase3Step9ActualRepairVerification.json). 임의 빈 파일 생성이나 원래 Library 복제가 아님 |

원래 package.json/lock과 이전 restore spec의 pin은 그대로 보존했다. 후보의 이식된 두 JSON을 원래 작업본에 자동 반영하지 않았다. 향후 실제 개발 경로로 채택할 때는 별도 변경으로 입력·backup·검증을 고정한다. Temp를 영구 저장소로 가정하지 않는다. 필요한 보존본/archive/Outputs/Logs가 유실되면 같은 상태를 Git만으로 복원할 수 없다. 오프라인은 필요한 공식 공급 입력과 적법한 로컬 보존본이 모두 있을 때만 가능하며 완전 오프라인 설치는 이번에 시험하지 않았다.

## 이번에 수행한 사용자 단계와 반복 조건

1. Step 5: 별도 후보의 고정 Editor/UPM 준비와 TMP 변경본/meta 재적용, import/컴파일 확인.
2. Step 6: 후보 경로만 이식한 실제 npm lock 생성. PowerShell stderr 처리 실패 후 실제 lock을 AI가 검증했다. NuGet 기본 restore 뒤 linux-x64 보완 restore로 기존 전체 target을 재현했다. AI 대조 후 사용자가 `npm ci --ignore-scripts --no-audit --no-fund` 실행. crypto-js 4.1.1의 기존 유지보수 중단 경고는 그대로 기록하고 임의 upgrade하지 않았다. 보안 audit 통과를 주장하지 않는다.
3. Step 8: 후보 전용 읽기 전용 Test 설치, 실제 컴파일·EditMode 5개, 기존 PlayMode 3개·표시 확인. Play 전에는 기존 `Flow State/Online Record Verification`의 격리 실행 예약을 사용했다. 폴더명만으로 저장/원격 격리를 보장하지 않는다. 최종 Runner XML은 미제공이며 실제 JSON은 pre-TearDown 관측이다. 사용자 최종 성공 보고와 구분해 보존한다.
4. Step 9: 사용자가 정확한 공식 파일 37개만 없는 target에 복원하고 고정 후보의 컴파일 성공·예상 밖 컴파일 Error/Warning 없음을 보고했다. AI는 공식 파일 부재 해소와 orphan 진단 37→0을 확인했다. 과거 immutable 경고의 모든 발생 원인/전체 로그상 재발 부재를 입증하지는 않았다.

**이번 Step 10에 추가 수동 작업은 없다.** 사용 완료 스크립트는 재실행하지 않는다. 새 복원 환경에서는 경로·HEAD·현재 입력/hash·후보 ownership·Editor/도구 버전과 이미 존재하는 출력을 먼저 확인하고 새 계획/출력명을 만든다. 과거 성공 스크립트를 다른 프로젝트에 그대로 실행하지 않는다. package/설치/Unity/Runner와 Scene/Prefab 작업은 사용자 담당이며 AI는 정적 계획/검증을 수행한다.

## Roadmap 11~14 실제 도입 점검표

현재 Roadmap 10 완료로 순서상 다음은 Roadmap 11이다. 이 인계가 후속 기능 구현이나 제품 구매/설치까지 실행하라는 지시는 아니다. 실제 제품을 정하면 아래 순서를 적용한다.

1. **등록:** 관리 기록에 영구 asset_id와 record_kind·제품/version·공식 취득 source/hash·담당·목적/게임 사용처·직접/동적/binary 참조를 기록한다. 가상 제품을 현재 빈 등록부에 추가하지 않는다.
2. **조건 확인:** 소유자가 계정/사용·수정·동봉/배포·Attribution·재취득/오프라인 조건의 근거를 확인한다. 기술적 복원 완료를 권리 확인으로 승계하지 않는다. 비밀값/유료 원본은 기록에 넣지 않는다.
3. **원본 비추적 보관:** 도입 전 원래 자료/설정/자산·폴더 meta·선언/lock의 before를 고정하고 공급 원본은 승인된 ignored/untracked 경계에 보관한다. 기존 추적 자료는 요청/근거 없이 해제하지 않는다. 과거 이력 삭제는 별도 범위다.
4. **필수 파일 도입:** 버전/source/hash와 import_path를 고정해 별도 후보에 필요한 범위만 도입한다. 변경본/파생물·Import 설정/meta를 원본과 구분한다. Scene/Prefab은 사용자가 정확한 적용표로 수행하고 AI는 정적으로 전후 대조한다.
5. **확장/사용처:** 공급 원본 직접 수정 대신 프로젝트 소유 adapter/복사본/variant 경계를 기록한다. 복사/수정 이유·원본 대응·게임 내 사용 위치/필수 리소스를 추적한다.
6. **Git/참조 gate:** registry→managed/staged ingress(정확한 KnownOrigins evidence 지정)→audit/import/name/dynamic/binary 검토→필요한 사용자 컴파일/Runner/표시 확인. raw finding을 지워 통과시키지 말고 필수성·원래 대비 영향·실제 사용 증거를 판정한다. 새로운 필수 실패는 중단한다.
7. **업데이트/제거/원복:** 같은 ID의 before/새 조건·사용처·hash와 검증된 backup을 보존하고 lifecycle plan을 검토한다. 늦은 변경/다른 작업 byte는 덮어쓰지 않는다. 실패 시 정확한 입력만 원복하며 새 부분 출력/로그를 보존한다. 전체 reset/clean/cache 삭제를 원복으로 사용하지 않는다.
8. **독립 복원:** 원래 작업본을 보존한 새 root에서 필요한 보존본/공식 입력을 재취득하고 새 lock/설치·필수 참조/Import·사용자 결과와 원본 비추적/자체 자료 보존을 함께 검증한다. 단순 도구/합성 Test 통과로 완료하지 않는다. 새 버전/필수 사용처의 결과로 등록부와 Task를 갱신한다.

새로운 필수 원본/hash·취득 조건·정확한 버전·권한·복원 가능성·참조/사용자 검증이 부족하면 소유자/실행 담당과 차단 이유를 기록하고 해당 도입을 보류한다. 기존 31회 진단 자료를 실제 사용하게 되면 위 재확인 조건을 적용한다. **다음 실제 Cloud Code Module 업데이트 전 실제 복구 시험 의무**는 Roadmap 9 인계를 그대로 유지한다. 이번 NuGet restore는 Module 빌드/게시/원격 복구 시험이 아니다. LogSink container 실행·immutable image digest와 모든 Player/platform/variant 검증도 이번 완료 범위 밖이다.
