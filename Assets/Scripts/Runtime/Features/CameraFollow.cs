using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _followTarget;

        private float _fixedPositionY;
        private float _fixedPositionZ;
        private bool _isFollowing;

        public bool IsFollowing => _isFollowing;

        private void LateUpdate()
        {
            if (!_isFollowing)
            {
                return;
            }

            if (_player == null || _followTarget == null)
            {
                Debug.LogWarning(
                    "[CameraFollow] Player or Follow Target disappeared. Camera follow stopped.");
                StopFollowing();
                return;
            }

            Vector3 targetPosition = _followTarget.position;
            targetPosition.x = _player.position.x;
            targetPosition.y = _fixedPositionY;
            targetPosition.z = _fixedPositionZ;
            _followTarget.position = targetPosition;
        }

        public void StartFollowing()
        {
            if (_player == null || _followTarget == null)
            {
                Debug.LogError(
                    "[CameraFollow] Player and Follow Target must be assigned before following starts.");
                _isFollowing = false;
                return;
            }

            Vector3 targetPosition = _followTarget.position;
            _fixedPositionY = targetPosition.y;
            _fixedPositionZ = targetPosition.z;
            _isFollowing = true;
        }

        public void StopFollowing()
        {
            _isFollowing = false;
        }
    }
}
