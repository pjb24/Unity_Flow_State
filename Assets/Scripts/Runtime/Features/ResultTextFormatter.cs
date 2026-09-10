using System;
using System.Globalization;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public static class ResultTextFormatter
    {
        private const string StageClearStatus = "Result: Stage Clear";
        private const string StageFailedStatus = "Result: Stage Failed";
        private const string ClearTimeFormat = "Clear Time: {0:F3} s";
        private const string ClearTimePlaceholder = "Clear Time: --";
        private const string RunTimeFormat = "Run Time: {0:F3} s";
        private const string RunTimePlaceholder = "Run Time: --";
        private const string CurrentDistanceFormat = "Distance: {0:0}";
        private const string CurrentDistancePlaceholder = "Distance: --";
        private const string FinalDistanceFormat = "Final Distance: {0:0}";
        private const string FinalDistancePlaceholder = "Final Distance: --";
        private const string DistanceScoreFormat = "Distance Score: {0}";
        private const string DistanceScorePlaceholder = "Distance Score: --";
        private const string CollectibleScoreFormat = "Collectible Score: {0}";
        private const string CollectibleScorePlaceholder = "Collectible Score: --";
        private const string TotalScoreFormat = "Total Score: {0}";
        private const string TotalScorePlaceholder = "Total Score: --";

        public static string FormatStageResultStatus(
            E_StageResultType stageResultType)
        {
            switch (stageResultType)
            {
                case E_StageResultType.Cleared:
                    return StageClearStatus;

                case E_StageResultType.Fell:
                    return StageFailedStatus;

                default:
                    return string.Empty;
            }
        }

        public static string FormatStageElapsedTime(
            E_StageResultType stageResultType,
            double elapsedTime)
        {
            bool isValid = !double.IsNaN(elapsedTime) &&
                           !double.IsInfinity(elapsedTime) &&
                           elapsedTime >= 0.0;

            if (stageResultType == E_StageResultType.Cleared)
            {
                return isValid
                    ? FormatTime(ClearTimeFormat, elapsedTime)
                    : ClearTimePlaceholder;
            }

            if (stageResultType == E_StageResultType.Fell)
            {
                return isValid
                    ? FormatTime(RunTimeFormat, elapsedTime)
                    : RunTimePlaceholder;
            }

            return string.Empty;
        }

        public static string FormatCurrentDistance(float distance)
        {
            return FormatDistance(
                distance,
                CurrentDistanceFormat,
                CurrentDistancePlaceholder);
        }

        public static string FormatFinalDistance(float distance)
        {
            return FormatDistance(
                distance,
                FinalDistanceFormat,
                FinalDistancePlaceholder);
        }

        public static string FormatDistanceScore(int score)
        {
            return FormatScore(
                score,
                DistanceScoreFormat,
                DistanceScorePlaceholder);
        }

        public static string FormatCollectibleScore(int score)
        {
            return FormatScore(
                score,
                CollectibleScoreFormat,
                CollectibleScorePlaceholder);
        }

        public static string FormatTotalScore(int score)
        {
            return FormatScore(
                score,
                TotalScoreFormat,
                TotalScorePlaceholder);
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
            out string resultStatusText,
            out string elapsedTimeText,
            out string collectibleScoreText)
        {
            resultStatusText = string.Empty;
            elapsedTimeText = string.Empty;
            collectibleScoreText = string.Empty;

            if (resultData == null ||
                resultData.GameMode != E_GameMode.Stage ||
                !resultData.HasStageResult ||
                resultData.HasInfiniteModeResult ||
                (resultData.StageResultType != E_StageResultType.Cleared &&
                 resultData.StageResultType != E_StageResultType.Fell))
            {
                return false;
            }

            resultStatusText = FormatStageResultStatus(
                resultData.StageResultType);
            elapsedTimeText = FormatStageElapsedTime(
                resultData.StageResultType,
                resultData.ElapsedTime);
            collectibleScoreText = FormatCollectibleScore(
                resultData.CollectibleScore);
            return true;
        }

        public static bool TryFormatInfiniteResult(
            ResultData resultData,
            out string finalDistanceText,
            out string distanceScoreText,
            out string collectibleScoreText,
            out string totalScoreText)
        {
            finalDistanceText = string.Empty;
            distanceScoreText = string.Empty;
            collectibleScoreText = string.Empty;
            totalScoreText = string.Empty;

            if (resultData == null ||
                resultData.GameMode != E_GameMode.Infinite ||
                resultData.HasStageResult ||
                !resultData.HasInfiniteModeResult)
            {
                return false;
            }

            finalDistanceText = FormatFinalDistance(resultData.FinalDistance);
            distanceScoreText = FormatDistanceScore(resultData.DistanceScore);
            collectibleScoreText = FormatCollectibleScore(
                resultData.CollectibleScore);
            totalScoreText = FormatTotalScore(resultData.TotalScore);
            return true;
        }

        private static string FormatTime(string format, double elapsedTime)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                elapsedTime);
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
