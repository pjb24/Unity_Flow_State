# PlayMode 테스트 필요성 검토 및 조치

검토일: 2026-10-06. 사용자 EditMode 887사례 성공 보고 후 PlayMode에도 동일한 검토/조치를 요청했다.

## 결론

원래 **37파일 / 234메서드 / 257실행 사례**를 기능별로 분류하고 테스트 본문, 관련 helper, 준비·정리 및 생산 연결을 정적으로 검토했다. NUnit Category는 현재 없다. 이번 카테고리는 문서상의 분류이며 코드에 Category를 새로 추가하지 않았다.

239사례는 유지 권고이며 해당 테스트 본문이 검토 시작 시점과 같음을 SHA256으로 확인했다. 18사례는 아래 조치를 적용했다. 같은 본문 해시인 테스트는 없지만 의미상 겹치는 흐름은 별도로 검토했다.

| 조치 | 사례 수 | 내용 |
| --- | ---: | --- |
| 유지 권고 | 239 | 실제 물리/프레임/Scene 연결, 실패 및 상태 보존 계약 유지 |
| 검증·격리 보강 | 10 | 실제 Submit 경로, 같은 프레임 입력, 초기 상태 성공/표시값 확인, 실패 시 객체 정리 |
| 통합받은 테스트 보강 | 3 | 제거한 사례의 즉시/프레임 간 상태 및 속도 보존을 옮김 |
| 이름 정정 | 1 | Flat 가속 표현을 현재 고정 속도 규칙으로 정정 |
| 불필요한 대기 정리 | 1 | 마지막 assertion 이후 수동 관찰용 5초 대기와 FreezeAll 제거 |
| 통합 제거 | 3 | 같은 메뉴 Retry 경로 2개, 단일 WorldRebase 1개 |
| 합계 | 257 | 원래 실행 사례 기준 |

현재 정적 예상은 **37파일 / 231메서드 / 254실행 사례**다. EditMode는 887사례를 유지한다. 테스트·생산 클래스를 삭제한 것이 아니라 중복 흐름의 3메서드를 다른 테스트에 통합했다. 버전 관리 이력으로 이전 코드를 복구할 수 있다.

이 결과는 PlayMode 254사례를 실행해 성공했다는 뜻이 아니다. Unity 컴파일·빌드·Test Runner·원격 호출은 수행하지 않았다. 생산 코드와 Scene/Prefab은 수정하지 않았다.

## 카테고리

| 카테고리 | 파일 수 | 수정 전 사례 | 수정 후 사례 |
| --- | ---: | ---: | ---: |
| 이동·물리·카메라 | 7 | 45 | 45 |
| 게임·입력·메뉴 | 7 | 40 | 38 |
| Infinite 진행·패턴 | 5 | 52 | 51 |
| 수집물 | 4 | 38 | 38 |
| 화면·Scene 참조 | 5 | 25 | 25 |
| 설정·저장 | 3 | 3 | 3 |
| Stage·타이머 | 3 | 30 | 30 |
| 계정 UI·검증 격리 | 3 | 24 | 24 |
| 합계 | 37 | 257 | 254 |

파일별 소속과 사례는 전체 CSV 및 `node Tools/Tests/playmode-test-review.cjs` 출력에서 확인할 수 있다.

## 통합한 3사례

1. `PauseMenuIntegrationTests.RetryButtonClick_MatchesDirectRetry`: 이름과 달리 직접 Retry와의 비교가 없고 같은 Pause Retry 버튼을 눌러 독립 Run/Playing을 검사했다. `RetryButton_StartsOneIndependentRun`에 클릭 직후 상태와 독립 Run 확인, 두 프레임 후 같은 Run 유지 검증을 옮긴 뒤 제거했다.
2. `ResultMenuIntegrationTests.ResultMenu_RetryButtonStartsNewStageRun`: 같은 fixture의 MouseClickRetry와 같은 Result Retry 버튼/Playing 경로다. `ResultMenu_MouseClickRetry_StartsNewStageOnce`에 클릭 직후 Playing/Stage와 다음 프레임 Run 동일성을 보강한 뒤 제거했다.
3. `InfiniteModeIntegrationTests.WorldRebase_ProductionScenePreservesRunStateAndRelativePositions`: 같은 설정/임계점으로 반복 재배치와 겹친다. 단일 사례에만 있던 Rigidbody 속도 보존 검사를 반복 사례의 매 재배치에 추가하고 metrics 갱신 성공도 확인한 뒤 통합했다. 100회 스트레스 테스트는 스코프/등록 수 누적 누수라는 별도 계약이므로 유지했다.

