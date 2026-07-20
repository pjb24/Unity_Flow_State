using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlowState.Runtime.Systems
{
    public class CollisionSystem : MonoBehaviour
    {
        private const int GroundHitBufferSize = 16;

        [SerializeField] private Collider _playerCollider;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _groundCheckRadius = 0.25f;
        [FormerlySerializedAs("_groundCheckDistance")]
        [SerializeField] private float _groundedDistance = 0.02f;
        [SerializeField] private float _groundPredictionDistance = 3.0f;

        private readonly RaycastHit[] _groundedHits =
            new RaycastHit[GroundHitBufferSize];

        private readonly RaycastHit[] _groundPredictionHits =
            new RaycastHit[GroundHitBufferSize];

        private PlayerCollisionState _collisionState;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public bool Initialize()
        {
            if (!HasRequiredReferences())
            {
                _isInitialized = false;
                _collisionState = CreateNoGroundState();
                return false;
            }

            _collisionState = CreateNoGroundState();
            _isInitialized = true;

            return true;
        }

        public void RefreshCollisionState()
        {
            if (!_isInitialized)
            {
                _collisionState = CreateNoGroundState();
                return;
            }

            float radius = Mathf.Max(0.0f, _groundCheckRadius);
            float groundedDistance = Mathf.Max(0.0f, _groundedDistance);
            float predictionDistance = Mathf.Max(
                groundedDistance,
                _groundPredictionDistance);

            int groundedHitCount = Physics.SphereCastNonAlloc(
                _groundCheck.position,
                radius,
                Vector3.down,
                _groundedHits,
                groundedDistance,
                _groundLayer,
                QueryTriggerInteraction.Ignore);
            bool isGrounded = TryGetClosestGroundHit(
                _groundedHits,
                groundedHitCount,
                out RaycastHit groundedHit);

            int predictionHitCount = Physics.SphereCastNonAlloc(
                _groundCheck.position,
                radius,
                Vector3.down,
                _groundPredictionHits,
                predictionDistance,
                _groundLayer,
                QueryTriggerInteraction.Ignore);

            if (TryGetClosestGroundHit(
                    _groundPredictionHits,
                    predictionHitCount,
                    out RaycastHit predictionHit))
            {
                _collisionState = new PlayerCollisionState(
                    isGrounded,
                    predictionHit.distance,
                    predictionHit.point,
                    predictionHit.normal);
                return;
            }

            if (!isGrounded)
            {
                _collisionState = CreateNoGroundState();
                return;
            }

            _collisionState = new PlayerCollisionState(
                true,
                groundedHit.distance,
                groundedHit.point,
                groundedHit.normal);
        }

        public PlayerCollisionState GetCollisionState()
        {
            return _collisionState;
        }

        private bool HasRequiredReferences()
        {
            if (_playerCollider == null)
            {
                Debug.LogError("[CollisionSystem] Player Collider is not assigned.");
                return false;
            }

            if (_groundCheck == null)
            {
                Debug.LogError("[CollisionSystem] Ground Check is not assigned.");
                return false;
            }

            if (_groundLayer.value == 0)
            {
                Debug.LogError("[CollisionSystem] Ground Layer is not assigned.");
                return false;
            }

            return true;
        }

        private bool TryGetClosestGroundHit(
            RaycastHit[] groundHits,
            int hitCount,
            out RaycastHit closestHit)
        {
            closestHit = default;
            float closestDistance = float.PositiveInfinity;
            bool hasGroundHit = false;

            for (int index = 0; index < hitCount; index++)
            {
                RaycastHit hit = groundHits[index];

                if (hit.collider == null || IsPlayerCollider(hit.collider))
                {
                    continue;
                }

                if (hit.distance >= closestDistance)
                {
                    continue;
                }

                closestHit = hit;
                closestDistance = hit.distance;
                hasGroundHit = true;
            }

            return hasGroundHit;
        }

        private bool IsPlayerCollider(Collider hitCollider)
        {
            Transform playerTransform = _playerCollider.transform;
            Transform hitTransform = hitCollider.transform;

            return hitCollider == _playerCollider ||
                   hitTransform == playerTransform ||
                   hitTransform.IsChildOf(playerTransform);
        }

        private PlayerCollisionState CreateNoGroundState()
        {
            return new PlayerCollisionState(
                false,
                float.PositiveInfinity,
                Vector3.zero,
                Vector3.up);
        }
    }
}
