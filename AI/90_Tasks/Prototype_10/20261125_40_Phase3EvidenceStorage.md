# 작업 정보

## 작업명

Prototype 10 Phase 3 — staged 제외 후보 11개 보존과 참조 정리

## 작업 일자

2026-11-25

## 작업 담당자

AI: 보존·참조/ignore·정적 검증 준비. 사용자: 준비 결과를 검토한 뒤 실제 unstage와 변경 문서 staging.

# 작업 목적

완료 근거를 유지하면서 중복된 검사 원본 출력 5개와 특정 임시 환경용 일회성 스크립트 6개를 Git 밖에 보관할 수 있도록 준비한다. 파일 확장자 전체나 다른 Step 자료를 일괄 제외하지 않는다.

# 작업 대상

[보관 manifest](20261125_39_Phase3EvidenceStorageManifest.json)의 정확한 11개 경로, 해당 파일을 참조하는 Markdown 5개와 루트 `.gitignore`다. 재사용 도구/테스트·복원 spec/입력·완료 판정은 유지한다.

# 작업 전 상태

준비 시작 시 staged 파일은 67개였다. 선정한 11개는 총 4,365,042 byte이며 모두 기존 문서/검증 metadata에서 참조하고 있다. 즉시 누락시키면 근거 탐색이 끊길 수 있다. 이 작업은 실제 unstage 요청 전의 준비이며 index를 쓰지 않는다.

# 조사 내용

다섯 raw JSON은 inventory/gate 전체 출력이고, 여섯 PS1은 고정 root/입력 hash·당시 복원/재개 조건에 묶인 실행 파일이다. 원래 SHA256과 Git blob OID를 함께 기록하고 staged byte와 작업 파일 byte를 비교했다. 이번 11개는 둘이 같았다. 제품 원본/Secret을 새로 수집하거나 외부로 전송하지 않았다.

# 작업 내용

1. 기존 `/Ignore` 규칙 아래에 보관 폴더를 새로 만들고 11개를 정확한 byte로 복사했다. 원래 파일은 삭제/이동/덮어쓰기하지 않았다.
2. 아래 manifest에 원래 경로·보관 경로·길이·SHA256·staged blob OID와 raw 검사 요약을 남겼다. 변경하는 Markdown 5개와 `.gitignore`의 변경 전 byte도 같은 보관 폴더의 `before/`에 저장했다.
3. Markdown의 해당 파일 링크를 이 보관 안내로 연결했다. 과거 절차/명령과 JSON의 원래 경로/hash는 당시 근거로 보존하고, 현재 새 검사 성공으로 다시 작성하지 않았다.
4. `.gitignore`에는 정확한 11개 원래 경로만 추가했다. 이미 staged인 파일에는 아직 적용되지 않으며 unstage한 뒤 적용된다.

<a id="archived-evidence"></a>

## 외부 보관 목록

공통 원래 경로는 `AI/90_Tasks/Prototype_10/`, 공통 보관 경로는 아래 폴더의 `files/`다. **원래 파일명에서 보관 위치와 정확한 해시를 찾을 때는 manifest를 사용한다.**

```text
Ignore/TaskEvidence/Prototype_10/20261125_Phase3StagingExclusion_01/
```

| 원래 파일명 | 분류 |
| --- | --- |
| 20261125_09_Phase3Step7SourceInventory.json | 원래 작업본 inventory |
| 20261125_10_Phase3Step7CandidateInventory.json | 후보 inventory |
| 20261125_24_Phase3Step9CandidateGates.json | 첫 gate 결과·최초 실패 포함 |
| 20261125_25_Phase3Step9IngressVerification.json | evidence 지정 후 ingress 재검사 |
| 20261125_33_Phase3Step9PostRepairGates.json | 공식 파일 복원 후 gate 결과 |
| 20261120_25_Phase3Step6StageA.ps1 | 당시 Stage A 실행 |
| 20261125_01_Phase3Step6ResumeNuGet.ps1 | npm notice 중단 후 재개 |
| 20261125_04_Phase3Step6RestoreLinux.ps1 | Linux target 보완 |
| 20261125_06_Phase3Step6NpmCi.ps1 | 당시 npm ci 실행 |
| 20261125_16_Phase3Step8PrepareChecks.ps1 | 후보 전용 Test 준비 |
| 20261125_30_Phase3Step9RestoreStockFiles.ps1 | 공식 파일 37개 복원 |

검사 요약: Step 7 원래/후보 모두 기존 GUID 미해결 31회다. Step 9 최초 ingress는 evidence 누락으로 두 번 exit 2였고 정확한 evidence 지정 후 두 검사 모두 exit 0이다. 최초 후보 audit의 orphan 37개는 공식 파일 복원 후 0개가 됐다. 사후 registry/managed·staged ingress/lifecycle은 exit 0이며 audit는 기존 GUID 31회를 유지한 exit 1/수집 완료다. 새 Unity/Runner/설치/복원 실행 결과가 아니다.

