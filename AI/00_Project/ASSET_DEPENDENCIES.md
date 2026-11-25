# 애셋·의존성 관리 기록

**처음 작업하는 경우:** [Third Party 파일 관리 가이드](../../THIRD_PARTY_GUIDE.md)에서 파일 구분과 실제 작업 순서를 먼저 확인한다. 이 문서는 가이드를 따라 결정한 제품별 정보를 작성하는 단일 기록이다.

최종 갱신: 2026-11-25

Roadmap 10 Phase 1~3은 완료(2026-11-25)했다. 관리 경계·등록/Git·도입/보존·업데이트/제거/원복 절차에 이어 현재 기술 기준선의 별도 후보 UPM/TMP·실제 npm lock/ci·NuGet 기본/Linux restore와 필수 참조/사용자 Unity 결과·원래 자료 보존을 확인했다. [Phase 3 Task Step 10](../90_Tasks/Prototype_10/20261120_04_Phase3ManualSteps.md)·[최종 인계](../90_Tasks/Prototype_10/20261125_37_Phase3RestoreAndAdoptionHandoff.md)·[최종 검증](../90_Tasks/Prototype_10/20261125_38_Phase3Step10FinalVerification.json)을 따른다. 후보 snapshot/이식된 선언·lock과 원래 입력을 구분하며 Git만으로 private 보존본까지 복원되지 않는다. raw audit exit 1/기존 GUID 31회는 현재 필수 사용 근거와 구분해 보존한다. 모든 자산의 참조 건강/Player/variant·제품별 권리·신규 구매/배포를 승인하지 않는다. 실제 lifecycle/원본 추적 해제 요청은 0개다.

## 책임과 현재 상태

제품/버전·출처·조건·복사/수정·게임 내 사용처·Git/meta/복원 정보를 계속 관리하는 단일 기록이다. [등록 형식](../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)과 [구현 규칙](../01_Rules/IMPLEMENTATION_RULE.md)을 사용하고, 조사/결정 이력은 [Roadmap 10 Phase 1 Task](../90_Tasks/Prototype_10/20261118_02_Phase1ManualSteps.md)에 연결한다. Project Memory나 별도 표에 같은 제품 기록을 중복 누적하지 않는다.

사용자 확정 전제상 현재 도입한 Third Party 애셋은 **0개**이며 실제 등록 항목은 없다. 후보를 정했을 때 템플릿을 복사해 등록한다. 이 문서 작성은 구매/도입이나 사용 조건 검증 완료를 의미하지 않는다.

기존 UPM/TMP·서버 보조 의존성은 신규 도입 애셋과 구분한 기술적 기준선이다. 현재 보존/미확인 경계는 위 Task Step 6, 세부 버전/파일·조건·참조 근거는 Step 2~5 metadata를 따른다. package별 선언을 여기에 수동 복제해 별도 버전 정답을 만들지 않는다. 향후 별도 도입/수정하는 기술 의존성의 결정도 필요하면 `record_kind`를 구분해 등록하고 manifest/lock으로 연결한다.

## 등록 목록

기존 URP/TMP의 31회 GUID 진단은 Phase 3에서 실제 import/필수 참조·사용자 표시/실행 결과와 함께 평가해 현재 기술 기준선 복원 비차단으로 구분했다. orphan meta는 공식 파일 37개 복원 후 0개다. raw 참조 건강 PASS는 아니며 해당 legacy volume/probe debug/TMP graph를 필수로 사용하는 후속 변경은 새로 검증한다. 상세 범위/근거와 획득·수동 단계는 Phase 3 최종 인계를 따른다.

현재 기술적 기준선은 보존 처리한다. TMP 변경본/연결 설정·조건 미확인 자료의 사용/제외·재배포를 새로 승인하지 않으며 실제 제외 후보/변경 요청은 없다. 현재 사용자 추가 결정이 불필요한 근거와 재확인 시점은 Phase 1 Task Step 8 및 그 정책 metadata에 남겼다. 제품별 권리의 미확인은 그대로 유지한다. 현재 기술 기준선의 별도 복원 결과는 위 Phase 3 최종 인계에 연결하며 미래 제품/버전의 재현 검증을 대신하지 않는다.

반복 정적 검사는 [DependencyAudit 안내](../../Tools/DependencyAudit/README.md)를 따른다. 도구의 finding/종료 코드와 실제 도입·조건·복원 상태는 구분한다.

항목/경로 표의 기계 검증 문법은 등록 형식의 자동 검사 형식 계약을 따른다. `node Tools/DependencyAudit/dependency-registry.cjs --check`는 이 기록과 최소 경계 정책의 연결을 읽기 전용으로 검사한다. 빈 등록부의 통과는 신규 제품 도입이나 Git 원본 유입 차단/독립 복원 성공을 의미하지 않는다.

| asset_id | 종류·제품/버전 | 도입 상태 | 조건 상태 | 경계 상태 | 복원 상태 | 항목 링크 |
| --- | --- | --- | --- | --- | --- | --- |

등록 항목 없음. 실제 제품이 없으므로 가상 예제나 채워진 license를 등록하지 않는다.

## 항목

실제 후보/도입 항목이 생기면 영구 ID별 본문을 추가하고 목록에 연결한다. 업데이트/제거 후에도 ID·결정/검증 근거를 보존하며 기록 삭제로 참조/조건 의무를 잃지 않는다.
