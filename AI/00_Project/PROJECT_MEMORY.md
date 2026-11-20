# Project Memory

최종 정리: 2026-11-20

현재 작업에 필요한 상태·사용자 결정·핵심 계약·남은 의무를 유지한다. 상세 규칙은 Rule/Feature/System, 실행 과정과 검증 근거는 Task/Roadmap을 따른다.
정리 전 이력은 [원문 보관본](../90_Tasks/20261117_02_ProjectMemoryCleanup.md)에 보존했다. 보관본의 과거 대기/다음 작업 문구를 현재 지시로 사용하지 않는다.

## 현재 진행 상태

- **Prototype 9 / Roadmap 9 Phase 1~4 완료(2026-11-18).** [Phase 4 Task Step 10](../90_Tasks/Prototype_9/20261118_01_Phase4ManualSteps.md)의 완료 표에 Compile/1031·316 사용자 성공·16스크립트 exit 0과 최신 Windows-production Candidate02 빌드/Player 6항목 정상 보고를 연결했다. 최종 후보는 `Build/ProductionQA/Candidate02/Unity_Flow_State.exe`의 사용자 2026-11-18 15:55 KST 빌드다. 추가 코드/Test/Scene 수정·재실행 없이 사용자 한정 QA 전환 후보로 완료했으며 일반 공개/Steam 승인은 아니다. 최신 실제 원격 제출/조회·이전/복구·별도 Player 로그 원문 한계와 다음 실제 Module 업데이트 전 복구 시험 의무를 유지한다. AI는 Unity/빌드/Player·원격을 실행하지 않았다.
- **다음 작업:** [Prototype 10 Phase 3 Task](../90_Tasks/Prototype_10/20261120_04_Phase3ManualSteps.md) Step 1 현재성/원래 작업본 보호 기준선을 고정한다. Phase 3 Task 작성은 완료(2026-11-20)했으며 Step 1~10 실제 실행은 미착수다. [인계](../90_Tasks/Prototype_10/20261120_02_Phase3Handoff.md)의 별도 환경·안전 전달/필수 입력·정적 검사/Unit Test·실제 복원/사용자 검증·최종 판정을 순서대로 진행한다. [Phase 2 Task](../90_Tasks/Prototype_10/20261119_05_Phase2ManualSteps.md) Step 1~10과 Phase 2는 완료(2026-11-20)다. 관리/보존·유입/누락/버전/lifecycle·조건부 사용자 실행 절차와 Step 8 Node Test 261개·오늘 CLI 7개/세 조건 판정·보존/인계를 연결했다. 생산 자산/설정·C#·Scene/Prefab·선언/lock·HEAD/index는 보존했으며 실제 도입/제외·lifecycle 요청은 0개다. TMP schema 2의 83파일/폴더 meta 8개·정확한 tgz 27개/지문 110개를 유지한다. Phase 3에는 미커밋 Tools/문서와 최초 안전 hash 기준선 밖 추적 경로 11개의 필요성/안전한 제공 여부도 확인한다. 기존 URP/TMP graph 미해결 31회와 sample orphan의 inventory exit 1은 그대로이며 실제 관련 도입/복원 전 영향/필수 여부를 재검증한다. 실제 manifest/lock 경로 적용·새 lock/설치·취득 권리/계정·Unity 해석·독립 복원은 미검증이다. 현재 사용자 수동 작업은 없다. Phase 3/실제 독립 복원과 Roadmap 10 전체는 미완료이며 Roadmap 11~14 실제 원본 도입 전 전체 완료 및 다음 실제 Module 업데이트 전 복구 시험 의무를 유지한다.
- Phase 2 최신 사용자 검증은 **EditMode 919개 / PlayMode 288개 전체 통과**다. Windows-production으로 `Build/ProductionQA/Candidate02/`에 덮어쓴 최신 Player의 기존 기능·Pause Retry 확인·메뉴 복귀·UI/포커스/로그 정상 범위를 보고받았다. [최종 근거](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md)
- 이전 Candidate02 실물 보존은 사용자가 불필요하다고 결정했다. 같은 경로의 최신 빌드가 현재 기준선이다. 이 결과를 이후 Phase 3 변경의 통과로 승계하지 않는다. AI가 Runner/빌드/Player를 실행하거나 로그/XML을 직접 검증했다고 기록하지 않는다.

