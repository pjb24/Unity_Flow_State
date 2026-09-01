using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfiniteDistanceStateTests
    {
        private const float OriginWorldX = 10.0f;

        private InfiniteDistanceState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new InfiniteDistanceState();
        }

        [Test]
        public void NewState_HasZeroDistanceAndIsNotInitialized()
        {
            Assert.That(_state.CurrentDistance, Is.Zero);
            Assert.That(_state.IsInitialized, Is.False);
            Assert.That(_state.IsFinalized, Is.False);
        }

        [Test]
        public void TryUpdate_BeforeInitialize_IsRejected()
        {
            bool didUpdate = _state.TryUpdate(OriginWorldX + 1.0f);

            Assert.That(didUpdate, Is.False);
            Assert.That(_state.CurrentDistance, Is.Zero);
        }

        [Test]
        public void Initialize_ValidOrigin_StartsAtZeroDistance()
        {
            bool didInitialize = _state.Initialize(OriginWorldX);

            Assert.That(didInitialize, Is.True);
            Assert.That(_state.OriginWorldX, Is.EqualTo(OriginWorldX));
            Assert.That(_state.CurrentDistance, Is.Zero);
            Assert.That(_state.IsInitialized, Is.True);
            Assert.That(_state.IsFinalized, Is.False);
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Initialize_InvalidOrigin_IsRejected(float originWorldX)
        {
            bool didInitialize = _state.Initialize(originWorldX);

            Assert.That(didInitialize, Is.False);
            Assert.That(_state.IsInitialized, Is.False);
            Assert.That(_state.CurrentDistance, Is.Zero);
        }

        [Test]
        public void TryUpdate_ForwardPosition_UsesDistanceFromOrigin()
        {
            Initialize();

            bool didUpdate = _state.TryUpdate(OriginWorldX + 12.5f);

            Assert.That(didUpdate, Is.True);
            Assert.That(_state.CurrentDistance, Is.EqualTo(12.5f));
        }

        [Test]
        public void TryUpdate_BackwardPosition_DoesNotReduceMaximumDistance()
        {
            Initialize();
            _state.TryUpdate(OriginWorldX + 12.5f);

            bool didUpdate = _state.TryUpdate(OriginWorldX + 5.0f);

            Assert.That(didUpdate, Is.True);
            Assert.That(_state.CurrentDistance, Is.EqualTo(12.5f));
        }

        [Test]
        public void TryUpdate_PositionBehindOrigin_KeepsZeroDistance()
        {
            Initialize();

            bool didUpdate = _state.TryUpdate(OriginWorldX - 5.0f);

            Assert.That(didUpdate, Is.True);
            Assert.That(_state.CurrentDistance, Is.Zero);
        }

        [Test]
        public void TryUpdate_SamePosition_KeepsCurrentDistance()
        {
            Initialize();
            _state.TryUpdate(OriginWorldX + 5.0f);

            bool didUpdate = _state.TryUpdate(OriginWorldX + 5.0f);

            Assert.That(didUpdate, Is.True);
            Assert.That(_state.CurrentDistance, Is.EqualTo(5.0f));
        }

        [Test]
        public void TryUpdate_LargeWorldX_UsesRunOriginWithoutPatternInput()
        {
            const float largeOriginWorldX = 1000000.0f;
            Assert.That(_state.Initialize(largeOriginWorldX), Is.True);

            bool didUpdate = _state.TryUpdate(largeOriginWorldX + 100.0f);

            Assert.That(didUpdate, Is.True);
            Assert.That(_state.CurrentDistance, Is.EqualTo(100.0f));
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void TryUpdate_InvalidPosition_IsRejected(float currentWorldX)
        {
            Initialize();

            bool didUpdate = _state.TryUpdate(currentWorldX);

            Assert.That(didUpdate, Is.False);
            Assert.That(_state.CurrentDistance, Is.Zero);
        }

        [Test]
        public void TryFinalize_InitializedState_FinalizesOnce()
        {
            Initialize();
            _state.TryUpdate(OriginWorldX + 5.0f);

            bool didFinalize = _state.TryFinalize();
            bool didFinalizeAgain = _state.TryFinalize();

            Assert.That(didFinalize, Is.True);
            Assert.That(didFinalizeAgain, Is.False);
            Assert.That(_state.IsFinalized, Is.True);
            Assert.That(_state.CurrentDistance, Is.EqualTo(5.0f));
        }

        [Test]
        public void TryFinalize_BeforeInitialize_IsRejected()
        {
            bool didFinalize = _state.TryFinalize();

            Assert.That(didFinalize, Is.False);
            Assert.That(_state.IsFinalized, Is.False);
        }

        [Test]
        public void TryUpdate_AfterFinalize_IsRejectedAndKeepsFinalDistance()
        {
            Initialize();
            _state.TryUpdate(OriginWorldX + 5.0f);
            _state.TryFinalize();

            bool didUpdate = _state.TryUpdate(OriginWorldX + 10.0f);

            Assert.That(didUpdate, Is.False);
            Assert.That(_state.CurrentDistance, Is.EqualTo(5.0f));
        }

        [Test]
        public void Reset_InitializedState_ClearsEntireRunState()
        {
            Initialize();
            _state.TryUpdate(OriginWorldX + 5.0f);
            _state.TryFinalize();

            _state.Reset();

            Assert.That(_state.OriginWorldX, Is.Zero);
            Assert.That(_state.CurrentDistance, Is.Zero);
            Assert.That(_state.IsInitialized, Is.False);
            Assert.That(_state.IsFinalized, Is.False);
            Assert.That(_state.TryUpdate(OriginWorldX + 10.0f), Is.False);
        }

        [Test]
        public void Initialize_AfterPreviousRun_ReplacesOriginAndClearsDistance()
        {
            Initialize();
            _state.TryUpdate(OriginWorldX + 5.0f);
            _state.TryFinalize();

            bool didInitialize = _state.Initialize(100.0f);

            Assert.That(didInitialize, Is.True);
            Assert.That(_state.OriginWorldX, Is.EqualTo(100.0f));
            Assert.That(_state.CurrentDistance, Is.Zero);
            Assert.That(_state.IsFinalized, Is.False);
        }

        private void Initialize()
        {
            Assert.That(_state.Initialize(OriginWorldX), Is.True);
        }
    }
}
