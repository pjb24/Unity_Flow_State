# EditMode 테스트 검토 후 조치

작업일: 2026-10-06.

요청: 최초 909사례 검토에서 유지 권고를 제외한 38사례를 재검토하고 조치한다.

## 적용 결과

| 최초 판정 | 최초 사례 수 | 조치 |
| --- | ---: | --- |
| 검증 보강 | 10 | 패턴 선택 성공/ID 검증, 난이도 전체 8조합, 실제 Pending 저장/재로드 검증 추가. 과장된 이름 3메서드는 실제 검사 내용으로 정정 |
| 이름 정정 | 2 | BothSpeeds → FixedSpeed |
| 중복 제거 후보 | 2 | Geometry 쪽 중복 제거. TraversalMath의 원래 검사 유지 |
| 보강 후 통합 후보 | 1 | RepeatedRebase 테스트에 거리·난이도 보존 검증을 옮긴 뒤 단일 사례 제거 |
| 레거시 정책 확인 | 23 | 최초 보존 후 사용자 명시적 승인으로 미사용 클래스·테스트와 각각의 meta 삭제 |

현재 정적 목록은 **52파일 / 605메서드 / 887실행 사례**다. `909 - 중복 2 - 통합 1 + 빠진 bool 입력 조합 4 - 승인된 레거시 23 = 887`이다. 테스트 수 감소보다 잘못된 동작을 판별하는 검증을 우선했다. 887개를 실행해 성공했다는 의미는 아니다.

생산 코드에서는 승인된 미사용 ScoreCalculator 클래스만 제거했다. Scene, PlayMode 테스트 및 원격 환경은 수정하지 않았다. 유지 권고 사례는 변경하지 않았으며, 단일 재배치 검증을 옮기는 대상으로 선정한 RepeatedRebase 테스트에만 필요한 assertion을 추가했다.

## 코드 변경

- `InfinitePatternGeometryTests`: 수학 테스트와 같은 함수·입력·기대값인 2메서드 제거. 고정 속도 테스트 이름 정정.
- `InfinitePatternTraversalMathTests`: 고정 속도 테스트 이름 정정. 중복과 대응하는 높이/짧은 착지면 수학 테스트는 유지.
- `WorldRebaseStateTests`: 첫째·둘째 재배치 전후 논리 거리와 난이도 동일 assertion을 추가. 기존 점수 불변/성공 반환 검증과 함께 보존하고 단일 재배치 메서드 통합.
- `InfinitePatternSelectionStateTests`: 시작 및 매 선택 성공, 허용 ID를 확인한 뒤 반복 제한과 같은 시드 시퀀스를 검사. false/null 두 결과가 같은 값이라는 이유로 통과하는 경우 차단.
- `GameNavigationStateTests`: 세 bool의 전체 8조합을 명시적 기대값으로 검사. 생산 공식을 기대값 계산에 재사용하지 않음.
- `OnlineRecordRepositoryTests`: 타임아웃 테스트에서 실제 LocalRecordRepository/Coordinator를 연결. 제한된 재시도 후 같은 submissionId Pending 1건 유지, terminal receipt 없음, 재로드 후 ID 보존 확인. 메모리 파일 대역과 즉시 완료되는 대기 대역을 사용해 네트워크·실제 1초/2초 대기가 없음. 생산 코드의 명시적인 타임아웃 Warning만 예상 로그로 등록.
- `AccountTransferCompletionTests`: handoff 통합 검사가 아닌 ClearSessionHistory 단위 검사임을 이름에 반영. 초기 enqueue 성공과 Submitted receipt 존재를 확인한 뒤 초기화 결과 검사.
- `MomentumProductionStateTests`: 중복 Finalize 검사가 아닌 종료 후 ProcessRunMetrics 거부 및 값 보존 검사로 이름 정정. private Finalize의 지원되지 않는 중복 직접 호출을 새 계약으로 만들지 않음.
- `CloudCodeModuleRoutingTests`: 생성 직후 NotRequested 진단 상태 검사에 맞게 이름 정정. SDK 미초기화까지 증명했다는 표현 제거.

## 레거시 23사례: 승인 후 삭제 적용

현재 `ScoreCalculator`의 소비자는 자체 테스트뿐이다. 생산 C# 코드에서 참조가 없고, 해당 클래스와 테스트의 meta GUID를 소비하는 에셋/설정도 검색되지 않았다. Prototype 5 작업 이력에는 생산 점수 계산기를 InfiniteScoreState로 교체한 기록이 있다.

