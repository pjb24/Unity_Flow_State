# Implementation Roadmap Documents

이 문서는 Implementation Roadmap 영역의 진입 문서이다.

Implementation Roadmap 영역에 존재하는 문서의 역할과 책임을 정의한다.

---

# 목적

## Roadmap 9~14 확정 계획

2026-11-11 사용자 요청으로 아래 여섯 Roadmap을 분리 유지하고 번호 순서대로 진행한다.
Roadmap 9는 **Phase 1~4 전체 완료(2026-11-18)**다. Compile/1031·316 사용자 성공·16스크립트 exit 0과 최신 Windows-production Candidate02의 QA 후보/한계·복구 의무를 유지한다. Roadmap 10도 **Phase 1 완료(2026-11-19)**이며 [Phase 1 Task Step 10](../90_Tasks/Prototype_10/20261118_02_Phase1ManualSteps.md)에 관리 경계/등록 체계와 검사 근거·제약을 연결했다. Roadmap 10 [Phase 2 Task](../90_Tasks/Prototype_10/20261119_05_Phase2ManualSteps.md) 절차 문서를 작성했으며 다음은 Step 1 기준선 확정이다. 실제 도입/추적 제외·독립 복원과 Roadmap 10 전체 완료는 아니며 11~14도 미착수다.

| 순서 | Roadmap | 목표 | 상태 |
|---|---|---|---|
| 9 | [InfiniteMode 단일화와 Stage Mode 접근 제거](IMPLEMENTATION_ROADMAP_009.md) | Stage 접근 차단·Deprecated 소스 보존과 Infinite 계약 유지 | Phase 1~4 완료·QA 후보/한계 기록 |
| 10 | [Thirdparty 애셋 관리와 프로젝트 복원 체계](IMPLEMENTATION_ROADMAP_010.md) | 외부 원본 비추적·의존성/참조 복원 | Phase 1 완료·Phase 2 문서 작성/Step 1 대기 |
| 11 | [플레이어 피드백 강화](IMPLEMENTATION_ROADMAP_011.md) | 착지/획득/실패의 화면·사운드 결과 전달 | 계획 확정·대기 |
| 12 | [캐릭터 그래픽과 동작 표현 강화](IMPLEMENTATION_ROADMAP_012.md) | 외형/동작의 가독성과 기존 판정 보존 | 계획 확정·대기 |
| 13 | [맵 그래픽과 공간 가독성 강화](IMPLEMENTATION_ROADMAP_013.md) | 기존 Pattern 규칙을 유지한 공간 표현 개선 | 계획 확정·대기 |
| 14 | [UI 스타일 통합과 가독성 강화](IMPLEMENTATION_ROADMAP_014.md) | 완성된 플레이 화면과 UI 스타일의 일관성 | 계획 확정·대기 |

### 공통 의존성과 경계

- [Roadmap 9 Phase 3 Task](../90_Tasks/Prototype_9/20261117_01_Phase3ManualSteps.md)의 기록/Editor/UI 경계와 Compile/1031·316 근거를 [Phase 4 Task](../90_Tasks/Prototype_9/20261118_01_Phase4ManualSteps.md) Step 1~10의 최신 후보/검증·사용자 보고에 연결해 Roadmap 9를 완료했다. 실제 원격/복구·로그 원문 한계는 해당 Task 10-3을 유지하며 Roadmap 10 Phase 1도 완료했으며 다음은 Phase 2 Task Step 1 기준선 확정이며 실제 실행은 미착수다.

- 2026-11-17 Pause Retry 확인창 추가 후 사용자 EditMode 919개/PlayMode 288개 전체 통과와 Windows-production Candidate02의 정상 플레이/UI 확인 보고를 받아 Phase 2 Task 9-3/Step 10을 완료했다. 사용자가 기존 Candidate02 보존 불필요를 명시해 같은 Build/ProductionQA/Candidate02에 덮어쓴 최신 빌드를 최종 후보로 기록한다. 이전 916/287 결과나 덮어쓰기 전 후보를 최신 검증으로 대신하지 않는다.

- 2026-11-17 사용자 요청으로 Leaderboard Stage 탭/사용자 조회 경계를 Phase 2 Step 7에 선행 반영했다. Stage 선택을 거절하고 Main Menu/Result 진입·Top/Around 조회를 Infinite로 고정했다. 당시 Compile/Scene 적용·919/288 성공 근거와 후속 Phase 3 전체 완료 근거는 구분한다.

