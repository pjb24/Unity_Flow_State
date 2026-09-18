using System.Collections.Generic;
using System.Reflection;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternSlotProgressionTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private InfiniteMapPattern _map;
        private InfinitePatternSlot _first;
        private InfinitePatternSlot _second;
        private InfinitePatternAuthoring[] _prefabs;
        private Collider _playerCollider;

        [SetUp]
        public void SetUp()
        {
            GameObject player = CreateObject("Player", null);
            _playerCollider = player.AddComponent<CapsuleCollider>();
            GameObject infiniteModeRoot = CreateObject("InfiniteModeRoot", null);
            GameObject mapObject = CreateObject("Map", infiniteModeRoot.transform);
            _map = mapObject.AddComponent<InfiniteMapPattern>();
            _first = CreateSlot(mapObject.transform, 0, Vector3.zero);
            _second = CreateSlot(
                mapObject.transform,
                1,
                new Vector3(44.0f, 0.0f, 0.0f));
            _prefabs = new[]
            {
                CreatePrefab("Flat", E_InfinitePatternDifficulty.D1),
                CreatePrefab("SingleRise", E_InfinitePatternDifficulty.D1),
                CreatePrefab("LegacySteps", E_InfinitePatternDifficulty.D2),
                CreatePrefab("InternalGap", E_InfinitePatternDifficulty.D3)
            };
            SetField(_map, "_firstSlot", _first);
            SetField(_map, "_secondSlot", _second);
            SetField(_map, "_patternPrefabs", _prefabs);
            SetField(_map, "_phase2PlayerCollider", _playerCollider);
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                if (_objects[i] != null)
                {
                    Object.DestroyImmediate(_objects[i]);
                }
            }

            _objects.Clear();
        }

        [Test]
        public void Initialize_CachesFourPatternsPerSlotAndStartsFlat()
        {
            Assert.That(_map.Initialize(), Is.True);
            Assert.That(_first.PatternInstanceCount, Is.EqualTo(4));
            Assert.That(_second.PatternInstanceCount, Is.EqualTo(4));
            Assert.That(_map.TrailingPatternId, Is.EqualTo("Flat"));
            Assert.That(_map.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(_first.CanPairWith(_second), Is.True);
            Assert.That(CountActivePatterns(_first), Is.EqualTo(1));
            Assert.That(CountActivePatterns(_second), Is.EqualTo(1));
            Assert.That(_first.TryGetPatternInstance(
                "Flat", out InfinitePatternAuthoring firstFlat), Is.True);
            Assert.That(_second.TryGetPatternInstance(
                "Flat", out InfinitePatternAuthoring secondFlat), Is.True);
            Assert.That(firstFlat, Is.Not.SameAs(secondFlat));
            Assert.That(firstFlat.StartAnchor.position,
                Is.EqualTo(new Vector3(-22.0f, 0.0f, 0.0f)));
            Assert.That(secondFlat.StartAnchor.position,
                Is.EqualTo(firstFlat.EndAnchor.position));
        }

        [Test]
        public void Advance_RequestedPattern_AlignsAnchorsAndBoundaryPoint()
        {
            _map.Initialize();
            SetField(_first.AdvanceBoundary, "_isTriggered", true);
            MovePlayerPastTrailingEnd();
            Assert.That(_map.TryRequestNextPattern(1, "SingleRise"), Is.True);

            Assert.That(_map.TryAdvance(1), Is.True);

            Assert.That(_map.AdvanceCount, Is.EqualTo(1));
            Assert.That(_map.CurrentPatternId, Is.EqualTo("SingleRise"));
            Assert.That(_map.TrailingPatternId, Is.EqualTo("Flat"));
            Assert.That(_first.TryGetCurrentPattern(
                out InfinitePatternAuthoring current), Is.True);
            Assert.That(_second.TryGetCurrentPattern(
                out InfinitePatternAuthoring previous), Is.True);
            Assert.That(current.StartAnchor.position,
                Is.EqualTo(previous.EndAnchor.position));
            Assert.That(_first.AdvanceBoundary.transform.position,
                Is.EqualTo(current.AdvanceBoundaryPoint.position));
            Assert.That(CountActivePatterns(_first), Is.EqualTo(1));
            Assert.That(_first.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(_first.TryGetPatternInstance(
                "Flat", out InfinitePatternAuthoring oldFlat), Is.True);
            Assert.That(oldFlat.gameObject.activeInHierarchy, Is.False);
            Assert.That(oldFlat.TryGetTerrainCollider(
                0, out Collider oldCollider), Is.True);
            Assert.That(oldCollider.gameObject.activeInHierarchy, Is.False);
        }

        [Test]
        public void Advance_WrongBoundary_DoesNotConsumeRequest()
        {
            _map.Initialize();
            MovePlayerPastTrailingEnd();
            Assert.That(_map.TryRequestNextPattern(1, "SingleRise"), Is.True);

            Assert.That(_map.TryAdvance(0), Is.False);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That(_map.TryAdvance(1), Is.True);
        }

        [Test]
        public void Advance_AlternatesManyTimesWithoutNewInstances()
        {
            _map.Initialize();
            int firstCount = _first.ContentRoot.childCount;
            int secondCount = _second.ContentRoot.childCount;
            string[] requested =
            {
                "SingleRise", "LegacySteps", "InternalGap", "Flat",
                "SingleRise", "LegacySteps"
            };

            for (int i = 0; i < requested.Length; i++)
            {
                MovePlayerPastTrailingEnd();
                Assert.That(_map.TryRequestNextPattern(i + 1, requested[i]),
                    Is.True);
                Assert.That(_map.TryAdvance((i + 1) % 2), Is.True);
                Assert.That(_map.CurrentPatternId, Is.EqualTo(requested[i]));
                Assert.That(CountActivePatterns(_first), Is.EqualTo(1));
                Assert.That(CountActivePatterns(_second), Is.EqualTo(1));
            }

            Assert.That(_map.AdvanceCount, Is.EqualTo(requested.Length));
            Assert.That(_first.ContentRoot.childCount, Is.EqualTo(firstCount));
            Assert.That(_second.ContentRoot.childCount, Is.EqualTo(secondCount));
        }

        [Test]
        public void Request_UnknownAndDuplicateIds_DoNotChangeState()
        {
            _map.Initialize();
            Assert.That(_map.TryRequestNextPattern(1, "Unknown"), Is.False);
            Assert.That(_map.TryRequestNextPattern(1, "SingleRise"), Is.True);
            Assert.That(_map.TryRequestNextPattern(2, "Flat"), Is.False);
            MovePlayerPastTrailingEnd();
            Assert.That(_map.TryAdvance(1), Is.True);
            Assert.That(_map.TryRequestNextPattern(1, "Flat"), Is.False);
            Assert.That(_map.TryAdvance(1), Is.False);
            Assert.That(_map.AdvanceCount, Is.EqualTo(1));
        }

        [Test]
        public void Request_DisconnectedPattern_IsRejected()
        {
            _map.Initialize();
            InfinitePatternDefinition flat = CreateDefinition(
                "Flat", 0.5f);
            InfinitePatternDefinition raised = CreateDefinition(
                "SingleRise", 1.5f);
            InfinitePatternCatalog disconnected =
                new InfinitePatternCatalog();
            Assert.That(disconnected.Initialize(
                new[] { flat, raised }), Is.True);
            Assert.That(disconnected.CanConnect(
                "Flat", "SingleRise"), Is.False);
            SetField(_map, "_phase2Catalog", disconnected);

            Assert.That(_map.TryRequestNextPattern(
                1, "SingleRise"), Is.False);
            Assert.That(_map.AdvanceCount, Is.Zero);
        }

        [Test]
        public void Advance_PlayerStillUsingTrailingPattern_IsRejected()
        {
            _map.Initialize();
            _playerCollider.transform.position = Vector3.zero;
            Physics.SyncTransforms();
            Assert.That(_map.TryRequestNextPattern(1, "SingleRise"), Is.True);

            Assert.That(_map.TryAdvance(1), Is.False);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That(_first.CurrentPatternId, Is.EqualTo("Flat"));

            MovePlayerPastTrailingEnd();
            Assert.That(_map.TryAdvance(1), Is.True);
        }

        [Test]
        public void ResetPatterns_RestoresInitialSlotsAndRequestState()
        {
            _map.Initialize();
            MovePlayerPastTrailingEnd();
            _map.TryRequestNextPattern(1, "InternalGap");
            _map.TryAdvance(1);

            Assert.That(_map.ResetPatterns(), Is.True);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That(_map.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(_map.TrailingPatternId, Is.EqualTo("Flat"));
            Assert.That(_first.transform.position, Is.EqualTo(Vector3.zero));
            Assert.That(_second.transform.position,
                Is.EqualTo(new Vector3(44.0f, 0.0f, 0.0f)));
            Assert.That(_first.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(_second.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(_map.TryRequestNextPattern(1, "SingleRise"), Is.True);
        }

        [Test]
        public void Initialize_Again_RestoresSameInitialState()
        {
            _map.Initialize();
            MovePlayerPastTrailingEnd();
            _map.TryRequestNextPattern(1, "InternalGap");
            _map.TryAdvance(1);

            Assert.That(_map.Initialize(), Is.True);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That(_map.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(_first.ContentRoot.childCount, Is.EqualTo(4));
            Assert.That(_second.ContentRoot.childCount, Is.EqualTo(4));
        }

        [Test]
        public void ApplyWorldRebaseOffset_MovesRootOnceAndPreservesPatternState()
        {
            Assert.That(_map.Initialize(), Is.True);
            Transform infiniteModeRoot = _map.transform.parent;
            Vector3 rootPosition = infiniteModeRoot.position;
            Vector3 firstPosition = _first.transform.position;
            Vector3 secondPosition = _second.transform.position;
            string currentPatternId = _map.CurrentPatternId;
            string trailingPatternId = _map.TrailingPatternId;
            int advanceCount = _map.AdvanceCount;
            bool firstTriggered = _first.AdvanceBoundary.IsTriggered;
            bool secondTriggered = _second.AdvanceBoundary.IsTriggered;

            Assert.That(_map.TryApplyWorldRebaseOffset(-880.0f), Is.True);

            Assert.That(infiniteModeRoot.position.x,
                Is.EqualTo(rootPosition.x - 880.0f));
            Assert.That(_first.transform.position.x,
                Is.EqualTo(firstPosition.x - 880.0f));
            Assert.That(_second.transform.position.x,
                Is.EqualTo(secondPosition.x - 880.0f));
            Assert.That(_second.transform.position.x - _first.transform.position.x,
                Is.EqualTo(44.0f));
            Assert.That(_map.CurrentPatternId, Is.EqualTo(currentPatternId));
            Assert.That(_map.TrailingPatternId, Is.EqualTo(trailingPatternId));
            Assert.That(_map.AdvanceCount, Is.EqualTo(advanceCount));
            Assert.That(_first.AdvanceBoundary.IsTriggered, Is.EqualTo(firstTriggered));
            Assert.That(_second.AdvanceBoundary.IsTriggered, Is.EqualTo(secondTriggered));
        }

        [TestCase(0.0f)]
        [TestCase(1.0f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void ApplyWorldRebaseOffset_InvalidOffsetLeavesRootUnchanged(
            float worldXOffset)
        {
            Assert.That(_map.Initialize(), Is.True);
            Vector3 rootPosition = _map.transform.parent.position;

            Assert.That(_map.TryApplyWorldRebaseOffset(worldXOffset), Is.False);
            Assert.That(_map.transform.parent.position, Is.EqualTo(rootPosition));
        }

        [Test]
        public void Clear_RemovesOwnedInstancesAndAllowsReinitialization()
        {
            _map.Initialize();
            _map.ClearPatternSlots();

            Assert.That(_map.IsInitialized, Is.False);
            Assert.That(_first.PatternInstanceCount, Is.Zero);
            Assert.That(_second.PatternInstanceCount, Is.Zero);
            Assert.That(_first.ContentRoot.childCount, Is.Zero);
            Assert.That(_second.ContentRoot.childCount, Is.Zero);
            Assert.That(_map.Initialize(), Is.True);
        }

        [Test]
        public void Initialize_InvalidPrefabCanBeFixedWithoutPartialInstances()
        {
            SetField(_prefabs[1], "_patternId", "Flat");

            Assert.That(_map.Initialize(), Is.False);
            Assert.That(_first.ContentRoot.childCount, Is.Zero);
            Assert.That(_second.ContentRoot.childCount, Is.Zero);

            SetField(_prefabs[1], "_patternId", "SingleRise");
            Assert.That(_map.Initialize(), Is.True);
        }

        private InfinitePatternSlot CreateSlot(
            Transform parent,
            int id,
            Vector3 position)
        {
            GameObject slotObject = CreateObject("Slot_" + id, parent);
            slotObject.transform.localPosition = position;
            Transform content = CreateObject(
                "ContentRoot", slotObject.transform).transform;
            GameObject boundaryObject = CreateObject(
                "Boundary", slotObject.transform);
            BoxCollider trigger = boundaryObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            InfinitePatternBoundary boundary =
                boundaryObject.AddComponent<InfinitePatternBoundary>();
            SetField(boundary, "_boundaryId", id);
            SetField(boundary, "_playerCollider", _playerCollider);
            SetField(boundary, "_mapPattern", _map);
            InfinitePatternSlot slot =
                slotObject.AddComponent<InfinitePatternSlot>();
            SetField(slot, "_slotId", id);
            SetField(slot, "_contentRoot", content);
            SetField(slot, "_advanceBoundary", boundary);
            return slot;
        }

        private InfinitePatternDefinition CreateDefinition(
            string id,
            float groundTopY)
        {
            InfinitePatternDefinition definition =
                new InfinitePatternDefinition();
            Assert.That(definition.Initialize(
                id,
                "Test",
                E_InfinitePatternDifficulty.D1,
                new Vector3(-22.0f, 0.0f, 0.0f),
                new Vector3(22.0f, 0.0f, 0.0f),
                Vector3.right,
                Vector3.right,
                2.0f,
                2.0f,
                groundTopY,
                groundTopY,
                4.0f,
                4.0f), Is.True);
            return definition;
        }

        private InfinitePatternAuthoring CreatePrefab(
            string patternId,
            E_InfinitePatternDifficulty difficulty)
        {
            GameObject root = CreateObject(patternId, null);
            Transform geometry = CreateObject("GeometryRoot", root.transform).transform;
            Transform start = CreateObject("StartAnchor", root.transform).transform;
            Transform end = CreateObject("EndAnchor", root.transform).transform;
            Transform boundary = CreateObject(
                "BoundaryPoint", root.transform).transform;
            Transform collectibles = CreateObject(
                "CollectibleRoot", root.transform).transform;
            GameObject ground = CreateObject("Ground", geometry);
            ground.layer = 6;
            BoxCollider groundCollider = ground.AddComponent<BoxCollider>();
            start.localPosition = new Vector3(-22.0f, 0.0f, 0.0f);
            end.localPosition = new Vector3(22.0f, 0.0f, 0.0f);
            InfinitePatternGeometry.TryGetAdvanceBoundaryPoint(
                patternId, out Vector3 boundaryPoint);
            boundary.localPosition = boundaryPoint;
            InfinitePatternAuthoring authoring =
                root.AddComponent<InfinitePatternAuthoring>();
            SetField(authoring, "_patternId", patternId);
            SetField(authoring, "_minimumDifficulty", difficulty);
            SetField(authoring, "_geometryRoot", geometry);
            SetField(authoring, "_startAnchor", start);
            SetField(authoring, "_endAnchor", end);
            SetField(authoring, "_advanceBoundaryPoint", boundary);
            SetField(authoring, "_collectibleRoot", collectibles);
            SetField(authoring, "_terrainColliders", new Collider[] { groundCollider });
            return authoring;
        }

        private void MovePlayerPastTrailingEnd()
        {
            InfinitePatternSlot trailing = _map.AdvanceCount % 2 == 0
                ? _first
                : _second;
            trailing.TryGetCurrentPattern(out InfinitePatternAuthoring pattern);
            _playerCollider.transform.position =
                pattern.EndAnchor.position + new Vector3(2.0f, 1.0f, 0.0f);
            Physics.SyncTransforms();
        }

        private int CountActivePatterns(InfinitePatternSlot slot)
        {
            int count = 0;

            for (int i = 0; i < slot.ContentRoot.childCount; i++)
            {
                if (slot.ContentRoot.GetChild(i).gameObject.activeSelf)
                {
                    count++;
                }
            }

            return count;
        }

        private GameObject CreateObject(string name, Transform parent)
        {
            GameObject created = new GameObject(name);
            created.transform.SetParent(parent, false);
            _objects.Add(created);
            return created;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, fieldName);
            field.SetValue(target, value);
        }
    }
}
