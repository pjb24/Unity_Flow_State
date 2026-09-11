using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class InfinitePatternDefinition
    {
        public const float PatternLength = 44.0f;
        public const float GroundGap = 4.0f;
        public const float GroundWidth = 4.0f;
        public const float PositionTolerance = 0.01f;
        public const float TangentToleranceDegrees = 0.1f;

        private string _id;
        private string _purpose;
        private E_InfinitePatternDifficulty _minimumDifficulty;
        private Vector3 _startAnchor;
        private Vector3 _endAnchor;
        private Vector3 _startTangent;
        private Vector3 _endTangent;
        private float _startGroundInset;
        private float _endGroundInset;
        private float _startGroundTopY;
        private float _endGroundTopY;
        private float _startGroundWidth;
        private float _endGroundWidth;
        private bool _isInitialized;

        public string Id => _id;

        public string Purpose => _purpose;

        public E_InfinitePatternDifficulty MinimumDifficulty =>
            _minimumDifficulty;

        public bool IsInitialized => _isInitialized;

        public bool Initialize(
            string id,
            string purpose,
            E_InfinitePatternDifficulty minimumDifficulty,
            Vector3 startAnchor,
            Vector3 endAnchor,
            Vector3 startTangent,
            Vector3 endTangent,
            float startGroundInset,
            float endGroundInset,
            float startGroundTopY,
            float endGroundTopY,
            float startGroundWidth,
            float endGroundWidth)
        {
            _isInitialized = false;

            if (string.IsNullOrWhiteSpace(id) ||
                string.IsNullOrWhiteSpace(purpose) ||
                !IsValidDifficulty(minimumDifficulty) ||
                !IsFinite(startAnchor) ||
                !IsFinite(endAnchor) ||
                !IsFinite(startTangent) ||
                !IsFinite(endTangent) ||
                !IsFinite(startGroundInset) ||
                !IsFinite(endGroundInset) ||
                !IsFinite(startGroundTopY) ||
                !IsFinite(endGroundTopY) ||
                !IsFinite(startGroundWidth) ||
                !IsFinite(endGroundWidth) ||
                startGroundInset < 0.0f ||
                endGroundInset < 0.0f)
            {
                return false;
            }

            Vector3 anchorDelta = endAnchor - startAnchor;

            if (Mathf.Abs(anchorDelta.x - PatternLength) > PositionTolerance ||
                Mathf.Abs(anchorDelta.y) > PositionTolerance ||
                Mathf.Abs(anchorDelta.z) > PositionTolerance ||
                !IsForwardTangent(startTangent) ||
                !IsForwardTangent(endTangent) ||
                Mathf.Abs(startGroundWidth - GroundWidth) > PositionTolerance ||
                Mathf.Abs(endGroundWidth - GroundWidth) > PositionTolerance ||
                startGroundInset + endGroundInset >= PatternLength)
            {
                return false;
            }

            _id = id;
            _purpose = purpose;
            _minimumDifficulty = minimumDifficulty;
            _startAnchor = startAnchor;
            _endAnchor = endAnchor;
            _startTangent = startTangent.normalized;
            _endTangent = endTangent.normalized;
            _startGroundInset = startGroundInset;
            _endGroundInset = endGroundInset;
            _startGroundTopY = startGroundTopY;
            _endGroundTopY = endGroundTopY;
            _startGroundWidth = startGroundWidth;
            _endGroundWidth = endGroundWidth;
            _isInitialized = true;
            return true;
        }

        public bool CanConnectTo(InfinitePatternDefinition nextPattern)
        {
            if (!_isInitialized ||
                nextPattern == null ||
                !nextPattern._isInitialized)
            {
                return false;
            }

            float connectedGroundGap =
                _endGroundInset + nextPattern._startGroundInset;

            return Vector3.Angle(
                       _endTangent,
                       nextPattern._startTangent) <= TangentToleranceDegrees &&
                   Mathf.Abs(
                       _endGroundTopY -
                       nextPattern._startGroundTopY) <= PositionTolerance &&
                   Mathf.Abs(
                       _endGroundWidth -
                       nextPattern._startGroundWidth) <= PositionTolerance &&
                   Mathf.Abs(connectedGroundGap - GroundGap) <= PositionTolerance;
        }

        private static bool IsValidDifficulty(
            E_InfinitePatternDifficulty difficulty)
        {
            return difficulty == E_InfinitePatternDifficulty.D1 ||
                   difficulty == E_InfinitePatternDifficulty.D2 ||
                   difficulty == E_InfinitePatternDifficulty.D3;
        }

        private static bool IsForwardTangent(Vector3 tangent)
        {
            return tangent.sqrMagnitude > Mathf.Epsilon &&
                   Vector3.Angle(tangent, Vector3.right) <=
                   TangentToleranceDegrees;
        }

        private static bool IsFinite(Vector3 value)
        {
            return IsFinite(value.x) &&
                   IsFinite(value.y) &&
                   IsFinite(value.z);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
