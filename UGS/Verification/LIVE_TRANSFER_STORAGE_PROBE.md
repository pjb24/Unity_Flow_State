# Step 12 — 실제 Cloud Save 저장·이전 복구 자동 검사

## 무엇을 확인하는가

서버 저장 중 한 요청이 실패하거나 저장 성공 응답을 받지 못해도, 기존 이전 코드를 다시 사용해 하나의 정상 연결로 복구되는지 확인한다. 두 대상이 동시에 이전을 완료하려고 할 때도 승자가 하나인지 검사한다. 서버 저장 코드와 이전 서비스 코드는 기존 `UGS/CloudCode` 구현을 그대로 호출한다.

이 도구는 Node에서 실행하는 **관리자 저장 검사**다. 게시된 C# Module/Anonymous 인증/Jint/서버의 전체 5초 budget 검사가 아니다. 합성 Player ID와 전용 번호는 격리 자료 안에서만 사용되며 실제 인증 성공이나 실제 공개 번호 발급으로 주장하지 않는다. Step 10~11의 실제 Module/사용자 검증과 근거를 구분한다.

## 준비된 안전 경계

- Project와 verification Environment를 코드에 고정하고 두 ID를 지정해 토큰을 교환한다.
- 새 실행마다 무작위 `p2v-<32자리 실행 ID>-<12자리 해시>` Private Game Data만 만든다. 기존 `fs8-*` 주소는 전용 주소로 변환되고, 해당 전용 주소에 이미 자료가 있으면 중단한다.
- A/B의 실제 Authentication·Local Save·계정·번호 발급기·Leaderboard를 변경하지 않는다.
- 실제 HMAC Secret 대신 실행 중에만 존재하는 임시 검증용 키를 사용한다.
- HTTP 요청마다 5초 제한을 사용한다. 실패한 HTTP 요청을 자동 재전송하지 않는다. 여러 사례를 검사하는 도구의 전체 실행 시간은 5초보다 길며, 복구 사례 사이에는 인증 시도 간격 5초를 지키도록 5.1초 기다린다.
- 종료 시 자료를 자동 삭제하지 않는다. 실패/timeout도 실제 저장이 반영됐을 수 있으므로 실행 ID를 보관한다. 성공/실패/실행 ID만 출력하며 키·토큰·코드·인증값·HMAC·원문 응답은 출력하지 않는다.
- 기존 Module을 수정하거나 빌드/재게시할 필요가 없다. C#/Scene도 이번 도구 준비에서 변경하지 않았다.

## 사용자가 할 작업

### 2026-10-06 실행 실패 후 — 남은 검사만 수행한다

**받은 결과:** 실제 batch/CAS·정상 이전, `BEFORE_1`~`BEFORE_10`, `AFTER_1`~`AFTER_9`는 PASS다. 다음 출력이 `REQUEST_UNCONFIRMED / HTTP=0`이므로 응답을 확인하지 못한 요청이 있었다. 기존 출력만으로 `AFTER_10` 준비/주입/복구/검증 중 어느 단계인지나 Timeout·네트워크 원인을 확정할 수 없다. PowerShell의 30줄 throw는 이 실패를 전달한 종료 처리다.

**목적:** 기존 19개 복구 PASS를 유지하고 미확인 `AFTER_10`, 두 대상 동시 이전, 제출 예약/이전 시작 경합만 확인한다.

