using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ScoreRecordTests
    {
        private const float FinalDistance = 123.45f;
        private const int FinalScore = 1234;

        private ScoreRecord _scoreRecord;

        [SetUp]
        public void SetUp()
        {
            _scoreRecord = new ScoreRecord();
        }

        [Test]
        public void NewRecord_HasNoResultData()
        {
            Assert.That(_scoreRecord.HasRecord, Is.False);
            Assert.That(_scoreRecord.ResultData, Is.Null);
        }

        [Test]
        public void TryRecord_EndedFinalizedInfiniteRun_CreatesResultData()
        {
            bool didRecord = TryRecord();

            Assert.That(didRecord, Is.True);
            Assert.That(_scoreRecord.HasRecord, Is.True);
            Assert.That(_scoreRecord.ResultData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_scoreRecord.ResultData.HasInfiniteModeResult, Is.True);
            Assert.That(_scoreRecord.ResultData.HasStageResult, Is.False);
            Assert.That(_scoreRecord.ResultData.FinalDistance, Is.EqualTo(FinalDistance));
            Assert.That(_scoreRecord.ResultData.FinalScore, Is.EqualTo(FinalScore));
        }

        [Test]
        public void TryRecord_BeforeStageEnd_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                false,
                true,
                FinalDistance,
                FinalScore);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_BeforeFinalization_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                false,
                FinalDistance,
                FinalScore);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_StageMode_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Stage,
                true,
                true,
                FinalDistance,
                FinalScore);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void TryRecord_InvalidFinalDistance_IsRejected(float finalDistance)
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                finalDistance,
                FinalScore);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_NegativeFinalScore_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                -1);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_SecondRequest_IsRejectedAndKeepsFirstResult()
        {
            Assert.That(TryRecord(), Is.True);

            bool didRecordAgain = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                200.0f,
                2000);

            Assert.That(didRecordAgain, Is.False);
            Assert.That(_scoreRecord.ResultData.FinalDistance, Is.EqualTo(FinalDistance));
            Assert.That(_scoreRecord.ResultData.FinalScore, Is.EqualTo(FinalScore));
        }

        [Test]
        public void Reset_ExistingRecord_AllowsNextRunRecord()
        {
            Assert.That(TryRecord(), Is.True);

            _scoreRecord.Reset();
            bool didRecordNextRun = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                200.0f,
                2000);

            Assert.That(didRecordNextRun, Is.True);
            Assert.That(_scoreRecord.HasRecord, Is.True);
            Assert.That(_scoreRecord.ResultData.FinalDistance, Is.EqualTo(200.0f));
            Assert.That(_scoreRecord.ResultData.FinalScore, Is.EqualTo(2000));
        }

        private bool TryRecord()
        {
            return _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                FinalScore);
        }
    }
}
