using System.Collections.Generic;
using FlowState.Runtime.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlowState.Runtime.Systems
{
    public class CollisionSystem : MonoBehaviour
    {
        private const int GroundHitBufferSize = 16;
        private const int ContactBufferSize = 16;

        private readonly struct ColliderWallContacts
        {
            public bool HasLeftWall { get; }

            public Vector3 LeftWallNormal { get; }

            public bool HasRightWall { get; }

            public Vector3 RightWallNormal { get; }

            public bool HasWallContact => HasLeftWall || HasRightWall;

            public ColliderWallContacts(
                bool hasLeftWall,
                Vector3 leftWallNormal,
                bool hasRightWall,
                Vector3 rightWallNormal)
            {
                HasLeftWall = hasLeftWall;
                LeftWallNormal = leftWallNormal;
                HasRightWall = hasRightWall;
                RightWallNormal = rightWallNormal;
            }
        }

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

        private readonly ContactPoint[] _contactBuffer =
            new ContactPoint[ContactBufferSize];

        private readonly Dictionary<Collider, ColliderWallContacts>
            _wallContactsByCollider =
                new Dictionary<Collider, ColliderWallContacts>();

        private PlayerCollisionState _collisionState;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        private void OnDisable()
        {
            _wallContactsByCollider.Clear();
        }

        private void OnCollisionEnter(Collision collision)
        {
            UpdateWallContacts(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            UpdateWallContacts(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider != null)
            {
                _wallContactsByCollider.Remove(collision.collider);
            }
        }

        public bool Initialize()
        {
            _wallContactsByCollider.Clear();

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
            PlayerWallContactState wallContacts = CreateWallContactState();

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
                    predictionHit.normal,
                    wallContacts);
                return;
            }

            if (!isGrounded)
            {
                _collisionState = CreateNoGroundState(wallContacts);
                return;
            }

            _collisionState = new PlayerCollisionState(
                true,
                groundedHit.distance,
                groundedHit.point,
                groundedHit.normal,
                wallContacts);
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

                if (!PlayerSurfaceMath.IsGroundSurface(hit.normal))
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
            return CreateNoGroundState(default);
        }

        private PlayerCollisionState CreateNoGroundState(
            in PlayerWallContactState wallContacts)
        {
            return new PlayerCollisionState(
                false,
                float.PositiveInfinity,
                Vector3.zero,
                Vector3.up,
                wallContacts);
        }

        private void UpdateWallContacts(Collision collision)
        {
            Collider otherCollider = collision.collider;

            if (!_isInitialized ||
                otherCollider == null ||
                IsPlayerCollider(otherCollider))
            {
                return;
            }

            int contactCount = collision.GetContacts(_contactBuffer);
            bool hasLeftWall = false;
            Vector3 leftWallNormal = Vector3.zero;
            bool hasRightWall = false;
            Vector3 rightWallNormal = Vector3.zero;

            for (int index = 0; index < contactCount; index++)
            {
                Vector3 normal = _contactBuffer[index].normal;

                if (!PlayerSurfaceMath.IsWallSurface(normal))
                {
                    continue;
                }

                if (normal.x > Mathf.Epsilon &&
                    (!hasLeftWall || normal.x > leftWallNormal.x))
                {
                    hasLeftWall = true;
                    leftWallNormal = normal;
                }

                if (normal.x < -Mathf.Epsilon &&
                    (!hasRightWall || normal.x < rightWallNormal.x))
                {
                    hasRightWall = true;
                    rightWallNormal = normal;
                }
            }

            ColliderWallContacts wallContacts = new ColliderWallContacts(
                hasLeftWall,
                leftWallNormal,
                hasRightWall,
                rightWallNormal);

            if (wallContacts.HasWallContact)
            {
                _wallContactsByCollider[otherCollider] = wallContacts;
                return;
            }

            _wallContactsByCollider.Remove(otherCollider);
        }

        private PlayerWallContactState CreateWallContactState()
        {
            bool hasLeftWall = false;
            Vector3 leftWallNormal = Vector3.zero;
            bool hasRightWall = false;
            Vector3 rightWallNormal = Vector3.zero;

            foreach (ColliderWallContacts contacts in
                     _wallContactsByCollider.Values)
            {
                if (contacts.HasLeftWall &&
                    (!hasLeftWall ||
                     contacts.LeftWallNormal.x > leftWallNormal.x))
                {
                    hasLeftWall = true;
                    leftWallNormal = contacts.LeftWallNormal;
                }

                if (contacts.HasRightWall &&
                    (!hasRightWall ||
                     contacts.RightWallNormal.x < rightWallNormal.x))
                {
                    hasRightWall = true;
                    rightWallNormal = contacts.RightWallNormal;
                }
            }

            return new PlayerWallContactState(
                hasLeftWall,
                leftWallNormal,
                hasRightWall,
                rightWallNormal);
        }
    }
}