1. 기존 결과는 `Ignore\Step12\live-transfer-storage-probe-result.txt`에 보관한다. 이전 실행의 `p2v-8ff93ebaaed15bf42f7043915b7365b4-*` 자료는 분석 근거로 보관한다.
2. 아래 명령을 실행한다. 이전 실행 공간을 재사용하는 복구가 아니라 **새 전용 공간에서 남은 3사례만 실행**하는 옵션이다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\Unity\Unity_Flow_State\UGS\Verification\Invoke-LiveTransferStorageProbe.ps1" -Run -Remaining
```

3. `VERIFY`를 입력하고 이번 임시 서비스 계정 Key ID·Secret Key를 숨김 질문에 각각 붙여넣는다. 이미 키/역할을 회수했다면 아래 지정 값으로 같은 용도의 임시 키/역할을 다시 준비한다.
4. 끝나면 RUN/PASS/FAIL 줄을 전달한다. 아래 3개의 개별 PASS와 마지막 결과가 기대값이다.

```text
PASS / STORAGE_RECOVERY_AFTER_10
PASS / STORAGE_SINGLE_WINNER_RACE
PASS / STORAGE_SUBMISSION_TRANSFER_FENCE / NoLeaderboardWrite=True
PASS / LIVE_STORAGE_REMAINDER / FaultCases=1 / RaceCases=2 / FullRun=False / ModuleAuthAndRuntime=NOT_TESTED / ExistingAccountsUntouched=True
```

5. 추가 결과는 `C:\Unity\Unity_Flow_State\Ignore\Step12\live-transfer-storage-probe-remaining-result.txt`에 보관하고 이번 키/역할을 회수한다.

`FullRun=False`는 이번 실행이 남은 사례만 검사했다는 뜻이다. 앞선 19 PASS와 합쳐 판정하며 단일 실행에서 전체 20사례가 통과했다고 쓰지 않는다. 이번에도 FAIL이면 표시되는 `Case / Phase / Operation / RequestOrdinal / NetworkFault`까지 전달한다. 원문 요청·키·토큰은 필요 없다. 요청 제한은 5초이며 자동 HTTP 재전송은 추가하지 않았다.

### 사용할 값 — 아래 값으로 통일한다

**사용자가 이름을 정하는 항목은 아래 값을 그대로 입력한다.** Unity가 생성하는 Key ID·Secret Key는 생성된 실제 값을 복사한다.

| 항목 | 지정 값 | 사용하는 곳 |
| --- | --- | --- |
| 프로젝트 폴더 | `C:\Unity\Unity_Flow_State` | PowerShell을 여는 폴더 |
| 프로젝트 이름 | `Unity_Flow_State` | Dashboard의 Add project role에서 선택. 로컬 ProjectSettings 기준 이름 |
| Project ID | `c76d55cf-7846-494b-9dce-a0797b179b36` | Dashboard에서 프로젝트 대조. 도구에는 이미 고정돼 있음 |
| 조직 식별자 | `pjb24` | 로컬 ProjectSettings 기준. 이 프로젝트를 소유한 조직 선택용이며 조직 표시 이름이 다르면 Project ID로 프로젝트를 확인 |
| 검사 환경 이름 | `verification` | 이번 검사가 사용하는 환경 |
| Environment ID | `a20a46fa-1edb-4d79-9c35-02f2fed31896` | 도구의 토큰 교환 대상. 이미 고정돼 있음 |
| Service Account 이름 / Name | `fs-phase2-storage-probe` | Create service account의 이름 입력칸 |
| Service Account 설명 / Description | `Temporary verification-only Phase 2 transfer storage probe. Revoke key and project role after the run.` | 설명 입력칸이 있으면 입력 |
| 역할 추가 종류 | `Add project role` | 계정 상세의 역할 추가 버튼 |
| 프로젝트 역할 / Role | `Cloud Save Editor` | 위 프로젝트에 부여하는 역할 1개 |
| 조직 역할 | 추가할 항목 없음 | 이번 작업은 프로젝트 역할 1개만 사용 |
| Key 표시 이름 / 설명 | `fs-phase2-storage-probe-key` | Create key에 이름/설명 입력칸이 있을 때 사용. 입력칸 없이 바로 생성되면 생성 결과를 사용 |
| 생성할 Key 수 | `1` | 이 임시 Service Account에 새 키 하나 생성 |
| Key ID | 방금 Unity가 생성한 Key의 **Key ID 원문** | 첫 번째 숨김 입력. 서비스 계정 자체의 ID와 구분 |
| Secret Key | 위 **같은 Key**의 Secret Key 원문 | 두 번째 숨김 입력. 기존 HMAC Secret과 구분 |
| 실행 승인 문자열 | `VERIFY` | 첫 질문에 대문자로 입력 |
| 실행 도구 | `C:\Unity\Unity_Flow_State\UGS\Verification\Invoke-LiveTransferStorageProbe.ps1` | 아래 명령의 File 값 |
| 실행 옵션 | `-Run` | 원격 검사 실행 모드 |
| 이미 19사례 통과한 후속 실행 옵션 | `-Run -Remaining` | 마지막 복구 1사례와 경합 2사례만 새 공간에서 실행 |
| 결과 보관 파일 | `C:\Unity\Unity_Flow_State\Ignore\Step12\live-transfer-storage-probe-result.txt` | RUN/PASS/FAIL 결과만 보관 |
| 권한·키 사용 기간 | 이번 검사 1회 실행이 끝날 때까지 | 성공/실패 확인 후 키 폐기·역할 회수 |

`Key ID`와 `Secret Key`는 원하는 문자열로 지정할 수 있는 값이 아니다. 위의 계정 이름이나 Key 표시 이름을 이 두 입력칸에 넣는 것이 아니라, **Create key 결과로 나온 두 실제 값**을 넣는다. 이름/설명은 사용자가 식별하기 위한 표기이며 도구가 계정을 찾는 데 사용하지 않는다.

아래 값은 도구가 자동 처리한다. Dashboard에서 직접 만들거나 편집할 입력값이 아니다.

| 자동 항목 | 값 / 생성 방식 |
| --- | --- |
| 토큰 교환 Project / Environment | 위의 고정된 Project ID / Environment ID |
| Access Token | 두 관리자 키로 Unity Token Exchange API에서 자동 발급 |
| HTTP 요청 제한 | `5000 ms` |
| 복구 단계 사이 간격 | `5100 ms` |
| 합성 Player 이름 | `probeA`, `probeB`, `probeC` — 실제 A/B 로그인 계정과 별개 |
| 합성 공개 번호 | `0000000001`, `0000000002`, `0000000003` — 전용 검증 저장 공간에서만 사용 |
| 검증 실행 ID | 도구가 생성한 무작위 32자리 소문자 16진 문자열 |
| 저장 주소 | `p2v-<실행 ID>-<12자리 해시>` — 최종 주소 길이 49자 |
| 저장 위치 | `verification → Cloud Save → Game Data → Private` |
| 저장 키 | `fs_account_v1`, `fs_creation_guard_v1`. batch 검사 전용 `probe_uncommitted`는 충돌로 전체 거부되어 최종 저장에 남지 않아야 함 |
| 논리 계정·고정 owner·이전 ID | 도구가 생성한 UUID. 전용 저장 주소 안에서만 사용 |
| 검증용 HMAC 키 | 실행 중 메모리에서 새로 생성. Dashboard Secret 생성 작업 없음 |

현재 PC에서 Node `v24.21.0`, 실행 경로 `C:\Program Files\nodejs\node.exe`를 정적으로 확인했다. 아래 실행 명령은 PATH의 `node`를 사용한다.

### 1. 임시 서비스 계정을 준비한다

목적: 도구가 verification의 Private Game Data를 읽고 쓸 수 있도록 허용한다.

1. Unity Dashboard에서 **Account 메뉴 → Manage organization**을 열고 `Unity_Flow_State` 프로젝트를 소유한 조직을 선택한다.
2. **Administration → Service Accounts → Create service account**를 연다.
3. **Name**에 `fs-phase2-storage-probe`를 입력한다. 설명칸이 있으면 위 표의 영어 Description을 입력하고 생성한다.
4. 생성한 `fs-phase2-storage-probe`의 상세 화면에서 **Add project role**을 누른다.
5. 프로젝트는 `Unity_Flow_State`, 역할은 `Cloud Save Editor`를 선택하고 추가를 확정한다. Project ID가 위 표와 같은 프로젝트를 선택한다.
6. 같은 계정의 **Keys → Create key**를 누른다. 이름/설명칸이 있으면 `fs-phase2-storage-probe-key`를 입력한다. Key 하나를 생성한다.
7. 생성 결과의 **Key ID**와 **Secret Key**를 각각 복사해 본인만 보는 임시 장소에 보관한다. 아래 터미널에서 두 번 나눠 붙여넣을 값이다. Secret Key가 한 번만 표시되는 화면이면 그 화면에서 먼저 복사한다.

**준비 완료 상태:** Service Account 이름은 `fs-phase2-storage-probe`, 프로젝트 역할은 `Unity_Flow_State / Cloud Save Editor`, 이번에 만든 Key는 하나이며 그 Key ID·Secret Key 두 값을 본인이 가지고 있다.

Cloud Save Editor는 **프로젝트 수준 쓰기 권한**이다. 계정 이름/설명에 verification을 적는 것만으로 권한이 그 환경에 제한되는 것은 아니다. 실제 요청은 도구가 고정한 verification ID로 교환한 토큰과 전용 저장 주소를 사용한다.

역할/키 생성 권한이 없거나 Dashboard에 해당 역할이 없으면 현재 보이는 메뉴/오류만 알려 준다. 키는 채팅에 보내지 않는다.

근거: [Service Account 인증·환경 지정 토큰 교환](https://docs.unity.com/en-us/services-web-apis/service-account-auth), [Cloud Save Editor 역할](https://docs.unity.com/en-us/oas-cloud-save-admin/1.0.0).

### 2. PowerShell에서 검사를 실행한다

목적: 타이밍을 맞춰 버튼을 누르는 대신 도구가 전체 순서를 자동 검사한다.

1. Windows 파일 탐색기에서 `C:\Unity\Unity_Flow_State`를 연다. 주소 표시줄에 `powershell`을 입력하고 Enter를 누른다.
2. 다음 명령을 실행한다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\Unity\Unity_Flow_State\UGS\Verification\Invoke-LiveTransferStorageProbe.ps1" -Run
```

