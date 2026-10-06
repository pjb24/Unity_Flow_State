# EditMode 테스트 필요성 정적 검토

검토일: 2026-10-06. 대상: 현재 작업 트리의 `Assets/Tests/EditMode`.

후속 구현 기록: [20261006_03_EditModeTestAuditActions.md](20261006_03_EditModeTestAuditActions.md). 이 문서와 909행 CSV는 수정 전 검토 이력이다. 현재 목록과 정적 검사 명령은 사용자 승인에 따른 레거시 삭제까지 반영해 605메서드 / 887사례를 확인한다.

## 결론

909개라는 수만으로 과다하다고 판단할 수 없다. 정상 동작뿐 아니라 실패 입력, 저장 실패, 재시작, 재전송, 계정 이전, 상태 보존을 별도로 검사하는 사례가 대부분이다. 다만 모든 사례가 독립적으로 꼭 필요하다고 판단하지도 않았다.

| 판정 | 실행 사례 수 | 의미 |
| --- | ---: | --- |
| 유지 권고 | 871 | 현재 정적 검토에서 제거할 근거를 찾지 못함 |
| 검증 보강 | 10 | 목적은 필요하지만 이름에 해당하는 검증 또는 입력 조합이 부족함 |
| 이름 정정 | 2 | 검사하는 속도와 이름이 다름. 테스트 목적은 유지 |
| 중복 제거 후보 | 2 | 다른 파일과 같은 함수·같은 입력·같은 기대값 |
| 보강 후 통합 후보 | 1 | 다른 테스트에 남은 검증을 옮긴 뒤 통합 가능 |
| 레거시 정책 확인 | 23 | 사용처 없는 이전 점수 계산 API를 유지할지 먼저 결정 |
| 합계 | 909 | 모든 실행 사례에 판정 부여 |

테스트·생산 코드·Scene을 변경하거나 테스트를 삭제하지 않았다. Unity 컴파일, 빌드, Unity Test Runner도 실행하지 않았다. 이 문서의 숫자는 실행 성공 수가 아니라 소스에서 확인한 사례 수다.

## 범위와 검토 방법

- 테스트 파일 53개, 테스트 메서드 620개, 실행 사례 909개를 확인했다.
- `[Test]` 단일 사례는 517개이고, `[TestCase]`로 정의한 사례는 392개다. 같은 메서드의 서로 다른 입력값도 별도 검토 대상으로 기록했다.
- 현재 NUnit `[Category]` 분류는 없다. 아래 분류는 이번 검토를 위한 기능별 분류이며 코드에 Category를 추가하지 않았다.
- 각 메서드의 본문을 확인하고 입력 조건, 생산 코드 호출, assertion 및 helper 검증, 상태 변경 전후, 다른 계층의 유사 검증과 비교했다. 의심 사례는 관련 생산 코드를 대조했다.
- 직접 assertion이 0개인 메서드도 helper에서 검사할 수 있다. 예를 들어 UIVisibility 및 지형의 AssertWindow 호출은 무검증 테스트로 취급하지 않았다.
- CSV의 호출 단서는 일부만 표시하며 helper 또는 테스트 대역 호출도 포함한다. 호출 개수나 assertion 개수로 필요성을 자동 판정하지 않았다.
- 정적 검토는 코드 커버리지, 변경을 주입하는 mutation testing, 실제 실행 시간 측정 또는 모든 중복의 부재를 증명하지 않는다. `유지 권고`는 최소 테스트 집합임이 증명됐다는 뜻이 아니다.

## 카테고리별 확인

