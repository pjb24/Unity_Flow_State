using Unity.Cinemachine;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private Transform _followTarget;
        [SerializeField] private float _orthographicSize = 5.0f;

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public bool IsCameraActive =>
            _isInitialized && _cinemachineCamera.enabled;

        public Transform FollowTarget => _followTarget;

        public bool Initialize()
        {
            if (!HasRequiredReferences())
            {
                _isInitialized = false;
                return false;
            }

            LensSettings lens = _cinemachineCamera.Lens;
            lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            lens.OrthographicSize = Mathf.Max(0.01f, _orthographicSize);

            _cinemachineCamera.Lens = lens;
            _cinemachineCamera.Target.TrackingTarget = _followTarget;
            _cinemachineCamera.enabled = true;
            _isInitialized = true;

            return true;
        }

        public void SetCameraActive(bool isActive)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[CameraSystem] System is not initialized.");
                return;
            }

            _cinemachineCamera.enabled = isActive;
        }

        public bool CanApplyWorldRebaseOffset(float worldXOffset)
        {
            return _isInitialized &&
                   _cinemachineCamera != null &&
                   _followTarget != null &&
                   _followTarget.parent != null &&
                   IsValidWorldXOffset(worldXOffset);
        }

        public bool TryApplyWorldRebaseOffset(float worldXOffset)
        {
            if (!CanApplyWorldRebaseOffset(worldXOffset))
            {
                return false;
            }

            Transform cameraRig = _followTarget.parent;
            Vector3 position = cameraRig.position;
            position.x += worldXOffset;
            cameraRig.position = position;
            return true;
        }

        public bool TryNotifyWorldRebase(Vector3 positionDelta)
        {
            if (!_isInitialized || _cinemachineCamera == null ||
                _followTarget == null || !IsFinite(positionDelta))
            {
                return false;
            }

            _cinemachineCamera.OnTargetObjectWarped(
                _followTarget, positionDelta);
            return true;
        }

        private bool HasRequiredReferences()
        {
            if (_cinemachineCamera == null)
            {
                Debug.LogError("[CameraSystem] Cinemachine Camera is not assigned.");
                return false;
            }

            if (_followTarget == null)
            {
                Debug.LogError("[CameraSystem] Follow Target is not assigned.");
                return false;
            }

            return true;
        }

        private static bool IsValidWorldXOffset(float worldXOffset)
        {
            return !float.IsNaN(worldXOffset) &&
                   !float.IsInfinity(worldXOffset) &&
                   worldXOffset < 0.0f;
        }

        private static bool IsFinite(Vector3 value)
        {
            return !float.IsNaN(value.x) && !float.IsInfinity(value.x) &&
                   !float.IsNaN(value.y) && !float.IsInfinity(value.y) &&
                   !float.IsNaN(value.z) && !float.IsInfinity(value.z);
        }
    }
}
