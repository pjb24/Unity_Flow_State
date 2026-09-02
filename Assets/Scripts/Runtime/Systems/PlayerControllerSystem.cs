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
        private bool _isPaused;
        private Vector3 _pausedLinearVelocity;
        private Vector3 _pausedAngularVelocity;
        private float _pausedHorizontalAcceleration;
        private RigidbodyConstraints _pausedConstraints;

        public bool IsInitialized => _isInitialized;

        public bool IsPaused => _isPaused;

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
            _isPaused = false;
            _isInitialized = true;

            return true;
        }

        public Vector3 GetVelocity()
        {
            if (!_isInitialized || _isPaused)
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
            if (!_isInitialized || _isPaused)
            {
                if (!_isInitialized)
                {
                    Debug.LogError("[PlayerControllerSystem] System is not initialized.");
                }

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

            if (_isPaused)
            {
                _playerRigidbody.constraints = _pausedConstraints;
                _isPaused = false;
            }

            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.angularVelocity = Vector3.zero;
            _currentHorizontalAcceleration = 0.0f;
        }

        public bool PausePhysics()
        {
            if (!_isInitialized || _isPaused)
            {
                return false;
            }

            _pausedLinearVelocity = _playerRigidbody.linearVelocity;
            _pausedAngularVelocity = _playerRigidbody.angularVelocity;
            _pausedHorizontalAcceleration = _currentHorizontalAcceleration;
            _pausedConstraints = _playerRigidbody.constraints;
            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.angularVelocity = Vector3.zero;
            _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _isPaused = true;
            return true;
        }

        public bool ResumePhysics()
        {
            if (!_isInitialized || !_isPaused)
            {
                return false;
            }

            _playerRigidbody.constraints = _pausedConstraints;
            _playerRigidbody.linearVelocity = _pausedLinearVelocity;
            _playerRigidbody.angularVelocity = _pausedAngularVelocity;
            _currentHorizontalAcceleration = _pausedHorizontalAcceleration;
            _isPaused = false;
            return true;
        }

        private void ResetToStartPoint()
        {
            _playerRigidbody.position = _startPoint.position;
            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