| 카테고리 | 파일 수 | 사례 수 | 확인하는 계약 |
| --- | ---: | ---: | --- |
| 이동·착지 | 5 | 44 | 이동/점프/착지의 정상·거부 입력 및 수학 경계 |
| 모멘텀·점수 | 6 | 121 | 점수 누적·표시·버전과 런타임 모멘텀 전달 |
| 월드 재배치·카메라 | 4 | 38 | 좌표 변경 후 거리·속도·카메라 연속성 |
| Infinite 진행 상태 | 4 | 92 | 시작/중단/재시작, 거리·난이도·런타임 상태 |
| Infinite 패턴·수집물 | 10 | 157 | 데이터·연결·선택·배치·통과 가능성·중복 수집 방지 |
| 게임·UI·설정 | 6 | 112 | 게임 상태·메뉴 이동·화면 표시·설정·키 바인딩 |
| 결과·시간·기록 | 6 | 93 | 결과 확정·기록 비교·시간/점수 표시 |
| 저장·온라인 기록 | 7 | 97 | 저장 무결성·격리·재전송·제출/조회 정책 |
| 계정·이전 | 3 | 82 | 인증·동의·이전·복구·개인 최고 기록 적용 |
| 검증 도구·모듈 연결 | 2 | 73 | 검증 환경 격리·원격 호출 제한·기준 비교·함수 연결 |
| 합계 | 53 | 909 | |

### 파일별 사례 수

| 카테고리 | 파일별 실행 사례 수 |
| --- | --- |
| 이동·착지 | JumpFeature 5, NormalLandingFeature 2, MomentumLandingFeature 14, PlayerMovementMath 10, PlayerSurfaceMath 13 |
| 모멘텀·점수 | MomentumScoreState 22, MomentumHudPresentation 15, MomentumProductionState 38, InfiniteScoreState 14, ScoreCalculator 23, ScoringVersion 9 |
| 월드 재배치·카메라 | WorldRebaseState 25, PlayerWorldRebase 6, CameraWorldRebase 5, CameraFollow 2 |
| Infinite 진행 상태 | InfiniteModeState 27, InfiniteDistanceState 19, InfiniteDifficultyState 20, InfiniteModeRuntimeData 26 |
| Infinite 패턴·수집물 | InfinitePatternAuthoring 22, InfinitePatternCatalogFactory 6, InfinitePatternCatalog 7, InfinitePatternDefinition 18, InfinitePatternGeometry 24, InfinitePatternSelectionState 22, InfinitePatternSlotProgression 16, InfinitePatternTraversalMath 15, InfiniteCollectibleLayout 5, CollectibleRuntimeData 22 |
| 게임·UI·설정 | GameState 19, GameRuntimeData 18, GameNavigationState 35, UIVisibilityState 18, SettingsState 18, HowToPlayBindingFormatter 4 |
| 결과·시간·기록 | TimeRecord 11, TimerRuntimeData 4, ScoreRecord 20, ResultData 2, ResultSystem 13, ResultTextFormatter 43 |
| 저장·온라인 기록 | LocalSaveJsonCodec 11, OnlineLocalSaveScope 25, OnlineRecordRepository 26, OnlineRecordConfiguration 5, RecordSubmissionPolicy 17, RecordLeaderboardPolicy 8, LeaderboardViewState 5 |
| 계정·이전 | AccountTransferCompletion 30, AccountTransferController 22, OnlineAccountCoordinator 30 |
| 검증 도구·모듈 연결 | VerificationSession 59, CloudCodeModuleRouting 14 |

표의 파일명은 모두 `Assets/Tests/EditMode/<이름>Tests.cs`에 대응한다.

## 1. 명확한 중복: 2사례

아래 테스트는 Geometry 자체를 호출하지 않고 TraversalMath 쪽과 같은 인자로 같은 반환값을 검사한다. Geometry의 fixture에 추가 Setup도 없다.

- `InfinitePatternGeometryTests.cs:209` — `TraversalContract_HeightAboveJumpApex_IsRejected`: `CanTraverseJump(4.0f, 3.011f, 8.0f, 8.0f)`가 false. `InfinitePatternTraversalMathTests.cs:39`와 중복.
- `InfinitePatternGeometryTests.cs:218` — `FixedSpeedTraversalContract_ShortLanding_IsAccepted`: `CanTraverseJump(4.0f, 0.0f, 4.0f, 4.0f)`가 true. `InfinitePatternTraversalMathTests.cs:51`과 중복.

