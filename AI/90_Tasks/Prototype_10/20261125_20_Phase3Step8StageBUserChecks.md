# Phase 3 Step 8 — Stage A 판정과 다음 사용자 확인

작성일: 2026-11-25. [실제 Stage A 검사](20261125_19_Phase3Step8StageAActualVerification.json), [원래 Task](20261120_04_Phase3ManualSteps.md)를 따른다. 이전 Test 복사 스크립트/EditMode 5개를 다시 실행할 필요는 없다.

## Stage A에서 확인된 사실

- 사용자: Script Compilation 성공, 예상 밖 컴파일 Error/Warning 없음, EditMode 5개 통과. 별도로 immutable package 경고 1묶음을 보고했다.
- AI: 실제 진단 JSON 5개에서 같은 후보/6000.3.5f2와 pre-TearDown Passed·assertionIssues 0을 확인했다. 최종 Runner XML은 아직 없으므로 사용자 통과 보고와 진단 JSON을 구분한다. XML을 만들기 위해 이미 통과한 Test를 재실행하지 않는다.
- 고정한 기존 파일 1,113개는 모두 같다. 새 후보 Test도 준비한 원문과 같다. 원래 프로젝트 Assets/기존 Test·HEAD/index는 보존했다.
- URP renderer/SSAO feature와 두 VolumeProfile, TMP font/atlas/material/sprite/fallback, 네 prefab component/anchor가 실제 Unity 객체로 로드됐다. 네 TMP graph는 Shader main object/support True이며 현재 compiler message가 없다. 이는 모든 shader variant/build의 정상 보증은 아니다.
- 기존 31회 GUID는 실제 AssetDatabase에서도 여전히 경로를 찾지 못했다. 두 VolumeProfile의 raw component 21개와 import 후 19개는 원래 Outline/Oasis 참조의 null 제거 동작과 일치한다. source YAML은 그대로다. 다른 TestAnimationCurve/TestVolume 미해결 객체는 raw components 목록에 없다. 기존 Test/null-script 기록을 디스크에서 삭제하지 않았다. 실제 게임/표시 영향은 다음 확인에 남긴다.
- PC_Renderer의 일곱 probe debug 참조는 고정 URP Runtime/Deprecated.cs의 obsolete 구조에 속한다. 조사한 URP/Core C#에서 필드의 현재 사용은 찾지 못했다. 이를 지우거나 현재 resource를 임의 배정하지 않는다.

## immutable package 경고의 판정

이 경고는 변경 불가 package 영역에서 기대한 상태와 차이를 감지했다는 뜻이다. **이번에 보고된 37개 경로는 Step 7의 orphan meta 대응 파일 목록과 정확히 같다.** 내장 module trigger 34개와 Editor 파일 3개다.

원래 대응 파일은 모두 0 byte이고, 후보에서는 **Test 실행 전에도 없었고 현재도 없다**. meta는 원래와 같으며 고정 입력 1,113개에도 새 변경은 없다. 검사 소스에 package 파일 쓰기/삭제 호출도 없다. 따라서 이 메시지만으로 새 Test가 package를 수정했다고 판정하지 않는다. 반대로 경고를 Unity 버그/무해한 false positive로 확정할 근거도 없다. 정확한 경고 발생 메커니즘은 미확정이며 원인 조사/최종 package 무결성 판정과 현재 기능 확인을 분리한다.

지금 package 파일 만들기/삭제·cache 복사/청소·reimport·Editor 업그레이드를 하지 않는다. 경고는 보존한 상태에서 아래 격리된 확인을 진행할 수 있지만 전체 복원 정상으로 처리하지 않는다. 같은 37개 경고가 반복되면 “같은 목록 재현”이라고 알려주면 되고, 새 경로/오류가 추가되면 그 메시지를 알려준다.

## Stage B — PlayMode 3개만 사용자 실행

대상은 기존 후보 `C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Project`, Editor **6000.3.5f2**다. 원래 프로젝트에서 실행하지 않는다. 새 코드/자산 복사는 없다.

### 매 실행 전에 메모리 저장·온라인 비활성을 예약한다

1. Edit Mode에서 `Flow State > Online Record Verification` 창을 연다.
2. **Play 전 격리 실행 예약 (원격 호출 없음)**을 누르고 **IsolationArmed=True**를 확인한다. 이 창은 열어 둔다. 원격 동의·전송·A/B 파일 세션 시작은 하지 않는다.
3. 현재 EditorSettings는 enterPlayModeOptionsEnabled=1/options=0으로 domain/scene reload를 유지하는 코드의 안전 조건에 맞는다. 설정 변경은 필요 없다. 예약 버튼이 거절되거나 True가 아니면 실행하지 말고 그 상태를 알려준다.

