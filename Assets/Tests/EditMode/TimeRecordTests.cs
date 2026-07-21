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
            bool didRecord = _timeRecord.TryRecord(true, 12.5);

            Assert.That(didRecord, Is.True);
            Assert.That(_timeRecord.HasRecord, Is.True);
            Assert.That(_timeRecord.ResultData.IsStageCleared, Is.True);
            Assert.That(_timeRecord.ResultData.ClearTime, Is.EqualTo(12.5));
        }

        [Test]
        public void TryRecord_NotClearedStage_IsRejected()
        {
            bool didRecord = _timeRecord.TryRecord(false, 12.5);

            Assert.That(didRecord, Is.False);
            Assert.That(_timeRecord.HasRecord, Is.False);
        }

        [Test]
        public void TryRecord_SecondRequest_IsRejected()
        {
            _timeRecord.TryRecord(true, 12.5);

            bool didRecordAgain = _timeRecord.TryRecord(true, 10.0);

            Assert.That(didRecordAgain, Is.False);
            Assert.That(_timeRecord.ResultData.ClearTime, Is.EqualTo(12.5));
        }

        [Test]
        public void Reset_ExistingRecord_ClearsState()
        {
            _timeRecord.TryRecord(true, 12.5);

            _timeRecord.Reset();

            Assert.That(_timeRecord.HasRecord, Is.False);
            Assert.That(_timeRecord.ResultData, Is.Null);
        }
    }
}