## 계속 적용할 사용자 작업 기준

- **AI는 Unity Compile·Build·Test Runner를 실행하지 않는다.** 코드/Test 준비와 정적 검사·격리된 로컬 대역 Test는 AI, Unity 실행과 빌드는 사용자가 담당한다.
- **Scene/Prefab 변경은 사용자가 수행한다.** AI는 저장 파일을 읽기 전용으로 검사하고 필요할 때 실제 Hierarchy/컴포넌트/필드·최종값·저장/원복 절차를 안내한다. 정적 판정 가능한 조사를 사용자에게 넘기지 않는다.
- **생산 UI를 코드로 동적 생성하거나 누락 보충하지 않는다.** 객체·컴포넌트·고정 배치/크기/Anchor/Pivot·스타일 기본값을 Scene/Prefab에 유지한다. Text/활성/포커스/입력/스크롤 등 Runtime 상태 갱신은 허용한다.
- 정책/계산·상태는 EditMode, Scene/입력/물리·시스템 연동은 PlayMode Test를 우선한다. 같은/인접 프레임·빠른 중복 조작·저장 실패·비동기 경합을 수동 재현으로 넘기지 않는다. 소스 검사는 C# 이름 해석/Compile 성공을 증명하지 않는다.
- 기존 승인·사용자 결정으로 가능한 작업은 반복 확인하지 않는다. 실행하지 않은 Test/서비스를 PASS로 기록하거나 과거 면제·승인을 다른 후보/단계에 자동 승계하지 않는다.
- 한글 폰트 도입은 보류 중이며 현재 계정/이전 화면은 영어를 사용한다. 세부 작업/검증 규칙은 [AI Rule](../01_Rules/AI_RULE.md), [Implementation Rule](../01_Rules/IMPLEMENTATION_RULE.md), [Verification Rule](../01_Rules/VERIFICATION_RULE.md)을 따른다.

## Infinite 전환의 확정 범위

