using System;

namespace FlowState.Runtime.Features
{
    public sealed class InfiniteScoreLimit
    {
        private readonly int _scoringVersion;
        private readonly double _maximumHorizontalSpeed;
        private readonly double _scorePerDistanceUnit;
        private readonly double _maximumMomentumMultiplier;
        private readonly double _minimumPatternLength;
        private readonly int _maximumCollectiblesPerPattern;
        private readonly int _collectibleScorePerItem;

        public int ScoringVersion => _scoringVersion;

        public double MaximumMomentumMultiplier => _maximumMomentumMultiplier;

        public InfiniteScoreLimit(
            int scoringVersion,
            double maximumHorizontalSpeed,
            double scorePerDistanceUnit,
            double maximumMomentumMultiplier,
            double minimumPatternLength,
            int maximumCollectiblesPerPattern,
            int collectibleScorePerItem)
        {
            _scoringVersion = scoringVersion;
            _maximumHorizontalSpeed = maximumHorizontalSpeed;
            _scorePerDistanceUnit = scorePerDistanceUnit;
            _maximumMomentumMultiplier = maximumMomentumMultiplier;
            _minimumPatternLength = minimumPatternLength;
            _maximumCollectiblesPerPattern = maximumCollectiblesPerPattern;
            _collectibleScorePerItem = collectibleScorePerItem;
        }

        public bool TryCalculateMaximumTotalScore(
            long runDurationMilliseconds,
            out int maximumTotalScore)
        {
            maximumTotalScore = 0;

            if (!IsValid() || runDurationMilliseconds < 0)
            {
                return false;
            }

            double durationSeconds = runDurationMilliseconds / 1000.0;
            double maximumDistance = durationSeconds * _maximumHorizontalSpeed;
            double maximumDistanceScore = maximumDistance *
                                          _scorePerDistanceUnit *
                                          _maximumMomentumMultiplier;
            double maximumPatternCount = Math.Ceiling(
                maximumDistance / _minimumPatternLength);
            double maximumCollectibleScore = maximumPatternCount *
                                             _maximumCollectiblesPerPattern *
                                             _collectibleScorePerItem;
            double maximumScore = maximumDistanceScore +
                                  maximumCollectibleScore;

            if (double.IsNaN(maximumScore) || maximumScore < 0.0)
            {
                return false;
            }

            maximumTotalScore = double.IsPositiveInfinity(maximumScore) ||
                                maximumScore >= int.MaxValue
                ? int.MaxValue
                : (int)Math.Floor(maximumScore);
            return true;
        }

        private bool IsValid()
        {
            return _scoringVersion > 0 &&
                   IsFinite(_maximumHorizontalSpeed) &&
                   _maximumHorizontalSpeed > 0.0 &&
                   IsFinite(_scorePerDistanceUnit) &&
                   _scorePerDistanceUnit > 0.0 &&
                   IsFinite(_maximumMomentumMultiplier) &&
                   _maximumMomentumMultiplier >= 1.0 &&
                   IsFinite(_minimumPatternLength) &&
                   _minimumPatternLength > 0.0 &&
                   _maximumCollectiblesPerPattern >= 0 &&
                   _collectibleScorePerItem >= 0;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
