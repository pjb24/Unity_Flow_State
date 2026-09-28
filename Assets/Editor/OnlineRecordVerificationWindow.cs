using System;
using System.Text;
using System.Threading.Tasks;
using FlowState.Runtime.Features;
using FlowState.Runtime.Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Services.Authentication;

namespace FlowState.Editor
{
    public sealed class OnlineRecordVerificationWindow : EditorWindow
    {
        private const long ProbeStageScore = 900000;
        private const long MutatedProbeStageScore = 899999;
        private const string SubmitRecordEndpoint = "submit-record";
        private const string StageLeaderboardId = "fs-stage-stage-001-r1";
        private const string DirectWriteProbeKey = "fs_verification_direct_write_probe";
        private const string VerificationProfile = "flow-state-verification";
        private const string MismatchProbeProfile = "flow-state-mismatch-probe";
        private bool _confirmed;
        private bool _busy;
        private string _summary = "Play Mode에서 기존 게임을 실행하세요. Scene은 변경하지 않습니다.";

        [MenuItem("Flow State/Online Record Verification")]
        private static void Open()
        {
            GetWindow<OnlineRecordVerificationWindow>("Online Verification");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("verification 전용 / 운영 환경 사용 금지");
            EditorGUILayout.HelpBox("익명 계정의 인증 정보를 잃으면 계정과 온라인 기록을 복구할 수 없습니다. " +
                "기존 로컬 기록은 최초 인증 계정에 귀속되며 다른 계정으로 이전하지 않습니다.", MessageType.Warning);
            _confirmed = EditorGUILayout.Toggle("복구 제한을 읽고 동의합니다", _confirmed);
            GameSystem game = Application.isPlaying ? UnityEngine.Object.FindFirstObjectByType<GameSystem>() : null;
            using (new EditorGUI.DisabledScope(game == null || _busy))
            {
                using (new EditorGUI.DisabledScope(!_confirmed))
                    if (GUILayout.Button("동의 저장 및 인증 / Pending 재시도")) Authenticate(game);
                if (GUILayout.Button("Pending 재시도")) RetryPending(game);
                if (GUILayout.Button("Stage 상위·내 주변·내 최고 조회")) Query(game, false);
                if (GUILayout.Button("Infinite 상위·내 주변·내 최고 조회")) Query(game, true);
                if (GUILayout.Button("서버 중복·변조·버전 거부 검증")) VerifyServerValidation(game);
                if (GUILayout.Button("Player 직접 Write 403 검증")) VerifyDirectWriteDenied(game);
                if (GUILayout.Button("계정 불일치 차단 검증 (원래 계정 복구)")) VerifyAccountMismatch(game);
                if (GUILayout.Button("테스트 Player ID를 클립보드에 복사"))
                    EditorGUIUtility.systemCopyBuffer = game.OnlinePlayerId;
            }
            EditorGUILayout.HelpBox(_summary, MessageType.Info);
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_summary)))
            {
                if (GUILayout.Button("표시 결과 복사"))
                {
                    EditorGUIUtility.systemCopyBuffer = _summary;
                }
            }
        }

        private async void Authenticate(GameSystem game)
        {
            _busy = true;
            try
            {
                _summary = await game.ConfirmOnlineRecoveryNoticeAsync()
                    ? "인증·귀속 성공. 최초 계정은 안내 문서에 따라 Protected ledger를 생성한 뒤 재시도하세요."
                    : "인증·귀속 실패. 계정 불일치 또는 로컬 저장/프로젝트 연결을 확인하세요.";
            }
            catch (Exception) { _summary = "검증 요청 실패. 토큰이나 전체 오류 응답을 공유하지 마세요."; }
            finally { _busy = false; Repaint(); }
        }

        private async void Query(GameSystem game, bool infinite)
        {
            _busy = true;
            try
            {
                RecordBoardKey key;
                if (infinite) RecordBoardKey.TryCreateInfinite(2, out key);
                else RecordBoardKey.TryCreateStage("stage-001", 1, out key);
                if (game.OnlineRecords == null) { _summary = "게임 초기화가 필요합니다."; return; }
                OnlineLeaderboardResult top = await game.OnlineRecords.GetTopAsync(key);
                OnlineLeaderboardResult around = await game.OnlineRecords.GetAroundAsync(key);
                OnlineLeaderboardResult me = await game.OnlineRecords.GetPersonalBestAsync(key);
                _summary = "verification / " + (infinite ? "fs-infinite-v2" : "fs-stage-stage-001-r1") +
                    "\n상위: " + Summarize(top) + "\n주변: " + Summarize(around) + "\n내 최고: " + Summarize(me);
            }
            catch (Exception) { _summary = "조회 실패. 로컬 기록은 유지됩니다."; }
            finally { _busy = false; Repaint(); }
        }

        private async void RetryPending(GameSystem game)
        {
            _busy = true;
            try
            {
                NetworkReachability reachability = Application.internetReachability;
                int before = game.PendingOnlineRecordCount;
                int submittedBefore = game.SubmittedOnlineRecordCount;
                int rejectedBefore = game.RejectedOnlineRecordCount;
                await game.RetryOnlineRecordsAsync();
                int after = game.PendingOnlineRecordCount;
                int submitted = game.SubmittedOnlineRecordCount - submittedBefore;
                int rejected = game.RejectedOnlineRecordCount - rejectedBefore;
                if (before == 0)
                {
                    _summary = "verification / Pending 재시도\n네트워크 판정: " + reachability +
                        "\n재시도 전: 0건\n처리 결과: 대기 기록 없음";
                }
                else if (after == 0)
                {
                    _summary = "verification / Pending 재시도\n네트워크 판정: " + reachability +
                        "\n재시도 전: " + before +
                        "건\n재시도 후: 0건\n이번 처리: Submitted " + submitted +
                        "건 / Rejected " + rejected + "건\n처리 결과: 모든 대기 기록 완료";
                }
                else
                {
                    _summary = "verification / Pending 재시도\n네트워크 판정: " + reachability +
                        "\n재시도 전: " + before +
                        "건\n재시도 후: " + after +
                        "건\n이번 처리: Submitted " + submitted + "건 / Rejected " + rejected +
                        "건\n처리 결과: 미완료 기록 유지 (Offline·인증·서비스 상태 확인)";
                }
            }
            catch (Exception)
            {
                _summary = "Pending 재시도 실패. 로컬 대기 기록은 유지됩니다.";
            }
            finally { _busy = false; Repaint(); }
        }

        private async void VerifyServerValidation(GameSystem game)
        {
            _busy = true;
            try
            {
                if (!IsAuthenticatedForProbe(game))
                {
                    _summary = "먼저 동의 저장 및 인증을 완료하고 Player ID 귀속 성공을 확인하세요.";
                    return;
                }

                UgsOnlineRecordTransport transport = new UgsOnlineRecordTransport();
                string submissionId = Guid.NewGuid().ToString("D");
                OnlineRecordRequest accepted = CreateStageProbe(submissionId, ProbeStageScore, 1);
                OnlineSubmissionResponse first = await CallSubmitAsync(transport, accepted);
                OnlineSubmissionResponse repeated = await CallSubmitAsync(transport, accepted);
                OnlineSubmissionResponse mutated = await CallSubmitAsync(transport,
                    CreateStageProbe(submissionId, MutatedProbeStageScore, 1));
                OnlineSubmissionResponse invalidVersion = await CallSubmitAsync(transport,
                    CreateStageProbe(Guid.NewGuid().ToString("D"), ProbeStageScore, 99));

                _summary = "verification / 서버 검증\n동일 ID 최초: " + Summarize(first) +
                    "\n동일 payload 재호출: " + Summarize(repeated) +
                    "\n동일 ID 변조: " + Summarize(mutated) +
                    "\n잘못된 버전: " + Summarize(invalidVersion) +
                    "\n기대: Submitted / Submitted / Rejected:SubmissionIdConflict / Rejected:InvalidVersion";
            }
            catch (Exception)
            {
                _summary = "서버 검증 요청 실패. 네트워크와 verification Cloud Code 배포 상태를 확인하세요.";
            }
            finally { _busy = false; Repaint(); }
        }

        private async void VerifyDirectWriteDenied(GameSystem game)
        {
            _busy = true;
            try
            {
                if (!IsAuthenticatedForProbe(game))
                {
                    _summary = "먼저 동의 저장 및 인증을 완료하고 Player ID 귀속 성공을 확인하세요.";
                    return;
                }

                string accessToken = AuthenticationService.Instance.AccessToken;
                long leaderboardStatus = await PostJsonAsync(
                    "https://leaderboards.services.api.unity.com/v1/projects/" +
                    OnlineRecordConfiguration.ProjectId + "/leaderboards/" + StageLeaderboardId +
                    "/scores/players/" + game.OnlinePlayerId,
                    "{\"score\":900000,\"metadata\":{\"source\":\"PlayerDirectWriteProbe\"}}",
                    accessToken);
                long cloudSaveStatus = await PostJsonAsync(
                    "https://cloud-save.services.api.unity.com/v1/data/projects/" +
                    OnlineRecordConfiguration.ProjectId + "/players/" + game.OnlinePlayerId + "/items",
                    "{\"key\":\"" + DirectWriteProbeKey +
                    "\",\"value\":{\"source\":\"PlayerDirectWriteProbe\"}}", accessToken);

                _summary = "verification / Player 직접 Write\nLeaderboard: HTTP " + leaderboardStatus +
                    "\nCloud Save Default: HTTP " + cloudSaveStatus +
                    "\n기대: 두 요청 모두 HTTP 403. 2xx이면 보안 검증 실패이며 probe key를 Dashboard에서 삭제하세요.";
            }
            catch (Exception)
            {
                _summary = "직접 Write 검증 요청 실패. 토큰이나 응답 본문은 공유하지 마세요.";
            }
            finally { _busy = false; Repaint(); }
        }

        private async void VerifyAccountMismatch(GameSystem game)
        {
            _busy = true;
            try
            {
                if (!IsAuthenticatedForProbe(game))
                {
                    _summary = "먼저 동의 저장 및 인증을 완료하고 Player ID 귀속 성공을 확인하세요.";
                    return;
                }

                string boundPlayerId = game.OnlinePlayerId;
                AuthenticationService.Instance.SignOut();
                AuthenticationService.Instance.SwitchProfile(MismatchProbeProfile);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey key);
                OnlineLeaderboardResult mismatch = await game.OnlineRecords.GetTopAsync(key);

                AuthenticationService.Instance.SignOut();
                AuthenticationService.Instance.SwitchProfile(VerificationProfile);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                bool restored = AuthenticationService.Instance.PlayerId == boundPlayerId;
                _summary = "verification / 계정 불일치\n다른 익명 계정 조회: " + Summarize(mismatch) +
                    "\n원래 계정 복구: " + (restored ? "성공" : "실패") +
                    "\n기대: TransientFailure / AuthenticationUnavailable, 원래 계정 복구 성공.";
            }
            catch (Exception)
            {
                _summary = "계정 불일치 검증 또는 원래 계정 복구에 실패했습니다. Play Mode를 중지한 뒤 다시 시작하고 인증을 재확인하세요.";
            }
            finally { _busy = false; Repaint(); }
        }

        private static bool IsAuthenticatedForProbe(GameSystem game)
        {
            if (!AuthenticationService.Instance.IsSignedIn || string.IsNullOrEmpty(game.OnlinePlayerId))
            {
                return false;
            }

            return true;
        }

        private static OnlineRecordRequest CreateStageProbe(string submissionId, long score, int rulesVersion)
        {
            return new OnlineRecordRequest
            {
                boardId = StageLeaderboardId,
                rulesVersion = rulesVersion,
                submissionId = submissionId,
                score = score,
                runDurationMilliseconds = 0,
                baseDistanceScore = 0,
                momentumBonus = 0,
                distanceScore = 0,
                collectibleScore = 0,
                totalScore = 0,
                maximumMomentumMultiplier = 1.0
            };
        }

        private static async Task<OnlineSubmissionResponse> CallSubmitAsync(
            UgsOnlineRecordTransport transport, OnlineRecordRequest request)
        {
            return await transport.CallAsync<OnlineSubmissionResponse>(
                SubmitRecordEndpoint, JsonUtility.ToJson(request));
        }

        private static async Task<long> PostJsonAsync(string url, string body, string accessToken)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
                operation.completed += _ => completion.TrySetResult(true);
                await completion.Task;
                return request.responseCode;
            }
        }

        private static string Summarize(OnlineLeaderboardResult result)
        {
            if (!result.IsSuccess) return result.status + " / " + result.reason;
            if (result.entries == null || result.entries.Length == 0) return "성공 / 기록 없음";
            return "성공 / " + result.entries.Length + "건 / 첫 기록 순위 " + result.entries[0].rank +
                " / 값 " + result.entries[0].score;
        }

        private static string Summarize(OnlineSubmissionResponse result)
        {
            return result == null ? "응답 없음" : result.status + ":" + result.reason;
        }
    }
}