수학 테스트를 남기고 Geometry 쪽 두 사례를 제거하는 것은 타당한 후속 작업이다. 이번 요청에서는 실제로 제거하지 않았다.

## 2. 보강 후 통합 가능: 1사례

`WorldRebaseStateTests.cs:62`의 `RebasePreservedDistance_ProducesSameScoreAndDifficultyInput`은 한 번의 재배치 후 거리·점수·난이도 입력 보존을 확인한다. 같은 파일 `:94`의 `RepeatedRebase_PreservesScoreAndDifficultyContinuity`는 두 번의 재배치와 성공 반환, 점수·난이도 보존을 더 상세히 확인한다.

후자에 첫 재배치 전후 논리 거리 동일 검증을 명시적으로 옮긴 뒤 전자를 통합할 수 있다. 기존 assertion을 옮기지 않은 즉시 삭제는 권고하지 않는다.

## 3. 목적은 필요하지만 검증 보강: 7메서드 / 10사례

| 위치 | 문제 | 권고 |
| --- | --- | --- |
| InfinitePatternSelectionStateTests.cs:185 — SameCatalogSeedAndRequests_ProducesSameSequence | StartRun/선택 반환 성공 미확인. 양쪽이 모두 실패하면 null==null로 통과 가능 | 시작과 매 선택 성공, 유효 ID 확인 후 시퀀스 비교 |
| InfinitePatternSelectionStateTests.cs:158 — TrySelectNext_WhenAlternativeExists_RespectsRepeatLimit | 실패/null 선택이 연속 선택 카운트를 끊어 제한 위반을 숨길 수 있음 | 매 선택 성공 및 ID 유효성 확인 |
| GameNavigationStateTests.cs:550 — ShouldShowDifficulty_RequiresDevelopmentContextAndSetting (4사례) | 핵심 참/거짓 조합 누락, 기대값도 생산 공식과 같은 식으로 계산 | 세 bool의 전체 8조합과 고정 기대값으로 변경 |
| MomentumProductionStateTests.cs:227 — Finalize_DuplicateRequestDoesNotChangeRuntimeValues | Finalize를 한 번만 호출하므로 중복 Finalize를 검증하지 않음 | 두 번째 Finalize 후 불변 확인 또는 종료 후 진행 거부에 맞게 이름 정정 |
| AccountTransferCompletionTests.cs:401 — AccountHandoff_ClearsPriorRuntimeTerminalReceiptsAndCounters | 실제 계정 이전 없이 ClearSessionHistory를 직접 호출 | 초기화 단위 테스트로 이름 정정 또는 실제 handoff 경로가 초기화를 호출함을 검증 |
| OnlineRecordRepositoryTests.cs:354 — Timeout_RemainsPending | 반환값 TransientFailure만 확인. Pending 큐는 사용하지 않음 | SubmitTimeout_ReturnsTransientFailure로 이름 정정 또는 큐의 동일 ID 보존까지 확인 |
| CloudCodeModuleRoutingTests.cs:10 — NewTransport_DiagnosticDoesNotInitializeSdkOrContainCredentials | 초기 진단 문자열만 확인. SDK 미초기화를 별도로 관찰하지 않음 | 초기 진단 상태에 맞게 이름 정정 또는 SDK 호출 없음 확인 |

### 난이도 표시 테스트의 정적 반례

생산 규칙은 `display && (editor || development)`다. 현재 입력 4개는 아래와 같다.

| editor | development | display | 기대값 |
| --- | --- | --- | --- |
| false | false | false | false |
| false | true | false | false |
| true | false | false | false |
| true | true | true | true |

잘못된 `display && editor && development`도 이 4개를 전부 만족한다. 특히 `(true, false, true)`와 `(false, true, true)`를 추가하면 이 오류를 구별할 수 있다. 이는 입력표를 대조한 논리 반례이며 Unity에서 변형 코드를 실행했다는 뜻이 아니다.

## 4. 이름 정정: 2사례

