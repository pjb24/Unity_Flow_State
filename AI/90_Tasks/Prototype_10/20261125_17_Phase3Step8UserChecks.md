# Phase 3 Step 8 — 사용자 실행 절차

> 2026-11-25 보관 정리: 이 문서가 참조하는 선정 파일 11개는 [보관 목록·해시·검사 요약](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 통해 찾는다. 기존 JSON 경로/hash와 아래 실행 명령은 당시 기록이며 스크립트를 재실행하지 않는다. 원래 완료 판정의 범위/한계는 유지한다.

작성일: 2026-11-25. [원래 Task](20261120_04_Phase3ManualSteps.md)의 Step 8을 8-1~8-5로 나눠 진행한다. 지금은 **Stage A: 후보 EditMode 5개**를 실행할 차례다. 사용자 실제 결과 전에는 Step 8을 완료 처리하지 않는다.

## 준비된 파일과 책임

- [읽기 전용 EditMode 소스](20261125_14_Phase3Step8ReadOnlyChecks.cs)는 원래 프로젝트의 AI/90_Tasks에만 있다. 원래 Assets/기존 Test/Scene은 바꾸지 않았다.
- [고정 입력](20261125_15_Phase3Step8ReadOnlyInput.json)은 후보의 기존 파일 1,113개 hash와 확인 대상 경로/타입/GUID/fileID를 담는다. Secret·저장·production 설정 본문은 담지 않는다.
- [사용자 복사 스크립트](20261125_40_Phase3EvidenceStorage.md#archived-evidence)는 후보에 Test C# 1개와 새 meta 1개만 CreateNew로 넣는다. 기존 파일이 있으면 덮어쓰지 않고 중단한다. Editor/Hub/빌드/Test Runner를 실행하지 않는다.
- [준비 검증](20261125_18_Phase3Step8PreparationVerification.json)은 구문/정적 부작용 감사와 원래/후보 보존 근거다. 실제 C# 컴파일/Runner 성공 기록이 아니다.

## Stage A — 지금 수행할 작업

1. **Unity Editor를 종료한다.** 원래/후보 Editor가 열려 있으면 닫는다. Unity Hub는 열려 있어도 된다. Scene에 저장하지 않은 변경이 있다면 임의로 저장하거나 폐기하지 말고 먼저 알려준다.
2. PowerShell에서 다음 명령을 한 번 실행한다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_16_Phase3Step8PrepareChecks.ps1'
```

고정 입력/원래 index·후보 HEAD/staged·기존 1,113파일 hash를 먼저 확인한다. 성공하면 후보 `Assets/Tests/EditMode/Phase3RestoreReadOnlyTests.cs`와 meta가 생기고 `Outputs/Step8Prepare_01.json`에 결과가 남는다. 오류나 부분 생성이 있으면 같은 스크립트를 반복하거나 파일을 삭제하지 말고 오류를 알려준다.

3. Unity Hub에서 다음 **후보 폴더**를 기존 프로젝트로 추가/열고 **6000.3.5f2**를 사용한다. 원래 `C:\Unity\Unity_Flow_State`를 열지 않는다.

```text
C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Project
```

import/컴파일이 끝나면 Console의 Error/Warning을 확인한다. 컴파일 오류가 있으면 여기서 중단하고 정확한 경로/행/메시지를 알려준다. 임의 패키지 업그레이드·TMP 재가져오기·전면 reimport·cache 삭제·Scene/Prefab 저장·빌드·Play는 하지 않는다.

4. Test Runner를 열어 **EditMode**에서 `Phase3RestoreReadOnlyTests`를 검색하고 아래 **5개만** 선택 실행한다. Run All이나 PlayMode 전체 실행은 필요하지 않다. AI는 이 실행을 대신하지 않는다.

| Test 이름 | 확인하는 책임 |
|---|---|
| RequiredTypes_AreAvailableWithoutStartingSystems | 실제 컴파일된 URP/TMP/게임/Editor TestRunner 타입의 존재. 시스템 생성/원격 호출 없음. |
| ActiveUrpAssets_LoadAndReportVolumeAndDebugReferences | 활성 PC pipeline/renderer/VolumeProfile 자산과 실제 component/localID. 기존 15회 참조를 별도 관측 기록. |
| ShaderGraphs_ReportActualImporterAndCompilerMessages | 네 TMP graph의 실제 importer/Shader/compiler 메시지와 16회 texture 참조 관측. |
| TmpResources_LoadFontAtlasSpriteAndMaterialSubassets | TMP 기본 font/sprite/style·atlas/material·fallback source font의 실제 object/subasset 로딩. |
| PrefabAssets_LoadAuthoringAndSerializedAnchorsWithoutInstantiation | 네 prefab의 실제 component/anchor 로딩과 Missing Script 확인. Scene 로딩/Instantiate 없음. |

**예상 결과:** 5개가 실행되고 필수 타입/실제 자산·필드·참조 검사가 성공해야 한다. Assert 실패나 예상 밖 Unity 로그는 그대로 남겨 보고한다. known GUID가 계속 미해결이거나 shader 경고/비활성 HDRP 지원 여부가 기록될 수 있다. 이를 자동으로 성공/실패 면제하지 않는다. Test가 녹색이어도 진단 JSON의 `reviewRequired`와 실제 Console/Runner 결과를 AI가 검토하기 전에는 참조 건강/Step 8 성공이 아니다. `approvalGranted`는 항상 false다. `runnerOutcomeBeforeTearDown` 역시 최종 Runner XML을 대체하지 않는다.

5. Test Runner 결과를 저장할 수 있으면 다음 새 경로에 저장한다. 기존 같은 파일이 있으면 덮어쓰지 말고 새 이름을 알려준다.

```text
C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Outputs\Step8EditMode_01.xml
```

각 Test는 같은 Outputs 폴더에 `Step8ReadOnly_<Test명>_<ticks>_<id>.json`을 자동 생성한다. 기존 결과는 덮어쓰지 않는다. 내용 복사/수동 편집/Secret 전달은 필요하지 않다. 검사가 끝나면 Editor를 **Scene/Prefab 저장 없이 종료**하고 다음을 알려준다.

- 복사 성공 여부, Editor 컴파일 오류/경고 유무.
- 5개 Test의 통과/실패/미실행 수와 실패 Test 이름/메시지.
- Console의 예상 밖 로그·경고와 저장한 결과 XML 경로.

## 8-1. 정적 감사에서 추가로 확인한 점

고정 SRP Core의 `Runtime/Volume/VolumeProfile.cs:49` OnEnable은 null component를 메모리 목록에서 제거한다. 따라서 원래 YAML의 기존 10개 null-script 객체나 미해결 타입이 실제 로딩 뒤 어떻게 보이는지 기록하고, YAML과 개수가 달라도 자동으로 자산 손실/정상이라고 판정하지 않는다. 검사에서 이 동작을 호출해 영구 수정하거나 SaveAssets하지 않는다. 실제 Console 오류/영향 확인은 남아 있다.

새 Test는 AssetDatabase.LoadMainAssetAtPath/LoadAllAssetsAtPath, SerializedObject getter, long localID 조회, 현재 Shader 메시지 조회만 사용한다. long localID는 [Unity 공식 API](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html), Shader 메시지 조회는 [공식 API](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/ShaderUtil.GetShaderMessages.html)와 대조했다. 그래프를 강제 import/컴파일하는 API는 호출하지 않는다. 아직 실제 C# 컴파일/실행은 하지 않았으며 사용자 Editor 결과로 확인한다.

검사 전후 고정된 1,113개 파일 hash를 비교한다. 테스트가 직접 쓰는 것은 후보 바깥 Outputs의 새 JSON뿐이다. Unity 자체의 import/컴파일/Runner가 만드는 Library/Temp/UserSettings 등은 실제 실행의 별도 출력이다. Scene 열기/저장·prefab instance 생성/삭제·컴포넌트 추가·AssetDatabase 저장/수정·PlayerPrefs/기존 저장·SessionState 변경·서비스/프로세스 실행과 로그 억제는 없다. 자산 OnEnable이 메모리 데이터를 갱신할 수 있으므로 디스크 불변 검사와 실제 Unity 로그를 함께 본다.

## 8-3~8-4. Stage B — Stage A 검토 뒤 안내할 범위

**지금은 실행하지 않는다.** Stage A 실제 결과를 AI가 비교한 뒤 필요한 다음 범위만 연결한다.

- U5 실제 Scene/UI 연결: 기존 `ModeUISceneConfigurationTests.UIComponents_AreSerializedBeforeUseAndNeverAddedAtRuntime`, `Phase4LeaderboardAndResultControls_AreWired`와 `InfinitePatternConnectionIntegrationTests.FrontBoundary_PhysicalTrigger_AdvancesOnlyOnce`를 최소 후보로 검토했다. 실제 production Scene과 runtime instance를 사용하되 transient 상태만 바꾸고 Scene 저장을 하지 않는 Test다. 코드가 고정됐는지와 결과에서 드러난 영향에 따라 정확한 선택을 확정한다. 전 Unity suite를 요구하지 않는다.
- PlayMode에는 같은 namespace의 `PlayModeRecordIsolationSetup`이 `GameSystem.UseIsolatedRecordsForPlayModeTests`를 켰다가 복구한다. GameSystem.InitializeLocalState는 이때 메모리 저장을 쓰고 online coordinator 생성 전에 return한다. 이 연결과 실제 격리 선행 조건을 재확인한 뒤 사용자에게 실행을 안내한다.
- U4/U5 표시: 자동으로 확인하기 어려운 텍스트/폰트 깨짐·분홍색 material·sprite 표시·실제 UI/게임 화면만 사람이 확인한다. 수동 Play 전에는 기존 `Flow State/Online Record Verification` 창의 `Play 전 격리 실행 예약 (원격 호출 없음)`과 `IsolationArmed=True` 확인이 필요하며, domain/scene reload를 끄지 않는다. 지금 새 Play/Scene 작업을 시작하라는 지시가 아니다.
- 빌드: 현재 미확인 import/type/subasset을 확인하기 위한 추가 빌드는 요구하지 않는다. Stage A 결과에서 build/Player에만 해당하는 필수 문제가 확인되면 별도 출력/설정/격리/성공 기준을 준비하고 사용자 실행에 연결한다. AI는 빌드를 수행하지 않는다.

## 8-5. 완료/중단 기준

현재는 8-1 정적 감사/검사 소스와 8-2 Stage A 사용자 절차 준비까지 완료했다. Stage A의 컴파일/실제 결과와 reviewRequired 판정, 필요한 Stage B/표시 결과가 남아 **Step 8은 미완료**다. 과거 Step 5 Editor 보고나 Step 7/Node 통과를 이번 실행 결과로 대체하지 않는다. 실패 로그/부분 결과를 보존하며 자동 reset/clean/원복·원래 파일 덮어쓰기는 하지 않는다. Scene 변경이 필요하다고 확인되면 사용자 적용표를 따로 준비한다. 현재 Scene 변경 제안/적용은 0건이다.
