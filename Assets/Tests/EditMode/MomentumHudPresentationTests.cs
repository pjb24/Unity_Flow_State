using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class MomentumHudPresentationTests
    {
        [TestCase(1.0, 0.0, "x1.00", 0.0f)]
        [TestCase(1.25, 1.0, "x1.25", 1.0f)]
        [TestCase(1.5, 0.6, "x1.50", 0.6f)]
        [TestCase(3.0, 0.1, "x3.00", 0.1f)]
        [TestCase(2.0, -1.0, "x2.00", 0.0f)]
        [TestCase(2.0, 2.0, "x2.00", 1.0f)]
        public void Create_ValidState_FormatsMultiplierAndClampsFill(
            double multiplier,
            double ratio,
            string expectedText,
            float expectedFill)
        {
            MomentumHudPresentation presentation = MomentumHudPresenter.Create(
                multiplier,
                ratio);

            Assert.That(presentation.MultiplierText, Is.EqualTo(expectedText));
            Assert.That(presentation.FillAmount, Is.EqualTo(expectedFill));
        }

        [TestCase(double.NaN, 0.5)]
        [TestCase(double.PositiveInfinity, 0.5)]
        [TestCase(0.75, 0.5)]
        [TestCase(3.25, 0.5)]
        [TestCase(1.25, double.NaN)]
        public void Create_InvalidState_ReturnsInactivePlaceholder(
            double multiplier,
            double ratio)
        {
            MomentumHudPresentation presentation = MomentumHudPresenter.Create(
                multiplier,
                ratio);

            Assert.That(presentation.MultiplierText, Is.EqualTo("x--"));
            Assert.That(presentation.FillAmount, Is.Zero);
        }

        [Test]
        public void ApprovedGradient_UsesHorizontalContractKeysAndInterpolation()
        {
            Gradient gradient = MomentumHudPresenter.CreateApprovedGradient();

            Assert.That(MomentumHudPresenter.IsApprovedGradient(gradient), Is.True);
            AssertColor(gradient.Evaluate(0.0f), Color.red);
            AssertColor(gradient.Evaluate(1.0f), Color.cyan);

            Color midpoint = gradient.Evaluate(0.5f);
            Assert.That(midpoint, Is.Not.EqualTo(Color.red));
            Assert.That(midpoint, Is.Not.EqualTo(Color.cyan));
        }

        [Test]
        public void IsApprovedGradient_ChangedKeyIsRejected()
        {
            Gradient gradient = MomentumHudPresenter.CreateApprovedGradient();
            GradientColorKey[] keys = gradient.colorKeys;
            keys[1].time = 0.2f;
            gradient.SetKeys(keys, gradient.alphaKeys);

            Assert.That(MomentumHudPresenter.IsApprovedGradient(gradient), Is.False);
        }

        [Test]
        public void IsApprovedGradient_LinearColorKeysAreAccepted()
        {
            Gradient gradient = MomentumHudPresenter.CreateApprovedGradient();
            GradientColorKey[] keys = gradient.colorKeys;

            for (int index = 0; index < keys.Length; index++)
            {
                keys[index].color = keys[index].color.linear;
            }

            gradient.SetKeys(keys, gradient.alphaKeys);

            Assert.That(MomentumHudPresenter.IsApprovedGradient(gradient), Is.True);
        }

        [Test]
        public void IsApprovedGradient_GammaColorKeysAreAccepted()
        {
            Gradient gradient = MomentumHudPresenter.CreateApprovedGradient();
            GradientColorKey[] keys = gradient.colorKeys;

            for (int index = 0; index < keys.Length; index++)
            {
                keys[index].color = keys[index].color.gamma;
            }

            gradient.SetKeys(keys, gradient.alphaKeys);

            Assert.That(MomentumHudPresenter.IsApprovedGradient(gradient), Is.True);
        }

        private static void AssertColor(Color actual, Color expected)
        {
            Assert.That(actual.r, Is.EqualTo(expected.r).Within(0.0001f));
            Assert.That(actual.g, Is.EqualTo(expected.g).Within(0.0001f));
            Assert.That(actual.b, Is.EqualTo(expected.b).Within(0.0001f));
            Assert.That(actual.a, Is.EqualTo(expected.a).Within(0.0001f));
        }
    }
}
