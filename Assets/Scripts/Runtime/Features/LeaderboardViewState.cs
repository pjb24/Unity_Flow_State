using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    // Keeps independently requested sections from replacing one another with late responses.
    public sealed class LeaderboardViewState
    {
        private int _topRequestVersion;
        private int _aroundRequestVersion;
        private int _personalBestRequestVersion;

        public E_GameMode GameMode { get; private set; } = E_GameMode.Stage;
        public E_LeaderboardQueryState TopState { get; private set; } =
            E_LeaderboardQueryState.NotRequested;
        public E_LeaderboardQueryState AroundState { get; private set; } =
            E_LeaderboardQueryState.NotRequested;
        public OnlineLeaderboardResult TopResult { get; private set; }
        public OnlineLeaderboardResult AroundResult { get; private set; }
        public OnlineLeaderboardResult PersonalBestResult { get; private set; }

        public void SelectGameMode(E_GameMode gameMode)
        {
            if (GameMode == gameMode)
            {
                return;
            }

            GameMode = gameMode;
            Invalidate();
        }

        public void Invalidate()
        {
            _topRequestVersion++;
            _aroundRequestVersion++;
            _personalBestRequestVersion++;
            TopState = E_LeaderboardQueryState.NotRequested;
            AroundState = E_LeaderboardQueryState.NotRequested;
            TopResult = null;
            AroundResult = null;
            PersonalBestResult = null;
        }

        public int BeginTopRequest()
        {
            TopState = E_LeaderboardQueryState.Loading;
            TopResult = null;
            _topRequestVersion++;
            return _topRequestVersion;
        }

        public int BeginAroundRequest()
        {
            AroundState = E_LeaderboardQueryState.Loading;
            AroundResult = null;
            _aroundRequestVersion++;
            return _aroundRequestVersion;
        }

        public void CompleteTopRequest(int version, OnlineLeaderboardResult result)
        {
            if (version != _topRequestVersion)
            {
                return;
            }

            TopResult = result;
            TopState = ToQueryState(result);
        }

        public void CompleteAroundRequest(int version, OnlineLeaderboardResult result)
        {
            if (version != _aroundRequestVersion)
            {
                return;
            }

            AroundResult = result;
            AroundState = ToQueryState(result);
        }

        public int BeginPersonalBestRequest()
        {
            _personalBestRequestVersion++;
            PersonalBestResult = null;
            return _personalBestRequestVersion;
        }

        public void CompletePersonalBestRequest(int version, OnlineLeaderboardResult result)
        {
            if (version == _personalBestRequestVersion)
            {
                PersonalBestResult = result;
            }
        }

        private static E_LeaderboardQueryState ToQueryState(OnlineLeaderboardResult result)
        {
            if (result == null || !result.IsSuccess)
            {
                return result != null && result.reason == "NotReachable"
                    ? E_LeaderboardQueryState.Offline
                    : E_LeaderboardQueryState.Error;
            }

            return result.entries != null && result.entries.Length > 0
                ? E_LeaderboardQueryState.Success
                : E_LeaderboardQueryState.Empty;
        }
    }
}