최초에는 명시적 삭제 승인이 없다는 승인 검토 결과로 변경이 차단되어 보존했다. 이후 사용자가 “승인한다”라고 명시적으로 승인했으며, 참조와 GUID 소비자 부재를 다시 확인한 뒤 아래 4개 파일을 삭제했다.

- `Assets/Scripts/Runtime/Features/ScoreCalculator.cs` 및 `.meta`
- `Assets/Tests/EditMode/ScoreCalculatorTests.cs` 및 `.meta`

기존 v1 저장 데이터, 마이그레이션, Result/Runtime의 명시적 호환 API는 유지한다. 삭제한 파일은 버전 관리 이력으로 복구할 수 있다.

## 정적 검증

후속 사용자 컴파일 실패 `CS0117: Is.AnyOf`를 확인했다. 현재 NUnit에서 지원하지 않는 표현을 두 곳 모두 `Is.EqualTo(...).Or.EqualTo(...)`로 교체했다. 허용 ID 검사와 실패/null 거부는 유지하며, 정적 회귀 검사에도 해당 미지원 표현의 재도입 방지를 추가했다. AI는 Unity 컴파일·빌드·Test Runner를 실행하지 않았다.

- 목록과 조치 CSV 일치: 통과. 현재 605메서드/887사례와 최초 909행 이력 관계 확인. 제거한 23개 레거시 사례는 최초 목록으로부터 조치 이력에 보존.
- 870개 미수정 유지 사례의 메서드·입력 식별자와 직접 assertion 수가 최초 목록과 일치. 통합 대상 1사례는 별도 표시.
- 전체 bool 조합/고정 기대값, 선택 성공 검사, 재배치 보존 assertion, 타임아웃 저장 재로드 검사의 소스 존재 확인: 통과.
- 기존 C# 소스 사전 검사: 통과. 중복 멤버 선언, 구분자, meta GUID, asmdef 및 격리 구성을 검사. C# 컴파일의 대체가 아님.
- ScoreCalculator 소비자 참조 및 삭제한 meta GUID 참조 부재 재확인. 소스 사전 검사는 삭제 파일을 읽으려 하지 않도록 Git diff의 삭제 항목을 검사 대상에서 제외.
- 변경된 tracked EditMode 파일의 whitespace 검사: CRLF를 정상 줄바꿈으로 취급하면 통과. Git의 LF→CRLF 안내는 Unity Warning 결과가 아님.
- Unity 컴파일/빌드/Test Runner/Scene 변경/원격 호출: 수행하지 않음.

정적 검사 명령:

```powershell
node Tools/Tests/editmode-test-inventory.cjs
node Tools/Tests/editmode-test-review.test.cjs
node UGS/Tests/client-unity-preflight.cjs
```

조치별 목록: [20261006_04_EditModeTestActions.csv](20261006_04_EditModeTestActions.csv). 원래 대상 38사례, 추가 bool 조합 4사례, 통합받은 기존 테스트 1사례를 합해 43행이다. 제거한 3메서드 행의 소스 위치는 제거 전 위치다. 최초 보고서와 909행 CSV는 수정 전 검토 이력으로 보존한다.

## 사용자 검증

1. Unity Editor에서 Script Compilation 결과와 예상하지 않은 Error/Warning 유무를 확인한다.
2. Unity Test Runner의 EditMode 전체 테스트를 실행한다. 현재 정적 예상 수는 887사례다.
3. 실패가 있으면 테스트 이름, 실패 메시지, Stack Trace를 전달한다.

이 작업에는 Scene 수동 작업이나 Module 재배포가 필요하지 않다. 원래 대상 38사례의 코드 조치를 반영했다.

## 최종 사용자 검증 결과 — 완료

사용자가 최종 수정 후 Unity Script Compilation 성공과 예상하지 않은 Error/Warning 없음을 보고했다. EditMode **887/887**, PlayMode **254/254** 모두 성공했으며 각각의 테스트에서도 예상하지 않은 Error/Warning이 없음을 확인했다.

정적 검사와 사용자 실행 결과를 근거로 EditMode 테스트 검토·정리 작업을 완료 처리한다. 이 결과는 사용자 실행 보고이며 AI가 Unity 컴파일·빌드·Test Runner를 실행한 것이 아니다. 별도 Player 빌드 성공이나 원격 배포 검증을 의미하지 않는다.
