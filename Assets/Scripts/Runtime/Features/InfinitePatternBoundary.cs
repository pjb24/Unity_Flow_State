using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class InfinitePatternBoundary : MonoBehaviour
    {
        [SerializeField] private Collider _playerCollider;
        [SerializeField] private InfiniteMapPattern _mapPattern;
        [SerializeField] private int _boundaryId;

        private bool _isInitialized;
        private bool _isTriggered;

        public int BoundaryId => _boundaryId;

        public bool IsTriggered => _isTriggered;

        public bool Initialize()
        {
            if (_playerCollider == null)
            {
                _isInitialized = false;
                Debug.LogError(
                    "[InfinitePatternBoundary] Player Collider is not assigned.");
                return false;
            }

            if (_mapPattern == null)
            {
                _isInitialized = false;
                Debug.LogError(
                    "[InfinitePatternBoundary] Map Pattern is not assigned.");
                return false;
            }

            ResetBoundary();
            _isInitialized = true;
            return true;
        }

        public void ResetBoundary()
        {
            _isTriggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized ||
                _isTriggered ||
                other != _playerCollider)
            {
                return;
            }

            if (_mapPattern.TryAdvance(_boundaryId))
            {
                _isTriggered = true;
            }
        }
    }
}
