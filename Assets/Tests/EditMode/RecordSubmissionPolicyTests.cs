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
        public void TryCreateStageCandidate_ClearedResult_IsRetired()
        {
            bool didCreate = RecordSubmissionPolicy.TryCreateStageCandidate(
                PlayerId,
                SubmissionId,
                StageId,
                1,
                E_StageResultType.Cleared,
                12.3455,
                out RecordSubmissionCandidate candidate);

            Assert.That(didCreate, Is.False);
            Assert.That(candidate, Is.Null);
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
            RecordSubmissionCandidate candidate = CreateInfiniteQueueCandidate(
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
            RecordSubmissionCandidate candidate = CreateInfiniteQueueCandidate(
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
            Assert.That(queue.TryEnqueue(CreateInfiniteQueueCandidate(
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
            Assert.That(queue.TryEnqueue(CreateInfiniteQueueCandidate(
                PlayerId,
                SubmissionId,
                12.0)), Is.True);

            Assert.That(queue.TryBeginSubmission("player-b", SubmissionId), Is.False);
            Assert.That(queue.TryBeginSubmission(PlayerId, SubmissionId), Is.True);
        }

        [TestCase(-1, 0, -1)]
        [TestCase(0, -1, -1)]
        [TestCase(60, 5, 64)]
        public void TryCreateInfiniteCandidate_InvalidComponents_AreRejected(int distance, int collectibles, int total)
        {
            Assert.That(RecordSubmissionPolicy.TryCreateInfiniteCandidate(PlayerId, SubmissionId,
                ScoringVersion.Current, 1000, distance, 0, distance, collectibles, total, 1.0,
                CreateScoreLimit(), out RecordSubmissionCandidate candidate), Is.False);
            Assert.That(candidate, Is.Null);
        }

        [TestCase("", SubmissionId)]
        [TestCase(PlayerId, "not-a-uuid")]
        [TestCase(PlayerId, "00000000-0000-1000-8000-000000000001")]
        public void TryCreateInfiniteCandidate_InvalidIdentity_IsRejected(string owner, string id)
        {
            Assert.That(RecordSubmissionPolicy.TryCreateInfiniteCandidate(owner, id,
                ScoringVersion.Current, 1000, 60, 0, 60, 0, 60, 1.0,
                CreateScoreLimit(), out RecordSubmissionCandidate candidate), Is.False);
            Assert.That(candidate, Is.Null);
        }

        [TestCase(E_GameMode.Stage, 1, "stage-001")]
        [TestCase((E_GameMode)999, 2, "")]
        [TestCase(E_GameMode.Infinite, 1, "")]
        [TestCase(E_GameMode.Infinite, 2, "stage-001")]
        public void StoreCandidate_UnsupportedRecord_DoesNotCallRepository(
            E_GameMode mode, int rulesVersion, string stageId)
        {
            CountingRepository repository = new CountingRepository();
            RecordSubmissionService service = new RecordSubmissionService(repository);
            Assert.That(service.TryStoreCandidate(SyntheticCandidate(mode, rulesVersion, stageId)), Is.False);
            Assert.That(repository.EnqueueCalls, Is.Zero);
            Assert.That(repository.BestCalls, Is.Zero);
        }

        [Test]
        public void StoreCandidate_Null_DoesNotCallRepository()
        {
            CountingRepository repository = new CountingRepository();
            Assert.That(new RecordSubmissionService(repository).TryStoreCandidate(null), Is.False);
            Assert.That(repository.EnqueueCalls, Is.Zero);
            Assert.That(repository.BestCalls, Is.Zero);
        }

        [TestCase(E_GameMode.Stage, 1, "stage-001")]
        [TestCase((E_GameMode)999, 2, "")]
        [TestCase(E_GameMode.Infinite, 1, "")]
        [TestCase(E_GameMode.Infinite, 2, "stage-001")]
        public void LocalNewWrites_UnsupportedRecord_KeepClockMemoryAndFileUnchanged(
            E_GameMode mode, int rulesVersion, string stageId)
        {
            int clockCalls = 0;
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = new LocalRecordRepository(file, now: () => { clockCalls++; return 1000; });
            Assert.That(local.TryLoad(out LocalSaveData ignored), Is.True);
            RecordSubmissionCandidate valid = CreateInfiniteQueueCandidate(PlayerId, SubmissionId, 1);
            Assert.That(new RecordSubmissionService(local).TryStoreCandidate(valid), Is.True);
            Assert.That(local.TrySave(local.CreateSaveData(PlayerId, new LocalSettingsData(75, true, null), true)), Is.True);
            string before = file.Contents;
            int writes = file.WriteAttempts;
            clockCalls = 0;
            RecordSubmissionCandidate unsupported = SyntheticCandidate(mode, rulesVersion, stageId);
            Assert.That(local.TryEnqueuePending(unsupported), Is.False);
            Assert.That(local.TryUpdatePersonalBest(unsupported), Is.False);
            Assert.That(new RecordSubmissionService(local).TryStoreCandidate(unsupported), Is.False);
            Assert.That(clockCalls, Is.Zero);
            Assert.That(file.WriteAttempts, Is.EqualTo(writes));
            Assert.That(file.Contents, Is.EqualTo(before));
            Assert.That(local.CreatePendingSnapshot().Count, Is.EqualTo(1));
            Assert.That(local.CreatePendingSnapshot()[0].SubmissionId, Is.EqualTo(SubmissionId));
            Assert.That(local.TryGetPersonalBest(PlayerId, valid.BoardKey, out RecordSubmissionCandidate best), Is.True);
            Assert.That(best, Is.SameAs(valid));
            Assert.That(LocalSaveJsonCodec.Serialize(local.CreateSaveData(PlayerId,
                new LocalSettingsData(75, true, null), true)), Is.EqualTo(before));
        }

        [Test]
        public void LocalNewWrites_Null_DoNotReadClockOrWriteFile()
        {
            int clockCalls = 0;
            OnlineTestFileStore file = new OnlineTestFileStore();
            LocalRecordRepository local = new LocalRecordRepository(file, now: () => { clockCalls++; return 1000; });
            Assert.That(local.TryLoad(out LocalSaveData ignored), Is.True);
            Assert.That(local.TryEnqueuePending(null), Is.False);
            Assert.That(local.TryUpdatePersonalBest(null), Is.False);
            Assert.That(clockCalls, Is.Zero);
            Assert.That(file.WriteAttempts, Is.Zero);
            Assert.That(local.CreatePendingSnapshot(), Is.Empty);
        }

        private static RecordSubmissionCandidate SyntheticCandidate(E_GameMode mode, int rulesVersion, string stageId)
        {
            RecordBoardKey board = (RecordBoardKey)System.Activator.CreateInstance(typeof(RecordBoardKey),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null,
                new object[] { mode, stageId, rulesVersion }, null);
            return (RecordSubmissionCandidate)System.Activator.CreateInstance(typeof(RecordSubmissionCandidate),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null,
                new object[] { PlayerId, "00000000-0000-4000-8000-000000000002", board,
                    100L, 0L, 0, 0, 0, 0, 0, 1.0, 0L }, null);
        }

        private sealed class CountingRepository : IRecordRepository
        {
            public int EnqueueCalls;
            public int BestCalls;
            public bool TryEnqueuePending(RecordSubmissionCandidate candidate) { EnqueueCalls++; return true; }
            public bool TryUpdatePersonalBest(RecordSubmissionCandidate candidate) { BestCalls++; return true; }
            public bool TryGetPersonalBest(string owner, RecordBoardKey board, out RecordSubmissionCandidate candidate)
            { candidate = null; return false; }
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

        private static RecordSubmissionCandidate CreateInfiniteQueueCandidate(
            string playerId,
            string submissionId,
            double elapsedTime)
        {
            Assert.That(RecordSubmissionPolicy.TryCreateInfiniteCandidate(playerId, submissionId,
                ScoringVersion.Current, 60000, 60, 0, 60, 0, 60, 1.0,
                new InfiniteScoreLimit(ScoringVersion.Current, 2, 10, 3, 2, 1, 5), out RecordSubmissionCandidate candidate), Is.True);
            return candidate;
        }
    }
}
