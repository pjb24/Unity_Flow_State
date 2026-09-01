namespace FlowState.Runtime.Core
{
    public class InfiniteModeRuntimeData
    {
        private float _currentDistance;
        private int _currentScore;
        private bool _isInitialized;
        private bool _isFinalized;

        public float CurrentDistance => _currentDistance;

        public int CurrentScore => _currentScore;

        public bool IsInitialized => _isInitialized;

        public bool IsFinalized => _isFinalized;

        public void Initialize()
        {
            _currentDistance = 0.0f;
            _currentScore = 0;
            _isInitialized = true;
            _isFinalized = false;
        }

        public bool TryUpdate(float distance, int score)
        {
            if (!_isInitialized ||
                _isFinalized ||
                !IsFinite(distance) ||
                distance < 0.0f ||
                score < 0 ||
                distance < _currentDistance ||
                score < _currentScore)
            {
                return false;
            }

            _currentDistance = distance;
            _currentScore = score;
            return true;
        }

        public bool TryFinalize()
        {
            if (!_isInitialized || _isFinalized)
            {
                return false;
            }

            _isFinalized = true;
            return true;
        }

        public void Clear()
        {
            _currentDistance = 0.0f;
            _currentScore = 0;
            _isInitialized = false;
            _isFinalized = false;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