Play 종료 후 예약이 해제되므로 **Test Runner를 다시 실행할 때마다 다시 예약**한다. Test namespace의 기존 SetupFixture도 메모리 저장 flag를 켜고 복구한다. GameSystem.InitializeLocalState는 이 조건에서 기존 저장 대신 메모리를 사용하고 online coordinator 생성 전에 return한다. 사용자가 상단 Play를 먼저 누를 필요는 없다.

### Test Runner에서 선택 실행한다

Test Runner의 **PlayMode**에서 아래 세 method만 실행한다. 한 번에 선택할 수 있으면 3개를 선택하고 Run Selected, 따로 실행하면 매 실행 전 위 격리 예약을 반복한다. Run All은 필요하지 않다.

| Fixture | Method | 목적 |
|---|---|---|
| ModeUISceneConfigurationTests | UIComponents_AreSerializedBeforeUseAndNeverAddedAtRuntime | 실제 SampleScene의 UI/필수 component와 기존 serialized handler 연결 |
| ModeUISceneConfigurationTests | Phase4LeaderboardAndResultControls_AreWired | 실제 leaderboard/result controls·TMP 문구/객체·listener 연결 |
| InfinitePatternConnectionIntegrationTests | FrontBoundary_PhysicalTrigger_AdvancesOnlyOnce | 실제 Scene에서 초기화된 pattern slot/prefab instance와 한 번의 경계 전환 |

이 Test는 사용자가 실행하며 Scene을 메모리에서 로드하고 transient 상태를 바꾼다. UI Test는 끝에 cleanup Scene으로 바꾸고 원래 Scene을 unload하며, pattern Test는 바꾼 runtime 값을 복구하고 timeScale을 1로 되돌린다. Scene/Prefab/AssetDatabase 저장과 persistent 저장/원격 호출을 Test가 수행하지 않는 경계를 감사했다. AI는 이 실행이나 Scene 조작을 수행하지 않는다.

**예상:** 3개 실행/3개 통과, 새로운 Error/실패/예상 밖 경고 없음. 실패하면 다음 Stage C로 넘어가지 말고 Test 이름·오류와 현재 결과를 남긴다. Test를 통과시키려고 Inspector 값을 보충하거나 항목을 삭제하지 않는다.

가능하면 결과를 후보 상위 `Outputs/Step8PlayMode_01.xml`에 저장한다. 따로 실행했다면 `_02.xml`, `_03.xml` 등 새 이름을 사용한다. 기존 XML을 덮어쓰지 않는다. 저장 기능을 못 찾으면 통과/실패/미실행 수와 메시지부터 알려준다.

## Stage C — Stage B 통과 뒤에만 실제 표시 확인

자동 검사로 대체할 수 없는 현재 화면 표현만 확인한다. Stage B가 실패했으면 여기까지 진행하지 않는다.

1. PlayMode Test가 끝나 Edit Mode로 돌아왔는지 확인한다. Project에서 **Assets/Scenes/SampleScene.unity**를 연다. 저장 창/기존 미저장 변경이 있으면 임의 저장/폐기하지 말고 상태를 알려준다.
2. 위 창에서 격리 실행을 다시 예약해 **IsolationArmed=True**를 확인한 후 사용자가 상단 Play를 누른다. 원격 동의나 계정/이전/실제 제출 기능은 누르지 않는다.
3. 메뉴에서 게임에 진입해 메뉴·점수/게임·Pause 화면의 기존 글자가 깨지거나 사각형으로 바뀌지 않는지, 사용 material이 분홍색/누락으로 보이지 않는지 확인한다. onClick/Navigation 등 자동 확인 가능한 연결을 수동으로 다시 검사할 필요는 없다.
4. Play를 종료한다. 필요하면 Project에서 `Assets/TextMesh Pro/Sprites/EmojiOne.png`를 선택해 Inspector 미리보기의 이미지가 정상인지 확인한다. Scene에 새 Sprite/TMP 객체를 만들지 않는다.
5. **Scene/Prefab을 저장하지 않는다.** 실제 화면 결과와 새 로그/오류를 알려준다. 종료 시 저장 여부가 불명확하면 임의로 선택하지 말고 상태를 알려준다.

## 전달할 결과와 완료 범위

- Stage B: 3개 통과/실패/미실행 수, 실패 이름·메시지, 있으면 XML 경로.
- Stage C: 메뉴/게임/Pause의 글자·material 표시와 EmojiOne 미리보기 정상 여부.
- 경고: 기존 37개 재현 여부와 새 경로/새 오류 추가 여부.

파일/meta/hash·진단 JSON과 원래/후보 보존은 AI가 직접 비교하므로 수동 목록 조사/값 복사나 Secret 전달은 필요 없다. 현재 새 Build를 추가할 근거는 없으며 AI는 빌드하지 않는다. Stage B/C와 경고 최종 판정이 남아 **Step 8은 미완료**다. Stage A의 5개 통과만으로 실제 Scene/runtime/표시·package 무결성/전체 독립 복원 성공을 선언하지 않는다.
