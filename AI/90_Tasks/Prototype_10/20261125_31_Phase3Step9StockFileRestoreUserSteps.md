# Phase 3 Step 9 — 전체 검사 결과와 필요한 정확한 공급 파일 복원

> 2026-11-25 보관 정리: 이 문서가 참조하는 선정 파일 11개는 [보관 목록·해시·검사 요약](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 통해 찾는다. 기존 JSON 경로/hash와 아래 실행 명령은 당시 기록이며 스크립트를 재실행하지 않는다. 원래 완료 판정의 범위/한계는 유지한다.

작성일: 2026-11-25. [실행 Task](20261120_04_Phase3ManualSteps.md)의 Step 9을 9-1~9-5로 나눴다. 정적 검증과 원복 근거는 준비됐지만, 아래 공식 파일 37개 누락이 남아 **Step 9은 아직 미완료**다.

## AI가 완료한 정적 검증

- 후보 root를 지정한 registry/lifecycle 검사와 HEAD/index 원본 유입 검사는 통과했다. 최초 ingress 호출의 필수 --evidence 누락은 실패로 보존하고 정확한 기존 evidence를 지정해 재실행했다. [초기 gate 기록](20261125_40_Phase3EvidenceStorage.md#archived-evidence), [정정된 ingress 검사](20261125_40_Phase3EvidenceStorage.md#archived-evidence)를 구분한다.
- 최초 snapshot 1,091개에서 현재 검토 대상 1,177개로의 차이는 승인된 변경 88개(기존 npm JSON 2개 변경·TMP 82개 및 .vsconfig/NuGet lock/새 Test 2파일 추가)와 정확히 일치한다. 누락/예상 밖 덮어쓰기는 없다. 111개 private 보존 입력은 ignored/untracked이고 TMP schema 2 보존본은 원래/후보 양쪽에서 검증했다.
- 실제 worktree의 현재 추적 byte도 알려진 원본 지문 110개로 추가 검사했다. HEAD/index만의 통과를 현재 worktree 검사나 다른 staged 정책 재현으로 혼동하지 않았다. HEAD/index의 mode/OID/path는 같고 980개 모두 정상 index flag, remote/staged 변경은 없다.
- npm 57개 설치 폴더의 실제 파일 **4,649개**를 SRI 검증된 설치 archive의 파일과 대조했다. lock/alias/version/65개 의존성과 NuGet 기본/Linux 13개·lock/cache도 검증했다. [전체성/원복 입력](20261125_26_Phase3Step9FidelityAndRollbackInputs.json), [의존성 실제 파일 검사](20261125_27_Phase3Step9DependencyVerification.json)를 따른다.
- 원복/늦은 변경 거절·잘못된 backup 거절·새 출력 보존 등 기존 임시 snapshot fixture **79개가 통과**했다. 실제 원래 프로젝트/후보를 되돌리지 않았다. npm JSON 2개의 실제 before/attempt/current/backup 해시를 대조해 필요한 경우의 정확한 계획만 만들었다. [fixture 검증](20261125_28_Phase3Step9RollbackFixtureVerification.json)을 따른다.

원래 31회 GUID 진단과 후보 orphan 37개를 삭제하지 않았으므로 audit는 수집 완료·exit 1이다. 등록/유입 gate의 exit 0은 참조 건강·권리·전체 독립 복원 PASS가 아니다.

## 추가로 확인된 공식 공급본과의 차이

고정 Editor **6000.3.5f2** 설치의 `Editor/Data/Resources/PackageManager/BuiltInPackages`와 직접 대조했다. 경고의 37개 파일은 **공식 공급본에 실제로 존재하는 0-byte 파일**이다. 공급본/후보의 package ID·버전과 해당 meta hash/GUID도 모두 일치한다. 후보에는 이 파일들이 없으며 별도 사용자 계정 읽기 전용 조회에서도 같은 부재를 확인했다.

따라서 이 차이를 무해한 Unity 버그라고 선언하거나 모든 필수 package 입력이 같다고 통과시키지 않는다. 경고 발생 메커니즘 전체는 여전히 미확정이지만, 확인된 파일 부재는 정확한 공급본으로 복원할 수 있다. 지금까지는 임의의 cache 파일 생성/복사를 하지 않았고, 이제 새로 확보한 공급 ID/버전/byte/meta 근거로 **이름 붙인 37개 파일만 사용자 복원**을 준비했다.

이 작업은 package 버전 업그레이드나 예전 프로젝트 Library 복사가 아니다. 설치된 동일 Editor의 공식 파일을 검증해 누락된 같은 위치로 복원한다. 총 payload는 0 byte이며 기존 meta/GUID·코드·shader·설정·Scene/Prefab을 바꾸지 않는다. 예전 후보에서 빠진 stock 파일을 임의 내용으로 새로 만드는 방식도 아니다.

## 사용자 실행 — 실제 파일 복원 1회

계획: [공식 파일 복원 manifest](20261125_29_Phase3Step9StockFileRestorePlan.json). 실행 파일: [검증된 사용자 스크립트](20261125_40_Phase3EvidenceStorage.md#archived-evidence). 준비 근거: [최종 정적 검증](20261125_32_Phase3Step9PreparationVerification.json).

1. **Unity Editor를 종료한다.** Hub는 열려 있어도 된다. 저장하지 않은 Scene 변경이 있다면 임의로 저장하거나 폐기하지 말고 상태를 알려준다.
2. PowerShell에서 아래 명령을 한 번 실행한다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_30_Phase3Step9RestoreStockFiles.ps1'
```

스크립트는 계획 hash·공식 source hash와 0-byte 조건·source/target package manifest·meta hash·기존 후보 입력 1,113개·원래 index/HEAD·후보 HEAD/staged를 먼저 검사한다. 37개 target이 모두 없을 때만 **overwrite=false**로 복사하며 새 경로를 만들거나 기존 파일/meta를 덮어쓰지 않는다. source는 고정 Editor 공급 폴더, target은 정확한 후보 Library/PackageCache 안으로 제한하고 junction/link를 거절한다. 원래 프로젝트에는 쓰지 않는다.

복원 전 상태는 후보 상위 `Outputs/Step9StockRepairBefore_01.json`, 성공/부분 결과는 `Outputs/Step9StockRepair_01.json`에 새로 남는다. 실제 source·target 해시를 대조하므로 0-byte라는 이유로 검증을 생략하지 않는다. 실패/부분 생성 시 같은 스크립트를 반복하거나 생성 파일·로그를 삭제하지 말고 오류를 알려준다. 새/부분 출력은 자동 원복/삭제하지 않는다.

3. 성공하면 **후보만** Editor 6000.3.5f2로 열고 import/Script Compilation이 끝날 때까지 기다린다.

```text
C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Project
```

4. Console에서 **같은 immutable package 경고 37개가 다시 발생하는지**, 새로운 Error/Warning이 있는지 확인해 알려준다. 경고가 없어졌다고 가정하지 않는다. Scene/Prefab을 저장하지 않고 Editor를 종료한다.

이번 작업은 실제 공급 파일의 복원과 Editor 확인이므로 사용자 실행이 필요하다. 정적 hash/경로/원본 비교는 이미 AI가 처리했다. **추가 빌드·Play·Unity Test Runner 실행은 요청하지 않는다.** 새 코드/게임 자산/설정/버전 변경이 없고 기존 실행 결과가 보존됐으므로 이를 위해 Stage A/B/C를 반복하지 않는다.

## 완료 및 실패 처리

사용자 수행 뒤 AI가 실제 37개 payload·meta·package identity와 보호 입력을 다시 읽고 audit/ingress/TMP/Git을 정적으로 대조한다. 그 결과와 실제 Editor 경고/컴파일 보고로 Step 9 완료 가능 여부를 판정한다. 경고가 재현되면 실제 before/attempt/current와 공식 stock 근거를 유지해 추가 원인을 좁힌다. 자동 reset/clean·전체 cache 삭제·package 업그레이드나 임의 Scene 수정은 하지 않는다.

현재 원래 자료 손실은 없고 다른 정적 검증은 통과했지만, 발견한 37개 공식 파일 부재를 남긴 채 Step 9/전체 복원을 완료로 처리하지 않는다. 권리/참조 건강·독립 복원은 각각의 결과이며 Step 10 최종 판정도 아직 수행하지 않았다.
