namespace FlowState.Runtime.Features
{
    public class InfiniteDistanceState
    {
        private float _originWorldX;
        private float _currentDistance;
        private bool _isInitialized;
        private bool _isFinalized;

        public float OriginWorldX => _originWorldX;

        public float CurrentDistance => _currentDistance;

        public bool IsInitialized => _isInitialized;

        public bool IsFinalized => _isFinalized;

        public bool Initialize(float originWorldX)
        {
            if (!IsFinite(originWorldX))
            {
                return false;
            }

            _originWorldX = originWorldX;
            _currentDistance = 0.0f;
            _isInitialized = true;
            _isFinalized = false;
            return true;
        }

        public bool TryUpdate(float currentWorldX)
        {
            if (!_isInitialized ||
                _isFinalized ||
                !IsFinite(currentWorldX))
            {
                return false;
            }

            float forwardDistance = currentWorldX - _originWorldX;

            if (forwardDistance > _currentDistance)
            {
                _currentDistance = forwardDistance;
            }

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

        public void Reset()
        {
            _originWorldX = 0.0f;
            _currentDistance = 0.0f;
            _isInitialized = false;
            _isFinalized = false;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
