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

## 작성 완료 기준

- 필수 필드가 있고 미확인/해당 없음에 이유가 있다. 미확인을 검증 완료로 바꾸지 않는다.
- 원본·복사 위치/변경·게임 내 사용처와 경로별 Git/meta·복원 경계가 연결된다.
- 현재 상태·제안 방향·실제 적용/사용자 결정·복원 성공을 구분한다.
- 원본/비밀값을 새로 포함하지 않고 상세 실행 이력은 Task로 연결한다.
