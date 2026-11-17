# 기능 개요

## 기능명

Leaderboard

---

## 목적

플레이어가 기록 순위를 확인할 수 있도록 한다.

기록 경쟁의 기준을 제공한다.

---

## Roadmap 9 적용 계약

사용자 Stage 탭/선택 차단과 Main Menu/Result의 Infinite 조회 전환은 Phase 2에서 완료했다. Phase 3의 직접 Repository 조회/제출·Editor 경계 차단은 미착수다. 아래 사용자 규칙과 별도 Deprecated 내부 계약을 구분하며, UI 차단을 모든 직접 호출의 차단 완료로 확대하지 않는다.

- 새 Client의 사용자 순위는 Infinite 단일 Board만 제공한다. Stage Tab·사용자 선택/표시는 차단했고 남은 직접 조회/조작 경계는 Phase 3에서 차단한다.
- 상위 10·내 주변 7·Result의 최고 조회, competition rank·공개 번호·출처별 Back/Cancel을 유지한다.
- Infinite Board/Scoring Version과 서버 요청/응답 계약을 유지한다. Stage 기록/구 Client 지원을 위한 보관·이관·서비스 변경은 추가하지 않는다.

검증 계약: P9-C04/P9-C06/P9-C10. [Phase 1 Task](../90_Tasks/Prototype_9/20261111_02_Phase1ManualSteps.md), [Roadmap 9](../04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_009.md).

---

# 기능 규칙

- 본인의 확정 Result·로컬 개인 최고·온라인 기록이 없어도 동의/인증·활성 계정 연결과 서비스 조건이 충족되면 Leaderboard를 조회할 수 있다.
- InfiniteMode의 Leaderboard는 InfiniteMode 전체에 대해 하나의 순위를 제공한다.
- InfiniteMode는 유효하게 확정된 Total Score 내림차순으로 순위를 제공한다.
- 동일한 Infinite Total Score는 서버 수락 시각과 무관하게 같은 순위를 부여하고 competition ranking을 사용한다. 예를 들어 동일 최고 값 두 건 다음 순위는 `1, 1, 3`이다.
- 같은 순위 안의 표시 순서는 서버 수락 시각 오름차순이다. 클라이언트의 기록 생성 시각은 사용하지 않는다.
- Score와 서버 수락 시각까지 같으면 정확한 10자리 공개 번호 오름차순으로 표시한다. 공개 번호는 이 최종 표시 정렬 외의 권한·복구·기록 소유 근거가 아니다.
- 동점 그룹은 고정 목록 경계에서 분리될 수 있으며, 분리된 모든 행은 서버가 계산한 전역 competition rank를 그대로 표시한다.
- Player는 같은 Board Key에서 더 나은 기록만 최고 기록으로 유지한다. 같은 값 재제출은 기존 기록을 유지한다.
- InfiniteMode Board는 Scoring Version으로 분리한다. 일반 Game Version은 Board 분리 기준이 아니다.
- Leaderboard는 기록을 순위 형태로 제공한다.
- Leaderboard는 기록을 생성하거나 수정하지 않는다.
- Main Menu에서는 상위 10건과 내 주변 7건을 독립적으로 조회하고 각 영역의 Loading·성공·Empty·Offline·Error를 독립적으로 표시한다. 내 최고 기록 전용 영역과 조회 요청은 제공하지 않는다.
- 사용자에게 Previous/Next 또는 페이지 번호 이동 UI를 제공하지 않는다. 서버는 상위·내 주변 결과를 만들기 위해 내부 페이지·cursor 조회를 사용할 수 있으나, Client는 해당 고정 목록만 표시한다.
- 새 Client는 Infinite Tab만 제공한다. Stage Tab은 숨기고 입력/Navigation에서 제외하며 기존 이벤트나 직접 선택 요청으로도 Stage 기록으로 전환할 수 없다.
- 온라인 요청은 RecordSubmission의 동의·계정 연결 조건을 충족해야 한다. 본인 기록 부재를 조회 금지 조건으로 사용하거나 로컬 기록만으로 서버 Empty/내 기록 없음 상태를 단정하지 않는다.
- Empty는 원격 조회가 성공하고 해당 목록이 0건일 때만 사용한다. 확정 기록 없음, 제출 대기, 내 온라인 기록 없음은 구분해 안내한다.
- 내 주변/내 기록 조회의 404는 보드 존재·읽기 가능과 본인 행 부재를 확인한 경우에만 성공한 `NoPersonalRecord`로 처리한다.
  내 주변은 `No online record yet. Submit a record to see your position.`으로 안내한다. 보드 누락·권한/서비스 오류나
  본인 행이 존재하는데 발생한 range 404는 Error로 유지하며 모든 404를 빈 목록으로 바꾸지 않는다.