- `InfinitePatternGeometryTests.cs:91` — `EveryTransition_ProvidesMinimumWindowAtBothSpeeds`는 BaseHorizontalSpeed 하나만 검사한다.
- `InfinitePatternTraversalMathTests.cs:27` — `CanTraverseJump_SelectedBoundaryGap_IsAcceptedAtBothSpeeds` 역시 두 속도를 구분해 검사하지 않는다.

현재 고정 속도 동작에 맞게 이름을 `FixedSpeed` 기준으로 정정한다. 필요하지 않은 두 번째 속도 규칙을 만들거나 이 테스트를 삭제할 이유는 없다.

## 5. 레거시 정책에 따라 결정: 23사례

`ScoreCalculatorTests`의 12메서드 / 23사례는 이전 점수 계산기의 정상 계산, 반올림/포화, 비정상 입력, 초기화 등을 검증한다. 검증 자체는 유효하다. 그러나 현재 `Assets/Scripts`에서 `ScoreCalculator`를 소비하는 참조는 검색되지 않으며 클래스 정의만 남아 있다. 현재 Infinite 런타임은 InfiniteDistanceState와 InfiniteScoreState를 사용한다.

권고: ScoreCalculator API를 호환 목적으로 유지할지를 먼저 결정하고, 미사용 클래스와 그 23사례를 함께 정리할지 검토한다. 테스트만 삭제하거나 v1 저장 기록/마이그레이션 검증까지 일괄 삭제하지 않는다. v1 데이터를 읽거나 명시적 레거시 진입점을 지원하는 다른 테스트는 이 후보와 구분했다.

## 6. 중복처럼 보이지만 유지할 사례

- 0, 음수, NaN, Infinity, 경계 직전/정확한 경계/직후는 입력의 의미가 다르다. 입력 수가 많다는 이유로 합쳐 삭제하지 않았다.
- 같은 패턴 쌍에 대한 Catalog 연결 검사, Geometry의 실제 통과 가능성, Collectible 배치 검사는 서로 다른 계층의 계약이다.
- Repository 반환값 검증과 Coordinator의 Pending 저장/재시작 검증은 별개다.
- `ResultTextFormatterTests`의 유효/무효 Stage 시간 메서드는 본문이 같지만 TestCase 입력 및 기대 문자열이 다르다. 같은 본문 해시만으로 중복 삭제를 결정하면 안 된다.
- 컴포넌트를 직접 구성하거나 메서드를 호출하는 EditMode 검사는 바인딩·실행 순서·전달 계약을 검사한다. 실제 Scene, 프레임 및 물리 동작을 검사하는 PlayMode 테스트의 대체라고 주장하지 않으며, 비슷한 시나리오라는 이유로 삭제하지 않았다.

## 전체 사례별 목록과 재현

전체 판정: [20261006_02_EditModeTestAudit_Cases.csv](20261006_02_EditModeTestAudit_Cases.csv).

909개 행 각각에 카테고리, fixture, 메서드, TestCase 입력, 소스 위치, 판정, 근거, 호출 단서, 직접 assertion 수를 기록했다. 입력별 판정이 같아도 행을 합치지 않았다. CSV는 Excel의 데이터 → 텍스트/CSV 가져오기로 열고 UTF-8을 선택하면 된다.

읽기 전용 목록 확인:

```powershell
node Tools/Tests/editmode-test-inventory.cjs
node Tools/Tests/editmode-test-review.cjs
node Tools/Tests/editmode-test-review.test.cjs
```

마지막 명령은 현재 목록/분류/조치 CSV 및 수정 전 909개 이력의 관계를 검사하는 Node 검사다. Unity EditMode 테스트 실행이 아니다. 검토 도구는 테스트 코드를 수정하거나 Unity/네트워크를 호출하지 않는다.

후속 우선순위: 검증 보강 → 명확한 중복 2개 정리 → 거리 검증을 옮긴 뒤 1개 통합 → 레거시 API 정책 결정. 전체 조합 보강은 사례 수를 늘릴 수도 있으므로 단순히 최종 개수를 줄이는 것을 완료 기준으로 삼지 않는다.
