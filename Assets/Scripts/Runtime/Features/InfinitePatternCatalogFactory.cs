using UnityEngine;

namespace FlowState.Runtime.Features
{
    public static class InfinitePatternCatalogFactory
    {
        public const string FlatId = "Flat";
        public const string SingleRiseId = "SingleRise";
        public const string LegacyStepsId = "LegacySteps";
        public const string InternalGapId = "InternalGap";

        private const float StartAnchorX = -22.0f;
        private const float EndAnchorX = 22.0f;
        private const float GroundInset = 2.0f;
        private const float GroundTopY = 0.5f;

        public static bool TryCreate(out InfinitePatternCatalog catalog)
        {
            catalog = new InfinitePatternCatalog();
            InfinitePatternDefinition[] definitions =
            {
                CreateDefinition(
                    FlatId,
                    "Provides the run start, automatic acceleration, and fallback.",
                    E_InfinitePatternDifficulty.D1),
                CreateDefinition(
                    SingleRiseId,
                    "Provides one rising jump and a wide landing surface.",
                    E_InfinitePatternDifficulty.D1),
                CreateDefinition(
                    LegacyStepsId,
                    "Preserves the existing two-platform jump and landing flow.",
                    E_InfinitePatternDifficulty.D2),
                CreateDefinition(
                    InternalGapId,
                    "Tests jump timing and horizontal travel across an internal gap.",
                    E_InfinitePatternDifficulty.D3)
            };

            return catalog.Initialize(definitions);
        }

        private static InfinitePatternDefinition CreateDefinition(
            string id,
            string purpose,
            E_InfinitePatternDifficulty minimumDifficulty)
        {
            InfinitePatternDefinition definition =
                new InfinitePatternDefinition();
            definition.Initialize(
                id,
                purpose,
                minimumDifficulty,
                new Vector3(StartAnchorX, 0.0f, 0.0f),
                new Vector3(EndAnchorX, 0.0f, 0.0f),
                Vector3.right,
                Vector3.right,
                GroundInset,
                GroundInset,
                GroundTopY,
                GroundTopY,
                InfinitePatternDefinition.GroundWidth,
                InfinitePatternDefinition.GroundWidth);
            return definition;
        }
    }
}