- 사용자 플레이는 **Infinite 단일 흐름**이다. Stage 소스·enum 숫자·GUID/직렬화 참조는 삭제하지 않고 Deprecated/이유·대체 경로와 함께 보존한다. Infinite가 사용하는 공통 StageSystem/Pattern/수집물 기반은 이름만 보고 제거하지 않는다.
- Phase 2에서 ModeSelect 없이 Infinite 시작, 과거 선택/대기 값 정규화, 명시적 Stage/무효 Mode 실행 거절, Stage HUD/Goal/결과 차단을 완료했다. 기존 Tutorial 완료 상태를 보존하며 미완료 사용자는 자동 안내의 Start Run 전까지 Run을 만들지 않는다.
- Leaderboard Stage 탭/Navigation/선택과 사용자 Top/Around·Result 조회의 Stage 진입도 선행 차단했다. Step 2~4는 후보 factory·신규 쓰기/개인 최고 조회·직접 Repository/raw 전송·재시도에 거절을 반영했다. 재시도는 만료/checkpoint/인증 전에 지원 Board·귀속/환경을 확인하고 미지원 자료/terminal 집계를 바꾸지 않는다. 내부 Memory/codec·계정 전체 snapshot은 보존하고 Step 5에서 계정 회귀 Test를 보강했다. Step 6은 Stage Editor UI/직접 메서드와 과거 probe를 부작용 전에 거절한다. Step 7은 기존 Infinite UI/복귀를 유지하며 Result 미지원 후보/선택의 표시·Retry·Me 조회와 다른 화면/runtime/후보의 늦은 응답 적용을 차단했다. Infinite/계정 도구·자동 격리 보호는 보존한다. Step 8~11의 정적/로컬 검증·사용자 최신 Compile/Runner 성공 보고와 최종 완료 판정을 연결했다. 후속 Player 통합 QA 후보 판정은 Phase 4 Task에서 완료했다. 미검증 원격/복구 한계는 유지한다.
- **기존 Stage 기록이 없고 앞으로도 제출되지 않는다는 사용자 전제**를 적용한다. Stage 자료 보관/이관·혼합 Pending 호환·전용 만료/폐기·이전 gate 예외 개발은 제외한다. 이 전제를 원격 전수조사 PASS로 바꾸지 않는다.
- 구 Client는 미지원이다. 현재 전환에 서버 계약 변경은 불필요하다. 구 버전 원격 강제 차단·기존 보드/자료 삭제·Infinite 변환/병합을 추가하지 않는다.
- Pause Retry는 기존 확인창을 재사용하고 Cancel 기본 선택이다. 취소/Escape는 같은 Run의 Pause/Retry로 복귀하고 확인만 새 Infinite Run을 만든다. Result Retry는 즉시 실행한다. 명시적 RetryGame 생명주기 API의 기존 직접 호출 계약과 UI 확인 단계를 구분한다.
- 현재 이동은 World +X의 직렬화된 일정 수평 속도를 사용하며 수평 가속/최대 속도 증가 기능은 제거됐다. Momentum Landing은 수평 속도가 아닌 거리 점수 배율에 반영된다. 중력/점프의 수직 규칙은 별개다. Pattern/수집물 제작 좌표 기준 속도는 `8`이며 변경 시 통과 검증·재배치를 함께 수행한다.
- 근거: [Phase 1 정책](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Phase 2 완료](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md), [Phase 3 절차](../90_Tasks/Prototype_9/20261117_01_Phase3ManualSteps.md), [InfiniteMode](../03_Features/InfiniteMode.md), [MomentumLanding](../03_Features/MomentumLanding.md).

## 저장·온라인 기록·계정의 보존 계약

- Run/확정 Result는 Runtime 데이터다. Settings·입력 Binding Override·Tutorial 완료·개인 최고 캐시·Pending은 로컬 영구 데이터이며, 온라인 최고/순위는 서버 논리 계정 데이터다.
- Local Save v7과 Infinite Scoring Version 2/`fs-infinite-v2`의 기존 계약을 유지한다. 인증 Player ID·공개 번호 캐시·최고·Pending은 `(Project ID, Environment ID)`별로 분리하고 Settings/Tutorial/입력 설정은 기기 공용이다. 빌드 환경을 명시하며 사용자 환경 전환 UI는 제공하지 않는다.
- 복구 제한에 대한 사용자 동의를 저장하기 전에는 생산 온라인 요청을 시작하지 않는다. 로컬 후보를 Anonymous 인증 ID로 임의 재귀속/전송하지 않고 기존 동의·계정 귀속 경계를 유지한다.
- 외부 ID 연결은 지원하지 않는다. Anonymous 연결은 하나의 논리 계정에 활성 Player ID 하나를 사용한다. 전체 10자리 공개 번호는 표시 식별자이며 번호만으로 계정을 복구하지 않는다.
- 이전은 서버 발급 8자리 Base32 코드와 9자리 십진 인증값을 사용한다. 이전 기기에서 자격 증명을 발급할 수 없으면 복구하지 않는다. 원문·token·Secret을 저장/로그에 남기지 않고 UI에서 코드/인증값을 개별 복사하며 입력은 일반 붙여넣기, 완료 요청은 별도로 수행한다.
- Leaderboard 행은 논리 계정의 고정 소유 ID에 귀속된다. 이전은 활성 연결만 바꾸며 행·점수·동점 순서·서버 수락 시각을 옮기거나 복사/변경하지 않는다. 연결된 계정 전체 최고로 캐시를 교체하고 recipient의 다른 계정 기록과 합산하지 않는다.
- Infinite Pending은 동일 Submission ID, trigger당 최대 3회 재시도, 180일 만료·내구 저장/실패 보호를 유지한다. terminal 응답 후에도 로컬 commit 성공이 필요하며 Pending 0을 제출 성공으로 간주하지 않는다. 계정 이전은 Pending 0·활성 연결·내구 저장·진행 중 요청 잠금을 유지한다.
- TOP 10/AROUND YOU 최대 7은 독립 요청/상태다. 불완전한 조회를 성공/Empty로 표시하지 않는다. competition rank·공개 번호·출처별 Back/Cancel·stale 응답 보호를 유지한다.
- Stage Board 식별자/합성 모델/codec·계정 snapshot 내부 읽기와 신규 실행 허용을 구분한다. 공유 Memory 저장소가 복원에도 쓰이므로 일괄 guard/필터로 복원 후 자료 누락·자동 삭제를 만들지 않는다. 구 Stage 호환 기능을 새로 개발하라는 의미는 아니다.
- 서버 제출은 Cloud Code 검증을 통과하며 Player 직접 Leaderboard 쓰기는 거절한다. receipt 수명/중복 ID·CAS·응답 유실/경합 계약은 유지하고 여러 컨테이너 전체 transaction을 가정하지 않는다. 예전 128 ID/100명 검증 한도를 현재 운영 한도로 재도입하지 않는다.
- 근거: [RecordSubmission](../03_Features/RecordSubmission.md), [Leaderboard](../03_Features/Leaderboard.md), [AccountTransfer](../03_Features/AccountTransfer.md), [Prototype 8 저장/이전 구현](../90_Tasks/Prototype_8/20261003_01_Phase2ManualSteps.md), [receipt/조회 계약](../90_Tasks/Prototype_8/20261006_08_Phase3ManualSteps.md).

