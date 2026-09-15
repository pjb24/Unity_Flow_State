using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public class MomentumScoreState
    {
        public const double BaseMultiplier = 1.0;
        public const double MultiplierStep = 0.25;
        public const double MaximumMultiplier = 3.0;

        private const int MaximumLevel = 8;
        private const double FirstDuration = 10.0;
        private const double DurationStep = 0.5;

        private int _scoringVersion;
        private int _successCount;
        private long _lastSuccessRequestId;
        private double _currentMultiplier;
        private double _maximumReachedMultiplier;
        private double _remainingDuration;
        private double _currentDuration;
        private bool _isRunning;
        private bool _isPaused;
        private bool _isFinalized;

        public int ScoringVersion => _scoringVersion;

        public int SuccessCount => _successCount;

        public double CurrentMultiplier => _currentMultiplier;

        public double MaximumReachedMultiplier => _maximumReachedMultiplier;

        public double RemainingDuration => _remainingDuration;

        public double CurrentDuration => _currentDuration;

        public double RemainingRatio => _currentDuration > 0.0
            ? _remainingDuration / _currentDuration
            : 0.0;

        public bool IsRunning => _isRunning;

        public bool IsPaused => _isPaused;

        public bool IsFinalized => _isFinalized;

        public bool StartRun(int scoringVersion)
        {
            if (_isRunning ||
                scoringVersion != global::FlowState.Runtime.Core.ScoringVersion.Current)
            {
                return false;
            }

            _scoringVersion = scoringVersion;
            _successCount = 0;
            _lastSuccessRequestId = 0;
            _currentMultiplier = BaseMultiplier;
            _maximumReachedMultiplier = BaseMultiplier;
            _remainingDuration = 0.0;
            _currentDuration = 0.0;
            _isRunning = true;
            _isPaused = false;
            _isFinalized = false;
            return true;
        }

        public bool TryStep(
            int scoringVersion,
            double deltaTime,
            bool hasMomentumSuccess,
            long successRequestId)
        {
            if (!_isRunning || _isPaused || _isFinalized ||
                scoringVersion != _scoringVersion ||
                !IsFinite(deltaTime) || deltaTime < 0.0)
            {
                return false;
            }

            if (hasMomentumSuccess)
            {
                return TryApplySuccess(successRequestId);
            }

            if (successRequestId != 0)
            {
                return false;
            }

            if (_currentMultiplier == BaseMultiplier)
            {
                return true;
            }

            if (deltaTime >= _remainingDuration)
            {
                ResetMultiplier();
                return true;
            }

            _remainingDuration -= deltaTime;
            return true;
        }

        public bool Pause()
        {
            if (!_isRunning || _isPaused || _isFinalized)
            {
                return false;
            }

            _isPaused = true;
            return true;
        }

        public bool TryNotifyNormalLanding(int scoringVersion)
        {
            return CanProcessNotification(scoringVersion);
        }

        public bool TryNotifyWallContact(int scoringVersion)
        {
            return CanProcessNotification(scoringVersion);
        }

        public bool Resume()
        {
            if (!_isRunning || !_isPaused || _isFinalized)
            {
                return false;
            }

            _isPaused = false;
            return true;
        }

        public bool FinalizeRun()
        {
            if (!_isRunning || _isFinalized)
            {
                return false;
            }

            _isPaused = false;
            _isFinalized = true;
            return true;
        }

        public void Reset()
        {
            _scoringVersion = global::FlowState.Runtime.Core.ScoringVersion.None;
            _successCount = 0;
            _lastSuccessRequestId = 0;
            _currentMultiplier = BaseMultiplier;
            _maximumReachedMultiplier = BaseMultiplier;
            _remainingDuration = 0.0;
            _currentDuration = 0.0;
            _isRunning = false;
            _isPaused = false;
            _isFinalized = false;
        }

        private bool TryApplySuccess(long successRequestId)
        {
            if (successRequestId <= 0 ||
                successRequestId <= _lastSuccessRequestId)
            {
                return false;
            }

            _lastSuccessRequestId = successRequestId;

            if (_successCount < MaximumLevel)
            {
                _successCount++;
            }

            _currentMultiplier = BaseMultiplier +
                                 _successCount * MultiplierStep;
            _currentDuration = FirstDuration -
                               (_successCount - 1) * DurationStep;
            _remainingDuration = _currentDuration;

            if (_currentMultiplier > _maximumReachedMultiplier)
            {
                _maximumReachedMultiplier = _currentMultiplier;
            }

            return true;
        }

        private void ResetMultiplier()
        {
            _successCount = 0;
            _currentMultiplier = BaseMultiplier;
            _remainingDuration = 0.0;
            _currentDuration = 0.0;
        }

        private bool CanProcessNotification(int scoringVersion)
        {
            return _isRunning && !_isPaused && !_isFinalized &&
                   scoringVersion == _scoringVersion;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
