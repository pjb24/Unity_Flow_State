using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternDefinitionTests
    {
        [Test]
        public void Initialize_ValidDefinition_StoresContract()
        {
            InfinitePatternDefinition definition = CreateDefinition();

            Assert.That(definition.IsInitialized, Is.True);
            Assert.That(definition.Id, Is.EqualTo("Flat"));
            Assert.That(definition.Purpose, Is.EqualTo("Start and fallback"));
            Assert.That(
                definition.MinimumDifficulty,
                Is.EqualTo(E_InfinitePatternDifficulty.D1));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Initialize_MissingId_IsRejected(string id)
        {
            InfinitePatternDefinition definition = CreateDefinition(id: id);

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_MissingPurpose_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(purpose: "");

            Assert.That(definition.IsInitialized, Is.False);
        }

        [TestCase(E_InfinitePatternDifficulty.None)]
        [TestCase((E_InfinitePatternDifficulty)4)]
        public void Initialize_InvalidDifficulty_IsRejected(
            E_InfinitePatternDifficulty difficulty)
        {
            InfinitePatternDefinition definition = CreateDefinition(
                difficulty: difficulty);

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_LengthWithinTolerance_IsAccepted()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                endAnchor: new Vector3(22.009f, 0.0f, 0.0f));

            Assert.That(definition.IsInitialized, Is.True);
        }

        [Test]
        public void Initialize_LengthOutsideTolerance_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                endAnchor: new Vector3(22.011f, 0.0f, 0.0f));

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_AnchorHeightMismatch_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                endAnchor: new Vector3(22.0f, 0.011f, 0.0f));

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_NonForwardTangent_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                endTangent: Quaternion.Euler(0.0f, 0.101f, 0.0f) *
                            Vector3.right);

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_InvalidGroundWidth_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                endGroundWidth: 4.011f);

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_GroundInsetsConsumePattern_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                startGroundInset: 22.0f,
                endGroundInset: 22.0f);

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void Initialize_NonFiniteAnchor_IsRejected()
        {
            InfinitePatternDefinition definition = CreateDefinition(
                endAnchor: new Vector3(float.NaN, 0.0f, 0.0f));

            Assert.That(definition.IsInitialized, Is.False);
        }

        [Test]
        public void CanConnectTo_ValidGroundGap_IsAccepted()
        {
            InfinitePatternDefinition previous = CreateDefinition(id: "Flat");
            InfinitePatternDefinition next = CreateDefinition(id: "SingleRise");

            Assert.That(previous.CanConnectTo(next), Is.True);
        }

        [Test]
        public void CanConnectTo_GroundGapOutsideTolerance_IsRejected()
        {
            InfinitePatternDefinition previous = CreateDefinition(id: "Flat");
            InfinitePatternDefinition next = CreateDefinition(
                id: "SingleRise",
                startGroundInset: 2.011f);

            Assert.That(previous.CanConnectTo(next), Is.False);
        }

        [Test]
        public void CanConnectTo_GroundHeightOutsideTolerance_IsRejected()
        {
            InfinitePatternDefinition previous = CreateDefinition(id: "Flat");
            InfinitePatternDefinition next = CreateDefinition(
                id: "SingleRise",
                startGroundTopY: 0.511f);

            Assert.That(previous.CanConnectTo(next), Is.False);
        }

        [Test]
        public void CanConnectTo_TangentDifferenceOutsideTolerance_IsRejected()
        {
            Vector3 positiveTangent =
                Quaternion.Euler(0.0f, 0.09f, 0.0f) * Vector3.right;
            Vector3 negativeTangent =
                Quaternion.Euler(0.0f, -0.09f, 0.0f) * Vector3.right;
            InfinitePatternDefinition previous = CreateDefinition(
                id: "Flat",
                endTangent: positiveTangent);
            InfinitePatternDefinition next = CreateDefinition(
                id: "SingleRise",
                startTangent: negativeTangent);

            Assert.That(previous.IsInitialized, Is.True);
            Assert.That(next.IsInitialized, Is.True);
            Assert.That(previous.CanConnectTo(next), Is.False);
        }

        private static InfinitePatternDefinition CreateDefinition(
            string id = "Flat",
            string purpose = "Start and fallback",
            E_InfinitePatternDifficulty difficulty =
                E_InfinitePatternDifficulty.D1,
            Vector3 startAnchor = default(Vector3),
            Vector3 endAnchor = default(Vector3),
            Vector3 startTangent = default(Vector3),
            Vector3 endTangent = default(Vector3),
            float startGroundInset = 2.0f,
            float endGroundInset = 2.0f,
            float startGroundTopY = 0.5f,
            float endGroundTopY = 0.5f,
            float startGroundWidth = 4.0f,
            float endGroundWidth = 4.0f)
        {
            if (startAnchor == default(Vector3))
            {
                startAnchor = new Vector3(-22.0f, 0.0f, 0.0f);
            }

            if (endAnchor == default(Vector3))
            {
                endAnchor = new Vector3(22.0f, 0.0f, 0.0f);
            }

            if (startTangent == default(Vector3))
            {
                startTangent = Vector3.right;
            }

            if (endTangent == default(Vector3))
            {
                endTangent = Vector3.right;
            }

            InfinitePatternDefinition definition =
                new InfinitePatternDefinition();
            definition.Initialize(
                id,
                purpose,
                difficulty,
                startAnchor,
                endAnchor,
                startTangent,
                endTangent,
                startGroundInset,
                endGroundInset,
                startGroundTopY,
                endGroundTopY,
                startGroundWidth,
                endGroundWidth);
            return definition;
        }
    }
}
