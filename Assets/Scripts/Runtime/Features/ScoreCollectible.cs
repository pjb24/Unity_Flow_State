using System.Collections.Generic;
using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class ScoreCollectible : MonoBehaviour
    {
        [SerializeField] private string _collectibleId;
        [SerializeField] private Collider _triggerCollider;
        [SerializeField] private Renderer _visual;
        [SerializeField] private LayerMask _playerLayers = 1;

        private GameRuntimeData _runtimeData;
        private CollectibleRuntimeData _collectibleData;
        private Rigidbody _player;
        private Collider[] _playerColliders;
        private readonly HashSet<Collider> _overlappingPlayerColliders =
            new HashSet<Collider>();
        private long _scopeId;
        private bool _isCollected;

        public bool IsBound => _runtimeData != null;

        public bool IsCollected => _isCollected;

        private void OnDisable()
        {
            Unbind();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void OnTriggerEnter(Collider other)
        {
            TrackPlayerContact(other);
            TryCollect(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TrackPlayerContact(other);
            TryCollect(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _overlappingPlayerColliders.Remove(other);
        }

        public bool Bind(GameRuntimeData runtimeData, Rigidbody player, long scopeId)
        {
            if (IsBound || !isActiveAndEnabled ||
                runtimeData == null || !runtimeData.IsCreated ||
                runtimeData.CollectibleRuntimeData == null || player == null ||
                _triggerCollider == null || _visual == null ||
                _triggerCollider.gameObject != gameObject ||
                !_triggerCollider.isTrigger ||
                !HasSupportedTriggerShape() ||
                !runtimeData.CollectibleRuntimeData.TryRegister(scopeId, _collectibleId))
            {
                return false;
            }

            _runtimeData = runtimeData;
            _collectibleData = runtimeData.CollectibleRuntimeData;
            _player = player;
            _playerColliders = player.GetComponentsInChildren<Collider>(true);
            _scopeId = scopeId;
            _isCollected = false;
            _overlappingPlayerColliders.Clear();
            _visual.enabled = true;
            _triggerCollider.enabled = true;
            return true;
        }

        public void Unbind()
        {
            _runtimeData = null;
            _collectibleData = null;
            _player = null;
            _playerColliders = null;
            _scopeId = 0;
            _overlappingPlayerColliders.Clear();
            SetPresentationInactive();
        }

        public bool TryCollectOverlappingPlayer()
        {
            if (!CanCollect() || _playerColliders == null)
            {
                return false;
            }

            Collider trackedOverlap = null;
            foreach (Collider overlap in _overlappingPlayerColliders)
            {
                if (IsRegisteredPlayerCollider(overlap) &&
                    IsSphereOverlapping(overlap))
                {
                    trackedOverlap = overlap;
                    break;
                }
            }

            if (trackedOverlap != null && TryCollect(trackedOverlap))
            {
                return true;
            }

            foreach (Collider playerCollider in _playerColliders)
            {
                if (IsRegisteredPlayerCollider(playerCollider) &&
                    IsSphereOverlapping(playerCollider) &&
                    TryCollect(playerCollider))
                {
                    return true;
                }
            }

            return false;
        }

        private void TrackPlayerContact(Collider other)
        {
            if (IsRegisteredPlayerCollider(other))
            {
                _overlappingPlayerColliders.Add(other);
            }
        }

        private bool IsSphereOverlapping(Collider playerCollider)
        {
            SphereCollider sphere = (SphereCollider)_triggerCollider;
            Transform triggerTransform = sphere.transform;
            Vector3 scale = triggerTransform.lossyScale;
            Vector3 center = triggerTransform.TransformPoint(sphere.center);
            float radius = sphere.radius * Mathf.Max(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y),
                Mathf.Abs(scale.z));
            Vector3 closestPoint = playerCollider.ClosestPoint(center);
            return (closestPoint - center).sqrMagnitude <= radius * radius;
        }

        private bool TryCollect(Collider other)
        {
            if (!CanCollect() || !IsRegisteredPlayerCollider(other) ||
                !_collectibleData.TryCollect(_scopeId, _collectibleId))
            {
                return false;
            }

            _isCollected = true;
            SetPresentationInactive();
            return true;
        }

        private bool CanCollect()
        {
            return isActiveAndEnabled && !_isCollected &&
                   _triggerCollider != null && _triggerCollider.enabled &&
                   _runtimeData != null && _runtimeData.IsCreated &&
                   _runtimeData.GameState == E_GameState.Playing &&
                   _collectibleData != null && _collectibleData.IsInitialized &&
                   ReferenceEquals(_runtimeData.CollectibleRuntimeData, _collectibleData) &&
                   _player != null;
        }

        private bool IsRegisteredPlayerCollider(Collider other)
        {
            return other != null && other.enabled && other.gameObject.activeInHierarchy &&
                   other.attachedRigidbody == _player &&
                   (_playerLayers.value & (1 << other.gameObject.layer)) != 0;
        }

        private bool HasSupportedTriggerShape()
        {
            return _triggerCollider is SphereCollider;
        }

        private void SetPresentationInactive()
        {
            if (_triggerCollider != null)
            {
                _triggerCollider.enabled = false;
            }

            if (_visual != null)
            {
                _visual.enabled = false;
            }
        }
    }
}
