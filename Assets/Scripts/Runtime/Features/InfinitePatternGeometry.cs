using UnityEngine;

namespace FlowState.Runtime.Features
{
    public static class InfinitePatternGeometry
    {
        public const float SurfaceTopBase = 0.5f;
        public const float SurfaceTopRaised = 1.5f;
        public const float SurfaceDepth = 1.0f;
        public const float SurfaceWidth = 4.0f;

        public static bool TryGetSurfaceCount(string patternId, out int count)
        {
            count = 0;

            if (patternId == InfinitePatternCatalogFactory.FlatId)
            {
                count = 1;
            }
            else if (patternId == InfinitePatternCatalogFactory.SingleRiseId ||
                     patternId == InfinitePatternCatalogFactory.InternalGapId)
            {
                count = patternId == InfinitePatternCatalogFactory.SingleRiseId
                    ? 3
                    : 2;
            }
            else if (patternId == InfinitePatternCatalogFactory.LegacyStepsId)
            {
                count = 4;
            }

            return count > 0;
        }

        public static bool TryGetSurface(
            string patternId,
            int index,
            out Vector3 localPosition,
            out Vector3 colliderSize)
        {
            localPosition = Vector3.zero;
            colliderSize = Vector3.zero;

            float startX;
            float endX;
            float topY = SurfaceTopBase;

            if (patternId == InfinitePatternCatalogFactory.FlatId)
            {
                if (index != 0) return false;
                startX = -20.0f;
                endX = 20.0f;
            }
            else if (patternId == InfinitePatternCatalogFactory.SingleRiseId)
            {
                switch (index)
                {
                    case 0: startX = -20.0f; endX = -8.0f; break;
                    case 1: startX = -6.0f; endX = 8.0f;
                        topY = SurfaceTopRaised; break;
                    case 2: startX = 10.0f; endX = 20.0f; break;
                    default: return false;
                }
            }
            else if (patternId == InfinitePatternCatalogFactory.LegacyStepsId)
            {
                switch (index)
                {
                    case 0: startX = -20.0f; endX = -12.0f; break;
                    case 1: startX = -10.0f; endX = -2.0f;
                        topY = SurfaceTopRaised; break;
                    case 2: startX = 0.0f; endX = 8.0f;
                        topY = SurfaceTopRaised; break;
                    case 3: startX = 10.0f; endX = 20.0f; break;
                    default: return false;
                }
            }
            else if (patternId == InfinitePatternCatalogFactory.InternalGapId)
            {
                switch (index)
                {
                    case 0: startX = -20.0f; endX = -3.0f; break;
                    case 1: startX = 3.0f; endX = 20.0f; break;
                    default: return false;
                }
            }
            else
            {
                return false;
            }

            localPosition = new Vector3(
                (startX + endX) * 0.5f,
                topY - SurfaceDepth * 0.5f,
                0.0f);
            colliderSize = new Vector3(
                endX - startX,
                SurfaceDepth,
                SurfaceWidth);
            return true;
        }

        public static bool TryGetAdvanceBoundaryPoint(
            string patternId,
            out Vector3 localPosition)
        {
            localPosition = Vector3.zero;

            if (patternId == InfinitePatternCatalogFactory.FlatId)
            {
                localPosition = new Vector3(-4.0f, 5.5f, 0.0f);
            }
            else if (patternId == InfinitePatternCatalogFactory.SingleRiseId)
            {
                localPosition = new Vector3(0.0f, 5.5f, 0.0f);
            }
            else if (patternId == InfinitePatternCatalogFactory.LegacyStepsId)
            {
                localPosition = new Vector3(2.0f, 5.5f, 0.0f);
            }
            else if (patternId == InfinitePatternCatalogFactory.InternalGapId)
            {
                localPosition = new Vector3(4.0f, 5.5f, 0.0f);
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool CanTraverseInternalSurfaces(string patternId)
        {
            if (!TryGetSurfaceCount(patternId, out int count))
            {
                return false;
            }

            for (int index = 0; index < count - 1; index++)
            {
                if (!TryGetSurface(patternId, index,
                        out Vector3 fromPosition, out Vector3 fromSize) ||
                    !TryGetSurface(patternId, index + 1,
                        out Vector3 toPosition, out Vector3 toSize))
                {
                    return false;
                }

                float fromEnd = fromPosition.x + fromSize.x * 0.5f;
                float toStart = toPosition.x - toSize.x * 0.5f;
                float gap = toStart - fromEnd;
                float fromTop = fromPosition.y + fromSize.y * 0.5f;
                float toTop = toPosition.y + toSize.y * 0.5f;
                float heightDifference = toTop - fromTop;

                if (!InfinitePatternTraversalMath.CanTraverseJump(
                        gap, heightDifference, fromSize.x, toSize.x))
                {
                    return false;
                }

                if (heightDifference > 0.0f &&
                    (!InfinitePatternTraversalMath.CanClearObstacle(
                         2.0f, gap, heightDifference,
                         InfinitePatternTraversalMath.BaseHorizontalSpeed) ||
                     !InfinitePatternTraversalMath.CanClearObstacle(
                         2.0f, gap, heightDifference,
                         InfinitePatternTraversalMath.MaximumHorizontalSpeed)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
