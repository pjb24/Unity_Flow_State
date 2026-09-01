using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ScoreCalculatorTests
    {
        private const float ScorePerUnit = 10.0f;

        private ScoreCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new ScoreCalculator();
        }

        [Test]
        public void NewCalculator_IsNotInitialized()
        {
            Assert.That(_calculator.IsInitialized, Is.False);
            Assert.That(_calculator.ScorePerUnit, Is.Zero);
        }

        [Test]
        public void Initialize_ValidScorePerUnit_StoresConfiguration()
        {
            bool didInitialize = _calculator.Initialize(ScorePerUnit);

            Assert.That(didInitialize, Is.True);
            Assert.That(_calculator.IsInitialized, Is.True);
            Assert.That(_calculator.ScorePerUnit, Is.EqualTo(ScorePerUnit));
        }

        [TestCase(0.0f)]
        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Initialize_InvalidScorePerUnit_IsRejected(float scorePerUnit)
        {
            bool didInitialize = _calculator.Initialize(scorePerUnit);

            Assert.That(didInitialize, Is.False);
            Assert.That(_calculator.IsInitialized, Is.False);
            Assert.That(_calculator.ScorePerUnit, Is.Zero);
        }

        [Test]
        public void TryCalculate_BeforeInitialize_IsRejected()
        {
            bool didCalculate = _calculator.TryCalculate(1.0f, out int score);

            Assert.That(didCalculate, Is.False);
            Assert.That(score, Is.Zero);
        }

        [Test]
        public void TryCalculate_ZeroDistance_ReturnsZero()
        {
            Initialize();

            bool didCalculate = _calculator.TryCalculate(0.0f, out int score);

            Assert.That(didCalculate, Is.True);
            Assert.That(score, Is.Zero);
        }

        [Test]
        public void TryCalculate_MinimumPositiveDistance_BelowFirstPoint_ReturnsZero()
        {
            Initialize();

            bool didCalculate = _calculator.TryCalculate(
                float.Epsilon,
                out int score);

            Assert.That(didCalculate, Is.True);
            Assert.That(score, Is.Zero);
        }

        [TestCase(0.099f, 0)]
        [TestCase(0.1f, 1)]
        [TestCase(1.0f, 10)]
        [TestCase(1.29f, 12)]
        [TestCase(1.3f, 13)]
        public void TryCalculate_Distance_UsesLinearRateAndFloor(
            float distance,
            int expectedScore)
        {
            Initialize();

            bool didCalculate = _calculator.TryCalculate(
                distance,
                out int score);

            Assert.That(didCalculate, Is.True);
            Assert.That(score, Is.EqualTo(expectedScore));
        }

        [Test]
        public void TryCalculate_DifferentRate_UsesOwnedConfiguration()
        {
            Assert.That(_calculator.Initialize(2.0f), Is.True);

            bool didCalculate = _calculator.TryCalculate(2.75f, out int score);

            Assert.That(didCalculate, Is.True);
            Assert.That(score, Is.EqualTo(5));
        }

        [Test]
        public void TryCalculate_LargeResult_SaturatesAtIntMaxValue()
        {
            Initialize();

            bool didCalculate = _calculator.TryCalculate(
                float.MaxValue,
                out int score);

            Assert.That(didCalculate, Is.True);
            Assert.That(score, Is.EqualTo(int.MaxValue));
        }

        [TestCase(-0.001f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void TryCalculate_InvalidDistance_IsRejected(float distance)
        {
            Initialize();

            bool didCalculate = _calculator.TryCalculate(
                distance,
                out int score);

            Assert.That(didCalculate, Is.False);
            Assert.That(score, Is.Zero);
        }

        [Test]
        public void TryCalculate_SameDistance_ReturnsSameScore()
        {
            Initialize();

            bool didCalculateFirst = _calculator.TryCalculate(
                12.345f,
                out int firstScore);
            bool didCalculateSecond = _calculator.TryCalculate(
                12.345f,
                out int secondScore);

            Assert.That(didCalculateFirst, Is.True);
            Assert.That(didCalculateSecond, Is.True);
            Assert.That(secondScore, Is.EqualTo(firstScore));
        }

        [Test]
        public void Reset_InitializedCalculator_ClearsConfiguration()
        {
            Initialize();

            _calculator.Reset();

            Assert.That(_calculator.IsInitialized, Is.False);
            Assert.That(_calculator.ScorePerUnit, Is.Zero);
            Assert.That(
                _calculator.TryCalculate(1.0f, out int score),
                Is.False);
            Assert.That(score, Is.Zero);
        }

        private void Initialize()
        {
            Assert.That(_calculator.Initialize(ScorePerUnit), Is.True);
        }
    }
}
