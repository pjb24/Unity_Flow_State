# Phase 3 Step 7 — 참조 판정과 Step 8 인계

> 2026-11-25 보관 정리: 이 문서가 참조하는 선정 파일 11개는 [보관 목록·해시·검사 요약](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 통해 찾는다. 기존 JSON 경로/hash와 아래 실행 명령은 당시 기록이며 스크립트를 재실행하지 않는다. 원래 완료 판정의 범위/한계는 유지한다.

작성일: 2026-11-25. 원래 Task의 Step 7 결과이며 Step 8 실행 성공 기록이 아니다.

- [원래 raw inventory](20261125_40_Phase3EvidenceStorage.md#archived-evidence)
- [후보 raw inventory](20261125_40_Phase3EvidenceStorage.md#archived-evidence)
- [개별 참조/차이/판정](20261125_11_Phase3Step7ReferenceAssessment.json)
- [Step 7 최종 보존 검증](20261125_13_Phase3Step7FinalVerification.json)
- [실행 Task](20261120_04_Phase3ManualSteps.md)

## 7-1. 원래 자료와 후보 비교

공통 Assets 파일 632개, 참조가 가리키는 실제 파일 278개, 공통 package meta 9,709개가 byte 단위로 같다. Scene/Prefab/Material·ShaderGraph·asmdef·입력/설정과 공통 meta의 Importer/platform 설정을 변경하지 않았다. 직렬화 closure 56파일의 1,299회 GUID 참조 목록도 같다. GUID 중복 진단은 양쪽 모두 0개다. 간단한 로컬 fileID 참조에서 같은 YAML 문서의 anchor를 찾지 못한 항목은 없었으나, 외부 fileID/subasset·MonoScript 타입·실제 Import 결과까지 정상이라는 뜻은 아니다.

원래 Assets에만 있는 production 설정 C#와 meta 2개는 Step 3 snapshot의 명시적 private 입력 제외다. production 설정 본문은 읽지 않았으며 이를 복원하거나 Scene에 붙이지 않았다. UGS의 .env 등 비밀값 제외 경로도 내용 조사/복사 대상이 아니다.

전체 meta 수는 원래 11,816개, 후보 10,039개다. 차이 1,777개 중 1개는 위 private meta이며 나머지 1,776개는 package의 Samples~ 1,760개·Documentation~ 2개·아이콘 14개다. 이 meta GUID들로 들어오는 현재 직렬화 참조는 없다. 개수 차이를 파일 누락 0개로 숨기거나 샘플/문서를 재가져오지 않는다. 공통 package meta의 GUID/Importer 내용 차이는 없다.

프로젝트 asmdef 4개의 이름 참조는 후보 프로젝트/설치 package asmdef에서 모두 대응 이름을 찾았다. Resources 상대 이름 9개와 Shader 선언 이름을 목록화했다. 이는 Unity 플랫폼별 assembly 활성화/컴파일·Resources 실제 로딩 보증이 아니다.

## 7-2. 기존 31회 미해결과 새 진단

미해결 31회는 아래와 같다. JSON에는 각 occurrence의 GUID·fileID·경로·행·소유 객체·필수 여부·정적 근거·후속 확인을 따로 기록했다. 원래 대비 새 GUID 미해결은 0개다.

| 대상 | 횟수 | 정적 근거와 현재 판정 | Step 8 |
|---|---:|---|---|
| Assets/Settings/DefaultVolumeProfile.asset | 4 | Global Settings가 이 profile을 직접 참조한다. 아래 4개 script GUID의 실제 타입/영향은 미확정이다. 현재 연결에서 비필수라고 단정하지 않는다. | U1 |
| URP package의 Editor/Volume/DefaultVolumeProfile.asset | 4 | 원래에도 같은 profile/script 참조가 있었다. 공급 profile의 Editor/실행 시 해석은 미확정이다. | U1 |
| Assets/Settings/PC_Renderer.asset | 7 | Graphics/Quality Settings → PC_RPAsset → PC_Renderer 연결이 있다. probeVolumeResources의 debug shader·mesh·texture·compute 필드다. m_LightProbeSystem=0 등 설정만으로 무조건 무관하다고 판정하지 않는다. | U1 |
| TMP_SDF-HDRP LIT / HDRP UNLIT / URP Lit / URP Unlit shadergraph | 각 4, 합계 16 | 같은 texture GUID를 참조한다. 현재 프로젝트 직렬화 자료에서 이 graph들로 들어오는 참조는 없고 실제 폰트 material은 TMP_SDF-Mobile.shader를 가리킨다. 현재 화면의 필수 material로 확인되지는 않았으나 Import/ShaderGraph 해석은 미확인이다. | U2 |

Volume profile의 script GUID와 객체 이름은 다음과 같다. 프로젝트와 공급 profile에서 각각 재현된다.

| GUID | 객체 이름 |
|---|---|
| 0fd9ee276a1023e439cf7a9c393195fa | TestAnimationCurveVolumeComponent |
| 5a00a63fdd6bd2a45ab1f2d869305ffd | OasisFogVolumeComponent |
| 60f3b30c03e6ba64d9a27dc9dba8f28d | OutlineVolumeComponent |
| 74955a4b0b4243bc87231e8b59ed9140 | TestVolume |

이름에 Test가 있다는 것만으로 활성 default profile의 항목을 삭제하거나 비필수로 확정하지 않는다. 별도로 두 profile에 m_Script fileID 0인 기존 객체 5개씩, 총 10개도 있으며 Editor.Tests class identifier를 기록했다. 이 기존 직렬화 내용도 유지하고 U1에서 실제 객체 해석을 확인한다.

원래 sample orphan 1개는 `com.unity.test-framework/Samples~/15_TestCases_15.meta`다. 후보에는 그 meta 자체가 없고, 현재 프로젝트에서 해당 GUID로 들어오는 참조는 없다. 게임 자산의 복원 누락으로 판정하지 않으며 원래 자료를 삭제하지 않는다.

후보에는 **새 orphan meta 37개**가 있다. 원래 sample orphan과 별도로 기록한다.

| 범위 | 건수 | 확인한 사실 | 미확인/처리 |
|---|---:|---|---|
| 내장 module의 package.ModuleCompilationTrigger.meta | 34 | meta는 원래와 같고, 원래 대응 파일은 0 byte, 후보 대응 파일은 없다. 현재 직렬화 incoming GUID는 없다. | U3의 Editor import/컴파일·module 가용성 확인. cache 파일을 AI가 만들거나 복사하지 않는다. |
| URP Editor/Converter/converter_widget.uss.meta | 1 | 같은 meta, 원래 대응 파일 0 byte, 후보 파일 없음, 직렬화 incoming 없음. | 기존 converter UI 사용 코드가 가리키는 main/item 파일과 구분한다. 화면/컴파일 결과 없이 도구 전체 정상으로 단정하지 않는다. U3에 기록한다. |
| ShaderGraph Editor/Importers/_.sgsubgraphdb.meta | 1 | 같은 meta, 원래 대응 파일 0 byte, 후보 파일 없음, 직렬화 incoming 없음. | U2/U3에서 ShaderGraph import 결과 확인. DB 재생성/삭제를 지시하지 않는다. |
| ShaderGraph Editor/Resources/Styles/MaterialGraph.uss.meta | 1 | 같은 meta, 원래 대응 파일 0 byte, 후보 파일 없음, 직렬화 incoming 없음. | MaterialGraphView 코드의 Styles/MaterialGraphView 로드와 구분하며 Editor 해석은 U3에 남긴다. |

두 raw inventory는 수집 완료이지만 진단을 유지하므로 모두 exit 1이다. 후보는 31회 미해결+37개 orphan이며, 원래는 31회 미해결+sample orphan 1개다. 이를 참조 건강 PASS로 바꾸지 않는다.

## 7-3. 동적·binary·이름 참조

- `InfinitePatternSlot.cs:90`의 Instantiate는 검증한 prefab 목록과 contentRoot를 입력으로 쓴다. 임의 이름으로 외부 자산을 찾는 호출은 아니다. 네 InfinitePatterns prefab과 SampleScene의 byte/직렬화 참조는 보존됐고 실제 instance/component 연결은 U5에 남긴다.
- `OnlineRecordVerificationWindow.cs:413`은 Editor TestRunner API 타입을 이름으로 찾는다. 나머지 이름/동적 지표 10파일은 Tests 아래의 reflection이다. 제품 runtime의 누락 리소스라는 뜻이 아니며, 전부를 다시 실행할 사유로 삼지 않는다. 실제 assembly/type 해석은 U3, 필요한 기존 Test 선택과 부작용 감사는 Step 8 책임이다.
- `LiberationSans.ttf`는 fallback font asset에서 참조한다. `EmojiOne.png`는 TMP Sprite Asset에서 참조한다. 파일과 Importer는 같지만 Unity의 font/texture/sprite subasset 생성 및 표시 정상 여부는 U4에 남긴다.
- 프로젝트 C#의 현재 탐지 범위에서 Resources.Load/Shader.Find/AssetBundle/Addressables를 통한 추가 이름 로딩 근거는 찾지 못했다. package 내부 모든 동적 경로를 포괄 검증한 것은 아니다. TMP의 Resources 파일/기본 설정처럼 package가 로드할 수 있는 자산은 byte/이름 보존과 U4를 함께 사용한다.

## Step 8에서 수행할 최소 확인 범위

**현재 Step 7을 끝내기 위한 사용자 실행은 없다.** 아래는 다음 Step 8의 준비/실행 범위다. 아직 구현하지 않은 Test 이름이나 실행 성공을 만들지 않는다. Step 8에서 읽기 전용 Test가 가능한 확인을 먼저 준비하고, 사람이 직접 확인해야 하는 표시 결과만 남긴다.

공통 환경은 후보 `C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Project`, Editor 6000.3.5f2, 메모리 저장·온라인 비활성이다. 원래 작업본에서 실행하지 않는다. production 설정/서비스 호출·기존 저장·업그레이드·패키지 재가져오기와 섞지 않는다.

| ID | 필요한 실제 확인 | 방법/성공 기준과 전달할 결과 |
|---|---|---|
| U1 | 활성 URP renderer/default profile의 타입·subasset/fileID·실제 영향 | 읽기 전용 Editor 자산 로딩/serialized property 검사로 위 경로와 15회 미해결·10개 기존 null-script 객체를 확인한다. Inspector 확인이 필요하면 Project에서 해당 Settings asset을 선택하고 값은 바꾸지 않는다. Missing Script/누락 resource/예외 및 실제 타입을 정확히 기록한다. 기존과 같다는 이유만으로 영향 없음으로 처리하지 않는다. |
| U2 | TMP graph의 16회 texture 참조와 Import 결과 | 네 graph의 importer/오류·shader 지원 여부를 읽기 전용으로 확인한다. graph를 편집/저장하거나 HDRP를 설치하지 않는다. 현재 사용 material이 아닌 graph의 진단도 결과에서 분리해 기록한다. |
| U3 | 37개 새 orphan·assembly/type 해석 | 사용자가 고정 Editor에서 import/컴파일 결과를 확인하고 Console 오류·예상 밖 경고를 보존한다. 필요한 type/assembly 존재 검사는 Step 8에서 준비한다. 원래 Step 5의 과거 오류 없음 보고를 현재 결과로 대체하지 않는다. 이 근거만을 위해 build를 추가하지 않는다. |
| U4 | 폰트·sprite·material subasset과 실제 표시 | 읽기 전용 로딩으로 TMP Settings/폰트 fallback/atlas/material/sprite texture 연결을 확인한 뒤 실제 후보 화면의 글자 깨짐·누락·분홍색 shader 여부를 사용자가 확인한다. 폰트를 다시 import하거나 원래 archive로 덮어쓰지 않는다. |
| U5 | Scene→serialized components/prefabs→runtime instance | Step 8에서 안전한 기존 Test의 정확한 범위와 side effect를 먼저 점검한다. 필요하면 사용자가 후보 SampleScene을 메모리/온라인 비활성 조건에서 확인한다. Infinite pattern과 기존 UI/포커스 연결이 실제로 유지되는지 확인하고 Scene은 저장하지 않는다. |

실패/오류 시 임의 수정·Missing 항목 제거·allowlist·전면 reimport·cache 삭제·SDK/Editor 변경을 하지 않고 대상 경로/오류/관련 로그를 전달한다. AI가 정적 분석으로 원인을 좁힌 뒤 필요한 사용자 작업만 다시 준비한다.

## Scene 적용표

현재 변경 제안 0건, 적용 0건이다. Scene/Prefab 저장이나 onClick/Navigation 재연결은 필요하다고 입증되지 않았다. Step 8에서 실제 수정 필요성이 확인되면 그때 기존→최종값·Hierarchy/컴포넌트/필드·저장/원복 절차를 따로 작성한다. AI는 Scene을 변경하지 않는다.

## 판정

Step 7은 정적으로 가능한 조사와 필수 미확인/사용자 검증 대상의 구체화를 완료했다. U1~U5의 실제 결과는 Step 8에서 확보해야 한다. Step 7 완료는 참조 정상/독립 Unity 복원·Phase 3·Roadmap 10 완료가 아니다.
