using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class JumpFeatureTests
    {
        private const float Tolerance = 0.0001f;

        private GameObject _gameObject;
        private JumpFeature _jumpFeature;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(nameof(JumpFeatureTests));
            _jumpFeature = _gameObject.AddComponent<JumpFeature>();
            _jumpFeature.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void TryStartJump_GroundedInput_StartsWithExpectedVerticalSpeed()
        {
            MakeGrounded();

            bool didStart = _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out float verticalSpeed);

            Assert.That(didStart, Is.True);
            Assert.That(verticalSpeed, Is.EqualTo(12.247449f).Within(Tolerance));
        }

        [Test]
        public void TryStartJump_SecondAirborneInput_IsRejected()
        {
            MakeGrounded();
            _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out _);

            bool didStartAgain = _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out float verticalSpeed);

            Assert.That(didStartAgain, Is.False);
            Assert.That(verticalSpeed, Is.Zero);
        }

        [Test]
        public void TryStartJump_WithinCoyoteTime_StartsJump()
        {
            MakeGrounded();
            _jumpFeature.UpdateCoyoteTime(
                E_PlayerMovementState.Airborne,
                0.05f);

            bool didStart = _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out _);

            Assert.That(didStart, Is.True);
        }

        [Test]
        public void TryStartJump_AfterCoyoteTime_IsRejected()
        {
            MakeGrounded();
            _jumpFeature.UpdateCoyoteTime(
                E_PlayerMovementState.Airborne,
                0.11f);

            bool didStart = _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out _);

            Assert.That(didStart, Is.False);
        }

        [Test]
        public void CompleteLanding_AllowsNextGroundedJump()
        {
            MakeGrounded();
            _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out _);
            _jumpFeature.CompleteLanding();
            MakeGrounded();

            bool didStartAgain = _jumpFeature.TryStartJump(
                CreateJumpInput(),
                25.0f,
                out _);

            Assert.That(didStartAgain, Is.True);
        }

        private void MakeGrounded()
        {
            _jumpFeature.UpdateCoyoteTime(
                E_PlayerMovementState.Grounded,
                0.02f);
        }

        private PlayerInputState CreateJumpInput()
        {
            return new PlayerInputState(0.0f, true, false);
        }
    }
}
