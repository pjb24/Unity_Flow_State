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

        public bool IsInitialized => _isInitialized;

        public int AdvanceCount => _advanceCount;

        private void OnEnable()
        {
            if (_hasInitialTransforms)
            {
                Initialize();
            }
        }

        private void OnDisable()
        {
            UnbindCollectibles();
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
            _isInitialized = false;

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

        public bool TryAdvance(int boundaryId)
        {
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
            Transform pattern = patternIndex == FirstPatternIndex
                ? _firstPattern
                : _secondPattern;
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
    }
}
