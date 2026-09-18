using System.Collections.Generic;
using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class InfiniteMapPattern : MonoBehaviour
    {
        private const int FirstPatternIndex = 0;
        private const int SecondPatternIndex = 1;

        [SerializeField] private Transform _firstPattern;
        [SerializeField] private Transform _firstStartAnchor;
        [SerializeField] private Transform _firstEndAnchor;
        [SerializeField] private InfinitePatternBoundary _firstBoundary;
        [SerializeField] private Transform _secondPattern;
        [SerializeField] private Transform _secondStartAnchor;
        [SerializeField] private Transform _secondEndAnchor;
        [SerializeField] private InfinitePatternBoundary _secondBoundary;
        [SerializeField] private InfinitePatternSlot _firstSlot;
        [SerializeField] private InfinitePatternSlot _secondSlot;
        [SerializeField] private InfinitePatternAuthoring[] _patternPrefabs;
        [SerializeField] private Collider _phase2PlayerCollider;

        private Vector3 _firstInitialPosition;
        private Quaternion _firstInitialRotation;
        private Vector3 _secondInitialPosition;
        private Quaternion _secondInitialRotation;
        private int _trailingPatternIndex;
        private int _advanceCount;
        private bool _hasInitialTransforms;
        private bool _isInitialized;
        private GameRuntimeData _runtimeData;
        private Rigidbody _playerRigidbody;
        private readonly List<ScoreCollectible> _firstCollectibles =
            new List<ScoreCollectible>();
        private readonly List<ScoreCollectible> _secondCollectibles =
            new List<ScoreCollectible>();
        private long _firstCollectibleScopeId;
        private long _secondCollectibleScopeId;
        private InfinitePatternCatalog _phase2Catalog;
        private string _pendingPatternId;
        private long _pendingRequestId;
        private long _lastProcessedRequestId;
        private bool _hasPendingRequest;
        private bool _hasProcessedRequest;
        private bool _usesPatternSlots;

        public bool IsInitialized => _isInitialized;

        public int AdvanceCount => _advanceCount;

        public string CurrentPatternId => _usesPatternSlots &&
            _firstSlot != null && _secondSlot != null
                ? GetSlot(GetFrontPatternIndex()).CurrentPatternId
                : null;

        public string TrailingPatternId => _usesPatternSlots &&
            _firstSlot != null && _secondSlot != null
                ? GetSlot(_trailingPatternIndex).CurrentPatternId
                : null;

        private void OnEnable()
        {
            if (_usesPatternSlots && _isInitialized)
            {
                ResetPatternSlots();
                return;
            }

            if (_hasInitialTransforms)
            {
                Initialize();
            }
        }

        private void OnDisable()
        {
            UnbindCollectibles();
        }

        private void OnDestroy()
        {
            ClearPatternSlots();
        }

        private void Start()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        public bool Initialize()
        {
            if (_usesPatternSlots && _isInitialized)
            {
                return ResetPatternSlots();
            }

            _isInitialized = false;

            if (_firstSlot != null || _secondSlot != null ||
                (_patternPrefabs != null && _patternPrefabs.Length > 0))
            {
                return InitializePatternSlots();
            }

            if (!HasRequiredReferences())
            {
                return false;
            }

            if (!HasValidBoundaryIds())
            {
                return false;
            }

            if (!_firstBoundary.Initialize() ||
                !_secondBoundary.Initialize())
            {
                return false;
            }

            if (!_hasInitialTransforms)
            {
                CaptureInitialTransforms();
            }

            _isInitialized = true;
            ResetPatterns();
            return true;
        }

        public bool ResetPatterns()
        {
            if (_usesPatternSlots)
            {
                return ResetPatternSlots();
            }

            if (!_isInitialized || !_hasInitialTransforms)
            {
                return false;
            }

            _firstPattern.SetPositionAndRotation(
                _firstInitialPosition,
                _firstInitialRotation);
            _secondPattern.SetPositionAndRotation(
                _secondInitialPosition,
                _secondInitialRotation);
            _firstBoundary.ResetBoundary();
            _secondBoundary.ResetBoundary();
            _trailingPatternIndex = FirstPatternIndex;
            _advanceCount = 0;

            if (_runtimeData != null)
            {
                ReleasePatternCollectibles(FirstPatternIndex);
                ReleasePatternCollectibles(SecondPatternIndex);

                if (!BindPatternCollectibles(FirstPatternIndex) ||
                    !BindPatternCollectibles(SecondPatternIndex))
                {
                    UnbindCollectibles();
                    return false;
                }
            }

            return true;
        }

        public bool ConfigureCollectibles(
            GameRuntimeData runtimeData,
            Rigidbody playerRigidbody)
        {
            UnbindCollectibles();

            if (!_isInitialized || runtimeData == null ||
                !runtimeData.IsCreated ||
                runtimeData.CollectibleRuntimeData == null ||
                playerRigidbody == null)
            {
                return false;
            }

            _runtimeData = runtimeData;
            _playerRigidbody = playerRigidbody;

            if (!BindPatternCollectibles(FirstPatternIndex) ||
                !BindPatternCollectibles(SecondPatternIndex))
            {
                UnbindCollectibles();
                return false;
            }

            return true;
        }

        public void UnbindCollectibles()
        {
            ReleasePatternCollectibles(FirstPatternIndex);
            ReleasePatternCollectibles(SecondPatternIndex);
            _runtimeData = null;
            _playerRigidbody = null;
        }

        public void RecheckCollectibleOverlaps()
        {
            RecheckCollectibles(_firstCollectibles);
            RecheckCollectibles(_secondCollectibles);
        }

        public bool TryApplyWorldRebaseOffset(float worldXOffset)
        {
            if (!CanApplyWorldRebaseOffset(worldXOffset))
            {
                return false;
            }

            Transform infiniteModeRoot = transform.parent;
            Vector3 position = infiniteModeRoot.position;
            position.x += worldXOffset;
            infiniteModeRoot.position = position;
            return true;
        }

        public bool CanApplyWorldRebaseOffset(float worldXOffset)
        {
            return _isInitialized &&
                   transform.parent != null &&
                   IsValidWorldXOffset(worldXOffset);
        }

        public bool TryAdvance(int boundaryId)
        {
            if (_usesPatternSlots)
            {
                return TryAdvancePatternSlots(boundaryId);
            }

            if (!_isInitialized)
            {
                return false;
            }

            int frontPatternIndex = GetFrontPatternIndex();

            if (boundaryId != frontPatternIndex)
            {
                return false;
            }

            int patternToReuse = _trailingPatternIndex;
            ReleasePatternCollectibles(patternToReuse);
            MoveTrailingPatternAfterFront();

            if (_runtimeData != null &&
                !BindPatternCollectibles(patternToReuse))
            {
                Debug.LogError(
                    "[InfiniteMapPattern] Reused Pattern Collectibles could not be bound.");
                return false;
            }

            _trailingPatternIndex = frontPatternIndex;
            _advanceCount++;
            return true;
        }

        public bool TryRequestNextPattern(long requestId, string patternId)
        {
            if (!_isInitialized || !_usesPatternSlots ||
                _hasPendingRequest ||
                (_hasProcessedRequest && requestId <= _lastProcessedRequestId) ||
                !_phase2Catalog.Contains(patternId) ||
                !_phase2Catalog.CanConnect(CurrentPatternId, patternId))
            {
                return false;
            }

            _pendingRequestId = requestId;
            _pendingPatternId = patternId;
            _hasPendingRequest = true;
            return true;
        }

        public void ClearPatternSlots()
        {
            if (!_usesPatternSlots)
            {
                return;
            }

            UnbindCollectibles();
            if (_firstSlot != null)
            {
                _firstSlot.Clear();
            }

            if (_secondSlot != null)
            {
                _secondSlot.Clear();
            }
            _phase2Catalog = null;
            _pendingPatternId = null;
            _hasPendingRequest = false;
            _hasProcessedRequest = false;
            _usesPatternSlots = false;
            _isInitialized = false;
        }

        private bool InitializePatternSlots()
        {
            if (_firstSlot == null || _secondSlot == null ||
                _firstSlot == _secondSlot ||
                _phase2PlayerCollider == null ||
                _firstSlot.AdvanceBoundary == null ||
                _secondSlot.AdvanceBoundary == null ||
                _firstSlot.AdvanceBoundary.PlayerCollider !=
                    _phase2PlayerCollider ||
                _secondSlot.AdvanceBoundary.PlayerCollider !=
                    _phase2PlayerCollider ||
                _patternPrefabs == null ||
                !InfinitePatternCatalogFactory.TryCreate(
                    out InfinitePatternCatalog catalog) ||
                _patternPrefabs.Length != catalog.Count)
            {
                return false;
            }

            if (!_firstSlot.InitializeFromPrefabs(
                    catalog, _patternPrefabs,
                    InfinitePatternCatalogFactory.FlatId))
            {
                return false;
            }

            if (!_secondSlot.InitializeFromPrefabs(
                    catalog, _patternPrefabs,
                    InfinitePatternCatalogFactory.FlatId) ||
                !_firstSlot.CanPairWith(_secondSlot) ||
                !_firstSlot.AdvanceBoundary.Initialize() ||
                !_secondSlot.AdvanceBoundary.Initialize())
            {
                _firstSlot.Clear();
                _secondSlot.Clear();
                return false;
            }

            _phase2Catalog = catalog;
            _usesPatternSlots = true;
            _isInitialized = true;

            if (!ResetPatternSlots())
            {
                ClearPatternSlots();
                return false;
            }

            return true;
        }

        private bool ResetPatternSlots()
        {
            if (!_isInitialized || !_usesPatternSlots)
            {
                return false;
            }

            ReleasePatternCollectibles(FirstPatternIndex);
            ReleasePatternCollectibles(SecondPatternIndex);

            if (!_firstSlot.ResetToInitialPattern() ||
                !_secondSlot.ResetToInitialPattern() ||
                !_firstSlot.TryGetCurrentPattern(
                    out InfinitePatternAuthoring firstPattern) ||
                !_secondSlot.TryAlignStartTo(firstPattern.EndAnchor.position))
            {
                return false;
            }

            _trailingPatternIndex = FirstPatternIndex;
            _advanceCount = 0;
            _pendingPatternId = null;
            _hasPendingRequest = false;
            _hasProcessedRequest = false;
            _pendingRequestId = 0;
            _lastProcessedRequestId = 0;

            if (_runtimeData != null &&
                (!BindPatternCollectibles(FirstPatternIndex) ||
                 !BindPatternCollectibles(SecondPatternIndex)))
            {
                UnbindCollectibles();
                return false;
            }

            return true;
        }

        private bool TryAdvancePatternSlots(int boundaryId)
        {
            if (!_isInitialized || !_hasPendingRequest ||
                boundaryId != GetFrontPatternIndex())
            {
                return false;
            }

            InfinitePatternSlot trailing = GetSlot(_trailingPatternIndex);
            InfinitePatternSlot front = GetSlot(GetFrontPatternIndex());

            if (!trailing.TryGetCurrentPattern(
                    out InfinitePatternAuthoring oldPattern) ||
                !front.TryGetCurrentPattern(
                    out InfinitePatternAuthoring frontPattern) ||
                _phase2PlayerCollider == null ||
                !_phase2PlayerCollider.enabled ||
                !_phase2PlayerCollider.gameObject.activeInHierarchy ||
                _phase2PlayerCollider.bounds.min.x <=
                    oldPattern.EndAnchor.position.x +
                    InfinitePatternDefinition.PositionTolerance ||
                !_phase2Catalog.CanConnect(
                    front.CurrentPatternId, _pendingPatternId))
            {
                return false;
            }

            string previousId = trailing.CurrentPatternId;
            Vector3 previousPosition = trailing.transform.position;
            Quaternion previousRotation = trailing.transform.rotation;
            ReleasePatternCollectibles(_trailingPatternIndex);

            if (!trailing.TryActivatePattern(_pendingPatternId) ||
                !trailing.TryAlignStartTo(
                    frontPattern.EndAnchor.position) ||
                (_runtimeData != null &&
                 !BindPatternCollectibles(_trailingPatternIndex)))
            {
                ReleasePatternCollectibles(_trailingPatternIndex);
                trailing.TryActivatePattern(previousId);
                trailing.transform.SetPositionAndRotation(
                    previousPosition, previousRotation);
                trailing.TryActivatePattern(previousId);
                Physics.SyncTransforms();
                if (_runtimeData != null)
                {
                    BindPatternCollectibles(_trailingPatternIndex);
                }
                return false;
            }

            _trailingPatternIndex = front.SlotId;
            _advanceCount++;
            _lastProcessedRequestId = _pendingRequestId;
            _hasProcessedRequest = true;
            _hasPendingRequest = false;
            _pendingPatternId = null;
            return true;
        }

        private InfinitePatternSlot GetSlot(int slotId)
        {
            return slotId == FirstPatternIndex ? _firstSlot : _secondSlot;
        }

        private bool HasRequiredReferences()
        {
            if (_firstPattern == null ||
                _firstStartAnchor == null ||
                _firstEndAnchor == null ||
                _firstBoundary == null ||
                _secondPattern == null ||
                _secondStartAnchor == null ||
                _secondEndAnchor == null ||
                _secondBoundary == null)
            {
                Debug.LogError(
                    "[InfiniteMapPattern] Required reference is not assigned.");
                return false;
            }

            if (_firstPattern == _secondPattern)
            {
                Debug.LogError(
                    "[InfiniteMapPattern] Pattern instances must be different.");
                return false;
            }

            return true;
        }

        private bool HasValidBoundaryIds()
        {
            if (_firstBoundary.BoundaryId != FirstPatternIndex ||
                _secondBoundary.BoundaryId != SecondPatternIndex)
            {
                Debug.LogError(
                    "[InfiniteMapPattern] Boundary IDs must match Pattern indices.");
                return false;
            }

            return true;
        }

        private void CaptureInitialTransforms()
        {
            _firstInitialPosition = _firstPattern.position;
            _firstInitialRotation = _firstPattern.rotation;
            _secondInitialPosition = _secondPattern.position;
            _secondInitialRotation = _secondPattern.rotation;
            _hasInitialTransforms = true;
        }

        private int GetFrontPatternIndex()
        {
            return _trailingPatternIndex == FirstPatternIndex
                ? SecondPatternIndex
                : FirstPatternIndex;
        }

        private void MoveTrailingPatternAfterFront()
        {
            if (_trailingPatternIndex == FirstPatternIndex)
            {
                AlignPatternStartToEnd(
                    _firstPattern,
                    _firstStartAnchor,
                    _secondEndAnchor);
                _firstBoundary.ResetBoundary();
                return;
            }

            AlignPatternStartToEnd(
                _secondPattern,
                _secondStartAnchor,
                _firstEndAnchor);
            _secondBoundary.ResetBoundary();
        }

        private void AlignPatternStartToEnd(
            Transform pattern,
            Transform patternStartAnchor,
            Transform frontEndAnchor)
        {
            Vector3 positionOffset =
                frontEndAnchor.position - patternStartAnchor.position;
            pattern.position += positionOffset;
            Physics.SyncTransforms();
        }

        private bool BindPatternCollectibles(int patternIndex)
        {
            Transform pattern;

            if (_usesPatternSlots)
            {
                if (!GetSlot(patternIndex).TryGetCurrentPattern(
                        out InfinitePatternAuthoring currentPattern))
                {
                    return false;
                }

                pattern = currentPattern.transform;
            }
            else
            {
                pattern = patternIndex == FirstPatternIndex
                    ? _firstPattern
                    : _secondPattern;
            }
            List<ScoreCollectible> collectibles = patternIndex == FirstPatternIndex
                ? _firstCollectibles
                : _secondCollectibles;

            if (!_runtimeData.CollectibleRuntimeData.TryCreateScope(
                    out long scopeId))
            {
                return false;
            }

            pattern.GetComponentsInChildren(false, collectibles);

            for (int i = 0; i < collectibles.Count; i++)
            {
                if (!collectibles[i].Bind(
                        _runtimeData,
                        _playerRigidbody,
                        scopeId))
                {
                    ReleaseCollectibles(collectibles, scopeId);
                    return false;
                }
            }

            SetScopeId(patternIndex, scopeId);
            return true;
        }

        private void ReleasePatternCollectibles(int patternIndex)
        {
            List<ScoreCollectible> collectibles = patternIndex == FirstPatternIndex
                ? _firstCollectibles
                : _secondCollectibles;
            long scopeId = patternIndex == FirstPatternIndex
                ? _firstCollectibleScopeId
                : _secondCollectibleScopeId;
            ReleaseCollectibles(collectibles, scopeId);
            SetScopeId(patternIndex, 0);
        }

        private void ReleaseCollectibles(
            List<ScoreCollectible> collectibles,
            long scopeId)
        {
            for (int i = 0; i < collectibles.Count; i++)
            {
                if (collectibles[i] != null)
                {
                    collectibles[i].Unbind();
                }
            }

            collectibles.Clear();

            if (scopeId != 0 && _runtimeData != null &&
                _runtimeData.CollectibleRuntimeData != null)
            {
                _runtimeData.CollectibleRuntimeData.TryReleaseScope(scopeId);
            }
        }

        private void SetScopeId(int patternIndex, long scopeId)
        {
            if (patternIndex == FirstPatternIndex)
            {
                _firstCollectibleScopeId = scopeId;
                return;
            }

            _secondCollectibleScopeId = scopeId;
        }

        private void RecheckCollectibles(List<ScoreCollectible> collectibles)
        {
            for (int i = 0; i < collectibles.Count; i++)
            {
                if (collectibles[i] != null)
                {
                    collectibles[i].TryCollectOverlappingPlayer();
                }
            }
        }

        private static bool IsValidWorldXOffset(float worldXOffset)
        {
            return !float.IsNaN(worldXOffset) &&
                   !float.IsInfinity(worldXOffset) &&
                   worldXOffset < 0.0f;
        }
    }
}
