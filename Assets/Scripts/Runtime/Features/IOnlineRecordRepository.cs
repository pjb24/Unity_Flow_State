using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public interface IOnlineRecordRepository
    {
        Task<OnlineSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate);
        Task<OnlineLeaderboardResult> GetTopAsync(RecordBoardKey boardKey, int limit = 10);
        Task<OnlineLeaderboardResult> GetAroundAsync(RecordBoardKey boardKey, int limit = 7);
        Task<OnlineLeaderboardResult> GetPersonalBestAsync(RecordBoardKey boardKey);
    }
}