## 운영 경계·남은 의무

- Prototype 8은 승인된 예외/한계를 포함해 완료했다. 당시 Candidate01은 공개 운영 후보 판정이며 일반 공개/Steam 승인은 별도다. 이후 Roadmap 9 후보를 같은 공개 승인으로 해석하지 않는다.
- verification/production 환경·Secret·보드·로그를 분리한다. verification 자료를 Production으로 복사하거나 삭제하지 않는다. 최소 구조화 운영 로그는 30일 보관하며 PII/비밀값/payload와 장기 archive를 추가하지 않는다. 운영 담당은 사용자 1인이다.
- 실제 Module 업데이트 전에는 **성공 source 전체·artifact/hash·환경/게시 reference·새 QA 기준 보존**, 호환성·시험/중단/재복구 범위·최종 유지 버전의 별도 승인 후 **연기된 실제 복구 시험**을 수행한다. 복구 배포 후 보존은 아직 미검증이며 정적 검사/로컬 DLL/hash를 원격 artifact 동일성이나 복구 PASS로 바꾸지 않는다.
- Prototype 8의 최초 조회 지연 확인 종료, 별도 Player UI/입력 검증 면제, 복구 시험 연기는 해당 후보/단계의 사용자 승인이다. 첫 Infinite 실패·legacy 검증 N/A, .NET/Jint host/원격 artifact·모든 장애/경합·컨테이너 간 원자성의 미확인 한계는 없어진 것으로 기록하지 않는다.
- 기존 verification 합성 점검 Service Account는 사용자 결정으로 재사용한다. 과거 키 동일성/추가 변경 이력은 미확인이다. 신규 없음/회수 완료로 단정하거나 불명확한 운영 키 삭제·Secret 수집·새 키 발급·불필요한 재조사를 요구하지 않는다. 실제 새 임시 항목이 확인되면 해당 범위에서 처리한다.
- QA 기준/계정/Secret/운영 권한·자료의 삭제, 원격 게시, 구매, 공개는 기존 지시 범위를 확인한다. Candidate02 덮어쓰기 결정은 해당 후보에 적용되며 다른 QA 자료 삭제 승인으로 확대하지 않는다.
- 근거: [Prototype 8 Phase 4 운영 기반](../90_Tasks/Prototype_8/20261007_09_Phase4ManualSteps.md), [Phase 5 최종 판정·예외/복구 의무](../90_Tasks/Prototype_8/20261021_01_Phase5ManualSteps.md), [검증 배포 문서](../../UGS/VERIFICATION_DEPLOYMENT.md).

