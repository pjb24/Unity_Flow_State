namespace FlowState.Runtime.Core
{
    public class InfiniteModeRuntimeData
    {
        private float _currentDistance;
        private int _baseDistanceScore;
        private int _momentumBonus;
        private int _currentScore;
        private int _collectibleScore;
        private int _totalScore;
        private int _currentDifficultyLevel;
        private int _scoringVersion;
        private double _currentMomentumMultiplier;
        private double _maximumMomentumMultiplier;
        private double _momentumRemainingDuration;
        private double _momentumDuration;
        private bool _isInitialized;
        private bool _isFinalized;

        public float CurrentDistance => _currentDistance;

        public int CurrentScore => _currentScore;

        public int BaseDistanceScore => _baseDistanceScore;

        public int MomentumBonus => _momentumBonus;

        public int CollectibleScore => _collectibleScore;

        public int TotalScore => _totalScore;

        public int CurrentDifficultyLevel => _currentDifficultyLevel;

        public int ScoringVersion => _scoringVersion;

        public double CurrentMomentumMultiplier => _currentMomentumMultiplier;

        public double MaximumMomentumMultiplier => _maximumMomentumMultiplier;

        public double MomentumRemainingDuration => _momentumRemainingDuration;

        public double MomentumDuration => _momentumDuration;

        public double MomentumRemainingRatio => _momentumDuration > 0.0
            ? _momentumRemainingDuration / _momentumDuration
            : 0.0;

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
            _baseDistanceScore = 0;
            _momentumBonus = 0;
            _currentScore = 0;
            _collectibleScore = 0;
            _totalScore = 0;
            _currentDifficultyLevel = 1;
            _scoringVersion = scoringVersion;
            _currentMomentumMultiplier = 1.0;
            _maximumMomentumMultiplier = 1.0;
            _momentumRemainingDuration = 0.0;
            _momentumDuration = 0.0;
            _isInitialized = true;
            _isFinalized = false;
            return true;
        }

        public bool TryUpdate(float distance, int score)
        {
            if (!_isInitialized ||
                _scoringVersion !=
                    global::FlowState.Runtime.Core.ScoringVersion.LegacyDistanceScore ||
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
            _baseDistanceScore = score;
            _currentScore = score;
            _totalScore = score;
            return true;
        }

        public bool TryUpdate(
            int scoringVersion,
            float distance,
            int baseDistanceScore,
            int momentumBonus,
            int distanceScore,
            int collectibleScore,
            int totalScore,
            double currentMomentumMultiplier,
            double maximumMomentumMultiplier,
            double momentumRemainingDuration,
            double momentumDuration)
        {
            if (!_isInitialized || _isFinalized ||
                _scoringVersion != scoringVersion ||
                scoringVersion !=
                    global::FlowState.Runtime.Core.ScoringVersion.Current ||
                !IsFinite(distance) || distance < _currentDistance ||
                baseDistanceScore < _baseDistanceScore ||
                momentumBonus < _momentumBonus ||
                distanceScore < _currentScore ||
                collectibleScore < _collectibleScore || totalScore < _totalScore ||
                SaturatedAdd(baseDistanceScore, momentumBonus) != distanceScore ||
                SaturatedAdd(distanceScore, collectibleScore) != totalScore ||
                !IsValidMomentum(
                    currentMomentumMultiplier,
                    maximumMomentumMultiplier,
                    momentumRemainingDuration,
                    momentumDuration))
            {
                return false;
            }

            _currentDistance = distance;
            _baseDistanceScore = baseDistanceScore;
            _momentumBonus = momentumBonus;
            _currentScore = distanceScore;
            _collectibleScore = collectibleScore;
            _totalScore = totalScore;
            _currentMomentumMultiplier = currentMomentumMultiplier;
            _maximumMomentumMultiplier = maximumMomentumMultiplier;
            _momentumRemainingDuration = momentumRemainingDuration;
            _momentumDuration = momentumDuration;
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
            _baseDistanceScore = 0;
            _momentumBonus = 0;
            _currentScore = 0;
            _collectibleScore = 0;
            _totalScore = 0;
            _currentDifficultyLevel = 0;
            _scoringVersion = global::FlowState.Runtime.Core.ScoringVersion.None;
            _currentMomentumMultiplier = 1.0;
            _maximumMomentumMultiplier = 1.0;
            _momentumRemainingDuration = 0.0;
            _momentumDuration = 0.0;
            _isInitialized = false;
            _isFinalized = false;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static int SaturatedAdd(int first, int second)
        {
            return first > int.MaxValue - second
                ? int.MaxValue
                : first + second;
        }

        private static bool IsValidMomentum(
            double currentMultiplier,
            double maximumMultiplier,
            double remainingDuration,
            double duration)
        {
            return IsFinite(currentMultiplier) &&
                   IsFinite(maximumMultiplier) &&
                   IsFinite(remainingDuration) &&
                   IsFinite(duration) &&
                   currentMultiplier >= 1.0 &&
                   currentMultiplier <= 3.0 &&
                   maximumMultiplier >= currentMultiplier &&
                   maximumMultiplier <= 3.0 &&
                   remainingDuration >= 0.0 &&
                   duration >= remainingDuration &&
                   ((currentMultiplier == 1.0 &&
                     remainingDuration == 0.0 && duration == 0.0) ||
                    (currentMultiplier > 1.0 && duration > 0.0));
        }
    }
}
