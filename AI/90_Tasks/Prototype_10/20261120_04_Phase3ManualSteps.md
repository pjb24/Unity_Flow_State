# 작업 정보

> 2026-11-25 보관 정리: 이 문서가 참조하는 선정 파일 11개는 [보관 목록·해시·검사 요약](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 통해 찾는다. 기존 JSON 경로/hash와 아래 실행 명령은 당시 기록이며 스크립트를 재실행하지 않는다. 원래 완료 판정의 범위/한계는 유지한다.

## 작업명

Prototype 10 Phase 3 — 별도 환경의 독립 복원 검증과 후속 애셋 도입 인계

## 작업 일자

20261120 — 실행 계획 작성일. 실제 Step 수행일/환경/입력은 각 결과에 기록한다.

## 작업 담당자

- AI: 기준선·경로/권리 근거/필수 입력 대조, 정적 검사·Node Unit Test/복원 보조 도구 준비, 안전 전달/사용자 적용표·최소 Unity Test 준비, 실제 결과 판정/기록.
- 사용자: 정적으로 확인할 수 없는 실제 취득/계정·조건 정보, 별도 환경의 필요한 권한/사용자 데이터 분리, 실제 Unity Editor/Compile/Build/Test Runner·Scene/Prefab 적용과 필요한 설치/lock 생성 실행.

## 작업 상태

**Step 1~10·Phase 3·Roadmap 10 완료(2026-11-25).** 현재 기술 기준선의 필수 입력/참조 복원을 실제 후보·설치/Unity 결과와 보존 근거로 판정했다. [Step 10 평가](20261125_36_Phase3Step10Assessment.json)·[최종 인계](20261125_37_Phase3RestoreAndAdoptionHandoff.md)·[최종 검증](20261125_38_Phase3Step10FinalVerification.json)을 따른다. raw audit exit 1/기존 GUID 31회는 유지하며 전체 자산 참조 건강 PASS나 신규 권리/배포·Player/all-variant 승인은 아니다. 추가 수동 작업은 없다. AI는 Unity/빌드/Runner·설치/restore·Scene/Prefab 작업을 수행하지 않았다.

# 작업 목적

원래 작업본·변경본/보존본·QA/저장/Secret을 유지하면서, 기존 Library/node_modules/obj 전체를 복제하지 않는 별도 환경에서 같은 의존성과 프로젝트 자료를 준비하고 필수 참조/Import 설정을 검증한다. 정적 검사와 Unit Test로 해결 가능한 작업은 AI가 수행하고, Unity 해석/실제 취득/설치처럼 필요한 실행만 사용자에게 연결한다.

현재 신규 Third Party 제품은 0개다. 가상 제품/권리 확인을 만들거나 복원 검증을 위해 제품을 구매하지 않는다. 기존 UPM/TMP·서버 보조 의존성을 기술 기준선으로 검증하고, 이후 Roadmap 11~14 실제 애셋 도입에 재사용할 절차/제약을 인계한다.

# 작업 대상

- 원래 작업본: `C:\Unity\Unity_Flow_State` — 생산 자산/설정·선언/lock·QA/저장/Secret·HEAD/index 보존.
- 별도 환경: `C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01\Project`에 snapshot을 생성했다. 같은 상위 폴더의 Logs/Outputs와 Git 격리용 빈 설정/템플릿/훅 경로를 준비했다. UserData/Player 출력은 예약만 했으며 현재 게임 실행 범위는 Editor 메모리 저장·온라인 비활성이다. UserData 폴더명만으로 기존 저장이 분리되는 것은 아니다.
- Phase 2 도구/정책·TMP schema 2 보존본·고정 archive·최신 문서/미커밋 파일과 실제 복원 후보.
- Task metadata/필요한 보조 도구·Unit Test, 조건부 사용자 Unity Test/Scene 적용표와 최종 판정.

# 작업 전 상태

- [Phase 2 Task](20261119_05_Phase2ManualSteps.md) Step 1~10 완료(2026-11-20), [Phase 3 인계](20261120_02_Phase3Handoff.md)와 [최종 기준선](20261120_03_Phase2Step10Verification.json)을 받았다.
- 기준선은 안전 hash 1,046파일·별도 환경의 안전 전달 준비 목록 961파일·그 밖의 추적 경로 11개다. 이 수치는 선행 관측값이며 실행 시 건수를 고정해 통과시키지 않는다. 미커밋/미추적 도구/문서가 있으므로 HEAD만 checkout하면 현재 결과가 빠진다.
- Editor 6000.3.5f2, 현재 UPM 61개/edge 91개, com.unity.ugui 2.0.0의 TMP 공급 archive와 schema 2 보존본 83파일/폴더 meta 8개, Cloud Code 2.10.4의 npm tgz 27개가 고정 근거다. 개별 hash·alias/version/integrity는 기존 spec/metadata를 참조한다.
- URP 11 GUID/15회·TMP graph 1 GUID/16회 미해결과 sample orphan 1개가 남아 inventory는 수집 완료의 exit 1이었다. 정적/Unit Test 통과를 이 참조의 실제 정상으로 승계하지 않는다.
- 실제 npm 경로 이식·새 lock/설치·NuGet 새 restore/lock·별도 환경의 Unity 검증은 미실행이다. 기존 local cache/obj·과거 Player/Runner 성공은 이번 독립 복원 성공이 아니다.

# 조사 내용

## Roadmap 완료 조건과 연결

| Phase 3 완료 조건 | 실제로 확보할 증거 |
| --- | --- |
| 필요한 외부 입력 도입 후 필수 참조 누락 없이 복원됨 | Step 2 필수 입력/조건, Step 4~6 별도 환경 준비/실제 의존성 결과, Step 7 참조/Import 대조, Step 8 필요한 사용자 Unity 결과 |
| 원본 비추적과 프로젝트 소유 자료 보존 | Step 1 원래 기준선, Step 3 안전 경로/보존 Test, Step 4 전달 목록과 후보 Git 구성, Step 9 실제 HEAD/index/staged·원래 자료 전후 검사 |
| 복원 제약/수동 단계와 결과 기록 | Step 2 부족 입력, Step 5~8 실제 사용자 명령/적용표·결과, Step 9 재현/원복 증거, Step 10 영구 기록·후속 도입 인계 |

## 계획 작성 시점의 도구와 추가 구현의 구분

`Tools/DependencyAudit`의 기존 audit/registry/adoption/preservation/ingress/restore/lifecycle 모듈과 7개 Node suite를 재사용한다. Step 3에서 **dependency-snapshot.cjs와 별도 suite**를 추가해 안전 snapshot·허용 변경 비교·필수 사용자 실행 결과 판정을 검증했다. 실제 후보 생성/전체 복원은 미실행이다. 새 snapshot CLI는 읽기 전용 `--check`만 있고 복사는 계획 해시를 명시한 API만 제공한다. 실제 입력/설치·Unity 의미 검증은 기존 도구 및 후속 Step과 함께 수행한다.

audit/registry/ingress/lifecycle에는 `--root`가 있지만 preservation/restore CLI에는 없다. 후자는 복사된 도구의 위치로 root를 정하거나 기존 API의 명시적 root를 쓰는 검증된 adapter가 필요하다. 없는 `--root`/apply 옵션을 제안 명령처럼 실행하지 않는다. preservation의 기존 reapplyToTemporary는 제한된 OS 임시 폴더용이며 실제 candidate의 Assets 재적용 도구가 아니다.

# 작업 내용

아래 Step을 순서대로 수행한다. Step이 크면 해당 Step 안에서 `3-1`, `3-2`처럼 나누고 결과/미실행 조건을 개별 기록한다. 단계 완료는 필요한 실행/입력·검사와 결과 근거가 모두 있을 때만 표시한다.

## Step 1. Phase 3 시작 기준선과 원래 작업본 보호 범위를 고정한다

1-1. AI가 Phase 2 최종 JSON 자체의 SHA-256을 새 metadata에 고정하고 parent/check/handoff·이전 metadata·도구 hash 연결을 대조한다. 기존 JSON은 덮어쓰지 않는다.

1-2. `restorationBaseline.originalWorkspace.files`·HEAD/index/tracked/staged·ignore·QA 실행 파일·보존본/archive를 현재 상태와 비교한다. 이번 계획 문서/연결 문서의 의도된 변경은 별도 경로/hash·이유로 기록하고, 예상 밖 변경은 사용자 작업을 덮어쓰지 않고 분리한다. 새로운 문서 상태를 과거 baseline에 넣어 검사 실패를 숨기지 않는다.

1-3. 민감 본문/실제 저장은 수집하지 않는다. QA 전체 디렉터리는 기존 실행 파일 hash 범위와 구분한다. 원래 생산 경로·선언/lock·원본/변경본·저장/Secret·HEAD/index의 불변 계약과 허용할 Task/보조 도구 변경 경로를 고정한다.

**산출물:** 새로운 Phase3 Step1 baseline/차이 metadata(실제 실행일의 Task 경로), 현재 대상/입력·범위/제외 목록. **완료:** 기준선 비교와 의도/예상 밖 변경을 구분하고 필수 입력 손실이 없다.

- [x] 원래 작업본의 현재성·보호 범위와 새 Step 1 기준선을 고정했다.

### Step 1 수행 결과 — 2026-11-20

- **1-1 완료 — 근거 연결:** [새 시작 기준선](20261120_06_Phase3Step1Baseline.json)에 Phase 2 최종 JSON 자체의 SHA-256 `22b13f9c2eab78697ae75d905b3cdd482c4ab08113f3097bce9ab6f759f5fe6d`를 고정했다. parent/check/handoff·이전 metadata·도구/정책·복원 입력·작성 검증의 경로별 pin 46개를 대조했다. 과거 JSON은 변경하지 않았다.
- **1-2 완료 — 현재성/차이:** 과거 안전 기준선 1,046파일 중 1,043파일은 동일하며, 3파일(Project Memory·Roadmap 10·로드맵 안내)은 작성 검증의 before/after hash·변경 이유와 일치했다. 손실/예상 밖 변경은 없다. 과거 기준선에 현재 값을 덮어쓰지 않고 차이와 현재 안전 파일 1,049개를 별도로 기록했다.
- **1-2 Git 이력 구분:** 시작 전 기존 커밋 `4e1d63b8a3c3efb1faa87927560d846199411a6e`(2026-11-20 19:36:28 KST)의 부모는 과거 HEAD `5a9b34d583bb744209a4444f69cd051ae88523df`다. 8개 문서 수정·42개 안전 근거/도구/계획 파일 추가로 추적 경로가 933→975개가 됐다. 추가 경로는 고정한 안전 자료에 포함되며 제외 추적 11개는 Git tree 항목이 동일하다. 과거의 미커밋/미추적 자료가 현재 커밋에 포함된 사실을 기록하고, 시작 시 clean worktree·staged 0개·현재 HEAD/index를 새 보호 기준으로 삼았다. AI는 commit/stage/reset을 수행하지 않았다.
- **1-3 완료 — 보호 범위:** TMP schema 2 보존본 83파일·폴더 meta 8개와 공급 archive, npm tgz 27개, 보존 중인 구 schema 1 payload를 검증했다. 보존본/archive의 ignore·비추적을 확인했다. QA는 Candidate02 실행 파일 hash만 대조했으며 전체 QA 디렉터리 정상·실행 성공을 주장하지 않는다. 실제 저장/Secret과 제외 추적 경로 본문은 수집하지 않았다. 생산 자산/설정·선언/lock·기존 도구·과거 근거·원본/변경본·QA/저장/Secret·시작 HEAD/index 보호 및 이번 허용 경로 6개를 기준선에 고정했다.
- **1-4 완료 — 최종 보존/문서 대조:** [Step 1 최종 검증](20261120_07_Phase3Step1Verification.json)에 기준선 이후 허용된 문서 4개 변경과 새 metadata 2개를 분리하고, 보호 파일/HEAD/index·추적/staged·보존본/archive 전후 대조 및 링크/JSON/공백 검증을 기록했다. 보조 도구/생산/Test 코드는 수정하지 않았으며 Unit Test·Unity/Build/Test Runner·Scene/Prefab 적용·설치·별도 후보 생성은 수행하지 않았다.

**판정:** Step 1 완료. 사용자 수동 작업은 없다. 제외 추적 11개의 필수 여부·안전한 제공, 실제 후보 root/권한·저장/계정 분리와 취득 입력의 확정은 Step 2에 남긴다. 이 기준선은 전달 목록 확정·권리 확인·참조 정상·독립 복원 성공이 아니다.

## Step 2. 전달 제외 11개·필수 입력·별도 환경/저장 분리를 판정한다

2-1. AI가 `candidateTransfer.excludedTrackedPaths`의 각 경로를 파일 역할·Git metadata·선언/EmbeddedResource/코드 참조로 조사한다. 이름 때문에 본문 차단된 일반 소스와 실제 비밀값/로컬 설정·복원 비필수 환경 파일을 구분한다. 정적으로 가능한 필요성 조사를 사용자에게 넘기지 않는다.

2-2. 공개/일반 코드의 추가 검사가 필요하면 최소 경로·읽기 정책을 검토하고 비밀값을 출력/복제하지 않는 방법을 먼저 검증한다. 기존 민감 경로 guard를 일괄 해제하지 않는다. 필요한 코드/설정 누락을 stub/자동 생성·샘플 값으로 감춰 성공시키지 않는다. 각 항목의 `필수 여부/근거/안전한 제공 방법/담당/준비 상태`를 기록하고 필수 자료가 없으면 후속 적용을 보류한다.

2-3. 고정 Editor/provider/archive와 npm/.NET toolchain·사용 조건/취득 경로·계정/네트워크/오프라인 제한을 대조한다. AI가 확인할 수 없는 실제 취득/계정/권리 정보만 `대상/확인된 사실/부족 정보/선택/영향`으로 사용자에게 요청한다. 기존 보존 결정을 반복 승인받지 않는다.

2-4. 구체적인 candidateRoot/출력/별도 저장·테스트 계정 경로와 권한을 실행 환경에서 확정한다. 원래 root와 동일/상하위 관계·junction/symlink·기존 데이터가 있는 대상은 거절한다. 현 workspace 밖 쓰기는 실제 환경의 권한을 확보한 후 진행한다. Editor 초기화/Player가 원래 저장·production 서비스/계정을 건드리지 않는 조건을 먼저 확인한다.

**산출물:** 입력 완전성/제외 경로 판정표·실제 root/환경 계약·필요한 사용자 질문. **완료:** 현재 단계에 필요한 결정/경로·필수 자료가 확정됐다. 미응답은 승인으로 바꾸지 않는다.

- [x] 필수 입력/제외 경로와 별도 root·저장/계정 분리·권한을 확정했다.

### Step 2 수행 결과 — 2026-11-20

**2-1 완료 — 제외 추적 경로 11개 판정.** [입력/환경 감사](20261120_08_Phase3Step2InputAudit.json)에 각 경로의 역할·필수 여부·근거·제공 방법·담당/준비 상태와 해시를 기록했다. 서버 원본 2개는 `../../CloudCode/*.js` EmbeddedResource 및 직접 import에 필요하며, 테스트 2개도 기존 서버 회귀 입력을 온전히 보존하기 위해 필수다. 실제 영수증 데이터와 이 소스 파일을 구분했다.

| 경로 | 판정/제공 |
| --- | --- |
| `.vscode/settings.json` | 로컬 편집기 설정. 현재 복원/컴파일에 비필수, 본문 미수집·전달 제외 |
| `.vsconfig` | Visual Studio 설치 설정. 고정 Unity/.NET CLI 복원에 비필수, 본문 미수집·전달 제외 |
| `README.md` | 저장소 제목 문서. 비필수지만 일반 reader로 확인한 해시 그대로 전달 가능 |
| `UGS/CloudCode/receipt-journal.js` | 필수 서버 소스. 검토한 정확한 해시/최소 읽기 정책으로 전달 |
| `UGS/CloudCode/receipt-store.js` | 필수 서버 소스. 검토한 정확한 해시/최소 읽기 정책으로 전달 |
| `UGS/LogSink/.env.example` | 현재 비실행인 LogSink 설정 template. 비필수, 본문 미수집·전달 제외; 실제 `.env`는 계속 금지 |
| `UGS/LogSink/CLOUD_RUN_SECRET_AND_IAM.md` | 현재 비실행인 원격 Secret/IAM 운영 문서. 비필수, 본문 미수집·전달 제외 |
| `UGS/Tests/receipt-journal.test.cjs` | 기존 회귀 입력 보존에 필수. 합성 대역을 쓰는 소스, 해시 고정 후 전달 |
| `UGS/Tests/receipt-store.test.cjs` | 기존 회귀 입력 보존에 필수. 합성 대역을 쓰는 소스, 해시 고정 후 전달 |
| `UGS/Verification/New-TransferHmacSecret.ps1` | Secret 생성기 소스 자체는 비밀값이 아님. 비필수 보조 소스로 전달 가능, 실행/생성값 수집 금지 |
| `UGS/Verification/new-transfer-hmac-secret.cjs` | Secret 생성기 소스 자체는 비밀값이 아님. 비필수 보조 소스로 전달 가능, 실행/생성값 수집 금지 |

**2-2 완료 — 최소 읽기/안전한 제공.** 이름으로 차단됐던 소스 6개만 정확한 경로 목록과 부모 link 검사로 읽었다. 범위 밖 5개 경로는 body 읽기 전에 거절됐으며 기존 전역 sensitive guard·도구/정책은 변경하지 않았다. JS 5개는 실행 없이 구문 검사(`vm.Script`)만, PS1은 PowerShell AST 구문 해석만 수행했고 생성기/모듈/Test를 실행하지 않았다. 구조/import·제한된 credential marker 검사를 확인했으며 payload/문자열 원문은 metadata/로그에 복제하지 않았다. 이 검사는 범용 Secret 탐지 보증이 아니다. Step 3에서 이 6개 경로와 고정 해시만 허용하는 전달 처리를 별도로 구현/검증해야 한다. stub/샘플 설정으로 필수 소스를 대체하지 않는다.

**2-3 완료 — 고정 입력/toolchain과 사용자 확인.** 기존 읽기 전용 restore check는 exit 0으로 UPM 61개/edge 91개와 ugui 2.0.0·Cloud Code 2.10.4 provider/선언/archive를 확인했다. schema 2 TMP 83파일·폴더 meta 8개와 tgz 27개는 보존 입력으로 유지한다. Editor 6000.3.5f2 설치, Node v24.21.0·npm.cmd 11.19.0, .NET SDK 9.0.315/10.0.301 설치를 확인했다. 이 후보의 .NET 복원은 SDK 9.0.315를 선택하며 후보 전용 global.json 계획은 Step 3/4에서 허용 변경으로 검증한 뒤 생성한다. PowerShell의 npm.ps1은 실행 정책으로 차단돼 npm.cmd를 사용하며 정책은 바꾸지 않았다. 사용자는 **기존 Unity 계정/의존성 사용 가능·공식 패키지/NuGet/npm 다운로드 가능**이라고 답했다. 이는 사용자 보고이고 실제 새 취득/설치·개별 재배포 권리 확인 완료는 아니다. 새 lock/npm 설치·NuGet restore 결과는 Step 6에서 검증한다. LogSink container/원격 배포/실제 계정 자료 복원은 현재 로컬 입력 복원에 비필수이며 digest·container 동작은 미검증으로 남긴다.

**2-4 완료 — 구체 경로/권한·저장/계정 분리.** 사용자가 제안한 임시 위치를 선택했다. 실행 상위 폴더는 `C:\Users\Admin\AppData\Local\Temp\FlowState_Phase3_20261120_01`, 후보는 그 안의 `Project`, 로그는 `Logs`, 출력은 `Outputs`다. 원래 프로젝트와 상하위 관계가 없고 현재 대상은 없으며 기존 부모 경로의 link 없음·임시 부모 쓰기 가능과 실제 임시 검토 script 쓰기를 확인했다. 현재 환경의 임시 폴더 쓰기 허용 범위를 사용한다. 폴더 생성/복사는 Step 4까지 하지 않으며, 직전에 경로/권한/기존 데이터·link를 다시 검사한다. 임시 위치는 장기 보존 보장이 아니다.

현재 필요한 게임 검증 범위는 기존 **Editor 격리 예약 → 메모리 저장·온라인 비활성**으로 고정했다. `GameSystem.InitializeLocalState`가 memory store를 선택하고 인증 gateway 생성 전에 반환하므로 기존 로컬 저장/인증 계정을 열지 않는다. 실제 Editor/Play는 사용자 담당이며 아직 수행하지 않았다. 향후 사용자는 일반 Windows 대상(FLOW_STATE_PRODUCTION 없음), Reload Domain/Scene 활성 상태에서 `Flow State > Online Record Verification` 창의 **Play 전 격리 실행 예약 (원격 호출 없음)**을 누르고 `IsolationArmed=True`를 확인한 다음 창을 유지한다. 원격 허용 toggle·A/B/Phase3New 세션 준비는 사용하지 않고 Play 종료/Editor 재시작마다 다시 예약한다. 이 절차는 Step 5/7/8의 입력/검증 준비 후 실행하도록 인계한다.

생산 Build Profile은 소스로 보존하지만 선택/실행하지 않는다. 현재 실 production config는 ignore 상태로 본문을 읽거나 전달하지 않는다. 원래 전역 Standalone define은 비어 있으며 production profile의 FLOW_STATE_PRODUCTION 조건에서는 해당 config가 별도 필수다. 일반 검증 범위에서 그 누락을 stub으로 보충하지 않는다. standalone Player는 원래 저장 경로와 겹칠 수 있어 현재 실행 범위에서 제외한다. 예약한 UserData 경로/기존 `--fs-verification-session`만으로는 분리를 보증하지 않으며, Step 8에서 Player 실행이 실제 필수가 되면 OS 사용자 데이터·인증 분리 방법을 먼저 별도로 확정한다. 기존 생산 Scene/Prefab과 설정은 변경하지 않았다.

**2-5 완료 — 최종 판정/보존.** [Step 2 최종 검증](20261120_09_Phase3Step2Verification.json)에 Step 1 근거 자체의 pin·허용 문서 변경과 보호 파일/Git/보존본 전후 비교, JSON/링크/공백 검증을 기록했다. **Step 2 완료**, 다음은 Step 3 안전 전달/비교 도구 최소 구현·Unit Test다. 현재 추가 수동 작업은 없으며 사용자 답변 2개를 기록했다. 후보 생성·설치/lock·Unity/Build/Test Runner·Scene/Prefab 적용·원격/Secret 생성은 미실행이다. Step 2 완료는 실제 독립 복원 성공이 아니다.

## Step 3. 안전 전달·후보 비교 도구를 최소 구현하고 Unit Test로 검증한다

3-1. 기존 모듈의 쓰기/프로세스/네트워크·require/main을 먼저 감사한다. 기본 check/plan은 읽기 전용, 명시적 전달/재적용은 별도 API/모드로 분리한다. 원래 root·허용 밖 대상에 쓰는 입력은 실패시킨다. 보존본 verify와 실제 자산/Scene 적용을 혼동하지 않는다.

3-2. 기존 reader/경로/Git/preservation/restore 함수로 해결되지 않는 안전 snapshot/허용 변경 비교만 추가한다. 외부 package 없이 Node/assert·합성 metadata·권한/부분 읽기 주입·임시 저장소를 우선 사용한다. fixture의 Git 초기화/임시 정리는 실제 원래 작업본에 적용하지 않는다.

| 필수 Unit Test 영역 | 정상/실패 입력과 확인할 책임 |
| --- | --- |
| 경로/전달 | 상하위/절대/공백·한글·대소문자·junction/symlink·중복/parent-file·기존 비어 있지 않은 대상/overwrite·허용 밖 Secret/생산 경로 거절 |
| 입력/복사 | 누락/추가·hash 오염·실제 body 읽기 실패·부분 열거/복사·권한 거절·미커밋 파일 포함·전달 제외 필수 입력을 빈 성공으로 만들지 않음·부분 출력 보존 |
| 복원 계약 | 폴더 meta/빈 디렉터리·GUID/Importer/Resources/Shader 이름·같은 GUID의 중복 버전·원본/파생 경계·공급 경로 suffix 변경과 오염/잘못된 버전 |
| 선언/결과 | BOM/JSON·npm alias/version/integrity/graph·경로만 바뀐 후보와 예상 밖 변경·NuGet 직접/전이/feed/resource drift·사용자 결과 누락/nonzero/예상 밖 로그 처리 |
| 보존/통합 | 원래 파일/HEAD/index 불변·읽기 전용 require/check·동일 입력 결과·실패 CLI nonzero·비밀값/원본 body 미노출·before/attempt/current 부분 원복과 늦은 변경 보호 |

3-3. 재사용할 기존 명령은 아래와 같다. 실행 시 실제 건수/실패 이름·원인/exit를 기록한다. 선행 261개를 정답으로 고정하지 않는다. 추가 suite/옵션은 실제 구현 후 기록한다. 코드/판정이 바뀌면 영향 suite를 재실행하고, 변경/실패가 없는데 동일 suite를 반복하지 않는다.

```powershell
node Tools/DependencyAudit/dependency-audit.test.cjs --policy AI/90_Tasks/Prototype_10/20261119_03_Phase1Step8Policy.json
node Tools/DependencyAudit/dependency-registry.test.cjs
node Tools/DependencyAudit/dependency-adoption.test.cjs
node Tools/DependencyAudit/dependency-preservation.test.cjs
node Tools/DependencyAudit/dependency-ingress.test.cjs
node Tools/DependencyAudit/dependency-restore.test.cjs
node Tools/DependencyAudit/dependency-lifecycle.test.cjs
node Tools/DependencyAudit/dependency-snapshot.test.cjs
```

**산출물:** 필요한 최소 도구/Test·실행/부작용 감사·정상/실패 결과. **완료:** 후보 쓰기 전 필요한 경계/실패 Test가 통과하고 원래 자료 보호를 입증했다.

- [x] 필요한 보조 도구와 정상/실패 Unit Test·읽기 전용/쓰기 경계를 검증했다.

### Step 3 수행 결과 — 2026-11-20

**3-1 완료 — 기존 부작용/재사용 감사.** [Step 3 감사](20261120_10_Phase3Step3Audit.json)에 모듈별 require/main·쓰기/프로세스/네트워크 경계를 기록했다. audit의 고정 Git reader, preservation.verify, restore의 기존 공급/이식 검사를 재사용했다. 기존 create/prepare-archives/임시 reapply API는 명시적 쓰기이며 일반 check/require와 구분했다. 8개 모듈을 쓰기/프로세스 API가 차단된 상태로 require해 쓰기/프로세스 실행 0건을 확인했다. Unity/설치/서비스 호출은 수행하지 않았다.

**3-2 완료 — 최소 snapshot/비교 구현.** [snapshot 도구](../../../Tools/DependencyAudit/dependency-snapshot.cjs)와 [사용 안내](../../../Tools/DependencyAudit/README.md)에 입력 계약/API를 정의했다. prepare/CLI `--check`는 읽기 전용이다. create는 다시 계산한 계획 해시·새 대상·허용 목록·필수 입력·source/Git/target 전후 hash가 모두 맞을 때만 exclusive 복사한다. 기존/상하위 대상·link/junction·경로/대소문자/parent-file 충돌·원본/QA/Secret/production config는 차단한다. 실패 출력은 자동 삭제하지 않는다. 폴더 meta/빈 폴더와 정확한 파일 byte를 보존하며 후보 전용 고정 SDK global.json만 생성할 수 있다. 기존 reader는 변경하지 않고 Step 2의 소스 6개에만 정확한 경로/고정 SHA-256 예외를 제공한다. 제한 공급 입력은 고정 근거로 검증한 npm tgz 27개·schema 2 보존본 manifest/payload 84개뿐이다.

**3-3 완료 — 정상/실패 Unit Test.** [새 suite](../../../Tools/DependencyAudit/dependency-snapshot.test.cjs) **79개**와 기존 7개 suite **261개**, 합계 **340개 통과/0개 실패**, 모든 최종 명령 exit 0이다. 기존 suite별 실측은 audit 55·registry 50·adoption 36·preservation 20·ingress 28·restore 30·lifecycle 42개다. 새 suite는 경로/한글/공백·상하위/대소문자·junction·중복/parent-file·기존 대상·누락/hash 오염·권한/부분 읽기/쓰기·빈 폴더/meta/GUID·제한 소스/공급 입력·Git/미커밋 입력 보존·후보 전용 SDK·허용 diff·부분 원복/늦은 변경·필수 사용자 결과/입력 pin·실패 CLI와 require 부작용을 검사했다. 복사/Git 초기화/commit·정리는 검증된 별도 합성 임시 fixture에만 수행했다. 최초 새 Test 파일 구문 오류를 수정한 후 정상 실행했고, 계획 해시 변조/Git 변경 실패 검증을 보강한 최종 79개 결과를 기준으로 삼는다. 이전 77개 중간 통과를 합계에 중복 계산하지 않는다.

복원 의미 검사는 역할별로 구분한다. 직접/이름 참조·GUID/Importer/Resources/Shader와 공급 경로 suffix·npm alias/version/integrity/graph는 기존 suite의 정상/실패 근거를 연결했다. 새 compare는 명시한 file hash 변경만 판정하며 NuGet 직접/전이/feed/resource 의미를 새로 파싱하거나 Unity 타입/subasset/동적/binary 해석을 인증하지 않는다. 실제 후보의 새 graph/lock·참조 의미/사용자 결과 검증은 Step 6~8에 남긴다. 모든 결과의 independentRestore는 false다.

**3-4 완료 — 실제 입력의 읽기 전용 계획 대조.** 이전 안전 전달 준비 목록 961개에 현재 문서/Step 1~2 근거·검토 소스/문서·새 도구와 제한 보존 입력을 추가한 사전 검사는 **1,089파일(후보 전용 SDK 포함)/109폴더**의 계획을 산출했다. 파일 읽기/해시·폴더 meta/GUID/필수 입력 검사는 통과했고 실제 확정 후보는 생성하지 않았다. 이 목록/해시는 문서/새 metadata 갱신 전 관측값이며 Step 4가 최신 파일/추가 근거와 최종 필수 목록을 다시 고정해야 한다. 기존 JSON/과거 목록은 고치지 않았다.

**3-5 완료 — 보존/최종 판정.** [Step 3 최종 검증](20261120_11_Phase3Step3Verification.json)에 이전 metadata 자체 pin·원래 파일/도구·Git·보존본/archive 전후 비교, 의도된 문서 5개 변경·새 도구/Test 2개·새 metadata 2개와 Unit Test 실행 근거를 기록했다. **Step 3 완료**, 다음은 Step 4의 최종 안전 전달 목록과 확정 후보 snapshot 준비다. 현재 수동 작업은 없다. Unity Editor/Compile/Build/Test Runner·Scene/Prefab 적용·설치·실제 후보 쓰기·원격/Secret 생성은 수행하지 않았다. snapshot byte 검증은 독립 Unity 복원 완료가 아니다.

## Step 4. 검증된 안전 목록으로 별도 snapshot을 준비한다

4-1. Step 2의 root/권한과 Step 3 도구 검증 후 안전 전달 목록을 구체화한다. Phase 2의 961개는 시작점이며 새 Task/도구/metadata와 승인된 필수 경로를 명시적으로 추가한 새 목록/hash를 기록한다. 과거 목록을 고쳐 쓰지 않는다.

4-2. 원래 .git/history·전체 Library/Ignore/Build·node_modules/obj/bin·실제 저장/Secret을 통째로 복사하지 않는다. 검사된 안전 파일과 미커밋 문서/도구를 별도 대상으로 전달하고, 보존본/archive는 필요한 제한 입력으로만 전달한다. exclusive 쓰기/전후 hash·목록·권한/부분 실패를 검사한다.

4-3. 후보에서 HEAD/index/staged를 검사하려면 후보만의 Git 구성을 명시적으로 준비한다. 실제 사용자 결정에 따라 검토된 안전 경로만 후보의 초기 추적 대상으로 구성하고 원본/보존본/archive는 비추적으로 둔다. 원래 index를 복사하거나 원래 저장소를 stage/commit하지 않는다. 후보 HEAD가 없으면 현재 Git CLI의 실패를 빈 통과로 바꾸지 않는다.

**산출물:** 실제 전달 manifest/후보 안전 입력·Git/ignore/실패 출력 상태·원래 작업본 비교. **완료:** 필수 코드/설정·미커밋 도구가 준비되고 원래 작업본 불변/원본 비추적·후보 경계가 확인됐다.

- [x] 별도 환경의 실제 안전 snapshot·입력 완전성/Git 경계를 확인했다.

### Step 4 수행 결과 — 2026-11-20

**4-1 완료 — 최종 입력/경계 고정.** [전달 manifest](20261120_12_Phase3Step4TransferManifest.json)에 Step 3 최종 JSON 자체의 SHA-256 `2be504a483dcb8bc38f94e39d293d762dca773b7887983cf1bb0a7130dc6c41e`와 현재 입력/원래 Git·보호 파일 1,175개를 고정했다. 과거 준비 목록 961개를 출발점으로, 현재 미커밋 문서/도구/Step 1~3 metadata와 검토 소스/문서를 추가해 프로젝트 입력 **979파일**, 제한 보존 입력 **111파일**을 확정했다. 현재 추적 975개는 전달 대상 또는 Step 2 제외 4개로 모두 설명되며 현재 미추적 입력도 포함했다. 후보 전용 SDK global.json 1개를 합쳐 **1,091파일/109폴더**다. 실제 쓰기 직전 원래 root와 상하위 관계 없음·대상 부재·기존 부모 link 없음·임시 경로 쓰기 가능을 재확인했다.

**4-2 완료 — 실제 exclusive snapshot.** 검증된 API의 계획 해시를 고정한 뒤 승인된 임시 `Project`에 실제 복사했고 파일/폴더 전체 목록과 각 byte hash를 대조했다. 원래 .git/history·전체 Library/Ignore/Build·node_modules/obj/bin·저장/Secret·실 production config는 복제하지 않았다. TMP는 Assets에 복사하지 않고 검증된 schema 2 manifest/payload 84개와 npm tgz 27개만 후보의 ignore 경로에 전달했다. 빈 폴더/meta·필수 서버 소스/회귀 입력도 보존했다. 실제 provider resolve/TMP 재적용과 설치는 Step 5~6에 남긴다.

초기 복사/전수 검증 후 Git ignore 검사에서 전역 `--literal-pathspecs`와 `check-ignore`의 옵션 호환 오류(exit 128)가 발생했다. 부분 출력/실패 로그를 보존하고 원래 파일/Git 불변과 후보의 전체 byte/목록·계획 해시를 다시 확인한 다음, literal 옵션을 add 명령에만 한정해 이어서 수행했다. 원래 작업본/후보의 reset·clean·삭제나 snapshot 재복사는 하지 않았다. 해결된 오류와 실제 명령 결과는 [실행 검사](20261120_13_Phase3Step4Check.json)에 기록했다.

**4-3 완료 — 후보만의 Git 구성.** 원래 .git/index/history를 사용하지 않고 빈 템플릿·훅·attributes와 시스템/전역 설정을 차단한 환경에서 후보 Git을 새로 초기화했다. 안전 프로젝트 파일 979개와 SDK pin 1개만 명시 목록으로 추가해 **980개 추적**, **111개 ignore/비추적**, staged 0개·clean worktree를 확인했다. 초기 branch는 `phase3-restore`, root commit은 `4d0d61924e50b7b0f8517ffd8deecc7af2d1b523`이며 부모 없는 로컬 commit 1개·remote 0개다. autocrlf false로 후보 blob과 snapshot byte hash도 대조했다. 원래 저장소에는 add/commit/config 변경을 하지 않았다.

HEAD 생성 전 managed gate의 exit 2를 실제 실패로 기록했고 빈 성공으로 처리하지 않았다. 초기 commit 후 registry·managed ingress·staged ingress·lifecycle **4개 CLI가 모두 exit 0**, 후보 schema 2 보존본 verify도 통과했다. 빈 등록부/유입 검사 통과는 권리·참조 정상·Unity 복원 성공이 아니다.

**4-4 완료 — snapshot 시점/후속 기록 분리.** 후보에는 manifest 고정 당시 최신 Step 3 완료 문서·도구/metadata(20261120_11까지)가 들어 있다. 자기 해시 순환과 기존 snapshot의 무기록 변경을 피하기 위해 이 Step 4 manifest와 이후 실행/최종 근거는 원래 Task 경로 및 후보 상위 `Outputs`의 sidecar로 보관한다. Step 4 완료 후 갱신한 원래 진행 문서 4개는 후보의 동결된 문서와 의도된 차이로 기록한다. 후보 문서를 최신 실행 지시로 오해하지 않고 **원래 작업본의 이 Task**를 기준으로 후속 작업한다. 실제 후보는 root Library/node_modules/obj와 TMP Assets가 없는 의존성 준비 전 snapshot이며 원래 cache를 재사용한 복원 성공이 아니다.

**4-5 완료 — 원래 자료/최종 판정.** [Step 4 최종 검증](20261120_14_Phase3Step4Verification.json)에 원래 생산 자산/설정·도구/Test·선언/lock·보존본/archive·QA 실행 파일·과거 metadata·HEAD/index 불변과 후보 전체성/ignore/추적을 연결했다. Scene/Prefab은 byte가 동일한 snapshot 입력으로만 전달했고 Editor 적용/변경은 하지 않았다. 코드/판정 변경이 없어 Step 3의 고정 source·340개 통과를 연결하며 같은 suite를 재실행한 것으로 기록하지 않는다. **Step 4 완료**, 현재 수동 작업은 없다. 다음은 Step 5의 고정 UPM/TMP 공급·변경본/Import 준비다. 사용자 Editor/설치 실행은 해당 입력/절차를 준비한 뒤 요청하며, 현재 Unity/빌드/Test Runner·서비스·Secret 생성은 수행하지 않았다.

## Step 5. 고정 UPM·TMP 공급/변경본과 Import 설정을 준비한다

5-1. 소유자가 분리된 환경에서 같은 Editor 6000.3.5f2로 manifest/lock에 따른 정확한 UPM을 준비한다. AI는 설치 metadata의 ID/version·선언/graph/source를 확인하고 suffix가 다른 cache도 다시 발견한다. 원래 Library의 통째 복사나 다른 버전 자동 선택은 하지 않는다.

5-2. AI가 com.unity.ugui 2.0.0의 archive hash와 schema 2 TMP 보존본을 전달 전후 검증한다. 현재 원래 환경의 기존 검증 명령은 다음과 같다. root 결정 방식은 앞서 감사한 구현을 따른다.

```powershell
node Tools/DependencyAudit/dependency-preservation.cjs --verify AI/90_Tasks/Prototype_10/20261119_14_Phase2Step4Check.json Ignore/DependencyPreservation/tmp-baseline/29efef842bcf96ba6475752e4e14fae61508b6c84b3c9a2e0595c2c59867cc5c
node Tools/DependencyAudit/dependency-restore.cjs --check AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json
```

5-3. 대상에 이미 존재하는 파일/동일 GUID와 before 상태를 조사하고, 변경 폰트/설정·자산/폴더 meta·빈 폴더·GUID/Import/platform·Resources/Shader 이름을 재적용할 정확한 표를 준비한다. 필요한 실제 Unity import/자산 적용은 사용자가 수행하고 AI는 전후 byte/metadata·참조를 대조한다. schema 1 bundle/공급 archive만으로 변경본 재현을 선언하지 않는다. TMP bundle의 Library 경로까지 candidate에 무조건 복사하지 않는다.

**산출물:** 실제 provider/공급 hash·재적용표/사용자 결과·TMP/설정 대조. **완료:** 필요한 원본/변경본·설정/meta가 같은 버전의 후보에 준비됐고 필수 자료 손실/충돌이 없다.

- [x] 실제 후보의 고정 UPM/TMP·변경본/Import·폴더 보존을 확인했다.

### Step 5 수행 결과/사용자 절차 — 2026-11-20

**5-1 완료 — 실제 후보 UPM 대조.** [준비 감사](20261120_15_Phase3Step5InputAudit.json)와 [실제 검사](20261120_18_Phase3Step5ActualCheck.json)를 연결했다. 사용자는 후보 Editor가 열렸고 오류·경고 없이 종료했다고 보고했다. 로그 metadata의 engine version은 6000.3.5f2이며 후보 PackageCache의 **61개 package ID/version/source·의존 관계 91개**가 고정 선언/lock과 일치한다. ugui 2.0.0·Cloud Code 2.10.4를 이름/버전 metadata로 재발견했고 후보의 기존 restore.check API도 읽기 전용으로 통과했다. 해당 API의 installed=false는 API가 설치를 실행하지 않았다는 뜻이며 실제 UPM resolve/import는 사용자 실행으로 구분한다. 전용 cache에는 새 파일 156개가 생겼지만 로그에 cache override 경로가 직접 출력되지는 않아, 이를 전체 네트워크 취득 경로/byte provenance 인증으로 해석하지 않는다.

**5-2 완료 — 실제 공급 archive/보존본 hash.** 후보에서 resolve된 ugui 2.0.0의 TMP Essential Resources.unitypackage SHA-256 `58564ec64cef8a8cea3d0b8ffb5e6fecf4176d544a2f6a29a2b43a0a33256431`가 원래 provider·원래/후보 schema 2 보존본의 공급 archive와 일치했다. 두 schema 2 보존본 83파일·폴더 meta 8개를 재검증했고 schema 1 보존본도 유지했다. 과거 JSON/expected hash나 공급 경로를 새 상태로 덮어쓰지 않았다.

**5-3 완료 — 사용자 TMP 적용 결과.** [재적용표](20261120_16_Phase3Step5TmpReapplyPlan.json)의 Assets **82파일·meta 45개·폴더 meta 8개**가 실제 후보에 존재하며 byte hash·GUID/Importer/platform·Resources/Shader 이름과 빈 폴더 쌍이 모두 일치했다. 후보 Assets의 전체 meta는 285→330개이며 GUID 중복은 0개다. LiberationSans SDF/Fallback 변경본과 TMP Settings/Material도 보존됐다. 공급 unitypackage는 Ignore에 남고 보존본의 옛 Library 경로는 재적용하지 않았다. 동결된 기존 1,091파일과 Scene/Prefab·선언/lock은 모두 동일하다. 후보의 신규 `.vsconfig`는 version/components만 가진 Unity/IDE 생성용 비필수 설정으로 확인해 비추적 상태로 보존했다. 후보 추적 변경/스테이징은 0개이며 이를 전체 worktree clean으로 잘못 표시하지 않는다.

**5-4 사용자 실행 완료 — 아래 최초 실행 절차는 이력으로 보관한다.**

**이미 TMP 적용/Editor 준비가 완료됐으므로 아래 블록을 다시 실행하지 않는다.** 반복 실행에서 남아 있던 백그라운드 Hub 때문에 발생한 사전 중단은 [실제 검사](20261120_18_Phase3Step5ActualCheck.json)에 구분했다. AI는 프로세스 강제 종료·기존 대상 덮어쓰기·reset/clean·자산 재복사 없이 실제 출력/사용자 보고를 검증했다.

1. **Unity Editor와 Unity Hub를 모두 종료**한다. 원래 프로젝트/저장·계정·Secret은 복사하지 않는다.
2. 아래 PowerShell 블록을 **사용자가 직접 붙여넣어 실행**한다. 고정된 계획/보존본·후보 HEAD/index·대상 부재를 확인한 뒤 정확한 Assets 파일 82개와 폴더 8개만 새로 복사하고, 설치된 6000.3.5f2를 후보 경로로 연다. AI는 이 블록을 구문 검사만 했으며 복사/Editor 실행은 하지 않았다. 기존 대상·파일/hash 불일치·부분 실패가 있으면 중단하며 덮어쓰기/자동 삭제/재시도를 하지 않는다.

```powershell

# Step5 user-only asset application and Editor launch; AI must not execute this block.
$ErrorActionPreference = 'Stop'
$phase5PlanPath = 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261120_16_Phase3Step5TmpReapplyPlan.json'
$phase5ExpectedPlanHash = '0e9e8325d2374d06abbc2c23d0cef88aea61436a1dbb6bd950b96f3d80cfd5f3'
if ((Get-FileHash -LiteralPath $phase5PlanPath -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase5ExpectedPlanHash) { throw 'Step5 plan changed; stop for review.' }
$phase5Plan = Get-Content -LiteralPath $phase5PlanPath -Raw -Encoding UTF8 | ConvertFrom-Json
if (Get-Process -Name 'Unity','Unity Hub' -ErrorAction SilentlyContinue) { throw 'Close Unity Editor and Unity Hub first.' }
function Assert-Phase5NoLink([string]$phase5Path) {
    $phase5Cursor = [System.IO.Path]::GetFullPath($phase5Path)
    while ($phase5Cursor) {
        if (Test-Path -LiteralPath $phase5Cursor) {
            $phase5Item = Get-Item -LiteralPath $phase5Cursor -Force
            if (($phase5Item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Link/junction found; stop.' }
        }
        $phase5Parent = Split-Path -Parent $phase5Cursor
        if ($phase5Parent -eq $phase5Cursor) { break }
        $phase5Cursor = $phase5Parent
    }
}
Assert-Phase5NoLink $phase5Plan.candidateRoot
Assert-Phase5NoLink $phase5Plan.bundleRoot
$phase5Head = & git --no-optional-locks -C $phase5Plan.candidateRoot rev-parse HEAD
if ($LASTEXITCODE -ne 0 -or $phase5Head.Trim() -ne $phase5Plan.expectedCandidateHead) { throw 'Candidate HEAD changed.' }
$phase5Index = Join-Path $phase5Plan.candidateRoot '.git\index'
if ((Get-FileHash -LiteralPath $phase5Index -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase5Plan.expectedCandidateIndexSha256) { throw 'Candidate index changed.' }
$phase5Status = & git --no-optional-locks -C $phase5Plan.candidateRoot status --porcelain=v1 --untracked-files=all
if ($LASTEXITCODE -ne 0 -or $phase5Status) { throw 'Candidate has tracked/untracked changes; stop for review.' }
$phase5Manifest = Join-Path $phase5Plan.bundleRoot 'manifest.json'
if ((Get-FileHash -LiteralPath $phase5Manifest -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase5Plan.bundleManifestSha256) { throw 'TMP bundle manifest changed.' }
foreach ($phase5Map in $phase5Plan.mappings) {
    $phase5Input = Join-Path $phase5Plan.candidateRoot $phase5Map.source
    $phase5Output = Join-Path $phase5Plan.candidateRoot $phase5Map.target
    Assert-Phase5NoLink $phase5Input
    Assert-Phase5NoLink $phase5Output
    if (Test-Path -LiteralPath $phase5Output) { throw 'TMP target already exists; do not overwrite.' }
    if ((Get-FileHash -LiteralPath $phase5Input -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase5Map.afterSha256) { throw 'TMP input hash mismatch.' }
}
foreach ($phase5Folder in $phase5Plan.directories) {
    $phase5OutputFolder = Join-Path $phase5Plan.candidateRoot $phase5Folder
    Assert-Phase5NoLink $phase5OutputFolder
    if (Test-Path -LiteralPath $phase5OutputFolder) { throw 'TMP folder already exists; stop.' }
}
$phase5Cache = $phase5Plan.editorLaunch.childEnvironment.UPM_CACHE_ROOT
$phase5Log = $phase5Plan.allowedCandidateChanges.dedicatedEditorLog
Assert-Phase5NoLink $phase5Cache
Assert-Phase5NoLink $phase5Log
if ((Test-Path -LiteralPath $phase5Cache) -or (Test-Path -LiteralPath $phase5Log)) { throw 'Step5 cache/log already exists; retain it and ask for review.' }
if (-not (Test-Path -LiteralPath $phase5Plan.editorLaunch.executable -PathType Leaf)) { throw 'Pinned Editor is unavailable.' }
foreach ($phase5Folder in ($phase5Plan.directories | Sort-Object Length)) {
    $phase5OutputFolder = Join-Path $phase5Plan.candidateRoot $phase5Folder
    Assert-Phase5NoLink $phase5OutputFolder
    New-Item -ItemType Directory -Path $phase5OutputFolder -ErrorAction Stop | Out-Null
}
foreach ($phase5Map in $phase5Plan.mappings) {
    $phase5Input = Join-Path $phase5Plan.candidateRoot $phase5Map.source
    $phase5Output = Join-Path $phase5Plan.candidateRoot $phase5Map.target
    Assert-Phase5NoLink $phase5Input
    Assert-Phase5NoLink $phase5Output
    if ((Get-FileHash -LiteralPath $phase5Input -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase5Map.afterSha256) { throw 'TMP source changed during application.' }
    [System.IO.File]::Copy($phase5Input, $phase5Output, $false)
    if ((Get-FileHash -LiteralPath $phase5Output -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase5Map.afterSha256) { throw 'TMP output hash mismatch; retain partial output.' }
}
New-Item -ItemType Directory -Path $phase5Cache -ErrorAction Stop | Out-Null
$phase5Launch = New-Object System.Diagnostics.ProcessStartInfo
$phase5Launch.FileName = $phase5Plan.editorLaunch.executable
$phase5Launch.WorkingDirectory = $phase5Plan.candidateRoot
$phase5Launch.Arguments = '-projectPath "{0}" -logFile "{1}"' -f $phase5Plan.candidateRoot, $phase5Log
$phase5Launch.UseShellExecute = $false
foreach ($phase5Key in $phase5Plan.editorLaunch.removeChildEnvironment) { $phase5Launch.EnvironmentVariables.Remove($phase5Key) }
$phase5Launch.EnvironmentVariables['UPM_CACHE_ROOT'] = $phase5Cache
[System.Diagnostics.Process]::Start($phase5Launch) | Out-Null
Write-Host 'TMP copy verified. Candidate Editor launched with a fresh UPM cache; do not enter Play, build or run tests.'
```

3. 열린 후보 Editor에서 패키지 다운로드/resolve와 import가 끝날 때까지 기다린다. **다른 Editor 버전으로 변경, 패키지 업그레이드, production Build Profile 선택, TMP Essentials/Samples 재가져오기, Play/빌드/Test Runner·Scene/Prefab 저장은 하지 않는다.** TMP Essentials 안내가 뜨면 취소하고, 오류로 정상 진행이 불가능하면 임의 보충하지 말고 해당 오류만 전달한다. 기본 archive를 다시 import하면 보존된 변경 폰트/설정이 덮일 수 있다.
4. 완료되면 Console의 Error/Warning 유무·발생한 정확한 오류를 알려주고 **Scene 변경을 저장하지 않은 채 후보 Editor를 종료**한다. 실제 로그는 후보 상위 `Logs\Step5Editor_01.log`, 새 UPM cache는 `Caches\UPM_Step5_01`이다. 계정/키/Secret 원문은 전달하지 않는다. 두 동작을 마친 뒤 “TMP 적용과 패키지 준비 후 Editor 종료, Console 오류/경고 …”라고 알려주면 AI가 실제 candidate metadata·archive·82파일/45 meta/8폴더·선언/lock·원래 자료/Git을 직접 재검증한다. 파일 누락/hash/필수 참조나 예상 밖 로그는 성공으로 처리하지 않는다.

UPM cache의 환경변수 지원/우선순위는 [Unity 6.3 공식 문서](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/package-manager-caches/upm-config-cache)로 확인했다. 위 블록은 **자식 Editor 프로세스에만** UPM_CACHE_ROOT를 지정하고 예전 subsidiary cache 변수를 제거한다. 전역 환경변수/Preferences/사용자 설정·기존 cache는 변경하거나 삭제하지 않는다. 새 cache 경로는 현재 비어 있는 별도 후보 상위 경로이며 실제 취득/경로 사용은 사용자 실행 후 metadata와 로그 근거로 다시 확인한다.

**5-5 완료 — 사용자 보고/원래 자료 보존/최종 판정.** [준비 검증](20261120_17_Phase3Step5PreparationVerification.json)은 미실행 시점의 역사적 근거로 보존하고, [실제 검사](20261120_18_Phase3Step5ActualCheck.json)와 [최종 검증](20261120_19_Phase3Step5Verification.json)에 사용자 보고·실제 UPM/TMP 결과와 보호 파일 1,181개·원래 HEAD/index·보존본/archive의 불변과 후보 HEAD/추적 객체·파일 보존/허용 출력을 연결했다. 최종 대조 중 후보 index의 byte hash가 최초 실제 검사 뒤 달라진 관측은 별도로 기록했으며 원인은 단정하지 않는다. 후보의 980개 추적 mode/OID/path·HEAD·실제 파일은 같고 assume-unchanged/skip-worktree 변경이나 staged 변경도 없어, 후보 index를 되돌리거나 원래 index 불변으로 오인하지 않고 보존했다. 사용자 Console 오류/경고 없음은 보고 근거이며 AI가 Console/Runner를 실행한 결과가 아니다. 로그에서는 engine version과 제한된 오류 패턴 0개만 확인했고 원문/계정/Secret은 metadata에 수집하지 않았다. **Step 5 완료(2026-11-20)**, 현재 추가 수동 작업은 없다. Step 6의 실제 npm 이식/새 lock/ci·NuGet restore와 Step 7~8의 참조/Unity 타입·subasset/렌더링 검증은 미실행이다. Step 5의 공급/Import 입력 보존을 Phase 3 전체/독립 복원·권리 확인 성공으로 확대하지 않는다.

## Step 6. npm 이식 경로와 새 lock·NuGet 보조 의존성을 검증한다

6-1. AI가 Cloud Code 2.10.4의 정확한 tgz 27개를 provider metadata로 찾아 내부 name/version·alias·SHA-256/SHA-512 integrity/dependencies와 대조하고, 별도 환경의 비추적 안정 경로를 확인한다. 재취득 제한/누락이면 구체 원인으로 중단한다.

6-2. Step 2에서 고정한 npm/toolchain·후보 root에서 기존 [manifest patch](20261119_21_Phase2Step6NpmPathMigration.patch)의 적용 가능성/예상 diff·원복 입력을 AI가 검사하고, 사용자가 검토된 경로 변경과 실제 lock 생성을 수행한다. 다음 명령은 **후보 root에서 필요한 시점의 사용자 실행**이다.

```powershell
npm install --package-lock-only --ignore-scripts --no-audit --no-fund
```

6-3. 후보의 실제 package.json/package-lock.json 두 JSON을 검증된 경로로 받아 원래 선언과 대조한다. 기존 CLI는 원래 root의 spec/선언을 읽으므로 원래 환경의 `Ignore/DependencyRestoreCandidate/`에 두 후보 JSON만 검토용 metadata로 준비했을 때 다음 기존 명령을 쓸 수 있다. 원래 manifest/lock은 덮어쓰지 않는다.

```powershell
node Tools/DependencyAudit/dependency-restore.cjs --verify-migration AI/90_Tasks/Prototype_10/20261119_19_Phase2Step6RestoreSpec.json Ignore/DependencyRestoreCandidate/package.json Ignore/DependencyRestoreCandidate/package-lock.json
```

6-4. 지정 경로 외 alias/version/integrity/graph·다른 필드 변화는 실패/검토 대상으로 기록한다. mapping을 손으로 적용한 lock을 실제 생성/설치 성공으로 취급하지 않는다. 필요한 사용자 `npm ci --ignore-scripts --no-audit --no-fund`의 실제 결과를 연결한다. 의도된 경로 이식 후에는 새 후보 기준선으로 검사하고 기존 spec hash를 새 파일로 덮어쓰지 않는다.

6-5. .NET 보조 의존성이 복원 범위에서 필수라면 소유자가 후보 root에서 `dotnet restore UGS/Modules/FlowStateVerification/FlowStateVerification.csproj --use-lock-file`를 실행한다. AI는 실제 graph/lock의 net9.0·직접 4개/기존 전이 13개·feed·EmbeddedResource 2개 선언/실제 source를 대조한다. local obj를 새 restore로 승계하지 않는다. LogSink의 node:22-alpine tag/digest·동봉 조건·container 실행 필요성도 분리해 기록한다. Module 빌드/배포·복구나 실제 서비스 호출은 이 복원 작업에 섞지 않는다. 비필수라면 근거와 미검증 범위를 남기고 전체 보조 체계 성공을 주장하지 않는다.

**산출물:** 실제 후보 manifest/lock·필요 설치/restore 결과·graph/경로/자원 검증·미확인/비필수 근거. **완료:** 필요한 실제 결과가 있고 필수 실패/미실행을 통과로 처리하지 않았다.

- [x] 필수 npm/보조 의존성의 실제 경로/lock/설치 결과 또는 비필수 근거를 검증했다.

### Step 6 수행 결과/사용자 Stage A — 2026-11-20

**6-1 정적 검증 완료 — 실제 공급/안정 경로.** [Step 6 입력 감사](20261120_20_Phase3Step6InputAudit.json)에 Step 5 최종 JSON의 자체 SHA-256 `d2cb69dabca504fadcf3a51088bfe15e6d08e6dcd593466b98fdebe5b71e79c0`와 원래 보호 파일 1,183개·실제 후보 상태를 고정했다. Cloud Code 2.10.4의 tgz **27개**를 새 후보 provider metadata에서 찾아 비추적 안정 경로와 byte 대조했고 내부 name/version·설치 alias·SHA-256/SHA-512 integrity/dependencies가 spec/기존 lock과 일치했다. 원래 선언/lock과 후보의 이식 전 선언/lock은 같으며 실제 node_modules/새 NuGet obj/lock은 없다. 후보 추적/staged 변경은 0개이고 기존 비추적 .vsconfig는 그대로다.

**6-2 정적 준비 완료 — manifest 변경/보존본.** [복원 계획](20261120_21_Phase3Step6RestorePlan.json)·[적용할 package.json](20261120_22_Phase3Step6ProposedPackage.json)을 준비했다. 기존 patch의 후보 `git apply --check --whitespace=error-all`는 exit 0이었다. 제안 JSON은 원래 줄바꿈/다른 필드를 유지하고 정확한 file 의존성 경로 **27개만** Library/PackageCache에서 Ignore/DependencyArchives로 바꾼다. 역변환한 JSON과 원래 JSON은 같으며 alias/version/ccaVersion 등은 변경하지 않는다. 예상 lock 경로 필드는 root 27개·resolved 27개다. 원래 package.json/lock은 후보 상위 Outputs/Step6Before에 해시 검증한 복사본으로 보존했고 실제 후보 파일은 아직 수정하지 않았다. 제안 JSON/경로 mapping은 실제 생성된 lock이 아니다.

**6-3 실제 lock 정적 검증 완료(2026-11-25)·원래 npm 종료 코드 미확인.** 사용자가 Stage A를 실행해 manifest 적용과 실제 npm lock 생성까지 수행했다. `up to date in 2s` 이후 `npm notice`가 Windows PowerShell의 `NativeCommandError`로 처리돼 스크립트가 중단됐다. 원래 결과 JSON의 npm exit는 null이며 이를 exit 0으로 보완하지 않는다. 실제 두 JSON을 원래 선언/lock과 기존 validateMigration API로 대조했고 경로 외 alias/version/integrity/graph 불변 검증은 통과했다. NuGet은 실행 전이며 다음의 재개 절차를 사용한다. 아래 최초 Stage A 블록은 이력으로 보존하며 다시 실행하지 않는다.

**6-4 완료(2026-11-25).** 사용자 npm ci exit 0과 설치된 57개/의존성 연결 65개·실제 name/alias/version/dependencies·hidden lock 일치·전용 cache archive SRI를 검증했다. 기존 crypto-js@4.1.1 유지보수 중단 경고는 아래 최종 판정에 기록했다. 실행 파일은 이력으로 보존하며 반복하지 않는다.

**6-5 완료(2026-11-25).** 사용자 NuGet Linux 보완 restore exit 0과 전체 13개/기본·Linux target·lock/cache/feed/resource 대조는 아래 최종 검증에 연결했다. 다음 내용은 최초 정적 준비 이력이다. 기존 결정상 Module의 완전한 로컬 의존성 준비에는 새 restore가 필요하다. net9.0·SDK **9.0.315**·직접 PackageReference **4개**·기존 resolved graph **13개**와 EmbeddedResource 선언 2개/실제 source 40개를 대조했다. csproj에는 임의 Exec/Target/Import가 없고 Module 빌드/실행을 요청하지 않는다. [전용 NuGet 설정](20261120_23_Phase3Step6NuGet.config)은 기존 source의 Microsoft SDK offline feed와 공식 nuget.org 두 곳만 유지하며 사용자 자격 증명/전역 NuGet 설정을 읽거나 복제하지 않는다. packages/HTTP cache/CLI home은 후보 상위의 새 전용 경로로 분리한다. SDK 도움말 조회에서 사용자 .dotnet 초기화 경로 쓰기가 거절돼, 별도 임시 CLI home과 인증서/PATH/telemetry/workload 변경 비활성 환경으로 **help만** 재확인했다. 실제 restore/build는 수행하지 않았다.

**사용자 실행 — Stage A 한 번만 수행한다.**

아래 블록을 PowerShell에 붙여넣어 직접 실행한다. 고정된 계획/제안 파일·기존 후보 상태와 버전을 확인하고, **후보 package.json만** 변경한 뒤 npm lock 생성과 NuGet restore를 순서대로 수행한다. 이미 새 cache/log/lock/obj나 manifest 변경이 있으면 중단한다. 오류 후 전체 블록을 다시 실행하거나 결과를 삭제하지 말고 AI에게 현재 결과를 알려준다. Unity Editor/Hub·Play·빌드·Test Runner·Scene 작업은 이 절차에 없다.

```powershell

# Step6 user Stage A only: apply reviewed manifest, generate real npm lock, restore NuGet. No npm ci/build/Unity.
$ErrorActionPreference = 'Stop'
$phase6PlanPath = 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261120_21_Phase3Step6RestorePlan.json'
$phase6PlanHash = '3a571a6fb6e010a2280f85edb1a4520ab49c11f9af8eec5e7cd3acb765e336f5'
if ((Get-FileHash -LiteralPath $phase6PlanPath -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6PlanHash) { throw 'Step6 plan changed; stop.' }
$phase6Plan = Get-Content -LiteralPath $phase6PlanPath -Raw -Encoding UTF8 | ConvertFrom-Json
$phase6SourceRoot = 'C:\Unity\Unity_Flow_State'
$phase6Root = $phase6Plan.candidateRoot
function Assert-Phase6NoLink([string]$phase6Path) {
    $phase6Cursor = [System.IO.Path]::GetFullPath($phase6Path)
    while ($phase6Cursor) {
        if (Test-Path -LiteralPath $phase6Cursor) {
            if (((Get-Item -LiteralPath $phase6Cursor -Force).Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Link/junction found; stop.' }
        }
        $phase6Parent = Split-Path -Parent $phase6Cursor
        if ($phase6Parent -eq $phase6Cursor) { break }
        $phase6Cursor = $phase6Parent
    }
}
Assert-Phase6NoLink $phase6Root
$phase6Head = & git --no-optional-locks -c "safe.directory=$phase6Root" -C $phase6Root rev-parse HEAD
if ($LASTEXITCODE -ne 0) { throw 'Candidate Git read failed; see Git error above. No package files changed.' }
if ($phase6Head.Trim() -ne $phase6Plan.expectedHead) { throw 'Candidate HEAD changed.' }
& git --no-optional-locks -c "safe.directory=$phase6Root" -C $phase6Root diff --quiet
if ($LASTEXITCODE -ne 0) { throw 'Candidate tracked files changed.' }
& git --no-optional-locks -c "safe.directory=$phase6Root" -C $phase6Root diff --cached --quiet
if ($LASTEXITCODE -ne 0) { throw 'Candidate index has staged changes.' }
$phase6Package = Join-Path $phase6Root 'package.json'
$phase6Lock = Join-Path $phase6Root 'package-lock.json'
if ((Get-FileHash -LiteralPath $phase6Package -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.beforePackageSha256) { throw 'Package already changed; do not rerun.' }
if ((Get-FileHash -LiteralPath $phase6Lock -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.beforeLockSha256) { throw 'Lock already changed; stop.' }
$phase6Proposed = Join-Path $phase6SourceRoot $phase6Plan.proposedPackage.path
$phase6NugetConfig = Join-Path $phase6SourceRoot $phase6Plan.nugetConfig.path
foreach ($phase6Input in @(@{ Path=$phase6Proposed; Hash=$phase6Plan.proposedPackage.sha256 }, @{ Path=$phase6NugetConfig; Hash=$phase6Plan.nugetConfig.sha256 }, @{ Path=$phase6Plan.npmUserConfig.path; Hash=$phase6Plan.npmUserConfig.sha256 }, @{ Path=$phase6Plan.npmGlobalConfig.path; Hash=$phase6Plan.npmGlobalConfig.sha256 })) {
    Assert-Phase6NoLink $phase6Input.Path
    if ((Get-FileHash -LiteralPath $phase6Input.Path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Input.Hash) { throw 'Step6 input hash mismatch.' }
}
foreach ($phase6Backup in $phase6Plan.backups) {
    if ((Get-FileHash -LiteralPath $phase6Backup.path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Backup.sha256) { throw 'Step6 backup changed.' }
}
foreach ($phase6Cache in $phase6Plan.cache.PSObject.Properties) {
    Assert-Phase6NoLink $phase6Cache.Value
    if (Test-Path -LiteralPath $phase6Cache.Value) { throw 'Step6 cache already exists; retain it and request review.' }
}
foreach ($phase6Log in $phase6Plan.logs.PSObject.Properties) {
    Assert-Phase6NoLink $phase6Log.Value
    if (Test-Path -LiteralPath $phase6Log.Value) { throw 'Step6 log/result exists; do not overwrite.' }
}
foreach ($phase6Absent in @('node_modules','.npmrc','UGS/Modules/FlowStateVerification/obj','UGS/Modules/FlowStateVerification/packages.lock.json')) {
    if (Test-Path -LiteralPath (Join-Path $phase6Root $phase6Absent)) { throw 'Unexpected existing restore output/config; stop for review.' }
}
if ((& node --version) -ne $phase6Plan.toolchain.node -or $LASTEXITCODE -ne 0) { throw 'Node version mismatch.' }
if ((& npm.cmd --version) -ne '11.19.0' -or $LASTEXITCODE -ne 0) { throw 'npm version mismatch.' }
$phase6SavedEnvironment = @{}
$phase6Result = [ordered]@{ schemaVersion=1; npmExecuted=$false; npmExitCode=$null; nugetExecuted=$false; nugetExitCode=$null; stageAComplete=$false; failureCategory=$null; npmCiExecuted=$false }
Push-Location -LiteralPath $phase6Root
try {
    foreach ($phase6Setting in $phase6Plan.dotnetChildEnvironment.PSObject.Properties) {
        $phase6SavedEnvironment[$phase6Setting.Name] = [Environment]::GetEnvironmentVariable($phase6Setting.Name, 'Process')
        [Environment]::SetEnvironmentVariable($phase6Setting.Name, [string]$phase6Setting.Value, 'Process')
    }
    if ((& dotnet --version) -ne '9.0.315' -or $LASTEXITCODE -ne 0) { throw 'SDK version mismatch.' }
    Assert-Phase6NoLink $phase6Package
    if ((Get-FileHash -LiteralPath $phase6Package -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.beforePackageSha256) { throw 'Manifest changed during preflight.' }
    if ((Get-FileHash -LiteralPath $phase6Lock -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.beforeLockSha256) { throw 'Lock changed during preflight.' }
    if ((Get-FileHash -LiteralPath $phase6Proposed -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.proposedPackage.sha256) { throw 'Proposed manifest changed during preflight.' }
    [System.IO.File]::WriteAllBytes($phase6Package, [System.IO.File]::ReadAllBytes($phase6Proposed))
    if ((Get-FileHash -LiteralPath $phase6Package -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.proposedPackage.sha256) { throw 'Manifest apply hash mismatch.' }
    $phase6Result.npmExecuted = $true
    $phase6NpmArgs = @($phase6Plan.npmStageAArgs)
    & npm.cmd @phase6NpmArgs 2>&1 | Tee-Object -FilePath $phase6Plan.logs.npm
    $phase6Result.npmExitCode = $LASTEXITCODE
    if ($phase6Result.npmExitCode -ne 0) { throw 'npm lock generation failed; retain output.' }
    $phase6Csproj = Join-Path $phase6Root 'UGS/Modules/FlowStateVerification/FlowStateVerification.csproj'
    Assert-Phase6NoLink $phase6Csproj
    if ((Get-FileHash -LiteralPath $phase6Csproj -Algorithm SHA256).Hash.ToLowerInvariant() -ne $phase6Plan.beforeCsprojSha256) { throw 'NuGet project changed before restore.' }
    $phase6Result.nugetExecuted = $true
    $phase6NugetArgs = @($phase6Plan.nugetStageAArgs)
    & dotnet @phase6NugetArgs 2>&1 | Tee-Object -FilePath $phase6Plan.logs.nuget
    $phase6Result.nugetExitCode = $LASTEXITCODE
    if ($phase6Result.nugetExitCode -ne 0) { throw 'NuGet restore failed; retain output.' }
    $phase6Result.stageAComplete = $true
    Write-Host 'Stage A finished. Report warnings/errors to AI; do not run npm ci until actual lock validation.'
} catch {
    $phase6Result.failureCategory = [string]$_.CategoryInfo.Category
    throw
} finally {
    foreach ($phase6Key in $phase6SavedEnvironment.Keys) { [Environment]::SetEnvironmentVariable($phase6Key, $phase6SavedEnvironment[$phase6Key], 'Process') }
    Pop-Location
    [System.IO.File]::WriteAllText($phase6Plan.logs.result, ($phase6Result | ConvertTo-Json -Depth 5), [System.Text.UTF8Encoding]::new($false))
}
```

사용자 실행에서 sandbox 계정이 만든 후보와 Admin 실행 계정의 소유권 차이로 Git dubious ownership 검사가 중단된 사례를 확인했다. 당시 HEAD/manifest/lock과 cache/log/obj는 바뀌지 않았다. 위 Git 명령 3개는 정확한 후보 경로만 명령 단위 `-c safe.directory=...`로 허용하고, Git 조회 실패와 실제 HEAD 변경 오류를 분리했다. 전역 safe.directory·ACL/소유권·원래 Git 설정은 바꾸지 않는다. 수정된 동일 블록은 [Stage A 실행 파일](20261125_40_Phase3EvidenceStorage.md#archived-evidence)로 제공하며, 기존 `C:\Unity\Step6.ps1` 대신 이 파일을 실행한다. [소유권 오류 후 검증](20261120_26_Phase3Step6OwnershipFixVerification.json)에 실제 미변경 상태와 수정/구문 검증을 기록한다.

수행 후 **Stage A 완료 여부·npm/NuGet 오류 또는 Warning 유무**를 알려준다. 로그/결과는 후보 상위 `Logs/Step6NpmLock_01.log`, `Logs/Step6NuGetRestore_01.log`, `Outputs/Step6StageAResult.json`에 남는다. 실제 생성 JSON/graph는 AI가 같은 파일시스템에서 직접 읽어 대조하므로 내용을 수동 편집/복사하거나 Secret을 전달하지 않는다. 실제 lock 검증 후에만 Stage B npm ci를 안내한다.

### Step 6 npm notice 중단 후 재개 — 2026-11-25

[실제 lock/중단 상태 검증](20261125_02_Phase3Step6NpmInterruptionVerification.json)에 사용자 보고·원래 npm exit null·실제 manifest/lock hash·기존 API의 경로 외 불변 검증과 NuGet 미실행을 기록했다. npm 로그에는 stdout만 남았고 notice 원문 전체는 없으므로 notice의 상세 내용/원래 종료 코드는 확정하지 않는다. 현재 후보의 추적 변경은 두 npm JSON뿐이며 기존 결과/cache/log를 삭제하거나 덮어쓰지 않는다.

사용자는 **아래 NuGet 재개 파일만** 실행한다. npm 명령/manifest 쓰기는 포함하지 않는다. 검증된 현재 npm JSON·csproj·NuGet config와 계획 hash, Git HEAD/staged/추적 변경, 새 NuGet 출력 부재를 확인한 뒤 기존 고정 인자로 restore만 수행한다. native stderr를 문자열로 로그에 보존하고 실제 종료 코드가 0일 때만 완료로 기록한다. 범위를 제한한 Continue 설정은 native 실행 블록 안에서만 사용하며 호출자 오류 설정은 유지한다. 구문 검사와 stderr를 출력하는 별도 PowerShell 프로세스의 exit 0/7 처리 검증은 통과했다. AI는 이 재개 파일이나 restore를 실행하지 않았다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_01_Phase3Step6ResumeNuGet.ps1'
```

새 로그는 `Logs/Step6NuGetResume_01.log`, 결과는 `Outputs/Step6NuGetResume_01.json`이다. 완료 또는 오류/경고를 알려준다. 실패하면 전체 스크립트를 반복하지 않고 생성 결과를 보존해 AI에게 알린다. 실제 NuGet 결과를 대조하기 전에는 npm ci를 실행하지 않는다. **Step 6은 미완료이며 Step 7도 대기한다.**

.NET CLI home·인증서/PATH/telemetry 설정의 범위는 [Microsoft 공식 환경변수 문서](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-environment-variables)로 확인했다. 위 블록은 현재 PowerShell 프로세스에서만 전용 CLI home/HTTP cache와 인증서/PATH/telemetry/workload 변경 비활성을 지정하고 finally에서 원복한다. 사용자/시스템 전역 설정과 기존 cache/인증서는 바꾸지 않는다. 비밀값 없는 전용 npm config와 두 feed의 NuGet config만 사용하며 실제 NuGet graph/feed/resource 일치 여부는 실행 결과로 별도 판정한다. LogSink의 container/image digest·서비스 실행은 이 단계에 필요하지 않아 미검증으로 유지한다.

**6-6 완료(2026-11-25).** [실제 npm ci 검증](20261125_07_Phase3Step6NpmCiActualVerification.json)과 [Step 6 최종 검증](20261125_08_Phase3Step6FinalVerification.json)에 실제 사용자 npm/NuGet 결과·경고 판정·원래/후보 보존을 연결했다. 제품 코드/정책/Test가 바뀌지 않아 Step 3의 340개 근거를 연결하며 같은 suite를 반복 실행하지 않았다. AI는 npm 설치/lock 생성·dotnet restore/build·Unity/Runner·Scene 작업을 수행하지 않았다.

### Step 6 NuGet 기본 복원 확인·Linux 의존성 보완 — 2026-11-25

사용자 NuGet 재개 결과는 exit 0/stageAComplete true다. [실제 기본 복원 검증](20261125_03_Phase3Step6NuGetBaseVerification.json)에서 net9.0 직접 4개/전체 10개 target 내용이 원래 기본 target과 같고, lock/assets/cache metadata contentHash와 archive의 raw SHA-512 sidecar 10개·feed 2개·전용 config/cache·EmbeddedResource 실제 파일 40개를 대조했다. assets 진단 항목은 0개다. 깨진 한글 로그는 원문을 보존하고 구조화된 결과와 별도로 기록했다.

원래 spec의 13개는 기본 net9.0 target 10개와 **net9.0/linux-x64** target을 합친 그래프다. 후보에서 빠진 것은 runtime.any.System.Runtime/4.3.0, runtime.native.System/4.3.0, runtime.unix.System.Private.Uri/4.3.0 세 패키지다. 원래 local obj는 SDK 10.0.301, 후보는 고정 SDK 9.0.315로 생성됐으나 기본 target은 완전히 같다. AI가 준비한 명령에 기존 linux-x64 target 지정이 빠져 있었다. 현재 10개를 전체 13개와 같다고 통과 처리하거나 SDK/spec pin을 바꾸지 않는다.

사용자는 [Linux 보완 복원 파일](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 한 번 실행한다. 기존 계획과 같은 SDK/config/cache를 사용하고 restore에 `--runtime linux-x64`를 추가한다. [공식 dotnet restore 문서](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-restore)에서 runtime 지정 옵션을 확인했다. 현재 lock/assets/dgspec은 후보 상위 `Outputs/Step6BeforeLinux_01`에 정확한 복사본으로 보존했다. 새 파일은 현재 npm JSON·NuGet lock/assets·csproj/config hash와 Git 상태를 검사하며, 사용자 restore만 기존 생성 obj/lock과 전용 cache를 필요한 범위에서 갱신한다. npm 재실행·직접 의존성 추가·SDK 변경·빌드는 포함하지 않는다. dotnet UI 언어만 en-US로 지정하고 finally에서 복구한다. PowerShell AST 검증을 수행했으며 native stderr/exit 처리 블록은 앞서 검증한 재개 파일과 같다. AI는 restore/build/Unity/Runner/Scene 작업을 수행하지 않았다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_04_Phase3Step6RestoreLinux.ps1'
```

완료/오류·경고를 알려준다. 새 로그는 `Logs/Step6NuGetLinux_01.log`, 결과는 `Outputs/Step6NuGetLinux_01.json`이다. 실패 시 반복 실행하거나 출력을 삭제하지 않고 현재 상태를 보존한다. **다음은 Linux 보완 결과의 전체 그래프 검증**이며 기존 Stage A/재개 파일은 반복하지 않는다. 실제 13개 그래프/runtime target/lock을 AI가 대조한 뒤 Stage B npm ci를 안내한다. Step 6은 미완료이고 Step 7도 대기한다.

### Step 6 NuGet 전체 검증 완료·사용자 Stage B — 2026-11-25

[Linux 복원 최종 검증](20261125_05_Phase3Step6NuGetLinuxVerification.json)에서 사용자 NuGet exit 0, 기본 net9.0 및 net9.0/linux-x64 target의 원래 결과와 완전 일치, 전체 13개/직접 4개, lock의 기본 항목+runtime override를 합친 version/contentHash, 전용 cache의 metadata/archive sidecar 13개를 검증했다. feed 2개·전용 config/SDK 9.0.315·EmbeddedResource 40개와 npm lock 경로 이식도 일치한다. NuGet 진단 항목은 0개다. 원래 보호 파일 1,179개와 HEAD/index, 후보 HEAD/staged 상태를 재확인했고 후보 추적 변경은 npm JSON 2개뿐이다. 기존 의도된 문서 4개는 기록 갱신 예외다.

사용자는 [Stage B npm ci 실행 파일](20261125_40_Phase3EvidenceStorage.md#archived-evidence)을 한 번 실행한다. 검증 JSON 자체 hash와 고정 계획·현재 후보 npm/NuGet/SDK 파일·tgz 27개·전용 npm config·Git 상태와 버전을 확인한 후, 같은 격리 cache/config에서 `npm ci --ignore-scripts --no-audit --no-fund`를 수행한다. 기존 node_modules나 실행 로그/결과가 있으면 중단한다. native stderr는 로그에 보존하고 실제 종료 코드로 실패를 판단한다. 설치 전후 npm manifest/lock과 NuGet 입력 hash를 확인한다. NuGet 재복원·빌드·Unity·Runner·Scene 작업은 없다. 구문 검사를 통과했으며 native stderr/exit 블록은 앞서 exit 0/7을 검증한 방식과 같다. AI는 npm ci를 실행하지 않았다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_06_Phase3Step6NpmCi.ps1'
```

완료/오류·경고를 알려준다. 로그는 `Logs/Step6NpmCi_01.log`, 결과는 `Outputs/Step6NpmCi_01.json`에 새로 남는다. 실패하면 출력을 보존하고 반복 실행하지 않는다. 사용자 실행 후 실제 node_modules의 alias/version/의존성/lock 일치와 원래/후보 보존은 AI가 정적으로 대조한다. 원래 npm lock 생성 명령의 exit null은 그대로 보존하며 이번 ci의 실제 결과를 별도로 기록한다. **npm ci 및 후속 검증이 남아 Step 6은 아직 미완료**다.

### Step 6 최종 판정 — 완료(2026-11-25)

사용자 Stage B는 exit 0이며 설치 패키지 57개와 의존성 연결 65개가 실제 lock과 일치한다. 각 package.json의 name/alias/version/dependencies·실제 설치 목록·hidden lock 전체 항목과 격리 cache archive 57개의 SRI를 정적으로 대조했다. npm manifest/lock·NuGet lock/assets/csproj/SDK pin 및 공급 tgz 27개는 Stage B 전후 동일하다. 원래 보호 파일 1,179개와 HEAD/index, 후보 HEAD/staged 상태도 보존했다. 원래 npm lock 생성 중단 때의 exit null은 그대로 두며, 실제 ci exit 0으로 설치 결과를 별도 입증했다.

경고는 `crypto-js@4.1.1`의 유지보수 중단 안내 1건이다. 원래 고정 lock에 같은 deprecated 문구가 있었고 `@unity-services/vivox-0.1`이 정확히 4.1.1을 요구한다. 신규 drift/누락이나 설치 실패가 아니므로 고정 의존성 복원 완료를 막지 않으며 경고를 삭제/억제하거나 버전을 바꾸지 않았다. 패키지 유지보수 상태 개선은 별도 공급 패키지 갱신 검토 범위다. 이번 `--no-audit` 설치를 보안 감사나 런타임/Unity 정상 검증으로 취급하지 않는다.

**Step 6 완료. 추가 수동 작업은 없다.** 앞의 최초 Stage A·오류 재개·Linux 보완·Stage B 절차는 수행 이력이며 다시 실행하지 않는다. 다음은 Step 7의 참조/진단 정적 판정이다. Step 7~10·Phase 3·Roadmap 10 전체는 미완료이며 독립 Unity 복원/참조 정상·권리 성공은 아직 주장하지 않는다.

## Step 7. 직접·이름·동적 참조와 기존 진단의 실제 영향을 정적으로 판정한다

7-1. AI가 원래 자료와 복원 후보의 파일/meta 목록·GUID/중복·subasset/fileID·Importer/platform·Scene/Prefab/Material/Animation/ShaderGraph/asmdef·Resources/Shader 이름과 사용처를 대조한다. YAML만으로 Unity 타입/subasset 해석을 확정하지 않는다.

7-2. 기존 31회 참조 미해결/sample orphan을 raw 항목별로 `사용처/후보 재현/필수 여부/정적 근거/부족한 Unity 해석/필요 Test`에 연결한다. 새 미해결/누락과 기존 진단을 구분한다. allowlist·진단 삭제·필요 파일 제거로 통과시키지 않는다. 비필수 판단도 실제 범위/근거를 남기며 필수인지 모르면 보류한다.

7-3. 동적 로딩·binary·렌더링·코드/리소스 연결을 추적하고, 정적으로 판정 불가능한 구체 대상만 Step 8에 넘긴다. 실제 Scene/Prefab 연결 변경이 필요하면 AI는 읽기 전용 조사와 Hierarchy/컴포넌트/필드·기존→최종값·onClick/Navigation(해당 시)·저장/원복·정적 확인표를 준비하고 사용자에게 적용을 연결한다. 동적 UI 생성/누락 참조 자동 보충은 하지 않는다.

**산출물:** 후보 raw 참조 inventory/원래 대비 차이·필수/비필수 판정·최소 Unity 검증/적용표. **완료:** 정적으로 가능한 조사를 끝내고 필수 미확인과 사용자 검증 범위를 구체화했다. 참조 정상/Phase 3 완료 판정은 필요한 Step 8 결과 후에 한다.

- [x] 실제 참조/Import·기존 진단의 영향과 최소 사용자 검증 대상을 구분했다.

### Step 7 수행 결과 — 완료(2026-11-25)

조사 범위를 7-1~7-5로 나눴다. [원래 raw inventory](20261125_40_Phase3EvidenceStorage.md#archived-evidence)·[후보 raw inventory](20261125_40_Phase3EvidenceStorage.md#archived-evidence)·[참조별 판정](20261125_11_Phase3Step7ReferenceAssessment.json)·[Step 8 인계/사용자 확인 방법](20261125_12_Phase3Step7UnityHandoff.md)·[최종 보존 검증](20261125_13_Phase3Step7FinalVerification.json)을 연결한다.

**7-1 완료 — 직접 참조/파일·meta·Import 비교.** 공통 Assets 파일 632개, 참조 target 실제 파일 278개, 공통 package meta 9,709개가 byte 단위로 같다. 직렬화 closure 56파일/1,299회 참조도 같고 GUID 중복 진단은 0개다. 단순 로컬 fileID→동일 YAML anchor 대조의 미대응도 0개지만 외부 fileID/subasset/타입의 실제 Unity 해석은 미확인으로 남겼다. 전체 meta 수 차이는 private production meta 1개와 package Samples 1,760개·문서 2개·아이콘 14개로 설명된다. 이 package meta로 들어오는 현재 직렬화 참조는 없다. production 설정 C#/meta 제외는 기존 snapshot 계약이며 비밀값 본문을 읽거나 복원하지 않았다.

**7-2 완료 — 기존 미해결 31회 raw 판정.** 원래/후보의 GUID 미해결 31회는 동일하며 새 GUID 미해결은 0개다. 프로젝트/공급 default volume profile의 script 4개씩·PC_Renderer의 probe debug 관련 7개·TMP graph texture 16회를 개별 행/GUID/fileID/소유 객체·사용처·후보 재현·필수 여부·Unity 확인 대상으로 연결했다. 활성 URP/default profile의 항목을 Test 이름만 보고 비필수로 정하지 않았다. TMP graph에는 현재 프로젝트 직렬화 incoming이 없고 사용 폰트 material은 TMP_SDF-Mobile shader를 가리키지만, graph Import 정상은 여전히 확인 대상이다. 두 default profile의 기존 null-script 객체 5개씩도 기록했다.

**7-3 완료 — 기존 sample orphan과 새 후보 진단 분리.** 원래 Test Framework sample orphan 1개의 meta는 후보에 없고 현재 게임 참조가 없다. 후보에는 별도로 내장 module compilation trigger 34개와 빈 Editor 파일 3개의 새 orphan meta가 있다. 대응 원래 파일은 모두 0 byte, meta는 같으며 현재 직렬화 incoming은 0개다. 이 사실을 cache 누락/Unity 영향 없음으로 단정하거나 진단에서 지우지 않고 Step 8 import/컴파일 확인에 넘겼다. raw inventory는 양쪽 모두 수집 완료·exit 1이며 참조 건강 PASS가 아니다.

**7-4 완료 — 이름/동적/binary 경계.** 프로젝트 asmdef 4개의 참조 이름을 실제 설치 package/프로젝트 asmdef와 대조했고 Resources 상대 이름 9개와 Shader 이름을 목록화했다. runtime 지표는 InfinitePatternSlot의 prefab Instantiate 1곳, Editor 1파일과 Test 10파일은 reflection이며 binary 2개는 폰트/스프라이트다. prefab·폰트·texture·material/리소스 연결은 보존됐으나 실제 component/type/subasset/표시 해석은 Step 8 대상으로 남겼다. metadata 비교만을 위해 모든 기존 Unity Test를 재실행하도록 요청하지 않았다.

**7-5 완료 — 최소 Unity 확인/Scene 적용 범위·보존.** 인계 문서에 U1 활성 URP/Volume, U2 TMP graph Import, U3 새 orphan/assembly·Editor 컴파일, U4 font/sprite/subasset/표시, U5 Scene/prefab runtime 연결을 구체화했다. 읽기 전용 Test 준비·부작용 감사와 정확한 사용자 실행은 Step 8에서 수행한다. 지금 Step 7의 추가 수동 작업은 없고 Scene 변경 제안/적용은 0건이다. 원래 보호 파일/생산 자료·이전 metadata·HEAD/index와 후보의 Step 6 입력을 보존했다. 제품 코드/도구/정책/Test는 바꾸지 않아 선행 340개 suite 근거를 반복 실행 결과로 기록하지 않았다. AI는 Unity/Build/Runner·설치·Scene 작업을 수행하지 않았다.

**Step 7 완료.** 완료 범위는 정적 조사와 미확인/사용자 확인 대상 구체화다. Step 8 실제 결과 전에는 참조 정상·Phase 3/Roadmap 10·독립 Unity 복원을 완료로 처리하지 않는다.

## Step 8. 필요한 Unity Test·Editor/Build 결과만 사용자에게 연결한다

8-1. AI가 실제 변경/해석 책임에 맞는 최소 Test를 준비한다. metadata/순수 판정·파일 실패는 Node Unit Test, Unity Import/type/fileID·asset 로딩은 가능한 읽기 전용 EditMode, 실제 Scene/시스템 연결은 관련 PlayMode로 검증한다. 새 Unity Test가 자산/저장/서비스를 수정하지 않는지 정적 감사한다.

8-2. 사용자 안내에 `후보 root/Editor/선행 입력·분리 상태/Test 위치·이름/예상 결과/실패 시 중단·원복/전달할 로그`를 적는다. Build가 필수면 별도 출력 경로·타깃/설정·성공 기준과 저장 분리를 확정하고 사용자가 수행한다. AI는 Unity Editor/Compile/Build/Test Runner를 실행하지 않는다.

8-3. 사용자가 실제 Console/실패 Test 이름·오류/예상 밖 로그·실행 결과를 전달하면 AI가 해당 입력/버전에 연결한다. 영향 코드가 바뀌면 관련 Unit Test/정적 검사를 다시 하고 필요한 사용자 Test만 재요청한다. 과거 Compile/Runner/QA 성공이나 무응답을 새 복원 성공으로 바꾸지 않는다.

**산출물:** 실제 필요한 Unity Test·사용자 적용/실행표와 결과 또는 구체적인 불필요 근거. **완료:** 필수 사용자 실행이 모두 확인됐다. 미실행/실패·필수 참조/타입 미확인이 남으면 Phase 3은 미완료다.

- [x] 필요한 실제 Unity/Scene/빌드 결과를 연결하거나 불필요 근거를 확정했다.

### Step 8 수행 결과/사용자 Stage A — 2026-11-25

작업을 8-1 정적 보완/부작용 감사, 8-2 후보 전용 EditMode 준비/사용자 실행, 8-3 실제 결과 판정, 8-4 필요한 PlayMode/표시 확인, 8-5 보존/최종 완료 판정으로 나눴다. [검사 소스](20261125_14_Phase3Step8ReadOnlyChecks.cs)·[고정 입력](20261125_15_Phase3Step8ReadOnlyInput.json)·[사용자 복사 스크립트](20261125_40_Phase3EvidenceStorage.md#archived-evidence)·[사용자 절차/단계별 확인표](20261125_17_Phase3Step8UserChecks.md)·[준비 검증](20261125_18_Phase3Step8PreparationVerification.json)을 연결한다.

**8-1 정적 준비 완료.** SRP Core의 VolumeProfile.OnEnable이 null component를 메모리 목록에서 제거하는 실제 코드를 확인했다. 원래 YAML과 import 후 component 개수가 다를 수 있으므로 개수 동일을 강제하거나 기존 null-script를 삭제하지 않는다. GameSystem의 PlayMode 격리 분기가 메모리 저장을 선택하고 online coordinator 생성 전 return하는 것과 기존 SetupFixture·Scene Test의 transient 변경/저장 없는 경계를 감사했다. 실제 compile/type/import/표시는 아직 미실행이다.

**8-2 Stage A 사용자 실행 확인(2026-11-25).** 후보 Test 복사·Script Compilation 성공과 EditMode 5개 통과를 보고받았다. 아래 내용은 최초 준비 이력이며 복사 스크립트를 다시 실행하지 않는다. 원래 Assets/기존 Test는 바꾸지 않고 Task 폴더에 후보 전용 `Phase3RestoreReadOnlyTests` 5개를 준비했다. 사용자가 복사 스크립트로 후보의 기존 EditMode asmdef 아래에 C# 1개/meta 1개만 넣는다. 새 meta GUID의 기존 10,039개 meta와 충돌은 없다. 고정 입력은 후보의 기존 파일 1,113개 hash·실제 경로/타입과 개별 31회 GUID/fileID 관측 대상을 담는다. 검사 전후 이 입력을 대조하고 새 진단 JSON만 후보 바깥 Outputs에 생성한다. AssetDatabase 저장/수정·Scene 열기/저장·Instantiate/컴포넌트 추가·서비스/프로세스 실행·저장/SessionState 변경이나 로그 억제를 호출하지 않는다. 자산 OnEnable의 메모리 동작과 실제 Unity import 출력은 별도로 기록한다.

사용자가 아래 최초 Stage A 절차를 수행해 성공했다. 다음은 [Stage B/C 절차](20261125_20_Phase3Step8StageBUserChecks.md)이며 아래 복사/EditMode 실행을 반복하지 않는다. 최초 [Stage A 안내](20261125_17_Phase3Step8UserChecks.md)는 준비 시점 이력으로 보존한다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_16_Phase3Step8PrepareChecks.ps1'
```

복사/컴파일 성공 여부·5개 Test의 통과/실패/미실행 수와 실패 이름/메시지·Console 오류/경고·결과 XML 경로를 알려준다. 진단 JSON은 Outputs/Step8ReadOnly_*에 자동 생성되므로 수동 편집/내용 전달은 필요 없다. 녹색 Test/diagnostic JSON만으로 known GUID·shader/import 경고를 면제하거나 Step 8 성공으로 처리하지 않는다. 진단의 reviewRequired는 실제 Runner XML/Console과 함께 AI가 판정한다. 실패/부분 출력은 보존하고 반복 실행·cache 삭제·임의 버전 변경·Scene 저장을 하지 않는다.

**8-3 Stage A 실제 결과 검토 완료·경고 보존.** 사용자 컴파일 성공/예상 밖 컴파일 Error·Warning 없음과 5개 Test 통과 보고, 실제 JSON 5개의 pre-TearDown Passed/assertionIssues 0을 대조했다. 최종 Runner XML은 없어 사용자 보고와 JSON을 구분하며 XML만을 위해 재실행하지 않는다. 현재 네 TMP graph는 support True/현재 compiler message 없음, URP/Volume·font/atlas/material/sprite·prefab 로딩도 확인됐다. 기존 31회 GUID는 실제 AssetDatabase에서도 계속 경로가 없으며 raw 자산은 변경하지 않았다. 별도의 immutable package 경고 37개는 Step 7의 orphan 대응 파일과 정확히 같다. 이 파일들은 검사 전부터 후보에 없었고 현재도 없으며 원래 대응 파일은 0 byte다. 경고를 새 Test의 쓰기 또는 Unity 버그/무해함으로 단정하지 않고 기록했다.

**8-4 완료 — Stage B/C 사용자 결과 확인(2026-11-25).** 안내한 기존 PlayMode 3개 통과와 메뉴/게임/Pause의 글자·material 및 EmojiOne 미리보기 정상 보고를 받았다. 실제 Test/격리 소스와 고정 입력 1,113개가 그대로이고 Scene/Prefab 파일도 변경되지 않았다. 최종 Runner XML은 없으므로 사용자 보고와 Stage A 진단 JSON을 구분해 기록한다. XML만을 위해 재실행하지 않는다. AI는 Runner/Play/Scene 작업을 수행하지 않았다.

**8-5 완료 — 실제 결과/보존과 판정 범위 구분.** [Stage B/C 실제 검증](20261125_22_Phase3Step8StageBCActualVerification.json)과 [Step 8 최종 검증](20261125_23_Phase3Step8FinalVerification.json)에 사용자 결과·소스/입력·원래 보호 파일 1,179개/HEAD/index·기존 설치/과거 metadata 보존을 연결했다. Step 8의 필요한 Unity 실행/표시 확인은 완료했다. 37개 package 경고의 재발/해소나 원인은 새 보고로 확정되지 않았고 현재도 같은 빈 payload 부재 상태다. 이 경고를 면제/해결로 바꾸지 않으며 최종 package/참조 무결성은 원래 Step 9~10 책임으로 명시적으로 남긴다. Phase 3/Roadmap 10/독립 복원 완료는 아니다.

### Step 8 Stage A 실제 검증/경고 및 Stage B/C 인계 — 2026-11-25

[실제 Stage A 검증](20261125_19_Phase3Step8StageAActualVerification.json)에 사용자 보고·실제 진단 5개·고정 입력 1,113개 불변과 경고 경로 37개 전후 비교를 기록했다. 원래/후보 HEAD/staged·원래 index와 설치된 Test 원문도 대조했다. 경고 원인과 package 무결성의 최종 판정은 미확정이며, 경고 삭제/억제·cache 복사/청소·자동 재설치는 하지 않는다. 현재의 사실은 새 보호 입력 변경 없음과 과거부터 같은 37개 payload 부재다. 실제 경고 발생 메커니즘까지 확인했다고 기록하지 않는다.

[Stage B/C 사용자 안내](20261125_20_Phase3Step8StageBUserChecks.md)에 실제 Scene/UI/runtime를 사용하는 기존 PlayMode 3개, 매 실행 전 격리 예약, 통과 후 최소 화면/이미지 확인, Scene 저장 없음·실패/결과 전달 기준을 준비했다. [후속 준비/보존 검증](20261125_21_Phase3Step8ContinuationVerification.json)에 기존 코드/격리 경로의 동일성과 실제 상태를 연결한다. 새 코드·asset 적용은 없으며 AI는 Unity/빌드/Test Runner·Scene 조작을 수행하지 않았다.

### Step 8 최종 판정 — 완료(2026-11-25)

사용자 Stage A 컴파일/EditMode 5개와 Stage B PlayMode 3개 통과, Stage C 메뉴·게임·Pause의 글자/material 및 EmojiOne 표시 정상 결과를 받았다. 실제 Scene/prefab·원래 Assets/코드·Import/설정은 바꾸지 않았다. 읽기 전용 자산 로딩/타입/localID·폰트/atlas/material/sprite·prefab 및 요청한 runtime/UI 확인을 완료한 범위로 기록한다. 원래 31회 GUID 진단 자체를 Unity가 해소했다고 기록하지 않는다.

현재 production code/자산/설정 byte가 보존됐고 필요한 Editor import/타입/요청 runtime/표시 결과를 받았으므로 이번 복원 차이 확인을 위해 새 build를 필수로 추가하지 않았다. build/Player·모든 shader variant/플랫폼을 검증했다는 뜻은 아니며 필요한 범위가 후속 전체 검사에서 달라지면 별도 사용자 실행 계획으로 연결한다.

기존 37개 경고는 Step 7부터 같은 0-byte package 파일 부재와 관련된 기록으로 남긴다. 새 Test 쓰기 증거가 없고 보호 파일 변경도 없지만, 경고 원인/해결이나 broad package 무결성 PASS는 미확정이다. Stage B/C의 추가 로그 유무는 별도 보고되지 않았으므로 경고가 사라졌다고 추정하지 않는다. 현재 사용처의 실행/표시 확인과 최종 package 무결성은 구분하며 Step 9에 이 미확정 사항을 넘긴다.

**Step 8 완료. 현재 추가 수동 작업은 없다.** 앞의 Stage A/B/C 실행표는 수행 이력이며 반복 실행하지 않는다. 다음은 Step 9의 후보 전체성/Git·원래 자료 보존/원복 검사다. AI는 Unity/Build/Runner·Scene 적용·package/cache 조작을 하지 않았다. Step 9~10·Phase 3·Roadmap 10 전체는 미완료다.

## Step 9. 후보 Git/복원 전체성과 원래 자료 보존·원복 가능성을 최종 검증한다

9-1. 실제 후보의 정책/등록·HEAD/index/staged·ignore와 알려진 원본 지문, 프로젝트 소유 자료 보존을 AI가 검사한다. registry/audit/ingress/lifecycle의 실제 `--root`를 사용하고 입력 pin을 해당 root에서 대조한다. 현재 worktree 정책 검사와 staged 정책 재현을 구분한다.

9-2. 전체 필수 의존성/참조·실제 설치/사용자 결과·보존본/설정 hash·원래 작업본 보호 계약을 함께 대조한다. registry/유입 gate/자료·권리/참조 건강/Unity/독립 복원은 독립 결과로 기록한다. 필수 누락/실행 실패가 있으면 다른 gate의 exit 0으로 전체 성공시키지 않는다.

9-3. 예상 실패/원복은 우선 임시 fixture Unit Test로 입증한다. 실제 실패가 있으면 이름 붙인 before/attempt/current hash와 검증된 보존본으로 필요한 목록/순서만 준비하고, 다른/늦은 변경을 덮어쓰지 않는다. 새/부분 출력·실패 로그를 유지하며 자동 삭제·전체 reset/clean·cache 청소를 하지 않는다. Scene/Prefab 원복은 사용자 담당이다.

**산출물:** 실제 후보/원래 작업본 전후 검사·실제 결과 집계·부분 원복 증거/한계. **완료:** 필수 영역이 모두 검증되고 원래 자료 손실/예상 밖 변경이 없다.

- [x] 후보 전체 검증·원본 비추적/자체 자료 보존·원래 작업본/원복 근거를 확보했다.

### Step 9 수행 결과/정확한 공식 파일 복원 대기 — 2026-11-25

작업을 9-1 정책/유입/Git, 9-2 snapshot/의존성/원래 자료 전체성, 9-3 임시 실패/원복 fixture, 9-4 확인된 공식 package 파일 차이 복원, 9-5 후속 전체 대조/완료 판정으로 나눴다.

**9-1 정적 검증 완료.** [후보 gate 기록](20261125_40_Phase3EvidenceStorage.md#archived-evidence)과 [정확한 evidence로 수행한 ingress 검사](20261125_40_Phase3EvidenceStorage.md#archived-evidence)에 실제 후보 --root를 명시했다. registry/lifecycle exit 0이며 등록/실제 lifecycle 요청은 0개다. 최초 ingress 호출의 필수 --evidence 누락 exit 2는 지우지 않고 보존했다. 기존 frozen KnownOrigins/evidence를 그대로 지정한 managed/staged 재실행은 exit 0이고 알려진 원본 지문은 110개다. HEAD/index mode/OID/path 980개는 같고 정상 flag H, staged/remote는 0개다. 실제 현재 추적 worktree byte도 별도로 evaluate해 원본 지문 침입이 없음을 확인했으며, 이를 다른 staged 정책/commit hook 재현이라고 기록하지 않는다. 111개 private 입력은 ignored/untracked다.

**9-2 정적 대조 완료·package 차이 37건 보류.** [snapshot 전체성/보존/원복 입력](20261125_26_Phase3Step9FidelityAndRollbackInputs.json)에서 최초 1,091개와 현재 검토 1,177개를 정확히 비교했다. npm JSON 2개 변경·TMP 82개 추가와 .vsconfig/NuGet lock/새 Test 2개 추가 등 승인된 88개 작업 외 차이는 없다. frozen 문서·109개 기존 directory·TMP schema 2 source/candidate 보존본과 실제 자산/폴더도 유지됐다. Library/node_modules/obj 등 생성 영역은 이 비교 밖으로 구분하고 각각 다른 검사로 확인했다. [실제 의존성 검사](20261125_27_Phase3Step9DependencyVerification.json)에서 npm 57개/연결 65개·SRI 검증 archive의 파일 4,649개가 설치 파일과 byte 단위로 같고 NuGet 기본/Linux 13개·cache/lock도 검증했다. Step 8 사용자 컴파일/5 EditMode/3 PlayMode 및 표시 보고는 보존하되 XML 미제공과 모든 build/variant 미검증 한계를 유지한다. raw audit는 31회 기존 미해결+37개 orphan을 보존해 exit 1이며 참조 건강 PASS가 아니다.

**9-3 완료 — 실제 원복 없이 fixture/보존 입력 검증.** [snapshot fixture 79개](20261125_28_Phase3Step9RollbackFixtureVerification.json)를 재실행해 late user change·잘못된 backup 거절, 새/부분 출력 유지, 입력별 사용자 결과와 실제 원복 없는 계획을 검증했다. 전체 340개 suite를 반복하지 않았다. 실제 npm 두 JSON의 before/attempt/current와 Outputs/Step6Before의 byte를 비교한 rollbackPlan은 검토 가능한 정확한 두 파일 계획이며 쓰기는 0건이다. 지금 그 두 파일을 되돌릴 실패는 없으므로 실행하지 않는다. Scene/Prefab을 되돌리거나 새 로그·부분 출력을 삭제하지 않는다.

**9-4 완료 — 사용자 공식 파일 복원/컴파일 확인(2026-11-25).** 사용자가 정확한 공식 파일 37개를 복원했고 후보의 컴파일 성공·예상 밖 컴파일 Error/Warning 없음 보고를 받았다. 실제 byte/meta/package identity를 사후 검증했다. 다음은 최초 준비 이력이며 복원 스크립트를 다시 실행하지 않는다. 고정 Editor 6000.3.5f2 설치의 BuiltInPackages와 대조해 경고 37개 파일이 공식 공급본에는 0 byte로 존재하고 후보에는 없음을 확인했다. package ID/version과 source/target meta hash/GUID도 전부 같다. 다른 사용자 계정의 읽기 전용 조회에서도 37개 부재가 같았다. 앞선 상태를 Unity 버그/무해함으로 단정하지 않으며 이번 새 근거로 실제 파일 부재를 해소할 필요가 있다. [공식 파일 복원 계획](20261125_29_Phase3Step9StockFileRestorePlan.json)·[사용자 스크립트](20261125_40_Phase3EvidenceStorage.md#archived-evidence)·[사용자 절차](20261125_31_Phase3Step9StockFileRestoreUserSteps.md)·[정적 준비 검증](20261125_32_Phase3Step9PreparationVerification.json)을 마련했다. 설치된 같은 Editor의 정확한 37개 파일만 후보 Library/PackageCache 안의 현재 없는 target으로 overwrite=false 복원한다. 임의 payload/GUID 생성·원래 Library 복사·버전 변경·전체 cache 삭제가 아니다. 기존 meta/자산/코드/설정은 그대로다. AI는 이 쓰기를 수행하지 않았다.

아래 최초 복원/Editor 확인은 사용자가 수행했다. 현재 추가 빌드·Play·Test Runner·Scene 저장이나 스크립트 재실행은 필요 없다. 최초 사용자 절차와 실행 파일은 before/attempt/current 근거로 보존한다.

```powershell
& 'C:\Unity\Unity_Flow_State\AI\90_Tasks\Prototype_10\20261125_30_Phase3Step9RestoreStockFiles.ps1'
```

**9-5 완료 — 사후 gate/원래 자료 보존 검증.** [복원 후 gate](20261125_40_Phase3EvidenceStorage.md#archived-evidence)·[실제 복원 검증](20261125_34_Phase3Step9ActualRepairVerification.json)·[Step 9 최종 검증](20261125_35_Phase3Step9FinalVerification.json)에 사용자 결과와 사후 전체성을 연결했다. 공식 37개 파일은 byte/meta/package identity가 모두 같고 orphan meta 진단은 37→0개다. 기존 입력 1,113개·원래 보호 파일 1,179개와 QA/HEAD/index·기존 설치/사용자 결과를 보존했으며 1,091→1,177개 frozen project 비교도 계속 일치한다. registry/managed·staged ingress/lifecycle은 exit 0, audit는 기존 31회 GUID finding만 유지해 수집 완료·exit 1이다. 원래 참조 건강/권리/전체 독립 복원 PASS로 바꾸지 않고 Step 10 최종 판정에 넘긴다.

### Step 9 최종 판정 — 완료(2026-11-25)

사용자 복원 결과는 created 37개/copyComplete/protectedInputsPreserved true이며, 실제 파일·기존 meta·공급 package ID/version/hash가 검증된 공식 공급본과 같다. 보호 입력/원래 자료·Scene/Prefab·이전 snapshot과 gate/설치/원복 fixture 근거가 보존됐다. 실제 되돌릴 실패나 늦은 변경은 없고 원복/삭제는 수행하지 않았다. source index와 후보 HEAD/index/staged 경계도 유지했다.

사용자는 복원 뒤 고정 후보 Editor의 Script Compilation 성공과 예상 밖 컴파일 Error/Warning 없음을 보고했다. 원래 immutable package 경고의 발생 메커니즘을 모두 입증하거나 별도 전체 Editor 로그에서 경고 부재를 직접 확인했다고 주장하지 않는다. 정적으로 확인한 것은 공식 파일 부재 해소와 orphan meta 37개 진단 소멸이다. 기존 31회 GUID 진단·최종 Runner XML 미제공·build/Player/all-variant 미검증 한계는 그대로 남긴다.

**Step 9 완료. 추가 수동 작업은 없다.** 필요한 사후 검증을 마쳤으므로 복원/Unity 검사를 반복하지 않는다. 다음은 Step 10의 세 완료 조건/남은 한계·Phase 3/Roadmap 10 최종 판정과 후속 인계다. Step 10/Phase 3/Roadmap 10 전체는 아직 완료 처리하지 않았다. AI는 Unity/빌드/Runner·Scene 또는 package/cache 복원을 수행하지 않았다.

## Step 10. Phase 3·Roadmap 10 완료를 판정하고 후속 도입에 인계한다

10-1. 위 Roadmap 세 완료 조건에 실제 후보 root/입력·필수 참조·사용자 Unity/설치 결과·원본 비추적/자체 자료/원래 작업본 보존·제약을 연결한다. 준비 도구/합성 Test만으로 독립 복원 완료를 선언하지 않는다.

10-2. 모두 충족한 경우에만 이 Task·Roadmap 10/공통 안내·Project Memory·관리 기록/영구 절차를 실제 결과로 갱신한다. 미충족이면 필수 남은 작업/담당·정확한 차단 이유와 보존 상태를 기록하고 완료를 보류한다. 신규 제품 0개라는 사실로 기존 기술 의존성의 필수 복원을 면제하지 않는다.

10-3. 이후 도입자가 사용할 등록→조건 확인→원본 비추적 보관→필수 파일 도입→확장/게임 사용처 기록→Git/참조 검사→업데이트/제거/원복→독립 복원 절차와 실제 버전/source/hash·취득/오프라인·수동 단계를 인계한다. Roadmap 11~14 실제 원본 도입은 Roadmap 10 전체 완료 뒤에 진행한다. 다음 실제 Module 업데이트 전 복구 시험 의무도 유지한다.

**산출물:** 세 조건 최종 판정·Task/영구 문서·후속 도입 점검표/남은 의무. **완료:** 실제 결과와 문서 상태가 일치하고 필수 실패/미실행·원래 자료 손실을 성공으로 표시하지 않았다.

- [x] 실제 증거로 Phase 3/Roadmap 10 완료 여부와 후속 도입 인계를 판정했다.

### Step 10 수행 결과 — 완료(2026-11-25)

**10-1 완료 — 세 조건 대조.** [실측 평가](20261125_36_Phase3Step10Assessment.json)에 현재 후보 root/고정 입력·실제 설치/사용자 Unity 결과·필수 참조·비추적/자체 자료 보존과 한계를 연결했다. registry/managed·staged ingress/lifecycle을 현재 후보에서 재검사해 모두 exit 0이다. audit는 수집 완료·기존 GUID 미해결 31회만 유지한 exit 1이며 orphan meta는 0개다. Step 8 실제 타입/Prefab/profile/TMP/graph 로딩과 사용자 컴파일·EditMode 5개·PlayMode 3개·표시 보고를 근거로 현재 필수 사용 범위를 판정했다. Volume legacy 8회·obsolete probe debug 7회·현재 incoming 사용 없는 TMP graph 16회는 원래 자료/실제 해석과 사용 범위를 함께 검토해 현재 복원 비차단으로 구분한다. 삭제/무시로 참조 건강 PASS를 만들지 않으며 해당 효과/graph를 후속 필수로 사용하면 재검증한다.

**10-2 완료 — 현재 기술 기준선의 Phase 3·Roadmap 10 완료.** UPM/TMP·npm/NuGet을 실제 별도 환경에 준비했고 필수 참조/사용자 결과와 원본 비추적·프로젝트 소유 파일 보존·제약 기록의 세 조건을 충족했다. 신규 제품 0개로 기술 복원을 면제하지 않았다. 원래 HEAD/index·QA/생산 입력과 후보 보호 입력은 그대로다. 이번 갱신 문서 6개를 제외한 원래 보호 파일 1,177개와 후보 보호 입력 1,113개, 승인된 frozen project 입력 1,177개를 재대조했다. Step 9의 1,179개와 차이는 갱신 문서 2개 추가이며 자료 손실이 아니다. Task·Roadmap/공통 안내·Memory·관리 기록·도구 안내에 같은 현재 판정을 연결했다. 현재 기술 기준선의 복원 완료와 모든 기존 자산의 결함 제거/제품별 권리/모든 Player·platform·variant 검증은 구분한다.

**10-3 완료 — 후속 도입 인계.** [인계 문서](20261125_37_Phase3RestoreAndAdoptionHandoff.md)에 실제 source/version/hash·취득/보존/오프라인 제약·수행한 수동 단계와 등록→조건→원본 비추적 보관→필수 도입→확장/사용처→Git/참조→업데이트/제거/원복→별도 복원 점검표를 연결했다. 후보는 미커밋 입력을 포함한 명시적 snapshot이며 remote clone/clean Git checkout만으로 재현되지 않는다. 원래 선언/lock/spec과 후보의 경로 이식된 두 JSON을 구분하고 자동 원본 반영은 하지 않았다. 제품 권리/구매/재배포·새 기능/원격 실행을 승인하지 않으며 다음 실제 Module 업데이트 전 복구 시험 의무를 유지한다.

**추가 수동 작업 없음.** 이번은 증거 판정·문서 인계이며 Unity/빌드/Runner·Scene/Prefab 적용이나 이전 복원 스크립트 재실행이 필요 없다. 최종 Runner XML 미제공, pre-TearDown JSON과 사용자 최종 성공 보고의 구분, 과거 immutable 경고의 전체 로그/발생 메커니즘 미입증, crypto-js 유지보수 경고·오프라인/Player/all-variant·원격/container 미검증 한계를 보존한다. AI는 package/cache/자산을 쓰거나 Unity/빌드/Runner를 실행하지 않았다.

# 영향 범위

Step 10은 기존 Task/Memory/Roadmap 10/공통 안내/관리 기록/도구 안내의 문서 6개와 평가·인계·최종 검증 3개만 추가/갱신했다. 생산 코드/자산·Scene/Prefab·선언/lock·원래 Git/보존본/cache·이전 metadata는 보존했다. 다음은 이전 Step 6의 허용 범위 기록이다.

Step 6 준비의 허용 변경은 이 Task·Project Memory·Roadmap 10·로드맵 안내와 입력 감사/복원 계획/제안 package.json/전용 NuGet config/준비 검증 metadata 5개·Outputs sidecar/검증된 backup·비밀값 없는 전용 npm config와 임시 help CLI home이다. 후보 package.json/lock·csproj·SDK pin·자산·Git·기존 cache는 수정하지 않았다. 실제 사용자 Stage A에서만 계획된 후보 package.json과 생성 lock/NuGet obj·packages.lock 및 별도 cache/log가 바뀐다. 사용자 Stage B ci는 실제 lock 검증 후 안내한다. 원래 생산 자료/선언/lock·보존본/archive·저장/Secret·HEAD/index와 이전 metadata를 보존하며 자동 reset/clean·cache 삭제·버전 대체는 하지 않는다.

# 검증 내용

- 일반 Task 필수 13개 섹션·Step 1~10 순서와 각 산출물/완료 조건·담당/중단·보존 계약을 확인한다.
- Roadmap 세 조건과 실제 후보 준비·정적 검사/Unit Test·필수 사용자 실행/참조/원복·최종 판정의 연결을 확인한다.
- 현재 존재하는 CLI/옵션과 미구현 제안을 구분하고, 실제 source/spec/경로와 상대 링크·날짜/번호·UTF-8·공백을 확인한다.
- 문서 작성 전후 Phase 2 기준선·생산/도구·이전 metadata·HEAD/index 보존을 확인하고 의도된 연결 문서 변경만 허용한다.

# 검증 결과

Step 1~9의 기존 metadata/실행·실패/재개·보존 근거는 유지했다. Step 10은 [평가](20261125_36_Phase3Step10Assessment.json)·[인계](20261125_37_Phase3RestoreAndAdoptionHandoff.md)·[최종 검증](20261125_38_Phase3Step10FinalVerification.json)에 현재 gate/입력/원래 자료·문서 연결과 세 완료 조건 판정을 기록한다. 코드/정책/자산은 변경하지 않았고 기존 Node 340개 또는 snapshot 79개를 이번에 새로 실행한 결과로 중복 기록하지 않는다. 사용자 최종 Runner 성공 보고와 실제 진단 JSON을 구분한다. raw audit exit 1/기존 31회는 유지하며 현재 필수 참조 복원 범위의 완료로 판정한다.

# 후속 작업

**Step 10·Phase 3·Roadmap 10 완료(2026-11-25). 다음 순서는 Roadmap 11이다.** 실제 후속 기능/제품 도입은 별도 작업에서 인계 점검표를 적용한다. 현재 추가 수동 작업은 없으며 기존 스크립트/Unity 검사를 반복하지 않는다. 기존 31회 진단 자료의 필수 사용·새 버전/사용처는 새로 검증하고 다음 실제 Module 업데이트 전 복구 시험 의무를 유지한다. Temp의 보존본/archive/출력 유실 시 Git만으로 재현할 수 없으며 원래 선언/lock은 자동 변경하지 않는다.

# 관련 문서

- [AI 진입점](../../README.md)
- [Project 안내](../../00_Project/README.md)
- [Project Memory](../../00_Project/PROJECT_MEMORY.md)
- [관리 기록](../../00_Project/ASSET_DEPENDENCIES.md)
- [등록 형식](../../99_Templates/ASSET_DEPENDENCY_TEMPLATE.md)
- [Roadmap 10](../../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_010.md)
- [공통 진행/검증 경계](../../04_Implementation_Roadmap/README.md)
- [AI Rule](../../01_Rules/AI_RULE.md)
- [구현 규칙](../../01_Rules/IMPLEMENTATION_RULE.md)
- [검증 규칙](../../01_Rules/VERIFICATION_RULE.md)
- [일반 Task 형식](../../99_Templates/GENERAL_TASK_TEMPLATE.md)
- [실제 도구/명령](../../../Tools/DependencyAudit/README.md)

# 관련 작업 기록

- [Phase 2 최종 Task](20261119_05_Phase2ManualSteps.md)
- [Phase 2 전체 Unit Test/현재 검사](20261119_26_Phase2Step8Check.json)
- [필요 사용자 작업/미실행 경계](20261119_28_Phase2Step9Check.json)
- [Phase 3 인계](20261120_02_Phase3Handoff.md)
- [Phase 2 최종 검사/안전 전달 기준선](20261120_03_Phase2Step10Verification.json)
- [Roadmap 9 QA/운영·복구 의무](../Prototype_9/20261118_01_Phase4ManualSteps.md)

# 작성 완료 기준

실제 작업 순서·기존 도구/추가 구현·정적/Unit Test·필수 사용자 작업과 산출물/완료/중단/보존 조건을 Step으로 표현했다. 문서 작성 완료와 실제 Step·Phase 3/독립 복원·Roadmap 전체 완료를 구분한다.
