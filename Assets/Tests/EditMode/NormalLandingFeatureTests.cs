using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class NormalLandingFeatureTests
    {
        private GameObject _gameObject;
        private NormalLandingFeature _feature;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(
                nameof(NormalLandingFeatureTests));
            _feature = _gameObject.AddComponent<NormalLandingFeature>();
            _feature.Initialize();
            _feature.BeginJump();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void TryCompleteLanding_NoMomentum_CompletesNormalLanding()
        {
            bool didComplete = _feature.TryCompleteLanding(
                CreateGroundedState(),
                false);

            Assert.That(didComplete, Is.True);
        }

        [Test]
        public void TryCompleteLanding_MomentumCompleted_RejectsNormalLanding()
        {
            bool didComplete = _feature.TryCompleteLanding(
                CreateGroundedState(),
                true);

            Assert.That(didComplete, Is.False);
        }

        private PlayerCollisionState CreateGroundedState()
        {
            return new PlayerCollisionState(
                true,
                0.0f,
                Vector3.zero,
                Vector3.up);
        }
    }
}
