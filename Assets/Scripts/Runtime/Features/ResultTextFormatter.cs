using System.Globalization;

namespace FlowState.Runtime.Features
{
    public static class ResultTextFormatter
    {
        private const string ClearTimeFormat = "Clear Time: {0:F3} s";

        public static string FormatClearTime(double clearTime)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                ClearTimeFormat,
                clearTime);
        }
    }
}
