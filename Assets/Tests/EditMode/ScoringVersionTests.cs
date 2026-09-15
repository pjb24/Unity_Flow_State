using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class ScoringVersionTests
    {
        [Test]
        public void Constants_DistinguishInvalidLegacyAndCurrentRules()
        {
            Assert.That(ScoringVersion.None, Is.Zero);
            Assert.That(ScoringVersion.LegacyDistanceScore, Is.EqualTo(1));
            Assert.That(ScoringVersion.Current, Is.EqualTo(2));
            Assert.That(ScoringVersion.IsSupported(1), Is.True);
            Assert.That(ScoringVersion.IsSupported(2), Is.True);
            Assert.That(ScoringVersion.IsSupported(0), Is.False);
            Assert.That(ScoringVersion.IsSupported(3), Is.False);
        }

        [Test]
        public void InfiniteRuntimeData_CurrentVersion_IsFixedUntilClear()
        {
            InfiniteModeRuntimeData runtimeData =
                new InfiniteModeRuntimeData();

            Assert.That(runtimeData.Initialize(ScoringVersion.Current), Is.True);
            Assert.That(runtimeData.ScoringVersion,
                Is.EqualTo(ScoringVersion.Current));
            Assert.That(
                runtimeData.Initialize(ScoringVersion.LegacyDistanceScore),
                Is.False);
            Assert.That(runtimeData.ScoringVersion,
                Is.EqualTo(ScoringVersion.Current));

            runtimeData.Clear();
            Assert.That(runtimeData.ScoringVersion, Is.EqualTo(ScoringVersion.None));
        }

        [Test]
        public void InfiniteRuntimeData_LegacyEntryPoint_RemainsVersionOne()
        {
            InfiniteModeRuntimeData runtimeData =
                new InfiniteModeRuntimeData();

            runtimeData.Initialize();

            Assert.That(runtimeData.ScoringVersion,
                Is.EqualTo(ScoringVersion.LegacyDistanceScore));
        }

        [Test]
        public void ResultData_StageAndLegacyConstructorsUseExpectedVersions()
        {
            ResultData stage = new ResultData(
                E_StageResultType.Cleared,
                1.0,
                0);
            ResultData legacy = new ResultData(10.0f, 100, 0, 100);

            Assert.That(stage.ScoringVersion, Is.EqualTo(ScoringVersion.None));
            Assert.That(legacy.ScoringVersion,
                Is.EqualTo(ScoringVersion.LegacyDistanceScore));
        }

        [Test]
        public void ScoreRecord_CurrentVersion_TransfersCompleteScoreContract()
        {
            ScoreRecord record = new ScoreRecord();

            Assert.That(
                record.TryRecord(
                    ScoringVersion.Current,
                    E_GameMode.Infinite,
                    true,
                    true,
                    12.0f,
                    120,
                    30,
                    150,
                    10,
                    2.0),
                Is.True);

            ResultData result = record.ResultData;
            Assert.That(result.ScoringVersion,
                Is.EqualTo(ScoringVersion.Current));
            Assert.That(result.BaseDistanceScore, Is.EqualTo(120));
            Assert.That(result.MomentumBonus, Is.EqualTo(30));
            Assert.That(result.DistanceScore, Is.EqualTo(150));
            Assert.That(result.CollectibleScore, Is.EqualTo(10));
            Assert.That(result.TotalScore, Is.EqualTo(160));
            Assert.That(result.MaximumMomentumMultiplier, Is.EqualTo(2.0));
        }

        [TestCase(ScoringVersion.None)]
        [TestCase(ScoringVersion.LegacyDistanceScore)]
        [TestCase(3)]
        public void ScoreRecord_NonCurrentVersion_IsRejected(int version)
        {
            ScoreRecord record = new ScoreRecord();

            Assert.That(
                record.TryRecord(
                    version,
                    E_GameMode.Infinite,
                    true,
                    true,
                    12.0f,
                    120,
                    30,
                    150,
                    10,
                    2.0),
                Is.False);
            Assert.That(record.HasRecord, Is.False);
        }

        [Test]
        public void ScoreRecord_MismatchedDistanceComponents_IsRejected()
        {
            ScoreRecord record = new ScoreRecord();

            Assert.That(
                record.TryRecord(
                    ScoringVersion.Current,
                    E_GameMode.Infinite,
                    true,
                    true,
                    12.0f,
                    120,
                    30,
                    149,
                    10,
                    2.0),
                Is.False);
            Assert.That(record.HasRecord, Is.False);
        }
    }
}
