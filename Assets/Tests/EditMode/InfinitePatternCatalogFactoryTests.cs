using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternCatalogFactoryTests
    {
        private static readonly string[] PatternIds =
        {
            InfinitePatternCatalogFactory.FlatId,
            InfinitePatternCatalogFactory.SingleRiseId,
            InfinitePatternCatalogFactory.LegacyStepsId,
            InfinitePatternCatalogFactory.InternalGapId
        };

        [Test]
        public void TryCreate_ConfirmedPatternList_CreatesFourUniquePatterns()
        {
            bool didCreate = InfinitePatternCatalogFactory.TryCreate(
                out InfinitePatternCatalog catalog);

            Assert.That(didCreate, Is.True);
            Assert.That(catalog.Count, Is.EqualTo(PatternIds.Length));

            for (int i = 0; i < PatternIds.Length; i++)
            {
                Assert.That(catalog.Contains(PatternIds[i]), Is.True);
            }
        }

        [TestCase(
            InfinitePatternCatalogFactory.FlatId,
            E_InfinitePatternDifficulty.D1)]
        [TestCase(
            InfinitePatternCatalogFactory.SingleRiseId,
            E_InfinitePatternDifficulty.D1)]
        [TestCase(
            InfinitePatternCatalogFactory.LegacyStepsId,
            E_InfinitePatternDifficulty.D2)]
        [TestCase(
            InfinitePatternCatalogFactory.InternalGapId,
            E_InfinitePatternDifficulty.D3)]
        public void TryCreate_Pattern_HasConfirmedDifficultyAndPurpose(
            string patternId,
            E_InfinitePatternDifficulty expectedDifficulty)
        {
            InfinitePatternCatalogFactory.TryCreate(
                out InfinitePatternCatalog catalog);

            bool didGet = catalog.TryGet(
                patternId,
                out InfinitePatternDefinition definition);

            Assert.That(didGet, Is.True);
            Assert.That(
                definition.MinimumDifficulty,
                Is.EqualTo(expectedDifficulty));
            Assert.That(definition.Purpose, Is.Not.Empty);
        }

        [Test]
        public void TryCreate_AllPatternPairs_SatisfyCommonConnectionContract()
        {
            InfinitePatternCatalogFactory.TryCreate(
                out InfinitePatternCatalog catalog);

            for (int previousIndex = 0;
                 previousIndex < PatternIds.Length;
                 previousIndex++)
            {
                for (int nextIndex = 0;
                     nextIndex < PatternIds.Length;
                     nextIndex++)
                {
                    Assert.That(
                        catalog.CanConnect(
                            PatternIds[previousIndex],
                            PatternIds[nextIndex]),
                        Is.True,
                        PatternIds[previousIndex] +
                        " -> " +
                        PatternIds[nextIndex]);
                }
            }
        }
    }
}
