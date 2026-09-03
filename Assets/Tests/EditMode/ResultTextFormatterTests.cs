using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ResultTextFormatterTests
    {
        [TestCase(0.0, "Clear Time: 0.000 s")]
        [TestCase(12.3454, "Clear Time: 12.345 s")]
        [TestCase(12.3456, "Clear Time: 12.346 s")]
        public void FormatClearTime_ValidTime_ReturnsApprovedFormat(
            double clearTime,
            string expectedText)
        {
            string resultText =
                ResultTextFormatter.FormatClearTime(clearTime);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(0.0f, "Distance: 0")]
        [TestCase(0.999f, "Distance: 0")]
        [TestCase(12.999f, "Distance: 12")]
        [TestCase(2147483648.0f, "Distance: 2147483648")]
        public void FormatCurrentDistance_ValidDistance_FloorsForDisplay(
            float distance,
            string expectedText)
        {
            string resultText =
                ResultTextFormatter.FormatCurrentDistance(distance);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void FormatCurrentDistance_InvalidDistance_ReturnsPlaceholder(
            float distance)
        {
            string resultText =
                ResultTextFormatter.FormatCurrentDistance(distance);

            Assert.That(resultText, Is.EqualTo("Distance: --"));
        }

        [TestCase(0, "Score: 0")]
        [TestCase(123, "Score: 123")]
        [TestCase(int.MaxValue, "Score: 2147483647")]
        public void FormatCurrentScore_ValidScore_ReturnsApprovedFormat(
            int score,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatCurrentScore(score);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [Test]
        public void FormatCurrentScore_NegativeScore_ReturnsPlaceholder()
        {
            string resultText = ResultTextFormatter.FormatCurrentScore(-1);

            Assert.That(resultText, Is.EqualTo("Score: --"));
        }

        [TestCase(0.0f, "Final Distance: 0")]
        [TestCase(12.999f, "Final Distance: 12")]
        public void FormatFinalDistance_ValidDistance_FloorsForDisplay(
            float distance,
            string expectedText)
        {
            string resultText =
                ResultTextFormatter.FormatFinalDistance(distance);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void FormatFinalDistance_InvalidDistance_ReturnsPlaceholder(
            float distance)
        {
            string resultText =
                ResultTextFormatter.FormatFinalDistance(distance);

            Assert.That(resultText, Is.EqualTo("Final Distance: --"));
        }

        [TestCase(0, "Final Score: 0")]
        [TestCase(int.MaxValue, "Final Score: 2147483647")]
        public void FormatFinalScore_ValidScore_ReturnsApprovedFormat(
            int score,
            string expectedText)
        {
            string resultText = ResultTextFormatter.FormatFinalScore(score);

            Assert.That(resultText, Is.EqualTo(expectedText));
        }

        [Test]
        public void FormatFinalScore_NegativeScore_ReturnsPlaceholder()
        {
            string resultText = ResultTextFormatter.FormatFinalScore(-1);

            Assert.That(resultText, Is.EqualTo("Final Score: --"));
        }

        [Test]
        public void TryFormatStageResult_StageData_ReturnsOnlyStageText()
        {
            ResultData resultData = new ResultData(true, 12.3456);

            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                resultData,
                out string clearTimeText);

            Assert.That(didFormat, Is.True);
            Assert.That(clearTimeText, Is.EqualTo("Clear Time: 12.346 s"));
        }

        [Test]
        public void TryFormatStageResult_InfiniteData_IsRejected()
        {
            ResultData resultData = new ResultData(12.999f, 129);

            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                resultData,
                out string clearTimeText);

            Assert.That(didFormat, Is.False);
            Assert.That(clearTimeText, Is.Empty);
        }

        [Test]
        public void TryFormatStageResult_NullData_IsRejected()
        {
            bool didFormat = ResultTextFormatter.TryFormatStageResult(
                null,
                out string clearTimeText);

            Assert.That(didFormat, Is.False);
            Assert.That(clearTimeText, Is.Empty);
        }

        [Test]
        public void TryFormatInfiniteResult_InfiniteData_ReturnsOnlyInfiniteText()
        {
            ResultData resultData = new ResultData(12.999f, 129);

            bool didFormat = ResultTextFormatter.TryFormatInfiniteResult(
                resultData,
                out string finalDistanceText,
                out string finalScoreText);

            Assert.That(didFormat, Is.True);
            Assert.That(finalDistanceText, Is.EqualTo("Final Distance: 12"));
            Assert.That(finalScoreText, Is.EqualTo("Final Score: 129"));
        }

        [Test]
        public void TryFormatInfiniteResult_StageData_IsRejected()
        {
            ResultData resultData = new ResultData(true, 12.3456);

            bool didFormat = ResultTextFormatter.TryFormatInfiniteResult(
                resultData,
                out string finalDistanceText,
                out string finalScoreText);

            Assert.That(didFormat, Is.False);
            Assert.That(finalDistanceText, Is.Empty);
            Assert.That(finalScoreText, Is.Empty);
        }

        [Test]
        public void TryFormatInfiniteResult_NullData_IsRejected()
        {
            bool didFormat = ResultTextFormatter.TryFormatInfiniteResult(
                null,
                out string finalDistanceText,
                out string finalScoreText);

            Assert.That(didFormat, Is.False);
            Assert.That(finalDistanceText, Is.Empty);
            Assert.That(finalScoreText, Is.Empty);
        }
    }
}