## 보강한 검증

후속 사용자 실행에서 Stage/Infinite Submit Retry 2사례가 Ended에 머무르는 실패를 보고했다. 초기 변경은 UIInputSystem의 Submit 플래그만 설정했지만 GameSystem은 Cancel을 판정하고 나머지 transient 입력을 소비하며, 버튼 Submit은 EventSystem이 담당한다. 테스트의 입력 주입 경계를 잘못 선택한 것이다. 두 테스트를 선택된 실제 Retry 버튼의 `ExecuteEvents.submitHandler` 이벤트로 변경하고, Click+Submit 2사례에도 실제 Submit 이벤트를 추가했다. 직접 SelectRetry 호출로 되돌리거나 생산 코드/Scene을 변경하지 않았다. 수정 후 정적 재검사는 수행하며, 실제 InputSystem/OS 키 이벤트부터의 전체 경로를 검증했다고 주장하지 않는다.

- `PauseClickAndSubmit_RetryExecutesOnlyOnce`, `ResultClickAndSubmit_RetryExecutesOnlyOnce`: 원래는 Submit을 넣지 않았다. 같은 프레임에 Click과 실제 Unity Submit 이벤트를 전달하고, 클릭 직후 생성한 Run을 다음 프레임과 비교하며 transient 입력 소비를 확인한다. 클릭 직후 캡처하므로 즉시/다음 프레임 중복 Retry를 놓치지 않는다.
- `ResultMenu_SubmitRetry_StartsNewStageOnce`: 원래 직접 `SelectRetry`만 호출했다. 이제 선택된 버튼에 EventSystem Submit 이벤트를 보내 Stage 시작을 검사하며 다음 프레임 Run 보존도 확인한다.
- `InfiniteResult_KeyboardSubmitRetry_StartsInfiniteRun`: 직접 SelectRetry 호출을 UI Submit 경로로 교체했다. 실제 OS 키보드 이벤트 검사는 아니므로 이름도 `InfiniteResult_UISubmitRetry_StartsInfiniteRun`으로 정정했다. 물리 입력 재지정은 별도 SettingsInteractiveRebind 테스트가 담당한다.
- `ModeResultDisplayIntegrationTests`의 Initialize 후 표시 정리, 연속 Infinite 결과, Infinite→Stage 결과 세 사례: 이전 결과 설정 성공과 실제 129점 표시를 먼저 확인한다. 이전 화면을 만들지 못했는데 이미 빈 필드를 지워도 통과하는 경우를 방지한다. 후속 결과 설정 성공도 확인한다.
- `InfiniteHudIntegrationTests.Retry_NewRuntimeDataResetsHudToZero`: Retry 전에 갱신 성공과 Distance 12/Score 129/Collectible 10 표시를 확인한 뒤 0 초기화를 검사한다.
- `InfiniteMapPatternTests.ResetPatterns_AfterAdvance_RestoresInitialTransforms`: Initialize/두 Advance 성공과 횟수 2를 확인한 뒤 초기화를 검사한다.
- `LocalPersistenceIntegrationTests`: 볼륨 변경 성공 확인, 생성한 두 객체를 finally에서 정리하고 원래 AudioListener 볼륨 복구. 이 테스트는 저장 데이터의 새 컴포넌트 적용 계약이며 실제 디스크 재시작 영속성의 증명은 아니다. 파일 직렬화·복구는 별도 EditMode 저장 테스트의 계약이다.

## 이름·대기 정리

- `Flat_StationaryStart_AcceleratesAndRemainsGrounded`를 `Flat_StationaryStart_UsesFixedSpeedAndRemainsGrounded`로 변경했다. 생산 규칙이 고정 수평 속도이므로 가속을 검증했다고 표현하지 않는다.
- `ControlledSeed_ProductionSelection_ActivatesDifferentPattern`의 마지막 assertion 이후 수동 시각 확인을 위해 붙어 있던 5초 realtime 대기와 FreezeAll 블록을 제거했다. 자동 검사에 사람이 화면을 관찰할 시간을 넣을 이유가 없다. 실제 총 실행 시간 개선은 측정하지 않았다.