- 네트워크 상태가 `NotReachable`이면 Offline으로 표시한다. Timeout·인증·서비스 실패 등은 Error로 표시하고 확인 가능한 원인과 안전한 오류 코드를 제공한다. 확인되지 않은 원인은 추측하지 않는다.
- 실패한 조회를 다시 요청하는 Retry는 제출 Retry와 구분한다. 원본 예외·token·secret·전체 내부 Player ID는 오류 화면에 노출하지 않는다.
- 상위·내 주변의 한 조회 응답은 스냅샷으로 유지한다. 최초 진입, 명시적 Retry·새로고침 또는 Leaderboard 재진입 때만 새 조회를 시작하며, 타이머·제출 완료만으로 자동 새로고침하지 않는다.
- 조회 중 페이지 누락·중복, rank 또는 total 불일치, 공개 번호 매핑 실패가 확인되면 부분 목록을 Success 또는 Empty로 표시하지 않고 해당 영역을 Error로 처리한다.
- 별도 `Retry Pending (N)`은 현재 Run에 한정되지 않고 Local Save에 남은 같은 계정의 Pending 후보 N건을 새 플레이 없이 재전송한다. 조회 Retry와 이름·요청 경로를 분리하고 Keyboard·Mouse로 사용할 수 있게 한다. N=0이면 Button을 숨기고 Navigation 선택 목록에서 제외한다. 이때 조회 Retry와 Back을 직접 연결하며, 사라지는 Pending Button에 포커스가 있으면 조회 Retry로 옮긴다. N>0이면 조회 Retry → Pending Retry → Back 순서로 연결한다. 실행 중·완료 후 제출·거절·남은 Pending 건수를 안전한 상태 Text로 표시한다.
- Infinite 점수는 천 단위 구분 정수 `12,345`로 표시한다.
- 공개 번호가 발급된 계정과 행 식별자는 전체 10자리 공개 번호로 표시한다. 본인 행에는 `<공개 번호> (You)`를 표시하며, 표시 문자열 대신 활성 Player ID 연결로 본인 여부를 판정한다.
- 공개 번호 발급 또는 행 번호 조회가 실패하면 해당 Leaderboard 영역은 안전한 Error와 조회 Retry를 표시한다. 내부 Player ID·마스킹 ID·다른 계정 번호로 폴백하지 않는다.
- Leaderboard 행은 논리 계정에 영구 귀속된 고정 소유 ID를 사용한다. 기기 이전은 행을 A에서 B로 이동하거나 순위 값을 변경하지 않는다.
- 기기 이전은 기존 행의 서버 수락 시각 metadata와 동점 표시 순서를 변경하지 않는다.
- Back UI 항목은 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있다.
- Result의 `순위 보기`는 Infinite Scoring Version Board를 연다. 진입 출처와 선택을 보존하며 Back/Cancel은 원래 Result 또는 Main Menu의 Leaderboard 선택으로 복귀한다.

---

# 시작 조건

다음 조건을 만족하는 경우 Leaderboard를 시작한다.

- 플레이어가 Leaderboard 조회를 요청하였다.

---

# 종료 조건

## 정상 종료

- Leaderboard 조회가 완료되었다.

## 강제 종료

- 게임이 종료된다.

---

# 수행 결과

- 현재 Leaderboard가 제공된다.
- 플레이어는 자신의 순위를 확인할 수 있다.

---

# 예외 사항

- 동의 취소 시 온라인 요청 없이 진입 화면으로 복귀한다.
- 게임이 종료된 이후에는 수행하지 않는다.

---

# 관련 System

- ResultSystem
- UIManagementSystem
- AccountConnectionSystem

---

# 제약 사항

- Leaderboard는 기록을 생성하거나 수정하지 않는다.
- InfiniteMode의 Leaderboard는 하나만 존재한다.
- InfiniteMode는 최종 점수를 기준으로 순위를 제공한다.
- 순위 행은 서버에서 확정된 유효 기록을 사용한다. 조회자 본인의 기록 보유 여부는 조회 허용 조건과 별개다.
- 규칙 Version이 다른 기록은 순위, 최고 기록 또는 동점 판정을 함께 수행하지 않는다.
- 전체 내부 Player ID를 화면에 표시하지 않는다.

---

# 검증 항목

- InfiniteMode에서 최종 점수가 InfiniteMode Leaderboard에 반영되는지 확인한다.
- 본인 확정 기록 없이도 조건 충족 시 조회되고 내 온라인 기록 없음과 빈 보드/조회 실패가 구분되는지 확인한다.
- Stage 탭/선택/표시 차단과 Phase 3의 직접 Stage 조회/제출 거절을 구분해 검증한다.
- Leaderboard 조회 시 현재 순위를 정상적으로 확인할 수 있는지 확인한다.
- Leaderboard가 기록을 생성하거나 수정하지 않는지 확인한다.
- 새로운 기록이 확정된 이후 Leaderboard를 다시 조회하면 최신 순위가 반영되는지 확인한다.
- 상위·내 주변 영역의 독립 상태, 기록 없음과 Empty 구분, 원인별 Error 및 조회 Retry를 확인한다.
- 동일 순위 값은 수락 시각이 달라도 같은 순위이며, 해당 순위 안에서는 수락 시각 오름차순인지 확인한다.
- Main Menu에서 내 최고 전용 조회가 발생하지 않고, Result 진입 및 Back/Cancel 복귀가 보존되는지 확인한다.

---

# 보존된 Stage Board 계약 — Deprecated

- Stage Board는 불변 Stage ID/Rules Version으로 구분하고 기존 Infinite Board와 섞거나 변환하지 않는다. 이는 내부 자료형/서버 계약 보존이며 사용자 Stage 탭/조회 기능을 제공한다는 뜻이 아니다.
- 보존된 Stage 순위 값은 `Cleared`의 정수 밀리초 Clear Time 오름차순이며 형식은 `12.345 s`다. 내부 형식/formatter·순위 정책의 합성 검증은 생산 Stage 제출/표시와 분리한다.
- 기존 보드/자료 삭제·Stage 기록 이관·구 Client 지원을 추가하지 않는다. 남은 직접 호출 차단은 Phase 3 Task의 실제 구현/검증 완료 후 반영한다.

---

# 문서 작성 원칙

현재 Feature의 정의만 작성한다.

Feature의 규칙만 작성한다.

System의 책임을 작성하지 않는다.

구현 방법을 작성하지 않는다.

작업 기록을 작성하지 않는다.

변경 이력을 작성하지 않는다.

추측을 작성하지 않는다.

동일한 내용을 여러 섹션에 중복 작성하지 않는다.

Feature 하나당 문서 하나를 사용한다.
