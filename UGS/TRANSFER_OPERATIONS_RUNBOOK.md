# Account transfer 운영 적용 안내

이 문서는 Prototype 8의 계정 이전 서비스가 구현된 뒤 verification과 production에 적용할 운영 경계를 기록한다. 현재 저장소에는 순수 정책만 있으며, 이 문서의 UGS Dashboard·CLI 변경은 AI가 실행하지 않는다.

## 환경 경계

- 계정 C, 공개 번호, 활성 Player 연결, 이전 요청·코드 digest·인증 HMAC과 감사 행은 `(Project ID, Environment ID)` 안에서만 유효하다. Client 입력의 환경·계정·공개 번호는 권한 근거가 아니다.
- Cloud Code는 실행 context의 Project ID와 Environment ID가 Account 및 Transfer 저장 행과 모두 일치할 때만 요청을 처리한다. 불일치, 누락 또는 다른 환경의 코드에는 연결 상태·번호·성공 여부를 노출하지 않는 일반 실패를 반환한다.
- verification 자료를 production에 복사·승계하지 않고, production 자료를 verification에 내려받거나 조회하지 않는다. 각 환경에서 별도 Anonymous 계정·번호·이전 자격 증명을 사용한다.

## 감사와 개인정보

- 서버 감사 행은 `Requested`, `CredentialReissued`, `Completed`, `Cancelled`, `Expired`, `VerificationFailed` 이벤트, 결과, 서버 생성 transfer ID, Environment ID, 서버 시각, 실패 누적 수와 안전한 사유 분류만 기록한다.
- 이전 코드 원문, 9자리 인증값, 인증·서비스 토큰, HMAC, digest, 내부 Player ID, 공개 번호, Client 전달 전체 요청 본문은 감사 행·일반 로그·오류·고객센터 대화에 기록하거나 표시하지 않는다.
- 일반 오류는 자격 증명 존재·일치 여부나 다른 기기의 성공 여부를 밝히지 않는다. 운영자가 특정 이전을 추적해야 할 때도 고객에게는 transfer ID나 내부 ID를 요청하지 않고, 서버 상관관계 ID를 운영 로그 안에서만 사용한다.
- UGS Cloud Code 로그는 장기 개인정보 감사 저장소가 아니다. UGS의 현재 로그 보관 기간과 조직의 보존·삭제 요건을 Production 적용 전에 확인하고, 그보다 긴 감사 보관이 필요할 때만 별도 서버 저장소·접근 권한·삭제 절차를 명시적으로 승인한다.
- Production은 환경·script version·안전한 outcome/reason 분류·처리 시간 구간·서버 correlation ID·요청 제한 결과만 가진 구조화 로그를 30일 보관한다. UGS Cloud Code 기본 로그 보관으로 30일을 충족하지 못하면, 개인정보·비밀값을 받지 않는 최소 외부 sink와 30일 삭제 절차를 먼저 준비해야 한다. 그보다 긴 archive는 만들지 않는다.

## 장애 대응과 요청 제한

- 운영 담당은 사용자 1인이다. Timeout·서비스 오류는 Pending 보존과 기존 제한 재시도로 처리한다. 데이터 정합성·비밀 노출·보안 이상은 해당 배포를 중단하고 동일 환경의 이전 Cloud Code version을 재게시하며, 필요하면 Secret을 회전한다. 계정·번호·Leaderboard·receipt·Pending은 삭제·초기화하지 않는다.
- 부정행위 방지는 서버 입력 검증·직접 Write 차단·활성 연결·제출 ID·점수 상한 검증 범위다. 서버 권위 Run 시뮬레이션은 별도 후속 범위다.
- 서버 제한은 C·목록 종류별 조회 5초 1회, C별 신규 제출 60초 3회, 동일 제출 ID 재호출 5초 간격이며, 이전 자격 증명 검증의 기존 5초 제한을 유지한다. 초과 요청은 `TooManyRequests`와 안전한 재시도 시각을 반환하고 Client는 자동 재시도하지 않는다.

## 배포·롤백 수동 절차 — Phase 4 사용자

1. 대상 Project와 Environment를 명시적으로 확인한다. verification에서 검증한 코드·계정·번호·이전 자격 증명을 production으로 복사하지 않는다.
2. 대상 환경의 기존 Cloud Code script source, 활성 version, 입력 정의, Access Control 정책, Secret 이름·권한(값 제외)을 읽기 전용으로 백업한다. 이전 버전 번호를 기록한다.
3. 모든 Node 정책·Cloud Code 검사 통과, 대상 환경의 Player 직접 Leaderboard/Cloud Save Write 거부 정책 검토, Secret Manager의 HMAC 비밀 이름과 Cloud Code 접근 권한 확인이 끝난 뒤에만 게시한다. 비밀값은 저장소·채팅·명령 기록에 넣지 않는다.
4. 한 환경의 한 배포 단위만 게시한다. 발급·재발급·만료·완료·A 차단과 일반 오류의 비밀값 비노출은 verification의 익명 테스트 계정으로 확인한다. Production에는 운영 테스트 계정·기록을 만들지 않으며, Cloud Code logger의 허용 감사 필드 검토도 verification 근거로 수행한다.
5. 실패 또는 비밀 노출 우려가 있으면 신규 게시를 중단하고, 같은 대상 환경에서 기록한 직전 Cloud Code version을 재게시한다. 계정·번호·Leaderboard·Cloud Save 자료를 삭제하거나 다른 환경 자료로 덮어쓰지 않는다. 비밀 노출이 있었다면 해당 Secret을 회전하고 영향 범위를 검토한다.

Cloud Code는 이전 script version을 지정하여 재게시하는 방식으로 롤백할 수 있다. 실제 명령·권한은 배포 시점의 Unity 공식 문서와 조직 권한을 기준으로 사용한다.
