using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    // The coordinator supplies the persisted consent/account binding; every call rechecks identity.
    public sealed class CloudCodeRecordRepository : IOnlineRecordRepository
    {
        private readonly string _localOwner;
        private readonly Func<OnlineAccountState> _account;
        private readonly IOnlineAuthenticationGateway _authentication;
        private readonly IOnlineRecordTransport _transport;

        public CloudCodeRecordRepository(string localOwner, Func<OnlineAccountState> account,
            IOnlineAuthenticationGateway authentication, IOnlineRecordTransport transport)
        {
            _localOwner = localOwner;
            _account = account;
            _authentication = authentication;
            _transport = transport;
        }

        public async Task<E_RecordSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate)
        {
            if (candidate == null || candidate.PlayerId != _localOwner)
                return E_RecordSubmissionResult.TransientFailure;
            if (!OnlineRecordConfiguration.TryGetLeaderboardId(candidate.BoardKey, out string boardId))
                return E_RecordSubmissionResult.Rejected;
            try
            {
                if (!await AuthenticateAsync()) return E_RecordSubmissionResult.TransientFailure;
                OnlineRecordRequest request = new OnlineRecordRequest
                {
                    boardId = boardId, rulesVersion = candidate.BoardKey.RulesVersion,
                    submissionId = candidate.SubmissionId, score = candidate.RankingValue,
                    runDurationMilliseconds = candidate.RunDurationMilliseconds,
                    baseDistanceScore = candidate.BaseDistanceScore, momentumBonus = candidate.MomentumBonus,
                    distanceScore = candidate.DistanceScore, collectibleScore = candidate.CollectibleScore,
                    totalScore = candidate.TotalScore, maximumMomentumMultiplier = candidate.MaximumMomentumMultiplier
                };
                OnlineSubmissionResponse response = await _transport.CallAsync<OnlineSubmissionResponse>(
                    "submit-record", JsonUtility.ToJson(request));
                if (response != null && response.status == "Submitted") return E_RecordSubmissionResult.Submitted;
                if (response != null && response.status == "Rejected") return E_RecordSubmissionResult.Rejected;
            }
            catch (Exception)
            {
                Debug.LogWarning("[CloudCodeRecordRepository] Submission unavailable; pending retained.");
            }
            return E_RecordSubmissionResult.TransientFailure;
        }

        public Task<OnlineLeaderboardResult> GetTopAsync(RecordBoardKey boardKey, int limit = 20)
        {
            return QueryAsync(boardKey, "top", limit);
        }

        public Task<OnlineLeaderboardResult> GetAroundAsync(RecordBoardKey boardKey, int limit = 20)
        {
            return QueryAsync(boardKey, "around", limit);
        }

        public Task<OnlineLeaderboardResult> GetPersonalBestAsync(RecordBoardKey boardKey)
        {
            return QueryAsync(boardKey, "me", 1);
        }

        private async Task<bool> AuthenticateAsync()
        {
            OnlineAccountState account = _account();
            if (!account.HasConfirmedRecoveryNotice || string.IsNullOrEmpty(account.PlayerId)) return false;
            OnlineAuthenticationResult result = await _authentication.TryAuthenticateAsync();
            return result.IsAuthenticated && result.PlayerId == account.PlayerId;
        }

        private async Task<OnlineLeaderboardResult> QueryAsync(RecordBoardKey boardKey, string kind, int limit)
        {
            if (limit < 1 || limit > 20 ||
                !OnlineRecordConfiguration.TryGetLeaderboardId(boardKey, out string boardId))
                return new OnlineLeaderboardResult { status = "TransientFailure", reason = "InvalidQuery" };
            try
            {
                if (await AuthenticateAsync())
                {
                    OnlineLeaderboardResult response = await _transport.CallAsync<OnlineLeaderboardResult>(
                        "query-records", JsonUtility.ToJson(new OnlineRecordRequest
                        { boardId = boardId, kind = kind, limit = limit }));
                    if (response != null) return response;
                    return new OnlineLeaderboardResult { status = "TransientFailure", reason = "EmptyResponse" };
                }
                return new OnlineLeaderboardResult { status = "TransientFailure", reason = "AuthenticationUnavailable" };
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("[CloudCodeRecordRepository] Query timed out.");
                return new OnlineLeaderboardResult { status = "TransientFailure", reason = "Timeout" };
            }
            catch (RequestFailedException exception)
            {
                Debug.LogWarning("[CloudCodeRecordRepository] Query request failed. Code: " + exception.ErrorCode);
                return new OnlineLeaderboardResult
                {
                    status = "TransientFailure", reason = "RequestFailed:" + exception.ErrorCode
                };
            }
            catch (Exception)
            {
                Debug.LogWarning("[CloudCodeRecordRepository] Query unavailable.");
                return new OnlineLeaderboardResult { status = "TransientFailure", reason = "ClientFailure" };
            }
        }
    }
}
