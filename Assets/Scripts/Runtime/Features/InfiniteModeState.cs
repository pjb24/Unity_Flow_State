using System;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public class InfiniteModeState
    {
        private E_GameMode _gameMode;
        private float _minimumHorizontalSpeed;
        private float _startGraceDuration;
        private float _belowSpeedGraceDuration;
        private float _playDuration;
        private float _belowSpeedDuration;
        private int _lastPatternBoundaryId;
        private bool _isInitialized;
        private bool _isPlaying;
        private bool _hasEnded;
        private bool _hasPatternBoundary;

        public E_GameMode GameMode => _gameMode;

        public bool IsPlaying => _isPlaying;

        public bool HasEnded => _hasEnded;

        public bool Initialize(
            float minimumHorizontalSpeed,
            float startGraceDuration,
            float belowSpeedGraceDuration)
        {
            if (minimumHorizontalSpeed < 0.0f ||
                startGraceDuration < 0.0f ||
                belowSpeedGraceDuration < 0.0f)
            {
                return false;
            }

            _minimumHorizontalSpeed = minimumHorizontalSpeed;
            _startGraceDuration = startGraceDuration;
            _belowSpeedGraceDuration = belowSpeedGraceDuration;
            _gameMode = E_GameMode.Stage;
            _isInitialized = true;
            ResetRunState();
            return true;
        }

        public bool SetGameMode(E_GameMode gameMode)
        {
            if (!_isInitialized || _isPlaying)
            {
                return false;
            }

            _gameMode = gameMode;
            return true;
        }

        public bool Start()
        {
            if (!_isInitialized || _isPlaying)
            {
                return false;
            }

            ResetRunState();
            _isPlaying = true;
            return true;
        }

        public void Reset()
        {
            ResetRunState();
        }

        public bool UpdateProgress(float horizontalSpeed, float deltaTime)
        {
            if (!CanProcessInfiniteMode() || deltaTime < 0.0f)
            {
                return false;
            }

            float activeDeltaTime = GetActiveSpeedCheckDeltaTime(deltaTime);

            if (activeDeltaTime <= 0.0f)
            {
                return false;
            }

            if (Math.Abs(horizontalSpeed) >= _minimumHorizontalSpeed)
            {
                _belowSpeedDuration = 0.0f;
                return false;
            }

            _belowSpeedDuration += activeDeltaTime;

            if (_belowSpeedDuration < _belowSpeedGraceDuration)
            {
                return false;
            }

            return End();
        }

        public bool NotifyFallThresholdReached()
        {
            if (!CanProcessInfiniteMode())
            {
                return false;
            }

            return End();
        }

        public bool NotifyGoalReached()
        {
            return false;
        }

        public bool TryRequestPatternAdvance(int patternBoundaryId)
        {
            if (!CanProcessInfiniteMode())
            {
                return false;
            }

            if (_hasPatternBoundary &&
                _lastPatternBoundaryId == patternBoundaryId)
            {
                return false;
            }

            _lastPatternBoundaryId = patternBoundaryId;
            _hasPatternBoundary = true;
            return true;
        }

        private bool CanProcessInfiniteMode()
        {
            return _isInitialized &&
                   _isPlaying &&
                   !_hasEnded &&
                   _gameMode == E_GameMode.Infinite;
        }

        private float GetActiveSpeedCheckDeltaTime(float deltaTime)
        {
            float previousPlayDuration = _playDuration;
            _playDuration += deltaTime;

            if (previousPlayDuration >= _startGraceDuration)
            {
                return deltaTime;
            }

            if (_playDuration <= _startGraceDuration)
            {
                return 0.0f;
            }

            return _playDuration - _startGraceDuration;
        }

        private bool End()
        {
            if (_hasEnded)
            {
                return false;
            }

            _isPlaying = false;
            _hasEnded = true;
            return true;
        }

        private void ResetRunState()
        {
            _playDuration = 0.0f;
            _belowSpeedDuration = 0.0f;
            _lastPatternBoundaryId = 0;
            _isPlaying = false;
            _hasEnded = false;
            _hasPatternBoundary = false;
        }
    }
}