3. 아래 질문이 순서대로 나온다. 각 행에 지정된 값을 붙여넣고 Enter를 누른다.

| 터미널 질문 | 입력할 값 |
| --- | --- |
| `Type VERIFY to approve these test writes` | `VERIFY` |
| `Temporary service account Key ID` | `fs-phase2-storage-probe`의 **이번에 생성한 Key ID** |
| `Temporary service account Secret Key` | 위 Key와 짝인 **Secret Key** |

4. 두 키 입력은 숨김 처리된다. 글자가 화면에 보이지 않아도 붙여넣은 뒤 Enter를 누른다. 키는 명령 인수/파일에 저장되지 않으며 실행 중 자식 프로세스 환경과 메모리에 잠시 존재한다.
5. 마지막 PASS 또는 FAIL까지 기다린다. 수 분이 걸릴 수 있으며 각 사례의 완료 줄이 출력된다. 대기 시간은 계정 UI의 응답 제한과 별개다.
6. 출력된 `RUN`, `PASS`, `FAIL` 줄을 복사한다. 파일 탐색기에서 `C:\Unity\Unity_Flow_State\Ignore`에 `Step12` 폴더를 만들고, 메모장에 결과 줄만 붙여넣어 `live-transfer-storage-probe-result.txt`로 저장한다. 저장 파일에는 Key ID·Secret Key 입력값을 넣지 않는다. 결과 줄을 채팅에도 붙여넣는다. 실패한 경우는 해당 FAIL 결과를 먼저 전달한다.
7. 아래 3단계에 따라 이번 임시 키와 권한을 회수한다. 검사 자료는 결과 분석이 끝날 때까지 보관한다.

