# 애셋·의존성 등록 형식

실제 항목은 [Project 관리 기록](../00_Project/ASSET_DEPENDENCIES.md)에만 작성한다. 이 문서는 빈 형식이며 가상 도입 제품을 넣지 않는다. 절차·상태 전환은 [구현 규칙](../01_Rules/IMPLEMENTATION_RULE.md)의 외부 애셋 관리 기준을 따른다.

## 항목 형식

항목마다 `## <asset_id>` 제목과 아래 필드를 사용한다. `미확인: 이유·필요 근거·다음 확인 시점`, `해당 없음: 이유`를 구분한다. 구성요소의 조건/보존 방향이 다르면 경로별로 나눈다.

| 필드 | 기록할 내용 |
| --- | --- |
| asset_id | 영구 식별자. 이름/버전/경로가 바뀌어도 유지 |
| record_kind | Third Party 애셋 / UPM·기술 의존성 / 프로젝트 소유 연결 자료 |
| product | 제품·제작자/공급자·패키지 ID |
| version | 요청/도입·해결 버전, 고정 revision(해당 시) |
| official_source | 공개 공식 배포 위치와 확인일. 비밀 URL/개인 계정 주소 제외 |
| acquisition | 취득 방식·도입/재취득 담당·계정/네트워크/오프라인 제한과 확인 상태 |
| license | 공급 license 이름/버전·조건 근거 경로/공식 URL·확인일. 구성요소별 조건 구분 |
| usage_conditions | 사용·수정·게임에 포함한 배포·원본 재배포 각각의 조건/제한·확인 상태 |
| notices | copyright/license/Attribution 요구·배포 파일/노출 위치·검증 근거 |
| evidence | 배포물/metadata SHA-256·근거 위치. 영수증/키/유료 원본 본문 제외 |
| original_storage | 비추적 원본 보관 식별·재취득 방법. 개인 절대 경로 대신 관리 위치 설명 |
| import_path | 프로젝트 복사 경로·하위 범위와 공급 version 대응 |
| modifications | 파일별 원본 대비 변경/파생 관계·이유/담당·원인 불명 여부 |
| purpose | 선택 이유·게임에서 전달할 표현/역할 |
| game_usage | Scene/Prefab·객체/컴포넌트/필드·게임 흐름의 실제 사용처. 예정과 구분 |
| references | GUID/fileID·코드/asmdef·Resources/Shader 이름·동적/binary 참조와 한계 |
| meta_import | 자산/폴더 meta·GUID·subasset·Importer/platform 설정 보존/재현 방법 |
| git_boundary | 원본/변경/파생/자체 연결 자료별 유지·비추적 후보·대기와 현 추적/ignore |
| restore | 재취득→도입→변경/설정 재적용→참조 검증의 순서·전제/중단 조건·근거 |
| lifecycle | 업데이트/경로 변경/제거의 영향·원복/보존 방법·승인된 범위 |
| verification | 정적/Unit Test·사용자 Unity 실행·독립 복원을 날짜/입력/실행자/실제 결과로 구분 |
| status | adoption・rights・boundary・restore의 독립 상태·미확인 사유/다음 작업 |
| owner_review | 관리 담당·최종 갱신일·확인/결정자·관련 Task |

### 경로별 경계 표

| 경로/구성요소 | 원본/변경/파생/자체/혼합·근거 | 현재 추적/ignore | 유지/비추적 후보/대기 | 자산/폴더 meta·Import 보존 | 조건/Attribution | 사용처/복원·검증·후속 |
| --- | --- | --- | --- | --- | --- | --- |

### 변경/검증 이력

| 날짜 | 대상 버전/경로 | 변경·결정 | 근거/관련 Task | 검증 종류·입력/실행자/결과 | 남은 의무 |
| --- | --- | --- | --- | --- | --- |

## 자동 검사 형식 계약 — Phase 2 Step 2

실제 항목 본문은 관리 기록의 `## 항목` 아래 `## <asset_id>`로 추가한다. ID는 소문자 영문으로 시작하는 영문/숫자/밑줄/하이픈 3~64자이며 중복하지 않는다. 항목에는 위 필수 23필드 표, `### 경로별 경계 표`, `### 변경/검증 이력`을 순서대로 둔다. 표 안의 실제 개행·HTML br·임의 continuation은 금지하며 셀의 세로줄은 `\|`, 역슬래시는 `\\`로 escape한다. 필드 표의 빈 값·누락/중복 필드와 bare `미확인`/`해당 없음`은 오류다. 사유·필요 근거·다음 시점을 같은 셀에 적는다.

