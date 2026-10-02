# 목적

프로젝트 진행 과정에서 현재도 유효한 결정 사항을 기록한다.

향후 작업에 영향을 주는 결정 사항을 기록한다.

---

# 프로젝트 결정 사항

## 프로젝트 구조

- Project, Rules, Systems, Features, Tasks, Templates 영역을 분리하여 관리한다.
- Project 문서는 프로젝트 수준의 정보만 관리한다.
- System의 책임은 Systems 문서에서 관리한다.
- Feature의 규칙은 Features 문서에서 관리한다.
- 작업 기록은 Tasks 문서에서 관리한다.
- 문서 작성 형식은 Templates에서 관리한다.
- 동일한 내용을 여러 문서에 중복 작성하지 않는다.

---

## 프로젝트 규칙

- 수평 속도 증가와 수평 가속 기능은 제거된 확정 사항이다. Playing 중 수평 이동은 PlayerMovementSystem의 직렬화된 이동 속도 하나로 World +X 고정 값을 사용하며, 지상/공중 가속도와 최대 수평 속도 설정을 사용하지 않는다.
- Momentum Landing은 수평 속도를 변경하지 않는다. 중력 가속도는 점프와 낙하를 위한 수직 규칙이므로 이 결정의 제거 대상이 아니다.
- Infinite Pattern과 Collectible의 직렬화된 제작 좌표는 현재 PlayerMovementSystem의 직렬화된 이동 속도 `8`을 기준으로 한다. 해당 속도 변경은 Pattern 통과 검증과 Collectible 재배치를 함께 수행하는 제작 변경이다.
- 문서는 자신의 책임 범위만 관리한다.
- 프로젝트 수준의 내용과 System, Feature 수준의 내용을 혼합하지 않는다.
- 현재 Run 데이터와 확정 Result Data는 Runtime 전용이다.
- Settings, Input Binding Override, Tutorial 완료 상태, 개인 최고 기록 캐시와 제출 대기열은 로컬 영구 데이터다.
- 온라인 최고 기록과 순위는 계정 귀속 서버 데이터다.
- 온라인 기능 요청 시 Anonymous 계정을 사용한다. 외부 ID 계정 연결은 지원하지 않는다.
- Anonymous 계정은 서버 발급 8자리 Base32 이전 코드와 9자리 십진 인증값으로 현재 활성 기기에서 새 기기로 연결을 이전할 수 있다. 같은 논리 계정은 한 번에 하나의 활성 Player ID만 가진다.
- 공개 번호만으로 계정을 복구하지 않는다. 이전 기기를 사용할 수 없어 이전 자격 증명을 발급하지 못하면 연결을 복구하지 않는다.
- 온라인 기록과 Leaderboard 행은 논리 계정에 영구 귀속된 고정 소유 ID를 사용한다. 기기 이전은 활성 Player ID만 교체하며, 기록 행·동점·서버 수락 시각 metadata를 이전·복사·변경하지 않는다.
- 인증 토큰·서비스 Secret·개인정보와 임의 표시명은 앱 저장소와 제출 대기열에 저장하지 않는다.
- 온라인 서비스는 기존 Flow State UGS 프로젝트의 `verification` 환경에서 먼저 검증한다. Runtime 초기화는 환경명을 명시하며, 검증·운영 환경을 묵시적으로 선택하지 않는다.
- verification 자료는 Production으로 이전·복사하지 않고 삭제하지 않는다. 운영 테스트 계정·기록은 verification에만 두며 Production은 일반 사용자 기록만 사용한다.
- Prototype 8의 부정행위 방지는 서버 입력 검증·직접 Write 차단·활성 연결·제출 ID·점수 상한 검증까지다. 최소 구조화 운영 로그는 30일 보관하되 PII·비밀값·payload를 제외하고, 그보다 긴 archive는 만들지 않는다. 운영 담당은 사용자 1인이며 보안·정합성 이상 때 동일 환경 버전 롤백과 필요 시 Secret 회전을 수행한다.
- 사용자 환경 전환 UI는 제공하지 않는다. 빌드별 UGS 대상 환경을 명시적으로 고정하고, 인증 Player ID·공개 번호 캐시·개인 최고·Pending은 `(Project ID, Environment ID)`별 Local Save 영역으로 분리한다. Settings·튜토리얼·입력 설정은 기기 공용이다.
- 로컬 UUID 후보를 UGS Anonymous Player ID로 자동 재귀속하거나 전송하지 않는다. 사용자가 복구 제한을 확인한 뒤 명시적으로 동의한 경우에만 현재 인증 Player ID를 제출 대상으로 1회 귀속한다.
- Anonymous 계정 복구 제한 확인을 저장하기 전에는 생산 온라인 요청을 시작하지 않는다. Phase 3 검증 경로는 이 확인 상태를 명시적으로 제공하는 경우에만 온라인 요청을 허용한다.
- 온라인 점수 쓰기는 Cloud Code의 서버 검증 경계를 통과하며, Access Control은 Player의 Leaderboard 직접 쓰기를 거부한다. 서버는 Submission ID 중복, Board·Version 및 InfiniteMode 점수 상한을 검증한다.
- Phase 3 검증 소스와 배포 절차는 `UGS/VERIFICATION_DEPLOYMENT.md`에 있다. Local Save v2에 동의·UGS 귀속·완료 제출 ID를 보존한다. 검증용 서버의 최초 ledger 수동 생성, 계정별 128 ID/보드 100명 한도는 운영 전 해소할 제한이며 Unity·UGS 실제 검증 완료를 의미하지 않는다.
- Roadmap 007 Phase 3은 verification 환경에서 완료했다. Stage·Infinite Board 제출/조회, submission ID 재호출·변조·Version 거부, Player 직접 Write 403, 계정 불일치 차단·복구, Offline Pending 보존과 연결 복구 뒤 자동 제출을 실제로 확인했다. Phase 4는 이 경계를 UI·출시 후보 품질로 확장하는 별도 작업이다.
- Roadmap 007 Phase 4는 verification 환경의 Windows x64·1920×1080 Windowed 후보를 대상으로 하며 성능·응답 시간 측정은 제외한다. 공개 운영 준비와 서버 발급 십진 Public Player Number는 다음 Prototype으로 분리하며 세부 작업은 Roadmap 007의 후속 계획에서 관리한다.
- Phase 4 UI·순위 규칙은 2026-09-29 사용자 결정에 따라 관련 Feature 문서에 반영했다. 이는 코드·Scene·서버 배포나 검증 완료를 뜻하지 않으며, 기존 Phase 3 구현과 변경 계약의 차이는 Phase 4에서 해소한다.
- Roadmap 007 Phase 4는 2026-09-30 완료했다. Edit Mode 729/729, Play Mode 234/234, verification 서비스 UI·Offline 복구 및 Windows x64·1920×1080 Windowed 비 Development Player Build·실행을 확인했다. 성능·응답 시간 측정은 합의에 따라 제외했다. 이는 공개 운영 승인이 아니며 Production 환경·Public Player Number는 다음 Prototype 범위다.