`-ExecutionPolicy Bypass`는 이 PowerShell 프로세스에만 적용한다. `-Run` 없이 실행하면 계획만 표시하고 원격 호출은 없다.

정상 완료의 마지막 줄은 현재 코드 기준 다음과 같다.

```text
PASS / LIVE_STORAGE_PROBE / FaultCases=20 / ModuleAuthAndRuntime=NOT_TESTED / ExistingAccountsUntouched=True
```

### 3. 이번 검사에 사용한 키와 권한을 회수한다

목적: 검사에만 필요했던 관리자 접근 권한을 종료한다.

1. **Administration → Service Accounts → fs-phase2-storage-probe**를 연다.
2. **Keys**에서 방금 실행에 사용한 Key ID와 일치하는 키 하나를 **Delete / Revoke**로 폐기한다.
3. **Project roles**에서 `Unity_Flow_State / Cloud Save Editor`를 제거한다.
4. 사용자 임시 보관 장소에 남은 이번 Key ID·Secret Key도 지운다. 결과 텍스트 파일은 보관한다.
5. 결과와 함께 `임시 Key 폐기 완료 / Cloud Save Editor 역할 회수 완료`라고 알려 준다.

Dashboard에서 항목 이름이 다르면 이름이 같은 계정과 실제 Key ID를 기준으로 선택한다. 폐기한 키로 발급된 Access Token의 즉시 무효화까지 확인한 것으로 간주하지 않으며, 도구는 실행 후 토큰을 파일로 저장하지 않는다.