## 과거 기록과 보관 위치의 해석

기존 JSON의 `path`, `sha256`, `documentationAfter` 등은 **해당 검사 당시** 입력/결과를 가리킨다. 11개 경로는 manifest의 `originalPath`로 찾아 `archivePath`에서 동일 byte를 확인한다. 문서 변경 전 hash는 manifest의 `documentationBefore.backupPath`에서 확인한다. 원래 완료 기록을 새 문서 상태의 검증 결과로 해석하지 않는다. 이 정리는 원래 Phase 3 판정/31회 진단·미검증 한계를 변경하지 않는다.

**이 폴더는 로컬 비추적 보관본이다.** Git clone에는 포함되지 않는다. 원본 출력/과거 스크립트까지 필요하면 소유자가 이 폴더를 별도로 보존/전달해야 한다. manifest만으로 원본 byte를 재생성할 수는 없다. 현재 11개 원래 파일도 로컬에 남기므로 로컬 과거 경로는 계속 열 수 있다. 보관 스크립트를 이 안내에서 재실행하라는 의미는 아니다.

## 준비 후 사용자가 실행할 명령

프로젝트 루트 `C:\Unity\Unity_Flow_State`에서 실행한다. 첫 블록은 manifest의 11개만 staged에서 제거하며 작업 파일은 유지한다. AI는 이 명령을 실행하지 않았다.

```powershell
$phase3Evidence = Get-Content -LiteralPath '.\AI\90_Tasks\Prototype_10\20261125_39_Phase3EvidenceStorageManifest.json' -Raw -Encoding UTF8 | ConvertFrom-Json
$phase3Paths = @($phase3Evidence.files | ForEach-Object { $_.originalPath })
git restore --staged -- $phase3Paths
if ($LASTEXITCODE -ne 0) { throw 'Unstage failed; stop.' }
```

준비한 ignore/참조 변경과 새 보관 안내를 이번 commit에 포함하려면 이어서 아래 파일만 stage한다. 이전에 staged했던 문서도 이번 참조 변경분을 다시 stage해야 한다.

```powershell
$phase3Docs = @($phase3Evidence.documentationBefore | ForEach-Object { $_.path }) + @(
    'AI/90_Tasks/Prototype_10/20261125_39_Phase3EvidenceStorageManifest.json',
    'AI/90_Tasks/Prototype_10/20261125_40_Phase3EvidenceStorage.md'
)
git add -- $phase3Docs
if ($LASTEXITCODE -ne 0) { throw 'Stage documentation failed; stop.' }
git status --short
```

원복이 필요하면 manifest의 보관본과 현재 파일을 먼저 비교한다. `before/`는 이번 문서 변경 전의 검증된 byte이며 늦은 사용자 변경을 덮어쓰는 자동 원복은 제공하지 않는다. 전체 reset/clean·파일 삭제는 필요 없다.

# 영향 범위

허용 쓰기는 보관 폴더·새 manifest/안내·해당 Markdown 5개와 `.gitignore`다. Git index, 선택한 원래 파일 11개, 기존 JSON evidence·도구/생산 코드/자산·Scene/Prefab·후보 프로젝트·설치/cache는 수정하지 않는다.

# 검증 내용

11개 staged/원래 파일과 보관본 byte/hash, 보관 경로 및 11개 경로의 ignore 규칙, Markdown 링크·manifest mapping, 기존 JSON 불변, Git index 불변과 공백 오류를 정적으로 검사한다. 문서/보관 변경이므로 Unity·빌드·Test Runner를 실행하지 않는다.

# 검증 결과

실제 정적 검증 수치와 문서 변경 후 hash는 manifest의 `verification` 및 `documentationAfter`를 따른다. 준비 완료와 실제 unstage/stage 완료를 구분한다. source registry 형식도 확인하되 과거 복원/참조 검사를 새로 통과했다고 기록하지 않는다.

# 후속 작업

사용자가 위 명령으로 11개를 unstage하고 ignore/참조 변경·manifest/안내를 stage하면 준비 결과가 index에 반영된다. 현재 AI는 index를 변경하지 않았다. 로컬 보관본은 필요한 동안 별도로 보존한다.

# 관련 문서

- [Phase 3 원래 Task](20261120_04_Phase3ManualSteps.md)
- [최종 복원/도입 인계](20261125_37_Phase3RestoreAndAdoptionHandoff.md)
- [AI 진입점](../../README.md)

# 관련 작업 기록

- [당시 Step 10 최종 검증](20261125_38_Phase3Step10FinalVerification.json)
- [이번 보관 manifest/정적 검증](20261125_39_Phase3EvidenceStorageManifest.json)

# 작성 완료 기준

원본 byte와 실패/성공 이력·완료 근거를 보존하고, 제외할 11개와 문서·ignore의 실제 상태·후속 명령이 일치해야 한다. 실제 unstage를 수행한 것처럼 표시하지 않는다.
