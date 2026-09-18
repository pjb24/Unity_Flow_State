namespace FlowState.Runtime.Features
{
    public class InfiniteDistanceState
    {
        private double _originLogicalX;
        private double _currentDistance;
        private bool _isInitialized;
        private bool _isFinalized;

        public double OriginWorldX => _originLogicalX;

        public double CurrentDistance => _currentDistance;

        public bool IsInitialized => _isInitialized;

        public bool IsFinalized => _isFinalized;

        public bool Initialize(double originLogicalX)
        {
            if (!IsFinite(originLogicalX))
            {
                return false;
            }

            _originLogicalX = originLogicalX;
            _currentDistance = 0.0f;
            _isInitialized = true;
            _isFinalized = false;
            return true;
        }

        public bool TryUpdate(double currentLogicalX)
        {
            if (!_isInitialized ||
                _isFinalized ||
                !IsFinite(currentLogicalX))
            {
                return false;
            }

            double forwardDistance = currentLogicalX - _originLogicalX;

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
            _originLogicalX = 0.0;
            _currentDistance = 0.0f;
            _isInitialized = false;
            _isFinalized = false;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
