using System.Collections.Generic;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class RecordLeaderboardPolicyTests
    {
        [Test]
        public void TryCompare_StageUsesAscendingClearTime()
        {
            RecordLeaderboardEntry faster = CreateStageEntry(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                "stage-001",
                1,
                10.0,
                10);
            RecordLeaderboardEntry slower = CreateStageEntry(
                "player-b",
                "00000000-0000-4000-8000-000000000002",
                "stage-001",
                1,
                12.0,
                5);

            bool didCompare = RecordLeaderboardPolicy.TryCompare(
                faster,
                slower,
                out int comparison);

            Assert.That(didCompare, Is.True);
            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void TryCompare_InfiniteUsesDescendingTotalScore()
        {
            RecordLeaderboardEntry higherScore = CreateInfiniteEntry(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                65,
                10);
            RecordLeaderboardEntry lowerScore = CreateInfiniteEntry(
                "player-b",
                "00000000-0000-4000-8000-000000000002",
                60,
                5);

            bool didCompare = RecordLeaderboardPolicy.TryCompare(
                higherScore,
                lowerScore,
                out int comparison);

            Assert.That(didCompare, Is.True);
            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void TryCompare_SameRankingValueUsesServerAcceptedTime()
        {
            RecordLeaderboardEntry earlier = CreateStageEntry(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                "stage-001",
                1,
                10.0,
                5);
            RecordLeaderboardEntry later = CreateStageEntry(
                "player-b",
                "00000000-0000-4000-8000-000000000002",
                "stage-001",
                1,
                10.0,
                10);

            Assert.That(
                RecordLeaderboardPolicy.TryCompare(
                    earlier,
                    later,
                    out int comparison),
                Is.True);
            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void TryGetCompetitionRank_CompleteTieSharesRankAndSkipsNextRank()
        {
            RecordLeaderboardEntry first = CreateStageEntry(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                "stage-001",
                1,
                10.0,
                5);
            RecordLeaderboardEntry tied = CreateStageEntry(
                "player-b",
                "00000000-0000-4000-8000-000000000002",
                "stage-001",
                1,
                10.0,
                5);
            RecordLeaderboardEntry slower = CreateStageEntry(
                "player-c",
                "00000000-0000-4000-8000-000000000003",
                "stage-001",
                1,
                11.0,
                10);
            List<RecordLeaderboardEntry> entries =
                new List<RecordLeaderboardEntry> { first, tied, slower };

            Assert.That(
                RecordLeaderboardPolicy.TryGetCompetitionRank(
                    entries,
                    first,
                    out int firstRank),
                Is.True);
            Assert.That(
                RecordLeaderboardPolicy.TryGetCompetitionRank(
                    entries,
                    tied,
                    out int tiedRank),
                Is.True);
            Assert.That(
                RecordLeaderboardPolicy.TryGetCompetitionRank(
                    entries,
                    slower,
                    out int slowerRank),
                Is.True);
            Assert.That(firstRank, Is.EqualTo(1));
            Assert.That(tiedRank, Is.EqualTo(1));
            Assert.That(slowerRank, Is.EqualTo(3));
        }

        [Test]
        public void TryCompare_DifferentBoardKeysIsRejected()
        {
            RecordLeaderboardEntry first = CreateStageEntry(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                "stage-001",
                1,
                10.0,
                5);
            RecordLeaderboardEntry second = CreateStageEntry(
                "player-b",
                "00000000-0000-4000-8000-000000000002",
                "stage-002",
                1,
                10.0,
                5);

            Assert.That(
                RecordLeaderboardPolicy.TryCompare(first, second, out int comparison),
                Is.False);
            Assert.That(comparison, Is.Zero);
        }

        [Test]
        public void ShouldReplaceBest_StageRequiresStrictlyShorterTime()
        {
            RecordSubmissionCandidate current = CreateStageCandidate(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                "stage-001",
                1,
                10.0);
            RecordSubmissionCandidate equal = CreateStageCandidate(
                "player-a",
                "00000000-0000-4000-8000-000000000002",
                "stage-001",
                1,
                10.0);
            RecordSubmissionCandidate better = CreateStageCandidate(
                "player-a",
                "00000000-0000-4000-8000-000000000003",
                "stage-001",
                1,
                9.0);

            Assert.That(
                RecordLeaderboardPolicy.ShouldReplaceBest(current, equal),
                Is.False);
            Assert.That(
                RecordLeaderboardPolicy.ShouldReplaceBest(current, better),
                Is.True);
        }

        [Test]
        public void ShouldReplaceBest_InfiniteRequiresStrictlyHigherScore()
        {
            RecordSubmissionCandidate current = CreateInfiniteCandidate(
                "player-a",
                "00000000-0000-4000-8000-000000000001",
                60);
            RecordSubmissionCandidate equal = CreateInfiniteCandidate(
                "player-a",
                "00000000-0000-4000-8000-000000000002",
                60);
            RecordSubmissionCandidate better = CreateInfiniteCandidate(
                "player-a",
                "00000000-0000-4000-8000-000000000003",
                65);

            Assert.That(
                RecordLeaderboardPolicy.ShouldReplaceBest(current, equal),
                Is.False);
            Assert.That(
                RecordLeaderboardPolicy.ShouldReplaceBest(current, better),
                Is.True);
        }

        private static RecordLeaderboardEntry CreateStageEntry(
            string playerId,
            string submissionId,
            string stageId,
            int stageRulesVersion,
            double elapsedTime,
            long serverAcceptedAtTicks)
        {
            return new RecordLeaderboardEntry(
                CreateStageCandidate(
                    playerId,
                    submissionId,
                    stageId,
                    stageRulesVersion,
                    elapsedTime),
                serverAcceptedAtTicks);
        }

        private static RecordSubmissionCandidate CreateStageCandidate(
            string playerId,
            string submissionId,
            string stageId,
            int stageRulesVersion,
            double elapsedTime)
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                playerId,
                submissionId,
                stageId,
                stageRulesVersion,
                E_StageResultType.Cleared,
                elapsedTime,
                out RecordSubmissionCandidate candidate);
            Assert.That(didCreate, Is.True);
            return candidate;
        }

        private static RecordLeaderboardEntry CreateInfiniteEntry(
            string playerId,
            string submissionId,
            int totalScore,
            long serverAcceptedAtTicks)
        {
            return new RecordLeaderboardEntry(
                CreateInfiniteCandidate(playerId, submissionId, totalScore),
                serverAcceptedAtTicks);
        }

        private static RecordSubmissionCandidate CreateInfiniteCandidate(
            string playerId,
            string submissionId,
            int totalScore)
        {
            int collectibleScore = totalScore - 60;
            bool didCreate = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                playerId,
                submissionId,
                ScoringVersion.Current,
                1000,
                20,
                40,
                60,
                collectibleScore,
                totalScore,
                3.0,
                new InfiniteScoreLimit(
                    ScoringVersion.Current,
                    2.0,
                    10.0,
                    3.0,
                    2.0,
                    2,
                    5),
                out RecordSubmissionCandidate candidate);
            Assert.That(didCreate, Is.True);
            return candidate;
        }
    }
}
