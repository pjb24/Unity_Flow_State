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
    }
}
