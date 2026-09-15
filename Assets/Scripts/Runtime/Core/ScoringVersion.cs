namespace FlowState.Runtime.Core
{
    public static class ScoringVersion
    {
        public const int None = 0;

        public const int LegacyDistanceScore = 1;

        public const int Current = 2;

        public static bool IsSupported(int version)
        {
            return version == LegacyDistanceScore || version == Current;
        }
    }
}