- Roadmap 8은 승인 예외/잔여 한계를 포함해 완료했다. Roadmap 9도 자체 최신 근거로 QA 후보 판정을 완료했으며 선행 후보의 예외/승인을 자동 승계한 결과가 아니다. 원격/복구 미검증 한계와 차기 실제 Module 복구 의무·별도 공개 승인 경계를 유지한다.
- Step 4에서 구 Client 미지원·Infinite 공통 계약 유지를 확정했다. 현재 전환 범위에 서버 계약 변경은 불필요하며 구 Client 강제 접근 차단이나 원격 변경은 수행하지 않았다.
- Step 3 사용자 전제로 기존 Stage 기록과 추가 제출은 없는 것으로 처리하며 별도 Stage 보관/이관·Pending 호환 개발은 제외한다. 자료 삭제·보드 초기화는 수행하지 않는다.
- Roadmap 9의 Stage 소스는 삭제하지 않고 Deprecated와 사유 주석으로 보존한다. 사용자 접근/명시적 실행 차단은 별도로 구현하고 Infinite 공통 기반·내부 자료 호환과 구분한다.
- Stage Mode 접근 제거는 기존 자료/보드 삭제 승인이 아니다. 내부 읽기 호환과 공통 실행 기반은 사용자 기능 제거와 구분한다.
- Roadmap 10은 사운드/그래픽/UI 외부 원본 도입 전에 완료한다. 현행 ignore 규칙만으로 추적 해제·이력 삭제·복원 성공을 선언하지 않는다.
- Roadmap 11 Phase 1에서 캐릭터/맵/UI의 공통 색상·명암·분위기·위험 표시 방향을 확정하고 12~14에 인계한다.
  이 선행 합의는 별도 일곱 번째 Roadmap으로 분리하지 않는다. 특정 애셋/스타일의 세부 선택은 착수 시 결정한다.
- 피드백은 결과 전달, 캐릭터는 외형/동작, 맵은 공간 표현, UI는 정보 위계/일관성을 중심으로 구분한다.
  표현 변경에 물리·착지·점수/경쟁 조건 변경을 섞지 않는다.
- Pattern 연결·난이도 변화·위험과 보상 선택 개선은 별도 후속 작업으로 보류한다. Roadmap 13에 포함하지 않으며 후속 번호/시점은 아직 정하지 않는다.

### 공통 검증·승인 경계

- 실제 동작 규칙/구현 방법은 착수할 Phase에서 Feature/System 문서·Task에 확정한다. 현재 Feature 문서를 미래 구현이 완료된 것처럼 변경하지 않는다.
- AI는 정적 검사/로컬 대역과 검증 준비를 수행한다. Unity 컴파일/Test Runner/Player·Module 빌드/원격 게시와 Scene 적용은 사용자 수행으로 남긴다.
- 정적으로 판정할 수 있는 항목을 수동 확인으로 전가하지 않으며 실제 시각·청각 인지성은 사용자 확인으로 구분한다.
- Roadmap 8의 특정 후보 검증 면제를 향후 변경 전체의 검증 면제로 승계하지 않는다.
- 실제 Module 업데이트 전에는 Roadmap 8에서 연기한 복구 시험의 source/artifact/reference·QA 기준 보존과 호환성/중단·재복구 범위 별도 승인을 적용한다.
- 원격 자료/계정 삭제·Git 이력 재작성·애셋 구매·Steam/일반 공개는 계획 확정만으로 승인되지 않는다.

## 영역 관리 목적

Implementation Roadmap 영역은 구현 순서를 관리한다.

Implementation Roadmap 영역은 개발 단계를 관리한다.

Implementation Roadmap 영역은 구현 우선순위를 관리한다.

Implementation Roadmap 영역은 현재 개발 진행 상태를 관리한다.

Implementation Roadmap 영역은 다음 작업 대상을 관리한다.

Implementation Roadmap 영역은 1차 프로토타입부터 최종 구현까지의 개발 계획을 관리한다.

Implementation Roadmap 영역은 프로젝트 정보를 관리하지 않는다.

Implementation Roadmap 영역은 프로젝트 규칙을 관리하지 않는다.

Implementation Roadmap 영역은 System의 책임을 관리하지 않는다.

Implementation Roadmap 영역은 Feature의 규칙을 관리하지 않는다.

Implementation Roadmap 영역은 작업 기록을 관리하지 않는다.

---

# Implementation Roadmap 정의

Implementation Roadmap은 프로젝트의 개발 계획을 관리하는 문서이다.