## 유지 판단

- EditMode와 비슷한 이름이어도 실제 Rigidbody/Trigger/Scene 참조/프레임 처리와 연결되는 검사는 다른 계층이므로 삭제하지 않았다.
- 왼쪽/오른쪽 벽, 낮은/높은 접근 속도, Stage/Infinite, 낙하 경계 직전/정확한 경계/직후는 별도 입력 계약이다.
- 전체 16개 패턴 연결을 helper에서 반복 검사하는 테스트는 1사례로 계산되지만 실제 16쌍을 검증한다. wrapper에 직접 assertion이 없다는 이유로 무검증 테스트로 취급하지 않았다.
- 텍스트 포맷/상태 규칙 단위 검사와 생산 HUD 갱신/유지/Inspector 연결 검사는 구별했다.
- 검증 창의 순수 문자열 allowlist 함수 10사례는 프레임을 필요로 하지 않아 향후 EditMode 배치로 정리할 여지는 있다. 현재 중복 사본은 없으며 삭제 대상은 아니다. 이번에는 완료된 EditMode 구성을 변경하면서 단지 수를 이동시키지 않았다.
- 계정 UI 테스트는 사용자가 구성한 실제 Scene 참조를 이용하고 가짜 전송만 주입하는 방식을 유지한다. UI나 참조를 자동 생성하도록 바꾸지 않았다.

## 검증 및 목록

- [수정 전 257사례별 목록](20261006_06_PlayModeTestAudit_Cases.csv): 각 입력/실행 방식, 카테고리, 소스 위치, 판정 근거, 본문 해시 및 직접 assertion 수.
- [18사례 조치 목록](20261006_07_PlayModeTestActions.csv): 수정 후 위치/해시. 통합 제거 행은 제거 전 위치/해시.
- 전체 카테고리 합계, 239개 유지 본문 해시 불변, 15개 수정 본문과 조치 목록의 일치 및 3개 통합 제거: 정적 검사 통과.
- Submit 주입/순서/Run 캡처, 재배치 속도 보존, 준비 값 표시, finally 정리, 5초 대기 제거, 미지원 Is.AnyOf 재도입 없음: 정적 검사 통과.
- 기존 EditMode 목록 검사: 887사례 유지 확인.
- C# 소스 사전 검사: 중복 선언·구분자·meta GUID·asmdef·온라인 격리 검사 통과. 컴파일의 대체가 아니다.
- 변경한 tracked PlayMode 파일의 whitespace 검사: CRLF를 정상 줄바꿈으로 취급하면 통과. Git 줄바꿈 안내는 Unity Warning 결과가 아니다.

```powershell
node Tools/Tests/playmode-test-review.test.cjs
node Tools/Tests/editmode-test-review.test.cjs
node UGS/Tests/client-unity-preflight.cjs
```

정적 검토는 mutation coverage, 실제 실행 시간, 모든 중복의 부재 또는 최소 테스트 집합임을 증명하지 않는다. 유지 권고는 현재 소스 검토에서 제거 근거를 찾지 못했다는 판단이다.

## 사용자 확인

Unity Editor에서 Script Compilation과 예상하지 않은 Error/Warning 유무를 확인한 뒤, PlayMode 전체 테스트를 실행한다. 예상 수는 **254사례**다. 실패하면 테스트 이름·메시지·Stack Trace를 전달한다. 이번 조치로 필요한 Scene 수동 작업이나 Module 재배포는 없다.

## 최종 사용자 검증 결과 — 완료

Submit 이벤트 수정 후 사용자가 다음 결과를 보고했다.

- Unity Script Compilation 성공, 예상하지 않은 Error/Warning 없음.
- EditMode **887/887** 성공, 예상하지 않은 Error/Warning 없음.
- PlayMode **254/254** 성공, 예상하지 않은 Error/Warning 없음.

정적 검사와 사용자 실행 결과를 근거로 PlayMode 테스트 검토·정리 및 후속 Submit 테스트 수정을 완료 처리한다. 전체 테스트 정리 후 실행 사례는 **1,141개**다. AI가 Unity 컴파일·빌드·Test Runner를 실행한 결과가 아니며, 별도 Player 빌드 성공이나 원격 배포 검증을 의미하지 않는다.