---

## 시스템

- System은 하나의 책임만 담당한다.
- System은 독립적인 책임과 경계를 가진다.
- System의 책임은 Feature와 분리하여 관리한다.
- System 간에는 필요한 데이터만 전달한다.

---

## 기능

- 게임의 핵심 플레이는 점프와 관성 착지를 이용한 이동이다.
- 게임은 3D 오소그래픽 횡스크롤 카메라를 사용한다.
- 스테이지는 클리어 시간을 기준으로 완료를 판단한다.
- 무한 모드는 Momentum Landing 연속 성공에 따른 거리 Score 배율을 제공한다.
- 현재 단일 일반 Stage의 불변 식별자는 코드 기본값 `stage-001`, Stage Rules Version은 `1`이다. Stage는 `Cleared` 결과만 이 식별자와 밀리초 Clear Time으로 Leaderboard 제출 후보가 된다.
- InfiniteMode는 유효하게 확정된 Total Score와 Scoring Version으로 Leaderboard 제출 후보가 된다.
- Stage와 InfiniteMode는 규칙 Version이 다른 기록을 비교하지 않는 독립 Leaderboard를 사용한다.
- Offline·인증·서비스 실패는 플레이를 차단하지 않으며 제출 후보를 로컬 대기열에 보존한다.

---

## 기타

- Prototype 8의 계획은 `AI/04_Implementation_Roadmap/IMPLEMENTATION_ROADMAP_008.md`에서 관리한다. 서버 발급 십진 Public Player Number, 검증용 제출·조회 제한 해소 및 공개 운영 준비가 목표다. Phase 1의 정책 확정·정적 계약 검증·Phase 2~5 인계는 2026-10-03 완료했고, 서비스·Client·Scene 구현과 운영 검증은 아직 착수하지 않았다. 기존 UI·순위·Pending 계약을 승계한다.

- 프로젝트는 1인 개발을 기준으로 진행한다.
- Unity를 사용하여 3D 게임으로 개발한다.
- AI와 협업하기 위한 문서 중심 개발 방식을 사용한다.
- 수동 검증 절차는 IDE 디버그 기능에 의존하지 않도록 구성한다.

---

# 관련 문서

## Project

- PROJECT_OVERVIEW.md
- ARCHITECTURE.md

---

## Rules

- AI_RULE.md
- IMPLEMENTATION_RULE.md

---

## Systems

- ResultSystem.md
- RecordSubmissionSystem.md

---

## Features

- Leaderboard.md
- RecordSubmission.md