Implementation Roadmap 문서는 어떤 순서로 개발해야 하는지를 설명한다.

Implementation Roadmap 문서는 현재 개발 진행 상태를 설명한다.

Implementation Roadmap 문서는 다음에 구현해야 하는 작업을 설명한다.

Implementation Roadmap 문서는 구현 우선순위를 설명한다.

Implementation Roadmap 문서는 개발 단계를 설명한다.

Implementation Roadmap 문서는 구현 방법을 설명하지 않는다.

Implementation Roadmap 문서는 System의 책임을 설명하지 않는다.

Implementation Roadmap 문서는 Feature의 규칙을 설명하지 않는다.

Implementation Roadmap 문서는 작업 기록을 저장하지 않는다.

---

# 문서 생성 기준

아래 조건 중 하나 이상을 만족하는 경우 새로운 Roadmap 문서를 생성할 수 있다.

```text
프로토타입 구현 순서를 정의해야 한다.

개발 단계를 구분하여 관리해야 한다.

구현 우선순위를 변경해야 한다.

대규모 기능 추가에 따른 개발 계획이 필요하다.
```

Roadmap 문서는 프로젝트 전체에서 필요한 최소 개수만 생성한다.

동일한 개발 계획을 여러 Roadmap 문서로 분리하지 않는다.

기존 Roadmap으로 관리 가능한 경우 새로운 Roadmap 문서를 생성하지 않는다.

---

# 문서 작성 형식

새로운 Roadmap 문서를 생성하는 경우 아래 템플릿을 사용한다.

```text
99_Templates/IMPLEMENTATION_ROADMAP_TEMPLATE.md
```

Roadmap 문서는 템플릿 형식을 따라 작성한다.

Roadmap 문서 구조는 Templates 영역에서 관리한다.

---

# 관리 대상

Implementation Roadmap에서는 아래 내용을 관리한다.

```text
개발 단계

현재 개발 진행 상태

다음 작업 대상

구현 우선순위

구현 순서

진행 중인 작업

완료된 작업

보류된 작업

각 단계의 목표

단계 완료 기준
```

---

Implementation Roadmap에서는 아래 내용을 관리하지 않는다.

```text
프로젝트 정보

프로젝트 규칙

System 책임

Feature 규칙

구현 방법

작업 기록

버그 수정 내역
```

---

# 상태 변경 기준

다음 작업을 시작하면 해당 작업을 진행 중인 작업으로 변경한다.

작업이 완료되면 진행 중인 작업에서 완료된 작업으로 변경한다.

작업이 중단되면 해당 작업을 보류된 작업으로 변경한다.

보류된 작업을 다시 시작하면 진행 중인 작업으로 변경한다.

구현 우선순위가 변경되면 Roadmap 문서를 먼저 수정한다.

작업 기록은 Roadmap에 작성하지 않는다.

작업 완료 후 Tasks 영역에 작업 기록을 작성한다.

---

# 문서 작성 원칙

Roadmap 하나당 하나의 개발 계획만 관리한다.

Roadmap은 구현 계획과 개발 진행 상태만 관리한다.

Roadmap은 구현 방법을 관리하지 않는다.

System 책임은 Systems 영역에서 관리한다.

Feature 규칙은 Features 영역에서 관리한다.

프로젝트 정보는 Project 영역에서 관리한다.

프로젝트 규칙은 Rules 영역에서 관리한다.

작업 기록은 Tasks 영역에서 관리한다.

문서 작성 형식은 Templates 영역에서 관리한다.

동일한 내용을 여러 문서에 중복 작성하지 않는다.

각 문서는 자신의 책임 범위만 관리한다.

---

# 작업 기록

Roadmap 관련 작업 기록은 Roadmap 영역에 저장하지 않는다.

Roadmap 관련 작업 기록은 아래 위치에 저장한다.

```text
90_Tasks
```

작업 기록은 적절한 Task Template을 사용하여 작성한다.

---

# 관련 문서

Project 영역은 프로젝트 정보를 관리한다.

Rules 영역은 프로젝트 규칙을 관리한다.

Systems 영역은 시스템 책임을 관리한다.

Features 영역은 기능 규칙을 관리한다.

Implementation Roadmap 영역은 개발 계획과 개발 진행 상태를 관리한다.

Tasks 영역은 실제 작업 기록을 관리한다.

Templates 영역은 문서 작성 형식을 관리한다.

동일한 내용을 여러 영역에 중복 저장하지 않는다.
