using System;

namespace FlowState.Runtime.Features
{
    public class ScoreCalculator
    {
        private float _scorePerUnit;
        private bool _isInitialized;

        public float ScorePerUnit => _scorePerUnit;

        public bool IsInitialized => _isInitialized;

        public bool Initialize(float scorePerUnit)
        {
            if (!IsFinite(scorePerUnit) || scorePerUnit <= 0.0f)
            {
                return false;
            }

            _scorePerUnit = scorePerUnit;
            _isInitialized = true;
            return true;
        }

        public bool TryCalculate(float distance, out int score)
        {
            score = 0;

            if (!_isInitialized ||
                !IsFinite(distance) ||
                distance < 0.0f)
            {
                return false;
            }

            float calculatedScore = distance * _scorePerUnit;

            if (float.IsInfinity(calculatedScore) ||
                calculatedScore >= int.MaxValue)
            {
                score = int.MaxValue;
                return true;
            }

            score = (int)Math.Floor(calculatedScore);
            return true;
        }

        public void Reset()
        {
            _scorePerUnit = 0.0f;
            _isInitialized = false;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
