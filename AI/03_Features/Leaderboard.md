# 기능 개요

## 기능명

Leaderboard

---

## 목적

플레이어가 기록 순위를 확인할 수 있도록 한다.

기록 경쟁의 기준을 제공한다.

---

# 기능 규칙

- Leaderboard는 제출 가능한 확정 기록이 존재하는 경우에만 조회할 수 있다.
- 일반 Stage와 InfiniteMode는 각각 독립적인 Leaderboard를 사용한다.
- 일반 Stage의 Leaderboard는 Stage별로 독립적으로 제공한다.
- InfiniteMode의 Leaderboard는 InfiniteMode 전체에 대해 하나의 순위를 제공한다.
- 일반 Stage는 `Cleared` 결과의 정수 밀리초 Clear Time 오름차순으로 순위를 제공한다.
- InfiniteMode는 유효하게 확정된 Total Score 내림차순으로 순위를 제공한다.
- 동일한 순위 값(Stage Clear Time 또는 Infinite Total Score)은 서버 수락 시각과 무관하게 같은 순위를 부여하고 competition ranking을 사용한다. 예를 들어 동일 최고 값 두 건 다음 순위는 `1, 1, 3`이다.
- 같은 순위 안의 표시 순서는 서버 수락 시각 오름차순이다. 클라이언트의 기록 생성 시각은 사용하지 않는다.
- Player는 같은 Board Key에서 더 나은 기록만 최고 기록으로 유지한다. 같은 값 재제출은 기존 기록을 유지한다.
- Stage Board는 불변 Stage ID와 Stage Rules Version으로, InfiniteMode Board는 Scoring Version으로 분리한다. 일반 Game Version은 Board 분리 기준이 아니다.
- Leaderboard는 기록을 순위 형태로 제공한다.
- Leaderboard는 기록을 생성하거나 수정하지 않는다.
- Main Menu에서는 상위 기록과 내 주변 기록을 독립적으로 조회하고 각 영역의 Loading·성공·Empty·Offline·Error를 독립적으로 표시한다. 내 최고 기록 전용 영역과 조회 요청은 제공하지 않는다.
- Stage와 Infinite Tab을 제공한다. Stage Tab은 현재 단일 Stage를 `Stage 1` 고정 선택 행으로 표시하며 변경 조작을 제공하지 않는다.
- 제출 가능한 확정 기록이 없으면 온라인 조회 없이 기록 확정 안내를 표시한다. 온라인 요청은 RecordSubmission의 동의·계정 귀속 조건도 충족해야 한다.
- Empty는 원격 조회가 성공하고 해당 목록이 0건일 때만 사용한다. 확정 기록 없음, 제출 대기, 내 온라인 기록 없음은 구분해 안내한다.
- 네트워크 상태가 `NotReachable`이면 Offline으로 표시한다. Timeout·인증·서비스 실패 등은 Error로 표시하고 확인 가능한 원인과 안전한 오류 코드를 제공한다. 확인되지 않은 원인은 추측하지 않는다.
- 실패한 조회를 다시 요청하는 Retry는 제출 Retry와 구분한다. 원본 예외·token·secret·전체 내부 Player ID는 오류 화면에 노출하지 않는다.
- 별도 `Retry Pending (N)`은 현재 Run에 한정되지 않고 Local Save에 남은 같은 계정의 Pending 후보 N건을 새 플레이 없이 재전송한다. 조회 Retry와 이름·요청 경로를 분리하고 Keyboard·Mouse로 사용할 수 있게 한다. N=0이면 Button을 숨기고 Navigation 선택 목록에서 제외한다. 이때 조회 Retry와 Back을 직접 연결하며, 사라지는 Pending Button에 포커스가 있으면 조회 Retry로 옮긴다. N>0이면 조회 Retry → Pending Retry → Back 순서로 연결한다. 실행 중·완료 후 제출·거절·남은 Pending 건수를 안전한 상태 Text로 표시한다.
- Stage 시간은 정수 밀리초 기준 `12.345 s`, Infinite 점수는 천 단위 구분 정수 `12,345`로 표시한다.
- 현재 계정과 행 식별자는 내부 Player ID를 마스킹하여 표시한다. 본인 행에는 `(You)`를 덧붙이며, 마스킹 문자열 대신 전체 내부 식별자로 본인 여부를 판정한다.
- Back UI 항목은 Keyboard Navigate·Submit과 Mouse Point·Click으로 실행할 수 있다.
- Result의 `순위 보기`로 진입하면 해당 Mode·Stage·Version Board를 연다. 진입 출처와 선택을 보존하며 Back/Cancel은 원래 Result 또는 Main Menu의 Leaderboard 선택으로 복귀한다.

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

---

# 제약 사항

- Leaderboard는 기록을 생성하거나 수정하지 않는다.
- 일반 Stage와 InfiniteMode의 Leaderboard는 서로 독립적으로 관리한다.
- 일반 Stage의 Leaderboard는 Stage별로 독립적으로 관리한다.
- InfiniteMode의 Leaderboard는 하나만 존재한다.
- 일반 Stage는 클리어 시간을 기준으로 순위를 제공한다.
- InfiniteMode는 최종 점수를 기준으로 순위를 제공한다.
- 순위는 제출 가능한 확정 기록만 사용한다.
- 규칙 Version이 다른 기록은 순위, 최고 기록 또는 동점 판정을 함께 수행하지 않는다.
- 공개 십진 식별자 발급 전에는 전체 내부 Player ID를 표시하지 않는다.

---

# 검증 항목

- 일반 Stage에서 클리어 시간이 해당 Stage의 Leaderboard에 반영되는지 확인한다.
- 서로 다른 Stage의 기록이 같은 Leaderboard에 포함되지 않는지 확인한다.
- InfiniteMode에서 최종 점수가 InfiniteMode Leaderboard에 반영되는지 확인한다.
- 일반 Stage와 InfiniteMode의 Leaderboard가 서로 분리되어 있는지 확인한다.
- 확정된 기록이 없는 경우 Leaderboard가 제공되지 않는지 확인한다.
- Leaderboard 조회 시 현재 순위를 정상적으로 확인할 수 있는지 확인한다.
- Leaderboard가 기록을 생성하거나 수정하지 않는지 확인한다.
- 새로운 기록이 확정된 이후 Leaderboard를 다시 조회하면 최신 순위가 반영되는지 확인한다.
- 상위·내 주변 영역의 독립 상태, 기록 없음과 Empty 구분, 원인별 Error 및 조회 Retry를 확인한다.
- 동일 순위 값은 수락 시각이 달라도 같은 순위이며, 해당 순위 안에서는 수락 시각 오름차순인지 확인한다.
- Main Menu에서 내 최고 전용 조회가 발생하지 않고, Result 진입 및 Back/Cancel 복귀가 보존되는지 확인한다.

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
