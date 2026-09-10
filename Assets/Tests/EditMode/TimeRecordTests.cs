using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class TimeRecordTests
    {
        private TimeRecord _timeRecord;

        [SetUp]
        public void SetUp()
        {
            _timeRecord = new TimeRecord();
        }

        [Test]
        public void TryRecord_ClearedStage_CreatesResultData()
        {
            bool didRecord = _timeRecord.TryRecord(
                E_StageResultType.Cleared,
                12.5,
                30);

            Assert.That(didRecord, Is.True);
            Assert.That(_timeRecord.HasRecord, Is.True);
            Assert.That(
                _timeRecord.ResultData.StageResultType,
                Is.EqualTo(E_StageResultType.Cleared));
            Assert.That(_timeRecord.ResultData.ElapsedTime, Is.EqualTo(12.5));
            Assert.That(_timeRecord.ResultData.CollectibleScore, Is.EqualTo(30));
        }

        [Test]
        public void TryRecord_FellStage_CreatesResultData()
        {
            bool didRecord = _timeRecord.TryRecord(
                E_StageResultType.Fell,
                12.5,
                20);

            Assert.That(didRecord, Is.True);
            Assert.That(
                _timeRecord.ResultData.StageResultType,
                Is.EqualTo(E_StageResultType.Fell));
            Assert.That(_timeRecord.ResultData.ElapsedTime, Is.EqualTo(12.5));
            Assert.That(_timeRecord.ResultData.CollectibleScore, Is.EqualTo(20));
        }

        [TestCase(E_StageResultType.None)]
        [TestCase((E_StageResultType)999)]
        public void TryRecord_InvalidStageResultType_IsRejected(
            E_StageResultType stageResultType)
        {
            bool didRecord = _timeRecord.TryRecord(
                stageResultType,
                12.5,
                20);

            Assert.That(didRecord, Is.False);
            Assert.That(_timeRecord.HasRecord, Is.False);
        }

        [TestCase(-0.001)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void TryRecord_InvalidElapsedTime_IsRejected(double elapsedTime)
        {
            bool didRecord = _timeRecord.TryRecord(
                E_StageResultType.Cleared,
                elapsedTime,
                20);

            Assert.That(didRecord, Is.False);
            Assert.That(_timeRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_NegativeCollectibleScore_IsRejected()
        {
            bool didRecord = _timeRecord.TryRecord(
                E_StageResultType.Cleared,
                12.5,
                -1);

            Assert.That(didRecord, Is.False);
            Assert.That(_timeRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_SecondRequest_IsRejected()
        {
            _timeRecord.TryRecord(E_StageResultType.Cleared, 12.5, 30);

            bool didRecordAgain = _timeRecord.TryRecord(
                E_StageResultType.Fell,
                10.0,
                20);

            Assert.That(didRecordAgain, Is.False);
            Assert.That(_timeRecord.ResultData.ElapsedTime, Is.EqualTo(12.5));
        }

        [Test]
        public void Reset_ExistingRecord_ClearsState()
        {
            _timeRecord.TryRecord(E_StageResultType.Cleared, 12.5, 30);

            _timeRecord.Reset();

            Assert.That(_timeRecord.HasRecord, Is.False);
            Assert.That(_timeRecord.ResultData, Is.Null);
        }
    }
}
