using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class OnlineRecordConfigurationTests
    {
        [Test]
        public void TryGetLeaderboardId_MapsCurrentStageBoard()
        {
            Assert.That(
                RecordBoardKey.TryCreateStage(
                    StageRecordIdentity.CurrentStageId,
                    StageRecordIdentity.CurrentRulesVersion,
                    out RecordBoardKey boardKey),
                Is.True);

            Assert.That(
                OnlineRecordConfiguration.TryGetLeaderboardId(
                    boardKey,
                    out string leaderboardId),
                Is.True);
            Assert.That(leaderboardId, Is.EqualTo("fs-stage-stage-001-r1"));
        }

        [Test]
        public void TryGetLeaderboardId_MapsCurrentInfiniteBoard()
        {
            Assert.That(
                RecordBoardKey.TryCreateInfinite(
                    ScoringVersion.Current,
                    out RecordBoardKey boardKey),
                Is.True);

            Assert.That(
                OnlineRecordConfiguration.TryGetLeaderboardId(
                    boardKey,
                    out string leaderboardId),
                Is.True);
            Assert.That(leaderboardId, Is.EqualTo("fs-infinite-v2"));
        }

        [Test]
        public void TryGetLeaderboardId_RejectsDifferentStageRulesVersion()
        {
            Assert.That(
                RecordBoardKey.TryCreateStage(
                    StageRecordIdentity.CurrentStageId,
                    StageRecordIdentity.CurrentRulesVersion + 1,
                    out RecordBoardKey boardKey),
                Is.True);

            Assert.That(
                OnlineRecordConfiguration.TryGetLeaderboardId(
                    boardKey,
                    out string leaderboardId),
                Is.False);
            Assert.That(leaderboardId, Is.Empty);
        }

        [Test]
        public void TryGetLeaderboardId_RejectsDifferentStageId()
        {
            Assert.That(
                RecordBoardKey.TryCreateStage(
                    "stage-002",
                    StageRecordIdentity.CurrentRulesVersion,
                    out RecordBoardKey boardKey),
                Is.True);

            Assert.That(
                OnlineRecordConfiguration.TryGetLeaderboardId(
                    boardKey,
                    out string leaderboardId),
                Is.False);
            Assert.That(leaderboardId, Is.Empty);
        }

        [Test]
        public void TryGetLeaderboardId_RejectsLegacyInfiniteScoringVersion()
        {
            Assert.That(
                RecordBoardKey.TryCreateInfinite(
                    ScoringVersion.LegacyDistanceScore,
                    out RecordBoardKey boardKey),
                Is.True);

            Assert.That(
                OnlineRecordConfiguration.TryGetLeaderboardId(
                    boardKey,
                    out string leaderboardId),
                Is.False);
            Assert.That(leaderboardId, Is.Empty);
        }
    }
}
