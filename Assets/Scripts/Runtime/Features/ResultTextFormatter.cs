using System;
using System.Globalization;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public static class ResultTextFormatter
    {
        private const string ClearTimeFormat = "Clear Time: {0:F3} s";
        private const string CurrentDistanceFormat = "Distance: {0:0}";
        private const string CurrentDistancePlaceholder = "Distance: --";
        private const string CurrentScoreFormat = "Score: {0}";
        private const string CurrentScorePlaceholder = "Score: --";
        private const string FinalDistanceFormat = "Final Distance: {0:0}";
        private const string FinalDistancePlaceholder = "Final Distance: --";
        private const string FinalScoreFormat = "Final Score: {0}";
        private const string FinalScorePlaceholder = "Final Score: --";

        public static string FormatClearTime(double clearTime)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                ClearTimeFormat,
                clearTime);
        }

        public static string FormatCurrentDistance(float distance)
        {
            return FormatDistance(
                distance,
                CurrentDistanceFormat,
                CurrentDistancePlaceholder);
        }

        public static string FormatCurrentScore(int score)
        {
            return FormatScore(
                score,
                CurrentScoreFormat,
                CurrentScorePlaceholder);
        }

        public static string FormatFinalDistance(float distance)
        {
            return FormatDistance(
                distance,
                FinalDistanceFormat,
                FinalDistancePlaceholder);
        }

        public static string FormatFinalScore(int score)
        {
            return FormatScore(
                score,
                FinalScoreFormat,
                FinalScorePlaceholder);
        }

        public static bool TryGetDisplayDistance(
            float distance,
            out double displayDistance)
        {
            displayDistance = 0.0;

            if (float.IsNaN(distance) ||
                float.IsInfinity(distance) ||
                distance < 0.0f)
            {
                return false;
            }

            displayDistance = Math.Floor(distance);
            return true;
        }

        public static bool TryFormatStageResult(
            ResultData resultData,
            out string clearTimeText)
        {
            clearTimeText = string.Empty;

            if (resultData == null ||
                resultData.GameMode != E_GameMode.Stage ||
                !resultData.HasStageResult ||
                resultData.HasInfiniteModeResult)
            {
                return false;
            }

            clearTimeText = FormatClearTime(resultData.ClearTime);
            return true;
        }

        public static bool TryFormatInfiniteResult(
            ResultData resultData,
            out string finalDistanceText,
            out string finalScoreText)
        {
            finalDistanceText = string.Empty;
            finalScoreText = string.Empty;

            if (resultData == null ||
                resultData.GameMode != E_GameMode.Infinite ||
                resultData.HasStageResult ||
                !resultData.HasInfiniteModeResult)
            {
                return false;
            }

            finalDistanceText = FormatFinalDistance(resultData.FinalDistance);
            finalScoreText = FormatFinalScore(resultData.FinalScore);
            return true;
        }

        private static string FormatDistance(
            float distance,
            string format,
            string placeholder)
        {
            if (!TryGetDisplayDistance(distance, out double displayDistance))
            {
                return placeholder;
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                displayDistance);
        }

        private static string FormatScore(
            int score,
            string format,
            string placeholder)
        {
            if (score < 0)
            {
                return placeholder;
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                score);
        }
    }
}
