using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternCatalogTests
    {
        [Test]
        public void Initialize_ValidDefinitions_StoresUniquePatterns()
        {
            InfinitePatternCatalog catalog = new InfinitePatternCatalog();

            bool didInitialize = catalog.Initialize(new[]
            {
                CreateDefinition("Flat"),
                CreateDefinition("SingleRise")
            });

            Assert.That(didInitialize, Is.True);
            Assert.That(catalog.Count, Is.EqualTo(2));
            Assert.That(catalog.Contains("Flat"), Is.True);
            Assert.That(catalog.Contains("SingleRise"), Is.True);
        }

        [Test]
        public void Initialize_EmptyDefinitions_IsRejected()
        {
            InfinitePatternCatalog catalog = new InfinitePatternCatalog();

            Assert.That(
                catalog.Initialize(new InfinitePatternDefinition[0]),
                Is.False);
            Assert.That(catalog.Count, Is.Zero);
        }

        [Test]
        public void Initialize_DuplicateId_IsRejectedWithoutPartialCatalog()
        {
            InfinitePatternCatalog catalog = new InfinitePatternCatalog();

            bool didInitialize = catalog.Initialize(new[]
            {
                CreateDefinition("Flat"),
                CreateDefinition("Flat")
            });

            Assert.That(didInitialize, Is.False);
            Assert.That(catalog.Count, Is.Zero);
        }

        [Test]
        public void Initialize_UninitializedDefinition_IsRejected()
        {
            InfinitePatternCatalog catalog = new InfinitePatternCatalog();

            bool didInitialize = catalog.Initialize(new[]
            {
                CreateDefinition("Flat"),
                new InfinitePatternDefinition()
            });

            Assert.That(didInitialize, Is.False);
            Assert.That(catalog.Count, Is.Zero);
        }

        [Test]
        public void CanConnect_KnownCompatiblePatterns_ReturnsTrue()
        {
            InfinitePatternCatalog catalog = CreateCatalog();

            Assert.That(catalog.CanConnect("Flat", "SingleRise"), Is.True);
        }

        [Test]
        public void CanConnect_UnknownPattern_ReturnsFalse()
        {
            InfinitePatternCatalog catalog = CreateCatalog();

            Assert.That(catalog.CanConnect("Missing", "Flat"), Is.False);
            Assert.That(catalog.CanConnect("Flat", "Missing"), Is.False);
        }

        [Test]
        public void TryGetAt_UsesDefinitionOrderAndRejectsInvalidIndex()
        {
            InfinitePatternCatalog catalog = CreateCatalog();

            Assert.That(catalog.TryGetAt(
                0,
                out InfinitePatternDefinition first), Is.True);
            Assert.That(first.Id, Is.EqualTo("Flat"));
            Assert.That(catalog.TryGetAt(
                1,
                out InfinitePatternDefinition second), Is.True);
            Assert.That(second.Id, Is.EqualTo("SingleRise"));
            Assert.That(catalog.TryGetAt(-1, out _), Is.False);
            Assert.That(catalog.TryGetAt(2, out _), Is.False);
        }

        private static InfinitePatternCatalog CreateCatalog()
        {
            InfinitePatternCatalog catalog = new InfinitePatternCatalog();
            catalog.Initialize(new[]
            {
                CreateDefinition("Flat"),
                CreateDefinition("SingleRise")
            });
            return catalog;
        }

        private static InfinitePatternDefinition CreateDefinition(string id)
        {
            InfinitePatternDefinition definition =
                new InfinitePatternDefinition();
            definition.Initialize(
                id,
                "Test purpose",
                E_InfinitePatternDifficulty.D1,
                new Vector3(-22.0f, 0.0f, 0.0f),
                new Vector3(22.0f, 0.0f, 0.0f),
                Vector3.right,
                Vector3.right,
                2.0f,
                2.0f,
                0.5f,
                0.5f,
                4.0f,
                4.0f);
            return definition;
        }
    }
}