## 후속 Roadmap의 확정 순서

- 2026-11-18 사용자 확정: 현재 사용 중인 Third Party 애셋은 없으며 Roadmap 10은 향후 도입 애셋의 관리 방안을 마련한다. 기존 UPM/TMP는 기술적 기준선으로 구분한다. Phase 1 Step 6에서 출처·라이선스·복사/수정·게임 사용처·Git/복원 정보를 계속 기록할 형식·위치·갱신 기준을 마련했다. 실제 기록은 ASSET_DEPENDENCIES.md, 형식은 Templates, 규칙은 Implementation Rule을 따른다. 실제 애셋 도입/구매는 이 준비 작업의 착수 조건이 아니다.

- [Roadmap 9~14 안내](../04_Implementation_Roadmap/README.md)의 여섯 문서를 분리 유지하고 번호 순서대로 진행한다.
- 9 Infinite 단일화/Stage 접근 제거 → 10 Thirdparty 원본 비추적·의존성/복원 체계 → 11 플레이어 피드백 → 12 캐릭터 그래픽/동작 → 13 맵 그래픽/가독성 → 14 UI 스타일 통합이다.
- Roadmap 10은 외부 사운드/그래픽/UI 원본 도입 전에 완료한다. 현재 ignore만으로 기존 추적 해제/과거 Git 이력 삭제/복원 성공을 판정하지 않는다.
- Roadmap 11 Phase 1에서 전체 표현 스타일을 합의해 12~14에 인계한다. 표현 변경에 판정/물리/점수 규칙 변경을 포함하지 않는다.
- Pattern 연결·난이도·위험 보상 개선은 9~14 범위 밖이며 후속 번호/시점은 미정이다.

## 이 문서의 갱신 기준

- 현재 진행 위치와 가장 최근의 유효한 기준선만 갱신한다. 같은 내용을 Step/날짜별 제목으로 계속 추가하지 않는다.
- 작업 중 오류·수정 과정·시험별 건수/로그·Scene fileID/적용표·세부 승인 과정은 해당 Task에 기록하고 여기에는 결론과 링크만 남긴다.
- Rule/Feature/System의 전체 설명을 복사하지 않는다. 후속 작업을 바꾸는 결정·예외·남은 의무만 짧게 요약한다.
- 완료/정책 변경 시 이전 대기 상태를 교체한다. 적용 범위와 사용자 보고/정적 확인/실제 실행의 차이를 유지한다.
- 고유 이력은 근거 문서에 보존한 뒤 요약한다. 정리 전 원문 보관본에 새 진행 기록을 누적하지 않는다.

## 상세 근거 찾기

| 대상 | 문서 |
| --- | --- |
| 현재 개발 상태·Phase 완료 조건 | [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md) |
| 최종 후보/검증 한계·다음 인계 | [Prototype 9 Phase 4 Task](../90_Tasks/Prototype_9/20261118_01_Phase4ManualSteps.md) |
| 최신 테스트·Player/Scene·Phase 2 완료 | [Prototype 9 Phase 2 Task](../90_Tasks/Prototype_9/20261112_01_Phase2ManualSteps.md) |
| Stage 처리 제외·구 Client·범위 결정 | [Prototype 9 Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md) |
| 운영 후보·승인 예외·차기 복구 시험 | [Prototype 8 Phase 5 Task](../90_Tasks/Prototype_8/20261021_01_Phase5ManualSteps.md) |
| 프로젝트/아키텍처·문서 책임 | [Project 안내](README.md), [Project Overview](PROJECT_OVERVIEW.md), [Architecture](ARCHITECTURE.md) |
| 정리 이전의 전체 기록 | [2026-11-17 Memory 정리 기록·원문](../90_Tasks/20261117_02_ProjectMemoryCleanup.md) |
