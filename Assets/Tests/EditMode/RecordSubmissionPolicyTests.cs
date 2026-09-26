using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class RecordSubmissionPolicyTests
    {
        private const string PlayerId = "player-a";
        private const string SubmissionId = "00000000-0000-4000-8000-000000000001";
        private const string StageId = "stage-001";

        [Test]
        public void TryCreateStageCandidate_ClearedResult_RoundsToMilliseconds()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId,
                SubmissionId,
                StageId,
                1,
                E_StageResultType.Cleared,
                12.3455,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.True);
            Assert.That(candidate.BoardKey.GameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(candidate.BoardKey.StageId, Is.EqualTo(StageId));
            Assert.That(candidate.BoardKey.RulesVersion, Is.EqualTo(1));
            Assert.That(candidate.RankingValue, Is.EqualTo(12346));
        }

        [Test]
        public void TryCreateStageCandidate_FellResult_IsRejected()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId,
                SubmissionId,
                StageId,
                1,
                E_StageResultType.Fell,
                12.0,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void TryCreateStageCandidate_NonVersionFourSubmissionId_IsRejected()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId,
                "00000000-0000-1000-8000-000000000001",
                StageId,
                1,
                E_StageResultType.Cleared,
                12.0,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void TryCreateStageCandidate_InvalidRulesVersion_IsRejected(
            int stageRulesVersion)
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId,
                SubmissionId,
                StageId,
                stageRulesVersion,
                E_StageResultType.Cleared,
                12.0,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(-0.001)]
        public void TryCreateStageCandidate_InvalidElapsedTime_IsRejected(
            double elapsedTime)
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId,
                SubmissionId,
                StageId,
                1,
                E_StageResultType.Cleared,
                elapsedTime,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void TryCreateInfiniteCandidate_AtLogicalMaximum_IsCreated()
        {
            InfiniteScoreLimit scoreLimit = CreateScoreLimit();

            bool didCreate = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                PlayerId,
                SubmissionId,
                ScoringVersion.Current,
                1000,
                20,
                40,
                60,
                5,
                65,
                3.0,
                scoreLimit,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.True);
            Assert.That(candidate.BoardKey.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(candidate.BoardKey.RulesVersion, Is.EqualTo(ScoringVersion.Current));
            Assert.That(candidate.RankingValue, Is.EqualTo(65));
            Assert.That(candidate.RunDurationMilliseconds, Is.EqualTo(1000));
        }

        [Test]
        public void TryCreateInfiniteCandidate_AboveLogicalMaximum_IsRejected()
        {
            InfiniteScoreLimit scoreLimit = CreateScoreLimit();

            bool didCreate = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                PlayerId,
                SubmissionId,
                ScoringVersion.Current,
                1000,
                20,
                40,
                60,
                6,
                66,
                3.0,
                scoreLimit,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void TryCreateInfiniteCandidate_MismatchedScoreComponents_IsRejected()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                PlayerId,
                SubmissionId,
                ScoringVersion.Current,
                1000,
                20,
                40,
                61,
                5,
                66,
                3.0,
                CreateScoreLimit(),
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void TryCreateInfiniteCandidate_MissingScoreLimit_IsRejected()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                PlayerId,
                SubmissionId,
                ScoringVersion.Current,
                1000,
                20,
                40,
                60,
                5,
                65,
                3.0,
                null,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void TryCreateInfiniteCandidate_UnsupportedVersion_IsRejected()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateInfiniteCandidate(
                PlayerId,
                SubmissionId,
                ScoringVersion.LegacyDistanceScore,
                1000,
                20,
                40,
                60,
                5,
                65,
                3.0,
                CreateScoreLimit(),
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void RecordSubmissionQueue_DuplicatePlayerAndSubmissionId_IsRejected()
        {
            RecordSubmissionQueue queue = new RecordSubmissionQueue();
            RecordSubmissionCandidate candidate = CreateStageCandidate(
                PlayerId,
                SubmissionId,
                12.0);

            Assert.That(queue.TryEnqueue(candidate), Is.True);
            Assert.That(queue.TryEnqueue(candidate), Is.False);
            Assert.That(queue.Count, Is.EqualTo(1));
        }

        [Test]
        public void RecordSubmissionQueue_TransientFailuresAreLimitedThenResetByRetryTrigger()
        {
            RecordSubmissionQueue queue = new RecordSubmissionQueue();
            RecordSubmissionCandidate candidate = CreateStageCandidate(
                PlayerId,
                SubmissionId,
                12.0);
            Assert.That(queue.TryEnqueue(candidate), Is.True);

            for (int i = 0; i < RecordSubmissionQueue.MaximumAttemptsPerRetryTrigger; i++)
            {
                Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.True);
                Assert.That(
                    queue.TryCompleteSubmission(
                        PlayerId,
                        SubmissionId,
                        E_RecordSubmissionResult.TransientFailure),
                    Is.True);
            }

            Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.False);
            queue.BeginRetryTrigger();
            Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.True);
        }

        [Test]
        public void RecordSubmissionQueue_RejectedEntryDoesNotRetry()
        {
            RecordSubmissionQueue queue = new RecordSubmissionQueue();
            Assert.That(queue.TryEnqueue(CreateStageCandidate(
                PlayerId,
                SubmissionId,
                12.0)), Is.True);
            Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.True);
            Assert.That(queue.TryCompleteSubmission(
                PlayerId,
                SubmissionId,
                E_RecordSubmissionResult.Rejected), Is.True);

            queue.BeginRetryTrigger();

            Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.False);
        }

        [Test]
        public void RecordSubmissionQueue_OtherPlayerCannotSubmitOwnedEntry()
        {
            RecordSubmissionQueue queue = new RecordSubmissionQueue();
            Assert.That(queue.TryEnqueue(CreateStageCandidate(
                PlayerId,
                SubmissionId,
                12.0)), Is.True);

            Assert.That(queue.TryBeginSubmission("player-b", SubmissionId), Is.False);
            Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.True);
        }

        private static InfiniteScoreLimit CreateScoreLimit()
        {
            return new InfiniteScoreLimit(
                ScoringVersion.Current,
                2.0,
                10.0,
                3.0,
                2.0,
                1,
                5);
        }

        private static RecordSubmissionCandidate CreateStageCandidate(
            string playerId,
            string submissionId,
            double elapsedTime)
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                playerId,
                submissionId,
                StageId,
                1,
                E_StageResultType.Cleared,
                elapsedTime,
                out RecordSubmissionCandidate candidate);
            Assert.That(didCreate, Is.True);
            return candidate;
        }
    }
}
