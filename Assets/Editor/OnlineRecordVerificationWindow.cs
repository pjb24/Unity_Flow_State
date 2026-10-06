using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
    public sealed class OnlineRecordVerificationWindow : EditorWindow, IAccountTransferView
    {
        private const long ProbeStageScore = 900000;
        private const long MutatedProbeStageScore = 899999;
        private const string SubmitRecordEndpoint = "submit-record";
        private const string StageLeaderboardId = "fs-stage-stage-001-r1";
        private const string DirectWriteProbeKey = "fs_verification_direct_write_probe";
        private const string VerificationProfile = "flow-state-verification";
        private const string MismatchProbeProfile = "flow-state-mismatch-probe";
        private const float WindowMinimumWidth = 520f;
        private const float WindowMinimumHeight = 420f;
        private const float ContentHorizontalPadding = 40f;
        private Vector2 _scrollPosition;
        private readonly List<string> _visibleMessages = new List<string>();
        private readonly List<bool> _sensitiveMessages = new List<bool>();
        private bool _confirmed;
        private bool _busy;
        private bool _phase2 = true;
        private bool _remoteConsent;
        private int _sessionGeneration;
        private E_VerificationSession _session;
        private VerificationSessionConfiguration _configuration;
        private VerificationRemoteGuard _guard;
        private UgsOnlineAuthenticationGateway _authenticationGateway;
        private UgsOnlineRecordTransport _moduleTransport;
        private LocalRecordRepository _isolatedLocal;
        private OnlineAccountCoordinator _account;
        private CloudCodeRecordRepository _records;
        private OnlineRecordCoordinator _pending;
        private AccountTransferController _transfer;
        private AccountTransferScreenState _screen;
        private string _code = string.Empty;
        private string _verificationValue = string.Empty;
        private string _baselineInput = string.Empty;
        private string _baselineExport = string.Empty;
        private bool _infinite;
        private string _summary = "Phase 2 격리 세션을 먼저 준비하세요. Scene은 변경하지 않습니다.";

        [MenuItem("Flow State/Online Record Verification")]
        private static void Open()
        {
            GetWindow<OnlineRecordVerificationWindow>("Online Verification");
        }

        private void OnGUI()
        {
            _visibleMessages.Clear(); _sensitiveMessages.Clear();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            try { DrawWindowContents(); DrawMessageCopyControls(); }
            finally { EditorGUILayout.EndScrollView(); }
        }

        private void OnEnable()
        {
            minSize = new Vector2(WindowMinimumWidth, WindowMinimumHeight);
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        private void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode && state != PlayModeStateChange.EnteredEditMode) return;
            _sessionGeneration++;
            _remoteConsent = false;
            if (_transfer != null) _transfer.Close();
            _configuration = null; _guard = null; _authenticationGateway = null; _moduleTransport = null;
            _account = null; _records = null; _pending = null; _transfer = null; _screen = null; _isolatedLocal = null;
            ClearSensitiveInputs(); _baselineInput = string.Empty; _baselineExport = string.Empty;
            _visibleMessages.Clear(); _sensitiveMessages.Clear();
            _busy = false;
            SessionState.SetBool(VerificationSessionConfiguration.EditorIsolationKey, false);
            _summary = "STOPPED / A/B 전환 전 Editor를 종료·재시작하세요.";
            Repaint();
        }

        private void ArmEditorIsolation()
        {
            if (IsAutomatedExecution() || EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!HasSafePlayModeOptions())
            {
                _summary = "ARM_BLOCKED / Project Settings > Editor > Enter Play Mode Settings에서 Reload Domain과 Reload Scene을 켜세요.";
                return;
            }
            SessionState.SetBool(VerificationSessionConfiguration.EditorIsolationKey, true);
            _summary = "ISOLATION_ARMED / 상단 Play를 직접 누르세요. 게임은 메모리 저장·온라인 비활성, 검증 창만 A/B 격리 저장을 사용합니다.";
        }

        private static bool HasSafePlayModeOptions()
        {
            return !EditorSettings.enterPlayModeOptionsEnabled ||
                (EditorSettings.enterPlayModeOptions & (EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload)) == 0;
        }

        private float ContentWidth => Mathf.Max(160f, position.width - ContentHorizontalPadding);

        private bool DrawWrappedButton(string text)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button) { wordWrap = true, fixedHeight = 0, stretchWidth = true };
            float height = Mathf.Max(26f, style.CalcHeight(new GUIContent(text), ContentWidth) + 6f);
            return GUILayout.Button(text, style, GUILayout.Height(height), GUILayout.ExpandWidth(true));
        }

        private bool DrawWrappedToggle(string text, bool value)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label) { wordWrap = true, fixedHeight = 0 };
            float height = Mathf.Max(EditorGUIUtility.singleLineHeight, style.CalcHeight(new GUIContent(text), ContentWidth - 20f));
            return EditorGUILayout.ToggleLeft(new GUIContent(text), value, style, GUILayout.Height(height));
        }

        private void DrawSelectableValue(string text, bool sensitive = false)
        {
            GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel) { fixedHeight = 0 };
            float height = Mathf.Max(EditorGUIUtility.singleLineHeight, style.CalcHeight(new GUIContent(text), ContentWidth));
            EditorGUILayout.SelectableLabel(text, style, GUILayout.Height(height), GUILayout.ExpandWidth(true));
            RememberVisibleMessage(text, sensitive);
            if (DrawWrappedButton("이 값 복사")) CopyVisibleText(text, sensitive);
        }

        private void DrawCopyableMessage(string text, MessageType type, bool sensitive = false)
        {
            EditorGUILayout.HelpBox(text, type);
            RememberVisibleMessage(text, sensitive);
            if (DrawWrappedButton("이 메시지 복사")) CopyVisibleText(text, sensitive);
        }

        private void RememberVisibleMessage(string text, bool sensitive)
        {
            _visibleMessages.Add(text); _sensitiveMessages.Add(sensitive);
        }

        private void CopyVisibleText(string text, bool sensitive)
        {
            if (IsAutomatedExecution()) return;
            if (sensitive && !EditorUtility.DisplayDialog("민감 정보 포함 / 로컬 전용",
                "공개 번호·경로·이전 코드·인증값이 포함될 수 있습니다. 채팅/로그/스크린샷에 공유하지 마세요. 로컬 용도로 복사할까요?", "복사", "취소")) return;
            EditorGUIUtility.systemCopyBuffer = text;
        }

        private string BuildVisibleMessageReport(bool includeSensitive)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("Online Verification / " + DateTime.UtcNow.ToString("u"));
            for (int i = 0; i < _visibleMessages.Count; i++)
                report.AppendLine(!includeSensitive && _sensitiveMessages[i] ? "[민감 정보 제외]" : _visibleMessages[i]);
            return report.ToString();
        }

        private void DrawMessageCopyControls()
        {
            EditorGUILayout.Space();
            if (DrawWrappedButton("전체 메시지 복사 (민감 정보 제외)"))
                CopyVisibleText(BuildVisibleMessageReport(false), false);
            if (DrawWrappedButton("전체 메시지 복사 (민감 정보 포함 / 로컬 전용)"))
                CopyVisibleText(BuildVisibleMessageReport(true), true);
        }

        private void DrawWindowContents()
        {
            using (new EditorGUI.DisabledScope(_configuration != null || _busy || (_screen != null && _screen.IsBusy)))
                _phase2 = DrawWrappedToggle("Prototype 8 Phase 2 (Scene 없음)", _phase2);
            if (_phase2) { DrawPhase2(); return; }
            EditorGUILayout.LabelField("verification 전용 / 운영 환경 사용 금지");
            DrawCopyableMessage("구 Prototype 7 도구입니다. Phase 2 검증은 위 모드를 사용하세요. 자동 Test에서는 실행하지 않습니다.", MessageType.Warning);
            DrawCopyableMessage("익명 계정의 인증 정보를 잃으면 계정과 온라인 기록을 복구할 수 없습니다. " +
                "기존 로컬 기록은 최초 인증 계정에 귀속되며 다른 계정으로 이전하지 않습니다.", MessageType.Warning);
            _confirmed = DrawWrappedToggle("복구 제한을 읽고 동의합니다", _confirmed);
            GameSystem game = Application.isPlaying ? UnityEngine.Object.FindFirstObjectByType<GameSystem>() : null;
            using (new EditorGUI.DisabledScope(game == null || _busy || IsAutomatedExecution()))
            {
                using (new EditorGUI.DisabledScope(!_confirmed))
                    if (DrawWrappedButton("동의 저장 및 인증 / Pending 재시도")) Authenticate(game);
                if (DrawWrappedButton("Pending 재시도")) RetryPending(game);
                if (DrawWrappedButton("Stage 상위·내 주변·내 최고 조회")) Query(game, false);
                if (DrawWrappedButton("Infinite 상위·내 주변·내 최고 조회")) Query(game, true);
                if (DrawWrappedButton("서버 중복·변조·버전 거부 검증")) VerifyServerValidation(game);
                if (DrawWrappedButton("Player 직접 Write 403 검증")) VerifyDirectWriteDenied(game);
                if (DrawWrappedButton("계정 불일치 차단 검증 (원래 계정 복구)")) VerifyAccountMismatch(game);
                if (DrawWrappedButton("테스트 Player ID를 클립보드에 복사"))
                    EditorGUIUtility.systemCopyBuffer = game.OnlinePlayerId;
            }
            DrawCopyableMessage(_summary, MessageType.Info);
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_summary)))
            {
                if (DrawWrappedButton("표시 결과 복사"))
                {
                    EditorGUIUtility.systemCopyBuffer = _summary;
                }
            }
        }

        private async void Authenticate(GameSystem game)
        {
            if (!CanRunLegacy(game)) return;
            _busy = true;
            try
            {
                _summary = await game.ConfirmOnlineRecoveryNoticeAsync()
                    ? "인증·번호 확인 완료. 수동 ledger 초기화는 하지 않습니다."
                    : "인증·귀속 실패. 계정 불일치 또는 로컬 저장/프로젝트 연결을 확인하세요.";
            }
            catch (Exception) { _summary = "검증 요청 실패. 토큰이나 전체 오류 응답을 공유하지 마세요."; }
            finally { _busy = false; Repaint(); }
        }

        private async void Query(GameSystem game, bool infinite)
        {
            if (!CanRunLegacy(game)) return;
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
            if (!CanRunLegacy(game)) return;
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
            if (!CanRunLegacy(game)) return;
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
            if (!CanRunLegacy(game)) return;
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
            if (!CanRunLegacy(game)) return;
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

        private bool CanRunLegacy(GameSystem game)
        { return game != null && _confirmed && !IsAutomatedExecution() && Application.cloudProjectId == OnlineRecordConfiguration.ProjectId; }

        private static bool IsAutomatedExecution()
        {
            if (Application.isBatchMode) return true;
            foreach (string argument in Environment.GetCommandLineArgs())
                if (string.Equals(argument, "-runTests", StringComparison.OrdinalIgnoreCase)) return true;
            // The installed Test Framework exposes this as internal. Refuse
            // execution if its API changes or its active-run state is unknown.
            Type api = Type.GetType("UnityEditor.TestTools.TestRunner.Api.TestRunnerApi, UnityEditor.TestRunner");
            MethodInfo method = api == null ? null : api.GetMethod("IsRunActive", BindingFlags.Static | BindingFlags.NonPublic);
            if (method == null) return true;
            try { return (bool)method.Invoke(null, null); }
            catch (Exception) { return true; }
        }

        private bool CanRunPhase2()
        { return VerificationSessionConfiguration.CanExecuteRemote(_remoteConsent, IsAutomatedExecution(), Application.isBatchMode,
            Application.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode && HasSafePlayModeOptions(), Application.cloudProjectId,
            VerificationSessionConfiguration.IsEditorIsolationArmed); }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                SessionState.SetBool(VerificationSessionConfiguration.EditorIsolationKey, false);
            _remoteConsent = false;
            if (_transfer != null) _transfer.Close();
            ClearSensitiveInputs(); _baselineExport = string.Empty; _baselineInput = string.Empty;
            _visibleMessages.Clear(); _sensitiveMessages.Clear();
        }

        public void Render(AccountTransferScreenState state) { _screen = state; Repaint(); }
        public void ClearSensitiveInputs() { _code = string.Empty; _verificationValue = string.Empty; }

        private void DrawPhase2()
        {
            DrawCopyableMessage("1. Play 전 격리 실행 예약 → 2. 상단 Play 직접 실행 → 3. A/B 선택·격리 세션 준비 → 4. 원격 요청 허용. " +
                "게임은 메모리 저장·온라인 비활성으로 실행합니다. A/B 전환 전 Editor를 종료·재시작하세요. 자동 Test·Batch Mode는 차단됩니다.", MessageType.Warning);
            using (new EditorGUI.DisabledScope(IsAutomatedExecution() || EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (DrawWrappedButton("Play 전 격리 실행 예약 (원격 호출 없음)")) ArmEditorIsolation();
                if (DrawWrappedButton("격리 실행 예약 취소")) SessionState.SetBool(VerificationSessionConfiguration.EditorIsolationKey, false);
            }
            DrawCopyableMessage("IsolationArmed=" + VerificationSessionConfiguration.IsEditorIsolationArmed + " / Playing=" + Application.isPlaying, MessageType.Info);
            using (new EditorGUI.DisabledScope(_configuration != null || _busy))
                _session = (E_VerificationSession)EditorGUILayout.EnumPopup("격리 세션", _session);
            _remoteConsent = DrawWrappedToggle("verification 원격 요청·테스트 기록 변경 허용", _remoteConsent);
            if (!_remoteConsent && _transfer != null && _transfer.State.IsOpen) _transfer.Close();
            using (new EditorGUI.DisabledScope(_configuration != null || IsAutomatedExecution() || !Application.isPlaying || !VerificationSessionConfiguration.IsEditorIsolationArmed))
                if (DrawWrappedButton("격리 세션 준비 (로컬만)")) PreparePhase2();
            if (_configuration != null)
            {
                EditorGUILayout.LabelField("인증 프로필");
                DrawSelectableValue(_configuration.Profile);
                EditorGUILayout.LabelField("격리 저장 경로");
                DrawSelectableValue(_configuration.SavePath, true);
            }
            if (_account != null) DrawCopyableMessage(DescribeAccountReadiness(), MessageType.Info);
            using (new EditorGUI.DisabledScope(!CanRunPhase2() || _transfer == null || _busy || (_screen != null && _screen.IsBusy)))
            {
                if (DrawWrappedButton("계정 패널 열기 / 상태 확인")) ExecutePhase2(E_AccountTransferAction.Back, true);
                if (_screen != null && _screen.IsOpen)
                {
                    EditorGUILayout.LabelField("공개 번호");
                    DrawSelectableValue(_screen.NumberText, true);
                    DrawCopyableMessage(_screen.Message, MessageType.Info);
                    if (_screen.Code.Length != 0)
                        DrawCopyableMessage("이전 코드: " + _screen.Code + "\n인증값: " + _screen.VerificationValue +
                            "\n만료(UTC): " + DateTimeOffset.FromUnixTimeMilliseconds(_screen.ExpiresAtMilliseconds).UtcDateTime.ToString("u"), MessageType.Warning, true);
                    if (_screen.Page == E_AccountTransferPage.Input)
                    {
                        _code = EditorGUILayout.TextField("이전 코드", _code);
                        _verificationValue = EditorGUILayout.PasswordField("9자리 인증값", _verificationValue);
                        RememberVisibleMessage("입력 코드: " + _code + " / 입력 인증값: " + _verificationValue, true);
                    }
                    foreach (E_AccountTransferAction action in Enum.GetValues(typeof(E_AccountTransferAction)))
                        using (new EditorGUI.DisabledScope(!_transfer.CanExecute(action)))
                            if (DrawWrappedButton(action.ToString())) ExecutePhase2(action, false);
                }
                _infinite = DrawWrappedToggle("Infinite (해제: Stage)", _infinite);
                if (DrawWrappedButton("내 최고 기준 저장 / 재조회 번호 비교")) ProbePhase2(0);
                if (DrawWrappedButton("기준 자료와 번호·점수·수락 시각 비교")) ProbePhase2(1);
                if (DrawWrappedButton("격리 Stage 기록 제출 (60초)")) ProbePhase2(2);
                if (DrawWrappedButton("기존 Pending 1건 재전송 (새 기록 생성 없음)")) ProbePhase2(8);
                if (DrawWrappedButton("Stage 제출 전 조회만 확인 (기록 제출 없음)")) ProbePhase2(6);
                if (DrawWrappedButton("Step 10-1 신규 Stage 제출·서버 행 확인 (60초)")) ProbePhase2(5);
                if (DrawWrappedButton("Player 직접 Write 403 검증")) ProbePhase2(3);
                if (DrawWrappedButton("원래 A 상태만 확인 (복구 전에 / 제출 없음)")) ProbePhase2(4);
                if (DrawWrappedButton("원래 A 기록 조회 거부만 확인 (복구 전에 / 제출 없음)")) ProbePhase2(7);
            }
            if (_transfer != null && DrawWrappedButton("원문 제거 / 계정 패널 닫기")) { _remoteConsent = false; _transfer.Close(); }
            DrawCopyableMessage("기준 자료는 본인 공개 행만 포함합니다. A→B 비교용 로컬 파일에만 보관하고 채팅/결과 보고에 붙이지 마세요. " +
                "기준 캡처/비교는 이전 전후, 후속 새 기록 제출 전에 실행합니다.", MessageType.Info);
            EditorGUILayout.LabelField("비교할 기준 JSON (로컬 전용)", EditorStyles.wordWrappedLabel);
            GUIStyle baselineStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, fixedHeight = 0 };
            float baselineHeight = Mathf.Max(90f, baselineStyle.CalcHeight(new GUIContent(_baselineInput), ContentWidth));
            _baselineInput = EditorGUILayout.TextArea(_baselineInput, baselineStyle, GUILayout.Height(baselineHeight));
            if (!string.IsNullOrEmpty(_baselineInput)) RememberVisibleMessage("기준 JSON: " + _baselineInput, true);
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_baselineExport)))
                if (DrawWrappedButton("기준 자료를 사용자 파일로 저장")) ExportBaseline();
            DrawCopyableMessage(_summary, MessageType.Info);
            if (DrawWrappedButton("안전한 결과 복사")) EditorGUIUtility.systemCopyBuffer = _summary;
        }

        private void PreparePhase2()
        {
            if (IsAutomatedExecution() || !Application.isPlaying || !VerificationSessionConfiguration.IsEditorIsolationArmed || !HasSafePlayModeOptions() || _configuration != null) return;
            _configuration = new VerificationSessionConfiguration(Application.persistentDataPath, _session);
            _isolatedLocal = new LocalRecordRepository(new PersistentLocalSaveFileStore(_configuration.DirectoryPath));
            _isolatedLocal.TryLoad(out LocalSaveData loaded);
            if (!_isolatedLocal.CanUseOnlineData) { _summary = "FAIL / 격리 저장 준비 실패. 원본 파일을 보존합니다."; return; }
            if (string.IsNullOrEmpty(_isolatedLocal.AccountId))
            {
                LocalSaveData identified = new LocalSaveData(LocalSaveData.CurrentVersion, Guid.NewGuid().ToString("D"), loaded.Settings,
                    loaded.HasCompletedTutorial, loaded.PersonalBests, loaded.OnlineAccount, loaded.PendingSubmissions,
                    loaded.OnlineScope, loaded.InactiveOnlineAreas);
                if (!_isolatedLocal.TrySave(identified)) { _summary = "FAIL / 격리 기기 UUID 저장 실패"; return; }
            }
            UgsOnlineRecordTransport transport = new UgsOnlineRecordTransport();
            _moduleTransport = transport;
            _authenticationGateway = new UgsOnlineAuthenticationGateway(_configuration.Profile);
            int generation = ++_sessionGeneration;
            _guard = new VerificationRemoteGuard(() => generation == _sessionGeneration && CanRunPhase2(), _authenticationGateway, transport, transport);
            _account = new OnlineAccountCoordinator(_isolatedLocal, _guard, _guard,
                () => CanRunPhase2() && Application.internetReachability != NetworkReachability.NotReachable,
                ClearPhase2Presentation);
            _records = new CloudCodeRecordRepository(_isolatedLocal.AccountId, () => _isolatedLocal.OnlineAccount, _account, _guard,
                () => CanRunPhase2() && _isolatedLocal.CanUseOnlineData);
            _pending = new OnlineRecordCoordinator(_isolatedLocal.AccountId, _isolatedLocal, _account, _records,
                milliseconds => _guard.WaitWithinOperationAsync(() => Task.Delay(milliseconds)));
            _transfer = new AccountTransferController(_isolatedLocal, _account, this, () => _pending.RetryAllPendingAsync());
            _summary = "LOCAL_READY / SDK 인증·원격 호출 없음. 저장 준비 실패 시 원본을 지우지 마세요.";
        }

        private void ClearPhase2Presentation() { if (_pending != null) _pending.ClearSessionHistory(); }

        private async void ExecutePhase2(E_AccountTransferAction action, bool open)
        {
            if (!CanRunPhase2() || _transfer == null || _busy) return;
            _busy = true;
            _guard.BeginOperation();
            try
            {
                if (open) await _transfer.OpenAsync();
                else await _transfer.ExecuteAsync(action, _code, _verificationValue);
                _summary = "verification / " + DateTime.UtcNow.ToString("u") + " / ACCOUNT_ACTION_FINISHED / " + DescribeAccountReadiness();
            }
            catch (Exception) { _summary = "FAIL / 계정 요청 확인 불가. 원문·토큰·응답 본문을 공유하지 마세요."; }
            finally { FinishPhase2Operation(); }
        }

        private void FinishPhase2Operation()
        {
            if (_guard != null)
            {
                if (_guard.OperationTimedOut)
                    _summary = "FAIL / WINDOW_TIMEOUT / TimeoutMs=5000 / Unconfirmed Pending is preserved; refresh explicitly. No automatic retry.";
                _guard.EndOperation();
            }
            _busy = false; Repaint();
        }

        private RecordBoardKey SelectedBoard()
        {
            if (_infinite) { RecordBoardKey.TryCreateInfinite(2, out RecordBoardKey infinite); return infinite; }
            RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey stage); return stage;
        }

        private string DescribeAccountReadiness()
        {
            if (_account == null || _isolatedLocal == null) return "State=NotPrepared";
            return "State=" + _account.ViewState.State + " / Reason=" + SafeDiagnosticReason(_account.ViewState.Reason) +
                " / Consent=" + _isolatedLocal.OnlineAccount.HasConfirmedRecoveryNotice +
                " / Pending=" + _isolatedLocal.CreatePendingSnapshot().Count +
                " / LocalSave=" + (_isolatedLocal.CanUseOnlineData ? "Ready" : "Unavailable") +
                " / RemoteAllowed=" + CanRunPhase2() +
                " / AccountPanelTimeoutMs=" + OnlineAccountCoordinator.PanelTimeoutMilliseconds +
                " / WindowOperationTimeoutMs=" + VerificationRemoteGuard.OperationTimeoutMilliseconds +
                " / " + (_authenticationGateway == null ? "AuthPhase=NotPrepared" : _authenticationGateway.LastDiagnostic) +
                " / " + (_moduleTransport == null ? "ModulePhase=NotPrepared" : _moduleTransport.LastDiagnostic);
        }

        private static string SafeDiagnosticReason(string reason)
        {
            // Only known classifications are copied; never surface arbitrary server text or identities.
            switch (reason)
            {
                case "": return "None";
                case "LocalAccountUnavailable":
                case "LocalSaveUnavailable":
                case "NotReachable":
                case "AuthenticationUnavailable":
                case "NewAuthenticationUnavailable":
                case "AccountMismatch":
                case "SessionResetUnavailable":
                case "StaleResponse":
                case "PublicNumberUnavailable":
                case "PersonalBestUnavailable":
                case "TransferUnavailable":
                case "PendingMustBeCleared":
                case "Timeout":
                case "ServiceUnavailable":
                case "EmptyResponse":
                case "InvalidResponse":
                case "InvalidCredential":
                case "TooManyRequests": return reason;
                case "ContextMismatch":
                case "InvalidRequest":
                case "InvalidQuery":
                case "VerificationBoardCapacity":
                case "MissingServerMetadata":
                case "ClientFailure": return reason;
                default: return "Unknown";
            }
        }

        private static string DescribeStageQuery(OnlineLeaderboardResult result)
        {
            if (result == null) return "QueryStatus=Missing / QueryReason=None / RowCount=Unknown";
            string status = result.status == "Success" || result.status == "TransientFailure" ? result.status : "Unknown";
            return "QueryStatus=" + status + " / QueryReason=" + SafeDiagnosticReason(result.reason) +
                " / RowCount=" + (result.entries == null ? "Unknown" : result.entries.Length.ToString()) +
                " / QueryPhase=" + SafeQueryPhase(result.queryPhase) +
                " / QueryFault=" + SafeQueryFault(result.queryFault) +
                " / ServiceStatus=" + (result.serviceStatus >= 0 && result.serviceStatus <= 599 ? result.serviceStatus.ToString() : "Unknown");
        }

        private static string SafeQueryPhase(string phase)
        {
            switch (phase)
            {
                case null:
                case "": return "None";
                case "ResolveAccount":
                case "ReadPublicNumber":
                case "ReadLeaderboard":
                case "ResolvePublicRows":
                case "ReadOwnerMapping":
                case "ProvisionLegacyRow":
                case "ResumeLegacyRow":
                case "ReadProvisionedOwner":
                case "ReadRowAccount":
                case "ValidateRowAccount":
                case "ValidatePublicNumber":
                case "ReadNumberMapping":
                case "ValidateNumberMapping":
                case "SortPublicRows":
                case "RecheckAccount":
                case "EndpointSetup": return phase;
                default: return "Unknown";
            }
        }

        private static string SafeQueryFault(string fault)
        {
            switch (fault)
            {
                case null:
                case "": return "None";
                case "ContextMismatch":
                case "InvalidStorageKey":
                case "StoredScopeMismatch":
                case "InvalidStorageResponse":
                case "WriteLockUnavailable":
                case "AccountConflict":
                case "AccountUnavailable":
                case "PublicNumberUnavailable":
                case "PublicNumberExhausted":
                case "LedgerConflict":
                case "LegacyRecordConflict":
                case "EarlierSubmissionPending":
                case "InvalidGeneratedId":
                case "AccountInactive":
                case "ActiveDeviceRequired":
                case "AccountBusy":
                case "LedgerUnavailable":
                case "MissingServerMetadata": return fault;
                default: return "Unknown";
            }
        }

        private async void ProbePhase2(int probe)
        {
            if (!CanRunPhase2() || _account == null || _busy || _screen != null && _screen.IsBusy) return;
            _busy = true;
            _guard.BeginOperation();
            try
            {
                if (probe == 8)
                {
                    // The production repository verifies persisted consent,
                    // scope and SDK Player; the server enforces active authority.
                    // Avoid account-panel completion recovery on each boundary.
                    IOnlineRecordRepository pendingRepository = new CloudCodeRecordRepository(_isolatedLocal.AccountId,
                        () => _isolatedLocal.OnlineAccount, _guard, _guard,
                        () => CanRunPhase2() && _isolatedLocal.CanUseOnlineData);
                    string pendingOutcome = await VerificationPendingRetry.RetryOnceAsync(_isolatedLocal, pendingRepository);
                    _summary = (pendingOutcome == "PENDING_SUBMISSION_CONFIRMED" ? "PASS / " : "FAIL / ") + pendingOutcome;
                    return;
                }
                if (probe == 4 || probe == 7)
                {
                    OnlineAuthenticationResult original = await _guard.TryAuthenticateAsync();
                    if (!original.IsAuthenticated || original.PlayerId != _isolatedLocal.OnlineAccount.PlayerId)
                    { _summary = "FAIL / 원래 인증 세션이 아님"; return; }
                    if (probe == 4)
                    {
                        OnlineAccountResponse status = await _guard.CallAsync<OnlineAccountResponse>("get-account-transfer-status", new System.Collections.Generic.Dictionary<string, string>());
                        _summary = status != null && status.status == "Inactive" ?
                            "OBSERVED / ORIGINAL_INACTIVE_STATUS / 기록 조회는 별도 버튼으로 확인하세요" :
                            "FAIL / ORIGINAL_STATUS_NOT_INACTIVE / 상태 확인 불가 또는 기대 상태 아님";
                        return;
                    }
                    OnlineRecordConfiguration.TryGetLeaderboardId(SelectedBoard(), out string originalBoard);
                    OnlineLeaderboardResult query = await _guard.CallAsync<OnlineLeaderboardResult>("query-records",
                        JsonUtility.ToJson(new OnlineRecordRequest { boardId = originalBoard, kind = "me", limit = 1 }));
                    _summary = query != null && !query.IsSuccess ?
                        "OBSERVED / ORIGINAL_QUERY_FAILED / " + DescribeStageQuery(query) +
                        " / 일반 오류만으로 권한 거부 원인·원자성은 증명하지 않음" : "FAIL / ORIGINAL_QUERY_DID_NOT_FAIL / 조회 실패 관찰 아님";
                    return;
                }
                if (probe == 1)
                {
                    // Read-only preservation check: do not repeat coordinator
                    // completion recovery, fetch bests twice or clear a session.
                    VerificationRecordBaseline baseline = JsonUtility.FromJson<VerificationRecordBaseline>(_baselineInput);
                    OnlineAuthenticationResult authenticated = await _guard.TryAuthenticateAsync();
                    if (!authenticated.IsAuthenticated || authenticated.PlayerId != _isolatedLocal.OnlineAccount.PlayerId)
                    { _summary = "FAIL / BASELINE_AUTHENTICATION_MISMATCH"; return; }
                    OnlineRecordConfiguration.TryGetLeaderboardId(SelectedBoard(), out string boardId);
                    OnlineLeaderboardResult comparedMe = await _guard.CallAsync<OnlineLeaderboardResult>("query-records",
                        JsonUtility.ToJson(new OnlineRecordRequest { boardId = boardId, kind = "me", limit = 1 }));
                    if (!VerificationRecordComparison.IsValidMe(comparedMe))
                    { _summary = "FAIL / BASELINE_QUERY_INVALID / " + DescribeStageQuery(comparedMe); return; }
                    // A nonempty authenticated me row carries the current server
                    // number. Empty rows require a separate server number read.
                    string number;
                    if (comparedMe.entries.Length == 1) number = comparedMe.entries[0].publicPlayerNumber;
                    else
                    {
                        OnlineAccountResponse response = await _guard.CallAsync<OnlineAccountResponse>("get-public-player-number",
                            new System.Collections.Generic.Dictionary<string, string>());
                        if (response == null || response.status != "Success")
                        { _summary = "FAIL / BASELINE_NUMBER_UNAVAILABLE"; return; }
                        number = response.publicPlayerNumber;
                    }
                    string mismatch = VerificationRecordComparison.GetPreservationFailureReason(baseline, comparedMe, number, boardId);
                    _summary = mismatch.Length == 0 ? "PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED" :
                        "FAIL / BASELINE_MISMATCH / " + mismatch;
                    return;
                }
                if (!(await _account.TryAuthenticateAsync()).IsAuthenticated)
                { _summary = "FAIL / ACCOUNT_NOT_READY / " + DescribeAccountReadiness(); return; }
                if (probe == 2 || probe == 5 || probe == 6)
                {
                    RecordBoardKey.TryCreateStage("stage-001", 1, out RecordBoardKey stageBoard);
                    OnlineLeaderboardResult before = await _records.GetPersonalBestAsync(stageBoard);
                    string blocked = VerificationRecordComparison.GetStageProbeBlockReason(before, probe != 2);
                    string queryDiagnostic = DescribeStageQuery(before);
                    if (blocked.Length != 0)
                    { _summary = "FAIL / " + blocked + " / " + queryDiagnostic + " / 이번 요청에서 기록 제출 없음"; return; }
                    if (probe == 6)
                    { _summary = "PASS / STAGE_ME_EMPTY / " + queryDiagnostic + " / 기록 제출 없음"; return; }
                    FlowState.Runtime.Features.RecordSubmissionPolicy.TryCreateStageCandidate(_isolatedLocal.AccountId, Guid.NewGuid().ToString("D"),
                        "stage-001", 1, FlowState.Runtime.Core.E_StageResultType.Cleared, 60, out RecordSubmissionCandidate candidate);
                    if (!_isolatedLocal.TryEnqueuePending(candidate) || !_isolatedLocal.TryCheckpoint()) { _summary = "FAIL / Pending 저장 실패"; return; }
                    await _pending.RetryAllPendingAsync();
                    if (_isolatedLocal.CreatePendingSnapshot().Count != 0 || !_pending.TryGetTerminalResult(candidate.SubmissionId, out OnlineSubmissionResult result) ||
                        result.Result != E_RecordSubmissionResult.Submitted)
                    { _summary = "FAIL / 기록 미확정 또는 거부. 미확정 Pending은 보존합니다."; return; }
                    OnlineLeaderboardResult submitted = await _records.GetPersonalBestAsync(stageBoard);
                    bool hasPreservedBest = before.entries.Length == 1 && before.entries[0].score <= candidate.RankingValue;
                    long expectedScore = hasPreservedBest ? before.entries[0].score : candidate.RankingValue;
                    bool isVerified = VerificationRecordComparison.IsSubmittedMe(submitted, _account.ViewState.NumberText, expectedScore);
                    if (isVerified && hasPreservedBest) isVerified = submitted.entries[0].acceptedAt == before.entries[0].acceptedAt;
                    _summary = isVerified ?
                        "PASS / SERVER_SUBMITTED_AND_ME_VERIFIED / Stage row, expected score and acceptedAt verified" :
                        "FAIL / SERVER_ME_NOT_VERIFIED / 재제출하지 말고 조회 상태를 확인하세요";
                    return;
                }
                if (probe == 3) { await ProbePhase2DirectWrites(); return; }
                OnlineLeaderboardResult current = await _records.GetPersonalBestAsync(SelectedBoard());
                if (!VerificationRecordComparison.IsValidMe(current)) { _summary = "FAIL / 유효한 내 최고 조회 아님"; return; }
                if (probe == 0)
                {
                    string number = _account.ViewState.NumberText;
                    if (!(await _account.RetryPublicNumberAsync()).IsAuthenticated || _account.ViewState.NumberText != number)
                    { _summary = "FAIL / 재조회 번호 불일치"; return; }
                    OnlineRecordConfiguration.TryGetLeaderboardId(SelectedBoard(), out string boardId);
                    _baselineExport = JsonUtility.ToJson(new VerificationRecordBaseline { projectId = OnlineRecordConfiguration.ProjectId,
                        environmentId = OnlineRecordConfiguration.EnvironmentId, boardId = boardId, publicPlayerNumber = number, me = current });
                    _summary = "PASS / NUMBER_STABLE_AND_BASELINE_CAPTURED / 재시작 뒤 같은 기준 자료로 비교하세요";
                }
                else
                {
                    VerificationRecordBaseline baseline = JsonUtility.FromJson<VerificationRecordBaseline>(_baselineInput);
                    OnlineRecordConfiguration.TryGetLeaderboardId(SelectedBoard(), out string boardId);
                    _summary = VerificationRecordComparison.ArePreserved(baseline, current, _account.ViewState.NumberText, boardId) ?
                        "PASS / PUBLIC_ME_SCORE_ACCEPTED_AT_PRESERVED" : "FAIL / 기준·현재 공개 행 불일치";
                }
            }
            catch (Exception) { _summary = "FAIL / 검증 요청 또는 기준 자료 해석 실패. 응답 본문은 출력하지 않습니다."; }
            finally { FinishPhase2Operation(); }
        }

        private async Task ProbePhase2DirectWrites()
        {
            if (!CanRunPhase2()) return;
            string player = _isolatedLocal.OnlineAccount.PlayerId;
            string token = AuthenticationService.Instance.AccessToken;
            long leaderboard = await PostJsonAsync("https://leaderboards.services.api.unity.com/v1/projects/" + OnlineRecordConfiguration.ProjectId +
                "/leaderboards/" + StageLeaderboardId + "/scores/players/" + player,
                "{\"score\":900000,\"metadata\":{\"source\":\"PlayerDirectWriteProbe\"}}", token, _guard.OperationDeadline);
            if (!CanRunPhase2()) return;
            long cloudSave = await PostJsonAsync("https://cloud-save.services.api.unity.com/v1/data/projects/" + OnlineRecordConfiguration.ProjectId +
                "/players/" + player + "/items", "{\"key\":\"" + DirectWriteProbeKey + "\",\"value\":{\"source\":\"PlayerDirectWriteProbe\"}}", token, _guard.OperationDeadline);
            _summary = leaderboard == 403 && cloudSave == 403 ? "PASS / PLAYER_WRITE_DENIED / Leaderboards=403 / CloudSave=403" :
                "FAIL / PLAYER_WRITE_NOT_BOTH_403 / Leaderboards=" + leaderboard + " / CloudSave=" + cloudSave +
                " / 2xx이면 테스트 계정의 probe 오염을 사용자 Dashboard에서 확인·복구";
        }

        private void ExportBaseline()
        {
            if (IsAutomatedExecution() || string.IsNullOrEmpty(_baselineExport)) return;
            string destination = EditorUtility.SaveFilePanel("로컬 전용 기준 자료", _configuration.DirectoryPath, "public-me-baseline", "json");
            if (!string.IsNullOrEmpty(destination) &&
                string.Equals(Path.GetFileName(destination), "flow-state-save.json", StringComparison.OrdinalIgnoreCase))
            { _summary = "FAIL / Local Save 파일에 기준 자료를 덮어쓸 수 없습니다."; return; }
            if (!string.IsNullOrEmpty(destination))
                try { File.WriteAllText(destination, _baselineExport, Encoding.UTF8); }
                catch (Exception) { _summary = "FAIL / 기준 파일 저장 실패"; }
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
            if (IsAutomatedExecution()) throw new InvalidOperationException("Verification remote execution disabled.");
            return await transport.CallAsync<OnlineSubmissionResponse>(
                SubmitRecordEndpoint, JsonUtility.ToJson(request));
        }

        private static async Task<long> PostJsonAsync(string url, string body, string accessToken, Task deadline = null)
        {
            if (IsAutomatedExecution()) throw new InvalidOperationException("Verification remote execution disabled.");
            if (deadline == null) deadline = Task.Delay(VerificationRemoteGuard.OperationTimeoutMilliseconds);
            if (deadline.IsCompleted) throw new TimeoutException();
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.timeout = 5;
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
                void HandleCompleted(AsyncOperation ignored) { completion.TrySetResult(true); }
                operation.completed += HandleCompleted;
                if (operation.isDone) completion.TrySetResult(true);
                try
                {
                    if (await Task.WhenAny(completion.Task, deadline) != completion.Task)
                    { request.Abort(); throw new TimeoutException(); }
                    await completion.Task;
                    if (deadline.IsCompleted) throw new TimeoutException();
                }
                finally { operation.completed -= HandleCompleted; }
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
