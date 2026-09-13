using System.Collections.Generic;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class InfinitePatternSlot : MonoBehaviour
    {
        private const int FirstSlotId = 0;
        private const int SecondSlotId = 1;

        [SerializeField] private int _slotId;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private InfinitePatternBoundary _advanceBoundary;

        private readonly List<InfinitePatternAuthoring> _patternInstances =
            new List<InfinitePatternAuthoring>();
        private string _initialPatternId;
        private string _currentPatternId;
        private bool _isInitialized;
        private bool _ownsInstances;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        public int SlotId => _slotId;

        public Transform ContentRoot => _contentRoot;

        public InfinitePatternBoundary AdvanceBoundary => _advanceBoundary;

        public string CurrentPatternId => _currentPatternId;

        public int PatternInstanceCount => _patternInstances.Count;

        public bool IsInitialized => _isInitialized;

        public bool Initialize(
            InfinitePatternCatalog catalog,
            IReadOnlyList<InfinitePatternAuthoring> patternInstances,
            string initialPatternId)
        {
            if (_isInitialized ||
                !HasValidSerializedReferences() ||
                !HasValidPatternInstances(
                    catalog,
                    patternInstances,
                    initialPatternId))
            {
                return false;
            }

            _patternInstances.Clear();

            for (int i = 0; i < patternInstances.Count; i++)
            {
                _patternInstances.Add(patternInstances[i]);
            }

            _initialPatternId = initialPatternId;
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _isInitialized = true;
            ApplyActivePattern(_initialPatternId);
            AlignBoundaryToCurrentPattern();
            return true;
        }

        public bool InitializeFromPrefabs(
            InfinitePatternCatalog catalog,
            IReadOnlyList<InfinitePatternAuthoring> prefabs,
            string initialPatternId)
        {
            if (_isInitialized ||
                !HasValidSerializedReferences() ||
                !HasValidPrefabs(catalog, prefabs, initialPatternId))
            {
                return false;
            }

            InfinitePatternAuthoring[] instances =
                new InfinitePatternAuthoring[prefabs.Count];

            for (int i = 0; i < prefabs.Count; i++)
            {
                instances[i] = Instantiate(prefabs[i], _contentRoot);

                if (instances[i] == null)
                {
                    DestroyInstances(instances);
                    return false;
                }

                instances[i].gameObject.SetActive(false);
            }

            if (!Initialize(catalog, instances, initialPatternId))
            {
                DestroyInstances(instances);
                return false;
            }

            _ownsInstances = true;
            return true;
        }

        public bool ResetToInitialPattern()
        {
            if (!_isInitialized)
            {
                return false;
            }

            ApplyActivePattern(_initialPatternId);
            transform.SetPositionAndRotation(
                _initialPosition,
                _initialRotation);
            AlignBoundaryToCurrentPattern();
            _advanceBoundary.ResetBoundary();
            return true;
        }

        public bool TryActivatePattern(string patternId)
        {
            if (!TryGetPatternInstance(patternId, out _))
            {
                return false;
            }

            ApplyActivePattern(patternId);
            AlignBoundaryToCurrentPattern();
            _advanceBoundary.ResetBoundary();
            return true;
        }

        public bool TryAlignStartTo(Vector3 worldEndPosition)
        {
            if (!TryGetPatternInstance(
                    _currentPatternId,
                    out InfinitePatternAuthoring current))
            {
                return false;
            }

            transform.position +=
                worldEndPosition - current.StartAnchor.position;
            AlignBoundaryToCurrentPattern();
            Physics.SyncTransforms();
            return true;
        }

        public bool TryGetCurrentPattern(
            out InfinitePatternAuthoring pattern)
        {
            return TryGetPatternInstance(_currentPatternId, out pattern);
        }

        public void Clear()
        {
            if (_ownsInstances)
            {
                for (int i = 0; i < _patternInstances.Count; i++)
                {
                    InfinitePatternAuthoring instance =
                        _patternInstances[i];

                    if (instance != null)
                    {
                        instance.gameObject.SetActive(false);
                        DestroyInstance(instance);
                    }
                }
            }

            _patternInstances.Clear();
            _initialPatternId = null;
            _currentPatternId = null;
            _isInitialized = false;
            _ownsInstances = false;
        }

        public bool CanPairWith(InfinitePatternSlot other)
        {
            if (!_isInitialized ||
                other == null ||
                !other._isInitialized ||
                other == this ||
                other._slotId == _slotId)
            {
                return false;
            }

            for (int i = 0; i < _patternInstances.Count; i++)
            {
                for (int otherIndex = 0;
                     otherIndex < other._patternInstances.Count;
                     otherIndex++)
                {
                    if (_patternInstances[i] ==
                        other._patternInstances[otherIndex])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool TryGetPatternInstance(
            string patternId,
            out InfinitePatternAuthoring patternInstance)
        {
            patternInstance = null;

            if (!_isInitialized || string.IsNullOrWhiteSpace(patternId))
            {
                return false;
            }

            for (int i = 0; i < _patternInstances.Count; i++)
            {
                if (_patternInstances[i].PatternId == patternId)
                {
                    patternInstance = _patternInstances[i];
                    return true;
                }
            }

            return false;
        }

        private bool HasValidSerializedReferences()
        {
            return (_slotId == FirstSlotId || _slotId == SecondSlotId) &&
                   _contentRoot != null &&
                   _contentRoot != transform &&
                   _contentRoot.IsChildOf(transform) &&
                   _advanceBoundary != null &&
                   _advanceBoundary.transform.IsChildOf(transform) &&
                   !_advanceBoundary.transform.IsChildOf(_contentRoot) &&
                   _advanceBoundary.BoundaryId == _slotId &&
                   HasValidBoundaryCollider();
        }

        private bool HasValidBoundaryCollider()
        {
            Collider[] colliders =
                _advanceBoundary.GetComponents<Collider>();

            return colliders.Length == 1 &&
                   colliders[0].enabled &&
                   colliders[0].isTrigger;
        }

        private bool HasValidPatternInstances(
            InfinitePatternCatalog catalog,
            IReadOnlyList<InfinitePatternAuthoring> patternInstances,
            string initialPatternId)
        {
            if (catalog == null ||
                patternInstances == null ||
                patternInstances.Count != catalog.Count ||
                string.IsNullOrWhiteSpace(initialPatternId) ||
                !catalog.Contains(initialPatternId))
            {
                return false;
            }

            bool hasInitialPattern = false;

            for (int i = 0; i < patternInstances.Count; i++)
            {
                InfinitePatternAuthoring patternInstance = patternInstances[i];

                if (patternInstance == null ||
                    !patternInstance.transform.IsChildOf(_contentRoot) ||
                    !patternInstance.IsValid(catalog))
                {
                    return false;
                }

                if (patternInstance.PatternId == initialPatternId)
                {
                    hasInitialPattern = true;
                }

                for (int previousIndex = 0;
                     previousIndex < i;
                     previousIndex++)
                {
                    if (patternInstances[previousIndex] == patternInstance ||
                        patternInstances[previousIndex].PatternId ==
                        patternInstance.PatternId)
                    {
                        return false;
                    }
                }
            }

            return hasInitialPattern;
        }

        private bool HasValidPrefabs(
            InfinitePatternCatalog catalog,
            IReadOnlyList<InfinitePatternAuthoring> prefabs,
            string initialPatternId)
        {
            if (catalog == null || prefabs == null ||
                prefabs.Count != catalog.Count ||
                !catalog.Contains(initialPatternId))
            {
                return false;
            }

            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i] == null || !prefabs[i].IsValid(catalog))
                {
                    return false;
                }

                for (int previousIndex = 0;
                     previousIndex < i; previousIndex++)
                {
                    if (prefabs[previousIndex].PatternId ==
                        prefabs[i].PatternId)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void AlignBoundaryToCurrentPattern()
        {
            TryGetCurrentPattern(out InfinitePatternAuthoring current);
            _advanceBoundary.transform.SetPositionAndRotation(
                current.AdvanceBoundaryPoint.position,
                current.AdvanceBoundaryPoint.rotation);
        }

        private void DestroyInstances(InfinitePatternAuthoring[] instances)
        {
            for (int i = 0; i < instances.Length; i++)
            {
                if (instances[i] != null)
                {
                    DestroyInstance(instances[i]);
                }
            }
        }

        private void DestroyInstance(InfinitePatternAuthoring instance)
        {
            if (Application.isPlaying)
            {
                Destroy(instance.gameObject);
            }
            else
            {
                DestroyImmediate(instance.gameObject);
            }
        }

        private void ApplyActivePattern(string patternId)
        {
            for (int i = 0; i < _patternInstances.Count; i++)
            {
                InfinitePatternAuthoring patternInstance = _patternInstances[i];
                bool isCurrent = patternInstance.PatternId == patternId;
                patternInstance.gameObject.SetActive(isCurrent);
            }

            _currentPatternId = patternId;
        }
    }
}