- `evidence`: 확인한 배포물은 `sha256:<64자리 소문자 hex>`와 근거 위치를 적는다. 미확인이라면 사유/다음 시점을 적으며 성공 근거로 사용하지 않는다.
- `import_path`: 경로별 표에 있는 모든 경로를 JSON 문자열 배열로 적고 전체를 백틱으로 감싼다. 모든 경로는 저장소 상대 `Assets/` 아래 정규 경로이며 상위/절대 경로·wildcard·중복/대소문자 충돌·상하위 범위 중복을 허용하지 않는다. 경계가 다른 범위는 겹치지 않는 구성요소 경로로 나눈다.
- `status`: 백틱으로 감싼 한 줄 JSON이다. 최상위 키는 `adoption`, `rights`, `boundary`, `restore`, `decision`만 허용한다. 각 값은 `state`, `evidence` 두 문자열이다. 네 상태 축의 state는 Implementation Rule의 값 그대로 사용한다. `decision.state`는 `pending` 또는 `exclude-candidate`이며 후자는 실제 명시적 사용자 결정 근거가 필요하다. 다른 축을 자동 승인하지 않는다.
- 경로별 표의 첫 셀은 정규 경로, 둘째 셀은 백틱 JSON `{"kind":"역할","evidence":"출처/변경 근거"}`다. kind는 `original`, `modified`, `derived`, `project`, `mixed`, `unknown`, `settings` 중 하나다. 셋째 셀은 현재 추적/ignore 근거이며 넷째 셀은 `preserve`, `pending`, `exclude-candidate`다.
- 경로별 표의 다섯째 셀은 백틱 JSON `{"asset":"preserve 또는 reproduce","folder":"preserve 또는 reproduce","evidence":"자산/폴더 meta·Import 보존/재현 근거"}`다. 여섯째/일곱째 셀에도 조건·사용처/복원 근거 또는 사유가 있는 미확인을 적는다. Assets/Settings 자료의 역할은 settings/project로 제한한다.
- 관리 기록의 목록 행은 `asset_id`, `product / version`, 네 상태 축의 state, `[asset_id](#asset_id)` 순서로 본문과 일치시킨다. 등록부/정책이 서로 누락된 경로는 오류다.

기계 정책은 `Tools/DependencyAudit/dependency-policy.json`의 schemaVersion 1, unregisteredRoots `["Assets/ThirdParty"]`, entries 배열이다. 각 entry는 `asset_id`, `path`, `kind`, `action`, `meta`(asset/folder), `evidenceFields`만 가진다. ID/경로·역할/방향·meta는 해당 본문 경계 행과 일치해야 하며 evidenceFields에는 최소 evidence/meta_import/restore/owner_review를 연결한다. 제품·버전·license/권리 값은 정책에 복제하지 않는다. 현재 실제 제품이 없으므로 entries도 비워 둔다.

확인 상태·권리/적용/복원 근거는 사람이 기록한 주장이다. 검사는 필수 근거와 모순을 확인하며 진위·법적 권한이나 실제 실행 성공을 인증하지 않는다. `--previous`로 이전 등록부를 명시하면 버전/source/license/hash 변경 시 기존 확인/결정을 새 입력에 자동 승계하지 않았는지도 검사한다. 과거 등록부를 제시하지 않은 검사는 현재 snapshot 검사로만 표시한다.

Phase 2 Step 7에서 전환 재검토 대상은 조건/Attribution·복사/수정·목적/import_path/실제 사용처·참조·meta/Import·Git/복원/lifecycle 정보까지 확장했다. 업데이트/이동/제거 때도 같은 ID를 유지하고 이력·재복원/고지 의무를 남긴다. 제거 계획은 기존 항목 삭제로 작성하지 않으며 후보 폐기 상태와 실제 사용처 해소/검증 결과를 구분한다. 늦은 조건/설정/경로 변경은 lifecycle 범위 digest와 소유자 검토를 다시 준비한다. 실제 계획 작성·판정 문법은 Tools/DependencyAudit README, 영구 절차는 Implementation Rule을 따른다.

## 작성 완료 기준

- 필수 필드가 있고 미확인/해당 없음에 이유가 있다. 미확인을 검증 완료로 바꾸지 않는다.
- 원본·복사 위치/변경·게임 내 사용처와 경로별 Git/meta·복원 경계가 연결된다.
- 현재 상태·제안 방향·실제 적용/사용자 결정·복원 성공을 구분한다.
- 원본/비밀값을 새로 포함하지 않고 상세 실행 이력은 Task로 연결한다.
