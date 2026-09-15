namespace FlowState.Runtime.Core
{
    public class InfiniteModeRuntimeData
    {
        private float _currentDistance;
        private int _currentScore;
        private int _currentDifficultyLevel;
        private int _scoringVersion;
        private bool _isInitialized;
        private bool _isFinalized;

        public float CurrentDistance => _currentDistance;

        public int CurrentScore => _currentScore;

        public int CurrentDifficultyLevel => _currentDifficultyLevel;

        public int ScoringVersion => _scoringVersion;

        public bool IsInitialized => _isInitialized;

        public bool IsFinalized => _isFinalized;

        public void Initialize()
        {
            Initialize(
                global::FlowState.Runtime.Core.ScoringVersion.LegacyDistanceScore);
        }

        public bool Initialize(int scoringVersion)
        {
            if (_isInitialized ||
                !global::FlowState.Runtime.Core.ScoringVersion.IsSupported(
                    scoringVersion))
            {
                return false;
            }

            _currentDistance = 0.0f;
            _currentScore = 0;
            _currentDifficultyLevel = 1;
            _scoringVersion = scoringVersion;
            _isInitialized = true;
            _isFinalized = false;
            return true;
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

        public bool TryUpdateDifficultyLevel(int difficultyLevel)
        {
            if (!_isInitialized ||
                _isFinalized ||
                difficultyLevel < _currentDifficultyLevel ||
                difficultyLevel > 3)
            {
                return false;
            }

            _currentDifficultyLevel = difficultyLevel;
            return true;
        }

        public void Clear()
        {
            _currentDistance = 0.0f;
            _currentScore = 0;
            _currentDifficultyLevel = 0;
            _scoringVersion = global::FlowState.Runtime.Core.ScoringVersion.None;
            _isInitialized = false;
            _isFinalized = false;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
