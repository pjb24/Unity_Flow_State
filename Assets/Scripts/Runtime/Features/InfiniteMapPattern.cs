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

        public bool IsInitialized => _isInitialized;

        public int AdvanceCount => _advanceCount;

        private void OnEnable()
        {
            if (_hasInitialTransforms)
            {
                Initialize();
            }
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
            return true;
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

            MoveTrailingPatternAfterFront();
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
    }
}
