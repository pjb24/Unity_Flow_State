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
            Assert.That(_runtimeData.CurrentDifficultyLevel, Is.Zero);
            Assert.That(_runtimeData.IsInitialized, Is.False);
            Assert.That(_runtimeData.IsFinalized, Is.False);
        }

        [Test]
        public void Initialize_NewData_CreatesEmptyRunState()
        {
            _runtimeData.Initialize();

            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.CurrentDifficultyLevel, Is.EqualTo(1));
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
        public void TryUpdateDifficultyLevel_AdvancesAndPreservesFinalValue()
        {
            _runtimeData.Initialize();

            Assert.That(_runtimeData.TryUpdateDifficultyLevel(2), Is.True);
            Assert.That(_runtimeData.TryUpdateDifficultyLevel(3), Is.True);
            Assert.That(_runtimeData.TryUpdateDifficultyLevel(2), Is.False);

            _runtimeData.TryFinalize();
            Assert.That(_runtimeData.TryUpdateDifficultyLevel(3), Is.False);
            Assert.That(_runtimeData.CurrentDifficultyLevel, Is.EqualTo(3));

            _runtimeData.Clear();
            Assert.That(_runtimeData.CurrentDifficultyLevel, Is.Zero);
            _runtimeData.Initialize();
            Assert.That(_runtimeData.CurrentDifficultyLevel, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(4)]
        public void TryUpdateDifficultyLevel_InvalidValue_IsRejected(
            int difficultyLevel)
        {
            _runtimeData.Initialize();

            Assert.That(_runtimeData.TryUpdateDifficultyLevel(difficultyLevel),
                Is.False);
            Assert.That(_runtimeData.CurrentDifficultyLevel, Is.EqualTo(1));
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

        [Test]
        public void CurrentVersionUpdate_StoresCompleteScoreAndMomentumState()
        {
            Assert.That(_runtimeData.Initialize(ScoringVersion.Current), Is.True);

            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, 12.5f, 125, 25, 150, 30, 180,
                1.5, 2.0, 4.0, 9.5), Is.True);

            Assert.That(_runtimeData.ScoringVersion, Is.EqualTo(ScoringVersion.Current));
            Assert.That(_runtimeData.CurrentDistance, Is.EqualTo(12.5f));
            Assert.That(_runtimeData.BaseDistanceScore, Is.EqualTo(125));
            Assert.That(_runtimeData.MomentumBonus, Is.EqualTo(25));
            Assert.That(_runtimeData.CurrentScore, Is.EqualTo(150));
            Assert.That(_runtimeData.CollectibleScore, Is.EqualTo(30));
            Assert.That(_runtimeData.TotalScore, Is.EqualTo(180));
            Assert.That(_runtimeData.CurrentMomentumMultiplier, Is.EqualTo(1.5));
            Assert.That(_runtimeData.MaximumMomentumMultiplier, Is.EqualTo(2.0));
            Assert.That(_runtimeData.MomentumRemainingDuration, Is.EqualTo(4.0));
            Assert.That(_runtimeData.MomentumDuration, Is.EqualTo(9.5));
            Assert.That(_runtimeData.MomentumRemainingRatio,
                Is.EqualTo(4.0 / 9.5).Within(0.0000001));
        }

        [TestCase(ScoringVersion.None)]
        [TestCase(ScoringVersion.LegacyDistanceScore)]
        [TestCase(3)]
        public void CurrentVersionUpdate_VersionMismatchIsRejected(int version)
        {
            _runtimeData.Initialize(ScoringVersion.Current);

            Assert.That(_runtimeData.TryUpdate(
                version, 12.5f, 125, 25, 150, 30, 180,
                1.5, 2.0, 4.0, 9.5), Is.False);
            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.TotalScore, Is.Zero);
        }

        [Test]
        public void CurrentVersionUpdate_InconsistentComponentsAreRejectedAtomically()
        {
            _runtimeData.Initialize(ScoringVersion.Current);

            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, 12.5f, 125, 25, 149, 30, 179,
                1.5, 2.0, 4.0, 9.5), Is.False);
            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, 12.5f, 125, 25, 150, 30, 179,
                1.5, 2.0, 4.0, 9.5), Is.False);
            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, 12.5f, 125, 25, 150, 30, 180,
                3.25, 3.25, 4.0, 9.5), Is.False);
            Assert.That(_runtimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.TotalScore, Is.Zero);
        }

        [Test]
        public void VersionSpecificEntryPoints_RejectTheOtherScoreContract()
        {
            _runtimeData.Initialize(ScoringVersion.Current);
            Assert.That(_runtimeData.TryUpdate(10.0f, 100), Is.False);
            _runtimeData.Clear();
            _runtimeData.Initialize();
            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, 10.0f, 100, 0, 100, 0, 100,
                1.0, 1.0, 0.0, 0.0), Is.False);
            Assert.That(_runtimeData.TryUpdate(10.0f, 100), Is.True);
        }

        [Test]
        public void CurrentVersionUpdate_SaturatesComponentsAndRejectsPostFinalizeChange()
        {
            _runtimeData.Initialize(ScoringVersion.Current);
            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, float.MaxValue,
                int.MaxValue, int.MaxValue, int.MaxValue,
                int.MaxValue, int.MaxValue,
                3.0, 3.0, 6.5, 6.5), Is.True);
            Assert.That(_runtimeData.TryFinalize(), Is.True);
            Assert.That(_runtimeData.TryUpdate(
                ScoringVersion.Current, float.MaxValue,
                int.MaxValue, int.MaxValue, int.MaxValue,
                int.MaxValue, int.MaxValue,
                3.0, 3.0, 6.5, 6.5), Is.False);
            Assert.That(_runtimeData.TotalScore, Is.EqualTo(int.MaxValue));
        }
    }
}
