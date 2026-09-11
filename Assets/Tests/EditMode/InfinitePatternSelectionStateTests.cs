using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class InfinitePatternSelectionStateTests
    {
        [Test]
        public void StartRun_UsesFlatAsFirstPattern()
        {
            InfinitePatternSelectionState state = CreateFactoryState();

            Assert.That(state.StartRun(123), Is.True);
            Assert.That(
                state.CurrentPatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.FlatId));
            Assert.That(state.ConsecutiveSelectionCount, Is.EqualTo(1));
        }

        [Test]
        public void TrySelectNext_D1_NeverSelectsHigherDifficultyPattern()
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(123);

            for (int requestId = 0; requestId < 50; requestId++)
            {
                Assert.That(state.TrySelectNext(
                    requestId,
                    E_InfinitePatternDifficulty.D1,
                    out string selectedId), Is.True);
                Assert.That(
                    selectedId == InfinitePatternCatalogFactory.FlatId ||
                    selectedId == InfinitePatternCatalogFactory.SingleRiseId,
                    Is.True);
            }
        }

        [Test]
        public void TrySelectNext_NewDifficultyUnlocksItsPattern()
        {
            InfinitePatternCatalog catalog = CreateCatalog(
                CreateDefinition("Flat", E_InfinitePatternDifficulty.D1),
                CreateDefinition("D2Only", E_InfinitePatternDifficulty.D2));
            InfinitePatternSelectionState state = CreateState(catalog);
            state.StartRun(9);
            state.TrySelectNext(0, E_InfinitePatternDifficulty.D1, out _);

            Assert.That(state.TrySelectNext(
                1,
                E_InfinitePatternDifficulty.D2,
                out string selectedId), Is.True);
            Assert.That(selectedId, Is.EqualTo("D2Only"));
        }

        [Test]
        public void TrySelectNext_IncompatiblePattern_IsExcluded()
        {
            InfinitePatternCatalog catalog = CreateCatalog(
                CreateDefinition("Flat", E_InfinitePatternDifficulty.D1),
                CreateDefinition(
                    "Incompatible",
                    E_InfinitePatternDifficulty.D1,
                    1.5f,
                    0.5f));
            InfinitePatternSelectionState state = CreateState(catalog);
            state.StartRun(4);

            Assert.That(state.TrySelectNext(
                0,
                E_InfinitePatternDifficulty.D1,
                out string selectedId), Is.True);
            Assert.That(selectedId, Is.EqualTo("Flat"));
        }

        [Test]
        public void TrySelectNext_OnlyCandidate_SelectsCandidate()
        {
            InfinitePatternSelectionState state = CreateState(CreateCatalog(
                CreateDefinition("Flat", E_InfinitePatternDifficulty.D1)));
            state.StartRun(1);

            Assert.That(state.TrySelectNext(
                0,
                E_InfinitePatternDifficulty.D1,
                out string selectedId), Is.True);
            Assert.That(selectedId, Is.EqualTo("Flat"));
        }

        [Test]
        public void TrySelectNext_RegularCandidatesExhausted_UsesFlatFallback()
        {
            InfinitePatternSelectionState state = CreateState(CreateCatalog(
                CreateDefinition("Flat", E_InfinitePatternDifficulty.D1)));
            state.StartRun(1);
            state.TrySelectNext(0, E_InfinitePatternDifficulty.D1, out _);

            Assert.That(state.TrySelectNext(
                1,
                E_InfinitePatternDifficulty.D3,
                out string selectedId), Is.True);
            Assert.That(selectedId, Is.EqualTo("Flat"));
            Assert.That(state.ConsecutiveSelectionCount, Is.EqualTo(3));
        }

        [Test]
        public void TrySelectNext_WhenAlternativeExists_RespectsRepeatLimit()
        {
            InfinitePatternSelectionState state = CreateState(CreateCatalog(
                CreateDefinition("Flat", E_InfinitePatternDifficulty.D1),
                CreateDefinition("Other", E_InfinitePatternDifficulty.D1)));
            state.StartRun(88);
            string previousId = state.CurrentPatternId;
            int consecutiveCount = 1;

            for (int requestId = 0; requestId < 100; requestId++)
            {
                state.TrySelectNext(
                    requestId,
                    E_InfinitePatternDifficulty.D1,
                    out string selectedId);
                consecutiveCount = selectedId == previousId
                    ? consecutiveCount + 1
                    : 1;
                Assert.That(
                    consecutiveCount,
                    Is.LessThanOrEqualTo(
                        InfinitePatternSelectionState.MaximumConsecutiveSelections));
                previousId = selectedId;
            }
        }

        [Test]
        public void SameCatalogSeedAndRequests_ProducesSameSequence()
        {
            InfinitePatternSelectionState first = CreateFactoryState();
            InfinitePatternSelectionState second = CreateFactoryState();
            first.StartRun(20260910);
            second.StartRun(20260910);

            for (int requestId = 0; requestId < 30; requestId++)
            {
                first.TrySelectNext(
                    requestId,
                    E_InfinitePatternDifficulty.D3,
                    out string firstId);
                second.TrySelectNext(
                    requestId,
                    E_InfinitePatternDifficulty.D3,
                    out string secondId);
                Assert.That(secondId, Is.EqualTo(firstId));
            }
        }

        [Test]
        public void TrySelectNext_DuplicateRequest_IsRejectedWithoutMutation()
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(11);
            state.TrySelectNext(5, E_InfinitePatternDifficulty.D3, out _);
            string selectedId = state.CurrentPatternId;
            int repeatCount = state.ConsecutiveSelectionCount;

            Assert.That(state.TrySelectNext(
                5,
                E_InfinitePatternDifficulty.D3,
                out _), Is.False);
            Assert.That(state.CurrentPatternId, Is.EqualTo(selectedId));
            Assert.That(state.ConsecutiveSelectionCount, Is.EqualTo(repeatCount));
        }

        [Test]
        public void TrySelectNext_DifferentRequestId_IsAcceptedAfterDuplicate()
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(11);
            state.TrySelectNext(5, E_InfinitePatternDifficulty.D3, out _);
            state.TrySelectNext(5, E_InfinitePatternDifficulty.D3, out _);

            Assert.That(state.TrySelectNext(
                4,
                E_InfinitePatternDifficulty.D3,
                out _), Is.True);
        }

        [Test]
        public void TrySelectNext_DuringPause_IsRejectedAndStateIsPreserved()
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(11);
            string selectedId = state.CurrentPatternId;

            Assert.That(state.Pause(), Is.True);
            Assert.That(state.TrySelectNext(
                0,
                E_InfinitePatternDifficulty.D3,
                out _), Is.False);
            Assert.That(state.CurrentPatternId, Is.EqualTo(selectedId));
            Assert.That(state.Resume(), Is.True);
            Assert.That(state.TrySelectNext(
                0,
                E_InfinitePatternDifficulty.D3,
                out _), Is.True);
        }

        [Test]
        public void TrySelectNext_AfterResult_IsRejectedAndStateIsPreserved()
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(11);
            string selectedId = state.CurrentPatternId;

            Assert.That(state.EndRun(), Is.True);
            Assert.That(state.TrySelectNext(
                0,
                E_InfinitePatternDifficulty.D3,
                out _), Is.False);
            Assert.That(state.CurrentPatternId, Is.EqualTo(selectedId));
        }

        [Test]
        public void StartRun_AfterResult_ResetsHistoryAndRequestState()
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(10);
            state.TrySelectNext(20, E_InfinitePatternDifficulty.D3, out _);
            state.EndRun();

            Assert.That(state.StartRun(99), Is.True);
            Assert.That(state.CurrentPatternId, Is.EqualTo("Flat"));
            Assert.That(state.ConsecutiveSelectionCount, Is.EqualTo(1));
            Assert.That(state.TrySelectNext(
                0,
                E_InfinitePatternDifficulty.D1,
                out _), Is.True);
        }

        [Test]
        public void Initialize_FlatCannotServeAsFallback_IsRejected()
        {
            InfinitePatternCatalog catalog = CreateCatalog(
                CreateDefinition("Flat", E_InfinitePatternDifficulty.D1),
                CreateDefinition(
                    "CannotConnectToFlat",
                    E_InfinitePatternDifficulty.D1,
                    0.5f,
                    1.5f));
            InfinitePatternSelectionState state = new InfinitePatternSelectionState();

            Assert.That(state.Initialize(catalog), Is.False);
            Assert.That(state.IsInitialized, Is.False);
        }

        [TestCase(E_InfinitePatternDifficulty.None)]
        [TestCase((E_InfinitePatternDifficulty)4)]
        public void TrySelectNext_InvalidDifficulty_IsRejected(
            E_InfinitePatternDifficulty difficulty)
        {
            InfinitePatternSelectionState state = CreateFactoryState();
            state.StartRun(1);

            Assert.That(state.TrySelectNext(0, difficulty, out _), Is.False);
            Assert.That(state.CurrentPatternId, Is.EqualTo("Flat"));
        }

        private static InfinitePatternSelectionState CreateFactoryState()
        {
            Assert.That(
                InfinitePatternCatalogFactory.TryCreate(
                    out InfinitePatternCatalog catalog),
                Is.True);
            return CreateState(catalog);
        }

        private static InfinitePatternSelectionState CreateState(
            InfinitePatternCatalog catalog)
        {
            InfinitePatternSelectionState state =
                new InfinitePatternSelectionState();
            Assert.That(state.Initialize(catalog), Is.True);
            return state;
        }

        private static InfinitePatternCatalog CreateCatalog(
            params InfinitePatternDefinition[] definitions)
        {
            InfinitePatternCatalog catalog = new InfinitePatternCatalog();
            Assert.That(catalog.Initialize(definitions), Is.True);
            return catalog;
        }

        private static InfinitePatternDefinition CreateDefinition(
            string id,
            E_InfinitePatternDifficulty minimumDifficulty,
            float startGroundTopY = 0.5f,
            float endGroundTopY = 0.5f)
        {
            InfinitePatternDefinition definition =
                new InfinitePatternDefinition();
            Assert.That(definition.Initialize(
                id,
                "Test purpose",
                minimumDifficulty,
                new Vector3(-22.0f, 0.0f, 0.0f),
                new Vector3(22.0f, 0.0f, 0.0f),
                Vector3.right,
                Vector3.right,
                2.0f,
                2.0f,
                startGroundTopY,
                endGroundTopY,
                4.0f,
                4.0f), Is.True);
            return definition;
        }
    }
}
