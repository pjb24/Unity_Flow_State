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
    }
}
