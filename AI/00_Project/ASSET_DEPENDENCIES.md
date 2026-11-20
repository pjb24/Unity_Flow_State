# 애셋·의존성 관리 기록

최종 갱신: 2026-11-20

Roadmap 10 Phase 1과 Phase 2 관리 경계·등록/검사·도입/보존/재도입/lifecycle 절차 준비는 완료했다(Phase 2: 2026-11-20). 최종 판정/새 기준선·실제 취득/복원 전 의무는 [Phase 2 Task Step 10](../90_Tasks/Prototype_10/20261119_05_Phase2ManualSteps.md)과 [Phase 3 인계](../90_Tasks/Prototype_10/20261120_02_Phase3Handoff.md)를 따른다. Phase 2 Step 1~3에 이어 Step 4에서 TMP 변경본/설정·meta와 공급 archive의 로컬 비추적 보존/임시 byte 재적용을 검증했다. 보존본·고정 해시/명령·제한은 Phase 2 Task Step 4에 연결한다. Step 5의 별도 ingress CLI로 현재 HEAD/index·staged 유입과 알려진 지문/경계를 검사했다. Step 6은 provider/버전·누락/graph 검사와 비추적 tgz 준비·이식 경로 제안을 마련했다. Step 7은 업데이트/이동·제거·실패 원복과 같은 ID의 이력/의무 보존 절차·읽기 전용 판정을 마련했다. Step 8은 전체 Node Test 261개와 실제 읽기 전용 검사·기존 자료 보존을 확인했다. 기존 참조 진단은 유지하며 현재 도구 통과를 실제 복원 성공으로 해석하지 않는다. 실제 lifecycle 작업은 0개이며 도입/추적 해제·선언/lock 적용·설치는 미실행이다. Step 9에서 현재 생산 입력 불변·실제 요청 0개에 따라 사용자 Unity/설치/lock 생성이 불필요한 근거와 조건부 후속 실행을 연결했다. 별도 환경의 실제 경로 이식/lock 생성·새 restore와 독립 복원은 미실행이며 Phase 3에 남긴다. 보존/미확인 조건은 Phase 1 Task Step 10을 유지한다.

## 책임과 현재 상태

제품/버전·출처·조건·복사/수정·게임 내 사용처·Git/meta/복원 정보를 계속 관리하는 단일 기록이다. [등록 형식](../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)과 [구현 규칙](../01_Rules/IMPLEMENTATION_RULE.md)을 사용하고, 조사/결정 이력은 [Roadmap 10 Phase 1 Task](../90_Tasks/Prototype_10/20261118_02_Phase1ManualSteps.md)에 연결한다. Project Memory나 별도 표에 같은 제품 기록을 중복 누적하지 않는다.

사용자 확정 전제상 현재 도입한 Third Party 애셋은 **0개**이며 실제 등록 항목은 없다. 후보를 정했을 때 템플릿을 복사해 등록한다. 이 문서 작성은 구매/도입이나 사용 조건 검증 완료를 의미하지 않는다.

기존 UPM/TMP·서버 보조 의존성은 신규 도입 애셋과 구분한 기술적 기준선이다. 현재 보존/미확인 경계는 위 Task Step 6, 세부 버전/파일·조건·참조 근거는 Step 2~5 metadata를 따른다. package별 선언을 여기에 수동 복제해 별도 버전 정답을 만들지 않는다. 향후 별도 도입/수정하는 기술 의존성의 결정도 필요하면 `record_kind`를 구분해 등록하고 manifest/lock으로 연결한다.

## 등록 목록

Phase 1 Task Step 9에서 기존 URP/TMP graph의 미해결 참조·sample orphan을 현재 자료 보존 범위의 비차단 한계로 분류했다. 실제 로딩/렌더링 정상 판정은 아니며 관련 자료의 실제 변경/도입·독립 복원 전에는 영향과 필요한 Unity 검증을 다시 확인한다. 현재 범위의 판정을 후속 검증 면제로 승계하지 않는다.

현재 기술적 기준선은 보존 처리한다. TMP 변경본/연결 설정·조건 미확인 자료의 사용/제외·재배포를 새로 승인하지 않으며 실제 제외 후보/변경 요청은 없다. 현재 사용자 추가 결정이 불필요한 근거와 재확인 시점은 Phase 1 Task Step 8 및 그 정책 metadata에 남겼다. 제품별 권리·재현 검증의 미확인은 그대로 유지한다.

반복 정적 검사는 [DependencyAudit 안내](../../Tools/DependencyAudit/README.md)를 따른다. 도구의 finding/종료 코드와 실제 도입·조건·복원 상태는 구분한다.

항목/경로 표의 기계 검증 문법은 등록 형식의 자동 검사 형식 계약을 따른다. `node Tools/DependencyAudit/dependency-registry.cjs --check`는 이 기록과 최소 경계 정책의 연결을 읽기 전용으로 검사한다. 빈 등록부의 통과는 신규 제품 도입이나 Git 원본 유입 차단/독립 복원 성공을 의미하지 않는다.

| asset_id | 종류·제품/버전 | 도입 상태 | 조건 상태 | 경계 상태 | 복원 상태 | 항목 링크 |
| --- | --- | --- | --- | --- | --- | --- |

등록 항목 없음. 실제 제품이 없으므로 가상 예제나 채워진 license를 등록하지 않는다.

## 항목

실제 후보/도입 항목이 생기면 영구 ID별 본문을 추가하고 목록에 연결한다. 업데이트/제거 후에도 ID·결정/검증 근거를 보존하며 기록 삭제로 참조/조건 의무를 잃지 않는다.
