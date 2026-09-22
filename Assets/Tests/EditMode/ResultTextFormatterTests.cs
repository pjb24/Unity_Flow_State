using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ResultTextFormatterTests
    {
        [TestCase(E_StageResultType.Cleared, "STAGE CLEAR")]
        [TestCase(E_StageResultType.Fell, "STAGE FAILED")]
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

        [TestCase(0, "Collectibles: 0")]
        [TestCase(int.MaxValue, "Collectibles: 2147483647")]
        public void FormatStageCollectibleCounter_ValidScore_ReturnsCompactHudFormat(
            int score,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatStageCollectibleCounter(
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
                ResultTextFormatter.FormatStageCollectibleCounter(-1),
                Is.EqualTo("Collectibles: --"));
            Assert.That(
                ResultTextFormatter.FormatTotalScore(-1),
                Is.EqualTo("Total Score: --"));
        }

        [Test]
        public void FormatCurrentScoreComponents_ReturnApprovedText()
        {
            Assert.That(
                ResultTextFormatter.FormatBaseDistanceScore(100),
                Is.EqualTo("Base Distance Score: 100"));
            Assert.That(
                ResultTextFormatter.FormatMomentumBonus(25),
                Is.EqualTo("Momentum Bonus: +25"));
            Assert.That(
                ResultTextFormatter.FormatMaximumMomentumMultiplier(2.5),
                Is.EqualTo("Max Momentum: x2.50"));
        }

        [Test]
        public void FormatCurrentScoreComponents_InvalidValues_ReturnPlaceholders()
        {
            Assert.That(
                ResultTextFormatter.FormatBaseDistanceScore(-1),
                Is.EqualTo("Base Distance Score: --"));
            Assert.That(
                ResultTextFormatter.FormatMomentumBonus(-1),
                Is.EqualTo("Momentum Bonus: --"));
            Assert.That(
                ResultTextFormatter.FormatMaximumMomentumMultiplier(double.NaN),
                Is.EqualTo("Max Momentum: x--"));
            Assert.That(
                ResultTextFormatter.FormatMaximumMomentumMultiplier(3.25),
                Is.EqualTo("Max Momentum: x--"));
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
            Assert.That(resultStatusText, Is.EqualTo("STAGE CLEAR"));
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
            Assert.That(resultStatusText, Is.EqualTo("STAGE FAILED"));
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
        public void TryFormatInfiniteResult_CurrentData_ReturnsCompleteScoreTexts()
        {
            ResultData resultData = new ResultData(
                ScoringVersion.Current,
                12.999f,
                100,
                29,
                129,
                30,
                159,
                2.5);

            bool didFormat = ResultTextFormatter.TryFormatInfiniteResult(
                resultData,
                out string finalDistanceText,
                out string baseDistanceScoreText,
                out string momentumBonusText,
                out string distanceScoreText,
                out string collectibleScoreText,
                out string totalScoreText,
                out string maximumMomentumText);

            Assert.That(didFormat, Is.True);
            Assert.That(finalDistanceText, Is.EqualTo("Final Distance: 12"));
            Assert.That(baseDistanceScoreText,
                Is.EqualTo("Base Distance Score: 100"));
            Assert.That(momentumBonusText, Is.EqualTo("Momentum Bonus: +29"));
            Assert.That(distanceScoreText, Is.EqualTo("Distance Score: 129"));
            Assert.That(collectibleScoreText,
                Is.EqualTo("Collectible Score: 30"));
            Assert.That(totalScoreText, Is.EqualTo("Total Score: 159"));
            Assert.That(maximumMomentumText,
                Is.EqualTo("Max Momentum: x2.50"));
        }

        [Test]
        public void TryFormatInfiniteResult_CompleteContractRejectsLegacyData()
        {
            ResultData resultData = new ResultData(12.999f, 129, 30, 159);

            Assert.That(ResultTextFormatter.TryFormatInfiniteResult(
                resultData,
                out _, out _, out _, out _, out _, out _, out _), Is.False);
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
