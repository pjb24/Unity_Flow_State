using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class LeaderboardViewStateTests
    {
        [Test]
        public void IndependentRequests_KeepSuccessfulTopWhenAroundFails()
        {
            LeaderboardViewState state = new LeaderboardViewState();
            int topVersion = state.BeginTopRequest();
            int aroundVersion = state.BeginAroundRequest();

            state.CompleteTopRequest(topVersion, new OnlineLeaderboardResult
            {
                status = "Success",
                entries = new[] { new OnlineLeaderboardEntry { rank = 1 } }
            });
            state.CompleteAroundRequest(aroundVersion, new OnlineLeaderboardResult
            {
                status = "TransientFailure", reason = "Timeout"
            });

            Assert.That(state.TopState, Is.EqualTo(E_LeaderboardQueryState.Success));
            Assert.That(state.AroundState, Is.EqualTo(E_LeaderboardQueryState.Error));
        }

        [Test]
        public void OlderResponse_DoesNotReplaceNewerRequest()
        {
            LeaderboardViewState state = new LeaderboardViewState();
            int first = state.BeginTopRequest();
            int second = state.BeginTopRequest();

            state.CompleteTopRequest(first, new OnlineLeaderboardResult
            {
                status = "Success",
                entries = new[] { new OnlineLeaderboardEntry { rank = 1 } }
            });
            Assert.That(state.TopState, Is.EqualTo(E_LeaderboardQueryState.Loading));

            state.CompleteTopRequest(second, new OnlineLeaderboardResult
            {
                status = "Success", entries = new OnlineLeaderboardEntry[0]
            });
            Assert.That(state.TopState, Is.EqualTo(E_LeaderboardQueryState.Empty));
        }

        [Test]
        public void ModeChange_InvalidatesInFlightRequests()
        {
            LeaderboardViewState state = new LeaderboardViewState();
            int request = state.BeginAroundRequest();
            state.SelectGameMode(E_GameMode.Infinite);
            state.CompleteAroundRequest(request, new OnlineLeaderboardResult
            {
                status = "Success",
                entries = new[] { new OnlineLeaderboardEntry { rank = 1 } }
            });

            Assert.That(state.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(state.AroundState, Is.EqualTo(E_LeaderboardQueryState.NotRequested));
        }

        [Test]
        public void ModeChange_InvalidatesInFlightPersonalBest()
        {
            LeaderboardViewState state = new LeaderboardViewState();
            int request = state.BeginPersonalBestRequest();
            state.SelectGameMode(E_GameMode.Infinite);
            state.CompletePersonalBestRequest(request, new OnlineLeaderboardResult
            {
                status = "Success",
                entries = new[] { new OnlineLeaderboardEntry { rank = 1, score = 16 } }
            });

            Assert.That(state.PersonalBestResult, Is.Null);
        }

        [Test]
        public void OfflineResponse_UsesOfflineStateWithoutReplacingSuccessfulTop()
        {
            LeaderboardViewState state = new LeaderboardViewState();
            int topVersion = state.BeginTopRequest();
            int aroundVersion = state.BeginAroundRequest();
            state.CompleteTopRequest(topVersion, new OnlineLeaderboardResult
            {
                status = "Success",
                entries = new[] { new OnlineLeaderboardEntry { rank = 1 } }
            });

            state.CompleteAroundRequest(aroundVersion, new OnlineLeaderboardResult
            {
                status = "TransientFailure", reason = "NotReachable"
            });

            Assert.That(state.TopState, Is.EqualTo(E_LeaderboardQueryState.Success));
            Assert.That(state.AroundState, Is.EqualTo(E_LeaderboardQueryState.Offline));
        }
    }
}
