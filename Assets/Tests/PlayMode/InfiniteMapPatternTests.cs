using System.Reflection;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.PlayMode
{
    public class InfiniteMapPatternTests
    {
        private const int FirstBoundaryId = 0;
        private const int SecondBoundaryId = 1;
        private const float PatternLength = 10.0f;

        private GameObject _rootObject;
        private GameObject _playerObject;
        private GameObject _otherObject;
        private Transform _firstPattern;
        private Transform _firstStartAnchor;
        private Transform _firstEndAnchor;
        private Transform _secondPattern;
        private Transform _secondStartAnchor;
        private Transform _secondEndAnchor;
        private Collider _playerCollider;
        private Collider _otherCollider;
        private Collider _firstGroundCollider;
        private InfinitePatternBoundary _firstBoundary;
        private InfinitePatternBoundary _secondBoundary;
        private InfiniteMapPattern _mapPattern;

        [SetUp]
        public void SetUp()
        {
            _rootObject = new GameObject("InfiniteMapPatternTests.Root");
            _playerObject = new GameObject("InfiniteMapPatternTests.Player");
            _otherObject = new GameObject("InfiniteMapPatternTests.Other");
            _playerCollider = _playerObject.AddComponent<CapsuleCollider>();
            _otherCollider = _otherObject.AddComponent<BoxCollider>();

            _firstPattern = CreatePattern(
                "InfiniteMapPatternTests.Pattern0",
                Vector3.zero,
                out _firstStartAnchor,
                out _firstEndAnchor,
                out _firstGroundCollider,
                out _firstBoundary);
            _secondPattern = CreatePattern(
                "InfiniteMapPatternTests.Pattern1",
                new Vector3(PatternLength, 0.0f, 0.0f),
                out _secondStartAnchor,
                out _secondEndAnchor,
                out _,
                out _secondBoundary);

            _mapPattern = _rootObject.AddComponent<InfiniteMapPattern>();
            ConfigureBoundary(_firstBoundary, FirstBoundaryId);
            ConfigureBoundary(_secondBoundary, SecondBoundaryId);
            ConfigureMapPattern();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_rootObject);
            Object.DestroyImmediate(_playerObject);
            Object.DestroyImmediate(_otherObject);
        }

        [Test]
        public void Initialize_ValidPatternPair_PreparesInitialState()
        {
            bool didInitialize = _mapPattern.Initialize();

            Assert.That(didInitialize, Is.True);
            Assert.That(_mapPattern.IsInitialized, Is.True);
            Assert.That(_mapPattern.AdvanceCount, Is.Zero);
        }

        [Test]
        public void TryAdvance_FrontBoundary_MovesTrailingPatternAfterFront()
        {
            _mapPattern.Initialize();
            _playerObject.transform.position = new Vector3(15.0f, 2.0f, 0.0f);
            Vector3 playerPosition = _playerObject.transform.position;

            bool didAdvance = _mapPattern.TryAdvance(SecondBoundaryId);

            Assert.That(didAdvance, Is.True);
            Assert.That(
                _firstStartAnchor.position,
                Is.EqualTo(_secondEndAnchor.position));
            Assert.That(_firstPattern.position.x, Is.EqualTo(20.0f));
            Assert.That(_firstGroundCollider.enabled, Is.True);
            Assert.That(_playerObject.transform.position, Is.EqualTo(playerPosition));
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
        }

        [Test]
        public void TryAdvance_SameBoundary_IsRejectedAfterFirstRequest()
        {
            _mapPattern.Initialize();

            bool didAdvance = _mapPattern.TryAdvance(SecondBoundaryId);
            bool didAdvanceAgain = _mapPattern.TryAdvance(SecondBoundaryId);

            Assert.That(didAdvance, Is.True);
            Assert.That(didAdvanceAgain, Is.False);
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
        }

        [Test]
        public void TryAdvance_AlternatingBoundaries_ReusesBothPatterns()
        {
            _mapPattern.Initialize();

            bool didAdvanceFirst =
                _mapPattern.TryAdvance(SecondBoundaryId);
            bool didAdvanceSecond =
                _mapPattern.TryAdvance(FirstBoundaryId);

            Assert.That(didAdvanceFirst, Is.True);
            Assert.That(didAdvanceSecond, Is.True);
            Assert.That(
                _secondStartAnchor.position,
                Is.EqualTo(_firstEndAnchor.position));
            Assert.That(_secondPattern.position.x, Is.EqualTo(30.0f));
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(2));
        }

        [Test]
        public void ResetPatterns_AfterAdvance_RestoresInitialTransforms()
        {
            _mapPattern.Initialize();
            _mapPattern.TryAdvance(SecondBoundaryId);
            _mapPattern.TryAdvance(FirstBoundaryId);

            bool didReset = _mapPattern.ResetPatterns();

            Assert.That(didReset, Is.True);
            Assert.That(_firstPattern.position, Is.EqualTo(Vector3.zero));
            Assert.That(
                _secondPattern.position,
                Is.EqualTo(new Vector3(PatternLength, 0.0f, 0.0f)));
            Assert.That(_firstBoundary.IsTriggered, Is.False);
            Assert.That(_secondBoundary.IsTriggered, Is.False);
            Assert.That(_mapPattern.AdvanceCount, Is.Zero);
        }

        [Test]
        public void Boundary_PlayerTrigger_AdvancesPatternOnce()
        {
            _mapPattern.Initialize();

            InvokeTriggerEnter(_secondBoundary, _playerCollider);
            InvokeTriggerEnter(_secondBoundary, _playerCollider);

            Assert.That(_secondBoundary.IsTriggered, Is.True);
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
        }

        [Test]
        public void Boundary_NonPlayerTrigger_DoesNotAdvancePattern()
        {
            _mapPattern.Initialize();

            InvokeTriggerEnter(_secondBoundary, _otherCollider);

            Assert.That(_secondBoundary.IsTriggered, Is.False);
            Assert.That(_mapPattern.AdvanceCount, Is.Zero);
        }

        private Transform CreatePattern(
            string patternName,
            Vector3 position,
            out Transform startAnchor,
            out Transform endAnchor,
            out Collider groundCollider,
            out InfinitePatternBoundary boundary)
        {
            GameObject patternObject = new GameObject(patternName);
            patternObject.transform.SetParent(_rootObject.transform);
            patternObject.transform.position = position;
            groundCollider = patternObject.AddComponent<BoxCollider>();

            startAnchor = CreateChildTransform(
                patternObject.transform,
                "StartAnchor",
                Vector3.zero);
            endAnchor = CreateChildTransform(
                patternObject.transform,
                "EndAnchor",
                new Vector3(PatternLength, 0.0f, 0.0f));

            GameObject boundaryObject = new GameObject("AdvanceBoundary");
            boundaryObject.transform.SetParent(patternObject.transform);
            boundaryObject.transform.localPosition =
                new Vector3(PatternLength * 0.75f, 0.0f, 0.0f);
            BoxCollider boundaryCollider =
                boundaryObject.AddComponent<BoxCollider>();
            boundaryCollider.isTrigger = true;
            boundary = boundaryObject.AddComponent<InfinitePatternBoundary>();
            return patternObject.transform;
        }

        private Transform CreateChildTransform(
            Transform parent,
            string childName,
            Vector3 localPosition)
        {
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(parent);
            childObject.transform.localPosition = localPosition;
            return childObject.transform;
        }

        private void ConfigureBoundary(
            InfinitePatternBoundary boundary,
            int boundaryId)
        {
            SetPrivateField(boundary, "_playerCollider", _playerCollider);
            SetPrivateField(boundary, "_mapPattern", _mapPattern);
            SetPrivateField(boundary, "_boundaryId", boundaryId);
        }

        private void ConfigureMapPattern()
        {
            SetPrivateField(_mapPattern, "_firstPattern", _firstPattern);
            SetPrivateField(
                _mapPattern,
                "_firstStartAnchor",
                _firstStartAnchor);
            SetPrivateField(_mapPattern, "_firstEndAnchor", _firstEndAnchor);
            SetPrivateField(_mapPattern, "_firstBoundary", _firstBoundary);
            SetPrivateField(_mapPattern, "_secondPattern", _secondPattern);
            SetPrivateField(
                _mapPattern,
                "_secondStartAnchor",
                _secondStartAnchor);
            SetPrivateField(
                _mapPattern,
                "_secondEndAnchor",
                _secondEndAnchor);
            SetPrivateField(_mapPattern, "_secondBoundary", _secondBoundary);
        }

        private void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private void InvokeTriggerEnter(
            InfinitePatternBoundary boundary,
            Collider otherCollider)
        {
            MethodInfo method = boundary.GetType().GetMethod(
                "OnTriggerEnter",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(method, Is.Not.Null);
            method.Invoke(boundary, new object[] { otherCollider });
        }
    }
}
