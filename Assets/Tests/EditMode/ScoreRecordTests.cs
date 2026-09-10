using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ScoreRecordTests
    {
        private const float FinalDistance = 123.45f;
        private const int DistanceScore = 1234;
        private const int CollectibleScore = 30;

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
            Assert.That(
                _scoreRecord.ResultData.DistanceScore,
                Is.EqualTo(DistanceScore));
            Assert.That(
                _scoreRecord.ResultData.CollectibleScore,
                Is.EqualTo(CollectibleScore));
            Assert.That(_scoreRecord.ResultData.TotalScore, Is.EqualTo(1264));
        }

        [Test]
        public void TryRecord_BeforeStageEnd_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                false,
                true,
                FinalDistance,
                DistanceScore,
                CollectibleScore);

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
                DistanceScore,
                CollectibleScore);

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
                DistanceScore,
                CollectibleScore);

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
                DistanceScore,
                CollectibleScore);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_NegativeDistanceScore_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                -1,
                CollectibleScore);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_NegativeCollectibleScore_IsRejected()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                DistanceScore,
                -1);

            Assert.That(didRecord, Is.False);
            Assert.That(_scoreRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_ZeroScores_RecordsZeroTotalScore()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                0.0f,
                0,
                0);

            Assert.That(didRecord, Is.True);
            Assert.That(_scoreRecord.ResultData.TotalScore, Is.Zero);
        }

        [Test]
        public void TryRecord_TotalAtMaximum_RecordsExactMaximum()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                int.MaxValue - 10,
                10);

            Assert.That(didRecord, Is.True);
            Assert.That(
                _scoreRecord.ResultData.TotalScore,
                Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void TryRecord_TotalAboveMaximum_SaturatesAtMaximum()
        {
            bool didRecord = _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                int.MaxValue,
                1);

            Assert.That(didRecord, Is.True);
            Assert.That(
                _scoreRecord.ResultData.TotalScore,
                Is.EqualTo(int.MaxValue));
        }

        [TestCase(100, 20, 120)]
        [TestCase(int.MaxValue, 1, int.MaxValue)]
        public void TryCalculateTotalScore_ValidScores_ReturnsSaturatedSum(
            int distanceScore,
            int collectibleScore,
            int expectedTotalScore)
        {
            bool didCalculate = ScoreRecord.TryCalculateTotalScore(
                distanceScore,
                collectibleScore,
                out int totalScore);

            Assert.That(didCalculate, Is.True);
            Assert.That(totalScore, Is.EqualTo(expectedTotalScore));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        public void TryCalculateTotalScore_NegativeScore_IsRejected(
            int distanceScore,
            int collectibleScore)
        {
            bool didCalculate = ScoreRecord.TryCalculateTotalScore(
                distanceScore,
                collectibleScore,
                out int totalScore);

            Assert.That(didCalculate, Is.False);
            Assert.That(totalScore, Is.Zero);
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
                2000,
                20);

            Assert.That(didRecordAgain, Is.False);
            Assert.That(_scoreRecord.ResultData.FinalDistance, Is.EqualTo(FinalDistance));
            Assert.That(
                _scoreRecord.ResultData.DistanceScore,
                Is.EqualTo(DistanceScore));
            Assert.That(
                _scoreRecord.ResultData.CollectibleScore,
                Is.EqualTo(CollectibleScore));
            Assert.That(_scoreRecord.ResultData.TotalScore, Is.EqualTo(1264));
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
                2000,
                20);

            Assert.That(didRecordNextRun, Is.True);
            Assert.That(_scoreRecord.HasRecord, Is.True);
            Assert.That(_scoreRecord.ResultData.FinalDistance, Is.EqualTo(200.0f));
            Assert.That(_scoreRecord.ResultData.DistanceScore, Is.EqualTo(2000));
            Assert.That(_scoreRecord.ResultData.CollectibleScore, Is.EqualTo(20));
            Assert.That(_scoreRecord.ResultData.TotalScore, Is.EqualTo(2020));
        }

        private bool TryRecord()
        {
            return _scoreRecord.TryRecord(
                E_GameMode.Infinite,
                true,
                true,
                FinalDistance,
                DistanceScore,
                CollectibleScore);
        }
    }
}
