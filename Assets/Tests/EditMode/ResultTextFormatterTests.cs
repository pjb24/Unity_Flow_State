using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ResultTextFormatterTests
    {
        [TestCase(E_StageResultType.Cleared, "Result: Stage Clear")]
        [TestCase(E_StageResultType.Fell, "Result: Stage Failed")]
        [TestCase(E_StageResultType.None, "")]
        public void FormatStageResultStatus_ReturnsApprovedText(
            E_StageResultType stageResultType,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatStageResultStatus(
                stageResultType);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(
            E_StageResultType.Cleared,
            12.3456,
            "Clear Time: 12.346 s")]
        [TestCase(
            E_StageResultType.Fell,
            12.3456,
            "Run Time: 12.346 s")]
        public void FormatStageElapsedTime_ValidTime_ReturnsModeText(
            E_StageResultType stageResultType,
            double elapsedTime,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatStageElapsedTime(
                stageResultType,
                elapsedTime);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(E_StageResultType.Cleared, -0.001, "Clear Time: --")]
        [TestCase(E_StageResultType.Cleared, double.NaN, "Clear Time: --")]
        [TestCase(E_StageResultType.Fell, double.PositiveInfinity, "Run Time: --")]
        [TestCase(E_StageResultType.Fell, double.NegativeInfinity, "Run Time: --")]
        public void FormatStageElapsedTime_InvalidTime_ReturnsPlaceholder(
            E_StageResultType stageResultType,
            double elapsedTime,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatStageElapsedTime(
                stageResultType,
                elapsedTime);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(0.0f, "Distance: 0")]
        [TestCase(12.999f, "Distance: 12")]
        public void FormatCurrentDistance_ValidDistance_FloorsForDisplay(
            float distance,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatCurrentDistance(
                distance);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void FormatCurrentDistance_InvalidDistance_ReturnsPlaceholder(
            float distance)
        {
            string resultText = ResultTextFormatter.FormatCurrentDistance(
                distance);

            Assert.That(resultText, Is.EqualTo("Distance: --"));
        }

        [TestCase(0, "Distance Score: 0")]
        [TestCase(int.MaxValue, "Distance Score: 2147483647")]
        public void FormatDistanceScore_ValidScore_ReturnsApprovedFormat(
            int score,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatDistanceScore(score);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(0, "Collectible Score: 0")]
        [TestCase(int.MaxValue, "Collectible Score: 2147483647")]
        public void FormatCollectibleScore_ValidScore_ReturnsApprovedFormat(
            int score,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatCollectibleScore(
                score);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(0, "Total Score: 0")]
        [TestCase(int.MaxValue, "Total Score: 2147483647")]
        public void FormatTotalScore_ValidScore_ReturnsApprovedFormat(
            int score,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatTotalScore(score);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [Test]
        public void FormatScores_NegativeValues_ReturnPlaceholders()
        {
            Assert.That(
                ResultTextFormatter.FormatDistanceScore(-1),
                Is.EqualTo("Distance Score: --"));
            Assert.That(
                ResultTextFormatter.FormatCollectibleScore(-1),
                Is.EqualTo("Collectible Score: --"));
            Assert.That(
                ResultTextFormatter.FormatTotalScore(-1),
                Is.EqualTo("Total Score: --"));
        }

        [TestCase(0.0f, "Final Distance: 0")]
        [TestCase(12.999f, "Final Distance: 12")]
        public void FormatFinalDistance_ValidDistance_FloorsForDisplay(
            float distance,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatFinalDistance(
                distance);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void FormatFinalDistance_InvalidDistance_ReturnsPlaceholder(
            float distance)
        {
            string resultText = ResultTextFormatter.FormatFinalDistance(
                distance);

            Assert.That(resultText, Is.EqualTo("Final Distance: --"));
        }

        [Test]
        public void TryFormatStageResult_ClearedData_ReturnsStageTexts()
        {
            ResultData resultData = new ResultData(
                E_StageResultType.Cleared,
                12.3456,
                30);

            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                resultData,
                out string resultStatusText,
                out string elapsedTimeText,
                out string collectibleScoreText);

            Assert.That(didFormat, Is.True);
            Assert.That(resultStatusText, Is.EqualTo("Result: Stage Clear"));
            Assert.That(elapsedTimeText, Is.EqualTo("Clear Time: 12.346 s"));
            Assert.That(collectibleScoreText, Is.EqualTo("Collectible Score: 30"));
        }

        [Test]
        public void TryFormatStageResult_FellData_ReturnsFailureTexts()
        {
            ResultData resultData = new ResultData(
                E_StageResultType.Fell,
                8.25,
                20);

            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                resultData,
                out string resultStatusText,
                out string elapsedTimeText,
                out string collectibleScoreText);

            Assert.That(didFormat, Is.True);
            Assert.That(resultStatusText, Is.EqualTo("Result: Stage Failed"));
            Assert.That(elapsedTimeText, Is.EqualTo("Run Time: 8.250 s"));
            Assert.That(collectibleScoreText, Is.EqualTo("Collectible Score: 20"));
        }

        [Test]
        public void TryFormatStageResult_InfiniteData_IsRejected()
        {
            ResultData resultData = new ResultData(12.999f, 129, 30, 159);

            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                resultData,
                out string resultStatusText,
                out string elapsedTimeText,
                out string collectibleScoreText);

            Assert.That(didFormat, Is.False);
            Assert.That(resultStatusText, Is.Empty);
            Assert.That(elapsedTimeText, Is.Empty);
            Assert.That(collectibleScoreText, Is.Empty);
        }

        [Test]
        public void TryFormatStageResult_InvalidStageType_IsRejected()
        {
            ResultData resultData = new ResultData(
                E_StageResultType.None,
                12.3456,
                30);

            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                resultData,
                out string resultStatusText,
                out string elapsedTimeText,
                out string collectibleScoreText);

            Assert.That(didFormat, Is.False);
            Assert.That(resultStatusText, Is.Empty);
            Assert.That(elapsedTimeText, Is.Empty);
            Assert.That(collectibleScoreText, Is.Empty);
        }

        [Test]
        public void TryFormatInfiniteResult_InfiniteData_ReturnsInfiniteTexts()
        {
            ResultData resultData = new ResultData(12.999f, 129, 30, 159);

            bool didFormat = ResultTextFormatter.TryFormatInfiniteResult(
                resultData,
                out string finalDistanceText,
                out string distanceScoreText,
                out string collectibleScoreText,
                out string totalScoreText);

            Assert.That(didFormat, Is.True);
            Assert.That(finalDistanceText, Is.EqualTo("Final Distance: 12"));
            Assert.That(distanceScoreText, Is.EqualTo("Distance Score: 129"));
            Assert.That(collectibleScoreText, Is.EqualTo("Collectible Score: 30"));
            Assert.That(totalScoreText, Is.EqualTo("Total Score: 159"));
        }

        [Test]
        public void TryFormatInfiniteResult_StageData_IsRejected()
        {
            ResultData resultData = new ResultData(
                E_StageResultType.Cleared,
                12.3456,
                30);

            bool didFormat = ResultTextFormatter.TryFormatInfiniteResult(
                resultData,
                out string finalDistanceText,
                out string distanceScoreText,
                out string collectibleScoreText,
                out string totalScoreText);

            Assert.That(didFormat, Is.False);
            Assert.That(finalDistanceText, Is.Empty);
            Assert.That(distanceScoreText, Is.Empty);
            Assert.That(collectibleScoreText, Is.Empty);
            Assert.That(totalScoreText, Is.Empty);
        }

        [Test]
        public void TryFormatResults_NullData_IsRejected()
        {
            bool didFormatStage = ResultTextFormatter.TryFormatStageResult(
                null,
                out string resultStatusText,
                out string elapsedTimeText,
                out string stageCollectibleScoreText);
            bool didFormatInfinite = ResultTextFormatter.TryFormatInfiniteResult(
                null,
                out string finalDistanceText,
                out string distanceScoreText,
                out string infiniteCollectibleScoreText,
                out string totalScoreText);

            Assert.That(didFormatStage, Is.False);
            Assert.That(didFormatInfinite, Is.False);
            Assert.That(resultStatusText, Is.Empty);
            Assert.That(elapsedTimeText, Is.Empty);
            Assert.That(stageCollectibleScoreText, Is.Empty);
            Assert.That(finalDistanceText, Is.Empty);
            Assert.That(distanceScoreText, Is.Empty);
            Assert.That(infiniteCollectibleScoreText, Is.Empty);
            Assert.That(totalScoreText, Is.Empty);
        }
    }
}
