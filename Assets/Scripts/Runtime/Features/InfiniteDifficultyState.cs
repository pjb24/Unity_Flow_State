namespace FlowState.Runtime.Features
{
    public class InfiniteDifficultyState
    {
        public const float D2StartDistance = 220.0f;
        public const float D3StartDistance = 440.0f;

        private float _maximumForwardDistance;
        private E_InfinitePatternDifficulty _currentDifficulty;
        private bool _isInitialized;
        private bool _isRunning;
        private bool _isPaused;
        private bool _hasEnded;

        public float MaximumForwardDistance => _maximumForwardDistance;

        public E_InfinitePatternDifficulty CurrentDifficulty =>
            _currentDifficulty;

        public bool IsInitialized => _isInitialized;

        public bool IsRunning => _isRunning;

        public bool IsPaused => _isPaused;

        public bool HasEnded => _hasEnded;

        public void Initialize()
        {
            _isInitialized = true;
            ResetRunState();
        }

        public bool StartRun()
        {
            if (!_isInitialized || _isRunning)
            {
                return false;
            }

            ResetRunState();
            _isRunning = true;
            return true;
        }

        public bool TryUpdate(float maximumForwardDistance)
        {
            if (!_isInitialized ||
                !_isRunning ||
                _isPaused ||
                _hasEnded ||
                !IsFinite(maximumForwardDistance) ||
                maximumForwardDistance < 0.0f ||
                maximumForwardDistance < _maximumForwardDistance)
            {
                return false;
            }

            _maximumForwardDistance = maximumForwardDistance;
            _currentDifficulty = CalculateDifficulty(maximumForwardDistance);
            return true;
        }

        public bool Pause()
        {
            if (!_isInitialized || !_isRunning || _isPaused || _hasEnded)
            {
                return false;
            }

            _isPaused = true;
            return true;
        }

        public bool Resume()
        {
            if (!_isInitialized || !_isRunning || !_isPaused || _hasEnded)
            {
                return false;
            }

            _isPaused = false;
            return true;
        }

        public bool EndRun()
        {
            if (!_isInitialized || !_isRunning || _hasEnded)
            {
                return false;
            }

            _isRunning = false;
            _isPaused = false;
            _hasEnded = true;
            return true;
        }

        private static E_InfinitePatternDifficulty CalculateDifficulty(
            float maximumForwardDistance)
        {
            if (maximumForwardDistance >= D3StartDistance)
            {
                return E_InfinitePatternDifficulty.D3;
            }

            if (maximumForwardDistance >= D2StartDistance)
            {
                return E_InfinitePatternDifficulty.D2;
            }

            return E_InfinitePatternDifficulty.D1;
        }

        private void ResetRunState()
        {
            _maximumForwardDistance = 0.0f;
            _currentDifficulty = E_InfinitePatternDifficulty.D1;
            _isRunning = false;
            _isPaused = false;
            _hasEnded = false;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
