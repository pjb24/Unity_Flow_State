using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public interface IOnlineRecordRepository
    {
        Task<OnlineSubmissionResult> SubmitAsync(RecordSubmissionCandidate candidate);
        Task<OnlineLeaderboardResult> GetTopAsync(RecordBoardKey boardKey, int limit = 20);
        Task<OnlineLeaderboardResult> GetAroundAsync(RecordBoardKey boardKey, int limit = 20);
        Task<OnlineLeaderboardResult> GetPersonalBestAsync(RecordBoardKey boardKey);
    }
}
