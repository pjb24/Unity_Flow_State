using System;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public class InfiniteScoreState
    {
        private int _scoringVersion;
        private double _scorePerUnit;
        private double _currentDistance;
        private double _preciseMomentumBonus;
        private int _baseDistanceScore;
        private int _momentumBonus;
        private int _distanceScore;
        private bool _isInitialized;
        private bool _isFinalized;

        public int ScoringVersion => _scoringVersion;

        public double CurrentDistance => _currentDistance;

        public double PreciseMomentumBonus => _preciseMomentumBonus;

        public int BaseDistanceScore => _baseDistanceScore;

        public int MomentumBonus => _momentumBonus;

        public int DistanceScore => _distanceScore;

        public bool IsInitialized => _isInitialized;

        public bool IsFinalized => _isFinalized;

        public bool Initialize(int scoringVersion, double scorePerUnit)
        {
            if (_isInitialized ||
                scoringVersion != global::FlowState.Runtime.Core.ScoringVersion.Current ||
                !IsFinite(scorePerUnit) || scorePerUnit <= 0.0)
            {
                return false;
            }

            _scoringVersion = scoringVersion;
            _scorePerUnit = scorePerUnit;
            _currentDistance = 0.0;
            _preciseMomentumBonus = 0.0;
            _baseDistanceScore = 0;
            _momentumBonus = 0;
            _distanceScore = 0;
            _isInitialized = true;
            _isFinalized = false;
            return true;
        }

        public bool TryUpdate(
            int scoringVersion,
            double logicalDistance,
            double multiplier)
        {
            if (!_isInitialized || _isFinalized ||
                scoringVersion != _scoringVersion ||
                !IsFinite(logicalDistance) ||
                logicalDistance < _currentDistance ||
                !IsValidMultiplier(multiplier))
            {
                return false;
            }

            double distanceDelta = logicalDistance - _currentDistance;
            double bonusDelta = distanceDelta * _scorePerUnit *
                                (multiplier - MomentumScoreState.BaseMultiplier);
            double nextBonus = _preciseMomentumBonus + bonusDelta;

            if (double.IsNaN(nextBonus) || nextBonus < 0.0)
            {
                return false;
            }

            _currentDistance = logicalDistance;
            _preciseMomentumBonus = nextBonus;
            _baseDistanceScore = ToSaturatedScore(
                logicalDistance * _scorePerUnit);
            _momentumBonus = ToSaturatedScore(_preciseMomentumBonus);
            _distanceScore = SaturatedAdd(
                _baseDistanceScore,
                _momentumBonus);
            return true;
        }

        public bool TryCalculateTotalScore(
            int scoringVersion,
            int collectibleScore,
            out int totalScore)
        {
            totalScore = 0;

            if (!_isInitialized ||
                scoringVersion != _scoringVersion ||
                collectibleScore < 0)
            {
                return false;
            }

            totalScore = SaturatedAdd(_distanceScore, collectibleScore);
            return true;
        }

        public bool TryFinalize(int scoringVersion)
        {
            if (!_isInitialized || _isFinalized ||
                scoringVersion != _scoringVersion)
            {
                return false;
            }

            _isFinalized = true;
            return true;
        }

        public void Reset()
        {
            _scoringVersion = global::FlowState.Runtime.Core.ScoringVersion.None;
            _scorePerUnit = 0.0;
            _currentDistance = 0.0;
            _preciseMomentumBonus = 0.0;
            _baseDistanceScore = 0;
            _momentumBonus = 0;
            _distanceScore = 0;
            _isInitialized = false;
            _isFinalized = false;
        }

        private static bool IsValidMultiplier(double multiplier)
        {
            if (!IsFinite(multiplier) ||
                multiplier < MomentumScoreState.BaseMultiplier ||
                multiplier > MomentumScoreState.MaximumMultiplier)
            {
                return false;
            }

            double level = (multiplier - MomentumScoreState.BaseMultiplier) /
                           MomentumScoreState.MultiplierStep;
            return Math.Abs(level - Math.Round(level)) <= 0.0000001;
        }

        private static int ToSaturatedScore(double value)
        {
            if (double.IsPositiveInfinity(value) || value >= int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)Math.Floor(value);
        }

        private static int SaturatedAdd(int first, int second)
        {
            return first > int.MaxValue - second
                ? int.MaxValue
                : first + second;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
