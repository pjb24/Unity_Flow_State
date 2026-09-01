using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class InfiniteModeRuntimeDataTests
    {
        private InfiniteModeRuntimeData _runtimeData;

        [SetUp]
        public void SetUp()
        {
            _runtimeData = new InfiniteModeRuntimeData();
        }

        [Test]
        public void NewData_HasDefaultState()
        {
            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.IsInitialized, Is.False);
            Assert.That(_runtimeData.IsFinalized, Is.False);
        }

        [Test]
        public void Initialize_NewData_CreatesEmptyRunState()
        {
            _runtimeData.Initialize();

            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.IsInitialized, Is.True);
            Assert.That(_runtimeData.IsFinalized, Is.False);
        }

        [Test]
        public void TryUpdate_BeforeInitialize_IsRejected()
        {
            bool didUpdate = _runtimeData.TryUpdate(10.0f, 100);

            Assert.That(didUpdate, Is.False);
            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
        }

        [Test]
        public void TryUpdate_ValidProgress_StoresSharedRunValues()
        {
            _runtimeData.Initialize();

            bool didUpdate = _runtimeData.TryUpdate(10.5f, 105);

            Assert.That(didUpdate, Is.True);
            Assert.That(_runtimeData.CurrentDistance, Is.EqualTo(10.5f));
            Assert.That(_runtimeData.CurrentScore, Is.EqualTo(105));
        }

        [TestCase(-0.001f, 0)]
        [TestCase(float.NaN, 0)]
        [TestCase(float.PositiveInfinity, 0)]
        [TestCase(float.NegativeInfinity, 0)]
        [TestCase(0.0f, -1)]
        public void TryUpdate_InvalidProgress_IsRejected(
            float distance,
            int score)
        {
            _runtimeData.Initialize();

            bool didUpdate = _runtimeData.TryUpdate(distance, score);

            Assert.That(didUpdate, Is.False);
            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
        }

        [TestCase(9.999f, 100)]
        [TestCase(10.0f, 99)]
        public void TryUpdate_DecreasedProgress_IsRejected(
            float distance,
            int score)
        {
            _runtimeData.Initialize();
            _runtimeData.TryUpdate(10.0f, 100);

            bool didUpdate = _runtimeData.TryUpdate(distance, score);

            Assert.That(didUpdate, Is.False);
            Assert.That(_runtimeData.CurrentDistance, Is.EqualTo(10.0f));
            Assert.That(_runtimeData.CurrentScore, Is.EqualTo(100));
        }

        [Test]
        public void TryFinalize_InitializedData_FinalizesOnce()
        {
            _runtimeData.Initialize();
            _runtimeData.TryUpdate(10.0f, 100);

            bool didFinalize = _runtimeData.TryFinalize();
            bool didFinalizeAgain = _runtimeData.TryFinalize();

            Assert.That(didFinalize, Is.True);
            Assert.That(didFinalizeAgain, Is.False);
            Assert.That(_runtimeData.IsFinalized, Is.True);
            Assert.That(_runtimeData.CurrentDistance, Is.EqualTo(10.0f));
            Assert.That(_runtimeData.CurrentScore, Is.EqualTo(100));
        }

        [Test]
        public void TryFinalize_BeforeInitialize_IsRejected()
        {
            bool didFinalize = _runtimeData.TryFinalize();

            Assert.That(didFinalize, Is.False);
            Assert.That(_runtimeData.IsFinalized, Is.False);
        }

        [Test]
        public void TryUpdate_AfterFinalize_IsRejectedAndKeepsFinalValues()
        {
            _runtimeData.Initialize();
            _runtimeData.TryUpdate(10.0f, 100);
            _runtimeData.TryFinalize();

            bool didUpdate = _runtimeData.TryUpdate(20.0f, 200);

            Assert.That(didUpdate, Is.False);
            Assert.That(_runtimeData.CurrentDistance, Is.EqualTo(10.0f));
            Assert.That(_runtimeData.CurrentScore, Is.EqualTo(100));
        }

        [Test]
        public void Clear_InitializedData_RemovesRunState()
        {
            _runtimeData.Initialize();
            _runtimeData.TryUpdate(10.0f, 100);
            _runtimeData.TryFinalize();

            _runtimeData.Clear();

            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.IsInitialized, Is.False);
            Assert.That(_runtimeData.IsFinalized, Is.False);
            Assert.That(_runtimeData.TryUpdate(20.0f, 200), Is.False);
        }

        [Test]
        public void Initialize_AfterClear_CreatesEmptyNextRun()
        {
            _runtimeData.Initialize();
            _runtimeData.TryUpdate(10.0f, 100);
            _runtimeData.TryFinalize();
            _runtimeData.Clear();

            _runtimeData.Initialize();

            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.IsInitialized, Is.True);
            Assert.That(_runtimeData.IsFinalized, Is.False);
        }
    }
}
