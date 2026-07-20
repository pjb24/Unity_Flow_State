using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class PlayerControllerSystem : MonoBehaviour
    {
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private Transform _startPoint;

        private float _currentHorizontalAcceleration;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public Vector3 CurrentVelocity =>
            _playerRigidbody == null
                ? Vector3.zero
                : _playerRigidbody.linearVelocity;

        public float CurrentHorizontalAcceleration =>
            _currentHorizontalAcceleration;

        public bool Initialize()
        {
            if (_playerRigidbody == null || _startPoint == null)
            {
                Debug.LogError(
                    "[PlayerControllerSystem] Player Rigidbody or Start Point is not assigned.");
                _isInitialized = false;
                return false;
            }

            _playerRigidbody.useGravity = false;
            _playerRigidbody.constraints |=
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;
            ResetToStartPoint();
            _currentHorizontalAcceleration = 0.0f;
            _isInitialized = true;

            return true;
        }

        public Vector3 GetVelocity()
        {
            if (!_isInitialized)
            {
                return Vector3.zero;
            }

            return _playerRigidbody.linearVelocity;
        }

        public Vector3 GetPosition()
        {
            if (!_isInitialized)
            {
                return transform.position;
            }

            return _playerRigidbody.position;
        }

        public void ApplyMovement(in PlayerMovementResult movementResult)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[PlayerControllerSystem] System is not initialized.");
                return;
            }

            Vector3 currentVelocity = _playerRigidbody.linearVelocity;
            Vector3 resultVelocity = movementResult.Velocity;
            resultVelocity.z = 0.0f;

            _currentHorizontalAcceleration =
                PlayerMovementMath.CalculateSignedHorizontalAcceleration(
                    currentVelocity.x,
                    resultVelocity.x,
                    Time.fixedDeltaTime);
            _playerRigidbody.linearVelocity = resultVelocity;
        }

        public void StopMovement()
        {
            if (!_isInitialized)
            {
                return;
            }

            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.angularVelocity = Vector3.zero;
            _currentHorizontalAcceleration = 0.0f;
        }

        private void ResetToStartPoint()
        {
            _playerRigidbody.position = _startPoint.position;
            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
