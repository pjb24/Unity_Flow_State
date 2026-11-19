# Dependency audit

외부 package 없이 Node 내장 모듈로 실행하는 Phase 1 읽기 전용 metadata 검사다. Unity/컴파일러/Test Runner·Player·서비스/원격·설치/복원을 실행하지 않는다. Git은 고정된 read 명령만 허용하고 optional lock·fsmonitor·external diff/textconv 실행을 차단한다. 검사 자체는 파일을 쓰지 않으며 JSON을 stdout으로 반환한다.

```powershell
node Tools/DependencyAudit/dependency-audit.test.cjs
node Tools/DependencyAudit/dependency-audit.test.cjs --policy AI/90_Tasks/Prototype_10/20261119_03_Phase1Step8Policy.json
node Tools/DependencyAudit/dependency-audit.cjs --check
node Tools/DependencyAudit/dependency-audit.cjs --check --baseline AI/90_Tasks/Prototype_10/20261118_02_Phase1Step1Baseline.json
```

`--root`는 조사할 Git 저장소 루트를 명시할 때만 쓴다. `--baseline`은 그 루트 안의 상대 경로이며 생략하면 과거 Task의 기준선을 자동으로 정답으로 쓰지 않는다. 실제 index는 매 실행 전후 비교한다. 결과를 보존할 경우 호출자가 지정한 Task metadata 경로에 UTF-8로 저장한다. Windows PowerShell의 기본 `>` 인코딩을 그대로 사용하지 않는다.

| 종료 코드 | 의미 |
| --- | --- |
| 0 | 구현된 정적 검사 범위에 진단 없음. 권리/분류 확정·Unity 해석·독립 복원 성공은 아님 |
| 1 | 수집은 완료했으나 unresolved GUID·orphan 등 finding 존재 |
| 2 | 접근/열거/JSON·Git/형식 오류·불완전 metadata·기준선 변경 등으로 완전한 결과라고 할 수 없음 |

Assets/Packages/ProjectSettings/Tools/UGS와 루트 ignore/npm 선언을 조사한다. cache는 package.json의 ID/버전으로 찾아 패키지 meta와 현재 자산에서 연결되는 직렬화 자료만 읽는다. 일반 Library/Build·bin/obj/node_modules·사용자 설정 등은 제외한다. symlink/junction은 따라가지 않고 오류로 남긴다. Secret/로컬 production 설정은 본문을 읽지 않으며 meta는 GUID/Importer 참조 metadata로만 사용한다.

현재 구현은 Git/ignore·패키지 graph/설치 metadata·GUID/meta·연결된 YAML/ShaderGraph/Importer·프로젝트 소스의 이름/동적/binary 한계·등록 형식·보수적 제외 판정을 검사한다. package의 임의 동적 로딩·Unity 타입/subasset/렌더링이나 권리 판단을 대신하지 않는다. `ownership`은 출처/권리/Import 재현 근거를 자동 추정하지 않아 실제 목록을 `preserve-pending`으로 둔다. 별도의 분류 함수도 필수 근거/사용자 결정이 없으면 제외 후보를 확정하지 않는다. 실제 기록과 판단은 [관리 기록](../../AI/00_Project/ASSET_DEPENDENCIES.md) 및 [Phase 1 Task](../../AI/90_Tasks/Prototype_10/20261118_02_Phase1ManualSteps.md)로 연결한다.

기본 Unit Test는 합성 metadata·주입 fs/Git 실패·임시 저장소만 사용한다. 임시 저장소의 초기화/staging/commit·파일 생성/정리는 fixture 준비이며 실제 작업본 index/설정에는 적용하지 않는다. 이름/개수는 고정된 과거 프로젝트 수치의 복제가 아니라 검사 책임의 정상/실패 입력으로 검증한다.

선택적인 Test `--policy`는 저장소 내 보존 정책 metadata를 실제 분류 함수에 재생한다. 승인/제외·권리 부여나 실제 애셋/저장소 변경을 하지 않으며, 보존 판정과 조건 미확인·후속 의무가 함께 유지되는지 확인한다. 기본 suite와 이 정책 검증 결과를 Unity Runner/독립 복원 결과로 해석하지 않는다.
