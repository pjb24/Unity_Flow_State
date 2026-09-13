using System.Collections.Generic;
using System.Reflection;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternAuthoringTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private InfinitePatternCatalog _catalog;

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                InfinitePatternCatalogFactory.TryCreate(out _catalog),
                Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(_objects[i]);
            }

            _objects.Clear();
        }

        [Test]
        public void IsValid_CompleteAuthoring_AcceptsContract()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.FlatId,
                E_InfinitePatternDifficulty.D1);

            Assert.That(authoring.IsValid(_catalog), Is.True);
            Assert.That(authoring.GeometryRoot, Is.Not.Null);
            Assert.That(authoring.StartAnchor, Is.Not.Null);
            Assert.That(authoring.EndAnchor, Is.Not.Null);
            Assert.That(authoring.AdvanceBoundaryPoint, Is.Not.Null);
            Assert.That(authoring.CollectibleRoot, Is.Not.Null);
            Assert.That(authoring.TerrainColliderCount, Is.EqualTo(1));
            Assert.That(
                authoring.TryGetTerrainCollider(0, out Collider collider),
                Is.True);
            Assert.That(collider.isTrigger, Is.False);
            Assert.That(collider.gameObject.layer, Is.EqualTo(6));
        }

        [TestCase("_geometryRoot")]
        [TestCase("_startAnchor")]
        [TestCase("_endAnchor")]
        [TestCase("_advanceBoundaryPoint")]
        [TestCase("_collectibleRoot")]
        public void IsValid_MissingTransform_IsRejected(string fieldName)
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.FlatId,
                E_InfinitePatternDifficulty.D1);
            SetPrivateField(authoring, fieldName, null);

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void IsValid_UnknownPatternId_IsRejected()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                "Unknown",
                E_InfinitePatternDifficulty.D1);

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void IsValid_DifficultyDoesNotMatchCatalog_IsRejected()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.LegacyStepsId,
                E_InfinitePatternDifficulty.D1);

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void IsValid_AnchorOutsidePatternRoot_IsRejected()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.FlatId,
                E_InfinitePatternDifficulty.D1);
            Transform externalAnchor = CreateObject(
                "ExternalEndAnchor",
                null).transform;
            SetPrivateField(authoring, "_endAnchor", externalAnchor);

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void IsValid_TriggerTerrainCollider_IsRejected()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.FlatId,
                E_InfinitePatternDifficulty.D1);
            authoring.TryGetTerrainCollider(0, out Collider collider);
            collider.isTrigger = true;

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void IsValid_NonGroundTerrainCollider_IsRejected()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.FlatId,
                E_InfinitePatternDifficulty.D1);
            authoring.TryGetTerrainCollider(0, out Collider collider);
            collider.gameObject.layer = 0;

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void IsValid_DuplicateTerrainCollider_IsRejected()
        {
            InfinitePatternAuthoring authoring = CreateAuthoring(
                InfinitePatternCatalogFactory.FlatId,
                E_InfinitePatternDifficulty.D1);
            authoring.TryGetTerrainCollider(0, out Collider collider);
            SetPrivateField(
                authoring,
                "_terrainColliders",
                new[] { collider, collider });

            Assert.That(authoring.IsValid(_catalog), Is.False);
        }

        [Test]
        public void Initialize_ValidSlot_ActivatesOnlyInitialPattern()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);

            bool didInitialize = slot.Initialize(
                _catalog,
                patterns,
                InfinitePatternCatalogFactory.FlatId);

            Assert.That(didInitialize, Is.True);
            Assert.That(slot.IsInitialized, Is.True);
            Assert.That(slot.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(slot.PatternInstanceCount, Is.EqualTo(4));
            Assert.That(patterns[0].gameObject.activeSelf, Is.True);

            for (int i = 1; i < patterns.Length; i++)
            {
                Assert.That(patterns[i].gameObject.activeSelf, Is.False);
            }
        }

        [Test]
        public void Initialize_DuplicatePatternId_IsRejectedWithoutPartialState()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);
            SetPrivateField(patterns[1], "_patternId", "Flat");
            bool[] initialStates = GetActiveStates(patterns);

            bool didInitialize = slot.Initialize(
                _catalog,
                patterns,
                InfinitePatternCatalogFactory.FlatId);

            Assert.That(didInitialize, Is.False);
            Assert.That(slot.IsInitialized, Is.False);
            Assert.That(slot.PatternInstanceCount, Is.Zero);
            Assert.That(slot.CurrentPatternId, Is.Null);
            Assert.That(GetActiveStates(patterns), Is.EqualTo(initialStates));
        }

        [Test]
        public void Initialize_MissingCatalogPattern_IsRejected()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);

            bool didInitialize = slot.Initialize(
                _catalog,
                new[] { patterns[0], patterns[1], patterns[2] },
                InfinitePatternCatalogFactory.FlatId);

            Assert.That(didInitialize, Is.False);
            Assert.That(slot.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_InvalidBoundaryId_CanRecoverAfterCorrection()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);
            SetPrivateField(slot.AdvanceBoundary, "_boundaryId", 1);

            Assert.That(
                slot.Initialize(_catalog, patterns, "Flat"),
                Is.False);
            Assert.That(slot.IsInitialized, Is.False);

            SetPrivateField(slot.AdvanceBoundary, "_boundaryId", 0);

            Assert.That(
                slot.Initialize(_catalog, patterns, "Flat"),
                Is.True);
            Assert.That(slot.CurrentPatternId, Is.EqualTo("Flat"));
        }

        [Test]
        public void Initialize_NonTriggerBoundaryCollider_IsRejected()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);
            slot.AdvanceBoundary.GetComponent<Collider>().isTrigger = false;

            Assert.That(
                slot.Initialize(_catalog, patterns, "Flat"),
                Is.False);
            Assert.That(slot.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_DuplicateRequest_PreservesInitialState()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);
            Assert.That(
                slot.Initialize(_catalog, patterns, "Flat"),
                Is.True);

            bool duplicateResult = slot.Initialize(
                _catalog,
                patterns,
                InfinitePatternCatalogFactory.SingleRiseId);

            Assert.That(duplicateResult, Is.False);
            Assert.That(slot.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(patterns[0].gameObject.activeSelf, Is.True);
            Assert.That(patterns[1].gameObject.activeSelf, Is.False);
        }

        [Test]
        public void ResetToInitialPattern_RestoresSameStartState()
        {
            InfinitePatternSlot slot = CreateSlot(
                0,
                out InfinitePatternAuthoring[] patterns);
            slot.Initialize(_catalog, patterns, "Flat");
            patterns[0].gameObject.SetActive(false);
            patterns[1].gameObject.SetActive(true);

            bool didReset = slot.ResetToInitialPattern();

            Assert.That(didReset, Is.True);
            Assert.That(slot.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(patterns[0].gameObject.activeSelf, Is.True);
            Assert.That(patterns[1].gameObject.activeSelf, Is.False);
        }

        [Test]
        public void CanPairWith_DifferentSlotsAndInstances_IsAccepted()
        {
            InfinitePatternSlot first = CreateSlot(0, out var firstPatterns);
            InfinitePatternSlot second = CreateSlot(1, out var secondPatterns);
            first.Initialize(_catalog, firstPatterns, "Flat");
            second.Initialize(_catalog, secondPatterns, "SingleRise");

            Assert.That(first.CanPairWith(second), Is.True);
            Assert.That(second.CanPairWith(first), Is.True);
        }

        [Test]
        public void CanPairWith_SharedPatternInstance_IsRejected()
        {
            InfinitePatternSlot first = CreateSlot(0, out var firstPatterns);
            InfinitePatternSlot second = CreateSlot(1, out var secondPatterns);
            first.Initialize(_catalog, firstPatterns, "Flat");
            Object.DestroyImmediate(secondPatterns[0].gameObject);
            firstPatterns[0].transform.SetParent(second.ContentRoot, false);
            secondPatterns[0] = firstPatterns[0];
            second.Initialize(_catalog, secondPatterns, "SingleRise");

            Assert.That(first.CanPairWith(second), Is.False);
        }

        private InfinitePatternSlot CreateSlot(
            int slotId,
            out InfinitePatternAuthoring[] patterns)
        {
            GameObject slotObject = CreateObject("Slot_" + slotId, null);
            Transform contentRoot = CreateObject(
                "ContentRoot",
                slotObject.transform).transform;
            GameObject boundaryObject = CreateObject(
                "AdvanceBoundary",
                slotObject.transform);
            InfinitePatternBoundary boundary =
                boundaryObject.AddComponent<InfinitePatternBoundary>();
            BoxCollider boundaryCollider =
                boundaryObject.AddComponent<BoxCollider>();
            boundaryCollider.isTrigger = true;
            SetPrivateField(boundary, "_boundaryId", slotId);

            InfinitePatternSlot slot =
                slotObject.AddComponent<InfinitePatternSlot>();
            SetPrivateField(slot, "_slotId", slotId);
            SetPrivateField(slot, "_contentRoot", contentRoot);
            SetPrivateField(slot, "_advanceBoundary", boundary);

            patterns = new[]
            {
                CreateAuthoring("Flat", E_InfinitePatternDifficulty.D1, contentRoot),
                CreateAuthoring("SingleRise", E_InfinitePatternDifficulty.D1, contentRoot),
                CreateAuthoring("LegacySteps", E_InfinitePatternDifficulty.D2, contentRoot),
                CreateAuthoring("InternalGap", E_InfinitePatternDifficulty.D3, contentRoot)
            };
            return slot;
        }

        private InfinitePatternAuthoring CreateAuthoring(
            string patternId,
            E_InfinitePatternDifficulty difficulty,
            Transform parent = null)
        {
            GameObject root = CreateObject(patternId, parent);
            Transform geometryRoot = CreateObject(
                "GeometryRoot",
                root.transform).transform;
            Transform startAnchor = CreateObject(
                "StartAnchor",
                root.transform).transform;
            Transform endAnchor = CreateObject(
                "EndAnchor",
                root.transform).transform;
            Transform boundaryPoint = CreateObject(
                "AdvanceBoundaryPoint",
                root.transform).transform;
            Transform collectibleRoot = CreateObject(
                "CollectibleRoot",
                root.transform).transform;
            GameObject ground = CreateObject("Ground", geometryRoot);
            ground.layer = 6;
            Collider groundCollider = ground.AddComponent<BoxCollider>();

            startAnchor.localPosition = new Vector3(-22.0f, 0.0f, 0.0f);
            endAnchor.localPosition = new Vector3(22.0f, 0.0f, 0.0f);

            InfinitePatternAuthoring authoring =
                root.AddComponent<InfinitePatternAuthoring>();
            SetPrivateField(authoring, "_patternId", patternId);
            SetPrivateField(authoring, "_minimumDifficulty", difficulty);
            SetPrivateField(authoring, "_geometryRoot", geometryRoot);
            SetPrivateField(authoring, "_startAnchor", startAnchor);
            SetPrivateField(authoring, "_endAnchor", endAnchor);
            SetPrivateField(
                authoring,
                "_advanceBoundaryPoint",
                boundaryPoint);
            SetPrivateField(authoring, "_collectibleRoot", collectibleRoot);
            SetPrivateField(
                authoring,
                "_terrainColliders",
                new[] { groundCollider });
            return authoring;
        }

        private GameObject CreateObject(string name, Transform parent)
        {
            GameObject created = new GameObject(name);
            created.transform.SetParent(parent, false);
            _objects.Add(created);
            return created;
        }

        private static bool[] GetActiveStates(
            InfinitePatternAuthoring[] patterns)
        {
            bool[] states = new bool[patterns.Length];

            for (int i = 0; i < patterns.Length; i++)
            {
                states[i] = patterns[i].gameObject.activeSelf;
            }

            return states;
        }

        private static void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, fieldName);
            field.SetValue(target, value);
        }
    }
}
