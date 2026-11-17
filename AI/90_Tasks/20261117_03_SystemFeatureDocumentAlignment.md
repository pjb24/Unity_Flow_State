# 작업 정보

## 작업명

Systems/Features 문서의 Infinite 단일 흐름·현재 구현 범위 정합성 정리

## 작업 일자

20261117

## 작업 목적

사용자 검토 요청에서 확인한 옛 Stage/계정·조회 규칙과 지난 검증 상태를 현재 목표/구현에 맞게 정리한다. 문서 정비이며 Phase 3 생산 코드 구현 착수를 뜻하지 않는다.

# 작업 내용

- RecordSubmission의 계정 이전 미지원/기기 변경 후 일괄 복구 불가 설명을 기존 자격 증명 기반 이전과 기존 기기 사용 불가 시 복구 제한으로 수정했다. 후보의 로컬 소유 ID·인증 Player ID·서버 고정 소유 ID를 구분했다.
- Leaderboard의 본인 확정 기록 선행 조건을 제거하고 동의/인증·활성 연결 조건과 서버 Empty/내 기록 없음/실패를 구분했다. 현재 GameSystem/CloudCodeRecordRepository 조회와 FirstLeaderboardContinue 계열 Test를 근거로 삼았다.
- Stage 후보/보드 규칙을 Deprecated 내부 계약으로 분리했다. 생산 사용자 UI 차단 완료와 미착수인 Phase 3 직접 factory/Repository/Editor 차단을 별도로 명시했다. 계정 이전·저장/온라인 연결 전체가 미구현인 것처럼 읽히는 옛 Phase 설명도 수정했다.
- GamePause는 Infinite Run으로 범위를 명확히 하고 확인창/Settings의 Cancel과 PausePanel Resume을 구분했다. Retry 확인 전 상태 보존/기본 Cancel·확인 후 단일 새 Run을 검증 항목에 반영했다.
- StagePlay/StageSystem/TimerSystem의 공통 Run·보존된 이름/키와 사용자 Stage Mode를 구분했다. StageClear/TimeRecord의 목적·검증은 Deprecated 독립 모델로 한정했다. Result 링크/최고 표시는 Infinite Board/점수 규칙으로 정리했다.
- Phase 2 완료 상태와 근거 Task 링크를 갱신하고 테스트 수/좌표 등 상세 실행 기록은 Task를 참조하도록 했다. InfiniteMode의 과거 개발 Phase 번호를 현재 규칙/검증 책임과 구분했다. ScoreCollectible/ScoreRecord의 Stage 선택 전제도 정리했다.

# 영향 범위

| 영역 | 문서 |
| --- | --- |
| Systems 7개 | GameSystem, UIManagementSystem, ResultSystem, StageSystem, TimerSystem, RuntimeDataSystem, RecordSubmissionSystem |
| Features 13개 | RecordSubmission, Leaderboard, ResultMenu, StagePlay, InfiniteMode, StageClear, TimeRecord, AccountTransfer, GamePause, ScoreRecord, GameEntryNavigation, HowToPlay, ScoreCollectible |

이 기록과 Phase 3 Task의 관련 기록 링크도 갱신한다. C#/Unit Test·Scene/Prefab·프로필·서버·PROJECT_MEMORY는 수정하지 않는다. 기존 사용자 미커밋 변경을 보존한다.

# 검증 내용

- 수정 문서의 내부 링크/참조와 제거 대상 충돌 문구·제목/공백을 정적으로 검사한다.
- Phase 2 완료/Phase 3 미착수 구분, 지원 규칙과 Deprecated 보존 계약의 분리, 조회/계정·Pause 흐름을 대조한다.
- 작업 전 기록한 비문서 변경 파일 53개의 SHA-256과 작업 후 값을 비교한다. 문서 정비로 실제 생산 guard가 추가됐거나 새 Test가 통과했다고 기록하지 않는다.
- Unity Compile/Runner/빌드·Scene 편집·원격 실행은 수행하지 않는다. 문서만 수정하므로 Unity 재실행을 요구하지 않는다.

# 검증 결과

정적 검사 PASS: 수정 문서 20개와 관련 Task의 로컬 링크 62개·공백/의도하지 않은 문자, Systems/Features 36개 문서의 옛 계정/조회/Stage 선택/검증 상태 문구를 확인했다. 변경 범위의 git diff --check도 통과했다.
작업 전 기록한 비문서 파일 53개의 SHA-256이 모두 일치한다. Phase 3 Task 완료 체크 11개는 미체크로 유지됐다. 생산 코드/Test·Scene/프로필 변경이나 새 Unity 실행 결과는 없다.

# 후속 작업

[Phase 3 Task](Prototype_9/20261117_01_Phase3ManualSteps.md)의 Step 1부터 진입 경계/기준선을 확정한다. 직접 Stage 후보/저장/Repository·Editor 차단의 실제 구현·Unit Test와 사용자 Compile/Runner는 후속 Step에서 수행한다.

# 관련 문서

- [Project Memory](../00_Project/PROJECT_MEMORY.md)
- [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md)
- [Phase 2 완료](Prototype_9/20261112_01_Phase2ManualSteps.md)
- [Phase 3 실행 절차](Prototype_9/20261117_01_Phase3ManualSteps.md)
- [Systems 안내](../02_Systems/README.md)
- [Features 안내](../03_Features/README.md)
