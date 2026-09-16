using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class MomentumLandingFeatureTests
    {
        private const float Tolerance = 0.0001f;

        private GameObject _gameObject;
        private MomentumLandingFeature _feature;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(
                nameof(MomentumLandingFeatureTests));
            _feature =
                _gameObject.AddComponent<MomentumLandingFeature>();
            _feature.Initialize();
            _feature.BeginJump();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void UpdateWindow_DescendingNearGround_ActivatesWindow()
        {
            _feature.UpdateWindow(
                -10.0f,
                CreateCollisionState(false, 1.0f),
                0.02f);

            Assert.That(_feature.IsWindowActive, Is.True);
        }

        [Test]
        public void UpdateWindow_Ascending_DoesNotActivateWindow()
        {
            _feature.UpdateWindow(
                10.0f,
                CreateCollisionState(false, 1.0f),
                0.02f);

            Assert.That(_feature.IsWindowActive, Is.False);
        }

        [Test]
        public void UpdateWindow_NoPredictedGround_DoesNotActivateWindow()
        {
            _feature.UpdateWindow(
                -10.0f,
                CreateCollisionState(false, float.PositiveInfinity),
                0.02f);

            Assert.That(_feature.IsWindowActive, Is.False);
        }

        [TestCase(0.0f)]
        [TestCase(8.0f)]
        [TestCase(-8.0f)]
        [TestCase(13.0f)]
        [TestCase(20.0f)]
        [TestCase(-20.0f)]
        public void TryCompleteLanding_BufferedInput_PreservesHorizontalSpeed(
            float horizontalSpeed)
        {
            OpenWindowAndBufferInput();

            bool didComplete = _feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f),
                horizontalSpeed,
                out float resultSpeed);

            Assert.That(didComplete, Is.True);
            Assert.That(resultSpeed, Is.EqualTo(horizontalSpeed).Within(Tolerance));
            Assert.That(_feature.SuccessId, Is.EqualTo(1));
        }

        [Test]
        public void BeginJump_PreservesSuccessIdAndNextSuccessIncrementsIt()
        {
            OpenWindowAndBufferInput();
            Assert.That(_feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f), 8.0f, out _), Is.True);

            _feature.BeginJump();
            Assert.That(_feature.SuccessId, Is.EqualTo(1));
            OpenWindowAndBufferInput();

            bool didComplete = _feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f),
                8.0f,
                out float resultSpeed);

            Assert.That(didComplete, Is.True);
            Assert.That(resultSpeed, Is.EqualTo(8.0f).Within(Tolerance));
            Assert.That(_feature.SuccessId, Is.EqualTo(2));
        }

        [Test]
        public void Initialize_AfterSuccess_ResetsSuccessIdAndWindow()
        {
            OpenWindowAndBufferInput();
            Assert.That(_feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f), 8.0f, out _), Is.True);

            _feature.Initialize();

            Assert.That(_feature.SuccessId, Is.Zero);
            Assert.That(_feature.IsWindowActive, Is.False);
            Assert.That(_feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f), 8.0f, out _), Is.False);
        }

        [Test]
        public void BufferInput_BeforeWindow_DoesNotCompleteMomentumLanding()
        {
            _feature.BufferInput(CreateMomentumInput());

            bool didComplete = _feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f),
                8.0f,
                out float resultSpeed);

            Assert.That(didComplete, Is.False);
            Assert.That(_feature.SuccessId, Is.Zero);
            Assert.That(resultSpeed, Is.EqualTo(8.0f).Within(Tolerance));
        }

        [Test]
        public void BufferInput_AfterWindowExpires_DoesNotCompleteMomentumLanding()
        {
            _feature.UpdateWindow(
                -10.0f,
                CreateCollisionState(false, 1.0f),
                0.02f);
            _feature.UpdateWindow(
                -10.0f,
                CreateCollisionState(false, 0.5f),
                0.16f);
            _feature.BufferInput(CreateMomentumInput());

            bool didComplete = _feature.TryCompleteLanding(
                CreateCollisionState(true, 0.0f),
                8.0f,
                out _);

            Assert.That(didComplete, Is.False);
            Assert.That(_feature.SuccessId, Is.Zero);
        }

        [Test]
        public void TryCompleteLanding_SecondAttempt_IsRejected()
        {
            OpenWindowAndBufferInput();
            PlayerCollisionState groundedState =
                CreateCollisionState(true, 0.0f);
            _feature.TryCompleteLanding(
                groundedState,
                8.0f,
                out _);

            bool didCompleteAgain = _feature.TryCompleteLanding(
                groundedState,
                8.0f,
                out _);

            Assert.That(didCompleteAgain, Is.False);
            Assert.That(_feature.SuccessId, Is.EqualTo(1));
        }

        private void OpenWindowAndBufferInput()
        {
            _feature.UpdateWindow(
                -10.0f,
                CreateCollisionState(false, 1.0f),
                0.02f);
            _feature.BufferInput(CreateMomentumInput());
        }

        private PlayerCollisionState CreateCollisionState(
            bool isGrounded,
            float groundDistance)
        {
            return new PlayerCollisionState(
                isGrounded,
                groundDistance,
                Vector3.zero,
                Vector3.up);
        }

        private PlayerInputState CreateMomentumInput()
        {
            return new PlayerInputState(false, true);
        }
    }
}