## 판정

**2026-10-06 사용자 실제 결과:** 첫 실행에서 batch/CAS·정상 이전·before10/after9가 PASS였고 Remaining 실행에서 AFTER10·두 대상 동시 이전·제출 예약/이전 시작 경합이 모두 PASS였다. 저장 복구20사례와 경합2사례를 **두 실행의 결과를 합쳐** 확인했다. 최초 HTTP0 실패 원인은 확정하지 않는다. 다음 사용자 작업은 위 **3. 이번 검사에 사용한 키와 권한을 회수한다**와 완료 보고다. 검사를 다시 실행할 필요는 없다.

- `LIVE_BATCH_ATOMICITY_AND_CAS_SINGLE_WINNER`: 실제 서비스가 오래된 writeLock의 batch 전체를 거부하고, 같은 토큰의 두 쓰기 중 하나만 성공했다.
- `BASELINE_STORAGE_TRANSFER`: 전용 저장 자료에서 이전이 완료됐다.
- `STORAGE_RECOVERY_BEFORE_n` / `AFTER_n`: 완료 과정의 해당 저장 경계 직전 요청 중단/실제 성공 응답 폐기 뒤 재호출로 복구됐다. 장애는 호출자 쪽에서 의도적으로 만든 것이지 Unity 서버 내부 장애를 발생시킨 것이 아니다.
- `STORAGE_SINGLE_WINNER_RACE`: 두 대상의 동시 완료 뒤 승자가 하나이고 원래 source는 거부됐다.
- `STORAGE_SUBMISSION_TRANSFER_FENCE`: 제출 예약과 이전 시작을 동시에 실행했을 때 한 계정에 두 작업이 함께 확정되지 않았다. 제출은 Account 예약까지만 검사하며 실제 Leaderboard 점수를 쓰지 않는다.
- 최종 `LIVE_STORAGE_PROBE`: 위 전체를 통과했다. **ModuleAuthAndRuntime=NOT_TESTED**는 정확한 검증 범위 표시이며 실패를 숨기는 값이 아니다.

현재 준비 코드 기준 완료 저장 경계는 10개, before/after 20사례다. 코드가 달라지면 baseline에서 실제 저장 횟수를 다시 측정한다. 예기치 않은 요청 실패·권한403·제한429·timeout은 PASS로 바꾸지 않고 중단한다. 전체 Step 12 완료는 이 원격 결과와 기존 Module 검증을 받은 뒤 남은 범위를 다시 대조해 판정한다.
