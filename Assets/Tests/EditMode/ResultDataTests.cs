using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ResultDataTests
    {
        [Test]
        public void StageResult_ContainsOnlyStageContract()
        {
            ResultData resultData = new ResultData(
                E_StageResultType.Cleared,
                12.5,
                30);

            Assert.That(resultData.GameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(resultData.HasStageResult, Is.True);
            Assert.That(resultData.HasInfiniteModeResult, Is.False);
            Assert.That(
                resultData.StageResultType,
                Is.EqualTo(E_StageResultType.Cleared));
            Assert.That(resultData.ElapsedTime, Is.EqualTo(12.5));
            Assert.That(resultData.FinalDistance, Is.Zero);
            Assert.That(resultData.DistanceScore, Is.Zero);
            Assert.That(resultData.CollectibleScore, Is.EqualTo(30));
            Assert.That(resultData.TotalScore, Is.Zero);
        }

        [Test]
        public void InfiniteResult_ContainsOnlyInfiniteContract()
        {
            ResultData resultData = new ResultData(
                123.45f,
                1234,
                30,
                1264);

            Assert.That(resultData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(resultData.HasStageResult, Is.False);
            Assert.That(resultData.HasInfiniteModeResult, Is.True);
            Assert.That(
                resultData.StageResultType,
                Is.EqualTo(E_StageResultType.None));
            Assert.That(resultData.ElapsedTime, Is.Zero);
            Assert.That(resultData.FinalDistance, Is.EqualTo(123.45f));
            Assert.That(resultData.DistanceScore, Is.EqualTo(1234));
            Assert.That(resultData.CollectibleScore, Is.EqualTo(30));
            Assert.That(resultData.TotalScore, Is.EqualTo(1264));
        }
    }
}
