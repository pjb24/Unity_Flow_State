using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ResultDataTests
    {
        [Test]
        public void StageResult_ContainsOnlyStageContract()
        {
            ResultData resultData = new ResultData(true, 12.5);

            Assert.That(resultData.GameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(resultData.HasStageResult, Is.True);
            Assert.That(resultData.HasInfiniteModeResult, Is.False);
            Assert.That(resultData.IsStageCleared, Is.True);
            Assert.That(resultData.ClearTime, Is.EqualTo(12.5));
            Assert.That(resultData.FinalDistance, Is.Zero);
            Assert.That(resultData.FinalScore, Is.Zero);
        }

        [Test]
        public void InfiniteResult_ContainsOnlyInfiniteContract()
        {
            ResultData resultData = new ResultData(123.45f, 1234);

            Assert.That(resultData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(resultData.HasStageResult, Is.False);
            Assert.That(resultData.HasInfiniteModeResult, Is.True);
            Assert.That(resultData.IsStageCleared, Is.False);
            Assert.That(resultData.ClearTime, Is.Zero);
            Assert.That(resultData.FinalDistance, Is.EqualTo(123.45f));
            Assert.That(resultData.FinalScore, Is.EqualTo(1234));
        }
    }
}
