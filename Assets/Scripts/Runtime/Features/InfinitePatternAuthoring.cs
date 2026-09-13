using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class InfinitePatternAuthoring : MonoBehaviour
    {
        private const string GroundLayerName = "Ground";

        [SerializeField] private string _patternId;
        [SerializeField] private E_InfinitePatternDifficulty _minimumDifficulty;
        [SerializeField] private Transform _geometryRoot;
        [SerializeField] private Transform _startAnchor;
        [SerializeField] private Transform _endAnchor;
        [SerializeField] private Transform _advanceBoundaryPoint;
        [SerializeField] private Transform _collectibleRoot;
        [SerializeField] private Collider[] _terrainColliders;

        public string PatternId => _patternId;

        public E_InfinitePatternDifficulty MinimumDifficulty =>
            _minimumDifficulty;

        public Transform GeometryRoot => _geometryRoot;

        public Transform StartAnchor => _startAnchor;

        public Transform EndAnchor => _endAnchor;

        public Transform AdvanceBoundaryPoint => _advanceBoundaryPoint;

        public Transform CollectibleRoot => _collectibleRoot;

        public int TerrainColliderCount => _terrainColliders == null
            ? 0
            : _terrainColliders.Length;

        public bool IsValid(InfinitePatternCatalog catalog)
        {
            if (catalog == null ||
                string.IsNullOrWhiteSpace(_patternId) ||
                !catalog.TryGet(
                    _patternId,
                    out InfinitePatternDefinition definition) ||
                definition.MinimumDifficulty != _minimumDifficulty ||
                !HasValidHierarchy() ||
                !HasValidTransforms() ||
                !HasValidTerrainColliders())
            {
                return false;
            }

            return true;
        }

        public bool TryGetTerrainCollider(int index, out Collider collider)
        {
            collider = null;

            if (_terrainColliders == null ||
                index < 0 ||
                index >= _terrainColliders.Length)
            {
                return false;
            }

            collider = _terrainColliders[index];
            return collider != null;
        }

        private bool HasValidHierarchy()
        {
            Transform patternRoot = transform;

            return IsDescendant(_geometryRoot, patternRoot) &&
                   IsDescendant(_startAnchor, patternRoot) &&
                   IsDescendant(_endAnchor, patternRoot) &&
                   IsDescendant(_advanceBoundaryPoint, patternRoot) &&
                   IsDescendant(_collectibleRoot, patternRoot) &&
                   _geometryRoot != _startAnchor &&
                   _geometryRoot != _endAnchor &&
                   _geometryRoot != _advanceBoundaryPoint &&
                   _geometryRoot != _collectibleRoot &&
                   _startAnchor != _endAnchor &&
                   _startAnchor != _advanceBoundaryPoint &&
                   _endAnchor != _advanceBoundaryPoint;
        }

        private bool HasValidTransforms()
        {
            return IsFinite(_startAnchor.localPosition) &&
                   IsFinite(_endAnchor.localPosition) &&
                   IsFinite(_advanceBoundaryPoint.localPosition) &&
                   IsFinite(_startAnchor.localRotation) &&
                   IsFinite(_endAnchor.localRotation) &&
                   IsFinite(_advanceBoundaryPoint.localRotation);
        }

        private bool HasValidTerrainColliders()
        {
            if (_terrainColliders == null || _terrainColliders.Length == 0)
            {
                return false;
            }

            int groundLayer = LayerMask.NameToLayer(GroundLayerName);

            if (groundLayer < 0)
            {
                return false;
            }

            for (int i = 0; i < _terrainColliders.Length; i++)
            {
                Collider collider = _terrainColliders[i];

                if (collider == null ||
                    !collider.transform.IsChildOf(_geometryRoot) ||
                    collider.isTrigger ||
                    collider.gameObject.layer != groundLayer)
                {
                    return false;
                }

                for (int previousIndex = 0;
                     previousIndex < i;
                     previousIndex++)
                {
                    if (_terrainColliders[previousIndex] == collider)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsDescendant(
            Transform candidate,
            Transform expectedParent)
        {
            return candidate != null &&
                   candidate != expectedParent &&
                   candidate.IsChildOf(expectedParent);
        }

        private static bool IsFinite(Vector3 value)
        {
            return IsFinite(value.x) &&
                   IsFinite(value.y) &&
                   IsFinite(value.z);
        }

        private static bool IsFinite(Quaternion value)
        {
            return IsFinite(value.x) &&
                   IsFinite(value.y) &&
                   IsFinite(value.z) &&
                   IsFinite(value.w);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
