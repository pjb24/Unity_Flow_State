using System;

namespace FlowState.Runtime.Features
{
    public class WorldRebaseState
    {
        public const double RebaseThreshold = 880.0;
        public const double BaseRebaseOffset = 880.0;

        private double _originLogicalX;
        private double _cumulativeRebaseOffset;
        private double _maximumForwardDistance;
        private bool _isRunning;
        private bool _isPaused;
        private bool _isFinalized;

        public double OriginLogicalX => _originLogicalX;

        public double CumulativeRebaseOffset => _cumulativeRebaseOffset;

        public double MaximumForwardDistance => _maximumForwardDistance;

        public bool IsRunning => _isRunning;

        public bool IsPaused => _isPaused;

        public bool IsFinalized => _isFinalized;

        public bool StartRun(double originWorldX)
        {
            if (_isRunning || !IsFinite(originWorldX))
            {
                return false;
            }

            _originLogicalX = originWorldX;
            _cumulativeRebaseOffset = 0.0;
            _maximumForwardDistance = 0.0;
            _isRunning = true;
            _isPaused = false;
            _isFinalized = false;
            return true;
        }

        public bool TryUpdate(double currentWorldX)
        {
            if (!CanChangeState() || !IsFinite(currentWorldX))
            {
                return false;
            }

            double logicalX = currentWorldX + _cumulativeRebaseOffset;

            if (!IsFinite(logicalX))
            {
                return false;
            }

            double forwardDistance = logicalX - _originLogicalX;

            if (!IsFinite(forwardDistance))
            {
                return false;
            }

            if (forwardDistance > _maximumForwardDistance)
            {
                _maximumForwardDistance = forwardDistance;
            }

            return true;
        }

        public bool TryGetRebaseOffset(
            double currentWorldX,
            out double rebaseOffset)
        {
            rebaseOffset = 0.0;

            if (!CanChangeState() || !IsFinite(currentWorldX))
            {
                return false;
            }

            if (currentWorldX < RebaseThreshold)
            {
                return true;
            }

            double shiftCount = Math.Floor(
                currentWorldX / BaseRebaseOffset);
            double calculatedOffset = shiftCount * BaseRebaseOffset;

            if (!IsFinite(calculatedOffset) || calculatedOffset <= 0.0 ||
                calculatedOffset > float.MaxValue)
            {
                return false;
            }

            rebaseOffset = calculatedOffset;
            return true;
        }

        public bool TryApplyRebaseOffset(double rebaseOffset)
        {
            if (!CanChangeState() ||
                !IsFinite(rebaseOffset) || rebaseOffset <= 0.0 ||
                rebaseOffset > float.MaxValue)
            {
                return false;
            }

            double shiftCount = rebaseOffset / BaseRebaseOffset;

            if (Math.Abs(shiftCount - Math.Round(shiftCount)) > 0.0000001)
            {
                return false;
            }

            double nextOffset = _cumulativeRebaseOffset + rebaseOffset;

            if (!IsFinite(nextOffset))
            {
                return false;
            }

            _cumulativeRebaseOffset = nextOffset;
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
            _originLogicalX = 0.0;
            _cumulativeRebaseOffset = 0.0;
            _maximumForwardDistance = 0.0;
            _isRunning = false;
            _isPaused = false;
            _isFinalized = false;
        }

        private bool CanChangeState()
        {
            return _isRunning && !_isPaused && !_isFinalized;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
